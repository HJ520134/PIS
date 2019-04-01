using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class System_LocalizedPropertyModelSearch : BaseModel
    {
        public int? System_LocalizedProperty_UID { get; set; }
        public int? System_Language_UID { get; set; }
        public string Table_Name { get; set; }
        public int? TablePK_UID { get; set; }
        public string Table_ColumnName { get; set; }
        public string ResourceValue { get; set; }
        public int? Modified_UID { get; set; }
    }
}
