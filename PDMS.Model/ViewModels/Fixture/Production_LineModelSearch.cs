using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Production_LineModelSearch : BaseModel
    {
        public int? Production_Line_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }

        public string Line_ID { get; set; }
        public string Line_Name { get; set; }
        public string Line_Desc { get; set; }

        public int? Workshop_UID { get; set; }
        public int? Workstation_UID { get; set; }
        public int? Project_UID { get; set; }
        public int? Process_Info_UID { get; set; }

        public bool? Is_Enable { get; set; }
    }
}
