using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Part_Order_DDTO : BaseModel
    {

        public int Fixture_Part_Order_D_UID { get; set; }
        public int Fixture_Part_Order_M_UID { get; set; }
        public int? Vendor_Info_UID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public System.DateTime Forcast_Complation_Date { get; set; }
        public bool Is_Complated { get; set; }
        public bool Del_Flag { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
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
        /// 实际交货数量
        /// </summary>
        public decimal Actual_Delivery_Qty { get; set; }
        /// <summary>
        /// 已入库总数量
        /// </summary>
        public decimal Inbound_Qty { get; set; }
    }
}
