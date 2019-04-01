using Newtonsoft.Json;
using PDMS.Common.Constants;
using PDMS.Model;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace PDMS.Core
{
    public class APIHelper
    {
        /// <summary>
        /// Call api via post action
        /// if process get unauthorized or other status code, throw an excepiton
        /// </summary>
        /// <param name="model">view model</param>
        /// <param name="requestURL">web api route</param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static HttpResponseMessage APIPostAsync(BaseModel model, string requestURL)
        {
            HttpResponseMessage responMessage;
            string apiUrl = WebAPIPath + requestURL;

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(model));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var ticket = GetUserToken();
                if (!string.IsNullOrEmpty(ticket))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SPPAuthentication", ticket);
                }
                responMessage = client.PostAsync(apiUrl, content).Result;
            }
            if (!responMessage.IsSuccessStatusCode)
            {
                CheckResponse(responMessage);
            }

            return responMessage;
        }

        /// <summary>
        /// Call api via post action
        /// if process get unauthorized or other status code, throw an excepiton
        /// </summary>
        /// <param name="string">json string</param>
        /// <param name="requestURL">web api route</param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static HttpResponseMessage APIPostAsync(string postContent, string requestURL)
        {
            HttpResponseMessage responMessage;
            string apiUrl = WebAPIPath + requestURL;

            using (var client = new HttpClient())
            {
                var content = new StringContent(postContent);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var ticket = GetUserToken();
                if (!string.IsNullOrEmpty(ticket))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SPPAuthentication", ticket);
                }
                responMessage = client.PostAsync(apiUrl, content).Result;
            }
            if (!responMessage.IsSuccessStatusCode)
            {
                CheckResponse(responMessage);
            }

            return responMessage;
        }

        /// <summary>
        /// get paged items via post action
        /// if process get unauthorized or other status code, throw an excepiton
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <param name="requestURL"></param>
        /// <returns></returns>
        public static HttpResponseMessage APIPostAsync(BaseModel model, Page page, string requestURL)
        {
            HttpResponseMessage responMessage;
            string apiUrl = WebAPIPath + requestURL;

            using (var client = new HttpClient())
            {
                var contentString = JsonConvert.SerializeObject(model).TrimEnd('}') + "," + JsonConvert.SerializeObject(page).TrimStart('{');
                var content = new StringContent(contentString);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var ticket = GetUserToken();
                if (!string.IsNullOrEmpty(ticket))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SPPAuthentication", ticket);
                }
                responMessage = client.PostAsync(apiUrl, content).Result;

                if (!responMessage.IsSuccessStatusCode)
                {
                    CheckResponse(responMessage);
                }

                return responMessage;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">dynamic 类型参数</param>
        /// <param name="requestURL"></param>
        /// <returns></returns>
        public static HttpResponseMessage APIPostDynamicAsync(dynamic model, string requestURL)
        {
            HttpResponseMessage responMessage;
            string apiUrl = WebAPIPath + requestURL;

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(model));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var ticket = GetUserToken();
                if (!string.IsNullOrEmpty(ticket))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SPPAuthentication", ticket);
                }
                responMessage = client.PostAsync(apiUrl, content).Result;
            }
            if (!responMessage.IsSuccessStatusCode)
            {
                CheckResponse(responMessage);
            }

            return responMessage;
        }

        /// <summary>
        /// get api via get action
        /// if process get unauthorized or other status code, throw an excepiton
        /// </summary>
        /// <param name="requestRUL"></param>
        /// <returns></returns>
        public static HttpResponseMessage APIGetAsync(string requestRUL)
        {
            HttpResponseMessage responMessage;
            var apiUrl = WebAPIPath + requestRUL;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var ticket = GetUserToken();

                if (!string.IsNullOrEmpty(ticket))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SPPAuthentication", ticket);
                }

                responMessage = client.GetAsync(apiUrl).Result;


                if (!responMessage.IsSuccessStatusCode)
                {
                    CheckResponse(responMessage);
                }

                return responMessage;
            }
        }

        internal static string GetUserToken()
        {
            if (HttpContext.Current.Session[SessionConstants.LoginTicket] != null)
            {
                return HttpContext.Current.Session[SessionConstants.LoginTicket].ToString();
            }
            else
            {
                var cookie = HttpContext.Current.Request.Cookies[SessionConstants.LoginTicket];
                if (cookie != null)
                {
                    //session is invalid, write again with info from cookie
                    HttpContext.Current.Session[SessionConstants.LoginTicket] = cookie.Value;
                    return cookie.Value;
                }
                else
                {
                    //cookie is invalid, relogin.
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// handle unexcept response code
        /// </summary>
        /// <param name="statusCode"></param>
        private static void CheckResponse(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new HttpException(401, "Unauthorized");
                case HttpStatusCode.BadRequest:
                    throw new HttpException(400, "BadRequest");
                case HttpStatusCode.NotFound:
                    break;
                default:
                    throw new HttpException(
                                (int)response.StatusCode,
                                string.Format("Opps, some error accour to load remote data.[StatusCode:{0}].[Error Message:{1}]",
                                response.StatusCode.ToString(), response.Content.ReadAsStringAsync().Result)
                          );
            }
        }

        /// <summary>
        /// configured in MVC config
        /// </summary>
        private static string WebAPIPath
        {
            get { return ConfigurationManager.AppSettings["WebApiPath"].ToString(); }
        }

        public static HttpResponseMessage APIGetAsync(string webAPIPath , string requestRUL)
        {
            HttpResponseMessage responMessage;
            var apiUrl = webAPIPath + requestRUL;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                responMessage = client.GetAsync(apiUrl).Result;


                if (!responMessage.IsSuccessStatusCode)
                {
                    CheckResponse(responMessage);
                }

                return responMessage;
            }
        }

    }
}
