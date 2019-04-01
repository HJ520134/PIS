using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureMaintenanceProcess
{
    public abstract class ABSFixtureMaintenanceToDo
    {

        public ABSFixtureMaintenanceToDo todo;
        public void SetNextToDo(ABSFixtureMaintenanceToDo todo)
        {
            this.todo = todo;
        }
        public abstract void Request(string request);
    }
}
