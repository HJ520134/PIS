using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ProductionSchedulNPIDTO : EntityDTOBase
    {
        public int Production_Schedul_NPI_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public Nullable<System.DateTime> Product_Date { get; set; }
        public Nullable<int> Input { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }

    }
}
