using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GoldenLineCTRecordDTO : EntityDTOBase
    {
        public int GoldenLineCTRecord_UID { get; set; }
        public int StationID { get; set; }
        public int ShiftTimeID { get; set; }
        public decimal CycleTime { get; set; }
        public string CycleTimeDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int LineID { get; set; }
        public int CustomerID { get; set; }
    }
}
