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
    
    public partial class Fixture_Storage_Outbound_M
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fixture_Storage_Outbound_M()
        {
            this.Fixture_Storage_Outbound_D = new HashSet<Fixture_Storage_Outbound_D>();
        }
    
        public int Fixture_Storage_Outbound_M_UID { get; set; }
        public int Fixture_Storage_Outbound_Type_UID { get; set; }
        public string Fixture_Storage_Outbound_ID { get; set; }
        public Nullable<int> Fixture_Repair_M_UID { get; set; }
        public Nullable<int> Outbound_Account_UID { get; set; }
        public string Remarks { get; set; }
        public int Applicant_UID { get; set; }
        public System.DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public int Approve_UID { get; set; }
        public System.DateTime Approve_Date { get; set; }
    
        public virtual Enumeration Enumeration { get; set; }
        public virtual Enumeration Enumeration1 { get; set; }
        public virtual Fixture_Repair_M Fixture_Repair_M { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fixture_Storage_Outbound_D> Fixture_Storage_Outbound_D { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
        public virtual System_Users System_Users2 { get; set; }
    }
}