using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QualityAssuranceInputMasterHistoryDTO : EntityDTOBase
    {
        public int QualityAssurance_InputMaster_UID { get; set; }
        public Nullable<int> FlowChart_Detail_UID { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string MaterielType { get; set; }
        public System.DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public Nullable<int> Input { get; set; }
        public Nullable<int> FirstCheck_Qty { get; set; }
        public Nullable<int> FirstOK_Qty { get; set; }
        public Nullable<decimal> FirstRejectionRate { get; set; }
        public Nullable<int> NG_Qty { get; set; }
        public Nullable<int> SurfaceSA_Qty { get; set; }
        public Nullable<int> SizeSA_Qty { get; set; }
        public Nullable<int> RepairCheck_Qty { get; set; }
        public Nullable<int> RepairOK_Qty { get; set; }
        public Nullable<int> Shipment_Qty { get; set; }
        public Nullable<int> WIPForCheck_Qty { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public bool FirstCheckFlag { get; set; }
        public bool NGFlag { get; set; }
        public Nullable<int> Displace_Qty { get; set; }
        public bool DisplaceFlag { get; set; }
    }
}
