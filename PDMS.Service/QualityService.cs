using PDMS.Common.Constants;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
namespace PDMS.Service
{
    public interface IQualityService
    {
        PagedListModel<ExceptionTypeVM> QueryBadTypes(BadTypeSearch search, Page page);
        ExceptionTypeVM QueryBadTypeByUID(int uuid);
        string ModifyBadType(ExceptionTypeVM ent);
        string ImportExceptionTypes(ExceptionTypeTempList badTypeList);
        string ImportExcepProcess(ExceptionTypeProcessList badTypeList);
        string DeleteBadType(int uiid);
        string AddExceptionType(ExceptionTypeVM ent);
        string CheckBadTypeByName(string typeName);
        string CheckBadTypeByCode(string code);

        string DeleteExecProcess(int uiid);
        string AddExecProcess(ExceptionTypeFlowChartVM ent);

        string DeleteAllExceptionProcess(int FlowChart_Detail_UID);

        PagedListModel<ExceptionTypeListVM> QueryExcepProcS(ExcepTypeFlowChartSearch searchModel, Page page);
        #region ----IPQC Add by Destiny

        #region --- QAMaster

        PagedListModel<CheckPointVM> GetCheckPointsList(int UserUId, int FlowChart_Master_UID, string Color, string MaterielType);
        List<CheckPointVM> GetProcessSource(string funplant, int FlowChart_Master_UID, DateTime Product_Date, string Color);
        CheckPointInputCondition QueryInputConditions(int FlowChart_Master_UID);
        PagedListModel<QAMasterVM> QueryQAMasterData(CheckPointInputConditionModel condition, DateTime productDate, string timeInteral);

        PagedListModel<QAMasterVM> QueryQAHistroyDatas(QAReportSearchVM searchModel, DateTime timeNow);
        ReturnMessageQA SaveQaMasterData(QAMasterVM data);
        void updateMesSynsData(QAMasterVM data);
        void updateMesSynsDataByUID(QAMasterVM data);
        string ModifyQAMasterData(QAMasterVM data);
        QualityAssurance_InputMasterDTO GetInputMasterByID(int uid);
        IPQCDailyDataVM IPQCInputDataForExportExcel(QAReportSearchVM data);


        #endregion


        #region --- QADetail

        List<ExceptionTypeVM> QueryExceptionTypeForAddAPI(int typeLevel, int Flowchart_Master_UID, string parentCode = null);
        List<ExceptionTypeVM> QueryExceptionTypeForSearchAPI(int typeLevel, string parentCode, int QAMasterUID, DateTime ProductDate, int Flowchart_Master_UID);

        string ModifyQAInputDetail(QAInputDetailVM data);
        PagedListModel<QAInputDetailVM> QueryQAInputDetailVM(QADetailSearch searchData, List<MesNGResult> mesResult);

        string InsertQAInputDetail(QAInputDetailListVM data);
        string UpdateQAInputDetail(QAInputDetailListVM data);

        List<ExceptionTypeVM> QueryQAExceptionTypeAPI(QADetailSearch data);
        QAInputDetailVM QuerySingleQAInputDetailInfoAPI(int QAInputDetailUID);

        #endregion

        #region --- QAReport
        QAReportSearchProjectAndMetrialTypeVM GetQAProject(int OPType_Organization_UID);
        List<PartTypeVM> GetPartType(int Project_UID);
        List<OrgVM> QueryOptype(int Account_uid, int Plant_OrganizationUID);
        List<OrgVM> QueryOrganzitionInfo(int Account_uid, int Plant_OrganizationUID = 0, int OPType_OrganizationUID = 0, int Department_OrganizationUID = 0);

        List<QAReportVM> QueryQAIPQCReport(QAReportSearchVM data);

        PagedListModel<QAReportDaySummeryVM> GetQAReportDaySummery(QAReportSearchVM search);
        string GetAllFirstTargetYield(QAReportSearchVM search);
        string GetAllSecondTargetYield(QAReportSearchVM search);
        PagedListModel<QAReportExceptionTypeRank> GetQAReportTypeRank(QAReportSearchVM search, int yield);

        PagedListModel<QAReportDaySummeryVM> QueryFuncReportSummaryAPI(QAReportSearchVM search);
        PagedListModel<QAReportDaySummeryVM> QueryIPQCALLProcessReportSummaryAPI(QAReportSearchVM search);

        #endregion

        PagedListModel<QAInputModifyVM> GetQAInputModify(QAInputModifySearch searchModel, Page page);

        #region --- QATargetYield

        List<QATargetRateVM> QueryCheckPointForTargetYield(int FlowChart_Master_UID);

        string UpdateTargetYieldAPI(QAMgDataListVM data);

        string ModifySingleQaTargetYield(QATargetYieldVM data);

        PagedListModel<QATargetYieldVM> QueryQATargetYield(int Flowchart_Master_UID, DateTime date);


        QATargetYieldVM QueryQAProcessMGDataSingle(int Flowchart_Master_UID, int Flowchart_Detail_UID, DateTime date);



        #endregion

        #endregion

        #region --- QA反推良率分配界面
        PagedListModel<QABackToFunPlant> QueryQABackToFunPlantInfo(QAReportSearchVM search);

        QABackToFunPlant QueryQABTFInfoByUID(string QualityAssurance_DistributeRate_UID);

        string SaveBackToFunPlantInfo(QABackToFunPlantListVM data);
        string UpdateBackToFunPlantInfo(QABackToFunPlant data);
        #endregion


        #region --- OQC Data Inpnut Add by Destiny

        OQC_InputMasterVM QueryOQCMasterData(QAReportSearchVM searhModel);


        PagedListModel<OQC_InputDetailVM> QueryOQCNGDetails(QAReportSearchVM searhModel);
        PagedListModel<OQC_InputDetailVM> QueryOQCReworkDetails(QAReportSearchVM searhModel);
        PagedListModel<OQC_InputDetailVM> QueryOQCNGRecord(QAReportSearchVM searhModel);
        PagedListModel<OQC_InputDetailVM> QueryOQCReworkRecord(QAReportSearchVM searhModel);

        string SaveOQCData(OQCInputData data);

        string AddTExceptionTypesoFlowChart(ExceptionTypesAddToFlowChartVM data);


        OQC_InputMasterVM QueryOQCRecordData(QAReportSearchVM searhModel);
        OQCRecordCondition QueryOQCReportConditions(string processName, string Project);


        List<string> QueryRecordColor(int Flowchart_Master_UID, string FunPlant, string ProductDate, string MaterialType);
        List<FunPlantVM> QueryFunPlant(int Flowchart_Master_UID);

        OQCReportExcel QueryQAOQCReport(QAReportSearchVM data);

        PagedListModel<OQCReprotVM> GetQAReportOQCDaySummery(QAReportSearchVM search);
        PagedListModel<OQCReprotTopFiveTypeVM> GetQAReportOQCTypeRank(QAReportSearchVM search);



        ExportOQCDataForExcel GetOQCInputDataForExportExcel(QAReportSearchVM data);

        #endregion

        #region -----Product&Sale Report

        PagedListModel<ProductSaleReport_RateVM> QueryProductSaleReportSummery(QAReportSearchVM searchModel);
        PagedListModel<ProductSaleReport_RateVM> QueryProductSaleReportFunplantDetail(QAReportSearchVM searchModel);

        PagedListModel<ProductSaleReport_RateVM> QueryProductSaleReportExceptionTypeDetail(string TypeFatherName, string FunPlant, string ProductDate, int FlowChart_Detail_UID, int RateType, string Color, string MeterialType);
        PagedListModel<TimeIntervalFPYReportVM> QueryTimeIntervalFPYReport(QAReportSearchVM searchModel);

        //-----反推报表
        PagedListModel<ProductSaleReport_RateVM> QueryDistributeRateReportDetail(QAReportSearchVM searchModel);
        List<ProductSaleReport_RateVM> QueryDistributeRateReportExceptionTypeDetail(string TypeFatherName, string FunPlant,
            string ProductDate, int FlowChart_Master_UID, int RateType, string Color, string MeterialType, int OPType_OrganizationUID);

        #endregion

    }

    public class QualityService : IQualityService
    {
        #region Private interfaces properties

        private readonly IUnitOfWork unitOfWork;
        private readonly IQualityAssurance_ExceptionTypeRepository qualityAssurance_ExceptionTypeRepository;
        private readonly IFlowChartDetailRepository flowChartDetailRepository;
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IPPForQAInterfaceRepository pPForQAInterfaceRepository;
        private readonly IQualityAssuranceInputMasterRepository qualityAssuranceInputMasterRepository;
        private readonly IQualityAssuranceInputDetailRepository qualityAssuranceInputDetailRepository;
        private readonly IQualityAssuranceMgDataRepository qualityAssuranceMgDataRepository;
        private readonly IQualityAssurance_ExceptionType_TempRepository qualityAssurance_ExceptionType_TempRepository;


        private readonly IExceptionTypeWithFlowchartRepository exceptionTypeWithFlowchartRepository;

