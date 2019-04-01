using Newtonsoft.Json;
using PDMS.Common.Constants;
using PDMS.Core.Authentication;
using PDMS.Core.Filters;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using PDMS.Common.Helpers;
using PDMS.Model.ViewModels.Common;
using System.Linq;

namespace PDMS.Core.BaseController
{
    [SPPWebAuthorize]
    [UnauthorizedError]
    public abstract class WebControllerBase : Controller
    {
        public CurrentUser CurrentUser { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            //Modify By Rock ---------------------------------------2017/7/18
            if (CurrentUser == null)
            {
                CurrentUser = new CurrentUser();
            }
            if (requestContext.HttpContext.Session[SessionConstants.CurrentUserInfo] == null)
            {
                var url = HttpContext.Request.RawUrl.Split('/').Where(m => !string.IsNullOrEmpty(m)).First();
                //if (HttpContext.Request.RawUrl.Contains("PlayBoard"))
                //{
                //    RedirectToAction("PlayBoardAuto", "Login");
                //}
                //else 
                if (HttpContext.Request.RawUrl.Contains("Board"))  ///PDMSWeb/Board/getShowContent?selectProjects=SH_MP_MP&selectFunplants=Band%2CAssembly1&Part_Types=Band%2CAssembly1&Optype=OP2&PageNumber=3&PageSize=12
                {
                    RedirectToAction("EboardAuto", "Login");
                }
                else { 
                    requestContext.HttpContext.Response.Redirect(string.Format("/{0}", url));
                }
            }
            //Modify By Rock ---------------------------------------2017/7/18
        }
    }

    public class CurrentUser
    {
        public int AccountUId
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.CurrentAccountUID] == null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[SessionConstants.CurrentAccountUID];

