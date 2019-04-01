using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Fixture
{
    public class Fixture_ReturnVM
    {
        public List<PlantVM> Plants { get; set; }
        public List<BGVM> optypes { get; set; }
        public List<FunPlantVM> funplants { get; set; }
        public Fixture_Totake_DDTO fixturetotakedetail { get; set; }
        public List<SystemUserDTO> users { get; set; }

    }
}
