using PDMS.Common.Enums;
using PDMS.Service;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;

namespace PDMS.Core.Authentication
{
    /// <summary>
    /// 基本验证Attribtue，用以Action的权限处理
    /// </summary>
    public class SPPAPIAuthenticationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 检查用户是否有该Action执行的操作权限
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var needAuthorized = Convert.ToBoolean(ConfigurationManager.AppSettings["AuthenticationSwitch"].ToString().ToUpper().Equals("ON"));
            if (needAuthorized)
            {
                //检验用户ticket信息，用户ticket信息来自调用发起方
                var authorization = actionContext.Request.Headers.Authorization;                

                if ((authorization != null) && (authorization.Parameter != null))
                {
                    //解密用户ticket,并校验用户名密码是否匹配
                    var encryptTicket = authorization.Parameter;
                    var authorized = Validate(actionContext, encryptTicket);

                    switch (authorized)
                    {
                        case EnumAuthorize.Success:
                            base.OnActionExecuting(actionContext);
                            break;

                        case EnumAuthorize.Fail:
                            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                            break;

                        default:
                            actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                            break;
                    }
                }
                else
                {
                    //如果请求Header不包含ticket，则判断是否是匿名调用
                    var attr = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
                    bool isAnonymous = attr.Any(a => a is AllowAnonymousAttribute);

                    //是匿名用户，则继续执行；非匿名用户，抛出“未授权访问”信息
                    if (isAnonymous)
                        base.OnActionExecuting(actionContext);
                    else
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                base.OnActionExecuting(actionContext);
            }
        }

        /// <summary>
        /// 校验用户ticket信息
        /// </summary>
        /// <param name="encryptTicket"></param>
        /// <returns></returns>
        private EnumAuthorize Validate(HttpActionContext actionContext, string encryptTicket)
        {
            //获取到SystemService
            var requestScope = actionContext.Request.GetDependencyScope();
            var service = requestScope.GetService(typeof(ISystemService)) as ISystemService;

            #region 验证Token
            var accountId = 0;
            FormsAuthenticationTicket ticket;

            try
            {
                //将Ticket加密
                ticket = FormsAuthentication.Decrypt(encryptTicket);
                //int.TryParse方法是检查ticket.Name是否能转换为数字，如果能则返回True并将数字给accountId。如果不能则返回False。
                if (!int.TryParse(ticket.Name, out accountId))
                {
                    return EnumAuthorize.TokenInvalid;
                }

                var systemUser = service.GetSystemUserByUId(accountId);
                var tokenAvaliable = systemUser == null ? false : systemUser.LoginToken == encryptTicket;

                if (!tokenAvaliable)
                {
                    return EnumAuthorize.TokenInvalid;
                }
            }
            catch
            {
                return EnumAuthorize.TokenInvalid;
            }
            #endregion

            #region 是否需要做DB的权限卡控
            var isAuthorized = IgnoreDBAuthorize(actionContext);
            #endregion

            #region 如果需要做DB的权限卡控，检验是否已授权
            if (isAuthorized == false)
            {
                var controller = actionContext.ControllerContext.RouteData.Values["controller"].ToString().ToUpper();
                var action = actionContext.ControllerContext.RouteData.Values["action"].ToString().ToUpper();
                isAuthorized = service.HasActionQulification(accountId, string.Format("{0}/{1}", controller, action));
            } 
            #endregion

            return isAuthorized ? EnumAuthorize.Success : EnumAuthorize.Fail;
        }

        /// <summary>
        /// Action方法是否有Ignore attribute
        /// 如果存在，则不需要去DB验证权限
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private bool IgnoreDBAuthorize(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor
                    .GetCustomAttributes<IgnoreDBAuthorizeAttribute>().Any();
        }
    }
}
