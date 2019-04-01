using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Common.Constants;
using PDMS.Model.ViewModels.Common;
using System.Data;
using PDMS.Model.EntityDTO;
using PDMS.Data;

namespace PDMS.Service
{
    //public interface IProductionPlanning
    //{
    //    #region ME
    //    PagedListModel<FlowChartModelGet> QueryMEFlowCharts(FlowChartModelSearch searchModel, Page page);
    //    void ImportFlowchartME(ProductionPlanningModelGetAPIModel importItem, bool isEdit, int accountID);
    //    string ChangePhase(int FlowChart_Master_UID);
    //    bool ClosedFL(int id, bool isClosed);
    //    #endregion

    //    #region PP
    //    PagedListModel<FlowChartModelGet> QueryPPFlowCharts(FlowChartModelSearch searchModel, Page page);
    //    PagedListModel<PPFlowchartDetailVM> FlowchartPPDetailList(int id, int Version);
    //    PagedListModel<FlowchartDetailMEDTO> QueryFLDetailAssociationByID(int id);
    //    string CheckProcess(int id, List<int> idList);
    //    List<int> QueryFLProcessCheckedSelectByID(int id);
    //    FlowchartMappingDTO QueryFLDetail(int id);
    //    void SavePPFlowchart(FlowChartDetailDTO dto, List<int> idList);
    //    #endregion

    //    #region ---- IE
    //    string JudgeFlowchart(int FlowChart_Master_UID);
    //    PagedListModel<FLowchart_Detail_IE_VM> QueryFlowChartForIE(int Flowchart_Master_UID,int flag);
    //    List<Flowchart_Detail_IE_VM> QueryFlowChartByRangeNumber(int Flowchart_Detail_ME_UID, int RangeNumber);
    //    Flowchart_Detail_ProductionPlanning QueryFlowChartByFlowchart_Detail_ME_UID(int Flowchart_Detail_ME_UID);

    //    string SaveMappingData(Flowchart_Detail_IE_VMList data);
    //    string SaveIEFlowchartData(Flowchart_Detail_ProductionPlanningList data);
    //    string UpdateIEFlowchartData(Flowchart_Detail_ProductionPlanningList data);
    //    string UpdateFlowchartIEList(List<FLowchart_Detail_IE_VM> ieList);

    //    PagedListModel<HumanResourcesSummaryDTO> GetHumanResourcesSummary(string strWhere);
    //    List<List<PlanDataView>> GetPlanDataByProject(int site, int op, int ProjectID, int PartTypeUID, string begin);
    //    List<List<PlanDataView>> GetInputDataByProject(int op, int project, string begin);
    //    List<HumanResources> GetHumanResourcesByProject(int ProjectID);
    //    List<HumanResources> GetHumanResourcesByFunplant(int ProjectID);
    //    DataTable GetActualAndEstimateHumanInfo(int ProjectID, string beginDate,string endDate, string project, string seq, string process, string estimate, string actual);
    //    DataTable GetActualAndEstimateHumanInfoForProcess(int flowchat, string beginDate, string endDate, string project, string seq, string process, string estimate, string actual);
    //    List<string> GetHumanColumn(int ProjectID, string beginDate, string endDate,int flag, string project, string seq, string process, string estimate, string actual);
    //    List<HumanInfo> GetNowHumanByBG(int bgOrgID, DateTime begindate);
    //    List<HumanInfo> GetDemissionRateByBG(int bgOrgID, DateTime begindate);
    //    List<InputDataForSelectDTO> GetInputDataForSelect(int projectId);

    //    DataTable QueryManPowerRequestRPT(ProductionPlanningReportVM vm);
    //    List<string> GetAllManPowerProject(ProductionPlanningReportVM vm);
    //    DataTable QueryManPowerRequestByProjectOne(ProductionPlanningReportVM vm);
    //    DataTable QueryManPowerRequestByProjectOneByTwo(ProductionPlanningReportVM vm);
    //    DataTable QueryManPowerRequestByProjectOneByThree(ProductionPlanningReportVM vm);
    //    DataTable QueryManPowerRequestByFuncOne(ProductionPlanningReportVM vm);
    //    #endregion

    //    #region 设备需求报表
    //    ProductionPlanningReportGetProject GetProjectList(CustomUserInfoVM vm);
    //    Dictionary<int, string> GetOpTypesByPlantName(string plantName);
    //    Dictionary<int, string> GetProjectByOpType(int OpTypeUID);
    //    Dictionary<int, string> GetPartTypesByProject(int ProjectUID);
    //    Dictionary<int, string> GetFunPlantByOpType(int OpTypeUID); 
    //    List<string> GetRPTColumnName(int PlantUID, int OpTypeUID);
    //    DataTable QueryEquipRPT(ProductionPlanningReportVM item, Page page);
    //    List<ProductionPlanningReportSearchVM> QueryEquipRPTByOPType(ProductionPlanningReportVM item, Page page);
    //    PagedListModel<ReportByProject> QueryEquipRPTBySingleProject(ReportByProject item, Page page);
    //    List<DataTable> QueryEquipRPT_ByProject(ProductionPlanningReportVM item, Page page);
    //    List<string> GetColumn_ByProject(ProductionPlanningReportVM item);
    //    DataTable QueryEquipRPTByFunc(ReportByProject item);
    //    #endregion

    //    #region 实际和预估对比报表
    //    DataTable QueryPlanAndActualReportInfo(ProductionPlanningReportVM item, Page page);
    //    #endregion

    //}

    //public class ProductionPlanningService: IProductionPlanning
    //{
    //    #region Private interfaces properties

    //    private readonly IUnitOfWork unitOfWork;
    //    private readonly IFlowChartDetailRepository flowChartDetailRepository;
    //    private readonly IEnumerationRepository enumerationRepository;
    //    private readonly ISystemOrgRepository systemOrgRepository;
    //    private readonly ISystemOrgBomRepository systemOrgBomRepository;
    //    private readonly ISystemProjectRepository systemProjectRepository;
    //    private readonly IFlowChartMasterRepository flowChartMasterRepository;
    //    //private readonly IFlowChartDetail_IE_Repository flowChartDetail_IE_Repository;
    //    //private readonly IIE_MonitorStaff_Mapping_Banding_Repository iE_MonitorStaff_Mapping_Banding_Repository;
    //    //private readonly IFlowchartMappingRepository iFlowchartMappingRepository;
    //    //private readonly IFlowchartDetailMERepository flowchartDetailMERepository;
    //    //private readonly IPPFlowchartProcessMappingRepository iPPFlowchartProcessMappingRepository;
    //    private readonly ISystemFunctionPlantRepository iSystemFunctionPlantRepository;
    //    private readonly ISystemUserOrgRepository systemUserOrgRepository;
    //    //private readonly IFlowchart_Detail_IE_MonitorStaffRepository flowchart_Detail_IE_MonitorStaffRepository;
    //    //private readonly IProductRequestStaffRepository productRequestStaffRepository;
    //    #endregion //Private interfaces properties

    //    #region Service constructor

