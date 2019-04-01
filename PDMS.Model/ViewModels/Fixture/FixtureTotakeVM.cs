using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FixtureTotakeVM: EntityDTOBase
    {
        public int Fixture_Totake_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Totake_NO { get; set; }
        public int Shiper_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Ship_Date { get; set; }
        public int Totake_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Totake_Date { get; set; }

        public string Plant { get; set; }
        public string BG { get; set; }
        public string FunPlant { get; set; }
        public string Shiper { get; set; }
        public string Totaker { get; set; }
        public List<fixture> fixtures { get; set; }
    }
    public class fixture : EntityDTOBase
    {
        public int Fixture_Totake_D_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public string ShortCode { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public string Process { get; set; }
        public string WorkStation { get; set; }
        public string Line { get; set; }
    }

}
