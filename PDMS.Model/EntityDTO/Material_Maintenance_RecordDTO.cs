using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Material_Maintenance_RecordDTO : EntityDTOBase
    {
        public int Material_Maintenance_Record_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int Material_UID { get; set; }
        public string Material_Seq { get; set; }
        public bool? Is_Warranty { get; set; }
        public int Submit_UID { get; set; }
        public System.DateTime Submit_Date { get; set; }
        public Nullable<int> Judge_UID { get; set; }
        public Nullable<System.DateTime> Judge_Date { get; set; }
        public string Abnormal { get; set; }
        public System.DateTime Delivery_Date { get; set; }
        public System.DateTime Expected_Return_Date { get; set; }
        public Nullable<int> Acceptance_Staff_UID { get; set; }
        public Nullable<System.DateTime> Acceptance_Date { get; set; }
        public string Acceptance_Results { get; set; }
        public string Vendor { get; set; }
        public string Maintenance_Items { get; set; }
        public Nullable<decimal> Maintenance_Fees { get; set; }
        public Nullable<int> Maintenance_Days { get; set; }
        public string Notes { get; set; }
        public int Status_UID { get; set; }
        public Nullable<System.DateTime> Return_Date { get; set; }
        public Nullable<int> Return_UID { get; set; }
        public Nullable<System.DateTime> Return_Time { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }

        //自定义
        /// <summary>
        /// 厂区名称
        /// </summary>
        public string Plant_Organization_Name { get; set; }
        /// <summary>
        /// OP名称
        /// </summary>
        public string BG_Organization_Name { get; set; }
        /// <summary>
        /// 功能厂名称
        /// </summary>
        public string FunPlant_Organization_Name { get; set; }
        /// <summary>
        /// 提交人名称
        /// </summary>
        public string Submiter { get; set; }
        /// <summary>
        /// 判定人员名称
        /// </summary>
        public string  Judger { get; set; }
        /// <summary>
        /// 验收人员
        /// </summary>
        public string Acceptancer { get; set; }
        /// <summary>
        /// 回厂日期填写人员名称
        /// </summary>
        public string Returner { get; set; }
        /// <summary>
        /// 更新人员名称
        /// </summary>
        public string Modifieder { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 备品料号
        /// </summary>
        public string Material_ID { get; set; }
        /// <summary>
        /// 开始维修天数
        /// </summary>
        public int? StartMaintenance_Days { get; set; }
        /// <summary>
        /// 结束维修天数
        /// </summary>
        public int? EndMaintenance_Days { get; set; }
        /// <summary>
        /// 预计天数
        /// </summary>
        public int Expected_Return_Days { get; set; }
        /// <summary>
        /// 预计开始维修天数
        /// </summary>
        public int? StartExpected_Return_Days { get; set; }
        /// <summary>
        /// 预计结束始维修天数
        /// </summary>
        public int? EndExpected_Return_Days { get; set; }
        /// <summary>
        /// 送修开始时间
        /// </summary>
        public System.DateTime? StartDelivery_Date { get; set; }
        /// <summary>
        /// 送修结束时间
        /// </summary>
        public System.DateTime? EndDelivery_Date { get; set; }
        /// <summary>
        /// 送修回厂时间
        /// </summary>
        public System.DateTime? StartReturn_Date { get; set; }
        /// <summary>
        /// 送修回厂时间
        /// </summary>
        public System.DateTime? EndReturn_Date { get; set; }
        /// <summary>
        /// 预计开始维修时间
        /// </summary>
        public System.DateTime? StartExpected_Return_Date { get; set; }
        /// <summary>
        /// 预计结束维修时间
        /// </summary>
        public System.DateTime? EndExpected_Return_Date { get; set; }

    }
}
