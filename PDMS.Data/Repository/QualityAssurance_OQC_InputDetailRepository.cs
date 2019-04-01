using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Common.Helpers;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace PDMS.Data.Repository
{

    public class QualityAssurance_OQC_InputDetailRepository : RepositoryBase<OQC_InputMaster>, IQualityAssurance_OQC_InputDetailRepository
    {
        private Logger log = new Logger("QualityAssurance_OQC_InputDetailRepository");

        public QualityAssurance_OQC_InputDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<OQC_InputDetailVM> QueryOQCExceptionDetails(QAReportSearchVM searchModel, string typeClassify)
        {
            List<OQC_InputDetailVM> result = new List<OQC_InputDetailVM>();
            try
            {
                bool searchHistory = false;
                TimeSpan tSpan = DateTime.Now.Date - searchModel.ProductDate;
                if (tSpan.Days >= 7)
                {
                    searchHistory = true;
                }
                string sql = string.Empty;

                sql = string.Format(@"
                                        SELECT  detai.OQCMater_UID,
		                                        detai.OQCDetail_UID,
		                                        detai.FunPlant,
		                                        detai.TypeClassify,
		                                        detai.Qty,
		                                        detai.ExceptionType_UID,
		                                        Etype.TypeName AS ExcetionTypeName
                                        INTO     #temp
                                        FROM    dbo.{7} detai WITH ( NOLOCK )
		                                        INNER JOIN dbo.QualityAssurance_ExceptionType Etype WITH(NOLOCK) ON Etype.ExceptionType_UID = detai.ExceptionType_UID
                                                INNER JOIN dbo.{0} OQCMa WITH ( NOLOCK ) ON OQCMa.OQCMater_UID = detai.OQCMater_UID
                                                INNER JOIN dbo.System_Project SP WITH ( NOLOCK ) ON SP.Project_UID = OQCMa.Project_UID
                                        WHERE   FlowChart_Detail_UID = {1}
                                                AND SP.Project_Name = N'{2}'
                                                AND OQCMa.Color = N'{3}'
                                                AND OQCMa.MaterialType = N'{4}'
                                                AND ProductDate = N'{5}'
                                                AND Time_interval = N'{6}'

                                        DECLARE @DefectRate TABLE
                                        (
                                           ExceptionType_UID INT ,
                                           DailyRate DECIMAL(8, 6) ,
                                           NightRate DECIMAL(8, 6) ,
                                           DayRate DECIMAL(8, 6) 
                                        )	
    
                                        INSERT INTO @DefectRate
				                                    (  DailyRate,
				                                      DayRate ,
				                                      NightRate,
				                                      ExceptionType_UID)
                                        EXEC dbo.usp_CalculateDefectRate N'{5}', N'{4}', N'{3}',N'{8}',N'{6}',N'{10}'
	
                                        DECLARE @CanModify BIT
                                        IF EXISTS (SELECT TOP 1 1 FROM #temp)
                                        BEGIN
                                            SET @CanModify=0
                                        END  
                                        ELSE IF EXISTS(SELECT TOP 1 1
                                                        FROM    dbo.{0} OQCMa WITH ( NOLOCK )
                                                        WHERE   OQCMa.Color = N'{3}'
                                                                AND FlowChart_Detail_UID = {1}
                                                                AND OQCMa.MaterialType = N'{4}'
                                                                AND ProductDate = N'{5}'
                                                                AND Time_interval = N'{6}')
                                        BEGIN
										     SET @CanModify=0
                                        END  
                                        ElSE
                                        BEGIN
                                             SET @CanModify=1    
                                        END
                            
                                        SELECT  0 AS OQCMater_UID ,
                                                0 AS OQCDetail_UID ,
                                                FunPlant ,
                                                EWF.TypeClassify ,
                                                0 AS QTY ,
                                                EType.TypeName AS ExcetionTypeName,
                                                EType.ExceptionType_UID,
                                                @CanModify AS CanModify,
                                                ISNULL(rate.DailyRate,0) AS DailyRate,
												ISNULL(rate.NightRate,0) AS NightRate,
												ISNULL(rate.DayRate,0) AS DayRate
                                        FROM    dbo.ExceptionTypeWithFlowchart EWF WITH ( NOLOCK )
                                                INNER JOIN dbo.QualityAssurance_ExceptionType EType WITH (NOLOCK) ON EType.ExceptionType_UID = EWF.ExceptionType_UID
                                                Left JOIN @DefectRate rate ON rate.ExceptionType_UID = EType.ExceptionType_UID 
                                        WHERE   EWF.FlowChart_Detail_UID = {1}
                                                AND EWF.FlowChart_Master_UID = {9}
                                                AND ewf.TypeClassify = N'{8}'
                                                AND ewf.ExceptionType_UID NOT IN (SELECT ExceptionType_UID FROM #temp)
                                        UNION

                                        SELECT  OQCMater_UID,
		                                        OQCDetail_UID,
		                                        FunPlant,
		                                        TypeClassify,
		                                        Qty,
		                                        ExcetionTypeName,
                                                t.ExceptionType_UID,
                                                @CanModify AS CanModify ,
                                                ISNULL(rate.DailyRate,0) AS DailyRate,
												ISNULL(rate.NightRate,0) AS NightRate,
												ISNULL(rate.DayRate,0) AS DayRate
                                        FROM    #temp t
				                                Left JOIN @DefectRate rate ON rate.ExceptionType_UID = t.ExceptionType_UID 
                                        Where   TypeClassify=N'{8}'
                                        ORDER BY FunPlant,ExceptionType_UID
                                        DROP TABLE #temp", !searchHistory ? "OQC_InputMaster" : "OQC_InputMaster_History", searchModel.FlowChart_Detail_UID,
                                         searchModel.ProjectName, searchModel.Color, searchModel.MaterialType, searchModel.ProductDate.ToShortDateString(), searchModel.Time_interval,
                                         !searchHistory ? "OQC_InputDetail" : "OQC_InputDetail_History", typeClassify, searchModel.FlowChart_Master_UID,searchModel.FlowChart_Detail_UID);

                var query = DataContext.Database.SqlQuery<OQC_InputDetailVM>(sql).ToList();

                result = query;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<OQC_InputDetailVM> QueryOQCExceptionRecord(QAReportSearchVM searchModel, string Classify)
        {
            if (searchModel.Tab_Select_Text == "夜班小计")
                searchModel.Tab_Select_Text = "Night_Sum";
            else if (searchModel.Tab_Select_Text == "白班小计")
                searchModel.Tab_Select_Text = "Daily_Sum";
            else if (searchModel.Tab_Select_Text == "全天")
                searchModel.Tab_Select_Text = "ALL";
            if (searchModel.Tab_Select_Text == "0")
            {
                searchModel.Time_interval = "ALL";
                searchModel.Tab_Select_Text = "ALL";
            }

            List<OQC_InputDetailVM> result = new List<OQC_InputDetailVM>();
            try
            {
                var ProductDate = new SqlParameter("ProductDate", searchModel.ProductDate.ToShortDateString());
                var MaterialType = new SqlParameter("MaterialType", searchModel.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color);
                var typeClassify = new SqlParameter("typeClassify ", Classify);
                var Time_Interval = new SqlParameter("TimeInterval", searchModel.Tab_Select_Text);
                var Flowchart_Detail_UID = new SqlParameter("Flowchart_Detail_UID", searchModel.FlowChart_Detail_UID);
                IEnumerable<OQC_InputDetailVM> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<OQC_InputDetailVM>("dbo.usp_QueryQAHistory_OQC @ProductDate, @MaterialType, @Color,@typeClassify,@TimeInterval,@Flowchart_Detail_UID", ProductDate, MaterialType, Color, typeClassify, Time_Interval, Flowchart_Detail_UID).ToArray();

                result = tempResult.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

    }



    public interface IQualityAssurance_OQC_InputDetailRepository : IRepository<OQC_InputMaster>
    {
        List<OQC_InputDetailVM> QueryOQCExceptionDetails(QAReportSearchVM searchModel, string typeClassify);
        List<OQC_InputDetailVM> QueryOQCExceptionRecord(QAReportSearchVM searchModel, string Classify);
    }



}
