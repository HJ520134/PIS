using Hangfire;
using PDMS.Core;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDMS.WebAPI.Tasks
{
    public class TaskException
    {

        private readonly IExceptionService exceptionService;

        public TaskException(IExceptionService exceptionService)
        {

            this.exceptionService = exceptionService;
        }

        [AutomaticRetry(Attempts = 0)]
        public void ExecuteEmail(string keyword,IJobCancellationToken cancellationToken)
        {
            exceptionService.ExceptionShedule();
            //int count = result.Count;
        }
    }
}