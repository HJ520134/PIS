using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_M_UseScanHistoryDTO : EntityDTOBase
    {
        public int Fixture_M_UseScanHistory_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int UseTimesTotal { get; set; }
        public bool IsValidSan { get; set; }
        public System.DateTime ScanDateTime { get; set; }

    }
}
