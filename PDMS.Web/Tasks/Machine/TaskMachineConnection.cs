using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDMS.Web.Tasks
{
    public class TaskMachineConnection : IPISTaskBase
    {
        [AutomaticRetry(Attempts = 0)]
        public static void DeviceInfoGet(IJobCancellationToken cancellationToken)
        {
            // step 1
            cancellationToken.ThrowIfCancellationRequested();
            // running step 1 ... long time

            // step 2
            cancellationToken.ThrowIfCancellationRequested();
            // running step 2 ... long time

            // step 3
            cancellationToken.ThrowIfCancellationRequested();
            // running step 3 ... long time

            // step 4
            cancellationToken.ThrowIfCancellationRequested();
            // running step 4 ... long time

            // save to db
            cancellationToken.ThrowIfCancellationRequested();
            // save to db, task done.
        }

        [AutomaticRetry(Attempts = 0)]
        public static void DeviceParameterSet(int deviceID, IJobCancellationToken cancellationToken)
        {

        }

        [AutomaticRetry(Attempts = 0)]
        public static void DeviceStatusCheck(int deviceID, IJobCancellationToken cancellationToken)
        {

        }
    }
}