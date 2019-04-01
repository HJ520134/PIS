using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Totake_DDTO: EntityDTOBase
    {
        public int Fixture_Totake_D_UID { get; set; }
        public int Fixture_Totake_M_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }

        public string Plant { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }
        public string Process_ID { get; set; }
        public string WorkStation_ID { get; set; }
        public string Line_ID { get; set; }
        public string Machine_ID { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public string Vendor_ID { get; set; }
        public string ShortCode { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Fixture_Name { get; set; }
        public DateTime Modified_Date_from { get; set; }
        public DateTime Modified_Date_to { get; set; }

    }
}
