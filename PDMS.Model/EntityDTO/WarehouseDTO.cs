using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public  class WarehouseDTO: EntityDTOBase
    {
        public int Warehouse_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Plant{ get; set; }
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
        public int Warehouse_Storage_UID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public string WarehouseStorageDesc { get; set; }
        public string Modifier { get; set; }
        public int Modified_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public System.DateTime Modified_Date { get; set; }
        public int Plant_UID { get; set; }
    }

    public class WarehouseBaseDTO : EntityDTOBase
    {
        public int Warehouse_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string Warehouse_ID { get; set; }
        public bool Is_Enable { get; set; }
        public int Warehouse_Type_UID { get; set; }
        public string Name_ZH { get; set; }
        public string Name_EN { get; set; }
        public string Desc { get; set; }
    }

}
