using PDMS.Common.Constants;
using PDMS.Common.Enums;
using PDMS.Data;
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
    public interface IGoldenLineService
    {
        Dictionary<string, int> GetOutput(string MESCustomerName, string MESStationName, DateTime startTime, DateTime? endTime, int Plant_Organization_UID);
        int GetLineOutput(int lineID, DateTime startTime, DateTime? endTime);
        decimal GetCustomerActualVSRealTimePlan(int customerID, int shiftTimeID, string outputDate);
        decimal GetCustomerVAOLE(int customerID, int shiftTimeID, string outputDate);
        EnumLineStatus GetLineStatus(int lineID);
        EnumLineStatus GetLineStatus(int lineID, DateTime dateTime);
        DateTime? GetStartTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime startTime, DateTime? endTime);
        double GetRunTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime);
        double GetUnPlanDownTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime);
        DateTime GetLastUpdateTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime flagTime);

        //List<GL_CustomerDTO> QueryCustomers(int oporgid, int? funorgid);
        List<StationStdActCTInfoDTO> GetStationStdActCT(int customerId, int lineId, string outputDate, int shiftTimeID);
        LatestLineStationInfoDTO GetLatestLineStationInfo(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID);
        List<LineShiftPlanActInfoDTO> GetShiftHourOutput(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID);
        ActualAndPlanDTO GetActualAndPlanDTOs(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID);
        GL_ShiftTimeDTO GetShiftTimeDTO(int ShiftTimeID);
        List<GLBuildPlanDTO> GetBuildPlanDTOList(int LineID, string week);
        List<GLBuildPlanDTO> GetBuildPlanDTOListAllAPI(int LineID, string week);


        string ImportBuildPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs);
        PagedListModel<GLBuildPlanVM> QueryPlanData(int LineID, string date);
        PagedListModel<GLBuildHCPlanVM> QueryHCPlanData(int LineID, string date);
        PagedListModel<GLHCActuaWM> QueryHCActuaData(int LineID, string date);
        string ImportBuildHCPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs);
        string ImportBuildHCActua(List<GLBuildPlanDTO> GLBuildPlanDTOs);
        GLHCActuaWM QueryHCActuaDataByLineID(int LineID, int ShiftTimeID, string date, string WeekDay);
        GLHCActuaWM SearchPlanHCDataByLineID(int LineID, int ShiftTimeID, string date, string WeekDay);
        GLHCActuaWM SearchPlanDataByLineID(int LineID, int ShiftTimeID, string date, string WeekDay);

        string InserOrUpdateStations(List<GL_StationDTO> listStations);

        GL_StationDTO GetStationDTOIsOutput(int LineID);
        PagedListModel<OEE_UserStationDTO> QueryOperatorList(int LineID);
        List<OEE_UserStationDTO> DownloadOperatorListExcel(int LineID);

        List<GL_StationDTO> GetStationDTO(int CustomerID);

        List<OEE_UserStationDTO> GetOperatorList(int CustomerID);

        List<GL_LineDTO> GetOEELineDTO(int CustomerID);
        List<GL_LineDTO> GetIPQCLineDTO(int CustomerID);
        
        List<SystemUserOEEDTO> GetAllUserByDTOs(int Plant_OrganizationUID);
        string ImportOperatorList(List<OEE_UserStationDTO> OperatorLists);
    }
    public class GoldenLineService : IGoldenLineService
    {
        private readonly IGoldenLineRepository goldenLineRepository;
        private readonly ISystemProjectRepository projectRepository;
        private readonly IGL_GoldenStationCTRecordRepository gL_GoldenStationCTRecordRepository;
        private readonly IGL_ShiftTimeRepository shiftTimeRepository;
        public GoldenLineService(IGoldenLineRepository goldenLineRepository, IGL_GoldenStationCTRecordRepository gL_GoldenStationCTRecordRepository, IGL_ShiftTimeRepository shiftTimeRepository, ISystemProjectRepository projectRepository)
        {
            this.goldenLineRepository = goldenLineRepository;
            this.gL_GoldenStationCTRecordRepository = gL_GoldenStationCTRecordRepository;
            this.shiftTimeRepository = shiftTimeRepository;
            this.projectRepository = projectRepository;
        }
        public Dictionary<string, int> GetOutput(string MESCustomerName, string MESStationName, DateTime startTime, DateTime? endTime,int Plant_Organization_UID)
        {
            var result = goldenLineRepository.GetOutput(MESCustomerName, MESStationName, startTime, endTime, Plant_Organization_UID);
            return result;
        }
        public int GetLineOutput(int lineID, DateTime startTime, DateTime? endTime)
        {
            var result = goldenLineRepository.GetLineOutput(lineID, startTime, endTime);
            return result;
        }

        public decimal GetCustomerActualVSRealTimePlan(int customerID, int shiftTimeID, string outputDate)
        {
            decimal result = 0;
            var totalPlanOutput = 0;
            var totalActualOutput = 0;

            var customer = projectRepository.GetFirstOrDefault(c => c.Project_UID == customerID);
            var shiftTime = shiftTimeRepository.GetFirstOrDefault(s => s.ShiftTimeID == shiftTimeID);

            if (shiftTime != null)
            {
                var now = DateTime.Now;

                var outputDatePair = outputDate.Split('-');
                var outputYear = int.Parse(outputDatePair[0]);
                var outputMonth = int.Parse(outputDatePair[1]);
                var outputDay = int.Parse(outputDatePair[2]);

                var startTimePair = shiftTime.StartTime.Split(':');
                var startHour = int.Parse(startTimePair[0]);
                var startMinute = int.Parse(startTimePair[1]);

                var endTimePair = shiftTime.End_Time.Split(':');
                var endHour = int.Parse(endTimePair[0]);
                var endMinute = int.Parse(endTimePair[1]);

                var shiftStartTime = new DateTime(outputYear, outputMonth, outputDay, startHour, startMinute, 0);
                var shiftEndTime = new DateTime(outputYear, outputMonth, outputDay, endHour, endMinute, 0);
                if (shiftStartTime > shiftEndTime)
                {
                    shiftEndTime = shiftEndTime.AddDays(1);
                }
                //如果当前时间超过班次结束时间，取班次结束时间
                if (now > shiftEndTime)
                {
                    now = shiftEndTime;
                }
                var timeRatio = ((decimal)(now - shiftStartTime).TotalSeconds) / (decimal)(shiftEndTime - shiftStartTime).TotalSeconds;
                //累计实际输出、实际输出
                var lineList = customer.GL_LineShiftPerf.Where(l => l.CustomerID == customerID && l.OutputDate == outputDate && l.ShiftTimeID == shiftTimeID && l.GL_Line.IsEnabled == true && l.GL_Line.GL_Station.Any(s => s.IsGoldenLine == true));
                foreach (var item in lineList)
                {
                    if (item.PlanOutput != 0)
                    {
                        totalPlanOutput += item.PlanOutput;
                        //实际输出超过计划输出，取计划输出
                        totalActualOutput += item.ActualOutput > item.PlanOutput * timeRatio ? (int)(item.PlanOutput * timeRatio) : item.ActualOutput;
                    }
                }

                if (totalPlanOutput > 0)
                {
                    if (now >= shiftStartTime && now < shiftEndTime)
                    {
                        //(相当于挣值)=实际产量/截止当前时间的计划产量
                        result = totalActualOutput / (totalPlanOutput * timeRatio);
                    }
                    else
                    {
                        result = totalActualOutput / (decimal)totalPlanOutput;
                    }
                }
            }
            return result;
        }
        public decimal GetCustomerVAOLE(int customerID, int shiftTimeID, string outputDate)
        {
            //公式: 
            //StdUPH = PlanOutput / 10   (原本一个班次12小时，除去2小时吃饭时间，即10小时工作时间)
            //SMH = Plan HC / StdUPH
            //VAOLE= ( SMH * Actual Output ) / ( Actual HC * Actual Available Time )

            decimal result = 0;
            //decimal totalActualOutputXCycleTime = 0;  //这里不除以3600, 直接按秒计算  相关公式:VAOLE=(∑OutputQty*SMH)/(∑HC*WorkingTime); SMH = CycleTiem/3600
            //decimal totalActualHCXWorkingTime = 0;
            decimal workingTime = 0;

            var customer = projectRepository.GetFirstOrDefault(c => c.Project_UID == customerID);
            var shiftTime = shiftTimeRepository.GetFirstOrDefault(s => s.ShiftTimeID == shiftTimeID);

            if (shiftTime != null)
            {
                var now = DateTime.Now;

                var outputDatePair = outputDate.Split('-');
                var outputYear = int.Parse(outputDatePair[0]);
                var outputMonth = int.Parse(outputDatePair[1]);
                var outputDay = int.Parse(outputDatePair[2]);

                var startTimePair = shiftTime.StartTime.Split(':');
                var startHour = int.Parse(startTimePair[0]);
                var startMinute = int.Parse(startTimePair[1]);

                var endTimePair = shiftTime.End_Time.Split(':');
                var endHour = int.Parse(endTimePair[0]);
                var endMinute = int.Parse(endTimePair[1]);

                var shiftStartTime = new DateTime(outputYear, outputMonth, outputDay, startHour, startMinute, 0);
                var shiftEndTime = new DateTime(outputYear, outputMonth, outputDay, endHour, endMinute, 0);
                if (shiftStartTime > shiftEndTime)
                {
                    shiftEndTime = shiftEndTime.AddDays(1);
                }

                decimal planOutput = 0;
                decimal planHC = 0;
                var actualOutput = 0;
                var actualHC = 0;

                //如果是正在进行的班次，取当前时间和班次开始时间的时间差，否则取班次开始到结束的时间差
                if (now >= shiftEndTime)
                {
                    workingTime += (decimal)(shiftEndTime - shiftStartTime).TotalSeconds;
                }
                else
                {
                    workingTime += (decimal)(now - shiftStartTime).TotalSeconds;
                }

                //遍历，计算需要累加的数据
                var lineShiftPerfList = customer.GL_LineShiftPerf.Where(l => l.OutputDate == outputDate && l.ShiftTimeID == shiftTimeID && l.GL_Line.IsEnabled == true && l.GL_Line.GL_Station.Any(s => s.IsGoldenLine == true));
                foreach (var lineShiftPerf in lineShiftPerfList)
                {

                    var line = customer.GL_Line.FirstOrDefault(l => l.LineID == lineShiftPerf.LineID && l.IsEnabled == true);
                    //if (line != null && line.CycleTime > 0)
                    //{
                    //    totalActualOutputXCycleTime += lineShiftPerf.ActualOutput * line.CycleTime; //实际输出的总耗时
                    //}
                    if (line != null)
                    {
                        //有计划的才参与运算
                        var buildPlan = customer.GL_BuildPlan.FirstOrDefault(b => b.CustomerID == customer.Project_UID && b.OutputDate == outputDate && b.ShiftTimeID == shiftTimeID && b.LineID == line.LineID);
                        if (buildPlan != null && buildPlan.PlanOutput != 0)
                        {
                            //PlanHC为0 或ActualHC为0，不参与运算
                            if (lineShiftPerf.ActualOutput > 0 && buildPlan.PlanHC.HasValue && buildPlan.PlanHC.Value > 0 && buildPlan.ActualHC.HasValue && buildPlan.ActualHC.Value > 0)
                            {
                                actualOutput += lineShiftPerf.ActualOutput;
                                planOutput += buildPlan.PlanOutput;
                                planHC += buildPlan.PlanHC.Value;
                                actualHC += buildPlan.ActualHC.Value;
                            }

                            //if (buildPlan.ActualHC.HasValue)
                            //{
                            //    if (now >= shiftEndTime)
                            //    {
                            //        totalActualHCXWorkingTime += (decimal)(shiftEndTime - shiftStartTime).TotalSeconds * buildPlan.ActualHC.Value;
                            //    }
                            //    else
                            //    {
                            //        totalActualHCXWorkingTime += (decimal)(now - shiftStartTime).TotalSeconds * buildPlan.ActualHC.Value;
                            //    }
                            //}//累加PlanOutput
                        }
                    }
                }

                decimal StdUPH = planOutput / 10;   //(原本一个班次12小时，除去2小时吃饭时间，即10小时工作时间)
                decimal SMH = 0;
                if (StdUPH != 0)
                {
                    SMH = planHC / StdUPH;
                }


                if (actualHC != 0 && workingTime != 0)
                {
                    //result = totalActualOutputXCycleTime / totalActualHCXWorkingTime;//原公式
                    result = (SMH * actualOutput) / (actualHC * (workingTime / 3600)) * (decimal)1.2;
                }
            }

            return result;
        }

        public EnumLineStatus GetLineStatus(int lineID)
        {
            var result = goldenLineRepository.GetLineStatus(lineID);
            return result;
        }
        public EnumLineStatus GetLineStatus(int lineID, DateTime dateTime)
        {
            var result = goldenLineRepository.GetLineStatus(lineID, dateTime);
            return result;
        }
        public DateTime? GetStartTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime startTime, DateTime? endTime)
        {
            var result = goldenLineRepository.GetStartTime(MESCustomerName, MESLineName,Plant_Organization_UID, startTime, endTime);
            return result;
        }
        public double GetRunTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime)
        {
            var result = goldenLineRepository.GetRunTime(MESCustomerName, MESLineName,Plant_Organization_UID, STDCT, startTime, endTime);
            return result;
        }
        public double GetUnPlanDownTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime)
        {
            var result = goldenLineRepository.GetUnPlanDownTime(MESCustomerName, MESLineName,Plant_Organization_UID, STDCT, startTime, endTime);
            return result;
        }
        public DateTime GetLastUpdateTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime flagTime)
        {
            var result = goldenLineRepository.GetLastUpdateTime(MESCustomerName, MESLineName,Plant_Organization_UID, flagTime);
            return result;
        }
        //public List<GL_CustomerDTO> QueryCustomers(int oporgid, int? funorgid)
        //{
        //    var result = goldenLineRepository.(oporgid, funorgid);
        //    return result;
        //}
        public List<StationStdActCTInfoDTO> GetStationStdActCT(int customerId, int lineId, string outputDate, int shiftTimeID)
        {
            var result = gL_GoldenStationCTRecordRepository.GetStationStdActCT(customerId, lineId, outputDate, shiftTimeID);
            return result;

        }
        public LatestLineStationInfoDTO GetLatestLineStationInfo(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID)
        {
            var customer = projectRepository.GetFirstOrDefault(x => x.Project_UID == customerId);
            LatestLineStationInfoDTO LatestLineStationInfo = gL_GoldenStationCTRecordRepository.GetLatestLineStationInfo(customerId, lineId, stationId, outputDate, shiftTimeID);
            DateTime dt = DateTime.Now;
            if (LatestLineStationInfo != null)
            {
                GL_ShiftTimeDTO ShiftTimeDTO = gL_GoldenStationCTRecordRepository.GetShiftTimeDTO(shiftTimeID);
                if (ShiftTimeDTO != null)
                {

                    DateTime StartTime = Convert.ToDateTime((outputDate + " " + ShiftTimeDTO.StartTime));
                    DateTime EndTime = Convert.ToDateTime((outputDate + " " + ShiftTimeDTO.End_Time));
                    var startTime = GetStartTime(LatestLineStationInfo.MESProjectName, LatestLineStationInfo.MESLineName, customer.Organization_UID, StartTime, EndTime);
                    if (startTime != null)
                    {
                        dt = startTime.Value;
                    }
                }
            }
            if (LatestLineStationInfo != null)
            {
                LatestLineStationInfo.LineStartTime = dt;
            }


            return LatestLineStationInfo;
        }
        public List<LineShiftPlanActInfoDTO> GetShiftHourOutput(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID)
        {
            return gL_GoldenStationCTRecordRepository.GetLineShiftPlanActInfo(customerId, lineId, stationId, outputDate, shiftTimeID);
        }
        public ActualAndPlanDTO GetActualAndPlanDTOs(int customerId, int lineId, int stationId, string outputDate, int shiftTimeID)
        {

            var actualAndPlanDTO = gL_GoldenStationCTRecordRepository.GetActualAndPlanDTOs(customerId, lineId, stationId, outputDate, shiftTimeID);
            int Output = 0;
            decimal ActualVAOLE = 0;
            decimal UPH = 0;

            //   decimal PlanVAOLE = 0;
            if (actualAndPlanDTO != null)
            {
                Dictionary<string, int> stationOutPut = new Dictionary<string, int>();
                var dateTimeNow = DateTime.Now;
                if (actualAndPlanDTO.StartDateTime <= dateTimeNow && actualAndPlanDTO.EndDateTime >= dateTimeNow)
                {
                    stationOutPut = GetOutput(actualAndPlanDTO.MESCustomerName, actualAndPlanDTO.MESStationName, actualAndPlanDTO.StartDateTime, dateTimeNow,actualAndPlanDTO.Plant_Organization_UID);
                }
                else
                {
                    stationOutPut = GetOutput(actualAndPlanDTO.MESCustomerName, actualAndPlanDTO.MESStationName, actualAndPlanDTO.StartDateTime, actualAndPlanDTO.EndDateTime, actualAndPlanDTO.Plant_Organization_UID);
                }
                if (stationOutPut != null && stationOutPut.Count > 0)
                {
                    Output = stationOutPut[actualAndPlanDTO.MESStationName];

                }
                if (actualAndPlanDTO.ActualHC != 0 && actualAndPlanDTO.SumTime != 0 && actualAndPlanDTO.PlanOutPut != 0)
                {
                    ActualVAOLE = (Output * (actualAndPlanDTO.PlanHC / (Convert.ToDecimal(actualAndPlanDTO.PlanOutPut * 1.0) / 10))) / (actualAndPlanDTO.ActualHC * actualAndPlanDTO.SumTime);
                }

                int TimeCount = GetTimeCount(actualAndPlanDTO.StartDateTime, dateTimeNow, actualAndPlanDTO.EndDateTime);

                if (TimeCount > 0)
                {
                    //Dictionary<string, int> stationUPHOutPut = new Dictionary<string, int>();
                    //stationUPHOutPut = GetOutput(actualAndPlanDTO.MESCustomerName, actualAndPlanDTO.MESStationName, actualAndPlanDTO.StartDateTime, actualAndPlanDTO.StartDateTime.AddHours(TimeCount));

                    //if (stationUPHOutPut != null && stationUPHOutPut.Count > 0)
                    //{
                    //  UPH = stationUPHOutPut[actualAndPlanDTO.MESStationName]/ TimeCount;
                    UPH = Convert.ToDecimal(Output * 1.0) / TimeCount;

                    //}
                }


            }

            decimal actualUPPH = 0;

            if (actualAndPlanDTO.ActualHC != 0)
            {
                actualUPPH = UPH / actualAndPlanDTO.ActualHC;
            }




            actualAndPlanDTO.ActualUPPH = actualUPPH;
            actualAndPlanDTO.ActualSMH = UPH;
            // actualAndPlanDTO.ActualVAOLE =ActualVAOLE * 3600*1.2;
            actualAndPlanDTO.ActualVAOLE = ActualVAOLE * 4320;
            actualAndPlanDTO.ActualOutPut = Output;
            actualAndPlanDTO.DateTimeNOW = DateTime.Now.ToString(FormatConstants.DateTimeFormatString);
            return actualAndPlanDTO;
        }

        public int GetTimeCount(DateTime StartDateTime, DateTime DateTimeNow, DateTime EndDateTime)
        {
            int TimeCount = 0;
            if (DateTimeNow > EndDateTime)
            {

                TimeCount = 12;

            }
            else
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (StartDateTime.AddHours(i) < DateTimeNow && DateTimeNow < EndDateTime)
                    {
                        TimeCount = i;
                    }
                }
            }
            return TimeCount;
        }


        public GL_ShiftTimeDTO GetShiftTimeDTO(int ShiftTimeID)
        {
            return gL_GoldenStationCTRecordRepository.GetShiftTimeDTO(ShiftTimeID);
        }

        public List<GLBuildPlanDTO> GetBuildPlanDTOListAllAPI(int LineID, string week)
        {
            List<GLBuildPlanDTO> GLBuildPlanDTOs = new List<GLBuildPlanDTO>();
            var LineDTO = gL_GoldenStationCTRecordRepository.GetOneLineDTO(LineID);
            List<GL_ShiftTimeDTO> GL_ShiftTimeDTOs = new List<GL_ShiftTimeDTO>();
            if (LineDTO != null)
            {
                GL_ShiftTimeDTOs = gL_GoldenStationCTRecordRepository.GetShiftTimeDTO(LineDTO.Plant_Organization_UID, LineDTO.BG_Organization_UID);

                var LineDTOs = gL_GoldenStationCTRecordRepository.GetLineDTOs(LineDTO.CustomerID);

                if (GL_ShiftTimeDTOs != null && GL_ShiftTimeDTOs.Count > 0 && LineDTOs != null && LineDTOs.Count > 0)
                {
                    foreach (var OneLineDTO in LineDTOs)
                    {
                        foreach (var item in GL_ShiftTimeDTOs)
                        {
                            GLBuildPlanDTO gLBuildPlanDTO = new GLBuildPlanDTO();
                            gLBuildPlanDTO.ShiftTime = item.Shift;
                            gLBuildPlanDTO.ProjectName = OneLineDTO.ProjectName;
                            gLBuildPlanDTO.MESProjectName = OneLineDTO.MESProjectName;  //added by Jay
                            gLBuildPlanDTO.LineName = OneLineDTO.LineName;
                            gLBuildPlanDTO.Plant_Organization = OneLineDTO.Plant_Organization;
                            gLBuildPlanDTO.BG_Organization = OneLineDTO.BG_Organization;
                            gLBuildPlanDTO.ShiftTimeID = item.ShiftTimeID;
                            gLBuildPlanDTO.LineID = OneLineDTO.LineID;
                            GLBuildPlanDTOs.Add(gLBuildPlanDTO);

                        }
                    }


                }
            }
            return GLBuildPlanDTOs;
        }
        public List<GLBuildPlanDTO> GetBuildPlanDTOList(int LineID, string week)
        {
            List<GLBuildPlanDTO> GLBuildPlanDTOs = new List<GLBuildPlanDTO>();
            var OneLineDTO = gL_GoldenStationCTRecordRepository.GetOneLineDTO(LineID);
            List<GL_ShiftTimeDTO> GL_ShiftTimeDTOs = new List<GL_ShiftTimeDTO>();
            if (OneLineDTO != null)
            {
                GL_ShiftTimeDTOs = gL_GoldenStationCTRecordRepository.GetShiftTimeDTO(OneLineDTO.Plant_Organization_UID, OneLineDTO.BG_Organization_UID);


                if (GL_ShiftTimeDTOs != null && GL_ShiftTimeDTOs.Count > 0)
                {
                    foreach (var item in GL_ShiftTimeDTOs)
                    {
                        GLBuildPlanDTO gLBuildPlanDTO = new GLBuildPlanDTO();
                        gLBuildPlanDTO.ShiftTime = item.Shift;
                        gLBuildPlanDTO.ProjectName = OneLineDTO.ProjectName;
                        gLBuildPlanDTO.MESProjectName = OneLineDTO.MESProjectName;  //added by Jay
                        gLBuildPlanDTO.LineName = OneLineDTO.LineName;
                        gLBuildPlanDTO.Plant_Organization = OneLineDTO.Plant_Organization;
                        gLBuildPlanDTO.BG_Organization = OneLineDTO.BG_Organization;
                        gLBuildPlanDTO.ShiftTimeID = item.ShiftTimeID;
                        GLBuildPlanDTOs.Add(gLBuildPlanDTO);

                    }

                }
            }
            return GLBuildPlanDTOs;
        }
        public string ImportBuildPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {
            return gL_GoldenStationCTRecordRepository.ImportBuildPlans(GLBuildPlanDTOs);

        }
        public string ImportBuildHCPlans(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {
            return gL_GoldenStationCTRecordRepository.ImportBuildHCPlans(GLBuildPlanDTOs);
        }
        public string ImportBuildHCActua(List<GLBuildPlanDTO> GLBuildPlanDTOs)
        {
            return gL_GoldenStationCTRecordRepository.ImportBuildHCActua(GLBuildPlanDTOs);
        }
        public PagedListModel<GLBuildPlanVM> QueryPlanData(int LineID, string date)
        {
            DateTime StartDate = DateTime.Parse(date);
            DateTime EndDate = StartDate.AddDays(7);
            var totalCount = 0;
            var buildPlans = gL_GoldenStationCTRecordRepository.GetBuildPlans(LineID, StartDate, EndDate);
            var result = new List<GLBuildPlanVM>();
            if (buildPlans != null && buildPlans.Count > 0)
            {
                List<int> ShiftTimes = buildPlans.Select(o => o.ShiftTimeID).Distinct().ToList();
                foreach (var itemTime in ShiftTimes)
                {
                    var everybuildPlans = buildPlans.Where(o => o.ShiftTimeID == itemTime).ToList();
                    if (everybuildPlans != null && everybuildPlans.Count > 0)
                    {
                        var returnItem = new GLBuildPlanVM();
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.ShiftTimeID = everybuildPlans[0].ShiftTimeID;
                        returnItem.LineID = everybuildPlans[0].LineID;
                        returnItem.CustomerID = everybuildPlans[0].CustomerID;
                        //returnItem.OutputDate = everybuildPlans[0].OutputDate;
                        returnItem.Plant_Organization_UID = everybuildPlans[0].Plant_Organization_UID;
                        returnItem.BG_Organization_UID = everybuildPlans[0].BG_Organization_UID;
                        returnItem.FunPlant_Organization_UID = everybuildPlans[0].FunPlant_Organization_UID;
                        returnItem.CustomerName = everybuildPlans[0].ProjectName;
                        returnItem.LineName = everybuildPlans[0].LineName;
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.BG_Organization = everybuildPlans[0].BG_Organization;
                        returnItem.FunPlant_Organization = everybuildPlans[0].FunPlant_Organization;
                        returnItem.ShiftTime = everybuildPlans[0].ShiftTime;
                        foreach (var item in everybuildPlans)
                        {

                            DateTime dateItem = Convert.ToDateTime(item.OutputDate);
                            DayOfWeek weekDay = dateItem.DayOfWeek;
                            string strweekDay = weekDay.ToString();
                            switch (strweekDay)
                            {
                                case "Monday":
                                    returnItem.MondayPlan = item.PlanOutput;
                                    break;
                                case "Tuesday":
                                    returnItem.TuesdayPlan = item.PlanOutput;
                                    break;
                                case "Wednesday":
                                    returnItem.WednesdayPlan = item.PlanOutput;
                                    break;
                                case "Thursday":
                                    returnItem.ThursdayPlan = item.PlanOutput;
                                    break;
                                case "Friday":
                                    returnItem.FridayPlan = item.PlanOutput;
                                    break;
                                case "Saturday":
                                    returnItem.SaterdayPlan = item.PlanOutput;
                                    break;
                                case "Sunday":
                                    returnItem.SundayPlan = item.PlanOutput;
                                    break;
                            }
                        }
                        result.Add(returnItem);
                    }
                }
            }
            return new PagedListModel<GLBuildPlanVM>(totalCount, result);
        }
        public PagedListModel<GLBuildHCPlanVM> QueryHCPlanData(int LineID, string date)
        {
            DateTime StartDate = DateTime.Parse(date);
            DateTime EndDate = StartDate.AddDays(7);
            var totalCount = 0;
            var buildPlans = gL_GoldenStationCTRecordRepository.GetBuildPlans(LineID, StartDate, EndDate);
            var result = new List<GLBuildHCPlanVM>();
            if (buildPlans != null && buildPlans.Count > 0)
            {
                List<int> ShiftTimes = buildPlans.Select(o => o.ShiftTimeID).Distinct().ToList();
                foreach (var itemTime in ShiftTimes)
                {
                    var everybuildPlans = buildPlans.Where(o => o.ShiftTimeID == itemTime).ToList();
                    if (everybuildPlans != null && everybuildPlans.Count > 0)
                    {
                        var returnItem = new GLBuildHCPlanVM();
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.ShiftTimeID = everybuildPlans[0].ShiftTimeID;
                        returnItem.LineID = everybuildPlans[0].LineID;
                        returnItem.CustomerID = everybuildPlans[0].CustomerID;
                        //returnItem.OutputDate = everybuildPlans[0].OutputDate;
                        returnItem.Plant_Organization_UID = everybuildPlans[0].Plant_Organization_UID;
                        returnItem.BG_Organization_UID = everybuildPlans[0].BG_Organization_UID;
                        returnItem.FunPlant_Organization_UID = everybuildPlans[0].FunPlant_Organization_UID;
                        returnItem.CustomerName = everybuildPlans[0].ProjectName;
                        returnItem.LineName = everybuildPlans[0].LineName;
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.BG_Organization = everybuildPlans[0].BG_Organization;
                        returnItem.FunPlant_Organization = everybuildPlans[0].FunPlant_Organization;
                        returnItem.ShiftTime = everybuildPlans[0].ShiftTime;
                        foreach (var item in everybuildPlans)
                        {

                            DateTime dateItem = Convert.ToDateTime(item.OutputDate);
                            DayOfWeek weekDay = dateItem.DayOfWeek;
                            string strweekDay = weekDay.ToString();
                            switch (strweekDay)
                            {
                                case "Monday":
                                    returnItem.MondayHCPlan = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Tuesday":
                                    returnItem.TuesdayHCPlan = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Wednesday":
                                    returnItem.WednesdayHCPlan = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Thursday":
                                    returnItem.ThursdayHCPlan = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Friday":
                                    returnItem.FridayHCPlan = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Saturday":
                                    returnItem.SaterdayHCPlan = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Sunday":
                                    returnItem.SundayHCPlan = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                            }
                        }
                        result.Add(returnItem);
                    }
                }
            }
            return new PagedListModel<GLBuildHCPlanVM>(totalCount, result);

        }
        public PagedListModel<GLHCActuaWM> QueryHCActuaData(int LineID, string date)
        {
            DateTime StartDate = DateTime.Parse(date);
            DateTime EndDate = StartDate.AddDays(7);
            var totalCount = 0;
            var buildPlans = gL_GoldenStationCTRecordRepository.GetBuildPlans(LineID, StartDate, EndDate);
            var result = new List<GLHCActuaWM>();
            if (buildPlans != null && buildPlans.Count > 0)
            {
                List<int> ShiftTimes = buildPlans.Select(o => o.ShiftTimeID).Distinct().ToList();
                foreach (var itemTime in ShiftTimes)
                {
                    var everybuildPlans = buildPlans.Where(o => o.ShiftTimeID == itemTime).ToList();
                    if (everybuildPlans != null && everybuildPlans.Count > 0)
                    {
                        var returnItem = new GLHCActuaWM();
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.ShiftTimeID = everybuildPlans[0].ShiftTimeID;
                        returnItem.LineID = everybuildPlans[0].LineID;
                        returnItem.CustomerID = everybuildPlans[0].CustomerID;
                        //returnItem.OutputDate = everybuildPlans[0].OutputDate;
                        returnItem.Plant_Organization_UID = everybuildPlans[0].Plant_Organization_UID;
                        returnItem.BG_Organization_UID = everybuildPlans[0].BG_Organization_UID;
                        returnItem.FunPlant_Organization_UID = everybuildPlans[0].FunPlant_Organization_UID;
                        returnItem.CustomerName = everybuildPlans[0].ProjectName;
                        returnItem.LineName = everybuildPlans[0].LineName;
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.BG_Organization = everybuildPlans[0].BG_Organization;
                        returnItem.FunPlant_Organization = everybuildPlans[0].FunPlant_Organization;
                        returnItem.ShiftTime = everybuildPlans[0].ShiftTime;
                        foreach (var item in everybuildPlans)
                        {

                            DateTime dateItem = Convert.ToDateTime(item.OutputDate);
                            DayOfWeek weekDay = dateItem.DayOfWeek;
                            string strweekDay = weekDay.ToString();
                            switch (strweekDay)
                            {
                                case "Monday":
                                    returnItem.MondayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Tuesday":
                                    returnItem.TuesdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Wednesday":
                                    returnItem.WednesdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Thursday":
                                    returnItem.ThursdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Friday":
                                    returnItem.FridayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Saturday":
                                    returnItem.SaterdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Sunday":
                                    returnItem.SundayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                            }
                        }
                        result.Add(returnItem);
                    }
                }
            }
            return new PagedListModel<GLHCActuaWM>(totalCount, result);
        }
        public GLHCActuaWM QueryHCActuaDataByLineID(int LineID, int ShiftTimeID, string date, string WeekDay)
        {
            DateTime StartDate = DateTime.Parse(date);
            DateTime EndDate = StartDate.AddDays(7);

            var buildPlans = gL_GoldenStationCTRecordRepository.GetBuildPlans(LineID, StartDate, EndDate);
            var result = new List<GLHCActuaWM>();
            if (buildPlans != null && buildPlans.Count > 0)
            {
                List<int> ShiftTimes = buildPlans.Select(o => o.ShiftTimeID).Distinct().ToList();
                foreach (var itemTime in ShiftTimes)
                {
                    var everybuildPlans = buildPlans.Where(o => o.ShiftTimeID == itemTime).ToList();
                    if (everybuildPlans != null && everybuildPlans.Count > 0)
                    {
                        var returnItem = new GLHCActuaWM();
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.ShiftTimeID = everybuildPlans[0].ShiftTimeID;
                        returnItem.LineID = everybuildPlans[0].LineID;
                        returnItem.CustomerID = everybuildPlans[0].CustomerID;
                        //returnItem.OutputDate = everybuildPlans[0].OutputDate;
                        returnItem.Plant_Organization_UID = everybuildPlans[0].Plant_Organization_UID;
                        returnItem.BG_Organization_UID = everybuildPlans[0].BG_Organization_UID;
                        returnItem.FunPlant_Organization_UID = everybuildPlans[0].FunPlant_Organization_UID;
                        returnItem.CustomerName = everybuildPlans[0].ProjectName;
                        returnItem.LineName = everybuildPlans[0].LineName;
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.BG_Organization = everybuildPlans[0].BG_Organization;
                        returnItem.FunPlant_Organization = everybuildPlans[0].FunPlant_Organization;
                        returnItem.ShiftTime = everybuildPlans[0].ShiftTime;
                        returnItem.WeekDay = WeekDay;
                        foreach (var item in everybuildPlans)
                        {

                            DateTime dateItem = Convert.ToDateTime(item.OutputDate);
                            DayOfWeek weekDay = dateItem.DayOfWeek;
                            string strweekDay = weekDay.ToString();
                            switch (strweekDay)
                            {
                                case "Monday":
                                    returnItem.MondayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Tuesday":
                                    returnItem.TuesdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Wednesday":
                                    returnItem.WednesdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Thursday":
                                    returnItem.ThursdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Friday":
                                    returnItem.FridayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Saturday":
                                    returnItem.SaterdayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                                case "Sunday":
                                    returnItem.SundayHCActua = item.ActualHC == null ? 0 : item.ActualHC.Value;
                                    break;
                            }
                        }
                        result.Add(returnItem);
                    }
                }
            }

            if (result.Count > 0)
            {
                return result.FirstOrDefault(o => o.ShiftTimeID == ShiftTimeID);

            }
            else
            {
                return null;
            }

        }

        public GLHCActuaWM SearchPlanHCDataByLineID(int LineID, int ShiftTimeID, string date, string WeekDay)
        {
            DateTime StartDate = DateTime.Parse(date);
            DateTime EndDate = StartDate.AddDays(7);

            var buildPlans = gL_GoldenStationCTRecordRepository.GetBuildPlans(LineID, StartDate, EndDate);
            var result = new List<GLHCActuaWM>();
            if (buildPlans != null && buildPlans.Count > 0)
            {
                List<int> ShiftTimes = buildPlans.Select(o => o.ShiftTimeID).Distinct().ToList();
                foreach (var itemTime in ShiftTimes)
                {
                    var everybuildPlans = buildPlans.Where(o => o.ShiftTimeID == itemTime).ToList();
                    if (everybuildPlans != null && everybuildPlans.Count > 0)
                    {
                        var returnItem = new GLHCActuaWM();
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.ShiftTimeID = everybuildPlans[0].ShiftTimeID;
                        returnItem.LineID = everybuildPlans[0].LineID;
                        returnItem.CustomerID = everybuildPlans[0].CustomerID;
                        //returnItem.OutputDate = everybuildPlans[0].OutputDate;
                        returnItem.Plant_Organization_UID = everybuildPlans[0].Plant_Organization_UID;
                        returnItem.BG_Organization_UID = everybuildPlans[0].BG_Organization_UID;
                        returnItem.FunPlant_Organization_UID = everybuildPlans[0].FunPlant_Organization_UID;
                        returnItem.CustomerName = everybuildPlans[0].ProjectName;
                        returnItem.LineName = everybuildPlans[0].LineName;
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.BG_Organization = everybuildPlans[0].BG_Organization;
                        returnItem.FunPlant_Organization = everybuildPlans[0].FunPlant_Organization;
                        returnItem.ShiftTime = everybuildPlans[0].ShiftTime;
                        returnItem.WeekDay = WeekDay;
                        foreach (var item in everybuildPlans)
                        {

                            DateTime dateItem = Convert.ToDateTime(item.OutputDate);
                            DayOfWeek weekDay = dateItem.DayOfWeek;
                            string strweekDay = weekDay.ToString();
                            switch (strweekDay)
                            {
                                case "Monday":
                                    returnItem.MondayHCActua = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Tuesday":
                                    returnItem.TuesdayHCActua = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Wednesday":
                                    returnItem.WednesdayHCActua = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Thursday":
                                    returnItem.ThursdayHCActua = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Friday":
                                    returnItem.FridayHCActua = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Saturday":
                                    returnItem.SaterdayHCActua = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                                case "Sunday":
                                    returnItem.SundayHCActua = item.PlanHC == null ? 0 : item.PlanHC.Value;
                                    break;
                            }
                        }
                        result.Add(returnItem);
                    }
                }
            }

            if (result.Count > 0)
            {
                return result.FirstOrDefault(o => o.ShiftTimeID == ShiftTimeID);

            }
            else
            {
                return null;
            }

        }
        public GLHCActuaWM SearchPlanDataByLineID(int LineID, int ShiftTimeID, string date, string WeekDay)
        {
            DateTime StartDate = DateTime.Parse(date);
            DateTime EndDate = StartDate.AddDays(7);

            var buildPlans = gL_GoldenStationCTRecordRepository.GetBuildPlans(LineID, StartDate, EndDate);
            var result = new List<GLHCActuaWM>();
            if (buildPlans != null && buildPlans.Count > 0)
            {
                List<int> ShiftTimes = buildPlans.Select(o => o.ShiftTimeID).Distinct().ToList();
                foreach (var itemTime in ShiftTimes)
                {
                    var everybuildPlans = buildPlans.Where(o => o.ShiftTimeID == itemTime).ToList();
                    if (everybuildPlans != null && everybuildPlans.Count > 0)
                    {
                        var returnItem = new GLHCActuaWM();
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.ShiftTimeID = everybuildPlans[0].ShiftTimeID;
                        returnItem.LineID = everybuildPlans[0].LineID;
                        returnItem.CustomerID = everybuildPlans[0].CustomerID;
                        //returnItem.OutputDate = everybuildPlans[0].OutputDate;
                        returnItem.Plant_Organization_UID = everybuildPlans[0].Plant_Organization_UID;
                        returnItem.BG_Organization_UID = everybuildPlans[0].BG_Organization_UID;
                        returnItem.FunPlant_Organization_UID = everybuildPlans[0].FunPlant_Organization_UID;
                        returnItem.CustomerName = everybuildPlans[0].ProjectName;
                        returnItem.LineName = everybuildPlans[0].LineName;
                        returnItem.Plant_Organization = everybuildPlans[0].Plant_Organization;
                        returnItem.BG_Organization = everybuildPlans[0].BG_Organization;
                        returnItem.FunPlant_Organization = everybuildPlans[0].FunPlant_Organization;
                        returnItem.ShiftTime = everybuildPlans[0].ShiftTime;
                        returnItem.WeekDay = WeekDay;
                        foreach (var item in everybuildPlans)
                        {

                            DateTime dateItem = Convert.ToDateTime(item.OutputDate);
                            DayOfWeek weekDay = dateItem.DayOfWeek;
                            string strweekDay = weekDay.ToString();
                            switch (strweekDay)
                            {
                                case "Monday":
                                    returnItem.MondayHCActua = item.PlanOutput;
                                    break;
                                case "Tuesday":
                                    returnItem.TuesdayHCActua = item.PlanOutput;
                                    break;
                                case "Wednesday":
                                    returnItem.WednesdayHCActua = item.PlanOutput;
                                    break;
                                case "Thursday":
                                    returnItem.ThursdayHCActua = item.PlanOutput;
                                    break;
                                case "Friday":
                                    returnItem.FridayHCActua = item.PlanOutput;
                                    break;
                                case "Saturday":
                                    returnItem.SaterdayHCActua = item.PlanOutput;
                                    break;
                                case "Sunday":
                                    returnItem.SundayHCActua = item.PlanOutput;
                                    break;
                            }
                        }
                        result.Add(returnItem);
                    }
                }
            }

            if (result.Count > 0)
            {
                return result.FirstOrDefault(o => o.ShiftTimeID == ShiftTimeID);

            }
            else
            {
                return null;
            }


        }

        public string InserOrUpdateStations(List<GL_StationDTO> listStations)
        {


            return gL_GoldenStationCTRecordRepository.InserOrUpdateStations(listStations);


        }

        public GL_StationDTO GetStationDTOIsOutput(int LineID)
        {

            return goldenLineRepository.GetStationDTOIsOutput(LineID);

        }

        public PagedListModel<OEE_UserStationDTO> QueryOperatorList(int LineID)
        {
            var totalCount = 0;
            var result = new List<OEE_UserStationDTO>();
            result = goldenLineRepository.QueryOperatorList(LineID);
            return new PagedListModel<OEE_UserStationDTO>(totalCount, result);
        }
        public List<OEE_UserStationDTO> DownloadOperatorListExcel(int LineID)
        {
            List<OEE_UserStationDTO> OEE_UserStationDTOs = new List<OEE_UserStationDTO>();
            var LineDTO = gL_GoldenStationCTRecordRepository.GetOneLineDTO(LineID);
            if (LineDTO != null)
            {
                //获取指定专案的所有站             
                var StationDTOs = gL_GoldenStationCTRecordRepository.GetStationDTO(LineDTO.CustomerID);
                //获取已有的用户信息
                var CustomerOEE_UserStationDTOs = goldenLineRepository.GetOperatorList(LineDTO.CustomerID);

                foreach (var StationDTO in StationDTOs)
                {

                    OEE_UserStationDTO oEE_UserStationDTO = new OEE_UserStationDTO();
                    oEE_UserStationDTO.Plant_Organization_UID = StationDTO.Plant_Organization_UID;
                    oEE_UserStationDTO.BG_Organization_UID = StationDTO.Plant_Organization_UID;
                    oEE_UserStationDTO.FunPlant_Organization_UID = StationDTO.Plant_Organization_UID;
                    oEE_UserStationDTO.Project_UID = StationDTO.CustomerID;
                    oEE_UserStationDTO.Project_Name = StationDTO.ProjectName;
                    oEE_UserStationDTO.Line_ID = StationDTO.LineID;
                    oEE_UserStationDTO.Line_Name = StationDTO.LineName;
                    oEE_UserStationDTO.StationID = StationDTO.StationID;
                    oEE_UserStationDTO.Station_Name = StationDTO.StationName;
                    oEE_UserStationDTO.Plant_Organization = StationDTO.Plant_Organization;
                    oEE_UserStationDTO.BG_Organization = StationDTO.BG_Organization;
                    oEE_UserStationDTO.FunPlant_Organization = StationDTO.FunPlant_Organization;
                    //设置已经设置的用户的站的下载模板
                    if (CustomerOEE_UserStationDTOs != null && CustomerOEE_UserStationDTOs.Count > 0)
                    {
                        //已确认 每一个站 只有一个可以管理的用户
                        var CustomerOEE_UserStationDTO = CustomerOEE_UserStationDTOs.FirstOrDefault(o => o.StationID == StationDTO.StationID);

                        if (CustomerOEE_UserStationDTO != null)
                        {
                            oEE_UserStationDTO.OEE_UserStation_UID = CustomerOEE_UserStationDTO.OEE_UserStation_UID;
                            oEE_UserStationDTO.KeyInNG_User_UID = CustomerOEE_UserStationDTO.KeyInNG_User_UID;
                            oEE_UserStationDTO.User_NTID = CustomerOEE_UserStationDTO.User_NTID;
                        }
                    }

                    OEE_UserStationDTOs.Add(oEE_UserStationDTO);
                }

            }
            return OEE_UserStationDTOs;

        }

        public List<GL_StationDTO> GetStationDTO(int CustomerID)
        {

            return gL_GoldenStationCTRecordRepository.GetStationDTO(CustomerID);
        }

        public List<OEE_UserStationDTO> GetOperatorList(int CustomerID)
        {

            return goldenLineRepository.GetOperatorList(CustomerID);
        }

        public List<GL_LineDTO> GetOEELineDTO(int CustomerID)
        {
            return gL_GoldenStationCTRecordRepository.GetOEELineDTO(CustomerID);
         
        }

        public List<GL_LineDTO> GetIPQCLineDTO(int CustomerID)
        {
            return gL_GoldenStationCTRecordRepository.GetIPQCLineDTO(CustomerID);
        }
        
        public List<SystemUserOEEDTO> GetAllUserByDTOs(int Plant_OrganizationUID)
        {

            return goldenLineRepository.GetAllUserByDTOs(Plant_OrganizationUID);
        }
        public string ImportOperatorList(List<OEE_UserStationDTO> OperatorLists)
        {

            return goldenLineRepository.ImportOperatorList(OperatorLists);

        }
    }
}
