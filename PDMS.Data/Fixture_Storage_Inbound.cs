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
    
    public partial class Fixture_Storage_Inbound
    {
        public int Fixture_Storage_Inbound_UID { get; set; }
        public int Fixture_Storage_Inbound_Type_UID { get; set; }
        public string Fixture_Storage_Inbound_ID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public int Fixture_Warehouse_Storage_UID { get; set; }
        public Nullable<int> Fixture_Part_Order_M_UID { get; set; }
        public decimal Inbound_Qty { get; set; }
        public Nullable<decimal> Inbound_Price { get; set; }
        public string Issue_NO { get; set; }
        public string Remarks { get; set; }
        public int Applicant_UID { get; set; }
        public System.DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public int Approve_UID { get; set; }
        public System.DateTime Approve_Date { get; set; }
    
        public virtual Enumeration Enumeration { get; set; }
        public virtual Enumeration Enumeration1 { get; set; }
        public virtual Fixture_Warehouse_Storage Fixture_Warehouse_Storage { get; set; }
        public virtual Fixture_Part Fixture_Part { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
    }
}
