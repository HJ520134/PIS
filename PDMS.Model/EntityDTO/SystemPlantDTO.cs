using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class SystemPlantDTO : EntityDTOBase
    {
        public int System_Plant_UID { get; set; }
        public string CCODE { get; set; }
        public string Plant { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Name_0 { get; set; }
        public string Name_1 { get; set; }
        public string Name_2 { get; set; }
        public string PlantManager_Name { get; set; }
        public string PlantManager_Tel { get; set; }
        public string PlantManager_Email { get; set; }
        public string Legal_Entity_EN { get; set; }
        public string Legal_Entity_ZH { get; set; }
        public string Address_EN { get; set; }
        public string Address_ZH { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> End_Date { get; set; }
        public string Coordinate { get; set; }
        //public SystemUserPlantDTO System_User_Plant { get; set; }
    }
}
