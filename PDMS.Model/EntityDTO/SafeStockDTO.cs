using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class SafeStockDTO: EntityDTOBase
    {
        public string Safe_Stock_Max { get; set; }
        public string Safe_Stock_LastOpenEQP { get; set; }
        public string Safe_Stock_PlanOpenEQP { get; set; }
    }
}
