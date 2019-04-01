using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GL_ShiftTimeModelSearch : BaseModel
    {
        public int? ShiftTimeID { get; set; }
        public int? Plant_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public string Shift { get; set; }
        public string StartTime { get; set; }
        public string End_Time { get; set; }
        public bool? IsEnabled { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}
