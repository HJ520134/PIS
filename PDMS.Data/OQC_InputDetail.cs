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
    
    public partial class OQC_InputDetail
    {
        public Nullable<int> ExceptionType_UID { get; set; }
        public int OQCDetail_UID { get; set; }
        public Nullable<int> OQCMater_UID { get; set; }
        public string FunPlant { get; set; }
        public string TypeClassify { get; set; }
        public int Qty { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual OQC_InputMaster OQC_InputMaster { get; set; }
        public virtual QualityAssurance_ExceptionType QualityAssurance_ExceptionType { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}