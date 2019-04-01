using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Model.EntityDTO;
using System.Data.Entity.SqlServer;

namespace PDMS.Service
{
    public interface IEquipmentmaintenanceService
    {   
        // Equipment_Info
        List<EquipmentReport> ExportEquipmentInfoNOTReprot(EquipmentReport searchModel);
        List<EquipmentReport> ExportEquipmentInfoReprot(EquipmentReport searchModel);
        PagedListModel<EquipmentReport> QueryEquipmentInfoReprot(EquipmentReport searchModel, Page page);
        PagedListModel<EquipmentReport> QueryEquipmentInfoNOTReprot(EquipmentReport searchModel, Page page);

        PagedListModel<EquipmentInfoDTO> QueryEquipmentInfo(EquipmentInfoSearchDTO searchModel, Page page);

        List<EquipmentInfoDTO> ExportALLEquipmentInfo(EquipmentInfoSearchDTO searchModel);

        List<EquipmentInfoDTO> ExportPartEquipmentInfo(string uids);

        List<System_Project> QueryProjects(int oporgid);
        List<System_Function_Plant> QueryFunplants(int oporgid, string optypes);

        List<SystemProjectDTO> QueryAllProjects(int optype);
        List<SystemProjectDTO> QueryOpType(int oporguid, int accuntid = 0);
        List<SystemProjectDTO> GetCurrentOPType(int parentOrg_UID, int organization_UID);
        List<System_Function_Plant> QueryAllPlant(string optype);
        List<EquipmentInfoDTO> QueryEquipmentSingle(string EQP_UID);
        Dictionary<int, string> AddManyEQPRepairs(Dictionary<int, EQPRepairInfoDTO> dtos);
        Dictionary<int, string> AddManyEQPRepairs2(Dictionary<int, EQPRepairInfoDTO> dics1, Dictionary<double, Dictionary<int, LaborUsingInfoDTO>> dics2, Dictionary<double, Dictionary<int, MeterialUpdateInfoDTO>> dics3);
        Dictionary<int, string> AddManyEQPRepairsClose(Dictionary<int, EQPRepairInfoDTO> dtos);
        string AddOrEditEquipmentInfo(EquipmentInfoDTO dto, string EQP_Uid, bool isEdit);
        string InsertEquipmentItem(List<EquipmentInfoDTO> dtolist);
        List<SystemProjectDTO> QueryAllProject();
        List<EquipmentInfoDTO> QueryAllEquipment(string location, int funplant, int optype);
        List<EquipmentInfoDTO> QueryAllEquipment();
        string DeleteEquipment(int EQP_Uid);
        string Getplanguid(string types, string plantname);
        List<SystemFunctionPlantDTO> QueryAllFunctionPlants();

        //EQP_UserTable
        PagedListModel<EQPUserTableDTO> QueryEQPUser(EQPUserTableDTO searchModel, Page page);
        List<EQPUserTableDTO> QueryEQPUserSingle(int EQPUser_Uid);
        string DeleteEQPUser(int EQPUser_Uid);
        List<EQPUserTableDTO> QueryAllEQPUser();
        string InsertEQPUserItem(List<EQPUserTableDTO> dtolist);
        string AddOrEditEQPUserInfo(EQPUserTableDTO dto, bool isEdit);
        List<EQP_UserTable> QueryEQPUserSingle2(string userid);

        //Safe_Stock
        List<SafeStockDTO> QuerySafeStock();
        string UpdateSafeStock(List<SafeStockDTO> safestock);

        //Material_info
        PagedListModel<MaterialInfoDTO> QueryMaterialr(MaterialInfoDTO searchModel, Page page);
        List<MaterialInfoDTO> DoExportMaterialReprot(string uids);
        List<MaterialInfoDTO> DoAllExportMaterialReprot(MaterialInfoDTO searchModel);
        List<MaterialInfoDTO> QueryMaterialSingle(int Material_Uid);
        List<MaterialInfoDTO> QueryAllMaterialInfo();
        List<MaterialInfoDTO> QueryAllImportMaterialInfo();
        List<MaterialInfoDTO> QueryAllOPMaterialInfo(int PlantID);
        List<MaterialInfoDTO> QueryOPMaterialInfoByKey(int PlantID, string key);
        string DeleteMaterial(int Material_Uid);
        string InsertMaterialItem(List<MaterialInfoDTO> dtolist);
        string AddOrEditMaterialInfo(MaterialInfoDTO dto, bool isEdit);

        //Error Info
        PagedListModel<ErrorInfoDTO> QueryErrorInfo(ErrorInfoDTO errorInfoDTO, Page page);
        List<ErrorInfoDTO> QueryErrorInfoSingle(int Enum_UID);
        string DeleteErrorInfo(int Enum_UID);
        string AddErrorInfoItem(ErrorInfoDTO dto, bool isEdit);

        //EQPMaintenance 
        List<EnumerationDTO> GetAllErrorTypes();
        List<EQPUserTableDTO> GetAllUser(int bgUID, int funplantUID, int plantUID);
        List<string> GetAllEQPLocation(int Plant_Organization_UID);
        List<MaterialInfoDTO> GetAllMaterial(int planId = 0);
        List<string> GetAllEBoardLocation(string optype);

        List<string> GetUnitMaterial();
        string AddOrEditEQPRepair(EQPRepairInfoDTO dto, int eqp_uid, bool isEdit);
        string AddOrEditEQPRepair2(EQPRepairInfoDTO dto, bool MH_Flag);
        PagedListModel<EQPRepairInfoDTO> QueryRepairInfo(EQPRepairInfoSearchDTO searchModel, Page page);
        EQPRepairInfoDTO QueryEQPRepairSingle(int Repair_Uid);
        string DeleteEQPRepairInfo(int Repair_Uid);
        List<MeterialUpdateInfoDTO> QueryMaterialUpdateSingle(int Material_Uid);

        string UpdateLabor(List<LaborUsingInfoDTO> labor, int userid, int Repair_Uid, bool MH_Flag);
        string UpdateMat(List<MeterialUpdateInfoDTO> mat, int userid, DateTime updatetime, int EQP_Uid, int Repair_Uid, bool MH_Flag);
        List<laborlist> GetLaborList(int Repair_Uid);
        List<matlist> GetMatList(int Repair_Uid);

        List<string> QueryUserRole(string userid);

        List<string> GetOptype();
        List<System_Organization_PlantDTO> GetOrganization_Plants();
        List<SystemProjectDTO> GetOptypeByUser(int optype);
        List<EquipmentInfoDTO> GetProjectnameByOptype(string Optype);
        List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes);
        List<EquipmentInfoDTO> GetProcessByFunplant(string funplantuid);
        List<string> GetEqpidByProcess(string funplantuid);
        List<EQPRepairInfoDTO> DoExportFunction(string uids);

        List<EQPRepairInfoDTO> DoAllEQPMaintenanceReprot(EQPRepairInfoSearchDTO search);
        List<SystemFunctionPlantDTO> GetUserFunPlantByUser(int userid, int optypeuid);
        List<string> GetUserOPTpye(int userid);
        //EQPMaintenanceReport
        PagedListModel<EQPRepairInfoDTO> QueryRepairReportInfo(EQPRepairInfoSearchDTO searchModel, Page page);

        List<EQPRepairInfoDTO> DoExportFunction2(string optypes, string projectname, string funplant, string process,
                        string eqpid, string reason, string repairid, string location, string classdesc, string contact,
                        DateTime fromdate, DateTime todate, string errorlever, string repairresult, string labor,
                        string updatepart, string remark, string status);

        List<EQPRepairInfoDTO> DoPartExportFunctionInfo(string uids);
        string CloseEQPRepairInfo(int Enum_UID);

        //取得成本中心信息
        List<CostCtrDTO> GetCostCtrs();

        //CostCenterMaintenace 
        #region define CostCenterMaintenace interface Add By Darren 2018/12/18 
        PagedListModel<CostCenterItem> QueryCostCenters(CostCenterModelSearch searchModel, Page page);
        string DeleteCostCenter(int uid);
        string AddCostCenter(CostCtr_infoDTO vm);
        CostCtr_infoDTO QueryCostCenter(int uid);
        string ModifyCostCenter(CostCtr_infoDTO vm);
        #endregion //end CostCenterMaintenace interface

        //电子看板
        PagedListModel<EQPRepairInfoDTO> getShowContent(EQPRepairInfoDTO search, Page page);

        //重要料号原因分析
        PagedListModel<MaterialReasonDTO> QueryMatReason(MaterialReasonDTO searchModel, Page page);
        string AddOrEditMaterialReason(MaterialReasonDTO dto);
        List<MaterialReasonDTO> QuerymatReasonByuid(int Material_Reason_UID);
        string DeleteMatReason(int Material_Reason_UID, int userid);
        List<MaterialReasonDTO> GetMatReasonByMat(int Material_Uid);
        string InsertMatReasonItem(List<MaterialReasonDTO> dtolist);
        List<string> GetFunPlants(string selectProjects, string optype);
        List<MaterialReasonDTO> DoExportMatReasonReprot(string Material_Reason_UIDs);

        List<MaterialReasonDTO> DoAllExportMatReasonReprot(MaterialReasonDTO search);
        PagedListModel<Material_Maintenance_RecordDTO> QueryMaterial_Maintenance_Records(Material_Maintenance_RecordDTO searchModel, Page page);

        string AddOrEditMaterial_Maintenance_Record(Material_Maintenance_RecordDTO dto);

        Material_Maintenance_RecordDTO QueryMaterial_Maintenance_RecordByUid(int Material_Maintenance_Record_UID);
        string Approve1Material_Maintenance_RecordByUid(int Material_Maintenance_Record_UID, int Useruid);
        string Approve2Material_Maintenance_Record(Material_Maintenance_RecordDTO dto);
        string Approve3Material_Maintenance_RecordByUid(int Material_Maintenance_Record_UID, int Useruid);
        string DeleteMaterial_Maintenance_Record(int Material_Maintenance_Record_UID);

