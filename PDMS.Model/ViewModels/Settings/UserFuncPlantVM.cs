using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class UserFuncPlantSearch: BaseModel
    {
        public string Plant { get; set; }
        public string OPType { get; set; }
        public string FuncPlant { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public string Modified_By_NTID { get; set; }
        public DateTime? Modified_Date_From { get; set; }
        public DateTime? Modified_Date_End { get; set; }
    }

    public class UserFuncPlantVM : BaseModel
    {
        public int System_User_FunPlant_UID { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public string Plant { get; set; }
        public string OP_Types { get; set; }
        public string FuncPlant { get; set; }
        public string Modified_UserName { get; set; }
        public string Modified_By_NTID { get; set; }
        public DateTime Modified_Date { get; set; }
    }

    public class EditUserFuncPlant : EntityDTOBase
    {
        public string FuncPlant { get; set; }
        public List<UserPlantWithPlants> UserPlantWithPlants { get; set; }
    }

    public class UserPlantWithPlants : BaseModel
    {
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
    }

}
