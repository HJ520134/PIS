using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels.Settings;
using PDMS.Model.ViewModels;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels.Common;
using PDMS.Model.ViewModels.Fixture;

namespace PDMS.WebAPI.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<System_FunctionSub, SubFunction>();

            Mapper.CreateMap<System_Users, SystemUserDTO>();
            Mapper.CreateMap<System_Function, SystemFunctionDTO>();
            Mapper.CreateMap<System_BU_M, SystemBUMDTO>();
            Mapper.CreateMap<SystemBUMDTO, System_BU_M>();
            Mapper.CreateMap<SystemBUDDTO, System_BU_D>();
            Mapper.CreateMap<System_BU_D, SystemBUDDTO>();
            Mapper.CreateMap<BUDetailModelGet, System_BU_D>();
            Mapper.CreateMap<System_BU_D, BUDetailModelGet>();
            Mapper.CreateMap<System_BU_D, CustomBUDDTO>();
            Mapper.CreateMap<System_User_Business_Group, SystemUserBusinessGroupDTO>();

            Mapper.CreateMap<PageUnauthorizedElementEntity, PageUnauthorizedElement>();
            Mapper.CreateMap<System_Plant, SystemPlantDTO>();
            Mapper.CreateMap<System_Role, SystemRoleDTO>();
            Mapper.CreateMap<System_FunctionSub, SystemFunctionSubDTO>();
            Mapper.CreateMap<System_Organization, SystemOrgDTO>();
            Mapper.CreateMap<System_OrganizationBOM, SystemOrgBomDTO>();
            Mapper.CreateMap<System_User_Role, SystemUserRoleDTO>();
            Mapper.CreateMap<FlowChart_Master, FlowChartMasterDTO>();
            Mapper.CreateMap<FlowChartMasterDTO, FlowChart_Master>();

            Mapper.CreateMap<FlowChartDetailDTO, FlowChart_Detail>();
            Mapper.CreateMap<FlowChart_Detail, FlowChartDetailDTO>();
            Mapper.CreateMap<List<FlowChartDetailDTO>, List<FlowChart_Detail>>();


            Mapper.CreateMap<FlowChartMgDataDTO, FlowChart_MgData>();
            Mapper.CreateMap<FlowChart_MgData, FlowChartMgDataDTO>();
            Mapper.CreateMap<FlowChart_Detail, FlowChartDetailAndMGDataDTO>();


            Mapper.CreateMap<FlowChartDetailDTO, FlowChartDetailTempDTO>();
            Mapper.CreateMap<FlowChartDetailTempDTO, FlowChartDetailDTO>();
            Mapper.CreateMap<FlowChartMgDataDTO, FlowChartMgDataTempDTO>();
            Mapper.CreateMap<FlowChartMgDataTempDTO, FlowChartMgDataDTO>();
            Mapper.CreateMap<List<SystemFunctionPlantDTO>, List<System_Function_Plant>>();

            Mapper.CreateMap<System_Project, SystemProjectDTO>();

            Mapper.CreateMap<Enumeration, EnumVM>();
            Mapper.CreateMap<System_Function_Plant, SystemFunctionPlantDTO>();
            Mapper.CreateMap<SystemFunctionPlantDTO, System_Function_Plant>();
            Mapper.CreateMap<Product_Input, ProductDataDTO>();
            Mapper.CreateMap<Product_Input, ProductDataViewDTO>();

            Mapper.CreateMap<PagedListModel<ProductDataDTO>, ProductDataList>();
            Mapper.CreateMap<PagedListModel<ProductDataDTO>, List<ProductDataDTO>>();
            Mapper.CreateMap<PagedListModel<ProductDataDTO>, List<ProductDataItem>>();
            Mapper.CreateMap<ZeroProcessDataSearch, ProcessDataSearch>();

            Mapper.CreateMap<System_View_Column, SystemViewColumnDTO>();
            Mapper.CreateMap<System_User_View, SystemUserViewDTO>();
            //Mapper.CreateMap<System_User_View, SystemUserViewDTO>();
            Mapper.CreateMap<QualityAssurance_ExceptionType, ExceptionTypeVM>();

            Mapper.CreateMap<QualityAssurance_InputDetail, QAInputDetailVM>();
            Mapper.CreateMap<PPForQAInterfaceDTO, PPForQAInterface>();
            Mapper.CreateMap<List<PPForQAInterfaceDTO>, List<PPForQAInterface>>();
            Mapper.CreateMap<List<PPForQAInterface>, List<PPForQAInterfaceDTO>>();


            Mapper.CreateMap<QAInputModifyDTO, QAInputModifyVM>();
            Mapper.CreateMap<System_Users, CustomUserInfoVM>();

            Mapper.CreateMap<ExceptionTypeTempVM, QualityAssurance_ExceptionType_Temp>();

            Mapper.CreateMap<OQC_InputMasterVM, OQC_InputMaster>();
            Mapper.CreateMap<ExceptionTypeWithFlowchartVM, ExceptionTypeWithFlowchart>();
            Mapper.CreateMap<Product_Input, Electrical_Board_DT>();
            Mapper.CreateMap<Product_Input_Location, Electrical_Board_DT>();

            Mapper.CreateMap<Electrical_Board_DT, EboardVM>();
            Mapper.CreateMap<Product_Rework_Info, ProductReworkInfoVM>();
            Mapper.CreateMap<Product_Input, ProductDataVM>();
            Mapper.CreateMap<Product_Input_Location, ProductDataVM>();
            Mapper.CreateMap<ProductData_Input, ProductDataVM>();
            Mapper.CreateMap<FlowChart_PC_MH_Relationship, FlowChartPCMHRelationshipDTO>();
            Mapper.CreateMap<Equipment_Info, EquipmentInfoDTO>();
            Mapper.CreateMap<EQP_UserTable, EQPUserTableDTO>();
            Mapper.CreateMap<FlowChart_Detail, FlowChartBomGet>();
            Mapper.CreateMap<FlowChart_Detail, FlowChartDetailAndMgData>();
            Mapper.CreateMap<Material_Info, MaterialInfoDTO>();
            Mapper.CreateMap<Enumeration, EnumerationDTO>();
            Mapper.CreateMap<QualityAssurance_InputMaster, QualityAssurance_InputMasterDTO>();
            Mapper.CreateMap<QualityAssurance_InputMaster_History, QualityAssuranceInputMasterHistoryDTO>();
            Mapper.CreateMap<OQC_InputMaster, OQC_InputMasterDTO>();
            Mapper.CreateMap<OQC_InputMaster_History, OQCInputMasterHistoryDTO>();
            Mapper.CreateMap<ExceptionTypeWithFlowchart, ExceptionTypeWithFlowchartDTO>();
            //Mapper.CreateMap<Flowchart_Detail_ME, FlowchartDetailMEDTO>();
            //Mapper.CreateMap<Flowchart_Detail_ME_Equipment, FlowchartDetailMEEquipmentDTO>();

            Mapper.CreateMap<Warehouse, WarehouseDTO>();
            Mapper.CreateMap<Warehouse_Storage, WarStorageDto>();
            Mapper.CreateMap<Warehouse_Storage, WarehouseStorageDTO>();
            Mapper.CreateMap<Storage_Inbound, StorageInboundDTO>();
            Mapper.CreateMap<Material_Inventory, MaterialInventoryDTO>();
            Mapper.CreateMap<Storage_Outbound_D, StorageOutboundDDTO>();
            Mapper.CreateMap<Storage_Outbound_D, OutBoundDetail>();
            Mapper.CreateMap<EQP_Type, EQPTypeDTO>();
            Mapper.CreateMap<EQP_PowerOn, EQPPowerOnDTO>();
            Mapper.CreateMap<EQP_Material, EQPMaterialDTO>();
            Mapper.CreateMap<EQP_Forecast_PowerOn, EQPForecastPowerOnDTO>();
            //Mapper.CreateMap<Production_Schedul, ProductionSchedulDTO>();
            Mapper.CreateMap<Material_Reason, MaterialReasonDTO>();
            Mapper.CreateMap<System_BU_D_Org, SystemBUD_OrgDTO>();

            Mapper.CreateMap<System_Language, SystemLanguageDTO>();
            Mapper.CreateMap<System_LocaleStringResource, SystemLocaleStringResourceDTO>();
            Mapper.CreateMap<Workshop, WorkshopDTO>();
            Mapper.CreateMap<Process_Info, Process_InfoDTO>();
            Mapper.CreateMap<DefectCode_RepairSolution, DefectRepairSolutionDTO>();

            Mapper.CreateMap<WorkStation, WorkStationDTO>();
            Mapper.CreateMap<Production_Line, Production_LineDTO>();
            Mapper.CreateMap<Fixture_Machine, FixtureMachineDTO>();
            Mapper.CreateMap<Fixture_DefectCode, Fixture_DefectCodeDTO>();
            Mapper.CreateMap<Fixture_RepairSolution, Fixture_RepairSolutionDTO>();
            Mapper.CreateMap<Vendor_Info, VendorInfoDTO>();
            Mapper.CreateMap<Maintenance_Plan, MaintenancePlanDTO>();
            Mapper.CreateMap<Fixture_Maintenance_Profile, FixtureMaintenanceProfileDTO>();
            Mapper.CreateMap<Fixture_User_Workshop, FixtureUserWorkshopDTO>();
            Mapper.CreateMap<Repair_Location, RepairLocationDTO>();
            Mapper.CreateMap<Fixture_M, FixtureDTO>();
            Mapper.CreateMap<Fixture_Repair_M, Fixture_Repair_MDTO>();
            Mapper.CreateMap<Fixture_Repair_D, Fixture_Repair_DDTO>();
            Mapper.CreateMap<Fixture_Repair_D_Defect, Fixture_Repair_D_DefectDTO>();
            Mapper.CreateMap<ReportByRepair, ReportByStatusAnalisis>();
            Mapper.CreateMap<Fixture_Part, Fixture_PartDTO>();
            Mapper.CreateMap<Fixture_Warehouse_Storage, FixturePartWarehouseStorageDTO>();
            Mapper.CreateMap<Fixture_Part_Setting_M, Fixture_Part_Setting_MDTO>();
            Mapper.CreateMap<Fixture_Part_Setting_D, Fixture_Part_Setting_DDTO>();
            Mapper.CreateMap<System_LocalizedProperty, System_LocalizedPropertyDTO>();
            Mapper.CreateMap<Fixture_Part_Demand_M, Fixture_Part_Demand_MDTO>();
            Mapper.CreateMap<Fixture_Part_Order_M, SubmitFixturePartOrderVM>();
            Mapper.CreateMap<Fixture_Part_Demand_D, Fixture_Part_Demand_DDTO>();
            Mapper.CreateMap<Fixture_Part_Demand_Summary_D, Fixture_Part_Demand_Summary_DDTO>();
            Mapper.CreateMap<Fixture_Part_Demand_Summary_M, Fixture_Part_Demand_Summary_MDTO>();
            Mapper.CreateMap<Fixture_Resume, Fixture_Resume_History>();
            Mapper.CreateMap<DemissionRateAndWorkSchedule, DemissionRateAndWorkScheduleDTO>();
            Mapper.CreateMap<ProcessIDTransformConfig, ProcessIDTransformConfigDTO>();
            Mapper.CreateMap<MES_StationDataRecord, MES_StationDataRecordDTO>();
            Mapper.CreateMap<ModelLineHR, ModelLineHRDTO>();
            Mapper.CreateMap<GL_ShiftTime, GL_ShiftTimeDTO>();
            Mapper.CreateMap<GL_LineShiftPerf, GL_LineShiftPerfDTO>();
            Mapper.CreateMap<GL_Station, GL_StationDTO>();
            Mapper.CreateMap<GL_WIPShiftOutput, GL_WIPShiftOutputDTO>();
            //Mapper.CreateMap<List<ProcessIDTransformConfig>, List<ProcessIDTransformConfigDTO>>();
            //Mapper.CreateMap<List<ProcessIDTransformConfigDTO>, List<ProcessIDTransformConfig>>();

            // Golden Line Entity and DTO
            Mapper.CreateMap<GL_Line, GL_LineDTO>();
            Mapper.CreateMap<GL_BuildPlan, GL_BuildPlanDTO>();
            Mapper.CreateMap<RP_Flowchart_Detail_ME_Equipment, RP_ME_D_Equipment>();
            Mapper.CreateMap<GL_LineShiftResposibleUser, GL_LineShiftResposibleUserDTO>();

            //PlayBoard 播放看板
            Mapper.CreateMap<PlayBoard_View, PlayBoard_ViewDTO>();
            Mapper.CreateMap<PlayBoard_Setting, PlayBoard_SettingDTO>();
            Mapper.CreateMap<PlayBoard_PlayTime, PlayBoard_PlayTimeDTO>();
            Mapper.CreateMap<System_Users, PlayBoard_PlayUsetDTO>();
         
            //OEE
            Mapper.CreateMap<OEE_MachineInfo, OEE_MachineInfoDTO>();
            Mapper.CreateMap<OEE_DefectCodeDailyNum, OEE_DefectCodeDailyNumDTO>();
            Mapper.CreateMap<OEE_StationDefectCode, OEE_StationDefectCodeDTO>();

            //PVD
            Mapper.CreateMap<Fixture_Part_UseTimes, Fixture_Part_UseTimesDTO>();
            Mapper.CreateMap<GL_Rest, GL_RestTimeDTO>();
            //Fixture_Return
            Mapper.CreateMap<Fixture_Return_M, Fixture_Return_MDTO>();
            Mapper.CreateMap<Fixture_Return_D, Fixture_Return_DDTO>();
            //Exption
            Mapper.CreateMap<Exception_Dept, ExceptionDTO>();
            Mapper.CreateMap<Exception_Code, ExceptionDTO>();
            Mapper.CreateMap<Exception_Email, ExceptionDTO>();
        }
    }
}