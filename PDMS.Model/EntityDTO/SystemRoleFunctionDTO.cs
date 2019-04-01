using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemRoleFunctionDTO:EntityDTOBase
    {
        public int Role_UID { get; set; }
        public string Function_UID { get; set; }
        public string Is_show { get; set; }
    }
}
