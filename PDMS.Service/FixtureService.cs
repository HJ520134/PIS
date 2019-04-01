using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Model.EntityDTO;
using System.Configuration;
using System.Collections;
using static PDMS.Common.Constants.StructConstants;
using PDMS.Common.Constants;
using PDMS.Common.Common;
using PDMS.Model.ViewModels.Fixture;

namespace PDMS.Service
{

    public interface IFixtureService
    {
        List<SystemProjectDTO> GetCurrentOPType(int parentOrg_UID, int organization_UID);
        List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes = "");
        List<FixtureStatuDTO> GetFixtureStatuDTO();
        PagedListModel<FixtureDTO> QueryFixture(FixtureDTO searchModel, Page page);
        PagedListModel<FixtureDTO> QueryFixtureStatusMoniter(FixtureDTO searchModel, Page page);
        List<FixtureDTO> GetFixtureStatusMoniterListBySearch(FixtureDTO searchModel);
        List<FixtureDTO> GetFixtureStatusMoniterListBySelected(FixtureDTO searchModel);
        FixtureDTO QueryFixtureByUid(int fixture_UID);
        List<FixtureDTO> FixtureList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<FixtureDTO> FixtureList(FixtureDTO searchModel);
        string BatchEnableFixturematerial(FixtureDTO searchModel, string IsEnable);

        string BatchEnablePartFixturematerial(string Fixture_M_UIDs, string IsEnable, int AccountID);

        #region                 karl   start
        #region                供应商维护--------------------START
        PagedListModel<VendorInfoDTO> QueryVendorInfo(VendorInfoDTO searchModel, Page page);
        IList<VendorInfoDTO> GetVendorInfoList(VendorInfoDTO searc);
        List<SystemOrgDTO> GetOrgByParant(int Parant_UID, int type);
        string AddOrEditVendorInfo(VendorInfoDTO dto);
        Vendor_Info QueryVendorInSingle(int Vendor_Info_UID);
        string DeleteVendorInfo(int Vendor_Info_UID);
        List<SystemOrgDTO> QueryAllPlants(int PLANT_UID, string levle);
        List<VendorInfoDTO> QueryAllVendorInfos();
        string InsertVendorInfoItem(List<VendorInfoDTO> dtolist);
        List<VendorInfoDTO> DoVIExportFunction(string uids);
        List<OrgBomDTO> GetAllOrgBom();
        #endregion                供应商维护--------------------END

        #region                    保养计划设定维护-------------------START
        PagedListModel<MaintenancePlanDTO> QueryMaintenancePlan(MaintenancePlanDTO searchModel, Page page);
        Maintenance_Plan QueryMaintenanceSingle(int MaintenancePlanDTO);
        string AddOrEditMaintenancePlan(MaintenancePlanDTO dto);
        string DeleteMaintenancePlan(int Maintenance_Plan_UID);
        string InsertMaintenancePlanItem(List<MaintenancePlanDTO> dtolist);
        List<MaintenancePlanDTO> QueryAllMaintenancePlans();
        List<MaintenancePlanDTO> DoMPExportFunction(string uids);
        List<MaintenancePlanDTO> DoAllMPExportFunction(MaintenancePlanDTO searchModel);
        #endregion              保养计划设定维护---------------------END
        #region                    治具保养设定维护-------------------START
        PagedListModel<FixtureMaintenanceProfileDTO> QueryFixtureMaintenanceProfile(FixtureMaintenanceProfileDTO searchModel, Page page);
        FixtureMaintenanceProfileDTO QueryFixtureMaintenanceProfileSingle(int Fixture_Maintenance_Profile_UID);
        string AddOrEditFixtureMaintenanceProfile(FixtureMaintenanceProfileDTO dto);
        string DeleteFixtureMaintenanceProfile(int Fixture_Maintenance_Profile_UID);
        string InsertFixtureMaintenanceProfileItem(List<FixtureMaintenanceProfileDTO> dtolist);
        List<FixtureMaintenanceProfileDTO> QueryAllFixtureMaintenanceProfiles();
        List<FixtureMaintenanceProfileDTO> DoFMPExportFunction(string uids);
        List<FixtureMaintenanceProfileDTO> DoAllFMPExportFunction(FixtureMaintenanceProfileDTO search);
        List<string> GetFixtureNoByFunPlant(int BG_Organization_UID, int FunPlant_Organization_UID);
        List<MaintenancePlanDTO> GetMaintenancePlanByFilters(int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type);
        List<FixtureDTO> QueryAllFixture();
        #endregion              治具保养设定维护---------------------END

        #region                    用户车间设定-------------------START
        PagedListModel<FixtureUserWorkshopDTO> QueryFixtureUserWorkshop(FixtureUserWorkshopDTO searchModel, Page page);
        FixtureUserWorkshopDTO QueryFixtureUserWorkshopSingle(int Fixture_User_Workshop_UID);
        string AddOrEditFixtureUserWorkshop(FixtureUserWorkshopDTO dto);
        string DeleteFixtureUserWorkshop(int Fixture_User_Workshop_UID);
        string InsertFixtureUserWorkshopItem(List<FixtureUserWorkshopDTO> dtolist);
        List<FixtureUserWorkshopDTO> QueryAllFixtureUserWorkshops();
        List<SystemUserDTO> GetUserByOp(int BG_Organization_UID, int FunPlant_Organization_UID);
        List<SystemUserDTO> GetUserByOpAPILY(int BG_Organization_UID, int FunPlant_Organization_UID);
        List<WorkshopDTO> QueryAllWorkshops();
        List<WorkshopDTO> GetWorkshopByNTID(int FunPlant_Organization_UID);
        List<FixtureUserWorkshopDTO> DoFUWExportFunction(string uids);
        List<FixtureUserWorkshopDTO> DoAllFUWExportFunction(FixtureUserWorkshopDTO searchModel);
        List<SystemUserDTO> QueryAllUsers();
        #endregion              用户车间设定---------------------END

        #region                 维修地点设定--------------------------START
        PagedListModel<RepairLocationDTO> QueryRepairLocation(RepairLocationDTO searchModel, Page page);
        Repair_Location QueryRepairLocationSingle(int Repair_Location_UID);
        string DeleteRepairLocation(int Repair_Location_UID);
        string AddOrEditRepairLocation(RepairLocationDTO dto);
        string InsertRepairLocation(List<RepairLocationDTO> dtolist);
        List<RepairLocationDTO> DoRLExportFunction(string uids);
        List<RepairLocationDTO> DoAllRLExportFunction(RepairLocationDTO searchModel);
        List<RepairLocationDTO> QueryAllRepairLocations();
        #endregion                 维修地点设定------------------------END
        #region                       治具领用-----------------START
        PagedListModel<Fixture_Totake_MDTO> QueryFixtureTotake(Fixture_Totake_MDTO searchModel, Page page);
        Fixture_Totake_MDTO QueryFixtureTotakeSingle(int Fixture_Totake_M_UID);
        List<fixture> GetfixtureList(int Fixture_Totake_M_UID);
        List<FixtureDTO> GetFixtureByWorkshop(int Account_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name);
        List<SystemUserDTO> GetUserByWorkshop(int Account_UID);
        string AddOrEditFixtureTotake(Fixture_Totake_MDTO dto);
        string UpdateFixture(List<FixtureDTO> fixturelist, int useruid, string Totake_NO, DateTime Totake_Date);
        List<Fixture_Totake_MDTO> DoFTExportFunction(string uids);
        List<Fixture_Totake_MDTO> DoAllFTExportFunction(Fixture_Totake_MDTO search);
        PagedListModel<Fixture_Totake_DDTO> QueryFixtureTotakeDetail(Fixture_Totake_DDTO searchModel, Page page);
        #endregion                 治具领用-------------------END
        #endregion           karl    end


        List<Vendor_InfoDTO> GetVendor_InfoList(int Plant_Organization_UID, int BG_Organization_UID);
        List<Production_LineDTO> GetProductionLineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<FixtureSystemUserDTO> GetFixtureSystemUser(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<WorkshopDTO> GetWorkshopList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        IList<WorkshopDTO> GetWorkshopListByQuery(WorkshopModelSearch search);
        List<WorkStationDTO> GetWorkstationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        IList<WorkStationDTO> GetWorkstationListByQuery(WorkStationModelSearch search);
        List<Process_InfoDTO> GetProcess_InfoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<SystemProjectDTO> GetProjectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        Production_LineDTO GetProductionLineDTO(int Production_Line_UID);
        string AddDefectRepair(DefectCode_RepairSolution DefectRepair);
        DefectCode_RepairSolution QueryDefectRepairSingle(int uid);
        bool IsDefectRepairExist(DefectRepairSearch search);
        List<Fixture_DefectCodeDTO> GetDefectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Fixture_RepairSolutionDTO> GetRepairSoulutions(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        string EditDefectRepair(DefectCode_RepairSolution workshop);
        string DeleteDefectRepair(int uid);
        PagedListModel<DefectRepairSolutionDTO> QueryDefectRepairs(DefectRepairSearch search, Page page);
        IList<DefectRepairSolutionDTO> GetDefectRepairList(DefectRepairSearch search);

        List<FixtureMachineDTO> GetFixtureMachineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, int Production_Line_UID);
        FixtureMachineDTO GetFixtureMachineByUid(int Fixture_Machine_UID);
        string AddOrEditFixtureM(FixtureDTO dto, bool isEdit);
        string DeleteFixtureM(int Fixture_M_UID);

        List<FixtureDTO> DoExportFixtureReprot(string Fixture_M_UIDs);

        #region 工站维护
        bool IsWorkStationExist(WorkStationModelSearch search);
        PagedListModel<WorkStationDTO> QueryWorkStations(WorkStationModelSearch search, Page page);
        WorkStation QueryWorkStationSingle(int uid);
        string AddWorkStation(WorkStation workshop);
        string EditWorkStation(WorkStation workshop);
        string DeleteWorkStation(int uid);
        IEnumerable<WorkStationDTO> DoExportWorkStation(string uids);
        IEnumerable<DefectRepairSolutionDTO> DoExportDefectRepair(string uids);
        string InsertDefectRepairSolutions(List<DefectCode_RepairSolution> list);
        string InsertWorkStaions(List<WorkStation> list);
        List<WorkStationDTO> QueryAllWorkStations();
        #endregion


        #region 治具履历查询 -------------------Add by ROck 2017-10-03-------------start
        Dictionary<int, string> GetFixtureStatus();
        PagedListModel<FixtureResumeSearchVM> FixtureResumeSearchVM(FixtureResumeSearchVM searchVM, Page page);
        ViewResumeByUID QueryFixtureResumeByUID(int Fixture_Resume_UID, int Fixture_M_UID);
        List<FixtureResumeSearchVM> ExportFixtureResumeByUID(string uids);

        List<FixtureResumeSearchVM> DoAllExportFixtureResumeReprot(FixtureResumeSearchVM searchVM);
        #endregion 治具履历查询 -------------------Add by ROck 2017-10-03-------------start

        #region 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------Start
        Dictionary<string, string> GetMaintenanceStatus(string Maintenance_Type);
        PagedListModel<NotMaintenanceSearchVM> QueryFixtureNotMaintained(NotMaintenanceSearchVM search, Page page);
        List<NotMaintenanceSearchVM> ExportFixtureNotMaintainedByUID(string uids, string hidDate);
        List<NotMaintenanceSearchVM> DoAllExportFixtureNotMaintainedReprot(NotMaintenanceSearchVM searchVM);
        #endregion 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------End

        #region 产线维护
        bool IsProduction_LineExist(Production_LineModelSearch search);
        PagedListModel<Production_LineDTO> QueryProduction_Lines(Production_LineModelSearch search, Page page);
        IList<Production_LineDTO> GetProductionLineList(Production_LineModelSearch search);
        Production_Line QueryProduction_LineSingle(int uid);
        string AddProduction_Line(Production_Line workshop);
        string EditProduction_Line(Production_Line workshop);
        string DeleteProduction_Line(int uid);
        IEnumerable<Production_LineDTO> DoExportProduction_Line(string uids);
        List<Production_LineDTO> QueryAllProduction_Lines();
        string InsertProduction_Lines(List<Production_Line> list);
        #endregion

        #region 设备机台维护
        bool IsFixture_MachineExist(Fixture_MachineModelSearch search);
        PagedListModel<FixtureMachineDTO> QueryFixture_Machines(Fixture_MachineModelSearch search, Page page);
        IList<FixtureMachineDTO> GetFixtureMachineList(Fixture_MachineModelSearch search);
        Fixture_Machine QueryFixture_MachineSingle(int uid);
        string AddFixture_Machine(Fixture_Machine machine);
        string EditFixture_Machine(Fixture_Machine machine);
        string DeleteFixture_Machine(int uid);
        IEnumerable<FixtureMachineDTO> DoExportFixture_Machine(string uids);
        List<FixtureMachineDTO> QueryAllFixture_Machines();
        string InsertFixture_Machines(List<Fixture_Machine> list);
        //TODO 2018/09/18 steven add 取设备信息檔功能
        List<EquipmentInfoDTO> GetEquipmentInfoList(int Plant_Organization_UID, int? BG_Organization_UID, int? FunPlant_Organization_UID);

        /// <summary>
        /// 从Equipment_Info 同步数据到Fixture_Machine
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        int BatchAppendExcute(int UserID, int? Plant_Organization_UID, int? BG_Organization_UID, int? FunPlant_Organization_UID);
        #endregion
        #region 治具保养
        PagedListModel<Fixture_Maintenance_RecordDTO> QueryFixtureMaintenance(Fixture_Maintenance_RecordDTO searchModel, Page page);
        Fixture_Maintenance_RecordDTO QueryFixtureMaintenanceByUid(int Fixture_Maintenance_Record_UID);
        List<Fixture_Maintenance_RecordDTO> DoExportFixtureMaintenanceReprot(string Fixture_Maintenance_Record_UIDs);
        List<Fixture_Maintenance_RecordDTO> DoAllExportFixtureMaintenanceReprot(Fixture_Maintenance_RecordDTO search);

        IEnumerable<Fixture_Maintenance_RecordDTO> GetFixtureMaintenance(string Fixture_Maintenance_Record_UIDs, int straus);
        List<Fixture_Maintenance_RecordDTO> GetFixtureMaintenanceList(Fixture_Maintenance_RecordDTO dto, int straus);
        string UpdateFixture_Maintenance_Record(string fixture_Maintenance_Record_UIDs, int NTID, string personNumber, string personName, DateTime date, int straus, int CurrentUserID);
        List<FixtureMaintenance_PlanDTO> GetFixtureMaintenance_Plan(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type);
        string CreateFixture_Maintenance_Records(Fixture_Maintenance_RecordDTO dto);
        // string CreateFixture_Maintenance_Record(List<Fixture_Maintenance_RecordDTO> recordDTOs);
        #endregion 治具保养

        #region 设备维修
        List<Fixture_Repair_MDTO> GetFixture_Repair_MDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        Fixture_Repair_MDTO GetFixture_Repair_MDTOByID(int Fixture_Repair_M_UID);

        string GetSentRepairNameById(string SentOut_Number);
        PagedListModel<FixtureRepairDTO> QueryFixtureRepairs(FixtureRepairSearch searchModel, Page page);
        IList<FixtureRepairDTO> GetFixtureRepairList(FixtureRepairSearch searchModel);

        string AddFixtureRepair(Fixture_Repair_MDTO dto);
        string EditFixtureRepair(Fixture_Repair_MDTO dto);
        Fixture_Repair_MDTO GetFixtureRepairByRepairNo(string repairNo);
        IList<FixtureDTO> GetFixtureListFixtureRepair(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name);
        IList<FixtureDTO> GetFixtureListFixtureRepairByUniqueID(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string uniqueID);
        List<DefectRepairSolutionDTO> GetDefectCodeReapairSolutionDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        FixtureDTO GetFixtureByFixtureUniqueID(string fixtureUniqueID);
        FixtureRepairItem QueryFixtureRepairByNo(string Repair_NO);
        List<RepairLocationDTO> GetRepairLocationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        IEnumerable<FixtureRepairDTO> DoExportFixtureRepair(string uids);
        #endregion

        #region 治具异常原因维护
        bool IsFixture_DefectCodeExist(Fixture_DefectCodeModelSearch search);
        PagedListModel<Fixture_DefectCodeDTO> QueryFixture_DefectCodes(Fixture_DefectCodeModelSearch search, Page page);
        IList<Fixture_DefectCodeDTO> GetFixtureDefectCodeList(Fixture_DefectCodeModelSearch search);
        Fixture_DefectCode QueryFixture_DefectCodeSingle(int uid);
        string AddFixture_DefectCode(Fixture_DefectCode machine);
        string EditFixture_DefectCode(Fixture_DefectCode machine);
        string DeleteFixture_DefectCode(int uid);
        IEnumerable<Fixture_DefectCodeDTO> DoExportFixture_DefectCode(string uids);
        List<Fixture_DefectCodeDTO> QueryAllFixture_DefectCodes();
        string InsertFixture_DefectCodes(List<Fixture_DefectCode> dtolist);
        Fixture_DefectCodeDTO GetFixture_DefectCode(string DefectCode_ID, int Plant_Organization_UID, int BG_Organization_UID);
        #endregion


        #region 维修对策维护
        bool IsFixture_RepairSolutionExist(Fixture_RepairSolutionModelSearch search);
        PagedListModel<Fixture_RepairSolutionDTO> QueryFixture_RepairSolutions(Fixture_RepairSolutionModelSearch search, Page page);
        IList<Fixture_RepairSolutionDTO> GetRepairSolutionList(Fixture_RepairSolutionModelSearch search);
        Fixture_RepairSolution QueryFixture_RepairSolutionSingle(int uid);
        string AddFixture_RepairSolution(Fixture_RepairSolution machine);
        string EditFixture_RepairSolution(Fixture_RepairSolution machine);
        string DeleteFixture_RepairSolution(int uid);
        IEnumerable<Fixture_RepairSolutionDTO> DoExportFixture_RepairSolution(string uids);
        List<Fixture_RepairSolutionDTO> QueryAllFixture_RepairSolutions();
        List<DefectRepairSolutionDTO> QueryAllDefectCode_RepairSolution();
        string InsertFixture_RepairSolutions(List<Fixture_RepairSolution> dtolist);
        #endregion

        #region 车间维护
        bool IsWorkshopExist(WorkshopModelSearch search);
        string InsertWorkshops(List<Workshop> list);
        #endregion

        #region 制程维护
        bool IsProcess_InfoExist(Process_InfoModelSearch search);
        List<Process_InfoDTO> QueryAllProcess_Infos();
        string InsertProcess_Infos(List<Process_Info> dtolist);
        #endregion

        #region 异常原因群组设定
        PagedListModel<DefectCode_GroupDTO> QueryDefectCode_Group(DefectCode_GroupDTO searchModel, Page page);

        List<DefectCode_GroupDTO> DoExportDefectCode_GroupReprot(string DefectCode_Group_UIDs);
        List<DefectCode_GroupDTO> DoAllExportDefectCode_GroupReprot(DefectCode_GroupDTO searchModel);

        string AddDefectCode_Group(DefectCode_GroupDTO dto);

        List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID);

        string InsertDefectCode_Group(List<DefectCode_GroupDTO> dtolist);
        string DeleteDefectCode_Group_UID(string DefectCode_Group_UIDs);

        #endregion 异常原因群组设定

