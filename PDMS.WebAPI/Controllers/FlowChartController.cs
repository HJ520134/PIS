using PDMS.Core;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Newtonsoft.Json;
using PDMS.Model.ViewModels;
using PDMS.Service.FlowChart;
using PDMS.Model.EntityDTO;

namespace PDMS.WebAPI.Controllers
{
    public class FlowChartController : ApiControllerBase
    {
        IFlowChartMasterService flowChartService;
        IFlowChartDetailService flowChartDetailService;
        IFlowChartPCMHService flowChartPCMHService;
        IFlowChartPlanService flowChartPlanService;
        //IFlowChartService flowChartService;
        ICommonService commonService;

        public FlowChartController(
            IFlowChartMasterService flowChartService,
            IFlowChartDetailService flowChartDetailService,
            IFlowChartPCMHService flowChartPCMHService,
            IFlowChartPlanService flowChartPlanService,
            ICommonService commonService)
        {
            this.flowChartService = flowChartService;
            this.flowChartDetailService = flowChartDetailService;
            this.flowChartPCMHService = flowChartPCMHService;
            this.flowChartPlanService = flowChartPlanService;
            this.commonService = commonService;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryFlowChartDataAPI(dynamic data)
        {
            var searchModel = JsonConvert.DeserializeObject<GetProcessSearch>(data.ToString());
            //添加当前用户的Project
            var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
            var result = flowChartPlanService.QueryProjectList(searchModel.user_account_uid, searchModel.MHFlag_MulitProject);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryPlantByUser(int userid)
        {
            var result = flowChartPlanService.QueryPlantByUser(userid);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryFlowChartDataByMasterUid(int flowChartMaster_Uid)
        {
            var result = flowChartPlanService.QueryFlowChartDataByMasterUid(flowChartMaster_Uid);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryFlowChartsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FlowChartModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            //var currentProject = commonService.GetFiltProject(searchModel.OpTypes, searchModel.Project_UID);
            var flCharts = flowChartService.QueryFlowCharts(searchModel, page);
            return Ok(flCharts);
        }

        /// <summary>
        /// 获取修改记录详情表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetWIPAlterRecordDetialAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<WIPDetialSearchParam>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var WIPAlter = flowChartService.GetWIPAlterRecordDetialAPI(searchModel, page);
            return Ok(WIPAlter);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public string CheckFlowChartAPI(FlowChartExcelImportParas parasItem)
        {
            var result = flowChartService.CheckFlowChart(parasItem);
            return result;
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFunctionPlantsAPI(int optypeUID)
        {
            var result = flowChartService.QueryAllFunctionPlants(optypeUID);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFlowChartMasterID(string BU_D_Name, string Project_Name, string Part_Types, string Product_Phase)
        {
            var result = flowChartService.GetFlowChartMasterID(BU_D_Name, Project_Name, Part_Types, Product_Phase);
            return Ok(result);
        }

        /// <summary>
        /// 检查FlowChartDetial是否存在 失败的时候返回不存在的绑定序号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CheckFlowChart_DetailByID(dynamic data)
        {
            try
            {
                var entity = JsonConvert.DeserializeObject<Machine_CustomerAndStationDTO>(data.ToString());
                var flowChart_Master_UID = int.Parse(entity.PIS_Customer_Name);
                var result = string.Empty;
                foreach (var item in entity.Storages)
                {
                    var flowChart_DetailByID = flowChartDetailService.GetFlowChart_DetailByID(flowChart_Master_UID, int.Parse(item.PIS_Station_Name));
                    if (flowChart_DetailByID == 0)
                    {
                        result = item.PIS_Station_Name;
                        return string.Format("绑定序号:{0}不存在", result);
                    }
                }
                return result = "SUCCESS";
            }
            catch (Exception ex)
            {
                return "FAilED";
            }         
        }

        //[AcceptVerbs("GET")]
        //[IgnoreDBAuthorize]
        //public IHttpActionResult GetFlowProcessID(int Process_Seq,int Binding_Seq)
        //{
        //    var result = flowChartService.GetFlowChartMasterID(Process_Seq, Binding_Seq, Part_Types, Product_Phase);
        //    return Ok(result);
        //}


        public void ImportFlowChartAPI(FlowChartImport importItem, bool isEdit, int accountID)
        {
            if (isEdit)
            {
                flowChartService.ImportFlowUpdateChart(importItem, accountID);
            }
            else
            {
                flowChartService.ImportFlowChart(importItem, accountID);
            }
        }

        public void ImportFlowChartWUXI_MAPI(FlowChartImport importItem, bool isEdit, int accountID)
        {
            if (isEdit)
            {
                flowChartService.ImportFlowUpdateChartWUXI_M(importItem, accountID);
            }
            else
            {
                flowChartService.ImportFlowChartWUXI_M(importItem, accountID);
            }
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public void ImportFlowChartMGDataAPI(dynamic json, int FlowChart_Master_UID)
        {
            var jsonData = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FlowChartMgDataDTO>>(jsonData);
            flowChartService.ImportFlowChartMGData(list, FlowChart_Master_UID);
        }


        [HttpPost]
        [IgnoreDBAuthorize]
        public void ImportFlowChartIEMGDataAPI(dynamic json, int FlowChart_Master_UID)
        {
            var jsonData = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FlowChartIEMgDataDTO>>(jsonData);
            flowChartService.ImportFlowChartIEMGData(list, FlowChart_Master_UID);
        }
        
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryFlowChartAPI(int FlowChart_Master_UID)
        {
            var result = flowChartService.QueryFlowChart(FlowChart_Master_UID);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryHistoryFlowChartAPI(int FlowChart_Master_UID)
        {
            var result = flowChartService.QueryHistoryFlowChart(FlowChart_Master_UID);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult DoHistoryExcelExportAPI(int id, int version)
        {
            var result = flowChartService.ExportFlowChart(id, version);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult DoHistoryExcelExportWUXI_MAPI(int id, int version)
        {
            var result = flowChartService.ExportFlowWUXI_MChart(id, version);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public bool ClosedFLAPI(int FlowChart_Master_UID, bool isClosed)
        {
            var result = flowChartService.ClosedFL(FlowChart_Master_UID, isClosed);
            return result;
        }

        [HttpPost]
        public IHttpActionResult QueryFLDetailListAPI(int id, int Version)
        {
            var result = flowChartDetailService.QueryFLDetailList(id, Version);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult QueryFLDetailWUXI_MListAPI(int id, int Version)
        {
            var result = flowChartDetailService.QueryFLDetailWUXI_MList(id, Version);
            return Ok(result);
        }



        [AcceptVerbs("GET")]
        public IHttpActionResult QueryFLDetailByIDAPI(int id)
        {
            var result = flowChartDetailService.QueryFLDetailByID(id);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryFLDetailListAPI(int id, string week)
        {
            var result = flowChartPlanService.QueryFLDetailList(id, week);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryFunPlantAPI(int id)
        {
            var result = flowChartService.QueryFunPlant(id);
            return Ok(result);
        }

        public IHttpActionResult SaveFLDetailInfoAPI(FlowChartDetailDTO dto, int AccountID)
        {
            var result = flowChartDetailService.SaveFLDetailInfo(dto, AccountID);
            return Ok(result);
        }

        public void SaveAllDetailInfoAPI(dynamic json, int AccountID)
        {
            var jsonData = json.ToString();
            var list = JsonConvert.DeserializeObject<List<FlowChartDetailAndMGDataInputDTO>>(jsonData);
            flowChartDetailService.SaveAllDetailInfo(list, AccountID);
        }



        [IgnoreDBAuthorize]
        [AcceptVerbs("GET")]
        public IHttpActionResult QueryProcessMGDataAPI(int masterUID, string date)
        {
            DateTime nowDate = DateTime.Parse(date);
            var result = flowChartPlanService.QueryProcessMGData(masterUID, nowDate);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        [AcceptVerbs("GET")]
        public IHttpActionResult QueryProcessIEMGDataAPI(int masterUID, string date)
        {
            DateTime nowDate = DateTime.Parse(date);
            var result = flowChartPlanService.QueryProcessIEMGData(masterUID, nowDate);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        public string ModifyProcessMGDataAPI(FlowChartPlanManagerDTO dto)
        {
            //为了可以修改上一版本的计划，现在增加通过当前条件获取所有版本的方法，然后循环修改。

            //获取上一版的该制程对应的detail_UID
            int lastUId = flowChartPlanService.getTheLastVersionDetailUID(dto.Detail_UID);
            if (lastUId != 0)
            {
                var ent1 = flowChartPlanService.QueryProcessMGDataSingle(lastUId, dto.date);
                if (ent1 != null)
                {
                    ent1.MondayProduct_Plan = dto.MonDayProduct_Plan;
                    ent1.MondayTarget_Yield = dto.MonDayTarget_Yield;
                    ent1.MondayProper_WIP = dto.MondayProper_WIP;
                    ent1.TuesdayProduct_Plan = dto.TuesDayProduct_Plan;
                    ent1.TuesdayTarget_Yield = dto.TuesDayTarget_Yield;
                    ent1.TuesdayProper_WIP = dto.TuesdayProper_WIP;

                    ent1.WednesdayProduct_Plan = dto.WednesdayProduct_Plan;
                    ent1.WednesdayTarget_Yield = dto.WednesdayTarget_Yield;
                    ent1.WednesdayProper_WIP = dto.WednesdayProper_WIP;

                    ent1.ThursdayProduct_Plan = dto.ThursdayProduct_Plan;
                    ent1.ThursdayTarget_Yield = dto.ThursdayTarget_Yield;
                    ent1.ThursdayProper_WIP = dto.ThursdayProper_WIP;

                    ent1.FridayProduct_Plan = dto.FriDayProduct_Plan;
                    ent1.FridayTarget_Yield = dto.FridayTarget_Yield;
                    ent1.FridayProper_WIP = dto.FridayProper_WIP;

                    ent1.SaterdayProduct_Plan = dto.SaterDayProduct_Plan;
                    ent1.SaterdayTarget_Yield = dto.SaterDayTarget_Yield;
                    ent1.SaterdayProper_WIP = dto.SaterdayProper_WIP;

                    ent1.SundayProduct_Plan = dto.SunDayProduct_Plan;
                    ent1.SundayTarget_Yield = dto.SunDayTarget_Yield;
                    ent1.SundayProper_WIP = dto.SundayProper_WIP;
                    string result1 = flowChartPlanService.FlowChartPlan(ent1);
                }

            }

            var ent = flowChartPlanService.QueryProcessMGDataSingle(dto.Detail_UID, dto.date);
            ent.MondayProduct_Plan = dto.MonDayProduct_Plan;
            ent.MondayTarget_Yield = dto.MonDayTarget_Yield;
            ent.MondayProper_WIP = dto.MondayProper_WIP;
            ent.TuesdayProduct_Plan = dto.TuesDayProduct_Plan;
            ent.TuesdayTarget_Yield = dto.TuesDayTarget_Yield;
            ent.TuesdayProper_WIP = dto.TuesdayProper_WIP;

            ent.WednesdayProduct_Plan = dto.WednesdayProduct_Plan;
            ent.WednesdayTarget_Yield = dto.WednesdayTarget_Yield;
            ent.WednesdayProper_WIP = dto.WednesdayProper_WIP;

            ent.ThursdayProduct_Plan = dto.ThursdayProduct_Plan;
            ent.ThursdayTarget_Yield = dto.ThursdayTarget_Yield;
            ent.ThursdayProper_WIP = dto.ThursdayProper_WIP;

            ent.FridayProduct_Plan = dto.FriDayProduct_Plan;
            ent.FridayTarget_Yield = dto.FridayTarget_Yield;
            ent.FridayProper_WIP = dto.FridayProper_WIP;

            ent.SaterdayProduct_Plan = dto.SaterDayProduct_Plan;
            ent.SaterdayTarget_Yield = dto.SaterDayTarget_Yield;
            ent.SaterdayProper_WIP = dto.SaterdayProper_WIP;

            ent.SundayProduct_Plan = dto.SunDayProduct_Plan;
            ent.SundayTarget_Yield = dto.SunDayTarget_Yield;
            ent.SundayProper_WIP = dto.SundayProper_WIP;
            string result = flowChartPlanService.FlowChartPlan(ent);
            return result;
        }


        [IgnoreDBAuthorize]
        public string ModifyProcessIEMGDataAPI(IEPlanManagerDTO dto)
        {
            //为了可以修改上一版本的计划，现在增加通过当前条件获取所有版本的方法，然后循环修改。
            if (dto.ShiftTimeID == "白班")
            {
                dto.ShiftTimeID = "4";
            }
            if (dto.ShiftTimeID == "夜班")
            {
                dto.ShiftTimeID = "5";
            }


            //获取上一版的该制程对应的detail_UID
            int lastUId = flowChartPlanService.getTheLastVersionDetailUID(dto.Detail_UID);
            if (lastUId != 0)
            {
                //, int.Parse(dto.ShiftTimeID)
                var ent1 = flowChartPlanService.QueryProcessIEMGDataSingle(lastUId, dto.date, int.Parse(dto.ShiftTimeID));
                if (ent1 != null)
                {
                    ent1.ShiftTimeID = int.Parse(dto.ShiftTimeID);

                    ent1.MondayIE_TargetEfficacy = dto.MondayIE_TargetEfficacy;
                    ent1.MondayIE_DeptHuman = dto.MondayIE_DeptHuman;

                    ent1.TuesdayIE_TargetEfficacy = dto.TuesdayIE_TargetEfficacy;
                    ent1.TuesdayIE_DeptHuman = dto.TuesdayIE_DeptHuman;

                    ent1.WednesdayIE_TargetEfficacy = dto.WednesdayIE_TargetEfficacy;
                    ent1.WednesdayIE_DeptHuman = dto.WednesdayIE_DeptHuman;

                    ent1.TuesdayIE_TargetEfficacy = dto.TuesdayIE_TargetEfficacy;
                    ent1.TuesdayIE_DeptHuman = dto.TuesdayIE_DeptHuman;

                    ent1.FridayIE_TargetEfficacy = dto.FridayIE_TargetEfficacy;
                    ent1.FridayIE_DeptHuman = dto.FridayIE_DeptHuman;

                    ent1.SaterdayIE_TargetEfficacy = dto.SaterdayIE_TargetEfficacy;
                    ent1.SaterdayIE_DeptHuman = dto.SaterdayIE_DeptHuman;

                    ent1.SundayIE_TargetEfficacy = dto.SundayIE_TargetEfficacy;
                    ent1.SundayIE_DeptHuman = dto.SundayIE_DeptHuman;

                    
                    string result1 = flowChartPlanService.FlowIEChartPlan(ent1);
                }

            }

            var ent = flowChartPlanService.QueryProcessIEMGDataSingle(dto.Detail_UID, dto.date,int.Parse(dto.ShiftTimeID));

            ent.ShiftTimeID = int.Parse(dto.ShiftTimeID);
            ent.MondayIE_TargetEfficacy = dto.MondayIE_TargetEfficacy;
            ent.MondayIE_DeptHuman = dto.MondayIE_DeptHuman;
            ent.TuesdayIE_TargetEfficacy = dto.TuesdayIE_TargetEfficacy;
            ent.TuesdayIE_DeptHuman = dto.TuesdayIE_DeptHuman;
            ent.WednesdayIE_TargetEfficacy = dto.WednesdayIE_TargetEfficacy;
            ent.WednesdayIE_DeptHuman = dto.WednesdayIE_DeptHuman;
            ent.ThursdayIE_TargetEfficacy = dto.ThursdayIE_TargetEfficacy;
            ent.ThursdayIE_DeptHuman = dto.ThursdayIE_DeptHuman;
            ent.FridayIE_TargetEfficacy = dto.FridayIE_TargetEfficacy;
            ent.FridayIE_DeptHuman = dto.FridayIE_DeptHuman;
            ent.SaterdayIE_TargetEfficacy = dto.SaterdayIE_TargetEfficacy;
            ent.SaterdayIE_DeptHuman = dto.SaterdayIE_DeptHuman;
            ent.SundayIE_TargetEfficacy = dto.SundayIE_TargetEfficacy;
            ent.SundayIE_DeptHuman = dto.SundayIE_DeptHuman;
          
            string result = flowChartPlanService.FlowIEChartPlan(ent);
            return result;
        }

        [IgnoreDBAuthorize]
        [AcceptVerbs("GET")]
        public IHttpActionResult QueryProcessMGDataSingleAPI(int detailUID, string date)
        {
            DateTime nowDate = DateTime.Parse(date);
            var result = flowChartPlanService.QueryProcessMGDataSingle(detailUID, nowDate);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        [AcceptVerbs("GET")]
        public IHttpActionResult QueryProcessIEMGDataSingleAPI(int detailUID ,string date, int shiftID)
        {
            DateTime nowDate = DateTime.Parse(date);
            var result = flowChartPlanService.QueryProcessIEMGDataSingle(detailUID, nowDate, shiftID);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryProjectTypes(string Project)
        {
            var result = flowChartPlanService.QueryProjectTypes(Project);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryProcessByFlowchartMasterUid(int flowcharMasterUid)
        {
            var result = flowChartPlanService.QueryProcess(flowcharMasterUid);
            return Ok(result);
        }

        #region 绑定物料员
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryBindBomByMasterId(int id)
        {
            var result = flowChartPCMHService.QueryBindBomByMasterId(id);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryBomByFlowChartUIDAPI(int id, int Version, string Plants)
        {
            var list = JsonConvert.DeserializeObject<List<int>>(Plants);
            var result = flowChartPCMHService.QueryBomByFlowChartUID(id, Version, list);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryBomEditByFlowChartUIDAPI(int PC_MH_UID)
        {
            var result = flowChartPCMHService.QueryBomEditByFlowChartUID(PC_MH_UID);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult QueryFLDetailByUIDAndVersionAPI(int MasterUID, int Version, string Plants)
        {
            var list = JsonConvert.DeserializeObject<List<int>>(Plants);
            var result = flowChartDetailService.QueryFLDetailByUIDAndVersion(MasterUID, Version, list);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryBomUserInfoAPI()
        {
            var result = flowChartPCMHService.QueryBomUserInfo();
            return Ok(result);
        }

        [AcceptVerbs("POST")]
        public int CheckBomUserAPI(dynamic search)
        {
            var searchModel = JsonConvert.DeserializeObject<GetFuncPlantProcessSearch>(search.ToString());
            var result = flowChartPCMHService.CheckBomUser(searchModel);
            return result;
        }

        [HttpPost]
        public string InsertBomUserInfoAPI(dynamic json, int MasterUID, int Version)
        {
            var jsonData = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FlowChartPCMHRelationshipVM>>(jsonData);
            return flowChartPCMHService.InsertBomUserInfo(list);
        }

        public string EditFLPCBomInfoAPI(FlowChartBomGet bomItem, int CurrUser)
        {
            var result = flowChartPCMHService.EditFLPCBomInfo(bomItem, CurrUser);
            return result;
        }

        [HttpPost]
        public void DeleteBomInfoByUIDListAPI(dynamic json)
        {
            var jsonData = json.ToString();
            var list = JsonConvert.DeserializeObject<List<int>>(jsonData);
            flowChartPCMHService.DeleteBomInfoByUIDList(list);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetALlPCMHAPI()
        {
            var list = flowChartPCMHService.GetALlPCMH();
            return Ok(list);
        }
        #endregion

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetMaxDetailInfoAPI(int flowChartMaster_uid)
        {
            var result = flowChartDetailService.GetMaxDetailInfoAPI(flowChartMaster_uid);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetMasterItemBySearchConditionAPI(PPCheckDataSearch searchModel)
        {
            var result = flowChartService.GetMasterItemBySearchCondition(searchModel);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public List<FlowChartDetailDTO> QueryDetailList(int id, int Version)
        {
            var result = flowChartDetailService.QueryDetailList(id, Version);
            return result;
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryFLMgDataList(dynamic json)
        {
            string idListString = json.ToString();
            var idList = JsonConvert.DeserializeObject<List<int>>(idListString);
            var result = flowChartPlanService.QueryFLMgDataList(idList);
            return Ok(result);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryFLBomDataList(dynamic json)
        {
            string idListString = json.ToString();
            var idList = JsonConvert.DeserializeObject<List<int>>(idListString);
            var result = flowChartPCMHService.QueryFLBomDataList(idList);
            return Ok(result);
        }

    }
}