//using Newtonsoft.Json;
//using PDMS.Core;
//using PDMS.Core.Authentication;
//using PDMS.Model;
//using PDMS.Service;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Web.Http;
//using System.Web.Http.Results;
//using PDMS.Model.ViewModels;
//using PDMS.Model.ViewModels.ProductionPlanning;
//using PDMS.Data;
//using System.Linq;
//using System.Text;
//using PDMS.Common.Constants;
//using System.Data.Entity.SqlServer;
//using PDMS.Service.ProductionPlanning;
//using PDMS.Model.ViewModels.Common;

//namespace PDMS.WebAPI.Controllers
//{
//    public class ProductionPlanningController : ApiControllerBase
//    {
//        IProductionPlanning ProductiongPlanningService;
//        IEventReportManagerService EventReportManagerService;
//        IFlowchartDetailMEService FlowchartDetailMEService;
//        IFlowchartDetailMEEquipmentService FlowchartDetailMEEquipmentService;
//        IProductionSchedulNPI ProductionSchedulNPI;

//        public ProductionPlanningController(IProductionPlanning ProductiongPlanningService,
//            IFlowchartDetailMEService FlowchartDetailMEService,
//            IFlowchartDetailMEEquipmentService FlowchartDetailMEEquipmentService,
//            IEventReportManagerService EventReportManagerService,
//            IProductionSchedulNPI ProductionSchedulNPI
//            )
//        {
//            this.ProductiongPlanningService = ProductiongPlanningService;
//            this.FlowchartDetailMEService = FlowchartDetailMEService;
//            this.FlowchartDetailMEEquipmentService = FlowchartDetailMEEquipmentService;
//            this.EventReportManagerService = EventReportManagerService;
//            this.ProductionSchedulNPI = ProductionSchedulNPI;
//        }

//        #region ME Flowchart列表界面
//        [HttpPost]
//        public IHttpActionResult QueryMEFlowChartsAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var searchModel = JsonConvert.DeserializeObject<FlowChartModelSearch>(jsonData);
//            var page = JsonConvert.DeserializeObject<Page>(jsonData);
//            var flCharts = ProductiongPlanningService.QueryMEFlowCharts(searchModel, page);
//            return Ok(flCharts);
//        }

//        [IgnoreDBAuthorize]
//        public void ProductionPlanningAPI(ProductionPlanningModelGetAPIModel importItem, bool isEdit, int accountID)
//        {
//            ProductiongPlanningService.ImportFlowchartME(importItem, isEdit, accountID);
//        }

//        [HttpGet]
//        [IgnoreDBAuthorize]
//        public string ChangePhaseAPI(int FlowChart_Master_UID)
//        {
//            var result = ProductiongPlanningService.ChangePhase(FlowChart_Master_UID);
//            return result;
//        }

//        [AcceptVerbs("GET")]
//        public bool ClosedFLAPI(int FlowChart_Master_UID, bool isClosed)
//        {
//            var result = ProductiongPlanningService.ClosedFL(FlowChart_Master_UID, isClosed);
//            return result;
//        }

//        #endregion

//        #region Me Flowchart Detail
//        [HttpPost]
//        public IHttpActionResult QueryFLMEDetailListAPI(int id, int Version)
//        {
//            var result = QueryFLDetailList(id, Version);
//            var list = new PagedListModel<FlowchartDetailMEDTO>(0, result);
//            return Ok(list);
//        }


//        public string SaveFLDetailInfoAPI(FlowchartDetailMEDTO dto, int AccountID)
//        {
//            var result = FlowchartDetailMEService.SaveFLDetailMEInfo(dto, AccountID);
//            return result;
//        }
//        #endregion

//        #region  IE Flowchart列表界面
//        [IgnoreDBAuthorize]
//        [HttpGet]
//        public IHttpActionResult JudgeFlowchartAPI(int FlowChart_Master_UID)
//        {
//            var result = ProductiongPlanningService.JudgeFlowchart(FlowChart_Master_UID);
//            return Ok(result);
//        }
//        #endregion


