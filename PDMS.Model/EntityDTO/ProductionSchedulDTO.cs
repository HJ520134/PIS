using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ProductionSchedulDTO : EntityDTOBase
    {
        public int Production_Schedul_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public DateTime Product_Date { get; set; }
        public int Input_Qty { get; set; }
        public int PlanType { get; set; }
        public decimal Target_Yield { get; set; }
        public DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        //自定义
        public int Process_Seq { get; set; }
        public string PlanTypeValue { get; set; }
        public string DayTypeValue { get; set; }
        public string Process { get; set; }
    }
}
