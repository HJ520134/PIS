using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers.ProductionResourcePlan
{
    public class ProductionResourcePlanController : ApiControllerBase
    {
        IProductionResourcePlanService PRP_Service;


        public ProductionResourcePlanController(IProductionResourcePlanService ProductionResourcePlanService)
        {
            this.PRP_Service = ProductionResourcePlanService;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetDSPlanListAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<DRAWS_QueryParam>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = PRP_Service.GetDSPlanList(searchModel, page);
            return Ok(checkData);
        }

        #region 离职率/排班计划维护
        public IHttpActionResult QueryTurnoverSchedulingInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<DemissionRateAndWorkScheduleDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var list = PRP_Service.QueryTurnoverSchedulingInfo(searchModel, page);
            return Ok(list);
        }
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetDemissionInfoByIDAPI(int demission_uid)
        {
            var item = PRP_Service.GetDemissionInfoByID(demission_uid);
            return Ok(item);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetWorkScheduleAPI()
        {
            var list = new List<string>();
            PRP_Service.GetWorkScheduleList().ForEach(p => list.Add(p.Enum_Value));
            return Ok(list);
        }


        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult DeleteDemissionInfoByIDAPI(int demission_uid)
        {
            var item = PRP_Service.DeleteDemissionInfoByID(demission_uid);
            return Ok(item);
        }

        public string SaveDemissionInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<DemissionRateAndWorkScheduleDTO>(jsonData);
            var result = PRP_Service.SaveDemissionInfo(searchModel);
            return result;
        }

        [IgnoreDBAuthorize]
        public string CheckImportTurnoverExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<List<DemissionRateAndWorkScheduleDTO>>(jsonData);
            var result = PRP_Service.CheckImportTurnoverExcel(list);
            return result;
        }

        public string ImportTurnoverExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<List<DemissionRateAndWorkScheduleDTO>>(jsonData);
            var result = PRP_Service.ImportTurnoverExcel(list);
            return result;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult ExportDemissionRateInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<DemissionRateAndWorkScheduleDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var list = PRP_Service.ExportDemissionRateInfo(searchModel, page);
            return Ok(list);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult ExportDemissionRateInfoByIDAPI(string uids)
        {
            var list = PRP_Service.ExportDemissionRateInfoByID(uids);
            return Ok(list);
        }
     
        #endregion

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetProjectInfoByUserAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<CustomUserInfoVM>(data.ToString());
            var item = PRP_Service.GetProjectList(searchModel);
            return Ok(item);
        }

        #region 现有人力管理
        public IHttpActionResult QueryCurrentStaffInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<CurrentStaffDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var list = PRP_Service.QueryCurrentStaffInfo(searchModel, page);
            return Ok(list);
        }

        public string ImportCurrentStaffInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<List<CurrentStaffDTO>>(jsonData);
            var result = PRP_Service.ImportCurrentStaffInfo(list);
            return result;
        }


        public string CheckImportCurrentStaffExcelAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var list = JsonConvert.DeserializeObject<List<CurrentStaffDTO>>(jsonData);
            var result = PRP_Service.CheckImportCurrentStaffExcel(list);
            return result;
        }

        public string SaveStaffInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<CurrentStaffDTO>(jsonData);
            var result = PRP_Service.SaveStaffInfo(item);
            return result;
        }
        #endregion

        #region ME
        /// <summary>
        /// 取得ME清單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryMEsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<RP_MESearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var ms = PRP_Service.QueryMEs(searchModel, page);
            return Ok(ms);
        }
        /// <summary>
        /// 取得ME_D資料清單by ME主檔流水號
        /// </summary>
        /// <param name="rP_Flowchart_Master_UID">ME主檔流水號</param>
        /// <returns></returns>
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetME_DsAPI(int rP_Flowchart_Master_UID)
        {
            var ms = PRP_Service.GetME_Ds(rP_Flowchart_Master_UID);
            return Ok(ms);
        }
        /// <summary>
        /// 取得ME主檔 Change History
        /// </summary>
        /// <param name="plant_Organization_UID">plant_Organization_UID</param>
        /// <param name="bG_Organization_UID">bG_Organization_UID</param>
        /// <param name="project_UID">project_UID</param>
        /// <returns></returns>
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetME_ChangeHistoryAPI(int plant_Organization_UID, int bG_Organization_UID, int project_UID)
        {
            var ms = PRP_Service.GetME_ChangeHistory(plant_Organization_UID, bG_Organization_UID, project_UID);
            return Ok(ms);
        }
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment清單
        /// </summary>
        /// <param name="search">搜尋條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetME_D_EquipmentsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ME_EquipmentSearchVM>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var ME_D_Equipments = PRP_Service.GetME_D_Equipments(searchModel, page);
            return Ok(ME_D_Equipments);
        }
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <returns></returns>
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetME_D_EquipmentAPI(int rP_Flowchart_Detail_ME_Equipment_UID)
        {
            var ME_D_Equipment = PRP_Service.GetME_D_Equipment(rP_Flowchart_Detail_ME_Equipment_UID);
            return Ok(ME_D_Equipment);
        }
        /// <summary>
        /// 保存設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <param name="wquipment_Manpower_Ratio">人力配比</param>
        /// <param name="wQP_Variable_Qty">設備變動數量</param>
        /// <param name="nPI_Current_Qty">NPI當前數量</param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult SaveME_D_EquipmentAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var _vm = JsonConvert.DeserializeObject<SaveME_EquipmentVM>(jsonData);
            var IsOK = PRP_Service.SaveME_D_Equipment(_vm);
            return Ok(IsOK);
        }
        [HttpPost]
        [IgnoreDBAuthorize]
        public void ProductionPlanningAPI(RP_All_VM all_vm)
        {
            PRP_Service.ImportFlowchartME(all_vm);
        }
        [HttpPost]
        [IgnoreDBAuthorize]
        public string CheckMEIsExistsAPI(RP_ME_ExcelImportParas parasItem)
        {
            var result = PRP_Service.CheckMEIsExists(parasItem);
            return result;
        }

        #endregion
    }
}