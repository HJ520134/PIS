using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Part_Setting_DDTO: EntityDTOBase
    {
        public int Fixture_Part_Setting_D_UID { get; set; }
        public int Fixture_Part_Setting_M_UID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public decimal Fixture_Part_Qty { get; set; }
        public decimal Fixture_Part_Life { get; set; }
        public bool IsUseTimesManagement { get; set; }
        public int? Fixture_Part_Life_UseTimes { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public System.DateTime Created_Date { get; set; }
        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Created_UserName { get; set; }
        public string Part_ID { get; set; }
        public string Part_Name { get; set; }
        public string Part_Spec { get; set; }
    }
}
