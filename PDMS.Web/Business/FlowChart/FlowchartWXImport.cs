using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;

namespace PDMS.Web.Business.Flowchart
{
    public class FlowchartWXImport : AbsFlowchart
    {
        public override string AddOrUpdateExcel(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, FlowChartImport importItem)
        {
            string apiUrl = string.Empty;
            string errorInfo = string.Empty;
            FlowChartMasterDTO newMaster = new FlowChartMasterDTO();
            List<FlowChartImportDetailDTO> detailDTOList = new List<FlowChartImportDetailDTO>();
            importItem = new FlowChartImport();
            importItem.FlowChartMasterDTO = newMaster;
            importItem.FlowChartImportDetailDTOList = detailDTOList;
            var userInfo = HttpContext.Current.Session[SessionConstants.CurrentUserInfo] as CustomUserInfoVM;
            if(userInfo == null)
            {
                return "Session is Null";
            }

            try
            {
                using (var xlPackage = new ExcelPackage(uploadName.InputStream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = worksheet.Dimension.End.Row;
                    if (worksheet == null)
                    {
                        errorInfo = "没有worksheet内容";
                        return errorInfo;
                    }
                    //头样式设置
                    var propertiesHead = FlowchartImportCommon.GetHeadColumn();
                    //内容样式设置
                    var propertiesContent = FlowchartImportCommon.GetEtransferContentColumn();

                    int iRow = 2;

                    bool allColumnsAreEmpty = true;
                    for (var i = 1; i <= propertiesHead.Length; i++)
                    {
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            allColumnsAreEmpty = false;
                            break;
                        }
                    }
                    if (allColumnsAreEmpty)
                    {
                        errorInfo = "Excel格式不正确";
                        return errorInfo;
                    }

                    string BU_D_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesHead, "客户")].Value);
                    string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesHead, "专案名称")].Value);
                    string Part_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesHead, "部件")].Value);
                    string Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesHead, "阶段")].Value);

                    if (string.IsNullOrWhiteSpace(BU_D_Name) || string.IsNullOrWhiteSpace(Project_Name) || string.IsNullOrWhiteSpace(Part_Types) || string.IsNullOrWhiteSpace(Product_Phase))
                    {
                        errorInfo = "客户,专案名称,部件,阶段不能为空Excel格式不正确";
                        return errorInfo;
                    }

                    FlowChartExcelImportParas paraItem = new FlowChartExcelImportParas();
                    paraItem.BU_D_Name = BU_D_Name.Trim();
                    paraItem.Project_Name = Project_Name.Trim();
                    paraItem.Part_Types = Part_Types.Trim();
                    paraItem.Product_Phase = Product_Phase.Trim();
                    paraItem.FlowChart_Master_UID = FlowChart_Master_UID;


                    if (isEdit)
                    {
                        paraItem.isEdit = true;
                    }
                    else
                    {
                        paraItem.isEdit = false;
                    }
                    apiUrl = string.Format("FlowChart/CheckFlowChartAPI");
                    HttpResponseMessage responMessage = APIHelper.APIPostAsync(paraItem, apiUrl);
                    var projectUIDOrFLMasterUID = responMessage.Content.ReadAsStringAsync().Result;
                    projectUIDOrFLMasterUID = projectUIDOrFLMasterUID.Replace("\"", "");

                    if (!FlowchartImportCommon.ValidIsInt(projectUIDOrFLMasterUID, isEdit))
                    {
                        errorInfo = projectUIDOrFLMasterUID;
                        return errorInfo;
                    }


                    if (isEdit)
                    {
                        var idList = projectUIDOrFLMasterUID.Split('_').ToList();
                        newMaster.FlowChart_Master_UID = Convert.ToInt32(idList[0]);
                        newMaster.Project_UID = Convert.ToInt32(idList[1]);
                        newMaster.FlowChart_Version = Convert.ToInt32(idList[2]);

                        //检查当前时间段是否包含有ProductInput的制程信息
                        string productapiUrl = string.Format("ProductInput/CheckHasExistProcessAPI?MasterUID={0}&Version={1}", newMaster.FlowChart_Master_UID, newMaster.FlowChart_Version);
                        HttpResponseMessage productResponMessage = APIHelper.APIGetAsync(productapiUrl);
                        var hasProcess = productResponMessage.Content.ReadAsStringAsync().Result;
                        if (hasProcess == "true")
                        {
                            errorInfo = "当前Flowchart的相关制程还未完全录入，不能导入新版本";
                            return errorInfo;
                        }
                    }
                    else
                    {
                        newMaster.Project_UID = Convert.ToInt32(projectUIDOrFLMasterUID);
                        newMaster.FlowChart_Version = 1;
                    }
                    newMaster.Part_Types = Part_Types.Trim();
                    newMaster.FlowChart_Version_Comment = FlowChart_Version_Comment;
                    newMaster.Is_Latest = true;
                    newMaster.Is_Closed = false;
                    newMaster.Modified_UID = userInfo.Account_UID;
                    newMaster.Modified_Date = DateTime.Now;


                    //获取所有厂别
                    //不考虑跨厂区的权限，跨厂区不会操作flowchart导入，默认取第一个optypes
                    var plantAPI = "FlowChart/QueryAllFunctionPlantsAPI?optype=" + userInfo.OrgInfo.First().OPType;
                    HttpResponseMessage message = APIHelper.APIGetAsync(plantAPI);
                    var jsonPlants = message.Content.ReadAsStringAsync().Result;
                    var functionPlants = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(jsonPlants);
                    //从第四行开始读取
                    iRow = iRow + 2;
                    //制程序号
                    string ItemNo = string.Empty;
                    int Process_Seq = 0;
                    //DRI
                    string DRI;
                    //场地
                    string Place;
                    //工站名稱
                    string Process;
                    //厂别
                    int System_FunPlant_UID=20;
                    string plantName;
                    //阶层
                    int Product_Stage = 0;
                    //颜色
                    string Color;
                    //工站說明
                    string Process_Desc;
                    //返工设定
                    string Rework_Setting;
                    //检测设定
                    string Check_Setting=string.Empty;
                    ///eTransfer所需属性---Robert 2016、10、31
                    string FromWHS = string.Empty;
                    string ToWHSOK = string.Empty;
                    string ToWHSNG = string.Empty;

                    //返修站点数量，每个FlowChart只能允许1个站点是返工站点
                    int Repair_Count = 0;
                    int Process_Sign = 0;
                    for (var i = iRow; i <= totalRows; i++)
                    {
                        //列名，列名序号是0
                        var value = worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value;
                        if (value == null)
                        {
                            break;
                        }
                        ItemNo = ExcelHelper.ConvertColumnToString(value);
                        //循环结束,防止有空行

                        DRI = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "DRI")].Value);
                        if (string.IsNullOrWhiteSpace(DRI))
                        {
                            //跳出循环
                            errorInfo = string.Format("第[{0}]行DRI不能为空", i);
                            return errorInfo;
                        }
                        Place = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "场地")].Value);
                        if (string.IsNullOrWhiteSpace(Place))
                        {
                            //跳出循环
                            errorInfo = string.Format("第[{0}]行场地不能为空", i);
                            return errorInfo;
                        }
                        Process = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "工站名稱")].Value);
                        if (string.IsNullOrWhiteSpace(Process))
                        {
                            //跳出循环
                            errorInfo = string.Format("第[{0}]行工站名稱不能为空", i);
                            return errorInfo;
                        }
                        plantName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "厂别")].Value) ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(plantName))
                        {
                            //跳出循环
                            errorInfo = string.Format("第[{0}]行厂别不能为空", i);
                            return errorInfo;
                        }
                        //获取到当前的OP类型
                        //var nowOpTypes = this.CurrentUser.DataPermissions.Op_Types.FirstOrDefault();
                        
                        //获取到当前账号所在的功能厂
                        //var nowFuncPlant = this.CurrentUser.DataPermissions.UserOrgInfo.Org_FuncPlant;
                        //var nowFuncPlant = userInfo.OrgInfo.Org_FuncPlant;


                        var hasItem = functionPlants.FirstOrDefault(m => m.FunPlant.ToLower() == plantName.ToLower().Trim());
                        if (hasItem != null)
                        {
                            System_FunPlant_UID = hasItem.System_FunPlant_UID;
                        }
                        //else
                        //{
                        //    //跳出循环
                        //    errorInfo = string.Format("厂别[{0}", plantName);
                        //    return errorInfo;
                        //}

                        Product_Stage = Convert.ToInt32(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "阶层")].Value);
                        Color = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "颜色")].Value) ?? string.Empty;
                        Process_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "工站說明")].Value);

                        Rework_Setting = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "返工设定")].Value);
                        if (Process_Sign != Process_Seq && Rework_Setting == "Repair")
                            Repair_Count++;
                        if (Repair_Count > 1)
                        {
                            //跳出循环
                            errorInfo = string.Format("第[{0}]行出现重复的修复站点！", i);
                            return errorInfo;
                        }
                        Process_Sign = Process_Seq;

                        //检测站点赋值 
                        var CheckTemp = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "检测设定")].Value);
                        if (!string.IsNullOrWhiteSpace(CheckTemp))
                        {
                            CheckTemp = CheckTemp.Trim();
                        }

                        switch (CheckTemp)
                        {
                            case StructConstants.IsQAProcessType.InspectText: //IPQC全检
                                Check_Setting = StructConstants.IsQAProcessType.InspectKey;
                                break;
                            case StructConstants.IsQAProcessType.PollingText: //IPQC巡检
                                Check_Setting = StructConstants.IsQAProcessType.PollingKey;
                                break;
                            case StructConstants.IsQAProcessType.InspectOQCText: //OQC检测
                                Check_Setting = StructConstants.IsQAProcessType.InspectOQCKey;
                                break;
                            case StructConstants.IsQAProcessType.InspectAssembleText: //组装检测
                                Check_Setting = StructConstants.IsQAProcessType.InspectAssembleKey;
                                break;
                            case StructConstants.IsQAProcessType.AssembleOQCText: //组装&OQC检测
                                Check_Setting = StructConstants.IsQAProcessType.AssembleOQCKey;
                                break;
                            default:
                                Check_Setting = string.Empty;
                                break;
                        }
                        //eTransfer部分获取值
                        FromWHS = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "FromWHS")].Value);
                        ToWHSNG = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "ToWHSNG")].Value);
                        ToWHSOK = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "ToWHSOK")].Value);

                        var listColor = Color.Split('/').ToList();
                        foreach (var itemColor in listColor)
                        {
                            FlowChartImportDetailDTO detailDTOItem = new FlowChartImportDetailDTO();

                            FlowChartDetailDTO newDetailDtoItem = new FlowChartDetailDTO();
                            newDetailDtoItem.FlowChart_Master_UID = newMaster.FlowChart_Master_UID;
                            newDetailDtoItem.System_FunPlant_UID = System_FunPlant_UID;
                            newDetailDtoItem.Process_Seq = Process_Seq.ToString();
                            newDetailDtoItem.ItemNo = ItemNo;
                            newDetailDtoItem.DRI = DRI;
                            newDetailDtoItem.Place = Place;
                            newDetailDtoItem.Process = Process;
                            newDetailDtoItem.Product_Stage = Product_Stage;
                            newDetailDtoItem.Color = itemColor;
                            newDetailDtoItem.Process_Desc = Process_Desc;
                            newDetailDtoItem.FlowChart_Version_Comment = FlowChart_Version_Comment;
                            newDetailDtoItem.Rework_Flag = Rework_Setting;
                            //新增QA字段
                            newDetailDtoItem.IsQAProcess = Check_Setting;
                            newDetailDtoItem.Modified_UID = newMaster.Modified_UID;
                            newDetailDtoItem.Modified_Date = newMaster.Modified_Date;
                            newDetailDtoItem.FromWHS = FromWHS;
                            newDetailDtoItem.ToWHSNG = ToWHSNG;
                            newDetailDtoItem.ToWHSOK = ToWHSOK;
                            if (isEdit)
                            {
                                //存到临时表里面的数据所以要加1
                                newDetailDtoItem.FlowChart_Version = newMaster.FlowChart_Version + 1;
                            }
                            else
                            {
                                //存到正式表里面的数据不用加1
                                newDetailDtoItem.FlowChart_Version = newMaster.FlowChart_Version;
                            }

                            detailDTOItem.FlowChartDetailDTO = newDetailDtoItem;
                            //detailDTOItem.FlowChartMgDataDTO = newMgDataDtoItem;
                            detailDTOList.Add(detailDTOItem);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                errorInfo = "上传的文件类型不正确 " + exc.ToString();
            }
            return errorInfo;

        }

        public override ActionResult DoHistoryExcelExport(int id, int version)
        {
            var apiUrl = string.Format("FlowChart/DoHistoryExcelExportWUXI_MAPI?id={0}&version={1}", id, version);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartExcelExport>(result);



            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FlowChart");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var propertiesHead = FlowchartImportCommon.GetHeadColumn();
            var propertiesContent = FlowchartImportCommon.GetEtransferContentColumn();
            using (var excelPackage = new ExcelPackage(stream))
            {
                //set sheet name
                var worksheet = excelPackage.Workbook.Worksheets.Add("FlowChart");

                //填充Title内容
                for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = propertiesHead[colIndex];

                    switch (colIndex)
                    {
                        case 0:
                            worksheet.Cells[2, colIndex + 1].Value = item.BU_D_Name;
                            break;
                        case 1:
                            worksheet.Cells[2, colIndex + 1].Value = item.Project_Name;
                            break;
                        case 2:
                            worksheet.Cells[2, colIndex + 1].Value = item.Part_Types;
                            break;
                        case 3:
                            worksheet.Cells[2, colIndex + 1].Value = item.Product_Phase;
                            break;
                    }
                }

                //填充Content内容
                for (int colIndex = 0; colIndex < propertiesContent.Length; colIndex++)
                {
                    worksheet.Cells[3, colIndex + 1].Value = propertiesContent[colIndex];
                }

                for (int index = 0; index < item.FlowChartDetailAndMGDataDTOList.Count(); index++)
                {
                    var currentRecord = item.FlowChartDetailAndMGDataDTOList[index];
                    worksheet.Cells[index + 4, 1].Value = currentRecord.ItemNo;
                    //DRI
                    worksheet.Cells[index + 4, 2].Value = currentRecord.DRI;
                    //场地
                    worksheet.Cells[index + 4, 3].Value = currentRecord.Place;
                    //Process
                    worksheet.Cells[index + 4, 4].Value = currentRecord.Process;
                    //厂别
                    worksheet.Cells[index + 4, 5].Value = currentRecord.PlantName;
                    //阶层
                    worksheet.Cells[index + 4, 6].Value = currentRecord.Product_Stage;
                    //颜色
                    worksheet.Cells[index + 4, 7].Value = currentRecord.Color;
                    //工站说明
                    worksheet.Cells[index + 4, 8].Value = currentRecord.Process_Desc;
                    //返工设定
                    worksheet.Cells[index + 4, 9].Value = currentRecord.Rework_Flag;
                    //检测设定
                    worksheet.Cells[index + 4, 10].Value = currentRecord.IsQAProcessName;
                    //FromWHS
                    worksheet.Cells[index + 4, 11].Value = currentRecord.FromWHS;
                    //ToWHSOK
                    worksheet.Cells[index + 4, 12].Value = currentRecord.ToWHSOK;
                    //ToWHSNG
                    worksheet.Cells[index + 4, 13].Value = currentRecord.ToWHSNG;

                    //目标良率
                    //worksheet.Cells[index + 4, 9].Value = currentRecord.Target_Yield;
                    //worksheet.Cells[index + 4, 9].Style.Numberformat.Format = "0.00%";
                    //计划目标
                    //worksheet.Cells[index + 4, 10].Value = currentRecord.Product_Plan;
                }

                excelPackage.Save();
            }
            return new FileContentResult(stream.ToArray(), "application/octet-stream")
            { FileDownloadName = fileName };

        }

    }
}