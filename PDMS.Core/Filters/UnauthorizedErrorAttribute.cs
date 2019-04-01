using System.Web;
using System.Web.Mvc;

namespace PDMS.Core.Filters
{
    public class UnauthorizedErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            var statusCode = new HttpException(null, filterContext.Exception).GetHttpCode();

            switch (statusCode)
            {
                //未授权
                case 401:
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonResult
                            {
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                Data = new
                                {
                                    error = true,
                                    message = "Unauthorized Request"
                                }
                            };
                        }
                        else
                        {
                            filterContext.HttpContext.Response.Redirect("~/Home/UnauthorizedRequest", true);
                        }
                    }
                    break;

                //BadRequest Token失效
                case 400:
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonResult
                            {
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                Data = new
                                {
                                    error = true,
                                    message = "ReLogin"
                                }
                            };
                        }
                        else
                        {
                            filterContext.HttpContext.Response.Redirect("~/Login", true);
                        }
                    }
                    break;

                default:
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonResult
                            {
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                Data = new
                                {
                                    error = true,
                                    message = filterContext.Exception.ToString()
                                }
                            };
                        }
                        else
                        {
                            var controllerName = (string)filterContext.RouteData.Values["controller"];
                            var actionName = (string)filterContext.RouteData.Values["action"];
                            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                            filterContext.Result = new ViewResult
                            {
                                ViewName = View,
                                MasterName = Master,
                                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                                TempData = filterContext.Controller.TempData                               
                            };
                        }
                    }
                    break;
            };

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
