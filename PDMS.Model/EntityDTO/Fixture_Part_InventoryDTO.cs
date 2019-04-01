using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Part_InventoryDTO : EntityDTOBase
    {
       
        public int Fixture_Part_Inventory_UID { get; set; }
        public int Fixture_Warehouse_Storage_UID { get; set; }
        public int Fixture_Part_UID { get; set; }
        //库存数量
        public decimal Inventory_Qty { get; set; }
        public string Remarks { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        ///<summary>
        /// 厂区
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// 厂区名称
        /// </summary>
        public string Plant { get; set; }
        /// <summary>
        /// OP类型ID
        /// </summary>
        public int BG_Organization_UID { get; set; }
        /// <summary>
        /// OP类型名称
        /// </summary>
        public string BG_Organization { get; set; }
        /// <summary>
        /// 功能厂ID
        /// </summary>
        public int? FunPlant_Organization_UID { get; set; }
        /// <summary>
        /// 功能厂名称
        /// </summary>
        public string FunPlant_Organization { get; set; }
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
        /// <summary>
        /// 出库数量
        /// </summary>
        public decimal Outbound_Qty { get; set; }
        /// <summary>
        /// 待审核数量
        /// </summary>
        public decimal Be_Out_Qty { get; set; }
        public string User_Name { get; set; }
    }
}