        private readonly IQualityAssurance_OQC_InputMasterRepository qualityAssurance_OQC_InputMasterRepository;
        private readonly IQualityAssurance_OQC_InputDetailRepository qualityAssurance_OQC_InputDetailRepository;

        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly ISystemProjectRepository systemProjectRepository;
        private readonly ISystemFunctionPlantRepository systemFunctionPlantRepository;

        #endregion //Private interfaces properties

        #region Service constructor

        public QualityService(
        IQualityAssurance_ExceptionTypeRepository qualityAssurance_ExceptionTypeRepository,
            IFlowChartDetailRepository FlowChartDetailRepository,
            IEnumerationRepository enumerationRepository,
            IPPForQAInterfaceRepository pPForQAInterfaceRepository,
            IQualityAssuranceInputMasterRepository qualityAssuranceInputMasterRepository,
            IQualityAssuranceInputDetailRepository qualityAssuranceInputDetailRepository,
            IQualityAssuranceMgDataRepository qualityAssuranceMgDataRepository,
            IQualityAssurance_ExceptionType_TempRepository qualityAssurance_ExceptionType_TempRepository,
            IQualityAssurance_OQC_InputMasterRepository qualityAssurance_OQC_InputMasterRepository,
            IQualityAssurance_OQC_InputDetailRepository qualityAssurance_OQC_InputDetailRepository,
            IExceptionTypeWithFlowchartRepository exceptionTypeWithFlowchartRepository,
            ISystemOrgRepository systemOrgRepository,
            ISystemProjectRepository systemProjectRepository,
            ISystemFunctionPlantRepository systemFunctionPlantRepository,
            IUnitOfWork unitOfWork)

        {
            this.qualityAssurance_ExceptionTypeRepository = qualityAssurance_ExceptionTypeRepository;
            this.flowChartDetailRepository = FlowChartDetailRepository;
            this.unitOfWork = unitOfWork;

            this.enumerationRepository = enumerationRepository;
            this.pPForQAInterfaceRepository = pPForQAInterfaceRepository;
            this.qualityAssuranceInputMasterRepository = qualityAssuranceInputMasterRepository;
            this.qualityAssuranceInputDetailRepository = qualityAssuranceInputDetailRepository;
            this.qualityAssuranceMgDataRepository = qualityAssuranceMgDataRepository;
            this.qualityAssurance_ExceptionType_TempRepository = qualityAssurance_ExceptionType_TempRepository;

            this.qualityAssurance_OQC_InputMasterRepository = qualityAssurance_OQC_InputMasterRepository;
            this.qualityAssurance_OQC_InputDetailRepository = qualityAssurance_OQC_InputDetailRepository;
            this.exceptionTypeWithFlowchartRepository = exceptionTypeWithFlowchartRepository;
            this.systemOrgRepository = systemOrgRepository;
            this.systemProjectRepository = systemProjectRepository;
            this.systemFunctionPlantRepository = systemFunctionPlantRepository;

        }

        #endregion //Service constructor

        #region --- ExceptionType

        public PagedListModel<ExceptionTypeVM> QueryBadTypes(BadTypeSearch searchModel, Page page)
        {
            var totalCount = 0;
            var BadTypes = qualityAssurance_ExceptionTypeRepository.QueryBadTypes(searchModel, page, out totalCount);
            return new PagedListModel<ExceptionTypeVM>(totalCount, BadTypes);
        }

        public string ImportExceptionTypes(ExceptionTypeTempList pDataListTem)
        {
            List<ExceptionTypeTempVM> pDataList = pDataListTem.ImportList;
            //先获flowchartmaster对应的专案名，然后遍历不良类型中的专案，检查是否匹配
            var masterUID = pDataList[0].Flowchart_Master_UID;
            var projectName = qualityAssurance_ExceptionType_TempRepository.getProjectNamebyMasterUid(masterUID);
            string result = "";
            foreach (ExceptionTypeTempVM pData in pDataList)
            {
                if (pData.Project != projectName)
                    return "专案名和系统中的专案名不同，不能导入，请检查";
                QualityAssurance_ExceptionType_Temp item = new QualityAssurance_ExceptionType_Temp();
                item = AutoMapper.Mapper.Map<QualityAssurance_ExceptionType_Temp>(pData);
                try
                {
                    //将数据插入临时表
                    qualityAssurance_ExceptionType_TempRepository.Add(item);

                }
                catch (Exception ex)
                {
                    result = "Error";
                    return ex.ToString();
                }
            }
            unitOfWork.Commit();
            if (string.IsNullOrEmpty(result))
            {
                //调用SP，将临时表数据插入到类型表
                result = qualityAssurance_ExceptionType_TempRepository.ExecSPImportData();
            }

            return result;
        }

        public string AddExceptionType(ExceptionTypeVM ent)
        {
            return qualityAssurance_ExceptionTypeRepository.AddExceptionType(ent);
        }



        public ExceptionTypeVM QueryBadTypeByUID(int uuid)
        {
            var item = qualityAssurance_ExceptionTypeRepository.GetById(uuid);
            return AutoMapper.Mapper.Map<ExceptionTypeVM>(item);

        }

