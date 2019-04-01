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
    public class MES_PIS_SyncFailedRecordController : ApiControllerBase
    {
        IMES_PIS_SyncFailedRecordService SyncFailedRecordService;

        public MES_PIS_SyncFailedRecordController(IMES_PIS_SyncFailedRecordService SyncFailedRecordService)
        {
            this.SyncFailedRecordService = SyncFailedRecordService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QuerySyncFailedRecordAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MES_PIS_SyncFailedRecordDTO>(jsondata);
            var result = SyncFailedRecordService.QuerySyncFailedRecord(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateSyncFailedLogAPI(MES_PIS_SyncFailedRecordDTO logModel)
        {
            var result = SyncFailedRecordService.updateSyncFailedLog(logModel);
            return Ok(result);
        }
    }
}