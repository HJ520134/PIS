using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class FixtureMaintenance_PlanDTO : EntityDTOBase
    {
        public int Maintenance_Plan_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Maintenance_Type { get; set; }
        public string Cycle_ID { get; set; }
        public int? Cycle_Interval { get; set; }
        public string Cycle_Unit { get; set; }
        public int? Lead_Time { get; set; }
        public System.DateTime? Start_Date { get; set; }
        public int? Tolerance_Time { get; set; }
        public System.DateTime? Last_Execution_Date { get; set; }
        public System.DateTime? Next_Execution_Date { get; set; }
        public bool Is_Enable { get; set; }
        public string Cycle_ALL { get; set; }



    }
}
