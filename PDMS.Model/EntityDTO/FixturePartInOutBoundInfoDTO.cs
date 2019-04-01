using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class FixturePartInOutBoundInfoDTO : EntityDTOBase
    {

        /// <summary>
        /// 出入库主键ID
        /// </summary>
        public int Storage_In_Out_Bound_UID { get; set; }
        /// <summary>
        /// 出库单明细UID Fixture_Storage_Outbound_D的主键
        /// </summary>
        public int Storage_In_Out_Bound_D_UID { get; set; }
        /// <summary>
        /// 采购单UID
        /// </summary>
        public int? Fixture_Part_Order_M_UID { get; set; }
        /// <summary>
        /// 采购单编号
        /// </summary>
        public string Fixture_Part_Order { get; set; }
        /// <summary>
        /// 采购单明细主键
        /// </summary>
        public int Fixture_Part_Order_D_UID { get; set; }
        /// <summary>
        /// 发料单号
        /// </summary>
        public string Issue_NO { get; set; }
        /// <summary>
        /// 设备维修主键
        /// </summary>
        public int? Fixture_Repair_M_UID { get; set; }
        /// <summary>
        /// 设备维修单编号
        /// </summary>
        public string Fixture_Repair_ID { get; set; }
        /// <summary>
        /// 出入库类型UID
        /// </summary>
        public int Storage_In_Out_Bound_Type_UID { get; set; }
        /// <summary>
        /// 出入库单类型 1.出库单，2.入库单
        /// </summary>
        public string InOut_Type { get; set; }
        /// <summary>
        /// 出入库类型
        /// </summary>
        public string In_Out_Type { get; set; }
        /// <summary>
        /// 出入库单号
        /// </summary>
        public string Storage_In_Out_Bound_ID { get; set; }
        /// <summary>
        /// 审核状态主键
        /// </summary>
        public int Status_UID { get; set; }
        /// <summary>
        /// 料号主键
        /// </summary>
        public int Fixture_Part_UID { get; set; }
        /// <summary>
        /// 储位号主键
        /// </summary>
        public int Fixture_Warehouse_Storage_UID { get; set; }
        /// <summary>
        /// 出入库数量
        /// </summary>
        public decimal In_Out_Bound_Qty { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public int Applicant_UID { get; set; }

        public string Applicant { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public System.DateTime Applicant_Date { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public int Approve_UID { get; set; }
        public string Approve { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public System.DateTime Approve_Date { get; set; }
        /// <summary>
        /// 领用人UID
        /// </summary>
        public int? Outbound_Account_UID { get; set; }
        /// <summary>
        /// 领用人
        /// </summary>
        public string Outbound_Account { get; set; }
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
        /// 审核状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 采购数量
        /// </summary>
        public decimal Qty { get; set; }
        /// <summary>
        /// 采购单价
        /// </summary>
        public decimal Price { get; set; }
        //出库明细信息
        public List<Fixture_Storage_Outbound_DDTO> details { get; set; }
        //出库说明
        public string Remarks { get; set; }

        public string SentOut_Number { get; set; }
        public string SentOut_Name { get; set; }
        public System.DateTime? SentOut_Date { get; set; }
    }

}
