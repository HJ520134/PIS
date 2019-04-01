using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_Part_Demand_MModelSearch : BaseModel
    {
        public int? Fixture_Part_Demand_M_UID { get; set; }
        public int? Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public DateTime? Calculation_Date { get; set; }
        public DateTime? Calculation_Date_From { get; set; }
        public DateTime? Calculation_Date_To { get; set; }
        public DateTime? Demand_Date { get; set; }
        public DateTime? Demand_Date_From { get; set; }
        public DateTime? Demand_Date_To { get; set; }
        public int? Applicant_UID { get; set; }
        public DateTime? Applicant_Date { get; set; }
        public DateTime? Applicant_Date_From { get; set; }
        public DateTime? Applicant_Date_To { get; set; }
        public int? Status_UID { get; set; }
        public int? Approver_UID { get; set; }
        public DateTime? Approver_Date { get; set; }
    }
}