    //    public ProductionPlanningService(
    //        IFlowChartDetailRepository FlowChartDetailRepository,
    //        IEnumerationRepository enumerationRepository,
    //        ISystemOrgRepository systemOrgRepository,
    //        ISystemOrgBomRepository systemOrgBomRepository,
    //    ISystemProjectRepository systemProjectRepository,
    //        IFlowChartMasterRepository flowChartMasterRepository,
    //        //IFlowChartDetail_IE_Repository flowChartDetail_IE_Repository,
    //        //IIE_MonitorStaff_Mapping_Banding_Repository iE_MonitorStaff_Mapping_Banding_Repository,
    //        //IFlowchartMappingRepository iFlowchartMappingRepository,
    //        //IFlowchartDetailMERepository flowchartDetailMERepository,
    //        //IPPFlowchartProcessMappingRepository iPPFlowchartProcessMappingRepository,
    //        ISystemFunctionPlantRepository iSystemFunctionPlantRepository,
    //        ISystemUserOrgRepository systemUserOrgRepository,
    //        //IFlowchart_Detail_IE_MonitorStaffRepository flowchart_Detail_IE_MonitorStaffRepository,
    //        //IProductRequestStaffRepository productRequestStaffRepository,
    //    IUnitOfWork unitOfWork)

    //    {
    //        this.flowChartDetailRepository = FlowChartDetailRepository;
    //        this.unitOfWork = unitOfWork;
    //        this.enumerationRepository = enumerationRepository;
    //        this.systemOrgRepository = systemOrgRepository;
    //        this.systemOrgBomRepository = systemOrgBomRepository;
    //        this.systemProjectRepository = systemProjectRepository;
    //        this.flowChartMasterRepository = flowChartMasterRepository;
    //        //this.flowChartDetail_IE_Repository = flowChartDetail_IE_Repository;
    //        //this.iE_MonitorStaff_Mapping_Banding_Repository = iE_MonitorStaff_Mapping_Banding_Repository;
    //        //this.iFlowchartMappingRepository = iFlowchartMappingRepository;
    //        //this.flowchartDetailMERepository = flowchartDetailMERepository;
    //        //this.iPPFlowchartProcessMappingRepository = iPPFlowchartProcessMappingRepository;
    //        this.iSystemFunctionPlantRepository = iSystemFunctionPlantRepository;
    //        this.systemUserOrgRepository = systemUserOrgRepository;
    //        //this.flowchart_Detail_IE_MonitorStaffRepository = flowchart_Detail_IE_MonitorStaffRepository;
    //        //this.productRequestStaffRepository = productRequestStaffRepository;
    //    }

    //    #endregion //Service constructor

    //    #region ME
    //    public PagedListModel<FlowChartModelGet> QueryMEFlowCharts(FlowChartModelSearch searchModel, Page page)
    //    {
    //        var totalCount = 0;
    //        var list = flowChartMasterRepository.GetFlowchartMEList(searchModel, page, out totalCount);
    //        var flCharts = new PagedListModel<FlowChartModelGet>(totalCount, list);
    //        return flCharts;
    //    }


    //    public void ImportFlowchartME(ProductionPlanningModelGetAPIModel importItem, bool isEdit, int accountID)
    //    {
    //        flowChartMasterRepository.ImportFlowchartME(importItem, isEdit, accountID);
    //    }

    //    public string ChangePhase(int FlowChart_Master_UID)
    //    {
    //        var flItem = flowChartMasterRepository.GetById(FlowChart_Master_UID);
    //        //flItem.Product_Phase = StructConstants.Phase.MP;
    //        flItem.CurrentDepartent = StructConstants.CurrentDepartent.IE;
    //        try
    //        {
    //            unitOfWork.Commit();
    //            return string.Empty;
    //        }
    //        catch (Exception ex)
    //        {
    //            return ex.ToString();
    //        }
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


    //    #endregion

    //    #region PP
    //    public PagedListModel<FlowChartModelGet> QueryPPFlowCharts(FlowChartModelSearch searchModel, Page page)
    //    {
    //        var totalCount = 0;
    //        var list = flowChartMasterRepository.GetFlowchartPPList(searchModel, page, out totalCount);
    //        var flCharts = new PagedListModel<FlowChartModelGet>(totalCount, list);
    //        return flCharts;
    //    }

    //    public PagedListModel<PPFlowchartDetailVM> FlowchartPPDetailList(int id, int Version)
    //    {
    //        var list = iFlowchartMappingRepository.FlowchartPPDetailList(id, Version);
    //        return list;
    //    }

    //    public PagedListModel<FlowchartDetailMEDTO> QueryFLDetailAssociationByID(int id)
    //    {
    //        FlowChartDetailGet detailItem = new FlowChartDetailGet();
    //        var data = flowchartDetailMERepository.GetById(id);
    //        var item = AutoMapper.Mapper.Map<FlowchartDetailMEDTO>(data);
    //        //设定i从1开始自增，总共取上下制程10个
    //        int i = 1,total = 10;
    //        List<int> processSqlList = new List<int>();
            
    //        for (; item.Process_Seq - i > 0; i++)
    //        {
    //            if (i > 6)
    //            {
    //                break;
    //            }
    //            var seq = item.Process_Seq - i;
    //            processSqlList.Add(seq);
    //        }

    //        int j = 1;
    //        for (; total - i >= 0; i++)
    //        {
    //            var seq = item.Process_Seq + j;
    //            processSqlList.Add(seq);
    //            j++;
    //        }

    //        var funcList = iSystemFunctionPlantRepository.GetAll().ToList();
    //        var dataList = flowchartDetailMERepository.GetMany(m => m.FlowChart_Master_UID == data.FlowChart_Master_UID && m.FlowChart_Version == data.FlowChart_Version
    //        && processSqlList.Contains(m.Process_Seq)).ToList();
    //        var dtoList = AutoMapper.Mapper.Map<List<FlowchartDetailMEDTO>>(dataList);
    //        foreach (var dtoItem in dtoList)
    //        {
    //            var funcItem = funcList.Where(m => m.System_FunPlant_UID == dtoItem.System_FunPlant_UID).First();
    //            dtoItem.FunPlant = funcItem.FunPlant;
    //        }

    //        var pageList = new PagedListModel<FlowchartDetailMEDTO>(0, dtoList);
    //        return pageList;
    //    }

    //    public FlowchartMappingDTO QueryFLDetail(int id)
    //    {
    //        var item = iFlowchartMappingRepository.GetMany(m => m.Flowchart_Detail_ME_UID == id).FirstOrDefault();
    //        var dto = AutoMapper.Mapper.Map<FlowchartMappingDTO>(item);
    //        return dto;
    //    }

    //    public string CheckProcess(int id, List<int> idList)
    //    {
    //        string errorInfo = string.Empty;
    //        List<string> errorList = new List<string>();
    //        List<int> flMEUIDList = new List<int>();
    //        var hasItem = iFlowchartMappingRepository.GetMany(m => m.Flowchart_Detail_ME_UID == id).FirstOrDefault();
    //        if (hasItem != null)
    //        {
    //            //查找Product_RequestStaff实际人力录入表有没有相关的此制程信息，如果有则不能删除有外键冲突
    //            var staffItem = productRequestStaffRepository.GetMany(m => m.FlowChart_Detail_UID == hasItem.FlowChart_Detail_UID).FirstOrDefault();
    //            if (staffItem != null)
    //            {
    //                errorInfo = "实际人力数维护画面已经有相关合并的制程信息了，不能再次修改制程";
    //                return errorInfo;
    //            }

