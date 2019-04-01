using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Constants
{
    public struct FormatConstants
    {
        //24小時制，顯示秒數
        public const string DateTimeFormatString = "yyyy-MM-dd HH:mm:ss";

        //24小时制，显示到天
        public const string DateTimeFormatStringByDate = "yyyy-MM-dd";

        //24小时制，只显示到分数
        public const string DateTimeFormatStringByMin = "yyyy-MM-dd HH:mm";
    }
}
