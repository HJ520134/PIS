using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;

namespace PDMS.Service
{
    //public interface IFlowChartService
    //{
    //    //PagedListModel<FlowChartModelGet> QueryFlowCharts(FlowChartModelSearch searchModel, Page page, List<string> currentProject);
        //string CheckFlowChart(FlowChartExcelImportParas parasItem);
        //List<SystemFunctionPlantDTO> QueryAllFunctionPlants();
        //void ImportFlowChart(FlowChartImport importItem);
        //void ImportFlowUpdateChart(FlowChartImport importItem);
        //FlowChartGet QueryFlowChart(int FlowChart_Master_UID);
        //FlowChartExcelExport ExportFlowChart(int FlowChart_Master_UID, bool isTemp);
        //List<FlowChartHistoryGet> QueryHistoryFlowChart(int FlowChart_Master_UID);
        //PagedListModel<FlowChartDetailGet> QueryFLDetailList(int id, int Version);
        //List<FlowChartDetailAndMgData> QueryFLDetailList(int id, string week);
        //bool ClosedFL(int id, bool isClosed);
        //PagedListModel<FlowChartDetailGet> QueryFLDetailTempList(int id, int Version);
        //FlowChartDetailGet QueryFLDetailByID(int id);
        //FlowChartDetailGetByMasterInfo QueryFunPlant(int id);
        //string SaveFLDetailInfo(FlowChartDetailDTO dto, int AccountID);
        //void SaveAllDetailInfo(List<FlowChartDetailAndMGDataInputDTO> dto, int AccountID);
        //void DeleteFLTemp(int id);
        //void ImportFlowChartMGData(List<FlowChartMgDataDTO> mgDataList, int FlowChart_Master_UID);
        //FlowChartMasterDTO GetMasterItemBySearchCondition(PPCheckDataSearch searchModel);

        //FlowChartDetailGetByMasterInfo QueryBindBomByMasterId(int id);
        //PagedListModel<FlowChartBomGet> QueryBomByFlowChartUID(int id, int Version, List<int> plants);
        //FlowChartBomGet QueryBomEditByFlowChartUID(int PC_MH_UID);
        //List<FlowChartBomGet> QueryFLDetailByUIDAndVersion(int MasterUID, int Version, List<int> idList);
        //List<SystemUserDTO> QueryBomUserInfo();
        //int CheckBomUser(GetFuncPlantProcessSearch search);
        //string InsertBomUserInfo(List<FlowChartPCMHRelationshipVM> list);
        //string EditFLPCBomInfo(FlowChartBomGet bomItem, int CurrUser);
        //void DeleteBomInfoByUIDList(List<int> IdList);
        //List<FlowChartPCMHRelationshipDTO> GetALlPCMH();
        //List<FlowChartDetailDTO> GetMaxDetailInfoAPI(int UID);
        //#region ----- add by Destiny Zhang  

        //PagedListModel<PrjectListVM> QueryFlowChartMasterDatas(int user_account_uid, List<string> currentProject);
        //ProcessDataSearch QueryFlowChartDataByMasterUid(int flowChartMaster_uid);
        //List<FlowChartMasterDTO> QueryProjectTypes();
        //List<string> QueryProcess(int flowchartmasterUid);

        //#endregion

        //#region ---------add by justin

        //PagedListModel<FlPlanManagerVM> QueryProcessMGData(int masterUID, DateTime date);
        //FlowChartPlanManagerVM QueryProcessMGDataSingle(int uid, DateTime date);
        //string FlowChartPlan(FlowChartPlanManagerVM ent);

        //#endregion
    //}

    //public class FlowChartService : IFlowChartService
    //{
    //    #region Private interfaces properties
    //    private readonly IUnitOfWork unitOfWork;
    //    private readonly IFlowChartMasterRepository flowChartMasterRepository;
    //    private readonly IFlowChartDetailRepository flowChartDetailRepository;
    //    private readonly IFlowChartMgDataRepository flowChartMgDataRepository;
    //    private readonly ISystemBUDRepository systemBUDRepository;
    //    private readonly ISystemProjectRepository systemProjectRepository;
    //    private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;
    //    private readonly IFlowChartDetailTempRepository flowChartDetailTempRepository;
    //    private readonly IFlowChartMgDataTempRepository flowChartMgDataTempRepository;
    //    private readonly ISystemUserRepository systemUserRepository;
    //    private readonly IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository;
    //    private readonly ISystemUserRoleRepository systemUserRoleRepository;
    //    private readonly ISystemRoleRepository systemRoleRepository;
    //    private readonly ISystemUserOrgRepository systemUserOrgRepository;
    //    private readonly IProjectUsersGroupRepository projectUsersGroupRepository;

    //    #endregion //Private interfaces properties

    //    #region Service constructor
    //    public FlowChartService(
    //        IFlowChartMasterRepository flowChartMasterRepository,
    //        IFlowChartDetailRepository flowChartDetailRepository,
    //        IFlowChartMgDataRepository flowChartMgDataRepository,
    //        ISystemBUDRepository systemBUDRepository,
    //        ISystemProjectRepository systemProjectRepository,
    //        ISystemFunctionPlantRepository systemFunctionPlantRepository,
    //        IFlowChartDetailTempRepository flowChartDetailTempRepository,
    //        IFlowChartMgDataTempRepository flowChartMgDataTempRepository,
    //        ISystemUserRepository systemUserRepository,
    //        ISystemUserRoleRepository systemUserRoleRepository,
    //        IFlowChartPCMHRelationshipRepository flowChartPCMHRelationshipRepository,
    //        ISystemRoleRepository systemRoleRepository,
    //        ISystemUserOrgRepository systemUserOrgRepository,
    //        IProjectUsersGroupRepository projectUsersGroupRepository,
    //    IUnitOfWork unitOfWork)
    //    {
    //        this.systemRoleRepository = systemRoleRepository;
    //        this.unitOfWork = unitOfWork;
    //        this.flowChartMasterRepository = flowChartMasterRepository;
    //        this.flowChartDetailRepository = flowChartDetailRepository;
    //        this.flowChartMgDataRepository = flowChartMgDataRepository;
    //        this.systemProjectRepository = systemProjectRepository;
    //        this.systemBUDRepository = systemBUDRepository;
    //        this.systemFunctionPlantRepository = systemFunctionPlantRepository;
    //        this.flowChartDetailTempRepository = flowChartDetailTempRepository;
    //        this.flowChartMgDataTempRepository = flowChartMgDataTempRepository;
    //        this.systemUserRepository = systemUserRepository;
    //        this.systemUserRoleRepository = systemUserRoleRepository;
    //        this.flowChartPCMHRelationshipRepository = flowChartPCMHRelationshipRepository;
    //        this.systemUserOrgRepository = systemUserOrgRepository;
    //        this.projectUsersGroupRepository = projectUsersGroupRepository;
    //    }
    //    #endregion //Service constructor

