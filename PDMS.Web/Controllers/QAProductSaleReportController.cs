using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PDMS.Web.Controllers
{
    public class QAProductSaleReportController : WebControllerBase
    {
        #region ----- 一次/二次良率

        public ActionResult ProSaleReport()
        {
            return View();
        }

        /// <summary>
        /// 全功能厂汇总良率
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryProductSaleReportSummery(QAReportSearchVM search, Page page)
        {
            var apiUrl = string.Format("QAProductSaleReport/QueryProductSaleReportSummery");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 各功能厂汇总
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryProductSaleReportFunplantDetail(QAReportSearchVM search, Page page)
        {
            search.SearchType = 1;
            var apiUrl = string.Format("QAProductSaleReport/QueryProductSaleReportFunplantDetail");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        
        /// <summary>
        /// 制程明细
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryProductSaleReportProcessDetail(QAReportSearchVM search, Page page)
        {
            search.SearchType = 2;
            var apiUrl = string.Format("QAProductSaleReport/QueryProductSaleReportFunplantDetail");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult QueryProductSaleReportExceptionTypeDetail(string TypeFatherName,string FunPlant,
            string ProductDate, int FlowChart_Detail_UID, int RateType,string Color,string MeterialType)
        {
            var apiUrl = string.Format(@"QAProductSaleReport/QueryProductSaleReportExceptionTypeDetail?TypeFatherName={0}&&FunPlant={1}&&ProductDate={2}&&FlowChart_Detail_UID={3}&&RateType={4}&&Color={5}&&MeterialType={6}",
                TypeFatherName, FunPlant, ProductDate, FlowChart_Detail_UID, RateType,Color, MeterialType);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        #endregion

        #region ----- 分时段直通率统计

        public ActionResult TimeIntervalFPYReport()
        {
            return View();
        }

        /// <summary>
        /// 全功能厂汇总良率
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryTimeIntervalFPYReport(QAReportSearchVM search, Page page)
        {
            var apiUrl = string.Format("QAProductSaleReport/QueryTimeIntervalFPYReportAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        #endregion


        #region ----- 产销反推报表

        public ActionResult ProSaleReportDistributeRate()
        {
            return View();
        }


        /// <summary>
        /// 制程明细
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryDistributeRateReportProcessDetail(QAReportSearchVM search, Page page)
        {
            search.SearchType = 1;
            var apiUrl = string.Format("QAProductSaleReport/QueryDistributeRateReportDetail");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult QueryDistributeRateReportExceptionTypeDetail(string TypeFatherName, string FunPlant,
            string ProductDate, int FlowChart_Master_UID, int RateType, string Color, string MeterialType,int OPType_OrganizationUID)
        {
            var apiUrl = string.Format(@"QAProductSaleReport/QueryDistributeRateReportExceptionTypeDetail?TypeFatherName={0}&&FunPlant={1}&&ProductDate={2}&&FlowChart_Master_UID={3}&&RateType={4}&&Color={5}&&MeterialType={6}&&OPType_OrganizationUID={7}",
                TypeFatherName, FunPlant, ProductDate, FlowChart_Master_UID, RateType, Color, MeterialType, OPType_OrganizationUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        

        /// <summary>
        /// 将百分比转换成小数
        /// </summary>
        /// <param name="perc">百分比值，可纯为数值，或都加上%号的表示，
        /// 如：65|65%</param>
        /// <returns></returns>
        public static decimal PerctangleToDecimal(string perc)
        {
            try
            {
                string patt = @"/^(?<num>[\d]{1,})(%?)$/";
                decimal percNum = Decimal.Parse(System.Text.RegularExpressions.Regex.Match(perc, patt).Groups["num"].Value);
                return percNum / (decimal)100;
            }
            catch
            {
                return 1;
            }
        }


        #endregion

    }
}