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
    
    public partial class Exception_Record
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Exception_Record()
        {
            this.Exception_Reply = new HashSet<Exception_Reply>();
        }
    
        public int Exception_Record_UID { get; set; }
        public Nullable<int> Origin_UID { get; set; }
        public Nullable<int> SubOrigin_UID { get; set; }
        public int Exception_Code_UID { get; set; }
        public Nullable<int> Project_UID { get; set; }
        public Nullable<System.DateTime> WorkDate { get; set; }
        public Nullable<int> ShiftTimeID { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> End_Date { get; set; }
        public string Note { get; set; }
        public string Contact_Person { get; set; }
        public string Contact_Phone { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    
        public virtual Exception_Code Exception_Code { get; set; }
        public virtual System_Project System_Project { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exception_Reply> Exception_Reply { get; set; }
    }
}