    //    public PagedListModel<FlowChartModelGet> QueryFlowCharts(FlowChartModelSearch searchModel, Page page, List<string> currentProject)
    //    {
    //        var totalCount = 0;
    //        var flChartList = flowChartMasterRepository.QueryFlowCharts(searchModel, page, out totalCount);
    //        if (currentProject.Count != 0)
    //            flChartList = flChartList.Where(m => currentProject.Contains(m.Project_Name));
    //        return new PagedListModel<FlowChartModelGet>(totalCount, flChartList.ToList());
    //    }

    //    private FlowChartGet SetAutoMapFlChart(FlowChart_Master item)
    //    {
    //        FlowChartGet model = new FlowChartGet();
    //        model.FlowChartMasterDTO = AutoMapper.Mapper.Map<FlowChartMasterDTO>(item);
    //        model.SystemProjectDTO = AutoMapper.Mapper.Map<SystemProjectDTO>(item.System_Project);
    //        model.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users);
    //        model.BU_D_Name = item.System_Project.System_BU_D.BU_D_Name;
    //        return model;
    //    }

    //    public List<FlowChartMasterDTO> QueryProjectTypes()
    //    {
    //        var EnumEntity = flowChartMasterRepository.QueryProjectTypes();
    //        var result = AutoMapper.Mapper.Map<List<FlowChartMasterDTO>>(EnumEntity);
    //        return result;
    //    }

    //    public List<string> QueryProcess(int flowchartmasterUid)
    //    {
    //        return flowChartDetailRepository.QueryProcess(flowchartmasterUid).ToList();
    //    }

    //    public string CheckFlowChart(FlowChartExcelImportParas parasItem)
    //    {
    //        string isValid = string.Empty;
    //        var budItem = systemBUDRepository.GetMany(m => m.BU_D_Name == parasItem.BU_D_Name).FirstOrDefault();
    //        //如果budItem没有，说明不存在这个客户
    //        if (budItem != null)
    //        {
    //            if (parasItem.isEdit)
    //            {
    //                var masterItem = flowChartMasterRepository.GetById(parasItem.FlowChart_Master_UID);
    //                if (masterItem.System_Project.Project_Name == parasItem.Project_Name && masterItem.System_Project.System_BU_D.BU_D_Name == parasItem.BU_D_Name
    //                    && masterItem.System_Project.Product_Phase == parasItem.Product_Phase && masterItem.Part_Types == parasItem.Part_Types)
    //                {
    //                    isValid = string.Format("{0}_{1}_{2}", masterItem.FlowChart_Master_UID, masterItem.Project_UID, masterItem.FlowChart_Version);
    //                }
    //                else
    //                {
    //                    isValid = "客户，专案名称，部件，阶段不匹配，不能更新";
    //                }
    //            }
    //            else
    //            {
    //                var projetItem = systemProjectRepository.GetMany(m => m.BU_D_UID == budItem.BU_D_UID && m.Project_Name == parasItem.Project_Name && m.Product_Phase == parasItem.Product_Phase).FirstOrDefault();
    //                //如果projectUIDList没有，说明该数据不存在
    //                if (projetItem != null)
    //                {
    //                    //如果flItem为空，说明可以新导入这条数据
    //                    var flItem = flowChartMasterRepository.GetMany(m => m.Project_UID == projetItem.Project_UID && m.Part_Types == parasItem.Part_Types).FirstOrDefault();
    //                    if (flItem == null)
    //                    {
    //                        isValid = projetItem.Project_UID.ToString();
    //                    }
    //                    else
    //                    {
    //                        isValid = "导入的专案已经存在，不能新增专案";
    //                    }
    //                }
    //                else
    //                {
    //                    isValid = "客户或专案名称或阶段不存在，不能导入";
    //                }
    //            }
    //        }
    //        else
    //        {
    //            isValid = "客户名称不存在";
    //        }
    //        return isValid;
    //    }

    //    public List<SystemFunctionPlantDTO> QueryAllFunctionPlants()
    //    {
    //        var functionPlants = systemFunctionPlantRepository.GetAll();
    //        List<SystemFunctionPlantDTO> dtoList = new List<SystemFunctionPlantDTO>();
    //        foreach (var item in functionPlants)
    //        {
    //            dtoList.Add(AutoMapper.Mapper.Map<SystemFunctionPlantDTO>(item));
    //        }
    //        return dtoList;
    //    }

    //    public void ImportFlowChart(FlowChartImport importItem)
    //    {
    //        FlowChart_Master flMasterItem = new FlowChart_Master();
    //        flMasterItem = AutoMapper.Mapper.Map<FlowChart_Master>(importItem.FlowChartMasterDTO);
    //        flowChartMasterRepository.Add(flMasterItem);

    //        foreach (var detailDTOItem in importItem.FlowChartImportDetailDTOList)
    //        {
    //            var detailItem = AutoMapper.Mapper.Map<FlowChart_Detail>(detailDTOItem.FlowChartDetailDTO);
    //            flMasterItem.FlowChart_Detail.Add(detailItem);

    //            var mgDataItem = AutoMapper.Mapper.Map<FlowChart_MgData>(detailDTOItem.FlowChartMgDataDTO);
    //            detailItem.FlowChart_MgData.Add(mgDataItem);
    //        }
    //        unitOfWork.Commit();
    //    }

    //    public void ImportFlowUpdateChart(FlowChartImport importItem)
    //    {
    //        //进行更新操作，全删全插
    //        //var oldMasterFLItem = flowChartMasterRepository.GetById(importItem.FlowChartMasterDTO.FlowChart_Master_UID);

    //        //全删操作
    //        //var tempDetailList = flowChartDetailTempRepository.GetMany(m => m.FlowChart_Master_UID == oldMasterFLItem.FlowChart_Master_UID).ToList();
    //        //foreach (var tempDetailItem in tempDetailList)
    //        //{
    //        //    flowChartDetailTempRepository.Delete(tempDetailItem);
    //        //}
    //        //var tempIdList = tempDetailList.Select(m => m.FlowChart_DT_UID).ToList();
    //        //var tempMgDataList = flowChartMgDataTempRepository.GetMany(m => tempIdList.Contains(m.FlowChart_DT_UID)).ToList();
    //        //foreach (var tempMgDataItem in tempMgDataList)
    //        //{
    //        //    flowChartMgDataTempRepository.Delete(tempMgDataItem);
    //        //}

    //        //全插操作
    //        //oldMasterFLItem.FlowChart_Version_Comment = importItem.FlowChartMasterDTO.FlowChart_Version_Comment;
    //        //List<FlowChart_Detail_Temp> flDetailTempList = new List<FlowChart_Detail_Temp>();
    //        //List<FlowChart_MgData_Temp> flMgTempList = new List<FlowChart_MgData_Temp>();
    //        //StringBuilder insertSql = new StringBuilder();
    //        //foreach (var detailDTOItem in importItem.FlowChartImportDetailDTOList)
    //        //{
    //        //    ////第二层表
    //        //    //var detailItem = AutoMapper.Mapper.Map<FlowChart_Detail_Temp>(detailDTOItem.FlowChartDetailDTO);
    //        //    //detailItem.Rise_Version_Flag = true;

    //        //    //oldMasterFLItem.FlowChart_Detail_Temp.Add(detailItem);