    //            flMEUIDList = iPPFlowchartProcessMappingRepository.GetMany(m => m.Flowchart_Mapping_UID == hasItem.Flowchart_Mapping_UID).Select(m => m.Flowchart_Detail_ME_UID).ToList();
    //        }
    //        //除了查找以上附属制程外还需要查找关键制程有没有
    //        var item = flowchartDetailMERepository.GetById(id);
    //        var meList = flowchartDetailMERepository.GetMany(m => m.FlowChart_Master_UID == item.FlowChart_Master_UID && m.FlowChart_Version == item.FlowChart_Version).Select(m => m.Flowchart_Detail_ME_UID).ToList();
    //        var hasExistUIDList = iFlowchartMappingRepository.GetMany(m => meList.Contains(m.Flowchart_Detail_ME_UID)).Select(m => m.Flowchart_Detail_ME_UID).ToList();
    //        if (hasItem != null)
    //        {
    //            hasExistUIDList.Remove(hasItem.Flowchart_Detail_ME_UID);
    //        }


    //        //排除掉已有的关联制程
            
    //        foreach (var flMEUIDItem in flMEUIDList)
    //        {
    //            idList.Remove(flMEUIDItem);
    //        }

    //        var processList = iPPFlowchartProcessMappingRepository.GetMany(m => idList.Contains(m.Flowchart_Detail_ME_UID)).ToList();
    //        foreach (var processItem in processList)
    //        {
    //            errorList.Add(string.Format("制程{0}已经被其他制程关联", processItem.Flowchart_Detail_ME.Process_Seq));
    //        }

    //        foreach (var hasExistUIDItem in hasExistUIDList)
    //        {
    //            var exist = idList.Exists(m => m == hasExistUIDItem);
    //            if (exist)
    //            {
    //                errorList.Add(string.Format("不能选择PP制程作为关联制程", exist));
    //            }
    //        }
            
    //        if (errorList.Count() > 0)
    //        {
    //            errorInfo = string.Join(";", errorList);
    //        }
    //        return errorInfo;
    //    }

    //    public List<int> QueryFLProcessCheckedSelectByID(int id)
    //    {
    //        List<int> idList = new List<int>();
    //        var item = iFlowchartMappingRepository.GetMany(m => m.Flowchart_Detail_ME_UID == id).FirstOrDefault();
    //        if (item != null)
    //        {
    //            var list = iPPFlowchartProcessMappingRepository.GetMany(m => m.Flowchart_Mapping_UID == item.Flowchart_Mapping_UID).Select(m => m.Flowchart_Detail_ME_UID).ToList();
    //            idList = list;
    //        }
    //        return idList;
    //    }

    //    public void SavePPFlowchart(FlowChartDetailDTO dto, List<int> idList)
    //    {
    //        iPPFlowchartProcessMappingRepository.SavePPFlowchart(dto, idList);
    //    }

    //    #endregion


    //    #region ---IE

    //    public string JudgeFlowchart(int FlowChart_Master_UID)
    //    {
    //        return flowChartMasterRepository.JudgeFlowchart(FlowChart_Master_UID);
    //    }


    //    public PagedListModel<FLowchart_Detail_IE_VM> QueryFlowChartForIE(int Flowchart_Master_UID,int flag)
    //    {
    //        List<FLowchart_Detail_IE_VM> resultTemp = flowChartDetail_IE_Repository.QueryFlowChartForIE(Flowchart_Master_UID);
    //        //List<Flowchart_Detail_ProductionPlanningListVM> result = new List<Flowchart_Detail_ProductionPlanningListVM>();

    //        //foreach (Flowchart_Detail_ProductionPlanning temp in resultTemp)
    //        //{
    //        //    Flowchart_Detail_ProductionPlanningListVM data = new Flowchart_Detail_ProductionPlanningListVM();
    //        //    data.Automation_Equipment = temp.Automation_Equipment;
    //        //    data.Auxiliary_Equipment = temp.Auxiliary_Equipment;
    //        //    data.Binding_Seq = temp.Binding_Seq;
    //        //    data.Capacity_ByDayVM = temp.Capacity_ByDay==null?"0":decimal.ToInt32(temp.Capacity_ByDay.Value).ToString();
    //        //    data.Capacity_ByHourVM=temp.Capacity_ByHour == null ? "0" : decimal.ToInt32(temp.Capacity_ByHour.Value).ToString();

    //        //    data.Epquipment_CTVM=temp.Epquipment_CT==null? "0" : temp.Epquipment_CT.ToString();
    //        //    data.Setup_TimeVM = temp.Setup_Time == null ? "0" : temp.Setup_Time.ToString();
    //        //    data.Total_CycletimeVM = temp.Total_Cycletime == null ? "0" : temp.Total_Cycletime.ToString();

    //        //    data.Flowchart_Detail_IE_UID = temp.Flowchart_Detail_IE_UID;
    //        //    data.Equipment_RequstQtyVM = temp.Equipment_RequstQty == null ? "0" : decimal.ToInt32(temp.Equipment_RequstQty.Value).ToString();
    //        //    data.Estimate_YieldVM = temp.Estimate_Yield == null ? "0" : (Convert.ToDecimal(temp.Estimate_Yield) * 100).ToString("F2") + "%";
    //        //    data.Flowchart_Detail_ME_UID = temp.Flowchart_Detail_ME_UID;
               
    //        //    data.Flowchart_Master_UID = temp.Flowchart_Master_UID;
    //        //    data.FlowChart_VersionVM = temp.FlowChart_Version.ToString();
    //        //    data.Funplant = temp.Funplant;

    //        //    if(temp.Flowchart_Detail_IE_UID!=null)
    //        //    {
    //        //        data.Manpower_RatioVM = temp.Manpower_Ratio == null ? "0" : temp.Manpower_Ratio.ToString();
    //        //        data.Manpower_TwoShiftVM = temp.Manpower_TwoShift == null ? "0" : temp.Manpower_TwoShift.ToString();
    //        //        data.Match_RuleVM = temp.Match_Rule == null ? "0" : temp.Match_Rule.ToString();
    //        //        data.Match_TypeVM = temp.Match_Type.ToString();
    //        //        data.MaterialKeeper_QtyVM = temp.MaterialKeeper_Qty == null ? "0" : temp.MaterialKeeper_Qty.ToString();
    //        //        data.Monitor_Match_RaitoVM = temp.Monitor_Match_Raito == null ? "0" : temp.Monitor_Match_Raito.ToString();
    //        //        data.Others_QtyVM = temp.Others_Qty == null ? "0" : temp.Others_Qty.ToString();
    //        //        data.RegularOP_QtyVM = temp.RegularOP_Qty == null ? "0" : temp.RegularOP_Qty.ToString();
    //        //        data.Technician_Match_RaitoVM = temp.Technician_Match_Raito == null ? "0" : temp.Technician_Match_Raito.ToString();
    //        //        data.VariableMonitor__QtyVM = temp.VariableMonitor__Qty == null ? "0" : temp.VariableMonitor__Qty.ToString();
    //        //        data.VariableTechnician__QtyVM = temp.VariableTechnician__Qty == null ? "0" : temp.VariableTechnician__Qty.ToString();
    //        //        data.VariationEquipment_RequstQtyVM = temp.VariationEquipment_RequstQty == null ? "0" : temp.VariationEquipment_RequstQty.ToString();
    //        //        data.VariationOP_QtyVM = temp.VariationOP_Qty == null ? "0" : temp.VariationOP_Qty.ToString();
    //        //    }
    //        //    else
    //        //    {
    //        //        data.Manpower_RatioVM = temp.Manpower_Ratio.ToString();
    //        //        data.Manpower_TwoShiftVM =  temp.Manpower_TwoShift.ToString();
    //        //        data.Match_RuleVM = temp.Match_Rule.ToString();
    //        //        data.Match_TypeVM = temp.Match_Type.ToString();

