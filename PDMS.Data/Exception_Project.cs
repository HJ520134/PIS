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
    
    public partial class Exception_Project
    {
        public int Exception_Project_UID { get; set; }
        public int Project_UID { get; set; }
        public int DelayDayNub { get; set; }
        public int DayPeriod { get; set; }
        public int SendMaxTime { get; set; }
        public Nullable<int> Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual System_Project System_Project { get; set; }
    }
}
