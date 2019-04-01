using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureProcess
{
    public class ConcreteFixtureToDoTwo : ABSFixtureToDo
    {
        public override void Request(string request)
        {
            //如果程序一切正常则执行下一步操作
            if (!CacheHelper.HasException())
            {
                if (request == ConstConstants.Batch_Log)
                {
                    //模具排程信息统计成功，写入排程日志表
                    new ExceptionEmailManager().InsertLogInfo(ConstConstants.FixtureMaintenanceBatch, true, null);
                }
                else if (todo != null)
                {
                    todo.Request(request);
                }
            }
        }
    }
}
