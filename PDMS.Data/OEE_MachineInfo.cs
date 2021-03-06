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
    
    public partial class OEE_MachineInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OEE_MachineInfo()
        {
            this.OEE_DefectCodeDailySum = new HashSet<OEE_DefectCodeDailySum>();
            this.OEE_DefectCodeDailyNum = new HashSet<OEE_DefectCodeDailyNum>();
            this.OEE_EveryDayDFcodeMissing = new HashSet<OEE_EveryDayDFcodeMissing>();
            this.OEE_EveryDayMachineDTCode = new HashSet<OEE_EveryDayMachineDTCode>();
            this.OEE_EveryDayMachine = new HashSet<OEE_EveryDayMachine>();
            this.OEE_MachineDailyDownRecord = new HashSet<OEE_MachineDailyDownRecord>();
            this.OEE_MachineStatus = new HashSet<OEE_MachineStatus>();
            this.OEE_ImprovementPlan = new HashSet<OEE_ImprovementPlan>();
        }
    
        public int OEE_MachineInfo_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int Project_UID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public int EQP_Uid { get; set; }
        public string MachineNo { get; set; }
        public bool Is_Enable { get; set; }
        public int Modify_UID { get; set; }
        public System.DateTime Modify_Date { get; set; }
        public Nullable<int> Is_Downtype { get; set; }
    
        public virtual Equipment_Info Equipment_Info { get; set; }
        public virtual GL_Line GL_Line { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_DefectCodeDailySum> OEE_DefectCodeDailySum { get; set; }
        public virtual System_Project System_Project { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_DefectCodeDailyNum> OEE_DefectCodeDailyNum { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_EveryDayDFcodeMissing> OEE_EveryDayDFcodeMissing { get; set; }
        public virtual GL_Station GL_Station { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_EveryDayMachineDTCode> OEE_EveryDayMachineDTCode { get; set; }
        public virtual System_Users System_Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_EveryDayMachine> OEE_EveryDayMachine { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_MachineDailyDownRecord> OEE_MachineDailyDownRecord { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_MachineStatus> OEE_MachineStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_ImprovementPlan> OEE_ImprovementPlan { get; set; }
    }
}
