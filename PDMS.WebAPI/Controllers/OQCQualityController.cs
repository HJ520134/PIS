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


namespace PDMS.WebAPI.Controllers
{
    public class OQCQualityController : ApiControllerBase
    {
        IQualityService QualityService;

        public OQCQualityController(IQualityService ChartService)
        {
            this.QualityService = ChartService;
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOQCMasterDataAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryOQCMasterData(conditionModel);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOQCNGDetailsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryOQCNGDetails(conditionModel);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOQCReworkDetailsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryOQCReworkDetails(conditionModel);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult SaveQaMasterDataAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<OQCInputData>(jsonData);
            var result = QualityService.SaveOQCData(conditionModel);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult AddTExceptionTypesoFlowChartAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<ExceptionTypesAddToFlowChartVM>(jsonData);
            var result = QualityService.AddTExceptionTypesoFlowChart(conditionModel);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOQCRecordDataAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryOQCRecordData(conditionModel);
            return Ok(result);
        }


        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQADayReportSummeryAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetQAReportOQCDaySummery(SearchData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryQADayReportTopFiveAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetQAReportOQCTypeRank(SearchData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOQCNGRecordAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryOQCNGRecord(conditionModel);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryOQCReworkRecordAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var conditionModel = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryOQCReworkRecord(conditionModel);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult DownloadOQCReportExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryQAOQCReport(SearchData);
            return Ok(result);
        }


        [IgnoreDBAuthorize]
        public IHttpActionResult DownloadOQCInputDataForExportExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.GetOQCInputDataForExportExcel(SearchData);
            return Ok(result);
        }

    }
}
