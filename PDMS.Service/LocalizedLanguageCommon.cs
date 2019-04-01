using PDMS.Common.Helpers;
using PDMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    /// <summary>
    /// Service层获取多语言信息共通方法
    /// </summary>
    public static class LocalizedLanguageCommon
    {
        public static string GetLocaleStringResource(int languageID, string resourceName)
        {
            using (var context = new SPPContext())
            {
                if (CacheHelper.Get(resourceName + languageID) != null)
                {
                    return CacheHelper.Get(resourceName + languageID).ToString();
                }
                else
                {
                    string result = string.Empty;
                    var item = context.System_LocaleStringResource.Where(m => m.ResourceName == resourceName && m.System_Language_UID == languageID).FirstOrDefault();
                    if (item == null)
                    {
                        result = resourceName;
                    }
                    else
                    {
                        result = item.ResourceValue;
                        CacheHelper.Set(resourceName + languageID, result);
                    }
                    return result;
                }
            }
        }
    }
}
