using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Exception
{
    public class ExceptionVM
    {
        public List<PlantVM> Plants { get; set; }
        public List<BGVM> optypes { get; set; }
        public List<FunPlantVM> funplants { get; set; }

        public List<SystemUserDTO> users { get; set; }

        public List<Depts> ExceDepts { get; set; }

        public List<Line> Lines { get; set; }
        public List<ShiftTime> Shifts { get; set; }


    }
}
