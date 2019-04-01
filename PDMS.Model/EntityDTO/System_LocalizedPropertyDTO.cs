using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class System_LocalizedPropertyDTO : EntityDTOBase
    {
        public int System_LocalizedProperty_UID { get; set; }
        public int System_Language_UID { get; set; }
        public string Table_Name { get; set; }
        public int TablePK_UID { get; set; }
        public string Table_ColumnName { get; set; }
        public string ResourceValue { get; set; }
        public virtual SystemLanguageDTO System_Language { get; set; }

        public string LanguageName { get; set; }
    }
}
