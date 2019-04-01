using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
   public class IPQCQualityReportVM:BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public DateTime ProductDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ShiftTimeID { get; set; }
        public string TimeInterval { get; set; }
        public int VersionNumber { get; set; }//0 内部版，1 外部版本
        public int? languageID { get; set; }
    }
}
