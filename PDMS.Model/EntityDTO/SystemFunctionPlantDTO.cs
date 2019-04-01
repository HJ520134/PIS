using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model
{
    public class SystemFunctionPlantDTO : EntityDTOBase
    {
        public int System_FunPlant_UID { get; set; }
        public int System_Plant_UID { get; set; }
        public string OP_Types { get; set; }
        public string FunPlant { get; set; }
        public string FunPlant_Manager { get; set; }
        public string FunPlant_Contact { get; set; }
        public int OPType_OrganizationUID { get; set; }
        public int  FunPlant_OrganizationUID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public Nullable<System.DateTime> End_Date { get; set; }

    }
}
