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
    
    public partial class CNCMachine
    {
        public int CNCMachineUID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public Nullable<int> EQP_Uid { get; set; }
        public string Machine_Name { get; set; }
        public string Machine_ID { get; set; }
        public int Project_UID { get; set; }
        public bool Is_Enable { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        public virtual System_Project System_Project { get; set; }
        public virtual Equipment_Info Equipment_Info { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
