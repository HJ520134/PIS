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
    
    public partial class Labor_UsingInfo
    {
        public int Labor_Using_Uid { get; set; }
        public int Repair_Uid { get; set; }
        public int EQPUser_Uid { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public Nullable<int> Modified_EQPUser_Uid { get; set; }
    
        public virtual EQP_UserTable EQP_UserTable { get; set; }
        public virtual EQPRepair_Info EQPRepair_Info { get; set; }
    }
}
