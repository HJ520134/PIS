using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using OfficeOpenXml.Style;
using System.Drawing;
using PDMS.Common.Constants;
using PDMS.Common;
using System.Text.RegularExpressions;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing;

namespace PDMS.Web.Controllers
{
    public class OEEController : WebControllerBase
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
        #region   -----OEE_MachineInfo  Create By WenFan -------
        public ActionResult OEE_MachineInfo()
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
            return View("OEE_MachineInfo", currentVM);
        }

        public ActionResult QueryOEE_MachineInfo(OEE_MachineInfoDTO serchModel, Page page)
        {
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/QueryOEE_MachineInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(serchModel, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOEE_Machine(OEE_MachineInfoDTO serchModel)
        {
            serchModel.Modify_UID = this.CurrentUser.AccountUId;
            serchModel.Modify_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/AddOEE_MachineAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult ExportOEE_MachineByQuery(OEE_MachineInfoDTO serchModel)
        {
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/ExportOEE_MachineByQueryAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_MachineInfoDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("OEE_MachineReport");
            var stringHeads = GetOEE_MachineByHead();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("OEE_MachineReport");
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 2, 1].Value = index + 1;//序号
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant_Organization_Name;//厂区
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization_Name;//BG
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization_Name;//功能厂
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Project_Name;//制程
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Station_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.EQP_EMTSerialNum;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.MachineNo;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modify_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modify_Date.ToString("yyyy-MM-dd HH:mm");
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult ExportOEE_Machine(string uids)
        {
            var apiUrl = string.Format("OEE/ExportOEE_MachineAPI?uids={0}", uids);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_MachineInfoDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("OEE_MachineReport");
            var stringHeads = GetOEE_MachineByHead();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("OEE_MachineReport");
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 2, 1].Value = index + 1;//序号
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Plant_Organization_Name;//厂区
                    worksheet.Cells[index + 2, 3].Value = currentRecord.BG_Organization_Name;//BG
                    worksheet.Cells[index + 2, 4].Value = currentRecord.FunPlant_Organization_Name;//功能厂
                    worksheet.Cells[index + 2, 5].Value = currentRecord.Project_Name;//制程
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Line_Name;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Station_Name;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.EQP_EMTSerialNum;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.MachineNo;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Is_Enable ? "启用" : "禁用";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modify_Name;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modify_Date.ToString("yyyy-MM-dd HH:mm");
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        public ActionResult UpdateOEE_Machine(OEE_MachineInfoDTO serchModel)
        {
            serchModel.Modify_UID = this.CurrentUser.AccountUId;
            serchModel.Modify_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/UpdateOEE_MachineAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetOEE_MachineInfoByUID(string uid)
        {
            var apiUrl = string.Format("OEE/GetOEE_MachineInfoByUIDAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult GetOEE_MachineInfoByEMT(int PlantUID, string EMT)
        {
            var apiUrl = string.Format("OEE/GetOEE_MachineInfoByEMTAPI?PlantUID={0}&&EMT={1}", PlantUID, EMT);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DeleteOEE_Machine(string OEE_MachineInfo_UID)
        {
            var model = new OEE_MachineInfoDTO();
            model.OEE_MachineInfo_UID = int.Parse(OEE_MachineInfo_UID);
            string json = JsonConvert.SerializeObject(model);
            var apiUrl = string.Format("OEE/DeleteOEE_MachineAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportOEE_Machine(HttpPostedFileBase uploadName)
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
                        "机台EMT号",
                         "厂区",
                        "流水线名称",
                        "工站名称",
                        "机台名称",
                        "是否启用",
                };

                var iRow = 1;
                bool isExcelError = false;
                //1 验证表头
                for (int i = 1; i <= propertiesHead.Length; i++)
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

                //获得所有ORGBOMLIST
                var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);
                var OEE_MachineList = new List<OEE_MachineInfoDTO>();

                //限制一次性只能最大导入200
                if (totalRows > 200)
                {
                    errorInfo = string.Format("目前只支持一次性导入200行，请分批导入，谢谢合作！");
                    return errorInfo;
                }

                for (int i = 2; i <= totalRows; i++)
                {
                    OEE_MachineInfoDTO machineModel = new OEE_MachineInfoDTO();
                    string Plant_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                    string MachineEMT = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台EMT号")].Value);

                    string Line_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "流水线名称")].Value);
                    string Station_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站名称")].Value);
                    string Machine_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台名称")].Value);
                    string Is_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);

                    #region 验证栏位

                    //厂区
                    if (string.IsNullOrEmpty(Plant_Name))
                    {
                        errorInfo = string.Format("第{0}厂区不存在", i);
                        return errorInfo;
                    }
                    else
                    {
                        var hasbg = orgboms.Where(m => m.Plant == Plant_Name).FirstOrDefault();
                        if (hasbg != null)
                        {
                            machineModel.Plant_Organization_UID = hasbg.Plant_Organization_UID;
                        }
                        else
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行厂区的值没有找到", i);
                            return errorInfo;
                        }
                    }
                    //EMT No
                    if (string.IsNullOrEmpty(MachineEMT))
                    {
                        errorInfo = string.Format("第{0}EMT号不存在", i);
                        return errorInfo;
                    }

                    machineModel.BG_Organization_UID = 0;
                    machineModel.FunPlant_Organization_UID = 0;
                    machineModel.Plant_Organization_Name = Plant_Name;
                    machineModel.BG_Organization_Name = "";
                    machineModel.FunPlant_Organization_Name = "";
                    machineModel.Project_Name = "";
                    machineModel.Line_Name = Line_Name;
                    machineModel.EQP_EMTSerialNum = MachineEMT;
                    machineModel.MachineNo = Machine_Name;
                    machineModel.Is_Enable = Is_Enable == "1" ? true : false;
                    machineModel.Modify_UID = this.CurrentUser.AccountUId;
                    machineModel.Modify_Date = DateTime.Now;
                    machineModel.Station_Name = Station_Name;
                    #endregion
                    OEE_MachineList.Add(machineModel);
                }

                if (OEE_MachineList.Distinct().Count() != totalRows - 1)
                {
                    errorInfo = "导入的Excel有重复行";
                    return errorInfo;
                }

                //检查数据库是否有重复
                string json = JsonConvert.SerializeObject(OEE_MachineList);
                var apiUrl = string.Format("OEE/ImportOEE_MachineAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                if (result.Contains("SUCCESS"))
                {
                    return "SUCCESS";
                }
                else
                {
                    return result;
                }
            }
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

        private string[] GetOEE_MachineByHead()
        {
            var stringHeads = new string[]
             {
            "序号",
            "厂区",
            "Business Group",
              "功能厂",
                "专案",
                  "Line",
                   "Station",
                    "机台ETM号",
                      "机台名称",
                        "是否启用",
                          "最后更新者",
                            "最后更新时间",

             };
            return stringHeads;
        }
        #endregion

        #region  ----OEE  DefectCode -- Create by Justin----

        public ActionResult OEE_StationDefectCode()
        {

            OEE_DownTimeCodeVM currentVM = new OEE_DownTimeCodeVM();
            int optypeID = 0;
            int funPlantID = 0;
            //var apiUrlEnum = string.Format("OEE/GetEnumerationDTOAPI?Enum_Type={0}&Enum_Name={1}", "OEE", "DownTimeCodeType");
            //var responMessageEnum = APIHelper.APIGetAsync(apiUrlEnum);
            //var resultEnum = responMessageEnum.Content.ReadAsStringAsync().Result;
            //var EnumDTOs = JsonConvert.DeserializeObject<List<EnumerationDTO>>(resultEnum);
            //currentVM.Enums = EnumDTOs;
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
            return View("OEE_StationDefectCode", currentVM);
        }
        public ActionResult QueryOEE_StationDefectCode(OEE_StationDefectCodeDTO search, Page page)
        {

            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("OEE/QueryOEE_StationDefectCodeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditOEE_StationDefectCode(OEE_StationDefectCodeDTO dto, bool isEdit)
        {

            dto.Modify_UID = CurrentUser.AccountUId;
            dto.Modify_Date = DateTime.Now;
            var apiUrl = string.Format("OEE/AddOEE_StationDefectCodeAPI?isEdit={0}", isEdit);
            HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        public ActionResult QueryOEE_StationDefectCodeByUid(int OEE_StationDefectCode_UID)
        {
            var apiUrl = string.Format("OEE/QueryOEE_StationDefectCodeByUidAPI?OEE_StationDefectCode_UID={0}", OEE_StationDefectCode_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public FileResult DoExportOEE_StationDefectCode(string uids)
        {

            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            propertiesHead = GetDownTimeCodeHeadColumn();
            apiUrl = string.Format("OEE/GetOEE_StationDefectCodeDTOListAPI?uids={0}", uids);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_StationDefectCodeDTO>>(result);


            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("异常代码");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("OEE_StationDefectCode");
                SetExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.Project_Name;
                        worksheet.Cells[iRow, 5].Value = item.Line_Name;
                        worksheet.Cells[iRow, 6].Value = item.Station_Name;
                        worksheet.Cells[iRow, 7].Value = item.Sequence;
                        worksheet.Cells[iRow, 8].Value = item.Defect_Code;
                        worksheet.Cells[iRow, 9].Value = item.DefectEnglishName;
                        worksheet.Cells[iRow, 10].Value = item.DefecChinesetName;
                        worksheet.Cells[iRow, 11].Value = item.Is_Enable ? "Y" : "N";
                        worksheet.Cells[iRow, 12].Value = item.Modifyer;
                        worksheet.Cells[iRow, 13].Value = item.Modify_Date.ToString(FormatConstants.DateTimeFormatString);

                        iRow++;
                    }
                }
                excelPackage.Save();

                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }

        public FileResult DoAllExportOEE_StationDefectCode(OEE_StationDefectCodeDTO dto)
        {
            dto.Plant_Organization_UID = GetPlantOrgUid();
            string[] propertiesHead = new string[] { };
            propertiesHead = GetOEE_StationDefectCodeHeadColumn();
            var apiUrl = "OEE/QueryOEE_StationDefectCodeListAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_StationDefectCodeDTO>>(result);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("不良代码");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("DefectCode");
                SetOEE_StationDefectCodeExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {

                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.Project_Name;
                        worksheet.Cells[iRow, 5].Value = item.Line_Name;
                        worksheet.Cells[iRow, 6].Value = item.Station_Name;
                        worksheet.Cells[iRow, 7].Value = item.Sequence;
                        worksheet.Cells[iRow, 8].Value = item.Defect_Code;
                        worksheet.Cells[iRow, 9].Value = item.DefectEnglishName;
                        worksheet.Cells[iRow, 10].Value = item.DefecChinesetName;
                        worksheet.Cells[iRow, 11].Value = item.Is_Enable ? "Y" : "N";
                        worksheet.Cells[iRow, 12].Value = item.Modifyer;
                        worksheet.Cells[iRow, 13].Value = item.Modify_Date.ToString(FormatConstants.DateTimeFormatString);
                        iRow++;
                    }
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }

        private string[] GetOEE_StationDefectCodeHeadColumn()
        {

            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "专案",
                "线别",
                "工站",
                "序号",
                "不良代码",
                "不良英文名",
                "不良中文名",
                "是否启用",
                "最后更新者",
                "最后更新时间",

            };
            return propertiesHead;
        }
        private void SetOEE_StationDefectCodeExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
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

            worksheet.Cells["A1:N1"].Style.Font.Bold = true;
            worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

        }

        public string DeleteOEE_StationDefectCode(int OEE_StationDefectCode_UID)
        {
            var apiUrl = string.Format("OEE/DeleteOEE_StationDefectCodeAPI?OEE_StationDefectCode_UID={0}&userid={1}", OEE_StationDefectCode_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        }
        public string ImportOEE_StationDefectCodeExcel(HttpPostedFileBase uploadName)
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
                 "厂区",
                "OP类型",
                "功能厂",
                "专案",
                "线别",
                "工站",
                "序号",
                "不良代码",
                "不良英文名",
                "不良中文名",
                "是否启用"
                };
                    bool isExcelError = false;
                    //1 验证表头
                    for (int i = 1; i <= propertiesHead.Length; i++)
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

                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                    //获取专案
                    var apiUrlProjectDTO = string.Format("OEE/GetSystemProjectDTOAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", 0, 0);
                    var responMessageProjectDTO = APIHelper.APIGetAsync(apiUrlProjectDTO);
                    var resultProjectDTO = responMessageProjectDTO.Content.ReadAsStringAsync().Result;
                    var projectDTOs = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(resultProjectDTO);



                    //获取线别

                    var apiUrlLine = string.Format("OEE/GetAllGL_LineDTOListAPI");
                    var responMessageLine = APIHelper.APIGetAsync(apiUrlLine);
                    var resultLine = responMessageLine.Content.ReadAsStringAsync().Result;
                    var LineDTOs = JsonConvert.DeserializeObject<List<GL_LineDTO>>(resultLine);

                    //获取工站
                    var apiUrlStation = string.Format("OEE/GetAllGL_StationDTOListAPI");
                    var responMessageStation = APIHelper.APIGetAsync(apiUrlStation);
                    var resultStation = responMessageStation.Content.ReadAsStringAsync().Result;
                    var StationDTOs = JsonConvert.DeserializeObject<List<GL_StationDTO>>(resultStation);


                    //获取不良代码
                    var apiUrlOEE_DownTimeCode = string.Format("OEE/GetAllOEE_StationDefectCodeDTOListAPI");
                    var responMessageOEE_DownTimeCode = APIHelper.APIGetAsync(apiUrlOEE_DownTimeCode);
                    var resultOEE_DownTimeCode = responMessageOEE_DownTimeCode.Content.ReadAsStringAsync().Result;
                    var OEE_DownTimeCodes = JsonConvert.DeserializeObject<List<OEE_StationDefectCodeDTO>>(resultOEE_DownTimeCode);

                    List<OEE_StationDefectCodeDTO> OEE_DownTimeCodeDTOs = new List<OEE_StationDefectCodeDTO>();

                    for (int i = 2; i <= totalRows; i++)
                    {
                        OEE_StationDefectCodeDTO oEE_DownTimeCodeDTO = new OEE_StationDefectCodeDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        int Project_UID = 0;
                        int LineID = 0;
                        int StationID = 0;
                        int sequenceValue = 0;

                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);

                        string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案")].Value);
                        string Line_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "线别")].Value);
                        string Station_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站")].Value);

                        string sequence = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "序号")].Value);
                        string Defect_Code = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "不良代码")].Value);
                        string DefectEnglishName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "不良英文名")].Value);
                        string DefecChinesetName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "不良中文名")].Value);
                        string strIs_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);

                        #region 验证栏位
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            isExcelError = true;
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
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            isExcelError = true;
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
                                isExcelError = true;
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
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Project_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasProject = projectDTOs.Where(m => m.Project_Name == Project_Name).FirstOrDefault();
                            if (hasProject != null)
                            {
                                Project_UID = hasProject.Project_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行专案没有找到", i);
                                return errorInfo;
                            }

                        }

                        if (string.IsNullOrWhiteSpace(Line_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行线没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var lineTrimName = Line_Name.Trim();
                            var hasLine = LineDTOs.Where(m => m.CustomerID == Project_UID && m.LineName == lineTrimName).FirstOrDefault();
                            if (hasLine != null)
                            {
                                LineID = hasLine.LineID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行线名的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Station_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行工站没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var StationTrim = Station_Name.Trim();
                            var hasStation = StationDTOs.Where(m => m.CustomerID == Project_UID && m.LineID == LineID && m.StationName == StationTrim).FirstOrDefault();
                            if (hasStation != null)
                            {
                                StationID = hasStation.StationID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行工站名的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Defect_Code))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行不良代码没有值", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(DefectEnglishName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行不良英文名称没有值", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(DefecChinesetName))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行不良中文名没有值", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(sequence))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行序号没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var s = int.TryParse(sequence, out sequenceValue);

                            if (!s)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行序号值必须为整数", i);
                                return errorInfo;
                            }

                        }

                        //if (string.IsNullOrWhiteSpace(EnumDownTimeCodeType))
                        //{
                        //    isExcelError = true;
                        //    errorInfo = string.Format("第{0}行设备故障类别没有值", i);
                        //    return errorInfo;
                        //}
                        //else
                        //{
                        //    var hasEnum = EnumDTOs.Where(m => m.Enum_Value == EnumDownTimeCodeType).FirstOrDefault();
                        //    if (hasEnum != null)
                        //    {
                        //        Enum_UID = hasEnum.Enum_UID;
                        //    }
                        //    else
                        //    {
                        //        isExcelError = true;
                        //        errorInfo = string.Format("第{0}行设备故障类别没有找到", i);
                        //        return errorInfo;
                        //    }

                        //}

                        var OEE_DownTimeCode = OEE_DownTimeCodes.FirstOrDefault(o => o.StationID == StationID && o.Defect_Code == Defect_Code);
                        if (OEE_DownTimeCode != null)
                        {
                            oEE_DownTimeCodeDTO.OEE_StationDefectCode_UID = OEE_DownTimeCode.OEE_StationDefectCode_UID;
                        }

                        oEE_DownTimeCodeDTO.Plant_Organization_UID = Plant_Organization_UID;
                        oEE_DownTimeCodeDTO.BG_Organization_UID = BG_Organization_UID;
                        oEE_DownTimeCodeDTO.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        oEE_DownTimeCodeDTO.Project_UID = Project_UID;
                        oEE_DownTimeCodeDTO.LineID = LineID;
                        oEE_DownTimeCodeDTO.StationID = StationID;
                        oEE_DownTimeCodeDTO.Sequence = sequenceValue;
                        oEE_DownTimeCodeDTO.Defect_Code = Defect_Code;
                        oEE_DownTimeCodeDTO.DefectEnglishName = DefectEnglishName;
                        oEE_DownTimeCodeDTO.DefecChinesetName = DefecChinesetName;
                        oEE_DownTimeCodeDTO.Is_Enable = strIs_Enable == "Y" ? true : false;
                        oEE_DownTimeCodeDTO.Modify_UID = this.CurrentUser.AccountUId;
                        oEE_DownTimeCodeDTO.Modify_Date = DateTime.Now;


                        #endregion
                        OEE_DownTimeCodeDTOs.Add(oEE_DownTimeCodeDTO);

                        //做重复判断
                        var cfOEE_DownTimeCodeDTOs = OEE_DownTimeCodeDTOs.Where(o => o.StationID == StationID && o.Defect_Code == Defect_Code).ToList();
                        if (cfOEE_DownTimeCodeDTOs.Count > 1)
                        {
                            return string.Format("第{0}行数据重复", i);
                        }


                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(OEE_DownTimeCodeDTOs);
                    var apiInsertVendorInfoUrl = string.Format("OEE/ImportOEE_StationDefectCodekExcelAPI");
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
        #endregion
        #region    OEE_DownTimeCode


        public ActionResult GetStationDTOs(int LineId)
        {

            var apiUrl = string.Format("OEE/GetStationDTOAPI?LineId={0}", LineId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetLineDTOs(int CustomerID)
        {

            var apiUrl = string.Format("OEE/GetOEELineDTOAPI?CustomerID={0}", CustomerID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult OEE_DownTimeCode()
        {

            OEE_DownTimeCodeVM currentVM = new OEE_DownTimeCodeVM();
            int optypeID = 0;
            int funPlantID = 0;
            int plant_OrganizationUID = 0;

            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant != null && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
            {
                plant_OrganizationUID = CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID.Value;
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


            //var apiUrlEnum = string.Format("OEE/GetOEE_DownTypeDTOAPI?Plant_Organization_UID={0}", plant_OrganizationUID);
            //var responMessageEnum = APIHelper.APIGetAsync(apiUrlEnum);
            //var resultEnum = responMessageEnum.Content.ReadAsStringAsync().Result;
            //var EnumDTOs = JsonConvert.DeserializeObject<List<OEE_DownTypeDTO>>(resultEnum);
            //currentVM.Enums = EnumDTOs;
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("OEE_DownTimeCode", currentVM);
        }
        public ActionResult QueryOEE_DownTimeCodes(OEE_DownTimeCodeDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("OEE/QueryOEE_DownTimeCodesAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var apiUrl = string.Format("OEE/GetSystemProjectDTOAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", Plant_Organization_UID, BG_Organization_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetOEE_DownTypeDTOs(int Plant_Organization_UID)
        {
            var apiUrl = string.Format("OEE/GetOEE_DownTypeDTOAPI?Plant_Organization_UID={0}", Plant_Organization_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string AddOrEditOEE_DownTimeCode(OEE_DownTimeCodeDTO dto, bool isEdit)
        {

            dto.Modify_UID = CurrentUser.AccountUId;
            dto.Modify_Date = DateTime.Now;
            var apiUrl = string.Format("OEE/AddOrEditOEE_DownTimeCodeAPI?isEdit={0}", isEdit);
            HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        public ActionResult QueryDownTimeCodeByUid(int OEE_DownTimeCode_UID)
        {
            var apiUrl = string.Format("OEE/QueryDownTimeCodeByUidAPI?OEE_DownTimeCode_UID={0}", OEE_DownTimeCode_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public FileResult DoExportDownTimeCodeReprot(string uids)
        {

            string apiUrl = string.Empty;
            string[] propertiesHead = new string[] { };
            propertiesHead = GetDownTimeCodeHeadColumn();
            apiUrl = string.Format("OEE/GetDownTimeCodeDTOListAPI?uids={0}", uids);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_DownTimeCodeDTO>>(result);


            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("异常代码");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("DownTimeCode");
                SetExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.ProjectName;
                        worksheet.Cells[iRow, 5].Value = item.LineName;
                        worksheet.Cells[iRow, 6].Value = item.StationName;
                        worksheet.Cells[iRow, 7].Value = item.EnumDownTimeCodeType;
                        worksheet.Cells[iRow, 8].Value = item.Error_Code;
                        worksheet.Cells[iRow, 9].Value = item.Level_Details;
                        worksheet.Cells[iRow, 10].Value = item.Error_Reasons;
                        worksheet.Cells[iRow, 11].Value = item.Upload_Ways;
                        worksheet.Cells[iRow, 12].Value = item.Is_Enable ? "Y" : "N";
                        worksheet.Cells[iRow, 13].Value = item.Modifyer;
                        worksheet.Cells[iRow, 14].Value = item.Modify_Date.ToString(FormatConstants.DateTimeFormatString);

                        iRow++;
                    }
                }
                excelPackage.Save();

                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }
        public FileResult DoAllExportDownTimeCodeReprot(OEE_DownTimeCodeDTO dto)
        {
            dto.Plant_Organization_UID = GetPlantOrgUid();
            string[] propertiesHead = new string[] { };
            propertiesHead = GetDownTimeCodeHeadColumn();
            var apiUrl = "OEE/QueryDownTimeCodeListAPI";
            var responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_DownTimeCodeDTO>>(result);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("异常代码");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("DownTimeCode");
                SetExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.ProjectName;
                        worksheet.Cells[iRow, 5].Value = item.LineName;
                        worksheet.Cells[iRow, 6].Value = item.StationName;
                        worksheet.Cells[iRow, 7].Value = item.EnumDownTimeCodeType;
                        worksheet.Cells[iRow, 8].Value = item.Error_Code;
                        worksheet.Cells[iRow, 9].Value = item.Level_Details;
                        worksheet.Cells[iRow, 10].Value = item.Error_Reasons;
                        worksheet.Cells[iRow, 11].Value = item.Upload_Ways;
                        worksheet.Cells[iRow, 12].Value = item.Is_Enable ? "Y" : "N";
                        worksheet.Cells[iRow, 13].Value = item.Modifyer;
                        worksheet.Cells[iRow, 14].Value = item.Modify_Date.ToString(FormatConstants.DateTimeFormatString);
                        iRow++;
                    }
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        private string[] GetDownTimeCodeHeadColumn()
        {

            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "专案",
                "线别",
                "工站",
                "设备故障类别代码",
                "异常代码",
                "异常名称",
                "异常说明",
                "上传方式",
                "是否启用",
                "最后更新者",
                "最后更新时间",

            };
            return propertiesHead;
        }

        private string[] GetAbnormalDFCodeHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "专案名称",
                "线别名称",
                "工站名称",
                "机台名称",
                "DownTimeCode",
                "班次",
                "日期",
                "创建时间",
            };

            return propertiesHead;
        }

        private string[] GetDFCodeHeadColumn()
        {
            var propertiesHead = new[]
            {
                "厂区",
                "OP类型",
                "功能厂",
                "专案名称",
                "线别名称",
                "工站名称",
                "机台名称",
                "DF Code",
                "班次",
                "日期",
                "创建时间",
            };

            return propertiesHead;
        }
        private void SetExcelStyle(ExcelWorksheet worksheet, string[] propertiesHead)
        {
            try
            {
                //填充Title内容
                for (int i = 0; i < propertiesHead.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = propertiesHead[i];
                    worksheet.Column(i + 1).Width = 12;
                    worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(191, 191, 191));
                }

                //设置列宽
                string HeadRange = string.Format("A1:{0}1", NunberToChar(propertiesHead.Count()));
                worksheet.Cells[HeadRange].Style.Font.Bold = true;
                worksheet.Cells[HeadRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[HeadRange].Style.Fill.BackgroundColor.SetColor(Color.Orange);
            }
            catch (Exception)
            {
            }
        }
        public string DeleteDownTimeCode(int OEE_DownTimeCode_UID)
        {
            var apiUrl = string.Format("OEE/DeleteDownTimeCodeAPI?OEE_DownTimeCode_UID={0}&userid={1}", OEE_DownTimeCode_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        }
        public string ImportOEE_DownTimeCodekExcel(HttpPostedFileBase uploadName)
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
                 "厂区",
                "OP类型",
                "功能厂",
                "专案",
                "线别",
                "工站",
                "设备故障类别代码",
                "异常代码",
                "异常名称",
                "异常说明",
                "上传方式",
                "是否启用"
                };
                    bool isExcelError = false;
                    //1 验证表头
                    for (int i = 1; i <= propertiesHead.Length; i++)
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

                    //获得所有ORGBOMLIST
                    var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                    HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                    var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                    var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                    //获取专案
                    var apiUrlProjectDTO = string.Format("OEE/GetSystemProjectDTOAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", 0, 0);
                    var responMessageProjectDTO = APIHelper.APIGetAsync(apiUrlProjectDTO);
                    var resultProjectDTO = responMessageProjectDTO.Content.ReadAsStringAsync().Result;
                    var projectDTOs = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(resultProjectDTO);

                    //设备故障类别
                    var apiUrlDownType = string.Format("OEE/GetOEE_DownTypeDTOAPI?Plant_Organization_UID={0}", 0);
                    var responMessageDownType = APIHelper.APIGetAsync(apiUrlDownType);
                    var resultDownType = responMessageDownType.Content.ReadAsStringAsync().Result;
                    var OEE_DownTypeDTODTOs = JsonConvert.DeserializeObject<List<OEE_DownTypeDTO>>(resultDownType);
                    //设置上传方式
                    List<string> upload_Ways = new List<string>() { "自动识别", "手动更新" };

                    //获取线别

                    var apiUrlLine = string.Format("OEE/GetAllGL_LineDTOListAPI");
                    var responMessageLine = APIHelper.APIGetAsync(apiUrlLine);
                    var resultLine = responMessageLine.Content.ReadAsStringAsync().Result;
                    var LineDTOs = JsonConvert.DeserializeObject<List<GL_LineDTO>>(resultLine);

                    //获取工站
                    var apiUrlStation = string.Format("OEE/GetAllGL_StationDTOListAPI");
                    var responMessageStation = APIHelper.APIGetAsync(apiUrlStation);
                    var resultStation = responMessageStation.Content.ReadAsStringAsync().Result;
                    var StationDTOs = JsonConvert.DeserializeObject<List<GL_StationDTO>>(resultStation);

                    //获取不良代码
                    var apiUrlOEE_DownTimeCode = string.Format("OEE/GetAllOEE_DownTimeCodeDTOListAPI");
                    var responMessageOEE_DownTimeCode = APIHelper.APIGetAsync(apiUrlOEE_DownTimeCode);
                    var resultOEE_DownTimeCode = responMessageOEE_DownTimeCode.Content.ReadAsStringAsync().Result;
                    var OEE_DownTimeCodes = JsonConvert.DeserializeObject<List<OEE_DownTimeCodeDTO>>(resultOEE_DownTimeCode);

                    List<OEE_DownTimeCodeDTO> OEE_DownTimeCodeDTOs = new List<OEE_DownTimeCodeDTO>();

                    for (int i = 2; i <= totalRows; i++)
                    {
                        OEE_DownTimeCodeDTO oEE_DownTimeCodeDTO = new OEE_DownTimeCodeDTO();
                        int Plant_Organization_UID = 0;
                        int BG_Organization_UID = 0;
                        int? FunPlant_Organization_UID = null;
                        int Project_UID = 0;
                        int? LineID = null;
                        int? StationID = null;
                        int OEE_DownTimeType_UID = 0;

                        string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value);
                        string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                        string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);

                        string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案")].Value);
                        string Line_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "线别")].Value);
                        string Station_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "工站")].Value);

                        string EnumDownTimeCodeType = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "设备故障类别代码")].Value);
                        string Error_Code = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常代码")].Value);
                        string Level_Details = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常名称")].Value);
                        string Error_Reasons = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "异常说明")].Value);
                        string Upload_Ways = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "上传方式")].Value);
                        string strIs_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);

                        #region 验证栏位
                        if (string.IsNullOrWhiteSpace(Plant_Organization))
                        {
                            isExcelError = true;
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
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行厂区代号的值没有找到", i);
                                return errorInfo;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(BG_Organization))
                        {
                            isExcelError = true;
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
                                isExcelError = true;
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
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行功能厂代号的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Project_Name))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行专案没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasProject = projectDTOs.Where(m => m.Project_Name == Project_Name).FirstOrDefault();
                            if (hasProject != null)
                            {
                                Project_UID = hasProject.Project_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行专案没有找到", i);
                                return errorInfo;
                            }

                        }

                        if (string.IsNullOrWhiteSpace(Line_Name))
                        {
                            LineID = null;
                        }
                        else
                        {

                            var hasLine = LineDTOs.Where(m => m.CustomerID == Project_UID && m.LineName == Line_Name).FirstOrDefault();
                            if (hasLine != null)
                            {
                                LineID = hasLine.LineID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行线名的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Station_Name))
                        {
                            StationID = null;
                        }
                        else
                        {

                            var hasStation = StationDTOs.Where(m => m.CustomerID == Project_UID && m.LineID == LineID && m.StationName == Station_Name).FirstOrDefault();
                            if (hasStation != null)
                            {
                                StationID = hasStation.StationID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行工站名的值没有找到", i);
                                return errorInfo;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Error_Code))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常代码没有值", i);
                            return errorInfo;
                        }
                        else if (Error_Code.Length > 50)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常代码长度超过50", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(Level_Details))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常名称没有值", i);
                            return errorInfo;
                        }
                        else if (Level_Details.Length > 150)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常名称长度超过50", i);
                            return errorInfo;
                        }
                        if (Error_Reasons.Length > 500)
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行异常说明长度超过500", i);
                            return errorInfo;
                        }

                        if (string.IsNullOrWhiteSpace(Upload_Ways))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行上传方式没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            var hasupload = upload_Ways.Where(m => m == Upload_Ways).FirstOrDefault();
                            if (hasupload != null)
                            {
                                Upload_Ways = hasupload;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行上传方式没有找到", i);
                                return errorInfo;
                            }

                        }

                        if (string.IsNullOrWhiteSpace(EnumDownTimeCodeType))
                        {
                            isExcelError = true;
                            errorInfo = string.Format("第{0}行设备故障类别没有值", i);
                            return errorInfo;
                        }
                        else
                        {
                            EnumDownTimeCodeType = EnumDownTimeCodeType.Trim();
                            var hasEnum = OEE_DownTypeDTODTOs.Where(m => m.Type_Code == EnumDownTimeCodeType && m.Plant_Organization_UID == Plant_Organization_UID).FirstOrDefault();
                            if (hasEnum != null)
                            {
                                OEE_DownTimeType_UID = hasEnum.OEE_DownTimeType_UID;
                            }
                            else
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行设备故障类别代码没有找到", i);
                                return errorInfo;
                            }

                        }

                        var OEE_DownTimeCode = OEE_DownTimeCodes.FirstOrDefault(o => o.Project_UID == Project_UID && o.Error_Code == Error_Code);
                        if (OEE_DownTimeCode != null)
                        {
                            oEE_DownTimeCodeDTO.OEE_DownTimeCode_UID = OEE_DownTimeCode.OEE_DownTimeCode_UID;
                        }

                        oEE_DownTimeCodeDTO.Plant_Organization_UID = Plant_Organization_UID;
                        oEE_DownTimeCodeDTO.BG_Organization_UID = BG_Organization_UID;
                        oEE_DownTimeCodeDTO.FunPlant_Organization_UID = FunPlant_Organization_UID;
                        oEE_DownTimeCodeDTO.Project_UID = Project_UID;
                        oEE_DownTimeCodeDTO.LineID = LineID;
                        oEE_DownTimeCodeDTO.StationID = StationID;
                        oEE_DownTimeCodeDTO.OEE_DownTimeType_UID = OEE_DownTimeType_UID;
                        oEE_DownTimeCodeDTO.Error_Code = Error_Code;
                        oEE_DownTimeCodeDTO.Level_Details = Level_Details;
                        oEE_DownTimeCodeDTO.Error_Reasons = Error_Reasons;
                        oEE_DownTimeCodeDTO.Upload_Ways = Upload_Ways;
                        oEE_DownTimeCodeDTO.Is_Enable = strIs_Enable == "Y" ? true : false;
                        oEE_DownTimeCodeDTO.Modify_UID = this.CurrentUser.AccountUId;
                        oEE_DownTimeCodeDTO.Modify_Date = DateTime.Now;


                        #endregion
                        OEE_DownTimeCodeDTOs.Add(oEE_DownTimeCodeDTO);

                        //做重复判断
                        var cfOEE_DownTimeCodeDTOs = OEE_DownTimeCodeDTOs.Where(o => o.Project_UID == Project_UID && o.Error_Code == Error_Code).ToList();
                        if (cfOEE_DownTimeCodeDTOs.Count > 1)
                        {
                            return string.Format("第{0}行数据重复", i);
                        }
                    }
                    //插入表
                    var json = JsonConvert.SerializeObject(OEE_DownTimeCodeDTOs);
                    var apiInsertVendorInfoUrl = string.Format("OEE/ImportOEE_DownTimeCodekExcelAPI");
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
        #endregion

        #region 站点基本数据维护

        public ActionResult StationDataMaintence()
        {
            return View("StationDataMaintence");
        }

        public ActionResult QueryStationDataMaintence(OEE_MachineInfoDTO serchModel)
        {
            serchModel.Modify_UID = this.CurrentUser.AccountUId;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/OEE_UserStationAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DefectCodeMaintence(string MachineInfo_UID, string Plant_UID, string BG_UID, string Fun_UID, string EQP_Uid, string MachineName, string LineID, string StationID, string Project_UID)
        {
            OEE_MachineInfoDTO model = new OEE_MachineInfoDTO();
            model.OEE_MachineInfo_UID = int.Parse(MachineInfo_UID);
            model.EQP_Uid = int.Parse(EQP_Uid);
            model.Plant_Organization_UID = int.Parse(Plant_UID);
            model.BG_Organization_UID = int.Parse(BG_UID);
            if (!string.IsNullOrEmpty(Fun_UID))
            {
                model.FunPlant_Organization_UID = int.Parse(Fun_UID);
            }

            string json = JsonConvert.SerializeObject(model);
            var apiUrl = string.Format("OEE/GetTimeModelAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var timeModel = JsonConvert.DeserializeObject<TimeModel>(result);
            ViewBag.EQP_Uid = model.EQP_Uid;
            ViewBag.Plant_Organization_UID = model.Plant_Organization_UID;
            ViewBag.BG_Organization_UID = model.BG_Organization_UID;
            ViewBag.OEE_MachineInfo_UID = model.OEE_MachineInfo_UID;
            ViewBag.FunPlant_Organization_UID = model.FunPlant_Organization_UID;
            ViewBag.currentDate = timeModel.currentDate;
            ViewBag.currentShiftID = timeModel.currentShiftID;
            ViewBag.currentTimeInterval = timeModel.currentTimeInterval;
            ViewBag.MachineName = MachineName;
            ViewBag.LineID = int.Parse(LineID);
            ViewBag.StationID = int.Parse(StationID);
            ViewBag.Project_UID = int.Parse(Project_UID);
            return View();
        }

        /// <summary>
        /// 工站下面的不良名称
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public ActionResult QueryStationDefectCode(OEE_StationDefectCodeDTO serchModel)
        {
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/QueryStationDefectCodeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        /// <summary>
        /// 工站下面的不良名称
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public ActionResult SaveDefectCodeDailyNum(string jsonWithProduct)
        {
            var ModelList = JsonConvert.DeserializeObject<List<OEE_DefectCodeDailyNumDTO>>(jsonWithProduct);
            foreach (var item in ModelList)
            {
                item.Modify_UID = this.CurrentUser.AccountUId;
                item.Modify_Date = DateTime.Now;
            }

            string json = JsonConvert.SerializeObject(ModelList);
            var apiUrl = string.Format("OEE/SaveDefectCodeDailyNumAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteDefect(string dailynnum_uid)
        {
            var apiUrl = string.Format("OEE/DeleteDefectAPI?dailynnum_uid={0}", dailynnum_uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 工站下面的不良名称
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public ActionResult GetDefectCodeIID(OEE_StationDefectCodeDTO defaectNumModel)
        {
            string json = JsonConvert.SerializeObject(defaectNumModel);
            var apiUrl = string.Format("OEE/GetDefectCodeIIDAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        #endregion

        #region OEE报表
        public ActionResult OEE_EveryDayMachine(string Plant_UID, string BG_UID, string customerId, string lineName, string stationName, string MachineName, string ShiftID, string startTime, string endTime, string isFromPhotoReport)
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

            if (isFromPhotoReport == "PieOEE") //从统计图表跳转过来
            {
                var OEE_PieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_PieSerchParam);
                Plant_UID = OEE_PieCookie["Plant_UID"];
                BG_UID = OEE_PieCookie["BG_UID"];
                customerId = OEE_PieCookie["customerId"];
                lineName = OEE_PieCookie["lineName"];
                stationName = OEE_PieCookie["stationName"];
                MachineName = MachineName;
                ShiftID = OEE_PieCookie["ShiftID"];
                startTime = OEE_PieCookie["startTime"];
                endTime = OEE_PieCookie["startTime"];
                isFromPhotoReport = "true";
            }
            else if (isFromPhotoReport == "MetricsPieOEE")//报表跳转
            {
                var OEE_PieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsPieSerchParam);
                Plant_UID = OEE_PieCookie["Plant_UID"];
                BG_UID = OEE_PieCookie["BG_UID"];
                customerId = OEE_PieCookie["customerId"];
                lineName = OEE_PieCookie["lineName"];
                stationName = OEE_PieCookie["stationName"];
                MachineName = OEE_PieCookie["MachineName"];
                ShiftID = OEE_PieCookie["ShiftID"];
                startTime = OEE_PieCookie["startTime"];
                endTime = OEE_PieCookie["startTime"];
                isFromPhotoReport = "true";
            }
            else
            {
                var OEE_MetricsMy_Cookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsSerchParam);
                if (OEE_MetricsMy_Cookie != null)
                {
                    Plant_UID = OEE_MetricsMy_Cookie["Plant_UID"];
                    BG_UID = OEE_MetricsMy_Cookie["BG_UID"];
                    customerId = OEE_MetricsMy_Cookie["customerId"];
                    lineName = OEE_MetricsMy_Cookie["lineName"];
                    stationName = OEE_MetricsMy_Cookie["stationName"];
                    MachineName = OEE_MetricsMy_Cookie["MachineName"];
                    ShiftID = OEE_MetricsMy_Cookie["ShiftID"];
                    startTime = OEE_MetricsMy_Cookie["startTime"];
                    endTime = OEE_MetricsMy_Cookie["startTime"];
                    isFromPhotoReport = "true";
                }
            }

            ViewBag.Plant_UID = Plant_UID;
            ViewBag.BG_UID = BG_UID;
            ViewBag.customerId = customerId;
            ViewBag.lineName = lineName;
            ViewBag.stationName = stationName;
            ViewBag.MachineName = MachineName;
            ViewBag.ShiftID = ShiftID;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.isFromPhotoReport = isFromPhotoReport;

            return View("OEE_EveryDayMachine", currentVM);
        }

        public ActionResult QueryOEE_EveryDayMachine(OEE_EveryDayMachineDTO search, Page page)
        {
            var apiUrl = string.Format("OEE/QueryOEE_EveryDayMachineAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetAllByStationID(string stationUID)
        {
            var apiUrl = string.Format("OEE/GetAllByStationIDAPI?stationUID={0}", string.IsNullOrEmpty(stationUID) ? "0" : stationUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetMachineIndexName(OEE_ReprortSearchModel search, Page page)
        {
            var apiUrl = string.Format("OEE/GetMachineIndexNameAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetLastUpdateTime(string MachineInfo_UID, string Plant_UID, string BG_UID)
        {
            OEE_ReprortSearchModel search = new OEE_ReprortSearchModel();
            search.Plant_Organization_UID = int.Parse(string.IsNullOrEmpty(Plant_UID) ? "0" : Plant_UID);
            search.BG_Organization_UID = int.Parse(string.IsNullOrEmpty(BG_UID) ? "0" : BG_UID);
            search.EQP_Uid = int.Parse(string.IsNullOrEmpty(MachineInfo_UID) ? "0" : MachineInfo_UID);
            var apiUrl = string.Format("OEE/GetLastUpdateTimeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetStationLastUpdateTime(string StationID, string Plant_UID, string BG_UID)
        {
            OEE_ReprortSearchModel search = new OEE_ReprortSearchModel();
            search.Plant_Organization_UID = int.Parse(string.IsNullOrEmpty(Plant_UID) ? "0" : Plant_UID);
            search.BG_Organization_UID = int.Parse(string.IsNullOrEmpty(BG_UID) ? "0" : BG_UID);
            search.StationID = int.Parse(string.IsNullOrEmpty(StationID) ? "0" : StationID);
            var apiUrl = string.Format("OEE/GetStationLastUpdateTimeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetRealLastUpdateTime(string StationID, string Plant_UID, string BG_UID)
        {
            OEE_ReprortSearchModel search = new OEE_ReprortSearchModel();
            search.Plant_Organization_UID = int.Parse(string.IsNullOrEmpty(Plant_UID) ? "0" : Plant_UID);
            search.BG_Organization_UID = int.Parse(string.IsNullOrEmpty(BG_UID) ? "0" : BG_UID);
            search.StationID = int.Parse(string.IsNullOrEmpty(StationID) ? "0" : StationID);
            var apiUrl = string.Format("OEE/GetRealLastUpdateTimeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetLineLastUpdateTime(string LineID, string Plant_UID, string BG_UID)
        {
            OEE_ReprortSearchModel search = new OEE_ReprortSearchModel();
            search.Plant_Organization_UID = int.Parse(string.IsNullOrEmpty(Plant_UID) ? "0" : Plant_UID);
            search.BG_Organization_UID = int.Parse(string.IsNullOrEmpty(BG_UID) ? "0" : BG_UID);
            search.LineID = int.Parse(string.IsNullOrEmpty(LineID) ? "0" : LineID);
            var apiUrl = string.Format("OEE/GetLineLastUpdateTimeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetMachineBreakDown(OEE_ReprortSearchModel search, Page page)
        {
            var MetricsCooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsSerchParam);
            if (MetricsCooike != null)
            {
                MetricsCooike["Plant_UID"] = search.Plant_Organization_UID.ToString();
                MetricsCooike["BG_UID"] = search.BG_Organization_UID.ToString();
                MetricsCooike["customerId"] = search.CustomerID.ToString();
                MetricsCooike["lineName"] = search.LineID.ToString();
                MetricsCooike["stationName"] = search.StationID.ToString();
                MetricsCooike["MachineName"] = search.EQP_Uid.ToString();
                MetricsCooike["ShiftID"] = search.ShiftTimeID.ToString();
                MetricsCooike["startTime"] = search.StartTime.ToString("yyyy-MM-dd");
                MetricsCooike["endTime"] = search.EndTime.ToString("yyyy-MM-dd");
                MetricsCooike["isFromPhotoReport"] = "false";
                MetricsCooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(MetricsCooike);
            }
            else
            {
                var NewMetricsCooike = new HttpCookie(SessionConstants.OEE_MetricsSerchParam);
                NewMetricsCooike["Plant_UID"] = search.Plant_Organization_UID.ToString();
                NewMetricsCooike["BG_UID"] = search.BG_Organization_UID.ToString();
                NewMetricsCooike["customerId"] = search.CustomerID.ToString();
                NewMetricsCooike["lineName"] = search.LineID.ToString();
                NewMetricsCooike["stationName"] = search.StationID.ToString();
                NewMetricsCooike["MachineName"] = search.EQP_Uid.ToString();
                NewMetricsCooike["ShiftID"] = search.ShiftTimeID.ToString();
                NewMetricsCooike["startTime"] = search.StartTime.ToString("yyyy-MM-dd");
                NewMetricsCooike["endTime"] = search.EndTime.ToString("yyyy-MM-dd");
                NewMetricsCooike["isFromPhotoReport"] = "false";
                NewMetricsCooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(NewMetricsCooike);
            }


            search.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("OEE/GetMachineBreakDownAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetBreckDownDetial(OEE_ReprortSearchModel search, Page page)
        {
            var apiUrl = string.Format("OEE/GetBreckDownDetialAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult CheckExportDate(DateTime EndTime, DateTime StartTime)
        {
            if ((EndTime - StartTime).Days > 31)
            {
                return Content("false", "application/json");
            }
            return Content("true", "application/json");
        }

        /// <summary>
        /// 导出基本资料
        /// </summary>
        public ActionResult ExportOEEMetricsReport(OEE_ReprortSearchModel searchParmm)
        {
            searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrlMetrics = string.Format("OEE/GETOEEMetricsAPI");
            HttpResponseMessage responMessageMetrics = APIHelper.APIPostAsync(searchParmm, apiUrlMetrics);
            var resultMetrics = responMessageMetrics.Content.ReadAsStringAsync().Result;
            var apiUrlFirstYield = string.Format("OEE/GetFirstYieldAPI");
            HttpResponseMessage responMessageFirstYield = APIHelper.APIPostAsync(searchParmm, apiUrlFirstYield);
            var resultFirstYield = responMessageFirstYield.Content.ReadAsStringAsync().Result;
            var apiUrlDowntime = string.Format("OEE/GetDowntimeBreakdownAPI");
            HttpResponseMessage responMessageDowntime = APIHelper.APIPostAsync(searchParmm, apiUrlDowntime);
            var resultDowntime = responMessageDowntime.Content.ReadAsStringAsync().Result;

            var resultDowntimeList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultDowntime).ToList();
            var resultFirstYieldList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultFirstYield).ToList();
            var resultMetricsList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultMetrics).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PIS_OEEMetrics_Machine");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = GetMetricsHeads(searchParmm);
            using (var excelPackage = new ExcelPackage(stream))
            {
                var index = 6;
                var worksheet = excelPackage.Workbook.Worksheets.Add("PIS_OEEMetrics_Machine");
                var countIndex = 0;
                var rowIndexHead = 2;
                worksheet.View.ShowGridLines = false;

                var apiUrl = string.Format("OEE/GetAllByEQPIDAPI?eqpuid={0}", searchParmm.EQP_Uid);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var resultMachine = responMessage.Content.ReadAsStringAsync().Result;
                var resultMachineModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(resultMachine);
                worksheet.Cells[countIndex + 2, rowIndexHead + 1].Value = "Machine ID 机器号";
                worksheet.Cells[countIndex + 2, rowIndexHead + 2].Value = resultMachineModel == null ? string.Empty : resultMachineModel.EQP_EMTSerialNum;
                worksheet.Cells[countIndex + 3, rowIndexHead + 1].Value = "Machine Name 机器名称";
                worksheet.Cells[countIndex + 3, rowIndexHead + 2].Value = resultMachineModel == null ? string.Empty : resultMachineModel.MachineNo;
                worksheet.Cells[countIndex + 4, rowIndexHead + 1].Value = "Bay # 线号";
                worksheet.Cells[countIndex + 4, rowIndexHead + 2].Value = searchParmm.Param_LineName;
                worksheet.Cells[countIndex + 5, rowIndexHead + 1].Value = "Station 工位";
                worksheet.Cells[countIndex + 5, rowIndexHead + 2].Value = searchParmm.Param_StationName;
                var colorRangeFirst = "C2:E5";

                worksheet.Cells["D2:E2"].Merge = true;
                worksheet.Cells["D3:E3"].Merge = true;
                worksheet.Cells["D4:E4"].Merge = true;
                worksheet.Cells["D5:E5"].Merge = true;

                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[colorRangeFirst].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[colorRangeFirst].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[colorRangeFirst].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[colorRangeFirst].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[index + 1, colIndex + 3].Value = stringHeads[colIndex];
                }
                #region
                var downtimeGroup = resultDowntimeList.GroupBy(p => p.IndexName).ToList();
                var firstYielGroup = resultFirstYieldList.GroupBy(p => p.IndexName).ToList();
                var metricsGroup = resultMetricsList.GroupBy(p => p.IndexName).ToList();

                var dayCount = (searchParmm.EndTime - searchParmm.StartTime).Days;
                var rowIndex = 2;
                worksheet.Cells[index + 2, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.Fixtures);
                worksheet.Cells[index + 2, rowIndex + 2].Value = (metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Fixtures)).Sum(p => p.Sum(q => double.Parse(q.IndexCount))) / (dayCount + 1)).ToString("f0");
                worksheet.Cells[index + 3, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.POR);
                var EntireBuildPOR = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.POR)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 3, rowIndex + 2].Value = (EntireBuildPOR / (dayCount + 1)).ToString("f0");
                worksheet.Cells[index + 4, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.ActualCT);
                var EntireBuildActualCT = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ActualCT)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 4, rowIndex + 2].Value = (EntireBuildActualCT / (dayCount + 1)).ToString("f0");
                worksheet.Cells[index + 5, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.CTAchivementRate);
                worksheet.Cells[index + 5, rowIndex + 2].Value = EntireBuildActualCT == 0 ? 0.ToString("P2") : (EntireBuildPOR / EntireBuildActualCT).ToString("P2");

                worksheet.Cells[index + 6, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour);
                var EntireBuildTotalAvailableHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 6, rowIndex + 2].Value = EntireBuildTotalAvailableHour;
                worksheet.Cells[index + 7, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedHour);
                var EntireBuildPlannedHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedHour)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 7, rowIndex + 2].Value = EntireBuildPlannedHour;
                worksheet.Cells[index + 8, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedMinute);
                var EntireBuildPlannedMinute = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 8, rowIndex + 2].Value = EntireBuildPlannedMinute;
                worksheet.Cells[index + 9, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedOutput);
                var EntireBuildPlannedOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedOutput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 9, rowIndex + 2].Value = EntireBuildPlannedOutput;
                var downtimeCount = downtimeGroup.Count();
                for (int i = 0; i < downtimeCount; i++)
                {
                    worksheet.Cells[index + 10 + i, rowIndex + 1].Value = downtimeGroup[i].Key;
                    worksheet.Cells[index + 10 + i, rowIndex + 2].Value = downtimeGroup[i].Sum(p => double.Parse(p.IndexCount));

                    for (int j = 0; j <= dayCount; j++)
                    {
                        worksheet.Cells[index + 10 + i, rowIndex + 3 + j].Value = downtimeGroup[i].Where(p => p.MachineDate == searchParmm.StartTime.AddDays(j)).Sum(q => double.Parse(q.IndexCount));
                    }
                }

                //Uptime（minute） 正常运行时间（分钟）
                worksheet.Cells[index + 10 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(DowntimeBreakdownEnum.UptimeMinute);
                worksheet.Cells[index + 10 + downtimeCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).Sum(p => p.Sum(q => double.Parse(q.IndexCount))) - downtimeGroup.Sum(p => p.Sum(q => double.Parse(q.IndexCount)));

                worksheet.Cells[index + 11 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.RunningCapacity);
                var EntireBuildRunningCapacity = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.RunningCapacity)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 11 + downtimeCount, rowIndex + 2].Value = EntireBuildRunningCapacity;
                worksheet.Cells[index + 12 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.Throughput);
                var EntireBuildThroughput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Throughput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 12 + downtimeCount, rowIndex + 2].Value = EntireBuildThroughput;

                worksheet.Cells[index + 13 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.GoodPartOutput);
                worksheet.Cells[index + 13 + downtimeCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                var ThroughputTotalCount = double.Parse(worksheet.Cells[index + 12 + downtimeCount, rowIndex + 2].Value.ToString());
                var GoodPartOutputTotalCount = double.Parse(worksheet.Cells[index + 13 + downtimeCount, rowIndex + 2].Value.ToString());
                worksheet.Cells[index + 14 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.NGQTY);
                var EntireBuildNGQTY = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.NGQTY)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 14 + downtimeCount, rowIndex + 2].Value = EntireBuildNGQTY;

                //First pass yield
                var firstYielCount = firstYielGroup.Count();
                for (int i = 0; i < firstYielCount; i++)
                {
                    worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 1].Value = firstYielGroup[i].Key;
                    worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 2].Value = firstYielGroup[i].Sum(p => double.Parse(p.IndexCount));

                    for (int j = 0; j <= dayCount; j++)
                    {
                        worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 3 + j].Value = firstYielGroup[i].Where(p => p.MachineDate == searchParmm.StartTime.AddDays(j)).Sum(q => double.Parse(q.IndexCount));
                    }
                }
                //First pass yield % 通过率
                worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(DowntimeBreakdownEnum.FirstPassYield);
                worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildThroughput == 0 ? 0.ToString("p2") : ((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput).ToString("p2");

                worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.AvailableRate);
                worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildPlannedOutput == 0 ? 0.ToString("p2") : (EntireBuildRunningCapacity / EntireBuildPlannedOutput).ToString("p2");

                worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PerformanceRate);
                worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildRunningCapacity == 0 ? 0.ToString("p2") : (EntireBuildThroughput / EntireBuildRunningCapacity).ToString("p2");

                worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency);
                worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 2].Value = (EntireBuildActualCT == 0 || EntireBuildPlannedOutput == 0 || EntireBuildThroughput == 0) ? 0.ToString("p2") : ((EntireBuildPOR / EntireBuildActualCT) * (EntireBuildRunningCapacity / EntireBuildPlannedOutput) * (((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput))).ToString("p2");

                worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency);
                worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 2].Value = (EntireBuildThroughput == 0 || EntireBuildPlannedOutput == 0 || EntireBuildRunningCapacity == 0) ? 0.ToString("p2") : (((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput) * (EntireBuildThroughput / EntireBuildPlannedOutput)).ToString("p2");

                worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss);
                worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss)).Sum(p => p.Sum(q => double.Parse(q.IndexCount))).ToString("f2");

                for (int i = 0; i <= dayCount; i++)
                {
                    var Fixtures = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Fixtures));
                    worksheet.Cells[index + 2, rowIndex + 3 + i].Value = Fixtures.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var POR = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.POR));
                    worksheet.Cells[index + 3, rowIndex + 3 + i].Value = POR.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var ActualCT = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ActualCT));
                    worksheet.Cells[index + 4, rowIndex + 3 + i].Value = ActualCT.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("f0");
                    var CTAchivementRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.CTAchivementRate));
                    worksheet.Cells[index + 5, rowIndex + 3 + i].Value = CTAchivementRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");
                    var TotalAvailableHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour));
                    worksheet.Cells[index + 6, rowIndex + 3 + i].Value = TotalAvailableHour.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var PlannedHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedHour));
                    worksheet.Cells[index + 7, rowIndex + 3 + i].Value = PlannedHour.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var PlannedMinute = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute));
                    worksheet.Cells[index + 8, rowIndex + 3 + i].Value = PlannedMinute.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var PlannedOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedOutput));
                    worksheet.Cells[index + 9, rowIndex + 3 + i].Value = PlannedOutput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

                    //Uptime（minute） 正常运行时间（分钟）
                    var plannedMinuteCount = double.Parse(worksheet.Cells[index + 8, rowIndex + 3 + i].Value.ToString());
                    worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Value = plannedMinuteCount - downtimeGroup.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

                    //Downtime breakdown
                    var RunningCapacity = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.RunningCapacity));
                    worksheet.Cells[index + 11 + downtimeCount, rowIndex + 3 + i].Value = RunningCapacity.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var Throughput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Throughput));
                    worksheet.Cells[index + 12 + downtimeCount, rowIndex + 3 + i].Value = Throughput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var GoodPartOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput));
                    worksheet.Cells[index + 13 + downtimeCount, rowIndex + 3 + i].Value = GoodPartOutput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var NGQTY = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.NGQTY));
                    worksheet.Cells[index + 14 + downtimeCount, rowIndex + 3 + i].Value = NGQTY.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

                    //First pass yield % 通过率
                    var GoodPartOutputCount = double.Parse(worksheet.Cells[index + 13 + downtimeCount, rowIndex + 3 + i].Value.ToString());
                    var ThroughputCount = double.Parse(worksheet.Cells[index + 12 + downtimeCount, rowIndex + 3 + i].Value.ToString());
                    worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 3 + i].Value = ThroughputCount == 0 ? 0.ToString("p2") : (GoodPartOutputCount / ThroughputCount).ToString("p2");

                    //First yield breakdown
                    var AvailableRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.AvailableRate));
                    worksheet.Cells[index + 16 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = AvailableRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");
                    var PerformanceRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PerformanceRate));
                    worksheet.Cells[index + 17 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = PerformanceRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");
                    var EquipmentEfficiency = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency));
                    worksheet.Cells[index + 18 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = EquipmentEfficiency.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");
                    var AllEquipmentEfficiency = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency));
                    worksheet.Cells[index + 19 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = AllEquipmentEfficiency.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");
                    var ProductionTimeLoss = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss));
                    worksheet.Cells[index + 20 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = ProductionTimeLoss.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("f2");
                }

                #endregion
                //设置背景颜色
                var maxRow = downtimeCount + firstYielCount + 20 + index;
                var colorRange = string.Format("D{0}:D{1}", index + 1, maxRow);
                worksheet.Cells[string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 5), index + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 5), index + 1)].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);

                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 3, NunberToChar(dayCount + 5), index + 3), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 5, NunberToChar(dayCount + 5), index + 5), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 6, NunberToChar(dayCount + 5), index + 6), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 8, NunberToChar(dayCount + 5), index + 8), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 9, NunberToChar(dayCount + 5), index + 9), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 10, NunberToChar(dayCount + 5), index + downtimeCount + 10), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 11, NunberToChar(dayCount + 5), index + downtimeCount + 11), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 13, NunberToChar(dayCount + 5), index + downtimeCount + 13), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 14, NunberToChar(dayCount + 5), index + downtimeCount + 14), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 15, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 15), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 16, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 16), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 17, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 17), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 18, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 18), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 19, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 19), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 20, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 20), Color.LightSteelBlue);

                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Green);

                worksheet.Cells[colorRangeFirst].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRangeFirst].Style.Fill.BackgroundColor.SetColor(Color.DeepSkyBlue);
                worksheet.Cells["C2,C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C2,C5"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);

                worksheet.Cells[index + 10, 2].Value = "Downtime Breakdown";
                worksheet.Cells[index + 10, 2].Style.Font.Size = 14;
                worksheet.Cells[index + 10, 2].Style.TextRotation = 90;
                worksheet.Cells[string.Format("B{0}:B{1}", index + 10, downtimeCount + index + 10)].Merge = true;

                worksheet.Cells[index + 15 + downtimeCount, 2].Value = "Yield Breakdown";
                worksheet.Cells[index + 15 + downtimeCount, 2].Style.TextRotation = 90;
                worksheet.Cells[index + 15 + downtimeCount, 2].Style.Font.Size = 14;
                worksheet.Cells[string.Format("B{0}:B{1}", index + 15 + downtimeCount, index + downtimeCount + firstYielCount + 15)].Merge = true;
                worksheet.Cells[string.Format("A1:A{0}", maxRow)].Merge = true;
                //设置边框
                SetExcelCellStyle(worksheet, string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 4 + 1), maxRow));
                SetExcelCellStyle(worksheet, string.Format("B{0}:B{1}", index + 10, downtimeCount + index + 10));
                SetExcelCellStyle(worksheet, string.Format("B{0}:B{1}", index + 15 + downtimeCount, index + downtimeCount + firstYielCount + 15));
                worksheet.Cells.AutoFitColumns();
                worksheet.Column(1).SetTrueColumnWidth(2);
                worksheet.Column(2).SetTrueColumnWidth(4);
                worksheet.Row(downtimeCount + index + 10).Height = 130;
                worksheet.Row(index + downtimeCount + firstYielCount + 15).Height = 130;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        /// <summary>
        /// 客户版 带公式
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult ExportPublicOEEMetricsReport(OEE_ReprortSearchModel searchParmm)
        {
            searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrlMetrics = string.Format("OEE/GETOEEMetricsAPI");
            HttpResponseMessage responMessageMetrics = APIHelper.APIPostAsync(searchParmm, apiUrlMetrics);
            var resultMetrics = responMessageMetrics.Content.ReadAsStringAsync().Result;
            var apiUrlFirstYield = string.Format("OEE/GetFirstYieldAPI");
            HttpResponseMessage responMessageFirstYield = APIHelper.APIPostAsync(searchParmm, apiUrlFirstYield);
            var resultFirstYield = responMessageFirstYield.Content.ReadAsStringAsync().Result;
            var apiUrlDowntime = string.Format("OEE/GetDowntimeBreakdownAPI");
            HttpResponseMessage responMessageDowntime = APIHelper.APIPostAsync(searchParmm, apiUrlDowntime);
            var resultDowntime = responMessageDowntime.Content.ReadAsStringAsync().Result;

            var resultDowntimeList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultDowntime).ToList();
            var resultFirstYieldList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultFirstYield).ToList();
            var resultMetricsList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultMetrics).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PIS_OEEMetrics_Machine");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = GetMetricsHeads(searchParmm);
            using (var excelPackage = new ExcelPackage(stream))
            {
                var index = 6;
                var worksheet = excelPackage.Workbook.Worksheets.Add("PIS_OEEMetrics_Machine");
                var countIndex = 0;
                var rowIndexHead = 2;
                worksheet.View.ShowGridLines = false;

                var apiUrl = string.Format("OEE/GetAllByEQPIDAPI?eqpuid={0}", searchParmm.EQP_Uid);
                HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
                var resultMachine = responMessage.Content.ReadAsStringAsync().Result;
                var resultMachineModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(resultMachine);
                worksheet.Cells[countIndex + 2, rowIndexHead + 1].Value = "Machine ID 机器号";
                worksheet.Cells[countIndex + 2, rowIndexHead + 2].Value = resultMachineModel == null ? string.Empty : resultMachineModel.EQP_EMTSerialNum;
                worksheet.Cells[countIndex + 3, rowIndexHead + 1].Value = "Machine Name 机器名称";
                worksheet.Cells[countIndex + 3, rowIndexHead + 2].Value = resultMachineModel == null ? string.Empty : resultMachineModel.MachineNo;
                worksheet.Cells[countIndex + 4, rowIndexHead + 1].Value = "Bay # 线号";
                worksheet.Cells[countIndex + 4, rowIndexHead + 2].Value = searchParmm.Param_LineName;
                worksheet.Cells[countIndex + 5, rowIndexHead + 1].Value = "Station 工位";
                worksheet.Cells[countIndex + 5, rowIndexHead + 2].Value = searchParmm.Param_StationName;
                var colorRangeFirst = "C2:E5";

                worksheet.Cells["D2:E2"].Merge = true;
                worksheet.Cells["D3:E3"].Merge = true;
                worksheet.Cells["D4:E4"].Merge = true;
                worksheet.Cells["D5:E5"].Merge = true;

                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[colorRangeFirst].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[colorRangeFirst].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[colorRangeFirst].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[colorRangeFirst].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[index + 1, colIndex + 3].Value = stringHeads[colIndex];
                }
                #region
                var downtimeGroup = resultDowntimeList.GroupBy(p => p.IndexName).ToList();
                var firstYielGroup = resultFirstYieldList.GroupBy(p => p.IndexName).ToList();
                var metricsGroup = resultMetricsList.GroupBy(p => p.IndexName).ToList();

                var dayCount = (searchParmm.EndTime - searchParmm.StartTime).Days;
                var rowIndex = 2;
                worksheet.Cells[index + 2, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.Fixtures);
                worksheet.Cells[index + 2, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Fixtures)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 3, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.POR);
                var EntireBuildPOR = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.POR)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 3, rowIndex + 2].Value = EntireBuildPOR;
                worksheet.Cells[index + 4, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.ActualCT);
                var EntireBuildActualCT = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ActualCT)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 4, rowIndex + 2].Value = EntireBuildActualCT;
                worksheet.Cells[index + 5, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.CTAchivementRate);
                //worksheet.Cells[index + 5, rowIndex + 2].Value = EntireBuildActualCT == 0 ? 0.ToString("P2") : (EntireBuildPOR / EntireBuildActualCT).ToString("P2");
                worksheet.Cells[index + 5, rowIndex + 2].Formula = string.Format("=IF(ISERROR(D{0}/D{1}),{2},D{0}/D{1})", index + 3, index + 4, "");
                worksheet.Cells[index + 5, rowIndex + 2].Style.Numberformat.Format = "0.00%";

                worksheet.Cells[index + 6, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour);
                var EntireBuildTotalAvailableHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 6, rowIndex + 2].Value = EntireBuildTotalAvailableHour;
                worksheet.Cells[index + 7, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedHour);
                var EntireBuildPlannedHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedHour)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 7, rowIndex + 2].Value = EntireBuildPlannedHour;
                worksheet.Cells[index + 8, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedMinute);
                var EntireBuildPlannedMinute = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                //计划时间 PlannedMinute
                worksheet.Cells[index + 8, rowIndex + 2].Formula = string.Format("=D{0}*60", index + 7);
                worksheet.Cells[index + 9, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedOutput);
                var EntireBuildPlannedOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedOutput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                //worksheet.Cells[index + 9, rowIndex + 2].Value = EntireBuildPlannedOutput;

                //计划产出 PlannedOutputSS
                worksheet.Cells[index + 9, rowIndex + 2].Formula = string.Format("=D{0}*60/D{1}", index + 8, index + 3);
                var downtimeCount = downtimeGroup.Count();
                for (int i = 0; i < downtimeCount; i++)
                {
                    worksheet.Cells[index + 10 + i, rowIndex + 1].Value = downtimeGroup[i].Key;
                    worksheet.Cells[index + 10 + i, rowIndex + 2].Value = downtimeGroup[i].Sum(p => double.Parse(p.IndexCount));

                    for (int j = 0; j <= dayCount; j++)
                    {
                        worksheet.Cells[index + 10 + i, rowIndex + 3 + j].Value = downtimeGroup[i].Where(p => p.MachineDate == searchParmm.StartTime.AddDays(j)).Sum(q => double.Parse(q.IndexCount));
                    }
                }

                //Uptime（minute） 正常运行时间（分钟）
                worksheet.Cells[index + 10 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(DowntimeBreakdownEnum.UptimeMinute);
                //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).Sum(p => p.Sum(q => double.Parse(q.IndexCount))) - downtimeGroup.Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                if (downtimeCount == 0)
                {
                    worksheet.Cells[index + 10 + downtimeCount, rowIndex + 2].Formula = string.Format("=D{0}-sum(0)", index + 8);
                }
                else
                {
                    worksheet.Cells[index + 10 + downtimeCount, rowIndex + 2].Formula = string.Format("=D{0}-sum(D{1}:D{2})", index + 8, index + 10, index + 9 + downtimeCount);
                }

                //Running Capacity  运行生产能力
                worksheet.Cells[index + 11 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.RunningCapacity);
                var EntireBuildRunningCapacity = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.RunningCapacity)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                //worksheet.Cells[index + 11 + downtimeCount, rowIndex + 2].Value = EntireBuildRunningCapacity;
                worksheet.Cells[index + 11 + downtimeCount, rowIndex + 2].Formula = string.Format("=60/D{0}*D{1}", index + 3, index + 10 + downtimeCount);

                //Throughput 生产量
                worksheet.Cells[index + 12 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.Throughput);
                var EntireBuildThroughput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Throughput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                worksheet.Cells[index + 12 + downtimeCount, rowIndex + 2].Value = EntireBuildThroughput;

                var firstYielCount = firstYielGroup.Count();

                //Good Part Output 良品数量
                worksheet.Cells[index + 13 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.GoodPartOutput);
                worksheet.Cells[index + 13 + downtimeCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                var ThroughputTotalCount = double.Parse(worksheet.Cells[index + 12 + downtimeCount, rowIndex + 2].Value.ToString());
                var GoodPartOutputTotalCount = double.Parse(worksheet.Cells[index + 13 + downtimeCount, rowIndex + 2].Value.ToString());

                // NG QTY 不良品数量
                worksheet.Cells[index + 14 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.NGQTY);
                var EntireBuildNGQTY = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.NGQTY)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
                //worksheet.Cells[index + 14 + downtimeCount, rowIndex + 2].Value = EntireBuildNGQTY;
                if (firstYielCount == 0)
                {
                    worksheet.Cells[index + 14 + downtimeCount, rowIndex + 2].Formula = string.Format("=sum(0)");
                }
                else
                {
                    worksheet.Cells[index + 14 + downtimeCount, rowIndex + 2].Formula = string.Format("=sum(D{0}:D{1})", index + 15 + downtimeCount, index + 15 + downtimeCount + firstYielCount - 1);
                }

                //First pass yield
                for (int i = 0; i < firstYielCount; i++)
                {
                    worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 1].Value = firstYielGroup[i].Key;
                    worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 2].Value = firstYielGroup[i].Sum(p => double.Parse(p.IndexCount));

                    for (int j = 0; j <= dayCount; j++)
                    {
                        worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 3 + j].Value = firstYielGroup[i].Where(p => p.MachineDate == searchParmm.StartTime.AddDays(j)).Sum(q => double.Parse(q.IndexCount));
                    }
                }

                //First pass yield % 通过率
                worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(DowntimeBreakdownEnum.FirstPassYield);
                //worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildThroughput == 0 ? 0.ToString("p2") : ((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput).ToString("p2");
                worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}/D{1}", index + 13 + downtimeCount, index + 12 + downtimeCount);
                worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
                //AV% 设备时间开动率
                worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.AvailableRate);
                //worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildPlannedOutput == 0 ? 0.ToString("p2") : (EntireBuildRunningCapacity / EntireBuildPlannedOutput).ToString("p2");
                worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}/D{1}", index + 11 + downtimeCount, index + 9);
                worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
                //PF% 性能开动
                worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PerformanceRate);
                //worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildRunningCapacity == 0 ? 0.ToString("p2") : (EntireBuildThroughput / EntireBuildRunningCapacity).ToString("p2");
                worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}/D{1}", index + 12 + downtimeCount, index + 11 + downtimeCount);
                worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
                //OEE% (w/o Microstop) 整体设备效率 （不包含微停）
                worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency);
                //worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 2].Value = (EntireBuildActualCT == 0 || EntireBuildPlannedOutput == 0 || EntireBuildThroughput == 0) ? 0.ToString("p2") : ((EntireBuildPOR / EntireBuildActualCT) * (EntireBuildRunningCapacity / EntireBuildPlannedOutput) * (((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput))).ToString("p2");
                worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}*D{1}*D{2}", index + 5, index + 15 + downtimeCount + firstYielCount, index + 16 + downtimeCount + firstYielCount);
                worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
                //OEE% 整体设备效率
                worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency);
                //worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 2].Value = (EntireBuildThroughput == 0 || EntireBuildPlannedOutput == 0 || EntireBuildRunningCapacity == 0) ? 0.ToString("p2") : (((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput) * (EntireBuildThroughput / EntireBuildPlannedOutput)).ToString("p2");
                worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}*D{1}*D{2}", index + 17 + downtimeCount + firstYielCount, index + 15 + downtimeCount + firstYielCount, index + 16 + downtimeCount + firstYielCount);
                worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
                //Production time loss (Hour) 生产损失（小时）
                worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss);
                //worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss)).Sum(p => p.Sum(q => double.Parse(q.IndexCount))).ToString("f2");
                worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=(D{0}-D{1})*D{2}/3600", index + 9, index + 13 + downtimeCount, index + 3);
                worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00";
                for (int i = 0; i <= dayCount; i++)
                {
                    var Fixtures = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Fixtures));
                    worksheet.Cells[index + 2, rowIndex + 3 + i].Value = Fixtures.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var POR = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.POR));
                    worksheet.Cells[index + 3, rowIndex + 3 + i].Value = POR.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var ActualCT = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ActualCT));
                    worksheet.Cells[index + 4, rowIndex + 3 + i].Value = ActualCT.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("f2");
                    var CTAchivementRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.CTAchivementRate));
                    //worksheet.Cells[index + 5, rowIndex + 3 + i].Value = CTAchivementRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");
                    worksheet.Cells[index + 5, rowIndex + 3 + i].Formula = string.Format($"=IF(ISERROR({NunberToChar(rowIndex + 3 + i)}{index + 3}/{NunberToChar(rowIndex + 3 + i)}{index + 4}),{""},{NunberToChar(rowIndex + 3 + i)}{index + 3}/{NunberToChar(rowIndex + 3 + i)}{index + 4})");
                    worksheet.Cells[index + 5, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

                    var TotalAvailableHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour));
                    worksheet.Cells[index + 6, rowIndex + 3 + i].Value = TotalAvailableHour.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var PlannedHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedHour));
                    worksheet.Cells[index + 7, rowIndex + 3 + i].Value = PlannedHour.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var PlannedMinute = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute));
                    //worksheet.Cells[index + 8, rowIndex + 3 + i].Value = PlannedMinute.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    worksheet.Cells[index + 8, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 7}*60");

                    var PlannedOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedOutput));
                    //worksheet.Cells[index + 9, rowIndex + 3 + i].Value = PlannedOutput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    worksheet.Cells[index + 9, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 8}*60/{NunberToChar(rowIndex + 3 + i)}{index + 3}");

                    //Uptime（minute） 正常运行时间（分钟）
                    //var plannedMinuteCount = double.Parse(worksheet.Cells[index + 8, rowIndex + 3 + i].Value.ToString());
                    //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Value = plannedMinuteCount - downtimeGroup.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

                    if (downtimeCount == 0)
                    {
                        worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 8}-sum(0)");
                        //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";
                    }
                    else
                    {
                        worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 8}-sum({NunberToChar(rowIndex + 3 + i)}{index + 10}:{NunberToChar(rowIndex + 3 + i)}{index + 9 + downtimeCount})");
                        //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";
                    }

                    //Downtime breakdown
                    var RunningCapacity = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.RunningCapacity));
                    worksheet.Cells[index + 11 + downtimeCount, rowIndex + 3 + i].Value = RunningCapacity.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

                    var Throughput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Throughput));
                    worksheet.Cells[index + 12 + downtimeCount, rowIndex + 3 + i].Value = Throughput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var GoodPartOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput));
                    worksheet.Cells[index + 13 + downtimeCount, rowIndex + 3 + i].Value = GoodPartOutput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
                    var NGQTY = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.NGQTY));
                    //worksheet.Cells[index + 14 + downtimeCount, rowIndex + 3 + i].Value = NGQTY.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

                    if (firstYielCount == 0)
                    {
                        worksheet.Cells[index + 14 + downtimeCount, rowIndex + 3 + i].Formula = string.Format("=sum(0)");
                    }
                    else
                    {
                        worksheet.Cells[index + 14 + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"=sum({NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount}:{NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount + firstYielCount - 1})");
                    }


                    //First pass yield % 通过率
                    //var GoodPartOutputCount = double.Parse(worksheet.Cells[index + 13 + downtimeCount, rowIndex + 3 + i].Value.ToString());
                    //var ThroughputCount = double.Parse(worksheet.Cells[index + 12 + downtimeCount, rowIndex + 3 + i].Value.ToString());
                    //worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 3 + i].Value = ThroughputCount == 0 ? 0.ToString("p2") : (GoodPartOutputCount / ThroughputCount).ToString("p2");

                    worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 13 + downtimeCount}/{NunberToChar(rowIndex + 3 + i)}{index + 12 + downtimeCount}", index + 13 + downtimeCount, index + 12 + downtimeCount);
                    worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

                    //AvailableRate
                    //var AvailableRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.AvailableRate));
                    //worksheet.Cells[index + 16 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = AvailableRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

                    worksheet.Cells[index + 16 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 11 + downtimeCount}/{NunberToChar(rowIndex + 3 + i)}{index + 9}");
                    worksheet.Cells[index + 16 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

                    //var PerformanceRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PerformanceRate));
                    //worksheet.Cells[index + 17 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = PerformanceRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

                    worksheet.Cells[index + 17 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 12 + downtimeCount}/{NunberToChar(rowIndex + 3 + i)}{ index + 11 + downtimeCount}");
                    worksheet.Cells[index + 17 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

                    //var EquipmentEfficiency = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency));
                    //worksheet.Cells[index + 18 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = EquipmentEfficiency.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

                    worksheet.Cells[index + 18 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 5}*{NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount + firstYielCount}*{NunberToChar(rowIndex + 3 + i)}{index + 16 + downtimeCount + firstYielCount}");
                    worksheet.Cells[index + 18 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

                    //var AllEquipmentEfficiency = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency));
                    //worksheet.Cells[index + 19 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = AllEquipmentEfficiency.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

                    worksheet.Cells[index + 19 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 17 + downtimeCount + firstYielCount}*{NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount + firstYielCount}*{NunberToChar(rowIndex + 3 + i)}{index + 16 + downtimeCount + firstYielCount}");
                    worksheet.Cells[index + 19 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

                    //var ProductionTimeLoss = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss));
                    //worksheet.Cells[index + 20 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = ProductionTimeLoss.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("f2");

                    worksheet.Cells[index + 20 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"=({NunberToChar(rowIndex + 3 + i)}{index + 9}-{NunberToChar(rowIndex + 3 + i)}{index + 13 + downtimeCount})*{NunberToChar(rowIndex + 3 + i)}{index + 3}/3600");
                    worksheet.Cells[index + 20 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00";
                }

                #endregion
                //设置背景颜色
                var maxRow = downtimeCount + firstYielCount + 20 + index;
                var colorRange = string.Format("D{0}:D{1}", index + 1, maxRow);
                worksheet.Cells[string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 5), index + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 5), index + 1)].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);

                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 3, NunberToChar(dayCount + 5), index + 3), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 5, NunberToChar(dayCount + 5), index + 5), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 6, NunberToChar(dayCount + 5), index + 6), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 8, NunberToChar(dayCount + 5), index + 8), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 9, NunberToChar(dayCount + 5), index + 9), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 10, NunberToChar(dayCount + 5), index + downtimeCount + 10), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 11, NunberToChar(dayCount + 5), index + downtimeCount + 11), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 13, NunberToChar(dayCount + 5), index + downtimeCount + 13), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 14, NunberToChar(dayCount + 5), index + downtimeCount + 14), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 15, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 15), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 16, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 16), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 17, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 17), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 18, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 18), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 19, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 19), Color.LightSteelBlue);
                SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 20, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 20), Color.LightSteelBlue);

                worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Green);

                worksheet.Cells[colorRangeFirst].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[colorRangeFirst].Style.Fill.BackgroundColor.SetColor(Color.DeepSkyBlue);
                worksheet.Cells["C2,C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C2,C5"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);

                worksheet.Cells[index + 10, 2].Value = "Downtime Breakdown";
                worksheet.Cells[index + 10, 2].Style.Font.Size = 14;
                worksheet.Cells[index + 10, 2].Style.TextRotation = 90;
                worksheet.Cells[string.Format("B{0}:B{1}", index + 10, downtimeCount + index + 10)].Merge = true;

                worksheet.Cells[index + 15 + downtimeCount, 2].Value = "Yield Breakdown";
                worksheet.Cells[index + 15 + downtimeCount, 2].Style.TextRotation = 90;
                worksheet.Cells[index + 15 + downtimeCount, 2].Style.Font.Size = 14;
                worksheet.Cells[string.Format("B{0}:B{1}", index + 15 + downtimeCount, index + downtimeCount + firstYielCount + 15)].Merge = true;
                worksheet.Cells[string.Format("A1:A{0}", maxRow)].Merge = true;
                //设置边框
                SetExcelCellStyle(worksheet, string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 4 + 1), maxRow));
                SetExcelCellStyle(worksheet, string.Format("B{0}:B{1}", index + 10, downtimeCount + index + 10));
                SetExcelCellStyle(worksheet, string.Format("B{0}:B{1}", index + 15 + downtimeCount, index + downtimeCount + firstYielCount + 15));
                worksheet.Cells.AutoFitColumns();
                worksheet.Column(1).SetTrueColumnWidth(2);
                worksheet.Column(2).SetTrueColumnWidth(4);
                worksheet.Row(downtimeCount + index + 10).Height = 130;
                worksheet.Row(index + downtimeCount + firstYielCount + 15).Height = 130;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }



        /// <summary>
        /// 导出 Bu Template
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult ExportBuTemplateReport(OEE_ReprortSearchModel searchParmm)
        {
            searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrlMetrics = string.Format("OEE/GetButemplateDataListAPI");
            HttpResponseMessage responMessageMetrics = APIHelper.APIPostAsync(searchParmm, apiUrlMetrics);
            var resultMetrics = responMessageMetrics.Content.ReadAsStringAsync().Result;
            var apiUrlFirstYield = string.Format("OEE/GetFirstYieldAPI");
            HttpResponseMessage responMessageFirstYield = APIHelper.APIPostAsync(searchParmm, apiUrlFirstYield);
            var resultFirstYield = responMessageFirstYield.Content.ReadAsStringAsync().Result;
            var apiUrlDowntime = string.Format("OEE/GetDowntimeBreakdownAPI");
            HttpResponseMessage responMessageDowntime = APIHelper.APIPostAsync(searchParmm, apiUrlDowntime);

            var resultDowntime = responMessageDowntime.Content.ReadAsStringAsync().Result;
            var apiUrlStationStatus = string.Format("OEE/GetAllStationMachineStatusListAPI");
            HttpResponseMessage responMessageMachineStatus = APIHelper.APIPostAsync(searchParmm, apiUrlDowntime);
            var resultMachineStatus = responMessageMachineStatus.Content.ReadAsStringAsync().Result;

            //获取当前厂区的班次信息
            var apiUrlShift = string.Format("OEE/GetShiftModelAPI?bg_uid={0}&&Plant_uid={1}", searchParmm.BG_Organization_UID, searchParmm.Plant_Organization_UID);
            HttpResponseMessage ShiftInfoRes = APIHelper.APIGetAsync(apiUrlShift);
            var resultShiftInfoRes = ShiftInfoRes.Content.ReadAsStringAsync().Result;
            var resultDowntimeList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultDowntime).ToList();
            var resultFirstYieldList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultFirstYield).ToList();
            var resultMetricsList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultMetrics).ToList();
            var resultShiftInfoResList = JsonConvert.DeserializeObject<List<ShiftBaseInfo>>(resultShiftInfoRes).ToList();
            var resultMachineStatusList = JsonConvert.DeserializeObject<List<OEE_MachineStatusDTO>>(resultMachineStatus).ToList();

            resultShiftInfoResList = resultShiftInfoResList.OrderBy(p => p.ShiftID).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("ME_OEE_Template");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //获取表头信息
            var stringHeads = GetBuTemplateReportHeads();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("ME_OEE_Template");
                worksheet.View.ShowGridLines = true;//去掉sheet的网格线
                worksheet.Row(1).Height = 35;
                for (int i = 1; i <= stringHeads.Count(); i++)
                {
                    worksheet.Cells[1, i].Value = stringHeads[i - 1];
                    worksheet.Cells[1, i].Style.WrapText = true;
                    worksheet.Cells[1, i].Style.Font.Bold = true;
                    worksheet.Cells[1, i].Style.Font.Name = "Cambria";
                    worksheet.Cells[1, i].Style.Font.Size = 11;
                    worksheet.Column(i).Width = 15;
                }

                worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
                worksheet.Cells["R1:R1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["R1:R1"].Style.Fill.BackgroundColor.SetColor(Color.Green);
                worksheet.Cells["T1:U1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["T1:U1"].Style.Fill.BackgroundColor.SetColor(Color.Green);
                worksheet.Cells["V1:W1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["V1:W1"].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
                worksheet.Cells["X1:Z1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["X1:Z1"].Style.Fill.BackgroundColor.SetColor(Color.Green);
                worksheet.Cells["Y1:Z1"].Merge = true;
                var allDayMetricsList = resultMetricsList.GroupBy(p => new
                {
                    MachineDate = p.MachineDate,
                });

                //  ShiftName = p.ShiftName,
                allDayMetricsList = allDayMetricsList.OrderByDescending(p => p.Key.MachineDate);
                var RowIndex = 2;
                var startDateMerge = 2;
                //全天的
                foreach (var DayItem in allDayMetricsList)
                {
                    //全天的所有机台
                    var fullDayMachineList = DayItem.GroupBy(p => p.OEE_MachineInfo_UID);
                    List<double> sumDailyCapacity = new List<double>();
                    List<double> sumAvailableCapacity = new List<double>();
                    List<double> sumAvailability = new List<double>();
                    List<double> sumInput = new List<double>();
                    List<double> sumPerformance = new List<double>();
                    List<double> sumEfficiency = new List<double>();
                    List<double> sumActualOutput = new List<double>();
                    List<double> sumDefectQTY = new List<double>();
                    List<double> sumYield = new List<double>();
                    List<double> sumOEE = new List<double>();
                    List<double> sumOEEAve = new List<double>();


                    foreach (var fullDayMachine in fullDayMachineList)
                    {
                        #region  当前机台全天
                        List<double> fullDayOeeValue = new List<double>();

                        foreach (var shiftInfo in resultShiftInfoResList)
                        {
                            var resMachineStatusList = resultMachineStatusList.Where(p => p.OEE_MachineInfo_UID == fullDayMachine.Key && p.ShiftTimeID == shiftInfo.ShiftID).ToList();
                            var flag = true;
                            if (resMachineStatusList.Count == 0 || resMachineStatusList == null)
                            {
                                flag = false;
                            }

                            var currentShiftInfo = fullDayMachine.Where(p => p.ShiftName == shiftInfo.ShiftName).ToList();
                            worksheet.Cells[RowIndex, 1].Value = DayItem.Key.MachineDate.ToString("yyyy-MM-dd");
                            worksheet.Cells[RowIndex, 2].Value = searchParmm.Param_LineName;
                            worksheet.Cells[RowIndex, 3].Value = searchParmm.Param_StationName;
                            worksheet.Cells[RowIndex, 4].Value = fullDayMachine.FirstOrDefault().MachineName;
                            worksheet.Cells[RowIndex, 5].Value = shiftInfo.ShiftName;
                            if (currentShiftInfo == null || currentShiftInfo.Count() == 0)
                            {
                                #region 计算指标的值
                                //Por CT
                                worksheet.Cells[RowIndex, 6].Value = string.Empty;
                                worksheet.Cells[RowIndex, 7].Value = string.Empty;
                                worksheet.Cells[RowIndex, 8].Value = string.Empty;
                                worksheet.Cells[RowIndex, 9].Value = string.Empty;
                                //获取该机台的故障
                                worksheet.Cells[RowIndex, 10].Value = 0;
                                worksheet.Cells[RowIndex, 11].Value = 0;
                                worksheet.Cells[RowIndex, 12].Value = 0;
                                worksheet.Cells[RowIndex, 13].Value = 0;
                                // UptimeMinute
                                worksheet.Cells[RowIndex, 14].Value = 0;
                                //Ideal UPH=3600/Por CT
                                worksheet.Cells[RowIndex, 15].Value = 0;
                                //Daily Capacity=Ideal UPH*(Planned Time/60)*Actual Machine
                                worksheet.Cells[RowIndex, 16].Value = 0;
                                //Available Capacity=Ideal UPH*(Update Time/60)*Actual Machine
                                worksheet.Cells[RowIndex, 17].Value = 0;
                                //Availability%=Available Capacity/Daily Capacity
                                worksheet.Cells[RowIndex, 18].Value = 0.ToString("P2");
                                //Input
                                worksheet.Cells[RowIndex, 19].Value = 0;
                                //Performance=input/Available Capacity
                                worksheet.Cells[RowIndex, 20].Value = 0.ToString("P2");
                                //Efficiency=Availability%*performance%
                                worksheet.Cells[RowIndex, 21].Value = 0.ToString("P2");
                                //Actual Output 
                                worksheet.Cells[RowIndex, 22].Value = 0;
                                //Defect QTY
                                worksheet.Cells[RowIndex, 23].Value = 0;
                                //Yiled%=Actual Output/Input
                                worksheet.Cells[RowIndex, 24].Value = 0.ToString("P2");//performance
                                //OEE=Availability*performance*Yiled%
                                worksheet.Cells[RowIndex, 25].Value = 0.ToString("P2");
                                #endregion
                            }
                            else
                            {
                                #region 计算指标的值
                                //Por CT
                                var POR = currentShiftInfo.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.POR));
                                worksheet.Cells[RowIndex, 6].Value = POR.FirstOrDefault().IndexCount;
                                worksheet.Cells[RowIndex, 7].Value = 1;
                                worksheet.Cells[RowIndex, 8].Value = 1;
                                var PlannedMinute = currentShiftInfo.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.PlannedMinute));
                                worksheet.Cells[RowIndex, 9].Value = PlannedMinute.FirstOrDefault().IndexCount;
                                //获取该机台的故障 1 待料时间 2 生产时间 3 宕机时间 4 保养时间 5 调试时间 6 关机断网时间 7 吃饭时间
                                worksheet.Cells[RowIndex, 10].Value = flag ? 0 : resMachineStatusList.Where(p => p.StatusID == 3).Sum(q => q.StatusDuration);
                                worksheet.Cells[RowIndex, 11].Value = flag ? 0 : resMachineStatusList.Where(p => p.StatusID == 4).Sum(q => q.StatusDuration);
                                worksheet.Cells[RowIndex, 12].Value = flag ? 0 : resMachineStatusList.Where(p => p.StatusID == 1).Sum(q => q.StatusDuration);
                                worksheet.Cells[RowIndex, 13].Value = flag ? 0 : resMachineStatusList.Where(p => p.StatusID == 7).Sum(q => q.StatusDuration);

                                var UptimeMinute = currentShiftInfo.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.UptimeMinute));
                                // UptimeMinute
                                worksheet.Cells[RowIndex, 14].Value = UptimeMinute.FirstOrDefault().IndexCount;
                                //Ideal UPH=3600/Por CT
                                worksheet.Cells[RowIndex, 15].Value = double.Parse(worksheet.Cells[RowIndex, 6].Value.ToString()) == 0 ? "0" : (3600 / double.Parse(worksheet.Cells[RowIndex, 6].Value.ToString())).ToString("f0");
                                //Daily Capacity=Ideal UPH*(Planned Time/60)*Actual Machine
                                var Capacity = double.Parse(worksheet.Cells[RowIndex, 15].Value.ToString()) * (double.Parse(worksheet.Cells[RowIndex, 9].Value.ToString()) / 60);
                                sumDailyCapacity.Add(Capacity);
                                worksheet.Cells[RowIndex, 16].Value = (Capacity).ToString("f0");
                                //Available Capacity=Ideal UPH*(Update Time/60)*Actual Machine
                                var AvailableCapacity = double.Parse(worksheet.Cells[RowIndex, 15].Value.ToString()) * (double.Parse(worksheet.Cells[RowIndex, 14].Value.ToString()) / 60);
                                sumAvailableCapacity.Add(AvailableCapacity);
                                worksheet.Cells[RowIndex, 17].Value = (AvailableCapacity).ToString("f0");
                                var Availability = double.Parse(worksheet.Cells[RowIndex, 16].Value.ToString()) == 0 ? 0 : (double.Parse(worksheet.Cells[RowIndex, 17].Value.ToString()) / (double.Parse(worksheet.Cells[RowIndex, 16].Value.ToString())));
                                sumAvailability.Add(Availability);
                                //Availability%=Available Capacity/Daily Capacity
                                worksheet.Cells[RowIndex, 18].Value = Availability.ToString("P2");
                                var Throughput = currentShiftInfo.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.Throughput));
                                var NGQTY = currentShiftInfo.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.NGQTY));
                                //Input
                                var input = double.Parse(Throughput.FirstOrDefault().IndexCount) + double.Parse(NGQTY.FirstOrDefault().IndexCount);
                                sumInput.Add(input);
                                worksheet.Cells[RowIndex, 19].Value = input;
                                var performance = double.Parse(worksheet.Cells[RowIndex, 17].Value.ToString()) == 0 ? 0 : ((double.Parse(worksheet.Cells[RowIndex, 19].Value.ToString()) / (double.Parse(worksheet.Cells[RowIndex, 17].Value.ToString()))));
                                sumPerformance.Add(performance);
                                //Performance=input/Available Capacity
                                worksheet.Cells[RowIndex, 20].Value = performance.ToString("P2");
                                //Efficiency=Availability%*performance%
                                var efficiency = (performance) * (Availability);
                                sumEfficiency.Add(efficiency);
                                worksheet.Cells[RowIndex, 21].Value = efficiency.ToString("P2");
                                //Actual Output 
                                sumActualOutput.Add(double.Parse(Throughput.FirstOrDefault().IndexCount));
                                worksheet.Cells[RowIndex, 22].Value = Throughput.FirstOrDefault().IndexCount;
                                //Defect QTY
                                sumDefectQTY.Add(double.Parse(NGQTY.FirstOrDefault().IndexCount));
                                worksheet.Cells[RowIndex, 23].Value = NGQTY.FirstOrDefault().IndexCount;
                                //Yiled%=Actual Output/Input
                                var yield = double.Parse(worksheet.Cells[RowIndex, 19].Value.ToString()) == 0 ? 0 : ((double.Parse(worksheet.Cells[RowIndex, 22].Value.ToString()) / (double.Parse(worksheet.Cells[RowIndex, 19].Value.ToString()))));
                                sumYield.Add(yield);
                                worksheet.Cells[RowIndex, 24].Value = yield.ToString("P2");//performance
                                //OEE=Availability*performance*Yiled%
                                sumOEE.Add((Availability * (performance) * (yield)));
                                worksheet.Cells[RowIndex, 25].Value = ((Availability * (performance) * (yield))).ToString("P2");
                                fullDayOeeValue.Add(Availability * (performance) * (yield));
                                #endregion
                            }
                            //全天的OEE 值
                            RowIndex++;
                        }

                        sumOEEAve.Add(fullDayOeeValue.Average());
                        worksheet.Cells[string.Format("Z{0}:Z{1}", RowIndex - 2, RowIndex - 1)].Merge = true;
                        worksheet.Cells[string.Format("B{0}:B{1}", RowIndex - 2, RowIndex - 1)].Merge = true;
                        worksheet.Cells[string.Format("C{0}:C{1}", RowIndex - 2, RowIndex - 1)].Merge = true;
                        worksheet.Cells[string.Format("D{0}:D{1}", RowIndex - 2, RowIndex - 1)].Merge = true;
                        worksheet.Cells[RowIndex - 2, 26].Value = fullDayOeeValue.Average().ToString("p2");
                        #endregion
                    }
                    worksheet.Cells[RowIndex, 15].Value = "Sum";
                    worksheet.Cells[RowIndex, 16].Value = sumDailyCapacity.Sum().ToString("f0");
                    worksheet.Cells[RowIndex, 17].Value = sumAvailableCapacity.Sum().ToString("f0");
                    worksheet.Cells[RowIndex, 18].Value = sumAvailability.Average().ToString("p2");
                    worksheet.Cells[RowIndex, 19].Value = sumInput.Sum().ToString("f0");
                    worksheet.Cells[RowIndex, 20].Value = sumPerformance.Average().ToString("p2");
                    worksheet.Cells[RowIndex, 21].Value = sumEfficiency.Average().ToString("p2");
                    worksheet.Cells[RowIndex, 22].Value = sumActualOutput.Sum().ToString("f0");
                    worksheet.Cells[RowIndex, 23].Value = sumDefectQTY.Sum().ToString("f0");
                    worksheet.Cells[RowIndex, 24].Value = sumYield.Average().ToString("p2");
                    worksheet.Cells[RowIndex, 25].Value = sumOEE.Average().ToString("p2");
                    worksheet.Cells[RowIndex, 26].Value = sumOEEAve.Average().ToString("p2");
                    worksheet.Cells[string.Format("A{0}:A{1}", startDateMerge, RowIndex)].Merge = true;
                    worksheet.Cells[string.Format("O{0}:Z{0}", RowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[string.Format("O{0}:Z{0}", RowIndex)].Style.Fill.BackgroundColor.SetColor(Color.YellowGreen);

                    RowIndex++;
                    startDateMerge = RowIndex;
                }

                SetExcelCellStyle(worksheet, string.Format("A1:Z{0}", RowIndex));
                worksheet.Cells[string.Format("A1:A{0}", RowIndex)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        /// <summary>
        /// 导出工站Metrics报表
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult ExportAllMachineMetrics(OEE_ReprortSearchModel searchParmm)
        {
            TestExport(searchParmm);
            //获取工站下面的所有机台
            var apiUrlMachine = string.Format("OEE/GetAllByStationIDAPI?stationUID={0}", searchParmm.StationID);
            HttpResponseMessage responMachineMessage = APIHelper.APIGetAsync(apiUrlMachine);
            var resultMachineList = responMachineMessage.Content.ReadAsStringAsync().Result;
            var MachineModelList = JsonConvert.DeserializeObject<List<OEE_MachineInfoDTO>>(resultMachineList).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("PIS_OEEMetrics_Machine");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //using (var excelPackage = new ExcelPackage(stream))
            //{
            //    #region excelValue
            //    foreach (var item in MachineModelList)
            //    {
            //        searchParmm.EQP_Uid = item.OEE_MachineInfo_UID;
            //        searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            //        var apiUrlMetrics = string.Format("OEE/GETOEEMetricsAPI");
            //        HttpResponseMessage responMessageMetrics = APIHelper.APIPostAsync(searchParmm, apiUrlMetrics);
            //        var resultMetrics = responMessageMetrics.Content.ReadAsStringAsync().Result;
            //        var apiUrlFirstYield = string.Format("OEE/GetFirstYieldAPI");
            //        HttpResponseMessage responMessageFirstYield = APIHelper.APIPostAsync(searchParmm, apiUrlFirstYield);
            //        var resultFirstYield = responMessageFirstYield.Content.ReadAsStringAsync().Result;
            //        var apiUrlDowntime = string.Format("OEE/GetDowntimeBreakdownAPI");
            //        HttpResponseMessage responMessageDowntime = APIHelper.APIPostAsync(searchParmm, apiUrlDowntime);
            //        var resultDowntime = responMessageDowntime.Content.ReadAsStringAsync().Result;

            //        var resultDowntimeList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultDowntime).ToList();
            //        var resultFirstYieldList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultFirstYield).ToList();
            //        var resultMetricsList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultMetrics).ToList();

            //        var stringHeads = GetMetricsHeads(searchParmm);

            //        var index = 6;
            //        var worksheet = excelPackage.Workbook.Worksheets.Add(item.MachineNo);
            //        var countIndex = 0;
            //        var rowIndexHead = 2;
            //        worksheet.View.ShowGridLines = false;

            //        var apiUrl = string.Format("OEE/GetAllByEQPIDAPI?eqpuid={0}", searchParmm.EQP_Uid);
            //        HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            //        var resultMachine = responMessage.Content.ReadAsStringAsync().Result;
            //        var resultMachineModel = JsonConvert.DeserializeObject<OEE_MachineInfoDTO>(resultMachine);
            //        worksheet.Cells[countIndex + 2, rowIndexHead + 1].Value = "Machine ID 机器号";
            //        worksheet.Cells[countIndex + 2, rowIndexHead + 2].Value = resultMachineModel == null ? string.Empty : resultMachineModel.EQP_EMTSerialNum;
            //        worksheet.Cells[countIndex + 3, rowIndexHead + 1].Value = "Machine Name 机器名称";
            //        worksheet.Cells[countIndex + 3, rowIndexHead + 2].Value = resultMachineModel == null ? string.Empty : resultMachineModel.MachineNo;
            //        worksheet.Cells[countIndex + 4, rowIndexHead + 1].Value = "Bay # 线号";
            //        worksheet.Cells[countIndex + 4, rowIndexHead + 2].Value = searchParmm.Param_LineName;
            //        worksheet.Cells[countIndex + 5, rowIndexHead + 1].Value = "Station 工位";
            //        worksheet.Cells[countIndex + 5, rowIndexHead + 2].Value = searchParmm.Param_StationName;
            //        var colorRangeFirst = "C2:E5";

            //        worksheet.Cells["D2:E2"].Merge = true;
            //        worksheet.Cells["D3:E3"].Merge = true;
            //        worksheet.Cells["D4:E4"].Merge = true;
            //        worksheet.Cells["D5:E5"].Merge = true;

            //        worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //        worksheet.Cells[colorRangeFirst].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //        worksheet.Cells[colorRangeFirst].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //        worksheet.Cells[colorRangeFirst].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //        worksheet.Cells[colorRangeFirst].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            //        for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
            //        {
            //            worksheet.Cells[index + 1, colIndex + 3].Value = stringHeads[colIndex];
            //        }
            //        #region
            //        var downtimeGroup = resultDowntimeList.GroupBy(p => p.IndexName).ToList();
            //        var firstYielGroup = resultFirstYieldList.GroupBy(p => p.IndexName).ToList();
            //        var metricsGroup = resultMetricsList.GroupBy(p => p.IndexName).ToList();

            //        var dayCount = (searchParmm.EndTime - searchParmm.StartTime).Days;
            //        var rowIndex = 2;
            //        worksheet.Cells[index + 2, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.Fixtures);
            //        worksheet.Cells[index + 2, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Fixtures)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        worksheet.Cells[index + 3, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.POR);
            //        var EntireBuildPOR = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.POR)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        worksheet.Cells[index + 3, rowIndex + 2].Value = EntireBuildPOR;
            //        worksheet.Cells[index + 4, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.ActualCT);
            //        var EntireBuildActualCT = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ActualCT)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        worksheet.Cells[index + 4, rowIndex + 2].Value = EntireBuildActualCT;
            //        worksheet.Cells[index + 5, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.CTAchivementRate);
            //        //worksheet.Cells[index + 5, rowIndex + 2].Value = EntireBuildActualCT == 0 ? 0.ToString("P2") : (EntireBuildPOR / EntireBuildActualCT).ToString("P2");
            //        worksheet.Cells[index + 5, rowIndex + 2].Formula = string.Format("=IF(ISERROR(D{0}/D{1}),{2},D{0}/D{1})", index + 3, index + 4, "");
            //        worksheet.Cells[index + 5, rowIndex + 2].Style.Numberformat.Format = "0.00%";

            //        worksheet.Cells[index + 6, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour);
            //        var EntireBuildTotalAvailableHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        worksheet.Cells[index + 6, rowIndex + 2].Value = EntireBuildTotalAvailableHour;
            //        worksheet.Cells[index + 7, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedHour);
            //        var EntireBuildPlannedHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedHour)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        worksheet.Cells[index + 7, rowIndex + 2].Value = EntireBuildPlannedHour;
            //        worksheet.Cells[index + 8, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedMinute);
            //        var EntireBuildPlannedMinute = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        //计划时间 PlannedMinute
            //        worksheet.Cells[index + 8, rowIndex + 2].Formula = string.Format("=D{0}*60", index + 7);
            //        worksheet.Cells[index + 9, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PlannedOutput);
            //        var EntireBuildPlannedOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedOutput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        //worksheet.Cells[index + 9, rowIndex + 2].Value = EntireBuildPlannedOutput;

            //        //计划产出 PlannedOutputSS
            //        worksheet.Cells[index + 9, rowIndex + 2].Formula = string.Format("=D{0}*60/D{1}", index + 8, index + 3);
            //        var downtimeCount = downtimeGroup.Count();
            //        for (int i = 0; i < downtimeCount; i++)
            //        {
            //            worksheet.Cells[index + 10 + i, rowIndex + 1].Value = downtimeGroup[i].Key;
            //            worksheet.Cells[index + 10 + i, rowIndex + 2].Value = downtimeGroup[i].Sum(p => double.Parse(p.IndexCount));

            //            for (int j = 0; j <= dayCount; j++)
            //            {
            //                worksheet.Cells[index + 10 + i, rowIndex + 3 + j].Value = downtimeGroup[i].Where(p => p.MachineDate == searchParmm.StartTime.AddDays(j)).Sum(q => double.Parse(q.IndexCount));
            //            }
            //        }

            //        //Uptime（minute） 正常运行时间（分钟）
            //        worksheet.Cells[index + 10 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(DowntimeBreakdownEnum.UptimeMinute);
            //        //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).Sum(p => p.Sum(q => double.Parse(q.IndexCount))) - downtimeGroup.Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        if (downtimeCount == 0)
            //        {
            //            worksheet.Cells[index + 10 + downtimeCount, rowIndex + 2].Formula = string.Format("=D{0}-sum(0)", index + 8);
            //        }
            //        else
            //        {
            //            worksheet.Cells[index + 10 + downtimeCount, rowIndex + 2].Formula = string.Format("=D{0}-sum(D{1}:D{2})", index + 8, index + 10, index + 9 + downtimeCount);
            //        }

            //        //Running Capacity  运行生产能力
            //        worksheet.Cells[index + 11 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.RunningCapacity);
            //        var EntireBuildRunningCapacity = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.RunningCapacity)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        //worksheet.Cells[index + 11 + downtimeCount, rowIndex + 2].Value = EntireBuildRunningCapacity;
            //        worksheet.Cells[index + 11 + downtimeCount, rowIndex + 2].Formula = string.Format("=60/D{0}*D{1}", index + 3, index + 10 + downtimeCount);

            //        //Throughput 生产量
            //        worksheet.Cells[index + 12 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.Throughput);
            //        var EntireBuildThroughput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Throughput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        worksheet.Cells[index + 12 + downtimeCount, rowIndex + 2].Value = EntireBuildThroughput;

            //        var firstYielCount = firstYielGroup.Count();

            //        //Good Part Output 良品数量
            //        worksheet.Cells[index + 13 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.GoodPartOutput);
            //        worksheet.Cells[index + 13 + downtimeCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        var ThroughputTotalCount = double.Parse(worksheet.Cells[index + 12 + downtimeCount, rowIndex + 2].Value.ToString());
            //        var GoodPartOutputTotalCount = double.Parse(worksheet.Cells[index + 13 + downtimeCount, rowIndex + 2].Value.ToString());

            //        // NG QTY 不良品数量
            //        worksheet.Cells[index + 14 + downtimeCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.NGQTY);
            //        var EntireBuildNGQTY = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.NGQTY)).Sum(p => p.Sum(q => double.Parse(q.IndexCount)));
            //        //worksheet.Cells[index + 14 + downtimeCount, rowIndex + 2].Value = EntireBuildNGQTY;
            //        if (firstYielCount == 0)
            //        {
            //            worksheet.Cells[index + 14 + downtimeCount, rowIndex + 2].Formula = string.Format("=sum(0)");
            //        }
            //        else
            //        {
            //            worksheet.Cells[index + 14 + downtimeCount, rowIndex + 2].Formula = string.Format("=sum(D{0}:D{1})", index + 15 + downtimeCount, index + 15 + downtimeCount + firstYielCount - 1);
            //        }

            //        //First pass yield
            //        for (int i = 0; i < firstYielCount; i++)
            //        {
            //            worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 1].Value = firstYielGroup[i].Key;
            //            worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 2].Value = firstYielGroup[i].Sum(p => double.Parse(p.IndexCount));

            //            for (int j = 0; j <= dayCount; j++)
            //            {
            //                worksheet.Cells[index + 15 + downtimeCount + i, rowIndex + 3 + j].Value = firstYielGroup[i].Where(p => p.MachineDate == searchParmm.StartTime.AddDays(j)).Sum(q => double.Parse(q.IndexCount));
            //            }
            //        }

            //        //First pass yield % 通过率
            //        worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(DowntimeBreakdownEnum.FirstPassYield);
            //        //worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildThroughput == 0 ? 0.ToString("p2") : ((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput).ToString("p2");
            //        worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}/D{1}", index + 13 + downtimeCount, index + 12 + downtimeCount);
            //        worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
            //        //AV% 设备时间开动率
            //        worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.AvailableRate);
            //        //worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildPlannedOutput == 0 ? 0.ToString("p2") : (EntireBuildRunningCapacity / EntireBuildPlannedOutput).ToString("p2");
            //        worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}/D{1}", index + 11 + downtimeCount, index + 9);
            //        worksheet.Cells[index + 16 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
            //        //PF% 性能开动
            //        worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.PerformanceRate);
            //        //worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 2].Value = EntireBuildRunningCapacity == 0 ? 0.ToString("p2") : (EntireBuildThroughput / EntireBuildRunningCapacity).ToString("p2");
            //        worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}/D{1}", index + 12 + downtimeCount, index + 11 + downtimeCount);
            //        worksheet.Cells[index + 17 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
            //        //OEE% (w/o Microstop) 整体设备效率 （不包含微停）
            //        worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency);
            //        //worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 2].Value = (EntireBuildActualCT == 0 || EntireBuildPlannedOutput == 0 || EntireBuildThroughput == 0) ? 0.ToString("p2") : ((EntireBuildPOR / EntireBuildActualCT) * (EntireBuildRunningCapacity / EntireBuildPlannedOutput) * (((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput))).ToString("p2");
            //        worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}*D{1}*D{2}", index + 5, index + 15 + downtimeCount + firstYielCount, index + 16 + downtimeCount + firstYielCount);
            //        worksheet.Cells[index + 18 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
            //        //OEE% 整体设备效率
            //        worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency);
            //        //worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 2].Value = (EntireBuildThroughput == 0 || EntireBuildPlannedOutput == 0 || EntireBuildRunningCapacity == 0) ? 0.ToString("p2") : (((EntireBuildThroughput - EntireBuildNGQTY) / EntireBuildThroughput) * (EntireBuildThroughput / EntireBuildPlannedOutput)).ToString("p2");
            //        worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=D{0}*D{1}*D{2}", index + 17 + downtimeCount + firstYielCount, index + 15 + downtimeCount + firstYielCount, index + 16 + downtimeCount + firstYielCount);
            //        worksheet.Cells[index + 19 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00%";
            //        //Production time loss (Hour) 生产损失（小时）
            //        worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 1].Value = EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss);
            //        //worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 2].Value = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss)).Sum(p => p.Sum(q => double.Parse(q.IndexCount))).ToString("f2");
            //        worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 2].Formula = string.Format("=(D{0}-D{1})*D{2}/3600", index + 9, index + 13 + downtimeCount, index + 3);
            //        worksheet.Cells[index + 20 + downtimeCount + firstYielCount, rowIndex + 2].Style.Numberformat.Format = "0.00";
            //        for (int i = 0; i <= dayCount; i++)
            //        {
            //            var Fixtures = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Fixtures));
            //            worksheet.Cells[index + 2, rowIndex + 3 + i].Value = Fixtures.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            var POR = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.POR));
            //            worksheet.Cells[index + 3, rowIndex + 3 + i].Value = POR.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            var ActualCT = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ActualCT));
            //            worksheet.Cells[index + 4, rowIndex + 3 + i].Value = ActualCT.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("f2");
            //            var CTAchivementRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.CTAchivementRate));
            //            //worksheet.Cells[index + 5, rowIndex + 3 + i].Value = CTAchivementRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");
            //            worksheet.Cells[index + 5, rowIndex + 3 + i].Formula = string.Format($"=IF(ISERROR({NunberToChar(rowIndex + 3 + i)}{index + 3}/{NunberToChar(rowIndex + 3 + i)}{index + 4}),{""},{NunberToChar(rowIndex + 3 + i)}{index + 3}/{NunberToChar(rowIndex + 3 + i)}{index + 4})");
            //            worksheet.Cells[index + 5, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

            //            var TotalAvailableHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.TotalAvailableHour));
            //            worksheet.Cells[index + 6, rowIndex + 3 + i].Value = TotalAvailableHour.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            var PlannedHour = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedHour));
            //            worksheet.Cells[index + 7, rowIndex + 3 + i].Value = PlannedHour.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            var PlannedMinute = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedMinute));
            //            //worksheet.Cells[index + 8, rowIndex + 3 + i].Value = PlannedMinute.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            worksheet.Cells[index + 8, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 7}*60");

            //            var PlannedOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PlannedOutput));
            //            //worksheet.Cells[index + 9, rowIndex + 3 + i].Value = PlannedOutput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            worksheet.Cells[index + 9, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 8}*60/{NunberToChar(rowIndex + 3 + i)}{index + 3}");

            //            //Uptime（minute） 正常运行时间（分钟）
            //            //var plannedMinuteCount = double.Parse(worksheet.Cells[index + 8, rowIndex + 3 + i].Value.ToString());
            //            //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Value = plannedMinuteCount - downtimeGroup.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

            //            if (downtimeCount == 0)
            //            {
            //                worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 8}-sum(0)");
            //                //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";
            //            }
            //            else
            //            {
            //                worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 8}-sum({NunberToChar(rowIndex + 3 + i)}{index + 10}:{NunberToChar(rowIndex + 3 + i)}{index + 9 + downtimeCount})");
            //                //worksheet.Cells[index + 10 + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";
            //            }

            //            //Downtime breakdown
            //            var RunningCapacity = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.RunningCapacity));
            //            worksheet.Cells[index + 11 + downtimeCount, rowIndex + 3 + i].Value = RunningCapacity.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

            //            var Throughput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.Throughput));
            //            worksheet.Cells[index + 12 + downtimeCount, rowIndex + 3 + i].Value = Throughput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            var GoodPartOutput = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput));
            //            worksheet.Cells[index + 13 + downtimeCount, rowIndex + 3 + i].Value = GoodPartOutput.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));
            //            var NGQTY = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.NGQTY));
            //            //worksheet.Cells[index + 14 + downtimeCount, rowIndex + 3 + i].Value = NGQTY.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount)));

            //            if (firstYielCount == 0)
            //            {
            //                worksheet.Cells[index + 14 + downtimeCount, rowIndex + 3 + i].Formula = string.Format("=sum(0)");
            //            }
            //            else
            //            {
            //                worksheet.Cells[index + 14 + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"=sum({NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount}:{NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount + firstYielCount - 1})");
            //            }


            //            //First pass yield % 通过率
            //            //var GoodPartOutputCount = double.Parse(worksheet.Cells[index + 13 + downtimeCount, rowIndex + 3 + i].Value.ToString());
            //            //var ThroughputCount = double.Parse(worksheet.Cells[index + 12 + downtimeCount, rowIndex + 3 + i].Value.ToString());
            //            //worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 3 + i].Value = ThroughputCount == 0 ? 0.ToString("p2") : (GoodPartOutputCount / ThroughputCount).ToString("p2");

            //            worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 13 + downtimeCount}/{NunberToChar(rowIndex + 3 + i)}{index + 12 + downtimeCount}", index + 13 + downtimeCount, index + 12 + downtimeCount);
            //            worksheet.Cells[index + 15 + downtimeCount + firstYielCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

            //            //AvailableRate
            //            //var AvailableRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.AvailableRate));
            //            //worksheet.Cells[index + 16 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = AvailableRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

            //            worksheet.Cells[index + 16 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 11 + downtimeCount}/{NunberToChar(rowIndex + 3 + i)}{index + 9}");
            //            worksheet.Cells[index + 16 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

            //            //var PerformanceRate = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.PerformanceRate));
            //            //worksheet.Cells[index + 17 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = PerformanceRate.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

            //            worksheet.Cells[index + 17 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{index + 12 + downtimeCount}/{NunberToChar(rowIndex + 3 + i)}{ index + 11 + downtimeCount}");
            //            worksheet.Cells[index + 17 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

            //            //var EquipmentEfficiency = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.EquipmentEfficiency));
            //            //worksheet.Cells[index + 18 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = EquipmentEfficiency.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

            //            worksheet.Cells[index + 18 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 5}*{NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount + firstYielCount}*{NunberToChar(rowIndex + 3 + i)}{index + 16 + downtimeCount + firstYielCount}");
            //            worksheet.Cells[index + 18 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

            //            //var AllEquipmentEfficiency = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency));
            //            //worksheet.Cells[index + 19 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = AllEquipmentEfficiency.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("p2");

            //            worksheet.Cells[index + 19 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"={NunberToChar(rowIndex + 3 + i)}{ index + 17 + downtimeCount + firstYielCount}*{NunberToChar(rowIndex + 3 + i)}{index + 15 + downtimeCount + firstYielCount}*{NunberToChar(rowIndex + 3 + i)}{index + 16 + downtimeCount + firstYielCount}");
            //            worksheet.Cells[index + 19 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00%";

            //            //var ProductionTimeLoss = metricsGroup.Where(p => p.Key == EnumHelper.GetDescription(MachineIndexName.ProductionTimeLoss));
            //            //worksheet.Cells[index + 20 + firstYielCount + downtimeCount, rowIndex + 3 + i].Value = ProductionTimeLoss.Select(p => p.Where(q => q.MachineDate == searchParmm.StartTime.AddDays(i))).Sum(m => m.Sum(r => double.Parse(r.IndexCount))).ToString("f2");

            //            worksheet.Cells[index + 20 + firstYielCount + downtimeCount, rowIndex + 3 + i].Formula = string.Format($"=({NunberToChar(rowIndex + 3 + i)}{index + 9}-{NunberToChar(rowIndex + 3 + i)}{index + 13 + downtimeCount})*{NunberToChar(rowIndex + 3 + i)}{index + 3}/3600");
            //            worksheet.Cells[index + 20 + firstYielCount + downtimeCount, rowIndex + 3 + i].Style.Numberformat.Format = "0.00";
            //        }

            //        #endregion
            //        //设置背景颜色
            //        var maxRow = downtimeCount + firstYielCount + 20 + index;
            //        var colorRange = string.Format("D{0}:D{1}", index + 1, maxRow);
            //        worksheet.Cells[string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 5), index + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        worksheet.Cells[string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 5), index + 1)].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);

            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 3, NunberToChar(dayCount + 5), index + 3), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 5, NunberToChar(dayCount + 5), index + 5), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 6, NunberToChar(dayCount + 5), index + 6), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 8, NunberToChar(dayCount + 5), index + 8), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + 9, NunberToChar(dayCount + 5), index + 9), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 10, NunberToChar(dayCount + 5), index + downtimeCount + 10), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 11, NunberToChar(dayCount + 5), index + downtimeCount + 11), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 13, NunberToChar(dayCount + 5), index + downtimeCount + 13), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + 14, NunberToChar(dayCount + 5), index + downtimeCount + 14), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 15, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 15), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 16, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 16), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 17, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 17), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 18, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 18), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 19, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 19), Color.LightSteelBlue);
            //        SetRowColor(worksheet, string.Format("C{0}:{1}{2}", index + downtimeCount + firstYielCount + 20, NunberToChar(dayCount + 5), index + downtimeCount + firstYielCount + 20), Color.LightSteelBlue);

            //        worksheet.Cells[colorRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        worksheet.Cells[colorRange].Style.Fill.BackgroundColor.SetColor(Color.Green);

            //        worksheet.Cells[colorRangeFirst].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        worksheet.Cells[colorRangeFirst].Style.Fill.BackgroundColor.SetColor(Color.DeepSkyBlue);
            //        worksheet.Cells["C2,C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        worksheet.Cells["C2,C5"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);

            //        worksheet.Cells[index + 10, 2].Value = "Downtime Breakdown";
            //        worksheet.Cells[index + 10, 2].Style.Font.Size = 14;
            //        worksheet.Cells[index + 10, 2].Style.TextRotation = 90;
            //        worksheet.Cells[string.Format("B{0}:B{1}", index + 10, downtimeCount + index + 10)].Merge = true;

            //        worksheet.Cells[index + 15 + downtimeCount, 2].Value = "Yield Breakdown";
            //        worksheet.Cells[index + 15 + downtimeCount, 2].Style.TextRotation = 90;
            //        worksheet.Cells[index + 15 + downtimeCount, 2].Style.Font.Size = 14;
            //        worksheet.Cells[string.Format("B{0}:B{1}", index + 15 + downtimeCount, index + downtimeCount + firstYielCount + 15)].Merge = true;
            //        worksheet.Cells[string.Format("A1:A{0}", maxRow)].Merge = true;
            //        //设置边框
            //        SetExcelCellStyle(worksheet, string.Format("C{0}:{1}{2}", index + 1, NunberToChar(dayCount + 4 + 1), maxRow));
            //        SetExcelCellStyle(worksheet, string.Format("B{0}:B{1}", index + 10, downtimeCount + index + 10));
            //        SetExcelCellStyle(worksheet, string.Format("B{0}:B{1}", index + 15 + downtimeCount, index + downtimeCount + firstYielCount + 15));
            //        worksheet.Cells.AutoFitColumns();
            //        worksheet.Column(1).SetTrueColumnWidth(2);
            //        worksheet.Column(2).SetTrueColumnWidth(4);
            //        worksheet.Row(downtimeCount + index + 10).Height = 130;
            //        worksheet.Row(index + downtimeCount + firstYielCount + 15).Height = 130;
            //    }
            //    #endregion
            //    excelPackage.Save();
            //}

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }

        private void SetExcelCellStyle(ExcelWorksheet worksheet, string localIndexCell)
        {
            worksheet.Cells[localIndexCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[localIndexCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="range"></param>
        /// <param name="color"></param>
        private void SetRowColor(ExcelWorksheet worksheet, string range, Color color)
        {
            worksheet.Cells[range].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[range].Style.Fill.BackgroundColor.SetColor(color);
        }

        private static string NunberToChar(int index)
        {
            //if (1 <= number && 36 >= number)
            //{
            //    int num = number + 64;
            //    System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            //    byte[] btNumber = new byte[] { (byte)num };
            //    return asciiEncoding.GetString(btNumber);
            //}
            //return "数字不在转换范围内";
            index = index - 1;
            if (index < 0) { throw new Exception("invalid parameter"); }

            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0) index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return String.Join(string.Empty, chars.ToArray());
        }

        public string[] GetMetricsHeads(OEE_ReprortSearchModel searchParmm)
        {
            var stringHeads = new string[] { "NO.", "DATE", "Entire Build" };
            var list = new List<string>();
            list.Add("DATE");
            list.Add("Entire Build");
            var dayCount = (searchParmm.EndTime - searchParmm.StartTime).Days;
            if (dayCount == 0)
            {
                list.Add(searchParmm.StartTime.ToString("yyyy-MM-dd"));
            }
            else
            {
                for (int i = 0; i <= dayCount; i++)
                {
                    list.Add(searchParmm.StartTime.AddDays(i).ToString("yyyy-MM-dd"));
                }
            }

            return list.ToArray();
        }

        public string[] GetBuTemplateReportHeads()
        {
            var TableHeads = new[]
            {
                "Date",
                "Line Name #",
                "Station",
                "Machine #（机台号）",
                "Shift",
                "POR C/T",
                "Planned machines",
                "Actual # machine  (机台数量）",
                "Planned  time (min) 計畫時間",
                "Unplanned Downtime 故障時間",
                "Preventive Maintenace 保養時間",
                 "Wait for part 待料時間",
                "Break 吃飯休息",
                "Uptime (min)",
                "Ideal UPH",
                "Daily Capacity",
                "Available Capacity",
                "Availability %",
                "Input",
                "Performance %",
                "Efficiency (AV x PF )",
                "Actual Output (good part)",
                 "Defect QTY",
                "Yield%",
                "OEE % (AVxPFxY)",
            };
            return TableHeads;
        }
        #endregion

        #region OEE图表
        /// <summary>
        /// OEE图表
        /// </summary>
        /// <returns></returns>
        public ActionResult OEE_MetricsPhotoReport(string Plant_UID, string BG_UID, string customerId, string lineName, string stationName, string MachineName, string ShiftID, string startTime, string endTime, string isFromPhotoReport)
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
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            #region  
            if (isFromPhotoReport == "PieOEE") //从统计图表跳转过来
            {
                var OEE_PieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_PieSerchParam);
                Plant_UID = OEE_PieCookie["Plant_UID"];
                BG_UID = OEE_PieCookie["BG_UID"];
                customerId = OEE_PieCookie["customerId"];
                lineName = OEE_PieCookie["lineName"];
                stationName = OEE_PieCookie["stationName"];
                MachineName = MachineName;
                ShiftID = OEE_PieCookie["ShiftID"];
                startTime = OEE_PieCookie["startTime"];
                endTime = OEE_PieCookie["startTime"];
                isFromPhotoReport = "true";
            }
            else if (isFromPhotoReport == "MetricsOEE")//Metrics 报表跳转
            {
                var OEE_PieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsSerchParam);
                Plant_UID = OEE_PieCookie["Plant_UID"];
                BG_UID = OEE_PieCookie["BG_UID"];
                customerId = OEE_PieCookie["customerId"];
                lineName = OEE_PieCookie["lineName"];
                stationName = OEE_PieCookie["stationName"];
                MachineName = OEE_PieCookie["MachineName"];
                ShiftID = OEE_PieCookie["ShiftID"];
                startTime = OEE_PieCookie["startTime"];
                endTime = OEE_PieCookie["startTime"];
                isFromPhotoReport = "true";
                //var MetricsPieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsPieSerchParam);
                //if (MetricsPieCookie != null)
                //{
                //    MetricsPieCookie = OEE_PieCookie;
                //    HttpContext.Response.SetCookie(MetricsPieCookie);
                //}
                //else
                //{
                //    var NewMetricsPieCookie = new HttpCookie(SessionConstants.OEE_MetricsPieSerchParam);
                //    NewMetricsPieCookie = OEE_PieCookie;
                //    HttpContext.Response.SetCookie(NewMetricsPieCookie);
                //}
            }
            else
            {
                var OEE_MetricsPieMy_Cookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsPieSerchParam);
                if (OEE_MetricsPieMy_Cookie != null)
                {
                    Plant_UID = OEE_MetricsPieMy_Cookie["Plant_UID"];
                    BG_UID = OEE_MetricsPieMy_Cookie["BG_UID"];
                    customerId = OEE_MetricsPieMy_Cookie["customerId"];
                    lineName = OEE_MetricsPieMy_Cookie["lineName"];
                    stationName = OEE_MetricsPieMy_Cookie["stationName"];
                    MachineName = OEE_MetricsPieMy_Cookie["MachineName"];
                    ShiftID = OEE_MetricsPieMy_Cookie["ShiftID"];
                    startTime = OEE_MetricsPieMy_Cookie["startTime"];
                    endTime = OEE_MetricsPieMy_Cookie["startTime"];
                    isFromPhotoReport = "true";
                }
            }

            #endregion
            ViewBag.Plant_UID = Plant_UID;
            ViewBag.BG_UID = BG_UID;
            ViewBag.customerId = customerId;
            ViewBag.lineName = lineName;
            ViewBag.stationName = stationName;
            ViewBag.MachineName = MachineName;
            ViewBag.ShiftID = ShiftID;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.isFromPhotoReport = isFromPhotoReport;

            return View("OEE_MetricsPhotoReport", currentVM);
        }

        public ActionResult QueryMetricsReport(OEE_ReprortSearchModel searchParmm, Page page)
        {
            var MetricsPieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsPieSerchParam);
            if (MetricsPieCookie != null)
            {
                MetricsPieCookie["Plant_UID"] = searchParmm.Plant_Organization_UID.ToString();
                MetricsPieCookie["BG_UID"] = searchParmm.BG_Organization_UID.ToString();
                MetricsPieCookie["customerId"] = searchParmm.CustomerID.ToString();
                MetricsPieCookie["lineName"] = searchParmm.LineID.ToString();
                MetricsPieCookie["stationName"] = searchParmm.StationID.ToString();
                MetricsPieCookie["MachineName"] = searchParmm.EQP_Uid.ToString();
                MetricsPieCookie["ShiftID"] = searchParmm.ShiftTimeID.ToString();
                MetricsPieCookie["startTime"] = searchParmm.StartTime.ToString("yyyy-MM-dd");
                MetricsPieCookie["endTime"] = searchParmm.EndTime.ToString("yyyy-MM-dd");
                MetricsPieCookie["isFromPhotoReport"] = "false";
                MetricsPieCookie.Expires.AddDays(30);
                HttpContext.Response.SetCookie(MetricsPieCookie);
            }
            else
            {
                var NewMetricsPieCookie = new HttpCookie(SessionConstants.OEE_MetricsPieSerchParam);
                NewMetricsPieCookie["Plant_UID"] = searchParmm.Plant_Organization_UID.ToString();
                NewMetricsPieCookie["BG_UID"] = searchParmm.BG_Organization_UID.ToString();
                NewMetricsPieCookie["customerId"] = searchParmm.CustomerID.ToString();
                NewMetricsPieCookie["lineName"] = searchParmm.LineID.ToString();
                NewMetricsPieCookie["stationName"] = searchParmm.StationID.ToString();
                NewMetricsPieCookie["MachineName"] = searchParmm.EQP_Uid.ToString();
                NewMetricsPieCookie["ShiftID"] = searchParmm.ShiftTimeID.ToString();
                NewMetricsPieCookie["startTime"] = searchParmm.StartTime.ToString("yyyy-MM-dd");
                NewMetricsPieCookie["endTime"] = searchParmm.EndTime.ToString("yyyy-MM-dd");
                NewMetricsPieCookie["isFromPhotoReport"] = "false";
                NewMetricsPieCookie.Expires.AddDays(30);
                HttpContext.Response.SetCookie(NewMetricsPieCookie);
            }


            searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrlMetrics = string.Format("OEE/GETOEEMetricsAPI");
            HttpResponseMessage responMessageMetrics = APIHelper.APIPostAsync(searchParmm, apiUrlMetrics);
            var resultMetrics = responMessageMetrics.Content.ReadAsStringAsync().Result;
            var apiUrlFirstYield = string.Format("OEE/GetFirstYieldAPI");
            HttpResponseMessage responMessageFirstYield = APIHelper.APIPostAsync(searchParmm, apiUrlFirstYield);
            var resultFirstYield = responMessageFirstYield.Content.ReadAsStringAsync().Result;
            var apiUrlDowntime = string.Format("OEE/GetDowntimeBreakdownAPI");
            HttpResponseMessage responMessageDowntime = APIHelper.APIPostAsync(searchParmm, apiUrlDowntime);
            var resultDowntime = responMessageDowntime.Content.ReadAsStringAsync().Result;

            var resultMetricsList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultMetrics).ToList();
            var resultDowntimeList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultDowntime).ToList();
            var resultFirstYieldList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultFirstYield).ToList();

            //1 计算OEE 值
            MetricsPhotoModel MetricsReport = new MetricsPhotoModel();

            #region  异常判断
            MetricsReport.Is_CncResetAbnomal = resultMetricsList.FirstOrDefault().Is_CncResetAbnomal;
            MetricsReport.Is_DtCodeAbnormal = resultMetricsList.FirstOrDefault().Is_DtCodeAbnormal;
            MetricsReport.Is_DFCodeAbnomal = resultMetricsList.FirstOrDefault().Is_DfCeodeAbnomal;
            MetricsReport.Is_OeeAbnomal = resultMetricsList.FirstOrDefault().Is_OeeAbnomal;
            MetricsReport.ResetTime = resultMetricsList.FirstOrDefault().ResetTime;
            MetricsReport.FirstDashBoardTarget = resultMetricsList.FirstOrDefault().FirstDashBoardTarget;
            MetricsReport.SecondDashBoardTarget = resultMetricsList.FirstOrDefault().SecondDashBoardTarget;
            #endregion

            MetricsReport.AllEquipmentEfficiency = resultMetricsList.Count() == 0 ? 0.ToString("p2") : ForMatOEEValueToP(double.Parse(resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.AllEquipmentEfficiency)).FirstOrDefault().IndexCount));
            MetricsReport.AvailableRate = resultMetricsList.Count() == 0 ? 0.ToString("p2") : ForMatOEEValueToP(double.Parse(resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.AvailableRate)).FirstOrDefault().IndexCount));
            MetricsReport.PerformanceRate = resultMetricsList.Count() == 0 ? 0.ToString("p2") : ForMatOEEValueToP(double.Parse(resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.PerformanceRate)).FirstOrDefault().IndexCount));
            var PerformanceRate = resultMetricsList.Count() == 0 ? 0 : double.Parse(resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.PerformanceRate)).FirstOrDefault().IndexCount);
            var firstYieldFenmu = resultMetricsList.Count() == 0 ? 0 : double.Parse(resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.Throughput)).FirstOrDefault().IndexCount);
            MetricsReport.FirstYield = (resultMetricsList.Count() == 0 || firstYieldFenmu == 0) ? 0.ToString("p2") :
              ForMatOEEValueToP(double.Parse(resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput)).FirstOrDefault().IndexCount)
                / firstYieldFenmu);
            var FirstYield = (resultMetricsList.Count() == 0 || firstYieldFenmu == 0) ? 0 :
               (double.Parse(resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.GoodPartOutput)).FirstOrDefault().IndexCount)
                / firstYieldFenmu);

            #region  计算Equipment的值
            var equipment = new List<double>();
            var process = new List<double>();
            var maitenance = new List<double>();
            var other = new List<double>();
            foreach (var item in resultDowntimeList)
            {
                string[] sArray = Regex.Split(item.IndexName, "-", RegexOptions.IgnoreCase);
                if (sArray[0].ToUpper().Trim() == "EQUIPMENT")
                {
                    equipment.Add(double.Parse(item.IndexCount));
                }
                else if (sArray[0].ToUpper().Trim() == "PROCESS")
                {
                    process.Add(double.Parse(item.IndexCount));
                }
                else if (sArray[0].ToUpper().Trim() == "MAITENANCE")
                {
                    maitenance.Add(double.Parse(item.IndexCount));
                }
                else if (sArray[0].ToUpper().Trim() == "OTHER")
                {
                    other.Add(double.Parse(item.IndexCount));
                }
            }
            #endregion

            var PlannedMinute = resultMetricsList.Count() == 0 || resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).FirstOrDefault() == null ? "0" : resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.PlannedMinute)).FirstOrDefault().IndexCount;
            MetricsReport.EquipmentDown = double.Parse(PlannedMinute) == 0 ? 0.ToString("p2") : ForMatOEEValueToP(equipment.Sum() / (double.Parse(PlannedMinute) * 60));
            MetricsReport.ProcessDown = double.Parse(PlannedMinute) == 0 ? 0.ToString("p2") : ForMatOEEValueToP(process.Sum() / (double.Parse(PlannedMinute) * 60));
            MetricsReport.Other = double.Parse(PlannedMinute) == 0 ? 0.ToString("p2") : ForMatOEEValueToP((other.Sum()) / (double.Parse(PlannedMinute) * 60));
            MetricsReport.Maitenance = double.Parse(PlannedMinute) == 0 ? 0.ToString("p2") : ForMatOEEValueToP((maitenance.Sum()) / (double.Parse(PlannedMinute) * 60));
            var POR = resultMetricsList.Count() == 0 ? "0" : resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.POR)).FirstOrDefault().IndexCount;
            var ActualCT = resultMetricsList.Count() == 0 ? "0" : resultMetricsList.Where(p => p.IndexName == EnumHelper.GetDescription(MachineIndexName.ActualCT)).FirstOrDefault().IndexCount;
            MetricsReport.CTDiscrepacies = double.Parse(ActualCT) == 0 ? 0.ToString("p2") : ForMatOEEValueToP((1 - double.Parse(POR) / double.Parse(ActualCT)));
            var CTDiscrepacies = double.Parse(ActualCT) == 0 ? 0 : (1 - double.Parse(POR) / double.Parse(ActualCT));
            MetricsReport.OtherMicrostop = ForMatOEEValueToP(1 - PerformanceRate - CTDiscrepacies);
            MetricsReport.Quality = ForMatOEEValueToP(1 - FirstYield);

            var result = JsonConvert.SerializeObject(MetricsReport);
            return Content(result, "application/json");
        }

        public string ForMatOEEValueToP(double param)
        {
            if (param < 0)
            {
                return 0.ToString("P2");
            }
            else if (param > 1)
            {
                return 1.ToString("P2");
            }
            else
            {
                return param.ToString("P2");
            }
        }
        public string ForMatOEEValueToS(double param)
        {
            if (param < 0)
            {
                return 0.ToString();
            }
            else if (param > 1)
            {
                return 1.ToString();
            }
            else
            {
                return param.ToString();
            }
        }
        public ActionResult GetMetricDowntimeBreakdown(OEE_ReprortSearchModel search, Page page)
        {
            search.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("OEE/GetDowntimeBreakdownAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //var resultMetricsList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(result).ToList();
            //if (resultMetricsList.Count() == 0 || resultMetricsList == null)
            //{
            //    result = "No Data";
            //}
            return Content(result, "application/json");
        }
        #endregion

        #region  停机大类维护
        public ActionResult OEE_DownType()
        {

            OEE_DownTimeCodeVM currentVM = new OEE_DownTimeCodeVM();
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
            return View("OEE_DownType", currentVM);
        }
        public ActionResult QueryOEE_DownType(OEE_DownTypeDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }

            var apiUrl = string.Format("OEE/QueryOEE_DownTypeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditOEE_DownType(OEE_DownTypeDTO dto, bool isEdit)
        {

            dto.Modify_UID = CurrentUser.AccountUId;
            dto.Modify_Date = DateTime.Now;
            var apiUrl = string.Format("OEE/AddOrEditOEE_DownTypeAPI?isEdit={0}", isEdit);
            HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }
        public ActionResult QueryOEE_DownTypeByUid(int OEE_DownTimeType_UID)
        {
            var apiUrl = string.Format("OEE/QueryDownTypeByUidAPI?OEE_DownTimeType_UID={0}", OEE_DownTimeType_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string DeleteOEE_DownType(int OEE_DownTimeType_UID)
        {
            var apiUrl = string.Format("OEE/DeleteDownTypeAPI?OEE_DownTimeType_UID={0}&userid={1}", OEE_DownTimeType_UID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        }

        #endregion

        #region 机台统计图表
        public ActionResult OEE_MachinePieReport(string Plant_UID, string BG_UID, string customerId, string lineName, string stationName, string MachineName, string ShiftID, string startTime, string endTime, string isFromPhotoReport, string OrderByRule)
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

            if (isFromPhotoReport == "MetricsPieOEE")//来源于Metrics 图表
            {
                var OEE_PieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_MetricsPieSerchParam);
                Plant_UID = OEE_PieCookie["Plant_UID"];
                BG_UID = OEE_PieCookie["BG_UID"];
                customerId = OEE_PieCookie["customerId"];
                lineName = OEE_PieCookie["lineName"];
                stationName = OEE_PieCookie["stationName"];
                MachineName = OEE_PieCookie["MachineName"];
                ShiftID = OEE_PieCookie["ShiftID"];
                startTime = OEE_PieCookie["startTime"];
                endTime = OEE_PieCookie["startTime"];
                isFromPhotoReport = "true";
            }
            else if (isFromPhotoReport == "LineOEE")//来源于线别报表
            {
                var OEE_PieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_LineSerchParam);
                Plant_UID = OEE_PieCookie["Plant_UID"];
                BG_UID = OEE_PieCookie["BG_UID"];
                customerId = OEE_PieCookie["customerId"];
                lineName = OEE_PieCookie["lineName"];
                stationName = stationName;
                OrderByRule = OEE_PieCookie["OrderByRule"];
                ShiftID = OEE_PieCookie["ShiftID"];
                startTime = OEE_PieCookie["startTime"];
                endTime = OEE_PieCookie["startTime"];
                isFromPhotoReport = "true";
            }
            else
            {
                var OEE_PieMy_Cookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_PieSerchParam);
                if (OEE_PieMy_Cookie != null)
                {
                    Plant_UID = OEE_PieMy_Cookie["Plant_UID"];
                    BG_UID = OEE_PieMy_Cookie["BG_UID"];
                    customerId = OEE_PieMy_Cookie["customerId"];
                    lineName = OEE_PieMy_Cookie["lineName"];
                    stationName = OEE_PieMy_Cookie["stationName"];
                    MachineName = OEE_PieMy_Cookie["MachineName"];
                    ShiftID = OEE_PieMy_Cookie["ShiftID"];
                    startTime = OEE_PieMy_Cookie["startTime"];
                    endTime = OEE_PieMy_Cookie["startTime"];
                    isFromPhotoReport = "true";
                    OrderByRule = OEE_PieMy_Cookie["OrderByRule"];
                }
            }

            ViewBag.Plant_UID = Plant_UID;
            ViewBag.BG_UID = BG_UID;
            ViewBag.customerId = customerId;
            ViewBag.lineName = lineName;
            ViewBag.stationName = stationName;
            ViewBag.MachineName = MachineName;
            ViewBag.ShiftID = ShiftID;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.isFromPhotoReport = isFromPhotoReport;
            ViewBag.OrderByRule = OrderByRule;

            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("OEE_MachinePieReport", currentVM);
        }

        public ActionResult GetMachinePieReportData(OEE_ReprortSearchModel searchParmm, Page page)
        {
            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_PieSerchParam);
            if (cooike != null)
            {
                cooike["Plant_UID"] = searchParmm.Plant_Organization_UID.ToString();
                cooike["BG_UID"] = searchParmm.BG_Organization_UID.ToString();
                cooike["customerId"] = searchParmm.CustomerID.ToString();
                cooike["lineName"] = searchParmm.LineID.ToString();
                cooike["stationName"] = searchParmm.StationID.ToString();
                cooike["ShiftID"] = searchParmm.ShiftTimeID.ToString();
                cooike["OrderByRule"] = searchParmm.OrderByRule.ToString();
                cooike["startTime"] = searchParmm.StartTime.ToString("yyyy-MM-dd");
                cooike["endTime"] = searchParmm.EndTime.ToString("yyyy-MM-dd");
                cooike["isFromPhotoReport"] = "false";
                cooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(cooike);
            }
            else
            {
                var OEE_Cookie = new HttpCookie(SessionConstants.OEE_PieSerchParam);
                OEE_Cookie["Plant_UID"] = searchParmm.Plant_Organization_UID.ToString();
                OEE_Cookie["BG_UID"] = searchParmm.BG_Organization_UID.ToString();
                OEE_Cookie["customerId"] = searchParmm.CustomerID.ToString();
                OEE_Cookie["lineName"] = searchParmm.LineID.ToString();
                OEE_Cookie["stationName"] = searchParmm.StationID.ToString();
                OEE_Cookie["ShiftID"] = searchParmm.ShiftTimeID.ToString();
                OEE_Cookie["OrderByRule"] = searchParmm.OrderByRule.ToString();
                OEE_Cookie["startTime"] = searchParmm.StartTime.ToString("yyyy-MM-dd");
                OEE_Cookie["endTime"] = searchParmm.EndTime.ToString("yyyy-MM-dd");
                OEE_Cookie["isFromPhotoReport"] = "false";
                OEE_Cookie.Expires.AddDays(30);
                HttpContext.Response.SetCookie(OEE_Cookie);
            }

            searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("OEE/GetMachinePieReportDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public static void TestExport(OEE_ReprortSearchModel searchParmm)
        {
            FileInfo newFile = new FileInfo(@"d:\test.xlsx");
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(@"d:\test.xlsx");
            }

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("test");
                ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("DataSoure");

                worksheet.Cells.Style.WrapText = true;
                worksheet.View.ShowGridLines = false;//去掉sheet的网格线
                searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
                var apiUrlMetrics = string.Format("OEE/GETOEEMetricsAPI");
                HttpResponseMessage responMessageMetrics = APIHelper.APIPostAsync(searchParmm, apiUrlMetrics);
                var resultMetrics = responMessageMetrics.Content.ReadAsStringAsync().Result;
                var apiUrlFirstYield = string.Format("OEE/GetFirstYieldAPI");
                HttpResponseMessage responMessageFirstYield = APIHelper.APIPostAsync(searchParmm, apiUrlFirstYield);
                var resultFirstYield = responMessageFirstYield.Content.ReadAsStringAsync().Result;
                var apiUrlDowntime = string.Format("OEE/GetDowntimeBreakdownAPI");
                HttpResponseMessage responMessageDowntime = APIHelper.APIPostAsync(searchParmm, apiUrlDowntime);
                var resultDowntime = responMessageDowntime.Content.ReadAsStringAsync().Result;

                var resultDowntimeList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultDowntime).ToList();
                var resultFirstYieldList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultFirstYield).ToList();
                var resultMetricsList = JsonConvert.DeserializeObject<List<MachineIndexModel>>(resultMetrics).ToList();

                List<MachineIndexModel> modelList = new List<MachineIndexModel>();
                for (int i = 2; i < 8; i++)
                {
                    MachineIndexModel model = new MachineIndexModel();
                    model.IndexCount = "1" + i;
                    model.IndexName = "Indename_" + i;
                    modelList.Add(model);
                }
                worksheet1.Cells[1, 1].Value = "名称";
                worksheet1.Cells[1, 2].Value = "价格";
                worksheet1.Cells[1, 3].Value = "销量";

                var index = 2;
                foreach (var item in modelList)
                {
                    worksheet1.Cells[index, 1].Value = item.IndexName;
                    worksheet1.Cells[index, 2].Value = int.Parse(item.IndexCount);
                    worksheet1.Cells[index, 3].Value = int.Parse(item.IndexCount);
                    index++;
                }

                //worksheet.Cells[1, 1].Value = "名称";
                //worksheet.Cells[1, 2].Value = "价格";
                //worksheet.Cells[1, 3].Value = "销量";

                //worksheet.Cells[2, 1].Value = "大米";
                //worksheet.Cells[2, 2].Value = 56;
                //worksheet.Cells[2, 3].Value = 100;

                //worksheet.Cells[3, 1].Value = "玉米";
                //worksheet.Cells[3, 2].Value = 45;
                //worksheet.Cells[3, 3].Value = 150;

                //worksheet.Cells[4, 1].Value = "小米";
                //worksheet.Cells[4, 2].Value = 38;
                //worksheet.Cells[4, 3].Value = 130;

                //worksheet.Cells[5, 1].Value = "糯米";
                //worksheet.Cells[5, 2].Value = 22;
                //worksheet.Cells[5, 3].Value = 200;

                using (ExcelRange range = worksheet1.Cells[1, 1, index, 3])
                {
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                using (ExcelRange range = worksheet1.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Font.Name = "微软雅黑";
                    range.Style.Font.Size = 12;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 128, 128));
                }

                ExcelPieChart chart = (ExcelPieChart)worksheet.Drawings.AddChart("crtExtensionsSize", eChartType.Pie3D);
                ExcelChartSerie serie = chart.Series.Add(worksheet1.Cells[2, 3, index, 3], worksheet1.Cells[2, 1, index, 1]);
                serie.HeaderAddress = worksheet.Cells[1, 3];

                chart.SetPosition(300, 20);
                chart.SetSize(800, 400);
                chart.Title.Text = "OEE";
                chart.Title.Font.Color = Color.FromArgb(89, 89, 89);
                chart.Title.Font.Size = 15;
                chart.Title.Font.Bold = true;
                chart.Style = eChartStyle.Style1;
                chart.Legend.Border.LineStyle = eLineStyle.Solid;
                chart.Legend.Border.Fill.Color = Color.FromArgb(217, 217, 217);
                chart.DataLabel.ShowCategory = true;
                chart.DataLabel.ShowPercent = true;
                chart.DataLabel.ShowLeaderLines = true;
                //if (package.Workbook.Worksheets["test"] != null)
                //{
                //    package.Workbook.Worksheets["test"].Hidden = eWorkSheetHidden.VeryHidden;
                //}
                package.Save();
            }
        }

        /// <summary>
        /// 导出机台统计图表的宕机Code
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult ExportMachineAbnormalDTCode(OEE_ReprortSearchModel searchParmm)
        {
            searchParmm.VersionNumber = 0;
            var propertiesHead = GetAbnormalDFCodeHeadColumn();
            var apiUrl = string.Format("OEE/ExportAbnormalDTCodeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_AbnormalDFCode>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("AbnormalDFCode");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("AbnormalDFCode");
                SetExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.ProjectName;
                        worksheet.Cells[iRow, 5].Value = item.LineName;
                        worksheet.Cells[iRow, 6].Value = item.StationName;
                        worksheet.Cells[iRow, 7].Value = item.MachineName;
                        worksheet.Cells[iRow, 8].Value = item.DownTimeCode;
                        worksheet.Cells[iRow, 9].Value = item.ShiftName;
                        worksheet.Cells[iRow, 10].Value = item.ProductDate.ToString("yyyy-MM-dd");
                        worksheet.Cells[iRow, 11].Value = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                        iRow++;
                    }
                }
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }

        /// <summary>
        /// 工站的DownTime Code
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult ExportStationAbnormalDTCode(OEE_ReprortSearchModel searchParmm)
        {
            searchParmm.VersionNumber = 1;
            var propertiesHead = GetAbnormalDFCodeHeadColumn();
            var apiUrl = string.Format("OEE/ExportAbnormalDTCodeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<OEE_AbnormalDFCode>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("AbnormalDTCode");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("AbnormalDTCode");
                SetExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.ProjectName;
                        worksheet.Cells[iRow, 5].Value = item.LineName;
                        worksheet.Cells[iRow, 6].Value = item.StationName;
                        worksheet.Cells[iRow, 7].Value = item.MachineName;
                        worksheet.Cells[iRow, 8].Value = item.DownTimeCode;
                        worksheet.Cells[iRow, 9].Value = item.ShiftName;
                        worksheet.Cells[iRow, 10].Value = item.ProductDate.ToString("yyyy-MM-dd");
                        worksheet.Cells[iRow, 11].Value = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                        iRow++;
                    }
                }
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }

        /// <summary>
        /// 机台的不良代码
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult ExportMachineAbnormalDFCode(OEE_ReprortSearchModel searchParmm)
        {
            searchParmm.VersionNumber = 0;
            var propertiesHead = GetDFCodeHeadColumn();
            var apiUrl = string.Format("OEE/ExportAbnormalDFCodeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<AbnormalDFCode>>(result);

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("ExportAbnormalDFCode");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("AbnormalDFCode");
                SetExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {

                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.ProjectName;
                        worksheet.Cells[iRow, 5].Value = item.LineName;
                        worksheet.Cells[iRow, 6].Value = item.StationName;
                        worksheet.Cells[iRow, 7].Value = item.MachineName;
                        worksheet.Cells[iRow, 8].Value = item.DFCode;
                        worksheet.Cells[iRow, 9].Value = item.ShiftName;
                        worksheet.Cells[iRow, 10].Value = item.ProductDate.ToString("yyyy-MM-dd");
                        worksheet.Cells[iRow, 11].Value = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                        iRow++;
                    }
                }
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }

        /// <summary>
        /// 工站的不良代码
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult ExportStationAbnormalDFCode(OEE_ReprortSearchModel searchParmm)
        {
            searchParmm.VersionNumber = 1;
            var propertiesHead = GetDFCodeHeadColumn();
            var apiUrl = string.Format("OEE/ExportAbnormalDFCodeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<AbnormalDFCode>>(result);
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("ExportAbnormalDFCode");

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("AbnormalDFCode");
                SetExcelStyle(worksheet, propertiesHead);
                int iRow = 2;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        worksheet.Cells[iRow, 1].Value = item.Plant_Organization_Name;
                        worksheet.Cells[iRow, 2].Value = item.BG_Organization_Name;
                        worksheet.Cells[iRow, 3].Value = item.FunPlant_Organization_Name;
                        worksheet.Cells[iRow, 4].Value = item.ProjectName;
                        worksheet.Cells[iRow, 5].Value = item.LineName;
                        worksheet.Cells[iRow, 6].Value = item.StationName;
                        worksheet.Cells[iRow, 7].Value = item.MachineName;
                        worksheet.Cells[iRow, 8].Value = item.DFCode;
                        worksheet.Cells[iRow, 9].Value = item.ShiftName;
                        worksheet.Cells[iRow, 10].Value = item.ProductDate.ToString("yyyy-MM-dd");
                        worksheet.Cells[iRow, 11].Value = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                        iRow++;
                    }
                }
                excelPackage.Save();
                return new FileContentResult(stream.ToArray(), "application/octet-stream")
                { FileDownloadName = Server.UrlEncode(fileName) };
            }

        }
        #endregion

        #region Line统计图表 
        public ActionResult OEE_LineStaticReport(string Plant_UID, string BG_UID, string customerId, string lineName, string stationName, string MachineName, string ShiftID, string startTime, string endTime, string isFromPhotoReport, string OrderByRule)
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

            var OEE_PieMy_Cookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_PieSerchParam);
            if (OEE_PieMy_Cookie != null && isFromPhotoReport == "StationStatic")//来源于工站统计图表
            {
                Plant_UID = OEE_PieMy_Cookie["Plant_UID"];
                BG_UID = OEE_PieMy_Cookie["BG_UID"];
                customerId = OEE_PieMy_Cookie["customerId"];
                lineName = OEE_PieMy_Cookie["lineName"];
                stationName = OEE_PieMy_Cookie["stationName"];
                OrderByRule = OEE_PieMy_Cookie["OrderByRule"];
                ShiftID = OEE_PieMy_Cookie["ShiftID"];
                startTime = OEE_PieMy_Cookie["startTime"];
                endTime = OEE_PieMy_Cookie["startTime"];
                isFromPhotoReport = "true";
            }
            else
            {
                var LineSerch_Cookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_LineSerchParam);
                if (LineSerch_Cookie != null)
                {
                    Plant_UID = LineSerch_Cookie["Plant_UID"];
                    BG_UID = LineSerch_Cookie["BG_UID"];
                    customerId = LineSerch_Cookie["customerId"];
                    lineName = LineSerch_Cookie["lineName"];
                    stationName = LineSerch_Cookie["stationName"];
                    OrderByRule = LineSerch_Cookie["OrderByRule"];
                    MachineName = LineSerch_Cookie["MachineName"];
                    ShiftID = LineSerch_Cookie["ShiftID"];
                    startTime = LineSerch_Cookie["startTime"];
                    endTime = LineSerch_Cookie["startTime"];
                    isFromPhotoReport = "true";
                }
            }

            ViewBag.Plant_UID = Plant_UID;
            ViewBag.BG_UID = BG_UID;
            ViewBag.customerId = customerId;
            ViewBag.lineName = lineName;
            ViewBag.stationName = stationName;
            ViewBag.MachineName = MachineName;
            ViewBag.ShiftID = ShiftID;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.OrderByRule = OrderByRule;
            ViewBag.isFromPhotoReport = isFromPhotoReport;

            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("OEE_LineStaticReport", currentVM);
        }

        public ActionResult GetLineStaticData(OEE_ReprortSearchModel searchParmm, Page page)
        {
            var cooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_LineSerchParam);
            if (cooike != null)
            {
                cooike["Plant_UID"] = searchParmm.Plant_Organization_UID.ToString();
                cooike["BG_UID"] = searchParmm.BG_Organization_UID.ToString();
                cooike["customerId"] = searchParmm.CustomerID.ToString();
                cooike["lineName"] = searchParmm.LineID.ToString();
                cooike["stationName"] = searchParmm.StationID.ToString();
                cooike["ShiftID"] = searchParmm.ShiftTimeID.ToString();
                cooike["OrderByRule"] = searchParmm.OrderByRule.ToString();
                cooike["startTime"] = searchParmm.StartTime.ToString("yyyy-MM-dd");
                cooike["endTime"] = searchParmm.EndTime.ToString("yyyy-MM-dd");
                cooike["isFromPhotoReport"] = "false";
                cooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(cooike);
            }
            else
            {
                var OEE_Cookie = new HttpCookie(SessionConstants.OEE_LineSerchParam);
                OEE_Cookie["Plant_UID"] = searchParmm.Plant_Organization_UID.ToString();
                OEE_Cookie["BG_UID"] = searchParmm.BG_Organization_UID.ToString();
                OEE_Cookie["customerId"] = searchParmm.CustomerID.ToString();
                OEE_Cookie["lineName"] = searchParmm.LineID.ToString();
                OEE_Cookie["stationName"] = searchParmm.StationID.ToString();
                OEE_Cookie["ShiftID"] = searchParmm.ShiftTimeID.ToString();
                OEE_Cookie["OrderByRule"] = searchParmm.OrderByRule.ToString();
                OEE_Cookie["startTime"] = searchParmm.StartTime.ToString("yyyy-MM-dd");
                OEE_Cookie["endTime"] = searchParmm.EndTime.ToString("yyyy-MM-dd");
                OEE_Cookie["isFromPhotoReport"] = "false";
                OEE_Cookie.Expires.AddDays(30);
                HttpContext.Response.SetCookie(OEE_Cookie);
            }

            searchParmm.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("OEE/GetLineStaticDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region OEE 4Q
        /// <summary>
        /// OEE 4Q Report 报表
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult OEEFourQReport()
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

            //ViewBag.Plant_UID = Plant_UID;
            //ViewBag.BG_UID = BG_UID;
            //ViewBag.customerId = customerId;
            //ViewBag.lineName = lineName;
            //ViewBag.stationName = stationName;
            //ViewBag.MachineName = MachineName;
            //ViewBag.ShiftID = ShiftID;
            //ViewBag.startTime = startTime;
            //ViewBag.endTime = endTime;
            //ViewBag.OrderByRule = OrderByRule;
            //ViewBag.isFromPhotoReport = isFromPhotoReport;

            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;
            return View("OEEFourQReport", currentVM);
        }

        /// <summary>
        ///  获取实际的开始时间和结束时间
        /// </summary>
        /// <param name="reporttype"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public string GetActualTime(string reporttype, string starttime, string endtime)
        {
            var starttimeDate = Convert.ToDateTime(starttime);
            var endtimeDate = Convert.ToDateTime(endtime);
            switch (reporttype)
            {
                case "Week":
                    starttime = starttimeDate.AddDays(-(int)starttimeDate.DayOfWeek + 1).ToString("yyyy-MM-dd");
                    break;
                case "Month":
                    starttime = starttimeDate.AddDays(-starttimeDate.Day + 1).ToString("yyyy-MM-dd");
                    break;
                default:
                    break;
            }

            switch (reporttype)
            {
                case "Week":
                    endtime = endtimeDate.AddDays(7 - (int)endtimeDate.DayOfWeek).ToString("yyyy-MM-dd");
                    break;
                case "Month":
                    endtime = endtimeDate.AddMonths(1).AddDays(-endtimeDate.AddMonths(1).Day + 1).AddDays(-1).ToString("yyyy-MM-dd");
                    break;
                default:
                    break;
            }

            var actualTime = string.Format("周期 ：{0} 至 {1} ", starttime, endtime);
            return actualTime;
        }


        public string CheckDate(string reporttype, string starttime, string endtime)
        {
            var starttimeDate = Convert.ToDateTime(starttime);
            var endtimeDate = Convert.ToDateTime(endtime);
            var result = "true";
            switch (reporttype)
            {
                case "Week":
                    if ((endtimeDate - starttimeDate).TotalDays > 13 * 7)
                    {
                        result = "日期的周数不能大于13周,请重新选择！";
                    }
                    break;
                case "Month":
                    int Month = Math.Abs((endtimeDate.Year - starttimeDate.Year)) * 12 + Math.Abs((endtimeDate.Month - endtimeDate.Month));
                    if (Month > 12)
                    {
                        result = "日期的月数不能大于12月,请重新选择！";
                    }
                    break;
                case "Daily":
                    if ((endtimeDate - starttimeDate).TotalDays > 14)
                    {
                        result = "日期的天数不能大于14天,请重新选择！";
                    }
                    break;
                default:
                    result = "请重新选择,查询类型";
                    break;
            }
            return result;
        }

        public ActionResult GetActualDTTime(OEEFourQParamModel searchParmm)
        {
            var starttimeDate = Convert.ToDateTime(searchParmm.StartTime);
            var endtimeDate = Convert.ToDateTime(searchParmm.EndTime);
            switch (searchParmm.ReportType)
            {
                case "Week":
                    searchParmm.StartTime = starttimeDate.AddDays(-(int)starttimeDate.DayOfWeek + 1);
                    break;
                case "Month":
                    searchParmm.StartTime = starttimeDate.AddDays(-starttimeDate.Day + 1);
                    break;
                default:
                    break;
            }

            switch (searchParmm.ReportType)
            {
                case "Week":
                    searchParmm.EndTime = endtimeDate.AddDays(7 - (int)endtimeDate.DayOfWeek);
                    break;
                case "Month":
                    searchParmm.EndTime = endtimeDate.AddMonths(1).AddDays(-endtimeDate.AddMonths(1).Day + 1).AddDays(-1);
                    break;
                default:
                    break;
            }
            var apiUrl = string.Format("OEE/GetFourQDTTimeAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        public ActionResult GetFourQActionInfo(OEEFourQParamModel searchParmm, Page page)
        {
            var apiUrl = string.Format("OEE/GetFourQActionInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        ///  获取4Q的明细信息
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult GetFourQDTTypeDetail(OEEFourQParamModel searchParmm)
        {
            var apiUrl = string.Format("OEE/GetFourQDTTypeDetailAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        ///  获取4Q的明细信息
        /// </summary>
        /// <param name="searchParmm"></param>
        /// <returns></returns>
        public ActionResult GetPaynterChartDetial(OEEFourQParamModel searchParmm)
        {
            var starttimeDate = Convert.ToDateTime(searchParmm.StartTime);
            var endtimeDate = Convert.ToDateTime(searchParmm.EndTime);
            switch (searchParmm.ReportType)
            {
                case "Week":
                    searchParmm.StartTime = starttimeDate.AddDays(-(int)starttimeDate.DayOfWeek + 1);
                    break;
                case "Month":
                    searchParmm.StartTime = starttimeDate.AddDays(-starttimeDate.Day + 1);
                    break;
                default:
                    break;
            }

            switch (searchParmm.ReportType)
            {
                case "Week":
                    searchParmm.EndTime = endtimeDate.AddDays(7 - (int)endtimeDate.DayOfWeek);
                    break;
                case "Month":
                    searchParmm.EndTime = endtimeDate.AddMonths(1).AddDays(-endtimeDate.AddMonths(1).Day + 1).AddDays(-1);
                    break;
                default:
                    break;
            }

            var apiUrl = string.Format("OEE/GetPaynterChartDetialAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParmm, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        #endregion


        #region OEE 4Q BaseMaintain


        #region  会议信息
        public ActionResult OEE_MeetingTypeInfo()
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
            return View("OEE_MeetingTypeInfo", currentVM);
        }

        public ActionResult QueryOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }

            var apiUrl = string.Format("OEE/QueryOEE_MeetingTypeInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public ActionResult AddOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/AddOEE_MeetingTypeInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult UpdateOEE_MeetingTypeInfo(OEE_MeetingTypeInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/UpdateOEE_MeetingTypeInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteOEE_MeetingTypeInfo(string meetingTypeInfo_UID)
        {
            var apiUrl = string.Format("OEE/DeleteOEE_MeetingTypeInfoAPI?meetingTypeInfo_UID={0}", meetingTypeInfo_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult GetOEE_MeetingTypeInfoById(string uid)
        {
            var apiUrl = string.Format("OEE/GetOEE_MeetingTypeInfoByIdAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetMeetingTypeName(string plantUid, string bgUid, string funplantUid)
        {
            var apiUrl = string.Format("OEE/GetMeetingTypeNameAPI?plantUid={0}&bgUid={1}&funplantUid={2}", int.Parse(plantUid), int.Parse(bgUid), string.IsNullOrEmpty(funplantUid) ? 0 : int.Parse(funplantUid));
            HttpResponseMessage response = APIHelper.APIGetAsync(apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion


        #region  OEE_MetricInfo
        public ActionResult OEE_MetricInfo()
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
            return View("OEE_MetricInfo", currentVM);
        }

        public ActionResult QueryMetricInfo(OEE_MetricInfoDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }

            var apiUrl = string.Format("OEE/QueryMetricInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        ///   增加Metrice 的基本信息
        /// </summary>
        /// <param name="serchModel"></param>
        /// <returns></returns>
        public ActionResult AddMetricInfoInfo(OEE_MetricInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/AddMetricInfoInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult UpdateMetricInfo(OEE_MetricInfoDTO serchModel)
        {
            serchModel.Modified_UID = this.CurrentUser.AccountUId;
            serchModel.Modified_Date = DateTime.Now;
            string json = JsonConvert.SerializeObject(serchModel);
            var apiUrl = string.Format("OEE/UpdateMetricInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(json, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteMetricInfo(string metricInfo_Uid)
        {
            var apiUrl = string.Format("OEE/DeleteMetricInfoAPI?metricInfo_Uid={0}", metricInfo_Uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetMetricInfoByIdAPI(string uid)
        {
            var apiUrl = string.Format("OEE/GetMetricInfoByIdAPI?uid={0}", uid);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string AddOrEditMetricInfo(OEE_MetricInfoDTO dto, bool isEdit)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = string.Format("OEE/AddOrEditMetricInfoAPI?isEdit={0}", isEdit);
            HttpResponseMessage response = APIHelper.APIPostAsync(dto, apiUrl);
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");
            return result;
        }

        public ActionResult GetMetricName(string plantUid, string bgUid, string funplantUid)
        {
            var apiUrl = string.Format("OEE/GetMetricNameAPI?plantUid={0}&bgUid={1}&funplantUid={2}", int.Parse(plantUid), int.Parse(bgUid), string.IsNullOrEmpty(funplantUid) ? 0 : int.Parse(funplantUid));
            HttpResponseMessage response = APIHelper.APIGetAsync(apiUrl);
            var result = response.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #region improvementPlan

        public ActionResult OEE_ImprovementPlan()
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
            return View("OEE_ImprovementPlan", currentVM);
        }

        public string SaveImprovementPlan(OEE_ImprovementPlanDTO vm)
        {
            vm.Created_UID = this.CurrentUser.AccountUId;
            vm.Modified_UID = this.CurrentUser.AccountUId;
            vm.Modified_Date = DateTime.Now;
            vm.Commit_Date = DateTime.Now;
            vm.Close_Date = null;
            vm.Created_Date = null;
            vm.DirDueDate = null;
            if (vm.Status == 1)
            {
                vm.Created_Date = DateTime.Now;
            }
            else if (vm.Status == 4)
            {
                vm.Close_Date = DateTime.Now;
            }

            var url = string.Format("OEE/AddImprovementPlan");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string UpdateImprovementPlan(OEE_ImprovementPlanDTO vm)
        {
            vm.Created_UID = this.CurrentUser.AccountUId;
            vm.Modified_UID = this.CurrentUser.AccountUId;
            vm.Modified_Date = DateTime.Now;
            vm.Commit_Date = DateTime.Now;
            vm.Close_Date = null;
            vm.Created_Date = null;
            if (vm.Status == 1)
            {
                vm.Created_Date = DateTime.Now;
            }
            else if (vm.Status == 4)
            {
                vm.Close_Date = DateTime.Now;
            }

            var url = string.Format("OEE/UpdateImprovementPlanAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(vm, url);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return result;
        }

        public ActionResult GetImprovementPlan(OEE_ImprovementPlanDTO search, Page page)
        {
            if (search.Plant_Organization_UID == 0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }

            var apiUrl = string.Format("OEE/GetImprovementPlanAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetImprovementPlanById(int improvementId)
        {
            var apiUrl = string.Format("OEE/GetImprovementPlanByIdAPI?improvementId={0}", improvementId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteImpeovementPlanById(int improvementId)
        {
            var apiUrl = string.Format("OEE/DeleteImpeovementPlanByIdAPI?improvementId={0}", improvementId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        #endregion

        #endregion


        #region OEE_RealStatus

        public ActionResult OEE_RealStatus()
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
            currentVM.OptypeID = optypeID;
            currentVM.FunPlantID = funPlantID;

            var OEE_PieCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_RealStatusSerchParam);
            if (OEE_PieCookie != null)
            {
                ViewBag.Plant_UID = OEE_PieCookie["Plant_UID"];
                ViewBag.BG_UID = OEE_PieCookie["BG_UID"];
                ViewBag.customerId = OEE_PieCookie["customerId"];
                ViewBag.lineName = OEE_PieCookie["lineName"];
                ViewBag.stationName = OEE_PieCookie["stationName"];
                ViewBag.MachineName = OEE_PieCookie["MachineName"];
                ViewBag.ShiftID = OEE_PieCookie["ShiftID"];
                ViewBag.startTime = OEE_PieCookie["startTime"];
                ViewBag.endTime = OEE_PieCookie["startTime"];
            }
            return View("OEE_RealStatus", currentVM);
        }

        public ActionResult QueryRealStatusReport(OEE_ReprortSearchModel search, Page page)
        {
            var MetricsCooike = System.Web.HttpContext.Current.Request.Cookies.Get(SessionConstants.OEE_RealStatusSerchParam);
            if (MetricsCooike != null)
            {
                MetricsCooike["Plant_UID"] = search.Plant_Organization_UID.ToString();
                MetricsCooike["BG_UID"] = search.BG_Organization_UID.ToString();
                MetricsCooike["customerId"] = search.CustomerID.ToString();
                MetricsCooike["lineName"] = search.LineID.ToString();
                MetricsCooike["stationName"] = search.StationID.ToString();
                MetricsCooike["MachineName"] = search.EQP_Uid.ToString();
                MetricsCooike["ShiftID"] = search.ShiftTimeID.ToString();
                MetricsCooike["startTime"] = search.StartTime.ToString("yyyy-MM-dd");
                MetricsCooike["endTime"] = search.EndTime.ToString("yyyy-MM-dd");
                MetricsCooike["isFromPhotoReport"] = "false";
                MetricsCooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(MetricsCooike);
            }
            else
            {
                var NewMetricsCooike = new HttpCookie(SessionConstants.OEE_RealStatusSerchParam);
                NewMetricsCooike["Plant_UID"] = search.Plant_Organization_UID.ToString();
                NewMetricsCooike["BG_UID"] = search.BG_Organization_UID.ToString();
                NewMetricsCooike["customerId"] = search.CustomerID.ToString();
                NewMetricsCooike["lineName"] = search.LineID.ToString();
                NewMetricsCooike["stationName"] = search.StationID.ToString();
                NewMetricsCooike["MachineName"] = search.EQP_Uid.ToString();
                NewMetricsCooike["ShiftID"] = search.ShiftTimeID.ToString();
                NewMetricsCooike["startTime"] = search.StartTime.ToString("yyyy-MM-dd");
                NewMetricsCooike["endTime"] = search.EndTime.ToString("yyyy-MM-dd");
                NewMetricsCooike["isFromPhotoReport"] = "false";
                NewMetricsCooike.Expires.AddDays(30);
                HttpContext.Response.SetCookie(NewMetricsCooike);
            }


            search.languageID = PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID;
            var apiUrl = string.Format("OEE/QueryRealStatusReportAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrentDateTime() {
            var apiUrl = string.Format("OEE/GetCurrentDateTimeAPI");
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
    }


    #endregion
    public static class EPPLUSHelper
    {
        public static void SetTrueColumnWidth(this ExcelColumn column, double width)
        {
            // Deduce what the column width would really get set to.
            var z = width >= (1 + 2 / 3)
                ? Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2)
                : Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);

            // How far off? (will be less than 1)
            var errorAmt = width - z;

            // Calculate what amount to tack onto the original amount to result in the closest possible setting.
            var adj = width >= 1 + 2 / 3
                ? Math.Round(7 * errorAmt - 7 / 256, 0) / 7
                : Math.Round(12 * errorAmt - 12 / 256, 0) / 12 + (2 / 12);

            // Set width to a scaled-value that should result in the nearest possible value to the true desired setting.
            if (z > 0)
            {
                column.Width = width + adj;
                return;
            }

            column.Width = 0d;
        }
    }


}