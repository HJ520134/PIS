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
    
    public partial class GL_MetricInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GL_MetricInfo()
        {
            this.GL_ActionTaskerD = new HashSet<GL_ActionTaskerD>();
        }
    
        public int Metric_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Metric_ID { get; set; }
        public string Metric_Name { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_ActionTaskerD> GL_ActionTaskerD { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