//        #region 查看设备明细
//        [HttpPost]
//        public IHttpActionResult QueryFLMEEquipmentDetailAPI(int id, int Version, int Param)
//        {
//            var list = FlowchartDetailMEEquipmentService.QueryFLMEEquipmentDetail(id, Version, Param);
//            return Ok(list);
//        }


//        public string SaveFLEquipmentInfoAPI(FlowchartDetailMEEquipmentDTO dto, int AccountID)
//        {
//            var result = FlowchartDetailMEEquipmentService.SaveFLEquipmentInfo(dto, AccountID);
//            return result;
//        }

//        #endregion

//        #region NPI生产计划维护
//        [IgnoreDBAuthorize]
//        public IHttpActionResult DownloadNPIExcelAPI(dynamic json)
//        {
//            var jsonData = json.ToString();
//            FlowchartMeNPI npiModel = JsonConvert.DeserializeObject<FlowchartMeNPI>(jsonData);
//            var page = JsonConvert.DeserializeObject<Page>(jsonData);
//            var list = ProductionSchedulNPI.ExportProductionSchedulNPI(npiModel, page);
//            return Ok(list);
//        }

//        [IgnoreDBAuthorize]
//        public string CheckDownloadNPIExcelApi(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var list = JsonConvert.DeserializeObject<List<ProductionSchedulNPIDTO>>(jsonData);
//            var result = ProductionSchedulNPI.CheckDownloadNPIExcel(list);
//            return result;
//        }

//        [IgnoreDBAuthorize]
//        public string ImportNPIExcelAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var list = JsonConvert.DeserializeObject<List<ProductionSchedulNPIDTO>>(jsonData);
//            var result = ProductionSchedulNPI.ImportNPIExcel(list);
//            return result;
//        }


//        [IgnoreDBAuthorize]
//        [HttpGet]
//        public string DeleteInfoByUIDListAPI(string json)
//        {
//            List<int> idList = JsonConvert.DeserializeObject<List<int>>(json);
//            var result = ProductionSchedulNPI.DeleteInfoByUIDList(idList);
//            return result;
//        }

//        public string SaveNPIInfoAPI(ProductionSchedulNPIDTO dto, int AccountID)
//        {
//            var result = ProductionSchedulNPI.SaveNPIInfo(dto, AccountID);
//            return result;
//        }
//        #endregion


//        #region Me Flowchart Detail Sql
//        private List<FlowchartDetailMEDTO> QueryFLDetailList(int id, int Version)
//        {
//            using (var context = new SPPContext())
//            {
//                var systemFunctionPlantList = context.System_Function_Plant.ToList();
//                var list = context.Flowchart_Detail_ME.Where(m => m.FlowChart_Master_UID == id && m.FlowChart_Version == Version).ToList();
//                var dtoList = AutoMapper.Mapper.Map<List<FlowchartDetailMEDTO>>(list);
//                foreach (var dtoItem in dtoList)
//                {
//                    var plantItem = systemFunctionPlantList.Where(m => m.System_FunPlant_UID == dtoItem.System_FunPlant_UID).First();
//                    dtoItem.FunPlant = plantItem.FunPlant;
//                }
//                return dtoList;
//            }
//        }

//        #endregion


//        #region 设备需求报表
//        [HttpPost]
//        [IgnoreDBAuthorize]
//        public IHttpActionResult GetProjectInfoByUserAPI(dynamic data)
//        {
//            var searchModel = JsonConvert.DeserializeObject<CustomUserInfoVM>(data.ToString());
//            var item = ProductiongPlanningService.GetProjectList(searchModel);
//            return Ok(item);
//        }

//        [HttpGet]
//        [IgnoreDBAuthorize]
//        public IHttpActionResult GetOpTypesByPlantNameAPI(string PlantName)
//        {
//            var item = ProductiongPlanningService.GetOpTypesByPlantName(PlantName);
//            return Ok(item);
//        }

