using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_LineShiftPerfDTO : EntityDTOBase
    {
        public int LSPID { get; set; }
        public int CustomerID { get; set; }
        public string OutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int LineID { get; set; }
        public int? AssemblyID { get; set; }
        public int PlanOutput { get; set; }
        public int StandOutput { get; set; }
        public int ActualOutput { get; set; }
        public decimal UPH { get; set; }
        public int LineStatus { get; set; }
        public int PlanDownTime { get; set; }
        public int UPDownTime { get; set; }
        public int RunTime { get; set; }
        public decimal ActualOutputVSPlan { get; set; }
        public decimal ActualOutputVSRealTimePlan { get; set; }
        public decimal ActualOutputVSStdOutput { get; set; }
        public decimal LineUtil { get; set; }
        public decimal CapacityLoading { get; set; }
        public decimal VAOLE { get; set; }

        //自定义
        public string CustomerName { get; set; }
        public string Shift { get; set; }
        public string LineName { get; set; }

        //每小时计算加总RealTimePlan=PlanOutput/10
        public int RealTimePlan { get; set; }
        //实时相减 Gap=Actual Output-Real Time Pan
        public int Gap { get; set; }

        public string ResponsibleUserName { get; set; }
    }
}
