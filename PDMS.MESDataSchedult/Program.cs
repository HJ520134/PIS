using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Service;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JEMS_3;
using ADODB;
using System;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using PDMS.Data;
using System.Data;
using PDMS.Common.Helpers;
using System.Timers;
using System.Threading;
using PDMS.Data.Repository.MesDataSyncReposity;

namespace PDMS.MESDataSchedult
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        private static ILogMessageRecordService _LogService;
        private static IMES_PIS_SyncFailedRecordService _SyncFailedLogServer;

        static void Main(string[] args)
        {

            //NewmMes_Service.SynchronizeMesInfo(syncParam);
            //mes_Service.SynchronizeMesInfo(syncParam);

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

            MES_SNOriginalService SNOriginalService = new MES_SNOriginalService
                   (
                          new UnitOfWork(_DatabaseFactory),
                          new MES_SNOriginalRepository(_DatabaseFactory),
                          new Mes_StationColorRepository(_DatabaseFactory),
                             new ShipMentRepository(_DatabaseFactory)
                  );

            _LogService = Log_Service;
            _SyncFailedLogServer = SyncFailedLogServer;
            EnumerationRepository enumration = new EnumerationRepository(_DatabaseFactory);

            //一 获取枚举表-构造参数
            //二 获取临时表的数据 
            //三 获取配置表的数据判断是否需要同步
            //四 判断数据是插入还是更新
            //1 获取当前日期和时段

            //测试代码
            //var starttime = DateTime.Now;
            if (false)
            {
                var i = 0;
                while (true)
                {
                    Thread t1 = new Thread(new ThreadStart(AddData));
                    t1.Start();
                    Thread.Sleep(60000);
                    Console.Write(++i + "， ");
                }
            }
            else
            {
                var startTime = string.Empty;
                var endTime = string.Empty;
                var current_Date = string.Empty;
                var current_TimeInterval = string.Empty;
                var _currentDateTime = DateTime.Now.AddMinutes(-10);
                //var _currentDateTime = Convert.ToDateTime("2018/09/28 10:10");
                //while (_currentDateTime.AddMinutes(10) < Convert.ToDateTime("2018/09/28 15:10"))
                //{
                //    _currentDateTime = _currentDateTime.AddMinutes(10);
                   var currentDateInfoModel = GetCurrentTime(_currentDateTime);
                    current_TimeInterval = currentDateInfoModel.CurrentInterval;
                    current_Date = currentDateInfoModel.CurrentDay;
                    startTime = currentDateInfoModel.CurrentStartMinute;
                    endTime = currentDateInfoModel.CurrentEndMinute;
                    //获取需要同步的专案
                    var projectList = enumration.GetMES_Project("MES_ProjectType");
                    //2 获取配置表需要同步的制程
                    var ProcessConfList = process_Service.GetSyscProcessConfig();

                    foreach (var project in projectList)
                    {
                        #region
                        List<MES_StationDataRecordDTO> MesPeocessInfoList = new List<MES_StationDataRecordDTO>();
                        var requestParam = project.Enum_Value.Split(',');
                        var SqlServer = requestParam[0].ToString();
                        var Database = requestParam[1].ToString();
                        var UserID_ID = int.Parse(requestParam[2]);
                        var customerIDString = requestParam[5].ToString();
                        var assemblyIDString = requestParam[6].ToString();
                        var customerNameString = requestParam[7].ToString();
                        //获取专案下面最新的一条数据
                        var LastDatemodel = mes_Service.GetLastSyncDate(customerIDString);
                        var model = new MES_StationDataRecordDTO();
                        Recordset resultMesData = new Recordset();
                        //第一条数据直接滴啊用接口
                        if (LastDatemodel == null)
                        {
                            MesRequestParamModel mesRequest = new MesRequestParamModel()
                            {
                                SqlServer = SqlServer,
                                Database = Database,
                                UserID_ID = UserID_ID,
                                StartDate = startTime,
                                EndDate = endTime,
                                CustomerIDString = customerIDString,
                                AssemblyIDString = assemblyIDString
                            };
                            resultMesData = GetMesAPIData(mesRequest);
                            if (resultMesData.RecordCount == 0)
                            {
                                AddLog("MES-无数据数据", "RequestMESAPI", mesRequest);
                                continue;
                            }

                            MesPeocessInfoList = ConvertMesDateToPis(resultMesData, customerNameString);
                           AddMES_PIS(mes_Service, MesPeocessInfoList, ProcessConfList, currentDateInfoModel, mesRequest);
                    }
                        else
                        {
                            //当前同步的开始时间=已经同步的结束的时间
                            if (LastDatemodel.EndTimeInterval == startTime)
                            {
                                MesRequestParamModel mesRequest = new MesRequestParamModel()
                                {
                                    SqlServer = SqlServer,
                                    Database = Database,
                                    UserID_ID = UserID_ID,
                                    StartDate = startTime,
                                    EndDate = endTime,
                                    CustomerIDString = customerIDString,
                                    AssemblyIDString = assemblyIDString
                                };

                                resultMesData = GetMesAPIData(mesRequest);
                                if (resultMesData.RecordCount == 0)
                                {
                                    AddLog("MES-无数据数据", "RequestMESAPI", mesRequest);
                                    continue;
                                }
                                MesPeocessInfoList = ConvertMesDateToPis(resultMesData, customerNameString);
                               AddMES_PIS(mes_Service, MesPeocessInfoList, ProcessConfList, currentDateInfoModel, mesRequest);
                        }
                            else
                            {
                                for (int i = 12; i >= 0; i--)
                                {
                                    Recordset resultMesDataForFail = new Recordset();
                                    //往前面推10分钟-最多查询前面一个小时数据
                                    var tempTineModel = GetCurrentTime(_currentDateTime.AddMinutes(-(10 * i)));
                                    if (Convert.ToDateTime(LastDatemodel.EndTimeInterval) > Convert.ToDateTime(tempTineModel.CurrentStartMinute))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        current_Date = tempTineModel.CurrentDay;
                                        current_TimeInterval = tempTineModel.CurrentInterval;
                                        startTime = tempTineModel.CurrentStartMinute;
                                        endTime = tempTineModel.CurrentEndMinute;
                                        MesRequestParamModel mesRequest = new MesRequestParamModel()
                                        {
                                            SqlServer = SqlServer,
                                            Database = Database,
                                            UserID_ID = UserID_ID,
                                            StartDate = startTime,
                                            EndDate = endTime,
                                            CustomerIDString = customerIDString,
                                            AssemblyIDString = assemblyIDString
                                        };
                                        resultMesDataForFail = GetMesAPIData(mesRequest);
                                        if (resultMesDataForFail.RecordCount == 0)
                                        {
                                            AddLog("MES-无数据数据", "RequestMESAPI", mesRequest);
                                            //记日志或者发邮件
                                            continue;
                                        }
                                        MesPeocessInfoList = ConvertMesDateToPis(resultMesDataForFail, customerNameString);
                                        //AddMES_PIS(mes_Service, MesPeocessInfoList, ProcessConfList, tempTineModel, mesRequest);
                                        //找到相等时的时段执行完直接跳出
                                    }
                                    //原逻辑
                                    #region 
                                    //if (LastDatemodel.EndTimeInterval == tempTineModel.CurrentStartMinute)
                                    //{
                                    //    current_Date = tempTineModel.CurrentDay;
                                    //    current_TimeInterval = tempTineModel.CurrentInterval;
                                    //    startTime = tempTineModel.CurrentStartMinute;
                                    //    endTime = tempTineModel.CurrentEndMinute;
                                    //    MesRequestParamModel mesRequest = new MesRequestParamModel()
                                    //    {
                                    //        SqlServer = SqlServer,
                                    //        Database = Database,
                                    //        UserID_ID = UserID_ID,
                                    //        StartDate = startTime,
                                    //        EndDate = endTime,
                                    //        CustomerIDString = customerIDString,
                                    //        AssemblyIDString = assemblyIDString
                                    //    };
                                    //    resultMesDataForFail = GetMesAPIData(mesRequest);
                                    //    if (resultMesDataForFail.RecordCount == 0)
                                    //    {
                                    //        AddLog("MES-无数据数据", "RequestMESAPI", mesRequest);
                                    //        //记日志或者发邮件
                                    //        continue;
                                    //    }
                                    //    //MesPeocessInfoList = ConvertMesDateToPis(resultMesDataForFail);
                                    //    //AddMES_PIS(mes_Service, MesPeocessInfoList, ProcessConfList, tempTineModel, mesRequest);
                                    //    //找到相等时的时段执行完直接跳出
                                    //    break;
                                    //}
                                    //else
                                    //{
                                    //    current_Date = tempTineModel.CurrentDay;
                                    //    current_TimeInterval = tempTineModel.CurrentInterval;
                                    //    startTime = tempTineModel.CurrentStartMinute;
                                    //    endTime = tempTineModel.CurrentEndMinute;
                                    //    MesRequestParamModel mesRequest = new MesRequestParamModel()
                                    //    {
                                    //        SqlServer = SqlServer,
                                    //        Database = Database,
                                    //        UserID_ID = UserID_ID,
                                    //        StartDate = startTime,
                                    //        EndDate = endTime,
                                    //        CustomerIDString = customerIDString,
                                    //        AssemblyIDString = assemblyIDString
                                    //    };
                                    //    resultMesDataForFail = GetMesAPIData(mesRequest);
                                    //    if (resultMesDataForFail.RecordCount == 0)
                                    //    {
                                    //        AddLog("MES-无数据数据", "RequestMESAPI", mesRequest);
                                    //        //记日志或者发邮件
                                    //        continue;
                                    //    }
                                    //    //MesPeocessInfoList = ConvertMesDateToPis(resultMesDataForFail);
                                    //    //AddMES_PIS(mes_Service, MesPeocessInfoList, ProcessConfList, tempTineModel, mesRequest);
                                    //}

                                    #endregion
                                //}
                            }
                            #endregion
                        }
                    }
                    //测试代码
                    //var endtime = DateTime.Now;
                    //Console.WriteLine("总耗时毫秒：" + (endtime - starttime).TotalMilliseconds);
                    //Console.WriteLine("总耗时秒：" + (endtime - starttime).Milliseconds);
                    //Console.WriteLine("总耗时分：" + (endtime - starttime).TotalMinutes);
                    //Console.ReadKey();
                }
            }
        }

        //添加数据
        public static void AddMES_PIS(MesDataSyncService mes_Service, List<MES_StationDataRecordDTO> mesDataList, List<ProcessIDTransformConfigDTO> pisConfigList, CurentTimeModel CurrentDate, MesRequestParamModel mesRequest)
        {
            var isSendEMail = false;
            var isAddFailRecord = false;
            try
            {
                var resultModelList = new List<MES_StationDataRecordDTO>();
                var convertModel = mesDataList.GroupBy(p => new { p.MES_ProcessID, p.Color }).ToDictionary(p => p.Key, m => m);
                var mesPickIngConfigList = pisConfigList.Select(m => new { m.PIS_ProcessID, m.PIS_ProcessName, m.MES_PickingID, m.Color }).ToList();
                var mesNgConfigList = pisConfigList.Select(m => new { m.PIS_ProcessID, m.PIS_ProcessName, m.MES_NgID, m.Color }).ToList();
                var mesReworkConfigList = pisConfigList.Select(m => new { m.PIS_ProcessID, m.PIS_ProcessName, m.MES_ReworkID, m.Color }).ToList();
                var mesGoodProductConfigList = pisConfigList.Select(m => new { m.PIS_ProcessID, m.PIS_ProcessName, m.MES_GoodProductID, m.Color }).ToList();
                //筛选领料--待商议
                var AllPickIngModel = convertModel.Where(p => mesPickIngConfigList.Where(m => m.MES_PickingID == p.Key.MES_ProcessID && m.Color == p.Key.Color).Count() > 0);
                var AllGoodProductModel = convertModel.Where(p => mesGoodProductConfigList.Where(m => m.MES_GoodProductID == p.Key.MES_ProcessID && m.Color == p.Key.Color).Count() > 0);
                var AllNgModel = convertModel.Where(p => mesNgConfigList.Where(m => m.MES_NgID == p.Key.MES_ProcessID && m.Color == p.Key.Color).Count() > 0);
                var AllReworkModel = convertModel.Where(p => mesReworkConfigList.Where(m => m.MES_ReworkID == p.Key.MES_ProcessID && m.Color == p.Key.Color).Count() > 0);

                //Pick
                foreach (var item in AllPickIngModel)
                {
                    isAddFailRecord = true;
                    MES_StationDataRecord AddPisModel = new MES_StationDataRecord();
                    var pisPickConfigModel = mesPickIngConfigList.Where(p => p.MES_PickingID == item.Key.MES_ProcessID && p.Color == item.Key.Color).FirstOrDefault();
                    AddPisModel.Date = CurrentDate.CurrentDay;
                    AddPisModel.TimeInterVal = CurrentDate.CurrentInterval;
                    AddPisModel.StartTimeInterval = CurrentDate.CurrentStartMinute;
                    AddPisModel.EndTimeInterval = CurrentDate.CurrentEndMinute;
                    AddPisModel.PIS_ProcessID = pisPickConfigModel.PIS_ProcessID;
                    AddPisModel.PIS_ProcessName = pisPickConfigModel.PIS_ProcessName;
                    AddPisModel.MES_ProcessID = item.Key.MES_ProcessID;
                    AddPisModel.MES_ProcessName = item.Value.FirstOrDefault().MES_ProcessName;
                    AddPisModel.ProductQuantity = item.Value.Count();
                    AddPisModel.ProjectName = mesRequest.CustomerIDString;
                    AddPisModel.ProcessType = "PIS-PICK";
                    //resultModelList.Add(AddPisModel);
                    var result = mes_Service.AddMesData(AddPisModel);
                    if (!result)
                    {
                        isSendEMail = true;
                    }
                }

                //GoodProduct
                foreach (var item in AllGoodProductModel)
                {
                    isAddFailRecord = true;
                    MES_StationDataRecord AddPisModel = new MES_StationDataRecord();
                    var pisPickConfigModel = mesGoodProductConfigList.Where(p => p.MES_GoodProductID == item.Key.MES_ProcessID && p.Color == item.Key.Color).FirstOrDefault();
                    AddPisModel.Date = CurrentDate.CurrentDay;
                    AddPisModel.TimeInterVal = CurrentDate.CurrentInterval;
                    AddPisModel.StartTimeInterval = CurrentDate.CurrentStartMinute;
                    AddPisModel.EndTimeInterval = CurrentDate.CurrentEndMinute;
                    AddPisModel.PIS_ProcessID = pisPickConfigModel.PIS_ProcessID;
                    AddPisModel.PIS_ProcessName = pisPickConfigModel.PIS_ProcessName;
                    AddPisModel.MES_ProcessID = item.Key.MES_ProcessID;
                    AddPisModel.MES_ProcessName = item.Value.FirstOrDefault().MES_ProcessName;
                    AddPisModel.ProductQuantity = item.Value.Count();
                    AddPisModel.ProjectName = mesRequest.CustomerIDString;
                    AddPisModel.ProcessType = "PIS-GP";
                    var result = mes_Service.AddMesData(AddPisModel);
                    if (!result)
                    {
                        isSendEMail = true;
                    }
                }

                //NG
                foreach (var item in AllNgModel)
                {
                    isAddFailRecord = true;
                    MES_StationDataRecord AddPisModel = new MES_StationDataRecord();
                    var pisNgConfigModel = mesNgConfigList.Where(p => p.MES_NgID == item.Key.MES_ProcessID && p.Color == item.Key.Color).FirstOrDefault();
                    AddPisModel.Date = CurrentDate.CurrentDay;
                    AddPisModel.TimeInterVal = CurrentDate.CurrentInterval;
                    AddPisModel.StartTimeInterval = CurrentDate.CurrentStartMinute;
                    AddPisModel.EndTimeInterval = CurrentDate.CurrentEndMinute;
                    AddPisModel.PIS_ProcessID = pisNgConfigModel.PIS_ProcessID;
                    AddPisModel.PIS_ProcessName = pisNgConfigModel.PIS_ProcessName;
                    AddPisModel.MES_ProcessID = item.Key.MES_ProcessID;
                    AddPisModel.MES_ProcessName = item.Value.FirstOrDefault().MES_ProcessName;
                    AddPisModel.ProductQuantity = item.Value.Count();
                    AddPisModel.ProjectName = mesRequest.CustomerIDString;
                    AddPisModel.ProcessType = "PIS-NG";
                    //resultModelList.Add(AddPisModel);
                    var result = mes_Service.AddMesData(AddPisModel);
                    if (!result)
                    {
                        isSendEMail = true;
                    }
                }

                //Rework
                foreach (var item in AllReworkModel)
                {
                    isAddFailRecord = true;
                    MES_StationDataRecord AddPisModel = new MES_StationDataRecord();
                    var pisReworkConfigModel = mesReworkConfigList.Where(p => p.MES_ReworkID == item.Key.MES_ProcessID && p.Color == item.Key.Color).FirstOrDefault();
                    AddPisModel.Date = CurrentDate.CurrentDay;
                    AddPisModel.TimeInterVal = CurrentDate.CurrentInterval;
                    AddPisModel.StartTimeInterval = CurrentDate.CurrentStartMinute;
                    AddPisModel.EndTimeInterval = CurrentDate.CurrentEndMinute;
                    AddPisModel.PIS_ProcessID = pisReworkConfigModel.PIS_ProcessID;
                    AddPisModel.PIS_ProcessName = pisReworkConfigModel.PIS_ProcessName;
                    AddPisModel.MES_ProcessID = item.Key.MES_ProcessID;
                    AddPisModel.MES_ProcessName = item.Value.FirstOrDefault().MES_ProcessName;
                    AddPisModel.ProductQuantity = item.Value.Count();
                    AddPisModel.ProjectName = mesRequest.CustomerIDString;
                    AddPisModel.ProcessType = "PIS-ReWork";
                    var result = mes_Service.AddMesData(AddPisModel);
                    if (!result)
                    {
                        isSendEMail = true;
                    }
                }

                if (isAddFailRecord)
                {
                    //发邮件记日志
                    if (isSendEMail)
                    {
                        AddMESSyncFailedLog("MES_PIS", "MES数据自动同步", mesRequest);
                    }
                }
                else
                {
                    AddLog("无数据", "没有获取到对应配置的数据", mesRequest);
                }
            }
            catch (Exception ex)
            {
                AddLog("Failed", "AddPIS", mesRequest);
            }
        }

        //获取MES接口
        public static Recordset GetMesAPIData(MesRequestParamModel mesRequest)
        {
            //3 转换MES接口的数据
            try
            {
                WP_WIPRouteSteps wp = new WP_WIPRouteSteps();
                Recordset re = wp.ListWIPHistory(mesRequest.SqlServer, mesRequest.Database, mesRequest.UserID_ID, mesRequest.StartDate, mesRequest.EndDate, mesRequest.CustomerIDString);
                //var i = re.GetString();
                //Recordset re = wp.ListWIPHistory("azapsectusql42", "jems", 794, "2018/03/29 12:00", "2018/03/29 14:00", "17");

                return re;
            }
            catch (Exception ex)
            {
                AddMESSyncFailedLog("MES_PIS", "MES数据自动同步", mesRequest);
                return new Recordset();
            }
        }

        /// <summary>
        /// 构造结束时间10：10-10：20
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static string GetEndTime(string startTime, string EndTime, int timeInterval)
        {
            //开始时间段
            var tempTimeStart = startTime.Split(':');
            //结束时间段
            var tempTimeEnd = EndTime.Split(':');
            //开始时段小时
            var tempBeginHour = tempTimeStart[0];
            //开始时段分钟
            var tempBeginMinute = tempTimeStart[1];

            //结束时段小时
            var tempEndHour = tempTimeEnd[0];
            //结束时段分钟
            var tempEndinMinute = tempTimeEnd[1];


            //返回的开始小时
            var ResultBeginHour = int.Parse(tempBeginHour);
            //返回的开始分钟
            var ResultBeginMinute = int.Parse(tempBeginMinute);
            //返回的结束小时
            var ResultEndHour = int.Parse(tempEndHour);
            //返回的结束分钟
            var ResultEndinMinute = int.Parse(tempEndinMinute);

            var temp = int.Parse(tempEndinMinute);
            if ((temp + timeInterval) >= 60)
            {
                ResultEndHour = ResultEndHour + 1;
                ResultEndinMinute = 0;
            }
            else
            {
                ResultEndinMinute = ResultEndinMinute + timeInterval;
            }
            var middleEndHourStr = string.Empty;
            var middleEndMinuteStr = string.Empty;
            //前面补齐
            if (ResultEndHour.ToString().Length == 1)
            {
                middleEndHourStr = "0" + ResultEndHour.ToString();
            }
            else
            {
                middleEndHourStr = ResultEndHour.ToString();
            }

            if (ResultEndinMinute == 0)
            {
                middleEndMinuteStr = "00";
            }
            else
            {
                middleEndMinuteStr = ResultEndinMinute.ToString();
            }

            if (ResultEndHour >= 24 && ResultEndinMinute > 0)
            {
                return "fasle";
            }

            return ResultEndHour.ToString() + ":" + middleEndMinuteStr;
        }


        /// <summary>
        /// 构造当前面时段的新增数据(10:00)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        public static string GetInsertEndTime(string startTime, int timeInterval)
        {
            //开始时间段
            var tempTimeStart = startTime.Split(':');
            //开始时段小时
            var tempBeginHour = tempTimeStart[0];
            //开始时段分钟
            var tempBeginMinute = tempTimeStart[1];

            //返回的开始小时
            var ResultBeginHour = int.Parse(tempBeginHour);
            //返回的开始分钟
            var ResultBeginMinute = int.Parse(tempBeginMinute);

            var middleEndHourStr = string.Empty;
            var middleEndMinuteStr = string.Empty;
            //前面补齐
            if (ResultBeginHour.ToString().Length == 1)
            {
                middleEndHourStr = "0" + ResultBeginHour.ToString();
            }
            else
            {
                middleEndHourStr = ResultBeginHour.ToString();
            }
            ResultBeginMinute = ResultBeginMinute + 10;

            if (ResultBeginHour >= 24 && ResultBeginMinute > 0)
            {
                return "false";
            }
            return middleEndHourStr + ":" + ResultBeginMinute.ToString();
        }

        /// <summary>
        /// 判断当前时间是属于哪个时段
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string CreateCurrentInterval(DateTime date)
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

        public static List<MES_StationDataRecordDTO> ConvertMesDateToPis(Recordset mesRecode, string mesProjectName)
        {
            MES_SNOriginalService SNOriginalService = new MES_SNOriginalService
               (
                      new UnitOfWork(_DatabaseFactory),
                      new MES_SNOriginalRepository(_DatabaseFactory),
                      new Mes_StationColorRepository(_DatabaseFactory),
                         new ShipMentRepository(_DatabaseFactory)
              );
            EnumerationRepository enumration = new EnumerationRepository(_DatabaseFactory);
            //获取MES已出货，需要删除的站点
            var mesDeleteProjectList = enumration.GetMesNeedDeleteStation("MES_DeleteProject", mesProjectName);
            //  var allColorStationList = SNOriginalService.GetAllStationColor();
            var allColorStationList = SNOriginalService.GetStationColorListByName(mesProjectName);
            List<MESResultModel> MESResultList = new List<MESResultModel>();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SeriesNumber", typeof(string));
            dataTable.Columns.Add("CustomerName", typeof(string));
            dataTable.Columns.Add("MES_ProcessID", typeof(int));
            dataTable.Columns.Add("StationName", typeof(string));
            dataTable.Columns.Add("Starttime", typeof(DateTime));
            dataTable.Columns.Add("Color", typeof(string));

            //获取前面的的数据测试
            var startTime = DateTime.Now;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (false)
            {
                //测试代码
                #region
                //var topDateList = SNOriginalService.GetTopDate(100);
                //foreach (var item in topDateList)
                //{
                //    MESResultModel MESResult = new MESResultModel();
                //    DataRow dataRow = dataTable.NewRow();
                //    MESResult.SeriesNumber = item.SeriesNumber;
                //    MESResult.CustomerName = item.CustomerName;
                //    MESResult.StationName = item.StationName;
                //    MESResult.StartTime = item.Starttime;
                //    MESResult.MES_ProcessID = "0";
                //    MESResult.MES_ProcessName = item.StationName;
                //    dataRow["SeriesNumber"] = item.SeriesNumber;
                //    dataRow["CustomerName"] = item.CustomerName;
                //    dataRow["Starttime"] = item.Starttime;
                //    dataRow["MES_ProcessID"] = "0";
                //    dataRow["StationName"] = item.StationName;
                //    dataRow["Color"] = "0";
                //    var key = MESResult.SeriesNumber + MESResult.CustomerName + MESResult.StationName;
                //    if (!dic.Keys.Contains(key))
                //    {
                //        dic.Add(key, MESResult.SeriesNumber);
                //        dataTable.Rows.Add(dataRow);
                //        MESResultList.Add(MESResult);
                //    }
                //}
                #endregion
            }
            else
            {
                #region 
                while (!mesRecode.EOF)
                {
                    MESResultModel MESResult = new MESResultModel();
                    DataRow dataRow = dataTable.NewRow();

                    if (mesRecode.Fields[1].Value.ToString().Length != 19)
                    {

                        mesRecode.MoveNext();
                        continue;
                    }

                    MESResult.SeriesNumber = mesRecode.Fields[1].Value.ToString();
                    MESResult.CustomerName = mesRecode.Fields[3].Value.ToString();
                    MESResult.StationName = mesRecode.Fields[17].Value.ToString();
                    MESResult.StartTime = Convert.ToDateTime(mesRecode.Fields[11].Value.ToString());
                    MESResult.MES_ProcessID = mesRecode.Fields[17].Value.ToString();
                    MESResult.MES_ProcessName = mesRecode.Fields[17].Value.ToString();
                    dataRow["SeriesNumber"] = mesRecode.Fields[1].Value.ToString();
                    dataRow["CustomerName"] = mesRecode.Fields[3].Value.ToString();
                    dataRow["Starttime"] = Convert.ToDateTime(mesRecode.Fields[11].Value.ToString());
                    dataRow["MES_ProcessID"] = mesRecode.Fields[17].Value.ToString();
                    dataRow["StationName"] = mesRecode.Fields[17].Value.ToString();
                    dataRow["Color"] = "0";
                    //判断MES的该10分钟的数据是不是有重复的
                    var key = MESResult.SeriesNumber + MESResult.CustomerName + MESResult.MES_ProcessID;
                    if (!dic.Keys.Contains(key))
                    {
                        dic.Add(key, MESResult.SeriesNumber);
                        dataTable.Rows.Add(dataRow);
                        MESResultList.Add(MESResult);
                    }
                    mesRecode.MoveNext();
                }
                #endregion
            }
            mesRecode.Close();

            //添加到删除的表
            DataTable dataTableShipment = new DataTable();
            dataTableShipment.Columns.Add("SeriesNumber", typeof(string));

            // 1 够造需要删除数据
            foreach (var deEnumModel in mesDeleteProjectList)
            {
                var deleteModel = MESResultList.Where(p => p.MES_ProcessID == deEnumModel.Enum_Value);
                StringBuilder sb = new StringBuilder();
                foreach (var item in deleteModel)
                {
                    DataRow dataShipMent = dataTableShipment.NewRow();
                    dataShipMent["SeriesNumber"] = item.SeriesNumber;
                    dataTableShipment.Rows.Add(dataShipMent);
                }
            }

            SNOriginalService.AddShipMent(dataTableShipment);

            //2 判断历史是否有重复的数据
            var repeatSNList = SNOriginalService.GetMES_SNOriginal(dataTable);
            Dictionary<string, string> dicRepeat = new Dictionary<string, string>();
            foreach (var item in repeatSNList)
            {
                var key = item.SeriesNumber + item.CustomerName + item.StationName;
                if (!dicRepeat.Keys.Contains(key))
                {
                    dicRepeat.Add(key, item.SeriesNumber);
                }
            }

            //测试代码
            //var endTime = DateTime.Now;
            //Console.WriteLine("执行时间毫秒: " + (endTime - startTime).TotalMilliseconds);
            //Console.WriteLine("执行时间秒: " + (endTime - startTime).TotalSeconds);
            //Console.WriteLine("执行时间分: " + (endTime - startTime).TotalMinutes);

            // 3 过滤需要数据
            List<MES_StationDataRecordDTO> resultList = new List<MES_StationDataRecordDTO>();//返回结果集合
            dataTable.Clear();
            //构造颜色
            DataTable dataTableColor = new DataTable();
            dataTableColor.Columns.Add("SeriesNumber", typeof(string));
            dataTableColor.Columns.Add("CustomerName", typeof(string));
            dataTableColor.Columns.Add("MES_ProcessID", typeof(int));
            dataTableColor.Columns.Add("StationName", typeof(string));
            dataTableColor.Columns.Add("Starttime", typeof(DateTime));
            dataTableColor.Columns.Add("Color", typeof(string));
            foreach (var item in MESResultList)
            {
                var key = item.SeriesNumber + item.CustomerName + item.MES_ProcessID;
                if (!dicRepeat.Keys.Contains(key))
                {
                    MES_StationDataRecordDTO model = new MES_StationDataRecordDTO();
                    model.MES_ProcessID = item.MES_ProcessID;
                    model.MES_ProcessName = item.MES_ProcessName;
                    DataRow dataRow = dataTableColor.NewRow();
                    dataRow["SeriesNumber"] = item.SeriesNumber;
                    dataRow["CustomerName"] = item.CustomerName;
                    dataRow["Starttime"] = item.StartTime;
                    dataRow["MES_ProcessID"] = item.MES_ProcessID;
                    dataRow["StationName"] = item.StationName;
                    DataRow dataOriRow = dataTable.NewRow();
                    dataOriRow["SeriesNumber"] = item.SeriesNumber;
                    dataOriRow["CustomerName"] = item.CustomerName;
                    dataOriRow["Starttime"] = item.StartTime;
                    dataOriRow["MES_ProcessID"] = item.MES_ProcessID;
                    dataOriRow["StationName"] = item.StationName;

                    var temoStationColor = allColorStationList.Where(p => p.CustomerName == item.CustomerName && p.StationName == item.MES_ProcessID);
                    if (temoStationColor != null && temoStationColor.Count() > 0)
                    {
                        //第一次经过工站的料品
                        var tempColor = temoStationColor.FirstOrDefault();
                        dataOriRow["Color"] = tempColor.Color;
                        dataTable.Rows.Add(dataOriRow);
                        model.Color = tempColor.Color;
                        resultList.Add(model);
                    }
                    else
                    {
                        dataRow["Color"] = "0";
                        dataTableColor.Rows.Add(dataRow);
                    }
                }
            }
            // 4将过滤后的批量数据插入
            SNOriginalService.AddMES_SNOriginal(dataTable);
            //批量获取该10分钟的SN经过第一个站时的颜色
            var count = dataTableColor.Rows.Count;
            var firStation = new StringBuilder();
            foreach (var item in allColorStationList)
            {
                firStation.AppendFormat("\'{0}\'", item.StationName);
                firStation.Append(",");
            }

            var StationColorList = SNOriginalService.GetStationColor(dataTableColor, firStation.ToString().TrimEnd(','));
            foreach (var item in StationColorList)
            {
                MES_StationDataRecordDTO model = new MES_StationDataRecordDTO();
                model.MES_ProcessName = item.StationName;
                model.MES_ProcessID = item.MES_ProcessID.ToString();
                model.Color = item.Color;
                resultList.Add(model);
            }

            SNOriginalService.AddMES_SNOriginal(dataTableColor);
            return resultList;
        }

        /// <summary>
        /// 返回当前时间的日期，时段，开始10分钟，结束10分钟
        /// </summary>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static CurentTimeModel GetCurrentTime(DateTime currentDate)
        {
            var minute = currentDate.Minute;
            var currentDay = currentDate.AddMinutes(30).ToString("yyyy/MM/dd");
            var timeInterval = currentDate.AddHours(+0.5);
            var currentHour = timeInterval.Hour / 2.0;

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

        /// <summary>
        /// 添加日志
        /// </summary>
        public static void AddLog(string logType, string LogName, MesRequestParamModel LogMessMode)
        {
            var message = string.Format("MES无数据-参数信息：SqlServer_{0},Database_{1},UserID_{2},StartDate_{3},EndDate_{4},CustomerIDString_{5}", LogMessMode.SqlServer, LogMessMode.Database, LogMessMode.UserID_ID, LogMessMode.StartDate, LogMessMode.EndDate, LogMessMode.CustomerIDString);
            _LogService.AddLog(new LogMessageRecord()
            {
                LogType = logType,
                LogName = LogName,
                LogMessage = message,
                LogTime = DateTime.Now,
                LogRemark = "同步数据失败"
            });
        }

        public static void AddMESSyncFailedLog(string logType, string LogName, MesRequestParamModel LogMessMode)
        {
            var message = string.Format("同步失败-参数信息-->:SqlServer_{0},Database_{1},UserID_{2},StartDate_{3},EndDate_{4},CustomerIDString_{5}", LogMessMode.SqlServer, LogMessMode.Database, LogMessMode.UserID_ID, LogMessMode.StartDate, LogMessMode.EndDate, LogMessMode.CustomerIDString);
            _SyncFailedLogServer.AddMESSyncFailedLog(new MES_PIS_SyncFailedRecord()
            {
                SyncType = logType,
                SyncName = LogName,
                SyncTime = DateTime.Now,
                SyncRequest = message,
                SyncResult = "同步数据失败",
                Is_ManuallySuccess = false,
                FailedNumber = 0,
                OperateID = 0,
                OperateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 测试代码
        /// </summary>
        public static void AddData()
        {
            MES_SNOriginalService SNOriginalService = new MES_SNOriginalService
            (
                   new UnitOfWork(_DatabaseFactory),
                   new MES_SNOriginalRepository(_DatabaseFactory),
                   new Mes_StationColorRepository(_DatabaseFactory),
                   new ShipMentRepository(_DatabaseFactory)
            );

            SNOriginalService.GetALLData();
        }
    }
    public class CurentTimeModel
    {
        public string CurrentDay { get; set; }
        public string CurrentInterval { get; set; }
        public string CurrentStartMinute { get; set; }
        public string CurrentEndMinute { get; set; }
    }

    /// <summary>
    ///调用MES接口请求参数Model
    /// </summary>
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
