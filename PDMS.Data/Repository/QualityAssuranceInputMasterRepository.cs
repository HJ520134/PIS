using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using PDMS.Common.Constants;
using System.Text;
using System.Configuration;
using System.Data;

namespace PDMS.Data.Repository
{
    public class QualityAssuranceInputMasterRepository : RepositoryBase<QAMasterVM>, IQualityAssuranceInputMasterRepository
    {
        private Logger log = new Logger("QualityAssuranceInputMasterRepository");
        public QualityAssuranceInputMasterRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public QualityAssurance_InputMasterDTO QueryAssuranceInputMaster(CheckPointInputConditionModel searchModel)
        {
            QualityAssurance_InputMasterDTO result = new QualityAssurance_InputMasterDTO();

            try
            {
                bool searchHistory = false;
                TimeSpan tSpan = DateTime.Now.Date - searchModel.ProductDate;
                if (tSpan.Days >= 7)
                {
                    searchHistory = true;
                }

                if (!searchHistory)
                {
                    var query = from qAMaster in DataContext.QualityAssurance_InputMaster
                                join flowChartDetail in DataContext.FlowChart_Detail on qAMaster.FlowChart_Detail_UID equals
                                    flowChartDetail.FlowChart_Detail_UID
                                where
                                    qAMaster.FlowChart_Detail_UID == searchModel.Flowchart_Detail_UID && qAMaster.Product_Date == searchModel.ProductDate &&
                                    qAMaster.MaterielType == searchModel.MaterialType
                                    && qAMaster.Time_Interval == searchModel.Time_interval && qAMaster.Color == searchModel.Color
                                select new QualityAssurance_InputMasterDTO
                                {
                                    NG_Qty = qAMaster.NG_Qty,
                                    Input = qAMaster.Input,
                                    Color = qAMaster.Color,
                                    FirstCheck_Qty = qAMaster.FirstCheck_Qty,
                                    FirstOK_Qty = qAMaster.FirstOK_Qty,
                                    FirstRejectionRate = qAMaster.FirstRejectionRate,
                                    FlowChart_Detail_UID = qAMaster.FlowChart_Detail_UID,
                                    MaterielType = qAMaster.MaterielType,
                                    Process = qAMaster.Process,
                                    Product_Date = qAMaster.Product_Date,
                                    Shipment_Qty = qAMaster.Shipment_Qty,
                                    SurfaceSA_Qty = qAMaster.SurfaceSA_Qty,
                                    SizeSA_Qty = qAMaster.SizeSA_Qty,
                                    RepairCheck_Qty = qAMaster.RepairCheck_Qty,
                                    RepairOK_Qty = qAMaster.RepairOK_Qty,
                                    WIPForCheck_Qty = qAMaster.WIPForCheck_Qty,
                                    Time_Interval = qAMaster.Time_Interval,
                                    QualityAssurance_InputMaster_UID = qAMaster.QualityAssurance_InputMaster_UID,
                                    NGFlag = qAMaster.NGFlag,
                                    FirstCheckFlag = qAMaster.FirstCheckFlag,
                                    Displace_Qty = qAMaster.Displace_Qty,
                                    DisplaceFlag = qAMaster.DisplaceFlag
                                };

                    if (query.Count() != 0)
                    {
                        result = query.ToList()[0];
                    }
                }
                else
                {
                    var query = from qAMaster in DataContext.QualityAssurance_InputMaster_History
                                join flowChartDetail in DataContext.FlowChart_Detail on qAMaster.FlowChart_Detail_UID equals
                                    flowChartDetail.FlowChart_Detail_UID
                                where
                                    qAMaster.FlowChart_Detail_UID == searchModel.Flowchart_Detail_UID && qAMaster.Product_Date == searchModel.ProductDate &&
                                    qAMaster.MaterielType == searchModel.MaterialType && qAMaster.Time_Interval == searchModel.Time_interval && qAMaster.Color == searchModel.Color
                                select new QualityAssurance_InputMasterDTO
                                {
                                    NG_Qty = qAMaster.NG_Qty,
                                    Input = qAMaster.Input,
                                    Color = qAMaster.Color,
                                    FirstCheck_Qty = qAMaster.FirstCheck_Qty,
                                    FirstOK_Qty = qAMaster.FirstOK_Qty,
                                    FirstRejectionRate = qAMaster.FirstRejectionRate,
                                    FlowChart_Detail_UID = qAMaster.FlowChart_Detail_UID,
                                    MaterielType = qAMaster.MaterielType,
                                    Process = qAMaster.Process,
                                    Product_Date = qAMaster.Product_Date,
                                    Shipment_Qty = qAMaster.Shipment_Qty,
                                    SurfaceSA_Qty = qAMaster.SurfaceSA_Qty,
                                    SizeSA_Qty = qAMaster.SizeSA_Qty,
                                    RepairCheck_Qty = qAMaster.RepairCheck_Qty,
                                    RepairOK_Qty = qAMaster.RepairOK_Qty,
                                    WIPForCheck_Qty = qAMaster.WIPForCheck_Qty,
                                    Time_Interval = qAMaster.Time_Interval,
                                    QualityAssurance_InputMaster_UID = qAMaster.QualityAssurance_InputMaster_UID,
                                    NGFlag = qAMaster.NGFlag,
                                    FirstCheckFlag = qAMaster.FirstCheckFlag,
                                    Displace_Qty = qAMaster.Displace_Qty,
                                    DisplaceFlag = qAMaster.DisplaceFlag
                                };

                    if (query.Count() != 0)
                    {
                        result = query.ToList()[0];
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public void updateMesSynsData(QAMasterVM data)
        {

            StringBuilder sb = new StringBuilder();

            if (data.Input != null)
            {
                var pickSql = $" Input={data.Input}";
                sb.AppendLine(pickSql);
            }

            if (data.NG_Qty != null)
            {
                if (data.Input != null)
                {
                    sb.AppendLine(",");
                }
                var ngSql = $" NG_Qty={data.NG_Qty}";
                sb.AppendLine(ngSql);
            }

            var strSql = $" UPDATE [QualityAssurance_InputMaster] SET {sb.ToString()} WHERE Product_Date='{data.Product_Date}' AND Time_Interval='{data.Time_Interval}' AND FlowChart_Detail_UID={data.FlowChart_Detail_UID}";
            DataContext.Database.ExecuteSqlCommand(strSql);
        }

        public void updateMesSynsDataByUID(QAMasterVM data)
        {
            StringBuilder sb = new StringBuilder();

            if (data.Input != null)
            {
                var pickSql = $"Input={data.Input}";
                sb.AppendLine(pickSql);
            }

            if (data.NG_Qty != null)
            {
                var ngSql = $" NG_Qty={data.NG_Qty}";
                sb.AppendLine(ngSql);
            }

            var strSql = $" UPDATE [QualityAssurance_InputMaster] SET {sb.ToString()} WHERE  QualityAssurance_InputMaster_UID={data.QualityAssurance_InputMaster_UID}";
            DataContext.Database.ExecuteSqlCommand(strSql);
        }


        public QualityAssurance_InputMasterDTO GetInputMasterByID(int uid)
        {
            QualityAssurance_InputMasterDTO result = new QualityAssurance_InputMasterDTO();
            try
            {
                var query = from qAMaster in DataContext.QualityAssurance_InputMaster
                            where qAMaster.QualityAssurance_InputMaster_UID == uid
                            select new QualityAssurance_InputMasterDTO
                            {
                                NG_Qty = qAMaster.NG_Qty,
                                Input = qAMaster.Input,
                                Color = qAMaster.Color,
                                FirstCheck_Qty = qAMaster.FirstCheck_Qty,
                                FirstOK_Qty = qAMaster.FirstOK_Qty,
                                FirstRejectionRate = qAMaster.FirstRejectionRate,
                                FlowChart_Detail_UID = qAMaster.FlowChart_Detail_UID,
                                MaterielType = qAMaster.MaterielType,
                                Process = qAMaster.Process,
                                Product_Date = qAMaster.Product_Date,
                                Shipment_Qty = qAMaster.Shipment_Qty,
                                SurfaceSA_Qty = qAMaster.SurfaceSA_Qty,
                                SizeSA_Qty = qAMaster.SizeSA_Qty,
                                RepairCheck_Qty = qAMaster.RepairCheck_Qty,
                                RepairOK_Qty = qAMaster.RepairOK_Qty,
                                WIPForCheck_Qty = qAMaster.WIPForCheck_Qty,
                                Time_Interval = qAMaster.Time_Interval,
                                QualityAssurance_InputMaster_UID = qAMaster.QualityAssurance_InputMaster_UID,
                                NGFlag = qAMaster.NGFlag,
                                FirstCheckFlag = qAMaster.FirstCheckFlag,
                                Displace_Qty = qAMaster.Displace_Qty,
                                DisplaceFlag = qAMaster.DisplaceFlag
                            };
                if (query.Count() != 0)
                {
                    result = query.ToList()[0];
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }


        public ReturnMessageQA SaveQaMasterData(QAMasterVM data)
        {
            ReturnMessageQA result = new ReturnMessageQA();

            try
            {
                double firstoK = (double)data.FirstOK_Qty;
                double firstCheck = (double)data.FirstCheck_Qty;

                string sql = string.Format(@"
                        DECLARE @supplierUID INT
                        DECLARE @message NVARCHAR(250)
                        IF NOT EXISTS ( SELECT TOP 1
                                                1
                                        FROM    dbo.QualityAssurance_InputMaster
                                        WHERE   Product_Date = '{4}'
                                                AND Time_Interval = N'{5}'
                                                AND FlowChart_Detail_UID = {0} 
                                                and Color= N'{2}')
                            BEGIN
                                INSERT  INTO dbo.QualityAssurance_InputMaster
                                        ( FlowChart_Detail_UID ,
                                          Process ,
                                          Color ,
                                          MaterielType ,
                                          Product_Date ,
                                          Time_Interval ,
                                          Input ,
                                          FirstCheck_Qty ,
                                          FirstOK_Qty ,
                                          FirstRejectionRate ,
                                          NG_Qty ,
                                          RepairOK_Qty ,
                                          Creator_UID ,
                                          Create_Date ,
                                          Modified_UID ,
                                          Modified_Date,
                                          Displace_Qty,
                                          WIPForCheck_Qty,
                                          RepairCheck_Qty
                                        )
                                VALUES  ( {0} , -- FlowChart_Detail_UID - int
                                          N'{1}' , -- Process - nvarchar(50)
                                          N'{2}' , -- Color - nvarchar(50)
                                          N'{3}' , -- MaterielType - nvarchar(50)
                                          '{4}' , -- Product_Date - date
                                          N'{5}' , -- Time_Interval - nvarchar(20)
                                          {6} , -- Input - int
                                          {7} , -- FirstCheck_Qty - int
                                          {8} , -- FirstOK_Qty - int
                                          {9} , -- FirstRejectionRate - decimal
                                          {10} , -- NG_Qty - int
                                          {11} , -- RepairOK_Qty - int
                                          {12} , -- Creator_UID - int
                                          GETDATE() , -- Create_Date - datetime
                                          {12} , -- Modified_UID - int
                                          GETDATE(),  -- Modified_Date - datetime,
                                          {13},
                                          {14},
                                          {15}  --RepairCheck_Qty  -INT
                                        )
                                delete  dbo.PPForQAInterface
                               
                                WHERE   FlowChart_Detail_UID = {0}
                                        AND Product_Date = '{4}'
                                        AND Time_Interval = N'{5}'
                                        AND MaterielType = N'{3}'
                                SET @supplierUID = SCOPE_IDENTITY()
                                DECLARE @temp TABLE
								(
									 Message NVARCHAR(200)  
								)
                                INSERT INTO @temp
                                        ( Message )
                                EXEC usp_CalculateQAReportSumData @supplierUID
                                
                                SELECT TOP 1 @message=Message FROM @temp
                            END
                        ELSE
                            BEGIN
                                SET @supplierUID = 0
                                SET @message=N'当前制程本时段数据已被录入，请选择其他制程录入数据.'
                            END
                        SELECT  @supplierUID AS UID, @message AS Message                            
", data.FlowChart_Detail_UID, data.Process, data.Color, data.MaterialType, data.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate), data.Time_Interval, data.Input, data.FirstCheck_Qty, data.FirstOK_Qty, firstCheck == 0 ? 0 : (1 - firstoK / firstCheck),
data.NG_Qty, data.RepairOK_Qty, data.Creator_UID, data.Displace_Qty, data.WIPForCheck_Qty, data.RepairCheck_Qty);

                result = DataContext.Database.SqlQuery<ReturnMessageQA>(sql).ToArray()[0];
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public string ModifyQAMasterData(QAMasterVM data)
        {
            string reuslt = "";
            try
            {
                string sql = string.Format(@"UPDATE  dbo.QualityAssurance_InputMaster
                                            SET     FirstCheck_Qty = {0} ,
                                                    FirstOK_Qty = {1} ,
                                                    RepairOK_Qty = {2} ,
                                                    Modified_UID = {3} ,
                                                    Modified_Date = N'{4}',
                                                    Input = {6},
                                                    NG_Qty={7},
                                                    Displace_Qty={8},
                                                    WIPForCheck_Qty={9},
                                                    RepairCheck_Qty={10}
                                            WHERE   QualityAssurance_InputMaster_UID = {5}

                                            EXEC usp_CalculateQAReportSumData {5}
                                            ", data.FirstCheck_Qty, data.FirstOK_Qty,
                                             data.RepairOK_Qty, data.Modified_UID, data.Modified_Date.ToString("yyyy-MM-dd")
                                            , data.QualityAssurance_InputMaster_UID, data.Input, data.NG_Qty, data.Displace_Qty
                                            , data.WIPForCheck_Qty, data.RepairCheck_Qty);

                DataContext.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                reuslt = "Error";
                log.Error(ex);
            }
            reuslt = "SUCCESS";
            return reuslt;
        }

        public List<QualityAssurance_InputMasterDTO> QueryQAHistroyDatas(QAReportSearchVM searchModel, DateTime dateNow)
        {
            var ProcesColor = string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color;
            List<QualityAssurance_InputMasterDTO> result = new List<QualityAssurance_InputMasterDTO>();
            try
            {
                string timeInterVal = searchModel.Time_interval;
                if (!string.IsNullOrEmpty(searchModel.Tab_Select_Text) && (searchModel.Tab_Select_Text.Contains("小计") || searchModel.Tab_Select_Text.Contains("全天")))
                {
                    //TODO:SP 获取 全天，白班小计，夜班小计的情况
                    var sumType = "";
                    if (searchModel.Tab_Select_Text.Equals("全天"))
                    {
                        sumType = "ALL";
                    }
                    else if (searchModel.Tab_Select_Text.Contains("白班"))
                    {
                        sumType = "Daily_Sum";
                    }
                    else
                    {
                        sumType = "Night_Sum";
                    }

                    var Product_date = new SqlParameter("ProductDate", searchModel.ProductDate);
                    var SumType = new SqlParameter("SumType", sumType);
                    var MaterialType = new SqlParameter("MaterialType", searchModel.MaterialType);
                    var Color = new SqlParameter("Color", string.IsNullOrEmpty(searchModel.Color) ? "" : searchModel.Color);
                    var FlowChart_Detail_UID = new SqlParameter("FlowChart_Detail_UID", searchModel.FlowChart_Detail_UID);

                    IEnumerable<QualityAssurance_InputMasterDTO> tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QualityAssurance_InputMasterDTO>("dbo.usp_GetQADayDataSum @ProductDate,@FlowChart_Detail_UID, @SumType, @MaterialType, @Color", Product_date, FlowChart_Detail_UID, SumType, MaterialType, Color).ToArray();
                    result = tempResult.ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(searchModel.Tab_Select_Text))
                        timeInterVal = searchModel.Tab_Select_Text;


                    bool searchHistory = false;
                    TimeSpan tSpan = DateTime.Now.Date - searchModel.ProductDate;
                    if (tSpan.Days >= 7)
                    {
                        searchHistory = true;
                    }

                    if (!searchHistory)
                    {
                        var query = from qAMaster in DataContext.QualityAssurance_InputMaster
                                    join flowDetail in DataContext.FlowChart_Detail on qAMaster.FlowChart_Detail_UID equals flowDetail.FlowChart_Detail_UID
                                    where qAMaster.Product_Date == searchModel.ProductDate && qAMaster.Time_Interval == timeInterVal
                                && flowDetail.FlowChart_Detail_UID == searchModel.FlowChart_Detail_UID &&
                                    qAMaster.MaterielType == searchModel.MaterialType && qAMaster.Color == ProcesColor
                                    select new QualityAssurance_InputMasterDTO
                                    {
                                        NG_Qty = qAMaster.NG_Qty,
                                        Input = qAMaster.Input,
                                        Color = qAMaster.Color,
                                        FirstCheck_Qty = qAMaster.FirstCheck_Qty,
                                        FirstOK_Qty = qAMaster.FirstOK_Qty,
                                        FirstRejectionRate = qAMaster.FirstRejectionRate,
                                        FlowChart_Detail_UID = qAMaster.FlowChart_Detail_UID,
                                        MaterielType = qAMaster.MaterielType,
                                        Process = qAMaster.Process,
                                        Product_Date = qAMaster.Product_Date,
                                        Shipment_Qty = qAMaster.Shipment_Qty,
                                        SurfaceSA_Qty = qAMaster.SurfaceSA_Qty,
                                        SizeSA_Qty = qAMaster.SizeSA_Qty,
                                        RepairCheck_Qty = qAMaster.RepairCheck_Qty,
                                        RepairOK_Qty = qAMaster.RepairOK_Qty,
                                        WIPForCheck_Qty = qAMaster.WIPForCheck_Qty,
                                        Time_Interval = qAMaster.Time_Interval,
                                        QualityAssurance_InputMaster_UID = qAMaster.QualityAssurance_InputMaster_UID,
                                        CanModify = (dateNow == qAMaster.Product_Date),
                                        NGFlag = qAMaster.NGFlag,
                                        FirstCheckFlag = qAMaster.FirstCheckFlag,
                                        Displace_Qty = qAMaster.Displace_Qty,
                                        DisplaceFlag = qAMaster.DisplaceFlag

                                    };

                        if (query.Count() != 0)
                        {
                            result = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from qAMaster in DataContext.QualityAssurance_InputMaster_History
                                    join flowDetail in DataContext.FlowChart_Detail on qAMaster.FlowChart_Detail_UID equals flowDetail.FlowChart_Detail_UID
                                    where qAMaster.Product_Date == searchModel.ProductDate && qAMaster.Time_Interval == timeInterVal
                                          && flowDetail.FlowChart_Detail_UID == searchModel.FlowChart_Detail_UID &&
                                    qAMaster.MaterielType == searchModel.MaterialType && qAMaster.Color == ProcesColor
                                    select new QualityAssurance_InputMasterDTO
                                    {
                                        NG_Qty = qAMaster.NG_Qty,
                                        Input = qAMaster.Input,
                                        Color = qAMaster.Color,
                                        FirstCheck_Qty = qAMaster.FirstCheck_Qty,
                                        FirstOK_Qty = qAMaster.FirstOK_Qty,
                                        FirstRejectionRate = qAMaster.FirstRejectionRate,
                                        FlowChart_Detail_UID = qAMaster.FlowChart_Detail_UID,
                                        MaterielType = qAMaster.MaterielType,
                                        Process = qAMaster.Process,
                                        Product_Date = qAMaster.Product_Date,
                                        Shipment_Qty = qAMaster.Shipment_Qty,
                                        SurfaceSA_Qty = qAMaster.SurfaceSA_Qty,
                                        SizeSA_Qty = qAMaster.SizeSA_Qty,
                                        RepairCheck_Qty = qAMaster.RepairCheck_Qty,
                                        RepairOK_Qty = qAMaster.RepairOK_Qty,
                                        WIPForCheck_Qty = qAMaster.WIPForCheck_Qty,
                                        Time_Interval = qAMaster.Time_Interval,
                                        QualityAssurance_InputMaster_UID = qAMaster.QualityAssurance_InputMaster_UID,
                                        CanModify = (dateNow == qAMaster.Product_Date),
                                        NGFlag = qAMaster.NGFlag,
                                        FirstCheckFlag = qAMaster.FirstCheckFlag,
                                        Displace_Qty = qAMaster.Displace_Qty,
                                        DisplaceFlag = qAMaster.DisplaceFlag

                                    };

                        if (query.Count() != 0)
                        {
                            result = query.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public QAReportSearchProjectAndMetrialTypeVM QueryQAReportSearchProjectAndMetrialInfo(int OPType_Organization_UID)
        {
            QAReportSearchProjectAndMetrialTypeVM result = new QAReportSearchProjectAndMetrialTypeVM();
            try
            {
                var queryProject = from proj in DataContext.System_Project
                                   join flowMaster in DataContext.FlowChart_Master on proj.Project_UID equals flowMaster.Project_UID
                                   where proj.Organization_UID == OPType_Organization_UID && flowMaster.Is_Closed == false
                                   select new QAProjectVM
                                   {
                                       ProjectName = proj.Project_Name,

                                       Project_UID = proj.Project_UID
                                   };
                result.Project = queryProject.Distinct().ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public QAReportVM QueryQAReport(QAReportSearchVM search)
        {
            QAReportVM result = new QAReportVM();
            try
            {
                result.summeryData = GetQAReportDaySummery(search);
                result.summeryData.Color = search.Color;
                result.FirstRejectionRateTopTen = GetQAReportTypeRank(search, 1);
                result.SecondRejectionRateTopTen = GetQAReportTypeRank(search, 2);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public List<FlowchartColor> GetProcessByProject(int Flowchart_Master_UID, string functionPlant)
        {
            List<FlowchartColor> result = new List<FlowchartColor>();
            try
            {
                List<string> isQA = new List<string> { "Inspect_" + functionPlant, "Polling_" + functionPlant };
                if ("Assemble,OQC".ToUpper().Contains(functionPlant))
                {
                    isQA.Add("Inspect_Assemble,Inspect_OQC");
                }
                var query = from FD in DataContext.FlowChart_Detail
                            join FM in DataContext.FlowChart_Master on new { FD.FlowChart_Master_UID, FD.FlowChart_Version } equals new { FM.FlowChart_Master_UID, FM.FlowChart_Version }

                            where FM.FlowChart_Master_UID == Flowchart_Master_UID && FD.IsQAProcess != ""
                            select new FlowchartColor
                            {
                                Process = FD.Process,
                                Process_Seq = FD.Process_Seq,
                                Color = FD.Color,
                                Flowchart_Detail_UID = FD.FlowChart_Detail_UID
                            };

                if (query.Count() > 0)
                {
                    result = query.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public QAReportDaySummeryDTO GetQAReportDaySummery(QAReportSearchVM search)
        {
            try
            {
                string selectedTabSumType = "";
                if (!string.IsNullOrEmpty(search.Tab_Select_Text))
                {
                    if (search.Tab_Select_Text == "白班小计")
                    {
                        selectedTabSumType = "Daily_Sum";
                    }
                    else if (search.Tab_Select_Text == "全天")
                    {
                        selectedTabSumType = "ALL";
                    }
                    else if (search.Tab_Select_Text == "夜班小计")
                    {
                        selectedTabSumType = "Night_Sum";
                    }
                }

                var Flowchart_Detail_UID = new SqlParameter("Flowchart_Detail_UID", search.FlowChart_Detail_UID);
                var Product_date = new SqlParameter("ProductDate", search.ProductDate);
                var SumType = new SqlParameter("SumType", string.IsNullOrEmpty(selectedTabSumType) ? search.Time_interval : selectedTabSumType);
                var MaterialType = new SqlParameter("MaterialType", search.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(search.Color) ? "" : search.Color);
                
                var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", search.FlowChart_Master_UID);
                var Process = new SqlParameter("Process", search.Process);
                var ProjectName = new SqlParameter("ProjectName", search.ProjectName);
                if (search.Part_Type == "ALL" || search.Color == "ALL")  // 增加的汇总所有Part Type
                {
                    IEnumerable<QAReportDaySummeryDTO> summeryData = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QAReportDaySummeryDTO>(@"
dbo.usp_GetQAReportSum @ProductDate,@Process, @SumType,@MaterialType,@Color,@ProjectName", Product_date, Process, SumType, MaterialType, Color, ProjectName).ToArray();

                    if (summeryData.Count() != 0)
                    {
                        return summeryData.ToArray()[0];
                    }
                }
                else
                {

               
                    IEnumerable<QAReportDaySummeryDTO> summeryData = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QAReportDaySummeryDTO>(@"
dbo.usp_GetQAReportDaySummery @ProductDate,@Flowchart_Detail_UID, @SumType,@MaterialType,@Color", Product_date, Flowchart_Detail_UID, SumType, MaterialType, Color).ToArray();
               
                if (summeryData.Count() != 0)
                {
                    return summeryData.ToArray()[0];
                }
               }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return new QAReportDaySummeryDTO();
        }

        public  List<QAReportDaySummeryDTO> ExecuteNonQuery(string spName, SqlParameter[] parameterValues)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SPPContext"].ConnectionString;
            List<QAReportDaySummeryDTO> resultList = new List<QAReportDaySummeryDTO>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                foreach (SqlParameter p in parameterValues)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    QAReportDaySummeryDTO model = new QAReportDaySummeryDTO();
                    model.FunPlant = dr["FunPlant"].ToString();
                    model.FirstCheck_Qty = int.Parse(dr["FirstCheck_Qty"].ToString());
                    model.FirstOK_Qty = int.Parse(dr["FirstOK_Qty"].ToString());
                    model.FirstRejectionRate = (decimal)dr["FirstRejectionRate"];
                    model.Input = int.Parse(dr["Input"].ToString());
                    model.SepcialAccept_Qty = int.Parse(dr["SepcialAccept_Qty"].ToString());
                    model.SecondRejectionRate = (decimal)dr["SecondRejectionRate"];

                    model.Process = dr["Process"].ToString();
                    model.Process_Seq = int.Parse(dr["Process_Seq"].ToString());
                    model.FlowChart_Detail_UID = int.Parse(dr["FlowChart_Detail_UID"].ToString());
                    model.NG = int.Parse(dr["NG"].ToString());
                    model.FirstTargetYield = (decimal)dr["FirstTargetYield"];
                    model.SecondTargetYield = (decimal)dr["SecondTargetYield"];

                    resultList.Add(model);
                }
            }
            return resultList;
        }
     
        public List<QAReportDaySummeryDTO> QueryIPQCALLProcessReportSummaryAPI(QAReportSearchVM search)
        {
            List<QAReportDaySummeryDTO> result = new List<QAReportDaySummeryDTO>();
            try
            {
                string selectedTabSumType = "";
                if (!string.IsNullOrEmpty(search.Tab_Select_Text))
                {
                    if (search.Tab_Select_Text == "白班小计")
                    {
                        selectedTabSumType = "Daily_Sum";
                    }
                    else if (search.Tab_Select_Text == "全天")
                    {
                        selectedTabSumType = "ALL";
                    }
                    else if (search.Tab_Select_Text == "夜班小计")
                    {
                        selectedTabSumType = "Night_Sum";
                    }
                }

                
                var ProjectName = new SqlParameter("ProjectName", search.ProjectName);
                var Part_Type = new SqlParameter("Part_Type", search.Part_Type);
                var OPType = new SqlParameter("OPType", int.Parse(search.OPType));
                var Product_Phase = new SqlParameter("Product_Phase", search.Product_Phase);




                var FlowChart_Master_UID = new SqlParameter("flowChartMaster_UID", search.FlowChart_Master_UID);
                var Product_date = new SqlParameter("ProductDate", search.ProductDate);
                var SumType = new SqlParameter("SumType", string.IsNullOrEmpty(selectedTabSumType) ? search.Time_interval : selectedTabSumType);
                var MaterialType = new SqlParameter("MaterialType", search.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(search.Color) ? "" : search.Color);
                if (search.Part_Type == "ALL" || search.Color=="ALL")  // 增加的汇总所有Part Type
                {
                    //SqlParameter[] parameters = new SqlParameter[5];
                    //parameters[0] = new SqlParameter() { ParameterName = "ProductDate", Value = search.ProductDate };//值为上面转换的datatable
                    //parameters[1] = new SqlParameter() { ParameterName = "ProjectName", Value = search.ProjectName };
                    //parameters[2] = new SqlParameter() { ParameterName = "OPType", Value = search.OPType };
                    //parameters[3] = new SqlParameter() { ParameterName = "Product_Phase", Value = search.Product_Phase };
                    //parameters[4] = new SqlParameter() { ParameterName = "SumType", Value = string.IsNullOrEmpty(selectedTabSumType) ? search.Time_interval : selectedTabSumType };
                    //IEnumerable<QAReportDaySummeryDTO> summeryData = ExecuteNonQuery("usp_Get_QA_ReportSummary", parameters);
                    IEnumerable<QAReportDaySummeryDTO> summeryData = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QAReportDaySummeryDTO>(@"
dbo.usp_Get_QA_ReportSummary @ProductDate,@ProjectName, @OPType,@Product_Phase,@SumType", Product_date, ProjectName, OPType, Product_Phase, SumType).ToArray();

                    if (summeryData.Count() != 0)
                    {
                        result = summeryData.ToList();
                    }
                }
                else
                {
                    //TODO: 修改SP 
                    IEnumerable<QAReportDaySummeryDTO> summeryData = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QAReportDaySummeryDTO>(@"
dbo.usp_GetIPQC_ALLProcess_ReportSummary @ProductDate,@flowChartMaster_UID, @SumType,@MaterialType,@Color", Product_date, FlowChart_Master_UID, SumType, MaterialType, Color).ToArray();
                    if (summeryData.Count() != 0)
                    {
                        result = summeryData.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public List<QAReportExceptionTypeRankDTO> GetQAReportTypeRank(QAReportSearchVM search, int yield)
        {
            try
            {
                string selectedTabSumType = "";
                if (!string.IsNullOrEmpty(search.Tab_Select_Text))
                {
                    if (search.Tab_Select_Text == "白班小计")
                    {
                        selectedTabSumType = "Daily_Sum";
                    }
                    else if (search.Tab_Select_Text == "全天")
                    {
                        selectedTabSumType = "ALL";
                    }
                    else if (search.Tab_Select_Text == "夜班小计")
                    {
                        selectedTabSumType = "Night_Sum";
                    }
                }

                var FlowChart_Detail_UID = new SqlParameter("FlowChart_Detail_UID", search.FlowChart_Detail_UID);
                var Product_date = new SqlParameter("ProductDate", search.ProductDate);
                var SumType = new SqlParameter("SumType", string.IsNullOrEmpty(selectedTabSumType) ? search.Time_interval : selectedTabSumType);
                var MaterialType = new SqlParameter("MaterialType", search.MaterialType);
                var Color = new SqlParameter("Color", string.IsNullOrEmpty(search.Color) ? "" : search.Color);
                var count = new SqlParameter("count", 10);
                var yieldType = new SqlParameter("yieldType", yield);
                var Process = new SqlParameter("Process", search.Process);
                var ProjectName = new SqlParameter("ProjectName", search.ProjectName);
                var FlowChart_Master_UID = new SqlParameter("FlowChart_Master_UID", search.FlowChart_Master_UID);

                if (search.Part_Type == "ALL" || search.Color == "ALL")  // 增加的汇总所有Part Type
                {
                    IEnumerable<QAReportExceptionTypeRankDTO> topTenRate = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QAReportExceptionTypeRankDTO>(@"
dbo.usp_GetQAReportSumDayYield @ProductDate,@Process, @SumType,@MaterialType,@Color,@count,@yieldType,@ProjectName", Product_date, Process, SumType, MaterialType, Color, count, yieldType, ProjectName).ToArray();
                    if (topTenRate.Count() != 0)
                    {
                        return topTenRate.ToList();
                    }
                }
                else
                {


                    IEnumerable<QAReportExceptionTypeRankDTO> topTenRate = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QAReportExceptionTypeRankDTO>(@"
dbo.usp_GetQAReportDayYield @ProductDate,@FlowChart_Detail_UID, @SumType,@MaterialType,@Color,@count,@yieldType", Product_date, FlowChart_Detail_UID, SumType, MaterialType, Color, count, yieldType).ToArray();
                    if (topTenRate.Count() != 0)
                    {
                        return topTenRate.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return new List<QAReportExceptionTypeRankDTO>();
        }

        public List<QualityAssurance_InputMaster> GetQAMasterListByIdList(List<int> idList)
        {
            var list = DataContext.QualityAssurance_InputMaster.Where(m => idList.Contains(m.FlowChart_Detail_UID.Value)).ToList();
            return list;
        }

        public string GetAllFirstTargetYield(QAReportSearchVM search)
        {
            string result = string.Empty;
            double first = 1.0;
            string Product_Date = search.ProductDate.ToString("yyyy-MM-dd");
            if (search.Part_Type == "ALL" || search.Color == "ALL")  //按功能厂汇总
            {
                //先判断哪一个颜色的不良检测点最多，以最多的颜色算直通良率

                int opUID = int.Parse(search.OPType);
                var query = from mg in DataContext.QualityAssurance_MgData
                            where mg.FlowChart_Master.System_Project.Project_Name == search.ProjectName
                            && mg.ProductDate == search.ProductDate
                            && mg.FlowChart_Master.System_Project.Organization_UID == opUID
                            select mg;
                try
                {
                    var list = query.Where(m => m.FlowChart_Detail.Color != "").GroupBy(m => m.FlowChart_Detail.Color).ToDictionary(p => p.Key, m => m.Count());
                    var color = list.OrderByDescending(p => p.Value).FirstOrDefault().Key;
                    var list2 = query.Where(m => m.FlowChart_Detail.Color == color || m.FlowChart_Detail.Color == "");
                    foreach (var item in list2)
                    {
                        first *= (float)item.FirstRejectionRate;
                    }
                }
                catch
                {
                    foreach (var item in query)
                    {
                        first *= (float)item.FirstRejectionRate;
                    }
                }

            }
            else if (search.Color != "ALL")
            {

                string sql = string.Format(@"
	SELECT EXP(SUM(Log(Fm.FirstRejectionRate))) FROM  dbo.[QualityAssurance_MgData] FM JOIN dbo.FlowChart_Detail FD ON FD.FlowChart_Detail_UID = FM.FlowChart_Detail_UID
	JOIN dbo.FlowChart_Master F ON F.FlowChart_Master_UID = FD.FlowChart_Master_UID WHERE 
	f.FlowChart_Master_UID={0} AND FM.ProductDate='{1}' AND (FD.Color='' OR fd.Color=N'{2}')", search.FlowChart_Master_UID, Product_Date, search.Color);

                first = DataContext.Database.SqlQuery<double>(sql).ToArray()[0];
            }
                return first.ToString();
        }
      public  string GetAllSecondTargetYield(QAReportSearchVM search)
        {
            string Product_Date = search.ProductDate.ToString("yyyy-MM-dd");
            string result = string.Empty;
            double first = 1.0;
            if (search.Part_Type == "ALL" || search.Color == "ALL")  //按功能厂汇总
            {
               
                int opUID = int.Parse(search.OPType);
                var query = from mg in DataContext.QualityAssurance_MgData
                            where mg.FlowChart_Master.System_Project.Project_Name == search.ProjectName
                            && mg.ProductDate == search.ProductDate
                            && mg.FlowChart_Master.System_Project.Organization_UID == opUID
                            select mg;


                var list = query.Where(m => m.FlowChart_Detail.Color != "").GroupBy(m => m.FlowChart_Detail.Color).ToDictionary(p => p.Key, m => m.Count());
                var color = list.OrderByDescending(p => p.Value).FirstOrDefault().Key;
                var list2 = query.Where(m => m.FlowChart_Detail.Color == color || m.FlowChart_Detail.Color == "");
                foreach (var item in list2)
                {
                    first *= (float)item.SecondRejectionRate;
                }

            }
            else if (search.Color != "ALL")
            {

                //int opUID = int.Parse(search.OPType);
                //var query = from mg in DataContext.QualityAssurance_MgData
                //            join fm in DataContext.FlowChart_Master on mg.FlowChart_Master_UID equals fm.FlowChart_Master_UID
                //            join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                //            where fm.FlowChart_Master_UID == search.FlowChart_Master_UID
                //            && (fd.Color == search.Color||fd.Color=="")
                //            && mg.ProductDate == search.ProductDate
                //            select mg;

                //foreach (var item in query)
                //{
                //    first *= (float)item.SecondRejectionRate;
                //}

                string sql = string.Format(@"
	SELECT EXP(SUM(Log(Fm.SecondRejectionRate))) FROM  dbo.[QualityAssurance_MgData] FM JOIN dbo.FlowChart_Detail FD ON FD.FlowChart_Detail_UID = FM.FlowChart_Detail_UID
	JOIN dbo.FlowChart_Master F ON F.FlowChart_Master_UID = FD.FlowChart_Master_UID WHERE 
	f.FlowChart_Master_UID={0} AND FM.ProductDate='{1}' AND (FD.Color='' OR fd.Color=N'{2}')", search.FlowChart_Master_UID, Product_Date, search.Color);

                first = DataContext.Database.SqlQuery<double>(sql).ToArray()[0];
            }
            return first.ToString();
        }
    }
    public interface IQualityAssuranceInputMasterRepository : IRepository<QAMasterVM>
    {
        QualityAssurance_InputMasterDTO QueryAssuranceInputMaster(CheckPointInputConditionModel searchModel);
        ReturnMessageQA SaveQaMasterData(QAMasterVM data);
        string ModifyQAMasterData(QAMasterVM data);
        void updateMesSynsData(QAMasterVM data);
        void updateMesSynsDataByUID(QAMasterVM data);
        List<QualityAssurance_InputMasterDTO> QueryQAHistroyDatas(QAReportSearchVM searchModel, DateTime dateNow);

        QAReportSearchProjectAndMetrialTypeVM QueryQAReportSearchProjectAndMetrialInfo(int OPType_Organization_UID);


        QAReportVM QueryQAReport(QAReportSearchVM data);

        QAReportDaySummeryDTO GetQAReportDaySummery(QAReportSearchVM search);
        string GetAllFirstTargetYield(QAReportSearchVM search);
        string GetAllSecondTargetYield(QAReportSearchVM search);
        List<QAReportDaySummeryDTO> QueryIPQCALLProcessReportSummaryAPI(QAReportSearchVM search);


        List<QAReportExceptionTypeRankDTO> GetQAReportTypeRank(QAReportSearchVM search, int yield);


        List<FlowchartColor> GetProcessByProject(int Flowchart_Master_UID, string functionPlant);

        List<QualityAssurance_InputMaster> GetQAMasterListByIdList(List<int> idList);

        QualityAssurance_InputMasterDTO GetInputMasterByID(int uid);
    }
}
