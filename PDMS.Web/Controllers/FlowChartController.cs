using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using PDMS.Model.ViewModels.Settings;
using System.Web.Helpers;
using System.Web;
using System.Linq;
using System.Text;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.Ajax.Utilities;
using PDMS.Web.Business.Flowchart;


namespace SPP.Web.Controllers
{
    public class FlowChartController : WebControllerBase
    {
        public ActionResult FlowChartList(string errorInfo)
        {
            if (!string.IsNullOrEmpty(errorInfo))
            {
                ViewBag.errorInfo = errorInfo;
            }
            //绑定状态下拉框
            Dictionary<int, string> statusDir = new Dictionary<int, string>();
            statusDir.Add(StructConstants.IsClosedStatus.AllKey, StructConstants.IsClosedStatus.AllValue);
            statusDir.Add(StructConstants.IsClosedStatus.ClosedKey, StructConstants.IsClosedStatus.ClosedValue);
            statusDir.Add(StructConstants.IsClosedStatus.ProcessKey, StructConstants.IsClosedStatus.ProcessValue);
            statusDir.Add(StructConstants.IsClosedStatus.ApproveKey, StructConstants.IsClosedStatus.ApproveValue);
            ViewBag.Status = statusDir;

            //绑定最新版下拉框
            Dictionary<int, string> lastestDir = new Dictionary<int, string>();
            lastestDir.Add(StructConstants.IsLastestStatus.AllKey, StructConstants.IsLastestStatus.AllValue);
            lastestDir.Add(StructConstants.IsLastestStatus.LastestKey, StructConstants.IsLastestStatus.LastestValue);
            ViewBag.Lastest = lastestDir;

            var roleList = this.CurrentUser.GetUserInfo.RoleList.Select(m => m.Role_ID).ToList();
            ViewBag.RoleList = JsonConvert.SerializeObject(roleList);

            //判断用户是否是Etransfer用户
            string etransferProject = ConstConstants.etransferProject; ;
            if (this.CurrentUser.GetUserInfo.projectTypeList.Exists(m => m.Contains(etransferProject)))
            {
                ViewBag.IsEtransfer = true;
            }
            else
            {
                ViewBag.IsEtransfer = false;
            }

            return View();
        }

        #region FlowChart List -----add by Destiny Zhang 2015-12-16 

        public ActionResult ProjectList()
        {
            ViewBag.PageTitle = null;
            string viewBag = string.Empty;
            var processList = this.CurrentUser.DataPermissions.ProcessInfo;
            if (processList.Any() && processList != null)
            {
                viewBag = string.Join(",", processList.Select(m => m.Process).ToList());
            }
            ViewBag.ProcessTitle = viewBag;
            return View();
        }

        public ActionResult WipDetail()
        {
            return View();
        }

        /// <summary>
        /// 获取修改记录详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWIPAlterRecordDetial(WIPDetialParam jsonSearchParams, Page page)
        {
            //var OPType = "";
            //if (jsonSearchParams.OPType == null)
            //{
            //    OPType = this.CurrentUser.UserOrgnizationInfo.
            //}

            var searchParams = new WIPDetialSearchParam
            {
                StartDate = Convert.ToDateTime(jsonSearchParams.StartDate),
                EndDate = Convert.ToDateTime(jsonSearchParams.EndDate),
                Project = jsonSearchParams.ProjectName,
                FunPlant = jsonSearchParams.FunPlant,
                OpType = jsonSearchParams.OPType,
                partType = jsonSearchParams.PartType,
                factoryAddress = jsonSearchParams.Plant
            };

            //JsonConvert.DeserializeObject<WIPDetialSearchParam>(string.Empty);

            string apiUrl = string.Format("FlowChart/GetWIPAlterRecordDetialAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParams, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFlowChartsListData()
        {
            var isMulit = (bool?)Session[SessionConstants.MHFlag_MulitProject] ?? false;
            var searchModel = new GetProcessSearch
            {
                user_account_uid = this.CurrentUser.AccountUId,
                MHFlag_MulitProject = isMulit
            };
            string apiUrl = string.Format("FlowChart/QueryFlowChartDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchModel, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult PrepareForEditeData(int flowChartMaster_Uid, string funcPlant)
        {
            return View("../ProductInput/ProductData");
        }

        #endregion

        #region FlowChart Plan -----add by Destiny Zhang 2016-1-14 

        public ActionResult PlanManager(int MasterUID)
        {
            ViewBag.MasterUID = MasterUID;

            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", MasterUID);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var ddlResult = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(ddlResult);
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;

            return View();
        }

        public ActionResult FlowChartPlanManager(int MasterUID)
        {
            ViewBag.MasterUID = MasterUID;

            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", MasterUID);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var ddlResult = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(ddlResult);
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;

            return View();
        }

        public ActionResult FlowChartIEManager(int MasterUID)
        {
            ViewBag.MasterUID = MasterUID;

            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", MasterUID);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var ddlResult = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(ddlResult);
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;

            return View();
        }

        public ActionResult GetDateTime(bool IsThisWork)
        {
            string startDate = string.Empty;
            string endDate = string.Empty;

            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                startDate = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                endDate = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 7).ToShortDateString();
            }
            else
            {
                int changeDay = DayOfWeek.Sunday - weekDay + 8;
                startDate = DateTime.Now.AddDays(changeDay).ToShortDateString();

                changeDay = DayOfWeek.Sunday - weekDay + 15;
                endDate = DateTime.Now.AddDays(changeDay).ToShortDateString();
            }

            string result = "从" + startDate + "至" + endDate;
            return Content(result);
        }
        public ActionResult GetDateTimeTResult(bool IsThisWork)
        {
            string startDate = string.Empty;
            string endDate = string.Empty;

            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                startDate = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                endDate = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 7).ToShortDateString();
            }
            else
            {

                startDate = getWeekUpOfDate(DateTime.Now, DayOfWeek.Monday, -1).ToShortDateString();


                endDate = getWeekUpOfDate(DateTime.Now, DayOfWeek.Monday, -1).AddDays(6).ToShortDateString();
            }

            string result = "从" + startDate + "至" + endDate;
            return Content(result);
        }
        public DateTime getWeekUpOfDate(DateTime dt, DayOfWeek weekday, int Number)
        {
            int wd1 = (int)weekday;
            int wd2 = (int)dt.DayOfWeek;
            return wd2 == wd1 ? dt.AddDays(7 * Number) : dt.AddDays(7 * Number - wd2 + wd1);
        }

        public ActionResult QueryMGData(int MasterUID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            }
            else
            {
                date = getWeekUpOfDate(DateTime.Now, DayOfWeek.Monday, -1).ToShortDateString();
            }
            string api = "FlowChart/QueryProcessMGDataAPI?masterUID=" + MasterUID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QueryIEMGData(int MasterUID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            }
            else
            {
                date = getWeekUpOfDate(DateTime.Now, DayOfWeek.Monday, -1).ToShortDateString();
            }
            string api = "FlowChart/QueryProcessIEMGDataAPI?masterUID=" + MasterUID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QueryProcessIEMGData(int MasterUID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            }
            else
            {
                date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            }
            string api = "FlowChart/QueryProcessIEMGDataAPI?masterUID=" + MasterUID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryProcessMGData(int MasterUID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            }
            else
            {
                date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            }
            string api = "FlowChart/QueryProcessMGDataAPI?masterUID=" + MasterUID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult ModifyProcessMGData(string jsonWithData)
        {
            var apiUrl = "FlowChart/ModifyProcessMGDataAPI";
            var entity = JsonConvert.DeserializeObject<FlowChartPlanManagerDTO>(jsonWithData);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }



