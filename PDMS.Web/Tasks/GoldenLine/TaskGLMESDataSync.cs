using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDMS.Web.Tasks
{
    public class TaskGLMESDataSync : IPISTaskBase
    {
        [AutomaticRetry(Attempts = 0)]
        public static void ReceiceDataFromLocalDB(IJobCancellationToken cancellationToken)
        {

        }
    }
}