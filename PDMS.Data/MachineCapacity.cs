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
    
    public partial class MachineCapacity
    {
        public int MachineCapacity_UID { get; set; }
        public string Area { get; set; }
        public string EQPID { get; set; }
        public Nullable<int> CycleTime { get; set; }
        public Nullable<int> PowerOnTime { get; set; }
        public Nullable<int> PlanQty { get; set; }
        public Nullable<int> OutputQty { get; set; }
        public Nullable<int> ErrorQty { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string State { get; set; }
    }
}
