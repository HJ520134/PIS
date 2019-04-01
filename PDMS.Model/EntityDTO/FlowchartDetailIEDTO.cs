using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowchartDetailIEDTO : EntityDTOBase
    {
        public int Flowchart_Detail_IE_UID { get; set; }
        public int Flowchart_Detail_ME_UID { get; set; }
        public Nullable<int> VariationOP_Qty { get; set; }
        public Nullable<int> RegularOP_Qty { get; set; }
        public Nullable<int> MaterialKeeper_Qty { get; set; }
        public Nullable<int> Match_Rule { get; set; }
        public Nullable<int> VariationEquipment_RequstQty { get; set; }
        public decimal SquadLeader_Raito { get; set; }
        public int SquadLeader_Variable_Qty { get; set; }
        public decimal Technician_Raito { get; set; }
        public int Technician_Variable_Qty { get; set; }
        public Nullable<int> SquadLeader_Qty { get; set; }
        public Nullable<int> Technician_Qty { get; set; }
        public Nullable<int> Others_Qty { get; set; }
        public string Notes { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
    }
}
