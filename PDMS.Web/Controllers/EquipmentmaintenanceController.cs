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

namespace PDMS.Web.Controllers
{
    public class EquipmentmaintenanceController : WebControllerBase
    {

        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }
        #region  BasicInfo
        public ActionResult BasicInfo()
        {
            return View();
        }
        public ActionResult GetSafeStockparameter()
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetSafeStockparameterAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string EditSafeStocko(SafeStockDTO safestock)
        {
            var apiUrl = string.Empty;
            List<SafeStockDTO> dtolist = new List<SafeStockDTO>();
            dtolist.Add(safestock);
            var json = JsonConvert.SerializeObject(dtolist);
            apiUrl = string.Format("Equipmentmaintenance/EditSafeStockoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        #endregion
        #region  //Equipment
        public ActionResult EquipmentInfo()
        {
            EQPRoleVM currentVM = new EQPRoleVM();

            var roles = CurrentUser.GetUserInfo.RoleList;
            foreach (var item in roles)
            {
                currentVM.rolename += item.Role_ID + ";";
            }
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
            return View(currentVM);
        }

        public ActionResult EQPAllUserAPI(int bgUID, int funplantUID)
        {
            int plantUID = 0;
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() == "EQPUSER")
            {
                var apiUrl1 = string.Format("Equipmentmaintenance/QueryEQPUserAPI?EQPUser_Uid={0}", CurrentUser.AccountUId);
                HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);
                var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                var eqpusers = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result1);
                if (eqpusers.FirstOrDefault().BG_Organization_UID != null)
                {
                    bgUID = eqpusers.FirstOrDefault().BG_Organization_UID.Value;
                }
                else
                {
                    bgUID = 0;
                }
                if (eqpusers.FirstOrDefault().FunPlant_Organization_UID != null)
                {
                    funplantUID = eqpusers.FirstOrDefault().FunPlant_Organization_UID.Value;
                }
                else
                {
                    funplantUID = 0;
                }
                plantUID = eqpusers.FirstOrDefault().Plant_OrganizationUID;
            }
            if (CurrentUser.GetUserInfo != null && CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0)
            {
                if (CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                {
                    plantUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
                }
                else
                {
                    plantUID = 0;
                }

                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null)
                {
                    bgUID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                }
                else
                {
                    bgUID = 0;
                }
                if (CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID != null)
                {
                    funplantUID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                }
                else
                {
                    funplantUID = 0;
                }
            }

