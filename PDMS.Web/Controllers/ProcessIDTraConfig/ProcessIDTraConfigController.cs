using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Core.BaseController;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels.ProcessIDTraConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace PDMS.Web.Controllers
{
    public class ProcessIDTraConfigController : WebControllerBase
    {
        public ActionResult ProcessConfig()
        {
            return View("ProcessIDTraConfig");
        }

        public int GetPlantOrgUid()
        {
            int plantorguid = 0;
            if (CurrentUser.GetUserInfo.OrgInfo != null && CurrentUser.GetUserInfo.OrgInfo.Count > 0 && CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID != null)
                plantorguid = (int)CurrentUser.GetUserInfo.OrgInfo[0].Plant_OrganizationUID;
            return plantorguid;
        }

        public ActionResult GetDemissionInfoByID(ProcessIDTraConfigVM searchParm, Page page)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/GetProcessIDConfigDataAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParm, page, apiUrl);
            //HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult AddOrEditProcessInfo(ProcessIDTransformConfigDTO editModel)
        {
            if (int.Parse(editModel.VM_IsEnabled) == 1)
            {
                editModel.IsEnabled = true;
            }
            else
            {
                editModel.IsEnabled = false;
            }
            if (int.Parse(editModel.VM_IsSyncNG) == 1)
            {
                editModel.IsSyncNG = true;
            }
            else
            {
                editModel.IsSyncNG = false;
            }

            var apiUrl = string.Format("ProcessIDTRSConfig/AddOrEditProcessInfoAPI");
            editModel.Modified_UID = CurrentUser.AccountUId;
            editModel.Modified_Date = DateTime.Now;
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(editModel, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public ActionResult DeleteProcessByUID(string Process_UID)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/DeleteProcessByUIDAPI?Process_UID={0}", Process_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            //if (result.Contains("SUCCESS"))
            //{
            //    result = "SUCCESS";
            //}
            //else
            //{
            //    result = "删除失败！";
            //}
            return Content(result, "application/json");
        }


        public string DeleteMES_PISProcessConfig(string json)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/DeleteMES_PISProcessConfigAPI?json={0}", json);
            HttpResponseMessage responddlMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responddlMessage.Content.ReadAsStringAsync().Result;
            result = result.Replace("\"", "");
            return result;
        }

        /// <summary>
        ///根据UID获取数据
        /// </summary>
        /// <param name="searchParm"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetProcessDataByUID(string ProcessTransformConfig_UID)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/GetProcessDataByUIDAPI?Process_UID={0}", ProcessTransformConfig_UID);
            HttpResponseMessage responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        public string ImportProcessIDTraConfig(HttpPostedFileBase uploadName)
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

                //确定专案
                var FlowChatMasterHead = new[]
               {
                        "客户",
                        "专案",
                        "生产阶段",
                        "部件",
                };

                var iRow = 1;
                bool isExcelError = false;
                for (var i = 1; i <= FlowChatMasterHead.Length; i++)
                {
                    if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                    {
                        var resultsheet = worksheet.Cells[1, i].Value.ToString();
                        var hasItem = FlowChatMasterHead.FirstOrDefault(m => m.Contains(resultsheet));
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

                //头样式设置
                var propertiesHead = new[]
                {
                        "PIS-绑定序号",
                        "PIS-制程名字",
                        "MES-不良数流水号",
                        "MES-领料数流水号",
                        "MES-良品数流水号",
                        "MES-返工返修流水号",
                        "颜色",
                        "是否启用",
                        "是否同步NG",
                        "备注信息",
                };

                //1 验证表头
                for (int i = 1; i <= propertiesHead.Length; i++)
                {
                    if (worksheet.Cells[3, i].Value != null && !string.IsNullOrWhiteSpace(worksheet.Cells[3, i].Value.ToString()))
                    {
                        var resultsheet = worksheet.Cells[3, i].Value.ToString();
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

                iRow = 2;
                string BU_D_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(FlowChatMasterHead, "客户")].Value);
                string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(FlowChatMasterHead, "专案")].Value);
                string Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(FlowChatMasterHead, "生产阶段")].Value);
                string Part_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(FlowChatMasterHead, "部件")].Value);
                int FlowChartMaster_UID = 0;
                if (!string.IsNullOrEmpty(BU_D_Name) && !string.IsNullOrEmpty(Project_Name) && !string.IsNullOrEmpty(Part_Types) && !string.IsNullOrEmpty(Product_Phase))
                {
                    //获取对应的FlowCharMaster
                    var flowChartMasterIDAPI = string.Format("FlowChart/GetFlowChartMasterID?BU_D_Name={0}&Project_Name={1}&Part_Types={2}&Product_Phase={3}", BU_D_Name, Project_Name, Part_Types, Product_Phase);
                    HttpResponseMessage plantsmessage = APIHelper.APIGetAsync(flowChartMasterIDAPI);
                    var flowChartMasterID = plantsmessage.Content.ReadAsStringAsync().Result;
                    FlowChartMaster_UID = int.Parse(flowChartMasterID);
                    if (int.Parse(flowChartMasterID) == 0)
                    {
                        return string.Format("未找到对应:客户:{0},专案:{1},部件类型:{2},生产阶段:{3}", BU_D_Name, Project_Name, Part_Types, Product_Phase);
                    }
                }

                var ProIDTraConfList = new List<ProcessIDTraConfigVM>();
                for (int i = 4; i <= totalRows; i++)
                {
                    var PIS_ProcessID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 1].Value);
                    var PIS_ProcessName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 2].Value);
                    var MES_NgID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 3].Value);
                    var MES_PickingID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 4].Value);
                    var MES_GoodProductID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 5].Value);
                    var MES_ReworkID = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 6].Value);
                    var Color = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 7].Value);
                    var IsEnable = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 8].Value);
                    var IsSyncNG = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 9].Value);
                    var ReMark = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, 10].Value);
                    #region 验证栏位
                    if (string.IsNullOrEmpty(PIS_ProcessID))
                    {
                        errorInfo = string.Format("第[{0}]PIS-绑定序号", i);
                        return errorInfo;
                    }

                    //Product_Date = Convert.ToDateTime(worksheet.Cells[iRow, 3].Value);
                    if (string.IsNullOrEmpty(PIS_ProcessName.ToString()))
                    {
                        errorInfo = string.Format("第[{0}]PIS-制程名字", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(MES_NgID))
                    {
                        errorInfo = string.Format("第[{0}]MES-不良数流水号", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(MES_PickingID))
                    {
                        errorInfo = string.Format("第[{0}]MES-领料数流水号", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(MES_NgID))
                    {
                        errorInfo = string.Format("第[{0}]MES_NG数流水号", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(MES_GoodProductID))
                    {
                        errorInfo = string.Format("第[{0}]MES_不良数流水号", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(MES_ReworkID))
                    {
                        errorInfo = string.Format("第[{0}]MES-返工返修流水号", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(Color))
                    {
                        errorInfo = string.Format("第[{0}]颜色", i);
                        return errorInfo;
                    }

                    if (string.IsNullOrEmpty(IsEnable))
                    {
                        errorInfo = string.Format("第[{0}] 是否启用", i);
                        return errorInfo;
                    }
                    if (string.IsNullOrEmpty(IsSyncNG))
                    {
                        errorInfo = string.Format("第[{0}]MES-是否同步NG", i);
                        return errorInfo;
                    }
                    #endregion
                    ProcessIDTraConfigVM item = new ProcessIDTraConfigVM();
                    item.Binding_Seq = int.Parse(PIS_ProcessID);
                    item.PIS_ProcessName = PIS_ProcessName;
                    item.MES_NgID = MES_NgID;
                    item.MES_PickingID = MES_PickingID;
                    item.MES_ReworkID = MES_ReworkID;
                    item.Modified_UID = this.CurrentUser.GetUserInfo.Account_UID;
                    item.Modified_Date = DateTime.Now;
                    item.ReMark = ReMark;
                    item.FlowChart_Master_UID = FlowChartMaster_UID;
                    item.Color = Color;
                    item.MES_GoodProductID = MES_GoodProductID;
                    item.IsEnabled = int.Parse(IsEnable) == 1 ? true : false;
                    item.IsSyncNG = int.Parse(IsSyncNG) == 1 ? true : false;
                    ProIDTraConfList.Add(item);
                }

                if (ProIDTraConfList.Distinct().Count() != totalRows - 3)
                {
                    errorInfo = "导入的Excel有重复行";
                    return errorInfo;
                }

                //检查数据库是否有重复
                string json = JsonConvert.SerializeObject(ProIDTraConfList);
                //var checkApiUrl = string.Format("ProcessIDTRSConfig/IsExist");
                //HttpResponseMessage checkResponMessage = APIHelper.APIPostAsync(json, checkApiUrl);
                ////var IsExist = checkResponMessage.Content.ReadAsStringAsync().Result;
                //if (IsExist == "True")
                //{
                //    errorInfo = "导入的Excel有重复行";
                //    return errorInfo;
                //}

                var apiUrl = string.Format("ProcessIDTRSConfig/AddProcessIDTRSConfigAPI");
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

        //导出全部设备信息
        public ActionResult ExportAllProcessData(ProcessIDTransformConfigDTO search)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/ExportAllProcessInfoAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<ProcessIDTransformConfigDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("ProcessInfoReport");
            var stringHeads = GetProcessConfigHead();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("ProcessInfoReport");
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 2, 1].Value = index + 1;//序号
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Binding_Seq;//功能厂
                    worksheet.Cells[index + 2, 3].Value = currentRecord.PIS_ProcessID;//专案
                    worksheet.Cells[index + 2, 4].Value = currentRecord.PIS_ProcessName;//OP类型
                    worksheet.Cells[index + 2, 5].Value = currentRecord.MES_NgID;//制程
                    worksheet.Cells[index + 2, 6].Value = currentRecord.MES_PickingID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.MES_GoodProductID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.MES_ReworkID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Color;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.IsEnabled == true ? "是" : "否";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.IsSyncNG == true ? "是" : "否";
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_UID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ReMark;

                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        /// <summary>
        /// 导出勾选部分的设备信息
        /// </summary>
        /// <param name="uids"></param>
        /// <returns></returns>
        public ActionResult ExportPartProcessData(string uids)
        {
            var apiUrl = string.Format("ProcessIDTRSConfig/ExportPartProcessInfo?uids={0}", uids);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<ProcessIDTransformConfigDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("ProcessInfoReport");
            var stringHeads = GetProcessConfigHead();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("ProcessInfoReport");
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 2, 1].Value = index + 1;//序号
                    worksheet.Cells[index + 2, 2].Value = currentRecord.Binding_Seq;//功能厂
                    worksheet.Cells[index + 2, 3].Value = currentRecord.PIS_ProcessID;//专案
                    worksheet.Cells[index + 2, 4].Value = currentRecord.PIS_ProcessName;//OP类型
                    worksheet.Cells[index + 2, 5].Value = currentRecord.MES_NgID;//制程
                    worksheet.Cells[index + 2, 6].Value = currentRecord.MES_PickingID;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.MES_GoodProductID;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.MES_ReworkID;
                    worksheet.Cells[index + 2, 9].Value = currentRecord.Color;
                    worksheet.Cells[index + 2, 10].Value = currentRecord.IsEnabled == true ? "是" : "否";
                    worksheet.Cells[index + 2, 11].Value = currentRecord.IsSyncNG == true ? "是" : "否";
                    worksheet.Cells[index + 2, 12].Value = currentRecord.Modified_UID;
                    worksheet.Cells[index + 2, 13].Value = currentRecord.Modified_Date.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[index + 2, 14].Value = currentRecord.ReMark;
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
        private string[] GetProcessConfigHead()
        {
            var stringHeads = new string[]
             {
           "序号",
            "PIS绑定序号",
            "PIS-制程流水号",
              "PIS-制程名称",
                "MES-不良数流水号",
                  "MES-领料数流水号",
                    "MES-良品数流水号",
                    "MES-返工返修流水号",
                    "部件类型",
                    "是否启用",
                    "是否禁用",
                    "创建人",
                    "创建时间",
                    "备注信息",
             };
            return stringHeads;
        }


        public ActionResult MESSyncReport()
        {
            return View("MESSyncReport");
        }

        public ActionResult MESSyncReportData(MES_StationDataRecordDTO searchParm, Page page)
        {
            var apiUrl = string.Format("MESStationDataRecord/GetMESSyncReportAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(searchParm, page, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            return Content(result, "application/json");
        }

        /// <summary>
        /// 导出MES数据同步的两小时的所有数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult ExportAllTwoHourReport(MES_StationDataRecordDTO search)
        {
            var apiUrl = string.Format("MESStationDataRecord/ExportAllTwoHourReportAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(search, apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<MES_StationDataRecordDTO>>(result).ToList();
            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("MESSyncTwoHourReport");
            var stringHeads = GetMESSyncHead();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("MESSyncTwoHourReport");
                for (int colIndex = 0; colIndex < stringHeads.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = stringHeads[colIndex];
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var currentRecord = list[index];
                    worksheet.Cells[index + 2, 1].Value = index + 1;//序号
                    worksheet.Cells[index + 2, 2].Value = currentRecord.TimeInterVal;//功能厂
                    worksheet.Cells[index + 2, 3].Value = currentRecord.PIS_ProcessID;//专案
                    worksheet.Cells[index + 2, 4].Value = currentRecord.PIS_ProcessName;//OP类型
                    worksheet.Cells[index + 2, 5].Value = currentRecord.MES_ProcessID;//制程
                    worksheet.Cells[index + 2, 6].Value = currentRecord.ProductQuantity;
                    worksheet.Cells[index + 2, 7].Value = currentRecord.ProcessType;
                    worksheet.Cells[index + 2, 8].Value = currentRecord.Color;
                }
                worksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = Server.UrlEncode(fileName) };
        }

        /// <summary>
        /// 获取所有的设备导出的表头
        /// </summary>
        /// <returns></returns>
        private string[] GetMESSyncHead()
        {
            var stringHeads = new string[]
             {
           "序号",
            "时段",
            "PIS-制程ID",
              "PIS-制程名称",
                "MES-制程名称",
                  "扫码数",
                    "制程类型",
                    "部件类型"
             };
            return stringHeads;
        }
    }
}