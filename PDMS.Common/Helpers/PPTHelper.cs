using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Slides;
using System.IO;

namespace PDMS.Common.Helpers
{
    public static class PPTHelper
    {
        /// <summary>
        /// PPT文件另存为指定的HTML文件
        /// </summary>
        /// <param name="pptFileName">PPT文件名</param>
        /// <param name="htmlFileName">HTML文件名</param>
        /// <returns></returns>
        public static string PptToHtml(string pptFileName, string htmlFileName)
        {
            //只转换扩展名为ppt,pptx 的文件
            var pptExtensionList = new List<string>() { ".ppt", ".pptx" };
            string extensionName = Path.GetExtension(pptFileName).ToLower();
            if (pptExtensionList.Contains(extensionName))
            {
                Presentation ppt = new Presentation(pptFileName);
                if (File.Exists(htmlFileName))
                {
                    //删除文件
                    File.Delete(htmlFileName);
                }
                ppt.Save(htmlFileName, Aspose.Slides.Export.SaveFormat.Html);
                return htmlFileName;
            }
            else
            {
                //未转换成功
                return null;
            }
        }

        /// <summary>
        /// PPT文件另存为同名的HTML文件
        /// </summary>
        /// <param name="pptFileName">PPT文件名</param>
        /// <returns></returns>
        public static string PptToHtml(string pptFileName)
        {
            var dir = Path.GetDirectoryName(pptFileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pptFileName);
            var htmlFileName = Path.Combine(dir, string.Format("{0}.html", fileNameWithoutExtension));
            return PptToHtml(pptFileName, htmlFileName);
        }

        /// <summary>
        /// PPT文件读取html 字符串
        /// </summary>
        /// <param name="pptFileName">PPT 文件名</param>
        /// <returns></returns>
        public static string ReadPptToHtml(string pptFileName) {
            var htmlFileName = PptToHtml(pptFileName);
            if (!string.IsNullOrWhiteSpace(htmlFileName) )
            {
                using (var fsRead = new FileStream(htmlFileName,FileMode.Open))
                {
                    int fsLen = (int)fsRead.Length;
                    byte[] heByte = new byte[fsLen];
                    int r = fsRead.Read(heByte, 0, heByte.Length);
                    string myStr = Encoding.UTF8.GetString(heByte);
                }
                //删除文件
                File.Delete(htmlFileName);
            }
            return null;
        }
    }
}
