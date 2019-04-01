using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Part_Setting_MDTO : EntityDTOBase
    {
        public int Fixture_Part_Setting_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Fixture_NO { get; set; }
        public int Line_Qty { get; set; }
        public decimal Line_Fixture_Ratio_Qty { get; set; }
        /// <summary>
        /// 使用寿命 小时 及每多少小时算一次
        /// </summary>
        public int? UseTimesScanInterval { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public System.DateTime Created_Date { get; set; }
        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Created_UserName { get; set; }

        public IList<Fixture_Part_Setting_DDTO> Fixture_Part_Setting_Ds { get; set; }
    }
}
