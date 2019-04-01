using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class MobileMenuItem
    {
        public int Function_UID { get; set; }
        public string Function_ID { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string onExecute { get; set; }
        public bool? visible { get; set; }
        public bool? disabled { get; set; }
    }

    public class MobileSystemMenu
    {
        public MobileMenuItem key { get; set; }
        public IEnumerable<MobileMenuItem> items { get; set; }
    }
}
