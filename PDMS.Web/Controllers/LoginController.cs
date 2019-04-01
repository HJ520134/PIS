using Newtonsoft.Json;
using PDMS.Common.Constants;
using PDMS.Model;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Model.ViewModels.Common;
using System.Linq;

namespace PDMS.Web.Controllers
{
    public class LoginController : Controller
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PISPAuth(string ntid, string eeid, string controllerName, string actionName)
        {    
            using (var client = new HttpClient())
            {
                var apiPath = ConfigurationManager.AppSettings["WebApiPath"].ToString();
                var content = new StringContent(JsonConvert.SerializeObject(new { ntid = ntid }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage responMessage = client.PostAsync(apiPath + "Login/GetUserProfile", content).Result;
                //log.Info(string.Format("PISPAuth - NTID: {0}, Status: {1}", ntid, responMessage.IsSuccessStatusCode));
                if (responMessage.IsSuccessStatusCode)
                {
                    var user = JsonConvert.DeserializeObject<AuthorizedLoginUser>(responMessage.Content.ReadAsStringAsync().Result);
                    FormsAuthentication.SetAuthCookie(user.USER_Ntid, false);
                    // 下一行 … >"<
                    SetLogon(user);
                    // 跳至其他頁面
                    return RedirectToAction(actionName, controllerName);
                }
                else
                    return Content("Not authencated");
            }
        }

        public ActionResult Eboard(string Name)
        {
            LoginUserMoel loginUser = new LoginUserMoel();
            loginUser.UserName = Name;
            loginUser.IsEmployee = 0;
            SignIn(loginUser, "");
            return RedirectToAction("Index", "Board");
        }

        public ActionResult EboardAuto()
        {
            ///PDMSWeb/Board/getShowContent?selectProjects=SH_MP_MP&selectFunplants=Band%2CAssembly1&Part_Types=Band%2CAssembly1&Optype=OP2&PageNumber=3&PageSize=12
            LoginUserMoel loginUser = new LoginUserMoel();

            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.UserSelect);
            loginUser.UserName = cooike["userName"];
            loginUser.IsEmployee = 0;
            SignIn(loginUser, "");
            return RedirectToAction("IndexWithCookie", "Board");
        }

        //废弃
        //[AllowAnonymous]
        //public ActionResult PlayBoardAuto()
        //{
        //    ///PDMSWeb/Board/getShowContent?selectProjects=SH_MP_MP&selectFunplants=Band%2CAssembly1&Part_Types=Band%2CAssembly1&Optype=OP2&PageNumber=3&PageSize=12
        //    LoginUserMoel loginUser = new LoginUserMoel();

        //    var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.PlayBoard);
        //    loginUser.UserName = cooike["userName"];
        //    loginUser.IsEmployee = 0;
        //    SignIn(loginUser, "");
        //    return RedirectToAction("IndexWithCookie", "PlayBoard");
        //}

        [HttpPost]
        public ActionResult SignIn(LoginUserMoel loginUser, string returnUrl)
        {
            HttpResponseMessage responMessage;
            var apiPath = ConfigurationManager.AppSettings["WebApiPath"].ToString();
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(loginUser));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                responMessage = client.PostAsync(apiPath + "Login/LoginIn", content).Result;
            }

