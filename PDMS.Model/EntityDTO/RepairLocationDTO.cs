using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class RepairLocationDTO: EntityDTOBase
    {
        public int Repair_Location_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Repair_Location_ID { get; set; }
        public string Repair_Location_Name { get; set; }
        public string Repair_Location_Desc { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }

        public string Creator { get; set; }
        public string Modifier { get; set; }
        public string Plant { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }
        public bool needSearchEnable { get; set; }
    }
}
