using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using PDMS.Core.Authentication;
using PDMS.Model.ViewModels.Settings;
using PDMS.Common.Helpers;

namespace PDMS.WebAPI.Controllers
{
    public class ProductInputController : ApiControllerBase
    {
        IProductDataService ProductDataService;
        ICommonService commonService;

        public ProductInputController(IProductDataService ProductDataService, ICommonService commonService)
        {
            this.ProductDataService = ProductDataService;
            this.commonService = commonService;
        }
        /// <summary>
        /// 获取制程信息，目前未使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryProcessAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ProcessDataSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var process = ProductDataService.QueryProcessData(searchModel, page);
            return Ok(process);
        }
        /// <summary>
        /// 获取生产数据信息，该FlowChart的所有生产数据信息或者制程信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]

        public IHttpActionResult QueryProductsAPI(dynamic data)
        {
            try
            {
                var jsonData = data.ToString();
                var searchModel = JsonConvert.DeserializeObject<ProcessDataSearch>(jsonData);
                var page = JsonConvert.DeserializeObject<Page>(jsonData);
                //获取当前用户Project
                //searchModel.Time = "02:00-04:00";
                var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
                var datas = ProductDataService.QueryProductDatas(searchModel, page, currentProject);
                int count = datas.TotalItemCount;
                //判断查询条件中有无符合条件的生产数据，若无数据，查询符合条件的制程信息。
                if (count == 0)
                {
                    var process = ProductDataService.QueryProcessData_Input(searchModel, page, currentProject);
                    return Ok(process);
                }
                else
                {
                    return Ok(datas);
                }
            }
            catch (Exception ex)
            {
                Logger logger = new Logger("物料员数据录入异常");
                logger.Error("物料员数据录入异常信息：", ex.ToString());
                return Ok("物料员数据录入查询失败:" + ex.Message.ToString());
            }
        }


        [HttpPost]

