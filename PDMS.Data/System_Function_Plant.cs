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
    
    public partial class System_Function_Plant
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public System_Function_Plant()
        {
            this.ExceptionTypeWithFlowchart = new HashSet<ExceptionTypeWithFlowchart>();
            this.QualityAssurance_DistributeRate = new HashSet<QualityAssurance_DistributeRate>();
            this.QualityAssurance_InputDetail = new HashSet<QualityAssurance_InputDetail>();
            this.QualityAssurance_InputDetail_History = new HashSet<QualityAssurance_InputDetail_History>();
            this.System_User_FunPlant = new HashSet<System_User_FunPlant>();
            this.Warning_List = new HashSet<Warning_List>();
            this.Warning_List1 = new HashSet<Warning_List>();
            this.FlowChart_Detail = new HashSet<FlowChart_Detail>();
            this.Equipment_Info = new HashSet<Equipment_Info>();
        }
    
        public int System_FunPlant_UID { get; set; }
        public int System_Plant_UID { get; set; }
        public string OP_Types { get; set; }
        public string FunPlant { get; set; }
        public string FunPlant_Manager { get; set; }
        public string FunPlant_Contact { get; set; }
        public System.DateTime Begin_Date { get; set; }
        public Nullable<System.DateTime> End_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public Nullable<int> OPType_OrganizationUID { get; set; }
        public Nullable<int> FunPlant_OrganizationUID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExceptionTypeWithFlowchart> ExceptionTypeWithFlowchart { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QualityAssurance_DistributeRate> QualityAssurance_DistributeRate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QualityAssurance_InputDetail> QualityAssurance_InputDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QualityAssurance_InputDetail_History> QualityAssurance_InputDetail_History { get; set; }
        public virtual System_Plant System_Plant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<System_User_FunPlant> System_User_FunPlant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Warning_List> Warning_List { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Warning_List> Warning_List1 { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FlowChart_Detail> FlowChart_Detail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Equipment_Info> Equipment_Info { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
