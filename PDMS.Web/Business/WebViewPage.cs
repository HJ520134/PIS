using Newtonsoft.Json;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model.ViewModels.Common;
using PDMS.Service.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace PDMS.Web.Business
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private Localizer _localizer;

        public Localizer T
        {
            get
            {
                if (_localizer == null)
                {
                    _localizer = (format, args) =>
                    {
                        var langUID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                        if (CacheHelper.Get(format + langUID) != null)
                        {
                            return new LocalizedString(CacheHelper.Get(format + langUID).ToString());
                        }
                        else
                        {
                            string apiUrl = string.Format("Common/GetResourceValueByNameAPI?languid={0}&resourceName={1}", langUID, format);
                            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
                            var result = responseMessage.Content.ReadAsStringAsync().Result;
                            result = JsonConvert.DeserializeObject(result).ToString();
                            CacheHelper.Set(format + langUID, result);
                            return new LocalizedString(result);
                        }
                    };
                }
                return _localizer;

            }
        }
    }
}