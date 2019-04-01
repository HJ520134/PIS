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
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{

    public class QualityAssurance_OQC_InputMasterRepository : RepositoryBase<OQC_InputMaster>, IQualityAssurance_OQC_InputMasterRepository
    {
        private Logger log = new Logger("QualityAssurance_OQC_InputMasterRepository");

        public QualityAssurance_OQC_InputMasterRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }


        public OQC_InputMasterDTO QueryOQCMasterData(QAReportSearchVM searchModel)
        {
            OQC_InputMasterDTO result = new OQC_InputMasterDTO();
            try
            {
                bool searchHistory = false;

                TimeSpan tSpan = DateTime.Now.Date - searchModel.ProductDate;
                if (tSpan.Days >= 7)
                {
                    searchHistory = true;
                }

                string sql = string.Format(@"
                                        SELECT  OQCMa.*
                                        FROM    dbo.{0} OQCMa WITH ( NOLOCK )
                                                INNER JOIN dbo.System_Project SP WITH ( NOLOCK ) ON SP.Project_UID = OQCMa.Project_UID
                                        WHERE   FlowChart_Detail_UID = {1}
                                                AND SP.Project_Name = N'{2}'
                                                AND OQCMa.Color = N'{3}'
                                                AND OQCMa.MaterialType = N'{4}'
                                                AND ProductDate = N'{5}'
                                                AND Time_interval = N'{6}' 
                                        ", !searchHistory ? "OQC_InputMaster" : "OQC_InputMaster_History", searchModel.FlowChart_Detail_UID,
                                                searchModel.ProjectName, searchModel.Color, searchModel.MaterialType, searchModel.ProductDate.ToShortDateString(), searchModel.Time_interval);

                var query = DataContext.Database.SqlQuery<OQC_InputMasterDTO>(sql).ToList();
                if (query.Count == 0)
                {
                    var query1 = from QAinterface in DataContext.PPForQAInterface
                                 join flowDetails in DataContext.FlowChart_Detail on QAinterface.FlowChart_Detail_UID equals flowDetails.FlowChart_Detail_UID
                                 join flowMaster in DataContext.FlowChart_Master on flowDetails.FlowChart_Master_UID equals flowMaster.FlowChart_Master_UID
                                 where QAinterface.Color == searchModel.Color &&
                                       QAinterface.Product_Date == searchModel.ProductDate &&
                                       QAinterface.Time_Interval == searchModel.Time_interval && QAinterface.MaterielType == searchModel.MaterialType
                                       && QAinterface.QAUsedFlag == false && QAinterface.FlowChart_Detail_UID == searchModel.FlowChart_Detail_UID
                                       && flowDetails.FlowChart_Version == flowMaster.FlowChart_Version
                                 select new OQC_InputMasterDTO
                                 {
                                     Input = QAinterface.Input_Qty,
                                     GoodParts_Qty = QAinterface.Good_Qty,
                                     NGParts_Qty = QAinterface.NG_Qty,
                                     Rework = QAinterface.ReWorkQty,
                                     ProductDate = searchModel.ProductDate,
                                     Time_interval = searchModel.Time_interval,
                                     FlowChart_Detail_UID = searchModel.FlowChart_Detail_UID,
                                     Color = searchModel.Color,
                                     MaterialType = searchModel.MaterialType,
                                     Project_UID = flowMaster.Project_UID
                                 };
                    if (query1.Count() > 0)
                    {
                        result = query1.ToList()[0];
                    }
                    else
                    {
                        var query2 = from flowDetails in DataContext.FlowChart_Detail
                                     join flowMaster in DataContext.FlowChart_Master on flowDetails.FlowChart_Master_UID equals flowMaster.FlowChart_Master_UID
                                     where flowDetails.FlowChart_Detail_UID == searchModel.FlowChart_Detail_UID
                                           && flowDetails.FlowChart_Version == flowMaster.FlowChart_Version
                                     select new OQC_InputMasterDTO
                                     {

                                         ProductDate = searchModel.ProductDate,
                                         Time_interval = searchModel.Time_interval,
                                         FlowChart_Detail_UID = searchModel.FlowChart_Detail_UID,
                                         Color = searchModel.Color,

                                         MaterialType = searchModel.MaterialType,
                                         Project_UID = flowMaster.Project_UID
                                     };

                        if (query2.Count() > 0)
                        {
                            result = query2.ToList()[0];
                        }

                    }

                }
                else
                {
                    result = query[0];
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public string SaveOQCData(OQCInputData data)
        {
            string result = "Success";

            try
            {
                if (data == null)
                {
                    return result;
                }

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    string MasterSql = "";
                    int OQCMasterUID = 0;

                    try
                    {
                        if (data.MasterData.OQCMater_UID != 0)
                        {
                            #region --- Update Master
                            MasterSql = string.Format(@"UPDATE  dbo.OQC_InputMaster
                                                    SET     Input = {0} ,
                                                            GoodParts_Qty = {1} ,
                                                            NGParts_Qty = {2} ,
                                                            Rework = {3} ,
                                                            ProductLineRework = {4} ,
                                                            ReworkQtyFromAssemble = {5} ,
                                                            NG_Qty = {6} ,
                                                            Storage_Qty = {7} ,
                                                            WaitStorage_Qty = {8},
                                                            ReworkQtyFromOQC={11},
                                                            Modifier_UID={10},
                                                            Modified_date=GETDATE()                      
                                                    WHERE   OQCMater_UID = {9}", data.MasterData.Input, data.MasterData.GoodParts_Qty,
                                                        data.MasterData.NGParts_Qty, data.MasterData.Rework, data.MasterData.ProductLineRework,
                                                        data.MasterData.ReworkQtyFromAssemble, data.MasterData.NG_Qty, data.MasterData.Storage_Qty,
                                                        data.MasterData.WaitStorage_Qty, data.MasterData.OQCMater_UID, data.MasterData.Modifier_UID, data.MasterData.ReworkQtyFromOQC);
                            #endregion

                            DataContext.Database.ExecuteSqlCommand(MasterSql);
                            OQCMasterUID = data.MasterData.OQCMater_UID;
                        }
                        else
                        {
                            #region --- Insert Master
                            MasterSql = string.Format(@"INSERT INTO dbo.OQC_InputMaster
                                                        ( FlowChart_Detail_UID ,   --0
                                                          Time_interval ,               --1
                                                          ProductDate ,  ---2
                                                          MaterialType ,   --3
                                                          Color ,  --4
                                                          Input ,   --5
                                                          GoodParts_Qty ,  --6
                                                          NGParts_Qty ,   --7
                                                          Rework ,    --8
                                                          ProductLineRework ,   --9
                                                          ReworkQtyFromAssemble ,    --10
                                                          NG_Qty ,             --11
                                                          Storage_Qty ,     --12
                                                          WaitStorage_Qty ,   --13
                                                          Creator_UID ,    
                                                          Create_date ,
                                                          Modified_date ,
                                                          Modifier_UID ,
                                                          Project_UID,
                                                          WIP,               --19
                                                          ReworkQtyFromOQC     
                                                        )
                                                VALUES  ( {0} , -- FlowChart_Detail_UID - int
                                                          N'{1}' , -- Time_interval - nvarchar(50)
                                                          N'{2}', -- ProductDate - date
                                                          N'{3}' , -- MaterialType - nvarchar(50)
                                                          N'{4}' , -- Color - nvarchar(20)
                                                          {5} , -- Input - int
                                                          {6} , -- GoodParts_Qty - -int  
                                                          {7} , -- NGParts_Qty - int
                                                          {8} , -- Rework - int
                                                          {9} , -- ProductLineRework - int
                                                          {10} , -- ReworkQtyFromAssemble - int
                                                          {11} , -- NG_Qty - int
                                                          {12} , -- Storage_Qty - int
                                                          {13} , -- WaitStorage_Qty - int
                                                          {14} , -- Creator_UID - int
                                                          GETDATE() , -- Create_date - datetime
                                                          GETDATE() , -- Modified_date - datetime
                                                          {14} , -- Modifier_UID - int
                                                          {15} ,  -- Project_UID - int
                                                          {16},
                                                          {17}
                                                        )
                                                 SELECT  @@IDENTITY     ", data.MasterData.FlowChart_Detail_UID, data.MasterData.Time_interval, data.MasterData.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate), data.MasterData.MaterialType,
                                                     data.MasterData.Color, data.MasterData.Input, data.MasterData.GoodParts_Qty, data.MasterData.NGParts_Qty, data.MasterData.Rework
                                                     , data.MasterData.ProductLineRework, data.MasterData.ReworkQtyFromAssemble, data.MasterData.NG_Qty, data.MasterData.Storage_Qty,
                                                     data.MasterData.WaitStorage_Qty, data.MasterData.Creator_UID, data.MasterData.Project_UID, data.MasterData.WIP, data.MasterData.ReworkQtyFromOQC);


                            #endregion

                            OQCMasterUID = int.Parse(DataContext.Database.SqlQuery<decimal>(MasterSql).ToList()[0].ToString());
                        }
                        SaveDetailData(data.DetailsData, OQCMasterUID, data.MasterData.Creator_UID);


                        var masterUID = new SqlParameter("QAMasterUID ", OQCMasterUID);

                        IEnumerable<SPReturnMessage> Tempresult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("usp_CalculateQAReportSumData_OQC  @QAMasterUID", masterUID).ToArray();

                        result = Tempresult.ToList()[0].Message;

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result = "Error";
                        log.Error(ex);
                    }


                }
            }
            catch (Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }
            return result;
        }

        void SaveDetailData(List<OQC_InputDetailVM> DetailsData, int OQCMasterUID, int uid)
        {
            //全插操作
            foreach (var data in DetailsData)
            {
                string DetailSql = "";
                if (data.OQCDetail_UID != 0)
                {
                    DetailSql = string.Format(@"UPDATE dbo.OQC_InputDetail 
                                                SET    Qty= {0},Modified_UID={1},Modified_Date=GETDATE()  
                                                WHERE  OQCDetail_UID={2}", data.Qty, uid, data.OQCDetail_UID);
                }
                else
                {
                    DetailSql = string.Format(@"INSERT INTO dbo.OQC_InputDetail
                                                    ( ExceptionType_UID ,
                                                      OQCMater_UID ,
                                                      FunPlant ,
                                                      TypeClassify ,
                                                      Qty ,
                                                      Creator_UID ,
                                                      Create_Date ,
                                                      Modified_UID ,
                                                      Modified_Date
                                                    )
                                            VALUES  ( {0} , -- ExceptionType_UID - int
                                                      {1} , -- OQCMater_UID - int
                                                      N'{2}' , -- FunPlant - nvarchar(50)
                                                      N'{3}' , -- TypeClassify - nvarchar(50)
                                                      {4} , -- Qty - int
                                                      {5} , -- Creator_UID - int
                                                      GETDATE() , -- Create_Date - datetime
                                                      {5} , -- Modified_UID - int
                                                      GETDATE()  -- Modified_Date - datetime
                                                    )", data.ExceptionType_UID, OQCMasterUID, data.FunPlant, data.TypeClassify, data.Qty,
                                                    uid);
                }
                DataContext.Database.ExecuteSqlCommand(DetailSql);
            }
        }


        public OQC_InputMasterDTO QueryOQCRecordData(QAReportSearchVM searchModel)
        {
            OQC_InputMasterDTO result = new OQC_InputMasterDTO();

            #region
            int uid = 0;
            if (searchModel.FlowChart_Detail_UID != 0)
            {
                uid = searchModel.FlowChart_Detail_UID;
            }
            else
            {

                uid = GetDetailUID(searchModel.ProjectName, searchModel.Process, searchModel.ProductDate);
            }

            if (searchModel.Tab_Select_Text == "Night_Sum" || searchModel.Tab_Select_Text == "Daily_Sum" || searchModel.Tab_Select_Text == "ALL")
            {

                try
                {
                    bool searchHistory = false;

                    TimeSpan tSpan = DateTime.Now.Date - searchModel.ProductDate;
                    if (tSpan.Days >= 7)
                    {
                        searchHistory = true;
                    }

                    string sql = string.Format(@"
                                                      
    DECLARE @SumReworkQty INT ,
        @SumNGQTy INT ,
        @DailyEndTimeIndex INT ,
        @NightEndTimeIndex INT ,
        @StartIndex INT ,
        @EndIndex INT ,
        @TimeType VARCHAR(50) 
			
    SET @TimeType = N'{6}'

    SELECT  @DailyEndTimeIndex = CONVERT(INT, EN.Enum_Name)
    FROM    dbo.Enumeration EN WITH ( NOLOCK )
            INNER JOIN dbo.Enumeration E WITH ( NOLOCK ) ON E.Enum_Value = EN.Enum_Value
    WHERE   En.Enum_Type = 'Time_InterVal_OP1'
            AND E.Enum_Type = 'Sum_TimeInterval'
            AND E.Enum_Name = 'Daily_Sum'  
         
    SELECT  @NightEndTimeIndex = CONVERT(INT, EN.Enum_Name)
    FROM    dbo.Enumeration EN WITH ( NOLOCK )
            INNER JOIN dbo.Enumeration E WITH ( NOLOCK ) ON E.Enum_Value = EN.Enum_Value
    WHERE   En.Enum_Type = 'Time_InterVal_OP1'
            AND E.Enum_Type = 'Sum_TimeInterval'
            AND E.Enum_Name = 'Night_Sum'  
                
    IF @TimeType = N'ALL'
        BEGIN
            SET @StartIndex = 1 
            SET @EndIndex = @NightEndTimeIndex
        END
    ELSE
        IF @TimeType = N'Daily_Sum'
            BEGIN
                SET @StartIndex = 1 
                SET @EndIndex = @DailyEndTimeIndex 
            END 
        ELSE
            IF @TimeType = N'Night_Sum'
                BEGIN
                    SET @StartIndex = @DailyEndTimeIndex + 1 
                    SET @EndIndex = @NightEndTimeIndex 
                END 

    SELECT  @SumReworkQty = SUM(QaDetails.Qty)
    FROM    dbo.{5} QaDetails WITH ( NOLOCK )
            INNER JOIN dbo.{0} OQCMa WITH ( NOLOCK ) ON OQCMa.OQCMater_UID = QaDetails.OQCMater_UID
            INNER JOIN dbo.Enumeration en ON OQCMa.Time_Interval = en.Enum_Value
    WHERE   FlowChart_Detail_UID = {1}
            AND QaDetails.TypeClassify = N'返修明细'
            AND OQCMa.Color = N'{2}'
            AND OQCMa.MaterialType = N'{3}'
            AND ProductDate = N'{4}'
            AND en.Enum_Type = N'Time_InterVal_OP1'
            AND CONVERT(INT, Enum_Name) <= @EndIndex
            AND CONVERT(INT, Enum_Name) >= @StartIndex
            
    SELECT  @SumNGQTy = SUM(QaDetails.Qty)
    FROM    dbo.{5} QaDetails WITH ( NOLOCK )
            INNER JOIN dbo.{0} OQCMa WITH ( NOLOCK ) ON OQCMa.OQCMater_UID = QaDetails.OQCMater_UID
            INNER JOIN dbo.Enumeration en ON OQCMa.Time_Interval = en.Enum_Value
    WHERE   FlowChart_Detail_UID = {1}
            AND QaDetails.TypeClassify = N'不良明细'
            AND OQCMa.Color = N'{2}'
            AND OQCMa.MaterialType = N'{3}'
            AND ProductDate = N'{4}'
            AND en.Enum_Type = N'Time_InterVal_OP1'
            AND CONVERT(INT, Enum_Name) <= @EndIndex
            AND CONVERT(INT, Enum_Name) >= @StartIndex                           

    SELECT  OQCMa.FlowChart_Detail_UID ,
            OQCMa.MaterialType ,
            OQCMa.Color ,
            OQCMa.Project_UID ,
            SUM(OQCMa.Input) AS Input ,
            SUM(OQCMa.GoodParts_Qty) AS GoodParts_Qty ,
            SUM(OQCMa.NGParts_Qty) AS NGParts_Qty ,
            SUM(OQCMa.Rework) AS Rework ,
            SUM(OQCMa.ReworkQtyFromOQC) AS ReworkQtyFromOQC,
            SUM(OQCMa.ProductLineRework) AS ProductLineRework ,
            SUM(OQCMa.ReworkQtyFromAssemble) AS ReworkQtyFromAssemble ,
            SUM(OQCMa.RepairNG_Qty) AS RepairNG_Qty ,
            SUM(OQCMa.NG_Qty) AS NG_Qty ,
            @SumReworkQty * 1.00 / ( SUM(OQCMa.GoodParts_Qty)
                                     + SUM(OQCMa.NGParts_Qty)
                                     + SUM(OQCMa.Rework)
                                     + SUM(OQCMa.ProductLineRework) ) * 1.00 AS RepairNG_Yield ,
            @SumNGQTy * 1.00 / ( SUM(OQCMa.GoodParts_Qty)
                                 + SUM(OQCMa.NGParts_Qty) ) * 1.00 AS NG_Yield ,
            CASE WHEN ( SUM(OQCMa.GoodParts_Qty) + SUM(OQCMa.NGParts_Qty)
                        + SUM(OQCMa.Rework) - SUM(OQCMa.ReworkQtyFromOQC) ) > 0
                 THEN (SUM(OQCMa.GoodParts_Qty)-SUM(OQCMa.ReworkQtyFromOQC)) * 1.00
                      / ( SUM(OQCMa.GoodParts_Qty) + SUM(OQCMa.NGParts_Qty)
                        + SUM(OQCMa.Rework) - SUM(OQCMa.ReworkQtyFromOQC) )
                      * 1.00
                 ELSE 0
            END AS FirstYieldRate ,
            CASE WHEN ( SUM(OQCMa.GoodParts_Qty) + SUM(OQCMa.NGParts_Qty) ) > 0
                 THEN SUM(OQCMa.GoodParts_Qty) * 1.00
                      / ( SUM(OQCMa.GoodParts_Qty) + SUM(OQCMa.NGParts_Qty) )
                      * 1.00
                 ELSE 0
            END AS SecondYieldRate ,
            SUM(OQCMa.Storage_Qty) AS Storage_Qty ,
            SUM(OQCMa.Storage_Qty) AS Storage_Qty ,
            SUM(OQCMa.WaitStorage_Qty) AS WaitStorage_Qty ,
            0 AS WIP
    FROM    dbo.{0} OQCMa WITH ( NOLOCK )
            INNER JOIN dbo.Enumeration EN WITH ( NOLOCK ) ON OQCMa.Time_Interval = en.Enum_Value
    WHERE   FlowChart_Detail_UID = {1}
            AND OQCMa.Color = N'{2}'
            AND OQCMa.MaterialType = N'{3}'
            AND ProductDate = N'{4}'
            AND en.Enum_Type = N'Time_InterVal_OP1'
            AND CONVERT(INT, Enum_Name) <= @EndIndex
            AND CONVERT(INT, Enum_Name) >= @StartIndex
    GROUP BY OQCMa.FlowChart_Detail_UID ,
            OQCMa.MaterialType ,
            OQCMa.Project_UID ,
            OQCMa.Color
                                        ", !searchHistory ? "OQC_InputMaster" : "OQC_InputMaster_History", uid, searchModel.Color, searchModel.MaterialType, searchModel.ProductDate.ToShortDateString(),
                                                    !searchHistory ? "OQC_InputDetail" : "OQC_InputDetail_History", searchModel.Tab_Select_Text);

                    var query = DataContext.Database.SqlQuery<OQC_InputMasterDTO>(sql).ToList();

                    if (query.Count == 0)
                    {
                        return result;
                    }
                    else
                    {
                        result = query[0];
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            else
            {
                try
                {
                    bool searchHistory = false;

                    TimeSpan tSpan = DateTime.Now.Date - searchModel.ProductDate;
                    if (tSpan.Days >= 7)
                    {
                        searchHistory = true;
                    }

                    string sql = string.Format(@"
                                        SELECT  OQCMa.*
                                        FROM    dbo.{0} OQCMa WITH ( NOLOCK )
                                        WHERE   FlowChart_Detail_UID = {1}
                                                AND OQCMa.Color = N'{2}'
                                                AND OQCMa.MaterialType = N'{3}'
                                                AND ProductDate = N'{4}'
                                                AND Time_interval = N'{5}' 
                                        ", !searchHistory ? "OQC_InputMaster" : "OQC_InputMaster_History", uid, searchModel.Color, searchModel.MaterialType, searchModel.ProductDate.ToShortDateString(), searchModel.Tab_Select_Text);

                    var query = DataContext.Database.SqlQuery<OQC_InputMasterDTO>(sql).ToList();
                    if (query.Count == 0)
                    {
                        return result;
                    }
                    else
                    {
                        result = query[0];
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            #endregion

            return result;
        }

        public int GetDetailUID(string project, string process, DateTime productDate)
        {
            TimeSpan tSpan = DateTime.Now.Date - productDate;
            if (tSpan.Days >= 7)
            {
                var query = from OQCMaster in DataContext.OQC_InputMaster_History
                            join Project in DataContext.System_Project on OQCMaster.Project_UID equals Project.Project_UID
                            join detail in DataContext.FlowChart_Detail on OQCMaster.FlowChart_Detail_UID equals detail.FlowChart_Detail_UID
                            where Project.Project_Name == project && detail.Process == process && DateTime.Compare(OQCMaster.ProductDate, productDate) == 0
                            select detail.FlowChart_Detail_UID;
                int count = query.Count();
                if (count == 0)
                    return 0;
                return query.FirstOrDefault();
            }
            else
            {
                var query = from OQCMaster in DataContext.OQC_InputMaster
                            join Project in DataContext.System_Project on OQCMaster.Project_UID equals Project.Project_UID
                            join detail in DataContext.FlowChart_Detail on OQCMaster.FlowChart_Detail_UID equals detail.FlowChart_Detail_UID
                            where Project.Project_Name == project && detail.Process == process && DateTime.Compare(OQCMaster.ProductDate, productDate) == 0
                            select detail.FlowChart_Detail_UID;
                int count = query.Count();
                if (count == 0)
                    return 0;
                return query.FirstOrDefault();
            }


        }

        public List<OQCReprotDTO> GetQAReportOQCDaySummery(QAReportSearchVM search)
        {
            List<OQCReprotDTO> result = new List<OQCReprotDTO>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", search.ProductDate);
                var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", search.FlowChart_Master_UID);
                var MaterialType = new SqlParameter("MaterialType", search.MaterialType);
                var FunPlant = new SqlParameter("FunPlant", search.FunPlant);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(search.Color) ? "" : search.Color);

                IEnumerable<OQCReprotDTO> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<OQCReprotDTO>("dbo.usp_GetQAReport_OQC_DaySummery @ProductDate, @FlowChart_Master_UID , @MaterialType,@FunPlant, @Color", Product_date, FlowChart_Master_UID, MaterialType, FunPlant, Color).ToArray();
                result = tempResult.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public List<OQCReprotTopFiveTypeDTO> GetQAReportOQCTypeRank(QAReportSearchVM search)
        {
            List<OQCReprotTopFiveTypeDTO> result = new List<OQCReprotTopFiveTypeDTO>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", search.ProductDate);
                var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID ", search.FlowChart_Master_UID);
                var MaterialType = new SqlParameter("MaterialType", search.MaterialType);
                var FunPlant = new SqlParameter("FunPlant", search.FunPlant);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(search.Color) ? "" : search.Color);
                var Count = new SqlParameter("Count", search.Count);

                IEnumerable<OQCReprotTopFiveTypeDTO> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<OQCReprotTopFiveTypeDTO>("dbo.usp_GetQAReport_OQC_DayTopFive @ProductDate, @FlowChart_Master_UID, @MaterialType,@FunPlant, @Color,@Count", Product_date, FlowChart_Master_UID, MaterialType, FunPlant, Color, Count).ToArray();
                result = tempResult.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }


        #region ------Product&Sale Report
        /// <summary>
        /// 查询产销一次/二次良率报表，全厂汇总直通率
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<ProductSaleReport_RateVM> QueryProductSaleReportSummery(QAReportSearchVM searchModel)
        {
            List<ProductSaleReport_RateVM> result = new List<ProductSaleReport_RateVM>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", searchModel.ProductDate);
                var Flowchart_Master_UID = new SqlParameter("Flowchart_Master_UID ", searchModel.FlowChart_Master_UID);
                var MaterialType = new SqlParameter("MaterialType", searchModel.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color);
                var RateType = new SqlParameter("RateType", searchModel.RateType);
                var OPType_OrganizationUID = new SqlParameter("OPType_OrganizationUID", searchModel.OPType_OrganizationUID);

                IEnumerable<ProductSaleReport_RateVM> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductSaleReport_RateVM>(@"dbo.usp_Get_QA_FPY @ProductDate,
                    @Flowchart_Master_UID, @MaterialType,@Color, @RateType,@OPType_OrganizationUID", Product_date, Flowchart_Master_UID, MaterialType, Color, RateType, OPType_OrganizationUID).ToArray();
                result = tempResult.ToList();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<ProductSaleReport_RateVM> QueryProductSaleReportFunplantDetail(QAReportSearchVM searchModel)
        {
            List<ProductSaleReport_RateVM> result = new List<ProductSaleReport_RateVM>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", searchModel.ProductDate);
                var ProjectName = new SqlParameter("Flowchart_Master_UID ", searchModel.FlowChart_Master_UID);
                var MaterialType = new SqlParameter("MaterialType", searchModel.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color);
                var FunPlant = new SqlParameter("FunPlant", searchModel.FunPlant);
                var RateType = new SqlParameter("RateType", searchModel.RateType);
                var SearchType = new SqlParameter("SearchType", searchModel.SearchType);
                var OPType_OrganizationUID = new SqlParameter("OPType_OrganizationUID", searchModel.OPType_OrganizationUID);

                IEnumerable<ProductSaleReport_RateVM> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductSaleReport_RateVM>(@"dbo.usp_Get_QAProductSalaReport_ProcessSummaryByFunPlant @ProductDate,
                    @Flowchart_Master_UID, @MaterialType,@Color,@FunPlant, @RateType,@SearchType,@OPType_OrganizationUID", Product_date, ProjectName, MaterialType, Color, FunPlant, RateType, SearchType, OPType_OrganizationUID).ToArray();
                result = tempResult.ToList();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<ProductSaleReport_RateVM> QueryProductSaleReportExceptionTypeDetail(string TypeFatherName, string FunPlant_Input, string ProductDate,
            int FlowChart_Detail_UID_input, int RateType_Input, string Color_Input, string MeterialType)
        {
            List<ProductSaleReport_RateVM> result = new List<ProductSaleReport_RateVM>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", DateTime.Parse(ProductDate));
                var FlowChart_Detail_UID = new SqlParameter("FlowChart_Detail_UID ", FlowChart_Detail_UID_input);
                var MaterialType = new SqlParameter("MaterialType", MeterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(Color_Input) ? "" : Color_Input);
                var FunPlant = new SqlParameter("FunPlant", FunPlant_Input);
                var RateType = new SqlParameter("RateType", RateType_Input);
                var FatherTypeName = new SqlParameter("FatherTypeName", TypeFatherName);

                IEnumerable<ProductSaleReport_RateVM> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductSaleReport_RateVM>(@"dbo.usp_Get_QAProductSalaReport_ExceptionTypeDetails @ProductDate,
                    @FlowChart_Detail_UID, @MaterialType,@Color,@FunPlant, @RateType,@FatherTypeName", Product_date, FlowChart_Detail_UID, MaterialType, Color, FunPlant, RateType, FatherTypeName).ToArray();
                result = tempResult.ToList();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<TimeIntervalFPYReportVM> QueryTimeIntervalFPYReport(QAReportSearchVM searchModel)
        {
            List<TimeIntervalFPYReportVM> result = new List<TimeIntervalFPYReportVM>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", searchModel.ProductDate);
                var ProjectName = new SqlParameter("Flowchart_Master_UID ", searchModel.FlowChart_Master_UID);
                var MaterialType = new SqlParameter("MaterialType", searchModel.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color);
                var RateType = new SqlParameter("RateType", searchModel.RateType);
                var OPType_OrganizationUID = new SqlParameter("OPType_OrganizationUID", searchModel.OPType_OrganizationUID);

                IEnumerable<TimeIntervalFPYReportVM> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<TimeIntervalFPYReportVM>(@"dbo.usp_Get_QAFPYByDay @ProductDate,
                    @Flowchart_Master_UID, @MaterialType,@Color, @RateType,@OPType_OrganizationUID", Product_date, ProjectName, MaterialType, Color, RateType, OPType_OrganizationUID).ToArray();
                result = tempResult.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<QABackToFunPlant> QueryQABackToFunPlantInfo(QAReportSearchVM searchModel)
        {
            List<QABackToFunPlant> result = new List<QABackToFunPlant>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", searchModel.ProductDate);
                var Flowchart_Master_UID = new SqlParameter("Flowchart_Master_UID ", searchModel.FlowChart_Master_UID);
                var FlowchartDetailUID = new SqlParameter("Flowchart_Detail_UID ", searchModel.FlowChart_Detail_UID);
                var systemFunPlantUID = new SqlParameter("System_FunPlant_UID ", searchModel.System_FunPlant_UID);
                var MaterialType = new SqlParameter("MaterialType", searchModel.MaterialType);
                var RateType = new SqlParameter("RateType", searchModel.RateType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color);

                IEnumerable<QABackToFunPlant> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QABackToFunPlant>(@"dbo.QueryQADistrbuteRate @ProductDate,
                    @Flowchart_Master_UID,@System_FunPlant_UID,@Flowchart_Detail_UID,@RateType, @MaterialType,@Color", Product_date, Flowchart_Master_UID, systemFunPlantUID, FlowchartDetailUID, RateType, MaterialType, Color).ToArray();
                result = tempResult.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public QABackToFunPlant QueryQABTFInfoByUID(string QualityAssurance_DistributeRate_UID)
        {
            QABackToFunPlant result = new QABackToFunPlant();
            try
            {
                string sql = string.Format(@" 

                SELECT  qa.FlowChart_Master_UID ,
                        FlowChart_Detail_UID ,
                        System_FunPlant_UID ,
                        qa.ExceptionType_UID ,
                        QualityAssurance_DistributeRate_UID ,
                        RejectionRate,
                        Surface_Rate ,
                        CNC_Rate ,
                        OQC_Rate ,
                        Assemble_Rate,
                        Anode_Rate ,
                        ProductDate ,
                        extype.TypeName AS ExceptionTypeName,
                        Color
                FROM    dbo.QualityAssurance_DistributeRate qa WITH ( NOLOCK )
                INNER JOIN dbo.QualityAssurance_ExceptionType extype WITH(NOLOCK) ON extype.ExceptionType_UID = qa.ExceptionType_UID
                WHERE   QualityAssurance_DistributeRate_UID=N'{0}'", QualityAssurance_DistributeRate_UID);


                var query = DataContext.Database.SqlQuery<QABackToFunPlant>(sql).ToList();

                if (query.Count != 0)
                {
                    result = query[0];
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public string SaveBackToFunPlantInfo(QABackToFunPlantListVM data)
        {
            string result = string.Empty;
            try
            {
                foreach (QABackToFunPlant temp in data.DataList)
                {
                    string Sql = string.Format(@"
                        IF EXISTS (SELECT TOP 1 1 FROM dbo.QualityAssurance_DistributeRate WITH(NOLOCK)
                        WHERE FlowChart_Master_UID={0}  AND FlowChart_Detail_UID={1} AND ExceptionType_UID={3} AND ProductDate=  N'{11}'  AND Color=  N'{12}'
                        and MaterialType=N'{13}' and RateType=N'{6}' )
                        BEGIN
			                UPDATE dbo.QualityAssurance_DistributeRate SET CNC_Rate={9},OQC_Rate={8} ,Assemble_Rate={10},Anode_Rate={7},Surface_Rate={5}
			                WHERE FlowChart_Master_UID={0}  AND FlowChart_Detail_UID={1}  AND ProductDate=N'{11}'  AND Color=N'{12}' AND ExceptionType_UID={3}  and MaterialType=N'{13}'
                            and RateType=N'{6}'
        
                        END
                        else
                        begin

                        INSERT INTO dbo.QualityAssurance_DistributeRate
                                ( QualityAssurance_DistributeRate_UID ,
                                    FlowChart_Master_UID ,
                                    FlowChart_Detail_UID ,
                                    System_FunPlant_UID ,
                                    ExceptionType_UID ,
                                    RejectionRate ,
                                    Surface_Rate ,
                                    RateType ,
                                    Anode_Rate ,
                                    OQC_Rate ,
                                    CNC_Rate ,
                                    Assemble_Rate ,
                                    ProductDate ,
                                    Color,
                                    MaterialType
                                )
                        VALUES  ( NEWID() , -- QualityAssurance_DistributeRate_UID - uniqueidentifier
                                    {0} , -- FlowChart_Master_UID - int
                                    {1} , -- FlowChart_Detail_UID - int
                                    {2} , -- System_FunPlant_UID - int
                                    {3} , -- ExceptionType_UID - int
                                    {4} , -- RejectionRate - decimal
                                    {5} , -- Surface_Rate - decimal
                                    N'{6}' , -- RateType - nvarchar(50)
                                    {7} , -- Anode_Rate - decimal
                                    {8} , -- OQC_Rate - decimal
                                    {9} , -- CNC_Rate - decimal
                                    {10} , -- Assemble_Rate - decimal
                                    N'{11}' , -- ProductDate - date
                                    N'{12}', -- Color - nvarchar(50)
                                    N'{13}'
                                ) 
                    end", temp.Flowchart_Master_UID, temp.FlowChart_Detail_UID, temp.System_FunPlant_UID, temp.ExceptionType_UID,
                                    temp.RejectionRate, temp.Surface_Rate, temp.RateType, temp.Anode_Rate, temp.OQC_Rate, temp.CNC_Rate, temp.Assemble_Rate, DateTime.Now.ToShortDateString(),
                                    temp.Color, temp.MaterialType);
                    DataContext.Database.ExecuteSqlCommand(Sql);
                }
                result = "Success";
            }
            catch (Exception ex)
            {
                result = "发生错误，请联系管理人员。";
                log.Error(ex);
            }
            return result;
        }

        public string UpdateBackToFunPlantInfo(QABackToFunPlant data)
        {

            string result = string.Empty;
            try
            {
                string sql = string.Format(@"
    UPDATE dbo.QualityAssurance_DistributeRate SET CNC_Rate={1},OQC_Rate={2},Anode_Rate={3},Assemble_Rate={4},Surface_Rate={5} 
    WHERE QualityAssurance_DistributeRate_UID ='{0}'", data.QualityAssurance_DistributeRate_UID, data.CNC_Rate, data.OQC_Rate,
    data.Anode_Rate, data.Assemble_Rate, data.Surface_Rate);

                DataContext.Database.ExecuteSqlCommand(sql);
                result = "Success";
            }
            catch (Exception ex)
            {
                result = "发生错误，请联系管理人员。";
                log.Error(ex);
            }
            return result;
        }


        #region  反推报表
        public List<ProductSaleReport_RateVM> QueryDistributeRateReportDetail(QAReportSearchVM searchModel)
        {
            List<ProductSaleReport_RateVM> result = new List<ProductSaleReport_RateVM>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", searchModel.ProductDate);
                var ProjectName = new SqlParameter("Flowchart_Master_UID ", searchModel.FlowChart_Master_UID);
                var MaterialType = new SqlParameter("MaterialType", searchModel.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color);
                var FunPlant = new SqlParameter("FunPlant", searchModel.FunPlant);
                var RateType = new SqlParameter("RateType", searchModel.RateType);
                var SearchType = new SqlParameter("SearchType", searchModel.SearchType);
                var OPType_OrganizationUID = new SqlParameter("OPType_OrganizationUID", searchModel.OPType_OrganizationUID);

                IEnumerable<ProductSaleReport_RateVM> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductSaleReport_RateVM>(@"dbo.usp_Get_QADistributeRate_ProcessSummaryByFunPlant @ProductDate,
                    @Flowchart_Master_UID, @MaterialType,@Color,@FunPlant, @RateType,@SearchType,@OPType_OrganizationUID", Product_date, ProjectName, MaterialType, Color, FunPlant, RateType, SearchType, OPType_OrganizationUID).ToArray();
                result = tempResult.ToList();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }


        public List<ProductSaleReport_RateVM> QueryDistributeRateReportExceptionTypeDetail(string FatherTypeName_input, string FunPlant_Input,
            string ProductDate, int FlowChart_Master_UID_Input, int RateType_Input, string Color_Input, string MaterialType_input, int OPType_OrganizationUID_input)
        {
            List<ProductSaleReport_RateVM> result = new List<ProductSaleReport_RateVM>();
            try
            {
                var Product_date = new SqlParameter("ProductDate", ProductDate);
                var FlowChart_Master_UID = new SqlParameter("Flowchart_Master_UID ", FlowChart_Master_UID_Input);
                var MaterialType = new SqlParameter("MaterialType", MaterialType_input);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(Color_Input) ? "" : Color_Input);
                var FunPlant = new SqlParameter("FunPlant", FunPlant_Input);
                var RateType = new SqlParameter("RateType", RateType_Input);
                var OPType_OrganizationUID = new SqlParameter("OPType_OrganizationUID", OPType_OrganizationUID_input);
                var FatherTypeName = new SqlParameter("FatherTypeName", FatherTypeName_input);

                IEnumerable<ProductSaleReport_RateVM> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<ProductSaleReport_RateVM>(@"dbo.usp_Get_QAyDistributeReport_ExceptionTypeDetails @ProductDate,
                    @FlowChart_Master_UID, @MaterialType,@Color,@FunPlant, @RateType,@OPType_OrganizationUID,@FatherTypeName", Product_date, FlowChart_Master_UID, MaterialType, Color, FunPlant, RateType, OPType_OrganizationUID, FatherTypeName).ToArray();
                result = tempResult.ToList();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }



        #endregion

        #endregion
    }

    public interface IQualityAssurance_OQC_InputMasterRepository : IRepository<OQC_InputMaster>
    {

        OQC_InputMasterDTO QueryOQCMasterData(QAReportSearchVM searhModel);
        OQC_InputMasterDTO QueryOQCRecordData(QAReportSearchVM searhModel);
        string SaveOQCData(OQCInputData data);
        List<OQCReprotDTO> GetQAReportOQCDaySummery(QAReportSearchVM search);
        List<OQCReprotTopFiveTypeDTO> GetQAReportOQCTypeRank(QAReportSearchVM search);

        List<ProductSaleReport_RateVM> QueryProductSaleReportFunplantDetail(QAReportSearchVM searchModel);
        List<ProductSaleReport_RateVM> QueryProductSaleReportSummery(QAReportSearchVM searchModel);
        List<TimeIntervalFPYReportVM> QueryTimeIntervalFPYReport(QAReportSearchVM searchModel);

        List<ProductSaleReport_RateVM> QueryProductSaleReportExceptionTypeDetail(string TypeFatherName, string FunPlant, string ProductDate, int FlowChart_Detail_UID, int RateType, string Color, string MeterialType);
        List<QABackToFunPlant> QueryQABackToFunPlantInfo(QAReportSearchVM search);
        QABackToFunPlant QueryQABTFInfoByUID(string QualityAssurance_DistributeRate_UID);
        string SaveBackToFunPlantInfo(QABackToFunPlantListVM data);
        string UpdateBackToFunPlantInfo(QABackToFunPlant data);

        //----反推报表
        List<ProductSaleReport_RateVM> QueryDistributeRateReportDetail(QAReportSearchVM searchModel);
        List<ProductSaleReport_RateVM> QueryDistributeRateReportExceptionTypeDetail(string TypeFatherName, string FunPlant,
            string ProductDate, int FlowChart_Master_UID, int RateType, string Color, string MeterialType, int OPType_OrganizationUID);


    }



}

