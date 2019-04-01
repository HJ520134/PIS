using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Constants
{
    public struct SessionConstants
    {
        public const string PageUnauthorizedElements = "PAGE_UNAUTHORIZED_ELEMENTS";
        public const string LoginTicket = "USER_LOGON_TICKET";
        public const string CurrentAccountUID = "CURRENT_ACCOUNTUID";
        public const string CurrentUserName = "CURRENT_USERNAME";
        public const string CurrentUserValidPlants = "Current_User_Valid_Plants";
        public const string CurrentUserValidBUMs = "Current_User_Valid_BUMs";
        public const string CurrentUserValidBUDs = "Current_User_Valid_BUDs";
        public const string CurrentUserValidOrgs = "Current_User_Valid_Orgs";
        public const string Functions = "SYSTEM_FUNCTIONS";
        public const string DataPermissions = "DATA_PERMISSIONS";
        public const string MHFlag_MulitProject = "MHFlag_MulitProject";

        /// <summary>
        /// HttpContext.Current键值
        /// </summary>
        public const string PISSessionContext = "PISSessionContext";

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public const string CurrentUserInfo = "CurrentUserInfo";

        /// <summary>
        /// 当前工作语言
        /// </summary>
        public const string CurrentWorkingLanguage = "CurrentWorkingLanguage";

        public const string CurrentLanguageId = "CurrentLanguageId";

        /// <summary>
        /// 所有语言信息
        /// </summary>
        public const string Languages = "Languages";

        /// <summary>
        /// 当前连接的DB
        /// </summary>
        public const string CurrentDB = "CurrentDB";

        public const string RootUrl = "RootUrl";

        //用户当前的多语言Key
        public const string LanguageUIDByUser = "LanguageUIDByUser";

        public const string UserSelect = "UserSelect";

        public const string PlayBoard = "PlayBoard";
        public const string OEE_RealStatusSerchParam = "OEE_RealStatusSerchParam";
        public const string OEE_PieSerchParam = "OEE_PieSerchParam";
        public const string OEE_MetricsSerchParam = "OEE_MetricsSerchParam";
        public const string OEE_MetricsPieSerchParam = "OEE_MetricsPieSerchParam";
        public const string OEE_LineSerchParam = "OEE_LineSerchParam";
        public const string IPQCQualityReport = "IPQCQualityReport";
    }
}
