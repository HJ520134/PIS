using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using PDMS.Service;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using Autofac;
using System.Web.Mvc;
using PDMS.WebAPI.Controllers;

[assembly: OwinStartup(typeof(PDMS.WebAPI.Startup))]
namespace PDMS.WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureHangfire(app);

        }
    }
}
