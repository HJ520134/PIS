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
    
    public partial class EQP_Type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EQP_Type()
        {
            this.EQP_Forecast_PowerOn = new HashSet<EQP_Forecast_PowerOn>();
            this.EQP_PowerOn = new HashSet<EQP_PowerOn>();
            this.Material_Spareparts_Demand = new HashSet<Material_Spareparts_Demand>();
            this.Material_Repair_Demand = new HashSet<Material_Repair_Demand>();
            this.Material_Normal_Demand = new HashSet<Material_Normal_Demand>();
            this.EQP_Material = new HashSet<EQP_Material>();
        }
    
        public int EQP_Type_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string EQP_Type1 { get; set; }
        public string Type_Desc { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public bool Is_Enable { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQP_Forecast_PowerOn> EQP_Forecast_PowerOn { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQP_PowerOn> EQP_PowerOn { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Material_Spareparts_Demand> Material_Spareparts_Demand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Material_Repair_Demand> Material_Repair_Demand { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Material_Normal_Demand> Material_Normal_Demand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQP_Material> EQP_Material { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}