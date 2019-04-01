using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Constants
{
    public static class ConstConstants
    {
        /// <summary>
        /// 模糊查询包含
        /// </summary>
        public const string Valid = "0";

        /// <summary>
        /// 模糊查询不包含
        /// </summary>
        public const string Invalid = "1";

        /// <summary>
        /// 模糊查询所有
        /// </summary>
        public const string All = "2";

        /// <summary>
        /// 下拉框Select的值
        /// </summary>
        public const string SelectAll = "All";

        public const string etransferProject = "PIS&eTransfer";

        //定义程序集名称
        public const string AssemblyName = "PDMS.Batch";

        public const string WebContainer = "firstContainer";

        //定义排程执行用户UID
        public const int AdminUID = 99999;

        public const string Batch_Log = "Batch_Log";

        public const string Has_Exception = "Has_Exception";

        public const string FixtureMaintenanceBatch = "週期保養展開計算排程作業";
        public const string FixtureMaintenanceBatch_Success = "週期保養展開計算排程作業執行成功";

        public const string FixtureNotMaintenanceBatch = "逾期保養檢查排程作業";
        public const string FixtureNotMaintenanceBatch_Success = "逾期保養檢查排程作業執行成功";

        public const string FixtureMaintenance = "7天保养记录转移历史表";
        public const string FixtureMaintenance_Success = "7天保养记录转移历史表執行成功";
    }
}
