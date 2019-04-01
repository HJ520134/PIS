using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemUserDTO:EntityDTOBase
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
        public bool IsMulitProject { get; set; }
        public int? flowChartMaster_Uid { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeePassword { get; set; }
        public int EnableEmpIdLogin { get; set; }
        public string Building { get; set; }

        //Jay 2018-09-06 带出Role
        public List<SystemRoleDTO> RoleList { get; set; }
    }

    public class SystemUserInfo : SystemUserDTO
    {
        public string Org_CTU { get; set; }
        public string Org_Plant { get; set; }
        public string Org_OP { get; set; }
        public string Org_PP { get; set; }
        public string Org_MH { get; set; }
        public string Project { get; set; }
        public string User_Role { get; set; }
    }
    public class SystemUserInfo1 : SystemUserDTO
    {
        public int CurrentUID { get; set; }
        public string Org_CTU { get; set; }
        public string Org_CTU_ID { get; set; }
        public string Org_OP { get; set; }
        public string Org_OP_ID { get; set; }
        public string Org_PP { get; set; }
        public string Org_PP_ID { get; set; }
        public string Org_FunP { get; set; }
        public string Org_FunP_ID { get; set; }
        public List<string> Org_FuncPlant { get; set; }
        public List<string> Org_FuncPlant_ID { get; set; }
        public List<string> Role { get; set; }
        public List<int> Project { get; set; }
    }
    public class SystemUserOEEDTO : EntityDTOBase
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
        public bool IsMulitProject { get; set; }
        public int? flowChartMaster_Uid { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeePassword { get; set; }
        public int? EnableEmpIdLogin { get; set; }
        public string Building { get; set; }
        public int Plant_OrganizationUID { get; set; }
    }
}
