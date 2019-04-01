using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
   public  class FixturePartWarehouseDTO : EntityDTOBase
    {
       /// <summary>
       /// 仓库UID
       /// </summary>
        public int Fixture_Warehouse_UID { get; set; }
        /// <summary>
        /// 厂区ID
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
        /// 仓库编码
        /// </summary>
        public string Fixture_Warehouse_ID { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Fixture_Warehouse_Name { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Is_Enable { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Createder { get; set; }
        /// <summary>
        /// 创建人UID
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime Created_Date { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改人UID
        /// </summary>
        public int Modified_UID { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public System.DateTime Modified_Date { get; set; }
        /// <summary>
        /// 厂区UID
        /// </summary>
        public int Plant_UID { get; set; }

        /// <summary>
        /// 储位UID
        /// </summary>
        public int Fixture_Warehouse_Storage_UID { get; set; }
        /// <summary>
        /// 储位号
        /// </summary>
        public string Storage_ID { get; set; }
        /// <summary>
        /// 料架号
        /// </summary>
        public string Rack_ID { get; set; }
   
    }
}