    //        //        data.MaterialKeeper_QtyVM = temp.MaterialKeeper_Qty.ToString();
    //        //        data.Monitor_Match_RaitoVM = temp.Monitor_Match_Raito.ToString();
    //        //        data.Others_QtyVM = temp.Others_Qty.ToString();
    //        //        data.RegularOP_QtyVM =  temp.RegularOP_Qty.ToString();
    //        //        data.Technician_Match_RaitoVM = temp.Technician_Match_Raito.ToString();
    //        //        data.VariableMonitor__QtyVM = temp.VariableMonitor__Qty.ToString();
    //        //        data.VariableTechnician__QtyVM = temp.VariableTechnician__Qty.ToString();
    //        //        data.VariationEquipment_RequstQtyVM =  temp.VariationEquipment_RequstQty.ToString();
    //        //        data.VariationOP_QtyVM = temp.VariationOP_Qty.ToString();
    //        //    }


    //        //    data.Notes = temp.Notes;
    //        //    if (flag == 1)
    //        //    {
    //        //        if (temp.Process.Length > 5)
    //        //        {
    //        //            data.Process = temp.Process.Substring(0, 5);
    //        //        }
    //        //        else
    //        //        {
    //        //            data.Process = temp.Process;
    //        //        }
    //        //    }
    //        //    else
    //        //    {
    //        //        data.Process = temp.Process;
    //        //    }
    //        //    data.Processing_Equipment = temp.Processing_Equipment;
    //        //    data.Process_Station = temp.Process_Station;
    //        //    data.Process_Desc = temp.Process_Desc;
    //        //    data.Process_Seq = temp.Process_Seq;



    //        //    result.Add(data);
    //        //}

    //        return new PagedListModel<FLowchart_Detail_IE_VM>(resultTemp.Count, resultTemp);
    //    }

    //    public List<Flowchart_Detail_IE_VM> QueryFlowChartByRangeNumber(int Flowchart_Detail_ME_UID, int RangeNumber)
    //    {
    //        List<Flowchart_Detail_IE_VM> result = flowChartDetail_IE_Repository.QueryFlowChartByRangeNumber(Flowchart_Detail_ME_UID, RangeNumber);

    //        return result;
    //    }

    //    public string SaveMappingData(Flowchart_Detail_IE_VMList data)
    //    {
    //        return iE_MonitorStaff_Mapping_Banding_Repository.SaveMappingData(data);
    //    }

    //    public string SaveIEFlowchartData(Flowchart_Detail_ProductionPlanningList data)
    //    {
    //        foreach (Flowchart_Detail_ProductionPlanning da in data.DataList)
    //        {
    //            var f = flowchartDetailMERepository.GetById(da.Flowchart_Detail_ME_UID.Value);
    //            decimal tmp = 0;
    //            if (da.Match_Rule == 1)
    //            {
    //                decimal va = f.Equipment_RequstQty + da.VariationEquipment_RequstQty.Value;
    //                tmp = da.Monitor_Match_Raito.Value * va;
    //                tmp = decimal.Ceiling(tmp);
    //                da.SquadLeader_Qty = decimal.ToInt32(tmp) + da.VariableMonitor__Qty.Value;
    //            }
    //            else
    //            {
    //                decimal va = (da.VariationOP_Qty + da.RegularOP_Qty + da.MaterialKeeper_Qty + da.Others_Qty).Value;
    //                tmp = da.Monitor_Match_Raito.Value * va;
    //                tmp = decimal.Ceiling(tmp);
    //                da.SquadLeader_Qty = decimal.ToInt32(tmp) + da.VariableMonitor__Qty.Value;
    //            }
    //            decimal tn = f.Equipment_RequstQty + da.VariationEquipment_RequstQty.Value;
    //            tmp = da.Technician_Match_Raito.Value * tn;
    //            tmp = decimal.Ceiling(tmp);
    //            da.Technician_Qty = decimal.ToInt32(tmp) + da.VariableTechnician__Qty.Value;
    //        }
    //        //return flowChartDetail_IE_Repository.SaveIEFlowchartData(data);
    //        return UpdateIEFlowchartData(data);
    //    }


    //    public string UpdateIEFlowchartData(Flowchart_Detail_ProductionPlanningList data)
    //    {
    //        var item = data.DataList.First();
    //        var flMeItem = flowchartDetailMERepository.GetById(item.Flowchart_Detail_ME_UID.Value);
    //        var ieItem = flowChartDetail_IE_Repository.GetById(item.Flowchart_Detail_IE_UID.Value);
    //        ieItem.VariationOP_Qty = item.VariationOP_Qty;
    //        ieItem.RegularOP_Qty = item.RegularOP_Qty;
    //        ieItem.MaterialKeeper_Qty = item.MaterialKeeper_Qty;
    //        ieItem.SquadLeader_Raito = item.SquadLeader_Raito;
    //        ieItem.SquadLeader_Variable_Qty = item.SquadLeader_Variable_Qty;
    //        ieItem.Technician_Raito = item.Technician_Raito;
    //        ieItem.Technician_Variable_Qty = ieItem.Technician_Variable_Qty;
    //        ieItem.Others_Qty = ieItem.Others_Qty;
    //        ieItem.Notes = ieItem.Notes;
    //        ieItem.Modified_Date = ieItem.Modified_Date;
    //        ieItem.Modified_UID = ieItem.Modified_UID;


    //        switch (item.Match_Rule)
    //        {
    //            case 1:
    //                ieItem.Match_Rule = item.Match_Rule;
    //                var s_Qty = (flMeItem.Equipment_RequstQty + ieItem.VariationEquipment_RequstQty) * ieItem.SquadLeader_Raito + ieItem.SquadLeader_Variable_Qty;
    //                ieItem.SquadLeader_Qty = Convert.ToInt32(s_Qty);
    //                break;
    //            case 2:
    //                ieItem.Match_Rule = item.Match_Rule;
    //                var k_Qty = (ieItem.VariationOP_Qty + ieItem.RegularOP_Qty + ieItem.MaterialKeeper_Qty + ieItem.Others_Qty) * ieItem.SquadLeader_Raito + ieItem.SquadLeader_Variable_Qty;
    //                ieItem.SquadLeader_Qty = Convert.ToInt32(k_Qty);
    //                break;
    //        }

    //        var t_Qty = (flMeItem.Equipment_RequstQty + ieItem.VariationEquipment_RequstQty) * ieItem.Technician_Raito + ieItem.Technician_Variable_Qty;
    //        ieItem.Technician_Qty = Convert.ToInt32(t_Qty);

    //        unitOfWork.Commit();
    //        return "Success";

    //        //foreach (Flowchart_Detail_ProductionPlanning da in data.DataList)
    //        //{
    //        //    var f = flowchartDetailMERepository.GetById(da.Flowchart_Detail_ME_UID.Value);
    //        //    decimal tmp = 0;
    //        //    if (da.Match_Rule == 1)
    //        //    {
    //        //        decimal va = f.Equipment_RequstQty + da.VariationEquipment_RequstQty.Value;
    //        //        tmp = da.Monitor_Match_Raito.Value * va;
    //        //        tmp = decimal.Ceiling(tmp);
    //        //        da.SquadLeader_Qty = decimal.ToInt32(tmp) + da.VariableMonitor__Qty.Value;
    //        //    }
    //        //    else
    //        //    {
    //        //        decimal va = (da.VariationOP_Qty + da.RegularOP_Qty + da.MaterialKeeper_Qty + da.Others_Qty).Value;
    //        //        tmp = da.Monitor_Match_Raito.Value * va;
    //        //        tmp = decimal.Ceiling(tmp);
    //        //        da.SquadLeader_Qty = decimal.ToInt32(tmp) + da.VariableMonitor__Qty.Value;
    //        //    }
    //        //    decimal tn = f.Equipment_RequstQty + da.VariationEquipment_RequstQty.Value;
    //        //    tmp = da.Technician_Match_Raito.Value * tn;
    //        //    tmp = decimal.Ceiling(tmp);
    //        //    da.Technician_Qty = decimal.ToInt32(tmp) + da.VariableTechnician__Qty.Value;

