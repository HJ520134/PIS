using PDMS.Batch.FixtureMaintenanceProcess;
using PDMS.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.BatchProcess
{
    class ConcreteFixtureMaintenanceExec : ABSExec
    {
        
        public override void Run()
        {
            //设置各自职责
            ABSFixtureMaintenanceToDo todoOne = new ConcreteFixtureMaintenanceToDoOne();
            ABSFixtureMaintenanceToDo todoTwo = new ConcreteFixtureMaintenanceToDoTwo();
            todoOne.SetNextToDo(todoTwo);

            List<string> requestList = new List<string>();
            //设置一级职责链
            requestList.Add(StructConstants.BatchFixtureMaintenanceRequest.FixtureMaintenanceToDoOne);
            //设置二级职责链,将运行结果插入到日志表中
            requestList.Add(ConstConstants.Batch_Log);


            foreach (var requestItem in requestList)
            {
                todoOne.Request(requestItem);
            }
        }
    }
}
