using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowChartMgDataTempDTO : EntityDTOBase
    {
        public int FlowChart_MgDataT_UID { get; set; }
        public int FlowChart_DT_UID { get; set; }
        public int Product_Plan { get; set; }
        public double Target_Yield { get; set; }
    }
}
