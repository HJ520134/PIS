using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Settings
{
    #region 往DB发送请求
    public class BUDetailModelSearch : BaseModel
    {
        public int BU_D_UID { get; set; }

        public string BU_ID { get; set; }

        public string BU_Name { get; set; }

        public string BU_D_ID { get; set; }

        public string BU_D_Name { get; set; }

        public DateTime? Reference_Date { get; set; }

        public string queryTypes { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }

        public int Organization_UID { get; set; }

        public int CurrentUser { get; set; }
    }
    #endregion
    public class BUD_OrgSearch : BaseModel
    {
      
    }
    #region 从DB中取出数据
    public class BUDetailModelGet
    {
        public SystemBUDDTO SystemBUDDTO { get; set; }

        public SystemBUMDTO SystemBUMDTO { get; set; }

        public SystemUserDTO SystemUserDTO { get; set; }

    }
    #endregion
}
