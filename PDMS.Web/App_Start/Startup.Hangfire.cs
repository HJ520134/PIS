using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Hangfire;

namespace PDMS.Web
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["PISDBConnection"] != null)
            {
                GlobalConfiguration.Configuration.UseSqlServerStorage("PISDBConnection");
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
                var options = new BackgroundJobServerOptions
                {
                    WorkerCount = Environment.ProcessorCount * 5,
                    Queues = new[] { "critical", "default" }
                };

                app.UseHangfireDashboard();
                //app.UseHangfireServer(options);
            }
        }
    }
}