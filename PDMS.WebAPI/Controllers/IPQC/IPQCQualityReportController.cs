using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class IPQCQualityReportController : ApiControllerBase
    {
        IIPQCQualityService _IPQCQualityServer;
        public IPQCQualityReportController(IIPQCQualityService _IPQCQualityServer)
        {
            this._IPQCQualityServer = _IPQCQualityServer;
        }

        #region IPQC报表
        [HttpGet]
        public IHttpActionResult GetStationDTOAPI(int LineId)
        {
            var result = _IPQCQualityServer.GetStationDTOs(LineId);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult GetCurrentTimeIntervalAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<TimeIntervalParamModel>(jsonData);
            searchModel.TimeIntervalLength = 2;
            searchModel.CurrentTime = DateTime.Now;
            var result = _IPQCQualityServer.GetCurrentTimeInterval(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetIpqcQualityReportAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<IPQCQualityReportVM>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _IPQCQualityServer.GetIPQCQualityReport(searchModel, page);
            return Ok(result);
        }


        [HttpPost]
        public IHttpActionResult GetQualityTopDetialAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<IPQCQualityReportVM>(jsonData);
            var result = _IPQCQualityServer.GetQualityTopDetial(searchModel);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetIPQCQualityMonthReportAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<IPQCQualityReportVM>(jsonData);
            var result = _IPQCQualityServer.GetQualityMonthReport(searchModel);
            return Ok(result);
        }
        #endregion

        [HttpPost]
        public IHttpActionResult QueryOEE_MachineInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<IPQCQualityReportDto>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var result = _IPQCQualityServer.GetIPQCQualityReport(searchModel, page);
            return Ok(result);
        }
        #region QA检测点维护

        [HttpPost]
        public IHttpActionResult GetGLQADetectionPointAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<GLQAReportVM>(jsondata);
            var result = _IPQCQualityServer.GetGLQADetectionPoint(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public string AddOrEditGL_QADetectionPointAPI(GL_QADetectionPointDTO dto)
        {
            var result = _IPQCQualityServer.AddOrEditGL_QADetectionPoint(dto);
            return result;
        }
        [HttpGet]
        public GL_QADetectionPointDTO GetGLQADetectionPointByIDAPI(int QADetectionPointID)
        {

            return _IPQCQualityServer.GetGLQADetectionPointByID(QADetectionPointID);

        }

        [HttpPost]
        public object RemoveGLQADetectionPointByIDAPI(dynamic value)
        {

            object idStr = value["QADetectionPointID"];
            int QADetectionPointID = 0;
            if (int.TryParse(idStr.ToString(), out QADetectionPointID))
            {
                bool result = _IPQCQualityServer.RemoveGLQADetectionPointByID(QADetectionPointID);
                return new { Result = result };
            }
            else
                return new { Result = false };
        }
        [HttpGet]
        public GL_QADetectionPointDTO GetStationsDetectionPointByIDAPI(int StationID)
        {

            return _IPQCQualityServer.GetStationsDetectionPointByID(StationID);

        }
        [HttpGet]
        public List<GL_QADetectionPointDTO> GetQADetectionPointDTOListAPI(int LineID)
        {
            return _IPQCQualityServer.GetQADetectionPointDTOList(LineID);

        }
        [HttpGet]

        public List<GL_QADetectionPointDTO> GetGL_QADetectionPointDTOAPI(int LineID)
        {
            return _IPQCQualityServer.GetGL_QADetectionPointDTO(LineID);

        }
        [HttpPost]
        public string InserOrUpdateDetectionPointsAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<GL_QADetectionPointDTO>>(jsondata);
             return _IPQCQualityServer.InserOrUpdateDetectionPoints(list);
        }

        #endregion

        #region QA目标良率维护
        [HttpPost]
        public IHttpActionResult QueryGLQAYieldsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<GL_QATargetYieldDTO>(jsondata);
            var result = _IPQCQualityServer.QueryGLQAYields(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public string AddOrEditGLQAYieldAPI(GL_QATargetYieldDTO dto)
        {
            var result = _IPQCQualityServer.AddOrEditGLQAYield(dto);
            return result;
        }
        
        [HttpGet]
        public GL_QATargetYieldDTO QueryGLQAYieldByUIDAPI(int GLQATargetYieldID)
        {

            return _IPQCQualityServer.QueryGLQAYieldByUID(GLQATargetYieldID);

        }
        [HttpGet]

        public List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOListAPI(int StationID)
        {
            return _IPQCQualityServer.GetGL_QATargetYieldDTOList(StationID);

        }
        public List<GL_QATargetYieldDTO> GetGL_QATargetYieldDTOsAPI(int StationID)
        {
            return _IPQCQualityServer.GetGL_QATargetYieldDTOs(StationID);
        }
        [HttpPost]
        public string InserOrUpdateTargetYieldsAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<GL_QATargetYieldDTO>>(jsondata);
            return _IPQCQualityServer.InserOrUpdateTargetYields(list);
        }
        [HttpGet]
        public string DeleteGLQAYieldAPI(int GLQATargetYieldID)
        {
            return _IPQCQualityServer.DeleteGLQAYield(GLQATargetYieldID);
        }
        #endregion
    }
}