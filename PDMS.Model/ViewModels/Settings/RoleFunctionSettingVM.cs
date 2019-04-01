using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class RoleFunctionSettingVM
    {
        public List<SystemRoleDTO> SystemRoles { get; set; }
    }

    public class RoleFunctionSearchModel : BaseModel
    {
        public int Role_UID { get; set; }

        public string Function_ID { get; set; }

        public string Function_Name { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }
    }

    public class RoleSubFunctionSearchModel
    {
        public int Role_UID { get; set; }

        public int Function_UID { get; set; }
    }

    public class RoleFunctionItem : EntityDTOBase
    {
        public int System_Role_Function_UID { get; set; }

        public int Role_UID { get; set; }

        public int Function_UID { get; set; }

        public string Role_ID { get; set; }

        public string Function_ID { get; set; }

        public string Function_Name { get; set; }

        public int Order_Index { get; set; }

        public string URL { get; set; }

        public bool Is_Show_Function { get; set; }

        public bool Is_Show_Role { get; set; }

        public int SubFunctionCount { get; set; }

    }

    public class RoleFunctionsWithSub : EntityDTOBase
    {
        public int Role_UID { get; set; }

        public string Role_ID { get; set; }

        public string Role_Name { get; set; }

        public List<HeadFunction> HeadFunctions { get; set; }

        public RoleFunctionsWithSub()
        {
            HeadFunctions = new List<HeadFunction>();
        }
    }

    public class HeadFunction
    {
        public HeadFunction()
        {
            SubFun = new List<SubFunction>();
        }
        public int System_Role_Function_UID { get; set; }
        public int Function_UID { get; set; }
        public string Function_ID { get; set; }
        public string Function_Name { get; set; }
        public int Order_Index { get; set; }
        public string URL { get; set; }
        public bool Is_Show { get; set; }
        public bool Is_Show_Role { get; set; }
        public List<SubFunction> SubFun { get; set; }
    }

    public class SubFunction
    {
        public int System_Role_FunctionSub_UID { get; set; }
        public int System_FunctionSub_UID { get; set; }
        public int Function_UID { get; set; }
        public string Sub_Fun { get; set; }
        public string Sub_Fun_Name { get; set; }
        public bool Grant { get; set; }
    }
}
