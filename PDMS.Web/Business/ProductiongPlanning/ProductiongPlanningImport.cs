using Newtonsoft.Json;
using OfficeOpenXml;
using PDMS.Common.Constants;
using PDMS.Common.Helpers;
using PDMS.Core;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;
using PDMS.Model.ViewModels.ProductionPlanning;
using PDMS.Web.Business.Flowchart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace PDMS.Web.Business.ProductiongPlanning
{
    public class ProductiongPlanningImport : LanguageCommon
    {
        #region RP - ME 資料匯入 ------ Add By Wesley
        public string RP_ImportCheck(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, List<RP_VM> rP_VM,
List<RP_ME_D_Equipment> equipmentList, List<RP_ME_D_Equipment> aotuEquipmentList, List<RP_ME_D_Equipment> auxiliaryEquipList = null)
        {
            string errorInfo = string.Empty;
            var userInfo = HttpContext.Current.Session[SessionConstants.CurrentUserInfo] as CustomUserInfoVM;

            if (userInfo == null || userInfo.OrgInfo.Count() == 0 || userInfo.Plant_OrganizationUIDList.Count() == 0)
            {
                return this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.NotSetPlant");
            }
            try
            {
                using (var xlPackage = new ExcelPackage(uploadName.InputStream))
                {
                    List<ExcelWorksheet> flowchartSheets = new List<ExcelWorksheet>();
                    var sheetList = xlPackage.Workbook.Worksheets.Where(m => m.Name.Contains("Flowchart")).ToList();
                    if (isEdit) //如果是专案更新，则只取第一个sheet
                    {
                        flowchartSheets.Add(sheetList.First());
                    }
                    else
                    {
                        flowchartSheets = sheetList;
                    }

                    var processingequipName = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.ProcessingEquip");
                    var autoEquipName = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.AutoEquip");
                    var auxiliaryEquipName = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.AuxiliaryEquip");
                    var processingNeedSheet = xlPackage.Workbook.Worksheets.Where(m => m.Name == processingequipName).First();
                    var autoNeedSheet = xlPackage.Workbook.Worksheets.Where(m => m.Name == autoEquipName).First();
                    var auxiliaryNeedSheet = xlPackage.Workbook.Worksheets.Where(m => m.Name == auxiliaryEquipName).First();
                    int iRow = 2;
                    string Part_Types;

                    #region 判断Excel是否合法
                    foreach (var flowchartSheet in flowchartSheets)
                    {
                        RP_VM _RP_VM = new RP_VM();
                        RP_ME_ExcelImportParas paraItem = new RP_ME_ExcelImportParas();
                        RP_M newMaster = new RP_M();
                        List<RP_ME_D> detailDTOMEList = new List<RP_ME_D>();
                        _RP_VM.RP_M = newMaster;
                        _RP_VM.RP_ME_D = detailDTOMEList;
                        rP_VM.Add(_RP_VM);

                        #region 判断客户，专案，名称，部件，阶段是否为空
                        errorInfo = RP_CheckExeclValid(flowchartSheet, iRow, FlowChart_Master_UID, paraItem, out Part_Types);
                        if (!string.IsNullOrEmpty(errorInfo))
                        {
                            return errorInfo;
                        }
                        #endregion

                        #region 判断新增和编辑都检查客户，专案名称，部件，阶段是否匹配
                        errorInfo = RP_CheckMEIsMatch(paraItem, newMaster, isEdit, FlowChart_Version_Comment, Part_Types, userInfo);
                        if (!string.IsNullOrEmpty(errorInfo))
                        {
                            return errorInfo;
                        }
                        #endregion

                        #region 读取Excel内容，判断单元格是否为空并赋值,判断绑定序号，制程序号是否重复
                        var propertiesContent = FlowchartImportCommon.GetMEContentColumn();
                        int startRow = 6;
                        errorInfo = RP_SetAndCheckExcelContent(detailDTOMEList, isEdit, flowchartSheet, startRow, newMaster, FlowChart_Version_Comment, userInfo);
                        if (!string.IsNullOrEmpty(errorInfo))
                        {
                            return errorInfo;
                        }
                        #endregion
                    }

                    #region 获取另外两个Excel Sheet的信息 
                    //设备需求Sheet
                    errorInfo = RP_SetAndCHeckExcelEquipment(processingNeedSheet, equipmentList, "主加工设备", userInfo);

                    //自动化设备需求Sheet
                    errorInfo = RP_SetAndCHeckExcelEquipment(autoNeedSheet, aotuEquipmentList, "自动化设备", userInfo);

                    //辅助设备需求Sheet
                    errorInfo = RP_SetAndCHeckExcelEquipment(auxiliaryNeedSheet, aotuEquipmentList, "辅助设备", userInfo);
                    #endregion

                    #endregion


                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        private string RP_CheckExeclValid(ExcelWorksheet worksheet, int iRow, int FlowChart_Master_UID, RP_ME_ExcelImportParas paraItem, out string Part_Types)
        {
            Part_Types = string.Empty;
            string errorInfo = string.Empty;
            if (worksheet == null)
            {
                errorInfo = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet");

                return errorInfo;
            }
            //头样式设置
            var propertiesHead = FlowchartImportCommon.GetHeadColumn();

            bool allColumnsAreEmpty = true;
            for (var i = 2; i <= propertiesHead.Length + 1; i++)
            {
                if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                {
                    allColumnsAreEmpty = false;
                    break;
                }
            }
            if (allColumnsAreEmpty)
            {
                errorInfo = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Excelformat");
                return errorInfo;
            }

            //客户
            string BU_D_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 1].Value).Trim();
            //专案名称
            string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value).Trim();
            //部件
            Part_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value).Trim();
            //阶段
            string Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value).Trim();
            //廠區
            string Plant = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 5].Value).Trim();
            // OP 類型(BG)
            string BG = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 6].Value).Trim();
            //// 顏色
            //string Color = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 7].Value).Trim();
            //// 每日目标产量
            //int Daily_Targetoutput = 0;
            //var _Daily_Targetoutput = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 8].Value).Trim();
            //if (int.TryParse(_Daily_Targetoutput, out Daily_Targetoutput))
            //{
            //    Daily_Targetoutput = int.Parse(Daily_Targetoutput.ToString());
            //}
            //// 直通率
            //decimal FPY = new decimal();
            //var _FPY = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 9].Value).Trim();
            //if (decimal.TryParse(_FPY, out FPY))
            //{
            //    FPY = decimal.Parse(_FPY.ToString());
            //}
            //// 版本说明
            //string FlowChart_Version_Comment = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 11].Value).Trim();

            if (string.IsNullOrWhiteSpace(BU_D_Name) || string.IsNullOrWhiteSpace(Project_Name) || string.IsNullOrWhiteSpace(Part_Types) || string.IsNullOrWhiteSpace(Product_Phase)
                || string.IsNullOrWhiteSpace(Plant) || string.IsNullOrWhiteSpace(BG))
            {
                errorInfo = worksheet.Name + this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.ErrorImport");
                return errorInfo;
            }
            var apiUrl_allOrgBom = string.Format("Fixture/GetAllOrgBomAPI");
            HttpResponseMessage responMessage_allOrgBom = APIHelper.APIGetAsync(apiUrl_allOrgBom);
            var allOrgBom = responMessage_allOrgBom.Content.ReadAsStringAsync().Result;
            List<OrgBomDTO> allOrgBomList = JsonConvert.DeserializeObject<List<OrgBomDTO>>(allOrgBom);
            var orgList = allOrgBomList.Where(r => r.Plant == Plant && r.BG == BG).ToList();
            if(orgList.Count() == 0)
            {
                errorInfo = worksheet.Name + string.Format("找不到对应厂区{0}，OP类型{1}，不能更新", Plant, BG);
                return errorInfo;
            }
            paraItem.BU_D_Name = BU_D_Name.Trim();
            paraItem.Project_Name = Project_Name.Trim();
            paraItem.Part_Types = Part_Types.Trim();
            paraItem.Product_Phase = Product_Phase.Trim();
            paraItem.FlowChart_Master_UID = FlowChart_Master_UID;
            paraItem.Plant_Organization = Plant;
            paraItem.Plant_Organization_UID = orgList.First().Plant_Organization_UID;
            paraItem.BG = BG;
            //paraItem.Color = Color;
            //paraItem.Daily_Targetoutput = Daily_Targetoutput;
            //paraItem.FPY = FPY;
            //paraItem.FlowChart_Version_Comment = FlowChart_Version_Comment;
            return errorInfo;
        }

        private string RP_CheckMEIsMatch(RP_ME_ExcelImportParas paraItem, RP_M newMaster, bool isEdit, string FlowChart_Version_Comment, string Part_Types, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            var orgList = userInfo.OrgInfo.Select(m => m.OPType_OrganizationUID.Value).ToList();
            paraItem.Organization_UIDList = orgList;
            if (isEdit)
            {
                paraItem.isEdit = true;
                //paraItem.FlowChart_Master_UID = 
            }
            else
            {
                paraItem.isEdit = false;
            }
            var apiUrl = string.Format("ProductionResourcePlan/CheckMEIsExistsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(paraItem, apiUrl);
            var projectUIDOrFLMasterUID = responMessage.Content.ReadAsStringAsync().Result;
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
                newMaster.RP_Flowchart_Master_UID = Convert.ToInt32(idList[0]);
                newMaster.Project_UID = Convert.ToInt32(idList[1]);
                newMaster.FlowChart_Version = Convert.ToInt32(idList[2]);
                newMaster.BG_Organization_UID = Convert.ToInt32(idList[3]);
                newMaster.Plant_Organization_UID = paraItem.Plant_Organization_UID;
            }
            else
            {
                var idList = projectUIDOrFLMasterUID.Split('_').ToList();
                newMaster.Project_UID = Convert.ToInt32(idList[0]);
                newMaster.BG_Organization_UID = Convert.ToInt32(idList[1]);
                newMaster.Plant_Organization_UID = paraItem.Plant_Organization_UID;
                newMaster.FlowChart_Version = 1;
                newMaster.Created_UID = userInfo.Account_UID;
                newMaster.Created_Date = DateTime.Now;
            }
            newMaster.Part_Types = Part_Types.Trim();
            newMaster.FlowChart_Version_Comment = FlowChart_Version_Comment;
            newMaster.Is_Latest = true;
            newMaster.Is_Closed = false;
            newMaster.Product_Phase = paraItem.Product_Phase;
            newMaster.Color = paraItem.Color;
            newMaster.Daily_Targetoutput = paraItem.Daily_Targetoutput;
            newMaster.FPY = paraItem.FPY;
            newMaster.FlowChart_Version_Comment = paraItem.FlowChart_Version_Comment;
            newMaster.Modified_UID = userInfo.Account_UID;
            newMaster.Modified_Date = DateTime.Now;
            return errorInfo;
        }

        public string RP_SetAndCheckExcelContent(List<RP_ME_D> detailDTOMEList, bool isEdit, ExcelWorksheet worksheet, int currentRow, RP_M newMaster, string FlowChart_Version_Comment, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            var propertiesContent = FlowchartImportCommon.GetMEContentColumn();
            //不考虑跨厂区权限，他们不会操作只会浏览，默认取第一个,通过三级权限获取四级
            var plantAPI = "FlowChart/QueryAllFunctionPlantsAPI?optypeUID=" + userInfo.OrgInfo.First().OPType_OrganizationUID;
            HttpResponseMessage message = APIHelper.APIGetAsync(plantAPI);
            var jsonPlants = message.Content.ReadAsStringAsync().Result;
            var functionPlants = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(jsonPlants);


            int Binding_Seq, Process_Seq, System_FunPlant_UID;
            int?  Manpower_2Shift = null;
            decimal? Total_Cycletime = 0, Equipment_CT = null, Setup_Time = null, Manpower_Ratio = null, Equipment_RequstQty = null;
            decimal Estimate_Yield = 0, Capacity_ByHour = 0, Capacity_ByDay = 0;
            string Process_Station, FunPlant, Process, Process_Desc, Processing_Equipment = string.Empty, Automation_Equipment = string.Empty, Processing_Fixtures = string.Empty, Auxiliary_Equipment = string.Empty;
            while (Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value) != 0)
            {
                ////如果Excel单元格加了删除线则跳过此行
                //if (worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "绑定序号")].Style.Font.Strike)
                //{
                //    currentRow++;
                //    continue;
                //}
                //Binding_Seq = Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "绑定序号")].Value);
                //if (Binding_Seq == 83)
                //{
                //    //int abc = 1;
                //}
                Process_Seq = Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value);
                if (Process_Seq == 0)
                {
                    //跳出循环  Production.BindSeqNotNull
                    errorInfo = string.Format(this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.BindSeqNotNull"), worksheet.Name, currentRow);
                    return errorInfo;
                }
                Process_Station = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "工站")].Value);
                FunPlant = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "厂别")].Value);
                var hasItem = functionPlants.FirstOrDefault(m => m.FunPlant.ToLower() == FunPlant.ToLower().Trim());
                if (hasItem != null)
                {
                    System_FunPlant_UID = hasItem.System_FunPlant_UID;
                }
                else
                {
                    //跳出循环  Production.PlantNotNull
                    errorInfo = string.Format(this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.PlantNotNull"), FunPlant);
                    return errorInfo;
                }


                Process = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "工序")].Value);
                if (string.IsNullOrWhiteSpace(Process))
                {
                    //跳出循环
                    errorInfo = string.Format(this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.SheetProcessNotNull"), worksheet.Name, currentRow);
                    return errorInfo;
                }

                //工序说明
                Process_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "工序说明")].Value);


                //加工设备
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工设备")].Style.Font.Strike)
                {
                    Processing_Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工设备")].Text);
                    Processing_Equipment = Processing_Equipment.Replace("N/A", "");
                }
                //自动化设备
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "自动化设备")].Style.Font.Strike)
                {
                    Automation_Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "自动化设备")].Text);
                    Automation_Equipment = Automation_Equipment.Replace("N/A", "");
                }
                //加工治具
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工治具")].Style.Font.Strike)
                {
                    Processing_Fixtures = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工治具")].Text);
                    Processing_Fixtures = Processing_Fixtures.Replace("N/A", "");
                }
                //辅助设备
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "辅助设备")].Style.Font.Strike)
                {
                    Auxiliary_Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "辅助设备")].Text);
                    Auxiliary_Equipment = Auxiliary_Equipment.Replace("N/A", "");
                }
                //设备Cycle Time(s)
                int validInt = 0;
                decimal validDecimal = 0;
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备 Cycle Time(s)")].Style.Font.Strike)
                {
                    var ctResult = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备 Cycle Time(s)")].Value);
                    var isInt = int.TryParse(ctResult, out validInt);
                    if (isInt)
                    {
                        Equipment_CT = Math.Round(Convert.ToDecimal(ctResult), 2, MidpointRounding.AwayFromZero);
                    }
                }

                //装夹时间
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "装夹时间")].Style.Font.Strike)
                {
                    var setupTime = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "装夹时间")].Value);
                    if (int.TryParse(setupTime, out validInt))
                    {
                        Setup_Time = Convert.ToInt32(setupTime);
                    }

                }
                //Cycle Time(s)
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "Cycle Time(s)")].Style.Font.Strike)
                {
                    var cycleTime = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "Cycle Time(s)")].Value);
                    if (int.TryParse(cycleTime, out validInt))
                    {
                        Total_Cycletime = Math.Round(Convert.ToDecimal(cycleTime), 2, MidpointRounding.AwayFromZero);
                    }
                }
                //预估良率(%)
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "预估良率(%)")].Style.Font.Strike)
                {
                    var yield = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "预估良率(%)")].Value);
                    Estimate_Yield = Convert.ToDecimal(yield);
                }
                //人力配比
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "人力配比")].Style.Font.Strike)
                {
                    var ratio = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "人力配比")].Value);
                    if (ratio != null)
                    {
                        if (decimal.TryParse(ratio, out validDecimal))
                        {
                            Manpower_Ratio = Math.Round(Convert.ToDecimal(ratio), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Manpower_Ratio = 0;
                        }
                    }
                    else
                    {
                        Manpower_Ratio = null;
                    }

                }
                //产能(/1H)
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能(/1H)")].Style.Font.Strike)
                {
                    var hour = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能(/1H)")].Value);
                    if (hour != null)
                    {
                        if (decimal.TryParse(hour, out validDecimal))
                        {
                            Capacity_ByHour = Math.Round(Convert.ToDecimal(hour), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Capacity_ByHour = 0;
                        }
                    }
                    else
                    {
                        Capacity_ByHour = 0;
                    }
                }


                //产能
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能")].Style.Font.Strike)
                {
                    var day = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能")].Value);
                    if (day != null)
                    {
                        if (decimal.TryParse(day, out validDecimal))
                        {
                            Capacity_ByDay = Math.Round(Convert.ToDecimal(day), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Capacity_ByDay = 0;
                        }
                    }
                    else
                    {
                        Capacity_ByDay = 0;
                    }
                }



                //设备需求
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求")].Style.Font.Strike)
                {
                    var requstQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求")].Value);
                    if (requstQty != null)
                    {
                        if (decimal.TryParse(requstQty, out validDecimal))
                        {
                            Equipment_RequstQty = Math.Round(Convert.ToDecimal(requstQty), 1, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Equipment_RequstQty = 0;
                        }
                    }
                    else
                    {
                        Equipment_RequstQty = 0;
                    }

                    //Manpower_2Shift
                    var shift = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "人力*2班")].Value);
                    if (shift != null)
                    {
                        if (int.TryParse(requstQty, out validInt))
                        {
                            Manpower_2Shift = Convert.ToInt32(shift);
                        }
                    }
                }

                RP_ME_D newFlowchartDetailMeDTO = new RP_ME_D();
                newFlowchartDetailMeDTO.RP_Flowchart_Master_UID = newMaster.RP_Flowchart_Master_UID;
                //newFlowchartDetailMeDTO.Binding_Seq = Binding_Seq;
                newFlowchartDetailMeDTO.FunPlant_Organization_UID = System_FunPlant_UID;
                newFlowchartDetailMeDTO.Process_Seq = Process_Seq;
                newFlowchartDetailMeDTO.Process_Station = Process_Station;
                newFlowchartDetailMeDTO.Process = Process;
                newFlowchartDetailMeDTO.Process_Desc = Process_Desc;
                newFlowchartDetailMeDTO.Processing_Equipment = Processing_Equipment;
                newFlowchartDetailMeDTO.Automation_Equipment = Automation_Equipment;
                newFlowchartDetailMeDTO.Processing_Fixtures = Processing_Fixtures;
                newFlowchartDetailMeDTO.Auxiliary_Equipment = Auxiliary_Equipment;
                newFlowchartDetailMeDTO.Equipment_CT = Equipment_CT;
                newFlowchartDetailMeDTO.Setup_Time = Setup_Time;
                newFlowchartDetailMeDTO.Total_Cycletime = Total_Cycletime;
                newFlowchartDetailMeDTO.ME_Estimate_Yield = Estimate_Yield;
                newFlowchartDetailMeDTO.Manpower_Ratio = Manpower_Ratio;
                newFlowchartDetailMeDTO.Capacity_ByHour = Capacity_ByHour;
                newFlowchartDetailMeDTO.Capacity_ByDay = Capacity_ByDay;
                newFlowchartDetailMeDTO.Equipment_RequstQty = Equipment_RequstQty;
                newFlowchartDetailMeDTO.Manpower_2Shift = Manpower_2Shift;  
                //if (isEdit)
                //{
                //    newFlowchartDetailMeDTO.RP_fl = newMaster.FlowChart_Version + 1;
                //}
                //else
                //{
                //    newFlowchartDetailMeDTO.FlowChart_Version = 1;
                //}
                //newFlowchartDetailMeDTO.FlowChart_Version_Comment = newMaster.FlowChart_Version_Comment;
                newFlowchartDetailMeDTO.Created_Date = DateTime.Now;
                newFlowchartDetailMeDTO.Created_UID = userInfo.Account_UID;
                newFlowchartDetailMeDTO.Modified_Date = DateTime.Now;
                newFlowchartDetailMeDTO.Modified_UID = userInfo.Account_UID;

                detailDTOMEList.Add(newFlowchartDetailMeDTO);

                currentRow++;
            }

            //var bindingSeqCount = detailDTOMEList.GroupBy(m => m.Binding_Seq).Select(m => m.Key).Count();
            //if (detailDTOMEList.Count() != bindingSeqCount)
            //{
            //    errorInfo = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.DoubleBindSeq");
            //    return errorInfo;
            //}
            return errorInfo;
        }

        private string RP_SetAndCHeckExcelEquipment(ExcelWorksheet needSheet, List<RP_ME_D_Equipment> equipmentList, string equipment_Type, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            var propertiesContent = FlowchartImportCommon.GetMEEquipmentColumn();
            int Process_Seq, EquipmentQty, RequestQty, EQP_Variable_Qty, NPI_Current_Qty, MP_Current_Qty;
            decimal? Ratio = null, Plan_CT = null;
            string Equipment_Name, Equipment_Spec, Notes;
            int currentRow = 3;
            while (Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value) != 0)
            {
                decimal validDecimal = 0;

                Process_Seq = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value);
                Equipment_Name = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备名称")].Value);
                if (Equipment_Name == "N/A")
                {
                    currentRow++;
                    continue;
                }
                Equipment_Spec = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备规格")].Value);
                if (Equipment_Spec == "N/A")
                {
                    currentRow++;
                    continue;
                }
                //Cycle Time(s)
                var cycleTime = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "Cycle Time(s)")].Value);
                if (decimal.TryParse(cycleTime, out validDecimal))
                {
                    Plan_CT = Convert.ToDecimal(cycleTime);
                }

                //设备需求
                EquipmentQty = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求")].Value);
                //配比
                var ratio = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "配比")].Value);
                if (ratio != null)
                {
                    if (decimal.TryParse(ratio, out validDecimal))
                    {
                        Ratio = Convert.ToDecimal(ratio);
                    }
                }
                //需求数量
                RequestQty = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "需求数量")].Value);
                //设备变动数量
                EQP_Variable_Qty = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备变动数量")].Value);
                //NPI阶段现有数量
                NPI_Current_Qty = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "NPI阶段现有数量")].Value);
                //MP阶段现有数量
                MP_Current_Qty = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "MP阶段现有数量")].Value);
                //备注
                Notes = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "备注")].Value);
                if (Notes == "N/A")
                {
                    currentRow++;
                    continue;
                }

                if(equipmentList.Where(r=> r.Equipment_Name == Equipment_Name).Count() > 0)
                {
                    errorInfo = needSheet.Name + ": " + this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.ErrorSamneProcess") + " '" + Equipment_Name + "'";
                    return errorInfo;
                }

                RP_ME_D_Equipment equipmentItem = new RP_ME_D_Equipment();
                equipmentItem.Process_Seq = Process_Seq;
                equipmentItem.Equipment_Name = Equipment_Name;
                equipmentItem.Equipment_Spec = Equipment_Spec;
                equipmentItem.Equipment_Type = equipment_Type;
                equipmentItem.Plan_CT = Plan_CT;
                equipmentItem.Equipment_Qty = EquipmentQty;
                equipmentItem.Ratio = Ratio;
                equipmentItem.Request_Qty = RequestQty;
                equipmentItem.EQP_Variable_Qty = EQP_Variable_Qty;
                equipmentItem.NPI_Current_Qty = NPI_Current_Qty;
                equipmentItem.MP_Current_Qty = MP_Current_Qty;
                equipmentItem.Notes = Notes;
                equipmentItem.Created_Date = DateTime.Now;
                equipmentItem.Created_UID = userInfo.Account_UID;
                equipmentItem.Modified_Date = DateTime.Now;
                equipmentItem.Modified_UID = userInfo.Account_UID;
                equipmentList.Add(equipmentItem);

                currentRow++;
            }
            return errorInfo;
        }
        #endregion

        #region 舊Code
        public string ImportCheck(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, List<ProductionPlanningModelGet> productGetList,
            List<FlowchartDetailMEEquipmentDTO> equipmentList, List<FlowchartDetailMEEquipmentDTO> aotuEquipmentList)
        {

            string errorInfo = string.Empty;
            var userInfo = HttpContext.Current.Session[SessionConstants.CurrentUserInfo] as CustomUserInfoVM;

            if (userInfo == null || userInfo.OrgInfo.Count() == 0 || userInfo.Plant_OrganizationUIDList.Count() == 0)
            {
                return this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.NotSetPlant");
            }
            try
            {
                using (var xlPackage = new ExcelPackage(uploadName.InputStream))
                {
                    List<ExcelWorksheet> flowchartSheets = new List<ExcelWorksheet>();
                    var sheetList = xlPackage.Workbook.Worksheets.Where(m => m.Name.Contains("Flowchart")).ToList();
                    if (isEdit) //如果是专案更新，则只取第一个sheet
                    {
                        flowchartSheets.Add(sheetList.First());
                    }
                    else
                    {
                        flowchartSheets = sheetList;
                    }

                    var processingequipName = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.ProcessingEquip");
                    var autoEquipName = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.AutoEquip");
                    var auxiliaryEquipName = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.AuxiliaryEquip");
                    var needSheet = xlPackage.Workbook.Worksheets.Where(m => m.Name == processingequipName).First();
                    var autoNeedSheet = xlPackage.Workbook.Worksheets.Where(m => m.Name == autoEquipName).First();
                    var auxiliaryNeedSheet = xlPackage.Workbook.Worksheets.Where(m => m.Name == auxiliaryEquipName).First();
                    int iRow = 2;
                    string Part_Types;

                    #region 判断Excel是否合法
                    foreach (var flowchartSheet in flowchartSheets)
                    {
                        ProductionPlanningModelGet pGet = new ProductionPlanningModelGet();
                        FlowChartExcelImportParas paraItem = new FlowChartExcelImportParas();
                        FlowChartMasterDTO newMaster = new FlowChartMasterDTO();
                        List<FlowchartDetailMEDTO> detailDTOMEList = new List<FlowchartDetailMEDTO>();
                        pGet.flowchartMasterDTO = newMaster;
                        pGet.flowchartDetailMeDTOList = detailDTOMEList;
                        productGetList.Add(pGet);

                        #region 判断客户，专案，名称，部件，阶段是否为空
                        errorInfo = CheckExeclValid(flowchartSheet, iRow, FlowChart_Master_UID, paraItem, out Part_Types);
                        if (!string.IsNullOrEmpty(errorInfo))
                        {
                            return errorInfo;
                        }
                        #endregion

                        #region 判断新增和编辑都检查客户，专案名称，部件，阶段是否匹配
                        errorInfo = CheckMEIsMatch(paraItem, newMaster, isEdit, FlowChart_Version_Comment, Part_Types, userInfo);
                        if (!string.IsNullOrEmpty(errorInfo))
                        {
                            return errorInfo;
                        }
                        #endregion

                        #region 读取Excel内容，判断单元格是否为空并赋值,判断绑定序号，制程序号是否重复
                        var propertiesContent = FlowchartImportCommon.GetMEContentColumn();
                        int startRow = 6;
                        errorInfo = SetAndCheckExcelContent(detailDTOMEList, isEdit, flowchartSheet, startRow, newMaster, FlowChart_Version_Comment, userInfo);
                        if (!string.IsNullOrEmpty(errorInfo))
                        {
                            return errorInfo;
                        }
                        #endregion
                    }

                    //#region 获取另外两个Excel Sheet的信息 
                    ////设备需求Sheet
                    //errorInfo = SetAndCHeckExcelEquipment(needSheet, equipmentList, userInfo);

                    ////自动化设备需求Sheet
                    //errorInfo = SetAndCHeckExcelAutoEquipment(autoNeedSheet, aotuEquipmentList, userInfo);
                    //#endregion

                    #endregion


                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        private string CheckExeclValid(ExcelWorksheet worksheet, int iRow, int FlowChart_Master_UID, FlowChartExcelImportParas paraItem, out string Part_Types)
        {
            Part_Types = string.Empty;
            string errorInfo = string.Empty;
            if (worksheet == null)
            {
                errorInfo = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Common.NoExcelSheet");

                return errorInfo;
            }
            //头样式设置
            var propertiesHead = FlowchartImportCommon.GetHeadColumn();

            bool allColumnsAreEmpty = true;
            for (var i = 2; i <= propertiesHead.Length + 1; i++)
            {
                if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                {
                    allColumnsAreEmpty = false;
                    break;
                }
            }
            if (allColumnsAreEmpty)
            {
                errorInfo = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "QA.Excelformat");
                return errorInfo;
            }

            //客户
            string BU_D_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 1].Value);
            //专案名称
            string Project_Name = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 2].Value);
            //部件
            Part_Types = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 3].Value);
            //阶段
            string Product_Phase = ExcelHelper.ConvertColumnToString(worksheet.Cells[iRow, 4].Value);

            if (string.IsNullOrWhiteSpace(BU_D_Name) || string.IsNullOrWhiteSpace(Project_Name) || string.IsNullOrWhiteSpace(Part_Types) || string.IsNullOrWhiteSpace(Product_Phase))
            {
                errorInfo = worksheet.Name + this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.ErrorImport");
                return errorInfo;
            }

            paraItem.BU_D_Name = BU_D_Name.Trim();
            paraItem.Project_Name = Project_Name.Trim();
            paraItem.Part_Types = Part_Types.Trim();
            paraItem.Product_Phase = Product_Phase.Trim();
            paraItem.FlowChart_Master_UID = FlowChart_Master_UID;
            return errorInfo;
        }

        private string CheckMEIsMatch(FlowChartExcelImportParas paraItem, FlowChartMasterDTO newMaster, bool isEdit, string FlowChart_Version_Comment, string Part_Types, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            var orgList = userInfo.OrgInfo.Select(m => m.OPType_OrganizationUID.Value).ToList();
            paraItem.Organization_UIDList = orgList;
            if (isEdit)
            {
                paraItem.isEdit = true;
                //paraItem.FlowChart_Master_UID = 
            }
            else
            {
                paraItem.isEdit = false;
            }
            var apiUrl = string.Format("ProductionResourcePlan/CheckMEIsExistsAPI");
            HttpResponseMessage responMessage = APIHelper.APIPostAsync(paraItem, apiUrl);
            var projectUIDOrFLMasterUID = responMessage.Content.ReadAsStringAsync().Result;
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
            newMaster.CurrentDepartent = "ME";
            newMaster.Modified_UID = userInfo.Account_UID;
            newMaster.Modified_Date = DateTime.Now;
            return errorInfo;
        }

        public string SetAndCheckExcelContent(List<FlowchartDetailMEDTO> detailDTOMEList, bool isEdit, ExcelWorksheet worksheet, int currentRow, FlowChartMasterDTO newMaster, string FlowChart_Version_Comment, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            var propertiesContent = FlowchartImportCommon.GetMEContentColumn();
            //不考虑跨厂区权限，他们不会操作只会浏览，默认取第一个,通过三级权限获取四级
            var plantAPI = "FlowChart/QueryAllFunctionPlantsAPI?optypeUID=" + userInfo.OrgInfo.First().OPType_OrganizationUID;
            HttpResponseMessage message = APIHelper.APIGetAsync(plantAPI);
            var jsonPlants = message.Content.ReadAsStringAsync().Result;
            var functionPlants = JsonConvert.DeserializeObject<List<SystemFunctionPlantDTO>>(jsonPlants);




            int Binding_Seq, Process_Seq, System_FunPlant_UID;
            int? Manpower_2Shift = null, Equipment_CT = null;
            decimal? Manpower_Ratio = null, Setup_Time = null, Equipment_RequstQty = null;
            decimal Total_Cycletime = 0, Estimate_Yield = 0, Capacity_ByHour = 0, Capacity_ByDay = 0;
            string Process_Station, FunPlant, Process, Process_Desc, Color = string.Empty, Processing_Equipment = string.Empty, Automation_Equipment = string.Empty, Processing_Fixtures = string.Empty, Auxiliary_Equipment = string.Empty;
            while (Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value) != 0)
            {
                ////如果Excel单元格加了删除线则跳过此行
                //if (worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "绑定序号")].Style.Font.Strike)
                //{
                //    currentRow++;
                //    continue;
                //}
                //Binding_Seq = Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "绑定序号")].Value);
                //if (Binding_Seq == 83)
                //{
                //    //int abc = 1;
                //}
                Process_Seq = Convert.ToInt32(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value);
                if (Process_Seq == 0)
                {
                    //跳出循环  Production.BindSeqNotNull
                    errorInfo = string.Format(this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.BindSeqNotNull"), worksheet.Name, currentRow);
                    return errorInfo;
                }
                Process_Station = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "工站")].Value);
                FunPlant = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "厂别")].Value);
                var hasItem = functionPlants.FirstOrDefault(m => m.FunPlant.ToLower() == FunPlant.ToLower().Trim());
                if (hasItem != null)
                {
                    System_FunPlant_UID = hasItem.System_FunPlant_UID;
                }
                else
                {
                    //跳出循环  Production.PlantNotNull
                    errorInfo = string.Format(this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.PlantNotNull"), FunPlant);
                    return errorInfo;
                }


                Process = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "工序")].Value);
                if (string.IsNullOrWhiteSpace(Process))
                {
                    //跳出循环
                    errorInfo = string.Format(this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.SheetProcessNotNull"), worksheet.Name, currentRow);
                    return errorInfo;
                }

                //工序说明
                Process_Desc = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "工序说明")].Value);
                //颜色
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "颜色")].Style.Font.Strike)
                {
                    Color = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "颜色")].Value);
                }

                //加工设备
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工设备")].Style.Font.Strike)
                {
                    Processing_Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工设备")].Text);
                    Processing_Equipment = Processing_Equipment.Replace("N/A", "");
                }
                //自动化设备
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "自动化设备")].Style.Font.Strike)
                {
                    Automation_Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "自动化设备")].Text);
                    Automation_Equipment = Automation_Equipment.Replace("N/A", "");
                }
                //加工治具
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工治具")].Style.Font.Strike)
                {
                    Processing_Fixtures = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "加工治具")].Text);
                    Processing_Fixtures = Processing_Fixtures.Replace("N/A", "");
                }
                //辅助设备
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "辅助设备")].Style.Font.Strike)
                {
                    Auxiliary_Equipment = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "辅助设备")].Text);
                    Auxiliary_Equipment = Auxiliary_Equipment.Replace("N/A", "");
                }
                //设备Cycle Time(s)
                int validInt = 0;
                decimal validDecimal = 0;
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备 Cycle Time(s)")].Style.Font.Strike)
                {
                    var ctResult = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备 Cycle Time(s)")].Value);
                    var isInt = int.TryParse(ctResult, out validInt);
                    if (isInt)
                    {
                        Equipment_CT = Convert.ToInt32(ctResult);
                    }
                }

                //装夹时间
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "装夹时间")].Style.Font.Strike)
                {
                    var setupTime = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "装夹时间")].Value);
                    if (decimal.TryParse(setupTime, out validDecimal))
                    {
                        Setup_Time = Convert.ToDecimal(setupTime);
                    }

                }
                //Cycle Time(s)
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "Cycle Time(s)")].Style.Font.Strike)
                {
                    var cycleTime = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "Cycle Time(s)")].Value);
                    if (decimal.TryParse(cycleTime, out validDecimal))
                    {
                        Total_Cycletime = Convert.ToDecimal(cycleTime);
                    }
                }
                //预估良率(%)
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "预估良率(%)")].Style.Font.Strike)
                {
                    var yield = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "预估良率(%)")].Value);
                    Estimate_Yield = Convert.ToDecimal(yield);
                }
                //人力配比
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "人力配比")].Style.Font.Strike)
                {
                    var ratio = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "人力配比")].Value);
                    if (ratio != null)
                    {
                        if (decimal.TryParse(ratio, out validDecimal))
                        {
                            Manpower_Ratio = Math.Round(Convert.ToDecimal(ratio), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Manpower_Ratio = 0;
                        }
                    }
                    else
                    {
                        Manpower_Ratio = null;
                    }

                }
                //产能(/1H)
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能(/1H)")].Style.Font.Strike)
                {
                    var hour = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能(/1H)")].Value);
                    if (hour != null)
                    {
                        if (decimal.TryParse(hour, out validDecimal))
                        {
                            Capacity_ByHour = Math.Round(Convert.ToDecimal(hour), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Capacity_ByHour = 0;
                        }
                    }
                    else
                    {
                        Capacity_ByHour = 0;
                    }
                }


                //产能
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能")].Style.Font.Strike)
                {
                    var day = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "产能")].Value);
                    if (day != null)
                    {
                        if (decimal.TryParse(day, out validDecimal))
                        {
                            Capacity_ByDay = Math.Round(Convert.ToDecimal(day), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Capacity_ByDay = 0;
                        }
                    }
                    else
                    {
                        Capacity_ByDay = 0;
                    }
                }



                //设备需求
                if (!worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求")].Style.Font.Strike)
                {
                    var requstQty = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求")].Value);
                    if (requstQty != null)
                    {
                        if (decimal.TryParse(requstQty, out validDecimal))
                        {
                            Equipment_RequstQty = Math.Round(Convert.ToDecimal(requstQty), 1, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Equipment_RequstQty = 0;
                        }
                    }
                    else
                    {
                        Equipment_RequstQty = 0;
                    }

                    //Manpower_2Shift
                    var shift = ExcelHelper.ConvertColumnToString(worksheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "人力*2班")].Value);
                    if (shift != null)
                    {
                        if (int.TryParse(requstQty, out validInt))
                        {
                            Manpower_2Shift = Convert.ToInt32(shift);
                        }
                    }
                }

                FlowchartDetailMEDTO newFlowchartDetailMeDTO = new FlowchartDetailMEDTO();
                newFlowchartDetailMeDTO.FlowChart_Master_UID = newMaster.FlowChart_Master_UID;
                //newFlowchartDetailMeDTO.Binding_Seq = Binding_Seq;
                newFlowchartDetailMeDTO.Process_Seq = Process_Seq;
                newFlowchartDetailMeDTO.Process = Process;
                newFlowchartDetailMeDTO.Process_Desc = Process_Desc;
                newFlowchartDetailMeDTO.Color = Color;
                newFlowchartDetailMeDTO.Processing_Equipment = Processing_Equipment;
                newFlowchartDetailMeDTO.Automation_Equipment = Automation_Equipment;
                newFlowchartDetailMeDTO.Processing_Fixtures = Processing_Fixtures;
                newFlowchartDetailMeDTO.Auxiliary_Equipment = Auxiliary_Equipment;
                newFlowchartDetailMeDTO.Equipment_CT = Equipment_CT;
                newFlowchartDetailMeDTO.Setup_Time = Setup_Time;
                newFlowchartDetailMeDTO.Total_Cycletime = Total_Cycletime;
                newFlowchartDetailMeDTO.Estimate_Yield = Estimate_Yield;
                newFlowchartDetailMeDTO.Manpower_Ratio = Manpower_Ratio;
                newFlowchartDetailMeDTO.Capacity_ByHour = Capacity_ByHour;
                newFlowchartDetailMeDTO.Capacity_ByDay = Capacity_ByDay;
                newFlowchartDetailMeDTO.Equipment_RequstQty = Equipment_RequstQty;
                newFlowchartDetailMeDTO.Manpower_2Shift = Manpower_2Shift;
                newFlowchartDetailMeDTO.Process_Station = Process_Station;
                newFlowchartDetailMeDTO.System_FunPlant_UID = System_FunPlant_UID;
                if (isEdit)
                {
                    newFlowchartDetailMeDTO.FlowChart_Version = newMaster.FlowChart_Version + 1;
                }
                else
                {
                    newFlowchartDetailMeDTO.FlowChart_Version = 1;
                }
                newFlowchartDetailMeDTO.FlowChart_Version_Comment = newMaster.FlowChart_Version_Comment;
                newFlowchartDetailMeDTO.Created_Date = DateTime.Now;
                newFlowchartDetailMeDTO.Created_UID = userInfo.Account_UID;
                newFlowchartDetailMeDTO.Modified_Date = DateTime.Now;
                newFlowchartDetailMeDTO.Modified_UID = userInfo.Account_UID;

                detailDTOMEList.Add(newFlowchartDetailMeDTO);

                currentRow++;
            }

            //var bindingSeqCount = detailDTOMEList.GroupBy(m => m.Binding_Seq).Select(m => m.Key).Count();
            //if (detailDTOMEList.Count() != bindingSeqCount)
            //{
            //    errorInfo = this.GetLocaleStringResource(PISSessionContext.Current.CurrentWorkingLanguage.System_Language_UID, "Production.DoubleBindSeq");
            //    return errorInfo;
            //}
            return errorInfo;
        }

        private string SetAndCHeckExcelEquipment(ExcelWorksheet needSheet, List<FlowchartDetailMEEquipmentDTO> equipmentList, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            var propertiesContent = FlowchartImportCommon.GetMEEquipmentColumn();
            int Process_Seq, EquipmentQty, RequestQty;
            decimal? Ratio = null, Plan_CT = null;
            string Equipment_Name;
            int currentRow = 3;
            while (Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value) != 0)
            {
                decimal validDecimal = 0;

                Process_Seq = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value);
                Equipment_Name = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备名称")].Value);
                if (Equipment_Name == "N/A")
                {
                    currentRow++;
                    continue;
                }
                //Cycle Time(s)
                var cycleTime = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "Cycle Time(s)")].Value);
                if (decimal.TryParse(cycleTime, out validDecimal))
                {
                    Plan_CT = Convert.ToDecimal(cycleTime);
                }

                //设备需求
                EquipmentQty = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "设备需求")].Value);
                //配比
                var ratio = ExcelHelper.ConvertColumnToString(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "配比")].Value);
                if (ratio != null)
                {
                    if (decimal.TryParse(ratio, out validDecimal))
                    {
                        Ratio = Convert.ToDecimal(ratio);
                    }
                }
                //需求数量
                RequestQty = Convert.ToInt32(needSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "需求数量")].Value);

                FlowchartDetailMEEquipmentDTO equipmentItem = new FlowchartDetailMEEquipmentDTO();
                equipmentItem.Process_Seq = Process_Seq;
                equipmentItem.Equipment_Name = Equipment_Name;
                equipmentItem.Plan_CT = Plan_CT;
                equipmentItem.EquipmentQty = EquipmentQty;
                equipmentItem.Ratio = Ratio;
                equipmentItem.RequestQty = RequestQty;
                equipmentItem.EquipmentType = "主加工设备";
                equipmentItem.Created_Date = DateTime.Now;
                equipmentItem.Created_UID = userInfo.Account_UID;
                equipmentItem.Modified_Date = DateTime.Now;
                equipmentItem.Modified_UID = userInfo.Account_UID;
                equipmentList.Add(equipmentItem);

                currentRow++;
            }
            return errorInfo;
        }

        private string SetAndCHeckExcelAutoEquipment(ExcelWorksheet autoNeedSheet, List<FlowchartDetailMEEquipmentDTO> autoEquipmentList, CustomUserInfoVM userInfo)
        {
            string errorInfo = string.Empty;
            var propertiesContent = FlowchartImportCommon.GetMEAutoEquipmentColumn();
            int Process_Seq, EquipmentQty, RequestQty;
            decimal? Ratio = null;
            string Equipment_Name;
            int currentRow = 3;
            while (Convert.ToInt32(autoNeedSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value) != 0)
            {
                decimal validDecimal = 0;
                if (currentRow == 102)
                {
                }

                Process_Seq = Convert.ToInt32(autoNeedSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "制程序号")].Value);
                Equipment_Name = ExcelHelper.ConvertColumnToString(autoNeedSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "設備名稱")].Value);
                if (Equipment_Name == "N/A")
                {
                    currentRow++;
                    continue;
                }
                //设备需求
                EquipmentQty = Convert.ToInt32(autoNeedSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "主設備需求")].Value);


                //配比
                var ratio = ExcelHelper.ConvertColumnToString(autoNeedSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "备用")].Value);
                if (ratio != null)
                {
                    if (decimal.TryParse(ratio, out validDecimal))
                    {
                        Ratio = Convert.ToDecimal(ratio);
                    }
                }
                //需求数量
                RequestQty = Convert.ToInt32(autoNeedSheet.Cells[currentRow, ExcelHelper.GetColumnIndex(propertiesContent, "需求數量")].Value);

                FlowchartDetailMEEquipmentDTO equipmentItem = new FlowchartDetailMEEquipmentDTO();
                equipmentItem.Process_Seq = Process_Seq;
                equipmentItem.Equipment_Name = Equipment_Name;
                equipmentItem.EquipmentQty = EquipmentQty;
                equipmentItem.Ratio = Ratio;
                equipmentItem.RequestQty = RequestQty;
                equipmentItem.EquipmentType = "自动化设备";
                equipmentItem.Created_Date = DateTime.Now;
                equipmentItem.Created_UID = userInfo.Account_UID;
                equipmentItem.Modified_Date = DateTime.Now;
                equipmentItem.Modified_UID = userInfo.Account_UID;
                autoEquipmentList.Add(equipmentItem);

                currentRow++;
            }

            return errorInfo;
        }
        #endregion


    }
}