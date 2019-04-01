using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class UserPlantModelSearch : BaseModel
    {
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public string Plant { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Plant_Code { get; set; }
        public DateTime? Reference_Date { get; set; }
        public int? query_types { get; set; }
        public string Modified_By_NTID { get; set; }
        public DateTime? Modified_Date_From { get; set; }
        public DateTime? Modified_Date_End { get; set; }
    }

    public class UserPlantItem : EntityDTOBase
    {
        public int Account_UID { get; set; }
        public int User_Plant_UID { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public string Plant { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Plant_Code { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? End_Date { get; set; }
    }

    public class UserPlantEditModel : EntityDTOBase
    {
        public int Account_UID { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }

        public IEnumerable<UserPlantWithPlant> UserPlantWithPlants { get; set; }
    }

    public class UserPlantWithPlant : EntityDTOBase
    {
        public int System_Plant_UID { get; set; }
        public string Plant { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Plant_Code { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Plant_Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Plant_End_Date { get; set; }

        public int System_User_Plant_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime User_Plant_Begin_Date { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? User_Plant_End_Date { get; set; }

    }
}
