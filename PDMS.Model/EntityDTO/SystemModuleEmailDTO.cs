using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemModuleEmailDTO : EntityDTOBase
    {
        public int System_Module_Email_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int System_Module_Function_UID { get; set; }
        public string User_Name_CN { get; set; }
        public string User_Name_EN { get; set; }
        public string Email { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
    }
}
