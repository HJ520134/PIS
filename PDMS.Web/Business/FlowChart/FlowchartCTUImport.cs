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
    public class FlowchartCTUImport : AbsFlowchart
    {
        public override string AddOrUpdateExcel(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, FlowChartImport importItem)
        {
            string apiUrl = string.Empty;
            string errorInfo = string.Empty;
            FlowChartMasterDTO newMaster = new FlowChartMasterDTO();
            List<FlowChartImportDetailDTO> detailDTOList = new List<FlowChartImportDetailDTO>();
            //importItem = new FlowChartImport();
            importItem.FlowChartMasterDTO = newMaster;
            importItem.FlowChartImportDetailDTOList = detailDTOList;
            var userInfo = HttpContext.Current.Session[SessionConstants.CurrentUserInfo] as CustomUserInfoVM;
            if (userInfo == null || userInfo.OrgInfo.Count() == 0 || userInfo.Plant_OrganizationUIDList.Count() == 0)
            {
                return "用户没有配置对应的SITE";
            }

            try
            {
                using (var xlPackage = new ExcelPackage(uploadName.InputStream))
                {
                    FlowChartExcelImportParas paraItem = new FlowChartExcelImportParas();
                    int iRow = 2;
                    var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalRows = worksheet.Dimension.End.Row;
                    string Part_Types;

                    #region 判断Excel是否合法
                    errorInfo = CheckExeclValid(worksheet, iRow, FlowChart_Master_UID, paraItem, out Part_Types);
                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        return errorInfo;
                    }
                    #endregion


                    #region 判断新增和编辑都检查客户，专案名称，部件，阶段是否匹配
                    string projectUIDOrFLMasterUID;
                    errorInfo = CheckFlowchartIsMatch(paraItem, newMaster, isEdit, FlowChart_Version_Comment, Part_Types, userInfo, out projectUIDOrFLMasterUID);
                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        return errorInfo;
                    }
                    #endregion

                    #region 读取Excel内容，判断单元格是否为空并赋值,判断绑定序号，制程序号是否重复
                    errorInfo = SetAndCheckExcelContent(detailDTOList, isEdit, worksheet, iRow, totalRows, newMaster, FlowChart_Version_Comment, userInfo);
                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        return errorInfo;
                    }
                    #endregion

                    #region Flowchart升版，把WIP,生产计划，绑定物料员都带出来
                    if (isEdit)
                    {
                        List<int> detailUIDList = new List<int>();
                        SetFlowchartWipAndPlan(userInfo, detailDTOList, newMaster, out detailUIDList);
                        importItem.FlowchartDetailUIDList = detailUIDList;
                    }

                    #endregion
                }

            }
            catch (Exception exc)
            {
                errorInfo = "上传的文件类型不正确 " + exc.ToString();
            }
            return errorInfo;
        }

        public string CheckExeclValid(ExcelWorksheet worksheet, int iRow, int FlowChart_Master_UID, FlowChartExcelImportParas paraItem, out string Part_Types)
        {
            Part_Types = string.Empty;
            string errorInfo = string.Empty;
            if (worksheet == null)
            {
                errorInfo = "没有worksheet内容";

                return errorInfo;
            }
            //头样式设置
            var propertiesHead = FlowchartImportCommon.GetHeadColumn();

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
            Part_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesHead, "部件")].Value);
            string Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, ExcelHelper.GetColumnIndex(propertiesHead, "阶段")].Value);

            if (string.IsNullOrWhiteSpace(BU_D_Name) || string.IsNullOrWhiteSpace(Project_Name) || string.IsNullOrWhiteSpace(Part_Types) || string.IsNullOrWhiteSpace(Product_Phase))
            {
                errorInfo = "客户,专案名称,部件,阶段不能为空Excel格式不正确";
                return errorInfo;
            }

            paraItem.BU_D_Name = BU_D_Name.Trim();
            paraItem.Project_Name = Project_Name.Trim();
            paraItem.Part_Types = Part_Types.Trim();
            paraItem.Product_Phase = Product_Phase.Trim();
            paraItem.FlowChart_Master_UID = FlowChart_Master_UID;
            return errorInfo;
        }

        public string CheckFlowchartIsMatch(FlowChartExcelImportParas paraItem, FlowChartMasterDTO newMaster, bool isEdit, string FlowChart_Version_Comment, string Part_Types, CustomUserInfoVM userInfo, out string projectUIDOrFLMasterUID)
        {
            string errorInfo = string.Empty;
            var orgList = userInfo.OrgInfo.Select(m => m.OPType_OrganizationUID.Value).ToList();
            paraItem.Organization_UIDList = orgList;
            if (isEdit)
            {
                paraItem.isEdit = true;
            }
            else
            {
                paraItem.isEdit = false;
            }
            var apiUrl = string.Format("FlowChart/CheckFlowChartAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(paraItem, apiUrl);
            projectUIDOrFLMasterUID = responMessage.Content.ReadAsStringAsync().Result;
            projectUIDOrFLMasterUID = projectUIDOrFLMasterUID.Replace("\"", "");
            //正确返回id字符串，错误则返回出错字符串
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
                newMaster.Organization_UID = Convert.ToInt32(idList[3]);
                newMaster.Modified_UID = userInfo.Account_UID;
                newMaster.Modified_Date = DateTime.Now;

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
                var idList = projectUIDOrFLMasterUID.Split('_').ToList();
                newMaster.Project_UID = Convert.ToInt32(idList[0]);
                newMaster.Organization_UID = Convert.ToInt32(idList[1]);
                newMaster.FlowChart_Version = 1;
                newMaster.Created_UID = userInfo.Account_UID;
                newMaster.Created_Date = DateTime.Now;
            }
            newMaster.Part_Types = Part_Types.Trim();
            newMaster.FlowChart_Version_Comment = FlowChart_Version_Comment;
            newMaster.Is_Latest = true;
            newMaster.Is_Closed = false;
            newMaster.Product_Phase = paraItem.Product_Phase;
            newMaster.Modified_UID = userInfo.Account_UID;
            newMaster.Modified_Date = DateTime.Now;

            return errorInfo;
        }

        public string SetAndCheckExcelContent(List<FlowChartImportDetailDTO> detailDTOList, bool isEdit, ExcelWorksheet worksheet, int iRow, int totalRows, FlowChartMasterDTO newMaster, string FlowChart_Version_Comment, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            //内容样式设置
            var propertiesContent = FlowchartImportCommon.GetContentColumn();

            //获取所有厂别
            //不考虑跨厂区权限，他们不会操作只会浏览，默认取第一个,通过三级权限获取四级
            var plantAPI = "FlowChart/QueryAllFunctionPlantsAPI?optypeUID=" + userInfo.OrgInfo.First().OPType_OrganizationUID;
            //HttpResponseMessage message = APIHelper.APIPostAsync(null,plantAPI);
            HttpResponseMessage message = APIHelper.APIGetAsync(plantAPI);
            var jsonPlants = message.Content.ReadAsStringAsync().Result;
            var functionPlants = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(jsonPlants);
            //从第四行开始读取
            iRow = iRow + 2;
            //绑定序号        //制程序号    //厂别               //阶层         //WIP数量
            int Binding_Seq, Process_Seq, System_FunPlant_UID, Product_Stage, WIP, NullWip, Current_WH_QTY;

            //DRI       //场地 //工站名稱  //厂别   //颜色  //工站說明      //检测设定 //是否分楼栋     //返工设定 //对应返修站点
            string DRI, Place, Process, plantName, Color, Process_Desc, Check_Setting, Location_Flag, Rework_Setting, RelatedRepairBingdingSeq  ;
            //用来测试返工的填写是否正确
            List<int> idList = new List<int>();
            //这是真正的excel行数，totalRows的行数是错误的
            int realTotalRows = 0;
            var col = worksheet.SelectedRange[1, 2];
            for (var i = iRow; i <= totalRows; i++)
            {
                //绑定序号
                Binding_Seq = Convert.ToInt32(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "绑定序号")].Value);
                //制程序号
                Process_Seq = Convert.ToInt32(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value);
                //循环结束,防止有空行
                if (Process_Seq == 0)
                {
                    break;
                }
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
                //接收空白字符,Y,N
                Location_Flag = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "是否分楼栋")].Value);
                if (string.IsNullOrWhiteSpace(Location_Flag))
                {
                    worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "是否分楼栋")].Value = "N";
                }
                else if (Location_Flag.ToUpper() != "Y" && Location_Flag.ToUpper() != "N")
                {
                    //跳出循环
                    errorInfo = string.Format("第[{0}]行是否分楼栋只能为Y,N 或空白字符", i);
                    return errorInfo;
                }

                Process = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "工站名稱")].Value);
                if (string.IsNullOrWhiteSpace(Process))
                {
                    //跳出循环
                    errorInfo = string.Format("第[{0}]行工站名稱不能为空", i);
                    return errorInfo;
                }
                else
                {
                    Process = Process.Trim(); //去掉前后空格
                }
                plantName = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "厂别")].Value) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(plantName))
                {
                    //跳出循环
                    errorInfo = string.Format("第[{0}]行厂别不能为空", i);
                    return errorInfo;
                }

                var hasItem = functionPlants.FirstOrDefault(m => m.FunPlant.ToLower() == plantName.ToLower().Trim());
                if (hasItem != null)
                {
                    System_FunPlant_UID = hasItem.System_FunPlant_UID;
                }
                else
                {
                    //跳出循环
                    errorInfo = string.Format("厂别[{0}]输入不正确", plantName);
                    return errorInfo;
                }


                Product_Stage = Convert.ToInt32(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "阶层")].Value);
                Color = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "颜色")].Value) ?? string.Empty;
                Process_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "工站說明")].Value);
                Rework_Setting = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "返工设定")].Value);
                RelatedRepairBingdingSeq = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "对应修复站点")].Value);
                WIP = Convert.ToInt32(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "WIP")].Value);
                NullWip = Convert.ToInt32(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "不可用WIP")].Value);
                Current_WH_QTY = Convert.ToInt32(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "当前库存数")].Value);

                string Data_Source  = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "数据来源")].Value);
                var Is_Synchronous = ExcelHelper.ConvertColumnToString(worksheet.Cells[i, ExcelHelper.GetColumnIndex(propertiesContent, "是否同步")].Value);
                bool bool_Is_Synchronous = false;
                try
                {
                    bool_Is_Synchronous = Convert.ToBoolean(Is_Synchronous == "Y" ? true : false);
                }
                catch
                {
                    errorInfo = string.Format("第{0}行是否同步[{1}]输入错误,请输入Y或N", i, Is_Synchronous);
                    return errorInfo;
                }

                if (!string.IsNullOrEmpty(Rework_Setting) && Rework_Setting.ToLower() == "rework")
                {
                    if (string.IsNullOrWhiteSpace(RelatedRepairBingdingSeq))
                    {
                        errorInfo = string.Format("返工设定为Rework时，第[{0}]行对应修复站点不能为空", i);
                        return errorInfo;
                    }

                    var ids = RelatedRepairBingdingSeq.Split(',').ToList();
                    var intIdList = ids.Select<string, int>(x => Convert.ToInt32(x));
                    //把所有id都添加进队列里面后面一起判断
                    idList.AddRange(intIdList);
                }

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

                #region 添加FlowChartDetail

                FlowChartImportDetailDTO detailDTOItem = new FlowChartImportDetailDTO();
                FlowChartDetailDTO newDetailDtoItem = new FlowChartDetailDTO()
                {
                    FlowChart_Master_UID = newMaster.FlowChart_Master_UID,
                    System_FunPlant_UID = System_FunPlant_UID,
                    Binding_Seq = Binding_Seq,
                    Process_Seq = Process_Seq.ToString(),
                    DRI = DRI,
                    Place = Place,
                    Location_Flag = (Location_Flag == "Y" || Location_Flag == "y") ? true : false,
                    Process = Process,
                    Product_Stage = Product_Stage,
                    Color = Color,
                    Process_Desc = Process_Desc,
                    FlowChart_Version_Comment = FlowChart_Version_Comment,
                    Rework_Flag = Rework_Setting,
                    IsQAProcess = Check_Setting,
                    Modified_UID = newMaster.Modified_UID,
                    Modified_Date = newMaster.Modified_Date,
                    BeginTime = DateTime.Now,
                    //存到临时表里面的数据所以要加1,存到正式表里面的数据不用加1
                    FlowChart_Version = isEdit ? newMaster.FlowChart_Version + 1 : newMaster.FlowChart_Version,
                    WIP_QTY = WIP,
                    NullWip=NullWip,
                    Current_WH_QTY=Current_WH_QTY,
                    Data_Source=Data_Source,
                    Is_Synchronous =bool_Is_Synchronous
                };
                if (Rework_Setting == "Rework")
                {
                    newDetailDtoItem.RelatedRepairBindingSeq = RelatedRepairBingdingSeq;
                }
                else
                {
                    newDetailDtoItem.RelatedRepairBindingSeq = null;
                }
                detailDTOItem.FlowChartDetailDTO = newDetailDtoItem;
                //detailDTOItem.FlowChartMgDataDTO = newMgDataDtoItem;
                detailDTOList.Add(detailDTOItem);
                #endregion
                //行数加1
                realTotalRows++;
            }

            var bindingSeqCount = detailDTOList.GroupBy(m => m.FlowChartDetailDTO.Binding_Seq).Select(m => m.Key).Count();
            //var processSeqCount = detailDTOList.GroupBy(m => m.FlowChartDetailDTO.Process_Seq).Select(m => m.Key).Count();
            var rowCount = realTotalRows;
            if (bindingSeqCount != rowCount)
            {
                errorInfo = "绑定序号有重复项,请修改";
                return errorInfo;
            }

            //判断设定的返工信息是否正确
            var bindSeqList = detailDTOList.Select(m => m.FlowChartDetailDTO.Binding_Seq).ToList();
            var notExistId = idList.Distinct().ToList().Except(bindSeqList).ToList();
            if (notExistId.Count() > 0)
            {
                errorInfo = string.Format("对应修复站点的绑定序号:{0},设定有问题", notExistId.First());
                return errorInfo;
            }
            return errorInfo;
        }

        public void SetFlowchartWipAndPlan(CustomUserInfoVM userInfo, List<FlowChartImportDetailDTO> detailDTOList, FlowChartMasterDTO newMaster, out List<int> idList)
        {
            idList = new List<int>();
            //获取上个版本的FLowchartDetail数据
            var api = string.Format("FlowChart/QueryDetailList?id={0}&Version={1}", newMaster.FlowChart_Master_UID, newMaster.FlowChart_Version);
            HttpResponseMessage message = APIHelper.APIGetAsync(api);
            var json = message.Content.ReadAsStringAsync().Result;
            var flowchartOldVersionList = JsonConvert.DeserializeObject<List<FlowChartDetailDTO>>(json);
            idList = flowchartOldVersionList.Select(m => m.FlowChart_Detail_UID).ToList();
            var idListJson = JsonConvert.SerializeObject(idList);
            //获取上个版本的生产计划数据包含这周和下周的数据
            var mgApi = "FlowChart/QueryFLMgDataList";
            HttpResponseMessage myApiMessage = APIHelper.APIPostAsync(idListJson, mgApi);
            var myApiJson = myApiMessage.Content.ReadAsStringAsync().Result;
            List<FlowChartMgDataDTO> mgList = JsonConvert.DeserializeObject<List<FlowChartMgDataDTO>>(myApiJson);
            //获取上个版本的绑定物料员数据
            var myBomApi = "FlowChart/QueryFLBomDataList";
            HttpResponseMessage myBomApiMessage = APIHelper.APIPostAsync(idListJson, myBomApi);
            var myBomApiJson = myBomApiMessage.Content.ReadAsStringAsync().Result;
            List<FlowChartPCMHRelationshipDTO> bomList = JsonConvert.DeserializeObject<List<FlowChartPCMHRelationshipDTO>>(myBomApiJson);

            //循环Excel中获取的列表 
            foreach (var detailDTOItem in detailDTOList)
            {
                //通过绑定序号列找到老版本的FlowChart_Detail_UID
                var hasItem = flowchartOldVersionList.Where(m => m.Binding_Seq == detailDTOItem.FlowChartDetailDTO.Binding_Seq).FirstOrDefault();
                if (hasItem != null)
                {
                    //WIP的数量已经在excel里面设置了，不需要读取上一次的记录了
                    //detailDTOItem.FlowChartDetailDTO.WIP_QTY = hasItem.WIP_QTY;
                    detailDTOItem.FlowChartDetailDTO.Old_FlowChart_Detail_UID = hasItem.FlowChart_Detail_UID;
                    //通过老版本的FlowChart_Detail_UID去生产计划表里找到这周和下周的该制程所有的生产计划
                    var hasMgList = mgList.Where(m => m.FlowChart_Detail_UID == hasItem.FlowChart_Detail_UID).ToList();
                    //将生产计划这周和下载的数据绑定到新版本的数据中，这样一个FlowChartDetailDTO类型对应一个MgDataList列表，但是在数据插入时FlowChart_Detail_UID已经变为新的值了，所以后面插入还要再处理
                    detailDTOItem.MgDataList = hasMgList;
                    //将老版本的绑定物料员绑定到新版本的item中
                    var pcList = bomList.Where(m => m.FlowChart_Detail_UID == hasItem.FlowChart_Detail_UID).ToList();//只会1对1，ToList是为了转换对象方便
                    if (pcList != null && pcList.Count() > 0)
                    {
                        pcList.First().Modified_UID = userInfo.Account_UID;
                        pcList.First().Modified_Date = DateTime.Now;
                    }
                    detailDTOItem.PCMHList = pcList;
                }

            }
        }

        public override ActionResult DoHistoryExcelExport(int id, int version)
        {
            var apiUrl = string.Format("FlowChart/DoHistoryExcelExportAPI?id={0}&version={1}", id, version);
            var responMessage = APIHelper.APIGetAsync(apiUrl);
            var result = responMessage.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<FlowChartExcelExport>(result);



            var stream = new MemoryStream();
            var fileName = PathHelper.SetGridExportExcelName("FlowChart");
            var filePath = Path.Combine(PathHelper.GetGridExportExcelDirectory, fileName);
            var propertiesHead = FlowchartImportCommon.GetHeadColumn();
            var propertiesContent = FlowchartImportCommon.GetContentColumn();
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
                    //绑定序号
                    worksheet.Cells[index + 4, 1].Value = currentRecord.Binding_Seq;
                    //制程序号
                    worksheet.Cells[index + 4, 2].Value = currentRecord.Process_Seq;
                    //DRI
                    worksheet.Cells[index + 4, 3].Value = currentRecord.DRI;
                    //场地
                    worksheet.Cells[index + 4, 4].Value = currentRecord.Place;
                    //是否分楼栋
                    worksheet.Cells[index + 4, 5].Value = currentRecord.Location_Flag ? "Y" : "N";
                    //Process
                    worksheet.Cells[index + 4, 6].Value = currentRecord.Process;
                    //厂别
                    worksheet.Cells[index + 4, 7].Value = currentRecord.PlantName;
                    //阶层
                    worksheet.Cells[index + 4, 8].Value = currentRecord.Product_Stage;
                    //颜色
                    worksheet.Cells[index + 4, 9].Value = currentRecord.Color;
                    //工站说明
                    worksheet.Cells[index + 4, 10].Value = currentRecord.Process_Desc;
                    //检测设定
                    worksheet.Cells[index + 4, 11].Value = currentRecord.IsQAProcessName;
                    //返工设定
                    worksheet.Cells[index + 4, 12].Value = currentRecord.Rework_Flag;
                    //对应修复站点
                    worksheet.Cells[index + 4, 13].Value = currentRecord.RepairJoin;
                    //WIP
                    worksheet.Cells[index + 4, 14].Value = currentRecord.WIP_QTY;
                    //nullWIP
                    worksheet.Cells[index + 4, 15].Value = currentRecord.NullWip;
                    //currentwH
                    worksheet.Cells[index + 4, 16].Value = currentRecord.Current_WH_QTY;
                    //数据来源
                    worksheet.Cells[index + 4, 17].Value = currentRecord.Data_Source;
                    //是否同步
                    worksheet.Cells[index + 4, 18].Value = currentRecord.Is_Synchronous ? "Y" : "N";

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