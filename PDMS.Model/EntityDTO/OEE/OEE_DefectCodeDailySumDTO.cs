using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_DefectCodeDailySumDTO
    {
        public int OEE_DefectCodeDailySum_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public int OEE_StationDefectCode_UID { get; set; }
        public int DefectNum { get; set; }
        public System.DateTime ProductDate { get; set; }
        public int ShiftTimeID { get; set; }
        public string DefectName { get; set; }
        public string DefectChineseName { get; set; }
    }
}
