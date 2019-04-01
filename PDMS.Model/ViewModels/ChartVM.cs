using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ChartSearch : BaseModel
    {
        public string Customer { get; set; }
        public string Project { get; set; }
        public string ProductPhase { get; set; }
        public string PartTypes { get; set; }
        public string Color { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string SearchType { get; set; }
        public string IntervalTime { get; set; }
        public string FuncPlant { get; set; }
        public List<string> ProcessList { get; set; }
    }
}
