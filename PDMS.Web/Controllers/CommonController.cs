using PDMS.Core;
using PDMS.Core.BaseController;
using System.Net.Http;
using System.Web.Mvc;
using System.Net;
using PDMS.Model.ViewModels.Common;
using System.Linq;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Service.Language;

namespace PDMS.Web.Controllers
{
    public class CommonController : WebControllerBase
    {
        #region System User
        /// <summary>
        /// 根据Account UID获取用户
        /// </summary>
        /// <param name="Account_UID">Account UID</param>
        /// <returns>null or entity json</returns>
        public ActionResult GetSystemUserByUId(int Account_UID)
        {
            var apiUrl = string.Format("Common/GetSystemUserByUId/{0}", Account_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.StatusCode == HttpStatusCode.NotFound ? "null"
                            : responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// 根据NTID获取用户
        /// </summary>
        /// <param name="User_NTID">NTID</param>
        /// <returns>null or entity json</returns>
        public ActionResult GetSystemUserByNTId(string User_NTID)
        {
            var apiUrl = string.Format("Common/GetSystemUserByNTId/?ntid={0}&islogin=0", User_NTID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.StatusCode == HttpStatusCode.NotFound ? "null"
                            : responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        #endregion //System User

        [HttpGet]
        public ActionResult GetValidPlantsByUserUId(int Account_UID)
        {
            var apiUrl = string.Format("Common/GetValidPlantsByUserUId/{0}",Account_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        [HttpGet]
        public ActionResult GetValidBUMsByUserUId(int Account_UID)
        {
            var apiUrl = string.Format("Common/GetValidBUMsByUserUId/{0}", Account_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        [HttpGet]
        public ActionResult GetValidBUDsByUserUId(int Account_UID)
        {
            var apiUrl = string.Format("Common/GetValidBUDsByUserUId/{0}", Account_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        [HttpGet]
        public ActionResult GetValidOrgsByUserUId(int Account_UID)
        {
            var apiUrl = string.Format("Common/GetValidOrgsByUserUId/{0}", Account_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public string GetSelectLanguageId()
        {
            if (CacheHelper.Get(SessionConstants.LanguageUIDByUser) != null)
            {
                return CacheHelper.Get(SessionConstants.LanguageUIDByUser).ToString();
            }
            else
            {
                var apiUrl = string.Format("Common/GetSelectLanguageId?accountId={0}", this.CurrentUser.AccountUId);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;

                result = result.Replace("\"", "");
                CacheHelper.Set(SessionConstants.LanguageUIDByUser, result);
                return result;
            }
        }

        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            LanguageVM vm = new LanguageVM();
            vm.CurrentLanguage = PISSessionContext.Current.CurrentWorkingLanguage;
            vm.Languages = PISSessionContext.Current.Languages;
            return PartialView(vm);
        }

        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var languages = PISSessionContext.Current.Languages;
            PISSessionContext.Current.CurrentWorkingLanguage = languages.Where(m => m.System_Language_UID.Equals(langid)).First();
            CacheHelper.Set(SessionConstants.LanguageUIDByUser, langid.ToString());
            //用于传值给前台JS
            Session[SessionConstants.CurrentLanguageId] = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            //更新用户多语言信息
            var apiUrl = string.Format("Common/ChangeLanguageInfoAPI?accountId={0}&languid={1}", this.CurrentUser.AccountUId,PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Redirect(returnUrl);
        }

    }
}