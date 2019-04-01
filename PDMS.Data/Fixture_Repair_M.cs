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
    
    public partial class Fixture_Repair_M
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fixture_Repair_M()
        {
            this.Fixture_Repair_D = new HashSet<Fixture_Repair_D>();
            this.Fixture_Storage_Outbound_M = new HashSet<Fixture_Storage_Outbound_M>();
        }
    
        public int Fixture_Repair_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Repair_NO { get; set; }
        public int Repair_Location_UID { get; set; }
        public string SentOut_Number { get; set; }
        public string SentOut_Name { get; set; }
        public int Receiver_UID { get; set; }
        public System.DateTime SentOut_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fixture_Repair_D> Fixture_Repair_D { get; set; }
        public virtual Repair_Location Repair_Location { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fixture_Storage_Outbound_M> Fixture_Storage_Outbound_M { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
        public virtual System_Users System_Users2 { get; set; }
    }
}