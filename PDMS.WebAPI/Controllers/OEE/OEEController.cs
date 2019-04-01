using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PDMS.Model;

namespace PDMS.WebAPI.Controllers
{
    public class OEEController : ApiControllerBase
    {
        IOEE_Service _OEE_Service;
        IEquipmentmaintenanceService equipmentmaintenanceService;
        public OEEController(IOEE_Service OEE_Service,
            IEquipmentmaintenanceService equipmentmaintenanceService
          )
        {
            this._OEE_Service = OEE_Service;
            this.equipmentmaintenanceService = equipmentmaintenanceService;
        }

        #region OEE站点基本资料维护

        [HttpPost]
        public IHttpActionResult QueryOEE_MachineInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _OEE_Service.QueryOEE_MachineInfo(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOEE_MachineInfoByUIDAPI(string uid)
        {
            var result = _OEE_Service.GetOEE_MachineInfoByUID(int.Parse(uid));
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOEE_MachineInfoByEMTAPI(int PlantUID, string EMT)
        {
            var result = equipmentmaintenanceService.GetOEE_MachineInfoByEMT(PlantUID, EMT);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAllByStationIDAPI(string stationUID)
        {
            var result = _OEE_Service.GetAllByStationID(int.Parse(stationUID));
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAllByEQPIDAPI(string eqpuid)
        {
            var result = _OEE_Service.GetAllByEQPID(int.Parse(eqpuid));
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ExportOEE_MachineByQueryAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(jsonData);
            var result = _OEE_Service.ExportOEE_Machine(searchModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult ExportOEE_MachineAPI(string uids)
        {
            var result = _OEE_Service.ExportOEE_Machine(uids);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult AddOEE_MachineAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(jsonData);
            var result = _OEE_Service.AddOEE_Machine(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult DeleteOEE_MachineAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(jsonData);
            var result = _OEE_Service.DeleteOEE_Machine(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ImportOEE_MachineAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<List<OEE_MachineInfoDTO>>(jsonData);
            var result = _OEE_Service.ImportOEE_Machine(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateOEE_MachineAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(jsonData);
            var result = _OEE_Service.UpdateOEE_Machine(searchModel);
            return Ok(result);
        }

        #endregion OEE站点基本资料维护
        #region OEE DownTimeCode
        [HttpGet]
        public IHttpActionResult GetStationDTOAPI(int LineId)
        {
            var result = _OEE_Service.GetStationDTOs(LineId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOEELineDTOAPI(int CustomerID)
        {
            var result = _OEE_Service.GetOEELineDTO(CustomerID);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult QueryOEE_DownTimeCodesAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<OEE_DownTimeCodeDTO>(jsondata);
            var result = _OEE_Service.QueryOEE_DownTimeCodes(searchModel, page);
            return Ok(result);
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetEnumerationDTOAPI(string Enum_Type, string Enum_Name)
        {
            var dto = _OEE_Service.GetEnumerationDTO(Enum_Type, Enum_Name);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult GetSystemProjectDTOAPI(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var dto = _OEE_Service.GetSystemProjectDTO(Plant_Organization_UID, BG_Organization_UID);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult GetOEE_DownTypeDTOAPI(int Plant_Organization_UID)
        {
            var dto = _OEE_Service.GetOEE_DownTypeDTO(Plant_Organization_UID);
            return Ok(dto);
        }
        [HttpPost]
        public string AddOrEditOEE_DownTimeCodeAPI(OEE_DownTimeCodeDTO dto, bool isEdit)
        {
            var result = _OEE_Service.AddOrEditOEE_DownTimeCode(dto, isEdit);
            return result;
        }
        [HttpGet]
        public IHttpActionResult QueryDownTimeCodeByUidAPI(int OEE_DownTimeCode_UID)
        {
            var result = _OEE_Service.QueryDownTimeCodeByUid(OEE_DownTimeCode_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetDownTimeCodeDTOListAPI(string uids)
        {
            var dto = Ok(_OEE_Service.GetDownTimeCodeDTOList(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult QueryDownTimeCodeListAPI(OEE_DownTimeCodeDTO search)
        {
            var result = _OEE_Service.QueryDownTimeCodeList(search);
            return Ok(result);
        }
        [HttpGet]
        public string DeleteDownTimeCodeAPI(int OEE_DownTimeCode_UID, int userid)
        {
            return _OEE_Service.DeleteDownTimeCode(OEE_DownTimeCode_UID, userid);
        }
        [HttpGet]
        public IHttpActionResult GetAllGL_LineDTOListAPI()
        {
            var result = _OEE_Service.GetAllGL_LineDTOList();
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetAllGL_StationDTOListAPI()
        {
            var result = _OEE_Service.GetAllGL_StationDTOList();
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetAllOEE_DownTimeCodeDTOListAPI()
        {
            var result = _OEE_Service.GetAllOEE_DownTimeCodeDTOList();
            return Ok(result);
        }
        [HttpPost]
        public string ImportOEE_DownTimeCodekExcelAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<OEE_DownTimeCodeDTO>>(jsondata);
            return _OEE_Service.ImportOEE_DownTimeCodekExcel(list);
        }

        #endregion OEE DownTimeCode
        #region OEE StationDefectCode

        [HttpPost]
        public IHttpActionResult QueryOEE_StationDefectCodeAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<OEE_StationDefectCodeDTO>(jsondata);
            var result = _OEE_Service.QueryOEE_StationDefectCode(searchModel, page);
            return Ok(result);
        }
        [HttpPost]
        public string AddOEE_StationDefectCodeAPI(OEE_StationDefectCodeDTO dto, bool isEdit)
        {
            var result = _OEE_Service.AddOrEditOEE_StationDefectCode(dto, isEdit);
            return result;
        }
        [HttpGet]
        public IHttpActionResult QueryOEE_StationDefectCodeByUidAPI(int OEE_StationDefectCode_UID)
        {
            var result = _OEE_Service.QueryOEE_StationDefectCodeByUid(OEE_StationDefectCode_UID);
            return Ok(result);
        }

     

        [HttpGet]
        public IHttpActionResult GetAllOEE_StationDefectCodeDTOListAPI()
        {
            var result = _OEE_Service.GetAllOEE_StationDefectCodeDTOList();
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult QueryOEE_StationDefectCodeListAPI(OEE_StationDefectCodeDTO search)
        {
            var result = _OEE_Service.QueryOEE_StationDefectCodeList(search);
            return Ok(result);
        }
        [HttpPost]
        public string ImportOEE_StationDefectCodekExcelAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<OEE_StationDefectCodeDTO>>(jsondata);
            return _OEE_Service.ImportOEE_StationDefectCodekExcel(list);
        }
        [HttpGet]
        public IHttpActionResult GetOEE_StationDefectCodeDTOListAPI(string uids)
        {
            var dto = Ok(_OEE_Service.GetOEE_StationDefectCodeDTOList(uids));
            return dto;
        }
        [HttpGet]
        public string DeleteOEE_StationDefectCodeAPI(int OEE_StationDefectCode_UID, int userid)
        {
            return _OEE_Service.DeleteOEE_StationDefectCode(OEE_StationDefectCode_UID, userid);
        }
        #endregion OEE DownTimeCode
        #region 工站基本资料维护

        [HttpPost]
        public IHttpActionResult OEE_UserStationAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _OEE_Service.QueryOEE_UserStation(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult QueryStationDefectCodeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_StationDefectCodeDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _OEE_Service.QueryStationDefectCode(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteDefectAPI(int dailynnum_uid)
        {
            var result = _OEE_Service.DeleteDefect(dailynnum_uid);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetTimeModelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(jsonData);
            var result = _OEE_Service.GetTimeModel(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SaveDefectCodeDailyNumAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<List<OEE_DefectCodeDailyNumDTO>>(jsonData);
            var result = _OEE_Service.SaveDefectCodeDailyNum(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetDefectCodeIIDAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_StationDefectCodeDTO>(jsonData);
            var result = _OEE_Service.GetDefectCodeUID(saveModel);
            return Ok(result);
        }


        #endregion
        #region OEE报表

        [HttpPost]
        public IHttpActionResult QueryOEE_EveryDayMachineAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var saveModel = JsonConvert.DeserializeObject<OEE_EveryDayMachineDTO>(jsonData);
            var result = _OEE_Service.QueryOEE_EveryDayMachine(saveModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetLastUpdateTimeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetLastUpdateTime(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetStationLastUpdateTimeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetStationLastUpdateTime(saveModel);
            return Ok(result);
        }

    
        [HttpPost]
        public IHttpActionResult GetRealLastUpdateTimeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetRealLastUpdateTime(saveModel);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult GetLineLastUpdateTimeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetLineLastUpdateTime(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetMachineIndexNameAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetMachineIndexName(saveModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetMachineBreakDownAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetMachineBreakDown(saveModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetBreckDownDetialAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetDownTimeDetials(saveModel, page);
            return Ok(result);
        }


        [HttpPost]
        public IHttpActionResult GETOEEMetricsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetOEEMetricsList(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetButemplateDataListAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetButemplateDataList(saveModel);
            return Ok(result);
        }


        [HttpPost]
        public IHttpActionResult GetAllStationMachineStatusListAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetAllStationMachineStatusList(saveModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetShiftModelAPI(int bg_uid, int Plant_uid)
        {
            var result = _OEE_Service.GetShiftModel(bg_uid, Plant_uid);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFirstYieldAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetFirstYieldList(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetDowntimeBreakdownAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetDowntimeBreakdownList(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetMachinePieReportDataAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetMachinePieReportData(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetLineStaticDataAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetLineStaticData(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ExportAbnormalDTCodeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetAbnormalDTCode(saveModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ExportAbnormalDFCodeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.GetAbnormalDFCode(saveModel);
            return Ok(result);
        }

        #endregion
        #region  停机大类
        [HttpPost]
        public IHttpActionResult QueryOEE_DownTypeAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<OEE_DownTypeDTO>(jsondata);
            var result = _OEE_Service.QueryOEE_DownType(searchModel, page);
            return Ok(result);
        }


        [HttpPost]

        public string AddOrEditOEE_DownTypeAPI(OEE_DownTypeDTO dto, bool isEdit)
        {
            var result = _OEE_Service.AddOrEditOEE_DownType(dto, isEdit);
            return result;
        }

        [HttpGet]
        public IHttpActionResult QueryDownTypeByUidAPI(int OEE_DownTimeType_UID)
        {
            var result = _OEE_Service.QueryDownTypeByUid(OEE_DownTimeType_UID);
            return Ok(result);
        }

        [HttpGet]
        public string DeleteDownTypeAPI(int OEE_DownTimeType_UID, int userid)
        {
            return _OEE_Service.DeleteDownType(OEE_DownTimeType_UID, userid);
        }
        #endregion

        #region 4Q Report
        [HttpPost]
        public IHttpActionResult GetFourQDTTimeAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEEFourQParamModel>(jsondata);
            var result = _OEE_Service.GetFourQDTTime(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFourQDTTypeDetailAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEEFourQParamModel>(jsondata);
            var result = _OEE_Service.GetFourQDTTypeDetail(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetPaynterChartDetialAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEEFourQParamModel>(jsondata);
            var result = _OEE_Service.GetPaynterChartDetial(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFourQActionInfoAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEEFourQParamModel>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = _OEE_Service.QueryActionInfoByCreateDate(searchModel, page);
            return Ok(result);
        }

        #endregion


        #region 4Q BaseMaintain

        /// <summary>
        /// Add 会议记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult QueryOEE_MeetingTypeInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MeetingTypeInfoDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _OEE_Service.QueryOEE_MeetingTypeInfo(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateOEE_MeetingTypeInfoAPI(OEE_MeetingTypeInfoDTO addModel)
        {
            var result = _OEE_Service.UpdateOEE_MeetingTypeInfo(addModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult AddOEE_MeetingTypeInfoAPI(OEE_MeetingTypeInfoDTO updateModel)
        {
            var result = _OEE_Service.AddOEE_MeetingTypeInfo(updateModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteOEE_MeetingTypeInfoAPI(int meetingTypeInfo_UID)
        {
            var result = _OEE_Service.DeleteOEE_MeetingTypeInfo(meetingTypeInfo_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOEE_MeetingTypeInfoByIdAPI(string uid)
        {
            var result = _OEE_Service.GetOEE_MeetingTypeInfoById(int.Parse(uid));
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetMeetingTypeNameAPI(int plantUid, int bgUid, int funplantUid)
        {
            var result = _OEE_Service.GetMeetingTypeName(plantUid, bgUid, funplantUid);
            return Ok(result);
        }

        #region 4Q Metric Base Maintain
        /// <summary>
        /// Add 会议记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult QueryMetricInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<OEE_MetricInfoDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _OEE_Service.QueryMetricInfo(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateMetricInfoAPI(OEE_MetricInfoDTO addModel)
        {
            var result = _OEE_Service.UpdateMetricInfo(addModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult AddMetricInfoInfoAPI(OEE_MetricInfoDTO updateModel)
        {
            var result = _OEE_Service.AddMetricInfoInfo(updateModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteMetricInfoAPI(int metricInfo_Uid)
        {
            var result = _OEE_Service.DeleteMetricInfo(metricInfo_Uid);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetMetricInfoByIdAPI(string uid)
        {
            var result = _OEE_Service.GetMetricInfoById(int.Parse(uid));
            return Ok(result);
        }



        [HttpPost]

        public string AddOrEditMetricInfoAPI(OEE_MetricInfoDTO dto, bool isEdit)
        {
            var result = _OEE_Service.AddOrEditMetricInfo(dto, isEdit);
            return result;
        }

        [HttpGet]
        public IHttpActionResult GetMetricNameAPI(int plantUid, int bgUid, int funplantUid)
        {
            var result = _OEE_Service.GetMetricName(plantUid, bgUid, funplantUid);
            return Ok(result);
        }

        [HttpPost]
        public string AddImprovementPlan(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<OEE_ImprovementPlanDTO>(jsonData);
            var result = _OEE_Service.AddImprovementPlan(item);
            return result;
        }

        [HttpPost]
        public string UpdateImprovementPlanAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<OEE_ImprovementPlanDTO>(jsonData);
            var result = _OEE_Service.UpdateImprovementPlan(item);
            return result;
        }

        [HttpPost]
        public IHttpActionResult GetImprovementPlanAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<OEE_ImprovementPlanDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _OEE_Service.QueryImprovementPlanInfo(item, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetImprovementPlanByIdAPI(int improvementId)
        {
            var result = _OEE_Service.GetImprovementPlanById(improvementId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteImpeovementPlanByIdAPI(int improvementId)
        {
            var result = _OEE_Service.DeleteImpeovementPlanById(improvementId);
            return Ok(result);
        }

        #endregion
        #endregion

        [HttpPost]
        public IHttpActionResult QueryRealStatusReportAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var saveModel = JsonConvert.DeserializeObject<OEE_ReprortSearchModel>(jsonData);
            var result = _OEE_Service.QueryRealStatusReport(saveModel);
            if (result == null) return null;
            return Ok(result);
        }

        /// <summary>
        ///  返回服务器的当前时间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCurrentDateTimeAPI()
        {
            var Date = DateTime.Now.ToString("yyyy-MM-dd");
            return Ok(Date);
        }

    }
}