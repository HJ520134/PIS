using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Helpers
{
    public class PathHelper
    {
        /// <summary>
        /// Set excel name
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static string SetGridExportExcelName(string moduleName)
        {
            moduleName = string.IsNullOrWhiteSpace(moduleName) ? string.Empty : moduleName.Replace(" ", "");
            return string.Format("{0}_{1}.xlsx", moduleName, DateTime.Now.ToString("yyyyMMddHHmmsss"));
        }

        /// <summary>
        /// Get the temp export directory in configuration file
        /// </summary>
        public static string GetGridExportExcelDirectory
        {
            get
            {
                var directory = ConfigurationManager.AppSettings["GridExportExcelDirectory"];

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return directory;
            }
        }
    }
}
