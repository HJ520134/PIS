using Hangfire;
using PDMS.Core;
using PDMS.Service;
using PDMS.WebAPI.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class TaskManagerController : ApiControllerBase
    {
        IExceptionService exceptionService;
        IGL_LineShiftPerfService glLineService;
        IOEE_Service _OEE_Service;
        public TaskManagerController(IExceptionService exceptionService, IGL_LineShiftPerfService glLineService, IOEE_Service OEE_Service)
        {
            this.exceptionService = exceptionService;
            this.glLineService = glLineService;
            _OEE_Service = OEE_Service;
        }


        [HttpGet]
        public IHttpActionResult ExecuteTask()
        {
            TaskException task = new TaskException(exceptionService);
            RecurringJob.AddOrUpdate(Environment.MachineName+"--ExceptionOrder", () => task.ExecuteEmail("test", null), Cron.MinuteInterval(5), queue: Environment.MachineName.ToLower(CultureInfo.CurrentCulture));

            TaskSyncLineShiftPerfLastShift taskSyncLineShiftPerf = new TaskSyncLineShiftPerfLastShift(glLineService);
            //同步GoldenLine 模组: 专案报表数据
            RecurringJob.AddOrUpdate(Environment.MachineName + "--SyncLineShiftPerf", () => taskSyncLineShiftPerf.SyncLineShiftPerf(null), Cron.MinuteInterval(5), queue: Environment.MachineName.ToLower(CultureInfo.CurrentCulture));
            //RecurringJob.AddOrUpdate("SyncLineShiftPerf", () => taskSyncLineShiftPerf.SyncLineShiftPerf(null), "*/5 * * * *", TimeZoneInfo.Local);
            //同步GoldenLine 模组: 专案报表当前班次的其他班次数据
            RecurringJob.AddOrUpdate(Environment.MachineName + "--SyncLineShiftPerfLastShift", () => taskSyncLineShiftPerf.SyncLineShiftPerfLastShift(null), Cron.MinuteInterval(5), queue: Environment.MachineName.ToLower(CultureInfo.CurrentCulture));
            //RecurringJob.AddOrUpdate("SyncLineShiftPerfLastShift", () => taskSyncLineShiftPerf.SyncLineShiftPerfLastShift(null), "*/5 * * * *", TimeZoneInfo.Local);

            ////同步OEE 改善计划的延迟状态
            TaskOEEActionManagement taskSyncOEEActionStatu = new TaskOEEActionManagement(_OEE_Service);
            RecurringJob.AddOrUpdate(Environment.MachineName + "--SyncOEEActionStatus", () => taskSyncOEEActionStatu.UpdateActionStatus(null), Cron.MinuteInterval(5), queue: Environment.MachineName.ToLower(CultureInfo.CurrentCulture));

            return Ok();
        }
        [HttpGet]
        public IHttpActionResult Test()
        {
            return Ok("ok");
        }

    }
}
