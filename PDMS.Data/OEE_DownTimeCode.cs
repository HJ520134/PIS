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
    
    public partial class OEE_DownTimeCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OEE_DownTimeCode()
        {
            this.OEE_StationDailyDownSum = new HashSet<OEE_StationDailyDownSum>();
            this.OEE_MachineDailyDownRecord = new HashSet<OEE_MachineDailyDownRecord>();
        }
    
        public int OEE_DownTimeCode_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int OEE_DownTimeType_UID { get; set; }
        public int Project_UID { get; set; }
        public Nullable<int> LineID { get; set; }
        public Nullable<int> StationID { get; set; }
        public string Error_Code { get; set; }
        public string Upload_Ways { get; set; }
        public string Level_Details { get; set; }
        public string Error_Reasons { get; set; }
        public string Remarks { get; set; }
        public bool Is_Enable { get; set; }
        public int Modify_UID { get; set; }
        public System.DateTime Modify_Date { get; set; }
    
        public virtual GL_Line GL_Line { get; set; }
        public virtual System_Project System_Project { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_StationDailyDownSum> OEE_StationDailyDownSum { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        public virtual OEE_DownTimeType OEE_DownTimeType { get; set; }
        public virtual GL_Station GL_Station { get; set; }
        public virtual System_Users System_Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_MachineDailyDownRecord> OEE_MachineDailyDownRecord { get; set; }
    }
}
