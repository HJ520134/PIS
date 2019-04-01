using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class OrgBomVM : BaseModel
    {    
        public string Parent_Organization_Name { get; set; }
        public string Child_Organization_Name { get; set; }
        public IEnumerable<SystemOrgBomDTO> IEOrgBom { get; set; }
    }
}
