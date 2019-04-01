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
using System.Web.Helpers;
using System.Web;
using System.Linq;
using System.Text;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.Ajax.Utilities;
using PDMS.Model.EntityDTO;
using PDMS.Core.Authentication;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Web.Business.Flowchart;
using PDMS.Model.ViewModels.Fixture;

namespace PDMS.Web.Controllers
{
    public class FixturePartController : WebControllerBase
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
        //治具二期
        #region 治具配件主档
        public ActionResult FixturePartMaintenance()
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
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;

            ViewBag.PageTitle = "治具配件维护";
            return View("FixturePartMaintenance", currentVM);
        }

        /// <summary>
        /// 分页查询治具配件
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryFixtureParts(Fixture_PartModelSearch search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            var apiUrl = "Fixture/QueryFixturePartsAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 根据uid 查询治具配件
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult QueryFixturePart(int uid)
        {
            var apiUrl = string.Format("Fixture/QueryFixturePartAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 查询所有治具配件
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult QueryAllFixtureParts(Fixture_PartModelSearch search)
        {
            var apiUrl = "FixturePart/QueryAllFixturePartsAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddFixturePart(Fixture_PartDTO fixture_Part)
        {
            var isExistUrl = "Fixture/IsFixturePartExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(fixture_Part, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            //去掉前后空格
            fixture_Part.Part_ID = fixture_Part.Part_ID.Trim();
            fixture_Part.Part_Name = fixture_Part.Part_Name.Trim();
            fixture_Part.Part_Spec = fixture_Part.Part_Spec.Trim();

            var apiUrl = "Fixture/AddFixturePartAPI";
            var entity = fixture_Part;
            entity.Modified_UID = CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditFixturePart(Fixture_PartDTO fixturePart)
        {
            var isExistUrl = "Fixture/IsFixturePartExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(fixturePart, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/EditFixturePartAPI";
            var entity = fixturePart;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteFixturePart(int uid)
        {
            var apiUrl = "Fixture/DeleteFixturePartAPI";
            var entity = new Fixture_PartDTO() { Fixture_Part_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        public ActionResult DoExportFixturePart(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportFixturePartAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_PartDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Part");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具配件料号", "品名", "是否自动化配件", "是否标准件", "是否库存管控", "采购周期(天)", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Part");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Automation ? "1" : "0";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Standardized ? "1" : "0";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Storage_Managed ? "1" : "0";
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Purchase_Cycle;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 12].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult DoExportFixturePartByQuery(Fixture_PartModelSearch search)
        {
            //get Export datas
            var apiUrl = "Fixture/QueryFixturePartListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_PartDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Part");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具配件料号", "品名", "是否自动化配件", "是否标准件", "是否库存管控", "采购周期(天)", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Part");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Automation ? "1" : "0";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Standardized ? "1" : "0";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Storage_Managed ? "1" : "0";
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Purchase_Cycle;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 12].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        ////下载模板
        //public FileResult Process_Info()
        //{
        //    var filePath = Server.MapPath("~/ExcelTemplate/");
        //    var fullFileName = filePath + "Process_Info.xlsx";
        //    FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //    return fpr;
        //}

        /// <summary>
        /// 导入治具配件
        /// </summary>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public string ImportFixturePart(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            try
            {
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "治具配件料号",
                        "品名",
                        "规格",
                        "是否自动化配件",
                        "是否标准件",
                        "是否库存管控",
                        "采购周期(天)",
                        "是否启用"
                    };
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
                    //获得厂区
                    var plantsAPI = string.Format("Fixture/QueryAllPlantAPI?PLANT_UID={0}&leval={1}", GetPlantOrgUid(), "1");
                    HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(plantsAPI);
                    var jsonPlants = plantsmessage.Content.ReadAsStringAsync().Result;
                    var plants = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonPlants);

                    //获取所有工站
                    var fixtureDefectCodeAPI = "Fixture/QueryAllFixturePartsAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var workStations = JsonConvert.DeserializeObject<List<Fixture_PartDTO>>(jsonsVendorInfo);

                    var workStaionList = new List<Fixture_PartDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var fixturePart = new Fixture_PartDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;

                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        var partID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具配件料号")].Value);
                        var partName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                        var partSpec = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "规格")].Value);
                        var isAutomation = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否自动化配件")].Value);
                        var isStandardized = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否标准件")].Value);
                        var isStorageManaged = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否库存管控")].Value);
                        var purchaseCycle = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "采购周期(天)")].Value);
                        var isEnabled = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
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

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行OP代号不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            BG_Organization = BG_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", Plant_Organization_UID, 1);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
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

                        if (!string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            FunPlant_Organization = FunPlant_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetFunPlantByOPTypeAPI?Optype={0}", BG_Organization_UID);
                            HttpResponseMessage responMessageFunPlant = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessageFunPlant.Content.ReadAsStringAsync().Result;
                            var optypes = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(result2);

                            var bg = optypes.FirstOrDefault(m => m.FunPlant == FunPlant_Organization);

                            if (bg != null)
                            {
                                FunPlant_Organization_UID = bg.FunPlant_OrganizationUID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(partID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具配件料号没有值", i);
                            return errorInfo;
                        }
                        else if (partID.Length > 20)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具配件料号长度超过最大限定[20]", i);
                            return errorInfo;
                        }
                        partID = partID.Trim();

                        if (string.IsNullOrWhiteSpace(partName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行品名没有值", i);
                            return errorInfo;
                        }
                        else if (partName.Length > 50)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行品名长度超过最大限定[50]", i);
                            return errorInfo;
                        }
                        partName = partName.Trim();

                        if (string.IsNullOrWhiteSpace(partSpec))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行规格没有值", i);
                            return errorInfo;
                        }
                        else if (partSpec.Length > 50)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行规格长度超过最大限定[50]", i);
                            return errorInfo;
                        }
                        partSpec = partSpec.Trim();

                        if (string.IsNullOrWhiteSpace(isAutomation))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否自动化配件没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isAutomation = isAutomation.Trim();
                            if (isAutomation != "0" && isAutomation != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否自动化配件填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(isStandardized))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否标准件没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isStandardized = isStandardized.Trim();
                            if (isStandardized != "0" && isStandardized != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否标准件填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(isStorageManaged))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否库存管控没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isStorageManaged = isStorageManaged.Trim();
                            if (isStorageManaged != "0" && isStorageManaged != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否库存管控填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(purchaseCycle))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行采购周期没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            int purchaseCycleValue;
                            var parseSuccess = int.TryParse(purchaseCycle, out purchaseCycleValue);
                            if (!parseSuccess)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行采购周期请填写正整数", i);
                                return errorInfo;
                            }
                        }
                        partSpec = partSpec.Trim();

                        if (string.IsNullOrWhiteSpace(isEnabled))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isEnabled = isEnabled.Trim();
                            if (isEnabled != "0" && isEnabled != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }
                        //数据库判重,
                        var isDbRepeated = workStations.Exists(m => m.Part_ID == partID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具配件[{1}]已经存在,不可重复导入", i, partID);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = workStaionList.Exists(m => m.Part_ID == partID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具配件[{1}]在导入数据中重复", i, partID);
                            return errorInfo;
                        }

                        fixturePart.Plant_Organization_UID = Plant_Organization_UID;
                        fixturePart.BG_Organization_UID = BG_Organization_UID;
                        fixturePart.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        fixturePart.Part_ID = partID;
                        fixturePart.Part_Name = partName;
                        fixturePart.Part_Spec = partSpec;
                        fixturePart.Is_Automation = isAutomation == "1" ? true : false;
                        fixturePart.Is_Standardized = isStandardized == "1" ? true : false;
                        fixturePart.Is_Storage_Managed = isStorageManaged == "1" ? true : false;
                        fixturePart.Purchase_Cycle = int.Parse(purchaseCycle);
                        fixturePart.Is_Enable = isEnabled == "1" ? true : false;
                        fixturePart.Created_UID = CurrentUser.AccountUId;
                        fixturePart.Created_Date = DateTime.Now;
                        fixturePart.Modified_UID = CurrentUser.AccountUId;
                        fixturePart.Modified_Date = DateTime.Now;

                        workStaionList.Add(fixturePart);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(workStaionList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InserFixturePartsAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入工站失败：" + e.ToString();
            }
            return errorInfo;
        }
        #endregion
        #region 治具配件设定
        public ActionResult FixturePartSetting()
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
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;

            ViewBag.PageTitle = "治具配件设定";
            return View("FixturePartSetting", currentVM);
        }
        public ActionResult QueryFixturePartSettingMs(Fixture_Part_Setting_MModelSearch search, Page page)
        {
            if (search.Plant_Organization_UID == null || search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if ((search.BG_Organization_UID == null || search.BG_Organization_UID == 0) && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            var apiUrl = "FixturePart/QueryFixturePartSettingMsAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddFixturePartSetting(Fixture_Part_Setting_MDTO fixturePartSettingM, List<Fixture_Part_Setting_DDTO> fixturePartSettingDs)
        {
            //验证是否重复
            var isExistUrl = "FixturePart/IsFixturePartSettingMExistAPI";
            var search = new Fixture_Part_Setting_MModelSearch()
            {
                Plant_Organization_UID = fixturePartSettingM.Plant_Organization_UID,
                BG_Organization_UID = fixturePartSettingM.BG_Organization_UID,
                FunPlant_Organization_UID = fixturePartSettingM.FunPlant_Organization_UID,
                Fixture_NO = fixturePartSettingM.Fixture_NO
            };
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(search, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            //构造Fixture_Part_Setting_M
            fixturePartSettingM.Created_UID = CurrentUser.AccountUId;
            fixturePartSettingM.Created_Date = DateTime.Now;
            fixturePartSettingM.Modified_UID = CurrentUser.AccountUId;
            fixturePartSettingM.Modified_Date = DateTime.Now;
            //if (FunPlant_Organization_UID != 0)
            //{
            //    fixturePartSettingM.FunPlant_Organization_UID = FunPlant_Organization_UID;
            //}

            //构造Fixture_Part_Setting_D
            foreach (var item in fixturePartSettingDs)
            {
                item.Created_UID = CurrentUser.AccountUId;
                item.Created_Date = DateTime.Now;
                item.Modified_UID = CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
            }

            fixturePartSettingM.Fixture_Part_Setting_Ds = fixturePartSettingDs;

            var apiUrl = string.Format("FixturePart/AddFixturePartSettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(fixturePartSettingM, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Content(result);
        }
        public ActionResult EditFixturePartSetting(Fixture_Part_Setting_MDTO fixturePartSettingM, List<Fixture_Part_Setting_DDTO> fixturePartSettingDs)
        {
            //验证是否重复
            QueryModel<Fixture_Part_Setting_MModelSearch> queryModel = new QueryModel<Fixture_Part_Setting_MModelSearch>();
            queryModel.Equal = new Fixture_Part_Setting_MModelSearch()
            {
                Plant_Organization_UID = fixturePartSettingM.Plant_Organization_UID,
                BG_Organization_UID = fixturePartSettingM.BG_Organization_UID,
                FunPlant_Organization_UID = fixturePartSettingM.FunPlant_Organization_UID,
                Fixture_NO = fixturePartSettingM.Fixture_NO
            };
            queryModel.NotEqual = new Fixture_Part_Setting_MModelSearch()
            {
                Fixture_Part_Setting_M_UID = fixturePartSettingM.Fixture_Part_Setting_M_UID
            };
            var fixturePartSettingMAPI = "FixturePart/QueryFixturePartSettingMAPI";
            HttpResponseMessage settingMessage = APIHelper.APIPostAsync(queryModel, fixturePartSettingMAPI);
            var jsonSettingMList = settingMessage.Content.ReadAsStringAsync().Result;
            var existFixturePartSettingMListDb = JsonConvert.DeserializeObject<List<Fixture_Part_Setting_MDTO>>(jsonSettingMList);
            if (existFixturePartSettingMListDb.Count > 0)
            {
                return Content("EXIST");
            }

            //构造Fixture_Part_Setting_M
            fixturePartSettingM.Created_UID = CurrentUser.AccountUId;
            fixturePartSettingM.Created_Date = DateTime.Now;
            fixturePartSettingM.Modified_UID = CurrentUser.AccountUId;
            fixturePartSettingM.Modified_Date = DateTime.Now;
            //if (FunPlant_Organization_UID != 0)
            //{
            //    fixturePartSettingM.FunPlant_Organization_UID = FunPlant_Organization_UID;
            //}

            //构造Fixture_Part_Setting_D
            foreach (var item in fixturePartSettingDs)
            {
                item.Created_UID = CurrentUser.AccountUId;
                item.Created_Date = DateTime.Now;
                item.Modified_UID = CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
            }

            fixturePartSettingM.Fixture_Part_Setting_Ds = fixturePartSettingDs;

            var apiUrl = string.Format("FixturePart/EditFixturePartSettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(fixturePartSettingM, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Content(result);
        }
        public ActionResult GetFixturePartSettingMDTOByUID(int fixturePartSettingMUID)
        {
            var apiUrl = string.Format("FixturePart/GetFixturePartSettingMDTOByUIDAPI?fixturePartSettingMUID={0}", fixturePartSettingMUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DoAllExportFixturePartSetting(Fixture_Part_Setting_MModelSearch search)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            var apiUrl = string.Format("FixturePart/GetFixturePartSettingMByQueryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Part_Setting_MDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixturePartSetting");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具图号", "线体数量", "线体治具配比数量", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };

            var stringHeads1 = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具图号", "配件料号", "配比数量", "使用寿命(月)", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };

            //var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            //var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("治具配件设定");
                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                var worksheet1 = excelPackage.Workbook.Worksheets.Add("治具配件配比");
                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }


                int subIndex = 0;
                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Line_Qty;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Line_Fixture_Ratio_Qty;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Created_UserName;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_Date == null ? "" : ((DateTime)currentRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                    if (currentRecord.Fixture_Part_Setting_Ds != null && currentRecord.Fixture_Part_Setting_Ds.Count > 0)
                    {
                        for (int j = 0; j < currentRecord.Fixture_Part_Setting_Ds.Count; j++)
                        {
                            var subRecord = currentRecord.Fixture_Part_Setting_Ds[j];
                            //seq
                            worksheet1.Cells[subIndex + j + 2, 1].Value = subIndex + j + 1;
                            worksheet1.Cells[subIndex + j + 2, 2].Value = currentRecord.Plant_Organization_Name;
                            worksheet1.Cells[subIndex + j + 2, 3].Value = currentRecord.BG_Organization_Name;
                            worksheet1.Cells[subIndex + j + 2, 4].Value = currentRecord.FunPlant_Organization_Name;
                            worksheet1.Cells[subIndex + j + 2, 5].Value = currentRecord.Fixture_NO;
                            worksheet1.Cells[subIndex + j + 2, 6].Value = subRecord.Part_ID;
                            worksheet1.Cells[subIndex + j + 2, 7].Value = subRecord.Fixture_Part_Qty;
                            worksheet1.Cells[subIndex + j + 2, 8].Value = subRecord.Fixture_Part_Life;
                            worksheet1.Cells[subIndex + j + 2, 9].Value = subRecord.Is_Enable ? "1" : "0";
                            worksheet1.Cells[subIndex + j + 2, 10].Value = subRecord.Created_UserName;
                            worksheet1.Cells[subIndex + j + 2, 11].Value = subRecord.Created_Date == null ? "" : ((DateTime)subRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                            worksheet1.Cells[subIndex + j + 2, 12].Value = subRecord.Modified_UserName;
                            worksheet1.Cells[subIndex + j + 2, 13].Value = subRecord.Modified_Date == null ? "" : ((DateTime)subRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                        }
                        subIndex += currentRecord.Fixture_Part_Setting_Ds.Count;
                    }
                }

                //set cell value
                worksheet.Cells.AutoFitColumns();
                worksheet1.Cells.AutoFitColumns();

                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出选中的数据
        public ActionResult DoExportSelectedFixturePartSetting(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/DoExportFixtuerPartSettingMAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Part_Setting_MDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixturePartSetting");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具图号", "线体数量", "线体治具配比数量", "使用扫码间隔(小时)", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };

            var stringHeads1 = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具图号", "配件料号", "配比数量", "使用寿命(月)","线体治具配比数量","使用扫码间隔(小时)", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };

            //var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            //var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("治具配件设定");
                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                var worksheet1 = excelPackage.Workbook.Worksheets.Add("治具配件配比");
                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }


                int subIndex = 0;
                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Line_Qty;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Line_Fixture_Ratio_Qty;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.UseTimesScanInterval;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_UserName;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Created_Date == null ? "" : ((DateTime)currentRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                    if (currentRecord.Fixture_Part_Setting_Ds != null && currentRecord.Fixture_Part_Setting_Ds.Count > 0)
                    {
                        for (int j = 0; j < currentRecord.Fixture_Part_Setting_Ds.Count; j++)
                        {
                            var subRecord = currentRecord.Fixture_Part_Setting_Ds[j];
                            //seq
                            worksheet1.Cells[subIndex + j + 2, 1].Value = subIndex + j + 1;
                            worksheet1.Cells[subIndex + j + 2, 2].Value = currentRecord.Plant_Organization_Name;
                            worksheet1.Cells[subIndex + j + 2, 3].Value = currentRecord.BG_Organization_Name;
                            worksheet1.Cells[subIndex + j + 2, 4].Value = currentRecord.FunPlant_Organization_Name;
                            worksheet1.Cells[subIndex + j + 2, 5].Value = currentRecord.Fixture_NO;
                            worksheet1.Cells[subIndex + j + 2, 6].Value = subRecord.Part_ID;
                            worksheet1.Cells[subIndex + j + 2, 7].Value = subRecord.Fixture_Part_Qty;
                            worksheet1.Cells[subIndex + j + 2, 8].Value = subRecord.Fixture_Part_Life;
                            worksheet1.Cells[subIndex + j + 2, 9].Value = subRecord.IsUseTimesManagement ? "1" : "0";
                            worksheet1.Cells[subIndex + j + 2, 10].Value = subRecord.Fixture_Part_Life_UseTimes;
                            worksheet1.Cells[subIndex + j + 2, 11].Value = subRecord.Is_Enable ? "1" : "0";
                            worksheet1.Cells[subIndex + j + 2, 12].Value = subRecord.Created_UserName;
                            worksheet1.Cells[subIndex + j + 2, 13].Value = subRecord.Created_Date == null ? "" : ((DateTime)subRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                            worksheet1.Cells[subIndex + j + 2, 14].Value = subRecord.Modified_UserName;
                            worksheet1.Cells[subIndex + j + 2, 15].Value = subRecord.Modified_Date == null ? "" : ((DateTime)subRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                        }
                        subIndex += currentRecord.Fixture_Part_Setting_Ds.Count;
                    }
                }

                //set cell value
                worksheet.Cells.AutoFitColumns();
                worksheet1.Cells.AutoFitColumns();

                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult DeleteFixturPartSettingByUID(int uid)
        {
            var apiUrl = "FixturePart/DeleteFixturePartSettingByUIDAPI";
            var entity = new Fixture_Part_Setting_MDTO() { Fixture_Part_Setting_M_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public string ImportFixturePartSetting(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            try
            {
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "治具图号",
                        "线体数量",
                        "线体治具配比数量",
                        "使用扫码间隔(小时)",
                        "是否启用"
                    };
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
                    //获得厂区
                    var plantsAPI = string.Format("Fixture/QueryAllPlantAPI?PLANT_UID={0}&leval={1}", GetPlantOrgUid(), "1");
                    HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(plantsAPI);
                    var jsonPlants = plantsmessage.Content.ReadAsStringAsync().Result;
                    var plants = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonPlants);

                    //获取所有配件设定
                    var fixtureDefectCodeAPI = "FixturePart/QueryAllFixturePartSettingMsAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var settingMListDb = JsonConvert.DeserializeObject<List<Fixture_Part_Setting_MDTO>>(jsonsVendorInfo);

                    //for 循环中会更新厂区，OP，功能厂，若和之前不同则刷新治具列表
                    int Plant_Organization_old = 0;
                    int BG_Organization_old = 0;
                    int? FunPlant_Organization_old = null;
                    var fixtureListDb = new List<FixtureDTO>();

                    var settingMList = new List<Fixture_Part_Setting_MDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var fixturePartSettingM = new Fixture_Part_Setting_MDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        int? useTimesScanIntervalValue = null;

                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        var fixtureNO = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具图号")].Value);
                        var lineQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "线体数量")].Value);
                        var lineFixtureRatioQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "线体治具配比数量")].Value);
                        var useTimesScanInterval = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "使用扫码间隔(小时)")].Value);
                        var isEnable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);

                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行[厂区代号]没有值", i);
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
                                errorInfo = string.Format("第{0}行[厂区代号]的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行[OP代号]不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            BG_Organization = BG_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", Plant_Organization_UID, 1);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
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
                                errorInfo = string.Format("第{0}行[OP代号]的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            FunPlant_Organization = FunPlant_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetFunPlantByOPTypeAPI?Optype={0}", BG_Organization_UID);
                            HttpResponseMessage responMessageFunPlant = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessageFunPlant.Content.ReadAsStringAsync().Result;
                            var optypes = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(result2);

                            var bg = optypes.FirstOrDefault(m => m.FunPlant == FunPlant_Organization);

                            if (bg != null)
                            {
                                FunPlant_Organization_UID = bg.FunPlant_OrganizationUID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行[功能厂代号]的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        //获取所有治具
                        if (Plant_Organization_old != Plant_Organization_UID || BG_Organization_old != BG_Organization_UID || FunPlant_Organization_old != FunPlant_Organization_UID)
                        {
                            Plant_Organization_old = Plant_Organization_UID;
                            BG_Organization_old = BG_Organization_UID;
                            FunPlant_Organization_old = FunPlant_Organization_UID;
                            var fixtureListAPI = string.Format("Fixture/FixtureListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_old, BG_Organization_old, (FunPlant_Organization_old.HasValue ? FunPlant_Organization_old.Value : 0));
                            HttpResponseMessage fixtureListMessage = APIHelper.APIGetAsync(fixtureListAPI);
                            var jsonsFixtureList = fixtureListMessage.Content.ReadAsStringAsync().Result;
                            fixtureListDb = JsonConvert.DeserializeObject<List<FixtureDTO>>(jsonsFixtureList);
                            if (fixtureListDb.Count == 0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行无对应的[治具图号]", i);
                                return errorInfo;
                            }
                        }

                        //治具图号
                        if (string.IsNullOrWhiteSpace(fixtureNO))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行[治具图号]没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            fixtureNO = fixtureNO.Trim();
                            if (fixtureNO.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行[治具图号]{1}长度超过最大限定[30]", i, fixtureNO);
                                return errorInfo;
                            }
                            else if (!fixtureListDb.Exists(f => f.Fixture_NO == fixtureNO))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行[治具图号]{1}不存在", i, fixtureNO);
                                return errorInfo;
                            }
                        }

                        //线体数量
                        if (string.IsNullOrWhiteSpace(lineQty))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行[线体数量]没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            lineQty = lineQty.Trim();
                            int lineQtyValue;
                            var parseSuccess = int.TryParse(lineQty, out lineQtyValue);
                            if (!parseSuccess)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行[线体数量]请填写正整数", i);
                                return errorInfo;
                            }
                        }

                        //线体治具配比数量
                        if (string.IsNullOrWhiteSpace(lineQty))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行[线体治具]配比数量没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            lineFixtureRatioQty = lineFixtureRatioQty.Trim();
                            decimal lineFixtureRatioQtyValue;
                            var parseSuccess = decimal.TryParse(lineFixtureRatioQty, out lineFixtureRatioQtyValue);
                            if (!parseSuccess)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行[线体治具配比数量]请填写正整数", i);
                                return errorInfo;
                            }
                        }

                        //使用扫码间隔(小时)
                        if (!string.IsNullOrWhiteSpace(useTimesScanInterval) )
                        {
                            useTimesScanInterval = useTimesScanInterval.Trim();
                            int useTimesScanIntervalParseValue;
                            var parseSuccess = int.TryParse(useTimesScanInterval, out useTimesScanIntervalParseValue);
                            if (!parseSuccess)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行使用扫码间隔(小时)请填写非负整数", i);
                                return errorInfo;
                            }
                            else if (useTimesScanIntervalValue < 0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行使用扫码间隔(小时)请填写非负整数", i);
                                return errorInfo;
                            }
                            useTimesScanIntervalValue = useTimesScanIntervalParseValue;
                        }

                        //是否启用
                        if (string.IsNullOrWhiteSpace(isEnable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isEnable = isEnable.Trim();
                            if (isEnable != "0" && isEnable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }
                        //数据库判重,
                        var isDbRepeated = false;
                        if (FunPlant_Organization_UID != null || FunPlant_Organization_UID != 0)
                        {
                            isDbRepeated = settingMListDb.Exists(m => m.Fixture_NO == fixtureNO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                        }
                        else
                        {
                            isDbRepeated = settingMListDb.Exists(m => m.Fixture_NO == fixtureNO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        }

                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具配件[{1}]已经存在,不可重复导入", i, fixtureNO);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = false;
                        if (FunPlant_Organization_UID != null || FunPlant_Organization_UID != 0)
                        {
                            isSelfRepeated = settingMList.Exists(m => m.Fixture_NO == fixtureNO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                        }
                        else
                        {
                            isSelfRepeated = settingMList.Exists(m => m.Fixture_NO == fixtureNO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        }
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具配件[{1}]在导入数据中重复", i, fixtureNO);
                            return errorInfo;
                        }

                        fixturePartSettingM.Plant_Organization_UID = Plant_Organization_UID;
                        fixturePartSettingM.BG_Organization_UID = BG_Organization_UID;
                        fixturePartSettingM.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        fixturePartSettingM.Fixture_NO = fixtureNO;
                        fixturePartSettingM.Line_Qty = int.Parse(lineQty);
                        fixturePartSettingM.Line_Fixture_Ratio_Qty = decimal.Parse(lineFixtureRatioQty);
                        fixturePartSettingM.UseTimesScanInterval = useTimesScanIntervalValue;
                        fixturePartSettingM.Is_Enable = isEnable == "1" ? true : false;
                        fixturePartSettingM.Created_UID = CurrentUser.AccountUId;
                        fixturePartSettingM.Created_Date = DateTime.Now;
                        fixturePartSettingM.Modified_UID = CurrentUser.AccountUId;
                        fixturePartSettingM.Modified_Date = DateTime.Now;

                        settingMList.Add(fixturePartSettingM);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(settingMList);
                    var apiInsertVendorInfoUrl = string.Format("FixturePart/InsertFixturePartSettingMsAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入配件设定失败：" + e.ToString();
            }
            return errorInfo;
        }

        /// <summary>
        /// 导入配件配比
        /// </summary>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public string ImportFixturePartRatio(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            try
            {
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "治具图号",
                        "配件料号",
                        "品名",
                        "配比数量",
                        "使用寿命(月)",
                        "是否使用次数管控",
                        "使用寿命(次)",
                        "是否启用"
                    };
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
                    //获得厂区
                    var plantsAPI = string.Format("Fixture/QueryAllPlantAPI?PLANT_UID={0}&leval={1}", GetPlantOrgUid(), "1");
                    HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(plantsAPI);
                    var jsonPlants = plantsmessage.Content.ReadAsStringAsync().Result;
                    var plants = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonPlants);

                    //获取所有配件设定
                    var fixtureDefectCodeAPI = "FixturePart/QueryAllFixturePartSettingMsAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    //var settingMListDb = JsonConvert.DeserializeObject<List<Fixture_Part_Setting_MDTO>>(jsonsVendorInfo);

                    //for 循环中会更新厂区，OP，功能厂，若和之前不同则刷新治具列表
                    int Plant_Organization_old = 0;
                    int BG_Organization_old = 0;
                    int? FunPlant_Organization_old = null;
                    var fixtureListDb = new List<FixtureDTO>();
                    var fixturePartSettingMListDb = new List<Fixture_Part_Setting_MDTO>();
                    var fixtureParListDb = new List<Fixture_PartDTO>();

                    var settingDList = new List<Fixture_Part_Setting_DDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var fixturePartSettingD = new Fixture_Part_Setting_DDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;

                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        var fixtureNO = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具图号")].Value);
                        var partID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "配件料号")].Value);
                        var partName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                        var partQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "配比数量")].Value);
                        var partLife = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "使用寿命(月)")].Value);
                        var isUseTimesManagement = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否使用次数管控")].Value);
                        var partLifeUseTimes = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "使用寿命(次)")].Value);
                        var isEnable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);

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

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行OP代号不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            BG_Organization = BG_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", Plant_Organization_UID, 1);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
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

                        if (!string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            FunPlant_Organization = FunPlant_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetFunPlantByOPTypeAPI?Optype={0}", BG_Organization_UID);
                            HttpResponseMessage responMessageFunPlant = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessageFunPlant.Content.ReadAsStringAsync().Result;
                            var optypes = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(result2);

                            var bg = optypes.FirstOrDefault(m => m.FunPlant == FunPlant_Organization);

                            if (bg != null)
                            {
                                FunPlant_Organization_UID = bg.FunPlant_OrganizationUID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        //获取所有治具
                        if (Plant_Organization_old != Plant_Organization_UID || BG_Organization_old != BG_Organization_UID || FunPlant_Organization_old != FunPlant_Organization_UID)
                        {
                            Plant_Organization_old = Plant_Organization_UID;
                            BG_Organization_old = BG_Organization_UID;
                            FunPlant_Organization_old = FunPlant_Organization_UID;
                            var fixtureListAPI = string.Format("Fixture/FixtureListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_old, BG_Organization_old, (FunPlant_Organization_old.HasValue ? FunPlant_Organization_old.Value : 0));
                            HttpResponseMessage fixtureListMessage = APIHelper.APIGetAsync(fixtureListAPI);
                            var jsonsFixtureList = fixtureListMessage.Content.ReadAsStringAsync().Result;
                            fixtureListDb = JsonConvert.DeserializeObject<List<FixtureDTO>>(jsonsFixtureList);
                            if (fixtureListDb.Count == 0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行无对应的治具图号", i);
                                return errorInfo;
                            }
                        }



                        //治具图号
                        if (string.IsNullOrWhiteSpace(fixtureNO))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具图号没有值", i);
                            return errorInfo;
                        }
                        else 
                        {
                            fixtureNO = fixtureNO.Trim();
                            if (fixtureNO.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具图号{1}长度超过最大限定[30]", i, fixtureNO);
                                return errorInfo;
                            }
                        }

                        //查询有没对应的Fixture_Part_SettingM 记录
                        QueryModel<Fixture_Part_Setting_MModelSearch> queryModel = new QueryModel<Fixture_Part_Setting_MModelSearch>();
                        queryModel.Equal = new Fixture_Part_Setting_MModelSearch() { Plant_Organization_UID = Plant_Organization_UID, BG_Organization_UID = BG_Organization_UID, FunPlant_Organization_UID = FunPlant_Organization_UID, Fixture_NO = fixtureNO };
                        var fixturePartSettingMAPI = "FixturePart/QueryFixturePartSettingMAPI";
                        HttpResponseMessage settingMessage = APIHelper.APIPostAsync(queryModel, fixturePartSettingMAPI);
                        var jsonSettingMList = settingMessage.Content.ReadAsStringAsync().Result;
                        fixturePartSettingMListDb = JsonConvert.DeserializeObject<List<Fixture_Part_Setting_MDTO>>(jsonSettingMList);

                        if (fixturePartSettingMListDb.Count == 0)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具图号{1}未在治具配件设定中存在", i, fixtureNO);
                            return errorInfo;
                        }
                        var fixturePartSettingMDb = fixturePartSettingMListDb[0];


                        //配件料号,品名
                        if (string.IsNullOrWhiteSpace(partID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行配件料号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            partID = partID.Trim();
                            if (partID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行配件料号{1}长度超过最大限定[20]", i, partID);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(partName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行品名没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            partName = partName.Trim();
                            if (partName.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行品名{1}长度超过最大限定[50]", i, partName);
                                return errorInfo;
                            }
                        }
                        QueryModel<Fixture_PartModelSearch> fixturePartQueryModel = new QueryModel<Fixture_PartModelSearch>();
                        fixturePartQueryModel.Equal = new Fixture_PartModelSearch() { Plant_Organization_UID = Plant_Organization_UID, BG_Organization_UID = BG_Organization_UID, FunPlant_Organization_UID = FunPlant_Organization_UID, Part_ID = partID, Part_Name = partName };
                        var fixturePartAPI = "FixturePart/QueryFixturePartAPI";
                        HttpResponseMessage fixturePartMessage = APIHelper.APIPostAsync(fixturePartQueryModel, fixturePartAPI);
                        var jsonfixturePartList = fixturePartMessage.Content.ReadAsStringAsync().Result;
                        fixtureParListDb = JsonConvert.DeserializeObject<List<Fixture_PartDTO>>(jsonfixturePartList);
                        if (fixtureParListDb.Count == 0)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具料号{1} 和品名{2} 不存在存在", i, partID, partName);
                            return errorInfo;
                        }
                        var fixturePartDb = fixtureParListDb[0];

                        //配件数量
                        if (string.IsNullOrWhiteSpace(partQty))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行配件数量没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            partQty = partQty.Trim();
                            int lineQtyValue;
                            var parseSuccess = int.TryParse(partQty, out lineQtyValue);
                            if (!parseSuccess)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行配件数量请填写正整数", i);
                                return errorInfo;
                            }
                        }

                        //配件寿命
                        if (string.IsNullOrWhiteSpace(partQty))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行配件寿命没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            partLife = partLife.Trim();
                            decimal lineFixtureRatioQtyValue;
                            var parseSuccess = decimal.TryParse(partLife, out lineFixtureRatioQtyValue);
                            if (!parseSuccess)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行配件寿命请填写正整数", i);
                                return errorInfo;
                            }
                        }

                        //使用次数管控
                        if (string.IsNullOrWhiteSpace(isUseTimesManagement))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否使用次数管控没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isUseTimesManagement = isUseTimesManagement.Trim();
                            if (isUseTimesManagement != "0" && isUseTimesManagement != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否使用次数管控填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //配件使用次数
                        int? partLifeUseTimesValue = null;
                        if (!string.IsNullOrWhiteSpace(partLifeUseTimes))
                        {
                            partLifeUseTimes = partLifeUseTimes.Trim();
                            int partLifeUseTimesInt;
                            var parseSuccess = int.TryParse(partLifeUseTimes, out partLifeUseTimesInt);
                            if (!parseSuccess)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行配件寿命(次)请填写正整数", i);
                                return errorInfo;
                            }
                            else
                            {
                                partLifeUseTimesValue = partLifeUseTimesInt;
                            }
                        }

                        //是否启用
                        if (string.IsNullOrWhiteSpace(isEnable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            isEnable = isEnable.Trim();
                            if (isEnable != "0" && isEnable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库判重,
                        //var isDbRepeated = false;
                        //if (FunPlant_Organization_UID != null || FunPlant_Organization_UID != 0)
                        //{
                        //    isDbRepeated = settingMListDb.Exists(m => m.Fixture_NO == fixtureNO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                        //}
                        //else
                        //{
                        //    isDbRepeated = settingMListDb.Exists(m => m.Fixture_NO == fixtureNO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        //}
                        //if (isDbRepeated)
                        //{
                        //    isExcelError = true;
                        //    errorInfo = string.Format("第{0}行治具配件[{1}]已经存在,不可重复导入", i, fixtureNO);
                        //    return errorInfo;
                        //}

                        //导入数据判重
                        var isSelfRepeated = settingDList.Exists(m => m.Fixture_Part_Setting_M_UID == fixturePartSettingMDb.Fixture_Part_Setting_M_UID && m.Fixture_Part_UID == fixturePartDb.Fixture_Part_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行治具料号[{1}]在导入同一个治具配件设定时重复", i, partID);
                            return errorInfo;
                        }

                        fixturePartSettingD.Fixture_Part_Setting_M_UID = fixturePartSettingMDb.Fixture_Part_Setting_M_UID;
                        fixturePartSettingD.Fixture_Part_UID = fixturePartDb.Fixture_Part_UID;
                        fixturePartSettingD.Fixture_Part_Qty = int.Parse(partQty);
                        fixturePartSettingD.Fixture_Part_Life = decimal.Parse(partLife);
                        fixturePartSettingD.IsUseTimesManagement = isUseTimesManagement == "1" ? true : false;
                        fixturePartSettingD.Fixture_Part_Life_UseTimes = partLifeUseTimesValue;
                        fixturePartSettingD.Is_Enable = isEnable == "1" ? true : false;
                        fixturePartSettingD.Created_UID = CurrentUser.AccountUId;
                        fixturePartSettingD.Created_Date = DateTime.Now;
                        fixturePartSettingD.Modified_UID = CurrentUser.AccountUId;
                        fixturePartSettingD.Modified_Date = DateTime.Now;

                        settingDList.Add(fixturePartSettingD);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(settingDList);
                    var apiInsertVendorInfoUrl = string.Format("FixturePart/InsertOrUpdateFixturePartSettingDsAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入配件配比失败：" + e.ToString();
            }
            return errorInfo;
        }

        #endregion
        #region 治具配件需求管理
        public ActionResult FixturePartDemand()
        {
            var currentVM = new Fixture_Part_Demand_MVM();
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

            var enumerationQueryModel = new QueryModel<EnumerationModelSearch>();
            enumerationQueryModel.Equal = new EnumerationModelSearch() { Enum_Type = "StorageManage", Enum_Name = "Status_Demand" };
            var enumerationApiUrl = string.Format("Enumeration/QueryEnummertaionAPI");
            var enumerationResponMessage = APIHelper.APIPostAsync(enumerationQueryModel, enumerationApiUrl);
            var enumerationResult = enumerationResponMessage.Content.ReadAsStringAsync().Result;
            var status = JsonConvert.DeserializeObject<List<EnumerationDTO>>(enumerationResult);
            currentVM.StatusList = status;

            //ViewBag.PageTitle = "治具配件需求管理/审核";
            return View("FixturePartDemand", currentVM);
        }

        //查询治具配件需求主档
        public ActionResult QueryFixturePartDemandMs(Fixture_Part_Demand_MModelSearch search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            var apiUrl = "FixturePart/QueryQueryFixturePartDemandMsAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFixturePartDemandMByUID(int uid)
        {
            var apiUrl = string.Format("FixturePart/QueryFixturePartDemandMByUIDAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //新增配件需求计算
        public ActionResult AddFixturePartDemand(int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, DateTime Demand_Date)
        {
            //查询是否有需求日期重复的
            var queryModel = new QueryModel<Fixture_Part_Demand_MModelSearch>();
            queryModel.Equal = new Fixture_Part_Demand_MModelSearch() { Plant_Organization_UID = Plant_Organization_UID, BG_Organization_UID = BG_Organization_UID, FunPlant_Organization_UID = FunPlant_Organization_UID, Demand_Date = Demand_Date };
            var url = "FixturePart/QueryFixturePartDemandMAPI";
            HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(queryModel, url);
            var checkResult = checkResponMessage.Content.ReadAsStringAsync().Result;
            var existedDemandMList = JsonConvert.DeserializeObject<List<Fixture_Part_Demand_MDTO>>(checkResult);
            if (existedDemandMList.Count > 0)
            {
                return Content("EXIST");
            }

            //计算需求
            var applicantUID = CurrentUser.AccountUId;
            var apiUrl = string.Format("FixturePart/CalculateFixturePartDemandAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}&Demand_Date={3}&Applicant_UID={4}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Demand_Date, applicantUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //编辑需求计算明细表
        public ActionResult EditFixturePartDemandDs(string json)
        {
            var apiInsertVendorInfoUrl = string.Format("FixturePart/EditFixturePartDemandDsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Content(result, "application/json");
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="Fixture_Part_Demand_M_UID"></param>
        /// <returns></returns>
        public ActionResult SubmitFixturePartDemand(int Fixture_Part_Demand_M_UID)
        {
            var statusList = GetDemandStatusByName("待审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("FixturePart/UpdateFixturePartDemandMStatusAPI?uid={0}&statusUID={1}", Fixture_Part_Demand_M_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("提交失败");
        }

        /// <summary>
        /// 需求计算状态改为删除
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult DeleteFixturPartDemandByUID(int uid)
        {
            var statusList = GetDemandStatusByName("已删除");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("FixturePart/UpdateFixturePartDemandMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("删除失败");
        }

        /// <summary>
        /// 更新状态为已审核
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult ApproveFixturPartDemandByUID(int uid)
        {
            var statusList = GetDemandStatusByName("已审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                //调用存储过程汇总需求计算
                var submitOrderApi = string.Format("FixturePart/CalculateFixturePartDemandSummaryAPI?Fixture_Part_Demand_M_UID={0}&Applicant_UID={1}", uid, CurrentUser.AccountUId);
                HttpResponseMessage responMessageSubmitOrder = APIHelper.APIGetAsync(submitOrderApi);
                var resultSubmitOrder = responMessageSubmitOrder.Content.ReadAsStringAsync().Result.Replace("\"", "");
                if (resultSubmitOrder == "true")
                {
                    status = statusList[0];
                    //用枚举uid更新状态
                    var api = string.Format("FixturePart/UpdateFixturePartDemandMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                    HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                    var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    return Content(result, "application/json");
                }
                return Content("审核失败");
            }
            return Content("审核失败");
        }

        /// <summary>
        /// 配件需求审核取消
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult ApproveCancelFixturPartDemandByUID(int uid)
        {
            var statusList = GetDemandStatusByName("审核取消");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                var apiApproveCancel = string.Format("FixturePart/ApproveCancelFixturPartDemandAPI?uid={0}", uid);
                HttpResponseMessage responMessageApproveCancel = APIHelper.APIGetAsync(apiApproveCancel);
                var resultresponMessageApproveCancel = responMessageApproveCancel.Content.ReadAsStringAsync().Result.Replace("\"", "");

                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("FixturePart/UpdateFixturePartDemandMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("审核取消失败");
        }

        /// <summary>
        /// 通过需求计算状态名称获取状态列表
        /// </summary>
        /// <param name="statusName"></param>
        /// <returns></returns>
        private List<EnumerationDTO> GetDemandStatusByName(string statusName)
        {
            var enumerationQueryModel = new QueryModel<EnumerationModelSearch>();
            enumerationQueryModel.Equal = new EnumerationModelSearch() { Enum_Type = "StorageManage", Enum_Name = "Status_Demand", Enum_Value = statusName };
            var enumerationApiUrl = string.Format("Enumeration/QueryEnummertaionAPI");
            var enumerationResponMessage = APIHelper.APIPostAsync(enumerationQueryModel, enumerationApiUrl);
            var enumerationResult = enumerationResponMessage.Content.ReadAsStringAsync().Result;
            var statusList = JsonConvert.DeserializeObject<List<EnumerationDTO>>(enumerationResult);
            return statusList;
        }

        public ActionResult DoAllExportFixturePartDemandD(int Fixture_Part_Demand_M_UID, string Fixture_NO, string Part_ID, string Part_Name, string Part_Spec)
        {
            var apiUrl = string.Format("FixturePart/QueryFixturePartDemandMByUIDAPI?uid={0}", Fixture_Part_Demand_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var demandM = JsonConvert.DeserializeObject<Fixture_Part_Demand_MDTO>(result);

            var queryModel = new QueryModel<Fixture_Part_Demand_DModelSearch>();
            queryModel.Equal = new Fixture_Part_Demand_DModelSearch() { Fixture_Part_Demand_M_UID = Fixture_Part_Demand_M_UID };
            if (!string.IsNullOrEmpty(Fixture_NO))
            {
                queryModel.Like = new Fixture_Part_Demand_DModelSearch() { Fixture_NO = Fixture_NO };
            }
            var fixturePartSettingMAPI = "FixturePart/QueryFixtureDemandDAPI";
            HttpResponseMessage demandDMessage = APIHelper.APIPostAsync(queryModel, fixturePartSettingMAPI);
            var jsonDemandDList = demandDMessage.Content.ReadAsStringAsync().Result;
            var demandDList = JsonConvert.DeserializeObject<List<Fixture_Part_Demand_DDTO>>(jsonDemandDList);
            if (!string.IsNullOrWhiteSpace(Part_ID))
            {
                demandDList = demandDList.Where(i => i.Part_Name.Contains(Part_ID)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Part_Name))
            {
                demandDList = demandDList.Where(i => i.Part_Name.Contains(Part_Name)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Part_Spec))
            {
                demandDList = demandDList.Where(i => i.Part_Name.Contains(Part_Spec)).ToList();
            }

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixturePartDemand");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "厂区", "OP类型", "功能厂", "审核状态", "需求日期", "计算日期", "申请人", "申请日期", "审核人", "审核日期" };

            var stringHeads1 = new string[] { "序号", "治具图号", "配件料号", "配件品名", "配件型号", "线体数量", "线体治具配比", "治具数量", "使用寿命(月)", "毛需求量", "用户调整量", "采购周期", "安全库存", "是否删除" };

            //var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            //var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("配件需求主档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                worksheet.Cells[2, 1].Value = demandM.Plant_Organization_Name;
                worksheet.Cells[2, 2].Value = demandM.BG_Organization_Name;
                worksheet.Cells[2, 3].Value = demandM.FunPlant_Organization_Name;
                worksheet.Cells[2, 4].Value = demandM.StatusName;
                worksheet.Cells[2, 5].Value = demandM.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 6].Value = demandM.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 7].Value = demandM.Applicant_Name;
                worksheet.Cells[2, 8].Value = demandM.Applicant_Date.ToString(FormatConstants.DateTimeFormatString);
                worksheet.Cells[2, 9].Value = demandM.Approver_Name;
                worksheet.Cells[2, 10].Value = demandM.Approver_Date.ToString(FormatConstants.DateTimeFormatString);


                var worksheet1 = excelPackage.Workbook.Worksheets.Add("配件需求明细档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < demandDList.Count; index++)
                {
                    var currentRecord = demandDList[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Fixture_NO;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Part_ID;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Part_Name;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Part_Spec;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Line_Qty;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Line_Fixture_Ratio_Qty;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Fixture_Part_Qty;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Fixture_Part_Life;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Gross_Demand;
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.User_Adjustments_Qty;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.Purchase_Cycle;
                    worksheet1.Cells[index + 2, 13].Value = currentRecord.Safe_Storage_Qty;
                    worksheet1.Cells[index + 2, 14].Value = currentRecord.Is_Deleted == true ? "1" : "0";
                }
                worksheet1.Cells.AutoFitColumns();

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult DoExportSelectedFixturePartDemandD(int Fixture_Part_Demand_M_UID, string uids)
        {
            var apiUrl = string.Format("FixturePart/QueryFixturePartDemandMByUIDAPI?uid={0}", Fixture_Part_Demand_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var demandM = JsonConvert.DeserializeObject<Fixture_Part_Demand_MDTO>(result);

            var queryModel = new QueryModel<Fixture_Part_Demand_DModelSearch>();
            queryModel.Equal = new Fixture_Part_Demand_DModelSearch() { Fixture_Part_Demand_M_UID = Fixture_Part_Demand_M_UID };

            var fixturePartSettingMAPI = "FixturePart/QueryFixtureDemandDAPI";
            HttpResponseMessage demandDMessage = APIHelper.APIPostAsync(queryModel, fixturePartSettingMAPI);
            var jsonDemandDList = demandDMessage.Content.ReadAsStringAsync().Result;
            var demandDList = JsonConvert.DeserializeObject<List<Fixture_Part_Demand_DDTO>>(jsonDemandDList);
            var uidList = new List<int>();
            foreach (var item in uids.Split(','))
            {
                uidList.Add(int.Parse(item));
            }
            demandDList = demandDList.Where(i => uidList.Contains(i.Fixture_Part_Demand_D_UID)).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixturePartDemand");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "厂区", "OP类型", "功能厂", "审核状态", "需求日期", "计算日期", "申请人", "申请日期", "审核人", "审核日期" };

            var stringHeads1 = new string[] { "序号", "治具图号", "配件料号", "配件品名", "配件型号", "线体数量", "线体治具配比", "治具数量", "使用寿命(月)", "毛需求量", "用户调整量", "采购周期", "安全库存", "是否删除" };

            //var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            //var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("配件需求主档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                worksheet.Cells[2, 1].Value = demandM.Plant_Organization_Name;
                worksheet.Cells[2, 2].Value = demandM.BG_Organization_Name;
                worksheet.Cells[2, 3].Value = demandM.FunPlant_Organization_Name;
                worksheet.Cells[2, 4].Value = demandM.StatusName;
                worksheet.Cells[2, 5].Value = demandM.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 6].Value = demandM.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 7].Value = demandM.Applicant_Name;
                worksheet.Cells[2, 8].Value = demandM.Applicant_Date.ToString(FormatConstants.DateTimeFormatString);
                worksheet.Cells[2, 9].Value = demandM.Approver_Name;
                worksheet.Cells[2, 10].Value = demandM.Approver_Date.ToString(FormatConstants.DateTimeFormatString);


                var worksheet1 = excelPackage.Workbook.Worksheets.Add("配件需求明细档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < demandDList.Count; index++)
                {
                    var currentRecord = demandDList[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Fixture_NO;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Part_ID;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Part_Name;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Part_Spec;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Line_Qty;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Line_Fixture_Ratio_Qty;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Fixture_Part_Qty;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Fixture_Part_Life;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Gross_Demand;
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.User_Adjustments_Qty;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.Purchase_Cycle;
                    worksheet1.Cells[index + 2, 13].Value = currentRecord.Safe_Storage_Qty;
                    worksheet1.Cells[index + 2, 14].Value = currentRecord.Is_Deleted == true ? "1" : "0";
                }
                worksheet1.Cells.AutoFitColumns();

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion

        #region 治具配件需求汇总
        public ActionResult FixturePartDemandSummary()
        {
            var currentVM = new Fixture_Part_Demand_MVM();
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

            var enumerationQueryModel = new QueryModel<EnumerationModelSearch>();
            enumerationQueryModel.Equal = new EnumerationModelSearch() { Enum_Type = "StorageManage", Enum_Name = "Status_Demand" };
            var enumerationApiUrl = string.Format("Enumeration/QueryEnummertaionAPI");
            var enumerationResponMessage = APIHelper.APIPostAsync(enumerationQueryModel, enumerationApiUrl);
            var enumerationResult = enumerationResponMessage.Content.ReadAsStringAsync().Result;
            var status = JsonConvert.DeserializeObject<List<EnumerationDTO>>(enumerationResult);
            status = status.Where(i => i.Enum_Value != "待审核" && i.Enum_Value != "审核取消").ToList();
            currentVM.StatusList = status;

            //ViewBag.PageTitle = "治具配件需求管理/审核";
            return View("FixturePartDemandSummary", currentVM);
        }
        //查询治具配件需求主档
        public ActionResult QueryFixturePartDemandMSummarys(Fixture_Part_Demand_Summary_MModelSearch search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            var apiUrl = "FixturePart/QueryQueryFixturePartDemandMSummarysAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFixturePartDemandSummaryMByUID(int uid)
        {
            var apiUrl = string.Format("FixturePart/QueryFixturePartDemandSummaryMByUIDAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 更新状态为已审核
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult ApproveFixturPartDemandSummaryByUID(int uid)
        {
            var statusList = GetDemandStatusByName("已审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("FixturePart/UpdateFixturePartDemandSummaryMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("审核取消失败");
        }

        /// <summary>
        /// 配件需求汇总审核取消
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult ApproveCancelFixturPartDemandSummaryByUID(int uid)
        {
            var statusList = GetDemandStatusByName("审核取消");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("FixturePart/UpdateFixturePartDemandSummaryMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("审核取消失败");
        }

        /// <summary>
        /// 需求计算汇总状态更新为删除
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult DeleteFixturPartDemandSummaryByUID(int uid)
        {
            var statusList = GetDemandStatusByName("已删除");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("FixturePart/UpdateFixturePartDemandSummaryMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("删除失败");
        }

        /// <summary>
        /// 取消删除，需求计算汇总状态更新为未审核
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult CancelDeleteFixturPartDemandSummaryByUID(int uid)
        {
            var statusList = GetDemandStatusByName("未审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("FixturePart/UpdateFixturePartDemandSummaryMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("删除失败");
        }

        /// <summary>
        ///转采购，需求计算汇总状态更新为未审核
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult SubmitPurchaseFixturPartDemand(int uid)
        {
            var statusList = GetDemandStatusByName("已采购");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //调存储过程转采购，生成采购单
                var submitOrderApi = string.Format("FixturePart/FixturePartDemandSubmitOrder?demandSummaryMUID={0}&modifiedUID={1}", uid, CurrentUser.AccountUId);
                HttpResponseMessage responMessageSubmitOrder = APIHelper.APIGetAsync(submitOrderApi);
                var resultSubmitOrder = responMessageSubmitOrder.Content.ReadAsStringAsync().Result.Replace("\"", "");
                if (resultSubmitOrder == "true")
                {
                    //用枚举uid更新状态
                    var api = string.Format("FixturePart/UpdateFixturePartDemandSummaryMStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
                    HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                    var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    return Content(result, "application/json");
                }
                return Content("转采购失败");
            }
            return Content("转采购失败");
        }

        //编辑需求计算明细表
        public ActionResult EditFixturePartDemandSummaryDs(string json)
        {
            var apiInsertVendorInfoUrl = string.Format("FixturePart/EditFixturePartDemandSummaryDsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return Content(result, "application/json");
        }

        public ActionResult DoAllExportFixturePartDemandSummaryD(int Fixture_Part_Demand_Summary_M_UID, string Part_ID, string Part_Name, string Part_Spec)
        {
            var apiUrl = string.Format("FixturePart/QueryFixturePartDemandSummaryMByUIDAPI?uid={0}", Fixture_Part_Demand_Summary_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var demandM = JsonConvert.DeserializeObject<Fixture_Part_Demand_Summary_MDTO>(result);

            var queryModel = new QueryModel<Fixture_Part_Demand_Summary_DModelSearch>();
            queryModel.Equal = new Fixture_Part_Demand_Summary_DModelSearch() { Fixture_Part_Demand_Summary_M_UID = Fixture_Part_Demand_Summary_M_UID };
            var fixturePartSettingMAPI = "FixturePart/QueryFixtureDemandSummaryDAPI";
            HttpResponseMessage demandDMessage = APIHelper.APIPostAsync(queryModel, fixturePartSettingMAPI);
            var jsonDemandDList = demandDMessage.Content.ReadAsStringAsync().Result;
            var demandDList = JsonConvert.DeserializeObject<List<Fixture_Part_Demand_Summary_DDTO>>(jsonDemandDList);
            if (!string.IsNullOrWhiteSpace(Part_ID))
            {
                demandDList = demandDList.Where(i => i.Part_Name.Contains(Part_ID)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Part_Name))
            {
                demandDList = demandDList.Where(i => i.Part_Name.Contains(Part_Name)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Part_Spec))
            {
                demandDList = demandDList.Where(i => i.Part_Name.Contains(Part_Spec)).ToList();
            }

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixturePartDemandSummary");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "厂区", "OP类型", "功能厂", "审核状态", "需求日期", "计算日期", "申请人", "申请日期", "审核人", "审核日期" };

            var stringHeads1 = new string[] { "序号", "配件料号", "配件品名", "配件型号", "库存量", "毛需求总量", "用户调整总量", "实际需求量", "安全库存", "是否删除" };

            //var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            //var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("配件需求汇总主档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                worksheet.Cells[2, 1].Value = demandM.Plant_Organization_Name;
                worksheet.Cells[2, 2].Value = demandM.BG_Organization_Name;
                worksheet.Cells[2, 3].Value = demandM.FunPlant_Organization_Name;
                worksheet.Cells[2, 4].Value = demandM.StatusName;
                worksheet.Cells[2, 5].Value = demandM.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 6].Value = demandM.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 7].Value = demandM.Applicant_Name;
                worksheet.Cells[2, 8].Value = demandM.Applicant_Date.ToString(FormatConstants.DateTimeFormatString);
                worksheet.Cells[2, 9].Value = demandM.Approver_Name;
                worksheet.Cells[2, 10].Value = demandM.Approver_Date.ToString(FormatConstants.DateTimeFormatString);


                var worksheet1 = excelPackage.Workbook.Worksheets.Add("配件需求汇总明细档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < demandDList.Count; index++)
                {
                    var currentRecord = demandDList[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Part_ID;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Part_Name;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Part_Spec;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Storage_Qty;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Total_Gross_Demand;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Total_User_Adjustments_Qty;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Total_Actual_Demand;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Safe_Storage_Qty;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Is_Deleted == true ? "1" : "0";
                }
                worksheet1.Cells.AutoFitColumns();

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult DoExportSelectedFixturePartDemandSummaryD(int Fixture_Part_Demand_Summary_M_UID, string uids)
        {
            var apiUrl = string.Format("FixturePart/QueryFixturePartDemandSummaryMByUIDAPI?uid={0}", Fixture_Part_Demand_Summary_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var demandM = JsonConvert.DeserializeObject<Fixture_Part_Demand_Summary_MDTO>(result);

            var queryModel = new QueryModel<Fixture_Part_Demand_Summary_DModelSearch>();
            queryModel.Equal = new Fixture_Part_Demand_Summary_DModelSearch() { Fixture_Part_Demand_Summary_M_UID = Fixture_Part_Demand_Summary_M_UID };
            var fixturePartSettingMAPI = "FixturePart/QueryFixtureDemandSummaryDAPI";
            HttpResponseMessage demandDMessage = APIHelper.APIPostAsync(queryModel, fixturePartSettingMAPI);
            var jsonDemandDList = demandDMessage.Content.ReadAsStringAsync().Result;
            var demandDList = JsonConvert.DeserializeObject<List<Fixture_Part_Demand_Summary_DDTO>>(jsonDemandDList);
            List<int> uidList = new List<int>();
            foreach (var item in uids.Split(','))
            {
                uidList.Add(int.Parse(item));
            }
            demandDList = demandDList.Where(i => uidList.Contains(i.Fixture_Part_Demand_Summary_D_UID)).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixturePartDemandSummary");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "厂区", "OP类型", "功能厂", "审核状态", "需求日期", "计算日期", "申请人", "申请日期", "审核人", "审核日期" };

            var stringHeads1 = new string[] { "序号", "配件料号", "配件品名", "配件型号", "库存量", "毛需求总量", "用户调整总量", "实际需求量", "安全库存", "是否删除" };

            //var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            //var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("配件需求汇总主档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                worksheet.Cells[2, 1].Value = demandM.Plant_Organization_Name;
                worksheet.Cells[2, 2].Value = demandM.BG_Organization_Name;
                worksheet.Cells[2, 3].Value = demandM.FunPlant_Organization_Name;
                worksheet.Cells[2, 4].Value = demandM.StatusName;
                worksheet.Cells[2, 5].Value = demandM.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 6].Value = demandM.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                worksheet.Cells[2, 7].Value = demandM.Applicant_Name;
                worksheet.Cells[2, 8].Value = demandM.Applicant_Date.ToString(FormatConstants.DateTimeFormatString);
                worksheet.Cells[2, 9].Value = demandM.Approver_Name;
                worksheet.Cells[2, 10].Value = demandM.Approver_Date.ToString(FormatConstants.DateTimeFormatString);


                var worksheet1 = excelPackage.Workbook.Worksheets.Add("配件需求汇总明细档");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < demandDList.Count; index++)
                {
                    var currentRecord = demandDList[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Part_ID;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Part_Name;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Part_Spec;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Storage_Qty;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Total_Gross_Demand;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Total_User_Adjustments_Qty;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Total_Actual_Demand;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Safe_Storage_Qty;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Is_Deleted == true ? "1" : "0";
                }
                worksheet1.Cells.AutoFitColumns();

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion

        #region 仓库管理
        public ActionResult FixturePartWarehouse()
        {
            FixturePartWarehouseVM currentVM = new FixturePartWarehouseVM();
            var apiUrl = string.Format("FixturePart/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;
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
                apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
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
            return View(currentVM);
        }
        /// <summary>
        /// 获取初始化列表数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryWarehouseStorageInfo(FixturePartWarehouseDTO search, Page page)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("FixturePart/QueryWarehouseStoragesAPI");
            var responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 根据OP获取功能厂
        /// </summary>
        /// <param name="opuid"></param>
        /// <returns></returns>
        public ActionResult QueryFunplantByop(int opuid)
        {
            var apiUrl = string.Format("FixturePart/QueryFunplantByopAPI?opuid={0}", opuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryWarehouseByuid(int Warehouse_UID)
        {
            var apiUrl = string.Format("FixturePart/QueryWarehouseByuidAPI?Warehouse_UID={0}", Warehouse_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditWarehouseInfo(string jsonStorages)
        {
            var apiUrl = string.Format("FixturePart/AddOrEditWarehouseInfoAPI");
            var entity = JsonConvert.DeserializeObject<FixturePartWarehouseStorages>(jsonStorages);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Created_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteWarehouseStorage(int Warehouse_Storage_UID)
        {
            var apiUrl = string.Format("FixturePart/DeleteWarehouseStorageAPI?WareStorage_UId={0}", Warehouse_Storage_UID);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public string DeleteWarehouse(int Warehouse_UID)
        {
            var apiUrl = string.Format("FixturePart/DeleteWarehouseAPI?Warehouse_UID={0}", Warehouse_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string ImportEQPWareHouseExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = ImportEQPWareHouseExcelM(upload_excel);
            return errorInfo;
        }
        private string[] GetEQPWarehouseHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "仓库代码",
                "仓库名称",
                "说明",
                "是否启用"
            };
            return propertiesHead;
        }
        private string ImportEQPWareHouseExcelM(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            try
            {
                int PlantOrgUid = GetPlantOrgUid();
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
                    var propertiesHead = GetEQPWarehouseHeadColumn();
                    bool excelIsError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[1, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[1, i].Value.ToString()))
                        {
                            var result1 = worksheet.Cells[1, i].Value.ToString();
                            var hasItem = propertiesHead.Where(m => m.Contains(result1)).FirstOrDefault();
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
                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                    //获取所有仓库
                    var warehouseStAPI = string.Format("FixturePart/QueryAllWarehouseStAPI?Plant_Organization_UID={0}", PlantOrgUid);
                    HttpResponseMessage WarhouseStmessage = APIHelper.APIGetAsync(warehouseStAPI);
                    var jsonwarst = WarhouseStmessage.Content.ReadAsStringAsync().Result;
                    var warehousest = JsonConvert.DeserializeObject<List<FixturePartWarehouseDTO>>(jsonwarst);

                    List<FixturePartWarehouseDTO> repairlocationlist = new List<FixturePartWarehouseDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        FixturePartWarehouseDTO RL = new FixturePartWarehouseDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        string Plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value).Trim();
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        string WarehouseCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库代码")].Value);
                        string WarehouseName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库名称")].Value);
                        string Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "说明")].Value);
                        string Is_Use = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Plant))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.Plant == Plant).FirstOrDefault();
                            if (hasbg != null)
                            {
                                Plant_Organization_UID = hasbg.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.BG == BG_Organization).FirstOrDefault();
                            if (hasbg != null)
                            {
                                BG_Organization_UID = hasbg.BG_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            //excelIsError = true;
                            //errorInfo = string.Format("第{0}行没有功能厂", i);
                            //return errorInfo;
                        }
                        else
                        {

                            var hasfunplant = orgboms.Where(m => m.BG == BG_Organization & m.Funplant == FunPlant_Organization).FirstOrDefault();
                            if (hasfunplant != null)
                            {
                                FunPlant_Organization_UID = hasfunplant.Funplant_UID;
                            }
                            else
                            {
                                //excelIsError = true;
                                //errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                //return errorInfo;
                            }
                        }


                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else if (Is_Enable != "0" && Is_Enable != "1")
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                            return errorInfo;
                        }


                        if (string.IsNullOrWhiteSpace(WarehouseCode))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库代码没有值", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(WarehouseName))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库名称没有值", i);
                            return errorInfo;
                        }

                        var HasWarehouseCode = warehousest.FirstOrDefault(m => m.Fixture_Warehouse_ID == WarehouseCode && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (HasWarehouseCode != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库代码已经存在,不可重复导入", i);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = repairlocationlist.Exists(m => m.Fixture_Warehouse_ID == WarehouseCode && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isSelfRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行导入数据中重复,不可重复导入", i);
                            return errorInfo;
                        }
                        RL.Plant_Organization_UID = Plant_Organization_UID;
                        RL.BG_Organization_UID = BG_Organization_UID;
                        RL.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        RL.Fixture_Warehouse_ID = WarehouseCode;
                        RL.Fixture_Warehouse_Name = WarehouseName;
                        RL.Remarks = Desc;
                        RL.Is_Enable = Is_Enable == "1" ? true : false;
                        RL.Created_Date = DateTime.Now;
                        RL.Created_UID = CurrentUser.AccountUId;
                        RL.Modified_UID = CurrentUser.AccountUId;
                        RL.Modified_Date = DateTime.Now;
                        repairlocationlist.Add(RL);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(repairlocationlist);
                    var apiInsertVendorInfoUrl = string.Format("FixturePart/InsertWarehouseAPI");
                    HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage1.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.ToString();
            }
            return errorInfo;
        }

        public string ImportEQPStorageExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = ImportEQPStorageExcelM(upload_excel);
            return errorInfo;
        }
        private string[] GetEQPStorageHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "仓库代码",
                "料架号",
                "储位号",
                "说明",
                "是否启用"
            };
            return propertiesHead;
        }
        private string ImportEQPStorageExcelM(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            try
            {
                int PlantOrgUid = GetPlantOrgUid();
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
                    var propertiesHead = GetEQPStorageHeadColumn();
                    bool excelIsError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[1, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[1, i].Value.ToString()))
                        {
                            var result = worksheet.Cells[1, i].Value.ToString();
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
                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                    //获得所有仓库代码
                    var warehouseStAPI = string.Format("FixturePart/QueryAllWarehouseStAPI?Plant_Organization_UID={0}", PlantOrgUid);
                    HttpResponseMessage warehouseStMessage = APIHelper.APIGetAsync(warehouseStAPI);
                    var jsonwarehouseresult = warehouseStMessage.Content.ReadAsStringAsync().Result;
                    var Warehouses = JsonConvert.DeserializeObject<List<FixturePartWarehouseDTO>>(jsonwarehouseresult);

                    //获取所有的储位信息
                    var warehouseStorageAPI = string.Format("FixturePart/GetFixtureWarehouseStorageALLAPI?Plant_Organization_UID={0}", PlantOrgUid);
                    HttpResponseMessage warehouseStorageAPIMessage = APIHelper.APIGetAsync(warehouseStorageAPI);
                    var jsonwarehouseStorageresult = warehouseStorageAPIMessage.Content.ReadAsStringAsync().Result;
                    var warehouseStorage = JsonConvert.DeserializeObject<List<FixturePartWarehouseDTO>>(jsonwarehouseStorageresult);

                    List<FixturePartWarehouseDTO> repairlocationlist = new List<FixturePartWarehouseDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        FixturePartWarehouseDTO RL = new FixturePartWarehouseDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        string Plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value).Trim();
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        string warehouseCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库代码")].Value);
                        string Rack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料架号")].Value);
                        string Storage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "储位号")].Value);
                        string Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "说明")].Value);
                        string Is_Use = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);

                        if (string.IsNullOrWhiteSpace(Plant))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区没有值", i);
                            return errorInfo;
                        }
                        else
                        {


                            var hasbg = orgboms.Where(m => m.Plant == Plant).FirstOrDefault();
                            if (hasbg != null)
                            {
                                Plant_Organization_UID = hasbg.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.BG == BG_Organization).FirstOrDefault();
                            if (hasbg != null)
                            {
                                BG_Organization_UID = hasbg.BG_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        int wareHouseUID = 0;
                        if (string.IsNullOrWhiteSpace(warehouseCode))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            //判断该仓库代码是否在数据库中
                            var hasbg = Warehouses.Where(m => m.Fixture_Warehouse_ID == warehouseCode && m.BG_Organization_UID == BG_Organization_UID && m.Plant_Organization_UID == Plant_Organization_UID).FirstOrDefault();
                            if (hasbg != null)
                            {
                                wareHouseUID = hasbg.Fixture_Warehouse_UID;
                                FunPlant_Organization_UID = hasbg.FunPlant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行的仓库代码没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Rack_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有料架号", i);
                            return errorInfo;
                        }
                        if (string.IsNullOrWhiteSpace(Storage_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有储位号", i);
                            return errorInfo;
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else if (Is_Enable != "0" && Is_Enable != "1")
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                            return errorInfo;
                        }
                        var HasWarehouseCode = warehouseStorage.FirstOrDefault(m => m.Fixture_Warehouse_UID == wareHouseUID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Rack_ID == Rack_ID && m.Storage_ID == Storage_ID);
                        if (HasWarehouseCode != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行储位已经存在,不可重复导入", i);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = repairlocationlist.Exists(m => m.Fixture_Warehouse_UID == wareHouseUID && m.Rack_ID == Rack_ID && m.Storage_ID == Storage_ID);
                        if (isSelfRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行导入数据中重复,不可重复导入", i);
                            return errorInfo;
                        }

                        RL.Plant_Organization_UID = Plant_Organization_UID;
                        RL.BG_Organization_UID = BG_Organization_UID;
                        RL.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        RL.Fixture_Warehouse_UID = wareHouseUID;
                        RL.Rack_ID = Rack_ID;
                        RL.Storage_ID = Storage_ID;
                        RL.Remarks = Desc;
                        RL.Is_Enable = Is_Enable == "1" ? true : false;
                        RL.Created_Date = DateTime.Now;
                        RL.Created_UID = CurrentUser.AccountUId;
                        RL.Modified_UID = CurrentUser.AccountUId;
                        RL.Modified_Date = DateTime.Now;
                        repairlocationlist.Add(RL);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(repairlocationlist);
                    var apiInsertVendorInfoUrl = string.Format("FixturePart/InsertWarehouse_StorageAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.ToString();
            }
            return errorInfo;
        }

        public ActionResult DoAllExportWarehouseReprot(FixturePartWarehouseDTO search)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("FixturePart/DoAllExportWarehouseReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePartWarehouseDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Warehouse");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "仓库代码", "仓库名称", "仓库说明", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };

            var stringHeads1 = new string[] { "序号", "厂区", "OP类型", "仓库代码", "料架号", "储位号", "储位说明", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };

            var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Warehouse");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < warehouselist.Count; index++)
                {
                    var currentRecord = warehouselist[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Remarks;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Createder;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_Date == null ? "" : ((DateTime)currentRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                }

                var worksheet1 = excelPackage.Workbook.Worksheets.Add("WarehouseStorage");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < warehouseStoragelist.Count; index++)
                {
                    var currentRecord = warehouseStoragelist[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Remarks;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Is_Enable;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Createder;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Created_Date == null ? "" : ((DateTime)currentRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet1.Cells.AutoFitColumns();

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportWarehouseReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/DoExportWarehouseReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePartWarehouseDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Warehouse");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "仓库代码", "仓库名称", "仓库说明", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };

            var stringHeads1 = new string[] { "序号", "厂区", "OP类型", "仓库代码", "料架号", "储位号", "储位说明", "是否启用", "创建人员", "创建日期", "修改人员", "修改日期" };


            var warehouselist = list.Where(o => o.Rack_ID == null || o.Rack_ID == "").ToList();
            var warehouseStoragelist = list.Where(o => o.Rack_ID != null && o.Rack_ID != "").ToList();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Warehouse");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < warehouselist.Count; index++)
                {
                    var currentRecord = warehouselist[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Remarks;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Createder;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_Date == null ? "" : ((DateTime)currentRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                }

                var worksheet1 = excelPackage.Workbook.Worksheets.Add("WarehouseStorage");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < warehouseStoragelist.Count; index++)
                {
                    var currentRecord = warehouseStoragelist[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Remarks;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Is_Enable;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Createder;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Created_Date == null ? "" : ((DateTime)currentRecord.Created_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet1.Cells.AutoFitColumns();

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion
        #region 出入库维护作业
        public ActionResult FixturePartUpdatebound()
        {
            FixturePartCreateboundVM currentVM = new FixturePartCreateboundVM();
            var apiUrlStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessageStatus = APIHelper.APIGetAsync(apiUrlStatus);
            var resultStatus = responMessageStatus.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultStatus);
            currentVM.FixturePartCreateboundStatus = fixtureStatus;

            var apiUrlIn_outStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI?Enum_Type={0}", "FixturePartIn_out_Type");
            HttpResponseMessage responMessageIn_outStatus = APIHelper.APIGetAsync(apiUrlIn_outStatus);
            var resultIn_outStatus = responMessageIn_outStatus.Content.ReadAsStringAsync().Result;
            var fixtureIn_outStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultIn_outStatus);
            currentVM.FixturePartInOutboundStatus = fixtureIn_outStatus;

            var apiUrlOutStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI?Enum_Type={0}", "FixturePartOutbound_Type");
            HttpResponseMessage responMessageOutStatus = APIHelper.APIGetAsync(apiUrlOutStatus);
            var resultOuttStatus = responMessageOutStatus.Content.ReadAsStringAsync().Result;
            var fixtureOutStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultOuttStatus);
            currentVM.FixturePartOutboundStatus = fixtureOutStatus.Where(o => o.Status != "盘点出库" && o.Status != "料品移转出库单").ToList();

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
            return View(currentVM);

        }

        public ActionResult GetFixture_Repair_MDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetFixture_Repair_MDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixturePartDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixturePartDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetWarehouseStorageByFixtureUID(int Fixture_Part_UID)
        {
            var apiUrl = string.Format("FixturePart/GetWarehouseStorageByFixtureUIDAPI?Fixture_Part_UID={0}", Fixture_Part_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixture_Repair_MDTOByID(int Fixture_Repair_M_UID)
        {
            var apiUrl = string.Format("Fixture/GetFixture_Repair_MDTOByIDAPI?Fixture_Repair_M_UID={0}", Fixture_Repair_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryBoundDetails(FixturePartInOutBoundInfoDTO search, Page page)
        {


            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("FixturePart/QueryBoundDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 导出All出入库维护作业
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult ExportAllOutboundInfo(FixturePartInOutBoundInfoDTO search)
        {
            var apiUrl = string.Format("FixturePart/ExportAllOutboundInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePartInOutBoundInfoDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("InOutboundDetial");
            var stringHeads = new string[]
             {
                        "序号",
                        "出入库类别",
                        "出入库单编号",
                        "状态",
                        "料号",
                        "品名",
                        "型号",
                        "仓库号",
                        "料架号",
                        "储位",
                        "采购单号",
                        "出入库数量",
                        "领用人",
                        "申请者",
                        "申请时间",
                        "审核者",
                        "审核时间"
             };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("InOutboundDetial");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.In_Out_Type;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Storage_In_Out_Bound_ID;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Fixture_Part_Order;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.In_Out_Bound_Qty;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Outbound_Account;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Applicant_Date.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Approve;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Approve_Date.ToString("yyyy-MM-dd hh:mm");
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        /// <summary>
        /// 导出已经勾选的库存库明细信息
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult ExportPartOutboundInfo(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/ExportPartOutboundInfoAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePartInOutBoundInfoDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("InOutboundDetial");
            var stringHeads = new string[]
            {
                        "序号",
                        "出入库类别",
                        "出入库单编号",
                        "状态",
                        "料号",
                        "品名",
                        "型号",
                        "仓库号",
                        "料架号",
                        "储位",
                        "采购单号",
                        "出入库数量",
                        "领用人",
                        "申请者",
                        "申请时间",
                        "审核者",
                        "审核时间"
            };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("InOutboundDetial");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.In_Out_Type;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Storage_In_Out_Bound_ID;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Fixture_Part_Order;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.In_Out_Bound_Qty;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Outbound_Account;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Applicant_Date.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Approve;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Approve_Date.ToString("yyyy-MM-dd hh:mm");
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }

        public ActionResult GetFixture_Part_Order_Ms(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixture_Part_Order_MsAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixture_Part_Order_Ds(int Fixture_Part_Order_M_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixture_Part_Order_DsAPI?Fixture_Part_Order_M_UID={0}", Fixture_Part_Order_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixture_Part_Order_D(int Fixture_Part_Order_D_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixture_Part_Order_DAPI?Fixture_Part_Order_D_UID={0}", Fixture_Part_Order_D_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixture_Part_Warehouses(int Fixture_Part_Order_D_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixture_Part_WarehousesAPI?Fixture_Part_Order_D_UID={0}", Fixture_Part_Order_D_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditInboundApply(FixturePartInOutBoundInfoDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Applicant_UID = CurrentUser.AccountUId;
            dto.Approve_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("FixturePart/AddOrEditInboundApplyAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryInboundByUids(int Storage_In_Out_Bound_UID, string Inout_Type)
        {
            var apiUrl = string.Format("FixturePart/QueryInboundByUidAPI?Storage_In_Out_Bound_UID={0}&Inout_Type={1}", Storage_In_Out_Bound_UID, Inout_Type);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureWarehouses(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixtureWarehousesAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult SaveWarehouseStorage(FixturePartWarehouseStorageDTO jsonStorage)
        {
            var apiUrl = string.Format("FixturePart/SaveWarehouseStorageAPI");
            jsonStorage.Modified_UID = this.CurrentUser.AccountUId;
            jsonStorage.Created_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonStorage, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public string DeleteBound(int Storage_In_Out_Bound_UID, string Inout_Type)
        {
            var apiUrl = string.Format("FixturePart/DeleteBoundAPI?Storage_In_Out_Bound_UID={0}&Inout_Type={1}", Storage_In_Out_Bound_UID, Inout_Type);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public string ApproveInboundByUid(int Storage_In_Out_Bound_UID)
        {
            var apiUrl = string.Format("FixturePart/ApproveInboundByUidAPI?Storage_In_Out_Bound_UID={0}&Useruid={1}", Storage_In_Out_Bound_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public ActionResult GetMatinventory(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID)
        {
            var apiUrl = string.Format("FixturePart/GetMatinventoryAPI?Fixture_Part_UID={0}&Fixture_Warehouse_Storage_UID={1}", Fixture_Part_UID, Fixture_Warehouse_Storage_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditOutbound(string jsonStorages)
        {
            var apiUrl = string.Format("FixturePart/AddOrEditOutboundAPI");
            var entity = JsonConvert.DeserializeObject<FixturePartInOutBoundInfoDTO>(jsonStorages);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public string ApproveOutboundByUid(int Storage_In_Out_Bound_UID)
        {
            var apiUrl = string.Format("FixturePart/ApproveOutboundByUidAPI?Storage_In_Out_Bound_UID={0}&Useruid={1}", Storage_In_Out_Bound_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        #endregion
        #region 储位移转维护作业
        public ActionResult FixtureStorageTransfer()
        {
            FixtureStorageTransferVM currentVM = new FixtureStorageTransferVM();
            var apiUrlStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessageStatus = APIHelper.APIGetAsync(apiUrlStatus);
            var resultStatus = responMessageStatus.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultStatus);
            currentVM.FixturePartCreateboundStatus = fixtureStatus;

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
            return View(currentVM);

        }

        public ActionResult QueryStorageTransfers(Fixture_Storage_TransferDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("FixturePart/QueryStorageTransfersAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFixturePartWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixturePartWarehouseStoragesAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditStorageTransfer(Fixture_Storage_TransferDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            dto.Approver_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("FixturePart/AddOrEditStorageTransferAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryStorageTransferByUid(int Fixture_Storage_Transfer_UID)
        {
            var apiUrl = string.Format("FixturePart/QueryStorageTransferByUidAPI?Fixture_Storage_Transfer_UID={0}", Fixture_Storage_Transfer_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string DeleteStorageTransfer(int Fixture_Storage_Transfer_UID)
        {
            var apiUrl = string.Format("FixturePart/DeleteStorageTransferAPI?Fixture_Storage_Transfer_UID={0}&userid={1}", Fixture_Storage_Transfer_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public ActionResult DoAllExportStorageTransferReprot(Fixture_Storage_TransferDTO search)
        {


            search.Plant_Organization_UID = GetPlantOrgUid();

            var apiUrl = string.Format("FixturePart/DoAllExportStorageTransferReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Storage_TransferDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageTransfer");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "储位移转单编号", "状态", "料号","品名","型号","转出仓","转出料架","转出储位","转入仓","转入料架",
                "转入储位","移转数量", "申请者","申请时间","审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("StorageTransfer");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Fixture_Storage_Transfer_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Out_Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Out_Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Out_Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.In_Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.In_Rack_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.In_Storage_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Transfer_Qty;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Approver;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);


                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目DoExportStorageTransferReprot
        public ActionResult DoExportStorageTransferReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/DoExportStorageTransferReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Storage_TransferDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageTransfer");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "储位移转单编号", "状态", "料号","品名","型号","转出仓","转出料架","转出储位","转入仓","转入料架",
                "转入储位","移转数量", "申请者","申请时间","审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("StorageTransfer");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Fixture_Storage_Transfer_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Out_Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Out_Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Out_Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.In_Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.In_Rack_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.In_Storage_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Transfer_Qty;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Approver;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 下载储位转移作业模板
        /// </summary>
        /// <returns></returns>
        public FileResult DownloadStorageTransferExcel()
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("批量上传储位移转作业模板");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetStorageTransferHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("批量上传储位移转作业模板");
                SetStorageTransferStyle(worksheet, propertiesHead);
                int maxRow = 1;
                //设置灰色背景
                var colorRange = string.Format("A1:M{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:M{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:M{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:M{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("批量上传储位移转作业模板说明");
                SetStorageTransferStyle(worksheetsm, propertiesHead);

                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:M{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:M{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:M{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                SetStorageTransferStylesm(worksheetsm);

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private string[] GetStorageTransferHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区", "OP类型", "功能厂","料号","品名","型号","转出仓库","转出料架号","转出储位","转入仓库","转入料架号","转入储位","转移数量"
            };
            return propertiesHead;
        }
        private void SetStorageTransferStyle(ExcelWorksheet worksheet, string[] propertiesHead)
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
            worksheet.Cells[1, 11].Value = propertiesHead[10];
            worksheet.Cells[1, 12].Value = propertiesHead[11];
            worksheet.Cells[1, 13].Value = propertiesHead[12];
            //设置列宽
            for (int i = 1; i <= 13; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:M1"].Style.Font.Bold = true;
            worksheet.Cells["A1:M1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:M1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        private void SetStorageTransferStylesm(ExcelWorksheet worksheet)
        {
            worksheet.Cells[2, 1].Value = "CTU(厂区-小于50个字符)";
            worksheet.Cells[2, 2].Value = "OP2(OP类型-小于500个字符)";
            worksheet.Cells[2, 3].Value = "CNC(功能厂-小于500个字符)";
            worksheet.Cells[2, 4].Value = "l001(料号-小于20字符)";
            worksheet.Cells[2, 5].Value = "500(品名-小于20字符)";
            worksheet.Cells[2, 6].Value = "500(型号-小于20字符)";
            worksheet.Cells[2, 7].Value = "G001(转入仓库-小于500个字符)";
            worksheet.Cells[2, 8].Value = "l001(转入料架号-小于20字符)";
            worksheet.Cells[2, 9].Value = "500(转入储位-小于20字符)";
            worksheet.Cells[2, 10].Value = "G001(转出仓库-小于500个字符)";
            worksheet.Cells[2, 11].Value = "l001(转出料架号-小于20字符)";
            worksheet.Cells[2, 12].Value = "500(转出储位-小于20字符)";
            worksheet.Cells[2, 13].Value = "500(转移数量-为整数)";
        }

        /// <summary>
        /// 导入储位转移数据
        /// </summary>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public string ImportStorageTransferExcel(HttpPostedFileBase uploadName)
        {
            int plant_OrganizationUID = GetPlantOrgUid();
            string errorInfo = string.Empty;
            if (plant_OrganizationUID != 0)
            {
                try
                {
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
                            "功能厂",
                            "料号",
                            "品名",
                            "型号",
                            "转出仓库",
                            "转出料架号",
                            "转出储位",
                            "转入仓库",
                            "转入料架号",
                            "转入储位",
                            "转移数量"
                         };
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
                        //获得所有ORGBOMLIST
                        var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                        HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                        var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                        var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                        //获取厂区下所有配件料。
                        var fixture_PartapiUrl = string.Format("FixturePart/GetFixturePartDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", plant_OrganizationUID, 0, 0);
                        HttpResponseMessage fixture_PartresponMessage = APIHelper.APIGetAsync(fixture_PartapiUrl);
                        var fixture_Partresult = fixture_PartresponMessage.Content.ReadAsStringAsync().Result;
                        List<Fixture_PartDTO> Fixture_PartDTOs = JsonConvert.DeserializeObject<List<Fixture_PartDTO>>(fixture_Partresult);

                        //获取厂区下所有转出储位，和转出储位的总库存。
                        var apiUrl = string.Format("FixturePart/Fixture_Part_InventoryDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", plant_OrganizationUID, 0, 0);
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var materialInventorysresult = responMessage.Content.ReadAsStringAsync().Result;
                        List<Fixture_Part_InventoryDTO> materialInventorys = JsonConvert.DeserializeObject<List<Fixture_Part_InventoryDTO>>(materialInventorysresult);


                        //获取所有转入储位
                        var inwarehouse_storageapiUrl = string.Format("FixturePart/GetFixturePartWarehouseStoragesAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", plant_OrganizationUID, 0, 0);
                        HttpResponseMessage inwarehouse_storageresponMessage = APIHelper.APIGetAsync(inwarehouse_storageapiUrl);
                        var inwarehouse_storageresult = inwarehouse_storageresponMessage.Content.ReadAsStringAsync().Result;
                        List<Fixture_Warehouse_StorageDTO> fixture_Warehouse_StorageDTOs = JsonConvert.DeserializeObject<List<Fixture_Warehouse_StorageDTO>>(inwarehouse_storageresult);

                        var storageTransferDTOList = new List<Fixture_Storage_TransferDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var storageTransferDTO = new Fixture_Storage_TransferDTO();
                            int Plant_Organization_UID = 0;
                            int BG_Organization_UID = 0;
                            int? FunPlant_Organization_UID = null;

                            string plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                            if (string.IsNullOrWhiteSpace(plant))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区没有值", i);
                                return errorInfo;
                            }
                            else if (plant.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            else
                            {
                                var hasbg = orgboms.Where(m => m.Plant == plant).FirstOrDefault();
                                if (hasbg != null)
                                {
                                    Plant_Organization_UID = hasbg.Plant_Organization_UID;
                                }
                                else
                                {
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行厂区的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            string bG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                            if (string.IsNullOrWhiteSpace(bG_Organization))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行OP类型没有值", i);
                                return errorInfo;
                            }
                            else if (bG_Organization.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行OP类型长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            else
                            {
                                var hasbg = orgboms.Where(m => m.BG == bG_Organization).FirstOrDefault();
                                if (hasbg != null)
                                {
                                    BG_Organization_UID = hasbg.BG_Organization_UID;
                                }
                                else
                                {
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行OP的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            string funPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                            if (string.IsNullOrWhiteSpace(funPlant_Organization))
                            {
                                //isExcelError = true;
                                //errorInfo = string.Format("第{0}行功能厂没有值", i);
                                //return errorInfo;
                            }
                            else if (funPlant_Organization.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行功能厂长度超过最大限定[50]", i);
                                return errorInfo;
                            }


                            string Part_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                            if (string.IsNullOrWhiteSpace(Part_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号没有值", i);
                                return errorInfo;
                            }
                            else if (Part_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string Part_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                            if (string.IsNullOrWhiteSpace(Part_Name))
                            {
                                //isExcelError = true;
                                //errorInfo = string.Format("第{0}行品名没有值", i);
                                //return errorInfo;
                            }
                            else if (Part_Name.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行品名长度超过最大限定[50]", i);
                                return errorInfo;
                            }

                            string Part_Spec = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "型号")].Value);
                            if (string.IsNullOrWhiteSpace(Part_Spec))
                            {
                                //isExcelError = true;
                                //errorInfo = string.Format("第{0}行型号没有值", i);
                                //return errorInfo;
                            }
                            else if (Part_Spec.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行型号长度超过最大限定[50]", i);
                                return errorInfo;
                            }

                            string Out_Fixture_Warehouse_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出仓库")].Value);
                            if (string.IsNullOrWhiteSpace(Out_Fixture_Warehouse_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出仓库没有值", i);
                                return errorInfo;
                            }
                            else if (Out_Fixture_Warehouse_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出仓库长度超过最大限定[10]", i);
                                return errorInfo;
                            }

                            string Out_Rack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出料架号")].Value);
                            if (string.IsNullOrWhiteSpace(Out_Rack_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出料架号没有值", i);
                                return errorInfo;
                            }
                            else if (Out_Rack_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出料架号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string Out_Storage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出储位")].Value);
                            if (string.IsNullOrWhiteSpace(Out_Storage_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出储位没有值", i);
                                return errorInfo;
                            }
                            else if (Out_Storage_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出储位长度超过最大限定[20]", i);
                                return errorInfo;
                            }


                            string In_Fixture_Warehouse_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入仓库")].Value);
                            if (string.IsNullOrWhiteSpace(In_Fixture_Warehouse_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入仓库没有值", i);
                                return errorInfo;
                            }
                            else if (In_Fixture_Warehouse_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入仓库长度超过最大限定[10]", i);
                                return errorInfo;
                            }

                            string In_Rack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入料架号")].Value);
                            if (string.IsNullOrWhiteSpace(In_Rack_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入料架号没有值", i);
                                return errorInfo;
                            }
                            else if (In_Rack_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入料架号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string In_Storage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入储位")].Value);
                            if (string.IsNullOrWhiteSpace(In_Storage_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入储位没有值", i);
                                return errorInfo;
                            }
                            else if (In_Storage_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入储位长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            decimal check_Qty = 0;
                            string strcheck_Qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转移数量")].Value);
                            if (string.IsNullOrWhiteSpace(strcheck_Qty))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转移数量没有值", i);
                                return errorInfo;
                            }
                            else
                            {
                                decimal.TryParse(strcheck_Qty, out check_Qty);
                            }
                            //获取料号主键
                            int Fixture_Part_UID = 0;
                            var fixture_PartDTO = Fixture_PartDTOs.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID && o.Part_ID == Part_ID);
                            if (fixture_PartDTO != null)
                            {
                                Fixture_Part_UID = fixture_PartDTO.Fixture_Part_UID;

                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没根据料号，品名 有找到对应的料号", i);
                                return errorInfo;
                            }

                            int Out_Fixture_Warehouse_Storage_UID = 0;
                            var Out_warehouse_storages = fixture_Warehouse_StorageDTOs.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID && o.Fixture_Warehouse_ID == Out_Fixture_Warehouse_ID &&
                            o.Rack_ID == Out_Rack_ID && o.Storage_ID == Out_Storage_ID);
                            if (Out_warehouse_storages != null)
                            {
                                Out_Fixture_Warehouse_Storage_UID = Out_warehouse_storages.Fixture_Warehouse_Storage_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有找到转出的储位", i);
                                return errorInfo;
                            }

                            decimal Inventory_Qty = 0;
                            var materialInventory = materialInventorys.FirstOrDefault(o => o.Fixture_Part_UID == Fixture_Part_UID && o.Fixture_Warehouse_Storage_UID == Out_Fixture_Warehouse_Storage_UID);
                            if (materialInventory != null)
                            {
                                Inventory_Qty = materialInventory.Inventory_Qty;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有存库数量", i);
                                return errorInfo;
                            }

                            int In_Fixture_Warehouse_Storage_UID = 0;
                            var innwarehouse_storages = fixture_Warehouse_StorageDTOs.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID && o.Fixture_Warehouse_ID == In_Fixture_Warehouse_ID &&
                            o.Rack_ID == In_Rack_ID && o.Storage_ID == In_Storage_ID);
                            if (innwarehouse_storages != null)
                            {
                                In_Fixture_Warehouse_Storage_UID = innwarehouse_storages.Fixture_Warehouse_Storage_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有找到转入的储位", i);
                                return errorInfo;
                            }

                            //导入数据判重
                            var isSelfRepeated = storageTransferDTOList.Exists(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Fixture_Part_UID == Fixture_Part_UID && m.Out_Fixture_Warehouse_Storage_UID == Out_Fixture_Warehouse_Storage_UID && m.In_Fixture_Warehouse_Storage_UID == In_Fixture_Warehouse_Storage_UID && m.Transfer_Qty == check_Qty);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行在导入数据中重复,不可重复导入", i);
                                return errorInfo;
                            }


                            if (Out_Fixture_Warehouse_Storage_UID == In_Fixture_Warehouse_Storage_UID)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入储位与转出储位不可一致,不可重复导入", i);
                                return errorInfo;
                            }

                            if (check_Qty > Inventory_Qty)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出储位数量(" + Inventory_Qty + ")不足", i);
                                return errorInfo;

                            }


                            //料号
                            storageTransferDTO.Fixture_Part_UID = Fixture_Part_UID;
                            storageTransferDTO.Part_ID = Part_ID;
                            storageTransferDTO.Part_Name = Part_Name;
                            storageTransferDTO.Part_Spec = Part_Spec;
                            //转出仓
                            storageTransferDTO.Out_Fixture_Warehouse_Storage_UID = Out_Fixture_Warehouse_Storage_UID;
                            //转入仓
                            storageTransferDTO.In_Fixture_Warehouse_Storage_UID = In_Fixture_Warehouse_Storage_UID;
                            //转移数量
                            storageTransferDTO.Transfer_Qty = check_Qty;
                            storageTransferDTO.Applicant_UID = CurrentUser.AccountUId;
                            storageTransferDTO.Applicant_Date = DateTime.Now;
                            storageTransferDTO.Approver_Date = DateTime.Now;
                            storageTransferDTO.Approver_UID = CurrentUser.AccountUId;
                            storageTransferDTOList.Add(storageTransferDTO);

                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(storageTransferDTOList);
                        var apiInsertStorageCheckDUrl = string.Format("FixturePart/ImportStorageTransferAPI");
                        HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertStorageCheckDUrl);
                        errorInfo = responSettingMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    }
                }
                catch (Exception e)
                {

                    errorInfo = "导入异常原因失败:" + e.ToString();
                }
            }
            else
            {
                errorInfo = "请设置厂区才能做此操作！";
            }
            return errorInfo;

            //return "";
        }

        /// <summary>
        /// 储位移转审核功能 
        /// </summary>
        /// <param name="Fixture_Storage_Transfer_UID"></param>
        /// <returns></returns>
        public string ApproveStorageTransferByUid(int Fixture_Storage_Transfer_UID)
        {
            var apiUrl = string.Format("FixturePart/ApproveStTransferAPI?Fixture_Storage_Transfer_UID={0}&Useruid={1}", Fixture_Storage_Transfer_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        #endregion
        #region 盘点单维护作业
        public ActionResult FixtureStorageCheck()
        {
            FixtureStorageCheckVM currentVM = new FixtureStorageCheckVM();
            var apiUrlStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessageStatus = APIHelper.APIGetAsync(apiUrlStatus);
            var resultStatus = responMessageStatus.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultStatus);
            currentVM.FixturePartCreateboundStatus = fixtureStatus;
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
            return View(currentVM);
        }
        public ActionResult QueryStorageChecks(FixtureStorageCheckDTO search, Page page)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("FixturePart/QueryStorageChecksAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditStorageCheck(FixtureStorageCheckDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("FixturePart/AddOrEditStorageCheckAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryStorageCheckByUid(int Fixture_Storage_Check_UID)
        {
            var apiUrl = string.Format("FixturePart/QueryStorageCheckByUidAPI?Fixture_Storage_Check_UID={0}", Fixture_Storage_Check_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string DeleteStorageCheck(int Fixture_Storage_Check_UID)
        {
            var apiUrl = string.Format("FixturePart/DeleteStorageCheckAPI?Fixture_Storage_Check_UID={0}&userid={1}", Fixture_Storage_Check_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public ActionResult DoAllExportStorageCheckReprot(FixtureStorageCheckDTO search)
        {

            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("FixturePart/DoAllExportStorageCheckReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureStorageCheckDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageCheck");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "盘点单编号", "状态", "料号","品名","型号","仓库","料架号","储位","盘点数量",
               "申请者","申请时间","审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("StorageCheck");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Fixture_Storage_Check_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Check_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Applicant;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Approve;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Approve_Date == null ? "" : ((DateTime)currentRecord.Approve_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        //导出excel----by勾选的项目
        public ActionResult DoExportStorageCheckReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/DoExportStorageCheckReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureStorageCheckDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageCheck");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "盘点单编号", "状态", "料号","品名","型号","仓库","料架号","储位","盘点数量",
               "申请者","申请时间","审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("StorageCheck");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Fixture_Storage_Check_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Check_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Applicant;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Approve;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Approve_Date == null ? "" : ((DateTime)currentRecord.Approve_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public FileResult DownloadStorageCheckExcel(string Part_ID, string Part_Name, string Part_Spec, string Fixture_Warehouse_ID, string Rack_ID, string Storage_ID)
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("批量上传盘点维护作业模板");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetStorageCheckHeadColumn();
            //  StorageCheckDTO
            using (var excelPackage = new ExcelPackage(stream))
            {
                //get Export datas
                var apiUrl = string.Format(@"FixturePart/DownloadStorageCheckAPI?&Part_ID={0}&Part_Name={1}&Part_Spec={2}&Fixture_Warehouse_ID={3}&Rack_ID={4}&Storage_ID={5}", Part_ID, Part_Name, Part_Spec, Fixture_Warehouse_ID, Rack_ID, Storage_ID);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<FixtureStorageCheckDTO>>(result).ToList();

                var worksheet = excelPackage.Workbook.Worksheets.Add("批量上传盘点维护作业模板");
                SetStorageCheckExcelStyle(worksheet, propertiesHead);
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    // "类别","料号","品名","型号","仓库","料架号","储位","盘点数量"
                    worksheet.Cells[index + 2, 1].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Storage_ID;
                    // worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;

                }
                int maxRow = 1;
                //设置灰色背景
                var colorRange = string.Format("A1:J{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("批量上传盘点维护作业模板说明");
                SetStorageCheckExcelStyle(worksheetsm, propertiesHead);

                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                SetStorageCheckExcelStylesm(worksheetsm);

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetStorageCheckExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
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
            for (int i = 1; i <= 10; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:J1"].Style.Font.Bold = true;
            worksheet.Cells["A1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        private void SetStorageCheckExcelStylesm(ExcelWorksheet worksheet)
        {
            worksheet.Cells[2, 1].Value = "CTU(厂区-小于20字符)";
            worksheet.Cells[2, 2].Value = "OP2(OP类型-小于20字符)";
            worksheet.Cells[2, 3].Value = "功能厂(功能厂-小于20字符)";
            worksheet.Cells[2, 4].Value = "l001(料号-小于20字符)";
            worksheet.Cells[2, 5].Value = "500(品名-小于20字符)";
            worksheet.Cells[2, 6].Value = "500(型号-小于20字符)";
            worksheet.Cells[2, 7].Value = "G001(仓库-小于500个字符)";
            worksheet.Cells[2, 8].Value = "l001(料架号-小于20字符)";
            worksheet.Cells[2, 9].Value = "500(储位-小于20字符)";
            worksheet.Cells[2, 10].Value = "500(盘点数量-为整数)";
        }
        private string[] GetStorageCheckHeadColumn()
        {
            var propertiesHead = new[]
            {
             "厂区",  "OP类型",  "功能厂", "料号","品名","型号","仓库","料架号","储位","盘点数量"
            };
            return propertiesHead;
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public string ImportStorageCheckExcel(HttpPostedFileBase uploadName)
        {
            int plant_OrganizationUID = GetPlantOrgUid();
            string errorInfo = string.Empty;
            if (plant_OrganizationUID != 0)
            {
                try
                {
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
                            "功能厂",
                            "料号",
                            "品名",
                            "型号",
                            "仓库",
                            "料架号",
                            "储位",
                            "盘点数量"
                        };
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
                        //获得所有ORGBOMLIST
                        var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                        HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                        var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                        var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                        //获取厂区下所有配件料。
                        var fixture_PartapiUrl = string.Format("FixturePart/GetFixturePartDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", plant_OrganizationUID, 0, 0);
                        HttpResponseMessage fixture_PartresponMessage = APIHelper.APIGetAsync(fixture_PartapiUrl);
                        var fixture_Partresult = fixture_PartresponMessage.Content.ReadAsStringAsync().Result;
                        List<Fixture_PartDTO> Fixture_PartDTOs = JsonConvert.DeserializeObject<List<Fixture_PartDTO>>(fixture_Partresult);

                        //获取厂区下所有已开账储位，和已开账储位的总库存。
                        var apiUrl = string.Format("FixturePart/Fixture_Part_InventoryDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", plant_OrganizationUID, 0, 0);
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var materialInventorysresult = responMessage.Content.ReadAsStringAsync().Result;
                        List<Fixture_Part_InventoryDTO> materialInventorys = JsonConvert.DeserializeObject<List<Fixture_Part_InventoryDTO>>(materialInventorysresult);

                        //获取所有储位
                        var inwarehouse_storageapiUrl = string.Format("FixturePart/GetFixturePartWarehouseStoragesAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", plant_OrganizationUID, 0, 0);
                        HttpResponseMessage inwarehouse_storageresponMessage = APIHelper.APIGetAsync(inwarehouse_storageapiUrl);
                        var inwarehouse_storageresult = inwarehouse_storageresponMessage.Content.ReadAsStringAsync().Result;
                        List<Fixture_Warehouse_StorageDTO> fixture_Warehouse_StorageDTOs = JsonConvert.DeserializeObject<List<Fixture_Warehouse_StorageDTO>>(inwarehouse_storageresult);

                        var storageCheckDTOList = new List<FixtureStorageCheckDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var storageCheckDTO = new FixtureStorageCheckDTO();

                            int Plant_Organization_UID = 0;
                            int BG_Organization_UID = 0;
                            int? FunPlant_Organization_UID = null;

                            string plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                            if (string.IsNullOrWhiteSpace(plant))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区没有值", i);
                                return errorInfo;
                            }
                            else if (plant.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            else
                            {
                                var hasbg = orgboms.Where(m => m.Plant == plant).FirstOrDefault();
                                if (hasbg != null)
                                {
                                    Plant_Organization_UID = hasbg.Plant_Organization_UID;
                                }
                                else
                                {
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行厂区的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            string bG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                            if (string.IsNullOrWhiteSpace(bG_Organization))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行OP类型没有值", i);
                                return errorInfo;
                            }
                            else if (bG_Organization.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行OP类型长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            else
                            {
                                var hasbg = orgboms.Where(m => m.BG == bG_Organization).FirstOrDefault();
                                if (hasbg != null)
                                {
                                    BG_Organization_UID = hasbg.BG_Organization_UID;
                                }
                                else
                                {
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行OP的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            string funPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                            if (string.IsNullOrWhiteSpace(funPlant_Organization))
                            {
                                //isExcelError = true;
                                //errorInfo = string.Format("第{0}行功能厂没有值", i);
                                //return errorInfo;
                            }
                            else if (funPlant_Organization.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行功能厂长度超过最大限定[50]", i);
                                return errorInfo;
                            }


                            string Part_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                            if (string.IsNullOrWhiteSpace(Part_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号没有值", i);
                                return errorInfo;
                            }
                            else if (Part_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string Part_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                            if (string.IsNullOrWhiteSpace(Part_Name))
                            {
                                //isExcelError = true;
                                //errorInfo = string.Format("第{0}行品名没有值", i);
                                //return errorInfo;
                            }
                            else if (Part_Name.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行品名长度超过最大限定[50]", i);
                                return errorInfo;
                            }

                            string Part_Spec = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "型号")].Value);
                            if (string.IsNullOrWhiteSpace(Part_Spec))
                            {
                                //isExcelError = true;
                                //errorInfo = string.Format("第{0}行型号没有值", i);
                                //return errorInfo;
                            }
                            else if (Part_Spec.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行型号长度超过最大限定[50]", i);
                                return errorInfo;
                            }

                            string Fixture_Warehouse_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_Warehouse_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行仓库没有值", i);
                                return errorInfo;
                            }
                            else if (Fixture_Warehouse_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行仓库长度超过最大限定[10]", i);
                                return errorInfo;
                            }

                            string rack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料架号")].Value);
                            if (string.IsNullOrWhiteSpace(rack_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料架号没有值", i);
                                return errorInfo;
                            }
                            else if (rack_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料架号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string storage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "储位")].Value);
                            if (string.IsNullOrWhiteSpace(storage_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行储位没有值", i);
                                return errorInfo;
                            }
                            else if (storage_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行储位长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            int check_Qty = 0;
                            string strcheck_Qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "盘点数量")].Value);
                            if (string.IsNullOrWhiteSpace(strcheck_Qty))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行盘点数量没有值", i);
                                return errorInfo;
                            }
                            else
                            {
                                int.TryParse(strcheck_Qty, out check_Qty);
                            }
                            //获取料号主键
                            int Fixture_Part_UID = 0;
                            var fixture_PartDTO = Fixture_PartDTOs.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID && o.Part_ID == Part_ID);
                            if (fixture_PartDTO != null)
                            {
                                Fixture_Part_UID = fixture_PartDTO.Fixture_Part_UID;

                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没根据料号，品名 有找到对应的料号", i);
                                return errorInfo;
                            }

                            int Fixture_Warehouse_Storage_UID = 0;
                            var warehouse_storages = fixture_Warehouse_StorageDTOs.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID && o.Fixture_Warehouse_ID == Fixture_Warehouse_ID &&
                            o.Rack_ID == rack_ID && o.Storage_ID == storage_ID);
                            if (warehouse_storages != null)
                            {
                                Fixture_Warehouse_Storage_UID = warehouse_storages.Fixture_Warehouse_Storage_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有找到转出的储位", i);
                                return errorInfo;
                            }
                            //获取库存数量
                            decimal Inventory_Qty = 0;
                            var materialInventory = materialInventorys.FirstOrDefault(o => o.Fixture_Part_UID == Fixture_Part_UID && o.Fixture_Warehouse_Storage_UID == Fixture_Warehouse_Storage_UID);
                            if (materialInventory != null)
                            {
                                Inventory_Qty = materialInventory.Inventory_Qty;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有存库数量", i);
                                return errorInfo;
                            }

                            ////导入数据判重
                            var isSelfRepeated = storageCheckDTOList.Exists(m => m.Fixture_Part_UID == Fixture_Part_UID && m.Fixture_Warehouse_Storage_UID == Fixture_Warehouse_Storage_UID && m.Check_Qty == check_Qty);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行在导入数据中重复,不可重复导入", i);
                                return errorInfo;
                            }

                            storageCheckDTO.Fixture_Part_UID = Fixture_Part_UID;
                            storageCheckDTO.Part_ID = Part_ID;
                            storageCheckDTO.Part_Name = Part_Name;
                            storageCheckDTO.Part_Spec = Part_Spec;
                            storageCheckDTO.Fixture_Warehouse_Storage_UID = Fixture_Warehouse_Storage_UID;
                            storageCheckDTO.Fixture_Warehouse_ID = Fixture_Warehouse_ID;
                            storageCheckDTO.Rack_ID = rack_ID;
                            storageCheckDTO.Storage_ID = storage_ID;
                            storageCheckDTO.Check_Qty = check_Qty;
                            storageCheckDTO.Old_Inventory_Qty = Inventory_Qty;
                            storageCheckDTO.Applicant_Date = DateTime.Now;
                            storageCheckDTO.Applicant_UID = CurrentUser.AccountUId;
                            storageCheckDTO.Approve_Date = DateTime.Now;
                            storageCheckDTO.Approve_UID = CurrentUser.AccountUId;
                            storageCheckDTOList.Add(storageCheckDTO);

                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(storageCheckDTOList);
                        var apiInsertStorageCheckDUrl = string.Format("FixturePart/ImportStorageCheckAPI");
                        HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertStorageCheckDUrl);
                        errorInfo = responSettingMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    }
                }
                catch (Exception e)
                {

                    errorInfo = "导入异常原因失败:" + e.ToString();
                }
            }
            else
            {
                errorInfo = "请设置厂区才能做此操作！";
            }
            return errorInfo;

            //return "";
        }

        public string ApproveStorageCheckByUid(int Fixture_Storage_Check_UID)
        {
            var apiUrl = string.Format("FixturePart/ApproveStCheckAPI?Fixture_Storage_Check_UID={0}&Useruid={1}", Fixture_Storage_Check_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string ApproveStorageCheck(FixtureStorageCheckDTO search)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            search.Approve_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("FixturePart/ApproveStorageCheckAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        #endregion
        #region 开账作业
        public ActionResult FixturePartCreatebound()
        {

            FixturePartCreateboundVM currentVM = new FixturePartCreateboundVM();
            var apiUrlStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessageStatus = APIHelper.APIGetAsync(apiUrlStatus);
            var resultStatus = responMessageStatus.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultStatus);
            currentVM.FixturePartCreateboundStatus = fixtureStatus;

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
            return View(currentVM);
        }

        public ActionResult QueryCreateBounds(FixturePartStorageInboundDTO search, Page page)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("FixturePart/QueryCreateBoundsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixtureWarehouseStoragesAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureParts(int Fixture_Warehouse_Storage_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixturePartsAPI?Fixture_Warehouse_Storage_UID={0}", Fixture_Warehouse_Storage_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditCreateBound(FixturePartStorageInboundDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Applicant_UID = CurrentUser.AccountUId;
            dto.Approve_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("FixturePart/AddOrEditCreateBoundAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryInboundByUid(int Fixture_Storage_Inbound_UID)
        {
            var apiUrl = string.Format("FixturePart/QueryInboundByUidAPI?Fixture_Storage_Inbound_UID={0}", Fixture_Storage_Inbound_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string DeleteCreateBound(int Fixture_Storage_Inbound_UID)
        {
            var apiUrl = string.Format("FixturePart/DeleteCreateBoundAPI?Fixture_Storage_Inbound_UID={0}", Fixture_Storage_Inbound_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        private string[] GetCreateBoundHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区","OP类型","功能厂","仓库代码","料架号","储位号","料号","品名","型号","库存数量"
            };
            return propertiesHead;
        }
        public FileResult DownloadCreateBoundExcel()
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("批量上传开账信息模板");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetCreateBoundHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("批量上传开账信息模板");
                SetCreateBoundExcelStyle(worksheet, propertiesHead);
                int maxRow = 1;
                //设置灰色背景
                var colorRange = string.Format("A1:J{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("批量上传开账信息模板说明");
                SetCreateBoundExcelStyle(worksheetsm, propertiesHead);

                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:J{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                SetCreateBoundExcelStylesm(worksheetsm);

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        private void SetCreateBoundExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
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
            for (int i = 1; i <= 10; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:J1"].Style.Font.Bold = true;
            worksheet.Cells["A1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        private void SetCreateBoundExcelStylesm(ExcelWorksheet worksheet)
        {

            worksheet.Cells[2, 1].Value = "CTU_M(厂区--小于20字符)";
            worksheet.Cells[2, 2].Value = "OP2(OP2-小于20字符)";
            worksheet.Cells[2, 3].Value = "CNC(功能厂-小于20字符)";
            worksheet.Cells[2, 4].Value = "ck001(仓库号-小于20字符)";
            worksheet.Cells[2, 5].Value = "lj001(料架号-小于20字符)";
            worksheet.Cells[2, 6].Value = "cw001(储位号-小于20字符)";
            worksheet.Cells[2, 7].Value = "cw001(料号号-小于20字符)";
            worksheet.Cells[2, 8].Value = "OP1阳极(品名-小于500个字符)";
            worksheet.Cells[2, 9].Value = "l001(型号-小于20字符)";
            worksheet.Cells[2, 10].Value = "500(库存数量-为整数)";

        }
        public ActionResult DoAllExportCreateBoundReprot(FixturePartStorageInboundDTO search)
        {


            search.Plant_Organization_UID = GetPlantOrgUid();

            var apiUrl = string.Format("FixturePart/DoAllExportCreateBoundReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePartStorageInboundDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CreateBound");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "状态", "厂区", "OP类型", "功能厂", "料号", "品名", "型号", "仓库代码", "仓库名称", "料架号", "储位号", "库存数量", "开账时间" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("CreateBound");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Inbound_Qty;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportCreateBoundReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/DoExportCreateBoundReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePartStorageInboundDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CreateBound");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "状态", "厂区", "OP类型", "功能厂", "料号", "品名", "型号", "仓库代码", "仓库名称", "料架号", "储位号", "库存数量", "开账时间" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("CreateBound");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Inbound_Qty;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public string ImportCreateBoundExcel(HttpPostedFileBase upload_excel)
        {


            string errorInfo = string.Empty;
            errorInfo = AddCreateBoundExcel(upload_excel);
            return errorInfo;
        }

        private string AddCreateBoundExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;

            try
            {

                using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
                {
                    int PlantOrgUid = GetPlantOrgUid();
                    var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalColumns = worksheet.Dimension.End.Column;
                    if (worksheet == null)
                    {
                        errorInfo = "没有worksheet内容";
                        return errorInfo;
                    }
                    //头样式设置
                    var propertiesHead = GetCreateBoundHeadColumn();

                    int iRow = 1;

                    bool excelIsError = false;
                    if (totalColumns != propertiesHead.Length)
                    {
                        return "Excel格式不正确";
                    }
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

                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                    //读取所有料号

                    var fixtureDefectCodeAPI = "Fixture/QueryAllFixturePartsAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var workStations = JsonConvert.DeserializeObject<List<Fixture_PartDTO>>(jsonsVendorInfo);

                    //获取所有仓库
                    var warehouseStAPI = string.Format("FixturePart/QueryAllWarehouseStAPI?Plant_Organization_UID={0}", PlantOrgUid);
                    HttpResponseMessage WarhouseStmessage = APIHelper.APIGetAsync(warehouseStAPI);
                    var jsonwarst = WarhouseStmessage.Content.ReadAsStringAsync().Result;
                    var warehouses = JsonConvert.DeserializeObject<List<FixturePartWarehouseDTO>>(jsonwarst);

                    //获取所有的储位信息
                    var warehouseStorageAPI = string.Format("FixturePart/GetFixtureWarehouseStorageALLAPI?Plant_Organization_UID={0}", PlantOrgUid);
                    HttpResponseMessage warehouseStorageAPIMessage = APIHelper.APIGetAsync(warehouseStorageAPI);
                    var jsonwarehouseStorageresult = warehouseStorageAPIMessage.Content.ReadAsStringAsync().Result;
                    var warehouseStorage = JsonConvert.DeserializeObject<List<FixturePartWarehouseDTO>>(jsonwarehouseStorageresult);


                    //获取所有开账信息
                    var StorageInboundAPI = string.Format("FixturePart/QueryAllStorageInboundAPI?Plant_Organization_UID={0}", PlantOrgUid);
                    HttpResponseMessage StorageInboundStmessage = APIHelper.APIGetAsync(StorageInboundAPI);
                    var jsonstoinbound = StorageInboundStmessage.Content.ReadAsStringAsync().Result;
                    var stoinbound = JsonConvert.DeserializeObject<List<FixturePartStorageInboundDTO>>(jsonstoinbound);

                    string PreInboundID = "Opening" + DateTime.Today.ToString("yyyyMMdd");
                    var test = stoinbound.Where(m => m.Fixture_Storage_Inbound_ID.StartsWith(PreInboundID)).ToList();

                    var apiUrlInOutStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI?Enum_Type={0}", "FixturePartInbound_Type");
                    HttpResponseMessage responMessageInOutStatus = APIHelper.APIGetAsync(apiUrlInOutStatus);
                    var resultInOutStatus = responMessageInOutStatus.Content.ReadAsStringAsync().Result;
                    var fixtureInOutStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultInOutStatus);


                    var apiUrlStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI");
                    HttpResponseMessage responMessageStatus = APIHelper.APIGetAsync(apiUrlStatus);
                    var resultStatus = responMessageStatus.Content.ReadAsStringAsync().Result;
                    var fixtureStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultStatus);


                    List<FixturePartStorageInboundDTO> storageinboundlist = new List<FixturePartStorageInboundDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {

                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        string Plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value).Trim();
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        if (string.IsNullOrWhiteSpace(Plant))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.Plant == Plant.Trim()).FirstOrDefault();
                            if (hasbg != null)
                            {
                                Plant_Organization_UID = hasbg.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.BG == BG_Organization.Trim()).FirstOrDefault();
                            if (hasbg != null)
                            {
                                BG_Organization_UID = hasbg.BG_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            //excelIsError = true;
                            //errorInfo = string.Format("第{0}行没有功能厂", i);
                            //return errorInfo;
                        }
                        else
                        {

                            var hasfunplant = orgboms.Where(m => m.BG == BG_Organization & m.Funplant == FunPlant_Organization.Trim()).FirstOrDefault();
                            if (hasfunplant != null)
                            {
                                FunPlant_Organization_UID = hasfunplant.Funplant_UID;
                            }
                            else
                            {
                                //excelIsError = true;
                                //errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                //return errorInfo;
                            }
                        }


                        string warehouseCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库代码")].Value);
                        string Rack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料架号")].Value);
                        string Storage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "储位号")].Value);

                        int wareHouseUID = 0;
                        if (string.IsNullOrWhiteSpace(warehouseCode))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            //判断该仓库代码是否在数据库中
                            var hasbg = warehouses.Where(m => m.Fixture_Warehouse_ID == warehouseCode && m.BG_Organization_UID == BG_Organization_UID && m.Plant_Organization_UID == Plant_Organization_UID).FirstOrDefault();
                            if (hasbg != null)
                            {
                                wareHouseUID = hasbg.Fixture_Warehouse_UID;
                                FunPlant_Organization_UID = hasbg.FunPlant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行的仓库代码没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Rack_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有料架号", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(Storage_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有储位号", i);
                            return errorInfo;
                        }

                        //找到储位
                        int Fixture_Warehouse_Storage_UID = 0;
                        var HasWarehouseCode = warehouseStorage.FirstOrDefault(m => m.Fixture_Warehouse_UID == wareHouseUID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Rack_ID == Rack_ID && m.Storage_ID == Storage_ID);
                        if (HasWarehouseCode != null)
                        {
                            Fixture_Warehouse_Storage_UID = HasWarehouseCode.Fixture_Warehouse_Storage_UID;
                        }
                        else
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行储位没找到", i);
                            return errorInfo;
                        }

                        //找到对应料号
                        // "料号", "品名", "型号",
                        string Part_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                        string Part_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                        string Part_Spec = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "型号")].Value);

                        int Fixture_Part_UID = 0;
                        var Fixture_Part = workStations.FirstOrDefault(m => m.Part_ID == Part_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);

                        if (Fixture_Part != null)
                        {
                            Fixture_Part_UID = Fixture_Part.Fixture_Part_UID;
                        }
                        else
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料号没找到", i);
                            return errorInfo;
                        }

                        decimal Inbound_Qty = 0;
                        string qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "库存数量")].Value);
                        try
                        {
                            Inbound_Qty = Convert.ToDecimal(qty);
                        }
                        catch
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行库存数量值错误", i);
                            return errorInfo;
                        }

                        //判断excel 重复?
                        var storageinbound = storageinboundlist.FirstOrDefault(o => o.Fixture_Warehouse_Storage_UID == Fixture_Warehouse_Storage_UID && o.Fixture_Part_UID == Fixture_Part_UID && o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID);
                        if (storageinbound != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行数据重复，请检查EXCEL", i);
                            return errorInfo;
                        }

                        //判断数据库中是否重复?

                        var storageinboundsql = stoinbound.FirstOrDefault(o => o.Fixture_Warehouse_Storage_UID == Fixture_Warehouse_Storage_UID && o.Fixture_Part_UID == Fixture_Part_UID && o.Plant_Organization_UID == Plant_Organization_UID && o.BG_Organization_UID == BG_Organization_UID);

                        if (storageinbound != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行数据重复,已经开账了", i);
                            return errorInfo;
                        }

                        FixturePartStorageInboundDTO storageinbounditem = new FixturePartStorageInboundDTO();
                        storageinbounditem.Plant_Organization_UID = Plant_Organization_UID;
                        storageinbounditem.Plant = Plant;
                        storageinbounditem.BG_Organization = BG_Organization;
                        storageinbounditem.BG_Organization_UID = BG_Organization_UID;
                        storageinbounditem.FunPlant_Organization = FunPlant_Organization;
                        storageinbounditem.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        storageinbounditem.Status_UID = fixtureStatus.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
                        storageinbounditem.Fixture_Storage_Inbound_Type_UID = fixtureInOutStatus.Where(o => o.Status == "开账").FirstOrDefault().Status_UID;
                        storageinbounditem.Fixture_Part_UID = Fixture_Part_UID;
                        storageinbounditem.Fixture_Warehouse_Storage_UID = Fixture_Warehouse_Storage_UID;
                        storageinbounditem.Remarks = "期初開帳-" + Part_ID + "-" + Part_Name;
                        storageinbounditem.Issue_NO = "None";
                        storageinbounditem.Inbound_Qty = Inbound_Qty;
                        storageinbounditem.Inbound_Price = 0;
                        string PostInboundID = (test.Count() + i - 1).ToString().PadLeft(4, '0');
                        storageinbounditem.Fixture_Storage_Inbound_ID = PreInboundID + PostInboundID;
                        storageinbounditem.Modified_UID = CurrentUser.AccountUId;
                        storageinbounditem.Applicant_UID = CurrentUser.AccountUId;
                        storageinbounditem.Approve_UID = CurrentUser.AccountUId;

                        storageinboundlist.Add(storageinbounditem);

                    }
                    //插入表?
                    var json = JsonConvert.SerializeObject(storageinboundlist);
                    var apiInsertEquipmentUrl = string.Format("FixturePart/InsertCreateBoundAPI");
                    HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(json, apiInsertEquipmentUrl);
                    errorInfo = responMessage1.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.Message;
            }
            return errorInfo;
        }

        public string ApproveCreateboundByUid(int Fixture_Storage_Inbound_UID)
        {
            var apiUrl = string.Format("FixturePart/ApproveCreateboundByUidAPI?Fixture_Storage_Inbound_UID={0}&Useruid={1}", Fixture_Storage_Inbound_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        #endregion

        #region Add by Rock 2018-01-10 ----------------采购单维护作业----------start
        public ActionResult PurchaseOrderMaintain()
        {
            var vm = GetFixtureStatus();
            return View(vm);
        }

        public ActionResult QueryPurchase(FixturePart_OrderVM search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }

            ////是否完成
            //if (search.Is_ComplatedValue == 0)
            //{
            //    search.Is_Complated = false;
            //}
            //else
            //{
            //    search.Is_Complated = true;
            //}

            //是否删除
            if (search.Del_Flag_Value == 0)
            {
                search.Del_Flag = false;
            }
            else
            {
                search.Del_Flag = true;
            }

            var url = string.Format("FixturePart/QueryPurchaseAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }

        public ActionResult doAllPurchaseordermaintain(FixturePart_OrderVM search)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }

            //if (search.Is_ComplatedValue == "0")
            //{
            //    search.Is_Complated = false;
            //}
            //else if (search.Is_ComplatedValue == "1")
            //{
            //    search.Is_Complated = true;
            //}

            //get Export datas
            var apiUrl = "FixturePart/doAllPurchaseorderMaintainAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePart_OrderEdit>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PurchaseorderMaintain");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            var stringHeads = getTableHead();
            var stringHeads2 = getTableHead2();
            var stringHeads3 = getTableHead3();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("PurchaseorderMaintain");
                var worksheet2 = excelPackage.Workbook.Worksheets.Add("PurchaseorderMaintain1");
                var worksheet3 = excelPackage.Workbook.Worksheets.Add("PurchaseorderMaintain2");

                //设置表头
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //设置表头
                for (int colIndex = 0; colIndex < stringHeads2.Length; colIndex++)
                {
                    worksheet2.Cells[1, colIndex + 1].Value = stringHeads2[colIndex];
                }

                //设置表头
                for (int colIndex = 0; colIndex < stringHeads3.Length; colIndex++)
                {
                    worksheet3.Cells[1, colIndex + 1].Value = stringHeads3[colIndex];
                }

                //set cell value
                int index2_Count = 0;
                int index3_count = 0;
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq 
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OpType_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Func_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Order_ID;
                    worksheet.Cells[index + 2, 6].Value = ((DateTime)currentRecord.Order_Date).ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Complated ? "已完成" : "未完成";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Remarks;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Del_Flag ? "已删除" : "未删除";

                    worksheet.Cells[index + 2, 10].Value = currentRecord.CreatName;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Created_Date.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 12].Value = currentRecord.ModifyName;
                    worksheet.Cells[index + 2, 13].Value = ((DateTime)currentRecord.ModifyTime).ToString("yyyy-MM-dd hh:mm");

                    var list2 = currentRecord.FixturePartOrderDList;
                    for (var index2 = 0; index2 < list2.Count; index2++)
                    {

                        var currentRecord2 = list2[index2];
                        //seq 
                        worksheet2.Cells[index2_Count + 2, 1].Value = index2_Count + 1;
                        worksheet2.Cells[index2_Count + 2, 2].Value = currentRecord.Order_ID;
                        worksheet2.Cells[index2_Count + 2, 3].Value = currentRecord2.Part_ID;
                        worksheet2.Cells[index2_Count + 2, 4].Value = currentRecord2.Part_Name;
                        worksheet2.Cells[index2_Count + 2, 5].Value = currentRecord2.Part_Spec;
                        worksheet2.Cells[index2_Count + 2, 6].Value = currentRecord2.Vendor_Name;
                        worksheet2.Cells[index2_Count + 2, 7].Value = currentRecord2.Price;
                        worksheet2.Cells[index2_Count + 2, 8].Value = currentRecord2.Qty;
                        worksheet2.Cells[index2_Count + 2, 9].Value = currentRecord2.SumActualQty;
                        worksheet2.Cells[index2_Count + 2, 10].Value = currentRecord2.Forcast_Complation_Date == null ? "" : ((DateTime)currentRecord2.Forcast_Complation_Date).ToString("yyyy-MM-dd hh:mm"); ;
                        worksheet2.Cells[index2_Count + 2, 11].Value = currentRecord2.Del_Flag ? "已删除" : "未删除"; ;
                        index2_Count++;
                    }

                    var list3 = currentRecord.FixturePartOrderScheduleList;
                    for (int index3 = 0; index3 < list3.Count; index3++)
                    {
                        var currentRecord3 = list3[index3];
                        //seq 
                        worksheet3.Cells[index3_count + 2, 1].Value = index3_count + 1;
                        worksheet3.Cells[index3_count + 2, 2].Value = currentRecord.Order_ID;
                        worksheet3.Cells[index3_count + 2, 3].Value = list2.Where(p => p.Fixture_Part_Order_D_UID == currentRecord3.Fixture_Part_Order_D_UID).FirstOrDefault().Part_ID;
                        worksheet3.Cells[index3_count + 2, 4].Value = currentRecord3.Receive_Date == null ? "" : ((DateTime)currentRecord3.Receive_Date).ToString("yyyy-MM-dd hh:mm");
                        worksheet3.Cells[index3_count + 2, 5].Value = currentRecord3.Forcast_Receive_Qty;
                        worksheet3.Cells[index3_count + 2, 6].Value = currentRecord3.Actual_Receive_Qty;
                        //worksheet3.Cells[index3_count + 2, 7].Value = currentRecord3.Is_Complated ? "已完成" : "未完成";

                        worksheet3.Cells[index3_count + 2, 7].Value = currentRecord3.DeliveryPeriod_Name;
                        worksheet3.Cells[index3_count + 2, 8].Value = currentRecord3.DeliveryPeriod_Date == null ? "" : ((DateTime)currentRecord3.DeliveryPeriod_Date).ToString("yyyy-MM-dd hh:mm");
                        worksheet3.Cells[index3_count + 2, 9].Value = currentRecord3.Delivery_Name;
                        worksheet3.Cells[index3_count + 2, 10].Value = currentRecord3.Delivery_Date == null ? "" : ((DateTime)currentRecord3.Delivery_Date).ToString("yyyy-MM-dd hh:mm");
                        index3_count++;
                    }
                }

                worksheet3.Cells.AutoFitColumns();
                worksheet2.Cells.AutoFitColumns();
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }



        public ActionResult doPartPurchaseordermaintain(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/doPartPurchaseorderMaintainAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePart_OrderEdit>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PurchaseorderMaintain");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            var stringHeads = getTableHead();
            var stringHeads2 = getTableHead2();
            var stringHeads3 = getTableHead3();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("PurchaseorderMaintain");
                var worksheet2 = excelPackage.Workbook.Worksheets.Add("PurchaseorderMaintain1");
                var worksheet3 = excelPackage.Workbook.Worksheets.Add("PurchaseorderMaintain2");

                //设置表头
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //设置表头
                for (int colIndex = 0; colIndex < stringHeads2.Length; colIndex++)
                {
                    worksheet2.Cells[1, colIndex + 1].Value = stringHeads2[colIndex];
                }

                //设置表头
                for (int colIndex = 0; colIndex < stringHeads3.Length; colIndex++)
                {
                    worksheet3.Cells[1, colIndex + 1].Value = stringHeads3[colIndex];
                }

                //set cell value
                //set cell value
                int index2_Count = 0;
                int index3_count = 0;
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq 
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OpType_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Func_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Order_ID;
                    worksheet.Cells[index + 2, 6].Value = ((DateTime)currentRecord.Order_Date).ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Complated ? "已完成" : "未完成";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Remarks;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Del_Flag ? "已删除" : "未删除";

                    worksheet.Cells[index + 2, 10].Value = currentRecord.CreatName;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Created_Date.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 12].Value = currentRecord.ModifyName;
                    worksheet.Cells[index + 2, 13].Value = ((DateTime)currentRecord.ModifyTime).ToString("yyyy-MM-dd hh:mm");

                    var list2 = currentRecord.FixturePartOrderDList;
                    for (var index2 = 0; index2 < list2.Count; index2++)
                    {

                        var currentRecord2 = list2[index2];
                        //seq 
                        worksheet2.Cells[index2_Count + 2, 1].Value = index2_Count + 1;
                        worksheet2.Cells[index2_Count + 2, 2].Value = currentRecord.Order_ID;
                        worksheet2.Cells[index2_Count + 2, 3].Value = currentRecord2.Part_ID;
                        worksheet2.Cells[index2_Count + 2, 4].Value = currentRecord2.Part_Name;
                        worksheet2.Cells[index2_Count + 2, 5].Value = currentRecord2.Part_Spec;
                        worksheet2.Cells[index2_Count + 2, 6].Value = currentRecord2.Vendor_Name;
                        worksheet2.Cells[index2_Count + 2, 7].Value = currentRecord2.Price;
                        worksheet2.Cells[index2_Count + 2, 8].Value = currentRecord2.Qty;
                        worksheet2.Cells[index2_Count + 2, 9].Value = currentRecord2.SumActualQty;
                        worksheet2.Cells[index2_Count + 2, 10].Value = currentRecord2.Forcast_Complation_Date == null ? "" : ((DateTime)currentRecord2.Forcast_Complation_Date).ToString("yyyy-MM-dd hh:mm"); ;
                        worksheet2.Cells[index2_Count + 2, 11].Value = currentRecord2.Del_Flag ? "已删除" : "未删除"; ;
                        index2_Count++;
                    }

                    var list3 = currentRecord.FixturePartOrderScheduleList;
                    for (int index3 = 0; index3 < list3.Count; index3++)
                    {
                        var currentRecord3 = list3[index3];
                        //seq 
                        worksheet3.Cells[index3_count + 2, 1].Value = index3_count + 1;
                        worksheet3.Cells[index3_count + 2, 2].Value = currentRecord.Order_ID;
                        worksheet3.Cells[index3_count + 2, 3].Value = list2.Where(p => p.Fixture_Part_Order_D_UID == currentRecord3.Fixture_Part_Order_D_UID).FirstOrDefault().Part_ID;
                        worksheet3.Cells[index3_count + 2, 4].Value = currentRecord3.Receive_Date == null ? "" : ((DateTime)currentRecord3.Receive_Date).ToString("yyyy-MM-dd hh:mm");
                        worksheet3.Cells[index3_count + 2, 5].Value = currentRecord3.Forcast_Receive_Qty;
                        worksheet3.Cells[index3_count + 2, 6].Value = currentRecord3.Actual_Receive_Qty;
                        //worksheet3.Cells[index3_count + 2, 7].Value = currentRecord3.Is_Complated ? "已完成" : "未完成";

                        worksheet3.Cells[index3_count + 2, 7].Value = currentRecord3.DeliveryPeriod_Name;
                        worksheet3.Cells[index3_count + 2, 8].Value = currentRecord3.DeliveryPeriod_Date == null ? "" : ((DateTime)currentRecord3.DeliveryPeriod_Date).ToString("yyyy-MM-dd hh:mm");
                        worksheet3.Cells[index3_count + 2, 9].Value = currentRecord3.Delivery_Name;
                        worksheet3.Cells[index3_count + 2, 10].Value = currentRecord3.Delivery_Date == null ? "" : ((DateTime)currentRecord3.Delivery_Date).ToString("yyyy-MM-dd hh:mm");
                        index3_count++;
                    }
                }

                worksheet3.Cells.AutoFitColumns();
                worksheet2.Cells.AutoFitColumns();
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        private string[] getTableHead()
        {
            var stringHeads = new string[] {
                        "序号",
                        "厂区",
                        "OP类型",
                        "功能厂",
                        "采购单号",
                        "订单日期",
                        "是否完成",
                        "备注",
                        "删除状态",
                        "提单人员",
                        "提单时间",
                        "修改人员",
                        "修改时间",
                };

            return stringHeads;
        }

        private string[] getTableHead2()
        {
            var stringHeads = new string[] {
                        "序号",
                        "采购单号",
                        "料号",
                        "品名",
                        "型号",
                        "供应商",
                        "采购单价",
                        "采购数量",
                        "实际交货量",
                        "预计完成时间",
                        "删除状态",
                };

            return stringHeads;
        }

        private string[] getTableHead3()
        {
            var stringHeads = new string[] {
                        "序号",
                        "采购单号",
                        "料号",
                        "预计交货日期",
                        "预计交货量",
                        "实际交货量",
                        //"是否完成",
                        "交期资料维护者",
                        "交期资料维护日期",
                        "交货资料维护者",
                        "交货资料维护日期",
                };

            return stringHeads;
        }
        public string QueryPurchaseByUID(int Fixture_Part_Order_M_UID)
        {
            var url = string.Format("FixturePart/QueryPurchaseByUIDAPI?UID={0}", Fixture_Part_Order_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string GetFixturePartByPlantOptypeFunc(int PlantUID, int Optype, int FuncUID)
        {
            var url = string.Format("FixturePart/GetFixturePartByPlantOptypeFuncAPI?PlantUID={0}&Optype={1}&FuncUID={2}", PlantUID, Optype, FuncUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;

        }

        public string GetFixturePartByMUID(int UID)
        {
            var url = string.Format("FixturePart/GetFixturePartByMUIDAPI?UID={0}", UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string SaveFixturePartByMUID(SubmitFixturePartOrderVM vm)
        {
            vm.Created_UID = this.CurrentUser.AccountUId;
            var url = string.Format("FixturePart/SaveFixturePartByMUID");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string DeletePurchaseByUIDS(string json)
        {
            var apiUrl = string.Format("FixturePart/DeletePurchaseByUIDSAPI?json={0}", json);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }

        public string ImportPurchaseExcel(HttpPostedFileBase uploadName)
        {
            string errorInfo = string.Empty;
            try
            {
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "订单日期",
                        "备注",
                    };
                    bool isExcelError = false;
                    for (int i = 1; i <= 5; i++)
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

                    SubmitFixturePartOrderVM vm = new SubmitFixturePartOrderVM();
                    //获得厂区
                    var plantsAPI = string.Format("Fixture/QueryAllPlantAPI?PLANT_UID={0}&leval={1}", GetPlantOrgUid(), "1");
                    HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(plantsAPI);
                    var jsonPlants = plantsmessage.Content.ReadAsStringAsync().Result;
                    var plants = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonPlants);

                    for (int i = 2; i <= 2; i++)
                    {
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        //string Order_Date = string.Empty;
                        //string Remarks = string.Empty;

                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        var Order_Date = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "订单日期")].Value);
                        var Remarks = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "备注")].Value);

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

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行OP代号不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            BG_Organization = BG_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", Plant_Organization_UID, 1);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
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

                        if (!string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            FunPlant_Organization = FunPlant_Organization.Trim();
                            var apiUrl = string.Format("Fixture/GetFunPlantByOPTypeAPI?Optype={0}", BG_Organization_UID);
                            HttpResponseMessage responMessageFunPlant = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessageFunPlant.Content.ReadAsStringAsync().Result;
                            var optypes = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(result2);

                            var bg = optypes.FirstOrDefault(m => m.FunPlant == FunPlant_Organization);

                            if (bg != null)
                            {
                                FunPlant_Organization_UID = bg.FunPlant_OrganizationUID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(Order_Date))
                        {
                            DateTime validTime = DateTime.Now;
                            var isDate = DateTime.TryParse(Order_Date, out validTime);
                            if (!isDate)
                            {
                                errorInfo = string.Format("第{0}行日期格式", i);
                                return errorInfo;
                            }
                        }



                        vm.Plant_Organization_UID = Plant_Organization_UID;
                        vm.BG_Organization_UID = BG_Organization_UID;
                        if (FunPlant_Organization != null)
                        {
                            vm.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        }
                        vm.Order_Date = Convert.ToDateTime(Order_Date);
                        vm.Remarks = Remarks;


                    }
                }
            }
            catch (Exception ex)
            {

            }
            return errorInfo;
        }


        private Fixture_PartVM GetFixtureStatus()
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
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return currentVM;
        }
        #endregion Add by Rock 2018-01-10 ----------------采购单维护作业----------end

        public ActionResult StoryDetialReport()
        {
            FixtureStorageTransferVM currentVM = new FixtureStorageTransferVM();
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
            return View("StoryDetialReport", currentVM);
        }

        /// <summary>
        /// 获取库存明细信息
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetStorageDetialReportData(Fixture_Part_InventoryDTO search, Page page)
        {
            var apiUrl = string.Format("FixturePart/GetFPStoryDetialReportAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFixtureinventory(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID)
        {
            var apiUrl = string.Format("FixturePart/GetFixtureinventoryAPI?Fixture_Part_UID={0}&Fixture_Warehouse_Storage_UID={1}", Fixture_Part_UID, Fixture_Warehouse_Storage_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }



        #region 库存报表查询
        public ActionResult FixturePartStorageReport()
        {
            FixtureStorageTransferVM currentVM = new FixtureStorageTransferVM();
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
            return View("FixturePartStorageReport", currentVM);
        }
        public ActionResult QueryStorageReports(FixturePartStorageReportDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("FixturePart/QueryStorageReportsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //导出excel----所有符合条件的项目
        public ActionResult DoSRExportFunction(int plant, int bg, int funplant, string Part_ID, string Part_Name, string Part_Spec, DateTime start_date, DateTime end_date)
        {
            if (plant == 0)
            {
                plant = GetPlantOrgUid();
            }
            var apiUrl = string.Format(@"FixturePart/DoSRExportFunctionAPI?plant={0}&bg={1}&funplant={2}&Part_ID={3}&Part_Name={4}&Part_Spec={5}
                                                                    &start_date={6}&end_date={7}", plant, bg, funplant, Part_ID, Part_Name, Part_Spec, start_date, end_date);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixturePartStorageReportDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixturePartStorageReport");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "料号", "品名", "型号", "期初库存", "本期入库", "本期出库", "本期结存" };
            if (funplant != 0)
                stringHeads = new string[] { "序号", "厂区", "BG", "功能厂", "料号", "品名", "型号", "期初初库存", "本期入库", "本期出库", "本期结存" };
            else if (bg != 0)
                stringHeads = new string[] { "序号", "厂区", "BG", "料号", "品名", "型号", "期初库存", "本期入库", "本期出库", "本期结存" };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixturePartStorageReport");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant;
                    if (funplant != 0)
                    {
                        worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                        worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization;
                        worksheet.Cells[index + 2, 5].Value = currentRecord.Part_ID;
                        worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Name;
                        worksheet.Cells[index + 2, 7].Value = currentRecord.Part_Spec;
                        worksheet.Cells[index + 2, 8].Value = currentRecord.Balance_Qty;
                        worksheet.Cells[index + 2, 9].Value = currentRecord.In_Qty;
                        worksheet.Cells[index + 2, 10].Value = currentRecord.Out_Qty;
                        worksheet.Cells[index + 2, 11].Value = currentRecord.Last_Qty;
                    }
                    else if (bg != 0)
                    {
                        worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                        worksheet.Cells[index + 2, 4].Value = currentRecord.Part_ID;
                        worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Name;
                        worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Spec;
                        worksheet.Cells[index + 2, 7].Value = currentRecord.Balance_Qty;
                        worksheet.Cells[index + 2, 8].Value = currentRecord.In_Qty;
                        worksheet.Cells[index + 2, 9].Value = currentRecord.Out_Qty;
                        worksheet.Cells[index + 2, 10].Value = currentRecord.Last_Qty;
                    }
                    else
                    {
                        worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                        worksheet.Cells[index + 2, 4].Value = currentRecord.Part_ID;
                        worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Name;
                        worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Spec;
                        worksheet.Cells[index + 2, 7].Value = currentRecord.Balance_Qty;
                        worksheet.Cells[index + 2, 8].Value = currentRecord.In_Qty;
                        worksheet.Cells[index + 2, 9].Value = currentRecord.Out_Qty;
                        worksheet.Cells[index + 2, 10].Value = currentRecord.Last_Qty;
                        //worksheet.Cells[index + 2, 3].Value = currentRecord.Part_ID;
                        //worksheet.Cells[index + 2, 4].Value = currentRecord.Part_Name;
                        //worksheet.Cells[index + 2, 5].Value = currentRecord.Part_Spec;
                        //worksheet.Cells[index + 2, 6].Value = currentRecord.Balance_Qty;
                        //worksheet.Cells[index + 2, 7].Value = currentRecord.In_Qty;
                        //worksheet.Cells[index + 2, 8].Value = currentRecord.Out_Qty;
                        //worksheet.Cells[index + 2, 9].Value = currentRecord.Last_Qty;
                    }
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion

        /// <summary>
        /// 导出库存明细信息
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult ExportALLFixtureinventoryReportData(FixtureInOutStorageModel search)
        {
            var apiUrl = string.Format("FixturePart/ExportAllFixtureInventoryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Part_InventoryDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureInventoryReport");
            var stringHeads = new string[]
             {
                "序号", "厂区", "OP类型", "功能厂", "料号", "品名", "型号", "储位", "料架", "仓库编码", "仓库名称", "庫存数量",
                "更新者", "更新时间", "说明"
             };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureInventoryReport");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Inventory_Qty;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Modified_Date.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Remarks;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        /// <summary>
        /// 导出已经勾选的库存库明细信息
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult ExportSelectedFixtureinventoryReportData(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/ExportSelectedFixtureInventoryAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Part_InventoryDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureInventoryReport");
            var stringHeads = new string[]
            {
                "序号", "厂区", "OP类型", "功能厂", "料号", "品名", "型号", "储位", "料架", "仓库编码", "仓库名称", "庫存数量",
                "更新者", "更新时间", "说明"
            };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureInventoryReport");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Inventory_Qty;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Modified_Date.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Remarks;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }

        public ActionResult FixtureInAndOutStorageDetial()
        {
            FixPartInOutReportDetailVM currentVM = new FixPartInOutReportDetailVM();
            var apiUrlIn_outStatus = string.Format("FixturePart/GetFixtureStatuDTOAPI?Enum_Type={0}", "FixturePartIn_out_Type");
            HttpResponseMessage responMessageIn_outStatus = APIHelper.APIGetAsync(apiUrlIn_outStatus);
            var resultIn_outStatus = responMessageIn_outStatus.Content.ReadAsStringAsync().Result;
            var fixtureIn_outStatus = JsonConvert.DeserializeObject<List<FixturePartCreateboundStatuDTO>>(resultIn_outStatus);
            currentVM.FixturePartInOutboundStatus = fixtureIn_outStatus;

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
            return View("FixtureInAndOutStorageDetial", currentVM);
        }

        /// <summary>
        /// 出入库明细查询
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetInOutDetialReportData(FixtureInOutStorageModel search, Page page)
        {
            var apiUrl = string.Format("FixturePart/GetInOutDetialReportDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 导出出入库明细信息
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult ExportALLInOutDetialReportData(FixtureInOutStorageModel search)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("FixturePart/ExportALLInOutDetialReportDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureInOutStorageModel>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureInOutDetialReport");
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "出入库表单编号", "出入库类型", "料号", "品名", "型号", "储位", "料架", "仓库编码", "仓库名称", "出入库时间", "入库数量", "出库数量", "结存数量", "最后更新者", "最后更新时间" };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureInOutDetialReport");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Storage_In_Out_Bound_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.InOut_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.In_Out_StorageTime.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 15].Value = currentRecord.In_Bound_Qty;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Out_Bound_Qty;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Remaining_Bound_Qty;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Modified_User_Name;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Modified_Date.ToString("yyyy-MM-dd hh:mm");
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        /// <summary>
        /// 导出已经勾选的出入库明细信息
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult ExportSelectedInOutDetialReportData(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("FixturePart/ExportSelectedInOutDetialReportDataAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureInOutStorageModel>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureInOutDetialReport");
            var stringHeads = new string[]
            {
                "序号", "厂区", "OP类型", "功能厂", "出入库表单编号", "出入库类型", "料号", "品名", "型号", "储位", "料架", "仓库编码", "仓库名称", "出入库时间",
                "入库数量", "出库数量", "结存数量", "最后更新者", "最后更新时间"
            };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureInOutDetialReport");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Storage_In_Out_Bound_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.InOut_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Part_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Part_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Part_Spec;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Fixture_Warehouse_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Fixture_Warehouse_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.In_Out_StorageTime.ToString("yyyy-MM-dd hh:mm");
                    worksheet.Cells[index + 2, 15].Value = currentRecord.In_Bound_Qty;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Out_Bound_Qty;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Remaining_Bound_Qty;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Modified_User_Name;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Modified_Date.ToString("yyyy-MM-dd hh:mm");
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }

        public ActionResult FixturePartScanCode()
        {
            //FixturePartScanCodeVM currentVM = new FixturePartScanCodeVM();    
            //int optypeID = 0;
            //int funPlantID = 0;
            //if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            //{
            //    if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
            //    {
            //        optypeID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            //    }

            //    if (CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null)
            //    {
            //        funPlantID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
            //    }
            //    currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };

            //}
            //else
            //{
            //    //获取
            //    var Plants = new List<PlantVM>();
            //    var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
            //    HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            //    var result = responMessage.Content.ReadAsStringAsync().Result;
            //    var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            //    foreach (var item in systemOrgDTOs)
            //    {

            //        PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
            //        Plants.Add(plantvm);
            //    }
            //    currentVM.Plants = Plants;
            //}
            //currentVM.OptypeID = optypeID;
            //currentVM.FunPlantID = funPlantID;
            return View("FixturePartScanCode");

        }
        public ActionResult GetFixturePartScanCodeDTO(string SN)
        {
            string result = "0";
            int optypeID = 0;
            int plantID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
                {
                    optypeID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }

                if (CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                {
                    plantID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
                }
            }
            if (optypeID != 0 && plantID != 0)
            {
                var apiUrl = string.Format("FixturePart/GetFixturePartScanCodeDTOAPI?plantID={0}&optypeID={1}&SN={2}&Modified_UID={3}", plantID, optypeID, SN, this.CurrentUser.AccountUId);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
            }
            return Content(result, "application/json");
        }

        #region 治具配件更换清零Jay
        public ActionResult FixturePartScanCodeClear()
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
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;

            return View("FixturePartScanCodeClear", currentVM);
        }
        public ActionResult GetFixtureScanCodeDTOBySN(string SN)
        {
            string result = "";
            int optypeID = 0;
            int plantID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
                {
                    optypeID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }

                if (CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                {
                    plantID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
                }
            }
            if (optypeID != 0 && plantID != 0)
            {
                var apiUrl = string.Format("FixturePart/GetFixtureScanCodeDTOBySNAPI?plantID={0}&optypeID={1}&SN={2}", plantID, optypeID, SN);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
            }
            else
            {
                result = "-1";//未设置厂区,OP
            }
            return Content(result, "application/json");
        }

        public ActionResult GetFixturePartDTOByFixtureUID(int fixtureUID)
        {
            string result = "";
            if (fixtureUID > 0)
            {
                var apiUrl = string.Format("FixturePart/GetFixturePartDTOByFixtureUIDAPI?fixtureUID={0}", fixtureUID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
            }
            return Content(result, "application/json");
        }

        public ActionResult ClearFixturePartUseTimes(List<int> uidList)
        {
            string result = "";
            if (uidList.Count>0)
            {
                var parameter = new { uidList = uidList, modifiedUid = CurrentUser.AccountUId, modifiedDate = DateTime.Now };
                var apiUrl = string.Format("FixturePart/ClearFixturePartUseTimesAPI");//?uidList={0}&modifiedUid={1}&modifiedDate={2}", uidList, CurrentUser.AccountUId, DateTime.Now);
                HttpResponseMessage responMessage = APIHelper.APIPostDynamicAsync(parameter, apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
            }
            
            return Content(result, "application/json");
        }
        #endregion
    }
}