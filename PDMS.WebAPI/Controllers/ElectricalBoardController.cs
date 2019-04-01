using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Data;
using PDMS.Model;
using PDMS.Service;
using System.Collections.Generic;
using System.Web.Http;
using PDMS.Core.Authentication;
using PDMS.Model.EntityDTO;
using System;

namespace PDMS.WebAPI.Controllers
{
    public class ElectricalBoardController : ApiControllerBase
    {
        IElectricalBoardService ElectricalBoardService;

        public ElectricalBoardController(IElectricalBoardService ElectricalBoardService)
        {
            this.ElectricalBoardService = ElectricalBoardService;
        }

       


        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryNoticeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<NoticeSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = ElectricalBoardService.QueryNotice(searchModel, page);
            return Ok(checkData);
        }

        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public string AddNoticeAPI(NoticeDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<Notice>(dto);
           
            var plantstring = ElectricalBoardService.AddNotice(ent);
            if (plantstring != "SUCCESS")
                return plantstring;
            else
                return "SUCCESS";
        }

        [HttpGet]
        public IHttpActionResult GetCapacityAPI()
        {
            var result = ElectricalBoardService.GetCapacity();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetTotalCapacityAPI()
        {
            var result = ElectricalBoardService.GetTotalCapacity();
            return Ok(result);
        }

        [HttpPost]
        public string InsertModelLineHRAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<ModelLineHRDTO>>(jsondata);
            return ElectricalBoardService.InsertModelLineHRList(list);
        }

        /// <summary>
        /// 覆盖
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public string CoverModelLineHRAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<ModelLineHRDTO>>(jsondata);
            return ElectricalBoardService.CoverModelLineHRList(list);
        }

        [HttpGet]
        public IHttpActionResult QueryModelLineHRsAPI(dynamic data)
        {
            var workStations = ElectricalBoardService.QueryModelLineHRs();
            return Ok(workStations);
        }

        [HttpGet]
        public IHttpActionResult QueryModelLineHRAPI(int uid)
        {
            //var dto = new Fixture_PartDTO();
            var dto = ElectricalBoardService.QueryModelLineHRSingle(uid);
            return Ok(dto);
        }

        public string AddModelLineHRAPI(ModelLineHRDTO dto)
        {
            var result = ElectricalBoardService.AddModelLineHR(dto);
            return result;
        }

        public string EditModelLineHRAPI(ModelLineHRDTO dto)
        {
            var result = ElectricalBoardService.EditModelLineHR(dto);
            return result;
        }

        [HttpGet]
        public string DeleteModelLineHRAPI(int uid)
        {
            var result = ElectricalBoardService.DeleteModelLineHR(uid);
            return result;
        }
    }
}
