using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;
using PDMS.Common.Constants;

namespace PDMS.Web.Controllers
{
    public class OQCQualityController : WebControllerBase
    {

        #region ---- QA Master

        public ActionResult OQCMaster(string Flowchart_Master_UID, string Process_seq, string color, string MaterialType, string FlowChart_Detail_UID, string Process, string ProjectName, string IsQAProcess)
        {
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(MaterialType))
            {
                CheckPointInputConditionModel condition = new CheckPointInputConditionModel();
                condition.Color = color;
                condition.Flowchart_Master_UID = int.Parse(Flowchart_Master_UID);
                condition.Flowchart_Detail_UID = int.Parse(FlowChart_Detail_UID);
                condition.Process_seq = int.Parse(Process_seq);
                condition.MaterialType = MaterialType;
                condition.Process = Process;
                condition.Project_Name = ProjectName;

                var apiUrl = string.Format("EventReportManager/GetIntervalInfoAPI?opType={0}", "OP1");
                var responseMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responseMessage.Content.ReadAsStringAsync().Result;
                var time = JsonConvert.DeserializeObject<IntervalEnum>(result);
                condition.Time_interval = time.Time_Interval;
                condition.ProductDate = DateTime.Parse(time.NowDate);

                ViewBag.Flowchart_Master_UID = condition.Flowchart_Master_UID;
                ViewBag.MaterialType = condition.MaterialType;
                ViewBag.Color = condition.Color;
                ViewBag.Process_seq = condition.Process_seq;
                ViewBag.Process = condition.Process;
                ViewBag.Flowchart_Detail_UID = condition.Flowchart_Detail_UID;
                ViewBag.Project = condition.Project_Name;
                ViewBag.ProductDate = condition.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                ViewBag.Time_interval = condition.Time_interval;
                ViewBag.IsAssembleProcess = IsQAProcess == "Inspect_Assemble" ? "" : "hidden";
            }
            return View();
        }

