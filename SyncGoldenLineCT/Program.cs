using PDMS.Common.Enums;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncGoldenLineCT
{
    class Program
    {
        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        static void Main(string[] args)
        {
            //开始任务 SyncGoldenLineCT
            var t1 = Task.Factory.StartNew(() =>
            {
                var taskName = "SyncGoldenLineCT";
                try
                {
                    Console.WriteLine("Task Started: {0}", taskName);
                    SyncGoldenLineCT();
                    Console.WriteLine("Task Complated: {0}", taskName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Task Exception: ({0}){1}", taskName, ex.Message);
                }
            });

            //开始任务 SyncLineShiftPerf
            //var t2 = Task.Factory.StartNew(() =>
            //{
            //    var taskName = "SyncLineShiftPerf";
            //    try
            //    {
            //        Console.WriteLine("Task Started: {0}", taskName);
            //        SyncLineShiftPerf();
            //        Console.WriteLine("Task Complated: {0}", taskName);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Task Exception: ({0}){1}", taskName, ex.Message);
            //    }
            //});

            //开始任务 SyncWIPHourOutput
            var t3 = Task.Factory.StartNew(() =>
            {
                var taskName = "SyncWIPHourOutput";
                try
                {
                    Console.WriteLine("Task Started: {0}", taskName);
                    SyncWIPHourOutput();
                    Console.WriteLine("Task Complated: {0}", taskName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Task Exception: ({0}){1}", taskName, ex.Message);
                }
            });

            Task.WaitAll(
                t1,
                //t2,
                t3
                );

            //以下代码废弃
            //try
            //{
            //    SyncGoldenLineCT();
            //}
            //catch (Exception ex)
            //{

            //}
            //try
            //{
            //    //同步GL_LineShiftPerf 数据
            //    SyncLineShiftPerf();

            //    //(废弃)同步宕机之前的数据,不含当天日期，参数为1表示昨天，参数为2表示前天，以此类推
            //    //SyncLineShiftPerfIfServerDown(4);

            //    //更新几天前的数据，不需要加到排程，暂时手动调用
            //    //UpdateLineShiftPerfIfServerDown(4);
            //}
            ////同步GL_LineShiftPerf 数据
            //catch (Exception ex)
            //{

            //}

            //try
            //{
            //    // 同步每小时产能
            //    SyncWIPHourOutput();
            //}
            ////同步GL_LineShiftPerf 数据
            //catch (Exception ex)
            //{

            //}
        }

        /// <summary>
        /// 同步每小时产能
        /// </summary>
        /// <returns></returns>
        public static void SyncWIPHourOutput()
        {
            GL_WIPHourOutputService WIPHourOutputService = new GL_WIPHourOutputService
              (
                  new GL_WIPHourOutputRepository(_DatabaseFactory),
                  new GoldenLineRepository(_DatabaseFactory),
                  new GL_LineShiftPerfRepository(_DatabaseFactory),
                  new GL_BuildPlanRepository(_DatabaseFactory),
                  new GL_ShiftTimeRepository(_DatabaseFactory),
                  new UnitOfWork(_DatabaseFactory)
             );
            //for (int j = 0; j < 34; j++)
            //{
            for (int i = 0; i < 12; i++)
            {
                //WIPHourOutputService.ExcuteGL_WIPHourOutput(DateTime.Now.AddHours(-i));//DateTime.Now
                //WIPHourOutputService.ExcuteGL_WIPHourOutput(Convert.ToDateTime(DateTime.Now.ToShortDateString()).AddDays(-j).AddHours(-i));//DateTime.Now

                WIPHourOutputService.NewExcuteGL_WIPHourOutput(DateTime.Now.AddHours(-i));
                //WIPHourOutputService.NewExcuteGL_WIPHourOutput(Convert.ToDateTime(DateTime.Now.ToShortDateString()).AddDays(-j).AddHours(-i));
            }
            //}
        }
        /// <summary>
        /// 同步数据
        /// </summary>
        /// <returns></returns>
        public static string SyncGoldenLineCT()
        {
            string result = "";
            GL_GoldenStationCTRecordService gL_GoldenStationCTRecordService = new GL_GoldenStationCTRecordService(
            new GL_GoldenStationCTRecordRepository(_DatabaseFactory),
            new UnitOfWork(_DatabaseFactory)
            );
            //获取所有要同步的站点
            var stationDTOs = gL_GoldenStationCTRecordService.GetStationDTO();
            //获取所有的班次
            var shiftTimeDTOs = gL_GoldenStationCTRecordService.GetShiftTimeDTO(0, 0);


            //获取所有线
            var lineDTOs = gL_GoldenStationCTRecordService.GetLineDTO();
            List<GoldenLineCTRecordDTO> GoldenLineCTRecordDTOs = new List<GoldenLineCTRecordDTO>();
            DateTime dateNow = DateTime.Now;
            // DateTime dateNow = DateTime.Now.AddDays(-1).AddHours(7);

            if (stationDTOs != null && stationDTOs.Count > 0)
            {
                stationDTOs = stationDTOs.Where(o => o.IsGoldenLine == true).ToList();
                string connectionString = "";
                foreach (var item in stationDTOs)
                {

                    if (item.Plant_Organization_UID == 1)
                    {
                        connectionString = GetCTUConnectionStrings();
                    }
                    else if (item.Plant_Organization_UID == 35)
                    {
                        connectionString = GetConnectionStrings();

                    }

                    var shiftTimes = shiftTimeDTOs.Where(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID);

                    var shiftTimeDTO = GetGL_ShiftTimeDTO(shiftTimes.ToList(), dateNow);
                    DateTime StartTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + shiftTimeDTO.StartTime));
                    DateTime EndTime = Convert.ToDateTime((dateNow.ToString("yyyy-MM-dd ") + shiftTimeDTO.End_Time));
                    DateTime CycleTimeDate = dateNow;

                    //同步当前班次的数据

                    if (StartTime > EndTime)
                    {
                        // EndTime = EndTime.AddDays(1);
                        StartTime = StartTime.AddDays(-1);
                        CycleTimeDate = StartTime;

                        //if (CycleTimeDate.Date.ToString("yyyy-MM-dd") == EndTime.Date.ToString("yyyy-MM-dd"))
                        //{
                        //    CycleTimeDate = CycleTimeDate.AddDays(-1);
                        //}

                    }

                    var WipeventDTOs = GetWipevent(item.MESProjectName, item.MESLineName, item.MESStationName, StartTime, dateNow, connectionString);

                    if (WipeventDTOs != null)
                    {
                        var goldenLineCTRecordDTO = GetGoldenLineCTRecords(WipeventDTOs, item.LineCycleTime, item.StationID, shiftTimeDTO.ShiftTimeID, StartTime, EndTime, CycleTimeDate.ToString("yyyy-MM-dd"));

                        if (goldenLineCTRecordDTO != null)
                        {
                            GoldenLineCTRecordDTOs.Add(goldenLineCTRecordDTO);
                        }
                    }

                    // 同步更新上一个班次的数据 这里要判断是否同步

                    ///Start StartTime 代表最后同步时间 及上一个班次的当前时间
                    var nextshiftTimeDTO = GetGL_ShiftTimeDTO(shiftTimes.ToList(), StartTime);
                    DateTime nextStartTime = Convert.ToDateTime((StartTime.ToString("yyyy-MM-dd ") + nextshiftTimeDTO.StartTime));
                    DateTime nextEndTime = StartTime;
                    DateTime nextCycleTimeDate = StartTime;

                    if (nextStartTime > nextEndTime)
                    {
                        nextStartTime = nextStartTime.AddDays(-1);
                        nextCycleTimeDate = nextStartTime;
                        //if (nextCycleTimeDate.Date.ToString("yyyy-MM-dd") == nextEndTime.Date.ToString("yyyy-MM-dd"))
                        //{
                        //    nextCycleTimeDate = nextCycleTimeDate.AddDays(-1);
                        //}

                    }

                    //根据当前的班次，站点，日期找出当前站点上一个班次的数据明细，获取最后更新时间
                    var nextGoldenLineCTRecordDTOs = gL_GoldenStationCTRecordService.GetGoldenLineCTRecordDTO(item.StationID, nextshiftTimeDTO.ShiftTimeID, nextCycleTimeDate.ToString("yyyy-MM-dd"));
                    if (nextGoldenLineCTRecordDTOs != null)
                    {

                        var GoldenLineCTRecordDTO = nextGoldenLineCTRecordDTOs.FirstOrDefault();

                        if (GoldenLineCTRecordDTO != null)
                        {
                            DateTime nextstratEndTime = nextEndTime.AddMinutes(-5);
                            //最后超过5分钟没同步就必须同步
                            if (GoldenLineCTRecordDTO.UpdateTime <= nextstratEndTime)
                            {
                                var nextWipeventDTOs = GetWipevent(item.MESProjectName, item.MESLineName, item.MESStationName, nextStartTime, nextEndTime, connectionString);
                                if (nextWipeventDTOs != null)
                                {
                                    var goldenLineCTRecordDTO = GetGoldenLineCTRecords(nextWipeventDTOs, item.LineCycleTime, item.StationID, nextshiftTimeDTO.ShiftTimeID, nextStartTime, nextEndTime, nextCycleTimeDate.ToString("yyyy-MM-dd"));
                                    if (goldenLineCTRecordDTO != null)
                                    {
                                        GoldenLineCTRecordDTOs.Add(goldenLineCTRecordDTO);
                                    }
                                }
                            }
                        }
                    }
                    ///End
                }
            }

            result = gL_GoldenStationCTRecordService.InsertANDUpdateGoldenLineCTRecord(GoldenLineCTRecordDTOs);
            return result;
        }
        /// <summary>
        /// 获取时间段的班次
        /// </summary>
        /// <param name="GL_ShiftTimeDTOs"></param>
        /// <param name="dateNow"></param>
        /// <returns></returns>
        public static GL_ShiftTimeDTO GetGL_ShiftTimeDTO(List<GL_ShiftTimeDTO> GL_ShiftTimeDTOs, DateTime dateNow)
        {
            GL_ShiftTimeDTO GL_ShiftTimeDTO = new GL_ShiftTimeDTO();
            string strdateNow = dateNow.ToString("yyyy-MM-dd");
            foreach (var item in GL_ShiftTimeDTOs)
            {
                DateTime StartdDateTime = Convert.ToDateTime((strdateNow + " " + item.StartTime));
                DateTime EndDateTime = Convert.ToDateTime((strdateNow + " " + item.End_Time));
                if (StartdDateTime <= EndDateTime)
                {
                    if ((StartdDateTime < dateNow && dateNow <= EndDateTime) || (StartdDateTime.AddDays(-1) < dateNow && dateNow <= EndDateTime.AddDays(-1)) || (StartdDateTime.AddDays(1) < dateNow && dateNow <= EndDateTime.AddDays(1)))
                    {
                        GL_ShiftTimeDTO = item;
                    }
                }
                else
                {
                    EndDateTime = EndDateTime.AddDays(1);
                    if ((StartdDateTime < dateNow && dateNow <= EndDateTime) || (StartdDateTime.AddDays(-1) < dateNow && dateNow <= EndDateTime.AddDays(-1)) || (StartdDateTime.AddDays(1) < dateNow && dateNow <= EndDateTime.AddDays(1)))
                    {
                        GL_ShiftTimeDTO = item;
                    }
                }

            }

            return GL_ShiftTimeDTO;
        }
        public static GL_ShiftTime GetCurrentShift(List<GL_ShiftTime> shiftTimeList, DateTime now)
        {
            GL_ShiftTime shiftTime = null;
            foreach (var item in shiftTimeList)
            {
                var startHourMinitePair = item.StartTime.Split(':');
                var startHour = int.Parse(startHourMinitePair[0]);
                var startMinute = int.Parse(startHourMinitePair[1]);

                var endHourMinitePair = item.End_Time.Split(':');
                var endHour = int.Parse(endHourMinitePair[0]);
                var endMinute = int.Parse(endHourMinitePair[1]);

                var startDateTime = new DateTime(now.Year, now.Month, now.Day, startHour, startMinute, 0);
                var endDateTime = new DateTime(now.Year, now.Month, now.Day, endHour, endMinute, 0);
                if (startDateTime < endDateTime)
                {
                    if (now >= startDateTime && now < endDateTime)
                    {
                        shiftTime = item;
                    }
                }
                else if (startDateTime > endDateTime)
                {
                    if (now >= startDateTime || now < endDateTime)
                    {
                        shiftTime = item;
                    }
                }
            }
            return shiftTime;
        }

        /// <summary>
        /// 返回配置文件数据库链接
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionStrings()
        {
            return ConfigurationManager.ConnectionStrings["SyncGoldenLineCT"].ConnectionString;
        }
        public static string GetCTUConnectionStrings()
        {
            return ConfigurationManager.ConnectionStrings["SyncGoldenLineCTCTU"].ConnectionString;
        }
        /// <summary>
        /// 获取指定时间段的数据
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="LineName"></param>
        /// <param name="StationName"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public static List<WipeventDTO> GetWipevent(string CustomerName, string LineName, string StationName, DateTime StartTime, DateTime EndTime, string connectionString)
        {

            List<WipeventDTO> WipeventDTOs = new List<WipeventDTO>();
            WipeventDTO model = null;
            string cmdText = @"SELECT * FROM wipevent where CustomerName=@CustomerName and LineName=@LineName and StationName=@StationName and StartTime>=@StartTime AND  StartTime<=@EndTime;";
            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@CustomerName",CustomerName),
                new SqlParameter("@LineName",LineName),
                new SqlParameter("@StationName",StationName),
                new SqlParameter("@StartTime",StartTime),
                new SqlParameter("@EndTime",EndTime)
            };
            CommandType commandType = CommandType.Text;
            //using (SqlDataReader dr = SqlHelper.ExecuteReader(GetConnectionStrings(), commandType, cmdText, paras))
            using (SqlDataReader dr = SqlHelper.ExecuteReader(connectionString, commandType, cmdText, paras))
            {
                while (dr.Read())
                {
                    model = new WipeventDTO
                    {

                        ID = Convert.ToInt32(dr["ID"]),
                        CustomerId = Convert.ToInt32(dr["CustomerId"]),
                        CustomerName = Convert.ToString(dr["CustomerName"]),
                        LineId = Convert.ToInt32(dr["LineId"]),
                        LineName = Convert.ToString(dr["LineName"]),
                        StationId = Convert.ToInt32(dr["StationId"]),
                        StationName = Convert.ToString(dr["StationName"]),
                        MachineName = Convert.ToString(dr["MachineName"]),
                        AssemblyId = Convert.ToInt32(dr["AssemblyId"]),
                        AssemblyNumber = Convert.ToString(dr["AssemblyNumber"]),
                        AssemblyFamily = Convert.ToString(dr["AssemblyFamily"]),
                        SerialNumber = Convert.ToString(dr["SerialNumber"]),
                        StartTime = Convert.ToDateTime(dr["StartTime"]),
                        // EndTime = Convert.ToDateTime(dr["EndTime"]),
                        Status = Convert.ToString(dr["Status"]),
                        DataSource = Convert.ToInt32(dr["DataSource"])

                    };
                    WipeventDTOs.Add(model);
                };
            }
            return WipeventDTOs;
        }
        /// <summary>
        /// 获取一个站点对应的CycleTime 明细
        /// </summary>
        /// <param name="WipeventDTOs"></param>
        /// <returns></returns>
        public static GoldenLineCTRecordDTO GetGoldenLineCTRecords(List<WipeventDTO> WipeventDTOs, decimal lineCycleTime, int stationID, int shiftTimeID, DateTime StartTime, DateTime EndTime, string CycleTimeDate)
        {

            List<GoldenLineCTRecordDTO> GoldenLineCTRecordDTOs = new List<GoldenLineCTRecordDTO>();
            var machineNames = GetMachineNames(WipeventDTOs);

            List<decimal> machineCycleTimes = new List<decimal>();
            //下面循环中计算单个机台的CycleTime
            foreach (var item in machineNames)
            {

                var machineWipeventDTOs = WipeventDTOs.Where(o => o.MachineName == item).OrderByDescending(p => p.StartTime).ToList();
                machineCycleTimes.Add(GetMachineCycleTime(machineWipeventDTOs, lineCycleTime));

            }
            decimal StationCycleTime = 0;
            //从上面得到所有机台的CycleTime然后再计算站点的CycleTime
            if (machineCycleTimes.Count > 0)
            {
                StationCycleTime = machineCycleTimes.Sum() / (machineCycleTimes.Count * machineCycleTimes.Count);
            }

            //下面要组装数据表

            GoldenLineCTRecordDTO goldenLineCTRecordDTO = new GoldenLineCTRecordDTO();
            goldenLineCTRecordDTO.StationID = stationID;
            goldenLineCTRecordDTO.ShiftTimeID = shiftTimeID;
            goldenLineCTRecordDTO.CycleTime = StationCycleTime;
            goldenLineCTRecordDTO.StartTime = StartTime;
            goldenLineCTRecordDTO.EndTime = EndTime;
            goldenLineCTRecordDTO.CycleTimeDate = CycleTimeDate;
            goldenLineCTRecordDTO.UpdateTime = DateTime.Now;

            return goldenLineCTRecordDTO;

        }
        /// <summary>
        /// 获取单个机台的CycleTime
        /// </summary>
        /// <param name="machineWipeventDTOs"></param>
        /// <returns></returns>
        public static decimal GetMachineCycleTime(List<WipeventDTO> machineWipeventDTOs, decimal lineCycleTime)
        {
            decimal machineCycleTime = 0;
            List<decimal> cycleTimes = new List<decimal>();

            for (int i = 0; i < machineWipeventDTOs.Count - 1; i++)
            {

                var timenum = machineWipeventDTOs[i].StartTime - machineWipeventDTOs[i + 1].StartTime;
                cycleTimes.Add(Convert.ToInt32(timenum.TotalSeconds));

            }

            //排除 小于 10% lineCycleTime 大于 3*lineCycleTime 
            List<decimal> usableMachineCTs = new List<decimal>();

            decimal minlineCT = lineCycleTime * decimal.Parse("0.1");
            decimal maxlineCT = 3 * lineCycleTime;
            foreach (var item in cycleTimes)
            {

                if (minlineCT <= item && item <= maxlineCT)
                {
                    usableMachineCTs.Add(item);
                }
            }
            //计算得出的平均数
            decimal averageMachinecycleTime = 0;
            if (usableMachineCTs != null && usableMachineCTs.Count > 0)
            {
                averageMachinecycleTime = usableMachineCTs.Sum() / usableMachineCTs.Count;
            }

            //得到偏差
            decimal deviation = GetDeviation(usableMachineCTs, averageMachinecycleTime);
            //排除小于 averageMachinecycleTime-（3*deviation） 和大于averageMachinecycleTime+（3*deviation）
            //最终要计算的机台CT
            List<decimal> overMachineCTs = new List<decimal>();
            decimal minDeviationCT = averageMachinecycleTime - (3 * deviation);
            decimal maxDeviationCT = averageMachinecycleTime + (3 * deviation);
            foreach (var item in usableMachineCTs)
            {

                if (minDeviationCT <= item && item <= maxDeviationCT)
                {
                    overMachineCTs.Add(item);
                }
            }
            //得到最终的机台CT
            if (overMachineCTs != null && overMachineCTs.Count > 0)
            {
                machineCycleTime = overMachineCTs.Sum() / overMachineCTs.Count;
            }

            return machineCycleTime;
        }
        /// <summary>
        /// 计算偏差
        /// </summary>
        /// <param name="usableMachineCTs"></param>
        /// <param name="averageMachinecycleTime"></param>
        /// <returns></returns>
        public static decimal GetDeviation(List<decimal> usableMachineCTs, decimal averageMachinecycleTime)
        {
            var deviation = 0.0;
            List<decimal> squareDetails = new List<decimal>();
            foreach (var item in usableMachineCTs)
            {
                decimal deviationDetail = 0;
                if ((item - averageMachinecycleTime) != 0)
                {

                    deviationDetail = (item - averageMachinecycleTime) * (item - averageMachinecycleTime);
                }
                squareDetails.Add(deviationDetail);
            }
            decimal average = 0;
            if (squareDetails.Count > 0)
            {
                average = squareDetails.Sum() / squareDetails.Count;
            }
            deviation = Math.Sqrt(Convert.ToDouble(average));
            return decimal.Parse(Math.Round(deviation).ToString());

        }

        /// <summary>
        /// 获取所查询站点下的所有机台（去重）
        /// </summary>
        /// <param name="WipeventDTOs"></param>
        /// <returns></returns>
        public static List<string> GetMachineNames(List<WipeventDTO> WipeventDTOs)
        {
            return WipeventDTOs.Select(o => o.MachineName).Distinct().ToList();
        }
        
    }
    /// <summary>
    /// 基础数据表模型
    /// </summary>
    public class WipeventDTO
    {
        public int ID { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int LineId { get; set; }
        public string LineName { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; }
        public string MachineName { get; set; }
        public int AssemblyId { get; set; }
        public string AssemblyNumber { get; set; }
        public string AssemblyFamily { get; set; }
        public string SerialNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public int DataSource { get; set; }
    }

    public class GL_LineShiftPerfDTO
    {
        public int LSPID { get; set; }
        public int CustomerID { get; set; }
        public string OutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int LineID { get; set; }
        public int? AssemblyID { get; set; }
        public int PlanOutput { get; set; }
        public int StandOutput { get; set; }
        public int ActualOutput { get; set; }
        public decimal UPH { get; set; }
        public int LineStatus { get; set; }
        public int PlanDownTime { get; set; }
        public int UPDownTime { get; set; }
        public int RunTime { get; set; }
        public decimal ActualOutputVSPlan { get; set; }
        public decimal ActualOutputVSRealTimePlan { get; set; }
        public decimal ActualOutputVSStdOutput { get; set; }
        public decimal LineUtil { get; set; }
        public decimal CapacityLoading { get; set; }
        public decimal VAOLE { get; set; }

        //自定义
        public string CustomerName { get; set; }
        public string Shift { get; set; }
        public string LineName { get; set; }
    }
}
