using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Common
{
    /// <summary>
    /// EventLog 的摘要描述
    /// </summary>
    public class EventLog
    {
        /// <summary>
        /// 檔案路徑
        /// </summary>
        public static string FilePath { get; set; }

        /// <summary>
        /// 寫入
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="arg">arg</param>
        public static void Write(string format, params object[] arg)
        {
            Write(string.Format(format, arg));
        }

        /// <summary>
        /// 寫入訊息
        /// </summary>
        /// <param name="message">訊息</param>
        public static void Write(string message)
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                FilePath = Directory.GetCurrentDirectory();
            }
            string filename = FilePath +
                string.Format("\\{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd}.txt", DateTime.Now);
            FileInfo finfo = new FileInfo(filename);
            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }
            string writeString = string.Format("{0:yyyy/MM/dd HH:mm:ss} {1}",
                DateTime.Now, message) + Environment.NewLine;
            File.AppendAllText(filename, writeString, Encoding.Unicode);
        }

        /// <summary>
        /// Create Log
        /// </summary>
        /// <param name="isError">Is This Error Message</param>
        /// <param name="ntid">User NTID</param>
        /// <param name="type">SPP Or Schedule Log</param>
        /// <param name="message">Message</param>
        /// <param name="modulename">Module</param>
        /// <param name="functionname">Function</param>
        public static void CreateLog(bool isError, string ntid, string type, string message, string modulename, string functionname)
        {

            if (type.Trim().ToUpper() == "PIS")
            {
                var pis = Properties.Settings.Default.PISLogPath.ToString();
                FilePath = pis + "\\";
            }

            else if (type.Trim().ToUpper() == "SCHEDULE")
            {
                var sche = Properties.Settings.Default.ScheduleLogPath.ToString();
                var schedule = sche;
                FilePath = schedule + "\\";
            }

            string naming = "";

            if (isError)
            {
                naming = string.Format("{0:yyyyMMdd}_" + ntid.Trim() + ".txt", DateTime.Now);
            }

            else if (!isError)
            {
                naming = string.Format(modulename.Trim() + "_" + functionname.Trim() + "_{0:yyyyMMdd}.txt", DateTime.Now);
            }

            string filename = FilePath + string.Format(naming);
            FileInfo finfo = new FileInfo(filename);
            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }
            string writeString = string.Format("{0:yyyy/MM/dd HH:mm:ss} {1}", DateTime.Now, message) + Environment.NewLine;
            File.AppendAllText(filename, writeString, Encoding.Unicode);
        }
    }
}
