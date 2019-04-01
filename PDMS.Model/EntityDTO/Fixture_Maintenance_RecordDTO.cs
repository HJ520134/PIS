using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
   public class Fixture_Maintenance_RecordDTO : BaseModel
    {

        /// <summary>
        ///  治具保養設定檔流水號
        /// </summary>
        public int Fixture_Maintenance_Record_UID { get; set; }
        /// <summary>
        /// 治具保養設定檔流水號
        /// </summary>
        public int Fixture_Maintenance_Profile_UID { get; set; }
        /// <summary>
        /// 治具保養單編號
        /// </summary>
        public string Maintenance_Record_NO { get; set; }
        /// <summary>
        /// 治具主檔流水號
        /// </summary>
        public int Fixture_M_UID { get; set; }
        /// <summary>
        /// 保養日期
        /// </summary>
        public System.DateTime? Maintenance_Date { get; set; }
        /// <summary>
        /// 保養結果
        /// </summary>
        public int? Maintenance_Status { get; set; }
        /// <summary>
        /// 保養人員工号
        /// </summary>
        public string Maintenance_Person_Number { get; set; }
        /// <summary>
        /// 保養人員名称
        /// </summary>
        public string Maintenance_Person_Name { get; set; }
        /// <summary>
        /// 確認日期
        /// </summary>
        public System.DateTime? Confirm_Date { get; set; }
        /// <summary>
        /// 確認結果
        /// </summary>
        public int? Confirm_Status { get; set; }
        /// <summary>
        /// 確認人員流水號
        /// </summary>
        public int? Confirmor_UID { get; set; }
        /// <summary>
        /// 确认者
        /// </summary>
        public string Confirmor { get; set; }
        /// <summary>
        /// 创建者代號
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 创建日期时间
        /// </summary>
        public System.DateTime Created_Date { get; set; }
        /// <summary>
        /// 修改者流水號
        /// </summary>
        public int Modified_UID { get; set; }
        /// <summary>
        /// 修改日期時間
        /// </summary>
        public System.DateTime Modified_Date { get; set; }
        /// <summary>
        /// 廠區組織流水號
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// Business Group流水號
        /// </summary>
        public int BG_Organization_UID { get; set; }
        /// <summary>
        /// FunPlant_Organization_UID
        /// </summary>
        public int? FunPlant_Organization_UID { get; set; }
        /// <summary>
        /// 治具編號(圖號)
        /// </summary>
        public string Fixture_NO { get; set; }
        /// <summary>
        /// 版本號
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 治具流水序號
        /// </summary>
        public string Fixture_Seq { get; set; }
        /// <summary>
        /// 治具唯一編號
        /// </summary>
        public string Fixture_Unique_ID { get; set; }
        /// <summary>
        /// 治具名稱
        /// </summary>
        public string Fixture_Name { get; set; }
        /// <summary>
        /// 专案管理档流水號
        /// </summary>
        public int? Project_UID { get; set; }
        /// <summary>
        /// 治具機台資料檔流水號
        /// </summary>
        public int? Fixture_Machine_UID { get; set; }
        /// <summary>
        /// 供應商資料檔流水號
        /// </summary>
        public int? Vendor_Info_UID { get; set; }
        /// <summary>
        /// 生產線資料檔流水號
        /// </summary>
        public int? Production_Line_UID { get; set; }
        /// <summary>
        /// 线别
        /// </summary>
        public string Line_Name { get; set; }
        /// <summary>
        /// 生产车间ID
        /// </summary>
        public int? Workshop_UID { get; set; }
        /// <summary>
        /// 专案
        /// </summary>
        public string Project { get; set; }
        /// <summary>
        /// 生产车间 生产地点
        /// </summary>
        public string Workshop { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        public int? Workstation_UID { get; set; }
        /// <summary>
        /// 工站名称
        /// </summary>
        public string Workstation { get; set; }
        /// <summary>
        /// 制程ID
        /// </summary>
        public int? Process_Info_UID { get; set; }
        /// <summary>
        /// 制程名称
        /// </summary>
        public string Process_Info { get; set; }
        /// <summary>
        /// 治具狀態
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 治具短碼
        /// </summary>
        public string ShortCode { get; set; }
        /// <summary>
        /// 治具二維條碼
        /// </summary>
        public string TwoD_Barcode { get; set; }  
        /// <summary>
        /// 厂区名称
        /// </summary>
        public string PlantName { get; set; }
        /// <summary>
        /// OP类型名称
        /// </summary>
        public string OPName { get; set; }
        /// <summary>
        /// 功能厂名称
        /// </summary>
        public string FunPlantName { get; set; }
        /// <summary>
        /// 設備編號
        /// </summary>
        public string Equipment_No { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Vendor_Name { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatuName { get; set; }
        /// <summary>
        /// 机台类别
        /// </summary>
        public string Machine_Type { get; set; }
        /// <summary>
        /// 时间段开始
        /// </summary>
        public System.DateTime? End_Date_From { get; set; }
        /// <summary>
        /// 时间段结束
        /// </summary>
        public System.DateTime? End_Date_To { get; set; }
        /// <summary>
        /// 保养类别
        /// </summary>
        public string Maintenance_Type { get; set; }
        /// <summary>
        /// 保养计划类别ID
        /// </summary>
        public int? Maintenance_Plan_UID { get; set; }
        /// <summary>
        /// 机台代码
        /// </summary>
        public string WorkStation_ID { get; set; }
        public bool Is_Enable { get; set; }
        //周期类别
        public string Cycle_ALL { get; set; }
    }

    public class Fixture_Maintenance_RecordDTOCS : BaseModel
    {

        public string fixture_Maintenance_Record_UIDs { get; set; }
        public int NTID { get; set; }
        public string personNumber { get; set; }
        public string personName { get; set; }
        public DateTime date { get; set; }
        public int straus { get; set; }
        public int CurrentUserID { get; set; }

    }


}
