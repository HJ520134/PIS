using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.FixtureProcess
{
    public abstract class ABSFixtureToDo
    {
        public ABSFixtureToDo todo;
        public void SetNextToDo(ABSFixtureToDo todo)
        {
            this.todo = todo;
        }
        public abstract void Request(string request);
    }
}
