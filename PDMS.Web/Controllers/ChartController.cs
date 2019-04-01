using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using PDMS.Model.ViewModels.Settings;
using System.Web.Helpers;
using System.Net.Mail;
namespace PDMS.Web.Controllers
{
    public class ChartController : Controller
    {
        #region View---------Sidney 2016/01/27
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Yield()
        {
            return View();
        }

        //public ActionResult EBoard_Project()
        //{
        //    var project = new GetProjectModel
        //    {

        //        OpTypes = this.CurrentUser.DataPermissions.Op_Types,
        //        Project_UID = this.CurrentUser.DataPermissions.Project_UID
        //    };
        //    var apiUrl = string.Format("EventReportManager/GetProjectSourceQAAPI");
        //    HttpResponseMessage responMessage = APIHelper.APIPostAsync(project, apiUrl);
        //    ViewBag.Projects = responMessage.Content.ReadAsStringAsync().Result;
        //    if (CurrentUser.DataPermissions.Op_Types.Count!=0)
        //    {
        //        ViewBag.Opty = CurrentUser.DataPermissions.Op_Types[0];
        //    }
        //    else ViewBag.Opty = null;
        //    return View();
        //}
        #endregion
        #region Common Function-------Sidney 2015/12/20
        /// <summary>
        /// GetCustomerSource
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomerSource()
        {
            var apiUrl = "EventReportManager/GetCustomerSourceAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetProjectSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public ActionResult GetProjectSource(string CustomerName)
        {
            var apiUrl = string.Format("EventReportManager/GetProjectSourceAPI?customer={0}", CustomerName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetProductPhaseSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public ActionResult GetProductPhaseSource(string CustomerName, string ProjectName)
        {
            var apiUrl = string.Format("EventReportManager/GetProductPhaseSourceAPI?customer={0}&&project={1}", CustomerName, ProjectName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetPartTypesSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <returns></returns>
        public ActionResult GetPartTypesSource(string CustomerName, string ProjectName, string ProductPhaseName)
        {
            var apiUrl = string.Format("EventReportManager/GetPartTypesSourceAPI?customer={0}&&project={1}&&productphase={2}", CustomerName, ProjectName, ProductPhaseName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetColorSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <param name="PartTypesName"></param>
        /// <returns></returns>
        public ActionResult GetColorSource(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName)
        {
            var apiUrl = string.Format("EventReportManager/GetColorSourceAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}", CustomerName, ProjectName, ProductPhaseName, PartTypesName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取当前或所有时段
        /// </summary>
        /// <param name="nowOrAllInterval">可传入两个参数：当前时段=PPCheckData，所有时段=DataReport </param>
        /// <returns></returns>
        public ActionResult GetIntervalTime(string nowOrAllInterval)
        {
            var apiUrl = string.Format("EventReportManager/GetIntervalTimeAPI?PageName={0}", nowOrAllInterval);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取所有FunPlant，外加ALL选项
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <param name="PartTypesName"></param>
        /// <param name="Color"></param>
        /// <returns></returns>
        public ActionResult GetFunPlant(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color)
        {
            var apiUrl = string.Format("Chart/GetFunPlantAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&color={4}", CustomerName, ProjectName, ProductPhaseName, PartTypesName,Color);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取功能厂下所有制程，当FunPlant=ALL则获取所有制程
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <param name="PartTypesName"></param>
        /// <param name="Color"></param>
        /// <param name="FunPlant"></param>
        /// <returns></returns>
        public ActionResult GetProcess(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color,string FunPlant)
        {
            var apiUrl = string.Format("Chart/GetProcessAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&color={4}&&funplant", CustomerName, ProjectName, ProductPhaseName, PartTypesName, Color, FunPlant);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
        #region Get Product Data-------Sidney 2016/01/28
        public ActionResult QuerySumProductData(ChartSearch search, Page page)
        {
            var apiUrl = string.Format("Chart/QuerySumProductDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
    }
}