using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IGL_LineShiftPerfService : IBaseSevice<GL_LineShiftPerf, GL_LineShiftPerfDTO, GL_LineShiftPerfModelSearch>
    {
        GL_LineShiftPerf Add(GL_LineShiftPerf lineShiftPerf);
        void Update(GL_LineShiftPerf lineShiftPerf);

        void SyncLineShiftPerf();

        void SyncLineShiftPerfLastShift();
    }

   public class GL_LineShiftPerfService : BaseSevice<GL_LineShiftPerf, GL_LineShiftPerfDTO, GL_LineShiftPerfModelSearch>, IGL_LineShiftPerfService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGL_LineShiftPerfRepository lineShiftPerfRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly IGL_ShiftTimeRepository shiftTimeRepository;
        private readonly IGL_LineRepository lineRepository;
        private readonly IGoldenLineRepository goldenLineRepository;
        private readonly IGL_BuildPlanRepository buildPlanRepository;
        public GL_LineShiftPerfService(
            IUnitOfWork unitOfWork
            , IGL_LineShiftPerfRepository lineShiftPerfRepository
            , ISystemProjectRepository systemProjectRepository
            , IGL_ShiftTimeRepository shiftTimeRepository
            , IGoldenLineRepository goldenLineRepository
            , IGL_LineRepository lineRepository
            , IGL_BuildPlanRepository buildPlanRepository) : base(lineShiftPerfRepository)
        {
            this.unitOfWork = unitOfWork;
            this.lineShiftPerfRepository = lineShiftPerfRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.shiftTimeRepository = shiftTimeRepository;
            this.goldenLineRepository = goldenLineRepository;
            this.lineRepository = lineRepository;
            this.buildPlanRepository = buildPlanRepository;
        }

        public GL_LineShiftPerf Add(GL_LineShiftPerf lineShiftPerf)
        {
            return lineShiftPerfRepository.Add(lineShiftPerf);
        }

        public void Update(GL_LineShiftPerf lineShiftPerf)
        {
            lineShiftPerfRepository.Update(lineShiftPerf);
        }

        /// <summary>
        /// 同步LineShiftPerf 数据
        /// </summary>
        public void SyncLineShiftPerf()
        {
            //var customerService = new System_ProjectService(new SystemProjectRepository(_DatabaseFactory));
            //var lineService = new GL_LineService(new GL_LineRepository(_DatabaseFactory));
            //var shiftTimeService = new GL_ShiftTimeService(new GL_ShiftTimeRepository(_DatabaseFactory));
            //var buildPlanService = new GL_BuildPlanService(new GL_BuildPlanRepository(_DatabaseFactory));
            //var goldenLineService = new GoldenLineService(new GoldenLineRepository(_DatabaseFactory),
            //    new GL_GoldenStationCTRecordRepository(_DatabaseFactory),
            //    new GL_ShiftTimeRepository(_DatabaseFactory), new SystemProjectRepository(_DatabaseFactory));
            //var stationService = new GL_StationService(new GL_StationRepository(_DatabaseFactory));
            //var WIPShiftOutputService = new GL_WIPShiftOutputService(new GL_WIPShiftOutputRepository(_DatabaseFactory));
            //var WIPShiftBatchOutputService = new GL_WIPShiftBatchOutputService(new GL_WIPShiftBatchOutputRepository(_DatabaseFactory));

            var customerList = systemProjectRepository.GetAll().ToList();
                //customerService.Query(new QueryModel<System_ProjectModelSearch>());
            customerList = customerList.Where(p => p.MESProject_Name != null && p.MESProject_Name != "").ToList();
            var now = DateTime.Now; //当前时间
            foreach (var customer in customerList)
            {

                var shiftTimeList = shiftTimeRepository.GetMany(x => x.BG_Organization_UID == customer.Organization_UID && x.IsEnabled == true);
                    //shiftTimeService.Query(new QueryModel<GL_ShiftTimeModelSearch>() { Equal = new GL_ShiftTimeModelSearch() { BG_Organization_UID = customer.Organization_UID, IsEnabled = true } });
                //shiftTimeList = shiftTimeList.Where(s => s.FunPlant_Organization_UID == customer.FunPlant_Organization_UID).ToList(); //班次不再需要功能厂过滤
                var currentShiftTime = GetCurrentShift(shiftTimeList.ToList(), now);
                if (currentShiftTime != null)
                {
                    var startTimePair = currentShiftTime.StartTime.Split(':');
                    var startHour = int.Parse(startTimePair[0]);
                    var startMinute = int.Parse(startTimePair[1]);

                    var endTimePair = currentShiftTime.End_Time.Split(':');
                    var endHour = int.Parse(endTimePair[0]);
                    var endMinute = int.Parse(endTimePair[1]);
                    //var startTime = currentShiftTime.StartTime;
                    var startDateTime = new DateTime(now.Year, now.Month, now.Day, startHour, startMinute, 0);
                    var endDateTime = new DateTime(now.Year, now.Month, now.Day, endHour, endMinute, 0);

                    if (startDateTime > endDateTime)
                    {
                        //班次从昨天延续到今天
                        if (now < endDateTime)
                        {
                            startDateTime = startDateTime.AddDays(-1);
                        }
                        else
                        {
                            //从今天延续到明天
                            endDateTime = endDateTime.AddDays(1);
                        }
                    }

                    var outputDate = startDateTime.ToString("yyyy-MM-dd");

                    //由于GoldenLine 和OEE 共用线体GL_Line，所以线体的工站GL_Station，栏位IsGoldenLine == true 才是GoldenLine 的工站，这条线也是GoldenLine, 否则这条线不是GoldenLine（OEE 的线不在此计算），不用计算
                    var lineList = lineRepository.GetMany(x => x.CustomerID == customer.Project_UID && x.IsEnabled == true && x.GL_Station.Any(s => s.IsGoldenLine == true));
                        //lineService.Query(new QueryModel<GL_LineModelSearch>() { Equal = new GL_LineModelSearch() { CustomerID = customer.Project_UID, IsEnabled = true } }).Where(l => l.GL_Station.Any(s => s.IsGoldenLine == true));
                    foreach (var line in lineList)
                    {
                        int linePlanOutput = 0;
                        int actualOutput = 0;
                        decimal UPH = 3600 / line.CycleTime;
                        //公式 Standard Output=UPH*Working hours=(3600/bottleneck cycle time)*Working hours.
                        int standOutput = Convert.ToInt32(Convert.ToDecimal((DateTime.Now - startDateTime).TotalSeconds) / line.CycleTime);//实际输出超过标准输出时，前端显示标红
                        int planDownTime = 0;
                        double upDownTime = 0;
                        var standCT = line.CycleTime;   //获取标准CT
                        double runTime = 0;

                        decimal actualOutputVSPlan = 0;
                        decimal actualOutputVSRealTimePlan = 0;
                        decimal actualOutputVSStdOutput = 0;
                        decimal lineUtil = 0;
                        int actualHC = 0;
                        decimal vaole = 0;
                        var lineStatus = goldenLineRepository.GetLineStatus(line.LineID);

                        //PlanOutput 从BuildPlan 获取, BuildPlan.OutputDate 为当天班次的开始时间
                        var buildPlanList = buildPlanRepository.GetMany(x=>x.LineID == line.LineID && x.ShiftTimeID == currentShiftTime.ShiftTimeID && x.OutputDate == outputDate).ToList();
                            //buildPlanService.Query(new QueryModel<GL_BuildPlanModelSearch>() { Equal = new GL_BuildPlanModelSearch() { LineID = line.LineID, ShiftTimeID = currentShiftTime.ShiftTimeID, OutputDate = startDateTime.ToString("yyyy-MM-dd") } });
                        if (buildPlanList.Count > 0)
                        {
                            var buildPlan = buildPlanList[0];
                            linePlanOutput = buildPlan.PlanOutput;
                        }
                        //获取output
                        actualOutput = goldenLineRepository.GetLineOutput(line.LineID, startDateTime, endDateTime);
                            //goldenLineService.GetLineOutput(line.LineID, startDateTime, endDateTime);

                        //获取StartTime,用来算PlanDownTime
                        var lineStartTime = goldenLineRepository.GetStartTime(customer.MESProject_Name, line.MESLineName, line.Plant_Organization_UID, startDateTime, endDateTime);
                        if (lineStartTime != null)
                        {
                            planDownTime = (int)(lineStartTime.Value - startDateTime).TotalSeconds;
                        }

                        //获取UPDownTime
                        upDownTime = goldenLineRepository.GetUnPlanDownTime(customer.MESProject_Name, line.MESLineName, line.Plant_Organization_UID, standCT, startDateTime, endDateTime);

                        //获取RunTime
                        runTime = goldenLineRepository.GetRunTime(customer.MESProject_Name, line.MESLineName, line.Plant_Organization_UID, standCT, startDateTime, endDateTime);

                        //ActualOutputVSPlan
                        if (linePlanOutput == 0)
                        {
                            actualOutputVSPlan = 0;
                        }
                        else
                        {
                            //如果actualOutput超过linePlanOutput, 取linePlanOutput
                            actualOutputVSPlan = Math.Round((decimal)(actualOutput > linePlanOutput ? linePlanOutput : actualOutput) / linePlanOutput, 3);
                            if (actualOutputVSPlan > 1)
                            {
                                actualOutputVSPlan = 1;
                            }
                        }

                        //ActualOutputVSRealTimePlan, 相当于挣值
                        var shiftTimeSpanSeconds = (endDateTime - startDateTime).TotalSeconds;
                        var realTimePlanOutput = linePlanOutput / shiftTimeSpanSeconds * (runTime + upDownTime + planDownTime); //截止当前时间的计划产能数量
                        if (linePlanOutput == 0 || shiftTimeSpanSeconds == 0 || realTimePlanOutput == 0)
                        {
                            actualOutputVSRealTimePlan = 0;
                        }
                        else
                        {
                            actualOutputVSRealTimePlan = Math.Round((decimal)actualOutput / (decimal)realTimePlanOutput, 3);
                            if (actualOutputVSRealTimePlan > 1)
                            {
                                actualOutputVSRealTimePlan = 1;
                            }
                        }

                        //ActualOutputVSStdOutput
                        if (standOutput == 0)
                        {
                            actualOutputVSStdOutput = 0;
                        }
                        else
                        {
                            actualOutputVSStdOutput = Math.Round((decimal)actualOutput / standOutput, 3);
                            if (actualOutputVSStdOutput > 1)
                            {
                                actualOutputVSStdOutput = 1;
                            }
                        }

                        //LineUtil
                        if (runTime + planDownTime + upDownTime == 0)
                        {
                            lineUtil = 0;
                        }
                        else
                        {
                            lineUtil = Math.Round((decimal)runTime / (decimal)(runTime + planDownTime + upDownTime), 3);
                            if (lineUtil > 1)
                            {
                                lineUtil = 0;
                            }
                        }

                        //VAOLE,相当于工人的工作效率，即工作时间有多少百分比转化成输出
                        //公式: 
                        //StdUPH = PlanOutput / 10   (原本一个班次12小时，除去2小时吃饭时间，即10小时工作时间)
                        //SMH = Plan HC / StdUPH
                        //VAOLE= ( SMH * Actual Output ) / ( Actual HC * Actual Available Time )
                        //输出工站获取Output
                        // var outputStation = stationService.Query(new QueryModel<GL_StationModelSearch>() { Equal = new GL_StationModelSearch() { LineID = line.LineID, IsOutput = true } }).FirstOrDefault();
                        var outputStation = goldenLineRepository.GetStationDTOIsOutput(line.LineID);
                        if (outputStation != null)
                        {
                            //获取产线的实际输出(即产线的输出工站的输出)
                            var output = goldenLineRepository.GetLineOutput(line.LineID, startDateTime, null);
                            //var totoalCT = stationService.Query(new QueryModel<GL_StationModelSearch>() { Equal = new GL_StationModelSearch() { LineID = line.LineID, IsOutput = true } }).Sum(s => s.CycleTime);
                            //var buildPlan = buildPlanService.Query(new QueryModel<GL_BuildPlanModelSearch>() { Equal = new GL_BuildPlanModelSearch() { CustomerID = customer.Project_UID, LineID = line.LineID } }).FirstOrDefault();
                            var buildPlans = buildPlanRepository.GetMany(x=>x.CustomerID == customer.Project_UID && x.LineID == line.LineID && x.ShiftTimeID == currentShiftTime.ShiftTimeID && x.OutputDate == outputDate).ToList();
                                //buildPlanService.Query(new QueryModel<GL_BuildPlanModelSearch>() { Equal = new GL_BuildPlanModelSearch() { CustomerID = customer.Project_UID, LineID = line.LineID, ShiftTimeID = currentShiftTime.ShiftTimeID, OutputDate = startDateTime.ToString("yyyy-MM-dd") } });
                            if (buildPlans != null && buildPlans.Count > 0)
                            {
                                var buildPlan = buildPlans.FirstOrDefault();
                                // if (buildPlan!= null)
                                //{
                                if (buildPlan.ActualHC != null)
                                {
                                    actualHC = buildPlan.ActualHC.Value;
                                }
                                double workingTime = 0; //单位：秒
                                if (now >= endDateTime)
                                {
                                    workingTime = (endDateTime - startDateTime).TotalSeconds;
                                }
                                else
                                {
                                    workingTime = (now - startDateTime).TotalSeconds;
                                }
                                //vaole = (decimal)(output * line.CycleTime) / (decimal)(actualHC * workingTime);
                                decimal StdUPH = (decimal)buildPlan.PlanOutput / 10;    //除以10小时
                                if (StdUPH > 0)
                                {
                                    if (buildPlan.PlanHC != null)
                                    {
                                        decimal SMH = buildPlan.PlanHC.Value / StdUPH;
                                        if (actualHC > 0 && workingTime > 0)
                                        {
                                            vaole = (SMH * output) / (decimal)(actualHC * (workingTime / 3600)) * (decimal)1.2;
                                        }
                                    }

                                }
                            }
                        }


                        var lineShiftPerf = new GL_LineShiftPerf()
                        {
                            CustomerID = customer.Project_UID,
                            LineID = line.LineID,
                            OutputDate = startDateTime.ToString("yyyy-MM-dd"),
                            ShiftTimeID = currentShiftTime.ShiftTimeID,
                            PlanOutput = linePlanOutput,
                            StandOutput = standOutput,
                            ActualOutput = actualOutput,
                            UPH = UPH,
                            LineStatus = (int)lineStatus,
                            PlanDownTime = planDownTime,
                            UPDownTime = (int)upDownTime,//转换问题
                            RunTime = (int)runTime,//转换问题
                            ActualOutputVSPlan = actualOutputVSPlan,
                            ActualOutputVSRealTimePlan = actualOutputVSRealTimePlan,
                            ActualOutputVSStdOutput = actualOutputVSStdOutput,
                            LineUtil = lineUtil,
                            CapacityLoading = 0,
                            VAOLE = vaole,
                            Modified_Date = DateTime.Now
                        };

                        InsertOrUpdateLineShiftPerf(lineShiftPerf);
                    }
                }
            }
            unitOfWork.Commit();
        }
        
        /// <summary>
        /// 同步上一个班次LineShiftPerf 数据
        /// </summary>
        public void SyncLineShiftPerfLastShift()
        {
            var customerList = systemProjectRepository.GetMany(x=>x.MESProject_Name!=null && x.MESProject_Name != "").ToList();
            //var unitOfWork = new UnitOfWork(_DatabaseFactory);
            var now = DateTime.Now; //当前时间
            foreach (var customer in customerList)
            {

                var shiftTimeList = shiftTimeRepository.GetMany(x => x.BG_Organization_UID == customer.Organization_UID && x.IsEnabled == true);// shiftTimeService.Query(new QueryModel<GL_ShiftTimeModelSearch>() { Equal = new GL_ShiftTimeModelSearch() { BG_Organization_UID = customer.Organization_UID, IsEnabled = true } });
                //shiftTimeList = shiftTimeList.Where(s => s.FunPlant_Organization_UID == customer.FunPlant_Organization_UID).ToList(); //班次不再需要功能厂过滤
                var currentShiftTime = GetCurrentShift(shiftTimeList.ToList(), now);
                var otherShiftTimeList = shiftTimeList.Where(s => s.ShiftTimeID != currentShiftTime.ShiftTimeID).ToList();
                if (otherShiftTimeList.Count > 0)
                {
                    foreach (var shiftTimeItem in otherShiftTimeList)
                    {
                        var startTimePair = shiftTimeItem.StartTime.Split(':');
                        var startHour = int.Parse(startTimePair[0]);
                        var startMinute = int.Parse(startTimePair[1]);

                        var endTimePair = shiftTimeItem.End_Time.Split(':');
                        var endHour = int.Parse(endTimePair[0]);
                        var endMinute = int.Parse(endTimePair[1]);
                        //var startTime = shiftTimeItem.StartTime;
                        var startDateTime = new DateTime(now.Year, now.Month, now.Day, startHour, startMinute, 0);
                        var endDateTime = new DateTime(now.Year, now.Month, now.Day, endHour, endMinute, 0);

                        //如果跨天，班次必然是从昨天延续到今天
                        if (startDateTime > endDateTime)
                        {
                            startDateTime = startDateTime.AddDays(-1);
                        }

                        //var lineShiftPerfService = new GL_LineShiftPerfService(new GL_LineShiftPerfRepository(_DatabaseFactory));
                        //goldenLineService.
                        //shiftTimeService.Query(new QueryModel<GL_ShiftTimeModelSearch>() { Equal = new GL_ShiftTimeModelSearch() { ShiftTimeID = shiftTimeItem.ShiftTimeID, } })

                        //班次的开始日期即为outputDate
                        var outputDate = string.Format("{0:yyyy-MM-dd}", startDateTime);

                        //由于GoldenLine 和OEE 共用线体GL_Line，所以线体的工站GL_Station，栏位IsGoldenLine == true 才是GoldenLine 的工站，这条线也是GoldenLine, 否则这条线不是GoldenLine（OEE 的线不在此计算），不用计算
                        var lineList = lineRepository.GetMany(x=>x.CustomerID == customer.Project_UID && x.IsEnabled == true && x.GL_Station.Any(s => s.IsGoldenLine == true));
                            //lineService.Query(new QueryModel<GL_LineModelSearch>() { Equal = new GL_LineModelSearch() { CustomerID = customer.Project_UID, IsEnabled = true } }).Where(l => l.GL_Station.Any(s => s.IsGoldenLine == true));
                        foreach (var line in lineList)
                        {
                            var lineShiftPerList = lineShiftPerfRepository.GetMany(x => x.CustomerID == customer.Project_UID && x.LineID == line.LineID && x.ShiftTimeID == shiftTimeItem.ShiftTimeID && x.OutputDate == outputDate);
                                //lineShiftPerfService.Query(new QueryModel<GL_LineShiftPerfModelSearch>() { Equal = new GL_LineShiftPerfModelSearch() { CustomerID = customer.Project_UID, LineID = line.LineID, ShiftTimeID = shiftTimeItem.ShiftTimeID, OutputDate = outputDate } });

                            if (lineShiftPerList.Count() > 0)
                            {
                                var lineShiftPer = lineShiftPerList.First();

                                //班次结束时间endDateTime 与Modified_Date 相差超过1秒则需要计算，补误差
                                TimeSpan interval = endDateTime - lineShiftPer.Modified_Date;
                                var intervalSeconds = interval.TotalSeconds;
                                if (intervalSeconds > 1)
                                {
                                    int linePlanOutput = 0;
                                    int actualOutput = 0;
                                    decimal UPH = 3600 / line.CycleTime;
                                    //公式 Standard Output=UPH*Working hours=(3600/bottleneck cycle time)*Working hours.
                                    int standOutput = Convert.ToInt32(Convert.ToDecimal((endDateTime - startDateTime).TotalSeconds) / line.CycleTime);//实际输出超过标准输出时，前端显示标红
                                    int planDownTime = 0;
                                    double upDownTime = 0;
                                    var standCT = line.CycleTime;   //获取标准CT
                                    double runTime = 0;

                                    decimal actualOutputVSPlan = 0;
                                    decimal actualOutputVSRealTimePlan = 0;
                                    decimal actualOutputVSStdOutput = 0;
                                    decimal lineUtil = 0;
                                    int actualHC = 0;
                                    decimal vaole = 0;
                                    var lineStatus = goldenLineRepository.GetLineStatus(line.LineID); //goldenLineService.GetLineStatus(line.LineID);
                                    
                                    //PlanOutput 从BuildPlan 获取, BuildPlan.OutputDate 为当天班次的开始时间
                                    var buildPlanList = buildPlanRepository.GetMany(x => x.LineID == line.LineID && x.ShiftTimeID == shiftTimeItem.ShiftTimeID && x.OutputDate == outputDate).ToList();
                                        //buildPlanRepository.Query(new QueryModel<GL_BuildPlanModelSearch>() { Equal = new GL_BuildPlanModelSearch() { LineID = line.LineID, ShiftTimeID = shiftTimeItem.ShiftTimeID, OutputDate = startDateTime.ToString("yyyy-MM-dd") } });
                                    if (buildPlanList.Count > 0)
                                    {
                                        var buildPlan = buildPlanList[0];
                                        linePlanOutput = buildPlan.PlanOutput;
                                    }
                                    //获取output
                                    actualOutput = goldenLineRepository.GetLineOutput(line.LineID, startDateTime, endDateTime);

                                    //获取StartTime,用来算PlanDownTime
                                    var lineStartTime = goldenLineRepository.GetStartTime(customer.MESProject_Name, line.MESLineName, line.Plant_Organization_UID, startDateTime, endDateTime);
                                    if (lineStartTime != null)
                                    {
                                        planDownTime = (int)(lineStartTime.Value - startDateTime).TotalSeconds;
                                    }

                                    //获取UPDownTime
                                    upDownTime = goldenLineRepository.GetUnPlanDownTime(customer.MESProject_Name, line.MESLineName, line.Plant_Organization_UID, standCT, startDateTime, endDateTime);

                                    //获取RunTime
                                    runTime = goldenLineRepository.GetRunTime(customer.MESProject_Name, line.MESLineName, line.Plant_Organization_UID, standCT, startDateTime, endDateTime);

                                    //ActualOutputVSPlan
                                    if (linePlanOutput == 0)
                                    {
                                        actualOutputVSPlan = 0;
                                    }
                                    else
                                    {
                                        //如果actualOutput超过linePlanOutput, 取linePlanOutput
                                        actualOutputVSPlan = Math.Round((decimal)(actualOutput > linePlanOutput ? linePlanOutput : actualOutput) / linePlanOutput, 3);
                                        if (actualOutputVSPlan > 1)
                                        {
                                            actualOutputVSPlan = 1;
                                        }
                                    }

                                    //ActualOutputVSRealTimePlan, 相当于挣值
                                    var shiftTimeSpanSeconds = (endDateTime - startDateTime).TotalSeconds;
                                    var realTimePlanOutput = linePlanOutput / shiftTimeSpanSeconds * (runTime + upDownTime + planDownTime); //截止当前时间的计划产能数量
                                    if (linePlanOutput == 0 || shiftTimeSpanSeconds == 0 || realTimePlanOutput == 0)
                                    {
                                        actualOutputVSRealTimePlan = 0;
                                    }
                                    else
                                    {
                                        actualOutputVSRealTimePlan = Math.Round((decimal)actualOutput / (decimal)realTimePlanOutput, 3);
                                        if (actualOutputVSRealTimePlan > 1)
                                        {
                                            actualOutputVSRealTimePlan = 1;
                                        }
                                    }

                                    //ActualOutputVSStdOutput
                                    if (standOutput == 0)
                                    {
                                        actualOutputVSStdOutput = 0;
                                    }
                                    else
                                    {
                                        actualOutputVSStdOutput = Math.Round((decimal)actualOutput / standOutput, 3);
                                        if (actualOutputVSStdOutput > 1)
                                        {
                                            actualOutputVSStdOutput = 1;
                                        }
                                    }

                                    //LineUtil
                                    if (runTime + planDownTime + upDownTime == 0)
                                    {
                                        lineUtil = 0;
                                    }
                                    else
                                    {
                                        lineUtil = Math.Round((decimal)runTime / (decimal)(runTime + planDownTime + upDownTime), 3);
                                        if (lineUtil > 1)
                                        {
                                            lineUtil = 0;
                                        }
                                    }

                                    //VAOLE,相当于工人的工作效率，即工作时间有多少百分比转化成输出
                                    //公式: 
                                    //StdUPH = PlanOutput / 10   (原本一个班次12小时，除去2小时吃饭时间，即10小时工作时间)
                                    //SMH = Plan HC / StdUPH
                                    //VAOLE= ( SMH * Actual Output ) / ( Actual HC * Actual Available Time )
                                    //输出工站获取Output
                                    // var outputStation = stationService.Query(new QueryModel<GL_StationModelSearch>() { Equal = new GL_StationModelSearch() { LineID = line.LineID, IsOutput = true } }).FirstOrDefault();
                                    var outputStation = goldenLineRepository.GetStationDTOIsOutput(line.LineID);
                                    if (outputStation != null)
                                    {
                                        //获取产线的实际输出(即产线的输出工站的输出)
                                        var output = goldenLineRepository.GetLineOutput(line.LineID, startDateTime, null);
                                        //var totoalCT = stationService.Query(new QueryModel<GL_StationModelSearch>() { Equal = new GL_StationModelSearch() { LineID = line.LineID, IsOutput = true } }).Sum(s => s.CycleTime);
                                        //var buildPlan = buildPlanService.Query(new QueryModel<GL_BuildPlanModelSearch>() { Equal = new GL_BuildPlanModelSearch() { CustomerID = customer.Project_UID, LineID = line.LineID } }).FirstOrDefault();
                                        var buildPlans = buildPlanRepository.GetMany(x=>x.CustomerID == customer.Project_UID && x.LineID == line.LineID && x.ShiftTimeID == shiftTimeItem.ShiftTimeID && x.OutputDate == outputDate).ToList();
                                            //.Query(new QueryModel<GL_BuildPlanModelSearch>() { Equal = new GL_BuildPlanModelSearch() { CustomerID = customer.Project_UID, LineID = line.LineID, ShiftTimeID = shiftTimeItem.ShiftTimeID, OutputDate = startDateTime.ToString("yyyy-MM-dd") } });
                                        if (buildPlans != null && buildPlans.Count > 0)
                                        {
                                            var buildPlan = buildPlans.FirstOrDefault();
                                            // if (buildPlan!= null)
                                            //{
                                            if (buildPlan.ActualHC != null)
                                            {
                                                actualHC = buildPlan.ActualHC.Value;
                                            }
                                            double workingTime = 0; //单位：秒
                                            workingTime = (endDateTime - startDateTime).TotalSeconds;
                                            //vaole = (decimal)(output * line.CycleTime) / (decimal)(actualHC * workingTime);
                                            decimal StdUPH = (decimal)buildPlan.PlanOutput / 10;    //除以10小时
                                            if (StdUPH > 0)
                                            {
                                                if (buildPlan.PlanHC != null)
                                                {
                                                    decimal SMH = buildPlan.PlanHC.Value / StdUPH;
                                                    if (actualHC > 0 && workingTime > 0)
                                                    {
                                                        vaole = (SMH * output) / (decimal)(actualHC * (workingTime / 3600)) * (decimal)1.2;
                                                    }
                                                }

                                            }
                                        }
                                    }


                                    var lineShiftPerf = new GL_LineShiftPerf()
                                    {
                                        CustomerID = customer.Project_UID,
                                        LineID = line.LineID,
                                        OutputDate = startDateTime.ToString("yyyy-MM-dd"),
                                        ShiftTimeID = shiftTimeItem.ShiftTimeID,
                                        PlanOutput = linePlanOutput,
                                        StandOutput = standOutput,
                                        ActualOutput = actualOutput,
                                        UPH = UPH,
                                        LineStatus = (int)lineStatus,
                                        PlanDownTime = planDownTime,
                                        UPDownTime = (int)upDownTime,//转换问题
                                        RunTime = (int)runTime,//转换问题
                                        ActualOutputVSPlan = actualOutputVSPlan,
                                        ActualOutputVSRealTimePlan = actualOutputVSRealTimePlan,
                                        ActualOutputVSStdOutput = actualOutputVSStdOutput,
                                        LineUtil = lineUtil,
                                        CapacityLoading = 0,
                                        VAOLE = vaole,
                                        Modified_Date = DateTime.Now
                                    };

                                    InsertOrUpdateLineShiftPerf(lineShiftPerf);
                                }
                            }
                        }
                    }
                }
            }
            unitOfWork.Commit();
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

        private bool InsertOrUpdateLineShiftPerf(GL_LineShiftPerf dto)
        {
            //var lineShiftPerfService = new GL_LineShiftPerfService(new GL_LineShiftPerfRepository(_DatabaseFactory));
            var lineShiftPerList = lineShiftPerfRepository.GetMany(x => x.CustomerID == dto.CustomerID && x.LineID == dto.LineID && x.ShiftTimeID == dto.ShiftTimeID && x.OutputDate == dto.OutputDate).ToList();
            //lineShiftPerfService.Query(new QueryModel<GL_LineShiftPerfModelSearch>() { Equal = new GL_LineShiftPerfModelSearch() { CustomerID = dto.CustomerID, LineID = dto.LineID, ShiftTimeID = dto.ShiftTimeID, OutputDate = dto.OutputDate } });
            try
            {
                if (lineShiftPerList.Count == 0)
                {
                    //add
                    dto.Created_Date = dto.Modified_Date;
                    Add(dto);
                }
                else
                {
                    //udpate
                    var update = lineShiftPerList[0];
                    update.ActualOutput = dto.ActualOutput;
                    update.ActualOutputVSPlan = dto.ActualOutputVSPlan;
                    update.ActualOutputVSRealTimePlan = dto.ActualOutputVSRealTimePlan;
                    update.ActualOutputVSStdOutput = dto.ActualOutputVSStdOutput;
                    update.AssemblyID = dto.AssemblyID;
                    update.CapacityLoading = dto.CapacityLoading;
                    update.LineStatus = dto.LineStatus;
                    update.LineUtil = dto.LineUtil;
                    update.PlanDownTime = dto.PlanDownTime;
                    update.PlanOutput = dto.PlanOutput;
                    update.RunTime = dto.RunTime;
                    update.StandOutput = dto.StandOutput;
                    update.UPDownTime = dto.UPDownTime;
                    update.UPH = dto.UPH;
                    update.VAOLE = dto.VAOLE;
                    update.Modified_Date = dto.Modified_Date;
                    Update(update);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
