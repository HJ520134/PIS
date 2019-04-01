using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Service;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using PDMS.Model.EntityDTO.APP;
using PDMS.Model.EntityDTO;

namespace PDMS.WebAPI.Controllers
{
    public class APPController: ApiControllerBase
    {
        IAPPService APPService;

        public APPController(IAPPService APPService)
        {
            this.APPService = APPService;
        }

        [AllowAnonymous]
        public IHttpActionResult QueryEquipmentInfoReprotAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EquipmentReport>(jsonData);
            var page = new Page()
            {
                PageNumber = 0,
                PageSize = 100000
                };
            var bus = APPService.QueryEquipmentInfoReprot(searchModel, page);
            return Ok(bus);
        }

        [AllowAnonymous]
        public HttpResponseMessage addAppUserFavorites(userFav UserFav)
        {
            var result = APPService.addUserFav(UserFav);
            if (result == "")
            {
                return Request.CreateResponse(new AuthorizedLoginUser
                {
                    Account_UID = 1
                });
            }else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "");
            }

        }
    }

}