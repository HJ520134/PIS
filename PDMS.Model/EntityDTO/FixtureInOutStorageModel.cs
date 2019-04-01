using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FixtureInOutStorageModel : EntityDTOBase
    {
        /// <summary>
        /// 出入库查询开始时间
        /// </summary>
        public System.DateTime? Start_Date { get; set; }

        /// <summary>
        /// 出入库查询结束时间
        /// </summary>
        public System.DateTime? End_Date { get; set; }

        public int Fixture_Storage_InOut_Detail_UID { get; set; }

        public int Fixture_Part_Inventory_UID { get; set; }
        public int Fixture_Warehouse_Storage_UID { get; set; }
        public int Fixture_Part_UID { get; set; }

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
        /// 出入库类型UID
        /// </summary>
        public int Storage_In_Out_Bound_Type_UID { get; set; }

        /// <summary>
        /// 出入库单号
        /// </summary>
        public string Storage_In_Out_Bound_ID { get; set; }

        /// <summary>
        /// 出入库单类型 1.出库单，2.入库单
        /// </summary>
        public string InOut_Type { get; set; }

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
        /// 出入库时间
        /// </summary>
        public DateTime In_Out_StorageTime { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public decimal In_Bound_Qty { get; set; }

        /// <summary>
        /// 出库数量
        /// </summary>
        public decimal Out_Bound_Qty { get; set; }

        /// <summary>
        /// 结存量
        /// </summary>
        public decimal Remaining_Bound_Qty { get; set; }

        /// <summary>
        /// 出入库类别
        /// </summary>
        public string In_Out_StorageType { get; set; }

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
        /// 更新者ID
        /// </summary>
        public int Modified_UID { get; set; }

        /// <summary>
        /// 更新者名字
        /// </summary>
        public string Modified_User_Name { get; set; }

        /// <summary>
        /// 更新者日期
        /// </summary>
        public DateTime Modified_Date { get; set; }
    }
}
