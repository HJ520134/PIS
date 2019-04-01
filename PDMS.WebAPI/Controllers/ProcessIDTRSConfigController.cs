using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Data;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Service;
using PDMS.Service.FlowChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class ProcessIDTRSConfigController : ApiControllerBase
    {
        IProcessIDTRSConfigService ProcessIDTRSConfigService;
        IFlowChartDetailService flowChartDetailService;
        // IMES_PIS_SyncFailedRecordService mES_PIS_SyncFailedRecordService;
        public ProcessIDTRSConfigController(IProcessIDTRSConfigService ProcessIDTRSConfigService, IFlowChartDetailService flowChartDetailService)
        {
            this.ProcessIDTRSConfigService = ProcessIDTRSConfigService;
            this.flowChartDetailService = flowChartDetailService;
            //this.mES_PIS_SyncFailedRecordService = mES_PIS_SyncFailedRecordService;
            //this.NewMesDataSyncService = NewMesDataSyncService;
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult SynchronizeMesInfoAPI(string currentDate, string currentInterval, string FlowChartMaster_UID)
        {
            MesSyncParam param = new MesSyncParam();
            param.currentDate = currentDate;
            param.currentInterval = currentInterval;
            param.FlowChartMaster_UID = int.Parse(FlowChartMaster_UID);
            return Ok(ProcessIDTRSConfigService.SynchronizeMesInfo(param));
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProcessDataByUIDAPI(string Process_UID)
        {
            return Ok(ProcessIDTRSConfigService.GetProcessDataByUID(int.Parse(Process_UID)));
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult DeleteProcessByUIDAPI(string Process_UID)
        {
            return Ok(ProcessIDTRSConfigService.DeleteProcessByUID(int.Parse(Process_UID)));
        }

        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public string AddOrEditProcessInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var model = JsonConvert.DeserializeObject<ProcessIDTransformConfigDTO>(jsonData);
            var plantstring = ProcessIDTRSConfigService.AddOrEditProcessInfo(model);
            return plantstring;
        }

        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public string AddProcessIDTRSConfigAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<List<ProcessIDTransformConfigDTO>>(jsonData);
            //调用接口匹配
            foreach (var item in list)
            {
                item.PIS_ProcessID = flowChartDetailService.GetFlowChart_DetailByID(item.FlowChart_Master_UID, item.Binding_Seq, item.Color);
            }

            var plantstring = ProcessIDTRSConfigService.AddProcessIDConfigInfo(list);
            return plantstring;
        }


        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProcessIDConfigDataAPI(dynamic data)
        {
            var jsonData = data.ToString();
            ProcessIDTransformConfigDTO searchModel = JsonConvert.DeserializeObject<ProcessIDTransformConfigDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = ProcessIDTRSConfigService.GetProcessIDConfigData(searchModel, page);
            return Ok(result);
        }

        /// <summary>
        /// 导出勾选配置信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportAllProcessInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ProcessIDTransformConfigDTO>(jsonData);
            var bus = ProcessIDTRSConfigService.ExportAllProcessInfo(searchModel);
            return Ok(bus);
        }


        /// <summary>
        /// 导出勾选的配置信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportPartProcessInfoAPI(string uids)
        {
            var result = ProcessIDTRSConfigService.ExportPartProcessInfo(uids);
            return Ok(result);
        }


        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public bool IsExist(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<List<ProcessIDTransformConfigDTO>>(jsonData);
            var result = ProcessIDTRSConfigService.IsExist(list);
            return result;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QuerySyncFailedRecordAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MES_PIS_SyncFailedRecordDTO>(jsondata);
            var result = ProcessIDTRSConfigService.QuerySyncFailedRecord(searchModel, page);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        [HttpGet]
        public string DeleteMES_PISProcessConfigAPI(string json)
        {
            List<int> idList = JsonConvert.DeserializeObject<List<int>>(json);
            var result = ProcessIDTRSConfigService.DeleteProcessConfig(idList);
            return result;
        }


        //[AcceptVerbs("Post")]
        //[IgnoreDBAuthorize]
        //public IHttpActionResult SynchronizeMesInfoAPI(dynamic data)
        //{
        //    var jsonData = data.ToString();
        //    var list = JsonConvert.DeserializeObject<MesSyncParam>(jsonData);
        //    return Ok(MesDataSyncService.SynchronizeMesInfo(list));
        //}


        [HttpGet]
        public IHttpActionResult GetMES_PIS_SyncFailedRecordDTOByIDAPI(int MES_PIS_SyncFailedRecord_UID)
        {
            var result = ProcessIDTRSConfigService.GetMES_PIS_SyncFailedRecordDTOByID(MES_PIS_SyncFailedRecord_UID);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateSyncFailedLogAPI(MES_PIS_SyncFailedRecordDTO logModel)
        {
            var result = ProcessIDTRSConfigService.updateSyncFailedLog(logModel);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult IsSyneProjectNameAPI()
        {
            var result = ProcessIDTRSConfigService.GetNeedSysncProjectName();
            return Ok(result);
        }
    }
}