            if (responMessage.IsSuccessStatusCode)
            {
                var user = JsonConvert.DeserializeObject<AuthorizedLoginUser>(responMessage.Content.ReadAsStringAsync().Result);

                #region put token/user info into Cookie and Session and SetAuthCookie make Identity true
                if (Request.Cookies[SessionConstants.LoginTicket] != null)
                {
                    CookiesHelper.RemoveCookiesByCookieskey(Request, Response, SessionConstants.LoginTicket);
                    CookiesHelper.RemoveCookiesByCookieskey(Request, Response, SessionConstants.CurrentAccountUID);
                    CookiesHelper.RemoveCookiesByCookieskey(Request, Response, SessionConstants.CurrentUserName);
                }
                else
                {
                    CookiesHelper.AddCookies(Response, SessionConstants.LoginTicket, user.Token, 1);
                    CookiesHelper.AddCookies(Response, SessionConstants.CurrentAccountUID, user.Account_UID.ToString(), 1);
                    CookiesHelper.AddCookies(Response, SessionConstants.CurrentUserName, user.User_Name, 1);
                }


                if (Request.Cookies["APIPath"] == null)
                {
                    CookiesHelper.AddCookies(Response, "APIPath", apiPath, 1);
                }

                FormsAuthentication.SetAuthCookie(loginUser.UserName, false);

                SetLogon(user);

                #endregion

                //get ticket of login user
                //var ticket = FormsAuthentication.Decrypt(user.Token);

                //set principal
                //IIdentity identity = new FormsIdentity(ticket);
                //IPrincipal principal = new GenericPrincipal(identity, null);
                //HttpContext.User = principal;
                if (user.RoleList != null && user.RoleList.Exists(x=>x.Role_ID == "PlayBoardPlayUser"))  //.User_Name.Contains("播放看板")
                {
                    //硬编码的角色Role_ID,这个角色免密码登录，直接显示播放看板
                    //PlayBoardPlayUser 播放看板播放账号
                    return RedirectToAction("Index", "PlayBoard", new { playUserUID= user.Account_UID });
                }
                if (user.User_Name.Contains("电子看板"))
                {
                    return RedirectToAction("Index", "Board");
                }

                //如果用户是物料员则跳转到生成数据维护画面，如果不是则到导航画面
                if (user.MH_Flag)
                {
                    if (user.IsMulitProject)
                    {
                        Session[SessionConstants.MHFlag_MulitProject] = user.IsMulitProject;

                        return RedirectToAction("ProjectList", "FlowChart");
                    }
                    else if (user.USER_Ntid == "EQPUser")
                    {
                        return RedirectToAction("EQPMaintenance", "Equipmentmaintenance", new { iseqp_user = "true" });
                    }
                    else
                    {
                        var master_Uid = user.flowChartMaster_Uid;
                        if (master_Uid == null)
                            return RedirectToAction("Index", "Home");
                        else
                            return RedirectToAction("ProductData", "ProductInput", new { flowChartMaster_Uid = master_Uid });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.InvalidCode = string.Empty;

                switch (responMessage.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                        ViewBag.InvalidCode = "ACCOUNTNOTENABLED";
                        break;
                    case HttpStatusCode.NotFound:
                        ViewBag.InvalidCode = "ACCOUNTNOTEXIST";
                        break;
                    case HttpStatusCode.Unauthorized:
                        ViewBag.InvalidCode = "WRONGPASSWORD";
                        break;
                    case HttpStatusCode.InternalServerError:
                        throw new Exception("API Server Error");
                    default:
                        break;
                }

                return View("Index");
            }
        }

        #region Modify By Rock -------------------------------------------2017/7/18
        private void SetLogon(AuthorizedLoginUser user)
        {
            Session[SessionConstants.LoginTicket] = user.Token;
            Session[SessionConstants.CurrentAccountUID] = user.Account_UID.ToString();
            Session[SessionConstants.CurrentUserName] = user.User_Name;
            Session[SessionConstants.CurrentLanguageId] = user.System_Language_UID.ToString();

            // Session[SessionConstants.CurrentUserInfo] = AAresult;
            if (user.USER_Ntid.ToUpper() != "EQPUSER")
            {
                var userApiUrl = string.Format("Common/GetUserInfo/?uid={0}", user.Account_UID);
                var AAresponMessage = APIHelper.APIGetAsync(userApiUrl);
                var AAresult = JsonConvert.DeserializeObject<CustomUserInfoVM>(AAresponMessage.Content.ReadAsStringAsync().Result);

                Session[SessionConstants.CurrentUserInfo] = AAresult;
                string apiUrl = string.Format("Common/GetLanguagesAPI?Language_UID={0}", user.System_Language_UID);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var languageVM = JsonConvert.DeserializeObject<LanguageVM>(result);

                string userUrl = string.Format("Common/GetUserInfo/?uid={0}", user.Account_UID);
                var userResponMessage = APIHelper.APIGetAsync(userUrl);
                var userResult = userResponMessage.Content.ReadAsStringAsync().Result;

                var userDTO = JsonConvert.DeserializeObject<CustomUserInfoVM>(userResult);

                PISSessionContext.Current.CurrentWorkingLanguage = languageVM.CurrentLanguage;
                PISSessionContext.Current.CurrentUser = userDTO;
                PISSessionContext.Current.Languages = languageVM.Languages;
                var userCookie = new HttpCookie(SessionConstants.UserSelect);
                userCookie[SessionConstants.CurrentAccountUID] = PISSessionContext.Current.CurrentUser.Account_UID.ToString();
                userCookie[SessionConstants.CurrentLanguageId] = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID.ToString();
                userCookie.Expires.AddDays(365);
                HttpContext.Response.SetCookie(userCookie);

                //用于传值给前台JS
                Session[SessionConstants.CurrentLanguageId] = languageVM.CurrentLanguage.System_Language_UID;

            }
            else
            {
                var customUserInfoVM = new CustomUserInfoVM()
                {
                    Account_UID = user.Account_UID,
                    Language_UID = 2,
                    User_NTID = user.USER_Ntid,
                    User_Name = user.User_Name,
                    MH_Flag = user.MH_Flag,
                    LoginToken = user.Token

                };
                Session[SessionConstants.CurrentUserInfo] = customUserInfoVM;
                string apiUrl = string.Format("Common/GetLanguagesAPI?Language_UID={0}", 2);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var languageVM = JsonConvert.DeserializeObject<LanguageVM>(result);
                PISSessionContext.Current.CurrentWorkingLanguage = languageVM.CurrentLanguage;
                PISSessionContext.Current.Languages = languageVM.Languages;
                //用于传值给前台JS
                Session[SessionConstants.CurrentLanguageId] = 2;

                // JsonConvert.DeserializeObject<CustomUserInfoVM>(customUserInfoVM)
            }
        }
        #endregion


        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            #region remove all sessions and token cookie                   
            Session.RemoveAll();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                CookiesHelper.RemoveCookiesByCookieskey(Request, Response, FormsAuthentication.FormsCookieName);
            }
            #endregion

            CacheHelper.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}