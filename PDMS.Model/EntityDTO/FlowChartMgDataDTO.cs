using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowChartMgDataDTO : EntityDTOBase
    {
        public int ShiftTimeID;

        public int FlowChart_MgData_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public DateTime Product_Date { get; set; }
        public int Product_Plan { get; set; }
        public int ? Proper_WIP { get; set; }
        public double IE_DeptHuman { get; set; }
        public int IE_TargetEfficacy { get; set; }
    }
}
