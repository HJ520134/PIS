using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowchartMappingDTO : EntityDTOBase
    {
        public int Flowchart_Mapping_UID { get; set; }
        public int Flowchart_Detail_ME_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
    }
}
