using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace PDMS.WebAPI.Controllers
{
    public class EventReportManagerController : ApiControllerBase
    {
        //之所以使用接口的方式，主要原因是，Service类太多，而且是可变的，不可能将其以子类的方式罗列在后面，而且有可能方法名重复，但接口就能很好的解决这个问题。
        ICommonService commonService;
        ISettingsService settingsService;
        IEventReportManagerService EventReportManagerService;
        public EventReportManagerController(IEventReportManagerService EventReportManagerService, ISettingsService settingsService, ICommonService commonService)
        {
            this.commonService = commonService;
            this.settingsService = settingsService;
            this.EventReportManagerService = EventReportManagerService;
        }

        #region Add by Rock -------------------------- 2016/03/18-------start
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetSystemViewColumnAPI(int Account_UID)
        {
            var list = EventReportManagerService.GetSystemViewColumnList(Account_UID);
            return Ok(list);
        }

        [IgnoreDBAuthorize]
        [AcceptVerbs("GET")]
        public bool UpdateColumnInfoAPI(int Account_UID, int column_Index, bool isDisplay)
        {
            var result = EventReportManagerService.UpdateColumnInfo(Account_UID, column_Index, isDisplay);
            return result;
        }
        #endregion Add by Rock -------------------------- 2016/03/18-------end

        #region WarningListDisplay ----------------------------------Destiny 2015/12/09

        [HttpPost]
        public IHttpActionResult GetWarningListAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<WarningSearch>(data.ToString());
            //添加当前用户的Project
            var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
            var EnumEntity = EventReportManagerService.GetWarningLists(searchModel.user_account_uid, currentProject);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult GetWarningDataByWarningUidAPI(int Warning_UID)
        {
            var result = EventReportManagerService.GetWarningDataByWarningUid(Warning_UID);
            return Ok(result);
        }
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMasterUidByWarningUidAPI(int Warning_UID)
        {
            var result = EventReportManagerService.GetMasterUidByWarningUid(Warning_UID);
            return Ok(result);
        }
        #endregion

        #region  修改不可用wip的值--------------------------- 2016-11-22 add by karl

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryNullWIPDatasAPI(dynamic data)
        {
            var jsonData = data.ToString();

            var searchModel = JsonConvert.DeserializeObject<PPCheckDataSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = EventReportManagerService.QueryNullDatasWIP(searchModel, page);
            return Ok(checkData);
        }

        [AcceptVerbs("Get")]
        public IHttpActionResult EditNullWIPAPI(int product_uid, int nullwip_qty, int modifiedUser)
        {
            var EnumEntity = EventReportManagerService.EditNullWIP(product_uid, nullwip_qty, modifiedUser);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("Get")]
        public IHttpActionResult GetSelctMasterUIDAPI(string ProjectName, string Part_Types, string Product_Phase, string opType)
        {
            var EnumEntity = EventReportManagerService.GetSelctMasterUID(ProjectName, Part_Types, Product_Phase, opType);
            return Ok(EnumEntity);
        }
        [AcceptVerbs("Post")]
        public IHttpActionResult DoExportFunctionAPI(ExportSearch search)
        {
            var dto = Ok(EventReportManagerService.DoExportFunction(search));
            return dto;
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult EditWIPWithZeroAPI(ExportSearch search)
        {
            return Ok(EventReportManagerService.EditWipWithZero(search));
        }

        //取得当前时间段
        [HttpGet]
        public IHttpActionResult GetNowIntervalAPI(int type)
        {
            return Ok(EventReportManagerService.GetIntervalTime(type));
        }

        #endregion
        /// <summary>
        /// 获取专案
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProjectSourceAPPAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<GetProjectModel>(data.ToString());
            //添加当前用户的Project
            var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
            var EnumEntity = EventReportManagerService.GetAllProjectAPP(searchModel.Customer, currentProject, searchModel.orgs);
            return Ok(EnumEntity);
        }
        #region ProductReportDisplay----------------------------------Sidney 2015/12/4
        /// <summary>
        /// 查询Product_Input的的数据API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public IHttpActionResult QueryPPCheckDatasAPI(dynamic data)
        {
            var jsonData = data.ToString();

            var searchModel = JsonConvert.DeserializeObject<PPCheckDataSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = EventReportManagerService.QueryPPCheckDatas(searchModel, page, "QueryPPCheckDatas");
            return Ok(checkData);
        }
        /// <summary>
        /// 查询Product_Input的的数据API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public IHttpActionResult QueryReportDatasAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ReportDataSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = EventReportManagerService.QueryReportDatas(searchModel, page, "QueryReportDatas");
            return Ok(checkData);
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryReportDatasIntervalAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<NewProductReportSumSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = EventReportManagerService.QueryReportDatasInterval(searchModel, page);
            return Ok(checkData);
        }
        [AcceptVerbs("Post")]
        public IHttpActionResult QuerySumReportDatasAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<NewProductReportSumSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = EventReportManagerService.QuerySumReportDatas(searchModel, page);
            return Ok(checkData);
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryReportDatasAPPAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ReportDataSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = EventReportManagerService.QueryReportDatasAPP(searchModel, page, "QueryReportDatas");
            return Ok(checkData);
        }


        [AcceptVerbs("Post")]
        public IHttpActionResult FirstReportDatasAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ReportDataSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = EventReportManagerService.FirstReportDatas(searchModel, page, "QueryReportDatas");
            return Ok(checkData);
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult CheckFunPlantDataIsFullAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<PPCheckDataSearch>(jsonData);
            var checkData = EventReportManagerService.CheckFunPlantDataIsFull(searchModel);
            return Ok(checkData);
        }
        [AcceptVerbs("Post")]
        public IHttpActionResult CheckProductDataIsFullAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<PPCheckDataSearch>(jsonData);
            var checkData = EventReportManagerService.CheckProductDataIsFull(searchModel);
            return Ok(checkData);
        }
        /// <summary>
        /// 修改WIP的API
        /// </summary>
        /// <param name="WIP"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public IHttpActionResult EditWIPAPI(PPEditWIP WIP)
        {
            var productUId = WIP.PPEditValue.FirstOrDefault().Product_UID;
            productUId = productUId.Remove(0, 3);
            var productId = Convert.ToInt32(productUId);
            //添加数据至QA中间表---------------------Sidney
            //var InsertQAData = EventReportManagerService.InsertIPQCData(productId);
            //if (InsertQAData=="SUCCESS")
            return Ok(EventReportManagerService.EditWIP(WIP, WIP.Modified_UID));
            //else
            //{
            //    return Ok("FAlSE"+ InsertQAData);
            //}
        }
       
        public IHttpActionResult EditWIPNewAPI(PPEditWIP WIP)
        {
            var productUId = WIP.PPEditValue.FirstOrDefault().Product_UID;
            productUId = productUId.Remove(0, 3);
            var productId = Convert.ToInt32(productUId);
            //添加数据至QA中间表---------------------Sidney
            //var InsertQAData = EventReportManagerService.InsertIPQCData(productId);
            //if (InsertQAData=="SUCCESS")
            return Ok(EventReportManagerService.EditWIPNew(WIP, WIP.Modified_UID));
            //else
            //{
            //    return Ok("FAlSE"+ InsertQAData);
            //}
        }

        [AcceptVerbs("Get")]
        public IHttpActionResult EditWIPViewAPI(int product_uid, int wip_qty, int wip_old, int wip_add, string comment, int modifiedUser, int nullwip)
        {
            var EnumEntity = EventReportManagerService.EditWIPView(product_uid, wip_qty, wip_old, wip_add, comment, modifiedUser, nullwip);
            return Ok(EnumEntity);
        }


        [IgnoreDBAuthorize]
        [AcceptVerbs("Get")]
        public IHttpActionResult GetUnacommpolished_ReasonAPI()
        {
            var EnumEntity = EventReportManagerService.GetUnacommpolished_Reason();

            return Ok(EnumEntity);
        }

        /// <summary>
        /// 查询时段的API
        /// </summary>
        /// <param name="PageName"></param>
        /// <returns></returns>
        [IgnoreDBAuthorize]
        [AcceptVerbs("Get")]
        public IHttpActionResult GetIntervalTimeAPI(string PageName, int LanguageID, string OP)
        {
            var EnumEntity = EventReportManagerService.GetIntervalTime(PageName, LanguageID, OP);
            var enumVM = AutoMapper.Mapper.Map<List<EnumVM>>(EnumEntity);
            return Ok(enumVM);
        }
        /// <summary>
        /// 获取客户
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetCustomerSourceAPI(List<int> userProjectUid, string oporg)
        {
            var EnumEntity = EventReportManagerService.GetAllCustomer(userProjectUid, oporg);
            return Ok(EnumEntity);
        }
        /// <summary>
        /// 获取专案
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProjectSourceAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<GetProjectModel>(data.ToString());
            //添加当前用户的Project
            var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
            var EnumEntity = EventReportManagerService.GetAllProjectAPP(searchModel.Customer, currentProject, searchModel.orgs);
            return Ok(EnumEntity);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProjectSourceQAAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<GetProjectModel>(data.ToString());
            //添加当前用户的Project
            var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
            //var EnumEntity = EventReportManagerService.GetAllProject(searchModel.Customer, currentProject);
            return Ok(currentProject);
        }





        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProjectByOpAPI(string Op_Type)
        {
            var EnumEntity = EventReportManagerService.GetProjectByOp(Op_Type);
            return Ok(EnumEntity);
        }


        /// <summary>
        /// 获取生产阶段
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProductPhaseSourceAPI(string customer, string project)
        {
            var EnumEntity = EventReportManagerService.GetAllProductPhase(customer, project);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetSelctOPAPI(string customer, string project)
        {
            var EnumEntity = EventReportManagerService.GetSelctOP(customer, project);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProductPhaseSourceAPPAPI(string project)
        {
            var EnumEntity = EventReportManagerService.GetAllProductPhaseAPP(project);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProjectPhaseSourceAPI(string plant, string project)
        {
            var EnumEntity = EventReportManagerService.GetProjectPhaseSource(plant, project);
            return Ok(EnumEntity);
        }

        /// <summary>
        /// 获取部件
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="project"></param>
        /// <param name="productphase"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetPartTypesSourceAPI(string customer, string project, string productphase)
        {
            var EnumEntity = EventReportManagerService.GetAllPartTypes(customer, project, productphase);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetPartTypesSourceAPPAPI(string project, string productphase)
        {
            var EnumEntity = EventReportManagerService.GetAllPartTypesAPP(project, productphase);
            return Ok(EnumEntity);
        }
        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="project"></param>
        /// <param name="productphase"></param>
        /// <param name="parttypes"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetColorSourceAPI(string customer, string project, string productphase, string parttypes)
        {
            var EnumEntity = EventReportManagerService.GetAllColor(customer, project, productphase, parttypes);
            return Ok(EnumEntity);
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetAllColorByFMAPI(string optype, string project, string productphase, string parttypes)
        {
            var EnumEntity = EventReportManagerService.GetAllColorByFM(optype, project,productphase,parttypes);
            return Ok(EnumEntity);
        }
        

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetColorSourceAPPAPI(string project, string productphase, string parttypes)
        {
            var EnumEntity = EventReportManagerService.GetAllColorAPP(project, productphase, parttypes);
            return Ok(EnumEntity);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetColorAPI(string customer, string project, string productphase, string parttypes)
        {
            var EnumEntity = EventReportManagerService.GetColor(customer, project, productphase, parttypes);
            return Ok(EnumEntity);

        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetDayVersionAPI(string customer, string project, string productphase, string parttypes, string day)
        {
            var EnumEntity = EventReportManagerService.GetDayVersion(customer, project, productphase, parttypes, day);
            return Ok(EnumEntity);
        }


        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetColorAPPAPI(string project, string productphase, string parttypes)
        {
            var EnumEntity = EventReportManagerService.GetColorAPP(project, productphase, parttypes);
            return Ok(EnumEntity);

        }


        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFunPlantAPI(string customer, string project, string productphase, string parttypes, int LanguageID)
        {
            var EnumEntity = EventReportManagerService.GetFunPlant(customer, project, productphase, parttypes, LanguageID);
            return Ok(EnumEntity);

        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFunPlantAPPAPI(string project, string productphase, string parttypes)
        {
            var EnumEntity = EventReportManagerService.GetFunPlantAPP(project, productphase, parttypes);
            return Ok(EnumEntity);

        }

        /// <summary>
        /// 获取时段及相关信息
        /// </summary>
        /// <param name="opType"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetIntervalInfoAPI(string opType)
        {
            var entity = EventReportManagerService.GetIntervalInfo(opType);
            return Ok(entity);
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult CheckWIPNeedUpdate(string Op_Type)
        {
            var wipflag = EventReportManagerService.CheckWIPNeedUpdate(Op_Type);
            return Ok(wipflag);
        }

        #endregion
        #region Day Week Month Report Function------------------------Sidney 2016/01/28
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetVersionSourceAPI(string customer, string project, string productphase, string parttypes, DateTime beginTime
            , DateTime endTime)
        {
            var EnumEntity = EventReportManagerService.GetAlVersion(customer, project, productphase, parttypes, beginTime, endTime);
            return Ok(EnumEntity);

        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetDailyVersionAPI(string customer, string project, string productphase, string parttypes, DateTime referenceDay)
        {
            var EnumEntity = EventReportManagerService.GetVersion(customer, project, productphase, parttypes, referenceDay);
            return Ok(EnumEntity);

        }


        [IgnoreDBAuthorize]
        public IHttpActionResult GetVersionBeginEndDateAPI(string customer, string project, string productphase, string parttypes, int version, DateTime startDay, DateTime endDay)
        {
            var EnumEntity = EventReportManagerService.GetVersionBeginEndDate(customer, project, productphase, parttypes, version);

            if (EnumEntity != null)
            {

                if (DateTime.Compare(startDay.Date, EnumEntity.VersionBeginDate.Date) > 0)
                {
                    EnumEntity.VersionBeginDate = startDay;
                }
                if (DateTime.Compare(endDay.Date, EnumEntity.VersionEndDate.Date) < 0 || EnumEntity.VersionEndDate.Date.ToShortDateString() == "1/1/0001")
                {
                    EnumEntity.VersionEndDate = endDay;
                }
                if (DateTime.Compare(DateTime.Now.Date, EnumEntity.VersionEndDate.Date) == 0)
                {
                    EnumEntity.VersionEndDate = DateTime.Now.AddDays(-1).Date;
                }
                EnumEntity.Interval = "从  " + EnumEntity.VersionBeginDate.ToShortDateString() + " 到 " + EnumEntity.VersionEndDate.ToShortDateString();
                return Ok(EnumEntity);
            }
            else
            {
                return Ok(EnumEntity);
            }
        }
        #endregion

        [AcceptVerbs("Post")]
        public string CheckMatchFlagAPI(PPCheckDataSearch search)
        {
            var checkData = EventReportManagerService.CheckMatchFlag(search);
            return checkData;
        }

        public IHttpActionResult ExportPPCheckDataAPI(ExportSearch search)
        {
            var checkData = EventReportManagerService.ExportPPCheckData(search);
            return Ok(checkData);
        }

        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult KeyProcessVertifyAPI(string ProjectName, string Part_Types)
        {
            var EnumEntity = EventReportManagerService.KeyProcessVertify(ProjectName, Part_Types);
            return Ok(EnumEntity);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult GetIntervalInfoAPI()
        {
            List<IntervalEnum> items = new List<IntervalEnum>();
            var item = EventReportManagerService.GetIntervalInfo("OP1");
            items.Add(item);
            var list = new PagedListModel<IntervalEnum>(0, items);
            return Ok(list);
        }

        /// <summary>
        /// 获取未关闭的专案
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetOpenProjectAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<GetProjectModel>(data.ToString());
            var EnumEntity = EventReportManagerService.GetOpenProject(searchModel.orgs);
            return Ok(EnumEntity);
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public string GetOPByFlowchartMasterUIDAPI(int masterUID)
        {
            var entity = EventReportManagerService.GetOPByFlowchartMasterUID(masterUID);
            return entity;
        }

        //2017-05-02 add by karl，判定厂别
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public int GetPlantAPI(string Project)
        {
            var entity = EventReportManagerService.GetPlant(Project);
            return entity;
        }
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFunPlantForChartAPI(string customer, string project, string productphase, string parttypes, int LanguageID)
        {
            var EnumEntity = EventReportManagerService.GetFunPlantForChart(customer, project, productphase, parttypes, LanguageID);
            return Ok(EnumEntity);

        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProductInputLocationAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<ProductInputLocationSearch>(data.ToString());
            var result = EventReportManagerService.QueryProductInputLocation(searchModel, null);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetPDInputLocationByProSeqAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<PDByProSeqSearch>(data.ToString());
            var result = EventReportManagerService.GetPDInputLocationByProSeqAPI(searchModel, null);
            return Ok(result);
        }

        /// <summary>
        /// 导出战情日报表的楼栋详情
        /// </summary>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult ExportFloorDetialDayReportAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<ReportDataSearch>(data.ToString());
            var result = EventReportManagerService.ExportFloorDetialDayReport(searchModel);
            return Ok(result);
        }
    }
}