using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
   public class System_Organization_PlantDTO : EntityDTOBase
    {

        public int System_FunPlant_UID { get; set; }
        public int System_Plant_UID { get; set; }
        public string OP_Types { get; set; }
        public string FunPlant { get; set; }
        public int OPType_OrganizationUID { get; set; }
        public int FunPlant_OrganizationUID { get; set; }
        public int ChildOrg_UID { get; set; }
        public int ParentOrg_UID { get; set; }
    }
}
