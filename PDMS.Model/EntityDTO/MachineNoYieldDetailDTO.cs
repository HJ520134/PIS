using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
   public  class MachineNoYieldDetailDTO : EntityDTOBase
    {

        public string Customer { get; set; }
        public string Station { get; set; }
        public string Machine { get; set; }
        public string DefectCode { get; set; }
        public string SN { get; set; }
        public string LastUpdateDate { get; set; }
        public string Fixture { get; set; }
    }
}
