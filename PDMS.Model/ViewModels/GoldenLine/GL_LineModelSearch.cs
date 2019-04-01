using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GL_LineModelSearch : BaseModel
    {
        public int? LineID { get; set; }
        public int? Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string LineName { get; set; }
        public int? CustomerID { get; set; }
        public int? Seq { get; set; }
        public bool? IsEnabled { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
        public int? CycleTime { get; set; }
    }
}