    //        //    ////第三层表
    //        //    //FlowChart_MgData_Temp newMgData = new FlowChart_MgData_Temp();
    //        //    //newMgData.FlowChart_DT_UID = detailItem.FlowChart_DT_UID;
    //        //    //newMgData.Product_Plan = detailDTOItem.FlowChartMgDataDTO.Product_Plan;
    //        //    //newMgData.Target_Yield = detailDTOItem.FlowChartMgDataDTO.Target_Yield;
    //        //    //newMgData.Modified_UID = detailDTOItem.FlowChartMgDataDTO.Modified_UID;
    //        //    //newMgData.Modified_Date = detailDTOItem.FlowChartMgDataDTO.Modified_Date;
    //        //    //detailItem.FlowChart_MgData_Temp.Add(newMgData);


    //        //}


    //        //flowChartMasterRepository.Update(oldMasterFLItem);

    //        //unitOfWork.Commit();

    //        flowChartMasterRepository.UpdateFolowCharts(importItem);
    //    }

    //    public FlowChartGet QueryFlowChart(int FlowChart_Master_UID)
    //    {
    //        var flItem = flowChartMasterRepository.GetById(FlowChart_Master_UID);
    //        var item = SetAutoMapFlChart(flItem);
    //        return item;
    //    }

    //    public PagedListModel<FlowChartDetailGet> QueryFLDetailList(int id, int Version)
    //    {
    //        var totalCount = 0;
    //        var masterItem = flowChartMasterRepository.GetById(id);
    //        if (masterItem != null)
    //        {
    //            IList<FlowChartDetailGet> importList = new List<FlowChartDetailGet>();

    //            var flChartList = flowChartMasterRepository.QueryFLDetailList(id, Version, out totalCount);
    //            //importItem.FlowChartMasterDTO = AutoMapper.Mapper.Map<FlowChartMasterDTO>(masterItem);
    //            foreach (var flChartItem in flChartList)
    //            {
    //                FlowChartDetailGet importDetailItem = new FlowChartDetailGet();
    //                importDetailItem.FlowChartDetailDTO = AutoMapper.Mapper.Map<FlowChartDetailDTO>(flChartItem);
    //                if (flChartItem.FatherProcess_UID != null)
    //                {
    //                    var process = flChartList.Where(m => m.FlowChart_Detail_UID == flChartItem.FatherProcess_UID).Select(m => m.Process).First();
    //                    importDetailItem.FlowChartDetailDTO.FatherProcess = process;
    //                }

    //                switch (flChartItem.IsQAProcess)
    //                {
    //                    case StructConstants.IsQAProcessType.InspectKey: //IPQC全检
    //                        importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectText;
    //                        break;
    //                    case StructConstants.IsQAProcessType.PollingKey: //IPQC巡检
    //                        importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.PollingText;
    //                        break;
    //                    case StructConstants.IsQAProcessType.InspectOQCKey: //OQC检测
    //                        importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectOQCText;
    //                        break;
    //                    case StructConstants.IsQAProcessType.InspectAssembleKey: //组装检测
    //                        importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.InspectAssembleText;
    //                        break;
    //                    case StructConstants.IsQAProcessType.AssembleOQCKey: //组装&OQC检测
    //                        importDetailItem.FlowChartDetailDTO.IsQAProcessName = StructConstants.IsQAProcessType.AssembleOQCText;
    //                        break;
    //                    default:
    //                        importDetailItem.FlowChartDetailDTO.IsQAProcessName = string.Empty;
    //                        break;
    //                }

    //                //importDetailItem.FlowChartMgDataDTO = AutoMapper.Mapper.Map<FlowChartMgDataDTO>(flChartItem.FlowChart_MgData.FirstOrDefault());
    //                importDetailItem.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(flChartItem.System_Users);
    //                importDetailItem.FunPlant = flChartItem.System_Function_Plant.FunPlant;
    //                importList.Add(importDetailItem);
    //            }

