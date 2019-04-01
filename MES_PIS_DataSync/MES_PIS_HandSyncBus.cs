using ADODB;
using JEMS_3;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_PIS_DataSync
{
    public class MES_PIS_HandSyncBus
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        private static ILogMessageRecordService _LogService;

        private static IMES_PIS_SyncFailedRecordService _SyncFailedLogServer;

        private static IMesDataSyncService _mes_Service;
        private static IProcessIDTRSConfigService _process_Service;

        public MES_PIS_HandSyncBus()
        {
            ProcessIDTRSConfigService process_Service = new ProcessIDTRSConfigService
               (
                new UnitOfWork(_DatabaseFactory),
                new EnumerationRepository(_DatabaseFactory),
                new ProcessIDTRSConfigRepository(_DatabaseFactory),
                new ProductInputRepository(_DatabaseFactory),
                new MES_PIS_SyncFailedRecordRepository(_DatabaseFactory),
                new FlowChartDetailRepository(_DatabaseFactory)
               );


            MesDataSyncService mes_Service = new MesDataSyncService
               (
                   new UnitOfWork(_DatabaseFactory),
                   new EnumerationRepository(_DatabaseFactory),
                   new MesDataSyncRepository(_DatabaseFactory),
                   new ProductInputRepository(_DatabaseFactory),
                   new ProcessIDTRSConfigRepository(_DatabaseFactory)
              );

            LogMessageRecordService Log_Service = new LogMessageRecordService
                      (
                          new UnitOfWork(_DatabaseFactory),
                          new LogMessageRecodeRepository(_DatabaseFactory)
                     );

            MES_PIS_SyncFailedRecordService SyncFailedLogServer = new MES_PIS_SyncFailedRecordService
                    (
                           new MES_PIS_SyncFailedRecordRepository(_DatabaseFactory),
                           new UnitOfWork(_DatabaseFactory)
                   );

            _LogService = Log_Service;
            _mes_Service = mes_Service;
            _process_Service = process_Service;
            _SyncFailedLogServer = SyncFailedLogServer;
        }

        //添加数据
        public bool AddMES_PIS(int uid,List<MES_StationDataRecordDTO> mesDataList, CurentTimeModel CurrentDate, MesRequestParamModel mesRequest)
        {
            //是否发邮件
            var isSendEMail = false;
            var isAddFailRecord = false;
            try
            {
                var pisConfigList = _process_Service.GetSyscProcessConfig();
                var resultModelList = new List<MES_StationDataRecordDTO>();
                var convertModel = mesDataList.GroupBy(p => p.MES_ProcessName).ToDictionary(p => p.Key, m => m);
                //筛选领料--待商议
                var AllPickIngModel = convertModel.Where(p => pisConfigList.Select(m => m.MES_PickingID).Contains(p.Key));
                var AllNgModel = convertModel.Where(p => pisConfigList.Select(m => m.MES_NgID).Contains(p.Key));
                var AllReworkModel = convertModel.Where(p => pisConfigList.Select(m => m.MES_ReworkID).Contains(p.Key));
                foreach (var item in AllNgModel)
                {
                    isAddFailRecord = true;
                    MES_StationDataRecord AddPisModel = new MES_StationDataRecord();
                    AddPisModel.Date = CurrentDate.CurrentDay;
                    AddPisModel.TimeInterVal = CurrentDate.CurrentInterval;
                    AddPisModel.StartTimeInterval = CurrentDate.CurrentStartMinute;
                    AddPisModel.EndTimeInterval = CurrentDate.CurrentEndMinute;
                    AddPisModel.PIS_ProcessID = pisConfigList.Where(p => p.MES_NgID == item.Key).FirstOrDefault().PIS_ProcessID;
                    AddPisModel.PIS_ProcessName = pisConfigList.Where(p => p.MES_NgID == item.Key).FirstOrDefault().PIS_ProcessName;
                    AddPisModel.MES_ProcessID = item.Value.FirstOrDefault().MES_ProcessID;
                    AddPisModel.MES_ProcessName = item.Key;
                    AddPisModel.ProductQuantity = item.Value.Count();
                    AddPisModel.ProjectName = mesRequest.CustomerIDString;
                    AddPisModel.ProcessType = "PIS-NG";
                    //resultModelList.Add(AddPisModel);
                    var result = _mes_Service.AddMesData(AddPisModel);
                    if (!result)
                    {
                        isSendEMail = true;
                        //AddMESSyncFailedLog("Failed", "AddPIS", mesRequest);
                    }
                }

                foreach (var item in AllReworkModel)
                {
                    isAddFailRecord = true;
                    MES_StationDataRecord AddPisModel = new MES_StationDataRecord();
                    AddPisModel.Date = CurrentDate.CurrentDay;
                    AddPisModel.TimeInterVal = CurrentDate.CurrentInterval;
                    AddPisModel.StartTimeInterval = CurrentDate.CurrentStartMinute;
                    AddPisModel.EndTimeInterval = CurrentDate.CurrentEndMinute;
                    AddPisModel.PIS_ProcessID = pisConfigList.Where(p => p.MES_ReworkID == item.Key).FirstOrDefault().PIS_ProcessID;
                    AddPisModel.PIS_ProcessName = pisConfigList.Where(p => p.MES_ReworkID == item.Key).FirstOrDefault().PIS_ProcessName;
                    AddPisModel.MES_ProcessID = item.Value.FirstOrDefault().MES_ProcessID;
                    AddPisModel.MES_ProcessName = item.Key;
                    AddPisModel.ProductQuantity = item.Value.Count();
                    AddPisModel.ProjectName = mesRequest.CustomerIDString;
                    AddPisModel.ProcessType = "PIS-ReWork";
                    var result = _mes_Service.AddMesData(AddPisModel);
                    if (!result)
                    {
                        isSendEMail = true;
                        //AddLog("Failed", "AddPIS", mesRequest);
                        //AddMESSyncFailedLog("Failed", "AddPIS", mesRequest);
                    }
                }

                foreach (var item in AllPickIngModel)
                {
                    isAddFailRecord = true;
                    MES_StationDataRecord AddPisModel = new MES_StationDataRecord();
                    AddPisModel.Date = CurrentDate.CurrentDay;
                    AddPisModel.TimeInterVal = CurrentDate.CurrentInterval;
                    AddPisModel.StartTimeInterval = CurrentDate.CurrentStartMinute;
                    AddPisModel.EndTimeInterval = CurrentDate.CurrentEndMinute;
                    AddPisModel.PIS_ProcessID = pisConfigList.Where(p => p.MES_PickingID == item.Key).FirstOrDefault().PIS_ProcessID;
                    AddPisModel.PIS_ProcessName = pisConfigList.Where(p => p.MES_PickingID == item.Key).FirstOrDefault().PIS_ProcessName;
                    AddPisModel.MES_ProcessID = item.Value.FirstOrDefault().MES_ProcessID;
                    AddPisModel.MES_ProcessName = item.Key;
                    AddPisModel.ProductQuantity = item.Value.Count();
                    AddPisModel.ProjectName = mesRequest.CustomerIDString;
                    AddPisModel.ProcessType = "PIS-PICK";
                    //resultModelList.Add(AddPisModel);
                    var result = _mes_Service.AddMesData(AddPisModel);
                    if (!result)
                    {
                        isSendEMail = true;
                        //AddLog("Failed", "AddPIS", mesRequest);
                        //AddMESSyncFailedLog("Failed", "AddPIS", mesRequest);
                    }
                }

                //手动同步失败更新状态和失败次数
                if (isAddFailRecord)
                {
                    if (isSendEMail)
                    {
                        UpdateSyncFailedLog("MES_PIS", "MES数据手动同步", false, uid, mesRequest);
                    }
                    else
                    {
                        UpdateSyncFailedLog("MES_PIS", "MES数据手动同步", true, uid, mesRequest);
                    }
                }

                //有数据同步成功
                if (isAddFailRecord && !isSendEMail)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _LogService.AddLog(new LogMessageRecord()
                {
                    LogType = "手动同步-异常",
                    LogName = "手动同步异常",
                    LogMessage = ex.Message.ToString(),
                    LogTime = DateTime.Now,
                    LogRemark = "添加_MES_StationDataRecord"
                });
                return false;
            }
        }

        //获取MES接口
        public Recordset GetMesAPIData(MesRequestParamModel mesRequest)
        {
            //3 转换MES接口的数据
            try
            {
                WP_WIPRouteSteps wp = new WP_WIPRouteSteps();
                Recordset re = wp.ListWIPHistory(mesRequest.SqlServer, mesRequest.Database, mesRequest.UserID_ID, mesRequest.StartDate, mesRequest.EndDate, mesRequest.CustomerIDString);
                //Recordset re = wp.ListWIPHistory("azapsectusql42", "jems", 794, "2018/03/29 12:00", "2018/03/29 14:00", "17");
                return re;
            }
            catch (Exception ex)
            {
                return new Recordset();
            }
        }

        /// <summary>
        /// 判断当前时间是属于哪个时段
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string CreateCurrentInterval(DateTime date)
        {
            var currentDate = date.ToString("yyyy-MM-dd/HH:mm");
            var hour = date.Hour;
            var minite = date.Minute;
            var x = hour / 2.0;
            var stratTimeInterval = 0;
            var endTimeInterVal = 0;
            for (int i = 0; i <= 12; i++)
            {
                if (x >= i && x < i + 1)
                {
                    stratTimeInterval = i;

                    break;
                }
            }

            stratTimeInterval = stratTimeInterval * 2;
            endTimeInterVal = stratTimeInterval + 2;
            var stratTimeIntervalStr = string.Empty;
            var endTimeInterValStr = string.Empty;
            stratTimeIntervalStr = stratTimeInterval.ToString();
            endTimeInterValStr = endTimeInterVal.ToString();

            //开始时间补0
            if (stratTimeIntervalStr.Length == 1)
            {
                stratTimeIntervalStr = "0" + stratTimeInterval.ToString();
            }
            else
            {
                stratTimeIntervalStr = stratTimeInterval.ToString();
            }

            //结束时段补0
            if (endTimeInterValStr.Length == 1)
            {
                endTimeInterValStr = "0" + endTimeInterVal.ToString();
            }
            else
            {
                endTimeInterValStr = endTimeInterVal.ToString();
            }

            var resultInterval = stratTimeIntervalStr + ":00" + "-" + endTimeInterValStr + ":00";
            return resultInterval;
        }

        public List<MES_StationDataRecordDTO> ConvertMesDateToPis(Recordset mesRecode)
        {
            List<MES_StationDataRecordDTO> resultList = new List<MES_StationDataRecordDTO>();
            while (!mesRecode.EOF)
            {
                MES_StationDataRecordDTO model = new MES_StationDataRecordDTO();
                model.MES_ProcessID = mesRecode.Fields[18].Value.ToString();
                model.MES_ProcessName = mesRecode.Fields[19].Value.ToString();
                resultList.Add(model);
                mesRecode.MoveNext();
            }
            mesRecode.Close();
            return resultList;
        }
        /// <summary>
        /// 返回当前时间的日期，时段，开始10分钟，结束10分钟
        /// </summary>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public CurentTimeModel GetCurrentTime(DateTime currentDate)
        {
            var minute = currentDate.Minute;
            var currentDay = currentDate.ToString("yyyy/MM/dd");
            var timeInterval = currentDate.AddHours(+0.5);
            var currentHour = currentDate.Hour / 2.0;
            //判断当前时间的时段
            var resultCurrentInterval = string.Empty;
            var resultStartTime = 0;
            var resultStartTimeSrt = string.Empty;
            var resultEndHour = 0;
            var resultEndHourSrt = string.Empty;
            for (int i = 0; i <= 12; i++)
            {
                if (currentHour >= i & currentHour < i + 1)
                {
                    resultStartTime = i * 2;
                    break;
                }
            }

            resultEndHour = resultStartTime + 2;

            //补齐 开始时段"08:00-10:00"
            if (resultStartTime.ToString().Length == 1)
            {
                resultStartTimeSrt = "0" + resultStartTime.ToString();
            }
            else
            {
                resultStartTimeSrt = resultStartTime.ToString();
            }

            //补齐 结束时段"08:00-10:00"
            if (resultEndHour.ToString().Length == 1)
            {
                resultEndHourSrt = "0" + resultEndHour.ToString();
            }
            else
            {
                resultEndHourSrt = resultEndHour.ToString();
            }

            resultCurrentInterval = resultStartTimeSrt + ":00" + "-" + resultEndHourSrt + ":00";

            //当前的分钟数
            var currentMinute = minute / 10;
            currentMinute = currentMinute * 10;

            var currentStartMinute = currentMinute;
            var currentEndMinute = currentMinute + 10;
            var currentStartMinuteStr = string.Empty;
            var currentEndMinuteStr = string.Empty;

            if (currentEndMinute >= 60)
            {
                currentStartMinuteStr = currentDate.ToString("yyyy/MM/dd HH:50");
                currentEndMinuteStr = currentDate.AddHours(1).ToString("yyyy/MM/dd HH:00");
            }
            else if (currentStartMinute == 0)
            {
                currentStartMinuteStr = currentDate.ToString("yyyy/MM/dd HH:00");
                currentEndMinuteStr = currentDate.ToString("yyyy/MM/dd HH:10");
            }
            else
            {
                currentStartMinuteStr = currentDate.ToString("yyyy/MM/dd HH:") + currentStartMinute.ToString();
                currentEndMinuteStr = currentDate.ToString("yyyy/MM/dd HH:") + currentEndMinute.ToString();
            }

            CurentTimeModel resultModel = new CurentTimeModel
            {
                CurrentDay = currentDay,
                CurrentInterval = resultCurrentInterval,
                CurrentStartMinute = currentStartMinuteStr,
                CurrentEndMinute = currentEndMinuteStr
            };
            return resultModel;
        }


        public MES_PIS_SyncFailedRecordDTO GetSyncFailedRecordByUID(int uid)
        {
            var syncModel = _SyncFailedLogServer.GetSyncFailedRecordByRequest(uid);
            return syncModel;
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        public static void AddLog(string logType, string LogName, MesRequestParamModel LogMessMode)
        {
            var message = string.Format("调用MES接口未获取数据-参数信息-->SqlServer:{0},Database:{1},UserID_ID:{2},StartDate:{3},EndDate:{4},CustomerIDString:{5}", LogMessMode.SqlServer, LogMessMode.Database, LogMessMode.UserID_ID, LogMessMode.StartDate, LogMessMode.EndDate, LogMessMode.CustomerIDString);
            _LogService.AddLog(new LogMessageRecord()
            {
                LogType = logType,
                LogName = LogName,
                LogMessage = message,
                LogTime = DateTime.Now,
                LogRemark = "同步数据失败"
            });
        }

        public static void UpdateSyncFailedLog(string logType, string LogName, bool isSuccess,int uid, MesRequestParamModel LogMessMode)
        {
            var message = string.Format("调用MES接口未获取数据-参数信息-->SqlServer:{0},Database:{1},UserID_ID:{2},StartDate:{3},EndDate:{4},CustomerIDString:{5}", LogMessMode.SqlServer, LogMessMode.Database, LogMessMode.UserID_ID, LogMessMode.StartDate, LogMessMode.EndDate, LogMessMode.CustomerIDString);
            _SyncFailedLogServer.updateSyncFailedLog(new MES_PIS_SyncFailedRecordDTO()
            {
                MES_PIS_SyncFailedRecord_UID= uid,
                SyncType = logType,
                SyncName = LogName,
                SyncTime = DateTime.Now,
                SyncRequest = message,
                Is_ManuallySuccess = isSuccess,
                SyncResult = "同步数据失败",
                OperateTime = DateTime.Now,
            });
        }
    }
    public class CurentTimeModel
    {
        public string CurrentDay { get; set; }
        public string CurrentInterval { get; set; }
        public string CurrentStartMinute { get; set; }
        public string CurrentEndMinute { get; set; }
    }
    public class MesRequestParamModel
    {
        public string SqlServer { get; set; }
        public string Database { get; set; }
        public int UserID_ID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CustomerIDString { get; set; }
        public string AssemblyIDString { get; set; }
    }
}