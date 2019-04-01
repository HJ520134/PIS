using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class UserModelSearch : BaseModel
    {
        public int? currntUID { get; set; }
        public int Account_UID { get; set; }

        public string User_NTID { get; set; }

        public string User_Name { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }

        public bool? Enable_Flag { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }
        public List<OrganiztionVM> Orgnizations { get; set; }
    }

    public class UserPasswordInfo:BaseModel
    {
        public int UserId { get; set; }
        /// <summary>
        /// 现密码
        /// </summary>
        public string NowPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
