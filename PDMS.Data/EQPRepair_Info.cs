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
    
    public partial class EQPRepair_Info
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EQPRepair_Info()
        {
            this.Labor_UsingInfo = new HashSet<Labor_UsingInfo>();
            this.Meterial_UpdateInfo = new HashSet<Meterial_UpdateInfo>();
            this.Storage_Outbound_M = new HashSet<Storage_Outbound_M>();
        }
    
        public int Repair_Uid { get; set; }
        public string Repair_id { get; set; }
        public int EQP_Uid { get; set; }
        public string Status { get; set; }
        public string Repair_Reason { get; set; }
        public string Error_Types { get; set; }
        public System.DateTime Error_Time { get; set; }
        public string Error_Level { get; set; }
        public string Contact { get; set; }
        public string Contact_tel { get; set; }
        public string Reason_Types { get; set; }
        public string Reason_Analysis { get; set; }
        public string Repair_Method { get; set; }
        public Nullable<System.DateTime> Repair_BeginTime { get; set; }
        public Nullable<System.DateTime> Repair_EndTime { get; set; }
        public string Repair_Result { get; set; }
        public string Labor_List { get; set; }
        public string Update_Part { get; set; }
        public Nullable<decimal> Labor_Time { get; set; }
        public Nullable<decimal> All_RepairCost { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string Repair_Remark { get; set; }
        public Nullable<int> EQPUSER_Uid { get; set; }
        public Nullable<System.DateTime> Apply_Time { get; set; }
        public string Mentioner { get; set; }
        public Nullable<int> CostCtr_UID { get; set; }
    
        public virtual CostCtr_info CostCtr_info { get; set; }
        public virtual Equipment_Info Equipment_Info { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Labor_UsingInfo> Labor_UsingInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Meterial_UpdateInfo> Meterial_UpdateInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Storage_Outbound_M> Storage_Outbound_M { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
