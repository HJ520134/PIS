using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class ModelLineHRDTO:BaseModel
    {
        public int ModelLineHR_UID { get; set; }
        public string Station { get; set; }
        public int? Total { get; set; }
        public int? ShouldCome { get; set; }
        public int? ActualCome { get; set; }
        public int? VacationLeave { get; set; }
        public int? PersonalLeave { get; set; }
        public int? SickLeave { get; set; }
        public int? AbsentLeave { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public System.DateTime Created_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public System.DateTime Modified_Date { get; set; }
    }
}
