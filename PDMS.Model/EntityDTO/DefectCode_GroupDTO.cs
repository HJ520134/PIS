using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class DefectCode_GroupDTO : EntityDTOBase
    {
        /// <summary>
        /// 异常群组ID
        /// </summary>
        public int DefectCode_Group_UID { get; set; }
        /// <summary>
        /// 厂区ID
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// OPID
        /// </summary>
        public int BG_Organization_UID { get; set; }
        /// <summary>
        /// 功能厂ID
        /// </summary>
        public int? FunPlant_Organization_UID { get; set; }
        /// <summary>
        /// 异常群组代码
        /// </summary>
        public string DefectCode_Group_ID { get; set; }
        /// <summary>
        /// 异常群组名称
        /// </summary>
        public string DefectCode_Group_Name { get; set; }
        /// <summary>
        ///异常代码ID
        /// </summary>
        public int Fixtrue_Defect_UID { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Is_Enable { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? Created_Date { get; set; }
        /// <summary>
        /// 厂区名称
        /// </summary>
        public string PlantName { get; set; }
        /// <summary>
        /// OP名称
        /// </summary>
        public string OPName { get; set; }
        /// <summary>
        /// 功能厂名称
        /// </summary>
        public string FunPlantName { get; set; }
        /// <summary>
        /// 异常代码
        /// </summary>
        public string DefectCode_ID { get; set; }
        /// <summary>
        /// 异常名称
        /// </summary>
        public string DefectCode_Name { get; set; }
        /// <summary>
        /// 时间段开始
        /// </summary>
        public System.DateTime? End_Date_From { get; set; }
        /// <summary>
        /// 时间段结束
        /// </summary>
        public System.DateTime? End_Date_To { get; set; }

        public List<Fixture_DefectCodeDTO> Fixture_Defect_UIDs { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Createder { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        public string Modifieder { get; set; }
    }
}
