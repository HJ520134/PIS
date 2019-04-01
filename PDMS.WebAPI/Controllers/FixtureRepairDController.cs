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
    public class FixtureRepairDController : ApiController
    {
        IFixtureRepairDService fixtureRepairDService;
        IFixtureService fixtureService;

        public FixtureRepairDController(IFixtureRepairDService fixtureRepairDService, IFixtureService fixtureService)
        {
            this.fixtureRepairDService = fixtureRepairDService;
            this.fixtureService = fixtureService;
        }

        [HttpPost]
        public IHttpActionResult QueryFixtureRepairDAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<Fixture_Repair_DModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<Fixture_Repair_DModelSearch>>(jsonData);
            var result = fixtureRepairDService.QueryDto(searchModel);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult ValidFixtureRepairStatusDAPI(int uid)
        {
            var validStatus = new List<string>();
            validStatus.Add("使用中In-PRD");
            validStatus.Add("未使用Non-PRD");
            var statusList = fixtureService.GetFixtureStatuDTO().Where(i => !validStatus.Contains(i.StatuName));
            var isValid = true;
            foreach (var status in statusList)
            {
                var searchModel = new QueryModel<Fixture_Repair_DModelSearch>();
                searchModel.Equal = new Fixture_Repair_DModelSearch() { Status = status.Status, Fixture_M_UID = uid };
                if (fixtureRepairDService.Query(searchModel).Count() > 0)
                {
                    isValid = false;
                    break;
                }
            }
            return Ok(isValid);
        }
    }
}
