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
    
    public partial class Fixture_Machine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fixture_Machine()
        {
            this.Fixture_M = new HashSet<Fixture_M>();
        }
    
        public int Fixture_Machine_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Machine_ID { get; set; }
        public string Equipment_No { get; set; }
        public string Machine_Name { get; set; }
        public string Machine_Desc { get; set; }
        public string Machine_Type { get; set; }
        public int Production_Line_UID { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public Nullable<int> EQP_Uid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fixture_M> Fixture_M { get; set; }
        public virtual Production_Line Production_Line { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
    }
}