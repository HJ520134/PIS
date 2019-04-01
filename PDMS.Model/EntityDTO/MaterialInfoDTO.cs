using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public  class MaterialInfoDTO: EntityDTOBase
    {
        public int Material_Uid { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string Assembly_Name { get; set; }
        public string Classification { get; set; }
        public Nullable<decimal> Unit_Price { get; set; }
        public int? Delivery_Date { get; set; }
        public int Update_No { get; set; }
        public Nullable<decimal> Update_Cost { get; set; }

        public int? Warehouse_Storage_UID { get; set; }
        public string Cost_Center { get; set; }
        public int? Maintenance_Cycle { get; set; }
        public int? Material_Life { get; set; }
        public int? Requisitions_Cycle { get; set; }
        public int? Sign_days { get; set; }
        public decimal? Daily_Consumption { get; set; }
        public int? Monthly_Consumption { get; set; }
        public bool? IsRework { get; set; }
        public bool? Is_Enable { get; set; }
        public string Warehouse_ID { get; set; }
        public string Rack_ID { get; set; }
        public string Storage_ID { get; set; }
        public string IsCheck { get; set; }
        public int? BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        /// <summary>
        /// 用于绑定
        /// </summary>
        public string PlantName { get; set; }
        /// <summary>
        /// 用于查询和编辑和绑定
        /// </summary>
        public  int? PlantId { get; set; }
        /// <summary>
        /// 用于查询的厂区ID
        /// </summary>
        public int? Plant_OrganizationUID { get; set; }
        /// <summary>
        /// 用于编辑的厂区ID
        /// </summary>
        public int? Organization_UID { get; set; }

        //用於移動成本計算
        public decimal? Last_Qty { get; set; }
    }
}