            var apiUrl = string.Format("Equipmentmaintenance/EQPAllUserAPI?bgUID={0}&funplantUID={1}&plantUID={2}", bgUID, funplantUID, plantUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetCurrentOPType(int plant_OrganizationUID)
        {
            int organization_UID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }


            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() == "EQPUSER")
            {

                var apiUrl1 = string.Format("Equipmentmaintenance/QueryEQPUserAPI?EQPUser_Uid={0}", CurrentUser.AccountUId);
                var responMessage1 = APIHelper.APIGetAsync(apiUrl1);
                var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                var eqpusers = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result1);
                if (eqpusers.FirstOrDefault().BG_Organization_UID != null)
                {
                    organization_UID = eqpusers.FirstOrDefault().BG_Organization_UID.Value;
                }

            }


            var apiUrl = string.Format("Equipmentmaintenance/GetCurrentOPTypeAPI?parentOrg_UID={0}&organization_UID={1}", plant_OrganizationUID, organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult QueryOpTypesByUser(int plant_OrganizationUID)
        {


            //int oporguid = 0;
            //if (currentVM.rolename.Contains("SystemAdmin;") || currentVM.rolename.Contains("EQPSuperAdimin;") ||
            //    (CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Department == "设备"))
            //{
            //    var apiUrl1 = string.Format("Equipmentmaintenance/QueryOpTypesByUserAPI?oporguid={0}", oporguid);
            //    var responMessage1 = APIHelper.APIGetAsync(apiUrl1);
            //    var result1 = responMessage1.Content.ReadAsStringAsync().Result;
            //    var optypes = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result1);
            //    currentVM.optypes = optypes;
            //}
            //else
            //{
            //    if (CurrentUser.GetUserInfo.OrgInfo.Count == 0)
            //        oporguid = -1;
            //    else
            //    {
            //        currentVM.optype = CurrentUser.GetUserInfo.OrgInfo[0].OPType;
            //        if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID == 0 || CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID == null)
            //            oporguid = -1;
            //        else
            //            oporguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID;
            //    }
            //    var apiUrl1 = string.Format("Equipmentmaintenance/QueryOpTypesByUserAPI?oporguid={0}", oporguid);
            //    var responMessage1 = APIHelper.APIGetAsync(apiUrl1);
            //    var result1 = responMessage1.Content.ReadAsStringAsync().Result;
            //    var optypes = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(result1);
            //    currentVM.optypes = optypes;
            //}

            int oporguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo == null)
            {
                //if (CurrentUser.GetUserInfo.OrgInfo.Count == 0)
                //{ 
                oporguid = 0;
                plant_OrganizationUID = oporguid;
                // }
            }

            else
            {

                if (CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID == 0 || CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID == null)

                {
                    oporguid = 0;
                    plant_OrganizationUID = oporguid;
                }
                else
                {
                    oporguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID;
                    plant_OrganizationUID = oporguid;
                }

            }
            int accuntid = 0;
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() == "EQPUSER")
            {
                accuntid = CurrentUser.GetUserInfo.Account_UID;
            }
            //CurrentUser.GetUserInfo.Account_UID
            var apiUrl = string.Format("Equipmentmaintenance/QueryOpTypesByUserAPI?oporguid={0}&accuntid={1}", plant_OrganizationUID, accuntid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //导出全部设备信息
        public ActionResult ExportALLEquipmentInfo(EquipmentInfoSearchDTO search)
        {
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.Organization_UID == null && organization_UID != 0)
            {
                search.Organization_UID = organization_UID;
            }

            var apiUrl = string.Format("Equipmentmaintenance/ExportALLEquipmentInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EquipmentInfoDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("EquipmentInfoReport");
            var stringHeads = GetAllEquipmentExportHead();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("EquipmentInfoReport");

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
                    worksheet.Cells[index + 2, 1].Value = index + 1;//序号
                    worksheet.Cells[index + 2, 2].Value = currentRecord.FunPlant;//功能厂
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Project_Name;//专案
                    worksheet.Cells[index + 2, 4].Value = currentRecord.OP_TYPES;//OP类型
                    worksheet.Cells[index + 2, 5].Value = currentRecord.process;//制程
                    worksheet.Cells[index + 2, 6].Value = currentRecord.EQP_Plant_No;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.CoCd;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.OpU;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.C;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Process_Group;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Class;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Mfg_Of_Asset;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Model_Number;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Mfg_Serial_Num;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Mfg_Part_Number;

                    worksheet.Cells[index + 2, 17].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Asset;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.AM_CostCtr;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Description_1;
                    worksheet.Cells[index + 2, 21].Value = currentRecord.User_Status;
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Func_Loc;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.EQP_Location;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.Room;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.Int_Note_L1;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.Int_Note_L2;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.Cap_date;
                    worksheet.Cells[index + 2, 28].Value = currentRecord.Acquisition_Value;
                    worksheet.Cells[index + 2, 29].Value = currentRecord.Asset_Life;
                    worksheet.Cells[index + 2, 30].Value = currentRecord.Net_Book_Value;
                    worksheet.Cells[index + 2, 31].Value = currentRecord.Monthly_Depreciation;
                    worksheet.Cells[index + 2, 32].Value = currentRecord.Remaining_Life;
                    worksheet.Cells[index + 2, 33].Value = currentRecord.Weight;
                    worksheet.Cells[index + 2, 34].Value = currentRecord.Un;
                    worksheet.Cells[index + 2, 35].Value = currentRecord.Size_dimension;
                    worksheet.Cells[index + 2, 36].Value = currentRecord.MCtry;
                    worksheet.Cells[index + 2, 37].Value = currentRecord.ConY;
                    worksheet.Cells[index + 2, 38].Value = currentRecord.CM;
                    worksheet.Cells[index + 2, 39].Value = currentRecord.Description_2;
                    worksheet.Cells[index + 2, 40].Value = currentRecord.Characteristic_1;
                    worksheet.Cells[index + 2, 41].Value = currentRecord.Description_3;
                    worksheet.Cells[index + 2, 42].Value = currentRecord.Characteristic_2;
                    worksheet.Cells[index + 2, 43].Value = currentRecord.Description_4;
                    worksheet.Cells[index + 2, 44].Value = currentRecord.Characteristic_3;
                    worksheet.Cells[index + 2, 45].Value = currentRecord.Description_5;
                    worksheet.Cells[index + 2, 46].Value = currentRecord.Characteristic_4;
                    worksheet.Cells[index + 2, 47].Value = currentRecord.Description_6;
                    worksheet.Cells[index + 2, 48].Value = currentRecord.Characteristic_5;
                    worksheet.Cells[index + 2, 49].Value = 0;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出勾选部分的设备信息
        public ActionResult ExportPartEquipmentInfo(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Equipmentmaintenance/ExportPartEquipmentInfoAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EquipmentInfoDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("EquipmentInfoReport");
            var stringHeads = GetAllEquipmentExportHead();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("EquipmentInfoReport");
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
                    worksheet.Cells[index + 2, 1].Value = index + 1;//序号
                    worksheet.Cells[index + 2, 2].Value = currentRecord.FunPlant;//功能厂
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Project_Name;//专案
                    worksheet.Cells[index + 2, 4].Value = currentRecord.OP_TYPES;//OP类型
                    worksheet.Cells[index + 2, 5].Value = currentRecord.process;//制程
                    worksheet.Cells[index + 2, 6].Value = currentRecord.EQP_Plant_No;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.CoCd;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.OpU;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.C;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Process_Group;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Class;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Mfg_Of_Asset;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Model_Number;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Mfg_Serial_Num;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Mfg_Part_Number;

                    worksheet.Cells[index + 2, 17].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Asset;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.AM_CostCtr;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Description_1;
                    worksheet.Cells[index + 2, 21].Value = currentRecord.User_Status;
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Func_Loc;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.EQP_Location;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.Room;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.Int_Note_L1;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.Int_Note_L2;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.Cap_date;
                    worksheet.Cells[index + 2, 28].Value = currentRecord.Acquisition_Value;
                    worksheet.Cells[index + 2, 29].Value = currentRecord.Asset_Life;
                    worksheet.Cells[index + 2, 30].Value = currentRecord.Net_Book_Value;
                    worksheet.Cells[index + 2, 31].Value = currentRecord.Monthly_Depreciation;
                    worksheet.Cells[index + 2, 32].Value = currentRecord.Remaining_Life;
                    worksheet.Cells[index + 2, 33].Value = currentRecord.Weight;
                    worksheet.Cells[index + 2, 34].Value = currentRecord.Un;
                    worksheet.Cells[index + 2, 35].Value = currentRecord.Size_dimension;
                    worksheet.Cells[index + 2, 36].Value = currentRecord.MCtry;
                    worksheet.Cells[index + 2, 37].Value = currentRecord.ConY;
                    worksheet.Cells[index + 2, 38].Value = currentRecord.CM;
                    worksheet.Cells[index + 2, 39].Value = currentRecord.Description_2;
                    worksheet.Cells[index + 2, 40].Value = currentRecord.Characteristic_1;
                    worksheet.Cells[index + 2, 41].Value = currentRecord.Description_3;
                    worksheet.Cells[index + 2, 42].Value = currentRecord.Characteristic_2;
                    worksheet.Cells[index + 2, 43].Value = currentRecord.Description_4;
                    worksheet.Cells[index + 2, 44].Value = currentRecord.Characteristic_3;
                    worksheet.Cells[index + 2, 45].Value = currentRecord.Description_5;
                    worksheet.Cells[index + 2, 46].Value = currentRecord.Characteristic_4;
                    worksheet.Cells[index + 2, 47].Value = currentRecord.Description_6;
                    worksheet.Cells[index + 2, 48].Value = currentRecord.Characteristic_5;
                    worksheet.Cells[index + 2, 49].Value = 0;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }

        /// <summary>
        /// 获取所有的设备导出的表头
        /// </summary>
        /// <returns></returns>
        private string[] GetAllEquipmentExportHead()
        {
            var stringHeads = new string[]
             {
            "序号",
            "功能厂",
            "专案",
            "OP类型",
            "制程",
            "厂内编号",
            "CoCd",
            "OpU",
            "C",
            "Process Group",
            "Class",
            "Class Desc",
            "Mfg of asset",
            "Model number",
            "Mfg Serial Num",
            "Mfg part number",
            "Equipment",
            "Asset",
            "AM-CostCtr",
            "Description_1",
            "User Status",
            "Func Loc Txt",
            "Location",
            "Room",
            "Int Note L1",
            "Int Note L2",
            "Cap.date",
            "Acquisition Value - Cum",
            "Asset Life",
            "Net Book Value - Cum",
            "Monthly Depreciation",
            "Remaining Life",
            "Weight",
            "Un.",
            "Size/dimension",
            "MCtry",
            "ConY",
            "CM",
            "Description_2",
            "Characteristic Value_1",
            "Description_3",
            "Characteristic Value_2",
            "Description_4",
            "Characteristic Value_3",
            "Description_5",
            "Characteristic Value_4",
            "Description_6",
            "Characteristic Value_5",
            "Inventory number",
             };

            return stringHeads;
        }
        public ActionResult QueryEquipmentInfo(EquipmentInfoSearchDTO search, Page page)
        {
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.Organization_UID == null && organization_UID != 0)
            {
                search.Organization_UID = organization_UID;
            }
            var apiUrl = string.Format("Equipmentmaintenance/QueryEquipmentInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryEquipmentInfoNOTReprot(EquipmentReport search, Page page)
        {
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.OPType_OrganizationUID == 0 && organization_UID != 0)
            {
                search.OPType_OrganizationUID = organization_UID;
            }
            var apiUrl = string.Format("Equipmentmaintenance/QueryEquipmentInfoNOTReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryEquipmentInfoReprot(EquipmentReport search, Page page)
        {
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.OPType_OrganizationUID == 0 && organization_UID != 0)
            {
                search.OPType_OrganizationUID = organization_UID;
            }
            var apiUrl = string.Format("Equipmentmaintenance/QueryEquipmentInfoReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        /// <summary>
        /// 新增，修改，编辑
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public string AddOrEditEquipmentInfo(EquipmentInfoDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            if (!string.IsNullOrEmpty(dto.ProductDate) && dto.ProductDate.Contains("-"))
            {
                var date = dto.ProductDate.Split('-');
                if (date.Count() > 1)
                {
                    dto.ConY = date[0];
                    dto.CM = date[1];
                }
            }
            string EQP_UID = Request.Form["EQP_Uid"];
            bool isEdit = Convert.ToBoolean(Request.Form["isEdit"]);
            //获取所有功能厂
            var plantAPI = "Equipmentmaintenance/GetOrganization_PlantsAPI";
            HttpResponseMessage plantmessage = APIHelper.APIGetAsync(plantAPI);
            var jsonPlants = plantmessage.Content.ReadAsStringAsync().Result;
            var functionPlants = JsonConvert.DeserializeObject<List<System_Organization_PlantDTO>>(jsonPlants);
            var functionPlant = functionPlants.FirstOrDefault(o => o.System_FunPlant_UID == dto.System_FunPlant_UID);
            if (functionPlant != null)
            {
                dto.Plant_Organization_UID = functionPlant.ParentOrg_UID;
                dto.BG_Organization_UID = functionPlant.OPType_OrganizationUID;
                dto.FunPlant_Organization_UID = functionPlant.FunPlant_OrganizationUID;
            }
            var apiUrl = string.Format("Equipmentmaintenance/EditOrEditEquipmentInfoAPI?EQP_Uid={0}&isEdit={1}", EQP_UID, isEdit);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
            //return RedirectToAction("EquipmentInfo", "Equipmentmaintenance");
        }

        public ActionResult QueryEquipmentInfoByUid(string EQP_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEquipmentAPI?EQP_UID={0}", EQP_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryAllOptypes()
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryAllProjectsAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryAllPlant(string optype)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryAllPlantAPI?optype={0}", optype);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryProjects(int oporgid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryProjectsAPI?oporgid={0}", oporgid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFunplants(int oporgid, string optypes = "")
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryFunplantsAPI?oporgid={0}&optypes={1}", oporgid, optypes);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportEquipmentExcel(HttpPostedFileBase upload_excel, string user_optype)
        {
            string errorInfo = string.Empty;
            errorInfo = AddEquipmentExcel(upload_excel, user_optype);
            return errorInfo;
        }

        private string[] GetEquipmentHeadColumn()
        {
            var stringHeads = new string[]
            {
            "功能厂",
            "专案",
            "OP类型",
            "制程",
            "厂内编号",
            "CoCd",
            "OpU",
            "C",
            "Process Group",
            "Class",
            "Class Desc",
            "Mfg of asset",
            "Model number",
            "Mfg Serial Num",
            "Mfg part number",
            "Equipment",
            "Asset",
            "AM-CostCtr",
            "Description_1",
            "User Status",
            "Func Loc Txt",
            "Location",
            "Room",
            "Int Note L1",
            "Int Note L2",
            "Cap.date",
            "Acquisition Value - Cum",
            "Asset Life",
            "Net Book Value - Cum",
            "Monthly Depreciation",
            "Remaining Life",
            "Weight",
            "Un.",
            "Size/dimension",
            "MCtry",
            "ConY",
            "CM",
            "Description_2",
            "Characteristic Value_1",
            "Description_3",
            "Characteristic Value_2",
            "Description_4",
            "Characteristic Value_3",
            "Description_5",
            "Characteristic Value_4",
            "Description_6",
            "Characteristic Value_5",
            "Inventory number",
            };

            return stringHeads;
        }

        private string AddEquipmentExcel(HttpPostedFileBase upload_excel, string user_optype)
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
                    var propertiesHead = GetEquipmentHeadColumn();

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

                    //读取Excel并进行验证判断
                    var plantAPI = "Equipmentmaintenance/QueryAllFunctionPlantsAPI";
                    //HttpResponseMessage message = APIHelper.APIPostAsync(null,plantAPI);
                    HttpResponseMessage plantmessage = APIHelper.APIGetAsync(plantAPI);
                    var jsonPlants = plantmessage.Content.ReadAsStringAsync().Result;
                    var functionPlants = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(jsonPlants);

                    var projectAPI = "Equipmentmaintenance/QueryAllProjectAPI";
                    HttpResponseMessage projectmessage = APIHelper.APIGetAsync(projectAPI);
                    var jsonProject = projectmessage.Content.ReadAsStringAsync().Result;
                    var functionProjects = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(jsonProject);

                    var equipmentAPI = "Equipmentmaintenance/QueryAllEquipmentAPI";
                    HttpResponseMessage equipmessage = APIHelper.APIGetAsync(equipmentAPI);
                    var jsonEquipment = equipmessage.Content.ReadAsStringAsync().Result;
                    var functionEquip = JsonConvert.DeserializeObject<List<EquipmentInfoDTO>>(jsonEquipment);

                    //获取所有功能厂 对应的厂区，OP，功能厂。
                    var Organization_PlantsAPI = "Equipmentmaintenance/GetOrganization_PlantsAPI";
                    HttpResponseMessage organization_Plantmessage = APIHelper.APIGetAsync(Organization_PlantsAPI);
                    var jsonorganization_Plants = organization_Plantmessage.Content.ReadAsStringAsync().Result;
                    var organization_Plants = JsonConvert.DeserializeObject<List<System_Organization_PlantDTO>>(jsonorganization_Plants);

                    List<EquipmentInfoDTO> equipmentlist = new List<EquipmentInfoDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        #region 导入设备信息
                        string Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Equipment")].Value);
                        if (string.IsNullOrWhiteSpace(Equipment))
                        {
                            continue;
                        }

                        EquipmentInfoDTO equipItem = new EquipmentInfoDTO();
                        int System_FunPlant_UID;
                        string optype = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        if (string.IsNullOrWhiteSpace(optype))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行OP类型值错误", i);
                            return errorInfo;
                        }
                        else if (!string.IsNullOrWhiteSpace(user_optype))
                        {
                            if (user_optype != optype)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行OP类型与登陆人员所属OP【" + user_optype + "】不一致", i);
                                return errorInfo;
                            }
                        }
                        string project = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案")].Value);

                        int Project_UID = 0;
                        if (!string.IsNullOrWhiteSpace(project))
                        {
                            var hasproject = functionProjects.FirstOrDefault(m => m.Project_Name == project&&m.OP_TYPES== optype);
                            if (hasproject != null)
                            {
                                Project_UID = hasproject.Project_UID;
                                equipItem.Project_UID = Project_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行专案值错误", i);
                                return errorInfo;
                            }
                        }
                        else
                        {
                            equipItem.Project_UID = Project_UID;
                        }

              
                        string plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        if (string.IsNullOrWhiteSpace(plant))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行功能厂值错误", i);
                            return errorInfo;
                        }

                        var hasplant = functionPlants.FirstOrDefault(m => m.FunPlant == plant & m.OP_Types == optype);
                        if (hasplant != null)
                        {
                            System_FunPlant_UID = hasplant.System_FunPlant_UID;
                            equipItem.System_FunPlant_UID = System_FunPlant_UID;
                            var organization_Plant = organization_Plants.FirstOrDefault(o => o.System_FunPlant_UID == System_FunPlant_UID);
                            if (organization_Plant != null)
                            {
                                equipItem.Plant_Organization_UID = organization_Plant.ParentOrg_UID;
                                equipItem.BG_Organization_UID = organization_Plant.OPType_OrganizationUID;
                                equipItem.FunPlant_Organization_UID = organization_Plant.FunPlant_OrganizationUID;
                            }

                        }
                        else
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行功能厂值错误或功能厂与op类型不匹配", i);
                            return errorInfo;
                        }
                        string process = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "制程")].Value);
                        if (!string.IsNullOrWhiteSpace(process))
                        {
                            process = process.Replace("'", "''");
                        }
                        if (process != null)
                            if (process.Length > 30)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行制程超过30个字符", i);
                                return errorInfo;
                            }
                        equipItem.process = process;

                        string Class_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Class Desc")].Value);
                        if (string.IsNullOrWhiteSpace(Class_Desc))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行机台类型(Class Desc)不可为空", i);
                            return errorInfo;
                        }
                        else
                        {
                            if (Class_Desc != null)
                                if (Class_Desc.Length > 50)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行机台类型(Class Desc)超过50字符", i);
                                    return errorInfo;
                                }
                            Class_Desc = Class_Desc.Replace("'", "''");
                            equipItem.Class_Desc = Class_Desc;
                        }


                        string asset = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Asset")].Value);
                        if (asset != null)
                            if (asset.Length > 30)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行Asset超过30字符", i);
                                return errorInfo;
                            }
                        if (!string.IsNullOrWhiteSpace(asset))
                            asset = asset.Replace("'", "''");

                        var hasEquipment = functionEquip.FirstOrDefault(m => m.Equipment == Equipment);
                        if (hasEquipment != null)
                        {
                            equipItem.EQP_Uid = hasEquipment.EQP_Uid;
                        }
                        {
                            string EQP_Plant_No = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂内编号")].Value);
                            if (EQP_Plant_No != null)
                                if (EQP_Plant_No.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行厂内编号超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(EQP_Plant_No))
                                EQP_Plant_No = EQP_Plant_No.Replace("'", "''");
                            equipItem.EQP_Plant_No = EQP_Plant_No;
                            string CoCd = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "CoCd")].Value);
                            if (CoCd != null)
                                if (CoCd.Length > 5)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行CoCd超过5字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(CoCd))
                                CoCd = CoCd.Replace("'", "''");
                            equipItem.CoCd = CoCd;
                            string OpU = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OpU")].Value);
                            if (OpU != null)
                                if (OpU.Length > 5)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行OpU超过5字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(OpU))
                                OpU = OpU.Replace("'", "''");
                            equipItem.OpU = OpU;
                            string C = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "C")].Value);
                            if (C != null)
                                if (C.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行C超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(C))
                                C = C.Replace("'", "''");
                            equipItem.C = C;
                            string Process_Group = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Process Group")].Value);
                            if (Process_Group != null)
                                if (Process_Group.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Process_Group超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Process_Group))
                                Process_Group = Process_Group.Replace("'", "''");
                            equipItem.Process_Group = Process_Group;
                            string Class = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Class")].Value);
                            if (Class != null)
                                if (Class.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Class超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Class))
                                Class = Class.Replace("'", "''");
                            equipItem.Class = Class;
                            string Mfg_Of_Asset = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Mfg of asset")].Value);
                            if (Mfg_Of_Asset != null)
                                if (Mfg_Of_Asset.Length > 30)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Mfg of asset超过30字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Mfg_Of_Asset))
                                Mfg_Of_Asset = Mfg_Of_Asset.Replace("'", "''");
                            equipItem.Mfg_Of_Asset = Mfg_Of_Asset;
                            string Model_Number = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Model number")].Value);
                            if (Model_Number != null)
                                if (Model_Number.Length > 30)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Model number超过30字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Model_Number))
                                Model_Number = Model_Number.Replace("'", "''");
                            equipItem.Model_Number = Model_Number;
                            string Mfg_Serial_Num = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Mfg Serial Num")].Value);
                            if (Mfg_Serial_Num != null)
                                if (Mfg_Serial_Num.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Mfg Serial Num超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Mfg_Serial_Num))
                                Mfg_Serial_Num = Mfg_Serial_Num.Replace("'", "''");
                            equipItem.Mfg_Serial_Num = Mfg_Serial_Num;

                            string Mfg_Part_Number = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Mfg part number")].Value);
                            if (Mfg_Part_Number != null)
                                if (Mfg_Part_Number.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Mfg part number超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Mfg_Part_Number))
                                Mfg_Part_Number = Mfg_Part_Number.Replace("'", "''");
                            equipItem.Mfg_Part_Number = Mfg_Part_Number;
                            equipItem.Equipment = Equipment;
                            equipItem.Asset = asset;

                            string User_Status = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "User Status")].Value);
                            if (User_Status != null)
                                if (User_Status.Length > 50)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行User Status超过50字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(User_Status))
                                User_Status = User_Status.Replace("'", "''");
                            equipItem.User_Status = User_Status;
                            string AM_CostCtr = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "AM-CostCtr")].Value);
                            if (AM_CostCtr != null)
                                if (AM_CostCtr.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行AM_CostCtr超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(AM_CostCtr))
                                AM_CostCtr = AM_CostCtr.Replace("'", "''");
                            equipItem.AM_CostCtr = AM_CostCtr;

                            string Description_1 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Description_1")].Value);
                            if (Description_1 != null)
                                if (Description_1.Length > 50)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Description_1超过50字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Description_1))
                                Description_1 = Description_1.Replace("'", "''");
                            equipItem.Description_1 = Description_1;

                            DateTime dateCap_date;
                            try
                            {
                                string Cap_date = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Cap.date")].Text);
                                if (Cap_date != "")
                                {
                                    dateCap_date = Convert.ToDateTime(Cap_date);
                                    equipItem.Cap_date = dateCap_date;
                                }
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行Cap.date值错误", i);
                                return errorInfo;
                            }
                            decimal decAcquisition_Value;
                            try
                            {
                                decAcquisition_Value = Convert.ToDecimal(ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Acquisition Value - Cum")].Value));
                                equipItem.Acquisition_Value = decAcquisition_Value;
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行decAcquisition Value值错误", i);
                                return errorInfo;
                            }
                            int intassetlife = 0;
                            try
                            {
                                intassetlife = Convert.ToInt32(ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Asset Life")].Value));
                                equipItem.Asset_Life = intassetlife;
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行Asset Life值错误", i);
                                return errorInfo;
                            }
                            decimal decNetBookValue;
                            try
                            {
                                decNetBookValue = Convert.ToDecimal(ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Net Book Value - Cum")].Value));
                                equipItem.Net_Book_Value = decNetBookValue;
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行Net Book Value - Cum值错误", i);
                                return errorInfo;
                            }
                            decimal decMonthly_Depreciation;
                            try
                            {
                                decMonthly_Depreciation = Convert.ToDecimal(ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Monthly Depreciation")].Value));
                                equipItem.Monthly_Depreciation = decMonthly_Depreciation;
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行Monthly Depreciation值错误", i);
                                return errorInfo;
                            }
                            int intRemaining_Life;
                            try
                            {
                                intRemaining_Life = Convert.ToInt32(ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Remaining Life")].Value));
                                equipItem.Remaining_Life = intRemaining_Life;
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行Remaining Life值错误", i);
                                return errorInfo;
                            }
                            string Func_Loc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Func Loc Txt")].Value);
                            if (Func_Loc != null)
                                if (Func_Loc.Length > 50)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Func_Loc超过50字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Func_Loc))
                                Func_Loc = Func_Loc.Replace("'", "''");
                            equipItem.Func_Loc = Func_Loc;
                            string Room = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Room")].Value);
                            if (Room != null)
                                if (Room.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Room超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Room))
                                Room = Room.Replace("'", "''");
                            equipItem.Room = Room;

                            string MCtry = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "MCtry")].Value);
                            if (MCtry != null)
                                if (MCtry.Length > 5)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行MCtry超过5字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(MCtry))
                                MCtry = MCtry.Replace("'", "''");
                            equipItem.MCtry = MCtry;
                            decimal decWeight;
                            try
                            {
                                decWeight = Convert.ToDecimal(ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Weight")].Value));
                                equipItem.Weight = decWeight;
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行Weight值错误", i);
                                return errorInfo;
                            }
                            string Un = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Un.")].Value);
                            if (Un != null)
                                if (Un.Length > 5)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Un超过5字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Un))
                                Un = Un.Replace("'", "''");
                            equipItem.Un = Un;
                            string Size_dimension = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Size/dimension")].Value);
                            if (Size_dimension != null)
                                if (Size_dimension.Length > 50)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Size_dimension超过50字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Size_dimension))
                                Size_dimension = Size_dimension.Replace("'", "''");
                            equipItem.Size_dimension = Size_dimension;
                            string ConY = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "ConY")].Value);
                            if (ConY != null)
                                if (ConY.Length > 5)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行ConY超过5字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(ConY))
                                ConY = ConY.Replace("'", "''");
                            equipItem.ConY = ConY;
                            string CM = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "CM")].Value);
                            if (CM != null)
                                if (CM.Length > 5)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行CM超过5字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(CM))
                                CM = CM.Replace("'", "''");
                            equipItem.CM = CM;
                            string Int_Note_L2 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Int Note L2")].Value);
                            if (Int_Note_L2 != null)
                                if (Int_Note_L2.Length > 50)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Int_Note_L2超过50字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Int_Note_L2))
                                Int_Note_L2 = Int_Note_L2.Replace("'", "''");
                            equipItem.Int_Note_L2 = Int_Note_L2;
                            string Description_2 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "C")].Value);
                            if (Description_2 != null)
                                if (Description_2.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行C超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Description_2))
                                Description_2 = Description_2.Replace("'", "''");
                            equipItem.Description_2 = Description_2;
                            string Characteristic_1 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Characteristic Value_1")].Value);
                            if (Characteristic_1 != null)
                                if (Characteristic_1.Length > 50)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Characteristic Value_1超过50字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Characteristic_1))
                                Characteristic_1 = Characteristic_1.Replace("'", "''");
                            equipItem.Characteristic_1 = Characteristic_1;
                            string Description_3 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Description_3")].Value);
                            if (Description_3 != null)
                                if (Description_3.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Description_3超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Description_3))
                                Description_3 = Description_3.Replace("'", "''");
                            equipItem.Description_3 = Description_3;
                            string Characteristic_2 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Characteristic Value_2")].Value);
                            if (Characteristic_2 != null)
                                if (Characteristic_2.Length > 30)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Characteristic Value_2超过30字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Characteristic_2))
                                Characteristic_2 = Characteristic_2.Replace("'", "''");
                            equipItem.Characteristic_2 = Characteristic_2;
                            string Description_4 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Description_4")].Value);
                            if (Description_4 != null)
                                if (Description_4.Length > 30)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Description_4超过30字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Description_4))
                                Description_4 = Description_4.Replace("'", "''");
                            equipItem.Description_4 = Description_4;
                            string Characteristic_3 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Characteristic Value_3")].Value);
                            if (Characteristic_3 != null)
                                if (Characteristic_3.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Characteristic Value_3超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Characteristic_3))
                                Characteristic_3 = Characteristic_3.Replace("'", "''");
                            equipItem.Characteristic_3 = Characteristic_3;
                            string Description_5 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Description_5")].Value);
                            if (Description_5 != null)
                                if (Description_5.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Description_5超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Description_5))
                                Description_5 = Description_5.Replace("'", "''");
                            equipItem.Description_5 = Description_5;
                            string Characteristic_4 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Characteristic Value_4")].Value);
                            if (Characteristic_4 != null)
                                if (Characteristic_4.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Characteristic Value_4超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Characteristic_4))
                                Characteristic_4 = Characteristic_4.Replace("'", "''");
                            equipItem.Characteristic_4 = Characteristic_4;
                            string Description_6 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Description_6")].Value);
                            if (Description_6 != null)
                                if (Description_6.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Description_6超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Description_6))
                                Description_6 = Description_6.Replace("'", "''");
                            equipItem.Description_6 = Description_6;
                            string Characteristic_5 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Characteristic Value_5")].Value);
                            if (Characteristic_5 != null)
                                if (Characteristic_5.Length > 10)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Characteristic Value_5超过10字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Characteristic_5))
                                Characteristic_5 = Characteristic_5.Replace("'", "''");
                            equipItem.Characteristic_5 = Characteristic_5;
                            string Int_Note_L1 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Int Note L1")].Value);
                            if (Int_Note_L1 != null)
                                if (Int_Note_L1.Length > 100)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行Int_Note_L1超过100字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(Int_Note_L1))
                                Int_Note_L1 = Int_Note_L1.Replace("'", "''");
                            equipItem.Int_Note_L1 = Int_Note_L1;
                            string EQP_Location = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "Location")].Value);
                            if (EQP_Location != null)
                                if (EQP_Location.Length > 20)
                                {
                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行EQP_Location超过20字符", i);
                                    return errorInfo;
                                }
                            if (!string.IsNullOrWhiteSpace(EQP_Location))
                                EQP_Location = EQP_Location.Replace("'", "''");
                            equipItem.EQP_Location = EQP_Location;
                        }

                        equipItem.Modified_UID = CurrentUser.AccountUId;
                        equipItem.Modified_Date = DateTime.Now;
                        equipmentlist.Add(equipItem);

                        #endregion
                        if (equipmentlist.Count == 500)
                        {
                            //插入表
                            var json = JsonConvert.SerializeObject(equipmentlist);
                            var apiInsertEquipmentUrl = string.Format("Equipmentmaintenance/InsertEquipmentAPI");
                            HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(json, apiInsertEquipmentUrl);
                            errorInfo = responMessage1.Content.ReadAsStringAsync().Result.Replace("\"", "");
                            if (errorInfo != "")
                                return errorInfo;
                            equipmentlist.Clear();
                        }
                    }
                    if (equipmentlist.Count > 0)
                    {
                        //插入表
                        var json2 = JsonConvert.SerializeObject(equipmentlist);
                        var apiInsertEquipmentUrl2 = string.Format("Equipmentmaintenance/InsertEquipmentAPI");
                        HttpResponseMessage responMessage2 = APIHelper.APIPostAsync(json2, apiInsertEquipmentUrl2);
                        errorInfo = responMessage2.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    }
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.ToString();
            }
            return errorInfo;
        }

        private void SetEquipmentExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
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
            worksheet.Cells[1, 14].Value = propertiesHead[13];
            worksheet.Cells[1, 15].Value = propertiesHead[14];
            worksheet.Cells[1, 16].Value = propertiesHead[15];
            worksheet.Cells[1, 17].Value = propertiesHead[16];
            worksheet.Cells[1, 18].Value = propertiesHead[17];
            worksheet.Cells[1, 19].Value = propertiesHead[18];
            worksheet.Cells[1, 20].Value = propertiesHead[19];
            worksheet.Cells[1, 21].Value = propertiesHead[20];
            worksheet.Cells[1, 22].Value = propertiesHead[21];
            worksheet.Cells[1, 23].Value = propertiesHead[22];
            worksheet.Cells[1, 24].Value = propertiesHead[23];
            worksheet.Cells[1, 25].Value = propertiesHead[24];
            worksheet.Cells[1, 26].Value = propertiesHead[25];
            worksheet.Cells[1, 27].Value = propertiesHead[26];
            worksheet.Cells[1, 28].Value = propertiesHead[27];
            worksheet.Cells[1, 29].Value = propertiesHead[28];
            worksheet.Cells[1, 30].Value = propertiesHead[29];
            worksheet.Cells[1, 31].Value = propertiesHead[30];
            worksheet.Cells[1, 32].Value = propertiesHead[31];
            worksheet.Cells[1, 33].Value = propertiesHead[32];
            worksheet.Cells[1, 34].Value = propertiesHead[33];
            worksheet.Cells[1, 35].Value = propertiesHead[34];
            worksheet.Cells[1, 36].Value = propertiesHead[35];
            worksheet.Cells[1, 37].Value = propertiesHead[36];
            worksheet.Cells[1, 38].Value = propertiesHead[37];
            worksheet.Cells[1, 39].Value = propertiesHead[38];
            worksheet.Cells[1, 40].Value = propertiesHead[39];
            worksheet.Cells[1, 41].Value = propertiesHead[40];
            worksheet.Cells[1, 42].Value = propertiesHead[41];
            worksheet.Cells[1, 43].Value = propertiesHead[42];
            worksheet.Cells[1, 44].Value = propertiesHead[43];
            worksheet.Cells[1, 45].Value = propertiesHead[44];
            worksheet.Cells[1, 46].Value = propertiesHead[45];
            worksheet.Cells[1, 47].Value = propertiesHead[46];
            worksheet.Cells[1, 48].Value = propertiesHead[47];
            //设置列宽
            for (int i = 1; i <= 48; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:AV1"].Style.Font.Bold = true;
            worksheet.Cells["A1:AV1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:AV1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }

        public FileResult DownloadEquipmentExcel()
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("导入设备信息模板");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetEquipmentHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("导入设备信息模板");
                SetEquipmentExcelStyle(worksheet, propertiesHead);
                int maxRow = 1;
                //设置灰色背景
                var colorRange = string.Format("A1:AV{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:AV{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:AV{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:AV{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("导入设备信息模板说明");
                SetEquipmentExcelStyle(worksheetsm, propertiesHead);
                SetEquipmentExcelStylesm(worksheetsm);
                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:AV{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:AV{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:AV{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;



                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        private void SetEquipmentExcelStylesm(ExcelWorksheet worksheet)
        {

            //"功能厂",
            //"专案",
            //"OP类型",
            //"制程",
            //"厂内编号",
            //"CoCd",
            //"OpU",
            //"C",
            //"Process Group",
            //"Class",
            //"Class Desc",
            //"Mfg of asset",
            //"Model number",
            //"Mfg Serial Num",
            //"Mfg part number",
            //"Equipment",
            //"Asset",

            //"AM-CostCtr",
            //"Description_1",
            //"User Status",
            //"Func Loc Txt",
            //"Location",
            //"Room",
            //"Int Note L1",
            //"Int Note L2",
            //"Cap.date",
            //"Acquisition Value - Cum",
            //"Asset Life",
            //"Net Book Value - Cum",
            //"Monthly Depreciation",
            //"Remaining Life",
            //"Weight",
            //"Un.",
            //"Size/dimension",
            //"MCtry",
            //"ConY",
            //"CM",
            //"Description_2",
            //"Characteristic Value_1",
            //"Description_3",
            //"Characteristic Value_2",
            //"Description_4",
            //"Characteristic Value_3",
            //"Description_5",
            //"Characteristic Value_4",
            //"Description_6",
            //"Characteristic Value_5",
            //"Inventory number",




            //填充Title内容
            worksheet.Cells[2, 1].Value = "小于50个字符";
            worksheet.Cells[2, 2].Value = "小于30个字符";
            worksheet.Cells[2, 3].Value = "小于50个字符";
            worksheet.Cells[2, 4].Value = "小于30个字符";
            worksheet.Cells[2, 5].Value = "小于20个字符";

            worksheet.Cells[2, 6].Value = "小于5个字符";
            worksheet.Cells[2, 7].Value = "小于5个字符";
            worksheet.Cells[2, 8].Value = "小于5个字符";
            worksheet.Cells[2, 9].Value = "小于20个字符";
            worksheet.Cells[2, 10].Value = "小于20个字符";
            worksheet.Cells[2, 11].Value = "小于50个字符";

            worksheet.Cells[2, 12].Value = "小于30个字符";
            worksheet.Cells[2, 13].Value = "小于30个字符";
            worksheet.Cells[2, 14].Value = "小于20个字符";
            worksheet.Cells[2, 15].Value = "小于20个字符";
            worksheet.Cells[2, 16].Value = "小于20个字符";
            worksheet.Cells[2, 17].Value = "小于10个字符";
            worksheet.Cells[2, 18].Value = "小于10个字符";
            worksheet.Cells[2, 19].Value = "小于50个字符";
            worksheet.Cells[2, 20].Value = "小于50个字符";
            worksheet.Cells[2, 21].Value = "小于50个字符";
            worksheet.Cells[2, 22].Value = "小于20个字符";
            worksheet.Cells[2, 23].Value = "小于10个字符";
            worksheet.Cells[2, 24].Value = "小于100个字符";
            worksheet.Cells[2, 25].Value = "小于50个字符";
            worksheet.Cells[2, 26].Value = "时间";
            worksheet.Cells[2, 27].Value = "数字格式";
            worksheet.Cells[2, 28].Value = "数字格式";
            worksheet.Cells[2, 29].Value = "数字格式";
            worksheet.Cells[2, 30].Value = "数字格式";
            worksheet.Cells[2, 31].Value = "数字格式";
            worksheet.Cells[2, 32].Value = "数字格式";
            worksheet.Cells[2, 33].Value = "小于5个字符";
            worksheet.Cells[2, 34].Value = "小于50个字符";
            worksheet.Cells[2, 35].Value = "小于5个字符";
            worksheet.Cells[2, 36].Value = "小于5个字符";
            worksheet.Cells[2, 37].Value = "小于5个字符";
            worksheet.Cells[2, 38].Value = "小于20个字符";
            worksheet.Cells[2, 39].Value = "小于50个字符";
            worksheet.Cells[2, 40].Value = "小于10个字符";
            worksheet.Cells[2, 41].Value = "小于30个字符";
            worksheet.Cells[2, 42].Value = "小于30个字符";
            worksheet.Cells[2, 43].Value = "小于10个字符";
            worksheet.Cells[2, 44].Value = "小于10个字符";
            worksheet.Cells[2, 45].Value = "小于10个字符";
            worksheet.Cells[2, 46].Value = "小于10个字符";
            worksheet.Cells[2, 47].Value = "小于10个字符";
            worksheet.Cells[2, 48].Value = "小于20个字符";
            //设置列宽
            for (int i = 1; i <= 48; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:AV1"].Style.Font.Bold = true;
            worksheet.Cells["A1:AV1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:AV1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
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

        public string DeleteEquipment(int EQP_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteEquipmentAPI?EQP_Uid={0}", EQP_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public ActionResult Checkoptypesandplant(string types, string plantname)
        {
            var apiUrl = string.Format("Equipmentmaintenance/CheckoptypesandplantAPI?types={0}&plantname={1}", types, plantname);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region  EQP_UserTable
        public ActionResult EQPUser()
        {

            var apiUrl = "Equipmentmaintenance/EQPMaintenanceAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var errorinfo = JsonConvert.DeserializeObject<IEnumerable<EnumerationDTO>>(result);
            EQPRepairSearchVM currentVM = new EQPRepairSearchVM();

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {

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

            return View("EQPUser", currentVM);

        }

        public ActionResult QueryEQPUser(EQPUserTableDTO search, Page page)
        {

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                search.Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if ((search.Organization_UID == null || search.Organization_UID == 0) && organization_UID != 0)
            {
                search.Organization_UID = organization_UID;
            }

            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPUserAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryEQPUserByUid(int EQPUser_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPUserAPI?EQPUser_Uid={0}", EQPUser_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string DeleteEQPUser(int EQPUser_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteEQPUserAPI?EQPUser_Uid={0}", EQPUser_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string AddOrEditEQPUser(EQPUserTableDTO dto, bool isEdit)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.EQPUser_Uid = dto.EQPUser_Uid;
            var apiUrl = string.Format("Equipmentmaintenance/AddOrEditEQPUserAPI?isEdit={0}", isEdit);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        private string[] GetEQPUserHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "用户工号",
                "用户姓名",
                "联系电话",
                "联系邮箱",
                "是否启用"
            };
            return propertiesHead;
        }

        private void SetEQPUserExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
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
            //设置列宽
            worksheet.Column(1).Width = 17;
            worksheet.Column(2).Width = 17;
            worksheet.Column(3).Width = 17;
            worksheet.Column(4).Width = 17;
            worksheet.Column(5).Width = 17;
            worksheet.Column(6).Width = 17;
            worksheet.Column(7).Width = 17;
            worksheet.Column(8).Width = 17;

            worksheet.Cells["A1:H1"].Style.Font.Bold = true;
            worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }

        public FileResult DownloadEQPUserExcel()
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("导入人员账号");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetEQPUserHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("导入人员账号");
                SetEQPUserExcelStyle(worksheet, propertiesHead);
                //设置灰色背景
                var colorRange = "A1:H1";
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1: H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1: H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1: H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("导入人员账号说明");
                SetEQPUserExcelStyle(worksheetsm, propertiesHead);
                SetEQPUserExcelStylesm(worksheetsm);
                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells["A1: H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells["A1: H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells["A1: H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetEQPUserExcelStylesm(ExcelWorksheet worksheet)
        {
            //填充Title内容
            worksheet.Cells[2, 1].Value = "CTU_M(成都厂)";
            worksheet.Cells[2, 2].Value = "OP2(OP类型)";
            worksheet.Cells[2, 3].Value = "CNC(功能厂)";
            worksheet.Cells[2, 4].Value = "12121(用户工号)";
            worksheet.Cells[2, 5].Value = "张三(用户姓名)";
            worksheet.Cells[2, 6].Value = "028-88812**(联系电话)";
            worksheet.Cells[2, 7].Value = "Jabil@.com(电子邮箱)";

        }
        public string ImportEQPUserExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddEQPUserExcel(upload_excel);
            return errorInfo;
        }

        private string AddEQPUserExcel(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetEQPUserHeadColumn();
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
                    //读取Excel并进行验证判断
                    var projectAPI = "Equipmentmaintenance/QueryAllEQPUserAPI";
                    HttpResponseMessage eqpusermessage = APIHelper.APIGetAsync(projectAPI);
                    var jsonProject = eqpusermessage.Content.ReadAsStringAsync().Result;
                    var functionEqpuser = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(jsonProject);

                    //获取所有的部门
                    var systemOrgDTOAPI = "Equipmentmaintenance/GetSystem_OrganizationAPI";
                    HttpResponseMessage systemOrgmessage = APIHelper.APIGetAsync(systemOrgDTOAPI);
                    var jsonsystemOrgDTO = systemOrgmessage.Content.ReadAsStringAsync().Result;
                    var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(jsonsystemOrgDTO);
                    //获取所有的部门层级关系
                    var systemOrgBomDTOAPI = "Equipmentmaintenance/GetSystem_OrganizationBOMAPI";
                    HttpResponseMessage systemOrgBommessage = APIHelper.APIGetAsync(systemOrgBomDTOAPI);
                    var jsonsystemOrgBomDTO = systemOrgBommessage.Content.ReadAsStringAsync().Result;
                    var systemOrgBomDTOs = JsonConvert.DeserializeObject<List<SystemOrgBomDTO>>(jsonsystemOrgBomDTO);


                    List<EQPUserTableDTO> eqpuserlist = new List<EQPUserTableDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        EQPUserTableDTO eqpuseritem = new EQPUserTableDTO();
                        int intuserid = 0;
                        //对厂区，OP类型，功能厂进行验证   对 BG_Organization_UID，FunPlant_Organization_UID 赋值。
                        int Plant_Organization_UID = 0;
                        int? BG_Organization_UID = null;
                        int? FunPlant_Organization_UID = null;
                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var systemOrgDTO = systemOrgDTOs.Where(o => o.Organization_Name == Plant_Organization).FirstOrDefault();

                            if (systemOrgDTO != null)
                            {
                                Plant_Organization_UID = systemOrgDTO.Organization_UID;
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

                            //excelIsError = true;
                            //errorInfo = string.Format("第{0}行OP类型没有值", i);
                            //return errorInfo;
                        }
                        else
                        {
                            List<int> opIDS = systemOrgBomDTOs.Where(o => o.ParentOrg_UID == Plant_Organization_UID).Select(o => o.ChildOrg_UID).ToList();
                            if (opIDS.Count > 0)
                            {
                                var systemOrgDTO = systemOrgDTOs.Where(o => o.Organization_Name == BG_Organization && opIDS.Contains(o.Organization_UID)).FirstOrDefault();
                                if (systemOrgDTO != null)
                                {
                                    BG_Organization_UID = systemOrgDTO.Organization_UID;
                                    var systemOrgBomDTO = systemOrgBomDTOs.Where(o => o.ParentOrg_UID == Plant_Organization_UID && o.ChildOrg_UID == BG_Organization_UID).FirstOrDefault();
                                    if (systemOrgBomDTO == null)
                                    {

                                        excelIsError = true;
                                        errorInfo = string.Format("第{0}行的厂区和OP类型没有组织关系", i);
                                        return errorInfo;
                                    }
                                }
                                else
                                {

                                    excelIsError = true;
                                    errorInfo = string.Format("第{0}行厂区下没有找到OP类型", i);
                                    return errorInfo;
                                }
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行厂区下没有找到OP类型", i);
                                return errorInfo;
                            }

                            // BG_Organization_UID
                        }
                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            //excelIsError = true;
                            //errorInfo = string.Format("第{0}行功能厂没有值", i);
                            //return errorInfo;
                        }
                        else
                        {
                            //得到当前厂区当前功能厂下的三级部门的ID

                            if (BG_Organization_UID != null)
                            {
                                var opChildIDS = systemOrgBomDTOs.Where(o => o.ParentOrg_UID == BG_Organization_UID).Select(o => o.ChildOrg_UID).ToList();
                                if (opChildIDS.Count > 0)
                                {
                                    //得到当前厂区，当前功能厂下Organization_Name为OP的部门
                                    var systemOrgDTO = systemOrgDTOs.Where(o => o.Organization_Name == "OP" && opChildIDS.Contains(o.Organization_UID)).FirstOrDefault();
                                    if (systemOrgDTO != null)
                                    {
                                        //找到的4级部门ID
                                        var funPlantChildIDS = systemOrgBomDTOs.Where(o => o.ParentOrg_UID == systemOrgDTO.Organization_UID).Select(o => o.ChildOrg_UID).ToList();
                                        if (funPlantChildIDS != null && funPlantChildIDS.Count > 0)
                                        {
                                            var funPlantsystemOrgDTO = systemOrgDTOs.Where(o => o.Organization_Name == FunPlant_Organization && funPlantChildIDS.Contains(o.Organization_UID)).FirstOrDefault();
                                            if (funPlantsystemOrgDTO != null)
                                            {
                                                FunPlant_Organization_UID = funPlantsystemOrgDTO.Organization_UID;
                                            }

                                        }

                                    }
                                    else
                                    {
                                        //excelIsError = true;
                                        //errorInfo = string.Format("第{0}行没有找到功能厂的层级关系", i);
                                        //return errorInfo;
                                    }

                                }
                            }

                            // FunPlant_Organization_UID
                        }

                        string User_Id = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "用户工号")].Value);
                        try
                        {
                            intuserid = Convert.ToInt32(User_Id);
                        }
                        catch
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行用户工号输入错误", i);
                            return errorInfo;
                        }
                        if (string.IsNullOrWhiteSpace(User_Id))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行用户工号没有值", i);
                            return errorInfo;
                        }

                        string User_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "用户姓名")].Value);
                        if (string.IsNullOrWhiteSpace(User_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行用户姓名没有值", i);
                            return errorInfo;
                        }

                        string User_Call = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "联系电话")].Value);
                        //if (string.IsNullOrWhiteSpace(User_Call))
                        //{
                        //    excelIsError = true;
                        //    errorInfo = string.Format("第{0}行联系电话没有值", i);
                        //    return errorInfo;
                        //}

                        string User_Email = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "联系邮箱")].Value);
                        //if (string.IsNullOrWhiteSpace(User_Email))
                        //{
                        //    excelIsError = true;
                        //    errorInfo = string.Format("第{0}行联系邮箱没有值", i);
                        //    return errorInfo;
                        //}

                        string IsDisableName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrEmpty(IsDisableName) || (IsDisableName != "启用" && IsDisableName != "禁用"))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行启用禁用栏位输入错误", i);
                            return errorInfo;
                        }

                        int IsDisable = IsDisableName == "启用" ? 1 : 0;

                        var hasEqpuser = functionEqpuser.FirstOrDefault(m => m.User_Id == intuserid);
                        if (hasEqpuser != null)
                            eqpuseritem.EQPUser_Uid = hasEqpuser.EQPUser_Uid;

                        var hasitem = eqpuserlist.FirstOrDefault(m => m.User_Id == intuserid);
                        if (hasitem != null)
                        {
                            errorInfo = string.Format("用户工号[{0}]重复", User_Id);
                            return errorInfo;
                        }
                        string userid_name = User_Id + "_" + User_Name;
                        eqpuseritem.User_Id = intuserid;
                        eqpuseritem.BG_Organization_UID = BG_Organization_UID;
                        eqpuseritem.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        eqpuseritem.User_Name = User_Name;
                        eqpuseritem.User_IdAndName = userid_name;
                        eqpuseritem.User_Email = User_Email;
                        eqpuseritem.User_Call = User_Call;
                        eqpuseritem.Modified_UID = CurrentUser.AccountUId;
                        eqpuseritem.Plant_OrganizationUID = Plant_Organization_UID;
                        eqpuseritem.IsDisable = IsDisable;
                        eqpuserlist.Add(eqpuseritem);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(eqpuserlist);
                    var apiInsertEquipmentUrl = string.Format("Equipmentmaintenance/InsertEQPUserAPI");
                    HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(json, apiInsertEquipmentUrl);
                    errorInfo = responMessage1.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.ToString();
            }
            return errorInfo;
        }
        #endregion

        #region  material 

        public ActionResult MaterialInfo()
        {
            MaterialVM currentVM = new MaterialVM();
            //var apiUrl = string.Format("StorageManage/QueryAllWarehouseStAPI");
            //var responMessage = APIHelper.APIGetAsync(apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result;
            //var warst = JsonConvert.DeserializeObject<List<WarehouseStorageDTO>>(result);
            //currentVM.warst = warst;

            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;
            //if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            //{

            //    currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            //}
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
            //價格權限管制，僅設備管理員有編輯權限，資產管理員有顯示權限
            currentVM.showPrice = false;
            currentVM.editPrice = false;
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() != "EQPUSER")
            {
                if (CurrentUser.GetUserInfo.RoleList != null)
                {
                    //TODO steven 修改只有設備管理員可看可編輯
                    if (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID.Trim() == "EQPAdmin").Count() > 0)
                    {
                        currentVM.showPrice = true;
                        currentVM.editPrice = true;
                    }
                    //else if (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID.Trim() == "AssetManager").Count() > 0)
                    //{
                    //    currentVM.showPrice = true;
                    //}
                }
            }
            return View("MaterialInfo", currentVM);
        }
        public ActionResult QueryAllWarehouseSt(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var apiUrl = string.Format("StorageManage/GetAllWarehouseStAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryMaterial(MaterialInfoDTO search, Page page)
        {
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                search.PlantId = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            var apiUrl = string.Format("Equipmentmaintenance/QueryMaterialAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult DoAllExportMaterialReprot(MaterialInfoDTO search)
        {

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                search.PlantId = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            var apiUrl = string.Format("Equipmentmaintenance/DoAllExportMaterialReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Material");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            //價格權限管制，僅設備管理員、資產管理員有顯示權限
            var showPrice = false;
            var minusCell = 1;
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() != "EQPUSER")
            {
                if (CurrentUser.GetUserInfo.RoleList != null)
                {
                    //if (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID.Trim() == "EQPAdmin" || x.Role_ID.Trim() == "AssetManager").Count() > 0)
                    //{
                    //    showPrice = true;
                    //    minusCell = 0;
                    //}
                    if (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID.Trim() == "EQPAdmin" ).Count() > 0)
                    {
                        showPrice = true;
                        minusCell = 0;
                    }
                }
            }

            //var stringHeads = new string[] { "排序", "厂区", "料号", "品名", "型号", "归类", "交期", "单价", "累计入库量", "预设仓库", "预设料架", "预设储位", "成本中心", "保养周期",
            //"备品寿命","请购周期","签核天数","日用量","每月单机用量","是否计算返修","是否启用"};
            var stringHeads = new string[] { "排序", "厂区", "料号", "品名", "型号", "归类", "预设仓库", "预设料架", "预设储位", "成本中心",
                "交期", "单价", "累计入库量",  "保养周期", "备品寿命","请购周期","签核天数","日用量","每月单机用量","是否计算返修","是否启用"};

            //沒有顯示金額權限移除金額欄位
            if (!showPrice) {
                stringHeads = stringHeads.Where(x => x != "单价").ToArray();
            }

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Material");

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
                    //worksheet.Cells[index + 2, 1].Value = index + 1;
                    //worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    //worksheet.Cells[index + 2, 3].Value = currentRecord.Material_Id;
                    //worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Name;
                    //worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Types;
                    //worksheet.Cells[index + 2, 6].Value = currentRecord.Classification;
                    //worksheet.Cells[index + 2, 7].Value = currentRecord.Delivery_Date;
                    //worksheet.Cells[index + 2, 8].Value = currentRecord.Unit_Price;
                    //worksheet.Cells[index + 2, 9].Value = currentRecord.Warehouse_ID;
                    //worksheet.Cells[index + 2, 10].Value = currentRecord.Rack_ID;
                    //worksheet.Cells[index + 2, 11].Value = currentRecord.Storage_ID;
                    //worksheet.Cells[index + 2, 12].Value = currentRecord.Cost_Center;
                    //worksheet.Cells[index + 2, 13].Value = currentRecord.Maintenance_Cycle;// == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    //worksheet.Cells[index + 2, 14].Value = currentRecord.Material_Life;
                    //worksheet.Cells[index + 2, 15].Value = currentRecord.Requisitions_Cycle;
                    //worksheet.Cells[index + 2, 16].Value = currentRecord.Sign_days;
                    //worksheet.Cells[index + 2, 17].Value = currentRecord.Daily_Consumption;
                    //worksheet.Cells[index + 2, 18].Value = currentRecord.Monthly_Consumption;
                    //worksheet.Cells[index + 2, 19].Value = currentRecord.IsRework.Value ? "Y" : "N"; ;
                    //worksheet.Cells[index + 2, 20].Value = currentRecord.Is_Enable.Value ? "Y" : "N"; ;

                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.PlantName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Classification;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Cost_Center;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Delivery_Date;
                    //金額權限管控，若無權限則移除且後續欄位往前移1欄
                    if (showPrice)
                    {
                        worksheet.Cells[index + 2, 12].Value = currentRecord.Unit_Price;
                    }
                    worksheet.Cells[index + 2, 13 - minusCell].Value = currentRecord.Last_Qty;
                    worksheet.Cells[index + 2, 14 - minusCell].Value = currentRecord.Maintenance_Cycle;// == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 15 - minusCell].Value = currentRecord.Material_Life;
                    worksheet.Cells[index + 2, 16 - minusCell].Value = currentRecord.Requisitions_Cycle;
                    worksheet.Cells[index + 2, 17 - minusCell].Value = currentRecord.Sign_days;
                    worksheet.Cells[index + 2, 18 - minusCell].Value = currentRecord.Daily_Consumption;
                    worksheet.Cells[index + 2, 19 - minusCell].Value = currentRecord.Monthly_Consumption;
                    worksheet.Cells[index + 2, 20 - minusCell].Value = currentRecord.IsRework.Value ? "Y" : "N"; ;
                    worksheet.Cells[index + 2, 21 - minusCell].Value = currentRecord.Is_Enable.Value ? "Y" : "N"; ;

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportMaterialReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Equipmentmaintenance/DoExportMaterialReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Material");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);

            //價格權限管制，僅設備管理員、資產管理員有顯示權限
            var showPrice = false;
            var minusCell = 1;
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() != "EQPUSER")
            {
                if (CurrentUser.GetUserInfo.RoleList != null)
                {
                    //if (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID.Trim() == "EQPAdmin" || x.Role_ID.Trim() == "AssetManager").Count() > 0)
                    //{
                    //    showPrice = true;
                    //    minusCell = 0;
                    //}
                   if (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID.Trim() == "EQPAdmin").Count() > 0)
                    {
                        showPrice = true;
                        minusCell = 0;
                    }
                }
            }

            //"厂区",
            //"料号",
            //"品名",
            //"型号",
            //"归类",
            //"预设仓库",
            //"预设料架",
            //"预设储位",
            //"成本中心",
            //"交期",
            //"单价",
            //"累计入库量",
            //"保养周期",
            //"备品寿命",
            //"请购周期",
            //"签核天数",
            //"日用量",
            //"每月单机用量",
            //"是否计算返修",
            //"是否启用"

            var stringHeads = new string[] { "排序", "厂区", "料号", "品名", "型号", "归类", "预设仓库", "预设料架", "预设储位", "成本中心",
                "交期", "单价", "累计入库量",  "保养周期", "备品寿命","请购周期","签核天数","日用量","每月单机用量","是否计算返修","是否启用"};

            //沒有顯示金額權限移除金額欄位
            if (!showPrice)
            {
                stringHeads = stringHeads.Where(x => x != "单价").ToArray();
            }

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Material");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Classification;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Cost_Center;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Delivery_Date;
                    //金額權限管控，若無權限則移除且後續欄位往前移1欄
                    if (showPrice)
                    {
                        worksheet.Cells[index + 2, 12].Value = currentRecord.Unit_Price;
                    }
                    worksheet.Cells[index + 2, 13 - minusCell].Value = currentRecord.Last_Qty;
                    worksheet.Cells[index + 2, 14 - minusCell].Value = currentRecord.Maintenance_Cycle;// == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 15 - minusCell].Value = currentRecord.Material_Life;
                    worksheet.Cells[index + 2, 16 - minusCell].Value = currentRecord.Requisitions_Cycle;
                    worksheet.Cells[index + 2, 17 - minusCell].Value = currentRecord.Sign_days;
                    worksheet.Cells[index + 2, 18 - minusCell].Value = currentRecord.Daily_Consumption;
                    worksheet.Cells[index + 2, 19 - minusCell].Value = currentRecord.Monthly_Consumption;
                    worksheet.Cells[index + 2, 20 - minusCell].Value = currentRecord.IsRework.Value ? "Y" : "N"; ;
                    worksheet.Cells[index + 2, 21 - minusCell].Value = currentRecord.Is_Enable.Value ? "Y" : "N"; ;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }


        public ActionResult QueryMaterialByUid(int Material_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryMaterialByUidAPI?Material_Uid={0}", Material_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditMaterialInfo(MaterialInfoDTO dto, bool isEdit)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Material_Uid = dto.Material_Uid;
            var apiUrl = string.Format("Equipmentmaintenance/AddOrEditMaterialInfoAPI?isEdit={0}", isEdit);
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string ImportMaterialInfoExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddMaterialInfoExcel(upload_excel);
            return errorInfo;
        }

        private string[] GetMaterialInforHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "料号",
                "品名",
                "型号",
                "归类",
                "预设仓库",
                "预设料架",
                "预设储位",
                "成本中心",
                "交期",
                "单价",
                "保养周期",
                "备品寿命",
                "请购周期",
                "签核天数",
                "日用量",
                "每月单机用量",
                "是否计算返修",
                "是否启用"
            };
            return propertiesHead;
        }

        private string AddMaterialInfoExcel(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetMaterialInforHeadColumn();
                    bool excelIsError = false;
                    if (totalColumns != propertiesHead.Length)
                    {
                        return "Excel格式不正确";
                    }
                    var PlantVMs = GetPlatVM();
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
                    //读取Excel并进行验证判断
                    var projectAPI = "Equipmentmaintenance/QueryAllImportMaterialInfoAPI";
                    HttpResponseMessage eqpusermessage = APIHelper.APIGetAsync(projectAPI);
                    var jsonProject = eqpusermessage.Content.ReadAsStringAsync().Result;
                    var functionmaterial = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(jsonProject);

                    var warehouseStAPI = "StorageManage/QueryAllWarehouseStAPI";
                    HttpResponseMessage WarhouseStmessage = APIHelper.APIGetAsync(warehouseStAPI);
                    var jsonwarst = WarhouseStmessage.Content.ReadAsStringAsync().Result;
                    var warehousest = JsonConvert.DeserializeObject<List<WarehouseStorageDTO>>(jsonwarst);

                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                    List<MaterialInfoDTO> eqpmateriallist = new List<MaterialInfoDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        MaterialInfoDTO materialitem = new MaterialInfoDTO();


                        string Plant_name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                        int Plant_Id = 0;
                        if (string.IsNullOrWhiteSpace(Plant_name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区没有值", i);
                            return errorInfo;
                        }
                        Plant_name = Plant_name.Trim();
                        var Plantvm = PlantVMs.Where(o => o.Plant == Plant_name).FirstOrDefault();

                        if (Plant_name != null && Plantvm == null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有权限导入,请设置厂区", i);
                            return errorInfo;
                        }
                        else
                        {
                            Plant_Id = Plantvm.Plant_OrganizationUID;

                        }

                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        int BG_Organization_UID = 0;
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
                        int FunPlant_Organization_UID = 0;
                        if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有功能厂", i);
                            return errorInfo;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }


                        string Material_Id = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                        if (string.IsNullOrWhiteSpace(Material_Id))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料号没有值", i);
                            return errorInfo;
                        }
                        if (Material_Id != null && Material_Id.Length > 20)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料长度超过20", i);
                            return errorInfo;
                        }

                        string Material_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                        if (string.IsNullOrWhiteSpace(Material_Name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行品名没有值", i);
                            return errorInfo;
                        }
                        Material_Name = Material_Name.Trim();
                        if (Material_Name != null && Material_Name.Length > 50)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行品名长度超过50", i);
                            return errorInfo;
                        }

                        string Material_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "型号")].Value);
                        if (Material_Types != null && Material_Types.Length > 255)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行型号长度超过255", i);
                            return errorInfo;
                        }

                        string Classification = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "归类")].Value);
                        if (string.IsNullOrWhiteSpace(Classification))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行归类为空", i);
                            return errorInfo;
                        }
                        Classification = Classification.Trim();
                        int Delivery_Date_date;
                        string Delivery_Date = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "交期")].Value);
                        try
                        {
                            Delivery_Date_date = Convert.ToInt32(Delivery_Date);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行交期[{1}]输入错误", i, Delivery_Date);
                            return errorInfo;
                        }
                        decimal Unit_Price_dec;
                        string Unit_Price = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "单价")].Value);
                        try
                        {
                            Unit_Price_dec = Convert.ToDecimal(Unit_Price);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行单价[{1}]输入错误", i, Unit_Price);
                            return errorInfo;
                        }

                        var warid = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "预设仓库")].Value);
                        var rackid = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "预设料架")].Value);
                        var storageid = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "预设储位")].Value);
                        int Warehouse_Storage_UID = 0;
                        //开账时不能选择
                        var haswarhousest = warehousest.FirstOrDefault(m => m.Warehouse_ID == warid & m.Rack_ID == rackid & m.Storage_ID == storageid & m.BG_Organization_UID == BG_Organization_UID & m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                        if (haswarhousest != null)
                        {
                            Warehouse_Storage_UID = haswarhousest.Warehouse_Storage_UID;
                        }
                        else
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行储位信息不存在", i);
                            return errorInfo;
                        }

                        var costcenter = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "成本中心")].Value);
                        var Maintenance_Cycle = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "保养周期")].Value);
                        int intMatCycle = 0;
                        try
                        {
                            intMatCycle = Convert.ToInt32(Maintenance_Cycle);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行保养周期[{1}]输入错误", i, Maintenance_Cycle);
                            return errorInfo;
                        }
                        var Material_Life = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "备品寿命")].Value);
                        int intMatLife = 0;
                        try
                        {
                            intMatLife = Convert.ToInt32(Material_Life);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行备品寿命[{1}]输入错误", i, Material_Life);
                            return errorInfo;
                        }

                        var Requisitions_Cycle = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "请购周期")].Value);
                        int int_Requisitions_Cycle = 0;
                        try
                        {
                            int_Requisitions_Cycle = Convert.ToInt32(Requisitions_Cycle);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行请购周期[{1}]输入错误", i, Requisitions_Cycle);
                            return errorInfo;
                        }
                        var Sign_days = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "签核天数")].Value);
                        int int_Sign_days = 0;
                        try
                        {
                            int_Sign_days = Convert.ToInt32(Sign_days);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行签核天数[{1}]输入错误", i, Sign_days);
                            return errorInfo;
                        }

                        var Daily_Consumption = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "日用量")].Value);
                        decimal decimal_Daily_Consumption = 0;
                        try
                        {
                            decimal_Daily_Consumption = Convert.ToDecimal(Daily_Consumption);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行日用量[{1}]输入错误", i, Daily_Consumption);
                            return errorInfo;
                        }

                        var Monthly_Consumption = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "每月单机用量")].Value);
                        int int_Monthly_Consumption = 0;
                        try
                        {
                            int_Monthly_Consumption = Convert.ToInt32(Monthly_Consumption);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行每月单机用量[{1}]输入错误", i, Monthly_Consumption);
                            return errorInfo;
                        }

                        var IsRework = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否计算返修")].Value);
                        bool bool_IsRework = false;
                        try
                        {
                            bool_IsRework = Convert.ToBoolean(IsRework == "Y" ? true : false);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行是否返修[{1}]输入错误,请输入Y或N", i, IsRework);
                            return errorInfo;
                        }

                        var Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        bool bool_Is_Enable = false;
                        try
                        {
                            bool_Is_Enable = Convert.ToBoolean(Is_Enable == "Y" ? true : false);
                        }
                        catch
                        {
                            errorInfo = string.Format("第{0}行是否启用[{1}]输入错误,请输入Y或N", i, Is_Enable);
                            return errorInfo;
                        }
                        /////增加判断型号是否重复
                        //var hasmaterialType = functionmaterial.FirstOrDefault(m => m.Organization_UID == Plant_Id && m.Material_Types == Material_Types);
                        //if (hasmaterialType != null)
                        //{
                          
                        //    errorInfo = string.Format("第{0}行型号[{1}]在系统中已经存在，不能重复导入", i, Material_Types);
                        //    return errorInfo;

                        //}

                        var hasitem = eqpmateriallist.FirstOrDefault(m =>  m.Organization_UID == Plant_Id &&m.Material_Id == Material_Id); //2019/03/01 Frank要修修改为 厂区+型号唯一
                        if (hasitem != null)
                        {
                            errorInfo = string.Format("第{0}行料号[{1}]重复", i,Material_Id);
                            return errorInfo;
                        }

                        //var hasitem1 = eqpmateriallist.FirstOrDefault(m => m.Organization_UID == Plant_Id && m.Material_Types == Material_Types); //2019/03/01 Frank要修修改为 厂区+型号唯一
                        //if (hasitem1 != null)
                        //{
                        //    errorInfo = string.Format("第{0}行型号号[{1}]重复", i, Material_Types);
                        //    return errorInfo;
                        //}

                        var hasmaterial = functionmaterial.FirstOrDefault(m =>  m.Organization_UID == Plant_Id && m.Material_Id == Material_Id);
                        if (hasmaterial != null)
                        {
                            //materialitem.Material_Uid = hasmaterial.Material_Uid;
                            //materialitem.Material_Id = Material_Id;
                            errorInfo = string.Format("第{0}行料号[{1}]在系统中已经存在，不能重复导入", i, Material_Id);
                            return errorInfo;

                        }
                        else
                        {
                            var liaohaobegin = Material_Id.Substring(0, 2);
                            if (liaohaobegin.Equals("CP"))
                            {
                                #region  导入料号以CP打头的自动生成编码

                                if (Material_Id.Length != 7)
                                {
                                    errorInfo = string.Format("第{0}行料号[{1}]新增料号必须是7位英文字母", i, Material_Id);
                                    return errorInfo;
                                }
                                var MaxMaterial_Id = string.Empty;
                                List<MaterialInfoDTO> MaterialList = (List<MaterialInfoDTO>)functionmaterial;
                                var tempList = MaterialList.Union(eqpmateriallist).ToList();
                                var Material_IdList = tempList.Where(p => (p.Material_Id.Length == 10 && p.Material_Id.Contains(Material_Id))).OrderByDescending(q => q.Material_Id);
                                if (Material_IdList.FirstOrDefault() != null)
                                {
                                    MaxMaterial_Id = Material_IdList.FirstOrDefault().Material_Id;
                                    string maxnumber = MaxMaterial_Id.Substring(7, MaxMaterial_Id.Length - 7);
                                    int maxMaterial_IdNum = 1;
                                    int.TryParse(maxnumber, out maxMaterial_IdNum);
                                    int NewMaterialIDNum = maxMaterial_IdNum + 1;
                                    if (NewMaterialIDNum > 999)
                                    {
                                        return "料号" + NewMaterialIDNum + "已经超过最大限度，请修改";
                                    }
                                    string NewMaterial_ID = string.Empty;
                                    //3位流水码补0
                                    string zeroCount = string.Empty;
                                    for (int j = 0; j < 3 - NewMaterialIDNum.ToString().Length; j++)
                                    {
                                        zeroCount += "0";
                                    }
                                    NewMaterial_ID = zeroCount + NewMaterialIDNum.ToString();
                                    materialitem.Material_Id = Material_Id + NewMaterial_ID;
                                }
                                else
                                {
                                    materialitem.Material_Id = Material_Id + "001";
                                }
                                #endregion
                            }
                            else
                            {
                                materialitem.Material_Id = Material_Id;
                            }
                        }
                        string Assembly_Name = Material_Id + "_" + Material_Name + "_" + Material_Types;
                        //materialitem.Material_Id = Material_Id;
                        materialitem.Material_Name = Material_Name;
                        materialitem.Material_Types = Material_Types;
                        materialitem.Assembly_Name = Assembly_Name;
                        materialitem.Classification = Classification;
                        materialitem.Unit_Price = Unit_Price_dec;
                        materialitem.Delivery_Date = Delivery_Date_date;

                        materialitem.Warehouse_Storage_UID = Warehouse_Storage_UID;
                        materialitem.Cost_Center = costcenter;
                        materialitem.Maintenance_Cycle = intMatCycle;
                        materialitem.Material_Life = intMatLife;
                        materialitem.Requisitions_Cycle = int_Requisitions_Cycle;
                        materialitem.Sign_days = int_Sign_days;
                        materialitem.Daily_Consumption = decimal_Daily_Consumption;
                        materialitem.Monthly_Consumption = int_Monthly_Consumption;
                        materialitem.IsRework = bool_IsRework;
                        materialitem.Is_Enable = bool_Is_Enable;

                        materialitem.Modified_UID = CurrentUser.AccountUId;
                        materialitem.Organization_UID = Plant_Id;
                        materialitem.Last_Qty = 0;

                        eqpmateriallist.Add(materialitem);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(eqpmateriallist);
                    var apiInsertEquipmentUrl = string.Format("Equipmentmaintenance/InsertMaterialAPI");
                    HttpResponseMessage responMessage1 = APIHelper.APIPostAsync(json, apiInsertEquipmentUrl);
                    errorInfo = responMessage1.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.ToString();
            }
            return errorInfo;
        }
        public List<PlantVM> GetPlatVM()
        {

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {

                return new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
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
                return Plants;
            }
        }


        private void SetMaterialExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
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
            worksheet.Cells[1, 14].Value = propertiesHead[13];
            worksheet.Cells[1, 15].Value = propertiesHead[14];
            worksheet.Cells[1, 16].Value = propertiesHead[15];
            worksheet.Cells[1, 17].Value = propertiesHead[16];
            worksheet.Cells[1, 18].Value = propertiesHead[17];
            worksheet.Cells[1, 19].Value = propertiesHead[18];
            worksheet.Cells[1, 20].Value = propertiesHead[19];
            worksheet.Cells[1, 21].Value = propertiesHead[20];
            //设置列宽
            worksheet.Column(1).Width = 17;
            worksheet.Column(2).Width = 17;
            worksheet.Column(3).Width = 17;
            worksheet.Column(4).Width = 17;
            worksheet.Column(5).Width = 17;
            worksheet.Column(6).Width = 17;
            worksheet.Column(7).Width = 17;
            worksheet.Column(8).Width = 17;
            worksheet.Column(9).Width = 17;
            worksheet.Column(10).Width = 17;
            worksheet.Column(11).Width = 17;
            worksheet.Column(12).Width = 17;
            worksheet.Column(13).Width = 17;
            worksheet.Column(14).Width = 17;
            worksheet.Column(15).Width = 17;
            worksheet.Column(16).Width = 17;
            worksheet.Column(17).Width = 17;
            worksheet.Column(18).Width = 17;
            worksheet.Column(19).Width = 17;
            worksheet.Column(20).Width = 17;
            worksheet.Column(21).Width = 17;
            worksheet.Cells["A1:U1"].Style.Font.Bold = true;
            worksheet.Cells["A1:U1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:U1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }

        public FileResult DownloadMaterialExcel()
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("导入料号关系");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetMaterialInforHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("导入料号关系");
                SetMaterialExcelStyle(worksheet, propertiesHead);
                //设置灰色背景
                var colorRange = "A1:U1";
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1: U1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1: U1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1: U1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                var worksheetsm = excelPackage.Workbook.Worksheets.Add("导入料号关系说明");
                SetMaterialExcelStyle(worksheetsm, propertiesHead);
                SetMaterialExcelStylesm(worksheetsm);


                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells["A1: U1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells["A1: U1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells["A1: U1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetMaterialExcelStylesm(ExcelWorksheet worksheet)
        {
            //填充Title内容
            worksheet.Cells[2, 1].Value = "CTU_M(厂区)";
            worksheet.Cells[2, 2].Value = "OP类型(OP2)";
            worksheet.Cells[2, 3].Value = "功能厂(CNC)";
            worksheet.Cells[2, 4].Value = "CNDZ056(料号)";
            worksheet.Cells[2, 5].Value = "马波斯对刀仪(品名)";
            worksheet.Cells[2, 6].Value = "A20B-3900-0298(型号)";
            worksheet.Cells[2, 7].Value = "表面(归类)";
            worksheet.Cells[2, 8].Value = "G001(预设仓库)";
            worksheet.Cells[2, 9].Value = "G001(预设料架)";
            worksheet.Cells[2, 10].Value = "G001(预设储位)";
            worksheet.Cells[2, 11].Value = "13212(成本中心)";
            worksheet.Cells[2, 12].Value = "7(交期)";
            worksheet.Cells[2, 13].Value = "2700(单价)";
            worksheet.Cells[2, 14].Value = "3(保养周期)";
            worksheet.Cells[2, 15].Value = "23(备品寿命)";
            worksheet.Cells[2, 16].Value = "7(请购周期)";
            worksheet.Cells[2, 17].Value = "1(签核天数)";
            worksheet.Cells[2, 18].Value = "21(日用量)";
            worksheet.Cells[2, 19].Value = "52(每月单机用量)";
            worksheet.Cells[2, 20].Value = "Y(是否计算返修)";
            worksheet.Cells[2, 21].Value = "N(是否启用)";

        }
        public string DeleteMaterial(int Material_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteMaterialAPI?Material_Uid={0}", Material_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        #endregion material

        #region  Error Info

        public ActionResult EQPError()
        {
            return View("EQPError");
        }

        public ActionResult QueryErrorInfo(ErrorInfoDTO search, Page page)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryErrorInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryErrorInfoByUid(int Enum_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryErrorInfoAPI?Enum_UID={0}", Enum_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string DeleteErrorInfo(int Enum_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteErrorInfoAPI?Enum_UID={0}", Enum_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string AddOrEditErrorInfo(ErrorInfoDTO dto, bool isEdit)
        {
            var apiUrl = string.Empty;
            apiUrl = string.Format("Equipmentmaintenance/AddOrEditErrorInfoAPI?isEdit={0}", isEdit);
            dto.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }

        #endregion  error info

        #region EQPMaintenance

        public ActionResult Queryeqpbyuser(string location, int funplant, int optype)
        {
            //待组织架构BUG解决后使用下面的对机台进行限制
            var apiUrl = string.Format("Equipmentmaintenance/QueryAllEquipmentAPI?location={0}&funplant={1}&optype={2}"
                , location, funplant, optype);
            //apiUrl = string.Format("Equipmentmaintenance/QueryAllEquipmentAPI");
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //var equipinfo = JsonConvert.DeserializeObject<List<EquipmentInfoDTO>>(result);
            return Content(result, "application/json");
        }

        public ActionResult EQPMaintenance(string iseqp_user)
        {
            //var orgs = CurrentUser.GetUserInfo.OrgInfo[0];
            if (iseqp_user == null)
                iseqp_user = "false";
            var apiUrl = "Equipmentmaintenance/EQPMaintenanceAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var errorinfo = JsonConvert.DeserializeObject<IEnumerable<EnumerationDTO>>(result);
            EQPRepairSearchVM currentVM = new EQPRepairSearchVM();
            currentVM.errorinfo = errorinfo;
            currentVM.iseqp_user = iseqp_user;
            var bgUID = 0;
            var funplantUID = 0;
            var plantUID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                bgUID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                funplantUID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                plantUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() == "EQPUSER")
            {
                apiUrl = string.Format("Equipmentmaintenance/QueryEQPUserAPI?EQPUser_Uid={0}", CurrentUser.AccountUId);
                responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                var eqpusers = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result);
                if (eqpusers.FirstOrDefault().BG_Organization_UID != null)
                {
                    bgUID = eqpusers.FirstOrDefault().BG_Organization_UID.Value;
                }
                if (eqpusers.FirstOrDefault().FunPlant_Organization_UID != null)
                {
                    funplantUID = eqpusers.FirstOrDefault().FunPlant_Organization_UID.Value;
                }
                plantUID = eqpusers.FirstOrDefault().Plant_OrganizationUID;
            }
            apiUrl = string.Format("Equipmentmaintenance/EQPAllUserAPI?bgUID={0}&funplantUID={1}&plantUID={2}", bgUID, funplantUID, plantUID);
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var userinfo = JsonConvert.DeserializeObject<IEnumerable<EQPUserTableDTO>>(result);
            currentVM.userinfo = userinfo;

            //int planId = 0;
            //if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0)
            //    if (CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            //    {
            //        planId = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            //    }
            apiUrl = "Equipmentmaintenance/EQPAllMaterialAPI?planId=" + GetPlantOrgUid();
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var materialinfo = JsonConvert.DeserializeObject<IEnumerable<MaterialInfoDTO>>(result);
            currentVM.materialinfo = materialinfo;

            //apiUrl = "Equipmentmaintenance/EQPUnitMatAPI";
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var unitmat = JsonConvert.DeserializeObject<List<string>>(result);
            //currentVM.unitmat = unitmat;
            currentVM.unitmat = new List<string>();
            //int Plant_Organization_UID = GetPlantOrgUid();
            //apiUrl = string.Format(@"Equipmentmaintenance/GetEQPLocationAPI?Plant_Organization_UID={0}", Plant_Organization_UID);
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var eqplocation = JsonConvert.DeserializeObject<List<string>>(result);
            //currentVM.eqplocation = eqplocation;
            currentVM.eqplocation = new List<string>();
            string strRole = "";
            if (iseqp_user != "true")
            {
                var roles = CurrentUser.GetUserInfo.RoleList;
                foreach (var item in roles)
                {
                    if (item.Role_ID == "EQPRepairer" || item.Role_ID == "EQPAdmin" || item.Role_ID == "EQPSuperAdimin")
                    {
                        currentVM.showrepair = true;
                    }
                    if (item.Role_ID == "EQPApplyer" || item.Role_ID == "EQPAdmin" || item.Role_ID == "EQPSuperAdimin")
                        currentVM.showapply = true;
                    strRole += item.Role_ID + ";";
                }
            }
            else
            {
                currentVM.optypes = new List<SystemProjectDTO>();
                currentVM.showrepair = true;
            }
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0)
            {
                if (CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                    currentVM.Plants = new List<PlantVM>() { new PlantVM() { Plant = CurrentUser.GetUserInfo.OrgInfo[0].Plant, Plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value } };
            }
            else
            {
                //获取
                var Plants = new List<PlantVM>();
                if (CurrentUser.GetUserInfo.User_NTID.ToUpper() == "EQPUSER")
                {
                    apiUrl = string.Format("Equipmentmaintenance/QueryEQPUserAPI?EQPUser_Uid={0}", CurrentUser.AccountUId);
                    responMessage = APIHelper.APIGetAsync(apiUrl);
                    result = responMessage.Content.ReadAsStringAsync().Result;
                    var eqpusers = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result);
                    apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", eqpusers.FirstOrDefault().Plant_OrganizationUID);
                }
                else
                {
                    apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                }

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
            apiUrl = string.Format("Equipmentmaintenance/QueryEQPUserAPI?EQPUser_Uid={0}", CurrentUser.AccountUId);
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var eqpuser = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result);
            if (eqpuser.Count > 0)
                currentVM.showrepair = true;
            ViewBag.nowtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            ViewBag.User_NTID = CurrentUser.GetUserInfo.User_NTID.ToUpper();//== "EQPUSER"
            ViewBag.Mentioner = CurrentUser.UserName + "_" + CurrentUser.GetUserInfo.User_NTID;
            currentVM.plantUID = plantUID;
            currentVM.bgUID = bgUID;
            currentVM.funplantUID = funplantUID;
            //價格顯示限制，僅設備管理員有權限
            currentVM.showprice = false;
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() != "EQPUSER")
            {
                if (CurrentUser.GetUserInfo.RoleList != null)
                {
                    currentVM.showprice = (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID == "EQPAdmin").Count() > 0);
                }
            }
            return View("EQPMaintenance", currentVM);
        }
        public ActionResult GetEQPPlanAllMaterial()
        {
            int Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = "Equipmentmaintenance/EQPAllMaterialAPI?planId=" + Plant_Organization_UID;
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

           return Content(result, "application/json");
        }

        public ActionResult QueryEQPMaintenance(EQPRepairInfoSearchDTO search, Page page)
        {



            int organization_UID = 0;
            //if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0)
            //    if (CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            //    {
            //        organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            //    }
            //if (search.Organization_UID == 0 && organization_UID != 0)
            //{
            //    search.Organization_UID = organization_UID;
            //}
            var bgUID = 0;
            var funplantUID = 0;
            var plantUID = 0;
            var costctrUID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                funplantUID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                plantUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() == "EQPUSER")
            {

                var apiUrl1 = string.Format("Equipmentmaintenance/QueryEQPUserAPI?EQPUser_Uid={0}", CurrentUser.AccountUId);
                var responMessage1 = APIHelper.APIGetAsync(apiUrl1);
                var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                var eqpusers = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result1);
                if (eqpusers.FirstOrDefault().BG_Organization_UID != null)
                {
                    search.Organization_UID = eqpusers.FirstOrDefault().BG_Organization_UID.Value;
                }
                search.Plant_OrganizationUID = eqpusers.FirstOrDefault().Plant_OrganizationUID;

                if (eqpusers.FirstOrDefault().BG_Organization_UID != null)
                {
                    bgUID = eqpusers.FirstOrDefault().BG_Organization_UID.Value;
                }
                if (eqpusers.FirstOrDefault().FunPlant_Organization_UID != null)
                {
                    funplantUID = eqpusers.FirstOrDefault().FunPlant_Organization_UID.Value;
                }
            }
            if (search.Organization_UID == 0 && organization_UID != 0)
            {
                search.Organization_UID = organization_UID;
            }
            if ((search.FunPlant == "" || search.FunPlant == null) && funplantUID != 0)
            {
                search.FunPlant_OrganizationUID = funplantUID;
            }
            if (int.TryParse(search.CostCtr,out costctrUID))
            {
                search.CostCtr_UID = costctrUID;
            }

            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPMaintenanceAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFunPlantByOp(int oporguid)
        {
            var useruid = 0;
            var roles = CurrentUser.GetUserInfo.RoleList;
            string strRole = "";
            foreach (var item in roles)
            {
                strRole += item.Role_ID + ";";
            }
            if (!strRole.Contains("SystemAdmin;") && !strRole.Contains("EQPSuperAdimin;"))
            {
                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0)
                    if (!(CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Department == "设备"))
                        useruid = CurrentUser.AccountUId;
            }
            var apiUrl = string.Format("Equipmentmaintenance/GetUserFunplantAPI?userid={0}&&optypeuid={1}", useruid, oporguid);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var funplant = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(result);
            return Content(result, "application/json");
        }

        public ActionResult QueryEQPMaintenanceByUid(int Repair_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPMaintenanceByUidAPI?Repair_Uid={0}", Repair_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditEQPMaintenance(string jsonEQPMaintenance, bool isEdit)
        {
            var result = "";
            try
            {
                var dto = JsonConvert.DeserializeObject<EQPRepairInfoDTO>(jsonEQPMaintenance);
                if (dto.Error_Time.ToString("yyyy/MM/dd") == "0001/01/01")
                {
                    return "请填写发生时间";
                }
                if (dto.Error_Time > dto.Apply_Time)
                    return "发生时间不可大于报修时间";
                if (dto.Error_Time > DateTime.Now.AddMinutes(6))
                    return "发生时间不可大于当前时间";
                if (!isEdit && dto.Error_Time < DateTime.Today.AddHours(-24))
                    return "发生时间不可比当前时间早一天以上";
                EQPRepairInfoDTO item = new EQPRepairInfoDTO();
                item.Repair_Uid = dto.Repair_Uid;
                item.EQP_Uid = dto.EQP_Uid;
                item.Error_Time = Convert.ToDateTime(dto.Error_Time);
                item.Error_Types = dto.Error_Types;
                item.Error_Level = dto.Error_Level;
                item.Modified_UID = CurrentUser.AccountUId;
                item.Contact = dto.Contact;
                item.Contact_tel = dto.Contact_tel;
                item.Repair_Reason = dto.Repair_Reason;
                item.Reason_Analysis = dto.Reason_Analysis;
                item.All_RepairCost = 0;
                item.Apply_Time = DateTime.Now;
                item.Mentioner = CurrentUser.UserName + "_" + CurrentUser.GetUserInfo.User_NTID;
                //成本中心信息
                item.CostCtr_UID = dto.CostCtr_UID;
                var apiUrl = string.Format("Equipmentmaintenance/AddOrEditEQPMaintenanceAPI?eqp_id={0}&isEdit={1}", Convert.ToInt32(Request.Form["EQP_Uid"]), isEdit);
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(item, apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                //return RedirectToAction("EQPMaintenance", "Equipmentmaintenance");
            }
            catch (Exception e)
            {
                result = "储存异常:" + e.Message;
            }
            return result;
        }

        public string ImportEQPMaintenanceExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            if (upload_excel == null)
            {
                errorInfo = "请上传档案";
                return errorInfo;
            }
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
                if (totalColumns < 10)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }
                Dictionary<int, string> rows_errors = new Dictionary<int, string>();
                Dictionary<int, EQPRepairInfoDTO> rows_contents = new Dictionary<int, EQPRepairInfoDTO>();
                // xls converter
                for (int iRow = 1; iRow <= totalRows; iRow++)
                {
                    // header
                    if (iRow == 1)
                    {
                        //for (int iCol = 1; iCol <= totalColumns; iCol++)
                        //{
                        //    // valify header columns
                        //    // do nothing
                        //}
                    }
                    else
                    {
                        EQPRepairInfoDTO dto = new EQPRepairInfoDTO();
                        string row_content_error = string.Empty;
                        for (int iCol = 1; iCol <= totalColumns; iCol++)
                        {
                            switch (iCol)
                            {
                                case 1:
                                    string v1 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v1))
                                        row_content_error = "提报人员为空";
                                    else if (v1.Split('_').Length < 2)
                                        row_content_error = "提报人员格式需为 (人名_NTID)";
                                    else
                                        dto.Mentioner = v1.Trim();
                                    break;
                                case 2:
                                    DateTime v2 = worksheet.GetValue<DateTime>(iRow, iCol);
                                    if (v2 == DateTime.MinValue)
                                        row_content_error = "发生日期不正确";
                                    dto.Error_Time = v2;
                                    break;
                                case 3:
                                    DateTime v3 = worksheet.GetValue<DateTime>(iRow, iCol);
                                    if (v3 == DateTime.MinValue)
                                        row_content_error = "报修日期不正确";
                                    dto.Apply_Time = v3;
                                    break;
                                case 4:
                                    string v4 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v4))
                                        row_content_error = "机台EMT为空";
                                    dto.Equipment = v4 ?? "";
                                    break;
                                case 5:
                                    string v5 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v5))
                                        row_content_error = "机台序列号为空";
                                    dto.Mfg_Serial_Num = v5 ?? "";
                                    break;
                                case 6:
                                    string v6 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v6))
                                        row_content_error = "连系人为空";
                                    dto.Contact = v6 ?? "";
                                    break;
                                case 7:
                                    string v7 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v7))
                                        row_content_error = "连系人电话为空";
                                    dto.Contact_tel = v7 ?? "";
                                    break;
                                case 8:
                                    string v8 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v8))
                                        row_content_error = "故障等级为空";
                                    else
                                        dto.Error_Level = v8.Trim();
                                    break;
                                case 9:
                                    string v9 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v9))
                                        row_content_error = "故障现象为空";
                                    else
                                        dto.Repair_Reason = v9.Trim();
                                    break;
                                case 10:
                                    string v10 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v10))
                                        row_content_error = "成本中心为空";
                                    else
                                        dto.CostCtr_ID = v10.Trim();                                                                        
                                    break;
                            }
                        }
                        // check biz logic
                        if (string.IsNullOrEmpty(row_content_error) == false)
                        {
                            if (dto.Error_Time > dto.Apply_Time)
                                row_content_error = "发生时间不可大于报修时间";
                            if (dto.Error_Time > DateTime.Now.AddMinutes(6))
                                row_content_error = "发生时间不可大于当前时间";
                        }
                        // check dump
                        if (rows_contents.Values.Select(x => x.Mfg_Serial_Num).Contains(dto.Mfg_Serial_Num))
                            row_content_error = "设备序号重覆上传";
                        if (rows_contents.Values.Select(x => x.Equipment).Contains(dto.Equipment))
                            row_content_error = "EMT设备号重覆上传";
                        // collect validations
                        if (string.IsNullOrEmpty(row_content_error) == false)
                            rows_errors.Add(iRow, row_content_error);
                        else
                            rows_contents.Add(iRow, dto);
                    }
                }
                // update into db
                if (rows_errors.Count == 0 && rows_contents.Count > 0)
                {
                    foreach (var item in rows_contents)
                    {
                        item.Value.Status = "Create";
                        item.Value.EQPUSER_Uid = CurrentUser.AccountUId;
                        item.Value.Modified_UID = CurrentUser.AccountUId;
                        item.Value.Modified_Date = DateTime.Now;
                    }
                    var jsonString = JsonConvert.SerializeObject(rows_contents, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                    });
                    var apiUrl = "Equipmentmaintenance/AddManyMaintenanceAPI";
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonString, apiUrl);
                    var result = responMessage.Content.ReadAsStringAsync().Result;
                    if (result.Length > 2)
                    {
                        result = result.Substring(1, result.Length - 2).Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\\"", "\"");
                        object results = JsonConvert.DeserializeObject(result, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                        });
                        var rs = (Dictionary<int, string>)results;
                        foreach (var r in rs)
                            rows_errors.Add(r.Key, r.Value);
                    }
                }
                if (rows_errors.Count > 0)
                {
                    StringBuilder string_builder = new StringBuilder();
                    foreach (var row_error in rows_errors)
                        string_builder.Append(string.Format("第{0}笔,{1}<br />", row_error.Key, row_error.Value));
                    return string_builder.ToString();
                }
            }
            return string.Empty;
        }

        public string ImportEQPMaintenanceExcel_Work(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            if (upload_excel == null)
            {
                errorInfo = "请上传档案";
                return errorInfo;
            }
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
                if (totalColumns < 16)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }
                // get all labors
                List<EQPUserTableDTO> labors = new List<EQPUserTableDTO>();
                var apiUrl_u = "Equipmentmaintenance/QueryAllEQPUserAPI";
                var responMessage_u = APIHelper.APIGetAsync(apiUrl_u);
                var result_u = responMessage_u.Content.ReadAsStringAsync().Result;
                labors = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result_u);
                // get all materials
                List<MaterialInfoDTO> materials = new List<MaterialInfoDTO>();
                var apiUrl_m = "Equipmentmaintenance/QueryAllMaterialAPI";
                var responMessage_m = APIHelper.APIGetAsync(apiUrl_m);
                var result_m = responMessage_m.Content.ReadAsStringAsync().Result;
                materials = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result_m);
                // row 2 data contents
                Dictionary<int, string> rows_errors = new Dictionary<int, string>();
                Dictionary<int, EQPRepairInfoDTO> rows_contents = new Dictionary<int, EQPRepairInfoDTO>();
                Dictionary<double, Dictionary<int, LaborUsingInfoDTO>> row_contents_labor = new Dictionary<double, Dictionary<int, LaborUsingInfoDTO>>();
                Dictionary<double, Dictionary<int, MeterialUpdateInfoDTO>> row_contents_material = new Dictionary<double, Dictionary<int, MeterialUpdateInfoDTO>>();
                // xls converter
                for (int iRow = 1; iRow <= totalRows; iRow++)
                {
                    // header
                    if (iRow == 1)
                    {
                        //for (int iCol = 1; iCol <= totalColumns; iCol++)
                        //{
                        //    // valify header columns
                        //    // do nothing
                        //}
                    }
                    else
                    {
                        EQPRepairInfoDTO dto = new EQPRepairInfoDTO();
                        string row_content_error = string.Empty;
                        string row_content_error_sub = string.Empty;
                        // first column - repair uid
                        string v1 = worksheet.GetValue<string>(iRow, 1);
                        if (string.IsNullOrEmpty(v1))
                        {
                            row_content_error = "维修编号空白";
                            rows_errors.Add(iRow, row_content_error);
                            continue;
                        }
                        double repair_id = 0;
                        if (v1.Length == 12 && double.TryParse(v1.Trim(), out repair_id) == true)
                        {
                            string myDateStr = v1.Substring(0, 8);
                            DateTime myDate = DateTime.MinValue;
                            if (DateTime.TryParseExact(myDateStr, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out myDate) == true)
                                dto.Repair_id = repair_id.ToString();
                            else
                            {
                                row_content_error = "维修编号不正确";
                                rows_errors.Add(iRow, row_content_error);
                                continue;
                            }
                        }
                        else
                        {
                            row_content_error = "维修编号不正确";
                            rows_errors.Add(iRow, row_content_error);
                            continue;
                        }
                        // others columns
                        var dtoEntity = rows_contents.Values.FirstOrDefault(x => x.Repair_id == repair_id.ToString());
                        if (dtoEntity != null)
                            dto = dtoEntity;
                        // others columns
                        for (int iCol = 2; iCol <= totalColumns; iCol++)
                        {
                            switch (iCol)
                            {
                                case 2:
                                    string v2 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v2) && dtoEntity == null)
                                        row_content_error = "机台EMT空白";
                                    if (dtoEntity == null)
                                        dto.Equipment = v2 ?? "";
                                    break;
                                case 3:
                                    string v3 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v3) && dtoEntity == null)
                                        row_content_error = "机台序列号空白";
                                    if (dtoEntity == null)
                                        dto.Mfg_Serial_Num = v3 ?? "";
                                    break;
                                case 4:
                                    string v4 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v4) && dtoEntity == null)
                                        row_content_error = "处理结果空白";
                                    if (dtoEntity == null)
                                        dto.Repair_Result = v4 ?? "";
                                    break;
                                case 5:
                                    string v5 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v5) && dtoEntity == null)
                                        row_content_error = "故障类型空白";
                                    if (dtoEntity == null)
                                        dto.Error_Types = v5 ?? "";
                                    break;
                                case 6:
                                    string v6 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v6) && dtoEntity == null)
                                        row_content_error = "故障原因空白";
                                    if (dtoEntity == null)
                                        dto.Reason_Analysis = v6 ?? "";
                                    break;
                                case 7:
                                    string v7 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v7) && dtoEntity == null)
                                        row_content_error = "处理措施空白";
                                    if (dtoEntity == null)
                                        dto.Repair_Method = v7 ?? "";
                                    break;
                                case 8:
                                    DateTime v8 = worksheet.GetValue<DateTime>(iRow, iCol);
                                    if (v8 == DateTime.MinValue && dtoEntity == null)
                                        row_content_error = "维修开始日期错误";
                                    if (dtoEntity == null)
                                        dto.Repair_BeginTime = v8;
                                    break;
                                case 9:
                                    DateTime v9 = worksheet.GetValue<DateTime>(iRow, iCol);
                                    if (v9 == DateTime.MinValue && dtoEntity == null)
                                        row_content_error = "维修结束日期错误";
                                    if (dtoEntity == null)
                                        dto.Repair_EndTime = v9;
                                    break;
                                case 10:
                                    decimal v10 = worksheet.GetValue<decimal>(iRow, iCol);
                                    //if (v10 == 0)
                                    //    row_content_error = "维修用时不为0";
                                    if (dtoEntity == null)
                                        dto.Labor_Time = v10;
                                    break;
                                case 11:
                                    string v11 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v11) == false)
                                    {
                                        v11 = v11 ?? "";
                                        var labor = labors.FirstOrDefault(x => x.User_Name == v11);
                                        if (labor != null)
                                        {
                                            if (row_contents_labor.ContainsKey(repair_id) == false)
                                                row_contents_labor.Add(repair_id, new Dictionary<int, LaborUsingInfoDTO>());
                                            if (row_contents_labor[repair_id].ContainsKey(labor.EQPUser_Uid) == false)
                                            {
                                                dto.Labor_List += v11 + ";";
                                                row_contents_labor[repair_id].Add(labor.EQPUser_Uid, new LaborUsingInfoDTO() { EQPUser_Uid = labor.EQPUser_Uid });
                                            }
                                            else
                                                row_content_error_sub = "同一维修单，员工重覆";
                                        }
                                        else
                                            row_content_error_sub = "查无此员工";
                                    }
                                    break;
                                case 12:
                                    decimal v12 = worksheet.GetValue<decimal>(iRow, iCol);
                                    if (dtoEntity == null)
                                        dto.All_RepairCost = v12;
                                    break;
                                case 13:
                                    // 配件名 改为了配件料号
                                    string v13 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v13) == false)
                                    {
                                        v13 = v13 ?? "";
                                        var material = materials.FirstOrDefault(x => x.Material_Id == v13);
                                        if (material != null)
                                        {
                                            if (row_contents_material.ContainsKey(repair_id) == false)
                                                row_contents_material.Add(repair_id, new Dictionary<int, MeterialUpdateInfoDTO>());
                                            if (row_contents_material[repair_id].ContainsKey(material.Material_Uid) == false)
                                            {
                                                // 配件单价
                                                decimal v14 = worksheet.GetValue<decimal>(iRow, iCol + 1);
                                                // 配件数量
                                                int v15 = worksheet.GetValue<int>(iRow, iCol + 2);
                                                // 配件小计
                                                decimal v16 = worksheet.GetValue<decimal>(iRow, iCol + 3);
                                                dto.Update_Part += string.Format("{0} *{1};", v13, v15);
                                                row_contents_material[repair_id].Add(material.Material_Uid, new MeterialUpdateInfoDTO() { Material_Name = v13, Material_Uid = material.Material_Uid, Update_No = v15, Update_Cost = v16 });
                                            }
                                            else
                                            {
                                                row_content_error_sub = "同一维修单，配件重覆";
                                            }
                                        }
                                        else
                                            row_content_error_sub = "查无此配件";
                                    }
                                    break;
                                case 17:
                                    string v17 = worksheet.GetValue<string>(iRow, iCol);
                                    dto.Repair_Remark = v17 ?? "";
                                    break;
                            }
                            // add labor user
                            //if (string.IsNullOrEmpty(dto_u.User_Name) == false)
                            //    row_contents_labor.Add(repair_uid, dto_u);
                            // add material
                            //if (string.IsNullOrEmpty(dto_m.Material_Name) == false)
                            //    row_contents_material.Add(repair_uid, dto_m);
                            // not valid
                            if (string.IsNullOrEmpty(row_content_error) == false)
                                continue;
                        }
                        // check biz logic
                        if (string.IsNullOrEmpty(row_content_error) == false)
                        {
                            if (dto.Repair_BeginTime > dto.Repair_EndTime)
                                row_content_error = "维修开始时间不可大于维修结束时间";
                            //if (dto.Repair_EndTime > DateTime.Now.AddMinutes(6))
                            //    row_content_error = "维修结束时间不可大于当前时间";
                        }
                        // check dump
                        //if (rows_contents.Values.Select(x => x.Mfg_Serial_Num).Contains(dto.Mfg_Serial_Num))
                        //    row_content_error = "设备序号重覆上传";
                        //if (rows_contents.Values.Select(x => x.Equipment).Contains(dto.Equipment))
                        //    row_content_error = "EMT设备号重覆上传";
                        // collect validations
                        if (string.IsNullOrEmpty(row_content_error) == false)
                            rows_errors.Add(iRow, row_content_error);
                        else
                        {
                            if (dtoEntity == null)
                                rows_contents.Add(iRow, dto);
                            if (string.IsNullOrEmpty(row_content_error_sub) == false)
                                rows_errors.Add(iRow, row_content_error_sub);
                        }
                    }
                }
                // update into db
                if (rows_errors.Count == 0 && rows_contents.Count > 0)
                {
                    foreach (var item in rows_contents)
                    {
                        item.Value.EQPUSER_Uid = CurrentUser.AccountUId;
                        item.Value.Modified_UID = CurrentUser.AccountUId;
                        item.Value.Modified_Date = DateTime.Now;
                    }
                    var jsonString1 = JsonConvert.SerializeObject(rows_contents, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                    });
                    var jsonString2 = JsonConvert.SerializeObject(row_contents_labor, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                    });
                    var jsonString3 = JsonConvert.SerializeObject(row_contents_material, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                    });
                    var apiUrl = "Equipmentmaintenance/AddManyMaintenanceAPI2";
                    string postData = "{" + string.Format(@"data:{0}, labors:{1} ,materials:{2}", jsonString1, jsonString2, jsonString3) + "}";
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(postData, apiUrl);
                    var result = responMessage.Content.ReadAsStringAsync().Result;
                    if (result.Length > 2)
                    {
                        result = result.Substring(1, result.Length - 2).Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\\"", "\"");
                        object results = JsonConvert.DeserializeObject(result, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                        });
                        var rs = (Dictionary<int, string>)results;
                        foreach (var r in rs)
                            rows_errors.Add(r.Key, r.Value);
                    }
                }
                if (rows_errors.Count > 0)
                {
                    StringBuilder string_builder = new StringBuilder();
                    foreach (var row_error in rows_errors)
                        string_builder.Append(string.Format("第{0}笔,{1}<br />", row_error.Key, row_error.Value));
                    return string_builder.ToString();
                }
            }
            return string.Empty;
        }

        public string ImportEQPMaintenanceExcel_Close(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            if (upload_excel == null)
            {
                errorInfo = "请上传档案";
                return errorInfo;
            }
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
                if (totalColumns < 3)
                {
                    errorInfo = "Excel格式不正确";
                    return errorInfo;
                }
                // 记录多少错的
                Dictionary<int, string> rows_errors = new Dictionary<int, string>();
                // 资料列
                Dictionary<int, EQPRepairInfoDTO> rows_contents = new Dictionary<int, EQPRepairInfoDTO>();
                // xls converter
                for (int iRow = 1; iRow <= totalRows; iRow++)
                {
                    // header
                    if (iRow == 1)
                    {
                        //for (int iCol = 1; iCol <= totalColumns; iCol++)
                        //{
                        //    // valify header columns
                        //    // do nothing
                        //}
                    }
                    else
                    {
                        // repository -> CRUD
                        // dto = data transpost object -> json
                        EQPRepairInfoDTO dto = new EQPRepairInfoDTO();
                        string row_content_error = string.Empty;
                        for (int iCol = 1; iCol <= totalColumns; iCol++)
                        {
                            switch (iCol)
                            {
                                case 1:
                                    string v1 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v1))
                                        row_content_error = "维修编号空白";
                                    dto.Repair_id = v1.Trim();
                                    break;
                                case 2:
                                    string v2 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v2))
                                        row_content_error = "机台EMT空白";
                                    dto.Mfg_Serial_Num = v2;
                                    break;
                                case 3:
                                    string v3 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v3))
                                        row_content_error = "机台序列号空白";
                                    dto.Equipment = v3;
                                    break;
                            }
                        }
                        // check dump
                        if (rows_contents.Values.Select(x => x.Repair_id).Contains(dto.Repair_id))
                            row_content_error = "维修编号重覆上传";
                        if (rows_contents.Values.Select(x => x.Mfg_Serial_Num).Contains(dto.Mfg_Serial_Num))
                            row_content_error = "设备序号重覆上传";
                        if (rows_contents.Values.Select(x => x.Equipment).Contains(dto.Equipment))
                            row_content_error = "EMT设备号重覆上传";
                        // collect validations
                        if (string.IsNullOrEmpty(row_content_error) == false)
                            rows_errors.Add(iRow, row_content_error);
                        else
                            rows_contents.Add(iRow, dto);
                    }
                }
                // update into db
                if (rows_errors.Count == 0 && rows_contents.Count > 0)
                {
                    foreach (var item in rows_contents)
                    {
                        item.Value.Status = "Close";
                        item.Value.Modified_UID = CurrentUser.AccountUId;
                        item.Value.Modified_Date = DateTime.Now;
                    }
                    var jsonString = JsonConvert.SerializeObject(rows_contents, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                    });
                    var apiUrl = "Equipmentmaintenance/AddManyMaintenanceAPI3";
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonString, apiUrl);
                    var result = responMessage.Content.ReadAsStringAsync().Result;
                    if (result.Length > 2)
                    {
                        result = result.Substring(1, result.Length - 2).Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\\"", "\"");
                        object results = JsonConvert.DeserializeObject(result, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                        });
                        var rs = (Dictionary<int, string>)results;
                        foreach (var r in rs)
                            rows_errors.Add(r.Key, r.Value);
                    }
                }
                if (rows_errors.Count > 0)
                {
                    StringBuilder string_builder = new StringBuilder();
                    foreach (var row_error in rows_errors)
                        string_builder.Append(string.Format("第{0}笔,{1}<br />", row_error.Key, row_error.Value));
                    return string_builder.ToString();
                }
            }
            return string.Empty;
        }

        public string DeleteEQPMaintenance(int Repair_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteEQPRepariAPI?Repair_Uid={0}", Repair_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public ActionResult QueryMaterialUpdateByUid(int Material_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryMaterialUpdateByUidAPI?Material_Uid={0}", Material_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditEQPMaintenance2(EQPRepairInfoDTO dto, string laborlist, string matlist, bool iseqp_user)
        {
            if (dto.Repair_BeginTime < dto.Apply_Time)
                return "维修开始时间不可小于报修时间";
            var apiUrl = string.Format("Equipmentmaintenance/UpdateLaborTableAPI?userid={0}&Repair_Uid={1}&MH_Flag={2}",
                CurrentUser.AccountUId, dto.Repair_Uid, iseqp_user);
            var jsonLaborData = Request.Form["laborlist"].ToString();
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonLaborData, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (result != "\"Success\"")
                return responMessage.Content.ReadAsStringAsync().Result;
            apiUrl = string.Format(@"Equipmentmaintenance/UpdateMatTableAPI?userid={0}&updatetime={1}&EQP_Uid={2}&Repair_Uid={3}&MH_Flag={4}",
                                CurrentUser.AccountUId, dto.Repair_EndTime, dto.EQP_Uid,
                                    dto.Repair_Uid, iseqp_user);
            var jsonMatData = Request.Form["matlist"].ToString();
            responMessage = APIHelper.APIPostAsync(jsonMatData, apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            if (result != "\"Success\"")
                return responMessage.Content.ReadAsStringAsync().Result;

            EQPRepairInfoDTO item = new EQPRepairInfoDTO();
            item.Repair_Uid = dto.Repair_Uid;
            item.Repair_BeginTime = dto.Repair_BeginTime;
            item.Repair_EndTime = dto.Repair_EndTime;
            if (item.Repair_EndTime < item.Repair_BeginTime)
            {
                return "完成时间需大于等于开始时间";
            }
            item.Error_Types = dto.Error_Types;
            item.EQP_Uid = dto.EQP_Uid;
            item.Status = "Commit";
            item.Reason_Analysis = dto.Reason_Analysis;
            item.Reason_Types = dto.Reason_Types;
            item.Repair_Method = dto.Repair_Method;
            item.Repair_Result = dto.Repair_Result;
            TimeSpan labor_time = ((DateTime)item.Repair_EndTime - (DateTime)item.Repair_BeginTime);
            item.Labor_Time = (decimal)1.0 * ((labor_time.Days * 24 + labor_time.Hours) * 60 + labor_time.Minutes) / 60;
            if (CurrentUser.GetUserInfo.MH_Flag)
                item.EQPUSER_Uid = CurrentUser.AccountUId;
            else
                item.Modified_UID = CurrentUser.AccountUId;
            item.Repair_Remark = dto.Repair_Remark;
            //成本中心信息
            //item.CostCtr_UID = dto.CostCtr_UID;

            apiUrl = string.Format("Equipmentmaintenance/AddOrEditEQPMaintenance2API?MH_Flag={0}", CurrentUser.GetUserInfo.MH_Flag);
            responMessage = APIHelper.APIPostAsync(item, apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }


        public string AddOrEditEQPMaintenanceR(string jsonEQPMaintenance, string laborlist, string matlist, bool iseqp_user)
        {
            var result = "";
            try
            {
                var dto = JsonConvert.DeserializeObject<EQPRepairInfoDTO>(jsonEQPMaintenance);
                if (dto.Repair_BeginTime < dto.Apply_Time)
                    return "维修开始时间不可小于报修时间";
                var apiUrl = string.Format("Equipmentmaintenance/UpdateLaborTableAPI?userid={0}&Repair_Uid={1}&MH_Flag={2}",
                    CurrentUser.AccountUId, dto.Repair_Uid, iseqp_user);
                var jsonLaborData = Request.Form["laborlist"].ToString();
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonLaborData, apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                if (result != "\"Success\"")
                    return responMessage.Content.ReadAsStringAsync().Result;
                apiUrl = string.Format(@"Equipmentmaintenance/UpdateMatTableAPI?userid={0}&updatetime={1}&EQP_Uid={2}&Repair_Uid={3}&MH_Flag={4}",
                                    CurrentUser.AccountUId, dto.Repair_EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), dto.EQP_Uid,
                                        dto.Repair_Uid, iseqp_user);
                var jsonMatData = Request.Form["matlist"].ToString();
                responMessage = APIHelper.APIPostAsync(jsonMatData, apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                if (result != "\"Success\"")
                    return responMessage.Content.ReadAsStringAsync().Result;

                EQPRepairInfoDTO item = new EQPRepairInfoDTO();
                item.Repair_Uid       = dto.Repair_Uid;
                item.Repair_BeginTime = dto.Repair_BeginTime;
                item.Repair_EndTime   = dto.Repair_EndTime;
                if (item.Repair_EndTime < item.Repair_BeginTime)
                {
                    return "完成时间需大于等于开始时间";
                }
                item.Error_Types = dto.Error_Types;
                item.EQP_Uid = dto.EQP_Uid;
                item.Status = "Commit";
                item.Reason_Analysis = dto.Reason_Analysis;
                item.Reason_Types = dto.Reason_Types;
                item.Repair_Method = dto.Repair_Method;
                item.Repair_Result = dto.Repair_Result;
                TimeSpan labor_time = ((DateTime)item.Repair_EndTime - (DateTime)item.Repair_BeginTime);
                item.Labor_Time = (decimal)1.0 * ((labor_time.Days * 24 + labor_time.Hours) * 60 + labor_time.Minutes) / 60;
                if (CurrentUser.GetUserInfo.MH_Flag)
                    item.EQPUSER_Uid = CurrentUser.AccountUId;
                else
                    item.Modified_UID = CurrentUser.AccountUId;
                item.Repair_Remark = dto.Repair_Remark;
                //成本中心信息
                //item.CostCtr_UID = dto.CostCtr_UID;

                apiUrl = string.Format("Equipmentmaintenance/AddOrEditEQPMaintenance2API?MH_Flag={0}", CurrentUser.GetUserInfo.MH_Flag);
                responMessage = APIHelper.APIPostAsync(item, apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                result = "储存异常:" + e.Message;
            }
            return result;

        }



        public ActionResult UpdateSubTable(string jsonLaborData, string jsonMatData, DateTime endtime, int EQP_Uid, int Repair_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/UpdateLaborTableAPI?userid={0}&Repair_Uid={1}", CurrentUser.AccountUId, Repair_Uid);

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(jsonLaborData, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            if (result != "\"Success\"")
                return Content(result, "application/json");
            apiUrl = string.Format("Equipmentmaintenance/UpdateMatTableAPI?userid={0}&updatetime={1}&EQP_Uid={2}&Repair_Uid={3}",
                CurrentUser.AccountUId, endtime, EQP_Uid, Repair_Uid);
            responMessage = APIHelper.APIPostAsync(jsonMatData, apiUrl);
            var result2 = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result2, "application/json");
        }

        public string EQPMaintenanceClose(EQPRepairInfoDTO dto)
        {
            var apiUrl = string.Format("Equipmentmaintenance/EQPMaintenanceCloseAPI?Repair_Uid={0}", dto.Repair_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }
        public string EQPMaintenanceCloseR(string jsonEQPMaintenance)
        {
            var result = "";
            try
            {
                var dto = JsonConvert.DeserializeObject<EQPRepairInfoDTO>(jsonEQPMaintenance);
                var apiUrl = string.Format("Equipmentmaintenance/EQPMaintenanceCloseAPI?Repair_Uid={0}", dto.Repair_Uid);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception e)
            {
                result = "储存异常:" + e.Message;
            }
            return result;
        }
        public ActionResult GetOptype()
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetOptypeAPI?");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetProjectnameByOptype(string Optype)
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetProjectnameByOptypeAPI?Optype={0}", Optype);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetFunPlantByOPTypes(int Optype, string Optypes = "")
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetFunPlantByOPTypeAPI?Optype={0}&Optypes={1}", Optype, Optypes);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetProcessByFunplant(string funplantuid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetProcessByFunplantAPI?funplantuid={0}", funplantuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetEqpidByProcess(string funplantuid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetEqpidByProcessAPI?funplantuid={0}"
                , funplantuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DoAllEQPMaintenanceReprot(EQPRepairInfoSearchDTO search)
        {


            int organization_UID = 0;
            var costctrUID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.Organization_UID == 0 && organization_UID != 0)
            {
                search.Organization_UID = organization_UID;
            }
            if (int.TryParse(search.CostCtr, out costctrUID))
            {
                search.CostCtr_UID = costctrUID;
            }
            var apiUrl = string.Format("Equipmentmaintenance/DoAllEQPMaintenanceReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EQPRepairInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Equipmentmaintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //var stringHeads = new string[] { "排序", "维修编号", "发生日期", "发生时间","报修时间","栋别","功能厂","厂别","专案","机台类型","厂内编号","机台EMT",
            //    "机台序列号","故障现象", "故障类型", "故障原因","处理措施","维修开始时间", "维修完成时间","处理结果","维修用时", "维修人",
            //    "总维修费用","更换配件","备注","提报人员","最后更新时间"};
            var stringHeads = new string[] { "排序", "维修编号", "状态","提报人员", "发生日期时间","报修时间","栋别","功能厂","厂别","专案","机台类型","厂内编号","机台EMT",
                "机台序列号","故障现象", "故障类型", "故障原因","处理措施","维修开始时间", "维修完成时间","最后更新时间","处理结果","维修用时", "维修人",
                "总维修费用","更换配件","备注","最后更新时间","成本中心","成本中心描述"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Equipment Maintenance");

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
                    //worksheet.Cells[index + 2, 1].Value = index + 1;
                    //worksheet.Cells[index + 2, 2].Value = currentRecord.Repair_id;
                    //worksheet.Cells[index + 2, 3].Value = currentRecord.Repair_Date;
                    //worksheet.Cells[index + 2, 4].Value = currentRecord.Repair_Time;
                    //worksheet.Cells[index + 2, 5].Value = currentRecord.Apply_Time == null ? "" : ((DateTime)currentRecord.Apply_Time).ToString(FormatConstants.DateTimeFormatString);
                    //worksheet.Cells[index + 2, 6].Value = currentRecord.EQP_Location;
                    //worksheet.Cells[index + 2, 7].Value = currentRecord.FunPlant;
                    //worksheet.Cells[index + 2, 8].Value = currentRecord.OP_TYPES;
                    //worksheet.Cells[index + 2, 9].Value = currentRecord.Project_Name;
                    //worksheet.Cells[index + 2, 10].Value = currentRecord.Class_Desc;
                    //worksheet.Cells[index + 2, 11].Value = currentRecord.EQP_Plant_No;
                    //worksheet.Cells[index + 2, 12].Value = currentRecord.Equipment;
                    //worksheet.Cells[index + 2, 13].Value = currentRecord.Mfg_Serial_Num;
                    //worksheet.Cells[index + 2, 14].Value = currentRecord.Repair_Reason;
                    //worksheet.Cells[index + 2, 15].Value = currentRecord.Error_Types;
                    //worksheet.Cells[index + 2, 16].Value = currentRecord.Reason_Analysis;
                    //worksheet.Cells[index + 2, 17].Value = currentRecord.Repair_Method;
                    //worksheet.Cells[index + 2, 18].Value = currentRecord.Repair_BeginTime == null ? "" : ((DateTime)currentRecord.Repair_BeginTime).ToString(FormatConstants.DateTimeFormatString);
                    //worksheet.Cells[index + 2, 19].Value = currentRecord.Repair_EndTime == null ? "" : ((DateTime)currentRecord.Repair_EndTime).ToString(FormatConstants.DateTimeFormatString);
                    //worksheet.Cells[index + 2, 20].Value = currentRecord.Repair_Result;
                    //worksheet.Cells[index + 2, 21].Value = currentRecord.TotalTime;
                    //worksheet.Cells[index + 2, 22].Value = currentRecord.Labor_List;
                    //worksheet.Cells[index + 2, 23].Value = currentRecord.All_RepairCost;
                    //worksheet.Cells[index + 2, 24].Value = currentRecord.Update_Part;
                    //worksheet.Cells[index + 2, 25].Value = currentRecord.Repair_Remark;
                    //worksheet.Cells[index + 2, 26].Value = currentRecord.Mentioner;
                    //worksheet.Cells[index + 2, 27].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Repair_id;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Mentioner;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Repair_Time;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Apply_Time == null ? "" : ((DateTime)currentRecord.Apply_Time).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 7].Value = currentRecord.EQP_Location;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.OP_TYPES;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.EQP_Plant_No;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Mfg_Serial_Num;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Repair_Reason;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Error_Types;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Reason_Analysis;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Repair_Method;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Repair_BeginTime == null ? "" : ((DateTime)currentRecord.Repair_BeginTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Repair_EndTime == null ? "" : ((DateTime)currentRecord.Repair_EndTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 21].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Repair_Result;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.TotalTime;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.Labor_List;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.All_RepairCost;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.Update_Part;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.Repair_Remark;
                    worksheet.Cells[index + 2, 28].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    //成本中心
                    worksheet.Cells[index + 2, 29].Value = currentRecord.CostCtr_ID;
                    worksheet.Cells[index + 2, 30].Value = currentRecord.CostCtr_Description;

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Equipmentmaintenance/DoExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EQPRepairInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Equipmentmaintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "维修编号", "状态","提报人员", "发生日期时间","报修时间","栋别","功能厂","厂别","专案","机台类型","厂内编号","机台EMT",
                "机台序列号","故障现象", "故障类型", "故障原因","处理措施","维修开始时间", "维修完成时间","最后更新时间","处理结果","维修用时", "维修人",
                "总维修费用","更换配件","备注","最后更新时间","成本中心","成本中心描述"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Equipment Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Repair_id;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Mentioner;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Repair_Time;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Apply_Time == null ? "" : ((DateTime)currentRecord.Apply_Time).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 7].Value = currentRecord.EQP_Location;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.OP_TYPES;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.EQP_Plant_No;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Mfg_Serial_Num;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Repair_Reason;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Error_Types;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Reason_Analysis;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Repair_Method;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Repair_BeginTime == null ? "" : ((DateTime)currentRecord.Repair_BeginTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Repair_EndTime == null ? "" : ((DateTime)currentRecord.Repair_EndTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 21].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Repair_Result;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.TotalTime;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.Labor_List;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.All_RepairCost;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.Update_Part;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.Repair_Remark;
                    worksheet.Cells[index + 2, 28].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    //成本中心
                    worksheet.Cells[index + 2, 29].Value = currentRecord.CostCtr_ID;
                    worksheet.Cells[index + 2, 30].Value = currentRecord.CostCtr_Description;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----所有符合条件的项目
        public ActionResult DoExportFunction2(string optypes, string projectname, string funplant, string process,
                        string eqpid, string errortype, string repairid, string location, string classdesc, string contact,
                        string fromdate, string todate, string errorlever, string repairresult, string labor,
                        string updatepart, string remark, string status)
        {
            //get Export datas
            DateTime datefromdate = new DateTime();
            DateTime datetodate = new DateTime();
            if (fromdate != "")
                datefromdate = Convert.ToDateTime(fromdate);
            if (todate != "")
                datetodate = Convert.ToDateTime(todate);
            if (projectname == "undefined")
                projectname = "";
            if (funplant == "undefined")
                funplant = "";
            if (status == "undefined")
                status = "";
            var apiUrl = string.Format("Equipmentmaintenance/DoExportFunctionAPI2?optypes={0}&projectname={1}" +
                            "&funplant={2}&process={3}&eqpid={4}&errortype={5}&repairid={6}&location={7}&classdesc={8}" +
                            "&contact={9}&fromdate={10}&todate={11}&errorlever={12}&repairresult={13}" +
                            "&labor={14}&updatepart={15}&remark={16}&status={17}", optypes, projectname,
                            funplant, process, eqpid, errortype, repairid, location, classdesc, contact, datefromdate,
                            datetodate, errorlever, repairresult, labor, updatepart, remark, status);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EQPRepairInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Equipmentmaintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "维修编号", "状态","提报人员", "发生日期时间","报修时间","栋别","功能厂","厂别","专案","机台类型","厂内编号","机台EMT",
                "机台序列号","故障现象", "故障类型", "故障原因","处理措施","维修开始时间", "维修完成时间", "最后更新时间","处理结果","维修用时", "维修人",
                "总维修费用","更换配件","备注"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Equipment Maintenance");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Repair_id;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Mentioner;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Repair_Time;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Apply_Time == null ? "" : ((DateTime)currentRecord.Apply_Time).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 7].Value = currentRecord.EQP_Location;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.OP_TYPES;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.EQP_Plant_No;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Mfg_Serial_Num;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Repair_Reason;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Error_Types;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Reason_Analysis;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Repair_Method;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Repair_BeginTime == null ? "" : ((DateTime)currentRecord.Repair_BeginTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Repair_EndTime == null ? "" : ((DateTime)currentRecord.Repair_EndTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 21].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Repair_Result;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.TotalTime;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.Labor_List;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.All_RepairCost;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.Update_Part;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.Repair_Remark;
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
        public ActionResult DoPartExportFunction(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Equipmentmaintenance/DoPartExportFunctionAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EQPRepairInfoDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Equipmentmaintenance");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "维修编号", "状态","提报人员", "发生日期时间","报修时间","栋别","功能厂","厂别","专案","机台类型","厂内编号","机台EMT",
                "机台序列号","故障现象", "故障类型", "故障原因","处理措施","维修开始时间", "维修完成时间", "最后更新时间","处理结果","维修用时", "维修人",
                "总维修费用","更换配件","备注"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Equipment Maintenance");
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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Repair_id;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Mentioner;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Repair_Time;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Apply_Time == null ? "" : ((DateTime)currentRecord.Apply_Time).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 7].Value = currentRecord.EQP_Location;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.OP_TYPES;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.EQP_Plant_No;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Mfg_Serial_Num;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Repair_Reason;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Error_Types;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Reason_Analysis;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Repair_Method;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Repair_BeginTime == null ? "" : ((DateTime)currentRecord.Repair_BeginTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Repair_EndTime == null ? "" : ((DateTime)currentRecord.Repair_EndTime).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 21].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Repair_Result;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.TotalTime;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.Labor_List;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.All_RepairCost;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.Update_Part;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.Repair_Remark;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }
        }
        public ActionResult DoExportEquipmentInfoNOTReprot(int plantId, int optypeId, int funplantId, string class_Desc, string mfg_Of_Asset)
        {
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            var apiUrl = string.Format("Equipmentmaintenance/ExportEquipmentInfoNOTReprotAPI?plantId={0}&optypeId={1}&funplantId={2}&class_Desc={3}&mfg_Of_Asset={4}&organization_UID={5}", plantId, optypeId, funplantId, class_Desc, mfg_Of_Asset, organization_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EquipmentReport>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("EquipmentInfoReprot");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "OP类型", "功能厂", "机台类型", "厂商名称", "机台数量" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("EquipmentInfo Reprot");

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
                    //{ "排序", "OP类型", "功能厂", "机台类型", "厂商名称", "机台数量" };
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.OP_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.FunPlant_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Mfg_Of_Asset;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.SumALL;
                }
                //   worksheet.Cells.AutoFitColumns();

                for (int i = 1; i <= 6; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:F1"].Style.Font.Bold = true;
                worksheet.Cells["A1:F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:F1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult DoExportEquipmentInfoReprot(int plantId, int optypeId, int funplantId, int project_UID, string class_Desc, string mfg_Of_Asset)
        {
            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }

            var apiUrl = string.Format("Equipmentmaintenance/ExportEquipmentInfoReprotAPI?plantId={0}&optypeId={1}&funplantId={2}&project_UID={3}&class_Desc={4}&mfg_Of_Asset={5}&organization_UID={6}", plantId, optypeId, funplantId, project_UID, class_Desc, mfg_Of_Asset, organization_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EquipmentReport>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("EquipmentInfo Reprot");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "OP类型", "专案", "功能厂", "机台类型", "厂商名称", "机台总数量", "维修中机台数量", "待备品机台数量", "可用机台数量" };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("EquipmentInfo Reprot");

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
                    // { "排序", "OP类型", "专案", "功能厂", "机台类型", "厂商名称", "机台总数量", "维修中机台数量", "待备品机台数量", "可用机台数量" };
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.OP_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Project_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Class_Desc;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Mfg_Of_Asset;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.SumALL;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.SumMaintenance;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.SumSpareparts;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.SumAvailable;
                }
                //worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 10; i++)
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
        /// 取得成本中心信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCostCtrs()
        {
            var apiUrl = "Equipmentmaintenance/GetCostCtrsAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion EQPMaintenance

        #region EQPMaintenanceReport
        public ActionResult EQPMaintenanceReport()
        {
            var apiUrl = "Equipmentmaintenance/EQPMaintenanceAPI";
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var errorinfo = JsonConvert.DeserializeObject<IEnumerable<EnumerationDTO>>(result);
            EQPRepairSearchVM currentVM = new EQPRepairSearchVM();
            currentVM.errorinfo = errorinfo;
            int Plant_Organization_UID = GetPlantOrgUid();
            //apiUrl = string.Format(@"Equipmentmaintenance/GetEQPLocationAPI?Plant_Organization_UID={0}", Plant_Organization_UID);
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var eqplocation = JsonConvert.DeserializeObject<List<string>>(result);
            //currentVM.eqplocation = eqplocation;
            currentVM.eqplocation = new List<string>();

            var bgUID = 0;
            var funplantUID = 0;
            var plantUID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                bgUID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
                funplantUID = CurrentUser.GetUserInfo.OrgInfo[0].Funplant_OrganizationUID.Value;
                plantUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            apiUrl = string.Format("Equipmentmaintenance/EQPAllUserAPI?bgUID={0}&funplantUID={1}&plantUID={2}", bgUID, funplantUID, plantUID);
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var userinfo = JsonConvert.DeserializeObject<IEnumerable<EQPUserTableDTO>>(result);
            currentVM.userinfo = userinfo;
            var roles = CurrentUser.GetUserInfo.RoleList;
            string strRole = "";
            foreach (var item in roles)
            {
                if (item.Role_ID == "EQPReportViewer" || item.Role_ID == "EQPSuperAdimin")
                {
                    currentVM.needcost = true;
                }
                strRole += item.Role_ID + ";";
            }
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {

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

            //apiUrl = "Equipmentmaintenance/EQPAllMaterialAPI";
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var materialinfo = JsonConvert.DeserializeObject<IEnumerable<MaterialInfoDTO>>(result);
            //currentVM.materialinfo = materialinfo;

            //apiUrl = "Equipmentmaintenance/EQPUnitMatAPI";
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var unitmat = JsonConvert.DeserializeObject<List<string>>(result);
            //currentVM.unitmat = unitmat;

            //apiUrl = string.Format("Equipmentmaintenance/QueryAllEquipmentAPI?AccountUId=1", CurrentUser.AccountUId);
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var equipinfo = JsonConvert.DeserializeObject<IEnumerable<EquipmentInfoDTO>>(result);
            //currentVM.equipmentinfo = equipinfo;

            currentVM.materialinfo = null;
            currentVM.unitmat = new List<string>();
            currentVM.equipmentinfo = null;

            //價格顯示限制，僅設備管理員有權限
            currentVM.showprice = false;
            if (CurrentUser.GetUserInfo.User_NTID.ToUpper() != "EQPUSER")
            {
                if (CurrentUser.GetUserInfo.RoleList != null)
                {
                    currentVM.showprice = (CurrentUser.GetUserInfo.RoleList.Where(x => x.Role_ID == "EQPAdmin").Count() > 0);
                }
            }
            return View("EQPMaintenanceReport", currentVM);
        }
        public ActionResult GetEQPLocation()
        {

            var apiUrl = string.Format(@"Equipmentmaintenance/GetEQPLocationAPI?Plant_Organization_UID={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        public ActionResult GetEQPAllMaterial()
        {
            var apiUrl = "Equipmentmaintenance/EQPAllMaterialAPI";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;         
            return Content(result, "application/json");

        }

        public ActionResult GetEQPUnitMat()
        {
            var apiUrl = "Equipmentmaintenance/EQPUnitMatAPI";
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
            //var unitmat = JsonConvert.DeserializeObject<List<string>>(result);
            //currentVM.unitmat = unitmat;
        }
            #endregion EQPMaintenanceReport
        public ActionResult QueryEQPMaintenanceReport(EQPRepairInfoSearchDTO search, Page page)
        {


            int organization_UID = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID != null && CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value != 0)
            {
                organization_UID = CurrentUser.GetUserInfo.OrgInfo[0].OPType_OrganizationUID.Value;
            }
            if (search.Organization_UID == 0 && organization_UID != 0)
            {
                search.Organization_UID = organization_UID;
            }

            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPMaintenanceReportAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #region CostCenterMaintenace Add By Darren 2018/12/17

        public ActionResult CostCenterMaintenance()
        {
            return View();
        }

        /// <summary>
        /// get paged records of CostCenters by query conditions
        /// </summary>
        /// <param name="search">query conditions</param>
        /// <param name="page">page info, auto fill by front-end</param>
        /// <returns>json of paged records</returns>
        public ActionResult QueryCostCenters(CostCenterModelSearch search, Page page)
        {
            var apiUrl = "Equipmentmaintenance/QueryCostCentersAPI";

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryCostCenter(int uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryCostCenterAPI?uid={0}", uid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult AddCostCenter(string jsonCostCenter)
        {

            var apiUrl = "Equipmentmaintenance/AddCostCenterAPI";
            var entity = JsonConvert.DeserializeObject<CostCtr_infoDTO>(jsonCostCenter);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult ModifyCostCenter(string jsonCostCenter)
        {

            var apiUrl = "Equipmentmaintenance/ModifyCostCenterAPI";
            var entity = JsonConvert.DeserializeObject<CostCtr_infoDTO>(jsonCostCenter);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        
        public ActionResult DeleteCostCenter(int uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteCostCenterAPI?uid={0}", uid);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult GetPlantsUseSettingsAPI()
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

        /// <summary>
        /// 获取当前用户的功能厂
        /// </summary>
        /// <param name="plant_OrganizationUID"></param>
        /// <returns></returns>
        public ActionResult GetOPTypeUseFixtureAPI(int plant_OrganizationUID)
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

        public ActionResult GetFunPlantUseFixtureAPI(int oporgid, string Optypes = "")
        {
            var apiUrl = string.Format("Fixture/GetFunPlantByOPTypeAPI?Optype={0}&Optypes={1}", oporgid, Optypes);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion //CostCenterMaintenace

        #region 机台类型、机台料号对应、每日开机-----------Robert2017/04/03
        #region 机台类型
        public ActionResult EQPType()
        {
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            if (Orgnizations != null && Orgnizations.Count > 0)
            {
                ViewBag.OP = Orgnizations[0].Plant;
            }
            else
            {
                ViewBag.OP = "0";
            }
            return View();
        }

        public ActionResult EditEQPType(string jsonEditOrg)
        {
            var apiUrl = "Equipmentmaintenance/EditEQPTypeAPI";
            var entity = JsonConvert.DeserializeObject<EQPTypeDTO>(jsonEditOrg);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult AddEQPType(string jsonAddOrg)
        {
            var apiUrl = "Equipmentmaintenance/AddEQPTypeAPI";
            var entity = JsonConvert.DeserializeObject<EQPTypeDTO>(jsonAddOrg);
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            //entity.BG_Organization_UID = Orgnizations[0].OPType_OrganizationUID.Value;
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryEQPTypes(EQPTypeDTO search, Page page)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPTypesAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DoAllExportEQPTypeReprot(EQPTypeDTO search)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("Equipmentmaintenance/DoAllExportEQPTypeReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EQPTypeDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("EQPType");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "OP类型", "功能厂", "机台类型", "说明", "是否启用", "修改人员", "修改日期" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("EQPType");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.EQP_Type1;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Type_Desc;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportEQPTypeReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("Equipmentmaintenance/DoExportEQPTypeReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<EQPTypeDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("EQPType");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "OP类型", "功能厂", "机台类型", "说明", "是否启用", "修改人员", "修改日期" };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("EQPType");
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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.BG;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.FunPlant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.EQP_Type1;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Type_Desc;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Is_Enable;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Modified_UserName;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public ActionResult QueryEQPType(int uuid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPTypeAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult QueryEQPByBgAndFunPlant(int bg, int funplant)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPTypeByBgAndFunplantAPI?bg={0}&funplant={1}", bg, funplant);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DeleteEQPType(int uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DelEQPTypeAPI?id={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }
        public ActionResult QueryFunctionPlant(int OPID)
        {
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;

            var apiUrl = string.Format("Equipmentmaintenance/QueryFunplantsAPI?oporgid={0}", OPID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion
        #region 导入仓库


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
                "BG",
                "功能厂",
                "仓库代码",
                "料架号",
                "储位号",
                 "描述",
                "是否启用"
            };
            return propertiesHead;
        }
        private string ImportEQPStorageExcelM(HttpPostedFileBase upload_excel)
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
                    var orgbomsUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomsMessage = APIHelper.APIGetAsync(orgbomsUrl);
                    var jsonorgbomsresult = orgbomsMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonorgbomsresult);
                    //获得所有仓库代码
                    var orgbomapiUrl = string.Format("StorageManage/GetAllWarehouseAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var Warehouses = JsonConvert.DeserializeObject<List<WarehouseBaseDTO>>(jsonresult);
                    //获取所有的储位信息
                    var warehouseStorageAPI = string.Format("StorageManage/GetWarehouseStorageALLAPI?Plant_UID={0}", GetPlantOrgUid());
                    HttpResponseMessage warehouseStorageAPIMessage = APIHelper.APIGetAsync(warehouseStorageAPI);
                    var jsonwarehouseStorageresult = warehouseStorageAPIMessage.Content.ReadAsStringAsync().Result;
                    var warehouseStorage = JsonConvert.DeserializeObject<List<WarehouseDTO>>(jsonwarehouseStorageresult);

                    List<Warehouse_StorageDTO> repairlocationlist = new List<Warehouse_StorageDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        Warehouse_StorageDTO RL = new Warehouse_StorageDTO();
                        int BG_Organization_UID = 0;
                        int FunPlant_Organization_UID = 0;
                        string BG_Organization1 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "BG")].Value);
                        string FunPlant_Organization1 = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        string warehouseCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库代码")].Value);
                        string Rack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料架号")].Value);
                        string Storage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "储位号")].Value);
                        string Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "描述")].Value);
                        string Is_Use = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        int wareHouseUID = 0;


                        if (string.IsNullOrWhiteSpace(BG_Organization1))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行op没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = orgboms.Where(m => m.BG == BG_Organization1).FirstOrDefault();
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

                        if (string.IsNullOrWhiteSpace(FunPlant_Organization1))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有功能厂", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasfunplant = orgboms.Where(m => m.BG == BG_Organization1 & m.Funplant == FunPlant_Organization1).FirstOrDefault();
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

                        if (string.IsNullOrWhiteSpace(warehouseCode))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库代码没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            //判断该仓库代码是否在数据库中
                            var hasbg = Warehouses.Where(m => m.Warehouse_ID == warehouseCode && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID).FirstOrDefault();
                            if (hasbg != null)
                            {
                                wareHouseUID = hasbg.Warehouse_UID;
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

                        var HasWarehouseCode = warehouseStorage.FirstOrDefault(m => m.Warehouse_UID == wareHouseUID && m.Rack_ID == Rack_ID && m.Storage_ID == Rack_ID);
                        if (HasWarehouseCode != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行储位已经存在,不可重复导入", i);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = repairlocationlist.Exists(m => m.Warehouse_UID == wareHouseUID && m.Rack_ID == Rack_ID && m.Storage_ID == Storage_ID);
                        if (isSelfRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行导入数据中重复,不可重复导入", i);
                            return errorInfo;
                        }

                        RL.Warehouse_UID = wareHouseUID;
                        RL.Rack_ID = Rack_ID;
                        RL.Storage_ID = Storage_ID;
                        RL.Desc = Desc;
                        RL.Is_Enable = Is_Enable == "1" ? true : false;
                        RL.Modified_UID = CurrentUser.AccountUId;
                        RL.Modified_Date = DateTime.Now;
                        repairlocationlist.Add(RL);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(repairlocationlist);
                    var apiInsertVendorInfoUrl = string.Format("StorageManage/InsertWarehouse_StorageAPI");
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

                "BG",
                "功能厂",
                "仓库类型",
                "仓库代码",
                "仓库名",
                "仓库英文名",
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
                    var warehouseStAPI = "StorageManage/QueryAllWarehouseStAPI";
                    HttpResponseMessage WarhouseStmessage = APIHelper.APIGetAsync(warehouseStAPI);
                    var jsonwarst = WarhouseStmessage.Content.ReadAsStringAsync().Result;
                    var warehousest = JsonConvert.DeserializeObject<List<WarehouseStorageDTO>>(jsonwarst);

                    //获取所有仓库类型


                    var apiUrl = string.Format("StorageManage/QueryWarehouseTypesAPI");
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = responMessage.Content.ReadAsStringAsync().Result;
                    var types = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);

                    List<WarehouseBaseDTO> repairlocationlist = new List<WarehouseBaseDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        WarehouseBaseDTO RL = new WarehouseBaseDTO();
                        int warehouseTypeUID = 0;
                        int BG_Organization_UID = 0;
                        int FunPlant_Organization_UID = 0;
                        string WarehouseType_N = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库类型")].Value).Trim();
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "BG")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        string WarehouseCode = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库代码")].Value);
                        string WarehouseName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库名")].Value);
                        string WarehouseEname = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库英文名")].Value);
                        string Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "说明")].Value);
                        string Is_Use = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(WarehouseType_N))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库类型没有值", i);
                            return errorInfo;
                        }
                        else
                        {

                            var hasbg = types.Where(m => m.Enum_Value == WarehouseType_N).FirstOrDefault();
                            if (hasbg != null)
                            {
                                warehouseTypeUID = hasbg.Enum_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行仓库类型的值没有找到", i);
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
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有功能厂", i);
                            return errorInfo;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
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


                        if (string.IsNullOrWhiteSpace(WarehouseCode))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库代码没有值", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(WarehouseName))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库名没有值", i);
                            return errorInfo;
                        }

                        var HasWarehouseCode = warehousest.FirstOrDefault(m => m.Warehouse_ID == WarehouseCode && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                        if (HasWarehouseCode != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行仓库代码已经存在,不可重复导入", i);
                            return errorInfo;
                        }

                        //导入数据判重
                        var isSelfRepeated = repairlocationlist.Exists(m => m.Warehouse_ID == WarehouseCode && m.BG_Organization_UID == BG_Organization_UID && m.FunPlant_Organization_UID == FunPlant_Organization_UID);
                        if (isSelfRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行导入数据中重复,不可重复导入", i);
                            return errorInfo;
                        }

                        RL.BG_Organization_UID = BG_Organization_UID;
                        RL.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        RL.Warehouse_Type_UID = warehouseTypeUID;
                        RL.Warehouse_ID = WarehouseCode;
                        RL.Name_EN = WarehouseEname;
                        RL.Name_ZH = WarehouseName;
                        RL.Desc = Desc;
                        RL.Is_Enable = Is_Enable == "1" ? true : false;
                        RL.Modified_UID = CurrentUser.AccountUId;
                        RL.Modified_Date = DateTime.Now;
                        repairlocationlist.Add(RL);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(repairlocationlist);
                    var apiInsertVendorInfoUrl = string.Format("StorageManage/InsertWarehouseAPI");
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


        #endregion
        public string ImportEQPTypeExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = ImportEQPTypeExcelM(upload_excel);
            return errorInfo;
        }
        private string[] GetEQPTypeHeadColumn()
        {
            var propertiesHead = new[]
            {
                "机台类型",
                "BG",
                "功能厂",
                "说明",
                "是否启用"
            };
            return propertiesHead;
        }
        private string ImportEQPTypeExcelM(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetEQPTypeHeadColumn();
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

                    //获取所有现有的设备类型

                    var orgbomapiUrl1 = string.Format("Equipmentmaintenance/GetEQPTypesAPI");
                    HttpResponseMessage orgbomMessage1 = APIHelper.APIGetAsync(orgbomapiUrl1);
                    var jsonresult1 = orgbomMessage1.Content.ReadAsStringAsync().Result;
                    var orgboms1 = JsonConvert.DeserializeObject<List<EQPTypeBaseDTO>>(jsonresult1);

                    List<EQPTypeDTO> repairlocationlist = new List<EQPTypeDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        EQPTypeDTO RL = new EQPTypeDTO();

                        int BG_Organization_UID = 0;
                        int FunPlant_Organization_UID = 0;
                        string EQPType_N = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台类型")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "BG")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        string Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "说明")].Value);
                        string Is_Use = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        if (string.IsNullOrWhiteSpace(EQPType_N))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行设备类型没有值", i);
                            return errorInfo;
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
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有功能厂", i);
                            return errorInfo;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
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

                        var hasEQPType = orgboms1.FirstOrDefault(m => m.BG_Organization_UID == BG_Organization_UID & m.EQP_Type1 == EQPType_N);
                        if (hasEQPType != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行设备类型已经存在,不可重复导入", i);
                            return errorInfo;
                        }


                        RL.BG_Organization_UID = BG_Organization_UID;
                        RL.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        RL.EQP_Type1 = EQPType_N;
                        RL.Type_Desc = Desc;
                        RL.Is_Enable = Is_Enable == "1" ? true : false;
                        RL.Modified_UID = CurrentUser.AccountUId;
                        RL.Modified_Date = DateTime.Now;
                        repairlocationlist.Add(RL);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(repairlocationlist);
                    var apiInsertVendorInfoUrl = string.Format("Equipmentmaintenance/InsertEQPTypeAPI");
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
        #region 机型料号对应
        public ActionResult EQPMaterial()
        {
            List<OrganiztionVM> Orgnizations = CurrentUser.GetUserInfo.OrgInfo;
            if (Orgnizations.Count > 0)
                ViewBag.OP = Orgnizations[0].Plant;
            return View();
        }

        public ActionResult QueryEQPMaterials(EQPMaterialDTO search, Page page)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPMaterialsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public ActionResult QueryMaterialByMaterial_Id(string id)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryMaterialByMaterialIdAPI?uuid={0}", id);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportEQPMaterialExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = ImportEQPMaterial(upload_excel);
            return errorInfo;
        }
        private string[] GetEQPMaterialHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区",
                "BG",
                "功能厂",
                "机台类型",
                "料号",
                "品名",
                "型号",
                "单机用量",
                "是否启用",
                "说明"

            };
            return propertiesHead;
        }
        private string ImportEQPMaterial(HttpPostedFileBase upload_excel)
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
                    var propertiesHead = GetEQPMaterialHeadColumn();
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
                    // 获取所有料号关系数据 
                    var eQPMaterialUrl = string.Format("Equipmentmaintenance/QueryEQPMaterialsAllAPI");
                    HttpResponseMessage eQPMaterialMessage = APIHelper.APIGetAsync(eQPMaterialUrl);
                    var eQPMaterialresult = eQPMaterialMessage.Content.ReadAsStringAsync().Result;
                    var eQPMaterials = JsonConvert.DeserializeObject<List<EQPMaterialDTO>>(eQPMaterialresult);

                    //获取所有料号
                    var materialUrl = string.Format("Equipmentmaintenance/QueryMaterialByMaterialAPI");
                    HttpResponseMessage materialMessage = APIHelper.APIGetAsync(materialUrl);
                    var materialresult = materialMessage.Content.ReadAsStringAsync().Result;
                    var materials = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(materialresult);
                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                    //获取所有现有的设备类型

                    var orgbomapiUrl1 = string.Format("Equipmentmaintenance/GetEQPTypeAllsAPI");
                    HttpResponseMessage orgbomMessage1 = APIHelper.APIGetAsync(orgbomapiUrl1);
                    var jsonresult1 = orgbomMessage1.Content.ReadAsStringAsync().Result;
                    var orgboms1 = JsonConvert.DeserializeObject<List<EQPTypeDTO>>(jsonresult1);

                    List<EQPMaterialDTO> repairlocationlist = new List<EQPMaterialDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        EQPMaterialDTO RL = new EQPMaterialDTO();
                        int Plant_UID = 0;
                        int BG_Organization_UID = 0;
                        int FunPlant_Organization_UID = 0;
                        int EQP_Type_UID = 0;
                        int Material_UID = 0;
                        Decimal DecimalQty = 0;
                        string Plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "BG")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                        string EQPType_N = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台类型")].Value);
                        string Material_Id = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                        string Material_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                        string Material_Type = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "型号")].Value);
                        string Qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "单机用量")].Value);
                        //  string Is_Use = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);
                        string Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "说明")].Value);
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
                                Plant_UID = hasbg.Plant_Organization_UID;
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
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有功能厂", i);
                            return errorInfo;
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
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(EQPType_N))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行设机台型没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasEQP_Type = orgboms1.Where(o => o.EQP_Type1 == EQPType_N && o.BG_Organization_UID == BG_Organization_UID && o.FunPlant_Organization_UID == FunPlant_Organization_UID).FirstOrDefault();
                            if (hasEQP_Type != null)
                            {
                                EQP_Type_UID = hasEQP_Type.EQP_Type_UID;
                            }
                            else
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行机台类型的值没有找到", i);
                                return errorInfo;
                            }

                        }

                        if (string.IsNullOrWhiteSpace(Material_Id))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料号没有值", i);
                            return errorInfo;
                        }

                        //if (string.IsNullOrWhiteSpace(Material_Name))
                        //{
                        //    excelIsError = true;
                        //    errorInfo = string.Format("第{0}行品名没有值", i);
                        //    return errorInfo;
                        //}
                        //if (string.IsNullOrWhiteSpace(Material_Type))
                        //{
                        //    excelIsError = true;
                        //    errorInfo = string.Format("第{0}行型号没有值", i);
                        //    return errorInfo;
                        //}

                        var hasMaterials = materials.Where(o => o.Organization_UID == Plant_UID && o.Material_Id == Material_Id).FirstOrDefault();
                        if (hasMaterials != null)
                        {
                            Material_UID = hasMaterials.Material_Uid;
                        }
                        else
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有找到对应OP的料号", i);
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



                        if (string.IsNullOrWhiteSpace(Qty))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行单机用量没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            try
                            {
                                DecimalQty = Convert.ToDecimal(Qty);
                            }
                            catch
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行单机用量的值不正确", i);
                                return errorInfo;
                            }

                        }

                        // repairlocationlist

                        //
                        var hasEQPType = eQPMaterials.FirstOrDefault(m => m.EQP_Type_UID == EQP_Type_UID & m.EQP_Material_UID == Material_UID);
                        if (hasEQPType != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行设备类型已经存在,不可重复导入", i);
                            return errorInfo;
                        }
                        //导入数据判重
                        var isSelfRepeated = repairlocationlist.Exists(m => m.EQP_Type_UID == EQP_Type_UID && m.Material_Uid == Material_UID);
                        if (isSelfRepeated)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行机台类型[{1}]，料号[{2}]在导入数据中重复,不可重复导入", i, EQPType_N, Material_Id);
                            return errorInfo;
                        }

                        RL.BG_Organization_UID = BG_Organization_UID;
                        RL.FunPlant_Organization_UID = FunPlant_Organization_UID.ToString();
                        RL.EQP_Type_UID = EQP_Type_UID;
                        RL.Material_Uid = Material_UID;
                        RL.BOM_Qty = DecimalQty;
                        RL.Desc = Desc;
                        RL.Is_Enable = Is_Enable == "1" ? true : false;
                        RL.Modified_UID = CurrentUser.AccountUId;
                        RL.Modified_Date = DateTime.Now;
                        repairlocationlist.Add(RL);
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(repairlocationlist);
                    var apiInsertVendorInfoUrl = string.Format("Equipmentmaintenance/InsertEQPMaterialAPI");
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

        public ActionResult AddorEditEQPMaterial(bool isEdit, string editValue)
        {
            var apiUrl = "Equipmentmaintenance/AddEQPMaterialAPI";
            var entity = JsonConvert.DeserializeObject<List<EQPMaterialDTO>>(editValue);
            foreach (var m in entity)
            {
                m.Modified_UID = this.CurrentUser.AccountUId;
                m.Modified_Date = DateTime.Now;
                m.Is_Enable = true;

            }
            var json = JsonConvert.SerializeObject(entity);
            if (isEdit)
            {
                apiUrl = "Equipmentmaintenance/EditEQPMaterialAPI";
            }
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult DeleteEQPMaterial(int uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DelEQPMaterialAPI?uuid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryEQPMaterial(int uuid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPMaterialAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryEQPMaterialByEqp(int eqp)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPMaterialByEqpAPI?eqp={0}", eqp);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion
        #region 每日开机
        public ActionResult EQPPowerOn()
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

        public ActionResult EditPowerOn(string jsonEditOrg)
        {
            var apiUrl = "Equipmentmaintenance/EditEQPPowerOnAPI";
            var entity = JsonConvert.DeserializeObject<EQPPowerOnDTO>(jsonEditOrg);
            entity.PowerOn_Date = DateTime.Parse(entity.PowerOnDateString);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            if (entity.Modified_UID > 0)
            {
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;

                return Content(result, "application/json");
            }
            else
            {
                return Content("操作失败：当前用户登录ID获取失败，请退出并重新登录", "application/json");
            }
        }

        public ActionResult AddPowerOn(string jsonAddOrg)
        {
            var apiUrl = "Equipmentmaintenance/AddEQPPowerOnAPI";
            var entity = JsonConvert.DeserializeObject<EQPPowerOnDTO>(jsonAddOrg);
            entity.PowerOn_Date = DateTime.Parse(entity.PowerOnDateString);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            if (entity.Modified_UID > 0)
            {
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;

                return Content(result, "application/json");
            }
            else
            {
                return Content("操作失败：当前用户登录ID获取失败，请退出并重新登录", "application/json");
            }
        }

        public ActionResult QueryEQPPowerOns(EQPPowerOnDTO search, Page page)
        {


            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPPowerOnsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryEQPPowerOn(int uuid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryEQPPowerOnAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteEQPPowerOn(int uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DelEQPPowerOnAPI?uuid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #region 每日开机模板导入
        public string ImportPowerOnExcel(HttpPostedFileBase upload_excel, string hid_currentOrNextWeek)
        {
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
                string[] propertiesHead = new string[] { };
                //bool flag = false;
                //switch (hid_currentOrNextWeek)
                //{
                //    case "nextWeek":
                //        flag = true;
                //        break;
                //    case "currentWeek":
                //        flag = false;
                //        break;
                //}

                bool allColumnsAreError = false;



                List<EQPPowerOnDTO> mgDataList = new List<EQPPowerOnDTO>();
                List<EQPForecastPowerOnDTO> mgDataList1 = new List<EQPForecastPowerOnDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                //int iColumn;
                //int j = 2;


                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }

                    EQPPowerOnDTO newMGDataItem = new EQPPowerOnDTO();


                    var plantId = 0;
                    var plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);
                    if (string.IsNullOrWhiteSpace(plant))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行厂别没有值", iRow);
                        break;
                    }
                    else
                    {

                        var apiUrl1 = string.Format("Equipmentmaintenance/QueryOrgByNameAPI?name={0}", plant);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        var Org = JsonConvert.DeserializeObject<SystemOrgDTO>(result1);
                        if (Org != null && Org.Organization_UID > 0)
                        {
                            plantId = Org.Organization_UID;
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,厂别输入不正确，没有找到该厂", iRow);
                            break;
                        }
                    }

                    var opId = 0;
                    var op = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(op))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行OP没有值", iRow);
                        break;
                    }
                    else
                    {
                        var apiUrl1 = string.Format("Equipmentmaintenance/CheckOPAPI?name={0}&plant={1}", op, plantId);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        if (int.Parse(result1) > 0)
                        {
                            opId = int.Parse(result1);
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,OP输入不正确", iRow);
                            break;
                        }
                    }

                    var funPlantId = 0;
                    var funPlant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(funPlant))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行功能厂没有值", iRow);
                        break;
                    }
                    else
                    {
                        var apiUrl1 = string.Format("Equipmentmaintenance/CheckFunplantAPI?name={0}&op={1}", funPlant, opId);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        if (int.Parse(result1) > 0)
                        {
                            funPlantId = int.Parse(result1);
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,功能厂输入不正确", iRow);
                            break;
                        }
                    }

                    var EQPTypeId = 0;
                    var EQPType = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(EQPType))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行机台类型没有值", iRow);
                        break;
                    }
                    else
                    {
                        var apiUrl1 = string.Format("Equipmentmaintenance/CheckEQPTypeAPI?op={0}&funplant={1}&name={2}", opId, funPlantId, EQPType);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        if (int.Parse(result1) > 0)
                        {
                            EQPTypeId = int.Parse(result1);
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,对应功能厂中找不到该机台或机台名称错误", iRow);
                            break;
                        }
                    }

                    var nowDate = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(nowDate))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行开机日期没有值", iRow);
                        break;
                    }
                    else
                    {
                        try
                        {
                            var apiUrl1 = string.Format("Equipmentmaintenance/CheckEQPPowerOnAPI?eqpType={0}&date={1}", EQPTypeId, DateTime.Parse(nowDate));
                            HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);
                            if (DateTime.Parse(nowDate) > DateTime.Now)
                            {
                                allColumnsAreError = true;
                                errorInfo = string.Format("第{0}行开机日期不能大于当前日期", iRow);
                                break;
                            }
                            var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                            if (int.Parse(result1) > 0)
                            {
                                allColumnsAreError = true;
                                errorInfo = string.Format("第{0}行,该机台该日期开机数已经存在", iRow);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行开机日期不是正确的日期格式", iRow);
                            break;
                        }
                    }

                    var qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(qty))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行开机数量没有值", iRow);
                        break;
                    }
                    int num = 0;
                    bool checkflag = int.TryParse(qty, out num);
                    if (!checkflag)
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行开机数量不是整数值", iRow);
                        break;
                    }

                    newMGDataItem.Daily_PowerOn_Qty = num;
                    newMGDataItem.EQP_Type_UID = EQPTypeId;
                    newMGDataItem.PowerOn_Date = DateTime.Parse(nowDate);
                    newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                    newMGDataItem.Modified_Date = DateTime.Now;

                    mgDataList.Add(newMGDataItem);
                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("Equipmentmaintenance/AddEQPPowerOnListAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
            }
            return errorInfo;
        }
        #endregion


        #region 每日预计开机模板导入
        public string ImportForecastPowerOnExcel(HttpPostedFileBase upload_excel, string hid_currentOrNextWeek)
        {
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
                string[] propertiesHead = new string[] { };
                //bool flag = false;
                //switch (hid_currentOrNextWeek)
                //{
                //    case "nextWeek":
                //        flag = true;
                //        break;
                //    case "currentWeek":
                //        flag = false;
                //        break;
                //}

                bool allColumnsAreError = false;




                List<EQPForecastPowerOnDTO> mgDataList = new List<EQPForecastPowerOnDTO>();
                //Excel行号
                int iRow = 2;
                //Excel列号
                //int iColumn;
                //int j = 2;


                for (iRow = 2; iRow <= totalRows; iRow++)
                {
                    if (allColumnsAreError)
                    {
                        break;
                    }

                    EQPForecastPowerOnDTO newMGDataItem = new EQPForecastPowerOnDTO();


                    var plantId = 0;
                    var plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);
                    if (string.IsNullOrWhiteSpace(plant))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行厂别没有值", iRow);
                        break;
                    }
                    else
                    {
                        var apiUrl1 = string.Format("Equipmentmaintenance/QueryOrgByNameAPI?name={0}", plant);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        var Org = JsonConvert.DeserializeObject<SystemOrgDTO>(result1);
                        if (Org != null && Org.Organization_UID > 0)
                        {
                            plantId = Org.Organization_UID;
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,厂别输入不正确，没有找到该厂", iRow);
                            break;
                        }
                    }

                    var opId = 0;
                    var op = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(op))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行OP没有值", iRow);
                        break;
                    }
                    else
                    {
                        var apiUrl1 = string.Format("Equipmentmaintenance/CheckOPAPI?name={0}&plant={1}", op, plantId);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        if (int.Parse(result1) > 0)
                        {
                            opId = int.Parse(result1);
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,OP输入不正确", iRow);
                            break;
                        }
                    }

                    var funPlantId = 0;
                    var funPlant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(funPlant))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行功能厂没有值", iRow);
                        break;
                    }
                    else
                    {
                        var apiUrl1 = string.Format("Equipmentmaintenance/CheckFunplantAPI?name={0}&op={1}", funPlant, opId);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        if (int.Parse(result1) > 0)
                        {
                            funPlantId = int.Parse(result1);
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,功能厂输入不正确", iRow);
                            break;
                        }
                    }

                    var EQPTypeId = 0;
                    var EQPType = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(EQPType))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行机台类型没有值", iRow);
                        break;
                    }
                    else
                    {
                        var apiUrl1 = string.Format("Equipmentmaintenance/CheckEQPTypeAPI?op={0}&funplant={1}&name={2}", opId, funPlantId, EQPType);
                        HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        if (int.Parse(result1) > 0)
                        {
                            EQPTypeId = int.Parse(result1);
                        }
                        else
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行,对应功能厂中找不到该机台或机台名称错误", iRow);
                            break;
                        }
                    }

                    var nowDate = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(nowDate))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行开机日期没有值", iRow);
                        break;
                    }
                    else
                    {

                        //var apiUrl1 = string.Format("Equipmentmaintenance/CheckEQPForecastPowerOnAPI?eqpType={0}&date={1}", EQPTypeId, DateTime.Parse(nowDate));
                        //HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);

                        //var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                        //if (int.Parse(result1) > 0)
                        //{
                        //    allColumnsAreError = true;
                        //    errorInfo = string.Format("第{0}行,该机台当时日期开机数已经存在", iRow);
                        //    break;
                        //}
                        try
                        {
                            var apiUrl1 = string.Format("Equipmentmaintenance/CheckEQPForecastPowerOnAPI?eqpType={0}&date={1}", EQPTypeId, DateTime.Parse(nowDate));
                            HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);
                            if (DateTime.Parse(nowDate) < DateTime.Now)
                            {
                                allColumnsAreError = true;
                                errorInfo = string.Format("第{0}行预计开机日期不能小于当前日期", iRow);
                                break;
                            }
                            var result1 = responMessage1.Content.ReadAsStringAsync().Result;
                            if (int.Parse(result1) > 0)
                            {
                                allColumnsAreError = true;
                                errorInfo = string.Format("第{0}行,该机台该日期开机数已经存在", iRow);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            allColumnsAreError = true;
                            errorInfo = string.Format("第{0}行开机日期不是正确的日期格式", iRow);
                            break;
                        }
                    }

                    var qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value);
                    //如果生产计划为空则默认为0
                    if (string.IsNullOrWhiteSpace(qty))
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行开机数量没有值", iRow);
                        break;
                    }
                    int num = 0;
                    bool checkflag = int.TryParse(qty, out num);
                    if (!checkflag)
                    {
                        allColumnsAreError = true;
                        errorInfo = string.Format("第{0}行开机数量不是整数值", iRow);
                        break;
                    }

                    newMGDataItem.PowerOn_Qty = num;
                    newMGDataItem.EQP_Type_UID = EQPTypeId;
                    newMGDataItem.Demand_Date = DateTime.Parse(nowDate);
                    newMGDataItem.Modified_UID = this.CurrentUser.AccountUId;
                    newMGDataItem.Modified_Date = DateTime.Now;

                    mgDataList.Add(newMGDataItem);
                }

                if (allColumnsAreError)
                {
                    return errorInfo;
                }

                var json = JsonConvert.SerializeObject(mgDataList);
                string api = string.Format("Equipmentmaintenance/AddForecastPowerOnListAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, api);
            }
            return errorInfo;
        }
        #endregion

        public ActionResult EQPForecastPowerOn()
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

        public ActionResult EditForecastPowerOn(string jsonEditOrg)
        {
            var apiUrl = "Equipmentmaintenance/EditForecastPowerOnAPI";
            var entity = JsonConvert.DeserializeObject<EQPForecastPowerOnDTO>(jsonEditOrg);
            entity.Demand_Date = DateTime.Parse(entity.PowerOnDateString);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            if (entity.Modified_UID > 0)
            {
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;

                return Content(result, "application/json");
            }
            else
            {
                return Content("操作失败：当前用户登录ID获取失败，请退出并重新登录", "application/json");
            }
        }

        public ActionResult AddForecastPowerOn(string jsonAddOrg)
        {
            var apiUrl = "Equipmentmaintenance/AddForecastPowerOnAPI";
            var entity = JsonConvert.DeserializeObject<EQPForecastPowerOnDTO>(jsonAddOrg);
            entity.Demand_Date = DateTime.Parse(entity.PowerOnDateString);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            entity.Modified_Date = DateTime.Now;
            if (entity.Modified_UID > 0)
            {
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;

                return Content(result, "application/json");
            }
            else
            {
                return Content("操作失败：当前用户登录ID获取失败，请退出并重新登录", "application/json");
            }
        }

        public ActionResult QueryForecastEQPPowerOns(EQPForecastPowerOnDTO search, Page page)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("Equipmentmaintenance/QueryForecastPowerOnsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryForecastEQPPowerOn(int uuid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryForecastPowerOnAPI?uuid={0}", uuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteForecastEQPPowerOn(int uid)
        {

            var apiUrl = string.Format("Equipmentmaintenance/DelForecastPowerOnAPI?uuid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        #endregion
        #endregion
        #region 重要料号原因维护 

        public ActionResult MaterialReason()
        {

            int planId = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                planId = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            var apiUrl = "Equipmentmaintenance/EQPAllMaterialAPI?planId=" + planId;
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var materialinfo = JsonConvert.DeserializeObject<IEnumerable<MaterialInfoDTO>>(result);
            ViewBag.mats = materialinfo;
            return View();
        }

        public ActionResult QueryMatReason(MaterialReasonDTO search, Page page)
        {
            search.PlantId = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                search.PlantId = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            var apiUrl = string.Format("Equipmentmaintenance/QueryMatReasonAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditMaterialReason(MaterialReasonDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Equipmentmaintenance/AddOrEditMaterialReasonAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult QuerymatReasonByuid(int Material_Reason_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QuerymatReasonByuidAPI?Material_Reason_UID={0}", Material_Reason_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string DeleteMatReason(int Material_Reason_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteMatReasonAPI?Material_Reason_UID={0}&userid={1}", Material_Reason_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public ActionResult GetMatReasonByMat(int Material_Uid)
        {
            var apiUrl = string.Format("Equipmentmaintenance/GetMatReasonByMatAPI?Material_Uid={0}", Material_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public FileResult DownloadMatReasonExcel()
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("导入重要料号原因模板");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetMatReasonHeadColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("导入重要料号原因模板");
                SetMatReasonExcelStyle(worksheet, propertiesHead);
                //设置灰色背景
                var colorRange = string.Format("A1:B1");
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:B1")].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:B1")].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:B1")].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("导入重要料号原因模板说明");
                SetMatReasonExcelStyle(worksheetsm, propertiesHead);
                SetMatReasonExcelStylesm(worksheetsm);
                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:B1")].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:B1")].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:B1")].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        private void SetMatReasonExcelStylesm(ExcelWorksheet worksheet)
        {
            //填充Title内容
            worksheet.Cells[2, 1].Value = "ASKG003(料号)";
            worksheet.Cells[2, 2].Value = "损坏(原因)";


        }
        public string ImportMatReasonExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            errorInfo = AddMatReasonExcel(upload_excel);
            return errorInfo;
        }

        private string AddMatReasonExcel(HttpPostedFileBase upload_excel)
        {
            string errorInfo = string.Empty;
            try
            {
                var plantids = new List<int>();
                if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                {

                    plantids.Add(CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value);
                }
                else
                {
                    //获取
                    var apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                    var responMessage = APIHelper.APIGetAsync(apiUrl);
                    var result = responMessage.Content.ReadAsStringAsync().Result;
                    var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                    foreach (var item in systemOrgDTOs)
                    {
                        plantids.Add(item.Organization_UID);
                    }
                }

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
                    var propertiesHead = GetMatReasonHeadColumn();

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
                    //读取Excel并进行验证判断
                    var matreasonAPI = "Equipmentmaintenance/QueryAllMaterialAPI";
                    HttpResponseMessage matreasonmessage = APIHelper.APIGetAsync(matreasonAPI);
                    var jsonMatreason = matreasonmessage.Content.ReadAsStringAsync().Result;
                    var functionMatReason = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(jsonMatreason);
                    List<MaterialReasonDTO> matreasonlist = new List<MaterialReasonDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        string Material = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                        if (string.IsNullOrWhiteSpace(Material))
                        {
                            continue;
                        }

                        MaterialReasonDTO MatReason = new MaterialReasonDTO();
                        var matreasonitem = functionMatReason.FirstOrDefault(m => m.Material_Id == Material && plantids.Contains(m.Organization_UID.Value));
                        if (matreasonitem == null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料号不存在", i);
                            return errorInfo;
                        }
                        MatReason.Material_UID = matreasonitem.Material_Uid;


                        string Reason = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "原因")].Value);

                        if (string.IsNullOrWhiteSpace(Reason))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行原因错误", i);
                            return errorInfo;
                        }
                        MatReason.Reason = Reason;
                        MatReason.Modified_UID = CurrentUser.AccountUId;
                        matreasonlist.Add(MatReason);
                    }
                    if (matreasonlist.Count > 0)
                    {
                        //插入表
                        var json2 = JsonConvert.SerializeObject(matreasonlist);
                        var apiInsertEquipmentUrl2 = string.Format("Equipmentmaintenance/InsertMatReasonAPI");
                        HttpResponseMessage responMessage2 = APIHelper.APIPostAsync(json2, apiInsertEquipmentUrl2);
                        errorInfo = responMessage2.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    }
                }
            }
            catch (Exception e)
            {
                errorInfo = "导入异常！" + e.ToString();
            }
            return errorInfo;
        }

        private string[] GetMatReasonHeadColumn()
        {
            var propertiesHead = new[]
            {
                "料号","原因"
            };
            return propertiesHead;
        }

        private void SetMatReasonExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            //填充Title内容
            worksheet.Cells[1, 1].Value = propertiesHead[0];
            worksheet.Cells[1, 2].Value = propertiesHead[1];

            //设置列宽
            for (int i = 1; i <= 2; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:B1"].Style.Font.Bold = true;
            worksheet.Cells["A1:B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:B1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        public ActionResult DoAllExportMatReasonReprot(MaterialReasonDTO search)
        {

            search.PlantId = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                search.PlantId = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
            }
            //get Export datas

            var apiUrl = "Equipmentmaintenance/DoAllExportMatReasonReprotAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialReasonDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Material Reason");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "料号", "品名", "型号", "原因", "修改者", "修改时间" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Material Reason");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Reason;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.ModifiedUser;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 7; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult DoExportMatReasonReprot(string Material_Reason_UIDs)
        {
            //get Export datas
            var apiUrl = string.Format("Equipmentmaintenance/DoExportMatReasonReprotAPI?Material_Reason_UIDs={0}", Material_Reason_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialReasonDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Material Reason");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "料号", "品名", "型号", "原因", "修改者", "修改时间" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Material Reason");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Reason;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.ModifiedUser;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Modified_Date.ToString(FormatConstants.DateTimeFormatString);

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 7; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion 重要料号原因维护    
        #region 未使用机台报表，适用机台报表
        public ActionResult EquipmentNOTReport()
        {
            EquipmentReportVM currentVM = new EquipmentReportVM();
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
                var errorinfo = JsonConvert.DeserializeObject<IEnumerable<EnumerationDTO>>(result);
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            return View("EquipmentNOTReport", currentVM);
        }
        public ActionResult EquipmentReport()
        {
            EquipmentReportVM currentVM = new EquipmentReportVM();
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
                var errorinfo = JsonConvert.DeserializeObject<IEnumerable<EnumerationDTO>>(result);
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {

                    PlantVM plantvm = new PlantVM() { Plant = item.Organization_Name, Plant_OrganizationUID = item.Organization_UID };
                    Plants.Add(plantvm);
                }
                currentVM.Plants = Plants;
            }
            return View("EquipmentReport", currentVM);
        }
        #endregion

        #region 备品维修作业维护
        public ActionResult Material_Maintenance_Record()
        {
            Material_Maintenance_RecordVM currentVM = new Material_Maintenance_RecordVM();
            int planId = GetPlantOrgUid();
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

            var apiUrl1 = "Equipmentmaintenance/EQPAllMaterialAPI?planId=" + planId;
            HttpResponseMessage responMessage1 = APIHelper.APIGetAsync(apiUrl1);
            var result1 = responMessage1.Content.ReadAsStringAsync().Result;
            var materialinfo = JsonConvert.DeserializeObject<IEnumerable<MaterialInfoDTO>>(result1);
            currentVM.materialinfo = materialinfo;

            return View("Material_Maintenance_Record", currentVM);
        }

        public ActionResult QueryMaterial_Maintenance_Records(Material_Maintenance_RecordDTO search, Page page)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("Equipmentmaintenance/QueryMaterial_Maintenance_RecordsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult AddOrEditMaterial_Maintenance_Record(Material_Maintenance_RecordDTO dto)
        {

            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Submit_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Equipmentmaintenance/AddOrEditMaterial_Maintenance_RecordAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        public ActionResult QueryMaterial_Maintenance_RecordByUid(int Material_Maintenance_Record_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryMaterial_Maintenance_RecordByUidAPI?Material_Maintenance_Record_UID={0}", Material_Maintenance_Record_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string Approve1Material_Maintenance_RecordByUid(int Material_Maintenance_Record_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/Approve1Material_Maintenance_RecordByUidAPI?Material_Maintenance_Record_UID={0}&Useruid={1}", Material_Maintenance_Record_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public ActionResult Approve2Material_Maintenance_Record(Material_Maintenance_RecordDTO dto)
        {

            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Return_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("Equipmentmaintenance/Approve2Material_Maintenance_RecordAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }


        public string Approve3Material_Maintenance_RecordByUid(int Material_Maintenance_Record_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/Approve3Material_Maintenance_RecordByUidAPI?Material_Maintenance_Record_UID={0}&Useruid={1}", Material_Maintenance_Record_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public string DeleteMaterial_Maintenance_Record(int Material_Maintenance_Record_UID)
        {
            var apiUrl = string.Format("Equipmentmaintenance/DeleteMaterial_Maintenance_RecordAPI?Material_Maintenance_Record_UID={0}", Material_Maintenance_Record_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        /// <summary>
        /// 导出治具资料
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult DoAllExportMaterial_Maintenance_Record(Material_Maintenance_RecordDTO search)
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

            var apiUrl = "Equipmentmaintenance/DoAllExportMaterial_Maintenance_RecordAPI";
            var responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Material_Maintenance_RecordDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Material_Maintenance_Record");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "状态", "厂区", "OP类型", "功能厂", "料号", "设备备品序号", "是否为保固期", "送修日期", "预计完成日期", "维修厂商", "维修项目", "维修费用", "维修天数", "提交人员", "判定人员", "回厂日期", "回厂日期填写人员", "验收人员", "备注" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Material_Maintenance_Record");

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
                    worksheet.Cells[index + 2, 2].Value = GetStatus(currentRecord.Status_UID);
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Seq;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Warranty.Value ? "是" : "否";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Delivery_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Expected_Return_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Vendor;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Maintenance_Items;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Maintenance_Fees;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Maintenance_Days;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Submiter;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Judger;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Return_Date.Value.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Returner;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Acceptancer;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Notes;

                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 20; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:T1"].Style.Font.Bold = true;
                worksheet.Cells["A1:T1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:T1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:T1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:T1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:T1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
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
        public ActionResult DoExportMaterial_Maintenance_Record(string Material_Maintenance_Record_UIDs)
        {


            //get Export datas
            var apiUrl = string.Format("Equipmentmaintenance/DoExportMaterial_Maintenance_RecordAPI?Material_Maintenance_Record_UIDs={0}", Material_Maintenance_Record_UIDs);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<Material_Maintenance_RecordDTO>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Material_Maintenance_Record");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "状态", "厂区", "OP类型", "功能厂", "料号", "设备备品序号", "是否为保固期", "送修日期", "预计完成日期", "维修厂商", "维修项目", "维修费用", "维修天数", "提交人员", "判定人员", "回厂日期", "回厂日期填写人员", "验收人员", "备注" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("Material_Maintenance_Record");

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
                    worksheet.Cells[index + 2, 2].Value = GetStatus(currentRecord.Status_UID);
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Plant_Organization_Name;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.BG_Organization_Name;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.FunPlant_Organization_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_ID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Seq;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Is_Warranty.Value ? "是" : "否";
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Delivery_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Expected_Return_Date.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Vendor;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Maintenance_Items;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Maintenance_Fees;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Maintenance_Days;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Submiter;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Judger;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Return_Date.Value.ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Returner;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Acceptancer;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Notes;


                }
                // worksheet.Cells.AutoFitColumns();
                for (int i = 1; i <= 20; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
                worksheet.Cells["A1:T1"].Style.Font.Bold = true;
                worksheet.Cells["A1:T1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:T1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells["A1:T1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:T1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:T1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }


        public string GetStatus(int data)
        {
            if (data == 1)
            {
                return "未判定";
            }
            else if (data == 2)
            {
                return "未填写返厂日期";
            }
            else if (data == 3)
            {
                return "未验收";
            }
            else
            {
                return "已结案";
            }

        }
        #endregion
    }
}