        public ActionResult ModifyProcessIEMGData(string jsonWithData)
        {
            var apiUrl = "FlowChart/ModifyProcessIEMGDataAPI";
            var entity = JsonConvert.DeserializeObject<IEPlanManagerDTO>(jsonWithData);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryProcessMGDataSingle(int detailUID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            }
            else
            {
                date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            }
            string api = "FlowChart/QueryProcessMGDataSingleAPI?detailUID=" + detailUID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryProcessIEMGDataSingle(int detailUID, bool IsThisWork,int shiftID)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            }
            else
            {
                date = DateTime.Now.AddDays(DayOfWeek.Sunday - weekDay + 8).ToShortDateString();
            }
            string api = "FlowChart/QueryProcessIEMGDataSingleAPI?detailUID=" + detailUID + "&date=" + date + "&shiftID=" + shiftID;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryMGDataSingle(int detailUID, bool IsThisWork)
        {
            string date = string.Empty;
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;

            if (IsThisWork)
            {
                date = DateTime.Now.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
            }
            else
            {
                date = getWeekUpOfDate(DateTime.Now, DayOfWeek.Monday, -1).ToShortDateString();
            }
            string api = "FlowChart/QueryProcessMGDataSingleAPI?detailUID=" + detailUID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        public ActionResult QueryFlowCharts(FlowChartModelSearch search, Page page)
        {
            //search.OpTypes = this.CurrentUser.DataPermissions.Op_Types;
            //search.Project_UID = this.CurrentUser.DataPermissions.Project_UID;

            //List<OrganiztionVM> orgInfoList = CurrentUser.GetUserInfo.OrgInfo;
            //List<string> opTypes = this.CurrentUser.GetUserInfo.OrgInfo.Where(m => m.OPType != null).Select(m => m.OPType).ToList();
            //var orgUIDList = orgInfoList.Select(m => m.OPType_OrganizationUID.Value).ToList();
            search.RoleList = CurrentUser.GetUserInfo.RoleList;
            //获取一级SITE UID信息
            search.PlantUIDList = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList;
            //获取二级OPTYPES UID信息
            search.OPType_OrganizationUIDList = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList;
            search.OpTypes = this.CurrentUser.GetUserInfo.OpTypes;
            //获取用户的专案访问权限
            search.ProjectUIDList = this.CurrentUser.GetUserInfo.ProjectUIDList;
            var apiUrl = string.Format("FlowChart/QueryFlowChartsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public string ImportExcel(HttpPostedFileBase uploadName, string FlowChart_Version_Comment)
        {
            string errorInfo = string.Empty;
            errorInfo = FlowchartImportSet(uploadName, 0, FlowChart_Version_Comment);
            return errorInfo;
        }

        public string ImportExcelUpdate(HttpPostedFileBase uploadName_update, int FlowChart_Master_UID, string FlowChart_Version_Comment)
        {
            string errorInfo = string.Empty;
            errorInfo = FlowchartImportSet(uploadName_update, FlowChart_Master_UID, FlowChart_Version_Comment);
            return errorInfo;
        }

        private string FlowchartImportSet(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment)
        {
            string errorInfo = string.Empty;
            FlowChartImport importItem = new FlowChartImport();
            importItem.OrganiztionVMList = this.CurrentUser.GetUserInfo.OrgInfo;
            importItem.Site = this.CurrentUser.GetUserInfo.OrgInfo.First().Plant;
            //判断用户是否是Etransfer用户
            bool isEtransferUser = false;
            string etransferProject = ConstConstants.etransferProject;
            if (this.CurrentUser.GetUserInfo.projectTypeList.Exists(m => m.Contains(etransferProject)))
            {
                isEtransferUser = true;
            }

            FlowchartContext flowContext = new FlowchartContext(isEtransferUser);
            bool isEdit = FlowChart_Master_UID > 0 ? true : false;

            errorInfo = flowContext.CallImportAdd(flowContext.AddOrUpdateExcel, uploadName, FlowChart_Master_UID, FlowChart_Version_Comment, isEdit, importItem);

            if (string.IsNullOrEmpty(errorInfo))
            {
                //string api = "FlowChart/ImportFlowChartAPI?isEdit=" + isEdit + "&accountID=" + this.CurrentUser.AccountUId;
                string api = string.Empty;
                if (isEtransferUser)
                {
                    api = "FlowChart/ImportFlowChartWuXi_MAPI?isEdit=" + isEdit + "&accountID=" + this.CurrentUser.AccountUId;
                }
                else
                {
                    api = "FlowChart/ImportFlowChartAPI?isEdit=" + isEdit + "&accountID=" + this.CurrentUser.AccountUId;
                }
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(importItem, api);
            }
            return errorInfo;
        }

        //private string CallImportAdd(Call A, HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, out FlowChartImport importItem)
        //{
        //    var error = A(uploadName, FlowChart_Master_UID, FlowChart_Version_Comment, isEdit, out importItem);
        //    return error;
        //}


        public ActionResult QueryHistoryFlowChart(int FlowChart_Master_UID)
        {
            string api = "FlowChart/QueryHistoryFlowChartAPI?FlowChart_Master_UID=" + FlowChart_Master_UID;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        private int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //Excel开始的index为1
                    return i + 1;
                }

            }
            return 0;
        }

        //public FileResult DownloadExcel()
        //{
        //    var filePath = Server.MapPath("~/ExcelTemplate/");
        //    var fullFileName = string.Empty;
        //    string etransferProject = "BEIJING";
        //    if (this.CurrentUser.GetUserInfo.ProjectNameList.Exists(m => m.Contains(etransferProject)))
        //    {
        //        fullFileName = filePath + "FlowChart_Etransfer_Template.xlsx";
        //    }
        //    else
        //    {
        //        fullFileName = filePath + "FlowChart_Template.xlsx";
        //    }

        //    //switch (this.CurrentUser.GetUserInfo.OrgInfo.Org_CTU)
        //    //{
        //    //    case StructConstants.Site.CTU:
        //    //        fullFileName = filePath + "FlowChart_Template.xlsx";
        //    //        break;
        //    //    case StructConstants.Site.WUXI:
        //    //        fullFileName = filePath + "FlowChart_Template_WX_Metal.xlsx";
        //    //        break;
        //    //}

        //    FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //    return fpr;
        //}

        public ActionResult QueryFlowChart(int FlowChart_Master_UID)
        {
            string api = "FlowChart/QueryFlowChartAPI?FlowChart_Master_UID=" + FlowChart_Master_UID;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult UpdateExcelInfo(HttpPostedFileBase uploadName_update, int FlowChart_Master_UID, string FlowChart_Version_Comment)
        {
            string errorInfo = string.Empty;
            try
            {
                using (var xlPackage = new ExcelPackage(uploadName_update.InputStream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        errorInfo = "没有worksheet内容";
                        return Content(errorInfo, "application/json");
                    }

                }

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return Content(errorInfo, "application/json");
        }

        private bool ValidIsInt(string result, bool isEdit)
        {
            var validResult = false;
            if (isEdit)
            {
                var splitList = result.Split('_').ToList();
                if (splitList.Count() > 1)
                {
                    validResult = true;
                }
            }
            else
            {
                int validInt = 0;
                var isInt = int.TryParse(result, out validInt);
                if (isInt)
                {
                    validResult = true;
                }
            }
            return validResult;
        }

        private bool ValidIsDouble(string result)
        {
            var validResult = false;
            double validDouble = 0;
            var isDouble = double.TryParse(result, out validDouble);
            if (isDouble)
            {
                validResult = true;
            }
            return validResult;
        }

        public ActionResult FlowChartDetail(int id, bool IsTemp, int Version)
        {
            ViewBag.ID = id;
            ViewBag.IsTemp = IsTemp;
            ViewBag.Version = Version;
            //ViewBag.NoAccess = false;
            //绑定功能厂下拉框
            //var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo.ToList();
            //if (orgInfo == null || orgInfo.Count() == 0)
            //{
            //    List<SystemFunctionPlantDTO> aabb = new List<SystemFunctionPlantDTO>();
            //    ViewBag.FunPlantList = aabb;
            //    ViewBag.NoAccess = true;
            //    return View();
            //}
            //获取用户所属的三级权限，从而获得4级权限功能厂，不考虑跨厂区权限
            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", id);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var ddlResult = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(ddlResult);
            ViewBag.FunPlantList = item.SystemFunctionPlantDTOList;
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;

            //获取数据来源的枚举传入参数为Report_Data_Source GetEnumListAPI

            var enumerationUrl = string.Format("Settings/GetEnumListAPI?enumType={0}", "Report_Data_Source");
            HttpResponseMessage enumerationMessage = APIHelper.APIGetAsync(enumerationUrl);
            var enumerationResult = enumerationMessage.Content.ReadAsStringAsync().Result;
            var enumerationitems = JsonConvert.DeserializeObject<IEnumerable<EnumerationDTO>>(enumerationResult);
            //  var enumerationitems = JsonConvert.DeserializeObject<EnumerationDTO>(enumerationResult);
            ViewBag.Data_SourceList = enumerationitems;
            return View();
        }

        public ActionResult QueryFLDetailList(int id, bool IsTemp, int Version)
        {
            ViewBag.DetailUID = id;
            ViewBag.IsTemp = IsTemp;
            string apiUrl = string.Empty;
            //switch (this.CurrentUser.GetUserInfo.OrgInfo.Org_CTU)
            //{
            //    case StructConstants.Site.CTU:
            //        apiUrl = string.Format("FlowChart/QueryFLDetailListAPI?id={0}&Version={1}", id, Version);
            //        break;
            //    case StructConstants.Site.WUXI:
            //        apiUrl = string.Format("FlowChart/QueryFLDetailWuXiListAPI?id={0}&Version={1}", id, Version);
            //        break;
            //}
            apiUrl = string.Format("FlowChart/QueryFLDetailListAPI?id={0}&Version={1}", id, Version);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(null, null, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<PagedListModel<FlowChartDetailGet>>(result);

            return Content(result, "application/json");
        }

        public ActionResult DoHistoryExcelExport(int id, int version)
        {
            //判断用户是否是Etransfer用户
            bool isEtransferUser = false;
            string etransferProject = ConstConstants.etransferProject;
            if (this.CurrentUser.GetUserInfo.projectTypeList.Exists(m => m.Contains(etransferProject)))
            {
                isEtransferUser = true;
            }

            return new FlowchartContext(isEtransferUser).DoHistoryExcelExport(id, version);
        }

        public ActionResult ClosedFlowChart(int FlowChart_Master_UID, bool isClosed)
        {
            var apiUrl = string.Format("FlowChart/ClosedFLAPI?FlowChart_Master_UID={0}&isClosed={1}", FlowChart_Master_UID, isClosed);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteFlowChartTemp(int FlowChart_Master_UID)
        {
            var apiUrl = string.Format("FlowChart/DeleteFLTempAPI?FlowChart_Master_UID={0}", FlowChart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return null;
        }

        public ActionResult QueryFLDetailByID(int FlowChart_Detail_UID)
        {
            var apiUrl = string.Format("FlowChart/QueryFLDetailByIDAPI?id={0}", FlowChart_Detail_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult EditFLDetailInfo(int id, bool isTemp, int Version, FlowChartDetailDTO dto)
        {
            var apiUrl = string.Format("FlowChart/SaveFLDetailInfoAPI?AccountID={0}", this.CurrentUser.AccountUId);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            if (result == "")
                return RedirectToAction("FlowChartDetail", "FlowChart", new { id = id, IsTemp = isTemp, Version = Version });
            else
                return Content(result, "application/json");

        }

        public ActionResult SaveAllDetailInfo(string jsonDataTable)
        {
            var apiUrl = string.Format("FlowChart/SaveAllDetailInfoAPI?AccountID={0}", this.CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonDataTable, apiUrl);
            return null;
        }

        #region FlowChart生产维护

        #region 生产维护模板下载
        public FileResult DownloadPlanExcel(int id, string clintName)
        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("FlowChart/QueryFLDetailListAPI?id={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("FlowChart/QueryFLDetailListAPI?id={0}&week=current", id);
                    break;
            }

            //var list = JsonConvert.DeserializeObject<List<FlowChartDetailDTO>()>
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FlowChartDetailAndMgData>>(result);
            if (list.Count() > 0)
            {
                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("FlowChartPlanManager");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("FlowChartPlanManager");

                    SetExcelStyle(worksheet, propertiesHead);

                    int iRow = 3;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Process_Seq;
                        worksheet.Cells[iRow, 2].Value = item.Process;
                        worksheet.Cells[iRow, 3].Value = item.Color;
                        worksheet.Cells[iRow, 4].Value = item.Place;
                        //循环每一天的产量数据,将上周的计划导入到这周的计划中来
                        foreach (var mgItem in item.MgDataList)
                        {
                            //获取这一天是星期几
                            var getDayOfWeek = mgItem.Product_Date.DayOfWeek.ToString();
                            switch (getDayOfWeek)
                            {
                                case "Monday":
                                    worksheet.Cells[iRow, 5].Value = mgItem.Product_Plan;
                                    worksheet.Cells[iRow, 6].Value = mgItem.IE_DeptHuman;
                                    worksheet.Cells[iRow, 7].Value = mgItem.Proper_WIP;
                                    break;
                                case "Tuesday":
                                    worksheet.Cells[iRow, 8].Value = mgItem.Product_Plan;
                                    worksheet.Cells[iRow, 9].Value = mgItem.IE_DeptHuman;
                                    worksheet.Cells[iRow, 10].Value = mgItem.Proper_WIP;
                                    break;
                                case "Wednesday":
                                    worksheet.Cells[iRow, 11].Value = mgItem.Product_Plan;
                                    worksheet.Cells[iRow, 12].Value = mgItem.IE_DeptHuman;
                                    worksheet.Cells[iRow, 13].Value = mgItem.Proper_WIP;
                                    break;
                                case "Thursday":
                                    worksheet.Cells[iRow, 14].Value = mgItem.Product_Plan;
                                    worksheet.Cells[iRow, 15].Value = mgItem.IE_DeptHuman;
                                    worksheet.Cells[iRow, 16].Value = mgItem.Proper_WIP;
                                    break;
                                case "Friday":
                                    worksheet.Cells[iRow, 17].Value = mgItem.Product_Plan;
                                    worksheet.Cells[iRow, 18].Value = mgItem.IE_DeptHuman;
                                    worksheet.Cells[iRow, 19].Value = mgItem.Proper_WIP;
                                    break;
                                case "Saturday":
                                    worksheet.Cells[iRow, 20].Value = mgItem.Product_Plan;
                                    worksheet.Cells[iRow, 21].Value = mgItem.IE_DeptHuman;
                                    worksheet.Cells[iRow, 22].Value = mgItem.Proper_WIP;
                                    break;
                                case "Sunday":
                                    worksheet.Cells[iRow, 23].Value = mgItem.Product_Plan;
                                    worksheet.Cells[iRow, 24].Value = mgItem.IE_DeptHuman;
                                    worksheet.Cells[iRow, 25].Value = mgItem.Proper_WIP;
                                    break;

                            }
                        }
                        worksheet.Cells[iRow, 26].Value = item.FlowChart_Detail_UID;
                        iRow++;
                    }
                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置灰色背景
                    var colorRange = string.Format("A3:D{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    //设置主键列隐藏
                    worksheet.Column(26).Hidden = true;
                    //设置边框
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }
        }


        #region IE模板下载
        public FileResult DownloadIEExcel(int id, string clintName)
        {
            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            switch (clintName)
            {
                case "js_btn_download_day":
                    propertiesHead = GetDayPlanHeadColumn();
                    apiUrl = string.Format("FlowChart/QueryFLDetailListAPI?id={0}&week=current", id);
                    break;
                case "js_btn_download_fl":
                    propertiesHead = GetNextWeekPlanHeadColumn();
                    apiUrl = string.Format("FlowChart/QueryFLDetailListAPI?id={0}&week=next", id);
                    break;
                case "js_btn_download_currentWK":
                    propertiesHead = GetCurrentWeekPlanHeadColumn();
                    apiUrl = string.Format("FlowChart/QueryFLDetailListAPI?id={0}&week=current", id);
                    break;
            }

            //var list = JsonConvert.DeserializeObject<List<FlowChartDetailDTO>()>
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FlowChartDetailAndMgData>>(result);
            if (list.Count() > 0)
            {
                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("FlowChartPlanManager");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("FlowChartPlanManager");
                    if (clintName == "js_btn_download_day")
                    {
                        SetIEDayExcelStyle(worksheet, propertiesHead);
                    } else {
                        SetIEExcelStyle(worksheet, propertiesHead);
                    }
                    int iRow = 3;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Process_Seq;
                        worksheet.Cells[iRow, 2].Value = item.Process;
                        worksheet.Cells[iRow, 3].Value = item.Color;
                        worksheet.Cells[iRow, 4].Value = item.Place;
                        worksheet.Cells[iRow, 5].Value = "白班";
                        //循环每一天的产量数据,将上周的计划导入到这周的计划中来
                        foreach (var mgItem in item.MgDataList)
                        {
                            //获取这一天是星期几
                            var getDayOfWeek = mgItem.Product_Date.DayOfWeek.ToString();
                            
                        }
                        worksheet.Cells[iRow, 26].Value = item.FlowChart_Detail_UID;
                        iRow++;
                    }
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Process_Seq;
                        worksheet.Cells[iRow, 2].Value = item.Process;
                        worksheet.Cells[iRow, 3].Value = item.Color;
                        worksheet.Cells[iRow, 4].Value = item.Place;
                        worksheet.Cells[iRow, 5].Value = "夜班";
                        //循环每一天的产量数据,将上周的计划导入到这周的计划中来
                        foreach (var mgItem in item.MgDataList)
                        {
                            //获取这一天是星期几
                            var getDayOfWeek = mgItem.Product_Date.DayOfWeek.ToString();

                        }
                        worksheet.Cells[iRow, 26].Value = item.FlowChart_Detail_UID;
                        iRow++;
                    }
                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置灰色背景
                    var colorRange = string.Format("A3:D{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    //设置主键列隐藏
                    worksheet.Column(26).Hidden = true;
                    //设置边框
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Y{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.Save();
                }
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
            else
            {
                return null;
            }
        }
        #endregion




        private void SetExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];

            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 8].Value = propertiesHead[5];
            worksheet.Cells[1, 11].Value = propertiesHead[6];
            worksheet.Cells[1, 14].Value = propertiesHead[7];
            worksheet.Cells[1, 17].Value = propertiesHead[8];
            worksheet.Cells[1, 20].Value = propertiesHead[9];
            worksheet.Cells[1, 23].Value = propertiesHead[10];
            worksheet.Cells[1, 26].Value = propertiesHead[11];
            //合并单元格
            worksheet.Cells["E1:G1"].Merge = true;
            worksheet.Cells["H1:J1"].Merge = true;
            worksheet.Cells["K1:M1"].Merge = true;
            worksheet.Cells["N1:P1"].Merge = true;
            worksheet.Cells["Q1:S1"].Merge = true;
            worksheet.Cells["T1:V1"].Merge = true;
            worksheet.Cells["W1:Y1"].Merge = true;

            //填充SubTitle内容
            for (int i = 5; i < 26; i++)
            {
                if (i % 3 == 0)
                {
                    worksheet.Cells[2, i].Value = "目标良率";
                }
                else if (i % 3 == 2)
                {
                    worksheet.Cells[2, i].Value = "生产计划";
                }
                else if (i % 3 == 1)
                {
                    worksheet.Cells[2, i].Value = "合理WIP";
                }
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            //设置百分比格式
            worksheet.Column(6).Style.Numberformat.Format = "0.00%";
            worksheet.Column(9).Style.Numberformat.Format = "0.00%";
            worksheet.Column(12).Style.Numberformat.Format = "0.00%";
            worksheet.Column(15).Style.Numberformat.Format = "0.00%";
            worksheet.Column(18).Style.Numberformat.Format = "0.00%";
            worksheet.Column(21).Style.Numberformat.Format = "0.00%";
            worksheet.Column(24).Style.Numberformat.Format = "0.00%";

            //设置列宽
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 17;
            for (int i = 3; i <= 25; i++)
            {
                worksheet.Column(i).Width = 12;
            }

            worksheet.Cells["A1:Y2"].Style.Font.Bold = true;
            worksheet.Cells["A1:Y2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:Y2"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }

        private void SetIEDayExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = "班别";
            worksheet.Cells[1, 6].Value = propertiesHead[4];
            //worksheet.Cells[1, 8].Value = propertiesHead[5];
            //worksheet.Cells[1, 10].Value = propertiesHead[6];
            //worksheet.Cells[1, 12].Value = propertiesHead[7];
            //worksheet.Cells[1, 14].Value = propertiesHead[8];
            //worksheet.Cells[1, 16].Value = propertiesHead[9];
            //worksheet.Cells[1, 18].Value = propertiesHead[10];

            //合并单元格
            worksheet.Cells["F1:G1"].Merge = true;

            //worksheet.Cells[2, 6].Value = "IE标准效率";
            //worksheet.Cells[2, 7].Value = "部门有效人力";
            //填充SubTitle内容
            for (int i = 6; i < 8; i++)
            {
                if (i % 2 == 1)
                {
                    worksheet.Cells[2, i].Value = "部门有效人力";
                }

                else if (i % 2 == 0)
                {
                    worksheet.Cells[2, i].Value = "IE标准效率";
                }
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            //设置百分比格式
            // worksheet.Column(6).Style.Numberformat.Format = "0.00%";
            //  worksheet.Column(9).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(12).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(15).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(18).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(21).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(24).Style.Numberformat.Format = "0.00%";




            //设置列宽
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 17;
            for (int i = 3; i <= 25; i++)
            {
                worksheet.Column(i).Width = 12;
            }

            worksheet.Cells["A1:Y2"].Style.Font.Bold = true;
            worksheet.Cells["A1:Y2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:Y2"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }


        private void SetIEExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = "班别";
            worksheet.Cells[1, 6].Value = propertiesHead[4];
            worksheet.Cells[1, 8].Value = propertiesHead[5];
            worksheet.Cells[1, 10].Value = propertiesHead[6];
            worksheet.Cells[1, 12].Value = propertiesHead[7];
            worksheet.Cells[1, 14].Value = propertiesHead[8];
            worksheet.Cells[1, 16].Value = propertiesHead[9];
            worksheet.Cells[1, 18].Value = propertiesHead[10];

            //合并单元格
            worksheet.Cells["F1:G1"].Merge = true;
            worksheet.Cells["H1:I1"].Merge = true;
            worksheet.Cells["J1:K1"].Merge = true;
            worksheet.Cells["L1:M1"].Merge = true;
            worksheet.Cells["N1:O1"].Merge = true;
            worksheet.Cells["P1:Q1"].Merge = true;
            worksheet.Cells["R1:S1"].Merge = true;

            //填充SubTitle内容
            for (int i = 6; i < 20; i++)
            {
                if (i % 2 == 1)
                {
                    worksheet.Cells[2, i].Value = "部门有效人力";
                }

                else if (i % 2 == 0)
                {
                    worksheet.Cells[2, i].Value = "IE标准效率";
                }
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            //设置百分比格式
            // worksheet.Column(6).Style.Numberformat.Format = "0.00%";
            //  worksheet.Column(9).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(12).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(15).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(18).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(21).Style.Numberformat.Format = "0.00%";
            // worksheet.Column(24).Style.Numberformat.Format = "0.00%";




            //设置列宽
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 17;
            for (int i = 3; i <= 25; i++)
            {
                worksheet.Column(i).Width = 12;
            }

            worksheet.Cells["A1:Y2"].Style.Font.Bold = true;
            worksheet.Cells["A1:Y2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:Y2"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }

        private string[] GetDayPlanHeadColumn()
        {

            var strDT = DateTime.Now.DayOfWeek.ToString();

          //  var strDT = DateTime.Now.ToString();
            var propertiesHead = new[]
            {
                "制程序号",
                "制程名称",
                "颜色",
                "场地",
                string.Format(strDT+"({0})", DateTime.Now.ToString(FormatConstants.DateTimeFormatStringByDate)),
               
                "FlowChart_Detail_UID"
            };
            return propertiesHead;
        }

        private string[] GetNextWeekPlanHeadColumn()
        {
            var nextWeek = GetNextWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "制程序号",
                "制程名称",
                "颜色",
                "场地",
                string.Format("星期一({0})", nextWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期二({0})", nextWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期三({0})", nextWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期四({0})", nextWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期五({0})", nextWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期六({0})", nextWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期日({0})", nextWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                "FlowChart_Detail_UID"
            };
            return propertiesHead;
        }

        private string[] GetNextIEWeekPlanHeadColumn()
        {
            var nextWeek = GetNextWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "制程序号",
                "制程名称",
                "颜色",
                "场地",
                "班别",
                string.Format("星期一({0})", nextWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期二({0})", nextWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期三({0})", nextWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期四({0})", nextWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期五({0})", nextWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期六({0})", nextWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期日({0})", nextWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                "FlowChart_Detail_UID"
            };
            return propertiesHead;
        }

        private string[] GetCurrentWeekPlanHeadColumn()
        {
            var currentWeek = GetCurrentWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "制程序号",
                "制程名称",
                "颜色",
                "场地",
                string.Format("星期一({0})", currentWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期二({0})", currentWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期三({0})", currentWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期四({0})", currentWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期五({0})", currentWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期六({0})", currentWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期日({0})", currentWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                "FlowChart_Detail_UID"
            };
            return propertiesHead;
        }





        private string[] GetCurrentIEWeekPlanHeadColumn()
        {
            var currentWeek = GetCurrentWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "制程序号",
                "制程名称",
                "颜色",
                "场地",
                "班别",
                string.Format("星期一({0})", currentWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期二({0})", currentWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期三({0})", currentWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期四({0})", currentWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期五({0})", currentWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期六({0})", currentWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format("星期日({0})", currentWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                "FlowChart_Detail_UID"
            };
            return propertiesHead;
        }

     


        private Week GetNextWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week nextWeek = new Week();
            //获取下周一的日期
            switch (strDT)
            {
                case "Monday":
                    nextWeek.Monday = dt.AddDays(7);
                    break;
                case "Tuesday":
                    nextWeek.Monday = dt.AddDays(6);
                    break;
                case "Wednesday":
                    nextWeek.Monday = dt.AddDays(5);
                    break;
                case "Thursday":
                    nextWeek.Monday = dt.AddDays(4);
                    break;
                case "Friday":
                    nextWeek.Monday = dt.AddDays(3);
                    break;
                case "Saturday":
                    nextWeek.Monday = dt.AddDays(2);
                    break;
                case "Sunday":
                    nextWeek.Monday = dt.AddDays(1);
                    break;
            }
            nextWeek.Tuesday = nextWeek.Monday.AddDays(1);
            nextWeek.Wednesday = nextWeek.Monday.AddDays(2);
            nextWeek.Thursday = nextWeek.Monday.AddDays(3);
            nextWeek.Friday = nextWeek.Monday.AddDays(4);
            nextWeek.Saturday = nextWeek.Monday.AddDays(5);
            nextWeek.Sunday = nextWeek.Monday.AddDays(6);

            return nextWeek;
        }

        private Week GetCurrentWeek(DateTime dt)
        {
            var strDT = dt.DayOfWeek.ToString();
            Week currentWeek = new Week();
            switch (strDT)
            {
                case "Monday":
                    currentWeek.Monday = dt;
                    break;
                case "Tuesday":
                    currentWeek.Monday = dt.AddDays(-1);
                    break;
                case "Wednesday":
                    currentWeek.Monday = dt.AddDays(-2);
                    break;
                case "Thursday":
                    currentWeek.Monday = dt.AddDays(-3);
                    break;
                case "Friday":
                    currentWeek.Monday = dt.AddDays(-4);
                    break;
                case "Saturday":
                    currentWeek.Monday = dt.AddDays(-5);
                    break;
                case "Sunday":
                    currentWeek.Monday = dt.AddDays(-6);
                    break;
            }
            currentWeek.Tuesday = currentWeek.Monday.AddDays(1);
            currentWeek.Wednesday = currentWeek.Monday.AddDays(2);
            currentWeek.Thursday = currentWeek.Monday.AddDays(3);
            currentWeek.Friday = currentWeek.Monday.AddDays(4);
            currentWeek.Saturday = currentWeek.Monday.AddDays(5);
            currentWeek.Sunday = currentWeek.Monday.AddDays(6);

            return currentWeek;
        }
        #endregion


        #region 生产维护模板导入
        public string ImportPlanExcel(HttpPostedFileBase upload_excel, int FlowChart_Master_UID, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
                    case "day":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "nextWeek":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
                allColumnsAreError = validExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (allColumnsAreError)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }

                List<FlowChartMgDataDTO> mgDataList = new List<FlowChartMgDataDTO>();
                //Excel行号
                int iRow = 3;
                //Excel列号
                int iColumn;
                int j = 5;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 5;
                            break;
                        case "Tuesday":
                            j = 8;
                            break;
                        case "Wednesday":
                            j = 11;
                            break;
                        case "Thursday":
                            j = 14;
                            break;
                        case "Friday":
                            j = 17;
                            break;
                        case "Saturday":
                            j = 20;
                            break;
                        case "Sunday":
                            j = 23;
                            break;
                    }
                }

                for (iRow = 3; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 25; iColumn++)
                    {
                        FlowChartMgDataDTO newMGDataItem = new FlowChartMgDataDTO();

                        var FlowChart_Detail_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 26].Value);
                        if (string.IsNullOrWhiteSpace(FlowChart_Detail_UID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行主键没有值", iRow);
                            break;
                        }

                        var Product_Plan = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_Plan))
                        {
                            Product_Plan = "0";
                            //allColumnsAreError = true;
                            //errorInfo = string.Format("第{0}行生产计划没有值", iRow);
                            //break;
                        }

                        if (!ValidIsDouble(Product_Plan))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行生产计划输入不正确", iRow);
                            break;
                        }

                        var Target_Yield = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ++iColumn].Value);
                        //如果目标良率为空则默认为0
                        if (string.IsNullOrWhiteSpace(Target_Yield))
                        {
                            Target_Yield = "0";
                            //allColumnsAreError = true;
                            //errorInfo = string.Format("第{0}行目标良率没有值", iRow);
                            //break;
                        }
                        if (!ValidIsDouble(Target_Yield))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行目标良率输入不正确", iRow);
                            break;
                        }

