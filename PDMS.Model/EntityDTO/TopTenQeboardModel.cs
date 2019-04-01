using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class TopTenQeboardModel
    {
        public int TopTenQeboard_UID { get; set; }
        public int FlowChartMaster_UID { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Time_Interval { get; set; }
        public string Product_Date { get; set; }
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public int CheckNum { get; set; }
        public int TotolNG { get; set; }
        public double TotalYidld { get; set; }
        public string DefectName { get; set; }
        public int NG { get; set; }
        public double Yield { get; set; }
        public string DefectType { get; set; }
        public string HistogramRate { get; set; } 

    }
}
