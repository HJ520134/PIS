using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public  class DefectRepairSolutionDTO : EntityDTOBase
    {
        public int Defect_RepairSolution_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int Fixtrue_Defect_UID { get; set; }
        public int Repair_Solution_UID { get; set; }
        public bool Is_Enable { get; set; }
        public string Fixture_DefectName { get; set; }
        public string Repair_SoulutionName { get; set; }
        public string PlantName { get; set; }
        public string BGName { get; set; }
        public string FunPlantName { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
    }
}
