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
    public class QAProductSaleReportController : ApiControllerBase
    {
        IQualityService QualityService;

        public QAProductSaleReportController(IQualityService ChartService)
        {
            this.QualityService = ChartService;
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryProductSaleReportSummery(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryProductSaleReportSummery(SearchData);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryProductSaleReportFunplantDetail(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryProductSaleReportFunplantDetail(SearchData);
            return Ok(result);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryProductSaleReportExceptionTypeDetail(string TypeFatherName, string FunPlant,
            string ProductDate, int FlowChart_Detail_UID, int RateType, string Color, string MeterialType)
        {
            var result = QualityService.QueryProductSaleReportExceptionTypeDetail(TypeFatherName, FunPlant, ProductDate, FlowChart_Detail_UID, RateType, Color, MeterialType);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryTimeIntervalFPYReportAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryTimeIntervalFPYReport(SearchData);
            return Ok(result);
        }


        #region ---- 反推报表


        [IgnoreDBAuthorize]
        public IHttpActionResult QueryDistributeRateReportDetail(dynamic data)
        {
            var jsonData = data.ToString();
            var SearchData = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonData);
            var result = QualityService.QueryDistributeRateReportDetail(SearchData);
            return Ok(result);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryDistributeRateReportExceptionTypeDetail(string TypeFatherName, string FunPlant,
            string ProductDate, int FlowChart_Master_UID, int RateType, string Color, string MeterialType, int OPType_OrganizationUID)
        {
            var result = QualityService.QueryDistributeRateReportExceptionTypeDetail(TypeFatherName, FunPlant, ProductDate, FlowChart_Master_UID, RateType, Color, MeterialType, OPType_OrganizationUID);
            return Ok(result);
        }

        #endregion

    }
}
