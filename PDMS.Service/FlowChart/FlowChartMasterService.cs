using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Repository;
using PDMS.Model;
using System.Transactions;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model.ViewModels;

namespace PDMS.Service.FlowChart
{
    public interface IFlowChartMasterService
    {
        PagedListModel<FlowChartModelGet> QueryFlowCharts(FlowChartModelSearch searchModel, Page page);

        /// <summary>
        /// 获取WIP修改记录详情
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        PagedListModel<WIPAlterDetialModel> GetWIPAlterRecordDetialAPI(WIPDetialSearchParam searchModel, Page page);

        string CheckFlowChart(FlowChartExcelImportParas parasItem);
        List<SystemFunctionPlantDTO> QueryAllFunctionPlants(int optypeUID);
        void ImportFlowChart(FlowChartImport importItem, int accountID);
        void ImportFlowChartWUXI_M(FlowChartImport importItem, int accountID);
        void ImportFlowUpdateChart(FlowChartImport importItem, int accountID);
        void ImportFlowUpdateChartWUXI_M(FlowChartImport importItem, int accountID);
        FlowChartGet QueryFlowChart(int FlowChart_Master_UID);
        FlowChartExcelExport ExportFlowChart(int FlowChart_Master_UID, int Version);
        FlowChartExcelExport ExportFlowWUXI_MChart(int FlowChart_Master_UID, int Version);

        List<FlowChartHistoryGet> QueryHistoryFlowChart(int FlowChart_Master_UID);
        bool ClosedFL(int id, bool isClosed);
        FlowChartDetailGetByMasterInfo QueryFunPlant(int id);
        void ImportFlowChartMGData(List<FlowChartMgDataDTO> mgDataList, int FlowChart_Master_UID);
        void ImportFlowChartIEMGData(List<FlowChartIEMgDataDTO> mgDataList, int FlowChart_Master_UID);
        FlowChartMasterDTO GetMasterItemBySearchCondition(PPCheckDataSearch searchModel);

        int GetFlowChartMasterID(string BU_D_Name, string Project_Name, string Part_Types, string Product_Phase);
    }

    public class FlowChartMasterService : IFlowChartMasterService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly IFlowChartMasterRepository flowChartMasterRepository;
        private readonly IFlowChartDetailRepository flowChartDetailRepository;
        private readonly IFlowChartMgDataRepository flowChartMgDataRepository;
        private readonly ISystemBUDRepository systemBUDRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository;
        private readonly ISystemUserRoleRepository systemUserRoleRepository;
        private readonly ISystemRoleRepository systemRoleRepository;
        private readonly ISystemUserOrgRepository systemUserOrgRepository;
        private readonly IProjectUsersGroupRepository projectUsersGroupRepository;
        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly ISystemOrgBomRepository systemOrgBomRepository;
        private readonly IQualityAssuranceMgDataRepository qualityAssuranceMgDataRepository;
        private readonly IQualityAssuranceInputMasterRepository qualityAssuranceInputMasterRepository;
        private readonly IQualityAssurance_OQC_InputMasterRepository oQC_InputMasterRepository;
        private readonly IQualityAssuranceInputMasterHistoryRepository qualityAssuranceInputMasterHistoryRepository;
        private readonly IOQCInputMasterHistoryRepository oQCInputMasterHistoryRepository;
        private readonly IExceptionTypeWithFlowchartRepository exceptionTypeWithFlowchartRepository;
        #endregion //Private interfaces properties
        public delegate string CallSaveFL(FlowChartDetailDTO dto, int AccountID);