        public IHttpActionResult QueryProductDataBackUpAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ProcessDataSearchModel>(jsonData);

            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            //获取当前用户Project
            //  var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
            var datas = ProductDataService.QueryProductDataForEmergency(searchModel, page);
            int count = datas.TotalItemCount;
            //判断查询条件中有无符合条件的生产数据，若无数据，查询符合条件的制程信息。
            if (count == 0)
            {
                var process = ProductDataService.QueryProcessDataForEmergency(searchModel, page);
                return Ok(process);
            }
            else
                return Ok(datas);
        }

        /// <summary>
        /// 修改生产数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [IgnoreDBAuthorize]
        public string ModifyProductDataAPI(ProductDataItem dto, int AccountId)
        {

            var ent = ProductDataService.QueryProductDataSingle(dto.Product_UID, dto.Location_Flag);
            if (ent.Is_Comfirm)
                return "PP已经保存数据，不能修改，请刷新页面";
            if (ent.Location_Flag)
            {
                return ProductDataService.SaveLocationInfoAndRework(dto, AccountId);
            }
            else
                return ProductDataService.SaveInfoAndRework(dto, AccountId);

        }

        [IgnoreDBAuthorize]
        public string ModifyProductDataEmergencyAPI(ProductDataItem dto, int AccountId)
        {

            var ent = ProductDataService.QueryProductDataSingle(dto.Product_UID, dto.Location_Flag);
            if (ent.Is_Comfirm)
                return "PP已经保存数据，不能修改，请刷新页面";


            ProductDataService.SaveInfoEmergency(dto, AccountId);
            return "success";
        }
        /// <summary>
        /// 根据UID查询制定的生产数据
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryProductDataAPI(int uuid, bool flag)
        {
            var item = ProductDataService.QueryProductDataSingle(uuid, flag);
            return Ok(item);
            //var dto = new ProductDataDTO();
            //dto = AutoMapper.Mapper.Map<ProductDataDTO>(ProductDataService.QueryProductDataSingle(uuid));
            //return Ok(dto);
        }

        /// <summary>
        /// 根据用户uid 查询该用户所属的功能厂名字
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [IgnoreDBAuthorize]
        public string GetCurrentPlantNameAPI(int uid)
        {
            var datas = ProductDataService.GetCurrentPlantName(uid);
            return datas;
        }


        /// <summary>
        /// 批量存储数据，接收的是对量的数值对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [IgnoreDBAuthorize]
        public string SaveDatasAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<ProductDataList>(data.ToString());
            return ProductDataService.AddProductDatas(entity);
        }
        [IgnoreDBAuthorize]

        public System_Function_Plant QueryFuncPlantInfoAPI(string funcPlant)
        {
            var datas = ProductDataService.QueryFuncPlantInfo(funcPlant);
            return datas;
        }

        /// <summary>
        /// 为未填写数据的FunPlant填充0值---------------------Sidney 2016-1-9
        /// </summary>
        /// <returns></returns>
        public string FillZeroProductDataAPI(ZeroProcessDataSearch funPlantInfo)
        {
            var result = ProductDataService.FillZeroProductData(funPlantInfo);
            return result;
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryTimeSpanReport(ReportDataSearch searchModel)
        {
            if (searchModel.Select_Type == "monthly")
            {
                var process = ProductDataService.QueryTimeSpanReport(searchModel);
                return Ok(process);
            }
            else
            {
                var process = ProductDataService.QueryTimeSpanReport_2(searchModel);
                return Ok(process);
            }
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryWeekReport(ReportDataSearch searchModel)
        {
            var process = ProductDataService.QueryWeekReport(searchModel);
            return Ok(process);
        }


        [IgnoreDBAuthorize]
        public IHttpActionResult QueryDailyDataAPI(ReportDataSearch searchModel)
        {
            if (searchModel.FunPlant == "关键制程")
            {
                searchModel.FunPlant = "Report_Key_Process";
            }
            if (searchModel.FunPlant == "Key Process")
            {
                searchModel.FunPlant = "Report_Key_Process";
            }
            searchModel.Interval_Date_Start = searchModel.Reference_Date;
            searchModel.Interval_Date_End = searchModel.Reference_Date;
            var process = ProductDataService.QueryTChartDailyData(searchModel);
            return Ok(process);
        }
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryMonthlyDataAPI(ReportDataSearch searchModel)
        {
            if (searchModel.FunPlant == "关键制程")
            {
                searchModel.FunPlant = "Report_Key_Process";
            }

            var process = ProductDataService.QueryTimeSpanReport(searchModel);
            return Ok(process);
        }

        [IgnoreDBAuthorize]
        public IHttpActionResult QueryDailyYieldAPI(ReportDataSearch searchModel)
        {
            if (searchModel.FunPlant == "关键制程")
            {
                searchModel.FunPlant = "Report_Key_Process";
            }

            var process = ProductDataService.QueryDailyYield(searchModel);
            return Ok(process);
        }


        #region Rework Module------------------- Sidney
        /// <summary>
        /// GetReworkProcessAPI
        /// </summary>
        /// <param name="Detail_UID"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetRepairToReworkProcessAPI(int Detail_UID, int Product_UID, string selectDate, string selectTime)
        {
            var EnumEntity = ProductDataService.GetRepairToReworkProcessAPI(Detail_UID, Product_UID, selectDate, selectTime);
            return Ok(EnumEntity);
        }
        #endregion

        [IgnoreDBAuthorize]
        public IHttpActionResult GetErrorInfoAPI(int productUid, string ErrorType)
        {
            var result = ProductDataService.GetErrorInfo(productUid, ErrorType);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        [AcceptVerbs("Get")]
        public bool CheckHasExistProcessAPI(int masterUID, int version)
        {
            var hasProcess = ProductDataService.CheckHasExistProcess(masterUID, version);
            return hasProcess;
        }
    }
}