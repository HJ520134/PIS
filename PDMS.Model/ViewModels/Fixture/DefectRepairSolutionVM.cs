using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.ViewModels
{

    public class DefectRepairSolutionVM : BaseModel
    {
        public List<PlantVM> Plants { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }

    public class DefectRepairSearch : BaseModel
    {
        public int? Defect_RepairSolution_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int? Fixtrue_Defect_UID { get; set; }
        public int? Repair_Solution_UID { get; set; }
        public string Fixture_DefectName { get; set; }
        public string Repair_SoulutionName { get; set; }
        public bool? Is_Enable { get; set; }
    }

    public class DefectRepairSVM : BaseModel
    {
        public string PlantName { get; set; }
        public string BGName { get; set; }
        public string FunPlantName { get; set; }
        public int Fixtrue_Defect_UID { get; set; }
        public int Repair_Solution_UID { get; set; }
        public bool Is_Enable { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }

    public class DefectRepriSVMList:BaseModel
    {
       public List<DefectRepairSVM> DefectRepairSVMLists { get; set; }
    }
}


   
