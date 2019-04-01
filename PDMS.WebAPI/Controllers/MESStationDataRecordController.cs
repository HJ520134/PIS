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

namespace PDMS.WebAPI.Controllers
{
    public class MESStationDataRecordController : ApiController
    {
        IMesDataSyncService MesDataSyncService;

        public MESStationDataRecordController(IMesDataSyncService MesDataSyncService)
        {
            this.MesDataSyncService = MesDataSyncService;
        }


        [HttpPost]
        public IHttpActionResult SynchronizeMesInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<MesSyncParam>(jsonData);
            return Ok(MesDataSyncService.SynchronizeMesInfo(list));
        }

        [HttpPost]
        public IHttpActionResult GetMESSyncReportAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<MES_StationDataRecordDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            return Ok(MesDataSyncService.GetMESSyncReport(list, page));
        }

        /// <summary>
        /// 导出所有两小时的报表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportAllTwoHourReportAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<MES_StationDataRecordDTO>(jsonData);
            var bus = MesDataSyncService.ExportAllTwoHourReport(searchModel);
            return Ok(bus);
        }

    }
}