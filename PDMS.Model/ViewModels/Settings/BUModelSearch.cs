using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    #region 往DB发送请求
    public class BUModelSearch : BaseModel
    {
        public int BU_M_UID { get; set; }

        public string BU_ID { get; set; }

        public string BU_Name { get; set; }

        public string BUManager_Name { get; set; }

        public DateTime? Reference_Date { get; set; }

        public string queryTypes { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }

    }
    #endregion

    #region 从DB中取出数据
    public class BUModelGet
    {
        public SystemBUMDTO SystemBUMDTO { get; set; }

        public SystemUserDTO SystemUserDTO { get; set; }
    }
    #endregion
}
