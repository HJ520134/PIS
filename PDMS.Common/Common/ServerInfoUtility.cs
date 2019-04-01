using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PDMS.Common.Common
{
    /// <summary>
    /// 伺服器資訊類別
    /// </summary>
    public class ServerInfoUtility
    {
        /// <summary>
        /// 網站根目錄
        /// </summary>
        public string RootPath
        {
            get { return ConfigurationManager.AppSettings["RootPath"].ToString(); }
        }

        /// <summary>
        /// File Server 根目錄
        /// </summary>
        public string ServerFileRoot
        {
            get { return ConfigurationManager.AppSettings["ServerFileRoot"].ToString(); }
        }

        /// <summary>
        /// File Server (上傳)
        /// </summary>
        public string ServerFileFolder
        {
            get { return ConfigurationManager.AppSettings["ServerFileFolder"].ToString(); }
        }

        /// <summary>
        /// WebPath
        /// </summary>
        public string WebPath
        {
            get { return ConfigurationManager.AppSettings["WebPath"].ToString(); }
        }

        /// <summary>
        /// WebApiPath
        /// </summary>
        public string WebApiPath
        {
            get { return ConfigurationManager.AppSettings["WebApiPath"].ToString(); }
        }

        /// <summary>
        /// Map根目錄
        /// </summary>
        /// <param name="strPath">虛擬路徑</param>
        /// <returns></returns>
        public string MapPath(string strPath)
        {
            if (System.Web.HttpContext.Current != null)
            {
                return System.Web.HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用   
            {
                strPath = strPath.Replace("~/", "");
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }

        /// <summary>
        /// 支持 web 和 service
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }

    }
}
