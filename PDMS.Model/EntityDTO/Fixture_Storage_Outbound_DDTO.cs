using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Storage_Outbound_DDTO : BaseModel
    {
        public int Fixture_Storage_Outbound_D_UID { get; set; }
        public int Fixture_Storage_Outbound_M_UID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public int Fixture_Warehouse_Storage_UID { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Outbound_Qty { get; set; }
        public string Remarks { get; set; }
        //出入库数量
        public decimal In_Out_Bound_Qty { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        //public decimal Inventory_Qty { get; set; }
        /// <summary>
        /// 出库待审核数量
        /// </summary>
        public decimal Be_Out_Qty { get; set; }
        /// <summary>
        /// 储位号
        /// </summary>
        public string Storage_ID { get; set; }
        /// <summary>
        /// 料架号
        /// </summary>
        public string Rack_ID { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string Fixture_Warehouse_ID { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Fixture_Warehouse_Name { get; set; }
        /// <summary>
        /// 料号
        /// </summary>
        public string Part_ID { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string Part_Name { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Part_Spec { get; set; }

    }
}
