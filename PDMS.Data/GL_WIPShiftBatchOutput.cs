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
    
    public partial class GL_WIPShiftBatchOutput
    {
        public int WSBOID { get; set; }
        public int CustomerID { get; set; }
        public System.DateTime OutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public Nullable<int> AssemblyID { get; set; }
        public int LineStarTime { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public Nullable<int> CycleTime { get; set; }
        public decimal MaxStdRouteCT { get; set; }
        public int PlanDownTime { get; set; }
        public int UPDownTime { get; set; }
        public int RunTime { get; set; }
        public int StandOutput { get; set; }
        public decimal UPH { get; set; }
        public decimal SMH { get; set; }
        public int StdHC { get; set; }
        public Nullable<decimal> StdUPPH { get; set; }
        public int ActualOutput { get; set; }
    
        public virtual GL_AssemblyModel GL_AssemblyModel { get; set; }
        public virtual System_Project System_Project { get; set; }
        public virtual GL_Line GL_Line { get; set; }
        public virtual GL_ShiftTime GL_ShiftTime { get; set; }
        public virtual GL_Station GL_Station { get; set; }
    }
}
