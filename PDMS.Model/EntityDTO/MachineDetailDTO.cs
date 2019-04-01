using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class MachineDetailDTO : EntityDTOBase
    {
        public string Customer { get; set; }
        public string Station { get; set; }
        public string Machine { get; set; }
        public string DefectName { get; set; }
        public int NG_Point { get; set; }
        public decimal NO_Yield { get; set; }
    }
}