    //        //    var ie = new Flowchart_Detail_IE();
    //        //    if (da.Flowchart_Detail_IE_UID != null&& da.Flowchart_Detail_IE_UID>0)
    //        //    {
    //        //        ie = flowChartDetail_IE_Repository.GetById(da.Flowchart_Detail_IE_UID.Value);
    //        //    }
    //        //    ie.Modified_Date = DateTime.Now;
    //        //    ie.Modified_UID = da.Created_UID;
    //        //    ie.Flowchart_Detail_ME_UID = da.Flowchart_Detail_ME_UID.Value;
    //        //    ie.VariationOP_Qty = da.VariationOP_Qty;
    //        //    ie.RegularOP_Qty = da.RegularOP_Qty;
    //        //    ie.MaterialKeeper_Qty = da.MaterialKeeper_Qty;
    //        //    ie.Others_Qty = da.Others_Qty;
    //        //    ie.Notes = da.Notes;
    //        //    ie.VariationEquipment_RequstQty = da.VariationEquipment_RequstQty;
    //        //    ie.SquadLeader_Qty = da.SquadLeader_Qty;
    //        //    ie.Technician_Qty = da.Technician_Qty;
    //        //    if (da.Flowchart_Detail_IE_UID != null && da.Flowchart_Detail_IE_UID > 0)
    //        //    {
    //        //        flowChartDetail_IE_Repository.Update(ie);
    //        //    }
    //        //    else
    //        //    {
    //        //        ie.Created_UID = da.Created_UID;
    //        //        ie.Created_Date = da.Created_Date;
    //        //        flowChartDetail_IE_Repository.Add(ie);
    //        //    }
    //        //    unitOfWork.Commit();
    //        //    var idList = new List<IE_MonitorStaff_Mapping>();
    //        //    if (da.Flowchart_Detail_IE_UID != null && da.Flowchart_Detail_IE_UID > 0)
    //        //    {
    //        //        idList = iE_MonitorStaff_Mapping_Repository.GetIEMonitorStaffIDByIE_DetailID(da.Flowchart_Detail_IE_UID.Value);
    //        //    }
    //        //    if (idList != null && idList.Count > 0)
    //        //    {
    //        //        foreach (var t in idList)
    //        //        {
    //        //            int Flowchart_Detail_IE_MonitorStaff_UID = t.Flowchart_Detail_IE_MonitorStaff_UID.Value;
    //        //            var MonitorStaff = flowchart_Detail_IE_MonitorStaffRepository.GetById(Flowchart_Detail_IE_MonitorStaff_UID);
    //        //            MonitorStaff.Match_Rule = da.Match_Rule.Value;
    //        //            if (MonitorStaff.Match_Type == 1)
    //        //            {
    //        //                MonitorStaff.Match_Raito = da.Monitor_Match_Raito.Value;
    //        //                MonitorStaff.Variable_Qty =da.VariableMonitor__Qty.Value;
    //        //            }
    //        //            else
    //        //            {
    //        //                MonitorStaff.Match_Raito =da.Technician_Match_Raito.Value;
    //        //                MonitorStaff.Variable_Qty =da.VariableTechnician__Qty.Value;
    //        //            }
    //        //            MonitorStaff.Modified_Date = DateTime.Now;
    //        //            MonitorStaff.Modified_UID = da.Created_UID;
    //        //            flowchart_Detail_IE_MonitorStaffRepository.Update(MonitorStaff);
    //        //            unitOfWork.Commit();
    //        //        }
    //        //    }
    //        //    else
    //        //    {
    //        //        Flowchart_Detail_IE_MonitorStaff MonitorStaff1 = new Flowchart_Detail_IE_MonitorStaff();
    //        //        Flowchart_Detail_IE_MonitorStaff MonitorStaff2 = new Flowchart_Detail_IE_MonitorStaff();
    //        //        MonitorStaff1.Match_Type = 1;
    //        //        MonitorStaff1.Match_Rule = da.Match_Rule.Value;
    //        //        MonitorStaff1.Match_Raito = da.Monitor_Match_Raito.Value;
    //        //        MonitorStaff1.Variable_Qty = da.VariableMonitor__Qty.Value;
    //        //        MonitorStaff1.Modified_Date = DateTime.Now;
    //        //        MonitorStaff1.Modified_UID = da.Created_UID;
    //        //        MonitorStaff1.Created_Date = DateTime.Now;
    //        //        MonitorStaff1.Created_UID = da.Created_UID;
    //        //        flowchart_Detail_IE_MonitorStaffRepository.Add(MonitorStaff1);
    //        //        unitOfWork.Commit();
    //        //        IE_MonitorStaff_Mapping mapp = new IE_MonitorStaff_Mapping();
    //        //        mapp.Flowchart_Detail_IE_MonitorStaff_UID = MonitorStaff1.Flowchart_Detail_IE_MonitorStaff_UID;
    //        //        mapp.Flowchart_Detail_IE_UID = ie.Flowchart_Detail_IE_UID;
    //        //        mapp.Modified_Date = DateTime.Now;
    //        //        mapp.Modified_UID = da.Created_UID;
    //        //        mapp.Created_Date = DateTime.Now;
    //        //        mapp.Created_UID = da.Created_UID;
    //        //        iE_MonitorStaff_Mapping_Repository.Add(mapp);
    //        //        unitOfWork.Commit();

    //        //        MonitorStaff2.Match_Type = 2;
    //        //        MonitorStaff2.Match_Rule = da.Match_Rule.Value;
    //        //        MonitorStaff2.Match_Raito = da.Technician_Match_Raito.Value;
    //        //        MonitorStaff2.Variable_Qty = da.VariableTechnician__Qty.Value;
    //        //        MonitorStaff2.Modified_Date = DateTime.Now;
    //        //        MonitorStaff2.Modified_UID = da.Created_UID;
    //        //        MonitorStaff2.Created_Date = DateTime.Now;
    //        //        MonitorStaff2.Created_UID = da.Created_UID;
    //        //        flowchart_Detail_IE_MonitorStaffRepository.Add(MonitorStaff2);
    //        //        unitOfWork.Commit();
    //        //        IE_MonitorStaff_Mapping mapp1 = new IE_MonitorStaff_Mapping();
    //        //        mapp1.Flowchart_Detail_IE_MonitorStaff_UID = MonitorStaff2.Flowchart_Detail_IE_MonitorStaff_UID;
    //        //        mapp1.Flowchart_Detail_IE_UID = ie.Flowchart_Detail_IE_UID;
    //        //        mapp1.Modified_Date = DateTime.Now;
    //        //        mapp1.Modified_UID = da.Created_UID;
    //        //        mapp1.Created_Date = DateTime.Now;
    //        //        mapp1.Created_UID = da.Created_UID;
    //        //        iE_MonitorStaff_Mapping_Repository.Add(mapp1);
    //        //        unitOfWork.Commit();
    //        //    }
    //        //}
    //        //return "Success";
    //    }

