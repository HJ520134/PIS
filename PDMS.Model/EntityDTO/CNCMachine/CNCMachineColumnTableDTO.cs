using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class CNCMachineColumnTableDTO : EntityDTOBase
    {

        public int Column_UID { get; set; }
        public string Column_Name { get; set; }
        public int NTID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }
}