//        [HttpGet]
//        [IgnoreDBAuthorize]
//        public IHttpActionResult GetProjectByOpTypeAPI(int OpTypeUID)
//        {
//            var item = ProductiongPlanningService.GetProjectByOpType(OpTypeUID);
//            return Ok(item);
//        }

//        [HttpGet]
//        [IgnoreDBAuthorize]
//        public IHttpActionResult GetPartTypesByProjectAPI(int ProjectUID)
//        {
//            var item = ProductiongPlanningService.GetPartTypesByProject(ProjectUID);
//            return Ok(item);
//        }

//        [HttpGet]
//        [IgnoreDBAuthorize]

//        public IHttpActionResult GetFunPlantByOpTypeAPI(int OpTypeUID)
//        {
//            var item = ProductiongPlanningService.GetFunPlantByOpType(OpTypeUID);
//            return Ok(item);
//        }

//        public IHttpActionResult QueryEquipRPTAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var searchModel = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
//            var page = JsonConvert.DeserializeObject<Page>(jsonData);
//            var list = ProductiongPlanningService.QueryEquipRPT(searchModel, page);
//            return Ok(list);
//        }

//        public IHttpActionResult QueryEquipRPTByOPTypeAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var searchModel = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
//            var page = JsonConvert.DeserializeObject<Page>(jsonData);
//            var list = ProductiongPlanningService.QueryEquipRPTByOPType(searchModel, page);
//            return Ok(list);
//        }

//        [HttpGet]
//        [IgnoreDBAuthorize]
//        public IHttpActionResult GetRPTColumnNameAPI(int PlantUID, int OpTypeUID)
//        {
//            var list = ProductiongPlanningService.GetRPTColumnName(PlantUID, OpTypeUID);
//            return Ok(list);
//        }

//        [HttpGet]
//        [IgnoreDBAuthorize]
//        public IHttpActionResult GetColumn_ByProjectAPI(int PlantUID, int OpTypeUID, int ProjectUID, int PartTypeUID, string StartDate, string EndDate)
//        {
//            ProductionPlanningReportVM vm = new ProductionPlanningReportVM();
//            vm.PlantUID = PlantUID;
//            vm.OpTypeUID = OpTypeUID;
//            vm.ProjectUID = ProjectUID;
//            vm.PartTypeUID = PartTypeUID;
//            vm.StartDate = StartDate;
//            vm.EndDate = EndDate;
//            var list = ProductiongPlanningService.GetColumn_ByProject(vm);
//            return Ok(list);
//        }

//        [IgnoreDBAuthorize]
//        public IHttpActionResult QueryEquipRPTBySingleProjectAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var searchModel = JsonConvert.DeserializeObject<ReportByProject>(jsonData);
//            var page = JsonConvert.DeserializeObject<Page>(jsonData);
//            var list = ProductiongPlanningService.QueryEquipRPTBySingleProject(searchModel, page);
//            return Ok(list);
//        }

//        [IgnoreDBAuthorize]
//        public IHttpActionResult QueryEquipRPTByFuncAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var searchModel = JsonConvert.DeserializeObject<ReportByProject>(jsonData);
//            var list = ProductiongPlanningService.QueryEquipRPTByFunc(searchModel);
//            return Ok(list);

//        }


//        public IHttpActionResult QueryEquipRPT_ByProjectAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var searchModel = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
//            var page = JsonConvert.DeserializeObject<Page>(jsonData);
//            var list = ProductiongPlanningService.QueryEquipRPT_ByProject(searchModel, page);
//            return Ok(list);
//        }
//        #endregion

//        #region 实际和预估对比报表
//        public IHttpActionResult QueryPlanAndActualReportInfoAPI(dynamic data)
//        {
//            var jsonData = data.ToString();
//            var searchModel = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
//            var page = JsonConvert.DeserializeObject<Page>(jsonData);
//            var list = ProductiongPlanningService.QueryPlanAndActualReportInfo(searchModel, page);
//            return Ok(list);
//        }
//        #endregion

//    }
//}
