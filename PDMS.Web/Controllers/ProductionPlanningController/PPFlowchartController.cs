using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Web.Business.Flowchart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers.ProductionPlanningController
{
    public class PPFlowchartController : WebControllerBase
    {
        #region PP数据维护主界面
        // GET: PPFlowchart
        public ActionResult FLowchartPPList()
        {
            var roleList = this.CurrentUser.GetUserInfo.RoleList.Select(m => m.Role_ID).ToList();
            ViewBag.RoleList = JsonConvert.SerializeObject(roleList);
            return View();
        }

        public ActionResult QueryPPFlowCharts(FlowChartModelSearch search, Page page)
        {
            search.RoleList = CurrentUser.GetUserInfo.RoleList;
            //获取一级SITE UID信息
            search.PlantUIDList = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList;
            //获取二级OPTYPES UID信息
            search.OPType_OrganizationUIDList = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList;
            search.OpTypes = this.CurrentUser.GetUserInfo.OpTypes;
            //获取用户的专案访问权限
            search.ProjectUIDList = this.CurrentUser.GetUserInfo.ProjectUIDList;
            var apiUrl = string.Format("PPFlowchart/QueryPPFlowChartsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }

        #endregion

        #region PP查看Flowchart明细
        public ActionResult FlowchartPPDetailList(int id, int Version)
        {
            ViewBag.ID = id;
            ViewBag.Version = Version;

            List<string> ReworkFlagList = new List<string>();
            ReworkFlagList.Add("");
            ReworkFlagList.Add(StructConstants.ReworkFlag.Rework);
            ReworkFlagList.Add(StructConstants.ReworkFlag.Repair);
            ViewBag.ReworkFlag = ReworkFlagList;

            Dictionary<string, string> IsQAProcessDir = new Dictionary<string, string>();
            IsQAProcessDir.Add("", "");
            IsQAProcessDir.Add(StructConstants.IsQAProcessType.InspectKey, StructConstants.IsQAProcessType.InspectText);
            IsQAProcessDir.Add(StructConstants.IsQAProcessType.PollingKey, StructConstants.IsQAProcessType.PollingText);
            IsQAProcessDir.Add(StructConstants.IsQAProcessType.InspectOQCKey, StructConstants.IsQAProcessType.InspectOQCText);
            IsQAProcessDir.Add(StructConstants.IsQAProcessType.InspectAssembleKey, StructConstants.IsQAProcessType.InspectAssembleText);
            IsQAProcessDir.Add(StructConstants.IsQAProcessType.AssembleOQCKey, StructConstants.IsQAProcessType.AssembleOQCText);
            ViewBag.IsQAProcess = IsQAProcessDir;


            Dictionary<int, string> funPlantList = new Dictionary<int, string>();
            funPlantList.Add(0, "All");

            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;

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

            return View();
        }

        public ActionResult QueryFlowchartPPDetails(int id, int Version)
        {
            var ddlapiUrl = string.Format("PPFlowchart/FlowchartPPDetailListAPI?id={0}&Version={1}", id, Version);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(ddlapiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFLDetailAssociationByID(int id)
        {
            var api = string.Format("PPFlowchart/QueryFLDetailAssociationByIDAPI?id={0}", id);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string QueryFLDetail(int id)
        {
            var api = string.Format("PPFlowchart/QueryFLDetailAPI?id={0}", id);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }

        public string CheckProcess(int Flowchart_Detail_ME_UID, string json, string Rework_Flag)
        {
            var api = string.Format("PPFlowchart/CheckProcessAPI?id={0}&json={1}", Flowchart_Detail_ME_UID, json);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }

        public string QueryFLProcessCheckedSelectByID(int id)
        {
            var api = string.Format("PPFlowchart/QueryFLProcessCheckedSelectByIDAPI?id={0}", id);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;

        }

        public ActionResult SavePPFlowchart(FlowChartDetailDTO dto)
        {
            var accountUID = this.CurrentUser.AccountUId;
            dto.Modified_UID = accountUID;
            var api = string.Format("PPFlowchart/SavePPFlowchartAPI");
            HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(dto, api);
            //var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return RedirectToAction("FlowchartPPDetailList", "PPFlowchart", new { id = dto.FlowChart_Master_UID, Version = dto.FlowChart_Version });
        }
        #endregion

        #region MP生产计划维护
        public ActionResult FLowchartPPMPMaintenanceList(int id, string Product_Phase, int Version)
        {
            ViewBag.ID = id;
            ViewBag.Version = Version;

            Dictionary<int, string> funPlantList = new Dictionary<int, string>();
            funPlantList.Add(0, "All");

            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;

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

            return View();
        }

        public ActionResult FlowchartPPMPMaintenanceInfo(FlowchartMeNPI npiModel, Page page)
        {
            var apiUrl = string.Format("PPFlowchart/QueryProductionSchedulMPAPI");
            HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(npiModel, page, apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public string CheckDownloadMPExcel(int id, int Version)
        {
            var apiUrl = string.Format("PPFlowchart/CheckDownloadMPExcelAPI?id={0}&Version={1}", id, Version);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }

        public FileResult DownloadMPExcel(int id, int Version)
        {
            //var apiUrl = string.Format("PPFlowchart/DownloadMPExcelAPI?id={0}&Version={1}", id, Version);
            //HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            //var result = responddlMessage.Content.ReadAsStringAsync().Result;
            //var list = JsonConvert.DeserializeObject<List<ProductionSchedulMPVM>>(result);
            string api = "FlowChart/QueryFlowChartAPI?FlowChart_Master_UID=" + id;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartGet>(result);


            var stream = new MemoryStream();
            var sheetName = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.MPPlan");
            var fileName = PathHelper.SetGridExportExcelName(sheetName);
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetMPHeadColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                SetMPExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                //foreach (var item in list)
                //{
                for (int j = 0; j < 2; j++)
                {
                    worksheet.Cells[iRow, 1].Value = item.SystemProjectDTO.Project_Name;
                    worksheet.Cells[iRow, 2].Value = item.FlowChartMasterDTO.FlowChart_Version;
                    worksheet.Cells[iRow, 3].Value = item.FlowChartMasterDTO.Product_Phase;

                    if (j == 0)
                    {
                        worksheet.Cells[iRow, 4].Value = StructConstants.PlanType.LocPlan;
                    }
                    else
                    {
                        worksheet.Cells[iRow, 4].Value = StructConstants.PlanType.WeekPlan;
                    }
                    //设置百分比格式
                    worksheet.Column(8).Style.Numberformat.Format = "0.00%";

                    iRow++;
                }
                //}
                //Excel最后一行行号
                int maxRow = iRow - 1;
                //设置灰色背景
                var colorRange = string.Format("A2:C{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置Excel日期格式yyyy-MM-dd
                var dateColorRange = string.Format("E2:E{0}", maxRow);
                worksheet.Cells[dateColorRange].Style.Numberformat.Format = FormatConstants.DateTimeFormatStringByDate;
                //设置主键列隐藏
                //worksheet.Column(5).Hidden = true;
                //设置边框
                worksheet.Cells[string.Format("A1:F{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:F{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:F{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();

            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        private void SetMPExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];
            //worksheet.Cells[1, 8].Value = propertiesHead[7];
            //worksheet.Cells[1, 9].Value = propertiesHead[8];
            //worksheet.Cells[1, 10].Value = propertiesHead[9];


            //设置列宽
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 10;
            worksheet.Column(3).Width = 10;
            worksheet.Column(4).Width = 17;
            worksheet.Column(5).Width = 17;
            worksheet.Column(6).Width = 17;
            //worksheet.Column(8).Width = 17;
            //worksheet.Column(9).Width = 10;
            //worksheet.Column(10).Width = 10;


            worksheet.Cells["A1:F1"].Style.Font.Bold = true;
            worksheet.Cells["A1:F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:F1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }

        public string ImportMPExcel(HttpPostedFileBase upload_excel, int FlowChart_Master_UID, int FlowChart_Version)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                var sheetName = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.MPPlan");

                if (worksheet == null || worksheet.Name != sheetName)
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet");
                    return errorInfo;
                }
                int currentRow = 2;
                List<ProductionSchedulDTO> list = new List<ProductionSchedulDTO>();
                errorInfo = GetExcelImportDTOList(worksheet, currentRow, FlowChart_Master_UID, FlowChart_Version, list);
                if (string.IsNullOrEmpty(errorInfo))
                {
                    //var groupCount = list.GroupBy(m =>new {m.PlanType,m.FlowChart_Detail_UID }).Count();
                    //if (groupCount != list.Count())
                    //{
                    //    errorInfo = "计划类型含有重复项，请修改后再导入";
                    //}
                    //else
                    //{
                    //开始导入
                    var json = JsonConvert.SerializeObject(list);
                    string api = string.Format("PPFlowchart/ImportMPExcelAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
                    var result = responMessage.Content.ReadAsStringAsync().Result;

                    //}
                }
                return errorInfo;

            }

        }

        private string GetExcelImportDTOList(ExcelWorksheet worksheet, int currentRow, int FlowChart_Master_UID, int FlowChart_Version, List<ProductionSchedulDTO> list)
        {

            var propertiesContent = FlowchartImportCommon.GetMPHeadColumn();
            string errorInfo = string.Empty;
            while (worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "专案名称")].Value != null)
            {
                int Input = 0;
                DateTime? Product_Date = null;
                int PlanType = 0;
                //int DayType = 0;
                // decimal Yield = 0;

                //计划类型
                var planType = worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "计划类型")].Text;
                if (string.IsNullOrWhiteSpace(planType))
                {
                    errorInfo = string.Format("第{0}行请输入计划类型", currentRow);
                    break;
                }
                else if (planType != StructConstants.PlanType.LocPlan && planType != StructConstants.PlanType.WeekPlan)
                {
                    errorInfo = string.Format("第{0}行计划类型请输入Loc计划或四周生产计划", currentRow);
                    break;
                }
                else
                {
                    switch (planType)
                    {
                        case StructConstants.PlanType.LocPlan:
                            PlanType = StructConstants.PlanType.LocPlanValue;
                            break;
                        case StructConstants.PlanType.WeekPlan:
                            PlanType = StructConstants.PlanType.WeekPlanValue;
                            break;
                    }
                }


                //日期
                var time = worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "日期")].Text;
                if (string.IsNullOrEmpty(time))
                {
                    errorInfo = string.Format("第{0}行请输入日期", currentRow);
                    break;
                }
                else
                {
                    Product_Date = Convert.ToDateTime(time);
                    if (DateTime.Now.Date > Product_Date)
                    {
                        errorInfo = string.Format("第{0}行日期不能小于现在的日期", currentRow);
                        break;
                    }
                }

                //投入数
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

                //良率
                //decimal validDecimal = 0;
                //var yield = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "良率")].Value);
                //var isDecimal = decimal.TryParse(yield, out validDecimal);
                //if (isDecimal)
                //{
                //    Yield = Convert.ToDecimal(yield);
                //    if (Yield == 0)
                //    {
                //        errorInfo = string.Format("第{0}行良率不能为0", currentRow);
                //        break;
                //    }
                //}
                //else
                //{
                //    errorInfo = string.Format("第{0}行良率必须为数字", currentRow);
                //    break;
                //}
                //int Flowchart_Detail_UID = Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "Flowchart_Detail_UID")].Value);


                ProductionSchedulDTO dto = new ProductionSchedulDTO();
                //dto.FlowChart_Detail_UID = Flowchart_Detail_UID;
                dto.FlowChart_Master_UID = FlowChart_Master_UID;
                dto.FlowChart_Version = FlowChart_Version;
                dto.Product_Date = Product_Date.Value;
                dto.Input_Qty = Input;
                dto.PlanType = PlanType;
                dto.Target_Yield = 0; //Yield;
                dto.Created_UID = this.CurrentUser.AccountUId;
                dto.Created_Date = DateTime.Now;
                dto.Modified_UID = this.CurrentUser.AccountUId;
                dto.Modified_Date = DateTime.Now;
                list.Add(dto);

                currentRow++;
            }
            if (string.IsNullOrEmpty(errorInfo))
            {
                //检查导入的数据是否存在
                var json = JsonConvert.SerializeObject(list);
                string api = string.Format("PPFlowchart/CheckImportMPExcelAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                result = result.Replace("\"", "");
                if (!string.IsNullOrEmpty(result))
                {
                    errorInfo = result;
                }
            }
            return errorInfo;
        }

        public string DeleteInfoByUIDList(string json)
        {
            var apiUrl = string.Format("PPFlowchart/DeleteInfoByUIDListAPI?json={0}", json);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }

        public ActionResult SaveMPInfo(ProductionSchedulDTO dto, int id, string Product_Phase, int Version)
        {
            var apiUrl = string.Format("PPFlowchart/SaveMPInfoAPI");
            HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return RedirectToAction("FLowchartPPMPMaintenanceList", "PPFlowchart", new { id = id, Product_Phase = Product_Phase, Version = Version });
        }
        #endregion

        #region 现有人力管理
        public ActionResult CurrentStaffList()
        {
            //var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;
            //var plantIDList = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList;
            //var opTypeList = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList;
            //var projectIDList = this.CurrentUser.GetUserInfo.ProjectUIDList;
            //Dictionary<int, string> plantDir = new Dictionary<int, string>();
            //Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            //Dictionary<int, string> partTypeDir = new Dictionary<int, string>();
            //Dictionary<int, string> funPlantDir = new Dictionary<int, string>();

            //foreach (var orgInfoItem in orgInfo)
            //{
            //    plantDir.Add(orgInfoItem.Plant_OrganizationUID.Value, orgInfoItem.Plant);
            //    opTypeDir.Add(orgInfoItem.OPType_OrganizationUID.Value, orgInfoItem.OPType);
            //    funPlantDir.Add(orgInfoItem.Funplant_OrganizationUID.Value, orgInfoItem.Funplant);
            //}

            //ViewBag.Plant = plantDir;
            //ViewBag.OPType = opTypeDir;
            //ViewBag.FunPlant = funPlantDir;
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

        public ActionResult CurrentStaffInfo(CurrentStaffDTO dto, Page page)
        {
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;
            if (orgInfo.Count() == 0)
            {
                dto.Plant_Organization_UID = 0;
                dto.BG_Organization_UID = 0;
            }
            var apiUrl = string.Format("PPFlowchart/QueryCurrentStaffInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportStaffExcel(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;
            if (orgInfo.Count() == 0)
            {
                errorInfo = "请先分配厂区和Business Group";
                return errorInfo;
            }

            using (var xlPackage = new ExcelPackage(uploadName.InputStream))
            {
                int iRow = 2;
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                //int totalRows = worksheet.Dimension.End.Row;

                List<CurrentStaffDTO> list = new List<CurrentStaffDTO>();

                //int funcPlantUID = 0;
                while (worksheet.Cells[iRow, 1].Value != null)
                {
                    var productDate = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 1].Value);
                    var productPhase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);
                    var opQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value);
                    var monQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);
                    var technicalQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    var materialQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);
                    var otherQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);


                    if (string.IsNullOrEmpty(productDate))
                    {
                        errorInfo = "请输入日期";
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(productPhase))
                    {
                        errorInfo = "请输入阶段";
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(opQty))
                    {
                        errorInfo = "请输入OP人力";
                        return errorInfo;
                    }

                    int validInt = 0;
                    var isInt = int.TryParse(opQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行OP人力必须为整数", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(monQty))
                    {
                        errorInfo = "请输入班长";
                        return errorInfo;
                    }

                    isInt = int.TryParse(monQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行班长必须为整数", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(technicalQty))
                    {
                        errorInfo = "请输入技术员";
                        return errorInfo;
                    }
                    isInt = int.TryParse(technicalQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行技术员必须为整数", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(materialQty))
                    {
                        errorInfo = "请输入物料员";
                        return errorInfo;
                    }
                    isInt = int.TryParse(materialQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行物料员必须为整数", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(otherQty))
                    {
                        errorInfo = "请输入其他人力";
                        return errorInfo;
                    }
                    isInt = int.TryParse(otherQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行其他人力必须为整数", iRow);
                        return errorInfo;
                    }


                    CurrentStaffDTO item = new CurrentStaffDTO();
                    item.Plant_Organization_UID = orgInfo.First().Plant_OrganizationUID.Value;
                    item.BG_Organization_UID = orgInfo.First().OPType_OrganizationUID.Value;
                    //item.FunPlant_OrganizationUID = funcPlantUID;
                    item.ProductDate = Convert.ToDateTime(productDate);
                    item.Product_Phase = productPhase;
                    item.OP_Qty = Convert.ToInt32(opQty);
                    item.Monitor_Staff_Qty = Convert.ToInt32(monQty);
                    item.Technical_Staff_Qty = Convert.ToInt32(technicalQty);
                    item.Material_Keeper_Qty = Convert.ToInt32(materialQty);
                    item.Others_Qty = Convert.ToInt32(otherQty);
                    item.Created_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Created_Date = DateTime.Now;
                    item.Modified_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Modified_Date = DateTime.Now;
                    item.LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                    list.Add(item);
                    iRow++;
                }

                int totalRows = iRow - 2;
                if (list.Distinct().Count() != totalRows)
                {
                    errorInfo = "导入的Excel有重复行";
                    return errorInfo;
                }

                string json = JsonConvert.SerializeObject(list);


                if (string.IsNullOrEmpty(errorInfo))
                {
                    //检查导入的数据是否存在
                    string api = string.Format("PPFlowchart/CheckImportCurrentStaffExcelAPI");
                    HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(json, api);
                    var checkResult = checkResponMessage.Content.ReadAsStringAsync().Result;
                    checkResult = checkResult.Replace("\"", "");
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        errorInfo = checkResult;
                        return errorInfo;
                    }
                }


                var apiUrl = string.Format("PPFlowchart/ImportCurrentStaffInfoAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;

            }
            return string.Empty;
        }

        public ActionResult SaveStaffInfo(CurrentStaffDTO dto)
        {
            var apiUrl = string.Format("PPFlowchart/SaveStaffInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return RedirectToAction("CurrentStaffList");
        }
        #endregion

        #region 离职率排班计划维护
        public ActionResult TurnoverSchedulingList()
        {
            return View();
        }

        public ActionResult TurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO dto, Page page)
        {
            //获取用户所属的三级权限，从而获得4级权限功能厂，不考虑跨厂区权限
            if (this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count() != 0)
            {
                dto.Plant_Organization_UID = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.First();
            }
            if (this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.Count() != 0)
            {
                dto.BG_Organization_UID = this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.First();
            }

            //dto.FunPlant_OrganizationUID = orgInfo.First().Funplant_OrganizationUID.Value;
            var apiUrl = string.Format("PPFlowchart/QueryTurnoverSchedulingInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SaveDemissionInfo(DemissionRateAndWorkScheduleDTO dto)
        {
            var apiUrl = string.Format("PPFlowchart/SaveDemissionInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return RedirectToAction("TurnoverSchedulingList");
        }

        public string ImportTurnoverExcel(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            if (this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count() == 0 || this.CurrentUser.GetUserInfo.OPType_OrganizationUIDList.Count() == 0)
            {
                errorInfo = "请先分配厂区和Business Group";
                return errorInfo;
            }

            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;

            using (var xlPackage = new ExcelPackage(uploadName.InputStream))
            {
                Dictionary<int, string> funPlantDir = new Dictionary<int, string>();

                //foreach (var orgInfoItem in orgInfo)
                //{
                //    funPlantDir.Add(orgInfoItem.Funplant_OrganizationUID.Value, orgInfoItem.Funplant);
                //}

                int iRow = 2;
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                //int totalRows = worksheet.Dimension.End.Row;

                List<DemissionRateAndWorkScheduleDTO> list = new List<DemissionRateAndWorkScheduleDTO>();
                string Plant, OpType;
                DateTime? Product_Date;
                string Product_Phase;
                while (worksheet.Cells[iRow, 1].Value != null)
                {
                    Plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 1].Value);
                    if (string.IsNullOrEmpty(Plant))
                    {
                        errorInfo = string.Format("第[{0}]行厂区不能为空", iRow);
                        return errorInfo;
                    }
                    if (this.CurrentUser.GetUserInfo.OrgInfo.First().Plant != Plant)
                    {
                        errorInfo = string.Format("第[{0}]行厂区设置错误",iRow);
                        return errorInfo;
                    }

                    OpType = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);
                    if (string.IsNullOrEmpty(OpType))
                    {
                        errorInfo = string.Format("第[{0}]行Business Group不能为空", iRow);
                    }
                    if (this.CurrentUser.GetUserInfo.OrgInfo.First().OPType != OpType)
                    {
                        errorInfo = string.Format("第[{0}]行Business Group设置错误", iRow);
                        return errorInfo;
                    }

                    Product_Date = Convert.ToDateTime(worksheet.Cells[iRow, 3].Value);
                    Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);
                    if (string.IsNullOrEmpty(Product_Phase))
                    {
                        errorInfo = string.Format("第[{0}]行阶段不能为空", iRow);
                        return errorInfo;
                    }

                    var NPIRate = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    var NPIQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);
                    var MPRate = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);
                    var MPQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 8].Value);
                    var Schedule = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 9].Value);

                    //NPI离职率
                    if (string.IsNullOrEmpty(NPIRate))
                    {
                        errorInfo = string.Format("第[{0}]行NPI离职率不能为空", iRow);
                        return errorInfo;
                    }


                    decimal validDecimal = 0;
                    var isDecimal = decimal.TryParse(NPIRate, out validDecimal);
                    if (!isDecimal)
                    {
                        errorInfo = string.Format("第{0}行NPI离职率必须为数字", iRow);
                        return errorInfo;
                    }


                    //NPI预计招募人力
                    if (string.IsNullOrEmpty(NPIQty))
                    {
                        errorInfo = string.Format("第[{0}]行NPI预计招募人力不能为空", iRow);
                        return errorInfo;
                    }


                    int validInt = 0;
                    var isInt = int.TryParse(NPIQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行NPI预计招募人力必须为整数", iRow);
                        return errorInfo;
                    }


                    //MP离职率
                    if (string.IsNullOrEmpty(MPRate))
                    {
                        errorInfo = string.Format("第[{0}]行MP离职率不能为空", iRow);
                        return errorInfo;
                    }

                    isDecimal = decimal.TryParse(MPRate, out validDecimal);
                    if (!isDecimal)
                    {
                        errorInfo = string.Format("第{0}行MP离职率必须为数字", iRow);
                        return errorInfo;
                    }



                    //MP预计招募人力
                    if (string.IsNullOrEmpty(MPQty))
                    {
                        errorInfo = string.Format("第[{0}]行MP预计招募人力不能为空", iRow);
                        return errorInfo;
                    }
                    isInt = int.TryParse(MPQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行MP预计招募人力必须为整数", iRow);
                        return errorInfo;
                    }



                    if (string.IsNullOrEmpty(Schedule))
                    {
                        errorInfo = string.Format("第[{0}]行排班机制不能为空", iRow);
                        return errorInfo;
                    }


                    DemissionRateAndWorkScheduleDTO item = new DemissionRateAndWorkScheduleDTO();

                    item.Plant_Organization_UID = orgInfo.First().Plant_OrganizationUID.Value;
                    item.BG_Organization_UID = orgInfo.First().OPType_OrganizationUID.Value;
                    //item.FunPlant_OrganizationUID = orgInfo.First().Funplant_OrganizationUID.Value;
                    item.Product_Date = Product_Date.Value;
                    item.DemissionRate_NPI = Convert.ToDecimal(NPIRate);
                    item.NPI_RecruitStaff_Qty = Convert.ToInt32(NPIQty);
                    item.DemissionRate_MP = Convert.ToDecimal(MPRate);
                    item.MP_RecruitStaff_Qty = Convert.ToInt32(MPQty);
                    item.WorkSchedule = Schedule;
                    item.Created_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Modified_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Created_Date = DateTime.Now;
                    item.Modified_Date = DateTime.Now;
                    item.LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                    list.Add(item);
                    iRow++;
                }

                int totalRows = iRow - 2;
                if (list.Distinct().Count() != totalRows)
                {
                    errorInfo = "导入的Excel有重复行";
                    return errorInfo;
                }

                //检查数据库是否有重复
                string json = JsonConvert.SerializeObject(list);
                var checkApiUrl = string.Format("PPFlowchart/CheckImportTurnoverExcelAPI");
                HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(json, checkApiUrl);
                errorInfo = checkResponMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                if (!string.IsNullOrEmpty(errorInfo))
                {
                    return errorInfo;
                }
                var apiUrl = string.Format("PPFlowchart/ImportTurnoverExcelAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
            }
            return string.Empty;
        }


        #endregion


        #region 实际人力数据录入
        public ActionResult ActualPowerList(int id, int Version)
        {
            ViewBag.ID = id;
            ViewBag.Version = Version;

            Dictionary<int, string> funPlantList = new Dictionary<int, string>();
            funPlantList.Add(0, "All");

            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;

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


            return View();
        }

        public ActionResult ActualPowerInfo(ActiveManPowerSearchVM vm, Page page)
        {
            var api = string.Format("PPFlowchart/ActualPowerInfoAPI");
            HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(vm, page, api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult ActualPowerInfoTwo(ActiveManPowerSearchVM vm, Page page)
        {
            return null;
            //var api = string.Format("PPFlowchart/ActualPowerInfoTwoAPI?id={0}&Version={1}", id, Version);
            //HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(vm,page,api);
            //var result = responddlMessage.Content.ReadAsStringAsync().Result;
            //return Content(result, "application/json");
        }

        public string CheckDownloadManPowerExcel(int id, int Version)
        {
            var api = string.Format("PPFlowchart/CheckDownloadManPowerExcelAPI?id={0}&Version={1}", id, Version);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result.Replace("\"", ""); ;
            return result;
        }


        public FileResult DownloadManPowerExcel(int id, int Version)
        {
            var api = string.Format("PPFlowchart/GetManPowerDownLoadInfoAPI?id={0}&Version={1}", id, Version);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<ActiveManPowerVM>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("实际人力生产计划维护");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetManPowerHeadColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("实际人力生产计划维护");
                SetManPowerExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                foreach (ActiveManPowerVM item in list)
                {
                    worksheet.Cells[iRow, 1].Value = item.Process_Seq;
                    worksheet.Cells[iRow, 2].Value = item.FunPlant;
                    worksheet.Cells[iRow, 3].Value = item.Process;
                    worksheet.Cells[iRow, 4].Value = item.FlowChart_Detail_UID;
                    worksheet.Cells[iRow, 5].Value = item.Flowchart_Detail_ME_UID;
                    worksheet.Cells[iRow, 6].Value = item.Father_UID;
                    worksheet.Cells[iRow, 7].Value = item.Child_UID;
                    iRow++;
                }
                //Excel最后一行行号
                int maxRow = iRow - 1;
                //设置灰色背景
                var colorRange = string.Format("A2:G{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置Excel日期格式yyyy-MM-dd
                var dateColorRange = string.Format("I2:I{0}", maxRow);
                worksheet.Cells[dateColorRange].Style.Numberformat.Format = FormatConstants.DateTimeFormatStringByDate;
                //设置主键列隐藏
                worksheet.Column(4).Hidden = true;
                worksheet.Column(5).Hidden = true;
                worksheet.Column(6).Hidden = true;
                worksheet.Column(7).Hidden = true;
                //设置边框
                worksheet.Cells[string.Format("A1:N{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:N{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:N{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        private void SetManPowerExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            //int j = 1;
            for (int i = 0; i < propertiesHead.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = propertiesHead[i];
                //设置列宽
                worksheet.Column(i + 1).Width = 15;
            }

            worksheet.Cells["A1:N1"].Style.Font.Bold = true;
            worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }

        public string ImportManPower(HttpPostedFileBase upload_excel, int id, int Version)
        {
            string errorInfo = string.Empty;
            var resourceApi = string.Empty;

            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null || worksheet.Name != "实际人力生产计划维护")
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet"); //"没有worksheet内容";
                    return errorInfo;
                }

                int iRow = 2;
                var propertiesContent = FlowchartImportCommon.GetManPowerHeadColumn();
                List<ActiveManPowerVM> list = new List<ActiveManPowerVM>();

                while (worksheet.Cells[iRow, 1].Value != null)
                {
                    var FlowChart_Detail_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);
                    var Flowchart_Detail_ME_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    var FatherUID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);
                    var ChildUID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);
                    var Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 8].Value);
                    var ProductDate = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 9].Text);
                    var opQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 10].Value);
                    var monQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 11].Value);
                    var technicalQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 12].Value);
                    var materialQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 13].Value);
                    var otherQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 14].Value);

                    if (string.IsNullOrEmpty(FlowChart_Detail_UID))
                    {
                        errorInfo = string.Format( "第{0}行D列主键不能为空",iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(Flowchart_Detail_ME_UID))
                    {
                        errorInfo = string.Format("第{0}行E列主键不能为空", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(Product_Phase))
                    {
                        errorInfo = string.Format("第{0}行阶段不能为空", iRow);
                        return errorInfo;
                    }
                    else if (Product_Phase != "MP" && Product_Phase != "NPI")
                    {
                        errorInfo = string.Format("第{0}行阶段只能为MP或NPI", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(ProductDate))
                    {
                        errorInfo = string.Format("第{0}行生产日期不能为空", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(opQty))
                    {
                        errorInfo = string.Format("第{0}行OP人力不能为空", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(monQty))
                    {
                        errorInfo = string.Format("第{0}行班长不能为空", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(technicalQty))
                    {
                        errorInfo = string.Format("第{0}行技术员不能为空", iRow);
                        return errorInfo;
                    }


                    if (string.IsNullOrEmpty(materialQty))
                    {
                        errorInfo = string.Format("第{0}行物料员不能为空", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(otherQty))
                    {
                        errorInfo = string.Format("第{0}行其他人力不能为空", iRow);
                        return errorInfo;
                    }


                    int validInt = 0;
                    var isInt = int.TryParse(opQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行OP人力必须为整数", iRow);
                        return errorInfo;
                    }


                    isInt = int.TryParse(monQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行班长必须为整数", iRow);
                        return errorInfo;
                    }

                    isInt = int.TryParse(technicalQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行技术员必须为整数", iRow);
                        return errorInfo;
                    }

                    isInt = int.TryParse(materialQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行物料员必须为整数", iRow);
                        return errorInfo;
                    }

                    isInt = int.TryParse(otherQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行其他人力必须为整数", iRow);
                        return errorInfo;
                    }

                    ActiveManPowerVM item = new ActiveManPowerVM();
                    item.FlowChart_Detail_UID = Convert.ToInt32(FlowChart_Detail_UID);
                    item.Flowchart_Detail_ME_UID = Convert.ToInt32(Flowchart_Detail_ME_UID);
                    if (!string.IsNullOrEmpty(FatherUID))
                    {
                        item.Father_UID = Convert.ToInt32(FatherUID);
                    }
                    if (!string.IsNullOrEmpty(ChildUID))
                    {
                        item.Child_UID = Convert.ToInt32(ChildUID);
                    }
                    item.Product_Phase = Product_Phase;
                    item.ProductDate = Convert.ToDateTime(ProductDate);
                    item.OP_Qty = Convert.ToInt32(opQty);
                    item.Monitor_Staff_Qty = Convert.ToInt32(monQty);
                    item.Technical_Staff_Qty = Convert.ToInt32(technicalQty);
                    item.Material_Keeper_Qty = Convert.ToInt32(materialQty);
                    item.Others_Qty = Convert.ToInt32(otherQty);
                    item.Created_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Created_Date = DateTime.Now;
                    item.Modified_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Modified_Date = DateTime.Now;
                    item.LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                    list.Add(item);
                    iRow++;
                }

                int totalRows = iRow - 2;
                if (list.Distinct().Count() != totalRows)
                {
                    errorInfo = "导入的Excel有重复行";
                    return errorInfo;
                }

                string json = JsonConvert.SerializeObject(list);


                if (string.IsNullOrEmpty(errorInfo))
                {
                    //检查导入的数据是否存在
                    string api = string.Format("PPFlowchart/CheckImportManPowerAPI");
                    HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(json, api);
                    var checkResult = checkResponMessage.Content.ReadAsStringAsync().Result;
                    checkResult = checkResult.Replace("\"", "");
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        errorInfo = checkResult;
                        return errorInfo;
                    }
                }

                var apiUrl = string.Format("PPFlowchart/ImportManPowerAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;


            }
            return errorInfo;
        }


        public ActionResult SaveActualPowerInfo(ProductRequestStaffDTO dto, int id, int Version)
        {
            dto.Created_UID = this.CurrentUser.AccountUId;
            dto.Modified_UID = this.CurrentUser.AccountUId;
            var json = JsonConvert.SerializeObject(dto);
            var api = string.Format("PPFlowchart/SaveActualPowerInfoAPI");
            HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(dto, api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return RedirectToAction("ActualPowerList", "PPFlowchart", new { id = id, Version = Version });
        }

        #endregion

        #region 实际机台数据录入
        public ActionResult ActualEquipList(int id, int Version)
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

            return View();
        }


        public ActionResult ActualEquipInfo(ActiveManPowerSearchVM vm, Page page)
        {
            var api = string.Format("PPFlowchart/EquipInfoAPI");
            HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(vm, page, api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string CheckDownloadEquipExcel(int id, int Version)
        {
            var api = string.Format("PPFlowchart/CheckDownloadManPowerExcelAPI?id={0}&Version={1}", id, Version);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result.Replace("\"", ""); ;
            return result;
        }

        public FileResult DownloadEquipExcel(int id, int Version)
        {

            var api = string.Format("PPFlowchart/GetEquipDownLoadInfoAPI?id={0}&Version={1}", id, Version);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<ActiveEquipVM>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("实际机台生产计划维护");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetEquipHeadColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("实际机台生产计划维护");
                SetEquipExcelStyle(worksheet, propertiesHead);
                int iRow = 2;

                foreach (var item in list)
                {
                    worksheet.Cells[iRow, 1].Value = item.Process_Seq;
                    worksheet.Cells[iRow, 2].Value = item.FunPlant;
                    worksheet.Cells[iRow, 3].Value = item.Process;
                    worksheet.Cells[iRow, 4].Value = item.Equipment_Name;
                    worksheet.Cells[iRow, 5].Value = item.FlowChart_Detail_UID;
                    worksheet.Cells[iRow, 6].Value = item.Flowchart_Detail_ME_UID;
                    worksheet.Cells[iRow, 7].Value = item.Flowchart_Detail_ME_Equipment_UID;
                    worksheet.Cells[iRow, 8].Value = item.Father_UID;
                    worksheet.Cells[iRow, 9].Value = item.Child_UID;
                    iRow++;
                }
                //Excel最后一行行号
                int maxRow = iRow - 1;
                //设置灰色背景
                var colorRange = string.Format("A2:I{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置Excel日期格式yyyy-MM-dd
                var dateColorRange = string.Format("K2:K{0}", maxRow);
                worksheet.Cells[dateColorRange].Style.Numberformat.Format = FormatConstants.DateTimeFormatStringByDate;
                //设置主键列隐藏
                worksheet.Column(5).Hidden = true;
                worksheet.Column(6).Hidden = true;
                worksheet.Column(7).Hidden = true;
                worksheet.Column(8).Hidden = true;
                worksheet.Column(9).Hidden = true;
                //设置边框
                worksheet.Cells[string.Format("A1:L{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:L{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:L{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();


                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }

        private void SetEquipExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            //int j = 1;
            for (int i = 0; i < propertiesHead.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = propertiesHead[i];
                //设置列宽
                if (i != 3)
                {
                    worksheet.Column(i + 1).Width = 15;
                }
                else
                {
                    worksheet.Column(i + 1).Width = 50;

                }
            }

            worksheet.Cells["A1:L1"].Style.Font.Bold = true;
            worksheet.Cells["A1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:L1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }

        public string ImportEquip(HttpPostedFileBase upload_excel, int id, int Version)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null || worksheet.Name != "实际机台生产计划维护")
                {
                    errorInfo = "没有worksheet内容";
                    return errorInfo;
                }

                int iRow = 2;
                var propertiesContent = FlowchartImportCommon.GetEquipHeadColumn();
                List<ProductEquipmentQTYDTO> list = new List<ProductEquipmentQTYDTO>();

                while (worksheet.Cells[iRow, 1].Value != null)
                {
                    var FlowChart_Detail_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    var Flowchart_Detail_ME_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);
                    var Flowchart_Detail_ME_Equipment_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);
                    var Father_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 8].Value);
                    var Child_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 9].Value);
                    var Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 10].Text);
                    var ProductDate = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 11].Text);
                    var Qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 12].Value);

                    if (string.IsNullOrEmpty(FlowChart_Detail_UID))
                    {
                        errorInfo = string.Format( "第{0}行E列主键不能为空",iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(Flowchart_Detail_ME_UID))
                    {
                        errorInfo = string.Format("第{0}行F列主键不能为空", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(Product_Phase))
                    {
                        errorInfo = string.Format("第{0}行H列阶段不能为空", iRow);
                        return errorInfo;
                    }
                    else if (Product_Phase != "MP" && Product_Phase != "NPI")
                    {
                        errorInfo = string.Format("第{0}行H列阶段只能为MP或NPI", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(ProductDate))
                    {
                        errorInfo = string.Format("第{0}行生产日期不能为空", iRow);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(Qty))
                    {
                        errorInfo = string.Format("第{0}行设备数量不能为空", iRow);
                        return errorInfo;
                    }


                    int validInt = 0;
                    var isInt = int.TryParse(Qty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行设备数量必须为整数", iRow);
                        return errorInfo;
                    }


                    ProductEquipmentQTYDTO item = new ProductEquipmentQTYDTO();
                    item.FlowChart_Detail_UID = Convert.ToInt32(FlowChart_Detail_UID);
                    item.Flowchart_Detail_ME_UID = Convert.ToInt32(Flowchart_Detail_ME_UID);
                    if (!string.IsNullOrEmpty(Flowchart_Detail_ME_Equipment_UID))
                    {
                        item.Flowchart_Detail_ME_Equipment_UID = Convert.ToInt32(Flowchart_Detail_ME_Equipment_UID);
                    }
                    if (!string.IsNullOrEmpty(Father_UID))
                    {
                        item.Father_UID = Convert.ToInt32(Father_UID);
                    }
                    if (!string.IsNullOrEmpty(Child_UID))
                    {
                        item.Child_UID = Convert.ToInt32(Child_UID);
                    }
                    item.Product_Phase = Product_Phase;
                    item.ProductDate = Convert.ToDateTime(ProductDate);
                    item.Qty = Convert.ToInt32(Qty);
                    item.Created_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Created_Date = DateTime.Now;
                    item.Modified_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Modified_Date = DateTime.Now;
                    item.LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                    list.Add(item);
                    iRow++;
                }

                int totalRows = iRow - 2;
                if (list.Distinct().Count() != totalRows)
                {
                    errorInfo = "导入的Excel有重复行";
                    return errorInfo;
                }

                string json = JsonConvert.SerializeObject(list);


                if (string.IsNullOrEmpty(errorInfo))
                {
                    //检查导入的数据是否存在
                    string api = string.Format("PPFlowchart/CheckImportEquipAPI");
                    HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(json, api);
                    var checkResult = checkResponMessage.Content.ReadAsStringAsync().Result;
                    checkResult = checkResult.Replace("\"", "");
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        errorInfo = checkResult;
                        return errorInfo;
                    }
                }

                var apiUrl = string.Format("PPFlowchart/ImportEquipAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;


            }
            return errorInfo;
        }


        public ActionResult SaveActualEquipInfo(ProductEquipmentQTYDTO dto, int id, int Version)
        {
            dto.Created_UID = this.CurrentUser.AccountUId;
            dto.Modified_UID = this.CurrentUser.AccountUId;
            var json = JsonConvert.SerializeObject(dto);
            var api = string.Format("PPFlowchart/SaveActualEquipInfoAPI");
            HttpResponseMessage responddlMessage = APIHelper.APIPostAsync(dto, api);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            return RedirectToAction("ActualEquipList", "PPFlowchart", new { id = id, Version = Version });

        }
        



        #endregion


    }
}