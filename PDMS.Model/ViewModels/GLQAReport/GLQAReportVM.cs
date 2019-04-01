using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GLQAReportVM : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public bool? IsEnabled { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }

    }
}
