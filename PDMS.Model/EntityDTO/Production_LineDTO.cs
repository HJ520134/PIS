using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Production_LineDTO : EntityDTOBase
    {
        //来自Production_Line
        public int Production_Line_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Line_ID { get; set; }
        public string Line_Name { get; set; }
        public string Line_Desc { get; set; }
        public int Workshop_UID { get; set; }
        public int Workstation_UID { get; set; }
        public int Project_UID { get; set; }
        public int Process_Info_UID { get; set; }
        public string Workshop { get; set; }
        public string Workstation { get; set; }
        public string Project { get; set; }
        public string Process_Info { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public string Created_UserName { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Workshop_Name { get; set; }
        public string Workshop_ID { get;set;}
        public string Workstation_Name { get; set; }
        public string Workstation_ID { get; set; }
        public string Project_Code { get; set; }
        public string Project_Name { get; set; }
        public string Process_Name { get; set; }
        public string Process_ID { get; set; }

    }
}
