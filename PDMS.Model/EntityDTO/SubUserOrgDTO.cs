using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SubUserOrgDTO : EntityDTOBase
    {
        public int Organization_UID { get; set; }
  
        public string Organization_ID { get; set; }
        public string Organization_Name { get; set; }
        public System.DateTime Begin_Date { get; set; }
        public Nullable<System.DateTime> End_Date { get; set; }

    }
}