                        var Proper_WIP = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ++iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Proper_WIP))
                        {
                            Proper_WIP = "0";
                            //allColumnsAreError = true;
                            //errorInfo = string.Format("第{0}行生产计划没有值", iRow);
                            //break;
                        }

                        if (!ValidIsDouble(Proper_WIP))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行合理WIP输入不正确", iRow);
                            break;
                        }

                        newMGDataItem.FlowChart_Detail_UID = Convert.ToInt32(FlowChart_Detail_UID);
                        switch (iColumn)
                        {
                            case 7:
                                newMGDataItem.Product_Date = week.Monday;
                                break;
                            case 10:
                                newMGDataItem.Product_Date = week.Tuesday;
                                break;
                            case 13:
                                newMGDataItem.Product_Date = week.Wednesday;
                                break;
                            case 16:
                                newMGDataItem.Product_Date = week.Thursday;
                                break;
                            case 19:
                                newMGDataItem.Product_Date = week.Friday;
                                break;
                            case 22:
                                newMGDataItem.Product_Date = week.Saturday;
                                break;
                            case 25:
                                newMGDataItem.Product_Date = week.Sunday;
                                break;
                        }

                        //四舍五入小数点
                        newMGDataItem.Product_Plan = Convert.ToInt32(Convert.ToDouble(Product_Plan).ToString("#0"));

                        //四舍五入小数点
                        newMGDataItem.IE_DeptHuman = Convert.ToDouble(Target_Yield);
                        newMGDataItem.IE_DeptHuman = Convert.ToDouble(newMGDataItem.IE_DeptHuman.ToString("#0.0000"));

                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        newMGDataItem.Proper_WIP = Convert.ToInt32(Convert.ToDouble(Proper_WIP).ToString("#0"));

                        mgDataList.Add(newMGDataItem);
                    }

                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("FlowChart/ImportFlowChartMGDataAPI?FlowChart_Master_UID={0}", FlowChart_Master_UID);
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;
        }

        #region /验证Excel的表头是否正确
        private bool validExcelTitleIsErrorTwo(ExcelWorksheet worksheet, string[] propertiesHead, int iRow, int totalColumns)
        {
            bool allColumnsAreError = false;
            for (var i = 1; i <= totalColumns; i++)
            {
                if (allColumnsAreError)
                {
                    break;
                }
                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5: //星期一
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, i - 1);
                        break;
                    case 8: //星期二
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 5);
                        break;
                    case 11: //星期三
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 6);
                        break;
                    case 14: //星期四
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 7);
                        break;
                    case 17: //星期五
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 8);
                        break;
                    case 20: //星期六
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 9);
                        break;
                    case 23: //星期日
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 10);
                        break;
                    case 26:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "FlowChart_Detail_UID")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    default:
                        continue;
                }
            }
            return allColumnsAreError;
        }

        private bool validExcelTitleIsError(ExcelWorksheet worksheet, string[] propertiesHead, int iRow, int iColumn, int column)
        {
            bool isError = false;
            if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value) != propertiesHead[column])
            {
                isError = true;
            }
            return isError;
        }
        #endregion

        #endregion


        #region IE维护模板导入
        public string ImportIEExcel(HttpPostedFileBase upload_excel, int FlowChart_Master_UID, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
               // var strDT = DateTime.Now.DayOfWeek.ToString();

                switch (hid_currentOrNextWeek)
                {
                    case "day":
                        propertiesHead = GetDayPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now); ;
                        break;
                    case "nextWeek":
                        propertiesHead = GetNextIEWeekPlanHeadColumn();
                        week = GetNextWeek(DateTime.Now);
                        break;
                    case "currentWeek":
                        propertiesHead = GetCurrentIEWeekPlanHeadColumn();
                        week = GetCurrentWeek(DateTime.Now);
                        break;
                }

                bool allColumnsAreError = false;

                //验证Excel的表头是否正确
               
                allColumnsAreError = validIEExcelTitleIsErrorTwo(worksheet, propertiesHead, 1, totalColumns);
                if (hid_currentOrNextWeek != "day")
                {
                    if (allColumnsAreError)
                    {
                        errorInfo = "Excel格式不正确";
                        return errorInfo;
                    }
                }
                List<FlowChartIEMgDataDTO> mgDataList = new List<FlowChartIEMgDataDTO>();
                //Excel行号
                int iRow = 3;
                //Excel列号
                int iColumn;
                int j = 5;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 6;
                            break;
                        case "Tuesday":
                            j = 8;
                            break;
                        case "Wednesday":
                            j = 10;
                            break;
                        case "Thursday":
                            j = 12;
                            break;
                        case "Friday":
                            j = 14;
                            break;
                        case "Saturday":
                            j = 16;
                            break;
                        case "Sunday":
                            j = 18;
                            break;
                    }
                }

              
                for (iRow = 3; iRow <= totalRows; iRow++)
                {
                  
                    if (hid_currentOrNextWeek != "day" && allColumnsAreError)
                    {
                        break;
                    }

                    //如果Excel导入的计划是每日计划
                    if (hid_currentOrNextWeek == "day")
                    {
                        FlowChartIEMgDataDTO newMGDataItem = new FlowChartIEMgDataDTO();
                        var ShiftTimeID1 = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                        //判断班别是否为空
                        if (ShiftTimeID1 == "白班")
                        {
                            ShiftTimeID1 = "4";
                        }
                        if (ShiftTimeID1 == "夜班")
                        {
                            ShiftTimeID1 = "5";
                        }
                        newMGDataItem.ShiftTimeID = int.Parse(ShiftTimeID1);
                        var strDT = DateTime.Now.DayOfWeek.ToString();
                        newMGDataItem.IE_TargetDate = DateTime.Now;
                        var IE_TargetEfficacy = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);
                        //如果IE标准效率为空则默认为0
                        if (string.IsNullOrWhiteSpace(IE_TargetEfficacy))
                        {
                            IE_TargetEfficacy = "0";
                            //allColumnsAreError = true;
                            //errorInfo = string.Format("第{0}行生产计划没有值", iRow);
                            //break;
                        }
                        newMGDataItem.IE_TargetEfficacy = int.Parse(IE_TargetEfficacy);
                        var IE_DeptHuman = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);
                        //如果部门有效人力为空则默认为0
                        if (string.IsNullOrWhiteSpace(IE_DeptHuman))
                        {
                            IE_DeptHuman = "0";
                            //allColumnsAreError = true;
                            //errorInfo = string.Format("第{0}行目标良率没有值", iRow);
                            //break;
                        }
                        newMGDataItem.IE_DeptHuman = int.Parse(IE_DeptHuman);
                        var FlowChart_Detail_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 26].Value);
                        newMGDataItem.FlowChart_Detail_UID = int.Parse(FlowChart_Detail_UID);
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;

                        mgDataList.Add(newMGDataItem);
                    }



                    var ShiftTimeID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    //判断班别是否为空
                    if (ShiftTimeID == "白班")
                    {
                        ShiftTimeID = "4";
                    }
                    if (ShiftTimeID == "夜班")
                    {
                        ShiftTimeID = "5";
                    }


                    if (hid_currentOrNextWeek != "day")
                    {
                        for (iColumn = j + 1; iColumn <= 19; iColumn++)
                        {
                            FlowChartIEMgDataDTO newMGDataItem = new FlowChartIEMgDataDTO();

                            var FlowChart_Detail_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 26].Value);
                            if (string.IsNullOrWhiteSpace(FlowChart_Detail_UID))
                            {
                                allColumnsAreError = true;
                                errorInfo = string.Format("第{0}行主键没有值", iRow);
                                break;
                            }

                            //判断班别是否为空

                            if (string.IsNullOrWhiteSpace(ShiftTimeID))
                            {
                                errorInfo = string.Format("第{0}行班别不能为空", iRow);
                            }


                            var IE_TargetEfficacy = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                            //如果IE标准效率为空则默认为0
                            if (string.IsNullOrWhiteSpace(IE_TargetEfficacy))
                            {
                                IE_TargetEfficacy = "0";
                                //allColumnsAreError = true;
                                //errorInfo = string.Format("第{0}行生产计划没有值", iRow);
                                //break;
                            }

                            if (!ValidIsDouble(IE_TargetEfficacy))
                            {
                                allColumnsAreError = true;
                                errorInfo = string.Format("第{0}行IE标准效率输入不正确", iRow);
                                break;
                            }

                            var IE_DeptHuman = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ++iColumn].Value);
                            //如果部门有效人力为空则默认为0
                            if (string.IsNullOrWhiteSpace(IE_DeptHuman))
                            {
                                IE_DeptHuman = "0";
                                //allColumnsAreError = true;
                                //errorInfo = string.Format("第{0}行目标良率没有值", iRow);
                                //break;
                            }
                            if (!ValidIsDouble(IE_DeptHuman))
                            {
                                allColumnsAreError = true;
                                errorInfo = string.Format("第{0}行部门有效人力输入不正确", iRow);
                                break;
                            }



                            newMGDataItem.FlowChart_Detail_UID = Convert.ToInt32(FlowChart_Detail_UID);
                            switch (iColumn)
                            {

                                case 7:
                                    newMGDataItem.IE_TargetDate = week.Monday;
                                    break;
                                case 8:
                                    newMGDataItem.IE_TargetDate = week.Monday;
                                    break;

                                case 9:
                                    newMGDataItem.IE_TargetDate = week.Tuesday;
                                    break;
                                case 10:
                                    newMGDataItem.IE_TargetDate = week.Tuesday;
                                    break;

                                case 11:
                                    newMGDataItem.IE_TargetDate = week.Wednesday;
                                    break;
                                case 12:
                                    newMGDataItem.IE_TargetDate = week.Wednesday;
                                    break;

                                case 13:
                                    newMGDataItem.IE_TargetDate = week.Thursday;
                                    break;
                                case 14:
                                    newMGDataItem.IE_TargetDate = week.Thursday;
                                    break;

                                case 15:
                                    newMGDataItem.IE_TargetDate = week.Friday;
                                    break;
                                case 16:
                                    newMGDataItem.IE_TargetDate = week.Friday;
                                    break;

                                case 17:
                                    newMGDataItem.IE_TargetDate = week.Saturday;
                                    break;
                                case 18:
                                    newMGDataItem.IE_TargetDate = week.Saturday;
                                    break;

                                case 19:
                                    newMGDataItem.IE_TargetDate = week.Sunday;
                                    break;
                                case 20:
                                    newMGDataItem.IE_TargetDate = week.Sunday;
                                    break;
                            }

                            //四舍五入小数点

                            newMGDataItem.IE_TargetEfficacy = Convert.ToInt32(Convert.ToDouble(IE_TargetEfficacy).ToString("#0"));

                            //四舍五入小数点
                            // newMGDataItem.IE_DeptHuman = Convert.ToDouble(IE_DeptHuman);
                            newMGDataItem.IE_DeptHuman = int.Parse(IE_DeptHuman);
                            newMGDataItem.ShiftTimeID = int.Parse(ShiftTimeID);
                            newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                            newMGDataItem.Modified_Date = DateTime.Now;


                            mgDataList.Add(newMGDataItem);

                        }

                    }
                    
                }

                if (hid_currentOrNextWeek != "day"&&allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("FlowChart/ImportFlowChartIEMGDataAPI?FlowChart_Master_UID={0}", FlowChart_Master_UID);
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

            }
            return errorInfo;
        }

        #region /验证IEExcel的表头是否正确
        private bool validIEExcelTitleIsErrorTwo(ExcelWorksheet worksheet, string[] propertiesHead, int iRow, int totalColumns)
        {
            bool allColumnsAreError = false;
            for (var i = 1; i <= totalColumns; i++)
            {
                if (allColumnsAreError)
                {
                    break;
                }
                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6: //星期一
                        allColumnsAreError = validIEExcelTitleIsError(worksheet, propertiesHead, iRow, i, i - 1);
                        break;

                    default:
                        continue;
                }
            }
            return allColumnsAreError;
        }

        private bool validIEExcelTitleIsError(ExcelWorksheet worksheet, string[] propertiesHead, int iRow, int iColumn, int column)
        {
            bool isError = false;
            if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value) != propertiesHead[column])
            {
                isError = true;
            }
            return isError;
        }
        #endregion

        #endregion


        #endregion

        #region 绑定物料员
        public ActionResult FlowChart_BindBom(int MasterUID, int Version)
        {
            var apiUrl = string.Format("FlowChart/QueryBindBomByMasterId?id={0}", MasterUID);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(result);
            ViewBag.MasterUID = MasterUID;
            ViewBag.Version = Version;
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;

            var roleList = this.CurrentUser.GetUserInfo.RoleList.Select(m => m.Role_ID).ToList();
            ViewBag.RoleList = JsonConvert.SerializeObject(roleList);


            return View();
        }

        public ActionResult QueryBomByFlowChartUID(int MasterUID, int Version)
        {
            //获取用户所属的功能厂权限
            List<int> idList = new List<int>();
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo.ToList();
            foreach (var orgInfoItem in orgInfo)
            {
                if (orgInfoItem.Funplant_OrganizationUID != null)
                {
                    idList.Add(orgInfoItem.Funplant_OrganizationUID.Value);
                }
            }
            var json = JsonConvert.SerializeObject(idList);

            var apiUrl = string.Format("FlowChart/QueryBomByFlowChartUIDAPI?id={0}&Version={1}&Plants={2}", MasterUID, Version, json);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //public List<int> GetFunPlantID()
        //{
        //    //获取所有funcPlant表信息
        //    HttpResponseMessage res = APIHelper.APIGetAsync("Settings/GetAllFuncPlantsAPI");
        //    var resResult = res.Content.ReadAsStringAsync().Result;
        //    var list = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(resResult);
        //    //获取用户所属的opTypes
        //    var orgianVMList = this.CurrentUser.GetUserInfo.OrgInfo;
        //    var opTypes = orgianVMList.Select(m => m.OPType).ToList();

        //    var idList = list.Where(m => opTypes.Contains(m.OP_Types)).Select(m => m.System_FunPlant_UID).ToList();
        //    return idList;
        //}

        public ActionResult QueryBomEditByFlowChartUID(int PC_MH_UID)
        {
            var apiUrl = string.Format("FlowChart/QueryBomEditByFlowChartUIDAPI?PC_MH_UID={0}", PC_MH_UID);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #region 物料员Excel下载检查
        public string CheckDownloadBomExcel(int MasterUID, int Version)
        {
            string errorInfo = string.Empty;
            //获取用户所属的功能厂权限
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo.ToList();
            if (orgInfo.First().Funplant_OrganizationUID == null || orgInfo.First().Funplant_OrganizationUID == 0)
            {
                errorInfo = "改用户还没有分配功能厂，请先分配功能厂";
            }
            if (string.IsNullOrEmpty(errorInfo))
            {
                //获取用户所属的功能厂权限
                List<int> idList = new List<int>();
                foreach (var orgInfoItem in orgInfo)
                {
                    if (orgInfoItem.Funplant_OrganizationUID != null)
                    {
                        idList.Add(orgInfoItem.Funplant_OrganizationUID.Value);
                    }
                }

                var json = JsonConvert.SerializeObject(idList);

                var apiUrl = string.Format("FlowChart/QueryFLDetailByUIDAndVersionAPI?MasterUID={0}&Version={1}&Plants={2}", MasterUID, Version, json);

                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<FlowChartBomGet>>(result);
                if (list.Count() == 0)
                {
                    //没有任何所属的厂区
                    errorInfo = string.Format("System_Function_Plant表中没有分配Key为{0}的主键", json);
                }
            }
            return errorInfo;
        }
        #endregion


        #region 物料员Excel下载
        public FileResult DownloadBomExcel(int MasterUID, int Version)
        {
            //获取用户所属的功能厂权限
            List<int> idList = new List<int>();
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo.ToList();
            foreach (var orgInfoItem in orgInfo)
            {
                if (orgInfoItem.Funplant_OrganizationUID != null)
                {
                    idList.Add(orgInfoItem.Funplant_OrganizationUID.Value);
                }
            }

            var json = JsonConvert.SerializeObject(idList);

            var apiUrl = string.Format("FlowChart/QueryFLDetailByUIDAndVersionAPI?MasterUID={0}&Version={1}&Plants={2}", MasterUID, Version, json);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FlowChartBomGet>>(result);
            if (list.Count() == 0)
            {
                //没有任何所属的厂区
                return null;
            }

            var exlList = list.Select(m => new EXLResult { Process_Seq = m.Process_Seq, Process = m.Process, Place = m.Place, FlowChart_Detail_UID = m.FlowChart_Detail_UID, Color = m.Color, User_Name = m.User_Name }).Distinct();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("维护物料员与制程关系模板");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetBomHeadColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("维护物料员与制程关系模板");

                SetBomExcelStyle(worksheet, propertiesHead);

                int iRow = 2;
                foreach (var item in exlList)
                {
                    //不区分栋别逻辑
                    worksheet.Cells[iRow, 1].Value = item.Process_Seq;
                    worksheet.Cells[iRow, 2].Value = item.Process;
                    worksheet.Cells[iRow, 3].Value = item.Color;
                    worksheet.Cells[iRow, 4].Value = item.Place;
                    if (!string.IsNullOrEmpty(item.User_Name))
                    {
                        worksheet.Cells[iRow, 5].Value = item.User_Name;
                    }
                    worksheet.Cells[iRow, 6].Value = item.FlowChart_Detail_UID;
                    iRow++;
                    #region 区分栋别的逻辑------------------Rock
                    //var placeList = item.Place.Split('/').ToList();
                    //foreach (var placeItem in placeList)
                    //{
                    //    worksheet.Cells[iRow, 1].Value = item.Process_Seq;
                    //    worksheet.Cells[iRow, 2].Value = item.Process;
                    //    worksheet.Cells[iRow, 3].Value = placeItem;
                    //    worksheet.Cells[iRow, 5].Value = item.FlowChart_Detail_UID;
                    //    iRow++;
                    //}
                    #endregion
                }
                //Excel最后一行行号
                int maxRow = iRow - 1;
                //设置灰色背景
                var colorRange = string.Format("A2:C{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置主键列隐藏
                worksheet.Column(6).Hidden = true;
                //设置边框
                worksheet.Cells[string.Format("A1:D{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:D{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:D{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        private string[] GetBomHeadColumn()
        {
            var nextWeek = GetNextWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                "制程序号",
                "制程名称",
                "颜色",
                "厂区",
                "物料员账号",
                "FlowChart_Detail_UID"
            };
            return propertiesHead;
        }

        private void SetBomExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];


            //设置列宽
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 17;
            worksheet.Column(3).Width = 17;
            worksheet.Column(4).Width = 17;
            worksheet.Column(5).Width = 17;
            worksheet.Column(6).Width = 17;


            worksheet.Cells["A1:E1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }

        #endregion

        #region 物料员Excel上传
        public string ImportBomExcel(HttpPostedFileBase upload_excel, int MasterUID, int Version)
        {
            string errorInfo = string.Empty;
            errorInfo = AddBomExcel(upload_excel, MasterUID, Version);
            return errorInfo;
        }

        private string AddBomExcel(HttpPostedFileBase upload_excel, int MasterUID, int Version)
        {
            string errorInfo = string.Empty;

            try
            {
                using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalColumns = worksheet.Dimension.End.Column;
                    if (worksheet == null)
                    {
                        errorInfo = "没有worksheet内容";
                        return errorInfo;
                    }
                    //头样式设置
                    var propertiesHead = GetBomHeadColumn();

                    int iRow = 1;

                    #region 检查Excel模板是否正确
                    bool excelIsError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            var result = worksheet.Cells[iRow, i].Value.ToString();
                            var hasItem = propertiesHead.Where(m => m.Contains(result)).FirstOrDefault();
                            if (hasItem == null)
                            {
                                excelIsError = true;
                                break;
                            }
                        }
                        else
                        {
                            excelIsError = true;
                            break;
                        }
                    }
                    if (excelIsError)
                    {
                        errorInfo = "Excel格式不正确";
                        return errorInfo;
                    }
                    #endregion

                    #region 检查当前功能厂是否存在flowchart物料员帐号了，导入Excel只能新增不能同时新增和修改
                    //var nowFuncPlant = this.CurrentUser.DataPermissions.UserOrgInfo.Org_FuncPlant;
                    //var search = new GetFuncPlantProcessSearch()
                    //{
                    //    Master_Uid=MasterUID,
                    //    Version = Version,
                    //    OwnerFuncPlant = nowFuncPlant
                    //};
                    //var apiUrl = string.Format("FlowChart/CheckBomUserAPI");
                    //HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
                    //var checkResult = responMessage.Content.ReadAsStringAsync().Result;
                    //if (Convert.ToInt32(checkResult) > 0)
                    //{
                    //    errorInfo = "已经存在该Flowchart的物流员信息了，请先解除绑定后再上传";
                    //    return errorInfo;
                    //}
                    #endregion

                    #region 获取所有MHPC数据
                    var apiUrl = string.Format("FlowChart/GetALlPCMHAPI");
                    HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                    var mhResult = responMessage.Content.ReadAsStringAsync().Result;
                    var allMHList = JsonConvert.DeserializeObject<List<FlowChartPCMHRelationshipDTO>>(mhResult);
                    #endregion

                    #region 从User表中获取所有物流员帐号信息
                    var apiBomUserUrl = string.Format("FlowChart/QueryBomUserInfoAPI");
                    HttpResponseMessage responBomUserMessage = APIHelper.APIGetAsync(apiBomUserUrl);
                    var bomUserResult = responBomUserMessage.Content.ReadAsStringAsync().Result;
                    var userList = JsonConvert.DeserializeObject<List<SystemUserDTO>>(bomUserResult);
                    #endregion

                    //读取Excel并进行验证判断
                    List<string> UserNameList = new List<string>();
                    List<FlowChartPCMHRelationshipVM> vmList = new List<FlowChartPCMHRelationshipVM>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        FlowChartPCMHRelationshipVM vmItem = new FlowChartPCMHRelationshipVM();

                        var checkUID = Convert.ToInt32(ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "FlowChart_Detail_UID")].Value));
                        var hasExist = allMHList.Exists(m => m.FlowChart_Detail_UID == checkUID);
                        if (hasExist)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行已经分配了物料员，不能再次分配", i);
                            break;
                        }

                        string process_Seq = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程序号")].Value);
                        if (string.IsNullOrWhiteSpace(process_Seq))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行制程序号没有值", i);
                            break;
                        }

                        string process = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程名称")].Value);
                        if (string.IsNullOrWhiteSpace(process))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行制程名称没有值", i);
                            break;
                        }

                        string place = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                        if (string.IsNullOrWhiteSpace(place))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区没有值", i);
                            break;
                        }

                        string user_NTID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "物料员账号")].Value);
                        if (string.IsNullOrWhiteSpace(user_NTID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行物料员账号没有值", i);
                            break;
                        }
                        else
                        {
                            user_NTID = user_NTID.Trim();
                        }

                        //验证userNTID是否不是[MH_Flag]=0
                        var hasExists = userList.Exists(m => m.User_NTID.ToUpper().Equals(user_NTID.ToUpper()) && m.MH_Flag == false);
                        if (hasExists)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行物料员账号已经存在，并且不是物料员所属的帐号", i);
                            break;
                        }

                        string FlowChart_Detail_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "FlowChart_Detail_UID")].Value);
                        if (string.IsNullOrWhiteSpace(FlowChart_Detail_UID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行主键没有值", i);
                            break;
                        }

                        FlowChartPCMHRelationshipDTO dtoItem = new FlowChartPCMHRelationshipDTO();
                        dtoItem.FlowChart_Detail_UID = Convert.ToInt32(FlowChart_Detail_UID);
                        dtoItem.Place = place;
                        dtoItem.System_Language_UID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                        dtoItem.Modified_UID = this.CurrentUser.AccountUId;
                        dtoItem.Modified_Date = DateTime.Now;
                        vmItem.FlowchartPCDTOItem = dtoItem;

                        vmItem.UserNTID = user_NTID;
                        vmList.Add(vmItem);
                    }

                    if (string.IsNullOrEmpty(errorInfo))
                    {
                        //插入表
                        var json = JsonConvert.SerializeObject(vmList);
                        var apiInsertBomUserUrl = string.Format("FlowChart/InsertBomUserInfoAPI?MasterUID={0}&Version={1}", MasterUID, Version);
                        HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(json, apiInsertBomUserUrl);
                        errorInfo = responMessage1.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    }

                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.ToString();
            }
            return errorInfo;
        }
        #endregion

        public string EditFLPCBomInfo(FlowChartBomGet bomItem, string Old_MH_Name)
        {
            string result = string.Empty;
            //如果修改的时候物料员名称没变，则直接返回成功
            if (Old_MH_Name.ToUpper() != bomItem.User_NTID.ToUpper())
            {
                var apiUrl = string.Format("FlowChart/EditFLPCBomInfoAPI?CurrUser={0}", this.CurrentUser.AccountUId);
                HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(bomItem, apiUrl);
                result = responddlMessage.Content.ReadAsStringAsync().Result;
            }

            return result;
        }

        public void DeleteBomInfoByUIDList(string json)
        {
            var apiUrl = string.Format("FlowChart/DeleteBomInfoByUIDListAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
        }
        #endregion
    }


}