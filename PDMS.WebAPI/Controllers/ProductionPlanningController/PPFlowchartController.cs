using Newtonsoft.Json;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Service;
//using PDMS.Service.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    //public class PPFlowchartController : ApiController
    //{
    //    IProductionPlanning ProductiongPlanningService;
    //    IEventReportManagerService EventReportManagerService;
    //    IFlowchartDetailMEService FlowchartDetailMEService;
    //    IFlowchartDetailMEEquipmentService FlowchartDetailMEEquipmentService;
    //    IProductionSchedulNPI ProductionSchedulNPI;
    //    IProductionSchedulService ProductionSchedulService;
    //    ICurrentStaffService CurrentStaffService;

    //    public PPFlowchartController(IProductionPlanning ProductiongPlanningService,
    //        IFlowchartDetailMEService FlowchartDetailMEService,
    //        IFlowchartDetailMEEquipmentService FlowchartDetailMEEquipmentService,
    //        IEventReportManagerService EventReportManagerService,
    //        IProductionSchedulNPI ProductionSchedulNPI,
    //        IProductionSchedulService ProductionSchedulService,
    //        ICurrentStaffService CurrentStaffService
    //        )
    //    {
    //        this.ProductiongPlanningService = ProductiongPlanningService;
    //        this.FlowchartDetailMEService = FlowchartDetailMEService;
    //        this.FlowchartDetailMEEquipmentService = FlowchartDetailMEEquipmentService;
    //        this.EventReportManagerService = EventReportManagerService;
    //        this.ProductionSchedulNPI = ProductionSchedulNPI;
    //        this.ProductionSchedulService = ProductionSchedulService;
    //        this.CurrentStaffService = CurrentStaffService;
    //    }

    //    #region PP Flowchart列表界面
    //    [HttpPost]
    //    public IHttpActionResult QueryPPFlowChartsAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var searchModel = JsonConvert.DeserializeObject<FlowChartModelSearch>(jsonData);
    //        var page = JsonConvert.DeserializeObject<Page>(jsonData);
    //        var flCharts = ProductiongPlanningService.QueryPPFlowCharts(searchModel, page);
    //        return Ok(flCharts);

    //    }
    //    #endregion

    //    #region PP Flowchart明细
    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult FlowchartPPDetailListAPI(int id, int Version)
    //    {
    //        var list = ProductiongPlanningService.FlowchartPPDetailList(id, Version);
    //        return Ok(list);
    //    }

    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult QueryFLDetailAssociationByIDAPI(int id)
    //    {
    //        var list = ProductiongPlanningService.QueryFLDetailAssociationByID(id);
    //        return Ok(list);
    //    }

    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public string CheckProcessAPI(int id, string json)
    //    {
    //        List<int> idList = JsonConvert.DeserializeObject<List<int>>(json);
    //        var result = ProductiongPlanningService.CheckProcess(id, idList);
    //        return result;
    //    }

    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public string QueryFLDetailAPI(int id)
    //    {
    //        var result = ProductiongPlanningService.QueryFLDetail(id);
    //        return string.Empty;
    //    }

    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public string QueryFLProcessCheckedSelectByIDAPI(int id)
    //    {
    //        var result = ProductiongPlanningService.QueryFLProcessCheckedSelectByID(id);
    //        var json = JsonConvert.SerializeObject(result);
    //        return json;
    //    }

    //    public void SavePPFlowchartAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        FlowChartDetailDTO dto = JsonConvert.DeserializeObject<FlowChartDetailDTO>(jsonData);

    //        List<int> idList = JsonConvert.DeserializeObject<List<int>>(dto.hidJson);
    //        ProductiongPlanningService.SavePPFlowchart(dto,idList);
    //    }
    //    #endregion

    //    #region MP 生产计划维护
    //    public IHttpActionResult QueryProductionSchedulMPAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        FlowchartMeNPI npiModel = JsonConvert.DeserializeObject<FlowchartMeNPI>(jsonData);
    //        Page page = JsonConvert.DeserializeObject<Page>(jsonData);
    //        var list = ProductionSchedulService.QueryProductionSchedulMP(npiModel, page);
    //        return Ok(list);
    //    }

    //    [IgnoreDBAuthorize]
    //    [HttpGet]
    //    public IHttpActionResult DownloadMPExcelAPI(int id, int Version)
    //    {
    //        var list = ProductionSchedulService.DownloadMPExcel(id, Version);
    //        return Ok(list);

    //    }

    //    [IgnoreDBAuthorize]
    //    [HttpGet]
    //    public string CheckDownloadMPExcelAPI(int id, int Version)
    //    {
    //        var result = ProductionSchedulService.CheckDownloadMPExcel(id, Version);
    //        return result;
    //    }

    //    [IgnoreDBAuthorize]
    //    public string CheckImportMPExcelAPI(dynamic json)
    //    {
    //        List<ProductionSchedulDTO> dtoList = JsonConvert.DeserializeObject<List<ProductionSchedulDTO>>(json.ToString());
    //        var result = ProductionSchedulService.CheckImportMPExcel(dtoList);
    //        return result;

    //    }

    //    public string ImportMPExcelAPI(dynamic json)
    //    {
    //        List<ProductionSchedulDTO> dtoList = JsonConvert.DeserializeObject<List<ProductionSchedulDTO>>(json.ToString());
    //        var result = ProductionSchedulService.ImportMPExcelAPI(dtoList);
    //        return result;
    //    }

    //    [IgnoreDBAuthorize]
    //    [HttpGet]
    //    public string DeleteInfoByUIDListAPI(string json)
    //    {
    //        List<int> idList = JsonConvert.DeserializeObject<List<int>>(json);
    //        var result = ProductionSchedulService.DeleteInfoByUIDListAPI(idList);
    //        return result;
    //    }

    //    public string SaveMPInfoAPI(dynamic json)
    //    {
    //        var item = json.ToString();
    //        ProductionSchedulDTO dto = JsonConvert.DeserializeObject<ProductionSchedulDTO>(item);
    //        ProductionSchedulService.SaveMPInfo(dto);
    //        return string.Empty;
    //    }

    //    #endregion

    //    #region 现有人力管理
    //    public IHttpActionResult QueryCurrentStaffInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var searchModel = JsonConvert.DeserializeObject<CurrentStaffDTO>(jsonData);
    //        var page = JsonConvert.DeserializeObject<Page>(jsonData);
    //        var list = CurrentStaffService.QueryCurrentStaffInfo(searchModel, page);
    //        return Ok(list);
    //    }

    //    public string ImportCurrentStaffInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<CurrentStaffDTO>>(jsonData);
    //        var result = CurrentStaffService.ImportCurrentStaffInfo(list);
    //        return result;
    //    }


    //    public string CheckImportCurrentStaffExcelAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<CurrentStaffDTO>>(jsonData);
    //        var result = CurrentStaffService.CheckImportCurrentStaffExcel(list);
    //        return result;
    //    }

    //    public string SaveStaffInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var item = JsonConvert.DeserializeObject<CurrentStaffDTO>(jsonData);
    //        var result = CurrentStaffService.SaveStaffInfo(item);
    //        return result;
    //    }
    //    #endregion

    //    #region 离职率/排班计划维护
    //    public IHttpActionResult QueryTurnoverSchedulingInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var searchModel = JsonConvert.DeserializeObject<DemissionRateAndWorkScheduleDTO>(jsonData);
    //        var page = JsonConvert.DeserializeObject<Page>(jsonData);
    //        var list = CurrentStaffService.QueryTurnoverSchedulingInfo(searchModel, page);
    //        return Ok(list);
    //    }

    //    public string SaveDemissionInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var searchModel = JsonConvert.DeserializeObject<DemissionRateAndWorkScheduleDTO>(jsonData);
    //        var result = CurrentStaffService.SaveDemissionInfo(searchModel);
    //        return result;
    //    }

    //    [IgnoreDBAuthorize]
    //    public string CheckImportTurnoverExcelAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<DemissionRateAndWorkScheduleDTO>>(jsonData);
    //        var result = CurrentStaffService.CheckImportTurnoverExcel(list);
    //        return result;
    //    }

    //    public string ImportTurnoverExcelAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<DemissionRateAndWorkScheduleDTO>>(jsonData);
    //        var result = CurrentStaffService.ImportTurnoverExcel(list);
    //        return result;
    //    }
    //    #endregion

    //    #region 实际人力机台使用数据录入
    //    public IHttpActionResult ActualPowerInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var searchModel = JsonConvert.DeserializeObject<ActiveManPowerSearchVM>(jsonData);
    //        var page = JsonConvert.DeserializeObject<Page>(jsonData);
    //        var result = CurrentStaffService.ActualPowerInfo(searchModel, page);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult CheckDownloadManPowerExcelAPI(int id, int Version)
    //    {
    //        var result = CurrentStaffService.CheckDownloadManPowerExcel(id, Version);
    //        return Ok(result);
    //    }

        
    //    [HttpGet]
    //    public IHttpActionResult GetManPowerDownLoadInfoAPI(int id, int Version)
    //    {
    //        var result = CurrentStaffService.GetManPowerDownLoadInfo(id, Version);
    //        return Ok(result);
    //    }

    //    public string CheckImportManPowerAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<ActiveManPowerVM>>(jsonData);
    //        var result = CurrentStaffService.CheckImportManPower(list);
    //        return result;

    //    }

    //    public string ImportManPowerAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<ActiveManPowerVM>>(jsonData);
    //        var result = CurrentStaffService.ImportManPower(list);
    //        return result;
    //    }


    //    public string SaveActualPowerInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var item = JsonConvert.DeserializeObject<ProductRequestStaffDTO>(jsonData);
    //        var result = CurrentStaffService.SaveActualPowerInfo(item);
    //        return string.Empty;
    //    }

    //    #endregion


    //    #region 实际机台数录入
    //    public IHttpActionResult EquipInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var searchModel = JsonConvert.DeserializeObject<ActiveManPowerSearchVM>(jsonData);
    //        var page = JsonConvert.DeserializeObject<Page>(jsonData);
    //        var result = CurrentStaffService.EquipInfo(searchModel, page);
    //        return Ok(result);
    //    }


    //    [HttpGet]
    //    public IHttpActionResult GetEquipDownLoadInfoAPI(int id, int Version)
    //    {
    //        var result = CurrentStaffService.GetEquipDownLoadInfo(id, Version);
    //        return Ok(result);
    //    }


    //    public string CheckImportEquipAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<ProductEquipmentQTYDTO>>(jsonData);
    //        var result = CurrentStaffService.CheckImportEquip(list);
    //        return result;
    //    }


    //    public string ImportEquipAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var list = JsonConvert.DeserializeObject<List<ProductEquipmentQTYDTO>>(jsonData);
    //        var result = CurrentStaffService.ImportEquip(list);
    //        return result;
    //    }

    //    public string SaveActualEquipInfoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var item = JsonConvert.DeserializeObject<ProductEquipmentQTYDTO>(jsonData);
    //        var result = CurrentStaffService.SaveActualEquipInfo(item);
    //        return string.Empty;
    //    }

        

    //    #endregion

    //}
}
