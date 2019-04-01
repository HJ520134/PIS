using DataTables.AspNet.Core;
using Newtonsoft.Json;
using PDMS.Common.Constants;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers.ProductionPlanningController
{
    public class ProductionPlanningReportController : WebControllerBase
    {
        #region 设备需求报表
        // GET: ProductionPlanningReport
        public ActionResult EquipmentRequestList()
        {
            //var plantIDList = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList;
            //var opTypeList = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList;
            //var projectIDList = this.CurrentUser.GetUserInfo.ProjectUIDList;
            Dictionary<int, string> plantDir = new Dictionary<int, string>();
            Dictionary<int, string> opTypeDir = new Dictionary<int, string>();


            var apiUrl = string.Format("ProductionPlanning/GetProjectInfoByUserAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(this.CurrentUser.GetUserInfo, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<ProductionPlanningReportGetProject>(result);
            ViewBag.Plant = item.plantDir;
            ViewBag.OPType = item.opTypeDir;

            return View();
        }

        public ActionResult GetOpTypesByPlantName(string PlantName)
        {
            var apiUrl = string.Format("ProductionPlanning/GetOpTypesByPlantNameAPI?PlantName={0}", PlantName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetProjectByOpType(int OpTypeUID)
        {
            var apiUrl = string.Format("ProductionPlanning/GetProjectByOpTypeAPI?OpTypeUID={0}", OpTypeUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetPartTypesByProject(int ProjectUID)
        {
            var apiUrl = string.Format("ProductionPlanning/GetPartTypesByProjectAPI?ProjectUID={0}", ProjectUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFunPlantByOpType(int OpTypeUID)
        {
            var apiUrl = string.Format("ProductionPlanning/GetFunPlantByOpTypeAPI?OpTypeUID={0}", OpTypeUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetRPTColumnName(ProductionPlanningReportVM vm)
        {
            var apiUrl = string.Format("ProductionPlanning/GetRPTColumnNameAPI?PlantUID={0}&OpTypeUID={1}", vm.PlantUID, vm.OpTypeUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryEquipRPT(ProductionPlanningReportVM vm, Page page)
        {
            string apiUrl = string.Empty;
            DateTime now = DateTime.Now.Date;
            DateTime date = Convert.ToDateTime(vm.DateFrom);

            if (vm.ProjectUID == 0)
            {
                apiUrl = string.Format("ProductionPlanning/QueryEquipRPTAPI");
                if (vm.QueryMode == 1)
                {
                    List<string> monthList = new List<string>();
                    for (int i = 1; i <= 3; i++)
                    {
                        monthList.Add(date.AddMonths(i).ToString(FormatConstants.DateTimeFormatStringByDate));
                    }
                    var selectDate = monthList.Where(m => m.Contains(vm.hidTab)).First();
                    //获取选择的月份的第一天
                    var firstDate = selectDate.Substring(0, 8);
                    firstDate = firstDate + "01";
                    //获取选择的月份的最后一天
                    var lastDate = Convert.ToDateTime(firstDate).AddMonths(1).AddDays(-1);
                    vm.StartDate = firstDate;
                    vm.EndDate = lastDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                }
                else
                {
                    //本月第一天
                    var firstDay = date.AddDays(1 - date.Day);
                    DateTime endDay = firstDay;
                    switch (vm.hidTab)
                    {
                        case "第1周":
                            endDay = firstDay.AddDays(6);
                            break;
                        case "第2周":
                            firstDay = firstDay.AddDays(7);
                            endDay = firstDay.AddDays(6);
                            break;
                        case "第3周":
                            firstDay = firstDay.AddDays(14);
                            endDay = firstDay.AddDays(6);
                            break;
                        case "第4周":
                            firstDay = firstDay.AddDays(21);
                            endDay = firstDay.AddDays(6);
                            break;
                    }
                    vm.StartDate = firstDay.ToString(FormatConstants.DateTimeFormatStringByDate);
                    vm.EndDate = endDay.ToString(FormatConstants.DateTimeFormatStringByDate);
                }
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, page, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var dt = JsonConvert.DeserializeObject<DataTable>(result) as DataTable;
                //获取动态列名
                List<string> columnList = new List<string>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var columnName = dt.Columns[i].ColumnName;
                    if (columnName != "FunPlant" && columnName != "Equipment_Name")
                    {
                        columnList.Add(columnName);
                    }
                }
                var jsonName = JsonConvert.SerializeObject(columnList);
                var test = "{\"columnList\":" + jsonName + "," + "\"data\":" + result + "}";


                //var test = "{\"data\":" + result + "}";
                return Content(test, "application/json");
            }
            else
            {
                apiUrl = string.Format("ProductionPlanning/QueryEquipRPT_ByProjectAPI");
                vm = GetDateVM(vm);

                HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, page, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var dtList = JsonConvert.DeserializeObject<List<DataTable>>(result) as List<DataTable>;
                //获取动态列名
                List<string> columnList = new List<string>();
                for (int i = 0; i < dtList.First().Columns.Count; i++)
                {
                    var columnName = dtList.First().Columns[i].ColumnName;
                    if (columnName != "Process_Seq" && columnName != "Process")
                    {
                        columnList.Add(columnName);
                    }
                }

                var jsonName = JsonConvert.SerializeObject(columnList);


                var test = "{\"columnList\":" + jsonName + "," + "\"data\":" + result + "}";
                //var test = "{\"data\":" + result + "}";
                return Content(test, "application/json");

            }

            //HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, page, apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result;
            //var test = "{\"data\":" + result + "}";
            ////result = string.Format("{\"data\":{0}}", result);
            //return Content(test, "application/json");
        }

        public ActionResult QueryEquipRPTByOPType(ProductionPlanningReportVM vm, Page page)
        {
            string apiUrl = string.Format("ProductionPlanning/QueryEquipRPTByOPTypeAPI");

            DateTime now = DateTime.Now.Date;
            DateTime date = Convert.ToDateTime(vm.DateFrom);

            if (vm.QueryMode == 1)
            {
                List<string> monthList = new List<string>();
                for (int i = 1; i <= 3; i++)
                {
                    monthList.Add(date.AddMonths(i).ToString(FormatConstants.DateTimeFormatStringByDate));
                }
                var selectDate = monthList.Where(m => m.Contains(vm.hidTab)).First();
                //获取选择的月份的第一天
                var firstDate = selectDate.Substring(0, 8);
                firstDate = firstDate + "01";
                //获取选择的月份的最后一天
                var lastDate = Convert.ToDateTime(firstDate).AddMonths(1).AddDays(-1);
                vm.StartDate = firstDate;
                vm.EndDate = lastDate.ToString(FormatConstants.DateTimeFormatStringByDate);

            }
            else
            {
                //本月第一天
                var firstDay = date.AddDays(1 - date.Day);
                DateTime endDay = firstDay;
                switch (vm.hidTab)
                {
                    case "第1周":
                        endDay = firstDay.AddDays(6);
                        break;
                    case "第2周":
                        firstDay = firstDay.AddDays(7);
                        endDay = firstDay.AddDays(6);
                        break;
                    case "第3周":
                        firstDay = firstDay.AddDays(14);
                        endDay = firstDay.AddDays(6);
                        break;
                    case "第4周":
                        firstDay = firstDay.AddDays(21);
                        endDay = firstDay.AddDays(6);
                        break;
                }
                vm.StartDate = firstDay.ToString(FormatConstants.DateTimeFormatStringByDate);
                vm.EndDate = endDay.ToString(FormatConstants.DateTimeFormatStringByDate);
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var test = "{\"data\":" + result + "}";
            return Content(test, "application/json");
        }

        public ActionResult QueryEquipRPTBySingleProject(ReportByProject item, Page page)
        {
            var apiUrl = string.Format("ProductionPlanning/QueryEquipRPTBySingleProjectAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(item, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryEquipRPTByFunc(ReportByProject item, Page page)
        {
            var apiUrl = string.Format("ProductionPlanning/QueryEquipRPTByFuncAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(item, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var dt = JsonConvert.DeserializeObject<DataTable>(result) as DataTable;
            //获取动态列名
            List<string> columnList = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var columnName = dt.Columns[i].ColumnName;
                if (columnName != "FunPlant" && columnName != "Equipment_Name")
                {
                    columnList.Add(columnName);
                }
            }
            var jsonName = JsonConvert.SerializeObject(columnList);
            var test = "{\"columnList\":" + jsonName + "," + "\"data\":" + result + "}";

            return Content(test, "application/json");
        }

        public ActionResult GetColumnNameByProject(ProductionPlanningReportVM vm)
        {
            vm = GetDateVM(vm);
            var apiUrl = string.Format(@"ProductionPlanning/GetColumn_ByProjectAPI?PlantUID={0}&OpTypeUID={1}&ProjectUID={2}&PartTypeUID={3} 
&StartDate={4}&EndDate={5}", vm.PlantUID, vm.OpTypeUID, vm.ProjectUID, vm.PartTypeUID, vm.StartDate, vm.EndDate);


            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        private ProductionPlanningReportVM GetDateVM(ProductionPlanningReportVM vm)
        {
            var selectDate = Convert.ToDateTime(vm.DateFrom);
            var selectYear = selectDate.Year;
            if (vm.QueryMode == 1)
            {

                vm.StartDate = string.Format("{0}-01-01", selectYear);
                vm.EndDate = string.Format("{0}-12-31", selectYear);
            }
            else
            {
                var formatDate = selectDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                vm.StartDate = formatDate;
                vm.EndDate = selectDate.AddDays(1 - selectDate.Day).AddMonths(1).AddDays(-1).ToString(FormatConstants.DateTimeFormatStringByDate);
            }
            return vm;
        }

        #endregion

        #region 预估与实际的对比报表
        public ActionResult PlanAndActualReportList()
        {
            //var plantIDList = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList;
            //var opTypeList = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList;
            //var projectIDList = this.CurrentUser.GetUserInfo.ProjectUIDList;
            Dictionary<int, string> plantDir = new Dictionary<int, string>();
            Dictionary<int, string> opTypeDir = new Dictionary<int, string>();


            var apiUrl = string.Format("ProductionPlanning/GetProjectInfoByUserAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(this.CurrentUser.GetUserInfo, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<ProductionPlanningReportGetProject>(result);
            ViewBag.Plant = item.plantDir;
            ViewBag.OPType = item.opTypeDir;
            return View();
        }

        public ActionResult GetActualAndEstimateHumanInfoByProject(int project, string begin, string end)
        {
            string projects = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Project");
            string seq = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.Seq");
            string process = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Process");
            string estimate = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Estimate");
            string actual = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Actual");

            var apiUrl = string.Format("ProductionPlanningForIE/GetActualAndEstimateHumanInfoAPI?project={0}&beginDate={1}&endDate={2}&projects={3}&seq={4}&process={5}&estimate={6}&actual={7}", project, begin, end, projects, seq, process, estimate, actual);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var test = "{\"data\":" + result + "}";
            return Content(test, "application/json");
        }

        public ActionResult GetActualAndEstimateHumanInfoForProcess(int flowchat, string begin, string end)
        {
            string project = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Project");
            string seq = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.Seq");
            string process = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Process");
            string estimate = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Estimate");
            string actual = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Actual");

            var apiUrl = string.Format("ProductionPlanningForIE/GetActualAndEstimateHumanInfoForProcessAPI?flowchat={0}&beginDate={1}&endDate={2}&project={3}&seq={4}&process={5}&estimate={6}&actual={7}", flowchat, begin, end, project, seq, process, estimate, actual);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var test = "{\"data\":" + result + "}";
            return Content(test, "application/json");
        }

        public ActionResult GetHumanColumnInfo(int project, string begin, string end, int flag)
        {
            string projects = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Project");
            string seq = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.Seq");
            string process = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Process");
            string estimate = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Estimate");
            string actual = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Actual");

            var apiUrl = string.Format("ProductionPlanningForIE/GetHumanColumnAPI?project={0}&beginDate={1}&endDate={2}&flag={3}&projects={4}&seq={5}&process={6}&estimate={7}&actual={8}", project, begin, end, flag, projects, seq, process, estimate, actual);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QueryPlanAndActualReportInfo(ProductionPlanningReportVM vm, Page page)
        {
            var apiUrl = string.Format("ProductionPlanning/QueryPlanAndActualReportInfoAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(result) as DataTable;
            //获取动态列名
            List<string> columnList = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var columnName = dt.Columns[i].ColumnName;
                if (columnName != "Process_Seq" && columnName != "Process" && columnName != "Equipment_Name")
                {
                    columnList.Add(columnName);
                }
            }

            var jsonName = JsonConvert.SerializeObject(columnList);


            var test = "{\"columnList\":" + jsonName + "," + "\"data\":" + result + "}";
            return Content(test, "application/json");
        }




        #endregion


        #region IE 人力需求预估报表----Robert2017/06/05
        public ActionResult DLSummary()
        {
            Dictionary<int, string> plantDir = new Dictionary<int, string>();
            Dictionary<int, string> opTypeDir = new Dictionary<int, string>();


            var apiUrl = string.Format("ProductionPlanning/GetProjectInfoByUserAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(this.CurrentUser.GetUserInfo, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<ProductionPlanningReportGetProject>(result);
            ViewBag.Plant = item.plantDir;
            ViewBag.OPType = item.opTypeDir;

            //List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            //if (Orgnizations != null && Orgnizations.Count > 0)
            //{
            //    ViewBag.OP = Orgnizations[0].Plant;
            //}
            //else
            //{
            //    ViewBag.OP = 0;
            //}
            return View();
        }


        public ActionResult ManpowerDemandForecastRePort()
        {
            //List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            //if (Orgnizations != null && Orgnizations.Count > 0)
            //{
            //    ViewBag.OP = Orgnizations[0].Plant;
            //}
            //else
            //{
            //    ViewBag.OP = 0;
            //}
            return View();
        }

        public ActionResult GetPlanDataByProject(int site, int op, int project, int PartTypeUID, string begin)
        {
            var apiUrl = string.Format("ProductionPlanningForIE/GetPlanDataByProjectAPI?site={0}&op={1}&project={2}&PartTypeUID={3}&begin={4}", site, op, project, PartTypeUID, begin);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 人力需求报表按专案查询
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public ActionResult GetHumanResourcesByProject(int project)
        {
            var apiUrl = string.Format("ProductionPlanningForIE/GetHumanResourcesByProjectAPI?project={0}", project);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 人力需求报表按功能厂查询
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public ActionResult GetHumanResourcesByFunplant(int project)
        {
            var apiUrl = string.Format("ProductionPlanningForIE/GetHumanResourcesByFunplantAPI?project={0}", project);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetInputDataByProject(int op, int project, string begin)
        {
            var apiUrl = string.Format("ProductionPlanningForIE/GetInputDataByProjectAPI?op={0}&project={1}&begin={2}", op, project, begin);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetNowHumanByBG(int op, string begin)
        {
            var apiUrl = string.Format("ProductionPlanningForIE/GetNowHumanByBGAPI?bgOrgID={0}&begindate={1}", op, DateTime.Parse(begin));
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetDemissionRateByBG(int op, string begin)
        {
            var apiUrl = string.Format("ProductionPlanningForIE/GetDemissionRateByBGAPI?bgOrgID={0}&begindate={1}", op, DateTime.Parse(begin));
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region IE人力需求预估报表 --------------- Rock 2017-09-13
        public ActionResult ManPowerRequestList()
        {
            Dictionary<int, string> plantDir = new Dictionary<int, string>();
            Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            var apiUrl = string.Format("ProductionPlanning/GetProjectInfoByUserAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(this.CurrentUser.GetUserInfo, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<ProductionPlanningReportGetProject>(result);
            ViewBag.Plant = item.plantDir;
            ViewBag.OPType = item.opTypeDir;

            return View();
        }

        public string QueryManPowerRequestRPT(ProductionPlanningReportVM vm)
        {
            List<ReportByDatePhase> datePhaseList = new List<ReportByDatePhase>();
            ReportDate phase = new ReportDate();
            var date = Convert.ToDateTime(vm.DateFrom);
            switch (date.DayOfWeek.ToString())
            {
                case "Monday":
                    phase.StartDate = date;
                    phase.EndDate = date.AddDays(6);
                    break;
                case "Tuesday":
                    phase.StartDate = date.AddDays(-1);
                    phase.EndDate = date.AddDays(5);
                    break;
                case "Wednesday":
                    phase.StartDate = date.AddDays(-2);
                    phase.EndDate = date.AddDays(4);
                    break;
                case "Thursday":
                    phase.StartDate = date.AddDays(-3);
                    phase.EndDate = date.AddDays(3);
                    break;
                case "Friday":
                    phase.StartDate = date.AddDays(-4);
                    phase.EndDate = date.AddDays(2);
                    break;
                case "Saturday":
                    phase.StartDate = date.AddDays(-5);
                    phase.EndDate = date.AddDays(1);
                    break;
                case "Sunday":
                    phase.StartDate = date.AddDays(-6);
                    phase.EndDate = date;
                    break;
            }

            ReportByDatePhase pPreviousPhase = new ReportByDatePhase();
            pPreviousPhase.StartDate = phase.StartDate.AddDays(-14).ToString(FormatConstants.DateTimeFormatStringByDate);
            pPreviousPhase.EndDate = phase.EndDate.AddDays(-14).ToString(FormatConstants.DateTimeFormatStringByDate);
            datePhaseList.Add(pPreviousPhase);

            ReportByDatePhase previousPhase = new ReportByDatePhase();
            previousPhase.StartDate = phase.StartDate.AddDays(-7).ToString(FormatConstants.DateTimeFormatStringByDate);
            previousPhase.EndDate = phase.EndDate.AddDays(-7).ToString(FormatConstants.DateTimeFormatStringByDate);
            datePhaseList.Add(previousPhase);

            ReportByDatePhase currentPhase = new ReportByDatePhase();
            currentPhase.StartDate = phase.StartDate.ToString(FormatConstants.DateTimeFormatStringByDate);
            currentPhase.EndDate = phase.EndDate.ToString(FormatConstants.DateTimeFormatStringByDate);
            datePhaseList.Add(currentPhase);

            ReportByDatePhase nextPhase = new ReportByDatePhase();
            nextPhase.StartDate = phase.StartDate.AddDays(7).ToString(FormatConstants.DateTimeFormatStringByDate);
            nextPhase.EndDate = phase.EndDate.AddDays(7).ToString(FormatConstants.DateTimeFormatStringByDate);
            datePhaseList.Add(nextPhase);

            ReportByDatePhase nNextPhase = new ReportByDatePhase();
            nNextPhase.StartDate = phase.StartDate.AddDays(14).ToString(FormatConstants.DateTimeFormatStringByDate);
            nNextPhase.EndDate = phase.EndDate.AddDays(14).ToString(FormatConstants.DateTimeFormatStringByDate);
            datePhaseList.Add(nNextPhase);

            var json = JsonConvert.SerializeObject(datePhaseList);
            return json;
        }

        public ActionResult QueryManPowerProject(ProductionPlanningReportVM vm, Page page)
        {
            var dateStrList = vm.hidTab.Split('~');
            vm.StartDate = dateStrList[0];
            vm.EndDate = dateStrList[1];

            List<string> projectNameList = new List<string>();
            //第一步，先获取所有的专案和部件，不管有没有值都显示到专案列表
            if (vm.ProjectUID == 0)
            {
                var allProjectUrl = string.Format("ProductionPlanningForIE/QueryManPowerRequestRPTByProjectAPI");
                //var allProjectUrl = "ProductionPlanningForIE/QueryManPowerRequestRPTAPI";
                HttpResponseMessage projectMessage = APIHelper.APIPostAsync(vm, page, allProjectUrl);
                var projectResult = projectMessage.Content.ReadAsStringAsync().Result;
                projectNameList = JsonConvert.DeserializeObject<List<string>>(projectResult);

            }


            //第二步，再单独获取有数据的专案和部件
            var apiUrl = string.Format("ProductionPlanningForIE/QueryManPowerRequestRPTAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(result) as DataTable;
            List<string> hasDataList = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                hasDataList.Add(dr["ProjectTypes"].ToString());
            }

            //第三步，把前两步的数据整合到一起输出
            DataTable allDataTable = new DataTable();

            if (hasDataList.Count() > 0)
            {
                foreach (string projectName in projectNameList)
                {
                    var isExist = hasDataList.Exists(m => m.Equals(projectName));
                    if (!isExist)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = projectName;
                        dr[1] = 0;
                        dr[2] = 0;
                        dr[3] = 0;
                        dr[4] = 0;
                        dr[5] = 0;
                        dr[6] = 0;
                        dr[7] = 0;
                        dt.Rows.Add(dr);
                    }
                }
            }

            //获取动态列名
            List<string> columnList = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columnList.Add(dc.ColumnName);
            }

            var jsonName = JsonConvert.SerializeObject(columnList);
            var serialData = JsonConvert.SerializeObject(dt);


            var test = "{\"columnList\":" + jsonName + "," + "\"data\":" + serialData + "}";
            return Content(test, "application/json");
        }

        public ActionResult QueryManPowerRequestByProjectOne(ProductionPlanningReportVM vm, Page page)
        {
            var dateStrList = vm.hidTab.Split('~');
            vm.StartDate = dateStrList[0];
            vm.EndDate = dateStrList[1];
            var currentDate = Convert.ToDateTime(vm.StartDate);

            var url = string.Format("ProductionPlanningForIE/QueryManPowerRequestByProjectOneAPI");
            HttpResponseMessage projectMessage = APIHelper.APIPostAsync(vm, page, url);
            var oneResult = projectMessage.Content.ReadAsStringAsync().Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(oneResult) as DataTable;

            DataTable dtAllData = new DataTable();
            List<string> columnNameList = new List<string>();
            var projectColumnName = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Project");
            columnNameList.Add(projectColumnName);

            //定义列名
            dtAllData.Columns.Add(projectColumnName);
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName != "Project_Name" && dc.ColumnName != "Part_Types" && dc.ColumnName != "Product_Phase")
                {
                    columnNameList.Add(dc.ColumnName);
                    dtAllData.Columns.Add(dc.ColumnName);
                }
            }
            
            foreach (DataRow dr in dt.Rows)
            {
                //专案数据
                DataRow newRow = dtAllData.NewRow();
                newRow[0] = string.Format("{0}({1})", dr["Project_Name"].ToString(), dr["Part_Types"].ToString());
                newRow[1] = dr[3];
                newRow[2] = dr[4];
                newRow[3] = dr[5];
                newRow[4] = dr[6];
                newRow[5] = dr[7];
                newRow[6] = dr[8];
                newRow[7] = dr[9];
                dtAllData.Rows.Add(newRow);

                //MP人力需求数据
                DataRow newRow1 = dtAllData.NewRow();
                newRow1[0] = string.Format("MP人力需求");
                if (dr["Product_Phase"].ToString() == "MP")
                {
                    newRow1[1] = dr[3];
                    newRow1[2] = dr[4];
                    newRow1[3] = dr[5];
                    newRow1[4] = dr[6];
                    newRow1[5] = dr[7];
                    newRow1[6] = dr[8];
                    newRow1[7] = dr[9];
                }
                else
                {
                    newRow1[1] = 0;
                    newRow1[2] = 0;
                    newRow1[3] = 0;
                    newRow1[4] = 0;
                    newRow1[5] = 0;
                    newRow1[6] = 0;
                    newRow1[7] = 0;
                }
                dtAllData.Rows.Add(newRow1);

                //NPI人力需求数据
                DataRow newRow2 = dtAllData.NewRow();
                newRow2[0] = string.Format("NPI人力需求");
                if (dr["Product_Phase"].ToString() == "NPI")
                {
                    newRow2[1] = dr[3];
                    newRow2[2] = dr[4];
                    newRow2[3] = dr[5];
                    newRow2[4] = dr[6];
                    newRow2[5] = dr[7];
                    newRow2[6] = dr[8];
                    newRow2[7] = dr[9];
                }
                else
                {
                    newRow2[1] = 0;
                    newRow2[2] = 0;
                    newRow2[3] = 0;
                    newRow2[4] = 0;
                    newRow2[5] = 0;
                    newRow2[6] = 0;
                    newRow2[7] = 0;
                }
                dtAllData.Rows.Add(newRow2);

                //需求总人力数据
                DataRow newRow3 = dtAllData.NewRow();
                newRow3[0] = "需求总人力";
                newRow3[1] = dr[3];
                newRow3[2] = dr[4];
                newRow3[3] = dr[5];
                newRow3[4] = dr[6];
                newRow3[5] = dr[7];
                newRow3[6] = dr[8];
                newRow3[7] = dr[9];
                dtAllData.Rows.Add(newRow3);
            }

            //如果专案选择ALL则有现有总人力的统计，如果是选择的单个专案则没有现有总人力的统计
            if (vm.ProjectUID == 0)
            {
                //获取按专案-现有总人力MP,现有总人力NPI
                var twoUrl = string.Format("ProductionPlanningForIE/QueryManPowerRequestByProjectOneByTwoAPI");
                HttpResponseMessage twoMessage = APIHelper.APIPostAsync(vm, page, twoUrl);
                var twoResult = twoMessage.Content.ReadAsStringAsync().Result;
                var twoDt = JsonConvert.DeserializeObject<DataTable>(twoResult) as DataTable;
                if (twoDt.Rows.Count > 0)
                {
                    int i = 1;
                    //现有总人力－MP
                    DataRow newRow1 = dtAllData.NewRow();

                    while (currentDate <= Convert.ToDateTime(vm.EndDate))
                    {
                        newRow1[0] = string.Format("现有总人力－MP");
                        DataRow[] drArray = twoDt.Select(string.Format("ProductDate='{0}' And Product_Phase='MP'", currentDate.ToString(FormatConstants.DateTimeFormatStringByDate)));
                        
                        
                        if (drArray.Count() > 0)
                        {
                            newRow1[i] = drArray.First()[0];
                        }
                        else
                        {
                            newRow1[i] = 0;
                        }

                        currentDate = currentDate.AddDays(1);
                        i++;
                    }
                    
                    dtAllData.Rows.Add(newRow1);

                    currentDate = Convert.ToDateTime(vm.StartDate);
                    i = 1;
                    //现有总人力－NPI
                    DataRow newRow2 = dtAllData.NewRow();

                    while (currentDate <= Convert.ToDateTime(vm.EndDate))
                    {
                        newRow2[0] = string.Format("现有总人力－NPI");
                        DataRow[] drArray = twoDt.Select(string.Format("ProductDate='{0}' And Product_Phase='NPI'", currentDate.ToString(FormatConstants.DateTimeFormatStringByDate)));


                        if (drArray.Count() > 0)
                        {
                            newRow2[i] = drArray.First()[0];
                        }
                        else
                        {
                            newRow2[i] = 0;
                        }

                        currentDate = currentDate.AddDays(1);
                        i++;

                    }
                    dtAllData.Rows.Add(newRow2);
                }
                else
                {
                    AddEmptyData(dtAllData, "现有总人力－MP");
                    AddEmptyData(dtAllData, "现有总人力－NPI");
                }
            }


            //如果专案选择ALL则有MP预计离职率,NPI预计离职率，预计招募人力，如果是选择的单个专案则没有
            currentDate = Convert.ToDateTime(vm.StartDate);
            if (vm.ProjectUID == 0)
            {
                var threeUrl = string.Format("ProductionPlanningForIE/QueryManPowerRequestByProjectOneByThreeAPI");
                HttpResponseMessage threeMessage = APIHelper.APIPostAsync(vm, page, threeUrl);
                var threeResult = threeMessage.Content.ReadAsStringAsync().Result;
                var threeDt = JsonConvert.DeserializeObject<DataTable>(threeResult) as DataTable;
                if (threeDt.Rows.Count > 0)
                {
                    int i = 1;
                    //现有总人力－MP
                    DataRow newRow1 = dtAllData.NewRow();

                    while (currentDate <= Convert.ToDateTime(vm.EndDate))
                    {
                        newRow1[0] = string.Format("MP预计离职率");
                        DataRow[] drArray = threeDt.Select(string.Format("Product_Date='{0}'", currentDate.ToString(FormatConstants.DateTimeFormatStringByDate)));


                        if (drArray.Count() > 0)
                        {
                            var dataMp = drArray.First()["DRN"] ?? 0;
                            newRow1[i] = Convert.ToDecimal(dataMp) * 100 + "%";
                        }
                        else
                        {
                            newRow1[i] = 0;
                        }

                        currentDate = currentDate.AddDays(1);
                        i++;
                    }

                    dtAllData.Rows.Add(newRow1);

                    currentDate = Convert.ToDateTime(vm.StartDate);
                    i = 1;
                    //NPI预计离职率
                    DataRow newRow2 = dtAllData.NewRow();

                    while (currentDate <= Convert.ToDateTime(vm.EndDate))
                    {
                        newRow2[0] = string.Format("NPI预计离职率");
                        DataRow[] drArray = threeDt.Select(string.Format("Product_Date='{0}'", currentDate.ToString(FormatConstants.DateTimeFormatStringByDate)));


                        if (drArray.Count() > 0)
                        {
                            var dataNPI = drArray.First()["DRM"] ?? 0;
                            newRow2[i] = Convert.ToDecimal(dataNPI) * 100 + "%";
                        }
                        else
                        {
                            newRow2[i] = 0;
                        }

                        currentDate = currentDate.AddDays(1);
                        i++;
                    }
                    dtAllData.Rows.Add(newRow2);

                    currentDate = Convert.ToDateTime(vm.StartDate);
                    i = 1;
                    //预计招募人力
                    DataRow newRow3 = dtAllData.NewRow();

                    while (currentDate <= Convert.ToDateTime(vm.EndDate))
                    {
                        newRow3[0] = string.Format("预计招募人力");
                        DataRow[] drArray = threeDt.Select(string.Format("Product_Date='{0}'", currentDate.ToString(FormatConstants.DateTimeFormatStringByDate)));


                        if (drArray.Count() > 0)
                        {
                            newRow3[i] = drArray.First()["Recruit"];
                        }
                        else
                        {
                            newRow3[i] = 0;
                        }

                        currentDate = currentDate.AddDays(1);
                        i++;
                    }
                    dtAllData.Rows.Add(newRow3);
                }
            }
            else
            {
                AddEmptyData(dtAllData, "MP预计离职率");
                AddEmptyData(dtAllData, "NPI预计离职率");
                AddEmptyData(dtAllData, "预计招募人力");
            }





            var jsonName = JsonConvert.SerializeObject(columnNameList);
            var serialData = JsonConvert.SerializeObject(dtAllData);

            var test = "{\"columnList\":" + jsonName + "," + "\"data\":" + serialData + "}";
            return Content(test, "application/json");

        }

        private void AddEmptyData(DataTable dtAllData, string Title)
        {
            DataRow newRow = dtAllData.NewRow();
            newRow[0] = Title;
            newRow[1] = 0;
            newRow[2] = 0;
            newRow[3] = 0;
            newRow[4] = 0;
            newRow[5] = 0;
            newRow[6] = 0;
            newRow[7] = 0;
            dtAllData.Rows.Add(newRow);
        }

        public ActionResult QueryManPowerRequestByProjectTwo(ProductionPlanningReportVM vm, Page page)
        {
            var dateStrList = vm.hidTab.Split('~');
            vm.StartDate = dateStrList[0];
            vm.EndDate = dateStrList[1];
            var currentDate = Convert.ToDateTime(vm.StartDate);

            var url = string.Format("ProductionPlanningForIE/QueryManPowerRequestByFuncOneAPI");
            HttpResponseMessage projectMessage = APIHelper.APIPostAsync(vm, page, url);
            var oneResult = projectMessage.Content.ReadAsStringAsync().Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(oneResult) as DataTable;

            DataTable dtAllData = new DataTable();
            List<string> columnNameList = new List<string>();
            var projectColumnName = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Functionfactory");
            columnNameList.Add(projectColumnName);

            //定义列名
            dtAllData.Columns.Add(projectColumnName);
            while (currentDate <= Convert.ToDateTime(vm.EndDate))
            {
                columnNameList.Add(currentDate.ToString(FormatConstants.DateTimeFormatStringByDate));
                dtAllData.Columns.Add(currentDate.ToString(FormatConstants.DateTimeFormatStringByDate));
                currentDate = currentDate.AddDays(1);
            }

            foreach (DataRow dr in dt.Rows)
            {
                //功能厂数据
                DataRow newRow = dtAllData.NewRow();
                newRow[0] = dr["FunPlant"];
                newRow[1] = dr["Total"];
                newRow[2] = dr["Total"];
                newRow[3] = dr["Total"];
                newRow[4] = dr["Total"];
                newRow[5] = dr["Total"];
                newRow[6] = dr["Total"];
                newRow[7] = dr["Total"];
                dtAllData.Rows.Add(newRow);
            }

            //定义需求总人力
            int d1 = 0, d2 = 0, d3 = 0, d4 = 0, d5 = 0, d6 = 0, d7 = 0;
            //MP人力需求数据
            DataRow newRow1 = dtAllData.NewRow();
            newRow1[0] = string.Format("MP人力需求");
            DataRow[] drArray = dt.Select(string.Format("Product_Phase='MP'"));
            if (drArray.Count() > 0)
            {
                var sumMP = 0;
                foreach (DataRow drArrayItem in drArray)
                {
                    sumMP += Convert.ToInt32(drArrayItem["Total"]);
                }
                newRow1[1] = sumMP;
                d1 += sumMP;
                newRow1[2] = sumMP;
                d2 += sumMP;
                newRow1[3] = sumMP;
                d3 += sumMP;
                newRow1[4] = sumMP;
                d4 += sumMP;
                newRow1[5] = sumMP;
                d5 += sumMP;
                newRow1[6] = sumMP;
                d6 += sumMP;
                newRow1[7] = sumMP;
                d7 += sumMP;
                dtAllData.Rows.Add(newRow1);
            }
            else
            {
                AddEmptyData(dtAllData, "MP人力需求");
            }

            //NPI人力需求数据
            DataRow newRow2 = dtAllData.NewRow();
            newRow2[0] = string.Format("NPI人力需求");
            DataRow[] drArrayNPI = dt.Select(string.Format("Product_Phase='NPI'"));
            if (drArrayNPI.Count() > 0)
            {
                var sumNPI = 0;
                foreach (DataRow drArrayItem in drArrayNPI)
                {
                    sumNPI += Convert.ToInt32(drArrayItem["Total"]);
                }
                newRow2[1] = sumNPI;
                d1 += sumNPI;
                newRow2[2] = sumNPI;
                d2 += sumNPI;
                newRow2[3] = sumNPI;
                d3 += sumNPI;
                newRow2[4] = sumNPI;
                d4 += sumNPI;
                newRow2[5] = sumNPI;
                d5 += sumNPI;
                newRow2[6] = sumNPI;
                d6 += sumNPI;
                newRow2[7] = sumNPI;
                d7 += sumNPI;
                dtAllData.Rows.Add(newRow2);
            }
            else
            {
                AddEmptyData(dtAllData, "NPI人力需求");
            }

            //需求总人力数据
            DataRow newRow3 = dtAllData.NewRow();
            newRow3[0] = "需求总人力";
            newRow3[1] = d1;
            newRow3[2] = d2;
            newRow3[3] = d3;
            newRow3[4] = d4;
            newRow3[5] = d5;
            newRow3[6] = d6;
            newRow3[7] = d7;
            dtAllData.Rows.Add(newRow3);




            var jsonName = JsonConvert.SerializeObject(columnNameList);
            var serialData = JsonConvert.SerializeObject(dtAllData);

            var test = "{\"columnList\":" + jsonName + "," + "\"data\":" + serialData + "}";
            return Content(test, "application/json");

        }
        #endregion
    }
}