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
using System.Net;
using System.Web;
using System.Linq;
using PDMS.Web.Business.Flowchart;
using OfficeOpenXml.Style;
using System.Drawing;
using PDMS.Model.ViewModels.Batch;
using PDMS.Model.EntityDTO;

namespace PDMS.Web.Controllers
{
    public class SettingsController : WebControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        #region User Maintenance-------------------Add By Tonny 2015/11/02
        public ActionResult UserMaintenance()
        {
            int result = 0;
            foreach (SystemRoleDTO role in CurrentUser.GetUserInfo.RoleList)
            {
                if (role.Role_ID == "SystemAdmin")
                    result = 1;
            }
            ViewBag.IsSuperAd = result;
            return View();
        }

        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryUsers(UserModelSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryUsersAPI");
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            search.Orgnizations = Orgnizations;
            search.currntUID = CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// get user by account id
        /// </summary>
        /// <param name="uuid">account id</param>
        /// <returns>single user json</returns>
        public ActionResult QueryUser(int uuid)
        {
            UserModelSearch search = new UserModelSearch();
            search.currntUID = uuid;
            var apiUrl = string.Format("Settings/QueryUserAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }



        public ActionResult queryUserByNTID(string NTID)
        {
            var apiUrl = string.Format("Settings/queryUserByNTIDAPI?NTID={0}", NTID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// delete specific user
        /// </summary>
        /// <param name="dto">entity, or just fill the primary key account id</param>
        /// <returns></returns>
        public ActionResult DeleteUser(SystemUserDTO dto)
        {
            var apiUrl = string.Format("Settings/DeleteUserAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        //禁用用户
        public string ForbidUser(int Account_UID)
        {
            var apiUrl = string.Format("Settings/ForbidUserAPI?Account_UID={0}", Account_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// edit or add new user
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="isEdit">flag, true edit/false add</param>
        /// <returns></returns>
        public ActionResult EditProfile(SystemUserInfo1 dto, bool isEdit)
        {

            dto.CurrentUID = CurrentUser.AccountUId;
            //add new
            var apiUrl = string.Format("Settings/AddUserAPI");
            dto.Modified_UID = this.CurrentUser.AccountUId;
            if (isEdit == true)
            {
                //modify
                apiUrl = string.Format("Settings/ModifyUserAPI");
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);

            return RedirectToAction("UserMaintenance", "Settings");
        }

        /// call by validate 根据用户Account UID判定用户是否存在
        /// </summary>
        /// <param name="Account_UID"></param>
        /// <returns>jsonstring : true [not exist]/ false [exist]</returns>
        public ActionResult SystemUserWithUIdNotExist(int Account_UID)
        {
            var apiUrl = string.Format("Common/GetSystemUserByUId/{0}", Account_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.StatusCode == HttpStatusCode.NotFound ? "true" : "false";

            return Content(result, "application/json");
        }

        /// <summary>
        /// call by validate 根据用户NTID判定用户是否存在
        /// </summary>
        /// <param name="User_NTID"></param>
        /// <returns>jsonstring : true/false</returns>
        public ActionResult SystemUserWithNTIdNotExist(string User_NTID)
        {
            var apiUrl = string.Format("Common/GetSystemUserByNTId/?ntid={0}&islogin={1}", User_NTID, 0);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.StatusCode == HttpStatusCode.NotFound ? "true" : "false";

            return Content(result, "application/json");
        }



        /// <summary>
        /// Export data selected in grid
        /// </summary>
        /// <param name="uuids">selected keys</param>
        /// <returns></returns>
        public ActionResult DoExportUser(string uuids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportUserAPI?uuids={0}", uuids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<SystemUserDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("SystemUsers");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Account UID", "User NTID", "User Name", "Enable", "User Email", "User Tel", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("System Users");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Account_UID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.User_NTID;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Enable_Flag ? "Y" : "N";
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Email;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Tel;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Modified_UserNTID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion //User Maintenance

        #region Plant Maintenance -------------------Add By Sidney 2015/11/09
        public ActionResult PlantMaintenance()
        {
            return View();
        }
        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryPlants(PlantModelSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryPlantsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// get user by account id
        /// </summary>
        /// <param name="uuid">account id</param>
        /// <returns>single user json</returns>
        public ActionResult QueryPlant(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryPlantAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// delete specific user
        /// </summary>
        /// <param name="dto">entity, or just fill the primary key account id</param>
        /// <returns></returns>
        public ActionResult DeletePlant(SystemPlantDTO dto)
        {
            var apiUrl = string.Format("Settings/DeletePlantAPI");
            //MVC调用WebAPI请使用PDMS.Core.APIHelper中的方法。此方法内对授权和异常进行了处
            //理。如有特殊复杂需求，可自行封装。
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportPlant(string uuids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportPlantAPI?uuids={0}", uuids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<SystemPlantDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Plant Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Plant", "Location", "Type", "Plant Code", "Plant Name(ZH)", "PlantManager Name", "PlantManager Tel", "Begin Date", "End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Plant Maintenance");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Location;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Type;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Name_0;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Name_1;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.PlantManager_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.PlantManager_Tel;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.End_Date == null ? null : ((DateTime)currentRecord.End_Date).ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult AddPlant(string jsonAddPlant)
        {

            var apiUrl = "Settings/AddPlantAPI";
            var entity = JsonConvert.DeserializeObject<SystemPlantDTO>(jsonAddPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditPlant(string jsonEditPlant)
        {

            var apiUrl = "Settings/ModifyPlantAPI";
            var entity = JsonConvert.DeserializeObject<SystemPlantDTO>(jsonEditPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>Plant
        /// edit or add new 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="isEdit">flag, true edit/false add</param>
        /// <returns></returns>
        public ActionResult EditPlantProfile(SystemPlantDTO dto, bool isEdit)
        {
            //add new
            var apiUrl = string.Format("Settings/AddPlantAPI");
            dto.Modified_UID = this.CurrentUser.AccountUId;
            if (isEdit == true)
            {
                //modify
                apiUrl = string.Format("Settings/ModifyPlantAPI");
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            //return Content("<script >alert('提交留言成功，谢谢对我们支持，我们会根据您提供联系方式尽快与您取的联系！');</script >", "text/html");
            return RedirectToAction("PlantMaintenance", "Settings");
        }

        /// <summary>
        /// call by validate, check user is exist or not
        /// </summary>
        /// <param name="Account_UID">account id</param>
        /// <returns>json, true/false</returns>
        public ActionResult CheckPlantExistByUId(int System_Plant_UID)
        {
            var apiUrl = string.Format("Settings/CheckPlantExistByUId/?uuid={0}", System_Plant_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        /// <summary>
        /// call by validate, check user is exist or not
        /// </summary>
        /// <param name="Account_UID">account id</param>
        /// <returns>json, true/false</returns>
        public ActionResult CheckPlantExistByPlant(string Plant)
        {
            var apiUrl = string.Format("Settings/CheckPlantExistByPlant/?Plant={0}", Plant);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        public ActionResult GetMaxEnddate4Plant(int uid)
        {
            var apiUrl = string.Format("Settings/GetMaxEnddate4Plant/{0}", uid.ToString());
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }
        #endregion //Plant Maintenance 

        #region BU Master Maintenance-------------------Add By Rock 2015/11/09
        //SETTINGS/QUERYBUINFOAPI
        public ActionResult BUMasterMaintenance()
        {
            return View();
        }

        /// <summary>
        /// 页面初始化加载
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryBUs(BUModelSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryBUsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// 检查BU ID是否存在
        /// </summary>
        /// <param name="BU_ID"></param>
        /// <returns></returns>
        public ActionResult CheckBuExistById(string BU_ID)
        {
            var apiUrl = string.Format("Settings/CheckBuExistById/?buid={0}", BU_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        /// <summary>
        /// 检查BU Name是否存在
        /// </summary>
        /// <param name="BU_Name"></param>
        /// <returns></returns>
        //public ActionResult CheckBuExistByName(string BU_Name)
        //{
        //    var apiUrl = string.Format("Settings/CheckBuExistByName/?buname={0}", BU_Name);
        //    HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

        //    return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        //}

        /// <summary>
        /// 新增或修改BU信息
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        public ActionResult AddOrEditBUInfo(SystemBUMDTO dto, bool isEdit)
        {
            //add new
            var apiUrl = string.Format("Settings/AddBUAPI");
            dto.Modified_UID = this.CurrentUser.AccountUId;
            if (isEdit == true)
            {
                //modify
                apiUrl = string.Format("Settings/ModifyBUAPI");
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);

            return RedirectToAction("BUMasterMaintenance", "Settings");
        }

        /// <summary>
        /// 通过BU_M_UID获取BU信息
        /// </summary>
        /// <param name="BU_M_UID"></param>
        /// <returns></returns>
        public ActionResult QueryBU(int BU_M_UID)
        {
            var apiUrl = string.Format("Settings/QueryBUAPI?BU_M_UID={0}", BU_M_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteBU(SystemBUMDTO dto)
        {
            var apiUrl = string.Format("Settings/DeleteBUAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DoExportBU(string uuids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportBUAPI?uuids={0}", uuids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<BUModelGet>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("BU Master Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "BU ID", "BU Name", "BUManager Name", "BUManager Tel", "BUManager Email", "Begin Date", "End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("BU Master Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.SystemBUMDTO.BU_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.SystemBUMDTO.BU_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.SystemBUMDTO.BUManager_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.SystemBUMDTO.BUManager_Tel;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.SystemBUMDTO.BUManager_Email;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.SystemBUMDTO.Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 8].Value = currentRecord.SystemBUMDTO.End_Date != null ? currentRecord.SystemBUMDTO.End_Date.Value.ToString(FormatConstants.DateTimeFormatStringByDate) : string.Empty;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.SystemUserDTO.User_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.SystemBUMDTO.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion //BU Master Maintenance

        #region BU Detail Maintenance ----------------------------Add by Rock 2015/1109
        public ActionResult BUDetailMaintenance()
        {
            return View();
        }
        public ActionResult QueryBU_Org(int System_BU_D_Org_UID)
        {
            var apiUrl = string.Format("Settings/QueryBU_OrgAPI?System_BU_D_Org_UID={0}", System_BU_D_Org_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult AddOrEditBUOrganation(SystemBUD_OrgDTO dto, bool isEdit)
        {
            var apiUrl = string.Empty;

            if (isEdit)
            {
                apiUrl = "Settings/ModifyBUD_OrgAPI";
            }
            else
            {
                apiUrl = "Settings/AddBUD_OrgAPI";
            }

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);

            return RedirectToAction("BUDetailMaintenance", "Settings");
        }

        public ActionResult DeleteBU_Org(SystemBUD_OrgDTO dto)
        {
            var apiUrl = string.Format("Settings/DeleteBU_OrgAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult QueryBUDetails(BUDetailModelSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryBUDetailsAPI");
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            if (Orgnizations != null && Orgnizations.Count > 0 && Orgnizations[0].OPType_OrganizationUID != null)
            {
                search.Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            else
            {
                search.Organization_UID = 0;
            }
            search.CurrentUser = CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult BUOrganization()
        {
            return View();
        }
        public ActionResult QueryBUD_Orgs(BUD_OrgSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryBUD_OrgsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult CheckBuExistById_Two(string BU_ID)
        {
            var apiUrl = string.Format("Settings/CheckBuExistById_TwoAPI/?buid={0}", BU_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        public ActionResult CheckBeginDateAndEndDate(string BU_ID, string BU_Name, string Begin_Date, string End_Date, bool isEdit)
        {
            var apiUrl = string.Format("Settings/CheckBeginDateAndEndDateAPI/?BU_ID={0}&BU_Name={1}&Begin_Date={2}&End_Date={3}", BU_ID, BU_Name, Begin_Date, End_Date);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryBUDInfoByBU_D_ID(string BU_D_ID)
        {
            var apiUrl = string.Format("Settings/QueryBUDInfoByBU_D_IDAPI/?BU_D_ID={0}", BU_D_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetBUIDAndBUNameByBUID(string BU_ID)
        {
            var apiUrl = string.Format("Settings/GetBUIDAndBUNameByBUIDAPI/?BU_ID={0}", BU_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (result == "null")
            {
                SystemBUMDTO dto = new SystemBUMDTO();
                result = JsonConvert.SerializeObject(dto);
                //result = Json(dto).ToString();
            }
            return Content(result, "application/json");
        }

        //public ActionResult CheckExistBU_D_ID(string BU_D_ID)
        //{
        //    var apiUrl = string.Format("Settings/CheckExistBU_D_IDAPI/?BU_D_ID={0}&BU_D_UID={1}&isEdit={2}", BU_D_ID,BU_D_UID,isEdit);
        //    HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
        //    var result = responMessage.Content.ReadAsStringAsync().Result;
        //    return Content(result, "application/json");
        //}

        public ActionResult AddOrEditBUDetailInfo(SystemBUDDTO dto, bool isEdit)
        {
            var apiUrl = string.Empty;
            string BU_ID = Request.Form["BU_ID"];
            string BU_Name = Request.Form["BU_Name"];
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
            {
                //dto.Organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value; 映射注释
            }
            else
            {
                //dto.Organization_UID = 0; 映射注释
            }
            if (isEdit)
            {
                apiUrl = string.Format("Settings/EditBUDetailInfoAPI?BU_ID={0}&BU_Name={1}&isEdit={2}", BU_ID, BU_Name, isEdit);
            }
            else
            {
                apiUrl = string.Format("Settings/AddBUDetailInfoAPI?BU_ID={0}&BU_Name={1}&isEdit={2}", BU_ID, BU_Name, isEdit);
            }
            dto.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);

            return RedirectToAction("BUDetailMaintenance", "Settings");
        }

        public ActionResult QueryBUDetail(int BU_D_UID)
        {
            var apiUrl = string.Format("Settings/QueryBUDetailAPI?BU_D_UID={0}", BU_D_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteBUDDetail(int BU_D_UID)
        {
            var apiUrl = string.Format("Settings/DeleteBUDDetailAPI?BU_D_UID={0}", BU_D_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DoExportBUDetail(string uuids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportBUDetailAPI?BU_D_UIDS={0}", uuids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<BUDetailModelGet>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("BU Detail Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "BU ID", "BU Name", "BU Customer ID", "BU Customer Name", "Begin Date", "End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("BU Detail Maintenance");

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
                    //BU ID
                    worksheet.Cells[index + 2, 2].Value = currentRecord.SystemBUMDTO.BU_ID;
                    //BU Name
                    worksheet.Cells[index + 2, 3].Value = currentRecord.SystemBUMDTO.BU_Name;
                    //BU Customer ID
                    worksheet.Cells[index + 2, 4].Value = currentRecord.SystemBUDDTO.BU_D_ID;
                    //BU Customer Name
                    worksheet.Cells[index + 2, 5].Value = currentRecord.SystemBUDDTO.BU_D_Name;
                    //Begin Date
                    worksheet.Cells[index + 2, 6].Value = currentRecord.SystemBUDDTO.Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 7].Value = currentRecord.SystemBUDDTO.End_Date != null ? currentRecord.SystemBUDDTO.End_Date.Value.ToString(FormatConstants.DateTimeFormatStringByDate) : string.Empty;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.SystemUserDTO.User_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.SystemBUDDTO.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion

        #region User BU Setting ------------------Start--------------Add by Rock 2015/11/18---------
        public ActionResult UserBUSetting()
        {
            return View();
        }

        public ActionResult QueryUserBUs(UserBUSettingSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryUserBUsAPI");
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            if (Orgnizations != null && Orgnizations.Count > 0 && Orgnizations[0].OPType_OrganizationUID != null)
            {
                search.OrgID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            else
            {
                search.OrgID = 0;
            }
            search.CurrentUser = CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");

        }

        public ActionResult QueryUserBU(int System_User_BU_UID)
        {
            var apiUrl = string.Format("Settings/QueryUserBUAPI?System_User_BU_UID={0}", System_User_BU_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryBUAndBUDSByBUID(string BU_ID)
        {
            var apiUrl = string.Format("Settings/QueryBUAndBUDSByBUIDAPI?BU_ID={0}", BU_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditUserBU(string jsonUserBUWithSubs, bool isEdit)
        {
            var apiUrl = string.Empty;
            if (isEdit)
            {
                apiUrl = string.Format("Settings/EditUserBUAPI");
            }
            else
            {
                apiUrl = string.Format("Settings/AddUserBUAPI");
            }
            var entity = JsonConvert.DeserializeObject<UserBUAddOrSave>(jsonUserBUWithSubs);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult ExportUserBU(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportUserBUAPI?KeyIDS={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<UserBUSettingGet>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("User BU Setting");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "User NTID", "User Name", "BU ID", "BU Name", "BU Customer ID", "BU Customer Name", "Begin Date", "End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("User BU Setting");

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
                    //User NTID
                    worksheet.Cells[index + 2, 2].Value = currentRecord.SystemUserDTO.Account_UID;
                    //User Name
                    worksheet.Cells[index + 2, 3].Value = index + 1;
                    //BU ID
                    worksheet.Cells[index + 2, 4].Value = currentRecord.SystemBUMDTO.BU_ID;
                    //BU Name
                    worksheet.Cells[index + 2, 5].Value = currentRecord.SystemBUMDTO.BU_Name;
                    //BU Customer ID
                    worksheet.Cells[index + 2, 6].Value = currentRecord.SystemBUDDTO != null ? currentRecord.SystemBUDDTO.BU_D_ID : string.Empty;
                    //BU Customer Name
                    worksheet.Cells[index + 2, 7].Value = currentRecord.SystemBUDDTO != null ? currentRecord.SystemBUDDTO.BU_D_Name : string.Empty;
                    //Begin Date
                    worksheet.Cells[index + 2, 8].Value = currentRecord.SystemUserBusinessGroupDTO.Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    //End Date
                    worksheet.Cells[index + 2, 9].Value = currentRecord.SystemUserBusinessGroupDTO.End_Date != null ? currentRecord.SystemUserBusinessGroupDTO.End_Date.Value.ToString(FormatConstants.DateTimeFormatStringByDate) : string.Empty;
                    //Modified User
                    worksheet.Cells[index + 2, 10].Value = currentRecord.SystemUserDTO1.User_Name;
                    //Modified Time
                    worksheet.Cells[index + 2, 11].Value = currentRecord.SystemUserBusinessGroupDTO.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion User BU Setting ------------------End--------------Add by Rock 2015/11/18---------

        #region Role Maintenance------------------Add By Allen 2015/11/9

        public ActionResult RoleMaintenance()
        {
            return View();
        }

        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryRoles(RoleModelSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryRolesAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// get user by account id
        /// </summary>
        /// <param name="uuid">account id</param>
        /// <returns>single user json</returns>
        public ActionResult QueryRole(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryRoleAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult IsExistRoleID(string role_ID)
        {
            var apiUrl = string.Format("Settings/IsExistRoleIDAPI?role_ID={0}", role_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// delete specific user
        /// </summary>
        /// <param name="dto">entity, or just fill the primary key account id</param>
        /// <returns></returns>
        public ActionResult DeleteRole(SystemRoleDTO dto)
        {
            var apiUrl = string.Format("Settings/DeleteRoleAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// edit or add new user
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="isEdit">flag, true edit/false add</param>
        /// <returns></returns>
        public ActionResult Editfile(SystemRoleDTO dto, bool isEdit)
        {
            //add new
            var apiUrl = string.Format("Settings/AddRoleAPI");
            dto.Modified_UID = this.CurrentUser.AccountUId;
            if (isEdit == true)
            {
                //modify
                apiUrl = string.Format("Settings/ModifyRoleAPI");
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);

            return RedirectToAction("RoleMaintenance", "Settings");
        }

        /// <summary>
        /// call by validate, check user is exist or not
        /// </summary>
        /// <param name="Account_UID">account id</param>
        /// <returns>json, true/false</returns>
        public ActionResult CheckRoleExistById(string Role_ID)
        {
            var apiUrl = string.Format("Settings/CheckRoleExistById/?rid={0}", Role_ID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        /// <summary>
        /// Export data selected in grid
        /// </summary>
        /// <param name="uuids">selected keys</param>
        /// <returns></returns>
        public ActionResult DoExportRole(string ruids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportRoleAPI?ruids={0}", ruids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<SystemRoleDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("SystemRole");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Role ID", "Role Name", "父ID", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("System Role");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Role_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Role_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Father_Role_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion //Role Maintenance

        #region System Function Maintenance Add By Tonny 2015/11/12

        public ActionResult SystemFunctionMaintenance()
        {
            return View();
        }

        /// <summary>
        /// get paged records of functions by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryFunctions(FunctionModelSearch search, Page page)
        {
            var apiUrl = "Settings/QueryFunctionsAPI";

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryFunction(int uid)
        {
            var apiUrl = string.Format("Settings/QueryFunctionWithSubsAPI?uid={0}", uid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult AddFunction(string jsonFunctionWithSubs)
        {

            var apiUrl = "Settings/AddFunctionWithSubsAPI";
            var entity = JsonConvert.DeserializeObject<FunctionWithSubs>(jsonFunctionWithSubs);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult ModifyFunctionWithSubs(string jsonFunctionWithSubs)
        {

            var apiUrl = "Settings/ModifyFunctionWithSubsAPI";
            var entity = JsonConvert.DeserializeObject<FunctionWithSubs>(jsonFunctionWithSubs);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult GetFunctionByID(string functionId)
        {
            var apiUrl = string.Format("Settings/GetFunctionByIDAPI?functionId={0}", functionId);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteFunction(int uid)
        {
            var apiUrl = string.Format("Settings/DeleteFunctionAPI?uid={0}", uid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteSubFunction(int subfunction_UId)
        {
            var apiUrl = string.Format("Settings/DeleteSubFunctionAPI?subfunction_UId={0}", subfunction_UId);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DoExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<IList<FunctionItem>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("SystemFunction");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Parent Function ID", "Parent Function Name", "Function ID"
                , "Function Name", "Order Index", "Is Show","URL","Sub Function", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("System Function");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Parent_Function_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Parent_Function_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Function_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Function_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Order_Index;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Is_Show ? "Y" : "N";
                    worksheet.Cells[index + 2, 8].Value = currentRecord.URL;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.SubFunctionCount;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion //end System Function Maintenance

        #region User Plant Setting Add By Sidney 2015/11/17

        public ActionResult UserPlantSetting()
        {
            return View();
        }
        #region Add

        [HttpGet]
        public ActionResult GetPlantInfoByPlant(string Plant)
        {
            var apiUrl = string.Format("Settings/GetPlantInfoAPI?Plant={0}", Plant);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        ///通过厂区获取组织ID
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getFunPOrgId(string Plant)
        {
            var apiUrl = string.Format("Settings/getFunPOrgIdAPI?Plant={0}", Plant);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        public ActionResult AddUserPlant(string jsonUserWithPlant)
        {

            var apiUrl = "Settings/AddUserPlantAPI";
            var entity = JsonConvert.DeserializeObject<UserPlantEditModel>(jsonUserWithPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        #endregion
        #region query
        public ActionResult QueryUserPlants(UserPlantModelSearch search, Page page)
        {
            var apiUrl = "Settings/QueryUserPlantsAPI";

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult QueryUserPlantByAccountUID(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryUserPlantByAccountUID?uuid={0}", uuid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        #endregion
        #region modify
        public ActionResult modifyUserPlant(string jsonUserWithPlant)
        {

            var apiUrl = "Settings/ModifyUserPlantAPI";
            var entity = JsonConvert.DeserializeObject<UserPlantEditModel>(jsonUserWithPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        #endregion
        public ActionResult DoExportUserPlant(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportUserPlantAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<IList<UserPlantItem>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("UserPlantSetting");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "User NTID", "User Name", "Plant"
                , "Location", "Type", "Plant Code","Begin Date","End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("UserPlantSetting");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.User_NTID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Location;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Plant_Code;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Begin_Date == null ? null : ((DateTime)currentRecord.Begin_Date).ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 9].Value = currentRecord.End_Date == null ? null : ((DateTime)currentRecord.End_Date).ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion

        #region Role Function Setting Module -------------- Add by Tonny 2015/11/18

        public ActionResult RoleFunctionSetting()
        {
            var apiUrl = "Common/GetAllRoles";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var rolesModel = JsonConvert.DeserializeObject<IEnumerable<SystemRoleDTO>>(result);
            return View(rolesModel);
        }

        /// <summary>
        /// get paged records of role functions by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryRoleFunctions(RoleFunctionSearchModel search, Page page)
        {
            var apiUrl = "Settings/QueryRoleFunctionsAPI";

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult QueryRoleFunction(int roleUId)
        {
            var apiUrl = "Settings/QueryRoleFunctionAPI/" + roleUId.ToString();

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult QueryRoleSubFunctions(RoleSubFunctionSearchModel search)
        {
            var apiUrl = string.Format("Settings/QueryRoleSubFunctionsAPI/?Role_UID={0}&Function_UID={1}", search.Role_UID.ToString(), search.Function_UID.ToString());

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult QuerySubFunctionsByFunctionUID(int uid)
        {
            var apiUrl = string.Format("Settings/QuerySubFunctionsByFunctionUIDAPI/{0}", uid.ToString());

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult DeleteRoleFunction(int uid)
        {
            var apiUrl = string.Format("Settings/DeleteRoleFunctionAPI/{0}", uid.ToString());

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult ModifyRoleFunctionWithSubs(string jsonFunctionWithSubs)
        {

            var apiUrl = string.Format("Settings/ModifyRoleFunctionWithSubsAPI?AccountUId={0}", CurrentUser.AccountUId);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonFunctionWithSubs, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult AddRoleFunctionWithSubs(string jsonFunctionWithSubs)
        {

            var apiUrl = string.Format("Settings/AddRoleFunctionWithSubsAPI?AccountUId={0}", CurrentUser.AccountUId);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonFunctionWithSubs, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult ExportRoleFunctionSetting(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportRoleFunctionsAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<IList<RoleFunctionItem>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("RoleFunctionSetting");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Role ID", "Function ID", "Function Name", "Order Index",
                "Is Show(Function)","URL","Is Show(Role)","Sub Function Qty", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("RoleFunctionSettings");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Role_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Function_ID;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Function_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Order_Index;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Is_Show_Function ? "Y" : "N";
                    worksheet.Cells[index + 2, 7].Value = currentRecord.URL;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Show_Role ? "Y" : "N";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.SubFunctionCount > 0 ? "Y" : "N";
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion

        #region Organization Maintenance      by  justin

        public ActionResult OrgMaintenance()
        {
            return View();
        }
        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryOrgs(OrgModelSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryOrgsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// get user by account id
        /// </summary>
        /// <param name="uuid">account id</param>
        /// <returns>single user json</returns>
        public ActionResult QueryOrg(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryOrgAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteOrg(int uid)
        {
            var apiUrl = string.Format("Settings/DeleteOrgAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrg(string jsonAddOrg)
        {

            var apiUrl = "Settings/AddOrgAPI";
            var entity = JsonConvert.DeserializeObject<SystemOrgDTO>(jsonAddOrg);
            if (entity.Organization_Name != null)
            {
                entity.Organization_Name = entity.Organization_Name.Trim();
            }
            if (entity.OrgManager_Name != null)
            {
                entity.OrgManager_Name = entity.OrgManager_Name.Trim();
            }
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditOrg(string jsonEditOrg)
        {

            var apiUrl = "Settings/ModifyOrgAPI";
            var entity = JsonConvert.DeserializeObject<SystemOrgDTO>(jsonEditOrg);
            if (entity.Organization_Name != null)
            {
                entity.Organization_Name = entity.Organization_Name.Trim();
            }
            if (entity.OrgManager_Name != null)
            {
                entity.OrgManager_Name = entity.OrgManager_Name.Trim();
            }
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult CheckOrgExistByIdAndName(string id, string name)
        {
            var apiUrl = string.Format("Settings/CheckOrgExistByIdAndName/?id={0}&name={1}", id, name);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        public ActionResult CheckOrgExistByIdAndNameWithUId(int uid, string id, string name)
        {
            var apiUrl = string.Format("Settings/CheckOrgExistByIdAndNameWithUId/?uid={0}&id={1}&name={2}", uid, id, name);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        public ActionResult GetMaxEnddate4Org(int orgUId)
        {
            var apiUrl = string.Format("Settings/GetMaxEnddate4Org/{0}", orgUId.ToString());
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        public ActionResult DoExportOrg(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportOrgAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("SystemOrganization");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Organization ID", "Organization Name", "Organization_Desc", "OrgManager Name", "OrgManager Tel", "OrgManager Email", "Begin Date", "End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("System Plants");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Organization_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Organization_Desc;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.OrgManager_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.OrgManager_Tel;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.OrgManager_Email;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Cost_Center;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 10].Value =
                        currentRecord.End_Date == null ? string.Empty :
                            ((DateTime)currentRecord.End_Date).ToString(FormatConstants.DateTimeFormatStringByDate);
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

        #region OrganizationBom Maintenance    by justin
        public ActionResult OrganizationBomMaintenance()
        {
            return View();
        }
        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryOrgBoms(OrgBomModelSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryOrgBomsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryOrgBom(int uid)
        {
            var apiUrl = string.Format("Settings/QueryOrgBomAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrgBom(string jsonAddOrgBom)
        {

            var apiUrl = "Settings/AddOrgBomAPI";
            var entity = JsonConvert.DeserializeObject<SystemOrgBomDTO>(jsonAddOrgBom);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditOrgBom(string jsonEditOrgBom)
        {

            var apiUrl = "Settings/ModifyOrgBomAPI";
            var entity = JsonConvert.DeserializeObject<SystemOrgBomDTO>(jsonEditOrgBom);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DoExportOrgBom(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportOrgBomAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<SystemOrgAndBomDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("SystemOrganizationBom");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "Parent Organization ID", "Parent Organization Name", "Order Index", "Child Organization ID", "Child Organization Name", "Begin Date", "End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("System OranizationBoms");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.ParentOrg_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Parent_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Order_Index;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.ChildOrg_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Child_Organization_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Begin_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 8].Value =
                        currentRecord.End_Date == null ? string.Empty :
                            ((DateTime)currentRecord.End_Date).ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult CheckOrgBomExist(int uid, int parentUId, int childUId, int index)
        {
            var apiUrl = string.Format("Settings/CheckOrgBomExistAPI/?uid={0}&parentUId={1}&childUId={2}&index={3}",
                                                uid.ToString(), parentUId.ToString(), childUId.ToString(), index.ToString());

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #endregion

        #region User Organization Setting Add By Justin

        public ActionResult UserOrgSetting()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetOrgInfoByOrg(string Org)
        {
            var apiUrl = string.Format("Settings/GetOrgInfoAPI?OrgID={0}", Org);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #region query
        public ActionResult QueryUserOrgs(UserOrgModelSearch search, Page page)
        {
            var apiUrl = "Settings/QueryUserOrgsAPI";

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult QueryUserOrg(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryUserOrgsByAccountUIDAPI?uuid={0}", uuid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        #endregion

        #region Add
        public ActionResult AddUserOrg(string jsonUserWithPlant)
        {

            var apiUrl = "Settings/AddUserOrgAPI";
            var entity = JsonConvert.DeserializeObject<UserOrgEditModel>(jsonUserWithPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        #endregion

        #region modify
        public ActionResult modifyUserOrg(string jsonUserWithPlant)
        {

            var apiUrl = "Settings/ModifyUserOrgAPI";
            var entity = JsonConvert.DeserializeObject<UserOrgEditModel>(jsonUserWithPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        #endregion

        public ActionResult DoExportUserOrg(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportUserOrgAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<IList<UserOrgItem>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("UserOrgSetting");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "User NTID", "User Name", "Organization ID"
                , "Organization Name","Begin Date","End Date", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("UserOrgSetting");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.User_NTID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Organization_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Organization_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Begin_Date == null ? null : ((DateTime)currentRecord.Begin_Date).ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 7].Value = currentRecord.End_Date == null ? null : ((DateTime)currentRecord.End_Date).ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        #endregion

        #region User Role Setting------------------Add By Allen 2015/11/16
        public ActionResult UserRoleSetting()
        {
            var apiUrl = "Common/GetAllRoles";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            var rolesModel = JsonConvert.DeserializeObject<IEnumerable<SystemRoleDTO>>(result);
            return View(rolesModel);
        }

        /// <summary>
        /// get paged records of users by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryUserRoles(UserRoleSearchModel search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryUserRolesAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// get user by account id 
        /// </summary>
        /// <param name="uuid">account id</param>
        /// <returns>single user json</returns>
        public ActionResult QueryUserRole(int uruid)
        {
            var apiUrl = string.Format("Settings/QueryUserRoleAPI?uruid={0}", uruid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// delete specific user
        /// </summary>
        /// <param name="dto">entity, or just fill the primary key account id</param>
        /// <returns></returns>
        public ActionResult DeleteUserRole(SystemUserRoleDTO dto)
        {
            var apiUrl = string.Format("Settings/DeleteUserRoleAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }



        /// <summary>
        /// edit or add new user 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="isEdit">flag, true edit/false add</param>
        /// <returns></returns>
        public string EditUserRoleProfile(SystemUserRoleDTO dto)
        {
            //add new
            var apiUrl = string.Format("Settings/AddUserRoleAPI");
            dto.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 檢查NTID
        /// </summary>
        /// <param name="User_NTID">User NTID</param>
        /// <returns>json, true/false</returns>
        public ActionResult CheckUserRoleExistByNtid(string ntid)
        {
            var apiUrl = string.Format("Settings/CheckUserRoleExistByNtidAPI/?ntid={0}", ntid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return Content(responMessage.Content.ReadAsStringAsync().Result, "application/json");
        }

        /// <summary>
        /// 取得RoleName
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public ActionResult GetRoleNameById(string roleid)
        {
            var apiUrl = string.Format("Settings/GetRoleNameByIdAPI?roleid={0}", roleid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// Export data selected in grid   
        /// </summary>
        /// <param name="uuids">selected keys</param>
        /// <returns></returns>
        public ActionResult DoExportUserRole(string uruids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportUserRoleAPI?uruids={0}", uruids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<UserRoleItem>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("SystemUserRole");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "Seq", "User NTID", "User Name", "Role ID", "Role Name", "Modified User", "Modified Date" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name    
                var worksheet = excelPackage.Workbook.Worksheets.Add("System UserRole");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.User_NTID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.User_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Role_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Role_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion //User Role Setting

        #region FuncPlant Maintenance----------------------------Add By Sidney 2016/02/23

        public ActionResult FuncPlantMatenance()
        {
            return View();
        }
        public ActionResult AddFuncPlant(string jsonAddPlant)
        {

            var apiUrl = "Settings/AddFuncPlantAPI";
            var entity = JsonConvert.DeserializeObject<FuncPlantMaintanance>(jsonAddPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryFuncPlants(FuncPlantSearchModel search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryFuncPlantsAPI");

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult getPlant()
        {
            var apiUrl = "Settings/GetPlantSingleAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFuncPlant(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryFuncPlantAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteFuncPlant(int System_FuncPlant_UID)
        {
            var apiUrl = string.Format("Settings/DeleteFuncPlantAPI?uuid={0}", System_FuncPlant_UID);
            //MVC调用WebAPI请使用PDMS.Core.APIHelper中的方法。此方法内对授权和异常进行了处
            //理。如有特殊复杂需求，可自行封装。
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditFuncPlant(string jsonEditPlant)
        {
            var apiUrl = "Settings/ModifyFuncPlantAPI";
            var entity = JsonConvert.DeserializeObject<FuncPlantMaintanance>(jsonEditPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DoExportFuncPlant(string uuids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportFuncPlantAPI?uuids={0}", uuids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<FuncPlantMaintanance>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("功能厂维护");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "功能厂", "厂区", "OP类型", "功能厂管理者", "功能厂联系方式", "修改者", "修改时间" };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("功能厂维护");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.OPType;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Plant_Manager;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.FuncPlant_Context;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Modified_Date;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion
        #region User FuncPlant Setting----------------------------Add By Sidney 2016/02/23

        public ActionResult UserFuncPlantSetting()
        {
            return View();
        }

        public ActionResult QueryUserFuncPlants(UserFuncPlantSearch search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryUserFuncPlantsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFuncPlant(string Plant, string OP_Types)
        {
            var apiUrl = string.Format("Settings/GetFuncPlantAPI?Plant={0}&&OP_Types={1}", Plant, OP_Types);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetUserInfo(string User_NTID)
        {
            var apiUrl = string.Format("Settings/GetUserInfoAPI?User_NTID={0}", User_NTID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult EditUserFuncPlant(string jsonUserWithFuncPlant)
        {
            var apiUrl = "Settings/EditUserFuncPlantAPI";
            var entity = JsonConvert.DeserializeObject<EditUserFuncPlant>(jsonUserWithFuncPlant);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult GetUserInfoByFuncPlant(string FuncPlant)
        {
            var apiUrl = string.Format("Settings/GetUserInfoByFuncPlantAPI?FuncPlant={0}", FuncPlant);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }



        #endregion

        #region key process Setting -----Add By Destiny

        public ActionResult MainProcessSetting()
        {
            return View();
        }


        public ActionResult QueryProject()
        {
            var apiUrl = string.Format("FlowChart/QueryProject");
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }



        public ActionResult QueryProjectTypes(string Project)
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
                OpTypes = this.CurrentUser.DataPermissions.Op_Types,
                Project_UID = this.CurrentUser.DataPermissions.Project_UID,
                orgs = oporg
            };
            var apiUrl = string.Format("FlowChart/QueryProjectTypes?Project={0}", Project);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryProcessByFlowchartMasterUid(int flowchartMasterUId)
        {
            string apiUrl = string.Format("FlowChart/QueryProcessByFlowchartMasterUid?flowcharMasterUid={0}", flowchartMasterUId);
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteKeyProcess(int Enum_UID)
        {
            var apiUrl = string.Format("Settings/DeleteKeyProcessAPI");
            EnumerationDTO dto = new EnumerationDTO();
            dto.Enum_UID = Enum_UID;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult AddKeyProcess(string Project, string partTypes, string process)
        {
            var apiUrl = string.Format("Settings/AddEnumeration");
            EnumerationDTO dto = new EnumerationDTO();
            dto.Decription = Project;
            dto.Enum_Type = "Report_Key_Process";
            dto.Enum_Name = partTypes;
            dto.Enum_Value = process;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryEnumkeyProcess(string Project, string partTypes, Page page)
        {
            FlowChartModelSearch search = new FlowChartModelSearch();
            search.Part_Types = partTypes;
            search.Project_Name = Project;
            var apiUrl = string.Format("Settings/GetEnumValueForKeyProcess");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetEnumNameForKeyProcess()
        {
            var apiUrl = string.Format("Settings/GetEnumNameForKeyProcess");
            HttpResponseMessage responseMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }



        #endregion

        #region 日常维护模块-------------------Add BySidney

        public ActionResult EditData()
        {
            return View();
        }
        public ActionResult Edit_WIP(EditWIPDTO dto)
        {
            var apiUrl = string.Format("Settings/Edit_WIPAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult Edit_Product(EditProductDTO dto)
        {
            var apiUrl = string.Format("Settings/Edit_ProductAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult Edit_Flag(EditFlagDTO dto)
        {
            var apiUrl = string.Format("Settings/Edit_FlagAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult Add_Enum(EditEnumDTO dto)
        {

            var apiUrl = "Settings/Add_EnumAPI";

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult Edit_Enum(EditEnumDTO dto)
        {
            var apiUrl = string.Format("Settings/Edit_EnumAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult ExeSql(SQLDTO dto)
        {
            var apiUrl = string.Format("Settings/ExeSqlAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region Project Maintanance-------------------------Add By Sidney 2016/05/20

        public ActionResult ProjectMaintanance()
        {
            return View();
        }

        public ActionResult QueryProjects(ProjectSearchModel search, Page page)
        {
            var apiUrl = string.Format("Settings/QueryProjectsAPI");
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            if (Orgnizations != null && Orgnizations.Count > 0 && Orgnizations[0].OPType_OrganizationUID != null)
            {
                search.OrgID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            else
            {
                search.OrgID = 0;
            }
            search.CurrentUser = CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryProject(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryProjectAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryProjectByUID(int uuid)
        {
            var apiUrl = string.Format("Settings/QueryProjectAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetProject(int org)
        {
            var apiUrl = string.Format("Settings/GetProjectByOrgIdAPI?org={0}", org);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetCustomer()
        {
            var apiUrl = "Settings/GetCustomerAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetProductPhase()
        {
            var apiUrl = "Settings/GetProductPhaseAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetOpTypes(int plantuid)
        {
            int oporguid = 1;
            var orgs = CurrentUser.GetUserInfo.RoleList;
            foreach (SystemRoleDTO item in orgs)
            {
                if (item.Role_ID == "SystemAdmin")
                    oporguid = 0;
            }
            if (oporguid == 1)
            {
                if (CurrentUser.GetUserInfo.OPType_OrganizationUIDList.Count == 0)
                    oporguid = 0;
                else
                oporguid = CurrentUser.GetUserInfo.OPType_OrganizationUIDList[0];
            }
               
            var apiUrl = string.Format("Settings/GetOpTypesAPI?plantuid={0}&oporguid={1}", plantuid, oporguid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetPlants()
        {
            int PlantOrgUid = 1;
            var orgs = CurrentUser.GetUserInfo.RoleList;
            if (CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count == 0)
            {
                PlantOrgUid = 0;
            }

            foreach (SystemRoleDTO item in orgs)
            {
                if (item.Role_ID == "SystemAdmin")
                {
                    PlantOrgUid = 0;
                }
            }
            if (PlantOrgUid == 1)
                PlantOrgUid = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID == null ? 0 : (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", PlantOrgUid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddProject(string jsonAddProject)
        {
            var apiUrl = "Settings/AddProjectAPI";
            var entity = JsonConvert.DeserializeObject<ProjectVMDTO>(jsonAddProject);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            //if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0&&CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID!=null)
            //{
            //    entity.OrgID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            //}
            //else
            //{
            //    entity.OrgID =0;
            //}
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditProject(string jsonEditProject)
        {

            var apiUrl = "Settings/ModifyProjectAPI";
            var entity = JsonConvert.DeserializeObject<ProjectVMDTO>(jsonEditProject);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteProject(int Project_UID)
        {
            var apiUrl = string.Format("Settings/DeleteProjectAPI?Project_UID={0}", Project_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region Modify UserMaintanance--------------Add By Sidney 2016/05/27
        /// <summary>
        /// 获取到当前用户的组织及专案信息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult getOrgAndPro()
        {
            int opid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
            {
                opid = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            else
            {
                opid = 0;
            }
            var apiUrl = string.Format("Settings/getOrgAndProAPI?currentUser={0}&opid={1}", this.CurrentUser.AccountUId, opid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        /// <summary>
        /// 获取到当前用户的厂区
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult getUserPlant()
        {
            string plant = "空";
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0)
                plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant;
            var apiUrl = string.Format("Settings/getUserPlantAPI?currentUser={0}&plant={1}", this.CurrentUser.AccountUId, plant);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        #endregion

        #region Modify UserPassword--------------Add By Robert 2017/02/16
        public ActionResult ChangePassWord(string info)
        {
            UserPasswordInfo dto = JsonConvert.DeserializeObject<UserPasswordInfo>(info);
            dto.UserId = CurrentUser.AccountUId;
            var apiUrl = string.Format("Settings/ChangePassWordAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult CheckAcount()
        {
            UserPasswordInfo dto = new UserPasswordInfo();
            dto.UserId = CurrentUser.AccountUId;
            var apiUrl = string.Format("Settings/CheckAcountAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region Language Maintenance -----------------------------Add By Rock 2017/07/18
        public ActionResult LanguageMaintenance()
        {
            return View();
        }

        public ActionResult LanguagesInfo(Page page)
        {
            var apiUrl = string.Format("Settings/LanguagesInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(null, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var test = "{\"data\":" + result + "}";

            return Content(result, "application/json");
        }

        public ActionResult LocaleStringResourceMaintenance(int System_Language_UID)
        {
            ViewBag.System_Language_UID = System_Language_UID;
            return View();
        }

        public ActionResult LocaleStringResourceInfo(SystemLocaleStringResourceDTO dto, Page page, int System_Language_UID)
        {
            var apiUrl = string.Format("Settings/LocaleStringResourceInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportLocalExcel(HttpPostedFileBase upload_excel, int System_Language_UID)
        {
            string errorInfo = string.Empty;
            //获取DB中的多语言信息
            var getApi = string.Format("Settings/LocaleStringResourceInfoByLanguageIdAPI?System_Language_UID={0}", System_Language_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(getApi);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<SystemLocaleStringResourceDTO>>(result);

            using (var xlPackage = new ExcelPackage(upload_excel.InputStream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                int totalRows = worksheet.Dimension.End.Row;
                int totalColumns = worksheet.Dimension.Columns;
                if (worksheet == null || worksheet.Name != "LanguageInfo")
                {

                    errorInfo = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet");
                    return errorInfo;
                }

                int currentRow = 2;
                var propertiesContent = FlowchartImportCommon.GetLanguageHeadColumn();
                List<SystemLocaleStringResourceDTO> ResourceList = new List<SystemLocaleStringResourceDTO>();

                var resourceName = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "System.Language.ResourceName");
                var resourceValue = this.CurrentUser.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "System.Language.ResourceValue");

                while (worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "资源名称")].Value != null)
                {
                    var resourceNameValue = worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "资源名称")].Text;
                    var resourceValueValue = worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "语言名称")].Text;

                    var isExist = list.Exists(m => m.ResourceName.ToUpper() == resourceNameValue.Trim().ToUpper());
                    if (isExist)
                    {
                        continue;
                    }
                    else
                    {
                        SystemLocaleStringResourceDTO dto = new SystemLocaleStringResourceDTO();
                        dto.System_Language_UID = System_Language_UID;
                        dto.ResourceName = resourceNameValue;
                        dto.ResourceValue = resourceValueValue;
                        dto.Modified_UID = this.CurrentUser.AccountUId;
                        dto.Modified_Date = DateTime.Now;
                        ResourceList.Add(dto);
                    }
                    currentRow++;
                }

                var json = JsonConvert.SerializeObject(ResourceList);
                string api = string.Format("Settings/ImportLanguageExcelAPI");
                HttpResponseMessage importResponMessage = APIHelper.APIPostAsync(json, api);
                var importResult = importResponMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return importResult;
            }
        }

        public string SaveResourceInfo(SystemLocaleStringResourceDTO dto)
        {
            dto.Modified_UID = PISSessionContext.Current.CurrentUser.Account_UID;
            dto.CurrentWorkingLanguage = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("Settings/SaveResourceInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        #endregion Language Maintenance -----------------------------Add By Rock 2017/07/18

        #region Language Data Maintenance -----------------------------Add By Rock 2017/08/03
        public ActionResult LanguageDataMaintenance()
        {
            return View();
        }

        #endregion

        #region 车间维护
        public ActionResult WorkshopMaintenance()
        {
            WorkshopVM currentVM = new WorkshopVM();
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
            //ViewBag.PageTitle = "生产车间维护";
            return View("WorkshopMaintenance", currentVM);
        }
        public ActionResult QueryWorkshop(int uid)
        {
            var apiUrl = string.Format("Settings/QueryWorkshopAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult AddWorkshop(WorkshopDTO workshop)
        {
            //去掉空格
            if (workshop!=null)
            {
                if (workshop.Workshop_ID != null)
                {
                    workshop.Workshop_ID = workshop.Workshop_ID.Trim();
                }
                if (workshop.Workshop_Name != null)
                {
                    workshop.Workshop_Name = workshop.Workshop_Name.Trim();
                }
                if (workshop.Building_Name != null)
                {
                    workshop.Building_Name = workshop.Building_Name.Trim();
                }
                if (workshop.Floor_Name != null)
                {
                    workshop.Floor_Name = workshop.Floor_Name.Trim();
                }
                
            }
            var isExistUrl = "Fixture/IsWorkshopExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Settings/AddWorkshopAPI";
            var entity = workshop;
            entity.Modified_UID = CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditWorkshop(WorkshopDTO workshop)
        {
            //去掉空格
            if (workshop != null)
            {
                if (workshop.Workshop_ID != null)
                {
                    workshop.Workshop_ID = workshop.Workshop_ID.Trim();
                }
                if (workshop.Workshop_Name != null)
                {
                    workshop.Workshop_Name = workshop.Workshop_Name.Trim();
                }
                if (workshop.Building_Name != null)
                {
                    workshop.Building_Name = workshop.Building_Name.Trim();
                }
                if (workshop.Floor_Name != null)
                {
                    workshop.Floor_Name = workshop.Floor_Name.Trim();
                }

            }
            var isExistUrl = "Fixture/IsWorkshopExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(workshop, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Settings/EditWorkshopAPI";
            var entity = workshop;
            entity.Modified_UID = CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteWorkshop(int uid)
        {
            var apiUrl = "Settings/DeleteWorkshopAPI";
            var entity = new WorkshopDTO() { Workshop_UID = uid };
            entity.Modified_UID = CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult DoExportWorkshop(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportWorkshopAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<WorkshopDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Workshop Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "车间代码", "楼栋名称", "楼层名称", "楼层名称", "是否启用", "修改人", "修改日期"};

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

        //下载模板
        public FileResult Workshop()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Workshop.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        #endregion

        #region 制程维护
        public ActionResult ProcessInfoMaintenance()
        {
            Process_InfoVM currentVM = new Process_InfoVM();
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

            //ViewBag.PageTitle = "制程维护";
            return View("ProcessInfoMaintenance", currentVM);
        }

        public ActionResult QueryProcessInfoList(Process_InfoModelSearch search)
        {
            var apiUrl = "Settings/QueryProcessInfoListAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryProcessInfo(int uid)
        {
            var apiUrl = string.Format("Settings/QueryProcessInfoAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddProcessInfo(Process_InfoDTO process_Info)
        {
            var isExistUrl = "Fixture/IsProcessInfoExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(process_Info, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Settings/AddProcessInfoAPI";
            var entity = process_Info;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditProcessInfo(Process_InfoDTO process_Info)
        {
            var isExistUrl = "Fixture/IsProcessInfoExistAPI";
            HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(process_Info, isExistUrl);
            var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            if (isExistResult.ToLower() == "true")
            {
                return Content("EXIST");
            }

            var apiUrl = "Settings/EditProcessInfoAPI";
            var entity = process_Info;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteProcessInfo(int uid)
        {
            var apiUrl = "Settings/DeleteProcessInfoAPI";
            var entity = new Process_InfoDTO() { Process_Info_UID = uid };
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }


        public ActionResult DoExportProcessInfo(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Settings/DoExportProcessInfoAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Process_InfoDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Process Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "制程代码", "制程名称", "制程描述", "是否启用", "修改人", "修改日期"};

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Process_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Process_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Process_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable ? "1" : "0"; ;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult DoExportProcessInfoByQuery(Process_InfoModelSearch search)
        {
            //get Export datas
            var apiUrl = "Settings/QueryProcessInfoListAPI";
            var responMessage = APIHelper.APIPostAsync(search,apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Process_InfoDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Process Maintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "制程代码", "制程名称", "制程描述", "是否启用", "修改人", "修改日期" };

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Process_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Process_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Process_Desc;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Enable ? "1" : "0"; ;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.ModifiedUser.User_Name;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //下载模板
        public FileResult Process_Info()
        {
            var filePath = Server.MapPath("~/ExcelTemplate/");
            var fullFileName = filePath + "Process_Info.xlsx";
            FilePathResult fpr = new FilePathResult(fullFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return fpr;
        }
        //上传模板
        public string ImportProcessInfo(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            //errorInfo = //AddVendorInfoExcel(upload_excel);
            return errorInfo;
        }
        #endregion

        #region 表格资料内容语系资源档维护
        public ActionResult SystemLocalizedPropertyMaintenance()
        {
            var currentVM = new System_LocalizedPropertyVM(); //new Process_InfoVM();
            var apiUrl = string.Format("Common/GetLanguagesAllAPI");
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var languageDTOs = JsonConvert.DeserializeObject<List<SystemLanguageDTO>>(result);
            currentVM.LanguageList = languageDTOs;

            ViewBag.PageTitle = "表格资料多语言维护";
            return View("SystemLocalizedPropertyMaintenance", currentVM);
        }
        
        public ActionResult QuerySystemLocalizedProperties(System_LocalizedPropertyModelSearch search, Page page)
        {
            var apiUrl = "Settings/QuerySystemLocalizedPropertiesAPI";
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QuerySystemLocalizedProperty(int uid)
        {
            var apiUrl = string.Format("Settings/QuerySystemLocalizedPropertyAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddSystemLocalizedProperty(System_LocalizedPropertyDTO systemLocalizedProperty)
        {
            //var isExistUrl = "Fixture/IsProcessInfoExistAPI";
            //HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(systemLocalizedProperty, isExistUrl);
            //var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            //if (isExistResult.ToLower() == "true")
            //{
            //    return Content("EXIST");
            //}

            var apiUrl = "Settings/AddSystemLocalizedPropertyAPI";
            var entity = systemLocalizedProperty;
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult EditSystemLocalizedProperty(System_LocalizedPropertyDTO systemLocalizedProperty)
        {
            //var isExistUrl = "Fixture/IsProcessInfoExistAPI";
            //HttpResponseMessage isExistresponMessage = APIHelper.APIPostAsync(systemLocalizedProperty, isExistUrl);
            //var isExistResult = isExistresponMessage.Content.ReadAsStringAsync().Result;
            //if (isExistResult.ToLower() == "true")
            //{
            //    return Content("EXIST");
            //}

            var apiUrl = "Settings/EditSystemLocalizedPropertyAPI";
            var entity = systemLocalizedProperty;
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteSystemLocalizedProperty(int uid)
        {
            var apiUrl = string.Format("Settings/DeleteSystemLocalizedPropertyAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region Add By Rock 排程邮件设定 平安夜加班..... 2017/12/24
        public ActionResult BatchEmailSetting()
        {
            var vm = GetFixtureStatus();
            var isAdmin = false;
            var roleList = this.CurrentUser.GetUserInfo.RoleList.ToList();
            var roleNameList = roleList.Select(m => m.Role_ID).ToList();
            if (roleNameList.Contains("SystemAdmin"))
            {
                isAdmin = true;
            }
            int plantUID = 0;
            if (this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.Count() > 0)
            {
                plantUID = this.CurrentUser.GetUserInfo.Plant_OrganizationUIDList.First();
            }
            var url = string.Format("Settings/GetBatchEmailSettingFunctionAPI?isAdmin={0}&plantUID={1}", isAdmin, plantUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<SystemModuleEmailFunctionVM>>(result);
            ViewBag.FunctionList = list;
             
            return View(vm);
        }

        public ActionResult QueryBatchEmailSetting(SystemModuleEmailVM search, Page page)
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
            search.AccountUID = this.CurrentUser.AccountUId;

            var roleList = this.CurrentUser.GetUserInfo.RoleList.ToList();
            var roleNameList = roleList.Select(m => m.Role_ID).ToList();
            if (roleNameList.Contains("SystemAdmin"))
            {
                search.IsAdmin = true;
            }
            search.RoleUIDList = roleList.Select(m => m.Role_UID).ToList();

            var url = string.Format("Settings/QueryBatchEmailSettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string QueryBatchEmailSettingByEdit(int System_Email_Delivery_UID)
        {
            var url = string.Format("Settings/QueryBatchEmailSettingByEditAPI?System_Email_Delivery_UID={0}", System_Email_Delivery_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string ChangeNTID(string User_NTID)
        {
            var url = string.Format("Settings/ChangeNTIDAPI?User_NTID={0}", User_NTID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string SaveBatchEmailSetting(SystemModuleEmailVM item)
        {
            item.Created_UID = this.CurrentUser.AccountUId;
            item.Modified_UID = this.CurrentUser.AccountUId;
            var url = string.Format("Settings/SaveBatchEmailSettingAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(item, url);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }

        public string CheckEmailIsError(SystemModuleEmailVM item)
        {
            item.Created_UID = this.CurrentUser.AccountUId;
            item.Modified_UID = this.CurrentUser.AccountUId;
            var url = string.Format("Settings/CheckEmailIsErrorAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(item, url);
            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;

        }


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

        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }
        #endregion 平安夜加班..... 2017/12/24

    }
}