    //    public Flowchart_Detail_ProductionPlanning QueryFlowChartByFlowchart_Detail_ME_UID(int Flowchart_Detail_ME_UID)
    //    {
    //        return flowChartDetail_IE_Repository.QueryFlowChartByFlowchart_Detail_ME_UID(Flowchart_Detail_ME_UID);
    //    }

    //    #endregion

    //    #region 设备需求报表
    //    public ProductionPlanningReportGetProject GetProjectList(CustomUserInfoVM vm)
    //    {
    //        ProductionPlanningReportGetProject projectItem = new ProductionPlanningReportGetProject();
    //        Dictionary<int, string> plantDir = new Dictionary<int, string>();
    //        Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
    //        Dictionary<int, string> funPlantDir = new Dictionary<int, string>();

    //        var plantList = systemOrgRepository.GetMany(m => m.Organization_ID.Contains("1000")).OrderBy(m => m.Organization_ID).ToList();
    //        foreach (var item in plantList)
    //        {
    //            plantDir.Add(item.Organization_UID, item.Organization_Desc);
    //        }

    //        var id = plantDir.First().Key;
    //        var childUIDList = systemOrgBomRepository.GetMany(m => m.ParentOrg_UID == id).Select(m => m.ChildOrg_UID).ToList();
    //        var optypeList = systemOrgRepository.GetMany(m => childUIDList.Contains(m.Organization_UID)).ToList();

    //        foreach (var item in optypeList)
    //        {
    //            opTypeDir.Add(item.Organization_UID, item.Organization_Desc);
    //        }
    //        opTypeDir.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);

    //        var funPlantList = systemOrgRepository.GetChildByParentUID(vm.OPType_OrganizationUIDList.FirstOrDefault());
    //        foreach (var item in funPlantList)
    //        {
    //            funPlantDir.Add(item.Organization_UID, item.Organization_Name);
    //        }

    //        projectItem.plantDir = plantDir;
    //        projectItem.opTypeDir = opTypeDir;
    //        //projectItem.FunPlantDir = funPlantDir;
    //        //projectItem.partTypeDir = partTypeDir;
    //        return projectItem;
    //    }

    //    public Dictionary<int, string> GetOpTypesByPlantName(string id)
    //    {
    //        Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
    //        var uid = Convert.ToInt32(id);
    //        var childUIDList = systemOrgBomRepository.GetMany(m => m.ParentOrg_UID == uid).Select(m => m.ChildOrg_UID).ToList();
    //        var optypeList = systemOrgRepository.GetMany(m => childUIDList.Contains(m.Organization_UID)).ToList();
    //        foreach (var item in optypeList)
    //        {
    //            opTypeDir.Add(item.Organization_UID, item.Organization_Desc);
    //        }
    //        opTypeDir.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
    //        return opTypeDir;
    //    }

    //    public Dictionary<int, string> GetProjectByOpType(int OpTypeUID)
    //    {
    //        Dictionary<int, string> dirList = new Dictionary<int, string>();
    //        dirList.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
    //        var list = systemProjectRepository.GetMany(m => m.Organization_UID == OpTypeUID).ToList();
    //        list.ForEach(m => dirList.Add(m.Project_UID, m.Project_Name));
    //        return dirList;
    //    }

    //    public Dictionary<int, string> GetPartTypesByProject(int ProjectUID)
    //    {
    //        Dictionary<int, string> dirList = new Dictionary<int, string>();
    //        dirList.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
    //        var list = flowChartMasterRepository.GetMany(m => m.Project_UID == ProjectUID).ToList();
    //        list.ForEach(m => dirList.Add(m.FlowChart_Master_UID, m.Part_Types));
    //        return dirList;
    //    }

    //    public Dictionary<int, string> GetFunPlantByOpType(int OpTypeUID)
    //    {
    //        Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
    //        //获取三级PP,PE
    //        var childUIDList = systemOrgBomRepository.GetMany(m => m.ParentOrg_UID == OpTypeUID).Select(m => m.ChildOrg_UID).ToList();
    //        //根据三级获取四级
    //        var funcUIDList = systemOrgBomRepository.GetMany(m => childUIDList.Contains(m.ParentOrg_UID.Value)).Select(m => m.ChildOrg_UID).ToList();
    //        var optypeList = systemOrgRepository.GetMany(m => funcUIDList.Contains(m.Organization_UID)).ToList();
    //        foreach (var item in optypeList)
    //        {
    //            opTypeDir.Add(item.Organization_UID, item.Organization_Name);
    //        }
    //        opTypeDir.Add(StructConstants.ReportStatus.AllKey, StructConstants.ReportStatus.AllValue);
    //        return opTypeDir;

    //    }

    //    public List<string> GetRPTColumnName(int PlantUID, int OpTypeUID)
    //    {
    //        return flowchartDetailMERepository.GetRPTColumnName(PlantUID, OpTypeUID);
    //    }

    //    public DataTable QueryEquipRPT(ProductionPlanningReportVM item, Page page)
    //    {
    //        var totalCount = 0;
    //        var list = flowchartDetailMERepository.QueryEquipRPT(item, out totalCount);
    //        return list;
    //        //return new PagedListModel<ProductionPlanningReportSearchVM>(totalCount,list);
    //    }

    //    public List<ProductionPlanningReportSearchVM> QueryEquipRPTByOPType(ProductionPlanningReportVM item, Page page)
    //    {
    //        var list = flowchartDetailMERepository.QueryEquipRPTByOPType(item);
    //        return list;
    //    }


    //    public PagedListModel<ReportByProject> QueryEquipRPTBySingleProject(ReportByProject item, Page page)
    //    {
    //        var list = flowchartDetailMERepository.QueryEquipRPTBySingleProject(item.Organization_UID, item.System_FunPlant_UID, item.Equipment_Name, item.time);
    //        return new PagedListModel<ReportByProject>(0, list);
    //    }


    //    public List<DataTable> QueryEquipRPT_ByProject(ProductionPlanningReportVM item,Page page)
    //    {
    //        var dt = flowchartDetailMERepository.QueryEquipRPT_ByProject(item);
    //        return dt;
    //    }

    //    public List<string> GetColumn_ByProject(ProductionPlanningReportVM item)
    //    {
    //        var list = flowchartDetailMERepository.GetColumn_ByProject(item);
    //        return list;
    //    }

    //    public DataTable QueryEquipRPTByFunc(ReportByProject item)
    //    {
    //        var dt = flowchartDetailMERepository.QueryEquipRPTByFunc(item);
    //        return dt;
    //    }

    //    public string UpdateFlowchartIEList(List<FLowchart_Detail_IE_VM> ieList)
    //    {
    //        var result = flowChartDetail_IE_Repository.ImportFlowchartIE(ieList);
    //        return result;
    //        //int tagnum = 0;
    //        //try
    //        //{

