using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Model.EntityDTO;
using PDMS.Common.Helpers;

namespace PDMS.Service
{
    public interface IMaterialManageService
    {

        #region 备品备件申请
        //PagedListModel<MaterialManageDTO> QueryMaterialApplyInfo(MaterialManageDTO searchModel, Page page);
        List<MaterialInfoDTO> GetAllMat();
        List<MaterialInfoDTO> QueryMatSingle(int Material_Uid);
        List<SystemUserDTO> GetAllUser();
        List<string> GetAllLocation(int Plant_Organization_UID);
        //string AddOrEditMaterialApply(MaterialManageDTO dto, bool isEdit);
       // List<MaterialManageDTO> QueryMaterialApplySingle(int Material_Apply_Uid);
        List<string> QueryDepartSingle(int Userid);
        //List<MaterialManageDTO> DoExportFunction(string materialid, string materialname, string materialtypes,
        //    string classification);
        #endregion
        #region 仓库管理
        List<EnumerationDTO> GetAllMatLocation();
        #endregion

        #region 料号机型维护-------------------------Robert 2017/03/27
        PagedListModel<EQPPowerOnDTO> QueryEQPPowerOns(EQPPowerOnDTO searchModel, Page page);
        PagedListModel<EQPMaterialDTO> QueryEQPMaterials(EQPMaterialDTO searchModel, Page page);
        PagedListModel<EQPTypeDTO> QueryEQPTypes(EQPTypeDTO searchModel, Page page);

        List< EQPTypeBaseDTO> GetEQPTypes();
        string InsertEQPType(List<EQPTypeDTO> dtolist);
        List<EQPTypeDTO> GetEQPTypeAlls();
        List<EQPTypeDTO> DoAllExportEQPTypeReprot(EQPTypeDTO searchModel);
        List<EQPTypeDTO> DoExportEQPTypeReprot(string uids);

        string AddEQPType(EQP_Type dto);
        void EditEQPType(EQP_Type dto);
        string DelEQPType(EQP_Type dto);
        EQP_Type QueryEQPTypeSingle(int uuid);
        MaterialInfoDTO GetMaterialByMaterialId(string id);
        List<MaterialInfoDTO> GetMaterialByMaterialId();
        string InsertEQPMaterial(List<EQPMaterialDTO> dtolist);
        List<EQPMaterialDTO> QueryEQPMaterialsAll();
        bool AddMaterial(List<EQPMaterialDTO> list);
        bool EditMaterial(List<EQPMaterialDTO> list);
        void DelMaterial(EQP_Material dto);
        List<EQP_Material> QueryMaterialsByEQPId(int id);
        int QueryFunplant(string name, int Op);
        int QueryOP(string OP, int fatherId);
        int QueryEQPTypeSingle(int op, int funplant, string eqpType);
        int QueryForecastPowerOn(int eqpType, DateTime date);
        int QueryPowerOn(int eqpType, DateTime date);
        PagedListModel<EQPForecastPowerOnDTO> QueryForecastPowerOns(EQPForecastPowerOnDTO searchModel, Page page);
        void DelForecastPowerOn(int uuid);
        int EditForecastPowerOn(EQP_Forecast_PowerOn dto);
        int AddForecastPowerOn(EQP_Forecast_PowerOn dto);
        void DelPowerOn(int uuid);
        int EditPowerOn(EQP_PowerOn dto);
        int AddPowerOn(EQP_PowerOn dto);
        bool AddForecastPowerOnList(List<EQPForecastPowerOnDTO> list);
        bool AddPowerOnList(List<EQPPowerOnDTO> list);
        EQPForecastPowerOnDTO QueryForecastPowerOnSingle(int uuid);
        EQPPowerOnDTO QueryEQPPowerOnSingle(int uuid);
        List<EQP_Type> QueryEQPTypeByBgAndFunplant(int bg, int funplant);
        List<EQPMaterialDTO> GetMaterialByEqp(int eqp);
        SystemOrgDTO QueryOrgByName(string name);
        #endregion