        EqumentOrgInfo GetOEE_MachineInfoByEMT(int PlantUID, string EMT);
        List<Material_Maintenance_RecordDTO> DoAllExportMaterial_Maintenance_Record(Material_Maintenance_RecordDTO search);
        List<Material_Maintenance_RecordDTO> DoExportMaterial_Maintenance_Record(string Material_Maintenance_Record_UIDs);
    }

    public class EquipmentmaintenanceService : IEquipmentmaintenanceService
    {
        private readonly IEquipmentInfoRepository equipmentInfoRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly IEQPUserTableRepository eqpuserTableRepository;
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IMaterialInfoRepository materialInfoRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;
        private readonly IEQPRepairInfoRepository eQPRepairInfoRepository;
        private readonly ICostCtrInfoRepository costCtrInfoRepository;
        private readonly IMeterialUpdateInfoRepository meterialUpdateInfoRepository;
        private readonly ILaborUsingInfoRepository laborUsingInfoRepository;
        private readonly IMaterialReasonRepository materialReasonRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IMaterial_Maintenance_RecordRepository material_Maintenance_RecordRepository;
        public EquipmentmaintenanceService(IEquipmentInfoRepository equipmentInfoRepository
            , ISystemProjectRepository systemProjectRepository
            , IEQPUserTableRepository eqpuserTableRepository
            , IEnumerationRepository enumerationRepository
            , IMaterialInfoRepository materialInfoRepository
            , IUnitOfWork unitOfWork, ISystemFunctionPlantRepository systemFunctionPlantRepository
            , IEQPRepairInfoRepository eQPRepairInfoRepository
            , ICostCtrInfoRepository costCtrInfoRepository
            , IMeterialUpdateInfoRepository meterialUpdateInfoRepository
            , ILaborUsingInfoRepository laborUsingInfoRepository
            , IMaterialReasonRepository materialReasonRepository,
            ISystemUserRepository systemUserRepository,
            IMaterial_Maintenance_RecordRepository material_Maintenance_RecordRepository)
        {
            this.equipmentInfoRepository = equipmentInfoRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.eqpuserTableRepository = eqpuserTableRepository;
            this.enumerationRepository = enumerationRepository;
            this.materialInfoRepository = materialInfoRepository;
            this.unitOfWork = unitOfWork;
            this.systemFunctionPlantRepository = systemFunctionPlantRepository;
            this.eQPRepairInfoRepository = eQPRepairInfoRepository;
            this.costCtrInfoRepository = costCtrInfoRepository;
            this.meterialUpdateInfoRepository = meterialUpdateInfoRepository;
            this.laborUsingInfoRepository = laborUsingInfoRepository;
            this.materialReasonRepository = materialReasonRepository;
            this.systemUserRepository = systemUserRepository;
            this.material_Maintenance_RecordRepository = material_Maintenance_RecordRepository;
        }

        #region safe_stock
        public List<SafeStockDTO> QuerySafeStock()
        {
            List<SafeStockDTO> result = new List<SafeStockDTO>();
            SafeStockDTO safestock = new SafeStockDTO();
            var allnum = enumerationRepository.GetAll();
            var safestocknum = allnum.FirstOrDefault(m => m.Enum_Name == "Safe_Stock_Max");
            if (safestocknum != null)
                safestock.Safe_Stock_Max = safestocknum.Enum_Value;
            else
                safestock.Safe_Stock_Max = "";
            safestocknum = allnum.FirstOrDefault(m => m.Enum_Name == "Safe_Stock_LastOpenEQP");
            if (safestocknum != null)
                safestock.Safe_Stock_LastOpenEQP = safestocknum.Enum_Value;
            else
                safestock.Safe_Stock_LastOpenEQP = "";
            safestocknum = allnum.FirstOrDefault(m => m.Enum_Name == "Safe_Stock_PlanOpenEQP");
            if (safestocknum != null)
                safestock.Safe_Stock_PlanOpenEQP = safestocknum.Enum_Value;
            else
                safestock.Safe_Stock_PlanOpenEQP = "";
            result.Add(safestock);
            return result;
        }
        public string UpdateSafeStock(List<SafeStockDTO> safestock)
        {
            return enumerationRepository.UpdateItem(safestock[0].Safe_Stock_Max, safestock[0].Safe_Stock_LastOpenEQP, safestock[0].Safe_Stock_PlanOpenEQP);
        }
        #endregion
        #region Equipment_Info
        public string DeleteEquipment(int EQP_Uid)
        {
            return equipmentInfoRepository.DeleteEquipment(EQP_Uid);
        }

        public EqumentOrgInfo GetOEE_MachineInfoByEMT(int PlantUID, string EMT)
        {
            var result = equipmentInfoRepository.GetOEE_MachineInfoByEMT(PlantUID, EMT);
            return result;
        }
        public string Getplanguid(string types, string plantname)
        {
            var item = systemFunctionPlantRepository.GetMany(m => m.FunPlant == plantname && m.OP_Types == types).ToList();
            if (item.Count() > 0)
                item[0].System_FunPlant_UID.ToString();
            return "";
        }
        public List<string> GetFunPlants(string selectProjects, string opType)
        {
            return equipmentInfoRepository.getFunplants(selectProjects, opType);
        }

        public List<SystemFunctionPlantDTO> QueryAllFunctionPlants()
        {
            var functionPlants = systemFunctionPlantRepository.GetAll();
            List<SystemFunctionPlantDTO> dtoList = new List<SystemFunctionPlantDTO>();
            foreach (var item in functionPlants)
            {
                dtoList.Add(AutoMapper.Mapper.Map<SystemFunctionPlantDTO>(item));
            }
            return dtoList;
        }

        public List<SystemProjectDTO> QueryAllProject()
        {
            var project = systemProjectRepository.GetAll();
            List<SystemProjectDTO> dtoList = new List<SystemProjectDTO>();
            foreach (var item in project)
            {
                dtoList.Add(AutoMapper.Mapper.Map<SystemProjectDTO>(item));
            }
            return dtoList;
        }

        public List<EQPUserTableDTO> GetAllUser(int bgUID, int funplantUID, int plantUID)
        {
            List<EQPUserTableDTO> result = new List<EQPUserTableDTO>();
            var userlist = eqpuserTableRepository.GetAll().ToList();

            if (plantUID != 0)
            {
                userlist = userlist.Where(O => O.Plant_OrganizationUID == plantUID).ToList();
            }
            if (bgUID != 0)
            {
                userlist = userlist.Where(O => O.BG_Organization_UID == bgUID).ToList();

            }
            if (funplantUID != 0)
            {
                userlist = userlist.Where(O => O.FunPlant_Organization_UID == funplantUID).ToList();
            }

            userlist = userlist.Where(O => O.IsDisable == 1).ToList();

            foreach (var item in userlist)
            {
                result.Add(AutoMapper.Mapper.Map<EQPUserTableDTO>(item));
            }
            return result;
        }

        public List<string> GetAllEQPLocation(int Plant_Organization_UID)
        {
            List<string> result = new List<string>();
            var eqplocation = equipmentInfoRepository.GetDistinctLocation( Plant_Organization_UID);
            foreach (var item in eqplocation)
            {
                result.Add(item);
            }
            return result;
        }

        public List<string> GetAllEBoardLocation(string optype)
        {
            List<string> result = new List<string>();
            var eqplocation = enumerationRepository.GetMany(m => m.Enum_Type == "EBoard EQPLocation" & m.Enum_Name == optype);
            foreach (var item in eqplocation)
            {
                result.Add(item.Enum_Value);
            }
            return result;
        }

        public List<SystemFunctionPlantDTO> GetUserFunPlantByUser(int userid, int optypeuid)
        {
            var funplant = equipmentInfoRepository.GetFunplantByUser(userid, optypeuid);
            return funplant;
        }

        public List<string> GetUserOPTpye(int userid)
        {
            var funplant = equipmentInfoRepository.GetOPTypeByUser(userid);
            return funplant;
        }

        public List<EquipmentInfoDTO> QueryAllEquipment(string location, int funplant, int optype)
        {

            var equipment = equipmentInfoRepository.GetInfoByUserid(location, funplant, optype);
            return equipment;
        }
        public List<EquipmentInfoDTO> QueryAllEquipment()
        {
            var equipment = equipmentInfoRepository.GetAll();
            List<EquipmentInfoDTO> dtoList = new List<EquipmentInfoDTO>();
            foreach (var item in equipment)
            {
                dtoList.Add(AutoMapper.Mapper.Map<EquipmentInfoDTO>(item));
            }
            return dtoList;
        }

        public PagedListModel<EquipmentInfoDTO> QueryEquipmentInfo(EquipmentInfoSearchDTO searchModel, Page page)
        {
            var totalcount = 0;
            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.Organization_UID == null ? 0 : searchModel.Organization_UID.Value);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = equipmentInfoRepository.GetInfo(searchModel, organization_UIDs, page, out totalcount);
            return new PagedListModel<EquipmentInfoDTO>(totalcount, result);
        }

        public List<EquipmentInfoDTO> ExportALLEquipmentInfo(EquipmentInfoSearchDTO searchModel)
        {
            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.Organization_UID == null ? 0 : searchModel.Organization_UID.Value);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = equipmentInfoRepository.ExportALLEquipmentInfo(searchModel, organization_UIDs);
            return result;
        }

        public List<EquipmentInfoDTO> ExportPartEquipmentInfo(string uids)
        {
            var result = equipmentInfoRepository.ExportPartEquipmentInfo(uids);
            return result;
        }


        public List<System_Project> QueryProjects(int oporgid)
        {
            var bud = systemProjectRepository.GetProjects(oporgid).ToList();
            return bud;
        }

        public List<System_Function_Plant> QueryFunplants(int oporgid, string optypes)
        {
            var bud = systemProjectRepository.GetFunplants(oporgid, optypes).ToList();
            return bud;
        }

        public List<SystemProjectDTO> QueryAllProjects(int optype)
        {
            var bud = systemProjectRepository.GetAllProjects(optype).ToList();
            return bud;
        }

        public List<SystemProjectDTO> QueryOpType(int oporguid, int accuntid = 0)
        {
            var bud = systemProjectRepository.GetOpType(oporguid, accuntid).ToList();
            return bud;
        }

        public List<SystemProjectDTO> GetCurrentOPType(int parentOrg_UID, int organization_UID)
        {
            var bud = systemProjectRepository.GetCurrentOPType(parentOrg_UID, organization_UID).ToList();
            return bud;
        }

        public List<System_Function_Plant> QueryAllPlant(string optype)
        {
            var bud = systemFunctionPlantRepository.GetAllPlant(optype).ToList();
            return bud;
        }

        public List<EquipmentInfoDTO> QueryEquipmentSingle(string EQP_UID)
        {
            var bud = equipmentInfoRepository.GetByUId(EQP_UID);
            return bud;
        }

        public string AddOrEditEquipmentInfo(EquipmentInfoDTO dto, string EQP_Uid, bool isEdit)
        {
            try
            {
                if (isEdit)
                {
                    var hasequipment = equipmentInfoRepository.GetMany(m => m.Equipment == dto.Equipment & m.EQP_Uid != dto.EQP_Uid).ToList();
                    if (hasequipment.Count > 0)
                        return "EMT号" + dto.Equipment + "已存在,不可重复添加";
                    var hasserial = equipmentInfoRepository.GetMany(m => m.Mfg_Part_Number == dto.Mfg_Part_Number && m.EQP_Uid != dto.EQP_Uid).ToList();
                    if (hasserial.Count > 0)
                    {
                        return "机台号" + dto.Mfg_Part_Number + "已存在,不可重复添加";
                    }
                    //2017//11/28 
                    //var hasprono = equipmentInfoRepository.GetMany(m => m.Asset == dto.Asset & m.EQP_Uid != dto.EQP_Uid).ToList();
                    //if (hasprono.Count > 0)
                    //{
                    //    return "财编" + dto.Asset + "已存在,不可重复添加";
                    //}

                    equipmentInfoRepository.UpdateItem(dto);
                }
                else
                {
                    var hasequipment = equipmentInfoRepository.GetMany(m => m.Equipment == dto.Equipment).ToList();
                    if (hasequipment.Count > 0)
                        return "EMT号" + dto.Equipment + "已存在,不可重复添加";
                    var hasserial = equipmentInfoRepository.GetMany(m => m.Mfg_Part_Number == dto.Mfg_Part_Number).ToList();
                    if (hasserial.Count > 0)
                    {
                        return "机台号" + dto.Mfg_Part_Number + "已存在,不可重复添加";
                    }
                    //var hasprono = equipmentInfoRepository.GetMany(m => m.Asset == dto.Asset).ToList();
                    //if (hasprono.Count > 0)
                    //{
                    //    return "财编" + dto.Asset + "已存在,不可重复添加";
                    //}
                    List<EquipmentInfoDTO> eqpmentlist = new List<EquipmentInfoDTO>();
                    eqpmentlist.Add(dto);

                    equipmentInfoRepository.InsertItem(eqpmentlist);
                }
                return "";
            }
            catch (Exception e)
            {
                return "更新设备信息失败:" + e.Message;
            }
        }


        public string InsertEquipmentItem(List<EquipmentInfoDTO> dtolist)
        {
            return equipmentInfoRepository.InsertItem(dtolist);
        }
        #endregion
        #region EQP_UserTable
        public PagedListModel<EQPUserTableDTO> QueryEQPUser(EQPUserTableDTO searchModel, Page page)
        {
            var totalcount = 0;
            var result = eqpuserTableRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<EQPUserTableDTO>(totalcount, result);
        }

        public List<EQPUserTableDTO> QueryEQPUserSingle(int EQPUser_Uid)
        {
            var bud = eqpuserTableRepository.GetByUId(EQPUser_Uid);
            return bud;
        }

        public List<EQP_UserTable> QueryEQPUserSingle2(string userid)
        {
            var bud = eqpuserTableRepository.GetByUserId(userid);
            return bud;
        }

        public string DeleteEQPUser(int EQPUser_Uid)
        {
            return eqpuserTableRepository.DeleteEQPUser(EQPUser_Uid);
        }

        public List<EQPUserTableDTO> QueryAllEQPUser()
        {
            var project = eqpuserTableRepository.GetAll().ToList();
            //var list = AutoMapper.Mapper.Map<List<EQPUserTableDTO>>(project);
            //return list;
            List<EQPUserTableDTO> dtoList = new List<EQPUserTableDTO>();
            foreach (var item in project)
            {
                dtoList.Add(AutoMapper.Mapper.Map<EQPUserTableDTO>(item));
            }
            return dtoList;
        }

        public string InsertEQPUserItem(List<EQPUserTableDTO> dtolist)
        {
            return eqpuserTableRepository.InsertItem(dtolist);
        }

        public string AddOrEditEQPUserInfo(EQPUserTableDTO dto, bool isEdit)
        {
            try
            {
                EQP_UserTable eqpuser = new EQP_UserTable();
                eqpuser.BG_Organization_UID = dto.Organization_UID;
                eqpuser.FunPlant_Organization_UID = dto.FunPlant_OrganizationUID;
                eqpuser.EQPUser_Uid = dto.EQPUser_Uid;
                eqpuser.User_Id = dto.User_Id;
                eqpuser.User_Name = dto.User_Name;
                eqpuser.User_IdAndName = dto.User_Id + "_" + dto.User_Name;
                eqpuser.User_Email = dto.User_Email;
                eqpuser.User_Call = dto.User_Call;
                eqpuser.Modified_UID = dto.Modified_UID;
                eqpuser.Modified_Date = DateTime.Now;
                eqpuser.Plant_OrganizationUID = dto.Plant_OrganizationUID;
                eqpuser.IsDisable = dto.IsDisable;

                if (isEdit)
                {
                    eqpuserTableRepository.Update(eqpuser);
                }
                else
                {
                    eqpuserTableRepository.Add(eqpuser);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "更新人员账号失败:" + e.Message;
            }
        }
        #endregion  EQP_UserTable

        #region material_info
        public PagedListModel<MaterialInfoDTO> QueryMaterialr(MaterialInfoDTO searchModel, Page page)
        {
            int totalcount;
            var result = materialInfoRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<MaterialInfoDTO>(totalcount, result);
        }
        public List<MaterialInfoDTO> DoExportMaterialReprot(string uids)
        {
            return materialInfoRepository.DoExportMaterialReprot(uids);
        }
        public List<MaterialInfoDTO> DoAllExportMaterialReprot(MaterialInfoDTO searchModel)
        {
            return materialInfoRepository.DoAllExportMaterialReprot(searchModel);
        }
        public List<MaterialInfoDTO> QueryMaterialSingle(int Material_Uid)
        {
            var bud = materialInfoRepository.GetByUId(Material_Uid);
            return bud;
        }

        public List<MaterialInfoDTO> QueryAllMaterialInfo()
        {
            var project = materialInfoRepository.GetAll().Where(o=>o.Is_Enable==true).ToList();
            List<MaterialInfoDTO> dtoList = new List<MaterialInfoDTO>();
            foreach (var item in project)
            {
                dtoList.Add(AutoMapper.Mapper.Map<MaterialInfoDTO>(item));
            }
            return dtoList;
        }

        public List<MaterialInfoDTO> QueryAllImportMaterialInfo()
        {
            var project = materialInfoRepository.GetAll().ToList();
            List<MaterialInfoDTO> dtoList = new List<MaterialInfoDTO>();
            foreach (var item in project)
            {
                dtoList.Add(AutoMapper.Mapper.Map<MaterialInfoDTO>(item));
            }
            return dtoList;
        }
        public List<MaterialInfoDTO> QueryAllOPMaterialInfo(int PlantID)
        {

            var project = materialInfoRepository.GetAll().Where(o => o.Is_Enable == true&&o.Organization_UID== PlantID);
            if(PlantID==0)
            {
                project = materialInfoRepository.GetAll().Where(o => o.Is_Enable == true);
            }
            List<MaterialInfoDTO> dtoList = new List<MaterialInfoDTO>();
            foreach (var item in project)
            {
                dtoList.Add(AutoMapper.Mapper.Map<MaterialInfoDTO>(item));
            }
            return dtoList;
        }
        public List<MaterialInfoDTO> QueryOPMaterialInfoByKey(int PlantID,string key)
        {

            var project = materialInfoRepository.GetAll().Where(o => o.Is_Enable == true && o.Organization_UID == PlantID);
            if (PlantID == 0)
            {
                project = materialInfoRepository.GetAll().Where(o => o.Is_Enable == true);
            }
            if (key.Length > 0) {
                project = project.Where(o => o.Material_Id.Contains(key) || o.Material_Name.Contains(key) || o.Material_Types.Contains(key));
            }
            List<MaterialInfoDTO> dtoList = new List<MaterialInfoDTO>();
            foreach (var item in project)
            {
                dtoList.Add(AutoMapper.Mapper.Map<MaterialInfoDTO>(item));
            }
            return dtoList;
        }
        public string AddOrEditMaterialInfo(MaterialInfoDTO dto, bool isEdit)
        {
            try
            {
                Material_Info iteammaterial = new Material_Info();
                iteammaterial.Material_Uid = dto.Material_Uid;
                //iteammaterial.Material_Id = dto.Material_Id;
                iteammaterial.Material_Name = dto.Material_Name;
                iteammaterial.Material_Types = dto.Material_Types;
                iteammaterial.Classification = dto.Classification;
                iteammaterial.Unit_Price = dto.Unit_Price;
                iteammaterial.Delivery_Date = dto.Delivery_Date;
                iteammaterial.Modified_UID = dto.Modified_UID;
                iteammaterial.Modified_Date = DateTime.Now;
                iteammaterial.Cost_Center = dto.Cost_Center;
                iteammaterial.IsRework = dto.IsRework;
                iteammaterial.Is_Enable = dto.Is_Enable;
                iteammaterial.Maintenance_Cycle = dto.Maintenance_Cycle;
                iteammaterial.Material_Life = dto.Material_Life;
                iteammaterial.Monthly_Consumption = dto.Monthly_Consumption;
                iteammaterial.Requisitions_Cycle = dto.Requisitions_Cycle;
                iteammaterial.Sign_days = dto.Sign_days;
                iteammaterial.Warehouse_Storage_UID = dto.Warehouse_Storage_UID;
                iteammaterial.Daily_Consumption = dto.Daily_Consumption;
                iteammaterial.Organization_UID = dto.Plant_OrganizationUID;

                if (isEdit)
                {
                    iteammaterial.Material_Id = dto.Material_Id;
                    iteammaterial.Assembly_Name = dto.Material_Id + "_" + dto.Material_Name + "_" + dto.Material_Types;
                    var hasitem1 = materialInfoRepository.GetFirstOrDefault(m => m.Material_Id == dto.Material_Id & m.Material_Uid != dto.Material_Uid && m.Organization_UID == dto.Plant_OrganizationUID); //2019-03-05号修改，Frank要求修改为“厂区+料号”唯一。
                    if (hasitem1 != null)
                        return "料号" + hasitem1.Material_Id + "重复,请修改";
                    //var hasitem = materialInfoRepository.GetFirstOrDefault(m => m.Material_Types == dto.Material_Types & m.Material_Uid != dto.Material_Uid&&m.Organization_UID==dto.Plant_OrganizationUID); //2019-03-01号修改，Frank要求修改为“厂区+型号”唯一。
                    //if (hasitem != null)
                    //    return "型号" + hasitem.Material_Types + "重复,请修改";
                    materialInfoRepository.Update(iteammaterial);
                }
                else
                {
                    var liaohaoBegin = dto.Material_Id.Substring(0, 2);

                    if (liaohaoBegin.Equals("CP"))
                    {
                        #region 以CP开始的料号自动生成编码
                        var MaxMaterial_Id = string.Empty;
                        var Material_IdList = materialInfoRepository.GetAll().Where(p => (p.Material_Id.Length == 10 && p.Material_Id.Contains(dto.Material_Id))).OrderByDescending(q => q.Material_Id);
                        if (Material_IdList.FirstOrDefault() != null)
                        {
                            //iteammaterial.Material_Id = dto.Material_Id + (maxMaterial_IdNum + 1);
                            MaxMaterial_Id = Material_IdList.FirstOrDefault().Material_Id;
                            string maxnumber = MaxMaterial_Id.Substring(7, MaxMaterial_Id.Length - 7);
                            int maxMaterial_IdNum = 1;
                            int.TryParse(maxnumber, out maxMaterial_IdNum);
                            int NewMaterialIDNum = maxMaterial_IdNum + 1;

                            if (NewMaterialIDNum > 999)
                            {
                                return "料号" + NewMaterialIDNum + "已经超过最大限度，请修改";
                            }
                            string NewMaterial_ID = string.Empty;
                            //3位流水码补0
                            string zeroCount = string.Empty;
                            for (int i = 0; i < 3 - NewMaterialIDNum.ToString().Length; i++)
                            {
                                zeroCount += "0";
                            }

                            NewMaterial_ID = zeroCount + NewMaterialIDNum.ToString();

                            iteammaterial.Material_Id = dto.Material_Id + NewMaterial_ID;
                        }
                        else
                        {
                            iteammaterial.Material_Id = dto.Material_Id + "001";
                        }
                    }
                    #endregion
                    else
                    {
                        iteammaterial.Material_Id = dto.Material_Id;
                    }

                    iteammaterial.Assembly_Name = dto.Material_Id + "_" + dto.Material_Name + "_" + dto.Material_Types;

                    var hasitem1 = materialInfoRepository.GetFirstOrDefault(m => m.Material_Id == dto.Material_Id & m.Material_Uid != dto.Material_Uid && m.Organization_UID == dto.Plant_OrganizationUID); //2019-03-05号修改，Frank要求修改为“厂区+料号+型号”唯一。

                    if (hasitem1 != null)
                        return "料号" + hasitem1.Material_Id + "已存在,请修改";

                    //var hasitem = materialInfoRepository.GetFirstOrDefault(m => m.Material_Types == dto.Material_Types & m.Material_Uid != dto.Material_Uid && m.Organization_UID == dto.Plant_OrganizationUID); //2019-03-01号修改，Frank要求修改为“厂区+料号+型号”唯一。

                    //if (hasitem != null)
                    //    return "型号" + hasitem.Material_Types + "已存在,请修改";
                    materialInfoRepository.Add(iteammaterial);
                }

                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "更新料号关系失败:" + e.Message;
            }

        }

        public string DeleteMaterial(int Material_Uid)
        {
            return materialInfoRepository.DeleteMaterial(Material_Uid);
        }

        public string InsertMaterialItem(List<MaterialInfoDTO> dtolist)
        {
            return materialInfoRepository.InsertItem(dtolist);
        }
        #endregion

        #region Error Info

        public PagedListModel<ErrorInfoDTO> QueryErrorInfo(ErrorInfoDTO errorInfoDTO, Page page)
        {
            var result = new List<ErrorInfoDTO>();
            var query = enumerationRepository.GetMany(m => m.Enum_Name == "EQPError_Type" || m.Enum_Name == "EQPResult" || m.Enum_Name == "EQPError_Level");
            if (!string.IsNullOrWhiteSpace(errorInfoDTO.ErrorType))
                query = query.Where(m => m.Decription.Contains(errorInfoDTO.ErrorType));
            if (!string.IsNullOrWhiteSpace(errorInfoDTO.Value))
                query = query.Where(m => m.Enum_Value.Contains(errorInfoDTO.Value));
            var count = query.Count();
            query = query.OrderByDescending(m => m.Enum_Type).ThenBy(m => m.Enum_Name).GetPage(page);
            foreach (var item in query)
            {
                ErrorInfoDTO errorinfo = new ErrorInfoDTO();
                errorinfo.Enum_UID = item.Enum_UID;
                errorinfo.ErrorType = item.Decription;
                errorinfo.Value = item.Enum_Value;
                result.Add(errorinfo);
            }
            return new PagedListModel<ErrorInfoDTO>(count, result);
        }




        public List<ErrorInfoDTO> QueryErrorInfoSingle(int Enum_UID)
        {
            List<ErrorInfoDTO> result = new List<ErrorInfoDTO>();
            ErrorInfoDTO errorinfo = new ErrorInfoDTO();
            var allnum = enumerationRepository.GetMany(m => m.Enum_UID == Enum_UID).ToList();
            errorinfo.Enum_UID = allnum[0].Enum_UID;
            errorinfo.ErrorType = allnum[0].Decription;
            errorinfo.Value = allnum[0].Enum_Value;
            result.Add(errorinfo);
            return result;
        }

        public string DeleteErrorInfo(int Enum_UID)
        {
            return enumerationRepository.DeleteByUid(Enum_UID);
        }

        public string AddErrorInfoItem(ErrorInfoDTO dto, bool isEdit)
        {
            try
            {
                Enumeration enumerrorinfo = new Enumeration();
                if (dto.ErrorType == "故障种类维护")
                {
                    enumerrorinfo.Enum_Type = "EQPError_Type";
                    enumerrorinfo.Enum_Name = "EQPError_Type";
                }
                else if (dto.ErrorType == "处理结果维护")
                {
                    enumerrorinfo.Enum_Type = "EQPResult";
                    enumerrorinfo.Enum_Name = "EQPResult";
                }
                else if (dto.ErrorType == "状况级别维护")
                {
                    enumerrorinfo.Enum_Type = "EQPError_Level";
                    enumerrorinfo.Enum_Name = "EQPError_Level";
                }
                enumerrorinfo.Enum_Value = dto.Value;
                enumerrorinfo.Decription = dto.ErrorType;
                enumerrorinfo.Enum_UID = dto.Enum_UID;
                if (isEdit)
                    enumerationRepository.Update(enumerrorinfo);
                else
                    enumerationRepository.Add(enumerrorinfo);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "已经添加了相同的故障种类，请从新添加！";
            }
        }
        #endregion

        #region  EQPRepair

        public List<EnumerationDTO> GetAllErrorTypes()
        {
            List<EnumerationDTO> result = new List<EnumerationDTO>();

            var enumerlist = enumerationRepository.GetMany(m => m.Enum_Type == "EQPError_Type" || m.Enum_Type == "EQPError_Level" || m.Enum_Type == "EQPResult").ToList();
            foreach (var item in enumerlist)
            {
                result.Add(AutoMapper.Mapper.Map<EnumerationDTO>(item));
            }
            return result;
        }


        public List<MaterialInfoDTO> GetAllMaterial(int planId = 0)
        {
            List<MaterialInfoDTO> result = new List<MaterialInfoDTO>();
            var materiallist = new List<Material_Info>();
            // var materiallist = materialInfoRepository.GetAll().Where(o => o.Is_Enable == true);
            if (planId != 0)
            {
                materiallist = materialInfoRepository.GetAll().Where(o => o.Is_Enable == true && o.Organization_UID == planId).ToList();
            }
            else
            {
                materiallist = materialInfoRepository.GetAll().Where(o => o.Is_Enable == true).ToList();
            }
            foreach (var item in materiallist)
            {
                result.Add(AutoMapper.Mapper.Map<MaterialInfoDTO>(item));
            }
            return result;
        }

        public List<string> GetUnitMaterial()
        {
            var materiallist = materialInfoRepository.GetUnitMat();
            return materiallist;
        }

        public Dictionary<int, string> AddManyEQPRepairs(Dictionary<int, EQPRepairInfoDTO> dics)
        {
            Dictionary<int, string> row_errors = new Dictionary<int, string>();
            //eqp_id不再拼接到维修编号,Jay 2018-02-12
            string repaireduid = DateTime.Now.ToString("yyyyMMdd");// + eqp_id.ToString();
            var currentcount = 1;
            var lastItem = eQPRepairInfoRepository.GetAll().Where(o => o.Repair_id.Contains(repaireduid)).OrderByDescending(o => o.Repair_id).FirstOrDefault();
            if (lastItem != null)
            {
                var maxNum = lastItem.Repair_id.Substring(lastItem.Repair_id.Length - 4, 4);
                currentcount = int.Parse(maxNum);
            }
            foreach (var dic in dics)
            {
                currentcount++;
                EQPRepairInfoDTO dto = dic.Value;
                try
                {
                    // get user
                    string[] mentionerSplit = dto.Mentioner.Split('_');
                    var user = systemUserRepository.GetUserInfoByNTID(mentionerSplit[1]).FirstOrDefault();
                    if (user == null)
                    {
                        row_errors.Add(dic.Key, "查无提报人员信息");
                        continue;
                    } else
                    {
                        dic.Value.Mentioner = string.Format("{0}_{1}", user.User_Name, user.User_NTID);
                    }
                    // get eqp_id
                    var eqp = equipmentInfoRepository.GetEqpBySerialAndEMT(dto.Mfg_Serial_Num, dto.Equipment);
                    if (eqp == null)
                    {
                        row_errors.Add(dic.Key, "查无此设备");
                        continue;
                    }
                    else
                    {
                        dto.EQP_Uid = eqp.EQP_Uid;
                    }
                    var eqprepair = eQPRepairInfoRepository.GetFirstOrDefault(m => m.EQP_Uid == dto.EQP_Uid & m.Repair_Result != "已完成" & m.Repair_Reason == dto.Repair_Reason & m.Repair_Uid != dto.Repair_Uid);
                    if (eqprepair != null)
                    {
                        row_errors.Add(dic.Key, "同一机台同一故障不可重复报修");
                        continue;
                    }
                    ////eqp_id不再拼接到维修编号,Jay 2018-02-12
                    //string repaireduid = DateTime.Now.ToString("yyyyMMdd");// + eqp_id.ToString();
                    //var currentcount = 1;
                    //var lastItem = eQPRepairInfoRepository.GetAll().Where(o => o.Repair_id.Contains(repaireduid)).OrderByDescending(o => o.Repair_id).FirstOrDefault();
                    //if (lastItem != null)
                    //{
                    //    var maxNum = lastItem.Repair_id.Substring(lastItem.Repair_id.Length - 4, 4);
                    //    currentcount = int.Parse(maxNum) + 1;
                    //}
                    //四位流水号
                 var   repaireduidnew = repaireduid + currentcount.ToString().PadLeft(4, '0');
                    // column mapping....
                    EQPRepair_Info item = new EQPRepair_Info();
                    item.Status = "Create";
                    item.EQP_Uid = dto.EQP_Uid;
                    item.Error_Types = dto.Error_Types == null ? "" : dto.Error_Types;
                    item.Error_Time = dto.Error_Time;
                    item.Error_Level = dto.Error_Level;
                    
                    item.Contact = dto.Contact;
                    item.Contact_tel = dto.Contact_tel;

                    item.Modified_UID = dto.Modified_UID;
                    item.Modified_Date = DateTime.Now;

                    item.Repair_Reason = dto.Repair_Reason;
                    item.All_RepairCost = 0;
                    item.Apply_Time = dto.Apply_Time;
                    item.Mentioner = dto.Mentioner;

                    item.EQPUSER_Uid = dto.EQPUSER_Uid;

                    item.Repair_id = repaireduidnew;
                    if (!string.IsNullOrEmpty(dto.CostCtr_ID))
                    {
                        var costCtr = costCtrInfoRepository.GetFirstOrDefault(c => c.CostCtr_ID == dto.CostCtr_ID);
                        if (costCtr == null)
                        {
                            row_errors.Add(dic.Key, "查无此成本中心");
                            continue;
                        }
                        else
                        {
                            item.CostCtr_UID = costCtr.CostCtr_UID;
                        }
                    }
                    eQPRepairInfoRepository.Add(item);
                }
                catch (Exception e)
                {
                    row_errors.Add(dic.Key, "保存设备申请单失败，请检查你的权限！" + e.Message);
                }
            }
            try
            {
                if (row_errors.Count == 0)
                    unitOfWork.Commit();
            }
            catch(Exception ex)
            {
                row_errors.Add(row_errors.Max(x => x.Key) + 1, "数据库储存失败" + ex.Message);
            }
            return row_errors;
        }

        public Dictionary<int, string> AddManyEQPRepairs2(Dictionary<int, EQPRepairInfoDTO> dics1, Dictionary<double, Dictionary<int, LaborUsingInfoDTO>> dics2, Dictionary<double, Dictionary<int, MeterialUpdateInfoDTO>> dics3)
        {
            Dictionary<int, string> row_errors = new Dictionary<int, string>();
            foreach (var dic in dics1)
            {
                EQPRepairInfoDTO dto = dic.Value;
                var item = eQPRepairInfoRepository.GetFirstOrDefault(x => x.Repair_id == dto.Repair_id);
                if (item == null)
                {
                    row_errors.Add(dic.Key, "查无此维修单");
                    continue;
                }
                else
                {
                    var eqp = equipmentInfoRepository.GetEqpBySerialAndEMT(dto.Mfg_Serial_Num, dto.Equipment);
                    if (eqp == null)
                    {
                        row_errors.Add(dic.Key, "维修单设备号不正确");
                        continue;
                    }
                }
                if (dto.Repair_Result == "已完成" && item.Repair_Result != "已完成" && dto.Repair_EndTime < DateTime.Now.AddHours(-24))
                {
                    row_errors.Add(dic.Key, "处理结果改为【已完成】,需要修改完成时间(不可比当前时间小24小时以上)");
                    continue;
                }
                try
                {
                    item.Status = "Commit";
                    item.Repair_Result = dto.Repair_Result;
                    item.Repair_Method = dto.Repair_Method;
                    item.Error_Types = dto.Error_Types;
                    //item.Repair_Reason = dto.Repair_Reason;
                    item.Reason_Analysis = dto.Reason_Analysis;
                    item.Repair_BeginTime = dto.Repair_BeginTime;
                    item.Repair_EndTime = dto.Repair_EndTime;

                    laborUsingInfoRepository.Delete(x => x.Repair_Uid == item.Repair_Uid);
                    if (dics2.ContainsKey(double.Parse(dto.Repair_id)) == true)
                    {
                        int laborCount = dics2[double.Parse(dto.Repair_id)].Count();
                        item.Labor_Time = Math.Round((decimal)dto.Labor_Time / (laborCount == 0 ? 1 : laborCount), 2);
                        item.Labor_List = dto.Labor_List;
                        foreach (var labor in dics2[double.Parse(dto.Repair_id)].Values)
                        {
                            var laborItem = new Labor_UsingInfo();
                            laborItem.Repair_Uid = item.Repair_Uid;
                            laborItem.EQPUser_Uid = labor.EQPUser_Uid;
                            laborItem.Modified_UID = dto.Modified_UID;
                            laborItem.Modified_Date = DateTime.Now;
                            laborUsingInfoRepository.Add(laborItem);
                        }
                    }

                    meterialUpdateInfoRepository.Delete(x => x.Repair_Uid == item.Repair_Uid);
                    if (dics3.ContainsKey(double.Parse(dto.Repair_id)) == true)
                    {
                        item.Update_Part = dto.Update_Part;
                        foreach (var material in dics3[double.Parse(dto.Repair_id)].Values)
                        {
                            var materialItem = new Meterial_UpdateInfo();
                            materialItem.Repair_Uid = item.Repair_Uid;
                            materialItem.Material_Uid = material.Material_Uid;
                            materialItem.EQP_Uid = item.EQP_Uid;
                            materialItem.Update_No = material.Update_No;
                            materialItem.Update_Cost = material.Update_Cost;
                            materialItem.Update_DateTime = DateTime.Now;
                            materialItem.Modified_UID = dto.Modified_UID;
                            materialItem.Modified_Date = DateTime.Now;
                            materialItem.Reason_Analysis = string.Empty;
                            meterialUpdateInfoRepository.Add(materialItem);
                        }
                    }
                    item.All_RepairCost = dto.All_RepairCost;
                    item.Repair_Remark = dto.Repair_Remark;
                    item.Modified_UID = dto.Modified_UID;
                    item.Modified_Date = DateTime.Now;
                    eQPRepairInfoRepository.Update(item);
                }
                catch (Exception e)
                {
                    row_errors.Add(dic.Key, "保存设备申请单失败，请检查你的权限！" + e.Message);
                }
            }
            try
            {
                if (row_errors.Count == 0)
                    unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                row_errors.Add(row_errors.Max(x => x.Key) + 1, "数据库储存失败" + ex.Message);
            }
            return row_errors;
        }

        public Dictionary<int, string> AddManyEQPRepairsClose(Dictionary<int, EQPRepairInfoDTO> dics)
        {
            Dictionary<int, string> row_errors = new Dictionary<int, string>();
            foreach (var dic in dics)
            {
                EQPRepairInfoDTO dto = dic.Value;
                var item = eQPRepairInfoRepository.GetFirstOrDefault(x => x.Repair_id == dto.Repair_id);
                if (item == null)
                {
                    row_errors.Add(dic.Key, "查无此维修单");
                    continue;
                }
                else
                {
                    var eqp = equipmentInfoRepository.GetEqpBySerialAndEMT(dto.Mfg_Serial_Num, dto.Equipment);
                    if (eqp == null)
                    {
                        row_errors.Add(dic.Key, "维修单设备号不正确");
                        continue;
                    }
                }
                if (item.Repair_Result != "已完成")
                {
                    row_errors.Add(dic.Key, "结案前维修单必需是已完成");
                    continue;
                }
                if (item.EQPUSER_Uid == dto.Modified_UID)
                {
                    // prepare...
                    item.Status = "Close";
                    item.Modified_UID = dto.Modified_UID;
                    item.Modified_Date = dto.Modified_Date;
                    // update
                    eQPRepairInfoRepository.Update(item);
                }
                else
                {
                    row_errors.Add(dic.Key, "结案必须和报修同一员");
                    continue;
                }
            }
            try
            {
                if (row_errors.Count == 0)
                    unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                row_errors.Add(row_errors.Max(x => x.Key) + 1, "数据库储存失败" + ex.Message);
            }
            return row_errors;
        }

        public string AddOrEditEQPRepair(EQPRepairInfoDTO dto, int eqp_id, bool isEdit)
        {
            var eqprepair = eQPRepairInfoRepository.GetFirstOrDefault(m => m.EQP_Uid == dto.EQP_Uid & m.Repair_Result != "已完成"
            & m.Repair_Reason == dto.Repair_Reason & m.Repair_Uid != dto.Repair_Uid);
            if (eqprepair != null)
                return "同一机台同一故障不可重复报修";
            try
            {
                EQPRepair_Info item;
                if (!isEdit) {
                    item = new EQPRepair_Info();
                    //eqp_id不再拼接到维修编号,Jay 2018-02-12
                    string repaireduid = DateTime.Now.ToString("yyyyMMdd");// + eqp_id.ToString();
                    var currentcount = 1;
                    var lastItem = eQPRepairInfoRepository.GetAll().Where(o => o.Repair_id.Contains(repaireduid)).OrderByDescending(o => o.Repair_id).FirstOrDefault();
                    if (lastItem != null)
                    {
                        var maxNum = lastItem.Repair_id.Substring(lastItem.Repair_id.Length - 4, 4);
                        currentcount = int.Parse(maxNum) + 1;
                    }
                    //int currentcount = eQPRepairInfoRepository.GetInfoCount(dto.Error_Time);
                    //四位流水号
                    switch (currentcount.ToString().Length)
                    {
                        case 1:
                            repaireduid += "000" + currentcount.ToString();
                            break;
                        case 2:
                            repaireduid += "00" + currentcount.ToString();
                            break;
                        case 3:
                            repaireduid += "0" + currentcount.ToString();
                            break;
                        default:
                            repaireduid += currentcount.ToString();
                            break;
                    }
                    item.Repair_id = repaireduid;
                }
                else
                {
                    item = eQPRepairInfoRepository.GetFirstOrDefault(m => m.Repair_Uid == dto.Repair_Uid);
                }
                item.EQP_Uid = dto.EQP_Uid;
                item.Status = "Create";
                item.Error_Types = dto.Error_Types == null ? "" : dto.Error_Types;
                item.Error_Time = dto.Error_Time;
                item.Error_Level = dto.Error_Level;
                item.Modified_UID = dto.Modified_UID;
                item.Contact = dto.Contact;
                item.Contact_tel = dto.Contact_tel;
                item.Modified_Date = DateTime.Now;
                item.Repair_Reason = dto.Repair_Reason;
                item.All_RepairCost = 0;
                item.Error_Types = "";
                item.Apply_Time = dto.Apply_Time;
                item.Mentioner = dto.Mentioner;
                //成本中心信息
                item.CostCtr_UID = dto.CostCtr_UID;
                if (!isEdit)
                {
                    eQPRepairInfoRepository.Add(item);
                }
                else
                {
                    //item.Repair_Uid = dto.Repair_Uid;
                    eQPRepairInfoRepository.Update(item);
                }
                unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception e)
            {
                return "保存设备申请单失败，请检查你的权限！" + e.Message.ToString();
            }
        }

        public string AddOrEditEQPRepair2(EQPRepairInfoDTO dto, bool MH_Flag)
        {
            try
            {
                var item = eQPRepairInfoRepository.GetById(dto.Repair_Uid);
                if (dto.Repair_Result == "已完成" && item.Repair_Result != "已完成" && dto.Repair_EndTime < DateTime.Now.AddHours(-24))
                {
                    return "处理结果改为【已完成】,需要修改完成时间(不可比当前时间小24小时以上)";
                }
                item.Repair_Uid = dto.Repair_Uid;
                item.EQP_Uid = dto.EQP_Uid;
                item.Repair_Method = dto.Repair_Method;
                item.Status = dto.Status;
                item.Reason_Types = dto.Reason_Types;
                item.Repair_Method = dto.Repair_Method;
                item.Repair_BeginTime = dto.Repair_BeginTime;
                item.Repair_EndTime = dto.Repair_EndTime;
                item.Repair_Result = dto.Repair_Result;
                item.Reason_Analysis = dto.Reason_Analysis;
                item.Error_Types = dto.Error_Types;
                item.Repair_Remark = dto.Repair_Remark;
                //成本中心信息
                item.CostCtr_UID = dto.CostCtr_UID;

                var labors = laborUsingInfoRepository.GetByRepairUid(dto.Repair_Uid);
                if (labors != null)
                {
                    item.Labor_List = "";
                    foreach (var labor in labors)
                    {
                        item.Labor_List += labor.User_Name + ";";
                    }
                }
                item.Labor_Time = Math.Round((decimal)dto.Labor_Time / labors.Count(), 2);
                var mats = meterialUpdateInfoRepository.GetByRepairUid(dto.Repair_Uid);
                if (mats != null)
                {
                    item.Update_Part = "";
                    item.All_RepairCost = 0;
                    foreach (var mat in mats)
                    {
                        item.All_RepairCost += mat.Update_Cost;
                        item.Update_Part += mat.Material_Name + "*" + mat.Update_No + ";";
                    }
                }
                if (MH_Flag)
                {
                    if (dto.EQPUSER_Uid == null)
                        dto.EQPUSER_Uid = 0;
                    var hasitem = eqpuserTableRepository.GetFirstOrDefault(m => m.EQPUser_Uid == dto.EQPUSER_Uid);
                    if (hasitem != null)
                        item.EQPUSER_Uid = dto.EQPUSER_Uid;
                    else
                    {
                        var hasitem2 = systemUserRepository.GetFirstOrDefault(m => m.Account_UID == dto.EQPUSER_Uid);
                        if (hasitem2 != null)
                            item.Modified_UID = (int)dto.EQPUSER_Uid;
                        else
                            return "登陆人员工号不存在,请重新登陆";
                    }
                }
                else
                {
                    var hasitem = systemUserRepository.GetFirstOrDefault(m => m.Account_UID == dto.Modified_UID);
                    if (hasitem != null)
                        item.Modified_UID = dto.Modified_UID;
                    else
                    {
                        var hasitem2 = eqpuserTableRepository.GetFirstOrDefault(m => m.EQPUser_Uid == dto.Modified_UID);
                        if (hasitem2 != null)
                            item.EQPUSER_Uid = dto.Modified_UID;
                        return "登陆人员工号不存在,请重新登陆";
                    }
                }
                item.Modified_Date = DateTime.Now;
                eQPRepairInfoRepository.Update(item);
                unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception e)
            {
                return "保存设备申请单失败！\n" + e.ToString();
            }
        }

        public PagedListModel<EQPRepairInfoDTO> QueryRepairInfo(EQPRepairInfoSearchDTO searchModel, Page page)
        {
            int totalcount;
            decimal allcost;
            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.Organization_UID);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = eQPRepairInfoRepository.GetInfo(searchModel, organization_UIDs, page, out totalcount, out allcost).ToList();
            var bd = new PagedListModel<EQPRepairInfoDTO>(totalcount, result);
            foreach (var test in bd.Items)
            {
                var updates = meterialUpdateInfoRepository.GetMany(m => m.Repair_Uid == test.Repair_Uid);
                foreach (var item in updates)
                {
                    if (!string.IsNullOrWhiteSpace(item.Reason_Analysis))
                    {
                        if (string.IsNullOrWhiteSpace(test.Reason_Analysis))
                            test.Reason_Analysis = item.Reason_Analysis;
                        else
                            test.Reason_Analysis += "," + item.Reason_Analysis;
                    }
                }
                test.allcost = allcost.ToString();
            }
            return bd;
        }

        public EQPRepairInfoDTO QueryEQPRepairSingle(int Repair_Uid)
        {
            var bud = eQPRepairInfoRepository.GetByUId(Repair_Uid);
            return bud;
        }

        public string DeleteEQPRepairInfo(int Repair_Uid)
        {
            return eQPRepairInfoRepository.DeleteByUid(Repair_Uid);
        }

        public List<MeterialUpdateInfoDTO> QueryMaterialUpdateSingle(int Material_Uid)
        {
            var bud = meterialUpdateInfoRepository.GetByUId(Material_Uid);
            return bud;
        }


        public string UpdateLabor(List<LaborUsingInfoDTO> laborlist, int userid, int Repair_Uid, bool MH_Flag)
        {
            try
            {
                string deletemessage = laborUsingInfoRepository.DeleteByRepairUid(Repair_Uid);
                if (deletemessage != "")
                    return deletemessage;

                foreach (var labor in laborlist)
                {
                    Labor_UsingInfo item = new Labor_UsingInfo();
                    item.Repair_Uid = Repair_Uid;
                    item.EQPUser_Uid = labor.EQPUser_Uid;
                    if (MH_Flag)
                        item.Modified_EQPUser_Uid = userid;
                    else
                        item.Modified_UID = userid;
                    item.Modified_Date = DateTime.Now;
                    laborUsingInfoRepository.Add(item);
                }
                unitOfWork.Commit();
                return "Success";
            }
            catch (Exception e)
            {
                return "更新维修人员失败:" + e.Message;
            }

        }

        public string UpdateMat(List<MeterialUpdateInfoDTO> matlist, int userid, DateTime updatetime,
                                int EQP_Uid, int Repair_Uid, bool MH_Flag)
        {
            try
            {
                string deletemessage = meterialUpdateInfoRepository.DeleteByRepairUid(Repair_Uid);
                if (deletemessage != "")
                    return deletemessage;
                foreach (var mat in matlist)
                {
                    Meterial_UpdateInfo item = new Meterial_UpdateInfo();
                    item.Repair_Uid = Repair_Uid;
                    item.EQP_Uid = EQP_Uid;
                    item.Material_Uid = mat.Material_Uid;
                    item.Update_DateTime = updatetime;
                    item.Update_No = mat.Update_No;
                    item.Update_Cost = mat.Update_Cost;
                    item.Reason_Analysis = mat.Reason_Analysis;
                    if (MH_Flag)
                        item.Modified_EQPUser_Uid = userid;
                    else
                        item.Modified_UID = userid;
                    item.Modified_Date = DateTime.Now;
                    meterialUpdateInfoRepository.Add(item);
                }
                unitOfWork.Commit();
                return "Success";
            }
            catch (Exception e)
            {
                return "更新更换材料失败:" + e.Message;
            }

        }

        public List<laborlist> GetLaborList(int Repair_Uid)
        {
            var bud = laborUsingInfoRepository.GetByRepairUid(Repair_Uid);
            return bud;
        }

        public List<matlist> GetMatList(int Repair_Uid)
        {
            var bud = meterialUpdateInfoRepository.GetByRepairUid(Repair_Uid);
            return bud;
        }


        public List<string> QueryUserRole(string userid)
        {
            var equipment = equipmentInfoRepository.Getuserrole(userid);
            List<string> result = new List<string>();
            foreach (var item in equipment)
            {
                result.Add(item.Role_ID);
            }
            return result;
        }

        public List<string> GetOptype()
        {
            var equipment = equipmentInfoRepository.GetDistinctoptype();
            List<string> result = new List<string>();
            foreach (var item in equipment)
            {
                result.Add(item.OP_TYPES);
            }
            return result;
        }
        public List<System_Organization_PlantDTO> GetOrganization_Plants()
        {
            var equipment = equipmentInfoRepository.GetOrganization_Plants();
            return equipment;
        }
        public List<SystemProjectDTO> GetOptypeByUser(int optype)
        {
            var equipment = equipmentInfoRepository.GetDistinctoptypeByUser(optype);
            return equipment;
        }

        public List<EquipmentInfoDTO> GetProjectnameByOptype(string Optype)
        {
            var bud = equipmentInfoRepository.GetProjectnameByOptype(Optype);
            return bud;
        }

        public List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes = "")
        {
            var bud = equipmentInfoRepository.GetFunPlantByOPType(Optype, Optypes);
            return bud;
        }
        public List<EquipmentInfoDTO> GetProcessByFunplant(string funplantuid)
        {
            var bud = equipmentInfoRepository.GetProcessByFunplant(funplantuid);
            return bud;
        }
        public List<string> GetEqpidByProcess(string funplantuid)
        {
            var bud = equipmentInfoRepository.GetEqpidByProcess(funplantuid);
            return bud;
        }

        public List<EQPRepairInfoDTO> DoExportFunction(string uids)
        {
            var bd = eQPRepairInfoRepository.DoExportFunction(uids);
            foreach (var test in bd)
            {
                var updates = meterialUpdateInfoRepository.GetMany(m => m.Repair_Uid == test.Repair_Uid);
                foreach (var item in updates)
                {
                    if (!string.IsNullOrWhiteSpace(item.Reason_Analysis))
                    {
                        if (string.IsNullOrWhiteSpace(test.Reason_Analysis))
                            test.Reason_Analysis = item.Reason_Analysis;
                        else
                            test.Reason_Analysis += "," + item.Reason_Analysis;
                    }
                }
            }
            return bd;
        }
        public List<EQPRepairInfoDTO> DoAllEQPMaintenanceReprot(EQPRepairInfoSearchDTO search)
        {
            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(search.Plant_OrganizationUID, search.Organization_UID);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = eQPRepairInfoRepository.DoAllEQPMaintenanceReprot(search, organization_UIDs).ToList();
            var updatealls = meterialUpdateInfoRepository.GetAll().ToList();
            foreach (var test in result)
            {
                var updates = updatealls.Where(m => m.Repair_Uid == test.Repair_Uid);
                foreach (var item in updates)
                {
                    if (!string.IsNullOrWhiteSpace(item.Reason_Analysis))
                    {
                        if (string.IsNullOrWhiteSpace(test.Reason_Analysis))
                            test.Reason_Analysis = item.Reason_Analysis;
                        else
                            test.Reason_Analysis += "," + item.Reason_Analysis;
                    }
                }

            }
            return result;
        }

        public List<CostCtrDTO> GetCostCtrs()
        {
            var costCenters = costCtrInfoRepository.GetCostCtrs();
            return costCenters;
        }

        #endregion EQPRepair

        #region CostCenterMaintenace Module Add By Darren 2018/12/18

        public PagedListModel<CostCenterItem> QueryCostCenters(CostCenterModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var costCenters = costCtrInfoRepository.QueryCostCenters(searchModel, page, out totalCount).AsEnumerable();

            return new PagedListModel<CostCenterItem>(totalCount, costCenters);
        }

        public CostCtr_infoDTO QueryCostCenter(int uid)
        {
            return costCtrInfoRepository.QueryCostCenter(uid);
        }

        public string AddCostCenter(CostCtr_infoDTO vm)
        {
            if (costCtrInfoRepository.GetMany(c => c.CostCtr_ID == vm.CostCtr_ID).Count() > 0)
            {
                return string.Format("item with same parent Cost Center ID [{0}] is exist!", vm.CostCtr_ID);
            }

            var now = DateTime.Now;
            //var costCtrEntity = AutoMapper.Mapper.Map<CostCtr_info>(vm);
            var costCtrEntity = new CostCtr_info();
            costCtrEntity.Plant_Organization_UID = vm.Plant_Organization_UID;
            costCtrEntity.BG_Organization_UID = vm.BG_Organization_UID;
            costCtrEntity.FunPlant_Organization_UID = vm.FunPlant_Organization_UID;
            costCtrEntity.CostCtr_ID = vm.CostCtr_ID;
            costCtrEntity.CostCtr_Description = vm.CostCtr_Description;
            costCtrEntity.Modified_UID = vm.Modified_UID;
            costCtrEntity.Modified_Date = now;
            costCtrInfoRepository.Add(costCtrEntity);
            unitOfWork.Commit();

            return "SUCCESS";
        }

        public string ModifyCostCenter(CostCtr_infoDTO vm)
        {
            if (costCtrInfoRepository.GetMany(c => c.CostCtr_UID != vm.CostCtr_UID && c.CostCtr_ID == vm.CostCtr_ID).Count() > 0)
            {
                return string.Format("item with same parent Cost Center ID [{0}] is exist!", vm.CostCtr_ID);
            }

            var now = DateTime.Now;
            var costCtrEntity = costCtrInfoRepository.GetFirstOrDefault(c => c.CostCtr_UID == vm.CostCtr_UID);
            if (costCtrEntity != null)
            {
                //costCtrEntity.Plant_Organization_UID = vm.Plant_Organization_UID;
                //costCtrEntity.BG_Organization_UID = vm.BG_Organization_UID;
                costCtrEntity.FunPlant_Organization_UID = vm.FunPlant_Organization_UID;
                //costCtrEntity.CostCtr_ID = vm.CostCtr_ID;
                costCtrEntity.CostCtr_Description = vm.CostCtr_Description;
                costCtrEntity.Modified_UID = vm.Modified_UID;
                costCtrEntity.Modified_Date = now;
            }

            costCtrInfoRepository.Update(costCtrEntity);
            unitOfWork.Commit();

            return "SUCCESS";
        }

        public string DeleteCostCenter(int uid)
        {
            if (costCtrInfoRepository.GetById(uid) == null)
            {
                return "Record aleady deleted";
            }
            if (eQPRepairInfoRepository.CheckCostCtrIsExist(uid)) {
                return "This Cost Center is using by EQPRepair_Info";
            }            
            costCtrInfoRepository.Delete(c => c.CostCtr_UID == uid);
            unitOfWork.Commit();
            return "SUCCESS";
        }
        #endregion //End CostCenterMaintenace

        #region 电子看板
        public PagedListModel<EQPRepairInfoDTO> getShowContent(EQPRepairInfoDTO search, Page page)
        {
            var totalCount = 0;
            var searchModel = new EQPRepairInfoDTO
            {
                EQP_Location = search.EQP_Location
            };

            var result = eQPRepairInfoRepository.GetEQPBoardInfo(searchModel, page, out totalCount).ToList();
            //var listDTO = list.ToList();
            return new PagedListModel<EQPRepairInfoDTO>(totalCount, result);
        }
        #endregion

        public PagedListModel<EQPRepairInfoDTO> QueryRepairReportInfo(EQPRepairInfoSearchDTO searchModel, Page page)
        {
            int totalcount;
            decimal allcost;
            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.Organization_UID);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = eQPRepairInfoRepository.GetReportInfo(searchModel, organization_UIDs, page, out totalcount, out allcost).ToList();
            var bd = new PagedListModel<EQPRepairInfoDTO>(totalcount, result);
            foreach (var test in bd.Items)
            {
                var updates = meterialUpdateInfoRepository.GetMany(m => m.Repair_Uid == test.Repair_Uid);
                foreach (var item in updates)
                {
                    if (!string.IsNullOrWhiteSpace(item.Reason_Analysis))
                    {
                        if (string.IsNullOrWhiteSpace(test.Reason_Analysis))
                            test.Reason_Analysis = item.Reason_Analysis;
                        else
                            test.Reason_Analysis += "," + item.Reason_Analysis;
                    }
                }
                test.allcost = allcost.ToString();
            }
            return bd;
        }

        public List<EQPRepairInfoDTO> DoExportFunction2(string optypes, string projectname, string funplant, string process,
                        string eqpid, string reason, string repairid, string location, string classdesc, string contact,
                        DateTime fromdate, DateTime todate, string errorlever, string repairresult, string labor,
                        string updatepart, string remark, string status)
        {
            var bd = eQPRepairInfoRepository.DoExportFunction2(optypes, projectname, funplant, process, eqpid, reason,
                repairid, location, classdesc, contact, fromdate, todate, errorlever, repairresult, labor,
                        updatepart, remark, status);

            var updatealls = meterialUpdateInfoRepository.GetAll().ToList();

            foreach (var test in bd)
            {
                var updates = updatealls.Where(m => m.Repair_Uid == test.Repair_Uid);
                //var updates = meterialUpdateInfoRepository.GetMany(m => m.Repair_Uid == test.Repair_Uid);
                foreach (var item in updates)
                {
                    if (!string.IsNullOrWhiteSpace(item.Reason_Analysis))
                    {
                        if (string.IsNullOrWhiteSpace(test.Reason_Analysis))
                            test.Reason_Analysis = item.Reason_Analysis;
                        else
                            test.Reason_Analysis += "," + item.Reason_Analysis;
                    }
                }
            }
            return bd;
        }

        public List<EQPRepairInfoDTO> DoPartExportFunctionInfo(string uids)
        {
            var bd = eQPRepairInfoRepository.DoPartExportFunctionInfo(uids);

            var updatealls = meterialUpdateInfoRepository.GetAll().ToList();

            foreach (var test in bd)
            {
                var updates = updatealls.Where(m => m.Repair_Uid == test.Repair_Uid);
                //var updates = meterialUpdateInfoRepository.GetMany(m => m.Repair_Uid == test.Repair_Uid);
                foreach (var item in updates)
                {
                    if (!string.IsNullOrWhiteSpace(item.Reason_Analysis))
                    {
                        if (string.IsNullOrWhiteSpace(test.Reason_Analysis))
                            test.Reason_Analysis = item.Reason_Analysis;
                        else
                            test.Reason_Analysis += "," + item.Reason_Analysis;
                    }
                }
            }
            return bd;
        }
        public string CloseEQPRepairInfo(int Repair_Uid)
        {
            return eQPRepairInfoRepository.ClosedByUid(Repair_Uid);
        }

        #region 重要料号原因维护

        public PagedListModel<MaterialReasonDTO> QueryMatReason(MaterialReasonDTO searchModel, Page page)
        {
            int totalcount;
            var result = materialReasonRepository.GetInfo(searchModel, page, out totalcount);
            return new PagedListModel<MaterialReasonDTO>(totalcount, result);
        }

        public string AddOrEditMaterialReason(MaterialReasonDTO dto)
        {
            try
            {
                if (dto.Material_Reason_UID == 0)
                {
                    Material_Reason MR = new Material_Reason();
                    MR.Material_UID = dto.Material_UID;
                    MR.Reason = dto.Reason;
                    MR.Modified_UID = dto.Modified_UID;
                    MR.Modified_Date = DateTime.Now;
                    materialReasonRepository.Add(MR);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var MR = materialReasonRepository.GetFirstOrDefault(m => m.Material_Reason_UID == dto.Material_Reason_UID);
                    MR.Material_UID = dto.Material_UID;
                    MR.Reason = dto.Reason;
                    MR.Modified_UID = dto.Modified_UID;
                    MR.Modified_Date = DateTime.Now;
                    materialReasonRepository.Update(MR);
                    unitOfWork.Commit();
                    return "";
                }

            }
            catch (Exception e)
            {
                return "维护重要料号原因失败:" + e.Message;
            }
        }

        public List<MaterialReasonDTO> QuerymatReasonByuid(int Material_Reason_UID)
        {
            var bud = materialReasonRepository.GetByUid(Material_Reason_UID).ToList();
            return bud;
        }

        public string DeleteMatReason(int Material_Reason_UID, int userid)
        {
            try
            {

                var MR = materialReasonRepository.GetById(Material_Reason_UID);
                materialReasonRepository.Delete(MR);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除重要料号原因失败:" + e.Message;
            }
        }

        public List<MaterialReasonDTO> GetMatReasonByMat(int Material_Uid)
        {
            var bud = materialReasonRepository.GetMany(m => m.Material_UID == Material_Uid).ToList();
            List<MaterialReasonDTO> dtoList = new List<MaterialReasonDTO>();
            foreach (var item in bud)
            {
                dtoList.Add(AutoMapper.Mapper.Map<MaterialReasonDTO>(item));
            }
            return dtoList;
        }

        public string InsertMatReasonItem(List<MaterialReasonDTO> dtolist)
        {
            return materialReasonRepository.InsertItem(dtolist);
        }
        public List<MaterialReasonDTO> DoExportMatReasonReprot(string Material_Reason_UIDs)
        {
            return materialReasonRepository.DoExportMatReasonReprot(Material_Reason_UIDs);
        }

        public List<MaterialReasonDTO> DoAllExportMatReasonReprot(MaterialReasonDTO search)
        {
            return materialReasonRepository.DoAllExportMatReasonReprot(search);
        }
        #endregion 重要料号原因维护
        public PagedListModel<EquipmentReport> QueryEquipmentInfoNOTReprot(EquipmentReport searchModel, Page page)
        {
            var totalcount = 0;
            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.OPType_OrganizationUID);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = equipmentInfoRepository.GetEquipmentInfoNOTReprot(searchModel, organization_UIDs, page, out totalcount);
            return new PagedListModel<EquipmentReport>(totalcount, result);
        }
        public List<EquipmentReport> ExportEquipmentInfoNOTReprot(EquipmentReport searchModel)
        {

            List<int> organization_UIDs = new List<int>();

            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.OPType_OrganizationUID);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }

            var result = equipmentInfoRepository.GetEquipmentInfoNOTReprot(searchModel, organization_UIDs);
            return result.ToList();

        }
        public PagedListModel<EquipmentReport> QueryEquipmentInfoReprot(EquipmentReport searchModel, Page page)
        {
            var totalcount = 0;
            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.OPType_OrganizationUID);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = equipmentInfoRepository.GetEquipmentInfoReprot(searchModel, organization_UIDs, page, out totalcount);
            return new PagedListModel<EquipmentReport>(totalcount, result);
        }
        public List<EquipmentReport> ExportEquipmentInfoReprot(EquipmentReport searchModel)
        {

            List<int> organization_UIDs = new List<int>();
            var opTypes = systemProjectRepository.GetCurrentOPType(searchModel.Plant_OrganizationUID, searchModel.OPType_OrganizationUID);
            foreach (var item in opTypes)
            {
                organization_UIDs.Add(item.Organization_UID);
            }
            var result = equipmentInfoRepository.GetEquipmentInfoReprot(searchModel, organization_UIDs);
            return result.ToList();

        }
        public PagedListModel<Material_Maintenance_RecordDTO> QueryMaterial_Maintenance_Records(Material_Maintenance_RecordDTO searchModel, Page page)
        {
            int totalcount;
            var result = material_Maintenance_RecordRepository.QueryMaterial_Maintenance_Records(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<Material_Maintenance_RecordDTO>(totalcount, result);
            return bd;
        }
        public string AddOrEditMaterial_Maintenance_Record(Material_Maintenance_RecordDTO dto)
        {
            try
            {
                if (dto.Material_Maintenance_Record_UID == 0)
                {
                    Material_Maintenance_Record material_Maintenance_Record = new Material_Maintenance_Record();
                    material_Maintenance_Record.Plant_Organization_UID = dto.Plant_Organization_UID;
                    material_Maintenance_Record.BG_Organization_UID = dto.BG_Organization_UID;
                    if (dto.FunPlant_Organization_UID != null)
                    {
                        material_Maintenance_Record.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    }

                    material_Maintenance_Record.Material_UID = dto.Material_UID;
                    material_Maintenance_Record.Material_Seq = dto.Material_Seq;
                    if(dto.Is_Warranty!=null)
                    {

                        material_Maintenance_Record.Is_Warranty = dto.Is_Warranty.Value;

                    } else
                    {
                        material_Maintenance_Record.Is_Warranty = true;
                    }
             
                    material_Maintenance_Record.Submit_UID = dto.Submit_UID;
                    material_Maintenance_Record.Submit_Date = DateTime.Now;
                    if (dto.Judge_UID != null)
                    {
                        material_Maintenance_Record.Judge_UID = dto.Judge_UID;
                    }
                    if (dto.Judge_Date != null)
                    {
                        material_Maintenance_Record.Judge_Date = dto.Judge_Date;
                    }
                    //else
                    //{
                    //    material_Maintenance_Record.Judge_Date = DateTime.Now;
                    //}
                    material_Maintenance_Record.Abnormal = dto.Abnormal;
                    material_Maintenance_Record.Delivery_Date = dto.Delivery_Date;
                    material_Maintenance_Record.Expected_Return_Date = dto.Expected_Return_Date;
                    if (dto.Acceptance_Staff_UID != null)
                    {
                        material_Maintenance_Record.Acceptance_Staff_UID = dto.Acceptance_Staff_UID;
                    }
                    if (dto.Acceptance_Date != null)
                    {
                        material_Maintenance_Record.Acceptance_Date = dto.Acceptance_Date;
                    }
                    //else
                    //{
                    //    material_Maintenance_Record.Acceptance_Date = DateTime.Now;
                    //}
                    material_Maintenance_Record.Acceptance_Results = dto.Acceptance_Results;
                    material_Maintenance_Record.Vendor = dto.Vendor;
                    material_Maintenance_Record.Maintenance_Items = dto.Maintenance_Items;
                    if (dto.Maintenance_Fees != null)
                    {
                        material_Maintenance_Record.Maintenance_Fees = dto.Maintenance_Fees;
                    }
                    if (dto.Maintenance_Days != null)
                    {
                        material_Maintenance_Record.Maintenance_Days = dto.Maintenance_Days;
                    }
                    material_Maintenance_Record.Notes = dto.Notes;
                    //material_Maintenance_Record.Status_UID = dto.Status_UID;
                    material_Maintenance_Record.Status_UID = 1;
                    if (dto.Return_Date != null)
                    {
                        material_Maintenance_Record.Return_Date = dto.Return_Date;
                    }
                    //else
                    //{
                    //    material_Maintenance_Record.Return_Date = DateTime.Now;
                    //}

                    if (dto.Return_UID != null)
                    {
                        material_Maintenance_Record.Return_UID = dto.Return_UID;
                    }
                    if (dto.Return_Time != null)
                    {
                        material_Maintenance_Record.Return_Time = dto.Return_Time;
                    }
                    //else
                    //{
                    //    material_Maintenance_Record.Return_Time = DateTime.Now;
                    //}

                    if (dto.Modified_UID != null)
                    {
                        material_Maintenance_Record.Modified_UID = dto.Modified_UID;
                    }
                    if (dto.Modified_Date != null)
                    {
                        material_Maintenance_Record.Modified_Date = dto.Modified_Date;
                    }
                    else
                    {
                        material_Maintenance_Record.Modified_Date = DateTime.Now;
                    }
                    material_Maintenance_RecordRepository.Add(material_Maintenance_Record);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var material_Maintenance_Record = material_Maintenance_RecordRepository.GetFirstOrDefault(m => m.Material_Maintenance_Record_UID == dto.Material_Maintenance_Record_UID);
                    if (material_Maintenance_Record != null)
                    {
                        material_Maintenance_Record.Plant_Organization_UID = dto.Plant_Organization_UID;
                        material_Maintenance_Record.BG_Organization_UID = dto.BG_Organization_UID;
                        if (dto.FunPlant_Organization_UID != null)
                        {
                            material_Maintenance_Record.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        }

                        material_Maintenance_Record.Material_UID = dto.Material_UID;
                        material_Maintenance_Record.Material_Seq = dto.Material_Seq;
                        if (dto.Is_Warranty != null)
                        {

                            material_Maintenance_Record.Is_Warranty = dto.Is_Warranty.Value;

                        }
                        else
                        {
                            material_Maintenance_Record.Is_Warranty = true;
                        }
                        // material_Maintenance_Record.Is_Warranty = dto.Is_Warranty;
                        //material_Maintenance_Record.Submit_UID = dto.Submit_UID;
                        //material_Maintenance_Record.Submit_Date = dto.Submit_Date;
                        if (dto.Judge_UID != null)
                        {
                            material_Maintenance_Record.Judge_UID = dto.Judge_UID;
                        }
                        if (dto.Judge_Date != null)
                        {
                            material_Maintenance_Record.Judge_Date = dto.Judge_Date;
                        }
                        //else
                        //{
                        //    material_Maintenance_Record.Judge_Date = DateTime.Now;
                        //}
                        material_Maintenance_Record.Abnormal = dto.Abnormal;
                        material_Maintenance_Record.Delivery_Date = dto.Delivery_Date;
                        material_Maintenance_Record.Expected_Return_Date = dto.Expected_Return_Date;
                        if (dto.Acceptance_Staff_UID != null)
                        {
                            material_Maintenance_Record.Acceptance_Staff_UID = dto.Acceptance_Staff_UID;
                        }
                        if (dto.Acceptance_Date != null)
                        {
                            material_Maintenance_Record.Acceptance_Date = dto.Acceptance_Date;
                        }
                        //else
                        //{
                        //    material_Maintenance_Record.Acceptance_Date = DateTime.Now;
                        //}
                        material_Maintenance_Record.Acceptance_Results = dto.Acceptance_Results;
                        material_Maintenance_Record.Vendor = dto.Vendor;
                        material_Maintenance_Record.Maintenance_Items = dto.Maintenance_Items;
                        if (dto.Maintenance_Fees != null)
                        {
                            material_Maintenance_Record.Maintenance_Fees = dto.Maintenance_Fees;
                        }
                        if (dto.Maintenance_Days != null)
                        {
                            material_Maintenance_Record.Maintenance_Days = dto.Maintenance_Days;
                        }
                        material_Maintenance_Record.Notes = dto.Notes;                     
                        if (dto.Return_Date != null)
                        {
                            material_Maintenance_Record.Return_Date = dto.Return_Date;
                        }
                        //else
                        //{
                        //    material_Maintenance_Record.Return_Date = DateTime.Now;
                        //}

                        if (dto.Return_UID != null)
                        {
                            material_Maintenance_Record.Return_UID = dto.Return_UID;
                        }
                        if (dto.Return_Time != null)
                        {
                            material_Maintenance_Record.Return_Time = dto.Return_Time;
                        }
                        //else
                        //{
                        //    material_Maintenance_Record.Return_Time = DateTime.Now;
                        //}

                        if (dto.Modified_UID != null)
                        {
                            material_Maintenance_Record.Modified_UID = dto.Modified_UID;
                        }
                        if (dto.Modified_Date != null)
                        {
                            material_Maintenance_Record.Modified_Date = dto.Modified_Date;
                        }
                        //else
                        //{
                        //    material_Maintenance_Record.Modified_Date = DateTime.Now;
                        //}
                        material_Maintenance_RecordRepository.Update(material_Maintenance_Record);
                        unitOfWork.Commit();
                    }

                    return "0";
                }
            }
            catch (Exception e)
            {
                return "入库操作失败:" + e.Message;
            }
        }
        public Material_Maintenance_RecordDTO QueryMaterial_Maintenance_RecordByUid(int Material_Maintenance_Record_UID)
        {

            return material_Maintenance_RecordRepository.QueryMaterial_Maintenance_RecordByUid(Material_Maintenance_Record_UID);

        }

        public string Approve1Material_Maintenance_RecordByUid(int Material_Maintenance_Record_UID, int Useruid)
        {
            try
            {                             
                var material_Maintenance_Record = material_Maintenance_RecordRepository.GetFirstOrDefault(m => m.Material_Maintenance_Record_UID == Material_Maintenance_Record_UID);
                if (material_Maintenance_Record != null)
                {
                    material_Maintenance_Record.Judge_UID = Useruid;
                    material_Maintenance_Record.Judge_Date = DateTime.Now;
                    material_Maintenance_Record.Status_UID = 2;
                    material_Maintenance_RecordRepository.Update(material_Maintenance_Record);
                    unitOfWork.Commit();
                }
                return "";
              
            }
            catch (Exception e)
            {
                return "入库操作失败:" + e.Message;
            }

        }
        public string Approve2Material_Maintenance_Record(Material_Maintenance_RecordDTO dto)
        {
            try
            {
                if (dto.Material_Maintenance_Record_UID == 0)
                {
                    return "没找到审核的资料";
                }
                else
                {
                    var material_Maintenance_Record = material_Maintenance_RecordRepository.GetFirstOrDefault(m => m.Material_Maintenance_Record_UID == dto.Material_Maintenance_Record_UID);
                    if (material_Maintenance_Record != null)
                    {

                        material_Maintenance_Record.Return_Date = dto.Return_Date;
                        material_Maintenance_Record.Return_UID = dto.Return_UID;
                        material_Maintenance_Record.Return_Time = DateTime.Now;
                        material_Maintenance_Record.Maintenance_Items = dto.Maintenance_Items;
                        material_Maintenance_Record.Status_UID = 3;
                        DateTime t1 = DateTime.Parse(material_Maintenance_Record.Delivery_Date.ToString());
                        DateTime t2 = DateTime.Parse(dto.Return_Date.ToString());
                        System.TimeSpan t3 = t2 - t1;  //两个时间相减 。默认得到的是 两个时间之间的天数   得到：365.00:00:00  
                        double getDay = t3.TotalDays; //将这个天数转换成天数, 返回值是double类型的（其实不必转换，因为t3默认就是天数）
                        material_Maintenance_Record.Maintenance_Days = Convert.ToInt32(getDay);
                        //  material_Maintenance_Record.Maintenance_Days= SqlFunctions.DateDiff("Day", material_Maintenance_Record.Delivery_Date, dto.Return_Date) == null ? 0 : (int)SqlFunctions.DateDiff("Day", material_Maintenance_Record.Delivery_Date, dto.Return_Date);
                        material_Maintenance_RecordRepository.Update(material_Maintenance_Record);
                        unitOfWork.Commit();
                    }

                    return "";
                }
            }
            catch (Exception e)
            {
                return "入库操作失败:" + e.Message;
            }
        }

        public string Approve3Material_Maintenance_RecordByUid(int Material_Maintenance_Record_UID, int Useruid)
        {
            try
            {
                var material_Maintenance_Record = material_Maintenance_RecordRepository.GetFirstOrDefault(m => m.Material_Maintenance_Record_UID == Material_Maintenance_Record_UID);
                if (material_Maintenance_Record != null)
                {
                    material_Maintenance_Record.Acceptance_Staff_UID= Useruid;
                    material_Maintenance_Record.Acceptance_Date = DateTime.Now;
                    material_Maintenance_Record.Status_UID = 4;
                    material_Maintenance_RecordRepository.Update(material_Maintenance_Record);
                    unitOfWork.Commit();
                }
                return "";

            }
            catch (Exception e)
            {
                return "入库操作失败:" + e.Message;
            }

        }

        public string DeleteMaterial_Maintenance_Record(int Material_Maintenance_Record_UID)
        {
            return material_Maintenance_RecordRepository.DeleteMaterial_Maintenance_Record(Material_Maintenance_Record_UID);
        }

        public List<Material_Maintenance_RecordDTO> DoAllExportMaterial_Maintenance_Record(Material_Maintenance_RecordDTO search)
        {
            return material_Maintenance_RecordRepository.DoAllExportMaterial_Maintenance_Record(search);
        }
        public List<Material_Maintenance_RecordDTO> DoExportMaterial_Maintenance_Record(string Material_Maintenance_Record_UIDs)
        {
            return material_Maintenance_RecordRepository.DoExportMaterial_Maintenance_Record(Material_Maintenance_Record_UIDs);
        }
    }
}
