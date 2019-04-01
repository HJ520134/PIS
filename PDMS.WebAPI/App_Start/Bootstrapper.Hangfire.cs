using Autofac;
using Hangfire;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDMS.WebAPI
{
    public static class BootstrapperHangfire
    {
        public static void Launch()
        {
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterType<Tasks.TaskException>();
            builder.RegisterType<Tasks.TaskSyncLineShiftPerfLastShift>();
            builder.RegisterType<Tasks.TaskOEEActionManagement>();
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerBackgroundJob();

            builder.RegisterType<DatabaseFactory>()
                   .As<IDatabaseFactory>()
                   .InstancePerBackgroundJob();

            builder.RegisterAssemblyTypes(typeof(SystemFunctionRepository).Assembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces()
                   .InstancePerBackgroundJob();

            builder.RegisterAssemblyTypes(typeof(SystemService).Assembly)
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .InstancePerBackgroundJob();

            Hangfire.GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());
        }
    }
}