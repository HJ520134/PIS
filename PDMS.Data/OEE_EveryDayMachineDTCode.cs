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
    
    public partial class OEE_EveryDayMachineDTCode
    {
        public int OEE_EveryDayMachineDTCode_UID { get; set; }
        public Nullable<int> OEE_MachineInfo_UID { get; set; }
        public Nullable<int> ShiftTimeID { get; set; }
        public string DFCode { get; set; }
        public System.DateTime Product_Date { get; set; }
        public System.DateTime Create_Date { get; set; }
        public Nullable<bool> Is_Enable { get; set; }
        public int Modified_UID { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
    
        public virtual GL_ShiftTime GL_ShiftTime { get; set; }
        public virtual OEE_MachineInfo OEE_MachineInfo { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
