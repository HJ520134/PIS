using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class MeterialUpdateInfoDTO: EntityDTOBase
    {
        public int PartsUpdate_Uid { get; set; }
        public int Material_Uid { get; set; }
        public System.DateTime Update_DateTime { get; set; }
        public int Update_No { get; set; }
        public int EQP_Uid { get; set; }
        public Nullable<decimal> Update_Cost { get; set; }
        public int Repair_Uid { get; set; }
        public string Material_Name { get; set; }
        public string Assembly_Name { get; set; }
        public string Material_Id { get; set; }
        public Nullable<decimal> Unit_Price { get; set; }
        public Nullable<int> Modified_EQPUser_Uid { get; set; }
        public string Reason_Analysis { get; set; }
    }

    public class MeterialUpdateInfoWithWarehouseDTO : EntityDTOBase
    {
        //public int PartsUpdate_Uid { get; set; }
        public int Material_Uid { get; set; }
        //public System.DateTime Update_DateTime { get; set; }
        public int Update_No { get; set; }
        //public int EQP_Uid { get; set; }
        //public Nullable<decimal> Update_Cost { get; set; }
        public int Repair_Uid { get; set; }
        //public Nullable<int> Modified_EQPUser_Uid { get; set; }
        //public string Reason_Analysis { get; set; }

        //料号信息
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }

        //儲位信息
        public int? Warehouse_Storage_UID { get; set; }
        public int? Warehouse_Type_UID { get; set; }
        public int? Inventory_Qty { get; set; }
        public int? Be_Out_Qty { get; set; }

    }
}
