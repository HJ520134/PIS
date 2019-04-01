using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class PlayBoard_PlayUsetDTO : EntityDTOBase
    {
        public int Account_UID { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public bool Enable_Flag { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public int System_Language_UID { get; set; }
        public string LoginToken { get; set; }
        public bool MH_Flag { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<int> EnableEmpIdLogin { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeePassword { get; set; }
        public string Building { get; set; }

        //自定义
        public string Role_ID { get; set; }
        public string Role_Name { get; set; }

    }
}
