using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_MachineStatusDTO
    {
        public int OEE_MachineStatus_UID { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public System.DateTime Product_Date { get; set; }
        public int ShiftTimeID { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public int StatusDuration { get; set; }
        public System.DateTime UpdateTime { get; set; }
    }


    public class OEE_MachineStatusD
    {
        public string MachineName { get; set; }
        public System.DateTime Product_Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTIme { get; set; }
        public int StatusID { get; set; }
        public int StatusDuration { get; set; }
        public System.DateTime UpdateTime { get; set; }
    }
}
