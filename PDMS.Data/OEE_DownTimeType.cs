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
    
    public partial class OEE_DownTimeType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OEE_DownTimeType()
        {
            this.OEE_DownTimeCode = new HashSet<OEE_DownTimeCode>();
        }
    
        public int OEE_DownTimeType_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Type_Name { get; set; }
        public int Sequence { get; set; }
        public bool Is_Enable { get; set; }
        public int Modify_UID { get; set; }
        public System.DateTime Modify_Date { get; set; }
        public string Type_Code { get; set; }
        public Nullable<int> BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OEE_DownTimeCode> OEE_DownTimeCode { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
