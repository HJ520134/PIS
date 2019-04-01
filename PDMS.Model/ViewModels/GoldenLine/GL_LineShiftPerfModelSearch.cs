using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GL_LineShiftPerfModelSearch : BaseModel
    {
        public int? LSPID { get; set; }
        public int? CustomerID { get; set; }
        public string OutputDate { get; set; }
        public int? ShiftTimeID { get; set; }
        public int? LineID { get; set; }
        public int? AssemblyID { get; set; }
        public int? PlanOutput { get; set; }
        public int? StandOutput { get; set; }
        public int? ActualOutput { get; set; }
        public decimal? UPH { get; set; }
        public int? LineStatus { get; set; }
        public int? PlanDownTime { get; set; }
        public int? UPDownTime { get; set; }
        public int? RunTime { get; set; }
        public decimal? ActualOutputVSPlan { get; set; }
        public decimal? ActualOutputVSRealTimePlan { get; set; }
        public decimal? ActualOutputVSStdOutput { get; set; }
        public decimal? LineUtil { get; set; }
        public decimal? CapacityLoading { get; set; }
        public decimal? VAOLE { get; set; }
    }
}