        List<SystemOrgBomDTO> GetSystem_OrganizationBOM();
        List<SystemOrgDTO> GetSystem_Organization();
    }
    public class MaterialManageService : IMaterialManageService
    {
        private Logger log = new Logger("MaterialManageService");
       // private readonly IMaterialApplyRepository materialApplyRepository;
        private readonly IMaterialInfoRepository materialInfoRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IEquipmentInfoRepository equipmentInfoRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IEQP_PowerOnRepository eqp_PowerOnRepository;
        private readonly IEQP_MaterialRepository eqp_MaterialRepository;
        private readonly IEQPForecastPowerOnRepository eQPForecastPowerOnRepository;
        private readonly IEQP_TypeRepository eqp_TypeRepository;
        private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;
        private readonly ISystemOrgBomRepository systemOrgBomRepository;
        private readonly ISystemOrgRepository systemOrgRepository;

        public MaterialManageService(//IMaterialApplyRepository materialApplyRepository,
            IMaterialInfoRepository materialInfoRepository,
            ISystemUserRepository systemUserRepository,
            IEquipmentInfoRepository equipmentInfoRepository, IUnitOfWork unitOfWork,
            IEnumerationRepository enumerationRepository, IEQP_PowerOnRepository eqp_PowerOnRepository, IEQP_MaterialRepository eqp_MaterialRepository, IEQP_TypeRepository eqp_TypeRepository, ISystemFunctionPlantRepository systemFunctionPlantRepository, ISystemOrgBomRepository systemOrgBomRepository, IEQPForecastPowerOnRepository eQPForecastPowerOnRepository, ISystemOrgRepository systemOrgRepository)
        {
           // this.materialApplyRepository = materialApplyRepository;
            this.materialInfoRepository = materialInfoRepository;
            this.systemUserRepository = systemUserRepository;
            this.equipmentInfoRepository = equipmentInfoRepository;
            this.unitOfWork = unitOfWork;
            this.enumerationRepository = enumerationRepository;
            this.eqp_PowerOnRepository = eqp_PowerOnRepository;
            this.eqp_MaterialRepository = eqp_MaterialRepository;
            this.eqp_TypeRepository = eqp_TypeRepository;
            this.systemFunctionPlantRepository = systemFunctionPlantRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
            this.eQPForecastPowerOnRepository = eQPForecastPowerOnRepository;
            this.systemOrgRepository = systemOrgRepository;
        }

        #region 备品备件申请
        //public PagedListModel<MaterialManageDTO> QueryMaterialApplyInfo(MaterialManageDTO searchModel, Page page)
        //{
        //    var totalcount = 0;
        //    var result = materialApplyRepository.GetInfo(searchModel, page, out totalcount);

        //    return new PagedListModel<MaterialManageDTO>(totalcount, result);
        //}
        public List<MaterialInfoDTO> GetAllMat()
        {
            List<MaterialInfoDTO> result = new List<MaterialInfoDTO>();

            var matlist = materialInfoRepository.GetAll().ToList();
            foreach (var item in matlist)
            {
                result.Add(AutoMapper.Mapper.Map<MaterialInfoDTO>(item));
            }
            return result;
        }

        public List<MaterialInfoDTO> QueryMatSingle(int Material_Uid)
        {
            var bud = materialInfoRepository.GetByUId(Material_Uid);
            return bud;
        }

        public List<SystemUserDTO> GetAllUser()
        {
            List<SystemUserDTO> result = new List<SystemUserDTO>();

            var userlist = systemUserRepository.GetAll().ToList();
            foreach (var item in userlist)
            {
                result.Add(AutoMapper.Mapper.Map<SystemUserDTO>(item));
            }
            return result;
        }

        public List<string> GetAllLocation(int Plant_Organization_UID)
        {
            List<string> result = new List<string>();

            var locationlist = equipmentInfoRepository.GetDistinctLocation( Plant_Organization_UID);
            return locationlist;
        }

        //public string AddOrEditMaterialApply(MaterialManageDTO dto, bool isEdit)
        //{
        //    try
        //    {
        //        Material_Apply item = new Material_Apply();
        //        item.Material_Uid = dto.Material_Uid;
        //        item.Apply_Type = dto.Apply_Type;
        //        item.Status = dto.Status;
        //        item.Apply_Reason = dto.Apply_Reason;
        //        item.Apply_Number = dto.Apply_Number;
        //        item.Used_Location = dto.Used_Location;
        //        item.Need_Date = dto.Need_Date;
        //        item.Applyer = dto.Applyer;
        //        item.Apply_Date = DateTime.Now;
        //        item.Acceprter = dto.Acceprter;
        //        item.Reason_Detail = dto.Reason_Detail;

