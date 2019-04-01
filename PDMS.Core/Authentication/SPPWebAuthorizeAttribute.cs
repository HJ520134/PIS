using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using PDMS.Common.Enums;
using System;
using PDMS.Model;
using Newtonsoft.Json;
using PDMS.Common.Constants;
using System.Collections.Generic;
using PDMS.Model.ViewModels;
using PDMS.Core.BaseController;
using PDMS.Common.Helpers;
using PDMS.Data;
using PDMS.Service;

namespace PDMS.Core.Authentication
{
    public class SPPWebAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            filterContext.Controller.ViewBag.PasswordFlag = "false";
            base.OnAuthorization(filterContext);

            var identity = filterContext.HttpContext.User.Identity;
            if (identity.IsAuthenticated)
            {
                if (!filterContext.IsChildAction && !filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var actionName = filterContext.ActionDescriptor.ActionName.ToUpper();
                    var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper();
                    var url = string.Format("{0}/{1}", controllerName, actionName);
                    var anonymous = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false);
                   

                    if (!anonymous)
                    {
                        //if (string.IsNullOrEmpty(APIHelper.GetUserToken()))
                        //{
                        //    //redirect to login page if cookie and session are invalid
                        //    filterContext.HttpContext.Response.Redirect("~/Login", true);
                        //}
                        //else
                        //{
                            //check page is authenticated
                            var pageUrl = string.Format("System/HasPageQulification/?controller={0}&action={1}", controllerName, actionName);
                            HttpResponseMessage responMessage = APIHelper.APIGetAsync(pageUrl);
                            var message = JsonConvert.DeserializeObject<Message>(responMessage.Content.ReadAsStringAsync().Result);
                            var result = (EnumAuthorize)Enum.Parse(typeof(EnumAuthorize), message.Content);

                            switch (result)
                            {
                                case EnumAuthorize.PageNotAuthorized:
                                    filterContext.HttpContext.Response.Redirect("~/Home/UnauthorizedRequest", true);
                                    break;
                                case EnumAuthorize.PageAuthorized:
                                {
                                    int languageId = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                                    filterContext.Controller.ViewBag.PageID = url;

                                    #region Get pagetitle from session/db
                                    IEnumerable<SystemFunctionDTO> functions;

                                    if (filterContext.RequestContext.HttpContext.Session[SessionConstants.Functions] == null)
                                    {
                                        functions
                                            = JsonConvert.DeserializeObject<IEnumerable<SystemFunctionDTO>>(
                                                APIHelper.APIGetAsync("System/GetSystemValidFunctions").Content.ReadAsStringAsync().Result);
                                        filterContext.RequestContext.HttpContext.Session[SessionConstants.Functions] = functions;
                                    }
                                    else
                                    {
                                        functions = filterContext.RequestContext.HttpContext.Session[SessionConstants.Functions] as IEnumerable<SystemFunctionDTO>;
                                    }

                                    var target = functions.FirstOrDefault(q => q.URL == url && q.System_Language_UID == languageId && q.Table_ColumnName == "Function_Desc");
                                    filterContext.Controller.ViewBag.PageTitle = target == null ? string.Empty : target.ResourceValue;
                                    if (filterContext.RequestContext.HttpContext.Session[SessionConstants.CurrentUserInfo] != null)
                                    {
                                        UserPasswordInfo dto = new UserPasswordInfo();
                                        CurrentUser cu = new CurrentUser();
                                        dto.UserId = PISSessionContext.Current.CurrentUser.Account_UID;
                                        if (cu.GetUserInfo.User_NTID.ToUpper() != "EQPUSER")
                                        {
                                            var changePassword = APIHelper.APIPostAsync(dto, "Settings/CheckAcountAPI").Content.ReadAsStringAsync().Result;
                                            filterContext.Controller.ViewBag.PasswordFlag = changePassword;
                                        }
                                        else
                                        {
                                            filterContext.Controller.ViewBag.PasswordFlag = "false";
                                        }
                                    }
                                    else
                                    {
                                        filterContext.Controller.ViewBag.PasswordFlag = "false";
                                    }
                                    #endregion
                                }
                                    break;
                                case EnumAuthorize.NotPageRequest:
                                    break;
                                default:
                                    break;
                            }
                        }
                   // }
                }
            }
            else
            {
                //Anonymous user, judge if controller allows anonymous access.
                var attr = filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>();
                bool isAnonymous = attr.Any(a => a is AllowAnonymousAttribute);

                if (!isAnonymous)
                {
                    filterContext.HttpContext.Response.Redirect("~/Login", true);
                }
                
            }
        }

    }
}
