using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model.ViewModels
{
    public class MaterialVM:BaseModel
    {
        public IEnumerable<MaterialManageDTO> mats { get; set; }
        public IEnumerable<SystemUserDTO> users { get; set; }
        public List<string> locations { get; set; }
        public List<WarehouseStorageDTO> warst { get; set; }
        public List<SystemOrgDTO> Orgs { get; set; }
        public List<PlantVM> Plants { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }

        //價格權限管制
        public bool showPrice { get; set; }
        public bool editPrice { get; set; }
    }
}
