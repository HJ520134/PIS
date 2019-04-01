using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class MaterialManageDTO: EntityDTOBase
    {
        public int Material_Apply_Uid { get; set; }
        public int Material_Uid { get; set; }
        public string Apply_Type { get; set; }
        public string Status { get; set; }
        public string Apply_Reason { get; set; }
        public int Apply_Number { get; set; }
        public int Actual_Number { get; set; }
        public string Used_Location { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Need_Date { get; set; }
        public int Applyer { get; set; }
        public string Applyer_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Apply_Date { get; set; }
        public int Acceprter { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Classification { get; set; }
        public string Reason_Detail { get; set; }
    }
}
