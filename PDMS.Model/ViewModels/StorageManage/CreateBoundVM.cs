using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
    public class CreateBoundVM: BaseModel
    {
        public List<WarehouseStorageDTO> warst { get; set; }
        public List<MaterialInfoDTO> mats { get; set; }
        public List<EnumerationDTO> enums { get; set; }
        public List<PlantVM> Plants { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }
}
