using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GL_BuildPlanModelSearch : BaseModel
    {
        public int? BuildPlanID { get; set; }
        public int? CustomerID { get; set; }
        public int? LineID { get; set; }
        public int? AssemblyID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? PlanOutput { get; set; }
        public int? BPSourceID { get; set; }
        public string OutputDate { get; set; }
        public int? ShiftTimeID { get; set; }
        public int? Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}
