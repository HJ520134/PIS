using System;
using System.Web;

namespace PDMS.Common.Helpers
{
    public class CookiesHelper
    {
        /// <summary>
        ///  新增Cookies
        /// </summary>
        /// <param name="Response">Response</param>
        /// <param name="Cookieskey">Cookieskey</param>
        /// <param name="Value">Value</param>
        /// <param name="Expires">Expires过期时间，天</param>
        public static void AddCookies(HttpResponseBase Response, string Cookieskey, string Value, int Expires)
        {
            Response.Cookies[Cookieskey].Value = Value;
            Response.Cookies[Cookieskey].Expires = DateTime.Now.AddDays(Expires);
        }

        /// <summary>
        ///  根据Key获取值
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="Cookieskey"></param>
        /// <returns></returns>
        public static string GetValueByCookieskey(HttpRequestBase Request, string Cookieskey)
        {
            string cookiesValue = string.Empty;
            if (Request.Cookies[Cookieskey] != null)
            {
                cookiesValue = Request.Cookies[Cookieskey].Value;
            }
            return cookiesValue;
        }

        /// <summary>
        ///  删除Cookies
        /// </summary>
        /// <param name="Request">Request</param>
        /// <param name="Response">Response</param>
        /// <param name="Cookieskey">Cookieskey</param>
        public static void RemoveCookiesByCookieskey(HttpRequestBase Request, HttpResponseBase Response, string Cookieskey)
        {
            HttpCookie cookies = Request.Cookies[Cookieskey];
            if (cookies != null)
            {
                cookies.Expires = DateTime.Today.AddDays(-1);
                Response.Cookies.Add(cookies);
            }
        }
    }
}
