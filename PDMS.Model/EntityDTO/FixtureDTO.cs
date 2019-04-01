using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class FixtureDTO : EntityDTOBase
    {
        /// <summary>
        /// 治具主檔流水號
        /// </summary>
        public int Fixture_M_UID { get; set; }
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
        //专案代码
        public string Project_ID { get; set; }
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
        /// 使用次数总计
        /// </summary>
        public int? UseTimesTotal { get; set; }
        /// <summary>
        /// 创建日期时间
        /// </summary>
        public System.DateTime? Created_Date { get; set; }
        /// <summary>
        /// 创建者代號
        /// </summary>
        public int Created_UID { get; set; }
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
        /// 机台编号
        /// </summary>
        public string Machine_ID { get; set; }
        public string Machine_Name { get; set; }
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
        /// 和localDB 数据比对后的状态，和PIS系统中显示的状态无关
        /// </summary>
        public string MoniterStatusMark { get; set; }
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
        /// 是否编辑
        /// </summary>
        public bool IsEdit { get; set; }

        public string Process_ID { get; set; }
        public string WorkStation_ID { get; set; }
        public string Line_ID { get; set; }
        public string Vendor_ID { get; set; }

        public string Workshop_ID { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Createder { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        public string Modifieder { get; set; }

        public int AccountID { get; set; }

        /// <summary>
        /// 状态是否pass，通过MES数据库[FixtrueManage].[dbo].[FX_SNFixture]获取
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 扫码时间
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime ScanedTime { get; set;}
        
        public int FX_SNFixtureID { get; set; }
    }
    public class FixtureStatuDTO
    { 
      public   int Status { get; set; }
      public string StatuName { get; set; }
    }


}
