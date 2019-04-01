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
    public class VendorInfoVM: BaseModel
    {
        public List<PlantVM> Plants { get; set; }
        public List<BGVM> optypes { get; set; }
        public List<MaintenancePlanDTO> maintenanceplan { get; set; }
        public List<FunPlantVM> funplants { get; set; }
        public Fixture_Totake_DDTO fixturetotakedetail { get; set; }
        public List<SystemUserDTO> users { get; set; }

    }
}
