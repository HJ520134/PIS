using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PDMS.Common.Constants;
using System.Linq;
using PDMS.Core.Authentication;
using PDMS.Model.EntityDTO;


using PushBots.NET;
using PushBots.NET.Enums;
using PushBots.NET.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace PDMS.WebAPI.Controllers
{
	public class PushMessageController: ApiController
    {
        private readonly PushBotsClient _pushBotsClient;
        /// App Id
        private readonly string AppId = "5a3c99bda5d103619b086c02";
        /// Secret
        private readonly string Secret = "a470248c9ec203f93e01d1c5092b128c";

        public string Alias;
        public string Message;
        public PushMessageController()
        {
            _pushBotsClient = new PushBotsClient(AppId, Secret);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostPush([FromBody]PushBotsModel push)
        {
            try
            {
                var pushMessage = new BatchPush
                {
                    Message = push.Message,
                    Alias = push.Alias,
                    Sound = "",
                    Badge = "+1",
                    Platforms = new[] { Platform.Android, Platform.iOS }
                };
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, error) =>
                {
                    return true;
                };
                var result = await _pushBotsClient.Push(pushMessage);

                if (result.IsSuccessStatusCode)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result.ReasonPhrase);
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Couldn't find device with alias: "+result.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.GetBaseException().Message);
            }
        }
    }
}