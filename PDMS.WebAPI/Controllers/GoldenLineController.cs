using Newtonsoft.Json;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class GoldenLineController : ApiController
    {
        IGL_Service gL_Service;
        IGL_GroupLineService gL_GroupLineService;
        IGoldenLineUIService _goldenLineService;
        IGoldenLineService goldenLineService;
        IGL_GoldenStationCTRecordService gL_GoldenStationCTRecordService;
        IGL_ShiftTimeService shiftTimeService;
        IGL_LineShiftPerfService lineShiftPerf;
        IGL_WIPHourOutputService WIPHourOutputService;
        ISystem_ProjectService projectService;
        public GoldenLineController(IGoldenLineService goldenLineService,
            IGoldenLineUIService _goldenLineService,
            IGL_GoldenStationCTRecordService gL_GoldenStationCTRecordService,
            IGL_ShiftTimeService shiftTimeService,
            IGL_LineShiftPerfService lineShiftPerf,
            IGL_WIPHourOutputService WIPHourOutputService,
            ISystem_ProjectService projectService,
            IGL_GroupLineService gL_GroupLineService,
            IGL_Service gL_Service)
        {
            this.goldenLineService = goldenLineService;
            this._goldenLineService = _goldenLineService;
            this.gL_GoldenStationCTRecordService = gL_GoldenStationCTRecordService;
            this.shiftTimeService = shiftTimeService;
            this.lineShiftPerf = lineShiftPerf;
            this.WIPHourOutputService = WIPHourOutputService;
            this.projectService = projectService;
            this.gL_GroupLineService = gL_GroupLineService;
            this.gL_Service = gL_Service;
        }

        #region 每小时产能
        public IHttpActionResult QueryWIPHourOutputListAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GL_WIPHourOutputDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = WIPHourOutputService.QueryWIPHourOutputList(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetShiftTimeAPI(int ShiftTimeID)
        {
            var result = WIPHourOutputService.GetShiftTime(ShiftTimeID);
            return Ok(result);
        }

        /// <summary>
        /// 导出产能的SN
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IHttpActionResult ExportHourOutPutAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GL_WIPHourOutputDTO>(jsonData);
            var result = WIPHourOutputService.ExportWIPHourOutput(searchModel);
            return Ok(result);
        }

        #endregion
        [HttpPost]
        public IHttpActionResult QueryCustomersAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<System_ProjectModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<System_ProjectModelSearch>>(jsonData);
            var demandDList = projectService.Query(searchModel);
            var demandDDtoList = AutoMapper.Mapper.Map<List<SystemProjectDTO>>(demandDList);
            //排除掉MESProject_Name 为空的数据，因为这些数据没有意义
            demandDDtoList = demandDDtoList.Where(p => p.MESProject_Name != null && p.MESProject_Name != "").ToList();
            return Ok(demandDDtoList);
        }

        [HttpPost]
        public PagedListModel<GL_LineShiftPerfDTO> GetCustomerLinePerfAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<GL_LineShiftPerfModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<GL_LineShiftPerfModelSearch>>(jsonData);
            Page page = JsonConvert.DeserializeObject<Page>(jsonData);
            //由于GoldenLine 和OEE 共用线体，包含IsGoldenLine = true 的工站的线体才是GoldenLine 线体
            var customerLinePerfList = lineShiftPerf.Query(searchModel).Where(f => f.GL_Line.IsEnabled == true && f.GL_Line.GL_Station.Any(s => s.IsGoldenLine)).OrderBy(l => l.GL_Line.Seq);
            var customerLinePerfDtoList = new List<GL_LineShiftPerfDTO>();
            if (customerLinePerfList.Count() > 0)
            {
                var shiftList = shiftTimeService.Query(new QueryModel<GL_ShiftTimeModelSearch>() { Equal = new GL_ShiftTimeModelSearch() { ShiftTimeID = searchModel.Equal.ShiftTimeID } });
                var shift = shiftList.FirstOrDefault();
                float workHours = 10;
                if (shift != null)
                {
                    var dateArr = searchModel.Equal.OutputDate.Split('-');
                    var outputDateYear = int.Parse(dateArr[0]);
                    var outputDateMonth = int.Parse(dateArr[1]);
                    var outputDateDay = int.Parse(dateArr[2]);

                    var timeStartArr = shift.StartTime.Split(':');
                    var timeStartHour = int.Parse(timeStartArr[0]);
                    var timeStartMinite = int.Parse(timeStartArr[1]);

                    var timeEndArr = shift.End_Time.Split(':');
                    var timeEndHour = int.Parse(timeEndArr[0]);
                    var timeEndMinite = int.Parse(timeEndArr[1]);

                    var shiftStartDateTime = new DateTime(outputDateYear, outputDateMonth, outputDateDay, timeStartHour, timeStartMinite, 0);
                    var shiftEndDateTime = new DateTime(outputDateYear, outputDateMonth, outputDateDay, timeEndHour, timeEndMinite, 0);
                    if (shiftStartDateTime > shiftEndDateTime)
                    {
                        shiftEndDateTime = shiftEndDateTime.AddDays(1);
                    }
                    var now = DateTime.Now;
                    if (now > shiftStartDateTime && now < shiftEndDateTime)
                    {
                        var spanHours = (now - shiftStartDateTime).TotalHours;
                        if (spanHours < 3.5)//8:00~11:30
                        {
                            workHours = (int)spanHours;
                        }
                        else if (spanHours < 4.5)//11:30~12:30
                        {
                            workHours = 3.5F;
                        }
                        else if (spanHours < 5.5)
                        {
                            workHours = 4;
                        }
                        else if (spanHours < 6)
                        {
                            workHours = 4.5F;
                        }
                        else if (spanHours < 9.5)
                        {
                            workHours = (int)spanHours - 1;
                        }
                        else if (spanHours < 10.5)
                        {
                            workHours = 8.5F;
                        }
                        else if (spanHours < 11.5)
                        {
                            workHours = 9;
                        }
                        else if (spanHours < 12)
                        {
                            workHours = 9.5F;
                        }
                    }
                }
                
                foreach (var item in customerLinePerfList)
                {
                    var dto = AutoMapper.Mapper.Map<GL_LineShiftPerfDTO>(item);
                    dto.LineName = item.GL_Line.LineName;

                    if (shift != null)
                    {
                        dto.RealTimePlan = (int)(dto.PlanOutput / 10 * workHours);
                        dto.Gap = dto.ActualOutput - dto.RealTimePlan;

                        //找到负责人
                        var resposibleUser = item.GL_Line.GL_LineShiftResposibleUser.FirstOrDefault(x=>x.ShiftTimeID == shift.ShiftTimeID);
                        if (resposibleUser != null)
                        {
                            dto.ResponsibleUserName = resposibleUser.System_Users.User_Name;
                        }
                    }

                    customerLinePerfDtoList.Add(dto);
                }
            }
            
            PagedListModel<GL_LineShiftPerfDTO> pagedData = new PagedListModel<GL_LineShiftPerfDTO>(customerLinePerfDtoList.Count, customerLinePerfDtoList.Skip(page.PageNumber * page.PageSize).Take(page.PageSize));

            return pagedData;
        }

        [HttpPost]
        public IHttpActionResult QueryGetShiftTimesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<GL_ShiftTimeModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<GL_ShiftTimeModelSearch>>(jsonData);
            var demandDList = shiftTimeService.Query(searchModel);
            var demandDDtoList = AutoMapper.Mapper.Map<List<GL_ShiftTimeDTO>>(demandDList);
            return Ok(demandDDtoList);
        }

        [HttpGet]
        public Dictionary<string, int> GetOutputAPI(string MESCustomerName, string MESStationName, DateTime startTime, int Plant_Organization_UID, DateTime? endTime = null)
        {
            var result = goldenLineService.GetOutput(MESCustomerName, MESStationName, startTime, endTime, Plant_Organization_UID);
            return result;
        }
        
        [HttpGet]
        public int GetLineOutputAPI(int lineID, DateTime startTime, DateTime? endTime = null)
        {
            var result = goldenLineService.GetLineOutput(lineID, startTime, endTime);
            return result;
        }

        [HttpGet]
        public decimal GetCustomerActualVSRealTimePlanAPI(int customerID, int shiftTimeID, string outputDate)
        {
            var result = goldenLineService.GetCustomerActualVSRealTimePlan(customerID, shiftTimeID, outputDate);
            return result;
        }

        [HttpGet]
        public decimal GetCustomerVAOLEAPI(int customerID, int shiftTimeID, string outputDate)
        {
            var result = goldenLineService.GetCustomerVAOLE(customerID, shiftTimeID, outputDate);
            return result;
        }

        [HttpGet]
        public string GetStartTimeAPI(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime startTime, DateTime endTime)
        {
            var result = goldenLineService.GetStartTime(MESCustomerName, MESLineName, Plant_Organization_UID, startTime, endTime);
            return result.ToString();
        }

        [HttpGet]
        public string GetRunTimeAPI(string MESCustomerName, string MESLineName, int Plant_Organization_UID, int STDCT, DateTime startTime, DateTime? endTime)
        {
            var result = goldenLineService.GetRunTime(MESCustomerName, MESLineName, Plant_Organization_UID, STDCT, startTime, endTime);
            return result.ToString();
        }

        [HttpGet]
        public string GetUnPlanDownTimeAPI(string MESCustomerName, string MESLineName, int Plant_Organization_UID, int STDCT, DateTime startTime, DateTime? endTime)
        {
            var result = goldenLineService.GetUnPlanDownTime(MESCustomerName, MESLineName, Plant_Organization_UID, STDCT, startTime, endTime);
            return result.ToString();
        }

        [HttpGet]
        public string GetLastUpdateTimeAPI(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime flagTime)
        {
            var result = goldenLineService.GetLastUpdateTime(MESCustomerName, MESLineName, Plant_Organization_UID, flagTime);
            return result.ToString();
        }

        [HttpGet]
        public IHttpActionResult GetStationStdActCTAPI(int customerId, int lineId, DateTime outputDate, int shiftTimeID)
        {
            string outputDate1 = outputDate.ToString("yyyy-MM-dd");
            var result = goldenLineService.GetStationStdActCT(customerId, lineId, outputDate1, shiftTimeID);
            return Ok(result);

        }
        [HttpGet]
        public IHttpActionResult GetCustomerDTOAPI(int BG_Organization_UID)
        {
            var result = gL_GoldenStationCTRecordService.GetCustomerDTOs(BG_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetShiftTimeDTOAPI(int Plant_Organization_UID, int BG_Organization_UID)
        {

            var result = gL_GoldenStationCTRecordService.GetShiftTimeDTO(Plant_Organization_UID, BG_Organization_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetStationDTOAPI(int LineId)
        {

            var result = gL_GoldenStationCTRecordService.GetStationDTOs(LineId);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetONOMESStationDTOsAPI(int LineId)
        {

            var result = gL_GoldenStationCTRecordService.GetONOMESStationDTOs(LineId);
            return Ok(result);
        }
        
        [HttpGet]
        public IHttpActionResult GetStationDTOsByLineIDAPI(int LineID)
        {
            var result = gL_GoldenStationCTRecordService.GetStationDTOsByLineID(LineID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetLineDTOAPI(int CustomerID)
        {

            var result = gL_GoldenStationCTRecordService.GetLineDTOs(CustomerID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetReportLineDTOAPI(int CustomerID)
        {

            var result = gL_GoldenStationCTRecordService.GetReportLineDTOs(CustomerID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetAllLineDTOsAPI(int CustomerID)
        {

            var result = gL_GoldenStationCTRecordService.GetAllLineDTOs(CustomerID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetGroupLineDTOAPI(int CustomerID)
        {

            var result = gL_GoldenStationCTRecordService.GetGroupLineDTOs(CustomerID);
            return Ok(result);
        }

        public IHttpActionResult GetLatestLineStationInfoAPI(int customerId, int lineId, int stationId, DateTime outputDate, int shiftTimeID)
        {
            string outputDate1 = outputDate.ToString("yyyy-MM-dd");
            var result = goldenLineService.GetLatestLineStationInfo(customerId, lineId, stationId, outputDate1, shiftTimeID);


            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetShiftHourOutputAPI(int customerId, int lineId, int stationId, DateTime outputDate, int shiftTimeID)
        {

            string outputDate1 = outputDate.ToString("yyyy-MM-dd");
            var result = goldenLineService.GetShiftHourOutput(customerId, lineId, stationId, outputDate1, shiftTimeID);
            return Ok(result);

        }
        [HttpGet]
        public IHttpActionResult GetActualAndPlanDTOAPI(int customerId, int lineId, int stationId, DateTime outputDate, int shiftTimeID)
        {

            string outputDate1 = outputDate.ToString("yyyy-MM-dd");
            var result = goldenLineService.GetActualAndPlanDTOs(customerId, lineId, stationId, outputDate1, shiftTimeID);
            return Ok(result);

        }


        [HttpGet]
        public IHttpActionResult GetBuildPlanDTOListAPI(int LineID, string week)
        {

            var result = goldenLineService.GetBuildPlanDTOList(LineID, week);
            return Ok(result);

        }

        [HttpGet]
        public IHttpActionResult GetBuildPlanDTOListAllAPI(int LineID, string week)
        {

            var result = goldenLineService.GetBuildPlanDTOListAllAPI(LineID, week);
            return Ok(result);

        }
        [HttpPost]
        public string ImportBuildPlansAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<GLBuildPlanDTO>>(jsondata);
            return goldenLineService.ImportBuildPlans(list);
        }
        [HttpPost]
        public string ImportBuildHCPlansAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<GLBuildPlanDTO>>(jsondata);
            return goldenLineService.ImportBuildHCPlans(list);
        }
        [HttpPost]
        public string ImportBuildHCActuaAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<GLBuildPlanDTO>>(jsondata);
            return goldenLineService.ImportBuildHCActua(list);
        }


        [HttpGet]
        public IHttpActionResult QueryPlanDataAPI(int LineID, string date)
        {

            var result = goldenLineService.QueryPlanData(LineID, date);
            return Ok(result);

        }
        [HttpGet]
        public IHttpActionResult QueryHCPlanDataAPI(int LineID, string date)
        {

            var result = goldenLineService.QueryHCPlanData(LineID, date);
            return Ok(result);

        }
        [HttpGet]
        public IHttpActionResult QueryHCActuaDataAPI(int LineID, string date)
        {

            var result = goldenLineService.QueryHCActuaData(LineID, date);
            return Ok(result);

        }

        [HttpGet]
        public IHttpActionResult QueryHCActuaDataByLineIDAPI(int LineID, int ShiftTimeID, string date, string WeekDay)
        {

            var result = goldenLineService.QueryHCActuaDataByLineID(LineID, ShiftTimeID, date, WeekDay);
            return Ok(result);

        }
        [HttpGet]
        public IHttpActionResult SearchPlanHCDataByLineIDAPI(int LineID, int ShiftTimeID, string date, string WeekDay)
        {

            var result = goldenLineService.SearchPlanHCDataByLineID(LineID, ShiftTimeID, date, WeekDay);
            return Ok(result);

        }
        [HttpGet]
        public IHttpActionResult SearchPlanDataByLineIDAPI(int LineID, int ShiftTimeID, string date, string WeekDay)
        {

            var result = goldenLineService.SearchPlanDataByLineID(LineID, ShiftTimeID, date, WeekDay);
            return Ok(result);

        }


        [HttpPost]
        [IgnoreDBAuthorize]
        public PagedListModel<GL_ShiftTimeDTO> GetShiftTime(dynamic values)
        {
            var jsonData = values.ToString();
            GoldenLineNormalQueryViewModel vm = JsonConvert.DeserializeObject<GoldenLineNormalQueryViewModel>(jsonData);
            Page page = JsonConvert.DeserializeObject<Page>(jsonData);

            return _goldenLineService.GetShiftTimesPaged(vm, page);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public GL_ShiftTimeDTO GetShiftTimeByID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
                return _goldenLineService.GetShiftTimeByID(id);
            else
                return null;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public PagedListModel<GL_RestTimeDTO> GetRestTimeList(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<RestTimeQueryViewModel>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            return _goldenLineService.GetRestTimeList(searchModel, page);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public object RemoveShiftTimeByID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
            {
                bool result = _goldenLineService.RemoveShiftTimeByID(id);
                return new { Result = result };
            }
            else
                return new { Result = false };
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public string AddOrUpdateShiftTime(dynamic values)
        {
            GL_ShiftTimeDTO dto = JsonConvert.DeserializeObject<GL_ShiftTimeDTO>(values.ToString());
            string errorMessage  = _goldenLineService.AddOrUpdateShiftTime(dto, out errorMessage);
            return errorMessage;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IList<SystemProjectDTO> GetCustomer(dynamic values)
        {
            var jsonData = values.ToString();
            GoldenLineNormalQueryViewModel vm = JsonConvert.DeserializeObject<GoldenLineNormalQueryViewModel>(jsonData);

            return _goldenLineService.GetCustomers(vm.BG_Organization_UID);
        }
        [HttpGet]
        public IList<GL_ShiftTimeDTO> GetAllShiftTimesAPI()
        {
            return _goldenLineService.GetAllShiftTimes();
        }
        [HttpGet]
        public IList<GL_RestTimeDTO> GetAllRestTimesAPI()
        {
            return _goldenLineService.GetAllRestTimesAPI();
        }


        [HttpPost]
        [IgnoreDBAuthorize]
        public PagedListModel<GL_LineDTO> GetLine(dynamic values)
        {
            var jsonData = values.ToString();
            GoldenLineNormalQueryViewModel vm = JsonConvert.DeserializeObject<GoldenLineNormalQueryViewModel>(jsonData);
            Page page = JsonConvert.DeserializeObject<Page>(jsonData);

            return _goldenLineService.GetLinePaged(vm, page);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public GL_LineDTO GetLineByID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
                return _goldenLineService.GetLineByID(id);
            else
                return null;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public object RemoveLineByID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
            {
                bool result = _goldenLineService.RemoveLineByID(id);
                return new { Result = result };
            }
            else
                return new { Result = false };
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public GL_LineDTO AddOrUpdateLine(dynamic values)
        {
            GL_LineDTO dto = JsonConvert.DeserializeObject<GL_LineDTO>(values.ToString());
            string errorMessage = string.Empty;
            dto = _goldenLineService.AddOrUpdateLine(dto, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
                return dto;
            else
                return null;
        }
        [HttpGet]
        public IList<GL_LineDTO> GetLinesAllAPI()
        {
            return _goldenLineService.GetLines(0, null, 0, null);
        }
        [HttpPost]
        [IgnoreDBAuthorize]
        public PagedListModel<GL_StationDTO> GetStation(dynamic values)
        {
            var jsonData = values.ToString();
            GoldenLineNormalQueryViewModel vm = JsonConvert.DeserializeObject<GoldenLineNormalQueryViewModel>(jsonData);
            Page page = JsonConvert.DeserializeObject<Page>(jsonData);

            return _goldenLineService.GetStationPaged(vm, page);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public GL_StationDTO GetStationByID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
                return _goldenLineService.GetStationByID(id);
            else
                return null;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public object RemoveStationByID(dynamic value)
        {

            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
            {
                bool result = _goldenLineService.RemoveStationByID(id);
                return new { Result = result };
            }
            else
                return new { Result = false };
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public object RemoveStationByLineID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
            {
                bool result = _goldenLineService.RemoveStationByLineID(id);
                return new { Result = result };
            }
            else
                return new { Result = false };
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public GL_StationDTO AddOrUpdateStation(dynamic values)
        {
            GL_StationDTO dto = JsonConvert.DeserializeObject<GL_StationDTO>(values.ToString());
            string errorMessage = string.Empty;
            dto = _goldenLineService.AddOrUpdateStation(dto, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
                return dto;
            else
                return null;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public PagedListModel<GL_BuildPlanDTO> GetBuildPlan(dynamic values)
        {
            var jsonData = values.ToString();
            GoldenLineNormalQueryViewModel vm = JsonConvert.DeserializeObject<GoldenLineNormalQueryViewModel>(jsonData);
            Page page = JsonConvert.DeserializeObject<Page>(jsonData);

            return _goldenLineService.GetBuildPlanPaged(vm, page);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public GL_BuildPlanDTO AddOrUpdateBuildPlan(dynamic values)
        {
            GL_BuildPlanDTO dto = JsonConvert.DeserializeObject<GL_BuildPlanDTO>(values.ToString());
            string errorMessage = string.Empty;

            GoldenLine_BuildPlanUpdateType updateType = GoldenLine_BuildPlanUpdateType.None;
            switch (dto.UpdateType)
            {
                case 1:
                    updateType = GoldenLine_BuildPlanUpdateType.PlanOutput;
                    break;
                case 2:
                    updateType = GoldenLine_BuildPlanUpdateType.PlanHC;
                    break;
                case 3:
                    updateType = GoldenLine_BuildPlanUpdateType.ActualHC;
                    break;
            }

            dto = _goldenLineService.AddOrUpdateBuildPlan(dto, updateType, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
                return dto;
            else
                return null;
        }
        [HttpPost]
        public string InserOrUpdateStationsAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<GL_StationDTO>>(jsondata);
            return goldenLineService.InserOrUpdateStations(list);
        }
        [HttpPost]

        public string AddOrUpdateGLStationsAPI(GL_StationDTO dto, bool isEdit)
        {
            var result = _goldenLineService.AddOrUpdateGLStations(dto, isEdit);
            return result;
        }

        #region 4Q Report
        [HttpPost]
        public IHttpActionResult GetDownTimeRecordAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GLFourQParamModel>(jsondata);
            var result = gL_Service.GetDownTimeRecord(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFourQDTTypeDetailAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GLFourQParamModel>(jsondata);
            var result = gL_Service.GetFourQDTTypeDetail(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetPaynterChartDetialAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GLFourQParamModel>(jsondata);
            var result = gL_Service.GetPaynterChartDetial(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFourQActionInfoAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GLFourQParamModel>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = gL_Service.QueryActionInfoByCreateDate(searchModel, page);
            return Ok(result);
        }
        #endregion

        #region 4QmeetingTypeInfo
        /// <summary>
        /// Add 会议记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult QueryGL_MeetingTypeInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GL_MeetingTypeInfoDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = gL_Service.QueryGL_MeetingTypeInfo(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateGL_MeetingTypeInfoAPI(GL_MeetingTypeInfoDTO addModel)
        {
            var result = gL_Service.UpdateGL_MeetingTypeInfo(addModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult AddGL_MeetingTypeInfoAPI(GL_MeetingTypeInfoDTO updateModel)
        {
            var result = gL_Service.AddGL_MeetingTypeInfo(updateModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteGL_MeetingTypeInfoAPI(int meetingTypeInfo_UID)
        {
            var result = gL_Service.DeleteGL_MeetingTypeInfo(meetingTypeInfo_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetGL_MeetingTypeInfoByIdAPI(string uid)
        {
            var result = gL_Service.GetGL_MeetingTypeInfoById(int.Parse(uid));
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetMeetingTypeNameAPI(int plantUid, int bgUid, int funplantUid)
        {
            var result = gL_Service.GetMeetingTypeName(plantUid, bgUid, funplantUid);
            return Ok(result);
        }
        #endregion

        #region ActionTask
        [HttpPost]
        public string Add_GL_ActionTaskerAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<GL_ActionTaskDTO>(jsonData);
            var result = gL_Service.Add_GL_ActionTasker(item);
            return result;
        }

        [HttpPost]
        public string Update_GL_ActionTaskerAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<GL_ActionTaskDTO>(jsonData);
            var result = gL_Service.Update_GL_ActionTasker(item);
            return result;
        }

        [HttpPost]
        public IHttpActionResult Get_GL_ActionTaskerAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<GL_ActionTaskDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = gL_Service.QueryActionTaskerInfo(item, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult Get_GL_ActionTaskerByIdAPI(int ActionTasker_ID)
        {
            var result = gL_Service.Get_GL_ActionTaskerById(ActionTasker_ID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult Delete_GL_ActionTaskerByIdAPI(int ActionTasker_ID)
        {
            var result = gL_Service.Delete_GL_ActionTaskerById(ActionTasker_ID);
            return Ok(result);
        }
        
        [HttpGet]
        public IHttpActionResult GetMetricNameAPI(int plantUid, int bgUid, int funplantUid)
        {
            var result = gL_Service.GetMetricName(plantUid, bgUid, funplantUid);
            return Ok(result);
        }
        #endregion

        #region 4Q Metric Base Maintain
        [HttpPost]
        public IHttpActionResult QueryMetricInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GL_MetricInfoDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = gL_Service.QueryMetricInfo(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateMetricInfoAPI(GL_MetricInfoDTO addModel)
        {
            var result = gL_Service.UpdateMetricInfo(addModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult AddMetricInfoInfoAPI(GL_MetricInfoDTO updateModel)
        {
            var result = gL_Service.AddMetricInfoInfo(updateModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteMetricInfoAPI(int metricInfo_Uid)
        {
            var result = gL_Service.DeleteMetricInfo(metricInfo_Uid);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetMetricInfoByIdAPI(string uid)
        {
            var result = gL_Service.GetMetricInfoById(int.Parse(uid));
            return Ok(result);
        }



        [HttpPost]

        public string AddOrEditMetricInfoAPI(GL_MetricInfoDTO dto, bool isEdit)
        {
            var result = gL_Service.AddOrEditMetricInfo(dto, isEdit);
            return result;
        }
        
        #endregion

        #region LineGroup Add By Roy 2018/12/24

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryGroupLinesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<GL_LineModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var groupLines = gL_GroupLineService.QueryGroupLines(searchModel, page);
            return Ok(groupLines);
        }

        [HttpGet]
        public IHttpActionResult QueryGroupLineAPI(int uid)
        {
            var entity = gL_GroupLineService.QueryGroupLine(uid);

            var result = new GL_LineGroupDTO
            {
                LineGroup_UID = entity.LineGroup_UID,
                Plant_Organization_UID = entity.Plant_Organization_UID,
                BG_Organization_UID = entity.BG_Organization_UID,
                FunPlant_Organization_UID = entity.FunPlant_Organization_UID,
                LineID = entity.LineID,
                LineParent_ID = entity.LineParent_ID,
                LineParent_Name = entity.LineParent_Name,
                LineName = entity.LineName,
                CustomerID = entity.CustomerID,
                Project_Name = entity.Project_Name
            };
            return Ok(result);
        }

        public IHttpActionResult GetGroupLineAPI(int? oporgid, int? Optypes, int? opFunPlant,int? customerId)
        {
            var groupLines = gL_GroupLineService.GetGroupLine(oporgid,  Optypes, opFunPlant, customerId);
            return Ok(groupLines);
        }

        public IHttpActionResult GetSubLineAPI(int Oporgid, int Optype)
        {
            var subLines = gL_GroupLineService.GetSubLine(Oporgid, Optype);
            return Ok(subLines);
        }

        /// <summary>
        /// Add function and its sub funtions
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //[AcceptVerbs("Post")]
        public string AddGroupLineAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<GL_LineGroupDTO>(data.ToString());
            return gL_GroupLineService.AddGroupLine(entity);
        }

        public string ModifyGroupLineAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<GL_LineGroupDTO>(data.ToString());
            return gL_GroupLineService.ModifyGroupLine(entity);
        }

        /// <summary>
        /// Delete function
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns>operate result</returns>
        [HttpGet]
        public string RemoveGroupLineAPI(int uid)
        {
            return gL_GroupLineService.RemoveGroupLine(uid);
        }

        public string AddSubToGroupAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<GL_LineGroupDTO>(data.ToString());
            return gL_GroupLineService.AddSubToGroup(entity);
        }
        

        #endregion  LineGroup

        [HttpGet]
        public IHttpActionResult QueryOperatorListAPI(int LineID)
        {

            var result = goldenLineService.QueryOperatorList(LineID);
            return Ok(result);

        }
        [HttpGet]
        public IHttpActionResult DownloadOperatorListExcelAPI(int LineID)
        {
            var result = goldenLineService.DownloadOperatorListExcel(LineID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOEEStationDTOAPI(int CustomerID)
        {
            var result = goldenLineService.GetStationDTO(CustomerID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOperatorListAPI(int CustomerID)
        {
            var result = goldenLineService.GetOperatorList(CustomerID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetOEELineDTOAPI(int CustomerID)
        {
            var result = goldenLineService.GetOEELineDTO(CustomerID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetIPQCLineDTOAPI(int CustomerID)
        {
            var result = goldenLineService.GetIPQCLineDTO(CustomerID);
            return Ok(result);
        }

        
        [HttpGet]
        public IHttpActionResult GetAllUserByDTOsAPI(int Plant_Organization_UID)
        {
            var result = goldenLineService.GetAllUserByDTOs(Plant_Organization_UID);
            return Ok(result);
        }
        [HttpPost]
        public string ImportOperatorListAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<OEE_UserStationDTO>>(jsondata);
            return goldenLineService.ImportOperatorList(list);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public GL_RestTimeDTO GetRestTimeByID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
                return _goldenLineService.GetRestTimeByID(id);
            else
                return null;
        }



        [HttpPost]
        [IgnoreDBAuthorize]
        public object RemoveRestTimeByID(dynamic value)
        {
            object idStr = value["id"];
            int id = 0;
            if (int.TryParse(idStr.ToString(), out id))
            {
                bool result = _goldenLineService.RemoveRestTimeByID(id);
                return new { Result = result };
            }
            else
                return new { Result = false };
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public string AddOrUpdateGLRestTime(dynamic values)
        {
            GL_RestTimeDTO dto = JsonConvert.DeserializeObject<GL_RestTimeDTO>(values.ToString());
            string errorMessage = _goldenLineService.AddOrUpdateRestTime(dto, out errorMessage);
            return errorMessage;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public string UpdateGLRestTime(dynamic values)
        {
            GL_RestTimeDTO dto = JsonConvert.DeserializeObject<GL_RestTimeDTO>(values.ToString());
            string errorMessage = _goldenLineService.UpdateRestTime(dto, out errorMessage);
            return errorMessage;
        }

    }
}
