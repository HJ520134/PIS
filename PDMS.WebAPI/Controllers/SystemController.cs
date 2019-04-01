using PDMS.Service;
using PDMS.Model;
using PDMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using PDMS.Core.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;

namespace PDMS.WebAPI.Controllers
{
    public class SystemController : ApiControllerBase
    {
        ISystemService systemService;
        ISettingsService settingsService;
        ICommonService commonService;

        public SystemController(
            ISystemService systemService,
            ISettingsService settingsService,
            ICommonService commonService)
        {
            this.systemService = systemService;
            this.settingsService = settingsService;
            this.commonService = commonService;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFunctionsByUserUId(int uid, int LanguageId)
        {
            var menus = systemService.GetFunctionsByUserUId(uid, LanguageId);
            var menusDTO = AutoMapper.Mapper.Map<IEnumerable<SystemFunctionDTO>>(menus);
            return Ok(menusDTO);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public HttpResponseMessage GetMobileFunctionsByUserUId(int uid)
        {

            var menus = JsonConvert.SerializeObject(systemService.GetMobileFunctionsByUserUId(uid), 
                                            Formatting.None,
                                            new JsonSerializerSettings()
                                            {
                                                NullValueHandling = NullValueHandling.Ignore
                                            });

            StringContent sc = new StringContent(menus);
            sc.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage resp = new HttpResponseMessage();
            resp.Content = sc;

            return resp;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult HasPageQulification(string controller, string action)
        {
            var result = systemService.HasPageQulification(controller,action).ToString();
            return Ok(new Message { Content = result });
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult UnauthorizedElements(string ntid)
        {
            var unauthorizedElements = systemService.GetUnauthorizedElementsByNTId(ntid);
            return Ok(unauthorizedElements.ToArray());
        }

        [AllowAnonymous]
        public IHttpActionResult ValidateToken(string token)
        {
            var result = true;
            int accountId = 0;
            FormsAuthenticationTicket ticket=null;

            try
            {
                ticket = FormsAuthentication.Decrypt(token);
            }
            catch
            {
               result=false;
            }

            if (!int.TryParse(ticket.Name, out accountId))
            {
                result = false;
            }

            var systemUser = systemService.GetSystemUserByUId(accountId);

            result = systemUser == null ? false : systemUser.LoginToken == token;

            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult GetSystemValidFunctions()
        {
            return Ok(systemService.GetSystemValidFunctions());
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            string path = System.Web.HttpContext.Current.Request.MapPath("~\\coming-soon");
            //var stream = new FileStream(path, FileMode.Open);


            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            return Ok(imageDataURL);
            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            //return result;
            
        }
    }
}
