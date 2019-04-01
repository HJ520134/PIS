using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Batch
{
    public class FixtureBatchVM
    {
        public int Maintenance_Plan_UID { get; set; }
        public int Fixture_Maintenance_Profile_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Maintenance_Type { get; set; }
        public string Cycle_ID { get; set; }
        public int Cycle_Interval { get; set; }
        public string Cycle_Unit { get; set; }
        public int Lead_Time { get; set; }
        public System.DateTime Start_Date { get; set; }
        public int Tolerance_Time { get; set; }
        public Nullable<System.DateTime> Last_Execution_Date { get; set; }
        public Nullable<System.DateTime> Next_Execution_Date { get; set; }
        public bool Is_Enable { get; set; }
        public string Fixture_NO { get; set; }
        public string Version { get; set; }
        public DateTime EndDate { get; set; }
    }
}
