using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class WorkStationVM
    {
        public List<PlantVM> Plants { get; set; }
        public List<SystemProjectDTO> Projects { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }
}
