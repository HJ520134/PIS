using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PDMS.Model.ViewModels.Settings;
using PDMS.Common.Constants;
using System.Linq;
using PDMS.Core.Authentication;
namespace PDMS.WebAPI.Controllers
{
    public class ChartController : ApiControllerBase
    {
        IChartService ChartService;
        public ChartController(IChartService ChartService)
        {
            this.ChartService = ChartService;
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFunPlantAPI(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color)
        {
            var entity = ChartService.GetFunPlant(CustomerName,ProjectName,ProductPhaseName,
            PartTypesName,Color);
            return Ok(entity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProcessAPI(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color, string FunPlant)
        {
            var entity = ChartService.GetProcess(CustomerName,ProjectName,ProductPhaseName,
            PartTypesName,Color,FunPlant);
            return Ok(entity);
        }

        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryNoticeAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<NoticeSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = ChartService.QueryNotice(searchModel, page);
            return Ok(checkData);
        }

        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public string AddNoticeAPI(NoticeDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<Notice>(dto);
           
            var plantstring = ChartService.AddNotice(ent);
            if (plantstring != "SUCCESS")
                return plantstring;
            else
                return "SUCCESS";
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public string DeleteNoticeAPI(int uuid)
        {
            return ChartService.DeleteNotice(uuid);

        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult getNoticeContentAPI(string optype)
        {
            var entity = ChartService.getNoticeContent(optype);
            return Ok(entity);
        } 

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult getMovieUrl(string optype ,int CurrentLocation)
        {
            var entity = ChartService.getMovieUrl(optype, CurrentLocation);
            return Ok(entity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetPageSizeAPI()
        {
            var entity = ChartService.GetPageSize();
            return Ok(entity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult getNoticeColorAPI(string optype)
        {
            var entity = ChartService.getNoticeColor(optype);
            return Ok(entity);
        }

         [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult getPartTypesAPI(string project)
        {
            var entity = ChartService.getPartTypes(project);
            return Ok(entity);
        }

        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public IHttpActionResult getShowContentAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EboardSearchModel>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = ChartService.getShowContent(searchModel, page);
            return Ok(checkData);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFinnalYieldAPI(string Projects)
        {
            var entity = ChartService.GetFinnalYield(Projects);
            return Ok(entity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetTopTenQeboardAPI(string Projects, int PageNumber, int PageSize)
        {
            var entity = ChartService.GetNotReachRateHeadData(Projects, PageNumber, PageSize);
            return Ok(entity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetNotReachRateInfoData(string Projects, int PageNumber ,int PageSize)
        {
            var entity = ChartService.GetNotReachRateInfoData(Projects, PageNumber, PageSize);
            return Ok(entity);
        }

        [HttpGet]
        public IHttpActionResult GetQEboardSumTotalData(string Projects,int PageNumber, int PageSize)
        {
            var entity = ChartService.GteQEboardSumDetailData(Projects, PageNumber, PageSize);
            return Ok(entity);
        }

        /// <summary>
        /// 获取品质报表汇总数据
        /// </summary>
        /// <param name="Projects"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetStaticQESumData(string projectName, string dataTime)
        {
            var entity = ChartService.GetStaticQESumData(projectName, dataTime);
            return Ok(entity);
        }

        /// <summary>
        /// 获取品质报表前10大不良数据数据
        /// </summary>
        /// <param name="Projects"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetStaticQETopTenData(string projectName, string dataTime)
        {
         
            var entity = ChartService.GetStaticQETopTenData(projectName, dataTime);
            return Ok(entity);
        }
    }
}
