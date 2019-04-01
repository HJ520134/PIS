using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class SystemViewColumnDTO : EntityDTOBase
    {
        public int Column_UID { get; set; }
        public string View_Name { get; set; }
        public int View_Column_Index { get; set; }
        public string View_Column_Name { get; set; }
        public bool isChecked { get; set; }
        public string View_Group { get; set; }
        public int new_index { get; set; }
    }
}
