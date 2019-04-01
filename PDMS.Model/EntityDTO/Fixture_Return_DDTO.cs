using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Return_DDTO:EntityDTOBase
    {

        #region 原始字段
        /// <summary>
        /// 治具归还DUID
        /// </summary>
        public int Fixture_Return_D_UID { get; set; }

        /// <summary>
        /// 治具领用明细档唯一标识
        /// </summary>
        public int Fixture_Totake_D_UID { get; set; }
        /// <summary>
        /// 治具归还主档唯一标识
        /// </summary>
        public int Fixture_Return_M_UID { get; set; }

        public int Fixture_M_UID { get; set; }
        /// <summary>
        /// 建档人员工号
        /// </summary>
        public int Created_UID { get; set; }

        public DateTime Created_Date { get; set; }
        /// <summary>
        /// 是否归还
        /// </summary>
        public int IS_Return { get; set; }
        /// <summary>
        /// 治具归还单号
        /// </summary>
        public string Return_NO { get; set; }
        #endregion

    }
}
