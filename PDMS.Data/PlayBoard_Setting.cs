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
    
    public partial class PlayBoard_Setting
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PlayBoard_Setting()
        {
            this.PlayBoard_PlayTime = new HashSet<PlayBoard_PlayTime>();
        }
    
        public int PlayBoard_Setting_ID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int PlayBoard_View_ID { get; set; }
        public int PlaySeq { get; set; }
        public string JsonParameter { get; set; }
        public string Remark { get; set; }
        public bool IsEnabled { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public bool IsTiming { get; set; }
        public int Play_UID { get; set; }
        public string Title { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayBoard_PlayTime> PlayBoard_PlayTime { get; set; }
        public virtual PlayBoard_View PlayBoard_View { get; set; }
        public virtual System_Organization System_Organization { get; set; }
        public virtual System_Organization System_Organization1 { get; set; }
        public virtual System_Organization System_Organization2 { get; set; }
        public virtual System_Users System_Users { get; set; }
        public virtual System_Users System_Users1 { get; set; }
        public virtual System_Users System_Users2 { get; set; }
    }
}
