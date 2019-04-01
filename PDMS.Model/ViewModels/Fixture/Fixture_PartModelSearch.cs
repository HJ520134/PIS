using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_PartModelSearch : BaseModel
    {
        public int? Fixture_Part_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Part_ID { get; set; }
        public string Part_Name { get; set; }
        public string Part_Spec { get; set; }
        public bool? Is_Automation { get; set; }
        public bool? Is_Standardized { get; set; }
        public bool? Is_Storage_Managed { get; set; }
        public int? Purchase_Cycle { get; set; }
        public bool? Is_Enable { get; set; }
    }
}
