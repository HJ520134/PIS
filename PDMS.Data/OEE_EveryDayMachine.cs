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
    
    public partial class OEE_EveryDayMachine
    {
        public int OEE_EveryDayMachine_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public int FixtureNum { get; set; }
        public double PORCT { get; set; }
        public double ActualCT { get; set; }
        public double TotalAvailableHour { get; set; }
        public double PlannedHour { get; set; }
        public int OutPut { get; set; }
        public Nullable<int> ShiftTimeID { get; set; }
        public System.DateTime Product_Date { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public string ResetTime { get; set; }
        public Nullable<int> Is_DownType { get; set; }
        public Nullable<bool> AbnormalDFCode { get; set; }
        public Nullable<bool> Is_Offline { get; set; }
    
        public virtual GL_ShiftTime GL_ShiftTime { get; set; }
        public virtual OEE_MachineInfo OEE_MachineInfo { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
    }
}
