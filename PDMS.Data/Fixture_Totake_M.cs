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
    
    public partial class Fixture_Totake_M
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fixture_Totake_M()
        {
            this.Fixture_Totake_D = new HashSet<Fixture_Totake_D>();
            this.Fixture_Return_M = new HashSet<Fixture_Return_M>();
        }
    
        public int Fixture_Totake_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Totake_NO { get; set; }
        public int Shiper_UID { get; set; }
        public System.DateTime Ship_Date { get; set; }
        public string Totake_Number { get; set; }
        public string Totake_Name { get; set; }
        public System.DateTime Totake_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fixture_Totake_D> Fixture_Totake_D { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fixture_Return_M> Fixture_Return_M { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
        public virtual System_Users System_Users2 { get; set; }
    }
}
