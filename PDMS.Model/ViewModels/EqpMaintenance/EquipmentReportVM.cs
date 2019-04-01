using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class  EquipmentReportVM : BaseModel
    {
        public List<PlantVM> Plants { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
    }
}
