using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemLocaleStringResourceDTO : EntityDTOBase
    {
        public decimal System_LocaleStringResource_UID { get; set; }
        public int System_Language_UID { get; set; }
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }

        public int CurrentWorkingLanguage { get; set; }
    }
}
