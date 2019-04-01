using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class OQC_InputMasterVM : BaseModel
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
        public Nullable<int> ProductLineRework { get; set; }    //前制程返回
        public Nullable<int> ReworkQtyFromAssemble { get; set; }
        public Nullable<int> RepairNG_Qty { get; set; }
        public Nullable<int> NG_Qty { get; set; }
        public string RepairNG_Yield { get; set; }
        public string NG_Yield { get; set; }
        public string FirstYieldRate { get; set; }
        public string SecondYieldRate { get; set; }
        public Nullable<int> Storage_Qty { get; set; }
        public Nullable<int> WaitStorage_Qty { get; set; }
        public int WIP { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_date { get; set; }
        public System.DateTime Modified_date { get; set; }
        public Nullable<int> Modifier_UID { get; set; }
        public Nullable<int> Project_UID { get; set; }
        public int ReworkQtyFromOQC { set; get; }   ///返修OK数


        public string Process { set; get; }

    }

    public class OQCInputData : BaseModel
    {
        public OQC_InputMasterVM MasterData { set; get; }
        public List<OQC_InputDetailVM> DetailsData { set; get; }
    }


}
