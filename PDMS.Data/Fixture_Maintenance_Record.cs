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
    
    public partial class Fixture_Maintenance_Record
    {
        public int Fixture_Maintenance_Record_UID { get; set; }
        public int Fixture_Maintenance_Profile_UID { get; set; }
        public string Maintenance_Record_NO { get; set; }
        public int Fixture_M_UID { get; set; }
        public Nullable<System.DateTime> Maintenance_Date { get; set; }
        public int Maintenance_Status { get; set; }
        public string Maintenance_Person_Number { get; set; }
        public string Maintenance_Person_Name { get; set; }
        public Nullable<System.DateTime> Confirm_Date { get; set; }
        public Nullable<int> Confirm_Status { get; set; }
        public Nullable<int> Confirmor_UID { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual Fixture_Maintenance_Profile Fixture_Maintenance_Profile { get; set; }
        public virtual Fixture_M Fixture_M { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
        public virtual System_Users System_Users2 { get; set; }
    }
}
