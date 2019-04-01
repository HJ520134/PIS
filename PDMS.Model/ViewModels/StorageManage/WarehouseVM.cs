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
    public  class WarehouseVM: BaseModel
    {
        public List<SystemOrgDTO> Orgs { get; set; }
        public List<EnumerationDTO> Types { get; set; }
        public List<WarehouseDTO> Wars { get; set; }
        public List<PlantVM> Plants { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }

    public class WarehouseStorages : EntityDTOBase
    {
        public int  Warehouse_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Plant { get; set; }
        public int BG_Organization_UID { get; set; }

        public string BG_Organization { get; set; }

        public int FunPlant_Organization_UID { get; set; }

        public string FunPlant_Organization { get; set; }

        public string Warehouse_ID { get; set; }

        public int Warehouse_Type_UID { get; set; }

        public string Warehouse_Type { get; set; }

        public string Name_ZH { get; set; }

        public string Name_EN { get; set; }

        public string Desc { get; set; }

        public List<WarStorageDto> Storages { get; set; }
    }
    public class WarStorageDto
     {
        public int Warehouse_Storage_UID { get; set; }
        public int Warehouse_UID { get; set; }
        public string Warehouse_ID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public string Desc { get; set; }
        public int Modified_UID { get; set; }
        public DateTime Modified_Date { get; set; }
    }
}
