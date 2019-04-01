using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_PartDTO : EntityDTOBase
    {
        public int Fixture_Part_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Part_ID { get; set; }
        public string Part_Name { get; set; }
        public string Part_Spec { get; set; }
        public bool Is_Automation { get; set; }
        public bool Is_Standardized { get; set; }
        public bool Is_Storage_Managed { get; set; }
        public int Purchase_Cycle { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public System.DateTime Created_Date { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Created_UserName { get; set; }
    }
}
