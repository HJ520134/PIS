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
    
    public partial class MES_StationDataRecord
    {
        public int MES_StationDataRecord_UID { get; set; }
        public string Date { get; set; }
        public string TimeInterVal { get; set; }
        public string StartTimeInterval { get; set; }
        public string EndTimeInterval { get; set; }
        public int PIS_ProcessID { get; set; }
        public string PIS_ProcessName { get; set; }
        public string MES_ProcessID { get; set; }
        public string MES_ProcessName { get; set; }
        public int ProductQuantity { get; set; }
        public string ProjectName { get; set; }
        public string ProcessType { get; set; }
    
        public virtual FlowChart_Detail FlowChart_Detail { get; set; }
    }
}
