using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public  class StorageOutboundDDTO: EntityDTOBase
    {
        public int Storage_Outbound_D_UID { get; set; }
        public int Storage_Outbound_M_UID { get; set; }
        public int Material_Uid { get; set; }
        public int Warehouse_Storage_UID { get; set; }
        public decimal Outbound_Qty { get; set; }
        public Nullable<decimal> Outbound_Price { get; set; }
    }

    public class RepairOutboundDTO : EntityDTOBase
    {
        public int Repair_Uid { get; set; }
        public int Material_Uid { get; set; }
        public string Material_Id { get; set; }
        public int Update_No { get; set; }
        public decimal Sum_Outbound_Qty { get; set; }
    }
}
