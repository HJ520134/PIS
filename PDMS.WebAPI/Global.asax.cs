using System.Web.Http;
using System.Web.Mvc;

namespace PDMS.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            Bootstrapper.Launch();
            BootstrapperHangfire.Launch();
        }
    }
}
