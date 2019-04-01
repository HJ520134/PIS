using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class MachineDetailVM : BaseModel
    {
        public List<SystemOrgDTO> Orgs { get; set; }
        public List<PlantVM> Plants { get; set; }
        public List<string> MachineDataSources { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }
   
}
