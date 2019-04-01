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
using Microsoft.Ajax.Utilities;
using System.Linq;
using System.Web.Script.Serialization;

namespace PDMS.Web.Controllers
{
    public class ProductInputController : WebControllerBase
    {
        // GET: ProductInput
        public ActionResult ProductData(int flowChartMaster_Uid, string product_Date, string time_Interval)
        {

            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            int iswx = 0;
            if (CurrentUser.GetUserInfo.MH_Flag)
            {
                string url = string.Format("FlowChart/QueryPlantByUser?userid={0}",CurrentUser.AccountUId);
                HttpResponseMessage responseMessage2 = APIHelper.APIGetAsync(url);
                var result1 = responseMessage2.Content.ReadAsStringAsync().Result;
                var entity1 = JsonConvert.DeserializeObject<List<string>>(result1);
                foreach (string item in entity1)
                {
                    if (item== "WUXI_M")
                        iswx = 1;
                }
            }
            else
            {
                foreach (OrganiztionVM item in userinfo)
                {
                    if (item.Plant == "WUXI_M")
                    {
                        iswx = 1;
                    }
                }
            }
            ViewBag.IsWX = iswx;
            ProcessDataSearch search = new ProcessDataSearch();
            string apiUrl = string.Format("FlowChart/QueryFlowChartDataByMasterUid?flowChartMaster_uid={0}", flowChartMaster_Uid);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<ProcessDataSearch>(result);

            //检查flowchartDetail是否有Repair和Rework---------------start
            apiUrl = string.Format("FlowChart/GetMaxDetailInfoAPI?flowChartMaster_uid={0}", flowChartMaster_Uid);
            responseMessage = APIHelper.APIGetAsync(apiUrl);
            result = responseMessage.Content.ReadAsStringAsync().Result;
            var detailList = JsonConvert.DeserializeObject<List<FlowChartDetailDTO>>(result);
            var hasReworkExist = detailList.Exists(m => m.Rework_Flag == "Rework");
            var hasRepairExist = detailList.Exists(m => m.Rework_Flag == "Repair");
            if (hasRepairExist && hasRepairExist)
            {
                ViewBag.ReworkFlag = true;
            }
            else
            {
                ViewBag.ReworkFlag = false;
            }
            //检查flowchartDetail是否有Repair和Rework---------------end
            //根据专案UID获取对应的OP
            
          var  apiUrl1 = string.Format("EventReportManager/GetOPByFlowchartMasterUIDAPI?masterUID={0}", flowChartMaster_Uid);
           var responseMessage1 = APIHelper.APIGetAsync(apiUrl1);
           var  OP = JsonConvert.DeserializeObject<string>(responseMessage1.Content.ReadAsStringAsync().Result) ;

            ViewBag.OP = OP;
            apiUrl = string.Format("EventReportManager/GetIntervalInfoAPI?opType={0}", OP);
            responseMessage = APIHelper.APIGetAsync(apiUrl);
            result = responseMessage.Content.ReadAsStringAsync().Result;
            var time = JsonConvert.DeserializeObject<IntervalEnum>(result);

            search.Date = DateTime.Parse(product_Date.IsNullOrWhiteSpace() ? time.NowDate : product_Date);
            search.Time = time_Interval.IsNullOrWhiteSpace() ? time.Time_Interval : time_Interval;
            search.Customer = entity.Customer;
            search.Project = entity.Project;
            search.Part_Types = entity.Part_Types;
            search.Product_Phase = entity.Product_Phase;
            search.QuertFlag = "ProjectList";
            search.Func_Plant = entity.Func_Plant;
            search.FlowChart_Master_UID = entity.FlowChart_Master_UID;
            search.FlowChart_Version = entity.FlowChart_Version;

            TempData["ProcessDataSearch"] = search;
            ViewBag.Product_Date = time.NowDate;
            ViewBag.Time_Interval = time.Time_Interval;
            return View();
        }


        public ActionResult ProductDataInputForEmergency()
        {
          
            return View();
        }

