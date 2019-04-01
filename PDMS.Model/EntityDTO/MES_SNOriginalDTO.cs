using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
   public class MES_SNOriginalDTO
    {
        public int MES_SNOriginal_UID { get; set; }
        public string SeriesNumber { get; set; }
        public string CustomerName { get; set; }
        public string StationName { get; set; }
        public System.DateTime Starttime { get; set; }
        public string Color { get; set; }
        public int MES_ProcessID { get; set; }
    }

    public class MESResultModel
    {
        public int MES_SNOriginal_UID { get; set; }
        public string SeriesNumber { get; set; }
        public string CustomerName { get; set; }
        public string StationName { get; set; }
        public System.DateTime StartTime { get; set; }
        public string MES_ProcessID { get; set; }
        public string MES_ProcessName { get; set; }
    }
}
