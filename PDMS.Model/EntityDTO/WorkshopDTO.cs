using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class WorkshopDTO: EntityDTOBase
    {
        public int Workshop_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string Workshop_ID { get; set; }
        public string Building_Name { get; set; }
        public string Floor_Name { get; set; }
        public string Workshop_Name { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string PlantName { get; set; }
        public string BGName { get; set; }
        public string FunPlantName { get; set; }
    }
}
