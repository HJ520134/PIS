using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{ 
    public class OrgVM
    {
        public string Name { set; get; }
        public int Organization_UID { set; get; }
    }

    public class OrganiztionVM
    {
        public string Plant { set; get; }
        public int? Plant_OrganizationUID { set; get; }
        public string Plant_OrganizationID { get; set; }
        public string OPType { set; get; }
        public int? OPType_OrganizationUID { set; get; }

        public string Department { set; get; }
        public int? Department_OrganizationUID { set; get; }

        public string Funplant { set; get; }
        public int? Funplant_OrganizationUID { set; get; }
    }
}
