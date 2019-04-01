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
    
    public partial class QualityAssurance_ExceptionType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QualityAssurance_ExceptionType()
        {
            this.ExceptionTypeWithFlowchart = new HashSet<ExceptionTypeWithFlowchart>();
            this.OQC_InputDetail = new HashSet<OQC_InputDetail>();
            this.OQC_InputDetail_History = new HashSet<OQC_InputDetail_History>();
            this.QualityAssurance_DistributeRate = new HashSet<QualityAssurance_DistributeRate>();
            this.QualityAssurance_InputDetail = new HashSet<QualityAssurance_InputDetail>();
            this.QualityAssurance_InputDetail_History = new HashSet<QualityAssurance_InputDetail_History>();
        }
    
        public int ExceptionType_UID { get; set; }
        public string TypeName { get; set; }
        public string ShortName { get; set; }
        public bool EnableFlag { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string Org_TypeCode { get; set; }
        public string TypeClassify { get; set; }
        public string Project { get; set; }
        public int Flowchart_Master_UID { get; set; }
        public string BadTypeCode { get; set; }
        public string BadTypeEnglishCode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExceptionTypeWithFlowchart> ExceptionTypeWithFlowchart { get; set; }
        public virtual FlowChart_Master FlowChart_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OQC_InputDetail> OQC_InputDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OQC_InputDetail_History> OQC_InputDetail_History { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QualityAssurance_DistributeRate> QualityAssurance_DistributeRate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QualityAssurance_InputDetail> QualityAssurance_InputDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QualityAssurance_InputDetail_History> QualityAssurance_InputDetail_History { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}