                    if (cookie!=null)
                    {
                        HttpContext.Current.Session[SessionConstants.CurrentAccountUID] = cookie.Value;
                    }
                    else
                    {
                        var apiUrl = string.Format("Common/GetSystemUserByNTId/?ntid={0}", HttpContext.Current.User.Identity.Name);
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var result = JsonConvert.DeserializeObject<SystemUserDTO>(responMessage.Content.ReadAsStringAsync().Result);

                        HttpContext.Current.Session[SessionConstants.CurrentAccountUID] = result.Account_UID;
                    }
                }
                return Convert.ToInt32(HttpContext.Current.Session[SessionConstants.CurrentAccountUID]);
            }
        }
        public static string UserName
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.CurrentUserName] == null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[SessionConstants.CurrentUserName];

                    if (cookie != null)
                    {
                        HttpContext.Current.Session[SessionConstants.CurrentUserName] = cookie.Value;
                    }
                    else
                    {
                        var apiUrl = string.Format("Common/GetSystemUserByNTId/?ntid={0}&&islogin={1}", HttpContext.Current.User.Identity.Name,1);
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var result = JsonConvert.DeserializeObject<SystemUserDTO>(responMessage.Content.ReadAsStringAsync().Result);

                        HttpContext.Current.Session[SessionConstants.CurrentUserName] = result.User_Name;
                    }
                }
                return HttpContext.Current.Session[SessionConstants.CurrentUserName].ToString();
            }
        }

        public static string Language_UID
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.CurrentLanguageId] == null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[SessionConstants.CurrentLanguageId];

                    if (cookie != null)
                    {
                        HttpContext.Current.Session[SessionConstants.CurrentLanguageId] = cookie.Value;
                    }
                    else
                    {
                        var apiUrl = string.Format("Common/GetSystemUserByNTId/?ntid={0}&&islogin={1}", HttpContext.Current.User.Identity.Name, 1);
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var result = JsonConvert.DeserializeObject<SystemUserDTO>(responMessage.Content.ReadAsStringAsync().Result);

                        HttpContext.Current.Session[SessionConstants.CurrentLanguageId] = result.System_Language_UID;
                    }
                }
                return HttpContext.Current.Session[SessionConstants.CurrentLanguageId].ToString();
            }
        }



        public IEnumerable<SystemPlantDTO> ValidPlants
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.CurrentUserValidPlants] == null)
                {
                    var apiUrl = string.Format("Common/GetValidPlantsByUserUId/{0}", this.AccountUId);
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = JsonConvert.DeserializeObject<IEnumerable<SystemPlantDTO>>(responMessage.Content.ReadAsStringAsync().Result);

                    HttpContext.Current.Session[SessionConstants.CurrentUserValidPlants] = result;
                }
                return HttpContext.Current.Session[SessionConstants.CurrentUserValidPlants] as IEnumerable<SystemPlantDTO>;
            }
        }

        public IEnumerable<SystemBUMDTO> ValidBUMs
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.CurrentUserValidBUMs] == null)
                {
                    var apiUrl = string.Format("Common/GetValidBUMsByUserUId/{0}", this.AccountUId);
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = JsonConvert.DeserializeObject<IEnumerable<SystemBUMDTO>>(responMessage.Content.ReadAsStringAsync().Result);

                    HttpContext.Current.Session[SessionConstants.CurrentUserValidBUMs] = result;
                }
                return HttpContext.Current.Session[SessionConstants.CurrentUserValidBUMs] as IEnumerable<SystemBUMDTO>;
            }
        }

        public IEnumerable<SystemBUDDTO> ValidBUDs
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.CurrentUserValidBUDs] == null)
                {
                    var apiUrl = string.Format("Common/GetValidBUDsByUserUId/{0}", this.AccountUId);
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = JsonConvert.DeserializeObject<IEnumerable<SystemBUDDTO>>(responMessage.Content.ReadAsStringAsync().Result);

                    HttpContext.Current.Session[SessionConstants.CurrentUserValidBUDs] = result;
                }
                return HttpContext.Current.Session[SessionConstants.CurrentUserValidBUDs] as IEnumerable<SystemBUDDTO>;
            }
        }

        public IEnumerable<SystemOrgDTO> ValidOrgs
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.CurrentUserValidOrgs] == null)
                {
                    var apiUrl = string.Format("Common/GetValidOrgsByUserUId/{0}", this.AccountUId);
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = JsonConvert.DeserializeObject<IEnumerable<SystemOrgDTO>>(responMessage.Content.ReadAsStringAsync().Result);

                    HttpContext.Current.Session[SessionConstants.CurrentUserValidOrgs] = result;
                }
                return HttpContext.Current.Session[SessionConstants.CurrentUserValidOrgs] as IEnumerable<SystemOrgDTO>;
            }
        }

        public IEnumerable<PageUnauthorizedElement> PageUnauthorizedElements
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.PageUnauthorizedElements] == null)
                {
                    var apiUrl = string.Format("System/UnauthorizedElements/?ntid={0}", HttpContext.Current.User.Identity.Name);
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = JsonConvert.DeserializeObject<IEnumerable<PageUnauthorizedElement>>(responMessage.Content.ReadAsStringAsync().Result);

                    HttpContext.Current.Session[SessionConstants.PageUnauthorizedElements] = result;
                }
                return HttpContext.Current.Session[SessionConstants.PageUnauthorizedElements] as IEnumerable<PageUnauthorizedElement>;
            }
        }

        public CurrentUserDataPermission DataPermissions
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.DataPermissions] == null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[SessionConstants.DataPermissions];

                    if (cookie != null)
                    {
                        HttpContext.Current.Session[SessionConstants.DataPermissions] = cookie.Value;
                    }
                    else
                    {
                        var apiUrl = string.Format("Common/GetPermissonsByNTId/?uid={0}", this.AccountUId);
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var result = JsonConvert.DeserializeObject<CurrentUserDataPermission>(responMessage.Content.ReadAsStringAsync().Result);

                        HttpContext.Current.Session[SessionConstants.DataPermissions] = result;
                    }
                }
                return HttpContext.Current.Session[SessionConstants.DataPermissions] as CurrentUserDataPermission; ;
            }
        }

        public CustomUserInfoVM GetUserInfo
        {
            get
            {
                return HttpContext.Current.Session[SessionConstants.CurrentUserInfo] as CustomUserInfoVM;
            }
        }

        public UserOrgInfo UserOrgnizationInfo
        {
            get
            {
                if (HttpContext.Current.Session[SessionConstants.DataPermissions] == null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[SessionConstants.DataPermissions];

                    if (cookie != null)
                    {
                        HttpContext.Current.Session[SessionConstants.DataPermissions] = cookie.Value;
                    }
                    else
                    {
                        var apiUrl = string.Format("Common/GetOrgInfoByUserUId/?Account_UId={0}", this.AccountUId);
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var result = JsonConvert.DeserializeObject<UserOrgInfo>(responMessage.Content.ReadAsStringAsync().Result);
                        HttpContext.Current.Session[SessionConstants.DataPermissions] = result;
                    }
                }
                return HttpContext.Current.Session[SessionConstants.DataPermissions] as UserOrgInfo;
            }
        }

        public string GetLocaleStringResource(int languid, string resourceName)
        {
            if (CacheHelper.Get(resourceName + languid) != null)
            {
                return CacheHelper.Get(resourceName + languid).ToString();
            }
            else
            {
                var apiUrl = string.Format("Common/GetResourceValueByNameAPI?languid={0}&resourceName={1}", languid, resourceName);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject(result).ToString();
                return result;
            }
        }
    }
}
