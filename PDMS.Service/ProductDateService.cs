using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.ViewModels;
using PDMS.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using PDMS.Common.Helpers;
using System.Transactions;
using PDMS.Common.Constants;
using System.Text;
using MoreLinq;

namespace PDMS.Service
{
    public interface IProductDataService
    {
        #region define ProductDataService Interface
        PagedListModel<ProductDataVM> QueryProductDatas(ProcessDataSearch search, Page page, List<string> currentProject);
        PagedListModel<ProductDataDTO> QueryProcessData(ProcessDataSearch search, Page page);
        PagedListModel<ProductDataVM> QueryProcessData_Input(ProcessDataSearch search, Page page, List<string> currentProject);
        PagedListModel<ProductDataDTO> QueryProcessDataForEmergency(ProcessDataSearchModel searchModel, Page page);
        ProductDataVM QueryProductDataSingle(int uuid, bool flag);
        string GetCurrentPlantName(int uid);
        System_Function_Plant QueryFuncPlantInfo(string funcPlant);
        string AddProductDatas(ProductDataList productDataList);
        PagedListModel<TimeSpanReportVM> QueryTimeSpanReport(ReportDataSearch searchModel);
        PagedListModel<ChartDailyReport> QueryTChartDailyData(ReportDataSearch searchModel);
        //2016-12-20 add by karl  时段报表查询
        PagedListModel<TimeSpanReport_2> QueryTimeSpanReport_2(ReportDataSearch searchModel);
        List<YieldVM> QueryDailyYield(ReportDataSearch searchModel);
        PagedListModel<WeekReportVM> QueryWeekReport(ReportDataSearch searchModel);
        //Rework Module--------------Sidney
        List<string> GetRepairToReworkProcessAPI(int Detail_UID, int Product_UID, string selectDate, string selectTime);

        List<int> QueryExitProductData(ProcessDataSearch search);
        ErrorInfoVM GetErrorInfo(int productUid, string ErrorType);
        string SaveInfoAndRework(ProductDataItem dto, int AccountId);
        string SaveLocationInfoAndRework(ProductDataItem dto, int AccountId);
        bool CheckHasExistProcess(int masterUID, int version);
        PagedListModel<ProductDataVM> QueryProductDataForEmergency(ProcessDataSearchModel searchModel, Page page);
        void SaveInfoEmergency(ProductDataItem dto, int AccountId);
        string FillZeroProductData(ZeroProcessDataSearch funPlantInfo);

        #endregion //define System interface
    }

    public class ProductDataService : IProductDataService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly IProductInputRepository productInputRepository;
        private readonly IProduct_Input_LocationRepository product_Input_LocationRepository;
        private readonly IProductReworkInfoRepository productReworkInfoRepository;
        private readonly IFlowChartDetailRepository flowChartDetailRepository;
        private readonly IFlowChartMasterRepository flowChartMasterRepository;
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IPPForQAInterfaceRepository pPForQAInterfaceRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly ISystemOrgBomRepository systemOrgBomRepository;
        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly IEnumerationRepository EnumerationRepository;
        private readonly IFlowChartDetailRepository flowchartRepository;
        private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;


        #endregion //Private interfaces properties

        #region Service constructor
        public ProductDataService(
            IProductInputRepository productInputRepository,
            IProduct_Input_LocationRepository product_Input_LocationRepository,
            IProductReworkInfoRepository productReworkInfoRepository,
            IFlowChartDetailRepository flowChartDetailRepository,
            IFlowChartMasterRepository flowChartMasterRepository,
            IEnumerationRepository enumerationRepository,
            IPPForQAInterfaceRepository pPForQAInterfaceRepository,
            IEnumerationRepository EnumerationRepository,
            ISystemProjectRepository systemProjectRepository,
            ISystemOrgBomRepository systemOrgBomRepository,
            ISystemOrgRepository systemOrgRepository,
            IFlowChartDetailRepository flowchartRepository,
            ISystemFunctionPlantRepository systemFunctionPlantRepository,
        IUnitOfWork unitOfWork)
        {
            this.productInputRepository = productInputRepository;
            this.product_Input_LocationRepository = product_Input_LocationRepository;
            this.productReworkInfoRepository = productReworkInfoRepository;
            this.flowChartDetailRepository = flowChartDetailRepository;
            this.flowChartMasterRepository = flowChartMasterRepository;
            this.enumerationRepository = enumerationRepository;
            this.pPForQAInterfaceRepository = pPForQAInterfaceRepository;
            this.EnumerationRepository = EnumerationRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.systemOrgBomRepository = systemOrgBomRepository;
            this.systemOrgRepository = systemOrgRepository;
            this.flowchartRepository = flowchartRepository;
            this.systemFunctionPlantRepository = systemFunctionPlantRepository;
            this.unitOfWork = unitOfWork;
        }
        #endregion //Service constructor



        //Product_Input生产数据新增初始化Method，编辑数据的初始化加载Method在QueryProductDatas方法
        public PagedListModel<ProductDataVM> QueryProcessData_Input(ProcessDataSearch search, Page page, List<string> currentProject)
        {
            var totalCount = 0;
            var ProductDatas = productInputRepository.QueryProcessData_Input(search, currentProject, page, out totalCount);

            //获取返工站点信息
            var reworkList = ProductDatas.Where(m => m.Rework_Flag == StructConstants.ReworkFlag.Rework).ToList();
            foreach (var item in reworkList)
            {
                var strIdList = item.RelatedRepairUID.Split(',').ToList();
                //将list<string>转换为list<int>
                var intIdList = strIdList.Select<string, int>(x => Convert.ToInt32(x));
                var detailList = flowChartDetailRepository.GetMany(m => intIdList.Contains(m.FlowChart_Detail_UID)).ToList();
                List<NewInfo_ReworkList> newInfoList = new List<NewInfo_ReworkList>();
                foreach (var detailItem in detailList)
                {
                    NewInfo_ReworkList newInfoItem = new NewInfo_ReworkList();
                    newInfoItem.RepairDetailUID = detailItem.FlowChart_Detail_UID;
                    newInfoItem.RepairProcess = detailItem.Process;
                    newInfoItem.RepairPlace = detailItem.Place;
                    newInfoItem.Color = detailItem.Color;

                    newInfoList.Add(newInfoItem);
                }
                item.NewInfo_ReworkList = newInfoList;
            }

            //获取修复站点信息
            var repairList = ProductDatas.Where(m => m.Rework_Flag == StructConstants.ReworkFlag.Repair).ToList();
            if (repairList.Count() > 0)
            {
                var flMasterUID = repairList.First().FlowChart_Master_UID;
                var flVersion = repairList.First().FlowChart_Version;

                var rList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == flMasterUID && m.FlowChart_Version == flVersion
                && m.Rework_Flag == StructConstants.ReworkFlag.Rework).ToList();

                foreach (var item in repairList)
                {
                    //查找对应的rework
                    var rework_List = rList.Where(m => m.RelatedRepairUID.Contains(item.FlowChart_Detail_UID.ToString())).ToList();
                    List<NewInfo_RepairList> newInfoRepairList = new List<NewInfo_RepairList>();
                    foreach (var reworkItem in rework_List)
                    {
                        NewInfo_RepairList newInfoItem = new NewInfo_RepairList();
                        newInfoItem.ReworkDetailUID = reworkItem.FlowChart_Detail_UID;
                        newInfoItem.ReworkProcess = reworkItem.Process;
                        newInfoItem.ReworkPlace = reworkItem.Place;
                        newInfoItem.Color = reworkItem.Color;
                        newInfoRepairList.Add(newInfoItem);
                    }
                    item.NewInfo_RepairList = newInfoRepairList;
                }
            }


