using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public class ProcessIDTRSConfigRepository : RepositoryBase<ProcessIDTransformConfig>, IProcessIDTRSConfigRepository
    {
        public ProcessIDTRSConfigRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// 添加PIS-MES制程配置信息
        /// </summary>
        /// <param name="AddData"></param>
        /// <returns></returns>
        public string AddProcessIDConfigInfo(List<ProcessIDTransformConfig> AddData)
        {
            var resulrMessage = string.Empty;
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    StringBuilder sb_insert = new StringBuilder();
                    StringBuilder sb_updateSql = new StringBuilder();
                    foreach (var item in AddData)
                    {
                        var isExistConfig = DataContext.ProcessIDTransformConfig.Where(p => p.PIS_ProcessID == item.PIS_ProcessID&&p.Color==item.Color && (p.MES_PickingID == item.MES_PickingID && p.MES_GoodProductID == item.MES_GoodProductID && p.MES_NgID == item.MES_NgID && p.MES_ReworkID == item.MES_ReworkID));
                        if (isExistConfig.Count() > 0)
                        {
                            var updateSql = $" UPDATE ProcessIDTransformConfig SET PIS_ProcessName = N'{item.PIS_ProcessName}', MES_NgID = N'{item.MES_NgID}', MES_PickingID = N'{item.MES_PickingID}', MES_GoodProductID = N'{item.MES_GoodProductID}', MES_ReworkID = N'{item.MES_ReworkID}',Modified_UID ={item.Modified_UID},Modified_Date ='{item.Modified_Date}',ReMark = N'{item.ReMark}',Color = N'{item.Color}',IsEnabled = N'{item.IsEnabled}',IsSyncNG = N'{item.IsSyncNG}' WHERE PIS_ProcessID = {item.PIS_ProcessID} and  MES_NgID = N'{item.MES_NgID}' and MES_PickingID =N'{item.MES_PickingID}' and MES_GoodProductID = N'{item.MES_GoodProductID}' and MES_ReworkID = N'{item.MES_ReworkID}' and  Color = N'{item.Color}'";
                            sb_insert.AppendLine(updateSql);
                        }
                        else
                        {
                            var SQl = @" INSERT INTO dbo.ProcessIDTransformConfig
                                      ( Binding_Seq,
                                        PIS_ProcessID,
                                        PIS_ProcessName,
                                        MES_NgID,
                                        MES_PickingID,
                                        MES_ReworkID,
                                        Modified_UID,
                                        Modified_Date,
                                        ReMark,
                                        Color,
                                        MES_GoodProductID,
                                        IsEnabled,
                                        IsSyncNG
                                      )
                              VALUES(   
                                        {0}, --PIS_ProcessID - int
                                        {1}, --PIS_ProcessID - int
                                        N'{2}', --PIS_ProcessName - nvarchar(20)
                                        N'{3}', --MES_NgID - nvarchar(20)
                                        N'{4}', --MES_PickingID - nvarchar(20)
                                        N'{5}', --MES_ReworkID - nvarchar(20)
                                        {6}, --Modified_UID - int
                                        '{7}', --Modified_Date - datetime
                                        N'{8}',-- ReMark - nvarchar(50)
                                        N'{9}',-- ReMark - nvarchar(50)
                                        N'{10}',-- MES_GoodProductID - nvarchar(50)
                                        N'{11}',-- IsEnabled - nvarchar(50)
                                        N'{12}'-- IsSyncNG - nvarchar(50)
                                      )";

                            SQl = string.Format(SQl,
                                          item.Binding_Seq,
                                          item.PIS_ProcessID,
                                          item.PIS_ProcessName,
                                          item.MES_NgID,
                                          item.MES_PickingID,
                                          item.MES_ReworkID,
                                          item.Modified_UID,
                                          item.Modified_Date,
                                          item.ReMark,
                                          item.Color,
                                          item.MES_GoodProductID,
                                          item.IsEnabled,
                                          item.IsSyncNG
                                          );
                            sb_insert.AppendLine(SQl);
                        }
                    }

                    if (!string.IsNullOrEmpty(sb_updateSql.ToString()))
                    {
                        DataContext.Database.ExecuteSqlCommand(sb_updateSql.ToString());
                    }

                    if (!string.IsNullOrEmpty(sb_insert.ToString()))
                    {
                        DataContext.Database.ExecuteSqlCommand(sb_insert.ToString());
                    }
                }
                catch (Exception ex)
                {
                    return resulrMessage = "导入失败";
                }

                trans.Commit();
            }

            return resulrMessage = "SUCCESS";
        }

        public string AddOrEditProcessInfo(ProcessIDTransformConfigDTO editModel)
        {
            if (editModel.ProcessTransformConfig_UID == 0)
            {
                return "没有找到该条数据，无法更新";
            }

            var updateSql = $" UPDATE ProcessIDTransformConfig SET PIS_ProcessName = N'{editModel.PIS_ProcessName}', MES_NgID = N'{editModel.MES_NgID}', MES_PickingID = N'{editModel.MES_PickingID}', MES_GoodProductID = N'{editModel.MES_GoodProductID}', MES_ReworkID = N'{editModel.MES_ReworkID}',Modified_UID ={ editModel.Modified_UID},Modified_Date ='{ editModel.Modified_Date}',ReMark = N'{editModel.ReMark}',Color = N'{editModel.Color}',IsEnabled = N'{editModel.IsEnabled}',IsSyncNG = N'{editModel.IsSyncNG}' WHERE ProcessTransformConfig_UID = { editModel.ProcessTransformConfig_UID}";
            var result = DataContext.Database.ExecuteSqlCommand(updateSql.ToString());
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "更新失败";
            }
        }

        /// <summary>
        /// 获取制程配置信息的UID
        /// </summary>
        /// <param name="Process_UID"></param>
        /// <returns></returns>
        public ProcessIDTransformConfigDTO GetProcessDataByUID(int Process_UID)
        {
            var query = from process in DataContext.ProcessIDTransformConfig
                        where process.ProcessTransformConfig_UID == Process_UID
                        select new ProcessIDTransformConfigDTO
                        {
                            ProcessTransformConfig_UID = process.ProcessTransformConfig_UID
                            ,
                            Binding_Seq = process.Binding_Seq
                            ,
                            PIS_ProcessID = process.PIS_ProcessID
                            ,
                            PIS_ProcessName = process.PIS_ProcessName
                            ,
                            MES_NgID = process.MES_NgID
                            ,
                            MES_PickingID = process.MES_PickingID
                            ,
                            MES_GoodProductID = process.MES_GoodProductID
                            ,
                            MES_ReworkID = process.MES_ReworkID
                            ,
                            Color = process.Color
                            ,
                            Modified_UID = process.Modified_UID
                            ,
                            Modified_Date = process.Modified_Date
                            ,
                            ReMark = process.ReMark
                              ,
                            IsEnabled = process.IsEnabled
                              ,
                            IsSyncNG = process.IsSyncNG
                        };

            return query.FirstOrDefault();
        }

        public bool AddMesDataInfo()
        {
            return false;
        }

        /// <summary>
        ///获取当前时段的数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        public List<MES_StationDataRecord> GteMesDataInfoByDate(string date, string timeInterval, int FlowChart_Master_UID)
        {
            var paramDate = System.Convert.ToDateTime(date).ToString("yyyy/MM/dd");
            var query = from MES in DataContext.MES_StationDataRecord
                        where MES.Date == paramDate && MES.TimeInterVal == timeInterval && MES.FlowChart_Detail.FlowChart_Master_UID == FlowChart_Master_UID
                        select MES;
            return query.OrderByDescending(p=>p.PIS_ProcessID).ToList();
        }


        /// <summary>
        /// 同步 FlowChartDetial_UID,日期，时段获取数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeInterval"></param>
        /// <param name="flowChartDetailUID"></param>
        /// <returns></returns>
        public List<MES_StationDataRecord> GetProcessDataById(string date, string timeInterval, int flowChartDetailUID, string syncType)
        {
            var paramDate = System.Convert.ToDateTime(date).ToString("yyyy/MM/dd");
            var query = from MES in DataContext.MES_StationDataRecord
                        select MES;
            query = query.Where(p => p.Date == paramDate && p.TimeInterVal == timeInterval && p.PIS_ProcessID == flowChartDetailUID && p.ProcessType == syncType);
            return query.ToList();
        }



        //获取需要同步的制程配置
        public List<ProcessIDTransformConfigDTO> GetSyscProcessConfig()
        {
            var query = from PRO in DataContext.ProcessIDTransformConfig
                        where PRO.IsEnabled==true
                        select new ProcessIDTransformConfigDTO
                        {
                            ProcessTransformConfig_UID = PRO.ProcessTransformConfig_UID,
                            PIS_ProcessID = PRO.PIS_ProcessID,
                            PIS_ProcessName = PRO.PIS_ProcessName,
                            MES_NgID = PRO.MES_NgID,
                            MES_PickingID = PRO.MES_PickingID,
                            MES_GoodProductID = PRO.MES_GoodProductID,
                            MES_ReworkID = PRO.MES_ReworkID,
                            Modified_UID = PRO.Modified_UID,
                            Modified_Date = PRO.Modified_Date,
                            ReMark = PRO.ReMark,
                            Is_Synchronous = PRO.FlowChart_Detail.Is_Synchronous,
                            Data_Source = PRO.FlowChart_Detail.Data_Source,
                            FlowChart_Master_UID = PRO.FlowChart_Detail.FlowChart_Master_UID,
                            Color = PRO.Color,
                            IsEnabled = PRO.IsEnabled,
                            IsSyncNG = PRO.IsSyncNG
                        };

            //query = query.Where(p => p.Is_Synchronous == true && p.Data_Source == "MES Global");
            return query.OrderBy(p => p.PIS_ProcessID).ToList();
        }

        public string DeleteProcessConfig(List<int> uids)
        {

            var deleteWhere = string.Empty;
            uids.ForEach(p =>
            {
                deleteWhere += "," + p;
            });
            deleteWhere = deleteWhere.TrimStart(',');

            var deleteSQl = $" DELETE FROM [ProcessIDTransformConfig] WHERE ProcessTransformConfig_UID IN ({deleteWhere});";
            var result = DataContext.Database.ExecuteSqlCommand(deleteSQl);
            if (result > 0)
            {
                return "删除成功";
            }
            else
            {
                return "删除失败";
            }
        }


        /// <summary>
        /// 通过UID删除
        /// </summary>
        /// <param name="Process_UID"></param>
        /// <returns></returns>
        public string DeleteProcessByUID(int Process_UID)
        {
            var deleteSQl = $" DELETE FROM [ProcessIDTransformConfig] WHERE ProcessTransformConfig_UID = ({Process_UID});";
            var result = DataContext.Database.ExecuteSqlCommand(deleteSQl);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "删除失败";
            }
        }
        public IQueryable<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO searchModel, Page page, out int totalcount)
        {
            var query = from SyncFailed in DataContext.MES_PIS_SyncFailedRecord
                        select new MES_PIS_SyncFailedRecordDTO
                        {
                            MES_PIS_SyncFailedRecord_UID = SyncFailed.MES_PIS_SyncFailedRecord_UID,
                            SyncType = SyncFailed.SyncType,
                            SyncName = SyncFailed.SyncName,
                            SyncTime = SyncFailed.SyncTime,
                            SyncRequest = SyncFailed.SyncRequest,
                            SyncResult = SyncFailed.SyncResult,
                            FailedNumber = SyncFailed.FailedNumber,
                            Is_ManuallySuccess = SyncFailed.Is_ManuallySuccess,
                            OperateID = SyncFailed.OperateID,
                            OperateTime = SyncFailed.OperateTime
                        };
            query = query.Where(p => p.Is_ManuallySuccess == false);
            totalcount = query.Count();
            return query.OrderByDescending(p => p.SyncTime).GetPage(page);
        }

        public MES_PIS_SyncFailedRecordDTO GetMES_PIS_SyncFailedRecordDTOByID(int MES_PIS_SyncFailedRecord_UID)
        {
            var query = from SyncFailed in DataContext.MES_PIS_SyncFailedRecord
                        select new MES_PIS_SyncFailedRecordDTO
                        {
                            MES_PIS_SyncFailedRecord_UID = SyncFailed.MES_PIS_SyncFailedRecord_UID,
                            SyncType = SyncFailed.SyncType,
                            SyncName = SyncFailed.SyncName,
                            SyncTime = SyncFailed.SyncTime,
                            SyncRequest = SyncFailed.SyncRequest,
                            SyncResult = SyncFailed.SyncResult,
                            FailedNumber = SyncFailed.FailedNumber,
                            Is_ManuallySuccess = SyncFailed.Is_ManuallySuccess,
                            OperateID = SyncFailed.MES_PIS_SyncFailedRecord_UID,
                            OperateTime = SyncFailed.OperateTime
                        };
            query = query.Where(p => p.Is_ManuallySuccess == false);
            return query.FirstOrDefault(o => o.MES_PIS_SyncFailedRecord_UID == MES_PIS_SyncFailedRecord_UID);
        }


      
    }

    public interface IProcessIDTRSConfigRepository : IRepository<ProcessIDTransformConfig>
    {
        string AddProcessIDConfigInfo(List<ProcessIDTransformConfig> AddData);

        //获取需要同步的制程
        List<ProcessIDTransformConfigDTO> GetSyscProcessConfig();

        ProcessIDTransformConfigDTO GetProcessDataByUID(int Process_UID);

        string AddOrEditProcessInfo(ProcessIDTransformConfigDTO editModel);

        string DeleteProcessConfig(List<int> uids);
        string DeleteProcessByUID(int Process_UID);
        bool AddMesDataInfo();

        List<MES_StationDataRecord> GteMesDataInfoByDate(string date, string timeInterval, int FlowChart_Master_UID);

        List<MES_StationDataRecord> GetProcessDataById(string date, string timeInterval, int flowChartDetailUID, string syncType);

        IQueryable<MES_PIS_SyncFailedRecordDTO> QuerySyncFailedRecord(MES_PIS_SyncFailedRecordDTO searchModel, Page page, out int totalcount);
        MES_PIS_SyncFailedRecordDTO GetMES_PIS_SyncFailedRecordDTOByID(int MES_PIS_SyncFailedRecord_UID);

        /// <summary>
        ///判断该良品数是否需要同步
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeInterval"></param>
        /// <param name="FlowChart_Master_UID"></param>
        /// <returns></returns>
        // ProcessIDTransformConfigDTO  IsNeedGPSyncNG(int FlowChart_Master_UID, string color);
    }
}