        #region Service constructor
        public FlowChartMasterService(
            IFlowChartMasterRepository flowChartMasterRepository,
            IFlowChartDetailRepository flowChartDetailRepository,
            IFlowChartMgDataRepository flowChartMgDataRepository,
            ISystemBUDRepository systemBUDRepository,
            ISystemProjectRepository systemProjectRepository,
            ISystemFunctionPlantRepository systemFunctionPlantRepository,
            ISystemUserRepository systemUserRepository,
            ISystemUserRoleRepository systemUserRoleRepository,
            IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository,
            ISystemRoleRepository systemRoleRepository,
            ISystemUserOrgRepository systemUserOrgRepository,
            IProjectUsersGroupRepository projectUsersGroupRepository,
            ISystemOrgRepository systemOrgRepository,
            ISystemOrgBomRepository systemOrgBomRepository,
            IQualityAssuranceMgDataRepository qualityAssuranceMgDataRepository,
            IQualityAssuranceInputMasterRepository qualityAssuranceInputMasterRepository,
            IQualityAssurance_OQC_InputMasterRepository oQC_InputMasterRepository,
            IQualityAssuranceInputMasterHistoryRepository qualityAssuranceInputMasterHistoryRepository,
            IOQCInputMasterHistoryRepository oQCInputMasterHistoryRepository,
            IExceptionTypeWithFlowchartRepository exceptionTypeWithFlowchartRepository,
        IUnitOfWork unitOfWork)
        {
            this.systemRoleRepository = systemRoleRepository;
            this.unitOfWork = unitOfWork;
            this.flowChartMasterRepository = flowChartMasterRepository;
            this.flowChartDetailRepository = flowChartDetailRepository;
            this.flowChartMgDataRepository = flowChartMgDataRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.systemBUDRepository = systemBUDRepository;
            this.systemFunctionPlantRepository = systemFunctionPlantRepository;
            this.systemUserRepository = systemUserRepository;
            this.systemUserRoleRepository = systemUserRoleRepository;
            this.flowChartPCMHRelationshipRepository = flowChartPCMHRelationshipRepository;
            this.systemUserOrgRepository = systemUserOrgRepository;
            this.projectUsersGroupRepository = projectUsersGroupRepository;
            this.systemOrgRepository = systemOrgRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
            this.qualityAssuranceMgDataRepository = qualityAssuranceMgDataRepository;
            this.qualityAssuranceInputMasterRepository = qualityAssuranceInputMasterRepository;
            this.oQC_InputMasterRepository = oQC_InputMasterRepository;
            this.qualityAssuranceInputMasterHistoryRepository = qualityAssuranceInputMasterHistoryRepository;
            this.oQCInputMasterHistoryRepository = oQCInputMasterHistoryRepository;
            this.exceptionTypeWithFlowchartRepository = exceptionTypeWithFlowchartRepository;
        }
        #endregion //Service constructor

        public PagedListModel<FlowChartModelGet> QueryFlowCharts(FlowChartModelSearch searchModel, Page page)
        {
            var totalCount = 0;
            var flChartList = flowChartMasterRepository.QueryFlowCharts(searchModel, page, out totalCount);
            return new PagedListModel<FlowChartModelGet>(totalCount, flChartList.ToList());
        }

        /// <summary>
        /// 获取WIP修改记录详情
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<WIPAlterDetialModel> GetWIPAlterRecordDetialAPI(WIPDetialSearchParam searchModel, Page page)
        {
            var totalCount = 0;
            var wipdetialData = flowChartMasterRepository.GetWIPAlterRecordDetialData(searchModel, page);
            totalCount = wipdetialData.Count;
            wipdetialData = wipdetialData.Skip(page.Skip).Take(page.PageSize).ToList();

            wipdetialData.ForEach(p =>
               {
                   p.alterTime = p.Modified_Date.ToString("yyyy-MM-dd");
                   p.WIP_Add = p.WIP_Old + p.WIP_Add;
               }
             );
            return new PagedListModel<WIPAlterDetialModel>(totalCount, wipdetialData);
        }

