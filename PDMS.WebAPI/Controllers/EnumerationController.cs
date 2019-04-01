using Newtonsoft.Json;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class EnumerationController : ApiController
    {
        IEnumerationService enumerationPartService;

        public EnumerationController(IEnumerationService enumerationPartService)
        {
            this.enumerationPartService = enumerationPartService;
        }
        [HttpPost]
        public IHttpActionResult QueryEnummertaionAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<EnumerationModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<EnumerationModelSearch>>(jsonData);
            var result = enumerationPartService.QueryDto(searchModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetMachineDataSourceAPI(string enum_Type)
        {
            var result = enumerationPartService.GetMachineDataSource(enum_Type);
            return Ok(result);
        }
    }
}
