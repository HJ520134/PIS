using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;

namespace PDMS.Model
{
    public class UpdateBoundVM: BaseModel
    {
        public List<EnumerationDTO> enums { get; set; }
        public List<MaterialInfoDTO> mats { get; set; }
        public List<WarehouseDTO> warsts { get; set; }
        public List<EQPUserTableDTO> users { get; set; }
        public List<SystemOrgDTO> Orgs { get; set; }
    }
    public class OutBoundInfo: EntityDTOBase
    {
        public int Storage_Outbound_M_UID { get; set; }
        public int Storage_Outbound_Type_UID { get; set; }
        public string Storage_Outbound_ID { get; set; }
        public int Repair_Uid { get; set; }
        public int Outbound_Account_UID { get; set; }
        public string Outbound_Account { get; set; }
        public int Modified_UID { get; set; }
        public List<OutBoundDetail> details { get; set; }
        public string Repair_id { get; set; }
        public string Desc { get; set; }
        public string desc2 { get; set; }
        public string Status { get; set; }

        public string Storage_Outbound_Type { get; set; }
        public Nullable<System.DateTime> Apply_Time { get; set; }
        public string EQP_Location { get; set; }
        public string Equipment { get; set; }
        public string OP_Types { get; set; }
        public string FunPlant { get; set; }
        public string Repair_Reason { get; set; }
    }
    public class OutBoundDetail: EntityDTOBase
    {
        public int Material_Uid { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public decimal Outbound_Qty {get;set;}
        public decimal Inventory_Qty { get; set; }
        public string mateial { get; set; }
        public string WarehouseStorage { get; set; }
        public decimal Be_Out_Qty { get; set; }
        public int PartType_UID { get; set; }
        public string PartType { get; set; }
    }
}
