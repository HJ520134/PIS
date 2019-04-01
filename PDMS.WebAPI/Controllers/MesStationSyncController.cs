using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class MesStationSyncController : ApiControllerBase
    {
        INewMesDataSyncService NewMesDataSyncService;
        public MesStationSyncController(INewMesDataSyncService NewMesDataSyncService)
        {
            this.NewMesDataSyncService = NewMesDataSyncService;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult SynchronizeMesInfoAPI(string currentDate,string currentInterval)
        {
            MesSyncParam param = new MesSyncParam();
            param.currentDate = currentDate;
            param.currentInterval = currentInterval;
            return Ok(NewMesDataSyncService.SynchronizeMesInfo(param));
        }


        [HttpPost]
        public IHttpActionResult SynchronizeMesInfoPostAPI(dynamic data)
        {
            //var jsonData = data.ToString();
            //var list = JsonConvert.DeserializeObject<MesSyncParam>(jsonData);
            //return Ok(MesDataSyncService.SynchronizeMesInfo(list));
            return Ok();
        }
    }
}