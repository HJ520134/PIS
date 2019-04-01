using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowchartDetailMEEquipmentDTO : BaseModel
    {
        public int Flowchart_Detail_ME_Equipment_UID { get; set; }
        public int Flowchart_Detail_ME_UID { get; set; }
        public string Equipment_Name { get; set; }
        public string Equipment_Spec { get; set; }
        public Nullable<decimal> Plan_CT { get; set; }
        public Nullable<decimal> Current_CT { get; set; }
        public Nullable<int> EquipmentQty { get; set; }
        public Nullable<decimal> Ratio { get; set; }
        public Nullable<int> RequestQty { get; set; }
        public string EquipmentType { get; set; }
        public string Notes { get; set; }
        public int? NPI_CurrentQty { get; set; }
        public int? MP_CurrentQty { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public int Modified_UID { get; set; }

        //额外增加
        public int Process_Seq { get; set; }
    }
}
