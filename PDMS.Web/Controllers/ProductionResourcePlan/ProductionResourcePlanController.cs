using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Web.Business.ProductiongPlanning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PDMS.Web.Controllers.ProductionResourcePlanController
{
    public class ProductionResourcePlanController : WebControllerBase
    {

        public ActionResult Index()
        {
            string OpType = "OP2";
            List<OrganiztionVM> userinfo = CurrentUser.GetUserInfo.OrgInfo;
            foreach (OrganiztionVM item in userinfo)
            {
                ViewBag.plant = item.Plant;
                OpType = item.OPType;
            }
            ViewBag.Opty = OpType;
            var apiUrl = string.Format("EventReportManager/GetProjectByOpAPI/?Op_Type={0}", OpType);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> Projects = js.Deserialize<List<string>>(result);
            ViewBag.Projects = Projects;

            apiUrl = string.Format("Equipmentmaintenance/GetEBoardLocationAPI/?optype={0}", OpType);
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            List<string> loctions = js.Deserialize<List<string>>(result);
            ViewBag.loctions = loctions;
            return View();
        }

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

        public ActionResult DepartureSchedulePlan()
        {
            FixtureVM currentVM = new FixtureVM();
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
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {
                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }

            var apiUrlWorkSchedule = string.Format("ProductionResourcePlan/GetWorkScheduleAPI");
            HttpResponseMessage WorkScheduleRes = APIHelper.APIGetAsync(apiUrlWorkSchedule);
            var resultWorkSchedule = WorkScheduleRes.Content.ReadAsStringAsync().Result;
            var WorkScheduleList = JsonConvert.DeserializeObject<List<string>>(resultWorkSchedule);
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            currentVM.WorkScheduleList = WorkScheduleList;
            return View("DepartureSchedulePlan", currentVM);
        }

        public ActionResult GetWorkSchedule()
        {
            var apiUrl = string.Format("ProductionResourcePlan/GetWorkScheduleAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取当前用户的功能厂
        /// </summary>
        /// <param name="plant_OrganizationUID"></param>
        /// <returns></returns>
        public ActionResult GetCurrentOPType(int plant_OrganizationUID)
        {
            int organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }

            var apiUrl = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", plant_OrganizationUID, organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 离职率/排版计划维护
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDRAWScheduleList(DRAWS_QueryParam searchParams, Page page)
        {
            var apiUrl = string.Format("ProductionResourcePlan/GetDSPlanListAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParams, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

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

            var apiUrl = string.Format("ProductionResourcePlan/QueryTurnoverSchedulingInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SaveDemissionInfo(DemissionRateAndWorkScheduleDTO dto)
        {
            //新加
            if (dto.DemissionRateAndWorkSchedule_UID == 0)
            {
                dto.Created_Date = DateTime.Now;
                dto.Created_UID = this.CurrentUser.AccountUId;
                dto.Modified_Date = DateTime.Now;
                dto.Modified_UID = this.CurrentUser.AccountUId;
              
            }
            else
            {
                dto.Modified_Date = DateTime.Now;
                dto.Modified_UID = this.CurrentUser.AccountUId;
            }

            dto.DemissionRate_NPI = dto.DemissionRate_NPI / 100;
            dto.DemissionRate_MP = dto.DemissionRate_MP / 100;
           
            ////判断是否是重复的值
            //var repeatList = demissionList.Where(p => p.Plant_Organization_UID == item.Plant_Organization_UID && p.BG_Organization_UID == item.BG_Organization_UID && p.Product_Date == item.Product_Date && p.Product_Phase == item.Product_Phase);
            //if (repeatList.Count() > 0)
            //{
            //    var model = repeatList.FirstOrDefault();
            //    errorInfo = string.Format("导入的Excel中厂区={0},OP={1},生产阶段={2},生产时间={3}", Plant_Organization, BG_Organization, model.Product_Phase, model.Product_Date);
            //    return errorInfo;
            //}

            var apiUrl = string.Format("ProductionResourcePlan/SaveDemissionInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetDemissionInfoByID(int demission_uid)
        {
            var apiUrl = string.Format("ProductionResourcePlan/GetDemissionInfoByIDAPI?demission_uid={0}", demission_uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 删除离职/排班维护
        /// </summary>
        /// <param name="demission_uid"></param>
        /// <returns></returns>
        public ActionResult DeleteDemissionInfoByID(int demission_uid)
        {
            var apiUrl = string.Format("ProductionResourcePlan/DeleteDemissionInfoByIDAPI?demission_uid={0}", demission_uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportTurnoverExcel(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            using (var xlPackage = new ExcelPackage(uploadName.InputStream))
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
                var propertiesHead = new[]
                {
                        "厂区",
                        "OP类型",
                        "生产日期",
                        "生产阶段",
                        "NPI离职率",
                        "NPI预计招募人力",
                        "MP离职率",
                        "MP预计招募人力",
                        "排班机制",
                    };

                //1 验证表头
                bool isExcelError = false;
                for (int i = 1; i <= totalColumns; i++)
                {
                    if (worksheet.Cells[1, i].Value != null && !string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Value.ToString()))
                    {
                        var resultsheet = worksheet.Cells[1, i].Value.ToString();
                        var hasItem = propertiesHead.FirstOrDefault(m => m.Contains(resultsheet));
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

                //获得厂区
                var plantsAPI = string.Format("Fixture/QueryAllPlantAPI?PLANT_UID={0}&leval={1}", GetPlantOrgUid(), "1");
                HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(plantsAPI);
                var jsonPlants = plantsmessage.Content.ReadAsStringAsync().Result;
                var plants = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonPlants);

                var demissionList = new List<DemissionRateAndWorkScheduleDTO>();
                for (int i = 2; i <= totalRows; i++)
                {
                    var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 1].Value);
                    var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 2].Value);
                    var Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 3].Value);
                    var productDate = DateTime.FromOADate(double.Parse(worksheet.Cells[i, 4].Value.ToString()));
                    var NPIRate = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 5].Value);
                    var NPIQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 6].Value);
                    var MPRate = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 7].Value);
                    var MPQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 8].Value);
                    var Schedule = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 9].Value);

                    var DemissionModel = new DemissionRateAndWorkScheduleDTO();
                    int Plant_Organization_UID = 0;
                    int BG_Organization_UID = 0;

                    #region  验证厂区 OP类型
                    // 1 验证厂区
                    if (string.IsNullOrWhiteSpace(Plant_Organization))
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行厂区代号没有值", i);
                        return errorInfo;
                    }
                    else
                    {
                        Plant_Organization = Plant_Organization.Trim();
                        var systemOrgDTO = plants.Where(m => m.Organization_Name == Plant_Organization).FirstOrDefault();

                        if (systemOrgDTO != null)
                        {
                            Plant_Organization_UID = systemOrgDTO.Organization_UID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                            return errorInfo;
                        }
                    }

                    //2 验证OP类型
                    if (string.IsNullOrWhiteSpace(BG_Organization))
                    {
                        isExcelError = true;
                        errorInfo = string.Format("第{0}行OP代号不能为空", i);
                        return errorInfo;
                    }
                    else
                    {
                        BG_Organization = BG_Organization.Trim();
                        var apiUrlAPI = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", Plant_Organization_UID, 1);
                        HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrlAPI);
                        var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                        var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);

                        var bg = optypes.Where(m => m.Organization_Name == BG_Organization).FirstOrDefault();

                        if (bg != null)
                        {
                            BG_Organization_UID = bg.Organization_UID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                            return errorInfo;
                        }
                    }
                    #endregion

                    #region 验证栏位
                    if (string.IsNullOrEmpty(Product_Phase))
                    {
                        errorInfo = string.Format("第[{0}]行生产阶段不能为空", i);
                        return errorInfo;
                    }

                    //Product_Date = Convert.ToDateTime(worksheet.Cells[iRow, 3].Value);
                    if (string.IsNullOrEmpty(productDate.ToString()))
                    {
                        errorInfo = string.Format("第[{0}]行生产日期不能为空", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(NPIRate))
                    {
                        errorInfo = string.Format("第[{0}]行NPI离职率不能为空", i);
                        return errorInfo;
                    }

                    decimal validDecimal = 0;
                    var isDecimal = decimal.TryParse(NPIRate, out validDecimal);
                    if (!isDecimal)
                    {
                        errorInfo = string.Format("第{0}行NPI离职率必须为数字", i);
                        return errorInfo;
                    }

                    //NPI预计招募人力
                    if (string.IsNullOrEmpty(NPIQty))
                    {
                        errorInfo = string.Format("第[{0}]行NPI预计招募人力不能为空", i);
                        return errorInfo;
                    }

                    int validInt = 0;
                    var isInt = int.TryParse(NPIQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行NPI预计招募人力必须为整数", i);
                        return errorInfo;
                    }


                    //MP离职率
                    if (string.IsNullOrEmpty(MPRate))
                    {
                        errorInfo = string.Format("第[{0}]行MP离职率不能为空", i);
                        return errorInfo;
                    }

                    isDecimal = decimal.TryParse(MPRate, out validDecimal);
                    if (!isDecimal)
                    {
                        errorInfo = string.Format("第{0}行MP离职率必须为数字", i);
                        return errorInfo;
                    }

                    //MP预计招募人力
                    if (string.IsNullOrEmpty(MPQty))
                    {
                        errorInfo = string.Format("第[{0}]行MP预计招募人力不能为空", i);
                        return errorInfo;
                    }
                    isInt = int.TryParse(MPQty, out validInt);
                    if (!isInt)
                    {
                        errorInfo = string.Format("第{0}行MP预计招募人力必须为整数", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(Schedule))
                    {
                        errorInfo = string.Format("第[{0}]行排班机制不能为空", i);
                        return errorInfo;
                    }

                    #endregion

                    DemissionRateAndWorkScheduleDTO item = new DemissionRateAndWorkScheduleDTO();
                    item.Plant_Organization_UID = Plant_Organization_UID;
                    item.BG_Organization_UID = BG_Organization_UID;
                    item.Product_Phase = Product_Phase;
                    item.Product_Date = productDate;
                    item.DemissionRate_NPI = Convert.ToDecimal(NPIRate)*100;
                    item.NPI_RecruitStaff_Qty = Convert.ToInt32(NPIQty);
                    item.DemissionRate_MP = Convert.ToDecimal(MPRate)*100;
                    item.MP_RecruitStaff_Qty = Convert.ToInt32(MPQty);
                    item.WorkSchedule = Schedule;
                    item.Created_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Modified_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Created_Date = DateTime.Now;
                    item.Modified_Date = DateTime.Now;
                    item.LanguageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;

                    //判断是否是重复的值
                    var repeatList = demissionList.Where(p => p.Plant_Organization_UID == item.Plant_Organization_UID && p.BG_Organization_UID == item.BG_Organization_UID && p.Product_Date == item.Product_Date && p.Product_Phase == item.Product_Phase);
                    if (repeatList.Count() > 0)
                    {
                        var model = repeatList.FirstOrDefault();
                        errorInfo = string.Format("导入的Excel中厂区={0},OP={1},生产阶段={2},生产时间={3}", Plant_Organization, BG_Organization, model.Product_Phase, model.Product_Date);
                        return errorInfo;
                    }
                    demissionList.Add(item);
                }

                if (demissionList.Distinct().Count() != totalRows - 1)
                {
                    errorInfo = "导入的Excel有重复行";
                    return errorInfo;
                }

                //检查数据库是否有重复
                string json = JsonConvert.SerializeObject(demissionList);
                var checkApiUrl = string.Format("ProductionResourcePlan/CheckImportTurnoverExcelAPI");
                HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(json, checkApiUrl);
                errorInfo = checkResponMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                if (!string.IsNullOrEmpty(errorInfo))
                {
                    return errorInfo;
                }

                var apiUrl = string.Format("ProductionResourcePlan/ImportTurnoverExcelAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
            }



            return string.Empty;
        }

        public ActionResult ExportDemissionRateInfo(DemissionRateAndWorkScheduleDTO search)
        {
            //get Export datas
            var apiUrl = "ProductionResourcePlan/ExportDemissionRateInfoAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<DemissionRateAndWorkScheduleDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("DemissionRateAndWorkSchedule");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] {
                        "序号",
                        "厂区",
                        "OP类型",
                        "生产日期",
                        "生产阶段",
                        "NPI离职率",
                        "NPI预计招募人力",
                        "MP离职率",
                        "MP预计招募人力",
                        "排班机制",
                };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("DemissionRateAndWorkSchedule");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Organization_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPType;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Product_Phase;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Product_Date.ToString("yyyy-MM-dd hh:ss");
                    worksheet.Cells[index + 2, 6].Value = (currentRecord.DemissionRate_NPI/100).ToString("p");
                    worksheet.Cells[index + 2, 7].Value = currentRecord.NPI_RecruitStaff_Qty;
                    worksheet.Cells[index + 2, 8].Value = (currentRecord.DemissionRate_MP/100).ToString("p"); ;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.MP_RecruitStaff_Qty;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.WorkSchedule;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult ExportDemissionRateInfoByID(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("ProductionResourcePlan/ExportDemissionRateInfoByIDAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<DemissionRateAndWorkScheduleDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("DemissionRateAndWorkSchedule");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] {
                        "序号",
                        "厂区",
                        "OP类型",
                        "生产阶段",
                        "生产日期",
                        "NPI离职率",
                        "NPI预计招募人力",
                        "MP离职率",
                        "MP预计招募人力",
                        "排班机制",
                };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("DemissionRateAndWorkSchedule");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Organization_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPType;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Product_Phase;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Product_Date.ToString("yyyy-MM-dd hh:ss");
                    worksheet.Cells[index + 2, 6].Value = (currentRecord.DemissionRate_NPI / 100).ToString("p");
                    worksheet.Cells[index + 2, 7].Value = currentRecord.NPI_RecruitStaff_Qty;
                    worksheet.Cells[index + 2, 8].Value = (currentRecord.DemissionRate_MP / 100).ToString("p"); ;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.MP_RecruitStaff_Qty;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.WorkSchedule;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }


        #endregion

        #region 现有人力管理
        public ActionResult Current_Manpower()
        {
            FixtureVM currentVM = new FixtureVM();
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
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {
                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("Current_Manpower", currentVM);

            //Dictionary<int, string> plantDir = new Dictionary<int, string>();
            //Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            //var apiUrl = string.Format("ProductionResourcePlan/GetProjectInfoByUserAPI");
            //HttpResponseMessage responMessage = APIHelper.APIPostAsync(this.CurrentUser.GetUserInfo, apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result;
            //var item = JsonConvert.DeserializeObject<ProductionPlanningReportGetProject>(result);
            //ViewBag.Plant = item.plantDir;
            //ViewBag.OPType = item.opTypeDir;
            //return View();
        }

        public ActionResult CurrentStaffInfo(CurrentStaffDTO dto, Page page)
        {
            var orgInfo = this.CurrentUser.GetUserInfo.OrgInfo;
            if (orgInfo.Count() == 0)
            {
                dto.Plant_Organization_UID = 0;
                dto.BG_Organization_UID = 0;
            }
            var apiUrl = string.Format("ProductionResourcePlan/QueryCurrentStaffInfoAPI");
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
                    string api = string.Format("ProductionResourcePlan/CheckImportCurrentStaffExcelAPI");
                    HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(json, api);
                    var checkResult = checkResponMessage.Content.ReadAsStringAsync().Result;
                    checkResult = checkResult.Replace("\"", "");
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        errorInfo = checkResult;
                        return errorInfo;
                    }
                }


                var apiUrl = string.Format("ProductionResourcePlan/ImportCurrentStaffInfoAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;

            }
            return string.Empty;
        }

        public ActionResult SaveStaffInfo(CurrentStaffDTO dto)
        {
            var apiUrl = string.Format("ProductionResourcePlan/SaveStaffInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return RedirectToAction("CurrentStaffList");
        }
        #endregion

        #region ME ------- Add By Wesley 2018/04/12
        /// <summary>
        /// 取得Quotation清單
        /// </summary>
        /// <param name="search">查詢條件集合</param>
        /// <param name="page">分頁參數</param>        
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult QueryMEs(RP_MESearch search, Page page)
        {
            search.Account_UID = this.CurrentUser.AccountUId;
            var apiUrl = string.Format("ProductionResourcePlan/QueryMEsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 取得Quotation清單
        /// </summary>
        /// <param name="search">查詢條件集合</param>
        /// <param name="page">分頁參數</param>        
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetME_Ds(int rP_Flowchart_Master_UID)
        {
            var apiUrl = string.Format("ProductionResourcePlan/GetME_DsAPI?rP_Flowchart_Master_UID={0}", rP_Flowchart_Master_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 取得ME主檔 Change History
        /// </summary>
        /// <param name="plant_Organization_UID">plant_Organization_UID</param>
        /// <param name="bG_Organization_UID">bG_Organization_UID</param>
        /// <param name="project_UID">project_UID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetME_ChangeHistory(int plant_Organization_UID, int bG_Organization_UID, int project_UID)
        {
            var apiUrl = string.Format("ProductionResourcePlan/GetME_ChangeHistoryAPI?plant_Organization_UID={0}&bG_Organization_UID={1}&project_UID={2}", plant_Organization_UID, bG_Organization_UID, project_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment清單
        /// </summary>
        /// <param name="search">搜尋條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetME_D_Equipments(ME_EquipmentSearchVM search, Page page)
        {
            var apiUrl = string.Format("ProductionResourcePlan/GetME_D_EquipmentsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetME_D_Equipment(string rP_Flowchart_Detail_ME_Equipment_UID)
        {
            var apiUrl = string.Format("ProductionResourcePlan/GetME_D_EquipmentAPI?rP_Flowchart_Detail_ME_Equipment_UID={0}", rP_Flowchart_Detail_ME_Equipment_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 保存設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <param name="wquipment_Manpower_Ratio">人力配比</param>
        /// <param name="wQP_Variable_Qty">設備變動數量</param>
        /// <param name="nPI_Current_Qty">NPI當前數量</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult SaveME_D_Equipment(SaveME_EquipmentVM vm)
        {
            vm.Account_UID = this.CurrentUser.AccountUId;
            var apiUrl = string.Format("ProductionResourcePlan/SaveME_D_EquipmentAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// ME 初始化資料
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductionPlanMeList()
        {
            ProductionPlanMeVM currentVM = new ProductionPlanMeVM();
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
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;

            ViewBag.PageTitle = "ME数据维护作业";
            return View("ProductionPlanMeList", currentVM);
            //return View();
        }

        /// <summary>
        /// ME Equipment 初始化資料
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductionPlanMe_Equipment(int RP_Flowchart_Master_UID)
        {
            ViewBag.RP_Flowchart_Master_UID = RP_Flowchart_Master_UID;
            return View();
        }

        /// <summary>
        /// 匯入 ME & 明細檔 & 設備明細檔
        /// </summary>
        /// <returns></returns>
        public string ImportExcel(HttpPostedFileBase uploadName, string FlowChart_Version_Comment, int FlowChart_Master_UID)
        {
            string errorInfo = string.Empty;
            bool isEdit = false;
            List<RP_VM> rP_VM = new List<RP_VM>();
            List<RP_ME_D_Equipment> equipmentList = new List<RP_ME_D_Equipment>();
            List<RP_ME_D_Equipment> autoEquipmentList = new List<RP_ME_D_Equipment>();
            List<RP_ME_D_Equipment> auxiliaryEquipList = new List<RP_ME_D_Equipment>();

            if (FlowChart_Master_UID > 0)
            {
                isEdit = true;
                errorInfo = new ProductiongPlanningImport().RP_ImportCheck(uploadName, FlowChart_Master_UID, FlowChart_Version_Comment, isEdit, rP_VM, equipmentList, autoEquipmentList, auxiliaryEquipList);
            }
            else
            {
                errorInfo = new ProductiongPlanningImport().RP_ImportCheck(uploadName, 0, FlowChart_Version_Comment, isEdit, rP_VM, equipmentList, autoEquipmentList, auxiliaryEquipList);
            }
            if (string.IsNullOrEmpty(errorInfo))
            {
                RP_All_VM all_VM = new RP_All_VM();
                all_VM.RP_VM = rP_VM;
                all_VM.ProcessingEquipList = equipmentList;
                all_VM.AutoEquipList = autoEquipmentList;
                all_VM.AuxiliaryEquipList = auxiliaryEquipList;
                all_VM.IsEdit = isEdit;
                all_VM.AccountID = this.CurrentUser.AccountUId;
                string api = "ProductionResourcePlan/ProductionPlanningAPI";
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(all_VM, api);
            }
            return errorInfo;
        }
        #endregion
    }
}