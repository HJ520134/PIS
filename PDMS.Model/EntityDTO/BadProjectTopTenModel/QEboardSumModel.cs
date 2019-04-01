using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class QEboardSumModel
    {
        public int QEboadSum_UID { get; set; }
        public int FlowChartMaster_UID { get; set; }

        public string OneDirectTarget { get; set; }

        public string OneDirectTargetActual { get; set; }

        public string TwoDirectTarget { get; set; }

        public string TwoDirectTargetActual { get; set; }

        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public int OneCheck_QTY { get; set; }
        public int OneCheck_OK { get; set; }
        public int NGReuse { get; set; }
        public int NGReject { get; set; }
        public double OneTargetYield { get; set; }
        public double OneYield { get; set; }
        public int RepairOK { get; set; }
        public double SecondTargetYield { get; set; }
        public double SecondYield { get; set; }
    }
}
