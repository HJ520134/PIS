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
    
    public partial class RP_Demand_Calculation_Record
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RP_Demand_Calculation_Record()
        {
            this.RP_Equipment_Demand = new HashSet<RP_Equipment_Demand>();
            this.RP_Manpower_Demand = new HashSet<RP_Manpower_Demand>();
        }
    
        public int RP_Demand_Calculation_Record_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public Nullable<int> BG_Organization_UID { get; set; }
        public Nullable<int> Project_UID { get; set; }
        public string Part_Types { get; set; }
        public System.DateTime Calculation_Date { get; set; }
        public System.DateTime Demand_Start_Date { get; set; }
        public System.DateTime Demand_End_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RP_Equipment_Demand> RP_Equipment_Demand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RP_Manpower_Demand> RP_Manpower_Demand { get; set; }
        public virtual System_Project System_Project { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
