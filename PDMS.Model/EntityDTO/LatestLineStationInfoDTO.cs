using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class LatestLineStationInfoDTO : EntityDTOBase
    {
        public int StationID { get; set; }
        public string StationName { get; set; }
        public string MESStationName { get; set; }
        public int LineID { get; set; }
        public bool IsBirth { get; set; }
        public bool IsOutput { get; set; }
        public bool IsTest { get; set; }
        public int Seq { get; set; }
        public bool IsEnabled { get; set; }
        public int CustomerID { get; set; }
        public string ProjectName { get; set; }
        public string MESProjectName { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string LineName { get; set; }
        public string MESLineName { get; set; }
        public bool LineIsEnabled { get; set; }
        public decimal CycleTime { get; set; }
        public decimal LineCycleTime { get; set; }
        public int Planqty { get; set; }
        public DateTime? LineStartTime { get; set; }
        public string OutputDate { get; set; }
        public int ShiftTimeID { get; set; }

    }
}
