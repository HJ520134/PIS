using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
   public class Fixture_Warehouse_StorageDTO : BaseModel
    {
        /// <summary>
        /// 储位主键
        /// </summary>
        public int Fixture_Warehouse_Storage_UID { get; set; }
        /// <summary>
        /// 厂区主键
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// OP主键
        /// </summary>
        public int BG_Organization_UID { get; set; }
        /// <summary>
        /// 功能厂主键
        /// </summary>
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        /// <summary>
        /// 仓库主键
        /// </summary>
        public int Fixture_Warehouse_UID { get; set; }
        /// <summary>
        /// 仓库号
        /// </summary>
        public string Storage_ID { get; set; }
        /// <summary>
        /// 料架号
        /// </summary>
        public string Rack_ID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Is_Enable { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime Created_Date { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public int Modified_UID { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public System.DateTime Modified_Date { get; set; }
        /// <summary>
        /// 厂区名称
        /// </summary>
        public string Plant { get; set; }   
        /// <summary>
        /// OP类型名称
        /// </summary>
        public string BG_Organization { get; set; }
        /// <summary>
        /// 功能厂名称
        /// </summary>
        public string FunPlant_Organization { get; set; }
        /// <summary>
        /// 出库仓库编码
        /// </summary>
        public string Fixture_Warehouse_ID { get; set; }
        /// <summary>
        /// 出库仓库名称
        /// </summary>
        public string Fixture_Warehouse_Name { get; set; }
    }
}