        public string ModifyBadType(ExceptionTypeVM ent)
        {
            var item = qualityAssurance_ExceptionTypeRepository.GetById(ent.ExceptionType_UID);
            item.ShortName = ent.ShortName;
            try
            {
                qualityAssurance_ExceptionTypeRepository.Update(item);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "OK";
        }

        public string DeleteBadType(int uiid)
        {
            return qualityAssurance_ExceptionTypeRepository.DeleteExceptionTypeByUID(uiid);
        }

        public string CheckBadTypeByName(string typeName)
        {
            return qualityAssurance_ExceptionTypeRepository.CheckBadTypeByName(typeName);
        }

        public string CheckBadTypeByCode(string code)
        {
            return qualityAssurance_ExceptionTypeRepository.CheckBadTypeByCode(code);
        }

        #endregion

        #region --- QAMaster


        public PagedListModel<CheckPointVM> GetCheckPointsList(int UserUId, int FlowChart_Master_UID, string Color, string MaterielType)
        {

            List<CheckPointVM> result = flowChartDetailRepository.GetCheckPointsList(UserUId, FlowChart_Master_UID, Color, MaterielType);
            return new PagedListModel<CheckPointVM>(result.Count, result);
        }

        public CheckPointInputCondition QueryInputConditions(int FlowChart_Master_UID)
        {
            CheckPointInputCondition reuslt = new CheckPointInputCondition();
            List<string> QA_MaterialType = enumerationRepository.GetEnumValuebyType("MaterielType");

            reuslt = flowChartDetailRepository.QueryInputConditions(FlowChart_Master_UID);
            reuslt.MaterialType = QA_MaterialType;

            return reuslt;
        }

        public PagedListModel<QAMasterVM> QueryQAMasterData(CheckPointInputConditionModel condition, DateTime productDate, string timeInteral)
        {
            //录入当前数据的时候 ProductDate 和 TimeInterval为空
            if (string.IsNullOrEmpty(condition.Time_interval))
            {
                condition.ProductDate = productDate;
                condition.Time_interval = timeInteral;
            }
            //获取中间表数据
            QualityAssurance_InputMasterDTO result = pPForQAInterfaceRepository.QueryDataFromPP(condition);


            //如果没有获取到数据，则到QAMaster表去获取数据，有可能是已经录入过，任然在当前时段，修改数据
            if (result.NG_Qty == null && result.Input == null)
            {
                result = qualityAssuranceInputMasterRepository.QueryAssuranceInputMaster(condition);
                result.CanModify = true;
            }
            else
            {
                result.CanModify = false;
            }


            List<QAMasterVM> te = new List<QAMasterVM>();
            if (result != null)
            {
                QAMasterVM t = new QAMasterVM();

                t.Color = result.Color;
                t.FirstCheck_Qty = result.FirstCheck_Qty;
                t.FirstOK_Qty = result.FirstOK_Qty;
                t.FirstRejectionRate = Convert.ToDouble(result.FirstRejectionRate).ToString("P");
                t.Input = result.Input;
                t.MaterialType = result.MaterielType;
                t.Modified_Date = result.Modified_Date;
                t.Modified_UID = result.Modified_UID;
                t.NG_Qty = result.NG_Qty;
                t.Process = result.Process;
                t.QualityAssurance_InputMaster_UID = result.QualityAssurance_InputMaster_UID;
                t.RepairCheck_Qty = result.RepairCheck_Qty;
                t.RepairOK_Qty = result.RepairOK_Qty;
                t.Shipment_Qty = result.Shipment_Qty;
                t.SizeSA_Qty = result.SizeSA_Qty;
                t.SurfaceSA_Qty = result.SurfaceSA_Qty;
                t.Time_Interval = result.Time_Interval;
                t.WIPForCheck_Qty = result.WIPForCheck_Qty;
                t.CanModify = result.CanModify;
                t.NGFlag = result.NGFlag;
                t.FirstCheckFlag = result.FirstCheckFlag;
                t.Displace_Qty = result.Displace_Qty;
                t.DisplaceFlag = result.DisplaceFlag;


                te.Add(t);
            }
            return new PagedListModel<QAMasterVM>(te.Count, te);
        }

        public ReturnMessageQA SaveQaMasterData(QAMasterVM data)
        {
            return qualityAssuranceInputMasterRepository.SaveQaMasterData(data);
        }

        public void updateMesSynsData(QAMasterVM data)
        {
            qualityAssuranceInputMasterRepository.updateMesSynsData(data);
        }

        public void updateMesSynsDataByUID(QAMasterVM data)
        {
            qualityAssuranceInputMasterRepository.updateMesSynsDataByUID(data);
        }


        public string ModifyQAMasterData(QAMasterVM data)
        {
            string result = qualityAssuranceInputMasterRepository.ModifyQAMasterData(data);

            if (result.Equals("SUCCESS"))
            {
                result = qualityAssuranceInputDetailRepository.CalculateQAReportSumData(data.QualityAssurance_InputMaster_UID);
            }
            return result;
        }

        public QualityAssurance_InputMasterDTO GetInputMasterByID(int uid)
        {
            return qualityAssuranceInputMasterRepository.GetInputMasterByID(uid);
        }
        public List<CheckPointVM> GetProcessSource(string funplant, int FlowChart_Master_UID, DateTime Product_Date, string Color)
        {
            List<CheckPointVM> result = flowChartDetailRepository.GetCheckPointsForSearchHistory(funplant, FlowChart_Master_UID, Color, Product_Date);
            return result;
        }


        public PagedListModel<QAMasterVM> QueryQAHistroyDatas(QAReportSearchVM searchMode, DateTime timeNow)
        {
            List<QualityAssurance_InputMasterDTO> tempData = qualityAssuranceInputMasterRepository.QueryQAHistroyDatas(searchMode, timeNow);
            List<QAMasterVM> result = new List<QAMasterVM>();

            foreach (var temp in tempData)
            {
                QAMasterVM t = new QAMasterVM();

                t.Color = temp.Color;
                t.FirstCheck_Qty = temp.FirstCheck_Qty;
                t.FirstOK_Qty = temp.FirstOK_Qty;
                t.FirstRejectionRate = Convert.ToDouble(temp.FirstRejectionRate).ToString("P");
                t.Input = temp.Input;
                t.MaterialType = temp.MaterielType;
                t.Modified_Date = temp.Modified_Date;
                t.Modified_UID = temp.Modified_UID;
                t.NG_Qty = temp.NG_Qty;
                t.Process = temp.Process;
                t.QualityAssurance_InputMaster_UID = temp.QualityAssurance_InputMaster_UID;
                t.RepairCheck_Qty = temp.RepairCheck_Qty;
                t.RepairOK_Qty = temp.RepairOK_Qty;
                t.Shipment_Qty = temp.Shipment_Qty;
                t.SizeSA_Qty = temp.SizeSA_Qty;
                t.SurfaceSA_Qty = temp.SurfaceSA_Qty;
                t.Time_Interval = temp.Time_Interval;
                t.WIPForCheck_Qty = temp.WIPForCheck_Qty;
                if ((DateTime.Now.Date - searchMode.ProductDate).Days <= 1)
                {
                    t.CanModify = true;
                }
                else
                {
                    t.CanModify = false;
                }
                t.FlowChart_Detail_UID = temp.FlowChart_Detail_UID;
                t.NGFlag = temp.NGFlag;
                t.FirstCheckFlag = temp.FirstCheckFlag;
                t.Displace_Qty = temp.Displace_Qty;
                t.DisplaceFlag = temp.DisplaceFlag;
                result.Add(t);
            }

            return new PagedListModel<QAMasterVM>(result.Count, result);
        }


        public IPQCDailyDataVM IPQCInputDataForExportExcel(QAReportSearchVM data)
        {
            IPQCDailyDataVM result = new IPQCDailyDataVM();

            List<FlowchartColor> Process = qualityAssuranceInputMasterRepository.GetProcessByProject(data.FlowChart_Master_UID, "IPQC");
            QADetailSearch searchData = new QADetailSearch();
            searchData.Color = data.Color;

            searchData.MaterialType = data.MaterialType;
            searchData.FlowChart_Master_UID = data.FlowChart_Master_UID;

            searchData.ProductDate = data.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate);
            searchData.Project = data.ProjectName;
            searchData.Time_interval = data.Time_interval;

            List<IPQCInputDataVM> DataList = new List<IPQCInputDataVM>();

            foreach (FlowchartColor temp in Process)
            {
                if (!string.IsNullOrEmpty(temp.Color) && temp.Color != data.Color)
                {
                    continue;
                }
                searchData.Process_seq = temp.Process_Seq;

                searchData.FlowChart_Detail_UID = temp.Flowchart_Detail_UID;
                searchData.FlowChart_Master_UID = data.FlowChart_Master_UID;
                data.FlowChart_Detail_UID = temp.Flowchart_Detail_UID;
                searchData.Time_interval = data.Tab_Select_Text;

                IPQCInputDataVM tempResult = new IPQCInputDataVM();
                List<QualityAssurance_InputMasterDTO> tempData = qualityAssuranceInputMasterRepository.QueryQAHistroyDatas(data, DateTime.Now);

                if (tempData.Count > 0)
                {
                    tempResult.MasterData = tempData[0];
                    List<QAInputDetailVM> detailInfos = qualityAssuranceInputDetailRepository.QueryQAInputDetail(searchData);
                    tempResult.DetailList = detailInfos;
                    DataList.Add(tempResult);
                }
            }
            result.DataList = DataList;

            return result;
        }

        #endregion

        #region ---- QADetail

        public List<ExceptionTypeVM> QueryExceptionTypeForAddAPI(int typeLevel, int Flowchart_Master_UID, string parentCode = null)
        {

            var tempResult = qualityAssurance_ExceptionTypeRepository.QueryExceptionTypeForAddAPI(typeLevel, Flowchart_Master_UID, parentCode);
            List<ExceptionTypeVM> busDTO = new List<ExceptionTypeVM>();
            foreach (var bu in tempResult)
            {
                var dto = AutoMapper.Mapper.Map<ExceptionTypeVM>(bu);
                busDTO.Add(dto);
            }

            return busDTO;
        }

        public List<ExceptionTypeVM> QueryExceptionTypeForSearchAPI(int typeLevel, string parentCode, int QAMasterUID, DateTime ProductDate, int Flowchart_Master_UID)
        {
            var tempResult = qualityAssurance_ExceptionTypeRepository.QueryExceptionTypeForSearchAPI(typeLevel, QAMasterUID, ProductDate, Flowchart_Master_UID, parentCode);
            List<ExceptionTypeVM> busDTO = new List<ExceptionTypeVM>();
            foreach (var bu in tempResult)
            {
                var dto = AutoMapper.Mapper.Map<ExceptionTypeVM>(bu);
                busDTO.Add(dto);
            }

            return busDTO;
        }

        public string ModifyQAInputDetail(QAInputDetailVM data)
        {
            string result = "";

            result = qualityAssuranceInputDetailRepository.ModifyQAInputDetail(data);

            if (result == "Success")
            {
                result = qualityAssuranceInputDetailRepository.CalculateQAReportSumData(int.Parse(data.QualityAssurance_InputMaster_UID.ToString()));
            }

            return result;
        }

        public PagedListModel<QAInputDetailVM> QueryQAInputDetailVM(QADetailSearch searchData, List<MesNGResult> mesResult)
        {
            List<QAInputDetailVM> result = qualityAssuranceInputDetailRepository.QueryQAInputDetail(searchData);
            List<FunPlantVM> FunPlants = systemFunctionPlantRepository.GetFunPlantByQAMasterUID(searchData.QAMaster_UID);
            foreach (QAInputDetailVM detail in result)
            {
                detail.FunPlants = FunPlants;
                var matchResult = mesResult.Where(p => p.DefectName == detail.BadTypeCode);
                if (matchResult != null && matchResult.Count() > 0)
                {
                    detail.NG_Qty = matchResult.FirstOrDefault().NG_Point;
                }
            }

            return new PagedListModel<QAInputDetailVM>(result.Count, result);
        }

        public string InsertQAInputDetail(QAInputDetailListVM data)
        {
            string result = "Success";
            try
            {
                foreach (var tempData in data.DataList)
                {
                    var dto = AutoMapper.Mapper.Map<QualityAssurance_InputDetail>(tempData);

                    dto.Create_Date = DateTime.Now;
                    dto.Modified_Date = DateTime.Now;

                    qualityAssuranceInputDetailRepository.Add(dto);
                }
                unitOfWork.Commit();

                qualityAssuranceInputDetailRepository.DeleteRepeatData();
            }
            catch (Exception ex)
            {
                result = "Error";
            }

            return result;
        }

