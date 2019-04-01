using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class NoticeVM : BaseModel
    {
        public string Color { get; set; }
        public int UID { get; set; }
        public string Notice_Content { get; set; }
        public string Period { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        public string Creator_User { get; set; }
        public System.DateTime Creat_Time { get; set; }
        public Nullable<int> RepeatTime { get; set; }
   
}

    public class NoticeVtual : BaseModel
    {
         public string Color { get; set; }
        public int UID { get; set; }
        public string Notice_Content { get; set; }
        public System.DateTime Start_Time { get; set; }
        public System.DateTime End_Time { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        public string Creator_User { get; set; }
        public System.DateTime Creat_Time { get; set; }
        public Nullable<int> RepeatTime { get; set; }

    }

    public class NoticeSearch : BaseModel
    {
        public string Creator_User { get; set; }
        public System.DateTime Creat_Time { get; set; }
    }

    public class NoticeDTO : EntityDTOBase
    {
        public int UID { get; set; }
        public string Notice_Content { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        public string Color { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Start_Time { get; set; }
        public System.DateTime End_Time { get; set; }
        public System.DateTime Creat_Time { get; set; }
        public Nullable<int> RepeatTime { get; set; }
   
}

}
