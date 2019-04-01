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
    
    public partial class System_Schedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public System_Schedule()
        {
            this.Batch_Log = new HashSet<Batch_Log>();
            this.System_Email_Delivery = new HashSet<System_Email_Delivery>();
            this.System_Email_M = new HashSet<System_Email_M>();
        }
    
        public int System_Schedule_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int System_Module_UID { get; set; }
        public int Function_UID { get; set; }
        public string Users_PIC_UIDs { get; set; }
        public string Role_UIDs { get; set; }
        public System.DateTime Start_DateTime { get; set; }
        public string Cycle_Unit { get; set; }
        public string Exec_Moment { get; set; }
        public string Exec_Time { get; set; }
        public Nullable<System.DateTime> Last_Execution_Date { get; set; }
        public Nullable<System.DateTime> Next_Execution_Date { get; set; }
        public bool Is_Email { get; set; }
        public Nullable<bool> Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Batch_Log> Batch_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<System_Email_Delivery> System_Email_Delivery { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<System_Email_M> System_Email_M { get; set; }
        public virtual System_Function System_Function { get; set; }
        public virtual System_Module System_Module { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
    }
}
