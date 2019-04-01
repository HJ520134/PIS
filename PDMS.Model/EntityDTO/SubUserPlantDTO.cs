using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SubUserPlantDTO : EntityDTOBase
    {
        public int System_Plant_UID { get; set; }
        public string Plant { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Plant_Code { get; set; }
        public DateTime Begin_Date { get; set; }
        public DateTime End_Date { get; set; }
    }
}