        /// <summary>
        /// 根据查询条件获取制程信息
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryProcessData(ProcessDataSearch search, Page page)
        {
            var apiUrl = string.Format("ProductInput/QueryProcessAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        #region Modify By Rock 2016-05-10----------------------合并版本

        public ActionResult QueryProductDatas(Page page)
        {
            ProcessDataSearch search = (ProcessDataSearch)TempData["ProcessDataSearch"];
            search.Date = DateTime.Parse(search.Date.ToShortDateString());

            page.PageSize = 10;

            var apiUrl1 = string.Format("ProductInput/QueryProductsAPI");

            var current = this.CurrentUser.AccountUId;
            search.Account_UID = current;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl1);
            var result1 = responMessage.Content.ReadAsStringAsync().Result;

            var list = JsonConvert.DeserializeObject<PagedListModel<ProductDataVM>>(result1).Items.ToList();
            var isRepair = list.Exists(m => m.Rework_Flag == "Repair");
            if (isRepair)
            {
                ViewBag.isRepair = true;
            }
            return Content(result1, "application/json"); 
        }

        public ActionResult QueryProductDataBackUp(ProcessDataSearchModel search,Page page)
        {

            if (String.IsNullOrWhiteSpace(search.Project)) return  Content("", "application/json");
            search.Date = DateTime.Parse(search.Date.ToShortDateString());

            //page.PageSize = 10;

            var apiUrl1 = string.Format("ProductInput/QueryProductDataBackUpAPI");

   
       
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl1);
            var result1 = responMessage.Content.ReadAsStringAsync().Result;

            var list = JsonConvert.DeserializeObject<PagedListModel<ProductDataVM>>(result1).Items.ToList();
            
            return Content(result1, "application/json");
        }

        #endregion Modify By Rock 2016-05-10----------------------end

        /// <summary>
        /// 用户warringList 和Project List那里调用，查询展现生产数据
        /// </summary>
        /// <returns></returns>
        public ActionResult QuestProductDatas()
        {
            Page page = new Page();
            page.PageSize = 10;
            if (TempData["ProcessDataSearch"] == null)
            {
                return View("ProductData");
            }
            //使用tempdata 获取传过来的查询条件
            ProcessDataSearch search = (ProcessDataSearch)TempData["ProcessDataSearch"];

            ViewBag.Func_Plant = search.Func_Plant;
            ViewBag.Customer = search.Customer;
            ViewBag.Project = search.Project;
            ViewBag.Part_Types = search.Part_Types;
            ViewBag.Product_Phase = search.Product_Phase;
            ViewBag.Date = search.Date;
            ViewBag.Time = search.Time;
            ViewBag.Flag = search.QuertFlag;
            return View("ProductData");
        }

        /// <summary>
        /// 单条数据修改product数据
        /// </summary>
        /// <param name="jsonWithProduct"></param>
        /// <returns></returns>
        public string ModifyProductData(string jsonWithProduct)
        {

       

            //var entity = JsonConvert.DeserializeObject<ProductData_Edit>(jsonWithProduct);
            var entity = JsonConvert.DeserializeObject<ProductDataList>(jsonWithProduct);
            var apiUrl = string.Format("ProductInput/ModifyProductDataAPI?AccountId={0}&&flag={1}", this.CurrentUser.AccountUId,entity.ProductLists[0].Location_Flag);
            //entity.Modified_UID = this.CurrentUser.AccountUId;
            //entity.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity.ProductLists.First(), apiUrl);
            var result1= responMessage.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = js.Deserialize<string>(result1);
          
            return result;
        }

        public string ModifyProductDataRorEmergency(string jsonWithProduct)
        {

            var apiUrl = string.Format("ProductInput/ModifyProductDataEmergencyAPI?AccountId={0}", this.CurrentUser.AccountUId);

            var entity = JsonConvert.DeserializeObject<ProductDataItem>(jsonWithProduct);
          // var entity = JsonConvert.DeserializeObject<ProductDataList>(jsonWithProduct);

            //entity.Modified_UID = this.CurrentUser.AccountUId;
            //entity.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result1 = responMessage.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = js.Deserialize<string>(result1);

            return result;
        }

