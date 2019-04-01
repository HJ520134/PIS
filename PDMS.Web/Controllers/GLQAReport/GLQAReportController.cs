using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class GLQAReportController : WebControllerBase
    {

        /// <summary>
        /// 获取当前登陆人的厂区ID
        /// </summary>
        /// <returns></returns>
        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }
        #region QA检测点维护
        public ActionResult GLQAStation(int StationID)
        {

            string apiUrl = "GoldenLine/GetStationByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", StationID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_StationDTO dto = JsonConvert.DeserializeObject<GL_StationDTO>(result);
            ViewBag.StationID = StationID;
            return View(dto);
        }
        public ActionResult GetGLQADetectionPoint(Page page, int StationID)
        {
            string apiUrl = "IPQCQualityReport/GetGLQADetectionPointAPI";
            GLQAReportVM vm = new GLQAReportVM();
            vm.StationID = StationID;
            HttpResponseMessage response = APIHelper.APIPostAsync(vm, page, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }

        public string AddOrEditGL_QADetectionPoint(GL_QADetectionPointDTO dto)
        {

            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = string.Format("IPQCQualityReport/AddOrEditGL_QADetectionPointAPI");
            HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        [HttpPost]
        public ActionResult GetGLQADetectionPointByID(int QADetectionPointID)
        {
            var apiUrl = string.Format("IPQCQualityReport/GetGLQADetectionPointByIDAPI?QADetectionPointID={0}", QADetectionPointID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        [HttpPost]
        public ActionResult RemoveGLQADetectionPointByID(int QADetectionPointID)
        {
            string apiUrl = "IPQCQualityReport/RemoveGLQADetectionPointByIDAPI";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{QADetectionPointID:{0}}}", QADetectionPointID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        [HttpPost]
        public ActionResult GetStationsDetectionPointByID(int StationID)
        {
            var apiUrl = string.Format("IPQCQualityReport/GetStationsDetectionPointByIDAPI?StationID={0}", StationID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }

        private string[] GetIPQCStationHeadColumn()
        {

            var propertiesHead = new[]
            {
                "专案",
                "线别",
                "工站",
                "WIP",
                "ScanIN_StationName",
                "ScanOUT_StationName",
                "ScanNG_StationName",
                "ScanBACK_StationName",
                "是否启用",
                "StationID"

            };
            return propertiesHead;
        }

        private void SetIPQCStationHeadExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];
            worksheet.Cells[1, 7].Value = propertiesHead[6];
            worksheet.Cells[1, 8].Value = propertiesHead[7];
            worksheet.Cells[1, 9].Value = propertiesHead[8];
            worksheet.Cells[1, 10].Value = propertiesHead[9];
            //设置列宽
            for (int i = 1; i <= 9; i++)
            {

                worksheet.Column(i).Width = 23;

            }
            worksheet.Column(10).Hidden = true;
            worksheet.Cells["A1:I1"].Style.Font.Bold = true;
            worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }
        public FileResult DownloadIPQCStationExcel(int id)
        {

            string[] propertiesHead = new string[] { };
            propertiesHead = GetIPQCStationHeadColumn();
            string apiUrl = string.Format("IPQCQualityReport/GetQADetectionPointDTOListAPI?LineID={0}", id);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GL_QADetectionPointDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("IPQC检测点");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("IPQC检测点");
                    SetIPQCStationHeadExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.ProjectName;
                        worksheet.Cells[iRow, 2].Value = item.LineName;
                        worksheet.Cells[iRow, 3].Value = item.StationName;

                        worksheet.Cells[iRow, 10].Value = item.StationID;

                        iRow++;
                    }

                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置列隐藏
                    worksheet.Column(10).Hidden = true;

                    //设置灰色背景
                    var colorRange = string.Format("A2:C{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
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

        public string UploadDetectionPointFile(HttpPostedFileBase uploadfile, int LineID)
        {
            string errorInfo = string.Empty;
            try
            {

                using (var xlsPackage = new OfficeOpenXml.ExcelPackage(uploadfile.InputStream))
                {

                    var worksheet = xlsPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalColumns = worksheet.Dimension.End.Column;
                    if (worksheet == null)
                    {
                        errorInfo = "没有worksheet内容";

                        return errorInfo;
                    }
                    var propertiesHead = GetIPQCStationHeadColumn();

                    bool isExcelError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[1, i].Value != null && !string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Value.ToString()))
                        {
                            var result = worksheet.Cells[1, i].Value.ToString();
                            var hasItem = propertiesHead.FirstOrDefault(m => m.Contains(result));
                            if (hasItem == null)
                            {
                                isExcelError = true;
                                break;
                            }
                        }
                        else
                        {
                            isExcelError = true;
                            break;
                        }
                    }
                    if (isExcelError)
                    {
                        errorInfo = "Excel格式不正确";
                        return errorInfo;
                    }

                    var apiUrl = string.Format("IPQCQualityReport/GetGL_QADetectionPointDTOAPI?LineId={0}", LineID);
                    HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                    var StationDTOresult = responMessage.Content.ReadAsStringAsync().Result;
                    List<GL_QADetectionPointDTO> gLQADetectionPointDTOs = JsonConvert.DeserializeObject<List<GL_QADetectionPointDTO>>(StationDTOresult);
                    List<GL_QADetectionPointDTO> newgLQADetectionPointDTOs = new List<GL_QADetectionPointDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        GL_QADetectionPointDTO entity = new GL_QADetectionPointDTO();
                        var ProjectName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案")].Value);
                        var LineName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "线别")].Value);
                        var StationName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站")].Value);
                        var WIP = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "WIP")].Value);
                        var ScanIN_StationName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "ScanIN_StationName")].Value);
                        var ScanOUT_StationName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "ScanOUT_StationName")].Value);
                        var ScanNG_StationName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "ScanNG_StationName")].Value);
                        var ScanBACK_StationName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "ScanBACK_StationName")].Value);
                        var isEnabled = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        var StationID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "StationID")].Value);
                        if (string.IsNullOrWhiteSpace(ProjectName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案没有值", i);
                            return errorInfo;
                        }
                        else if (ProjectName.Length > 30)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案长度超过最大限定[30]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(LineName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行线别没有值", i);
                            return errorInfo;
                        }
                        else if (LineName.Length > 50)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行线别长度超过最大限定[50]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(StationName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站没有值", i);
                            return errorInfo;
                        }
                        else if (StationName.Length > 50)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站长度超过最大限定[50]", i);
                            return errorInfo;
                        }


                        if (string.IsNullOrWhiteSpace(ScanIN_StationName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanIN_StationName没有值", i);
                            return errorInfo;
                        }
                        else if (ScanIN_StationName.Length > 100)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanIN_StationName长度超过最大限定[100]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(ScanOUT_StationName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanOUT_StationName没有值", i);
                            return errorInfo;
                        }
                        else if (ScanOUT_StationName.Length > 100)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanOUT_StationName长度超过最大限定[100]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(ScanNG_StationName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanNG_StationName没有值", i);
                            return errorInfo;
                        }
                        else if (ScanNG_StationName.Length > 100)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanNG_StationName长度超过最大限定[100]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(ScanBACK_StationName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanBACK_StationName没有值", i);
                            return errorInfo;
                        }
                        else if (ScanBACK_StationName.Length > 100)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行ScanBACK_StationName长度超过最大限定[100]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(isEnabled))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isEnabled = isEnabled.Trim();
                            if (isEnabled != "Y" && isEnabled != "N")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写N(禁用)或Y(启用)", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(StationID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行数据错误", i);
                            return errorInfo;
                        }
                        int stationID = 0;
                        if (!int.TryParse(StationID, out stationID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行数据类型错误", i);
                            return errorInfo;
                        }
                        if (string.IsNullOrWhiteSpace(WIP))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行WIP没有值", i);
                            return errorInfo;
                        }
                        int wip = 0;
                        if (!int.TryParse(WIP, out wip))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行WIP只能是数字", i);
                            return errorInfo;
                        }
                        entity.ProjectName = ProjectName;
                        entity.LineName = LineName;
                        entity.StationName = StationName;
                        entity.ScanIN = ScanIN_StationName;
                        entity.ScanOUT = ScanOUT_StationName;
                        entity.ScanNG = ScanNG_StationName;
                        entity.ScanBACK = ScanBACK_StationName;
                        entity.IsEnabled = isEnabled == "Y" ? true : false;
                        entity.StationID = Convert.ToInt32(StationID);
                        entity.WIP = Convert.ToInt32(WIP);
                        entity.Modified_UID = CurrentUser.AccountUId;
                        entity.Modified_Date = DateTime.Now;
                        var gLQADetectionPointDTO = gLQADetectionPointDTOs.FirstOrDefault(o => o.StationID == entity.StationID);
                        if (gLQADetectionPointDTO != null)
                        {
                            entity.QADetectionPointID = gLQADetectionPointDTO.QADetectionPointID;
                        }

                        //导入数据判重
                        var isSelfRepeated = newgLQADetectionPointDTOs.Exists(m => m.StationID == entity.StationID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行[{1}]在导入数据中重复", i, StationName);
                            return errorInfo;
                        }

                        //导入数据判重
                        var isSelfRepeatedOne = newgLQADetectionPointDTOs.Exists(m => m.ProjectName == ProjectName && m.LineName == LineName && m.StationName == StationName);
                        if (isSelfRepeatedOne)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行[{1}]在导入数据中重复", i, StationName);
                            return errorInfo;
                        }
                        newgLQADetectionPointDTOs.Add(entity);

                    }

                    //插入表
                    var json = JsonConvert.SerializeObject(newgLQADetectionPointDTOs);
                    var apiInsertVendorInfoUrl = string.Format("IPQCQualityReport/InserOrUpdateDetectionPointsAPI");
                    HttpResponseMessage responMessageStation = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessageStation.Content.ReadAsStringAsync().Result.Replace("\"", "");

                }

            }
            catch (Exception ex)
            {

            }

            return errorInfo;

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


        #endregion
        #region QA目标良率维护
        public ActionResult GLQAYield(int StationID)
        {

            string apiUrl = "GoldenLine/GetStationByID";
            HttpResponseMessage response = APIHelper.APIPostAsync(string.Format("{{id:{0}}}", StationID), apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            GL_StationDTO dto = JsonConvert.DeserializeObject<GL_StationDTO>(result);
            ViewBag.StationID = StationID;
            return View(dto);
        }
        public ActionResult QueryGLQAYields(GL_QATargetYieldDTO search, Page page)
        {
            var apiUrl = string.Format("IPQCQualityReport/QueryGLQAYieldsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string AddOrEditGLQAYield(GL_QATargetYieldDTO dto)
        {

            if (dto.Tag == 0)
            {
                return "请选择一次/二次目标良率";
            }
            if (dto.TargetYieldDate == null || dto.TargetYieldDate == "")
            {
                return "请选择目标良率日期";
            }
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = string.Format("IPQCQualityReport/AddOrEditGLQAYieldAPI");
            HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        [HttpPost]
        public ActionResult QueryGLQAYieldByUID(int GLQATargetYieldID)
        {
            var apiUrl = string.Format("IPQCQualityReport/QueryGLQAYieldByUIDAPI?GLQATargetYieldID={0}", GLQATargetYieldID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        private string[] GetGLQAYieldHeadColumn()
        {

            var propertiesHead = new[]
            {
                "专案",
                "线别",
                "工站",
                "一次/二次目标良率",
                "目标良率日期",
                "目标良率"
            };
            return propertiesHead;
        }
        private void SetGLQAYieldHeadExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];


            //设置列宽
            for (int i = 1; i <= 6; i++)
            {

                worksheet.Column(i).Width = 23;

            }

            worksheet.Cells["A1:F1"].Style.Font.Bold = true;
            worksheet.Cells["A1:F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:F1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }
        public FileResult DownloadGLQAYieldExcel(int id)
        {
            string[] propertiesHead = new string[] { };
            propertiesHead = GetGLQAYieldHeadColumn();
            string apiUrl = string.Format("IPQCQualityReport/GetGL_QATargetYieldDTOListAPI?StationID={0}", id);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<GL_QATargetYieldDTO>>(result);

            if (list.Count() > 0)
            {

                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("目标良率");

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("目标良率");
                    SetGLQAYieldHeadExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.ProjectName;
                        worksheet.Cells[iRow, 2].Value = item.LineName;
                        worksheet.Cells[iRow, 3].Value = item.StationName;
                        iRow++;
                    }
                    //Excel最后一行行号
                    int maxRow = iRow - 1;

                    //设置灰色背景
                    var colorRange = string.Format("A2:C{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    //设置边框
                    worksheet.Cells[string.Format("A1:F{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:F{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:F{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:F{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
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

        public string UploadGLQAYieldFile(HttpPostedFileBase uploadfile, int StationID)
        {
            string errorInfo = string.Empty;
            try
            {

                using (var xlsPackage = new OfficeOpenXml.ExcelPackage(uploadfile.InputStream))
                {

                    var worksheet = xlsPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalColumns = worksheet.Dimension.End.Column;
                    if (worksheet == null)
                    {
                        errorInfo = "没有worksheet内容";

                        return errorInfo;
                    }
                    var propertiesHead = GetGLQAYieldHeadColumn();

                    bool isExcelError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[1, i].Value != null && !string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Value.ToString()))
                        {
                            var result = worksheet.Cells[1, i].Value.ToString();
                            var hasItem = propertiesHead.FirstOrDefault(m => m.Contains(result));
                            if (hasItem == null)
                            {
                                isExcelError = true;
                                break;
                            }
                        }
                        else
                        {
                            isExcelError = true;
                            break;
                        }
                    }
                    if (isExcelError)
                    {
                        errorInfo = "Excel格式不正确";
                        return errorInfo;
                    }
                    //获取专案下所有点IPQC站点
                    string apiUrlallStation = string.Format("IPQCQualityReport/GetGL_QATargetYieldDTOListAPI?StationID={0}", StationID);
                    HttpResponseMessage responMessageallStation = APIHelper.APIGetAsync(apiUrlallStation);
                    var resultallStation = responMessageallStation.Content.ReadAsStringAsync().Result;
                    var allStationlist = JsonConvert.DeserializeObject<List<GL_QATargetYieldDTO>>(resultallStation);

                    //获取数据库已存在的目标良率
                    var apiUrl = string.Format("IPQCQualityReport/GetGL_QATargetYieldDTOsAPI?StationID={0}", StationID);
                    HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                    var StationDTOresult = responMessage.Content.ReadAsStringAsync().Result;
                    List<GL_QATargetYieldDTO> glQATargetYieldDTOs = JsonConvert.DeserializeObject<List<GL_QATargetYieldDTO>>(StationDTOresult);
                    List<GL_QATargetYieldDTO> newglQATargetYieldDTOs = new List<GL_QATargetYieldDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        GL_QATargetYieldDTO entity = new GL_QATargetYieldDTO();
                        var ProjectName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案")].Value);
                        var LineName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "线别")].Value);
                        var StationName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站")].Value);
                        var TargetYieldType = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "一次/二次目标良率")].Value);
                        var TargetYieldDate = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "目标良率日期")].Value);
                        var TargetYield = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "目标良率")].Value);

                        if (string.IsNullOrWhiteSpace(ProjectName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案没有值", i);
                            return errorInfo;
                        }
                        else if (ProjectName.Length > 30)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案长度超过最大限定[30]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(LineName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行线别没有值", i);
                            return errorInfo;
                        }
                        else if (LineName.Length > 50)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行线别长度超过最大限定[50]", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(StationName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站没有值", i);
                            return errorInfo;
                        }
                        else if (StationName.Length > 50)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站长度超过最大限定[50]", i);
                            return errorInfo;
                        }

                        var StationDTO = allStationlist.FirstOrDefault(o => o.ProjectName == ProjectName && o.LineName == LineName && o.StationName == StationName);
                        if (StationDTO != null)
                        {
                            entity.ProjectName = ProjectName;
                            entity.LineName = LineName;
                            entity.StationName = StationName;
                            entity.StationID = StationDTO.StationID;
                            entity.LineID = StationDTO.LineID;
                            entity.CustomerID = StationDTO.CustomerID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案，线别，工站所匹配的数据没有找到.", i);
                            return errorInfo;
                        }

                        int intTargetYieldType = 0;
                        if (!int.TryParse(TargetYieldType, out intTargetYieldType))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行数据类型错误，1：代表一次目标良率，2：代表二次目标良率。", i);
                            return errorInfo;
                        }

                        if (intTargetYieldType == 1 || intTargetYieldType == 2)
                        {
                            entity.Tag = intTargetYieldType;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行数型错误，1：代表一次目标良率，2：代表二次目标良率。", i);
                            return errorInfo;
                        }

                        DateTime targetYieldDate;
                        if (!DateTime.TryParse(TargetYieldDate, out targetYieldDate))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行数据类型错误，格式例如:2018-10-11。", i);
                            return errorInfo;
                        }
                        else
                        {
                            entity.TargetYieldDate = targetYieldDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                        }
                        decimal targetYield = 0;
                        if (!decimal.TryParse(TargetYield, out targetYield))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行数据类型错误，格式例如:0.99。", i);
                            return errorInfo;
                        }
                        else
                        {
                            entity.TargetYield = targetYield;
                        }

                        var glQATargetYieldDTO = glQATargetYieldDTOs.FirstOrDefault(o => o.StationID == entity.StationID && o.Tag == entity.Tag && o.TargetYieldDate == entity.TargetYieldDate);
                        if (glQATargetYieldDTO != null)
                        {
                            entity.GLQATargetYieldID = glQATargetYieldDTO.GLQATargetYieldID;
                        }

                        entity.Modified_UID = CurrentUser.AccountUId;
                        entity.Modified_Date = DateTime.Now;
                        //导入数据判重
                        var isSelfRepeated = newglQATargetYieldDTOs.Exists(m => m.StationID == entity.StationID && m.Tag == entity.Tag && m.TargetYieldDate == entity.TargetYieldDate);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行站点：[{1}],日期：[{2}]在导入数据中重复", i, StationName, TargetYieldDate);
                            return errorInfo;
                        }
                        newglQATargetYieldDTOs.Add(entity);

                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(newglQATargetYieldDTOs);
                    var apiInsertVendorInfoUrl = string.Format("IPQCQualityReport/InserOrUpdateTargetYieldsAPI");
                    HttpResponseMessage responMessageStation = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessageStation.Content.ReadAsStringAsync().Result.Replace("\"", "");

                }

            }
            catch (Exception ex)
            {

            }

            return errorInfo;

        }

        public string DeleteGLQAYield(int GLQATargetYieldID)
        {
            var apiUrl = string.Format("IPQCQualityReport/DeleteGLQAYieldAPI?GLQATargetYieldID={0}", GLQATargetYieldID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        }
        #endregion
        #region  IPQC良率看板

        /// <summary>
        /// 时段报表 Init
        /// </summary>
        /// <returns></returns>
        public ActionResult IPQCQualityReport(string Plant_UID, string BG_UID, string Fun_Plant, string customerId, string lineName, string stationName, string MachineName, string ShiftID, string startTime, string endTime, string isFromPhotoReport)
        {
            Fixture_PartVM currentVM = new Fixture_PartVM();
            int optypeID = 0;
            int funPlantID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
                {
                    optypeID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }

                if (CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null)
                {
                    funPlantID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                }
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {
                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }

            var OEE_PieMy_Cookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.IPQCQualityReport);
            if (OEE_PieMy_Cookie != null)
            {
                Plant_UID = OEE_PieMy_Cookie["Plant_UID"];
                BG_UID = OEE_PieMy_Cookie["BG_UID"];
                Fun_Plant = OEE_PieMy_Cookie["Fun_Plant"];
                customerId = OEE_PieMy_Cookie["customerId"];
                lineName = OEE_PieMy_Cookie["lineName"];
                stationName = OEE_PieMy_Cookie["stationName"];
                ShiftID = OEE_PieMy_Cookie["ShiftID"];
                startTime = OEE_PieMy_Cookie["startTime"];
                endTime = OEE_PieMy_Cookie["ProductDate"];
                isFromPhotoReport = "true";
            }

            ViewBag.Plant_UID = Plant_UID;
            ViewBag.BG_UID = BG_UID;
            ViewBag.Fun_Plant = Fun_Plant;
            ViewBag.customerId = customerId;
            ViewBag.lineName = lineName;
            ViewBag.stationName = stationName;
            ViewBag.ShiftID = ShiftID;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.isFromPhotoReport = isFromPhotoReport;
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("IPQCQualityReport", currentVM);
        }

        public ActionResult GetIpqcQualityReport(IPQCQualityReportVM paramModel, Page page)
        {
            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.IPQCQualityReport);
            if (cooike != null)
            {
                cooike["Plant_UID"] = paramModel.Plant_Organization_UID.ToString();
                cooike["BG_UID"] = paramModel.BG_Organization_UID.ToString();
                cooike["Fun_Plant"] = paramModel.FunPlant_Organization_UID.ToString();
                cooike["customerId"] = paramModel.CustomerID.ToString();
                cooike["lineName"] = paramModel.LineID.ToString();
                cooike["stationName"] = paramModel.StationID.ToString();
                cooike["ShiftID"] = paramModel.ShiftTimeID.ToString();
                cooike["startTime"] = paramModel.ProductDate.AddDays(-14).ToString("yyyy-MM-dd");
                cooike["endTime"] = paramModel.ProductDate.ToString("yyyy-MM-dd");
                cooike["ProductDate"] = paramModel.ProductDate.ToString("yyyy-MM-dd");
                cooike["isFromPhotoReport"] = "false";
                cooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(cooike);
            }
            else
            {
                var OEE_Cookie = new HttpCookie(SessionConstants.IPQCQualityReport);
                OEE_Cookie["Plant_UID"] = paramModel.Plant_Organization_UID.ToString();
                OEE_Cookie["BG_UID"] = paramModel.BG_Organization_UID.ToString();
                OEE_Cookie["Fun_Plant"] = paramModel.FunPlant_Organization_UID.ToString();
                OEE_Cookie["customerId"] = paramModel.CustomerID.ToString();
                OEE_Cookie["lineName"] = paramModel.LineID.ToString();
                OEE_Cookie["stationName"] = paramModel.StationID.ToString();
                OEE_Cookie["ShiftID"] = paramModel.ShiftTimeID.ToString();
                OEE_Cookie["startTime"] = paramModel.ProductDate.AddDays(-14).ToString("yyyy-MM-dd");
                OEE_Cookie["endTime"] = paramModel.ProductDate.ToString("yyyy-MM-dd");
                OEE_Cookie["ProductDate"] = paramModel.ProductDate.ToString("yyyy-MM-dd");
                OEE_Cookie["isFromPhotoReport"] = "false";
                OEE_Cookie.Expires.AddDays(30);
                HttpContext.Response.SetCookie(OEE_Cookie);
            }

            string apiUrl = "IPQCQualityReport/GetIpqcQualityReportAPI";
            HttpResponseMessage response = APIHelper.APIPostAsync(paramModel, page, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetQualityTopDetial(string stationid, string shiftid, string productdate, string timeInterval, string NgType)
        {
            IPQCQualityReportVM paramModel = new IPQCQualityReportVM();
            paramModel.StationID = int.Parse(stationid);
            paramModel.ShiftTimeID = int.Parse(shiftid);
            paramModel.ProductDate = Convert.ToDateTime(productdate);
            paramModel.TimeInterval = timeInterval;
            paramModel.VersionNumber = int.Parse(NgType);
            string apiUrl = "IPQCQualityReport/GetQualityTopDetialAPI";
            HttpResponseMessage response = APIHelper.APIPostAsync(paramModel, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string GetShiftTimeHead(int ShiftTimeID)
        {
            var apiUrl = string.Format("GoldenLine/GetShiftTimeAPI?ShiftTimeID={0}", ShiftTimeID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<GL_ShiftTimeDTO>(result);
            var currentDate = DateTime.Now;
            var startTime = Convert.ToDateTime(currentDate.ToString("yyyy-MM-dd") + " " + model.StartTime);
            var endTime = Convert.ToDateTime(currentDate.ToString("yyyy-MM-dd") + " " + model.End_Time);
            if (startTime > endTime)
            {
                endTime = endTime.AddDays(1);
            }

            int hours = Math.Abs(endTime.Hour - startTime.Hour) / 2;
            List<string> shiftHour = new List<string>();
            for (int i = 0; i < hours; i++)
            {
                shiftHour.Add(startTime.ToString("HH:mm") + '-' + startTime.AddHours(2).ToString("HH:mm"));
                startTime = startTime.AddHours(2);
            }

            return JsonConvert.SerializeObject(shiftHour);
        }

        /// <summary>
        /// 月报表 Init
        /// </summary>
        /// <returns></returns>
        public ActionResult IPQCQualityMonthReport(string Plant_UID, string BG_UID, string Fun_Plant, string customerId, string lineName, string stationName, string MachineName, string ShiftID, string startTime, string endTime, string isFromPhotoReport)
        {
            Fixture_PartVM currentVM = new Fixture_PartVM();
            int optypeID = 0;
            int funPlantID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
                {
                    optypeID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }

                if (CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null)
                {
                    funPlantID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                }
                currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {
                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                startTime = Convert.ToDateTime(endTime).AddDays(-14).ToString("yyyy-MM-dd");
            }
            var OEE_PieMy_Cookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.IPQCQualityReport);
            if (OEE_PieMy_Cookie != null)
            {
                Plant_UID = OEE_PieMy_Cookie["Plant_UID"];
                BG_UID = OEE_PieMy_Cookie["BG_UID"];
                Fun_Plant = OEE_PieMy_Cookie["Fun_Plant"];
                customerId = OEE_PieMy_Cookie["customerId"];
                lineName = OEE_PieMy_Cookie["lineName"];
                stationName = OEE_PieMy_Cookie["stationName"];
                ShiftID = OEE_PieMy_Cookie["ShiftID"];
                startTime = OEE_PieMy_Cookie["startTime"];
                endTime = OEE_PieMy_Cookie["endTime"];
                isFromPhotoReport = "true";
            }

            ViewBag.Plant_UID = Plant_UID;
            ViewBag.BG_UID = BG_UID;
            ViewBag.Fun_Plant = Fun_Plant;
            ViewBag.customerId = customerId;
            ViewBag.lineName = lineName;
            ViewBag.stationName = stationName;
            ViewBag.ShiftID = ShiftID;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.isFromPhotoReport = isFromPhotoReport;

            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("IPQCQualityMonthReport", currentVM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetIPQCQualityMonthReport(string paramModel, string NgType, Page page)
        {
            var model = JsonConvert.DeserializeObject<IPQCQualityReportVM>(paramModel);
            model.VersionNumber = int.Parse(NgType);
            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.IPQCQualityReport);
            if (cooike != null)
            {
                cooike["Plant_UID"] = model.Plant_Organization_UID.ToString();
                cooike["BG_UID"] = model.BG_Organization_UID.ToString();
                cooike["Fun_Plant"] = model.FunPlant_Organization_UID.ToString();
                cooike["customerId"] = model.CustomerID.ToString();
                cooike["lineName"] = model.LineID.ToString();
                cooike["stationName"] = model.StationID.ToString();
                cooike["ShiftID"] = model.ShiftTimeID.ToString();
                cooike["startTime"] = model.StartTime.ToString("yyyy-MM-dd");
                cooike["endTime"] = model.EndTime.ToString("yyyy-MM-dd");
                cooike["ProductDate"] = model.EndTime.ToString("yyyy-MM-dd");
                cooike["isFromPhotoReport"] = "false";
                cooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(cooike);
            }
            else
            {
                var OEE_Cookie = new HttpCookie(SessionConstants.IPQCQualityReport);
                OEE_Cookie["Plant_UID"] = model.Plant_Organization_UID.ToString();
                OEE_Cookie["BG_UID"] = model.BG_Organization_UID.ToString();
                OEE_Cookie["Fun_Plant"] = model.FunPlant_Organization_UID.ToString();
                OEE_Cookie["customerId"] = model.CustomerID.ToString();
                OEE_Cookie["lineName"] = model.LineID.ToString();
                OEE_Cookie["stationName"] = model.StationID.ToString();
                OEE_Cookie["ShiftID"] = model.ShiftTimeID.ToString();
                OEE_Cookie["startTime"] = model.StartTime.ToString("yyyy-MM-dd");
                OEE_Cookie["endTime"] = model.EndTime.ToString("yyyy-MM-dd");
                OEE_Cookie["ProductDate"] = model.EndTime.ToString("yyyy-MM-dd");
                OEE_Cookie["isFromPhotoReport"] = "false";
                OEE_Cookie.Expires.AddDays(30);
                HttpContext.Response.SetCookie(OEE_Cookie);
            }

            string apiUrl = "IPQCQualityReport/GetIPQCQualityMonthReportAPI";
            HttpResponseMessage response = APIHelper.APIPostAsync(model, page, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetStationDTOs(int LineId)
        {
            var apiUrl = string.Format("IPQCQualityReport/GetStationDTOAPI?LineId={0}", LineId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 判断当前时间是哪个时段
        /// </summary>
        /// <param name="PlantUID"></param>
        /// <param name="BgUID"></param>
        /// <param name="ShiftUID"></param>
        /// <returns></returns>
        public ActionResult GetCurrentTimeInterval(int PlantUID, int BgUID, int ShiftUID)
        {
            TimeIntervalParamModel param = new TimeIntervalParamModel();
            param.Plant_Organization_UID = PlantUID;
            param.BG_Organization_UID = BgUID;
            param.CurrentShiftID = ShiftUID;
            var apiUrl = string.Format("IPQCQualityReport/GetCurrentTimeIntervalAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(param, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion
    }
}