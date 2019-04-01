using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class WorkshopModelSearch : BaseModel
    {
        public int? Workshop_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string Workshop_ID { get; set; }
        public string Workshop_Name { get; set; }
        public string Building_Name { get; set; }
        public string Floor_Name { get; set; }
        public bool? Is_Enable { get; set; }
    }
}
