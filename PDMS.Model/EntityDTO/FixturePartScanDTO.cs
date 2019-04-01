using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class FixturePartScanDTO : EntityDTOBase
    {
        public int Fixture_M_UID { get; set; }
        public int Fixture_Part_Setting_D_UID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public string Part_ID { get; set; }
        public string Part_Name { get; set; }
        public string Part_Spec { get; set; }
        /// <summary>
        /// 治具配比数量
        /// </summary>
        public decimal Fixture_Part_Qty { get; set; }
        /// <summary>
        /// 治具使用寿命数量
        /// </summary>
        public int? Fixture_Part_Life_UseTimes { get; set; }

        /// <summary>
        /// 治具已经使用寿命数量
        /// </summary>
        public int UseTimes { get; set; }
        /// <summary>
        /// 是否更换
        /// </summary>
        public bool IsReplace { get; set; }
        /// <summary>
        /// 是否管控
        /// </summary>
        public bool IsUseTimesManagement { get; set; }

    }
}
