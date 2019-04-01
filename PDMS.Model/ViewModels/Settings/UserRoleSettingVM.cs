using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class UserRoleSettingVM
    {
        public List<SystemRoleDTO> SystemRoles { get; set; }
    }

    /// <summary>
    /// search功能
    /// </summary>
    public class UserRoleSearchModel : BaseModel
    {
        public int System_User_Role_UID { get; set; }

        public string User_NTID { get; set; }

        public string User_Name { get; set; }

        public string Role_ID { get; set; }

        public string Role_Name { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }
    }

    /// <summary>
    /// grid顯示
    /// </summary>
    public class UserRoleItem : EntityDTOBase
    {
        public int System_User_Role_UID { get; set; }

        public string User_NTID { get; set; }

        public string User_Name { get; set; }

        public string Role_ID { get; set; }

        public string Role_Name { get; set; }
    }
}
