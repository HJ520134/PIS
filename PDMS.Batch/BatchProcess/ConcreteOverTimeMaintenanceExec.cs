using PDMS.Batch.FixtureOverTimeMaintenanceProcess;
using PDMS.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.BatchProcess
{
    public class ConcreteOverTimeMaintenanceExec : ABSExec
    {
        public override void Run()
        {
            ABSFixtureOverTimeToDo todoOne = new ConcreteFixtureOverTimeToDoOne();
            ABSFixtureOverTimeToDo todoTwo = new ConcreteFixtureOverTimeToDoTwo();
            todoOne.SetNextToDo(todoTwo);

            List<string> requestList = new List<string>();
            //设置一级职责链
            requestList.Add(StructConstants.BatchFixtureOverTimeRequest.OverTimeToDoOne);
            //设置二级职责链,将运行结果插入到日志表中
            requestList.Add(ConstConstants.Batch_Log);

            foreach (var requestItem in requestList)
            {
                todoOne.Request(requestItem);
            }

        }
    }
}
