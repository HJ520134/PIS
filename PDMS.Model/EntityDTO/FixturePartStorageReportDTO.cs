using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class FixturePartStorageReportDTO : BaseModel
    {

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
        /// 初期库存
        /// </summary>
        public decimal Balance_Qty { get; set; }
        /// <summary>
        /// 本期入库
        /// </summary>
        public decimal In_Qty { get; set; }
        /// <summary>
        /// 本期出库
        /// </summary>
        public decimal Out_Qty { get; set; }
        /// <summary>
        /// 本期结存
        /// </summary>
        public decimal Last_Qty { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime Start_Date { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime End_Date { get; set; }

        public System.DateTime InOut_Date { get; set; }

        public System.DateTime intMonth { get; set; }
    }


    public class Fixture_Storage_InOut_DetailDTO : BaseModel
    {
        /// <summary>
        /// 库存明细ID
        /// </summary>
        public int Fixture_Storage_InOut_Detail_UID { get; set; }
        /// <summary>
        /// 出入库主键UID
        /// </summary>
        public int Fixture_Storage_InOut_UID { get; set; }
        /// <summary>
        /// 出入库类型
        /// </summary>
        public int InOut_Type_UID { get; set; }
        /// <summary>
        /// 配件UID
        /// </summary>
        public int Fixture_Part_UID { get; set; }
        /// <summary>
        /// 仓库UID
        /// </summary>
        public int Fixture_Warehouse_Storage_UID { get; set; }
        /// <summary>
        /// 出入库时间
        /// </summary>
        public System.DateTime InOut_Date { get; set; }
        /// <summary>
        /// 出入库数量
        /// </summary>
        public decimal InOut_Qty { get; set; }
        /// <summary>
        /// 结存数量
        /// </summary>
        public decimal Balance_Qty { get; set; }
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
    }
}
