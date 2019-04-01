using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Results;
using PDMS.Model.ViewModels;
using PDMS.Data;
using PDMS.Common.Helpers;
using PDMS.Model.EntityDTO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PDMS.WebAPI.Controllers
{
    public class QualityController : ApiControllerBase
    {
        IQualityService QualityService;
        IEventReportManagerService EventReportManagerService;
        IProcessIDTRSConfigService ProcessIDTRSConfigService;
        IMachineYieldReportService machineYieldReportService;
        IMES_SNOriginalService MES_SNOriginalService;
        public QualityController(IQualityService ChartService,
             IEventReportManagerService EventReportManagerService,
             IProcessIDTRSConfigService ProcessIDTRSConfigService,
             IMachineYieldReportService machineYieldReportService,
             IMES_SNOriginalService MES_SNOriginalService
            )
        {
            this.QualityService = ChartService;
            this.EventReportManagerService = EventReportManagerService;
            this.ProcessIDTRSConfigService = ProcessIDTRSConfigService;
            this.machineYieldReportService = machineYieldReportService;
            this.MES_SNOriginalService = MES_SNOriginalService;
        }

        #region ----Common Method
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryPlantAPI(int Account_UId)
        {
            var project = QualityService.QueryOrganzitionInfo(Account_UId);
            return Ok(project);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult GetProcessSource(string FunPlant, int Flowchart_Master_UID, string Product_Date, string Color)
        {
            var query = QualityService.GetProcessSource(FunPlant, Flowchart_Master_UID, DateTime.Parse(Product_Date), Color);
            return Ok(query);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOPTypeAPI(int Account_UId, int Plant_OrganizationUID)
        {
            var result = QualityService.QueryOrganzitionInfo(Account_UId, Plant_OrganizationUID);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult GetQAProjectAPI(int OPType_Organization_UID)
        {
            var project = QualityService.GetQAProject(OPType_Organization_UID);
            return Ok(project);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult GetPartTypeAPI(int Project_UID)
        {
            var project = QualityService.GetPartType(Project_UID);
            return Ok(project);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryRecordColorAPI(int Flowchart_Master_UID, string FunPlant, string ProductDate, string MaterialType)
        {
            var result = QualityService.QueryRecordColor(Flowchart_Master_UID, FunPlant, ProductDate, MaterialType);
            return Ok(result);
        }


        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryFunPlantAPI(int Flowchart_Master_UID)
        {
            var result = QualityService.QueryFunPlant(Flowchart_Master_UID);
            return Ok(result);

        }
        #endregion

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetCheckPointsList(int UserUid, int FlowChart_Master_UID, string Color, string MaterialType)
        {
            var query = QualityService.GetCheckPointsList(UserUid, FlowChart_Master_UID, Color, MaterialType);
            return Ok(query);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryInputConditions(int FlowChart_Master_UID)
        {
            var query = QualityService.QueryInputConditions(FlowChart_Master_UID);
            return Ok(query);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQAInputDataAPI(dynamic data)
        {
            var jsonData = data.ToString();

            var conditionModel = JsonConvert.DeserializeObject<CheckPointInputConditionModel>(jsonData);
            var time = EventReportManagerService.GetIntervalInfo("OP1");

            var result = QualityService.QueryQAMasterData(conditionModel, DateTime.Parse(time.NowDate), time.Time_Interval);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult SaveQaMasterData(dynamic data)
        {
            var jsonData = data.ToString();
            QAMasterVM qaMasterData = JsonConvert.DeserializeObject<QAMasterVM>(jsonData);

            var time = EventReportManagerService.GetIntervalInfo("OP1");
            qaMasterData.Product_Date = DateTime.Parse(time.NowDate);
            qaMasterData.Time_Interval = time.Time_Interval;

            var result = QualityService.SaveQaMasterData(qaMasterData);


            #region 同步数据暂时不同步
            //var flowChart_Detail_UID = int.Parse(qaMasterData.FlowChart_Detail_UID.ToString());
            //var time_Interval = qaMasterData.Time_Interval;
            //var curDate = qaMasterData.Product_Date.ToString("yyyy/MM/dd");

            //通过flowchar_UID,日期，时段，获取临时表的数据
            //var mesSyncData = ProcessIDTRSConfigService.GetProcessDataById(curDate, time_Interval, flowChart_Detail_UID);
            //if (mesSyncData.Input == null && mesSyncData.NG_Qty == null)
            //{
            //    return Ok(result);
            //}

            //qaMasterData.Input = mesSyncData.Input;
            //qaMasterData.NG_Qty = mesSyncData.NG_Qty;
            #endregion
            QualityService.updateMesSynsData(qaMasterData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult ModifyQAMasterDataAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QAMasterVM qaMasterData = JsonConvert.DeserializeObject<QAMasterVM>(jsonData);
            var result = QualityService.ModifyQAMasterData(qaMasterData);
            //var model = QualityService.GetInputMasterByID(qaMasterData.QualityAssurance_InputMaster_UID);
            //if (model == null)
            //{
            //    return Ok(result);
            //}

            //var flowChart_Detail_UID = int.Parse(model.FlowChart_Detail_UID.ToString());
            //var time_Interval = model.Time_Interval;
            //var curDate = model.Product_Date.ToString("yyyy/MM/dd");

            ////通过flowchar_UID,日期，时段，获取临时表的数据
            //var mesSyncData = ProcessIDTRSConfigService.GetProcessDataById(curDate, time_Interval, flowChart_Detail_UID);
            //if (mesSyncData.Input == null && mesSyncData.NG_Qty == null)
            //{
            //    return Ok(result);
            //}

            //qaMasterData.Input = mesSyncData.Input;
            //qaMasterData.NG_Qty = mesSyncData.NG_Qty;
            //QualityService.updateMesSynsDataByUID(qaMasterData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQAHistroyDatasAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QAReportSearchVM conditionModel = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            IntervalEnum time = EventReportManagerService.GetIntervalInfo("OP1");
            if (string.IsNullOrEmpty(conditionModel.Time_interval) && string.IsNullOrEmpty(conditionModel.Tab_Select_Text))
            {
                conditionModel.Time_interval = time.Time_Interval;
                conditionModel.ProductDate = DateTime.Parse(time.NowDate);
            }

            var result = QualityService.QueryQAHistroyDatas(conditionModel, DateTime.Parse(time.NowDate));
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public string SaveDatasAPI(dynamic data)
        {
            //var entity = JsonConvert.DeserializeObject<ExceptionTypeList>(data.ToString());
            //return QualityService.AddExceptionTypes(entity);

            return "OK";
        }

        [IgnoreDBAuthorize]
        public string AddBadTypeAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<ExceptionTypeVM>(data.ToString());
            return QualityService.AddExceptionType(entity);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryBadTypesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<BadTypeSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var process = QualityService.QueryBadTypes(searchModel, page);
            return Ok(process);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public string ImportExcelAPI(dynamic json)
        {
            var jsonData = json.ToString();
            var list = JsonConvert.DeserializeObject<ExceptionTypeTempList>(jsonData);
            var result = QualityService.ImportExceptionTypes(list);
            return result;
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryBadTypeByUIDAPI(int uuid)
        {
            var dto = new ExceptionTypeVM();
            dto = AutoMapper.Mapper.Map<ExceptionTypeVM>(QualityService.QueryBadTypeByUID(uuid));
            return Ok(dto);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public string DeleteBadTypeAPI(int uuid)
        {
            return QualityService.DeleteBadType(uuid);

        }

        [IgnoreDBAuthorize]
        public string CheckBadTypeByNameAPI(dynamic data)
        {
            string typeName = data.ToString();
            var process = QualityService.CheckBadTypeByName(typeName);
            return process;

        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public string CheckTypeByCodeAPI(string code)
        {
            var process = QualityService.CheckBadTypeByCode(code);
            return process;

        }

        [IgnoreDBAuthorize]
        public string CheckBadTypeByCodeAPI(dynamic data)
        {
            string code = data.ToString();
            var process = QualityService.CheckBadTypeByCode(code);
            return process;

        }

        [IgnoreDBAuthorize]
        public string ModifyBadTypeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ExceptionTypeVM>(jsonData);

            var process = QualityService.ModifyBadType(searchModel);
            return process;
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult DownloadIPQCInputDataForExportExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.IPQCInputDataForExportExcel(SearchData);
            return Ok(result);
        }

        #region --- QADetails

        #region --- 数据维护页面
        /// <summary>
        /// 查询
        /// 页面初始化 调用，传递QAMasterUID
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult QueryQAInputDetailVMAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QADetailSearch>(jsonData);
            List<MesNGResult> ResultInfo = new List<MesNGResult>();
            try
            {
                var model = machineYieldReportService.GetStationList(SearchData.FlowChart_Master_UID, SearchData.FlowChart_Detail_UID);
                if (!string.IsNullOrEmpty(model.MES_Customer_Name) && !string.IsNullOrEmpty(model.MES_Station_Name))
                {
                    //通过时段获取开始时间结束时间
                    var requestInterval = machineYieldReportService.GetMesNGParam(SearchData, model);
                    //调用MES的接口
                    var apiUrl = string.Format("pis/NgDetail?Customer={0}&StartTime={1}&EndTime={2}&Station={3}", model.MES_Customer_Name, requestInterval.startTime, requestInterval.endTime, model.MES_Station_Name);
                    var path = SearchData.MesAPIPath + apiUrl;
                    var resultMes = HttpHelper.HttpGet(path);
                    JObject josnData = (JObject)JsonConvert.DeserializeObject(resultMes);
                    var NgDetailList = JsonConvert.DeserializeObject<List<MesNgDetail>>(josnData["data"].ToString());


                    //用SN的List, 颜色去过滤
                    var SNList = NgDetailList.Select(i => i.SN).ToList();
                    var color = (string)SearchData.Color;
                    //var MES_ColoredData = MES_SNOriginalService.QueryMES_SNOriginalBySN(SNList).Where(x => x.Color == color);
                    var MES_ColoredData = MES_SNOriginalService.GetMES_SNOriginalBySN(SNList).Where(x => x.Color == color);
                    //从MES_SNOriginal获取到数据后再过滤
                    var MES_SNList = MES_ColoredData.Select(x => x.SeriesNumber);
                    NgDetailList = NgDetailList.Where(x => MES_SNList.Contains(x.SN)).ToList();

                    ResultInfo = NgDetailList.GroupBy(x => new { x.DefectName }).Select(g => new MesNGResult { DefectName = g.Key.DefectName, NG_Point = g.Count() }).ToList();
                }
            }
            catch (Exception)
            {
            }

            var result = QualityService.QueryQAInputDetailVM(SearchData, ResultInfo);
            return Ok(result);
        }

        /// <summary>
        ///  查询页面使用 获取Type的API
        /// </summary>
        /// <param name="typeLevel"></param>
        /// <param name="parentCode"></param>
        /// <param name="QAMasterUID"></param>
        /// <returns></returns>
        [IgnoreDBAuthorize]
        [AcceptVerbs("Get")]
        public IHttpActionResult QueryExceptionTypeForSearchAPI(int typeLevel, string parentCode, int QAMasterUID, string ProductDate, int Flowchart_Master_UID)
        {
            var result = QualityService.QueryExceptionTypeForSearchAPI(typeLevel, parentCode, QAMasterUID, Convert.ToDateTime(ProductDate), Flowchart_Master_UID);
            return Ok(result);
        }


        /// <summary>
        /// 编辑使用，Delete的时候 将IsDelete字段设置为true
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult ModifyQAInputDetailAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QAInputDetailVM qaMasterData = JsonConvert.DeserializeObject<QAInputDetailVM>(jsonData);
            var result = QualityService.ModifyQAInputDetail(qaMasterData);
            return Ok(result);
        }

        /// <summary>
        /// 页面数据整体保存调用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult UpdateQAInputDetailAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QAInputDetailListVM qaMasterData = JsonConvert.DeserializeObject<QAInputDetailListVM>(jsonData);
            var result = QualityService.UpdateQAInputDetail(qaMasterData);
            return Ok(result);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QuerySingleQAInputDetailInfoAPI(int QADetailUID)
        {
            var result = QualityService.QuerySingleQAInputDetailInfoAPI(QADetailUID);
            return Ok(result);
        }



        #endregion

        #region --- 不良类型新增页面

        /// <summary>
        /// 获取Type
        /// </summary>
        /// <param name="typeLevel"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryExceptionTypeForAddAPI(int typeLevel, string parentCode, int Flowchart_Master_UID)
        {
            var result = QualityService.QueryExceptionTypeForAddAPI(typeLevel, Flowchart_Master_UID, parentCode);
            return Ok(result);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult InsertQAInputDetailAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QAInputDetailListVM qaMasterData = JsonConvert.DeserializeObject<QAInputDetailListVM>(jsonData);
            var result = QualityService.InsertQAInputDetail(qaMasterData);
            return Ok(result);
        }


        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQAExceptionTypeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QADetailSearch>(jsonData);
            var result = QualityService.QueryQAExceptionTypeAPI(SearchData);
            return Ok(result);
        }


        #endregion

        #endregion

        #region ---- QAReport



        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQADayReportSummeryAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetQAReportDaySummery(SearchData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQADayReportFirstTopTenAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetQAReportTypeRank(SearchData, 1);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllFirstTargetYield(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetAllFirstTargetYield(SearchData);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllSecondTargetYield(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetAllSecondTargetYield(SearchData);
            return Ok(result);
        }
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQADayReportSecondTopTenAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetQAReportTypeRank(SearchData, 2);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult DownloadIPQCReportExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryQAIPQCReport(SearchData);
            return Ok(result);
        }

        

         [IgnoreDBAuthorize]
        public IHttpActionResult QueryFuncReportSummaryAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryFuncReportSummaryAPI(SearchData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryIPQCALLProcessReportSummaryAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryIPQCALLProcessReportSummaryAPI(SearchData);
            return Ok(result);
        }


        #endregion

        #region  ---- 不良明细 修改记录 查询

        /// <summary>
        /// 不良类型修改记录查询
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ///       [HttpPost]
        [IgnoreDBAuthorize]
        [HttpPost]
        public IHttpActionResult GetQAInputModifyAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAInputModifySearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = QualityService.GetQAInputModify(SearchData, page);
            return Ok(result);
        }

        #endregion

        #region ---QA Target Yield

        /// <summary>
        /// 下载Excel模板，获取检测点信息
        /// </summary>
        /// <param name="masterUID"></param>
        /// <returns></returns>
        [HttpGet]
        [IgnoreDBAuthorize]

        public IHttpActionResult QueryCheckPointForTargetYieldAPI(int FlowChart_Master_UID)
        {

            var result = QualityService.QueryCheckPointForTargetYield(FlowChart_Master_UID);
            return Ok(result);

        }

        /// <summary>
        /// Excel 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult ImportTargetYieldAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var importData = JsonConvert.DeserializeObject<QAMgDataListVM>(jsonData);

            var result = QualityService.UpdateTargetYieldAPI(importData);
            return Ok(result);
        }

        /// <summary>
        /// 修改 单个检测点 目标良率
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult ModifySingleQaTargetYieldAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var modifyData = JsonConvert.DeserializeObject<QATargetYieldVM>(jsonData);

            var result = QualityService.ModifySingleQaTargetYield(modifyData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        [AcceptVerbs("GET")]

        public IHttpActionResult QueryQATargetYieldAPI(int Flowchart_Master_UID, string date)
        {
            DateTime nowDate = DateTime.Parse(date);

            var result = QualityService.QueryQATargetYield(Flowchart_Master_UID, nowDate);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        [AcceptVerbs("GET")]
        public IHttpActionResult QueryQAProcessMGDataSingleAPI(int Flowchart_Master_UID, int Flowchart_Detail_UID, string date)
        {
            DateTime nowDate = DateTime.Parse(date);

            var result = QualityService.QueryQAProcessMGDataSingle(Flowchart_Master_UID, Flowchart_Detail_UID, nowDate);
            return Ok(result);
        }

        #endregion

        #region    不良名称与不良点的绑定
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryExcepProcSAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ExcepTypeFlowChartSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var process = QualityService.QueryExcepProcS(searchModel, page);
            return Ok(process);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public string ImportExcepAPI(dynamic json)
        {
            var jsonData = json.ToString();
            var list = JsonConvert.DeserializeObject<ExceptionTypeProcessList>(jsonData);
            var result = QualityService.ImportExcepProcess(list);
            return result;
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public string DeleteExecProcessAPI(int uuid)
        {
            return QualityService.DeleteExecProcess(uuid);

        }

        [IgnoreDBAuthorize]
        public string AddExecProcessAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<ExceptionTypeFlowChartVM>(data.ToString());
            return QualityService.AddExecProcess(entity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]

        public string DeleteAllExceptionProcessAPI(int FlowChart_Detail_UID)
        {

            return QualityService.DeleteAllExceptionProcess(FlowChart_Detail_UID);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOQCReportConditionsAPI(string processName, string Project)
        {
            var query = QualityService.QueryOQCReportConditions(processName, Project);
            return Ok(query);
        }

        #endregion

        #region ---- QA反推良率分配界面
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQABackToFunPlantInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryQABackToFunPlantInfo(SearchData);
            return Ok(result);
        }


        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQABTFInfoByUIDAPI(string QualityAssurance_DistributeRate_UID)
        {
            var result = QualityService.QueryQABTFInfoByUID(QualityAssurance_DistributeRate_UID);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult SaveBackToFunPlantInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QABackToFunPlantListVM qaMasterData = JsonConvert.DeserializeObject<QABackToFunPlantListVM>(jsonData);
            var result = QualityService.SaveBackToFunPlantInfo(qaMasterData);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult UpdateBackToFunPlantInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QABackToFunPlant qaMasterData = JsonConvert.DeserializeObject<QABackToFunPlant>(jsonData);
            var result = QualityService.UpdateBackToFunPlantInfo(qaMasterData);
            return Ok(result);
        }

        #endregion
    }
}
