using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_Repair_DModelSearch : BaseModel
    {
        public int? Fixture_Repair_D_UID { get; set; }
        public int? Fixture_Repair_M_UID { get; set; }
        public int? Fixture_M_UID { get; set; }
        public int? Repair_Staff_UID { get; set; }
        public DateTime? Completion_Date { get; set; }
        public int? Status { get; set; }
        public int? Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}