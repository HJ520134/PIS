using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GLBuildHCPlanVM
    {
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        //public int PlanOutput { get; set; }
        //public string OutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string CustomerName { get; set; }
        public string LineName { get; set; }
        public string Plant_Organization { get; set; }
        public string BG_Organization { get; set; }
        public string FunPlant_Organization { get; set; }
        public string ShiftTime { get; set; }
        public int Created_UID { get; set; }
        public string Createder { get; set; }
        public System.DateTime Created_Date { get; set; }
        public DateTime Modified_Date { get; set; }
        public string Modifieder { get; set; }
        public int Modified_UID { get; set; }
        public int MondayHCPlan { get; set; }
        public int TuesdayHCPlan { get; set; }
        public int WednesdayHCPlan { get; set; }
        public int ThursdayHCPlan { get; set; }
        public int FridayHCPlan { get; set; }
        public int SaterdayHCPlan { get; set; }
        public int SundayHCPlan { get; set; }
    }
}
