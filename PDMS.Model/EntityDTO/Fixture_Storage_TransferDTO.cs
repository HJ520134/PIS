using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Storage_TransferDTO : BaseModel
    {
        /// <summary>
        /// 转移单主键UID
        /// </summary>
        public int Fixture_Storage_Transfer_UID { get; set; }
        /// <summary>
        /// 储位转移单
        /// </summary>
        public string Fixture_Storage_Transfer_ID { get; set; }
        /// <summary>
        /// 料号主键UID
        /// </summary>
        public int Fixture_Part_UID { get; set; }
        /// <summary>
        /// 出库储位主键UID
        /// </summary>
        public int Out_Fixture_Warehouse_Storage_UID { get; set; }
        /// <summary>
        /// 入库主键UID
        /// </summary>
        public int In_Fixture_Warehouse_Storage_UID { get; set; }
        /// <summary>
        /// 转移数量
        /// </summary>
        public decimal Transfer_Qty { get; set; }
        /// <summary>
        /// 申请人主键UID
        /// </summary>
        public int Applicant_UID { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public System.DateTime Applicant_Date { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public int Status_UID { get; set; }
        /// <summary>
        /// 审核人主键UID
        /// </summary>
        public int Approver_UID { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string Approver { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public System.DateTime Approver_Date { get; set; }
        /// <summary>
        /// 入库储位号
        /// </summary>
        public string In_Storage_ID { get; set; }
        /// <summary>
        /// 入库料架号
        /// </summary>
        public string In_Rack_ID { get; set; }
        /// <summary>
        /// 入库仓库编码
        /// </summary>
        public string In_Fixture_Warehouse_ID { get; set; }
        /// <summary>
        /// 入库仓库名称
        /// </summary>
        public string In_Fixture_Warehouse_Name { get; set; }
        /// <summary>
        /// 出库储位号
        /// </summary>
        public string Out_Storage_ID { get; set; }
        /// <summary>
        /// 出库料架号
        /// </summary>
        public string Out_Rack_ID { get; set; }
        /// <summary>
        /// 出库仓库编码
        /// </summary>
        public string Out_Fixture_Warehouse_ID { get; set; }
        /// <summary>
        /// 出库仓库名称
        /// </summary>
        public string Out_Fixture_Warehouse_Name { get; set; }
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
        /// 审核状态
        /// </summary>
        public string Status { get; set; }
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
        /// 库存数量
        /// </summary>
        public decimal Inventory_Qty { get; set; }
    }
}