        public string UpdateQAInputDetail(QAInputDetailListVM data)
        {
            string result = "Success";
            try
            {
                int qaMasterUID = 0;
                if (data != null && data.DataList.Count != 0)
                {
                    qaMasterUID = (int)data.DataList[0].QualityAssurance_InputMaster_UID;
                }

                foreach (var tempData in data.DataList)
                {
                    var dto = AutoMapper.Mapper.Map<QualityAssurance_InputDetail>(tempData);

                    if (dto.QualityAssurance_InputDetail_UID == 0 && (dto.RepairNG_Qty != 0 || dto.Repair_Qty != 0 || dto.SepcialAccept_Qty != 0 || dto.NG_Qty != 0 || dto.Displace_Qty != 0))
                    {
                        dto.Create_Date = DateTime.Now;
                        dto.Modified_Date = DateTime.Now;

                        qualityAssuranceInputDetailRepository.Add(dto);
                    }
                    else if (dto.RepairNG_Qty != 0 || dto.Repair_Qty != 0 || dto.SepcialAccept_Qty != 0 || dto.NG_Qty != 0 || dto.Displace_Qty != 0)
                    {
                        dto.RepairNG_Qty = dto.RepairNG_Qty == null ? 0 : dto.RepairNG_Qty;
                        dto.Repair_Qty = dto.Repair_Qty == null ? 0 : dto.Repair_Qty;
                        dto.SepcialAccept_Qty = dto.SepcialAccept_Qty == null ? 0 : dto.SepcialAccept_Qty;
                        dto.NG_Qty = dto.NG_Qty == null ? 0 : dto.NG_Qty;
                        dto.Displace_Qty = dto.Displace_Qty == null ? 0 : dto.Displace_Qty;
                        dto.System_FunPlant_UID = tempData.System_FunPlant_UID;

                        dto.Modified_Date = DateTime.Now;
                        qualityAssuranceInputDetailRepository.Update(dto);
                    }
                }

                unitOfWork.Commit();
                qualityAssuranceInputDetailRepository.DeleteNullData();

                result = qualityAssuranceInputDetailRepository.CalculateQAReportSumData(qaMasterUID);
            }
            catch (Exception ex)
            {
                result = "Error";
            }

            return result;
        }


        public List<ExceptionTypeVM> QueryQAExceptionTypeAPI(QADetailSearch data)
        {
            var tempResult = qualityAssurance_ExceptionTypeRepository.QueryQAExceptionTypeAPI(data);

            List<ExceptionTypeVM> busDTO = new List<ExceptionTypeVM>();
            foreach (var bu in tempResult)
            {
                var dto = AutoMapper.Mapper.Map<ExceptionTypeVM>(bu);
                busDTO.Add(dto);
            }

            return busDTO;
        }

        public QAInputDetailVM QuerySingleQAInputDetailInfoAPI(int QAInputDetailUID)
        {
            return qualityAssuranceInputDetailRepository.QuerySingleQAInputDetailInfoAPI(QAInputDetailUID);
        }

        #endregion

        #region --- QAReport

        public List<OrgVM> QueryOrganzitionInfo(int Account_uid, int Plant_OrganizationUID = 0, int OPType_OrganizationUID = 0, int Department_OrganizationUID = 0)
        {
            List<OrgVM> result = new List<OrgVM>();
            List<OrganiztionVM> tempResult = systemOrgRepository.QueryOrganzitionInfoByAccountID(Account_uid);

            if (tempResult.Count != 0)
            {
                if (Department_OrganizationUID != 0)
                {
                    foreach (OrganiztionVM org in tempResult.Where(x => x.Department_OrganizationUID == Department_OrganizationUID))
                    {
                        OrgVM torg = new OrgVM();
                        torg.Organization_UID = int.Parse(org.Funplant_OrganizationUID.ToString());
                        torg.Name = org.Funplant;
                        result.Add(torg);
                    }
                }
                else if (OPType_OrganizationUID != 0)
                {
                    foreach (OrganiztionVM org in tempResult.Where(x => x.OPType_OrganizationUID == OPType_OrganizationUID))
                    {
                        OrgVM torg = new OrgVM();
                        torg.Organization_UID = int.Parse(org.Department_OrganizationUID.ToString());
                        torg.Name = org.Department;
                        result.Add(torg);
                    }
                }
                else if (Plant_OrganizationUID != 0)
                {
                    //如果存在OPType则直接获取
                    if (tempResult.Exists(x => x.Plant_OrganizationUID == Plant_OrganizationUID && x.OPType_OrganizationUID != null && x.OPType_OrganizationUID != 0))
                    {
                        foreach (OrganiztionVM org in tempResult.Where(x => x.Plant_OrganizationUID == Plant_OrganizationUID))
                        {
                            OrgVM torg = new OrgVM();
                            torg.Organization_UID = int.Parse(org.OPType_OrganizationUID.ToString());
                            torg.Name = org.OPType;
                            result.Add(torg);
                        }
                    }
                    else if (tempResult.Exists(x => x.Plant_OrganizationUID == Plant_OrganizationUID && (x.OPType_OrganizationUID == null || x.OPType_OrganizationUID == 0)))
                    {

                        // 获取该厂区的所有OP   2017/5/25
                        foreach (OrganiztionVM org1 in tempResult.Where(x => x.Plant_OrganizationUID == Plant_OrganizationUID))
                        {
                            var funResult = systemOrgRepository.GetOpTypeByPlant(int.Parse(org1.Plant_OrganizationUID.ToString()), 0);
                            foreach (SystemOrgDTO org in funResult)
                            {
                                OrgVM torg = new OrgVM();
                                torg.Organization_UID = org.Organization_UID;
                                torg.Name = org.Organization_Name;
                                result.Add(torg);
                            }
                        }
                    }
                }
                else
                {
                    //如果存在Plant则直接获取
                    if (tempResult.Exists(x => x.Plant_OrganizationUID != null))
                    {
                        foreach (OrganiztionVM org in tempResult)
                        {
                            OrgVM torg = new OrgVM();
                            torg.Organization_UID = int.Parse(org.Plant_OrganizationUID.ToString());
                            torg.Name = org.Plant;
                            result.Add(torg);
                        }
                    }
                    else
                    {
                        //返回所有厂区  2017/5/25
                        var funResult = systemOrgRepository.GetPlants(0);
                        foreach (SystemOrgDTO org in funResult)
                        {
                            OrgVM torg = new OrgVM();
                            torg.Organization_UID = org.Organization_UID;
                            torg.Name = org.Organization_Name;
                            result.Add(torg);
                        }
                    }
                }
            }
            if (result.Count == 0)
            {
                //  result = systemOrgRepository.QueryOrganzitionInfo(Plant_OrganizationUID, OPType_OrganizationUID, Department_OrganizationUID);
                //返回所有厂区  2017/5/25
                if (Plant_OrganizationUID == 0)
                {
                    var funResult = systemOrgRepository.GetPlants(0);
                    foreach (SystemOrgDTO org in funResult)
                    {
                        OrgVM torg = new OrgVM();
                        torg.Organization_UID = org.Organization_UID;
                        torg.Name = org.Organization_Name;
                        result.Add(torg);
                    }
                }
                else
                {
                    var funResult = systemOrgRepository.GetOpTypeByPlant(Plant_OrganizationUID, 0);
                    foreach (SystemOrgDTO org in funResult)
                    {
                        OrgVM torg = new OrgVM();
                        torg.Organization_UID = org.Organization_UID;
                        torg.Name = org.Organization_Name;
                        result.Add(torg);
                    }
                }
            }
            return result;
        }


        public QAReportSearchProjectAndMetrialTypeVM GetQAProject(int OPType_Organization_UID)
        {
            QAReportSearchProjectAndMetrialTypeVM result = new QAReportSearchProjectAndMetrialTypeVM();
            List<string> QA_MaterielType = enumerationRepository.GetEnumValuebyType("MaterielType");

            result = qualityAssuranceInputMasterRepository.QueryQAReportSearchProjectAndMetrialInfo(OPType_Organization_UID);
            result.MaterielType = QA_MaterielType;

            return result;
        }

        public List<PartTypeVM> GetPartType(int Project_UID)
        {
            return systemProjectRepository.GetPartType(Project_UID);
        }


        public List<QAReportVM> QueryQAIPQCReport(QAReportSearchVM data)
        {
            List<QAReportVM> result = new List<QAReportVM>();

            List<FlowchartColor> Process = qualityAssuranceInputMasterRepository.GetProcessByProject(data.FlowChart_Master_UID, "IPQC");

            foreach (FlowchartColor temp in Process)
            {
                if (!string.IsNullOrEmpty(temp.Color) && temp.Color != data.Color&&data.Color!="ALL")
                {
                    continue;
                }
                data.Process_seq = temp.Process_Seq;
                data.FlowChart_Detail_UID = temp.Flowchart_Detail_UID;
                data.Process = temp.Process;
                QAReportVM tempResult = qualityAssuranceInputMasterRepository.QueryQAReport(data);
                if (tempResult.FirstRejectionRateTopTen.Count == 0)
                    continue;
                result.Add(tempResult);
            }


            return result;
        }


