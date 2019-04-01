using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowChartPCMHRelationshipDTO : EntityDTOBase
    {
        public int PC_MH_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public string Place { get; set; }
        public int MH_UID { get; set; }
        public int System_Language_UID { get; set; }
        public bool isAdd { get; set; }
    }
}
