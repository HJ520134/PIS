using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Mes_StationColorDTO : EntityDTOBase
    {
        public int Mes_StationColor_UID { get; set; }
        public string CustomerName { get; set; }
        public string StationName { get; set; }
        public string Color { get; set; }
    }
}
