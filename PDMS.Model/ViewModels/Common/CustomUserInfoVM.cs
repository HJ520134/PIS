using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Common
{
    public class CustomUserInfoVM : BaseModel
    {
        public int Account_UID { get; set; }

        public string User_NTID { get; set; }

        public string User_Name { get; set; }

        public bool Enable_Flag { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }

        public int Language_UID { get; set; }

        public string LoginToken { get; set; }

        public bool MH_Flag { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public int? EnableEmpIdLogin { get; set; }

        public int? EmployeeNumber { get; set; }

        public string EmployeePassword { get; set; }

        public string Building { get; set; }

        public List<SystemRoleDTO> RoleList { get; set; }

        public List<OrganiztionVM> OrgInfo { get; set; }

        public List<string> OpTypes { get; set; }

        public List<int> OPType_OrganizationUIDList { get; set; }

        public List<int> Plant_OrganizationUIDList { get; set; }

        //获取用户所属的二级组织Organization_Name为2开头的
        //public List<string> OpTypeList { get; set; }

        //获取该用户可以访问的专案
        public List<int> ProjectUIDList { get; set; }

        //无锡Etransfer要用，需要用到专案名字的匹配
        public List<string> projectTypeList { get; set; }
    }

}
