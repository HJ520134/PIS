using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Part_Demand_MDTO : EntityDTOBase
    {
        public int Fixture_Part_Demand_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Calculation_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Demand_Date { get; set; }
        public int Applicant_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public int Approver_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Approver_Date { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Applicant_Name { get; set; }
        public string Approver_Name{ get; set; }
        public string StatusName { get; set; }
        public IList<Fixture_Part_Demand_DDTO> Fixture_Part_Demand_DDTOList { get; set; }
    }
}
