using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class EQPUserTableDTO : EntityDTOBase
    {
        public int EQPUser_Uid { get; set; }    
        public int? BG_Organization_UID { get; set; }
        public string BG_Organization { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string FunPlant_Organization { get; set; }
        public int User_Id { get; set; }
        public string User_Name { get; set; }
        public string User_IdAndName { get; set; }
        public string User_Email { get; set; }
        public string User_Call { get; set; }
        public int Plant_OrganizationUID { get; set; }
        public int? Organization_UID { get; set; }
        public int? FunPlant_OrganizationUID { get; set; }
        public int IsDisable { get; set; }
        public string IsDisableName { get; set; }
    }
}
