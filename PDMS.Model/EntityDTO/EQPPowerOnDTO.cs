using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class EQPPowerOnDTO : EntityDTOBase
    {
        public int EQP_PowerOn_UID { get; set; }
        public int EQP_Type_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime PowerOn_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime PowerOn_DateForShow { get; set; }
        public string PowerOnDateString { get; set; }
        public int Daily_PowerOn_Qty { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string FunPlant_Organization_UID { get; set; }
        public int FunPlantUID { get; set; }
        public string Plant { get; set; }
        public string OPType { get; set; }
        public string Funplant { get; set; }
        public string EQP_Type1 { get; set; }
        public int BG_Organization_UID { get; set; }
        public int Plant_UID { get; set; }
    }
}
