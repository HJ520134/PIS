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
    
    public partial class System_Email_Delivery
    {
        public int System_Email_Delivery_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public Nullable<int> BG_Organization_UID { get; set; }
        public int System_Schedule_UID { get; set; }
        public Nullable<int> Account_UID { get; set; }
        public string User_Name_CN { get; set; }
        public string User_Name_EN { get; set; }
        public string Email { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Schedule System_Schedule { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
        public virtual System_Users System_Users2 { get; set; }
    }
}
