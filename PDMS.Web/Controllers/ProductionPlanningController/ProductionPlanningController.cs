using PDMS.Model;
using PDMS.Web.Business.ProductiongPlanning;
using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.IO;
using OfficeOpenXml.Style;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Common.Constants;
using System.Drawing;
using PDMS.Web.Business.Flowchart;
using PDMS.Model.EntityDTO;

namespace PDMS.Web.Controllers.ProductionPlanningController
{
    public class ProductionPlanningController : WebControllerBase
    {
        #region Flowchart ME List
        // GET: ProductionPlanning
        public ActionResult ProductionPlanningList()
        {
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
            ViewBag.PlantUID = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count();
            ViewBag.OPType_OrganizationUIDList = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.Count();

            return View();
        }

        public ActionResult QueryMEFlowCharts(FlowChartModelSearch search, Page page)
        {
            search.RoleList = CurrentUser.GetUserInfo.RoleList;
            //获取一级SITE UID信息
            search.PlantUIDList = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList;
            //获取二级OPTYPES UID信息
            search.OPType_OrganizationUIDList = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList;
            search.OpTypes = this.CurrentUser.GetUserInfo.OpTypes;
            //获取用户的专案访问权限
            search.ProjectUIDList = this.CurrentUser.GetUserInfo.ProjectUIDList;
            search.AccountId = this.CurrentUser.AccountUId;

            var apiUrl = string.Format("ProductionPlanning/QueryMEFlowChartsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #region Flowchart Excel新专案导入
        public string ImportExcel(HttpPostedFileBase uploadName, string FlowChart_Version_Comment, int FlowChart_Master_UID)
        {
            string errorInfo = string.Empty;
            bool isEdit = false;
            List<ProductionPlanningModelGet> productGetList = new List<ProductionPlanningModelGet>();
            List<FlowchartDetailMEEquipmentDTO> equipmentList = new List<FlowchartDetailMEEquipmentDTO>();
            List<FlowchartDetailMEEquipmentDTO> autoEquipmentList = new List<FlowchartDetailMEEquipmentDTO>();

            if (FlowChart_Master_UID > 0)
            {
                isEdit = true;
                errorInfo = new ProductiongPlanningImport().ImportCheck(uploadName, FlowChart_Master_UID, FlowChart_Version_Comment, isEdit, productGetList, equipmentList, autoEquipmentList);
            }
            else
            {
                errorInfo = new ProductiongPlanningImport().ImportCheck(uploadName, 0, FlowChart_Version_Comment, isEdit, productGetList, equipmentList, autoEquipmentList);
            }
            if (string.IsNullOrEmpty(errorInfo))
            {
                ProductionPlanningModelGetAPIModel apiModelItem = new ProductionPlanningModelGetAPIModel();
                apiModelItem.GetList = productGetList;
                apiModelItem.EquipDTOList = equipmentList;
                apiModelItem.AutoEquipDTOList = autoEquipmentList;
                string api = "ProductionPlanning/ProductionPlanningAPI?isEdit=" + isEdit + "&accountID=" + this.CurrentUser.AccountUId;
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(apiModelItem, api);
            }
            return errorInfo;
        }

        public string UpdateImportExcel(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment)
        {
            string errorInfo = string.Empty;
            List<ProductionPlanningModelGet> productGetList = new List<ProductionPlanningModelGet>();
            List<FlowchartDetailMEEquipmentDTO> equipmentList = new List<FlowchartDetailMEEquipmentDTO>();
            List<FlowchartDetailMEEquipmentDTO> autoEquipmentList = new List<FlowchartDetailMEEquipmentDTO>();
            errorInfo = new ProductiongPlanningImport().ImportCheck(uploadName, 0, FlowChart_Version_Comment, false, productGetList, equipmentList, autoEquipmentList);
            if (string.IsNullOrEmpty(errorInfo))
            {
                ProductionPlanningModelGetAPIModel apiModelItem = new ProductionPlanningModelGetAPIModel();
                apiModelItem.GetList = productGetList;
                apiModelItem.EquipDTOList = equipmentList;
                apiModelItem.AutoEquipDTOList = autoEquipmentList;
                string api = "ProductionPlanning/ProductionPlanningAPI?isEdit=" + false + "&accountID=" + this.CurrentUser.AccountUId;

                HttpResponseMessage responMessage = APIHelper.APIPostAsync(apiModelItem, api);

            }
            return errorInfo;

        }

        #endregion

        public string ChangePhase(int FlowChart_Master_UID)
        {
            var apiUrl = string.Format("ProductionPlanning/ChangePhaseAPI?FlowChart_Master_UID={0}",FlowChart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"","");
            return result;
        }

        public ActionResult ClosedFlowChart(int FlowChart_Master_UID, bool isClosed)
        {
            var apiUrl = string.Format("ProductionPlanning/ClosedFLAPI?FlowChart_Master_UID={0}&isClosed={1}", FlowChart_Master_UID, isClosed);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region Flowchart ME Detail

        public ActionResult FlowchartMEDetailList(int id, string Product_Phase, int Version)
        {
            ViewBag.ID = id;
            ViewBag.Product_Phase = Product_Phase;
            ViewBag.Version = Version;
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;
            //获取用户所属的三级权限，从而获得4级权限功能厂，不考虑跨厂区权限
            if (this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count() == 0 || this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.Count() == 0)
            {
                ViewBag.NoRole = "Yes";
                ViewBag.FunPlantList = new List<SystemFunctionPlantDTO>();
                return View();
            }
            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", id);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var ddlResult = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(ddlResult);
            ViewBag.FunPlantList = item.SystemFunctionPlantDTOList;
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;
            return View();
        }

        public ActionResult QueryFLMEDetailList(int id, int Version)
        {
            string apiUrl = string.Format("ProductionPlanning/QueryFLMEDetailListAPI?id={0}&Version={1}", id, Version);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(null, null, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<PagedListModel<FlowchartDetailMEDTO>>(result);
            return Content(result, "application/json");
        }


        public ActionResult EditFLDetailInfo(int id, string Product_Phase, int Version, FlowchartDetailMEDTO dto)
        {
            var apiUrl = string.Format("ProductionPlanning/SaveFLDetailInfoAPI?AccountID={0}", this.CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            if (result == "")
                return RedirectToAction("FlowchartMEDetailList", "ProductionPlanning", new { id = id, Product_Phase = Product_Phase, Version = Version });
            else
                return Content(result, "application/json");

        }

        #endregion

        #region 查看设备明细
        public ActionResult FlowchartMEquipmentList(int id, string Product_Phase, int Version, int CurrentTab)
        {
            ViewBag.ID = id;
            ViewBag.Product_Phase = Product_Phase;
            ViewBag.Version = Version;
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;
            //获取用户所属的三级权限，从而获得4级权限功能厂，不考虑跨厂区权限
            //if (this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count() == 0 || this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.Count() == 0)
            //{
            //    ViewBag.NoRole = "Yes";
            //    ViewBag.FunPlantList = new List<SystemFunctionPlantDTO>();
            //    return View();
            //}
            int opTypeUID = 0;
            int opType = 0;
            //获取用户所属的三级权限，从而获得4级权限功能厂，不考虑跨厂区权限
            if (this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count() != 0)
            {
                opTypeUID = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.First();
            }
            if (this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.Count() != 0)
            {
                opType = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.First();
            }
            
            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", id);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var ddlResult = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(ddlResult);
            ViewBag.FunPlantList = item.SystemFunctionPlantDTOList;
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;
            ViewBag.CurrentTab = CurrentTab;
            return View();
        }

        public ActionResult QueryFLMEEquipmentDetailList(int id, int Version, int Param)
        {
            string apiUrl = string.Format("ProductionPlanning/QueryFLMEEquipmentDetailAPI?id={0}&Version={1}&Param={2}", id, Version, Param);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(null, null, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<PagedListModel<EquipmentGet>>(result);
            return Content(result, "application/json");
        }

        public ActionResult EditFLEquipmentInfo(int id, string Product_Phase, int Version, int CurrentTab, FlowchartDetailMEEquipmentDTO dto)
        {
            var apiUrl = string.Format("ProductionPlanning/SaveFLEquipmentInfoAPI?AccountID={0}", this.CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            if (result == "")
                return RedirectToAction("FlowchartMEquipmentList", "ProductionPlanning", new { id = id, Product_Phase = Product_Phase, Version = Version, CurrentTab = CurrentTab });
            else
                return Content(result, "application/json");

        }

        #endregion


        #region NPI生产计划维护
        public ActionResult FlowchartMENPIList(int id, string Product_Phase, int Version)
        {
            ViewBag.ID = id;
            ViewBag.Version = Version;

            Dictionary<int, string> funPlantList = new Dictionary<int, string>();
            funPlantList.Add(0, "All");

            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", id);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var ddlResult = responddlMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartDetailGetByMasterInfo>(ddlResult);
            foreach (var dto in item.SystemFunctionPlantDTOList)
            {
                funPlantList.Add(dto.System_FunPlant_UID, dto.FunPlant);
            }
            ViewBag.FunPlantList = funPlantList;
            ViewBag.CustomerName = item.BU_D_Name;
            ViewBag.ProjectName = item.Project_Name;
            ViewBag.PartTypes = item.Part_Types;
            ViewBag.ProductPhase = item.Product_Phase;

            var Week = GetCurrentWeek(DateTime.Now);
            ViewBag.Monday = Week.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate);
            ViewBag.Sunday = Week.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate);

            return View();
        }

        public string CheckDownloadNPIExcel(int id, int Version)
        {
            return string.Empty;
        }

        public ActionResult FlowchartMENPIInfo(FlowchartMeNPI npiModel, Page page)
        {
            var apiUrl = string.Format("ProductionPlanning/DownloadNPIExcelAPI");
            HttpResponseMessage responddlMessageList = APIHelper.APIPostAsync(npiModel, page, apiUrl);
            var result = responddlMessageList.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
            
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

        public FileResult DownloadNPIExcel(int id, int Version)
        {
            string api = "FlowChart/QueryFlowChartAPI?FlowChart_Master_UID=" + id;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var item = JsonConvert.DeserializeObject<FlowChartGet>(result);
            var Week = GetCurrentWeek(DateTime.Now);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("NPI_生产计划维护");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetNPIHeadColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("NPI_生产计划维护");
                SetNPIExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                //foreach (var item in list)
                //{
                worksheet.Cells[iRow, 1].Value = item.SystemProjectDTO.Project_Name;
                worksheet.Cells[iRow, 2].Value = item.FlowChartMasterDTO.FlowChart_Version;
                worksheet.Cells[iRow, 3].Value = item.FlowChartMasterDTO.Product_Phase;
                    //if (item.Product_Date == null) //没有生产数据
                    //{
                    //    worksheet.Cells[iRow, 3].Value = DateTime.Now.ToString("yyyy-MM-dd");
                    //}
                    //else //已经存在生产数据
                    //{
                    //    worksheet.Cells[iRow, 3].Value = item.Product_Date;
                    //}
                    //if (item.Input != null)
                    //{
                    //    worksheet.Cells[iRow, 4].Value = item.Input;
                    //}
                    //worksheet.Cells[iRow, 6].Value = item.FlowChartMasterDTO.FlowChart_Master_UID;
                    iRow++;
                //}
                //Excel最后一行行号
                int maxRow = iRow - 1;
                //设置灰色背景
                var colorRange = string.Format("A2:C{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置Excel日期格式yyyy-MM-dd
                var dateColorRange = string.Format("D2:D{0}", maxRow);
                worksheet.Cells[dateColorRange].Style.Numberformat.Format = FormatConstants.DateTimeFormatStringByDate;
                //设置主键列隐藏
                //worksheet.Column(5).Hidden = true;
                //设置边框
                worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:E{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();

            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult SaveNPIInfo(ProductionSchedulNPIDTO dto, int id, string Product_Phase, int Version)
        {
            var apiUrl = string.Format("ProductionPlanning/SaveNPIInfoAPI?AccountID={0}", this.CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            if (result == "")
                return RedirectToAction("FlowchartMENPIList", "ProductionPlanning", new { id = id, Product_Phase = Product_Phase, Version = Version });
            else
                return Content(result, "application/json");
        }

        private void SetNPIExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];


            //设置列宽
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 17;
            worksheet.Column(3).Width = 17;
            worksheet.Column(4).Width = 17;
            worksheet.Column(5).Width = 17;


            worksheet.Cells["A1:E1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }

        public string ImportNPIExcel(HttpPostedFileBase upload_excel, int FlowChart_Master_UID, int FlowChart_Version)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null || worksheet.Name != "NPI_生产计划维护")
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                int currentRow = 2;
                var propertiesContent = FlowchartImportCommon.GetNPIHeadColumn();
                List<ProductionSchedulNPIDTO> ProductionSchedulNPIList = new List<ProductionSchedulNPIDTO>();
                //int FlowChart_Master_UID = Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "FlowChart_Master_UID")].Value);
                int Input = 0;
                DateTime? Product_Date = null;
                int nowRow = 2;
                int nowColumn = 4;

                //int[,] aa = new int[3,2] { { 1, 2 }, { 3, 4 }, { 5,6} };
                var aa = worksheet.SelectedRange[string.Format("A{0}:E{1}",nowRow,nowColumn)].Value;

                string testa = ((object[,])aa)[0, 0].ToString();
                int testb = Convert.ToInt32(((object[,])aa)[0, 1]);
                while (worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "专案名称")].Value != null)
                {
                    var datetime = worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "日期")].Text;
                    if (string.IsNullOrEmpty(datetime))
                    {
                        errorInfo = string.Format("第{0}行请输入日期", currentRow);
                        break;
                    }
                    else
                    {
                        Product_Date = Convert.ToDateTime(datetime);
                        //if (DateTime.Now.Date > Product_Date)
                        //{
                        //    errorInfo = string.Format("第{0}行日期不能小于现在的日期", currentRow);
                        //    break;
                        //}
                    }
                    int validInt = 0;
                    var inputNum = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "投入数")].Value);
                    var isInt = int.TryParse(inputNum, out validInt);
                    if (isInt)
                    {
                        Input = Convert.ToInt32(inputNum);
                        if (Input == 0)
                        {
                            errorInfo = string.Format("第{0}行投入数不能为0", currentRow);
                            break;
                        }
                    }
                    else
                    {
                        errorInfo = string.Format("第{0}行请输入投入数且必须为整数", currentRow);
                        break;
                    }

                    ProductionSchedulNPIDTO npiDTO = new ProductionSchedulNPIDTO();
                    npiDTO.FlowChart_Master_UID = FlowChart_Master_UID;
                    npiDTO.FlowChart_Version = FlowChart_Version;
                    npiDTO.Product_Date = Product_Date;
                    npiDTO.Input = Input;
                    npiDTO.Created_Date = DateTime.Now;
                    npiDTO.Created_UID = this.CurrentUser.AccountUId;
                    npiDTO.Modified_Date = DateTime.Now;
                    npiDTO.Modified_UID = this.CurrentUser.AccountUId;
                    ProductionSchedulNPIList.Add(npiDTO);

                    currentRow++;
                }

                if (string.IsNullOrEmpty(errorInfo))
                {
                    var json = JsonConvert.SerializeObject(ProductionSchedulNPIList);
                    //检查导入的Excel是否已经存在
                    string checkApi = string.Format("ProductionPlanning/CheckDownloadNPIExcelApi");
                    HttpResponseMessage chkMessage = APIHelper.APIPostAsync(json, checkApi);
                    var chkResult = chkMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");

                    if (string.IsNullOrEmpty(chkResult))
                    {
                        string api = string.Format("ProductionPlanning/ImportNPIExcelAPI");
                        HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
                        var result = responMessage.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        errorInfo = chkResult;
                    }

                }
                else
                {
                    return errorInfo;
                }


                return errorInfo;
            }
        }

