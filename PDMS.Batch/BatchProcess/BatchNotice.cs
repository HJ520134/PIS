using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Batch.BatchProcess
{
    public class BatchNotice
    {
        List<ABSExec> execList = new List<ABSExec>();

        public void Attend(ABSExec exec)
        {
            execList.Add(exec);
        }

        public void Remove(ABSExec exec)
        {
            if (execList.Count() > 0)
            {
                execList.Remove(exec);
            }
        }

        public void Run()
        {
            foreach (var item in execList)
            {
                item.Run();
            }
        }
    }
}
