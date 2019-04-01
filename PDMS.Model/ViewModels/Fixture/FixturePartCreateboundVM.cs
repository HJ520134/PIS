using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
   public  class FixturePartCreateboundVM : BaseModel
    {
        public List<PlantVM> Plants { get; set; }
        public List<FixturePartCreateboundStatuDTO> FixturePartCreateboundStatus { get; set; }
        public List<FixturePartCreateboundStatuDTO> FixturePartInOutboundStatus { get; set; }
        public List<FixturePartCreateboundStatuDTO> FixturePartOutboundStatus { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
        public string Plant { get; set; }
        public int Plant_OrganizationUID { get; set; }
    }

}
