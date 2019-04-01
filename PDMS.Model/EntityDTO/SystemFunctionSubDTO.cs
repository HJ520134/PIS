using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemFunctionSubDTO : EntityDTOBase
    {
        public int System_FunctionSub_UID { get; set; }
        public int Function_UID { get; set; }
        public string Sub_Fun { get; set; }
        public string Sub_Fun_Name { get; set; }
        public string Sub_Fun_URL { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }
}
