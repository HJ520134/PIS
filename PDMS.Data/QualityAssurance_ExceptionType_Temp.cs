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
    
    public partial class QualityAssurance_ExceptionType_Temp
    {
        public int UID { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public string TypeName { get; set; }
        public string ShortName { get; set; }
        public System.DateTime Create_Date { get; set; }
        public string TypeClassify { get; set; }
        public int TypeLevel { get; set; }
        public string FatherNode { get; set; }
        public string Project { get; set; }
        public int Flowchart_Master_UID { get; set; }
        public string BadTypeCode { get; set; }
        public string BadTypeEnglishCode { get; set; }
    
        public virtual FlowChart_Master FlowChart_Master { get; set; }
        public virtual System_Users System_Users { get; set; }
    }
}
