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
    
    public partial class System_Seq
    {
        public int System_Seq_UID { get; set; }
        public string Seq_Type { get; set; }
        public string Seq_Desc { get; set; }
        public int Next_Seq_Value { get; set; }
        public int Max_Seq_Value { get; set; }
        public string Seq_Format { get; set; }
        public System.DateTime Current_Date { get; set; }
        public int Seq_Initial { get; set; }
        public int Seq_Interval { get; set; }
        public string Reset_Type { get; set; }
        public string Is_Running { get; set; }
        public bool Is_Enable { get; set; }
        public string Remarks { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
    }
}
