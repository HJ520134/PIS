using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class StationStdActCTInfoDTO : EntityDTOBase
    {

        public string WorkCenter { get; set; }
        public string StationName { get; set; }
        public decimal StdRouteCT { get; set; } //in seconds
        public string AssemblyNumber { get; set; }
        public decimal CycleTime { get; set; } //in seconds       
        public bool IsEnabled { get; set; }
        public bool CustomerIsEnabled { get; set; }
        public bool LineIsEnabled { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public int ShiftTimeID { get; set; }
        public string OutputDate { get; set; }
    }
}