            return new PagedListModel<ProductDataVM>(totalCount, ProductDatas);
        }


        /// <summary>
        /// Product_Input生产数据编辑初始化Method
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<ProductDataVM> QueryProductDatas(ProcessDataSearch searchModel, Page page, List<string> currentProject)
        {
            var totalCount = 0;
            var ProductDatas = productInputRepository.QueryProductDatas(searchModel, page, out totalCount);

            if (ProductDatas.Count() == 0)
            {
                return new PagedListModel<ProductDataVM>(0, new List<ProductDataVM>());
            }

            var flMasterUID = ProductDatas.First().FlowChart_Master_UID;
            var flVersion = ProductDatas.First().FlowChart_Version;
            var prodDate = ProductDatas.First().Product_Date;


            var flList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == flMasterUID && m.FlowChart_Version == flVersion).ToList();

            //获取Rework，Repair的信息
            var reworkRepairUIDList = flList.Where(m => m.Rework_Flag == StructConstants.ReworkFlag.Rework || m.Rework_Flag == StructConstants.ReworkFlag.Repair).Select(m => m.FlowChart_Detail_UID).ToList();
            var reworkRepairList = productReworkInfoRepository.GetMany(m => reworkRepairUIDList.Contains(m.FlowChart_Detail_UID) && m.Product_Date == prodDate && m.Time_Interval == searchModel.Time).ToList();

            //获取功能厂信息
            var funlist = systemFunctionPlantRepository.GetAll().ToList();

            IList<ProductDataVM> ProductDatasDTO = new List<ProductDataVM>();

            foreach (var item in ProductDatas)
            {
                var pd = new ProductDataVM();
                pd.Location_Flag = item.Location_Flag;
                pd.Is_Comfirm = item.Is_Comfirm;
                pd.Product_Date = item.Product_Date;
                pd.Time_Interval = item.Time_Interval;
                pd.Customer = item.Customer;
                pd.Project = item.Project;
                pd.Part_Types = item.Part_Types;
                pd.FunPlant = item.FunPlant;
                pd.FunPlant_Manager = item.FunPlant_Manager;
                pd.Product_Phase = item.Product_Phase;
                pd.Process_Seq = item.Process_Seq;
                pd.Place = item.Place;
                pd.Process = item.Process;
                pd.FlowChart_Master_UID = item.FlowChart_Master_UID;
                pd.FlowChart_Version = item.FlowChart_Version;
                pd.Color = item.Color;
                pd.Prouct_Plan = item.Prouct_Plan;
                pd.Product_Stage = item.Product_Stage;
                pd.Target_Yield = item.Target_Yield;
                pd.Good_QTY = item.Good_QTY;
                pd.Picking_QTY = item.Picking_QTY;
                pd.WH_Picking_QTY = item.WH_Picking_QTY;
                pd.NG_QTY = item.NG_QTY;
                pd.WH_QTY = item.WH_QTY;
                pd.WIP_QTY = item.WIP_QTY;
                pd.NullWip_QTY = item.NullWip_QTY;
                pd.Adjust_QTY = item.Adjust_QTY;
                pd.Creator_UID = item.Creator_UID;
                pd.Create_Date = item.Create_Date;
                pd.Material_No = item.Material_No;
                pd.Modified_UID = item.Modified_UID;
                pd.Modified_Date = item.Modified_Date;
                pd.Product_UID = item.Product_UID;
                pd.Good_MismatchFlag = item.Good_MismatchFlag;
                pd.Normal_Good_QTY = item.Normal_Good_QTY;
                pd.Normal_NG_QTY = item.Normal_NG_QTY;
                pd.Abnormal_Good_QTY = item.Abnormal_Good_QTY;
                pd.Abnormal_NG_QTY = item.Abnormal_NG_QTY;
                pd.Unacommpolished_Reason = item.Unacommpolished_Reason;
                if (!string.IsNullOrWhiteSpace(item.Good_MismatchFlag))
                {
                    //pd.Good_Contact = productInputRepository.QueryFuncPlantInfo(item.Good_MismatchFlag).FunPlant_Contact;
                    var funName = funlist.Where(m => m.FunPlant == item.Good_MismatchFlag).Select(m => m.FunPlant_Contact).First();
                    pd.Good_Contact = funName;
                }
                pd.Picking_MismatchFlag = item.Picking_MismatchFlag;
                if (!string.IsNullOrWhiteSpace(item.Picking_MismatchFlag))
                {
                    //pd.Picking_Contact = productInputRepository.QueryFuncPlantInfo(item.Picking_MismatchFlag).FunPlant_Contact;
                    var funName = funlist.Where(m => m.FunPlant == item.Picking_MismatchFlag).Select(m => m.FunPlant_Contact).First();
                    pd.Picking_Contact = funName;
                }
                pd.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                #region Add Rework info------------------------------
                var item1 = item;
                //从FlowChartDetail获取返工类型
                var detailinfo =
                    flList.Where(m => m.FlowChart_Detail_UID == item1.FlowChart_Detail_UID)
                        .FirstOrDefault();
                var reworkFlag = "";
                if (detailinfo != null)
                {
                    reworkFlag = detailinfo.Rework_Flag;
                    pd.FlowChart_Detail_UID = detailinfo.FlowChart_Detail_UID;
                }
                pd.Rework_Flag = reworkFlag;
                //从ReworkInfo中获取返工数 
                if (reworkFlag == StructConstants.ReworkFlag.Rework)
                {

                    var strIdList = detailinfo.RelatedRepairUID.Split(',').ToList();

                    //将list<string>转换为list<int>
                    var intIdList = strIdList.Select<string, int>(x => Convert.ToInt32(x));
                    var detailList = flList.Where(m => intIdList.Contains(m.FlowChart_Detail_UID)).ToList();
                    List<NewInfo_ReworkList> newInfoList = new List<NewInfo_ReworkList>();
                    List<Product_Rework_Info> priList = null;
                    foreach (var detailItem in detailList)
                    {
                        NewInfo_ReworkList newInfoItem = new NewInfo_ReworkList();
                        newInfoItem.RepairDetailUID = detailItem.FlowChart_Detail_UID;
                        newInfoItem.RepairProcess = detailItem.Process;
                        newInfoItem.RepairPlace = detailItem.Place;
                        newInfoItem.Color = detailItem.Color;

                        //获取每个对应的楼栋的返工信息
                        List<ProductReworkInfoVM> vmList = new List<ProductReworkInfoVM>();
                        var infoList = reworkRepairList.Where(m => m.FlowChart_Detail_UID == item.FlowChart_Detail_UID
                        && m.Product_Date == item.Product_Date && m.Time_Interval == searchModel.Time).ToList();
                        vmList = AutoMapper.Mapper.Map<List<ProductReworkInfoVM>>(infoList);
                        //添加到dto对象
                        newInfoItem.ProductReworkInfoVM = vmList;
                        //添加到list
                        newInfoList.Add(newInfoItem);
                        if (priList == null)
                        {
                            priList = infoList;
                        }
                    }
                    pd.NewInfo_ReworkList = newInfoList;
                    //获取返工数量总和
                    var inputSum = priList.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                    var outputSum = priList.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);
                    pd.SumInputOutput_ByRework = string.Format("入:{0} 出:{1}", inputSum, outputSum);
                    //获取返工数据是否匹配，用于界面是否标红
                    pd.IsRedDisplay = priList.Exists(m => m.Is_Match == false);
                }


                if (reworkFlag == StructConstants.ReworkFlag.Repair)
                {
                    var rList = flList.Where(m => m.Rework_Flag == StructConstants.ReworkFlag.Rework).ToList();

                    //查找对应的rework
                    var rework_List = rList.Where(m => m.RelatedRepairUID.Contains(item.FlowChart_Detail_UID.ToString())).ToList();
                    List<NewInfo_RepairList> newInfoRepairList = new List<NewInfo_RepairList>();
                    List<Product_Rework_Info> priList = null;
                    foreach (var reworkItem in rework_List)
                    {
                        NewInfo_RepairList newInfoItem = new NewInfo_RepairList();
                        newInfoItem.ReworkDetailUID = reworkItem.FlowChart_Detail_UID;
                        newInfoItem.ReworkProcess = reworkItem.Process;
                        newInfoItem.ReworkPlace = reworkItem.Place;
                        newInfoItem.Color = reworkItem.Color;
                        newInfoRepairList.Add(newInfoItem);

                        //获取每个对应的楼栋的返工信息
                        List<ProductReworkInfoVM> vmList = new List<ProductReworkInfoVM>();
                        var infoList = reworkRepairList.Where(m => m.FlowChart_Detail_UID == item.FlowChart_Detail_UID
                        && m.Product_Date == item.Product_Date && m.Time_Interval == searchModel.Time).ToList();
                        vmList = AutoMapper.Mapper.Map<List<ProductReworkInfoVM>>(infoList);
                        //添加到dto对象
                        newInfoItem.ProductRepairInfoVM = vmList;
                        if (priList == null)
                        {
                            priList = infoList;
                        }
                    }
                    pd.NewInfo_RepairList = newInfoRepairList;
                    //获取返工数量总和
                    try
                    {
                        var inputSum = priList.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                        var outputSum = priList.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);
                        pd.SumInputOutput_ByRepair = string.Format("入:{0} 出:{1}", inputSum, outputSum);
                        //获取返工数据是否匹配，用于界面是否标红
                        pd.IsRedDisplay = priList.Exists(m => m.Is_Match == false);
                    }
                    catch
                    {

                    }
                }


                #endregion

                //如果该数据来源为Product_Input 并且该数据对应的制程为分楼栋的，排除该数据
                if (!item.Location_Flag && detailinfo.Location_Flag)
                    continue;
                ProductDatasDTO.Add(pd);
            }
            //加上当前账户所在的Project
            if (ProductDatasDTO.Any())
            {
                var result = new List<ProductDataVM>();
                foreach (var item in currentProject)
                {
                    var resultItem = new List<ProductDataVM>();
                    resultItem = ProductDatasDTO.Where(m => m.Project == item).ToList();
                    result.AddRange(resultItem);
                }
                ProductDatasDTO = result;
            }
            return new PagedListModel<ProductDataVM>(ProductDatasDTO.Count, ProductDatasDTO);
        }



        public PagedListModel<ProductDataVM> QueryProductDataForEmergency(ProcessDataSearchModel searchModel, Page page)
        {
            var totalCount = 0;
            var ProductDatas = productInputRepository.QueryProductDataForEmergency(searchModel, page, out totalCount);


            IList<ProductDataVM> ProductDatasDTO = new List<ProductDataVM>();

            foreach (var item in ProductDatas)
            {
                var pd = new ProductDataVM();
                pd.Is_Comfirm = item.Is_Comfirm;
                pd.Product_Date = item.Product_Date;
                pd.Time_Interval = item.Time_Interval;
                pd.Customer = item.Customer;
                pd.Project = item.Project;
                pd.Part_Types = item.Part_Types;
                pd.FunPlant = item.FunPlant;
                pd.FunPlant_Manager = item.FunPlant_Manager;
                pd.Product_Phase = item.Product_Phase;
                pd.Process_Seq = item.Process_Seq;
                pd.Place = item.Place;
                pd.Process = item.Process;
                pd.FlowChart_Master_UID = item.FlowChart_Master_UID;
                pd.FlowChart_Version = item.FlowChart_Version;
                pd.Color = item.Color;
                pd.Prouct_Plan = item.Prouct_Plan;
                pd.Product_Stage = item.Product_Stage;
                pd.Target_Yield = item.Target_Yield;
                pd.Good_QTY = item.Good_QTY;
                pd.Picking_QTY = item.Picking_QTY;
                pd.WH_Picking_QTY = item.WH_Picking_QTY;
                pd.NG_QTY = item.NG_QTY;
                pd.WH_QTY = item.WH_QTY;
                pd.WIP_QTY = item.WIP_QTY;
                pd.Adjust_QTY = item.Adjust_QTY;
                pd.Creator_UID = item.Creator_UID;
                pd.Create_Date = item.Create_Date;
                pd.Material_No = item.Material_No;
                pd.Modified_UID = item.Modified_UID;
                pd.Modified_Date = item.Modified_Date;
                pd.Product_UID = item.Product_UID;
                pd.Good_MismatchFlag = item.Good_MismatchFlag;
                pd.Normal_Good_QTY = item.Normal_Good_QTY;
                pd.Normal_NG_QTY = item.Normal_NG_QTY;
                pd.Abnormal_Good_QTY = item.Abnormal_Good_QTY;
                pd.Abnormal_NG_QTY = item.Abnormal_NG_QTY;
                if (!string.IsNullOrWhiteSpace(item.Good_MismatchFlag))
                {
                    pd.Good_Contact = productInputRepository.QueryFuncPlantInfo(item.Good_MismatchFlag).FunPlant_Contact;
                }
                pd.Picking_MismatchFlag = item.Picking_MismatchFlag;
                if (!string.IsNullOrWhiteSpace(item.Picking_MismatchFlag))
                {
                    pd.Picking_Contact = productInputRepository.QueryFuncPlantInfo(item.Picking_MismatchFlag).FunPlant_Contact;
                }
                pd.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                #region Add Rework info------------------------------Sidney 2016-04-18
                var item1 = item;
                //从FlowChartDetail获取返工类型
                var detailinfo =
                    flowChartDetailRepository.GetMany(m => m.FlowChart_Detail_UID == item1.FlowChart_Detail_UID)
                        .FirstOrDefault();
                var reworkFlag = "";
                if (detailinfo != null)
                {
                    reworkFlag = detailinfo.Rework_Flag;
                    pd.FlowChart_Detail_UID = detailinfo.FlowChart_Detail_UID;
                }
                pd.Rework_Flag = reworkFlag;
                //从ReworkInfo中获取返工数
                if (reworkFlag == "Rework" || reworkFlag == "Repair")
                {
                    var reworkList = productInputRepository.getReworkList(item.FlowChart_Detail_UID, item.Product_Date, item.Time_Interval);
                    if (reworkList.Count() > 0)
                    {
                        pd.ReworkInfoList = AutoMapper.Mapper.Map<List<ProductReworkInfoVM>>(reworkList);
                        var inputNum = pd.ReworkInfoList.Where(m => m.Rework_Type == "Input").Select(m => m.Opposite_QTY).Sum();
                        var outputNum = pd.ReworkInfoList.Where(m => m.Rework_Type == "Output").Select(m => m.Opposite_QTY).Sum();
                        //pd.SumInputOutput = string.Format("入:{0} 出:{1}", inputNum, outputNum);
                    }
                    if (reworkList.Exists(m => !m.Is_Match))
                    {
                        pd.Is_Match = false;
                    }

                    switch (reworkFlag)
                    {
                        case "Rework":
                            //var reworkList = item.Product_Rework_Info.ToList();

                            break;
                        case "Repair":
                            break;
                    }



                }

                else
                {
                    //pd.Rework_QTY = 0;
                }


                #endregion
                ProductDatasDTO.Add(pd);
            }
            //加上当前账户所在的Project

            return new PagedListModel<ProductDataVM>(totalCount, ProductDatasDTO);
        }


        public string AddProductDatas(ProductDataList productDataList)
        {

            using (TransactionScope scope = new TransactionScope())
            {
                //只插入Product_Input_Location和Product_Rework_Info表不插入Product_Input
                bool onlyInsertReworkTable = false;
                List<string> insertSqlList = new List<string>();
                string nowInterval = "";
                var nowDate = new DateTime();
                //获取当前时间及时段
                //先获取当前OP

                var currentProject = productDataList.ProductLists[0].Project;
                var customer = productDataList.ProductLists[0].Customer;
                var currentOP = systemProjectRepository.GetSelctOP(customer, currentProject);

                var nowDateAndTime = enumerationRepository.GetIntervalInfo(currentOP).FirstOrDefault();
                if (nowDateAndTime != null)
                {
                    nowDate = Convert.ToDateTime(nowDateAndTime.NowDate);
                    nowInterval = nowDateAndTime.Time_Interval;
                }


                List<ProductDataItem> pDataList = productDataList.ProductLists;
                Product_Input search = new Product_Input();

                //获取对应的制程FlowChart_Detail_UID
                var mUid = pDataList.First().FlowChart_Master_UID;
                var ver = pDataList.First().FlowChart_Version;
                string Time_Interval = pDataList.First().Time_Interval;

                //add by karl 2017-03-03 判定若为成都厂的专案则不卡不可用wip大于wip总数---start
                var flowchartMst = flowChartMasterRepository.GetFirstOrDefault(m => m.FlowChart_Master_UID == mUid);
                var project = systemProjectRepository.GetFirstOrDefault(m => m.Project_UID == flowchartMst.Project_UID);
                var orgbom = systemOrgBomRepository.GetFirstOrDefault(m => m.ChildOrg_UID == project.Organization_UID);
                var org = systemOrgRepository.GetFirstOrDefault(m => m.Organization_UID == orgbom.ParentOrg_UID);
                //add by karl 2017-03-03 判定若为成都厂的专案则不卡不可用wip大于wip总数---end

                //根据uid和版本获取当前的FlowchartDetailList
                var currentFlDetailList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == mUid && m.FlowChart_Version == ver).ToList();
                //var repairOper = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == mUid
                //&& m.FlowChart_Version == ver && m.Rework_Flag == "Repair").ToList();
                var repairOper = currentFlDetailList.Where(m => m.Rework_Flag == "Repair").ToList();
                Product_Rework_Info updateReworkInfo = null;

                var flowchartDetailUIDList = pDataList.Select(m => m.FlowChart_Detail_UID).ToList();
                var flowchartDetailList = flowChartDetailRepository.GetMany(m => flowchartDetailUIDList.Contains(m.FlowChart_Detail_UID)).ToList();

                //判断PP是否已经检核，检核后不能再提交数据。
                string ErrorInfo = "";
                bool islable= productInputRepository.IsChecked(mUid, ver, pDataList.First().Product_Date.ToString("yyyy-MM-dd"),  Time_Interval);
                if(islable)
                {
                    ErrorInfo = "PP已经保存数据，不能提交数据";
                    return ErrorInfo;
                }
              
                try
                {
                    foreach (ProductDataItem pData in pDataList)
                    {
                        onlyInsertReworkTable = false;
                        //if (pData.Is_Comfirm)
                        //{
                        //    throw new Exception("PP已经保存数据，不能修改，特此记录-----ProductDataService-----404行，数据时段：" + pData.Time_Interval + ",服务器时段：" + nowInterval);
                        //}


                        search = AutoMapper.Mapper.Map<Product_Input>(pData);
                        //插入之前先检查是否已经存在数据
                        var hasExistItem = productInputRepository.GetMany(m => m.FlowChart_Master_UID == search.FlowChart_Master_UID && m.FlowChart_Version == search.FlowChart_Version
                        && m.Product_Date == search.Product_Date && m.Time_Interval == search.Time_Interval && m.FlowChart_Detail_UID == search.FlowChart_Detail_UID).FirstOrDefault();
                        if (hasExistItem != null)
                        {
                            ErrorInfo = string.Format("{0}-{1}-{2} 已经存在相同的数据了，不能重复提交", search.Process, search.Color, search.Place);
                            break;
                        }

                        Product_Input newItem = new Product_Input();
                        //以下四列赋值，在ProductInfo不插入数据的时候赋值给Rework表
                        newItem.FlowChart_Detail_UID = pData.FlowChart_Detail_UID;
                        newItem.Product_Date = pData.Product_Date;
                        newItem.Modified_UID = pData.Modified_UID;
                        newItem.Modified_Date = pData.Modified_Date;

                        //判断是否为多楼栋数据
                        if (pData.Location_Flag)
                        {
                            //多楼栋将数据插入详细表
                            Product_Input_Location item = AutoMapper.Mapper.Map<Product_Input_Location>(pData);
                            //计算当前要插入的WIP_QTY 获取各楼栋分别的WIP
                            var addQTYLocation = item.Picking_QTY + item.WH_Picking_QTY - item.Abnormal_Good_QTY - item.Normal_Good_QTY
                                - item.WH_QTY - item.Abnormal_NG_QTY - item.Normal_NG_QTY - item.Adjust_QTY;
                            var detailLocation = currentFlDetailList.Where(m => m.FlowChart_Detail_UID == pData.FlowChart_Detail_UID).First();  //flowChartDetailRepository.GetById(pData.FlowChart_Detail_UID);
                            var nowQtyLocation = addQTYLocation + detailLocation.WIP_QTY + detailLocation.NullWip - item.NullWip_QTY.Value;  //加上不可用wip变化数

                            //更新Flowchart Detail WIP
                            detailLocation.WIP_QTY = nowQtyLocation;
                            detailLocation.NullWip = pData.NullWip_QTY;

                            if (nowQtyLocation < 0 && org.Organization_Name == "WUXI_M" && (detailLocation.Rework_Flag != "Rework" && detailLocation.Rework_Flag != "Repair"))
                                return pData.Process + "_" + pData.Color + "_" + pData.Place + "　　WIP总数不能为负,当前WIP为：(" + nowQtyLocation + ") 请核实数据或者找总战情修改WIP，谢谢";

                            item.Unacommpolished_Reason = pData.Unacommpolished_Reason;

                            //newItem.FunPlant = flowchartDetailList.Where(m => m.FlowChart_Detail_UID == pData.FlowChart_Detail_UID).Select(m => m.System_Function_Plant.FunPlant).First();
                            item.FunPlant = detailLocation.System_Function_Plant.FunPlant;
                            item.IsLast = true;
                            item.Good_QTY = pData.Normal_Good_QTY + pData.Abnormal_Good_QTY;
                            item.NG_QTY = pData.Normal_NG_QTY + pData.Abnormal_NG_QTY;
                            item.Modified_Date = pData.Create_Date;
                            //计算新增的Wip_Qty,不包含Rework和Repair，这里需要插入返工数据
                            item.WIP_QTY = nowQtyLocation;
                            newItem.WIP_QTY = nowQtyLocation;
                            item.NullWip_QTY = pData.NullWip_QTY;
                            item.Is_Comfirm = false;
                            //修改Product_Input_Location表的Wip
                            string resultStr = AddWipQtyByReowrkOrRepair(pData, item);
                            if (!string.IsNullOrWhiteSpace(resultStr))
                            {
                                return resultStr;
                            }
                            product_Input_LocationRepository.Add(item);


                            //else
                            //{
                            //    onlyInsertReworkTable = true;
                            //    //只插入Rework表不插入Product_Input表

                            //}
                        }
                        else   //按原来的逻辑继续走下去
                        {
                            #region 插入Product_Input表数据准备
                            newItem = AutoMapper.Mapper.Map<Product_Input>(pData);
                            //计算当前要插入的WIP_QTY
                            var addQTY = newItem.Picking_QTY + newItem.WH_Picking_QTY - newItem.Abnormal_Good_QTY - newItem.Normal_Good_QTY
                                - newItem.WH_QTY - newItem.Abnormal_NG_QTY - newItem.Normal_NG_QTY - newItem.Adjust_QTY;
                            var detail = currentFlDetailList.Where(m => m.FlowChart_Detail_UID == pData.FlowChart_Detail_UID).First();  //flowChartDetailRepository.GetById(pData.FlowChart_Detail_UID);
                            var nowQty = addQTY + detail.WIP_QTY + detail.NullWip - newItem.NullWip_QTY;  // 待确认
                            //准备Product_Input的数据
                            //newItem.FunPlant = flowchartDetailList.Where(m => m.FlowChart_Detail_UID == pData.FlowChart_Detail_UID).Select(m => m.System_Function_Plant.FunPlant).First();
                            newItem.FunPlant = detail.System_Function_Plant.FunPlant;
                            newItem.IsLast = true;
                            newItem.Good_QTY = pData.Normal_Good_QTY + pData.Abnormal_Good_QTY;
                            newItem.NG_QTY = pData.Normal_NG_QTY + pData.Abnormal_NG_QTY;
                            newItem.Modified_Date = pData.Create_Date;
                            //计算新增的Wip_Qty,不包含Rework和Repair，在下面还有计算
                            newItem.WIP_QTY = nowQty;
                            newItem.NullWip_QTY = pData.NullWip_QTY;
                            newItem.Is_Comfirm = false;
                            //不是返工返修站点才需要这样

                            if (nowQty < 0 && org.Organization_Name == "WUXI_M" && (detail.Rework_Flag != "Rework" && detail.Rework_Flag != "Repair"))
                                return pData.Process + "_" + pData.Color + "_" + pData.Place + "　　WIP总数不能为负,当前WIP为：(" + nowQty + ") 请核实数据或者找总战情修改WIP，谢谢";
                            //if (nowQty < pData.NullWip_QTY && org.Organization_Name == "WUXI_M")
                            //    ErrorInfo += "不可用wip不可以大于wip总数(" + nowQty + ")";
                            //var result=productInputRepository.InsertOrUpdateProductAndWIP(item);
                            //if (result != "SUCCESS") return result;
                            #endregion
                            #region Insert ReworkInfo
                            //ErrorInfo = AddReworkAndRepairInfo(pData, repairOper, newItem, nowDate, Time_Interval, updateReworkInfo);
                            //更新Detail表的Wip
                            detail.WIP_QTY = newItem.WIP_QTY.Value;
                            detail.NullWip = newItem.NullWip_QTY.Value;
                            newItem.Unacommpolished_Reason = pData.Unacommpolished_Reason;
                            productInputRepository.Add(newItem);
                            //flowChartDetailRepository.Update(detail);
                            #endregion
                        }

                        //插入返工数据

                        ErrorInfo += AddReworkAndRepairInfo(pData, repairOper, newItem, nowDate, Time_Interval, updateReworkInfo, onlyInsertReworkTable, insertSqlList);
                        if (!string.IsNullOrEmpty(ErrorInfo))
                        {
                            break;
                        }
                        ////如果分楼栋并且楼栋信息没有完全录完
                        //if (!onlyInsertReworkTable)
                        //{
                        //    productInputRepository.Add(newItem);
                        //}
                    }
                    if (string.IsNullOrEmpty(ErrorInfo))
                    {
                        unitOfWork.Commit();
                        productInputRepository.AddReworkAndRepairInfo(insertSqlList);
                        ModifyReworkAndRepairIsMatch(pDataList, nowDate, Time_Interval);

                        //先将product_Input数据全部插入到数据库，然后再判断是否需要插入到Product_Input
                        //先获取此次插入到Loacation数据表中所有中不同制程和颜色的数据

                        var ProductInputLocationList = pDataList.Where(A => A.Location_Flag == true).DistinctBy(A => new { Process = A.Process, Color = A.Color });

                        //遍历每个插入的数据，查看是否已经全部录入完毕，录完了后，将合并后的数据插入到product_input
                        foreach (var item in ProductInputLocationList)
                        {
                            //数据插入完后检查是否为已经插入完整
                            //1先获取该制程有多少楼栋
                            int PlaceCount = getFlowchartBuildingCount(item.FlowChart_Master_UID, item.FlowChart_Version, item.Process_Seq, item.Color);

                            //2 获取详细表i该时段有多少数据
                            var BuildingList = getDetailBuildingList(item.FlowChart_Master_UID, item.FlowChart_Version, item.Process_Seq, item.Color, item.Product_Date, item.Time_Interval);
                            int DetailCount = BuildingList.Count();

                            if (PlaceCount == DetailCount)

                            //获取当前提交的数据有几个数据
                            // 如果楼栋数据表中的个数和Flowchart中楼栋数相同，将数据合并并写入到Product_Input
                            {
                                //合并各项数据
                                int SumGood_QTY = 0; int SumNG_QTY = 0; int? SumWIP = 0; int SumPicking_QTY = 0;
                                int SumWH_Picking_QTY = 0; int SumWH_QTY = 0; int SumAdjust_QTY = 0;
                                int SumAbnormal_Good_QTY = 0; int SumAbnormal_NG_QTY = 0;
                                int SumNormal_Good_QTY = 0;
                                int SumNormal_NG_QTY = 0;
                                int SumPlan = 0;
                                int? SumNullWIP = 0;
                                string uReason = string.Empty;
                                //获取该制程在楼栋详细表中的数据列表
                                foreach (var Building in BuildingList)
                                {
                                    SumGood_QTY += Building.Good_QTY;
                                    SumNG_QTY += Building.NG_QTY;
                                    SumWIP += Building.WIP_QTY;
                                    SumPicking_QTY += Building.Picking_QTY;
                                    SumWH_Picking_QTY += Building.WH_Picking_QTY;
                                    SumWH_QTY += Building.WH_QTY;
                                    SumAdjust_QTY += Building.Adjust_QTY;
                                    SumAbnormal_Good_QTY += Building.Abnormal_Good_QTY;
                                    SumAbnormal_NG_QTY += Building.Abnormal_NG_QTY;
                                    SumNormal_Good_QTY += Building.Normal_Good_QTY;
                                    SumNormal_NG_QTY += Building.Normal_NG_QTY;
                                    SumPlan += Building.Prouct_Plan;
                                    SumNullWIP += Building.NullWip_QTY;
                                    if (!string.IsNullOrWhiteSpace(Building.Unacommpolished_Reason))
                                    {
                                        if (!string.IsNullOrWhiteSpace(uReason))
                                            uReason = uReason + "_" + Building.Unacommpolished_Reason;
                                        else
                                            uReason = Building.Unacommpolished_Reason;

                                    }
                                }

                                //组合Product_input数据
                                var newItem = new Product_Input();
                                newItem.Abnormal_Good_QTY = SumAbnormal_Good_QTY;
                                newItem.Abnormal_NG_QTY = SumAbnormal_NG_QTY;
                                newItem.Adjust_QTY = SumAdjust_QTY;
                                newItem.Color = item.Color;
                                newItem.Customer = item.Customer;
                                newItem.DRI = item.DRI;
                                newItem.FlowChart_Detail_UID = getFlowchartMaxUID(item.FlowChart_Master_UID, item.FlowChart_Version, item.Process_Seq, item.Color);
                                newItem.FlowChart_Master_UID = item.FlowChart_Master_UID;
                                newItem.FlowChart_Version = item.FlowChart_Version;
                                newItem.FunPlant = item.FunPlant;
                                newItem.FunPlant_Manager = item.FunPlant_Manager;
                                newItem.Good_QTY = SumGood_QTY;
                                newItem.Is_Comfirm = false;
                                newItem.Modified_Date = item.Modified_Date;
                                newItem.Modified_UID = item.Modified_UID;
                                newItem.NG_QTY = SumNG_QTY;
                                newItem.Normal_Good_QTY = SumNormal_Good_QTY;
                                newItem.Normal_NG_QTY = SumNormal_NG_QTY;
                                newItem.Part_Types = item.Part_Types;
                                newItem.Picking_QTY = SumPicking_QTY;
                                newItem.Place = "";
                                newItem.Process = item.Process;
                                newItem.Process_Seq = item.Process_Seq;
                                newItem.Product_Date = item.Product_Date;
                                newItem.Product_Phase = item.Product_Phase;
                                newItem.Product_Stage = item.Product_Stage;
                                newItem.Project = item.Project;
                                newItem.Prouct_Plan = SumPlan;
                                newItem.Target_Yield = item.Target_Yield;
                                newItem.Time_Interval = item.Time_Interval;
                                newItem.WH_Picking_QTY = SumWH_Picking_QTY;
                                newItem.WH_QTY = SumWH_QTY;
                                newItem.WIP_QTY = SumWIP;
                                newItem.Create_Date = item.Create_Date;
                                newItem.Creator_UID = item.Creator_UID;
                                newItem.NullWip_QTY = SumNullWIP;
                                //分楼栋的 合并和用哪个？？？？？？？？？？？？？？？？？？
                                newItem.Unacommpolished_Reason = uReason;
                                productInputRepository.Add(newItem);
                            }
                        }
                        unitOfWork.Commit();
                        productInputRepository.ExecAlterSp(search);
                    }
                    scope.Complete();
                }
              
                catch (Exception e)
                {
                    Logger logger = new Logger("生产数据维护");
                    string error = string.Format("生产数据维护，用户:{0},Process_Seq:{1}", pDataList.First().Modified_UID, pDataList.First().Process_Seq);
                    logger.Error(error + e.ToString(), e);
                    ErrorInfo += "False" + e.ToString();
                }

                return ErrorInfo;
            }
        }

        private string AddReworkAndRepairInfo(ProductDataItem pData, List<FlowChart_Detail> repairOper, Product_Input newItem, DateTime nowDate, string Time_Interval, Product_Rework_Info updateReworkInfo, bool onlyInsertReworkTable, List<string> insertSqlList)
        {
            StringBuilder sb = new StringBuilder();

            string errorInfo = string.Empty;
            #region Rework
            if (pData.IsRepair == "Rework")
            {
                //获取ReworkInfo表相匹配的列表
                //var repairMatchList = productReworkInfoRepository.GetMany(m => repairFlUIDList.Contains(m.FlowChart_Detail_UID) && m.Opposite_Detail_UID == pData.FlowChart_Detail_UID
                //&& m.Product_Date == nowDate && m.Time_Interval == Time_Interval && m.Rework_Flag == StructConstants.ReworkFlag.Repair).ToList();
                foreach (var item in pData.ProductReworkInfoVM)
                {
                    var sql = @"INSERT INTO dbo.Product_Rework_Info
                                ( 
                                    FlowChart_Detail_UID ,
                                    Opposite_Detail_UID ,
                                    Opposite_QTY ,
                                    Product_Date ,
                                    Time_Interval ,
                                    Is_Match ,
                                    Rework_Type ,
                                    Rework_Flag ,
                                    Modified_UID ,
                                    Modified_Date
                                )
                        VALUES  ( 
                                    {0} , -- FlowChart_Detail_UID - int
                                    {1} , -- Opposite_Detail_UID - int
                                    {2} , -- Opposite_QTY - int
                                    '{3}' , -- Product_Date - date
                                    N'{4}' , -- Time_Interval - nvarchar(20)
                                    {5} , -- Is_Match - bit
                                    N'{6}' , -- Rework_Type - nvarchar(20)
                                    N'{7}' , -- Rework_Flag - nvarchar(20)
                                    {8} , -- Modified_UID - int
                                    '{9}'  -- Modified_Date - datetime
                                );";
                    sql = string.Format(sql,
                        item.FlowChart_Detail_UID,
                        item.Opposite_Detail_UID,
                        item.Opposite_QTY,
                        newItem.Product_Date,
                        Time_Interval,
                        1,
                        item.Rework_Type,
                        StructConstants.ReworkFlag.Rework,
                        newItem.Modified_UID,
                        newItem.Modified_Date);
                    insertSqlList.Add(sql);
                    //计算WIP
                    if (item.Rework_Type == "Input")
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY + item.Opposite_QTY;

                    }
                    else
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY - item.Opposite_QTY;
                    }
                    if (newItem.WIP_QTY < 0)
                        return pData.Process + "_" + pData.Color + "_" + pData.Place + "　　WIP总数不能为负,当前WIP为：(" + newItem.WIP_QTY + ") 请核实数据或者找总战情修改WIP，谢谢";

                    if (newItem.WIP_QTY != null)
                        flowchartRepository.GetById(item.FlowChart_Detail_UID).WIP_QTY = int.Parse(newItem.WIP_QTY.ToString());
                }
            }
            #endregion

            #region Repair
            if (pData.IsRepair == "Repair")
            {
                #region 注释


                //List<ReworkItem> reworkList = pData.ReworkList;
                ////------------将组装出料件的数据插入到表中业务--------------
                //List<Product_Rework_Info> newReworkList = new List<Product_Rework_Info>();
                ////获取对应的制程的flowchartdetailuid
                //var detailUidList = reworkList.Select(m => m.reworkDetailUid).Distinct().ToList();
                ////获取对应的制程的信息进行匹配
                ////var OQCList = productReworkInfoRepository.GetReworkMatch(detailUidList, item.FlowChart_Detail_UID.Value, DateTime.Now, nowInterval);

                ////查找Rework里面是否存在OQC返工的数据
                //List<Product_Rework_Info> newReworkInfoList = new List<Product_Rework_Info>();
                ////计算WIP数量
                //var inputQty = reworkList.Where(m => m.reworkOper.Substring(m.reworkOper.Length - 1, 1) == "入").Sum(m => m.reworkQty);
                //var outputQty = reworkList.Where(m => m.reworkOper.Substring(m.reworkOper.Length - 1, 1) == "出").Sum(m => m.reworkQty);
                //newItem.WIP_QTY = newItem.WIP_QTY + inputQty - outputQty;
                //foreach (var reworkItem in reworkList)
                //{
                //    if (onlyInsertReworkTable)
                //    {
                //        string sql = @"INSERT INTO dbo.Product_Rework_Info
                //                                    ( 
                //                                      FlowChart_Detail_UID ,
                //                                      Opposite_Detail_UID ,
                //                                      Opposite_QTY ,
                //                                      Product_Date ,
                //                                      Time_Interval ,
                //                                      Is_Match ,
                //                                      Rework_Type ,
                //                                      Rework_Flag ,
                //                                      Modified_UID ,
                //                                      Modified_Date
                //                                    )
                //                            VALUES  ( 
                //                                      {0} , -- FlowChart_Detail_UID - int
                //                                      {1} , -- Opposite_Detail_UID - int
                //                                      {2} , -- Opposite_QTY - int
                //                                      '{3}' , -- Product_Date - date
                //                                      N'{4}' , -- Time_Interval - nvarchar(20)
                //                                      {5} , -- Is_Match - bit
                //                                      N'{6}' , -- Rework_Type - nvarchar(20)
                //                                      N'{7}' , -- Rework_Flag - nvarchar(20)
                //                                      {8} , -- Modified_UID - int
                //                                      '{9}'  -- Modified_Date - datetime
                //                                    );";

                //        switch (reworkItem.reworkOper.Substring(reworkItem.reworkOper.Length - 1, 1))
                //        {
                //            case "入":
                //                sql = string.Format(sql,
                //                    newItem.FlowChart_Detail_UID,
                //                    reworkItem.reworkDetailUid,
                //                    reworkItem.reworkQty,
                //                    newItem.Product_Date,
                //                    Time_Interval,
                //                    1,
                //                    "Input",
                //                    "Repair",
                //                    newItem.Modified_UID,
                //                    newItem.Modified_Date);
                //                insertSqlList.Add(sql);
                //                break;
                //            case "出":
                //                sql = string.Format(sql,
                //                    newItem.FlowChart_Detail_UID,
                //                    reworkItem.reworkDetailUid,
                //                    reworkItem.reworkQty,
                //                    newItem.Product_Date,
                //                    Time_Interval,
                //                    1,
                //                    "Output",
                //                    "Repair",
                //                    newItem.Modified_UID,
                //                    newItem.Modified_Date);
                //                insertSqlList.Add(sql);
                //                break;
                //        }
                //    }
                //    else
                //    {
                //        Product_Rework_Info newInfoItem = new Product_Rework_Info();
                //        newInfoItem.FlowChart_Detail_UID = newItem.FlowChart_Detail_UID;
                //        newInfoItem.Opposite_Detail_UID = reworkItem.reworkDetailUid;
                //        newInfoItem.Opposite_QTY = reworkItem.reworkQty;
                //        newInfoItem.Product_Date = newItem.Product_Date;
                //        newInfoItem.Time_Interval = Time_Interval;
                //        newInfoItem.Modified_UID = newItem.Modified_UID;
                //        newInfoItem.Modified_Date = newItem.Modified_Date;
                //        newInfoItem.Rework_Flag = "Repair";
                //        newInfoItem.Is_Match = true;
                //        switch (reworkItem.reworkOper.Substring(reworkItem.reworkOper.Length - 1, 1))
                //        {
                //            case "入":
                //                newInfoItem.Rework_Type = "Input";
                //                break;
                //            case "出":
                //                newInfoItem.Rework_Type = "Output";
                //                break;
                //        }
                //        newItem.Product_Rework_Info.Add(newInfoItem);
                //    }

                //}

                #endregion

                foreach (var item in pData.ProductRepairInfoVM)
                {
                    var sql = @"INSERT INTO dbo.Product_Rework_Info
                                ( 
                                    FlowChart_Detail_UID ,
                                    Opposite_Detail_UID ,
                                    Opposite_QTY ,
                                    Product_Date ,
                                    Time_Interval ,
                                    Is_Match ,
                                    Rework_Type ,
                                    Rework_Flag ,
                                    Modified_UID ,
                                    Modified_Date
                                )
                        VALUES  ( 
                                    {0} , -- FlowChart_Detail_UID - int
                                    {1} , -- Opposite_Detail_UID - int
                                    {2} , -- Opposite_QTY - int
                                    '{3}' , -- Product_Date - date
                                    N'{4}' , -- Time_Interval - nvarchar(20)
                                    {5} , -- Is_Match - bit
                                    N'{6}' , -- Rework_Type - nvarchar(20)
                                    N'{7}' , -- Rework_Flag - nvarchar(20)
                                    {8} , -- Modified_UID - int
                                    '{9}'  -- Modified_Date - datetime
                                );";
                    sql = string.Format(sql,
                        item.FlowChart_Detail_UID,
                        item.Opposite_Detail_UID,
                        item.Opposite_QTY,
                        newItem.Product_Date,
                        Time_Interval,
                        1,
                        item.Rework_Type,
                        StructConstants.ReworkFlag.Repair,
                        newItem.Modified_UID,
                        newItem.Modified_Date);
                    insertSqlList.Add(sql);
                    //计算WIP
                    if (item.Rework_Type == "Input")
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY + item.Opposite_QTY;

                    }
                    else
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY - item.Opposite_QTY;
                    }
                    if (newItem.WIP_QTY != null)
                        flowchartRepository.GetById(item.FlowChart_Detail_UID).WIP_QTY = int.Parse(newItem.WIP_QTY.ToString());
                }

            }
            #endregion
            return errorInfo;
        }

        private string AddWipQtyByReowrkOrRepair(ProductDataItem pData, Product_Input_Location newItem)
        {
            #region Rework
            if (pData.IsRepair == "Rework")
            {
                foreach (var item in pData.ProductReworkInfoVM)
                {
                    //计算WIP
                    if (item.Rework_Type == "Input")
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY + item.Opposite_QTY;

                    }
                    else
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY - item.Opposite_QTY;
                    }
                    if (newItem.WIP_QTY < 0)
                        return pData.Process + "_" + pData.Color + "_" + pData.Place + "　　WIP总数不能为负,当前WIP为：(" + newItem.WIP_QTY + ") 请核实数据或者找总战情修改WIP，谢谢";
                }
            }
            #endregion

            #region Repair
            if (pData.IsRepair == "Repair")
            {
                foreach (var item in pData.ProductRepairInfoVM)
                {
                    //计算WIP
                    if (item.Rework_Type == "Input")
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY + item.Opposite_QTY;

                    }
                    else
                    {
                        newItem.WIP_QTY = newItem.WIP_QTY - item.Opposite_QTY;
                    }
                    if (newItem.WIP_QTY < 0)
                        return pData.Process + "_" + pData.Color + "_" + pData.Place + "　　WIP总数不能为负,当前WIP为：(" + newItem.WIP_QTY + ") 请核实数据或者找总战情修改WIP，谢谢";
                }
            }
            #endregion
            return string.Empty;
        }


        private void ModifyReworkAndRepairIsMatch(List<ProductDataItem> pDataList, DateTime nowDate, string Time_Interval)
        {
            List<Product_Rework_Info> allList = new List<Product_Rework_Info>();
            List<RepeatItem> RepeatList = new List<RepeatItem>();
            //var reworkIdList = pDataList.Where(m => m.IsRepair == "Rework").Select(m => m.FlowChart_Detail_UID).ToList();
            //var repairIdList = pDataList.Where(m => m.IsRepair == "Repair").Select(m => m.FlowChart_Detail_UID).ToList();

            //var reworkList = productReworkInfoRepository.GetMany(m => reworkIdList.Contains(m.FlowChart_Detail_UID) && m.Product_Date == nowDate && m.Time_Interval == Time_Interval).ToList();
            //var repairList = productReworkInfoRepository.GetMany(m => repairIdList.Contains(m.FlowChart_Detail_UID) && m.Product_Date == nowDate && m.Time_Interval == Time_Interval).ToList();
            var flowchartMasterUID = pDataList.First().FlowChart_Master_UID;
            var version = pDataList.First().FlowChart_Version;
            var currentTime = DateTime.Now;
            var curUser = pDataList.First().Modified_UID;
            var flowchartDetailUIDList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == flowchartMasterUID && m.FlowChart_Version == version).Select(m => m.FlowChart_Detail_UID).ToList();


            var reworkList = productReworkInfoRepository.GetMany(m => flowchartDetailUIDList.Contains(m.FlowChart_Detail_UID) && m.Product_Date == nowDate && m.Time_Interval == Time_Interval && m.Rework_Flag == StructConstants.ReworkFlag.Rework).ToList();
            var repairList = productReworkInfoRepository.GetMany(m => flowchartDetailUIDList.Contains(m.FlowChart_Detail_UID) && m.Product_Date == nowDate && m.Time_Interval == Time_Interval && m.Rework_Flag == StructConstants.ReworkFlag.Repair).ToList();

            allList.AddRange(reworkList);
            allList.AddRange(repairList);

            foreach (var item in reworkList)
            {
                switch (item.Rework_Type)
                {
                    case "Input":
                        var matchItemA = repairList.Where(m => m.FlowChart_Detail_UID == item.Opposite_Detail_UID && m.Opposite_Detail_UID == item.FlowChart_Detail_UID
                        && m.Rework_Type == "Output").FirstOrDefault();
                        if (matchItemA != null)
                        {
                            item.Modified_Date = currentTime;
                            item.Modified_UID = curUser;
                            matchItemA.Modified_Date = currentTime;
                            matchItemA.Modified_UID = curUser;
                            if (item.Opposite_QTY != matchItemA.Opposite_QTY)
                            {
                                item.Is_Match = false;
                                matchItemA.Is_Match = false;
                            }
                            else
                            {
                                item.Is_Match = true;
                                matchItemA.Is_Match = true;
                            }
                            //移除掉Repair集合的元素
                            repairList.Remove(matchItemA);
                        }
                        break;
                    case "Output":
                        var matchItemB = repairList.Where(m => m.FlowChart_Detail_UID == item.Opposite_Detail_UID && m.Opposite_Detail_UID == item.FlowChart_Detail_UID
                        && m.Rework_Type == "Input").FirstOrDefault();
                        if (matchItemB != null)
                        {
                            item.Modified_Date = currentTime;
                            item.Modified_UID = curUser;
                            matchItemB.Modified_Date = currentTime;
                            matchItemB.Modified_UID = curUser;

                            if (item.Opposite_QTY != matchItemB.Opposite_QTY)
                            {
                                item.Is_Match = false;
                                matchItemB.Is_Match = false;
                            }
                            else
                            {
                                item.Is_Match = true;
                                matchItemB.Is_Match = true;
                            }
                            //移除掉Repair集合的元素
                            repairList.Remove(matchItemB);
                        }
                        break;
                }
            }

            foreach (var item in repairList)
            {
                switch (item.Rework_Type)
                {
                    case "Input":
                        var matchItemA = reworkList.Where(m => m.FlowChart_Detail_UID == item.Opposite_Detail_UID && m.Opposite_Detail_UID == item.FlowChart_Detail_UID
                        && m.Rework_Type == "Output").FirstOrDefault();
                        if (matchItemA != null)
                        {
                            item.Modified_Date = currentTime;
                            item.Modified_UID = curUser;
                            matchItemA.Modified_Date = currentTime;
                            matchItemA.Modified_UID = curUser;
                            if (item.Opposite_QTY != matchItemA.Opposite_QTY)
                            {
                                item.Is_Match = false;
                                matchItemA.Is_Match = false;
                            }
                            else
                            {
                                item.Is_Match = true;
                                matchItemA.Is_Match = true;
                            }
                        }
                        break;
                    case "Output":
                        var matchItemB = reworkList.Where(m => m.FlowChart_Detail_UID == item.Opposite_Detail_UID && m.Opposite_Detail_UID == item.FlowChart_Detail_UID
                        && m.Rework_Type == "Input").FirstOrDefault();
                        if (matchItemB != null)
                        {
                            item.Modified_Date = currentTime;
                            item.Modified_UID = curUser;
                            matchItemB.Modified_Date = currentTime;
                            matchItemB.Modified_UID = curUser;
                            if (item.Opposite_QTY != matchItemB.Opposite_QTY)
                            {
                                item.Is_Match = false;
                                matchItemB.Is_Match = false;
                            }
                            else
                            {
                                item.Is_Match = true;
                                matchItemB.Is_Match = true;
                            }
                        }
                        break;
                }
            }
            unitOfWork.Commit();
        }

        /// <summary>
        /// 获取制定的制程有多少楼栋
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public int getFlowchartBuildingCount(int master_UID, int FlowChart_Version, int Process_Seq, string color)
        {
            return flowChartMasterRepository.getFlowchartBuildingCount(master_UID, FlowChart_Version, Process_Seq, color);
        }

        public int getFlowchartMaxUID(int master_UID, int FlowChart_Version, int Process_Seq, string color)
        {
            return flowChartMasterRepository.getFlowchartMaxUID(master_UID, FlowChart_Version, Process_Seq, color);
        }

        /// <summary>
        /// 获取制定的制程的楼栋详细数据列表
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public List<Product_Input_Location> getDetailBuildingList(int master_UID, int FlowChart_Version, int Process_Seq, string color, DateTime productDate, string TimeInterval)
        {
            return flowChartMasterRepository.getDetailBuildingList(master_UID, FlowChart_Version, Process_Seq, color, productDate, TimeInterval);
        }
        public string SaveInfoAndRework(ProductDataItem dto, int AccountId)
        {
            string errorInfo = string.Empty;
            using (TransactionScope scope = new TransactionScope())
            {
                //需要先判断是否为多楼栋数据
                //多楼栋的数据，修改产品详细表数据，并判断是否多个楼栋数据已录完（在Product_Input表中是否有对应数据。）
                //若有数据，需要将Product_Input的wip数据也要修改

                List<Product_Rework_Info> reworkList = new List<Product_Rework_Info>();

                var productItem = productInputRepository.GetById(dto.Product_UID);
                if (productItem.Is_Comfirm)
                {
                    errorInfo = "PP已经保存数据，不能修改，请刷新页面";
                    return errorInfo;
                }
                productItem.Picking_QTY = dto.Picking_QTY;
                productItem.WH_Picking_QTY = dto.WH_Picking_QTY;
                productItem.Good_QTY = dto.Normal_Good_QTY + dto.Abnormal_Good_QTY;
                productItem.Adjust_QTY = dto.Adjust_QTY;
                productItem.NG_QTY = dto.Normal_NG_QTY + dto.Abnormal_NG_QTY;
                productItem.WH_QTY = dto.WH_QTY;
                productItem.NullWip_QTY = dto.NullWip_QTY;
                //Q数据准备
                productItem.Normal_Good_QTY = dto.Normal_Good_QTY;
                productItem.Abnormal_Good_QTY = dto.Abnormal_Good_QTY;
                productItem.Normal_NG_QTY = dto.Normal_NG_QTY;
                productItem.Abnormal_NG_QTY = dto.Abnormal_NG_QTY;
                productItem.Unacommpolished_Reason = dto.Unacommpolished_Reason;
                //修改wip的值
                var flowchartItem = flowChartDetailRepository.GetById(dto.FlowChart_Detail_UID);
                var lastWip =
                    productInputRepository.GetMany(m => m.Product_UID == dto.Product_UID)
                        .Select(m => m.Picking_QTY + m.WH_Picking_QTY - m.Good_QTY - m.NG_QTY - m.Adjust_QTY - m.WH_QTY)
                        .FirstOrDefault();
                var lastNullWip =
                  productInputRepository.GetMany(m => m.Product_UID == dto.Product_UID)
                      .Select(m => m.NullWip_QTY)
                      .FirstOrDefault();

                var nowWip = productItem.Picking_QTY + productItem.WH_Picking_QTY - productItem.NG_QTY -
                             productItem.Adjust_QTY - productItem.Good_QTY - productItem.WH_QTY;
                //计算出不包含rework和repair的wip_qty，下面还有包含rework和repair的计算  将原来的不可用加上-现在的不可用WIP
                productItem.WIP_QTY = flowchartItem.WIP_QTY + nowWip - lastWip + lastNullWip - productItem.NullWip_QTY;



                var flowchartMst = flowChartMasterRepository.GetFirstOrDefault(m => m.FlowChart_Master_UID == productItem.FlowChart_Master_UID);
                var project = systemProjectRepository.GetFirstOrDefault(m => m.Project_UID == flowchartMst.Project_UID);
                var orgbom = systemOrgBomRepository.GetFirstOrDefault(m => m.ChildOrg_UID == project.Organization_UID);
                var org = systemOrgRepository.GetFirstOrDefault(m => m.Organization_UID == orgbom.ParentOrg_UID);

                //获取出上一次的wip_Qty的数据，rework和repair算法都一样，repair把多个制程的数据也汇总到一起
                reworkList = productInputRepository.getReworkList(productItem.FlowChart_Detail_UID, productItem.Product_Date, productItem.Time_Interval);
                //计算上一次的wip_QTY的值
                var lastInputQty = reworkList.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                var lastOutputQty = reworkList.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);

                //计算这一次修改过的Wip_QTY的值
                var nowInputQty = 0;
                var nowOutputQty = 0;

                switch (dto.IsRepair)
                {
                    case StructConstants.ReworkFlag.Rework:
                        nowInputQty = dto.ProductReworkInfoVM.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                        nowOutputQty = dto.ProductReworkInfoVM.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);
                        break;
                    case StructConstants.ReworkFlag.Repair:
                        nowInputQty = dto.ProductRepairInfoVM.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                        nowOutputQty = dto.ProductRepairInfoVM.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);
                        break;
                }

                //计算出包含rework和repair的wip_qty
                var nowWipQty = (nowInputQty - nowOutputQty) - (lastInputQty - lastOutputQty);
                //更新ProductInput的WIP_QTY
                productItem.WIP_QTY = productItem.WIP_QTY + nowWipQty;
                //更新Flowchart对的WIP_QTY
                flowchartItem.WIP_QTY = productItem.WIP_QTY.Value;
                flowchartItem.NullWip = productItem.NullWip_QTY.Value;

                if (productItem.WIP_QTY < 0)
                    return productItem.Process + "_" + productItem.Color + "_" + productItem.Place + "　　WIP总数不能为负,当前WIP为：(" + productItem.WIP_QTY + ") 请核实数据或者找总战情修改WIP，谢谢";
                //更新Rework表出入数量
                StringBuilder sb = new StringBuilder();
                switch (dto.IsRepair)
                {
                    case StructConstants.ReworkFlag.Repair:
                        foreach (var item in dto.ProductRepairInfoVM)
                        {
                            var hasItem = productReworkInfoRepository.GetById(item.Rework_UID);
                            hasItem.Opposite_QTY = item.Opposite_QTY;
                            hasItem.Modified_UID = AccountId;


                            //var sql = @"UPDATE dbo.Product_Rework_Info SET Opposite_QTY = {0}, Modified_UID={1}, Modified_Date=GETDATE()
                            //    WHERE Rework_UID={2};";
                            //sql = string.Format(sql, item.Opposite_QTY, AccountId, item.Rework_UID);
                            //sb.AppendLine(sql);
                        }
                        break;
                    case StructConstants.ReworkFlag.Rework:
                        foreach (var item in dto.ProductReworkInfoVM)
                        {
                            //var sql = @"UPDATE dbo.Product_Rework_Info SET Opposite_QTY = {0}, Modified_UID={1}, Modified_Date=GETDATE()
                            //    WHERE Rework_UID={2};";
                            //sql = string.Format(sql, item.Opposite_QTY, AccountId, item.Rework_UID);
                            //sb.AppendLine(sql);

                            var hasItem = productReworkInfoRepository.GetById(item.Rework_UID);
                            hasItem.Opposite_QTY = item.Opposite_QTY;
                            hasItem.Modified_UID = AccountId;

                        }
                        break;
                }
                //productInputRepository.ModifyReworkNumInfo(sb);

                //匹配Match
                List<ProductDataItem> list = new List<ProductDataItem>();
                dto.FlowChart_Master_UID = productItem.FlowChart_Master_UID;
                dto.FlowChart_Version = productItem.FlowChart_Version;
                dto.Modified_UID = productItem.Modified_UID;
                list.Add(dto);

                unitOfWork.Commit();
                //上面的语句需要先提交才行，不然获取的匹配数量就不是画面的数量
                ModifyReworkAndRepairIsMatch(list, productItem.Product_Date, productItem.Time_Interval);

                #region 注释
                //if (reworkList.Any())
                //{
                //    var detailUIDList = reworkList.Select(m => m.FlowChart_Detail_UID).ToList();
                //    var oppoDetailUIDList = reworkList.Select(m => m.Opposite_Detail_UID).ToList();
                //    var productDate = reworkList.First().Product_Date;
                //    var timeInterval = reworkList.First().Time_Interval;
                //    var dateNow = DateTime.Now;
                //    //获取所有相匹配的数据
                //    var matchReworkList = productReworkInfoRepository.GetMany(m => detailUIDList.Contains(m.Opposite_Detail_UID) && oppoDetailUIDList.Contains(m.FlowChart_Detail_UID)
                //    && m.Product_Date == productDate && m.Time_Interval == timeInterval).ToList();
                //    foreach (var item in dto.ReworkList)
                //    {
                //        var hasReworkItem = reworkList.First(m => m.Rework_UID == item.Rework_UID);
                //        //更新数据
                //        hasReworkItem.Opposite_QTY = item.reworkQty;
                //        hasReworkItem.Modified_Date = dateNow;
                //        hasReworkItem.Modified_UID = AccountId;

                //        Product_Rework_Info updateReworkItem = null;
                //        switch (item.Rework_Type)
                //        {
                //            case "Input":
                //                //查找相匹配的数据
                //                updateReworkItem = matchReworkList.FirstOrDefault(m => m.FlowChart_Detail_UID == hasReworkItem.Opposite_Detail_UID && m.Opposite_Detail_UID == hasReworkItem.FlowChart_Detail_UID
                //                                                                       && m.Product_Date == productDate && m.Time_Interval == timeInterval && m.Rework_Type == "Output");
                //                if (updateReworkItem != null)
                //                {
                //                    if (hasReworkItem.Opposite_QTY != updateReworkItem.Opposite_QTY)
                //                    {
                //                        hasReworkItem.Is_Match = false;
                //                        updateReworkItem.Is_Match = false;

                //                    }
                //                    else
                //                    {
                //                        hasReworkItem.Is_Match = true;
                //                        updateReworkItem.Is_Match = true;
                //                    }
                //                    updateReworkItem.Modified_Date = dateNow;
                //                }

                //                break;
                //            case "Output":
                //                //查找相匹配的数据
                //                updateReworkItem = matchReworkList.FirstOrDefault(m => m.FlowChart_Detail_UID == hasReworkItem.Opposite_Detail_UID && m.Opposite_Detail_UID == hasReworkItem.FlowChart_Detail_UID
                //                                                                       && m.Product_Date == productDate && m.Time_Interval == timeInterval && m.Rework_Type == "Input");
                //                if (updateReworkItem != null)
                //                {
                //                    if (hasReworkItem.Opposite_QTY != updateReworkItem.Opposite_QTY)
                //                    {
                //                        hasReworkItem.Is_Match = false;
                //                        updateReworkItem.Is_Match = false;
                //                    }
                //                    else
                //                    {
                //                        hasReworkItem.Is_Match = true;
                //                        updateReworkItem.Is_Match = true;
                //                    }
                //                    updateReworkItem.Modified_Date = dateNow;
                //                }
                //                break;
                //        }
                //    }
                //}
                #endregion


                productInputRepository.ExecAlterSp(productItem);
                scope.Complete();
            }
            return "success";
        }

        public string SaveLocationInfoAndRework(ProductDataItem dto, int AccountId)
        {
            string errorInfo = string.Empty;

            using (TransactionScope scope = new TransactionScope())
            {
                //需要先判断是否为多楼栋数据
                //多楼栋的数据，修改产品详细表数据，并判断是否多个楼栋数据已录完（在Product_Input表中是否有对应数据。）
                //若有数据，需要将Product_Input的wip数据也要修改

                List<Product_Rework_Info> reworkList = new List<Product_Rework_Info>();


                var productItem = product_Input_LocationRepository.GetById(dto.Product_UID);
                if (productItem.Is_Comfirm)
                {
                    errorInfo = "PP已经保存数据，不能修改，请刷新页面";
                    return errorInfo;
                }
                productItem.Picking_QTY = dto.Picking_QTY;
                productItem.WH_Picking_QTY = dto.WH_Picking_QTY;
                productItem.Good_QTY = dto.Normal_Good_QTY + dto.Abnormal_Good_QTY;
                productItem.Adjust_QTY = dto.Adjust_QTY;
                productItem.NG_QTY = dto.Normal_NG_QTY + dto.Abnormal_NG_QTY;
                productItem.WH_QTY = dto.WH_QTY;
                productItem.NullWip_QTY = dto.NullWip_QTY;
                //Q数据准备
                productItem.Normal_Good_QTY = dto.Normal_Good_QTY;
                productItem.Abnormal_Good_QTY = dto.Abnormal_Good_QTY;
                productItem.Normal_NG_QTY = dto.Normal_NG_QTY;
                productItem.Abnormal_NG_QTY = dto.Abnormal_NG_QTY;

                productItem.Unacommpolished_Reason = dto.Unacommpolished_Reason;

                //修改wip的值
                var flowchartItem = flowChartDetailRepository.GetById(dto.FlowChart_Detail_UID);
                var lastWip =
                    product_Input_LocationRepository.GetMany(m => m.Product_Input_Location_UID == dto.Product_UID)
                        .Select(m => m.Picking_QTY + m.WH_Picking_QTY - m.Good_QTY - m.NG_QTY - m.Adjust_QTY - m.WH_QTY)
                        .FirstOrDefault();
                var lastNullWip =
                 product_Input_LocationRepository.GetMany(m => m.Product_Input_Location_UID == dto.Product_UID)
                     .Select(m => m.NullWip_QTY)
                     .FirstOrDefault();
                var nowWip = productItem.Picking_QTY + productItem.WH_Picking_QTY - productItem.NG_QTY -
                             productItem.Adjust_QTY - productItem.Good_QTY - productItem.WH_QTY;
                //计算出不包含rework和repair的wip_qty，下面还有包含rework和repair的计算
                productItem.WIP_QTY = flowchartItem.WIP_QTY + nowWip - lastWip + lastNullWip - productItem.NullWip_QTY;

                var flowchartMst = flowChartMasterRepository.GetFirstOrDefault(m => m.FlowChart_Master_UID == productItem.FlowChart_Master_UID);
                var project = systemProjectRepository.GetFirstOrDefault(m => m.Project_UID == flowchartMst.Project_UID);
                var orgbom = systemOrgBomRepository.GetFirstOrDefault(m => m.ChildOrg_UID == project.Organization_UID);
                var org = systemOrgRepository.GetFirstOrDefault(m => m.Organization_UID == orgbom.ParentOrg_UID);

                //这边取到的值有多个楼栋的返工信息
                reworkList = productInputRepository.getReworkList(productItem.FlowChart_Detail_UID, productItem.Product_Date, productItem.Time_Interval);
                //计算上一次的wip_QTY的值
                var lastInputQty = reworkList.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                var lastOutputQty = reworkList.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);

                //计算这一次修改过的Wip_QTY的值
                var nowInputQty = 0;
                var nowOutputQty = 0;
                switch (dto.IsRepair)
                {
                    case StructConstants.ReworkFlag.Rework:
                        nowInputQty = dto.ProductReworkInfoVM.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                        nowOutputQty = dto.ProductReworkInfoVM.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);
                        break;
                    case StructConstants.ReworkFlag.Repair:
                        nowInputQty = dto.ProductRepairInfoVM.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
                        nowOutputQty = dto.ProductRepairInfoVM.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);
                        break;
                }

                //计算出包含rework和repair的wip_qty
                var nowWipQty = (nowInputQty - nowOutputQty) - (lastInputQty - lastOutputQty);
                //更新ProductInput的WIP_QTY
                productItem.WIP_QTY = productItem.WIP_QTY + nowWipQty;
                //更新Flowchart对的WIP_QTY
                flowchartItem.WIP_QTY = productItem.WIP_QTY.Value;
                flowchartItem.NullWip = productItem.NullWip_QTY.Value;
                if (productItem.WIP_QTY < 0)
                    return productItem.Process + "_" + productItem.Color + "_" + productItem.Place + "　　WIP总数不能为负,当前WIP为：(" + productItem.WIP_QTY + ") 请核实数据或者找总战情修改WIP，谢谢";
                //判断该楼栋数据是否全部录入完全，如果全部录入完全，还需要修改Product_Input表数据
                //1先获取该制程有多少楼栋
                int PlaceCount = getFlowchartBuildingCount(productItem.FlowChart_Master_UID, productItem.FlowChart_Version, productItem.Process_Seq, productItem.Color);
                //2 获取详细表i该时段有多少数据
                var BuildingList = getDetailBuildingList(productItem.FlowChart_Master_UID, productItem.FlowChart_Version, productItem.Process_Seq, productItem.Color, productItem.Product_Date, productItem.Time_Interval);
                int DetailCount = BuildingList.Count();

                // 如果楼栋数据表中的个数和Flowchart中楼栋数相同，将数据合并并写入到Product_Input
                if (PlaceCount == DetailCount)
                {
                    //合并各项数据
                    int SumGood_QTY = 0; int SumNG_QTY = 0; int? SumWIP = 0; int SumPicking_QTY = 0;
                    int SumWH_Picking_QTY = 0; int SumWH_QTY = 0; int SumAdjust_QTY = 0;
                    int SumAbnormal_Good_QTY = 0; int SumAbnormal_NG_QTY = 0;
                    int SumNormal_Good_QTY = 0;
                    int SumNormal_NG_QTY = 0;
                    int? SumNullWIP = 0;
                    string uReason = string.Empty;
                    //获取该制程在楼栋详细表中的数据列表
                    foreach (var Building in BuildingList)
                    {
                        SumGood_QTY += Building.Good_QTY;
                        SumNG_QTY += Building.NG_QTY;
                        SumWIP += Building.WIP_QTY;
                        SumPicking_QTY += Building.Picking_QTY;
                        SumWH_Picking_QTY += Building.WH_Picking_QTY;
                        SumWH_QTY += Building.WH_QTY;
                        SumAdjust_QTY += Building.Adjust_QTY;
                        SumAbnormal_Good_QTY += Building.Abnormal_Good_QTY;
                        SumAbnormal_NG_QTY += Building.Abnormal_NG_QTY;
                        SumNormal_Good_QTY += Building.Normal_Good_QTY;
                        SumNormal_NG_QTY += Building.Normal_NG_QTY;
                        SumNullWIP += Building.NullWip_QTY;
                        if (!string.IsNullOrWhiteSpace(Building.Unacommpolished_Reason))
                        {
                            if (!string.IsNullOrWhiteSpace(uReason))
                                uReason = uReason + "_" + Building.Unacommpolished_Reason;
                            else
                                uReason = Building.Unacommpolished_Reason;
                        }
                    }

                    //获取该数据对应的Product_Input数据，并更新该对象
                    var PdItem = flowChartMasterRepository.getRealtedProduct(productItem.FlowChart_Master_UID, productItem.FlowChart_Version, productItem.Process_Seq, productItem.Color, productItem.Product_Date, productItem.Time_Interval);
                    PdItem.Good_QTY = SumGood_QTY;
                    PdItem.NG_QTY = SumNG_QTY;
                    PdItem.WIP_QTY = SumWIP;
                    PdItem.NullWip_QTY = SumNullWIP;
                    PdItem.Picking_QTY = SumPicking_QTY;
                    PdItem.WH_QTY = SumWH_QTY;
                    PdItem.Adjust_QTY = SumAdjust_QTY;
                    PdItem.Abnormal_Good_QTY = SumAdjust_QTY;
                    PdItem.Abnormal_NG_QTY = SumAbnormal_NG_QTY;
                    PdItem.WH_Picking_QTY = SumWH_Picking_QTY;
                    PdItem.Normal_Good_QTY = SumNG_QTY;
                    PdItem.NG_QTY = SumNG_QTY;
                    PdItem.Unacommpolished_Reason = uReason;
                }


                //更新Rework表出入数量
                StringBuilder sb = new StringBuilder();
                switch (dto.IsRepair)
                {
                    case StructConstants.ReworkFlag.Repair:
                        foreach (var item in dto.ProductRepairInfoVM)
                        {
                            var hasItem = productReworkInfoRepository.GetById(item.Rework_UID);
                            hasItem.Opposite_QTY = item.Opposite_QTY;
                            hasItem.Modified_UID = AccountId;


                            //var sql = @"UPDATE dbo.Product_Rework_Info SET Opposite_QTY = {0}, Modified_UID={1}, Modified_Date=GETDATE()
                            //    WHERE Rework_UID={2};";
                            //sql = string.Format(sql, item.Opposite_QTY, AccountId, item.Rework_UID);
                            //sb.AppendLine(sql);
                        }
                        break;
                    case StructConstants.ReworkFlag.Rework:
                        foreach (var item in dto.ProductReworkInfoVM)
                        {
                            //var sql = @"UPDATE dbo.Product_Rework_Info SET Opposite_QTY = {0}, Modified_UID={1}, Modified_Date=GETDATE()
                            //    WHERE Rework_UID={2};";
                            //sql = string.Format(sql, item.Opposite_QTY, AccountId, item.Rework_UID);
                            //sb.AppendLine(sql);

                            var hasItem = productReworkInfoRepository.GetById(item.Rework_UID);
                            hasItem.Opposite_QTY = item.Opposite_QTY;
                            hasItem.Modified_UID = AccountId;

                        }
                        break;
                }

                //productInputRepository.ModifyReworkNumInfo(sb);

                //匹配Match
                List<ProductDataItem> list = new List<ProductDataItem>();
                dto.FlowChart_Master_UID = productItem.FlowChart_Master_UID;
                dto.FlowChart_Version = productItem.FlowChart_Version;
                dto.Modified_UID = productItem.Modified_UID;
                list.Add(dto);

                unitOfWork.Commit();
                //上面的语句需要先提交才行，不然获取的匹配数量就不是画面的数量
                ModifyReworkAndRepairIsMatch(list, productItem.Product_Date, productItem.Time_Interval);



                Product_Input product = new Product_Input();
                product.FlowChart_Master_UID = productItem.FlowChart_Master_UID;
                product.FlowChart_Version = productItem.FlowChart_Version;
                product.Product_Date = productItem.Product_Date;
                product.Time_Interval = productItem.Time_Interval;
                productInputRepository.ExecAlterSp(product);
                scope.Complete();
            }
            return "success";
        }


        public void SaveInfoEmergency(ProductDataItem dto, int AccountId)
        {
            List<Product_Rework_Info> reworkList = new List<Product_Rework_Info>();
            var productItem = productInputRepository.GetById(dto.Product_UID);
            productItem.Picking_QTY = dto.Picking_QTY;
            productItem.WH_Picking_QTY = dto.WH_Picking_QTY;
            productItem.Good_QTY = dto.Normal_Good_QTY + dto.Abnormal_Good_QTY;
            productItem.Adjust_QTY = dto.Adjust_QTY;
            productItem.NG_QTY = dto.Normal_NG_QTY + dto.Abnormal_NG_QTY;
            productItem.WH_QTY = dto.WH_QTY;
            //Q数据准备
            productItem.Normal_Good_QTY = dto.Normal_Good_QTY;
            productItem.Abnormal_Good_QTY = dto.Abnormal_Good_QTY;
            productItem.Normal_NG_QTY = dto.Normal_NG_QTY;
            productItem.Abnormal_NG_QTY = dto.Abnormal_NG_QTY;

            //修改wip的值
            var flowchartItem = flowChartDetailRepository.GetById(productItem.FlowChart_Detail_UID);
            var lastWip =
                productInputRepository.GetMany(m => m.Product_UID == dto.Product_UID)
                    .Select(m => m.Picking_QTY + m.WH_Picking_QTY - m.Good_QTY - m.NG_QTY - m.Adjust_QTY - m.WH_QTY)
                    .FirstOrDefault();
            var nowWip = productItem.Picking_QTY + productItem.WH_Picking_QTY - productItem.NG_QTY -
                         productItem.Adjust_QTY - productItem.Good_QTY - productItem.WH_QTY;
            //计算出不包含rework和repair的wip_qty，下面还有包含rework和repair的计算
            productItem.WIP_QTY = flowchartItem.WIP_QTY + nowWip - lastWip;

            //获取出上一次的wip_Qty的数据，rework和repair算法都一样，repair把多个制程的数据也汇总到一起
            reworkList = productItem.Product_Rework_Info.ToList();
            //计算上一次的wip_QTY的值
            var lastInputQty = reworkList.Where(m => m.Rework_Type == "Input").Sum(m => m.Opposite_QTY);
            var lastOutputQty = reworkList.Where(m => m.Rework_Type == "Output").Sum(m => m.Opposite_QTY);

            //计算这一次修改过的Wip_QTY的值
            var nowInputQty = 0;
            var nowOutputQty = 0;
            if (dto.ReworkList != null)
            {
                nowInputQty = dto.ReworkList.Where(m => m.Rework_Type == "Input").Sum(m => m.reworkQty);
                nowOutputQty = dto.ReworkList.Where(m => m.Rework_Type == "Output").Sum(m => m.reworkQty);
            }

            //计算出包含rework和repair的wip_qty
            var nowWipQty = (nowInputQty - nowOutputQty) - (lastInputQty - lastOutputQty);
            //更新ProductInput的WIP_QTY
            productItem.WIP_QTY = productItem.WIP_QTY + nowWipQty;
            //更新Flowchart对的WIP_QTY
            flowchartItem.WIP_QTY = productItem.WIP_QTY.Value;

            unitOfWork.Commit();
            productInputRepository.ExecAlterSp(productItem);
        }



        /// <summary>
        /// 通过功能厂名获取功能厂对象
        /// </summary>
        /// <param name="funcPlant"></param>
        /// <returns></returns>
        public System_Function_Plant QueryFuncPlantInfo(string funcPlant)
        {
            return productInputRepository.QueryFuncPlantInfo(funcPlant);
        }

        /// <summary>
        /// 根据UID查询单条生产数据信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ProductDataVM QueryProductDataSingle(int uid, bool flag)
        {
            //首先判断该数据来源哪里
            ProductDataVM item = new ProductDataVM();
            if (flag)
            {
                var pdl = product_Input_LocationRepository.GetById(uid);
                item = AutoMapper.Mapper.Map<ProductDataVM>(pdl);
            }
            else
            {
                var ProductDatas = productInputRepository.GetById(uid);
                item = AutoMapper.Mapper.Map<ProductDataVM>(ProductDatas);
            }
            var flowchartItem = flowChartDetailRepository.GetById(item.FlowChart_Detail_UID);
            item.Rework_Flag = flowchartItem.Rework_Flag;
            item.Location_Flag = flowchartItem.Location_Flag;
            return item;
        }

        /// <summary>
        /// 根据条件 查询身材制程信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<ProductDataDTO> QueryProcessData(ProcessDataSearch searchModel, Page page)
        {
            var totalCount = 0;
            var ProductDatas = productInputRepository.QueryProcessData(searchModel, page, out totalCount);

            //IList<ProductDataDTO> ProductDatasDTO = new List<ProductDataDTO>();
            //bool isFork = false;
            //foreach (var pd in ProductDatas)
            //{
            //    var dto = AutoMapper.Mapper.Map<ProductDataDTO>(pd);
            //    ProductDatasDTO.Add(dto);
            //}
            return new PagedListModel<ProductDataDTO>(totalCount, ProductDatas);

        }

        public PagedListModel<ProductDataDTO> QueryProcessDataForEmergency(ProcessDataSearchModel searchModel, Page page)
        {
            var totalCount = 0;
            var ProductDatas = productInputRepository.QueryProcessDataForEmergency(searchModel, page, out totalCount);

            //IList<ProductDataDTO> ProductDatasDTO = new List<ProductDataDTO>();
            //bool isFork = false;
            //foreach (var pd in ProductDatas)
            //{
            //    var dto = AutoMapper.Mapper.Map<ProductDataDTO>(pd);
            //    ProductDatasDTO.Add(dto);
            //}
            return new PagedListModel<ProductDataDTO>(totalCount, ProductDatas);

        }

        public List<int> QueryExitProductData(ProcessDataSearch search)
        {
            var temp = productInputRepository.GetMany(
                m =>
                    m.Customer == search.Customer && m.Project == search.Project &&
                    m.Product_Phase == search.Product_Phase
                    && m.Part_Types == search.Part_Types && m.Product_Date == search.Date && m.Time_Interval == search.Time && m.IsLast == true).Select(m => m.FlowChart_Detail_UID);
            var temp1 = productInputRepository.GetMany(
               m =>
                   m.Customer == search.Customer && m.Project == search.Project &&
                   m.Product_Phase == search.Product_Phase
                   && m.Part_Types == search.Part_Types && m.Product_Date == search.Date && m.Time_Interval == search.Time && m.IsLast == true).Select(m => m.FlowChart_Detail_UID);

            var result = temp.Concat(temp1).ToList();
            return result;
        }


        /// <summary>
        /// 根据uid获取功能厂名字
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string GetCurrentPlantName(int uid)
        {
            return productInputRepository.GetCurrentPlantName(uid);
        }

        public PagedListModel<TimeSpanReportVM> QueryTimeSpanReport(ReportDataSearch searchModel)
        {
            var pplist = productInputRepository.QueryTimeSpanReport(searchModel);
            List<TimeSpanReportVM> result = new List<TimeSpanReportVM>();
            foreach (TimeSpanReport data in pplist)
            {
                result.Add(new TimeSpanReportVM
                {
                    Process = data.Process,
                    SumGoodQty = data.SumGoodQty,
                    SumPlan = data.SumPlan,
                    SumYieldRate = data.SumYieldRate.ToString("P"),
                    reachedReate = data.SumPlan == 0 ? 100 : Math.Round((data.SumGoodQty * 1.00 / data.SumPlan * 1.00) * 100.00, 2),

                });
            }
            return new PagedListModel<TimeSpanReportVM>(0, result);
        }

        public PagedListModel<ChartDailyReport> QueryTChartDailyData(ReportDataSearch searchModel)
        {
            var pplist = productInputRepository.QueryChartDailyData(searchModel);
            List<ChartDailyReport> result = new List<ChartDailyReport>();
            foreach (ChartDailyReport data in pplist)
            {
                result.Add(new ChartDailyReport
                {
                    Process = data.Process,
                    SumGoodQty = data.SumGoodQty,
                    SumPlan = data.SumPlan,
                    SumYieldRate = data.SumYieldRate,
                    reachedReate = data.SumPlan == 0 ? 100 : Math.Round((data.SumGoodQty * 1.00 / data.SumPlan * 1.00) * 100.00, 2),
                    WIP = data.WIP,
                    Proper_WIP = data.Proper_WIP
                });
            }
            return new PagedListModel<ChartDailyReport>(0, result);
        }


        //2016-12-20 add by karl 按时段查询报表

        public PagedListModel<TimeSpanReport_2> QueryTimeSpanReport_2(ReportDataSearch searchModel)
        {
            var pplist = productInputRepository.QueryTimeSpanReport_2(searchModel);
            List<TimeSpanReport_2> result = new List<TimeSpanReport_2>();
            foreach (TimeSpanReport_2 data in pplist)
            {
                result.Add(new TimeSpanReport_2
                {
                    IE_TargetEfficacy = data.IE_TargetEfficacy,
                    IE_DeptHuman = data.IE_TargetEfficacy,
                     SumIE_DeptHuman =data.SumIE_DeptHuman,
                    Process_Seq = data.Process_Seq,
                    Place = data.Place,
                    FunPlant = data.FunPlant,
                    Process = data.Process,
                    Color = data.Color,
                    DRI = data.DRI,
                    Target_Yield = data.Target_Yield,
                    Product_Plan = data.Product_Plan,
                    Picking_QTY = data.Picking_QTY,
                    WH_Picking_QTY = data.WH_Picking_QTY,
                    Good_QTY = data.Good_QTY,
                    Adjust_QTY = data.Adjust_QTY,
                    WH_QTY = data.WH_QTY,
                    NG_QTY = data.NG_QTY,
                    All_Finally_Achieving = data.All_Finally_Achieving,
                    All_Finally_Yield = data.All_Finally_Yield,
                    WIP_QTY = data.WIP_QTY,
                    NullWip_QTY = data.NullWip_QTY,
                    OK_QTY = data.OK_QTY
                });
            }
            return new PagedListModel<TimeSpanReport_2>(0, result);
        }
        //2016-12-20 add by karl 按时段查询报表

        public List<YieldVM> QueryDailyYield(ReportDataSearch searchModel)
        {
            var result = productInputRepository.QueryDailyYield(searchModel);

            return result;
        }

        public PagedListModel<WeekReportVM> QueryWeekReport(ReportDataSearch searchModel)
        {
            //var totalCount = 0;
            var PPlist = productInputRepository.QueryWeekReport(searchModel);
            List<WeekReportVM> result = new List<WeekReportVM>();
            foreach (WeekReport data in PPlist)
            {
                result.Add(new WeekReportVM
                {
                    Process = data.Process,
                    SumPlan = data.SumPlan,
                    SumGoodQty = data.SumGoodQty,
                    SumYieldRate = data.SumYieldRate.ToString("P"),

                    MondayPlan = data.MondayPlan,
                    MondayGoodQty = data.MondayGoodQty,
                    MondayYieldRate = data.MondayYieldRate.ToString("P"),

                    TuesdayPlan = data.TuesdayPlan,
                    TuesdayGoodQty = data.TuesdayGoodQty,
                    TuesdayYieldRate = data.TuesdayYieldRate.ToString("P"),

                    WednesdayPlan = data.WednesdayPlan,
                    WednesdayGoodQty = data.WednesdayGoodQty,
                    WednesdayYieldRate = data.WednesdayYieldRate.ToString("P"),

                    ThursdayPlan = data.ThursdayPlan,
                    ThursdayGoodQty = data.ThursdayGoodQty,
                    ThursdayYieldRate = data.ThursdayYieldRate.ToString("P"),

                    FridayPlan = data.FridayPlan,
                    FridayGoodQty = data.FridayGoodQty,
                    FridayYieldRate = data.FridayYieldRate.ToString("P"),

                    SaterdayPlan = data.SaterdayPlan,
                    SaterdayGoodQty = data.SaterdayGoodQty,
                    SaterdayYieldRate = data.SaterdayYieldRate.ToString("P"),

                    SundayPlan = data.SundayPlan,
                    SundayGoodQty = data.SundayGoodQty,
                    SundayYieldRate = data.SundayYieldRate.ToString("P")

                });
            }
            return new PagedListModel<WeekReportVM>(0, result);
        }

        /// <summary>
        /// 获取所有Rework制程信息----------Sidney
        /// </summary>
        /// <param name="Detail_UID"></param>
        /// <param name="Product_UID"></param>
        /// <returns></returns>
        public List<string> GetRepairToReworkProcessAPI(int Detail_UID, int Product_UID, string selectDate, string selectTime)
        {
            var result = productReworkInfoRepository.GetRepairOper(Detail_UID, Product_UID, selectDate, selectTime);
            return result;
        }


        public ErrorInfoVM GetErrorInfo(int productUid, string ErrorType)
        {
            var result = productInputRepository.GetErrorInfo(productUid, ErrorType);
            return result;
        }

        public bool CheckHasExistProcess(int masterUID, int version)
        {
            var hasProcess = false;
            string nowInterval = "";
            var nowDate = new DateTime();
            //获取当前时间及时段
            var nowDateAndTime = enumerationRepository.GetIntervalInfo("OP1").FirstOrDefault();
            if (nowDateAndTime != null)
            {
                nowDate = Convert.ToDateTime(nowDateAndTime.NowDate);
                nowInterval = nowDateAndTime.Time_Interval;
            }
            var detailUIDListTemp = productInputRepository.GetMany(m => m.Product_Date == nowDate && m.Time_Interval == nowInterval
         && m.FlowChart_Master_UID == masterUID && m.FlowChart_Version == version && m.Is_Comfirm == true).Select(m => m.FlowChart_Detail_UID).ToList();
            //如果已经保存了不用提示
            if (detailUIDListTemp.Count() > 0)
            {
                return false;
            }
            //获取当前时段的productinput信息
            var detailUIDList = productInputRepository.GetMany(m => m.Product_Date == nowDate && m.Time_Interval == nowInterval
            && m.FlowChart_Master_UID == masterUID && m.FlowChart_Version == version).Select(m => m.FlowChart_Detail_UID).ToList();
            if (detailUIDList.Count() > 0)
            {

                //获取flowchart制程信息
                var flDetailUIDList = flowChartDetailRepository.GetMany(m => m.FlowChart_Master_UID == masterUID && m.FlowChart_Version == version).Select(m => m.FlowChart_Detail_UID).ToList();
                var uidList = flDetailUIDList.Except(detailUIDList).ToList();
                if (uidList.Count() > 0)
                {
                    hasProcess = true;
                }
            }
            return hasProcess;
        }

        public string FillZeroProductData(ZeroProcessDataSearch funPlantInfo)
        {
            //获取该专案所有需填写的数据
            var searchModel = AutoMapper.Mapper.Map<ProcessDataSearch>(funPlantInfo);
            //Add by Rock 2016-09-22--------------start
            string nowInterval = "";
            var nowDate = new DateTime();
            //获取当前时间及时段
            var nowDateAndTime = enumerationRepository.GetIntervalInfo("OP1").FirstOrDefault();
            if (nowDateAndTime != null)
            {
                nowDate = Convert.ToDateTime(nowDateAndTime.NowDate);
                nowInterval = nowDateAndTime.Time_Interval;
            }
            //在第二天的凌晨到7：30都是前一天的日期
            searchModel.Date = nowDate;
            //Add by Rock 2016-09-22--------------end

            Page page = new Page();
            page.PageSize = 10;
            var noDataResult = QueryProcessData(searchModel, page);
            List<ProductDataItem> ZeroDataList = new List<ProductDataItem>();
            ProductDataList ZeroData = new ProductDataList();
            var AllNeedData = AutoMapper.Mapper.Map<List<ProductDataDTO>>(noDataResult.Items);
            //获取已经存在的数据
            var exitData = QueryExitProductData(searchModel);
            //得到还未填写的数据
            var surplusData = AllNeedData.FindAll(x => !exitData.Contains(x.FlowChart_Detail_UID));
            //获取当前版本的所有制程信息

            foreach (var proList in surplusData)
            {
                ProductDataItem ZeroDataItem = new ProductDataItem();
                ZeroDataItem.FlowChart_Detail_UID = proList.FlowChart_Detail_UID;
                ZeroDataItem.Adjust_QTY = proList.Adjust_QTY;
                ZeroDataItem.Color = proList.Color;
                ZeroDataItem.Create_Date = funPlantInfo.Create_Time;
                ZeroDataItem.Creator_UID = funPlantInfo.Create_User;
                ZeroDataItem.Customer = proList.Customer;
                ZeroDataItem.FlowChart_Master_UID = proList.FlowChart_Master_UID;
                ZeroDataItem.FlowChart_Version = proList.FlowChart_Version;
                ZeroDataItem.FunPlant = proList.FunPlant;
                ZeroDataItem.FunPlant_Manager = proList.FunPlant_Manager;
                ZeroDataItem.Good_MismatchFlag = proList.Good_MismatchFlag;
                ZeroDataItem.Good_QTY = proList.Good_QTY;
                ZeroDataItem.Is_Comfirm = false;
                ZeroDataItem.Material_No = proList.Material_No;
                ZeroDataItem.Modified_Date = funPlantInfo.Create_Time;
                ZeroDataItem.Modified_UID = funPlantInfo.Create_User;
                ZeroDataItem.NG_QTY = proList.NG_QTY;
                ZeroDataItem.Part_Types = proList.Part_Types;
                ZeroDataItem.Picking_MismatchFlag = proList.Picking_MismatchFlag;
                ZeroDataItem.Picking_QTY = proList.Picking_QTY;
                ZeroDataItem.Place = proList.Place;
                ZeroDataItem.Process = proList.Process;
                ZeroDataItem.Process_Seq = proList.Process_Seq;
                ZeroDataItem.Product_Date = nowDate;  //proList.Product_Date;
                ZeroDataItem.Time_Interval = nowInterval; //proList.Time_Interval;
                ZeroDataItem.Product_Phase = proList.Product_Phase;
                ZeroDataItem.Product_Stage = proList.Product_Stage;
                ZeroDataItem.Project = proList.Project;
                ZeroDataItem.Prouct_Plan = proList.Prouct_Plan;
                ZeroDataItem.Target_Yield = proList.Target_Yield;
                ZeroDataItem.WH_Picking_QTY = proList.WH_Picking_QTY;
                ZeroDataItem.WH_QTY = proList.WH_QTY;
                ZeroDataItem.WIP_QTY = proList.WIP_QTY.Value;
                ZeroDataItem.DRI = proList.DRI;
                ZeroDataList.Add(ZeroDataItem);

                switch (proList.Rework_Flag)
                {
                    case StructConstants.ReworkFlag.Repair:
                        ZeroDataItem.IsRepair = StructConstants.ReworkFlag.Repair;
                        //查找相对应颜色的Rework制程
                        var detailList = AllNeedData.Where(m => m.Rework_Flag == StructConstants.ReworkFlag.Rework && m.Color == ZeroDataItem.Color).ToList();
                        List<ReworkItem> reworkList = new List<ReworkItem>();
                        foreach (var detailItem in detailList)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                ReworkItem reworkItem = new ReworkItem();
                                reworkItem.detailuid = ZeroDataItem.FlowChart_Detail_UID;
                                //对应的Rework的DetailUID
                                reworkItem.reworkDetailUid = detailItem.FlowChart_Detail_UID;

                                if (i == 1)
                                {
                                    reworkItem.reworkOper = string.Format("{0}_{1}", ZeroDataItem.Process, "入");
                                }
                                else
                                {
                                    reworkItem.reworkOper = string.Format("{0}_{1}", ZeroDataItem.Process, "出");
                                }
                                reworkItem.reworkQty = 0;
                                reworkList.Add(reworkItem);
                            }

                        }
                        ZeroDataItem.ReworkList = reworkList;
                        break;
                    case StructConstants.ReworkFlag.Rework:
                        ZeroDataItem.IsRepair = StructConstants.ReworkFlag.Rework;
                        ZeroDataItem.INum = 0;
                        ZeroDataItem.ONum = 0;

                        //newInfoItem.FlowChart_Detail_UID = newItem.FlowChart_Detail_UID;
                        //newInfoItem.Opposite_Detail_UID = reworkItem.reworkDetailUid;
                        //newInfoItem.Opposite_QTY = reworkItem.reworkQty;
                        //newInfoItem.Product_Date = newItem.Product_Date;
                        //newInfoItem.Time_Interval = Time_Interval;
                        //newInfoItem.Modified_UID = newItem.Modified_UID;
                        //newInfoItem.Modified_Date = newItem.Modified_Date;
                        //newInfoItem.Rework_Flag = "Repair";
                        //newInfoItem.Is_Match = true;
                        break;

                }
            }
            ZeroData.ProductLists = ZeroDataList;
            return AddProductDatas(ZeroData);
        }
    }
}

