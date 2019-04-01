using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowChartIEMgDataDTO : EntityDTOBase
    {
       
        public DateTime IE_TargetDate { get; set; }
        public int ShiftTimeID { get; set; }

        public DateTime Product_Date { get; set; }
        public int FlowChart_IEData_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
         public DateTime TargetDate_Date { get; set; }
        public int ShiftTimeId { get; set; }
        public int IE_TargetEfficacy { get; set; }

        public int IE_DeptHuman { get; set; }
    }
}
