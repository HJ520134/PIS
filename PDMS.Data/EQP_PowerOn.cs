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
    
    public partial class EQP_PowerOn
    {
        public int EQP_PowerOn_UID { get; set; }
        public int EQP_Type_UID { get; set; }
        public System.DateTime PowerOn_Date { get; set; }
        public int Daily_PowerOn_Qty { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual EQP_Type EQP_Type { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