        #region 治具异常原因维护
        string DeleteFixtureDefectCode_Setting_UID(string FixtureDefectCode_Setting_UIDs);
        PagedListModel<FixtureDefectCode_SettingDTO> QueryFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO searchModel, Page page);

        List<FixtureDefectCode_SettingDTO> DoAllExportFixtureDefectCode_SettingReprot(FixtureDefectCode_SettingDTO searchModel);

        List<FixtureDefectCode_SettingDTO> DoExportFixtureDefectCode_SettingReprot(string FixtureDefectCode_Setting_UIDs);

        string AddFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO dto);
        List<Fixture_DefectCodeDTO> GetDefectCodeByGroup(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID);
        List<Fixture_DefectCodeDTO> GetDefectCodesByPlant(int Plant_Organization_UID);

        List<FixtureDefectCode_SettingDTO> GetFixtureDefectCode_SettingByPlant(int Plant_Organization_UID);
        List<FixtureDTO> GetFixture_MByPlant(int Plant_Organization_UID);
        string InsertFixtureDefectCode_Setting(List<FixtureDefectCode_SettingDTO> dtolist);
        List<Production_LineDTO> GetProduction_LineByPlant(int Plant_Organization_UID);

        string Insertfixture_M(List<FixtureDTO> dtolist);
        string Updatefixture_MAPI(List<FixtureDTO> dtolist);

        DefectCode_GroupDTO GetDefectCode_GroupByUID(int DefectCode_Group_UID);
        FixtureDefectCode_SettingDTO GetFixtureDefectCode_SettingDTOByUID(int FixtureDefectCode_Setting_UID);

        string EditDefectCode_Group(DefectCode_GroupDTO dto);
        string EditFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO dto);
        DefectCode_GroupDTO DefectCode_Group(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID);
        #endregion 治具异常原因维护

        #region 治具配件维护
        bool IsFixture_PartExist(Fixture_PartModelSearch search);
        PagedListModel<Fixture_PartDTO> QueryFixture_Parts(Fixture_PartModelSearch search, Page page);
        IList<Fixture_PartDTO> GetFixturePartList(Fixture_PartModelSearch search);
        Fixture_Part QueryFixture_PartSingle(int uid);
        string AddFixture_Part(Fixture_Part fixturePart);
        string EditFixture_Part(Fixture_Part fixtrePart);
        string DeleteFixture_Part(int uid);
        IEnumerable<Fixture_PartDTO> DoExportFixture_Part(string uids);
        List<Fixture_PartDTO> QueryAllFixture_Parts();
        string InsertFixture_Parts(List<Fixture_Part> list);
        #endregion

        #region 报表-治具维修次数查询 Add by Rock 2017-10-31---------------------------Start
        PagedListModel<ReportByRepair> QueryReportByRepair(ReportByRepair model, Page page);

        List<ReportByRepair> ExportReportByRepair(ReportByRepair model);
        #endregion 报表-治具维修次数查询 Add by Rock 2017-10-31---------------------------End

        #region 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ Start
        PagedListModel<ReportByRepair> QueryReportByRepairPerson(ReportByRepair model, Page page);
        List<ReportByRepair> ExportReportByRepairPersonValid(ReportByRepair model);


        #endregion 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ End

        #region 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ Start
        PagedListModel<ReportByRepair> QueryReportByPage(ReportByRepair model, Page page);
        List<ReportByRepair> ExportReportByPageValid(ReportByRepair model);

        #endregion 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ End

        #region 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ Start
        PagedListModel<ReportByRepair> QueryFixtureReportByDetail(ReportByRepair model, Page page);
        List<ReportByRepair> ExportReportByDetailValid(ReportByRepair model);

        #endregion 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ End

        #region 报表-治具间维修时间分析报表 Add by Rock 2017-11-28------------------------ Start
        PagedListModel<ReportByRepair> QueryFixtureReportByAnalisis(ReportByRepair model, Page page);
        List<ReportByRepair> ExportReportByAnalisisValid(ReportByRepair model);
        #endregion 报表-治具间维修时间分析报表 Add by Rock 2017-11-28------------------------ End

        #region 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------Start
        List<string> GetFixtureNoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        PagedListModel<ReportByRepair> QueryFixtureReportByStatus(ReportByRepair model, Page page);
        List<ReportByRepair> ExportReportByStatusValid(ReportByRepair model);

        #endregion 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------End

        #region 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------Start
        PagedListModel<ReportByStatusAnalisis> QueryFixtureReportByStatusAnalisis(ReportByRepair model, Page page);
        List<ReportByStatusAnalisis> ExportReportByStatusAnalisisValid(ReportByRepair model);

        #endregion 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------End

        #region 报表-FMT Dashboard Add by Rock 2017-12-11----------------Start
        PagedListModel<Batch_ReportByStatus> QueryFixtureReportByFMT(Batch_ReportByStatus model, Page page);
        List<Batch_ReportByStatus> QueryQueryFixtureReportByFMTDetail(int Process_Info_UID, string startDate, string endDate);
        List<Batch_ReportByStatus> ExportReportByFMTValid(Batch_ReportByStatus model);
        #endregion 报表-FMT Dashboard Add by Rock 2017-12-11----------------End

        #region 主执行程序 -------------------Add By Rock 2017-12-18---------Start
        void ExecFMTDashboard(string functionName, int Plant_Organization_UID, int System_Schedule_UID);
        #endregion -------------------Add By Rock 2017-12-18---------End

        #region 治具归还
        List<string> FetchFixtureTotakeforFixtureReturn(int plant_ID, int op_type, int funPlant);
        /// <summary>
        /// 根据治具领取号获取所有take_UID
        /// </summary>
        /// <param name="Take_NO"></param>
        /// <returns></returns>
        List<Fixture_Taken_InfoDTO> FetchAllFixturesBasedTakeNo(string Take_NO);
        /// <summary>
        /// 根据fixtureTake_UID返回治具Fixture_Taken_InfoDTO集合
        /// </summary>
        /// <param name="FixtureTake_UID"></param>
        /// <returns></returns>
        List<Fixture_Taken_InfoDTO> FetchFixtureTakenInfo(List<int> FixtureTake_UIDs);
        string AddFixtureRetrun(Fixture_Return_MDTO dto);
        string AddFixtureRetrunD(Fixture_Return_DDTO dto);
        PagedListModel<Fixture_Return_Index> QueryFixtureToReturn(Fixture_Return_MDTO searchModel, Page page);
        Fixture_Return_MDTO QueryFixtureReturnUid(int uid);
        string FixtureReturnUpdatePost(Fixture_Return_MDTO dto);
        List<Fixture_Return_MDTO> FixtureReturnDetail(int uid);
        string DelFixtureReturnM(int uid);
        string GetCurrentReturnNub();
        List<Fixture_Taken_InfoDTO> FetchAllFixturesBasedReturnMUID(int uid);
        string UpdateFixtureRetrun(Fixture_Return_MDTO dto);
        string UpdateFixtureRetrunD(Fixture_Return_DDTO dto);
        List<Fixture_Return_Index> ExportFixtrueReturn2Excel(Fixture_Return_MDTO dto);
        #endregion


    }
    public class FixtureService : IFixtureService
    {
        #region 所有仓储变量
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IFixtureRepository fixtureRepository;
        private readonly IFixture_ResumeRepository fixture_ResumeRepository;

        #region     karl  start
        private readonly IVendorInfoRepository vendorInfoRepository;
        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly IMaintenancePlanRepository maintenancePlanRepository;
        private readonly IFixtureMaintenanceProfileRepository fixtureMaintenanceProfileRepository;
        private readonly IFixtureUserWorkshopRepository fixtureUserWorkshopRepository;
        private readonly IRepairLocationRepository repairLocationRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IWorkshopRepository workshopRepository;
        private readonly IFixture_Totake_MRepository fixture_Totake_MRepository;
        private readonly IFixture_Totake_DRepository fixture_Totake_DRepository;
        #endregion     karl end

        private readonly IDefectRepairRepository defectRepairRepository;
        private readonly IWorkStationRepository workStationRepository;
        private readonly IProcess_InfoRepository processInfoRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IProduction_LineRepository production_LineRepository;
        private readonly IFixture_MachineRepository fixture_MachineRepository;
        private readonly IFixture_DefectCodeRepository fixture_DefectCodeRepository;
        private readonly IFixture_Maintenance_RecordRepository fixture_Maintenance_RecordRepository;
        private readonly IFixture_RepairSolutionRepository fixture_RepairSolutionRepository;
        private readonly IFixtureRepairRepository fixtureRepairRepository;

        private readonly IDefectCode_GroupRepository defectCode_GroupRepository;
        private readonly IFixtureDefectCode_SettingRepository fixtureDefectCode_SettingRepository;
        private readonly IFixture_Repair_DRepository fixtureRepairDRepository;
        private readonly IFixture_Repair_D_DefectRepository fixtureRepairDDefectRepository;

        private readonly IFixture_PartRepository fixturePartRepository;

        //TODO 2018/09/18 steven add 取设备信息檔功能
        private readonly IEquipmentInfoRepository equipmentInfoRepository;
        //
        private readonly IFixture_Return_MRepository fixture_Return_MRepository;

        #endregion
        #region 构造函数
        public FixtureService(
            ISystemProjectRepository systemProjectRepository,
            IEnumerationRepository enumerationRepository,
            IFixtureRepository fixtureRepository,
            IDefectRepairRepository defectRepairRepository,
            IUnitOfWork unitOfWork,
            IWorkStationRepository workStationRepository,
            IProcess_InfoRepository processInfoRepository,
            IVendorInfoRepository vendorInfoRepository,
            ISystemOrgRepository systemOrgRepository,
            IMaintenancePlanRepository maintenancePlanRepository,
            IFixtureMaintenanceProfileRepository fixtureMaintenanceProfileRepository,
            IFixtureUserWorkshopRepository fixtureUserWorkshopRepository,
            IRepairLocationRepository repairLocationRepository,
            IProduction_LineRepository production_LineRepository,
            IFixture_MachineRepository fixture_MachineRepository,
            IFixture_DefectCodeRepository fixture_DefectCodeRepository,
            IFixture_Maintenance_RecordRepository fixture_Maintenance_RecordRepository,
            IFixture_RepairSolutionRepository fixture_RepairSolutionRepository,
            ISystemUserRepository systemUserRepository,
            IWorkshopRepository workshopRepository,
            IFixtureRepairRepository fixtureRepairRepository,
            IFixture_ResumeRepository fixture_ResumeRepository,
            IDefectCode_GroupRepository defectCode_GroupRepository,
            IFixtureDefectCode_SettingRepository fixtureDefectCode_SettingRepository,
            IFixture_Totake_MRepository fixture_Totake_MRepository,
            IFixture_Totake_DRepository fixture_Totake_DRepository,
            IFixture_Repair_DRepository fixtureRepairDRepository,
            IFixture_Repair_D_DefectRepository fixtureRepairDDefectRepository,
            IFixture_PartRepository fixturePartRepository,
            IEquipmentInfoRepository equipmentInfoRepository,
            IFixture_Return_MRepository fixture_Return_MRepository)

        {
            this.systemProjectRepository = systemProjectRepository;
            this.enumerationRepository = enumerationRepository;
            this.fixtureRepository = fixtureRepository;
            this.vendorInfoRepository = vendorInfoRepository;
            this.unitOfWork = unitOfWork;
            this.systemOrgRepository = systemOrgRepository;
            this.defectRepairRepository = defectRepairRepository;
            this.workStationRepository = workStationRepository;
            this.processInfoRepository = processInfoRepository;
            this.maintenancePlanRepository = maintenancePlanRepository;
            this.fixtureMaintenanceProfileRepository = fixtureMaintenanceProfileRepository;
            this.fixtureUserWorkshopRepository = fixtureUserWorkshopRepository;
            this.repairLocationRepository = repairLocationRepository;
            this.production_LineRepository = production_LineRepository;
            this.fixture_MachineRepository = fixture_MachineRepository;
            this.fixture_DefectCodeRepository = fixture_DefectCodeRepository;
            this.fixture_Maintenance_RecordRepository = fixture_Maintenance_RecordRepository;
            this.fixture_RepairSolutionRepository = fixture_RepairSolutionRepository;

            this.fixtureRepairRepository = fixtureRepairRepository;

            this.systemUserRepository = systemUserRepository;
            this.workshopRepository = workshopRepository;
            this.fixture_ResumeRepository = fixture_ResumeRepository;
            this.defectCode_GroupRepository = defectCode_GroupRepository;
            this.fixtureDefectCode_SettingRepository = fixtureDefectCode_SettingRepository;
            this.fixture_Totake_MRepository = fixture_Totake_MRepository;
            this.fixture_Totake_DRepository = fixture_Totake_DRepository;
            this.fixtureRepairDRepository = fixtureRepairDRepository;
            this.fixtureRepairDDefectRepository = fixtureRepairDDefectRepository;
            this.fixturePartRepository = fixturePartRepository;
            this.equipmentInfoRepository = equipmentInfoRepository;
            this.fixture_Return_MRepository = fixture_Return_MRepository;
        }
        #endregion

        public List<SystemProjectDTO> GetCurrentOPType(int parentOrg_UID, int organization_UID)
        {
            var bud = systemProjectRepository.GetCurrentFixtureOPType(parentOrg_UID, organization_UID).ToList();
            return bud;
        }

        public List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes = "")
        {
            var bud = fixtureRepository.GetFunPlantByOPType(Optype, Optypes);
            return bud;

        }
        public List<FixtureSystemUserDTO> GetFixtureSystemUser(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixtureRepository.GetFixtureSystemUser(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;

        }

        public List<FixtureStatuDTO> GetFixtureStatuDTO()
        {
            var bud = fixtureRepository.GetFixtureStatuDTO();
            return bud;
        }

        public PagedListModel<FixtureDTO> QueryFixture(FixtureDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryFixture(searchModel, page, out totalcount);
            return new PagedListModel<FixtureDTO>(totalcount, result);
        }

        public PagedListModel<FixtureDTO> QueryFixtureStatusMoniter(FixtureDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryFixtureStatusMoniter(searchModel, page, out totalcount);
            return new PagedListModel<FixtureDTO>(totalcount, result);
        }

        public List<FixtureDTO> GetFixtureStatusMoniterListBySearch(FixtureDTO searchModel)
        {
            var list = fixtureRepository.QueryFixtureStatusMoniterBySearch(searchModel);
            return list;
        }
        public List<FixtureDTO> GetFixtureStatusMoniterListBySelected(FixtureDTO searchModel)
        {
            var list = fixtureRepository.QueryFixtureStatusMoniterBySelected(searchModel);
            return list;
        }

        public FixtureDTO QueryFixtureByUid(int fixture_UID)
        {
            var bud = fixtureRepository.QueryFixtureByUid(fixture_UID);
            return bud;

        }
        public List<FixtureDTO> FixtureList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {


            var bud = fixtureRepository.FixtureList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;
        }

        #region                 karl  start

        #region                  供应商维护-----------------------START
        public PagedListModel<VendorInfoDTO> QueryVendorInfo(VendorInfoDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = vendorInfoRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<VendorInfoDTO>(totalcount, result);
        }
        public IList<VendorInfoDTO> GetVendorInfoList(VendorInfoDTO search)
        {
            var result = vendorInfoRepository.QueryVenderInfo(search);
            return result.ToList();
        }

        public List<SystemOrgDTO> GetOrgByParant(int Parant_UID, int type)
        {
            var bud = vendorInfoRepository.GetOrgByParant(Parant_UID, type).ToList();
            return bud;
        }

        public string AddOrEditVendorInfo(VendorInfoDTO dto)
        {
            try
            {
                int? BG_Organization_UID = dto.BG_Organization_UID == 0 ? null : dto.BG_Organization_UID;
                if (dto.Vendor_Info_UID == 0)
                {
                    var hasitem = vendorInfoRepository.GetMany(m => m.Plant_Organization_UID == dto.Plant_Organization_UID
                    & m.BG_Organization_UID == BG_Organization_UID & m.Vendor_ID == dto.Vendor_ID).FirstOrDefault();
                    if (hasitem != null)
                        return "供应商【" + dto.Vendor_ID + "已存在】,不可重复添加";
                    DateTime currentTimd = DateTime.Now;
                    Vendor_Info VI = new Vendor_Info();
                    VI.Plant_Organization_UID = dto.Plant_Organization_UID;
                    VI.BG_Organization_UID = BG_Organization_UID;
                    VI.Vendor_ID = dto.Vendor_ID;
                    VI.Vendor_Name = dto.Vendor_Name;
                    VI.Is_Enable = dto.Is_Enable;
                    VI.Created_UID = dto.Created_UID;
                    VI.Created_Date = currentTimd;
                    VI.Modified_UID = dto.Created_UID;
                    VI.Modified_Date = currentTimd;
                    vendorInfoRepository.Add(VI);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var hasitem = vendorInfoRepository.GetMany(m => m.Plant_Organization_UID == dto.Plant_Organization_UID
                        & m.BG_Organization_UID == BG_Organization_UID & m.Vendor_ID == dto.Vendor_ID & m.Vendor_Info_UID != dto.Vendor_Info_UID).FirstOrDefault();
                    if (hasitem != null)
                        return "供应商【" + dto.Vendor_ID + "已存在】,不可修改";
                    var VI = vendorInfoRepository.GetFirstOrDefault(m => m.Vendor_Info_UID == dto.Vendor_Info_UID);
                    VI.Vendor_ID = dto.Vendor_ID;
                    VI.Vendor_Name = dto.Vendor_Name;
                    VI.Is_Enable = dto.Is_Enable;
                    VI.Modified_UID = dto.Created_UID;
                    VI.Modified_Date = DateTime.Now;

                    vendorInfoRepository.Update(VI);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "更新供应商失败:" + e.Message;
            }
        }

        public Vendor_Info QueryVendorInSingle(int Vendor_Info_UID)
        {
            var bud = vendorInfoRepository.GetById(Vendor_Info_UID);
            return bud;
        }

        public string DeleteVendorInfo(int Vendor_Info_UID)
        {
            try
            {
                var VI = vendorInfoRepository.GetById(Vendor_Info_UID);
                vendorInfoRepository.Delete(VI);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除供应商信息失败:" + e.Message;
            }

        }

        public List<SystemOrgDTO> QueryAllPlants(int PLANT_UID, string levle)
        {
            var plants = systemOrgRepository.GetAll().Where(m => m.Organization_ID.StartsWith(levle)).ToList();
            if (PLANT_UID != 0)
                plants = systemOrgRepository.GetAll().Where(m => m.Organization_UID == PLANT_UID & m.Organization_ID.StartsWith(levle)).ToList();
            List<SystemOrgDTO> dtoList = new List<SystemOrgDTO>();
            foreach (var item in plants)
            {
                dtoList.Add(AutoMapper.Mapper.Map<SystemOrgDTO>(item));
            }
            return dtoList;
        }
        public List<VendorInfoDTO> QueryAllVendorInfos()
        {
            var vendorinfos = vendorInfoRepository.GetAll().ToList();
            List<VendorInfoDTO> dtoList = new List<VendorInfoDTO>();
            foreach (var item in vendorinfos)
            {
                dtoList.Add(AutoMapper.Mapper.Map<VendorInfoDTO>(item));
            }
            return dtoList;
        }

        public string InsertVendorInfoItem(List<VendorInfoDTO> dtolist)
        {
            return vendorInfoRepository.InsertItem(dtolist);
        }
        public List<VendorInfoDTO> DoVIExportFunction(string uids)
        {
            var bd = vendorInfoRepository.DoExportFunction(uids);
            return bd;
        }

        public List<OrgBomDTO> GetAllOrgBom()
        {
            var bud = vendorInfoRepository.GetAllOrgBom().ToList();
            return bud;
        }
        #endregion            供应商维护-----------------END
        #region                  保养计划设定维护--------------------START
        public PagedListModel<MaintenancePlanDTO> QueryMaintenancePlan(MaintenancePlanDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = maintenancePlanRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<MaintenancePlanDTO>(totalcount, result);
        }

        public Maintenance_Plan QueryMaintenanceSingle(int Maintenance_Plan_UID)
        {
            var bud = maintenancePlanRepository.GetById(Maintenance_Plan_UID);
            return bud;
        }
        public string AddOrEditMaintenancePlan(MaintenancePlanDTO dto)
        {
            try
            {
                int? FunPlant_Organization_UID = dto.FunPlant_Organization_UID == 0 ? null : dto.FunPlant_Organization_UID;
                if (dto.Maintenance_Plan_UID == 0)
                {
                    var hasitem = maintenancePlanRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                    & m.FunPlant_Organization_UID == FunPlant_Organization_UID
                    & m.Cycle_ID == dto.Cycle_ID & m.Cycle_Interval == dto.Cycle_Interval & m.Cycle_Unit == dto.Cycle_Unit).FirstOrDefault();
                    if (hasitem != null)
                        return "此笔信息已存在,不可新增";
                    DateTime currentTimd = DateTime.Now;
                    Maintenance_Plan MP = new Maintenance_Plan();
                    MP.Plant_Organization_UID = dto.Plant_Organization_UID;
                    MP.BG_Organization_UID = dto.BG_Organization_UID;
                    MP.FunPlant_Organization_UID = FunPlant_Organization_UID;
                    MP.Maintenance_Type = dto.Maintenance_Type;
                    MP.Cycle_ID = dto.Cycle_ID;
                    MP.Cycle_Interval = (int)dto.Cycle_Interval;
                    MP.Cycle_Unit = dto.Cycle_Unit;
                    MP.Lead_Time = (int)dto.Lead_Time;
                    MP.Start_Date = dto.Start_Date;
                    MP.Tolerance_Time = (int)dto.Tolerance_Time;
                    MP.Last_Execution_Date = dto.Last_Execution_Date;
                    MP.Next_Execution_Date = dto.Next_Execution_Date;
                    MP.Is_Enable = dto.Is_Enable;
                    MP.Created_UID = dto.Created_UID;
                    MP.Created_Date = currentTimd;
                    MP.Modified_UID = dto.Created_UID;
                    MP.Modified_Date = currentTimd;
                    maintenancePlanRepository.Add(MP);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var hasitem = maintenancePlanRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                        & m.FunPlant_Organization_UID == FunPlant_Organization_UID & m.Maintenance_Plan_UID != dto.Maintenance_Plan_UID
                        & m.Cycle_ID == dto.Cycle_ID & m.Cycle_Interval == dto.Cycle_Interval & m.Cycle_Unit == dto.Cycle_Unit).FirstOrDefault();
                    if (hasitem != null)
                        return "此笔信息已存在,不可修改";
                    var MP = maintenancePlanRepository.GetFirstOrDefault(m => m.Maintenance_Plan_UID == dto.Maintenance_Plan_UID);
                    MP.Maintenance_Type = dto.Maintenance_Type;
                    MP.Cycle_ID = dto.Cycle_ID;
                    MP.Cycle_Interval = (int)dto.Cycle_Interval;
                    MP.Cycle_Unit = dto.Cycle_Unit;
                    MP.Lead_Time = (int)dto.Lead_Time;
                    MP.Start_Date = dto.Start_Date;
                    MP.Tolerance_Time = (int)dto.Tolerance_Time;
                    MP.Last_Execution_Date = dto.Last_Execution_Date;
                    MP.Next_Execution_Date = dto.Next_Execution_Date;
                    MP.Is_Enable = dto.Is_Enable;
                    MP.Modified_UID = dto.Created_UID;
                    MP.Modified_Date = DateTime.Now;

                    maintenancePlanRepository.Update(MP);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "更新保养计划设定失败:" + e.Message;
            }
        }
        public string DeleteMaintenancePlan(int Maintenance_Plan_UID)
        {
            try
            {
                var MP = maintenancePlanRepository.GetById(Maintenance_Plan_UID);
                maintenancePlanRepository.Delete(MP);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除保养计划设定失败:" + e.Message;
            }

        }
        public string InsertMaintenancePlanItem(List<MaintenancePlanDTO> dtolist)
        {
            return maintenancePlanRepository.InsertItem(dtolist);
        }
        public List<MaintenancePlanDTO> QueryAllMaintenancePlans()
        {
            var vendorinfos = maintenancePlanRepository.GetAll().ToList();
            List<MaintenancePlanDTO> dtoList = new List<MaintenancePlanDTO>();
            foreach (var item in vendorinfos)
            {
                dtoList.Add(AutoMapper.Mapper.Map<MaintenancePlanDTO>(item));
            }
            return dtoList;
        }
        public List<MaintenancePlanDTO> DoMPExportFunction(string uids)
        {
            var bd = maintenancePlanRepository.DoExportFunction(uids);
            return bd;
        }

        public List<MaintenancePlanDTO> DoAllMPExportFunction(MaintenancePlanDTO searchModel)
        {
            var bd = maintenancePlanRepository.DoAllMPExportFunction(searchModel);
            return bd;

        }

        #endregion            保养计划设定维护----------------------END

        #region                  治具保养设定维护--------------------START
        public PagedListModel<FixtureMaintenanceProfileDTO> QueryFixtureMaintenanceProfile(FixtureMaintenanceProfileDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = fixtureMaintenanceProfileRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<FixtureMaintenanceProfileDTO>(totalcount, result);
        }

        public FixtureMaintenanceProfileDTO QueryFixtureMaintenanceProfileSingle(int Fixture_Maintenance_Profile_UID)
        {
            var bud = fixtureMaintenanceProfileRepository.GetByUId(Fixture_Maintenance_Profile_UID);
            return bud;
        }
        public string AddOrEditFixtureMaintenanceProfile(FixtureMaintenanceProfileDTO dto)
        {
            try
            {
                int? FunPlant_Organization_UID = dto.FunPlant_Organization_UID == 0 ? null : dto.FunPlant_Organization_UID;
                if (dto.Fixture_Maintenance_Profile_UID == 0)
                {
                    var hasitem = fixtureMaintenanceProfileRepository.GetMany(m => m.Maintenance_Plan_UID == dto.Maintenance_Plan_UID &
                    m.Fixture_NO == dto.Fixture_NO & m.BG_Organization_UID == dto.BG_Organization_UID & m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                    if (hasitem != null && hasitem.Count() > 0)
                        return "此项治具保养数据已经存在,不可重复添加";
                    DateTime currentTimd = DateTime.Now;
                    Fixture_Maintenance_Profile FMP = new Fixture_Maintenance_Profile();
                    FMP.Plant_Organization_UID = dto.Plant_Organization_UID;
                    FMP.BG_Organization_UID = dto.BG_Organization_UID;
                    FMP.FunPlant_Organization_UID = FunPlant_Organization_UID;
                    FMP.Fixture_NO = dto.Fixture_NO;
                    FMP.Maintenance_Plan_UID = dto.Maintenance_Plan_UID;
                    FMP.Is_Enable = dto.Is_Enable;
                    FMP.Created_UID = dto.Created_UID;
                    FMP.Created_Date = currentTimd;
                    FMP.Modified_UID = dto.Created_UID;
                    FMP.Modified_Date = currentTimd;
                    fixtureMaintenanceProfileRepository.Add(FMP);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {

                    var hasitem = fixtureMaintenanceProfileRepository.GetMany(m => m.Maintenance_Plan_UID == dto.Maintenance_Plan_UID &
                        m.Fixture_NO == dto.Fixture_NO & m.BG_Organization_UID == dto.BG_Organization_UID & m.FunPlant_Organization_UID == FunPlant_Organization_UID
                        & m.Fixture_Maintenance_Profile_UID != dto.Fixture_Maintenance_Profile_UID);
                    if (hasitem != null && hasitem.Count() > 0)
                        return "修改后的治具保养数据已经存在,不可修改";
                    var FMP = fixtureMaintenanceProfileRepository.GetFirstOrDefault(m => m.Fixture_Maintenance_Profile_UID == dto.Fixture_Maintenance_Profile_UID);
                    FMP.Fixture_NO = dto.Fixture_NO;
                    FMP.Maintenance_Plan_UID = dto.Maintenance_Plan_UID;
                    FMP.Is_Enable = dto.Is_Enable;
                    FMP.Modified_UID = dto.Created_UID;
                    FMP.Modified_Date = DateTime.Now;

                    fixtureMaintenanceProfileRepository.Update(FMP);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "更新保养计划设定失败:" + e.Message;
            }
        }
        public string DeleteFixtureMaintenanceProfile(int Fixture_Maintenance_Profile_UID)
        {
            try
            {
                var FMP = fixtureMaintenanceProfileRepository.GetById(Fixture_Maintenance_Profile_UID);
                fixtureMaintenanceProfileRepository.Delete(FMP);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除治具保养设定失败:" + e.Message;
            }

        }
        public string InsertFixtureMaintenanceProfileItem(List<FixtureMaintenanceProfileDTO> dtolist)
        {
            return fixtureMaintenanceProfileRepository.InsertItem(dtolist);
        }
        public List<FixtureMaintenanceProfileDTO> QueryAllFixtureMaintenanceProfiles()
        {
            var vendorinfos = fixtureMaintenanceProfileRepository.GetAll().ToList();
            List<FixtureMaintenanceProfileDTO> dtoList = new List<FixtureMaintenanceProfileDTO>();
            foreach (var item in vendorinfos)
            {
                dtoList.Add(AutoMapper.Mapper.Map<FixtureMaintenanceProfileDTO>(item));
            }
            return dtoList;
        }
        public List<FixtureMaintenanceProfileDTO> DoFMPExportFunction(string uids)
        {
            var bd = fixtureMaintenanceProfileRepository.DoExportFunction(uids);
            return bd;
        }
        public List<FixtureMaintenanceProfileDTO> DoAllFMPExportFunction(FixtureMaintenanceProfileDTO search)
        {
            var bd = fixtureMaintenanceProfileRepository.DoAllFMPExportFunction(search);
            return bd;
        }
        public List<string> GetFixtureNoByFunPlant(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var fixturemList = maintenancePlanRepository.GetFixtureNoByFunPlant(BG_Organization_UID, FunPlant_Organization_UID).ToList();

            return fixturemList;
        }

        public List<MaintenancePlanDTO> GetMaintenancePlanByFilters(int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type)
        {
            var fixturemList = maintenancePlanRepository.GetMaintenancePlanByFilters(BG_Organization_UID, FunPlant_Organization_UID, Maintenance_Type).ToList();
            return fixturemList;
        }

        public List<FixtureDTO> QueryAllFixture()
        {
            var vendorinfos = fixtureRepository.GetAll().ToList();
            List<FixtureDTO> dtoList = new List<FixtureDTO>();
            foreach (var item in vendorinfos)
            {
                dtoList.Add(AutoMapper.Mapper.Map<FixtureDTO>(item));
            }
            return dtoList;
        }

        #endregion            保养计划设定维护----------------------END

        #region                                   用户车间设定----------------START
        public PagedListModel<FixtureUserWorkshopDTO> QueryFixtureUserWorkshop(FixtureUserWorkshopDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = fixtureUserWorkshopRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<FixtureUserWorkshopDTO>(totalcount, result);
        }
        public FixtureUserWorkshopDTO QueryFixtureUserWorkshopSingle(int Fixture_User_Workshop_UID)
        {
            var bud = fixtureUserWorkshopRepository.GetByUId(Fixture_User_Workshop_UID);
            return bud;
        }
        public string AddOrEditFixtureUserWorkshop(FixtureUserWorkshopDTO dto)
        {
            try
            {
                if (dto.Fixture_User_Workshop_UID == 0)
                {
                    var hasitem = fixtureUserWorkshopRepository.GetMany(m => m.Account_UID == dto.Account_UID
                    & m.Workshop_UID == dto.Workshop_UID).FirstOrDefault();
                    if (hasitem != null)
                        return "此笔资料已存在,不可重复添加";
                    DateTime currentTimd = DateTime.Now;
                    Fixture_User_Workshop FW = new Fixture_User_Workshop();
                    FW.Account_UID = dto.Account_UID;
                    FW.Workshop_UID = dto.Workshop_UID;
                    FW.Created_UID = dto.Created_UID;
                    FW.Created_Date = currentTimd;
                    FW.Modified_UID = dto.Created_UID;
                    FW.Modified_Date = currentTimd;
                    FW.Is_Enable = dto.Is_Enable;
                    fixtureUserWorkshopRepository.Add(FW);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var hasitem = fixtureUserWorkshopRepository.GetMany(m => m.Account_UID == dto.Account_UID
                        & m.Workshop_UID == dto.Workshop_UID & m.Fixture_User_Workshop_UID != dto.Fixture_User_Workshop_UID).FirstOrDefault();
                    if (hasitem != null)
                        return "修改后的资料已存在,不可修改";
                    var FW = fixtureUserWorkshopRepository.GetFirstOrDefault(m => m.Fixture_User_Workshop_UID == dto.Fixture_User_Workshop_UID);
                    FW.Account_UID = dto.Account_UID;
                    FW.Workshop_UID = dto.Workshop_UID;
                    FW.Modified_UID = dto.Created_UID;
                    FW.Is_Enable = dto.Is_Enable;
                    FW.Modified_Date = DateTime.Now;

                    fixtureUserWorkshopRepository.Update(FW);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "更新用户车间设定失败:" + e.Message;
            }
        }
        public string DeleteFixtureUserWorkshop(int Fixture_User_Workshop_UID)
        {
            try
            {
                var FW = fixtureUserWorkshopRepository.GetById(Fixture_User_Workshop_UID);
                fixtureUserWorkshopRepository.Delete(FW);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除用户车间设定失败:" + e.Message;
            }

        }
        public string InsertFixtureUserWorkshopItem(List<FixtureUserWorkshopDTO> dtolist)
        {
            return fixtureUserWorkshopRepository.InsertItem(dtolist);
        }
        public List<FixtureUserWorkshopDTO> QueryAllFixtureUserWorkshops()
        {
            var vendorinfos = fixtureUserWorkshopRepository.GetAll().ToList();
            List<FixtureUserWorkshopDTO> dtoList = new List<FixtureUserWorkshopDTO>();
            foreach (var item in vendorinfos)
            {
                dtoList.Add(AutoMapper.Mapper.Map<FixtureUserWorkshopDTO>(item));
            }
            return dtoList;
        }
        public List<SystemUserDTO> GetUserByOp(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixtureUserWorkshopRepository.GetUserByOp(BG_Organization_UID, FunPlant_Organization_UID).ToList();
            return bud;
        }
        public List<SystemUserDTO> GetUserByOpAPILY(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixtureUserWorkshopRepository.GetUserByOpAPILY(BG_Organization_UID, FunPlant_Organization_UID).ToList();
            return bud;
        }

        public List<WorkshopDTO> QueryAllWorkshops()
        {
            var workshops = workshopRepository.GetAll().ToList();
            List<WorkshopDTO> dtoList = new List<WorkshopDTO>();
            foreach (var item in workshops)
            {
                dtoList.Add(AutoMapper.Mapper.Map<WorkshopDTO>(item));
            }
            return dtoList;
        }
        public List<WorkshopDTO> GetWorkshopByNTID(int Account_UID)
        {
            var bud = fixtureUserWorkshopRepository.GetByNTID(Account_UID).ToList();
            return bud;
        }
        public List<FixtureUserWorkshopDTO> DoFUWExportFunction(string uids)
        {
            var bd = fixtureUserWorkshopRepository.DoExportFunction(uids);
            return bd;
        }
        public List<FixtureUserWorkshopDTO> DoAllFUWExportFunction(FixtureUserWorkshopDTO searchModel)
        {
            var bd = fixtureUserWorkshopRepository.DoAllFUWExportFunction(searchModel);
            return bd;
        }
        public List<SystemUserDTO> QueryAllUsers()
        {
            var vendorinfos = systemUserRepository.GetAll().ToList();
            List<SystemUserDTO> dtoList = new List<SystemUserDTO>();
            foreach (var item in vendorinfos)
            {
                dtoList.Add(AutoMapper.Mapper.Map<SystemUserDTO>(item));
            }
            return dtoList;
        }
        #endregion                             用户车间设定------------------END   

        #region                                   维修地点设定----------------START
        public PagedListModel<RepairLocationDTO> QueryRepairLocation(RepairLocationDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = repairLocationRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<RepairLocationDTO>(totalcount, result);
        }

        public Repair_Location QueryRepairLocationSingle(int Repair_Location_UID)
        {
            var bud = repairLocationRepository.GetById(Repair_Location_UID);
            return bud;
        }
        public string DeleteRepairLocation(int Repair_Location_UID)
        {
            try
            {
                var RL = repairLocationRepository.GetById(Repair_Location_UID);
                repairLocationRepository.Delete(RL);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除维修地点失败:" + e.Message;
            }

        }
        public string AddOrEditRepairLocation(RepairLocationDTO dto)
        {
            try
            {
                int? FunPlant_Organization_UID = dto.FunPlant_Organization_UID == 0 ? null : dto.FunPlant_Organization_UID;
                if (dto.Repair_Location_UID == 0)
                {
                    var hasitem = repairLocationRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                    & m.FunPlant_Organization_UID == FunPlant_Organization_UID
                    & m.Repair_Location_ID == dto.Repair_Location_ID).FirstOrDefault();
                    if (hasitem != null)
                        return "维修代码【" + dto.Repair_Location_ID + "】已存在,不可重复添加";
                    DateTime currentTimd = DateTime.Now;
                    Repair_Location RL = new Repair_Location();
                    RL.Plant_Organization_UID = dto.Plant_Organization_UID;
                    RL.BG_Organization_UID = dto.BG_Organization_UID;
                    RL.FunPlant_Organization_UID = FunPlant_Organization_UID;
                    RL.Repair_Location_ID = dto.Repair_Location_ID;
                    RL.Repair_Location_Name = dto.Repair_Location_Name;
                    RL.Repair_Location_Desc = dto.Repair_Location_Desc;
                    RL.Is_Enable = dto.Is_Enable;
                    RL.Created_UID = dto.Created_UID;
                    RL.Created_Date = currentTimd;
                    RL.Modified_UID = dto.Created_UID;
                    RL.Modified_Date = currentTimd;
                    repairLocationRepository.Add(RL);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var hasitem = repairLocationRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                        & m.FunPlant_Organization_UID == FunPlant_Organization_UID & m.Repair_Location_UID != dto.Repair_Location_UID
                        & m.Repair_Location_ID == dto.Repair_Location_ID).FirstOrDefault();
                    if (hasitem != null)
                        return "维修代码【" + dto.Repair_Location_ID + "】已存在,修改";
                    var RL = repairLocationRepository.GetFirstOrDefault(m => m.Repair_Location_UID == dto.Repair_Location_UID);
                    RL.Repair_Location_ID = dto.Repair_Location_ID;
                    RL.Repair_Location_Name = dto.Repair_Location_Name;
                    RL.Repair_Location_Desc = dto.Repair_Location_Desc;
                    RL.Is_Enable = dto.Is_Enable;
                    RL.Modified_UID = dto.Created_UID;
                    RL.Modified_Date = DateTime.Now;

                    repairLocationRepository.Update(RL);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "更新维修地点失败:" + e.Message;
            }
        }
        public string InsertRepairLocation(List<RepairLocationDTO> dtolist)
        {
            return repairLocationRepository.InsertItem(dtolist);
        }
        public List<RepairLocationDTO> DoRLExportFunction(string uids)
        {
            var bd = repairLocationRepository.DoExportFunction(uids);
            return bd;
        }
        public List<RepairLocationDTO> DoAllRLExportFunction(RepairLocationDTO searchModel)
        {
            var bd = repairLocationRepository.DoAllRLExportFunction(searchModel);
            return bd;
        }
        public List<RepairLocationDTO> QueryAllRepairLocations()
        {
            var repairlocations = repairLocationRepository.GetAll().ToList();
            List<RepairLocationDTO> dtoList = new List<RepairLocationDTO>();
            foreach (var item in repairlocations)
            {
                dtoList.Add(AutoMapper.Mapper.Map<RepairLocationDTO>(item));
            }
            return dtoList;
        }
        #endregion                             维修地点设定------------------END
        #region                  治具领用------------------START
        public PagedListModel<Fixture_Totake_MDTO> QueryFixtureTotake(Fixture_Totake_MDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = fixture_Totake_MRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<Fixture_Totake_MDTO>(totalcount, result);
        }
        public Fixture_Totake_MDTO QueryFixtureTotakeSingle(int Fixture_Totake_M_UID)
        {
            var bud = fixture_Totake_MRepository.GetByUId(Fixture_Totake_M_UID);
            return bud;
        }

        public List<fixture> GetfixtureList(int Fixture_Totake_M_UID)
        {
            var bud = fixture_Totake_MRepository.GetFixture(Fixture_Totake_M_UID);
            return bud;
        }
        public List<FixtureDTO> GetFixtureByWorkshop(int Account_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name)
        {
            var status_uid = enumerationRepository.GetMany(m => m.Enum_Type == "Fixture_Status" & m.Enum_Name == "2").FirstOrDefault().Enum_UID;
            var bud = fixture_Totake_MRepository.GetFixtureByFilters(Account_UID, Line_ID, Line_Name, Machine_ID, Machine_Name, Process_ID, Process_Name, status_uid);
            return bud;
        }
        public List<SystemUserDTO> GetUserByWorkshop(int Account_UID)
        {
            var bud = fixture_Totake_MRepository.GetUserByWorkshop(Account_UID);
            return bud;
        }
        public string AddOrEditFixtureTotake(Fixture_Totake_MDTO dto)
        {
            try
            {
                string head = "B" + DateTime.Today.ToString("yyyyMMdd") + "_";
                int totake_no = 1;
                var items = fixture_Totake_MRepository.GetMany(m => m.Totake_NO.StartsWith(head));
                if (items != null)
                    totake_no = items.Count() + 1;
                Fixture_Totake_M FTM = new Fixture_Totake_M();
                FTM.Plant_Organization_UID = dto.Plant_Organization_UID;
                FTM.BG_Organization_UID = dto.BG_Organization_UID;
                FTM.FunPlant_Organization_UID = dto.FunPlant_Organization_UID == 0 ? null : dto.FunPlant_Organization_UID;
                FTM.Totake_NO = head + totake_no.ToString().PadLeft(3, '0');
                FTM.Shiper_UID = dto.Shiper_UID;
                FTM.Ship_Date = dto.Ship_Date;
                FTM.Totake_Date = dto.Totake_Date;
                FTM.Created_UID = dto.Created_UID;
                FTM.Created_Date = DateTime.Now;
                FTM.Modified_UID = dto.Created_UID;
                FTM.Modified_Date = DateTime.Now;
                FTM.Totake_Number = dto.Totake_Number;
                FTM.Totake_Name = dto.Totake_Name;
                fixture_Totake_MRepository.Add(FTM);
                unitOfWork.Commit();
                return FTM.Totake_NO;
            }
            catch (Exception e)
            {
                return "保存治具领用单失败！\n" + e.ToString();
            }
        }

        public string UpdateFixture(List<FixtureDTO> fixturelist, int useruid, string Totake_NO, DateTime Totake_Date)
        {
            try
            {
                int Fixture_Totake_M_UID = 0;
                var FTM = fixture_Totake_MRepository.GetMany(M => M.Totake_NO == Totake_NO).FirstOrDefault();
                if (FTM == null)
                    return "更新治具失败";
                else
                    Fixture_Totake_M_UID = FTM.Fixture_Totake_M_UID;
                string deletemessage = fixture_Totake_DRepository.DeleteByMuid(Fixture_Totake_M_UID);
                if (deletemessage != "")
                    return deletemessage;

                foreach (var fixture in fixturelist)
                {
                    Fixture_Totake_D item = new Fixture_Totake_D();
                    item.Fixture_Totake_M_UID = Fixture_Totake_M_UID;
                    item.Fixture_M_UID = fixture.Fixture_M_UID;
                    item.Created_UID = useruid;
                    item.Created_Date = DateTime.Now;
                    item.Modified_UID = useruid;
                    item.Modified_Date = DateTime.Now;
                    fixture_Totake_DRepository.Add(item);
                    unitOfWork.Commit();

                    var FTD = fixture_Totake_DRepository.GetMany(m => m.Fixture_Totake_M_UID == Fixture_Totake_M_UID
                    & m.Fixture_M_UID == fixture.Fixture_M_UID).FirstOrDefault();

                    if (FTD == null)
                    {
                        return "更新治具失败";
                    }
                    else
                    {
                        var Fixture_M = fixtureRepository.GetById(fixture.Fixture_M_UID);
                        Fixture_M.Status = GetFixtureStatuDTO().FirstOrDefault(o => o.StatuName == "使用中In-PRD").Status;
                        fixtureRepository.Update(Fixture_M);
                    }

                    int sourceuid = FTD.Fixture_Totake_D_UID;
                    Fixture_Resume FR = new Fixture_Resume();
                    FR.Fixture_M_UID = fixture.Fixture_M_UID;
                    FR.Data_Source = "5";
                    FR.Resume_Date = Totake_Date;
                    FR.Source_UID = sourceuid;
                    FR.Source_NO = Totake_NO;
                    FR.Modified_UID = useruid;
                    FR.Modified_Date = DateTime.Now;
                    FR.Resume_Notes = "领用";
                    fixture_ResumeRepository.Add(FR);



                }
                unitOfWork.Commit();
                return Totake_NO;
            }
            catch (Exception e)
            {
                return "更新治具失败:" + e.Message;
            }

        }
        public List<Fixture_Totake_MDTO> DoFTExportFunction(string uids)
        {
            var bd = fixture_Totake_MRepository.DoExportFunction(uids);
            return bd;
        }

        public List<Fixture_Totake_MDTO> DoAllFTExportFunction(Fixture_Totake_MDTO search)
        {
            var bd = fixture_Totake_MRepository.DoAllFTExportFunction(search);
            return bd;
        }
        public PagedListModel<Fixture_Totake_DDTO> QueryFixtureTotakeDetail(Fixture_Totake_DDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = fixture_Totake_DRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<Fixture_Totake_DDTO>(totalcount, result);
        }
        #endregion            治具领用--------------------END
        #endregion           karl   end






        public List<Vendor_InfoDTO> GetVendor_InfoList(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var bud = fixtureRepository.GetVendor_InfoList(Plant_Organization_UID, BG_Organization_UID);
            return bud;
        }

        public List<Production_LineDTO> GetProductionLineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixtureRepository.GetProductionLineDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;
        }


        public List<WorkshopDTO> GetWorkshopList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var bud = fixtureRepository.GetWorkshopList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;

        }
        public IList<WorkshopDTO> GetWorkshopListByQuery(WorkshopModelSearch search)
        {
            var query = workshopRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID > 0)
                {
                    query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
                }
                if (!string.IsNullOrWhiteSpace(search.Workshop_ID))
                {
                    query = query.Where(w => w.Workshop_ID.Contains(search.Workshop_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.Workshop_Name))
                {
                    query = query.Where(w => w.Workshop_Name.Contains(search.Workshop_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Building_Name))
                {
                    query = query.Where(w => w.Building_Name.Contains(search.Building_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Floor_Name))
                {
                    query = query.Where(w => w.Floor_Name.Contains(search.Floor_Name));
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(w => w.Is_Enable == search.Is_Enable);
                }
            }
            query = query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.FunPlant_Organization_UID).ThenBy(w => w.Workshop_UID);
            IList<WorkshopDTO> workshopsDTO = new List<WorkshopDTO>();

            foreach (var workshop in query)
            {
                var dto = AutoMapper.Mapper.Map<WorkshopDTO>(workshop);
                if (workshop.System_Organization != null)
                {
                    dto.PlantName = workshop.System_Organization.Organization_Name;
                }
                if (workshop.System_Organization1 != null)
                {
                    dto.BGName = workshop.System_Organization1.Organization_Name;
                }
                if (workshop.System_Organization2 != null)
                {
                    dto.FunPlantName = workshop.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(workshop.System_Users1);
                workshopsDTO.Add(dto);
            }
            return workshopsDTO;
        }

        public List<WorkStationDTO> GetWorkstationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixtureRepository.GetWorkstationList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;
        }

        //根据查询条件获取工站数据
        public IList<WorkStationDTO> GetWorkstationListByQuery(WorkStationModelSearch search)
        {
            var query = workStationRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID.HasValue && search.FunPlant_Organization_UID.Value > 0)
                {
                    query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                }
                if (search.Project_UID > 0)
                {
                    query = query.Where(w => w.Project_UID == search.Project_UID);
                }
                if (search.Process_Info_UID > 0)
                {
                    query = query.Where(w => w.Process_Info_UID == search.Process_Info_UID);
                }
                if (!string.IsNullOrWhiteSpace(search.WorkStation_ID))
                {
                    query = query.Where(w => w.WorkStation_ID.Contains(search.WorkStation_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.WorkStation_Name))
                {
                    query = query.Where(w => w.WorkStation_Name.Contains(search.WorkStation_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.WorkStation_Desc))
                {
                    query = query.Where(w => w.WorkStation_Desc.Contains(search.WorkStation_Desc));
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
                }
            }
            query = query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.WorkStation_UID);

            IList<WorkStationDTO> WorkStationDTOs = new List<WorkStationDTO>();

            foreach (var workStation in query)
            {
                var dto = AutoMapper.Mapper.Map<WorkStationDTO>(workStation);
                if (workStation.System_Organization != null)
                {
                    dto.PlantName = workStation.System_Organization.Organization_Name;
                }
                if (workStation.System_Organization1 != null)
                {
                    dto.BGName = workStation.System_Organization1.Organization_Name;
                }
                if (workStation.System_Organization2 != null)
                {
                    dto.FunPlantName = workStation.System_Organization2.Organization_Name;
                }
                if (workStation.System_Project != null)
                {
                    dto.Project_Name = workStation.System_Project.Project_Name;
                }
                if (workStation.Process_Info != null)
                {
                    dto.Process_Name = workStation.Process_Info.Process_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(workStation.System_Users1);
                WorkStationDTOs.Add(dto);
            }
            return WorkStationDTOs;
        }

        public List<Process_InfoDTO> GetProcess_InfoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var bud = fixtureRepository.GetProcess_InfoList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;
        }

        public List<SystemProjectDTO> GetProjectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var bud = fixtureRepository.GetProjectList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;
        }


        public Production_LineDTO GetProductionLineDTO(int Production_Line_UID)
        {
            var bud = fixtureRepository.GetProductionLineDTO(Production_Line_UID);
            return bud;
        }


        #region 治具异常原因及相应维修策略设定--------Add by justin 2017-09-29
        public bool IsDefectRepairExist(DefectRepairSearch search)
        {
            var isExist = false;
            var query = defectRepairRepository.GetAll();
            if (search.Defect_RepairSolution_UID.HasValue)
            {
                query = query.Where(i => i.Defect_RepairSolution_UID != search.Defect_RepairSolution_UID.Value);
            }
            var workStation = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.Fixtrue_Defect_UID == search.Fixtrue_Defect_UID && d.Repair_Solution_UID == search.Repair_Solution_UID);
            if (workStation != null)
            {
                isExist = true;
            }
            return isExist;
        }
        public List<Fixture_DefectCodeDTO> GetDefectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureRepository.GetDefectCodeList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return result;
        }
        public List<Fixture_RepairSolutionDTO> GetRepairSoulutions(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureRepository.GeRepairSolutionList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return result;
        }

        public PagedListModel<DefectRepairSolutionDTO> QueryDefectRepairs(DefectRepairSearch search, Page page)
        {
            var totalCount = 0;
            var DefectRepairS = defectRepairRepository.QueryDefectRepairs(search, page, out totalCount);

            IList<DefectRepairSolutionDTO> defectRepairDTOS = new List<DefectRepairSolutionDTO>();

            foreach (var DefectRepair in DefectRepairS)
            {
                var dto = AutoMapper.Mapper.Map<DefectRepairSolutionDTO>(DefectRepair);
                if (DefectRepair.System_Organization != null)
                {
                    dto.PlantName = DefectRepair.System_Organization.Organization_Name;
                }
                if (DefectRepair.System_Organization1 != null)
                {
                    dto.BGName = DefectRepair.System_Organization1.Organization_Name;
                }
                if (DefectRepair.System_Organization2 != null)
                {
                    dto.FunPlantName = DefectRepair.System_Organization2.Organization_Name;
                }
                if (DefectRepair.Fixture_DefectCode != null)
                {
                    dto.Fixture_DefectName = DefectRepair.Fixture_DefectCode.DefectCode_Name;
                }
                if (DefectRepair.Fixture_RepairSolution != null)
                {
                    dto.Repair_SoulutionName = DefectRepair.Fixture_RepairSolution.RepairSolution_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(DefectRepair.System_Users1);
                defectRepairDTOS.Add(dto);
            }

            return new PagedListModel<DefectRepairSolutionDTO>(totalCount, defectRepairDTOS);
        }
        public IList<DefectRepairSolutionDTO> GetDefectRepairList(DefectRepairSearch search)
        {
            var query = defectRepairRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID > 0)
                {
                    query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
                }
                if (search.Fixtrue_Defect_UID != null && search.Fixtrue_Defect_UID != 0)
                {
                    query = query.Where(w => w.Fixtrue_Defect_UID == search.Fixtrue_Defect_UID);
                }
                if (!string.IsNullOrWhiteSpace(search.Fixture_DefectName))
                {
                    //获取Fixture_DefectName对应的UID
                    int Fixture_Defect_UID = defectRepairRepository.getFixture_DefectUID(search.Fixture_DefectName);

                    query = query.Where(w => w.Fixtrue_Defect_UID == Fixture_Defect_UID);
                }
                if (search.Repair_Solution_UID != null && search.Repair_Solution_UID != 0)
                {
                    query = query.Where(w => w.Repair_Solution_UID == search.Repair_Solution_UID);
                }
                if (!string.IsNullOrWhiteSpace(search.Repair_SoulutionName))
                {
                    int Fixture_RepairSolution_UID = defectRepairRepository.getFixture_RepairSolution_UID(search.Repair_SoulutionName);
                    query = query.Where(w => w.Repair_Solution_UID == Fixture_RepairSolution_UID);
                }

                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(w => w.Is_Enable == search.Is_Enable);
                }
            }
            query = query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Defect_RepairSolution_UID);

            IList<DefectRepairSolutionDTO> defectRepairDTOS = new List<DefectRepairSolutionDTO>();

            foreach (var DefectRepair in query)
            {
                var dto = AutoMapper.Mapper.Map<DefectRepairSolutionDTO>(DefectRepair);
                if (DefectRepair.System_Organization != null)
                {
                    dto.PlantName = DefectRepair.System_Organization.Organization_Name;
                }
                if (DefectRepair.System_Organization1 != null)
                {
                    dto.BGName = DefectRepair.System_Organization1.Organization_Name;
                }
                if (DefectRepair.System_Organization2 != null)
                {
                    dto.FunPlantName = DefectRepair.System_Organization2.Organization_Name;
                }
                if (DefectRepair.Fixture_DefectCode != null)
                {
                    dto.Fixture_DefectName = DefectRepair.Fixture_DefectCode.DefectCode_Name;
                }
                if (DefectRepair.Fixture_RepairSolution != null)
                {
                    dto.Repair_SoulutionName = DefectRepair.Fixture_RepairSolution.RepairSolution_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(DefectRepair.System_Users1);
                defectRepairDTOS.Add(dto);
            }

            return defectRepairDTOS;
        }

        public DefectCode_RepairSolution QueryDefectRepairSingle(int uid)
        {
            return defectRepairRepository.GetById(uid);
        }

        public string AddDefectRepair(DefectCode_RepairSolution DefectRepair)
        {

            defectRepairRepository.Add(DefectRepair);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditDefectRepair(DefectCode_RepairSolution workshop)
        {
            defectRepairRepository.Update(workshop);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteDefectRepair(int uid)
        {
            var DefectRepair = defectRepairRepository.GetFirstOrDefault(w => w.Defect_RepairSolution_UID == uid);
            if (DefectRepair != null)
            {

                try
                {
                    defectRepairRepository.Delete(DefectRepair);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<DefectRepairSolutionDTO> DoExportDefectRepair(string uids)
        {
            var totalCount = 0;
            var defectRepairS = defectRepairRepository.QueryDefectRepairs(new DefectRepairSearch { ExportUIds = uids }, null, out totalCount);

            IList<DefectRepairSolutionDTO> defectRepairDTOs = new List<DefectRepairSolutionDTO>();

            foreach (var defectRepair in defectRepairS)
            {
                var dto = AutoMapper.Mapper.Map<DefectRepairSolutionDTO>(defectRepair);
                if (defectRepair.System_Organization != null)
                {
                    dto.PlantName = defectRepair.System_Organization.Organization_Name;
                }
                if (defectRepair.System_Organization1 != null)
                {
                    dto.BGName = defectRepair.System_Organization1.Organization_Name;
                }
                if (defectRepair.System_Organization2 != null)
                {
                    dto.FunPlantName = defectRepair.System_Organization2.Organization_Name;
                }
                if (defectRepair.Fixture_DefectCode != null)
                {
                    dto.Fixture_DefectName = defectRepair.Fixture_DefectCode.DefectCode_Name;
                }
                if (defectRepair.Fixture_RepairSolution != null)
                {
                    dto.Repair_SoulutionName = defectRepair.Fixture_RepairSolution.RepairSolution_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(defectRepair.System_Users1);
                defectRepairDTOs.Add(dto);
            }
            return defectRepairDTOs;
        }

        #endregion 治具异常原因及相应维修策略设定--------Add by justin 2017-09-29

        public List<FixtureMachineDTO> GetFixtureMachineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, int Production_Line_UID)
        {
            var bud = fixtureRepository.GetFixtureMachineDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Production_Line_UID);
            return bud;
        }
        public FixtureMachineDTO GetFixtureMachineByUid(int Fixture_Machine_UID)
        {

            var bud = fixtureRepository.GetFixtureMachineByUid(Fixture_Machine_UID);
            return bud;
        }

        public string AddOrEditFixtureM(FixtureDTO dto, bool isEdit)
        {
            try
            {

                int count = GetFixture_MCount(dto.Plant_Organization_UID, dto.BG_Organization_UID, dto.Fixture_NO + dto.Fixture_Seq);
                if (count <= 1)
                {
                    if (!isEdit)
                    {
                        if (count < 1)
                        {
                            Fixture_M item = new Fixture_M();
                            item.Plant_Organization_UID = dto.Plant_Organization_UID;
                            item.BG_Organization_UID = dto.BG_Organization_UID;
                            item.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                            item.Fixture_NO = dto.Fixture_NO.Trim();
                            item.Version = dto.Version.Trim();
                            item.Fixture_Seq = dto.Fixture_Seq.Trim();
                            item.Fixture_Unique_ID = dto.Fixture_NO.Trim() + dto.Fixture_Seq.Trim();
                            item.Fixture_Name = dto.Fixture_Name;
                            item.Project_UID = dto.Project_UID;
                            // item.Modified_Date = DateTime.Now;
                            item.Fixture_Machine_UID = dto.Fixture_Machine_UID;
                            item.Vendor_Info_UID = dto.Vendor_Info_UID;
                            item.Production_Line_UID = dto.Production_Line_UID;
                            item.Status = dto.Status;
                            item.ShortCode = dto.ShortCode;
                            item.TwoD_Barcode = dto.TwoD_Barcode;
                            item.Created_UID = dto.Created_UID;
                            item.Created_Date = DateTime.Now;
                            item.Modified_UID = dto.Modified_UID;
                            item.Modified_Date = DateTime.Now;
                            Fixture_M newitem = fixtureRepository.Add(item);
                            unitOfWork.Commit();
                            // Fixture_M newitem = new Fixture_M();
                            // newitem = fixtureRepository.GetFixtureByUid(dto);
                            if (newitem != null)
                            {
                                Fixture_Resume item_Resume = new Fixture_Resume();
                                item_Resume.Fixture_M_UID = newitem.Fixture_M_UID;
                                item_Resume.Data_Source = "1";
                                item_Resume.Resume_Date = newitem.Created_Date;
                                item_Resume.Resume_Notes = "新品入庫建檔";
                                item_Resume.Modified_UID = newitem.Created_UID;
                                item_Resume.Source_UID = newitem.Fixture_M_UID;
                                //表单流水号 需要获取当前是第几笔新增的治具资料；
                                int source_UID = fixtureRepository.GetFixtureCount(dto);

                                //后面还要加表单编号
                                item_Resume.Source_NO = "C" + newitem.Created_Date.Date.ToString("yyyyMMdd") + "_" + source_UID.ToString().PadLeft(3, '0');
                                item_Resume.Modified_Date = newitem.Created_Date;
                                fixture_ResumeRepository.Add(item_Resume);
                                unitOfWork.Commit();
                            }
                        }
                        else
                        {
                            return "数据重复，已经添加了此数据！";

                        }
                    }
                    else
                    {
                        //if (count == 1)
                        //{

                        Fixture_M fixture_M = fixtureRepository.GetById(dto.Fixture_M_UID);
                        if (fixture_M == null)
                        {
                            return "保存失败，请检查你的数据！";
                        }
                        else
                        {
                            fixture_M.Plant_Organization_UID = dto.Plant_Organization_UID;
                            fixture_M.BG_Organization_UID = dto.BG_Organization_UID;
                            fixture_M.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                            fixture_M.Fixture_NO = dto.Fixture_NO.Trim();
                            fixture_M.Version = dto.Version.Trim();
                            fixture_M.Fixture_Seq = dto.Fixture_Seq.Trim();
                            fixture_M.Fixture_Unique_ID = dto.Fixture_NO.Trim() + dto.Fixture_Seq.Trim();
                            fixture_M.Fixture_Name = dto.Fixture_Name;
                            fixture_M.Project_UID = dto.Project_UID;
                            fixture_M.Modified_Date = DateTime.Now;
                            fixture_M.Fixture_Machine_UID = dto.Fixture_Machine_UID;
                            fixture_M.Vendor_Info_UID = dto.Vendor_Info_UID;
                            fixture_M.Production_Line_UID = dto.Production_Line_UID;
                            fixture_M.Status = dto.Status;
                            fixture_M.ShortCode = dto.ShortCode;
                            fixture_M.TwoD_Barcode = dto.TwoD_Barcode;
                            //fixture_M.Created_UID = dto.Created_UID;
                            //fixture_M.Created_Date = dto.Created_Date;
                            fixture_M.Modified_UID = dto.Modified_UID;
                            fixture_M.Modified_Date = DateTime.Now;
                            fixtureRepository.Update(fixture_M);
                            unitOfWork.Commit();
                            //}
                        }
                    }

                    return string.Empty;
                }
                else
                {
                    return "数据重复，已经添加了此数据！";
                }
            }
            catch (Exception e)
            {
                return "保存失败，请检查你的数据！";
            }
        }

        public int GetFixture_MCount(int Plant_Organization_UID, int BG_Organization_UID, string Fixture_Unique_ID)
        {
            return fixtureRepository.GetFixture_MCount(Plant_Organization_UID, BG_Organization_UID, Fixture_Unique_ID);
        }

        public string DeleteFixtureM(int Fixture_M_UID)
        {

            return fixtureRepository.DeleteByUid(Fixture_M_UID);
        }
        public List<FixtureDTO> DoExportFixtureReprot(string Fixture_M_UIDs)
        {
            return fixtureRepository.DoExportFixtureReprot(Fixture_M_UIDs);

        }
        public List<FixtureDTO> FixtureList(FixtureDTO searchModel)
        {
            return fixtureRepository.FixtureList(searchModel);
        }

        /// <summary>
        /// 批量启用禁用治具资料维护
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="IsEnable"></param>
        /// <returns></returns>
        public string BatchEnableFixturematerial(FixtureDTO searchModel, string IsEnable)
        {

            //1 查询需要启用、禁用的数据
            List<FixtureDTO> queryModeList = new List<FixtureDTO>();
            queryModeList = fixtureRepository.FixtureList(searchModel);
            var enableStatus = int.Parse(IsEnable);
            //如果是批量启用-将未使用(478)notUseingCode-->使用中(477) UseingCode
            var StatuList = GetFixtureStatuDTO();
            var UseingCode = StatuList.FirstOrDefault(p => p.StatuName == "使用中In-PRD").Status;
            var notUseingCode = StatuList.FirstOrDefault(p => p.StatuName == "未使用Non-PRD").Status;
            int status = 0;
            if (enableStatus == 0)
            {
                queryModeList = queryModeList.Where(p => p.Status == notUseingCode).ToList();
                status = UseingCode;

            }
            //如果是批量启用-将使用中(477)-->未使用(478)
            if (enableStatus == 1)
            {
                queryModeList = queryModeList.Where(p => p.Status == UseingCode).ToList();
                status = notUseingCode;
            }

            //2 更新数据  //3 修改记录
            var result = fixtureRepository.BatchEnableFixturematerial(searchModel, queryModeList, status);
            return result;
        }

        public string BatchEnablePartFixturematerial(string Fixture_M_UIDs, string IsEnable, int AccountID)
        {
            //1 查询需要启用、禁用的数据
            List<FixtureDTO> queryModeList = new List<FixtureDTO>();
            queryModeList = fixtureRepository.DoExportFixtureReprot(Fixture_M_UIDs);
            var enableStatus = int.Parse(IsEnable);
            //如果是批量启用-将未使用(478)-->使用中(477)
            int status = 0;
            var StatuList = GetFixtureStatuDTO();
            var UseingCode = StatuList.FirstOrDefault(p => p.StatuName == "使用中In-PRD").Status;
            var notUseingCode = StatuList.FirstOrDefault(p => p.StatuName == "未使用Non-PRD").Status;
            //如果是批量启用-将未使用(478)notUseingCode-->使用中(477) UseingCode
            if (enableStatus == 0)
            {
                queryModeList = queryModeList.Where(p => p.Status == notUseingCode).ToList();
                status = UseingCode;
            }
            //如果是批量启用-将使用中(477)-->未使用(478)
            if (enableStatus == 1)
            {
                queryModeList = queryModeList.Where(p => p.Status == UseingCode).ToList();
                status = notUseingCode;
            }
            //2 更新数据  //3 修改记录
            FixtureDTO model = new FixtureDTO();
            model.AccountID = AccountID;
            var result = fixtureRepository.BatchEnableFixturematerial(model, queryModeList, status);
            return result;
        }

        #region 工站维护
        public bool IsWorkStationExist(WorkStationModelSearch search)
        {
            var isExist = false;
            var query = workStationRepository.GetAll();
            if (search.WorkStation_UID.HasValue)
            {
                query = query.Where(i => i.WorkStation_UID != search.WorkStation_UID.Value);
            }
            var workStation = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.WorkStation_ID == search.WorkStation_ID);
            if (workStation != null)
            {
                isExist = true;
            }
            return isExist;
        }
        public PagedListModel<WorkStationDTO> QueryWorkStations(WorkStationModelSearch search, Page page)
        {
            var totalCount = 0;
            var WorkStations = workStationRepository.QueryWorkStations(search, page, out totalCount);

            IList<WorkStationDTO> WorkStationDTOs = new List<WorkStationDTO>();

            foreach (var workStation in WorkStations)
            {
                var dto = AutoMapper.Mapper.Map<WorkStationDTO>(workStation);
                if (workStation.System_Organization != null)
                {
                    dto.PlantName = workStation.System_Organization.Organization_Name;
                }
                if (workStation.System_Organization1 != null)
                {
                    dto.BGName = workStation.System_Organization1.Organization_Name;
                }
                if (workStation.System_Organization2 != null)
                {
                    dto.FunPlantName = workStation.System_Organization2.Organization_Name;
                }
                if (workStation.System_Project != null)
                {
                    dto.Project_Name = workStation.System_Project.Project_Name;
                }
                if (workStation.Process_Info != null)
                {
                    dto.Process_Name = workStation.Process_Info.Process_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(workStation.System_Users1);
                WorkStationDTOs.Add(dto);
            }

            return new PagedListModel<WorkStationDTO>(totalCount, WorkStationDTOs);
        }

        public WorkStation QueryWorkStationSingle(int uid)
        {
            return workStationRepository.GetById(uid);
        }

        public string AddWorkStation(WorkStation WorkStation)
        {
            workStationRepository.Add(WorkStation);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditWorkStation(WorkStation WorkStation)
        {
            workStationRepository.Update(WorkStation);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteWorkStation(int uid)
        {
            var workStation = workStationRepository.GetFirstOrDefault(w => w.WorkStation_UID == uid);
            if (workStation != null)
            {
                //如果有关联的数据则不可删
                var productLineCount = workStation.Production_Line.Count();
                if (productLineCount > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    workStationRepository.Delete(workStation);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<WorkStationDTO> DoExportWorkStation(string uids)
        {
            var totalCount = 0;
            var workStations = workStationRepository.QueryWorkStations(new WorkStationModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<WorkStationDTO> WorkStationDTO = new List<WorkStationDTO>();

            foreach (var workStation in workStations)
            {
                var dto = AutoMapper.Mapper.Map<WorkStationDTO>(workStation);
                if (workStation.System_Organization != null)
                {
                    dto.PlantName = workStation.System_Organization.Organization_Name;
                }
                if (workStation.System_Organization1 != null)
                {
                    dto.BGName = workStation.System_Organization1.Organization_Name;
                }
                if (workStation.System_Organization2 != null)
                {
                    dto.FunPlantName = workStation.System_Organization2.Organization_Name;
                }
                if (workStation.System_Project != null)
                {
                    dto.Project_Name = workStation.System_Project.Project_Name;
                }
                if (workStation.Process_Info != null)
                {
                    dto.Process_Name = workStation.Process_Info.Process_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(workStation.System_Users1);
                WorkStationDTO.Add(dto);
            }
            return WorkStationDTO;
        }
        public string InsertDefectRepairSolutions(List<DefectCode_RepairSolution> list)
        {
            string result = "";
            try
            {
                defectRepairRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }


        public string InsertWorkStaions(List<WorkStation> list)
        {
            string result = "";
            try
            {
                workStationRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }

        public List<WorkStationDTO> QueryAllWorkStations()
        {
            var workshops = workStationRepository.GetAll().ToList();
            var dtoList = new List<WorkStationDTO>();
            foreach (var item in workshops)
            {
                dtoList.Add(AutoMapper.Mapper.Map<WorkStationDTO>(item));
            }
            return dtoList;
        }
        #endregion


        #region 国庆长假第二天_治具履历查询 Add By Rock 2017-10-02 ------------------------start
        public Dictionary<int, string> GetFixtureStatus()
        {
            var list = enumerationRepository.GetMany(m => m.Enum_Type == "Fixture_Status").ToList();
            Dictionary<int, string> dicList = new Dictionary<int, string>();
            dicList.Add(0, "");
            foreach (var item in list)
            {
                dicList.Add(Convert.ToInt32(item.Enum_UID), item.Enum_Value);
            }
            return dicList;
        }

        public PagedListModel<FixtureResumeSearchVM> FixtureResumeSearchVM(FixtureResumeSearchVM searchVM, Page page)
        {
            int totalcount;
            var result = fixtureRepository.FixtureResumeSearchVM(searchVM, page, out totalcount);
            return result;
        }

        public ViewResumeByUID QueryFixtureResumeByUID(int Fixture_Resume_UID, int Fixture_M_UID)
        {
            var item = fixtureRepository.QueryFixtureResumeByUID(Fixture_Resume_UID, Fixture_M_UID);
            return item;
        }
        public List<FixtureResumeSearchVM> DoAllExportFixtureResumeReprot(FixtureResumeSearchVM searchVM)
        {
            return fixtureRepository.DoAllExportFixtureResumeReprot(searchVM);
        }
        public List<FixtureResumeSearchVM> ExportFixtureResumeByUID(string uids)
        {
            var list = fixtureRepository.ExportFixtureResumeByUID(uids);
            return list;
        }

        #endregion 国庆长假第二天_治具履历查询 Add By Rock 2017-10-02 ------------------------end

        #region 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------Start
        public Dictionary<string, string> GetMaintenanceStatus(string Maintenance_Type)
        {
            var list = fixtureRepository.GetMaintenanceStatus(Maintenance_Type);
            return list;
        }

        public PagedListModel<NotMaintenanceSearchVM> QueryFixtureNotMaintained(NotMaintenanceSearchVM search, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryFixtureNotMaintained(search, page, out totalcount);
            return new PagedListModel<NotMaintenanceSearchVM>(totalcount, result);
        }

        public List<NotMaintenanceSearchVM> ExportFixtureNotMaintainedByUID(string uids, string hidDate)
        {
            var list = fixtureRepository.ExportFixtureNotMaintainedByUID(uids, hidDate);
            return list;
        }
        public List<NotMaintenanceSearchVM> DoAllExportFixtureNotMaintainedReprot(NotMaintenanceSearchVM searchVM)
        {
            return fixtureRepository.DoAllExportFixtureNotMaintainedReprot(searchVM);
        }
        #endregion 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------End


        #region 产线维护
        public bool IsProduction_LineExist(Production_LineModelSearch search)
        {
            var isExist = false;
            var query = production_LineRepository.GetAll();
            if (search.Production_Line_UID.HasValue)
            {
                query = query.Where(i => i.Production_Line_UID != search.Production_Line_UID.Value);
            }
            var line = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.Workshop_UID == search.Workshop_UID && d.Line_ID == search.Line_ID);
            if (line != null)
            {
                isExist = true;
            }
            return isExist;
        }
        public PagedListModel<Production_LineDTO> QueryProduction_Lines(Production_LineModelSearch search, Page page)
        {
            var totalCount = 0;
            var Production_Lines = production_LineRepository.QueryProduction_Lines(search, page, out totalCount);

            IList<Production_LineDTO> production_LineDTOList = new List<Production_LineDTO>();

            foreach (var production_Line in Production_Lines)
            {
                var dto = AutoMapper.Mapper.Map<Production_LineDTO>(production_Line);
                if (production_Line.System_Organization != null)
                {
                    dto.Plant_Organization_Name = production_Line.System_Organization.Organization_Name;
                }
                if (production_Line.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = production_Line.System_Organization1.Organization_Name;
                }

                if (production_Line.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = production_Line.System_Organization2.Organization_Name;
                }
                if (production_Line.Workshop != null)
                {
                    dto.Workshop_Name = production_Line.Workshop.Workshop_Name;
                }
                if (production_Line.WorkStation != null)
                {
                    dto.Workstation_Name = production_Line.WorkStation.WorkStation_Name;
                }
                if (production_Line.System_Project != null)
                {
                    dto.Project_Name = production_Line.System_Project.Project_Name;
                }
                if (production_Line.Process_Info != null)
                {
                    dto.Process_Name = production_Line.Process_Info.Process_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(production_Line.System_Users1);
                production_LineDTOList.Add(dto);
            }

            return new PagedListModel<Production_LineDTO>(totalCount, production_LineDTOList);
        }

        public IList<Production_LineDTO> GetProductionLineList(Production_LineModelSearch search)
        {
            var query = production_LineRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(l => l.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(l => l.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID.HasValue && search.FunPlant_Organization_UID.Value > 0)
                {
                    query = query.Where(l => l.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                }

                if (search.Workshop_UID > 0)
                {
                    query = query.Where(l => l.Workshop_UID == search.Workshop_UID);
                }
                if (search.Workstation_UID > 0)
                {
                    query = query.Where(l => l.Workstation_UID == search.Workstation_UID);
                }
                if (search.Project_UID > 0)
                {
                    query = query.Where(l => l.Project_UID == search.Project_UID);
                }
                if (search.Process_Info_UID > 0)
                {
                    query = query.Where(l => l.Process_Info_UID == search.Process_Info_UID);
                }

                if (!string.IsNullOrWhiteSpace(search.Line_ID))
                {
                    query = query.Where(l => l.Line_ID.Contains(search.Line_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.Line_Name))
                {
                    query = query.Where(l => l.Line_Name.Contains(search.Line_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Line_Desc))
                {
                    query = query.Where(l => l.Line_Desc.Contains(search.Line_Desc));
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(l => l.Is_Enable == search.Is_Enable.Value);
                }
            }
            query = query.OrderBy(l => l.Plant_Organization_UID).ThenBy(l => l.BG_Organization_UID).ThenBy(l => l.Production_Line_UID);

            IList<Production_LineDTO> production_LineDTOList = new List<Production_LineDTO>();

            foreach (var production_Line in query)
            {
                var dto = AutoMapper.Mapper.Map<Production_LineDTO>(production_Line);
                if (production_Line.System_Organization != null)
                {
                    dto.Plant_Organization_Name = production_Line.System_Organization.Organization_Name;
                }
                if (production_Line.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = production_Line.System_Organization1.Organization_Name;
                }

                if (production_Line.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = production_Line.System_Organization2.Organization_Name;
                }
                if (production_Line.Workshop != null)
                {
                    dto.Workshop_Name = production_Line.Workshop.Workshop_Name;
                }
                if (production_Line.WorkStation != null)
                {
                    dto.Workstation_Name = production_Line.WorkStation.WorkStation_Name;
                }
                if (production_Line.System_Project != null)
                {
                    dto.Project_Name = production_Line.System_Project.Project_Name;
                }
                if (production_Line.Process_Info != null)
                {
                    dto.Process_Name = production_Line.Process_Info.Process_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(production_Line.System_Users1);
                production_LineDTOList.Add(dto);
            }

            return production_LineDTOList;
        }

        public Production_Line QueryProduction_LineSingle(int uid)
        {
            return production_LineRepository.GetById(uid);
        }

        public string AddProduction_Line(Production_Line Production_Line)
        {
            production_LineRepository.Add(Production_Line);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditProduction_Line(Production_Line Production_Line)
        {
            production_LineRepository.Update(Production_Line);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteProduction_Line(int uid)
        {
            var production_Line = production_LineRepository.GetFirstOrDefault(w => w.Production_Line_UID == uid);
            if (production_Line != null)
            {
                //如果有关联的数据则不可删
                var fixture_MCount = production_Line.Fixture_M.Count();
                var fixture_MachineCount = production_Line.Fixture_Machine.Count();
                if (fixture_MCount > 0 || fixture_MachineCount > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    production_LineRepository.Delete(production_Line);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<Production_LineDTO> DoExportProduction_Line(string uids)
        {
            var totalCount = 0;
            var production_Lines = production_LineRepository.QueryProduction_Lines(new Production_LineModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<Production_LineDTO> production_LineDTOList = new List<Production_LineDTO>();

            foreach (var production_Line in production_Lines)
            {
                var dto = AutoMapper.Mapper.Map<Production_LineDTO>(production_Line);
                if (production_Line.System_Organization != null)
                {
                    dto.Plant_Organization_Name = production_Line.System_Organization.Organization_Name;
                }
                if (production_Line.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = production_Line.System_Organization1.Organization_Name;
                }
                if (production_Line.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = production_Line.System_Organization2.Organization_Name;
                }
                if (production_Line.Workshop != null)
                {
                    dto.Workshop_Name = production_Line.Workshop.Workshop_Name;
                }
                if (production_Line.WorkStation != null)
                {
                    dto.Workstation_Name = production_Line.WorkStation.WorkStation_Name;
                }
                if (production_Line.System_Project != null)
                {
                    dto.Project_Name = production_Line.System_Project.Project_Name;
                }
                if (production_Line.Process_Info != null)
                {
                    dto.Process_Name = production_Line.Process_Info.Process_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(production_Line.System_Users1);
                production_LineDTOList.Add(dto);
            }
            return production_LineDTOList;
        }

        public List<Production_LineDTO> QueryAllProduction_Lines()
        {
            var productionLines = production_LineRepository.GetAll().ToList();
            var dtoList = new List<Production_LineDTO>();
            foreach (var item in productionLines)
            {
                dtoList.Add(AutoMapper.Mapper.Map<Production_LineDTO>(item));
            }
            return dtoList;
        }

        public string InsertProduction_Lines(List<Production_Line> list)
        {
            string result = "";
            try
            {
                production_LineRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }
        #endregion

        #region 机台维护
        public bool IsFixture_MachineExist(Fixture_MachineModelSearch search)
        {
            var isExist = false;
            var query = fixture_MachineRepository.GetAll();
            if (search.Fixture_Machine_UID.HasValue)
            {
                query = query.Where(i => i.Fixture_Machine_UID != search.Fixture_Machine_UID.Value);
            }
            var machine = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.Machine_ID == search.Machine_ID && d.Production_Line_UID == search.Production_Line_UID);
            if (machine != null)
            {
                isExist = true;
            }
            return isExist;
        }
        public PagedListModel<FixtureMachineDTO> QueryFixture_Machines(Fixture_MachineModelSearch search, Page page)
        {
            var totalCount = 0;
            //TODO steven 这边把search条件，取fixture_MachineRepository.QueryFixture_Machines资料
            var Fixture_Machines = fixture_MachineRepository.QueryFixture_Machines(search, page, out totalCount);

            IList<FixtureMachineDTO> fixture_MachineDTOList = new List<FixtureMachineDTO>();

            foreach (var fixture_Machine in Fixture_Machines)
            {
                var dto = AutoMapper.Mapper.Map<FixtureMachineDTO>(fixture_Machine);
                if (fixture_Machine.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_Machine.System_Organization.Organization_Name;
                }
                if (fixture_Machine.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_Machine.System_Organization1.Organization_Name;
                }
                if (fixture_Machine.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_Machine.System_Organization2.Organization_Name;
                }
                if (fixture_Machine.Production_Line != null)
                {
                    dto.Production_Line_Name = fixture_Machine.Production_Line.Line_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Machine.System_Users1);
                fixture_MachineDTOList.Add(dto);
            }

            return new PagedListModel<FixtureMachineDTO>(totalCount, fixture_MachineDTOList);
        }

        public IList<FixtureMachineDTO> GetFixtureMachineList(Fixture_MachineModelSearch search)
        {
            var query = fixture_MachineRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID.HasValue && search.FunPlant_Organization_UID.Value > 0)
                {
                    query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                }
                if (search.Production_Line_UID.HasValue)
                {
                    query = query.Where(m => m.Production_Line_UID == search.Production_Line_UID.Value);
                }
                if (!string.IsNullOrWhiteSpace(search.Machine_ID))
                {
                    query = query.Where(m => m.Machine_ID.Contains(search.Machine_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.Equipment_No))
                {
                    query = query.Where(m => m.Equipment_No.Contains(search.Equipment_No));
                }
                if (!string.IsNullOrWhiteSpace(search.Machine_Name))
                {
                    query = query.Where(m => m.Machine_Name.Contains(search.Machine_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Machine_Desc))
                {
                    query = query.Where(m => m.Machine_Desc.Contains(search.Machine_Desc));
                }
                if (!string.IsNullOrWhiteSpace(search.Machine_Type))
                {
                    query = query.Where(m => m.Machine_Type.Contains(search.Machine_Type));
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(m => m.Is_Enable == search.Is_Enable.Value);
                }
            }

            query = query.OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Fixture_Machine_UID);

            IList<FixtureMachineDTO> fixture_MachineDTOList = new List<FixtureMachineDTO>();

            foreach (var fixture_Machine in query)
            {
                var dto = AutoMapper.Mapper.Map<FixtureMachineDTO>(fixture_Machine);
                if (fixture_Machine.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_Machine.System_Organization.Organization_Name;
                }
                if (fixture_Machine.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_Machine.System_Organization1.Organization_Name;
                }
                if (fixture_Machine.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_Machine.System_Organization2.Organization_Name;
                }
                if (fixture_Machine.Production_Line != null)
                {
                    dto.Production_Line_Name = fixture_Machine.Production_Line.Line_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Machine.System_Users1);
                fixture_MachineDTOList.Add(dto);
            }
            return fixture_MachineDTOList;
        }

        public Fixture_Machine QueryFixture_MachineSingle(int uid)
        {
            return fixture_MachineRepository.GetById(uid);
        }

        public string AddFixture_Machine(Fixture_Machine fixtureMachine)
        {
            fixture_MachineRepository.Add(fixtureMachine);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditFixture_Machine(Fixture_Machine fixtureMachine)
        {
            fixture_MachineRepository.Update(fixtureMachine);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteFixture_Machine(int uid)
        {
            var fixtureMachine = fixture_MachineRepository.GetFirstOrDefault(w => w.Fixture_Machine_UID == uid);
            if (fixtureMachine != null)
            {
                //如果有关联的数据则不可删
                var fixture_MCount = fixtureMachine.Fixture_M.Count();
                if (fixture_MCount > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    fixture_MachineRepository.Delete(fixtureMachine);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<FixtureMachineDTO> DoExportFixture_Machine(string uids)
        {
            var totalCount = 0;
            var fixture_Machines = fixture_MachineRepository.QueryFixture_Machines(new Fixture_MachineModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<FixtureMachineDTO> fixture_MachineDTOList = new List<FixtureMachineDTO>();

            foreach (var fixture_Machine in fixture_Machines)
            {
                var dto = AutoMapper.Mapper.Map<FixtureMachineDTO>(fixture_Machine);
                if (fixture_Machine.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_Machine.System_Organization.Organization_Name;
                }
                if (fixture_Machine.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_Machine.System_Organization1.Organization_Name;
                }

                if (fixture_Machine.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_Machine.System_Organization2.Organization_Name;
                }
                if (fixture_Machine.Production_Line != null)
                {
                    dto.Production_Line_Name = fixture_Machine.Production_Line.Line_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Machine.System_Users1);
                fixture_MachineDTOList.Add(dto);
            }
            return fixture_MachineDTOList;
        }

        public List<FixtureMachineDTO> QueryAllFixture_Machines()
        {
            var machines = fixture_MachineRepository.GetAll().ToList();
            var dtoList = new List<FixtureMachineDTO>();
            foreach (var item in machines)
            {
                dtoList.Add(AutoMapper.Mapper.Map<FixtureMachineDTO>(item));
            }
            return dtoList;
        }

        public string InsertFixture_Machines(List<Fixture_Machine> list)
        {
            string result = "";
            try
            {
                fixture_MachineRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }

        //TODO 2018/09/18 steven add 取设备信息檔功能
        public List<EquipmentInfoDTO> GetEquipmentInfoList(int Plant_Organization_UID, int? BG_Organization_UID, int? FunPlant_Organization_UID)
        {

            var query = equipmentInfoRepository.GetAll();
            if (Plant_Organization_UID > 0)
            {
                query = query.Where(w => w.Plant_Organization_UID == Plant_Organization_UID);
            }
            if (BG_Organization_UID.HasValue)
            {
                query = query.Where(w => w.BG_Organization_UID == BG_Organization_UID.Value);
            }
            if (FunPlant_Organization_UID.HasValue)
            {
                query = query.Where(w => w.FunPlant_Organization_UID == FunPlant_Organization_UID.Value);
            }

            query = query.OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.FunPlant_Organization_UID);

            var dtoList = new List<EquipmentInfoDTO>();

            foreach (var DataQuery in query.ToList())
            {
                dtoList.Add(AutoMapper.Mapper.Map<EquipmentInfoDTO>(DataQuery)); ;
            }
            return dtoList;

        }
        //TODO 2018/09/18 steven add 治具机台批量新增
        public int BatchAppendExcute(int UserID, int? Plant_Organization_UID, int? BG_Organization_UID, int? FunPlant_Organization_UID)
        {
            //用厂区，OP，功能厂过滤
            var equipmentResult = equipmentInfoRepository.GetAll();
            var fixture_MachineResult = fixture_MachineRepository.GetAll();
            if (Plant_Organization_UID.HasValue)
            {
                equipmentResult = equipmentResult.Where(x => x.Plant_Organization_UID == Plant_Organization_UID.Value);
                fixture_MachineResult = fixture_MachineResult.Where(x => x.Plant_Organization_UID == Plant_Organization_UID.Value);
            }
            if (BG_Organization_UID.HasValue)
            {
                equipmentResult = equipmentResult.Where(x => x.BG_Organization_UID == BG_Organization_UID.Value);
                fixture_MachineResult = fixture_MachineResult.Where(x => x.BG_Organization_UID == BG_Organization_UID.Value);
            }
            if (FunPlant_Organization_UID.HasValue)
            {
                equipmentResult = equipmentResult.Where(x => x.FunPlant_Organization_UID == FunPlant_Organization_UID.Value);
                fixture_MachineResult = fixture_MachineResult.Where(x => x.FunPlant_Organization_UID == FunPlant_Organization_UID.Value);
            }

            //过滤出不存在于Fixture_Machine 的Equipment_Info，这部分Equipment_Info 转换成Fixture_Machine 新增到Fixture_Machine
            var fixtureMachineUid = fixture_MachineResult.Select(x => x.EQP_Uid);
            equipmentResult = equipmentResult.Where(e => !fixtureMachineUid.Contains(e.EQP_Uid));
            var newFixtureMachineList = equipmentResult.Select(x => new Fixture_Machine()
            {
                Plant_Organization_UID = x.Plant_Organization_UID,
                BG_Organization_UID = x.BG_Organization_UID,
                FunPlant_Organization_UID = x.FunPlant_Organization_UID,
                EQP_Uid = x.EQP_Uid,
                Equipment_No = x.Equipment,
                Machine_Type = x.Class_Desc,
                Created_Date = DateTime.Now,
                Created_UID = UserID,
                Modified_Date = DateTime.Now,
                Modified_UID = UserID,
                Machine_ID = "122",
                Machine_Name = "AA",
                Production_Line_UID = 11,   //这个外键没法构造
                Is_Enable = true
            }).ToList();
            if (newFixtureMachineList.Count > 0)
            {
                fixture_MachineRepository.AddList(newFixtureMachineList);
                unitOfWork.Commit();
            }
            return newFixtureMachineList.Count;
        }
        #endregion

        #region 设备异常原因维护
        public bool IsFixture_DefectCodeExist(Fixture_DefectCodeModelSearch search)
        {
            var isExist = false;
            var query = fixture_DefectCodeRepository.GetAll();
            if (search.Fixture_Defect_UID.HasValue)
            {
                query = query.Where(i => i.Fixture_Defect_UID != search.Fixture_Defect_UID.Value);
            }
            var defectCode = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.DefectCode_ID == search.DefectCode_ID);
            if (defectCode != null)
            {
                isExist = true;
            }
            return isExist;
        }
        public PagedListModel<Fixture_DefectCodeDTO> QueryFixture_DefectCodes(Fixture_DefectCodeModelSearch search, Page page)
        {
            var totalCount = 0;
            var Fixture_DefectCodes = fixture_DefectCodeRepository.QueryFixture_DefectCodes(search, page, out totalCount);

            IList<Fixture_DefectCodeDTO> fixture_DefectCodeDTOList = new List<Fixture_DefectCodeDTO>();

            foreach (var fixture_DefectCode in Fixture_DefectCodes)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_DefectCodeDTO>(fixture_DefectCode);
                if (fixture_DefectCode.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_DefectCode.System_Organization.Organization_Name;
                }
                if (fixture_DefectCode.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_DefectCode.System_Organization1.Organization_Name;
                }
                if (fixture_DefectCode.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_DefectCode.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_DefectCode.System_Users1);
                fixture_DefectCodeDTOList.Add(dto);
            }

            return new PagedListModel<Fixture_DefectCodeDTO>(totalCount, fixture_DefectCodeDTOList);
        }

        public IList<Fixture_DefectCodeDTO> GetFixtureDefectCodeList(Fixture_DefectCodeModelSearch search)
        {
            var query = fixture_DefectCodeRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID.HasValue)
                {
                    query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                }
                if (!string.IsNullOrWhiteSpace(search.DefectCode_ID))
                {
                    query = query.Where(m => m.DefectCode_ID.Contains(search.DefectCode_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.DefectCode_Name))
                {
                    query = query.Where(m => m.DefectCode_Name.Contains(search.DefectCode_Name));
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(m => m.Is_Enable == search.Is_Enable.Value);
                }
            }
            query = query.OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Fixture_Defect_UID);

            IList<Fixture_DefectCodeDTO> fixture_DefectCodeDTOList = new List<Fixture_DefectCodeDTO>();

            foreach (var fixture_DefectCode in query)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_DefectCodeDTO>(fixture_DefectCode);
                if (fixture_DefectCode.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_DefectCode.System_Organization.Organization_Name;
                }
                if (fixture_DefectCode.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_DefectCode.System_Organization1.Organization_Name;
                }
                if (fixture_DefectCode.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_DefectCode.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_DefectCode.System_Users1);
                fixture_DefectCodeDTOList.Add(dto);
            }

            return fixture_DefectCodeDTOList;
        }

        public Fixture_DefectCode QueryFixture_DefectCodeSingle(int uid)
        {
            return fixture_DefectCodeRepository.GetById(uid);
        }

        public string AddFixture_DefectCode(Fixture_DefectCode fixtureDefectCode)
        {
            try
            {
                fixture_DefectCodeRepository.Add(fixtureDefectCode);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "保存失败";
            }
        }

        public string EditFixture_DefectCode(Fixture_DefectCode fixtureDefectCode)
        {
            try
            {
                fixture_DefectCodeRepository.Update(fixtureDefectCode);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "保存失败";
            }
        }

        public string DeleteFixture_DefectCode(int uid)
        {
            var fixtureDefectCode = fixture_DefectCodeRepository.GetFirstOrDefault(w => w.Fixture_Defect_UID == uid);
            if (fixtureDefectCode != null)
            {
                //如果有关联的数据则不可删
                var defectCodeGroupCount = fixtureDefectCode.DefectCode_Group.Count();
                var repairSolutionCount = fixtureDefectCode.DefectCode_RepairSolution.Count();
                var defectCodeSettingCount = fixtureDefectCode.FixtureDefectCode_Setting.Count();
                var fixtureRepairDDefect = fixtureDefectCode.Fixture_Repair_D_Defect.Count();
                if (defectCodeGroupCount > 0 || repairSolutionCount > 0 || defectCodeGroupCount > 0 || fixtureRepairDDefect > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    fixture_DefectCodeRepository.Delete(fixtureDefectCode);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<Fixture_DefectCodeDTO> DoExportFixture_DefectCode(string uids)
        {
            var totalCount = 0;
            var fixture_DefectCodes = fixture_DefectCodeRepository.QueryFixture_DefectCodes(new Fixture_DefectCodeModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<Fixture_DefectCodeDTO> fixture_DefectCodeDTOList = new List<Fixture_DefectCodeDTO>();

            foreach (var fixture_DefectCode in fixture_DefectCodes)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_DefectCodeDTO>(fixture_DefectCode);
                if (fixture_DefectCode.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_DefectCode.System_Organization.Organization_Name;
                }
                if (fixture_DefectCode.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_DefectCode.System_Organization1.Organization_Name;
                }
                if (fixture_DefectCode.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_DefectCode.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_DefectCode.System_Users1);
                fixture_DefectCodeDTOList.Add(dto);
            }
            return fixture_DefectCodeDTOList;
        }

        public List<Fixture_DefectCodeDTO> QueryAllFixture_DefectCodes()
        {
            var fixtureDefects = fixture_DefectCodeRepository.GetAll().ToList();
            var dtoList = new List<Fixture_DefectCodeDTO>();
            foreach (var item in fixtureDefects)
            {
                dtoList.Add(AutoMapper.Mapper.Map<Fixture_DefectCodeDTO>(item));
            }
            return dtoList;
        }
        public string InsertFixture_DefectCodes(List<Fixture_DefectCode> list)
        {
            string result = "";
            try
            {
                fixture_DefectCodeRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }

        public Fixture_DefectCodeDTO GetFixture_DefectCode(string DefectCode_ID, int Plant_Organization_UID, int BG_Organization_UID)
        {
            return fixture_DefectCodeRepository.GetFixture_DefectCode(DefectCode_ID, Plant_Organization_UID, BG_Organization_UID);

        }
        #endregion


        #region 维修对策维护
        public bool IsFixture_RepairSolutionExist(Fixture_RepairSolutionModelSearch search)
        {
            var isExist = false;
            var query = fixture_RepairSolutionRepository.GetAll();
            if (search.Fixture_RepairSolution_UID.HasValue)
            {
                query = query.Where(i => i.Fixture_RepairSolution_UID != search.Fixture_RepairSolution_UID.Value);
            }
            var defectCode = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.RepairSolution_ID == search.RepairSolution_ID);
            if (defectCode != null)
            {
                isExist = true;
            }
            return isExist;
        }

        public PagedListModel<Fixture_RepairSolutionDTO> QueryFixture_RepairSolutions(Fixture_RepairSolutionModelSearch search, Page page)
        {
            var totalCount = 0;
            var Fixture_RepairSolutions = fixture_RepairSolutionRepository.QueryFixture_RepairSolutions(search, page, out totalCount);

            IList<Fixture_RepairSolutionDTO> fixture_RepairSolutionDTOList = new List<Fixture_RepairSolutionDTO>();

            foreach (var fixture_RepairSolution in Fixture_RepairSolutions)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_RepairSolutionDTO>(fixture_RepairSolution);
                if (fixture_RepairSolution.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_RepairSolution.System_Organization.Organization_Name;
                }
                if (fixture_RepairSolution.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_RepairSolution.System_Organization1.Organization_Name;
                }
                if (fixture_RepairSolution.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_RepairSolution.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_RepairSolution.System_Users1);
                fixture_RepairSolutionDTOList.Add(dto);
            }

            return new PagedListModel<Fixture_RepairSolutionDTO>(totalCount, fixture_RepairSolutionDTOList);
        }

        /// <summary>
        /// 根据查询条件获取Fixture_RepairSolutionDTO
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public IList<Fixture_RepairSolutionDTO> GetRepairSolutionList(Fixture_RepairSolutionModelSearch search)
        {
            var query = fixture_RepairSolutionRepository.GetAll();
            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.Plant_Organization_UID > 0)
                    {
                        query = query.Where(r => r.Plant_Organization_UID == search.Plant_Organization_UID);
                    }
                    if (search.BG_Organization_UID > 0)
                    {
                        query = query.Where(r => r.BG_Organization_UID == search.BG_Organization_UID);
                    }
                    if (search.FunPlant_Organization_UID.HasValue)
                    {
                        query = query.Where(r => r.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(search.RepairSolution_ID))
                    {
                        query = query.Where(r => r.RepairSolution_ID.Contains(search.RepairSolution_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.RepairSolution_Name))
                    {
                        query = query.Where(r => r.RepairSolution_Name.Contains(search.RepairSolution_Name));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(r => r.Is_Enable == search.Is_Enable.Value);
                    }
                }
                query = query.OrderBy(r => r.Plant_Organization_UID).ThenBy(r => r.BG_Organization_UID).ThenBy(r => r.Fixture_RepairSolution_UID);
            }

            IList<Fixture_RepairSolutionDTO> fixture_RepairSolutionDTOList = new List<Fixture_RepairSolutionDTO>();

            foreach (var fixture_RepairSolution in query)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_RepairSolutionDTO>(fixture_RepairSolution);
                if (fixture_RepairSolution.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_RepairSolution.System_Organization.Organization_Name;
                }
                if (fixture_RepairSolution.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_RepairSolution.System_Organization1.Organization_Name;
                }
                if (fixture_RepairSolution.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_RepairSolution.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_RepairSolution.System_Users1);
                fixture_RepairSolutionDTOList.Add(dto);
            }

            return fixture_RepairSolutionDTOList;
        }

        public Fixture_RepairSolution QueryFixture_RepairSolutionSingle(int uid)
        {
            return fixture_RepairSolutionRepository.GetById(uid);
        }

        public string AddFixture_RepairSolution(Fixture_RepairSolution fixtureDefectCode)
        {
            fixture_RepairSolutionRepository.Add(fixtureDefectCode);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditFixture_RepairSolution(Fixture_RepairSolution fixtureDefectCode)
        {
            fixture_RepairSolutionRepository.Update(fixtureDefectCode);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteFixture_RepairSolution(int uid)
        {
            var fixtureDefectCode = fixture_RepairSolutionRepository.GetFirstOrDefault(w => w.Fixture_RepairSolution_UID == uid);
            if (fixtureDefectCode != null)
            {
                //如果有关联的数据则不可删
                var repairSolutionCount = fixtureDefectCode.DefectCode_RepairSolution.Count();
                var fixtureRepairDefectCount = fixtureDefectCode.Fixture_Repair_D_Defect.Count();
                if (repairSolutionCount > 0 || fixtureRepairDefectCount > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    fixture_RepairSolutionRepository.Delete(fixtureDefectCode);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<Fixture_RepairSolutionDTO> DoExportFixture_RepairSolution(string uids)
        {
            var totalCount = 0;
            var fixture_RepairSolutions = fixture_RepairSolutionRepository.QueryFixture_RepairSolutions(new Fixture_RepairSolutionModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<Fixture_RepairSolutionDTO> fixture_RepairSolutionDTOList = new List<Fixture_RepairSolutionDTO>();

            foreach (var fixture_RepairSolution in fixture_RepairSolutions)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_RepairSolutionDTO>(fixture_RepairSolution);
                if (fixture_RepairSolution.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_RepairSolution.System_Organization.Organization_Name;
                }
                if (fixture_RepairSolution.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_RepairSolution.System_Organization1.Organization_Name;
                }
                if (fixture_RepairSolution.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_RepairSolution.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_RepairSolution.System_Users1);
                fixture_RepairSolutionDTOList.Add(dto);
            }
            return fixture_RepairSolutionDTOList;
        }


        public List<Fixture_RepairSolutionDTO> QueryAllFixture_RepairSolutions()
        {
            var fixtureRepairSolutions = fixture_RepairSolutionRepository.GetAll().ToList();
            var dtoList = new List<Fixture_RepairSolutionDTO>();
            foreach (var item in fixtureRepairSolutions)
            {
                dtoList.Add(AutoMapper.Mapper.Map<Fixture_RepairSolutionDTO>(item));
            }
            return dtoList;
        }

        public List<DefectRepairSolutionDTO> QueryAllDefectCode_RepairSolution()
        {
            var fixtureRepairSolutions = defectRepairRepository.GetAll().ToList();
            var dtoList = new List<DefectRepairSolutionDTO>();
            foreach (var item in fixtureRepairSolutions)
            {
                dtoList.Add(AutoMapper.Mapper.Map<DefectRepairSolutionDTO>(item));
            }
            return dtoList;
        }
        public string InsertFixture_RepairSolutions(List<Fixture_RepairSolution> list)
        {
            string result = "";
            try
            {
                fixture_RepairSolutionRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }
        #endregion

        #region 车间维护
        public bool IsWorkshopExist(WorkshopModelSearch search)
        {
            var isExist = false;
            var query = workshopRepository.GetAll();
            if (search.Workshop_UID.HasValue)
            {
                query = query.Where(i => i.Workshop_UID != search.Workshop_UID.Value);
            }
            var defectCode = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.FunPlant_Organization_UID == search.FunPlant_Organization_UID && d.Workshop_ID == search.Workshop_ID);
            if (defectCode != null)
            {
                isExist = true;
            }
            return isExist;
        }
        public string InsertWorkshops(List<Workshop> list)
        {
            string result = "";
            try
            {
                workshopRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }
        #endregion

        #region 制程维护
        public bool IsProcess_InfoExist(Process_InfoModelSearch search)
        {
            var isExist = false;
            var query = processInfoRepository.GetAll();
            if (search.Process_Info_UID.HasValue)
            {
                query = query.Where(i => i.Process_Info_UID != search.Process_Info_UID.Value);
            }
            var defectCode = query.FirstOrDefault(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.Process_ID == search.Process_ID);
            if (defectCode != null)
            {
                isExist = true;
            }
            return isExist;
        }
        public List<Process_InfoDTO> QueryAllProcess_Infos()
        {
            var processInfos = processInfoRepository.GetAll().ToList();
            var dtoList = new List<Process_InfoDTO>();
            foreach (var item in processInfos)
            {
                dtoList.Add(AutoMapper.Mapper.Map<Process_InfoDTO>(item));
            }
            return dtoList;
        }

        public string InsertProcess_Infos(List<Process_Info> list)
        {
            string result = "";
            try
            {
                processInfoRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }
        #endregion

        #region 治具配件维护
        public bool IsFixture_PartExist(Fixture_PartModelSearch search)
        {
            var isExist = false;
            var query = fixturePartRepository.GetAll();
            if (search.Fixture_Part_UID.HasValue)
            {
                query = query.Where(i => i.Fixture_Part_UID != search.Fixture_Part_UID.Value);
            }
            //料号Part_ID 不能重复
            isExist = query.Any(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.Part_ID == search.Part_ID);

            return isExist;
        }
        public PagedListModel<Fixture_PartDTO> QueryFixture_Parts(Fixture_PartModelSearch search, Page page)
        {
            var totalCount = 0;
            var fixture_Parts = fixturePartRepository.QueryFixtureParts(search, page, out totalCount);

            IList<Fixture_PartDTO> fixture_MachineDTOList = new List<Fixture_PartDTO>();

            foreach (var fixture_Part in fixture_Parts)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_PartDTO>(fixture_Part);
                if (fixture_Part.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_Part.System_Organization.Organization_Name;
                }
                if (fixture_Part.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_Part.System_Organization1.Organization_Name;
                }
                if (fixture_Part.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_Part.System_Organization2.Organization_Name;
                }
                if (fixture_Part.System_Users != null)
                {
                    dto.Created_UserName = fixture_Part.System_Users.User_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Part.System_Users1);
                fixture_MachineDTOList.Add(dto);
            }

            return new PagedListModel<Fixture_PartDTO>(totalCount, fixture_MachineDTOList);
        }

        public IList<Fixture_PartDTO> GetFixturePartList(Fixture_PartModelSearch search)
        {
            var query = fixturePartRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID.HasValue)
                {
                    query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                }
                if (!string.IsNullOrWhiteSpace(search.Part_ID))
                {
                    query = query.Where(w => w.Part_ID.Contains(search.Part_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.Part_Name))
                {
                    query = query.Where(w => w.Part_Name.Contains(search.Part_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Part_Spec))
                {
                    query = query.Where(w => w.Part_Spec.Contains(search.Part_Spec));
                }
                if (search.Purchase_Cycle.HasValue)
                {
                    query = query.Where(w => w.Purchase_Cycle == search.Purchase_Cycle.Value);
                }
                if (search.Is_Automation.HasValue)
                {
                    query = query.Where(w => w.Is_Automation == search.Is_Automation.Value);
                }
                if (search.Is_Standardized.HasValue)
                {
                    query = query.Where(w => w.Is_Standardized == search.Is_Standardized.Value);
                }
                if (search.Is_Storage_Managed.HasValue)
                {
                    query = query.Where(w => w.Is_Storage_Managed == search.Is_Storage_Managed.Value);
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
                }
            }

            query = query.OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Part_ID);

            IList<Fixture_PartDTO> fixture_MachineDTOList = new List<Fixture_PartDTO>();

            foreach (var fixture_Machine in query)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_PartDTO>(fixture_Machine);
                if (fixture_Machine.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_Machine.System_Organization.Organization_Name;
                }
                if (fixture_Machine.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_Machine.System_Organization1.Organization_Name;
                }
                if (fixture_Machine.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_Machine.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Machine.System_Users1);
                fixture_MachineDTOList.Add(dto);
            }
            return fixture_MachineDTOList;
        }

        public Fixture_Part QueryFixture_PartSingle(int uid)
        {
            return fixturePartRepository.GetById(uid);
        }

        public string AddFixture_Part(Fixture_Part fixturePart)
        {
            fixturePartRepository.Add(fixturePart);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string EditFixture_Part(Fixture_Part fixturePart)
        {
            fixturePartRepository.Update(fixturePart);
            unitOfWork.Commit();
            return "SUCCESS";
        }

        public string DeleteFixture_Part(int uid)
        {
            var fixturePart = fixturePartRepository.GetFirstOrDefault(w => w.Fixture_Part_UID == uid);
            if (fixturePart != null)
            {
                //如果有关联的数据则不可删,未完成
                if (fixturePart.Fixture_Part_Demand_D.Count() > 0 || fixturePart.Fixture_Part_Demand_Summary_D.Count() > 0 || fixturePart.Fixture_Part_Inventory.Count() > 0
                    || fixturePart.Fixture_Part_Order_D.Count() > 0 || fixturePart.Fixture_Part_Setting_D.Count() > 0 || fixturePart.Fixture_Storage_Check.Count() > 0
                    || fixturePart.Fixture_Storage_Inbound.Count() > 0 || fixturePart.Fixture_Storage_InOut_Detail.Count() > 0 || fixturePart.Fixture_Storage_Outbound_D.Count() > 0
                    || fixturePart.Fixture_Storage_Transfer.Count() > 0)
                {
                    return "HAVEREFERENCE";
                }
                try
                {
                    fixturePartRepository.Delete(fixturePart);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public IEnumerable<Fixture_PartDTO> DoExportFixture_Part(string uids)
        {
            var totalCount = 0;
            var fixture_Parts = fixturePartRepository.QueryFixtureParts(new Fixture_PartModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<Fixture_PartDTO> fixture_PartDTOList = new List<Fixture_PartDTO>();

            foreach (var fixture_Part in fixture_Parts)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_PartDTO>(fixture_Part);
                if (fixture_Part.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_Part.System_Organization.Organization_Name;
                }
                if (fixture_Part.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_Part.System_Organization1.Organization_Name;
                }

                if (fixture_Part.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_Part.System_Organization2.Organization_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Part.System_Users1);
                fixture_PartDTOList.Add(dto);
            }
            return fixture_PartDTOList;
        }

        public List<Fixture_PartDTO> QueryAllFixture_Parts()
        {
            var machines = fixturePartRepository.GetAll().ToList();
            var dtoList = new List<Fixture_PartDTO>();
            foreach (var item in machines)
            {
                dtoList.Add(AutoMapper.Mapper.Map<Fixture_PartDTO>(item));
            }
            return dtoList;
        }

        public string InsertFixture_Parts(List<Fixture_Part> list)
        {
            string result = "";
            try
            {
                fixturePartRepository.AddList(list);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }
        #endregion

        #region 治具保养
        public PagedListModel<Fixture_Maintenance_RecordDTO> QueryFixtureMaintenance(Fixture_Maintenance_RecordDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixture_Maintenance_RecordRepository.QueryFixtureMaintenance(searchModel, page, out totalcount);
            return new PagedListModel<Fixture_Maintenance_RecordDTO>(totalcount, result);
        }


        public Fixture_Maintenance_RecordDTO QueryFixtureMaintenanceByUid(int Fixture_Maintenance_Record_UID)
        {
            var bud = fixture_Maintenance_RecordRepository.QueryFixtureMaintenanceByUid(Fixture_Maintenance_Record_UID);
            return bud;

        }
        public List<Fixture_Maintenance_RecordDTO> DoExportFixtureMaintenanceReprot(string Fixture_Maintenance_Record_UIDs)
        {
            return fixture_Maintenance_RecordRepository.DoExportFixtureMaintenanceReprot(Fixture_Maintenance_Record_UIDs);
        }
        public List<Fixture_Maintenance_RecordDTO> DoAllExportFixtureMaintenanceReprot(Fixture_Maintenance_RecordDTO search)
        {
            return fixture_Maintenance_RecordRepository.DoAllExportFixtureMaintenanceReprot(search);
        }

        public IEnumerable<Fixture_Maintenance_RecordDTO> GetFixtureMaintenance(string Fixture_Maintenance_Record_UIDs, int straus)
        {
            return fixture_Maintenance_RecordRepository.GetFixtureMaintenance(Fixture_Maintenance_Record_UIDs, straus);

        }
        public List<Fixture_Maintenance_RecordDTO> GetFixtureMaintenanceList(Fixture_Maintenance_RecordDTO dto, int straus)
        {
            return fixture_Maintenance_RecordRepository.GetFixtureMaintenanceList(dto, straus);

        }


        public string UpdateFixture_Maintenance_Record(string fixture_Maintenance_Record_UIDs, int NTID, string personNumber, string personName, DateTime date, int straus, int CurrentUserID)
        {

            List<Fixture_Maintenance_Record> Fixture_Maintenance_Records = fixture_Maintenance_RecordRepository.GetUpdateFixture_Maintenance_Record(fixture_Maintenance_Record_UIDs, straus);
            return fixture_Maintenance_RecordRepository.UpdateFixture_Maintenance_Record(Fixture_Maintenance_Records, NTID, personNumber, personName, date, straus, CurrentUserID);

        }

        public List<FixtureMaintenance_PlanDTO> GetFixtureMaintenance_Plan(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type)
        {
            return fixture_Maintenance_RecordRepository.GetFixtureMaintenance_Plan(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Maintenance_Type);

        }
        public string CreateFixture_Maintenance_Records(Fixture_Maintenance_RecordDTO dto)
        {

            int count = fixture_Maintenance_RecordRepository.GetFixtureCount(dto) + 1;
            string maintenance_Record_NO = "M" + DateTime.Now.Date.ToString("yyyyMMdd") + "_" + count.ToString().PadLeft(3, '0');
            List<Fixture_Maintenance_RecordDTO> recordDTOs = fixture_Maintenance_RecordRepository.GetFixture_Maintenance_RecordDTO(dto);
            if (Is12HMaintenance_Plan(dto.Maintenance_Plan_UID))
            {
                string strMDDate = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 08:30:00";
                //  string strEndDate = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 20:30 00";
                DateTime mdDate = DateTime.Parse(strMDDate);
                DateTime endDate = DateTime.Now;
                DateTime startDate = DateTime.Now;
                List<int> Fixture_M_UIDs = new List<int>();
                if (DateTime.Now >= mdDate)
                {
                    startDate = mdDate;
                    endDate = mdDate.AddHours(12);

                }
                else
                {
                    startDate = mdDate.AddHours(-12);
                    endDate = mdDate;

                }
                Fixture_M_UIDs = fixture_Maintenance_RecordRepository.GetAll().Where(o => o.Created_Date >= startDate && o.Created_Date <= endDate).Select(o => o.Fixture_M_UID).ToList();

                recordDTOs = recordDTOs.Where(o => !Fixture_M_UIDs.Contains(o.Fixture_M_UID)).ToList();
            }
            return fixture_Maintenance_RecordRepository.CreateFixture_Maintenance_Record(recordDTOs, dto, maintenance_Record_NO);

        }

        /// <summary>
        /// 判断12小时保养的数据在某一段时间只能生成一次保养
        /// </summary>
        /// <param name="Maintenance_Plan_UID"></param>
        /// <returns></returns>
        private bool Is12HMaintenance_Plan(int? Maintenance_Plan_UID)
        {
            bool is12HMaintenance_Plan = false;
            if (Maintenance_Plan_UID != null && Maintenance_Plan_UID != 0)
            {
                Maintenance_Plan maintenance_Plan = QueryMaintenanceSingle(Maintenance_Plan_UID.Value);

                if (maintenance_Plan != null && maintenance_Plan.Maintenance_Type == "D" && maintenance_Plan.Cycle_Interval == 12 && maintenance_Plan.Cycle_Unit == "H")
                {
                    is12HMaintenance_Plan = true;
                }
            }
            return is12HMaintenance_Plan;

        }
        #endregion 治具保养

        #region 治具维修
        public PagedListModel<FixtureRepairDTO> QueryFixtureRepairs(FixtureRepairSearch searchModel, Page page)
        {
            int totalcount;
            var result = fixtureRepairRepository.QueryFixtureRepairs(searchModel, page, out totalcount);
            return new PagedListModel<FixtureRepairDTO>(totalcount, result);
        }
        public IList<FixtureRepairDTO> GetFixtureRepairList(FixtureRepairSearch searchModel)
        {
            var query = fixtureRepairRepository.QueryFixtureRepairsByQuery(searchModel);
            return query.ToList();
        }

        public string AddFixtureRepair(Fixture_Repair_MDTO dto)
        {
            try
            {
                var fixtureRepairM = AutoMapper.Mapper.Map<Fixture_Repair_M>(dto);
                string repairNo = string.Format("R{0:yyyyMMdd_}", DateTime.Now);
                var fixtureRepairs = fixtureRepairRepository.GetMany(i => i.Repair_NO.Contains(repairNo));
                repairNo += string.Format("{0:000}", fixtureRepairs.Count() + 1);
                fixtureRepairM.Repair_NO = repairNo;
                var fixtureRepairMDb = fixtureRepairRepository.Add(fixtureRepairM);

                var fixtureResumeList = new List<Fixture_Resume>();
                foreach (var item in dto.Fixture_Repair_DDTOList)
                {
                    var fixtureRepairD = AutoMapper.Mapper.Map<Fixture_Repair_D>(item);
                    fixtureRepairD.Fixture_Repair_M_UID = fixtureRepairMDb.Fixture_Repair_M_UID;
                    var fixtureRepairDDb = fixtureRepairDRepository.Add(fixtureRepairD);
                    unitOfWork.Commit();
                    var defectRepairSolutionList = new List<Fixture_Repair_D_Defect>();
                    foreach (var subitem in item.Fixture_Repair_D_DefectDTOList)
                    {
                        var defectRepairSolution = AutoMapper.Mapper.Map<Fixture_Repair_D_Defect>(subitem);
                        defectRepairSolution.Fixture_Repair_D_UID = fixtureRepairDDb.Fixture_Repair_D_UID;
                        defectRepairSolutionList.Add(defectRepairSolution);

                    }
                    fixtureRepairDDefectRepository.AddList(defectRepairSolutionList);
                    var statusName = "";
                    var status = enumerationRepository.GetFirstOrDefault(i => i.Enum_UID == fixtureRepairDDb.Status);
                    if (status != null)
                    {
                        statusName = status.Enum_Value;
                    }
                    //新增Fixture_Resume 记录
                    var fixtureResume = new Fixture_Resume()
                    {
                        Fixture_M_UID = fixtureRepairDDb.Fixture_M_UID,
                        Data_Source = "4",
                        Resume_Date = fixtureRepairDDb.Created_Date,
                        Source_UID = fixtureRepairDDb.Fixture_Repair_D_UID,
                        Source_NO = fixtureRepairMDb.Repair_NO,
                        Resume_Notes = statusName,
                        Modified_UID = fixtureRepairDDb.Modified_UID,
                        Modified_Date = fixtureRepairDDb.Modified_Date
                    };
                    fixtureResumeList.Add(fixtureResume);
                    //修改＂治具主檔(Fixture_M)＂的治具狀態欄位值
                    var fixture = fixtureRepository.GetFirstOrDefault(i => i.Fixture_M_UID == fixtureRepairDDb.Fixture_M_UID);
                    if (fixture != null)
                    {
                        fixture.Status = fixtureRepairDDb.Status;
                    }
                    fixtureRepository.Update(fixture);
                }
                fixture_ResumeRepository.AddList(fixtureResumeList);
                unitOfWork.Commit();

                return "新增保存成功: 维修单号:" + repairNo;
            }
            catch (Exception ex)
            {
                return "新增保存失败:" + ex.Message;
            }
        }

        public string EditFixtureRepair(Fixture_Repair_MDTO dto)
        {
            try
            {
                //更新Fixture_Repair_M
                var fixtureRepairMDb = fixtureRepairRepository.GetFirstOrDefault(i => i.Repair_NO == dto.Repair_NO);
                fixtureRepairMDb.SentOut_Number = dto.SentOut_Number;
                fixtureRepairMDb.SentOut_Name = dto.SentOut_Name;
                fixtureRepairMDb.Receiver_UID = dto.Receiver_UID;
                fixtureRepairMDb.SentOut_Date = dto.SentOut_Date;
                fixtureRepairRepository.Update(fixtureRepairMDb);
                unitOfWork.Commit();

                //新增和编辑
                var fixtureResumeAddList = new List<Fixture_Resume>();
                foreach (var item in dto.Fixture_Repair_DDTOList)
                {
                    if (item.Fixture_Repair_D_UID.HasValue && item.Fixture_Repair_D_UID.Value != 0)
                    {
                        //编辑
                        var fixtureRepairDDb = fixtureRepairDRepository.GetFirstOrDefault(i => i.Fixture_Repair_D_UID == item.Fixture_Repair_D_UID.Value);
                        if (fixtureRepairDDb != null)
                        {
                            //fixtureRepairDDb.Fixture_Repair_M_UID = item.Fixture_Repair_M_UID;
                            //,Fixture_M_UID 治具不更新
                            fixtureRepairDDb.Repair_Staff_UID = item.Repair_Staff_UID;
                            fixtureRepairDDb.Completion_Date = item.Completion_Date;
                            fixtureRepairDDb.Status = item.Status;
                            fixtureRepairDDb.Modified_UID = item.Modified_UID;
                            fixtureRepairDDb.Modified_Date = item.Modified_Date;
                            fixtureRepairDRepository.Update(fixtureRepairDDb);
                            unitOfWork.Commit();
                        }

                        //修改＂治具主檔(Fixture_M)＂的治具狀態欄位值
                        var fixture = fixtureRepository.GetFirstOrDefault(i => i.Fixture_M_UID == fixtureRepairDDb.Fixture_M_UID);
                        if (fixture != null)
                        {
                            fixture.Status = fixtureRepairDDb.Status;
                            fixture.Modified_UID = item.Modified_UID;
                            fixture.Modified_Date = item.Modified_Date;
                            fixtureRepository.Update(fixture);
                            unitOfWork.Commit();
                        }
                        //更新fixture_Resume
                        var statusName = "";
                        var status = enumerationRepository.GetFirstOrDefault(i => i.Enum_UID == fixtureRepairDDb.Status);
                        if (status != null)
                        {
                            statusName = status.Enum_Value;
                        }
                        var fixtureResumeDb = fixture_ResumeRepository.GetFirstOrDefault(i => i.Source_NO == fixtureRepairMDb.Repair_NO && i.Fixture_M_UID == fixtureRepairDDb.Fixture_M_UID && i.Data_Source == "4" && i.Source_UID == fixtureRepairDDb.Fixture_Repair_D_UID);

                        if (fixtureResumeDb != null && fixtureResumeDb.Resume_Notes != statusName)
                        {
                            fixtureResumeDb.Resume_Notes = statusName;
                            fixtureResumeDb.Modified_UID = fixtureRepairDDb.Modified_UID;
                            fixtureResumeDb.Modified_Date = fixtureRepairDDb.Modified_Date;
                            fixture_ResumeRepository.Update(fixtureResumeDb);
                            unitOfWork.Commit();
                        }

                        //删除不在数据库存在的数据
                        var editUidList = new List<int>();
                        foreach (var subitem in item.Fixture_Repair_D_DefectDTOList)
                        {
                            if (subitem.Fixture_Repair_D_Defect_UID.HasValue && subitem.Fixture_Repair_D_Defect_UID != 0)
                            {
                                editUidList.Add(subitem.Fixture_Repair_D_Defect_UID.Value);
                            }
                        }
                        var fixture_Repair_D_DefectList = fixtureRepairDDb.Fixture_Repair_D_Defect;

                        var deleteList = fixture_Repair_D_DefectList.Where(f => !editUidList.Contains(f.Fixture_Repair_D_Defect_UID)).ToList();
                        if (deleteList.Count > 0)
                        {
                            fixtureRepairDDefectRepository.DeleteList(deleteList);
                            unitOfWork.Commit();
                        }

                        foreach (var subitem in item.Fixture_Repair_D_DefectDTOList)
                        {
                            //var fixtureRepairDDefectList = new List<Fixture_Repair_D_Defect>();
                            if (subitem.Fixture_Repair_D_Defect_UID.HasValue && subitem.Fixture_Repair_D_Defect_UID != 0)
                            {
                                //编辑
                                var fixtureRepairDDefectDb = fixtureRepairDDefectRepository.GetFirstOrDefault(i => i.Fixture_Repair_D_Defect_UID == subitem.Fixture_Repair_D_Defect_UID);
                                if (fixtureRepairDDefectDb != null)
                                {
                                    //fixtureRepairDDefectDb.Fixture_Repair_D_UID =subitem.Fixture_Repair_D_UID
                                    // fixtureRepairDDefectDb.Defect_Code_UID = subitem.
                                    fixtureRepairDDefectDb.Solution_UID = subitem.Solution_UID;
                                    fixtureRepairDDefectDb.Modified_UID = subitem.Modified_UID;
                                    fixtureRepairDDefectDb.Modified_Date = subitem.Modified_Date;
                                    fixtureRepairDDefectRepository.Update(fixtureRepairDDefectDb);
                                    unitOfWork.Commit();
                                }

                                editUidList.Add(subitem.Fixture_Repair_D_Defect_UID.Value);
                            }
                            else
                            {
                                //新增
                                var fixtureRepairDDefect = AutoMapper.Mapper.Map<Fixture_Repair_D_Defect>(subitem);
                                fixtureRepairDDefect.Fixture_Repair_D_UID = fixtureRepairDDb.Fixture_Repair_D_UID;
                                //fixtureRepairDDefectList.Add(fixtureRepairDDefect);
                                var x = fixtureRepairDDefectRepository.Add(fixtureRepairDDefect);
                                unitOfWork.Commit();

                                //编辑Fixture_Resume
                                //var fixtureResume = new Fixture_Resume()
                                //{
                                //    //Fixture_M_UID = fixtureRepairDDb.Fixture_M_UID,
                                //    //Data_Source = "4",
                                //    Resume_Date = fixtureRepairDDb.Created_Date,
                                //    //Source_UID = fixtureRepairDDb.Fixture_Repair_D_UID,
                                //    //Source_NO = fixtureRepairMDb.Repair_NO,
                                //    Resume_Notes = statusName,
                                //    Modified_UID = fixtureRepairDDb.Modified_UID,
                                //    Modified_Date = fixtureRepairDDb.Modified_Date
                                //};

                            }
                        }
                    }
                    else
                    {
                        //新增
                        var fixtureRepairD = AutoMapper.Mapper.Map<Fixture_Repair_D>(item);
                        fixtureRepairD.Fixture_Repair_M_UID = fixtureRepairMDb.Fixture_Repair_M_UID;
                        var fixtureRepairDDb = fixtureRepairDRepository.Add(fixtureRepairD);
                        unitOfWork.Commit();
                        var defectRepairSolutionList = new List<Fixture_Repair_D_Defect>();
                        foreach (var subitem in item.Fixture_Repair_D_DefectDTOList)
                        {
                            var defectRepairSolution = AutoMapper.Mapper.Map<Fixture_Repair_D_Defect>(subitem);
                            defectRepairSolution.Fixture_Repair_D_UID = fixtureRepairDDb.Fixture_Repair_D_UID;
                            defectRepairSolutionList.Add(defectRepairSolution);
                        }
                        fixtureRepairDDefectRepository.AddList(defectRepairSolutionList);
                        unitOfWork.Commit();
                        var statusName = "";
                        var status = enumerationRepository.GetFirstOrDefault(i => i.Enum_UID == fixtureRepairDDb.Status);
                        if (status != null)
                        {
                            statusName = status.Enum_Value;
                        }
                        //新增Fixture_Resume 记录
                        var fixtureResume = new Fixture_Resume()
                        {
                            Fixture_M_UID = fixtureRepairDDb.Fixture_M_UID,
                            Data_Source = "4",
                            Resume_Date = fixtureRepairDDb.Created_Date,
                            Source_UID = fixtureRepairDDb.Fixture_Repair_D_UID,
                            Source_NO = fixtureRepairMDb.Repair_NO,
                            Resume_Notes = statusName,
                            Modified_UID = fixtureRepairDDb.Modified_UID,
                            Modified_Date = fixtureRepairDDb.Modified_Date
                        };
                        fixtureResumeAddList.Add(fixtureResume);
                        //修改＂治具主檔(Fixture_M)＂的治具狀態欄位值
                        var fixture = fixtureRepository.GetFirstOrDefault(i => i.Fixture_M_UID == fixtureRepairDDb.Fixture_M_UID);
                        if (fixture != null)
                        {
                            fixture.Status = fixtureRepairDDb.Status;
                            fixture.Modified_UID = item.Modified_UID;
                            fixture.Modified_Date = item.Modified_Date;
                            fixtureRepository.Update(fixture);
                            unitOfWork.Commit();
                        }
                    }
                }
                //fixture_ResumeRepository.AddList(fixtureResumeList);
                //提交保存
                unitOfWork.Commit();

                return "编辑保存成功: 维修单号:" + dto.Repair_NO;
            }
            catch (Exception ex)
            {
                return "编辑保存失败:" + ex.Message;
            }
        }

        public Fixture_Repair_MDTO GetFixtureRepairByRepairNo(string repairNo)
        {
            var fixtureRepairM = fixtureRepairRepository.GetFirstOrDefault(i => i.Repair_NO == repairNo);
            var fixtureRepairMDTO = AutoMapper.Mapper.Map<Fixture_Repair_MDTO>(fixtureRepairM);

            fixtureRepairMDTO.Plant_Organization_Name = fixtureRepairM.System_Organization.Organization_Name;
            fixtureRepairMDTO.BG_Organization_Name = fixtureRepairM.System_Organization1.Organization_Name;
            if (fixtureRepairM.FunPlant_Organization_UID.HasValue && fixtureRepairM.FunPlant_Organization_UID.Value != 0)
            {
                fixtureRepairMDTO.FunPlant_Organization_Name = fixtureRepairM.System_Organization2.Organization_Name;
            }
            fixtureRepairMDTO.Repair_Location_Name = fixtureRepairM.Repair_Location.Repair_Location_Name;

            fixtureRepairMDTO.SentOut_Number = fixtureRepairM.SentOut_Number;
            fixtureRepairMDTO.SentOut_Name = fixtureRepairM.SentOut_Name;

            fixtureRepairMDTO.Receiver_NTID = fixtureRepairM.System_Users2.User_NTID;
            fixtureRepairMDTO.Receiver_Name = fixtureRepairM.System_Users2.User_Name;

            fixtureRepairMDTO.Fixture_Repair_DDTOList = new List<Fixture_Repair_DDTO>();
            foreach (var item in fixtureRepairM.Fixture_Repair_D)
            {
                var repariDDTO = AutoMapper.Mapper.Map<Fixture_Repair_DDTO>(item);
                repariDDTO.Repair_Staff_NTID = item.System_Users2.User_NTID;
                repariDDTO.Repair_Staff_Name = item.System_Users2.User_Name;
                repariDDTO.Fixture_Name = item.Fixture_M.Fixture_Name;
                repariDDTO.Fixture_Unique_ID = item.Fixture_M.Fixture_Unique_ID;
                repariDDTO.ShortCode = item.Fixture_M.ShortCode;
                if (item.Fixture_M.Production_Line != null)
                {
                    repariDDTO.Line_Name = item.Fixture_M.Production_Line.Line_Name;
                    repariDDTO.Workshop_Name = item.Fixture_M.Production_Line.Workshop.Workshop_Name;
                }
                repariDDTO.StatusName = item.Enumeration.Enum_Value;

                repariDDTO.Fixture_Repair_D_DefectDTOList = new List<Fixture_Repair_D_DefectDTO>();
                foreach (var subItem in item.Fixture_Repair_D_Defect)
                {
                    var repairDDefectDTO = AutoMapper.Mapper.Map<Fixture_Repair_D_DefectDTO>(subItem);
                    repairDDefectDTO.Defect_Code_Name = subItem.Fixture_DefectCode.DefectCode_Name;
                    repairDDefectDTO.Solution_Name = subItem.Fixture_RepairSolution.RepairSolution_Name;
                    repariDDTO.Fixture_Repair_D_DefectDTOList.Add(repairDDefectDTO);
                }
                fixtureRepairMDTO.Fixture_Repair_DDTOList.Add(repariDDTO);
            }
            return fixtureRepairMDTO;
        }

        public IList<FixtureDTO> GetFixtureListFixtureRepair(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name)
        {
            var dtoList = new List<FixtureDTO>();
            var query = fixtureRepository.GetAll();
            if (Plant_Organization_UID != 0)
            {
                query = query.Where(f => f.Plant_Organization_UID == Plant_Organization_UID);
            }
            if (BG_Organization_UID != 0)
            {
                query = query.Where(f => f.BG_Organization_UID == BG_Organization_UID);
            }
            if (FunPlant_Organization_UID != 0)
            {
                query = query.Where(f => f.FunPlant_Organization_UID == FunPlant_Organization_UID);
            }
            if (!string.IsNullOrWhiteSpace(Line_ID))
            {
                query = query.Where(f => f.Production_Line.Line_ID.Contains(Line_ID));
            }
            if (!string.IsNullOrWhiteSpace(Line_Name))
            {
                query = query.Where(f => f.Production_Line.Line_Name.Contains(Line_Name));
            }
            if (!string.IsNullOrWhiteSpace(Machine_ID))
            {
                query = query.Where(f => f.Fixture_Machine.Machine_ID.Contains(Machine_ID));
            }
            if (!string.IsNullOrWhiteSpace(Machine_Name))
            {
                query = query.Where(f => f.Fixture_Machine.Machine_Name.Contains(Machine_Name));
            }
            if (!string.IsNullOrWhiteSpace(Process_ID))
            {
                query = query.Where(f => f.Production_Line.Process_Info.Process_ID.Contains(Process_ID));
            }
            if (!string.IsNullOrWhiteSpace(Process_Name))
            {
                query = query.Where(f => f.Production_Line.Process_Info.Process_Name.Contains(Process_Name));
            }
            foreach (var item in query)
            {
                var dto = AutoMapper.Mapper.Map<FixtureDTO>(item);
                if (item.Production_Line != null)
                {
                    dto.Line_Name = item.Production_Line.Line_Name;
                    if (item.Production_Line.Workshop != null)
                    {
                        dto.Workshop = item.Production_Line.Workshop.Workshop_Name;
                    }
                }

                dtoList.Add(dto);
            }

            return dtoList;
        }

        public IList<FixtureDTO> GetFixtureListFixtureRepairByUniqueID(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string uniqueID)
        {
            var dtoList = new List<FixtureDTO>();
            var query = fixtureRepository.GetMany(x => x.Fixture_Unique_ID.Contains(uniqueID));
            if (Plant_Organization_UID != 0)
            {
                query = query.Where(f => f.Plant_Organization_UID == Plant_Organization_UID);
            }
            if (BG_Organization_UID != 0)
            {
                query = query.Where(f => f.BG_Organization_UID == BG_Organization_UID);
            }
            if (FunPlant_Organization_UID != 0)
            {
                query = query.Where(f => f.FunPlant_Organization_UID == FunPlant_Organization_UID);
            }
            foreach (var item in query)
            {
                var dto = AutoMapper.Mapper.Map<FixtureDTO>(item);
                if (item.Production_Line != null)
                {
                    dto.Line_Name = item.Production_Line.Line_Name;
                    if (item.Production_Line.Workshop != null)
                    {
                        dto.Workshop = item.Production_Line.Workshop.Workshop_Name;
                    }
                }

                dtoList.Add(dto);
            }

            return dtoList;
        }

        public List<DefectRepairSolutionDTO> GetDefectCodeReapairSolutionDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var defectRepairSolutions = defectRepairRepository.QueryDefectCodeReapairSolution(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);//.Distinct()

            var defectRepairSolutionDTOs = new List<DefectRepairSolutionDTO>();
            foreach (var item in defectRepairSolutions)
            {
                var defectRepairSolutionDTO = AutoMapper.Mapper.Map<DefectRepairSolutionDTO>(item);
                defectRepairSolutionDTO.Fixture_DefectName = item.Fixture_DefectCode.DefectCode_Name;
                defectRepairSolutionDTO.Repair_SoulutionName = item.Fixture_RepairSolution.RepairSolution_Name;
                defectRepairSolutionDTOs.Add(defectRepairSolutionDTO);
            }
            return defectRepairSolutionDTOs;
        }

        public FixtureDTO GetFixtureByFixtureUniqueID(string fixtureUniqueID)
        {
            var fixture = fixtureRepository.GetFirstOrDefault(f => f.Fixture_Unique_ID == fixtureUniqueID);
            if (fixture != null)
            {
                var fixtureDTO = AutoMapper.Mapper.Map<FixtureDTO>(fixture);
                return fixtureDTO;
            }
            return null;
        }
        public List<Fixture_Repair_MDTO> GetFixture_Repair_MDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixtureRepairRepository.GetFixture_Repair_MDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;
        }
        public Fixture_Repair_MDTO GetFixture_Repair_MDTOByID(int Fixture_Repair_M_UID)
        {
            var bud = fixtureRepairRepository.GetFixture_Repair_MDTOByID(Fixture_Repair_M_UID);
            return bud;
        }

        public string GetSentRepairNameById(string SentOut_Number)
        {
            try
            {
                var bud = fixtureRepairRepository.GetSentRepairNameById(SentOut_Number);
                return bud;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public FixtureRepairItem QueryFixtureRepairByNo(string Repair_NO)
        {
            var bud = fixtureRepairRepository.QueryFixtureRepairByNo(Repair_NO);
            return bud;
        }
        public List<RepairLocationDTO> GetRepairLocationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var resultList = repairLocationRepository.GetRepairLocationList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return resultList;

        }

        public IEnumerable<FixtureRepairDTO> DoExportFixtureRepair(string uids)
        {
            var totalCount = 0;
            var fixtureRepairs = fixtureRepairRepository.QueryFixtureRepairs(new FixtureRepairSearch { ExportUIds = uids }, null, out totalCount);

            var fixtureRepairDTOList = fixtureRepairs.ToList();
            return fixtureRepairDTOList;
        }
        #endregion

        #region 异常原因群组设定

        public PagedListModel<DefectCode_GroupDTO> QueryDefectCode_Group(DefectCode_GroupDTO searchModel, Page page)
        {
            int totalcount = 0;
            var result = defectCode_GroupRepository.QueryDefectCode_Group(searchModel, page, out totalcount);
            return new PagedListModel<DefectCode_GroupDTO>(totalcount, result);
        }

        public List<DefectCode_GroupDTO> DoAllExportDefectCode_GroupReprot(DefectCode_GroupDTO searchModel)
        {
            return defectCode_GroupRepository.DoAllExportDefectCode_GroupReprot(searchModel);


        }
        public List<DefectCode_GroupDTO> DoExportDefectCode_GroupReprot(string DefectCode_Group_UIDs)
        {
            return defectCode_GroupRepository.DoExportDefectCode_GroupReprot(DefectCode_Group_UIDs);

        }
        public string AddDefectCode_Group(DefectCode_GroupDTO dto)
        {
            try
            {
                if (dto.Fixture_Defect_UIDs.Count > 0)
                {
                    List<DefectCode_GroupDTO> defectCode_GroupDTOlists = defectCode_GroupRepository.DefectCode_GroupList(dto.Fixture_Defect_UIDs[0].Plant_Organization_UID);

                    foreach (var item in dto.Fixture_Defect_UIDs)
                    {
                        var defectCode_GroupDTOts = defectCode_GroupDTOlists.Where(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID && o.Fixtrue_Defect_UID == item.Fixture_Defect_UID && o.DefectCode_Group_ID == dto.DefectCode_Group_ID);
                        if (defectCode_GroupDTOts != null && defectCode_GroupDTOts.Count() >= 1)
                        {
                            return string.Format("数据重复，{1}治具群组下已经配置有该治具异常原因代码{0}", item.DefectCode_ID, dto.DefectCode_Group_ID);
                        }

                        DefectCode_Group defectCode_Group = new DefectCode_Group();
                        defectCode_Group.Plant_Organization_UID = dto.Plant_Organization_UID;
                        defectCode_Group.BG_Organization_UID = dto.BG_Organization_UID;
                        defectCode_Group.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        defectCode_Group.Fixtrue_Defect_UID = item.Fixture_Defect_UID;
                        defectCode_Group.DefectCode_Group_ID = dto.DefectCode_Group_ID;
                        defectCode_Group.DefectCode_Group_Name = dto.DefectCode_Group_Name;
                        defectCode_Group.Is_Enable = item.Is_Enable;
                        defectCode_Group.Created_UID = dto.Created_UID;
                        defectCode_Group.Modified_UID = dto.Modified_UID;
                        defectCode_Group.Created_Date = DateTime.Now;
                        defectCode_Group.Modified_Date = DateTime.Now;
                        defectCode_GroupRepository.Add(defectCode_Group);
                    }

                    unitOfWork.Commit();

                    return "保存成功！";
                }
                else
                {
                    return "没有异常原因数据";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }
        public List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var bud = defectCode_GroupRepository.DefectCode_GroupList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;
        }

        public List<DefectCode_GroupDTO> DefectCode_GroupList(int Plant_Organization_UID)
        {
            var bud = defectCode_GroupRepository.DefectCode_GroupList(Plant_Organization_UID);
            return bud;
        }

        public string InsertDefectCode_Group(List<DefectCode_GroupDTO> dtolist)
        {
            try
            {
                if (dtolist.Count > 0)
                {
                    List<DefectCode_GroupDTO> defectCode_GroupDTOlists = defectCode_GroupRepository.DefectCode_GroupList(dtolist[0].Plant_Organization_UID);

                    foreach (var item in dtolist)
                    {
                        var defectCode_GroupDTOts = defectCode_GroupDTOlists.Where(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID && o.Fixtrue_Defect_UID == item.Fixtrue_Defect_UID && o.DefectCode_Group_ID == item.DefectCode_Group_ID);

                        int funPlant_Organization_UID = 0;
                        if (item.FunPlant_Organization_UID != null)
                        {
                            funPlant_Organization_UID = item.FunPlant_Organization_UID.Value;
                            defectCode_GroupDTOts = defectCode_GroupDTOts.Where(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID && o.FunPlant_Organization_UID == funPlant_Organization_UID && o.Fixtrue_Defect_UID == item.Fixtrue_Defect_UID && o.DefectCode_Group_ID == item.DefectCode_Group_ID);
                        }
                        if (defectCode_GroupDTOts != null && defectCode_GroupDTOts.Count() > 1)
                        {
                            return string.Format("数据重复，{1}治具群组下已经配置有该治具异常原因代码{0}", item.DefectCode_ID, item.DefectCode_Group_ID);
                        }
                        DefectCode_Group defectCode_Group = new DefectCode_Group();
                        defectCode_Group.Plant_Organization_UID = item.Plant_Organization_UID;
                        defectCode_Group.BG_Organization_UID = item.BG_Organization_UID;
                        defectCode_Group.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                        defectCode_Group.Fixtrue_Defect_UID = item.Fixtrue_Defect_UID;
                        defectCode_Group.DefectCode_Group_ID = item.DefectCode_Group_ID;
                        defectCode_Group.DefectCode_Group_Name = item.DefectCode_Group_Name;
                        defectCode_Group.Is_Enable = item.Is_Enable.Value;
                        defectCode_Group.Created_UID = item.Created_UID;
                        defectCode_Group.Modified_UID = item.Modified_UID;
                        defectCode_Group.Created_Date = DateTime.Now;
                        defectCode_Group.Modified_Date = DateTime.Now;
                        defectCode_GroupRepository.Add(defectCode_Group);
                    }
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    return "没有异常群组数据";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string DeleteDefectCode_Group_UID(string DefectCode_Group_UIDs)
        {
            try
            {
                DefectCode_Group_UIDs = "," + DefectCode_Group_UIDs + ",";
                List<DefectCode_Group> defectCode_Groups = new List<DefectCode_Group>();
                defectCode_Groups = defectCode_GroupRepository.GetAll().Where(m => DefectCode_Group_UIDs.Contains("," + m.DefectCode_Group_UID + ",")).ToList();
                if (defectCode_Groups.Count > 0)
                {
                    defectCode_GroupRepository.DeleteList(defectCode_Groups);
                    unitOfWork.Commit();
                    return "删除成功！";
                }
                else
                {
                    return "没有异常群组数据！";
                }
            }
            catch (Exception ex)
            {
                return "所选数据有些在使用中。" + ex.Message;
            }
        }
        #endregion 异常原因群组设定


        #region 治具异常原因维护

        public string DeleteFixtureDefectCode_Setting_UID(string FixtureDefectCode_Setting_UIDs)
        {
            try
            {
                FixtureDefectCode_Setting_UIDs = "," + FixtureDefectCode_Setting_UIDs + ",";
                List<FixtureDefectCode_Setting> fixtureDefectCode_Setting = new List<FixtureDefectCode_Setting>();
                fixtureDefectCode_Setting = fixtureDefectCode_SettingRepository.GetAll().Where(m => FixtureDefectCode_Setting_UIDs.Contains("," + m.FixtureDefectCode_Setting_UID + ",")).ToList();

                if (fixtureDefectCode_Setting.Count > 0)
                {
                    fixtureDefectCode_SettingRepository.DeleteList(fixtureDefectCode_Setting);
                    unitOfWork.Commit();
                    return "删除成功！";
                }
                else
                {
                    return "没有异常群组数据！";
                }
            }
            catch (Exception ex)
            {
                return "所选数据有些在使用中。" + ex.Message;
            }
        }


        public PagedListModel<FixtureDefectCode_SettingDTO> QueryFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO searchModel, Page page)
        {
            int totalcount = 0;
            var result = fixtureDefectCode_SettingRepository.QueryDefectCode_Setting(searchModel, page, out totalcount);
            return new PagedListModel<FixtureDefectCode_SettingDTO>(totalcount, result);
        }

        public List<FixtureDefectCode_SettingDTO> DoAllExportFixtureDefectCode_SettingReprot(FixtureDefectCode_SettingDTO searchModel)
        {
            return fixtureDefectCode_SettingRepository.DoAllExportFixtureDefectCode_SettingReprot(searchModel);


        }

        public List<FixtureDefectCode_SettingDTO> DoExportFixtureDefectCode_SettingReprot(string FixtureDefectCode_Setting_UIDs)
        {
            return fixtureDefectCode_SettingRepository.DoExportFixtureDefectCode_SettingReprot(FixtureDefectCode_Setting_UIDs);

        }

        public string AddFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO dto)
        {
            try
            {
                if (dto.Fixture_Defect_UIDs.Count > 0)
                {
                    List<FixtureDefectCode_SettingDTO> fixtureDefectCode_Settinglists = fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingByPlant(dto.Fixture_Defect_UIDs[0].Plant_Organization_UID);

                    foreach (var item in dto.Fixture_Defect_UIDs)
                    {
                        int funPlant_Organization_UID = 0;
                        if (dto.FunPlant_Organization_UID != null)
                        {
                            funPlant_Organization_UID = dto.FunPlant_Organization_UID.Value;

                        }
                        var fixtureDefectCode_Settings = fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingList(dto.Plant_Organization_UID, dto.BG_Organization_UID, funPlant_Organization_UID, item.Fixture_Defect_UID, dto.Fixture_NO.Trim());

                        if (fixtureDefectCode_Settings != null && fixtureDefectCode_Settings.Count >= 1)
                        {
                            return string.Format("数据重复，{1}治具下已经配置有该治具异常原因代码{0}", dto.DefectCode_ID, dto.Fixture_NO);
                        }

                        FixtureDefectCode_Setting defectCode_Setting = new FixtureDefectCode_Setting();
                        defectCode_Setting.Plant_Organization_UID = dto.Plant_Organization_UID;
                        defectCode_Setting.BG_Organization_UID = dto.BG_Organization_UID;
                        defectCode_Setting.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        defectCode_Setting.Fixture_Defect_UID = item.Fixture_Defect_UID;
                        defectCode_Setting.Fixture_NO = dto.Fixture_NO.Trim();
                        defectCode_Setting.Is_Enable = item.Is_Enable;
                        defectCode_Setting.Created_UID = dto.Created_UID.Value;
                        defectCode_Setting.Modified_UID = dto.Modified_UID;
                        defectCode_Setting.Created_Date = DateTime.Now;
                        defectCode_Setting.Modified_Date = DateTime.Now;
                        fixtureDefectCode_SettingRepository.Add(defectCode_Setting);
                    }

                    unitOfWork.Commit();

                    return "保存成功！";
                }
                else
                {
                    return "没有异常原因数据！";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public List<Fixture_DefectCodeDTO> GetDefectCodeByGroup(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID)
        {

            var defectCode_Groups = defectCode_GroupRepository.DefectCode_GroupList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, DefectCode_Group_ID);
            List<int> defect_IDs = defectCode_Groups.Select(o => o.Fixtrue_Defect_UID).ToList();
            return fixture_DefectCodeRepository.GetDefectCodes(defect_IDs);

        }

        public DefectCode_GroupDTO DefectCode_Group(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID)
        {
            var defectCode_Groups = defectCode_GroupRepository.DefectCode_GroupList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, DefectCode_Group_ID);
            if (defectCode_Groups != null && defectCode_Groups.Count > 0)
            {
                return defectCode_Groups[0];
            }
            else
            {
                return null;
            }
        }
        public List<Fixture_DefectCodeDTO> GetDefectCodesByPlant(int Plant_Organization_UID)
        {

            return fixture_DefectCodeRepository.GetDefectCodesByPlant(Plant_Organization_UID);
        }

        public List<FixtureDefectCode_SettingDTO> GetFixtureDefectCode_SettingByPlant(int Plant_Organization_UID)
        {
            return fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingByPlant(Plant_Organization_UID);
        }
        public List<FixtureDTO> GetFixture_MByPlant(int Plant_Organization_UID)
        {
            return fixtureRepository.GetFixture_MByPlant(Plant_Organization_UID);

        }
        public string InsertFixtureDefectCode_Setting(List<FixtureDefectCode_SettingDTO> dtolist)
        {
            try
            {
                if (dtolist.Count > 0)
                {
                    List<FixtureDefectCode_SettingDTO> fixtureDefectCode_Settinglists = fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingByPlant(dtolist[0].Plant_Organization_UID);
                    foreach (var item in dtolist)
                    {
                        var fixtureDefectCode_Settings = fixtureDefectCode_Settinglists.Where(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID && o.Fixture_Defect_UID == item.Fixture_Defect_UID && o.Fixture_NO == item.Fixture_NO);

                        int funPlant_Organization_UID = 0;
                        if (item.FunPlant_Organization_UID != null)
                        {
                            funPlant_Organization_UID = item.FunPlant_Organization_UID.Value;
                            fixtureDefectCode_Settings = fixtureDefectCode_Settinglists.Where(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID && o.FunPlant_Organization_UID == funPlant_Organization_UID && o.Fixture_Defect_UID == item.Fixture_Defect_UID && o.Fixture_NO == item.Fixture_NO);
                        }
                        if (fixtureDefectCode_Settings != null && fixtureDefectCode_Settings.Count() > 1)
                        {
                            return string.Format("数据重复，{1}治具下已经配置有该治具异常原因代码{0}", item.DefectCode_ID, item.Fixture_NO);
                        }
                        FixtureDefectCode_Setting defectCode_Setting = new FixtureDefectCode_Setting();
                        defectCode_Setting.Plant_Organization_UID = item.Plant_Organization_UID;
                        defectCode_Setting.BG_Organization_UID = item.BG_Organization_UID;
                        defectCode_Setting.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                        defectCode_Setting.Fixture_Defect_UID = item.Fixture_Defect_UID;
                        defectCode_Setting.Fixture_NO = item.Fixture_NO;
                        defectCode_Setting.Is_Enable = item.Is_Enable.Value;
                        defectCode_Setting.Created_UID = item.Created_UID.Value;
                        defectCode_Setting.Modified_UID = item.Modified_UID;
                        defectCode_Setting.Created_Date = DateTime.Now;
                        defectCode_Setting.Modified_Date = DateTime.Now;
                        fixtureDefectCode_SettingRepository.Add(defectCode_Setting);
                    }

                    unitOfWork.Commit();

                    return "";
                }
                else
                {
                    return "没有异常原因数据";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<Production_LineDTO> GetProduction_LineByPlant(int Plant_Organization_UID)
        {
            return fixtureRepository.GetProductionLineDTOList(Plant_Organization_UID, 0, 0);
        }
        public string Insertfixture_M(List<FixtureDTO> dtolist)
        {

            try
            {
                if (dtolist.Count > 0)
                {
                    // int source_UID = fixtureRepository.GetFixtureCount(dtolist[0]);
                    foreach (var item in dtolist)
                    {
                        Fixture_M fixture_M = new Fixture_M();
                        fixture_M.Plant_Organization_UID = item.Plant_Organization_UID;
                        fixture_M.BG_Organization_UID = item.BG_Organization_UID;
                        fixture_M.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                        fixture_M.Fixture_NO = item.Fixture_NO;
                        fixture_M.Version = item.Version;
                        fixture_M.Fixture_Seq = item.Fixture_Seq;
                        fixture_M.Fixture_Unique_ID = item.Fixture_Unique_ID;
                        fixture_M.Fixture_Name = item.Fixture_Name;
                        fixture_M.Project_UID = item.Project_UID;
                        fixture_M.Fixture_Machine_UID = item.Fixture_Machine_UID;
                        fixture_M.Vendor_Info_UID = item.Vendor_Info_UID;
                        fixture_M.Production_Line_UID = item.Production_Line_UID;
                        fixture_M.Status = item.Status;
                        fixture_M.ShortCode = item.ShortCode;
                        fixture_M.TwoD_Barcode = item.TwoD_Barcode;
                        fixture_M.Created_UID = item.Created_UID;
                        fixture_M.Modified_UID = item.Modified_UID;
                        fixture_M.Created_Date = DateTime.Now;
                        fixture_M.Modified_Date = DateTime.Now;

                        Fixture_M newitem = fixtureRepository.Add(fixture_M);
                        unitOfWork.Commit();
                        //Fixture_M newitem = new Fixture_M();
                        //newitem = fixtureRepository.GetFixtureByUid(item);

                        if (newitem != null)
                        {
                            //source_UID++;
                            Fixture_Resume item_Resume = new Fixture_Resume();
                            item_Resume.Fixture_M_UID = newitem.Fixture_M_UID;
                            item_Resume.Data_Source = "1";
                            item_Resume.Resume_Date = newitem.Created_Date;
                            item_Resume.Resume_Notes = "新品入庫建檔";
                            item_Resume.Modified_UID = newitem.Created_UID;
                            item_Resume.Source_UID = newitem.Fixture_M_UID;
                            //表单流水号 需要获取当前是第几笔新增的治具资料；
                            int source_UID = fixtureRepository.GetFixtureCount(item);
                            // item_Resume.Source_UID = source_UID;
                            //后面还要加表单编号
                            item_Resume.Source_NO = "C" + newitem.Created_Date.Date.ToString("yyyyMMdd") + "_" + source_UID.ToString().PadLeft(3, '0');
                            item_Resume.Modified_Date = newitem.Created_Date;
                            fixture_ResumeRepository.Add(item_Resume);
                            //unitOfWork.Commit();
                        }

                    }
                    unitOfWork.Commit();
                    return "";

                }
                else
                {
                    return "没有异常原因数据！";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string Updatefixture_MAPI(List<FixtureDTO> dtolist)
        {

            return fixtureRepository.Updatefixture_MAPI(dtolist);

        }
        public DefectCode_GroupDTO GetDefectCode_GroupByUID(int DefectCode_Group_UID)
        {
            var bud = defectCode_GroupRepository.GetDefectCode_GroupByUID(DefectCode_Group_UID);
            return bud;

        }

        public FixtureDefectCode_SettingDTO GetFixtureDefectCode_SettingDTOByUID(int FixtureDefectCode_Setting_UID)
        {
            var bud = fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingDTOByUID(FixtureDefectCode_Setting_UID);
            return bud;

        }

        public string EditDefectCode_Group(DefectCode_GroupDTO dto)
        {
            try
            {
                int defectCode_UID = 0;
                List<Fixture_DefectCodeDTO> fixture_DefectCodeDTOs = GetDefectCodesByPlant(dto.Plant_Organization_UID);
                if (dto.BG_Organization_UID != 0)
                {
                    if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                    {
                        fixture_DefectCodeDTOs = fixture_DefectCodeDTOs.Where(o => o.BG_Organization_UID == dto.BG_Organization_UID).ToList();

                    }

                }
                //if (dto.FunPlant_Organization_UID != null && dto.FunPlant_Organization_UID != 0)
                //{
                //    if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                //    {
                //        fixture_DefectCodeDTOs = fixture_DefectCodeDTOs.Where(o => o.FunPlant_Organization_UID == dto.FunPlant_Organization_UID).ToList();
                //    }
                //}
                if (dto.DefectCode_ID != null)
                {
                    if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                    {
                        fixture_DefectCodeDTOs = fixture_DefectCodeDTOs.Where(o => o.DefectCode_ID == dto.DefectCode_ID).ToList();
                    }
                }
                if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                {
                    defectCode_UID = fixture_DefectCodeDTOs.FirstOrDefault().Fixture_Defect_UID;
                }
                if (defectCode_UID == 0)
                {
                    return string.Format("没有找到对应的治具原因编码{0}", dto.DefectCode_ID);

                }
                int funPlant_Organization_UID = 0;
                if (dto.FunPlant_Organization_UID != null)
                {
                    funPlant_Organization_UID = dto.FunPlant_Organization_UID.Value;

                }
                List<DefectCode_GroupDTO> defectCode_Groups = defectCode_GroupRepository.DefectCode_GroupList(dto.Plant_Organization_UID, dto.BG_Organization_UID, funPlant_Organization_UID, dto.DefectCode_Group_ID.Trim()).ToList();
                if (defectCode_Groups.Count() <= 0)
                {
                    return string.Format("没有找到对应的治具原因原因群组代码{0}", dto.DefectCode_Group_ID.Trim());
                }
                var newdefectCode_Groups = defectCode_Groups.Where(o => o.DefectCode_ID == dto.DefectCode_ID).ToList();
                if (newdefectCode_Groups != null && newdefectCode_Groups.Count > 1)
                {
                    return string.Format("{1}群组下已经有对应的治具异常原因代码{0}", dto.DefectCode_ID, dto.DefectCode_Group_ID);
                }
                else if (newdefectCode_Groups != null && newdefectCode_Groups.Count == 1)
                {
                    if (newdefectCode_Groups[0].DefectCode_Group_UID != dto.DefectCode_Group_UID)
                    {
                        return string.Format("{1}群组下已经有对应的治具异常原因代码{0}", dto.DefectCode_ID, dto.DefectCode_Group_ID);
                    }
                }
                DefectCode_Group defectCode_Group = defectCode_GroupRepository.GetById(dto.DefectCode_Group_UID);
                defectCode_Group.Plant_Organization_UID = dto.Plant_Organization_UID;
                defectCode_Group.BG_Organization_UID = dto.BG_Organization_UID;
                defectCode_Group.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                defectCode_Group.Fixtrue_Defect_UID = defectCode_UID;
                defectCode_Group.DefectCode_Group_ID = dto.DefectCode_Group_ID.Trim();
                defectCode_Group.DefectCode_Group_Name = dto.DefectCode_Group_Name.Trim();
                defectCode_Group.Modified_UID = dto.Modified_UID;
                defectCode_Group.Modified_Date = DateTime.Now;
                defectCode_Group.Is_Enable = dto.Is_Enable.Value;
                defectCode_GroupRepository.Update(defectCode_Group);
                unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception e)
            {
                return "保存失败，请检查你的数据！";
            }
        }
        public string EditFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO dto)
        {
            try
            {
                int defectCode_UID = 0;
                List<Fixture_DefectCodeDTO> fixture_DefectCodeDTOs = GetDefectCodesByPlant(dto.Plant_Organization_UID);
                if (dto.BG_Organization_UID != 0)
                {
                    if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                    {
                        fixture_DefectCodeDTOs = fixture_DefectCodeDTOs.Where(o => o.BG_Organization_UID == dto.BG_Organization_UID).ToList();

                    }

                }
                //if (dto.FunPlant_Organization_UID != null && dto.FunPlant_Organization_UID != 0)
                //{
                //    if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                //    {
                //        fixture_DefectCodeDTOs = fixture_DefectCodeDTOs.Where(o => o.FunPlant_Organization_UID == dto.FunPlant_Organization_UID).ToList();

                //    }
                //}
                if (dto.DefectCode_ID != null)
                {
                    if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                    {
                        fixture_DefectCodeDTOs = fixture_DefectCodeDTOs.Where(o => o.DefectCode_ID == dto.DefectCode_ID.Trim()).ToList();

                    }

                }
                if (fixture_DefectCodeDTOs != null && fixture_DefectCodeDTOs.Count > 0)
                {
                    defectCode_UID = fixture_DefectCodeDTOs.FirstOrDefault().Fixture_Defect_UID;

                }
                if (defectCode_UID == 0)
                {
                    return string.Format("没有找到对应的治具原因编码{0}", dto.DefectCode_ID);
                }

                int funPlant_Organization_UID = 0;
                if (dto.FunPlant_Organization_UID != null)
                {
                    funPlant_Organization_UID = dto.FunPlant_Organization_UID.Value;

                }
                List<FixtureDTO> fixtures = fixtureRepository.FixtureList(dto.Plant_Organization_UID, dto.BG_Organization_UID, funPlant_Organization_UID);
                int fixtureCount = fixtures.Where(o => o.Fixture_NO == dto.Fixture_NO).Count();
                if (fixtureCount <= 0)
                {
                    return string.Format("没有找到对应的治具编码{0}", dto.Fixture_NO);

                }

                var fixtureDefectCode_Settings = fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingList(dto.Plant_Organization_UID, dto.BG_Organization_UID, funPlant_Organization_UID, defectCode_UID, dto.Fixture_NO);

                if (fixtureDefectCode_Settings != null && fixtureDefectCode_Settings.Count > 1)
                {
                    return string.Format("数据重复，{1}治具下已经配置有该治具异常原因代码{0}", dto.DefectCode_ID, dto.Fixture_NO);
                }
                else if (fixtureDefectCode_Settings != null && fixtureDefectCode_Settings.Count == 1)
                {
                    if (fixtureDefectCode_Settings[0].FixtureDefectCode_Setting_UID != dto.FixtureDefectCode_Setting_UID)
                    {
                        return string.Format("数据重复，{1}治具下已经配置有该治具异常原因代码{0}", dto.DefectCode_ID, dto.Fixture_NO);
                    }
                }

                FixtureDefectCode_Setting fixtureDefectCode_Setting = fixtureDefectCode_SettingRepository.GetById(dto.FixtureDefectCode_Setting_UID);
                fixtureDefectCode_Setting.Plant_Organization_UID = dto.Plant_Organization_UID;
                fixtureDefectCode_Setting.BG_Organization_UID = dto.BG_Organization_UID;
                fixtureDefectCode_Setting.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                fixtureDefectCode_Setting.Fixture_NO = dto.Fixture_NO.Trim();
                fixtureDefectCode_Setting.Is_Enable = dto.Is_Enable.Value;
                fixtureDefectCode_Setting.Fixture_Defect_UID = defectCode_UID;
                fixtureDefectCode_Setting.Modified_Date = DateTime.Now;
                fixtureDefectCode_Setting.Modified_UID = dto.Modified_UID;
                fixtureDefectCode_Setting.Modified_Date = DateTime.Now;
                fixtureDefectCode_SettingRepository.Update(fixtureDefectCode_Setting);

                unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception e)
            {
                return "保存失败，请检查你的数据！";
            }
        }
        #endregion 治具异常原因维护

        #region 报表-治具维修次数查询 Add by Rock 2017-10-31---------------------------Start
        public PagedListModel<ReportByRepair> QueryReportByRepair(ReportByRepair model, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryReportByRepair(model, page, out totalcount);
            return new PagedListModel<ReportByRepair>(totalcount, result);
        }

        public List<ReportByRepair> ExportReportByRepair(ReportByRepair model)
        {
            var result = fixtureRepository.ExportReportByRepair(model);
            return result;
        }
        #endregion 报表-治具维修次数查询 Add by Rock 2017-10-31---------------------------End

        #region 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ Start
        public PagedListModel<ReportByRepair> QueryReportByRepairPerson(ReportByRepair model, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryReportByRepairPerson(model, page, out totalcount, false);
            return new PagedListModel<ReportByRepair>(totalcount, result);
        }

        public List<ReportByRepair> ExportReportByRepairPersonValid(ReportByRepair model)
        {
            var result = fixtureRepository.ExportReportByRepairPersonValid(model);
            return result;

        }


        #endregion 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ End

        #region 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ Start
        public PagedListModel<ReportByRepair> QueryReportByPage(ReportByRepair model, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryReportByPage(model, page, out totalcount, false);
            return new PagedListModel<ReportByRepair>(totalcount, result);
        }

        public List<ReportByRepair> ExportReportByPageValid(ReportByRepair model)
        {
            var result = fixtureRepository.ExportReportByPageValid(model);
            return result;

        }

        #endregion 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ End

        #region 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ Start
        public PagedListModel<ReportByRepair> QueryFixtureReportByDetail(ReportByRepair model, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryFixtureReportByDetail(model, page, out totalcount, false);
            return new PagedListModel<ReportByRepair>(totalcount, result);
        }

        public List<ReportByRepair> ExportReportByDetailValid(ReportByRepair model)
        {
            var result = fixtureRepository.ExportReportByDetailValid(model);
            return result;
        }


        #endregion 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ End

        #region 报表-治具维修次数查询（明细） Add by Rock 2017-11-28------------------------ Start
        public PagedListModel<ReportByRepair> QueryFixtureReportByAnalisis(ReportByRepair model, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryFixtureReportByAnalisis(model, page, out totalcount, false);
            return new PagedListModel<ReportByRepair>(totalcount, result);
        }

        public List<ReportByRepair> ExportReportByAnalisisValid(ReportByRepair model)
        {
            var result = fixtureRepository.ExportReportByAnalisisValid(model);
            return result;
        }


        #endregion 报表-治具维修次数查询（明细） Add by Rock 2017-11-28------------------------ End

        #region 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------Start
        public List<string> GetFixtureNoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureRepository.GetFixtureNoList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return result;
        }

        public PagedListModel<ReportByRepair> QueryFixtureReportByStatus(ReportByRepair model, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryFixtureReportByStatus(model, page, out totalcount, false);
            return new PagedListModel<ReportByRepair>(totalcount, result);
        }

        public List<ReportByRepair> ExportReportByStatusValid(ReportByRepair model)
        {
            var result = fixtureRepository.ExportReportByStatusValid(model);
            return result;
        }
        #endregion 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------End


        #region 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------Start
        public PagedListModel<ReportByStatusAnalisis> QueryFixtureReportByStatusAnalisis(ReportByRepair model, Page page)
        {
            int totalcount;
            var result = fixtureRepository.QueryFixtureReportByStatusAnalisis(model, page, out totalcount, false);
            return new PagedListModel<ReportByStatusAnalisis>(totalcount, result);
        }

        public List<ReportByStatusAnalisis> ExportReportByStatusAnalisisValid(ReportByRepair model)
        {
            var result = fixtureRepository.ExportReportByStatusAnalisisValid(model);
            return result;
        }
        #endregion 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------End

        #region 报表-FMT Dashboard Add by Rock 2017-12-11----------------Start
        public PagedListModel<Batch_ReportByStatus> QueryFixtureReportByFMT(Batch_ReportByStatus model, Page page)
        {
            int totalcount;
            DateTime StartDate = model.StartDate.Value.Date.AddDays(-1).AddHours(8);
            DateTime EndDate = model.StartDate.Value.Date.AddHours(8);

            var result = fixtureRepository.QueryFixtureReportByFMT(model, page, out totalcount, false, StartDate, EndDate);
            return new PagedListModel<Batch_ReportByStatus>(totalcount, result);
        }

        public List<Batch_ReportByStatus> QueryQueryFixtureReportByFMTDetail(int Process_Info_UID, string startDate, string endDate)
        {
            var list = fixtureRepository.QueryQueryFixtureReportByFMTDetail(Process_Info_UID, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), 0);
            return list;
        }

        public List<Batch_ReportByStatus> ExportReportByFMTValid(Batch_ReportByStatus model)
        {
            var result = fixtureRepository.ExportReportByFMTValid(model);
            return result;
        }

        #endregion 报表-FMT Dashboard Add by Rock 2017-12-11----------------End

        #region 主执行程序 -------------------Add By Rock 2017-12-18---------Start
        public void ExecFMTDashboard(string functionName, int Plant_Organization_UID, int System_Schedule_UID)
        {
            fixtureRepository.ExecFMTDashboard(functionName, Plant_Organization_UID, System_Schedule_UID);
        }


        #endregion 主执行程序 -------------------Add By Rock 2017-12-18---------End

        #region 治具归还
        public List<string> FetchFixtureTotakeforFixtureReturn(int plant_ID, int op_type, int funPlant)
        {
            var result = fixture_Return_MRepository.FetchFixtureTotakeforFixtureReturn(plant_ID, op_type, funPlant);
            return result;
        }
        public List<Fixture_Taken_InfoDTO> FetchAllFixturesBasedTakeNo(string Take_NO)
        {
            var result = fixture_Return_MRepository.FetchAllFixtureUIDBasedFixtureTakeNo(Take_NO);
            return result;
        }
        public List<Fixture_Taken_InfoDTO> FetchFixtureTakenInfo(List<int> FixtureTake_UIDs)
        {
            List<Fixture_Taken_InfoDTO> fixture_Taken_InfoDTOs = new List<Fixture_Taken_InfoDTO>();
            foreach(var each in FixtureTake_UIDs)
            {
                var result = fixture_Return_MRepository.FetchFixtureTakenInfo(each);
                foreach (var item in result)
                {
                    fixture_Taken_InfoDTOs.Add(new Fixture_Taken_InfoDTO
                    {
                        Fixture_Totake_M_UID = item.Fixture_Totake_M_UID,
                        Fixture_M_UID = item.Fixture_M_UID,
                        ShortCode = item.ShortCode,
                        Fixture_Unique_ID = item.Fixture_Unique_ID,
                        Process = item.Process,
                        WorkStation = item.WorkStation,
                        Line = item.Line,
                        IS_Return=item.IS_Return,
                        Fixture_Totake_D_UID=item.Fixture_Totake_D_UID

                    });
                }
            }
            return fixture_Taken_InfoDTOs;
        }

        public string AddFixtureRetrun(Fixture_Return_MDTO dto)
        {
            var result = fixture_Return_MRepository.AddFixtureRetrun(dto);

            return result;
        }

        public string AddFixtureRetrunD(Fixture_Return_DDTO dto)
        {
            var result = fixture_Return_MRepository.AddFixtureRetrunD(dto);

            return result;
        }

        public PagedListModel<Fixture_Return_Index> QueryFixtureToReturn(Fixture_Return_MDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = fixture_Return_MRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<Fixture_Return_Index>(totalcount, result);
        }

        public Fixture_Return_MDTO QueryFixtureReturnUid(int uid)
        {
            var result = fixture_Return_MRepository.QueryFixtureReturnUid(uid);
            return result;
        }

        public string FixtureReturnUpdatePost(Fixture_Return_MDTO dto)
        {
            var result = fixture_Return_MRepository.FixtureReturnUpdatePost(dto);
            return result.ToString();

        }

        public List<Fixture_Return_MDTO> FixtureReturnDetail(int uid)
        {
            var result = fixture_Return_MRepository.FixtureReturnDetail(uid);
            return result;
        }

        public string DelFixtureReturnM(int uid)
        {
            var result = fixture_Return_MRepository.DelFixtureReturnM(uid);
            return result;
        }

        public string GetCurrentReturnNub()
        {
            var result = fixture_Return_MRepository.GetCurrentReturnNub();
            return result;
        }

        public List<Fixture_Taken_InfoDTO> FetchAllFixturesBasedReturnMUID(int uid)
        {
            {
                var result = fixture_Return_MRepository.FetchAllFixturesBasedReturnMUID(uid);
                return result;
            }
        }

        public string UpdateFixtureRetrun(Fixture_Return_MDTO dto)
        {
            var result = fixture_Return_MRepository.UpdateFixtureRetrun(dto);

            return result;
        }

        public string UpdateFixtureRetrunD(Fixture_Return_DDTO dto)
        {
            var result = fixture_Return_MRepository.UpdateFixtureRetrunD(dto);

            return result;
        }

        public List<Fixture_Return_Index> ExportFixtrueReturn2Excel(Fixture_Return_MDTO dto)
        {
            var result = fixture_Return_MRepository.ExportFixtrueReturn2Excel(dto);

            return result;
        }
        #endregion
    }
}
