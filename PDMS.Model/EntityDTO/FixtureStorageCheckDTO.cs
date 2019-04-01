using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class FixtureStorageCheckDTO : BaseModel
    {
        public int Fixture_Storage_Check_UID { get; set; }
        public string Fixture_Storage_Check_ID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public int Fixture_Warehouse_Storage_UID { get; set; }
        public decimal Old_Inventory_Qty { get; set; }
        public decimal Check_Qty { get; set; }
        public int Check_Status_UID { get; set; }
        public int Applicant_UID { get; set; }
        public System.DateTime Applicant_Date { get; set; }
        public int Status_UID { get; set; }
        public int Approve_UID { get; set; }
        public System.DateTime Approve_Date { get; set; }
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
        /// 审核状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 申请者
        /// </summary>
        public string Applicant { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public string Approve { get; set; }
        /// <summary>
        /// 盘点更新状态
        /// </summary>
        public string Check_Status { get; set; }
    }
}
