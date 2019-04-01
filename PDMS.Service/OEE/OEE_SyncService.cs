using Newtonsoft.Json;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IOEE_SyncService
    {
        void ExcuteOEE_Output(DateTime excuteTime);
        GL_ShiftTimeDTO GetShiftTime(int ShiftTimeID);

    }

    public class OEE_SyncService : IOEE_SyncService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IOEE_EveryDayMachineRepository OEE_OutputRepository;
        private readonly IOEE_MachineDailyDownRecordRepository OEE_MachineDailyDownRecordRepository;
        private readonly IOEE_DefectCodeDailyNumDTORepository OEE_DefectCodeDailyNumDTORepository;
        private readonly IGL_WIPHourOutputRepository GL_WIPHourOutputRepository;
        private readonly IOEE_EveryDayDFcodeMissingRepository OEE_EveryDayDFcodeMissingRepository;
        private readonly IOEE_EveryDayMachineDTCodeRepository OEE_EveryDayMachineDTCodeRepository;
        private readonly IOEE_MachineStatusRepository Oee_MachineStatusRepository;
        public OEE_SyncService(
              IOEE_EveryDayMachineRepository OEE_OutputRepository,
             IGL_WIPHourOutputRepository GL_WIPHourOutputRepository,
              IOEE_MachineDailyDownRecordRepository OEE_MachineDailyDownRecordRepository,
              IOEE_DefectCodeDailyNumDTORepository OEE_DefectCodeDailyNumDTORepository,
              IOEE_EveryDayDFcodeMissingRepository OEE_EveryDayDFcodeMissingRepository,
              IOEE_EveryDayMachineDTCodeRepository OEE_EveryDayMachineDTCodeRepository,
              IOEE_MachineStatusRepository Oee_MachineStatusRepository,
              IUnitOfWork unitOfWork)
        {
            this.OEE_OutputRepository = OEE_OutputRepository;
            this.OEE_MachineDailyDownRecordRepository = OEE_MachineDailyDownRecordRepository;
            this.GL_WIPHourOutputRepository = GL_WIPHourOutputRepository;
            this.OEE_DefectCodeDailyNumDTORepository = OEE_DefectCodeDailyNumDTORepository;
            this.OEE_EveryDayDFcodeMissingRepository = OEE_EveryDayDFcodeMissingRepository;
            this.OEE_EveryDayMachineDTCodeRepository = OEE_EveryDayMachineDTCodeRepository;
            this.Oee_MachineStatusRepository = Oee_MachineStatusRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void ExcuteOEE_Output(DateTime excuteTime)
        {
            //1 获取所有的customter
            var customerList = GL_WIPHourOutputRepository.GetAllMachineGL_Customer();
            foreach (var customerModel in customerList)
            {
                bool whetherUpdateLasteShfit = false;
                bool Latestflag = true;
                #region 判断当前时间的班次与上一次更新的班次是否相同，日期和班次相同的话就不需要同步上一次班次的数据，否则需要更新
                //根据当前专案获取最后更新时间
                var LasteUpdateTime = OEE_OutputRepository.GetLatestUpdateTime(customerModel.Project_UID);
                if (LasteUpdateTime == DateTime.MinValue)
                {
                    //获取最后更新时间属于哪一个班次
                    Latestflag = false;
                    LasteUpdateTime = DateTime.Now;
                    whetherUpdateLasteShfit = false;
                }
                #endregion
                #region 获取继续信息
                //根据OP获取设置的班次
                var shiftModelList = GL_WIPHourOutputRepository.GetShiftTimeList(customerModel.Organization_UID);
                if (shiftModelList.Count == 0)
                    continue;
                //获取当前属于哪个生产日期  如果当前“时间”不加日期的时间小于
                var currentDayTemp1 = excuteTime.AddMinutes(-5).ToShortTimeString();
                var currentDay = excuteTime.AddMinutes(-5).ToString("yyyy-MM-dd");
                var LatestcurrentDay = LasteUpdateTime.ToString("yyyy-MM-dd");
                var currentTime = excuteTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm");
                GL_ShiftTimeDTO ShiftTimeModel = new GL_ShiftTimeDTO();
                DateTime TimeIntervalStart =DateTime.Now;
                DateTime TimeIntervalEnd=DateTime.Now;
                //取出白天还是夜晚 同步12区间小时
                Double PlanHourVlue = 0.0;
                var ShiftTimeID = 0;
                var LatestShiftTimeID = 0;
                var actHourIndex = 0;
                var shiftdate = string.Empty;
                Dictionary<string, int> outputDic = new Dictionary<string, int>();
                Dictionary<string, int> ALLoutputDic = new Dictionary<string, int>();
                Dictionary<string, int> outputCNCDic = new Dictionary<string, int>();
                Dictionary<string, int> ALLoutputCNCDic = new Dictionary<string, int>();

                List<ProPerCT> PorCTS = new List<ProPerCT>();

                Dictionary<string, int> FixtureS = new Dictionary<string, int>();
                Dictionary<string, int> ALLFixtureS = new Dictionary<string, int>();

                Dictionary<string, int> CTUFixtureS = new Dictionary<string, int>();

                Dictionary<string, double> ActualCTS = new Dictionary<string, double>();
                Dictionary<string, double> ALLActualCTS = new Dictionary<string, double>();

                List<LocalCNCInfo> CNCInfoLists = new List<LocalCNCInfo>();
                List<LocalCNCInfo> ALLCNCInfoLists = new List<LocalCNCInfo>();
                Dictionary<string, double> ActualCNCCTS = new Dictionary<string, double>();
                Dictionary<string, double> ALLActualCNCCTS = new Dictionary<string, double>();
                DateTime startTimeDate = DateTime.Now;
                DateTime endTimeDate = DateTime.Now;

                string TimeInterval = string.Empty;
                DateTime LasteststartTimeDate = DateTime.Now;
                DateTime LatestendTimeDate = DateTime.Now;

                DateTime ALLstartTimeDate = DateTime.Now;
                DateTime ALLendTimeDate = DateTime.Now;  //全天的数据

                DateTime LatestALLstartTimeDate = DateTime.Now;
                DateTime LatestALLendTimeDate = DateTime.Now;  //全天的数据
                List<int> MissingDownMachineUIDList = new List<int>();  //用于存放有漏失的机台UID
                List<int> MissingDefectCodeUIDList = new List<int>();  //用于存放有漏失的机台UID
                List<int> OfflineMachineUIDList = new List<int>();  //用于存放当前断网的机台UID

                #endregion
                #region 2 判断当前时间是那个班次
                var currentdate = Convert.ToDateTime(currentTime);
                bool  flag = false;  //判断是否找到当前
                foreach (var item in shiftModelList)
                {
                    endTimeDate = Convert.ToDateTime(currentDay + " " + item.End_Time);
                    //下面判断当前时间属于哪一个时段
                    if (DateTime.Compare(DateTime.Parse(currentDayTemp1), DateTime.Parse(item.StartTime)) < 0 && DateTime.Compare(DateTime.Parse(item.StartTime), DateTime.Parse(item.End_Time)) > 0)
                    {
                        currentDay = excuteTime.AddDays(-1).AddMinutes(-5).ToString("yyyy-MM-dd");
                    }
                    startTimeDate = Convert.ToDateTime(currentDay + " " + item.StartTime);

                    if (startTimeDate > endTimeDate)
                    {
                        endTimeDate = endTimeDate.AddDays(1);
                    }

                     flag = currentdate >= startTimeDate && currentdate < endTimeDate;
                    if (flag)  //如果当前时间属于该时段
                    {
                        ShiftTimeID = item.ShiftTimeID;
                        #region 判断当前时间是属于那个时段

                        var tempStartTime = startTimeDate;
                        var loopCount = 0;
                        while (tempStartTime < endTimeDate)
                        {
                            loopCount++;
                            if (tempStartTime < currentdate && tempStartTime.AddHours(2) > currentdate)
                            {
                                TimeInterval = tempStartTime.ToString("HH:mm") + "-" + tempStartTime.AddHours(2).ToString("HH:mm");
                                TimeIntervalStart = tempStartTime;
                                TimeIntervalEnd = tempStartTime.AddHours(2);
                                break;
                            }
                            tempStartTime = tempStartTime.AddHours(2);

                            //防止死循环
                            if (loopCount > 12)
                            {
                                break;
                            }
                        }
                        #endregion
                        break;
                    }
                    else
                    {
                        currentDay = excuteTime.AddMinutes(-5).ToString("yyyy-MM-dd");
                        endTimeDate = Convert.ToDateTime(currentDay + " " + item.End_Time);
                    }

                }
                #endregion

                if (!flag) continue;
                //判断是否需要同步上一班次的数据

                if (currentDay == LatestcurrentDay && ShiftTimeID == LatestShiftTimeID)
                {
                    whetherUpdateLasteShfit = false;
                }
                //设定全天班的开始于结束时间
                var StartTimeStr = OEE_OutputRepository.getStartTime(customerModel.Organization_UID);
                ALLstartTimeDate = Convert.ToDateTime(currentDay + " " + StartTimeStr);
                ALLendTimeDate = ALLstartTimeDate.AddDays(1);

                LatestALLstartTimeDate = Convert.ToDateTime(LatestcurrentDay + " " + StartTimeStr);
                LatestALLendTimeDate = LatestALLstartTimeDate.AddDays(1);

                //根据专案ID获取该专案下的所有OEE机台
                var allMachines = OEE_OutputRepository.getAllMachineByProject(customerModel.Project_UID);


                /******************************同步不良数据*****************************/
            MissingDefectCodeUIDList = SyncDefect( customerModel,  TimeIntervalStart,  TimeIntervalEnd,  currentDay,  ShiftTimeID,  TimeInterval,  allMachines);

                /*********************************************/

                /******************************同步停机数据*****************************/
           MissingDownMachineUIDList = SyncDT( customerModel,  startTimeDate,  endTimeDate,  currentDay,  ShiftTimeID,  TimeInterval,  allMachines);

                /*********************************************/

                /******************************同步机台状态数据*****************************/
                // SyncRealStatus(customerModel, startTimeDate, endTimeDate, currentDay, ShiftTimeID, TimeInterval, allMachines);

                /*********************************************/
                //获取断网的机台清单
                //OfflineMachineUIDList = GetOffLineMachine(customerModel.Plant, customerModel.MESProject_Name,  allMachines);

                #region 同步Local DB数据
                try
                {
                    ////  获取计划时间
                    var PlanHourValue = getShiftRealTime(currentDay, excuteTime, startTimeDate, endTimeDate, ShiftTimeID, customerModel.Organization_UID);
                    var ALLPlanHourValue = getShiftRealTime(currentDay, excuteTime, ALLstartTimeDate, ALLendTimeDate, 0, customerModel.Organization_UID);
                    //var PlanHourValue = 12;
                    //var ALLPlanHourValue = 24;
                    //判断该专案该时段是否有数据
                    var EveryDayMachineList = OEE_OutputRepository.judgmentData(customerModel.Project_UID, currentDay, ShiftTimeID);
                    //判断该专案全天是否有数据
                    var ALLEveryDayMachineList = OEE_OutputRepository.ALLjudgmentData(customerModel.Project_UID, currentDay);
                    //获取机台产出数量
                    outputDic = OEE_OutputRepository.SetOEE_MachineDayOutput(customerModel.Plant, customerModel.MESProject_Name, startTimeDate, endTimeDate);
                    ALLoutputDic = OEE_OutputRepository.SetOEE_MachineDayOutput(customerModel.Plant, customerModel.MESProject_Name, ALLstartTimeDate, ALLendTimeDate);

                    PorCTS = OEE_OutputRepository.GetPorCT(customerModel.Plant, customerModel.MESProject_Name);

                    FixtureS = OEE_OutputRepository.GetFixtureNum(customerModel.Plant, customerModel.MESProject_Name, startTimeDate, endTimeDate);
                    ALLFixtureS = OEE_OutputRepository.GetFixtureNum(customerModel.Plant, customerModel.MESProject_Name, ALLstartTimeDate, ALLendTimeDate);
                    //成都厂某些制程不能获取治具数量，需要通过机器信息表获取国定的值
                    CTUFixtureS = OEE_OutputRepository.GetCTUFixtureNum(currentDay);

                    //获取机台实际CT
                    ActualCTS = OEE_OutputRepository.GetActualCT(customerModel.Plant, customerModel.MESProject_Name, startTimeDate, endTimeDate);

                    ALLActualCTS = OEE_OutputRepository.GetActualCT(customerModel.Plant, customerModel.MESProject_Name, ALLstartTimeDate, ALLendTimeDate);

                    //获取ＣＮＣ机台Ｏｕｔｐｕｔ

                    //获取ＣＮＣ机台信息
                    CNCInfoLists = OEE_OutputRepository.GetCNCActualCT(customerModel.Plant, startTimeDate, endTimeDate);
                    ALLCNCInfoLists = OEE_OutputRepository.GetCNCActualCT(customerModel.Plant, ALLstartTimeDate, ALLendTimeDate);

                    List<OEE_EveryDayMachine> addList = new List<OEE_EveryDayMachine>();
                    List<OEE_EveryDayMachine> updateList = new List<OEE_EveryDayMachine>();

                    //3 添加和更新数据
                    foreach (var Machine in allMachines)
                    {

                        int OutPutValue = 0;
                        double outValue = 0.0;
                        OEE_EveryDayMachine stationModel = new OEE_EveryDayMachine();
                        OEE_EveryDayMachine ALLstationModel = new OEE_EveryDayMachine();
                        #region 当班次
                        stationModel.Plant_Organization_UID = customerModel.Plant_UID;
                        stationModel.BG_Organization_UID = customerModel.Organization_UID;
                        stationModel.OEE_MachineInfo_UID = Machine.OEE_MachineInfo_UID;
                        stationModel.FunPlant_Organization_UID = Machine.FunPlant_Organization_UID;
                        if (FixtureS.TryGetValue(Machine.MachineNo, out OutPutValue))
                        {
                            stationModel.FixtureNum = OutPutValue;
                        }
                        else if ((customerModel.Plant == "CTU" || customerModel.Plant == "CTU_M") && CTUFixtureS.TryGetValue(Machine.MachineNo, out OutPutValue))
                        {
                            stationModel.FixtureNum = OutPutValue;
                        }
                        else
                            stationModel.FixtureNum = 0;
                        if (ActualCTS.TryGetValue(Machine.MachineNo, out outValue))
                        {
                            stationModel.ActualCT = outValue;
                        }
                        else
                        {
                            stationModel.ActualCT = 0.0;
                        }

                        var CurrentPorCt = PorCTS.Find(o => o.MachineName == Machine.MachineNo);
                        if (CurrentPorCt != null)
                        {
                            stationModel.PORCT = CurrentPorCt.PorCT;
                        }
                        else
                            stationModel.PORCT = 0.0;

                        stationModel.Product_Date = Convert.ToDateTime(currentDay);
                        stationModel.ShiftTimeID = ShiftTimeID;
                        if (outputDic.TryGetValue(Machine.MachineNo, out OutPutValue))
                        {
                            stationModel.OutPut = OutPutValue;
                        }
                        else
                            stationModel.OutPut = 0;
                        //无锡厂需要为ＣＮＣ机台单独处理

                        var CNCInfoItem = CNCInfoLists.Find(o => o.MachineNo == Machine.MachineNo);
                        if (CNCInfoItem != null)
                        {
                            stationModel.ActualCT = CNCInfoItem.ActualCT;
                            stationModel.OutPut = CNCInfoItem.OutPut;
                            stationModel.ResetTime = CNCInfoItem.ResetTime;
                        }

                        stationModel.TotalAvailableHour = 12;
                        stationModel.PlannedHour = PlanHourValue;
                        stationModel.UpdateTime = DateTime.Now;

                        //判断是否该机台该天是否已经写入数据库
                        var CurrentItem = EveryDayMachineList.Find(o => o.OEE_MachineInfo_UID == Machine.OEE_MachineInfo_UID);

                        if (CurrentItem != null)
                        {
                            stationModel.OEE_EveryDayMachine_UID = CurrentItem.OEE_EveryDayMachine_UID;
                            updateList.Add(stationModel);
                        }
                        else
                        {
                            addList.Add(stationModel);
                        }

                        #endregion

                        #region 全天班次
                        ALLstationModel.Plant_Organization_UID = customerModel.Plant_UID;
                        ALLstationModel.BG_Organization_UID = customerModel.Organization_UID;
                        ALLstationModel.OEE_MachineInfo_UID = Machine.OEE_MachineInfo_UID;
                        ALLstationModel.FunPlant_Organization_UID = Machine.FunPlant_Organization_UID;

                        if (ALLFixtureS.TryGetValue(Machine.MachineNo, out OutPutValue))
                        {
                            ALLstationModel.FixtureNum = OutPutValue;
                        }
                        else if ((customerModel.Plant == "CTU" || customerModel.Plant == "CTU_M") && CTUFixtureS.TryGetValue(Machine.MachineNo, out OutPutValue))
                        {
                            ALLstationModel.FixtureNum = OutPutValue;
                        }
                        else
                            ALLstationModel.FixtureNum = 0;

                        if (ALLActualCTS.TryGetValue(Machine.MachineNo, out outValue))
                        {
                            ALLstationModel.ActualCT = outValue;
                        }

                        else
                            ALLstationModel.ActualCT = 0.0;

                        if (CurrentPorCt != null)
                        {
                            ALLstationModel.PORCT = CurrentPorCt.PorCT;
                        }
                        else
                            ALLstationModel.PORCT = 0.0;

                        ALLstationModel.Product_Date = Convert.ToDateTime(currentDay);

                        if (ALLoutputDic.TryGetValue(Machine.MachineNo, out OutPutValue))
                        {
                            ALLstationModel.OutPut = OutPutValue;
                        }

                        else
                            ALLstationModel.OutPut = 0;
                        //无锡厂需要为ＣＮＣ机台单独处理

                        var ALLCNCInfoItem = ALLCNCInfoLists.Find(o => o.MachineNo == Machine.MachineNo);
                        if (ALLCNCInfoItem != null)
                        {
                            ALLstationModel.ActualCT = ALLCNCInfoItem.ActualCT;
                            ALLstationModel.OutPut = ALLCNCInfoItem.OutPut;
                            ALLstationModel.ResetTime = ALLCNCInfoItem.ResetTime;
                        }

                        ALLstationModel.TotalAvailableHour = 24;
                        ALLstationModel.PlannedHour = ALLPlanHourValue;
                        ALLstationModel.UpdateTime = DateTime.Now;

                        if (MissingDownMachineUIDList.Contains(Machine.OEE_MachineInfo_UID))
                        {
                            stationModel.Is_DownType = 1;
                            ALLstationModel.Is_DownType = 1;
                        }
                        if (MissingDefectCodeUIDList.Contains(Machine.OEE_MachineInfo_UID))
                        {
                            stationModel.AbnormalDFCode = true;
                            ALLstationModel.AbnormalDFCode = true;
                        }
                        if (OfflineMachineUIDList.Contains(Machine.OEE_MachineInfo_UID))
                        {
                            stationModel.Is_Offline = true;
                            ALLstationModel.Is_Offline = true;
                        }
                        else
                        {
                            stationModel.Is_Offline = false;
                            ALLstationModel.Is_Offline = false;
                        }
                        //判断是否该机台该天是否已经写入数据库
                        var ALLCurrentItem = ALLEveryDayMachineList.Find(o => o.OEE_MachineInfo_UID == Machine.OEE_MachineInfo_UID);

                        if (ALLCurrentItem != null)
                        {
                            ALLstationModel.OEE_EveryDayMachine_UID = ALLCurrentItem.OEE_EveryDayMachine_UID;
                            updateList.Add(ALLstationModel);
                        }
                        else
                        {
                            addList.Add(ALLstationModel);

                        }

                    }
                    OEE_OutputRepository.AddList(addList);
                    OEE_OutputRepository.UpDateOEEDailyInfo(updateList);
                    unitOfWork.Commit();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                    //后续记录到失败日志表
                }

                #endregion
            }



            #region  同步机台是否断网


            #endregion
        }
        public OEE_MachineInfo GetMachineInfoByMachineNo(string MachineNo)
        {
            return OEE_OutputRepository.GetMachineInfoByMachineNo(MachineNo);

        }
        /// <summary>
        /// /根据Shift ID获取当前的实时运行时间
        /// </summary>
        /// <param name="ShiftTimeID"></param>
        /// <returns></returns>
        public   int getShiftRealTime(string currentDay,DateTime currentTime, DateTime startTime, DateTime endTime, int  ShiftID,int OPUID)
        {
            currentTime = currentTime.AddMinutes(-5);
            List<GL_Rest> RestList = new List<GL_Rest>();
            if (ShiftID == 0) //表示全天班
            {
                RestList = OEE_DefectCodeDailyNumDTORepository.getGlRestListByOP(OPUID);
            }
            else
            {
                //先获取所有符合条件的休息时间段数据
                RestList = OEE_DefectCodeDailyNumDTORepository.getGlRestList(ShiftID);
            }
          
             int RestListCount = RestList.Count; //用于存放返回的休息时间有几段
            //先判断当前日期是不是还没到最小的休息时间  
            if (RestListCount == 0)  //如果没有休息时间直接返回当前时间到班次开始时间的时长
            {
                return int.Parse(Math.Ceiling((currentTime.Subtract(startTime)).TotalHours).ToString());
            }
            else
            {
                Double BreakTime = 0;
                //先判断第一笔数据的开始时间是否在当前时间以前
                //先获取第一笔的开始时间
                //遍历所有休息时间，找到当前时间处于哪个时间段
                int CurrentBreakTimeInterval;
                DateTime ItemStartTime = DateTime.Now;
                DateTime ItemEndTime = DateTime.Now;
     
              //  var FirstStartTime = Convert.ToDateTime(currentDay + " " + RestList[RestListCount - 1].StartTime.ToString());
              //  var LasteStartTime = Convert.ToDateTime(currentDay + " " + RestList[0].StartTime.ToString());
              //  var LasteEndTime = Convert.ToDateTime(currentDay + " " + RestList[0].EndTime.ToString());
              //  if (FirstStartTime .CompareTo(currentTime)>=0)  //表示当前时间还没有到第一个休息时间段
              //  {
              //      //直接返回，不需要减去休息时间
              //      return int.Parse(Math.Ceiling((currentTime.Subtract(startTime)).TotalHours ).ToString());
              //  }
              //  //先获取最后一笔的结束时间
           
              //  else if (LasteEndTime.CompareTo(currentTime)<= 0)   //判断最后一笔的休息时间的结束时间是否在当前时间之前
              //  {
                 
              //      //算出休息时间为所有休息时间段的累计。
              //      foreach (var item in RestList)
              //      {
              //           ItemStartTime = Convert.ToDateTime(currentDay + " " + item.StartTime.ToString());
              //           ItemEndTime = Convert.ToDateTime(currentDay + " " + item.EndTime.ToString());
              //          BreakTime +=ItemEndTime.Subtract(ItemStartTime).Hours;
              //      }
              //  }
              //else  //表明当前时间已经经过休息时间段，且没有完全经过所有时间段
              //  {
                    //判断当前时间在
                    for (int  i=0; i<RestListCount;i++)
                    {
                        ItemStartTime = Convert.ToDateTime(currentDay + " " + RestList[i].StartTime.ToString());
                        ItemEndTime = Convert.ToDateTime(currentDay + " " + RestList[i].EndTime.ToString());
                        if(ItemStartTime.CompareTo(currentTime)<0&&ItemEndTime.CompareTo(currentTime)>0) //表示在时段内
                        {
                            BreakTime += currentTime.Subtract(ItemStartTime).Hours;   //将当前时段在该休息时间段的休息时间的时间算出来
                        }
                        else if(ItemEndTime.CompareTo(currentTime) <= 0)  //表明当前时间已经在该休息时间段休息过了
                        {
                            BreakTime += ItemEndTime.Subtract(ItemStartTime).Hours;
                        }
                       
                    }

                //}

                return int.Parse(Math.Ceiling((currentTime.Subtract(startTime)).TotalHours-BreakTime).ToString());

            }
        }
        public GL_ShiftTimeDTO GetShiftTime(int ShiftTimeID)
        {
            var model = GL_WIPHourOutputRepository.GetShiftTime(ShiftTimeID);
            return model;

        }
        /// <summary>
        /// 同步获取当前为断网的机台 ， 五分钟内心跳无数据，那么该机台断网
        /// </summary>
        /// <returns></returns>
        public  List<int>  GetOffLineMachine(string Plant,string MESCustomerName, List<OEE_MachineInfo> allMachines)
        {
            List<int> OfflineMachineUIDList = new List<int>();
            //先获取当前机台为未断网的数据
            List<string> NoOfflineMachineList = OEE_OutputRepository. GetNoOffLineMachineList( Plant,  MESCustomerName, DateTime.Now.AddMinutes(-5), DateTime.Now);

            foreach (var Item in allMachines)
            {
                var currentItem = NoOfflineMachineList.Find(o => o == Item.MachineNo);
                if (currentItem == null)  //表示该机台断网了
                {
                    //找到该断网机台UID存放到list中返回
                    OfflineMachineUIDList.Add(Item.OEE_MachineInfo_UID);
                }
            }
                return OfflineMachineUIDList;
        }
        /// <summary>
        /// 同步机台状态值到数据库
        /// </summary>
        public void SyncRealStatus(SystemProjectPlantDTO customerModel, DateTime startTime, DateTime endTime, string currentDay, int ShiftTimeID, string TimeInterval, List<OEE_MachineInfo> allMachines )
        {
            #region 同步状态数据
            try                          
            {
                if (DateTime.Now < endTime) endTime = DateTime.Now;  /// 截止到执行时间为准
                var InsertLists = new List<OEE_MachineStatus>();
                List<OEE_MachineStatusD> StatusLists = OEE_OutputRepository.getOEEStatusDatas(customerModel.Plant, startTime, endTime);
                var CurrentDate = Convert.ToDateTime(currentDay);
                foreach (var item in StatusLists)
                {
                    OEE_MachineStatus InsertItem = new OEE_MachineStatus();
                    var currentItem = allMachines.Find(o => o.MachineNo == item.MachineName);
                    if (currentItem == null)
                        continue;
                    //得到当前机台所在的工站
                    var CurrentStationUID = currentItem.GL_Station.StationID;
                    InsertItem.OEE_MachineInfo_UID = currentItem.OEE_MachineInfo_UID;
                    //InsertItem.Plant_Organization_UID = currentItem.Plant_Organization_UID;
                    InsertItem.ShiftTimeID = ShiftTimeID;
                    InsertItem.Product_Date = CurrentDate;
                    InsertItem.StartTime = item.StartTime;
                    InsertItem.StatusID = item.StatusID;
                    InsertItem.StatusDuration = item.StatusDuration;
                    InsertItem.UpdateTime = DateTime.Now;
                    InsertItem.EndTime = item.EndTIme;
                    InsertLists.Add(InsertItem);
                }
                //先删掉当前班次的数据然后新增
                var DeleteItemList = Oee_MachineStatusRepository.GetMany(o => o.Product_Date == CurrentDate && o.ShiftTimeID == ShiftTimeID);
                Oee_MachineStatusRepository.DeleteList(DeleteItemList.ToList());
                Oee_MachineStatusRepository.AddList(InsertLists);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                //后续记录到失败日志表
            }
            #endregion

        }

        /// <summary>
        /// 同步停机数据
        /// </summary>
        /// <param name="customerModel"></param>
        /// <param name="startTimeDate"></param>
        /// <param name="endTimeDate"></param>
        /// <param name="currentDay"></param>
        /// <param name="ShiftTimeID"></param>
        /// <param name="TimeInterval"></param>
        /// <param name="allMachines"></param>
        /// <returns></returns>
        public List<int> SyncDT(SystemProjectPlantDTO customerModel, DateTime startTimeDate, DateTime endTimeDate, string currentDay, int ShiftTimeID, string TimeInterval, List<OEE_MachineInfo> allMachines)
        {
            if (DateTime.Now < endTimeDate) endTimeDate = DateTime.Now;  /// 截止到执行时间为准
            List<int> MissingDownMachineUIDList = new List<int>();
            #region 同步停机数据
            try
            {
                //先获取Local DB的停机数据。
                //获取停机id
                List<OEE_DownTimeCode> DownCodeLists = OEE_OutputRepository.getAllDownCodeByProject(customerModel.Project_UID);
                //先判断当天当班次是否已经有数据，有数据为Update
                List<OEE_MachineDailyDownRecord> DownRocordLists = OEE_OutputRepository.GetOEEDownRocord(customerModel.Project_UID, currentDay, ShiftTimeID);

                //d待插入的数据集
                var InsertLists = new List<OEE_MachineDailyDownRecord>();
                var UPdatesLists = new List<OEE_MachineDailyDownRecord>();
                List<OEE_DownRecordsDTO> DownLists = OEE_OutputRepository.getOEELocalDatas(customerModel.Plant, startTimeDate, endTimeDate);
                OEE_DownTimeCode CurrentCode;
                List<OEE_EveryDayDFcodeMissing> MissingDFList = new List<OEE_EveryDayDFcodeMissing>();

                var DBMissingDFList = OEE_EveryDayDFcodeMissingRepository.getDFMissingList(currentDay, ShiftTimeID);
                foreach (var item in DownLists)
                {
                    OEE_MachineDailyDownRecord InsertItem = new OEE_MachineDailyDownRecord();
                    var currentItem = allMachines.Find(o => o.MachineNo == item.MachineName);
                    if (currentItem == null)
                        continue;
                    //得到当前机台所在的工站
                    var CurrentStationUID = currentItem.GL_Station.StationID;
                    InsertItem.BG_Organization_UID = currentItem.BG_Organization_UID;
                    InsertItem.DownDate = Convert.ToDateTime(currentDay);
                    InsertItem.DownTime = item.count;
                    InsertItem.EndTIme = Convert.ToDateTime(item.EndTIme);
                    InsertItem.FunPlant_Organization_UID = currentItem.FunPlant_Organization_UID;
                    ///找到当前的代码
                    CurrentCode = DownCodeLists.Find(o => (o.StationID == null && o.Error_Code == item.ErrorCode) || (o.StationID != null && o.StationID == currentItem.GL_Station.StationID && o.Error_Code == item.ErrorCode));
                    //若机台提供的停机代码不在用户配置的数据中，系统需记录，用户还未明确记录格式，当前未完成
                    if (CurrentCode != null)
                    {
                        InsertItem.OEE_DownTimeCode_UID = CurrentCode.OEE_DownTimeCode_UID;
                    }
                    else
                    {
                        #region 插入停机名称漏失表
                        //先判断是否已经插入数据库,没有插入数据库的才需要插入数据库
                        if (DBMissingDFList.Find(o => o.DownTimeCode == item.ErrorCode) != null)
                        {
                            continue;
                        }
                        OEE_EveryDayDFcodeMissing MissingItem = new OEE_EveryDayDFcodeMissing();
                        MissingItem.BG_Organization_UID = currentItem.BG_Organization_UID;
                        MissingItem.Plant_Organization_UID = currentItem.Plant_Organization_UID;
                        MissingItem.FunPlant_Organization_UID = currentItem.FunPlant_Organization_UID;
                        MissingItem.OEE_MachineInfo_UID = currentItem.OEE_MachineInfo_UID;
                        MissingItem.DownTimeCode = item.ErrorCode;
                        MissingItem.Product_Date = Convert.ToDateTime(currentDay);
                        MissingItem.ShiftTimeID = ShiftTimeID;
                        MissingItem.Create_Date = DateTime.Now;
                        MissingItem.Is_Enable = true;
                        MissingItem.Modified_Date = DateTime.Now;
                        MissingDFList.Add(MissingItem);
                        MissingDownMachineUIDList.Add(currentItem.OEE_MachineInfo_UID);
                        #endregion
                        continue;
                    }
                    InsertItem.OEE_MachineInfo_UID = currentItem.OEE_MachineInfo_UID;
                    InsertItem.Plant_Organization_UID = currentItem.Plant_Organization_UID;
                    InsertItem.ShiftTimeID = ShiftTimeID;
                    InsertItem.StartTime = Convert.ToDateTime(item.StartTime);
                    InsertItem.StationID = currentItem.StationID;
                    //判断是否该机台该天是否已经写入数据库
                  //  var CurrentDownItem = DownRocordLists.Find(o => o.OEE_MachineInfo_UID == currentItem.OEE_MachineInfo_UID && o.OEE_DownTimeCode_UID == CurrentCode.OEE_DownTimeCode_UID);

                    //if (CurrentDownItem != null)
                    //{
                    //    InsertItem.OEE_MachineDailyDownRecord_UID = CurrentDownItem.OEE_MachineDailyDownRecord_UID;
                    //    UPdatesLists.Add(InsertItem);
                    //}
                    //else
                    //{
                        InsertLists.Add(InsertItem);
                   // }
                    //OEE_MachineDailyDownRecordRepository.Add(InsertItem);  //单个添加的上下文是很慢的，如果使用Save Changes 更慢。需要批量
                }


                if (InsertLists.Count != 0)
                {
                    //先删掉当前数据，然后新增插入数据
                  
                        OEE_OutputRepository.DeleteDownRocords(customerModel.Project_UID, currentDay, ShiftTimeID);
                
                    OEE_MachineDailyDownRecordRepository.AddList(InsertLists);
                }
                    
                //if (UPdatesLists.Count != 0)
                //    OEE_MachineDailyDownRecordRepository.UpdateList(UPdatesLists);
                if (MissingDFList.Count != 0)
                    OEE_EveryDayDFcodeMissingRepository.AddList(MissingDFList);
                unitOfWork.Commit();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                //后续记录到失败日志表
            }
            #endregion
            return MissingDownMachineUIDList;
        }

        /// 同步不良数据
        public List<int> SyncDefect(SystemProjectPlantDTO customerModel, DateTime TimeIntervalStart, DateTime TimeIntervalEnd, string currentDay, int ShiftTimeID, string TimeInterval, List<OEE_MachineInfo> allMachines)
        {
            #region 同步不良数据
            List<int> MissingDefectCodeUIDList = new List<int>();
            try
            {
                if (customerModel.Plant == "CTU_M" || customerModel.Plant == "CTU")
                {
                    //获取停机id
                    List<OEE_StationDefectCode> DefectCodeLists = OEE_OutputRepository.getAllErrorCodeByProject(customerModel.Project_UID);
                    //先判断当天当班次是否已经有数据，有数据为Update
                    List<OEE_DefectCodeDailyNum> DefectRocordLists = OEE_OutputRepository.GetOEEDefectRord(customerModel.Project_UID, currentDay, ShiftTimeID, TimeInterval);
                    //先判断当天当班次是否已经有数据，有数据为Update
                    List<OEE_DefectCodeDailySum> DefectSumLists = OEE_OutputRepository.GetOEEDefectSum(customerModel.Project_UID, currentDay, ShiftTimeID);
                    //d待插入的数据集
                    var InsertLists = new List<OEE_DefectCodeDailyNum>();
                    var UPdatesLists = new List<OEE_DefectCodeDailyNum>();

                    //不良当前时段数据
                    List<LoacalMachineDefect> MachineDefectList = OEE_OutputRepository.GetDefectInterval(TimeIntervalStart, TimeIntervalEnd);
                    //将当前时段的数据插入或者更新到数据库
                    OEE_StationDefectCode CurrentCode;

                    List<OEE_EveryDayMachineDTCode> MissingDTList = new List<OEE_EveryDayMachineDTCode>();
                    foreach (var item in MachineDefectList)
                    {
                        OEE_DefectCodeDailyNum InsertItem = new OEE_DefectCodeDailyNum();
                        var currentItem = allMachines.Find(o => o.MachineNo == item.MachineNo);
                        if (currentItem == null)
                            continue;
                        //得到当前机台所在的工站
                        var CurrentStationUID = currentItem.GL_Station.StationID;
                        InsertItem.BG_Organization_UID = currentItem.BG_Organization_UID;
                        InsertItem.Plant_Organization_UID = currentItem.Plant_Organization_UID;
                        InsertItem.FunPlant_Organization_UID = currentItem.FunPlant_Organization_UID;
                        InsertItem.OEE_MachineInfo_UID = currentItem.OEE_MachineInfo_UID;
                        ///找到当前的代码
                        CurrentCode = DefectCodeLists.Find(o => o.StationID == currentItem.GL_Station.StationID && o.Defect_Code == item.DefectCode);
                        //若机台提供的不良代码不在用户配置的数据中，系统需记录，
                        if (CurrentCode != null)
                        {
                            InsertItem.OEE_StationDefectCode_UID = CurrentCode.OEE_StationDefectCode_UID;
                        }
                        else
                        {
                            OEE_EveryDayMachineDTCode MissingItem = new OEE_EveryDayMachineDTCode();

                            MissingItem.OEE_MachineInfo_UID = currentItem.OEE_MachineInfo_UID;
                            MissingItem.DFCode = item.DefectCode;
                            MissingItem.Product_Date = Convert.ToDateTime(currentDay);
                            MissingItem.ShiftTimeID = ShiftTimeID;
                            MissingItem.Create_Date = DateTime.Now;
                            MissingItem.Is_Enable = true;
                            MissingItem.Modified_Date = DateTime.Now;
                            MissingItem.Modified_UID = 90004;  //SA表设计问题导致这里弄的默认值 
                            MissingDTList.Add(MissingItem);

                            MissingDefectCodeUIDList.Add(currentItem.OEE_MachineInfo_UID);

                            continue;
                        }
                        InsertItem.ProductDate = Convert.ToDateTime(currentDay);
                        InsertItem.ShiftTimeID = ShiftTimeID;
                        InsertItem.TimeInterval = TimeInterval;
                        InsertItem.DefectNum = item.DownTime;
                        InsertItem.Modify_Date = DateTime.Now;
                        //判断是否该机台该天是否已经写入数据库
                        var DefectDownItem = DefectRocordLists.Find(o => o.OEE_MachineInfo_UID == currentItem.OEE_MachineInfo_UID && o.OEE_StationDefectCode_UID == CurrentCode.OEE_StationDefectCode_UID);
                        if (DefectDownItem != null)
                        {
                            InsertItem.OEE_DefectCodeDailyNum_UID = DefectDownItem.OEE_DefectCodeDailyNum_UID;
                            UPdatesLists.Add(InsertItem);
                        }
                        else
                        {
                            InsertLists.Add(InsertItem);
                        }
                        //OEE_MachineDailyDownRecordRepository.Add(InsertItem);  //单个添加的上下文是很慢的，如果使用Save Changes 更慢。需要批量
                    }
                    OEE_DefectCodeDailyNumDTORepository.AddList(InsertLists);
                    OEE_DefectCodeDailyNumDTORepository.UpdateList(UPdatesLists);
                    OEE_EveryDayMachineDTCodeRepository.AddList(MissingDTList);
                    unitOfWork.Commit();
                    ////同步不良汇总数据
                    OEE_OutputRepository.setDefectSums(currentDay, ShiftTimeID, customerModel.Organization_UID);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                //后续记录到失败日志表
            }
            return MissingDefectCodeUIDList;
            #endregion
        }
    }
}