    //            return new PagedListModel<FlowChartDetailGet>(0, importList);
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }

    //    public List<FlowChartDetailAndMgData> QueryFLDetailList(int id, string week)
    //    {
    //        Week getWeek = new Model.ViewModels.Week();
    //        List<FlowChartMgDataDTO> mgDataList = new List<FlowChartMgDataDTO>();
    //        List<int> flDetailUIDList = new List<int>();
    //        switch (week)
    //        {
    //            case "next":
    //                getWeek = GetCurrentWeek(DateTime.Now.Date);
    //                break;
    //            case "current":
    //                getWeek = GetLastWeek(DateTime.Now.Date);
    //                break;
    //        }
    //        var maxVersion = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == id).Max(m => m.FlowChart_Version);
    //        var list = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == id && m.FlowChart_Version == maxVersion).OrderBy(m => m.Process_Seq).ToList();
    //        var dto = AutoMapper.Mapper.Map<List<FlowChartDetailAndMgData>>(list);
    //        if (list.Count() > 0)
    //        {
    //            flDetailUIDList = list.Select(m => m.FlowChart_Detail_UID).ToList();
    //            var mgList = flowChartMgDataRepository.GetMany(m => flDetailUIDList.Contains(m.FlowChart_Detail_UID) && m.Product_Date >= getWeek.Monday && m.Product_Date <= getWeek.Sunday).ToList();
    //            mgDataList = AutoMapper.Mapper.Map<List<FlowChartMgDataDTO>>(mgList);

    //            foreach (var flDetailMgDataItem in dto)
    //            {
    //                var currentMgList = mgDataList.Where(m => m.FlowChart_Detail_UID == flDetailMgDataItem.FlowChart_Detail_UID).ToList();
    //                flDetailMgDataItem.MgDataList = currentMgList;
    //            }
    //        }
    //        return dto;


    //    }

    //    private Week GetLastWeek(DateTime dt)
    //    {
    //        var strDT = dt.DayOfWeek.ToString();
    //        Week nextWeek = new Week();
    //        //获取下周一的日期
    //        switch (strDT)
    //        {
    //            case "Monday":
    //                nextWeek.Monday = dt.AddDays(-7);
    //                break;
    //            case "Tuesday":
    //                nextWeek.Monday = dt.AddDays(-8);
    //                break;
    //            case "Wednesday":
    //                nextWeek.Monday = dt.AddDays(-9);
    //                break;
    //            case "Thursday":
    //                nextWeek.Monday = dt.AddDays(-10);
    //                break;
    //            case "Friday":
    //                nextWeek.Monday = dt.AddDays(-11);
    //                break;
    //            case "Saturday":
    //                nextWeek.Monday = dt.AddDays(-12);
    //                break;
    //            case "Sunday":
    //                nextWeek.Monday = dt.AddDays(-13);
    //                break;
    //        }
    //        nextWeek.Tuesday = nextWeek.Monday.AddDays(1);
    //        nextWeek.Wednesday = nextWeek.Monday.AddDays(2);
    //        nextWeek.Thursday = nextWeek.Monday.AddDays(3);
    //        nextWeek.Friday = nextWeek.Monday.AddDays(4);
    //        nextWeek.Saturday = nextWeek.Monday.AddDays(5);
    //        nextWeek.Sunday = nextWeek.Monday.AddDays(6);

    //        return nextWeek;
    //    }

    //    private Week GetCurrentWeek(DateTime dt)
    //    {
    //        var strDT = dt.DayOfWeek.ToString();
    //        Week currentWeek = new Week();
    //        switch (strDT)
    //        {
    //            case "Monday":
    //                currentWeek.Monday = dt;
    //                break;
    //            case "Tuesday":
    //                currentWeek.Monday = dt.AddDays(-1);
    //                break;
    //            case "Wednesday":
    //                currentWeek.Monday = dt.AddDays(-2);
    //                break;
    //            case "Thursday":
    //                currentWeek.Monday = dt.AddDays(-3);
    //                break;
    //            case "Friday":
    //                currentWeek.Monday = dt.AddDays(-4);
    //                break;
    //            case "Saturday":
    //                currentWeek.Monday = dt.AddDays(-5);
    //                break;
    //            case "Sunday":
    //                currentWeek.Monday = dt.AddDays(-6);
    //                break;
    //        }
    //        currentWeek.Tuesday = currentWeek.Monday.AddDays(1);
    //        currentWeek.Wednesday = currentWeek.Monday.AddDays(2);
    //        currentWeek.Thursday = currentWeek.Monday.AddDays(3);
    //        currentWeek.Friday = currentWeek.Monday.AddDays(4);
    //        currentWeek.Saturday = currentWeek.Monday.AddDays(5);
    //        currentWeek.Sunday = currentWeek.Monday.AddDays(6);

    //        return currentWeek;
    //    }

    //    public PagedListModel<FlowChartDetailGet> QueryFLDetailTempList(int id, int Version)
    //    {
    //        var totalCount = 0;
    //        var masterItem = flowChartMasterRepository.GetById(id);
    //        IList<FlowChartDetailGet> importList = new List<FlowChartDetailGet>();
    //        if (masterItem != null)
    //        {
    //            var flChartList = flowChartMasterRepository.QueryFLDetailTempList(id, Version, out totalCount);
    //            //importItem.FlowChartMasterDTO = AutoMapper.Mapper.Map<FlowChartMasterDTO>(masterItem);
    //            foreach (var flChartItem in flChartList)
    //            {
    //                FlowChartDetailGet importDetailItem = new FlowChartDetailGet();
    //                importDetailItem.FlowChartDetailTempDTO = AutoMapper.Mapper.Map<FlowChartDetailTempDTO>(flChartItem);
    //                importDetailItem.FlowChartMgDataTempDTO = AutoMapper.Mapper.Map<FlowChartMgDataTempDTO>(flChartItem.FlowChart_MgData_Temp.FirstOrDefault());

    //                importDetailItem.FlowChartDetailDTO = AutoMapper.Mapper.Map<FlowChartDetailDTO>(importDetailItem.FlowChartDetailTempDTO);
    //                importDetailItem.FlowChartDetailDTO.FlowChart_Detail_UID = importDetailItem.FlowChartDetailTempDTO.FlowChart_DT_UID;

    //                //importDetailItem.FlowChartMgDataDTO = AutoMapper.Mapper.Map<FlowChartMgDataDTO>(importDetailItem.FlowChartMgDataTempDTO);
    //                //importDetailItem.FlowChartMgDataDTO.FlowChart_MgData_UID = importDetailItem.FlowChartMgDataTempDTO.FlowChart_MgDataT_UID;

    //                importDetailItem.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(flChartItem.System_Users);
    //                importDetailItem.FunPlant = flChartItem.System_Function_Plant.FunPlant;
    //                importList.Add(importDetailItem);
    //            }
    //        }
    //        return new PagedListModel<FlowChartDetailGet>(0, importList);
    //    }

    //    public List<FlowChartHistoryGet> QueryHistoryFlowChart(int FlowChart_Master_UID)
    //    {
    //        List<FlowChartHistoryGet> historyList = new List<FlowChartHistoryGet>();
    //        var userInfoList = systemUserRepository.GetAll().ToList();
    //        var masterItem = flowChartMasterRepository.GetById(FlowChart_Master_UID);
    //        var detailVersionList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == FlowChart_Master_UID).OrderBy(m => m.FlowChart_Version).Select(m => new { m.FlowChart_Version, m.FlowChart_Version_Comment, m.Modified_Date }).Distinct().ToList();
    //        //var flChartList = flowChartMasterRepository.QueryFLList(FlowChart_Master_UID);
    //        foreach (var detailVersionItem in detailVersionList)
    //        {
    //            var projectItem = masterItem.System_Project;
    //            var userItem = masterItem.System_Users; //userInfoList.Where(m => m.Account_UID == detailVersionItem).First();
    //            FlowChartHistoryGet historyItem = new FlowChartHistoryGet();
    //            historyItem.FlowChart_Master_UID = masterItem.FlowChart_Master_UID;
    //            historyItem.BU_D_Name = projectItem.System_BU_D.BU_D_Name;
    //            historyItem.FlowChart_Version = detailVersionItem.FlowChart_Version;
    //            historyItem.FlowChart_Version_Comment = detailVersionItem.FlowChart_Version_Comment;
    //            historyItem.Project_Name = projectItem.Project_Name;
    //            historyItem.Product_Phase = projectItem.Product_Phase;
    //            historyItem.Part_Types = masterItem.Part_Types;
    //            historyItem.User_Name = userItem.User_Name;
    //            historyItem.Modified_Date = detailVersionItem.Modified_Date;
    //            historyList.Add(historyItem);
    //        }
    //        return historyList;
    //    }

    //    public FlowChartExcelExport ExportFlowChart(int FlowChart_Master_UID, bool isTemp)
    //    {
    //        FlowChartExcelExport exportItem = new FlowChartExcelExport();

    //        var flMasterItem = flowChartMasterRepository.GetById(FlowChart_Master_UID);
    //        var flmasterVersion = flMasterItem.FlowChart_Version;
    //        flMasterItem.FlowChart_Detail = flMasterItem.FlowChart_Detail.Where(m => m.FlowChart_Version == flmasterVersion).ToList();
    //        List<FlowChartDetailAndMGDataDTO> detailList = new List<FlowChartDetailAndMGDataDTO>();

    //        if (flMasterItem != null)
    //        {
    //            exportItem.BU_D_Name = flMasterItem.System_Project.System_BU_D.BU_D_Name;
    //            exportItem.Project_Name = flMasterItem.System_Project.Project_Name;
    //            exportItem.Part_Types = flMasterItem.Part_Types;
    //            exportItem.Product_Phase = flMasterItem.System_Project.Product_Phase;

    //            exportItem.SystemUserDTO = AutoMapper.Mapper.Map<SystemUserDTO>(flMasterItem.System_Users);
    //            //exportItem.FlowChartDetailAndMGDataDTOList = detailList;
    //            exportItem.FlowChartDetailAndMGDataDTOList = flowChartDetailRepository.QueryExportDetailList(flMasterItem.FlowChart_Master_UID, flMasterItem.FlowChart_Version);
    //        }

    //        return exportItem;
    //    }


    //    public PagedListModel<PrjectListVM> QueryFlowChartMasterDatas(int user_account_uid, List<string> currentProject)
    //    {
    //        int totalCount = 0;
    //        var flowChartListDatas = flowChartMasterRepository.QueryFlowChartMasterDatas(user_account_uid, out totalCount);
    //        //添加当前用户的Project
    //        if (flowChartListDatas.Any())
    //        {
    //            var result = new List<PrjectListVM>();
    //            foreach (var item in currentProject)
    //            {
    //                var resultItem = new List<PrjectListVM>();
    //                resultItem = flowChartListDatas.Where(m => m.Project == item).ToList();
    //                result.AddRange(resultItem);
    //            }
    //            flowChartListDatas = result.AsQueryable();
    //        }
    //        return new PagedListModel<PrjectListVM>(totalCount, flowChartListDatas);
    //    }

    //    public ProcessDataSearch QueryFlowChartDataByMasterUid(int flowChartMaster_uid)
    //    {

    //        var tempDataList = flowChartMasterRepository.QueryFlowChartDataByMasterUid(flowChartMaster_uid);
    //        ProcessDataSearch result = new ProcessDataSearch();
    //        foreach (var item in tempDataList.Distinct())
    //        {
    //            result.Customer = item.Customer;
    //            result.Part_Types = item.Part_Types;
    //            result.Product_Phase = item.Product_Phase;
    //            result.Project = item.Project;
    //            result.Func_Plant = item.Func_Plant;
    //            result.FlowChart_Master_UID = item.FlowChart_Master_UID;
    //            result.FlowChart_Version = item.FlowChart_Version;
    //        }
    //        return result;
    //    }

    //    public bool ClosedFL(int id, bool isClosed)
    //    {
    //        var item = flowChartMasterRepository.GetById(id);
    //        if (isClosed)
    //        {
    //            item.Is_Closed = true;
    //        }
    //        else
    //        {
    //            item.Is_Closed = false;
    //        }
    //        flowChartMasterRepository.Update(item);
    //        unitOfWork.Commit();
    //        return true;
    //    }

    //    public FlowChartDetailGet QueryFLDetailByID(int id)
    //    {
    //        FlowChartDetailGet detailItem = new FlowChartDetailGet();
    //        var item = flowChartDetailRepository.GetById(id);
    //        detailItem.FlowChartDetailDTO = AutoMapper.Mapper.Map<FlowChartDetailDTO>(item);
    //        //获取父节点制程信息
    //        Dictionary<int, string> FatherProcessDict = new Dictionary<int, string>();
    //        //去掉重复的process后的制程信息
    //        Dictionary<int, string> DistinctFatherProcessDict = new Dictionary<int, string>();
    //        //var fatherProcessItem = flowChartDetailRepository.GetById(detailItem.FlowChartDetailDTO.FatherProcess);
    //        switch (item.Process_Seq)
    //        {
    //            case 1:
    //                //取后面4个Seq
    //                var seqListOne = new List<int> { 2, 3, 4, 5 };
    //                var afterSeqList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
    //                && seqListOne.Contains(m.Process_Seq))
    //                    .Select(m => new { m.FlowChart_Detail_UID, m.Process }).ToList();
    //                foreach (var afterSeqItem in afterSeqList)
    //                {
    //                    FatherProcessDict.Add(afterSeqItem.FlowChart_Detail_UID, afterSeqItem.Process);
    //                }
    //                break;
    //            //取Seq=1和Seq=3，4
    //            case 2:
    //                var seqListTwo = new List<int> { 1, 3, 4, 5 };
    //                var afterSeqTwoList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
    //                && seqListTwo.Contains(m.Process_Seq))
    //                    .Select(m => new { m.FlowChart_Detail_UID, m.Process }).ToList();
    //                foreach (var afterSeqItem in afterSeqTwoList)
    //                {
    //                    FatherProcessDict.Add(afterSeqItem.FlowChart_Detail_UID, afterSeqItem.Process);
    //                }
    //                break;
    //            //取前两个Seq和后两个Seq
    //            default:
    //                var frontOne = item.Process_Seq - 1;
    //                var frontTwo = item.Process_Seq - 2;
    //                var behindOne = item.Process_Seq + 1;
    //                var behindTwo = item.Process_Seq + 2;
    //                var seqListThree = new List<int> { frontOne, frontTwo, behindOne, behindTwo };
    //                var seqThreeList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
    //                && seqListThree.Contains(m.Process_Seq))
    //                    .Select(m => new { m.FlowChart_Detail_UID, m.Process }).ToList();
    //                foreach (var afterSeqItem in seqThreeList)
    //                {
    //                    FatherProcessDict.Add(afterSeqItem.FlowChart_Detail_UID, afterSeqItem.Process);
    //                }
    //                break;
    //        }
    //        var groupByValues = FatherProcessDict.GroupBy(m => m.Value);
    //        foreach (var groupItem in groupByValues)
    //        {
    //            var hasItem = FatherProcessDict.Where(m => m.Value == groupItem.Key).FirstOrDefault();
    //            DistinctFatherProcessDict.Add(hasItem.Key, hasItem.Value);
    //        }
    //        detailItem.FatherProcess = DistinctFatherProcessDict;
    //        detailItem.FunPlant = item.System_Function_Plant.FunPlant;
    //        return detailItem;
    //    }

    //    public FlowChartDetailGetByMasterInfo QueryFunPlant(int id)
    //    {
    //        var masterItem = flowChartMasterRepository.GetById(id);

    //        FlowChartDetailGetByMasterInfo detailInfo = new FlowChartDetailGetByMasterInfo();
    //        detailInfo.BU_D_Name = masterItem.System_Project.System_BU_D.BU_D_Name;
    //        detailInfo.Project_Name = masterItem.System_Project.Project_Name;
    //        detailInfo.Part_Types = masterItem.Part_Types;
    //        detailInfo.Product_Phase = masterItem.System_Project.Product_Phase;

    //        List<SystemFunctionPlantDTO> dtoList = new List<SystemFunctionPlantDTO>();
    //        var list = systemFunctionPlantRepository.GetAll().OrderBy(m => m.FunPlant).ToList();
    //        dtoList = AutoMapper.Mapper.Map<List<SystemFunctionPlantDTO>>(list);
    //        //为功能厂加上OP类型，以区分不同OP类型相同功能厂
    //        foreach (var item in dtoList)
    //        {
    //            item.FunPlant += "_" + item.OP_Types;
    //        }
    //        detailInfo.SystemFunctionPlantDTOList = dtoList;
    //        return detailInfo;
    //    }

    //    public string SaveFLDetailInfo(FlowChartDetailDTO dto, int AccountID)
    //    {
    //        string errorInfo = "";
    //        try
    //        {
    //            var item = flowChartDetailRepository.GetById(dto.FlowChart_Detail_UID);
    //            item.DRI = dto.DRI;
    //            item.Place = dto.Place;
    //            item.System_FunPlant_UID = dto.System_FunPlant_UID;
    //            //item.Product_Stage = dto.Product_Stage;
    //            item.Process_Desc = dto.Process_Desc;
    //            //item.Color = dto.Color;
    //            //item.Material_No = dto.Material_No;
    //            item.Modified_UID = AccountID;
    //            item.Modified_Date = DateTime.Now;
    //            item.FatherProcess_UID = dto.FatherProcess_UID;
    //            //设置QA站点--------------------Sidney
    //            if (dto.IsQAProcess == "NULL")
    //            {
    //                dto.IsQAProcess = "";
    //            }
    //            if (item.IsQAProcess != dto.IsQAProcess)
    //            {
    //                //获取相同制程的信息
    //                var isSameProcessList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version
    //                && m.Process_Seq == item.Process_Seq).ToList();
    //                foreach (var isSameProcessItem in isSameProcessList)
    //                {
    //                    isSameProcessItem.IsQAProcess = dto.IsQAProcess;
    //                }
    //            }


    //            //item.IsQAProcess = dto.IsQAProcess;
    //            flowChartDetailRepository.Update(item);

    //            #region 更新Rework_Flag,空值则置为Null------------------Sidney 2016/04/17
    //            if (dto.Rework_Flag == "NULL") dto.Rework_Flag = null;
    //            //检查是否已经存在Rework_Flag==Repair，是则不能更新
    //            var masterVersion = flowChartMasterRepository.GetById(item.FlowChart_Master_UID).FlowChart_Version;
    //            var isExsitRepair = flowChartDetailRepository.GetMany(m => m.Rework_Flag == "Repair" && m.Process_Seq != dto.Process_Seq && m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == masterVersion).Count();
    //            if (isExsitRepair == 0 || dto.Rework_Flag != "Repair")
    //            {
    //                //将该条记录所在的相同站点所有记录一同更新
    //                var sameitem = flowChartDetailRepository.GetMany(m => m.Process_Seq == item.Process_Seq && m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == masterVersion).ToList();
    //                foreach (var r in sameitem)
    //                {
    //                    var result = flowChartDetailRepository.GetById(r.FlowChart_Detail_UID);
    //                    result.Rework_Flag = dto.Rework_Flag;
    //                    flowChartDetailRepository.Update(result);
    //                }
    //            }
    //            else
    //            {
    //                errorInfo += "已经存在修复站点，不能多余1个修复站点！";
    //            }
    //            #endregion



    //            unitOfWork.Commit();
    //        }
    //        catch (Exception e)
    //        {
    //            errorInfo += "保存失败！，原因如下：" + e.Message.ToString();
    //        }
    //        return errorInfo;
    //    }

    //    public void SaveAllDetailInfo(List<FlowChartDetailAndMGDataInputDTO> list, int AccountID)
    //    {
    //        var idList = list.Select(m => m.FlowChart_Detail_UID).ToList();
    //        var mgDataList = flowChartMgDataRepository.GetMany(m => idList.Contains(m.FlowChart_Detail_UID)).ToList();

    //        foreach (var item in list)
    //        {
    //            var mgDataItem = mgDataList.Where(m => m.FlowChart_Detail_UID == item.FlowChart_Detail_UID).FirstOrDefault();
    //            if (mgDataItem != null)
    //            {
    //                mgDataItem.Product_Plan = item.Product_Plan;
    //                mgDataItem.Target_Yield = double.Parse(item.Target_Yield.Substring(0, item.Target_Yield.Length - 1)) / 100;
    //                mgDataItem.Modified_Date = DateTime.Now;
    //                mgDataItem.Modified_UID = AccountID;
    //                flowChartMgDataRepository.Update(mgDataItem);
    //            }
    //        }
    //        unitOfWork.Commit();
    //    }

    //    public void DeleteFLTemp(int id)
    //    {
    //        var item = flowChartMasterRepository.GetById(id);
    //        if (item != null)
    //        {
    //            var detailList = flowChartDetailTempRepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID).ToList();
    //            var detailIDList = detailList.Select(m => m.FlowChart_DT_UID).ToList();
    //            var mgTempList = flowChartMgDataTempRepository.GetMany(m => detailIDList.Contains(m.FlowChart_DT_UID)).ToList();
    //            foreach (var mgTempItem in mgTempList)
    //            {
    //                flowChartMgDataTempRepository.Delete(mgTempItem);
    //            }
    //            foreach (var detailItem in detailList)
    //            {
    //                flowChartDetailTempRepository.Delete(detailItem);
    //            }
    //            unitOfWork.Commit();
    //        }
    //    }

    //    public void ImportFlowChartMGData(List<FlowChartMgDataDTO> mgDataList, int FlowChart_Master_UID)
    //    {
    //        //var maxVersion = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == FlowChart_Master_UID).Max(m => m.FlowChart_Version);
    //        //var detailIDList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == FlowChart_Master_UID && m.FlowChart_Version == maxVersion).Select(m => m.FlowChart_Detail_UID).ToList();

    //        //flowChartMgDataRepository.Delete(q=> detailIDList.Contains(q.FlowChart_Detail_UID));

    //        //var dtoList = AutoMapper.Mapper.Map<IEnumerable<FlowChart_MgData>>(mgDataList);
    //        //foreach (var mgDataItem in dtoList)
    //        //{
    //        //    flowChartMgDataRepository.Add(mgDataItem);
    //        //}
    //        //unitOfWork.Commit();

    //        flowChartMasterRepository.BatchImportPlan(mgDataList, FlowChart_Master_UID);
    //    }

    //    /// <summary>
    //    /// 根据flowchart masteruid 查询计划数据
    //    /// </summary>
    //    /// <param name="masterUID"></param>
    //    /// <returns></returns>
    //    public PagedListModel<FlPlanManagerVM> QueryProcessMGData(int masterUID, DateTime date)
    //    {
    //        var totalCount = 0;

    //        var flChartList = flowChartMasterRepository.QueryFlowMGData(masterUID, date, out totalCount);
    //        var result = new List<FlPlanManagerVM>();
    //        foreach (var item in flChartList)
    //        {
    //            var returnItem = new FlPlanManagerVM();
    //            returnItem.Detail_UID = item.Detail_UID;
    //            returnItem.Process_seq = item.Process_seq;
    //            returnItem.Process = item.Process;
    //            returnItem.date = item.date;
    //            returnItem.Color = item.Color;
    //            returnItem.MondayProduct_Plan = item.MondayProduct_Plan;
    //            if (item.MondayTarget_Yield != null)
    //            {
    //                returnItem.MondayTarget_Yield = item.MondayTarget_Yield * 100 + "%";
    //            }

    //            returnItem.TuesdayProduct_Plan = item.TuesdayProduct_Plan;
    //            if (item.TuesdayTarget_Yield != null)
    //            {
    //                returnItem.TuesdayTarget_Yield = item.TuesdayTarget_Yield * 100 + "%";
    //            }

    //            returnItem.WednesdayProduct_Plan = item.WednesdayProduct_Plan;
    //            if (item.WednesdayTarget_Yield != null)
    //            {
    //                returnItem.WednesdayTarget_Yield = item.WednesdayTarget_Yield * 100 + "%";
    //            }

    //            returnItem.ThursdayProduct_Plan = item.ThursdayProduct_Plan;
    //            if (item.ThursdayTarget_Yield != null)
    //            {
    //                returnItem.ThursdayTarget_Yield = item.ThursdayTarget_Yield * 100 + "%";
    //            }

    //            returnItem.FridayProduct_Plan = item.FridayProduct_Plan;
    //            if (item.FridayTarget_Yield != null)
    //            {
    //                returnItem.FridayTarget_Yield = item.FridayTarget_Yield * 100 + "%";
    //            }

    //            returnItem.SaterdayProduct_Plan = item.SaterdayProduct_Plan;
    //            if (item.SaterdayTarget_Yield != null)
    //            {
    //                returnItem.SaterdayTarget_Yield = item.SaterdayTarget_Yield * 100 + "%";
    //            }

    //            returnItem.SundayProduct_Plan = item.SundayProduct_Plan;
    //            if (item.SundayTarget_Yield != null)
    //            {
    //                returnItem.SundayTarget_Yield = item.SundayTarget_Yield * 100 + "%";
    //            }
    //            result.Add(returnItem);
    //        }

    //        return new PagedListModel<FlPlanManagerVM>(totalCount, result);
    //    }

    //    public FlowChartPlanManagerVM QueryProcessMGDataSingle(int uid, DateTime date)
    //    {
    //        return flowChartMasterRepository.QueryFlowMGDataSingle(uid, date);
    //    }

    //    public string Week(DateTime d)
    //    {
    //        string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
    //        string week = weekdays[Convert.ToInt32(d.DayOfWeek)];

    //        return week;
    //    }
    //    public string FlowChartPlan(FlowChartPlanManagerVM ent)
    //    {
    //        var items = flowChartMasterRepository.UpdatePlan(ent.Detail_UID, ent.date);
    //        int i = 0;

    //        foreach (var item in items)
    //        {
    //            i++;
    //            if (Week(item.Product_Date) == "星期一")
    //            {
    //                item.Product_Plan = int.Parse(ent.MondayProduct_Plan.ToString());
    //                item.Target_Yield = double.Parse(ent.MondayTarget_Yield.ToString());
    //                flowChartMgDataRepository.Update(item);
    //            }
    //            if (Week(item.Product_Date) == "星期二")
    //            {
    //                item.Product_Plan = int.Parse(ent.TuesdayProduct_Plan.ToString());
    //                item.Target_Yield = double.Parse(ent.TuesdayTarget_Yield.ToString());
    //                flowChartMgDataRepository.Update(item);
    //            }
    //            if (Week(item.Product_Date) == "星期三")
    //            {
    //                item.Product_Plan = int.Parse(ent.WednesdayProduct_Plan.ToString());
    //                item.Target_Yield = double.Parse(ent.WednesdayTarget_Yield.ToString());
    //                flowChartMgDataRepository.Update(item);
    //            }
    //            if (Week(item.Product_Date) == "星期四")
    //            {
    //                item.Product_Plan = int.Parse(ent.ThursdayProduct_Plan.ToString());
    //                item.Target_Yield = double.Parse(ent.ThursdayTarget_Yield.ToString());
    //                flowChartMgDataRepository.Update(item);
    //            }
    //            if (Week(item.Product_Date) == "星期五")
    //            {
    //                item.Product_Plan = int.Parse(ent.FridayProduct_Plan.ToString());
    //                item.Target_Yield = double.Parse(ent.FridayTarget_Yield.ToString());
    //                flowChartMgDataRepository.Update(item);
    //            }
    //            if (Week(item.Product_Date) == "星期六")
    //            {
    //                item.Product_Plan = int.Parse(ent.SaterdayProduct_Plan.ToString());
    //                item.Target_Yield = double.Parse(ent.SaterdayTarget_Yield.ToString());
    //                flowChartMgDataRepository.Update(item);
    //            }
    //            if (Week(item.Product_Date) == "星期日")
    //            {
    //                item.Product_Plan = int.Parse(ent.SundayProduct_Plan.ToString());
    //                item.Target_Yield = double.Parse(ent.SundayTarget_Yield.ToString());
    //                flowChartMgDataRepository.Update(item);
    //            }

    //        }
    //        unitOfWork.Commit();
    //        return "SUCCESS";
    //    }

    //    #region 绑定物料员画面
    //    public List<FlowChartPCMHRelationshipDTO> GetALlPCMH()
    //    {
    //        var list = flowChartPCMHRelationshipRepository.GetAll().ToList();
    //        var dtoList = AutoMapper.Mapper.Map<List<FlowChartPCMHRelationshipDTO>>(list);
    //        return dtoList;
    //    }

    //    public FlowChartDetailGetByMasterInfo QueryBindBomByMasterId(int id)
    //    {
    //        var masterItem = flowChartMasterRepository.GetById(id);

    //        FlowChartDetailGetByMasterInfo detailInfo = new FlowChartDetailGetByMasterInfo();
    //        detailInfo.BU_D_Name = masterItem.System_Project.System_BU_D.BU_D_Name;
    //        detailInfo.Project_Name = masterItem.System_Project.Project_Name;
    //        detailInfo.Part_Types = masterItem.Part_Types;
    //        detailInfo.Product_Phase = masterItem.System_Project.Product_Phase;

    //        return detailInfo;
    //    }

    //    public PagedListModel<FlowChartBomGet> QueryBomByFlowChartUID(int id, int version, List<int> plants)
    //    {
    //        List<FlowChartBomGet> list = new List<FlowChartBomGet>();
    //        var detailList = flowChartDetailRepository.QueryBomByFlowChartUID(id, version, plants).ToList();

    //        return new PagedListModel<FlowChartBomGet>(0, detailList);
    //    }

    //    public FlowChartBomGet QueryBomEditByFlowChartUID(int PC_MH_UID)
    //    {

    //        var mhItem = flowChartPCMHRelationshipRepository.GetById(PC_MH_UID);
    //        var detailItem = flowChartDetailRepository.GetById(mhItem.FlowChart_Detail_UID);
    //        FlowChartBomGet bomItem = new FlowChartBomGet();
    //        bomItem.FlowChart_Detail_UID = detailItem.FlowChart_Detail_UID;
    //        bomItem.PC_MH_UID = PC_MH_UID;
    //        bomItem.Process_Seq = detailItem.Process_Seq;
    //        bomItem.Process = detailItem.Process;
    //        bomItem.Place = mhItem.Place;
    //        bomItem.User_NTID = mhItem.System_Users1.User_NTID;
    //        return bomItem;
    //    }

    //    public List<FlowChartBomGet> QueryFLDetailByUIDAndVersion(int MasterUID, int Version, List<int> idList)
    //    {
    //        //var list = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == MasterUID && m.FlowChart_Version == Version
    //        //&& idList.Contains(m.System_FunPlant_UID));
    //        //var dto = AutoMapper.Mapper.Map<List<FlowChartDetailDTO>>(list);
    //        //return dto;
    //        var list = flowChartDetailRepository.QueryFLDetailByUIDAndVersion(MasterUID, Version, idList);
    //        return list;
    //    }

    //    public List<SystemUserDTO> QueryBomUserInfo()
    //    {
    //        //获取所有用户信息，还要判断用户是否是MHFLAG
    //        var list = systemUserRepository.GetAll().ToList();
    //        var dto = AutoMapper.Mapper.Map<List<SystemUserDTO>>(list);
    //        return dto;
    //    }

    //    public int CheckBomUser(GetFuncPlantProcessSearch search)
    //    {
    //        var count = flowChartDetailRepository.CheckBomUser(search);
    //        return count;
    //    }

    //    public string InsertBomUserInfo(List<FlowChartPCMHRelationshipVM> list)
    //    {
    //        //var MHRoleUid =
    //        //    systemRoleRepository.GetMany(m => m.Role_Name == "维护角色").Select(m => m.Role_UID).FirstOrDefault();
    //        var ErrorInfo = "";
    //        try
    //        {
    //            using (TransactionScope scope = new TransactionScope())
    //            {
    //                foreach (var item in list)
    //                {
    //                    var flowchartPCItem = AutoMapper.Mapper.Map<FlowChart_PC_MH_Relationship>(item.FlowchartPCDTOItem);

    //                    var hasExistUserItem = systemUserRepository.GetMany(m => m.User_NTID.ToUpper().Equals(item.UserNTID.ToUpper())).FirstOrDefault();
    //                    if (hasExistUserItem == null)
    //                    {
    //                        int maxAccountID = flowChartPCMHRelationshipRepository.InsertBomUserInfo(list);

    //                        System_Users userItem = new System_Users();
    //                        userItem.Account_UID = maxAccountID + 1;
    //                        userItem.User_NTID = item.UserNTID;
    //                        userItem.User_Name = item.UserNTID;
    //                        userItem.Enable_Flag = true;
    //                        userItem.Email = "";
    //                        userItem.Modified_UID = item.FlowchartPCDTOItem.Modified_UID;
    //                        userItem.Modified_Date = item.FlowchartPCDTOItem.Modified_Date;
    //                        userItem.MH_Flag = true;

    //                        systemUserRepository.Add(userItem);

    //                        userItem.FlowChart_PC_MH_Relationship1.Add(flowchartPCItem);

    //                        //SystemUserRoleDTO userRoleDTO = new SystemUserRoleDTO();
    //                        //userRoleDTO.Account_UID = userDTO.Account_UID;
    //                        //userRoleDTO.Modified_UID = this.CurrentUser.AccountUId;
    //                        //userRoleDTO.Modified_Date = userDTO.Modified_Date;

    //                    }
    //                    else
    //                    {
    //                        hasExistUserItem.FlowChart_PC_MH_Relationship1.Add(flowchartPCItem);
    //                    }
    //                    //数据提交后才能查找的到值，才能判断是否重复UserNTID
    //                    unitOfWork.Commit();

    //                }

    //                scope.Complete();
    //            }

    //        }
    //        catch (Exception e)
    //        {

    //            ErrorInfo = "插入数据失败！" + e.ToString();
    //        }
    //        return ErrorInfo;
    //    }

    //    public string EditFLPCBomInfo(FlowChartBomGet bomItem, int currUser)
    //    {
    //        string result = string.Empty;
    //        FlowChart_PC_MH_Relationship pcItem = new FlowChart_PC_MH_Relationship();
    //        pcItem.FlowChart_Detail_UID = bomItem.FlowChart_Detail_UID;
    //        pcItem.Place = bomItem.Place;
    //        pcItem.Modified_UID = currUser;
    //        pcItem.Modified_Date = DateTime.Now;

    //        //检查用户表中是否存在
    //        try
    //        {
    //            using (var trans = new TransactionScope())
    //            {
    //                //删除以前的那条数据
    //                var deleteItem = flowChartPCMHRelationshipRepository.GetById(bomItem.PC_MH_UID);
    //                flowChartPCMHRelationshipRepository.Delete(deleteItem);

    //                var hasUserItem = systemUserRepository.GetMany(m => m.User_NTID.ToUpper().Equals(bomItem.User_NTID.ToUpper())).FirstOrDefault();
    //                if (hasUserItem != null)
    //                {
    //                    if (!hasUserItem.MH_Flag)
    //                    {
    //                        result = "该账号已经存在，并且不是物料员所属的帐号";
    //                        return result;
    //                    }
    //                    else //用户表已经存在，直接新增该账户
    //                    {
    //                        pcItem.MH_UID = hasUserItem.Account_UID;
    //                        flowChartPCMHRelationshipRepository.Add(pcItem);
    //                    }
    //                }
    //                else //新增用户表和关系表
    //                {
    //                    //获取maxAccountUID
    //                    int maxAccountID = flowChartPCMHRelationshipRepository.InsertBomUserInfo(null);

    //                    System_Users newUserItem = new System_Users();
    //                    newUserItem.Account_UID = maxAccountID + 1;
    //                    newUserItem.User_NTID = bomItem.User_NTID;
    //                    newUserItem.User_Name = bomItem.User_NTID;
    //                    newUserItem.Enable_Flag = true;
    //                    newUserItem.Email = "";
    //                    newUserItem.Modified_UID = currUser;
    //                    newUserItem.Modified_Date = pcItem.Modified_Date;
    //                    newUserItem.MH_Flag = true;
    //                    systemUserRepository.Add(newUserItem);
    //                    //新增关系表
    //                    newUserItem.FlowChart_PC_MH_Relationship1.Add(pcItem);

    //                }


    //                unitOfWork.Commit();
    //                trans.Complete();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            result = "数据修改错误";
    //        }

    //        return result;
    //    }

    //    public void DeleteBomInfoByUIDList(List<int> IdList)
    //    {
    //        var list = flowChartPCMHRelationshipRepository.GetMany(m => IdList.Contains(m.PC_MH_UID)).ToList();
    //        flowChartPCMHRelationshipRepository.DeleteList(list);
    //        unitOfWork.Commit();
    //    }

    //    #endregion

    //    public List<FlowChartDetailDTO> GetMaxDetailInfoAPI(int UID)
    //    {
    //        var flowchartMaster = flowChartMasterRepository.GetById(UID);
    //        var detailList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == UID && m.FlowChart_Version == flowchartMaster.FlowChart_Version).ToList();
    //        var list = AutoMapper.Mapper.Map<List<FlowChartDetailDTO>>(detailList);
    //        return list;
    //    }

    //    #region 根据查询条件获取Flowchart信息
    //    public FlowChartMasterDTO GetMasterItemBySearchCondition(PPCheckDataSearch searchModel)
    //    {
    //        var item = flowChartMasterRepository.GetMany(m => m.System_Project.Project_Name == searchModel.Project && m.Part_Types == searchModel.Part_Types
    //        && m.Is_Closed == false).FirstOrDefault();
    //        if (item != null)
    //        {
    //            var dto = AutoMapper.Mapper.Map<FlowChartMasterDTO>(item);
    //            return dto;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //    #endregion

    //}
}