        public string DeleteInfoByUIDList(string json)
        {
            var apiUrl = string.Format("ProductionPlanning/DeleteInfoByUIDListAPI?json={0}", json);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }
        #endregion



        #region -----IE

        #region IE Flowchart列表界面

        public ActionResult FlowchartListIE()
        {
            return View();
        }

        public ActionResult JudgeFlowchart(int FlowChart_Master_UID)
        {
            var apiUrl = string.Format(@"ProductionPlanning/JudgeFlowchartAPI?Flowchart_Master_UID={0}", FlowChart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string UploadIEFlowCharts(HttpPostedFileBase upload_excel,int FlowChart_Master_UID)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/QueryFlowChartsAPI?Flowchart_Master_UID={0}&flag=2", FlowChart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<PagedListModel<FLowchart_Detail_IE_VM>>(responMessage.Content.ReadAsStringAsync().Result);
            var list = result.Items.ToList();

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
                var propertiesContent = FlowchartImportCommon.GetFlowchartDetailIEColumn();
               // bool flag = false;
                

                bool allColumnsAreError = false;

                List<FLowchart_Detail_IE_VM> ieList = new List<FLowchart_Detail_IE_VM>();
                //Excel行号
                int iRow = 4;
                //Excel列号
               // int iColumn;
                //int j = 1;


                for (iRow = 4; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }

                    FLowchart_Detail_IE_VM newMGDataItem = new FLowchart_Detail_IE_VM();
                    FLowchart_Detail_IE_VM existData = new FLowchart_Detail_IE_VM();

                    var Equipment_RequestQty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求")].Text;
                    var VariationEquipment_RequstQty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求(变动数)")].Text;
                    var RegularOP_Qty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "固定OP")].Text;
                    var VariationOP_Qty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "OP变动数")].Text;
                    var Match_Rule_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "班长(配比规则)")].Text;
                    var SquadLeader_Raito_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "班长(配比比例)")].Text;
                    var SquadLeader_Variable_Qty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "班长(变动数)")].Text;
                    var Technician_Raito_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "技术员(配比)")].Text;
                    var Technician_Variable_Qty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "技术员(变动数)")].Text;
                    var MaterialKeeper_Qty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "物料员")].Text;
                    var Others_Qty_Text = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "其他")].Text;

                    #region 验证不为空
                    if (string.IsNullOrEmpty(Equipment_RequestQty_Text))
                    {
                        errorInfo = string.Format("第{0}行设备需求不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(VariationEquipment_RequstQty_Text))
                    {
                        errorInfo = string.Format("第{0}行设备需求变动数不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(VariationOP_Qty_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行固定OP变动数不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(RegularOP_Qty_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行固定OP不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(SquadLeader_Raito_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行班长配比比例不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(SquadLeader_Variable_Qty_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行班长变动数不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(Technician_Raito_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行技术员配比比例不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(Technician_Variable_Qty_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行技术员变动数不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(MaterialKeeper_Qty_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行物料员不能为空", iRow);
                        break;
                    }

                    if (string.IsNullOrEmpty(Others_Qty_Text))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行其他不能为空", iRow);
                        break;
                    }
                    #endregion

                    #region 验证是否为数字
                    int t = 0;
                    bool ffff = int.TryParse(VariationEquipment_RequstQty_Text, out t);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行设备需求变动数不是数字", iRow);
                        break;
                    }

                    ffff = int.TryParse(VariationOP_Qty_Text, out t);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行固定OP变动数不是数字", iRow);
                        break;
                    }

                    ffff = int.TryParse(RegularOP_Qty_Text, out t);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行固定OP不是数字", iRow);
                        break;
                    }

                    decimal tt = 0;
                    ffff = decimal.TryParse(SquadLeader_Raito_Text, out tt);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行班长配比比例不是数字", iRow);
                        break;
                    }

                    ffff = int.TryParse(SquadLeader_Variable_Qty_Text, out t);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行班长变动数不是数字", iRow);
                        break;
                    }

                    ffff = decimal.TryParse(Technician_Raito_Text, out tt);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行技术员配比比例不是数字", iRow);
                        break;
                    }

                    ffff = int.TryParse(Technician_Variable_Qty_Text, out t);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行技术员变动数不是数字", iRow);
                        break;
                    }

                    ffff = int.TryParse(MaterialKeeper_Qty_Text, out t);
                    if (!ffff)
                    {
                        errorInfo = string.Format("第{0}行物料员不是数字", iRow);
                        break;
                    }

                    ffff = int.TryParse(Others_Qty_Text, out t);
                    if (!ffff)
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行其他不是数字", iRow);
                        break;
                    }
                    #endregion


                    newMGDataItem.Process_Seq= int.Parse(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "工序")].Text);
                    if (newMGDataItem.Process_Seq.Value>0)
                    {
                        existData = list.Find(a => a.Process_Seq == newMGDataItem.Process_Seq);
                    }

                    newMGDataItem.Funplant = existData.Funplant;  //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);

                    newMGDataItem.Process = existData.Process; //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value);

                    newMGDataItem.Total_Cycletime = existData.Total_Cycletime; //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);

                    newMGDataItem.Manpower_Ratio = existData.Manpower_Ratio; //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);

                    newMGDataItem.Estimate_Yield = existData.Estimate_Yield; //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);

                    newMGDataItem.Capacity_ByHour = existData.Capacity_ByHour;  //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);

                    newMGDataItem.Capacity_ByDay = existData.Capacity_ByDay; //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 8].Value);

                    newMGDataItem.Equipment_RequstQty = existData.Equipment_RequstQty; //ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 9].Value);

                    //设备需求(变动数)
                    newMGDataItem.VariationEquipment_RequstQty = Convert.ToInt32(VariationEquipment_RequstQty_Text);


                    //newMGDataItem.VariationEquipment_RequstQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 10].Value);
                    //newMGDataItem.VariationEquipment_RequstQtyVM=newMGDataItem.VariationEquipment_RequstQtyVM == null ?"0":newMGDataItem.VariationEquipment_RequstQtyVM;
                    //newMGDataItem.VariationEquipment_RequstQtyVM = newMGDataItem.VariationEquipment_RequstQtyVM == "" ? "0" : newMGDataItem.VariationEquipment_RequstQtyVM;

                    //固定OP
                    newMGDataItem.RegularOP_Qty = Convert.ToInt32(RegularOP_Qty_Text);

                    //newMGDataItem.RegularOP_QtyVM = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 11].Value);
                    //newMGDataItem.RegularOP_QtyVM = newMGDataItem.RegularOP_QtyVM == null ? "0" : newMGDataItem.RegularOP_QtyVM;
                    //newMGDataItem.RegularOP_QtyVM = newMGDataItem.RegularOP_QtyVM == "" ? "0" : newMGDataItem.RegularOP_QtyVM;

                    //OP变动数
                    newMGDataItem.VariationOP_Qty = Convert.ToInt32(VariationOP_Qty_Text);

                    //newMGDataItem.VariationOP_QtyVM = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 12].Value);
                    //newMGDataItem.VariationOP_QtyVM = newMGDataItem.VariationOP_QtyVM == null ? "0" : newMGDataItem.VariationOP_QtyVM;
                    //newMGDataItem.VariationOP_QtyVM = newMGDataItem.VariationOP_QtyVM == "" ? "0" : newMGDataItem.VariationOP_QtyVM;

                    //班长(配比比例)
                    newMGDataItem.SquadLeader_Raito = Convert.ToDecimal(SquadLeader_Raito_Text);

                    //newMGDataItem.SquadLeader_Raito = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);
                    //newMGDataItem.SquadLeader_Raito = newMGDataItem.SquadLeader_Raito == null ? "0" : newMGDataItem.SquadLeader_Raito;
                    //newMGDataItem.SquadLeader_Raito = newMGDataItem.SquadLeader_Raito == "" ? "0" : newMGDataItem.SquadLeader_Raito;

                    //班长(变动数)
                    newMGDataItem.SquadLeader_Variable_Qty = Convert.ToInt32(SquadLeader_Variable_Qty_Text);

                    //newMGDataItem.SquadLeader_Variable_Qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 15].Value);
                    //newMGDataItem.SquadLeader_Variable_Qty = newMGDataItem.SquadLeader_Variable_Qty == null ? "0" : newMGDataItem.SquadLeader_Variable_Qty;
                    //newMGDataItem.SquadLeader_Variable_Qty = newMGDataItem.SquadLeader_Variable_Qty == "" ? "0" : newMGDataItem.SquadLeader_Variable_Qty;

                    //技术员(配比比例)
                    newMGDataItem.Technician_Raito = Convert.ToDecimal(Technician_Raito_Text);

                    //newMGDataItem.Technician_Raito = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 16].Value);
                    //newMGDataItem.Technician_Raito = newMGDataItem.Technician_Raito == null ? "0" : newMGDataItem.Technician_Raito;
                    //newMGDataItem.Technician_Raito = newMGDataItem.Technician_Raito == "" ? "0" : newMGDataItem.Technician_Raito;

                    //技术员(变动数)
                    newMGDataItem.Technician_Variable_Qty = Convert.ToInt32(Technician_Variable_Qty_Text);

                    //newMGDataItem.Technician_Variable_Qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 17].Value);
                    //newMGDataItem.Technician_Variable_Qty = newMGDataItem.Technician_Variable_Qty == null ? "0" : newMGDataItem.Technician_Variable_Qty;
                    //newMGDataItem.Technician_Variable_Qty = newMGDataItem.Technician_Variable_Qty == "" ? "0" : newMGDataItem.Technician_Variable_Qty;

                    //物料员
                    newMGDataItem.MaterialKeeper_Qty = Convert.ToInt32(MaterialKeeper_Qty_Text);

                    //newMGDataItem.MaterialKeeper_QtyVM = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 18].Value);
                    //newMGDataItem.MaterialKeeper_QtyVM = newMGDataItem.MaterialKeeper_QtyVM == null ? "0" : newMGDataItem.MaterialKeeper_QtyVM;
                    //newMGDataItem.MaterialKeeper_QtyVM = newMGDataItem.MaterialKeeper_QtyVM == "" ? "0" : newMGDataItem.MaterialKeeper_QtyVM;

                    //其他
                    newMGDataItem.Others_Qty = Convert.ToInt32(Others_Qty_Text);

                    //newMGDataItem.Others_QtyVM = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 19].Value);
                    //newMGDataItem.Others_QtyVM = newMGDataItem.Others_QtyVM == null ? "0" : newMGDataItem.Others_QtyVM;
                    //newMGDataItem.Others_QtyVM = newMGDataItem.Others_QtyVM == "" ? "0" : newMGDataItem.Others_QtyVM;

                    newMGDataItem.Notes = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "备注")].Text;


                    //配比规则
                    var match = worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "班长(配比规则)")].Text;
                    switch (match)
                    {
                        case "人机比":
                            newMGDataItem.Match_Rule = 1;
                            //（设备需求 + 设备需求变动数） × 班长配比比例 + 班长配比变动数
                            var SQty = (Convert.ToInt32(Equipment_RequestQty_Text) + Convert.ToInt32(VariationEquipment_RequstQty_Text)) * Convert.ToDecimal(SquadLeader_Raito_Text) + Convert.ToInt32(SquadLeader_Variable_Qty_Text);
                            newMGDataItem.SquadLeader_Qty = Convert.ToInt32(SQty);
                            break;

                        case "人数比":
                            newMGDataItem.Match_Rule = 2;
                            //(固定OP + OP变动数 + 物料员 + 其他) * 班长配比比例
                            newMGDataItem.SquadLeader_Qty = (Convert.ToInt32(RegularOP_Qty_Text) + Convert.ToInt32(VariationOP_Qty_Text) + Convert.ToInt32(MaterialKeeper_Qty_Text) + Convert.ToInt32(Others_Qty_Text)) * Convert.ToInt32(SquadLeader_Raito_Text);
                            break;
                    }

                    //技术员只有人机比
                    //设备需求 + 设备需求变动数） × 技术员配比规则 + 技术员配比变动数
                    var TQty = (Convert.ToInt32(Equipment_RequestQty_Text) + Convert.ToInt32(VariationEquipment_RequstQty_Text)) * Convert.ToDecimal(Technician_Raito_Text) + Convert.ToInt32(Technician_Variable_Qty_Text);
                    newMGDataItem.Technician_Qty = Convert.ToInt32(TQty);

                    //if (Exworksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesContent, "班长(配比规则)")].Text == "人机比")
                    //{
                    //    newMGDataItem.Match_RuleVM = "1";
                    //    //（设备需求 + 设备需求变动数） × 班长配比比例 + 班长配比变动数
                    //    newMGDataItem.SquadLeader_Qty = (Convert.ToInt32(newMGDataItem.Equipment_RequstQtyVM) + Convert.ToInt32(newMGDataItem.VariationEquipment_RequstQtyVM)) * Convert.ToInt32(newMGDataItem.SquadLeader_Raito) + Convert.ToInt32(newMGDataItem.SquadLeader_Variable_Qty);
                    //}
                    //else if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value) == "人数比")
                    //{
                    //    newMGDataItem.Match_RuleVM = "2";
                    //    //(固定OP + OP变动数 + 物料员 + 其他) * 班长配比比例
                    //    newMGDataItem.SquadLeader_Qty = (Convert.ToInt32(newMGDataItem.RegularOP_QtyVM) + Convert.ToInt32(newMGDataItem.VariationOP_QtyVM) + Convert.ToInt32(newMGDataItem.MaterialKeeper_QtyVM) + Convert.ToInt32(newMGDataItem.Others_QtyVM)) * Convert.ToInt32(newMGDataItem.SquadLeader_Raito);
                    //}


                    //编辑还是新增
                    newMGDataItem.IEFlag = existData.IEFlag;

                    //if (newMGDataItem.Process_Seq!=existData.Process_Seq)
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行工序与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (newMGDataItem.Funplant != existData.Funplant)
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行功能厂与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (newMGDataItem.Process != existData.Process)
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行制程与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (Convert.ToDecimal(newMGDataItem.Total_CycletimeVM) != Convert.ToDecimal(existData.Total_CycletimeVM))
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行总CT时间(/秒)与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (Convert.ToDecimal(newMGDataItem.Manpower_RatioVM) != Convert.ToDecimal(existData.Manpower_RatioVM))
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行人机配比与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (Convert.ToDecimal(newMGDataItem.Estimate_YieldVM.Replace("%","")) != Convert.ToDecimal(existData.Estimate_YieldVM.Replace("%","")))
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行目标良率与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (Convert.ToDecimal(newMGDataItem.Capacity_ByHourVM) != Convert.ToDecimal(existData.Capacity_ByHourVM))
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行1小时产能与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (Convert.ToInt32(newMGDataItem.Capacity_ByDayVM) != Convert.ToInt32(existData.Capacity_ByDayVM))
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行1天产能与现有制程不一致", iRow);
                    //    break;
                    //}
                    //if (Convert.ToInt32(newMGDataItem.Equipment_RequstQtyVM) != Convert.ToInt32(existData.Equipment_RequstQtyVM))
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行设备需求与现有制程不一致", iRow);
                    //    break;
                    //}




                    //if (newMGDataItem.VariationEquipment_RequstQtyVM != existData.VariationEquipment_RequstQtyVM)
                    //{
                    //    allColumnsAreError = true;
                    //    errorInfo = string.Format("第{0}行设备需求(变动数)与现有制程不一致", iRow);
                    //    break;
                    //}




                    newMGDataItem.Flowchart_Master_UID = existData.Flowchart_Master_UID;
                    newMGDataItem.Flowchart_Detail_IE_UID = existData.Flowchart_Detail_IE_UID;
                    newMGDataItem.Flowchart_Detail_ME_UID = existData.Flowchart_Detail_ME_UID;
                    newMGDataItem.Modified_UID= this.CurrentUser.AccountUId;
                    ieList.Add(newMGDataItem);
                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }
                var json = JsonConvert.SerializeObject(ieList);
                string api = string.Format("ProductionPlanningForIE/UpdateFlowchartIEListAPI");
                HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(json, api);
                var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                return result1;
            }
        }

        #endregion

        #region IE 人力配比明细界面

        public ActionResult FlowchartListIEMatching(string FlowChart_Master_UID, string ProjectName)
        {
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.ProjectName = ProjectName;
            return View();
        }

        public ActionResult QueryFlowCharts(Flowchart_Detail_ProductionPlanning entity)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/QueryFlowChartsAPI?Flowchart_Master_UID={0}&flag=1", entity.Flowchart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DownloadIEFlowCharts(int Flowchart_Master_UID,string Project_Name)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/QueryFlowChartsAPI?Flowchart_Master_UID={0}&flag=2", Flowchart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<PagedListModel<FLowchart_Detail_IE_VM>>(responMessage.Content.ReadAsStringAsync().Result);
            var list = result.Items.ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FlowChart_IE");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var propertiesHead = new[]
                {
                    "专案名称"
                };
            var propertiesContent = new[]
                {
                    "工序",
                    "功能厂",
                    "制程",
                    "总CT时间(/秒)",
                    "人机配比",
                    "目标良率",
                    "1小时产能(/台)",
                    "1天产能(/台)",
                    "设备需求",
                    "设备需求(变动数)",
                    "固定OP",
                    "OP变动数",
                    "班长(配比规则)",
                    "班长(配比比例)",
                    "班长(变动数)",
                    "技术员(配比)",
                    "技术员(变动数)",
                    "物料员",
                    "其他",
                    "备注"
                };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("IEFlowChart");

                //填充Title内容
                for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];

                    switch (colIndex)
                    {
                        case 0:
                            worksheet.Cells[2, colIndex + 1].Value = Project_Name;
                            break;
                    }
                }

                //填充Content内容
                for (int colIndex = 0; colIndex < propertiesContent.Length; colIndex++)
                {
                    worksheet.Cells[3, colIndex + 1].Value = propertiesContent[colIndex];
                }

                for (int index = 0; index < list.Count(); index++)
                {
                    var currentRecord = list[index];
                    //序号
                    worksheet.Cells[index + 4, 1].Value = currentRecord.Process_Seq;
                    //功能厂
                    worksheet.Cells[index + 4, 2].Value = currentRecord.Funplant;
                    //制程
                    worksheet.Cells[index + 4, 3].Value = currentRecord.Process;
                    //总CT时间
                    worksheet.Cells[index + 4, 4].Value = currentRecord.Total_Cycletime; //.Total_CycletimeVM;
                    //人机配比
                    worksheet.Cells[index + 4, 5].Value = currentRecord.Manpower_Ratio; //.Manpower_RatioVM;

                    //目标良率
                    worksheet.Cells[index + 4, 6].Value = currentRecord.Estimate_Yield; //.Estimate_YieldVM;
                    //worksheet.Cells[index + 4, 6].Style.Numberformat.Format = "0.00%";
                    //1小时产能
                    worksheet.Cells[index + 4, 7].Value = currentRecord.Capacity_ByHour; //.Capacity_ByHourVM;
                    //1天产能
                    worksheet.Cells[index + 4, 8].Value = currentRecord.Capacity_ByDay; //.Capacity_ByDayVM;
                    //设备需求
                    worksheet.Cells[index + 4, 9].Value = currentRecord.Equipment_RequstQty; //.Equipment_RequstQtyVM;
                    //设备需求（变动数）
                    worksheet.Cells[index + 4, 10].Value = currentRecord.VariationEquipment_RequstQty; //.VariationEquipment_RequstQtyVM == null?"0": currentRecord.VariationEquipment_RequstQtyVM;
                    //worksheet.Cells[index + 4, 10].Value = currentRecord.VariationEquipment_RequstQtyVM == "" ? "0" : currentRecord.VariationEquipment_RequstQtyVM;
                    //固定OP
                    worksheet.Cells[index + 4, 11].Value = currentRecord.RegularOP_Qty; //.RegularOP_QtyVM == null ? "0" : currentRecord.RegularOP_QtyVM;
                    //worksheet.Cells[index + 4, 11].Value = currentRecord.RegularOP_QtyVM == "" ? "0" : currentRecord.RegularOP_QtyVM;
                    //worksheet.SelectedRange[worksheet.Cells[index + 4, 12].DataValidation.AddListDataValidation(]
                    //worksheet.Cells[index + 4, 12].DataValidation.AddListDataValidation();
                    //固定OP（变动数）
                    worksheet.Cells[index + 4, 12].Value = currentRecord.VariationOP_Qty; //.VariationOP_QtyVM == null ? "0" : currentRecord.VariationOP_QtyVM;
                    //worksheet.Cells[index + 4, 12].Value = currentRecord.VariationOP_QtyVM == "" ? "0" : currentRecord.VariationOP_QtyVM;
                    if (currentRecord.Match_Rule != 2)
                    {
                        worksheet.Cells[index + 4, 13].Value = "人机比";
                    }
                    else
                    {
                        worksheet.Cells[index + 4, 13].Value = "人数比";
                    }
                    worksheet.Cells[index + 4, 14].Value = currentRecord.SquadLeader_Raito;     //.Monitor_Match_RaitoVM == null ? "0" : currentRecord.Monitor_Match_RaitoVM;
                    worksheet.Cells[index + 4, 15].Value = currentRecord.SquadLeader_Variable_Qty; //.VariableMonitor__QtyVM == null ? "0" : currentRecord.VariableMonitor__QtyVM;
                    worksheet.Cells[index + 4, 16].Value = currentRecord.Technician_Raito;      //.Technician_Match_RaitoVM == null ? "0" : currentRecord.Technician_Match_RaitoVM;
                    worksheet.Cells[index + 4, 17].Value = currentRecord.Technician_Variable_Qty; //.VariableTechnician__QtyVM == null ? "0" : currentRecord.VariableTechnician__QtyVM;
                    worksheet.Cells[index + 4, 18].Value = currentRecord.MaterialKeeper_Qty;    //.MaterialKeeper_QtyVM == null ? "0" : currentRecord.MaterialKeeper_QtyVM;

                    worksheet.Cells[index + 4, 19].Value = currentRecord.Others_Qty; //.Others_QtyVM == null ? "0" : currentRecord.Others_QtyVM;

                    //worksheet.Cells[index + 4, 14].Value = currentRecord.Monitor_Match_RaitoVM == "" ? "0" : currentRecord.Monitor_Match_RaitoVM;
                    //worksheet.Cells[index + 4, 15].Value = currentRecord.VariableMonitor__QtyVM == "" ? "0" : currentRecord.VariableMonitor__QtyVM;
                    //worksheet.Cells[index + 4, 16].Value = currentRecord.Technician_Match_RaitoVM == "" ? "0" : currentRecord.Technician_Match_RaitoVM;
                    //worksheet.Cells[index + 4, 17].Value = currentRecord.VariableTechnician__QtyVM == "" ? "0" : currentRecord.VariableTechnician__QtyVM;
                    //worksheet.Cells[index + 4, 18].Value = currentRecord.MaterialKeeper_QtyVM == "" ? "0" : currentRecord.MaterialKeeper_QtyVM;

                    //worksheet.Cells[index + 4, 19].Value = currentRecord.Others_QtyVM == "" ? "0" : currentRecord.Others_QtyVM;
                    worksheet.Cells[index + 4, 20].Value = currentRecord.Notes;
                    //目标良率
                    //worksheet.Cells[index + 4, 9].Value = currentRecord.Target_Yield;
                    //worksheet.Cells[index + 4, 9].Style.Numberformat.Format = "0.00%";
                    //计划目标
                    //worksheet.Cells[index + 4, 10].Value = currentRecord.Product_Plan;
                }

                //设置灰色背景
                var maxRow = list.Count() + 3;
                var colorRange = string.Format("A3:I{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = fileName };
        }

        public ActionResult QueryFlowChartByRangeNumber(int Flowchart_Detail_ME_UID, int RangeNumber)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/QueryFlowChartByRangeNumberAPI?Flowchart_Detail_ME_UID={0}&RangeNumber={1}", Flowchart_Detail_ME_UID, RangeNumber);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult SaveMappingData(string jsonWithProduct)
        {
            var apiUrl = "ProductionPlanningForIE/SaveMappingDataAPI";
            var entity = JsonConvert.DeserializeObject<Flowchart_Detail_IE_VMList>(jsonWithProduct);

            foreach(Flowchart_Detail_IE_VM data in entity.DataList)
            {
                data.Created_UID = this.CurrentUser.AccountUId;
                data.Created_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SaveIEFlowchartData(string jsonWithProduct)
        {
            var apiUrl = "ProductionPlanningForIE/SaveIEFlowchartDataAPI";
            var entity = JsonConvert.DeserializeObject<Flowchart_Detail_ProductionPlanningList>(jsonWithProduct);

            foreach (Flowchart_Detail_ProductionPlanning data in entity.DataList)
            {
                data.Created_UID = this.CurrentUser.AccountUId;
                data.Created_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult UpdateIEFlowchartData(string jsonWithProduct)
        {
            var apiUrl = "ProductionPlanningForIE/UpdateIEFlowchartDataAPI";
            var entity = JsonConvert.DeserializeObject<Flowchart_Detail_ProductionPlanningList>(jsonWithProduct);

            foreach (Flowchart_Detail_ProductionPlanning data in entity.DataList)
            {
                data.Created_UID = this.CurrentUser.AccountUId;
                data.Created_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFlowChartByFlowchart_Detail_ME_UID(int Flowchart_Detail_ME_UID)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/QueryFlowChartByFlowchart_Detail_ME_UID?Flowchart_Detail_ME_UID={0}", Flowchart_Detail_ME_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        #endregion


        #region IE 人力需求汇总表

        public ActionResult HumanResourcesSummary(string FlowChart_Master_UID, string ProjectName)
        {
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            if (Orgnizations != null && Orgnizations.Count > 0)
            {
                ViewBag.OP = Orgnizations[0].Plant;
            }
            else
            {
                ViewBag.OP = 0;
            }
            return View();
        }

        public ActionResult ShowHumanResourcesSummary(int InputPcs)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/GetHumanResourcesSummaryAPI?strWhere={0}", " where b.[FlowChart_Version]="+ InputPcs);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetInputDataForSelect(int project)
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/GetInputDataForSelectAPI?project={0}", project);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DownloadHumanResourcesSummary()
        {
            var apiUrl = string.Format(@"ProductionPlanningForIE/GetHumanResourcesSummaryAPI?strWhere={0}", "");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<PagedListModel<HumanResourcesSummaryDTO>>(responMessage.Content.ReadAsStringAsync().Result);
            var list = result.Items.ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("人力资源汇总表");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var propertiesHead = new[]
                {
                    "人力汇总需求",
                    "料件",
                    "投入量",
                    "直通率",
                    "产出"
                };
            var propertiesContent = new[]
                {
                    "DRI",
                    "班长当日出勤",
                    "技术员当日出勤",
                    "物料员当日出勤",
                    "作业员当日出勤",
                    "Total DL当日出勤",
                    "Total DL七轮休"
                };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("人力需求汇总");

                //填充Title内容
                for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
                {
                    switch (colIndex)
                    {
                        case 0:
                            worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                            break;
                        case 1:
                            worksheet.Cells[3, 1].Value = propertiesHead[colIndex];
                            break;
                        case 2:
                            worksheet.Cells[3, 2].Value = propertiesHead[colIndex];
                            break;
                        case 3:
                            worksheet.Cells[3, 3].Value = propertiesHead[colIndex];
                            break;
                        case 4:
                            worksheet.Cells[3,4].Value = propertiesHead[colIndex];
                            break;
                    }
                }

                //填充Content内容
                for (int colIndex = 0; colIndex < propertiesContent.Length; colIndex++)
                {
                    switch (colIndex)
                    {
                        case 0:
                            worksheet.Cells[5, 1].Value = propertiesContent[colIndex];
                            break;
                        case 1:
                            worksheet.Cells[5, 2].Value = propertiesContent[colIndex];
                            break;
                        case 2:
                            worksheet.Cells[5, 3].Value = propertiesContent[colIndex];
                            break;
                        case 3:
                            worksheet.Cells[5, 4].Value = propertiesContent[colIndex];
                            break;
                        case 4:
                            worksheet.Cells[5, 5].Value = propertiesContent[colIndex];
                            break;
                        case 5:
                            worksheet.Cells[5, 6].Value = propertiesContent[colIndex];
                            break;
                        case 6:
                            worksheet.Cells[5, 7].Value = propertiesContent[colIndex];
                            break;
                    }
                }

                for (int index = 0; index < list.Count(); index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[1, 1].Value = currentRecord.FlowerName+"人力需求汇总";
                    worksheet.Cells[2, 1].Value = "投入量&直通率";
                    worksheet.Cells[4, 1].Value =currentRecord.FlowerName;
                    worksheet.Cells[4, 2].Value = currentRecord.InputNum;
                    worksheet.Cells[4, 3].Value = currentRecord.OnePass;
                    worksheet.Cells[4, 4].Value = currentRecord.OutputNum;
                    //序号
                    worksheet.Cells[index +6, 1].Value = currentRecord.FunPlant;
                    //功能厂
                    worksheet.Cells[index + 6, 2].Value = currentRecord.SquadLeaderNow;
                    //制程
                    worksheet.Cells[index + 6, 3].Value = currentRecord.TechnicianNow;
                    //总CT时间
                    worksheet.Cells[index + 6, 4].Value = currentRecord.MaterialKeeperNow;
                    //人机配比
                    worksheet.Cells[index + 6, 5].Value = currentRecord.OPNow;

                    //目标良率
                    worksheet.Cells[index + 6, 6].Value = currentRecord.TotalNow;
                    //1小时产能
                    worksheet.Cells[index + 6, 7].Value = currentRecord.TotalRound;
                    //目标良率
                    //worksheet.Cells[index + 4, 9].Value = currentRecord.Target_Yield;
                    //worksheet.Cells[index + 4, 9].Style.Numberformat.Format = "0.00%";
                    //计划目标
                    //worksheet.Cells[index + 4, 10].Value = currentRecord.Product_Plan;
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = fileName };
        }
        #endregion


        #endregion




    }
}