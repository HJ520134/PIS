using Newtonsoft.Json;
using PDMS.Common.Helpers;
using PDMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDMS.Web.Business
{
    public class LanguageCommon
    {
        public string GetLocaleStringResource(int languid, string resourceName)
        {
            if (CacheHelper.Get(resourceName + languid) != null)
            {
                return CacheHelper.Get(resourceName + languid).ToString();
            }
            else
            {
                var apiUrl = string.Format("Common/GetResourceValueByNameAPI?languid={0}&resourceName={1}", languid, resourceName);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject(result).ToString();
                return result;
            }
        }

    }
}