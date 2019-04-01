using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureMaintenanceProcess
{
    public class ConcreteFixtureMaintenanceToDoOne : ABSFixtureMaintenanceToDo
    {

        public override void Request(string request)
        {
            if (request == StructConstants.BatchFixtureMaintenanceRequest.FixtureMaintenanceToDoOne)
            {
                try
                {
                    new FixtureMaintenanceDate().ExecToDoOne();
                }
                catch (Exception ex)
                {
                    CacheHelper.Set(ConstConstants.Has_Exception, true);
                    var errorInfo = string.Empty;
                    if (ex.InnerException == null)
                    {
                        errorInfo = ex.Message;
                    }
                    else
                    {
                        errorInfo = ex.InnerException.Message;
                    }
                }
            }
            else if (todo != null)
            {
                todo.Request(request);
            }
        }



    }
}
