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
    
    public partial class RP_Flowchart_Detail_ME_Equipment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RP_Flowchart_Detail_ME_Equipment()
        {
            this.Actual_Production_Equipment_Qty = new HashSet<Actual_Production_Equipment_Qty>();
            this.RP_Equipment_Demand = new HashSet<RP_Equipment_Demand>();
        }
    
        public int RP_Flowchart_Detail_ME_Equipment_UID { get; set; }
        public int RP_Flowchart_Detail_ME_UID { get; set; }
        public string Equipment_Name { get; set; }
        public string Equipment_Spec { get; set; }
        public string Equipment_Type { get; set; }
        public Nullable<decimal> Plan_CT { get; set; }
        public Nullable<int> Equipment_Qty { get; set; }
        public decimal Ratio { get; set; }
        public Nullable<int> Request_Qty { get; set; }
        public Nullable<int> EQP_Variable_Qty { get; set; }
        public int NPI_Current_Qty { get; set; }
        public int MP_Current_Qty { get; set; }
        public string Notes { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public int Modified_UID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Actual_Production_Equipment_Qty> Actual_Production_Equipment_Qty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RP_Equipment_Demand> RP_Equipment_Demand { get; set; }
        public virtual RP_Flowchart_Detail_ME RP_Flowchart_Detail_ME { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
    }
}