    //        //    foreach (Flowchart_Detail_ProductionPlanningListVM vm in data.mgDataList)
    //        //    {
    //        //        var ie = new Flowchart_Detail_IE();
    //        //        if (vm.Flowchart_Detail_IE_UID != null)
    //        //        {
    //        //            ie = flowChartDetail_IE_Repository.GetById(vm.Flowchart_Detail_IE_UID.Value);
    //        //        }
    //        //        ie.Modified_Date = DateTime.Now;
    //        //        ie.Modified_UID = vm.Modified_UID;
    //        //        ie.Flowchart_Detail_ME_UID = vm.Flowchart_Detail_ME_UID.Value;
    //        //        ie.VariationOP_Qty = int.Parse(vm.VariationOP_QtyVM);
    //        //        ie.RegularOP_Qty = int.Parse(vm.RegularOP_QtyVM);
    //        //        ie.MaterialKeeper_Qty = int.Parse(vm.MaterialKeeper_QtyVM);
    //        //        ie.Others_Qty = int.Parse(vm.Others_QtyVM);
    //        //        ie.Notes = vm.Notes;
    //        //        //ie.Modified_Date = DateTime.Now;
    //        //        //ie.Modified_UID = vm.Modified_UID;
    //        //        ie.VariationEquipment_RequstQty = int.Parse(vm.VariationEquipment_RequstQtyVM);
    //        //        decimal tmp = 0;
    //        //        if (vm.Match_RuleVM == "1")
    //        //        {
    //        //            decimal va = decimal.Parse(vm.Equipment_RequstQtyVM) + decimal.Parse(vm.VariationEquipment_RequstQtyVM);
    //        //            tmp = decimal.Parse(vm.Monitor_Match_RaitoVM) * va;
    //        //            tmp = decimal.Ceiling(tmp);
    //        //            ie.SquadLeader_Qty = decimal.ToInt32(tmp) + int.Parse(vm.VariableMonitor__QtyVM);
    //        //        }
    //        //        else
    //        //        {
    //        //            decimal va = (ie.VariationOP_Qty + ie.RegularOP_Qty + ie.MaterialKeeper_Qty + ie.Others_Qty).Value;
    //        //            tmp = decimal.Parse(vm.Monitor_Match_RaitoVM) * va;
    //        //            tmp = decimal.Ceiling(tmp);
    //        //            ie.SquadLeader_Qty = decimal.ToInt32(tmp) + int.Parse(vm.VariableMonitor__QtyVM);
    //        //        }
    //        //        decimal tn = (decimal.Parse(vm.Equipment_RequstQtyVM) + decimal.Parse(vm.VariationEquipment_RequstQtyVM));
    //        //        tmp = decimal.Parse(vm.Technician_Match_RaitoVM) * tn;
    //        //        tmp = decimal.Ceiling(tmp);
    //        //        ie.Technician_Qty = decimal.ToInt32(tmp) + int.Parse(vm.VariableTechnician__QtyVM);
    //        //        if (vm.Flowchart_Detail_IE_UID != null)
    //        //        {
    //        //            flowChartDetail_IE_Repository.Update(ie);
    //        //        }
    //        //        else
    //        //        {
    //        //            ie.Created_Date = DateTime.Now;
    //        //            ie.Created_UID = vm.Modified_UID;
    //        //            flowChartDetail_IE_Repository.Add(ie);
    //        //        }
    //        //        unitOfWork.Commit();
    //        //        var idList = new List<IE_MonitorStaff_Mapping>();
    //        //        if (vm.Flowchart_Detail_IE_UID != null)
    //        //        {
    //        //            idList = iE_MonitorStaff_Mapping_Repository.GetIEMonitorStaffIDByIE_DetailID(vm.Flowchart_Detail_IE_UID.Value);
    //        //        }
    //        //        if (idList != null && idList.Count > 0)
    //        //        {
    //        //            foreach (var t in idList)
    //        //            {
    //        //                int Flowchart_Detail_IE_MonitorStaff_UID = t.Flowchart_Detail_IE_MonitorStaff_UID.Value;
    //        //                var MonitorStaff = flowchart_Detail_IE_MonitorStaffRepository.GetById(Flowchart_Detail_IE_MonitorStaff_UID);
    //        //                MonitorStaff.Match_Rule = int.Parse(vm.Match_RuleVM);
    //        //                if (MonitorStaff.Match_Type == 1)
    //        //                {
    //        //                    MonitorStaff.Match_Raito = decimal.Parse(vm.Monitor_Match_RaitoVM);
    //        //                    MonitorStaff.Variable_Qty = int.Parse(vm.VariableMonitor__QtyVM);
    //        //                }
    //        //                else
    //        //                {
    //        //                    MonitorStaff.Match_Raito = decimal.Parse(vm.Technician_Match_RaitoVM);
    //        //                    MonitorStaff.Variable_Qty = int.Parse(vm.VariableTechnician__QtyVM);
    //        //                }
    //        //                MonitorStaff.Modified_Date = DateTime.Now;
    //        //                MonitorStaff.Modified_UID = vm.Modified_UID;
    //        //                flowchart_Detail_IE_MonitorStaffRepository.Update(MonitorStaff);
    //        //                unitOfWork.Commit();
    //        //            }
    //        //        }
    //        //        else
    //        //        {
    //        //            Flowchart_Detail_IE_MonitorStaff MonitorStaff1 = new Flowchart_Detail_IE_MonitorStaff();
    //        //            Flowchart_Detail_IE_MonitorStaff MonitorStaff2 = new Flowchart_Detail_IE_MonitorStaff();
    //        //            MonitorStaff1.Match_Type = 1;
    //        //            MonitorStaff1.Match_Rule = int.Parse(vm.Match_RuleVM);
    //        //            MonitorStaff1.Match_Raito = decimal.Parse(vm.Monitor_Match_RaitoVM);
    //        //            MonitorStaff1.Variable_Qty = int.Parse(vm.VariableMonitor__QtyVM);
    //        //            MonitorStaff1.Modified_Date = DateTime.Now;
    //        //            MonitorStaff1.Modified_UID = vm.Modified_UID;
    //        //            MonitorStaff1.Created_Date = DateTime.Now;
    //        //            MonitorStaff1.Created_UID = vm.Modified_UID;
    //        //            flowchart_Detail_IE_MonitorStaffRepository.Add(MonitorStaff1);
    //        //            unitOfWork.Commit();
    //        //            IE_MonitorStaff_Mapping mapp = new IE_MonitorStaff_Mapping();
    //        //            mapp.Flowchart_Detail_IE_MonitorStaff_UID = MonitorStaff1.Flowchart_Detail_IE_MonitorStaff_UID;
    //        //            mapp.Flowchart_Detail_IE_UID = ie.Flowchart_Detail_IE_UID;
    //        //            mapp.Modified_Date = DateTime.Now;
    //        //            mapp.Modified_UID = vm.Modified_UID;
    //        //            mapp.Created_Date = DateTime.Now;
    //        //            mapp.Created_UID = vm.Modified_UID;
    //        //            iE_MonitorStaff_Mapping_Repository.Add(mapp);
    //        //            unitOfWork.Commit();

    //        //            MonitorStaff2.Match_Type = 2;
    //        //            MonitorStaff2.Match_Rule = int.Parse(vm.Match_RuleVM);
    //        //            MonitorStaff2.Match_Raito = decimal.Parse(vm.Technician_Match_RaitoVM);
    //        //            MonitorStaff2.Variable_Qty = int.Parse(vm.VariableTechnician__QtyVM);
    //        //            MonitorStaff2.Modified_Date = DateTime.Now;
    //        //            MonitorStaff2.Modified_UID = vm.Modified_UID;
    //        //            MonitorStaff2.Created_Date = DateTime.Now;
    //        //            MonitorStaff2.Created_UID = vm.Modified_UID;
    //        //            flowchart_Detail_IE_MonitorStaffRepository.Add(MonitorStaff2);
    //        //            unitOfWork.Commit();
    //        //            IE_MonitorStaff_Mapping mapp1 = new IE_MonitorStaff_Mapping();
    //        //            mapp1.Flowchart_Detail_IE_MonitorStaff_UID = MonitorStaff2.Flowchart_Detail_IE_MonitorStaff_UID;
    //        //            mapp1.Flowchart_Detail_IE_UID = ie.Flowchart_Detail_IE_UID;
    //        //            mapp1.Modified_Date = DateTime.Now;
    //        //            mapp1.Modified_UID = vm.Modified_UID;
    //        //            mapp1.Created_Date = DateTime.Now;
    //        //            mapp1.Created_UID = vm.Modified_UID;
    //        //            iE_MonitorStaff_Mapping_Repository.Add(mapp1);
    //        //            unitOfWork.Commit();
    //        //        }
    //        //        tagnum++;
    //        //    }
    //        //    return "Success";
    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //    int ddd = tagnum;
    //        //    return "Fail";
    //        //}
    //    }