        //        if (!isEdit)
        //            materialApplyRepository.Add(item);
        //        else
        //        {
        //            item.Material_Apply_Uid = dto.Material_Apply_Uid;
        //            materialApplyRepository.Update(item);
        //        }
        //        unitOfWork.Commit();
        //        return string.Empty;
        //    }
        //    catch (Exception e)
        //    {

        //        return "保存领料申请单失败！\n" + e.Message;
        //    }
        //}

        //public List<MaterialManageDTO> QueryMaterialApplySingle(int Material_Apply_Uid)
        //{
        //    var bud = materialApplyRepository.GetByUId(Material_Apply_Uid);
        //    return bud;
        //}

        public List<string> QueryDepartSingle(int Userid)
        {
            var bud = materialInfoRepository.GetDepart(Userid);
            return bud;
        }

        //public List<MaterialManageDTO> DoExportFunction(string materialid, string materialname, string materialtypes,
        //    string classification)
        //{
        //    return materialApplyRepository.DoExportFunction(materialid, materialname, materialtypes, classification);
        //}

        #endregion

        #region 仓库管理

        public List<EnumerationDTO> GetAllMatLocation()
        {
            List<EnumerationDTO> result = new List<EnumerationDTO>();

            var enumerlist = enumerationRepository.GetMany(m => m.Enum_Type == "Mat").ToList();
            foreach (var item in enumerlist)
            {
                result.Add(AutoMapper.Mapper.Map<EnumerationDTO>(item));
            }
            return result;
        }
        #endregion


        #region 机台类型-------------------------Robert 2017/03/27
        public PagedListModel<EQPPowerOnDTO> QueryEQPPowerOns(EQPPowerOnDTO searchModel, Page page)
        {
            var totalCount = 0;
            var f = eqp_PowerOnRepository.QueryEQPPowerOns(searchModel, page, out totalCount).ToList();
            return new PagedListModel<EQPPowerOnDTO>(totalCount, f);
        }

        public List<EQPTypeBaseDTO> GetEQPTypes()
        {
            return eqp_TypeRepository.GetEQPTypes();
        }
        public List<EQPTypeDTO> GetEQPTypeAlls()
        { 
        
        return eqp_TypeRepository.GetEQPTypeAlls();
        }
        public PagedListModel<EQPForecastPowerOnDTO> QueryForecastPowerOns(EQPForecastPowerOnDTO searchModel, Page page)
        {
            var totalCount = 0;
            var f = eQPForecastPowerOnRepository.QueryEQPForecastPowerOns(searchModel, page, out totalCount).ToList();
            return new PagedListModel<EQPForecastPowerOnDTO>(totalCount, f);
        }

        public PagedListModel<EQPMaterialDTO> QueryEQPMaterials(EQPMaterialDTO searchModel, Page page)
        {
            var totalCount = 0;
            var f = eqp_MaterialRepository.QueryEQPMaterials(searchModel, page, out totalCount).ToList();
            return new PagedListModel<EQPMaterialDTO>(totalCount, f);
        }

        public PagedListModel<EQPTypeDTO> QueryEQPTypes(EQPTypeDTO searchModel, Page page)
        {
            var totalCount = 0;
            var f = eqp_TypeRepository.QueryEQPTypes(searchModel, page, out totalCount).ToList();
            return new PagedListModel<EQPTypeDTO>(totalCount, f);
        }
        public List<EQPTypeDTO> DoAllExportEQPTypeReprot(EQPTypeDTO searchModel)
        {
            return eqp_TypeRepository.DoAllExportEQPTypeReprot(searchModel);
        }
        public List<EQPTypeDTO> DoExportEQPTypeReprot(string uids)
        {
            return eqp_TypeRepository.DoExportEQPTypeReprot(uids);
        }
        public string AddEQPType(EQP_Type dto)
        {
            try
            {
                eqp_TypeRepository.Add(dto);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "机台类型不能重复添加！";
            }
        }