        public ActionResult QueryOQCMasterData(string jsonWithData)
        {
            var apiUrl = "OQCQuality/QueryOQCMasterDataAPI";
            var entity = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonWithData);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryOQCNGDetails(QAReportSearchVM search, Page page)
        {
            var apiUrl = "OQCQuality/QueryOQCNGDetailsAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryOQCReworkDetails(QAReportSearchVM search, Page page)
        {
            var apiUrl = "OQCQuality/QueryOQCReworkDetailsAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SaveOQCData(string jsonWithProduct)
        {
            var apiUrl = "OQCQuality/SaveQaMasterDataAPI";
            var entity = JsonConvert.DeserializeObject<OQCInputData>(jsonWithProduct);

            entity.MasterData.Creator_UID = this.CurrentUser.AccountUId;
            entity.MasterData.Create_date = DateTime.Now;
            entity.MasterData.Modifier_UID = this.CurrentUser.AccountUId;
            entity.MasterData.Modified_date = DateTime.Now;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult AddTExceptionTypesoFlowChart(string jsonBadType)
        {
            var apiUrl = "OQCQuality/AddTExceptionTypesoFlowChartAPI";
            var entity = JsonConvert.DeserializeObject<ExceptionTypesAddToFlowChartVM>(jsonBadType);

            foreach (ExceptionTypeWithFlowchartVM item in entity.ExcetionTypes)
            {
                item.Creator_UID = this.CurrentUser.AccountUId;
                item.Creator_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #endregion

        #region ----- QA Reprot

        public ActionResult OQCReport()
        {
            if (TempData["search"] != null)
            {
                QAReportSearchVM search = (QAReportSearchVM)TempData["search"];
                ViewBag.Color = search.Color;
                ViewBag.ProductDate = search.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                ViewBag.MaterialType = search.MaterialType;
                ViewBag.OPtype = search.OPType;
                ViewBag.FunPlant = search.FunPlant;
                ViewBag.ProjectName = search.ProjectName;
                ViewBag.FlowChart_Master_UID = search.FlowChart_Master_UID;
                ViewBag.PartType = search.Part_Type;

                ViewBag.IsSearchHistory = "visible";
            }
            else
            {
                ViewBag.IsSearchHistory = "hidden";
            }

            return View();
        }

        public ActionResult QueryQADayReportSummery(QAReportSearchVM search, Page page)
        {
            var apiUrl = string.Format("OQCQuality/QueryQADayReportSummeryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryQADayReportTopFive(QAReportSearchVM search, Page page)
        {
            int languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            search.languageID = languageID;
            var apiUrl = string.Format("OQCQuality/QueryQADayReportTopFiveAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult OQCReportFromOQCHistoryBack(int FlowChart_Master_UID, string PartType, string project, string date, string materialType, string color, string OPType, string funplant)
        {
            QAReportSearchVM search = new QAReportSearchVM();
            search.Color = color;
            search.ProductDate = DateTime.Parse(date);
            search.MaterialType = materialType;
            search.OPType = OPType;
            search.FunPlant = funplant;
            search.ProjectName = project;
            search.FlowChart_Master_UID = FlowChart_Master_UID;
            search.Part_Type = PartType;

            TempData["search"] = search;

            return RedirectToAction("OQCReport", "OQCQuality");
        }

        #region  报表导出

        public FileResult ExportReportExcel(string query)
        {
            OQCReportExcel Report = new OQCReportExcel();
            QAReportSearchVM search = JsonConvert.DeserializeObject<QAReportSearchVM>(query);
            search.Count = 5;
            Report = GetOQCReportForExportExcel(search);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheetYield = excelPackage.Workbook.Worksheets.Add("Yield");
                var worksheetTopFive = excelPackage.Workbook.Worksheets.Add("Top5");

                string title = this.CurrentUser.GetUserInfo.OrgInfo.First().Plant + "_" + search.ProjectName + "_" + search.FunPlant + " " +
                    search.MaterialType + (string.IsNullOrEmpty(search.Color) ? "" : "(" + search.Color + ") ")
                    + search.ProductDate.Date.ToShortDateString() + this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.2Htestreport");

                WriteYieldDataToExcel(worksheetYield, Report, title, search.FunPlant);

                if (Report.TopFive.Count != 0)
                {
                    WriteTopFiveToExcel(worksheetTopFive, Report, search.FunPlant);
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        private void WriteYieldDataToExcel(ExcelWorksheet worksheet, OQCReportExcel data, string title, string funplant)
        {
            #region 设置标题信息

            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["A1:P1"].Merge = true;
            worksheet.Cells[1, 1].Value = title;
            SetExcelCellStyle(worksheet, 28, "A1:P1");

            worksheet.Cells[2, 1].Value = funplant == "OQC" ? "OQC FlOW" : "FLOW";

            worksheet.Cells[2, 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Dayshifts");

            worksheet.Cells["B2:E2"].Merge = true;
            worksheet.Cells[2, 6].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Nightshifts");

            worksheet.Cells["F2:I2"].Merge = true;
            worksheet.Cells[2, 10].Value = "24 hours";
            worksheet.Cells["J2:N2"].Merge = true;

            worksheet.Cells[3, 1].Value = "Item";
            worksheet.Cells[3, 2].Value = "Input";
            worksheet.Cells[3, 3].Value = "OK";
            worksheet.Cells[3, 4].Value = "NG";
            worksheet.Cells[3, 5].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Rework");

            worksheet.Cells[3, 6].Value = "Input";
            worksheet.Cells[3, 7].Value = "OK";
            worksheet.Cells[3, 8].Value = "NG";
            worksheet.Cells[3, 9].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Rework");
            worksheet.Cells[3, 10].Value = "Total";
            worksheet.Cells[3, 11].Value = "OK";
            worksheet.Cells[3, 12].Value = "NG";
            worksheet.Cells[3, 13].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Rework");
            worksheet.Cells[3, 14].Value = "WIP";
            worksheet.Cells[3, 15].Value = "First Yield Rate";
            worksheet.Cells[3, 16].Value = "Second Yield Rate";

            #endregion

            int index = 4;

            #region  Yield data
            List<OQCReprotVM> sumData = data.SumData;
            foreach (OQCReprotVM temp in sumData)
            {
                worksheet.Cells[index, 1].Value = temp.Process;
                worksheet.Cells[index, 2].Value = temp.DailyInput;
                worksheet.Cells[index, 3].Value = temp.DailyOK;
                worksheet.Cells[index, 4].Value = temp.DailyNG;
                worksheet.Cells[index, 5].Value = temp.DailyRework;

                worksheet.Cells[index, 6].Value = temp.NightInput;
                worksheet.Cells[index, 7].Value = temp.NightOK;
                worksheet.Cells[index, 8].Value = temp.NightNG;
                worksheet.Cells[index, 9].Value = temp.NightRework;

                worksheet.Cells[index, 10].Value = temp.Input;
                worksheet.Cells[index, 11].Value = temp.OK;
                worksheet.Cells[index, 12].Value = temp.NG;
                worksheet.Cells[index, 13].Value = temp.Rework;
                worksheet.Cells[index, 14].Value = temp.WIP;

                worksheet.Cells[index, 15].Value = temp.FirstYieldRate;
                worksheet.Cells[index, 16].Value = temp.SecondYieldRate;

                index++;
            }
            #endregion

            string cellsIndex = "A1:P" + (index - 1).ToString();
            worksheet.Cells[cellsIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[cellsIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[cellsIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[cellsIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells.AutoFitColumns();
        }

        private void WriteTopFiveToExcel(ExcelWorksheet worksheet, OQCReportExcel data, string funplant)
        {
            List<OQCReprotTopFiveTypeVM> TopFive = data.TopFive;

            var process = from T in TopFive
                          select T.Process;

            List<string> ProcessList = process.Distinct().ToList();

            //第一次将只有五个排名的Process存放在里面
            List<OQCReprotTopFiveTypeVM> fiveRandk = new List<OQCReprotTopFiveTypeVM>();
            int rowIndex = 0;
            foreach (string pro in ProcessList)
            {
                List<OQCReprotTopFiveTypeVM> tempList = TopFive.Where(x => x.Process == pro).ToList();
                if (tempList.Count >= 5)
                {
                    WriteTopFiveByProcess(worksheet, tempList, rowIndex);
                    rowIndex = rowIndex + 8;
                }
                else
                {
                    fiveRandk.AddRange(tempList);
                }
            }
            if (fiveRandk.Count > 0)
            {
                WriteTopFiveByProcess(worksheet, fiveRandk, rowIndex);
                rowIndex = rowIndex + 8;
            }

            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            string cellsIndex = funplant == "OQC" ? "A1:H" + rowIndex.ToString() : "A1:D8";
            worksheet.Cells[cellsIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[cellsIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[cellsIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[cellsIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells.AutoFitColumns();
        }

        private void WriteTopFiveByProcess(ExcelWorksheet worksheet, List<OQCReprotTopFiveTypeVM> TopFive, int rowIndex)
        {
            List<string> topType = new List<string> { this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.AyieldTop5"), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.SecondaryyieldTop5") };

            var process = from T in TopFive
                          select T.Process;

            List<string> ProcessList = process.Distinct().ToList();

            int index = 1;
            for (int i = 0; i < ProcessList.Count; i++)
            {
                string processName = ProcessList[i];
                if (i % 2 == 0)
                {
                    index = 1;
                }
                foreach (string type in topType)
                {
                    int StartIndex = 0;
                    string StartColumn = "";
                    string EndColumn = "";

                    if (TopFive.Exists(x => x.TOPType == type))
                    {
                        if (index == 1)
                        {
                            StartIndex = 1;
                            StartColumn = "A";
                            EndColumn = "D";
                        }
                        else
                        {
                            StartIndex = 5;
                            StartColumn = "E";
                            EndColumn = "H";
                            index = 1;
                        }

                        string ProcessName = StartColumn + (index + rowIndex).ToString() + ":" + EndColumn + (index + rowIndex).ToString();
                        worksheet.Cells[index + rowIndex, StartIndex].Value = processName;
                        worksheet.Cells[ProcessName].Merge = true;
                        SetExcelCellStyle(worksheet, 24, ProcessName);
                        index++;

                        string TypeName = StartColumn + (index + rowIndex).ToString() + ":" + EndColumn + (index + rowIndex).ToString();
                        worksheet.Cells[index + rowIndex, StartIndex].Value = type;
                        worksheet.Cells[TypeName].Merge = true;
                        index++;

                        worksheet.Cells[index + rowIndex, StartIndex].Value = "Rank";
                        worksheet.Cells[index + rowIndex, StartIndex + 1].Value = "D/I";
                        worksheet.Cells[index + rowIndex, StartIndex + 2].Value = "D/Q";
                        worksheet.Cells[index + rowIndex, StartIndex + 3].Value = "D/R";
                        index++;

                        foreach (OQCReprotTopFiveTypeVM detail in TopFive.Where(x => x.TOPType == type && x.Process == processName))
                        {
                            worksheet.Cells[index + rowIndex, StartIndex].Value = detail.RankNum;
                            worksheet.Cells[index + rowIndex, StartIndex + 1].Value = detail.TypeName;
                            worksheet.Cells[index + rowIndex, StartIndex + 2].Value = detail.Qty;
                            worksheet.Cells[index + rowIndex, StartIndex + 3].Value = detail.YieldRate;
                            index++;
                        }
                    }
                }
            }
        }

        private void SetExcelCellStyle(ExcelWorksheet worksheet, float fontSize, string localIndexCell)
        {
            worksheet.Cells[localIndexCell].Style.Font.Size = fontSize;
        }

        private OQCReportExcel GetOQCReportForExportExcel(QAReportSearchVM search)
        {
            var apiUrl = string.Format("OQCQuality/DownloadOQCReportExcelAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var data = JsonConvert.DeserializeObject<OQCReportExcel>(result);

            return data;
        }

        #endregion

        #endregion

        #region  ----- OQC历史查询


        public ActionResult OQCDetail(string FlowChart_Master_UID = null, string project = null, string PartType = null, string date = null, string process = null, string materialType = null, string color = null, string OPType = null, string funplant = null, int Flowchart_Detail_UID = 0)
        {
            ViewBag.project = string.IsNullOrEmpty(project) ? "" : project;
            ViewBag.date = string.IsNullOrEmpty(date) ? DateTime.Now.Date.ToString(FormatConstants.DateTimeFormatStringByDate) : date;
            ViewBag.process = string.IsNullOrEmpty(process) ? "" : process;
            ViewBag.materialType = string.IsNullOrEmpty(materialType) ? this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Normalmaterial") : materialType;
            ViewBag.color = string.IsNullOrEmpty(color) ? "" : color;
            ViewBag.OPType = string.IsNullOrEmpty(OPType) ? "" : OPType;
            ViewBag.Funplant = funplant;
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.PartType = PartType;
            ViewBag.Flowchart_Detail_UID = Flowchart_Detail_UID;

            if (!string.IsNullOrEmpty(color))
            {
                ViewBag.From = "Report";
            }
            else
            {
                ViewBag.From = "Home";
            }
            return View();
        }

        public ActionResult OQCHistorySearch(string FlowChart_Master_UID = null, string project = null, string PartType = null, string date = null, string process = null, string materialType = null, string color = null, string OPType = null, string funplant = null, int Flowchart_Detail_UID = 0)
        {
            ViewBag.project = string.IsNullOrEmpty(project) ? "" : project;
            ViewBag.date = string.IsNullOrEmpty(date) ? DateTime.Now.Date.ToString(FormatConstants.DateTimeFormatStringByDate) : date;
            ViewBag.process = string.IsNullOrEmpty(process) ? "" : process;
            ViewBag.materialType = string.IsNullOrEmpty(materialType) ? this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Normalmaterial") : materialType;
            ViewBag.color = string.IsNullOrEmpty(color) ? "" : color;
            ViewBag.OPType = string.IsNullOrEmpty(OPType) ? "" : OPType;
            ViewBag.Funplant = funplant;
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.PartType = PartType;
            ViewBag.Flowchart_Detail_UID = Flowchart_Detail_UID;

            if (!string.IsNullOrEmpty(color))
            {
                ViewBag.From = "Report";
            }
            else
            {
                ViewBag.From = "Home";
            }
            return View();
        }

        public ActionResult QueryOQCRecordData(string jsonWithData)
        {
            var apiUrl = "OQCQuality/QueryOQCRecordDataAPI";
            var entity = JsonConvert.DeserializeObject<QAReportSearchVM>(jsonWithData);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryOQCNGRecord(QAReportSearchVM search, Page page)
        {
            var apiUrl = "OQCQuality/QueryOQCNGRecordAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryOQCReworkRecord(QAReportSearchVM search, Page page)
        {
            var apiUrl = "OQCQuality/QueryOQCReworkRecordAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region  明细数据导出

        public FileResult ExportOQCDailyInputDataExcel(string query)
        {
            QAReportSearchVM search = JsonConvert.DeserializeObject<QAReportSearchVM>(query);
            if (search.FunPlant == "Assembly1" || search.FunPlant == "Assembly2" || search.FunPlant == "Assembly")
            {
                return ExportAmessbleDailyInputDataForExcel(search);
            }
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("");
            ExportOQCDataForExcel data = GetOQCInputDataForExportExcel(search);

            if (data.OQCDatas.Count != 0)
            {
                using (var excelPackage = new ExcelPackage(stream))
                {
                    foreach (OQCExportModel oqcData in data.OQCDatas)
                    {
                        var worksheetDaily = excelPackage.Workbook.Worksheets.Add(oqcData.MasterData.Process);
                        WriteOQCInputDataToExcel(worksheetDaily, oqcData);
                    }
                    excelPackage.Save();
                }
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #region ---- OQC 明细数据导出

        public void WriteOQCInputDataToExcel(ExcelWorksheet worksheet, OQCExportModel data)
        {
            OQC_InputMasterVM sumData = data.MasterData;
            List<OQC_InputDetailVM> DetailList = data.DetailDatas;

            #region 设置标题信息
            string CellColumn = "A1:F2";
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[CellColumn].Merge = true;
            worksheet.Cells[1, 1].Value = sumData.Process;

            worksheet.Cells[CellColumn].Style.Font.Size = 28;
            SetExcelCellStyle(worksheet, CellColumn);
            #endregion

            #region ------ Summary info

            string titleItemcell = "A3:A16";
            worksheet.Cells[titleItemcell].Merge = true;
            worksheet.Cells[3, 1].Value = "Item";
            SetExcelCellStyle(worksheet, titleItemcell);

            string titlecell = "B3:D3";
            worksheet.Cells[titlecell].Merge = true;
            worksheet.Cells[3, 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Enteritem");// "录入细项"; QA.Enteritem
            SetExcelCellStyle(worksheet, titlecell);

            string titleInputcell = "B4:D4";
            worksheet.Cells[titleInputcell].Merge = true;
            worksheet.Cells[4, 2].Value = "Input" + this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Numberinputs");// "Input 投入数"; QA.Numberinputs
            SetExcelCellStyle(worksheet, titleInputcell);

            string titleReworkFromAssemblecell = "B5:D5";
            worksheet.Cells[titleReworkFromAssemblecell].Merge = true;
            worksheet.Cells[5, 2].Value = "良品"; //"组装返回"; QA.Assemblereturn
            SetExcelCellStyle(worksheet, titleReworkFromAssemblecell);

            string titleGoodPartscell = "B6:D6";
            worksheet.Cells[titleGoodPartscell].Merge = true;
            worksheet.Cells[6, 2].Value = "NG"; //"Good Parts(良品数量)"; QA.GoodParts
            SetExcelCellStyle(worksheet, titleGoodPartscell);

            string titleNGPartscell = "B7:D7";
            worksheet.Cells[titleNGPartscell].Merge = true;
            worksheet.Cells[7, 2].Value = "返修";
            SetExcelCellStyle(worksheet, titleNGPartscell);

            string titleReworkcell = "B8:D8";
            worksheet.Cells[titleReworkcell].Merge = true;
            worksheet.Cells[8, 2].Value = "返修OK"; //"Rework(返组装)"; QA.Reworks
            SetExcelCellStyle(worksheet, titleReworkcell);

            string titleReworkFromProductLinecell = "B9:D9";
            worksheet.Cells[titleReworkFromProductLinecell].Merge = true;
            worksheet.Cells[9, 2].Value = "返前制程";// "产线返修数"; QA.Productionlinerepair
            SetExcelCellStyle(worksheet, titleReworkFromProductLinecell);

            string titleReworkNGcell = "B10:D10";
            worksheet.Cells[titleReworkNGcell].Merge = true;
            worksheet.Cells[10, 2].Value = "前制程返回"; //"返修缺陷数"; QA.Numberofdefects
            SetExcelCellStyle(worksheet, titleReworkNGcell);

            string titleNGcell = "B11:D11";
            worksheet.Cells[titleNGcell].Merge = true;
            worksheet.Cells[11, 2].Value = "NG缺陷数"; //"NG返修缺陷数";
            SetExcelCellStyle(worksheet, titleNGcell);

            string titleWIPcell = "B12:D12";
            worksheet.Cells[titleWIPcell].Merge = true;
            worksheet.Cells[12, 2].Value = "返修缺陷数"; //"WIP(未加工数)"; QA.Numberunprocessed
            SetExcelCellStyle(worksheet, titleWIPcell);

            string titleFirstRatecell = "B13:D13";
            worksheet.Cells[titleFirstRatecell].Merge = true;
            worksheet.Cells[13, 2].Value = "WIP"; //"一次良率"; QA.Ayield
            SetExcelCellStyle(worksheet, titleFirstRatecell);

            string titleReworkNGRatecell = "B14:D14";
            worksheet.Cells[titleReworkNGRatecell].Merge = true;
            worksheet.Cells[14, 2].Value = "一次良率"; //"返修缺陷率"; QA.Defectrate
            SetExcelCellStyle(worksheet, titleReworkNGRatecell);

            string titleNGRatecell = "B15:D15";
            worksheet.Cells[titleNGRatecell].Merge = true;
            worksheet.Cells[15, 2].Value = "二次良率"; //"NG缺陷率"; QA.NGdefectrate
            SetExcelCellStyle(worksheet, titleNGRatecell);

            //string titleSecondRatecell = "B16:D16";
            //worksheet.Cells[titleSecondRatecell].Merge = true;
            //worksheet.Cells[16, 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Secondaryyield");// "二次良率"; QA.Secondaryyield
            // SetExcelCellStyle(worksheet, titleSecondRatecell);

            string titleSumcell = "E3:F3";
            worksheet.Cells[titleSumcell].Merge = true;
            worksheet.Cells[3, 5].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Total"); //"合计"; QA.Total
            SetExcelCellStyle(worksheet, titleSumcell);

            string Inputcell = "E4:F4";
            worksheet.Cells[Inputcell].Merge = true;
            worksheet.Cells[4, 5].Value = sumData.Input;
            SetExcelCellStyle(worksheet, Inputcell);

            string Assemblecell = "E5:F5";
            worksheet.Cells[Assemblecell].Merge = true;
            worksheet.Cells[5, 5].Value = sumData.GoodParts_Qty;
            SetExcelCellStyle(worksheet, Assemblecell);

            string GoodQTycell = "E6:F6";
            worksheet.Cells[GoodQTycell].Merge = true;
            worksheet.Cells[6, 5].Value = sumData.NGParts_Qty;
            SetExcelCellStyle(worksheet, GoodQTycell);

            string NGPartsQTycell = "E7:F7";
            worksheet.Cells[NGPartsQTycell].Merge = true;
            worksheet.Cells[7, 5].Value = sumData.Rework;
            SetExcelCellStyle(worksheet, NGPartsQTycell);

            string Reworkcell = "E8:F8";
            worksheet.Cells[Reworkcell].Merge = true;
            worksheet.Cells[8, 5].Value = sumData.ReworkQtyFromOQC;
            SetExcelCellStyle(worksheet, Reworkcell);

            string ReworkFromProductLinecell = "E9:F9";
            worksheet.Cells[ReworkFromProductLinecell].Merge = true;
            worksheet.Cells[9, 5].Value = sumData.ReworkQtyFromAssemble;
            SetExcelCellStyle(worksheet, ReworkFromProductLinecell);

            string RepairNGcell = "E10:F10";
            worksheet.Cells[RepairNGcell].Merge = true;
            worksheet.Cells[10, 5].Value = sumData.ProductLineRework;
            SetExcelCellStyle(worksheet, RepairNGcell);

            string NGpartscell = "E11:F11";
            worksheet.Cells[NGpartscell].Merge = true;
            worksheet.Cells[11, 5].Value = sumData.NG_Qty;
            SetExcelCellStyle(worksheet, NGpartscell);

            string WIPcell = "E12:F12";
            worksheet.Cells[WIPcell].Merge = true;
            worksheet.Cells[12, 5].Value = sumData.RepairNG_Qty;
            SetExcelCellStyle(worksheet, WIPcell);

            string FirstYieldRatecell = "E13:F13";
            worksheet.Cells[FirstYieldRatecell].Merge = true;
            worksheet.Cells[13, 5].Value = sumData.WIP;
            SetExcelCellStyle(worksheet, FirstYieldRatecell);

            string RepairNG_Yieldcell = "E14:F14";
            worksheet.Cells[RepairNG_Yieldcell].Merge = true;
            worksheet.Cells[14, 5].Value = sumData.FirstYieldRate;
            SetExcelCellStyle(worksheet, RepairNG_Yieldcell);

            string NG_YieldCell = "E15:F15";
            worksheet.Cells[NG_YieldCell].Merge = true;
            worksheet.Cells[15, 5].Value = sumData.SecondYieldRate;
            SetExcelCellStyle(worksheet, NG_YieldCell);

            //string SecondYieldRateCell = "E16:F16";
            //worksheet.Cells[SecondYieldRateCell].Merge = true;
            //worksheet.Cells[16, 5].Value = sumData.SecondYieldRate;
            //SetExcelCellStyle(worksheet, SecondYieldRateCell);
            #endregion

            #region ------- Detail ExceptionTypes info

            worksheet.Cells[17, 1].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.Seq");// "序号";
            worksheet.Cells[17, 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Type");// "类型";
            worksheet.Cells[17, 3].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Functionfactory");// "功能厂";
            worksheet.Cells[17, 4].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Exceptionname"); //"异常名称";
            worksheet.Cells[17, 5].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Quantity");// "数量";
            worksheet.Cells[17, 6].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Defectrates");// "瑕疵率";

            int detailIndex = 18;
            foreach (OQC_InputDetailVM DetailInfo in DetailList)
            {
                worksheet.Cells[detailIndex, 1].Value = detailIndex - 18 + 1;
                worksheet.Cells[detailIndex, 2].Value = DetailInfo.TypeClassify;
                worksheet.Cells[detailIndex, 3].Value = DetailInfo.FunPlant;
                worksheet.Cells[detailIndex, 4].Value = DetailInfo.ExcetionTypeName;
                worksheet.Cells[detailIndex, 5].Value = DetailInfo.Qty;
                worksheet.Cells[detailIndex, 6].Value = DetailInfo.DayDefectRate;

                detailIndex++;
            }
            #endregion

            worksheet.Cells.AutoFitColumns();
        }
        private ExportOQCDataForExcel GetOQCInputDataForExportExcel(QAReportSearchVM search)
        {
            var apiUrl = string.Format("OQCQuality/DownloadOQCInputDataForExportExcelAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<ExportOQCDataForExcel>(result);
            return list;
        }
        #endregion

        #region  ---- 组装 明细数据导出
        public FileResult ExportAmessbleDailyInputDataForExcel(QAReportSearchVM search)
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("");
            search.Count = 10000;
            OQCReportExcel detailData = GetOQCReportForExportExcel(search);
            if (detailData.SumData.Count != 0)
            {
                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheetYield = excelPackage.Workbook.Worksheets.Add("Yield");
                    var worksheetDetails = excelPackage.Workbook.Worksheets.Add("Details");

                    string title = this.CurrentUser.GetUserInfo.OrgInfo.First().Plant + "_" + search.ProjectName + "_" + search.FunPlant + " " +
                        search.MaterialType + (string.IsNullOrEmpty(search.Color) ? "" : "(" + search.Color + ") ")
                        + search.ProductDate.Date.ToShortDateString() + this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.2Htestreport");

                    WriteYieldDataToExcel(worksheetYield, detailData, title, search.FunPlant);

                    if (detailData.TopFive.Count != 0)
                    {
                        WriteDetailsDataToExcel(worksheetDetails, detailData);
                    }
                    excelPackage.Save();
                }
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }


        private void WriteDetailsDataToExcel(ExcelWorksheet worksheet, OQCReportExcel TopFive)
        {
            #region 设置标题信息
            string CellColumn = "A1:E2";
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[CellColumn].Merge = true;
            worksheet.Cells[1, 1].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Detaildata");

            worksheet.Cells[CellColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[CellColumn].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Pink);

            worksheet.Cells[CellColumn].Style.Font.Size = 28;
            SetExcelCellStyle(worksheet, CellColumn);


            worksheet.Cells[3, 1].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.Seq");
            worksheet.Cells[3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
            worksheet.Cells[CellColumn].Style.Font.Size = 16;
            SetExcelCellStyle(worksheet, "A3");

            worksheet.Cells[3, 3].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badname");
            worksheet.Cells[3, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[3, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
            worksheet.Cells[CellColumn].Style.Font.Size = 16;
            SetExcelCellStyle(worksheet, "C3");

            worksheet.Cells[3, 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.Process_Station");
            worksheet.Cells[3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[3, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
            worksheet.Cells[CellColumn].Style.Font.Size = 16;
            SetExcelCellStyle(worksheet, "B3");

            worksheet.Cells[3, 4].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Quantity");
            worksheet.Cells[3, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[3, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
            worksheet.Cells[CellColumn].Style.Font.Size = 16;
            SetExcelCellStyle(worksheet, "D3");

            worksheet.Cells[3, 5].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badrate");
            worksheet.Cells[3, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[3, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
            worksheet.Cells[CellColumn].Style.Font.Size = 16;
            SetExcelCellStyle(worksheet, "E5");


            #endregion

            #region ------- Detail ExceptionTypes info

            int detailIndex = 4;
            foreach (OQCReprotTopFiveTypeVM DetailInfo in TopFive.TopFive)
            {
                worksheet.Cells[detailIndex, 1].Value = detailIndex - 4 + 1;
                worksheet.Cells[detailIndex, 2].Value = DetailInfo.Process;
                worksheet.Cells[detailIndex, 3].Value = DetailInfo.TypeName;
                worksheet.Cells[detailIndex, 4].Value = DetailInfo.Qty;
                worksheet.Cells[detailIndex, 5].Value = DetailInfo.YieldRate;

                detailIndex++;
            }
            #endregion
        }


        #endregion

        private void SetExcelCellStyle(ExcelWorksheet worksheet, string localIndexCell)
        {

            worksheet.Cells[localIndexCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        #endregion
    }
}