using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QAReportDaySummeryDTO : EntityDTOBase
    {
        public string FunPlant { get; set; }
        public string Process { set; get; }
        public int FirstCheck_Qty { set; get; }
        public int FirstOK_Qty { set; get; }
        public decimal FirstRejectionRate { set; get; }
        public decimal SecondRejectionRate { set; get; }
        public int Input { set; get; }
        public int SepcialAccept_Qty { set; get; }
        public int Shipment_Qty { set; get; }
        public decimal FirstTargetYield { set; get; }
        public decimal SecondTargetYield { set; get; }

        public string MaterialType { set; get; }
        public string Color { set; get; }
        public int Process_Seq { set; get; }
        public int FlowChart_Detail_UID { set; get; }

        public int NG { get; set; }

    }
}
