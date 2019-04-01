
using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Results;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.ProductionPlanning;

namespace PDMS.WebAPI.Controllers
{
    //public class ProductionPlanningForIEController : ApiControllerBase
    //{
    //    IProductionPlanning ProductiongPlanningService;
    //    public ProductionPlanningForIEController(IProductionPlanning ProductiongPlanningService)
    //    {
    //        this.ProductiongPlanningService = ProductiongPlanningService;
    //    }

    //    #region IE 人力配比明细界面

    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult QueryFlowChartsAPI(int Flowchart_Master_UID,int flag)
    //    {
    //        var result = ProductiongPlanningService.QueryFlowChartForIE(Flowchart_Master_UID,flag);
    //        return Ok(result);
    //    }
    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult QueryFlowChartByRangeNumberAPI(int Flowchart_Detail_ME_UID, int RangeNumber)
    //    {
    //        var result = ProductiongPlanningService.QueryFlowChartByRangeNumber(Flowchart_Detail_ME_UID, RangeNumber);
    //        return Ok(result);
    //    }

    //    [HttpPost]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult SaveMappingDataAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        Flowchart_Detail_IE_VMList qaMasterData = JsonConvert.DeserializeObject<Flowchart_Detail_IE_VMList>(jsonData);
    //        var result = ProductiongPlanningService.SaveMappingData(qaMasterData);
    //        return Ok(result);
    //    }


    //    [HttpPost]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult SaveIEFlowchartDataAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        Flowchart_Detail_ProductionPlanningList qaMasterData = JsonConvert.DeserializeObject<Flowchart_Detail_ProductionPlanningList>(jsonData);
    //        var result = ProductiongPlanningService.SaveIEFlowchartData(qaMasterData);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult QueryFlowChartByFlowchart_Detail_ME_UID(int Flowchart_Detail_ME_UID)
    //    {
    //        var result = ProductiongPlanningService.QueryFlowChartByFlowchart_Detail_ME_UID(Flowchart_Detail_ME_UID);
    //        return Ok(result);
    //    }

    //    [HttpPost]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult UpdateIEFlowchartDataAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        Flowchart_Detail_ProductionPlanningList qaMasterData = JsonConvert.DeserializeObject<Flowchart_Detail_ProductionPlanningList>(jsonData);
    //        var result = ProductiongPlanningService.UpdateIEFlowchartData(qaMasterData);
    //        return Ok(result);
    //    }

    //    [HttpPost]
    //    public IHttpActionResult UpdateFlowchartIEListAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        List<FLowchart_Detail_IE_VM> ieList = JsonConvert.DeserializeObject<List<FLowchart_Detail_IE_VM>>(jsonData);
    //        var result = ProductiongPlanningService.UpdateFlowchartIEList(ieList);
    //        return Ok(result);
    //    }

    //    #endregion


    //    #region IE 人力配比明细界面
    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult GetHumanResourcesSummaryAPI(string strWhere)
    //    {
    //        var result = ProductiongPlanningService.GetHumanResourcesSummary(strWhere);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    [IgnoreDBAuthorize]
    //    public IHttpActionResult GetInputDataForSelectAPI(int project)
    //    {
    //        var result = ProductiongPlanningService.GetInputDataForSelect(project);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetPlanDataByProjectAPI(int site, int op, int project, int PartTypeUID, string begin)
    //    {
    //        var result = ProductiongPlanningService.GetPlanDataByProject(site, op, project, PartTypeUID, begin);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetInputDataByProjectAPI(int op, int project, string begin)
    //    {
    //        var result = ProductiongPlanningService.GetInputDataByProject(op,project,begin);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetHumanResourcesByProjectAPI(int project)
    //    {
    //        var result = ProductiongPlanningService.GetHumanResourcesByProject(project);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetHumanResourcesByFunplantAPI(int project)
    //    {
    //        var result = ProductiongPlanningService.GetHumanResourcesByFunplant(project);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetActualAndEstimateHumanInfoAPI(int project, string beginDate,string endDate, string projects, string seq, string process, string estimate, string actual)
    //    {
    //        var result = ProductiongPlanningService.GetActualAndEstimateHumanInfo(project,beginDate,endDate,  projects,  seq, process,  estimate,  actual);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetActualAndEstimateHumanInfoForProcessAPI(int flowchat, string beginDate, string endDate, string project, string seq, string process, string estimate, string actual)
    //    {
    //        var result = ProductiongPlanningService.GetActualAndEstimateHumanInfoForProcess(flowchat, beginDate,endDate, project, seq, process,  estimate,  actual);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetHumanColumnAPI(int project, string beginDate, string endDate,int flag, string projects, string seq, string process, string estimate, string actual)
    //    {
    //        var result = ProductiongPlanningService.GetHumanColumn(project, beginDate, endDate,flag,  projects,  seq,  process,  estimate,  actual);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetNowHumanByBGAPI(int bgOrgID, DateTime begindate)
    //    {
    //        var result = ProductiongPlanningService.GetNowHumanByBG(bgOrgID, begindate);
    //        return Ok(result);
    //    }

    //    [HttpGet]
    //    public IHttpActionResult GetDemissionRateByBGAPI(int bgOrgID, DateTime begindate)
    //    {
    //        var result = ProductiongPlanningService.GetDemissionRateByBG(bgOrgID, begindate);
    //        return Ok(result);
    //    }

    //    public IHttpActionResult QueryManPowerRequestRPTByProjectAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var vm = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
    //        var result = ProductiongPlanningService.GetAllManPowerProject(vm);
    //        return Ok(result);
    //    }


    //    public IHttpActionResult QueryManPowerRequestRPTAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var vm  = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
    //        var result = ProductiongPlanningService.QueryManPowerRequestRPT(vm);
    //        return Ok(result);
    //    }

    //    public IHttpActionResult QueryManPowerRequestByProjectOneAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var vm = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
    //        var result = ProductiongPlanningService.QueryManPowerRequestByProjectOne(vm);
    //        return Ok(result);
    //    }

    //    public IHttpActionResult QueryManPowerRequestByProjectOneByTwoAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var vm = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
    //        var result = ProductiongPlanningService.QueryManPowerRequestByProjectOneByTwo(vm);
    //        return Ok(result);
    //    }

    //    public IHttpActionResult QueryManPowerRequestByProjectOneByThreeAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var vm = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
    //        var result = ProductiongPlanningService.QueryManPowerRequestByProjectOneByThree(vm);
    //        return Ok(result);
    //    }

    //    public IHttpActionResult QueryManPowerRequestByFuncOneAPI(dynamic data)
    //    {
    //        var jsonData = data.ToString();
    //        var vm = JsonConvert.DeserializeObject<ProductionPlanningReportVM>(jsonData);
    //        var result = ProductiongPlanningService.QueryManPowerRequestByFuncOne(vm);
    //        return Ok(result);
    //    }
    //    #endregion
    //}
}