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
using System.Linq;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Web.Script.Serialization;

namespace PDMS.Web.Controllers
{
    public class EventReportManagerController : WebControllerBase
    {
        // GET: EventReportManager
        public ActionResult Index()
        {
            return View();
        }

        #region WarningList----------------------------------Destiny 2015/12/9
        public ActionResult WarningList()
        {
            ViewBag.PageTitle = null;
            return View();
        }

        public ActionResult GetWarningList()
        {
            var warning = new WarningSearch
            {
                user_account_uid = this.CurrentUser.AccountUId,
                OpTypes = this.CurrentUser.DataPermissions.Op_Types,
                Project_UID = this.CurrentUser.DataPermissions.Project_UID
            };
            string apiUrl = string.Format("EventReportManager/GetWarningListAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(warning, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult PrepareForEditWarningListData(int Warning_UID)
        {
            string apiUrl = string.Format("EventReportManager/GetMasterUidByWarningUidAPI?Warning_UID={0}", Warning_UID);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            var entity = JsonConvert.DeserializeObject<string>(result);
            return Content(entity, "application/json");
        }
        #endregion

        #region  修改不可用wip  -----------------------2016-11-22 add by karl
        public ActionResult ModifyNullWIP()
        {
            int yesterday = 0;
            if (DateTime.Now.Hour < 2)
            {
                yesterday = 1;
                ViewBag.date = DateTime.Today.AddDays(-1).ToString(FormatConstants.DateTimeFormatStringByDate);
            }
            else
                ViewBag.date = DateTime.Today.ToString(FormatConstants.DateTimeFormatStringByDate);
            ViewBag.yesterday = yesterday;
            var apiUrl = string.Format("EventReportManager/QueryUserRoleAPI?userid={0}", CurrentUser.AccountUId);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var roles = JsonConvert.DeserializeObject<List<string>>(result);
            int modify = 0;
            foreach (var item in roles)
            {
                if (item == "NullWIPModify")
                {
                    modify = 1;
                    ViewBag.Modify = 1;
                }
            }
            apiUrl = string.Format("EventReportManager/GetNowIntervalAPI?type={0}", -2);
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var nowperiod = JsonConvert.DeserializeObject<string>(result);
            ViewBag.nowperiod = nowperiod;
            if (modify == 1)
            {
                apiUrl = string.Format("EventReportManager/GetNowIntervalAPI?type={0}", -4);
                responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                var preperiod = JsonConvert.DeserializeObject<string>(result);
                ViewBag.preperiod = preperiod;
            }
            else
                ViewBag.preperiod = nowperiod;
            return View();
        }

        public ActionResult QueryNullWIPDatas(PPCheckDataSearch search, Page page)
        {
            var apiUrl = string.Format("EventReportManager/QueryNullWIPDatasAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        // 修改不可用wip
        public ActionResult EditNullWIP(int product_uid, int nullwip_qty, string timeinterval)
        {
            var apiUrl = string.Format("EventReportManager/EditNullWIPAPI?product_uid={0}&&nullwip_qty={1}&&modifiedUser={2}",
                product_uid, nullwip_qty, this.CurrentUser.AccountUId, timeinterval);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetSelctMasterUID(string ProjectName, string Part_Types, string Product_Phase, string opType)
        {
            var apiUrl = string.Format("EventReportManager/GetSelctMasterUIDAPI?ProjectName={0}&&Part_Types={1}&&Product_Phase={2}&&opType={3}",
                ProjectName, Part_Types, Product_Phase, opType);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult EditWIPWithZero(string jsonExportList)
        {
            var entity = JsonConvert.DeserializeObject<ExportNullWipData>(jsonExportList);
            var search = new ExportSearch
            {
                Time_Interval = entity.Interval_Time,
                Product_Date = entity.Product_Date,
                Customer = entity.Customer,
                Project = entity.Project,
                Product_Phase = entity.Product_Phase,
                Part_Types = entity.Part_Types,
                Color = entity.Color,
                Modified_UID = CurrentUser.AccountUId
            };
            var apiUrl = string.Format("EventReportManager/EditWIPWithZeroAPI");
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DoExportNULLWipData(string jsonExportList)
        {
            //创建EXL文档
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("不可用wip未维护数据");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "制程序号", "场地", "功能厂", "制程", "颜色", "负责主管", "目标良率",
                "计划", "滚动计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "滚动达成率", "最终良率",
                "WIP", "不可用WIP","可用WIP" };
            var entity = JsonConvert.DeserializeObject<ExportNullWipData>(jsonExportList);
            var search = new ExportSearch
            {
                Time_Interval = entity.Interval_Time,
                Product_Date = entity.Product_Date,
                Customer = entity.Customer,
                Project = entity.Project,
                Product_Phase = entity.Product_Phase,
                Part_Types = entity.Part_Types,
                Color = entity.Color
            };
            var apiUrl = string.Format("EventReportManager/DoExportFunctionAPI");
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<ExportNullWIP_Data>>(result).ToList();


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("不可用wip未维护数据");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Process_Seq;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Place;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Process;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Color;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.FunPlant_Manager;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Target_Yield;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Product_Plan;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Product_Plan_Sum;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Picking_QTY;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.WH_Picking_QTY;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Good_QTY;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Adjust_QTY;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.WH_QTY;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.NG_QTY;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Rolling_Yield_Rate;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Finally_Field;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.WIP_QTY;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.NullWip_QTY;
                    worksheet.Cells[index + 2, 21].Value = currentRecord.OKWip_QTY;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }

        #endregion

        #region  报表
        /// <summary>
        /// PPCheckData
        /// </summary>
        /// <returns></returns>
        public ActionResult PPCheckData()
        {
            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            int iswx = 0;
            foreach (OrganiztionVM item in userinfo)
            {
                if (item.Plant == "WUXI_M")
                {
                    iswx = 1;
                }
            }
            ViewBag.IsWX = iswx;
            return View();
        }
        /// <summary>
        /// 获取为未达成原因
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUnacommpolished_Reason()
        {
            var apiUrl = string.Format("EventReportManager/GetUnacommpolished_ReasonAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// QueryPPCheckDatas
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryPPCheckDatas(PPCheckDataSearch search, Page page)
        {
            var apiUrl = string.Format("EventReportManager/QueryPPCheckDatasAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 修改WIP
        /// </summary>
        /// <param name="jsonPPCheckList"></param>
        /// <returns></returns>
        public ActionResult EditWIPView(int product_uid, int wip_qty, int wip_old, int wip_add, string comment, int nullwip)
        {
            var apiUrl = string.Format("EventReportManager/EditWIPViewAPI?product_uid={0}&&wip_qty={1}&&wip_old={2}" +
                                        "&&wip_add={3}&&comment={4}&&modifiedUser={5}&&nullwip={6}",
                                        product_uid, wip_qty, wip_old, wip_add, comment, this.CurrentUser.AccountUId, nullwip);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 修改WIP
        /// </summary>
        /// <param name="jsonPPCheckList"></param>
        /// <returns></returns>
        public ActionResult EditWIP(string jsonPPCheckList)
        {
            var apiUrl = "EventReportManager/EditWIPAPI";
            var entity = JsonConvert.DeserializeObject<PPEditWIP>(jsonPPCheckList);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult EditWIPNew(string jsonPPCheckList)
        {
            var apiUrl = "EventReportManager/EditWIPNewAPI?product_uid={0}";
            var entity = JsonConvert.DeserializeObject<PPEditWIP>(jsonPPCheckList);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //同步Mes数据
        public ActionResult SynchronizeMesInfo(string jsonmesSyncParam)
        {
            //var apiUrl = "MesStationSync/SynchronizeMesInfoAPI";
            var entity = JsonConvert.DeserializeObject<MesSyncParam>(jsonmesSyncParam);
            string BU_D_Name = entity.Customer;
            string Project_Name = entity.Project;
            string Product_Phase = entity.PhaseName;
            string Part_Types = entity.PartTypes;
            int FlowChartMaster_UID = 0;
            if (!string.IsNullOrEmpty(BU_D_Name) && !string.IsNullOrEmpty(Project_Name) && !string.IsNullOrEmpty(Part_Types) && !string.IsNullOrEmpty(Product_Phase))
            {
                //获取对应的FlowCharMaster
                var flowChartMasterIDAPI = string.Format("FlowChart/GetFlowChartMasterID?BU_D_Name={0}&Project_Name={1}&Part_Types={2}&Product_Phase={3}", BU_D_Name, Project_Name, Part_Types, Product_Phase);
                HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(flowChartMasterIDAPI);
                var flowChartMasterID = plantsmessage.Content.ReadAsStringAsync().Result;
                FlowChartMaster_UID = int.Parse(flowChartMasterID);
                if (int.Parse(flowChartMasterID) == 0)
                {
                    var resultFlowchartMaster = string.Format("未找到对应:客户:{0},专案:{1},部件类型:{2},生产阶段:{3}", BU_D_Name, Project_Name, Part_Types, Product_Phase);
                    return Content(resultFlowchartMaster, "application/json");
                }
            }

            //entity.Modified_UID = this.CurrentUser.AccountUId;
            var apiUrl = string.Format("ProcessIDTRSConfig/SynchronizeMesInfoAPI?currentDate={0}&currentInterval={1}&FlowChartMaster_UID={2}", entity.currentDate, entity.currentInterval, FlowChartMaster_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //同步Mes数据
        public string  IsSyneProjectName(string ProjectName)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/IsSyneProjectNameAPI?ProjectName={0}", ProjectName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (result.Contains(ProjectName))
            {
                return "SHOW";
            }
            else
            {
                return "HIDDLE";
            }
        }

        public ActionResult DoExportPPCheckData(string jsonExportList, string submitType, string exportName)
        {
            //创建EXL文档
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PP检核资料数据");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new List<string> { "序号", "制程序号", "场地", "功能厂", "制程", "颜色", "负责主管", "目标良率", "计划", "滚动计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "滚动达成率", "未达成原因", "最终良率", "WIP", "滚动计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "滚动达成率", "最终良率", "WIP" };
            var stringSubs = new List<string> { "滚动计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "滚动达成率", "最终良率", "WIP" };
            int columnIndex = 0;
            using (var excelPackage = new ExcelPackage(stream))
            {
                //定义变量
                var entity = JsonConvert.DeserializeObject<ExportPPCheckData>(jsonExportList);
                var sheetName = entity.TabList[1].Time_InterVal;
                var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                if (!entity.TabList.Any())
                {
                    return Content("导出EXL数据前，请先查询数据！");
                }
                var queryData = new ExportSearch
                {
                    Time_Interval = sheetName,
                    Product_Date = DateTime.Now,
                    Customer = entity.Customer,
                    Project = entity.Project,
                    Product_Phase = entity.Product_Phase,
                    Part_Types = entity.Part_Types,
                    Color = entity.Color
                };
                var apiUrl = string.Format("EventReportManager/ExportPPCheckDataAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(queryData, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<ExportPPCheck_Data>>(result);
                //创建EXL头部
                worksheet.Cells[1, 1, 1, 9].Merge = true;
                worksheet.Cells[1, 1].Value = "客户:" + entity.Customer + " 专案:" + entity.Project + " 部件:" + entity.Part_Types + " 日期:" + entity.Reference_Date + " 2H战情报表";
                worksheet.Cells[2, 1, 2, 9].Merge = true;
                worksheet.Cells[2, 1].Value = "Update:" + entity.Reference_Date;
                worksheet.Cells[3, 1, 3, 9].Merge = true;
                worksheet.Cells[3, 10, 3, 19].Merge = true;
                worksheet.Cells[3, 10].Value = "当日生产达成状况";
                worksheet.Cells[3, 20, 3, stringHeads.Count()].Merge = true;
                worksheet.Cells[3, 20].Value = "时段：" + sheetName;
                //创建标题
                columnIndex = stringHeads.Count();
                for (int colIndex = 0; colIndex < columnIndex; colIndex++)
                {
                    worksheet.Cells[4, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < list.Count; index++)
                {

                    var currentRecord = list[index];
                    worksheet.Cells[index + 5, 1].Value = index + 1;
                    worksheet.Cells[index + 5, 2].Value = currentRecord.Process_Seq;
                    worksheet.Cells[index + 5, 3].Value = currentRecord.Place;
                    worksheet.Cells[index + 5, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 5, 5].Value = currentRecord.Process;
                    worksheet.Cells[index + 5, 6].Value = currentRecord.Color;
                    worksheet.Cells[index + 5, 7].Value = currentRecord.FunPlant_Manager;
                    worksheet.Cells[index + 5, 8].Value = currentRecord.Target_Yield + "%";
                    worksheet.Cells[index + 5, 9].Value = currentRecord.Product_Plan;
                    worksheet.Cells[index + 5, 10].Value = currentRecord.Product_Plan_Sum;
                    worksheet.Cells[index + 5, 11].Value = currentRecord.Picking_QTY;
                    worksheet.Cells[index + 5, 12].Value = currentRecord.WH_Picking_QTY;
                    worksheet.Cells[index + 5, 13].Value = currentRecord.Good_QTY;
                    worksheet.Cells[index + 5, 14].Value = currentRecord.Adjust_QTY;
                    worksheet.Cells[index + 5, 15].Value = currentRecord.WH_QTY;
                    worksheet.Cells[index + 5, 16].Value = currentRecord.NG_QTY;
                    worksheet.Cells[index + 5, 17].Value = currentRecord.Rolling_Yield_Rate + "%";
                    worksheet.Cells[index + 5, 18].Value = currentRecord.Unacommpolished_Reason;
                    worksheet.Cells[index + 5, 19].Value = currentRecord.Finally_Field + "%";
                    worksheet.Cells[index + 5, 20].Value = currentRecord.WIP_QTY;

                    worksheet.Cells[index + 5, 21].Value = currentRecord.Product_Plan_Sum_Now;
                    worksheet.Cells[index + 5, 22].Value = currentRecord.Picking_QTY_Now;
                    worksheet.Cells[index + 5, 23].Value = currentRecord.WH_Picking_QTY_Now;
                    worksheet.Cells[index + 5, 24].Value = currentRecord.Good_QTY_Now;
                    worksheet.Cells[index + 5, 25].Value = currentRecord.Adjust_QTY_Now;
                    worksheet.Cells[index + 5, 26].Value = currentRecord.WH_QTY_Now;
                    worksheet.Cells[index + 5, 27].Value = currentRecord.NG_QTY_Now;
                    worksheet.Cells[index + 5, 28].Value = currentRecord.Rolling_Yield_Rate_Now + "%";
                    worksheet.Cells[index + 5, 29].Value = currentRecord.Finally_Field_Now + "%";
                    worksheet.Cells[index + 5, 30].Value = currentRecord.WIP_QTY_Now;
                }

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            if (submitType == "AJAX")
            {
                return Content("SUCCESS");
            }
            else
            {
                return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }
        /// <summary>
        /// 获取Comfirm标志
        /// </summary>
        /// <param name="ComfirmJson"></param>
        /// <returns></returns>
        public ActionResult GetComfirmValue(string ComfirmJson)
        {
            var ComfirmResult = "";
            var entity = JsonConvert.DeserializeObject<ExportPPCheckData>(ComfirmJson);
            foreach (var item in entity.TabList)
            {
                //获取到数据源
                var QueryData = new PPCheckDataSearch();
                QueryData.Customer = entity.Customer;
                QueryData.Color = entity.Color;
                QueryData.Part_Types = entity.Part_Types;
                QueryData.Interval_Time = entity.Time_InterVal;
                QueryData.Project = entity.Project;
                QueryData.Reference_Date = entity.Reference_Date;
                QueryData.Tab_Select_Text = item.Time_InterVal;
                QueryData.Product_Phase = entity.Product_Phase;
                QueryData.OP = entity.OP;
                var page = new Page();
                page.PageSize = 10;
                var apiUrl1 = string.Format("EventReportManager/QueryPPCheckDatasAPI");
                HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(QueryData, page, apiUrl1);
                var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<ExportPPCheckDataResult>(result1);
                foreach (var listItem in list.Items)
                {
                    if (listItem.Is_Comfirm == 1)
                        ComfirmResult = "Fail";
                    break;
                }
                if (ComfirmResult == "Fail")
                    break;
            }
            return Content("Fail", "application/json");
        }
        /// <summary>
        /// PP检核时，检查是否所有功能厂都有提供数据
        /// </summary>
        /// <param name="jsonList"></param>
        /// <returns></returns>
        public ActionResult CheckFunPlantDataIsFull(string jsonList)
        {
            var entity = JsonConvert.DeserializeObject<ExportPPCheckData>(jsonList);
            var pp = new PPCheckDataSearch
            {
                Customer = entity.Customer,
                Color = entity.Color,
                Interval_Time = entity.Time_InterVal,
                Part_Types = entity.Part_Types,
                Product_Phase = entity.Product_Phase,
                Project = entity.Project,
                Reference_Date = entity.Reference_Date
            };
            foreach (var en in entity.TabList)
            {
                //仅检查当前时段，当日汇总不检查
                if (en.Time_InterVal != "ALL")
                    pp.Tab_Select_Text = en.Time_InterVal;
            }
            var apiUrl = string.Format("EventReportManager/CheckFunPlantDataIsFullAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(pp, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult CheckProductDataIsFull(string jsonList)
        {
            var entity = JsonConvert.DeserializeObject<ExportPPCheckData>(jsonList);
            var pp = new PPCheckDataSearch
            {
                OP = entity.OP,
                Customer = entity.Customer,
                Color = entity.Color,
                Interval_Time = entity.Time_InterVal,
                Part_Types = entity.Part_Types,
                Product_Phase = entity.Product_Phase,
                Project = entity.Project,
                Reference_Date = entity.Reference_Date
            };
            foreach (var en in entity.TabList)
            {
                //仅检查当前时段，当日汇总不检查
                if (en.Time_InterVal != "ALL")
                    pp.Tab_Select_Text = en.Time_InterVal;
            }
            var apiUrl = string.Format("EventReportManager/CheckProductDataIsFullAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(pp, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult ProductReportDisplay()
        {
            var dateFrom = DateTime.Now;
            #region ---------------------------------------------Modify by Rock 2016/03/18 Start
            var apiUrl = string.Format("EventReportManager/GetSystemViewColumnAPI?Account_UID={0}", this.CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<ProductReportDisplay>(result);
            var allCheckboxList = item.ColumnDTOList;
            var isCheckedList = item.ViewDTOList;

            if (isCheckedList.Count() > 0)
            {
                foreach (var isCheckItem in isCheckedList)
                {
                    var hasItem = allCheckboxList.Where(m => m.Column_UID == isCheckItem.Column_UID).FirstOrDefault();
                    if (hasItem != null)
                    {
                        hasItem.isChecked = true;
                    }
                }
            }

            ViewBag.Column = allCheckboxList;

            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            bool PlantFlag = false;
            foreach (OrganiztionVM t in userinfo)
            {
                if (t.Plant == "WUXI_M")
                {
                    PlantFlag = true;
                }
            }
            ViewBag.PlantFlag = PlantFlag;

            #endregion ---------------------------------------------Modify by Rock 2016/03/18 End
            var dateTo = DateTime.Now;
            var result1 = dateTo - dateFrom;
            return View();
        }

        public ActionResult ProductReportInterval()
        {
            var dateFrom = DateTime.Now;
            #region ---------------------------------------------Modify by Rock 2016/03/18 Start
            var apiUrl = string.Format("EventReportManager/GetSystemViewColumnAPI?Account_UID={0}", this.CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<ProductReportDisplay>(result);
            var allCheckboxList = item.ColumnDTOList;
            var isCheckedList = item.ViewDTOList;

            if (isCheckedList.Count() > 0)
            {
                foreach (var isCheckItem in isCheckedList)
                {
                    var hasItem = allCheckboxList.Where(m => m.Column_UID == isCheckItem.Column_UID).FirstOrDefault();
                    if (hasItem != null)
                    {
                        hasItem.isChecked = true;
                    }
                }
            }

            ViewBag.Column = allCheckboxList;

            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            bool PlantFlag = false;
            foreach (OrganiztionVM t in userinfo)
            {
                if (t.Plant == "WUXI_M")
                {
                    PlantFlag = true;
                }
            }
            ViewBag.PlantFlag = PlantFlag;

            #endregion ---------------------------------------------Modify by Rock 2016/03/18 End
            var dateTo = DateTime.Now;
            var result1 = dateTo - dateFrom;
            return View();
        }


        public ActionResult ProductReport()
        {
            var dateFrom = DateTime.Now;
            #region ---------------------------------------------Modify by Rock 2016/03/18 Start
            var apiUrl = string.Format("EventReportManager/GetSystemViewColumnAPI?Account_UID={0}", this.CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<ProductReportDisplay>(result);
            var allCheckboxList = item.ColumnDTOList;
            var isCheckedList = item.ViewDTOList;

            if (isCheckedList.Count() > 0)
            {
                foreach (var isCheckItem in isCheckedList)
                {
                    var hasItem = allCheckboxList.Where(m => m.Column_UID == isCheckItem.Column_UID).FirstOrDefault();
                    if (hasItem != null)
                    {
                        hasItem.isChecked = true;
                    }
                }
            }

            ViewBag.Column = allCheckboxList;

            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            bool PlantFlag = false;
            foreach (OrganiztionVM t in userinfo)
            {
                if (t.Plant == "WUXI_M")
                {
                    PlantFlag = true;
                }
            }
            ViewBag.PlantFlag = PlantFlag;

            #endregion ---------------------------------------------Modify by Rock 2016/03/18 End
            var dateTo = DateTime.Now;
            var result1 = dateTo - dateFrom;
            return View();
        }
        /// <summary>
        /// 初始化是传入数据
        /// </summary>
        /// <param name="FlowChart_Master_UID"></param>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public ActionResult LocationList(string FlowChart_Detail_UID)
        {
            ViewBag.FlowChart_Detail_UID = FlowChart_Detail_UID;
            //ViewBag.ProjectName = ProjectName;


            return View();

        }
        /// <summary>
        /// 加载详细列表数据方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult QueryLocationLists(Flowchart_Detail_ProductionPlanning entity)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/QueryFlowChartsAPI?Flowchart_Master_UID={0}&flag=1", entity.Flowchart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }



        #region ---------------------------------------------Add by Rock 2016/03/21 更新数据列状态 Start
        public ActionResult UpdateColumnInfo(string column_Index, bool isDisplay)
        {
            var apiUrl = string.Format("EventReportManager/UpdateColumnInfoAPI?Account_UID={0}&column_Index={1}&isDisplay={2}",
                this.CurrentUser.AccountUId, Convert.ToInt32(column_Index), isDisplay);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion ---------------------------------------------Add by Rock 2016/03/21 更新数据列状态 End

        /// <summary>
        /// 查询日报表数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryReportDatas(ReportDataSearch search, Page page)
        {
            //获取指定专案所在的OP

            var dateFrom = DateTime.Now;
            var apiUrl = string.Format("EventReportManager/QueryReportDatasAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dateTo = DateTime.Now;
            var result1 = dateTo - dateFrom;
            return Content(result, "application/json");
        }
        public ActionResult QueryReportDatasInterval(ReportDataSearch search, Page page)
        {
            //获取指定专案所在的OP

            var dateFrom = DateTime.Now;
            var apiUrl = string.Format("EventReportManager/QueryReportDatasIntervalAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var dateTo = DateTime.Now;
            var result1 = dateTo - dateFrom;
            return Content(result, "application/json");
        }

        public ActionResult QuerySumReportDatas(ReportDataSearch search, Page page)
        {

            var apiUrl = string.Format("EventReportManager/QuerySumReportDatasAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="jsonExportList"></param>
        /// <returns></returns>
        public ActionResult DoExportIntervalData(string jsonexportlist, string submitType, string exportName)
        {
            var entity = JsonConvert.DeserializeObject<ReportDataSearch>(jsonexportlist);
            string sheetName = string.Empty;
            DateTime startDay = new DateTime();
            DateTime endDay = new DateTime();
            switch (entity.Select_Type)
            {
                case "monthly":
                    startDay = new DateTime(entity.Month_Date_Start.Value.Year, entity.Month_Date_Start.Value.Month, 1);
                    endDay = DateTime.Parse(entity.Month_Date_End.ToString());
                    DateTime referenceDay = DateTime.Parse(entity.Month_Date_Start.ToString());
                    sheetName = referenceDay.Year + "年" + referenceDay.Month + "月";

                    //get Export datas
                    var apiUrl = string.Format("ProductInput/QueryTimeSpanReport");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
                    var result = responMessage.Content.ReadAsStringAsync().Result;

                    var list = JsonConvert.DeserializeObject<ExportIntervalReportDataResult>(result);


                    var stream = new MemoryStream();
                    var fileName = PathHelper.SetGridExportExcelName(exportName);
                    var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
                    var stringHeads = new string[] { "制程", "Total 计划", "Total 实际", "Total 达成率" , "Total IE标准效率", "Total 部门有效人力" };

                    using (var excelPackage = new ExcelPackage(stream))
                    {
                        //set sheet name
                        var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName + "汇总报告");
                        //创建EXL头部
                        worksheet.Cells[1, 1, 1, 9].Merge = true;
                        worksheet.Cells[1, 1].Value = "客户:" + entity.Customer + " 专案:" + entity.Project + " 部件:" + entity.Part_Types + "日期: " + "从" + startDay.Year + "年"
                            + startDay.Month + "月" + startDay.Day + "日" + "到" + endDay.Year + "年"
                            + endDay.Month + "月" + endDay.Day + "日" + " 报表";
                        worksheet.Cells[2, 1, 2, 9].Merge = true;
                        worksheet.Cells[2, 1].Value = "下载时间:" + DateTime.Now.ToLocalTime();
                        worksheet.Cells[3, 1, 3, 9].Merge = true;

                        //创建标题
                        for (int i = 0; i < stringHeads.Count(); i++)
                        {
                            worksheet.Cells[4, i + 1].Value = stringHeads[i];
                        }

                        //set cell value
                        for (int index = 0; index < list.Items.Count; index++)
                        {
                            var currentRecord = list.Items[index];
                            //seq

                            worksheet.Cells[index + 5, 1].Value = currentRecord.Process;
                            worksheet.Cells[index + 5, 2].Value = currentRecord.SumPlan;
                            worksheet.Cells[index + 5, 3].Value = currentRecord.SumGoodQty;
                            worksheet.Cells[index + 5, 4].Value = currentRecord.SumYieldRate;

                            worksheet.Cells[index + 5, 5].Value = currentRecord.SumIE_TargetEfficacy;
                            worksheet.Cells[index + 5, 6].Value = currentRecord.SumIE_DeptHuman;

                        }
                        worksheet.Cells.AutoFitColumns();
                        excelPackage.Save();
                    }

                    if (submitType == "AJAX")
                    {
                        return Content("SUCCESS");
                    }
                    else
                    {
                        return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
                    }
                // break;
                case "time":
                    startDay = DateTime.Parse(entity.Interval_Date_Start.ToString());
                    endDay = DateTime.Parse(entity.Interval_Date_End.ToString());
                    sheetName = startDay.Year + "年" + startDay.Month + "月" + startDay.Day + "日" + " 到 " + endDay.Year + "年" + endDay.Month + "月" + endDay.Day + "日";

                    //get Export datas
                    var apiUrl2 = string.Format("ProductInput/QueryTimeSpanReport");
                    HttpResponseMessage responMessage2 = APIHelper.APIPostAsync(entity, apiUrl2);
                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;

                    var list2 = JsonConvert.DeserializeObject<ExportTimeReport>(result2);


                    var stream2 = new MemoryStream();
                    var fileName2 = PathHelper.SetGridExportExcelName(exportName);
                    var filePath2 = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName2);
                    var stringHeads2 = new string[] { "序号", " 制程序号", "场地", "功能厂", "制程", "颜色", "负责主管", "目标良率", "总计划",
                        "总领料","总仓库领料","总良品" ,"总试制调机" ,"总入库" ,"总NG" ,"总最终良率" ,"达成率" ,"总WIP" ,"不可用WIP" ,"可用WIP" };
                    var Plant = 0;
                    if (CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count > 0)
                        Plant = CurrentUser.GetUserInfo.Plant_OrganizationUIDList[0];
                    if (Plant == 1)
                        stringHeads2 = new string[] { "序号", " 制程序号", "场地", "功能厂", "制程", "颜色", "负责主管", "目标良率", "总计划",
                        "总领料","总仓库领料","总良品" ,"总试制调机" ,"总入库" ,"总NG" ,"总最终良率" ,"达成率" ,"总WIP","IE标准效率","IE实际产出需求人力","部门有效人力","IE人力效率比"};

                    using (var excelPackage = new ExcelPackage(stream2))
                    {
                        //set sheet name
                        var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName + "汇总报告");
                        //创建EXL头部
                        worksheet.Cells[1, 1, 1, 9].Merge = true;
                        worksheet.Cells[1, 1].Value = "客户:" + entity.Customer + " 专案:" + entity.Project + " 部件:" + entity.Part_Types + "日期: " + "从" + startDay.Year + "年"
                            + startDay.Month + "月" + startDay.Day + "日" + "到" + endDay.Year + "年"
                            + endDay.Month + "月" + endDay.Day + "日" + " 报表";
                        worksheet.Cells[2, 1, 2, 9].Merge = true;
                        worksheet.Cells[2, 1].Value = "下载时间:" + DateTime.Now.ToLocalTime();
                        worksheet.Cells[3, 1, 3, 9].Merge = true;

                        //创建标题
                        for (int i = 0; i < stringHeads2.Count(); i++)
                        {
                            worksheet.Cells[4, i + 1].Value = stringHeads2[i];
                        }

                        //set cell value
                        for (int index = 0; index < list2.Items.Count; index++)
                        {
                            var currentRecord = list2.Items[index];
                            //seq

                            worksheet.Cells[index + 5, 1].Value = index + 1;
                            worksheet.Cells[index + 5, 2].Value = currentRecord.Process_Seq;
                            worksheet.Cells[index + 5, 3].Value = currentRecord.Place;
                            worksheet.Cells[index + 5, 4].Value = currentRecord.FunPlant;
                            worksheet.Cells[index + 5, 5].Value = currentRecord.Process;
                            worksheet.Cells[index + 5, 6].Value = currentRecord.Color;
                            worksheet.Cells[index + 5, 7].Value = currentRecord.DRI;
                            worksheet.Cells[index + 5, 8].Value = currentRecord.Target_Yield;
                            worksheet.Cells[index + 5, 9].Value = currentRecord.Product_Plan;
                            worksheet.Cells[index + 5, 10].Value = currentRecord.Picking_QTY;
                            worksheet.Cells[index + 5, 11].Value = currentRecord.WH_Picking_QTY;
                            worksheet.Cells[index + 5, 12].Value = currentRecord.Good_QTY;
                            worksheet.Cells[index + 5, 13].Value = currentRecord.Adjust_QTY;
                            worksheet.Cells[index + 5, 14].Value = currentRecord.WH_QTY;
                            worksheet.Cells[index + 5, 15].Value = currentRecord.NG_QTY;
                            worksheet.Cells[index + 5, 16].Value = currentRecord.All_Finally_Yield;
                            worksheet.Cells[index + 5, 17].Value = currentRecord.All_Finally_Achieving;
                            worksheet.Cells[index + 5, 18].Value = currentRecord.WIP_QTY;

                            worksheet.Cells[index + 5, 19].Value = currentRecord.IE_TargetEfficacy;
                            if (currentRecord.IE_TargetEfficacy != 0)
                            {
                                worksheet.Cells[index + 5, 20].Value = (currentRecord.WH_QTY + currentRecord.Good_QTY) / currentRecord.IE_TargetEfficacy;
                            }else
                            {
                                worksheet.Cells[index + 5, 20].Value = 0;
                            }
                            worksheet.Cells[index + 5, 21].Value = currentRecord.IE_DeptHuman;

                            if (currentRecord.IE_TargetEfficacy != 0)
                            {
                                if ((currentRecord.WH_QTY + currentRecord.Good_QTY) / currentRecord.IE_TargetEfficacy != 0)
                                {

                                    worksheet.Cells[index + 5, 22].Value = currentRecord.IE_DeptHuman / ((currentRecord.WH_QTY + currentRecord.Good_QTY) / currentRecord.IE_TargetEfficacy);

                                }
                                else
                                {
                                    worksheet.Cells[index + 5, 22].Value = 0;
                                }
                            }
                            else
                            {
                                worksheet.Cells[index + 5, 22].Value = 0;
                            }

                            if (Plant != 1)
                            {
                                worksheet.Cells[index + 5, 23].Value = currentRecord.NullWip_QTY;
                                worksheet.Cells[index + 5, 24].Value = currentRecord.OK_QTY;
                            }
                        }
                        worksheet.Cells.AutoFitColumns();
                        excelPackage.Save();
                    }
                    if (submitType == "AJAX")
                    {
                        return Content("SUCCESS");
                    }
                    else
                    {
                        return new FileContentResult(stream2.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName2) };
                    }
                    //break;
            }
            return Content("SUCCESS");
        }

        /// <summary>
        /// 获取指定日期，在为一年中为第几周
        /// </summary>
        /// <param name="dt">指定时间</param>
        /// <reutrn>返回第几周</reutrn>
        private static int GetWeekOfYear(DateTime dt)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }

        public Week GetWeekOfDay(DateTime currentTime)
        {
            Week week = new Week();
            var strDT = currentTime.DayOfWeek.ToString();
            switch (strDT)
            {
                case "Monday":
                    week.Monday = currentTime;
                    break;
                case "Tuesday":
                    week.Monday = currentTime.AddDays(-1);
                    break;
                case "Wednesday":
                    week.Monday = currentTime.AddDays(-2);
                    break;
                case "Thursday":
                    week.Monday = currentTime.AddDays(-3);
                    break;
                case "Friday":
                    week.Monday = currentTime.AddDays(-4);
                    break;
                case "Saturday":
                    week.Monday = currentTime.AddDays(-5);
                    break;
                case "Sunday":
                    week.Monday = currentTime.AddDays(-6);
                    break;
            }
            week.Tuesday = week.Monday.AddDays(1);
            week.Wednesday = week.Monday.AddDays(2);
            week.Thursday = week.Monday.AddDays(3);
            week.Friday = week.Monday.AddDays(4);
            week.Saturday = week.Monday.AddDays(5);
            week.Sunday = week.Monday.AddDays(6);
            return week;
        }


        public ActionResult DoExportWeeklyData(string jsonexportlist, string submitType, string exportName)
        {

            var entity = JsonConvert.DeserializeObject<ReportDataSearch>(jsonexportlist);
            DateTime referenceDay = DateTime.Parse(entity.Week_Date_Start.ToString());
            int week = GetWeekOfYear(referenceDay);
            //get Export datas
            var apiUrl = string.Format("ProductInput/QueryWeekReport");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var list = JsonConvert.DeserializeObject<ExportWeeklyReportDataResult>(result);

            //var dayWeek = referenceDay.DayOfWeek;
            //DateTime monday = DateTime.Parse(entity.Week_Date_Start.ToString());
            //DateTime Tuesday = monday.AddDays(1);
            //DateTime Wednesday = monday.AddDays(2);
            //DateTime Thursday = monday.AddDays(3);
            //DateTime Friday = monday.AddDays(4);
            //DateTime Saterday = monday.AddDays(5);
            //DateTime Sunday = monday.AddDays(6);
            var dayWeek = GetWeekOfDay(entity.Week_Date_Start.Value);
            DateTime monday = dayWeek.Monday;
            DateTime Tuesday = dayWeek.Tuesday;
            DateTime Wednesday = dayWeek.Wednesday;
            DateTime Thursday = dayWeek.Thursday;
            DateTime Friday = dayWeek.Friday;
            DateTime Saterday = dayWeek.Saturday;
            DateTime Sunday = dayWeek.Sunday;
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName(exportName);
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "制程", "Total 计划", "Total 实际", "Total 达成率",monday.ToShortDateString()+"计划", monday.ToShortDateString()+"实际", monday.ToShortDateString()+"达成率", monday.ToShortDateString()+"IE标准效率",
                monday.ToShortDateString()+"部门有效人力",
               Tuesday.ToShortDateString()+ "计划",  Tuesday.ToShortDateString()+"实际", Tuesday.ToShortDateString()+ "达成率",
                Tuesday.ToShortDateString()+"IE标准效率",
                Tuesday.ToShortDateString()+"部门有效人力",

                 Wednesday.ToShortDateString()+"计划",  Wednesday.ToShortDateString()+"实际",  Wednesday.ToShortDateString()+"达成率",
                  Wednesday.ToShortDateString()+"IE标准效率",
                Wednesday.ToShortDateString()+"部门有效人力",

                     Thursday.ToShortDateString()+"计划",  Thursday.ToShortDateString()+"实际",  Thursday.ToShortDateString()+"达成率",
                      Thursday.ToShortDateString()+"IE标准效率",
                Thursday.ToShortDateString()+"部门有效人力",

                       Friday.ToShortDateString()+"计划",  Friday.ToShortDateString()+"实际",  Friday.ToShortDateString()+"达成率",
                        Friday.ToShortDateString()+"IE标准效率",
                Friday.ToShortDateString()+"部门有效人力",

                        Saterday.ToShortDateString()+ "计划",   Saterday.ToShortDateString()+"实际",   Saterday.ToShortDateString()+"达成率",
                         Saterday.ToShortDateString()+"IE标准效率",
                Saterday.ToShortDateString()+"部门有效人力",

                           Sunday.ToShortDateString()+ "计划",    Sunday.ToShortDateString()+"实际",    Sunday.ToShortDateString()+"达成率",
             Sunday.ToShortDateString()+"IE标准效率",
                Sunday.ToShortDateString()+"部门有效人力",};


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add(referenceDay.Year + "年" + "第" + week + "周报");

                //创建EXL头部
                worksheet.Cells[1, 1, 1, 9].Merge = true;
                worksheet.Cells[1, 1].Value = "客户:" + entity.Customer + " 专案:" + entity.Project + " 部件:" + entity.Part_Types + "周报日期:" + "从： " + monday.ToShortDateString() + " 到： " + Sunday.ToShortDateString() + " 报表";
                worksheet.Cells[2, 1, 2, 9].Merge = true;
                worksheet.Cells[2, 1].Value = "下载时间:" + DateTime.Now.ToLocalTime();
                worksheet.Cells[3, 1, 3, 9].Merge = true;

                //创建标题
                for (int i = 0; i < stringHeads.Count(); i++)
                {
                    worksheet.Cells[4, i + 1].Value = stringHeads[i];
                }

                //set cell value
                for (int index = 0; index < list.Items.Count; index++)
                {
                    var currentRecord = list.Items[index];
                    //seq

                    worksheet.Cells[index + 5, 1].Value = currentRecord.Process;
                    worksheet.Cells[index + 5, 2].Value = currentRecord.SumPlan;
                    worksheet.Cells[index + 5, 3].Value = currentRecord.SumGoodQty;
                    worksheet.Cells[index + 5, 4].Value = currentRecord.SumYieldRate;
                    worksheet.Cells[index + 5, 5].Value = currentRecord.MondayPlan;
                    worksheet.Cells[index + 5, 6].Value = currentRecord.MondayGoodQty;
                    worksheet.Cells[index + 5, 7].Value = currentRecord.MondayYieldRate;

                    worksheet.Cells[index + 5, 8].Value = currentRecord.MondayIE_TargetEfficacy;
                    worksheet.Cells[index + 5, 9].Value = currentRecord.MondayIE_DeptHuman;


                    worksheet.Cells[index + 5, 10].Value = currentRecord.TuesdayPlan;
                    worksheet.Cells[index + 5, 11].Value = currentRecord.TuesdayGoodQty;
                    worksheet.Cells[index + 5, 12].Value = currentRecord.TuesdayYieldRate;

                    worksheet.Cells[index + 5, 13].Value = currentRecord.TuesdayIE_TargetEfficacy;
                    worksheet.Cells[index + 5, 14].Value = currentRecord.TuesdayIE_DeptHuman;

                    worksheet.Cells[index + 5, 15].Value = currentRecord.WednesdayPlan;
                    worksheet.Cells[index + 5, 16].Value = currentRecord.WednesdayGoodQty;
                    worksheet.Cells[index + 5, 17].Value = currentRecord.WednesdayYieldRate;

                    worksheet.Cells[index + 5, 18].Value = currentRecord.WednesdayIE_TargetEfficacy;
                    worksheet.Cells[index + 5, 19].Value = currentRecord.WednesdayIE_DeptHuman;

                    worksheet.Cells[index + 5, 20].Value = currentRecord.ThursdayPlan;
                    worksheet.Cells[index + 5, 21].Value = currentRecord.ThursdayGoodQty;
                    worksheet.Cells[index + 5, 22].Value = currentRecord.ThursdayYieldRate;

                    worksheet.Cells[index + 5, 23].Value = currentRecord.ThursdayIE_TargetEfficacy;
                    worksheet.Cells[index + 5, 24].Value = currentRecord.ThursdayIE_DeptHuman;

                    worksheet.Cells[index + 5, 25].Value = currentRecord.FridayPlan;
                    worksheet.Cells[index + 5, 26].Value = currentRecord.FridayGoodQty;
                    worksheet.Cells[index + 5, 27].Value = currentRecord.FridayYieldRate;

                    worksheet.Cells[index + 5, 28].Value = currentRecord.FridayIE_TargetEfficacy;
                    worksheet.Cells[index + 5, 29].Value = currentRecord.FridayIE_DeptHuman;


                    worksheet.Cells[index + 5, 30].Value = currentRecord.SaterdayPlan;
                    worksheet.Cells[index + 5, 31].Value = currentRecord.SaterdayGoodQty;
                    worksheet.Cells[index + 5, 32].Value = currentRecord.SaterdayYieldRate;

                    worksheet.Cells[index + 5, 33].Value = currentRecord.SaterdayIE_TargetEfficacy;
                    worksheet.Cells[index + 5, 34].Value = currentRecord.SaterdayIE_DeptHuman;

                    worksheet.Cells[index + 5, 35].Value = currentRecord.SundayPlan;
                    worksheet.Cells[index + 5, 36].Value = currentRecord.SundayGoodQty;
                    worksheet.Cells[index + 5, 37].Value = currentRecord.SundayYieldRate;

                    worksheet.Cells[index + 5, 38].Value = currentRecord.SundayIE_TargetEfficacy;
                    worksheet.Cells[index + 5, 39].Value = currentRecord.SundayIE_DeptHuman;

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }


            if (submitType == "AJAX")
            {
                return Content("SUCCESS");
            }
            else
            {
                return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }

        public ActionResult DoExportProductReportNew(string jsonExportList, string submitType, string exportName, string IsColour)
        {
            //创建EXL文档
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PP检核资料数据");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new List<string> { "序号", "制程序号", "场地", "功能厂", "制程", "颜色", "负责主管", "目标良率", "总计划" };
            var stringSum = new List<string> { "汇总计划", "总领料", "总仓库领料", "总良品", "总调制试机", "总入库", "总NG", "总达成率", "总良率" };
            var stringAll = new List<string> { "滚动计划", "总领料", "总仓库领料", "总良品", "总调制试机", "总入库", "总NG", "滚动达成率", "最终良率" };
            var stringSubs = new List<string> { "计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "达成率", "未达成原因", "良率", "总WIP", "不可用WIP", "可用WIP" };
            // int columnIndex = 0;
            using (var excelPackage = new ExcelPackage(stream))
            {
                var entity = JsonConvert.DeserializeObject<ExportPPCheckDataNew>(jsonExportList);


                //定义变量
                var fail_NoData = "";
                //获取当前时段及日期
                var apiUrl = string.Format("EventReportManager/GetIntervalInfoAPI?opType={0}", entity.OP);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var intervalResult = responMessage.Content.ReadAsStringAsync().Result;
                var intervalList = JsonConvert.DeserializeObject<IntervalEnum>(intervalResult);
                var nowDate = intervalList.NowDate;
                var nowInterval = intervalList.Time_Interval;

                //set sheet name

                var sheetName = entity.Reference_Date.ToShortDateString().ToString();
                var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                var finallyList = entity.TabList.Count();
                if (!entity.TabList.Any())
                {
                    return Content("导出EXL数据前，请先查询数据！");
                }

                var Plant = 0;
                if (CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count > 0)
                    Plant = CurrentUser.GetUserInfo.Plant_OrganizationUIDList[0];
                if (Plant == 1)
                    stringSubs = new List<string> { "计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "达成率", "未达成原因", "良率", "总WIP" };
                foreach (var item in entity.TabList)
                {
                    NewProductReportSumSearch searchModel = new NewProductReportSumSearch();
                    searchModel.Reference_Date = entity.Reference_Date;
                    searchModel.Tab_Select_Text = entity.Time_InterVal;
                    searchModel.Flowchart_Master_UID = entity.Flowchart_Master_UID;
                    searchModel.Color = "ALL";
                    searchModel.OP = entity.OP;
                    searchModel.IsColour = 0;
                    searchModel.FunPlant = "ALL";
                    searchModel.input_day_verion = entity.input_day_verion;
                    var tv = entity.TabList.FirstOrDefault().Time_InterVal;
                    if (tv == "ALL")
                    {
                        searchModel.Tab_Select_Text = "04:00-06:00";
                    }
                    else if (tv == "Daily_Sum")
                    {
                        searchModel.Tab_Select_Text = "16:00-18:00";
                    }

                    else
                        searchModel.Tab_Select_Text = entity.TabList.FirstOrDefault().Time_InterVal;


                    var page = new Page();
                    page.PageSize = 10;
                    var apiUrl1 = "";
                    apiUrl1 = string.Format("EventReportManager/QueryReportDatasIntervalAPI");
                    HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(searchModel, page, apiUrl1);
                    var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<ExportReportDataResult>(result1);
                    if (!list.Items.Any())
                    {
                        fail_NoData = "时段：" + item.Time_InterVal + "没有数据，请重新选择查询条件！";
                        return Content(fail_NoData);
                    }
                    //创建EXL头部
                    worksheet.Cells[1, 1, 1, 9].Merge = true;
                    worksheet.Cells[1, 1].Value = "客户:" + entity.Customer + " 专案:" + entity.Project + " 部件:" + entity.Part_Types + " 日期:"
                        + sheetName + " 2H战情报表";
                    worksheet.Cells[2, 1, 2, 9].Merge = true;
                    worksheet.Cells[2, 1].Value = "Update:" + sheetName;
                    worksheet.Cells[3, 1, 3, 9].Merge = true;
                    #region Phase1 计算Heads内容
                    //创建标题
                    for (int i = 0; i < stringHeads.Count(); i++)
                    {
                        worksheet.Cells[4, i + 1].Value = stringHeads[i];
                    }
                    //添加值
                    for (int index = 0; index < list.Items.Count; index++)
                    {
                        var currentRecord = list.Items[index];
                        //seq                       
                        worksheet.Cells[index + 5, 1].Value = index + 1;
                        worksheet.Cells[index + 5, 2].Value = currentRecord.Process_Seq;
                        worksheet.Cells[index + 5, 3].Value = currentRecord.Place;
                        worksheet.Cells[index + 5, 4].Value = currentRecord.FunPlant;
                        worksheet.Cells[index + 5, 5].Value = currentRecord.Process;
                        worksheet.Cells[index + 5, 6].Value = currentRecord.Color;
                        worksheet.Cells[index + 5, 7].Value = currentRecord.DRI;
                        worksheet.Cells[index + 5, 8].Value = currentRecord.Target_Yield + "%";
                        worksheet.Cells[index + 5, 9].Value = currentRecord.All_Product_Plan;
                    }
                    #endregion
                    #region Phase2 如果查询条件中有白班/夜班小计，则不计算Subs,但包含WIP

                    if (item.Time_InterVal == "白班小计" || item.Time_InterVal == "夜班小计" || item.Time_InterVal == "全天")
                    {
                        stringHeads.AddRange(stringSum);
                        stringHeads.Add("不可用WIP");
                        stringHeads.Add("可用WIP");
                        stringHeads.Add("WIP");
                        stringHeads.Add("合理WIP");
                        var stringIndex = stringHeads.Count() - stringSum.Count() - 4;
                        //创建时段标题
                        worksheet.Cells[3, stringIndex + 1, 3, stringHeads.Count()].Merge = true;
                        worksheet.Cells[3, stringIndex + 1].Value = "时段：" + item.Time_InterVal;
                        //创建标题

                        for (int i = stringIndex; i < stringHeads.Count(); i++)
                        {
                            worksheet.Cells[4, i + 1].Value = stringHeads[i];
                        }
                        //填充数据
                        for (int index = 0; index < list.Items.Count; index++)
                        {
                            var currentRecord = list.Items[index];
                            worksheet.Cells[index + 5, stringIndex + 1].Value = currentRecord.All_Product_Plan_Sum;
                            worksheet.Cells[index + 5, stringIndex + 2].Value = currentRecord.All_Picking_QTY;
                            worksheet.Cells[index + 5, stringIndex + 3].Value = currentRecord.All_WH_Picking_QTY;
                            worksheet.Cells[index + 5, stringIndex + 4].Value = currentRecord.All_Good_QTY;
                            worksheet.Cells[index + 5, stringIndex + 5].Value = currentRecord.All_Adjust_QTY;
                            worksheet.Cells[index + 5, stringIndex + 6].Value = currentRecord.All_WH_QTY;
                            worksheet.Cells[index + 5, stringIndex + 7].Value = currentRecord.All_NG_QTY;
                            worksheet.Cells[index + 5, stringIndex + 8].Value = currentRecord.All_Rolling_Yield_Rate + "%";

                            worksheet.Cells[index + 5, stringIndex + 9].Value = currentRecord.All_Finally_Field + "%";
                            if (Plant != 1)
                            {
                                worksheet.Cells[index + 5, stringIndex + 10].Value = currentRecord.NullWIP_QTY;
                                worksheet.Cells[index + 5, stringIndex + 11].Value = currentRecord.OKWIP_QTY;

                            }


                            worksheet.Cells[index + 5, stringIndex + 12].Value = currentRecord.WIP_QTY;
                            worksheet.Cells[index + 5, stringIndex + 13].Value = currentRecord.Proper_WIP;


                        }
                    }
                    #endregion
                    else
                    {
                        #region Phase3 如果查询最后1时段的数据,则需要将ALL的数据放在前面，如果不是最后1个时段的数据，则只计算Subs
                        if (item == entity.TabList[finallyList - 1])
                        {
                            stringHeads.AddRange(stringAll);
                            stringHeads.AddRange(stringSubs);
                            stringHeads.Add("合理WIP");
                            //创建时段标题
                            var stringIndex = stringHeads.Count() - stringAll.Count() - stringSubs.Count() - 1;
                            worksheet.Cells[3, stringIndex + 1, 3, stringHeads.Count()].Merge = true;
                            worksheet.Cells[3, stringIndex + 1].Value = "时段：" + item.Time_InterVal;
                            //创建标题
                            for (int i = stringIndex; i < stringHeads.Count(); i++)
                            {
                                worksheet.Cells[4, i + 1].Value = stringHeads[i];
                            }
                            //填充数据
                            for (int index = 0; index < list.Items.Count; index++)
                            {
                                var currentRecord = list.Items[index];
                                worksheet.Cells[index + 5, stringIndex + 1].Value = currentRecord.All_Product_Plan_Sum;
                                worksheet.Cells[index + 5, stringIndex + 2].Value = currentRecord.All_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 3].Value = currentRecord.All_WH_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 4].Value = currentRecord.All_Good_QTY;
                                worksheet.Cells[index + 5, stringIndex + 5].Value = currentRecord.All_Adjust_QTY;
                                worksheet.Cells[index + 5, stringIndex + 6].Value = currentRecord.All_WH_QTY;
                                worksheet.Cells[index + 5, stringIndex + 7].Value = currentRecord.All_NG_QTY;
                                worksheet.Cells[index + 5, stringIndex + 8].Value = currentRecord.All_Rolling_Yield_Rate + "%";
                                worksheet.Cells[index + 5, stringIndex + 9].Value = currentRecord.All_Finally_Field + "%";

                                worksheet.Cells[index + 5, stringIndex + 10].Value = currentRecord.Product_Plan;
                                worksheet.Cells[index + 5, stringIndex + 11].Value = currentRecord.Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 12].Value = currentRecord.WH_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 13].Value = currentRecord.Good_QTY;
                                worksheet.Cells[index + 5, stringIndex + 14].Value = currentRecord.Adjust_QTY;
                                worksheet.Cells[index + 5, stringIndex + 15].Value = currentRecord.WH_QTY;
                                worksheet.Cells[index + 5, stringIndex + 16].Value = currentRecord.NG_QTY;
                                worksheet.Cells[index + 5, stringIndex + 17].Value = currentRecord.Rolling_Yield_Rate + "%";
                                worksheet.Cells[index + 5, stringIndex + 18].Value = currentRecord.Unacommpolished_Reason;
                                worksheet.Cells[index + 5, stringIndex + 19].Value = currentRecord.Finally_Field + "%";
                                worksheet.Cells[index + 5, stringIndex + 20].Value = currentRecord.WIP_QTY;
                                if (Plant != 1)
                                {
                                    worksheet.Cells[index + 5, stringIndex + 21].Value = currentRecord.NullWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 22].Value = currentRecord.OKWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 23].Value = currentRecord.Proper_WIP;
                                }
                            }
                        }
                        else
                        {
                            stringHeads.AddRange(stringSubs);
                            stringHeads.Add("合理WIP");
                            //创建时段标题
                            var stringIndex = stringHeads.Count() - stringSubs.Count() - 1;
                            worksheet.Cells[3, stringIndex + 1, 3, stringHeads.Count()].Merge = true;
                            worksheet.Cells[3, stringIndex + 1].Value = "时段：" + item.Time_InterVal;
                            //创建标题
                            for (int i = stringIndex; i < stringHeads.Count(); i++)
                            {
                                worksheet.Cells[4, stringIndex + 1].Value = stringHeads[i];
                            }
                            //填充数据
                            for (int index = 0; index < list.Items.Count; index++)
                            {
                                var currentRecord = list.Items[index];
                                worksheet.Cells[index + 5, stringIndex + 1].Value = currentRecord.Product_Plan;
                                worksheet.Cells[index + 5, stringIndex + 2].Value = currentRecord.Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 3].Value = currentRecord.WH_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 4].Value = currentRecord.Good_QTY;
                                worksheet.Cells[index + 5, stringIndex + 5].Value = currentRecord.Adjust_QTY;
                                worksheet.Cells[index + 5, stringIndex + 6].Value = currentRecord.WH_QTY;
                                worksheet.Cells[index + 5, stringIndex + 7].Value = currentRecord.NG_QTY;
                                worksheet.Cells[index + 5, stringIndex + 8].Value = currentRecord.Rolling_Yield_Rate + "%";
                                worksheet.Cells[index + 5, stringIndex + 9].Value = currentRecord.Unacommpolished_Reason;
                                worksheet.Cells[index + 5, stringIndex + 10].Value = currentRecord.Finally_Field + "%";
                                worksheet.Cells[index + 5, stringIndex + 11].Value = currentRecord.WIP_QTY;
                                if (Plant != 1)
                                {
                                    worksheet.Cells[index + 5, stringIndex + 12].Value = currentRecord.NullWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 13].Value = currentRecord.OKWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 14].Value = currentRecord.Proper_WIP;
                                }
                            }
                        }
                        #endregion
                    }
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            if (submitType == "AJAX")
            {
                return Content("SUCCESS");
            }
            else
            {
                return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }
        public ActionResult DoExportProductReport(string jsonExportList, string submitType, string exportName, string IsColour)
        {
            //创建EXL文档
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PP检核资料数据");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new List<string> { "序号", "制程序号", "场地", "功能厂", "制程", "颜色", "负责主管", "目标良率", "总计划", "IE标准效率", "IE实际产出需求人力", "部门有效人力", "IE人力效率比" };
            var stringSum = new List<string> { "汇总计划", "总领料", "总仓库领料", "总良品", "总调制试机", "总入库", "总NG", "总达成率", "总良率" };
            var stringAll = new List<string> { "滚动计划", "总领料", "总仓库领料", "总良品", "总调制试机", "总入库", "总NG", "滚动达成率", "最终良率" };
            var stringSubs = new List<string> { "计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "达成率", "未达成原因", "良率", "总WIP", "不可用WIP", "可用WIP" };
            // int columnIndex = 0;
            using (var excelPackage = new ExcelPackage(stream))
            {
                var entity = JsonConvert.DeserializeObject<ExportPPCheckData>(jsonExportList);


                //定义变量
                var fail_NoData = "";
                //获取当前时段及日期
                var apiUrl = string.Format("EventReportManager/GetIntervalInfoAPI?opType={0}", entity.OP);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var intervalResult = responMessage.Content.ReadAsStringAsync().Result;
                var intervalList = JsonConvert.DeserializeObject<IntervalEnum>(intervalResult);
                var nowDate = intervalList.NowDate;
                var nowInterval = intervalList.Time_Interval;

                //set sheet name

                var sheetName = entity.Reference_Date.ToShortDateString().ToString();
                var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                var finallyList = entity.TabList.Count();
                if (!entity.TabList.Any())
                {
                    return Content("导出EXL数据前，请先查询数据！");
                }

                var Plant = 0;
                if (CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count > 0)
                    Plant = CurrentUser.GetUserInfo.Plant_OrganizationUIDList[0];
                if (Plant == 1)
                    stringSubs = new List<string> { "计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "达成率", "未达成原因", "良率", "总WIP" };
                foreach (var item in entity.TabList)
                {
                    //获取到数据源
                    var QueryData = new ReportDataSearch();
                    QueryData.Customer = entity.Customer;
                    //ie
                    QueryData.IE_TargetEfficacy = entity.IE_TargetEfficacy;
                    QueryData.IE_DeptHuman = entity.IE_DeptHuman;

                    QueryData.Color = entity.Color;
                    QueryData.Part_Types = entity.Part_Types;
                    QueryData.Interval_Time = entity.Time_InterVal;
                    QueryData.Project = entity.Project;
                    QueryData.Reference_Date = entity.Reference_Date;
                    QueryData.Tab_Select_Text = item.Time_InterVal;
                    QueryData.Product_Phase = entity.Product_Phase;
                    QueryData.input_day_verion = entity.input_day_verion;
                    QueryData.FunPlant = entity.FunPlant;
                    QueryData.IsColour = int.Parse(IsColour);
                    QueryData.OP = entity.OP;
                    var page = new Page();
                    page.PageSize = 10;
                    var apiUrl1 = "";
                    apiUrl1 = string.Format("EventReportManager/QueryReportDatasAPI");
                    HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(QueryData, page, apiUrl1);
                    var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<ExportReportDataResult>(result1);
                    if (!list.Items.Any())
                    {
                        fail_NoData = "时段：" + item.Time_InterVal + "没有数据，请重新选择查询条件！";
                        return Content(fail_NoData);
                    }
                    //创建EXL头部
                    worksheet.Cells[1, 1, 1, 9].Merge = true;
                    worksheet.Cells[1, 1].Value = "客户:" + entity.Customer + " 专案:" + entity.Project + " 部件:" + entity.Part_Types + " 日期:"
                        + sheetName + " 2H战情报表";
                    worksheet.Cells[2, 1, 2, 9].Merge = true;
                    worksheet.Cells[2, 1].Value = "Update:" + sheetName;
                    worksheet.Cells[3, 1, 3, 9].Merge = true;
                    #region Phase1 计算Heads内容
                    //创建标题
                    for (int i = 0; i < stringHeads.Count(); i++)
                    {
                        worksheet.Cells[4, i + 1].Value = stringHeads[i];
                    }
                    //添加值
                    for (int index = 0; index < list.Items.Count; index++)
                    {
                        var currentRecord = list.Items[index];
                        //seq                       
                        worksheet.Cells[index + 5, 1].Value = index + 1;
                        worksheet.Cells[index + 5, 2].Value = currentRecord.Process_Seq;
                        worksheet.Cells[index + 5, 3].Value = currentRecord.Place;
                        worksheet.Cells[index + 5, 4].Value = currentRecord.FunPlant;
                        worksheet.Cells[index + 5, 5].Value = currentRecord.Process;
                        worksheet.Cells[index + 5, 6].Value = currentRecord.Color;
                        worksheet.Cells[index + 5, 7].Value = currentRecord.DRI;
                        worksheet.Cells[index + 5, 8].Value = currentRecord.Target_Yield + "%";
                        worksheet.Cells[index + 5, 9].Value = currentRecord.All_Product_Plan;
                        //IE
                        worksheet.Cells[index + 5, 10].Value = currentRecord.IE_TargetEfficacy;
                        if (currentRecord.IE_TargetEfficacy != 0)
                        {
                            worksheet.Cells[index + 5, 11].Value = (currentRecord.Good_QTY + currentRecord.WH_QTY) / currentRecord.IE_TargetEfficacy;
                        }else
                        {
                            worksheet.Cells[index + 5, 11].Value = 0;
                        }
                        worksheet.Cells[index + 5, 12].Value = currentRecord.IE_DeptHuman;
                        if (currentRecord.IE_TargetEfficacy != 0)
                        {
                            if ((currentRecord.Good_QTY + currentRecord.WH_QTY) / currentRecord.IE_TargetEfficacy != 0)
                            {
                                worksheet.Cells[index + 5, 13].Value = currentRecord.IE_DeptHuman / ((currentRecord.Good_QTY + currentRecord.WH_QTY) / currentRecord.IE_TargetEfficacy);
                            }
                            else
                            {
                                worksheet.Cells[index + 5, 13].Value = 0;
                            }
                        }
                        else
                        {
                            worksheet.Cells[index + 5, 13].Value = 0;
                        }
                    }
                    #endregion
                    #region Phase2 如果查询条件中有白班/夜班小计，则不计算Subs,但包含WIP

                    if (item.Time_InterVal == "白班小计" || item.Time_InterVal == "夜班小计" || item.Time_InterVal == "全天")
                    {
                        stringHeads.AddRange(stringSum);
                        stringHeads.Add("不可用WIP");
                        stringHeads.Add("可用WIP");
                        stringHeads.Add("WIP");
                        stringHeads.Add("合理WIP");
                        var stringIndex = stringHeads.Count() - stringSum.Count() - 4;
                        //创建时段标题
                        worksheet.Cells[3, stringIndex + 1, 3, stringHeads.Count()].Merge = true;
                        worksheet.Cells[3, stringIndex + 1].Value = "时段：" + item.Time_InterVal;
                        //创建标题

                        for (int i = stringIndex; i < stringHeads.Count(); i++)
                        {
                            worksheet.Cells[4, i + 1].Value = stringHeads[i];
                        }
                        //填充数据
                        for (int index = 0; index < list.Items.Count; index++)
                        {
                            var currentRecord = list.Items[index];
                            worksheet.Cells[index + 5, stringIndex + 1].Value = currentRecord.All_Product_Plan_Sum;
                            worksheet.Cells[index + 5, stringIndex + 2].Value = currentRecord.All_Picking_QTY;
                            worksheet.Cells[index + 5, stringIndex + 3].Value = currentRecord.All_WH_Picking_QTY;
                            worksheet.Cells[index + 5, stringIndex + 4].Value = currentRecord.All_Good_QTY;
                            worksheet.Cells[index + 5, stringIndex + 5].Value = currentRecord.All_Adjust_QTY;
                            worksheet.Cells[index + 5, stringIndex + 6].Value = currentRecord.All_WH_QTY;
                            worksheet.Cells[index + 5, stringIndex + 7].Value = currentRecord.All_NG_QTY;
                            worksheet.Cells[index + 5, stringIndex + 8].Value = currentRecord.All_Rolling_Yield_Rate + "%";

                            worksheet.Cells[index + 5, stringIndex + 9].Value = currentRecord.All_Finally_Field + "%";

                            //IE
                            worksheet.Cells[index + 5, 10].Value = currentRecord.IE_TargetEfficacy;
                            if (currentRecord.IE_TargetEfficacy != 0)
                            {
                                worksheet.Cells[index + 5, 11].Value = (currentRecord.All_Good_QTY + currentRecord.All_WH_QTY) / currentRecord.IE_TargetEfficacy;
                            }
                            else
                            {
                                worksheet.Cells[index + 5, 11].Value = 0;
                            }
                            worksheet.Cells[index + 5, 12].Value = currentRecord.IE_DeptHuman;
                            if (currentRecord.IE_TargetEfficacy != 0)
                            {
                                if ((currentRecord.All_Good_QTY + currentRecord.All_WH_QTY) / currentRecord.IE_TargetEfficacy != 0)
                                {
                                    worksheet.Cells[index + 5, 13].Value = currentRecord.IE_DeptHuman / ((currentRecord.All_Good_QTY + currentRecord.All_WH_QTY) / currentRecord.IE_TargetEfficacy);
                                }
                                else
                                {
                                    worksheet.Cells[index + 5, 13].Value = 0;
                                }
                            }
                            else
                            {
                                worksheet.Cells[index + 5, 13].Value = 0;
                            }

                            if (Plant != 1)
                            {
                                worksheet.Cells[index + 5, stringIndex + 14].Value = currentRecord.NullWIP_QTY;
                                worksheet.Cells[index + 5, stringIndex + 11].Value = currentRecord.OKWIP_QTY;

                            }


                            worksheet.Cells[index + 5, stringIndex + 15].Value = currentRecord.WIP_QTY;
                            worksheet.Cells[index + 5, stringIndex + 16].Value = currentRecord.Proper_WIP;


                        }
                    }
                    #endregion
                    else
                    {
                        #region Phase3 如果查询最后1时段的数据,则需要将ALL的数据放在前面，如果不是最后1个时段的数据，则只计算Subs
                        if (item == entity.TabList[finallyList - 1])
                        {
                            stringHeads.AddRange(stringAll);
                            stringHeads.AddRange(stringSubs);
                            stringHeads.Add("合理WIP");
                            //创建时段标题
                            var stringIndex = stringHeads.Count() - stringAll.Count() - stringSubs.Count() - 1;
                            worksheet.Cells[3, stringIndex + 1, 3, stringHeads.Count()].Merge = true;
                            worksheet.Cells[3, stringIndex + 1].Value = "时段：" + item.Time_InterVal;
                            //创建标题
                            for (int i = stringIndex; i < stringHeads.Count(); i++)
                            {
                                worksheet.Cells[4, i + 1].Value = stringHeads[i];
                            }
                            //填充数据
                            for (int index = 0; index < list.Items.Count; index++)
                            {
                                var currentRecord = list.Items[index];
                                worksheet.Cells[index + 5, stringIndex + 1].Value = currentRecord.All_Product_Plan_Sum;
                                worksheet.Cells[index + 5, stringIndex + 2].Value = currentRecord.All_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 3].Value = currentRecord.All_WH_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 4].Value = currentRecord.All_Good_QTY;
                                worksheet.Cells[index + 5, stringIndex + 5].Value = currentRecord.All_Adjust_QTY;
                                worksheet.Cells[index + 5, stringIndex + 6].Value = currentRecord.All_WH_QTY;
                                worksheet.Cells[index + 5, stringIndex + 7].Value = currentRecord.All_NG_QTY;
                                worksheet.Cells[index + 5, stringIndex + 8].Value = currentRecord.All_Rolling_Yield_Rate + "%";
                                worksheet.Cells[index + 5, stringIndex + 9].Value = currentRecord.All_Finally_Field + "%";

                                worksheet.Cells[index + 5, stringIndex + 10].Value = currentRecord.Product_Plan;
                                worksheet.Cells[index + 5, stringIndex + 11].Value = currentRecord.Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 12].Value = currentRecord.WH_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 13].Value = currentRecord.Good_QTY;
                                worksheet.Cells[index + 5, stringIndex + 14].Value = currentRecord.Adjust_QTY;
                                worksheet.Cells[index + 5, stringIndex + 15].Value = currentRecord.WH_QTY;
                                worksheet.Cells[index + 5, stringIndex + 16].Value = currentRecord.NG_QTY;
                                worksheet.Cells[index + 5, stringIndex + 17].Value = currentRecord.Rolling_Yield_Rate + "%";
                                worksheet.Cells[index + 5, stringIndex + 18].Value = currentRecord.Unacommpolished_Reason;
                                worksheet.Cells[index + 5, stringIndex + 19].Value = currentRecord.Finally_Field + "%";
                                worksheet.Cells[index + 5, stringIndex + 20].Value = currentRecord.WIP_QTY;
                                if (Plant != 1)
                                {
                                    worksheet.Cells[index + 5, stringIndex + 21].Value = currentRecord.NullWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 22].Value = currentRecord.OKWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 23].Value = currentRecord.Proper_WIP;
                                }
                            }
                        }
                        else
                        {
                            stringHeads.AddRange(stringSubs);
                            stringHeads.Add("合理WIP");
                            //创建时段标题
                            var stringIndex = stringHeads.Count() - stringSubs.Count() - 1;
                            worksheet.Cells[3, stringIndex + 1, 3, stringHeads.Count()].Merge = true;
                            worksheet.Cells[3, stringIndex + 1].Value = "时段：" + item.Time_InterVal;
                            //创建标题
                            for (int i = stringIndex; i < stringHeads.Count(); i++)
                            {
                                worksheet.Cells[4, stringIndex + 1].Value = stringHeads[i];
                            }
                            //填充数据
                            for (int index = 0; index < list.Items.Count; index++)
                            {
                                var currentRecord = list.Items[index];
                                worksheet.Cells[index + 5, stringIndex + 1].Value = currentRecord.Product_Plan;
                                worksheet.Cells[index + 5, stringIndex + 2].Value = currentRecord.Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 3].Value = currentRecord.WH_Picking_QTY;
                                worksheet.Cells[index + 5, stringIndex + 4].Value = currentRecord.Good_QTY;
                                worksheet.Cells[index + 5, stringIndex + 5].Value = currentRecord.Adjust_QTY;
                                worksheet.Cells[index + 5, stringIndex + 6].Value = currentRecord.WH_QTY;
                                worksheet.Cells[index + 5, stringIndex + 7].Value = currentRecord.NG_QTY;
                                worksheet.Cells[index + 5, stringIndex + 8].Value = currentRecord.Rolling_Yield_Rate + "%";
                                worksheet.Cells[index + 5, stringIndex + 9].Value = currentRecord.Unacommpolished_Reason;
                                worksheet.Cells[index + 5, stringIndex + 10].Value = currentRecord.Finally_Field + "%";
                                worksheet.Cells[index + 5, stringIndex + 11].Value = currentRecord.WIP_QTY;
                                if (Plant != 1)
                                {
                                    worksheet.Cells[index + 5, stringIndex + 12].Value = currentRecord.NullWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 13].Value = currentRecord.OKWIP_QTY;
                                    worksheet.Cells[index + 5, stringIndex + 14].Value = currentRecord.Proper_WIP;
                                }
                            }
                        }
                        #endregion
                    }
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            if (submitType == "AJAX")
            {
                return Content("SUCCESS");
            }
            else
            {
                return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }
        public ActionResult DoExportOnePassReport(string jsonExportList, string submitType, string exportName)
        {
            var entity = JsonConvert.DeserializeObject<ExportOnePaseeData>(jsonExportList);
            //创建EXL文档
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName(entity.Project + "直通良率报表");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new List<string> { "Band", "PVD", "Assembly1", "Assembly2", "OQC", "汇总" };
            var stringSum = new List<string> { "汇总计划", "总领料", "总仓库领料", "总良品", "总调制试机", "总入库", "总NG", "总达成率", "总良率" };
            var stringAll = new List<string> { "滚动计划", "总领料", "总仓库领料", "总良品", "总调制试机", "总入库", "总NG", "滚动达成率", "最终良率" };
            var stringSubs = new List<string> { "计划", "领料", "仓库领料", "良品", "试制调机", "入库", "NG", "达成率", "良率", "WIP" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add(fileName);
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //创建EXL头部
                worksheet.Cells[1, 1, 1, 9].Merge = true;
                worksheet.Cells[1, 1].Value = " 专案:" + entity.Project;
                worksheet.Cells[2, 1, 2, 9].Merge = true;
                worksheet.Cells[2, 1].Value = "下载时间:" + DateTime.Now.ToLocalTime();
                worksheet.Cells[3, 1, 3, 9].Merge = true;
                // worksheet.Row(4).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); 
                //创建标题
                for (int i = 0; i < stringHeads.Count(); i++)
                {
                    worksheet.Cells[4, i + 1].Value = stringHeads[i];
                }

                //set cell value

                worksheet.Cells[5, 1].Value = entity.CNCPass;
                worksheet.Cells[5, 2].Value = entity.BiaoMianPass;
                worksheet.Cells[5, 3].Value = entity.YangJiPass;
                worksheet.Cells[5, 4].Value = entity.ZuZhuangPass;
                worksheet.Cells[5, 5].Value = entity.OQCPass;
                worksheet.Cells[5, 6].Value = entity.onePass;

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();


            }
            if (submitType == "AJAX")
            {
                return Content("SUCCESS");
            }
            else
            {
                return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }
        #endregion
        #region Product_Input And Product_Input_History Common Function-------Sidney 2015/12/20
        /// <summary>
        /// GetCustomerSource
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomerSource()
        {
            var userProjectUid = this.CurrentUser.DataPermissions.Project_UID;
            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            string oporg = ",";
            if (userinfo.Count != 0)
            {
                foreach (OrganiztionVM item in userinfo)
                {
                    if (item.OPType_OrganizationUID != 0)
                        oporg += item.OPType_OrganizationUID.ToString() + ",";
                }
            }
            var apiUrl = string.Format("EventReportManager/GetCustomerSourceAPI?userProjectUid={0}&&oporg={1}", userProjectUid, oporg);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetProjectSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public ActionResult GetProjectSource(string CustomerName)
        {
            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            List<int> oporg = new List<int>();
            if (userinfo.Count != 0)
            {
                for (int i = 0; i < userinfo.Count; i++)
                {
                    if ((int)userinfo[i].OPType_OrganizationUID != 0)
                        oporg.Add((int)userinfo[i].OPType_OrganizationUID);
                }
            }
            var project = new GetProjectModel
            {
                Customer = CustomerName,
                OpTypes = this.CurrentUser.GetUserInfo.OpTypes,
                Project_UID = this.CurrentUser.GetUserInfo.ProjectUIDList,
                orgs = oporg
            };
            var apiUrl = string.Format("EventReportManager/GetProjectSourceAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(project, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetSelctOP(string CustomerName, string ProjectName)
        {
            var apiUrl = string.Format("EventReportManager/GetSelctOPAPI?customer={0}&&project={1}", CustomerName, ProjectName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetProject()
        {
            var project = new GetProjectModel
            {

                OpTypes = this.CurrentUser.DataPermissions.Op_Types,
                Project_UID = this.CurrentUser.DataPermissions.Project_UID
            };
            var apiUrl = string.Format("EventReportManager/GetProjectSourceQAAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(project, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetProductPhaseSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public ActionResult GetProductPhaseSource(string CustomerName, string ProjectName)
        {
            var apiUrl = string.Format("EventReportManager/GetProductPhaseSourceAPI?customer={0}&&project={1}", CustomerName, ProjectName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetProductPhaseSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public ActionResult GetProjectPhaseSource(string Plant, string ProjectName)
        {
            var apiUrl = string.Format("EventReportManager/GetProjectPhaseSourceAPI?Plant={0}&&project={1}", Plant, ProjectName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// GetPartTypesSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <returns></returns>
        public ActionResult GetPartTypesSource(string CustomerName, string ProjectName, string ProductPhaseName)
        {
            var apiUrl = string.Format("EventReportManager/GetPartTypesSourceAPI?customer={0}&&project={1}&&productphase={2}", CustomerName, ProjectName, ProductPhaseName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SelectPart(string selectProjects, string opType, string location, string plant)
        {
            ViewBag.selectProjects = selectProjects;
            ViewBag.opType = opType;
            ViewBag.location = location;
            ViewBag.plant = plant;
            //获取所有功能厂
            var apiUrl = string.Format("Equipmentmaintenance/GetFunPlantsAPI/?selectProjects={0}&opType={1}", selectProjects, opType);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> funplants = js.Deserialize<List<string>>(result);
            ViewBag.funplants = funplants;
            //return View();
            return Content(result, "application/json"); ;
        }
        /// <summary>
        /// GetColorSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <param name="PartTypesName"></param>
        /// <returns></returns>
        public ActionResult GetColorSource(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName)
        {
            var apiUrl = string.Format("EventReportManager/GetColorSourceAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}", CustomerName, ProjectName, ProductPhaseName, PartTypesName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetAllColorByFM(string optype, string ProjectName, string ProductPhaseName, string PartTypesName)
        {
            var apiUrl = string.Format("EventReportManager/GetAllColorByFMAPI?optype={0}&&project={1}&&productphase={2}&&parttypes={3}", optype, ProjectName, ProductPhaseName, PartTypesName);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetDayVersion(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName, string Day)
        {
            var apiUrl = string.Format("EventReportManager/GetDayVersionAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&day={4}", CustomerName, ProjectName, ProductPhaseName, PartTypesName, Day);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        /// <summary>
        /// GetColorSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <param name="PartTypesName"></param>
        /// <returns></returns>
        public ActionResult GetFunPlant(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName)
        {
            int LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("EventReportManager/GetFunPlantAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&LanguageID={4}", CustomerName, ProjectName, ProductPhaseName, PartTypesName, LanguageID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// GetIntervalTime
        /// </summary>
        /// <param name="nowOrAllInterval">可传入两个参数：当前时段=PPCheckData，所有时段=DataReport </param>
        /// <returns></returns>
        public ActionResult GetIntervalTime(string nowOrAllInterval, string OP)
        {
            int LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("EventReportManager/GetIntervalTimeAPI?PageName={0}&&LanguageID={1}&&OP={2}", nowOrAllInterval, LanguageID, OP);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult CheckWIPNeedUpdate(string Op_Type)
        {
            if (this.CurrentUser.GetUserInfo.OpTypes.Count > 0)
            {
                Op_Type = this.CurrentUser.GetUserInfo.OpTypes[0];
            }

            var apiUrl = string.Format("EventReportManager/CheckWIPNeedUpdate?Op_Type={0}", Op_Type);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion
        #region Day Week Month Report Function------------------------Sidney 2016/01/28
        public ActionResult GetWeekVersionSource(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName, DateTime myDate)
        {
            DateTime beginTime; DateTime endTime;
            DayOfWeek weekDay = myDate.DayOfWeek;
            beginTime = myDate.AddDays(DayOfWeek.Monday - weekDay);
            endTime = myDate.AddDays(DayOfWeek.Sunday - weekDay + 7);
            var apiUrl = string.Format("EventReportManager/GetVersionSourceAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&beginTime={4}&&endTime={5}", CustomerName, ProjectName, ProductPhaseName, PartTypesName, beginTime, endTime);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetVersionSource(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName, DateTime beginTime, DateTime endTime)
        {

            var apiUrl = string.Format("EventReportManager/GetVersionSourceAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&beginTime={4}&&endTime={5}", CustomerName, ProjectName, ProductPhaseName, PartTypesName, beginTime, endTime);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetDailyVersion(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName, DateTime referenceDay)
        {

            var apiUrl = string.Format("EventReportManager/GetDailyVersionAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&referenceDay={4}", CustomerName, ProjectName, ProductPhaseName, PartTypesName, referenceDay);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetVersionBeginEndDate(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName, string Version, DateTime startDay, DateTime endDay)
        {
            int InteverVersion = 0;
            bool trys = int.TryParse(Version, out InteverVersion);
            var apiUrl = string.Format("EventReportManager/GetVersionBeginEndDateAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&version={4}&&startDay={5}&&endDay={6}", CustomerName, ProjectName, ProductPhaseName, PartTypesName, InteverVersion, startDay, endDay);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult GetDateTime(string mydate)
        {
            try
            {
                DateTime myDate = DateTime.Parse(mydate);

                string startDate = string.Empty;
                string endDate = string.Empty;

                DayOfWeek weekDay = myDate.DayOfWeek;

                if (weekDay.ToString() == "Sunday")
                {
                    endDate = myDate.AddDays(DayOfWeek.Sunday - weekDay).ToShortDateString();
                    startDate = myDate.AddDays(DayOfWeek.Sunday - weekDay - 6).ToShortDateString();

                }
                else
                {
                    startDate = myDate.AddDays(DayOfWeek.Monday - weekDay).ToShortDateString();
                    endDate = myDate.AddDays(DayOfWeek.Sunday - weekDay + 7).ToShortDateString();
                }


                string result = "从 " + startDate + " 到 " + endDate;
                return Content(result);
            }
            catch
            {
                return null;
            }

        }

        public ActionResult QueryTimeSpanReport(ReportDataSearch search, Page page)
        {
            var apiUrl = string.Format("ProductInput/QueryTimeSpanReport");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryWeekReport(ReportDataSearch search, Page page)
        {
            var apiUrl = string.Format("ProductInput/QueryWeekReport");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryWeekData(string searchStr)
        {
            ReportDataSearch search = JsonConvert.DeserializeObject<ReportDataSearch>(searchStr);
            var apiUrl = string.Format("ProductInput/QueryWeekReport");
            if (search.FunPlant == "关键制程")
            {
                search.FunPlant = null;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //获取日报图表数据
        public ActionResult QueryDailyData(string searchStr)
        {

            ReportDataSearch search = JsonConvert.DeserializeObject<ReportDataSearch>(searchStr);

            var apiUrl = string.Format("ProductInput/QueryDailyDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //获取月报图表数据
        public ActionResult QueryMonthlyData(string searchStr)
        {
            ReportDataSearch search = JsonConvert.DeserializeObject<ReportDataSearch>(searchStr);
            var apiUrl = string.Format("ProductInput/QueryMonthlyDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        //获取日良率图表数据
        public ActionResult QueryDailyYield(string searchStr)
        {
            ReportDataSearch search = JsonConvert.DeserializeObject<ReportDataSearch>(searchStr);
            var apiUrl = string.Format("ProductInput/QueryDailyYieldAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
        #region FirstReportDatas------------------------Sidney 2016/03/28
        public ActionResult FirstReportDatas(string search, Page page)
        {
            var apiUrl = string.Format("EventReportManager/FirstReportDatasAPI");
            ReportDataSearch searchModel = new ReportDataSearch();
            searchModel.Customer = "Apple";
            searchModel.Project = "BC41";
            searchModel.Product_Phase = "MP";
            searchModel.Part_Types = "BC41";
            searchModel.Color = "ALL";
            searchModel.Select_Type = "daily";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchModel, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
        public string CheckMatchFlag(PPCheckDataSearch searchModel)
        {
            //根据Project，PartTypes获取MasterUID和Version
            var flUrl = string.Format("FlowChart/GetMasterItemBySearchConditionAPI");
            HttpResponseMessage flResponMessage = APIHelper.APIPostAsync(searchModel, flUrl);
            var flUrlResult = flResponMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartMasterDTO>(flUrlResult);
            if (item != null)
            {
                searchModel.FlowChart_Master_UID = item.FlowChart_Master_UID;
                searchModel.FlowChart_Version = item.FlowChart_Version;
            }


            var apiUrl = string.Format("EventReportManager/CheckMatchFlagAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchModel, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }
        public ActionResult KeyProcessVertify(string ProjectName, string Part_Types)
        {

            var apiUrl = string.Format("EventReportManager/KeyProcessVertifyAPI?ProjectName={0}&&Part_Types={1}", ProjectName, Part_Types);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// GetColorSource
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProductPhaseName"></param>
        /// <param name="PartTypesName"></param>
        /// <returns></returns>
        public ActionResult GetFunPlantForChart(string CustomerName, string ProjectName, string ProductPhaseName, string PartTypesName)
        {
            int LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("EventReportManager/GetFunPlantForChartAPI?customer={0}&&project={1}&&productphase={2}&&parttypes={3}&&LanguageID={4}", CustomerName, ProjectName, ProductPhaseName, PartTypesName, LanguageID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetOpenProject()
        {
            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            List<int> oporg = new List<int>();
            if (userinfo.Count != 0)
            {
                for (int i = 0; i < userinfo.Count; i++)
                {
                    if ((int)userinfo[i].OPType_OrganizationUID != 0)
                        oporg.Add((int)userinfo[i].OPType_OrganizationUID);
                }
            }
            var project = new GetProjectModel
            {
                OpTypes = this.CurrentUser.GetUserInfo.OpTypes,
                Project_UID = this.CurrentUser.GetUserInfo.ProjectUIDList,
                orgs = oporg
            };
            var apiUrl = string.Format("EventReportManager/GetOpenProjectAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(project, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //public ActionResult GetIntervalInfo()
        //{
        //    var url = string.Format("EventReportManager/GetIntervalInfoAPI");
        //    HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
        //    var result = responMessage.Content.ReadAsStringAsync().Result;
        //    return Content(result, "application/json");
        //}
        public ActionResult GetIntervalInfo(string OPType)
        {
            var url = string.Format("EventReportManager/GetIntervalInfoAPI?opType={0}", OPType);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult GetProductInputLocation(int flowChartDetailUID, string color, string process, DateTime? date, string timeInterval, string optype)
        {
            var queryDetailApiUrl = string.Format("FlowChart/QueryFLDetailByIDAPI?id={0}", flowChartDetailUID);
            HttpResponseMessage detailResponMessage = APIHelper.APIGetAsync(queryDetailApiUrl);

            var detailResult = detailResponMessage.Content.ReadAsStringAsync().Result;
            var flowChartGet = JsonConvert.DeserializeObject<FlowChartDetailGet>(detailResult);
            var flowChartMasterUID = flowChartGet.FlowChartDetailDTO.FlowChart_Master_UID;

            var searchModel = new ProductInputLocationSearch()
            {
                FlowChart_Master_UID = flowChartMasterUID,
                Color = color,
                Product_Date = date,
                Process = process,
                Time_Interval = timeInterval,
                opType = optype
            };
            var url = string.Format("EventReportManager/GetProductInputLocationAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchModel, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 通过制程序号获取楼栋信息
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryInputByProSeq(string customer, string project, string Product_Phase, string part_types, string Process_Seq, string input_date, string input_time, string optype)
        {
            var searchModel = new PDByProSeqSearch()
            {
                customer = customer,
                project = project,
                Product_Phase = Product_Phase,
                part_types = part_types,
                Process_Seq = Process_Seq,
                input_date = input_date,
                Time_Interval = input_time,
                optype = optype
            };

            var url = string.Format("EventReportManager/GetPDInputLocationByProSeqAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchModel, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 导出楼栋详情
        /// </summary>
        public ActionResult ExportFloorDetail(string jsonSearchParam, string submitType, string exportName, string opType)
        {
            var searchParamModel = JsonConvert.DeserializeObject<ReportDataSearch>(jsonSearchParam);
            //创建EXL文档 
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("导出楼栋详情");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new List<string> { "序号", "制程序号", "功能厂", "制程", "颜色", "场地", "领料", "仓库领料", "良品", "NG", "试制调机", "入库", "WIP", "返修入", "返修出" };
            int columnIndex = 0;

            using (var excelPackage = new ExcelPackage(stream))
            {
                //定义变量
                var entity = searchParamModel;
                var sheetName = entity.Interval_Time;
                var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                if (entity == null)
                {
                    return Content("导出EXL数据前，请先查询数据！");
                }
                //var queryData = new ExportSearch
                //{
                //    Time_Interval = sheetName,
                //    Product_Date = DateTime.Now,
                //    Customer = entity.Customer,
                //    Project = entity.Project,
                //    Product_Phase = entity.Product_Phase,
                //    Part_Types = entity.Part_Types,
                //    Color = entity.Color
                //};

                var apiUrl = string.Format("EventReportManager/ExportFloorDetialDayReportAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParamModel, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<ExportPPCheck_Data>>(result);
                //创建EXL头部
                worksheet.Cells[1, 1, 1, 13].Merge = true;
                worksheet.Cells[1, 1].Value = "客户:" + entity.Customer + " 专案:" + entity.Project + " 部件:" + entity.Part_Types + " 日期:" + entity.Reference_Date + " 2H战情报表";
                worksheet.Cells[2, 1, 2, 13].Merge = true;
                worksheet.Cells[2, 1].Value = "Update:" + entity.Reference_Date;
                worksheet.Cells[3, 1, 3, 6].Merge = true;
                worksheet.Cells[3, 1].Value = "当日生产达成状况";
                worksheet.Cells[3, 7, 3, 13].Merge = true;
                worksheet.Cells[3, 7].Value = "时段：" + sheetName;
                //创建标题
                columnIndex = stringHeads.Count();
                for (int colIndex = 0; colIndex < columnIndex; colIndex++)
                {
                    worksheet.Cells[4, colIndex + 1].Value = stringHeads[colIndex];
                }

                //填充数据
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 5, 1].Value = index + 1;
                    worksheet.Cells[index + 5, 2].Value = currentRecord.Process_Seq;
                    worksheet.Cells[index + 5, 3].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 5, 4].Value = currentRecord.Process;
                    worksheet.Cells[index + 5, 5].Value = currentRecord.Color;
                    worksheet.Cells[index + 5, 6].Value = currentRecord.Place;
                    worksheet.Cells[index + 5, 7].Value = currentRecord.Picking_QTY;
                    worksheet.Cells[index + 5, 8].Value = currentRecord.WH_Picking_QTY;
                    worksheet.Cells[index + 5, 9].Value = currentRecord.Good_QTY;
                    worksheet.Cells[index + 5, 10].Value = currentRecord.NG_QTY;
                    worksheet.Cells[index + 5, 11].Value = currentRecord.Adjust_QTY;
                    worksheet.Cells[index + 5, 12].Value = currentRecord.WH_QTY;
                    worksheet.Cells[index + 5, 13].Value = currentRecord.WIP_QTY;

                    worksheet.Cells[index + 5, 14].Value = currentRecord.repairInputCount;
                    worksheet.Cells[index + 5, 15].Value = currentRecord.repairOutputCount;
                }

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            if (submitType == "AJAX")
            {
                return Content("SUCCESS");
            }
            else
            {
                return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }
    }
}