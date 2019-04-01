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

namespace PDMS.Web.Controllers
{
    public class StorageManageController : WebControllerBase
    {
        // GET: StorageManage
        #region   仓库管理
        
        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }
        public ActionResult Warehouse()
        {
           WarehouseVM currentVM = new WarehouseVM();

    

            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;

            apiUrl = string.Format("StorageManage/QueryWarehouseTypesAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var types = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.Types = types;

            apiUrl = string.Format("StorageManage/QueryWarehousesAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var Wars = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result);
            currentVM.Wars = Wars;
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
            return View(currentVM);
        }

        public ActionResult QueryWarehouseStorageInfo(WarehouseDTO search, Page page)
        {
          
            search.Plant_UID= GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryWarehouseStoragesAPI");
            var responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }




        public ActionResult DoAllExportWarehouseReprot(WarehouseDTO search)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/DoAllExportWarehouseReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Warehouse");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //var stringHeads = new string[] { "排序", "Business Group", "功能厂", "仓库代码","仓库类别","仓库名称","英文名称","仓库说明", "料架号", "储位号","储位说明","修改人员", "修改日期"};
            var stringHeads = new string[] { "序号", "厂区", "OP类型", "功能厂", "仓库代码", "仓库类别", "仓库名称", "英文名称", "仓库说明", "修改人员", "修改日期" };

            var stringHeads1 = new string[] { "排序", "仓库代码", "料架号", "储位号", "储位说明", "修改人员", "修改日期" };


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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Warehouse_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Name_ZH;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Name_EN;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Desc;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                    //worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    //worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    //worksheet.Cells[index + 2, 11].Value = currentRecord.WarehouseStorageDesc;
                    //worksheet.Cells[index + 2, 12].Value = currentRecord.Modifier;
                    //worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
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
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Warehouse_ID;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.WarehouseStorageDesc;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Modifier;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
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
            var apiUrl = string.Format("StorageManage/DoExportWarehouseReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("Warehouse");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //var stringHeads = new string[] { "排序", "Business Group", "功能厂", "仓库代码", "仓库类别", "仓库名称", "英文名称", "仓库说明", "料架号", "储位号", "储位说明", "修改人员", "修改日期" };

            var stringHeads = new string[] { "序号","厂区", "OP类型", "功能厂", "仓库代码", "仓库类别", "仓库名称", "英文名称", "仓库说明", "修改人员", "修改日期" };

            var stringHeads1 = new string[] { "序号",  "仓库代码", "料架号", "储位号", "储位说明", "修改人员", "修改日期" };


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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Warehouse_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Name_ZH;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Name_EN;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Desc;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifier;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                    //worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    //worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    //worksheet.Cells[index + 2, 11].Value = currentRecord.WarehouseStorageDesc;
                    //worksheet.Cells[index + 2, 12].Value = currentRecord.Modifier;
                    //worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
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
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Warehouse_ID;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.WarehouseStorageDesc;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Modifier;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet1.Cells.AutoFitColumns();

                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }




        //获取当前用户所有的BGID
        public List<int> CurrentBG_UIDs()
        {
            List<int> CurrentBG_UIDs = new List<int>();           
            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            foreach(var item in optypes)
            {
                CurrentBG_UIDs.Add(item.Organization_UID);
            }
            return CurrentBG_UIDs;
        }
        public ActionResult QueryFunplantByop(int opuid)
        {
            var apiUrl = string.Format("StorageManage/QueryFunplantByopAPI?opuid={0}", opuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditWarehouseInfo(string jsonStorages)
        {
            var apiUrl = string.Format("StorageManage/AddOrEditWarehouseInfoAPI");
            var entity = JsonConvert.DeserializeObject<WarehouseStorages>(jsonStorages);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public string EditWarehouseStorageInfo(WarehouseDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/EditWarehouseStorageInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult QueryWarehouseStByuid(int Warehouse_Storage_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryWarehouseStByuidAPI?Warehouse_Storage_UID={0}", Warehouse_Storage_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryWarehouseByuid (int Warehouse_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryWarehouseByuidAPI?Warehouse_UID={0}", Warehouse_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteWarehouseStorage(int Warehouse_Storage_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteWarehouseStorageAPI?WareStorage_UId={0}", Warehouse_Storage_UID);

            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public string DeleteWarehouse(int Warehouse_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteWarehouseAPI?Warehouse_UID={0}", Warehouse_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        #endregion  仓库管理
        #region 开账

        public ActionResult CreateBound()
        {
            CreateBoundVM currentVM = new CreateBoundVM();

            var apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            //apiUrl = string.Format("StorageManage/QueryAllWarehouseStAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var warst = JsonConvert.DeserializeObject<List<WarehouseStorageDTO>>(result);
            //currentVM.warst = warst;


            //根据厂区过滤料号
            var plantids = new List<int>();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {

                plantids.Add(CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value);
            }
            else
            {
                //获取
                apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {
                    plantids.Add(item.Organization_UID);
                }
            }

            apiUrl = string.Format("Equipmentmaintenance/QueryAllMaterialAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var mats = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result);
            currentVM.mats = mats.Where(o => plantids.Contains(o.Organization_UID.Value)).ToList();
            //currentVM.mats = mats;

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

        public ActionResult QueryCreateBounds(StorageInboundDTO search, Page page)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryCreateBoundsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
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
                //获取对应的厂区ID
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

                    //读取Excel并进行验证判断
                    var MatAPI = "Equipmentmaintenance/QueryAllMaterialAPI";
                    HttpResponseMessage matmessage = APIHelper.APIGetAsync(MatAPI);
                    var jsonMats = matmessage.Content.ReadAsStringAsync().Result;
                    var mats = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(jsonMats);

                    var warehouseStAPI = "StorageManage/QueryAllWarehouseStAPI";
                    HttpResponseMessage WarhouseStmessage = APIHelper.APIGetAsync(warehouseStAPI);
                    var jsonwarst = WarhouseStmessage.Content.ReadAsStringAsync().Result;
                    var warehousest = JsonConvert.DeserializeObject<List<WarehouseStorageDTO>>(jsonwarst);

                    var enumAPI = "StorageManage/QueryStorageEnumsAPI";
                    HttpResponseMessage StorageEnumStmessage = APIHelper.APIGetAsync(enumAPI);
                    var jsonenums = StorageEnumStmessage.Content.ReadAsStringAsync().Result;
                    var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(jsonenums);

                    var StorageInboundAPI= "StorageManage/QueryStorageInboundAPI";
                    HttpResponseMessage StorageInboundStmessage = APIHelper.APIGetAsync(StorageInboundAPI);
                    var jsonstoinbound= StorageInboundStmessage.Content.ReadAsStringAsync().Result;
                    var stoinbound = JsonConvert.DeserializeObject<List<StorageInboundDTO>>(jsonstoinbound);

                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);


                    string PreInboundID = "Opening" + DateTime.Today.ToString("yyyyMMdd");
                    var test = stoinbound.Where(m => m.Storage_Inbound_ID.StartsWith(PreInboundID)).ToList();
                    List <StorageInboundDTO> storageinboundlist = new List<StorageInboundDTO>();
                    for (int i = 2; i <= totalRows; i++)
                    {

                        string Plant_name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                        int Plant_Id = GetPlantOrgUid();
                        if (string.IsNullOrWhiteSpace(Plant_name))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行厂区没有值", i);
                            return errorInfo;
                        }
                        Plant_name = Plant_name.Trim();
                        if (Plant_Id == 0)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行没有权限导入,请设置厂区", i);
                            return errorInfo;
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


                        int inboundtypeuid = 0;
                        string inboundtype = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "类别")].Value);
                        if (string.IsNullOrWhiteSpace(inboundtype) || inboundtype=="不良品")
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行类别错误", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasinboundtype = enums.FirstOrDefault(m => m.Enum_Value == inboundtype); 
                            if (hasinboundtype == null)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行类别错误", i);
                                return errorInfo;
                            }
                            else
                            {
                                inboundtypeuid = hasinboundtype.Enum_UID;
                            }
                        }

                        string PostInboundID = (test.Count() + i - 1).ToString("0000");
                        StorageInboundDTO storageinbounditem = new StorageInboundDTO();
                        int matuid = 0;
                        string materialid = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                        if (string.IsNullOrWhiteSpace(materialid))
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料号错误", i);
                            return errorInfo;
                        }
                        else
                        {
                            //根据厂区过滤料号
                            var hasmat = mats.FirstOrDefault(m => m.Material_Id == materialid&&plantids.Contains(m.Organization_UID.Value));
                            if (hasmat == null)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行料号不存在", i);
                                return errorInfo;
                            }
                            else
                            {
                                matuid = hasmat.Material_Uid;
                            }
                        }
                        var hasinboundmat = stoinbound.FirstOrDefault(m => m.Material_Uid == matuid);
                        if (hasinboundmat != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料号已开过账", i);
                            return errorInfo;
                        }
                        var hasexcelmat = storageinboundlist.FirstOrDefault(m => m.Material_Uid == matuid);
                        if (hasexcelmat != null)
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行料号重复", i);
                            return errorInfo;
                        }
                        decimal decqty = 0;
                        string qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "库存数量")].Value);
                        try
                        {
                            decqty = Convert.ToDecimal(qty);
                        }
                        catch
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行库存数量值错误", i);
                            return errorInfo;
                        }
                        var warid = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库号")].Value);
                        var rackid = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料架号")].Value);
                        var storageid = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "储位号")].Value);
                        int storageuid = 0;
                        //开账时不能选择
                        var haswarhousest = warehousest.FirstOrDefault(m => m.Warehouse_ID == warid & m.Rack_ID == rackid & m.Storage_ID==storageid&m.BG_Organization_UID==BG_Organization_UID&m.FunPlant_Organization_UID==FunPlant_Organization_UID);
                        if (haswarhousest != null)
                        {
                            //fky2017/11/13
                            // if (haswarhousest.Warehouse_Type_UID==398)
                            if (haswarhousest.Warehouse_Type_UID == 418)
                            {
                                excelIsError = true;
                                errorInfo = string.Format("第{0}行类别与仓库类型不符", i);
                                return errorInfo;
                            }
                            else
                            {
                                storageuid = haswarhousest.Warehouse_Storage_UID;
                            }
                        }
                        else
                        {
                            excelIsError = true;
                            errorInfo = string.Format("第{0}行储位信息不存在", i);
                            return errorInfo;
                        }
                        var Desc  = "期初開帳";
                        storageinbounditem.Modified_UID = CurrentUser.AccountUId;
                        storageinbounditem.Modified_Date = DateTime.Now;
                        storageinbounditem.Storage_Inbound_Type_UID = 406;
                        storageinbounditem.Storage_Inbound_ID = PreInboundID + PostInboundID;
                        storageinbounditem.Material_Uid = matuid;
                        storageinbounditem.Warehouse_Storage_UID = storageuid;
                        storageinbounditem.Inbound_Price = 0;
                        storageinbounditem.PartType_UID = inboundtypeuid;
                        storageinbounditem.PU_NO = "None";
                        storageinbounditem.PU_Qty = 0;
                        storageinbounditem.Issue_NO = "None";
                        storageinbounditem.Inbound_Qty = decqty;
                        storageinbounditem.NG_Qty = 0;
                        //fky2017/11/13
                       //  storageinbounditem.Status_UID = 374;
                        storageinbounditem.Status_UID = 407;
                        storageinbounditem.Desc = Desc;
                        storageinboundlist.Add(storageinbounditem);

                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(storageinboundlist);
                    var apiInsertEquipmentUrl = string.Format("StorageManage/InsertCreateBoundAPI");
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

        private string[] GetCreateBoundHeadColumn()
        {
            var propertiesHead = new[]
            {
                  "厂区", "OP类型", "功能厂", "类别","料号","库存数量","仓库号","料架号","储位号"
            };
            return propertiesHead;
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
                var colorRange = string.Format("A1:I{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("批量上传开账信息模板说明");
                SetCreateBoundExcelStyle(worksheetsm, propertiesHead);

                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:I{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
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
            //设置列宽
            for (int i = 1; i <= 9; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:I1"].Style.Font.Bold = true;
            worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        private void SetCreateBoundExcelStylesm(ExcelWorksheet worksheet)
        {
            //填充Title内容   "类别","料号","库存数量","仓库号","料架号","储位号"
            worksheet.Cells[2, 1].Value = "CTU_M(厂区)";
            worksheet.Cells[2, 2].Value = "OP2(OP类型)";
            worksheet.Cells[2, 3].Value = "CNC(功能厂)";
            worksheet.Cells[2, 4].Value = "正常料良品/非正常料良品(类别-小于500个字符)";
            worksheet.Cells[2, 5].Value = "l001(料号-小于20字符)";
            worksheet.Cells[2, 6].Value = "500(库存数量-为整数)";
            worksheet.Cells[2, 7].Value = "ck001(仓库号-小于20字符)";
            worksheet.Cells[2, 8].Value = "lj001(料架号-小于20字符)";
            worksheet.Cells[2, 9].Value = "cw001(储位号-小于20字符)";
      
        }
        public ActionResult AddOrEditCreateBound(StorageInboundDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Applicant_UID= CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddOrEditCreateBoundAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ApproveCreateboundByUid(int Storage_Inbound_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveCreateboundByUidAPI?Storage_Inbound_UID={0}&Useruid={1}", Storage_Inbound_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string DeleteCreateBound(int Storage_Inbound_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteCreateBoundAPI?Storage_Inbound_UID={0}&userid={1}", Storage_Inbound_UID,CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public string ApproveCreatebound(StorageInboundDTO search)
        {   
            search.Plant_UID = GetPlantOrgUid();
            search.Approver_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/ApproveCreateboundAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
           //  var result = responMessage.Content.ReadAsStringAsync().Result;
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public ActionResult DoAllExportCreateBoundReprot(StorageInboundDTO search)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/DoAllExportCreateBoundReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageInboundDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CreateBound");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "状态", "类型", "功能厂","料号","品名","型号","归类","说明","仓库代码","仓库名称","料架号",
                "储位号","库存数量", "开账时间"};

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.PartType;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Funplant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Classification;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Desc;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Name_ZH;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Inbound_Qty;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
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
            var apiUrl = string.Format("StorageManage/DoExportCreateBoundReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageInboundDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CreateBound");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "状态", "类型", "功能厂","料号","品名","型号","归类","说明","仓库代码","仓库名称","料架号",
                "储位号","库存数量", "开账时间"};
         
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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.PartType;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Funplant;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Classification;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Desc;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Name_ZH;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Inbound_Qty;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }



        #endregion 开账 
        #region 出入库
        public ActionResult UpdateBound()
        {


            UpdateBoundVM currentVM = new UpdateBoundVM();
            var apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            //根据厂区过滤料号
            var plantids = new List<int>();
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {

                plantids.Add(CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value);
            }
            else
            {
                //获取
                 apiUrl = string.Format("Settings/GetPlantsAPI?PlantOrgUid={0}", 0);
                 responMessage = APIHelper.APIGetAsync(apiUrl);
                result = responMessage.Content.ReadAsStringAsync().Result;
                var systemOrgDTOs = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
                foreach (var item in systemOrgDTOs)
                {
                    plantids.Add(item.Organization_UID);
                }
            }

            //apiUrl = string.Format("Equipmentmaintenance/QueryAllMaterialAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var mats = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result);
            //currentVM.mats = mats.Where(o=>plantids.Contains(o.Organization_UID.Value)).ToList();

            //apiUrl = string.Format("StorageManage/QueryWarehouseStAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var Wars = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result);
            //currentVM.warsts = Wars;

            //apiUrl = string.Format("StorageManage/QueryUsersAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var users = JsonConvert.DeserializeObject<List<EQPUserTableDTO>>(result);
            //currentVM.users = users;

            apiUrl = string.Format("StorageManage/QueryWarOpAPI?PlantID={0}", GetPlantOrgUid());
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;

            return View(currentVM);
        }

        public ActionResult QueryWarStroge(int OPTypeID)
        {

            //apiUrl = string.Format("StorageManage/QueryWarehouseStAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var Wars = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result);
            //currentVM.warsts = Wars;
            var apiUrl = string.Format("StorageManage/QueryWarStrogeAPI?OPTypeID={0}", OPTypeID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        
        public ActionResult QueryAllMaterial()
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryAllOPMaterialAPI?PlantID={0}", GetPlantOrgUid());
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");  
                
        }

        public ActionResult QueryMaterialByKey(string key)
        {
            var apiUrl = string.Format("Equipmentmaintenance/QueryOPMaterialByKeyAPI?PlantID={0}&key={1}", GetPlantOrgUid(),key);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }

        public ActionResult QueryUsers()
        {
            var apiUrl = string.Format("StorageManage/QueryUsersAPI?PlantID={0}", GetPlantOrgUid());
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        public ActionResult QueryBoundDetails(InOutBoundInfoDTO search, Page page)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryBoundDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditInboundApply(StorageInboundDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddOrEditInboundApplyAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryInboundByUid(int Storage_Bound_UID, string Inout_Type)
        {
            var apiUrl = string.Format("StorageManage/QueryInboundByUidAPI?Storage_Bound_UID={0}&Inout_Type={1}", Storage_Bound_UID, Inout_Type);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ApproveInboundByUid(int Storage_Inbound_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveInboundByUidAPI?Storage_Inbound_UID={0}&Useruid={1}", Storage_Inbound_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public ActionResult QueryWarSt(int inboundtype)
        {
            var apiUrl = string.Format("StorageManage/QueryWarStAPI?inboundtype={0}&plantuid={1}", inboundtype, GetPlantOrgUid());
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryWarStByKey(int inboundtype,int warStUid, string key)
        {
            var apiUrl = string.Format("StorageManage/QueryWarStByKeyAPI?inboundtype={0}&warStUid={1}&key={2}&plantuid={3}", inboundtype, warStUid, key, GetPlantOrgUid());
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryFunplantByUser(int userid)
        {
            var apiUrl = string.Format("StorageManage/QueryFunplantByUserAPI?userid={0}", userid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryMatinventory(int Material_Uid,int Warehouse_Storage_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryMatinventoryAPI?Material_Uid={0}&Warehouse_Storage_UID={1}", Material_Uid, Warehouse_Storage_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryMatinventoryByMat(int Material_Uid)
        {
            var apiUrl = string.Format("StorageManage/QueryMatinventoryByMatAPI?Material_Uid={0}", Material_Uid);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryEqprepair(string Repair_id)
        {
            var apiUrl = string.Format("StorageManage/QueryEqprepairAPI?Repair_id={0}", Repair_id);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditOutbound(string jsonStorages)
        {
            var apiUrl = string.Format("StorageManage/AddOrEditOutboundAPI");
            var entity = JsonConvert.DeserializeObject<OutBoundInfo>(jsonStorages);
            entity.Modified_UID = this.CurrentUser.AccountUId;

            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public string DeleteBound(int Storage_Bound_UID, string Inout_Type)
        {
            var apiUrl = string.Format("StorageManage/DeleteBoundAPI?Storage_Bound_UID={0}&Inout_Type={1}&UserUid={2}",
                Storage_Bound_UID, Inout_Type,CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string ApproveOutboundByUid(int Storage_Outbound_M_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveOutboundByUidAPI?Storage_Outbound_M_UID={0}&Useruid={1}", Storage_Outbound_M_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public ActionResult QueryPuqty(string PUNO,int Storage_Inbound_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryPuqtyAPI?PUNO={0}&Storage_Inbound_UID={1}", PUNO, Storage_Inbound_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;

            return Content(result, "application/json");
        }

        public ActionResult QueryWarFunplantByop(int opuid,int PartType_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryWarFunplantByopAPI?opuid={0}&PartType_UID={1}", opuid, PartType_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryWarIdByFunction(int Functionuid,int PartType_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryWarIdByFunctionAPI?Functionuid={0}&PartType_UID={1}", Functionuid, PartType_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult SaveWarehouseSt(WarehouseStorageDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/SaveWarehouseStAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryMatByOutType(int Out_Type_Uid)
        {
            var apiUrl = string.Format("StorageManage/QueryMatByOutTypeAPI?Out_Type_Uid={0}&FunplantUid={1}", Out_Type_Uid, GetPlantOrgUid());
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryWarStByMaterial(int Material_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryWarStByMaterialAPI?Material_UID={0}", Material_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetWarStByMatCheck(int Material_UID)
        {
            var apiUrl = string.Format("StorageManage/GetWarStByMatCheckAPI?Material_UID={0}", Material_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
   
        public ActionResult DoAllExportUpdateBoundReprot(InOutBoundInfoDTO search)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/DoAllExportUpdateBoundReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<InOutBoundInfoDTO>>(result).ToList();
          
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("UpdateBound");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "出入库单类别", "出入库单编号", "状态","料号","品名","型号","仓库代码","料架号",
                "储位号","出入库数量","领用人", "申请者","申请时间","审核者","审核时间"};
            var stringOutHeads = new string[] {"排序",  "出入库单编号", "料号","品名","型号","仓库代码","料架号",
                "储位号","材料类别", "待审数量","申请数量","库存数量"};
            List<OutBoundInfoDTO> OutBoundInfoDTOs = new List<OutBoundInfoDTO>();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("UpdateBound");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }
                var worksheet1 = excelPackage.Workbook.Worksheets.Add("OutBound");

                //set Title
                for (int colIndex = 0; colIndex < stringOutHeads.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringOutHeads[colIndex];
                }
                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.In_Out_Type;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Storage_Bound_ID;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Bound_Qty;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Accepter;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.ModifiedUser;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 15].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);
                  if(currentRecord.OutBoundInfos!=null&&currentRecord.OutBoundInfos.Count>0)
                    {
                        foreach(var item in currentRecord.OutBoundInfos)
                        {
                            item.Storage_Bound_ID = currentRecord.Storage_Bound_ID;
                            OutBoundInfoDTOs.Add(item);
                        }
                      //  OutBoundInfoDTOs.AddRange(currentRecord.OutBoundInfos);
                    }
                }
                //set cell value
                for (int index = 0; index < OutBoundInfoDTOs.Count; index++)
                {
                    var currentRecord = OutBoundInfoDTOs[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Storage_Bound_ID;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Material_Id;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Material_Name;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Material_Types;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Warehouse_ID;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.PartType;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Be_Out_Qty;
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.Outbound_Qty;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.Inventory_Qty;

                }
                worksheet.Cells.AutoFitColumns();        
                worksheet1.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        public ActionResult DoExportUpdateBoundReprot(string InOutBounds)
        {
            //var listBounds = JsonConvert.DeserializeObject<List<InOutBoundVM>>(InOutBounds).ToList();
            //get Export datas
            var apiUrl = string.Format("StorageManage/DoExportUpdateBoundReprotAPI?uids={0}", InOutBounds);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<InOutBoundInfoDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("UpdateBound");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "出入库单类别", "出入库单编号", "状态","料号","品名","型号","仓库代码","料架号",
                "储位号","出入库数量","领用人", "申请者","申请时间","审核者","审核时间"};
            var stringOutHeads = new string[] {"排序",  "出入库单编号", "料号","品名","型号","仓库代码","料架号",
                "储位号","材料类别", "待审数量","申请数量","库存数量"};
            List<OutBoundInfoDTO> OutBoundInfoDTOs = new List<OutBoundInfoDTO>();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("UpdateBound");
                var worksheet1 = excelPackage.Workbook.Worksheets.Add("OutBound");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }
                //set Title
                for (int colIndex = 0; colIndex < stringOutHeads.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringOutHeads[colIndex];
                }
                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    //seq
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.In_Out_Type;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Storage_Bound_ID;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Bound_Qty;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Accepter;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.ModifiedUser;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 15].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);
                    if (currentRecord.OutBoundInfos != null && currentRecord.OutBoundInfos.Count > 0)
                    {
                        foreach (var item in currentRecord.OutBoundInfos)
                        {
                            item.Storage_Bound_ID = currentRecord.Storage_Bound_ID;
                            OutBoundInfoDTOs.Add(item);
                        }
                        //  OutBoundInfoDTOs.AddRange(currentRecord.OutBoundInfos);
                    }
           
                }



                //set cell value
                for (int index = 0; index < OutBoundInfoDTOs.Count; index++)
                {
                    var currentRecord = OutBoundInfoDTOs[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.Storage_Bound_ID;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Material_Id;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Material_Name;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Material_Types;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Warehouse_ID;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.PartType;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Be_Out_Qty;
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.Outbound_Qty;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.Inventory_Qty;
                }
                worksheet1.Cells.AutoFitColumns();
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public string ImportCoupaPOExcel(HttpPostedFileBase upload_excel)
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
                Dictionary<int, StorageInboundDTO> rows_contents = new Dictionary<int, StorageInboundDTO>();
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
                        StorageInboundDTO dto = new StorageInboundDTO();
                        string row_content_error = string.Empty;
                        for (int iCol = 1; iCol <= totalColumns; iCol++)
                        {
                            switch (iCol)
                            {
                                case 1:
                                    string v1 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v1))
                                        row_content_error = "采购单号为空";
                                    dto.CoupaPO_ID = v1.Trim();
                                    dto.Desc = "该入库单由采购单" + v1.Trim() + "号"+ DateTime.Now.ToString("YYYY/MM/DD") + "汇入";
                                    break;
                                case 2:
                                    string v2 = worksheet.GetValue<string>(iRow, iCol);
                                    if (string.IsNullOrEmpty(v2))
                                        row_content_error = "PIS料号为空";
                                    dto.Material_Id = v2;
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    decimal v4 = worksheet.GetValue<decimal>(iRow, iCol);
                                    if (v4 < 1)
                                        row_content_error = "数量不可小於1";
                                    dto.PU_Qty = v4;
                                    dto.OK_Qty = v4;
                                    break;
                                case 5:
                                    break;
                                case 6:
                                    break;
                                case 7:                                
                                    break;
                                case 8:
                                    decimal v8 = worksheet.GetValue<decimal>(iRow, iCol);
                                    if (v8 < 1)
                                        row_content_error = "单价不可小於1";
                                    dto.Current_POPrice = v8;
                                    break;
                                case 9:
                                    break;
                                case 10:
                                    break;
                            }
                        }
                        if (rows_contents.Values.Select(x => x.CoupaPO_ID).Contains(dto.CoupaPO_ID))
                            row_content_error = "采购单号重覆上传";
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
                        item.Value.Applicant_UID = CurrentUser.AccountUId;
                        item.Value.Applicant_Date = DateTime.Now;
                        //item.Value.Modified_UID = CurrentUser.AccountUId;
                        //item.Value.Modified_Date = DateTime.Now;
                    }
                    var jsonString = JsonConvert.SerializeObject(rows_contents, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                    });
                    var apiUrl = "StorageManage/ImportCoupaPO_To_InBoundApplyAPI";
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
        #endregion 出入库    
        #region  盘点
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
                            "类别",
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
                        //获取所有类型数据。
                        var apiStorageEnumsUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
                        var responStorageEnumsMessage = APIHelper.APIGetAsync(apiStorageEnumsUrl);
                        var storageEnumsresult = responStorageEnumsMessage.Content.ReadAsStringAsync().Result;
                        List<EnumerationDTO> enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(storageEnumsresult);

                        //获取厂区下所有料号
                        var apiUrl = string.Format("StorageManage/QueryMatByOutTypeAPI?Out_Type_Uid={0}&FunplantUid={1}", 0, GetPlantOrgUid());
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var materialInventorysresult = responMessage.Content.ReadAsStringAsync().Result;
                        List<MaterialInventoryDTO> materialInventorys= JsonConvert.DeserializeObject<List<MaterialInventoryDTO>>(materialInventorysresult);
                        //获取所有储位
                        var apiMaterialUrl = string.Format("StorageManage/GetWarStByMatCheckAPI?Material_UID={0}", 0);
                        HttpResponseMessage materialresponMessage = APIHelper.APIGetAsync(apiMaterialUrl);
                        var warehouse_storageresult = materialresponMessage.Content.ReadAsStringAsync().Result;
                        List<MaterialInventoryDTO> warehouse_storages = JsonConvert.DeserializeObject<List<MaterialInventoryDTO>>(warehouse_storageresult);
                        
                        //获得所有ORGBOMLIST
                        var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                        HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                        var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                        var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                        var storageCheckDTOList = new List<StorageCheckDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {

                            var storageCheckDTO = new StorageCheckDTO();
                            string Plant_name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                            int Plant_Id = GetPlantOrgUid();
                            if (string.IsNullOrWhiteSpace(Plant_name))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区没有值", i);
                                return errorInfo;
                            }
                            Plant_name = Plant_name.Trim();
                            if (Plant_Id == 0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有权限导入,请设置厂区", i);
                                return errorInfo;
                            }

                            string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                            string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                            int BG_Organization_UID = 0;
                            if (string.IsNullOrWhiteSpace(BG_Organization))
                            {
                                isExcelError = true;
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
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行OP的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            int FunPlant_Organization_UID = 0;
                            if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                            {
                                isExcelError = true;
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
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            string enumvalue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "类别")].Value);
                            if (string.IsNullOrWhiteSpace(enumvalue))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行类别没有值", i);
                                return errorInfo;
                            }
                            else if (enumvalue.Length > 500)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行类别长度超过最大限定[500]", i);
                                return errorInfo;
                            }

                            string material_Id = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                            if (string.IsNullOrWhiteSpace(material_Id))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号没有值", i);
                                return errorInfo;
                            }
                            else if (material_Id.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string material_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                            if (string.IsNullOrWhiteSpace(material_Name))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行品名没有值", i);
                                return errorInfo;
                            }
                            else if (material_Name.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行品名长度超过最大限定[50]", i);
                                return errorInfo;
                            }

                            string material_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "型号")].Value);
                            if (string.IsNullOrWhiteSpace(material_Types))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行型号没有值", i);
                                return errorInfo;
                            }
                            else if (material_Types.Length > 150)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行型号长度超过最大限定[150]", i);
                                return errorInfo;
                            }

                            string warehouse_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "仓库")].Value);
                            if (string.IsNullOrWhiteSpace(warehouse_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行仓库没有值", i);
                                return errorInfo;
                            }
                            else if (warehouse_ID.Length > 10)
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
                            int PartType_UID = 0;
                            if (enums.Where(e => e.Enum_Value == enumvalue)!=null&& enums.Where(e => e.Enum_Value == enumvalue).Count()>0)
                            {
                                 PartType_UID = enums.Where(e => e.Enum_Value == enumvalue).FirstOrDefault().Enum_UID;
                            }

                            if(PartType_UID==0)
                             {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有找到对应的类型", i);
                                return errorInfo;
                             }

                            //获取料号
                            int Material_Uid = 0;                         
                            if (PartType_UID == 417 || PartType_UID == 433)  //不良品时只能选择MRB仓
                            {

                             var nmaterialInventorys=   materialInventorys.Where(e => e.Warehouse_Type_UID == 418&& e.Material_Id== material_Id&&e.Material_Name==material_Name&&e.Material_Types== material_Types);
                           
                                if(nmaterialInventorys!=null&&nmaterialInventorys.Count()>0)
                                {
                                    Material_Uid = materialInventorys.Where(e => e.Warehouse_Type_UID == 418 && e.Material_Id == material_Id && e.Material_Name == material_Name && e.Material_Types == material_Types).FirstOrDefault().Material_Uid;
                                }
                            }
                            else
                            {
                                var nmaterialInventorys = materialInventorys.Where(e => e.Material_Id == material_Id && e.Material_Name == material_Name && e.Material_Types == material_Types);

                                if (nmaterialInventorys != null && nmaterialInventorys.Count() > 0)
                                {
                                    Material_Uid = materialInventorys.Where(e => e.Material_Id == material_Id && e.Material_Name == material_Name && e.Material_Types == material_Types).FirstOrDefault().Material_Uid;
                                }
                            }
                            if(Material_Uid==0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没根据料号，品名，类型 有找到对应的料号", i);
                                return errorInfo;
                            }

                            int Warehouse_Storage_UID = 0;
                                                       
                            var nwarehouse_storages = warehouse_storages.Where(o => o.Material_Uid == Material_Uid && o.Warehouse_ID == warehouse_ID && o.Rack_ID == rack_ID && o.Storage_ID == storage_ID && o.BG_Organization_UID==BG_Organization_UID && o.FunPlant_Organization_UID == FunPlant_Organization_UID);
                            if(nwarehouse_storages!=null&& nwarehouse_storages.Count()>0)
                            {
                                Warehouse_Storage_UID = nwarehouse_storages.FirstOrDefault().Warehouse_Storage_UID;
                            }else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没找到对应的储位。", i);
                                return errorInfo;
                            }
                            ////导入数据判重
                            var isSelfRepeated = storageCheckDTOList.Exists(m => m.PartType_UID == PartType_UID && m.Material_Uid == Material_Uid && m.Warehouse_Storage_UID == Warehouse_Storage_UID && m.Check_Qty == check_Qty);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行在导入数据中重复,不可重复导入", i);
                                return errorInfo;
                            }

                            storageCheckDTO.PartType_UID = PartType_UID;
                            storageCheckDTO.Material_Uid = Material_Uid;
                            storageCheckDTO.Material_Id = material_Id;
                            storageCheckDTO.Material_Name = material_Name;
                            storageCheckDTO.Material_Types = material_Types;
                            storageCheckDTO.Warehouse_Storage_UID =Warehouse_Storage_UID;
                            storageCheckDTO.Warehouse_ID = warehouse_ID;
                            storageCheckDTO.Rack_ID = rack_ID;
                            storageCheckDTO.Storage_ID = storage_ID;
                            storageCheckDTO.Check_Qty = check_Qty;
                            storageCheckDTO.Storage_InOut_UID = 0;
                            storageCheckDTO.Approver_Date= DateTime.Now;
                            storageCheckDTO.Approver_UID = CurrentUser.AccountUId;
                            storageCheckDTOList.Add(storageCheckDTO);
                            
                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(storageCheckDTOList);
                        var apiInsertStorageCheckDUrl = string.Format("StorageManage/ImportStorageCheckAPI");
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
        public FileResult DownloadStorageCheckExcel(int PartType_UID,string Material_Id,string Material_Name, string Material_Types, string Warehouse_ID,string Rack_ID,string Storage_ID,int Plant_UID,int BG_Organization_UID,int FunPlant_Organization_UID)
        {
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("批量上传盘点维护作业模板");
            string[] propertiesHead = new string[] { };
            propertiesHead = GetStorageCheckHeadColumn();
          //  StorageCheckDTO
            using (var excelPackage = new ExcelPackage(stream))
            {
                //get Export datas
                var apiUrl = string.Format(@"StorageManage/DownloadStorageCheckAPI?PartType_UID={0}&Material_Id={1}&Material_Name={2}&Material_Types={3}&Warehouse_ID={4}&Rack_ID={5}&Storage_ID={6}&Plant_UID={7}&BG_Organization_UID={8}&FunPlant_Organization_UID={9}", PartType_UID,  Material_Id,  Material_Name,  Material_Types,  Warehouse_ID,  Rack_ID,  Storage_ID, Plant_UID,  BG_Organization_UID,  FunPlant_Organization_UID);
                var responMessage = APIHelper.APIGetAsync(apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<StorageCheckDTO>>(result).ToList();

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
                    worksheet.Cells[index + 2, 4].Value = currentRecord.PartType;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                                           
                }
                int maxRow = 1;
                //设置灰色背景
                var colorRange = string.Format("A1:K{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:K{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:K{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:K{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("批量上传盘点维护作业模板说明");
                SetStorageCheckExcelStyle(worksheetsm, propertiesHead);

                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:K{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:K{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:K{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
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
            worksheet.Cells[1, 11].Value = propertiesHead[10];
            //设置列宽
            for (int i = 1; i <= 11; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:K1"].Style.Font.Bold = true;
            worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        private void SetStorageCheckExcelStylesm(ExcelWorksheet worksheet)
        {
            worksheet.Cells[2, 1].Value = "CTU_M(厂区)";
            worksheet.Cells[2, 2].Value = "OP2(OP类型)";
            worksheet.Cells[2, 3].Value = "CNC(功能厂)";
            worksheet.Cells[2, 4].Value = "正常料品(类别-小于500个字符)";
            worksheet.Cells[2, 5].Value = "l001(料号-小于20字符)";
            worksheet.Cells[2, 6].Value = "500(品名-小于20字符)";
            worksheet.Cells[2, 7].Value = "500(型号-小于20字符)";
            worksheet.Cells[2, 8].Value = "G001(仓库-小于500个字符)";
            worksheet.Cells[2, 9].Value = "l001(料架号-小于20字符)";
            worksheet.Cells[2, 10].Value = "500(储位-小于20字符)";
            worksheet.Cells[2, 11].Value = "500(盘点数量-为整数)";
        }
        private string[] GetStorageCheckHeadColumn()
        {
            var propertiesHead = new[]
            {
               "厂区","OP类型","功能厂", "类别","料号","品名","型号","仓库","料架号","储位","盘点数量"
            };
            return propertiesHead;
        }

        public ActionResult StorageCheck()
        {
            StorageCheckVM currentVM = new StorageCheckVM();
            var apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;
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

        public ActionResult QueryStorageChecks(StorageCheckDTO search, Page page)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryStorageChecksAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult DoAllExportStorageCheckReprot(StorageCheckDTO search)
        {


            search.Plant_UID = GetPlantOrgUid();

            var apiUrl = string.Format("StorageManage/DoAllExportStorageCheckReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageCheckDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageCheck");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "盘点单编号", "状态", "料号","品名","型号","仓库","料架号","储位","盘点数量","类别",
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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Storage_Check_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Check_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.PartType;        
                    worksheet.Cells[index + 2, 12].Value = currentRecord.ApplicantUser;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);


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
            var apiUrl = string.Format("StorageManage/DoExportStorageCheckReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageCheckDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageCheck");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "盘点单编号", "状态", "料号","品名","型号","仓库","料架号","储位","盘点数量","类别",
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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Storage_Check_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Check_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.PartType;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.ApplicantUser;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult AddOrEditStorageCheck(StorageCheckDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddOrEditStorageCheckAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryStorageCheckByUid(int Storage_Check_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryStorageCheckByUidAPI?Storage_Check_UID={0}", Storage_Check_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ApproveStorageCheckByUid(int Storage_Check_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveStCheckAPI?Storage_Check_UID={0}&Useruid={1}", Storage_Check_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string ApproveStorageCheck(StorageCheckDTO search)
        {
            search.Plant_UID = GetPlantOrgUid();
            search.Approver_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/ApproveStorageCheckAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);         
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public string DeleteStorageCheck(int Storage_Check_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteStorageCheckAPI?Storage_Check_UID={0}&userid={1}", Storage_Check_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        #endregion 盘点    
        #region 库存明细查询

        public ActionResult StorageDetail()
        {
            StorageDetailVM currentVM = new StorageDetailVM();
            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.BGOrgs = optypes;

            //apiUrl = string.Format("StorageManage/QueryWarehouseStAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var Wars = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result);
            //currentVM.Warsts = Wars;        
            currentVM.Warsts = null;

            apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            return View(currentVM);
        }

        public ActionResult DoAllExportStorageDetailReprot(StorageSearchMod search)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/DoAllExportStorageDetailReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageDetailDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageDetails");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "Business Group", "功能厂", "出入库表单编号", "出入库单类别", "料号", "品名", "型号", "仓库号", "料架号", "储位", "出入库时间", "出入库数量", "料号结存数量", "类别", "更新者", "更新时间" };
            
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("StorageDetails");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.BG_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Funplant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Storage_Bound_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Inout_Type;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.InOut_Date == null ? "" : ((DateTime)currentRecord.InOut_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 13].Value = currentRecord.InOut_QTY;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Balance_Qty;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Bound_Type;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.ModifiedUser;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportStorageDetailReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("StorageManage/DoExportStorageDetailReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageDetailDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageDetails");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "Business Group", "功能厂", "出入库表单编号", "出入库单类别", "料号", "品名", "型号", "仓库号", "料架号", "储位", "出入库时间", "出入库数量", "料号结存数量", "类别", "更新者", "更新时间" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("StorageDetails");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.BG_Name;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Funplant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Storage_Bound_ID;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Inout_Type;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Material_Types;                
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Warehouse_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Rack_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Storage_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.InOut_Date == null ? "" : ((DateTime)currentRecord.InOut_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 13].Value = currentRecord.InOut_QTY;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Balance_Qty;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Bound_Type;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.ModifiedUser;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }



        public ActionResult QueryStorageDetails(StorageSearchMod search, Page page)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryStorageDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion 库存明细查询
        #region 储位移转
        private string[] GetStorageTransferHeadColumn()
        {
            var propertiesHead = new[]
            {
               "厂区", "类别","料号","品名","型号","转出OP类型","转出功能厂","转出仓库","转出料架号","转出储位","转入OP类型","转入功能厂","转入仓库","转入料架号","转入储位","转移数量"
            };
            return propertiesHead;
        }

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
                var colorRange = string.Format("A1:P{0}", maxRow);
                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheet.Cells[string.Format("A1:P{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:P{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[string.Format("A1:P{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var worksheetsm = excelPackage.Workbook.Worksheets.Add("批量上传储位移转作业模板说明");
                SetStorageTransferStyle(worksheetsm, propertiesHead);

                worksheetsm.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheetsm.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                //设置边框
                worksheetsm.Cells[string.Format("A1:P{0}", maxRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:P{0}", maxRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheetsm.Cells[string.Format("A1:P{0}", maxRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                SetStorageTransferStylesm(worksheetsm);

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
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
            worksheet.Cells[1, 14].Value = propertiesHead[13];
            worksheet.Cells[1, 15].Value = propertiesHead[14];
            worksheet.Cells[1, 16].Value = propertiesHead[15];
            //设置列宽
            for (int i = 1; i <=16; i++)
            {
                worksheet.Column(i).Width = 17;
            }
            worksheet.Cells["A1:P1"].Style.Font.Bold = true;
            worksheet.Cells["A1:P1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:P1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
        }
        private void SetStorageTransferStylesm(ExcelWorksheet worksheet)
        {
            worksheet.Cells[2, 1].Value = "CTU_M(厂区)";
            worksheet.Cells[2, 2].Value = "正常料品(类别-小于500个字符)";
            worksheet.Cells[2, 3].Value = "l001(料号-小于20字符)";
            worksheet.Cells[2, 4].Value = "500(品名-小于20字符)";
            worksheet.Cells[2, 5].Value = "500(型号-小于20字符)";
            worksheet.Cells[2, 6].Value = "OP2(转出OP类型)";
            worksheet.Cells[2, 7].Value = "CNC(转出功能厂)";
            worksheet.Cells[2, 8].Value = "G001(转出仓库-小于500个字符)";
            worksheet.Cells[2, 9].Value = "l001(转出料架号-小于20字符)";
            worksheet.Cells[2, 10].Value = "500(转出储位-小于20字符)";
            worksheet.Cells[2, 11].Value = "OP2(转入OP类型)";
            worksheet.Cells[2, 12].Value = "CNC(转入功能厂)";
            worksheet.Cells[2, 13].Value = "G001(转入仓库-小于500个字符)";
            worksheet.Cells[2, 14].Value = "l001(转入料架号-小于20字符)";
            worksheet.Cells[2, 15].Value = "500(转入储位-小于20字符)";
            worksheet.Cells[2, 16].Value = "500(转移数量-为整数)";
        }
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

                          "厂区", "类别","料号","品名","型号","转出OP类型","转出功能厂","转出仓库","转出料架号","转出储位","转入OP类型","转入功能厂","转入仓库","转入料架号","转入储位","转移数量"
               
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
                        //获取所有类型数据。
                        var apiStorageEnumsUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
                        var responStorageEnumsMessage = APIHelper.APIGetAsync(apiStorageEnumsUrl);
                        var storageEnumsresult = responStorageEnumsMessage.Content.ReadAsStringAsync().Result;
                        List<EnumerationDTO> enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(storageEnumsresult);

                        //获取厂区下所有料号
                        var apiUrl = string.Format("StorageManage/QueryMatByOutTypeAPI?Out_Type_Uid={0}&FunplantUid={1}", 0, GetPlantOrgUid());
                        var responMessage = APIHelper.APIGetAsync(apiUrl);
                        var materialInventorysresult = responMessage.Content.ReadAsStringAsync().Result;
                        List<MaterialInventoryDTO> materialInventorys = JsonConvert.DeserializeObject<List<MaterialInventoryDTO>>(materialInventorysresult);
                        //获取所有转出储位
                        var apiMaterialUrl = string.Format("StorageManage/QueryWarStByMaterialAPI?Material_UID={0}", 0);
                        HttpResponseMessage materialresponMessage = APIHelper.APIGetAsync(apiMaterialUrl);
                        var warehouse_storageresult = materialresponMessage.Content.ReadAsStringAsync().Result;
                        List<MaterialInventoryDTO> warehouse_storages = JsonConvert.DeserializeObject<List<MaterialInventoryDTO>>(warehouse_storageresult);
                        //获取所有转入储位
                        var inapiMaterialUrl = string.Format("StorageManage/QueryWarStAPI?inboundtype={0}&plantuid={1}", 0, GetPlantOrgUid());
                        HttpResponseMessage inmaterialresponMessage = APIHelper.APIGetAsync(inapiMaterialUrl);
                        var inwarehouse_storageresult = inmaterialresponMessage.Content.ReadAsStringAsync().Result;
                        List<MaterialInventoryDTO> inwarehouse_storages = JsonConvert.DeserializeObject<List<MaterialInventoryDTO>>(inwarehouse_storageresult);
                        //获得所有ORGBOMLIST
                        var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                        HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                        var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                        var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                        var storageTransferDTOList = new List<StorageTransferDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var storageTransferDTO = new StorageTransferDTO();
                            string Plant_name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                            int Plant_Id = GetPlantOrgUid();
                            if (string.IsNullOrWhiteSpace(Plant_name))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区没有值", i);
                                return errorInfo;
                            }
                            Plant_name = Plant_name.Trim();
                            if (Plant_Id == 0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有权限导入,请设置厂区", i);
                                return errorInfo;
                            }

                            string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出OP类型")].Value);
                            string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出功能厂")].Value);
                            int BG_Organization_UID = 0;
                            if (string.IsNullOrWhiteSpace(BG_Organization))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出OP类型没有值", i);
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
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行转出OP类型的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            int FunPlant_Organization_UID = 0;
                            if (string.IsNullOrWhiteSpace(FunPlant_Organization))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有转出功能厂", i);
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
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行转出功能厂代号的值没有找到", i);
                                    return errorInfo;
                                }
                            }

                            string INBG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入OP类型")].Value);
                            string INFunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入功能厂")].Value);
                            int INBG_Organization_UID = 0;
                            if (string.IsNullOrWhiteSpace(INBG_Organization))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入OP类型没有值", i);
                                return errorInfo;
                            }
                            else
                            {

                                var hasbg = orgboms.Where(m => m.BG == INBG_Organization).FirstOrDefault();
                                if (hasbg != null)
                                {
                                    INBG_Organization_UID = hasbg.BG_Organization_UID;
                                }
                                else
                                {
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行转入OP类型的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            int INFunPlant_Organization_UID = 0;
                            if (string.IsNullOrWhiteSpace(INFunPlant_Organization))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有转入功能厂", i);
                                return errorInfo;
                            }
                            else
                            {

                                var hasfunplant = orgboms.Where(m => m.BG == INBG_Organization & m.Funplant == INFunPlant_Organization).FirstOrDefault();
                                if (hasfunplant != null)
                                {
                                    INFunPlant_Organization_UID = hasfunplant.Funplant_UID;
                                }
                                else
                                {
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行转入功能厂代号的值没有找到", i);
                                    return errorInfo;
                                }
                            }
                            string enumvalue = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "类别")].Value);
                            if (string.IsNullOrWhiteSpace(enumvalue))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行类别没有值", i);
                                return errorInfo;
                            }
                            else if (enumvalue.Length > 500)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行类别长度超过最大限定[500]", i);
                                return errorInfo;
                            }

                            string material_Id = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "料号")].Value);
                            if (string.IsNullOrWhiteSpace(material_Id))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号没有值", i);
                                return errorInfo;
                            }
                            else if (material_Id.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行料号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string material_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "品名")].Value);
                            if (string.IsNullOrWhiteSpace(material_Name))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行品名没有值", i);
                                return errorInfo;
                            }
                            else if (material_Name.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行品名长度超过最大限定[50]", i);
                                return errorInfo;
                            }

                            string material_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "型号")].Value);
                            if (string.IsNullOrWhiteSpace(material_Types))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行型号没有值", i);
                                return errorInfo;
                            }
                            else if (material_Types.Length > 150)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行型号长度超过最大限定[150]", i);
                                return errorInfo;
                            }

                            string warehouse_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出仓库")].Value);
                            if (string.IsNullOrWhiteSpace(warehouse_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出仓库没有值", i);
                                return errorInfo;
                            }
                            else if (warehouse_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出仓库长度超过最大限定[10]", i);
                                return errorInfo;
                            }

                            string rack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出料架号")].Value);
                            if (string.IsNullOrWhiteSpace(rack_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出料架号没有值", i);
                                return errorInfo;
                            }
                            else if (rack_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出料架号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string storage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转出储位")].Value);
                            if (string.IsNullOrWhiteSpace(storage_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出储位没有值", i);
                                return errorInfo;
                            }
                            else if (storage_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转出储位长度超过最大限定[20]", i);
                                return errorInfo;
                            }


                            string inwarehouse_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入仓库")].Value);
                            if (string.IsNullOrWhiteSpace(inwarehouse_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入仓库没有值", i);
                                return errorInfo;
                            }
                            else if (inwarehouse_ID.Length > 10)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入仓库长度超过最大限定[10]", i);
                                return errorInfo;
                            }

                            string inrack_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入料架号")].Value);
                            if (string.IsNullOrWhiteSpace(inrack_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入料架号没有值", i);
                                return errorInfo;
                            }
                            else if (inrack_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入料架号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            string instorage_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转入储位")].Value);
                            if (string.IsNullOrWhiteSpace(instorage_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入储位没有值", i);
                                return errorInfo;
                            }
                            else if (instorage_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入储位长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            int check_Qty = 0;
                            string strcheck_Qty = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "转移数量")].Value);
                            if (string.IsNullOrWhiteSpace(strcheck_Qty))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转移数量没有值", i);
                                return errorInfo;
                            }
                            else
                            {
                                int.TryParse(strcheck_Qty, out check_Qty);
                            }
                            int PartType_UID = 0;
                            if (enums.Where(e => e.Enum_Value == enumvalue) != null && enums.Where(e => e.Enum_Value == enumvalue).Count() > 0)
                            {
                                PartType_UID = enums.Where(e => e.Enum_Value == enumvalue).FirstOrDefault().Enum_UID;
                            }

                            if (PartType_UID == 0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有找到对应的类型", i);
                                return errorInfo;
                            }

                            //获取料号
                            int Material_Uid = 0;
                            if (PartType_UID == 417 || PartType_UID == 433)  //不良品时只能选择MRB仓
                            {

                                var nmaterialInventorys = materialInventorys.Where(e => e.Warehouse_Type_UID == 418 && e.Material_Id == material_Id && e.Material_Name == material_Name && e.Material_Types == material_Types);

                                if (nmaterialInventorys != null && nmaterialInventorys.Count() > 0)
                                {
                                    Material_Uid = materialInventorys.Where(e => e.Warehouse_Type_UID == 418 && e.Material_Id == material_Id && e.Material_Name == material_Name && e.Material_Types == material_Types).FirstOrDefault().Material_Uid;
                                }
                            }
                            else
                            {
                                var nmaterialInventorys = materialInventorys.Where(e => e.Material_Id == material_Id && e.Material_Name == material_Name && e.Material_Types == material_Types);

                                if (nmaterialInventorys != null && nmaterialInventorys.Count() > 0)
                                {
                                    Material_Uid = materialInventorys.Where(e => e.Material_Id == material_Id && e.Material_Name == material_Name && e.Material_Types == material_Types).FirstOrDefault().Material_Uid;
                                }
                            }
                            if (Material_Uid == 0)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没根据料号，品名，类型 有找到对应的料号", i);
                                return errorInfo;
                            }

                            int Warehouse_Storage_UID = 0;

                            var nwarehouse_storages = warehouse_storages.Where(o => o.Material_Uid == Material_Uid && o.Warehouse_ID == warehouse_ID && o.Rack_ID == rack_ID && o.Storage_ID == storage_ID && o.BG_Organization_UID == BG_Organization_UID && o.FunPlant_Organization_UID == FunPlant_Organization_UID);
                            if (nwarehouse_storages != null && nwarehouse_storages.Count() > 0)
                            {
                                Warehouse_Storage_UID = nwarehouse_storages.FirstOrDefault().Warehouse_Storage_UID;
                            }else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没找到对应的转出料架", i);
                                return errorInfo;
                            }

                            int inWarehouse_Storage_UID = 0;

                            var innwarehouse_storages = inwarehouse_storages.Where(o => o.Warehouse_ID == inwarehouse_ID && o.Rack_ID == inrack_ID && o.Storage_ID == instorage_ID&&o.BG_Organization_UID== INBG_Organization_UID && o.FunPlant_Organization_UID== INFunPlant_Organization_UID);
                            if (innwarehouse_storages != null && innwarehouse_storages.Count() > 0)
                            {
                                inWarehouse_Storage_UID = innwarehouse_storages.FirstOrDefault().Warehouse_Storage_UID;
                            }else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行没有找到对应的转入料架号", i);
                                return errorInfo;
                            }

                            //导入数据判重
                            var isSelfRepeated = storageTransferDTOList.Exists(m => m.PartType_UID == PartType_UID && m.Material_Uid == Material_Uid && m.In_Warehouse_Storage_UID == inWarehouse_Storage_UID && m.Out_Warehouse_Storage_UID == Warehouse_Storage_UID && m.Transfer_Qty == check_Qty);
                            if (isSelfRepeated)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行在导入数据中重复,不可重复导入", i);
                                return errorInfo;
                            }

                            if (Warehouse_Storage_UID == inWarehouse_Storage_UID)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行转入储位与转出储位不可一致,不可重复导入", i);
                                return errorInfo;                              
                            }

                            var matinventory = materialInventorys.Where(m => m.Material_Uid == Material_Uid & m.Warehouse_Storage_UID == Warehouse_Storage_UID).FirstOrDefault();

                            if (matinventory != null)
                            {

                                if (check_Qty > matinventory.Inventory_Qty)
                                {
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行转出储位数量(" + matinventory.Inventory_Qty + ")不足", i);
                                    return errorInfo;                                  
                                }
                            }else
                            {
                              
                                    isExcelError = true;
                                    errorInfo = string.Format("第{0}行转出储位没有找到", i);
                                    return errorInfo;
                                                                
                            }
                            //类型
                            storageTransferDTO.PartType_UID = PartType_UID;
                            //料号
                            storageTransferDTO.Material_Uid = Material_Uid;
                            storageTransferDTO.Material_Id = material_Id;
                            storageTransferDTO.Material_Name = material_Name;
                            storageTransferDTO.Material_Types = material_Types;
                            //转出仓
                            storageTransferDTO.Out_Warehouse_Storage_UID = Warehouse_Storage_UID;
                            //转入仓
                            storageTransferDTO.In_Warehouse_Storage_UID = inWarehouse_Storage_UID;
                            //转移数量
                            storageTransferDTO.Transfer_Qty = check_Qty;                       
                            storageTransferDTO.Approver_Date = DateTime.Now;
                            storageTransferDTO.Approver_UID = CurrentUser.AccountUId;
                            storageTransferDTO.Applicant_UID= CurrentUser.AccountUId;
                            storageTransferDTO.Applicant_Date= DateTime.Now;
                            storageTransferDTOList.Add(storageTransferDTO);

                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(storageTransferDTOList);
                        var apiInsertStorageCheckDUrl = string.Format("StorageManage/ImportStorageTransferAPI");
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

        }
        
        public ActionResult StorageTransfer()
        {
            StorageTransferVM currentVM = new StorageTransferVM();
            var apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            //apiUrl = string.Format("StorageManage/QueryWarehouseStAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var Wars = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result);
            //currentVM.warst = Wars;
            currentVM.warst = null;
            return View(currentVM);
        }

        public ActionResult GetWarehouseSITE()
        {           
            var apiUrl = string.Format("StorageManage/GetWarehouseSITEAPI?planID={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");

        }
        public ActionResult QueryStorageTransfers(StorageTransferDTO search, Page page)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryStorageTransfersAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditStorageTransfer(StorageTransferDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddOrEditStorageTransferAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryStorageTransferByUid(int Storage_Transfer_UID)
        {
            var apiUrl = string.Format("StorageManage/QueryStorageTransferByUidAPI?Storage_Transfer_UID={0}", Storage_Transfer_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ApproveStorageTransferByUid(int Storage_Transfer_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveStTransferAPI?Storage_Transfer_UID={0}&Useruid={1}", Storage_Transfer_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string DeleteStorageTransfer(int Storage_Transfer_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteStorageTransferAPI?Storage_Transfer_UID={0}&userid={1}", Storage_Transfer_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public ActionResult DoAllExportStorageTransferReprot(StorageTransferDTO search)
        {


            search.Plant_UID = GetPlantOrgUid();

            var apiUrl = string.Format("StorageManage/DoAllExportStorageTransferReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageTransferDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageTransfer");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "储位移转单编号", "状态", "料号","品名","型号","转出OP类型","转出功能厂","转出仓","转出料架","转出储位","转入OP类型","转入功能厂","转入仓","转入料架",
                "转入储位","移转数量", "类别","申请者","申请时间","审核者","审核时间"};

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Storage_Transfer_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Out_BG_Organization;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Out_FunPlant_Organization;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Out_Warehouse_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Out_Rack_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Out_Storage_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.In_BG_Organization;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.In_FunPlant_Organization;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.In_Warehouse_ID;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.In_Rack_ID;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.In_Storage_ID;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Transfer_Qty;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Transfer_Type;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.ApplicantUser;// == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 21].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);
                

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportStorageTransferReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("StorageManage/DoExportStorageTransferReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageTransferDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageTransfer");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "储位移转单编号", "状态", "料号","品名","型号","转出OP类型","转出功能厂","转出仓","转出料架","转出储位","转入OP类型","转入功能厂","转入仓","转入料架",
                "转入储位","移转数量", "类别","申请者","申请时间","审核者","审核时间"};

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Storage_Transfer_ID;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Out_BG_Organization;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Out_FunPlant_Organization;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Out_Warehouse_ID;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Out_Rack_ID;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Out_Storage_ID;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.In_BG_Organization;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.In_FunPlant_Organization;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.In_Warehouse_ID;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.In_Rack_ID;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.In_Storage_ID;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Transfer_Qty;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Transfer_Type;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.ApplicantUser;// == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Applicant_Date == null ? "" : ((DateTime)currentRecord.Applicant_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet.Cells[index + 2, 21].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 22].Value = currentRecord.Approver_Date == null ? "" : ((DateTime)currentRecord.Approver_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }


        #endregion 储位移转     
        #region  一般需求计算

        public ActionResult MaterialNormalDemandList()
        {
            MaterialNormalDemandVM currentVM = new MaterialNormalDemandVM();
            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;

            apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            apiUrl = string.Format("Equipmentmaintenance/QueryAllMaterialAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var mats = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result);
            currentVM.mats = mats;

            return View(currentVM);
        }

        public ActionResult QueryMaterialNormalDemands(MaterialNormalDemandDTO search, Page page)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryMaterialNormalDemandsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddMatNormalDemand(MaterialNormalDemandDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddMatNormalDemandAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult MaterialNormalDemandDetail(int Material_Normal_Demand_UID,int Status_UID,bool IsView)
        {
            matNDDetail currentVM = new matNDDetail();
            currentVM.Material_Normal_Demand_UID= Material_Normal_Demand_UID;
            currentVM.Status_UID = Status_UID;
            currentVM.IsView = IsView;
            return View(currentVM);
        }

        public ActionResult QueryMatNormalDetails(MaterialNormalDemandDTO search, Page page)
        {
            var apiUrl = string.Format("StorageManage/QueryMatNormalDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult EditUserAdjustQty(string jsonDemandList)
        {
            var apiUrl = "StorageManage/EditUserAdjustQtyAPI";
            var entity = JsonConvert.DeserializeObject<matNDVM>(jsonDemandList);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ApproveMatND(int Material_Normal_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveMatNDAPI?Material_Normal_Demand_UID={0}&userid={1}", Material_Normal_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string DisapproveMatND(int Material_Normal_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DisapproveMatNDAPI?Material_Normal_Demand_UID={0}&userid={1}", Material_Normal_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        //导出excel----所有符合条件的项目
        public ActionResult DoExportFunction(int uid)
        {
            var apiUrl = string.Format("StorageManage/DoExportFunctionAPI?Material_Normal_Demand_UID={0}", uid);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialNormalDemandDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaterialNormalDemand");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂区", "BG", "功能厂","状态","机台类型","料号","品名","型号","安全库存量","现有库存量","毛需求量",
                "用户调整量","实际需求量", "计算日期", "需求日期","申请者","申请时间", "审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaterialNormalDemand");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.EQP_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Safe_Stock_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Existing_Stock_Qty;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Calculated_Demand_Qty;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.User_Adjustments_Qty;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Actual_Qty;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 17].Value = currentRecord.ApplicantUser;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Applicant_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                    worksheet.Cells[index + 2, 19].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.Approver_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //批量删除
        public string DeleteMatNormalDemandList(int Material_Normal_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatNormalDemandListAPI?Material_Normal_Demand_UID={0}&userid={1}", Material_Normal_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        //单笔删除
        public string DeleteMatNormalDemand(int Material_Normal_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatNormalDemandAPI?Material_Normal_Demand_UID={0}&userid={1}", Material_Normal_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 一般需求提交
        /// </summary>
        /// <param name="Material_Normal_Demand_UID"></param>
        /// <returns></returns>
        public ActionResult SubmitMatNormalDemand(int Material_Normal_Demand_UID)
        {
            var statusList = GetDemandStatusByName("待审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialNormalDemandStatusAPI?uid={0}&statusUID={1}", Material_Normal_Demand_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("提交失败");
        }
        
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="Material_Normal_Demand_UID"></param>
        /// <returns></returns>
        public ActionResult ApproveMatNormalDemand(int Material_Normal_Demand_UID)
        {
            var statusList = GetDemandStatusByName("已审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialRepairDemandStatus?uid={0}&statusUID={1}", Material_Normal_Demand_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("审核失败");
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

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="Fixture_Part_Demand_M_UID"></param>
        /// <returns></returns>
        public ActionResult SubmitMaterialNormalDemand(int Fixture_Part_Demand_M_UID)
        {
            var statusList = GetDemandStatusByName("待审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialNormalDemandStatusAPI?uid={0}&statusUID={1}", Fixture_Part_Demand_M_UID, status.Enum_UID);
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
                var api = string.Format("StorageManage/UpdateMaterialNormalDemandStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
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
        //public ActionResult ApproveFixturPartDemandByUID(int uid)
        //{
        //    var statusList = GetDemandStatusByName("已审核");

        //    EnumerationDTO status = null;
        //    if (statusList.Count > 0)
        //    {
        //        //调用存储过程汇总需求计算
        //        var submitOrderApi = string.Format("MaterialNormal/CalculateMaterialNormalDemandSummaryAPI?Fixture_Part_Demand_M_UID={0}&Applicant_UID={1}", uid, CurrentUser.AccountUId);
        //        HttpResponseMessage responMessageSubmitOrder = APIHelper.APIGetAsync(submitOrderApi);
        //        var resultSubmitOrder = responMessageSubmitOrder.Content.ReadAsStringAsync().Result.Replace("\"", "");
        //        if (resultSubmitOrder == "true")
        //        {
        //            status = statusList[0];
        //            //用枚举uid更新状态
        //            var api = string.Format("StorageManage/UpdateMaterialNormalDemandStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
        //            HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
        //            var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        //            return Content(result, "application/json");
        //        }
        //        return Content("审核失败");
        //    }
        //    return Content("审核失败");
        //}

        /// <summary>
        /// 配件需求审核取消
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        //public ActionResult ApproveCancelFixturPartDemandByUID(int uid)
        //{
        //    var statusList = GetDemandStatusByName("审核取消");

        //    EnumerationDTO status = null;
        //    if (statusList.Count > 0)
        //    {
        //        var apiApproveCancel = string.Format("MaterialNormal/ApproveCancelFixturPartDemandAPI?uid={0}", uid);
        //        HttpResponseMessage responMessageApproveCancel = APIHelper.APIGetAsync(apiApproveCancel);
        //        var resultresponMessageApproveCancel = responMessageApproveCancel.Content.ReadAsStringAsync().Result.Replace("\"", "");

        //        status = statusList[0];
        //        //用枚举uid更新状态
        //        var api = string.Format("StorageManage/UpdateMaterialNormalDemandStatusAPI?uid={0}&statusUID={1}", uid, status.Enum_UID);
        //        HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
        //        var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        //        return Content(result, "application/json");
        //    }
        //    return Content("审核取消失败");
        //}
        #endregion 一般需求计算        
        #region 库存明细查询
        public ActionResult DoAllExportMaterialInventoryReprot(MaterialInventoryDTO search)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/DoAllExportMaterialInventoryReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialInventoryDTO>>(result).ToList();
       
               var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaterialInventory");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "Business Group", "功能厂", "仓库别","料号","品名","型号","总数量","更新者","更新时间"};
            var stringHeads1 = new string[] { "排序", "Business Group", "功能厂", "仓库别", "料号", "品名", "型号", "仓库", "料架", "储位", "库存数量", "更新者", "更新时间", "说明" };
            List<MaterialInventoryDetailDTO> materialInventoryDetailDTOs = new List<MaterialInventoryDetailDTO>();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaterialInventory");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Funplant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Warehouse_Type;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Total_Qty;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.ModifyUser;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    materialInventoryDetailDTOs.AddRange(list[index].MaterialInventoryDetails);
                }

                var worksheet1 = excelPackage.Workbook.Worksheets.Add("MaterialInventoryDetail");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < materialInventoryDetailDTOs.Count; index++)
                {
                    var currentRecord = materialInventoryDetailDTOs[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.BG;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Funplant;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Warehouse_Type;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Warehouse_ID;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.Inventory_Qty;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.ModifyUser;
                    worksheet1.Cells[index + 2, 13].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet1.Cells[index + 2, 14].Value = currentRecord.Desc;

                }
                worksheet1.Cells.AutoFitColumns();


                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //导出excel----by勾选的项目
        public ActionResult DoExportMaterialInventoryReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("StorageManage/DoExportMaterialInventoryReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialInventoryDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaterialInventory");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "Business Group", "功能厂", "仓库别", "料号", "品名", "型号", "总数量", "更新者", "更新时间" };
            var stringHeads1 = new string[] { "排序", "Business Group", "功能厂", "仓库别", "料号", "品名", "型号", "仓库", "料架", "储位", "库存数量", "更新者", "更新时间" ,"说明"};

            List<MaterialInventoryDetailDTO> materialInventoryDetailDTOs = new List<MaterialInventoryDetailDTO>();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaterialInventory");

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
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Funplant;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.Warehouse_Type;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Total_Qty;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.ModifyUser;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    materialInventoryDetailDTOs.AddRange(list[index].MaterialInventoryDetails);
                }

                var worksheet1 = excelPackage.Workbook.Worksheets.Add("MaterialInventoryDetail");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads1.Length; colIndex++)
                {
                    worksheet1.Cells[1, colIndex + 1].Value = stringHeads1[colIndex];
                }

                //set cell value
                for (int index = 0; index < materialInventoryDetailDTOs.Count; index++)
                {
                    var currentRecord = materialInventoryDetailDTOs[index];
                    //seq
                    worksheet1.Cells[index + 2, 1].Value = index + 1;
                    worksheet1.Cells[index + 2, 2].Value = currentRecord.BG;
                    worksheet1.Cells[index + 2, 3].Value = currentRecord.Funplant;
                    worksheet1.Cells[index + 2, 4].Value = currentRecord.Warehouse_Type;
                    worksheet1.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                    worksheet1.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                    worksheet1.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                    worksheet1.Cells[index + 2, 8].Value = currentRecord.Warehouse_ID;
                    worksheet1.Cells[index + 2, 9].Value = currentRecord.Rack_ID;
                    worksheet1.Cells[index + 2, 10].Value = currentRecord.Storage_ID;
                    worksheet1.Cells[index + 2, 11].Value = currentRecord.Inventory_Qty;
                    worksheet1.Cells[index + 2, 12].Value = currentRecord.ModifyUser;
                    worksheet1.Cells[index + 2, 13].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                    worksheet1.Cells[index + 2, 14].Value = currentRecord.Desc;
                }
                worksheet1.Cells.AutoFitColumns();


                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult MaterialInventory()
        {
            MatInventoryVM currentVM = new MatInventoryVM();
            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.oporg = optypes;

            apiUrl = "StorageManage/QueryStorageEnumsAPI";
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.Types = enums;

            //apiUrl = string.Format("StorageManage/QueryWarehouseStAPI");
            //responMessage = APIHelper.APIGetAsync(apiUrl);
            //result = responMessage.Content.ReadAsStringAsync().Result;
            //var Wars = JsonConvert.DeserializeObject<List<WarehouseDTO>>(result);
            //currentVM.warst = Wars;

            currentVM.warst = null;
            return View(currentVM);
        }

        public ActionResult QueryMaterialInventorySum(MaterialInventoryDTO search, Page page)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryMaterialInventorySumAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult MaterialInventoryDetail(int Material_Inventory_UID)
        {
            ViewBag.Material_Inventory_UID = Material_Inventory_UID;
            return View();
        }

        public ActionResult QueryMaterialInventoryDetails(MaterialInventoryDTO search, Page page)
        {
            var apiUrl = string.Format("StorageManage/QueryMaterialInventoryDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion 库存明细查询 
        #region 备品需求计算

        public ActionResult MaterialSparepartsDemandList()
        {
            MatSparepartsDemandVM currentVM = new MatSparepartsDemandVM();
            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;

            apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            apiUrl = string.Format("Equipmentmaintenance/QueryAllMaterialAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var mats = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result);
            currentVM.mats = mats;

            return View(currentVM);
        }

        public ActionResult QueryMatSparepartsDemands(MaterialSparepartsDemandDTO search, Page page)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryMatSparepartsDemandsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddMatSparepartsDemand(MaterialSparepartsDemandDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddMatSparepartsDemandAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult MaterialSparepartsDemandDetail(int Material_Spareparts_Demand_UID, int Status_UID, bool IsView)
        {
            matNDDetail currentVM = new matNDDetail();
            currentVM.Material_Normal_Demand_UID = Material_Spareparts_Demand_UID;
            currentVM.Status_UID = Status_UID;
            currentVM.IsView = IsView;
            return View(currentVM);
        }

        public ActionResult QueryMatSDDetails(MaterialSparepartsDemandDTO search, Page page)
        {
            var apiUrl = string.Format("StorageManage/QueryMatSDDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult EditSDUserAdjustQty(string jsonDemandList)
        {
            var apiUrl = "StorageManage/EditSDUserAdjustQtyAPI";
            var entity = JsonConvert.DeserializeObject<matSDVM>(jsonDemandList);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ApproveMatSD(int Material_Spareparts_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveMatSDAPI?Material_Spareparts_Demand_UID={0}&userid={1}", Material_Spareparts_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        public string DisapproveMatSD(int Material_Spareparts_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DisapproveMatSDAPI?Material_Spareparts_Demand_UID={0}&userid={1}", Material_Spareparts_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 备品需求提交
        /// </summary>
        /// <param name="Material_Spareparts_Demand_UID"></param>
        /// <returns></returns>
        public ActionResult SubmitMatSparepartsDemand(int Material_Spareparts_Demand_UID)
        {
            var statusList = GetDemandStatusByName("待审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialSparepartsDemandStatusAPI?uid={0}&statusUID={1}", Material_Spareparts_Demand_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("提交失败");
        }

        //导出excel----所有符合条件的项目
        public ActionResult doSDExportFunction(int uid)
        {
            var apiUrl = string.Format("StorageManage/DoSDExportFunctionAPI?Material_Spareparts_Demand_UID={0}", uid);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialSparepartsDemandDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaterialSparepartsDemand");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂区", "BG", "功能厂","状态","机台类型","料号","品名","型号","预计开机数量","备品寿命","每月单机用量",
                "毛需求数量","用户调整量","实际需求量", "计算日期", "需求日期","申请者","申请时间", "审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaterialSparepartsDemand");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.EQP_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Forecast_PowerOn_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Material_Life;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Monthly_Consumption;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Calculated_Demand_Qty;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Actual_Qty;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Calculated_Demand_Qty + currentRecord.User_Adjustments_Qty;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 18].Value = currentRecord.ApplicantUser;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Applicant_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 21].Value = currentRecord.Approver_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //批量删除
        public string DeleteMatSparepartsDemandList(int Material_Spareparts_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatSparepartsDemandListAPI?Material_Spareparts_Demand_UID={0}&userid={1}", Material_Spareparts_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        //单笔删除
        public string DeleteMatSD(int Material_Spareparts_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatSDAPI?Material_Spareparts_Demand_UID={0}&userid={1}", Material_Spareparts_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        #endregion 备品需求计算    
        #region 返修品需求计算

        public ActionResult MaterialRepairDemandList()
        {
            MatRepairDemandVM currentVM = new MatRepairDemandVM();
            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;

            apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            apiUrl = string.Format("Equipmentmaintenance/QueryAllMaterialAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var mats = JsonConvert.DeserializeObject<List<MaterialInfoDTO>>(result);
            currentVM.mats = mats;

            return View(currentVM);
        }

        public ActionResult QueryMatRepairDemands(MaterialRepairDemandDTO search, Page page)
        {
            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryMatRepairDemandsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddMatRepairDemand(MaterialRepairDemandDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddMatRepairDemandAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult MaterialRepairDemandDetail(int Material_Repair_Demand_UID, int Status_UID, bool IsView)
        {
            matNDDetail currentVM = new matNDDetail();
            currentVM.Material_Normal_Demand_UID = Material_Repair_Demand_UID;
            currentVM.Status_UID = Status_UID;
            currentVM.IsView = IsView;
            return View(currentVM);
        }

        public ActionResult QueryMatRDDetails(MaterialRepairDemandDTO search, Page page)
        {
            var apiUrl = string.Format("StorageManage/QueryMatRDDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult EditRDUserAdjustQty(string jsonDemandList)
        {
            var apiUrl = "StorageManage/EditRDUserAdjustQtyAPI";
            var entity = JsonConvert.DeserializeObject<matRDVM>(jsonDemandList);
            entity.Modified_UID = this.CurrentUser.AccountUId;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(entity, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ApproveMatRD(int Material_Repair_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/ApproveMatRDAPI?Material_Repair_Demand_UID={0}&userid={1}", Material_Repair_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="Material_Repair_Demand_UID"></param>
        /// <returns></returns>
        public string DisapproveMatRD(int Material_Repair_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DisapproveMatRDAPI?Material_Repair_Demand_UID={0}&userid={1}", Material_Repair_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 一般需求提交
        /// </summary>
        /// <param name="Material_Repair_Demand_UID"></param>
        /// <returns></returns>
        public ActionResult SubmitMatRepairDemand(int Material_Repair_Demand_UID)
        {
            var statusList = GetDemandStatusByName("待审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialRepairDemandStatusAPI?uid={0}&statusUID={1}", Material_Repair_Demand_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("提交失败");
        }

        //导出excel----所有符合条件的项目
        public ActionResult doRDExportFunction(int uid)
        {
            var apiUrl = string.Format("StorageManage/DoRDExportFunctionAPI?Material_Repair_Demand_UID={0}", uid);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialRepairDemandDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaterialRepairDemand");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂区", "BG", "功能厂","状态","机台类型","料号","品名","型号","预计开机数量","前三个月平均损坏率","毛需求数量"
                ,"用户调整量","实际需求量", "计算日期", "需求日期","申请者","申请时间", "审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaterialRepairDemand");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.EQP_Type;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Forecast_PowerOn_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.F3M_PowerOn_Qty==0? 0: Convert.ToDecimal(Math.Round(1.0 * currentRecord.F3M_Damage_Qty / currentRecord.F3M_PowerOn_Qty, 2, MidpointRounding.AwayFromZero));
                    worksheet.Cells[index + 2, 12].Value = currentRecord.F3M_Damage_Qty;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Calculated_Demand_Qty;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.User_Adjustments_Qty;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.Calculated_Demand_Qty + currentRecord.User_Adjustments_Qty;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 17].Value = currentRecord.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 18].Value = currentRecord.ApplicantUser;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.Applicant_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                    worksheet.Cells[index + 2, 20].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 21].Value = currentRecord.Approver_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //批量删除
        public string DeleteMatRepairDemandList(int Material_Repair_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatRepairDemandListAPI?Material_Repair_Demand_UID={0}&userid={1}", Material_Repair_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        //单笔删除
        public string DeleteMatRD(int Material_Repair_Demand_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatRDAPI?Material_Repair_Demand_UID={0}&userid={1}", Material_Repair_Demand_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        #endregion 返修品需求计算    
        #region 采购需求汇总

        public ActionResult MaterialDemandSummaryList()
        {
            MatDemandSummaryVM currentVM = new MatDemandSummaryVM();
            var apiUrl = string.Format("StorageManage/QueryOpTypesByUserAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.Orgs = optypes;

            apiUrl = string.Format("StorageManage/QueryStorageEnumsAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var enums = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.enums = enums;

            return View(currentVM);
        }

        public ActionResult QueryMatDemandSummary(MaterialDemandSummaryDTO search, Page page)
        {

            search.Plant_UID = GetPlantOrgUid();
            var apiUrl = string.Format("StorageManage/QueryMatDemandSummaryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddMatDemandSummary(MaterialDemandSummaryDTO dto)
        {
            dto.Applicant_UID = CurrentUser.AccountUId;
            var apiUrl = string.Format("StorageManage/AddMatDemandSummaryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult MaterialDemandSummaryDetail(int Material_Demand_Summary_UID, int Status_UID)
        {
            matNDDetail currentVM = new matNDDetail();
            currentVM.Material_Normal_Demand_UID = Material_Demand_Summary_UID;
            currentVM.Status_UID = Status_UID;
            return View(currentVM);
        }

        public ActionResult QueryMatDSDetails(MaterialDemandSummaryDTO search, Page page)
        {
            var apiUrl = string.Format("StorageManage/QueryMatDSDetailsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //public string SubmitMatDemandSummary(int Material_Demand_Summary_UID)
        //{
        //    var apiUrl = string.Format("StorageManage/SubmitMatDemandSummaryAPI?Material_Demand_Summary_UID={0}&userid={1}", Material_Demand_Summary_UID, CurrentUser.AccountUId);
        //    HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

        //    return responMessage.Content.ReadAsStringAsync().Result;
        //}

        //导出excel----所有符合条件的项目
        public ActionResult doDSExportFunction(int uid)
        {
            var apiUrl = string.Format("StorageManage/doDSExportFunctionAPI?Material_Demand_Summary_UID={0}", uid);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MaterialDemandSummaryDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MaterialRepairDemand");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂区", "BG", "功能厂","状态","料号","品名","型号","一般需求量","备品需求量","返修品需求量"
                ,"应采购数量", "计算日期", "需求日期","申请者","申请时间", "审核者","审核时间"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("MaterialRepairDemand");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Id;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Material_Types;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.NormalDemand_Qty;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.SparepartsDemand_Qty;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Repair_Demand_Qty;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Be_Purchase_Qty;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Calculation_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
                    worksheet.Cells[index + 2, 15].Value = currentRecord.ApplicantUser;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.Applicant_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                    worksheet.Cells[index + 2, 17].Value = currentRecord.ApproverUser;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.Approver_Date.ToString(FormatConstants.DateTimeFormatStringByMin);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        //批量删除
        public string DeleteMatDemandSummaryList(int Material_Demand_Summary_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatDemandSummaryListAPI?Material_Demand_Summary_UID={0}&userid={1}", Material_Demand_Summary_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        
        //批量取消删除
        public ActionResult CancelDeleteMatDemandSummaryList(int Material_Demand_Summary_UID)
        {
            var statusList = GetDemandStatusByName("未审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialDemandSummaryStatusAPI?uid={0}&statusUID={1}", Material_Demand_Summary_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("取消删除失败");
        }

        //单笔删除
        public string DeleteMatDS(int Material_Demand_Summary_UID)
        {
            var apiUrl = string.Format("StorageManage/DeleteMatDSAPI?Material_Demand_Summary_UID={0}&userid={1}", Material_Demand_Summary_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        //取消删除
        public string DisdeleteMatDS(int Material_Demand_Summary_UID)
        {
            var apiUrl = string.Format("StorageManage/DisdeleteMatDSAPI?Material_Demand_Summary_UID={0}&userid={1}", Material_Demand_Summary_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        //转采购
        public string PurchaseMatDS(int Material_Demand_Summary_UID)
        {
            var apiUrl = string.Format("StorageManage/PurchaseMatDSAPI?Material_Demand_Summary_UID={0}&userid={1}", Material_Demand_Summary_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }

        //取消采购
        public string DisPurchaseMatDS(int Material_Demand_Summary_UID)
        {
            var apiUrl = string.Format("StorageManage/DisPurchaseMatDSAPI?Material_Demand_Summary_UID={0}&userid={1}", Material_Demand_Summary_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            return responMessage.Content.ReadAsStringAsync().Result;
        }
        /// <summary>
        /// 备品需求提交
        /// </summary>
        /// <param name="Material_Demand_Summary_UID"></param>
        /// <returns></returns>
        public ActionResult SubmitMatDemandSummary(int Material_Demand_Summary_UID)
        {
            var statusList = GetDemandStatusByName("待审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialDemandSummaryStatusAPI?uid={0}&statusUID={1}", Material_Demand_Summary_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("提交失败");
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="Material_Demand_Summary_UID"></param>
        /// <returns></returns>
        public ActionResult ApproveMatDemandSummary(int Material_Demand_Summary_UID)
        {
            var statusList = GetDemandStatusByName("已审核");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialDemandSummaryStatusAPI?uid={0}&statusUID={1}", Material_Demand_Summary_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("审核失败");
        }

        /// <summary>
        /// 审核取消
        /// </summary>
        /// <param name="Material_Demand_Summary_UID"></param>
        /// <returns></returns>
        public ActionResult CancelApproveMatDemandSummary(int Material_Demand_Summary_UID)
        {
            var statusList = GetDemandStatusByName("审核取消");

            EnumerationDTO status = null;
            if (statusList.Count > 0)
            {
                status = statusList[0];
                //用枚举uid更新状态
                var api = string.Format("StorageManage/UpdateMaterialDemandSummaryStatusAPI?uid={0}&statusUID={1}", Material_Demand_Summary_UID, status.Enum_UID);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(api);
                var result = responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
                return Content(result, "application/json");
            }
            return Content("审核取消失败");
        }

        #endregion 采购需求汇总 
        #region 库存报表

        public ActionResult StorageReport()
        {
            StorageReportVM currentVM = new StorageReportVM();
            var apiUrl = string.Format("StorageManage/QueryPlantsAPI?plantorguid={0}", GetPlantOrgUid());
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var optypes = JsonConvert.DeserializeObject<List<SystemOrgDTO>>(result);
            currentVM.plants = optypes;
            apiUrl = string.Format("StorageManage/QueryWarehouseTypesAPI");
            responMessage = APIHelper.APIGetAsync(apiUrl);
            result = responMessage.Content.ReadAsStringAsync().Result;
            var types = JsonConvert.DeserializeObject<List<EnumerationDTO>>(result);
            currentVM.Types = types;
            return View(currentVM);
        }

        public ActionResult QueryStorageReports(StorageReportDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("StorageManage/QueryStorageReportsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryBGByPlant(int plantuid)
        {
            var apiUrl = string.Format("StorageManage/QueryBGByPlantAPI?plantuid={0}", plantuid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);

            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        //导出excel----所有符合条件的项目
        public ActionResult DoSRExportFunction(int plant,int bg,int funplant,string material,DateTime start_date,DateTime end_date)
        {
            if(plant==0)
            {
                plant = GetPlantOrgUid();
            }
            var apiUrl = string.Format(@"StorageManage/DoSRExportFunctionAPI?plant={0}&bg={1}&funplant={2}&material={3}
                                                                    &start_date={4}&end_date={5}", plant, bg, funplant, material, start_date, end_date);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<StorageReportDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("StorageReport");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "序号", "厂区", "料号","品名","型号","期初库存","本期入库","本期出库","本期结存","仓库类型"};
            if (funplant!=0)
                stringHeads = new string[] { "序号", "厂区", "BG", "功能厂", "料号", "品名", "型号", "期初初库存", "本期入库", "本期出库", "本期结存", "仓库类型" };
            else if (bg!=0)
                stringHeads = new string[] { "序号", "厂区", "BG", "料号", "品名", "型号", "期初库存", "本期入库", "本期出库", "本期结存", "仓库类型" };
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("StorageReport");

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
                        worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                        worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant;
                        worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Id;
                        worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Name;
                        worksheet.Cells[index + 2, 7].Value = currentRecord.Material_Types;
                        worksheet.Cells[index + 2, 8].Value = currentRecord.Balance_Qty;
                        worksheet.Cells[index + 2, 9].Value = currentRecord.In_Qty;
                        worksheet.Cells[index + 2, 10].Value = currentRecord.Out_Qty;
                        worksheet.Cells[index + 2, 11].Value = currentRecord.Last_Qty;
                        worksheet.Cells[index + 2, 12].Value = currentRecord.Warehouse_Type_UID== 418 ? "MRB仓" : "正常仓";
                    }
                    else if(bg != 0)
                    {
                        worksheet.Cells[index + 2, 3].Value = currentRecord.BG;
                        worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Id;
                        worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Name;
                        worksheet.Cells[index + 2, 6].Value = currentRecord.Material_Types;
                        worksheet.Cells[index + 2, 7].Value = currentRecord.Balance_Qty;
                        worksheet.Cells[index + 2, 8].Value = currentRecord.In_Qty;
                        worksheet.Cells[index + 2, 9].Value = currentRecord.Out_Qty;
                        worksheet.Cells[index + 2, 10].Value = currentRecord.Last_Qty;
                        worksheet.Cells[index + 2, 11].Value = currentRecord.Warehouse_Type_UID == 418 ? "MRB仓" : "正常仓";

                    }
                    else
                    {
                        worksheet.Cells[index + 2, 3].Value = currentRecord.Material_Id;
                        worksheet.Cells[index + 2, 4].Value = currentRecord.Material_Name;
                        worksheet.Cells[index + 2, 5].Value = currentRecord.Material_Types;
                        worksheet.Cells[index + 2, 6].Value = currentRecord.Balance_Qty;
                        worksheet.Cells[index + 2, 7].Value = currentRecord.In_Qty;
                        worksheet.Cells[index + 2, 8].Value = currentRecord.Out_Qty;
                        worksheet.Cells[index + 2, 9].Value = currentRecord.Last_Qty;
                        worksheet.Cells[index + 2, 10].Value = currentRecord.Warehouse_Type_UID == 418 ? "MRB仓" : "正常仓";

                    }
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        #endregion 库存报表 
    }
}