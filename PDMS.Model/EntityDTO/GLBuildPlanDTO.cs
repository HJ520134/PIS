using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GLBuildPlanDTO : EntityDTOBase
    {

        public int BuildPlanID { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int? AssemblyID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PlanOutput { get; set; }
        public int? BPSourceID { get; set; }
        public string OutputDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int? PlanHC { get; set; }
        public int? ActualHC { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string ProjectName { get; set; }
        public string MESProjectName { get; set; }
        public string LineName { get; set; }
        public string Plant_Organization { get; set; }
        public string BG_Organization { get; set; }
        public string FunPlant_Organization { get; set; }
        public string ShiftTime { get; set; }
        public int ImportType { get; set; }


    }
}
