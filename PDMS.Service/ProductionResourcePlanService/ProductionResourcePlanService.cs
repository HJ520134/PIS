using PDMS.Common.Constants;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDMS.Service
{
    public class ProductionResourcePlanService : IProductionResourcePlanService
    {
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IProductionResourcePlanRepository PRP_Repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentStaffRepository currentStaffRepository;
        private readonly IDemissionRateAndWorkScheduleRepository demissionRateAndWorkScheduleRepository;

        #region Private interfaces properties
        private readonly IFlowChartDetailRepository flowChartDetailRepository;
        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly ISystemOrgBomRepository systemOrgBomRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly IFlowChartMasterRepository flowChartMasterRepository;
        private readonly ISystemFunctionPlantRepository iSystemFunctionPlantRepository;
        private readonly ISystemUserOrgRepository systemUserOrgRepository;
        private readonly ISystemBUDRepository systemBUDRepository;
        private readonly IRP_Flowchart_MasterRepository rP_Flowchart_MasterRepository;
        private readonly IRP_Flowchart_Detail_ME_EquipmentRepository rP_Flowchart_Detail_ME_EquipmentRepository;
        private readonly IVendorInfoRepository vendorInfoRepository;

        #endregion

        public ProductionResourcePlanService(
            IEnumerationRepository enumerationRepository,
            IProductionResourcePlanRepository ProductionResourcePlanRepository,
            ICurrentStaffRepository currentStaffRepository,
            IDemissionRateAndWorkScheduleRepository demissionRateAndWorkScheduleRepository,
              IUnitOfWork unitOfWork,
              IFlowChartDetailRepository FlowChartDetailRepository,
              ISystemOrgRepository systemOrgRepository,
              ISystemOrgBomRepository systemOrgBomRepository,
              ISystemProjectRepository systemProjectRepository,
              IFlowChartMasterRepository flowChartMasterRepository,
              ISystemFunctionPlantRepository iSystemFunctionPlantRepository,
              ISystemUserOrgRepository systemUserOrgRepository,
              ISystemBUDRepository systemBUDRepository,
              IRP_Flowchart_MasterRepository rP_Flowchart_MasterRepository,
              IRP_Flowchart_Detail_ME_EquipmentRepository rP_Flowchart_Detail_ME_EquipmentRepository,
              IVendorInfoRepository vendorInfoRepository
              )
        {
            this.enumerationRepository = enumerationRepository;
            this.PRP_Repository = ProductionResourcePlanRepository;
            this.currentStaffRepository = currentStaffRepository;
            this.demissionRateAndWorkScheduleRepository = demissionRateAndWorkScheduleRepository;
            this.unitOfWork = unitOfWork;
            this.flowChartDetailRepository = FlowChartDetailRepository;
            this.unitOfWork = unitOfWork;
            this.enumerationRepository = enumerationRepository;
            this.systemOrgRepository = systemOrgRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.flowChartMasterRepository = flowChartMasterRepository;
            this.iSystemFunctionPlantRepository = iSystemFunctionPlantRepository;
            this.systemUserOrgRepository = systemUserOrgRepository;
            this.systemBUDRepository = systemBUDRepository;
            this.rP_Flowchart_MasterRepository = rP_Flowchart_MasterRepository;
            this.rP_Flowchart_Detail_ME_EquipmentRepository = rP_Flowchart_Detail_ME_EquipmentRepository;
            this.vendorInfoRepository = vendorInfoRepository;
        }

        public PagedListModel<DemissionRateAndWorkScheduleDTO> GetDSPlanList(DRAWS_QueryParam searchModel, Page page)
        {
            int totalcount = 0;
            var result = PRP_Repository.GetDSPlanList(searchModel, page, out totalcount);
            var dtoList = AutoMapper.Mapper.Map<IList<DemissionRateAndWorkScheduleDTO>>(result);
            return new PagedListModel<DemissionRateAndWorkScheduleDTO>(totalcount, dtoList);
        }

        #region 现有人力
        public PagedListModel<CurrentStaffDTO> QueryCurrentStaffInfo(CurrentStaffDTO dto, Page page)
        {
            var totalCount = 0;
            var list = currentStaffRepository.QueryCurrentStaffInfo(dto, page, out totalCount);
            return new PagedListModel<CurrentStaffDTO>(totalCount, list);
        }

        public string ImportCurrentStaffInfo(List<CurrentStaffDTO> list)
        {
            List<CurrentStaffDTO> dtoList = AutoMapper.Mapper.Map<List<CurrentStaffDTO>>(list);
            var Plant_Organization_UID = list.First().Plant_Organization_UID;
            var BG_Organization_UID = list.First().BG_Organization_UID;

            //var oldList = currentStaffRepository.GetMany(m => m.Plant_Organization_UID == Plant_Organization_UID
            //&& m.BG_Organization_UID == BG_Organization_UID).ToList();
            //currentStaffRepository.DeleteList(oldList);

            currentStaffRepository.AddList(dtoList);
            unitOfWork.Commit();
            return string.Empty;
        }

        public string CheckImportCurrentStaffExcel(List<CurrentStaffDTO> list)
        {
            var plantUIDList = list.Select(m => m.Plant_Organization_UID).ToList();
            var bgUIDList = list.Select(m => m.BG_Organization_UID).ToList();
            var dateList = list.Select(m => m.ProductDate).ToList();
            int LanguageID = list.Select(m => m.LanguageID).First();

            var hasItem = currentStaffRepository.GetMany(m => plantUIDList.Contains(m.Plant_Organization_UID) && bgUIDList.Contains(m.BG_Organization_UID)
            && dateList.Contains(m.ProductDate)).FirstOrDefault();

            if (hasItem != null)
            {
                //var error = LocalizedLanguageCommon.GetLocaleStringResource(LanguageID, "Common.DataExist");
                var errorInfo = string.Format("已经存在相同的日期：{0}", hasItem.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate));
                return errorInfo;
            }
            return string.Empty;
        }

        public string SaveStaffInfo(CurrentStaffDTO dto)
        {
            var item = currentStaffRepository.GetById(dto.Current_Staff_UID);
            item.OP_Qty = dto.OP_Qty;
            item.Monitor_Staff_Qty = dto.Monitor_Staff_Qty;
            item.Technical_Staff_Qty = dto.Technical_Staff_Qty;
            item.Material_Keeper_Qty = dto.Material_Keeper_Qty;
            item.Others_Qty = dto.Others_Qty;
            unitOfWork.Commit();
            return string.Empty;
        }

        #endregion

        #region 离职率排班维护
        public PagedListModel<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page)
        {
            var totalCount = 0;
            var list = demissionRateAndWorkScheduleRepository.QueryTurnoverSchedulingInfo(searchModel, page, out totalCount);
            return new PagedListModel<DemissionRateAndWorkScheduleDTO>(totalCount, list);
        }

        public List<DemissionRateAndWorkScheduleDTO> ExportDemissionRateInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page)
        {
            var list = demissionRateAndWorkScheduleRepository.ExportDemissionRateList(searchModel);
            return list.OrderByDescending(p => p.Product_Date).ToList();
        }

        public List<DemissionRateAndWorkScheduleDTO> ExportDemissionRateInfoByID(string uids)
        {
            uids = "," + uids + ",";
            DemissionRateAndWorkScheduleDTO searchModel = new DemissionRateAndWorkScheduleDTO();
            var list = demissionRateAndWorkScheduleRepository.ExportDemissionRateList(searchModel);
            var query = list.Where(m => uids.Contains("," + m.DemissionRateAndWorkSchedule_UID + ","));
            return query.OrderByDescending(p => p.Product_Date).ToList();
        }

        public DemissionRateAndWorkScheduleDTO GetDemissionInfoByID(int demissionID)
        {
            var demissionModel = demissionRateAndWorkScheduleRepository.GetDemissionInfoByID(demissionID);
            return demissionModel;
        }

        public bool DeleteDemissionInfoByID(int demissionID)
        {
            var result = demissionRateAndWorkScheduleRepository.DeleteDemissionInfoByID(demissionID);
            return result;
        }
        public List<Enumeration> GetWorkScheduleList()
        {
            var result = demissionRateAndWorkScheduleRepository.GetWorkScheduleList();
            return result;
        }

        public string SaveDemissionInfo(DemissionRateAndWorkScheduleDTO dto)
        {
            try
            {
                if (dto.DemissionRateAndWorkSchedule_UID == 0)
                {
                    var result = demissionRateAndWorkScheduleRepository.IsExistSchedule(dto);
                    if (result)
                    {
                        return "数据已经存在，请修改！";
                    }

                    DemissionRateAndWorkSchedule dtoModel = AutoMapper.Mapper.Map<DemissionRateAndWorkSchedule>(dto);
                    demissionRateAndWorkScheduleRepository.Add(dtoModel);
                    unitOfWork.Commit();
                    return "添加成功";
                }
                else
                {
                    var item = demissionRateAndWorkScheduleRepository.GetById(dto.DemissionRateAndWorkSchedule_UID);
                    item.Product_Date = dto.Product_Date;
                    item.DemissionRate_MP = dto.DemissionRate_MP;
                    item.DemissionRate_NPI = dto.DemissionRate_NPI;
                    item.MP_RecruitStaff_Qty = dto.MP_RecruitStaff_Qty;
                    item.NPI_RecruitStaff_Qty = dto.NPI_RecruitStaff_Qty;
                    item.WorkSchedule = dto.WorkSchedule;
                    unitOfWork.Commit();
                    return "修改成功";
                }
            }
            catch (Exception ex)
            {
                return "保存失败";
            }
        }

        public string CheckImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list)
        {
            var dateList = list.Select(m => m.Product_Date).ToList();
            var hasItem = demissionRateAndWorkScheduleRepository.GetMany(m => dateList.Contains(m.Product_Date)).FirstOrDefault();
            if (hasItem != null)
            {
                return string.Format("已经存在日期：{0}的数据了", hasItem.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
            }
            return string.Empty;
        }

        public string ImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list)
        {
            List<DemissionRateAndWorkSchedule> dtoList = AutoMapper.Mapper.Map<List<DemissionRateAndWorkSchedule>>(list);
            demissionRateAndWorkScheduleRepository.AddList(dtoList);
            unitOfWork.Commit();
            return string.Empty;
        }
        #endregion

        #region 专案
        public ProductionPlanningReportGetProject GetProjectList(CustomUserInfoVM vm)
        {
            ProductionPlanningReportGetProject projectItem = new ProductionPlanningReportGetProject();
            Dictionary<int, string> plantDir = new Dictionary<int, string>();
            Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            Dictionary<int, string> funPlantDir = new Dictionary<int, string>();

            var plantList = systemOrgRepository.GetMany(m => m.Organization_ID.Contains("1000")).OrderBy(m => m.Organization_ID).ToList();
            foreach (var item in plantList)
            {
                plantDir.Add(item.Organization_UID, item.Organization_Desc);
            }

            var id = plantDir.First().Key;
            var childUIDList = systemOrgBomRepository.GetMany(m => m.ParentOrg_UID == id).Select(m => m.ChildOrg_UID).ToList();
            var optypeList = systemOrgRepository.GetMany(m => childUIDList.Contains(m.Organization_UID)).ToList();

            foreach (var item in optypeList)
            {
                opTypeDir.Add(item.Organization_UID, item.Organization_Desc);
            }
            opTypeDir.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);

            var funPlantList = systemOrgRepository.GetChildByParentUID(vm.OPType_OrganizationUIDList.FirstOrDefault());
            foreach (var item in funPlantList)
            {
                funPlantDir.Add(item.Organization_UID, item.Organization_Name);
            }

            projectItem.plantDir = plantDir;
            projectItem.opTypeDir = opTypeDir;
            //projectItem.FunPlantDir = funPlantDir;
            //projectItem.partTypeDir = partTypeDir;
            return projectItem;
        }

        public Dictionary<int, string> GetOpTypesByPlantName(string id)
        {
            Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            var uid = Convert.ToInt32(id);
            var childUIDList = systemOrgBomRepository.GetMany(m => m.ParentOrg_UID == uid).Select(m => m.ChildOrg_UID).ToList();
            var optypeList = systemOrgRepository.GetMany(m => childUIDList.Contains(m.Organization_UID)).ToList();
            foreach (var item in optypeList)
            {
                opTypeDir.Add(item.Organization_UID, item.Organization_Desc);
            }
            opTypeDir.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
            return opTypeDir;
        }

        public Dictionary<int, string> GetProjectByOpType(int OpTypeUID)
        {
            Dictionary<int, string> dirList = new Dictionary<int, string>();
            dirList.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
            var list = systemProjectRepository.GetMany(m => m.Organization_UID == OpTypeUID).ToList();
            list.ForEach(m => dirList.Add(m.Project_UID, m.Project_Name));
            return dirList;
        }

        public Dictionary<int, string> GetPartTypesByProject(int ProjectUID)
        {
            Dictionary<int, string> dirList = new Dictionary<int, string>();
            dirList.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
            var list = flowChartMasterRepository.GetMany(m => m.Project_UID == ProjectUID).ToList();
            list.ForEach(m => dirList.Add(m.FlowChart_Master_UID, m.Part_Types));
            return dirList;
        }

        public Dictionary<int, string> GetFunPlantByOpType(int OpTypeUID)
        {
            Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            //获取三级PP,PE
            var childUIDList = systemOrgBomRepository.GetMany(m => m.ParentOrg_UID == OpTypeUID).Select(m => m.ChildOrg_UID).ToList();
            //根据三级获取四级
            var funcUIDList = systemOrgBomRepository.GetMany(m => childUIDList.Contains(m.ParentOrg_UID.Value)).Select(m => m.ChildOrg_UID).ToList();
            var optypeList = systemOrgRepository.GetMany(m => funcUIDList.Contains(m.Organization_UID)).ToList();
            foreach (var item in optypeList)
            {
                opTypeDir.Add(item.Organization_UID, item.Organization_Name);
            }
            opTypeDir.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
            return opTypeDir;
        }
        #endregion

        #region ME ------- Add By Wesley 2018/04/12
        /// <summary>
        /// 取得ME資料
        /// </summary>
        /// <param name="search">查詢條件集合</param>
        /// <param name="page">分頁參數</param>        
        /// <returns></returns>
        public PagedListModel<RP_ME_VM> QueryMEs(RP_MESearch search, Page page)
        {
            var totalCount = 0;
            var MEs = rP_Flowchart_MasterRepository.QueryMEs(search, page, out totalCount);
            return new PagedListModel<RP_ME_VM>(totalCount, MEs);
        }
        /// <summary>
        /// 取得ME_D資料清單by ME主檔流水號
        /// </summary>
        /// <param name="rP_Flowchart_Master_UID">ME主檔流水號</param>
        /// <returns></returns>
        public List<RP_ME_D> GetME_Ds(int rP_Flowchart_Master_UID)
        {
            var ME_Ds = rP_Flowchart_MasterRepository.GetME_Ds(rP_Flowchart_Master_UID);
            return ME_Ds;
        }

        /// <summary>
        /// 取得ME主檔 Change History
        /// </summary>
        /// <param name="plant_Organization_UID">plant_Organization_UID</param>
        /// <param name="bG_Organization_UID">bG_Organization_UID</param>
        /// <param name="project_UID">project_UID</param>
        /// <returns></returns>
        public List<RP_M> GetME_ChangeHistory(int plant_Organization_UID, int bG_Organization_UID, int project_UID)
        {
            var MEs = rP_Flowchart_MasterRepository.GetME_ChangeHistory(plant_Organization_UID, bG_Organization_UID, project_UID);
            return MEs;
        }

        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment 清單
        /// </summary>
        /// <param name="search">搜尋條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <returns></returns>
        public PagedListModel<RP_ME_D_Equipment> GetME_D_Equipments(ME_EquipmentSearchVM search, Page page)
        {
            var totalCount = 0;
            var ME_D_Equipments = rP_Flowchart_MasterRepository.GetME_D_Equipments(search, page, out totalCount);
            return new PagedListModel<RP_ME_D_Equipment>(totalCount, ME_D_Equipments);
        }
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <returns></returns>
        public RP_ME_D_Equipment GetME_D_Equipment(int rP_Flowchart_Detail_ME_Equipment_UID)
        {
            var rP_Flowchart_Detail_ME_Equipment = rP_Flowchart_MasterRepository.GetME_D_Equipment(rP_Flowchart_Detail_ME_Equipment_UID);
            var dt = AutoMapper.Mapper.Map<RP_ME_D_Equipment>(rP_Flowchart_Detail_ME_Equipment);
            return dt;
        }

        /// <summary>
        /// 保存設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <param name="wquipment_Manpower_Ratio">人力配比</param>
        /// <param name="wQP_Variable_Qty">設備變動數量</param>
        /// <param name="nPI_Current_Qty">NPI當前數量</param>
        /// <returns></returns>
        public MessageStatus SaveME_D_Equipment(SaveME_EquipmentVM vm)
        {
            MessageStatus ms = new MessageStatus(); 
            try
            {
                var dt = rP_Flowchart_Detail_ME_EquipmentRepository.GetById(vm.RP_Flowchart_Detail_ME_Equipment_UID);
                dt.Ratio = vm.Equipment_Manpower_Ratio;
                dt.EQP_Variable_Qty = vm.EQP_Variable_Qty;
                dt.NPI_Current_Qty = vm.NPI_Current_Qty;
                dt.Modified_UID = vm.Account_UID;
                dt.Modified_Date = DateTime.Now;
                rP_Flowchart_Detail_ME_EquipmentRepository.Update(dt);
                unitOfWork.Commit();
                ms.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ms.IsSuccess = false;
                ms.Message = ex.ToString();
            }
            return ms;
        }
        /// <summary>
        /// 匯入ME資料
        /// </summary>
        /// <param name="all_vm">資料集合</param>
        public void ImportFlowchartME(RP_All_VM all_vm)
        {
            rP_Flowchart_MasterRepository.ImportFlowchartME(all_vm);
        }
        /// <summary>
        /// 驗證ME Excel匯入資訊
        /// </summary>
        /// <param name="parasItem">匯入資訊</param>
        /// <returns></returns>
        public string CheckMEIsExists(RP_ME_ExcelImportParas parasItem)
        {
            string isValid = string.Empty;
            var budItem = systemBUDRepository.GetMany(m => m.BU_D_Name == parasItem.BU_D_Name).FirstOrDefault();
            //如果budItem没有，说明不存在这个客户
            if (budItem != null)
            {
                if (parasItem.isEdit)
                {
                    var masterItem = rP_Flowchart_MasterRepository.GetById(parasItem.FlowChart_Master_UID);
                    if (masterItem.System_Project.Project_Name == parasItem.Project_Name && masterItem.System_Project.System_BU_D.BU_D_Name == parasItem.BU_D_Name
                        && masterItem.System_Project.Product_Phase == parasItem.Product_Phase && masterItem.Part_Types == parasItem.Part_Types)
                    {
                        isValid = string.Format("{0}_{1}_{2}_{3}", masterItem.RP_Flowchart_Master_UID, masterItem.Project_UID, masterItem.FlowChart_Version, masterItem.System_Project.Organization_UID);
                    }
                    else
                    {
                        isValid = string.Format("客户{0}，专案名称{1}，部件{2}，阶段{3}不匹配，不能更新", parasItem.BU_D_Name, parasItem.Project_Name, parasItem.Part_Types, parasItem.Product_Phase);
                    }
                }
                else　
                {
                    var projetItem = systemProjectRepository.GetMany(m => m.BU_D_UID == budItem.BU_D_UID && m.Project_Name == parasItem.Project_Name
                    && m.Product_Phase == parasItem.Product_Phase && parasItem.Organization_UIDList.Contains(m.Organization_UID)).FirstOrDefault();
                    //如果projectUIDList没有，说明该数据不存在
                    if (projetItem != null)
                    {
                        //如果flItem为空，说明可以新导入这条数据
                        var flItem = rP_Flowchart_MasterRepository.GetMany(m => m.Project_UID == projetItem.Project_UID && m.Part_Types == parasItem.Part_Types).FirstOrDefault();
                        if (flItem == null)
                        {
                            isValid = string.Format("{0}_{1}", projetItem.Project_UID.ToString(), projetItem.Organization_UID);
                        }
                        else
                        {
                            isValid = string.Format("导入的专案{0}已经存在，不能新增专案", parasItem.Project_Name);
                        }
                    }
                    else
                    {
                        isValid = string.Format("客户{0}或专案名称{1}或阶段{2}不存在，或者用户没有该专案的权限，不能导入", parasItem.BU_D_Name, parasItem.Project_Name, parasItem.Product_Phase);
                    }
                }
            }
            else
            {
                isValid = string.Format("客户名称{0}不存在", parasItem.BU_D_Name);
            }
            return isValid;
        }
        #endregion
    }
}
