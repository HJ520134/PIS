using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class OrgBomDTO: EntityDTOBase
    {
        public int Plant_Organization_UID { get; set; }
        public string Plant { get; set; }
        public int BG_Organization_UID { get; set; }
        public string BG { get; set; }
        public int UID3 { get; set; }
        public int Funplant_UID { get; set; }
        public string Funplant { get; set; }

    }
}
