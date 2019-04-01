using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Common.Helpers;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public class QualityAssuranceMgDataRepository : RepositoryBase<QualityAssurance_MgData>, IQualityAssuranceMgDataRepository
    {
        private Logger log = new Logger("QualityAssuranceMgDataRepository ");
        public QualityAssuranceMgDataRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public int SaveQaMasterData(QAMasterVM data)
        {
            int result = 0;

            try
            {
                double firstoK = (double)data.FirstOK_Qty;
                double firstCheck = (double)data.FirstCheck_Qty;

                string sql = string.Format(@"
                    DECLARE  @supplierUID INT
                    INSERT INTO dbo.QualityAssurance_InputMaster
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
                              Modified_Date
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
                              GETDATE()  -- Modified_Date - datetime
                             )

                              UPDATE dbo.PPForQAInterface SET QAUsedFlag=1 WHERE FlowChart_Detail_UID={0} AND Product_Date='{4}' AND Time_Interval=N'{5}' 
                                AND MaterielType=N'{3}'
                                
                             SET @supplierUID = SCOPE_IDENTITY()
                             SELECT @supplierUID
", data.FlowChart_Detail_UID,data.Process,data.Color,data.MaterialType, data.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate),data.Time_Interval,data.Input,data.FirstCheck_Qty,data.FirstOK_Qty,(1- firstoK/firstCheck),
data.NG_Qty,data.RepairOK_Qty,data.Creator_UID);

                result = DataContext.Database.SqlQuery<int>(sql).ToArray()[0];

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public List<QATargetRateVM> QueryCheckPointForTargetYield(int Flowchart_Master_UID)
        {
            List<QATargetRateVM> result = new List<QATargetRateVM>();
            try
            {
                var query = from dataMasters in DataContext.FlowChart_Master
                            join dataDetails in DataContext.FlowChart_Detail on dataMasters.FlowChart_Master_UID equals dataDetails.FlowChart_Master_UID
                            where dataMasters.FlowChart_Master_UID == Flowchart_Master_UID && dataDetails.FlowChart_Version == dataMasters.FlowChart_Version &&
                            !dataDetails.Process.Contains("包装") && !string.IsNullOrEmpty(dataDetails.IsQAProcess)
                            select new QATargetRateVM
                            {
                                Color=dataDetails.Color,
                                Process = dataDetails.Process,
                                Process_seq = dataDetails.Process_Seq,
                                FlowChart_Master_UID = dataDetails.FlowChart_Master_UID,
                                Flowchart_Detail_UID = dataDetails.FlowChart_Detail_UID
                            };

                result = query.Distinct().ToList();
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public string UpdateTargetYield(List<QualityAssurance_MgDataDTO> mgDataList)
        {
            string result = "Success";

            try
            {
                if (mgDataList.Count() == 0)
                {
                    return result;
                }

                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    foreach (var qaMgData in mgDataList)
                    {
                        var insertSql = string.Format(@" delete from dbo.QualityAssurance_MgData where FlowChart_Master_UID={4} 
                                                            and FlowChart_Detail_UID={7} and ProductDate='{2}' 

                                                       INSERT INTO dbo.QualityAssurance_MgData
                                                                ( FirstRejectionRate,
                                                                  SecondRejectionRate,
                                                                  ProductDate,
                                                                  Modified_Date,
                                                                  Modified_UID,
                                                                  FlowChart_Master_UID,
                                                                  Create_Date,
                                                                  Creator_UID,
                                                                  Process_seq,
                                                                  Process,
                                                                  FlowChart_Detail_UID
                                                                )
                                                        VALUES  ( {0} , -- FirstRejectionRate - decimal
                                                                  {1} , -- SecondRejectionRate - decimal
                                                                  '{2}' , -- ProductDate - datetime
                                                                  GETDATE() , -- Modified_Date - datetime
                                                                  '{3}' , -- Modified_UID - int
                                                                  {4} , -- FlowChart_Master_UID - int
                                                                  GETDATE() , -- Create_Date - datetime
                                                                  '{3}' , -- Creator_UID - int
                                                                  {5} , -- Process_seq - int
                                                                  N'{6}', -- Process - nvarchar(50)
                                                                   {7}  -- FlowChart_Detail_UID - int
                                                                )
                                                        ", qaMgData.FirstRejectionRate, qaMgData.SecondRejectionRate, qaMgData.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate), qaMgData.Modified_UID,
                                                                qaMgData.FlowChart_Master_UID, qaMgData.Process_seq, qaMgData.Process, qaMgData.FlowChart_Detail_UID);

                        DataContext.Database.ExecuteSqlCommand(insertSql);
                    }
                    trans.Commit();
                }

            }
            catch(Exception ex)
            {
                result = "Error";
                log.Error(ex);
            }
            return result;
        }


        public List<QualityAssurance_MgData> GetTargetYield(int FlowChart_Detail_UID, DateTime date)
        {
            DateTime endDay = date.AddDays(6);
            var query = from mgdata in DataContext.QualityAssurance_MgData
                        where mgdata.ProductDate >= date && mgdata.ProductDate <= endDay && mgdata.FlowChart_Detail_UID == FlowChart_Detail_UID
                        select mgdata;

            return query.ToList();
        }

        public List<QATargetRateVM> QueryQATargetYield(int Flowchart_Master_UID, DateTime date)
        {
            List<QATargetRateVM> result = new List<QATargetRateVM>();
            try
            {
                var Flowchart_master_uid = new SqlParameter("Flowchart_master_uid ", Flowchart_Master_UID);
                var Monday_Date = new SqlParameter("Monday_Date", date);
                var tempResult = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<QATargetRateVM>("usp_SearchQATargetYield @Flowchart_master_uid , @Monday_Date", Flowchart_master_uid, Monday_Date).ToList();
                result = tempResult;
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }
    }

    public interface IQualityAssuranceMgDataRepository : IRepository<QualityAssurance_MgData>
    {
        List<QATargetRateVM> QueryCheckPointForTargetYield(int Flowchart_Master_UID);

        string UpdateTargetYield(List<QualityAssurance_MgDataDTO> datas);

        List<QualityAssurance_MgData> GetTargetYield(int FlowChart_Detail_UID, DateTime date);

        List<QATargetRateVM> QueryQATargetYield(int Flowchart_Master_UID, DateTime date);
    }
}
