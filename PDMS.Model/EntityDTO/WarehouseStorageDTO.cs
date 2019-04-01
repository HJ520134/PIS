using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model
{
    public class WarehouseStorageDTO : EntityDTOBase
    {
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public int Warehouse_UID { get; set; }
        public string Warehouse_ID { get; set; }
        public string Name_ZH { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public string Desc { get; set; }
        public int Modified_UID { get; set; }
        public DateTime Modified_Date { get; set; }
        public int Warehouse_Type_UID { get; set; }
    }

    public class Warehouse_StorageDTO : EntityDTOBase
    {
        public int Warehouse_UID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public bool Is_Enable { get; set; }
        public string Desc { get; set; }
    
    }


}
