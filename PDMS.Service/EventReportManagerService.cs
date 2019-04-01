using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Model.ViewModels.Settings;
using PDMS.Common.Constants;
using PDMS.Model.EntityDTO;
using System.Transactions;
using System.Text.RegularExpressions;
using System.Data;

namespace PDMS.Service
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface IEventReportManagerService
    {
        List<string> GetFunPlantForChart(string customer, string project, string productphase, string parttypes, int LanguageID);
        List<Enumeration> GetIntervalTime(string PageName, int GetIntervalTime, string OP);
        bool KeyProcessVertify(string ProjcetName, string Part_Types);
        List<string> GetAllCustomer(List<int> userProjectUid, string oporg);
        List<string> GetOpenProject(List<int> orgs);
        List<string> GetAllProjectAPP(string Customer, List<string> currentProject, List<int> orgs);
        List<string> GetAllProject(string Customer, List<string> currentProject, List<int> orgs);
        List<string> GetAllProductPhase(string customer, string project);
        List<string> GetProjectPhaseSource(string customer, string project);
        string GetSelctOP(string customer, string project);
        List<string> GetAllProductPhaseAPP(string project);
        List<string> GetAllPartTypes(string customer, string project, string productphase);
        List<string> GetAllPartTypesAPP(string project, string productphase);
        List<string> GetAllColor(string customer, string project, string productphase, string parttypes);
        List<string> GetAllColorByFM(string optype, string project, string productphase, string parttypes);

        List<string> GetColor(string customer, string project, string productphase, string parttypes);
        List<string> GetColorAPP(string project, string productphase, string parttypes);
        List<string> GetAllColorAPP(string project, string productphase, string parttypes);
        List<string> GetFunPlant(string customer, string project, string productphase, string parttypes, int LanguageID);
        List<string> GetFunPlantAPP(string project, string productphase, string parttypes);
        List<int> GetDayVersion(string customer, string project, string productphase, string parttypes, string day);
        PagedListModel<PPCheckDataItem> QueryPPCheckDatas(PPCheckDataSearch searchModel, Page page, string QueryType);

        //2016-11-22 add by karl 修改以增加无锡厂区可以修改不可用wip数量
        PagedListModel<PPCheckDataItem> QueryNullDatasWIP(PPCheckDataSearch searchModel, Page page);
        int GetSelctMasterUID(string ProjectName, string Part_Types, string Product_Phase, string opType);
        string EditNullWIP(int product_uid, int nullwip_qty, int modifiedUser);
        List<ExportPPCheck_Data> DoExportFunction(ExportSearch search);
        List<ProductLocationItem> QueryProductInputLocation(ProductInputLocationSearch searchModel, Page page);

        /// <summary>
        /// 通过制程序号获取
        /// </summary>
        List<ProductLocationItem> GetPDInputLocationByProSeqAPI(PDByProSeqSearch searchModel, Page page);

        /// <summary>
        /// 导出战情日报表的楼栋详情
        /// </summary>
        /// <param name="search"></param>
        List<ExportPPCheck_Data> ExportFloorDetialDayReport(ReportDataSearch search);

        string EditWipWithZero(ExportSearch search);
        List<string> QueryUserRole(string userid);
        string GetIntervalTime(int type);

        PagedListModel<Daily_ProductReportItem> QueryReportDatas(ReportDataSearch searchModel, Page page,
            string QueryType);
        PagedListModel<Daily_ProductReport> QueryReportDatasInterval(NewProductReportSumSearch searchModel, Page page);
        PagedListModel<Daily_ProductReportSum> QuerySumReportDatas(NewProductReportSumSearch searchModel, Page page);
        PagedListModel<Daily_ProductReportItem> QueryReportDatasAPP(ReportDataSearch searchModel, Page page,
            string QueryType);

        PagedListModel<Daily_ProductReportItem> FirstReportDatas(ReportDataSearch searchModel, Page page,
            string QueryType);

        string EditWIP(PPEditWIP WIP, int modified_UID);
        //string SaveEditWIPAPI(PPEditWIP WIP, int modified_UID);
        string EditWIPNew(PPEditWIP WIP, int modified_UID);
        string SynchronizeMesInfo(MesSyncParam syncParam);

        string EditWIPView(int product_uid, int wip_qty, int wip_old, int wip_add, string comment, int modifiedUser, int nullwip);
        IntervalEnum GetIntervalInfo(string opType);
        PagedListModel<WarningListVM> GetWarningLists(int user_account_uid, List<string> currentProject);
        ProcessDataSearch GetWarningDataByWarningUid(int WarningUid);
        int GetMasterUidByWarningUid(int WarningUid);
        List<string> GetUnacommpolished_Reason();
        List<string> CheckFunPlantDataIsFull(PPCheckDataSearch searchModel);
        List<GetErrorData> CheckProductDataIsFull(PPCheckDataSearch searchModel);

        List<string> GetAlVersion(string customer, string project, string productphase, string parttypes,
            DateTime beginTime, DateTime endTime);

        VersionBeginEndDate GetVersionBeginEndDate(string customer, string project, string productphase,
            string parttypes, int version);

        int GetVersion(string customer, string project, string productphase, string parttypes,
            DateTime referenceDay);

        string CheckWIPNeedUpdate(string Op_Type);
        ProductReportDisplay GetSystemViewColumnList(int Account_UID);
        bool UpdateColumnInfo(int account_UID, int column_Index, bool isDisplay);

        string InsertIPQCData(int product_Uid);
        string CheckMatchFlag(PPCheckDataSearch search);
        List<string> GetProjectByOp(string Optype);
        List<ExportPPCheck_Data> ExportPPCheckData(ExportSearch search);
        int GetPlant(string Project);
        string GetOPByFlowchartMasterUID(int masterUID);
    }

    //根据日期和masterUID查找专案 客户 阶段等信息

    /// <summary>
    /// EventReportManagerService: IEventReportManagerService
    /// </summary>
    public class EventReportManagerService : IEventReportManagerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEnumerationRepository EnumerationRepository;
        private readonly ISystemBUDRepository SystemBUDRepository;
        private readonly ISystemProjectRepository SystemProjectRepository;
        private readonly IFlowChartMasterRepository FlowChartMasterRepository;
        private readonly IFlowChartDetailRepository FlowChartDetailRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IProductInputRepository ProductInputRepository;
        private readonly IProductInputHistoryRepository ProductInputHistoryRepository;
        private readonly IWarningListRepository warningListRepository;
        private readonly IWIPChangeHistoryRepository WIPChangeHistoryRepository;
        private readonly ISystemViewColumnRepository systemViewColumnRepository;
        private readonly ISystemUserViewRepository systemUserViewRepository;
        private readonly IProductInputRepository productInputRepository;
        private readonly IPPForQAInterfaceRepository pPForQAInterfaceRepository;
        private readonly IElectricalBoardDTRepository electricalBoardDtRepository;
        private readonly IFlowChartDetailRepository flowChartDetailRepository;
        private readonly ISystemOrgBomRepository systemOrgBomRepository;
        private readonly IProductInputLocationRepository productInputLocationRepository;
        //private readonly IMesDataSyncReposity MesDataSyncReposity;
        public EventReportManagerService(
            IUnitOfWork unitOfWork,
            IProductInputRepository productInputRepository,
            IEnumerationRepository EnumerationRepository,
            ISystemBUDRepository SystemBUDRepository,
            ISystemProjectRepository SystemProjectRepository,
            IFlowChartMasterRepository FlowChartMasterRepository,
            IFlowChartDetailRepository FlowChartDetailRepository,
            ISystemUserRepository systemUserRepository,
            IProductInputRepository ProductInputRepository,
            IProductInputHistoryRepository ProductInputHistoryRepository,
            IWarningListRepository warningListRepository,
            IWIPChangeHistoryRepository WIPChangeHistoryRepository,
            ISystemViewColumnRepository systemViewColumnRepository,
            IPPForQAInterfaceRepository pPForQAInterfaceRepository,
            ISystemUserViewRepository systemUserViewRepository,
            IElectricalBoardDTRepository electricalBoardDtRepository,
            IFlowChartDetailRepository flowChartDetailRepository,
            ISystemOrgBomRepository systemOrgBomRepository,
            IProductInputLocationRepository productInputLocationRepository
            //IMesDataSyncReposity MesDataSyncReposity
            )
        {
            this.unitOfWork = unitOfWork;
            this.productInputRepository = productInputRepository;
            this.EnumerationRepository = EnumerationRepository;
            this.SystemBUDRepository = SystemBUDRepository;
            this.SystemProjectRepository = SystemProjectRepository;
            this.FlowChartMasterRepository = FlowChartMasterRepository;
            this.FlowChartDetailRepository = FlowChartDetailRepository;
            this.systemUserRepository = systemUserRepository;
            this.ProductInputRepository = ProductInputRepository;
            this.ProductInputHistoryRepository = ProductInputHistoryRepository;
            this.warningListRepository = warningListRepository;
            this.WIPChangeHistoryRepository = WIPChangeHistoryRepository;
            this.systemViewColumnRepository = systemViewColumnRepository;
            this.systemUserViewRepository = systemUserViewRepository;
            this.pPForQAInterfaceRepository = pPForQAInterfaceRepository;
            this.electricalBoardDtRepository = electricalBoardDtRepository;
            this.flowChartDetailRepository = flowChartDetailRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
            this.productInputLocationRepository = productInputLocationRepository;
            //this.MesDataSyncReposity = MesDataSyncReposity;
        }

        #region Add by Rock 2016/03/18---------------------start
        public ProductReportDisplay GetSystemViewColumnList(int Account_UID)
        {
            ProductReportDisplay displayItem = new ProductReportDisplay();
            var columnlist = systemViewColumnRepository.GetMany(m => m.View_Name.ToLower() == StructConstants.ViewColumnMapping.ProductReportDisplay.ToLower())
                .OrderBy(m => m.View_Column_Index).ToList();
            var userViewList = systemUserViewRepository.GetMany(m => m.Account_UID == Account_UID).OrderBy(m => m.View_UID).ToList();

            var dtoList = AutoMapper.Mapper.Map<List<SystemViewColumnDTO>>(columnlist);
            var dtoList2 = AutoMapper.Mapper.Map<List<SystemUserViewDTO>>(userViewList);
            int new_index = 1;
            for (var i = 0; i < dtoList.Count; i++)
            {
                if (dtoList[i].View_Group != "仅用于日报")
                    dtoList[i].new_index = new_index++;
                else
                    dtoList[i].new_index = 100;
            }
            displayItem.ColumnDTOList = dtoList;
            displayItem.ViewDTOList = dtoList2;
            return displayItem;
        }
        public List<string> GetAllProjectAPP(string Customer, List<string> currentProject, List<int> orgs)
        {
            var EnumEntity = SystemProjectRepository.QueryDistinctProjectAPP(Customer, orgs);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            //添加当前用户所在Project
            if (customerList.Any())
            {
                var result = new List<string>();
                foreach (var item in currentProject)
                {
                    var resultItem = new List<string>();
                    //2017-02-13 add by karl 排除所有专案名称中含 beijing字样的
                    resultItem = customerList.Where(m => m.ToString() == item).ToList();
                    //if (!item.ToUpper().Contains("BEIJING"))
                    //{
                    //    resultItem = customerList.Where(m => m.ToString() == item).ToList();
                    //}
                    result.AddRange(resultItem);
                }
                customerList = result;
            }
            return customerList;
        }
        public bool UpdateColumnInfo(int account_UID, int column_Index, bool isDisplay)
        {
            bool result = false;
            var columnItem = systemViewColumnRepository.GetMany(m => m.View_Name.ToLower() == StructConstants.ViewColumnMapping.ProductReportDisplay.ToLower()
            && m.View_Column_Index == column_Index).FirstOrDefault();
            if (columnItem != null)
            {
                var userViewItem = systemUserViewRepository.GetMany(m => m.Account_UID == account_UID && m.Column_UID == columnItem.Column_UID).FirstOrDefault();

                switch (isDisplay)
                {
                    case true:
                        systemUserViewRepository.Delete(userViewItem);
                        unitOfWork.Commit();
                        break;
                    case false:
                        if (userViewItem == null)
                        {
                            System_User_View item = new System_User_View();
                            item.Account_UID = account_UID;
                            item.Column_UID = columnItem.Column_UID;
                            item.Modified_UID = account_UID;
                            item.Modified_Date = DateTime.Now;
                            systemUserViewRepository.Add(item);
                            unitOfWork.Commit();
                        }
                        break;
                }
                return true;
            }
            return result;
        }
        #endregion Add by Rock 2016/03/18---------------------end

        #region  WarningList-----add by Destiny Zhang 2015/12/21

        public PagedListModel<WarningListVM> GetWarningLists(int user_account_uid, List<string> currentProject)
        {
            int totalCount = 0;
            var warningListData = warningListRepository.GetWarninglistDatas(user_account_uid, out totalCount).ToList();

            List<WarningListVM> tempList = new List<WarningListVM>();
            foreach (var VARIABLE in warningListData)
            {
                tempList.Add(new WarningListVM
                {
                    Warning_UID = VARIABLE.Warning_UID,
                    Part_Types = VARIABLE.Part_Types,
                    Product_Date = VARIABLE.Product_Date.ToString("yyyy-M-d"),
                    Product_Phase = VARIABLE.Product_Phase,
                    Project = VARIABLE.Project,
                    Customer = VARIABLE.Customer,
                    FncPlant_Effect = VARIABLE.FncPlant_Effect,
                    Time_Interval = VARIABLE.Time_Interval
                });
            }
            //添加当前用户所在Project
            if (tempList.Any())
            {
                var result = new List<WarningListVM>();
                foreach (var item in currentProject)
                {
                    var resultItem = new List<WarningListVM>();
                    resultItem = tempList.Where(m => m.Project == item).ToList();
                    result.AddRange(resultItem);
                }
                tempList = result;
            }
            return new PagedListModel<WarningListVM>(totalCount, tempList);
        }

        public ProcessDataSearch GetWarningDataByWarningUid(int WarningUid)
        {
            var tempData = warningListRepository.GetWarningDataByWarningUid(WarningUid);
            if (tempData.ToList().Count != 0)
            {
                ProcessDataSearch reuslt = tempData.ToList()[0];
                return reuslt;
            }
            else
            {
                return new ProcessDataSearch();
            }

        }

        public int GetMasterUidByWarningUid(int WarningUid)
        {
            var temp = warningListRepository.GetMasterUidByWarningUid(WarningUid);
            return temp;

        }

        #endregion

        #region 修改无锡厂区的不可用wip数量-----------------------2016-11-22 add by karl 
        public PagedListModel<PPCheckDataItem> QueryNullDatasWIP(PPCheckDataSearch searchModel, Page page)
        {
            var totalCount = 0;

            var PPlist = ProductInputRepository.QueryNullWIPDatas(searchModel, page, out totalCount);
            return new PagedListModel<PPCheckDataItem>(0, PPlist);

        }
        public int GetSelctMasterUID(string ProjectName, string Part_Types, string Product_Phase, string opType)
        {
            return ProductInputRepository.GetSelctMasterUID(ProjectName, Part_Types, Product_Phase, opType);
        }
        public string EditNullWIP(int product_uid, int nullwip_qty, int modifiedUser)
        {
            try
            {
                //新增不可用wip数据
                Product_Input proinput = ProductInputRepository.GetFirstOrDefault(c => c.Product_UID == product_uid);
                proinput.NullWip_QTY = nullwip_qty;
                proinput.Modified_UID = modifiedUser;
                proinput.Modified_Date = DateTime.Now;
                ProductInputRepository.Update(proinput);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch
            { return "FALSE"; }
        }

        public List<ExportPPCheck_Data> DoExportFunction(ExportSearch search)
        {
            return ProductInputRepository.DoExportFunction(search);
        }

        public List<ProductLocationItem> QueryProductInputLocation(ProductInputLocationSearch searchModel, Page page)
        {
            return productInputLocationRepository.QueryProductInputLocation(searchModel, page);
        }

        /// <summary>
        /// 通过制程名字获取
        /// </summary>
        /// <returns></returns>
        public List<ProductLocationItem> GetPDInputLocationByProSeqAPI(PDByProSeqSearch searchModel, Page page)
        {
            return productInputLocationRepository.GetPDInputLocationByProSeqAPI(searchModel, page);
        }

        public string EditWipWithZero(ExportSearch search)
        {
            return ProductInputRepository.EditWipWithZero(search);
        }

        public string GetIntervalTime(int type)
        {
            var result = "";
            var temp = EnumerationRepository.GetSingleIntervalInfo(type);
            var firstOrDefault = temp.FirstOrDefault();
            if (firstOrDefault != null)
            {
                var info = EnumerationRepository.GetMany(m => m.Enum_Value == firstOrDefault.ToString());
                if (info != null && info.Count() > 0)
                {
                    result = info.FirstOrDefault().Enum_Value;
                }
            }
            return result;
        }

        #endregion

        #region Product_Input And Product_Input_History Common Function-------Sidney 2015/12/20
        /// <summary>
        /// 查询Product_Input
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<PPCheckDataItem> QueryPPCheckDatas(PPCheckDataSearch searchModel, Page page, string QueryType)
        {
            string OP = searchModel.OP;
            var nowTimeInfo = GetIntervalInfo(OP);
            searchModel.Interval_Time = nowTimeInfo.Time_Interval;
            searchModel.Reference_Date = Convert.ToDateTime(nowTimeInfo.NowDate);
            var totalCount = 0;
            var betweenDay = DateTime.Now.Day - searchModel.Reference_Date.Day;
            //if (betweenDay > 7)
            //{
            //    var PPlist = ProductInputRepository.QueryHistoryDatas(searchModel, page, out totalCount, QueryType);
            //    return new PagedListModel<PPCheckDataItem>(totalCount, PPlist);
            //}
            //else
            //{
            var PPlist = ProductInputRepository.QueryPpCheckDatas(searchModel, page, out totalCount, QueryType);
            return new PagedListModel<PPCheckDataItem>(0, PPlist);
            //}
        }

        public List<string> GetUnacommpolished_Reason()
        {
            return ProductInputRepository.GetUnacommpolished_Reason();
        }
        public PagedListModel<Daily_ProductReportItem> FirstReportDatas(ReportDataSearch searchModel, Page page, string QueryType)
        {

            var totalCount = 0;
            if (searchModel.Tab_Select_Text == "夜班小计")
                searchModel.Tab_Select_Text = "Night_Sum";
            else if (searchModel.Tab_Select_Text == "白班小计")
                searchModel.Tab_Select_Text = "Daily_Sum";
            else if (searchModel.Tab_Select_Text == "全天")
                searchModel.Tab_Select_Text = "ALL";
            //获取当前时段
            var temp = EnumerationRepository.GetIntervalInfo(searchModel.OP); // Time_InterVal_OP1
            var firstOrDefault = temp.FirstOrDefault();
            if (firstOrDefault != null)
            {
                searchModel.Reference_Date = DateTime.Parse(firstOrDefault.NowDate);
                searchModel.Interval_Time = firstOrDefault.Time_Interval;
                searchModel.Tab_Select_Text = firstOrDefault.Time_Interval;


            }
            var PPlist = ProductInputRepository.QueryAll_ReportData(searchModel, firstOrDefault.Time_Interval, firstOrDefault.NowDate, out totalCount);
            //缓存FlowChart_Detail 表的所有数据。然后遍历缓存表判断IsFloor的值
            SetIsFloor(PPlist);
            return new PagedListModel<Daily_ProductReportItem>(0, PPlist);
        }
        public PagedListModel<Daily_ProductReportSum> QuerySumReportDatas(NewProductReportSumSearch searchModel, Page page)
        {


            var totalCount = 0;
            if (searchModel.Tab_Select_Text == "夜班小计")
                searchModel.Tab_Select_Text = "Night_Sum";
            else if (searchModel.Tab_Select_Text == "白班小计")
                searchModel.Tab_Select_Text = "Daily_Sum";
            else if (searchModel.Tab_Select_Text == "全天")
                searchModel.Tab_Select_Text = "ALL";
            var PPlist = ProductInputRepository.QuerySum_ReportData(searchModel, out totalCount);
            return new PagedListModel<Daily_ProductReportSum>(0, PPlist);
        }

        public PagedListModel<Daily_ProductReport> QueryReportDatasInterval(NewProductReportSumSearch searchModel, Page page)
        {

            var totalCount = 0;
            if (searchModel.Tab_Select_Text == "夜班小计")
                searchModel.Tab_Select_Text = "Night_Sum";
            else if (searchModel.Tab_Select_Text == "白班小计")
                searchModel.Tab_Select_Text = "Daily_Sum";
            else if (searchModel.Tab_Select_Text == "全天")
                searchModel.Tab_Select_Text = "ALL";
            var PPlist = ProductInputRepository.QueryInterval_ReportData(searchModel, out totalCount);
            return new PagedListModel<Daily_ProductReport>(0, PPlist);
        }

        public PagedListModel<Daily_ProductReportItem> QueryReportDatas(ReportDataSearch searchModel, Page page, string QueryType)
        {
            var totalCount = 0;
            if (searchModel.Tab_Select_Text == "夜班小计")
                searchModel.Tab_Select_Text = "Night_Sum";
            else if (searchModel.Tab_Select_Text == "白班小计")
                searchModel.Tab_Select_Text = "Daily_Sum";
            else if (searchModel.Tab_Select_Text == "全天")
                searchModel.Tab_Select_Text = "ALL";
            //获取当前时段
            var temp = EnumerationRepository.GetIntervalInfo(searchModel.OP);
            var firstOrDefault = temp.FirstOrDefault();
            List<Daily_ProductReportItem> items = new List<Daily_ProductReportItem>();
            if (searchModel.Tab_Select_Text != "Night_Sum" && searchModel.Tab_Select_Text != "Daily_Sum" && searchModel.Tab_Select_Text != "ALL")
            {
                items = ProductInputRepository.QueryAll_ReportData1(searchModel, firstOrDefault.Time_Interval, firstOrDefault.NowDate, out totalCount);
            }
            else
            {
                items = ProductInputRepository.QueryAll_ReportData(searchModel, firstOrDefault.Time_Interval, firstOrDefault.NowDate, out totalCount);
            }
            if (searchModel.FunPlant != "ALL")
            {
                items = items.Where(A => A.FunPlant == searchModel.FunPlant).ToList();
            }

            //缓存FlowChart_Detail 表的所有数据。然后遍历缓存表判断IsFloor的值
            SetIsFloor(items);

            //按颜色汇总（0：不是 1：是）
            if (searchModel.IsColour == 1)
            {
                return new PagedListModel<Daily_ProductReportItem>(0, SetIsColour(items, searchModel, out totalCount));
            }
            else
            {
                return new PagedListModel<Daily_ProductReportItem>(0, items);
            }
        }
        /// <summary>
        /// 设置是否有楼栋信息
        /// </summary>
        /// <returns></returns>
        public List<Daily_ProductReportItem> SetIsFloor(List<Daily_ProductReportItem> productReportItem)
        {
            //缓存FlowChart_Detail 表的所有数据。然后遍历缓存表判断IsFloor的值
            // FlowChart_Detail
            var flowChartDetails = flowChartDetailRepository.GetAll().ToList();
            foreach (var item in productReportItem)
            {
                var flowChartDetail = flowChartDetails.Where(o => o.FlowChart_Detail_UID == item.FlowChart_Detail_UID).FirstOrDefault();

                if (flowChartDetail == null)
                {
                    item.IsFloor = false;
                }
                else
                {
                    item.IsFloor = flowChartDetail.Location_Flag;
                }
            }
            return productReportItem;
        }


        public List<Daily_ProductReportItem> SetIsColour(List<Daily_ProductReportItem> productReportItem, ReportDataSearch searchModel, out int totalCount)
        {


            if (searchModel.IsColour == 1)
            {
                ////获取去去重之后的制程列表；IE_TargetEfficacy { get; set; }
       // public int? IE_DeptHuman
                //List<string> productReportColour = productReportItem.Select(o => o.Process).Distinct().ToList();
                //List<Daily_ProductReportItem> productReportColourall = new List<Daily_ProductReportItem>();
                //for (int j = 0; j < productReportColour.Count; j++)
                //{
                //    Daily_ProductReportItem newitem = new Daily_ProductReportItem();
                //    bool fristin = true;
                //    for (int i = 0; i < productReportItem.Count; i++)
                //    {
                //        if(productReportColour[j]== productReportItem[i].Process&& fristin==true)
                //        {
                //            newitem = productReportItem[i];
                //            fristin = false;
                //        }
                //       else if(productReportColour[j] == productReportItem[i].Process && fristin == false)
                //        {
                //        }
                //    }
                //    productReportColourall.Add(newitem);
                //}
                //return productReportColourall;

        var temp = from item in productReportItem
                           group item by item.Process into g
                           select new Daily_ProductReportItem()
                           {
                               Process_Seq = g.Select(o => o.Process_Seq).First(),
                               Place = "",
                               FunPlant = g.Select(o => o.FunPlant).First(),
                               Process = g.Select(o => o.Process).First(),
                               Color = "",
                               DRI = g.Select(o => o.DRI).First(),

                               IE_TargetEfficacy = g.Sum(p => p.IE_TargetEfficacy),
                                 IE_DeptHuman = g.Sum(p => p.IE_DeptHuman),

                               Target_Yield = g.Select(o => o.Target_Yield).First(),
                               All_Product_Plan = g.Sum(o => o.All_Product_Plan),
                               All_Product_Plan_Sum = g.Sum(o => o.All_Product_Plan_Sum),
                               All_Picking_QTY = g.Sum(o => o.All_Picking_QTY),
                               All_WH_Picking_QTY = g.Sum(o => o.All_WH_Picking_QTY),
                               All_Good_QTY = g.Sum(o => o.All_Good_QTY),
                               All_Adjust_QTY = g.Sum(o => o.All_Adjust_QTY),
                               All_WH_QTY = g.Sum(o => o.All_WH_QTY),
                               All_NG_QTY = g.Sum(p => p.All_NG_QTY),
                               Product_Plan = g.Sum(p => p.Product_Plan),
                               Picking_QTY = g.Sum(p => p.Picking_QTY),
                               WH_Picking_QTY = g.Sum(p => p.WH_Picking_QTY),
                               Good_QTY = g.Sum(p => p.Good_QTY),
                               Adjust_QTY = g.Sum(p => p.Adjust_QTY),
                               WH_QTY = g.Sum(p => p.WH_QTY),
                               NG_QTY = g.Sum(p => p.NG_QTY),
                               WIP_QTY = g.Sum(p => p.WIP_QTY == null ? 0 : p.WIP_QTY),
                               NullWIP_QTY = g.Sum(p => p.NullWIP_QTY == null ? 0 : p.NullWIP_QTY),
                               OKWIP_QTY = g.Sum(p => p.OKWIP_QTY == null ? 0 : p.OKWIP_QTY),
                               Proper_WIP = g.Sum(o => o.Proper_WIP),
                               IsFloor = g.Select(p => p.IsFloor).FirstOrDefault(),
                               //FlowChart_Detail_UID= g.Select(p => p.FlowChart_Detail_UID).FirstOrDefault()
                           };

                List<Daily_ProductReportItem> productReportColourall = temp.ToList();
                foreach (var item in productReportColourall)
                {
                    if (item.All_Product_Plan_Sum != 0)
                    {
                        // decimal
                        double all_Rolling_Yield_Rate = (((item.All_Good_QTY + item.All_WH_QTY) * 1.0) / item.All_Product_Plan_Sum) * 100;
                        item.All_Rolling_Yield_Rate = decimal.Parse(all_Rolling_Yield_Rate.ToString("f2"));
                        //item.All_Rolling_Yield_Rate = ((item.All_Good_QTY + item.All_WH_QTY) / item.All_Product_Plan_Sum)*100;
                    }
                    else
                    {
                        item.All_Rolling_Yield_Rate = 100;

                    }
                    if ((item.All_Good_QTY + item.All_WH_QTY + item.All_NG_QTY) != 0)
                    {
                        double all_Finally_Field = (((item.All_Good_QTY + item.All_WH_QTY) * 1.0) / (item.All_Good_QTY + item.All_WH_QTY + item.All_NG_QTY)) * 100;
                        item.All_Finally_Field = decimal.Parse(all_Finally_Field.ToString("f2"));
                        // item.All_Finally_Field = ((item.All_Good_QTY + item.All_WH_QTY) / (item.All_Good_QTY + item.All_WH_QTY + item.All_NG_QTY))*100;
                    }
                    else
                    {
                        item.All_Finally_Field = 100;
                    }

                    if (item.Product_Plan != 0)
                    {
                        double rolling_Yield_Rate = (((item.Good_QTY + item.WH_QTY) * 1.0) / item.Product_Plan) * 100;
                        item.Rolling_Yield_Rate = decimal.Parse(rolling_Yield_Rate.ToString("f2"));
                        // item.Rolling_Yield_Rate = ((item.Good_QTY + item.WH_QTY) / item.Product_Plan)*100;
                    }
                    else
                    {
                        item.Rolling_Yield_Rate = 100;
                    }

                    if ((item.Good_QTY + item.WH_QTY + item.NG_QTY) != 0)
                    {
                        double finally_Field = (((item.Good_QTY + item.WH_QTY) * 1.0) / (item.Good_QTY + item.WH_QTY + item.NG_QTY)) * 100;
                        item.Finally_Field = decimal.Parse(finally_Field.ToString("f2"));
                        // item.Finally_Field =( (item.Good_QTY + item.WH_QTY) / (item.Good_QTY + item.WH_QTY + item.NG_QTY))*100;
                    }
                    else
                    {
                        item.Finally_Field = 100;
                    }


                }
                totalCount = productReportColourall.Count;
                return productReportColourall;

            }
            else
            {
                totalCount = 0;
                return productReportItem;
            }


        }

        public PagedListModel<Daily_ProductReportItem> QueryReportDatasAPP(ReportDataSearch searchModel, Page page, string QueryType)
        {
            var totalCount = 0;
            if (searchModel.Tab_Select_Text == "夜班小计")
                searchModel.Tab_Select_Text = "Night_Sum";
            else if (searchModel.Tab_Select_Text == "白班小计")
                searchModel.Tab_Select_Text = "Daily_Sum";
            else if (searchModel.Tab_Select_Text == "全天")
                searchModel.Tab_Select_Text = "ALL";
            //获取当前时段
            var temp = EnumerationRepository.GetIntervalInfo(searchModel.OP);
            var firstOrDefault = temp.FirstOrDefault();
            var PPlist = ProductInputRepository.QueryAll_ReportDataAPP(searchModel, firstOrDefault.Time_Interval, firstOrDefault.NowDate, out totalCount);
            return new PagedListModel<Daily_ProductReportItem>(0, PPlist);
        }

        /// <summary>
        /// 检查功能厂是否存在数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<string> CheckFunPlantDataIsFull(PPCheckDataSearch searchModel)
        {
            var nowTimeInfo = GetIntervalInfo(searchModel.OP);
            searchModel.Reference_Date = Convert.ToDateTime(nowTimeInfo.NowDate);
            return ProductInputRepository.CheckFunPlantDataIsFull(searchModel);
        }

        public List<GetErrorData> CheckProductDataIsFull(PPCheckDataSearch searchModel)
        {
            var nowTimeInfo = GetIntervalInfo(searchModel.OP);
            searchModel.Reference_Date = Convert.ToDateTime(nowTimeInfo.NowDate);
            return ProductInputRepository.CheckProductDataIsFull(searchModel);
        }

        /// <summary>
        /// 获取时段
        /// </summary>
        /// <param name="PageName"></param>
        /// <returns></returns>
        public List<Enumeration> GetIntervalTime(string PageName, int LanguageID, string OP)
        {
            //获取当前时段及日期
            if (PageName == "PPCheckData")
            {
                List<Enumeration> ppCheck = new List<Enumeration>();
                Enumeration pp1 = new Enumeration();
                Enumeration pp2 = new Enumeration();
                var temp = EnumerationRepository.GetIntervalInfo(OP);
                var firstOrDefault = temp.FirstOrDefault();
                if (firstOrDefault != null) pp1.Enum_Value = firstOrDefault.Time_Interval;

                pp2.Enum_Value = "ALL";
                ppCheck.Add(pp2);
                ppCheck.Add(pp1);
                return ppCheck;
            }
            else
            {
                List<Enumeration> ppCheck = new List<Enumeration>();
                Enumeration en1 = new Enumeration();
                Enumeration en2 = new Enumeration();
                Enumeration en3 = new Enumeration();

                var error1 = LocalizedLanguageCommon.GetLocaleStringResource(LanguageID, "Flowchart.All");
                var error2 = LocalizedLanguageCommon.GetLocaleStringResource(LanguageID, "Flowchart.Daily_Sum");
                var error3 = LocalizedLanguageCommon.GetLocaleStringResource(LanguageID, "Flowchart.Night_Sum");

                en1.Enum_Value = error1;  //Flowchart.All
                en2.Enum_Value = error2;  //Flowchart.Daily_Sum
                en3.Enum_Value = error3;  //Flowchart.Night_Sum
                ppCheck.Add(en1);
                ppCheck.Add(en2);
                ppCheck.Add(en3);
                var EnumEntity = EnumerationRepository.GetIntervalOrder(OP);
                ppCheck.AddRange(EnumEntity);
                return ppCheck;
            }
        }
        /// <summary>
        /// 获取所有的客户
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllCustomer(List<int> userProjectUid, string oporg)
        {
            var EnumEntity = SystemBUDRepository.QueryDistinctCustomer(userProjectUid, oporg);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            return customerList;
        }
        /// <summary>
        /// 获取所有专案
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public List<string> GetAllProject(string Customer, List<string> currentProject, List<int> orgs)
        {
            var EnumEntity = SystemProjectRepository.QueryDistinctProject(Customer, orgs);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            //添加当前用户所在Project
            if (customerList.Any())
            {
                var result = new List<string>();
                foreach (var item in currentProject)
                {
                    var resultItem = new List<string>();
                    //2017-02-13 add by karl 排除所有专案名称中含 beijing字样的
                    resultItem = customerList.Where(m => m.ToString() == item).ToList();
                    //if (!item.ToUpper().Contains("BEIJING"))
                    //{
                    //    resultItem = customerList.Where(m => m.ToString() == item).ToList();
                    //}
                    result.AddRange(resultItem);
                }
                customerList = result;
            }
            return customerList;
        }
        /// <summary>
        /// 获取所有生产阶段
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public List<string> GetAllProductPhase(string customer, string project)
        {
            var EnumEntity = SystemProjectRepository.QueryDistinctProductPhase(customer, project);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            return customerList;
        }

        public List<string> GetProjectPhaseSource(string customer, string project)
        {
            var EnumEntity = SystemProjectRepository.GetProjectPhaseSource(int.Parse(customer), int.Parse(project));
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            return customerList;
        }
        public string GetSelctOP(string customer, string project)
        {
            var EnumEntity = SystemProjectRepository.GetSelctOP(customer, project);

            return EnumEntity;
        }

        public List<string> GetProjectByOp(string Optype)
        {
            return SystemProjectRepository.GetProjectByOp(Optype);
        }

        public List<string> GetAllProductPhaseAPP(string project)
        {
            var EnumEntity = SystemProjectRepository.QueryDistinctProductPhaseAPP(project);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            return customerList;
        }
        /// <summary>
        /// 获取所有的部件
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="project"></param>
        /// <param name="productphase"></param>
        /// <returns></returns>
        public List<string> GetAllPartTypes(string customer, string project, string productphase)
        {
            var EnumEntity = FlowChartMasterRepository.QueryDistinctPartTypes(customer, project, productphase);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            return customerList;
        }

        public List<string> GetAllPartTypesAPP(string project, string productphase)
        {
            var EnumEntity = FlowChartMasterRepository.QueryDistinctPartTypesAPP(project, productphase);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            return customerList;
        }
        /// <summary>
        /// 获取所有颜色
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="project"></param>
        /// <param name="productphase"></param>
        /// <param name="parttypes"></param>
        /// <returns></returns>
        public List<string> GetAllColor(string customer, string project, string productphase, string parttypes)
        {
            List<string> result = new List<string>();
            result.Add("ALL");
            var EnumEntity = FlowChartDetailRepository.QueryDistinctColor(customer, project, productphase, parttypes);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }


        public List<string> GetAllColorByFM(string optype, string project, string productphase, string parttypes)
        {
            List<string> result = new List<string>();
            result.Add("ALL");
            var EnumEntity = FlowChartDetailRepository.GetAllColorByFM(optype, project,productphase,parttypes);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }
        public List<string> GetAllColorAPP(string project, string productphase, string parttypes)
        {
            List<string> result = new List<string>();
            result.Add("ALL");
            var EnumEntity = FlowChartDetailRepository.QueryDistinctColorAPP(project, productphase, parttypes);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }
        public List<string> GetColor(string customer, string project, string productphase, string parttypes)
        {
            List<string> result = new List<string>();

            var EnumEntity = FlowChartDetailRepository.QueryDistinctColor(customer, project, productphase, parttypes);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }
        public List<string> GetColorAPP(string project, string productphase, string parttypes)
        {
            List<string> result = new List<string>();

            var EnumEntity = FlowChartDetailRepository.QueryDistinctColorAPP(project, productphase, parttypes);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }

        public List<int> GetDayVersion(string customer, string project, string productphase, string parttypes, string day)
        {
            var versionList = FlowChartDetailRepository.GetDayVersion(customer, project, productphase, parttypes, day);
            return versionList;
        }
        public List<string> GetFunPlant(string customer, string project, string productphase, string parttypes, int LanguageID)
        {
            List<string> result = new List<string>();
            var KeyProcess = LocalizedLanguageCommon.GetLocaleStringResource(LanguageID, "FlowChart.KeyProcess");
            result.Add(KeyProcess);
            var temp = FlowChartDetailRepository.GetFunPlant(customer, project, productphase, parttypes).ToList();
            //var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(temp);
            return result;
        }

        public List<string> GetFunPlantAPP(string project, string productphase, string parttypes)
        {
            List<string> result = new List<string>();
            result.Add("关键制程");
            var temp = FlowChartDetailRepository.GetFunPlantAPP(project, productphase, parttypes).ToList();
            //var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(temp);
            return result;
        }



        /// <summary>
        /// 获取当前时段及当前日期
        /// </summary>
        /// <param name="opType">OP类型</param>
        /// <returns>包含当前时段及日期的类</returns>
        public IntervalEnum GetIntervalInfo(string opType = "OP1")
        {
            var temp = EnumerationRepository.GetIntervalInfo(opType);
            return temp.FirstOrDefault();
        }

        //private delegate string DelegateName(PPEditWIP WIP, int modified_UID);
        ///// <summary>
        ///// 修改WIP
        ///// </summary>
        ///// <param name="WIP"></param>
        ///// <returns></returns>
        //public string EditWIP(PPEditWIP WIP, int modified_UID)
        //{
        //    List<int> UIDList = new List<int>();
        //    var dateNow = DateTime.Now;
        //    var wiPlist = WIP.PPEditValue.FirstOrDefault();

        //    if (wiPlist != null)
        //    {
        //        try
        //        {
        //            var productUid = wiPlist.Product_UID.ToString();
        //            productUid = productUid.Remove(0, 3);
        //            var productId = Convert.ToInt32(productUid);
        //            var proinput = ProductInputRepository.GetFirstOrDefault(c => c.Product_UID == productId);
        //            var productDate = proinput.Product_Date;
        //            var timeInterval = proinput.Time_Interval;
        //            var flowchartMaster = proinput.FlowChart_Master_UID;
        //            var flowchartVersion = proinput.FlowChart_Version;
        //            //更新isconfrm字段信息
        //            ProductInputRepository.updateConfirmInfo(productDate, timeInterval, flowchartMaster, flowchartVersion);
        //            ProductInputRepository.updateConfirmLocationInfo(productDate, timeInterval, flowchartMaster, flowchartVersion);

        //            //异步执行
        //            DelegateName dn = SaveEditWIPAPI;
        //            IAsyncResult iar = dn.BeginInvoke(WIP, modified_UID,null,null);
        //            return "SUCCESS";
        //        }
        //        catch (Exception e)
        //        {
        //            Logger logger = new Logger("Comfirm数据");
        //            logger.Error("Comfirm数据", e);
        //            return e.ToString();
        //        }

        //    }
        //    else
        //    {
        //        return "WIP Edit False";
        //    }
        //}

        public string EditWIP(PPEditWIP WIP, int modified_UID)
        {

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
new TimeSpan(0, 6, 0)))
            {
                List<int> UIDList = new List<int>();
                var dateNow = DateTime.Now;
                var wiPlist = WIP.PPEditValue.FirstOrDefault();
                if (wiPlist != null)
                {
                    try
                    {
                        var productUid = wiPlist.Product_UID.ToString();
                        productUid = productUid.Remove(0, 3);
                        var productId = Convert.ToInt32(productUid);
                        var proinput = ProductInputRepository.GetFirstOrDefault(c => c.Product_UID == productId);
                        var productDate = proinput.Product_Date;
                        var timeInterval = proinput.Time_Interval;
                        var flowchartMaster = proinput.FlowChart_Master_UID;
                        var flowchartVersion = proinput.FlowChart_Version;
                        var proInputLocationList =
                               productInputLocationRepository.GetMany(
                                   c =>
                                       c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                                       c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();

                        var proInputList =
                            ProductInputRepository.GetMany(
                                c =>
                                    c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                                    c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();
                        //更新isconfrm字段信息
                        ProductInputRepository.updateConfirmInfo(productDate,modified_UID, timeInterval, flowchartMaster, flowchartVersion);
                        ProductInputRepository.updateConfirmLocationInfo(productDate, timeInterval, flowchartMaster, flowchartVersion);

                        //获取所有组装和OQC数据用于存放到中间表
                        var info = ProductInputRepository.GetCustomerInfo(productDate, flowchartMaster);
                        #region
                        if (info.Customer == "ABC")
                        {
                            //var proInputList = ProductInputRepository.getQAProductData(productDate, timeInterval, flowchartMaster, flowchartVersion);
                            //var proInputLocationList = ProductInputRepository.getQAProductLocationData(productDate, timeInterval, flowchartMaster, flowchartVersion);
                            foreach (var item in proInputList)
                            {
                                var QAItem = new PPForQAInterface();
                                //item.Modified_Date = dateNow;
                                //item.Is_Comfirm = true;
                                //item.Modified_UID = modified_UID;
                                //ProductInputRepository.Update(item);
                                //UIDList.Add(item.Product_UID);
                                //将数据插入到PPforQAInterface表
                                //先获去Flowchart_Detal_UID 对应的制程，判断是否为
                                //var FD = flowChartDetailRepository.GetById(item.FlowChart_Detail_UID);
                                //if (!string.IsNullOrWhiteSpace(FD.IsQAProcess) && !FD.IsQAProcess.Contains("IPQC"))
                                //{
                                QAItem.Color = item.Color;
                                QAItem.Create_Date = item.Create_Date;
                                QAItem.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                                QAItem.Good_Qty = item.Good_QTY;
                                QAItem.Input_Qty = item.Picking_QTY + item.WH_Picking_QTY;
                                QAItem.MaterielType = "正常料";
                                QAItem.Modified_Date = item.Modified_Date;
                                QAItem.NG_Qty = item.NG_QTY;
                                QAItem.Product_Date = item.Product_Date;
                                QAItem.QAUsedFlag = false;
                                QAItem.ReWorkQty = item.WH_QTY;  //  借用返工字段存放入库数
                                QAItem.Time_Interval = item.Time_Interval;
                                pPForQAInterfaceRepository.Add(QAItem);
                                //}
                            }
                            foreach (var item in proInputLocationList)
                            {
                                // 先删掉该Item对应Product在PPforQA接口表产生的数据
                                var pi = pPForQAInterfaceRepository.getPrdInfo(item.FlowChart_Detail_UID, item.Product_Date, item.Time_Interval);
                                if (pi != null)
                                {
                                    pPForQAInterfaceRepository.Delete(pi);
                                }
                                var QAItem = new PPForQAInterface();
                                item.Modified_Date = dateNow;
                                item.Is_Comfirm = true;
                                item.Modified_UID = modified_UID;
                                productInputLocationRepository.Update(item);

                                //将数据插入到PPforQAInterface表
                                //先获去Flowchart_Detal_UID 对应的制程，判断是否为
                                //var FD = flowChartDetailRepository.GetById(item.FlowChart_Detail_UID);
                                //if (!string.IsNullOrWhiteSpace(FD.IsQAProcess) && !FD.IsQAProcess.Contains("IPQC"))
                                //{
                                QAItem.Color = item.Color;
                                QAItem.Create_Date = item.Create_Date;
                                QAItem.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                                QAItem.Good_Qty = item.Good_QTY;
                                QAItem.Input_Qty = item.Picking_QTY + item.WH_Picking_QTY;
                                QAItem.MaterielType = "正常料";
                                QAItem.Modified_Date = item.Modified_Date;
                                QAItem.NG_Qty = item.NG_QTY;
                                QAItem.Product_Date = item.Product_Date;
                                QAItem.QAUsedFlag = false;
                                QAItem.ReWorkQty = item.WH_QTY;  //  借用返工字段存放入库数
                                QAItem.Time_Interval = item.Time_Interval;
                                pPForQAInterfaceRepository.Add(QAItem);

                                //}
                            }
                        }
                        #endregion

                        #region 将数据插入到Electrical_Board_DT表中
                        //先删除掉Electrical_Board_DT表中数据
                        var isDeleteSuccess = electricalBoardDtRepository.DeleteElectrical_Board_DT(flowchartMaster);
                        // 汇总数据
                        string OP = SystemProjectRepository.GetSelctOP(info.Customer, info.Project);
                        NewProductReportSumSearch searchModel = new NewProductReportSumSearch();
                        searchModel.Reference_Date = productDate;
                        searchModel.Tab_Select_Text = timeInterval;
                        searchModel.Flowchart_Master_UID = flowchartMaster;
                        searchModel.Color = "ALL";
                        searchModel.OP = OP;
                        searchModel.IsColour = 0;
                        searchModel.FunPlant = "ALL";
                        searchModel.input_day_verion = flowchartVersion;


                        var temp = EnumerationRepository.GetIntervalInfo(OP);
                        var firstOrDefault = temp.FirstOrDefault();
                        int totalCount;

                        //获取不分楼栋全天汇总汇总数据

                        var PPlist = ProductInputRepository.QueryInterval_ReportData(searchModel, out totalCount);
                        List<Electrical_Board_DT> dts = new List<Electrical_Board_DT>();

                        foreach (var item in PPlist)
                        {
                            Electrical_Board_DT dt = new Electrical_Board_DT();
                            dt.Color = item.Color;
                            dt.Project = info.Project;
                            dt.Time_Interval = timeInterval;
                            dt.FunPlant = item.FunPlant;
                            dt.Process = item.Process;
                            dt.Process_Seq = (short)item.Process_Seq;
                            dt.DRI = item.DRI;
                            dt.Prouct_Plan = item.All_Product_Plan_Sum;
                            dt.Target_Yield = (double)item.Target_Yield / 100.00;
                            dt.Picking_QTY = item.All_Picking_QTY;
                            dt.WH_Picking_QTY = item.All_WH_Picking_QTY;
                            dt.Good_QTY = item.All_Good_QTY;
                            dt.Adjust_QTY = item.All_Adjust_QTY;
                            dt.WH_QTY = item.All_WH_QTY;
                            dt.NG_QTY = item.All_NG_QTY;
                            dt.WIP_QTY = int.Parse(item.WIP_QTY == null ? "0" : item.WIP_QTY.ToString());
                            dt.Board_UID = 0;
                            dt.Part_Types = info.Part_Types;
                            dt.FlowChart_Master_UID = flowchartMaster;
                            //dt.Place = Regex.Replace(item.Place, @"[\u4e00-\u9fa5]", "");
                            dt.Place = item.Place;
                            dt.IsDiffLocation = false;
                            dt.Flag = "全天累计";
                            dts.Add(dt);
                        }

                        //全天获取分楼栋的数据汇总
                        var FullDayLoclationList = ProductInputRepository.GetFullDayInputLocaltion(searchModel, out totalCount);
                        foreach (var item in FullDayLoclationList)
                        {
                            Electrical_Board_DT dt = new Electrical_Board_DT();
                            dt.Color = item.Color;//这个颜色的无效
                            dt.Project = info.Project;
                            dt.Time_Interval = timeInterval;
                            dt.FunPlant = item.FunPlant;
                            dt.Process = item.Process;
                            dt.Process_Seq = (short)item.Process_Seq;
                            dt.DRI = item.DRI;
                            dt.Prouct_Plan = item.All_Product_Plan_Sum;
                            dt.Target_Yield = (double)item.Target_Yield / 100.00;
                            dt.Picking_QTY = item.All_Picking_QTY;
                            dt.WH_Picking_QTY = item.All_WH_Picking_QTY;
                            dt.Good_QTY = item.All_Good_QTY;
                            dt.Adjust_QTY = item.All_Adjust_QTY;
                            dt.WH_QTY = item.All_WH_QTY;
                            dt.NG_QTY = item.All_NG_QTY;
                            dt.WIP_QTY = int.Parse(item.WIP_QTY == null ? "0" : item.WIP_QTY.ToString());
                            dt.Board_UID = 0;
                            dt.Part_Types = info.Part_Types;
                            dt.FlowChart_Master_UID = flowchartMaster;
                            //dt.Place = Regex.Replace(item.Place, @"[\u4e00-\u9fa5]", "");
                            dt.Place = item.Place;
                            dt.IsDiffLocation = true;
                            dt.Flag = "全天累计";
                            dts.Add(dt);
                        }


                        // wuxi并且有几个颜色的才需要汇总

                        int countColor = PPlist.FindAll(A => A.Color != "").Distinct().Count();
                    
                            var proInputListAll =
                           ProductInputRepository.GetMany(
                               c =>
                                   c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                                   c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();

                        if (countColor != 0)
                        {
                            var temp1 = from item in PPlist
                                        group item by item.Process into g
                                        select new Electrical_Board_DT()
                                        {
                                            Project = dts[0].Project,
                                            Process_Seq = Convert.ToInt16(g.Select(o => o.Process_Seq).First()),
                                            FunPlant = g.Select(o => o.FunPlant).First(),
                                            Process = g.Select(o => o.Process).First(),
                                            Color = "汇总",
                                            DRI = g.Select(o => o.DRI).First(),
                                            Target_Yield = (double)g.Select(o => o.Target_Yield).First() / 100.00,
                                            Time_Interval = dts[0].Time_Interval,
                                            Prouct_Plan = g.Sum(p => p.All_Product_Plan_Sum),
                                            Picking_QTY = g.Sum(p => p.All_Picking_QTY),
                                            WH_Picking_QTY = g.Sum(p => p.All_WH_Picking_QTY),
                                            Good_QTY = g.Sum(p => p.All_Good_QTY),
                                            Adjust_QTY = g.Sum(p => p.All_Adjust_QTY),
                                            WH_QTY = g.Sum(p => p.All_WH_QTY),
                                            NG_QTY = g.Sum(p => p.All_NG_QTY),
                                            WIP_QTY = g.Sum((p => p.WIP_QTY == null ? 0 : int.Parse(p.WIP_QTY.ToString()))),
                                            Board_UID = 0,
                                            Part_Types = info.Part_Types,
                                            FlowChart_Master_UID = flowchartMaster,
                                            Flag = "全天累计",
                                            Place = "",
                                            IsDiffLocation = false
                                        };
                            dts.AddRange(temp1);
                        }
                        //    if (countColor != 0)
                        //    {
                        //        var temp2 = from item in proInputListAll
                        //                    group item by item.Process into g
                        //                    select new Electrical_Board_DT()
                        //                    {
                        //                        FlowChart_Master_UID = g.Select(o => o.FlowChart_Master_UID).First(),
                        //                        Project = g.Select(o => o.Project).First(),
                        //                        Process_Seq = Convert.ToInt16(g.Select(o => o.Process_Seq).First()),
                        //                        FunPlant = g.Select(o => o.FunPlant).First(),
                        //                        Process = g.Select(o => o.Process).First(),
                        //                        Color = "汇总",
                        //                        DRI = g.Select(o => o.DRI).First(),
                        //                        Target_Yield = (double)g.Select(o => o.Target_Yield).First(),
                        //                        Time_Interval = g.Select(o => o.Time_Interval).First(),
                        //                        Prouct_Plan = g.Sum(p => p.Prouct_Plan),
                        //                        Picking_QTY = g.Sum(p => p.Picking_QTY),
                        //                        WH_Picking_QTY = g.Sum(p => p.WH_Picking_QTY),
                        //                        Good_QTY = g.Sum(p => p.Good_QTY),
                        //                        Adjust_QTY = g.Sum(p => p.Adjust_QTY),
                        //                        WH_QTY = g.Sum(p => p.WH_QTY),
                        //                        NG_QTY = g.Sum(p => p.NG_QTY),
                        //                        WIP_QTY = g.Sum((p => p.WIP_QTY == null ? 0 : int.Parse(p.WIP_QTY.ToString()))),
                        //                        Board_UID = 0,
                        //                        Part_Types = info.Part_Types,
                        //                        Place ="",
                        //                    IsDiffLocation = false,
                        //                    Flag = "全天累计"

                        //};
                        //dts.AddRange(temp2);
                        //    }
                     
                      //  var proInputListAll =
                      //ProductInputRepository.GetMany(
                      //    c =>
                      //        c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                      //        c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();
                        //var proInputListAllDT = AutoMapper.Mapper.Map<List<Electrical_Board_DT>>(proInputListAll);
                        //electricalBoardDtRepository.AddList(proInputListAllDT);
                        //foreach (var item in proInputListAll)
                        //{
                        //    item.Place = Regex.Replace(item.Place, @"[\u4e00-\u9fa5]", "");
                        //}


                        //foreach (var item in IntervalLocationAll)
                        //{
                        //    item.Place = Regex.Replace(item.Place, @"[\u4e00-\u9fa5]", "");
                        //}

                        try
                        {
                            //分楼栋的数据
                            var IntervalLocationAll =
                              productInputLocationRepository.GetMany(
                                  c =>
                                      c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                                      c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();

                            //汇总楼栋的数据
                            var Interval_LocationAll = AutoMapper.Mapper.Map<List<Electrical_Board_DT>>(IntervalLocationAll);
                            foreach (var item in Interval_LocationAll)
                            {
                                item.IsDiffLocation = true;
                            }

                            DataTable dataTable = ConvertTodataTable(Interval_LocationAll);
                            electricalBoardDtRepository.AddElectrical_Board_DT(dataTable);
                            //electricalBoardDtRepository.AddList(Interval_LocationAll);
                        }
                        catch (Exception)
                        {
                        }

                        //不分楼栋的数据(当时段数据--根据成都PP提出的，取消该数据)
                        //var eBoard = AutoMapper.Mapper.Map<List<Electrical_Board_DT>>(proInputListAll);
                        //foreach (var item in eBoard)
                        //{
                        //    item.IsDiffLocation = false;
                        //}
                        //DataTable dt_Board = ConvertTodataTable(eBoard);
                        //electricalBoardDtRepository.AddElectrical_Board_DT(dt_Board);
                        dts = dts.Distinct().ToList();
                        DataTable dt_dts = ConvertTodataTable(dts);
                        electricalBoardDtRepository.AddElectrical_Board_DT(dt_dts);
                        //electricalBoardDtRepository.AddList(eBoard);
                        //electricalBoardDtRepository.AddList(dts);
                        //}
                        #endregion
                        //#endregion
                        unitOfWork.Commit();
                        //Logger log = new Logger("检核生产数据");
                        //var UIDListStr = string.Join(",", UIDList);
                        //var strInfo = string.Format("用户编号：{0},时间：{1},操作：检核生产数据，条数据,UID:{2}", modified_UID, dateNow, UIDListStr);
                        //log.Info(strInfo);
                        scope.Complete();
                        return "SUCCESS";
                        //}
                    }
                    catch (Exception e)
                    {
                        Logger logger = new Logger("Comfirm数据");
                        logger.Error("Comfirm数据", e);
                        return e.ToString();
                    }
                }

                return string.Empty;
            }
        }


  
        /// <summary>
        /// 将对象转换成dataTable
        /// </summary>
        /// <param name="ElectricalList"></param>
        /// <returns></returns>
        public DataTable ConvertTodataTable(List<Electrical_Board_DT> ElectricalList)
        {
            DataTable dataTable = GetTableSchema();
            foreach (var item in ElectricalList)
            {
                DataRow dataOriRow = dataTable.NewRow();
                dataOriRow["Project"] = item.Project;
                dataOriRow["Part_Types"] = item.Part_Types;
                dataOriRow["FlowChart_Master_UID"] = item.FlowChart_Master_UID;
                dataOriRow["Time_Interval"] = item.Time_Interval;
                dataOriRow["FunPlant"] = item.FunPlant;
                dataOriRow["Process"] = item.Process;
                dataOriRow["Process_Seq"] = item.Process_Seq;
                dataOriRow["DRI"] = item.DRI;
                dataOriRow["Prouct_Plan"] = item.Prouct_Plan;
                dataOriRow["Target_Yield"] = item.Target_Yield;
                dataOriRow["Picking_QTY"] = item.Picking_QTY;
                dataOriRow["WH_Picking_QTY"] = item.WH_Picking_QTY;
                dataOriRow["Good_QTY"] = item.Good_QTY;
                dataOriRow["Adjust_QTY"] = item.Adjust_QTY;
                dataOriRow["WH_QTY"] = item.WH_QTY;
                dataOriRow["NG_QTY"] = item.NG_QTY;
                dataOriRow["WIP_QTY"] = item.WIP_QTY;
                dataOriRow["Color"] = item.Color;
                dataOriRow["Flag"] = item.Flag;
                dataOriRow["Place"] = Regex.Replace(item.Place, @"[\u4e00-\u9fa5]", ""); 
                dataOriRow["IsDiffLocation"] = item.IsDiffLocation;
                dataTable.Rows.Add(dataOriRow);
            }

            return dataTable;
        }
        public static DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
            new DataColumn("Project",typeof(string)),
            new DataColumn("Part_Types",typeof(string)),
            new DataColumn("FlowChart_Master_UID",typeof(int)),
            new DataColumn("Time_Interval",typeof(string)),
            new DataColumn("FunPlant",typeof(string)),
            new DataColumn("Process",typeof(string)),
            new DataColumn("Process_Seq",typeof(int)),
            new DataColumn("DRI",typeof(string)),
            new DataColumn("Prouct_Plan",typeof(string)),
            new DataColumn("Target_Yield",typeof(float)),
            new DataColumn("Picking_QTY",typeof(int)),
            new DataColumn("WH_Picking_QTY",typeof(int)),
            new DataColumn("Good_QTY",typeof(int)),
            new DataColumn("Adjust_QTY",typeof(int)),
            new DataColumn("WH_QTY",typeof(int)),
            new DataColumn("NG_QTY",typeof(int)),
            new DataColumn("WIP_QTY",typeof(int)),
            new DataColumn("Color",typeof(string)),
            new DataColumn("Flag",typeof(string)),
            new DataColumn("Place",typeof(string)),
            new DataColumn("IsDiffLocation",typeof(bool)),
            });
            return dt;
        }


        public string SynchronizeMesInfo(MesSyncParam syncParam)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //通过日期时段
                    //1 获取Mes临时表数据
                    //2 匹配ProductInput表的数据
                    //3 更新字段
                    //var mesList = MesDataSyncReposity.GteMesDataInfoByDate(syncParam.currentDate, syncParam.currentInterval);
                    //var sumMesProcessList = mesList.GroupBy(p => p.PIS_ProcessID).ToDictionary(m => m.Key, n => n.ToList());
                    //List<MES_StationDataRecord> MesModelList = new List<MES_StationDataRecord>();
                    //foreach (var item in sumMesProcessList)
                    //{
                    //    MES_StationDataRecord model = new MES_StationDataRecord();
                    //    model.PIS_ProcessID = item.Key;
                    //    model.TimeInterVal = item.Value.FirstOrDefault().TimeInterVal;
                    //    model.Date = item.Value.FirstOrDefault().Date;
                    //    model.ProductQuantity = item.Value.Sum(p => p.ProductQuantity);
                    //    MesModelList.Add(model);
                    //}

                    //ProductInputRepository.updateMesSynsData(MesModelList);
                    return "同步成功";
                }
            }
            catch (Exception)
            {
                return "同步失败";
            }
        }
        public string EditWIPNew(PPEditWIP WIP, int modified_UID)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                List<int> UIDList = new List<int>();
                var dateNow = DateTime.Now;
                var wiPlist = WIP.PPEditValue.FirstOrDefault();
                if (wiPlist != null)
                {
                    try
                    {
                        var productUid = wiPlist.Product_UID.ToString();
                        productUid = productUid.Remove(0, 3);
                        var productId = Convert.ToInt32(productUid);
                        var proinput = ProductInputRepository.GetFirstOrDefault(c => c.Product_UID == productId);
                        var productDate = proinput.Product_Date;
                        var timeInterval = proinput.Time_Interval;
                        var flowchartMaster = proinput.FlowChart_Master_UID;
                        var flowchartVersion = proinput.FlowChart_Version;
                        //var proInputLocationList =
                        //       productInputLocationRepository.GetMany(
                        //           c =>
                        //               c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                        //               c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();

                        //var proInputList =
                        //    ProductInputRepository.GetMany(
                        //        c =>
                        //            c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                        //            c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();

                        //更新isconfrm字段信息
                        ProductInputRepository.updateConfirmInfo(productDate,modified_UID, timeInterval, flowchartMaster, flowchartVersion);
                        ProductInputRepository.updateConfirmLocationInfo(productDate, timeInterval, flowchartMaster, flowchartVersion);
                        //获取所有组装和OQC数据用于存放到中间表
                        //var info = ProductInputRepository.GetCustomerInfo(productDate, flowchartMaster);
                        //if (info.Customer == "ABC")
                        //{
                        //    var proInputList = ProductInputRepository.getQAProductData(productDate, timeInterval, flowchartMaster, flowchartVersion);


                        //    var proInputLocationList = ProductInputRepository.getQAProductLocationData(productDate, timeInterval, flowchartMaster, flowchartVersion);
                        //    foreach (var item in proInputList)
                        //    {

                        //        var QAItem = new PPForQAInterface();
                        //        //item.Modified_Date = dateNow;
                        //        //item.Is_Comfirm = true;
                        //        //item.Modified_UID = modified_UID;
                        //        //ProductInputRepository.Update(item);
                        //        //UIDList.Add(item.Product_UID);
                        //        //将数据插入到PPforQAInterface表
                        //        //先获去Flowchart_Detal_UID 对应的制程，判断是否为
                        //        //var FD = flowChartDetailRepository.GetById(item.FlowChart_Detail_UID);
                        //        //if (!string.IsNullOrWhiteSpace(FD.IsQAProcess) && !FD.IsQAProcess.Contains("IPQC"))
                        //        //{
                        //        QAItem.Color = item.Color;
                        //        QAItem.Create_Date = item.Create_Date;
                        //        QAItem.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                        //        QAItem.Good_Qty = item.Good_QTY;
                        //        QAItem.Input_Qty = item.Picking_QTY + item.WH_Picking_QTY;
                        //        QAItem.MaterielType = "正常料";
                        //        QAItem.Modified_Date = item.Modified_Date;
                        //        QAItem.NG_Qty = item.NG_QTY;
                        //        QAItem.Product_Date = item.Product_Date;
                        //        QAItem.QAUsedFlag = false;
                        //        QAItem.ReWorkQty = item.WH_QTY;  //  借用返工字段存放入库数
                        //        QAItem.Time_Interval = item.Time_Interval;
                        //        pPForQAInterfaceRepository.Add(QAItem);

                        //        //}

                        //    }
                        //    foreach (var item in proInputLocationList)
                        //    {

                        //        // 先删掉该Item对应Product在PPforQA接口表产生的数据
                        //        var pi = pPForQAInterfaceRepository.getPrdInfo(item.FlowChart_Detail_UID, item.Product_Date, item.Time_Interval);
                        //        if (pi != null)
                        //        {
                        //            pPForQAInterfaceRepository.Delete(pi);
                        //        }
                        //        var QAItem = new PPForQAInterface();
                        //        item.Modified_Date = dateNow;
                        //        item.Is_Comfirm = true;
                        //        item.Modified_UID = modified_UID;
                        //        productInputLocationRepository.Update(item);

                        //        //将数据插入到PPforQAInterface表
                        //        //先获去Flowchart_Detal_UID 对应的制程，判断是否为
                        //        //var FD = flowChartDetailRepository.GetById(item.FlowChart_Detail_UID);
                        //        //if (!string.IsNullOrWhiteSpace(FD.IsQAProcess) && !FD.IsQAProcess.Contains("IPQC"))
                        //        //{
                        //        QAItem.Color = item.Color;
                        //        QAItem.Create_Date = item.Create_Date;
                        //        QAItem.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                        //        QAItem.Good_Qty = item.Good_QTY;
                        //        QAItem.Input_Qty = item.Picking_QTY + item.WH_Picking_QTY;
                        //        QAItem.MaterielType = "正常料";
                        //        QAItem.Modified_Date = item.Modified_Date;
                        //        QAItem.NG_Qty = item.NG_QTY;
                        //        QAItem.Product_Date = item.Product_Date;
                        //        QAItem.QAUsedFlag = false;
                        //        QAItem.ReWorkQty = item.WH_QTY;  //  借用返工字段存放入库数
                        //        QAItem.Time_Interval = item.Time_Interval;
                        //        pPForQAInterfaceRepository.Add(QAItem);

                        //        //}
                        //    }
                        //}


                        //#region 将数据插入到Electrical_Board_DT表中
                        ////先删除掉Electrical_Board_DT表中数据
                        //electricalBoardDtRepository.DeleteList(electricalBoardDtRepository.GetMany(m => m.FlowChart_Master_UID == flowchartMaster).ToList());
                        ////Insert Electrical_Board_DT表中数据
                        ////get  汇总数据

                        ////获取当前时段
                        ////获取当前OP
                        //string OP = SystemProjectRepository.GetSelctOP(info.Customer, info.Project);

                        //NewProductReportSumSearch searchModel = new NewProductReportSumSearch();
                        //searchModel.Reference_Date = productDate;
                        //searchModel.Tab_Select_Text =timeInterval;
                        //searchModel.Flowchart_Master_UID = flowchartMaster;
                        //searchModel.Color = "ALL";
                        //searchModel.OP = OP;
                        //searchModel.IsColour = 0;
                        //searchModel.FunPlant = "ALL";
                        //searchModel.input_day_verion = flowchartVersion;


                        //var temp = EnumerationRepository.GetIntervalInfo(OP);
                        //var firstOrDefault = temp.FirstOrDefault();
                        //int totalCount;

                        ////获取汇总数据
                        //var PPlist = ProductInputRepository.QueryInterval_ReportData(searchModel, out totalCount);
                        //List<Electrical_Board_DT> dts = new List<Electrical_Board_DT>();


                        //foreach (var item in PPlist)
                        //{
                        //    Electrical_Board_DT dt = new Electrical_Board_DT();

                        //    dt.Color = item.Color;
                        //    dt.Project = info.Project;
                        //    dt.Time_Interval = timeInterval;
                        //    dt.FunPlant = item.FunPlant;
                        //    dt.Process = item.Process;
                        //    dt.Process_Seq = (short)item.Process_Seq;
                        //    dt.DRI = item.DRI;
                        //    dt.Prouct_Plan = item.All_Product_Plan_Sum;
                        //    dt.Target_Yield = (double)item.Target_Yield / 100.00;
                        //    dt.Picking_QTY = item.All_Picking_QTY;
                        //    dt.WH_Picking_QTY = item.All_WH_Picking_QTY;
                        //    dt.Good_QTY = item.All_Good_QTY;
                        //    dt.Adjust_QTY = item.All_Adjust_QTY;
                        //    dt.WH_QTY = item.All_WH_QTY;
                        //    dt.NG_QTY = item.All_NG_QTY;
                        //    dt.WIP_QTY = int.Parse(item.WIP_QTY == null ? "0" : item.WIP_QTY.ToString());
                        //    dt.Board_UID = 0;
                        //    dt.Part_Types = info.Part_Types;
                        //    dt.FlowChart_Master_UID = flowchartMaster;
                        //    dt.Flag = "全天累计";
                        //    dts.Add(dt);
                        //}




                        //// wuxi并且有几个颜色的才需要汇总

                        //int countColor = PPlist.FindAll(A => A.Color != "").Distinct().Count();
                        //if (info.Customer == "ABC-WUXI")
                        //{
                        //    var proInputListAll =
                        //   ProductInputRepository.GetMany(
                        //       c =>
                        //           c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                        //           c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();


                        //    var temp1 = from item in PPlist
                        //                group item by item.Process into g
                        //                select new Electrical_Board_DT()
                        //                {


                        //                    Project = dts[0].Project,
                        //                    Process_Seq = Convert.ToInt16(g.Select(o => o.Process_Seq).First()),
                        //                    FunPlant = g.Select(o => o.FunPlant).First(),
                        //                    Process = g.Select(o => o.Process).First(),
                        //                    Color = "汇总",
                        //                    DRI = g.Select(o => o.DRI).First(),
                        //                    Target_Yield = (double)g.Select(o => o.Target_Yield).First() / 100.00,
                        //                    Time_Interval = dts[0].Time_Interval,
                        //                    Prouct_Plan = g.Sum(p => p.All_Product_Plan_Sum),
                        //                    Picking_QTY = g.Sum(p => p.All_Picking_QTY),
                        //                    WH_Picking_QTY = g.Sum(p => p.All_WH_Picking_QTY),
                        //                    Good_QTY = g.Sum(p => p.All_Good_QTY),
                        //                    Adjust_QTY = g.Sum(p => p.All_Adjust_QTY),
                        //                    WH_QTY = g.Sum(p => p.All_WH_QTY),
                        //                    NG_QTY = g.Sum(p => p.All_NG_QTY),
                        //                    WIP_QTY = g.Sum((p => p.WIP_QTY == null ? 0 : int.Parse(p.WIP_QTY.ToString()))),
                        //                    Board_UID = 0,
                        //                    Part_Types = info.Part_Types,
                        //                    FlowChart_Master_UID = flowchartMaster,
                        //                    Flag = "全天累计"
                        //                };
                        //    dts.AddRange(temp1);

                        //    if (countColor != 0)
                        //    {
                        //        var temp2 = from item in proInputListAll
                        //                    group item by item.Process into g
                        //                    select new Electrical_Board_DT()
                        //                    {
                        //                        FlowChart_Master_UID = g.Select(o => o.FlowChart_Master_UID).First(),
                        //                        Project = g.Select(o => o.Project).First(),
                        //                        Process_Seq = Convert.ToInt16(g.Select(o => o.Process_Seq).First()),
                        //                        FunPlant = g.Select(o => o.FunPlant).First(),
                        //                        Process = g.Select(o => o.Process).First(),
                        //                        Color = "汇总",
                        //                        DRI = g.Select(o => o.DRI).First(),
                        //                        Target_Yield = (double)g.Select(o => o.Target_Yield).First(),
                        //                        Time_Interval = g.Select(o => o.Time_Interval).First(),
                        //                        Prouct_Plan = g.Sum(p => p.Prouct_Plan),
                        //                        Picking_QTY = g.Sum(p => p.Picking_QTY),
                        //                        WH_Picking_QTY = g.Sum(p => p.WH_Picking_QTY),
                        //                        Good_QTY = g.Sum(p => p.Good_QTY),
                        //                        Adjust_QTY = g.Sum(p => p.Adjust_QTY),
                        //                        WH_QTY = g.Sum(p => p.WH_QTY),
                        //                        NG_QTY = g.Sum(p => p.NG_QTY),
                        //                        WIP_QTY = g.Sum((p => p.WIP_QTY == null ? 0 : int.Parse(p.WIP_QTY.ToString()))),
                        //                        Board_UID = 0,
                        //                        Part_Types = info.Part_Types,


                        //                    };
                        //        dts.AddRange(temp2);
                        //    }
                        //    var eBoard = AutoMapper.Mapper.Map<List<Electrical_Board_DT>>(proInputListAll);


                        //    electricalBoardDtRepository.AddList(eBoard);


                        //}

                        //else
                        //{
                        //    var proInputListAll =
                        //      ProductInputRepository.GetMany(
                        //          c =>
                        //              c.Product_Date == productDate && c.Time_Interval == timeInterval &&
                        //              c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();

                        //    var eBoard = AutoMapper.Mapper.Map<List<Electrical_Board_DT>>(proInputListAll);

                        //    electricalBoardDtRepository.AddList(eBoard);

                        //}
                        //#endregion
                        //electricalBoardDtRepository.AddList(dts);


                        //#endregion
                        unitOfWork.Commit();




                        //Logger log = new Logger("检核生产数据");
                        //var UIDListStr = string.Join(",", UIDList);
                        //var strInfo = string.Format("用户编号：{0},时间：{1},操作：检核生产数据，条数据,UID:{2}", modified_UID, dateNow, UIDListStr);
                        //log.Info(strInfo);
                        scope.Complete();
                        return "SUCCESS";
                    }
                    catch (Exception e)
                    {
                        Logger logger = new Logger("Comfirm数据");
                        logger.Error("Comfirm数据", e);
                        return e.ToString();
                    }

                }
                else
                {
                    return "WIP Edit False";
                }
            }

        }
        //public string EditWIP(PPEditWIP WIP, int modified_UID)
        //{
        //    using (TransactionScope scope = new TransactionScope())
        //    {
        //        List<int> UIDList = new List<int>();
        //        var dateNow = DateTime.Now;
        //        var wiPlist = WIP.PPEditValue.FirstOrDefault();
        //        if (wiPlist != null)
        //        {
        //            try
        //            {
        //                var productUid = wiPlist.Product_UID.ToString();
        //                productUid = productUid.Remove(0, 3);
        //                var productId = Convert.ToInt32(productUid);
        //                var proinput = ProductInputRepository.GetFirstOrDefault(c => c.Product_UID == productId);
        //                var productDate = proinput.Product_Date;
        //                var timeInterval = proinput.Time_Interval;
        //                var flowchartMaster = proinput.FlowChart_Master_UID;
        //                var flowchartVersion = proinput.FlowChart_Version;
        //                var proInputLocationList =
        //                      productInputLocationRepository.GetMany(
        //                          c =>
        //                              c.Product_Date == productDate && c.Time_Interval == timeInterval &&
        //                              c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();

        //                var proInputList =
        //                    ProductInputRepository.GetMany(
        //                        c =>
        //                            c.Product_Date == productDate && c.Time_Interval == timeInterval &&
        //                            c.FlowChart_Master_UID == flowchartMaster && c.FlowChart_Version == flowchartVersion).ToList();
        //                foreach (var item in proInputList)
        //                {

        //                    var QAItem = new PPForQAInterface();
        //                    item.Modified_Date = dateNow;
        //                    item.Is_Comfirm = true;
        //                    item.Modified_UID = modified_UID;
        //                    ProductInputRepository.Update(item);
        //                    UIDList.Add(item.Product_UID);
        //                    //将数据插入到PPforQAInterface表
        //                    //先获去Flowchart_Detal_UID 对应的制程，判断是否为
        //                    var FD = flowChartDetailRepository.GetById(item.FlowChart_Detail_UID);
        //                    if (!string.IsNullOrWhiteSpace(FD.IsQAProcess) && !FD.IsQAProcess.Contains("IPQC"))
        //                    {
        //                        QAItem.Color = item.Color;
        //                        QAItem.Create_Date = item.Create_Date;
        //                        QAItem.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
        //                        QAItem.Good_Qty = item.Good_QTY;
        //                        QAItem.Input_Qty = item.Picking_QTY + item.WH_Picking_QTY;
        //                        QAItem.MaterielType = "正常料";
        //                        QAItem.Modified_Date = item.Modified_Date;
        //                        QAItem.NG_Qty = item.NG_QTY;
        //                        QAItem.Product_Date = item.Product_Date;
        //                        QAItem.QAUsedFlag = false;
        //                        QAItem.ReWorkQty = item.WH_QTY;  //  借用返工字段存放入库数
        //                        QAItem.Time_Interval = item.Time_Interval;
        //                        pPForQAInterfaceRepository.Add(QAItem);

        //                    }

        //                }
        //             //   unitOfWork.Commit();
        //                //foreach (var item in proInputLocationList)
        //                //{

        //                //   // 先删掉该Item对应Product在PPforQA接口表产生的数据
        //                //    var pi = pPForQAInterfaceRepository.getPrdInfo(item.FlowChart_Detail_UID, item.Product_Date, item.Time_Interval);
        //                //    if (pi != null)
        //                //    {
        //                //        pPForQAInterfaceRepository.Delete(pi);
        //                //    }
        //                //    var QAItem = new PPForQAInterface();
        //                //    item.Modified_Date = dateNow;
        //                //    item.Is_Comfirm = true;
        //                //    item.Modified_UID = modified_UID;
        //                //    productInputLocationRepository.Update(item);

        //                //    //将数据插入到PPforQAInterface表
        //                //    //先获去Flowchart_Detal_UID 对应的制程，判断是否为
        //                //    //var FD = flowChartDetailRepository.GetById(item.FlowChart_Detail_UID);
        //                //    //if (!string.IsNullOrWhiteSpace(FD.IsQAProcess) && !FD.IsQAProcess.Contains("IPQC"))
        //                //    //{
        //                //    //    QAItem.Color = item.Color;
        //                //    //    QAItem.Create_Date = item.Create_Date;
        //                //    //    QAItem.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
        //                //    //    QAItem.Good_Qty = item.Good_QTY;
        //                //    //    QAItem.Input_Qty = item.Picking_QTY + item.WH_Picking_QTY;
        //                //    //    QAItem.MaterielType = "正常料";
        //                //    //    QAItem.Modified_Date = item.Modified_Date;
        //                //    //    QAItem.NG_Qty = item.NG_QTY;
        //                //    //    QAItem.Product_Date = item.Product_Date;
        //                //    //    QAItem.QAUsedFlag = false;
        //                //    //    QAItem.ReWorkQty = item.WH_QTY;  //  借用返工字段存放入库数
        //                //    //    QAItem.Time_Interval = item.Time_Interval;
        //                //    //    pPForQAInterfaceRepository.Add(QAItem);

        //                //    //}

        //                //}



        //                //#region 将数据插入到Electrical_Board_DT表中
        //                ////先删除掉Electrical_Board_DT表中数据
        //                electricalBoardDtRepository.DeleteList(electricalBoardDtRepository.GetMany(m => m.FlowChart_Master_UID == flowchartMaster).ToList());
        //                //Insert Electrical_Board_DT表中数据
        //                //get  汇总数据
        //                //unitOfWork.Commit();
        //                var info = ProductInputRepository.GetCustomerInfo(productDate, flowchartMaster);
        //                if (info.Customer == "ABC")
        //                {
        //                    ReportDataSearch searchModel = new ReportDataSearch();
        //                    searchModel.Reference_Date = productDate;
        //                    searchModel.Customer = info.Customer;
        //                    searchModel.Interval_Time = timeInterval;
        //                    searchModel.Tab_Select_Text = "ALL";
        //                    searchModel.Color = "ALL";
        //                    searchModel.Product_Phase = info.Product_Phase;
        //                    searchModel.Part_Types = info.Part_Types;
        //                    searchModel.Project = info.Project;
        //                    searchModel.input_day_verion = flowchartVersion;
        //                    //获取当前时段
        //                    var temp = EnumerationRepository.GetIntervalInfo("OP1");
        //                    var firstOrDefault = temp.FirstOrDefault();
        //                    int totalCount;
        //                    var PPlist = ProductInputRepository.QueryAll_ReportData(searchModel, firstOrDefault.Time_Interval, firstOrDefault.NowDate, out totalCount);
        //                    List<Electrical_Board_DT> dts = new List<Electrical_Board_DT>();
        //                    foreach (var item in PPlist)
        //                    {
        //                        Electrical_Board_DT dt = new Electrical_Board_DT();

        //                        dt.Color = item.Color;
        //                        dt.Project = info.Project;
        //                        dt.Time_Interval = timeInterval;
        //                        dt.FunPlant = item.FunPlant;
        //                        dt.Process = item.Process;
        //                        dt.Process_Seq = (short)item.Process_Seq;
        //                        dt.DRI = item.DRI;
        //                        dt.Prouct_Plan = item.All_Product_Plan_Sum;
        //                        dt.Target_Yield = (double)item.Target_Yield / 100.00;
        //                        dt.Picking_QTY = item.All_Picking_QTY;
        //                        dt.WH_Picking_QTY = item.All_WH_Picking_QTY;
        //                        dt.Good_QTY = item.All_Good_QTY;
        //                        dt.Adjust_QTY = item.All_Adjust_QTY;
        //                        dt.WH_QTY = item.All_WH_QTY;
        //                        dt.NG_QTY = item.All_NG_QTY;
        //                        dt.WIP_QTY = int.Parse(item.WIP_QTY == null ? "0" : item.WIP_QTY.ToString());
        //                        dt.Board_UID = 0;
        //                        dt.Part_Types = info.Part_Types;
        //                        dt.FlowChart_Master_UID = flowchartMaster;
        //                        dt.Flag = "全天累计";
        //                        dts.Add(dt);
        //                    }
        //                    electricalBoardDtRepository.AddList(dts);
        //                }
        //                // 汇总数据
        //                var eBoard = AutoMapper.Mapper.Map<List<Electrical_Board_DT>>(proInputList);

        //                electricalBoardDtRepository.AddList(eBoard);


        //                //#endregion
        //                unitOfWork.Commit();




        //                //Logger log = new Logger("检核生产数据");
        //                //var UIDListStr = string.Join(",", UIDList);
        //                //var strInfo = string.Format("用户编号：{0},时间：{1},操作：检核生产数据，此次保存了{2}条数据,UID:{3}", modified_UID, dateNow, proInputList.Count(), UIDListStr);
        //                //log.Info(strInfo);
        //                scope.Complete();
        //                return "SUCCESS";
        //            }
        //            catch (Exception e)
        //            {
        //                Logger logger = new Logger("Comfirm数据");
        //                logger.Error("Comfirm数据", e);
        //                return e.ToString();
        //            }

        //        }
        //        else
        //        {
        //            return "WIP Edit False";
        //        }
        //    }

        //}
        public string EditWIPView(int product_uid, int wip_qty, int wip_old, int wip_add, string comment,
            int modifiedUser, int nullwip)
        {

            try
            {
                //修改Product_Input表的WIP
                Product_Input proinput = ProductInputRepository.GetFirstOrDefault(c => c.Product_UID == product_uid);
                proinput.WIP_QTY = wip_qty;
                proinput.NullWip_QTY = nullwip;
                ProductInputRepository.Update(proinput);

                if (wip_add != 0)
                {
                    //修改Detail表的WIP_QTY
                    var detail =
                        FlowChartDetailRepository.GetFirstOrDefault(
                            c => c.FlowChart_Detail_UID == proinput.FlowChart_Detail_UID);
                    detail.WIP_QTY = wip_qty;
                    FlowChartDetailRepository.Update(detail);
                    //插入到WIP_Change_Hitory表中
                    WIP_Change_History item = new WIP_Change_History();
                    item.FlowChart_Detail_UID = detail.FlowChart_Detail_UID;
                    item.Change_Type = "AbNormal";
                    item.Comment = comment;
                    item.Modified_Date = DateTime.Now;
                    item.Modified_UID = modifiedUser;
                    item.WIP_Add = wip_add;
                    item.WIP_Old = wip_old;
                    item.Product_UID = product_uid;
                    WIPChangeHistoryRepository.Add(item);
                }

                unitOfWork.Commit();

                return "SUCCESS";
            }
            catch (Exception e)
            { return "FALSE"; }
        }

        public string CheckWIPNeedUpdate(string Op_Type)
        {
            var test = EnumerationRepository.GetFirstOrDefault(p => p.Enum_Type == "WIPChangeFlag" && p.Enum_Name == Op_Type).Enum_Value;
            return test;
        }

        #endregion
        #region Day Week Month Report Function------------------------Sidney 2016/01/28
        public List<string> GetAlVersion(string customer, string project, string productphase, string parttypes, DateTime beginTime, DateTime endTime)
        {
            List<string> result = new List<string>();
            var EnumEntity = FlowChartDetailRepository.QueryDistinctVersion(customer, project, productphase, parttypes, beginTime, endTime);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }

        public int GetVersion(string customer, string project, string productphase, string parttypes, DateTime referenceDay)
        {

            return FlowChartDetailRepository.QueryVersion(customer, project, productphase, parttypes, referenceDay);

        }
        public VersionBeginEndDate GetVersionBeginEndDate(string customer, string project, string productphase, string parttypes, int version)
        {
            var EnumEntity = FlowChartDetailRepository.GetVersionBeginEndDate(customer, project, productphase, parttypes, version);
            return EnumEntity;
        }
        #endregion

        /// <summary>
        /// 插入IPQC的数据至中间表PPForQAInterface
        /// </summary>
        /// <param name="Producr_Date"></param>
        /// <param name="Time_Interval"></param>
        /// <param name="Master_UID"></param>
        /// <param name="Color"></param>
        /// <returns></returns>
        public string InsertIPQCData(int product_Uid)
        {
            var temp = EnumerationRepository.GetIntervalInfo("OP1").FirstOrDefault();
            var Product_Date = temp.NowDate;
            var Time_Interval = temp.Time_Interval;
            int Master_UID = productInputRepository.GetById(product_Uid).FlowChart_Master_UID;
            try
            {
                //插入IPQC巡检的数据
                var IPQCOper = pPForQAInterfaceRepository.GetIPQCOper(Master_UID);
                var PPForQAList = new List<PPForQAInterface>();
                foreach (var item in IPQCOper)
                {
                    var IPQC = pPForQAInterfaceRepository.GetPPForQAInterface(item.BeforeSeq, item.AfterSeq, Product_Date,
                        Time_Interval, Master_UID);
                    foreach (var item1 in IPQC)
                    {
                        var result = new PPForQAInterface();
                        result.Color = item1.Color;
                        result.Create_Date = item1.Create_Date;
                        result.FlowChart_Detail_UID = item1.FlowChart_Detail_UID;
                        result.Input_Qty = item1.Input_Qty;
                        result.MaterielType = item1.MaterielType;
                        result.Modified_Date = item1.Modified_Date;
                        result.NG_Qty = item1.NG_Qty;
                        result.Product_Date = item1.Product_Date;
                        result.QAUsedFlag = item1.QAUsedFlag;
                        result.Time_Interval = item1.Time_Interval;
                        PPForQAList.Add(result);
                    }
                }
                //插入QA的数据
                var QA = pPForQAInterfaceRepository.GetQADataForInterface(Product_Date, Time_Interval, Master_UID);
                foreach (var item in QA)
                {
                    var result = new PPForQAInterface();
                    result.Color = item.Color;
                    result.Create_Date = item.Create_Date;
                    result.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                    result.Input_Qty = item.Input_Qty;
                    result.MaterielType = item.MaterielType;
                    result.Modified_Date = item.Modified_Date;
                    result.NG_Qty = item.NG_Qty;
                    result.Product_Date = item.Product_Date;
                    result.QAUsedFlag = item.QAUsedFlag;
                    result.Time_Interval = item.Time_Interval;
                    PPForQAList.Add(result);
                }
                foreach (var item in PPForQAList)
                {
                    pPForQAInterfaceRepository.Add(item);
                }
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {

                return "FALSE" + e.ToString();
            }

        }
        public List<string> GetOpenProject(List<int> orgs)
        {
            return SystemProjectRepository.QueryOpenProject(orgs).ToList();

        }

        public bool KeyProcessVertify(string projectName, string Part_Types)
        {

            return productInputRepository.KeyProcessVertify(projectName, Part_Types);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string CheckMatchFlag(PPCheckDataSearch search)
        {
            string errorInfo = string.Empty;

            string nowInterval = "";
            var nowDate = new DateTime();
            //获取当前时间及时段
            var nowDateAndTime = EnumerationRepository.GetIntervalInfo(search.OP).First();
            if (nowDateAndTime != null)
            {
                nowDate = Convert.ToDateTime(nowDateAndTime.NowDate);
                nowInterval = nowDateAndTime.Time_Interval;
            }
            search.Reference_Date = nowDate;
            search.Interval_Time = nowInterval;

            //获取楼栋详细数据表中当前时段的数据列表

            var resultLocation =
 productInputLocationRepository.GetMany(
     m =>
         m.FlowChart_Master_UID == search.FlowChart_Master_UID && m.FlowChart_Version == search.FlowChart_Version
         && m.Product_Date == nowDate && m.Time_Interval == nowInterval).ToList();
            var ErrorItems1 = resultLocation.FindAll(m => m.Picking_MismatchFlag != null || m.Good_MismatchFlag != null);
            //  var hasError = result.Exists(m => m.Picking_MismatchFlag != null || m.Good_MismatchFlag != null);
            if (ErrorItems1.Count() > 0)
            {

                errorInfo = "部分领料与良品数据不匹配!    ";

                foreach (var item in ErrorItems1)
                {
                    errorInfo += item.Process + "_" + item.Color + "_" + item.Place + "           ；";
                }
                return errorInfo;
            }



            var result =
    productInputRepository.GetMany(
        m =>
            m.FlowChart_Master_UID == search.FlowChart_Master_UID && m.FlowChart_Version == search.FlowChart_Version
            && m.Product_Date == nowDate && m.Time_Interval == nowInterval).ToList();

            //检查数据Picking_MismatchFlag，Good_MismatchFlag标记

            var ErrorItems = result.FindAll(m => m.Picking_MismatchFlag != null || m.Good_MismatchFlag != null);
            //  var hasError = result.Exists(m => m.Picking_MismatchFlag != null || m.Good_MismatchFlag != null);
            if (ErrorItems.Count() > 0)
            {

                errorInfo = "部分领料与良品数据不匹配!";
                foreach (var item in ErrorItems)
                {
                    errorInfo += item.Process + "_" + item.Color + "_" + item.Place + "  ；/r/n";
                }
                return errorInfo;
            }

            //检查OQC返工和组装出料检的数量是否匹配
            var list = productInputRepository.CheckReworkAndRepairQty(search);
            if (list.Count() > 0)
            {
                CheckProductInputQty item = new CheckProductInputQty();
                var firstItemError = list.First();
                switch (firstItemError.Rework_Type)
                {
                    case StructConstants.ReworkType.Input:
                        item = list.Where(m => m.FlowChart_Detail_UID == firstItemError.Opposite_Detail_UID && m.Opposite_Detail_UID == firstItemError.FlowChart_Detail_UID
&& m.Rework_Type == StructConstants.ReworkType.Output).First();
                        errorInfo = string.Format("{0}-{1}-入：{2} 不能匹配 {3}-{4}-出：{5}",
                            firstItemError.Process, firstItemError.Color, firstItemError.Opposite_QTY, item.Process, item.Color, item.Opposite_QTY);
                        break;

                    case StructConstants.ReworkType.Output:
                        item = list.Where(m => m.FlowChart_Detail_UID == firstItemError.Opposite_Detail_UID && m.Opposite_Detail_UID == firstItemError.FlowChart_Detail_UID
&& m.Rework_Type == StructConstants.ReworkType.Input).First();
                        errorInfo = string.Format("{0}-{1}-出：{2} 不能匹配 {3}-{4}-入：{5}",
    firstItemError.Process, firstItemError.Color, firstItemError.Opposite_QTY, item.Process, item.Color, item.Opposite_QTY);
                        break;
                }

            }
            return errorInfo;
        }

        public List<ExportPPCheck_Data> ExportPPCheckData(ExportSearch search)
        {
            return productInputRepository.ExportPPCheckData(search);
        }

        public List<string> GetFunPlantForChart(string customer, string project, string productphase, string parttypes, int LanguageID)
        {
            List<string> result = new List<string>();
            var KeyProcess = LocalizedLanguageCommon.GetLocaleStringResource(LanguageID, "FlowChart.KeyProcess");
            result.Add(KeyProcess);
            //result.Add("Band_Assembly1");
            //result.Add("Assembly2_OQC");
            var temp = FlowChartDetailRepository.GetFunPlantForChart(customer, project, productphase, parttypes).ToList();
            //var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(temp);
            return result;
        }

        public List<string> QueryUserRole(string userid)
        {
            var equipment = productInputRepository.Getuserrole(userid);
            List<string> result = new List<string>();
            foreach (var item in equipment)
            {
                result.Add(item.Role_ID);
            }
            return result;
        }

        public string GetOPByFlowchartMasterUID(int masterUID)
        {
            return productInputRepository.GetOPByFlowchartMasterUID(masterUID);
        }

        public int GetPlant(string Project)
        {
            var plant = SystemProjectRepository.GetFirstOrDefault(m => m.Project_Name == Project);
            var org = systemOrgBomRepository.GetFirstOrDefault(m => m.ChildOrg_UID == plant.Organization_UID);
            return (int)org.ParentOrg_UID;
        }

        /// <summary>
        /// 导出战情日报表的楼栋详情
        /// </summary>
        /// <param name="search"></param>
        public List<ExportPPCheck_Data> ExportFloorDetialDayReport(ReportDataSearch search)
        {
            return productInputRepository.ExportFloorDetialDayReport(search);
        }
    }
}
