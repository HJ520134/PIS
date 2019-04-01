using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureOverTimeMaintenanceProcess
{
    public abstract class ABSFixtureOverTimeToDo
    {
        public ABSFixtureOverTimeToDo todo;
        public void SetNextToDo(ABSFixtureOverTimeToDo todo)
        {
            this.todo = todo;
        }
        public abstract void Request(string request);
    }
}
