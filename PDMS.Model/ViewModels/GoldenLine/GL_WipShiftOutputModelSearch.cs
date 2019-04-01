using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GL_WIPShiftOutputModelSearch
    {
        public int? WSOID { get; set; }
        public int? CustomerID { get; set; }
        public System.DateTime? OutputDate { get; set; }
        public int? ShiftIndex { get; set; }
        public int? LineID { get; set; }
        public int? StationID { get; set; }
        public int? AssemblyID { get; set; }
        public int? LineStarTime { get; set; }
        public System.DateTime? StartTime { get; set; }
        public System.DateTime? EndTime { get; set; }
        public int? CycleTime { get; set; }
        public decimal? MaxStdRouteCT { get; set; }
        public int? PlanDownTime { get; set; }
        public int? UPDownTime { get; set; }
        public int? RunTime { get; set; }
        public int? StandOutput { get; set; }
        public decimal? UPH { get; set; }
        public decimal? SMH { get; set; }
        public int? StdHC { get; set; }
        public decimal? StdUPPH { get; set; }
        public int? ActualOutput { get; set; }
        public decimal? ActualSMH { get; set; }
        public int? ActualHC { get; set; }
        public decimal? ActualUPPH { get; set; }
        public decimal? ActualVAOLE { get; set; }
    }
}
