using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_MachineInformationDTO : EntityDTOBase
    {
        public int OEE_MachineInfo_UID { get; set; }
        public string Machine_No { get; set; }
        public int FixtureNum { get; set; }
        public int Por_CT { get; set; }
        public int Actual_CT { get; set; }
        public int AvailableTime { get; set; }
        public System.DateTime WorkDate { get; set; }
    }

    public class MachineResultDTO
    {
        public string Result { get; set; }
        public string ResultMessage { get; set; }
    }
}
