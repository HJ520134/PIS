using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class HomeController : WebControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.PageTitle = null;
            return View();
        }

        /// <summary>
        /// Get authorized menus
        /// API Path - SYSTEMSERVICE/GETFUNCTIONSBYUSERUID
        /// </summary>
        /// <returns></returns>
        public ActionResult MenuListPartial()
        {
            List<SystemFunctionDTO> menuModel = null;
            var apiUrl = string.Format("System/GetFunctionsByUserUId?uid={0}&LanguageId={1}", this.CurrentUser.AccountUId,PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var item = responMessage.Content.ReadAsStringAsync().Result;

            menuModel = JsonConvert.DeserializeObject<List<SystemFunctionDTO>>(item);

            return PartialView(menuModel);
        }
        public ActionResult ShowMenus()
        {
            List<SystemFunctionDTO> menuModel = null;
            var apiUrl = string.Format("System/GetFunctionsByUserUId?uid={0}&LanguageId={1}", this.CurrentUser.AccountUId, PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var item = responMessage.Content.ReadAsStringAsync().Result;
            menuModel = JsonConvert.DeserializeObject<List<SystemFunctionDTO>>(item);
            var p = typeof(SystemFunctionDTO).GetProperty("Function_ID");
            var result=menuModel.Select(i => p.GetValue(i).ToString()).ToList();
            var result1 = JsonConvert.SerializeObject(result);
            return Content(result1, "application/json");
        }
        /// <summary>
        /// call by system, get unauthorized elements by page id. fronte-end hide those elements.
        /// </summary>
        /// <param name="pageUrl">page id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PageUnauthorizedElements(string pageUrl)
        {
            var pageUnauthorizedElements = this.CurrentUser.PageUnauthorizedElements.FirstOrDefault(p => p.PageURL == pageUrl);
            if (pageUnauthorizedElements == null)
            {
                return HttpNotFound();
            }
            return Json(pageUnauthorizedElements, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// MargueeUpload 跑馬燈上傳頁面 
        /// 2018/11/14 Steven Add
        /// </summary>
        /// <returns></returns>
        public ActionResult MargueeUpload()
        {
            return View();
        }
        /// <summary>
        /// MargueeUpload 跑馬燈上傳方法
        /// 2018/11/14 Steven Add
        /// </summary>
        /// <returns></returns>
        public ActionResult MargueeFileUpload(System.Web.HttpPostedFileBase[] files)
        {
            var path = Server.MapPath("~/Content/");
            if (files.Count() > 0)
            {
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                foreach (var uploadFile in files)
                {
                    if (uploadFile.ContentLength > 0 && uploadFile != null)
                    {
                        string savePath = path;
                        uploadFile.SaveAs(savePath + uploadFile.FileName);
                    }
                }
            }
            return Content("<script>alert('上傳檔案成功');window.history.back();</script>");
        }


        [AllowAnonymous]
        public ActionResult ComingSoon()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult UnauthorizedRequest()
        {
            return View();
        }

        public ActionResult MenuHome()
        {
            return RedirectToAction("Index");
        }
    }
}