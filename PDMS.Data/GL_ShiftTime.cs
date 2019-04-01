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
    
    public partial class GL_ShiftTime
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GL_ShiftTime()
        {
            this.GL_CustomerShiftPerf = new HashSet<GL_CustomerShiftPerf>();
            this.GL_GoldenStationCTRecord = new HashSet<GL_GoldenStationCTRecord>();
            this.GL_LineShiftPerf = new HashSet<GL_LineShiftPerf>();
            this.GL_MealTime = new HashSet<GL_MealTime>();
            this.GL_BuildPlan = new HashSet<GL_BuildPlan>();
            this.GL_StationShiftPerf = new HashSet<GL_StationShiftPerf>();
            this.GL_WIPHourOutput = new HashSet<GL_WIPHourOutput>();
            this.GL_WIPShiftBatchOutput = new HashSet<GL_WIPShiftBatchOutput>();
            this.OEE_DefectCodeDailySum = new HashSet<OEE_DefectCodeDailySum>();
            this.OEE_StationDailyDownSum = new HashSet<OEE_StationDailyDownSum>();
            this.IPQCQualityReport = new HashSet<IPQCQualityReport>();
            this.IPQCQualityDetial = new HashSet<IPQCQualityDetial>();
            this.GL_LineShiftResposibleUser = new HashSet<GL_LineShiftResposibleUser>();
            this.OEE_DefectCodeDailyNum = new HashSet<OEE_DefectCodeDailyNum>();
            this.GL_Rest = new HashSet<GL_Rest>();
            this.OEE_EveryDayDFcodeMissing = new HashSet<OEE_EveryDayDFcodeMissing>();
            this.OEE_EveryDayMachineDTCode = new HashSet<OEE_EveryDayMachineDTCode>();
            this.FlowChart_IEData = new HashSet<FlowChart_IEData>();
            this.OEE_EveryDayMachine = new HashSet<OEE_EveryDayMachine>();
            this.OEE_MachineDailyDownRecord = new HashSet<OEE_MachineDailyDownRecord>();
            this.OEE_MachineStatus = new HashSet<OEE_MachineStatus>();
        }
    
        public int ShiftTimeID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public string Shift { get; set; }
        public string StartTime { get; set; }
        public string End_Time { get; set; }
        public bool IsEnabled { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public double Break_Time { get; set; }
        public int Sequence { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_CustomerShiftPerf> GL_CustomerShiftPerf { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_GoldenStationCTRecord> GL_GoldenStationCTRecord { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_LineShiftPerf> GL_LineShiftPerf { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_MealTime> GL_MealTime { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_BuildPlan> GL_BuildPlan { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_StationShiftPerf> GL_StationShiftPerf { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_WIPHourOutput> GL_WIPHourOutput { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_WIPShiftBatchOutput> GL_WIPShiftBatchOutput { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_DefectCodeDailySum> OEE_DefectCodeDailySum { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_StationDailyDownSum> OEE_StationDailyDownSum { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IPQCQualityReport> IPQCQualityReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IPQCQualityDetial> IPQCQualityDetial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_LineShiftResposibleUser> GL_LineShiftResposibleUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_DefectCodeDailyNum> OEE_DefectCodeDailyNum { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_Rest> GL_Rest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_EveryDayDFcodeMissing> OEE_EveryDayDFcodeMissing { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_EveryDayMachineDTCode> OEE_EveryDayMachineDTCode { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FlowChart_IEData> FlowChart_IEData { get; set; }
        public virtual System_Users System_Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_EveryDayMachine> OEE_EveryDayMachine { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_MachineDailyDownRecord> OEE_MachineDailyDownRecord { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_MachineStatus> OEE_MachineStatus { get; set; }
    }
}
