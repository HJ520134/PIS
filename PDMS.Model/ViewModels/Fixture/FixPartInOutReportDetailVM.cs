using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
   public class FixPartInOutReportDetailVM : BaseModel
    {
        public List<PlantVM> Plants { get; set; }
        public List<EnumerationDTO> enums { get; set; }
        public List<SystemOrgDTO> BGOrgs { get; set; }
        public List<FixturePartCreateboundStatuDTO> FixturePartInOutboundStatus { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }
}