       public string GetAllFirstTargetYield(QAReportSearchVM search)
        {
            return qualityAssuranceInputMasterRepository.GetAllFirstTargetYield(search);
        }
        public string GetAllSecondTargetYield(QAReportSearchVM search)
        {
            return qualityAssuranceInputMasterRepository.GetAllSecondTargetYield(search);
        }
        public PagedListModel<QAReportDaySummeryVM> GetQAReportDaySummery(QAReportSearchVM search)
        {
            var tempData = qualityAssuranceInputMasterRepository.GetQAReportDaySummery(search);
            QAReportDaySummeryVM result = new QAReportDaySummeryVM();

            result.FirstCheck_Qty = tempData.FirstCheck_Qty;
            result.FirstOK_Qty = tempData.FirstOK_Qty;
            result.FirstTargetYield = Convert.ToDouble(tempData.FirstTargetYield).ToString("P");
            result.FirstRejectionRate = Convert.ToDouble(tempData.FirstRejectionRate).ToString("P");
            result.SecondRejectionRate = Convert.ToDouble(tempData.SecondRejectionRate).ToString("P");
            result.SecondTargetYield = Convert.ToDouble(tempData.SecondTargetYield).ToString("P");
            result.Input = tempData.Input;
            result.Process = tempData.Process;
            result.SepcialAccept_Qty = tempData.SepcialAccept_Qty;
            result.NG = tempData.NG;
            result.Shipment_Qty = tempData.Shipment_Qty;

            return new PagedListModel<QAReportDaySummeryVM>(1, new List<QAReportDaySummeryVM> { result });
        }


        public PagedListModel<QAReportDaySummeryVM> QueryFuncReportSummaryAPI(QAReportSearchVM search)
        {
            var tempResult = qualityAssuranceInputMasterRepository.QueryIPQCALLProcessReportSummaryAPI(search);
            List<QAReportDaySummeryVM> result = new List<QAReportDaySummeryVM>();
            var ss = tempResult.GroupBy(p => p.FunPlant);
           
            foreach (var item in ss)
            {
                
                QAReportDaySummeryVM temp = new QAReportDaySummeryVM();
                Double FirstTargetYield = 1.00;
                Double SecondTargetYield = 1.00;
                Double FirstRejectionRate = 1.00;
                Double SecondRejectionRate = 1.00;
                int FirstCheck_Qty = 0;
                int FirstOK_Qty = 0;
                int Input = 0;
                int SepcialAccept_Qty = 0;
                int Shipment_Qty = 0;
                int NG = 0;
                foreach (var item1 in item)
                {
                    FirstTargetYield *= (Double)item1.FirstTargetYield;
                    SecondTargetYield *= (Double)item1.SecondTargetYield;
                    if(item1.FirstRejectionRate!=0)
                    FirstRejectionRate *= (Double)item1.FirstRejectionRate;
                    if (item1.SecondRejectionRate != 0)
                        SecondRejectionRate *= (Double)item1.SecondRejectionRate;
                    FirstCheck_Qty += item1.FirstCheck_Qty;
                    FirstOK_Qty += item1.FirstOK_Qty;
                    Input += item1.Input;
                    SepcialAccept_Qty += item1.SepcialAccept_Qty;
                    NG += item1.NG;
                    Shipment_Qty += item1.Shipment_Qty;
                }
               
                temp.FunPlant = item.FirstOrDefault().FunPlant; 
              
                temp.FirstCheck_Qty = FirstCheck_Qty;
                temp.FirstOK_Qty = FirstOK_Qty;
                temp.FirstTargetYield = FirstTargetYield.ToString("P");
                temp.FirstRejectionRate = FirstRejectionRate.ToString("P");
                temp.SecondRejectionRate = SecondRejectionRate.ToString("P");
                temp.SecondTargetYield = SecondTargetYield.ToString("P");
                temp.Input = Input;
                temp.Process = "Sum";
                temp.SepcialAccept_Qty = SepcialAccept_Qty;
                temp.Shipment_Qty = Shipment_Qty;
                temp.Process_Seq = 0; 
                temp.FlowChart_Detail_UID = item.Min(P=>P.FlowChart_Detail_UID);
                temp.NG = NG;
                result.Add(temp);
            }

            result = result.OrderBy(P => P.FlowChart_Detail_UID).ToList();
            return new PagedListModel<QAReportDaySummeryVM>(result.Count, result);
        }

        public PagedListModel<QAReportDaySummeryVM> QueryIPQCALLProcessReportSummaryAPI(QAReportSearchVM search)
        {
            var tempResult = qualityAssuranceInputMasterRepository.QueryIPQCALLProcessReportSummaryAPI(search);
            List<QAReportDaySummeryVM> result = new List<QAReportDaySummeryVM>();

            foreach (QAReportDaySummeryDTO tempData in tempResult)
            {
                QAReportDaySummeryVM temp = new QAReportDaySummeryVM();
                temp.FunPlant = tempData.FunPlant;
                temp.FirstCheck_Qty = tempData.FirstCheck_Qty;
                temp.FirstOK_Qty = tempData.FirstOK_Qty;
                temp.FirstTargetYield = Convert.ToDouble(tempData.FirstTargetYield).ToString("P");
                temp.FirstRejectionRate = Convert.ToDouble(tempData.FirstRejectionRate).ToString("P");
                temp.SecondRejectionRate = Convert.ToDouble(tempData.SecondRejectionRate).ToString("P");
                temp.SecondTargetYield = Convert.ToDouble(tempData.SecondTargetYield).ToString("P");
                temp.Input = tempData.Input;
                temp.Process = tempData.Process;
                temp.SepcialAccept_Qty = tempData.SepcialAccept_Qty;
                temp.Shipment_Qty = tempData.Shipment_Qty;
                temp.Process_Seq = tempData.Process_Seq;
                temp.FlowChart_Detail_UID = tempData.FlowChart_Detail_UID;
                temp.NG = tempData.NG;
                result.Add(temp);
            }


            return new PagedListModel<QAReportDaySummeryVM>(result.Count, result);
        }
        public PagedListModel<QAReportExceptionTypeRank> GetQAReportTypeRank(QAReportSearchVM search, int yield)
        {
            List<QAReportExceptionTypeRank> result = new List<QAReportExceptionTypeRank>();
            var resultData = qualityAssuranceInputMasterRepository.GetQAReportTypeRank(search, yield);
            foreach (var temp in resultData)
            {
                QAReportExceptionTypeRank tempResult = new QAReportExceptionTypeRank();
                if (search.languageID == 1)
                {
                    if (temp.ExceptionType.Substring(0, 2) == "外观")
                        tempResult.ExceptionType = "Appearance";

                    if (temp.ExceptionType.Substring(0, 2) == "尺寸")
                        tempResult.ExceptionType = "size";
                    tempResult.TypeName = temp.BadTypeEnglishCode;
                }
                else
                {
                    tempResult.ExceptionType = temp.ExceptionType.Substring(0, 2);
                    tempResult.TypeName = temp.TypeName;
                }

                tempResult.RankNum = temp.RankNum;
                tempResult.RejectionRate = Convert.ToDouble(temp.RejectionRate).ToString("P");
                tempResult.TotalCount = temp.TotalCount;
                result.Add(tempResult);
            }
            return new PagedListModel<QAReportExceptionTypeRank>(result.Count, result);
        }

        #endregion

        ///不良类型修改记录查询
        public PagedListModel<QAInputModifyVM> GetQAInputModify(QAInputModifySearch searchModel, Page page)
        {
            var totalCount = 0;
            var BadTypes = qualityAssurance_ExceptionTypeRepository.GetQAInputModify(searchModel, page, out totalCount);

            List<QAInputModifyVM> result = new List<QAInputModifyVM>();
            foreach (var temp in BadTypes)
            {
                var dvm = AutoMapper.Mapper.Map<QAInputModifyVM>(temp);
                dvm.Product_Date = temp.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                dvm.Modified_Date = temp.Modified_Date.ToString("yyyy-MM-dd hh:mm:ss");
                result.Add(dvm);
            }
            return new PagedListModel<QAInputModifyVM>(totalCount, result);
        }

        #region --- QATargetYield

        public List<QATargetRateVM> QueryCheckPointForTargetYield(int FlowChart_Master_UID)
        {

            var result = qualityAssuranceMgDataRepository.QueryCheckPointForTargetYield(FlowChart_Master_UID);
            return result;
        }

        public string UpdateTargetYieldAPI(QAMgDataListVM data)
        {
            return qualityAssuranceMgDataRepository.UpdateTargetYield(data.QAMgDataList);
        }

        public string GetWeek(DateTime d)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(d.DayOfWeek)];

