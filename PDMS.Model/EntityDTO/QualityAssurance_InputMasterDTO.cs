using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QualityAssurance_InputMasterDTO : EntityDTOBase
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
        public bool NGFlag { get; set; }
        public bool FirstCheckFlag { get; set; }

        public Nullable<int> Displace_Qty { get; set; }
        public bool CanModify { set; get; }
        public bool DisplaceFlag { get; set; }
    }


    public class OQC_InputMasterDTO 
    {
        public Nullable<int> FlowChart_Detail_UID { get; set; }
        public int OQCMater_UID { get; set; }
        public string Time_interval { get; set; }
        public System.DateTime ProductDate { get; set; }
        public string MaterialType { get; set; }
        public string Color { get; set; }
        public int Input { get; set; }
        public Nullable<int> GoodParts_Qty { get; set; }
        public Nullable<int> NGParts_Qty { get; set; }
        public Nullable<int> Rework { get; set; }
        public Nullable<int> ProductLineRework { get; set; }
        public Nullable<int> ReworkQtyFromAssemble { get; set; }
        public Nullable<int> RepairNG_Qty { get; set; }
        public Nullable<int> NG_Qty { get; set; }
        public Nullable<decimal> RepairNG_Yield { get; set; }
        public Nullable<decimal> NG_Yield { get; set; }
        public Nullable<decimal> FirstYieldRate { get; set; }
        public Nullable<decimal> SecondYieldRate { get; set; }
        public Nullable<int> Storage_Qty { get; set; }
        public Nullable<int> WaitStorage_Qty { get; set; }
        public int WIP { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_date { get; set; }
        public System.DateTime Modified_date { get; set; }
        public Nullable<int> Modifier_UID { get; set; }
        public Nullable<int> Project_UID { get; set; }
        public int ReworkQtyFromOQC { set; get; }
    }
}
