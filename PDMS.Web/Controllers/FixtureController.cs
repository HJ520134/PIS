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
using System.Web.Script.Serialization;

namespace PDMS.Web.Controllers
{
    public class FixtureController : WebControllerBase
    {
        #region  治具资料维护----------Add by keyong 2017-09-30
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
        public ActionResult DoAllExportFixtureUpdate(FixtureDTO search)
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

            //get Export datas

            var apiUrl = "Fixture/DoAllExportFixtureReprotAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "厂区代号", "OP代号", "功能厂代号", "治具编码", "版本号", "治具流水序号", "治具名称",
                "专案代码", "生产车间代码", "生产线代码", "机台代码", "供应商代码", "治具状态", "治具短码", "治具二维码","治具资料主键" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Maintenance");

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
                    // worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 1].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Version;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Fixture_Seq;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Fixture_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Project_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Workshop_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Line_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Vendor_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.StatuName;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.TwoD_Barcode;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Fixture_M_UID;
                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 16; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:P1"].Style.Font.Bold = true;
                worksheet.Cells["A1:P1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:P1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:P1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 导出治具资料
        /// </summary>
        /// <param name="Fixture_M_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoAllExportFixtureReprot(FixtureDTO search)
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

            //get Export datas

            var apiUrl = "Fixture/DoAllExportFixtureReprotAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "线别", "工站", "制程", "机台编码", "治具唯一编号", "治具编号(图号)", "治具短码", "治具状态", "图号版本", "供应商", "最后更新者", "最后更新日期" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Workstation;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Process_Info;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.StatuName;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Version;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Vendor_Name;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Modifieder;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 16; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:P1"].Style.Font.Bold = true;
                worksheet.Cells["A1:P1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:P1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:P1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 导出治具资料
        /// </summary>
        /// <param name="Fixture_M_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoExportFixtureReprot(string Fixture_M_UIDs)
        {


            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportFixtureReprotAPI?Fixture_M_UIDs={0}", Fixture_M_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "线别", "工站", "制程", "机台编码", "治具唯一编号", "治具编号(图号)", "治具短码", "治具状态", "图号版本", "供应商", "最后更新者", "最后更新日期" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Workstation;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Process_Info;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.StatuName;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Version;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Vendor_Name;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Modifieder;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);


                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 16; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:P1"].Style.Font.Bold = true;
                worksheet.Cells["A1:P1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:P1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:P1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 导出治具资料
        /// </summary>
        /// <param name="Fixture_M_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoExportFixtureUpdate(string Fixture_M_UIDs)
        {

            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportFixtureReprotAPI?Fixture_M_UIDs={0}", Fixture_M_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] {  "厂区代号", "OP代号", "功能厂代号", "治具编码", "版本号", "治具流水序号", "治具名称",
                "专案代码", "生产车间代码", "生产线代码", "机台代码", "供应商代码", "治具状态", "治具短码", "治具二维码","治具资料主键" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Maintenance");

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
                    worksheet.Cells[index + 2, 1].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Version;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Fixture_Seq;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Fixture_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Project_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Workshop_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Line_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Vendor_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.StatuName;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.TwoD_Barcode;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Fixture_M_UID;
                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 16; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:P1"].Style.Font.Bold = true;
                worksheet.Cells["A1:P1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:P1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:P1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:P1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 批量启用
        /// </summary>
        /// <param name="Fixture_M_UIDs"></param>
        /// <returns></returns>
        public ActionResult BatchEnableFixturematerial(FixtureDTO search, string IsEnable)
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

            search.AccountID = CurrentUser.AccountUId;

            //get Export datas ?fixture_UID={0}
            var apiUrl = string.Format("Fixture/BatchEnableFixturematerial?IsEnable={0}", IsEnable);
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        /// <summary>
        /// 启用勾选部分
        /// </summary>
        /// <param name="Fixture_M_UIDs"></param>
        /// <returns></returns>
        public ActionResult BatchEnablePartFixturematerial(string Fixture_M_UIDs, string IsEnable)
        {
            var AccountID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Fixture/BatchEnablePartFixturematerialAPI?Fixture_M_UIDs={0}&IsEnable={1}&AccountID={2}", Fixture_M_UIDs, IsEnable, AccountID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 治具资料维护主界面
        /// </summary>
        /// <returns></returns>
        public ActionResult FixtureDataMaintain()
        {
            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.SearchFixtureStatus = fixtureStatus;
            currentVM.FixtureStatus = fixtureStatus.Where(o => o.StatuName == "使用中In-PRD" || o.StatuName == "未使用Non-PRD").ToList();
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
            //ViewBag.PageTitle = "治具资料维护";
            return View("FixtureDataMaintain", currentVM);
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
        public ActionResult GetFunPlantByOPTypes(int Optype, string Optypes = "")
        {
            var apiUrl = string.Format("Fixture/GetFunPlantByOPTypeAPI?Optype={0}&Optypes={1}", Optype, Optypes);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 初始化查询数据治具资料
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryFixture(FixtureDTO search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult FixtureList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var apiUrl = string.Format("Fixture/FixtureListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureByUid(int Fixture_M_UID)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureByUidAPI?fixture_UID={0}", Fixture_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetVendorInfoList(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetVendor_InfoListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", Plant_Organization_UID, BG_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取产线
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetProductionLineList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetProductionLineDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取绑定当前用户
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetFixtureSystemUser(int Plant_Organization_UID = 0, int BG_Organization_UID = 0, int FunPlant_Organization_UID = 0)
        {
            if (Plant_Organization_UID == 0)
            {
                Plant_Organization_UID = GetPlantOrgUid();
            }
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
            {
                BG_Organization_UID = (int)CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID;

            }
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null)
            {
                FunPlant_Organization_UID = (int)CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID;

            }
            var apiUrl = string.Format("Fixture/GetFixtureSystemUserAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取生产地点
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetWorkshopList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetWorkshopListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取工站
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetWorkstationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetWorkstationListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetWorkstationListBySearch(WorkStationModelSearch search)
        {
            var apiUrl = string.Format("Fixture/GetWorkstationListByQueryAPI");
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //var list = JsonConvert.DeserializeObject<List<WorkStationDTO>>(result);
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取制程
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetProcess_InfoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetProcess_InfoListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取专案
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public ActionResult GetProjectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetProjectListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetProductionLineByUid(int Production_Line_UID)
        {
            var apiUrl = string.Format("Fixture/GetProductionLineDTOAPI?Production_Line_UID={0}", Production_Line_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 获取治具机台
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <param name="Production_Line_UID"></param>
        /// <returns></returns>
        public ActionResult GetFixtureMachineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, int Production_Line_UID)
        {
            var apiUrl = string.Format("Fixture/GetFixtureMachineDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}&Production_Line_UID={3}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Production_Line_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureMachineByUid(int Fixture_Machine_UID)
        {
            var apiUrl = string.Format("Fixture/GetFixtureMachineByUidAPI?Fixture_Machine_UID={0}", Fixture_Machine_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string AddOrEditFixtureM(FixtureDTO dto, bool isEdit)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            dto.Fixture_Unique_ID = dto.Fixture_NO + dto.Fixture_Seq;
            var apiUrl = string.Format("Fixture/AddOrEditFixtureMAPI?isEdit={0}", isEdit);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        ///导入功能
        public string ImportFixture(HttpPostedFileBase uploadName)
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "治具编码",
                        "版本号",
                        "治具流水序号",
                        "治具名称",
                        "专案代码",
                        "生产车间代码",
                        "生产线代码",
                        "机台代码",
                        "供应商代码",
                        "治具状态",
                        "治具短码",
                        "治具二维码"
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

                        //获取OPTYPE
                        int BGorganization_UID = 0;
                        if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                        {
                            BGorganization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                        }
                        var apiUrlGetOPType = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", plant_OrganizationUID, BGorganization_UID);
                        HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrlGetOPType);
                        var OPTypeResult = responMessage.Content.ReadAsStringAsync().Result;
                        List<SystemProjectDTO> systemOPtypes = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(OPTypeResult);


                        //根据厂区获取所供应商信息。
                        var vendor_InfoByPlantAPI = string.Format("Fixture/GetVendor_InfoByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage vendor_InfoMessage = APIHelper.APIGetAsync(vendor_InfoByPlantAPI);
                        var jsonsvendor_Infos = vendor_InfoMessage.Content.ReadAsStringAsync().Result;
                        var vendor_Infos = JsonConvert.DeserializeObject<List<Vendor_InfoDTO>>(jsonsvendor_Infos);

                        //根据厂区获取所有机台数据。
                        var fixtureMachineByPlantAPI = string.Format("Fixture/GetFixtureMachineByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixtureMachineMessage = APIHelper.APIGetAsync(fixtureMachineByPlantAPI);
                        var jsonsfixtureMachines = fixtureMachineMessage.Content.ReadAsStringAsync().Result;
                        var fixtureMachines = JsonConvert.DeserializeObject<List<FixtureMachineDTO>>(jsonsfixtureMachines);

                        //获取所有专案
                        var queryAllProjectsAPI = string.Format("Equipmentmaintenance/QueryAllProjectsAPI?optype={0}", 0);
                        HttpResponseMessage allProjectMessage = APIHelper.APIGetAsync(queryAllProjectsAPI);
                        var jsonsallProjects = allProjectMessage.Content.ReadAsStringAsync().Result;
                        var allProjects = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(jsonsallProjects);

                        //根据厂区获取所有生产车间。
                        var workShopByPlantAPI = string.Format("Fixture/GetWorkShopByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage workShopMessage = APIHelper.APIGetAsync(workShopByPlantAPI);
                        var jsonsworkShops = workShopMessage.Content.ReadAsStringAsync().Result;
                        var workShops = JsonConvert.DeserializeObject<List<WorkshopDTO>>(jsonsworkShops);

                        //根据厂区获取所有生产线。
                        var production_LineByPlantAPI = string.Format("Fixture/GetProduction_LineByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage production_LineMessage = APIHelper.APIGetAsync(production_LineByPlantAPI);
                        var jsonsproduction_Lines = production_LineMessage.Content.ReadAsStringAsync().Result;
                        var production_Lines = JsonConvert.DeserializeObject<List<Production_LineDTO>>(jsonsproduction_Lines);

                        //根据厂区获取所有治具数据
                        var fixture_MByPlantAPI = string.Format("Fixture/GetFixture_MByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixture_MMessage = APIHelper.APIGetAsync(fixture_MByPlantAPI);
                        var jsonsfixture_Ms = fixture_MMessage.Content.ReadAsStringAsync().Result;
                        var fixture_Ms = JsonConvert.DeserializeObject<List<FixtureDTO>>(jsonsfixture_Ms);
                        //获取治具状态
                        var fixture_StatusAPI = string.Format("Fixture/GetFixtureStatuDTOAPI");
                        HttpResponseMessage fixture_StatusMessage = APIHelper.APIGetAsync(fixture_StatusAPI);
                        var jsonsfixture_Status = fixture_StatusMessage.Content.ReadAsStringAsync().Result;
                        var fixture_Status = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(jsonsfixture_Status).Where(o => o.StatuName == "使用中In-PRD" || o.StatuName == "未使用Non-PRD").ToList();

                        var fixtureDTOList = new List<FixtureDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var fixtureDTO = new FixtureDTO();
                            int Plant_Organization_UID = 0;
                            int BG_Organization_UID = 0;
                            int? FunPlant_Organization_UID = null;
                            var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                            var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                            var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
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
                                var bg = systemOPtypes.Where(m => m.OP_TYPES == BG_Organization).FirstOrDefault();
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
                            string Fixture_NO = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具编码")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_NO))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编码没有值", i);
                                return errorInfo;
                            }
                            else if (Fixture_NO.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编码长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                            Fixture_NO = Fixture_NO.Trim();
                            string Version = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "版本号")].Value);
                            if (string.IsNullOrWhiteSpace(Version))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行版本号没有值", i);
                                return errorInfo;
                            }
                            else if (Version.Length > 2)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行版本号长度超过最大限定[2]", i);
                                return errorInfo;
                            }
                            Version = Version.Trim();
                            string Fixture_Seq = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具流水序号")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_Seq))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具流水序号没有值", i);
                                return errorInfo;
                            }
                            else if (Fixture_Seq.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具流水序号长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            Fixture_Seq = Fixture_Seq.Trim();
                            string Fixture_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具名称")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_Name))
                            {
                                Fixture_Name = null;

                            }
                            else
                            if (Fixture_Name.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具名称长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            if (Fixture_Name != null)
                            {
                                Fixture_Name = Fixture_Name.Trim();
                            }
                            string Project_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案代码")].Value);
                            if (string.IsNullOrWhiteSpace(Project_ID))
                            {
                                Project_ID = null;
                            }
                            else if (Project_ID.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行专案代码长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            if (Project_ID != null)
                            {
                                Project_ID = Project_ID.Trim();
                            }


                            string Workshop_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产车间代码")].Value);
                            if (string.IsNullOrWhiteSpace(Workshop_ID))
                            {
                                Workshop_ID = null;
                            }
                            else if (Workshop_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产车间代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (Workshop_ID != null)
                            {
                                Workshop_ID = Workshop_ID.Trim();
                            }


                            string Production_Line_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产线代码")].Value);
                            if (string.IsNullOrWhiteSpace(Production_Line_ID))
                            {
                                Production_Line_ID = null;

                            }
                            else if (Production_Line_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产线代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (Production_Line_ID != null)
                            {
                                Production_Line_ID = Production_Line_ID.Trim();
                            }

                            string Fixture_Machine_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台代码")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_Machine_ID))
                            {
                                Fixture_Machine_ID = null;

                            }
                            else if (Fixture_Machine_ID.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台代码长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                            if (Fixture_Machine_ID != null)
                            {
                                Fixture_Machine_ID = Fixture_Machine_ID.Trim();
                            }

                            string Vendor_Info_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "供应商代码")].Value);
                            if (string.IsNullOrWhiteSpace(Vendor_Info_ID))
                            {
                                Vendor_Info_ID = null;

                            }
                            else if (Vendor_Info_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行供应商代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (Vendor_Info_ID != null)
                            {
                                Vendor_Info_ID = Vendor_Info_ID.Trim();
                            }


                            string Status = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具状态")].Value);
                            if (string.IsNullOrWhiteSpace(Status))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具状态没有值", i);
                                return errorInfo;
                            }
                            else if (Status.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具状态长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            if (Status != null)
                            {
                                Status = Status.Trim();
                            }

                            string ShortCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具短码")].Value);
                            if (string.IsNullOrWhiteSpace(ShortCode))
                            {
                                ShortCode = null;

                            }
                            else if (ShortCode.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具短码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (ShortCode != null)
                            {
                                ShortCode = ShortCode.Trim();
                            }

                            string TwoD_Barcode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具二维码")].Value);

                            int? Project_UID = 0;
                            if (Project_ID != null && Project_ID != "")
                            {


                                var hasitem = allProjects.FirstOrDefault(m => m.Organization_UID == BG_Organization_UID && m.Project_Code == Project_ID);
                                if (hasitem == null)
                                {
                                    Project_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此专案代码[{1}]", i, Project_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    Project_UID = hasitem.Project_UID;
                                }
                                if (TwoD_Barcode != null)
                                {
                                    TwoD_Barcode = TwoD_Barcode.Trim();
                                }
                            }
                            int? Fixture_Machine_UID = null;
                            if (Fixture_Machine_ID != null && Fixture_Machine_ID != "")
                            {
                                var hasMachineitem = fixtureMachines.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Machine_ID == Fixture_Machine_ID);
                                if (hasMachineitem == null)
                                {
                                    Fixture_Machine_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此机台代码[{1}]", i, Fixture_Machine_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    Fixture_Machine_UID = hasMachineitem.Fixture_Machine_UID;
                                }
                            }
                            int? Vendor_Info_UID = 0;
                            if (Vendor_Info_ID != null && Vendor_Info_ID != "")
                            {
                                var hasVendor_Infoitem = vendor_Infos.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Vendor_ID == Vendor_Info_ID);
                                if (hasVendor_Infoitem == null)
                                {
                                    Vendor_Info_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此供应商代码[{1}]", i, Vendor_Info_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    Vendor_Info_UID = hasVendor_Infoitem.Vendor_Info_UID;
                                }
                            }

                            int? production_Line_UID = 0;
                            if (Production_Line_ID != null && Production_Line_ID != "")
                            {
                                var hasProduction_Lineitem = production_Lines.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Line_ID == Production_Line_ID);
                                if (hasProduction_Lineitem == null)
                                {
                                    production_Line_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此产线代码[{1}]", i, Production_Line_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    production_Line_UID = hasProduction_Lineitem.Production_Line_UID;
                                }
                            }

                            int fixture_Status_UID = 0;
                            var fixture_Statusitem = fixture_Status.FirstOrDefault(m => m.StatuName == Status);
                            if (fixture_Statusitem == null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有此治具状态[{1}],导入只有使用中In-PRD,未使用Non-PRD两种状态.", i, Status);
                                return errorInfo;
                            }
                            else
                            {
                                fixture_Status_UID = fixture_Statusitem.Status;
                            }

                            string Fixture_Unique_ID = Fixture_NO + Fixture_Seq;

                            var fixture_M = fixture_Ms.FirstOrDefault(m => m.Fixture_Unique_ID == Fixture_Unique_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (fixture_M != null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编号[{1}]，治具流水号[{2}]已经存在,不可重复导入", i, Fixture_NO, Fixture_Seq);
                                return errorInfo;
                            }
                            //导入数据判重
                            var isSelfRepeated = fixtureDTOList.Exists(m => m.Fixture_Unique_ID == Fixture_Unique_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编号[{1}]，治具流水号[{2}]在导入数据中重复,不可重复导入", i, Fixture_NO, Fixture_Seq);
                                return errorInfo;
                            }

                            fixtureDTO.Plant_Organization_UID = Plant_Organization_UID;
                            fixtureDTO.BG_Organization_UID = BG_Organization_UID;
                            fixtureDTO.FunPlant_Organization_UID = FunPlant_Organization_UID;
                            fixtureDTO.Fixture_NO = Fixture_NO;
                            fixtureDTO.Version = Version;
                            fixtureDTO.Fixture_Seq = Fixture_Seq;
                            fixtureDTO.Fixture_Unique_ID = Fixture_Unique_ID;
                            fixtureDTO.Fixture_Name = Fixture_Name;
                            fixtureDTO.Project_UID = Project_UID;
                            fixtureDTO.Fixture_Machine_UID = Fixture_Machine_UID;
                            fixtureDTO.Vendor_Info_UID = Vendor_Info_UID;
                            fixtureDTO.Production_Line_UID = production_Line_UID;
                            fixtureDTO.Status = fixture_Status_UID;
                            fixtureDTO.ShortCode = ShortCode;
                            fixtureDTO.TwoD_Barcode = TwoD_Barcode;
                            fixtureDTO.Created_UID = CurrentUser.AccountUId;
                            fixtureDTO.Modified_UID = CurrentUser.AccountUId;

                            fixtureDTOList.Add(fixtureDTO);
                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(fixtureDTOList);
                        var apiInsertDefectCode_GroupUrl = string.Format("Fixture/Insertfixture_MAPI");
                        HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertDefectCode_GroupUrl);
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

        }
        ///导入功能
        public string ImportFixtureUpdate(HttpPostedFileBase uploadName)
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "治具编码",
                        "版本号",
                        "治具流水序号",
                        "治具名称",
                        "专案代码",
                        "生产车间代码",
                        "生产线代码",
                        "机台代码",
                        "供应商代码",
                        "治具状态",
                        "治具短码",
                        "治具二维码",
                        "治具资料主键"
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

                        //获取OPTYPE
                        int BGorganization_UID = 0;
                        if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                        {
                            BGorganization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                        }
                        var apiUrlGetOPType = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", plant_OrganizationUID, BGorganization_UID);
                        HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrlGetOPType);
                        var OPTypeResult = responMessage.Content.ReadAsStringAsync().Result;
                        List<SystemProjectDTO> systemOPtypes = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(OPTypeResult);


                        //根据厂区获取所供应商信息。
                        var vendor_InfoByPlantAPI = string.Format("Fixture/GetVendor_InfoByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage vendor_InfoMessage = APIHelper.APIGetAsync(vendor_InfoByPlantAPI);
                        var jsonsvendor_Infos = vendor_InfoMessage.Content.ReadAsStringAsync().Result;
                        var vendor_Infos = JsonConvert.DeserializeObject<List<Vendor_InfoDTO>>(jsonsvendor_Infos);

                        //根据厂区获取所有机台数据。
                        var fixtureMachineByPlantAPI = string.Format("Fixture/GetFixtureMachineByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixtureMachineMessage = APIHelper.APIGetAsync(fixtureMachineByPlantAPI);
                        var jsonsfixtureMachines = fixtureMachineMessage.Content.ReadAsStringAsync().Result;
                        var fixtureMachines = JsonConvert.DeserializeObject<List<FixtureMachineDTO>>(jsonsfixtureMachines);

                        //获取所有专案
                        var queryAllProjectsAPI = string.Format("Equipmentmaintenance/QueryAllProjectsAPI?optype={0}", 0);
                        HttpResponseMessage allProjectMessage = APIHelper.APIGetAsync(queryAllProjectsAPI);
                        var jsonsallProjects = allProjectMessage.Content.ReadAsStringAsync().Result;
                        var allProjects = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(jsonsallProjects);

                        //根据厂区获取所有生产车间。
                        var workShopByPlantAPI = string.Format("Fixture/GetWorkShopByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage workShopMessage = APIHelper.APIGetAsync(workShopByPlantAPI);
                        var jsonsworkShops = workShopMessage.Content.ReadAsStringAsync().Result;
                        var workShops = JsonConvert.DeserializeObject<List<WorkshopDTO>>(jsonsworkShops);

                        //根据厂区获取所有生产线。
                        var production_LineByPlantAPI = string.Format("Fixture/GetProduction_LineByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage production_LineMessage = APIHelper.APIGetAsync(production_LineByPlantAPI);
                        var jsonsproduction_Lines = production_LineMessage.Content.ReadAsStringAsync().Result;
                        var production_Lines = JsonConvert.DeserializeObject<List<Production_LineDTO>>(jsonsproduction_Lines);

                        //根据厂区获取所有治具数据
                        var fixture_MByPlantAPI = string.Format("Fixture/GetFixture_MByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixture_MMessage = APIHelper.APIGetAsync(fixture_MByPlantAPI);
                        var jsonsfixture_Ms = fixture_MMessage.Content.ReadAsStringAsync().Result;
                        var fixture_Ms = JsonConvert.DeserializeObject<List<FixtureDTO>>(jsonsfixture_Ms);
                        //获取治具状态
                        var fixture_StatusAPI = string.Format("Fixture/GetFixtureStatuDTOAPI");
                        HttpResponseMessage fixture_StatusMessage = APIHelper.APIGetAsync(fixture_StatusAPI);
                        var jsonsfixture_Status = fixture_StatusMessage.Content.ReadAsStringAsync().Result;
                        var fixture_Status = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(jsonsfixture_Status);

                        var fixtureDTOList = new List<FixtureDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var fixtureDTO = new FixtureDTO();
                            int Plant_Organization_UID = 0;
                            int BG_Organization_UID = 0;
                            int? FunPlant_Organization_UID = null;
                            var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                            var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                            var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
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
                                var bg = systemOPtypes.Where(m => m.OP_TYPES == BG_Organization).FirstOrDefault();
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
                            string Fixture_NO = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具编码")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_NO))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编码没有值", i);
                                return errorInfo;
                            }
                            else if (Fixture_NO.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编码长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                            Fixture_NO = Fixture_NO.Trim();
                            string Version = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "版本号")].Value);
                            if (string.IsNullOrWhiteSpace(Version))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行版本号没有值", i);
                                return errorInfo;
                            }
                            else if (Version.Length > 2)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行版本号长度超过最大限定[2]", i);
                                return errorInfo;
                            }
                            Version = Version.Trim();
                            string Fixture_Seq = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具流水序号")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_Seq))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具流水序号没有值", i);
                                return errorInfo;
                            }
                            else if (Fixture_Seq.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具流水序号长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            Fixture_Seq = Fixture_Seq.Trim();
                            string Fixture_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具名称")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_Name))
                            {
                                Fixture_Name = null;
                            }
                            else
                            if (Fixture_Name.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具名称长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            if (Fixture_Name != null)
                            {
                                Fixture_Name = Fixture_Name.Trim();
                            }
                            string Project_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案代码")].Value);
                            if (string.IsNullOrWhiteSpace(Project_ID))
                            {
                                Project_ID = null;
                            }
                            else if (Project_ID.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行专案代码长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            if (Project_ID != null)
                            {
                                Project_ID = Project_ID.Trim();
                            }


                            string Workshop_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产车间代码")].Value);
                            if (string.IsNullOrWhiteSpace(Workshop_ID))
                            {
                                Workshop_ID = null;
                            }
                            else if (Workshop_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产车间代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (Workshop_ID != null)
                            {
                                Workshop_ID = Workshop_ID.Trim();
                            }


                            string Production_Line_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产线代码")].Value);
                            if (string.IsNullOrWhiteSpace(Production_Line_ID))
                            {
                                Production_Line_ID = null;
                            }
                            else if (Production_Line_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产线代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (Production_Line_ID != null)
                            {
                                Production_Line_ID = Production_Line_ID.Trim();
                            }

                            string Fixture_Machine_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台代码")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_Machine_ID))
                            {
                                Fixture_Machine_ID = null;
                            }
                            else if (Fixture_Machine_ID.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台代码长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                            if (Fixture_Machine_ID != null)
                            {
                                Fixture_Machine_ID = Fixture_Machine_ID.Trim();
                            }

                            string Vendor_Info_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "供应商代码")].Value);
                            if (string.IsNullOrWhiteSpace(Vendor_Info_ID))
                            {
                                Vendor_Info_ID = null;
                            }
                            else if (Vendor_Info_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行供应商代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (Vendor_Info_ID != null)
                            {
                                Vendor_Info_ID = Vendor_Info_ID.Trim();
                            }


                            string Status = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具状态")].Value);
                            if (string.IsNullOrWhiteSpace(Status))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具状态没有值", i);
                                return errorInfo;
                            }
                            else if (Status.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具状态长度超过最大限定[50]", i);
                                return errorInfo;
                            }
                            if (Status != null)
                            {
                                Status = Status.Trim();
                            }

                            string ShortCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具短码")].Value);
                            if (string.IsNullOrWhiteSpace(ShortCode))
                            {
                                ShortCode = null;

                            }
                            else if (ShortCode.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具短码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            if (ShortCode != null)
                            {
                                ShortCode = ShortCode.Trim();
                            }

                            string TwoD_Barcode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具二维码")].Value);

                            int? Project_UID = 0;
                            if (Project_ID != null && Project_ID != "")
                            {


                                var hasitem = allProjects.FirstOrDefault(m => m.Organization_UID == BG_Organization_UID && m.Project_Code == Project_ID);
                                if (hasitem == null)
                                {
                                    Project_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此专案代码[{1}]", i, Project_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    Project_UID = hasitem.Project_UID;
                                }
                                if (TwoD_Barcode != null)
                                {
                                    TwoD_Barcode = TwoD_Barcode.Trim();
                                }
                            }
                            int? Fixture_Machine_UID = 0;
                            if (Fixture_Machine_ID != null && Fixture_Machine_ID != "")
                            {
                                var hasMachineitem = fixtureMachines.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Machine_ID == Fixture_Machine_ID);
                                if (hasMachineitem == null)
                                {
                                    Fixture_Machine_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此机台代码[{1}]", i, Fixture_Machine_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    Fixture_Machine_UID = hasMachineitem.Fixture_Machine_UID;
                                }
                            }
                            int? Vendor_Info_UID = 0;
                            if (Vendor_Info_ID != null && Vendor_Info_ID != "")
                            {
                                var hasVendor_Infoitem = vendor_Infos.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Vendor_ID == Vendor_Info_ID);
                                if (hasVendor_Infoitem == null)
                                {
                                    Vendor_Info_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此供应商代码[{1}]", i, Vendor_Info_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    Vendor_Info_UID = hasVendor_Infoitem.Vendor_Info_UID;
                                }
                            }

                            int? production_Line_UID = 0;
                            if (Production_Line_ID != null && Production_Line_ID != "")
                            {
                                var hasProduction_Lineitem = production_Lines.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Line_ID == Production_Line_ID);
                                if (hasProduction_Lineitem == null)
                                {
                                    production_Line_UID = null;
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行没有此产线代码[{1}]", i, Production_Line_ID);
                                    return errorInfo;
                                }
                                else
                                {
                                    production_Line_UID = hasProduction_Lineitem.Production_Line_UID;
                                }
                            }

                            int fixture_Status_UID = 0;
                            var fixture_Statusitem = fixture_Status.FirstOrDefault(m => m.StatuName == Status);
                            if (fixture_Statusitem == null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有此治具状态[{1}]", i, Status);
                                return errorInfo;
                            }
                            else
                            {
                                fixture_Status_UID = fixture_Statusitem.Status;
                            }

                            string Fixture_Unique_ID = Fixture_NO + Fixture_Seq;

                            //var fixture_M = fixture_Ms.FirstOrDefault(m => m.Fixture_Unique_ID == Fixture_Unique_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            //if (fixture_M != null)
                            //{
                            //    isExcelError = true;
                            //    errorInfo = string.Format("第{0}行治具编号[{1}]，治具流水号[{2}]已经存在,不可重复导入", i, Fixture_NO, Fixture_Seq);
                            //    return errorInfo;
                            //}
                            //导入数据判重
                            var isSelfRepeated = fixtureDTOList.Exists(m => m.Fixture_Unique_ID == Fixture_Unique_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编号[{1}]，治具流水号[{2}]在导入数据中重复,不可重复导入", i, Fixture_NO, Fixture_Seq);
                                return errorInfo;
                            }
                            int Fixture_M_UID = 0;
                            string stringFixture_M_UID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具资料主键")].Value);
                            Fixture_M_UID = int.Parse(stringFixture_M_UID.Trim());

                            if (fixture_Ms.Where(o => o.Fixture_M_UID == Fixture_M_UID && o.Fixture_NO == Fixture_NO).Count() > 0)
                            {
                                fixtureDTO.Fixture_M_UID = Fixture_M_UID;
                                fixtureDTO.Plant_Organization_UID = Plant_Organization_UID;
                                fixtureDTO.BG_Organization_UID = BG_Organization_UID;
                                fixtureDTO.FunPlant_Organization_UID = FunPlant_Organization_UID;
                                fixtureDTO.Fixture_NO = Fixture_NO;
                                fixtureDTO.Version = Version;
                                fixtureDTO.Fixture_Seq = Fixture_Seq;
                                fixtureDTO.Fixture_Unique_ID = Fixture_Unique_ID;
                                fixtureDTO.Fixture_Name = Fixture_Name;
                                fixtureDTO.Project_UID = Project_UID;
                                fixtureDTO.Fixture_Machine_UID = Fixture_Machine_UID;
                                fixtureDTO.Vendor_Info_UID = Vendor_Info_UID;
                                fixtureDTO.Production_Line_UID = production_Line_UID;
                                fixtureDTO.Status = fixture_Status_UID;
                                fixtureDTO.ShortCode = ShortCode;
                                fixtureDTO.TwoD_Barcode = TwoD_Barcode;
                                fixtureDTO.Created_UID = CurrentUser.AccountUId;
                                fixtureDTO.Modified_UID = CurrentUser.AccountUId;
                                fixtureDTOList.Add(fixtureDTO);
                            }
                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(fixtureDTOList);
                        var apiInsertDefectCode_GroupUrl = string.Format("Fixture/Updatefixture_MAPI");
                        HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertDefectCode_GroupUrl);
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

        }
        public string DeleteFixtureM(int Fixture_M_UID)
        {
            var apiUrl = string.Format("Fixture/DeleteFixtureMAPI?Fixture_M_UID={0}", Fixture_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FileResult Fixture_M()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Fixture_M.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        #endregion 治具资料维护--------------end
        #region               karl start
        #region  供应商维护------START
        public ActionResult VendorInfo()
        {

            VendorInfoVM currentVM = new VendorInfoVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
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
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            return View("VendorInfo", currentVM);
        }
        public ActionResult QueryVendorInfo(VendorInfoDTO search, Page page)
        {
            int organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("Fixture/QueryVendorInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetOrgByParant(int Parant_UID, int type)
        {
            var apiUrl = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", Parant_UID, type);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditVendorInfo(VendorInfoDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Fixture/AddOrEditVendorInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryVendorInfoByUid(int Vendor_Info_UID)
        {
            var apiUrl = string.Format("Fixture/QueryVendorInfoByUidAPI?Vendor_Info_UID={0}", Vendor_Info_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string DeleteVendorInfo(int Vendor_Info_UID)
        {
            var apiUrl = string.Format("Fixture/DeleteVendorInfoAPI?Vendor_Info_UID={0}", Vendor_Info_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        private string[] GetVendorInfoHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区代号",
                "OP代号",
                "供货商代码",
                "供货商名称",
                "是否启用"
            };
            return propertiesHead;
        }
        private void SetVendorInfoExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];
            worksheet.Cells[1, 3].Value = propertiesHead[2];
            worksheet.Cells[1, 4].Value = propertiesHead[3];
            worksheet.Cells[1, 5].Value = propertiesHead[4];
            worksheet.Cells[1, 6].Value = propertiesHead[5];
            //设置列宽
            worksheet.Column(1).Width = 17;
            worksheet.Column(2).Width = 17;
            worksheet.Column(3).Width = 17;
            worksheet.Column(4).Width = 17;
            worksheet.Column(5).Width = 17;
            worksheet.Column(6).Width = 17;

            worksheet.Cells["A1:F1"].Style.Font.Bold = true;
            worksheet.Cells["A1:F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:F1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        public string ImportVendorInfoExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddVendorInfoExcel(upload_excel);
            return errorInfo;
        }
        private string AddVendorInfoExcel(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetVendorInfoHeadColumn();
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


                    //获取所有的供应商资料
                    var vendorInfoAPI = "Fixture/QueryAllVendorInfoAPI";
                    HttpResponseMessage vendorInfomessage = APIHelper.APIGetAsync(vendorInfoAPI);
                    var jsonsVendorInfo = vendorInfomessage.Content.ReadAsStringAsync().Result;
                    var vendorinfos = JsonConvert.DeserializeObject<List<VendorInfoDTO>>(jsonsVendorInfo);

                    List<VendorInfoDTO> vendorinfolist = new List<VendorInfoDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        VendorInfoDTO VI = new VendorInfoDTO();
                        //int intuserid = 0;
                        int Plant_Organization_UID = 0;
                        int? BG_Organization_UID = null;
                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区代号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var systemOrgDTO = orgboms.Where(m => m.Plant == Plant_Organization).FirstOrDefault();

                            if (systemOrgDTO != null)
                            {
                                Plant_Organization_UID = systemOrgDTO.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                        }
                        else
                        {
                            var hasbg = orgboms.Where(m => m.Plant == Plant_Organization & m.BG == BG_Organization).FirstOrDefault();
                            if (hasbg != null)
                            {
                                BG_Organization_UID = hasbg.BG_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                                return errorInfo;
                            }

                        }

                        string Vendor_Id = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "供货商代码")].Value);
                        if (string.IsNullOrWhiteSpace(Vendor_Id))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行供应商代码没有值", i);
                            return errorInfo;
                        }
                        else if (Vendor_Id.Length > 10)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行供应商代码长度超过最大限定[10]", i);
                            return errorInfo;
                        }

                        string Vendor_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "供货商名称")].Value);
                        if (string.IsNullOrWhiteSpace(Vendor_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行供应商名称没有值", i);
                            return errorInfo;
                        }
                        else if (Vendor_Name.Length > 50)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行供应商名称长度超过最大限定[50]", i);
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

                        var hasvendorinfo = vendorinfos.FirstOrDefault(m => m.Vendor_ID == Vendor_Id & m.Plant_Organization_UID == Plant_Organization_UID);
                        if (hasvendorinfo != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("供应商[{0}]已经存在,不可重复导入", Vendor_Id);
                            return errorInfo;
                        }
                        var hasitem = vendorinfolist.FirstOrDefault(m => m.Vendor_ID == Vendor_Id);
                        if (hasitem != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("供应商代号[{0}]重复", Vendor_Id);
                            return errorInfo;
                        }
                        VI.Plant_Organization_UID = Plant_Organization_UID;
                        VI.BG_Organization_UID = BG_Organization_UID;
                        VI.Vendor_ID = Vendor_Id;
                        VI.Vendor_Name = Vendor_Name;
                        VI.Is_Enable = Is_Enable == "0" ? false : true;
                        VI.Created_UID = CurrentUser.AccountUId;

                        vendorinfolist.Add(VI);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(vendorinfolist);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertVendorInfoAPI");
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

        public ActionResult DoVIExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoVIExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<VendorInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("VendorInfo");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "供应商代码", "供应商名称", "是否启用", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("VendorInfo");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Vendor_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Vendor_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoVIExportFunctionByQuery(VendorInfoDTO search)
        {
            int organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && organization_UID != 0)
            {
                search.BG_Organization_UID = organization_UID;
            }
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            //get Export datas
            var apiUrl = "Fixture/GetVendorInfoListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<VendorInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("VendorInfo");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "供应商代码", "供应商名称", "是否启用", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("VendorInfo");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Vendor_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Vendor_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion  供应商维护----END

        #region   保养计划设定维护--------------START
        public ActionResult MaintenancePlan()
        {
            VendorInfoVM currentVM = new VendorInfoVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
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
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }
            return View("MaintenancePlan", currentVM);
        }
        public ActionResult QueryMaintenancePlan(MaintenancePlanDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            var apiUrl = string.Format("Fixture/QueryMaintenancePlanAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryMaintenancePlanByUid(int Maintenance_Plan_UID)
        {
            var apiUrl = string.Format("Fixture/QueryMaintenancePlanByUidAPI?Maintenance_Plan_UID={0}", Maintenance_Plan_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditMaintenancePlan(MaintenancePlanDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Fixture/AddOrEditMaintenancePlanAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string DeleteMaintenancePlan(int Maintenance_Plan_UID)
        {
            var apiUrl = string.Format("Fixture/DeleteMaintenancePlanAPI?Maintenance_Plan_UID={0}", Maintenance_Plan_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public FileResult DownloadMaintenancePlanExcel()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Maintenance_Plan.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string ImportMaintenancePlanExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddMaintenancePlanExcel(upload_excel);
            return errorInfo;
        }
        private string AddMaintenancePlanExcel(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetMaintenancePlanHeadColumn();
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

                    //获取所有的保养设定
                    var vendorInfoAPI = "Fixture/QueryAllMaintenancePlanAPI";
                    HttpResponseMessage vendorInfomessage = APIHelper.APIGetAsync(vendorInfoAPI);
                    var jsonsVendorInfo = vendorInfomessage.Content.ReadAsStringAsync().Result;
                    var maintenanceplans = JsonConvert.DeserializeObject<List<MaintenancePlanDTO>>(jsonsVendorInfo);

                    List<MaintenancePlanDTO> maintenanceplanlist = new List<MaintenancePlanDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        MaintenancePlanDTO MP = new MaintenancePlanDTO();
                        // int intuserid = 0;
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区代号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasplant = orgboms.Where(m => m.Plant == Plant_Organization).FirstOrDefault();

                            if (hasplant != null)
                            {
                                Plant_Organization_UID = hasplant.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op代号没有值", i);
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
                                errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            FunPlant_Organization_UID = null;
                        }
                        else
                        {

                            var hasfunplant = orgboms.Where(m => m.Plant == Plant_Organization
                            & m.BG == BG_Organization & m.Funplant == FunPlant_Organization).FirstOrDefault();
                            if (hasfunplant != null)
                            {
                                FunPlant_Organization_UID = hasfunplant.Funplant_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        string Maintenance_Type = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "保养类别")].Value);
                        if (string.IsNullOrWhiteSpace(Maintenance_Type))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行保养类别没有值", i);
                            return errorInfo;
                        }
                        else if (Maintenance_Type != "D" && Maintenance_Type != "P")
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行保养类别只能是D或P", i);
                            return errorInfo;
                        }

                        string Cycle_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "周期类别代码")].Value);
                        if (string.IsNullOrWhiteSpace(Cycle_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行周期类别代码没有值", i);
                            return errorInfo;
                        }
                        else if (Cycle_ID.Length > 10)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行周期类别代码长度超过最大限定[10]", i);
                            return errorInfo;
                        }
                        int Cycle_Interval;
                        string strCycle_Interval = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "周期间隔")].Value);
                        if (string.IsNullOrWhiteSpace(strCycle_Interval))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行周期间隔没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            try
                            {
                                Cycle_Interval = Convert.ToInt16(strCycle_Interval);
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行周期间隔只能为整数", i);
                                return errorInfo;
                            }
                        }

                        string Cycle_Unit = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "周期单位")].Value);
                        if (string.IsNullOrWhiteSpace(Cycle_Unit))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行周期单位没有值", i);
                            return errorInfo;
                        }
                        else if (Cycle_Unit != "W" && Cycle_Unit != "H" && Cycle_Unit != "M")
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行保养类别只能是W,H或M", i);
                            return errorInfo;
                        }
                        int Lead_Time;
                        string strLead_Time = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "前置时间(天)")].Value);
                        if (string.IsNullOrWhiteSpace(strLead_Time))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行前置时间没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            try
                            {
                                Lead_Time = Convert.ToInt16(strLead_Time);
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行前置时间只能为整数", i);
                                return errorInfo;
                            }
                        }
                        DateTime Start_Date;
                        string strStart_Date = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "开始日期")].Value);
                        if (string.IsNullOrWhiteSpace(strStart_Date))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行开始日期没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            try
                            {
                                Start_Date = Convert.ToDateTime(strStart_Date);
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行开始日期错误,请输入正确日期格式", i);
                                return errorInfo;
                            }
                        }
                        int Tolerance_Time;
                        string strTolerance_Time = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "寛容时间(天)")].Value);
                        if (string.IsNullOrWhiteSpace(strTolerance_Time))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行宽容时间没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            try
                            {
                                Tolerance_Time = Convert.ToInt16(strTolerance_Time);
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行宽容时间只能为整数", i);
                                return errorInfo;
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

                        var hasMaintenancePlan = maintenanceplans.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID & m.Cycle_ID == Cycle_ID);
                        if (hasMaintenancePlan != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("周期类别代码[{0}]已经存在,不可重复导入", Cycle_ID);
                            return errorInfo;
                        }
                        var hasitem = maintenanceplanlist.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID & m.Cycle_ID == Cycle_ID);
                        if (hasitem != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("周期类别代码[{0}]重复", Cycle_ID);
                            return errorInfo;
                        }
                        MP.Plant_Organization_UID = Plant_Organization_UID;
                        MP.BG_Organization_UID = BG_Organization_UID;
                        MP.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        MP.Maintenance_Type = Maintenance_Type;
                        MP.Cycle_ID = Cycle_ID;
                        MP.Cycle_Interval = Cycle_Interval;
                        MP.Cycle_Unit = Cycle_Unit;
                        MP.Lead_Time = Lead_Time;
                        MP.Start_Date = Start_Date;
                        MP.Tolerance_Time = Tolerance_Time;
                        MP.Is_Enable = Is_Enable == "0" ? false : true;
                        MP.Created_UID = CurrentUser.AccountUId;

                        maintenanceplanlist.Add(MP);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(maintenanceplanlist);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertMaintenancePlanAPI");
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
        private string[] GetMaintenancePlanHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区代号",
                "OP代号",
                "功能厂代号",
                "保养类别",
                "周期类别代码",
                "周期间隔",
                "周期单位",
                "前置时间(天)",
                "开始日期",
                "寛容时间(天)",
                "是否启用"
            };
            return propertiesHead;
        }
        public ActionResult DoAllMPExportFunction(MaintenancePlanDTO search)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }
            //get Export datas     
            var apiUrl = "Fixture/DoAllMPExportFunctionAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaintenancePlanDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaintenancePlan");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序","厂别", "BG", "功能厂", "保养类别", "周期类别代码","周期间隔","周期单位",
                "前置时间","开始日期","宽容时间","最后执行时间","下次执行日期","是否启用","创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaintenancePlan");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Maintenance_Type;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Cycle_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Cycle_Interval;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Cycle_Unit;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Lead_Time;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Start_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Tolerance_Time;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Last_Execution_Date == null ? "" : ((DateTime)currentRecord.Last_Execution_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Next_Execution_Date == null ? "" : ((DateTime)currentRecord.Next_Execution_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoMPExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoMPExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaintenancePlanDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaintenancePlan");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序","厂别", "BG", "功能厂", "保养类别", "周期类别代码","周期间隔","周期单位",
                "前置时间","开始日期","宽容时间","最后执行时间","下次执行日期","是否启用","创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaintenancePlan");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Maintenance_Type;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Cycle_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Cycle_Interval;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Cycle_Unit;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Lead_Time;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Start_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Tolerance_Time;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Last_Execution_Date == null ? "" : ((DateTime)currentRecord.Last_Execution_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Next_Execution_Date == null ? "" : ((DateTime)currentRecord.Next_Execution_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public string GetMaintenancePlanInfo(string Cycle_Unit, DateTime Start_Date, int Cycle_Interval, int Lead_Time)
        {
            DateTime nextExecDate = DateTime.Now;
            switch (Cycle_Unit)
            {
                case "H":
                    nextExecDate = Start_Date.AddHours(Cycle_Interval);
                    break;
                case "W":
                    nextExecDate = Start_Date.AddDays(7 * Cycle_Interval);
                    break;
                case "M":
                    nextExecDate = Start_Date.AddMonths(Cycle_Interval);
                    break;
            }

            string Next_Execution_Date = nextExecDate.AddDays(0 - Lead_Time).ToString(FormatConstants.DateTimeFormatStringByDate);
            return Next_Execution_Date;
        }

        #endregion     保养计划设定维护-----------END
        #region   治具保养设定--------------START
        public ActionResult FixtureMaintenanceProfile()
        {
            VendorInfoVM currentVM = new VendorInfoVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
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
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }
            //获取所有的保养设定
            var vendorInfoAPI = "Fixture/QueryAllMaintenancePlanAPI";
            HttpResponseMessage vendorInfomessage = APIHelper.APIGetAsync(vendorInfoAPI);
            var jsonsVendorInfo = vendorInfomessage.Content.ReadAsStringAsync().Result;
            var maintenanceplans = JsonConvert.DeserializeObject<List<MaintenancePlanDTO>>(jsonsVendorInfo);
            currentVM.maintenanceplan = maintenanceplans;
            return View("FixtureMaintenanceProfile", currentVM);
        }
        public ActionResult QueryFixtureMaintenanceProfile(FixtureMaintenanceProfileDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            var apiUrl = string.Format("Fixture/QueryFixtureMaintenanceProfileAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureMaintenanceProfileByUid(int Fixture_Maintenance_Profile_UID)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureMaintenanceProfileByUidAPI?Fixture_Maintenance_Profile_UID={0}", Fixture_Maintenance_Profile_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditFixtureMaintenanceProfile(FixtureMaintenanceProfileDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Fixture/AddOrEditFixtureMaintenanceProfileAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string DeleteFixtureMaintenanceProfile(int Fixture_Maintenance_Profile_UID)
        {
            var apiUrl = string.Format("Fixture/DeleteFixtureMaintenanceProfileAPI?Fixture_Maintenance_Profile_UID={0}", Fixture_Maintenance_Profile_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public string ImportFixtureMaintenanceProfileExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddFixtureMaintenanceProfileExcel(upload_excel);
            return errorInfo;
        }
        private string AddFixtureMaintenanceProfileExcel(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetFixtureMaintenanceProfileHeadColumn();
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

                    //获取所有的保养设定
                    var maintenanceplanAPI = "Fixture/QueryAllMaintenancePlanAPI";
                    HttpResponseMessage maintenanceplanmessage = APIHelper.APIGetAsync(maintenanceplanAPI);
                    var jsonsmaintenanceplan = maintenanceplanmessage.Content.ReadAsStringAsync().Result;
                    var maintenanceplans = JsonConvert.DeserializeObject<List<MaintenancePlanDTO>>(jsonsmaintenanceplan);

                    //获取所有的治具保养
                    var fixturemaintenanceprofileAPI = "Fixture/QueryAllFixtureMaintenanceProfileAPI";
                    HttpResponseMessage fixturemaintenanceprofilemessage = APIHelper.APIGetAsync(fixturemaintenanceprofileAPI);
                    var jsonsFixturemaintenanceprofile = fixturemaintenanceprofilemessage.Content.ReadAsStringAsync().Result;
                    var fixturemaintenanceprofile = JsonConvert.DeserializeObject<List<FixtureMaintenanceProfileDTO>>(jsonsFixturemaintenanceprofile);

                    //获取所有的治具
                    var fixturemAPI = string.Format("Fixture/QueryAllFixtureAPI");
                    HttpResponseMessage fixturemmessage = APIHelper.APIGetAsync(fixturemAPI);
                    var jsonsFixturem = fixturemmessage.Content.ReadAsStringAsync().Result;
                    var fixturems = JsonConvert.DeserializeObject<List<FixtureDTO>>(jsonsFixturem);

                    List<FixtureMaintenanceProfileDTO> fixturemaintenanceprofilelist = new List<FixtureMaintenanceProfileDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        FixtureMaintenanceProfileDTO FMP = new FixtureMaintenanceProfileDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区代号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasplant = orgboms.Where(m => m.Plant == Plant_Organization).FirstOrDefault();

                            if (hasplant != null)
                            {
                                Plant_Organization_UID = hasplant.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op代号没有值", i);
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
                                errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            FunPlant_Organization_UID = null;
                        }
                        else
                        {

                            var hasfunplant = orgboms.Where(m => m.Plant == Plant_Organization
                            & m.BG == BG_Organization & m.Funplant == FunPlant_Organization).FirstOrDefault();
                            if (hasfunplant != null)
                            {
                                FunPlant_Organization_UID = hasfunplant.Funplant_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        string Fixture_NO = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具编号(图号)")].Value);
                        if (string.IsNullOrWhiteSpace(Fixture_NO))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}治具编号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasfixtureList = fixturems.Where(m => m.BG_Organization_UID == BG_Organization_UID & m.Fixture_NO == Fixture_NO).ToList();
                            if (FunPlant_Organization_UID != null)
                            {
                                hasfixtureList = fixturems.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID).ToList();

                            }
                            //  FixtureDTO hasfixture = null;
                            if (hasfixtureList == null || hasfixtureList.Count <= 0)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行对应的治具编号不存在 ", i);
                                return errorInfo;
                            }
                            //else
                            //{
                            //    hasfixture = hasfixtureList.FirstOrDefault();
                            //}
                        }
                        string Maintenance_Type = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "保养类别")].Value);
                        if (string.IsNullOrWhiteSpace(Maintenance_Type))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}保养类别没有值", i);
                            return errorInfo;
                        }
                        else if (Maintenance_Type != "D" && Maintenance_Type != "P")
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行保养类别只能是D或P", i);
                            return errorInfo;
                        }

                        string Cycle_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "周期类别代码")].Value);
                        if (string.IsNullOrWhiteSpace(Cycle_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行周期类别代码没有值", i);
                            return errorInfo;
                        }
                        else if (Cycle_ID.Length > 10)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行周期类别代码长度超过最大限定[10]", i);
                            return errorInfo;
                        }
                        int Maintenance_Plan_UID;
                        var hasmaintenanceplan = maintenanceplans.Where(m => m.Maintenance_Type == Maintenance_Type & m.Cycle_ID == Cycle_ID).FirstOrDefault();
                        if (hasmaintenanceplan == null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行对应的保养类别+周期类别代码组合不存在", i);
                            return errorInfo;
                        }
                        else
                        {
                            Maintenance_Plan_UID = hasmaintenanceplan.Maintenance_Plan_UID;
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

                        //同一个周期类别代码，只能导入一次
                        FixtureMaintenanceProfileDTO hasfixturemaintenanceprofile = null;
                        if (FunPlant_Organization_UID != null && FunPlant_Organization_UID != 0)
                        {
                            hasfixturemaintenanceprofile = fixturemaintenanceprofile.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID & m.BG_Organization_UID == BG_Organization_UID & m.FunPlant_Organization_UID == FunPlant_Organization_UID & m.Fixture_NO == Fixture_NO & m.Maintenance_Plan_UID == Maintenance_Plan_UID);
                        }
                        else
                        {
                            hasfixturemaintenanceprofile = fixturemaintenanceprofile.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID & m.BG_Organization_UID == BG_Organization_UID & m.Fixture_NO == Fixture_NO & m.Maintenance_Plan_UID == Maintenance_Plan_UID);
                        }
                        if (hasfixturemaintenanceprofile != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("治具编号[{0}]已经存在,不可重复导入", Fixture_NO);
                            return errorInfo;
                        }
                        var hasitem = fixturemaintenanceprofilelist.FirstOrDefault(m => m.Plant_Organization_UID == Plant_Organization_UID & m.Fixture_NO == Fixture_NO);
                        if (hasitem != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("治具编号[{0}]重复", Fixture_NO);
                            return errorInfo;
                        }
                        FMP.Plant_Organization_UID = Plant_Organization_UID;
                        FMP.BG_Organization_UID = BG_Organization_UID;
                        FMP.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        FMP.Fixture_NO = Fixture_NO;
                        FMP.Maintenance_Plan_UID = Maintenance_Plan_UID;
                        FMP.Is_Enable = Is_Enable == "0" ? false : true;
                        FMP.Created_UID = CurrentUser.AccountUId;

                        fixturemaintenanceprofilelist.Add(FMP);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(fixturemaintenanceprofilelist);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertFixtureMaintenanceProfileAPI");
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
        private string[] GetFixtureMaintenanceProfileHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区代号",
                "OP代号",
                "功能厂代号",
                "治具编号(图号)",
                "保养类别",
                "周期类别代码",
                "是否启用"
            };
            return propertiesHead;
        }
        public ActionResult GetMaintenancePlanByFilters(int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type)
        {
            var maintenanceplanAPI = string.Format("Fixture/GetMaintenancePlanByFiltersAPI?BG_Organization_UID={0}&FunPlant_Organization_UID={1}&Maintenance_Type={2}",
                BG_Organization_UID, FunPlant_Organization_UID, Maintenance_Type);
            HttpResponseMessage maintenanceplanmessage = APIHelper.APIGetAsync(maintenanceplanAPI);
            var result = maintenanceplanmessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DoAllFMPExportFunction(FixtureMaintenanceProfileDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }
            //get Export datas
            var apiUrl = "Fixture/DoAllFMPExportFunctionAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureMaintenanceProfileDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureMaintenanceProfile");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "功能厂", "治具编号", "保养类别", "周期类别代码", "是否启用", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureMaintenanceProfile");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Maintenance_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Cycle_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoFMPExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoFMPExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureMaintenanceProfileDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureMaintenanceProfile");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "功能厂", "治具编号", "保养类别", "周期类别代码", "是否启用", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureMaintenanceProfile");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Maintenance_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Cycle_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult GetFixtureNoByFunPlant(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var maintenanceplanAPI = string.Format("Fixture/GetFixtureNoByFunPlantAPI?BG_Organization_UID={0}&FunPlant_Organization_UID={1}"
                , BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage maintenanceplanmessage = APIHelper.APIGetAsync(maintenanceplanAPI);
            var result = maintenanceplanmessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion     治具保养设定-----------END
        #region   用户车间设定--------------START
        public ActionResult FixtureUserWorkshop()
        {
            VendorInfoVM currentVM = new VendorInfoVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
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
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }
            }

            return View("FixtureUserWorkshop", currentVM);
        }
        public ActionResult GetUserByOp(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetUserByOpAPI?BG_Organization_UID={0}&FunPlant_Organization_UID={1}",
                BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetUserByOpLY(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetUserByOpAPILY?BG_Organization_UID={0}&FunPlant_Organization_UID={1}",
                BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureUserWorkshop(FixtureUserWorkshopDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            var apiUrl = string.Format("Fixture/QueryFixtureUserWorkshopAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureUserWorkshopByUid(int Fixture_User_Workshop_UID)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureUserWorkshopByUidAPI?Fixture_User_Workshop_UID={0}", Fixture_User_Workshop_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditFixtureUserWorkshop(FixtureUserWorkshopDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Fixture/AddOrEditFixtureUserWorkshopAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string DeleteFixtureUserWorkshop(int Fixture_User_Workshop_UID)
        {
            var apiUrl = string.Format("Fixture/DeleteFixtureUserWorkshopAPI?Fixture_User_Workshop_UID={0}", Fixture_User_Workshop_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public string ImportFixtureUserWorkshopExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddFixtureUserWorkshopExcel(upload_excel);
            return errorInfo;
        }
        private string AddFixtureUserWorkshopExcel(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetFixtureUserWorkshopHeadColumn();
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

                    //获取所有的用户车间设定
                    var userworkshopAPI = "Fixture/QueryAllFixtureUserWorkshopAPI";
                    HttpResponseMessage userworkshopmessage = APIHelper.APIGetAsync(userworkshopAPI);
                    var jsonsUserworkshop = userworkshopmessage.Content.ReadAsStringAsync().Result;
                    var userworkshops = JsonConvert.DeserializeObject<List<FixtureUserWorkshopDTO>>(jsonsUserworkshop);

                    //获取所有的用户
                    var usersAPI = "Fixture/QueryAllUsersAPI";
                    HttpResponseMessage usersmessage = APIHelper.APIGetAsync(usersAPI);
                    var jsonsUser = usersmessage.Content.ReadAsStringAsync().Result;
                    var users = JsonConvert.DeserializeObject<List<SystemUserDTO>>(jsonsUser);

                    //获取所有的车间
                    var workshopAPI = "Fixture/QueryAllWorkshopsAPI";
                    HttpResponseMessage workshopmessage = APIHelper.APIGetAsync(workshopAPI);
                    var jsonsWorkshop = workshopmessage.Content.ReadAsStringAsync().Result;
                    var workshops = JsonConvert.DeserializeObject<List<WorkshopDTO>>(jsonsWorkshop);

                    List<FixtureUserWorkshopDTO> fixtureuserworkshoplist = new List<FixtureUserWorkshopDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        FixtureUserWorkshopDTO FUW = new FixtureUserWorkshopDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区代号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasplant = orgboms.Where(m => m.Plant == Plant_Organization).FirstOrDefault();

                            if (hasplant != null)
                            {
                                Plant_Organization_UID = hasplant.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op代号没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.Plant == Plant_Organization & m.BG == BG_Organization).FirstOrDefault();
                            if (hasbg != null)
                            {
                                BG_Organization_UID = hasbg.BG_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                        }
                        else
                        {

                            var hasfunplant = orgboms.Where(m => m.Plant == Plant_Organization
                            & m.BG == BG_Organization & m.Funplant == FunPlant_Organization).FirstOrDefault();
                            if (hasfunplant != null)
                            {
                                FunPlant_Organization_UID = hasfunplant.Funplant_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        int Account_UID;
                        string User_NTID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "使用者NTID")].Value);
                        if (string.IsNullOrWhiteSpace(User_NTID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}使用者NTID没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasuser = users.Where(m => m.User_NTID.ToUpper() == User_NTID.ToUpper()).FirstOrDefault();
                            if (hasuser == null)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}使用者NTID值不存在", i);
                                return errorInfo;
                            }
                            else
                            {
                                Account_UID = hasuser.Account_UID;
                            }
                        }
                        int Workshop_UID;
                        string Workshop_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产车间代码")].Value);
                        if (string.IsNullOrWhiteSpace(Workshop_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行生产车间代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasworkshop = workshops.Where(m => m.Workshop_ID == Workshop_ID).FirstOrDefault();
                            if (hasworkshop == null)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行生产车间代码不存在", i);
                                return errorInfo;
                            }
                            else
                            {
                                Workshop_UID = hasworkshop.Workshop_UID;
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


                        var hasFixtureUserWorkshop = userworkshops.FirstOrDefault(m => m.Account_UID == Account_UID & m.Workshop_UID == Workshop_UID);
                        if (hasFixtureUserWorkshop != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行用户车间资料已经存在,不可重复导入", i);
                            return errorInfo;
                        }
                        var hasitem = fixtureuserworkshoplist.FirstOrDefault(m => m.Account_UID == Account_UID & m.Workshop_UID == Workshop_UID);
                        if (hasitem != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行用户车间资料重复", i);
                            return errorInfo;
                        }
                        FUW.Account_UID = Account_UID;
                        FUW.Workshop_UID = Workshop_UID;
                        FUW.Created_UID = CurrentUser.AccountUId;
                        FUW.Is_Enable = Is_Enable == "1" ? true : false;

                        fixtureuserworkshoplist.Add(FUW);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(fixtureuserworkshoplist);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertFixtureUserWorkshopAPI");
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
        private string[] GetFixtureUserWorkshopHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区代号",
                "OP代号",
                "功能厂代号",
                "使用者NTID",
                "生产车间代码",
                "是否启用"
            };
            return propertiesHead;
        }
        public ActionResult GetWorkshopByNTID(int Account_UID)
        {
            var apiUrl = string.Format("Fixture/GetWorkshopByNTIDAPI?Account_UID={0}", Account_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DoAllFUWExportFunction(FixtureUserWorkshopDTO search)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }
            //get Export datas
            var apiUrl = "Fixture/DoAllFUWExportFunctionAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureUserWorkshopDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureUserWorkshop");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "功能厂", "使用者NTID", "使用者姓名", "生产车间代码", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureUserWorkshop");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.User_NTID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Workshop_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoFUWExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoFUWExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureUserWorkshopDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureUserWorkshop");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "功能厂", "使用者NTID", "使用者姓名", "生产车间代码", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureUserWorkshop");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.User_NTID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Workshop_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion     用户车间设定-----------END
        #region           维修地点设定------------------START
        public ActionResult RepairLocation()
        {
            VendorInfoVM currentVM = new VendorInfoVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
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
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            return View("RepairLocation", currentVM);
        }
        public ActionResult QueryRepairLocation(RepairLocationDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            var apiUrl = string.Format("Fixture/QueryRepairLocationAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryRepairLocationByUid(int Repair_Location_UID)
        {
            var apiUrl = string.Format("Fixture/QueryRepairLocationByUidAPI?Repair_Location_UID={0}", Repair_Location_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditRepairLocation(RepairLocationDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Fixture/AddOrEditRepairLocationAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string DeleteRepairLocation(int Repair_Location_UID)
        {
            var apiUrl = string.Format("Fixture/DeleteRepairLocationAPI?Repair_Location_UID={0}", Repair_Location_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public string ImportRepairLocationExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddRepairLocationExcel(upload_excel);
            return errorInfo;
        }
        private string AddRepairLocationExcel(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetRepairLocationHeadColumn();
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

                    //获取所有的维修地点
                    var repairlocationAPI = "Fixture/QueryAllRepairLocationAPI";
                    HttpResponseMessage repairlocationmessage = APIHelper.APIGetAsync(repairlocationAPI);
                    var jsonsRepailocation = repairlocationmessage.Content.ReadAsStringAsync().Result;
                    var repairlocations = JsonConvert.DeserializeObject<List<RepairLocationDTO>>(jsonsRepailocation);

                    List<RepairLocationDTO> repairlocationlist = new List<RepairLocationDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        RepairLocationDTO RL = new RepairLocationDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区代号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasplant = orgboms.Where(m => m.Plant == Plant_Organization).FirstOrDefault();

                            if (hasplant != null)
                            {
                                Plant_Organization_UID = hasplant.Plant_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op代号没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.Plant == Plant_Organization & m.BG == BG_Organization).FirstOrDefault();
                            if (hasbg != null)
                            {
                                BG_Organization_UID = hasbg.BG_Organization_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            FunPlant_Organization_UID = null;
                        }
                        else
                        {

                            var hasfunplant = orgboms.Where(m => m.Plant == Plant_Organization
                            & m.BG == BG_Organization & m.Funplant == FunPlant_Organization).FirstOrDefault();
                            if (hasfunplant != null)
                            {
                                FunPlant_Organization_UID = hasfunplant.Funplant_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        string Repair_Location_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "维修地点代码")].Value);
                        if (string.IsNullOrWhiteSpace(Repair_Location_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行维修地点代码没有值", i);
                            return errorInfo;
                        }
                        else if (Repair_Location_ID.Length > 10)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行维修地点代码长度不可超过10", i);
                            return errorInfo;
                        }

                        string Repair_Location_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "维修地点名称")].Value);
                        if (string.IsNullOrWhiteSpace(Repair_Location_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行维修地点名称没有值", i);
                            return errorInfo;
                        }
                        else if (Repair_Location_Name.Length > 30)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行维修地点名称没有值长度不可超过30", i);
                            return errorInfo;
                        }

                        string Repair_Location_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "维修地点描述")].Value);
                        if (!string.IsNullOrWhiteSpace(Repair_Location_Desc) && Repair_Location_Desc.Length > 100)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行维修地点描述长度不可超过100", i);
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

                        var hasRepairLocation = repairlocations.FirstOrDefault(m => m.BG_Organization_UID == BG_Organization_UID & m.Repair_Location_ID == Repair_Location_ID);
                        if (hasRepairLocation != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行维修地点已经存在,不可重复导入", i);
                            return errorInfo;
                        }
                        var hasitem = repairlocationlist.FirstOrDefault(m => m.BG_Organization_UID == BG_Organization_UID & m.Repair_Location_ID == Repair_Location_ID);
                        if (hasitem != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行维修地点重复", i);
                            return errorInfo;
                        }
                        RL.Plant_Organization_UID = Plant_Organization_UID;
                        RL.BG_Organization_UID = BG_Organization_UID;
                        RL.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        RL.Repair_Location_ID = Repair_Location_ID;
                        RL.Repair_Location_Name = Repair_Location_Name;
                        RL.Repair_Location_Desc = Repair_Location_Desc;
                        RL.Is_Enable = Is_Enable == "1" ? true : false;
                        RL.Created_UID = CurrentUser.AccountUId;
                        repairlocationlist.Add(RL);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(repairlocationlist);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertRepairLocationAPI");
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

        private string[] GetRepairLocationHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区代号",
                "OP代号",
                "功能厂代号",
                "维修地点代码",
                "维修地点名称",
                "维修地点描述",
                "是否启用"
            };
            return propertiesHead;
        }

        public ActionResult DoAllRLExportFunction(RepairLocationDTO search)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }
            //get Export datas
            var apiUrl = "Fixture/DoAllRLExportFunctionAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<RepairLocationDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("RepairLocation");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "功能厂", "维修地点代码", "维修地点名称", "维修地点描述", "是否启用", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("RepairLocation");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Repair_Location_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Repair_Location_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Repair_Location_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoRLExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoRLExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<RepairLocationDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("RepairLocation");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂别", "BG", "功能厂", "维修地点代码", "维修地点名称", "维修地点描述", "是否启用", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("RepairLocation");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Repair_Location_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Repair_Location_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Repair_Location_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion     维修地点设定--------------------END

        #region             治具领用----------------------START
        public ActionResult FixtureRecipients()
        {
            VendorInfoVM currentVM = new VendorInfoVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
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
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }

            return View("FixtureRecipients", currentVM);

        }

        public ActionResult QueryFixtureTotake(Fixture_Totake_MDTO search, Page page)
        {

            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            var apiUrl = string.Format("Fixture/QueryFixtureTotakeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var test = Content(result, "application/json");
            return Content(result, "application/json");
        }

        public ActionResult QueryFixtureTotakeByUid(int Fixture_Totake_M_UID)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureTotakeByUidAPI?Fixture_Totake_M_UID={0}", Fixture_Totake_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditFixtureTotake(Fixture_Totake_MDTO dto)
        {

            Fixture_Totake_MDTO FTM = new Fixture_Totake_MDTO();
            FTM.Plant_Organization_UID = dto.Plant_Organization_UID;
            FTM.BG_Organization_UID = dto.BG_Organization_UID;
            FTM.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            FTM.Shiper_UID = dto.Shiper_UID;
            FTM.Ship_Date = dto.Ship_Date;
            FTM.Totake_Date = dto.Totake_Date;
            FTM.Created_UID = CurrentUser.AccountUId;
            FTM.Totake_Name = dto.Totake_Name;
            FTM.Totake_Number = dto.Totake_Number;

            var apiUrl = string.Format("Fixture/AddOrEditFixtureTotakeAPI");
            var responMessage = APIHelper.APIPostAsync(FTM, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (result.Contains("失败"))
            {
                return Content(result, "application/json");
            }

            string Totake_NO = result.Substring(1, 13);
            apiUrl = string.Format(@"Fixture/UpdateFixtureAPI?useruid={0}&Totake_NO={1}&Totake_Date={2}", CurrentUser.AccountUId, Totake_NO, dto.Totake_Date);
            var jsonMatData = Request.Form["fixturelist"].ToString();
            responMessage = APIHelper.APIPostAsync(jsonMatData, apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFixtureByWorkshop(int Account_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name)
        {
            var apiUrl = string.Format("Fixture/GetFixtureByWorkshopAPI?Account_UID={0}&Line_ID={1}&Line_Name={2}&Machine_ID={3}&Machine_Name={4}&Process_ID={5}&Process_Name={6}",
                Account_UID, Line_ID, Line_Name, Machine_ID, Machine_Name, Process_ID, Process_Name);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetUserByWorkshop(int Account_UID)
        {
            var apiUrl = string.Format("Fixture/GetUserByWorkshopAPI?Account_UID={0}", Account_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DoAllFTExportFunction(Fixture_Totake_MDTO search)
        {

            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            //get Export datas
            var apiUrl = "Fixture/DoAllFTExportFunctionAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Totake_MDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureTotak");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "表单编号", "厂别", "BG", "功能厂", "发货者", "发货日期", "领用者", "领用日期", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureTotak");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Totake_NO;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Shiper;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Ship_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Totaker;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Totake_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoFTExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoFTExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Totake_MDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureTotak");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "表单编号", "厂别", "BG", "功能厂", "发货者", "发货日期", "领用者", "领用日期", "创建者", "创建日期", "修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureTotak");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Totake_NO;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Shiper;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Ship_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Totaker;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Totake_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Creator;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Created_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString); ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult FixtureTotakeDetail(int Fixture_Totake_M_UID, int Plant_Organization_UID,
            int BG_Organization_UID, int FunPlant_Organization_UID, string Process_ID, string WorkStation_ID,
            string Line_ID, string Fixture_Unique_ID, string Machine_ID, string Vendor_ID,
            string ShortCode, string Fixture_Name, string Modifier, string strModified_Date_from, string strModified_Date_to)
        {
            VendorInfoVM currentVM = new VendorInfoVM();
            DateTime Modified_Date_from = new DateTime();
            DateTime Modified_Date_to = new DateTime();
            try
            {
                Modified_Date_from = Convert.ToDateTime(strModified_Date_from);
                Modified_Date_to = Convert.ToDateTime(strModified_Date_to);
            }
            catch
            {

            }
            Fixture_Totake_DDTO fixturetotakedetail = new Fixture_Totake_DDTO();
            fixturetotakedetail.Fixture_Totake_M_UID = Fixture_Totake_M_UID;
            fixturetotakedetail.Plant_Organization_UID = Plant_Organization_UID;
            fixturetotakedetail.BG_Organization_UID = BG_Organization_UID;
            fixturetotakedetail.FunPlant_Organization_UID = FunPlant_Organization_UID;
            fixturetotakedetail.Process_ID = Process_ID;
            fixturetotakedetail.WorkStation_ID = WorkStation_ID;
            fixturetotakedetail.Line_ID = Line_ID;
            fixturetotakedetail.Fixture_Unique_ID = Fixture_Unique_ID;
            fixturetotakedetail.Machine_ID = Machine_ID;
            fixturetotakedetail.ShortCode = ShortCode;
            fixturetotakedetail.Fixture_Name = Fixture_Name;
            fixturetotakedetail.Modifier = Modifier;
            fixturetotakedetail.Modified_Date_from = Modified_Date_from;
            fixturetotakedetail.Modified_Date_to = Modified_Date_to;
            currentVM.fixturetotakedetail = fixturetotakedetail;
            return View(currentVM);
        }
        public ActionResult QueryFixtureTotakeDetail(Fixture_Totake_DDTO search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureTotakeDetailAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion       治具领用---------------------------END



        #endregion            karl   end
        #region Add by Rock 2017-09-29 治具履历查询--------------start
        public ActionResult FixtureResume()
        {
            ////获取厂区，BusinessGroup
            //Dictionary<int, string> plantDir = new Dictionary<int, string>();
            //Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            //var apiUrl = string.Format("ProductionPlanning/GetProjectInfoByUserAPI");
            //HttpResponseMessage responMessage = APIHelper.APIPostAsync(this.CurrentUser.GetUserInfo, apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result;
            //var item = JsonConvert.DeserializeObject<ProductionPlanningReportGetProject>(result);
            //ViewBag.Plant = item.plantDir;
            //ViewBag.OPType = item.opTypeDir;

            ////获取治具状态
            //var fxStatueApiUrl = string.Format("Fixture/GetFixtureStatusAPI");
            //HttpResponseMessage statusResponMessage = APIHelper.APIGetAsync(fxStatueApiUrl);
            //var statusresult = statusResponMessage.Content.ReadAsStringAsync().Result;
            //var fixtureStatus = JsonConvert.DeserializeObject<Dictionary<int, string>>(statusresult);
            //ViewBag.FxStatus = fixtureStatus;
            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
        public ActionResult QueryFixtureResume(FixtureResumeSearchVM search, Page page)
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
            var url = string.Format("Fixture/FixtureResumeSearchVMAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string QueryFixtureResumeByUID(int Fixture_Resume_UID, int Fixture_M_UID)
        {
            var url = string.Format("Fixture/QueryFixtureResumeByUIDAPI?Fixture_Resume_UID={0}&Fixture_M_UID={1}", Fixture_Resume_UID, Fixture_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public ActionResult DoAllExportFixtureResumeReprot(FixtureResumeSearchVM search)
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

            //get Export datas

            var apiUrl = "Fixture/DoAllExportFixtureResumeReprotAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureResumeSearchVM>>(result);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Resume");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureResumeHeadColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Resume");
                SetResumeExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[iRow, 1].Value = iRow - 1;
                    worksheet.Cells[iRow, 2].Value = item.Source_NO;
                    worksheet.Cells[iRow, 3].Value = item.PlantName;
                    worksheet.Cells[iRow, 4].Value = item.OpType_Name;
                    worksheet.Cells[iRow, 5].Value = item.Func_Name;
                    worksheet.Cells[iRow, 6].Value = item.Process_Name;
                    worksheet.Cells[iRow, 7].Value = item.WorkStation_Name;
                    worksheet.Cells[iRow, 8].Value = item.Line_Name;
                    worksheet.Cells[iRow, 9].Value = item.Equipment_No;
                    worksheet.Cells[iRow, 10].Value = item.Fixture_NO;
                    worksheet.Cells[iRow, 11].Value = item.ShortCode;
                    worksheet.Cells[iRow, 12].Value = item.Resume_Notes;
                    worksheet.Cells[iRow, 13].Value = item.User_Name;
                    worksheet.Cells[iRow, 14].Value = item.Modified_Date;
                    iRow++;
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult ExportFixtureResumeByUID(string uids)
        {
            var apiUrl = string.Format("Fixture/ExportFixtureResumeByUIDAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureResumeSearchVM>>(result);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Resume");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureResumeHeadColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Resume");
                SetResumeExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[iRow, 1].Value = iRow - 1;
                    worksheet.Cells[iRow, 2].Value = item.Source_NO;
                    worksheet.Cells[iRow, 3].Value = item.PlantName;
                    worksheet.Cells[iRow, 4].Value = item.OpType_Name;
                    worksheet.Cells[iRow, 5].Value = item.Func_Name;
                    worksheet.Cells[iRow, 6].Value = item.Process_Name;
                    worksheet.Cells[iRow, 7].Value = item.WorkStation_Name;
                    worksheet.Cells[iRow, 8].Value = item.Line_Name;
                    worksheet.Cells[iRow, 9].Value = item.Equipment_No;
                    worksheet.Cells[iRow, 10].Value = item.Fixture_NO;
                    worksheet.Cells[iRow, 11].Value = item.ShortCode;
                    worksheet.Cells[iRow, 12].Value = item.Resume_Notes;
                    worksheet.Cells[iRow, 13].Value = item.User_Name;
                    worksheet.Cells[iRow, 14].Value = item.Modified_Date;
                    iRow++;
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetResumeExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
                worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
        }
        #endregion Add by Rock 2017-09-29 治具履历查询--------------end
        #region Add By Rock 2017-10-05 国庆长假第五天未保养治具查询--------------------start
        public ActionResult FixtureNotMaintained()
        {
            ////获取厂区，BusinessGroup
            //Dictionary<int, string> plantDir = new Dictionary<int, string>();
            //Dictionary<int, string> opTypeDir = new Dictionary<int, string>();
            //var apiUrl = string.Format("ProductionPlanning/GetProjectInfoByUserAPI");
            //HttpResponseMessage responMessage = APIHelper.APIPostAsync(this.CurrentUser.GetUserInfo, apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result;
            //var item = JsonConvert.DeserializeObject<ProductionPlanningReportGetProject>(result);
            //ViewBag.Plant = item.plantDir;
            //ViewBag.OPType = item.opTypeDir;

            ////保养类别绑定
            var maintTypeUrl = string.Format("Fixture/GetMaintenanceStatusAPI?Maintenance_Type={0}", "D");
            HttpResponseMessage maintenanceResponMessage = APIHelper.APIGetAsync(maintTypeUrl);
            var maintResult = maintenanceResponMessage.Content.ReadAsStringAsync().Result;
            var maintenanceDir = JsonConvert.DeserializeObject<Dictionary<string, string>>(maintResult);
            ViewBag.MaintenanceType = maintenanceDir;

            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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

            //查询类别绑定
            Dictionary<int, string> typeDir = new Dictionary<int, string>();
            //typeDir.Add(0, "");
            typeDir.Add(1, "未保养");
            //typeDir.Add(2, "未确认");

            ViewBag.ViewType = typeDir;

            return View(currentVM);
        }
        public ActionResult QueryFixtureNotMaintained(NotMaintenanceSearchVM search, Page page)
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
            var url = string.Format("Fixture/QueryFixtureNotMaintainedAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        public ActionResult DoAllExportFixtureNotMaintainedReprot(NotMaintenanceSearchVM search)
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

            //get Export datas

            var apiUrl = "Fixture/DoAllExportFixtureNotMaintainedReprotAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<NotMaintenanceSearchVM>>(result);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture NotMaintained");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureNotMaintainedHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture NotMaintained");
                SetNotMaintainedExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[iRow, 1].Value = iRow - 1;
                    worksheet.Cells[iRow, 2].Value = item.PlantName;
                    worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                    worksheet.Cells[iRow, 4].Value = item.Func_Name;
                    worksheet.Cells[iRow, 5].Value = item.Process_Name;
                    worksheet.Cells[iRow, 6].Value = item.WorkStation_Name;
                    worksheet.Cells[iRow, 7].Value = item.Line_Name;
                    worksheet.Cells[iRow, 8].Value = item.Fixture_NO;
                    worksheet.Cells[iRow, 9].Value = item.ShortCode;
                    worksheet.Cells[iRow, 10].Value = item.CycleValue;
                    worksheet.Cells[iRow, 11].Value = item.DataFormat;
                    iRow++;
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult ExportFixtureNotMaintainedByUID(string uids, string hidDate)
        {
            var apiUrl = string.Format("Fixture/ExportFixtureNotMaintainedByUIDAPI?uids={0}&hidDate={1}", uids, hidDate);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<NotMaintenanceSearchVM>>(result);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture NotMaintained");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureNotMaintainedHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture NotMaintained");
                SetNotMaintainedExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[iRow, 1].Value = iRow - 1;
                    worksheet.Cells[iRow, 2].Value = item.PlantName;
                    worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                    worksheet.Cells[iRow, 4].Value = item.Func_Name;
                    worksheet.Cells[iRow, 5].Value = item.Process_Name;
                    worksheet.Cells[iRow, 6].Value = item.WorkStation_Name;
                    worksheet.Cells[iRow, 7].Value = item.Line_Name;
                    worksheet.Cells[iRow, 8].Value = item.Fixture_NO;
                    worksheet.Cells[iRow, 9].Value = item.ShortCode;
                    worksheet.Cells[iRow, 10].Value = item.CycleValue;
                    worksheet.Cells[iRow, 11].Value = item.DataFormat;
                    iRow++;
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetNotMaintainedExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
                worksheet.Cells["A1:K1"].Style.Font.Bold = true;
                worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }

        }
        #endregion Add By Rock 2017-10-05 国庆长假第五天未保养治具查询--------------------end
        #region Added by Jay 20170930
        #region 工站维护
        public ActionResult WorkStationMaintenance()
        {
            WorkStationVM currentVM = new WorkStationVM();
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
            //ViewBag.PageTitle = "工站维护";
            return View("WorkStationMaintenance", currentVM);
        }
        public ActionResult QueryWorkStations(WorkStationModelSearch search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryWorkStationsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryWorkStation(int uid)
        {
            var apiUrl = string.Format("Fixture/QueryWorkStationAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddWorkStation(WorkStationDTO workshop)
        {
            //去掉空格
            if (workshop != null)
            {
                if (workshop.WorkStation_ID != null)
                {
                    workshop.WorkStation_ID = workshop.WorkStation_ID.Trim();
                }
                if (workshop.WorkStation_Name != null)
                {
                    workshop.WorkStation_Name = workshop.WorkStation_Name.Trim();
                }
                if (workshop.WorkStation_Desc != null)
                {
                    workshop.WorkStation_Desc = workshop.WorkStation_Desc.Trim();
                }
            }
            var isExistUrl = "Fixture/IsWorStationExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/AddWorkStationAPI";
            var entity = workshop;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult EditWorkStation(WorkStationDTO workshop)
        {
            //去掉空格
            if (workshop != null)
            {
                if (workshop.WorkStation_ID != null)
                {
                    workshop.WorkStation_ID = workshop.WorkStation_ID.Trim();
                }
                if (workshop.WorkStation_Name != null)
                {
                    workshop.WorkStation_Name = workshop.WorkStation_Name.Trim();
                }
                if (workshop.WorkStation_Desc != null)
                {
                    workshop.WorkStation_Desc = workshop.WorkStation_Desc.Trim();
                }
            }
            var isExistUrl = "Fixture/IsWorStationExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/EditWorkStationAPI";
            var entity = workshop;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteWorkStation(int uid)
        {
            var apiUrl = "Fixture/DeleteWorkStationAPI";
            var entity = new WorkStationDTO() { WorkStation_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportWorkStation(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportWorkStationAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<WorkStationDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("WorkStation Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "工站代码", "工站名称", "工站描述", "专案", "制程", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("WorkStation Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BGName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.WorkStation_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.WorkStation_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.WorkStation_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Process_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportWorkStationByQuery(WorkStationModelSearch search)
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
            //get Export datas
            var apiUrl = "Fixture/GetWorkStationListByQueryAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<WorkStationDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("WorkStation Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "工站代码", "工站名称", "工站描述", "专案", "制程", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("WorkStation Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BGName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.WorkStation_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.WorkStation_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.WorkStation_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Process_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public FileResult WorkStation()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "WorkStation.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string ImportWorkStation(HttpPostedFileBase uploadName)
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
                        "工站代码",
                        "工站名称",
                        "工站描述",
                        "专案代码",
                        "制程代码",
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
                    var fixtureDefectCodeAPI = "Fixture/QueryAllWorkStationsAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var workStations = JsonConvert.DeserializeObject<List<WorkStationDTO>>(jsonsVendorInfo);

                    var workStaionList = new List<WorkStationDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var workStation = new WorkStationDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        int Project_UID = 0;
                        int Process_Info_UID = 0;

                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        var projectCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案代码")].Value);
                        var processInfoID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程代码")].Value);
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

                        if (string.IsNullOrWhiteSpace(projectCode))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            projectCode = projectCode.Trim();
                            var apiUrl = string.Format("Equipmentmaintenance/QueryProjectsAPI?oporgid={0}", BG_Organization_UID);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var projects = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                            var project = projects.FirstOrDefault(p => p.Project_Code == projectCode);
                            if (project != null)
                            {
                                Project_UID = project.Project_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行专案代码没找到相应的专案", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(processInfoID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行制程代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            processInfoID = processInfoID.Trim();
                            var apiUrl = string.Format("Fixture/GetProcess_InfoListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID.HasValue ? FunPlant_Organization_UID.Value : 0);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var processes = JsonConvert.DeserializeObject<List<Process_InfoDTO>>(result2);
                            var process = processes.FirstOrDefault(p => p.Process_ID == processInfoID);
                            if (process != null)
                            {
                                Process_Info_UID = process.Process_Info_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行制程代码没找到相应的制程", i);
                                return errorInfo;
                            }
                        }

                        string WorkStation_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站代码")].Value);
                        if (string.IsNullOrWhiteSpace(WorkStation_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            WorkStation_ID = WorkStation_ID.Trim();
                            if (WorkStation_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行工站代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string WorkStation_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站名称")].Value);
                        if (string.IsNullOrWhiteSpace(WorkStation_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            WorkStation_Name = WorkStation_Name.Trim();
                            if (WorkStation_Name.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行工站名称长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                        }

                        string WorkStation_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站描述")].Value);
                        if (!string.IsNullOrWhiteSpace(WorkStation_Desc))
                        {
                            WorkStation_Desc = WorkStation_Desc.Trim();
                            if (WorkStation_Desc.Length > 100)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行工站描述长度超过最大限定[100]", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }
                        //数据库判重
                        var isDbRepeated = workStations.Exists(m => m.WorkStation_ID == WorkStation_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站[{1}]已经存在,不可重复导入", i, WorkStation_ID);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = workStaionList.Exists(m => m.WorkStation_ID == WorkStation_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站[{1}]在导入数据中重复", i, WorkStation_ID);
                            return errorInfo;
                        }

                        workStation.Plant_Organization_UID = Plant_Organization_UID;
                        workStation.BG_Organization_UID = BG_Organization_UID;
                        workStation.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        workStation.WorkStation_ID = WorkStation_ID;
                        workStation.WorkStation_Name = WorkStation_Name;
                        workStation.WorkStation_Desc = WorkStation_Desc;
                        workStation.Project_UID = Project_UID;
                        workStation.Process_Info_UID = Process_Info_UID;
                        workStation.Is_Enable = Is_Enable == "1" ? true : false;
                        workStation.Created_UID = CurrentUser.AccountUId;
                        workStation.Created_Date = DateTime.Now;
                        workStation.Modified_UID = CurrentUser.AccountUId;
                        workStation.Modified_Date = DateTime.Now;

                        workStaionList.Add(workStation);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(workStaionList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertWorkStationsAPI");
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
        #region 产线维护
        public ActionResult ProductionLineMaintenance()
        {
            Production_LineVM currentVM = new Production_LineVM();
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
            //ViewBag.PageTitle = "产线维护";
            return View("ProductionLineMaintenance", currentVM);
        }
        public ActionResult QueryProductionLines(Production_LineModelSearch search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryProductionLinesAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryProductionLine(int uid)
        {
            var apiUrl = string.Format("Fixture/QueryProductionLineAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddProductionLine(Production_LineDTO line)
        {
            //去掉空格
            if (line != null)
            {
                if (line.Line_ID != null)
                {
                    if (line.Line_ID != null)
                    {
                        line.Line_ID = line.Line_ID.Trim();
                    }
                    if (line.Line_Name != null)
                    {
                        line.Line_Name = line.Line_Name.Trim();
                    }
                    if (line.Line_Desc != null)
                    {
                        line.Line_Desc = line.Line_Desc.Trim();
                    }
                }
            }
            var isExistUrl = "Fixture/IsProductionLineExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(line, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/AddProductionLineAPI";
            var entity = line;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult EditProductionLine(Production_LineDTO line)
        {
            //去掉空格
            if (line != null)
            {
                if (line.Line_ID != null)
                {
                    if (line.Line_ID != null)
                    {
                        line.Line_ID = line.Line_ID.Trim();
                    }
                    if (line.Line_Name != null)
                    {
                        line.Line_Name = line.Line_Name.Trim();
                    }
                    if (line.Line_Desc != null)
                    {
                        line.Line_Desc = line.Line_Desc.Trim();
                    }
                }
            }
            var isExistUrl = "Fixture/IsProductionLineExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(line, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/EditProductionLineAPI";
            var entity = line;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteProductionLine(int uid)
        {
            var apiUrl = "Fixture/DeleteProductionLineAPI";
            var entity = new Production_LineDTO() { Production_Line_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportProductionLine(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportProductionLineAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Production_LineDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Production_Line Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "生产线代码", "生产线名称", "生产线描述", "车间", "工站", "专案", "制程", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Production_Line Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Line_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Line_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Workshop_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Workstation_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Process_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 13].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportProductionLineByQuery(Production_LineModelSearch search)
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
            //get Export datas
            var apiUrl = "Fixture/GetProductionLineListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Production_LineDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Production_Line Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "生产线代码", "生产线名称", "生产线描述", "车间", "工站", "专案", "制程", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Production_Line Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Line_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Line_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Workshop_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Workstation_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Process_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 13].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult GetProductionLineListBySearch(Production_LineModelSearch search)
        {
            var apiUrl = string.Format("Fixture/GetProductionLineListAPI");
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public FileResult Production_Line()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Production_Line.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string ImportProductionLine(HttpPostedFileBase uploadName)
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
                        "生产线代码",
                        "生产线名称",
                        "生产线描述",
                        "生产车间代码",
                        "工站代码",
                        "专案代码",
                        "制程代码",
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

                    //获取所有生产线
                    var fixtureDefectCodeAPI = "Fixture/QueryAllProductionLinesAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var productionLines = JsonConvert.DeserializeObject<List<Production_LineDTO>>(jsonsVendorInfo);

                    var productionLineList = new List<Production_LineDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var productionLine = new Production_LineDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        int Workshop_UID = 0;
                        int WorkStation_UID = 0;
                        int Project_UID = 0;
                        int Process_Info_UID = 0;

                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        var Workshop_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产车间代码")].Value);
                        var WorkStation_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站代码")].Value);
                        var projectCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案代码")].Value);
                        var processInfoID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程代码")].Value);
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

                        if (string.IsNullOrWhiteSpace(Workshop_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行生产车间代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            Workshop_ID = Workshop_ID.Trim();
                            var apiUrl = string.Format("Fixture/GetWorkshopListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID.HasValue ? FunPlant_Organization_UID.Value : 0);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var workshops = JsonConvert.DeserializeObject<List<WorkshopDTO>>(result2);
                            var workshop = workshops.FirstOrDefault(p => p.Workshop_ID == Workshop_ID);
                            if (workshop != null)
                            {
                                Workshop_UID = workshop.Workshop_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产车间代码没找到相应的生产车间", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(WorkStation_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            WorkStation_ID = WorkStation_ID.Trim();
                            var apiUrl = string.Format("Fixture/GetWorkstationListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID.HasValue ? FunPlant_Organization_UID.Value : 0);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var workstations = JsonConvert.DeserializeObject<List<WorkStationDTO>>(result2);
                            var workstation = workstations.FirstOrDefault(p => p.WorkStation_ID == WorkStation_ID);
                            if (workstation != null)
                            {
                                WorkStation_UID = workstation.WorkStation_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行工站代码没找到相应的工站", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(projectCode))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            projectCode = projectCode.Trim();
                            var apiUrl = string.Format("Equipmentmaintenance/QueryProjectsAPI?oporgid={0}", BG_Organization_UID);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var projects = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                            var project = projects.FirstOrDefault(p => p.Project_Code == projectCode);
                            if (project != null)
                            {
                                Project_UID = project.Project_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行专案代码没找到相应的专案", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(processInfoID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行制程代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            processInfoID = processInfoID.Trim();
                            var apiUrl = string.Format("Fixture/GetProcess_InfoListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID.HasValue ? FunPlant_Organization_UID.Value : 0);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var processes = JsonConvert.DeserializeObject<List<Process_InfoDTO>>(result2);
                            var process = processes.FirstOrDefault(p => p.Process_ID == processInfoID);
                            if (process != null)
                            {
                                Process_Info_UID = process.Process_Info_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行制程代码没找到相应的制程", i);
                                return errorInfo;
                            }
                        }

                        string Line_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产线代码")].Value);
                        if (string.IsNullOrWhiteSpace(Line_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行生产线代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Line_ID = Line_ID.Trim();
                            if (Line_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产线代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string Line_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产线名称")].Value);
                        if (string.IsNullOrWhiteSpace(Line_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行生产线名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Line_Name = Line_Name.Trim();
                            if (Line_Name.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产线名称长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                        }

                        string Line_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产线描述")].Value);
                        if (!string.IsNullOrWhiteSpace(Line_Desc))
                        {
                            Line_Desc = Line_Desc.Trim();
                            if (Line_Desc.Length > 100)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产线描述长度超过最大限定[100]", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库判重
                        var isDbRepeated = productionLines.Exists(m => m.Line_ID == Line_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Workshop_UID == Workshop_UID);
                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行生产线[{1}]已经存在,不可重复导入", i, Line_ID);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = productionLineList.Exists(m => m.Line_ID == Line_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Workshop_UID == Workshop_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行生产线[{1}]在导入数据中重复", i, Line_ID);
                            return errorInfo;
                        }

                        productionLine.Plant_Organization_UID = Plant_Organization_UID;
                        productionLine.BG_Organization_UID = BG_Organization_UID;
                        productionLine.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        productionLine.Line_ID = Line_ID;
                        productionLine.Line_Name = Line_Name;
                        productionLine.Line_Desc = Line_Desc;
                        productionLine.Workshop_UID = Workshop_UID;
                        productionLine.Workstation_UID = WorkStation_UID;
                        productionLine.Project_UID = Project_UID;
                        productionLine.Process_Info_UID = Process_Info_UID;
                        productionLine.Is_Enable = Is_Enable == "1" ? true : false;
                        productionLine.Created_UID = CurrentUser.AccountUId;
                        productionLine.Created_Date = DateTime.Now;
                        productionLine.Modified_UID = CurrentUser.AccountUId;
                        productionLine.Modified_Date = DateTime.Now;

                        productionLineList.Add(productionLine);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(productionLineList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertProductionLinesAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入生产线失败：" + e.ToString();
            }
            return errorInfo;
        }
        #endregion
        #region 设备机台维护
        public ActionResult FixtureMachineMaintenance()
        {
            Fixture_MachineVM currentVM = new Fixture_MachineVM();
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
            //ViewBag.PageTitle = "设备机台维护";
            return View("FixtureMachineMaintenance", currentVM);
        }
        public ActionResult QueryFixtureMachines(Fixture_MachineModelSearch search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureMachinesAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureMachine(int uid)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureMachineAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddFixtureMachine(FixtureMachineDTO machine)
        {
            //去掉空格
            if (machine != null)
            {
                if (machine.Machine_ID != null)
                {
                    machine.Machine_ID = machine.Machine_ID.Trim();
                }
                if (machine.Equipment_No != null)
                {
                    machine.Equipment_No = machine.Equipment_No.Trim();
                }
                if (machine.Machine_Name != null)
                {
                    machine.Machine_Name = machine.Machine_Name.Trim();
                }
                if (machine.Machine_Desc != null)
                {
                    machine.Machine_Desc = machine.Machine_Desc.Trim();
                }
                if (machine.Machine_Type != null)
                {
                    machine.Machine_Type = machine.Machine_Type.Trim();
                }
            }
            var isExistUrl = "Fixture/IsFixtureMachineExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(machine, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/AddFixtureMachineAPI";
            var entity = machine;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult EditFixtureMachine(FixtureMachineDTO machine)
        {
            //去掉空格
            if (machine != null)
            {
                if (machine.Machine_ID != null)
                {
                    machine.Machine_ID = machine.Machine_ID.Trim();
                }
                if (machine.Equipment_No != null)
                {
                    machine.Equipment_No = machine.Equipment_No.Trim();
                }
                if (machine.Machine_Name != null)
                {
                    machine.Machine_Name = machine.Machine_Name.Trim();
                }
                if (machine.Machine_Desc != null)
                {
                    machine.Machine_Desc = machine.Machine_Desc.Trim();
                }
                if (machine.Machine_Type != null)
                {
                    machine.Machine_Type = machine.Machine_Type.Trim();
                }
            }
            var isExistUrl = "Fixture/IsFixtureMachineExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(machine, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/EditFixtureMachineAPI";
            var entity = machine;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteFixtureMachine(int uid)
        {
            var apiUrl = "Fixture/DeleteFixtureMachineAPI";
            var entity = new FixtureMachineDTO() { Fixture_Machine_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportFixtureMachine(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportFixtureMachineAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureMachineDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_Machine Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "机台代码", "机台名称", "机台描述", "机台类型", "产线名称", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_Machine Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Machine_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Machine_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Machine_Type;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Production_Line_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportFixtureMachineByQuery(Fixture_MachineModelSearch search)
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
            //get Export datas
            var apiUrl = "Fixture/GetFixtureMachineListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureMachineDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_Machine Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "机台代码", "机台名称", "机台描述", "机台类型", "产线名称", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_Machine Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Machine_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Machine_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Machine_Type;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Production_Line_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 根据查询条件获取机台信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetFixtureMachineList(Fixture_MachineModelSearch search)
        {
            var apiUrl = "Fixture/GetFixtureMachineListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public FileResult Fixture_Machine()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Fixture_Machine.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string ImportFixture_Machine(HttpPostedFileBase uploadName)
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
                        "机台代码",
                        "设备编号",
                        "机台名称",
                        "机台描述",
                        "机台类别",
                        "生产线代码",
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

                    //获取所有机台
                    var fixtureDefectCodeAPI = "Fixture/QueryAllFixtureMachinesAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var machines = JsonConvert.DeserializeObject<List<FixtureMachineDTO>>(jsonsVendorInfo);

                    var productionLineList = new List<FixtureMachineDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var machine = new FixtureMachineDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        int Production_Line_UID = 0;

                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        var Line_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "生产线代码")].Value);
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
                                errorInfo = string.Format("第{0}行厂区代号[{1}]没有找到", i, Plant_Organization);
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
                                errorInfo = string.Format("第{0}行OP代号[{1}]没有找到", i, BG_Organization);
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
                                errorInfo = string.Format("第{0}行功能厂代号[{1}]没有找到", i, FunPlant_Organization);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Line_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行生产线代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            Line_ID = Line_ID.Trim();
                            var apiUrl = string.Format("Fixture/GetProductionLineDTOListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID.HasValue ? FunPlant_Organization_UID.Value : 0);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var productionLines = JsonConvert.DeserializeObject<List<Production_LineDTO>>(result2);
                            var productionLine = productionLines.FirstOrDefault(p => p.Line_ID == Line_ID);
                            if (productionLine != null)
                            {
                                Production_Line_UID = productionLine.Production_Line_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行生产线代码[{1}]没找到相应的生产线", i, Line_ID);
                                return errorInfo;
                            }
                        }

                        string Machine_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台代码")].Value);
                        if (string.IsNullOrWhiteSpace(Machine_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行机台代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Machine_ID = Machine_ID.Trim();
                            if (Machine_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台代码[{1}]长度超过最大限定[10]", i, Machine_ID);
                                return errorInfo;
                            }
                        }

                        string Equipment_No = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "设备编号")].Value);
                        if (!string.IsNullOrWhiteSpace(Equipment_No))
                        {
                            Equipment_No = Equipment_No.Trim();
                            if (Equipment_No.Length > 100)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行设备编号[{1}]长度超过最大限定[100]", i, Equipment_No);
                                return errorInfo;
                            }
                        }

                        string Machine_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台名称")].Value);
                        if (string.IsNullOrWhiteSpace(Machine_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行机台名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Machine_Name = Machine_Name.Trim();
                            if (Machine_Name.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台名称[{1}]长度超过最大限定[30]", i, Machine_Name);
                                return errorInfo;
                            }
                        }

                        string Machine_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台描述")].Value);
                        if (!string.IsNullOrWhiteSpace(Machine_Desc))
                        {
                            Machine_Desc = Machine_Desc.Trim();
                            if (Machine_Desc.Length > 100)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台描述长度超过最大限定[100]", i);
                                return errorInfo;
                            }
                        }

                        string Machine_Type = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台类别")].Value);
                        if (!string.IsNullOrWhiteSpace(Machine_Type))
                        {
                            Machine_Type = Machine_Type.Trim();
                            if (Machine_Type.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台类别长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库判重
                        var isDbRepeated = machines.Exists(m => m.Machine_ID == Machine_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Production_Line_UID == Production_Line_UID);
                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行机台[{1}]已经存在,不可重复导入", i, Machine_ID);
                            return errorInfo;
                        }

                        //导入数据判重
                        var isSelfRepeated = productionLineList.Exists(m => m.Machine_ID == Machine_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Production_Line_UID == Production_Line_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行机台[{1}]在导入数据中重复", i, Machine_ID);
                            return errorInfo;
                        }

                        machine.Plant_Organization_UID = Plant_Organization_UID;
                        machine.BG_Organization_UID = BG_Organization_UID;
                        machine.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        machine.Machine_ID = Machine_ID;
                        machine.Equipment_No = Equipment_No;
                        machine.Machine_Name = Machine_Name;
                        machine.Machine_Desc = Machine_Desc;
                        machine.Machine_Type = Machine_Type;
                        machine.Production_Line_UID = Production_Line_UID;
                        machine.Is_Enable = Is_Enable == "1" ? true : false;
                        machine.Created_UID = CurrentUser.AccountUId;
                        machine.Created_Date = DateTime.Now;
                        machine.Modified_UID = CurrentUser.AccountUId;
                        machine.Modified_Date = DateTime.Now;

                        productionLineList.Add(machine);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(productionLineList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertFixtureMachinesAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入机台失败：" + e.ToString();
            }
            return errorInfo;
        }
        #region 设备机台信息与治具机台关联功能 by Steven ---------2018/09/21---------
        //TODO 2018/09/18 steven add 取设备机台信息檔功能
        public ActionResult GetEquipmentInfoList(int? Plant_Organization_UID, int? BG_Organization_UID, int? FunPlant_Organization_UID)
        {

            var apiUrl = string.Format("Fixture/GetEquipmentInfoListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //TODO 2018/09/18 steven add 批量新增设备机台信息档
        [HttpGet]
        public ActionResult BatchAppend()
        {
            int? plantUid = null;
            int? optypeUid = null;
            int? funPlantUid = null;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0)
            {
                if (CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                {
                    plantUid = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
                }
                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
                {
                    optypeUid = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }

                if (CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null)
                {
                    funPlantUid = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                }
            }

            int userID = CurrentUser.AccountUId;

            //var apiUrl = string.Format("Fixture/FixtureListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            var apiUrl = string.Format("Fixture/BatchAppendAPI?userID={0}&Plant_Organization_UID={1}&BG_Organization_UID={2}&FunPlant_Organization_UID={3}", userID, plantUid, optypeUid, funPlantUid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
            //}
            //else
            //{
            //    return new JsonResult() { Data = "{success: false}" };
            //}
        }
        #endregion


        #endregion

        #region 异常原因维护
        public ActionResult FixtureDefectCodeMaintenance()
        {
            Fixture_DefectCodeVM currentVM = new Fixture_DefectCodeVM();
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
            //ViewBag.PageTitle = "异常原因维护";
            return View("FixtureDefectCodeMaintenance", currentVM);
        }
        public ActionResult QueryFixtureDefectCodes(Fixture_DefectCodeModelSearch search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureDefectCodesAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureDefectCode(int uid)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureDefectCodeAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddFixtureDefectCode(Fixture_DefectCodeDTO workshop)
        {
            //去空格
            if (workshop != null)
            {
                if (workshop.DefectCode_ID != null)
                {
                    workshop.DefectCode_ID = workshop.DefectCode_ID.Trim();
                }
                if (workshop.DefectCode_Name != null)
                {
                    workshop.DefectCode_Name = workshop.DefectCode_Name.Trim();
                }
            }
            var isExistUrl = "Fixture/IsFixtureDefectCodeExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/AddFixtureDefectCodeAPI";
            var entity = workshop;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult EditFixtureDefectCode(Fixture_DefectCodeDTO workshop)
        {
            //去空格
            if (workshop != null)
            {
                if (workshop.DefectCode_ID != null)
                {
                    workshop.DefectCode_ID = workshop.DefectCode_ID.Trim();
                }
                if (workshop.DefectCode_Name != null)
                {
                    workshop.DefectCode_Name = workshop.DefectCode_Name.Trim();
                }
            }
            var isExistUrl = "Fixture/IsFixtureDefectCodeExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/EditFixtureDefectCodeAPI";
            var entity = workshop;
            entity.Modified_UID = CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteFixtureDefectCode(int uid)
        {
            var apiUrl = "Fixture/DeleteFixtureDefectCodeAPI";
            var entity = new Fixture_DefectCodeDTO() { Fixture_Defect_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportFixtureDefectCode(string uids)
        {
            var apiUrl = string.Format("Fixture/DoExportFixtureDefectCodeAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_DefectCodeDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_DefectCode Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "异常代码", "异常名称", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_DefectCode Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.DefectCode_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.DefectCode_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportFixtureDefectCodeByQuery(Fixture_DefectCodeModelSearch search)
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
            //var apiUrl = string.Format("Fixture/GetFixtureDefectCodeListAPI?searchModel={0}", search);
            var apiUrl = "Fixture/GetFixtureDefectCodeListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_DefectCodeDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_DefectCode Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "异常代码", "异常名称", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_DefectCode Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.DefectCode_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.DefectCode_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public FileResult Fixture_DefectCode()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Fixture_DefectCode.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string ImportFixtureDefectCode(HttpPostedFileBase uploadName)
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
                        "异常代码",
                        "异常名称",
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

                    //获取所有异常原因
                    var fixtureDefectCodeAPI = "Fixture/QueryAllFixtureDefectCodesAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var fixtureDefectCodes = JsonConvert.DeserializeObject<List<Fixture_DefectCodeDTO>>(jsonsVendorInfo);

                    var defectCodeList = new List<Fixture_DefectCodeDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var defectCode = new Fixture_DefectCodeDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
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


                        string defectCodeId = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常代码")].Value);
                        if (string.IsNullOrWhiteSpace(defectCodeId))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            defectCodeId = defectCodeId.Trim();
                            if (defectCodeId.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string DefectCode_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常名称")].Value);
                        if (string.IsNullOrWhiteSpace(DefectCode_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            DefectCode_Name = DefectCode_Name.Trim();
                            if (DefectCode_Name.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常名称长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库判重
                        var isDbRepeated = fixtureDefectCodes.Exists(m => m.DefectCode_ID == defectCodeId && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常[{1}]已经存在,不可重复导入", i, defectCodeId);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = defectCodeList.Exists(m => m.DefectCode_ID == defectCodeId && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常[{1}]在导入数据中重复", i, defectCodeId);
                            return errorInfo;
                        }
                        defectCode.Plant_Organization_UID = Plant_Organization_UID;
                        defectCode.BG_Organization_UID = BG_Organization_UID;
                        defectCode.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        defectCode.DefectCode_ID = defectCodeId;
                        defectCode.DefectCode_Name = DefectCode_Name;
                        defectCode.Is_Enable = Is_Enable == "1" ? true : false;
                        defectCode.Created_UID = CurrentUser.AccountUId;
                        defectCode.Created_Date = DateTime.Now;
                        defectCode.Modified_UID = CurrentUser.AccountUId;
                        defectCode.Modified_Date = DateTime.Now;

                        defectCodeList.Add(defectCode);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(defectCodeList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertFixtureDefectCodeAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常原因失败:" + e.ToString();
            }
            return errorInfo;
        }
        public ActionResult GetFixture_DefectCode(string DefectCode_ID, int Plant_Organization_UID, int BG_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetFixture_DefectCodeAPI?DefectCode_ID={0}&Plant_Organization_UID={1}&BG_Organization_UID={2}", DefectCode_ID, Plant_Organization_UID, BG_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
        #region 维修对策维护
        public ActionResult FixtureRepairSolutionMaintenance()
        {
            Fixture_RepairSolutionVM currentVM = new Fixture_RepairSolutionVM();
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
            //ViewBag.PageTitle = "维修对策维护";
            return View("FixtureRepairSolutionMaintenance", currentVM);
        }
        public ActionResult QueryFixtureRepairSolutions(Fixture_RepairSolutionModelSearch search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureRepairSolutionsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureRepairSolution(int uid)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureRepairSolutionAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddFixtureRepairSolution(Fixture_RepairSolutionDTO workshop)
        {
            if (workshop != null)
            {
                if (workshop.RepairSolution_ID != null)
                {
                    workshop.RepairSolution_ID = workshop.RepairSolution_ID.Trim();
                }
                if (workshop.RepairSolution_Name != null)
                {
                    workshop.RepairSolution_Name = workshop.RepairSolution_Name.Trim();
                }
            }
            var isExistUrl = "Fixture/IsFixtureRepairSolutionExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/AddFixtureRepairSolutionAPI";
            var entity = workshop;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult EditFixtureRepairSolution(Fixture_RepairSolutionDTO workshop)
        {
            if (workshop != null)
            {
                if (workshop.RepairSolution_ID != null)
                {
                    workshop.RepairSolution_ID = workshop.RepairSolution_ID.Trim();
                }
                if (workshop.RepairSolution_Name != null)
                {
                    workshop.RepairSolution_Name = workshop.RepairSolution_Name.Trim();
                }
            }
            var isExistUrl = "Fixture/IsFixtureRepairSolutionExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/EditFixtureRepairSolutionAPI";
            var entity = workshop;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteFixtureRepairSolution(int uid)
        {
            var apiUrl = "Fixture/DeleteFixtureRepairSolutionAPI";
            var entity = new Fixture_RepairSolutionDTO() { Fixture_RepairSolution_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportFixtureRepairSolution(string uids)
        {
            var apiUrl = string.Format("Fixture/DoExportFixtureRepairSolutionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_RepairSolutionDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_RepairSolution Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "维修对策代码", "维修对策名称", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_RepairSolution Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.RepairSolution_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.RepairSolution_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportFixtureRepairSolutionByQuery(Fixture_RepairSolutionModelSearch search)
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

            var apiUrl = "Fixture/GetFixtureRepairSolutionListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_RepairSolutionDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_RepairSolution Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "维修对策代码", "维修对策名称", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_RepairSolution Maintenance");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.RepairSolution_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.RepairSolution_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public FileResult Fixture_RepairSolution()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Fixture_RepairSolution.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string ImportFixtureRepairSolution(HttpPostedFileBase uploadName)
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
                        "维修对策代码",
                        "维修对策名称",
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

                    //获取所有维修对策
                    var fixtureDefectCodeAPI = "Fixture/QueryAllFixtureRepairSolutionsAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var fixtureDefectCodes = JsonConvert.DeserializeObject<List<Fixture_RepairSolutionDTO>>(jsonsVendorInfo);

                    var defectCodeList = new List<Fixture_RepairSolutionDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var repairSolution = new Fixture_RepairSolutionDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
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


                        string RepairSolution_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "维修对策代码")].Value);
                        if (string.IsNullOrWhiteSpace(RepairSolution_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行维修对策代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            RepairSolution_ID = RepairSolution_ID.Trim();
                            if (RepairSolution_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行维修对策代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string RepairSolution_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "维修对策名称")].Value);
                        if (string.IsNullOrWhiteSpace(RepairSolution_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行维修对策名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            RepairSolution_Name = RepairSolution_Name.Trim();
                            if (RepairSolution_Name.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行维修对策名称长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库判重
                        var isDbRepeated = fixtureDefectCodes.Exists(m => m.RepairSolution_ID == RepairSolution_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行维修对策[{1}]已经存在,不可重复导入", i, RepairSolution_ID);
                            return errorInfo;
                        }

                        //导入数据自身判重
                        var isSelfRepeated = defectCodeList.Exists(m => m.RepairSolution_ID == RepairSolution_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行维修对策[{1}]在导入数据中重复", i, RepairSolution_ID);
                            return errorInfo;
                        }
                        repairSolution.Plant_Organization_UID = Plant_Organization_UID;
                        repairSolution.BG_Organization_UID = BG_Organization_UID;
                        repairSolution.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        repairSolution.RepairSolution_ID = RepairSolution_ID;
                        repairSolution.RepairSolution_Name = RepairSolution_Name;
                        repairSolution.Is_Enable = Is_Enable == "1" ? true : false;
                        repairSolution.Created_UID = CurrentUser.AccountUId;
                        repairSolution.Created_Date = DateTime.Now;
                        repairSolution.Modified_UID = CurrentUser.AccountUId;
                        repairSolution.Modified_Date = DateTime.Now;

                        defectCodeList.Add(repairSolution);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(defectCodeList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertFixtureRepairSolutionAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入维修对策失败：" + e.ToString();
            }
            return errorInfo;
        }
        #endregion

        #region 治具状态监控
        public ActionResult FixtureStatusMoniter()
        {
            WorkStationVM currentVM = new WorkStationVM();
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
            //ViewBag.PageTitle = "工站维护";
            return View("FixtureStatusMoniter", currentVM);
        }
        public ActionResult QueryFixtureStatusMoniter(FixtureDTO search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureStatusMoniterAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult ExportFixtureStatusMoniterByQuery(FixtureDTO search)
        {
            //get Export datas
            var apiUrl = "Fixture/GetFixtureStatusMoniterListBySearchAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Status");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "车间", "制程", "工站", "线别", "机台", "治具唯一编码", "治具短码", "治具图号", "治具二维码", "扫码时间", "判定结果", "备注" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Status");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Workshop;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Process_Info;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Workstation;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Machine_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.TwoD_Barcode;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ScanedTime.ToString("yyyy-MM-dd hh:mm:ss");
                    worksheet.Cells[index + 2, 15].Value = currentRecord.IsPass ? "Pass" : "NG";
                    worksheet.Cells[index + 2, 16].Value = currentRecord.MoniterStatusMark;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult ExportFixtureStatusMoniterBySelected(FixtureDTO search, string IDArr)
        {
            //get Export datas
            search.Fixture_Name = IDArr;
            var apiUrl = "Fixture/GetFixtureStatusMoniterListBySelectedAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Status");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "车间", "制程", "工站", "线别", "机台", "治具唯一编码", "治具短码", "治具图号", "治具二维码", "扫码时间", "判定结果", "备注" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Status");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Workshop;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Process_Info;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Workstation;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Machine_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.TwoD_Barcode;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ScanedTime.ToString("yyyy-MM-dd hh:mm:ss");
                    worksheet.Cells[index + 2, 15].Value = currentRecord.IsPass ? "Pass" : "NG";
                    worksheet.Cells[index + 2, 16].Value = currentRecord.MoniterStatusMark;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream") { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion
        #endregion

        #region    治具归还
        /// <summary>
        /// 治具归还首页
        /// </summary>
        /// <returns></returns>
        public ActionResult FixtureReturnIndex()
        {
            Fixture_ReturnVM currentVM = new Fixture_ReturnVM();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
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
            if (currentVM.Plants.Count == 1)
            {
                var BGS = new List<BGVM>();
                int organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                {
                    organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                var apiUrl2 = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", currentVM.Plants[0].Plant_OrganizationUID, organization_UID);
                HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs2 = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result2);
                foreach (var item in systemOrgDTOs2)
                {
                    BGVM bgvm = new BGVM() { BG = item.OP_TYPES, BG_OrganizationUID = item.Organization_UID };
                    BGS.Add(bgvm);
                }
                currentVM.optypes = BGS;
            }
            if (currentVM.optypes != null && currentVM.optypes.Count == 1)
            {
                var funplants = new List<FunPlantVM>();
                int FunPlant_Organization_UID = 0;

                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value != 0)
                {
                    FunPlant_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                    FunPlantVM funplantvm = new FunPlantVM { FunPlant = CurrentUser.GetUserInfo.OrgInfo[0].Funplant, System_FunPlant_UID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID };
                    funplants.Add(funplantvm);
                }
                else
                {
                    var apiUrl2 = string.Format("Fixture/GetOrgByParantAPI?Parant_UID={0}&type={1}", currentVM.optypes[0].BG_OrganizationUID, 2);
                    HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl2);

                    var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                    var systemfunplant = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result2);
                    foreach (var item in systemfunplant)
                    {
                        FunPlantVM funplantvm = new FunPlantVM() { FunPlant = item.Organization_Name, System_FunPlant_UID = item.Organization_UID };
                        funplants.Add(funplantvm);
                    }
                    currentVM.funplants = funplants;
                }

            }
            return View(currentVM);
        }

        /// <summary>
        /// 治具归还查询
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryFixtureToReturn(Fixture_Return_MDTO search, Page page)
        {
            //这里要根据登录者的厂区所在厂区，BG等查询
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            var apiUrl = string.Format("Fixture/QueryFixtureToReturnAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var test = Content(result, "application/json");
            return Content(result, "application/json");


        }
        /// <summary>
        /// 治具归还添加和修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult AddOrEditFixtureToReturn(Fixture_Return_MDTO dto)
        {
            return View();
        }
        [HttpPost]
        public ActionResult FetchFixtureTotakeforFixtureReturn(int plant_ID, int op_type, int funPlant)
        {
            var apiUrl = $@"Fixture/FetchFixtureTotakeforFixtureReturnAPI?plant_ID={plant_ID}&op_type={op_type}&funPlant={funPlant}";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 根据领用单号获取所有的治具信息返回List<Fixture_Taken_Info>
        /// </summary>
        /// <param name="Take_NO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FetchAllFixturesBasedTakeNo(string Take_NO)
        {
            var apiUrl = $@"Fixture/FetchAllFixturesBasedTakeNoAPI?Take_NO={Take_NO}";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        /// <summary>
        /// 添加治具归还单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertFixtureReturn(string model)
        {
            string errorInfo = string.Empty;
            IList<Fixture_Return_Post> ctm = new JavaScriptSerializer().Deserialize<IList<Fixture_Return_Post>>(model);
            List<Fixture_Return_MDTO> fixRetM = new List<Fixture_Return_MDTO>();
            List<Fixture_Return_DDTO> fixRetD = new List<Fixture_Return_DDTO>();
            //获取当天插入多少个主档，生成Return_NO
            var apiUrl = string.Format("Fixture/GetCurrentReturnNubAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            string Return_NO = "R" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + result;

            var fixM = new Fixture_Return_MDTO();
            fixM.Fixture_Totake_M_UID = ctm[0].Fixture_Totake_M_UID;
            fixM.Return_NO = Return_NO;
            fixM.Return_Name = ctm[0].Return_Name;
            fixM.Return_User = ctm[0].Return_User;
            fixM.Return_Date = ctm[0].Return_Date;
            fixM.Created_UID = CurrentUser.AccountUId;
            fixM.Create_Date = System.DateTime.Now;
            fixM.Plant_Organization_UID = ctm[0].Plant_Organization_UID;
            fixM.BG_Organization_UID = ctm[0].BG_Organization_UID;
            fixM.FunPlant_Organization_UID = ctm[0].FunPlant_Organization_UID;
            fixM.Modified_UID = CurrentUser.AccountUId;
            fixM.Modified_Date = System.DateTime.Now;
            var apiUrlM = string.Format("Fixture/AddFixtureRetrunAPI");
            HttpResponseMessage responMessageM = APIHelper.APIPostAsync(fixM, apiUrlM);
            var resultM = responMessageM.Content.ReadAsStringAsync().Result.Replace("\"", "");
            int ret2 = 0;
            foreach (var item in ctm)
            {
                var fixD = new Fixture_Return_DDTO();
                fixD.Fixture_Totake_D_UID = item.Fixture_Totake_D_UID;
                fixD.Fixture_Return_M_UID = Convert.ToInt32(resultM);
                fixD.Fixture_M_UID = item.Fixture_M_UID;
                fixD.Created_UID = CurrentUser.AccountUId;
                fixD.Created_Date = System.DateTime.Now;
                fixD.IS_Return = 1;
                fixD.Modified_UID = CurrentUser.AccountUId;
                fixD.Modified_Date = System.DateTime.Now;
                fixD.Return_NO = Return_NO;
                var apiUrlD = string.Format("Fixture/AddFixtureRetrunDAPI");
                HttpResponseMessage responMessageD = APIHelper.APIPostAsync(fixD, apiUrlD);
                var resultD = responMessageD.Content.ReadAsStringAsync().Result.Replace("\"", "");
                ret2 += Convert.ToInt16(resultD);
            }

            if (Convert.ToInt32(resultM) > 0 && ret2 > 0)
            {
                var obj = new { success = Return_NO,status=1 };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var obj = new { success = "False",status=0 };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 根据归还唯一单号获取所有数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QueryFixtureReturnUid(int uid)
        {
            var apiUrl = $@"Fixture/QueryFixtureReturnUidAPI?uid={uid}";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 更新归还单详细档
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult FixtureReturnUpdatePost(Fixture_Return_MDTO dto)
        {

            var apiUrl = string.Format("Fixture/FixtureReturnUpdatePostAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var test = Content(result, "application/json");
            return Content(result, "application/json");
        }
        /// <summary>
        /// 根据Return_M_UID查询所有详细档
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult FixtureReturnDetail(int uid)
        {
            var apiUrl = $@"Fixture/FixtureReturnDetailAPI?uid={uid}";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            List<Fixture_Return_MDTO> model = new JavaScriptSerializer().Deserialize<List<Fixture_Return_MDTO>>(result);
            ViewBag.Entity = model;
            return View();
        }
        [HttpPost]
        public ActionResult DelFixtureReturnM(int uid)
        {
            var apiUrl = $@"Fixture/DelFixtureReturnMAPI?uid={uid}";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            if (Convert.ToInt16(result) > 0)
            {
                return Json(new { success = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            else if (Convert.ToInt16(result) < 0)
            {
                return Json(new { success = "删除失败，已存在归还治具，不允许删除！" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FetchAllFixturesBasedReturnMUID(int uid)
        {
            var apiUrl = $@"Fixture/FetchAllFixturesBasedReturnMUIDAPI?uid={uid}";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result,"application/json");
        }
        [HttpPost]
        public ActionResult UpdateFixtureReturn(string model)
        {
            string errorInfo = string.Empty;
            IList<Fixture_Return_Post> ctm = new JavaScriptSerializer().Deserialize<IList<Fixture_Return_Post>>(model);
            List<Fixture_Return_MDTO> fixRetM = new List<Fixture_Return_MDTO>();
            List<Fixture_Return_DDTO> fixRetD = new List<Fixture_Return_DDTO>();

            var fixM = new Fixture_Return_MDTO();
            fixM.Fixture_Return_M_UID = ctm[0].Fixture_Return_M_UID;
            fixM.Return_Name = ctm[0].Return_Name;
            fixM.Return_User = ctm[0].Return_User;
            fixM.Modified_UID = CurrentUser.AccountUId;
            fixM.Modified_Date = System.DateTime.Now;
            var apiUrlM = string.Format("Fixture/UpdateFixtureRetrunAPI");
            HttpResponseMessage responMessageM = APIHelper.APIPostAsync(fixM, apiUrlM);
            var resultM = responMessageM.Content.ReadAsStringAsync().Result.Replace("\"", "");
            int ret2 = 1;
            //foreach (var item in ctm)
            //{
            //    var fixD = new Fixture_Return_DDTO();
            //    fixD.Fixture_Return_D_UID =item.Fixture_Return_D_UID;
            //    fixD.Modified_UID = CurrentUser.AccountUId;
            //    fixD.Modified_Date = System.DateTime.Now;
            //    var apiUrlD = string.Format("Fixture/UpdateFixtureRetrunDAPI");
            //    HttpResponseMessage responMessageD = APIHelper.APIPostAsync(fixD, apiUrlD);
            //    var resultD = responMessageD.Content.ReadAsStringAsync().Result.Replace("\"", "");
            //    ret2 += Convert.ToInt16(resultD);
            //}

            if (Convert.ToInt32(resultM) > 0 && ret2 > 0)
            {
                var obj = new { success = "修改成功" };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var obj = new { success = "False" };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportFixtrueReturn2Excel(Fixture_Return_MDTO search)
        {

            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            int BG_Organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                BG_Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.BG_Organization_UID == 0 && BG_Organization_UID != 0)
            {
                search.BG_Organization_UID = BG_Organization_UID;
            }

            //get Export datas
            var apiUrl = "Fixture/ExportFixtrueReturn2ExcelAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Return_Index>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FixtureReturn");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "表单编号", "领用单编号","厂别", "BG", "功能厂", "归还人员NT","归还人姓名", "领用日期", "设备编号", "最后修改者", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FixtureReturn");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Return_NO;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Taken_NO;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Return_User;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Return_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Totake_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion

        #region  治具保养----------Add by keyong 2017-09-30
        /// <summary>
        /// 初始化加载治具保养页面
        /// </summary>
        /// <returns></returns>
        public ActionResult FixtureMaintenance()
        {

            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
            //ViewBag.PageTitle = "治具保养";
            return View("FixtureMaintenance", currentVM);

        }
        /// <summary>
        /// 初始化查询治具保养数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryFixtureMaintenance(Fixture_Maintenance_RecordDTO search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureMaintenanceAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 根据保养ID获取保养数据
        /// </summary>
        /// <param name="Fixture_Maintenance_Record_UID"></param>
        /// <returns></returns>
        public ActionResult QueryFixtureMaintenanceByUid(int Fixture_Maintenance_Record_UID)
        {
            var apiUrl = string.Format("Fixture/QueryFixtureMaintenanceByUidAPI?Fixture_Maintenance_Record_UID={0}", Fixture_Maintenance_Record_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 导出保养资料
        /// </summary>
        /// <param name="Fixture_Maintenance_Record_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoAllExportFixtureMaintenanceReprot(Fixture_Maintenance_RecordDTO search, Page page)
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
            //get Export datas
            var apiUrl = "Fixture/DoAllExportFixtureMaintenanceReprotAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Maintenance_RecordDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具保养单编号", "治具唯一编号", "线别", "工站", "设备编码", "治具编号(图号)", "治具短码", "保养状态", "保养者", "确认状态", "确认者" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Maintenance_Record_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Workstation;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.WorkStation_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ShortCode;
                    string maintenance_Status = "未保养";
                    if (currentRecord.Maintenance_Status == 1)
                    {
                        maintenance_Status = "完成保养";
                    }
                    else if (currentRecord.Maintenance_Status == 1)
                    {
                        maintenance_Status = "免保养";
                    }

                    worksheet.Cells[index + 2, 12].Value = maintenance_Status;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Maintenance_Person_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Confirm_Status == 1 ? "确认完成" : "未确认";
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Confirmor;

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 15; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:O1"].Style.Font.Bold = true;
                worksheet.Cells["A1:O1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:O1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:O1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:O1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:O1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        /// <summary>
        /// 导出保养资料
        /// </summary>
        /// <param name="Fixture_Maintenance_Record_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoExportFixtureMaintenanceReprot(string Fixture_Maintenance_Record_UIDs)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportFixtureMaintenanceReprotAPI?Fixture_Maintenance_Record_UIDs={0}", Fixture_Maintenance_Record_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Fixture_Maintenance_RecordDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具保养单编号", "治具唯一编号", "线别", "工站", "设备编码", "治具编号(图号)", "治具短码", "保养状态", "保养者", "确认状态", "确认者" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Maintenance_Record_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Workstation;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.WorkStation_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ShortCode;
                    string maintenance_Status = "未保养";
                    if (currentRecord.Maintenance_Status == 1)
                    {
                        maintenance_Status = "完成保养";
                    }
                    else if (currentRecord.Maintenance_Status == 1)
                    {
                        maintenance_Status = "免保养";
                    }

                    worksheet.Cells[index + 2, 12].Value = maintenance_Status;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Maintenance_Person_Name;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Confirm_Status == 1 ? "确认完成" : "未确认";
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Confirmor;

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 15; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:O1"].Style.Font.Bold = true;
                worksheet.Cells["A1:O1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:O1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:O1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:O1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:O1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult GetFixtureMaintenance(string Fixture_Maintenance_Record_UIDs, int straus)
        {
            var apiUrl = string.Format("Fixture/GetFixtureMaintenanceAPI?Fixture_Maintenance_Record_UIDs={0}&straus={1}", Fixture_Maintenance_Record_UIDs, straus);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFixtureMaintenanceList(string fixture_Maintenance_RecordDTO, int straus)
        {
            Fixture_Maintenance_RecordDTO Fixture_Maintenance_RecordDTO = JsonConvert.DeserializeObject<Fixture_Maintenance_RecordDTO>(fixture_Maintenance_RecordDTO);

            var apiUrl = string.Format("Fixture/GetFixtureMaintenanceListAPI?straus={0}", straus);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(Fixture_Maintenance_RecordDTO, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 更新状态，保养，确认，取消保养
        /// </summary>
        /// <param name="Fixture_Maintenance_Record_UIDs"></param>
        /// <param name="NTID"></param>
        /// <param name="date"></param>
        /// <param name="straus"></param>
        /// <returns></returns>
        public string UpdateFixture_Maintenance_Record(string Fixture_Maintenance_Record_UIDs, int NTID, string personNumber, string personName, DateTime date, int straus)
        {
            int CurrentUserID = CurrentUser.AccountUId;
            //  var apiUrl = string.Format("Fixture/UpdateFixture_Maintenance_RecordAPI?Fixture_Maintenance_Record_UIDs={0}&NTID={1}&personNumber={2}&personName={3}&date={4}&straus={5}&CurrentUserID={6}", Fixture_Maintenance_Record_UIDs, NTID, personNumber, personName, date, straus, CurrentUserID);
            var apiUrl = string.Format("Fixture/UpdateFixture_Maintenance_RecordAPI");

            Fixture_Maintenance_RecordDTOCS fixture_Maintenance_RecordDTOCS = new Fixture_Maintenance_RecordDTOCS()
            {
                fixture_Maintenance_Record_UIDs = Fixture_Maintenance_Record_UIDs,
                NTID = NTID,
                personName = personName,
                personNumber = personNumber,
                date = date,
                CurrentUserID = CurrentUserID,
                straus = straus

            };
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(fixture_Maintenance_RecordDTOCS, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;


        }
        public ActionResult GetFixtureMaintenance_Plan(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type)
        {
            var apiUrl = string.Format("Fixture/GetFixtureMaintenance_PlanAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}&Maintenance_Type={3}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Maintenance_Type);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string CreateFixture_Maintenance_Records(Fixture_Maintenance_RecordDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            if (dto.Plant_Organization_UID == 0)
            {
                dto.Plant_Organization_UID = GetPlantOrgUid();
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (dto.BG_Organization_UID == 0 && organization_UID != 0)
            {
                dto.BG_Organization_UID = organization_UID;
            }
            var apiUrl = string.Format("Fixture/CreateFixture_Maintenance_RecordsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        #endregion 治具保养--------------end
        #region Added by Justin 20170929
        #region 治具异常原因及相应维修策略设定

        public ActionResult GetDefectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetDefectListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetRepairSoulutionList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetRepairSoulutionListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DefectRepairSolution()
        {
            DefectRepairSolutionVM currentVM = new DefectRepairSolutionVM();
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
            //ViewBag.PageTitle = "治具维修原因及对策关系维护";
            return View("DefectRepairSolution", currentVM);
        }
        public ActionResult QueryDefectRepairs(DefectRepairSearch search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryDefectRepairsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryDefectRepair(int uid)
        {
            var apiUrl = string.Format("Fixture/QueryDefectRepairAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddDefectRepair(DefectRepairSolutionDTO DefectRepair)
        {
            var isExistUrl = "Fixture/IsDefectRepairExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(DefectRepair, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/AddDefectRepairAPI";
            var entity = DefectRepair;
            entity.Modified_UID = CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult EditDefectRepair(DefectRepairSolutionDTO DefectRepair)
        {
            var isExistUrl = "Fixture/IsDefectRepairExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(DefectRepair, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Fixture/EditDefectRepairAPI";
            var entity = DefectRepair;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DeleteDefectRepair(int uid)
        {
            var apiUrl = "Fixture/DeleteDefectRepairAPI";
            var entity = new DefectRepairSolutionDTO() { Defect_RepairSolution_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportDefectRepair(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportDefectRepairAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<DefectRepairSolutionDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_Defect_Repair");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "异常名称", "维修策略", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_Defect_Repair");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BGName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_DefectName;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Repair_SoulutionName;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Enable ? "Y" : "N";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportDefectRepairByQuery(DefectRepairSearch search)
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

            //get Export datas
            var apiUrl = "Fixture/GetDefectRepairListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<DefectRepairSolutionDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture_Defect_Repair");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "异常名称", "维修策略", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture_Defect_Repair");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BGName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_DefectName;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Repair_SoulutionName;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Enable ? "Y" : "N";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public FileResult FixtureDefect_RepairRelation()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "DefectCode_RepairSolution.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string ImportDefectRepairRealtion(HttpPostedFileBase uploadName)
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
                        "异常代码",
                        "维修对策代码",
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

                    //获取所有治具异常原因维修对策
                    var fixtureDefectCodeAPI = "Fixture/QueryAllDefectCode_RepairSolutionAPI";
                    HttpResponseMessage fixtureDefectRepairMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsDefectRepair = fixtureDefectRepairMessage.Content.ReadAsStringAsync().Result;
                    var defectRepairs = JsonConvert.DeserializeObject<List<DefectRepairSolutionDTO>>(jsonsDefectRepair);

                    var defectCodeRepairSolutionList = new List<DefectRepairSolutionDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var repairSolutionDto = new DefectRepairSolutionDTO();

                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);

                        int Fixtrue_Defect_UID = 0;
                        int Repair_Solution_UID = 0;
                        var DefectCode_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常代码")].Value);
                        var RepairSolution_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "维修对策代码")].Value);

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
                        //异常原因
                        if (string.IsNullOrWhiteSpace(DefectCode_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            DefectCode_ID = DefectCode_ID.Trim();
                            var apiUrl = string.Format("Fixture/GetDefectListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID.HasValue ? FunPlant_Organization_UID.Value : 0);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var defectCodes = JsonConvert.DeserializeObject<List<Fixture_DefectCodeDTO>>(result2);
                            var defectCode = defectCodes.FirstOrDefault(p => p.DefectCode_ID == DefectCode_ID);
                            if (defectCode != null)
                            {
                                Fixtrue_Defect_UID = defectCode.Fixture_Defect_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常代码没找到相应的异常", i);
                                return errorInfo;
                            }
                        }

                        //维修对策
                        if (string.IsNullOrWhiteSpace(RepairSolution_ID))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行维修对策代码不能为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            RepairSolution_ID = RepairSolution_ID.Trim();
                            var apiUrl = string.Format("Fixture/GetRepairSoulutionListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID.HasValue ? FunPlant_Organization_UID.Value : 0);
                            HttpResponseMessage responMessage2 = APIHelper.APIGetAsync(apiUrl);
                            var result2 = responMessage2.Content.ReadAsStringAsync().Result;
                            var repairSolutions = JsonConvert.DeserializeObject<List<Fixture_RepairSolutionDTO>>(result2);
                            var epairSolution = repairSolutions.FirstOrDefault(p => p.RepairSolution_ID == RepairSolution_ID);
                            if (epairSolution != null)
                            {
                                Repair_Solution_UID = epairSolution.Fixture_RepairSolution_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行维修对策代码没找到相应的维修对策", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库判重
                        var isDbRepeated = defectRepairs.Exists(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Fixtrue_Defect_UID == Fixtrue_Defect_UID && m.Repair_Solution_UID == Repair_Solution_UID);
                        if (isDbRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行数据已经存在,不可重复导入", i);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = defectCodeRepairSolutionList.Exists(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Fixtrue_Defect_UID == Fixtrue_Defect_UID && m.Repair_Solution_UID == Repair_Solution_UID);
                        if (isSelfRepeated)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行导入数据重复", i);
                            return errorInfo;
                        }

                        repairSolutionDto.Plant_Organization_UID = Plant_Organization_UID;
                        repairSolutionDto.BG_Organization_UID = BG_Organization_UID;
                        repairSolutionDto.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        repairSolutionDto.Fixtrue_Defect_UID = Fixtrue_Defect_UID;
                        repairSolutionDto.Repair_Solution_UID = Repair_Solution_UID;
                        repairSolutionDto.Is_Enable = Is_Enable == "1" ? true : false;
                        repairSolutionDto.Created_UID = CurrentUser.AccountUId;
                        repairSolutionDto.Created_Date = DateTime.Now;
                        repairSolutionDto.Modified_UID = CurrentUser.AccountUId;
                        repairSolutionDto.Modified_Date = DateTime.Now;

                        defectCodeRepairSolutionList.Add(repairSolutionDto);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(defectCodeRepairSolutionList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertDefectRepairSolutionAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入车间失败:" + e.ToString();
            }
            return errorInfo;
        }
        #endregion
        #region 车间维护
        public ActionResult QueryWorkshops(WorkshopModelSearch search, Page page)
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
            var apiUrl = string.Format("Settings/QueryWorkshopsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ImportWorkshop(HttpPostedFileBase uploadName)
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
                        "车间代码",
                        "楼栋名称",
                        "楼层名称",
                        "车间名称",
                        "是否启用"
                    };
                    bool excelIsError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[1, i].Value != null && !string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Value.ToString()))
                        {
                            var result = worksheet.Cells[1, i].Value.ToString();
                            var hasItem = propertiesHead.FirstOrDefault(m => m.Contains(result));
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
                    //获得厂区
                    var plantsAPI = string.Format("Fixture/QueryAllPlantAPI?PLANT_UID={0}&leval={1}", GetPlantOrgUid(), "1");
                    HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(plantsAPI);
                    var jsonPlants = plantsmessage.Content.ReadAsStringAsync().Result;
                    var plants = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonPlants);

                    //获取所有车间原因
                    var fixtureDefectCodeAPI = "Fixture/QueryAllWorkshopsAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsWorkshop = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var workshops = JsonConvert.DeserializeObject<List<WorkshopDTO>>(jsonsWorkshop);

                    var workshopList = new List<WorkshopDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var workshopDto = new WorkshopDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int FunPlant_Organization_UID = 0;
                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行功能厂代号不能为空", i);
                            return errorInfo;
                        }
                        else
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }


                        string Workshop_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "车间代码")].Value);
                        if (string.IsNullOrWhiteSpace(Workshop_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行车间代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Workshop_ID = Workshop_ID.Trim();
                            if (Workshop_ID.Length > 10)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行车间代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string Building_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "楼栋名称")].Value);
                        if (string.IsNullOrWhiteSpace(Building_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行楼栋名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Building_Name = Building_Name.Trim();
                            if (Building_Name.Length > 20)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行楼栋名称长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string Floor_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "楼层名称")].Value);
                        if (string.IsNullOrWhiteSpace(Floor_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行楼层名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Floor_Name = Floor_Name.Trim();
                            if (Floor_Name.Length > 20)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行楼层名称长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string Workshop_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "车间名称")].Value);
                        if (string.IsNullOrWhiteSpace(Workshop_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行车间名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Workshop_Name = Workshop_Name.Trim();
                            if (Workshop_Name.Length > 20)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行车间名称长度超过最大限定[20]", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库判重
                        var isDbRepeated = workshops.Exists(m => m.Workshop_ID == Workshop_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Building_Name == Building_Name && m.Floor_Name == Floor_Name && m.Workshop_Name == Workshop_Name);
                        if (isDbRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("车间[{0}]已经存在,不可重复导入", Workshop_Name);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = workshopList.Exists(m => m.Workshop_ID == Workshop_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Building_Name == Building_Name && m.Floor_Name == Floor_Name && m.Workshop_Name == Workshop_Name);
                        if (isSelfRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("车间[{0}]导入数据重复", Workshop_Name);
                            return errorInfo;
                        }

                        workshopDto.Plant_Organization_UID = Plant_Organization_UID;
                        workshopDto.BG_Organization_UID = BG_Organization_UID;
                        workshopDto.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        workshopDto.Workshop_ID = Workshop_ID;
                        workshopDto.Building_Name = Building_Name;
                        workshopDto.Floor_Name = Floor_Name;
                        workshopDto.Workshop_Name = Workshop_Name;
                        workshopDto.Is_Enable = Is_Enable == "1" ? true : false;
                        workshopDto.Created_UID = CurrentUser.AccountUId;
                        workshopDto.Created_Date = DateTime.Now;
                        workshopDto.Modified_UID = CurrentUser.AccountUId;
                        workshopDto.Modified_Date = DateTime.Now;

                        workshopList.Add(workshopDto);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(workshopList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertWorkshopAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入车间失败:" + e.ToString();
            }
            return errorInfo;
        }
        #endregion
        #region 制程维护
        public ActionResult QueryProcessInfos(Process_InfoModelSearch search, Page page)
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
            var apiUrl = string.Format("Settings/QueryProcessInfosAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DoExportWorkshopByQuery(WorkshopModelSearch search)
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

            //get Export datas
            var apiUrl = "Fixture/GetWorkshopListByQueryAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<WorkshopDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Workshop Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "车间代码", "楼栋名称", "楼层名称", "楼层名称", "是否启用", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Workshop Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BGName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Workshop_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Building_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Floor_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Workshop_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Enable ? "1" : "0";
                    worksheet.Cells[index + 2, 10].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public string ImportProcessInfo(HttpPostedFileBase uploadName)
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
                        "制程代码",
                        "制程名称",
                        "制程描述",
                        "是否启用"
                    };
                    bool excelIsError = false;
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        if (worksheet.Cells[1, i].Value != null && !string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Value.ToString()))
                        {
                            var result = worksheet.Cells[1, i].Value.ToString();
                            var hasItem = propertiesHead.FirstOrDefault(m => m.Contains(result));
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
                    //获得厂区
                    var plantsAPI = string.Format("Fixture/QueryAllPlantAPI?PLANT_UID={0}&leval={1}", GetPlantOrgUid(), "1");
                    HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(plantsAPI);
                    var jsonPlants = plantsmessage.Content.ReadAsStringAsync().Result;
                    var plants = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonPlants);

                    //获取所有制程原因
                    var fixtureDefectCodeAPI = "Fixture/QueryAllProcessInfosAPI";
                    HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeAPI);
                    var jsonsVendorInfo = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                    var processes = JsonConvert.DeserializeObject<List<Process_InfoDTO>>(jsonsVendorInfo);

                    var processList = new List<Process_InfoDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var workshopDto = new Process_InfoDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                        var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                        var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            excelIsError = true;
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
                                excelIsError = true;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        string Process_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程代码")].Value);
                        if (string.IsNullOrWhiteSpace(Process_ID))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行制程代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Process_ID = Process_ID.Trim();
                            if (Process_ID.Length > 10)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行制程代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                        }

                        string Process_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程名称")].Value);
                        if (string.IsNullOrWhiteSpace(Process_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行制程名称没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Process_Name = Process_Name.Trim();
                            if (Process_Name.Length > 30)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行制程名称长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                        }

                        string Process_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程描述")].Value);
                        if (!string.IsNullOrWhiteSpace(Process_Name))
                        {
                            Process_Desc = Process_Desc.Trim();
                            if (Process_Name.Length > 100)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行制程描述长度超过最大限定[100]", i);
                                return errorInfo;
                            }
                        }

                        string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(Is_Enable))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行是否启用没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            Is_Enable = Is_Enable.Trim();
                            if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                        }

                        //数据库查重
                        var isDbRepeated = processes.Exists(m => m.Process_ID == Process_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isDbRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行制程[{1}]已经存在,不可重复导入", i, Process_ID);
                            return errorInfo;
                        }
                        //Excel 查重
                        var isSelfRepeated = processList.Exists(m => m.Process_ID == Process_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                        if (isSelfRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行制程[{1}]在导入数据中重复", i, Process_ID);
                            return errorInfo;
                        }

                        workshopDto.Plant_Organization_UID = Plant_Organization_UID;
                        workshopDto.BG_Organization_UID = BG_Organization_UID;
                        workshopDto.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        workshopDto.Process_ID = Process_ID;
                        workshopDto.Process_Name = Process_Name;
                        workshopDto.Process_Desc = Process_Desc;
                        workshopDto.Is_Enable = Is_Enable == "1" ? true : false;
                        workshopDto.Created_UID = CurrentUser.AccountUId;
                        workshopDto.Created_Date = DateTime.Now;
                        workshopDto.Modified_UID = CurrentUser.AccountUId;
                        workshopDto.Modified_Date = DateTime.Now;

                        processList.Add(workshopDto);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(processList);
                    var apiInsertVendorInfoUrl = string.Format("Fixture/InsertProcessInfosAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiInsertVendorInfoUrl);
                    errorInfo = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入制程失败:" + e.ToString();
            }
            return errorInfo;
        }
        #endregion
        #region  治具维修
        public ActionResult AddFixtureRepair(Fixture_Repair_MDTO fixtureRepairM)//, List<Fixture_Repair_DDTO> fixtureRepairDList)
        {
            //FixtureDefectCode_SettingDTO dto = new FixtureDefectCode_SettingDTO();
            fixtureRepairM.Created_UID = CurrentUser.AccountUId;
            fixtureRepairM.Created_Date = DateTime.Now;
            fixtureRepairM.Modified_UID = CurrentUser.AccountUId;
            fixtureRepairM.Modified_Date = DateTime.Now;
            if (fixtureRepairM.FunPlant_Organization_UID == 0)
            {
                fixtureRepairM.FunPlant_Organization_UID = null;
            }

            foreach (var item in fixtureRepairM.Fixture_Repair_DDTOList)
            {
                item.Created_UID = CurrentUser.AccountUId;
                item.Created_Date = DateTime.Now;
                item.Modified_UID = CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
                foreach (var subItem in item.Fixture_Repair_D_DefectDTOList)
                {
                    subItem.Created_UID = CurrentUser.AccountUId;
                    subItem.Created_Date = DateTime.Now;
                    subItem.Modified_UID = CurrentUser.AccountUId;
                    subItem.Modified_Date = DateTime.Now;
                }
            }
            //dto.Plant_Organization_UID = Plant_Organization_UID;
            //dto.BG_Organization_UID = BG_Organization_UID;
            //if (FunPlant_Organization_UID != 0)
            //{
            //    dto.FunPlant_Organization_UID = FunPlant_Organization_UID;
            //}
            //dto.Fixture_NO = Fixture_NO;
            //dto.Fixture_Defect_UIDs = Fixture_DefectCodeDTOs;
            var apiUrl = string.Format("Fixture/AddFixtureRepairAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(fixtureRepairM, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        //编辑治具维修
        public ActionResult EditFixtureRepair(Fixture_Repair_MDTO fixtureRepairM)
        {
            fixtureRepairM.Modified_UID = CurrentUser.AccountUId;
            fixtureRepairM.Modified_Date = DateTime.Now;
            if (fixtureRepairM.FunPlant_Organization_UID == 0)
            {
                fixtureRepairM.FunPlant_Organization_UID = null;
            }

            foreach (var item in fixtureRepairM.Fixture_Repair_DDTOList)
            {
                item.Modified_UID = CurrentUser.AccountUId;
                item.Modified_Date = DateTime.Now;
                if (!(item.Fixture_Repair_D_UID.HasValue && item.Fixture_Repair_D_UID.Value != 0))
                {
                    item.Created_UID = CurrentUser.AccountUId;
                    item.Created_Date = DateTime.Now;
                }
                foreach (var subItem in item.Fixture_Repair_D_DefectDTOList)
                {
                    subItem.Modified_UID = CurrentUser.AccountUId;
                    subItem.Modified_Date = DateTime.Now;
                    if (!(subItem.Fixture_Repair_D_Defect_UID.HasValue && subItem.Fixture_Repair_D_Defect_UID != 0))
                    {
                        subItem.Created_UID = CurrentUser.AccountUId;
                        subItem.Created_Date = DateTime.Now;
                    }
                }
            }
            var apiUrl = string.Format("Fixture/EditFixtureRepairAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(fixtureRepairM, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureRepairByRepairNo(string repairNo)
        {
            var apiUrl = string.Format("Fixture/GetFixtureRepairByRepairNoAPI/?repairNo={0}", repairNo);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(result))
            {
                return Content("");
            }
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureListFixtureRepair(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name)
        {
            var apiUrl = string.Format("Fixture/GetFixtureListFixtureRepairAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}&Line_ID={3}&Line_Name={4}&Machine_ID={5}&Machine_Name={6}&Process_ID={7}&Process_Name={8}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Line_ID, Line_Name, Machine_ID, Machine_Name, Process_ID, Process_Name);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(result))
            {
                return Content("");
            }
            return Content(result, "application/json");
        }

        /// <summary>
        /// 根据唯一编码模糊查询治具
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        public ActionResult GetFixtureListFixtureRepairByUniqueID(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string UniqueID)
        {
            var apiUrl = string.Format("Fixture/GetFixtureListFixtureRepairByUniqueIDAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}&UniqueID={3}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, UniqueID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(result))
            {
                return Content("");
            }
            return Content(result, "application/json");
        }
        public ActionResult GetSystemUserByNTId(string NTID)
        {
            var apiUrl = string.Format("Common/GetSystemUserByNTId/?ntid={0}&islogin=0", NTID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //var result = responMessage.StatusCode == HttpStatusCode.NotFound ? "null"
            //                : responMessage.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(result))
            {
                return Content("");
            }
            return Content(result, "application/json");
        }
        public ActionResult GetSentRepairNameById(string SentOut_Number)
        {
            var apiUrl = string.Format("Fixture/GetSentRepairNameByIdAPI/?SentOut_Number={0}", SentOut_Number);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(result))
            {
                return Content("");
            }
            return Content(result, "application/json");
        }
        public ActionResult GetDefectCodeReapairSolutionList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetDefectCodeReapairSolutionListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 維修單新增時，維修狀態只有(3.維修中In-Repair;4.報廢Scrap;5:返供應商維修RTV)三種可以選取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFixtureStatusListWhenAdd()
        {
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOListWhenAddAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureByFixtureUniqueID(string fixtureUniqueID)
        {
            var apiUrl = string.Format("Fixture/GetFixtureByFixtureUniqueIDAPI/?fixtureUniqueID={0}", fixtureUniqueID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(result))
            {
                return Content("");
            }
            return Content(result, "application/json");
        }
        public ActionResult fixtureRepair()
        {

            FixtureRepairVM currentVM = new FixtureRepairVM();
            //获取当前用户信息，前端接收者默认带出来
            currentVM.Account_UID = CurrentUser.AccountUId;
            currentVM.User_NTID = CurrentUser.GetUserInfo.User_NTID;
            currentVM.User_Name = CurrentUser.UserName;

            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
            //ViewBag.PageTitle = "治具维修";
            return View("fixtureRepair", currentVM);

        }
        public ActionResult GetFixtureStatusList()
        {
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DoExportFixtureRepair(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportFixtureRepairAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureRepairDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("WorkStation Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "表单编号", "厂区", "OP类型", "功能厂", "制程", "工站", "线别", "设备编号", "治具编号", "治具短码", "治具状态", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("WorkStation Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Repair_NO;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Process_Info;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Production_Line_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Equipment_No;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.StatusName;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportFixtureRepairByQuery(FixtureRepairSearch search)
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
            //get Export datas
            var apiUrl = "Fixture/GetFixtureRepairListAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureRepairDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("WorkStation Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "表单编号", "厂区", "OP类型", "功能厂", "制程", "线别", "设备编号", "治具编号", "治具短码", "治具唯一编码", "治具状态", "修改人", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("WorkStation Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Repair_NO;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Process_Info;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Production_Line_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Equipment_No;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.ShortCode;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Fixture_Unique_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.StatusName;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 初始化查询治具保养数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryFixtureRepairs(FixtureRepairSearch search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureRepairAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetRepairLoactionList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetRepairLocationListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult ValidFixtureRepairStatus(int fixtureMUID)
        {
            var apiUrl = string.Format("FixtureRepairD/ValidFixtureRepairStatusDAPI?uid={0}", fixtureMUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
        #endregion
        #region 异常原因群组设定
        public ActionResult FixtureAbnormalReasonGroup()
        {

            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
            //ViewBag.PageTitle = "治具异常群组设定维护";
            return View("FixtureAbnormalReasonGroup", currentVM);

        }
        /// <summary>
        /// 初始化查询异常原因群组
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryDefectCode_Group(DefectCode_GroupDTO search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryDefectCode_GroupAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 导出治具资料
        /// </summary>
        /// <param name="DefectCode_Group_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoAllExportDefectCode_GroupReprot(DefectCode_GroupDTO search)
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
            var apiUrl = string.Format("Fixture/DoAllExportDefectCode_GroupReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<DefectCode_GroupDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("DefectCode Group");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "异常群组代码", "异常群组名称", "异常代码", "异常名称", "是否启用", "最后更新者", "最后更新日期" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("DefectCode Group");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.DefectCode_Group_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.DefectCode_Group_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.DefectCode_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.DefectCode_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifieder;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 11; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:K1"].Style.Font.Bold = true;
                worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:K1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:K1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 导出治具资料
        /// </summary>
        /// <param name="DefectCode_Group_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoExportDefectCode_GroupReprot(string DefectCode_Group_UIDs)
        {


            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportDefectCode_GroupReprotAPI?DefectCode_Group_UIDs={0}", DefectCode_Group_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<DefectCode_GroupDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("DefectCode Group");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "异常群组代码", "异常群组名称", "异常代码", "异常名称", "是否启用", "最后更新者", "最后更新日期" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("DefectCode Group");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.DefectCode_Group_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.DefectCode_Group_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.DefectCode_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.DefectCode_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifieder;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 11; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:K1"].Style.Font.Bold = true;
                worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:K1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:K1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public FileResult DefectCode_Group()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "DefectCode_Group.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string AddDefectCode_Group(List<Fixture_DefectCodeDTO> Fixture_DefectCodeDTOs, int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID, string DefectCode_Group_Name)
        {

            DefectCode_GroupDTO dto = new DefectCode_GroupDTO();
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            dto.Plant_Organization_UID = Plant_Organization_UID;
            dto.BG_Organization_UID = BG_Organization_UID;
            if (FunPlant_Organization_UID != 0)
            {
                dto.FunPlant_Organization_UID = FunPlant_Organization_UID;
            }
            dto.DefectCode_Group_ID = DefectCode_Group_ID.Trim();
            dto.DefectCode_Group_Name = DefectCode_Group_Name.Trim();
            dto.Fixture_Defect_UIDs = Fixture_DefectCodeDTOs;
            var apiUrl = string.Format("Fixture/AddDefectCode_GroupAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;


        }
        public ActionResult DefectCode_GroupList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/DefectCode_GroupListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 导入治具异常原因配置导入
        /// </summary>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public string ImportDefectCode_Group(HttpPostedFileBase uploadName)
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "异常群组代码",
                        "异常群组名称",
                        "异常代码",
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


                        int BGorganization_UID = 0;
                        if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                        {
                            BGorganization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                        }
                        var apiUrlGetOPType = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", plant_OrganizationUID, BGorganization_UID);
                        HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrlGetOPType);
                        var OPTypeResult = responMessage.Content.ReadAsStringAsync().Result;
                        List<SystemProjectDTO> systemOPtypes = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(OPTypeResult);

                        //根据厂区获取所有异常群组数据
                        var defectCode_GroupListAPI = string.Format("Fixture/DefectCode_GroupListAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage defectCode_GroupMessage = APIHelper.APIGetAsync(defectCode_GroupListAPI);
                        var jsonsDefectCode_Groups = defectCode_GroupMessage.Content.ReadAsStringAsync().Result;
                        var defectCode_Groups = JsonConvert.DeserializeObject<List<DefectCode_GroupDTO>>(jsonsDefectCode_Groups);

                        //根据厂区获取所有异常原因。
                        var fixtureDefectCodeByPlantAPI = string.Format("Fixture/GetDefectCodesByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeByPlantAPI);
                        var jsonsDefectCodes = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                        var fixtureDefectCodes = JsonConvert.DeserializeObject<List<Fixture_DefectCodeDTO>>(jsonsDefectCodes);


                        var defectCode_GroupList = new List<DefectCode_GroupDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var defectCode_Group = new DefectCode_GroupDTO();
                            int Plant_Organization_UID = 0;
                            int BG_Organization_UID = 0;
                            int? FunPlant_Organization_UID = null;
                            var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                            var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                            var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);


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
                                var bg = systemOPtypes.Where(m => m.OP_TYPES == BG_Organization).FirstOrDefault();
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
                            string DefectCode_Group_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常群组代码")].Value);
                            if (string.IsNullOrWhiteSpace(DefectCode_Group_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常群组代码没有值", i);
                                return errorInfo;
                            }
                            else if (DefectCode_Group_ID.Length > 10)
                            {
                                DefectCode_Group_ID = DefectCode_Group_ID.Trim();
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常群组代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }

                            string DefectCode_Group_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常群组名称")].Value);
                            if (string.IsNullOrWhiteSpace(DefectCode_Group_Name))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常群组名称没有值", i);
                                return errorInfo;
                            }
                            else if (DefectCode_Group_Name.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常群组名称长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                            DefectCode_Group_Name = DefectCode_Group_Name.Trim();
                            string defectCodeId = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常代码")].Value);
                            if (string.IsNullOrWhiteSpace(defectCodeId))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常代码没有值", i);
                                return errorInfo;
                            }
                            else if (defectCodeId.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            defectCodeId = defectCodeId.Trim();
                            string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                            if (string.IsNullOrWhiteSpace(Is_Enable))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用没有值", i);
                                return errorInfo;
                            }
                            else if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                            Is_Enable = Is_Enable.Trim();
                            int Fixture_Defect_UID = 0;
                            var hasitem = fixtureDefectCodes.FirstOrDefault(m => m.DefectCode_ID == defectCodeId && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (hasitem == null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有此异常代码[{1}]", i, defectCodeId);
                                return errorInfo;
                            }
                            else
                            {
                                Fixture_Defect_UID = hasitem.Fixture_Defect_UID;
                            }

                            var hasDefectCode = defectCode_Groups.FirstOrDefault(m => m.Fixtrue_Defect_UID == Fixture_Defect_UID && m.DefectCode_Group_ID == DefectCode_Group_ID && m.DefectCode_Group_Name == DefectCode_Group_Name && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (hasDefectCode != null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常群组代码[{1}]异常群组名称[{3}]，异常代码[{2}]已经存在,不可重复导入", i, DefectCode_Group_ID, defectCodeId, DefectCode_Group_Name);
                                return errorInfo;
                            }
                            //导入数据判重
                            var isSelfRepeated = defectCode_GroupList.Exists(m => m.Fixtrue_Defect_UID == Fixture_Defect_UID && m.DefectCode_Group_ID == DefectCode_Group_ID && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常原因[{1}]异常群组编号[{2}]在导入数据中重复", i, defectCodeId, DefectCode_Group_ID);
                                return errorInfo;
                            }
                            defectCode_Group.Plant_Organization_UID = Plant_Organization_UID;
                            defectCode_Group.BG_Organization_UID = BG_Organization_UID;
                            defectCode_Group.FunPlant_Organization_UID = FunPlant_Organization_UID;
                            defectCode_Group.DefectCode_ID = defectCodeId;
                            defectCode_Group.DefectCode_Group_ID = DefectCode_Group_ID;
                            defectCode_Group.DefectCode_Group_Name = DefectCode_Group_Name;
                            defectCode_Group.Fixtrue_Defect_UID = Fixture_Defect_UID;
                            defectCode_Group.Is_Enable = Is_Enable == "1" ? true : false;
                            defectCode_Group.Created_UID = CurrentUser.AccountUId;
                            defectCode_Group.Modified_UID = CurrentUser.AccountUId;

                            defectCode_GroupList.Add(defectCode_Group);
                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(defectCode_GroupList);
                        var apiInsertDefectCode_GroupUrl = string.Format("Fixture/InsertDefectCode_GroupAPI");
                        HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertDefectCode_GroupUrl);
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


        }
        /// <summary>
        /// 根据所选择的治具ID删除治具分组信息
        /// </summary>
        /// <param name="DefectCode_Group_UIDs"></param>
        /// <returns></returns>
        public string DeleteDefectCode_GroupByID(string DefectCode_Group_UIDs)
        {

            var apiUrl = string.Format("Fixture/DeleteDefectCode_Group_UIDAPI?DefectCode_Group_UIDs={0}", DefectCode_Group_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        #endregion 异常原因群组设定
        #region 治具异常原因维护
        /// <summary>
        /// 根据所选择的治具ID删除治具分组信息
        /// </summary>
        /// <param name="DefectCode_Group_UIDs"></param>
        /// <returns></returns>
        public string DeleteFixtureDefectCode_SettingByID(string FixtureDefectCode_Setting_UIDs)
        {

            var apiUrl = string.Format("Fixture/DeleteFixtureDefectCode_Setting_UIDAPI?FixtureDefectCode_Setting_UIDs={0}", FixtureDefectCode_Setting_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        public ActionResult GetDefectCode_GroupByUID(int DefectCode_Group_UID)
        {
            var apiUrl = string.Format("Fixture/GetDefectCode_GroupByUIDAPI?DefectCode_Group_UID={0}", DefectCode_Group_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetFixtureDefectCode_SettingDTOByUID(int FixtureDefectCode_Setting_UID)
        {
            var apiUrl = string.Format("Fixture/GetFixtureDefectCode_SettingDTOByUIDAPI?FixtureDefectCode_Setting_UID={0}", FixtureDefectCode_Setting_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 治具异常原因设定维护
        /// </summary>
        /// <returns></returns>
        public ActionResult FixtureAbnormalReason()
        {

            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
            //ViewBag.PageTitle = "治具异常原因设定维护";
            return View("FixtureAbnormalReason", currentVM);

        }
        /// <summary>
        /// 初始化查询治具异常原因
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult QueryFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO search, Page page)
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
            var apiUrl = string.Format("Fixture/QueryFixtureDefectCode_SettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="DefectCode_Group_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoAllExportFixtureDefectCode_SettingReprot(FixtureDefectCode_SettingDTO search)
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
            //get Export datas
            var apiUrl = "Fixture/DoAllExportFixtureDefectCode_SettingReprotAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDefectCode_SettingDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("DefectCode Group");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具编号", "异常代码", "异常名称", "是否启用", "最后更新者", "最后更新日期" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("DefectCode Group");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.DefectCode_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.DefectCode_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modifieder;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 11; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:J1"].Style.Font.Bold = true;
                worksheet.Cells["A1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="DefectCode_Group_UIDs"></param>
        /// <returns></returns>
        public ActionResult DoExportFixtureDefectCode_SettingReprot(string FixtureDefectCode_Setting_UIDs)
        {

            //get Export datas
            var apiUrl = string.Format("Fixture/DoExportFixtureDefectCode_SettingReprotAPI?FixtureDefectCode_Setting_UIDs={0}", FixtureDefectCode_Setting_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FixtureDefectCode_SettingDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("DefectCode Group");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "治具编号", "异常代码", "异常名称", "是否启用", "最后更新者", "最后更新日期" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("DefectCode Group");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.OPName;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlantName;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Fixture_NO;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.DefectCode_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.DefectCode_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modifieder;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 11; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:J1"].Style.Font.Bold = true;
                worksheet.Cells["A1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public FileResult FixtureDefectCode_Setting()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "FixtureDefectCode_Setting.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        public string AddFixtureDefectCode_Setting(List<Fixture_DefectCodeDTO> Fixture_DefectCodeDTOs, int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Fixture_NO)
        {
            FixtureDefectCode_SettingDTO dto = new FixtureDefectCode_SettingDTO();
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            dto.Plant_Organization_UID = Plant_Organization_UID;
            dto.BG_Organization_UID = BG_Organization_UID;
            if (FunPlant_Organization_UID != 0)
            {
                dto.FunPlant_Organization_UID = FunPlant_Organization_UID;
            }
            dto.Fixture_NO = Fixture_NO;
            dto.Fixture_Defect_UIDs = Fixture_DefectCodeDTOs;
            var apiUrl = string.Format("Fixture/AddFixtureDefectCode_SettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        public ActionResult GetDefectCodeByGroup(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID)
        {
            var apiUrl = string.Format("Fixture/GetDefectCodeByGroupAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}&DefectCode_Group_ID={3}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, DefectCode_Group_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetDefectCodeGroup(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID)
        {
            var apiUrl = string.Format("Fixture/GetDefectCodeGroupAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}&DefectCode_Group_ID={3}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, DefectCode_Group_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 导入治具异常原因配置导入
        /// </summary>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public string ImportFixtureDefectCode_Setting(HttpPostedFileBase uploadName)
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
                        "厂区代号",
                        "OP代号",
                        "功能厂代号",
                        "治具编号",
                        "异常代码",
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

                        int BGorganization_UID = 0;
                        if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
                        {
                            BGorganization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                        }
                        var apiUrlGetOPType = string.Format("Fixture/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", plant_OrganizationUID, BGorganization_UID);
                        HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrlGetOPType);
                        var OPTypeResult = responMessage.Content.ReadAsStringAsync().Result;
                        List<SystemProjectDTO> systemOPtypes = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(OPTypeResult);

                        //根据厂区获取所有异常原因配置数据
                        var fixtureDefectCode_SettingByPlantAPI = string.Format("Fixture/GetFixtureDefectCode_SettingByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixtureDefectCode_SettingBMessage = APIHelper.APIGetAsync(fixtureDefectCode_SettingByPlantAPI);
                        var jsonsDefectCode_Settings = fixtureDefectCode_SettingBMessage.Content.ReadAsStringAsync().Result;
                        var fixtureDefectCode_Settings = JsonConvert.DeserializeObject<List<FixtureDefectCode_SettingDTO>>(jsonsDefectCode_Settings);

                        //根据厂区获取所有异常原因。
                        var fixtureDefectCodeByPlantAPI = string.Format("Fixture/GetDefectCodesByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixtureDefectCodeMessage = APIHelper.APIGetAsync(fixtureDefectCodeByPlantAPI);
                        var jsonsDefectCodes = fixtureDefectCodeMessage.Content.ReadAsStringAsync().Result;
                        var fixtureDefectCodes = JsonConvert.DeserializeObject<List<Fixture_DefectCodeDTO>>(jsonsDefectCodes);

                        //根据厂区获取异常主档资料
                        var fixture_MByPlantAPI = string.Format("Fixture/GetFixture_MByPlantAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
                        HttpResponseMessage fixture_MMessage = APIHelper.APIGetAsync(fixture_MByPlantAPI);
                        var jsonsfixture_Ms = fixture_MMessage.Content.ReadAsStringAsync().Result;
                        var fixture_Ms = JsonConvert.DeserializeObject<List<FixtureDTO>>(jsonsfixture_Ms);


                        var fixtureDefectCode_SettingList = new List<FixtureDefectCode_SettingDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var fixtureDefectCode_Setting = new FixtureDefectCode_SettingDTO();
                            int Plant_Organization_UID = 0;
                            int BG_Organization_UID = 0;
                            int? FunPlant_Organization_UID = null;
                            var Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区代号")].Value);
                            var BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP代号")].Value);
                            var FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂代号")].Value);
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

                                var bg = systemOPtypes.Where(m => m.OP_TYPES == BG_Organization).FirstOrDefault();

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
                            string defectCodeId = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常代码")].Value);
                            if (string.IsNullOrWhiteSpace(defectCodeId))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常代码没有值", i);
                                return errorInfo;
                            }
                            else if (defectCodeId.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常代码长度超过最大限定[10]", i);
                                return errorInfo;
                            }
                            defectCodeId = defectCodeId.Trim();
                            string Fixture_NO = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "治具编号")].Value);
                            if (string.IsNullOrWhiteSpace(Fixture_NO))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编号没有值", i);
                                return errorInfo;
                            }
                            else if (Fixture_NO.Length > 30)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行治具编号长度超过最大限定[30]", i);
                                return errorInfo;
                            }
                            Fixture_NO = Fixture_NO.Trim();
                            string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                            if (string.IsNullOrWhiteSpace(Is_Enable))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用没有值", i);
                                return errorInfo;
                            }
                            else if (Is_Enable != "0" && Is_Enable != "1")
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行是否启用值填写错误,请填写0(禁用)或1(启用)", i);
                                return errorInfo;
                            }
                            Is_Enable = Is_Enable.Trim();
                            var hasfixture_Mitem = fixture_Ms.FirstOrDefault(m => m.Fixture_NO == Fixture_NO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (hasfixture_Mitem == null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有此治具编号[{1}]", i, Fixture_NO);
                                return errorInfo;
                            }

                            int Fixture_Defect_UID = 0;
                            var hasitem = fixtureDefectCodes.FirstOrDefault(m => m.DefectCode_ID == defectCodeId && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (hasitem == null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有此异常代码[{1}]", i, defectCodeId);
                                return errorInfo;
                            }
                            else
                            {
                                Fixture_Defect_UID = hasitem.Fixture_Defect_UID;
                            }

                            var hasDefectCode = fixtureDefectCode_Settings.FirstOrDefault(m => m.DefectCode_ID == defectCodeId && m.Fixture_NO == Fixture_NO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID);
                            if (hasDefectCode != null)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常[{1}]已经存在,不可重复导入", i, defectCodeId);
                                return errorInfo;
                            }
                            //导入数据判重
                            var isSelfRepeated = fixtureDefectCode_SettingList.Exists(m => m.Fixture_Defect_UID == Fixture_Defect_UID && m.Fixture_NO == Fixture_NO && m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行异常原因[{1}]治具编号[{2}]在导入数据中重复", i, defectCodeId, Fixture_NO);
                                return errorInfo;
                            }
                            fixtureDefectCode_Setting.Plant_Organization_UID = Plant_Organization_UID;
                            fixtureDefectCode_Setting.BG_Organization_UID = BG_Organization_UID;
                            fixtureDefectCode_Setting.FunPlant_Organization_UID = FunPlant_Organization_UID;
                            fixtureDefectCode_Setting.DefectCode_ID = defectCodeId;
                            fixtureDefectCode_Setting.Fixture_NO = Fixture_NO;
                            fixtureDefectCode_Setting.Fixture_Defect_UID = Fixture_Defect_UID;
                            fixtureDefectCode_Setting.Is_Enable = Is_Enable == "1" ? true : false;
                            fixtureDefectCode_Setting.Created_UID = CurrentUser.AccountUId;
                            fixtureDefectCode_Setting.Modified_UID = CurrentUser.AccountUId;

                            fixtureDefectCode_SettingList.Add(fixtureDefectCode_Setting);
                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(fixtureDefectCode_SettingList);
                        var apiInsertfixtureDefectCode_SettingUrl = string.Format("Fixture/InsertFixtureDefectCode_SettingAPI");
                        HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertfixtureDefectCode_SettingUrl);
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

        }
        public string EditDefectCode_Group(DefectCode_GroupDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;

            var apiUrl = string.Format("Fixture/EditDefectCode_GroupAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        public string EditFixtureDefectCode_Setting(FixtureDefectCode_SettingDTO dto)
        {
            dto.Created_UID = CurrentUser.AccountUId;
            dto.Created_Date = DateTime.Now;
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = string.Format("Fixture/EditFixtureDefectCode_SettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        #endregion 治具
        #region 报表-治具维修次查询 Add by Rock 2017-10-30------------------------ Start
        public ActionResult FixtureReportByRepair()
        {
            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
        public ActionResult QueryFixtureReportByRepair(ReportByRepair search, Page page)
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
            var url = string.Format("Fixture/QueryReportByRepairAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByRepairValid(ReportByRepair search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByRepairAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(result);
            return result;
        }
        public ActionResult ExportReportByRepair(ReportByRepair search)
        {
            var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(search.JsonValue);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Repair Report");
            string[] propertiesHeadOne = new string[] { };
            string[] propertiesHeadTwo = new string[] { };
            propertiesHeadOne = FlowchartImportCommon.GetFixtureRepairReportHeadColumnOne();
            propertiesHeadTwo = FlowchartImportCommon.GetFixtureRepairReportHeadColumnTwo();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var listOne = list.Where(m => m.sheetCount == 1).ToList();
                var listTwo = list.Where(m => m.sheetCount == 2).ToList();

                if (listOne.Count() > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Repair Report(By Reason)");
                    SetRepairOneExcelStyle(worksheet, propertiesHeadOne);
                    int iRow = 2;
                    foreach (var item in listOne)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.Fixture_NO;
                        worksheet.Cells[iRow, 6].Value = item.WorkStation_Name;
                        worksheet.Cells[iRow, 7].Value = item.DefectCode_ID;
                        worksheet.Cells[iRow, 8].Value = item.DefectCode_Name;
                        worksheet.Cells[iRow, 9].Value = item.TotalCount;
                        iRow++;
                    }
                }

                if (listTwo.Count() > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Repair Report(By DateAndWorkStation)");
                    SetRepairTwoExcelStyle(worksheet, propertiesHeadTwo);
                    int iRow = 2;
                    foreach (var item in listTwo)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.Fixture_NO;
                        worksheet.Cells[iRow, 6].Value = item.SentOut_Date;
                        worksheet.Cells[iRow, 7].Value = item.WorkStation_Name;
                        worksheet.Cells[iRow, 8].Value = item.TotalCount;
                        iRow++;
                    }
                    var dateColorRange = string.Format("F2:F{0}", iRow - 1);
                    worksheet.Cells[dateColorRange].Style.Numberformat.Format = FormatConstants.DateTimeFormatStringByDate;
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetRepairOneExcelStyle(ExcelWorksheet worksheet, string[] propertiesHeadOne)
        {
            for (int colIndex = 0; colIndex < propertiesHeadOne.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHeadOne[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
                worksheet.Cells["A1:I1"].Style.Font.Bold = true;
                worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);

            }
        }
        private void SetRepairTwoExcelStyle(ExcelWorksheet worksheet, string[] propertiesHeadTwo)
        {
            for (int colIndex = 0; colIndex < propertiesHeadTwo.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHeadTwo[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);

            }
        }
        #endregion 报表-治具维修次查询Add by Rock 2017-10-30------------------------ End
        #region 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ Start
        public ActionResult FixtureReportByRepairPerson()
        {
            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
        public ActionResult QueryFixtureReportByRepairPerson(ReportByRepair search, Page page)
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
            var url = string.Format("Fixture/QueryReportByRepairPersonAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByRepairPersonValid(ReportByRepair search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByRepairPersonValidAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(result);
            return result;
        }
        public ActionResult ExportReportByRepairPerson(ReportByRepair search)
        {
            var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(search.JsonValue);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Repair Report By Person");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureRepairReportHeadByPersonColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                if (list.Count() > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Repair Report(By Reason)");
                    SetRepairPersonExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.WorkStation_Name;
                        worksheet.Cells[iRow, 6].Value = item.DefectCode_ID;
                        worksheet.Cells[iRow, 7].Value = item.DefectCode_Name;
                        worksheet.Cells[iRow, 8].Value = item.RepairName;
                        worksheet.Cells[iRow, 9].Value = item.TotalCount;
                        iRow++;
                    }
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        private void SetRepairPersonExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
                worksheet.Cells["A1:I1"].Style.Font.Bold = true;
                worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);

            }
        }
        #endregion 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ End
        #region 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ Start
        public ActionResult FixtureReportByPage()
        {
            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
        public ActionResult QueryFixtureReportByPage(ReportByRepair search, Page page)
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
            var url = string.Format("Fixture/QueryReportByPageAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByPageValid(ReportByRepair search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByPageValidAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public ActionResult ExportReportByPage(ReportByRepair search)
        {
            var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(search.JsonValue);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Fixture Repair Report By Page");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureHeadByPageColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                if (list.Count() > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Repair Report(By Page)");
                    SetPageExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.Fixture_NO;
                        worksheet.Cells[iRow, 6].Value = item.Version;
                        worksheet.Cells[iRow, 7].Value = item.TotalCount;
                        iRow++;
                    }
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        private void SetPageExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
        }
        #endregion 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ End
        #region 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ Start
        public ActionResult FixtureReportByDetail()
        {
            var vm = GetFixtureStatus();
            return View(vm);
        }
        public ActionResult QueryFixtureReportByDetail(ReportByRepair search, Page page)
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
            var url = string.Format("Fixture/QueryFixtureReportByDetailAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByDetailValid(ReportByRepair search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByDetailValidAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public ActionResult ExportReportByDetail(ReportByRepair search)
        {
            var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(search.JsonValue);
            var stream = new MemoryStream();

            var fileName = PathHelper.SetGridExportExcelName("Fixture Repair Report Detail");
            string[] propertiesHead = new string[] { };
            if (search.hidType == 1)
            {
                propertiesHead = FlowchartImportCommon.GetFixtureRepairReportByDetailColumnOne();
            }
            else
            {
                propertiesHead = FlowchartImportCommon.GetFixtureRepairReportByDetailColumnTwo();
            }

            using (var excelPackage = new ExcelPackage(stream))
            {
                int iRow = 2;
                ExcelWorksheet worksheet = null;
                if (search.hidType == 1)
                {
                    worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Repair Report Detail(By Person)");
                    SetRepairDetailExcelStyle(worksheet, propertiesHead, search.hidType);
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.WorkStation_Name;
                        worksheet.Cells[iRow, 6].Value = item.Fixture_Unique_ID;
                        worksheet.Cells[iRow, 7].Value = item.RepairName;
                        worksheet.Cells[iRow, 8].Value = item.TotalCount;
                        iRow++;
                    }

                }
                else
                {
                    worksheet = excelPackage.Workbook.Worksheets.Add("Fixture Repair Report Detail(By Reason)");
                    SetRepairDetailExcelStyle(worksheet, propertiesHead, search.hidType);
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.WorkStation_Name;
                        worksheet.Cells[iRow, 6].Value = item.Fixture_Unique_ID;
                        worksheet.Cells[iRow, 7].Value = item.DefectCode_ID;
                        worksheet.Cells[iRow, 8].Value = item.DefectCode_Name;
                        worksheet.Cells[iRow, 9].Value = item.TotalCount;
                        iRow++;
                    }
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public void SetRepairDetailExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead, int hidType)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
            }

            if (hidType == 1)
            {
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);

            }
            else
            {
                worksheet.Cells["A1:I1"].Style.Font.Bold = true;
                worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
        }
        #endregion 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ End
        #region 报表-治具间维修时间分析报表 Add by Rock 2017-11-23----------------Start
        public ActionResult FixtureReportTimeAnalisis()
        {
            var vm = GetFixtureStatus();
            return View(vm);
        }
        public ActionResult QueryFixtureReportByAnalisis(ReportByRepair search, Page page)
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
            var url = string.Format("Fixture/QueryFixtureReportByAnalisisAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByAnalisisValid(ReportByRepair search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByAnalisisValidAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public ActionResult ExportReportByAnalisis(ReportByRepair search)
        {
            var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(search.JsonValue);
            var stream = new MemoryStream();

            var fileName = PathHelper.SetGridExportExcelName("治具间维修时间分析报表");
            string[] propertiesHead = FlowchartImportCommon.GetFixtureRepairReportByAnalisisColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                int iRow = 2;
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("治具间维修时间分析报表");
                SetRepairAnalisisExcelStyle(worksheet, propertiesHead);
                foreach (var item in list)
                {
                    worksheet.Cells[iRow, 1].Value = iRow - 1;
                    worksheet.Cells[iRow, 2].Value = item.PlantName;
                    worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                    worksheet.Cells[iRow, 4].Value = item.Func_Name;
                    worksheet.Cells[iRow, 5].Value = item.Repair_Location_Name;
                    worksheet.Cells[iRow, 6].Value = item.WorkStation_Name;
                    worksheet.Cells[iRow, 7].Value = item.Fixture_NO;
                    worksheet.Cells[iRow, 8].Value = item.LessHalfMinutes;
                    worksheet.Cells[iRow, 9].Value = item.LessTwoHour;
                    worksheet.Cells[iRow, 10].Value = item.LessFourHour;
                    worksheet.Cells[iRow, 11].Value = item.OtherHour;
                    iRow++;
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetRepairAnalisisExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
            }

            worksheet.Cells["A1:K1"].Style.Font.Bold = true;
            worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
        }
        #endregion 报表-治具间维修时间分析报表 Add by Rock 2017-11-23----------------End
        #region 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------Start
        public ActionResult FixtureReportByStatus()
        {
            var vm = GetFixtureStatus();
            return View(vm);
        }
        public ActionResult GetFixtureNoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("Fixture/GetFixtureNoListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureReportByStatus(ReportByRepair search, Page page)
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
            var url = string.Format("Fixture/QueryFixtureReportByStatusAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByStatusValid(ReportByRepair search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByStatusValidAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public ActionResult ExportReportByStatus(ReportByRepair search)
        {
            var list = JsonConvert.DeserializeObject<List<ReportByRepair>>(search.JsonValue);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("治具数量查询(治具状态)");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureHeadByStatusColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                if (list.Count() > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("治具数量查询(治具状态)");
                    SetStatusExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.WorkStation_Name;
                        worksheet.Cells[iRow, 6].Value = item.Fixture_NO;
                        worksheet.Cells[iRow, 7].Value = item.Fixture_Status;
                        worksheet.Cells[iRow, 8].Value = item.TotalCount;
                        iRow++;
                    }
                }
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        private void SetStatusExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 15;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
        }
        #endregion 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------End
        #region 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------Start
        public ActionResult FixtureReportByStatusAnalisis()
        {
            var vm = GetFixtureStatus();
            return View(vm);
        }
        public ActionResult QueryFixtureReportByStatusAnalisis(ReportByRepair search, Page page)
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
            var url = string.Format("Fixture/QueryFixtureReportByStatusAnalisisAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByStatusAnalisisValid(ReportByRepair search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByStatusAnalisisValidAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public ActionResult ExportReportByStatusAnalisis(ReportByRepair search)
        {
            var list = JsonConvert.DeserializeObject<List<ReportByStatusAnalisis>>(search.JsonValue);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("厂内治具状态分析表");
            string[] propertiesHead = new string[] { };
            propertiesHead = FlowchartImportCommon.GetFixtureHeadByStatusAnalisisColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                if (list.Count() > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("厂内治具状态分析表");
                    SetStatusAnalisisExcelStyle(worksheet, propertiesHead);
                    int iRow = 2;
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = iRow - 1;
                        worksheet.Cells[iRow, 2].Value = item.PlantName;
                        worksheet.Cells[iRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[iRow, 4].Value = item.Func_Name;
                        worksheet.Cells[iRow, 5].Value = item.WorkStation_Name;
                        if (item.TotalCount == -1)
                        {
                            worksheet.Cells[iRow, 6].Value = "-";

                            worksheet.Cells[iRow, 7].Formula = string.Format("=IF(F{0}=0,0,G{0}/$F${0})", iRow - 1); //item.StatusOne + "%";
                            worksheet.Cells[iRow, 7].Style.Numberformat.Format = "0.00%";

                            worksheet.Cells[iRow, 8].Formula = string.Format("=IF(F{0}=0,0,H{0}/$F${0})", iRow - 1); //item.StatusTwo + "%";
                            worksheet.Cells[iRow, 8].Style.Numberformat.Format = "0.00%";

                            worksheet.Cells[iRow, 9].Formula = string.Format("=IF(F{0}=0,0,I{0}/$F${0})", iRow - 1);//item.StatusThree + "%";
                            worksheet.Cells[iRow, 9].Style.Numberformat.Format = "0.00%";

                            worksheet.Cells[iRow, 10].Formula = string.Format("=IF(F{0}=0,0,J{0}/$F${0})", iRow - 1);//item.StatusFour + "%";
                            worksheet.Cells[iRow, 10].Style.Numberformat.Format = "0.00%";

                            worksheet.Cells[iRow, 11].Formula = string.Format("=IF(F{0}=0,0,K{0}/$F${0})", iRow - 1);//item.StatusFive + "%";
                            worksheet.Cells[iRow, 11].Style.Numberformat.Format = "0.00%";

                            worksheet.Cells[iRow, 12].Formula = string.Format("=IF(F{0}=0,0,L{0}/$F${0})", iRow - 1);//item.StatusSix + "%";
                            worksheet.Cells[iRow, 12].Style.Numberformat.Format = "0.00%";

                            worksheet.Cells[iRow, 13].Formula = string.Format("=IF(F{0}=0,0,M{0}/$F${0})", iRow - 1);//item.StatusSeven + "%";
                            worksheet.Cells[iRow, 13].Style.Numberformat.Format = "0.00%";

                            worksheet.Cells[iRow, 14].Formula = string.Format("=IF(F{0}=0,0,N{0}/$F${0})", iRow - 1);//item.StatusEight + "%";
                            worksheet.Cells[iRow, 14].Style.Numberformat.Format = "0.00%";

                        }
                        else
                        {
                            //治具总数
                            worksheet.Cells[iRow, 6].Formula = string.Format("=SUM(G{0}:L{0})", iRow);
                            //使用中
                            worksheet.Cells[iRow, 7].Value = item.StatusOne;
                            //保养逾期
                            worksheet.Cells[iRow, 8].Value = item.StatusSix;
                            //未使用
                            worksheet.Cells[iRow, 9].Value = item.StatusTwo;
                            //维修中
                            worksheet.Cells[iRow, 10].Value = item.StatusThree;
                            //返供应商RTV
                            worksheet.Cells[iRow, 11].Value = item.StatusFive;
                            //报废
                            worksheet.Cells[iRow, 12].Value = item.StatusFour;
                            //已保养
                            worksheet.Cells[iRow, 13].Value = item.StatusSeven;
                            //未保养
                            worksheet.Cells[iRow, 14].Value = item.StatusEight;
                        }
                        iRow++;
                    }

                    excelPackage.Save();
                }
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetStatusAnalisisExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 10;
                worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
            worksheet.Column(5).Width = 30;

        }

        #endregion 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------End
        #region 报表-FMT Dashboard Add by Rock 2017-12-11----------------Start
        public ActionResult FixtureReportByFMT()
        {
            var vm = GetFixtureStatus();
            return View(vm);
        }
        public ActionResult QueryFixtureReportByFMT(Batch_ReportByStatus search, Page page)
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
            var url = string.Format("Fixture/QueryFixtureReportByFMTAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFixtureReportByFMTDetail(int Process_Info_UID, string Time)
        {
            var timeList = Time.Split('~').ToList();
            var startDate = timeList[0].Trim();
            var endDate = timeList[1].Trim();

            var url = string.Format("Fixture/QueryQueryFixtureReportByFMTDetailAPI?Process_Info_UID={0}&startDate={1}&endDate={2}", Process_Info_UID, startDate, endDate);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string ExportReportByFMTValid(Batch_ReportByStatus search)
        {
            string errorInfo = string.Empty;
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
            var url = string.Format("Fixture/ExportReportByFMTValidAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public ActionResult ExportReportByFMT(Batch_ReportByStatus search)
        {
            var list = JsonConvert.DeserializeObject<List<Batch_ReportByStatus>>(search.JsonValue);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FMT Dashboard");
            string[] propertiesHead = FlowchartImportCommon.GetFixtureHeadByFMTColumn();
            string[] propertiesDetaiHead = FlowchartImportCommon.GetFixtureHeadByFMTDetailColumn();

            using (var excelPackage = new ExcelPackage(stream))
            {
                if (list.Count() > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("FMT Dashboard");
                    SetStatusStyle(worksheet, propertiesHead, search.hidDate);
                    //Excel sheet名称防止sheet名称重复导致不能创建Excel
                    int sheetCount = 1;
                    int currentRow = 4;
                    foreach (var item in list)
                    {
                        worksheet.Cells[currentRow, 1].Value = currentRow - 3;
                        worksheet.Cells[currentRow, 2].Value = item.PlantName;
                        worksheet.Cells[currentRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[currentRow, 4].Value = item.Func_Name;
                        worksheet.Cells[currentRow, 5].Value = item.Process_Name;
                        worksheet.Cells[currentRow, 6].Value = item.TotalCount;
                        worksheet.Cells[currentRow, 7].Value = item.NewCount;
                        worksheet.Cells[currentRow, 8].Value = item.FreeCount;
                        worksheet.Cells[currentRow, 9].Value = item.SendRepairCount;
                        worksheet.Cells[currentRow, 10].Value = item.ShipCount;
                        worksheet.Cells[currentRow, 11].Value = item.WaitRepairCount;
                        worksheet.Cells[currentRow, 12].Value = item.NeedMaintenCount;
                        worksheet.Cells[currentRow, 13].Value = item.HasMaintenCount;
                        worksheet.Cells[currentRow, 14].Value = item.NotMaintenCount;
                        currentRow++;
                    }

                    //生成明细表
                    var timeList = search.hidDate.Split('~').ToList();
                    var startDate = timeList[0].Trim();
                    var endDate = timeList[1].Trim();
                    foreach (var item in list)
                    {
                        var url = string.Format("Fixture/QueryQueryFixtureReportByFMTDetailAPI?Process_Info_UID={0}&startDate={1}&endDate={2}", item.Process_Info_UID, startDate, endDate);
                        HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
                        var result = responMessage.Content.ReadAsStringAsync().Result;
                        var detailList = JsonConvert.DeserializeObject<List<Batch_ReportByStatus>>(result);

                        var detailworksheet = excelPackage.Workbook.Worksheets.Add(item.Process_Name + "_" + sheetCount);
                        SetFMTDetailStyle(detailworksheet, propertiesDetaiHead, search.hidDate);
                        int detailCurrentRow = 4;
                        foreach (var detailItem in detailList)
                        {
                            detailworksheet.Cells[detailCurrentRow, 1].Value = detailCurrentRow - 3;
                            detailworksheet.Cells[detailCurrentRow, 2].Value = item.PlantName;
                            detailworksheet.Cells[detailCurrentRow, 3].Value = item.OpType_Name;
                            detailworksheet.Cells[detailCurrentRow, 4].Value = item.Func_Name;
                            detailworksheet.Cells[detailCurrentRow, 5].Value = item.Process_Name;
                            detailworksheet.Cells[detailCurrentRow, 6].Value = detailItem.WorkStation_Name;
                            detailworksheet.Cells[detailCurrentRow, 7].Value = detailItem.TotalCount;
                            detailworksheet.Cells[detailCurrentRow, 8].Value = detailItem.NewCount;
                            detailworksheet.Cells[detailCurrentRow, 9].Value = detailItem.FreeCount;
                            detailworksheet.Cells[detailCurrentRow, 10].Value = detailItem.SendRepairCount;
                            detailworksheet.Cells[detailCurrentRow, 11].Value = detailItem.ShipCount;
                            detailworksheet.Cells[detailCurrentRow, 12].Value = detailItem.WaitRepairCount;
                            detailworksheet.Cells[detailCurrentRow, 13].Value = detailItem.NeedMaintenCount;
                            detailworksheet.Cells[detailCurrentRow, 14].Value = detailItem.HasMaintenCount;
                            detailworksheet.Cells[detailCurrentRow, 15].Value = detailItem.NotMaintenCount;
                            detailCurrentRow++;
                        }
                        sheetCount++;
                    }
                    excelPackage.Save();
                }
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        private void SetStatusStyle(ExcelWorksheet worksheet, string[] propertiesHead, string masterTime)
        {
            worksheet.Cells[1, 1].Value = "治具使用情况总计";
            worksheet.Cells[2, 1].Value = masterTime;
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[3, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 10;
                worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
            worksheet.Column(6).Width = 20;
            worksheet.Cells["A1:N1"].Merge = true;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        private void SetFMTDetailStyle(ExcelWorksheet worksheet, string[] propertiesHead, string masterTime)
        {
            worksheet.Cells[1, 1].Value = "治具使用情况明细";
            worksheet.Cells[2, 1].Value = masterTime;
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[3, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 10;
                worksheet.Cells["A1:O1"].Style.Font.Bold = true;
                worksheet.Cells["A1:O1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:O1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
            worksheet.Column(6).Width = 25;
            worksheet.Column(7).Width = 15;
            worksheet.Cells["A1:O1"].Merge = true;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        }
        #endregion 报表-FMT Dashboard Add by Rock 2017-12-11----------------End
        private FixtureVM GetFixtureStatus()
        {
            FixtureVM currentVM = new FixtureVM();
            var apiUrl = string.Format("Fixture/GetFixtureStatuDTOAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var fixtureStatus = JsonConvert.DeserializeObject<List<FixtureStatuDTO>>(result);
            currentVM.FixtureStatus = fixtureStatus;
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
            return currentVM;
        }
    }
}