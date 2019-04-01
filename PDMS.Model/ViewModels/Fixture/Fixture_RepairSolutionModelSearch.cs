using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Fixture_RepairSolutionModelSearch : BaseModel
    {
        public int? Fixture_RepairSolution_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string RepairSolution_ID { get; set; }
        public string RepairSolution_Name { get; set; }
        public bool? Is_Enable { get; set; }
    }
}
