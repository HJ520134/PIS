using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class FixturePartWarehouseVM : BaseModel
    {
        public List<SystemOrgDTO> Orgs { get; set; }
        public List<PlantVM> Plants { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }
    public class FixturePartWarehouseStorages : EntityDTOBase
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


        public List<FixturePartWarehouseStorageDTO> Storages { get; set; }
    }
    public class FixturePartWarehouseStorageDTO : EntityDTOBase
    {
        public int Fixture_Warehouse_Storage_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int Fixture_Warehouse_UID { get; set; }
        public string Storage_ID { get; set; }
        public string Rack_ID { get; set; }
        public string Remarks { get; set; }
        public bool Is_Enable { get; set; }
        public string Createder { get; set; }
        public int Created_UID { get; set; }
        public string Modifier { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }
}
