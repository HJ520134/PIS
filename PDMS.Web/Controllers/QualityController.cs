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
using System.Configuration;

namespace PDMS.Web.Controllers
{
    public class QualityController : WebControllerBase
    {
        // GET: Quality
        private static string WebAPIPath
        {
            get { return ConfigurationManager.AppSettings["WebMachinePath"].ToString(); }
        }

        #region ----Common Method

        public ActionResult QueryPlant()
        {
            int Account_UId = this.CurrentUser.AccountUId;
            string apiUrl = string.Format("Quality/QueryPlantAPI?Account_UId={0}", Account_UId);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取指定工作日，指定功能厂当天录入数据的制程列表
        /// </summary>
        /// <param name="Flowchart_Master_UID"></param>
        /// <param name="funplant"></param>
        /// <param name="Product_Date"></param>
        /// <returns></returns>
        public ActionResult GetProcessSource(string Flowchart_Master_UID, string FunPlant, string Product_Date, string Color)
        {
            string apiUrl = string.Format("Quality/GetProcessSource?FunPlant={0}&Flowchart_Master_UID={1}&Product_Date={2}&Color={3}", FunPlant, Flowchart_Master_UID, Product_Date, Color);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取当前用户的OP类型
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryOPType(int Plant_OrganizationUID)
        {
            int Account_UId = this.CurrentUser.AccountUId;
            var apiUrl = string.Format("Quality/QueryOPTypeAPI?Account_UId={0}&&Plant_OrganizationUID={1}", Account_UId, Plant_OrganizationUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取指定OP类型下的专案
        /// </summary>
        /// <param name="OPType_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetQAProject(int OPType_Organization_UID)
        {
            var apiUrl = string.Format("Quality/GetQAProjectAPI?OPType_Organization_UID={0}", OPType_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 根据所选专案获取部件类型
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPartType(int Project_UID)
        {
            var apiUrl = string.Format("Quality/GetPartTypeAPI?Project_UID={0}", Project_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryRecordColor(int Flowchart_Master_UID, string FunPlant, string ProductDate, string MaterialType)
        {
            var apiUrl = string.Format(@"Quality/QueryRecordColorAPI?Flowchart_Master_UID={0}&&FunPlant='{1}'&&ProductDate={2}&&MaterialType={3}", Flowchart_Master_UID, FunPlant, ProductDate, MaterialType);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFlowChartVerion(int Flowchart_Master_UID, string ProductDate)
        {
            var apiUrl = string.Format(@"Quality/GetFlowChartVerionAPI?Flowchart_Master_UID={0}&&ProductDate={1}", Flowchart_Master_UID, ProductDate);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFunPlant(int Flowchart_Master_UID)
        {
            string apiUrl = string.Format("Quality/QueryFunPlantAPI?Flowchart_Master_UID={0}", Flowchart_Master_UID);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        #endregion


        public ActionResult QAFlowChartList()
        {
            var roleList = this.CurrentUser.GetUserInfo.RoleList.Select(m => m.Role_ID).ToList();
            if (roleList.Contains("QA Assemble Input") || roleList.Contains("QA OQC Input"))
            {
                ViewBag.Role = "QA_OQC_Input";
            }
            else if (roleList.Contains("QA IPQC Input"))
            {
                ViewBag.Role = "QA_IPQC_Input";
            }
            else
            {
                ViewBag.Role = "Admin";
            }
            return View();
        }

        #region ---- 不良类型维护


        public ActionResult QAExcetionType(int FlowChart_Master_UID, string ProjectName, string Part_Types)
        {
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.ProjectName = ProjectName;
            ViewBag.Part_Types = Part_Types;
            return View();
        }


        public ActionResult QueryBadTypes(BadTypeSearch search, Page page)
        {
            var apiUrl = string.Format("Quality/QueryBadTypesAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryBadTypeByUID(int uuid)
        {
            var apiUrl = string.Format("Quality/QueryBadTypeByUIDAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddBadType(string jsonBadType)
        {
            var apiUrl = "Quality/AddBadTypeAPI";
            var item = JsonConvert.DeserializeObject<ExceptionTypeVM>(jsonBadType);
            item.Creator_UID = this.CurrentUser.AccountUId;
            item.Create_Date = DateTime.Now;
            item.Modified_UID = this.CurrentUser.AccountUId;
            item.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(item, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult SaveDatas(string jsonWithProduct)
        {

            var apiUrl = "Quality/SaveDatasAPI";
            var entity = JsonConvert.DeserializeObject<ExceptionTypeList>(jsonWithProduct);

            foreach (ExceptionTypeVM item in entity.ExceptionTypeLists)
            {
                item.Creator_UID = this.CurrentUser.AccountUId;
                item.Create_Date = DateTime.Now;
                item.Modified_UID = this.CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult ModifyBadType(string jsonWithProduct)
        {

            var apiUrl = "Quality/ModifyBadTypeAPI";
            var entity = JsonConvert.DeserializeObject<ExceptionTypeVM>(jsonWithProduct);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteBadType(int UID)
        {

            var apiUrl = string.Format("Quality/DeleteBadTypeAPI?uuid={0}", UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult CheckTypeByCode(string code)
        {

            var apiUrl = string.Format("Quality/CheckTypeByCodeAPI?code={0}", code);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <returns></returns>
        public FileResult DownloadExcel()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "BadType_Template.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
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

        public string ImportExcel(HttpPostedFileBase uploadName, string FlowChart_Master_UID)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(uploadName.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet");
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] {
                   this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badname"),
                   this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Project"),
                   this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badtype"),
                   "不良类型Code",
                   "不良类型英文名称",
                   this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Referredto"),
                   this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Parenttype"),
                   this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Levelnumber"),
                };

                int iRow1 = 1;
                bool allColumnsAreEmpty = true;
                for (var i = 1; i <= propertiesHead.Length; i++)
                {
                    if (worksheet.Cells[iRow1, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow1, i].Value.ToString()))
                    {
                        allColumnsAreEmpty = false;
                        break;
                    }
                }
                if (allColumnsAreEmpty)
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Excelformat");
                    return errorInfo;
                }
                string ExceptionType = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow1, GetColumnIndex(propertiesHead, this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badtype"))].Value);
                string ExceptionName = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow1, GetColumnIndex(propertiesHead, this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badname"))].Value);
                string TypeLevel = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow1, GetColumnIndex(propertiesHead, this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Levelnumber"))].Value);
                string Project = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow1, GetColumnIndex(propertiesHead, this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Project"))].Value);

                if (string.IsNullOrWhiteSpace(Project) || string.IsNullOrWhiteSpace(ExceptionType) || string.IsNullOrWhiteSpace(ExceptionName) || string.IsNullOrWhiteSpace(TypeLevel))
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Levelnotbeempty");
                    return errorInfo;
                }
                List<ExceptionTypeTempVM> listTemp = new List<ExceptionTypeTempVM>();
                ExceptionTypeTempList mgDataList = new ExceptionTypeTempList();
                //Excel行号
                int iRow = 2;
                //Excel列号
                //int iColumn;
                //int j = 1;

                bool flag = false;
                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    ExceptionTypeTempVM newExTypeDataItem = new ExceptionTypeTempVM();
                    var typeName = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 1].Value);
                    var ProjectName = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);
                    var typeClassfy = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value);
                    var BadTypeCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);
                    var BadTypeEnglishCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    if (string.IsNullOrWhiteSpace(typeName))
                    {
                        flag = true;
                        errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Typenovalue"), iRow);
                        break;
                    }
                    else if (string.IsNullOrWhiteSpace(typeClassfy))
                    {
                        flag = true;
                        errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Nobadtype"), iRow);
                        break;
                    }
                    var level = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 8].Value);
                    if (string.IsNullOrWhiteSpace(level))
                    {
                        flag = true;
                        errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Levelnovalue"), iRow);
                        break;
                    }
                    var fatherNode = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);
                    var shortName = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);

                    newExTypeDataItem.Creator_UID = this.CurrentUser.AccountUId;
                    newExTypeDataItem.Create_Date = DateTime.Now;
                    newExTypeDataItem.TypeClassify = typeClassfy.Trim();
                    newExTypeDataItem.ShortName = string.IsNullOrEmpty(shortName) ? "" : shortName.Trim();
                    newExTypeDataItem.FatherNode = string.IsNullOrEmpty(fatherNode) ? "" : fatherNode.Trim();
                    newExTypeDataItem.TypeLevel = int.Parse(level);
                    newExTypeDataItem.TypeName = typeName.Trim();
                    newExTypeDataItem.Project = ProjectName.Trim();
                    newExTypeDataItem.BadTypeCode = string.IsNullOrEmpty(BadTypeCode) ? "" : BadTypeCode.Trim();
                    newExTypeDataItem.BadTypeEnglishCode = string.IsNullOrEmpty(BadTypeEnglishCode) ? "" : BadTypeEnglishCode.Trim();
                    newExTypeDataItem.Flowchart_Master_UID = int.Parse(FlowChart_Master_UID);

                    if (listTemp.Exists(x=>x.TypeName.ToLower() == newExTypeDataItem.TypeName.ToLower()))
                    {
                        return string.Format("第{0}行[不良名称: {1}] 重复(或大小写重复),请检查数据.", iRow, newExTypeDataItem.TypeName);
                    }
                    listTemp.Add(newExTypeDataItem);
                }
                mgDataList.ImportList = listTemp;
                if (flag)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("Quality/ImportExcelAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

                var result = responMessage.Content.ReadAsStringAsync().Result;
                return result;

            }

        }

        public ActionResult GetTypeClassfy()
        {
            int userUID = this.CurrentUser.AccountUId;
            var apiUrl = string.Format("Settings/GetTypeClassfyAPI?userUID={0}", userUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region ---- Check Point List
        public ActionResult CheckPointsList(string FlowChart_Master_UID, string Color, string MaterialType, string Part_Types)
        {
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.MaterialType = MaterialType;
            ViewBag.Color = Color == "/" ? "" : Color;
            ViewBag.Part_Types = Part_Types;
            return View();
        }

        public ActionResult CheckPointsListo(string FlowChart_Master_UID, string Color, string MaterialType, string Part_Types)
        {
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.MaterialType = MaterialType;
            ViewBag.Color = Color == "/" ? "" : Color;
            ViewBag.Part_Types = Part_Types;
            return View();
        }
        public ActionResult QueryCheckPointList(QAReportSearchVM query)
        {
            int userUID = this.CurrentUser.AccountUId;
            string apiUrl = string.Format("Quality/GetCheckPointsList?UserUid={0}&FlowChart_Master_UID={1}&Color={2}&MaterialType={3}", userUID, query.FlowChart_Master_UID, query.Color, query.MaterialType);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 数据条件获取
        /// </summary>
        /// <param name="processSeq"></param>
        /// <param name="flowchartMatserUid"></param>
        /// <returns></returns>  
        public ActionResult QueryConditions(string FlowChart_Master_UID)
        {
            string apiUrl = string.Format("Quality/QueryInputConditions?FlowChart_Master_UID={0}", FlowChart_Master_UID);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryOQCReportConditions(string processName, string Project)
        {
            string apiUrl = string.Format("Quality/QueryOQCReportConditionsAPI?processName={0}&&Project={1}", processName, Project);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region ---- QA Master

        public ActionResult QAMaster(string FlowChart_Master_UID, string Process_seq, string color, string MaterialType, string FlowChart_Detail_UID, string Process, string ProjectName, string Part_Types)
        {
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(MaterialType))
            {
                CheckPointInputConditionModel condition = new CheckPointInputConditionModel();
                condition.Color = color;

                condition.Flowchart_Master_UID = int.Parse(FlowChart_Master_UID);
                condition.Flowchart_Detail_UID = int.Parse(FlowChart_Detail_UID);
                condition.Process_seq = int.Parse(Process_seq);
                condition.MaterialType = MaterialType;
                condition.Process = Process;
                condition.Project_Name = ProjectName;
                condition.Part_Types = Part_Types;

                //根据专案UID获取对应的OP

                var apiUrl1 = string.Format("EventReportManager/GetOPByFlowchartMasterUIDAPI?masterUID={0}", FlowChart_Master_UID);
                var responseMessage1 = APIHelper.APIGetAsync(apiUrl1);
                var OP = responseMessage1.Content.ReadAsStringAsync().Result;


                var apiUrl = string.Format("EventReportManager/GetIntervalInfoAPI?opType={0}", JsonConvert.DeserializeObject<String>(OP));
                var responseMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responseMessage.Content.ReadAsStringAsync().Result;
                var time = JsonConvert.DeserializeObject<IntervalEnum>(result);
                condition.Time_interval = time.Time_Interval;
                condition.ProductDate = DateTime.Parse(time.NowDate);


                ViewBag.FlowChartMasterUID = condition.Flowchart_Master_UID;
                ViewBag.MaterialType = condition.MaterialType;
                ViewBag.Color = condition.Color;
                ViewBag.Place = condition.Place;
                ViewBag.Process_seq = condition.Process_seq;
                ViewBag.Process = condition.Process;

                ViewBag.DetailUID = condition.Flowchart_Detail_UID;
                ViewBag.Project_Name = condition.Project_Name;
                ViewBag.IsSearchHistory = "hidden";
                ViewBag.ProductDate = condition.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                ViewBag.Time_interval = condition.Time_interval;
                ViewBag.Part_Types = Part_Types;
                TempData["Condition"] = condition;
                ViewBag.CurrentOP = JsonConvert.DeserializeObject<String>(OP);
            }
            else
            {

                //根据专案UID获取对应的OP

                var apiUrl1 = string.Format("EventReportManager/GetOPByFlowchartMasterUIDAPI?masterUID={0}", FlowChart_Master_UID);
                var responseMessage1 = APIHelper.APIGetAsync(apiUrl1);
                var OP = responseMessage1.Content.ReadAsStringAsync().Result;
                ViewBag.CurrentOP = JsonConvert.DeserializeObject<String>(OP);
                ViewBag.FlowChartMasterUID = FlowChart_Master_UID;
                ViewBag.Project_Name = ProjectName;
                ViewBag.IsSearchHistory = "visible";
            }
            return View();
        }

        public ActionResult QueryQAHistroyDatas(QAReportSearchVM search, Page page)
        {
            var apiUrl = string.Format("Quality/QueryQAHistroyDatasAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryQAInputData()
        {
            if (TempData["Condition"] == null)
            {
                return Content("", "application/json");
            }
            CheckPointInputConditionModel condition = (CheckPointInputConditionModel)TempData["Condition"];
            var apiUrl = "Quality/QueryQAInputDataAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(condition, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SaveQAMasterData(string jsonWithProduct)
        {
            var apiUrl = "Quality/SaveQaMasterData";
            var entity = JsonConvert.DeserializeObject<QAMasterVM>(jsonWithProduct);

            entity.Creator_UID = this.CurrentUser.AccountUId;
            entity.Creator_UID = this.CurrentUser.AccountUId;
            entity.Create_Date = DateTime.Now;
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult ModifyQAMasterData(string jsonWithData)
        {
            var apiUrl = "Quality/ModifyQAMasterDataAPI";
            var entity = JsonConvert.DeserializeObject<QAMasterVM>(jsonWithData);

            entity.Modified_Date = DateTime.Now;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #endregion

        #region   ---- Qa Detail
        public ActionResult QAInputDetail(int QAMasterUID, string criteriaInfo, int FlowChart_Master_UID, bool CanModify, string Project, int FlowChart_Detail_UID, string ProductDate, string Time_interval, string MaterialType, string color, bool FromQAMasterHistory)
        {
            criteriaInfo = criteriaInfo.Replace("_", "&");
            criteriaInfo = criteriaInfo.EndsWith("颜色:") ? criteriaInfo.Replace("颜色:", "") : criteriaInfo;

            ViewBag.MasterUID = QAMasterUID;
            ViewBag.Criteria = criteriaInfo;
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.CanModify = CanModify;
            ViewBag.Project = Project;
            ViewBag.FlowChart_Detail_UID = FlowChart_Detail_UID;
            ViewBag.ProductDate = ProductDate;
            ViewBag.Time_interval = Time_interval;
            ViewBag.MaterialType = MaterialType;
            ViewBag.Color = color;

            TimeSpan t = DateTime.Now - DateTime.Parse(ProductDate);

            if (FromQAMasterHistory)
            {
                ViewBag.From = "hidden";
            }
            else if (t.Days > 1)
            {
                ViewBag.From = "hidden";
            }
            else
            {
                ViewBag.From = "visible";
            }


            return View();
        }

        public ActionResult QueryQAInputDetailVM(QADetailSearch search, Page page)
        {
            var apiUrl = string.Format("Quality/QueryQAInputDetailVMAPI");

            search.User_UID = this.CurrentUser.AccountUId;
            search.MesAPIPath = WebAPIPath;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryQAExceptionType(string JsonBadType)
        {
            var apiUrl = string.Format("Quality/QueryQAExceptionTypeAPI");
            var entity = JsonConvert.DeserializeObject<QADetailSearch>(JsonBadType);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult GetAllFirstTargetYield(QAReportSearchVM search)
        {
            var apiUrl = string.Format("Quality/GetAllFirstTargetYield");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetAllSecondTargetYield(QAReportSearchVM search)
        {
            var apiUrl = string.Format("Quality/GetAllSecondTargetYield");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
      

        public ActionResult ModifyQAInputDetail(string jsonWithProduct)
        {

            var apiUrl = "Quality/ModifyQAInputDetailAPI";
            var entity = JsonConvert.DeserializeObject<QAInputDetailVM>(jsonWithProduct);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryQAInputDetailVMByUID(int uuid)
        {
            var apiUrl = string.Format("Quality/QuerySingleQAInputDetailInfoAPI?QADetailUID={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryExceptionType(int typeLevel, string parentCode, int Flowchart_Master_UID)
        {
            var apiUrl = string.Format("Quality/QueryExceptionTypeForAddAPI?typeLevel={0}&&parentCode={1}&&Flowchart_Master_UID={2}", typeLevel, parentCode, Flowchart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryExceptionTypeForSearch(int typeLevel, string parentCode, int QAMasterUID, string ProductDate, int Flowchart_Master_UID)
        {
            var apiUrl = string.Format("Quality/QueryExceptionTypeForSearchAPI?typeLevel={0}&&parentCode={1}&&QAMasterUID={2}&&ProductDate={3}&&Flowchart_Master_UID={4}", typeLevel, parentCode, QAMasterUID, ProductDate, Flowchart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult addBadDetail(int mmasterUID, string criteriaInfo)
        {
            criteriaInfo = criteriaInfo.Replace("+", "&amp");
            criteriaInfo = criteriaInfo.Replace("-", "=");

            TempData["QAMasterUID"] = mmasterUID;
            TempData["criteriaInfo"] = criteriaInfo;

            return RedirectToAction("QAInputDetailAdd", "Quality");
        }


        public ActionResult QueryQAInputDetailByMasterUID(QAReportSearchVM search, Page page)
        {
            var apiUrl = string.Format("Quality/QueryQAInputDetailByMasterUIDAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult InsertQAInputDetail(string jsonBadType)
        {

            var apiUrl = "Quality/InsertQAInputDetailAPI";
            var entity = JsonConvert.DeserializeObject<QAInputDetailListVM>(jsonBadType);

            foreach (QAInputDetailVM item in entity.DataList)
            {
                item.Creator_UID = this.CurrentUser.AccountUId;
                item.CreateDate = DateTime.Now;
                item.Modified_UID = this.CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult UpdateQAInputDetail(string jsonBadType)
        {

            var apiUrl = "Quality/UpdateQAInputDetailAPI";
            var entity = JsonConvert.DeserializeObject<QAInputDetailListVM>(jsonBadType);

            foreach (QAInputDetailVM item in entity.DataList)
            {
                item.Creator_UID = this.CurrentUser.AccountUId;
                item.CreateDate = DateTime.Now;
                item.Modified_UID = this.CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #endregion

        #region ---- QA Report

        public ActionResult IPQCQAReportSum()
        {
            if (TempData["search"] != null)
            {
                QAReportSearchVM search = (QAReportSearchVM)TempData["search"];
                ViewBag.Color = search.Color;
                ViewBag.ProductDate = search.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                ViewBag.MaterialType = search.MaterialType;
                ViewBag.FunPlant = "IPQC";
                ViewBag.ProjectName = search.ProjectName;
                ViewBag.PartType = search.Part_Type;

                ViewBag.FlowChart_Master_UID = search.FlowChart_Master_UID;

                ViewBag.Time_interval = search.Time_interval;


                ViewBag.IsReturnFromDetailReprot = "true";
            }
            else
            {
                ViewBag.IsReturnFromDetailReprot = "false";
            }
            return View();
        }
        public ActionResult QAReportSum()
        {
            if (TempData["search"] != null)
            {
                QAReportSearchVM search = (QAReportSearchVM)TempData["search"];
                ViewBag.Color = search.Color;
                ViewBag.ProductDate = search.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate);
                ViewBag.MaterialType = search.MaterialType;
                ViewBag.FunPlant = search.FunPlant;
                ViewBag.ProjectName = search.ProjectName;
                ViewBag.PartType = search.Part_Type;

                ViewBag.FlowChart_Master_UID = search.FlowChart_Master_UID;

                ViewBag.Time_interval = search.Time_interval;


                ViewBag.IsReturnFromDetailReprot = "true";
            }
            else
            {
                ViewBag.IsReturnFromDetailReprot = "false";
            }
            return View();
        }

        
             public ActionResult QueryFuncReportSummary(QAReportSearchVM search, Page page)
        {
            if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Allday"))
            {
                search.Tab_Select_Text = "全天";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Dayshift"))
            {
                search.Tab_Select_Text = "白班小计";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Nightshift"))
            {
                search.Tab_Select_Text = "夜班小计";
            }

            var apiUrl = string.Format("Quality/QueryFuncReportSummaryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryIPQCALLProcessReportSummary(QAReportSearchVM search, Page page)
        {
            if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Allday"))
            {
                search.Tab_Select_Text = "全天";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Dayshift"))
            {
                search.Tab_Select_Text = "白班小计";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Nightshift"))
            {
                search.Tab_Select_Text = "夜班小计";
            }

            var apiUrl = string.Format("Quality/QueryIPQCALLProcessReportSummaryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QAReport(string ProductDate, int FlowChart_Master_UID, string color,
            int FlowChart_Detail_UID, string MaterialType, string TimeInterVal, string ProjectName, string Process, string PartType)
        {
            ViewBag.Process = Process;
            ViewBag.ProjectName = ProjectName;

            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.ProductDate = string.IsNullOrEmpty(ProductDate) ? DateTime.Now.Date.ToString(FormatConstants.DateTimeFormatStringByDate) : ProductDate;
            ViewBag.FlowChart_Detail_UID = FlowChart_Detail_UID;
            ViewBag.MaterialType = string.IsNullOrEmpty(MaterialType) ? this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Normalmaterial") : MaterialType;
            ViewBag.color = string.IsNullOrEmpty(color) ? "" : color;
            ViewBag.TimeInterVal = TimeInterVal;
            ViewBag.PartType = PartType;

            return View();
        }

        public ActionResult IPQCReportFromQAReportBack(string Color, int FlowChart_Master_UID, string ProductDate, string ProjectName, string TimeInterVal, string MaterialType, string PartType)
        {
            QAReportSearchVM search = new QAReportSearchVM();
            search.Color = Color;
            search.ProductDate = DateTime.Parse(ProductDate);
            search.MaterialType = MaterialType;

            search.FlowChart_Master_UID = FlowChart_Master_UID;
            search.Time_interval = TimeInterVal;
            search.ProjectName = ProjectName;
            search.Part_Type = PartType;

            TempData["search"] = search;
            return RedirectToAction("QAReportSum", "Quality");
        }

        public ActionResult QueryQADayReportSummery(QAReportSearchVM search, Page page)
        {
            if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Allday"))
            {
                search.Tab_Select_Text = "全天";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Dayshift"))
            {
                search.Tab_Select_Text = "白班小计";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Nightshift"))
            {
                search.Tab_Select_Text = "夜班小计";
            }
            var apiUrl = string.Format("Quality/QueryQADayReportSummeryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryQADayReportFirstTopTen(QAReportSearchVM search, Page page)
        {
            if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Allday"))
            {
                search.Tab_Select_Text = "全天";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Dayshift"))
            {
                search.Tab_Select_Text = "白班小计";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Nightshift"))
            {
                search.Tab_Select_Text = "夜班小计";
            }
            int languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            search.languageID = languageID;
            var apiUrl = string.Format("Quality/QueryQADayReportFirstTopTenAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryQADayReportSecondTopTen(QAReportSearchVM search, Page page)
        {
            if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Allday"))
            {
                search.Tab_Select_Text = "全天";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Dayshift"))
            {
                search.Tab_Select_Text = "白班小计";
            }
            else if (search.Tab_Select_Text == this.CurrentUser.GetLocaleStringResource(1, "QA.Nightshift"))
            {
                search.Tab_Select_Text = "夜班小计";
            }
            int languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            search.languageID = languageID;
            var apiUrl = string.Format("Quality/QueryQADayReportSecondTopTenAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #region  报表导出

        public FileResult ExportIPQCReportExcel(string query)
        {
            List<QAReportVM> dailySumReport = new List<QAReportVM>();
            List<QAReportVM> NightSumReport = new List<QAReportVM>();
            List<QAReportVM> DayReport = new List<QAReportVM>();
            QAReportSearchVM search = JsonConvert.DeserializeObject<QAReportSearchVM>(query);

            #region 分别获取全天、白班、夜班的所有报表
            search.Time_interval = "Daily_Sum";
            dailySumReport = GetIPQCReportForExportExcel(search);

            search.Time_interval = "Night_Sum";
            NightSumReport = GetIPQCReportForExportExcel(search);

            search.Time_interval = "ALL";
            DayReport = GetIPQCReportForExportExcel(search);
            #endregion

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheetDaily = excelPackage.Workbook.Worksheets.Add("daily");
                var worksheetNight = excelPackage.Workbook.Worksheets.Add("Night");
                var worksheetDay = excelPackage.Workbook.Worksheets.Add("24H");

                if (dailySumReport.Count > 0)
                {
                    WriteDataToExcel(worksheetDaily, dailySumReport,
                        this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Dayshifts"),
                        search.ProjectName, search.ProductDate.ToShortDateString(), search.MaterialType);
                }
                if (NightSumReport.Count > 0)
                {
                    WriteDataToExcel(worksheetNight, NightSumReport,
                           this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Nightshifts"),
                       search.ProjectName, search.ProductDate.ToShortDateString(), search.MaterialType);
                }
                if (DayReport.Count > 0)
                {
                    WriteDataToExcel(worksheetDay, DayReport, "24H", search.ProjectName, search.ProductDate.ToShortDateString(), search.MaterialType);
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public void WriteDataToExcel(ExcelWorksheet worksheet, List<QAReportVM> data, string title, string Project, string ProductDate, string MaterialType)
        {
            #region 设置标题信息

            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A:R"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A:R"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A:R"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A:R"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells[1, 1].Value = Project;
            worksheet.Cells[1, 7].Value = ProductDate;
            worksheet.Cells[1, 13].Value = MaterialType + "(" + data[0].summeryData.Color + ")";
            worksheet.Cells["A1:F2"].Merge = true;
            worksheet.Cells["G1:L2"].Merge = true;
            worksheet.Cells["M1:R2"].Merge = true;
            worksheet.Cells[3, 1].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.InspectionReport") + "(" + title + ")";
            worksheet.Cells["A3:R3"].Merge = true;

            worksheet.Cells["A3:R3"].Style.Font.Size = 24;
            worksheet.Cells["A1:F2"].Style.Font.Size = 28;
            worksheet.Cells["G1:L2"].Style.Font.Size = 28;
            worksheet.Cells["M1:R2"].Style.Font.Size = 20;

            SetExcelCellStyle(worksheet, "A3:R3");
            SetExcelCellStyle(worksheet, "A1:F2");
            SetExcelCellStyle(worksheet, "G1:L2");
            SetExcelCellStyle(worksheet, "M1:R2");

            #endregion

            int index = 4;
            foreach (QAReportVM temp in data)
            {
                QAReportDaySummeryDTO sumData = temp.summeryData;

                #region 设置 title
                int titleIndex = index;
                string titleFirst_cell = "B" + index.ToString() + ":" + "I" + index.ToString();
                string titleSecond_cell = "J" + index.ToString() + ":" + "R" + index.ToString();

                worksheet.Cells[titleFirst_cell].Merge = true;
                worksheet.Cells[index, 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Abadrate");
                worksheet.Cells[titleSecond_cell].Merge = true;
                worksheet.Cells[index, 10].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Twobadtimes");

                worksheet.Cells[titleFirst_cell].Style.Font.Size = 14;
                worksheet.Cells[titleSecond_cell].Style.Font.Size = 14;
                SetExcelCellStyle(worksheet, titleFirst_cell);
                SetExcelCellStyle(worksheet, titleSecond_cell);

                string titleFirstCheck_cell = "B" + (index + 1).ToString() + ":" + "B" + (index + 2).ToString();
                string titleFirstOK_cell = "C" + (index + 1).ToString() + ":" + "C" + (index + 2).ToString();
                string titleFirstTargetYiled_cell = "D" + (index + 1).ToString() + ":" + "D" + (index + 2).ToString();
                string titleFirstYiled_cell = "E" + (index + 1).ToString() + ":" + "E" + (index + 2).ToString();
                string titleFirstTitle_cell = "F" + (index + 1).ToString() + ":" + "I" + (index + 1).ToString();

                worksheet.Cells[index + 1, 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Numbertests");
                worksheet.Cells[titleFirstCheck_cell].Merge = true;
                worksheet.Cells[index + 1, 3].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.OnceOK");
                worksheet.Cells[titleFirstOK_cell].Merge = true;
                worksheet.Cells[index + 1, 4].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.TargetYield");
                worksheet.Cells[titleFirstTargetYiled_cell].Merge = true;
                worksheet.Cells[index + 1, 5].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Yield");
                worksheet.Cells[titleFirstYiled_cell].Merge = true;
                worksheet.Cells[index + 1, 6].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Toptenbaddetails");
                worksheet.Cells[titleFirstTitle_cell].Merge = true;

                worksheet.Cells[index + 2, 6].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.Seq");
                worksheet.Cells[index + 2, 7].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Unpleasantsight");
                worksheet.Cells[index + 2, 8].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Pooramount");
                worksheet.Cells[index + 2, 9].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badrate");

                worksheet.Cells[titleFirstCheck_cell].Style.Font.Size = 11;
                worksheet.Cells[titleFirstOK_cell].Style.Font.Size = 11;
                worksheet.Cells[titleFirstTargetYiled_cell].Style.Font.Size = 11;
                worksheet.Cells[titleFirstYiled_cell].Style.Font.Size = 11;
                worksheet.Cells[titleFirstTitle_cell].Style.Font.Size = 11;

                SetExcelCellStyle(worksheet, titleFirstCheck_cell);
                SetExcelCellStyle(worksheet, titleFirstOK_cell);
                SetExcelCellStyle(worksheet, titleFirstTargetYiled_cell);
                SetExcelCellStyle(worksheet, titleFirstYiled_cell);
                SetExcelCellStyle(worksheet, titleFirstTitle_cell);

                string titleInput_cell = "J" + (index + 1).ToString() + ":" + "J" + (index + 2).ToString();
                string titleShipment_cell = "K" + (index + 1).ToString() + ":" + "K" + (index + 2).ToString();
                string titleSepcial_cell = "L" + (index + 1).ToString() + ":" + "L" + (index + 2).ToString();
                string titleSecondTargetYiled_cell = "M" + (index + 1).ToString() + ":" + "M" + (index + 2).ToString();
                string titleSecondYiled_cell = "N" + (index + 1).ToString() + ":" + "N" + (index + 2).ToString();
                string titleSecondTitle_cell = "O" + (index + 1).ToString() + ":" + "R" + (index + 1).ToString();

                worksheet.Cells[index + 1, 10].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Numberinputs");
                worksheet.Cells[titleInput_cell].Merge = true;
                worksheet.Cells[index + 1, 11].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Shipments");
                worksheet.Cells[titleShipment_cell].Merge = true;
                worksheet.Cells[index + 1, 12].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Specialmining");
                worksheet.Cells[titleSepcial_cell].Merge = true;
                worksheet.Cells[index + 1, 13].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.TargetYield");
                worksheet.Cells[titleSecondTargetYiled_cell].Merge = true;
                worksheet.Cells[index + 1, 14].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Yield");
                worksheet.Cells[titleSecondYiled_cell].Merge = true;
                worksheet.Cells[index + 1, 15].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Toptenbaddetails");
                worksheet.Cells[titleSecondTitle_cell].Merge = true;

                SetExcelCellStyle(worksheet, titleInput_cell);
                SetExcelCellStyle(worksheet, titleShipment_cell);
                SetExcelCellStyle(worksheet, titleSepcial_cell);
                SetExcelCellStyle(worksheet, titleSecondTargetYiled_cell);
                SetExcelCellStyle(worksheet, titleSecondYiled_cell);
                SetExcelCellStyle(worksheet, titleSecondTitle_cell);

                worksheet.Cells[titleInput_cell].Style.Font.Size = 11;
                worksheet.Cells[titleShipment_cell].Style.Font.Size = 11;
                worksheet.Cells[titleSepcial_cell].Style.Font.Size = 11;
                worksheet.Cells[titleSecondTargetYiled_cell].Style.Font.Size = 11;
                worksheet.Cells[titleSecondYiled_cell].Style.Font.Size = 11;
                worksheet.Cells[titleSecondTitle_cell].Style.Font.Size = 11;



                worksheet.Cells[index + 2, 15].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.Seq");
                worksheet.Cells[index + 2, 16].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Unpleasantsight");
                worksheet.Cells[index + 2, 17].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Pooramount");
                worksheet.Cells[index + 2, 18].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badrate");

                string titleNull_cell = "A" + index.ToString() + ":" + "A" + (index + 2).ToString();
                worksheet.Cells[titleNull_cell].Merge = true;
                worksheet.Cells[titleNull_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, titleNull_cell);

                #endregion
                List<QAReportExceptionTypeRankDTO> FirstRejectionRateTopTen = temp.FirstRejectionRateTopTen;
                List<QAReportExceptionTypeRankDTO> SecondRejectionRateTopTen = temp.SecondRejectionRateTopTen;

                int outlookIndex = 0;//外观index
                int sizeIndex = 0;//尺寸index
                int TypeExistsFlag = 0;

                #region 设置type title

                if (FirstRejectionRateTopTen.Exists(x => x.ExceptionType == "外观不良") || SecondRejectionRateTopTen.Exists(x => x.ExceptionType == "外观不良"))
                {
                    TypeExistsFlag = 1;
                    string typeTitle = "A" + (index + 3).ToString() + ":" + "A" + (index + 12).ToString();
                    worksheet.Cells[index + 3, 1].Value = sumData.Process + "_" + this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Exteriors");
                    worksheet.Cells[typeTitle].Merge = true;
                    SetExcelCellStyle(worksheet, typeTitle);

                    worksheet.Cells[typeTitle].Style.Font.Size = 11;
                    outlookIndex = index + 3;
                }

                if (FirstRejectionRateTopTen.Exists(x => x.ExceptionType == "尺寸不良") || SecondRejectionRateTopTen.Exists(x => x.ExceptionType == "尺寸不良"))
                {
                    TypeExistsFlag = TypeExistsFlag == 0 ? 2 : 3;

                    string typeTitle = "";
                    if (TypeExistsFlag == 3)
                    {
                        sizeIndex = index + 13;
                        typeTitle = "A" + (index + 13).ToString() + ":" + "A" + (index + 22).ToString();
                    }
                    else
                    {
                        sizeIndex = index + 3;
                        typeTitle = "A" + (index + 3).ToString() + ":" + "A" + (index + 12).ToString();
                    }

                    worksheet.Cells[sizeIndex, 1].Value = sumData.Process + "_" + this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Size");
                    worksheet.Cells[typeTitle].Merge = true;
                    worksheet.Cells[typeTitle].Style.Font.Size = 11;
                    SetExcelCellStyle(worksheet, typeTitle);
                }

                if (FirstRejectionRateTopTen.Count == 0 && SecondRejectionRateTopTen.Count == 0)
                {
                    string typeTitle = "A" + (index + 3).ToString() + ":" + "A" + (index + 12).ToString();
                    worksheet.Cells[index + 3, 1].Value = sumData.Process;
                    worksheet.Cells[typeTitle].Merge = true;
                    worksheet.Cells[typeTitle].Style.Font.Size = 11;
                    SetExcelCellStyle(worksheet, typeTitle);
                }

                #endregion

                int otherTitleEndIdnex = 0;
                if (TypeExistsFlag == 1 || TypeExistsFlag == 2 || TypeExistsFlag == 0)
                {
                    otherTitleEndIdnex = index + 12;
                }
                else if (TypeExistsFlag == 3)
                {
                    otherTitleEndIdnex = index + 22;
                }

                #region Sum Data

                string FirstCheckNum_cell = "B" + (index + 3).ToString() + ":" + "B" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[FirstCheckNum_cell].Merge = true;
                worksheet.Cells[index + 3, 2].Value = sumData.FirstCheck_Qty;

                worksheet.Cells[FirstCheckNum_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, FirstCheckNum_cell);

                string FirstOKNum_cell = "C" + (index + 3).ToString() + ":" + "C" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[FirstOKNum_cell].Merge = true;
                worksheet.Cells[index + 3, 3].Value = sumData.FirstOK_Qty;
                worksheet.Cells[FirstOKNum_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, FirstOKNum_cell);

                string FirstTargetYield_cell = "D" + (index + 3).ToString() + ":" + "D" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[FirstTargetYield_cell].Merge = true;
                worksheet.Cells[index + 3, 4].Value = sumData.FirstTargetYield.ToString("P");
                worksheet.Cells[FirstTargetYield_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, FirstTargetYield_cell);

                string FirstYield_cell = "E" + (index + 3).ToString() + ":" + "E" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[FirstYield_cell].Merge = true;
                worksheet.Cells[index + 3, 5].Value = sumData.FirstRejectionRate.ToString("P");
                worksheet.Cells[FirstYield_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, FirstYield_cell);

                string SecondInput_cell = "J" + (index + 3).ToString() + ":" + "J" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[SecondInput_cell].Merge = true;
                worksheet.Cells[index + 3, 10].Value = sumData.Input;
                worksheet.Cells[SecondInput_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, SecondInput_cell);

                string Shipment_cell = "K" + (index + 3).ToString() + ":" + "K" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[Shipment_cell].Merge = true;
                worksheet.Cells[index + 3, 11].Value = sumData.Shipment_Qty;
                worksheet.Cells[Shipment_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, Shipment_cell);

                string SepcialQty_cell = "L" + (index + 3).ToString() + ":" + "L" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[SepcialQty_cell].Merge = true;
                worksheet.Cells[index + 3, 12].Value = sumData.SepcialAccept_Qty;
                worksheet.Cells[SepcialQty_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, SepcialQty_cell);

                string SecondTargetYield_cell = "M" + (index + 3).ToString() + ":" + "M" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[SecondTargetYield_cell].Merge = true;
                worksheet.Cells[index + 3, 13].Value = sumData.SecondTargetYield.ToString("P");
                worksheet.Cells[SecondTargetYield_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, SecondTargetYield_cell);

                string SecondYield_cell = "N" + (index + 3).ToString() + ":" + "N" + (otherTitleEndIdnex).ToString();
                worksheet.Cells[SecondYield_cell].Merge = true;
                worksheet.Cells[index + 3, 14].Value = sumData.SecondRejectionRate.ToString("P");
                worksheet.Cells[SecondYield_cell].Style.Font.Size = 11;
                SetExcelCellStyle(worksheet, SecondYield_cell);

                #endregion

                int SecondoutlookIndex = outlookIndex;//外观index
                int SecondsizeIndex = sizeIndex;//尺寸index

                #region 一次 Top 10

                foreach (QAReportExceptionTypeRankDTO rank in FirstRejectionRateTopTen)
                {
                    if (rank.ExceptionType == "外观不良")
                    {
                        string Rankcell = "F" + outlookIndex.ToString();
                        worksheet.Cells[outlookIndex, 6].Value = rank.RankNum;
                        worksheet.Cells[Rankcell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rankcell);

                        string TypeName_cell = "G" + outlookIndex.ToString();
                        worksheet.Cells[outlookIndex, 7].Value = rank.TypeName;
                        worksheet.Cells[outlookIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[TypeName_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, TypeName_cell);

                        string Number_cell = "H" + outlookIndex.ToString();
                        worksheet.Cells[outlookIndex, 8].Value = rank.TotalCount;
                        worksheet.Cells[Number_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Number_cell);

                        string Rate_cell = "I" + outlookIndex.ToString();
                        worksheet.Cells[outlookIndex, 9].Value = rank.RejectionRate.ToString("P");
                        worksheet.Cells[Rate_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rate_cell);
                        outlookIndex++;
                    }
                    else
                    {
                        string Rankcell = "F" + sizeIndex.ToString();
                        worksheet.Cells[sizeIndex, 6].Value = rank.RankNum;
                        worksheet.Cells[Rankcell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rankcell);

                        string TypeName_cell = "G" + sizeIndex.ToString();
                        worksheet.Cells[sizeIndex, 7].Value = rank.TypeName;
                        worksheet.Cells[sizeIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[TypeName_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, TypeName_cell);

                        string Number_cell = "H" + sizeIndex.ToString();
                        worksheet.Cells[sizeIndex, 8].Value = rank.TotalCount;
                        worksheet.Cells[Number_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Number_cell);

                        string Rate_cell = "I" + sizeIndex.ToString();
                        worksheet.Cells[sizeIndex, 9].Value = rank.RejectionRate.ToString("P");
                        worksheet.Cells[Rate_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rate_cell);
                        sizeIndex++;
                    }
                }

                #endregion

                #region 二次 Top 10

                foreach (QAReportExceptionTypeRankDTO rank in SecondRejectionRateTopTen)
                {
                    if (rank.ExceptionType == "外观不良")
                    {
                        string Rankcell = "O" + SecondoutlookIndex.ToString();
                        worksheet.Cells[SecondoutlookIndex, 15].Value = rank.RankNum;
                        worksheet.Cells[Rankcell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rankcell);

                        string TypeName_cell = "P" + SecondoutlookIndex.ToString();
                        worksheet.Cells[SecondoutlookIndex, 16].Value = rank.TypeName;
                        worksheet.Cells[SecondoutlookIndex, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[TypeName_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, TypeName_cell);

                        string Number_cell = "Q" + SecondoutlookIndex.ToString();
                        worksheet.Cells[SecondoutlookIndex, 17].Value = rank.TotalCount;
                        worksheet.Cells[Number_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Number_cell);

                        string Rate_cell = "R" + SecondoutlookIndex.ToString();
                        worksheet.Cells[SecondoutlookIndex, 18].Value = rank.RejectionRate.ToString("P");
                        worksheet.Cells[Rate_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rate_cell);
                        SecondoutlookIndex++;
                    }
                    else
                    {
                        string Rankcell = "O" + SecondsizeIndex.ToString();
                        worksheet.Cells[SecondsizeIndex, 15].Value = rank.RankNum;
                        worksheet.Cells[Rankcell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rankcell);

                        string TypeName_cell = "P" + SecondsizeIndex.ToString();
                        worksheet.Cells[SecondsizeIndex, 16].Value = rank.TypeName;
                        worksheet.Cells[SecondsizeIndex, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[TypeName_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, TypeName_cell);

                        string Number_cell = "Q" + SecondsizeIndex.ToString();
                        worksheet.Cells[SecondsizeIndex, 17].Value = rank.TotalCount;
                        worksheet.Cells[Number_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Number_cell);

                        string Rate_cell = "R" + SecondsizeIndex.ToString();
                        worksheet.Cells[SecondsizeIndex, 18].Value = rank.RejectionRate.ToString("P");
                        worksheet.Cells[Rate_cell].Style.Font.Size = 11;
                        SetExcelCellStyle(worksheet, Rate_cell);
                        SecondsizeIndex++;
                    }
                }

                #endregion

                index = otherTitleEndIdnex + 1;

            }

            worksheet.Cells.AutoFitColumns();

        }

        private void SetExcelCellStyle(ExcelWorksheet worksheet, string localIndexCell)
        {

            worksheet.Cells[localIndexCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        private List<QAReportVM> GetIPQCReportForExportExcel(QAReportSearchVM search)
        {
            var apiUrl = string.Format("Quality/DownloadIPQCReportExcelAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var list = JsonConvert.DeserializeObject<List<QAReportVM>>(result);

            return list;
        }

        #endregion

        #endregion

        #region ---- QA反推良率分配界面
        public ActionResult QABackToFunPlant()
        {
            return View();
        }

        public ActionResult QueryQABackToFunPlantInfo(QAReportSearchVM search, Page page)
        {
            var apiUrl = string.Format("Quality/QueryQABackToFunPlantInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryQABTFInfoByUID(string QualityAssurance_DistributeRate_UID)
        {
            string apiUrl = string.Format("Quality/QueryQABTFInfoByUIDAPI?QualityAssurance_DistributeRate_UID={0}", QualityAssurance_DistributeRate_UID);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SaveBackToFunPlantInfo(string jsonWithProduct)
        {
            var apiUrl = "Quality/SaveBackToFunPlantInfoAPI";
            var entity = JsonConvert.DeserializeObject<QABackToFunPlantListVM>(jsonWithProduct);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult UpdateBackToFunPlantInfo(string jsonWithProduct)
        {
            var apiUrl = "Quality/UpdateBackToFunPlantInfoAPI";
            var entity = JsonConvert.DeserializeObject<QABackToFunPlant>(jsonWithProduct);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region    不良类型修改记录查询
        public ActionResult QAInputModifyQuery()
        {
            return View();
        }

        public ActionResult GetQAInputModify(QAInputModifySearch search, Page page)
        {
            var apiUrl = string.Format("Quality/GetQAInputModifyAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region ---- QA 目标良率维护


        public ActionResult TargetYield(int Flowchart_Master_UID)
        {

            ViewBag.Flowchart_Master_UID = Flowchart_Master_UID;

            var ddlapiUrl = string.Format("FlowChart/QueryFunPlantAPI?id={0}", Flowchart_Master_UID);
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


        public ActionResult QueryQATargetYield(int Flowchart_Master_UID, bool IsThisWork)
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

            string api = "Quality/QueryQATargetYieldAPI?Flowchart_Master_UID=" + Flowchart_Master_UID + "&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QueryQAProcessMGDataSingle(int Flowchart_Master_UID, int Flowchart_Detail_UID, bool IsThisWork)
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

            string api = "Quality/QueryQAProcessMGDataSingleAPI?Flowchart_Master_UID=" + Flowchart_Master_UID + "&&Flowchart_Detail_UID=" + Flowchart_Detail_UID + "&&date=" + date;
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult ModifySingleQaTargetYield(string jsonWithData)
        {
            var apiUrl = "Quality/ModifySingleQaTargetYieldAPI";
            var entity = JsonConvert.DeserializeObject<QATargetYieldVM>(jsonWithData);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        #region 良率维护模板下载

        public ActionResult DownloadPlanExcel(int FlowChart_Master_UID, string clintName)
        {

            var apiUrl = string.Format("Quality/QueryCheckPointForTargetYieldAPI?FlowChart_Master_UID={0}", FlowChart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<QATargetRateVM>>(result);
            if (list.Count() > 0)
            {
                var stream = new MemoryStream();
                var fileName = PathHelper.SetGridExportExcelName("QA_Target_Yield");
                string[] propertiesHead = new string[] { };
                switch (clintName)
                {
                    case "js_btn_download_fl":
                        propertiesHead = GetNextWeekPlanHeadColumn();
                        break;
                    case "js_btn_download_currentWK":
                        propertiesHead = GetCurrentWeekPlanHeadColumn();
                        break;
                }

                using (var excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("QA_Target_Yield");

                    SetExcelStyle(worksheet, propertiesHead);

                    int iRow = 3;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Process_seq;
                        worksheet.Cells[iRow, 2].Value = item.Process;

                        worksheet.Cells[iRow, 3].Value = item.Color;
                        worksheet.Cells[iRow, 18].Value = item.FlowChart_Master_UID;
                        worksheet.Cells[iRow, 19].Value = item.Flowchart_Detail_UID;
                        iRow++;
                    }
                    //Excel最后一行行号
                    int maxRow = iRow - 1;
                    //设置灰色背景
                    var colorRange = string.Format("A3:C{0}", maxRow);
                    worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    //设置主键列隐藏
                    worksheet.Column(18).Hidden = true;
                    worksheet.Column(19).Hidden = true;
                    //设置边框
                    worksheet.Cells[string.Format("A1:Q{0}", maxRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Q{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Q{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[string.Format("A1:Q{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
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

        private void SetExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 6].Value = propertiesHead[4];
            worksheet.Cells[1, 8].Value = propertiesHead[5];
            worksheet.Cells[1, 10].Value = propertiesHead[6];
            worksheet.Cells[1, 12].Value = propertiesHead[7];
            worksheet.Cells[1, 14].Value = propertiesHead[8];
            worksheet.Cells[1, 16].Value = propertiesHead[9];
            worksheet.Cells[1, 18].Value = propertiesHead[10];
            worksheet.Cells[1, 19].Value = propertiesHead[11];
            //合并单元格
            worksheet.Cells["D1:E1"].Merge = true;
            worksheet.Cells["F1:G1"].Merge = true;
            worksheet.Cells["H1:I1"].Merge = true;
            worksheet.Cells["J1:K1"].Merge = true;
            worksheet.Cells["L1:M1"].Merge = true;
            worksheet.Cells["N1:O1"].Merge = true;
            worksheet.Cells["P1:Q1"].Merge = true;

            //填充SubTitle内容
            for (int i = 4; i < 18; i++)
            {
                if (i % 2 == 0)
                {
                    worksheet.Cells[2, i].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Ayield");
                }
                else
                {
                    worksheet.Cells[2, i].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Secondaryyield");
                }
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            //设置百分比格式
            worksheet.Column(5).Style.Numberformat.Format = "0.00%";
            worksheet.Column(7).Style.Numberformat.Format = "0.00%";
            worksheet.Column(9).Style.Numberformat.Format = "0.00%";
            worksheet.Column(11).Style.Numberformat.Format = "0.00%";
            worksheet.Column(13).Style.Numberformat.Format = "0.00%";
            worksheet.Column(15).Style.Numberformat.Format = "0.00%";
            worksheet.Column(17).Style.Numberformat.Format = "0.00%";
            worksheet.Column(4).Style.Numberformat.Format = "0.00%";
            worksheet.Column(6).Style.Numberformat.Format = "0.00%";
            worksheet.Column(8).Style.Numberformat.Format = "0.00%";
            worksheet.Column(10).Style.Numberformat.Format = "0.00%";
            worksheet.Column(12).Style.Numberformat.Format = "0.00%";
            worksheet.Column(14).Style.Numberformat.Format = "0.00%";
            worksheet.Column(16).Style.Numberformat.Format = "0.00%";

            //设置列宽
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 17;
            for (int i = 3; i <= 17; i++)
            {
                worksheet.Column(i).Width = 12;
            }

            worksheet.Cells["A1:Q2"].Style.Font.Bold = true;
            worksheet.Cells["A1:Q2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:Q2"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }

        private string[] GetNextWeekPlanHeadColumn()
        {
            var nextWeek = GetNextWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Systemnumber"), // "制程序号",QA.Systemnumber
                this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Processname"), // "制程名称",QA.Processname
                this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Colour"), // "颜色",QA.Colour
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Mondays")+"({0})", nextWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),//QA.Mondays
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Tuesdays")+"({0})", nextWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),//QA.Tuesdays
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Wednesdays")+"({0})", nextWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),//QA.Wednesdays
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Thursdays")+"({0})", nextWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),//QA.Thursdays
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Fridays")+"({0})", nextWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),//QA.Fridays
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Saturdays")+"({0})", nextWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),//QA.Saturdays
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Sundays")+"({0})", nextWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),//QA.Sundays
                "FlowChart_Master_UID",
                "FlowChart_Detail_UID"
            };
            return propertiesHead;
        }

        private string[] GetCurrentWeekPlanHeadColumn()
        {
            var currentWeek = GetCurrentWeek(DateTime.Now);
            var propertiesHead = new[]
            {
                this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Systemnumber"), // "制程序号",QA.Systemnumber
                this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Processname"), // "制程名称",QA.Processname
                this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Colour"), // "颜色",QA.Colour       
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Mondays")+"({0})", currentWeek.Monday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Tuesdays")+"({0})", currentWeek.Tuesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Wednesdays")+"({0})", currentWeek.Wednesday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Thursdays")+"({0})",currentWeek.Thursday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Fridays")+"({0})", currentWeek.Friday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Saturdays")+"({0})", currentWeek.Saturday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Sundays")+"({0})", currentWeek.Sunday.Date.ToString(FormatConstants.DateTimeFormatStringByDate)),
                "FlowChart_Master_UID",
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

        #region 良率维护模板导入

        public string ImportPlanExcel(HttpPostedFileBase upload_excel, string hid_currentOrNextWeek)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet"); //"没有worksheet内容"; 
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] { };
                Week week = new Week();
                switch (hid_currentOrNextWeek)
                {
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
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Excelformat"); //"Excel格式不正确";
                    return errorInfo;
                }

                List<QualityAssurance_MgDataDTO> mgDataList = new List<QualityAssurance_MgDataDTO>();
                //Excel行号
                int iRow = 3;
                //Excel列号
                int iColumn;
                int j = 4;
                //如果Excel导入的计划是本周的计划，则导入的数据从当天的日期开始到往后导入到数据库中
                if (hid_currentOrNextWeek == "currentWeek")
                {
                    var strDT = DateTime.Now.DayOfWeek.ToString();
                    switch (strDT)
                    {
                        case "Monday":
                            j = 4;
                            break;
                        case "Tuesday":
                            j = 6;
                            break;
                        case "Wednesday":
                            j = 8;
                            break;
                        case "Thursday":
                            j = 10;
                            break;
                        case "Friday":
                            j = 12;
                            break;
                        case "Saturday":
                            j = 14;
                            break;
                        case "Sunday":
                            j = 16;
                            break;
                    }
                }

                for (iRow = 3; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }
                    for (iColumn = j; iColumn <= 17; iColumn++)
                    {
                        QualityAssurance_MgDataDTO newMGDataItem = new QualityAssurance_MgDataDTO();

                        var FlowChart_Master_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 18].Value);
                        var FlowChart_Detail_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 19].Value);
                        var ProcessSeq = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 1].Value);
                        var Process = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);

                        if (string.IsNullOrWhiteSpace(FlowChart_Master_UID) || string.IsNullOrWhiteSpace(FlowChart_Detail_UID))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Primarykeynovalue"), iRow); //QA.Primarykeynovalue
                            break;
                        }

                        var Product_Plan = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, iColumn].Value);
                        //如果生产计划为空则默认为0
                        if (string.IsNullOrWhiteSpace(Product_Plan))
                        {
                            Product_Plan = "0";

                        }

                        if (!ValidIsDouble(Product_Plan))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Inputincorrect"), iRow);//第{0}行一次良率输入不正确
                            break;
                        }

                        var Target_Yield = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ++iColumn].Value);
                        //如果目标良率为空则默认为0
                        if (string.IsNullOrWhiteSpace(Target_Yield))
                        {
                            Target_Yield = "0";

                        }
                        if (!ValidIsDouble(Target_Yield))
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.TInputincorrect"), iRow);// QA.TInputincorrect第{ 0} 行二次良率输入不正确
                            break;
                        }


                        newMGDataItem.FlowChart_Master_UID = Convert.ToInt32(FlowChart_Master_UID);
                        newMGDataItem.FlowChart_Detail_UID = Convert.ToInt32(FlowChart_Detail_UID);
                        switch (iColumn)
                        {
                            case 5:
                                newMGDataItem.ProductDate = week.Monday;
                                break;
                            case 7:
                                newMGDataItem.ProductDate = week.Tuesday;
                                break;
                            case 9:
                                newMGDataItem.ProductDate = week.Wednesday;
                                break;
                            case 11:
                                newMGDataItem.ProductDate = week.Thursday;
                                break;
                            case 13:
                                newMGDataItem.ProductDate = week.Friday;
                                break;
                            case 15:
                                newMGDataItem.ProductDate = week.Saturday;
                                break;
                            case 17:
                                newMGDataItem.ProductDate = week.Sunday;
                                break;
                        }

                        //四舍五入小数点
                        newMGDataItem.FirstRejectionRate = Convert.ToDecimal(Convert.ToDouble(Product_Plan).ToString("#0.0000"));

                        //四舍五入小数点
                        newMGDataItem.SecondRejectionRate = Convert.ToDecimal(Convert.ToDouble(Target_Yield).ToString("#0.0000"));
                        newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Modified_Date = DateTime.Now;
                        newMGDataItem.Create_Date = DateTime.Now;
                        newMGDataItem.Creator_UID = this.CurrentUser.AccountUId;
                        newMGDataItem.Process_seq = int.Parse(ProcessSeq);
                        mgDataList.Add(newMGDataItem);
                    }
                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                QAMgDataListVM data = new QAMgDataListVM();
                data.QAMgDataList = mgDataList;

                var json = JsonConvert.SerializeObject(data);
                string api = string.Format("Quality/ImportTargetYieldAPI");
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
                    case 4: //星期一
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, i - 1);
                        break;
                    case 6: //星期二
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 4);
                        break;
                    case 8: //星期三
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 5);
                        break;
                    case 10: //星期四
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 6);
                        break;
                    case 12: //星期五
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 7);
                        break;
                    case 14: //星期六
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 8);
                        break;
                    case 16: //星期日
                        allColumnsAreError = validExcelTitleIsError(worksheet, propertiesHead, iRow, i, 9);
                        break;
                    case 18:
                        if (ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, i].Value) != "FlowChart_Master_UID")
                        {
                            allColumnsAreError = true;
                        }
                        break;
                    case 19:
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




        #endregion

        #region  制程不良类型绑定
        public FileResult DownloadProcessExcel()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "BadTypeProcess_Template.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }

        public ActionResult ExcetetionTypeProcess(string projectName, int FlowChart_Detail_UID, int FlowChart_Master_UID, string processName)
        {
            ViewBag.projectName = projectName;

            ViewBag.FlowChart_Detail_UID = FlowChart_Detail_UID;
            ViewBag.FlowChart_Master_UID = FlowChart_Master_UID;
            ViewBag.ProcessName = processName;
            return View();
        }


        public string ImportExcepProcess(HttpPostedFileBase uploadName, string Project, int FlowChart_Detail_UID, int FlowChart_Master_UID)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(uploadName.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null)
                {

                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet");
                    return errorInfo;
                }

                //获取表头内容
                string[] propertiesHead = new string[] {
                     this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badtype"),
                     this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Functionfactory"),
                     this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Type"),
                };

                int iRow1 = 1;
                bool allColumnsAreEmpty = true;
                for (var i = 1; i <= propertiesHead.Length; i++)
                {
                    if (worksheet.Cells[iRow1, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow1, i].Value.ToString()))
                    {
                        allColumnsAreEmpty = false;
                        break;
                    }
                }
                if (allColumnsAreEmpty)
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Excelformat");
                    return errorInfo;
                }

                string ExceptionType = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow1, GetColumnIndex(propertiesHead, this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badtype"))].Value);

                string FuncPlant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow1, GetColumnIndex(propertiesHead, this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Functionfactory"))].Value);
                string type = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow1, GetColumnIndex(propertiesHead, this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Type"))].Value);

                if (string.IsNullOrWhiteSpace(ExceptionType) || string.IsNullOrWhiteSpace(type))
                {
                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Badtype") + "," + this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Type");
                    return errorInfo;
                }
                List<ExceptionTypeFlowChartVM> listTemp = new List<ExceptionTypeFlowChartVM>();
                ExceptionTypeProcessList mgDataList = new ExceptionTypeProcessList();
                //Excel行号
                int iRow = 2;
                //Excel列号
                //int iColumn;
                //int j = 1;

                bool flag = false;
                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    ExceptionTypeFlowChartVM newExTypeDataItem = new ExceptionTypeFlowChartVM();

                    var typeName = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 1].Value);
                    var funcPlant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);
                    var typeClassfy = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value);
                    if (string.IsNullOrWhiteSpace(typeName))
                    {
                        flag = true;

                        errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Typenovalue"), iRow);
                        break;
                    }
                    else if (string.IsNullOrWhiteSpace(typeClassfy))
                    {
                        flag = true;

                        errorInfo = string.Format(this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Nobadtype"), iRow);
                        break;
                    }

                    newExTypeDataItem.Creator_UID = this.CurrentUser.AccountUId;
                    newExTypeDataItem.Creator_Date = DateTime.Now;
                    newExTypeDataItem.TypeClassify = typeClassfy.Trim();
                    newExTypeDataItem.ExceptionType_Name = typeName.Trim();

                    newExTypeDataItem.FlowChart_Master_UID = FlowChart_Master_UID;
                    newExTypeDataItem.FlowChart_Detail_UID = FlowChart_Detail_UID;
                    newExTypeDataItem.FunPlant = funcPlant.Trim();
                    newExTypeDataItem.Project = Project.Trim();
                    listTemp.Add(newExTypeDataItem);
                }
                mgDataList.ExceptionTypeProcessLists = listTemp;
                if (flag)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("Quality/ImportExcepAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);

                var result = responMessage.Content.ReadAsStringAsync().Result;
                return result;
            }
            //return errorInfo;
        }

        public ActionResult DeleteExecProcess(int UID)
        {
            var apiUrl = string.Format("Quality/DeleteExecProcessAPI?uuid={0}", UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddExecProcess(string jsonBadType)
        {
            var apiUrl = "Quality/AddExecProcessAPI";
            var item = JsonConvert.DeserializeObject<ExceptionTypeFlowChartVM>(jsonBadType);

            item.Creator_UID = this.CurrentUser.AccountUId;
            item.Creator_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(item, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        public ActionResult DeleteAllExceptionProcess(int FlowChart_Detail_UID)
        {

            var apiUrl = string.Format("Quality/DeleteAllExceptionProcessAPI?FlowChart_Detail_UID={0}", FlowChart_Detail_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryExcepProcS(ExcepTypeFlowChartSearch search, Page page)
        {
            var apiUrl = string.Format("Quality/QueryExcepProcSAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #endregion

        #region  InputData导出

        private void SetTitelValue(ExcelWorksheet worksheet, int index, string StartSubTitleColumnCell, string StartColumnCell, string EndSubTitleColumnCell, string EndTitleColumnCell, int HorizontalIndex, string value, string title)
        {
            string titlecell = StartColumnCell + index.ToString() + ":" + EndSubTitleColumnCell + index.ToString();
            try
            {
                worksheet.Cells[titlecell].Merge = true;
            }
            catch
            {

            }
            worksheet.Cells[index, HorizontalIndex].Value = title;
            worksheet.Cells[titlecell].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[titlecell].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSeaGreen);
            SetExcelCellStyle(worksheet, titlecell);

            string valuecell = StartSubTitleColumnCell + index.ToString() + ":" + EndTitleColumnCell + index.ToString();
            worksheet.Cells[valuecell].Merge = true;
            worksheet.Cells[index, HorizontalIndex + 3].Value = value;
            SetExcelCellStyle(worksheet, valuecell);
        }

        #region IPQC 明细数据导出

        public FileResult ExportIPQCDailyInputDataExcel(string query)
        {
            IPQCDailyDataVM dailySumReport = new IPQCDailyDataVM();
            IPQCDailyDataVM NightSumReport = new IPQCDailyDataVM();
            IPQCDailyDataVM DayReport = new IPQCDailyDataVM();

            QAReportSearchVM search = JsonConvert.DeserializeObject<QAReportSearchVM>(query);


            #region 分别获取全天、白班、夜班的所有报表QA.Dayshift白班小计
            search.Tab_Select_Text = "白班小计";// this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Dayshift"); //白班小计;

            dailySumReport = GetIPQCInputDataForExportExcel(search);

            search.Tab_Select_Text = "夜班小计";// this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Nightshift"); // "夜班小计"; 
            NightSumReport = GetIPQCInputDataForExportExcel(search);

            search.Tab_Select_Text = "全天"; //this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Allday"); //全天; 
            DayReport = GetIPQCInputDataForExportExcel(search);
            #endregion

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheetDaily = excelPackage.Workbook.Worksheets.Add("daily");
                var worksheetNight = excelPackage.Workbook.Worksheets.Add("Night");
                var worksheetDay = excelPackage.Workbook.Worksheets.Add("24H");

                if (dailySumReport.DataList.Count > 0)
                {
                    WriteInputDataToExcel(worksheetDaily, dailySumReport.DataList,
                          this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Dayshifts"),

                        search.ProjectName, search.ProductDate.ToShortDateString(), search.MaterialType, search.Color);
                }
                if (NightSumReport.DataList.Count > 0)
                {

                    WriteInputDataToExcel(worksheetNight, NightSumReport.DataList,
                                this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Nightshifts"),
                        search.ProjectName, search.ProductDate.ToShortDateString(), search.MaterialType, search.Color);
                }
                if (DayReport.DataList.Count > 0)
                {

                    WriteInputDataToExcel(worksheetDay, DayReport.DataList, "24H", search.ProjectName, search.ProductDate.ToShortDateString(), search.MaterialType, search.Color);
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public void WriteInputDataToExcel(ExcelWorksheet worksheet, List<IPQCInputDataVM> data, string title, string Project, string ProductDate, string MaterialType, string Color)
        {
            int ColumnsCount = data.Count * 6;
            string endLetter = GetLetterByIndex(ColumnsCount);

            #region 设置标题信息

            string CellColumn = "A1:" + endLetter + "2";
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[CellColumn].Merge = true;
            worksheet.Cells[1, 1].Value = Project + " " + MaterialType + " " + (string.IsNullOrEmpty(Color) ? null : "(" + Color + ") ") + ProductDate + " " + title;

            worksheet.Cells[CellColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[CellColumn].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);

            worksheet.Cells[CellColumn].Style.Font.Size = 28;
            SetExcelCellStyle(worksheet, CellColumn);
            #endregion

            int HorizontalIndex = 1;

            foreach (IPQCInputDataVM temp in data)
            {
                QualityAssurance_InputMasterDTO sumData = temp.MasterData;
                List<QAInputDetailVM> DetailList = temp.DetailList;
                if (temp.DetailList.Count == 0)
                    continue;
                int VerticalIndex = 3;

                #region ------ Summary info
                int index = VerticalIndex;
                string StartColumnCell = GetLetterByIndex(HorizontalIndex);
                string EndTitleColumnCell = GetLetterByIndex(HorizontalIndex + 5);
                string EndSubTitleColumnCell = GetLetterByIndex(HorizontalIndex + 2);
                string StartSubTitleColumnCell = GetLetterByIndex(HorizontalIndex + 3);

                string titleProcesscell = StartColumnCell + index.ToString() + ":" + EndTitleColumnCell + index.ToString();
                try
                {
                    worksheet.Cells[titleProcesscell].Merge = true;
                }
                catch { }
                worksheet.Cells[index, HorizontalIndex].Value = sumData.Process;
                worksheet.Cells[index, HorizontalIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index, HorizontalIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Pink);
                worksheet.Cells[CellColumn].Style.Font.Size = 16;
                SetExcelCellStyle(worksheet, titleProcesscell);

                SetTitelValue(worksheet, index + 1, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.Input.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Numberinputs")); //"投入数"QA.Numberinputs
                SetTitelValue(worksheet, index + 2, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.FirstCheck_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Numbertests"));//一次检验数QA.Numbertests
                SetTitelValue(worksheet, index + 3, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.Shipment_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Shipments")); //QA.Shipments"出货数"
                SetTitelValue(worksheet, index + 4, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.FirstOK_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.OnceOK"));//QA.OnceOK"一次OK数"
                SetTitelValue(worksheet, index + 5, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.NG_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.NGnumber"));//QA.NGnumber"NG数"
                SetTitelValue(worksheet, index + 6, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, Convert.ToDouble(sumData.FirstRejectionRate).ToString("P"), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Abadrate"));//QA.Abadrate"一次不良率"
                SetTitelValue(worksheet, index + 7, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.SurfaceSA_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Exterior"));//QA.Exterior"外观特采数量"
                SetTitelValue(worksheet, index + 8, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.SizeSA_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Sizenumber"));//QA.Sizenumber "尺寸特采数量"
                SetTitelValue(worksheet, index + 9, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.RepairCheck_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Numberrework"));//QA.Numberrework "返修检验数"
                SetTitelValue(worksheet, index + 10, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.RepairOK_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Reworkreturned"));//QA.Reworkreturned"返修已回良品"
                SetTitelValue(worksheet, index + 11, StartSubTitleColumnCell, StartColumnCell, EndSubTitleColumnCell, EndTitleColumnCell, HorizontalIndex, sumData.Displace_Qty.ToString(), this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Replacement"));//QA.Replacement"置换数量"

                #endregion

                #region ------- Detail ExceptionTypes info

                worksheet.Cells[index + 13, HorizontalIndex].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Baddetail");// "不良明细"; QA.Baddetail
                worksheet.Cells[index + 13, HorizontalIndex + 1].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Rework"); //"返修"; QA.Rework
                worksheet.Cells[index + 13, HorizontalIndex + 2].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Specialmining"); //"特采"; QA.Specialmining
                worksheet.Cells[index + 13, HorizontalIndex + 3].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.ReworkNG"); //"返修NG"; QA.ReworkNG
                worksheet.Cells[index + 13, HorizontalIndex + 4].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.NG");// "NG"; QA.NG
                worksheet.Cells[index + 13, HorizontalIndex + 5].Value = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Replacements"); //"置换"; QA.Replacements

                worksheet.Cells[index + 13, HorizontalIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 13, HorizontalIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                worksheet.Cells[index + 13, HorizontalIndex + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 13, HorizontalIndex + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                worksheet.Cells[index + 13, HorizontalIndex + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 13, HorizontalIndex + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                worksheet.Cells[index + 13, HorizontalIndex + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 13, HorizontalIndex + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                worksheet.Cells[index + 13, HorizontalIndex + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 13, HorizontalIndex + 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                worksheet.Cells[index + 13, HorizontalIndex + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[index + 13, HorizontalIndex + 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                int detailIndex = index + 14;
                foreach (QAInputDetailVM DetailInfo in DetailList)
                {
                    worksheet.Cells[detailIndex, HorizontalIndex].Value = DetailInfo.ExceptionTypeName;
                    worksheet.Cells[detailIndex, HorizontalIndex + 1].Value = DetailInfo.Repair_Qty;
                    worksheet.Cells[detailIndex, HorizontalIndex + 2].Value = DetailInfo.SepcialAccept_Qty;
                    worksheet.Cells[detailIndex, HorizontalIndex + 3].Value = DetailInfo.RepairNG_Qty;
                    worksheet.Cells[detailIndex, HorizontalIndex + 4].Value = DetailInfo.NG_Qty;
                    worksheet.Cells[detailIndex, HorizontalIndex + 5].Value = DetailInfo.Displace_Qty;

                    detailIndex++;
                }
                #endregion

                HorizontalIndex = HorizontalIndex + 6;
            }

            worksheet.Cells.AutoFitColumns();
        }


        private IPQCDailyDataVM GetIPQCInputDataForExportExcel(QAReportSearchVM search)
        {
            var apiUrl = string.Format("Quality/DownloadIPQCInputDataForExportExcelAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<IPQCDailyDataVM>(result);
            return list;
        }

        public string GetLetterByIndex(int index)
        {


            Dictionary<int, string> LetterDic = new Dictionary<int, string>();
            LetterDic[1] = "A";
            LetterDic[2] = "B";
            LetterDic[3] = "C";
            LetterDic[4] = "D";
            LetterDic[5] = "E";
            LetterDic[6] = "F";
            LetterDic[7] = "G";
            LetterDic[8] = "H";
            LetterDic[9] = "I";
            LetterDic[10] = "J";
            LetterDic[11] = "K";
            LetterDic[12] = "L";
            LetterDic[13] = "M";
            LetterDic[14] = "N";
            LetterDic[15] = "O";
            LetterDic[16] = "P";
            LetterDic[17] = "Q";
            LetterDic[18] = "R";
            LetterDic[19] = "S";
            LetterDic[20] = "T";
            LetterDic[21] = "U";
            LetterDic[22] = "V";
            LetterDic[23] = "W";
            LetterDic[24] = "X";
            LetterDic[25] = "Y";
            LetterDic[26] = "Z";

            string result = "";
            string startLetter = "";
            string endLetter = "";

            int startIndex = 0;
            int endIndex = 0;
            if (index > 26)
            {
                startIndex = index / 26;
                endIndex = index % 26;
                if (endIndex == 0)
                {
                    endIndex = 1;
                }
            }
            else
            {
                endIndex = index;
            }

            if (LetterDic.Keys.Contains(startIndex))
            {
                startLetter = LetterDic[startIndex];
            }
            if (LetterDic.Keys.Contains(endIndex))
            {
                endLetter = LetterDic[endIndex];
            }
            result = startLetter + endLetter;

            return result;
        }

        #endregion



        #endregion

        #region  ---- QA 历史记录查询
        public ActionResult QAMasterHistory()
        {
            return View();
        }


        #endregion
    }
}