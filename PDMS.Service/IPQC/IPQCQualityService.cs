using PDMS.Common;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IIPQCQualityService
    {
        List<GL_StationDTO> GetStationDTOs(int LineId);
        TimeModel GetCurrentTimeInterval(TimeIntervalParamModel ParamModel);
        PagedListModel<IPQCQualityResultVM> GetIPQCQualityReport(IPQCQualityReportVM model, Page page);
        PagedListModel<IPQCQualityDetialDto> GetQualityTopDetial(IPQCQualityReportVM model);
        List<IPQCQualityMonthVM> GetQualityMonthReport(IPQCQualityReportVM model);
        List<GL_QADetectionPointDTO> GetGLQADetectionPoint(GLQAReportVM model, Page page);
        string AddOrEditGL_QADetectionPoint(GL_QADetectionPointDTO dto);
        GL_QADetectionPointDTO GetGLQADetectionPointByID(int QADetectionPointID);
        GL_QADetectionPointDTO GetStationsDetectionPointByID(int StationID);
        bool RemoveGLQADetectionPointByID(int QADetectionPointID);
        List<GL_QADetectionPointDTO> GetQADetectionPointDTOList(int LineID);
        List<GL_QADetectionPointDTO> GetGL_QADetectionPointDTO(int LineID);
        string InserOrUpdateDetectionPoints(List<GL_QADetectionPointDTO> list);
        PagedListModel<GL_QATargetYieldDTO> QueryGLQAYields(GL_QATargetYieldDTO searchModel, Page page);

        string AddOrEditGLQAYield(GL_QATargetYieldDTO dto);
        GL_QATargetYieldDTO QueryGLQAYieldByUID(int GLQATargetYieldID);
        List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOList(int StationID);
        List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOs(int StationID);
        string InserOrUpdateTargetYields(List<GL_QATargetYieldDTO> list);
        string DeleteGLQAYield(int GLQATargetYieldID);
        List<GL_ShiftTimeDTO> GetShiftTimeDTO(int Plant_Organization_UID, int BG_Organization_UID);
        List<GL_QADetectionPointDTO> GetAllGLQADetectionPointDTO();
        List<GL_IPQCQualityDetialDTO> GetAllGL_IPQCQualityDetialDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval);
        GL_IPQCQualityReportDTO GetGLIPQCQualityReportDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval);
        List<GL_QATargetYieldDTO> GetGLQATargetYieldDTOList(int StationID, string TargetYieldDate);
        string InserOrUpdateIPQCReports(List<GL_IPQCQualityDetialDTO> gL_IPQCQualityDetialDTOs, List<GL_IPQCQualityReportDTO> gL_IPQCQualityReportDTOs);
        List<GL_WIPHourOutputDTO> GetGL_WIPHourOutputDTOs(int StationID, string OutputDate, List<int> HourIndexs, int ShiftTimeID);
        string InserOrUpdateIPQCWIP(int StationID, int WIP);

    }

    public class IPQCQualityService : IIPQCQualityService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IIPQCQUalityReportRepository IPQCQUalityReportRepository;
        private readonly IGL_QADetectionPointRepository gL_QADetectionPointRepository;
        private readonly IGL_ShiftTimeRepository GL_ShiftTimeRepository;
        private readonly IGL_StationRepository _stationRepository;
        private readonly IIPQCQualityDetialRepository _IPQCQualityDetialRepository;
        private readonly IGL_QATargetYieldRepository gL_QATargetYieldRepository;
        private readonly IGL_GoldenStationCTRecordRepository gL_GoldenStationCTRecordRepository;
        private readonly IGL_WIPHourOutputRepository gL_WIPHourOutputRepository;
        public IPQCQualityService(
            IUnitOfWork unitOfWork,
            IIPQCQUalityReportRepository IPQCQUalityReportRepository,
            IGL_QADetectionPointRepository gL_QADetectionPointRepository,
            IGL_ShiftTimeRepository GL_ShiftTimeRepository,
            IGL_StationRepository _stationRepository,
            IGL_QATargetYieldRepository gL_QATargetYieldRepository,
            IIPQCQualityDetialRepository IPQCQualityDetialRepository,
            IGL_GoldenStationCTRecordRepository gL_GoldenStationCTRecordRepository,
            IGL_WIPHourOutputRepository gL_WIPHourOutputRepository
            )
        {
            this.unitOfWork = unitOfWork;
            this.IPQCQUalityReportRepository = IPQCQUalityReportRepository;
            this.gL_QADetectionPointRepository = gL_QADetectionPointRepository;
            this.GL_ShiftTimeRepository = GL_ShiftTimeRepository;
            this._stationRepository = _stationRepository;
            this.gL_QATargetYieldRepository = gL_QATargetYieldRepository;
            this._IPQCQualityDetialRepository = IPQCQualityDetialRepository;
            this.gL_GoldenStationCTRecordRepository = gL_GoldenStationCTRecordRepository;
            this.gL_WIPHourOutputRepository = gL_WIPHourOutputRepository;

        }
        public List<GL_StationDTO> GetStationDTOs(int LineId)
        {
            var result = IPQCQUalityReportRepository.GetStationDTOs(LineId);
            result = NaturalStationSort(result).ToList();
            return result;
        }


        /// <summary>
        /// 判断当前时间数据那个时段-(以后可供所有模组公用)
        /// </summary>
        /// <param name="ParamModel"></param>
        /// <returns></returns>
        public TimeModel GetCurrentTimeInterval(TimeIntervalParamModel ParamModel)
        {
            var currentDate1 = ParamModel.CurrentTime;
            var currentDayTemp1 = currentDate1.ToShortTimeString();
            var currentDay = currentDate1.ToString("yyyy-MM-dd");
            var currentTime = currentDate1.ToString("yyyy-MM-dd HH:mm");
            var currentDate = Convert.ToDateTime(currentTime);
            var allShiftModel = GL_ShiftTimeRepository.GetMany(p => p.BG_Organization_UID == ParamModel.BG_Organization_UID && p.Plant_Organization_UID == ParamModel.Plant_Organization_UID && p.IsEnabled == true).ToList();
            allShiftModel = allShiftModel.OrderBy(p => p.Sequence).ToList();
            var shiftModel = new ShiftModel();
            #region  判断当前时间是那个班次
            foreach (var item in allShiftModel)
            {
                var currentDayFlag = false;
                var StartTime = Convert.ToDateTime(currentDay + " " + item.StartTime);
                var End_Time = Convert.ToDateTime(currentDay + " " + item.End_Time);
                if (DateTime.Compare(DateTime.Parse(currentDayTemp1), DateTime.Parse(item.StartTime)) < 0 && DateTime.Compare(DateTime.Parse(item.StartTime), DateTime.Parse(item.End_Time)) > 0)
                {
                    currentDayFlag = true;
                    currentDay = currentDate1.AddDays(-1).ToString("yyyy-MM-dd");
                    StartTime = Convert.ToDateTime(currentDay + " " + item.StartTime);
                    End_Time = Convert.ToDateTime(currentDate1.ToString("yyyy-MM-dd") + " " + item.End_Time);
                }
                else
                {
                    StartTime = Convert.ToDateTime(currentDay + " " + item.StartTime);
                    End_Time = Convert.ToDateTime(currentDay + " " + item.End_Time);
                    if (StartTime > End_Time)
                    {
                        End_Time = End_Time.AddDays(1);
                    }
                }

                //判断当前班次是属于那个班次
                var flag = StartTime < currentDate && End_Time > currentDate;
                if (flag)
                {
                    shiftModel.Plant_Organization_UID = item.Plant_Organization_UID;
                    shiftModel.BG_Organization_UID = item.BG_Organization_UID;
                    shiftModel.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                    shiftModel.ShiftTimeID = item.ShiftTimeID;
                    shiftModel.Shift = item.Shift;
                    shiftModel.StartTime = item.StartTime;
                    shiftModel.End_Time = item.End_Time;
                    shiftModel.StartDateTime = StartTime;
                    shiftModel.EndDateTime = End_Time;
                    shiftModel.IsEnabled = item.IsEnabled;
                    break;
                }
                else
                {
                    if (currentDayFlag)
                    {
                        currentDay = currentDate1.AddDays(1).ToString("yyyy-MM-dd");
                    }
                }
            }
            #endregion
            #region 判断当前时间是属于那个时段
            if (shiftModel != null)
            {
                var tempStartTime = shiftModel.StartDateTime;
                currentDay = tempStartTime.ToShortDateString();
                var loopCount = 0;
                while (tempStartTime < shiftModel.EndDateTime)
                {
                    loopCount++;
                    if (tempStartTime < currentDate && tempStartTime.AddHours(ParamModel.TimeIntervalLength) > currentDate)
                    {
                        shiftModel.TimeInterval = tempStartTime.ToString("HH:mm") + "-" + tempStartTime.AddHours(ParamModel.TimeIntervalLength).ToString("HH:mm");
                        break;
                    }

                    tempStartTime = tempStartTime.AddHours(ParamModel.TimeIntervalLength);

                    //防止死循环
                    if (loopCount > 24)
                    {
                        break;
                    }
                }
            }

            #endregion

            return new TimeModel
            {
                currentDate = currentDay,
                currentShiftID = shiftModel.ShiftTimeID,
                currentTimeInterval = shiftModel.TimeInterval,
            };
        }

        /// <summary>
        ///  工站下拉选项
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<GL_StationDTO> NaturalStationSort(IEnumerable<GL_StationDTO> list)
        {
            int maxLen = list.Select(s => s.StationName.Length).Max();
            Func<string, char> PaddingChar = s => char.IsDigit(s[0]) ? ' ' : char.MaxValue;
            return list
                    .Select(s =>
                        new
                        {
                            OrgStr = s,
                            SortStr = Regex.Replace(s.StationName, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, PaddingChar(m.Value)))
                        })
                    .OrderBy(x => x.SortStr)
                    .Select(x => x.OrgStr);
        }
        /// <summary>
        /// 获取IPQC的报表
        /// </summary>
        /// <returns></returns>
        public PagedListModel<IPQCQualityResultVM> GetIPQCQualityReport(IPQCQualityReportVM model, Page page)
        {
            var qualityList = IPQCQUalityReportRepository.GetIPQCQualityReport(model, false);
            List<IPQCQualityResultVM> qualityModelList = new List<IPQCQualityResultVM>();
            var timeIntervalModel1 = qualityList.Where(p => p.TimeIntervalIndex == 1);
            var timeIntervalModel2 = qualityList.Where(p => p.TimeIntervalIndex == 2);
            var timeIntervalModel3 = qualityList.Where(p => p.TimeIntervalIndex == 3);
            var timeIntervalModel4 = qualityList.Where(p => p.TimeIntervalIndex == 4);
            var timeIntervalModel5 = qualityList.Where(p => p.TimeIntervalIndex == 5);
            var timeIntervalModel6 = qualityList.Where(p => p.TimeIntervalIndex == 6);
            #region 
            foreach (string colorName in Enum.GetNames(typeof(QualityType)))
            {
                IPQCQualityResultVM qualityModel = new IPQCQualityResultVM();
                switch (Convert.ToInt32(Enum.Parse(typeof(QualityType), colorName)))
                {
                    case 1:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.FirstYield);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.FirstYield).ToString("p2");
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.FirstYield).ToString("p2");
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.FirstYield).ToString("p2");
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.FirstYield).ToString("p2");
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.FirstYield).ToString("p2");
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.FirstYield).ToString("p2");
                        qualityModel.Toatal = qualityList.Sum(p => p.TestNumber) == 0 ? 0.ToString("p2") : ((qualityList.Sum(p => p.FirstPassNumber) * 1.0 / qualityList.Sum(p => p.TestNumber)).ToString("p2"));
                        qualityModelList.Add(qualityModel);
                        break;
                    case 0:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.FirstTargetYield);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.FirstTargetYield).ToString("p2");
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.FirstTargetYield).ToString("p2");
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.FirstTargetYield).ToString("p2");
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.FirstTargetYield).ToString("p2");
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.FirstTargetYield).ToString("p2");
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.FirstTargetYield).ToString("p2");
                        qualityModel.Toatal = timeIntervalModel1.Sum(q => q.FirstTargetYield).ToString("p2");
                        qualityModelList.Add(qualityModel);
                        break;
                    case 4:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.SecondYield);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.SecondYield).ToString("p2");
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.SecondYield).ToString("p2");
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.SecondYield).ToString("p2");
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.SecondYield).ToString("p2");
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.SecondYield).ToString("p2");
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.SecondYield).ToString("p2");
                        qualityModel.Toatal = qualityList.Sum(p => p.SecondPassNumber) == 0 ? 0.ToString("p2") : (qualityList.Sum(p => p.SecondPassNumber) * 1.0 / (qualityList.Sum(p => p.SecondPassNumber) + qualityList.Sum(p => p.NGNumber))).ToString("p2");
                        qualityModelList.Add(qualityModel);
                        break;
                    case 3:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.SecondTargetYield);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.SecondTargetYield).ToString("p2");
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.SecondTargetYield).ToString("p2");
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.SecondTargetYield).ToString("p2");
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.SecondTargetYield).ToString("p2");
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.SecondTargetYield).ToString("p2");
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.SecondTargetYield).ToString("p2");
                        qualityModel.Toatal = timeIntervalModel1.Sum(q => q.SecondTargetYield).ToString("p2");
                        qualityModelList.Add(qualityModel);
                        break;
                    case 5:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.InputNumber);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.InputNumber).ToString();
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.InputNumber).ToString();
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.InputNumber).ToString();
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.InputNumber).ToString();
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.InputNumber).ToString();
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.InputNumber).ToString();
                        qualityModel.Toatal = qualityList.Sum(p => p.InputNumber).ToString();
                        qualityModelList.Add(qualityModel);
                        break;
                    case 6:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.TestNumber);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.TestNumber).ToString();
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.TestNumber).ToString();
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.TestNumber).ToString();
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.TestNumber).ToString();
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.TestNumber).ToString();
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.TestNumber).ToString();
                        qualityModel.Toatal = qualityList.Sum(p => p.TestNumber).ToString();
                        qualityModelList.Add(qualityModel);
                        break;
                    case 7:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.FirstPassNumber);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.FirstPassNumber).ToString();
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.FirstPassNumber).ToString();
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.FirstPassNumber).ToString();
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.FirstPassNumber).ToString();
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.FirstPassNumber).ToString();
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.FirstPassNumber).ToString();
                        qualityModel.Toatal = qualityList.Sum(p => p.FirstPassNumber).ToString();
                        qualityModelList.Add(qualityModel);
                        break;
                    case 8:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.SecondPassNumber);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.SecondPassNumber).ToString();
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.SecondPassNumber).ToString();
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.SecondPassNumber).ToString();
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.SecondPassNumber).ToString();
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.SecondPassNumber).ToString();
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.SecondPassNumber).ToString();
                        qualityModel.Toatal = qualityList.Sum(p => p.SecondPassNumber).ToString();
                        qualityModelList.Add(qualityModel);
                        break;
                    case 9:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.RepairNumber);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.RepairNumber).ToString();
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.RepairNumber).ToString();
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.RepairNumber).ToString();
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.RepairNumber).ToString();
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.RepairNumber).ToString();
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.RepairNumber).ToString();
                        qualityModel.Toatal = qualityList.Sum(p => p.RepairNumber).ToString();
                        qualityModelList.Add(qualityModel);
                        break;
                    case 10:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.NGNumber);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.NGNumber).ToString();
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.NGNumber).ToString();
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.NGNumber).ToString();
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.NGNumber).ToString();
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.NGNumber).ToString();
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.NGNumber).ToString();
                        qualityModel.Toatal = qualityList.Sum(p => p.NGNumber).ToString();
                        qualityModelList.Add(qualityModel);
                        break;
                    case 11:
                        qualityModel.IndexName = EnumHelper.GetDescription(QualityType.WIP);
                        qualityModel.TimeInterval1 = timeIntervalModel1.Sum(q => q.WIP).ToString();
                        qualityModel.TimeInterval2 = timeIntervalModel2.Sum(q => q.WIP).ToString();
                        qualityModel.TimeInterval3 = timeIntervalModel3.Sum(q => q.WIP).ToString();
                        qualityModel.TimeInterval4 = timeIntervalModel4.Sum(q => q.WIP).ToString();
                        qualityModel.TimeInterval5 = timeIntervalModel5.Sum(q => q.WIP).ToString();
                        qualityModel.TimeInterval6 = timeIntervalModel6.Sum(q => q.WIP).ToString();
                        qualityModel.Toatal = qualityList.Sum(p => p.WIP).ToString();
                        qualityModelList.Add(qualityModel);
                        break;
                    default:
                        break;
                }
            }
            #endregion 
            return new PagedListModel<IPQCQualityResultVM>(0, qualityModelList);
        }
        public PagedListModel<IPQCQualityDetialDto> GetQualityTopDetial(IPQCQualityReportVM model)
        {
            var reulltDetial = _IPQCQualityDetialRepository.GetIPQCDetialReport(model);
            ///获取一次良率的Top5
            if (model.VersionNumber == 0)
            {
                reulltDetial = reulltDetial.Where(p => p.NGType == "0").ToList();
            }
            else ///获取二次良率的Top5
            if (model.VersionNumber == 1)
            {
                reulltDetial = reulltDetial.Where(p => p.NGType == "1").ToList();
            }
            reulltDetial = reulltDetial.OrderByDescending(p => p.NGNumber).Take(5).ToList();
            reulltDetial = reulltDetial.OrderBy(p => p.NGNumber).ToList();
            return new PagedListModel<IPQCQualityDetialDto>(0, reulltDetial);
        }

        /// <summary>
        ///  获取月报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<IPQCQualityMonthVM> GetQualityMonthReport(IPQCQualityReportVM model)
        {
            var qualityList = IPQCQUalityReportRepository.GetIPQCQualityReport(model, true);
            var tempModel = qualityList.GroupBy(p => p.ProductDate);
            var allShiftModel = GL_ShiftTimeRepository.GetAll().ToList();
            List<IPQCQualityMonthVM> resultList = new List<IPQCQualityMonthVM>();
            //一次良率
            tempModel = tempModel.OrderBy(p => p.Key);
            if (model.VersionNumber == 0)
            {
                foreach (var item in tempModel)
                {
                    //判断是白班还是夜班
                    var shiftModel = allShiftModel.Where(p => p.ShiftTimeID == item.FirstOrDefault().ShiftID).FirstOrDefault();
                    var currentStartTime = Convert.ToDateTime("2018-01-01" + " " + shiftModel.StartTime);
                    var currentEnd_Time = Convert.ToDateTime("2018-01-01" + " " + shiftModel.End_Time);
                    var currentDay = string.Empty;
                    var FirstYieldStr = item.Sum(p => p.TestNumber) == 0 ? 0.ToString("p2") : ((item.Sum(p => p.FirstPassNumber) * 1.0 / item.Sum(p => p.TestNumber)).ToString("p2"));
                    var FirstYield = item.Sum(p => p.TestNumber) == 0 ? 0.ToString("f2") : ((item.Sum(p => p.FirstPassNumber) * 1.0 / item.Sum(p => p.TestNumber) * 100).ToString("f2"));
                    var FirstTargetYieldStr = item.FirstOrDefault().FirstTargetYield.ToString("p2");
                    var FirstTargetYield = (item.FirstOrDefault().FirstTargetYield * 100).ToString("f2");
                    if (currentStartTime > currentEnd_Time)
                    {
                        //晚班
                        currentDay = item.Key.ToString("MM/dd") + "N" + "\n" + " " + "\n" + FirstYieldStr + "\n" + " " + "\n" + FirstTargetYieldStr;
                    }
                    else
                    {
                        //白班
                        currentDay = item.Key.ToString("MM/dd") + "D" + "\n" + " " + "\n" + FirstYieldStr + "\n" + " " + "\n" + FirstTargetYieldStr;
                    }

                    IPQCQualityMonthVM monthModel = new IPQCQualityMonthVM();
                    monthModel.FirstYield = FirstYield;
                    monthModel.FirstTargetYield = FirstTargetYield;
                    monthModel.ProductDate = currentDay;
                    resultList.Add(monthModel);
                }
            }   //二次良率
            else if (model.VersionNumber == 1)
            {

                foreach (var item in tempModel)
                {
                    //判断是白班还是夜班
                    var shiftModel = allShiftModel.Where(p => p.ShiftTimeID == item.FirstOrDefault().ShiftID).FirstOrDefault();
                    var currentStartTime = Convert.ToDateTime("2018-01-01" + " " + shiftModel.StartTime);
                    var currentEnd_Time = Convert.ToDateTime("2018-01-01" + " " + shiftModel.End_Time);
                    var currentDay = string.Empty;
                    var SecondYieldStr = (item.Sum(p => p.SecondPassNumber) + item.Sum(p => p.NGNumber)) == 0 ? 0.ToString("p2") : (item.Sum(p => p.SecondPassNumber) * 1.0 / (item.Sum(p => p.SecondPassNumber) + item.Sum(p => p.NGNumber))).ToString("p2");
                    var SecondYield = (item.Sum(p => p.SecondPassNumber) + item.Sum(p => p.NGNumber)) == 0 ? 0.ToString("f2") : ((item.Sum(p => p.SecondPassNumber) * 1.0 / (item.Sum(p => p.SecondPassNumber) + item.Sum(p => p.NGNumber))) * 100).ToString("f2");
                    var SecondTargetYieldStr = item.FirstOrDefault().SecondTargetYield.ToString("p2");
                    var SecondTargetYield = (item.FirstOrDefault().SecondTargetYield * 100).ToString("f2");
                    if (currentStartTime > currentEnd_Time)
                    {
                        //晚班
                        currentDay = item.Key.ToString("MM/dd") + "N" + "\n" + " " + "\n" + SecondYieldStr + "\n" + " " + "\n" + SecondTargetYieldStr;
                    }
                    else
                    {
                        //白班
                        currentDay = item.Key.ToString("MM/dd") + "D" + "\n" + " " + "\n" + SecondYieldStr + "\n" + " " + "\n" + SecondTargetYieldStr;
                    }

                    IPQCQualityMonthVM monthModel = new IPQCQualityMonthVM();
                    monthModel.SecondYield = SecondYield;
                    monthModel.SecondTargetYield = SecondTargetYield;
                    monthModel.ProductDate = currentDay;
                    resultList.Add(monthModel);
                }
            }
            return resultList;
        }

        #region QA检测点维护
        /// <summary>
        /// 初始化检测点列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<GL_QADetectionPointDTO> GetGLQADetectionPoint(GLQAReportVM model, Page page)
        {
            return gL_QADetectionPointRepository.GetGLQADetectionPoint(model, page);
        }
        public string AddOrEditGL_QADetectionPoint(GL_QADetectionPointDTO dto)
        {
            string errorMessage = string.Empty;
            try
            {
                GL_QADetectionPoint entityContext;
                if (dto.QADetectionPointID == 0)
                {
                    entityContext = new GL_QADetectionPoint();
                    entityContext.StationID = dto.StationID;
                    entityContext.WIP = dto.WIP;
                    entityContext.ScanIN = dto.ScanIN;
                    entityContext.ScanOUT = dto.ScanOUT;
                    entityContext.ScanNG = dto.ScanNG;
                    entityContext.ScanBACK = dto.ScanBACK;
                    entityContext.IsEnabled = dto.IsEnabled;
                    entityContext.Modified_UID = dto.Modified_UID;
                    entityContext.Modified_Date = dto.Modified_Date;
                    gL_QADetectionPointRepository.Add(entityContext);
                    unitOfWork.Commit();
                }
                else
                {
                    entityContext = gL_QADetectionPointRepository.GetById(dto.QADetectionPointID);
                    entityContext.StationID = dto.StationID;
                    entityContext.WIP = dto.WIP;
                    entityContext.ScanIN = dto.ScanIN;
                    entityContext.ScanOUT = dto.ScanOUT;
                    entityContext.ScanNG = dto.ScanNG;
                    entityContext.ScanBACK = dto.ScanBACK;
                    entityContext.IsEnabled = dto.IsEnabled;
                    entityContext.Modified_UID = dto.Modified_UID;
                    entityContext.Modified_Date = dto.Modified_Date;
                    gL_QADetectionPointRepository.Update(entityContext);
                    unitOfWork.Commit();
                }

                return "0";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }

        }
        public GL_QADetectionPointDTO GetGLQADetectionPointByID(int QADetectionPointID)
        {
            return gL_QADetectionPointRepository.GetGLQADetectionPointByID(QADetectionPointID);
        }

        public GL_QADetectionPointDTO GetStationsDetectionPointByID(int StationID)
        {
            GL_Station Station = _stationRepository.GetById(StationID);
            GL_QADetectionPointDTO qADetectionPointDTO = new GL_QADetectionPointDTO();
            var gGL_QADetectionPointDTO = gL_QADetectionPointRepository.GetStationsDetectionPointByID(StationID);
            if (gGL_QADetectionPointDTO != null)
            {
                qADetectionPointDTO = gGL_QADetectionPointDTO;
            }
            else
            {
                qADetectionPointDTO.StationID = Station.StationID;
                qADetectionPointDTO.StationName = Station.StationName;
                qADetectionPointDTO.MESStationName = Station.MESStationName;
            }
            return qADetectionPointDTO;
        }
        public bool RemoveGLQADetectionPointByID(int QADetectionPointID)
        {
            IQueryable<GL_QADetectionPoint> result = gL_QADetectionPointRepository.GetMany(
         x => x.QADetectionPointID == QADetectionPointID);
            if (result.Count() > 0)
            {

                GL_QADetectionPoint entity = result.First();
                gL_QADetectionPointRepository.Delete(entity);
                unitOfWork.Commit();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 根据LineID找到线下面所有的IPQC工站
        /// </summary>
        /// <param name="LineID"></param>
        /// <returns></returns>
        public List<GL_QADetectionPointDTO> GetQADetectionPointDTOList(int LineID)
        {
            var LineDTO = gL_QADetectionPointRepository.GetOneLineDTO(LineID);


            List<GL_QADetectionPointDTO> gL_QADetectionPointDTOs = new List<GL_QADetectionPointDTO>();
            if (LineDTO != null)
            {

                gL_QADetectionPointDTOs = gL_QADetectionPointRepository.GetQADetectionPointDTOList(LineDTO.CustomerID);
            }

            return gL_QADetectionPointDTOs;
        }

        public List<GL_QADetectionPointDTO> GetGL_QADetectionPointDTO(int LineID)
        {
            var LineDTO = gL_QADetectionPointRepository.GetOneLineDTO(LineID);


            List<GL_QADetectionPointDTO> gL_QADetectionPointDTOs = new List<GL_QADetectionPointDTO>();
            if (LineDTO != null)
            {

                gL_QADetectionPointDTOs = gL_QADetectionPointRepository.GetGL_QADetectionPointDTO(LineDTO.CustomerID);
            }

            return gL_QADetectionPointDTOs;
        }

        public string InserOrUpdateDetectionPoints(List<GL_QADetectionPointDTO> list)
        {
            return gL_QADetectionPointRepository.InserOrUpdateDetectionPoints(list);
        }

        #endregion

        #region QA一次/二次良率维护
        public PagedListModel<GL_QATargetYieldDTO> QueryGLQAYields(GL_QATargetYieldDTO searchModel, Page page)
        {
            int totalcount;
            var result = gL_QATargetYieldRepository.QueryGLQAYields(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<GL_QATargetYieldDTO>(totalcount, result);
            return bd;
        }

        public string AddOrEditGLQAYield(GL_QATargetYieldDTO dto)
        {

            string errorMessage = string.Empty;
            try
            {
                GL_QATargetYield entityContext;
                if (dto.GLQATargetYieldID == 0)
                {
                    //新增要判断是否已存在数据
                    var GLQATargetYieldDTO = gL_QATargetYieldRepository.GetGLQATargetYieldDTO(dto.StationID, dto.Tag, dto.TargetYieldDate);
                    if (GLQATargetYieldDTO != null)
                    {
                        return string.Format(@"已添加此工站日期为{0}的{1}目标良率。", dto.TargetYieldDate, dto.Tag == 1 ? "一次" : "二次");
                    }
                    else
                    {
                        entityContext = new GL_QATargetYield();
                        entityContext.StationID = dto.StationID;
                        entityContext.TargetYieldDate = dto.TargetYieldDate;
                        entityContext.TargetYield = dto.TargetYield;
                        entityContext.Tag = dto.Tag;
                        entityContext.Modified_UID = dto.Modified_UID;
                        entityContext.Modified_Date = dto.Modified_Date;
                        gL_QATargetYieldRepository.Add(entityContext);
                        unitOfWork.Commit();
                    }

                }
                else
                {
                    entityContext = gL_QATargetYieldRepository.GetById(dto.GLQATargetYieldID);
                    entityContext.StationID = dto.StationID;
                    entityContext.TargetYieldDate = dto.TargetYieldDate;
                    entityContext.TargetYield = dto.TargetYield;
                    entityContext.Tag = dto.Tag;
                    entityContext.Modified_UID = dto.Modified_UID;
                    entityContext.Modified_Date = dto.Modified_Date;
                    gL_QATargetYieldRepository.Update(entityContext);
                    unitOfWork.Commit();
                }

                return "0";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }

        }

        public GL_QATargetYieldDTO QueryGLQAYieldByUID(int GLQATargetYieldID)
        {
            return gL_QATargetYieldRepository.QueryGLQAYieldByUID(GLQATargetYieldID);
        }
        public List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOList(int StationID)
        {
            var StationDTO = gL_QATargetYieldRepository.GetOneStationDTO(StationID);

            List<GL_QATargetYieldDTO> gL_QATargetYieldDTOs = new List<GL_QATargetYieldDTO>();
            if (StationDTO != null)
            {
                gL_QATargetYieldDTOs = gL_QATargetYieldRepository.GetQATargetYieldDTOList(StationDTO.CustomerID);
            }

            return gL_QATargetYieldDTOs;
        }

        public List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOs(int StationID)
        {
            var StationDTO = gL_QATargetYieldRepository.GetOneStationDTO(StationID);
            List<GL_QATargetYieldDTO> gL_QATargetYieldDTOs = new List<GL_QATargetYieldDTO>();
            if (StationDTO != null)
            {
                gL_QATargetYieldDTOs = gL_QATargetYieldRepository.GetGL_QATargetYieldDTOs(StationDTO.CustomerID);
            }
            return gL_QATargetYieldDTOs;
        }

        public string InserOrUpdateTargetYields(List<GL_QATargetYieldDTO> list)
        {
            return gL_QATargetYieldRepository.InserOrUpdateTargetYields(list);
        }

        public string DeleteGLQAYield(int GLQATargetYieldID)
        {
            return gL_QATargetYieldRepository.DeleteGLQAYield(GLQATargetYieldID);

        }

        public List<GL_QATargetYieldDTO> GetGLQATargetYieldDTOList(int StationID, string TargetYieldDate)
        {

            return gL_QATargetYieldRepository.GetGLQATargetYieldDTOList(StationID, TargetYieldDate);
        }
        #endregion

        #region IPQC同步相关

        /// <summary>
        ///  获取IPQC所有要同步的站点
        /// </summary>
        /// <returns></returns>
        public List<GL_StationDTO> GetIPQCAllStationDTOs()
        {

            return gL_QATargetYieldRepository.GetIPQCAllStationDTOs();
        }

        public List<GL_ShiftTimeDTO> GetShiftTimeDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            return gL_GoldenStationCTRecordRepository.GetShiftTimeDTO(Plant_Organization_UID, BG_Organization_UID);
        }

        public List<GL_QADetectionPointDTO> GetAllGLQADetectionPointDTO()
        {
            return gL_QADetectionPointRepository.GetAllGLQADetectionPointDTO();
        }

        public List<GL_IPQCQualityDetialDTO> GetAllGL_IPQCQualityDetialDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval)
        {
            return _IPQCQualityDetialRepository.GetAllGL_IPQCQualityDetialDTO(StationID, ShiftID, TimeIntervalIndex, ProductDate, TimeInterval);
        }
        public GL_IPQCQualityReportDTO GetGLIPQCQualityReportDTO(int StationID, int ShiftID, int TimeIntervalIndex, DateTime ProductDate, string TimeInterval)
        {
            return IPQCQUalityReportRepository.GetGLIPQCQualityReportDTO(StationID, ShiftID, TimeIntervalIndex, ProductDate, TimeInterval);
        }

        public string InserOrUpdateIPQCReports(List<GL_IPQCQualityDetialDTO> gL_IPQCQualityDetialDTOs, List<GL_IPQCQualityReportDTO> gL_IPQCQualityReportDTOs)
        {
            return gL_QATargetYieldRepository.InserOrUpdateIPQCReports(gL_IPQCQualityDetialDTOs, gL_IPQCQualityReportDTOs);
        }
        public List<GL_WIPHourOutputDTO> GetGL_WIPHourOutputDTOs(int StationID, string OutputDate, List<int> HourIndexs, int ShiftTimeID)
        {
            return gL_WIPHourOutputRepository.GetGL_WIPHourOutputDTOs(StationID, OutputDate, HourIndexs, ShiftTimeID);
        }
        public string InserOrUpdateIPQCWIP(int StationID, int WIP)
        {
            return gL_QATargetYieldRepository.InserOrUpdateIPQCWIP(StationID, WIP);
        }
        #endregion
    }
}
