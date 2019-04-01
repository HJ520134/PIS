using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class CurrentStaffDTO : BaseModel
    {
        public int Current_Staff_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime ProductDate { get; set; }
        public string Product_Phase { get; set; }
        public int OP_Qty { get; set; }
        public int Monitor_Staff_Qty { get; set; }
        public int Technical_Staff_Qty { get; set; }
        public int Material_Keeper_Qty { get; set; }
        public int Others_Qty { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public int Modified_UID { get; set; }

        public string PlantName { get; set; }
        public string Optype { get; set;}
        public string FunPlant { get; set; }
        public int LanguageID { get; set; }

    }
}
