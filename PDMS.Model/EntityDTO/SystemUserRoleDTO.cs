using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemUserRoleDTO:EntityDTOBase
    {
        public int System_User_Role_UID { get; set; }
        public int Account_UID { get; set; }
        public int Role_UID { get; set; }
        
    }
}