            return week;
        }

        public string ModifySingleQaTargetYield(QATargetYieldVM data)
        {
            string result = "SUCCESS";
            try
            {
                var items = qualityAssuranceMgDataRepository.GetTargetYield(data.Flowchart_Detail_UID, DateTime.Parse(data.Product_Date));
                int i = 0;

                foreach (var item in items)
                {
                    i++;
                    if (GetWeek(item.ProductDate) == "星期一")
                    {
                        item.FirstRejectionRate = PerctangleToDecimal(data.MondayQAFirstRejectionRate);
                        item.SecondRejectionRate = PerctangleToDecimal(data.MondayQASecondRejectionRate);
                        qualityAssuranceMgDataRepository.Update(item);
                    }
                    if (GetWeek(item.ProductDate) == "星期二")
                    {
                        item.FirstRejectionRate = PerctangleToDecimal(data.TuesdayQAFirstRejectionRate);
                        item.SecondRejectionRate = PerctangleToDecimal(data.TuesdayQASecondRejectionRate);
                        qualityAssuranceMgDataRepository.Update(item);
                    }
                    if (GetWeek(item.ProductDate) == "星期三")
                    {
                        item.FirstRejectionRate = PerctangleToDecimal(data.WednesdayQAFirstRejectionRate);
                        item.SecondRejectionRate = PerctangleToDecimal(data.WednesdayQASecondRejectionRate);
                        qualityAssuranceMgDataRepository.Update(item);
                    }
                    if (GetWeek(item.ProductDate) == "星期四")
                    {
                        item.FirstRejectionRate = PerctangleToDecimal(data.ThursdayQAFirstRejectionRate);
                        item.SecondRejectionRate = PerctangleToDecimal(data.ThursdayQASecondRejectionRate);
                        qualityAssuranceMgDataRepository.Update(item);
                    }
                    if (GetWeek(item.ProductDate) == "星期五")
                    {
                        item.FirstRejectionRate = PerctangleToDecimal(data.FridayQAFirstRejectionRate);
                        item.SecondRejectionRate = PerctangleToDecimal(data.FridayQASecondRejectionRate);
                        qualityAssuranceMgDataRepository.Update(item);
                    }
                    if (GetWeek(item.ProductDate) == "星期六")
                    {
                        item.FirstRejectionRate = PerctangleToDecimal(data.SaterdayQAFirstRejectionRate);
                        item.SecondRejectionRate = PerctangleToDecimal(data.SaterdayQASecondRejectionRate);
                        qualityAssuranceMgDataRepository.Update(item);
                    }
                    if (GetWeek(item.ProductDate) == "星期日")
                    {
                        item.FirstRejectionRate = PerctangleToDecimal(data.SundayQAFirstRejectionRate);
                        item.SecondRejectionRate = PerctangleToDecimal(data.SundayQASecondRejectionRate);
                        qualityAssuranceMgDataRepository.Update(item);
                    }
                }
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = "Error";
            }

            return result;
        }

        /// <summary>
        /// 将百分比转换成小数
        /// </summary>
        /// <param name="perc">百分比值，可纯为数值，或都加上%号的表示，
        /// 如：65|65%</param>
        /// <returns></returns>
        decimal PerctangleToDecimal(string strInput)
        {
            try
            {
                Decimal f = Decimal.Parse(strInput.Substring(0, strInput.Length - 1));
                f /= 100;

                return f;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }


        public PagedListModel<QATargetYieldVM> QueryQATargetYield(int Flowchart_Master_UID, DateTime date)
        {

            var result = QueryQATarget(Flowchart_Master_UID, date);
            return new PagedListModel<QATargetYieldVM>(result.Count, result);
        }

        List<QATargetYieldVM> QueryQATarget(int Flowchart_Master_UID, DateTime date)
        {

            var dataFromDataBase = qualityAssuranceMgDataRepository.QueryQATargetYield(Flowchart_Master_UID, date);
            var result = new List<QATargetYieldVM>();
            foreach (var item in dataFromDataBase)
            {
                var tempResult = new QATargetYieldVM();

                tempResult.FlowChart_Master_UID = item.FlowChart_Master_UID;
                tempResult.Flowchart_Detail_UID = item.Flowchart_Detail_UID;
                tempResult.Process = item.Process;
                tempResult.Process_seq = item.Process_seq;
                tempResult.Product_Date = item.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                tempResult.Color = item.Color;

                if (item.MondayQAFirstRejectionRate != null)
                {
                    tempResult.MondayQAFirstRejectionRate = decimal.Round(decimal.Parse(item.MondayQAFirstRejectionRate.ToString()) * 100, 2) + "%";
                }
                if (item.MondayQASecondRejectionRate != null)
                {
                    tempResult.MondayQASecondRejectionRate = decimal.Round(decimal.Parse(item.MondayQASecondRejectionRate.ToString()) * 100, 2) + "%";
                }

                if (item.TuesdayQAFirstRejectionRate != null)
                {
                    tempResult.TuesdayQAFirstRejectionRate = decimal.Round(decimal.Parse(item.TuesdayQAFirstRejectionRate.ToString()) * 100, 2) + "%";
                }
                if (item.TuesdayQASecondRejectionRate != null)
                {
                    tempResult.TuesdayQASecondRejectionRate = decimal.Round(decimal.Parse(item.TuesdayQASecondRejectionRate.ToString()) * 100, 2) + "%";
                }

                if (item.WednesdayQAFirstRejectionRate != null)
                {
                    tempResult.WednesdayQAFirstRejectionRate = decimal.Round(decimal.Parse(item.WednesdayQAFirstRejectionRate.ToString()) * 100, 2) + "%";
                }
                if (item.WednesdayQASecondRejectionRate != null)
                {
                    tempResult.WednesdayQASecondRejectionRate = decimal.Round(decimal.Parse(item.WednesdayQASecondRejectionRate.ToString()) * 100, 2) + "%";
                }

                if (item.ThursdayQAFirstRejectionRate != null)
                {
                    tempResult.ThursdayQAFirstRejectionRate = decimal.Round(decimal.Parse(item.ThursdayQAFirstRejectionRate.ToString()) * 100, 2) + "%";
                }
                if (item.ThursdayQASecondRejectionRate != null)
                {
                    tempResult.ThursdayQASecondRejectionRate = decimal.Round(decimal.Parse(item.ThursdayQASecondRejectionRate.ToString()) * 100, 2) + "%";
                }

                if (item.FridayQAFirstRejectionRate != null)
                {
                    tempResult.FridayQAFirstRejectionRate = decimal.Round(decimal.Parse(item.FridayQAFirstRejectionRate.ToString()) * 100, 2) + "%";
                }
                if (item.FridayQASecondRejectionRate != null)
                {
                    tempResult.FridayQASecondRejectionRate = decimal.Round(decimal.Parse(item.FridayQASecondRejectionRate.ToString()) * 100, 2) + "%";
                }

                if (item.SaterdayQAFirstRejectionRate != null)
                {
                    tempResult.SaterdayQAFirstRejectionRate = decimal.Round(decimal.Parse(item.SaterdayQAFirstRejectionRate.ToString()) * 100, 2) + "%";
                }
                if (item.SaterdayQASecondRejectionRate != null)
                {
                    tempResult.SaterdayQASecondRejectionRate = decimal.Round(decimal.Parse(item.SaterdayQASecondRejectionRate.ToString()) * 100, 2) + "%";
                }

                if (item.SundayQAFirstRejectionRate != null)
                {
                    tempResult.SundayQAFirstRejectionRate = decimal.Round(decimal.Parse(item.SundayQAFirstRejectionRate.ToString()) * 100, 2) + "%";
                }
                if (item.SundayQASecondRejectionRate != null)
                {
                    tempResult.SundayQASecondRejectionRate = decimal.Round(decimal.Parse(item.SundayQASecondRejectionRate.ToString()) * 100, 2) + "%";
                }
                result.Add(tempResult);
            }

            return result;

        }


        public QATargetYieldVM QueryQAProcessMGDataSingle(int Flowchart_Master_UID, int Flowchart_Detail_UID, DateTime date)
        {

            var result = QueryQATarget(Flowchart_Master_UID, date);
            return result.Find(x => x.Flowchart_Detail_UID == Flowchart_Detail_UID);
        }

        #endregion

        #region  不良类型与制程绑定
        public PagedListModel<ExceptionTypeListVM> QueryExcepProcS(ExcepTypeFlowChartSearch searchModel, Page page)
        {
            var totalCount = 0;

            var BadTypes = exceptionTypeWithFlowchartRepository.QueryExcepProcS(searchModel, page, out totalCount);

            return new PagedListModel<ExceptionTypeListVM>(totalCount, BadTypes);
        }

        public string ImportExcepProcess(ExceptionTypeProcessList pDataListTem)
        {
            string result = "";
            List<ExceptionTypeFlowChartVM> lists = new List<ExceptionTypeFlowChartVM>();

            lists = pDataListTem.ExceptionTypeProcessLists;
            int i = 1;
            foreach (ExceptionTypeFlowChartVM item in lists)
            {
                i++;
                ExceptionTypeWithFlowchart excePro = new ExceptionTypeWithFlowchart();
                excePro.Creator_Date = item.Creator_Date;
                excePro.Creator_UID = item.Creator_UID;

                excePro.ExceptionType_UID = getExcUID(item.ExceptionType_Name, item.FlowChart_Master_UID);
                if (excePro.ExceptionType_UID == 0)
                {
                    return "第" + i + "行对应的专案和不良类型名在系统中找不到，请核对后再上传。";
                }

                excePro.FlowChart_Master_UID = item.FlowChart_Master_UID;

                excePro.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
                excePro.FunPlant = item.FunPlant;
                excePro.TypeClassify = item.TypeClassify;
                excePro.ExceptionTypeWithFlowchart_UID = 0;
                exceptionTypeWithFlowchartRepository.Add(excePro);

            }
            try
            {

                unitOfWork.Commit();
                result = "OK";
            }
            catch (Exception ex)
            {
                result = "Error";
                return ex.ToString();
            }



            return result;
        }

        private int getExcUID(string name, int Flowchart_Master_UID)
        {
            return qualityAssurance_ExceptionTypeRepository.getExcUID(name, Flowchart_Master_UID);
        }

        public string AddExecProcess(ExceptionTypeFlowChartVM item)
        {
            ExceptionTypeWithFlowchart excePro = new ExceptionTypeWithFlowchart();
            excePro.ExceptionType_UID = getExcUID(item.ExceptionType_Name, item.FlowChart_Master_UID);

            excePro.FlowChart_Master_UID = item.FlowChart_Master_UID;

            excePro.FlowChart_Detail_UID = item.FlowChart_Detail_UID;
            excePro.FunPlant = item.FunPlant;
            excePro.TypeClassify = item.TypeClassify;
            excePro.ExceptionTypeWithFlowchart_UID = 0;
            excePro.Creator_Date = item.Creator_Date;
            excePro.Creator_UID = item.Creator_UID;
            try
            {
                exceptionTypeWithFlowchartRepository.Add(excePro);
                unitOfWork.Commit();
            }
            catch
            {
                return "新增失败，可能原因是系统已经存在该条数据，请核对后再重试，或联系系统管理员。";
            }
            return "OK";
        }


        public string DeleteExecProcess(int uiid)
        {
            var item = exceptionTypeWithFlowchartRepository.GetById(uiid);
            try
            {
                exceptionTypeWithFlowchartRepository.Delete(item);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "OK";

        }



        public string DeleteAllExceptionProcess(int FlowChart_Detail_UID)
        {

            return exceptionTypeWithFlowchartRepository.DeleteAllExceptionProcess(FlowChart_Detail_UID);
        }


        #endregion

        #region ---OQC

        /// <summary>
        /// master页面初始化
        /// </summary>
        /// <param name="searhModel"></param>
        /// <returns></returns>

        public OQC_InputMasterVM QueryOQCMasterData(QAReportSearchVM searhModel)
        {
            OQC_InputMasterDTO data = qualityAssurance_OQC_InputMasterRepository.QueryOQCMasterData(searhModel);

            OQC_InputMasterVM result = new OQC_InputMasterVM();

            result.FlowChart_Detail_UID = data.FlowChart_Detail_UID;
            result.OQCMater_UID = data.OQCMater_UID;
            result.Time_interval = data.Time_interval;
            result.ProductDate = data.ProductDate;
            result.MaterialType = data.MaterialType;
            result.Color = data.Color;
            result.Input = data.Input;
            result.GoodParts_Qty = data.GoodParts_Qty;
            result.NGParts_Qty = data.NGParts_Qty;
            result.Rework = data.Rework;
            result.ProductLineRework = data.ProductLineRework;
            result.ReworkQtyFromAssemble = data.ReworkQtyFromAssemble;
            result.RepairNG_Qty = data.RepairNG_Qty;
            result.NG_Qty = data.NG_Qty;
            result.RepairNG_Yield = Convert.ToDouble(data.RepairNG_Yield).ToString("P");
            result.NG_Yield = Convert.ToDouble(data.NG_Yield).ToString("P");
            result.FirstYieldRate = Convert.ToDouble(data.FirstYieldRate).ToString("P");
            result.SecondYieldRate = Convert.ToDouble(data.SecondYieldRate).ToString("P");
            result.Storage_Qty = data.Storage_Qty;
            result.WaitStorage_Qty = data.WaitStorage_Qty;
            result.WIP = data.WIP;
            result.Creator_UID = data.Creator_UID;
            result.Create_date = data.Create_date;
            result.Modified_date = data.Modified_date;
            result.Modifier_UID = data.Modifier_UID;
            result.Project_UID = data.Project_UID;
            result.ReworkQtyFromOQC = data.ReworkQtyFromOQC;

            return result;
        }

        public PagedListModel<OQC_InputDetailVM> QueryOQCNGDetails(QAReportSearchVM searhModel)
        {
            List<OQC_InputDetailVM> result = qualityAssurance_OQC_InputDetailRepository.QueryOQCExceptionDetails(searhModel, "不良明细");

            return new PagedListModel<OQC_InputDetailVM>(result.Count, result);
        }

        public PagedListModel<OQC_InputDetailVM> QueryOQCReworkDetails(QAReportSearchVM searhModel)
        {
            List<OQC_InputDetailVM> result = qualityAssurance_OQC_InputDetailRepository.QueryOQCExceptionDetails(searhModel, "返修明细");

            return new PagedListModel<OQC_InputDetailVM>(result.Count, result);
        }

        public PagedListModel<OQC_InputDetailVM> QueryOQCNGRecord(QAReportSearchVM searhModel)
        {
            List<OQC_InputDetailVM> result = qualityAssurance_OQC_InputDetailRepository.QueryOQCExceptionRecord(searhModel, "不良明细");
            return new PagedListModel<OQC_InputDetailVM>(result.Count, result);
        }

        public PagedListModel<OQC_InputDetailVM> QueryOQCReworkRecord(QAReportSearchVM searhModel)
        {
            List<OQC_InputDetailVM> result = qualityAssurance_OQC_InputDetailRepository.QueryOQCExceptionRecord(searhModel, "返修明细");

            return new PagedListModel<OQC_InputDetailVM>(result.Count, result);
        }

        public string SaveOQCData(OQCInputData data)
        {
            return qualityAssurance_OQC_InputMasterRepository.SaveOQCData(data);
        }

        public string AddTExceptionTypesoFlowChart(ExceptionTypesAddToFlowChartVM data)
        {
            return exceptionTypeWithFlowchartRepository.AddTExceptionTypesoFlowChart(data);
        }


        public OQC_InputMasterVM QueryOQCRecordData(QAReportSearchVM searhModel)
        {
            if (searhModel.Tab_Select_Text == "夜班小计")
                searhModel.Tab_Select_Text = "Night_Sum";
            else if (searhModel.Tab_Select_Text == "白班小计")
                searhModel.Tab_Select_Text = "Daily_Sum";
            else if (searhModel.Tab_Select_Text == "全天")
                searhModel.Tab_Select_Text = "ALL";
            if (searhModel.Tab_Select_Text == "0")
            {
                searhModel.Time_interval = "ALL";
                searhModel.Tab_Select_Text = "ALL";
            }
            OQC_InputMasterDTO data = qualityAssurance_OQC_InputMasterRepository.QueryOQCRecordData(searhModel);

            OQC_InputMasterVM result = new OQC_InputMasterVM();

            result.FlowChart_Detail_UID = data.FlowChart_Detail_UID;
            result.OQCMater_UID = data.OQCMater_UID;
            result.Time_interval = data.Time_interval;
            result.ProductDate = data.ProductDate;
            result.MaterialType = data.MaterialType;
            result.Color = data.Color;
            result.Input = data.Input;
            result.GoodParts_Qty = data.GoodParts_Qty;
            result.NGParts_Qty = data.NGParts_Qty;
            result.Rework = data.Rework;
            result.ProductLineRework = data.ProductLineRework;
            result.ReworkQtyFromAssemble = data.ReworkQtyFromAssemble;
            result.RepairNG_Qty = data.RepairNG_Qty;
            result.NG_Qty = data.NG_Qty;
            result.RepairNG_Yield = Convert.ToDecimal((data.RepairNG_Yield * 100)).ToString("F4") + "%";
            result.NG_Yield = Convert.ToDecimal((data.NG_Yield * 100)).ToString("F4") + "%";
            result.FirstYieldRate = searhModel.Process == "UMP2" ? "" : Convert.ToDecimal((data.FirstYieldRate * 100)).ToString("F4") + "%";
            result.SecondYieldRate = Convert.ToDecimal((data.SecondYieldRate * 100)).ToString("F4") + "%";
            result.Storage_Qty = data.Storage_Qty;
            result.WaitStorage_Qty = data.WaitStorage_Qty;
            result.WIP = data.WIP;
            result.Creator_UID = data.Creator_UID;
            result.Create_date = data.Create_date;
            result.Modified_date = data.Modified_date;
            result.Modifier_UID = data.Modifier_UID;
            result.Project_UID = data.Project_UID;
            result.Process = searhModel.Process;
            result.ReworkQtyFromOQC = data.ReworkQtyFromOQC;
            return result;
        }

        public OQCRecordCondition QueryOQCReportConditions(string processName, string Project)
        {
            OQCRecordCondition reuslt = new OQCRecordCondition();
            List<string> QA_MaterialType = enumerationRepository.GetEnumValuebyType("MaterielType");
            // reuslt = flowChartDetailRepository.QueryOQCReportConditions(processName, Project);
            reuslt.MaterialType = QA_MaterialType;

            return reuslt;
        }

        public OQCReportExcel QueryQAOQCReport(QAReportSearchVM data)
        {
            List<OQCReprotVM> SumData = GetOQCReprotVM(data);
            List<OQCReprotTopFiveTypeVM> DetailData = GETQOCReportTopFive(data);

            OQCReportExcel result = new OQCReportExcel();
            result.SumData = SumData;
            result.TopFive = DetailData;

            return result;
        }

        public PagedListModel<OQCReprotVM> GetQAReportOQCDaySummery(QAReportSearchVM search)
        {
            List<OQCReprotVM> result = GetOQCReprotVM(search);
            return new PagedListModel<OQCReprotVM>(result.Count, result);
        }

        private List<OQCReprotVM> GetOQCReprotVM(QAReportSearchVM search)
        {
            List<OQCReprotDTO> resultData = qualityAssurance_OQC_InputMasterRepository.GetQAReportOQCDaySummery(search);

            List<OQCReprotVM> result = new List<OQCReprotVM>();
            foreach (var temp in resultData)
            {
                OQCReprotVM tempResult = new OQCReprotVM();
                tempResult.Process = temp.Process;
                tempResult.DailyInput = temp.DailyInput;
                tempResult.DailyOK = temp.DailyOK;
                tempResult.DailyNG = temp.DailyNG;
                tempResult.DailyRework = temp.DailyRework;
                tempResult.NightInput = temp.NightInput;
                tempResult.NightOK = temp.NightOK;
                tempResult.NightNG = temp.NightNG;
                tempResult.NightRework = temp.NightRework;
                tempResult.Input = temp.Input;
                tempResult.OK = temp.OK;
                tempResult.NG = temp.NG;
                tempResult.Rework = temp.Rework;
                tempResult.WIP = temp.WIP;
                tempResult.FirstYieldRate = Convert.ToDouble(temp.FirstYieldRate).ToString("P");
                tempResult.SecondYieldRate = Convert.ToDouble(temp.SecondYieldRate).ToString("P");
                tempResult.FlowChart_Detail_UID = temp.FlowChart_Detail_UID;
                result.Add(tempResult);
            }

            return result;
        }

        public PagedListModel<OQCReprotTopFiveTypeVM> GetQAReportOQCTypeRank(QAReportSearchVM search)
        {
            search.Count = 5;
            List<OQCReprotTopFiveTypeVM> result = GETQOCReportTopFive(search);
            return new PagedListModel<OQCReprotTopFiveTypeVM>(result.Count, result);
        }

        private List<OQCReprotTopFiveTypeVM> GETQOCReportTopFive(QAReportSearchVM search)
        {
            List<OQCReprotTopFiveTypeDTO> resultData = qualityAssurance_OQC_InputMasterRepository.GetQAReportOQCTypeRank(search);
            List<OQCReprotTopFiveTypeVM> result = new List<OQCReprotTopFiveTypeVM>();
            foreach (var temp in resultData)
            {
                OQCReprotTopFiveTypeVM tempResult = new OQCReprotTopFiveTypeVM();
                tempResult.Qty = temp.Qty;
                tempResult.RankNum = temp.RankNum;
                tempResult.YieldRate = Convert.ToDouble(temp.YieldRate).ToString("P");
                tempResult.TypeName = temp.TypeName;
                if (search.languageID == 1)
                {
                    if (temp.TOPType == "一次良率Top5")
                        tempResult.TOPType = "The Top 5 of the first check yield";

                    if (temp.TOPType == "二次良率Top5")
                        tempResult.TOPType = "The Top 5 of the second check yield";
                }
                else
                    tempResult.TOPType = temp.TOPType;
                tempResult.Process = temp.Process;
                result.Add(tempResult);
            }
            return result;
        }


        public List<string> QueryRecordColor(int Flowchart_Master_UID, string FunPlant, string ProductDate, string MaterialType)
        {
            return flowChartDetailRepository.QueryRecordColor(Flowchart_Master_UID, FunPlant, ProductDate, MaterialType);
        }

        public List<FunPlantVM> QueryFunPlant(int Flowchart_Master_UID)
        {
            return flowChartDetailRepository.QueryFunPlant(Flowchart_Master_UID);
        }

        //---明细数据导出

        public ExportOQCDataForExcel GetOQCInputDataForExportExcel(QAReportSearchVM data)
        {
            ExportOQCDataForExcel result = new ExportOQCDataForExcel();

            List<FlowchartColor> Process = qualityAssuranceInputMasterRepository.GetProcessByProject(data.FlowChart_Master_UID, "OQC");

            data.Tab_Select_Text = "全天";
            List<OQCExportModel> DataList = new List<OQCExportModel>();

            foreach (FlowchartColor temp in Process)
            {

                if (!string.IsNullOrEmpty(temp.Color) && temp.Color != data.Color)
                {
                    continue;
                }
                data.Process_seq = temp.Process_Seq;
                data.Process = temp.Process;

                data.FlowChart_Detail_UID = temp.Flowchart_Detail_UID;
                OQCExportModel tempData = new OQCExportModel();
                tempData.MasterData = QueryOQCRecordData(data);

                List<OQC_InputDetailVM> ReworkdetailsTempData = qualityAssurance_OQC_InputDetailRepository.QueryOQCExceptionRecord(data, "返修明细").ToList();
                List<OQC_InputDetailVM> NGdetailsTempData = qualityAssurance_OQC_InputDetailRepository.QueryOQCExceptionRecord(data, "不良明细").ToList();

                tempData.DetailDatas.AddRange(NGdetailsTempData);
                tempData.DetailDatas.AddRange(ReworkdetailsTempData);

                DataList.Add(tempData);
            }
            result.OQCDatas = DataList;

            return result;
        }






        #endregion

        #region -----Product&Sale Report

        public PagedListModel<ProductSaleReport_RateVM> QueryProductSaleReportSummery(QAReportSearchVM searchModel)
        {
            List<ProductSaleReport_RateVM> result = qualityAssurance_OQC_InputMasterRepository.QueryProductSaleReportSummery(searchModel);

            return new PagedListModel<ProductSaleReport_RateVM>(result.Count, result);
        }

        public PagedListModel<ProductSaleReport_RateVM> QueryProductSaleReportFunplantDetail(QAReportSearchVM searchModel)
        {
            List<ProductSaleReport_RateVM> result = qualityAssurance_OQC_InputMasterRepository.QueryProductSaleReportFunplantDetail(searchModel);

            return new PagedListModel<ProductSaleReport_RateVM>(result.Count, result);
        }

        public PagedListModel<ProductSaleReport_RateVM> QueryProductSaleReportExceptionTypeDetail(string TypeFatherName, string FunPlant, string ProductDate, int FlowChart_Detail_UID, int RateType, string Color, string MeterialType)
        {
            List<ProductSaleReport_RateVM> result = qualityAssurance_OQC_InputMasterRepository.QueryProductSaleReportExceptionTypeDetail(TypeFatherName, FunPlant, ProductDate, FlowChart_Detail_UID, RateType, Color, MeterialType);
            return new PagedListModel<ProductSaleReport_RateVM>(0, result);
        }

        public PagedListModel<TimeIntervalFPYReportVM> QueryTimeIntervalFPYReport(QAReportSearchVM searchModel)
        {
            List<TimeIntervalFPYReportVM> result = qualityAssurance_OQC_InputMasterRepository.QueryTimeIntervalFPYReport(searchModel);
            return new PagedListModel<TimeIntervalFPYReportVM>(result.Count, result);
        }

        #region --- QA反推良率分配界面

        public PagedListModel<QABackToFunPlant> QueryQABackToFunPlantInfo(QAReportSearchVM search)
        {
            List<QABackToFunPlant> result = qualityAssurance_OQC_InputMasterRepository.QueryQABackToFunPlantInfo(search);
            return new PagedListModel<QABackToFunPlant>(result.Count, result);
        }

        public QABackToFunPlant QueryQABTFInfoByUID(string QualityAssurance_DistributeRate_UID)
        {
            QABackToFunPlant result = new QABackToFunPlant();
            result = qualityAssurance_OQC_InputMasterRepository.QueryQABTFInfoByUID(QualityAssurance_DistributeRate_UID);
            return result;

        }
        public List<OrgVM> QueryOptype(int Account_uid, int Plant_OrganizationUID)
        {
            List<OrgVM> result = new List<OrgVM>();
            var funResult = systemOrgRepository.GetOpTypeByPlant(Plant_OrganizationUID, 1);
            foreach (SystemOrgDTO org in funResult)
            {
                OrgVM torg = new OrgVM();
                torg.Organization_UID = org.Organization_UID;
                torg.Name = org.Organization_Name;
                result.Add(torg);
            }
            return result;
        }

        public string SaveBackToFunPlantInfo(QABackToFunPlantListVM data)
        {
            return qualityAssurance_OQC_InputMasterRepository.SaveBackToFunPlantInfo(data);
        }

        public string UpdateBackToFunPlantInfo(QABackToFunPlant data)
        {
            return qualityAssurance_OQC_InputMasterRepository.UpdateBackToFunPlantInfo(data);
        }

        public PagedListModel<ProductSaleReport_RateVM> QueryDistributeRateReportDetail(QAReportSearchVM searchModel)
        {
            List<ProductSaleReport_RateVM> result = qualityAssurance_OQC_InputMasterRepository.QueryDistributeRateReportDetail(searchModel);
            return new PagedListModel<ProductSaleReport_RateVM>(result.Count, result);
        }

        public List<ProductSaleReport_RateVM> QueryDistributeRateReportExceptionTypeDetail(string TypeFatherName, string FunPlant,
            string ProductDate, int FlowChart_Master_UID, int RateType, string Color, string MeterialType, int OPType_OrganizationUID)
        {
            return qualityAssurance_OQC_InputMasterRepository.QueryDistributeRateReportExceptionTypeDetail(TypeFatherName, FunPlant, ProductDate, FlowChart_Master_UID, RateType, Color, MeterialType, OPType_OrganizationUID);
        }


        #endregion
        #endregion
    }
}
