using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model
{
    public class SystemBUD_OrgDTO : EntityDTOBase
    {
        public int System_BU_D_Org_UID { get; set; }
        public int BU_D_UID { get; set; }
        public int Organization_UID { get; set; }

    }
}
