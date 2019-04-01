using AutoMapper;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Fixture;
using PDMS.Model.ViewModels.ProcessIDTraConfig;
using PDMS.Model.ViewModels.ProductionPlanning;

namespace PDMS.WebAPI.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<SystemUserDTO, System_Users>();
            Mapper.CreateMap<SystemFunctionDTO, System_Function>();
            Mapper.CreateMap<PageUnauthorizedElement, PageUnauthorizedElementEntity>();
            Mapper.CreateMap<SystemPlantDTO, System_Plant>();
            Mapper.CreateMap<SystemRoleDTO, System_Role>();
            Mapper.CreateMap<FunctionWithSubs, System_Function>();
            Mapper.CreateMap<SystemFunctionSubDTO, System_FunctionSub>();
            Mapper.CreateMap<SystemOrgDTO, System_Organization>();
            Mapper.CreateMap<SystemOrgBomDTO, System_OrganizationBOM>();
            Mapper.CreateMap<SystemUserRoleDTO, System_User_Role>();
            Mapper.CreateMap<ProductDataViewDTO, Product_Input>();
            Mapper.CreateMap<ProductDataDTO, Product_Input>();
            Mapper.CreateMap<EditEnumDTO, Enumeration>();
            Mapper.CreateMap<ExceptionTypeVM, QualityAssurance_ExceptionType>();

            Mapper.CreateMap<QAInputDetailVM, QualityAssurance_InputDetail>();
        	Mapper.CreateMap<FlowChartPCMHRelationshipDTO, FlowChart_PC_MH_Relationship>();

            Mapper.CreateMap<QualityAssurance_ExceptionType_Temp, ExceptionTypeTempVM>();

            Mapper.CreateMap<OQC_InputMaster, OQC_InputMasterVM>();

            Mapper.CreateMap<ExceptionTypeWithFlowchart, ExceptionTypeWithFlowchartVM>();
            Mapper.CreateMap<NoticeDTO, Notice>();
            Mapper.CreateMap<ProductDataItem, Product_Input>();
            Mapper.CreateMap<QualityAssurance_InputMasterDTO, QualityAssurance_InputMaster>();
            Mapper.CreateMap<QualityAssuranceInputMasterHistoryDTO, QualityAssurance_InputMaster_History>();
            Mapper.CreateMap<OQC_InputMasterDTO, OQC_InputMaster>();
            Mapper.CreateMap<OQCInputMasterHistoryDTO, OQC_InputMaster_History>();
            Mapper.CreateMap<ExceptionTypeWithFlowchartDTO, ExceptionTypeWithFlowchart>();
            //Mapper.CreateMap<ProductionSchedulNPIDTO, Production_Schedul_NPI>();
            Mapper.CreateMap<EQPTypeDTO, EQP_Type>();
            Mapper.CreateMap<EQPPowerOnDTO, EQP_PowerOn>();
            Mapper.CreateMap<EQPMaterialDTO, EQP_Material>();
            Mapper.CreateMap<EQPForecastPowerOnDTO, EQP_Forecast_PowerOn>();
            //Mapper.CreateMap<ProductionSchedulDTO, Production_Schedul>();
            //Mapper.CreateMap<CurrentStaffDTO, Current_Staff>();
            //Mapper.CreateMap<DemissionRateAndWorkScheduleDTO, DemissionRateAndWorkSchedule>();
            Mapper.CreateMap<SystemBUD_OrgDTO, System_BU_D_Org>();
            //Mapper.CreateMap<ProductRequestStaffDTO, Product_RequestStaff>();
            //Mapper.CreateMap<ProductEquipmentQTYDTO, Product_Equipment_QTY>();
            //Mapper.CreateMap<ActiveManPowerVM, Product_RequestStaff>();
            //Mapper.CreateMap<ProductEquipmentQTYDTO, Product_Equipment_QTY>();
            Mapper.CreateMap<SystemLanguageDTO, System_Language>();
            Mapper.CreateMap<SystemLocaleStringResourceDTO, System_LocaleStringResource>();
            Mapper.CreateMap<ProductDataItem, Product_Input_Location>();
            Mapper.CreateMap<WorkshopDTO, Workshop>();
            Mapper.CreateMap<Process_InfoDTO, Process_Info>();

            Mapper.CreateMap<DefectRepairSolutionDTO, DefectCode_RepairSolution>();

            Mapper.CreateMap<WorkStationDTO, WorkStation>();
            Mapper.CreateMap<Production_LineDTO, Production_Line>();
            Mapper.CreateMap<FixtureMachineDTO, Fixture_Machine>();
            Mapper.CreateMap<Fixture_DefectCodeDTO, Fixture_DefectCode>();
            Mapper.CreateMap<Fixture_RepairSolutionDTO, Fixture_RepairSolution>();
            Mapper.CreateMap<Fixture_Repair_MDTO, Fixture_Repair_M>();
            Mapper.CreateMap<Fixture_Repair_DDTO, Fixture_Repair_D>();
            Mapper.CreateMap<Fixture_Repair_D_DefectDTO, Fixture_Repair_D_Defect>();
            Mapper.CreateMap<Fixture_PartDTO, Fixture_Part>(); 
            Mapper.CreateMap<FixturePartWarehouseStorageDTO, Fixture_Warehouse_Storage > ();
            Mapper.CreateMap<Fixture_Part_Setting_MDTO, Fixture_Part_Setting_M>();
            Mapper.CreateMap<Fixture_Part_Setting_DDTO, Fixture_Part_Setting_D>();
            Mapper.CreateMap<System_LocalizedPropertyDTO, System_LocalizedProperty>();
            Mapper.CreateMap<Fixture_Part_Demand_MDTO, Fixture_Part_Demand_M>();
            Mapper.CreateMap<SubmitFixturePartOrderVM, Fixture_Part_Order_M>();
            Mapper.CreateMap<Fixture_Part_Demand_DDTO, Fixture_Part_Demand_D>();
            Mapper.CreateMap<Fixture_Part_Demand_Summary_DDTO, Fixture_Part_Demand_Summary_D>();
            Mapper.CreateMap<Fixture_Part_Demand_Summary_MDTO, Fixture_Part_Demand_Summary_M>();
            Mapper.CreateMap<Fixture_Resume_History, Fixture_Resume>();
            Mapper.CreateMap<DemissionRateAndWorkScheduleDTO, DemissionRateAndWorkSchedule>();
            Mapper.CreateMap<ProcessIDTransformConfigDTO, ProcessIDTraConfigVM>();
            Mapper.CreateMap<ProcessIDTransformConfigDTO, ProcessIDTransformConfig>();
            Mapper.CreateMap<MES_StationDataRecordDTO, MES_StationDataRecord>();
            Mapper.CreateMap<ModelLineHRDTO, ModelLineHR>();
            Mapper.CreateMap<GL_ShiftTimeDTO, GL_ShiftTime>();
            Mapper.CreateMap<GL_LineShiftPerfDTO, GL_LineShiftPerf>();
            Mapper.CreateMap<GL_StationDTO, GL_Station>();
            Mapper.CreateMap<GL_WIPShiftOutputDTO, GL_WIPShiftOutput>();

            // Golden Line Entity and DTO
            Mapper.CreateMap<GL_LineDTO, GL_Line>();
            Mapper.CreateMap<GL_BuildPlanDTO, GL_BuildPlan>();
            Mapper.CreateMap<GL_LineShiftResposibleUserDTO, GL_LineShiftResposibleUser>();

            //PlayBoard 播放看板
            Mapper.CreateMap<PlayBoard_ViewDTO, PlayBoard_View>();
            Mapper.CreateMap<PlayBoard_SettingDTO, PlayBoard_Setting>();
            Mapper.CreateMap<PlayBoard_PlayTimeDTO, PlayBoard_PlayTime>();

            Mapper.CreateMap<OEE_MachineInfoDTO, OEE_MachineInfo>();
            Mapper.CreateMap<OEE_DefectCodeDailyNumDTO, OEE_DefectCodeDailyNum>();
            Mapper.CreateMap<OEE_StationDefectCodeDTO, OEE_StationDefectCode>();
            Mapper.CreateMap<PlayBoard_PlayUsetDTO, System_Users>();

            //PVD
            Mapper.CreateMap<Fixture_Part_UseTimesDTO, Fixture_Part_UseTimes>();

            Mapper.CreateMap<GL_RestTimeDTO, GL_Rest>();
            //Fixture_Return
            Mapper.CreateMap<Fixture_Return_MDTO, Fixture_Return_M>();
            Mapper.CreateMap<Fixture_Return_DDTO, Fixture_Return_D>();
            //Exption
            Mapper.CreateMap<ExceptionDTO, Exception_Dept>();
            Mapper.CreateMap<ExceptionDTO, Exception_Code>();
            Mapper.CreateMap<ExceptionDTO, Exception_Email>();
        }
    }
}