    //    public PagedListModel<HumanResourcesSummaryDTO> GetHumanResourcesSummary(string strWhere)
    //    {
    //        var list=flowChartDetail_IE_Repository.GetHumanResourcesSummary(strWhere);
    //        PagedListModel<HumanResourcesSummaryDTO> pageList = new PagedListModel<HumanResourcesSummaryDTO>(0, list);
    //        return pageList;
    //    }

    //    public List<InputDataForSelectDTO> GetInputDataForSelect(int projectId)
    //    {
    //        return flowChartDetail_IE_Repository.GetInputDataForSelect(projectId);
    //    }

    //    public List<List<PlanDataView>> GetPlanDataByProject(int site, int op, int ProjectID, int PartTypeUID, string begin)
    //    {
    //        //int flowchatId = 0;
    //        //int orgid = 0;
    //        //var f = systemProjectRepository.GetById(ProjectID);
    //        //if (f != null)
    //        //{
    //        //    orgid = f.Organization_UID;
    //        //    var ff = flowChartMasterRepository.GetAll().FirstOrDefault(a => a.Project_UID == ProjectID);
    //        //    flowchatId = ff.FlowChart_Master_UID;
    //        //}
    //        //return flowChartDetail_IE_Repository.GetPlanDataByProject(ProjectID, orgid, flowchatId,begin);
    //        return flowChartDetail_IE_Repository.GetPlanDataByProject(site, op, ProjectID, PartTypeUID, begin);

    //    }

    //    public List<List<PlanDataView>> GetInputDataByProject(int op, int project, string begin)
    //    {
    //        int flowchatId = 0;
    //        var ff = flowChartMasterRepository.GetAll().FirstOrDefault(a => a.Project_UID == project);
    //        if (ff != null)
    //        {
    //            flowchatId = ff.FlowChart_Master_UID;
    //        }
    //        return flowChartDetail_IE_Repository.GetInputDataByProject(project, op, flowchatId, begin);
    //    }

    //    public List<HumanResources> GetHumanResourcesByFunplant(int ProjectID)
    //    {
    //        int flowchatId = 0;
    //        if (ProjectID > 0)
    //        {
    //            var ff = flowChartMasterRepository.GetAll().FirstOrDefault(a => a.Project_UID == ProjectID);
    //            flowchatId = ff.FlowChart_Master_UID;
    //        }
    //        return flowChartDetail_IE_Repository.GetHumanResourcesByFunplant(flowchatId);
    //    }

    //    public List<HumanResources> GetHumanResourcesByProject(int ProjectID)
    //    {
    //        int flowchatId = 0;
    //        if (ProjectID > 0)
    //        {
    //            var ff = flowChartMasterRepository.GetAll().FirstOrDefault(a => a.Project_UID == ProjectID);
    //            flowchatId = ff.FlowChart_Master_UID;
    //        }
    //        return flowChartDetail_IE_Repository.GetHumanResourcesByProject(flowchatId);
    //    }


    //    public DataTable GetActualAndEstimateHumanInfo(int ProjectID, string beginDate, string endDate, string project, string seq, string process, string estimate, string actual)
    //    {
    //        int flowchatId = 0;
    //        if (ProjectID != 0)
    //        {
    //            var ff = flowChartMasterRepository.GetAll().FirstOrDefault(a => a.Project_UID == ProjectID);
    //            flowchatId = ff.FlowChart_Master_UID;
    //        }

    //        return productRequestStaffRepository.GetActualAndEstimateHumanInfo(flowchatId, beginDate,endDate,  project,  seq,  process,  estimate, actual);
    //    }


    //    public DataTable GetActualAndEstimateHumanInfoForProcess(int flowchat, string beginDate, string endDate, string project, string seq, string process, string estimate, string actual)
    //    {

    //        return productRequestStaffRepository.GetActualAndEstimateHumanInfoForProcess(flowchat, beginDate, endDate,  project,  seq, process,  estimate,  actual);
    //    }

    //    public List<string> GetHumanColumn(int ProjectID, string beginDate, string endDate,int flag, string project, string seq, string process, string estimate, string actual)
    //    {
    //        int flowchatId = 0;
    //        if (ProjectID != 0)
    //        {
    //            var ff = flowChartMasterRepository.GetAll().FirstOrDefault(a => a.Project_UID == ProjectID);
    //            flowchatId = ff.FlowChart_Master_UID;
    //        }
    //        return productRequestStaffRepository.GetHumanColumn(beginDate, endDate, flag,  project,  seq,  process, estimate,  actual);
    //    }

    //    public List<HumanInfo> GetNowHumanByBG(int bgOrgID, DateTime begindate)
    //    {
    //        return flowChartDetail_IE_Repository.GetNowHumanByBG(bgOrgID, begindate);
    //    }

    //    public List<HumanInfo> GetDemissionRateByBG(int bgOrgID, DateTime begindate)
    //    {
    //        return flowChartDetail_IE_Repository.GetDemissionRateByBG(bgOrgID, begindate);
    //    }
    //    #endregion

    //    #region 人力需求报表
    //    public List<string> GetAllManPowerProject(ProductionPlanningReportVM vm)
    //    {
    //        var result = flowChartDetail_IE_Repository.GetAllManPowerProject(vm);
    //        return result;

    //    }


    //    public DataTable QueryManPowerRequestRPT(ProductionPlanningReportVM vm)
    //    {
    //        var result = flowChartDetail_IE_Repository.QueryManPowerRequestRPT(vm);
    //        return result;
    //    }

    //    public DataTable QueryManPowerRequestByProjectOne(ProductionPlanningReportVM vm)
    //    {
    //        var result = flowChartDetail_IE_Repository.QueryManPowerRequestByProjectOne(vm);
    //        return result;
    //    }

    //    public DataTable QueryManPowerRequestByProjectOneByTwo(ProductionPlanningReportVM vm)
    //    {
    //        var result = flowChartDetail_IE_Repository.QueryManPowerRequestByProjectOneByTwo(vm);
    //        return result;
    //    }

    //    public DataTable QueryManPowerRequestByProjectOneByThree(ProductionPlanningReportVM vm)
    //    {
    //        var result = flowChartDetail_IE_Repository.QueryManPowerRequestByProjectOneByThree(vm);
    //        return result;
    //    }

    //    public DataTable QueryManPowerRequestByFuncOne(ProductionPlanningReportVM vm)
    //    {
    //        var result = flowChartDetail_IE_Repository.QueryManPowerRequestByFuncOne(vm);
    //        return result;
    //    }

    //    #endregion

    //    #region 实际和预估对比报表
    //    public DataTable QueryPlanAndActualReportInfo(ProductionPlanningReportVM item, Page page)
    //    {
    //        var dt = flowchartDetailMERepository.QueryPlanAndActualReportInfo(item);
    //        return dt;
    //    }
    //    #endregion



    //}
}
