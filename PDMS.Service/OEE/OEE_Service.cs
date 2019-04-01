using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System.Reflection;
using System.ComponentModel;
using PDMS.Common;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PDMS.Service
{
    public interface IOEE_Service
    {
        PagedListModel<OEE_MachineInfoDTO> QueryOEE_MachineInfo(OEE_MachineInfoDTO serchModel, Page page);
        OEE_MachineInfoDTO GetOEE_MachineInfoByUID(int uid);
        List<OEE_MachineInfoDTO> ExportOEE_Machine(OEE_MachineInfoDTO exportModel);
        List<OEE_MachineInfoDTO> ExportOEE_Machine(string uids);
        List<OEE_MachineInfoDTO> GetAllByStationID(int stationUID);
        string AddOEE_Machine(OEE_MachineInfoDTO addModel);
        string DeleteOEE_Machine(OEE_MachineInfoDTO deleteModel);
        string ImportOEE_Machine(List<OEE_MachineInfoDTO> importModelList);
        string UpdateOEE_Machine(OEE_MachineInfoDTO updateModel);
        PagedListModel<OEE_DownTimeCodeDTO> QueryOEE_DownTimeCodes(OEE_DownTimeCodeDTO searchModel, Page page);
        List<EnumerationDTO> GetEnumerationDTO(string Enum_Type, string Enum_Name);
        List<SystemProjectDTO> GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID);
        List<OEE_DownTypeDTO> GetOEE_DownTypeDTO(int Plant_Organization_UID);
        string AddOrEditOEE_DownTimeCode(OEE_DownTimeCodeDTO dto, bool isEdit);
        OEE_DownTimeCodeDTO QueryDownTimeCodeByUid(int OEE_DownTimeCode_UID);
        List<OEE_DownTimeCodeDTO> QueryDownTimeCodeList(OEE_DownTimeCodeDTO search);
        List<OEE_DownTimeCodeDTO> GetDownTimeCodeDTOList(string uids);
        string DeleteDownTimeCode(int OEE_DownTimeCode_UID, int userid);
        List<GL_LineDTO> GetAllGL_LineDTOList();
        List<GL_StationDTO> GetAllGL_StationDTOList();
        List<OEE_DownTimeCodeDTO> GetAllOEE_DownTimeCodeDTOList();
        string ImportOEE_DownTimeCodekExcel(List<OEE_DownTimeCodeDTO> OEE_DownTimeCodeDTOs);


        #region StationDefectCode
        PagedListModel<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCode(OEE_StationDefectCodeDTO searchModel, Page page);
        string AddOrEditOEE_StationDefectCode(OEE_StationDefectCodeDTO dto, bool isEdit);
        OEE_StationDefectCodeDTO QueryOEE_StationDefectCodeByUid(int OEE_StationDefectCode_UID);
        List<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCodeList(OEE_StationDefectCodeDTO search);
        List<OEE_StationDefectCodeDTO> GetAllOEE_StationDefectCodeDTOList();
        List<OEE_StationDefectCodeDTO> GetOEE_StationDefectCodeDTOList(string uids);
        string DeleteOEE_StationDefectCode(int OEE_DownTimeCode_UID, int userid);
        string ImportOEE_StationDefectCodekExcel(List<OEE_StationDefectCodeDTO> OEE_DownTimeCodeDTOs);
        #endregion

        PagedListModel<OEE_MachineInfoDTO> QueryOEE_UserStation(OEE_MachineInfoDTO serchModel, Page page);

        PagedListModel<OEE_StationDefectCodeDTO> QueryStationDefectCode(OEE_StationDefectCodeDTO serchModel, Page page);

        TimeModel GetTimeModel(OEE_MachineInfoDTO searchModel);
        string SaveDefectCodeDailyNum(List<OEE_DefectCodeDailyNumDTO> saveModelList);

        List<GL_StationDTO> GetStationDTOs(int CustomerID);

        List<GL_LineDTO> GetOEELineDTO(int CustomerID);

        OEE_StationDefectCodeDTO GetDefectCodeUID(OEE_StationDefectCodeDTO model);

        PagedListModel<OEE_EveryDayMachineDTO> QueryOEE_EveryDayMachine(OEE_ReprortSearchModel serchModel, Page page);

        PagedListModel<MachineIndexModel> GetMachineBreakDown(OEE_ReprortSearchModel serchModel, Page page);

        PagedListModel<MachineIndexModel> GetMachineIndexName(OEE_ReprortSearchModel serchModel, Page page);

        List<MachineIndexModel> GetOEEMetricsList(OEE_ReprortSearchModel serchModel);
        List<MachineIndexModel> GetButemplateDataList(OEE_ReprortSearchModel serchModel);
        List<MachineIndexModel> GetFirstYieldList(OEE_ReprortSearchModel serchModel);
        List<MachineIndexModel> GetDowntimeBreakdownList(OEE_ReprortSearchModel serchModel);

        PagedListModel<OEE_DownTypeDTO> QueryOEE_DownType(OEE_DownTypeDTO searchModel, Page page);
        string AddOrEditOEE_DownType(OEE_DownTypeDTO dto, bool isEdit);
        OEE_DownTypeDTO QueryDownTypeByUid(int OEE_DownTimeType_UID);
        string DeleteDownType(int OEE_DownTimeType_UID, int userid);
        OEE_MachineInfoDTO GetAllByEQPID(int eqpuid);
        string GetLastUpdateTime(OEE_ReprortSearchModel serchMode);
        string GetStationLastUpdateTime(OEE_ReprortSearchModel serchMode);
        string GetRealLastUpdateTime(OEE_ReprortSearchModel serchMode);
        string GetLineLastUpdateTime(OEE_ReprortSearchModel serchMode);

        PagedListModel<OEE_MachineDailyDownRecordDTO> GetDownTimeDetials(OEE_ReprortSearchModel serchModel, Page page);
        PagedListModel<OEE_ImprovementPlanDTO> QueryActionInfoByCreateDate(OEEFourQParamModel paramModel, Page page);

        List<MachinePieIndexModel> GetMachinePieReportData(OEE_ReprortSearchModel serchModel);

        List<MachinePieIndexModel> GetLineStaticData(OEE_ReprortSearchModel serchModel);
        List<OEE_AbnormalDFCode> GetAbnormalDTCode(OEE_ReprortSearchModel serchModel);
        List<AbnormalDFCode> GetAbnormalDFCode(OEE_ReprortSearchModel serchModel);
        string DeleteDefect(int dailyNum_UID);
        List<ShiftBaseInfo> GetShiftModel(int bg_uid, int Plant_uid);
        List<OEE_MachineStatus> GetAllStationMachineStatusList(OEE_ReprortSearchModel serchModel);

        /// <summary>
        ///  获取4Q的数据
        /// </summary>
        /// <returns></returns>
        List<OEEFourQDTModel> GetFourQDTTime(OEEFourQParamModel paramModel);

        /// <summary>
        ///  GetFourQDTTypeDetail
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        List<OEEFourQDTModel> GetFourQDTTypeDetail(OEEFourQParamModel paramModel);

        /// <summary>
        ///  获取Payter Chart 的明细信息
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        List<PaynTerChartModel> GetPaynterChartDetial(OEEFourQParamModel paramModel);

        PagedListModel<OEE_MeetingTypeInfoDTO> QueryOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO serchModel, Page page);

        string UpdateOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO updateModel);
        string AddOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO serchModel);

        string DeleteOEE_MeetingTypeInfo(int meetingTypeInfo_UID);

        OEE_MeetingTypeInfoDTO GetOEE_MeetingTypeInfoById(int uid);

        List<OEE_MeetingTypeInfoDTO> GetMeetingTypeName(int plantUid, int bgUid, int funplantUid);

        PagedListModel<OEE_MetricInfoDTO> QueryMetricInfo(OEE_MetricInfoDTO serchModel, Page page);

        string AddMetricInfoInfo(OEE_MetricInfoDTO serchModel);
        string DeleteMetricInfo(int metricInfo_Uid);
        string UpdateMetricInfo(OEE_MetricInfoDTO updateModel);
        OEE_MetricInfoDTO GetMetricInfoById(int uid);

        string AddOrEditMetricInfo(OEE_MetricInfoDTO dto, bool isEdit);
        List<OEE_MetricInfoDTO> GetMetricName(int plantUid, int bgUid, int funplantUid);
        string AddImprovementPlan(OEE_ImprovementPlanDTO dto);

        string UpdateImprovementPlan(OEE_ImprovementPlanDTO item);
        PagedListModel<OEE_ImprovementPlanDTO> QueryImprovementPlanInfo(OEE_ImprovementPlanDTO serchModel, Page page);

        OEE_ImprovementPlanDTO GetImprovementPlanById(int improvementPlanId);
        string DeleteImpeovementPlanById(int improvementPlanId);

        OEE_RealStatusReport QueryRealStatusReport(OEE_ReprortSearchModel serchModel);

        ////定时更新OEE 的改善计划的状态
        void UpdateActionStatus();
    }

    public class OEE_Service : IOEE_Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOEE_MachineInfoRepository _OEE_MachineInfoRepository;
        private readonly IOEE_UserStationRepository _OEE_UserStationRepository;
        private readonly IOEE_DownTimeCodeRepository OEE_DownTimeCodeRepository;
        private readonly IOEE_StationDefectCodeRepository _OEE_StationDefectCodeRepository;
        private readonly IGL_ShiftTimeRepository _GL_ShiftTimeRepository;
        private readonly IOEE_DefectCodeDailyNumDTORepository _IOEE_DefectCodeDailyNumDTORepository;
        private readonly IEnumerationRepository _EnumerationRepository;
        private readonly IOEE_DefectCodeDailySumRepository _OEE_DefectCodeDailySumRepository;
        private readonly IOEE_EveryDayMachineRepository _OEE_EveryDayMachineRepository;
        private readonly IOEE_MachineDailyDownRecordRepository _OEE_MachineDailyDownRecordRepository;
        private readonly IEquipmentInfoRepository equipmentInfoRepository;
        private readonly IOEE_DownTypeRepository OEE_DownTypeRepository;
        private readonly IOEE_EveryDayDFcodeMissingRepository DFcodeMissingRepository;
        private readonly IOEE_EveryDayDFcodeRepository DFCodeRepository;
        private readonly IOEE_MachineStatusRepository MachineStatusRepository;
        private readonly IOEE_MeetingTypeInfoRepository OEE_MeetingTypeInfoRepository;
        private readonly IOEE_MetricInfoRepository OEE_MetricInfoRepository;
        private readonly IOEE_ImprovementPlanDRepository ImprovementPlanDRepository;
        private readonly IOEE_ImprovementPlanRepository ImprovementPlanRepository;

        public OEE_Service(
          IUnitOfWork unitOfWork,
          IOEE_MachineInfoRepository OEE_MachineInfoRepository,
          IOEE_DownTimeCodeRepository OEE_DownTimeCodeRepository,
          IOEE_UserStationRepository OEE_UserStationRepository,
          IOEE_StationDefectCodeRepository OEE_StationDefectCodeRepository,
          IGL_ShiftTimeRepository GL_ShiftTimeRepository,
          IOEE_DefectCodeDailyNumDTORepository OEE_DefectCodeDailyNumDTORepository,
          IEnumerationRepository EnumerationRepository,
          IOEE_DefectCodeDailySumRepository OEE_DefectCodeDailySumRepository,
          IOEE_EveryDayMachineRepository OEE_EveryDayMachineRepository,
          IOEE_MachineDailyDownRecordRepository OEE_MachineDailyDownRecordRepository,
          IOEE_DownTypeRepository OEE_DownTypeRepository,
          IEquipmentInfoRepository equipmentInfoRepository,
          IOEE_EveryDayDFcodeMissingRepository DFcodeMissingRepository,
          IOEE_EveryDayDFcodeRepository DFCodeRepository,
          IOEE_MachineStatusRepository MachineStatusRepository,
          IOEE_MeetingTypeInfoRepository OEE_MeetingTypeInfoRepository,
          IOEE_MetricInfoRepository OEE_MetricInfoRepository,
          IOEE_ImprovementPlanDRepository ImprovementPlanDRepository,
          IOEE_ImprovementPlanRepository ImprovementPlanRepository

         )
        {
            this._unitOfWork = unitOfWork;
            this._OEE_MachineInfoRepository = OEE_MachineInfoRepository;
            this.OEE_DownTimeCodeRepository = OEE_DownTimeCodeRepository;
            this._OEE_UserStationRepository = OEE_UserStationRepository;
            this._OEE_StationDefectCodeRepository = OEE_StationDefectCodeRepository;
            this._GL_ShiftTimeRepository = GL_ShiftTimeRepository;
            this._IOEE_DefectCodeDailyNumDTORepository = OEE_DefectCodeDailyNumDTORepository;
            this._EnumerationRepository = EnumerationRepository;
            this._OEE_DefectCodeDailySumRepository = OEE_DefectCodeDailySumRepository;
            this._OEE_EveryDayMachineRepository = OEE_EveryDayMachineRepository;
            this._OEE_MachineDailyDownRecordRepository = OEE_MachineDailyDownRecordRepository;
            this.OEE_DownTypeRepository = OEE_DownTypeRepository;
            this.equipmentInfoRepository = equipmentInfoRepository;
            this.DFcodeMissingRepository = DFcodeMissingRepository;
            this.DFCodeRepository = DFCodeRepository;
            this.MachineStatusRepository = MachineStatusRepository;
            this.OEE_MeetingTypeInfoRepository = OEE_MeetingTypeInfoRepository;
            this.OEE_MetricInfoRepository = OEE_MetricInfoRepository;
            this.ImprovementPlanDRepository = ImprovementPlanDRepository;
            this.ImprovementPlanRepository = ImprovementPlanRepository;
        }
        #region 机台基本资料维护
        public PagedListModel<OEE_MachineInfoDTO> QueryOEE_MachineInfo(OEE_MachineInfoDTO serchModel, Page page)
        {
            var result = _OEE_MachineInfoRepository.QueryOEE_MachineInfo(serchModel, page);
            return result;
        }

        public OEE_MachineInfoDTO GetOEE_MachineInfoByUID(int uid)
        {
            var result = _OEE_MachineInfoRepository.GetOEE_MachineInfoByUID(uid);
            return result;
        }

        public List<OEE_MachineInfoDTO> GetAllByStationID(int stationUID)
        {
            var result = _OEE_MachineInfoRepository.GetAllByStationID(stationUID);
            if (result.Count == 0) return null;
            else
                result = NaturalMachineNameSort(result).ToList();
            return result;
        }

        public OEE_MachineInfoDTO GetAllByEQPID(int eqpuid)
        {
            var result = _OEE_MachineInfoRepository.GetAllByEQP_UID(eqpuid);
            return result;
        }

        public List<OEE_MachineInfoDTO> ExportOEE_Machine(OEE_MachineInfoDTO exportModel)
        {
            var result = _OEE_MachineInfoRepository.ExportOEE_Machine(exportModel);
            return result;
        }

        public List<OEE_MachineInfoDTO> ExportOEE_Machine(string uids)
        {
            var result = _OEE_MachineInfoRepository.ExportOEE_Machine(uids);
            return result;
        }

        public string AddOEE_Machine(OEE_MachineInfoDTO addModel)
        {
            addModel.EQP_Uid = _OEE_MachineInfoRepository.GetEquipment_UID(addModel);
            if (addModel.EQP_Uid == 0)
            {
                var resultMessage = string.Format("EMT号不存在-{0}，请检查", addModel.EQP_EMTSerialNum);
                return resultMessage;
            }

            //添加判断是否重复
            var is_Exist = _OEE_MachineInfoRepository.GetMany(p => p.Plant_Organization_UID == addModel.Plant_Organization_UID
                                && p.BG_Organization_UID == addModel.BG_Organization_UID
                                && p.FunPlant_Organization_UID == addModel.FunPlant_Organization_UID
                                && p.Project_UID == addModel.Project_UID
                                && p.LineID == addModel.LineID
                                && p.StationID == addModel.StationID
                                && p.EQP_Uid == addModel.EQP_Uid
                                    );
            if (is_Exist.Count() > 0)
            {
                var resultMessage = string.Format("EMT号已存在，请检查", addModel.EQP_EMTSerialNum);
                return resultMessage;
            }

            var result = _OEE_MachineInfoRepository.AddOEE_Machine(addModel);
            return result;
        }

        public string DeleteOEE_Machine(OEE_MachineInfoDTO deleteModel)
        {
            var result = _OEE_MachineInfoRepository.DeleteOEE_Machine(deleteModel);
            return result;
        }

        public string ImportOEE_Machine(List<OEE_MachineInfoDTO> importModelList)
        {
            //1 导入前先判断是否重复
            List<OEE_MachineInfo> list = new List<OEE_MachineInfo>();
            var resultMessage = string.Empty;
            var CurrentItem = importModelList.FirstOrDefault();
            int i = 2;
            if (CurrentItem == null) return "没有数据";
            var ProjectLists = OEE_DownTimeCodeRepository.GetSystemProjectDTO(CurrentItem.Plant_Organization_UID, CurrentItem.BG_Organization_UID);
            var LineLists = OEE_DownTimeCodeRepository.GetAllGL_LineDTOList();
            var StationLists = OEE_DownTimeCodeRepository.GetAllGL_StationDTOList();
            foreach (var item in importModelList)
            {
                i++;
                var machineInfo = new OEE_MachineInfo();
                machineInfo.Plant_Organization_UID = item.Plant_Organization_UID;
                //通过EMT号码和厂区获取机台组织信息。
                var OrgInfo = equipmentInfoRepository.GetOEE_MachineInfoByEMT(item.Plant_Organization_UID, item.EQP_EMTSerialNum);

                if (OrgInfo == null)
                {
                    return "上传失败，第" + i + 1 + "行的EMT号在所填的厂区找不到，请修正";
                }
                #region 数据校验
                //判断专案是否存在

                machineInfo.Project_UID = int.Parse(OrgInfo.Project_UID.ToString());
                machineInfo.BG_Organization_UID = int.Parse(OrgInfo.BG_Organization_UID.ToString());
                machineInfo.FunPlant_Organization_UID = OrgInfo.FunPlant_Organization_UID;
                item.Line_Name = item.Line_Name.Trim();
                item.Station_Name = item.Station_Name.Trim();
                //判断线名称是否存在
                var hasLine = LineLists.Where(m => m.CustomerID == machineInfo.Project_UID && m.LineName == item.Line_Name).FirstOrDefault();
                if (hasLine != null)
                {
                    machineInfo.LineID = hasLine.LineID;
                }
                else
                {
                    resultMessage = string.Format("第{0}行线没有找到", i);
                    return resultMessage;
                }
                //判断工站名称是否存在
                if (string.IsNullOrWhiteSpace(item.Station_Name))
                {
                    resultMessage = string.Format("第{0}行工站不存在", i);
                    return resultMessage;
                }
                var StationStr = item.Station_Name.Trim();
                var hasStation = StationLists.Where(m => m.CustomerID == machineInfo.Project_UID && m.LineID == machineInfo.LineID && m.StationName == StationStr).FirstOrDefault();
                if (hasStation != null)
                {
                    machineInfo.StationID = hasStation.StationID;
                }
                else
                {
                    resultMessage = string.Format("第{0}行工站没有找到", i);
                    return resultMessage;
                }

                //判断EMT号是否存在
                var Equipment_UID = _OEE_MachineInfoRepository.GetEquipment_UID(item);
                if (Equipment_UID != 0)
                {
                    machineInfo.EQP_Uid = Equipment_UID;
                }
                else
                {
                    resultMessage = string.Format("EMT号不存在-{0}，请检查", item.EQP_EMTSerialNum);
                    return resultMessage;
                }
                machineInfo.MachineNo = item.MachineNo;
                machineInfo.Is_Enable = item.Is_Enable;
                machineInfo.Modify_UID = item.Modify_UID;
                machineInfo.Modify_Date = item.Modify_Date;

                //添加判断重复
                var is_Exist = _OEE_MachineInfoRepository.GetMany(p => p.Plant_Organization_UID == machineInfo.Plant_Organization_UID
                                  && p.BG_Organization_UID == machineInfo.BG_Organization_UID
                                  && p.FunPlant_Organization_UID == machineInfo.FunPlant_Organization_UID
                                  && p.Project_UID == machineInfo.Project_UID
                                  && p.LineID == machineInfo.LineID
                                  && p.StationID == machineInfo.StationID
                                  && p.EQP_Uid == machineInfo.EQP_Uid
                                      );
                if (is_Exist.Count() > 0)
                {
                    resultMessage = string.Format("EMT号已存在，请检查", item.EQP_EMTSerialNum);
                    return resultMessage;
                }
                #endregion
                list.Add(machineInfo);
            }

            _OEE_MachineInfoRepository.AddList(list);
            _unitOfWork.Commit();
            return "SUCCESS";
        }

        /// <summary>
        /// 目前只支持修改机台名称和是否启用
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public string UpdateOEE_Machine(OEE_MachineInfoDTO updateModel)
        {
            try
            {
                OEE_MachineInfo entityContext;
                entityContext = _OEE_MachineInfoRepository.GetById(updateModel.OEE_MachineInfo_UID);
                updateModel.FunPlant_Organization_UID = entityContext.FunPlant_Organization_UID;
                updateModel.BG_Organization_UID = entityContext.BG_Organization_UID;
                updateModel.Plant_Organization_UID = entityContext.Plant_Organization_UID;
                updateModel.EQP_EMTSerialNum = updateModel.EQP_EMTSerialNum;
                entityContext.EQP_Uid = _OEE_MachineInfoRepository.GetEquipment_UID(updateModel);
                if (entityContext.EQP_Uid == 0)
                {
                    return string.Format("修改失败，对应专案没有该ETM号-{0}", updateModel.EQP_EMTSerialNum);
                }
                entityContext.Plant_Organization_UID = updateModel.Plant_Organization_UID;
                entityContext.LineID = updateModel.LineID;
                entityContext.StationID = updateModel.StationID;
                entityContext.MachineNo = updateModel.MachineNo;
                entityContext.Is_Enable = updateModel.Is_Enable;
                entityContext.Modify_UID = updateModel.Modify_UID;
                entityContext.Modify_Date = DateTime.Now;
                _OEE_MachineInfoRepository.Update(entityContext);
                _unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "修改失败，将检查数据是否重复";
            }

        }
        #endregion

        #region OEE DownTimeCode

        public PagedListModel<OEE_DownTimeCodeDTO> QueryOEE_DownTimeCodes(OEE_DownTimeCodeDTO searchModel, Page page)
        {
            int totalcount;
            var result = OEE_DownTimeCodeRepository.QueryOEE_DownTimeCodes(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<OEE_DownTimeCodeDTO>(totalcount, result);
            return bd;
        }

        public List<EnumerationDTO> GetEnumerationDTO(string Enum_Type, string Enum_Name)
        {

            var result = OEE_DownTimeCodeRepository.GetEnumerationDTO(Enum_Type, Enum_Name);
            return result;



        }

        public List<SystemProjectDTO> GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var result = OEE_DownTimeCodeRepository.GetSystemProjectDTO(Plant_Organization_UID, BG_Organization_UID);
            return result;
        }
        public List<OEE_DownTypeDTO> GetOEE_DownTypeDTO(int Plant_Organization_UID)
        {
            var result = OEE_DownTimeCodeRepository.GetOEE_DownTypeDTO(Plant_Organization_UID);
            return result;
        }
        public string AddOrEditOEE_DownTimeCode(OEE_DownTimeCodeDTO dto, bool isEdit)
        {

            string errorMessage = string.Empty;
            try
            {
                OEE_DownTimeCode entityContext;
                if (dto.OEE_DownTimeCode_UID == 0)
                {
                    entityContext = new OEE_DownTimeCode();
                    entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                    entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    entityContext.OEE_DownTimeType_UID = dto.OEE_DownTimeType_UID;
                    entityContext.Project_UID = dto.Project_UID;
                    entityContext.LineID = dto.LineID;
                    entityContext.StationID = dto.StationID;
                    entityContext.Error_Code = dto.Error_Code;
                    entityContext.Upload_Ways = dto.Upload_Ways;
                    entityContext.Level_Details = dto.Level_Details;
                    entityContext.Error_Reasons = dto.Error_Reasons;
                    entityContext.Is_Enable = dto.Is_Enable;
                    entityContext.Modify_UID = dto.Modify_UID;
                    entityContext.Modify_Date = dto.Modify_Date;
                    OEE_DownTimeCodeRepository.Add(entityContext);
                    _unitOfWork.Commit();
                }
                else
                {
                    entityContext = OEE_DownTimeCodeRepository.GetById(dto.OEE_DownTimeCode_UID);
                    entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                    entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    entityContext.OEE_DownTimeType_UID = dto.OEE_DownTimeType_UID;
                    entityContext.Project_UID = dto.Project_UID;
                    entityContext.LineID = dto.LineID;
                    entityContext.StationID = dto.StationID;
                    entityContext.Error_Code = dto.Error_Code;
                    entityContext.Upload_Ways = dto.Upload_Ways;
                    entityContext.Level_Details = dto.Level_Details;
                    entityContext.Error_Reasons = dto.Error_Reasons;
                    entityContext.Is_Enable = dto.Is_Enable;
                    entityContext.Modify_UID = dto.Modify_UID;
                    entityContext.Modify_Date = dto.Modify_Date;
                    OEE_DownTimeCodeRepository.Update(entityContext);
                    _unitOfWork.Commit();
                }

                return "0";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }

        }

        public OEE_DownTimeCodeDTO QueryDownTimeCodeByUid(int OEE_DownTimeCode_UID)
        {

            var result = OEE_DownTimeCodeRepository.QueryDownTimeCodeByUid(OEE_DownTimeCode_UID);
            return result;

        }
        public List<OEE_DownTimeCodeDTO> QueryDownTimeCodeList(OEE_DownTimeCodeDTO search)
        {
            return OEE_DownTimeCodeRepository.QueryDownTimeCodeList(search);
        }

        public List<OEE_DownTimeCodeDTO> GetDownTimeCodeDTOList(string uids)
        {
            return OEE_DownTimeCodeRepository.GetDownTimeCodeDTOList(uids);
        }
        public List<GL_StationDTO> GetStationDTOs(int LineId)
        {
            var result = OEE_DownTimeCodeRepository.GetStationDTOs(LineId);
            result = NaturalStationSort(result).ToList();
            return result;
        }

        public List<GL_LineDTO> GetOEELineDTO(int CustomerID)
        {

            var result = OEE_DownTimeCodeRepository.GetOEELineDTO(CustomerID);
            return result.ToList();
        }
        public string DeleteDownTimeCode(int OEE_DownTimeCode_UID, int userid)
        {
            var downTimeCode = OEE_DownTimeCodeRepository.GetFirstOrDefault(w => w.OEE_DownTimeCode_UID == OEE_DownTimeCode_UID);
            if (downTimeCode != null)
            {
                try
                {

                    OEE_DownTimeCodeRepository.Delete(downTimeCode);
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "";

        }

        public List<GL_LineDTO> GetAllGL_LineDTOList()
        {

            return OEE_DownTimeCodeRepository.GetAllGL_LineDTOList();

        }

        public List<GL_StationDTO> GetAllGL_StationDTOList()
        {
            return OEE_DownTimeCodeRepository.GetAllGL_StationDTOList();
        }

        public List<OEE_DownTimeCodeDTO> GetAllOEE_DownTimeCodeDTOList()
        {
            return OEE_DownTimeCodeRepository.GetAllOEE_DownTimeCodeDTOList();
        }

        public string ImportOEE_DownTimeCodekExcel(List<OEE_DownTimeCodeDTO> OEE_DownTimeCodeDTOs)
        {
            return OEE_DownTimeCodeRepository.ImportOEE_DownTimeCodekExcel(OEE_DownTimeCodeDTOs);
        }
        #endregion OEE DownTimeCode

        #region 工站数据维护
        public PagedListModel<OEE_MachineInfoDTO> QueryOEE_UserStation(OEE_MachineInfoDTO serchModel, Page page)
        {
            var result = _OEE_UserStationRepository.QueryOEE_UserStation(serchModel, page);
            var List = new List<OEE_MachineInfoDTO>();
            foreach (var item in result)
            {
                var timeModel = GetTimeModel(item);
                item.TimeInterval = timeModel.currentTimeInterval;
                var userModel = _OEE_UserStationRepository.GetMachineTimeInfo(item);
                if (userModel != null)
                {
                    item.Modify_UID = userModel.Modify_UID;
                    item.Modify_Name = userModel.Modify_Name;
                    item.Modify_Date = userModel.Modify_Date;
                    item.IsSubmit = userModel.num > 0 ? "是" : "否";
                }
                List.Add(item);
            }
            return new PagedListModel<OEE_MachineInfoDTO>(0, List);
        }

        /// <summary>
        /// 工站不良信息
        /// </summary>
        /// <param name="serchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<OEE_StationDefectCodeDTO> QueryStationDefectCode(OEE_StationDefectCodeDTO serchModel, Page page)
        {
            var result = _IOEE_DefectCodeDailyNumDTORepository.QueryOEE_DefectCodeDailyNum(serchModel, page);
            //if (result.Count() > 0)
            //{
            //    return new PagedListModel<OEE_StationDefectCodeDTO>(0, result);
            //}
            //var resultStationDefect = _OEE_StationDefectCodeRepository.QueryStationDefectCode(serchModel, page);
            //return resultStationDefect;

            var resultStationDefect = _OEE_StationDefectCodeRepository.QueryStationDefectCode(serchModel, page);
            foreach (var item in resultStationDefect)
            {
                var tempModel = result.Where(p => p.Defect_Code == item.Defect_Code && p.DefectEnglishName == item.DefectEnglishName && p.DefecChinesetName == item.DefecChinesetName).FirstOrDefault();
                item.OEE_DefectCodeDailyNum_UID = (tempModel == null ? 0 : tempModel.OEE_DefectCodeDailyNum_UID);
                item.DefectNum = (tempModel == null ? 0 : tempModel.DefectNum);
            }

            return new PagedListModel<OEE_StationDefectCodeDTO>(0, resultStationDefect.OrderByDescending(p => p.DefectNum));
        }

        /// <summary>
        /// 判断当前时间属于哪个时段
        /// </summary>
        public TimeModel GetTimeModel(OEE_MachineInfoDTO searchModel)
        {
            var enumModel = _EnumerationRepository.GetMany(p => p.Enum_Type == "OEE_TimeInterval" && p.Enum_Name == "OEE_TimeInterval");
            var time = int.Parse(enumModel.FirstOrDefault().Enum_Value);
            var currentDate1 = DateTime.Now.AddMinutes(-time);
            var currentDayTemp1 = currentDate1.ToShortTimeString();
            var currentDay = currentDate1.ToString("yyyy-MM-dd");
            var currentTime = currentDate1.ToString("yyyy-MM-dd HH:mm");
            var currentDate = Convert.ToDateTime(currentTime);
            ///获取这个厂区下面的所有班次
            var allShiftModel = _GL_ShiftTimeRepository.GetMany(p => p.BG_Organization_UID == searchModel.BG_Organization_UID && p.Plant_Organization_UID == searchModel.Plant_Organization_UID && p.IsEnabled == true).ToList();
            allShiftModel = allShiftModel.OrderBy(p => p.Sequence).ToList();
            //判断当前时间是那个时段
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
                    if (tempStartTime < currentDate && tempStartTime.AddHours(2) > currentDate)
                    {
                        shiftModel.TimeInterval = tempStartTime.ToString("HH:mm") + "-" + tempStartTime.AddHours(2).ToString("HH:mm");
                    }

                    tempStartTime = tempStartTime.AddHours(2);

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
                currentTimeInterval = shiftModel.TimeInterval,
                currentShiftID = shiftModel.ShiftTimeID
            };
        }

        public List<ShiftBaseInfo> GetShiftModel(int bg_uid, int Plant_uid)
        {
            var allShiftModel = _GL_ShiftTimeRepository.GetMany(p => p.BG_Organization_UID == bg_uid && p.Plant_Organization_UID == Plant_uid && p.IsEnabled == true).ToList();
            List<ShiftBaseInfo> shiftList = new List<ShiftBaseInfo>();
            foreach (var item in allShiftModel)
            {
                shiftList.Add(new ShiftBaseInfo
                {
                    ShiftID = item.ShiftTimeID,
                    ShiftName = item.Shift
                });
            }
            return shiftList;
        }
        public string DeleteDefect(int dailyNum_UID)
        {
            var result = _IOEE_DefectCodeDailyNumDTORepository.DeleteDailyNum(dailyNum_UID);
            if (result)
            {
                return "SUCCESS";
            }
            else
            {
                return "FALSE";
            }
        }

        /// <summary>
        /// 添加每日烦人不良
        /// </summary>
        public string SaveDefectCodeDailyNum(List<OEE_DefectCodeDailyNumDTO> saveModelList)
        {
            var list = saveModelList.Where(p => p.DefectNum != 0);
            var dailySum = list.GroupBy(p => p.OEE_StationDefectCode_UID);

            try
            {
                //判断是否重复
                foreach (var item in list)
                {
                    if (item.OEE_DefectCodeDailyNum_UID != 0)
                    {
                        var entity = _IOEE_DefectCodeDailyNumDTORepository.GetById(item.OEE_DefectCodeDailyNum_UID);
                        entity.Plant_Organization_UID = item.Plant_Organization_UID;
                        entity.BG_Organization_UID = item.BG_Organization_UID;
                        entity.OEE_MachineInfo_UID = item.OEE_MachineInfo_UID;
                        entity.OEE_StationDefectCode_UID = item.OEE_StationDefectCode_UID;
                        entity.ProductDate = item.ProductDate;
                        entity.TimeInterval = item.TimeInterval;
                        entity.DefectNum = item.DefectNum;
                        entity.ShiftTimeID = item.ShiftTimeID;
                        entity.Modify_UID = item.Modify_UID;
                        entity.Modify_Date = item.Modify_Date;
                        _IOEE_DefectCodeDailyNumDTORepository.Update(entity);
                        _unitOfWork.Commit();

                    }
                    else
                    {
                        var addEntuty = AutoMapper.Mapper.Map<OEE_DefectCodeDailyNum>(item);
                        _IOEE_DefectCodeDailyNumDTORepository.Add(addEntuty);
                        _unitOfWork.Commit();
                    }
                }

                //daily Sum 一天的统计
                foreach (var item in dailySum)
                {
                    OEE_DefectCodeDailySum dailySumModel = new OEE_DefectCodeDailySum();
                    dailySumModel.Plant_Organization_UID = item.FirstOrDefault().Plant_Organization_UID;
                    dailySumModel.BG_Organization_UID = item.FirstOrDefault().BG_Organization_UID;
                    dailySumModel.FunPlant_Organization_UID = item.FirstOrDefault().FunPlant_Organization_UID;
                    dailySumModel.OEE_MachineInfo_UID = item.FirstOrDefault().OEE_MachineInfo_UID;
                    dailySumModel.OEE_StationDefectCode_UID = item.Key;
                    dailySumModel.DefectNum = item.Sum(p => p.DefectNum);
                    dailySumModel.ProductDate = item.FirstOrDefault().ProductDate;
                    dailySumModel.ShiftTimeID = item.FirstOrDefault().ShiftTimeID;

                    var dailySumDTO = _IOEE_DefectCodeDailyNumDTORepository.GetMany(p =>
                  p.Plant_Organization_UID == dailySumModel.Plant_Organization_UID
                  && p.BG_Organization_UID == dailySumModel.BG_Organization_UID
                  && p.FunPlant_Organization_UID == dailySumModel.FunPlant_Organization_UID
                  && p.OEE_MachineInfo_UID == dailySumModel.OEE_MachineInfo_UID
                  && p.OEE_StationDefectCode_UID == dailySumModel.OEE_StationDefectCode_UID
                  && p.ProductDate == dailySumModel.ProductDate
                  && p.ShiftTimeID == dailySumModel.ShiftTimeID
                  );

                    var entity = _OEE_DefectCodeDailySumRepository.GetMany(p =>
                    p.Plant_Organization_UID == dailySumModel.Plant_Organization_UID
                    && p.BG_Organization_UID == dailySumModel.BG_Organization_UID
                    && p.FunPlant_Organization_UID == dailySumModel.FunPlant_Organization_UID
                    && p.OEE_MachineInfo_UID == dailySumModel.OEE_MachineInfo_UID
                    && p.OEE_StationDefectCode_UID == dailySumModel.OEE_StationDefectCode_UID
                    && p.ProductDate == dailySumModel.ProductDate
                    && p.ShiftTimeID == dailySumModel.ShiftTimeID
                    );

                    if (entity != null && entity.Count() > 0)
                    {
                        entity.First().DefectNum = dailySumDTO.Sum(p => p.DefectNum);
                        _OEE_DefectCodeDailySumRepository.Update(entity.First());
                    }
                    else
                    {
                        _OEE_DefectCodeDailySumRepository.Add(dailySumModel);
                    }

                    _unitOfWork.Commit();
                }

                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "保存失败:" + ex.Message.ToString();
            }
        }

        public OEE_StationDefectCodeDTO GetDefectCodeUID(OEE_StationDefectCodeDTO model)
        {
            try
            {
                var query = _OEE_StationDefectCodeRepository.GetMany(p =>
                                       p.Defect_Code == model.Defect_Code
                                       && p.DefectEnglishName == model.DefectEnglishName
                                       && p.DefecChinesetName == model.DefecChinesetName
                                       && p.Plant_Organization_UID == model.Plant_Organization_UID
                                       && p.BG_Organization_UID == model.BG_Organization_UID
                                       && p.Project_UID == model.Project_UID
                                       && p.LineID == model.LineID
                                       && p.StationID == model.StationID
                                       && p.Is_Enable == true
                           );

                var result = AutoMapper.Mapper.Map<OEE_StationDefectCodeDTO>(query.FirstOrDefault());
                return result;
            }
            catch (Exception ex)
            {
                return new OEE_StationDefectCodeDTO();
            }
        }


        #endregion

        #region 不良明细
        public PagedListModel<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCode(OEE_StationDefectCodeDTO searchModel, Page page)
        {
            int totalcount;
            var result = _OEE_StationDefectCodeRepository.QueryOEE_StationDefectCode(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<OEE_StationDefectCodeDTO>(totalcount, result);
            return bd;
        }

        public string AddOrEditOEE_StationDefectCode(OEE_StationDefectCodeDTO dto, bool isEdit)
        {

            string errorMessage = string.Empty;
            try
            {
                OEE_StationDefectCode entityContext;
                if (dto.OEE_StationDefectCode_UID == 0)
                {
                    entityContext = new OEE_StationDefectCode();
                    entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                    entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    entityContext.Project_UID = dto.Project_UID;
                    entityContext.LineID = dto.LineID;
                    entityContext.Sequence = dto.Sequence;
                    entityContext.StationID = dto.StationID;
                    entityContext.Sequence = dto.Sequence;
                    entityContext.Defect_Code = dto.Defect_Code;
                    entityContext.DefectEnglishName = dto.DefectEnglishName;
                    entityContext.DefecChinesetName = dto.DefecChinesetName;
                    entityContext.Is_Enable = dto.Is_Enable;
                    entityContext.Modify_UID = dto.Modify_UID;
                    entityContext.Modify_Date = dto.Modify_Date;
                    _OEE_StationDefectCodeRepository.Add(entityContext);
                    _unitOfWork.Commit();
                }
                else
                {
                    entityContext = _OEE_StationDefectCodeRepository.GetById(dto.OEE_StationDefectCode_UID);
                    entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                    entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    entityContext.Project_UID = dto.Project_UID;
                    entityContext.LineID = dto.LineID;
                    entityContext.Sequence = dto.Sequence;
                    entityContext.StationID = dto.StationID;
                    entityContext.Sequence = dto.Sequence;
                    entityContext.Defect_Code = dto.Defect_Code;
                    entityContext.DefectEnglishName = dto.DefectEnglishName;
                    entityContext.DefecChinesetName = dto.DefecChinesetName;
                    entityContext.Is_Enable = dto.Is_Enable;
                    entityContext.Modify_UID = dto.Modify_UID;
                    entityContext.Modify_Date = dto.Modify_Date;
                    _OEE_StationDefectCodeRepository.Update(entityContext);
                    _unitOfWork.Commit();
                }

                return "0";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }

        }

        public OEE_StationDefectCodeDTO QueryOEE_StationDefectCodeByUid(int OEE_StationDefectCode_UID)
        {

            var result = _OEE_StationDefectCodeRepository.QueryOEE_StationDefectCodeByUid(OEE_StationDefectCode_UID);
            return result;

        }
        public List<OEE_StationDefectCodeDTO> QueryOEE_StationDefectCodeList(OEE_StationDefectCodeDTO search)
        {
            return _OEE_StationDefectCodeRepository.QueryOEE_StationDefectCodeList(search);
        }
        public List<OEE_StationDefectCodeDTO> GetAllOEE_StationDefectCodeDTOList()
        {
            return _OEE_StationDefectCodeRepository.GetAllOEE_StationDefectCodeDTOList();
        }

        public List<OEE_StationDefectCodeDTO> GetOEE_StationDefectCodeDTOList(string uids)
        {
            return _OEE_StationDefectCodeRepository.GetOEE_StationDefectCodeDTOList(uids);
        }
        public string DeleteOEE_StationDefectCode(int OEE_DownTimeCode_UID, int userid)
        {
            var downTimeCode = _OEE_StationDefectCodeRepository.GetById(OEE_DownTimeCode_UID);
            if (downTimeCode != null)
            {
                try
                {

                    _OEE_StationDefectCodeRepository.Delete(downTimeCode);
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "";

        }
        public string ImportOEE_StationDefectCodekExcel(List<OEE_StationDefectCodeDTO> OEE_DownTimeCodeDTOs)
        {
            return _OEE_StationDefectCodeRepository.ImportOEE_StationDefectCodekExcel(OEE_DownTimeCodeDTOs);
        }
        #endregion

        #region OEE报表
        public string GetLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            return _OEE_EveryDayMachineRepository.GetLastUpdateTime(serchMode);
        }

        public string GetStationLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            return _OEE_EveryDayMachineRepository.GetStationLastUpdateTime(serchMode);
        }

        public string GetRealLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            return _OEE_EveryDayMachineRepository.GetRealLastUpdateTime(serchMode);
        }
        public string GetLineLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            return _OEE_EveryDayMachineRepository.GetLineLastUpdateTime(serchMode);
        }
        public PagedListModel<OEE_EveryDayMachineDTO> QueryOEE_EveryDayMachine(OEE_ReprortSearchModel serchModel, Page page)
        {
            var result = _OEE_EveryDayMachineRepository.QueryOEE_EveryDayMachine(serchModel, page);
            return new PagedListModel<OEE_EveryDayMachineDTO>(0, result.ToList());
        }

        public PagedListModel<MachineIndexModel> GetMachineIndexName(OEE_ReprortSearchModel serchModel, Page page)
        {
            var result = _OEE_EveryDayMachineRepository.QueryOEE_EveryDayMachine(serchModel, page);
            var everyMachineDic = result.GroupBy(p => p.Product_Date);
            List<MachineIndexModel> ListIndexName = new List<MachineIndexModel>();

            //设置默认值
            if (everyMachineDic.Count() == 0)
            {
                foreach (string colorName in Enum.GetNames(typeof(MachineIndexName)))
                {
                    var resultIndexNameModel = GetPageReportIndex(colorName, 0, 0, 0, new List<OEE_EveryDayMachineDTO>());
                    resultIndexNameModel.ResetTime = string.Empty;
                    ListIndexName.Add(resultIndexNameModel);
                }
            }
            else
            {
                foreach (var item in everyMachineDic)
                {
                    var resultModel = item.ToList();
                    serchModel.StartTime = Convert.ToDateTime(item.Key.ToShortDateString());
                    serchModel.EndTime = Convert.ToDateTime(item.Key.ToShortDateString());
                    //计算正常运行时间
                    var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel);
                    int outputCount = resultModel == null ? 0 : resultModel.Sum(p => p.OutPut);///resultOutput.Sum(p => p.Input);

                    var resultDefect = _OEE_DefectCodeDailySumRepository.GetDefectCodeDailySum(serchModel);
                    var ngCount = resultDefect.Sum(p => p.DefectNum);
                    //正常运转时间计算
                    var normalTime = (
                        dailyDownResult.Sum(m => m.DownTime)/60
                        );
                    var resetTime = GetResetTime(resultModel);
                    foreach (string colorName in Enum.GetNames(typeof(MachineIndexName)))
                    {
                        var resultIndexNameModel = GetPageReportIndex(colorName, normalTime, outputCount, ngCount, resultModel);
                        resultIndexNameModel.ResetTime = resetTime;
                        ListIndexName.Add(resultIndexNameModel);
                    }
                }
            }

            return new PagedListModel<MachineIndexModel>(0, ListIndexName);
        }

        public static MachineIndexModel GetMachineIndex(string colorName, double normalTime, int outputCount, int ngCount, List<OEE_EveryDayMachineDTO> dayMachineModel)
        {
            MachineIndexModel indexNameModel = new MachineIndexModel();
            switch (Convert.ToInt32(Enum.Parse(typeof(MachineIndexName), colorName)))
            {
                case 0:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.Fixtures);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : dayMachineModel.Sum(p => p.FixtureNum).ToString("F");
                    break;
                case 1:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.POR);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : dayMachineModel.Sum(p => p.PORCT).ToString("F");
                    break;
                case 2:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.ActualCT);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : dayMachineModel.Sum(p => p.ActualCT).ToString("F2");
                    break;
                case 3:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.CTAchivementRate);
                    indexNameModel.IndexCount = (dayMachineModel == null || dayMachineModel.Sum(p => p.ActualCT) == 0) ? "0.00" : (dayMachineModel.Sum(p => p.PORCT) / (dayMachineModel.Sum(p => p.ActualCT) == 0 ? 1 : dayMachineModel.Sum(p => p.ActualCT))).ToString("F4");
                    break;
                case 4:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : dayMachineModel.Sum(p => p.TotalAvailableHour).ToString("F");
                    break;
                case 5:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PlannedHour);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : dayMachineModel.Sum(p => p.PlannedHour).ToString("F");
                    break;
                case 6:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PlannedMinute);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : (dayMachineModel.Sum(p => p.PlannedHour) * 60).ToString("F");
                    break;
                case 7:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PlannedOutput);
                    indexNameModel.IndexCount = (dayMachineModel.Count() == 0 || dayMachineModel.Sum(p => p.PORCT) == 0) ? "0.00" : (dayMachineModel.Sum(p => p.PlannedHour) * 60 * 60 / dayMachineModel.Sum(p => p.PORCT)).ToString("F");
                    break;
                case 8:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.UptimeMinute);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : (dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime).ToString("F");
                    break;
                case 9:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.RunningCapacity);
                    indexNameModel.IndexCount = (dayMachineModel.Count() == 0 || dayMachineModel.Sum(p => p.PORCT) == 0) ? "0.00" : ((60 * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / dayMachineModel.Sum(p => p.PORCT)).ToString("F");
                    break;
                case 10:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.Throughput);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : outputCount.ToString("F");
                    break;
                case 11:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.GoodPartOutput);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : (outputCount - ngCount).ToString("F");
                    break;
                case 12:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.NGQTY);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : ngCount.ToString("F4");
                    break;
                case 13:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.FirstPassYield);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || outputCount == 0 ? "0.00" : ((outputCount - ngCount) * 1.0 / outputCount).ToString("F4");
                    break;
                case 14:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.AvailableRate);
                    var RunningCapacity2 = ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime));
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || dayMachineModel.Sum(p => p.PlannedHour) == 0 ? "0.00" : (RunningCapacity2 / (dayMachineModel.Sum(p => p.PlannedHour) * 60)).ToString("F4");
                    break;
                case 15:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PerformanceRate);
                    var pfdenominator = (dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime) * 60;
                    indexNameModel.IndexCount = (dayMachineModel.Count() == 0 || pfdenominator == 0) ? "0.00" : (outputCount * (dayMachineModel.Sum(p => p.PORCT) / pfdenominator)).ToString("F4");
                    break;
                case 16:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency);
                    //分子
                    var oeemolecule = dayMachineModel.Sum(p => p.PORCT) * (outputCount - ngCount) * (dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime);
                    //分母
                    var oeedenominator = outputCount * dayMachineModel.Sum(p => p.ActualCT) * dayMachineModel.Sum(p => p.PlannedHour) * 60;
                    //indexNameModel.IndexCount = dayMachineModel.Count() == 0 || oeedenominator ==0? "0.00" : ((dayMachineModel.Sum(p => p.PORCT) / dayMachineModel.Sum(p => p.ActualCT) == 0 ? 1 : dayMachineModel.Sum(p => p.ActualCT)) * (outputCount - ngCount / outputCount == 0 ? 1 : outputCount) * (60 / (dayMachineModel.Sum(p => p.PORCT) * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / dayMachineModel.Sum(p => p.PlannedHour) == 0 ? 1 : dayMachineModel.Sum(p => p.PlannedHour))).ToString("F");
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || oeedenominator == 0 ? "0.00" : (oeemolecule / oeedenominator).ToString("F4");
                    break;
                case 17:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency);
                    //分子
                    var oeeAllDenominator = (outputCount - ngCount) * dayMachineModel.Sum(p => p.PORCT);
                    //分母
                    var oeeAllMolecule = 60 * 60 * dayMachineModel.Sum(p => p.PlannedHour);                    //indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : ((outputCount * (dayMachineModel.Sum(p => p.PORCT) * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / 60) * (60 / (dayMachineModel.Sum(p => p.PORCT) * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / dayMachineModel.Sum(p => p.PlannedHour) == 0 ? 1 : dayMachineModel.Sum(p => p.PlannedHour)) * (outputCount - ngCount / outputCount == 0 ? 1 : outputCount)).ToString("F");
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || oeeAllMolecule == 0 ? "0.00" : (oeeAllDenominator / oeeAllMolecule).ToString("F4");
                    break;
                case 18:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : (((dayMachineModel.Sum(p => p.PlannedHour) * 3600) - ((outputCount - ngCount) * dayMachineModel.Sum(p => p.PORCT))) / 3600).ToString("F4");
                    break;
            }

            return indexNameModel;
        }

        public static MachineIndexModel GetPageReportIndex(string colorName, double normalTime, int outputCount, int ngCount, List<OEE_EveryDayMachineDTO> dayMachineModel)
        {
            MachineIndexModel indexNameModel = new MachineIndexModel();
            switch (Convert.ToInt32(Enum.Parse(typeof(MachineIndexName), colorName)))
            {
                case 0:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.Fixtures);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : dayMachineModel.Sum(p => p.FixtureNum).ToString("F0");
                    break;
                case 1:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.POR);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : dayMachineModel.Sum(p => p.PORCT).ToString("F0");
                    break;
                case 2:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.ActualCT);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : dayMachineModel.Sum(p => p.ActualCT).ToString("F2");
                    break;
                case 3:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.CTAchivementRate);
                    indexNameModel.IndexCount = (dayMachineModel == null || dayMachineModel.Sum(p => p.ActualCT) == 0) ? 0.ToString("p2") : ForMatOEEValueToP(dayMachineModel.Sum(p => p.PORCT) / (dayMachineModel.Sum(p => p.ActualCT) == 0 ? 1 : dayMachineModel.Sum(p => p.ActualCT)));
                    break;
                case 4:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : dayMachineModel.Sum(p => p.TotalAvailableHour).ToString("F0");
                    break;
                case 5:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PlannedHour);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : dayMachineModel.Sum(p => p.PlannedHour).ToString("F0");
                    break;
                case 6:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PlannedMinute);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : (dayMachineModel.Sum(p => p.PlannedHour) * 60).ToString("F0");
                    break;
                case 7:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PlannedOutput);
                    indexNameModel.IndexCount = (dayMachineModel.Count() == 0 || dayMachineModel.Sum(p => p.PORCT) == 0) ? "0" : (dayMachineModel.Sum(p => p.PlannedHour) * 60 * 60 / dayMachineModel.Sum(p => p.PORCT)).ToString("F0");
                    break;
                case 8:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.UptimeMinute);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : (dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime).ToString("F0");
                    break;
                case 9:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.RunningCapacity);
                    indexNameModel.IndexCount = (dayMachineModel.Count() == 0 || dayMachineModel.Sum(p => p.PORCT) == 0) ? "0" : ((60 * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / dayMachineModel.Sum(p => p.PORCT)).ToString("F0");
                    break;
                case 10:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.Throughput);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : outputCount.ToString("F0");
                    break;
                case 11:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.GoodPartOutput);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : (outputCount - ngCount).ToString("F0");
                    break;
                case 12:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.NGQTY);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : ngCount.ToString("F0");
                    break;
                case 13:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.FirstPassYield);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || outputCount == 0 ? 0.ToString("p2") : ForMatOEEValueToP((outputCount - ngCount) * 1.0 / outputCount);
                    break;
                case 14:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.AvailableRate);
                    var RunningCapacity2 = ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime));
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || dayMachineModel.Sum(p => p.PlannedHour) == 0 ? 0.ToString("p2") : ForMatOEEValueToP(RunningCapacity2 / (dayMachineModel.Sum(p => p.PlannedHour) * 60));
                    break;
                case 15:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.PerformanceRate);
                    var pfdenominator = (dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime) * 60;
                    indexNameModel.IndexCount = (dayMachineModel.Count() == 0 || pfdenominator == 0) ? 0.ToString("p2") : ForMatOEEValueToP(outputCount * (dayMachineModel.Sum(p => p.PORCT) / pfdenominator));
                    break;
                case 16:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency);
                    //分子
                    var oeemolecule = dayMachineModel.Sum(p => p.PORCT) * (outputCount - ngCount) * (dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime);
                    //分母
                    var oeedenominator = outputCount * dayMachineModel.Sum(p => p.ActualCT) * dayMachineModel.Sum(p => p.PlannedHour) * 60;
                    //indexNameModel.IndexCount = dayMachineModel.Count() == 0 || oeedenominator ==0? "0.00" : ((dayMachineModel.Sum(p => p.PORCT) / dayMachineModel.Sum(p => p.ActualCT) == 0 ? 1 : dayMachineModel.Sum(p => p.ActualCT)) * (outputCount - ngCount / outputCount == 0 ? 1 : outputCount) * (60 / (dayMachineModel.Sum(p => p.PORCT) * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / dayMachineModel.Sum(p => p.PlannedHour) == 0 ? 1 : dayMachineModel.Sum(p => p.PlannedHour))).ToString("F");
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || oeedenominator == 0 ? 0.ToString("p2") : ForMatOEEValueToP(oeemolecule / oeedenominator);
                    break;
                case 17:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency);
                    //分子
                    var oeeAllDenominator = (outputCount - ngCount) * dayMachineModel.Sum(p => p.PORCT);
                    //分母
                    var oeeAllMolecule = 60 * 60 * dayMachineModel.Sum(p => p.PlannedHour);                    //indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0.00" : ((outputCount * (dayMachineModel.Sum(p => p.PORCT) * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / 60) * (60 / (dayMachineModel.Sum(p => p.PORCT) * ((dayMachineModel.Sum(p => p.PlannedHour) * 60 - normalTime))) / dayMachineModel.Sum(p => p.PlannedHour) == 0 ? 1 : dayMachineModel.Sum(p => p.PlannedHour)) * (outputCount - ngCount / outputCount == 0 ? 1 : outputCount)).ToString("F");
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 || oeeAllMolecule == 0 ? 0.ToString("p2") : ForMatOEEValueToP(oeeAllDenominator / oeeAllMolecule);
                    break;
                case 18:
                    indexNameModel.IndexName = EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss);
                    indexNameModel.IndexCount = dayMachineModel.Count() == 0 ? "0" : (((dayMachineModel.Sum(p => p.PlannedHour) * 3600) - ((outputCount - ngCount) * dayMachineModel.Sum(p => p.PORCT))) / 3600).ToString("F0");
                    break;
            }

            return indexNameModel;
        }
        public PagedListModel<MachineIndexModel> GetMachineBreakDown(OEE_ReprortSearchModel serchModel, Page page)
        {
            var DownTypeList = OEE_DownTypeRepository.GetMany(p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID && p.Is_Enable == true);
            var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel);
            //var totalDownTime = dailyDownResult.Sum(q => q.DownTime);
            List<MachineIndexModel> List = new List<MachineIndexModel>();
            var result = _OEE_EveryDayMachineRepository.QueryOEE_EveryDayMachine(serchModel, page);
            var totalDownTime = result.Sum(p => p.PlannedHour) * 60;
            //获取计划分钟数
            foreach (var item in DownTypeList)
            {
                var dailyDown = dailyDownResult.Where(p => p.OEE_DownTimeType_UID == item.OEE_DownTimeType_UID);
                MachineIndexModel indexNameModel = new MachineIndexModel();
                indexNameModel.DownTimeType = "Downtime Breakdown";
                //if (serchModel.languageID == 1)
                //{
                //    var EnglishTypeName = Regex.Replace(item.Type_Name, @"[\u4e00-\u9fa5]", ""); //去除汉字
                //    indexNameModel.IndexName = EnglishTypeName;
                //}
                //else
                //{
                //    var ChinaTypeName = Regex.Replace(item.Type_Name, @"[^\u4e00-\u9fa5]", ""); //只留汉字
                //    indexNameModel.IndexName = ChinaTypeName;
                //}
                indexNameModel.IndexName = item.Type_Name;
                indexNameModel.IndexCount = (dailyDown.Sum(p => p.DownTime)).ToString("F");
                indexNameModel.Percentage = totalDownTime == 0 ? 0 : dailyDown.Sum(p => p.DownTime) / (totalDownTime*60);
                indexNameModel.MachineDate = dailyDown.FirstOrDefault() == null ? DateTime.Now : dailyDown.FirstOrDefault().DownDate;
                indexNameModel.OEE_DownTimeType_UID = item.OEE_DownTimeType_UID;
                List.Add(indexNameModel);
            }

            var downTimeCount = List.Count();
            //设置默认值
            if (downTimeCount < 7)
            {
                for (int i = 0; i < 7 - downTimeCount; i++)
                {
                    MachineIndexModel indexNameModel = new MachineIndexModel();
                    indexNameModel.DownTimeType = "Downtime Breakdown";
                    indexNameModel.IndexName = "No Downtime Breakdown";
                    indexNameModel.IndexCount = "0";
                    indexNameModel.Percentage = 0;
                    List.Add(indexNameModel);
                }
            }

            var resultDefect = _OEE_DefectCodeDailySumRepository.GetDefectCodeDailySum(serchModel).GroupBy(p => p.OEE_StationDefectCode_UID);
            var totalDefectNum = resultDefect.Sum(p => p.Sum(q => q.DefectNum));
            var firstYieldCount = resultDefect.Count();
            foreach (var item in resultDefect)
            {
                MachineIndexModel indexNameModel = new MachineIndexModel();
                indexNameModel.DownTimeType = "Yield Breakdown";
                if (serchModel.languageID == 1)
                {
                    indexNameModel.IndexName = item.FirstOrDefault().DefectName;
                }
                else if (serchModel.languageID == 2)
                {
                    indexNameModel.IndexName = item.FirstOrDefault().DefectChineseName;
                }

                indexNameModel.IndexCount = item.Sum(p => p.DefectNum).ToString("F");
                indexNameModel.Percentage = item.Sum(p => p.DefectNum) * 1.0 / totalDefectNum;
                List.Add(indexNameModel);
            }

            //设置默认值
            if (firstYieldCount < 7)
            {
                for (int i = 0; i < 7 - firstYieldCount; i++)
                {
                    MachineIndexModel indexNameModel = new MachineIndexModel();
                    indexNameModel.DownTimeType = "Yield Breakdown";
                    indexNameModel.IndexName = "No Yield Breakdown";
                    indexNameModel.IndexCount = "0";
                    indexNameModel.Percentage = 0;
                    List.Add(indexNameModel);
                }
            }

            List = List.OrderBy(p => p.DownTimeType).ToList();
            return new PagedListModel<MachineIndexModel>(0, List);
        }

        public PagedListModel<OEE_MachineDailyDownRecordDTO> GetDownTimeDetials(OEE_ReprortSearchModel serchModel, Page page)
        {
            var totalCount = 0;
            var result = _OEE_MachineDailyDownRecordRepository.GetDownTimeDetialsDTO(serchModel, page, out totalCount);
            return new PagedListModel<OEE_MachineDailyDownRecordDTO>(totalCount, result.ToList());
        }

        /// <summary>
        /// 获取导出报表一般指标
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public List<MachineIndexModel> GetOEEMetricsList(OEE_ReprortSearchModel serchModel)
        {
            var result = _OEE_EveryDayMachineRepository.QueryOEE_EveryDayMachine(serchModel, new Page());
            var everyMachineDic = result.GroupBy(p => p.Product_Date);
            List<MachineIndexModel> ListIndexName = new List<MachineIndexModel>();
            foreach (var item in everyMachineDic)
            {
                var resultModel = item.ToList();
                serchModel.StartTime = Convert.ToDateTime(item.Key.ToShortDateString());
                serchModel.EndTime = Convert.ToDateTime(item.Key.ToShortDateString());
                //计算正常运行时间
                var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel);
                int outputCount = resultModel.Sum(p => p.OutPut);///resultOutput.Sum(p => p.Input);
                var resultDefect = _OEE_DefectCodeDailySumRepository.GetDefectCodeDailySum(serchModel);
                var ngCount = resultDefect.Sum(p => p.DefectNum);
                //正常运转时间计算
                var normalTime = (
                    dailyDownResult.Sum(m => m.DownTime) /60
                    );

                var DtCodeAbnormal = IsDtCodeAbnormal(resultModel);
                var ResetAbnormal = IsCNCResetAbnormal(resultModel);
                var ResetTime = GetResetTime(resultModel);
                var DFCodeAbnormal = IsDFCodeAbnormal(resultModel);
                var TargetArray = GetTargetArray(resultModel.FirstOrDefault().OEEDashBoardTarget);
                foreach (string colorName in Enum.GetNames(typeof(MachineIndexName)))
                {
                    var resultIndexNameModel = GetMachineIndex(colorName, normalTime, outputCount, ngCount, resultModel);
                    resultIndexNameModel.MachineDate = item.Key;
                    resultIndexNameModel.Is_CncResetAbnomal = ResetAbnormal;
                    resultIndexNameModel.Is_DtCodeAbnormal = DtCodeAbnormal;
                    resultIndexNameModel.Is_DfCeodeAbnomal = DFCodeAbnormal;
                    resultIndexNameModel.ResetTime = ResetTime;
                    resultIndexNameModel.FirstDashBoardTarget = TargetArray[0];
                    resultIndexNameModel.SecondDashBoardTarget = TargetArray[1];
                    ListIndexName.Add(resultIndexNameModel);
                }
            }

            return ListIndexName;
        }

        /// <summary>
        /// 获取导出BuTemplate 报表的模板
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public List<MachineIndexModel> GetButemplateDataList(OEE_ReprortSearchModel serchModel)
        {
            var result = _OEE_EveryDayMachineRepository.QueryBuTemplateData(serchModel, new Page());

            var machineResult = result.GroupBy(p => p.OEE_MachineInfo_UID);
            List<MachineIndexModel> ListIndexName = new List<MachineIndexModel>();

            foreach (var machineItem in machineResult)
            {
                var everyMachineDic = machineItem.GroupBy(p => new
                {
                    Product_Date = p.Product_Date,
                    ShiftTimeID = p.ShiftTimeID
                });

                foreach (var item in everyMachineDic)
                {
                    var resultModel = item.ToList();
                    serchModel.StartTime = Convert.ToDateTime(item.Key.Product_Date.ToShortDateString());
                    serchModel.EndTime = Convert.ToDateTime(item.Key.Product_Date.ToShortDateString());
                    if (item.Key.ShiftTimeID.HasValue)
                    {
                        serchModel.ShiftTimeID = Convert.ToInt32(item.Key.ShiftTimeID);
                    };

                    //计算正常运行时间
                    var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel);
                    int outputCount = resultModel.Sum(p => p.OutPut);///resultOutput.Sum(p => p.Input);
                    var resultDefect = _OEE_DefectCodeDailySumRepository.GetDefectCodeDailySum(serchModel);
                    var ngCount = resultDefect.Sum(p => p.DefectNum);
                    //正常运转时间计算
                    var normalTime = (
                        dailyDownResult.Sum(m => m.DownTime)/60
                        );
                    foreach (string colorName in Enum.GetNames(typeof(MachineIndexName)))
                    {
                        var resultIndexNameModel = GetMachineIndex(colorName, normalTime, outputCount, ngCount, resultModel);
                        resultIndexNameModel.MachineDate = item.Key.Product_Date;
                        resultIndexNameModel.ShiftName = item.FirstOrDefault().ShiftName;
                        resultIndexNameModel.MachineName = item.FirstOrDefault().MachineName;
                        resultIndexNameModel.OEE_MachineInfo_UID = machineItem.Key;
                        ListIndexName.Add(resultIndexNameModel);
                    }
                }
            }

            return ListIndexName;
        }


        public List<OEE_MachineStatus> GetAllStationMachineStatusList(OEE_ReprortSearchModel serchModel)
        {
            return MachineStatusRepository.GetAllStationMachineStatusList(serchModel);
        }
        /// <summary>
        /// 获取导出报表的FirstYield
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public List<MachineIndexModel> GetFirstYieldList(OEE_ReprortSearchModel serchModel)
        {
            List<MachineIndexModel> List = new List<MachineIndexModel>();
            var resultDefect = _OEE_DefectCodeDailySumRepository.GetDefectCodeDailySum(serchModel).GroupBy(p => p.OEE_StationDefectCode_UID);
            var totalDefectNum = resultDefect.Sum(p => p.Sum(q => q.DefectNum));
            var firstYieldCount = resultDefect.Count();
            foreach (var item in resultDefect)
            {
                var dateItemList = item.GroupBy(p => p.ProductDate);
                foreach (var dateItem in dateItemList)
                {
                    MachineIndexModel indexNameModel = new MachineIndexModel();
                    indexNameModel.DownTimeType = "Yield Breakdown";
                    indexNameModel.IndexName = dateItem.FirstOrDefault().DefectName;
                    indexNameModel.IndexCount = dateItem.Sum(p => p.DefectNum).ToString("F4");
                    indexNameModel.Percentage = dateItem.Sum(p => p.DefectNum) / totalDefectNum;
                    indexNameModel.MachineDate = dateItem.FirstOrDefault() == null ? DateTime.Now : dateItem.FirstOrDefault().ProductDate;
                    List.Add(indexNameModel);
                }
            }
            return List;
        }

        /// <summary>
        /// 获取导出报表的DowntimeBreakdown
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public List<MachineIndexModel> GetDowntimeBreakdownList(OEE_ReprortSearchModel serchModel)
        {
            var DownTypeList = OEE_DownTypeRepository.GetMany(p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID && p.Is_Enable == true);
            var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel).GroupBy(p => p.DownDate);
            //var totalDownTime = dailyDownResult.Sum(p => p.Sum(q => q.DownTime));
            List<MachineIndexModel> List = new List<MachineIndexModel>();
            var result = _OEE_EveryDayMachineRepository.QueryOEE_EveryDayMachine(serchModel, new Page());
            var everyMachineDic = result.GroupBy(p => p.Product_Date).ToDictionary(p => p.Key, q => q);
            //获取计划分钟数
            string defaultColor = "Red";
            foreach (var item in DownTypeList)
            {
                var DownTimeColorDic = GetDownTimeColorDic();

                foreach (var dailyDownItem in dailyDownResult)
                {
                    var dailyDown = dailyDownItem.Where(p => p.OEE_DownTimeType_UID == item.OEE_DownTimeType_UID);
                    var totalDownTime = everyMachineDic.Where(p => p.Key == dailyDownItem.FirstOrDefault().DownDate).Sum(q => q.Value.Sum(m => m.PlannedHour)) * 60;
                    MachineIndexModel indexNameModel = new MachineIndexModel();
                    indexNameModel.DownTimeType = "Downtime Breakdown";
                    indexNameModel.IndexName = item.Type_Name;
                    indexNameModel.IndexCount = (dailyDown.Sum(p => p.DownTime)).ToString("F");
                    indexNameModel.Percentage = totalDownTime == 0 ? 0 : dailyDown.Sum(p => p.DownTime) / (totalDownTime*60);
                    indexNameModel.MachineDate = dailyDown.FirstOrDefault() == null ? DateTime.Now : dailyDown.FirstOrDefault().DownDate;
                    DownTimeColorDic.TryGetValue(item.Sequence, out defaultColor);
                    indexNameModel.Color = defaultColor;
                    List.Add(indexNameModel);
                }
            }

            //Downtime breakdown
            //var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel).GroupBy(p => p.OEE_DownTimeType_UID);
            //var totalDownTime = dailyDownResult.Sum(p => p.Sum(q => q.DownTime));
            //List<MachineIndexModel> List = new List<MachineIndexModel>();
            //var downTimeCount = dailyDownResult.Count();
            //foreach (var item in dailyDownResult)
            //{
            //    MachineIndexModel indexNameModel = new MachineIndexModel();
            //    indexNameModel.DownTimeType = "Downtime Breakdown";
            //    indexNameModel.IndexName = item.FirstOrDefault().Type_Name;
            //    indexNameModel.IndexCount = item.Sum(p => p.DownTime).ToString("F");
            //    indexNameModel.Percentage = item.Sum(p => p.DownTime) / totalDownTime;
            //    indexNameModel.MachineDate = item.FirstOrDefault().DownDate;
            //    List.Add(indexNameModel);
            //}
            return List;
        }



        /// <summary>
        /// 获取DownTime和颜色配置字典
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, string> GetDownTimeColorDic()
        {
            var dic = new Dictionary<int, string>();
            foreach (var demoEnum in Enum.GetValues(typeof(BreakDownColorConfig)))
            {
                BreakDownColorConfig input = (BreakDownColorConfig)demoEnum;
                dic.Add((int)input, input.GetDescription());
            }
            return dic;
        }

        #endregion

        #region OEE 机台统计Pie图表
        public List<MachinePieIndexModel> GetMachinePieReportData(OEE_ReprortSearchModel serchModel)
        {
            var result = _OEE_EveryDayMachineRepository.GetAllStationData(serchModel, new Page());
            var everyMachineDic = result.GroupBy(p => p.OEE_MachineInfo_UID);
            List<MachineIndexModel> ListIndexName = new List<MachineIndexModel>();
            foreach (var item in everyMachineDic)
            {
                var resultModel = item.ToList();
                //计算正常运行时间
                serchModel.EQP_Uid = item.Key;
                var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel);
                int outputCount = resultModel.Sum(p => p.OutPut);///resultOutput.Sum(p => p.Input);
                var resultDefect = _OEE_DefectCodeDailySumRepository.GetDefectCodeDailySum(serchModel);
                var ngCount = resultDefect.Sum(p => p.DefectNum);
                //正常运转时间计算
                var TotalTime = (
                    dailyDownResult.Sum(m => m.DownTime)
                    );

                //判断该机台是不是异常
                var DtCodeAbnormal = IsDtCodeAbnormal(resultModel);
                var ResetAbnormal = IsCNCResetAbnormal(resultModel);
                var DFCodeAbnormal = IsDFCodeAbnormal(resultModel);
                var IsOffLine = IsOffLineAbnormal(resultModel);
                var Is_ShowOffLine = IsShowOffLine(resultModel);
                var DashBoardTarget = resultModel.FirstOrDefault().OEEDashBoardTarget;
                foreach (string colorName in Enum.GetNames(typeof(MachineIndexName)))
                {
                    var resultIndexNameModel = GetMachineIndex(colorName, TotalTime, outputCount, ngCount, resultModel);
                    resultIndexNameModel.MachineName = item.FirstOrDefault().MachineName;
                    resultIndexNameModel.OEE_MachineInfo_UID = item.Key;
                    resultIndexNameModel.Is_CncResetAbnomal = ResetAbnormal;
                    resultIndexNameModel.Is_DtCodeAbnormal = DtCodeAbnormal;
                    resultIndexNameModel.Is_DfCeodeAbnomal = DFCodeAbnormal;
                    resultIndexNameModel.Is_OfflineAbnomal = IsOffLine;
                    resultIndexNameModel.Is_ShowOffLine = Is_ShowOffLine;

                    resultIndexNameModel.DashBoardTarget = DashBoardTarget;
                    ListIndexName.Add(resultIndexNameModel);
                }
            }

            List<MachinePieIndexModel> pieModelList = new List<MachinePieIndexModel>();
            var pieList = ListIndexName.GroupBy(p => p.OEE_MachineInfo_UID);
            foreach (var item in pieList)
            {
                MachinePieIndexModel pieModel = new MachinePieIndexModel();
                pieModel.IsTotalStationStatic = "NO";
                pieModel.OEE_MachineInfo_UID = item.Key;
                pieModel.MachineName = item.FirstOrDefault().MachineName;

                //控制OEE值的范围
                var OEEValue = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency)).FirstOrDefault().IndexCount;
                pieModel.OEEValue = ForMatOEEValueToS(double.Parse(OEEValue));
                var PerformanceRate = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.PerformanceRate)).FirstOrDefault().IndexCount;
                pieModel.PerformanceRate = ForMatOEEValueToS(double.Parse(PerformanceRate));
                var AvailableRate = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.AvailableRate)).FirstOrDefault().IndexCount;
                pieModel.AvailableRate = ForMatOEEValueToS(double.Parse(AvailableRate));
                var FirstYield = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.FirstPassYield)).FirstOrDefault().IndexCount;
                pieModel.FirstYield = ForMatOEEValueToS(double.Parse(FirstYield));

                pieModel.Is_CncResetAbnomal = item.FirstOrDefault().Is_CncResetAbnomal;
                pieModel.Is_DtCodeAbnormal = item.FirstOrDefault().Is_DtCodeAbnormal;
                pieModel.Is_DfCodeAbnomal = item.FirstOrDefault().Is_DtCodeAbnormal;
                pieModel.Is_OffLine = item.FirstOrDefault().Is_OfflineAbnomal;
                pieModel.Is_ShowOffLine = item.FirstOrDefault().Is_ShowOffLine;
                var TargetArray = GetTargetArray(item.FirstOrDefault().DashBoardTarget);
                pieModel.FirstDashBoardTarget = TargetArray[0];
                pieModel.SecondDashBoardTarget = TargetArray[1];

                if (double.Parse(pieModel.OEEValue) < 0)
                {
                    pieModel.Is_OeeAbnomal = true;
                }

                pieModel.Is_OeeAbnomal = item.FirstOrDefault().Is_OeeAbnomal;
                pieModelList.Add(pieModel);
            }

            //根据工站排序
            var resultList = GetOrderByRule(pieModelList, serchModel.OrderByRule);
            if (serchModel.PageNum != 0)
            {
                serchModel.PageNum = serchModel.PageNum - 1;
            }
            resultList = resultList.Skip(serchModel.PagePieSize * serchModel.PageNum).Take(serchModel.PagePieSize).ToList();
            MachinePieIndexModel pieModelStation = new MachinePieIndexModel();
            pieModelStation.IsTotalStationStatic = "YES";
            //增加OEE的管控
            pieModelStation.OEEValue = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.OEEValue)) / pieModelList.Count());
            pieModelStation.PerformanceRate = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.PerformanceRate)) / pieModelList.Count());
            pieModelStation.AvailableRate = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.AvailableRate)) / pieModelList.Count());
            pieModelStation.FirstYield = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.FirstYield)) / pieModelList.Count());
            resultList.Add(pieModelStation);

            MachinePieIndexModel piePageModel = new MachinePieIndexModel();
            piePageModel.IsTotalStationStatic = "PAGE";
            piePageModel.TotalCount = pieModelList.Count();
            piePageModel.TotalPage = int.Parse(Math.Ceiling((piePageModel.TotalCount * 1.0 / serchModel.PagePieSize)).ToString());
            resultList.Add(piePageModel);
            return resultList;

        }
        public static string ForMatOEEValueToP(double param)
        {
            if (param < 0)
            {
                return 0.ToString("P2");
            }
            else if (param > 1)
            {
                return 1.ToString("P2");
            }
            else
            {
                return param.ToString("P2");
            }
        }
        public static string ForMatOEEValueToS(double param)
        {
            if (param < 0)
            {
                return 0.ToString();
            }
            else if (param > 1)
            {
                return 1.ToString();
            }
            else
            {
                return param.ToString();
            }
        }
        /// <summary>
        /// 是否有停机异常
        /// </summary>
        /// <returns></returns>
        public bool IsDtCodeAbnormal(List<OEE_EveryDayMachineDTO> resultModel)
        {
            try
            {
                var DtCodeAbnormal = resultModel.Where(p => p.Is_DownType != null);
                if (DtCodeAbnormal == null || DtCodeAbnormal.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return true;
            }

        }

        /// <summary>
        /// 是否CNC 机台重置
        /// </summary>
        /// <returns></returns>
        public bool IsCNCResetAbnormal(List<OEE_EveryDayMachineDTO> resultModel)
        {
            try
            {
                var ResetTimeAbnormal = resultModel.Where(p => !string.IsNullOrEmpty(p.ResetTime));
                if (ResetTimeAbnormal == null || ResetTimeAbnormal.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool IsDFCodeAbnormal(List<OEE_EveryDayMachineDTO> resultModel)
        {
            try
            {
                var ResetTimeAbnormal = resultModel.Where(p => p.AbnormalDFCode == true);
                if (ResetTimeAbnormal == null || ResetTimeAbnormal.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 设备是否断网
        /// </summary>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public bool IsOffLineAbnormal(List<OEE_EveryDayMachineDTO> resultModel)
        {
            try
            {
                var ResetTimeAbnormal = resultModel.Where(p => p.Is_Offline == true);
                if (ResetTimeAbnormal == null || ResetTimeAbnormal.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 设备是否断网
        /// </summary>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public bool IsShowOffLine(List<OEE_EveryDayMachineDTO> resultModel)
        {
            try
            {
                if (resultModel.FirstOrDefault().OrganitionName.ToUpper() == "CNC")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取的重置的时间
        /// </summary>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public string GetResetTime(List<OEE_EveryDayMachineDTO> resultModel)
        {
            try
            {
                var ResetTimeAbnormal = resultModel.Where(p => !string.IsNullOrEmpty(p.ResetTime));
                if (ResetTimeAbnormal == null || ResetTimeAbnormal.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return ResetTimeAbnormal.OrderByDescending(p => p.ResetTime).FirstOrDefault().ResetTime;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取目标设置
        /// </summary>
        /// <param name="dashBoardtarget"></param>
        /// <returns></returns>
        public double[] GetTargetArray(string dashBoardtarget)
        {
            double[] targetArray = new double[] { 50, 80 };
            try
            {
                targetArray[0] = double.Parse(dashBoardtarget.Split(',')[0]);
                targetArray[1] = double.Parse(dashBoardtarget.Split(',')[1]);
                return targetArray;
            }
            catch (Exception)
            {
                return targetArray;
            }
        }

        public List<MachinePieIndexModel> GetOrderByRule(List<MachinePieIndexModel> pieList, int orderRule)
        {
            switch (orderRule)
            {
                case 0:
                    return pieList.OrderBy(p => p.OEEValue).ToList();//OEE%由低到高
                case 1:
                    return pieList.OrderByDescending(p => p.OEEValue).ToList();//OEE%由高到低
                case 2:
                    return pieList.OrderBy(p => p.AvailableRate).ToList();//AR%由低到高
                case 3:
                    return pieList.OrderByDescending(p => p.AvailableRate).ToList();//AR%由高到低
                case 4:
                    return pieList.OrderBy(p => p.PerformanceRate).ToList();//PR%由低到高
                case 5:
                    return pieList.OrderByDescending(p => p.PerformanceRate).ToList();//PR%由高到低
                case 6:
                    return pieList.OrderBy(p => p.FirstYield).ToList();//QR%由低到高
                case 7:
                    return pieList.OrderByDescending(p => p.FirstYield).ToList();//QR%由高到低
                case 8:
                    //return pieList.OrderBy(p => p.MachineName).ToList();//机台名称A-Z
                    return NaturalSort(pieList).ToList();
                case 9:
                    //return pieList.OrderByDescending(p => p.MachineName).ToList();//机台名称Z-A
                    return NaturalDescSort(pieList).ToList();
                default:
                    return pieList.OrderBy(p => p.OEEValue).ToList();//OEE%由低到高
            }
        }

        public List<MachinePieIndexModel> GetLineOrderByRule(List<MachinePieIndexModel> pieList, int orderRule)
        {
            switch (orderRule)
            {
                case 0:
                    return pieList.OrderBy(p => p.OEEValue).ToList();//OEE%由低到高
                case 1:
                    return pieList.OrderByDescending(p => p.OEEValue).ToList();//OEE%由高到低
                case 2:
                    return pieList.OrderBy(p => p.AvailableRate).ToList();//AR%由低到高
                case 3:
                    return pieList.OrderByDescending(p => p.AvailableRate).ToList();//AR%由高到低
                case 4:
                    return pieList.OrderBy(p => p.PerformanceRate).ToList();//PR%由低到高
                case 5:
                    return pieList.OrderByDescending(p => p.PerformanceRate).ToList();//PR%由高到低
                case 6:
                    return pieList.OrderBy(p => p.FirstYield).ToList();//QR%由低到高
                case 7:
                    return pieList.OrderByDescending(p => p.FirstYield).ToList();//QR%由高到低
                case 8:
                    return NaturalStationSort(pieList).ToList();
                case 9:
                    return NaturalStationDescSort(pieList).ToList();
                default:
                    return pieList.OrderBy(p => p.OEEValue).ToList();//OEE%由低到高
            }
        }

        public static IEnumerable<MachinePieIndexModel> NaturalDescSort(IEnumerable<MachinePieIndexModel> list)
        {
            int maxLen = list.Select(s => s.MachineName.Length).Max();
            Func<string, char> PaddingChar = s => char.IsDigit(s[0]) ? ' ' : char.MaxValue;

            return list
                    .Select(s =>
                        new
                        {
                            OrgStr = s,
                            SortStr = Regex.Replace(s.MachineName, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, PaddingChar(m.Value)))
                        })
                    .OrderByDescending(x => x.SortStr)
                    .Select(x => x.OrgStr);
        }

        public static IEnumerable<MachinePieIndexModel> NaturalStationDescSort(IEnumerable<MachinePieIndexModel> list)
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
                    .OrderByDescending(x => x.SortStr)
                    .Select(x => x.OrgStr);
        }


        public static IEnumerable<MachinePieIndexModel> NaturalStationSort(IEnumerable<MachinePieIndexModel> list)
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
        /// 机台名称排序呢
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<MachinePieIndexModel> NaturalSort(IEnumerable<MachinePieIndexModel> list)
        {
            int maxLen = list.Select(s => s.MachineName.Length).Max();
            Func<string, char> PaddingChar = s => char.IsDigit(s[0]) ? ' ' : char.MaxValue;

            return list
                    .Select(s =>
                        new
                        {
                            OrgStr = s,
                            SortStr = Regex.Replace(s.MachineName, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, PaddingChar(m.Value)))
                        })
                    .OrderBy(x => x.SortStr)
                    .Select(x => x.OrgStr);
        }


        /// <summary>
        /// 机台的下拉熏香
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<OEE_MachineInfoDTO> NaturalMachineNameSort(IEnumerable<OEE_MachineInfoDTO> list)
        {
            if (list.Count() == 0) return null;
            int maxLen = list.Select(s => s.MachineNo.Length).Max();
            Func<string, char> PaddingChar = s => char.IsDigit(s[0]) ? ' ' : char.MaxValue;
            return list
                    .Select(s =>
                        new
                        {
                            OrgStr = s,
                            SortStr = Regex.Replace(s.MachineNo, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, PaddingChar(m.Value)))
                        })
                    .OrderBy(x => x.SortStr)
                    .Select(x => x.OrgStr);
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

        public List<OEE_AbnormalDFCode> GetAbnormalDTCode(OEE_ReprortSearchModel serchModel)
        {
            //获取工工站下面所有机台的不良代码
            var result = DFcodeMissingRepository.GetDTcodeMissingList(serchModel);

            //等于0表示机台
            if (serchModel.VersionNumber == 0)
            {
                return result = result.Where(p => p.Machine_UID == serchModel.EQP_Uid).ToList();
            }

            return result;
        }

        public List<AbnormalDFCode> GetAbnormalDFCode(OEE_ReprortSearchModel serchModel)
        {
            //获取工工站下面所有机台的不良代码
            var result = DFCodeRepository.GetAbnormalDFCodeList(serchModel);

            //等于0表示机台
            if (serchModel.VersionNumber == 0)
            {
                return result = result.Where(p => p.Machine_UID == serchModel.EQP_Uid).ToList();
            }

            return result;
        }

        #endregion

        #region OEE Line 统计图表
        public List<MachinePieIndexModel> GetLineStaticData(OEE_ReprortSearchModel serchModel)
        {
            //1 获取线下面的所有的机台数据
            var resultLine = _OEE_EveryDayMachineRepository.GetLineStaticData(serchModel, new Page());
            var lineDic = resultLine.GroupBy(p => p.StationID);
            List<MachineIndexModel> ListIndexName = new List<MachineIndexModel>();
            foreach (var stationItem in lineDic)
            {
                var everyMachineDic = stationItem.GroupBy(p => p.OEE_MachineInfo_UID);
                #region   //计算每个机台的数据
                foreach (var item in everyMachineDic)
                {
                    var resultModel = item.ToList();
                    //计算正常运行时间
                    serchModel.EQP_Uid = item.Key;
                    var dailyDownResult = _OEE_MachineDailyDownRecordRepository.GetMachineDailyDownRecord(serchModel);
                    int outputCount = resultModel.Sum(p => p.OutPut);///resultOutput.Sum(p => p.Input);
                    var resultDefect = _OEE_DefectCodeDailySumRepository.GetDefectCodeDailySum(serchModel);
                    var ngCount = resultDefect.Sum(p => p.DefectNum);
                    //正常运转时间计算
                    var TotalTime = (
                        dailyDownResult.Sum(m => m.DownTime)
                        );
                    var DashBoardTarget = resultModel.FirstOrDefault().OEEDashBoardTarget;
                    foreach (string colorName in Enum.GetNames(typeof(MachineIndexName)))
                    {
                        var resultIndexNameModel = GetMachineIndex(colorName, TotalTime, outputCount, ngCount, resultModel);
                        resultIndexNameModel.MachineName = item.FirstOrDefault().MachineName;
                        resultIndexNameModel.OEE_MachineInfo_UID = item.Key;
                        resultIndexNameModel.Station_UID = stationItem.Key;
                        resultIndexNameModel.Station_Name = stationItem.FirstOrDefault().StationName;
                        resultIndexNameModel.DashBoardTarget = DashBoardTarget;
                        ListIndexName.Add(resultIndexNameModel);
                    }
                }
                #endregion
            }

            List<MachinePieIndexModel> pieModelList = new List<MachinePieIndexModel>();
            var pieList = ListIndexName.GroupBy(p => p.Station_UID);
            foreach (var item in pieList)
            {
                //工站统计模型
                MachinePieIndexModel pieModel = new MachinePieIndexModel();
                pieModel.IsTotalStationStatic = "NO";
                pieModel.Station_UID = item.Key;
                pieModel.StationName = item.FirstOrDefault().Station_Name;
                var allEquipmentEfficiency = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency));
                pieModel.OEEValue = ForMatOEEValueToS(allEquipmentEfficiency.Sum(q => double.Parse(q.IndexCount)) / allEquipmentEfficiency.Count());
                var PerformanceRate = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.PerformanceRate));
                pieModel.PerformanceRate = ForMatOEEValueToS(PerformanceRate.Sum(q => double.Parse(q.IndexCount)) / PerformanceRate.Count());
                var AvailableRate = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.AvailableRate));
                pieModel.AvailableRate = ForMatOEEValueToS(AvailableRate.Sum(q => double.Parse(q.IndexCount)) / AvailableRate.Count());
                var FirstPassYield = item.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.FirstPassYield));
                pieModel.FirstYield = ForMatOEEValueToS(FirstPassYield.Sum(q => double.Parse(q.IndexCount)) / FirstPassYield.Count());

                var TargetArray = GetTargetArray(item.FirstOrDefault().DashBoardTarget);
                pieModel.FirstDashBoardTarget = TargetArray[0];
                pieModel.SecondDashBoardTarget = TargetArray[1];
                pieModelList.Add(pieModel);
            }

            var resultList = GetLineOrderByRule(pieModelList, serchModel.OrderByRule);
            if (serchModel.PageNum != 0)
            {
                serchModel.PageNum = serchModel.PageNum - 1;
            }
            resultList = resultList.Skip(serchModel.PagePieSize * serchModel.PageNum).Take(serchModel.PagePieSize).ToList();

            ////Title 模型
            MachinePieIndexModel pieModelStation = new MachinePieIndexModel();
            pieModelStation.IsTotalStationStatic = "YES";
            pieModelStation.OEEValue = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.OEEValue)) / pieModelList.Count());
            pieModelStation.PerformanceRate = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.PerformanceRate)) / pieModelList.Count());
            pieModelStation.AvailableRate = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.AvailableRate)) / pieModelList.Count());
            pieModelStation.FirstYield = ForMatOEEValueToP(pieModelList.Sum(p => double.Parse(p.FirstYield)) / pieModelList.Count());
            resultList.Add(pieModelStation);

            //分页模型
            MachinePieIndexModel piePageModel = new MachinePieIndexModel();
            piePageModel.IsTotalStationStatic = "PAGE";
            piePageModel.TotalCount = pieModelList.Count();
            piePageModel.TotalPage = int.Parse(Math.Ceiling((piePageModel.TotalCount * 1.0 / serchModel.PagePieSize)).ToString());
            resultList.Add(piePageModel);
            return resultList;

        }

        #endregion
        #region 停机大类

        public PagedListModel<OEE_DownTypeDTO> QueryOEE_DownType(OEE_DownTypeDTO searchModel, Page page)
        {
            int totalcount;
            var result = OEE_DownTypeRepository.QueryOEE_DownType(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<OEE_DownTypeDTO>(totalcount, result);
            return bd;
        }
        public string AddOrEditOEE_DownType(OEE_DownTypeDTO dto, bool isEdit)
        {
            string errorMessage = string.Empty;
            try
            {
                OEE_DownTimeType entityContext;
                if (dto.OEE_DownTimeType_UID == 0)
                {
                    entityContext = new OEE_DownTimeType();
                    entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                    entityContext.Sequence = dto.Sequence;
                    entityContext.Type_Name = dto.Type_Name;
                    entityContext.Is_Enable = dto.Is_Enable;
                    entityContext.Modify_UID = dto.Modify_UID;
                    entityContext.Modify_Date = dto.Modify_Date;
                    entityContext.Type_Code = dto.Type_Code;
                    entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    OEE_DownTypeRepository.Add(entityContext);
                    _unitOfWork.Commit();
                }
                else
                {
                    entityContext = OEE_DownTypeRepository.GetById(dto.OEE_DownTimeType_UID);
                    entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                    entityContext.Sequence = dto.Sequence;
                    entityContext.Type_Name = dto.Type_Name;
                    entityContext.Is_Enable = dto.Is_Enable;
                    entityContext.Modify_UID = dto.Modify_UID;
                    entityContext.Modify_Date = dto.Modify_Date;
                    entityContext.Type_Code = dto.Type_Code;
                    entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                    entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    OEE_DownTypeRepository.Update(entityContext);
                    _unitOfWork.Commit();
                }

                return "0";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }

        }

        public OEE_DownTypeDTO QueryDownTypeByUid(int OEE_DownTimeType_UID)
        {

            var result = OEE_DownTypeRepository.QueryDownTypeByUid(OEE_DownTimeType_UID);
            return result;

        }
        public string DeleteDownType(int OEE_DownTimeType_UID, int userid)
        {
            var downTimeCode = OEE_DownTypeRepository.GetFirstOrDefault(w => w.OEE_DownTimeType_UID == OEE_DownTimeType_UID);
            if (downTimeCode != null)
            {
                try
                {

                    OEE_DownTypeRepository.Delete(downTimeCode);
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "";

        }
        #endregion

        #region  FourQ Report
        /// <summary>
        /// 根据报表类型获取DT的时间
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        public List<OEEFourQDTModel> GetFourQDTTime(OEEFourQParamModel paramModel)
        {
            var resultList = new List<OEEFourQDTModel>();
            var dtResult = _OEE_MachineDailyDownRecordRepository.GetDTTimeInfo(paramModel);
            switch (paramModel.ReportType)
            {
                case "Month":
                    resultList = GetDTTimeByMonth(GetDTTimeByType(dtResult));
                    break;
                case "Week":
                    resultList = GetDTTimeByWeek(GetDTTimeByType(dtResult));
                    break;
                case "Daily":
                    resultList = GetDTTimeByType(dtResult);
                    break;
                default:
                    break;
            }
            return resultList;
        }

        /// <summary>
        /// 获取宕机的明细信息
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        public List<OEEFourQDTModel> GetFourQDTTypeDetail(OEEFourQParamModel paramModel)
        {
            switch (paramModel.ReportType)
            {
                case "Month":
                    paramModel.StartTime = GetMonthStartTime(paramModel.Param_Name);//月初
                    paramModel.EndTime = paramModel.StartTime.AddMonths(1).AddDays(-1);//月末
                    break;
                case "Week":
                    var resWeek = GetWeekStartTime(paramModel.Param_Name);
                    paramModel.StartTime = resWeek.Item1;//月初
                    paramModel.EndTime = resWeek.Item2;//月末
                    break;
                case "Daily":
                    paramModel.StartTime = GetDayStartTime(paramModel.Param_Name);//月初
                    paramModel.EndTime = GetDayStartTime(paramModel.Param_Name);//月末
                    break;
                default:
                    break;
            }

            var resultList = new List<OEEFourQDTModel>();
            var dtResult = _OEE_MachineDailyDownRecordRepository.GetDTTimeInfo(paramModel);
            return GetDTTimeDetail(dtResult);
        }


        /// <summary>
        /// 根据报表类型获取DT的时间
        /// </summary>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        public List<PaynTerChartModel> GetPaynterChartDetial(OEEFourQParamModel paramModel)
        {
            var resultList = new List<PaynTerChartModel>();
            var dtResult = _OEE_MachineDailyDownRecordRepository.GetDTTimeInfo(paramModel);
            switch (paramModel.ReportType)
            {
                case "Month":
                    resultList = GetPayterChartDTDetial(GetDTAllDetail(dtResult), "Month");
                    break;
                case "Week":
                    resultList = GetPayterChartDTDetial(GetDTAllDetail(dtResult), "Week");
                    break;
                case "Daily":
                    resultList = GetPayterChartDTDetial(GetDTAllDetail(dtResult), "Daily"); ;
                    break;
                default:
                    break;
            }
            return resultList;
        }

        public PagedListModel<OEE_ImprovementPlanDTO> QueryActionInfoByCreateDate(OEEFourQParamModel paramModel, Page page)
        {
            var dtResult = ImprovementPlanRepository.QueryActionInfoByCreateDate(paramModel, page);
            return dtResult;
        }
        private DateTime GetMonthStartTime(string param_Name)
        {
            var paramDate = param_Name.Split('_');
            var startDate = paramDate[0] + "-" + paramDate[1] + "-" + "01";
            var startMonth = Convert.ToDateTime(startDate);
            return startMonth;
        }

        private Tuple<DateTime, DateTime> GetWeekStartTime(string param_Name)
        {
            var paramDate = param_Name.Split('_');
            var dutchCultureInfo = CultureInfo.CreateSpecificCulture("nl-NL");
            var resWeek = GetFirstEndDayOfWeek(int.Parse(paramDate[0]), int.Parse(paramDate[1]), dutchCultureInfo);
            return resWeek;
        }

        private DateTime GetDayStartTime(string param_Name)
        {
            var paramDate = param_Name.Split('_');
            var resDay = paramDate[0] + "-" + paramDate[1] + "-" + paramDate[2];
            return Convert.ToDateTime(resDay);
        }

        private List<OEEFourQDTModel> GetDTTimeDetail(List<OEE_MachineDailyDownRecordDTO> paramResult)
        {
            var allDayDTList = paramResult.GroupBy(p => p.Type_Name).ToList();
            var allDTTime = paramResult.Sum(p => p.DownTime) ;
            var resultList = new List<OEEFourQDTModel>();
            foreach (var item in allDayDTList)
            {
                OEEFourQDTModel model = new OEEFourQDTModel();
                model.DTName = item.Key;
                model.TotalDTTime = item.Sum(p => p.DownTime);
                model.TotalGoalNum = allDTTime == 0 ? 0 : double.Parse(((item.Sum(p => p.DownTime) * 1.0 / (allDTTime)) * 100).ToString("f2"));
                resultList.Add(model);
            }
            return resultList.OrderByDescending(p => p.TotalDTTime).ToList();
        }
        private List<OEEFourQDTModel> GetDTTimeByType(List<OEE_MachineDailyDownRecordDTO> paramResult)
        {
            var allDayDTList = paramResult.GroupBy(p => p.DownDate).ToList();
            var resultList = new List<OEEFourQDTModel>();
            foreach (var item in allDayDTList)
            {
                OEEFourQDTModel model = new OEEFourQDTModel();
                model.DTName = item.Key.ToString("yyyy_MM_dd") + "_D";
                model.ReportMonth = item.Key.ToString("yyyy") + "_" + item.Key.ToString("MM") + "_M";
                model.ReportWeek = item.Key.ToString("yyyy") + "_" + GetWeekOfYear(item.Key).ToString() + "_W";
                model.ReportDaily = item.Key.ToString("yyyy_MM_dd") + "_D";
                model.TotalDTTime = item.Sum(p => p.DownTime);
                model.TotalGoalNum = 50;
                resultList.Add(model);
            }
            return resultList;
        }

        private List<OEEFourQDTModel> GetDTAllDetail(List<OEE_MachineDailyDownRecordDTO> paramResult)
        {
            var allDayDTList = paramResult.GroupBy(p => p.DownDate).ToList();
            var resultList = new List<OEEFourQDTModel>();
            foreach (var item in allDayDTList)
            {
                var dtDetauls = item.GroupBy(p => p.Type_Name).ToList();
                foreach (var detailitem in dtDetauls)
                {
                    OEEFourQDTModel model = new OEEFourQDTModel();
                    model.DTName = item.Key.ToString("yyyy_MM_dd") + "_D";
                    model.ReportMonth = item.Key.ToString("yyyy") + "_" + item.Key.ToString("MM") + "_M";
                    model.ReportWeek = item.Key.ToString("yyyy") + "_" + GetWeekOfYear(item.Key).ToString() + "_W";
                    model.ReportDaily = item.Key.ToString("yyyy_MM_dd") + "_D";
                    model.TotalDTTime = detailitem.Sum(p => p.DownTime);
                    model.DTTypeName = detailitem.Key;
                    model.TotalGoalNum = 50;
                    resultList.Add(model);
                }
            }

            return resultList;
        }


        private List<OEEFourQDTModel> GetDTTimeByWeek(List<OEEFourQDTModel> paramResult)
        {
            var resWeek = paramResult.GroupBy(p => p.ReportWeek);
            var resultList = new List<OEEFourQDTModel>();
            foreach (var item in resWeek)
            {
                OEEFourQDTModel weekModel = new OEEFourQDTModel();
                weekModel.DTName = item.Key;
                weekModel.TotalDTTime = item.Sum(p => p.TotalDTTime);
                weekModel.TotalGoalNum = item.FirstOrDefault().TotalGoalNum;
                resultList.Add(weekModel);
            }
            return resultList;
        }

        private List<OEEFourQDTModel> GetDTTimeByMonth(List<OEEFourQDTModel> paramResult)
        {
            var resWeek = paramResult.GroupBy(p => p.ReportMonth);
            var resultList = new List<OEEFourQDTModel>();
            foreach (var item in resWeek)
            {
                OEEFourQDTModel weekModel = new OEEFourQDTModel();
                weekModel.DTName = item.Key;
                weekModel.TotalDTTime = item.Sum(p => p.TotalDTTime);
                weekModel.TotalGoalNum = item.FirstOrDefault().TotalGoalNum;
                resultList.Add(weekModel);
            }
            return resultList;
        }

        private List<PaynTerChartModel> GetPayterChartDTDetial(List<OEEFourQDTModel> paramResult, string reportType)
        {
            var resWeek = paramResult.GroupBy(p => p.ReportMonth);
            switch (reportType)
            {
                case "Month":
                    resWeek = paramResult.GroupBy(p => p.ReportMonth);
                    break;
                case "Week":
                    resWeek = paramResult.GroupBy(p => p.ReportWeek);
                    break;
                case "Daily":
                    resWeek = paramResult.GroupBy(p => p.ReportDaily);
                    break;
                default:
                    break;
            }

            var delTypeNameList = paramResult.GroupBy(p => p.DTTypeName);
            var resultList = new List<PaynTerChartModel>();
            var typeNameList = new List<string>();
            foreach (var item in delTypeNameList)
            {
                typeNameList.Add(item.Key);
            }

            foreach (var item in resWeek)
            {
                PaynTerChartModel weekModel = new PaynTerChartModel();
                weekModel.DateCycleName = item.Key;
                weekModel.DTTypeNameList = typeNameList;
                var allDTTypeList = item.GroupBy(p => p.DTTypeName);
                var detialDTTypeList = new List<OEEFourQDTModel>();
                weekModel.DTDetailsList = new List<OEEFourQDTModel>();
                foreach (var detialItem in allDTTypeList)
                {
                    OEEFourQDTModel detialModel = new OEEFourQDTModel();
                    detialModel.DTTypeName = detialItem.Key;
                    detialModel.TotalDTTime = detialItem.Sum(p => p.TotalDTTime);
                    weekModel.DTDetailsList.Add(detialModel);
                }

                resultList.Add(weekModel);
            }

            return resultList;
        }
        /// <summary>
        /// 获取指定日期，在为一年中为第几周
        /// </summary>
        /// <param name="dt">指定时间</param>
        /// <reutrn>返回第几周</reutrn>
        private static int GetWeekOfYear(DateTime dt)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }

        /// <summary>
        /// 根据一年中的周数获取开始时间和结束时间
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekNumber"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public Tuple<DateTime, DateTime> GetFirstEndDayOfWeek(int year, int weekNumber, CultureInfo culture)
        {
            Calendar calendar = culture.Calendar;
            DateTime firstOfYear = new DateTime(year, 1, 1, calendar);
            DateTime targetDay = calendar.AddWeeks(firstOfYear, weekNumber - 1);
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

            while (targetDay.DayOfWeek != firstDayOfWeek)
            {
                targetDay = targetDay.AddDays(-1);
            }

            return Tuple.Create<DateTime, DateTime>(targetDay, targetDay.AddDays(6));
        }

        #endregion



        #region 4Q MetricInfo BaseMaintain

        #region meetType
        public PagedListModel<OEE_MeetingTypeInfoDTO> QueryOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO serchModel, Page page)
        {
            var result = OEE_MeetingTypeInfoRepository.QueryOEE_MeetingTypeInfo(serchModel, page);
            return result;
        }

        public string AddOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO serchModel)
        {
            //判断是否重复
            var isExist = OEE_MeetingTypeInfoRepository.GetMany(
                   p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID
                   && p.BG_Organization_UID == serchModel.BG_Organization_UID
                   && p.FunPlant_Organization_UID == serchModel.FunPlant_Organization_UID
                   && p.MeetingType_ID == serchModel.MeetingType_ID
                   && p.MeetingType_Name == serchModel.MeetingType_Name
               );

            if (isExist.Count() > 0)
            {
                return "添加的数据已经存在，请检查！";
            }
            else
            {
                var result = OEE_MeetingTypeInfoRepository.AddOEE_MeetingTypeInfo(serchModel);
                return result;
            }
        }

        public string DeleteOEE_MeetingTypeInfo(int meetingTypeInfo_UID)
        {
            var result = OEE_MeetingTypeInfoRepository.DeleteMeetTypeInfo(meetingTypeInfo_UID);
            return result;
        }

        public string UpdateOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO updateModel)
        {
            try
            {
                var entityContext = OEE_MeetingTypeInfoRepository.GetById(updateModel.MeetingType_UID);
                entityContext.MeetingType_ID = updateModel.MeetingType_ID;
                entityContext.MeetingType_Name = updateModel.MeetingType_Name;
                OEE_MeetingTypeInfoRepository.Update(entityContext);
                _unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "修改失败，错误信息" + ex.Message.ToString();
            }
        }
        public OEE_MeetingTypeInfoDTO GetOEE_MeetingTypeInfoById(int uid)
        {
            var entityContext = OEE_MeetingTypeInfoRepository.GetOEE_MeetingTypeInfo(uid);
            return entityContext;
        }

        public List<OEE_MeetingTypeInfoDTO> GetMeetingTypeName(int plantUid, int bgUid, int funplantUid)
        {
            var entityContext = OEE_MeetingTypeInfoRepository.GetMeetingTypeName(plantUid, bgUid, funplantUid);
            return entityContext;
        }
        #endregion

        #region Metrices
        public PagedListModel<OEE_MetricInfoDTO> QueryMetricInfo(OEE_MetricInfoDTO serchModel, Page page)
        {
            var result = OEE_MetricInfoRepository.QueryMetricInfoInfo(serchModel, page);
            return result;
        }

        public string AddMetricInfoInfo(OEE_MetricInfoDTO serchModel)
        {
            var result = OEE_MetricInfoRepository.AddMetricInfoInfo(serchModel);
            return result;
        }

        public string DeleteMetricInfo(int metricInfo_Uid)
        {
            var result = OEE_MetricInfoRepository.DeleteMetricInfo(metricInfo_Uid);
            return result;
        }

        public string UpdateMetricInfo(OEE_MetricInfoDTO updateModel)
        {
            try
            {
                var entityContext = OEE_MetricInfoRepository.GetById(updateModel.Metric_UID);
                entityContext.Metric_ID = updateModel.Metric_ID;
                entityContext.Metric_Name = updateModel.Metric_Name;
                OEE_MetricInfoRepository.Update(entityContext);
                _unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "修改失败，错误信息" + ex.Message.ToString();
            }
        }

        public OEE_MetricInfoDTO GetMetricInfoById(int uid)
        {
            var entityContext = OEE_MetricInfoRepository.GetMetricInfoInfoById(uid);
            return entityContext;
        }

        public string AddOrEditMetricInfo(OEE_MetricInfoDTO dto, bool isEdit)
        {
            string errorMessage = string.Empty;
            try
            {
                OEE_MetricInfo entityContext;
                if (dto.Metric_UID == 0)
                {
                    //判断是否重复
                    var result = OEE_MetricInfoRepository.GetMany(p => p.Plant_Organization_UID == dto.Plant_Organization_UID && p.BG_Organization_UID == dto.BG_Organization_UID && p.Metric_ID == dto.Metric_ID && p.Metric_Name == dto.Metric_Name);
                    if (result.Count() > 0)
                    {
                        return "填加的数据已经存在，请检查！";
                    }
                    else
                    {
                        entityContext = new OEE_MetricInfo();
                        entityContext.Plant_Organization_UID = dto.Plant_Organization_UID;
                        entityContext.BG_Organization_UID = dto.BG_Organization_UID;
                        entityContext.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        entityContext.Metric_ID = dto.Metric_ID;
                        entityContext.Metric_Name = dto.Metric_Name;
                        entityContext.Modified_UID = dto.Modified_UID;
                        entityContext.Modified_Date = dto.Modified_Date;
                        OEE_MetricInfoRepository.Add(entityContext);
                        _unitOfWork.Commit();
                    }
                }
                else
                {
                    entityContext = OEE_MetricInfoRepository.GetById(dto.Metric_UID);
                    entityContext.Metric_ID = dto.Metric_ID;
                    entityContext.Metric_Name = dto.Metric_Name;
                    OEE_MetricInfoRepository.Update(entityContext);
                    _unitOfWork.Commit();
                }

                return "0";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }


        }

        public List<OEE_MetricInfoDTO> GetMetricName(int plantUid, int bgUid, int funplantUid)
        {
            var entityContext = OEE_MetricInfoRepository.GetMetricName(plantUid, bgUid, funplantUid);
            return entityContext;
        }

        #endregion

        #region improvementplan

        public string AddImprovementPlan(OEE_ImprovementPlanDTO dto)
        {
            string errorMessage = string.Empty;
            try
            {
                var result = ImprovementPlanRepository.AddImprovementPlan(dto);
                return result;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }
        }

        public string UpdateImprovementPlan(OEE_ImprovementPlanDTO dto)
        {
            string errorMessage = string.Empty;
            try
            {
                var result = ImprovementPlanRepository.UpdateImprovementPlan(dto);
                return result;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return ex.Message;
            }
        }

        public PagedListModel<OEE_ImprovementPlanDTO> QueryImprovementPlanInfo(OEE_ImprovementPlanDTO serchModel, Page page)
        {
            var result = ImprovementPlanRepository.QueryImprovementPlanInfo(serchModel, page);
            return result;
        }

        public OEE_ImprovementPlanDTO GetImprovementPlanById(int improvementPlanId)
        {
            var result = ImprovementPlanRepository.GetImprovementPlanById(improvementPlanId);
            return result;
        }

        public string DeleteImpeovementPlanById(int improvementPlanId)
        {
            var result = ImprovementPlanRepository.DeleteImpeovementPlanById(improvementPlanId);
            return result;
        }

        public void UpdateActionStatus()
        {
            var currentDate = DateTime.Now.AddDays(-1);
            var actionList = ImprovementPlanRepository.GetMany(p => (p.Status == 1 || p.Status == 2)).ToList();
            foreach (var item in actionList)
            {
                try
                {
                    if (item.DirDueDate != null&& item.DirDueDate.Value.ToString("yyyy-MM--dd")== "1900-01-01")
                    {
                        var entityContext = ImprovementPlanRepository.GetById(item.ImprovementPlan_UID);
                        entityContext.Status = 3;
                        ImprovementPlanRepository.Update(entityContext);
                        _unitOfWork.Commit();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion

        #endregion

        public OEE_RealStatusReport QueryRealStatusReport(OEE_ReprortSearchModel serchModel)
        {
            var result = _OEE_EveryDayMachineRepository.QueryRealStatusReport(serchModel);
            // if (result.OEE_RealStatusList.Count == 0) return result;
            //for (int i = 0; i < result.OEE_RealStatusList.Count; i++)
            // {
            //     result.OEE_RealStatusList[i].MachineID = i;
            // }
            return result;
        }
    }
}
public class Factory : IComparer<string>
{
    private Factory() { }
    public static IComparer<string> Comparer
    {
        get { return new Factory(); }
    }
    public int Compare(string x, string y)
    {
        return x.Length == y.Length ? x.CompareTo(y) : x.Length - y.Length;





    }
}





