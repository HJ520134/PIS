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
    
    public partial class Storage_Inbound
    {
        public int Storage_Inbound_UID { get; set; }
        public int Storage_Inbound_Type_UID { get; set; }
        public string Storage_Inbound_ID { get; set; }
        public int Material_UID { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public Nullable<decimal> Inbound_Price { get; set; }
        public int PartType_UID { get; set; }
        public string PU_NO { get; set; }
        public decimal PU_Qty { get; set; }
        public string Issue_NO { get; set; }
        public decimal Be_Check_Qty { get; set; }
        public decimal OK_Qty { get; set; }
        public decimal NG_Qty { get; set; }
        public int Applicant_UID { get; set; }
        public System.DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public string Desc { get; set; }
        public int Approver_UID { get; set; }
        public System.DateTime Approver_Date { get; set; }
        public Nullable<decimal> Current_POPrice { get; set; }
    
        public virtual Enumeration Enumeration { get; set; }
        public virtual Enumeration Enumeration1 { get; set; }
        public virtual Enumeration Enumeration2 { get; set; }
        public virtual Warehouse_Storage Warehouse_Storage { get; set; }
        public virtual Material_Info Material_Info { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
    }
}