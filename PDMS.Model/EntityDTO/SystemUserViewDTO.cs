using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class SystemUserViewDTO : EntityDTOBase
    {
        public int View_UID { get; set; }
        public int Account_UID { get; set; }
        public int Column_UID { get; set; }

    }
}
