//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PDMS.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product_Input_Location
    {
        public int Product_Input_Location_UID { get; set; }
        public bool Is_Comfirm { get; set; }
        public System.DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string FunPlant { get; set; }
        public string FunPlant_Manager { get; set; }
        public string Product_Phase { get; set; }
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string Process { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public string Color { get; set; }
        public int Prouct_Plan { get; set; }
        public int Product_Stage { get; set; }
        public double Target_Yield { get; set; }
        public int Good_QTY { get; set; }
        public string Good_MismatchFlag { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public int NG_QTY { get; set; }
        public int WH_QTY { get; set; }
        public Nullable<int> WIP_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public string Material_No { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public Nullable<bool> IsLast { get; set; }
        public string DRI { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int Normal_Good_QTY { get; set; }
        public int Abnormal_Good_QTY { get; set; }
        public int Normal_NG_QTY { get; set; }
        public int Abnormal_NG_QTY { get; set; }
        public byte[] RowVersion { get; set; }
        public Nullable<int> NullWip_QTY { get; set; }
        public string Config { get; set; }
        public Nullable<int> NotNullWIP { get; set; }
        public string Unacommpolished_Reason { get; set; }
    }
}
