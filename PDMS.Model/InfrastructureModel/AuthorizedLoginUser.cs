using System.Collections.Generic;

namespace PDMS.Model
{
    public class AuthorizedLoginUser
    {
        public int Account_UID { get; set; }
        public string User_Name { get; set; }
        public string Token { get; set; }
        public int System_Language_UID { get; set; }
        public bool MH_Flag { get; set; }
        public bool IsMulitProject { get; set; }
        public int? flowChartMaster_Uid { get; set; }
        public string USER_Ntid { get; set; }

        //Jay 2018-09-06 带出Role
        public List<SystemRoleDTO> RoleList { get; set; }
    }
}
