using Newtonsoft.Json;
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
    public interface IGL_WIPHourOutputService
    {
        void ExcuteGL_WIPHourOutput(DateTime excuteTime);
        PagedListModel<TimeIntervalModel> QueryWIPHourOutputList(GL_WIPHourOutputDTO model, Page page);
        GL_ShiftTimeDTO GetShiftTime(int ShiftTimeID);
        void ExcuteGL_MesHourOutPut(Dictionary<string, int> MesData, MESTimeInfo Mesnfo, string MesProject);
        List<WipEventModel> ExportWIPHourOutput(GL_WIPHourOutputDTO search);
        SystemProjectDTO GetMESProjectInfo(string MesProject);
        MESTimeInfo GetMesTime(DateTime excuteTime, string MesProject);
    }

    public class GL_WIPHourOutputService : IGL_WIPHourOutputService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGL_WIPHourOutputRepository GL_WIPHourOutputRepository;
        private readonly IGoldenLineRepository GoldenLineRepository;
        private readonly IGL_LineShiftPerfRepository GL_LineShiftPerfRepository;
        private readonly IGL_BuildPlanRepository GL_BuildPlanRepository;
        private readonly IGL_ShiftTimeRepository GL_ShiftTimeRepository;
        public GL_WIPHourOutputService(
              IGL_WIPHourOutputRepository GL_WIPHourOutputRepository,
              IGoldenLineRepository GoldenLineRepository,
              IGL_LineShiftPerfRepository GL_LineShiftPerfRepository,
              IGL_BuildPlanRepository GL_BuildPlanRepository,
              IGL_ShiftTimeRepository GL_ShiftTimeRepository,
        IUnitOfWork unitOfWork)
        {
            this.GL_WIPHourOutputRepository = GL_WIPHourOutputRepository;
            this.GoldenLineRepository = GoldenLineRepository;
            this.GL_LineShiftPerfRepository = GL_LineShiftPerfRepository;
            this.GL_BuildPlanRepository = GL_BuildPlanRepository;
            this.GL_ShiftTimeRepository = GL_ShiftTimeRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void ExcuteGL_WIPHourOutput(DateTime excuteTime)
        {
            try
            {
                #region 

                //1 获取所有的customter
                var customerList = GL_WIPHourOutputRepository.GetAllEnGL_Customer();
                foreach (var customerModel in customerList)
                {
                    //根据OP获取设置的班次
                    var shiftModelList = GL_WIPHourOutputRepository.GetShiftTimeList(customerModel.Organization_UID);

                    //获取当前属于哪个生产日期  如果当前“时间”不加日期的时间小于

                    var currentDayTemp1 = excuteTime.AddMinutes(-5).ToShortTimeString();
                    var currentDay = excuteTime.AddMinutes(-5).ToString("yyyy-MM-dd");
                    var currentTime = excuteTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm");
                    GL_ShiftTimeDTO ShiftTimeModel = new GL_ShiftTimeDTO();

                    //取出白天还是夜晚 同步12区间小时
                    var ShiftTimeID = 0;
                    var actHourIndex = 0;
                    var shiftdate = string.Empty;
                    Dictionary<string, int> outputDic = new Dictionary<string, int>();
                    #region 2 判断当前时间是那个班次
                    var currentdate = Convert.ToDateTime(currentTime);
                    foreach (var item in shiftModelList)
                    {
                        if (DateTime.Compare(DateTime.Parse(currentDayTemp1), DateTime.Parse(item.StartTime)) < 0 && DateTime.Compare(DateTime.Parse(item.StartTime), DateTime.Parse(item.End_Time)) < 0)
                        {
                            currentDay = excuteTime.AddDays(-1).AddMinutes(-5).ToString("yyyy-MM-dd");
                        }

                        var startTimeDate = Convert.ToDateTime(currentDay + " " + item.StartTime);
                        var endTimeDate = Convert.ToDateTime(currentDay + " " + item.End_Time);

                        if (startTimeDate < endTimeDate)
                        {
                            endTimeDate = endTimeDate.AddDays(1);
                        }

                        int hourIndex = Math.Abs(endTimeDate.Hour - startTimeDate.Hour);
                        //actHourIndex = hourIndex;
                        for (int k = 0; k < hourIndex; k++)
                        {
                            //如果是相等刚好是跨天的哪一个小时
                            var flag = currentdate >= startTimeDate.AddHours(k) && currentdate < startTimeDate.AddHours(k + 1);
                            if (flag)
                            {
                                shiftdate = startTimeDate.ToString("yyyy-MM-dd");
                                actHourIndex = k + 1;
                                ShiftTimeID = item.ShiftTimeID;
                                //调用接口
                                outputDic = GoldenLineRepository.GetOutput(customerModel.MESProject_Name, string.Empty, startTimeDate.AddHours(k), startTimeDate.AddHours(k + 1), item.Plant_Organization_UID);
                                break;
                            }
                        }
                    }
                    #endregion

                    List<GL_WIPHourOutputDTO> addList = new List<GL_WIPHourOutputDTO>();
                    List<GL_WIPHourOutputDTO> updateList = new List<GL_WIPHourOutputDTO>();

                    //3 添加和更新数据
                    foreach (var itemKeyValue in outputDic)
                    {
                        GL_StationDTO stationModel = new GL_StationDTO();
                        stationModel.StationName = itemKeyValue.Key;
                        stationModel.ProjectName = customerModel.Project_Name;
                        stationModel.BG_Organization_UID = customerModel.Organization_UID;
                        stationModel.CustomerID = customerModel.Project_UID;
                        //判断配置的工站表里面有个是否存在该工站
                        var stationResult = GL_WIPHourOutputRepository.GetStation(stationModel);
                        if (stationResult == null)
                        {
                            continue;
                        }

                        GL_WIPHourOutputDTO hourOutputModel = new GL_WIPHourOutputDTO();
                        hourOutputModel.CustomerID = customerModel.Project_UID;
                        hourOutputModel.LineID = stationResult.LineID;
                        hourOutputModel.StationID = stationResult.StationID;
                        hourOutputModel.AssemblyID = 0;
                        hourOutputModel.OutputDate = currentdate;
                        hourOutputModel.ShiftTimeID = ShiftTimeID;
                        hourOutputModel.HourIndex = actHourIndex;
                        hourOutputModel.ActualOutput = itemKeyValue.Value;
                        hourOutputModel.StandOutput = 0;
                        hourOutputModel.ShiftDate = shiftdate;
                        var model = GL_WIPHourOutputRepository.GetGL_WIPHourOutputBy(hourOutputModel);
                        if (model == null)
                        {
                            addList.Add(hourOutputModel);
                        }
                        else
                        {
                            hourOutputModel.WHOID = model.WHOID;
                            updateList.Add(hourOutputModel);
                        }
                    }
                    GL_WIPHourOutputRepository.AddGL_WIPHourOutput(addList);
                    GL_WIPHourOutputRepository.UpDateGL_WIPHourOutput(updateList);
                }
                #endregion
            }
            catch (Exception ex)
            {
                //后续记录到失败日志表
            }
        }
        public void NewExcuteGL_WIPHourOutput(DateTime excuteTime)
        {
            try
            {
                #region 

                //1 获取所有的customter
                var customerList = GL_WIPHourOutputRepository.GetAllEnGL_Customer();
                foreach (var customerModel in customerList)
                {
                    //根据OP获取设置的班次
                    var shiftModelList = GL_WIPHourOutputRepository.GetShiftTimeList(customerModel.Organization_UID);

                    //获取当前属于哪个生产日期  如果当前“时间”不加日期的时间小于

                    var currentDayTemp1 = excuteTime.AddMinutes(-5).ToShortTimeString();
                    var currentDay = excuteTime.AddMinutes(-5).ToString("yyyy-MM-dd");
                    var currentTime = excuteTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm");
                    GL_ShiftTimeDTO ShiftTimeModel = new GL_ShiftTimeDTO();

                    //取出白天还是夜晚 同步12区间小时
                    var ShiftTimeID = 0;
                    var actHourIndex = 0;
                    var shiftdate = string.Empty;
                    Dictionary<string, GL_HoureOutputModel> outputDic = new Dictionary<string, GL_HoureOutputModel>();

                    Dictionary<string, int> outputMESDic = new Dictionary<string, int>(); //存储从MES同步获取的数据
                    #region 2 判断当前时间是那个班次
                    var currentdate = Convert.ToDateTime(currentTime);
                    foreach (var item in shiftModelList)
                    {
                        if (DateTime.Compare(DateTime.Parse(currentDayTemp1), DateTime.Parse(item.StartTime)) < 0 && DateTime.Compare(DateTime.Parse(item.StartTime), DateTime.Parse(item.End_Time)) < 0)
                        {
                            currentDay = excuteTime.AddDays(-1).AddMinutes(-5).ToString("yyyy-MM-dd");
                        }

                        var startTimeDate = Convert.ToDateTime(currentDay + " " + item.StartTime);
                        var endTimeDate = Convert.ToDateTime(currentDay + " " + item.End_Time);

                        if (startTimeDate < endTimeDate)
                        {
                            endTimeDate = endTimeDate.AddDays(1);
                        }

                        int hourIndex = Math.Abs(endTimeDate.Hour - startTimeDate.Hour);
                        //actHourIndex = hourIndex;
                        for (int k = 0; k < hourIndex; k++)
                        {
                            //如果是相等刚好是跨天的哪一个小时
                            var flag = currentdate >= startTimeDate.AddHours(k) && currentdate < startTimeDate.AddHours(k + 1);
                            if (flag)
                            {
                                shiftdate = startTimeDate.ToString("yyyy-MM-dd");
                                actHourIndex = k + 1;
                                ShiftTimeID = item.ShiftTimeID;
                                //调用接口
                                outputDic = GoldenLineRepository.GetNewOutput(customerModel.MESProject_Name, string.Empty, startTimeDate.AddHours(k), startTimeDate.AddHours(k + 1), item.Plant_Organization_UID);
                                break;
                            }
                        }
                    }
                    #endregion

                    List<GL_WIPHourOutputDTO> addList = new List<GL_WIPHourOutputDTO>();
                    List<GL_WIPHourOutputDTO> updateList = new List<GL_WIPHourOutputDTO>();


                    //根据专案ID获取该专案下的所有站点
                    var allStationDTOs = GL_WIPHourOutputRepository.GetAllStationDTO(customerModel.Project_UID);

                    //获取已经获取到的数据的站点
                    List<GL_StationDTO> inGL_StationDTOs = new List<GL_StationDTO>();
                    //获取没有获取到的数据的站点
                    List<GL_StationDTO> noGL_StationDTOs = new List<GL_StationDTO>();

                    //var s = outputMESDic.Concat(outputDic);  //合并两个数据来源的Output数据
                    //3 添加和更新数据
                    foreach (var itemKeyValue in outputDic)
                    {
                        //判断配置的工站表里面有个是否存在该工站
                        var stationResult = allStationDTOs.FirstOrDefault(o => o.BG_Organization_UID == customerModel.Organization_UID && o.CustomerID == customerModel.Project_UID && o.MESStationName == itemKeyValue.Value.StationName && o.MESLineName == itemKeyValue.Value.LineName);
                        if (stationResult == null)
                        {
                            continue;
                        }

                        GL_WIPHourOutputDTO hourOutputModel = new GL_WIPHourOutputDTO();
                        hourOutputModel.CustomerID = customerModel.Project_UID;
                        hourOutputModel.LineID = stationResult.LineID;
                        hourOutputModel.StationID = stationResult.StationID;
                        hourOutputModel.AssemblyID = 0;
                        hourOutputModel.OutputDate = currentdate;
                        hourOutputModel.ShiftTimeID = ShiftTimeID;
                        hourOutputModel.HourIndex = actHourIndex;
                        hourOutputModel.ActualOutput = itemKeyValue.Value.Count;
                        hourOutputModel.StandOutput = 0;
                        hourOutputModel.ShiftDate = shiftdate;
                        var model = GL_WIPHourOutputRepository.GetGL_WIPHourOutputBy(hourOutputModel);
                        if (model == null)
                        {
                            addList.Add(hourOutputModel);
                        }
                        else
                        {
                            hourOutputModel.WHOID = model.WHOID;
                            updateList.Add(hourOutputModel);
                        }
                        inGL_StationDTOs.Add(stationResult);
                    }
                    //得到没有产生数据的站点
                    foreach (var item in allStationDTOs)
                    {
                        var isStationDTO = inGL_StationDTOs.FirstOrDefault(o => o.StationID == item.StationID);
                        if (isStationDTO == null)
                        {
                            noGL_StationDTOs.Add(item);
                        }
                    }
                    //根据没有获取到的数据，得到的ActualOutput 就为0.
                    foreach (var item in noGL_StationDTOs)
                    {
                        GL_WIPHourOutputDTO hourOutputModel = new GL_WIPHourOutputDTO();
                        hourOutputModel.CustomerID = customerModel.Project_UID;
                        hourOutputModel.LineID = item.LineID;
                        hourOutputModel.StationID = item.StationID;
                        hourOutputModel.AssemblyID = 0;
                        hourOutputModel.OutputDate = currentdate;
                        hourOutputModel.ShiftTimeID = ShiftTimeID;
                        hourOutputModel.HourIndex = actHourIndex;
                        hourOutputModel.ActualOutput = 0;
                        hourOutputModel.StandOutput = 0;
                        hourOutputModel.ShiftDate = shiftdate;
                        var model = GL_WIPHourOutputRepository.GetGL_WIPHourOutputBy(hourOutputModel);
                        if (model == null)
                        {
                            addList.Add(hourOutputModel);
                        }
                        else
                        {
                            hourOutputModel.WHOID = model.WHOID;
                            updateList.Add(hourOutputModel);
                        }

                    }

                    GL_WIPHourOutputRepository.AddGL_WIPHourOutput(addList);
                    GL_WIPHourOutputRepository.UpDateGL_WIPHourOutput(updateList);
                }
                #endregion
            }
            catch (Exception ex)
            {
                //后续记录到失败日志表
            }
        }

        public void ExcuteGL_MesHourOutPut(Dictionary<string, int> MesData, MESTimeInfo Mesnfo, string MesProject)
        {
            try
            {
                #region 

                //1 获取所有的customter
                var customerModel = GL_WIPHourOutputRepository.GetEnGL_Customer(MesProject);

                //根据OP获取设置的班次
                var shiftModelList = GL_WIPHourOutputRepository.GetShiftTimeList(customerModel.Organization_UID);



                List<GL_WIPHourOutputDTO> addList = new List<GL_WIPHourOutputDTO>();
                List<GL_WIPHourOutputDTO> updateList = new List<GL_WIPHourOutputDTO>();


                //根据专案ID获取该专案下的所有(MES站点)
                var allStationDTOs = GL_WIPHourOutputRepository.GetAllMesStationDTO(customerModel.Project_UID);

                //获取已经获取到的数据的站点
                List<GL_StationDTO> inGL_StationDTOs = new List<GL_StationDTO>();
                //获取没有获取到的数据的站点
                List<GL_StationDTO> noGL_StationDTOs = new List<GL_StationDTO>();
                //3 添加和更新数据
                foreach (var itemKeyValue in MesData)
                {

                    //判断配置的工站表里面有个是否存在该工站
                    var stationResult = allStationDTOs.FirstOrDefault(o => o.BG_Organization_UID == customerModel.Organization_UID && o.CustomerID == customerModel.Project_UID && o.MESStationName == itemKeyValue.Key);
                    if (stationResult == null)
                    {
                        continue;
                    }

                    GL_WIPHourOutputDTO hourOutputModel = new GL_WIPHourOutputDTO();
                    hourOutputModel.CustomerID = customerModel.Project_UID;
                    hourOutputModel.LineID = stationResult.LineID;
                    hourOutputModel.StationID = stationResult.StationID;
                    hourOutputModel.AssemblyID = 0;
                    hourOutputModel.OutputDate = Mesnfo.currentDayTime;
                    hourOutputModel.ShiftTimeID = Mesnfo.ShiftID;
                    hourOutputModel.HourIndex = Mesnfo.actHourIndex;
                    hourOutputModel.ActualOutput = itemKeyValue.Value;
                    hourOutputModel.StandOutput = 0;
                    hourOutputModel.ShiftDate = Mesnfo.shiftdate;
                    var model = GL_WIPHourOutputRepository.GetGL_WIPHourOutputBy(hourOutputModel);
                    if (model == null)
                    {
                        addList.Add(hourOutputModel);
                    }
                    else
                    {
                        hourOutputModel.WHOID = model.WHOID;
                        updateList.Add(hourOutputModel);
                    }
                    inGL_StationDTOs.Add(stationResult);
                }
                //得到没有产生数据的站点
                foreach (var item in allStationDTOs)
                {
                    var isStationDTO = inGL_StationDTOs.FirstOrDefault(o => o.StationID == item.StationID);
                    if (isStationDTO == null)
                    {
                        noGL_StationDTOs.Add(item);
                    }
                }
                //根据没有获取到的数据，得到的ActualOutput 就为0.
                foreach (var item in noGL_StationDTOs)
                {
                    GL_WIPHourOutputDTO hourOutputModel = new GL_WIPHourOutputDTO();
                    hourOutputModel.CustomerID = customerModel.Project_UID;
                    hourOutputModel.LineID = item.LineID;
                    hourOutputModel.StationID = item.StationID;
                    hourOutputModel.AssemblyID = 0;
                    hourOutputModel.OutputDate = Mesnfo.currentDayTime;
                    hourOutputModel.ShiftTimeID = Mesnfo.ShiftID;
                    hourOutputModel.HourIndex = Mesnfo.actHourIndex;
                    hourOutputModel.ActualOutput = 0;
                    hourOutputModel.StandOutput = 0;
                    hourOutputModel.ShiftDate = Mesnfo.shiftdate;
                    var model = GL_WIPHourOutputRepository.GetGL_WIPHourOutputBy(hourOutputModel);
                    if (model == null)
                    {
                        addList.Add(hourOutputModel);
                    }
                    else
                    {
                        hourOutputModel.WHOID = model.WHOID;
                        updateList.Add(hourOutputModel);
                    }

                }

                GL_WIPHourOutputRepository.AddGL_WIPHourOutput(addList);
                GL_WIPHourOutputRepository.UpDateGL_WIPHourOutput(updateList);
            }
            #endregion

            catch (Exception ex)
            {
                //后续记录到失败日志表
            }
        }


        public SystemProjectDTO GetMESProjectInfo(string MesProject)
        {
            return GL_WIPHourOutputRepository.GetEnGL_Customer(MesProject);
        }
        public MESTimeInfo GetMesTime(DateTime excuteTime, string MesProject)
        {
            MESTimeInfo ReturnMesInfo = new MESTimeInfo();
            try
            {
                #region 
                //设置推迟时间  推迟时间应大于间隔时间  如间隔30分钟  只一般delay时间大于30分钟。
                int DelayTime = -15;

                //1 获取所有的customter
                var customerModel = GL_WIPHourOutputRepository.GetEnGL_Customer(MesProject);

                //根据OP获取设置的班次
                var shiftModelList = GL_WIPHourOutputRepository.GetShiftTimeList(customerModel.Organization_UID);

                //获取当前属于哪个生产日期  如果当前“时间”不加日期的时间小于

                var currentDayTemp1 = excuteTime.AddMinutes(DelayTime).ToShortTimeString();
                var currentDay = excuteTime.AddMinutes(DelayTime).ToString("yyyy-MM-dd");
                var currentTime = excuteTime.AddMinutes(DelayTime).ToString("yyyy-MM-dd HH:mm");
                GL_ShiftTimeDTO ShiftTimeModel = new GL_ShiftTimeDTO();

                //取出白天还是夜晚 同步12区间小时
                var ShiftTimeID = 0;
                var actHourIndex = 0;
                var shiftdate = string.Empty;
                Dictionary<string, int> outputDic = new Dictionary<string, int>();

                Dictionary<string, int> outputMESDic = new Dictionary<string, int>(); //存储从MES同步获取的数据
                #region 2 判断当前时间是那个班次
                var currentdate = Convert.ToDateTime(currentTime);
                foreach (var item in shiftModelList)
                {
                    currentDay = excuteTime.AddMinutes(DelayTime).ToString("yyyy-MM-dd");
                    if (DateTime.Compare(DateTime.Parse(currentDayTemp1), DateTime.Parse(item.StartTime)) < 0 && DateTime.Compare(DateTime.Parse(item.StartTime), DateTime.Parse(item.End_Time)) > 0)
                    {
                        currentDay = excuteTime.AddDays(-1).AddMinutes(DelayTime).ToString("yyyy-MM-dd");
                    }

                    var startTimeDate = Convert.ToDateTime(currentDay + " " + item.StartTime);
                    var endTimeDate = Convert.ToDateTime(currentDay + " " + item.End_Time);

                    if (startTimeDate > endTimeDate)
                    {
                        endTimeDate = endTimeDate.AddDays(1);
                    }

                    var flagBanChi = currentdate >= startTimeDate && currentdate < endTimeDate;
                    if (flagBanChi)  //如果当前时间属于该时段
                    {
                        int hourIndex = Math.Abs(endTimeDate.Hour - startTimeDate.Hour);
                        //actHourIndex = hourIndex;
                        for (int k = 0; k < hourIndex; k++)
                        {
                            //如果是相等刚好是跨天的哪一个小时
                            var flag = currentdate >= startTimeDate.AddHours(k) && currentdate < startTimeDate.AddHours(k + 1);
                            if (flag)
                            {
                                shiftdate = startTimeDate.ToString("yyyy-MM-dd");
                                actHourIndex = k + 1;
                                ShiftTimeID = item.ShiftTimeID;
                                ReturnMesInfo.ShiftID = item.ShiftTimeID;
                                ReturnMesInfo.StartTime = startTimeDate.AddHours(k).ToString("yyyy/MM/dd HH:mm");
                                ReturnMesInfo.EndTime = startTimeDate.AddHours(k + 1).ToString("yyyy/MM/dd HH:mm");
                                ReturnMesInfo.shiftdate = shiftdate;
                                ReturnMesInfo.currentDayTime = DateTime.Parse(currentTime);
                                ReturnMesInfo.actHourIndex = k + 1;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion



            catch (Exception ex)
            {
                //后续记录到失败日志表   outputDic = GoldenLineRepository.GetOutput(customerModel.MESProject_Name, string.Empty, startTimeDate.AddHours(k), startTimeDate.AddHours(k + 1),item.Plant_Organization_UID);
            }
            #endregion
            return ReturnMesInfo;
        }
        public PagedListModel<TimeIntervalModel> QueryWIPHourOutputList(GL_WIPHourOutputDTO model, Page page)
        {
            try
            {
                var totalCount = 0;
                var result = GL_WIPHourOutputRepository.QueryWIPHourOutput(model);
              
                var SationList = result.GroupBy(p => p.StationID);
                var dic = new Dictionary<string, List<string>>();
                List<TimeIntervalModel> TimeIntervalList = new List<TimeIntervalModel>();
                var LineShiftPerfModel = GL_LineShiftPerfRepository.GetMany(p => p.CustomerID == model.CustomerID && p.LineID == model.LineID && p.OutputDate == model.myRetriveDate && p.ShiftTimeID == model.ShiftTimeID);
                var BuildPlanModel = GL_BuildPlanRepository.GetMany(p => p.CustomerID == model.CustomerID && p.LineID == model.LineID && p.OutputDate == model.myRetriveDate && p.ShiftTimeID == model.ShiftTimeID);
                //判断是白班还是夜班

                foreach (var item in SationList)
                {
                    List<int> OutPutNumList = new List<int>();
                    TimeIntervalModel ModelInterval = new TimeIntervalModel();
                    ModelInterval.Station = item.FirstOrDefault().stationName;
                    ModelInterval.TimeInterval1 = item.Where(p => p.HourIndex == 1).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 1).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval2 = item.Where(p => p.HourIndex == 2).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 2).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval3 = item.Where(p => p.HourIndex == 3).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 3).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval4 = item.Where(p => p.HourIndex == 4).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 4).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval5 = item.Where(p => p.HourIndex == 5).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 5).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval6 = item.Where(p => p.HourIndex == 6).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 6).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval7 = item.Where(p => p.HourIndex == 7).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 7).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval8 = item.Where(p => p.HourIndex == 8).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 8).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval9 = item.Where(p => p.HourIndex == 9).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 9).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval10 = item.Where(p => p.HourIndex == 10).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 10).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval11 = item.Where(p => p.HourIndex == 11).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 11).FirstOrDefault().ActualOutput;
                    ModelInterval.TimeInterval12 = item.Where(p => p.HourIndex == 12).FirstOrDefault() == null ? 0 : item.Where(p => p.HourIndex == 12).FirstOrDefault().ActualOutput;
                    ModelInterval.TatalOutPut =
                      ModelInterval.TimeInterval1 + ModelInterval.TimeInterval2
                    + ModelInterval.TimeInterval3 + ModelInterval.TimeInterval4
                    + ModelInterval.TimeInterval5 + ModelInterval.TimeInterval6
                    + ModelInterval.TimeInterval7 + ModelInterval.TimeInterval8
                    + ModelInterval.TimeInterval9 + ModelInterval.TimeInterval10
                    + ModelInterval.TimeInterval11 + ModelInterval.TimeInterval12;
                    OutPutNumList.Add(ModelInterval.TimeInterval1);
                    OutPutNumList.Add(ModelInterval.TimeInterval2);
                    OutPutNumList.Add(ModelInterval.TimeInterval3);
                    OutPutNumList.Add(ModelInterval.TimeInterval4);
                    OutPutNumList.Add(ModelInterval.TimeInterval5);
                    OutPutNumList.Add(ModelInterval.TimeInterval6);
                    OutPutNumList.Add(ModelInterval.TimeInterval7);
                    OutPutNumList.Add(ModelInterval.TimeInterval8);
                    OutPutNumList.Add(ModelInterval.TimeInterval9);
                    OutPutNumList.Add(ModelInterval.TimeInterval10);
                    OutPutNumList.Add(ModelInterval.TimeInterval11);
                    OutPutNumList.Add(ModelInterval.TimeInterval12);
                    OutPutNumList.RemoveAll(n => n == 0);
                    if (OutPutNumList != null && OutPutNumList.Count > 0)
                    {
                        ModelInterval.Max = OutPutNumList.Max();
                        ModelInterval.Min = OutPutNumList.Min();
                    }
                    else
                    {
                        ModelInterval.Max = 0;
                        ModelInterval.Min = 0;
                    }
                    ModelInterval.HourStandardOutPut = BuildPlanModel.FirstOrDefault() == null ? 0m :(BuildPlanModel.FirstOrDefault().PlanOutput/12);
                    TimeIntervalList.Add(ModelInterval);
                }

                return new PagedListModel<TimeIntervalModel>(totalCount, TimeIntervalList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public GL_ShiftTimeDTO GetShiftTime(int ShiftTimeID)
        {
            var model = GL_WIPHourOutputRepository.GetShiftTime(ShiftTimeID);
            return model;
            //DataTable dataTable = new DataTable();
            //dataTable.Columns.Add("station", typeof(string));
            //dataTable.Columns.Add("TimeInterval1", typeof(string));
            //dataTable.Columns.Add("TimeInterval2", typeof(string));
            //dataTable.Columns.Add("TimeInterval3", typeof(string));
            //dataTable.Columns.Add("TimeInterval4", typeof(string));
            //dataTable.Columns.Add("TimeInterval5", typeof(string));
            //dataTable.Columns.Add("TimeInterval6", typeof(string));
            //dataTable.Columns.Add("TimeInterval4", typeof(string));
            //dataTable.Columns.Add("TimeInterval8", typeof(string));
            //dataTable.Columns.Add("TimeInterval9", typeof(string));
            //dataTable.Columns.Add("TimeInterval10", typeof(string));
            //dataTable.Columns.Add("TimeInterval11", typeof(string));
            //dataTable.Columns.Add("TimeInterval12", typeof(string));
            //DataRow dataRow = dataTable.NewRow();
            //dataRow["station"] = "08:30-09:30";
            //dataRow["TimeInterval1"] = "08:30-09:30";
            //dataRow["TimeInterval2"] = "08:30-09:30";
            //dataRow["TimeInterval3"] = "08:30-09:30";
            //dataRow["TimeInterval4"] = "08:30-09:30";
            //dataRow["TimeInterval5"] = "08:30-09:30";
            //dataRow["TimeInterval6"] = "08:30-09:30";
            //dataRow["TimeInterval7"] = "08:30-09:30";
            //dataRow["TimeInterval8"] = "08:30-09:30";
            //dataRow["TimeInterval9"] = "08:30-09:30";
            //dataRow["TimeInterval10"] = "08:30-09:30";
            //dataRow["TimeInterval11"] = "08:30-09:30";
            //dataRow["TimeInterval12"] = "08:30-09:30";
            //dataTable.Rows.Add(dataRow);
            //return JsonConvert.SerializeObject(dataTable);
        }

        /// <summary>
        /// 导出这条线下面的所有的SN
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<WipEventModel> ExportWIPHourOutput(GL_WIPHourOutputDTO search)
        {
            //获取开始结束时间
            var ShiftModel = GL_WIPHourOutputRepository.GetShiftTime(search.ShiftTimeID);

            var startTimeDate = Convert.ToDateTime(search.myRetriveDate + " " + ShiftModel.StartTime);
            var endTimeDate = Convert.ToDateTime(search.myRetriveDate + " " + ShiftModel.End_Time);

            if (startTimeDate > endTimeDate)
            {
                endTimeDate = endTimeDate.AddDays(1);
            }

            //获取MESName
            var mesStationNameModel = GL_WIPHourOutputRepository.GetMESProjectNameByID(search.CustomerID);
            //获取LineName
            var LineNameModel = GL_WIPHourOutputRepository.GetMESLineNameByID(search.LineID);
            search.MESCustomerName = mesStationNameModel.MESCustomerName;
            search.MESLineName = LineNameModel.MESLineName;
            search.StartTime = startTimeDate;
            search.EndTime = endTimeDate;
            var result = GoldenLineRepository.ExportWIPHourOutput(search);
            var stationIdList = result.GroupBy(p => p.StationName);
            var stationDic = new Dictionary<string, string>();
            //转行对应的工站名称
            foreach (var item in stationIdList)
            {
                var sttionModel = GL_WIPHourOutputRepository.GetStationNameByID(item.Key);
                if (!stationDic.Keys.Contains(item.Key) && sttionModel != null)
                {
                    stationDic.Add(item.Key, sttionModel.stationName);
                }
            }

            //判断这个时间属于哪个时段
            //int hourindex = math.abs(endtimedate.hour - starttimedate.hour);
            foreach (var item in result)
            {
                item.CustomerName = mesStationNameModel.ProjectName;
                item.LineName = LineNameModel.LineName;
                var stationName = string.Empty;
                stationDic.TryGetValue(item.StationName, out stationName);
                item.StationName = stationName;
            }
            return result;
        }
    }
}
