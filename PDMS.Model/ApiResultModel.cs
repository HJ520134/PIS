using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    /// <summary>
    /// 返回类型
    /// </summary>
    public class ApiResultModel
    {
        public int code { get; set; }

        /// <summary>
        /// 返回的数据
        /// </summary>
        public object data { get; set; }


        /// <summary>
        /// 错误消息
        /// </summary>
        public string message { get; set; }


        /// <summary>
        /// 是否成功
        /// </summary>
        public bool isSuccess { get; set; }

    }
}
