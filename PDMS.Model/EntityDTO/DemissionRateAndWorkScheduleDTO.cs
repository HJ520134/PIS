using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class DemissionRateAndWorkScheduleDTO : BaseModel
    {
        public int DemissionRateAndWorkSchedule_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Organization_Name { get; set; }
        public int BG_Organization_UID { get; set; }
        public string OPType { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public System.DateTime Product_Date { get; set; }
        public decimal DemissionRate_NPI { get; set; }
        public decimal DemissionRate_MP { get; set; }
        public Nullable<int> NPI_RecruitStaff_Qty { get; set; }
        public Nullable<int> MP_RecruitStaff_Qty { get; set; }
        public string WorkSchedule { get; set; }
        public System.DateTime? Created_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime? Modified_Date { get; set; }
        public int Modified_UID { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int LanguageID { get; set; }
        public string Product_Phase { get; set; }
    }
}