        public void EditEQPType(EQP_Type dto)
        {
            try
            {
                eqp_TypeRepository.Update(dto);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public string DelEQPType(EQP_Type dto)
        {
            try
            {
                eqp_TypeRepository.Delete(dto);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception ex)
            {
                return "此类型在使用中，不能删除！";
               // log.Error(ex);
            }
        }

        public EQP_Type QueryEQPTypeSingle(int uuid)
        {
            try
            {
                return eqp_TypeRepository.GetById(uuid);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
             
            }
        }

        public List<EQP_Type> QueryEQPTypeByBgAndFunplant(int bg, int funplant)
        {
            try
            {
                return eqp_TypeRepository.GetAll().Where(a => a.BG_Organization_UID == bg && a.FunPlant_Organization_UID == funplant && a.Is_Enable == true).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
              
            }
        }

        public int QueryEQPTypeSingle(int op, int funplant, string eqpType)
        {
            try
            {
                var f = eqp_TypeRepository.GetAll().Where(a => a.BG_Organization_UID == op && a.FunPlant_Organization_UID == funplant && a.EQP_Type1 == eqpType).FirstOrDefault();
                if (f != null && f.EQP_Type_UID > 0)
                {
                    return f.EQP_Type_UID;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
                log.Error(ex);
            }
        }
        #endregion

        #region 机型料号-------------------------Robert 2017/03/27
        public MaterialInfoDTO GetMaterialByMaterialId(string id)
        {
            return materialInfoRepository.GetMaterialByMaterialId(id);
        }
        public List<MaterialInfoDTO> GetMaterialByMaterialId()
        {

            return materialInfoRepository.GetMaterialByMaterialId();
        }

        public string InsertEQPMaterial(List<EQPMaterialDTO> dtolist)
        {
            return eqp_MaterialRepository.InsertEQPMaterial(dtolist);
        }

        public List<EQPMaterialDTO> QueryEQPMaterialsAll()
        {
            return eqp_MaterialRepository.QueryEQPMaterialsAll();
        }
        public List<EQPMaterialDTO> GetMaterialByEqp(int eqp)
        {
            return eqp_MaterialRepository.QueryEQPMaterialsByEqpID(eqp);
        }

        public bool AddMaterial(List<EQPMaterialDTO> list)
        {
            try
            {
                List<EQP_Material> rList = AutoMapper.Mapper.Map<List<EQP_Material>>(list);
                eqp_MaterialRepository.AddList(rList);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }

        }
        public string InsertEQPType(List<EQPTypeDTO> dtolist)
        {
            return eqp_MaterialRepository.InsertItem(dtolist);
        }
        public bool EditMaterial(List<EQPMaterialDTO> list)
        {
            try
            {
                int id = list[0].EQP_Type_UID;
                var f = eqp_MaterialRepository.GetAll().Where(p => p.EQP_Type_UID == id).ToList();
                eqp_MaterialRepository.DeleteList(f);
                unitOfWork.Commit();
                List<EQP_Material> rList = AutoMapper.Mapper.Map<List<EQP_Material>>(list);
                eqp_MaterialRepository.AddList(rList);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }

        }

        public void DelMaterial(EQP_Material dto)
        {
            try
            {
                eqp_MaterialRepository.Delete(dto);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public List<EQP_Material> QueryMaterialsByEQPId(int id)
        {
            return eqp_MaterialRepository.GetAll().Where(p => p.EQP_Type_UID == id).ToList();
        }

        public int QueryOP(string OP, int fatherId)
        {
            var query = systemOrgBomRepository.GetChildOrgID(fatherId);
            var OpVM = query.Where(a => a.Child_Organization_Name == OP).FirstOrDefault();
            if (OpVM != null && OpVM.ChildOrg_UID > 0)
            {
                return OpVM.ChildOrg_UID;
            }
            else
            {
                return 0;
            }
        }

        public int QueryFunplant(string name, int Op)
        {
            var f = systemFunctionPlantRepository.GetAll().Where(a => a.OPType_OrganizationUID == Op && a.FunPlant == name).FirstOrDefault();
            if (f != null && f.FunPlant_OrganizationUID > 0)
            {
                return f.FunPlant_OrganizationUID.Value;
            }
            else
            {
                return 0;
            }
        }

        public int QueryPowerOn(int eqpType, DateTime date)
        {
            var f = eqp_PowerOnRepository.GetAll().Where(a => a.EQP_Type_UID == eqpType && a.PowerOn_Date == date).FirstOrDefault();
            if (f != null && f.EQP_PowerOn_UID > 0)
            {
                return f.EQP_PowerOn_UID;
            }
            else
            {
                return 0;
            }
        }

        public int QueryForecastPowerOn(int eqpType, DateTime date)
        {
            var f = eQPForecastPowerOnRepository.GetAll().Where(a => a.EQP_Type_UID == eqpType && a.Demand_Date == date).FirstOrDefault();
            if (f != null && f.EQP_Forecast_PowerOn_UID > 0)
            {
                return f.EQP_Forecast_PowerOn_UID;
            }
            else
            {
                return 0;
            }
        }

        public bool AddPowerOnList(List<EQPPowerOnDTO> list)
        {
            try
            {
                List<EQP_PowerOn> rList = AutoMapper.Mapper.Map<List<EQP_PowerOn>>(list);
                eqp_PowerOnRepository.AddList(rList);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        public bool AddForecastPowerOnList(List<EQPForecastPowerOnDTO> list)
        {
            try
            {
                List<EQP_Forecast_PowerOn> rList = AutoMapper.Mapper.Map<List<EQP_Forecast_PowerOn>>(list);
                eQPForecastPowerOnRepository.AddList(rList);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        public int AddPowerOn(EQP_PowerOn dto)
        {
            try
            {
                int i = QueryPowerOn(dto.EQP_Type_UID, dto.PowerOn_Date);
                if (i > 0)
                {
                    return i;
                }
                else
                {
                    eqp_PowerOnRepository.Add(dto);
                    unitOfWork.Commit();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return -1;
            }
        }

        public int EditPowerOn(EQP_PowerOn dto)
        {
            try
            {

                eqp_PowerOnRepository.Update(dto);
                unitOfWork.Commit();
                return 0;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return -1;
            }
        }

        public void DelPowerOn(int uuid)
        {
            try
            {
                var dto = eqp_PowerOnRepository.GetById(uuid);
                eqp_PowerOnRepository.Delete(dto);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }


        public int AddForecastPowerOn(EQP_Forecast_PowerOn dto)
        {
            try
            {
                int i = QueryForecastPowerOn(dto.EQP_Type_UID, dto.Demand_Date);
                if (i > 0)
                {
                    return i;
                }
                else
                {
                    eQPForecastPowerOnRepository.Add(dto);
                    unitOfWork.Commit();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return -1;
            }
        }

        public int EditForecastPowerOn(EQP_Forecast_PowerOn dto)
        {
            try
            {

                eQPForecastPowerOnRepository.Update(dto);
                unitOfWork.Commit();
                return 0;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return -1;
            }
        }

        public void DelForecastPowerOn(int uuid)
        {
            try
            {
                var dto = eQPForecastPowerOnRepository.GetById(uuid);
                eQPForecastPowerOnRepository.Delete(dto);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }



        public EQPPowerOnDTO QueryEQPPowerOnSingle(int uuid)
        {
            try
            {
                return eqp_PowerOnRepository.QueryEQPPowerOnByPowerId(uuid);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
               
            }
        }

        public EQPForecastPowerOnDTO QueryForecastPowerOnSingle(int uuid)
        {
            try
            {
                return eQPForecastPowerOnRepository.QueryEQPForecastPowerOnByPowerId(uuid);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            
            }
        }

        public SystemOrgDTO QueryOrgByName(string name)
        {
            var f = new SystemOrgDTO();
            f = AutoMapper.Mapper.Map<SystemOrgDTO>(systemOrgRepository.QueryOrgByName(name));
            return f;
        }
        #endregion
      
        public List<SystemOrgDTO> GetSystem_Organization()
        {

            return systemOrgRepository.GetSystem_Organization();

        }
        public List<SystemOrgBomDTO> GetSystem_OrganizationBOM()
        {
            return systemOrgRepository.GetSystem_OrganizationBOM();
        }
    }
}
