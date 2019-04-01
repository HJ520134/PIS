using Hangfire;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDMS.WebAPI.Tasks
{
    /// <summary>
    /// Golden Line 模组，为专案报表上一班次矫正数据
    /// </summary>
    public class TaskSyncLineShiftPerfLastShift
    {
        private readonly IGL_LineShiftPerfService lineShiftPerfService;

        public TaskSyncLineShiftPerfLastShift(IGL_LineShiftPerfService lineShiftPerfService)
        {

            this.lineShiftPerfService = lineShiftPerfService;
        }

        [AutomaticRetry(Attempts = 0)]
        public void SyncLineShiftPerf(IJobCancellationToken cancellationToken)
        {
            lineShiftPerfService.SyncLineShiftPerf();
        }

        [AutomaticRetry(Attempts = 0)]
        public void SyncLineShiftPerfLastShift(IJobCancellationToken cancellationToken)
        {
            lineShiftPerfService.SyncLineShiftPerfLastShift();
        }
    }
}