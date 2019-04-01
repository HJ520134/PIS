using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ExceptionTypeWithFlowchartDTO : EntityDTOBase
    {
        public int ExceptionTypeWithFlowchart_UID { get; set; }
        public int ExceptionType_UID { get; set; }
        public string FunPlant { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public Nullable<int> FlowChart_Master_UID { get; set; }
        public System.DateTime Creator_Date { get; set; }
        public string TypeClassify { get; set; }
        public int Process_Seq { get; set; }
        public Nullable<int> FlowChart_Detail_UID { get; set; }
    }
}
