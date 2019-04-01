using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Part_UseTimesDTO : EntityDTOBase
    {

        public int Fixture_Part_UseTimes_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int Fixture_Part_Setting_D_UID { get; set; }
        public int Fixture_Part_UseTimesCount { get; set; }

        //自定义
        public string Part_ID { get; set; }
        public string Part_Name { get; set; }
        public string Part_Spec { get; set; }
        public decimal Fixture_Part_Qty { get; set; }
        public int? Fixture_Part_Life_UseTimes { get; set; }
        public bool IsNeedUpdate { get; set; }
        public int Fixture_Part_UID { get; set; }
        public string LastClear_UserName { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime? LastClear_DateTime { get; set; }
    }
}
