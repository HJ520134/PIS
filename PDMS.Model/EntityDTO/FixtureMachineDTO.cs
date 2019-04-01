using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
   public class FixtureMachineDTO : EntityDTOBase
    {
        //来自Fixture_Machine
        public int Fixture_Machine_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Machine_ID { get; set; }
        public string Equipment_No { get; set; }
        public string Machine_Name { get; set; }
        public string Machine_Desc { get; set; }
        public string Machine_Type { get; set; }
        public int Production_Line_UID { get; set; }
        public bool Is_Enable { get; set; }
        public string Created_UserName { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public System.DateTime? Created_Date { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Production_Line_ID { get; set; }
        public string Production_Line_Name { get; set; }

        public string Machine_IDandName { get; set; }

        //TODO 2018/09/19 steven 加入与equipment设备关联栏位EQP_Uid，但EQP_EMTSerialNum是用来参照
        public int? EQP_Uid { get; set; }
    }
}
