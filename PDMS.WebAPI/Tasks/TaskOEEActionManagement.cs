using Hangfire;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDMS.WebAPI.Tasks
{
    public class TaskOEEActionManagement
    {
        private readonly IOEE_Service _OEE_Service;

        public TaskOEEActionManagement(IOEE_Service OEE_Service)
        {
            this._OEE_Service = OEE_Service;
        }

        [AutomaticRetry(Attempts = 0)]
        public void UpdateActionStatus(IJobCancellationToken cancellationToken)
        {
            _OEE_Service.UpdateActionStatus();
        }
    }
}