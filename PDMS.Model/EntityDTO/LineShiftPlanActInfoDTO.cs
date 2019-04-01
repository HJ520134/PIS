using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class LineShiftPlanActInfoDTO : EntityDTOBase
    {
        public int StationID { get; set; }
        public string StationName { get; set; }
        public bool IsEnabled { get; set; }
        public int LineID { get; set; }
        public string LineName { get; set; }
        public bool LineIsEnabled { get; set; }
        public int CustomerID { get; set; }
        public string ProjectName { get; set; }
        public string MESProjectName { get; set; }
        public bool CustomerIsEnabled { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }        
        public int ActualOutput { get; set; }
        public DateTime OutputDate { get; set; }
        public string StrOutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int HourIndex { get; set; }
        public string HourQuantum { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public  decimal StationSMH { get; set; }
    }
}
