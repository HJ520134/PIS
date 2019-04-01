using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public  class MaterialInventoryDTO: EntityDTOBase
    {
        public int Material_Inventory_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_OrganizationUID { get; set; }
        //盘点导入时候用到的 
        public int FunPlant_Organization_UID { get; set; }
        public int Material_Uid { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public decimal Inventory_Qty { get; set; }
        public string Desc { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Classification { get; set; }
        public string Funplant { get; set; }
        public string Warehouse_ID { get; set; }
        public string Name_ZH { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public decimal Be_Out_Qty { get; set; }
        public int Warehouse_Type_UID { get; set; }

        public string BG { get; set; }
        public string  Warehouse_Type { get; set; }
        public decimal Total_Qty { get; set; }
        public string ModifyUser { get; set; }
        public int Plant_UID { get; set; }
        public List<MaterialInventoryDetailDTO> MaterialInventoryDetails { get; set; }
    }

    public class MaterialInventoryDetailDTO : EntityDTOBase
    {
        public int Material_Inventory_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int FunPlant_OrganizationUID { get; set; }
        //盘点导入时候用到的 
        public int FunPlant_Organization_UID { get; set; }
        public int Material_Uid { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public decimal Inventory_Qty { get; set; }
        public string Desc { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Classification { get; set; }
        public string Funplant { get; set; }
        public string Warehouse_ID { get; set; }
        public string Name_ZH { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public decimal Be_Out_Qty { get; set; }
        public int Warehouse_Type_UID { get; set; }

        public string BG { get; set; }
        public string Warehouse_Type { get; set; }
        public decimal Total_Qty { get; set; }
        public string ModifyUser { get; set; }
        public int Plant_UID { get; set; }
    }
}
