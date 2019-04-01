using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace PDMS.Model
{
   public class SystemOrgAndProVM
   {
       public List<SystemOrg> SystemOrg { get; set; }
       public List<SystemPro> Project { get; set; }
       public List<UserRoleJ> UserRole { get; set; }
   }

    public class SystemOrg
    {
        public string Father_Org { get; set; }
        public string Child_Org { get; set; }
        public int Father_Org_ID { get; set; }
        public int Organization_UID { get; set; }
        public string Organization_ID { get; set; }
    }

    public class SystemPro
    {
        public string OPTypes { get; set; }
        public string Project { get; set; }
        public int ProjectID { get; set; }
        public string plant { get; set; }
    }

    public class UserRole
    {
        public string UserRoleID { get; set; }
        public string UserRoleName { get; set; }
        
    }
    public class UserRoleJ
    {
        public int RoleUID { get; set; }
        public string UserRoleID { get; set; }
        public string UserRoleName { get; set; }

    }
}
