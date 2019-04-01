using PDMS.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PDMS.Machine
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        private static ILogMessageRecordService _LogService ;
        private static IMES_PIS_SyncFailedRecordService _SyncFailedLogServer;
        static void Main(string[] args)
        {
            MachineYieldReportService machineYieldReportService = new MachineYieldReportService(
                new Machine_YieldRepository(_DatabaseFactory),
                new UnitOfWork(_DatabaseFactory),
                new Machine_Schedule_ConfigRepository(_DatabaseFactory),
                new Machine_StationRepository(_DatabaseFactory),
                new Machine_CustomerRepository(_DatabaseFactory)
               
                );
            MES_PIS_SyncFailedRecordService SyncFailedLogServer = new MES_PIS_SyncFailedRecordService
                                                           (
                                                                  new MES_PIS_SyncFailedRecordRepository(_DatabaseFactory),
                                                                  new UnitOfWork(_DatabaseFactory)
                                                           );

            LogMessageRecordService Log_Service = new LogMessageRecordService
                                    (
                                        new UnitOfWork(_DatabaseFactory),
                                        new LogMessageRecodeRepository(_DatabaseFactory)
                                    );
            _LogService = Log_Service;
            _SyncFailedLogServer = SyncFailedLogServer;
            //获取站点配置参数。
            var machine_Schedule_ConfigDTOs = machineYieldReportService.GetMachine_Schedule_ConfigDTO();
           DateTime datetime = DateTime.Now;
           // DateTime datetime =Convert.ToDateTime("2018-05-18 14:20");
            // DateTime datetime = Convert.ToDateTime("2018-05-14 13:20");

            var nomachine_Schedule_ConfigDTOs =  machineYieldReportService.GetNOMachine_Schedule_ConfigDTO();
            var timeQuantum = GetTimeQuantum(datetime);
            //获取到的时候得到的数据，及有过排程的时候。
            if (machine_Schedule_ConfigDTOs.Count > 0)
            {
                //循环站点数据
                foreach (var item in machine_Schedule_ConfigDTOs)
                {
                    if (item.EndTime == timeQuantum.StartTime)
                    {
                        //30分钟
                        SetMachine_Yield(timeQuantum.StartTime, item.MES_Customer_Name, item.MES_Station_Name, 1, item.Machine_Station_UID);
                    }
                    else if (item.EndTime == timeQuantum.StartTime.AddMinutes(-30))
                    {
                        //60分钟调用2次
                        SetMachine_Yield(timeQuantum.StartTime.AddMinutes(-30), item.MES_Customer_Name, item.MES_Station_Name, 2, item.Machine_Station_UID);
                    }
                    else if (item.EndTime == timeQuantum.StartTime.AddMinutes(-60))
                    {
                        //90分钟调用3次
                        SetMachine_Yield(timeQuantum.StartTime.AddMinutes(-60), item.MES_Customer_Name, item.MES_Station_Name, 3, item.Machine_Station_UID);
                    }
                    else //if (item.EndTime <= timeQuantum.StartTime.AddMinutes(-90))
                    {
                        //120分钟调用4次
                        SetMachine_Yield(timeQuantum.StartTime.AddMinutes(-90), item.MES_Customer_Name, item.MES_Station_Name, 4, item.Machine_Station_UID);
                    }
                }
                //获取所有已经同步的过得站点ID。
                List<int> yesMachine_Station_UIDs = machine_Schedule_ConfigDTOs.Select(o => o.Machine_Station_UID).ToList();
                //获取所有没有同步过得站点
                var nmachine_Schedule_ConfigDTOs = nomachine_Schedule_ConfigDTOs.Where(o => yesMachine_Station_UIDs.Contains(o.Machine_Station_UID)==false).ToList();
                foreach (var item in nmachine_Schedule_ConfigDTOs)
                {
                    SetMachine_Yield(timeQuantum.StartTime, item.MES_Customer_Name, item.MES_Station_Name, 1, item.Machine_Station_UID);
                }


            }
            else
            {
                //从来没有排程的时候执行下面的排程
                machine_Schedule_ConfigDTOs = nomachine_Schedule_ConfigDTOs;
                foreach (var item in machine_Schedule_ConfigDTOs)
                {
                    SetMachine_Yield(timeQuantum.StartTime, item.MES_Customer_Name, item.MES_Station_Name, 1, item.Machine_Station_UID);
                }
            }

        }

        public static bool SetMachine_Yield(DateTime dateTime, string MES_Customer_Name, string MES_Station_Name, int num, int Machine_Station_UID)
        {
            bool isMachine_Yield = false;

            try
            {

                for (int i = 0; i < num; i++)
                {
                    List<Machine_YieldDTO> Machine_YieldDTOs = new List<Machine_YieldDTO>();
                    List<Machine_Schedule_ConfigDTO> Machine_Schedule_ConfigDTOs = new List<Machine_Schedule_ConfigDTO>();

                    Machine_Schedule_ConfigDTO machine_Schedule_ConfigDTO = new Machine_Schedule_ConfigDTO();
                    DateTime startTime = dateTime.AddMinutes(i * 30);
                    DateTime endTime = startTime.AddMinutes(30);
                    machine_Schedule_ConfigDTO.MES_Customer_Name = MES_Customer_Name;
                    machine_Schedule_ConfigDTO.MES_Station_Name = MES_Station_Name;
                    machine_Schedule_ConfigDTO.StarTime = startTime;
                    machine_Schedule_ConfigDTO.EndTime = endTime;
                    machine_Schedule_ConfigDTO.Machine_Station_UID = Machine_Station_UID;
                    machine_Schedule_ConfigDTO.Created_Date = DateTime.Now;
                    Machine_Schedule_ConfigDTOs.Add(machine_Schedule_ConfigDTO);

                    // var apiUrl = string.Format("pis/pdcaNgSummary?Customer={0}&Station={1}&StartTime={2}&EndTime={3}", "SHH-CTU-Housing", "Milan-HSG-UMP1", "2018-05-03 14:10", "2018-05-03 16:10");
                    // var apiUrl = string.Format("http://CNCTUG0IT777D:8080/pis/pdcaNgSummary?Customer={0}&Station={1}&StartTime={2}&EndTime={3}", MES_Customer_Name, MES_Station_Name, startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"));
                    var apiUrl = string.Format("pis/pdcaNgSummary?Customer={0}&Station={1}&StartTime={2}&EndTime={3}", MES_Customer_Name, MES_Station_Name, startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"));

                    var result = HttpGet(WebAPIPath + apiUrl);
                    JObject josnData = (JObject)JsonConvert.DeserializeObject(result);
                    var list = JsonConvert.DeserializeObject<List<MachineYield>>(josnData["data"].ToString());
                    SetMachine_YieldDTOs(list, Machine_YieldDTOs, Machine_Station_UID, startTime, endTime);
                 
                    //下面写执行插入语句.插入报表数据。
                    string message = InsertMachine_YieldAndMachineConfig(Machine_YieldDTOs, Machine_Schedule_ConfigDTOs);
                    //AddMESSyncFailedLog("Machine", "机台数据同步", WebAPIPath + apiUrl, message);
                    if (message != "SUCCESS")
                    {
                        AddMESSyncFailedLog("Machine", "机台数据同步", WebAPIPath + apiUrl, message);
                        AddLog("Machine", "机台数据同步", message);
                    }

                }
                isMachine_Yield = true;
                //下面写执行插入语句.插入报表数据。
                //  InsertMachine_YieldAndMachineConfig(Machine_YieldDTOs, Machine_Schedule_ConfigDTOs); 
            }
            catch (Exception ex)
            {
                //获取错误信息
                AddLog("Machine", "机台数据同步", ex.Message.ToString());
                isMachine_Yield = false;
            }
            return isMachine_Yield;
        }

        public static string InsertMachine_YieldAndMachineConfig(List<Machine_YieldDTO> Machine_YieldDTOs, List<Machine_Schedule_ConfigDTO> Machine_Schedule_ConfigDTOs)
        {
            MachineYieldReportService machineYieldReportService = new MachineYieldReportService(
           new Machine_YieldRepository(_DatabaseFactory),
           new UnitOfWork(_DatabaseFactory),
           new Machine_Schedule_ConfigRepository(_DatabaseFactory),        
           new Machine_StationRepository(_DatabaseFactory),
           new Machine_CustomerRepository(_DatabaseFactory)
           );
            return machineYieldReportService.InsertMachine_YieldAndMachineConfig(Machine_YieldDTOs, Machine_Schedule_ConfigDTOs);
        }

        /// <summary>
        /// 计算机台良率
        /// </summary>
        /// <param name="MachineYields">获取的参数列表</param>
        /// <param name="machine_YieldDTOs">计算的机台良率结果</param>
        /// <returns></returns>
        public static List<Machine_YieldDTO> SetMachine_YieldDTOs(List<MachineYield> MachineYields, List<Machine_YieldDTO> machine_YieldDTOs, int Machine_Station_UID, DateTime startTime, DateTime endTime)
        {

            foreach (var item in MachineYields)
            {
                Machine_YieldDTO tempMachine_YieldDTO = new Machine_YieldDTO();
                tempMachine_YieldDTO.Machine_Station_UID = Machine_Station_UID;
                tempMachine_YieldDTO.Machine_ID = item.Machine;
                tempMachine_YieldDTO.InPut_Qty = item.Input;
                tempMachine_YieldDTO.NG_Qty = item.NG;
                tempMachine_YieldDTO.NG_Point_Qty = item.NGP;
                tempMachine_YieldDTO.StarTime = startTime;
                tempMachine_YieldDTO.EndTime = endTime;
                tempMachine_YieldDTO.Created_Date = DateTime.Now;
                machine_YieldDTOs.Add(tempMachine_YieldDTO);
            }
            return machine_YieldDTOs;
        }
        /// <summary>
        /// 根据当前时间获取时间段。
        /// </summary>
        public static TimeQuantum GetTimeQuantum(DateTime datetime)
        {
            TimeQuantum timeQuantum = new TimeQuantum();
            int min = datetime.Minute;
            string EndTime = "";
            if (min < 30)
            {
                EndTime = datetime.ToString("yyyy-MM-dd HH") + ":00";
                DateTime EndDateTime = Convert.ToDateTime(EndTime);
                timeQuantum.EndTime = EndDateTime;
                timeQuantum.StartTime = EndDateTime.AddMinutes(-30);
            }
            else
            {
                EndTime = datetime.ToString("yyyy-MM-dd HH") + ":30";
                DateTime EndDateTime = Convert.ToDateTime(EndTime);
                timeQuantum.EndTime = EndDateTime;
                timeQuantum.StartTime = EndDateTime.AddMinutes(-30);
            }
            return timeQuantum;
        }
        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        private static string WebAPIPath
        {
            get { return ConfigurationManager.AppSettings["WebApiPath"].ToString(); }
        }
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// 添加日志
        /// </summary>
        public static void AddLog(string logType, string LogName, string LogMessMode)
        {
            var message =  LogMessMode;
            _LogService.AddLog(new LogMessageRecord()
            {
                LogType = logType,
                LogName = LogName,
                LogMessage = message,
                LogTime = DateTime.Now,
                LogRemark = "同步数据失败"
            });
        }
        public static void AddMESSyncFailedLog(string logType, string LogName, string url,string messagees)
        {
           // var message = string.Format("同步失败-参数信息-->SqlServer:{0},Database:{1},UserID_ID:{2},StartDate:{3},EndDate:{4},CustomerIDString:{5}", LogMessMode.SqlServer, LogMessMode.Database, LogMessMode.UserID_ID, LogMessMode.StartDate, LogMessMode.EndDate, LogMessMode.CustomerIDString);
            _SyncFailedLogServer.AddMESSyncFailedLog(new MES_PIS_SyncFailedRecord()
            {
                SyncType = logType,
                SyncName = LogName,
                SyncTime = DateTime.Now,
                SyncRequest = url,
                SyncResult = messagees,
                Is_ManuallySuccess = false,
                FailedNumber = 0,
                OperateID = 0,
                OperateTime = DateTime.Now,
            });
        }

    }
    public class TimeQuantum
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    public class MachineYield
    {
        public string Customer { get; set; }
        public string Station { get; set; }
        public string Machine { get; set; }
        public int Input { get; set; }
        public int NG { get; set; }
        public int NGP { get; set; }
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
