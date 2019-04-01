using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class CNCMachineController : WebControllerBase
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

        public ActionResult CNCMachine()
        {
            CNCMachineVM currentVM = new CNCMachineVM();
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
            return View("CNCMachine", currentVM);
        }

        public ActionResult QueryCNCMachineDTOs(CNCMachineDTO search, Page page)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("CNCMachine/QueryCNCMachineDTOsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditCNCMachine(CNCMachineDTO dto)
        {
            dto.Modified_UID = CurrentUser.AccountUId;
            dto.Modified_Date = DateTime.Now;
            var apiUrl = string.Format("CNCMachine/AddOrEditCNCMachineAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(dto, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult QueryCNCMachineDTOByUid(int CNCMachineUID)
        {
            var apiUrl = string.Format("CNCMachine/QueryCNCMachineDTOByUidAPI?CNCMachineUID={0}", CNCMachineUID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetSystemProjectDTO(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var apiUrl = string.Format("CNCMachine/GetSystemProjectDTOAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", Plant_Organization_UID, BG_Organization_UID);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public string DeleteCNCMachine(int CNCMachineUID)
        {
            var apiUrl = string.Format("CNCMachine/DeleteCNCMachineAPI?CNCMachineUID={0}&userid={1}", CNCMachineUID, CurrentUser.AccountUId);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            return responMessage.Content.ReadAsStringAsync().Result;
        }
        public ActionResult DoAllExportCNCMachineReprot(CNCMachineDTO search)
        {

            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("CNCMachine/DoAllExportCNCMachineReprotAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<CNCMachineDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CNCMachine");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂区", "OP类型", "功能厂", "专案", "机台EMT号", "机台号", "机台名称", "是否启用", "最后更新者", "最后更新时间" };

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("CNCMachine");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.ProjectName;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Machine_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Enable ? "Y" : "N";
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifyer;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        //导出excel----by勾选的项目
        public ActionResult DoExportCNCMachineReprot(string uids)
        {
            //get Export datas
            var apiUrl = string.Format("CNCMachine/DoExportCNCMachineReprotAPI?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<CNCMachineDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CNCMachine");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var stringHeads = new string[] { "排序", "厂区", "OP类型", "功能厂", "专案", "机台EMT号", "机台号", "机台名称", "是否启用", "最后更新者", "最后更新时间" };


            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("CNCMachine");

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
                    worksheet.Cells[index + 2, 5].Value = currentRecord.ProjectName;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.Equipment;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Machine_ID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Machine_Name;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Is_Enable ? "Y" : "N";
                    worksheet.Cells[index + 2, 10].Value = currentRecord.Modifyer;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.Modified_Date == null ? "" : ((DateTime)currentRecord.Modified_Date).ToString(FormatConstants.DateTimeFormatString);

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }
        public string ImportCNCMachine(HttpPostedFileBase uploadName)
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
                            "专案",
                            "机台EMT号",
                            "机台代码",
                            "机台名称",
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
                        //获得所有ORGBOMLIST
                        var orgbomapiUrl = string.Format("Fixture/GetAllOrgBomAPI");
                        HttpResponseMessage orgbomMessage = APIHelper.APIGetAsync(orgbomapiUrl);
                        var jsonresult = orgbomMessage.Content.ReadAsStringAsync().Result;
                        var orgboms = JsonConvert.DeserializeObject<List<OrgBomDTO>>(jsonresult);

                        //获取专案
                        var apiUrlProjectDTO = string.Format("CNCMachine/GetSystemProjectDTOAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}", 0, 0);
                        var responMessageProjectDTO = APIHelper.APIGetAsync(apiUrlProjectDTO);
                        var resultProjectDTO = responMessageProjectDTO.Content.ReadAsStringAsync().Result;
                        var projectDTOs = JsonConvert.DeserializeObject<List<SystemProjectDTO>>(resultProjectDTO);


                        //获取设备
                        var apiUrlEquipmentInfoDTO = string.Format("CNCMachine/GetAllEquipmentInfoDTOsAPI");
                        var responMessageEquipmentInfoDTO = APIHelper.APIGetAsync(apiUrlEquipmentInfoDTO);
                        var resultEquipmentInfoDTO = responMessageEquipmentInfoDTO.Content.ReadAsStringAsync().Result;
                        var equipmentInfoDTOs = JsonConvert.DeserializeObject<List<EquipmentInfoDTO>>(resultEquipmentInfoDTO);


                        //获取所有已添加的机台
                        var apiUrlCNCMachineDTO = string.Format("CNCMachine/GetAllCNCMachineDTOListAPI");
                        var responMessageCNCMachineDTO = APIHelper.APIGetAsync(apiUrlCNCMachineDTO);
                        var resultCNCMachineDTO = responMessageCNCMachineDTO.Content.ReadAsStringAsync().Result;
                        var CNCMachineDTOs = JsonConvert.DeserializeObject<List<CNCMachineDTO>>(resultCNCMachineDTO);

                        var cNCMachinekDTOList = new List<CNCMachineDTO>();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var cNCMachinekDTO = new CNCMachineDTO();

                            int Plant_Organization_UID = 0;
                            int BG_Organization_UID = 0;
                            int? FunPlant_Organization_UID = null;
                            int Project_UID = 0;
                            int? EQP_Uid = null;

                            string Plant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "厂区")].Value).Trim();
                            string BG_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "OP类型")].Value);
                            string FunPlant_Organization = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "功能厂")].Value);
                            string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "专案")].Value);
                            string strIs_Enable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "是否启用")].Value);


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
                                var hasProject = projectDTOs.Where(m => m.Project_Name == Project_Name&&m.Organization_UID== BG_Organization_UID).FirstOrDefault();
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
                            string Machine_ID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台代码")].Value);

                            if (string.IsNullOrWhiteSpace(Machine_ID))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台代码没有值", i);
                                return errorInfo;
                            }
                            else if (Machine_ID.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台代码长度超过最大限定[20]", i);
                                return errorInfo;
                            }
                            string Machine_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台名称")].Value);

                            if (string.IsNullOrWhiteSpace(Machine_Name))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台名称没有值", i);
                                return errorInfo;
                            }
                            else if (Machine_Name.Length > 50)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行机台名称长度超过最大限定[50]", i);
                                return errorInfo;
                            }


                            string Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, GetColumnIndex(propertiesHead, "机台EMT号")].Value);

                            if (string.IsNullOrWhiteSpace(Equipment))
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行SAP设备编号没有值", i);
                                return errorInfo;
                            }
                            else if (Equipment.Length > 20)
                            {
                                isExcelError = true;
                                errorInfo = string.Format("第{0}行SAP设备编号长度超过最大限定[20]", i);
                                return errorInfo;
                            }

                            var equipmentInfoDTO = equipmentInfoDTOs.FirstOrDefault(o => o.Project_UID == Project_UID && o.Organization_UID == BG_Organization_UID && o.Equipment == Equipment);
                            if (equipmentInfoDTO != null)
                            {
                                EQP_Uid = equipmentInfoDTO.EQP_Uid;
                            }


                            //导入数据判重
                            //var isSelfRepeated = cNCMachinekDTOList.Exists(m => m.Machine_Name == Machine_Name && m.Plant_Organization_UID == Plant_Organization_UID);
                            //if (isSelfRepeated)
                            //{
                            //    isExcelError = true;
                            //    errorInfo = string.Format("第{0}行在导入数据中重复,不可重复导入", i);
                            //    return errorInfo;
                            //}

                            var CNCMachineDTO = CNCMachineDTOs.FirstOrDefault(o => o.Plant_Organization_UID == Plant_Organization_UID && o.Machine_Name == Machine_Name);
                            if (CNCMachineDTO != null)
                            {
                                cNCMachinekDTO.CNCMachineUID = CNCMachineDTO.CNCMachineUID;
                            }

                            cNCMachinekDTO.Plant_Organization_UID = Plant_Organization_UID;
                            cNCMachinekDTO.BG_Organization_UID = BG_Organization_UID;
                            cNCMachinekDTO.EQP_Uid = EQP_Uid;
                            cNCMachinekDTO.Machine_Name = Machine_Name;
                            cNCMachinekDTO.Machine_ID = Machine_ID;
                            cNCMachinekDTO.Project_UID = Project_UID;
                            cNCMachinekDTO.Is_Enable = strIs_Enable == "Y" ? true : false;
                            cNCMachinekDTO.Modified_Date = DateTime.Now;
                            cNCMachinekDTO.Modified_UID = CurrentUser.AccountUId;
                            cNCMachinekDTOList.Add(cNCMachinekDTO);

                        }
                        //插入表
                        var json = JsonConvert.SerializeObject(cNCMachinekDTOList);
                        var apiInsertMachineDUrl = string.Format("CNCMachine/ImportMachineAPI");
                        HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertMachineDUrl);
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
        public ActionResult CNCMachineReport()
        {
            CNCMachineReportVM currentVM = new CNCMachineReportVM();
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

            var systemViewColumnDTOs = GetSystemViewColumnDTOs();

            var apiUrlColumnTableDTO = string.Format("CNCMachine/GetCNCMachineColumnTableDTOsAPI?Account_UID={0}",this.CurrentUser.AccountUId);
            HttpResponseMessage responMessageColumnTableDTO = APIHelper.APIGetAsync(apiUrlColumnTableDTO);
            var resultColumnTableDTO = responMessageColumnTableDTO.Content.ReadAsStringAsync().Result;
            var listColumnTableDTO = JsonConvert.DeserializeObject<List<CNCMachineColumnTableDTO>>(resultColumnTableDTO).ToList();

            if (listColumnTableDTO != null && listColumnTableDTO.Count > 0)
            {
                List<SystemViewColumnDTO> newSystemViewColumnDTO = new List<SystemViewColumnDTO>();
                foreach (var item in systemViewColumnDTOs)
                {
                    var ColumnTableDTO = listColumnTableDTO.FirstOrDefault(o => o.Column_Name == item.View_Name);
                    if (ColumnTableDTO != null)
                    {
                        item.isChecked = true;
                    }
                    else
                    {
                        item.isChecked = false;
                    }
                    newSystemViewColumnDTO.Add(item);
                }
                systemViewColumnDTOs = newSystemViewColumnDTO;
            }
            else
            {
                //string InsertMachineColumnTableAPI(dynamic json)
                List<CNCMachineColumnTableDTO> CNCMachineColumnTableDTOs = new List<CNCMachineColumnTableDTO>();
                foreach (var item in systemViewColumnDTOs)
                {
                    CNCMachineColumnTableDTO ColumnTableDTO = new CNCMachineColumnTableDTO();
                    ColumnTableDTO.Column_Name = item.View_Name;
                    ColumnTableDTO.NTID = this.CurrentUser.AccountUId;
                    ColumnTableDTO.Modified_Date = DateTime.Now;

                    CNCMachineColumnTableDTOs.Add(ColumnTableDTO);

                }
                var json = JsonConvert.SerializeObject(CNCMachineColumnTableDTOs);
                var apiInsertMachineDUrl = string.Format("CNCMachine/InsertMachineColumnTableAPI");
                HttpResponseMessage responSettingMessage = APIHelper.APIPostAsync(json, apiInsertMachineDUrl);


            }


            // currentVM.ColumnDTOList = GetSystemViewColumnDTOs();

            currentVM.ColumnDTOList = systemViewColumnDTOs;
            //ViewBag.PageTitle = "异常原因维护";
            return View("CNCMachineReport", currentVM);
        }

        /// <summary>
        /// 初始化创建所有要选择的栏位
        /// </summary>
        /// <returns></returns>
        public List<SystemViewColumnDTO> GetSystemViewColumnDTOs()
        {
            List<SystemViewColumnDTO> systemViewColumnDTOs=new List<SystemViewColumnDTO>();
            CNCMachineReportDTO model = new CNCMachineReportDTO();
            Type ColumnDTOtype = model.GetType();
            PropertyInfo[] PropertyColumnList = ColumnDTOtype.GetProperties();
            int i = 0;
            foreach (PropertyInfo item in PropertyColumnList)
            {
                i++;
                SystemViewColumnDTO systemViewColumnDTO = new SystemViewColumnDTO();
                systemViewColumnDTO.Column_UID = i;
                systemViewColumnDTO.View_Column_Index = i;
                systemViewColumnDTO.isChecked = true;
                systemViewColumnDTO.View_Name = item.Name;         
                systemViewColumnDTO.View_Group = "CNC机台报表";
                systemViewColumnDTO.new_index = i;          
                if ((item.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)).Length < 1) continue;
                systemViewColumnDTO.View_Column_Name = ((System.ComponentModel.DescriptionAttribute[])item.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false))[0].Description;
                systemViewColumnDTOs.Add(systemViewColumnDTO);
           
            }

            return systemViewColumnDTOs;
        }
        
        public ActionResult QueryReportCNCMachineDatas(CNCMachineDTO search, Page page)
        {
            if(search.Plant_Organization_UID==0)
            {
                search.Plant_Organization_UID = GetPlantOrgUid();
            }
            if (search.Plant_Organization_UID ==0)
            {
             
                return Content("", "application/json");
            }
            else
            {
                var apiUrl = string.Format("CNCMachine/QueryReportCNCMachineDatasAPI");
                HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
                var result = responMessage.Content.ReadAsStringAsync().Result;
                return Content(result, "application/json");
            }
            //var apiUrl = string.Format("CNCMachine/QueryReportCNCMachineDatasAPI");
            //HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, page, apiUrl);
            //var result = responMessage.Content.ReadAsStringAsync().Result;
            //return Content(result, "application/json");
        }
        public ActionResult DoAllExportMachineReport(CNCMachineDTO search)
        {
            search.Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("CNCMachine/DoAllExportMachineReportAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<CNCMachineReportDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CNCMachineReport");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //var stringHeads = new string[] { "排序", "机台名称", "机台编号", "机台SN号", "機台開機", "關機時間(啟/停)", "CNC型号", "CNC类型", "CNC序列号",
            //    "CNC版本号", "轴名称","开机时间(Min)","切削时间","循环时间","当前刀具号","最大控制轴数","当前控制轴数","CNC模式","主程序号","当前程序号",
            //    "程序段号","单段备注","程序备注","系统IP地址","主轴指定速度","实际速度","主軸溫度","主軸扭力" ,"主軸熱伸長","宏變量", "切削液狀態","主轴誤差",
            //    "伺服温度" ,"主轴负载","切削指定速度","实际速度","倍率（快移）","倍率（切削）" ,"倍率（HAND）","倍率（主轴）","快速进给时间常数","快移速度",
            //    "每次加工起始時間" ,"工件加工C/T","X軸的負載" ,"Y軸的負載","Z軸的負載","A軸的負載","主軸轉速（S）","進給(F)"  ,"四軸角度","對刀儀" };

            var stringHeads = new string[] {   "排序","机台名称", "機台狀態","工件加工C/T(sec)","加工工件數", "X軸的負載(%)","Y軸的負載(%)", "Z軸的負載(%)","A軸的負載(%)","刀具壽命(次)", "主軸轉速(rpm)",
                         "進給(F)(毫米/分)", "四軸角度", "宏變量", "主轴誤差","轴名称", "主轴负载(%)","开机时间(Min)","切削时间(Min)","循环时间(sec)","当前刀具号", "报警号", "报警信息", "倍率（快移）",
                         "倍率（切削）", "倍率（JOG）",  "倍率（HAND）", "倍率（主轴）", "快速进给时间常数","快移速度", "最大控制轴数", "CNC模式",   "主程序号",  "当前程序号",  "程序段号",
                         "单段备注","程序备注",  "主轴指定速度(rpm)", "主轴实际速度(rpm)", "系统IP地址","CNC型号","CNC类型", "CNC序列号", "CNC版本号", "Customer ID / Name","Site","Building",
                         "Product ID","Process name", "Operator ID", "伺服温度(℃)",  "主軸溫度(℃)", "主軸扭力", "停機時主軸原始座標X / Y / Z / A","切削液狀態", "切削指定速度", "伺服实际速度",
                         "当前控制轴数","报警类型" , "對刀儀", "主軸熱伸長(mm)","每次加工起始時間", "伺服负载(%)"};

            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("CNCMachineReport");

                //set Title
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                //set cell value
                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = currentRecord.MachineName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.ProcessCycleTm;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.TotalNum;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.X_load;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Y_load;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Z_load;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.A_load;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.ToolSum;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.S_RPM;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Feed;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Axisangle;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Mcode;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.S_Gap;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.ServoName;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.S_Load;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.PowerOnTm;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.CuttingTm;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.CycleRunTm;
                    worksheet.Cells[index + 2, 21].Value = currentRecord.CurrenttoolNo;
                    worksheet.Cells[index + 2, 22].Value = currentRecord.ErrorCode;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.ErrorDels;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.RateJOG;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.RateFastmove;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.RateJOG1;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.RateHAND;
                    worksheet.Cells[index + 2, 28].Value = currentRecord.RateS;
                    worksheet.Cells[index + 2, 29].Value = currentRecord.FastmoveTmC;
                    worksheet.Cells[index + 2, 30].Value = currentRecord.Fastmovespeed;
                    worksheet.Cells[index + 2, 31].Value = currentRecord.MaxAxisNo;
                    worksheet.Cells[index + 2, 32].Value = currentRecord.CNCMode;
                    worksheet.Cells[index + 2, 33].Value = currentRecord.MainProgram;
                    worksheet.Cells[index + 2, 34].Value = currentRecord.NowProgram;
                    worksheet.Cells[index + 2, 35].Value = currentRecord.ProgramStep;
                    worksheet.Cells[index + 2, 36].Value = currentRecord.Singlenote;
                    worksheet.Cells[index + 2, 37].Value = currentRecord.Programnote;
                    worksheet.Cells[index + 2, 38].Value = currentRecord.S_SetSpeed;
                    worksheet.Cells[index + 2, 39].Value = currentRecord.S_Speed;
                    worksheet.Cells[index + 2, 40].Value = currentRecord.IPaddress;
                    worksheet.Cells[index + 2, 41].Value = currentRecord.CNCModel;
                    worksheet.Cells[index + 2, 42].Value = currentRecord.CNCType;
                    worksheet.Cells[index + 2, 43].Value = currentRecord.CNCSequence;
                    worksheet.Cells[index + 2, 44].Value = currentRecord.CNCVersion;
                    worksheet.Cells[index + 2, 45].Value = currentRecord.Customer;
                    worksheet.Cells[index + 2, 46].Value = currentRecord.Site;
                    worksheet.Cells[index + 2, 47].Value = currentRecord.Building;
                    worksheet.Cells[index + 2, 48].Value = currentRecord.ProductID;
                    worksheet.Cells[index + 2, 49].Value = currentRecord.ProcessName;
                    worksheet.Cells[index + 2, 50].Value = currentRecord.OperatorID;
                    worksheet.Cells[index + 2, 51].Value = currentRecord.ServoTemp;
                    worksheet.Cells[index + 2, 52].Value = currentRecord.S_Temp;
                    worksheet.Cells[index + 2, 53].Value = currentRecord.S_Torque;
                    worksheet.Cells[index + 2, 54].Value = currentRecord.XYZA_load;
                    worksheet.Cells[index + 2, 55].Value = currentRecord.Cuttingfluid;
                    worksheet.Cells[index + 2, 56].Value = currentRecord.CuttingSpeed;
                    worksheet.Cells[index + 2, 57].Value = currentRecord.ActualSpeed;
                    worksheet.Cells[index + 2, 58].Value = currentRecord.UseAxisNo;
                    worksheet.Cells[index + 2, 59].Value = currentRecord.ErrorType;
                    worksheet.Cells[index + 2, 60].Value = currentRecord.Toolset;
                    worksheet.Cells[index + 2, 61].Value = currentRecord.S_TE;
                    worksheet.Cells[index + 2, 62].Value = currentRecord.ProcessStartTm;
                    worksheet.Cells[index + 2, 63].Value = currentRecord.SF_Load;


                    #region 
                    //worksheet.Cells[index + 2, 1].Value = index + 1;
                    //worksheet.Cells[index + 2, 2].Value = currentRecord.MachineName;
                    //worksheet.Cells[index + 2, 3].Value = currentRecord.AuthCode;
                    //worksheet.Cells[index + 2, 4].Value = currentRecord.dSn;
                    //worksheet.Cells[index + 2, 5].Value = currentRecord.PowerOn;
                    //worksheet.Cells[index + 2, 6].Value = currentRecord.ShutdownTm;
                    //worksheet.Cells[index + 2, 7].Value = currentRecord.CNCModel;
                    //worksheet.Cells[index + 2, 8].Value = currentRecord.CNCType;
                    //worksheet.Cells[index + 2, 9].Value = currentRecord.CNCSequence;
                    //worksheet.Cells[index + 2, 10].Value = currentRecord.CNCVersion;
                    //worksheet.Cells[index + 2, 11].Value = currentRecord.ServoName;
                    //worksheet.Cells[index + 2, 12].Value = currentRecord.PowerOnTm;
                    //worksheet.Cells[index + 2, 13].Value = currentRecord.CuttingTm;
                    //worksheet.Cells[index + 2, 14].Value = currentRecord.CycleRunTm;
                    //worksheet.Cells[index + 2, 15].Value = currentRecord.CurrenttoolNo;
                    //worksheet.Cells[index + 2, 16].Value = currentRecord.CurrenttoolNo;
                    //worksheet.Cells[index + 2, 17].Value = currentRecord.MaxAxisNo;
                    //worksheet.Cells[index + 2, 18].Value = currentRecord.UseAxisNo;
                    //worksheet.Cells[index + 2, 19].Value = currentRecord.MainProgram;
                    //worksheet.Cells[index + 2, 20].Value = currentRecord.NowProgram;
                    //worksheet.Cells[index + 2, 21].Value = currentRecord.ProgramStep;
                    //worksheet.Cells[index + 2, 22].Value = currentRecord.Singlenote;
                    //worksheet.Cells[index + 2, 23].Value = currentRecord.Programnote;
                    //worksheet.Cells[index + 2, 24].Value = currentRecord.IPaddress;
                    //worksheet.Cells[index + 2, 25].Value = currentRecord.S_SetSpeed;
                    //worksheet.Cells[index + 2, 26].Value = currentRecord.S_Speed;
                    //worksheet.Cells[index + 2, 27].Value = currentRecord.S_Temp;
                    //worksheet.Cells[index + 2, 28].Value = currentRecord.S_Torque;
                    //worksheet.Cells[index + 2, 29].Value = currentRecord.S_TE;
                    //worksheet.Cells[index + 2, 30].Value = currentRecord.Mcode;
                    //worksheet.Cells[index + 2, 31].Value = currentRecord.Cuttingfluid;
                    //worksheet.Cells[index + 2, 32].Value = currentRecord.S_Gap;
                    //worksheet.Cells[index + 2, 33].Value = currentRecord.ServoTemp;
                    //worksheet.Cells[index + 2, 34].Value = currentRecord.CuttingSpeed;
                    //worksheet.Cells[index + 2, 35].Value = currentRecord.ActualSpeed;
                    //worksheet.Cells[index + 2, 36].Value = currentRecord.RateJOG;
                    //worksheet.Cells[index + 2, 37].Value = currentRecord.RateFastmove;
                    //worksheet.Cells[index + 2, 38].Value = currentRecord.RateHAND;
                    //worksheet.Cells[index + 2, 39].Value = currentRecord.RateS;
                    //worksheet.Cells[index + 2, 40].Value = currentRecord.FastmoveTmC;
                    //worksheet.Cells[index + 2, 41].Value = currentRecord.Fastmovespeed;
                    //worksheet.Cells[index + 2, 42].Value = currentRecord.ProcessStartTm;
                    //worksheet.Cells[index + 2, 43].Value = currentRecord.ProcessCycleTm;
                    //worksheet.Cells[index + 2, 44].Value = currentRecord.X_load;
                    //worksheet.Cells[index + 2, 45].Value = currentRecord.Y_load;
                    //worksheet.Cells[index + 2, 46].Value = currentRecord.Z_load;
                    //worksheet.Cells[index + 2, 47].Value = currentRecord.A_load;
                    //worksheet.Cells[index + 2, 48].Value = currentRecord.S_RPM;
                    //worksheet.Cells[index + 2, 49].Value = currentRecord.Feed;
                    //worksheet.Cells[index + 2, 50].Value = currentRecord.Axisangle;
                    //worksheet.Cells[index + 2, 51].Value = currentRecord.Toolset;  
                    #endregion
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        public ActionResult UpdateColumnInfo(string Column_Name, bool isDisplay)
        {
            var apiUrl = string.Format("CNCMachine/UpdateColumnInfoAPI?Account_UID={0}&Column_Name={1}&isDisplay={2}",
            this.CurrentUser.AccountUId, Column_Name, isDisplay);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult GetCNCMachineList(int Plant_Organization_UID = 0, int BG_Organization_UID = 0, int FunPlant_Organization_UID = 0)
        {

            if (Plant_Organization_UID == 0)
            {
                Plant_Organization_UID = GetPlantOrgUid();
            }
            var apiUrl = string.Format("CNCMachine/GetCNCMachineListAPI?Plant_Organization_UID={0}&BG_Organization_UID={1}&FunPlant_Organization_UID={2}", Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }
        public ActionResult DoHisExportMachineReport(string Machine_Name,DateTime? Date_From ,DateTime? Date_To)
        {
            var Plant_Organization_UID = GetPlantOrgUid();
            var apiUrl = string.Format("CNCMachine/DoHisExportMachineReportAPI?Plant_Organization_UID={0}&Machine_Name={1}&Date_From={2}&Date_To={3}", Plant_Organization_UID, Machine_Name, Date_From, Date_To);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync( apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<CNCMachineHisReportDTO>>(result).ToList();

            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("CNCMachineHisReport");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            //var stringHeads = new string[] { "排序", "机台名称", "机台编号", "机台SN号", "機台開機", "關機時間(啟/停)", "CNC型号", "CNC类型", "CNC序列号",
            //    "CNC版本号", "轴名称","开机时间(Min)","切削时间","循环时间","当前刀具号","最大控制轴数","当前控制轴数","CNC模式","主程序号","当前程序号",
            //    "程序段号","单段备注","程序备注","系统IP地址","主轴指定速度","实际速度","主軸溫度","主軸扭力" ,"主軸熱伸長","宏變量", "切削液狀態","主轴誤差",
            //    "伺服温度" ,"主轴负载","切削指定速度","实际速度","倍率（快移）","倍率（切削）" ,"倍率（HAND）","倍率（主轴）","快速进给时间常数","快移速度",
            //    "每次加工起始時間" ,"工件加工C/T","X軸的負載" ,"Y軸的負載","Z軸的負載","A軸的負載","主軸轉速（S）","進給(F)"  ,"四軸角度","對刀儀" ,"分组类型","创建时间"};
    
            var stringHeads = new string[] {   "排序","机台名称", "機台狀態","工件加工C/T(sec)","加工工件數", "X軸的負載(%)","Y軸的負載(%)", "Z軸的負載(%)","A軸的負載(%)","刀具壽命(次)", "主軸轉速(rpm)",
                         "進給(F)(毫米/分)", "四軸角度", "宏變量", "主轴誤差","轴名称", "主轴负载(%)","开机时间(Min)","切削时间(Min)","循环时间(sec)","当前刀具号", "报警号", "报警信息", "倍率（快移）",
                         "倍率（切削）", "倍率（JOG）",  "倍率（HAND）", "倍率（主轴）", "快速进给时间常数","快移速度", "最大控制轴数", "CNC模式",   "主程序号",  "当前程序号",  "程序段号",
                         "单段备注","程序备注",  "主轴指定速度(rpm)", "主轴实际速度(rpm)", "系统IP地址","CNC型号","CNC类型", "CNC序列号", "CNC版本号", "Customer ID / Name","Site","Building",
                         "Product ID","Process name", "Operator ID", "伺服温度(℃)",  "主軸溫度(℃)", "主軸扭力", "停機時主軸原始座標X / Y / Z / A","切削液狀態", "切削指定速度", "伺服实际速度",
                         "当前控制轴数","报警类型" , "對刀儀", "主軸熱伸長(mm)","每次加工起始時間", "伺服负载(%)","分组类型","创建时间"};
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("CNCMachineHisReport");

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
                    worksheet.Cells[index + 2, 2].Value = currentRecord.MachineName;
                    worksheet.Cells[index + 2, 3].Value = currentRecord.Status;
                    worksheet.Cells[index + 2, 4].Value = currentRecord.ProcessCycleTm;
                    worksheet.Cells[index + 2, 5].Value = currentRecord.TotalNum;
                    worksheet.Cells[index + 2, 6].Value = currentRecord.X_load;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.Y_load;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Z_load;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.A_load;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.ToolSum;
                    worksheet.Cells[index + 2, 11].Value = currentRecord.S_RPM;
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Feed;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Axisangle;
                    worksheet.Cells[index + 2, 14].Value = currentRecord.Mcode;
                    worksheet.Cells[index + 2, 15].Value = currentRecord.S_Gap;
                    worksheet.Cells[index + 2, 16].Value = currentRecord.ServoName;
                    worksheet.Cells[index + 2, 17].Value = currentRecord.S_Load;
                    worksheet.Cells[index + 2, 18].Value = currentRecord.PowerOnTm;
                    worksheet.Cells[index + 2, 19].Value = currentRecord.CuttingTm;
                    worksheet.Cells[index + 2, 20].Value = currentRecord.CycleRunTm;
                    worksheet.Cells[index + 2, 21].Value = currentRecord.CurrenttoolNo;
                    worksheet.Cells[index + 2, 22].Value = currentRecord.ErrorCode;
                    worksheet.Cells[index + 2, 23].Value = currentRecord.ErrorDels;
                    worksheet.Cells[index + 2, 24].Value = currentRecord.RateJOG;
                    worksheet.Cells[index + 2, 25].Value = currentRecord.RateFastmove;
                    worksheet.Cells[index + 2, 26].Value = currentRecord.RateJOG1;
                    worksheet.Cells[index + 2, 27].Value = currentRecord.RateHAND;
                    worksheet.Cells[index + 2, 28].Value = currentRecord.RateS;
                    worksheet.Cells[index + 2, 29].Value = currentRecord.FastmoveTmC;
                    worksheet.Cells[index + 2, 30].Value = currentRecord.Fastmovespeed;
                    worksheet.Cells[index + 2, 31].Value = currentRecord.MaxAxisNo;
                    worksheet.Cells[index + 2, 32].Value = currentRecord.CNCMode;
                    worksheet.Cells[index + 2, 33].Value = currentRecord.MainProgram;
                    worksheet.Cells[index + 2, 34].Value = currentRecord.NowProgram;
                    worksheet.Cells[index + 2, 35].Value = currentRecord.ProgramStep;
                    worksheet.Cells[index + 2, 36].Value = currentRecord.Singlenote;
                    worksheet.Cells[index + 2, 37].Value = currentRecord.Programnote;
                    worksheet.Cells[index + 2, 38].Value = currentRecord.S_SetSpeed;
                    worksheet.Cells[index + 2, 39].Value = currentRecord.S_Speed;
                    worksheet.Cells[index + 2, 40].Value = currentRecord.IPaddress;
                    worksheet.Cells[index + 2, 41].Value = currentRecord.CNCModel;
                    worksheet.Cells[index + 2, 42].Value = currentRecord.CNCType;
                    worksheet.Cells[index + 2, 43].Value = currentRecord.CNCSequence;
                    worksheet.Cells[index + 2, 44].Value = currentRecord.CNCVersion;
                    worksheet.Cells[index + 2, 45].Value = currentRecord.Customer;
                    worksheet.Cells[index + 2, 46].Value = currentRecord.Site;
                    worksheet.Cells[index + 2, 47].Value = currentRecord.Building;
                    worksheet.Cells[index + 2, 48].Value = currentRecord.ProductID;
                    worksheet.Cells[index + 2, 49].Value = currentRecord.ProcessName;
                    worksheet.Cells[index + 2, 50].Value = currentRecord.OperatorID;
                    worksheet.Cells[index + 2, 51].Value = currentRecord.ServoTemp;
                    worksheet.Cells[index + 2, 52].Value = currentRecord.S_Temp;
                    worksheet.Cells[index + 2, 53].Value = currentRecord.S_Torque;
                    worksheet.Cells[index + 2, 54].Value = currentRecord.XYZA_load;
                    worksheet.Cells[index + 2, 55].Value = currentRecord.Cuttingfluid;
                    worksheet.Cells[index + 2, 56].Value = currentRecord.CuttingSpeed;
                    worksheet.Cells[index + 2, 57].Value = currentRecord.ActualSpeed;
                    worksheet.Cells[index + 2, 58].Value = currentRecord.UseAxisNo;
                    worksheet.Cells[index + 2, 59].Value = currentRecord.ErrorType;
                    worksheet.Cells[index + 2, 60].Value = currentRecord.Toolset;
                    worksheet.Cells[index + 2, 61].Value = currentRecord.S_TE;
                    worksheet.Cells[index + 2, 62].Value = currentRecord.ProcessStartTm;
                    worksheet.Cells[index + 2, 63].Value = currentRecord.SF_Load;
                    worksheet.Cells[index + 2, 64].Value = currentRecord.ScanType;
                    worksheet.Cells[index + 2, 65].Value = currentRecord.CreateTime == null ? "" : ((DateTime)currentRecord.CreateTime).ToString(FormatConstants.DateTimeFormatString);

                    #region 
                    ////seq
                    //worksheet.Cells[index + 2, 1].Value = index + 1;
                    //worksheet.Cells[index + 2, 2].Value = currentRecord.MachineName;
                    //worksheet.Cells[index + 2, 3].Value = currentRecord.AuthCode;
                    //worksheet.Cells[index + 2, 4].Value = currentRecord.dSn;
                    //worksheet.Cells[index + 2, 5].Value = currentRecord.PowerOn;
                    //worksheet.Cells[index + 2, 6].Value = currentRecord.ShutdownTm;
                    //worksheet.Cells[index + 2, 7].Value = currentRecord.CNCModel;
                    //worksheet.Cells[index + 2, 8].Value = currentRecord.CNCType;
                    //worksheet.Cells[index + 2, 9].Value = currentRecord.CNCSequence;
                    //worksheet.Cells[index + 2, 10].Value = currentRecord.CNCVersion;
                    //worksheet.Cells[index + 2, 11].Value = currentRecord.ServoName;
                    //worksheet.Cells[index + 2, 12].Value = currentRecord.PowerOnTm;
                    //worksheet.Cells[index + 2, 13].Value = currentRecord.CuttingTm;
                    //worksheet.Cells[index + 2, 14].Value = currentRecord.CycleRunTm;
                    //worksheet.Cells[index + 2, 15].Value = currentRecord.CurrenttoolNo;
                    //worksheet.Cells[index + 2, 16].Value = currentRecord.MaxAxisNo;
                    //worksheet.Cells[index + 2, 17].Value = currentRecord.UseAxisNo;
                    //worksheet.Cells[index + 2, 18].Value = currentRecord.CNCMode;
                    //worksheet.Cells[index + 2, 19].Value = currentRecord.MainProgram;
                    //worksheet.Cells[index + 2, 20].Value = currentRecord.NowProgram;
                    //worksheet.Cells[index + 2, 21].Value = currentRecord.ProgramStep;
                    //worksheet.Cells[index + 2, 22].Value = currentRecord.Singlenote;
                    //worksheet.Cells[index + 2, 23].Value = currentRecord.Programnote;
                    //worksheet.Cells[index + 2, 24].Value = currentRecord.IPaddress;
                    //worksheet.Cells[index + 2, 25].Value = currentRecord.S_SetSpeed;
                    //worksheet.Cells[index + 2, 26].Value = currentRecord.S_Speed;


                    //worksheet.Cells[index + 2, 27].Value = currentRecord.S_Temp;
                    //worksheet.Cells[index + 2, 28].Value = currentRecord.S_Torque;
                    //worksheet.Cells[index + 2, 29].Value = currentRecord.S_TE;
                    //worksheet.Cells[index + 2, 30].Value = currentRecord.Mcode;
                    //worksheet.Cells[index + 2, 31].Value = currentRecord.Cuttingfluid;
                    //worksheet.Cells[index + 2, 32].Value = currentRecord.S_Gap;
                    //worksheet.Cells[index + 2, 33].Value = currentRecord.ServoTemp;
                    //worksheet.Cells[index + 2, 34].Value = currentRecord.S_Load;              
                    //worksheet.Cells[index + 2, 35].Value = currentRecord.CuttingSpeed;
                    //worksheet.Cells[index + 2, 36].Value = currentRecord.ActualSpeed;
                    //worksheet.Cells[index + 2, 37].Value = currentRecord.RateJOG;
                    //worksheet.Cells[index + 2, 38].Value = currentRecord.RateFastmove;
                    //worksheet.Cells[index + 2, 39].Value = currentRecord.RateHAND;
                    //worksheet.Cells[index + 2, 40].Value = currentRecord.RateS;
                    //worksheet.Cells[index + 2, 41].Value = currentRecord.FastmoveTmC;
                    //worksheet.Cells[index + 2, 42].Value = currentRecord.Fastmovespeed;

                    //worksheet.Cells[index + 2, 43].Value = currentRecord.ProcessStartTm;
                    //worksheet.Cells[index + 2, 44].Value = currentRecord.ProcessCycleTm;
                    //worksheet.Cells[index + 2, 45].Value = currentRecord.X_load;
                    //worksheet.Cells[index + 2, 46].Value = currentRecord.Y_load;
                    //worksheet.Cells[index + 2, 47].Value = currentRecord.Z_load;
                    //worksheet.Cells[index + 2, 48].Value = currentRecord.A_load;
                    //worksheet.Cells[index + 2, 49].Value = currentRecord.S_RPM;
                    //worksheet.Cells[index + 2, 50].Value = currentRecord.Feed;
                    //worksheet.Cells[index + 2, 51].Value = currentRecord.Axisangle;
                    //worksheet.Cells[index + 2, 52].Value = currentRecord.Toolset;
                    //worksheet.Cells[index + 2, 53].Value = currentRecord.ScanType;
                    //worksheet.Cells[index + 2, 54].Value = currentRecord.CreateTime == null ? "" : ((DateTime)currentRecord.CreateTime).ToString(FormatConstants.DateTimeFormatString);

                    #endregion
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };

        }
        

    }
}