        private FlowChartGet SetAutoMapFlChart(FlowChart_Master item)
        {
            FlowChartGet model = new FlowChartGet();
            model.FlowChartMasterDTO = AutoMapper.Mapper.Map<FlowChartMasterDTO>(item);
            model.SystemProjectDTO = AutoMapper.Mapper.Map<SystemProjectDTO>(item.System_Project);
            model.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users);
            model.BU_D_Name = item.System_Project.System_BU_D.BU_D_Name;
            return model;
        }



        public string CheckFlowChart(FlowChartExcelImportParas parasItem)
        {
            string isValid = string.Empty;
            var budItem = systemBUDRepository.GetMany(m => m.BU_D_Name == parasItem.BU_D_Name).FirstOrDefault();
            //如果budItem没有，说明不存在这个客户
            if (budItem != null)
            {
                if (parasItem.isEdit)
                {
                    var masterItem = flowChartMasterRepository.GetById(parasItem.FlowChart_Master_UID);
                    if (masterItem.System_Project.Project_Name == parasItem.Project_Name && masterItem.System_Project.System_BU_D.BU_D_Name == parasItem.BU_D_Name
                        && masterItem.System_Project.Product_Phase == parasItem.Product_Phase && masterItem.Part_Types == parasItem.Part_Types)
                    {
                        isValid = string.Format("{0}_{1}_{2}_{3}", masterItem.FlowChart_Master_UID, masterItem.Project_UID, masterItem.FlowChart_Version, masterItem.System_Project.Organization_UID);
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
                        var flItem = flowChartMasterRepository.GetMany(m => m.Project_UID == projetItem.Project_UID && m.Part_Types == parasItem.Part_Types).FirstOrDefault();
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

        public List<SystemFunctionPlantDTO> QueryAllFunctionPlants(int optypeUID)
        {
            ////获取orgid
            //var orgId = systemOrgRepository.GetMany(m => m.Organization_Name == site).Select(m => m.Organization_UID).First();
            ////通过orgid获取二级组织的optypes
            //var orgIdList = systemOrgBomRepository.GetMany(m => m.ParentOrg_UID == orgId).Select(m => m.ChildOrg_UID).ToList();
            ////通过二级optypes查询对应的值
            //var opTypesList = systemOrgRepository.GetMany(m => orgIdList.Contains(m.Organization_UID)).Select(m => m.Organization_Name).ToList();

            var functionPlants = systemFunctionPlantRepository.GetMany(m => m.OPType_OrganizationUID == optypeUID).ToList();
            List<SystemFunctionPlantDTO> dtoList = new List<SystemFunctionPlantDTO>();
            foreach (var item in functionPlants)
            {
                dtoList.Add(AutoMapper.Mapper.Map<SystemFunctionPlantDTO>(item));
            }
            return dtoList;
        }

        public void ImportFlowChart(FlowChartImport importItem, int accountID)
        {
            flowChartMasterRepository.ImportFlowChart(importItem, accountID);

            //using (TransactionScope scope = new TransactionScope())
            //{
            //    FlowChart_Master flMasterItem = new FlowChart_Master();
            //    flMasterItem = AutoMapper.Mapper.Map<FlowChart_Master>(importItem.FlowChartMasterDTO);
            //    flMasterItem.CurrentDepartent = "PP";
            //    flowChartMasterRepository.Add(flMasterItem);

            //    List<FLUIDAndBindSeq> FlSeqList = new List<FLUIDAndBindSeq>();
            //    foreach (var detailDTOItem in importItem.FlowChartImportDetailDTOList)
            //    {
            //        var detailItem = AutoMapper.Mapper.Map<FlowChart_Detail>(detailDTOItem.FlowChartDetailDTO);
            //        detailItem.EndTime = null;
            //        flMasterItem.FlowChart_Detail.Add(detailItem);

            //    }
            //    unitOfWork.Commit();
            //    scope.Complete();
            //}
        }

        public void ImportFlowChartWUXI_M(FlowChartImport importItem, int accountID)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    FlowChart_Master flMasterItem = new FlowChart_Master();
                    flMasterItem = AutoMapper.Mapper.Map<FlowChart_Master>(importItem.FlowChartMasterDTO);
                    flowChartMasterRepository.Add(flMasterItem);

                    foreach (var detailDTOItem in importItem.FlowChartImportDetailDTOList)
                    {
                        var detailItem = AutoMapper.Mapper.Map<FlowChart_Detail>(detailDTOItem.FlowChartDetailDTO);
                        flMasterItem.FlowChart_Detail.Add(detailItem);

                        //var mgDataItem = AutoMapper.Mapper.Map<FlowChart_MgData>(detailDTOItem.FlowChartMgDataDTO);
                        //detailItem.FlowChart_MgData.Add(mgDataItem);
                    }
                    unitOfWork.Commit();
                    //将数据插入到中间表
                    flowChartMasterRepository.ExecSPTemp(importItem);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ImportFlowUpdateChart(FlowChartImport importItem, int accountID)
        {
            flowChartMasterRepository.UpdateFolowCharts(importItem, accountID);
        }

        public void ImportFlowUpdateChartWUXI_M(FlowChartImport importItem, int accountID)
        {
            flowChartMasterRepository.UpdateFolowChartsWUXI_M(importItem, accountID);
        }

        public FlowChartGet QueryFlowChart(int FlowChart_Master_UID)
        {
            var flItem = flowChartMasterRepository.GetById(FlowChart_Master_UID);
            var item = SetAutoMapFlChart(flItem);
            return item;
        }

        public List<FlowChartHistoryGet> QueryHistoryFlowChart(int FlowChart_Master_UID)
        {

            List<FlowChartHistoryGet> historyList = new List<FlowChartHistoryGet>();
            historyList = flowChartMasterRepository.QueryHistoryVersion(FlowChart_Master_UID);
            return historyList;
        }

        public FlowChartExcelExport ExportFlowChart(int FlowChart_Master_UID, int Version)
        {
            FlowChartExcelExport exportItem = new FlowChartExcelExport();

            var flMasterItem = flowChartMasterRepository.GetById(FlowChart_Master_UID);
            //var flmasterVersion = flMasterItem.FlowChart_Version;
            var flmasterVersion = Version;
            //flMasterItem.FlowChart_Detail = flMasterItem.FlowChart_Detail.Where(m => m.FlowChart_Version == flmasterVersion).ToList();

            List<FlowChartDetailAndMGDataDTO> detailList = new List<FlowChartDetailAndMGDataDTO>();

            if (flMasterItem != null)
            {
                exportItem.BU_D_Name = flMasterItem.System_Project.System_BU_D.BU_D_Name;
                exportItem.Project_Name = flMasterItem.System_Project.Project_Name;
                exportItem.Part_Types = flMasterItem.Part_Types;
                exportItem.Product_Phase = flMasterItem.System_Project.Product_Phase;

                exportItem.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(flMasterItem.System_Users);
                //exportItem.FlowChartDetailAndMGDataDTOList = detailList;
                exportItem.FlowChartDetailAndMGDataDTOList = flowChartDetailRepository.QueryExportDetailList(flMasterItem.FlowChart_Master_UID, flMasterItem.FlowChart_Version);
            }

            return exportItem;
        }

        public FlowChartExcelExport ExportFlowWUXI_MChart(int FlowChart_Master_UID, int Version)
        {
            FlowChartExcelExport exportItem = new FlowChartExcelExport();

            var flMasterItem = flowChartMasterRepository.GetById(FlowChart_Master_UID);
            //var flmasterVersion = flMasterItem.FlowChart_Version;
            var flmasterVersion = Version;
            //flMasterItem.FlowChart_Detail = flMasterItem.FlowChart_Detail.Where(m => m.FlowChart_Version == flmasterVersion).ToList();

            List<FlowChartDetailAndMGDataDTO> detailList = new List<FlowChartDetailAndMGDataDTO>();

            if (flMasterItem != null)
            {
                exportItem.BU_D_Name = flMasterItem.System_Project.System_BU_D.BU_D_Name;
                exportItem.Project_Name = flMasterItem.System_Project.Project_Name;
                exportItem.Part_Types = flMasterItem.Part_Types;
                exportItem.Product_Phase = flMasterItem.System_Project.Product_Phase;

                exportItem.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(flMasterItem.System_Users);
                //exportItem.FlowChartDetailAndMGDataDTOList = detailList;
                exportItem.FlowChartDetailAndMGDataDTOList = flowChartDetailRepository.QueryExportWUXI_MDetailList(flMasterItem.FlowChart_Master_UID, flMasterItem.FlowChart_Version);
            }

            return exportItem;
        }





        public bool ClosedFL(int id, bool isClosed)
        {
            var item = flowChartMasterRepository.GetById(id);
            if (isClosed)
            {
                item.Is_Closed = true;
            }
            else
            {
                item.Is_Closed = false;
            }
            flowChartMasterRepository.Update(item);
            unitOfWork.Commit();
            return true;
        }


        public FlowChartDetailGetByMasterInfo QueryFunPlant(int id)
        {
            var masterItem = flowChartMasterRepository.GetById(id);

            FlowChartDetailGetByMasterInfo detailInfo = new FlowChartDetailGetByMasterInfo();
            detailInfo.BU_D_Name = masterItem.System_Project.System_BU_D.BU_D_Name;
            detailInfo.Project_Name = masterItem.System_Project.Project_Name;
            detailInfo.Part_Types = masterItem.Part_Types;
            detailInfo.Product_Phase = masterItem.System_Project.Product_Phase;


            //var list = systemUserOrgRepository.GetChildFunPlant(orgUID);
            //var dtoList = AutoMapper.Mapper.Map<List<SystemOrgDTO>>(list);
            ////为功能厂加上OP类型，以区分不同OP类型相同功能厂
            //foreach (var item in dtoList)
            //{
            //    item.Organization_Name = optype + "_" + item.Organization_Name;
            //}
            //detailInfo.SystemOrgDTOList = dtoList;
            List<SystemFunctionPlantDTO> dtoList = new List<SystemFunctionPlantDTO>();
            var projectItem = systemProjectRepository.GetById(masterItem.Project_UID);
            var list = systemFunctionPlantRepository.GetMany(m => m.OPType_OrganizationUID == projectItem.Organization_UID).OrderBy(m => m.FunPlant).ToList();
            dtoList = AutoMapper.Mapper.Map<List<SystemFunctionPlantDTO>>(list);
            //为功能厂加上OP类型，以区分不同OP类型相同功能厂
            foreach (var item in dtoList)
            {
                item.FunPlant += "_" + item.OP_Types;
            }
            detailInfo.SystemFunctionPlantDTOList = dtoList;
            return detailInfo;
        }


        public void ImportFlowChartMGData(List<FlowChartMgDataDTO> mgDataList, int FlowChart_Master_UID)
        {
            flowChartMasterRepository.BatchImportPlan(mgDataList, FlowChart_Master_UID);
        }

        public void ImportFlowChartIEMGData(List<FlowChartIEMgDataDTO> mgDataList, int FlowChart_Master_UID)
        {
            flowChartMasterRepository.BatchIEImportPlan(mgDataList, FlowChart_Master_UID);
        }


        #region 根据查询条件获取Flowchart信息
        public FlowChartMasterDTO GetMasterItemBySearchCondition(PPCheckDataSearch searchModel)
        {
            var item = flowChartMasterRepository.GetMany(m => m.System_Project.Project_Name == searchModel.Project && m.Part_Types == searchModel.Part_Types
            && m.Is_Closed == false).FirstOrDefault();
            if (item != null)
            {
                var dto = AutoMapper.Mapper.Map<FlowChartMasterDTO>(item);
                return dto;
            }
            else
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 获取flowCahrtMasterID
        /// </summary>
        /// <param name="BU_D_Name"></param>
        /// <param name="Project_Name"></param>
        /// <param name="Part_Types"></param>
        /// <param name="Product_Phase"></param>
        /// <returns></returns>
        public int GetFlowChartMasterID(string BU_D_Name, string Project_Name, string Part_Types, string Product_Phase)
        {
            try
            {
                var item = flowChartMasterRepository.GetFlowChartMasterID(BU_D_Name, Project_Name, Part_Types, Product_Phase);
                return item;
            }
            catch (Exception ex)
            {
                return 0;
            }
          
        }

        public void ImportFlowChartIEMGData(List<FlowChartMgDataDTO> mgDataList, int FlowChart_Master_UID)
        {
            throw new NotImplementedException();
        }
    }
}

