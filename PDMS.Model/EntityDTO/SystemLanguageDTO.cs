using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemLanguageDTO : EntityDTOBase
    {
        public int System_Language_UID { get; set; }
        public string Name { get; set; }
        public string LanguageCulture { get; set; }
        public string FlagImageFileName { get; set; }
        public bool Enable_Flag { get; set; }
        public int DisplayOrder { get; set; }
    }
}
