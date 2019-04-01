using Hangfire;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Hangfire.SqlServer;

namespace PDMS.WebAPI
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {


            //if (System.Configuration.ConfigurationManager.ConnectionStrings["SPPContext"] != null)
            //{
            var optionsDB = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = false
            };
            //data source=JUSTIN\SQL2014;initial catalog=PDMS_Test;persist security info=True;user id=sa;password=123;MultipleActiveResultSets=True;
            //data source=CNCTUG0PDMSTST1;initial catalog=PDMS_Test;persist security info=True;user id=justin;password=P@SSw0rd@1234;MultipleActiveResultSets=True;App=EntityFramework&quot;
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"data source=CNCTUG0PDMSTST1;initial catalog=PDMS_Dev;persist security info=True;user id=justin;password=P@SSw0rd@1234;MultipleActiveResultSets=True;App=EntityFramework&quot;");
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            var options = new BackgroundJobServerOptions
            {
                WorkerCount = Environment.ProcessorCount * 5,
                Queues = new[] { Environment.MachineName.ToLower() }
            };

            app.UseHangfireDashboard();
            app.UseHangfireServer(options);
            //}
        }
    }
}