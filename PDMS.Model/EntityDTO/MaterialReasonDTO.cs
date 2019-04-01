using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class MaterialReasonDTO:BaseModel
    {
        public int Material_Reason_UID { get; set; }
        public int Material_UID { get; set; }
        public string Reason { get; set; }
        public int Modified_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }

        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string ModifiedUser { get; set; }
        public int? PlantId { get; set; }
    }
}
