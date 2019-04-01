using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class BadProjectTopTenController : ApiController
    {
        IBadProjectTopTenServer BadProjectTopTenServer;

        public BadProjectTopTenController(IBadProjectTopTenServer BadProjectTopTenServer)
        {
            this.BadProjectTopTenServer = BadProjectTopTenServer;
        }

        [HttpGet]
        public IHttpActionResult GetQEboardSumTotalData(string Projects)
        {
            var entity = BadProjectTopTenServer.GteQEboardSumDetailData(Projects);
            return Ok(entity);
        }


        [HttpGet]
        public IHttpActionResult GetQEboardSumDetailData(string Projects)
        {
            var entity = BadProjectTopTenServer.GteQEboardSumDetailData(Projects);
            return Ok(entity);
        }
    }
}