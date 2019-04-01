using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_RepairSolutionDTO : EntityDTOBase
    {
        //来自Fixture_RepairSolution
        public int Fixture_RepairSolution_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string RepairSolution_ID { get; set; }
        public string RepairSolution_Name { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }

        //自定义

        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
    }
}