        /// <summary>
        /// 批量提交保存用户输入的战情报表数据
        /// </summary>
        /// <param name="jsonWithProduct"></param>
        /// <returns></returns>
        public ActionResult SaveDatas(string jsonWithProduct)
        {

            var apiUrl = "ProductInput/SaveDatasAPI";
            var entity = JsonConvert.DeserializeObject<ProductDataList>(jsonWithProduct);
            foreach (ProductDataItem item in entity.ProductLists)
            {
                item.Creator_UID = this.CurrentUser.AccountUId;
                item.Create_Date = DateTime.Now;
                item.Modified_UID = this.CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //var result = string.Empty;
            return Content(result, "application/json");
        }

        public ActionResult YieldChartDatas(string jsonWithProduct)
        {

            var apiUrl = "ProductInput/YieldChartDatasAPI";
            var entity = JsonConvert.DeserializeObject<YieldChartSearch>(jsonWithProduct);


            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// 根据UID查询特定的生产数据，用于修改数据时候
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ActionResult QueryProductData(int uuid ,string flagStr)
        {
            bool flag = false;
            if(flagStr== "true")
            {
                flag = true;
            }
            var apiUrl = string.Format("ProductInput/QueryProductDataAPI?uuid={0}&&flag={1}", uuid, flag);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 根据当前登录人信息，获取所在功能厂名字，这里主要是在Project List中调用。
        /// </summary>
        /// <returns></returns>
        public ActionResult getCurrentPlantName()
        {
            int uid = this.CurrentUser.AccountUId;
            var apiUrl = string.Format("ProductInput/getCurrentPlantNameAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 通过功能厂名获取功能厂信息  查询功能厂表，将功能厂对象返回
        /// </summary>
        /// <param name="funcPlant"></param>
        /// <returns></returns>
        public ActionResult QueryFuncPlantInfo(string funcPlant)
        {
            var apiUrl = string.Format("ProductInput/QueryFuncPlantInfoAPI?funcPlant={0}", funcPlant);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// FillZeroProductData--------------------------Sidney 为未填写数据的FunPlant填充0值
        /// </summary>
        /// <returns></returns>
        public ActionResult FillZeroProductData(string jsonWithFunPlantInfo)
        {
            string result = string.Empty;
            var apiUrl = "ProductInput/FillZeroProductDataAPI";
            var Creator_UID = this.CurrentUser.AccountUId;
            var Create_Date = DateTime.Now;
            var entity = JsonConvert.DeserializeObject<ZeroFunPlantInfo>(jsonWithFunPlantInfo);
            foreach (var list in entity.ZeroList)
            {
                list.Create_Time = Create_Date;
                list.Create_User = Creator_UID;
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(list, apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result.ToString();
                result = result.Replace("\"", string.Empty);
                //if (result1 != "\"SUCCESS\"")
                //    result = result1;
            }
            return Json(result);
        }
        public ActionResult TestAjax(string jsonWithFunPlantInfo)
        {
            return Content("TEST", "application/json");
        }

        public ActionResult SendEmail()
        {
            try
            {
                //发送电子邮件的SMTP的服务器名称

                WebMail.SmtpServer = "smtp.qq.com";
                //发送端口
                WebMail.SmtpPort = 25;
                //启用SSL（GMAIL需要），其他的都不需要
                WebMail.EnableSsl = true;
                //-----------配置 
                //账户名 
                WebMail.UserName = "1023929186";
                //邮箱名
                WebMail.From = "1023929186@qq.com";
                //密码
                WebMail.Password = "?yang1989";
                //设置默认配置
                WebMail.SmtpUseDefaultCredentials = true;

                WebMail.Send(
                to: "Sidney_Yang@jabil.com", //指定地址
                subject: "测试标题1", //标题
                body: "天天开心" //内容
                  );

                return Content("SUCCESS", "application/json");
            }
            catch (Exception)
            {
                return Content("Fail", "application/json");
            }
        }

        /// <summary>
        /// 获取返工的制程
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public ActionResult GetRepairToReworkProcess(int Detail_UID, int Product_UID, string selectDate, string selectTime)
        {
            var apiUrl = string.Format("ProductInput/GetRepairToReworkProcessAPI?Detail_UID={0}&Product_UID={1}&selectDate={2}&selectTime={3}", Detail_UID, Product_UID, selectDate, selectTime);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetErrorInfo(int productUid, string ErrorType)
        {
            var apiUrl = string.Format("ProductInput/GetErrorInfoAPI?productUid={0}&&ErrorType={1}", productUid, ErrorType);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

      

       
    }
}