using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GLHCActuaWM
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
        public int MondayHCActua { get; set; }
        public int TuesdayHCActua { get; set; }
        public int WednesdayHCActua { get; set; }
        public int ThursdayHCActua { get; set; }
        public int FridayHCActua { get; set; }
        public int SaterdayHCActua { get; set; }
        public int SundayHCActua { get; set; }
        public bool IsThisWork { get; set; }
        public string WeekDay { get; set; }
    }
}
