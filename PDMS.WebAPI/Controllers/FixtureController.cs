using Newtonsoft.Json;
using PDMS.Core;
using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PDMS.Common.Constants;
using System.Linq;
using PDMS.Core.Authentication;
using PDMS.Model.EntityDTO;

namespace PDMS.WebAPI.Controllers
{
    public class FixtureController : ApiController
    {

        IFixtureService fixtureService;

        public FixtureController(IFixtureService fixtureService)
        {
            this.fixtureService = fixtureService;
        }
        #region 治具资料维护 ----Add by keyong 2017-09-25
        /// <summary>
        /// 获取当前OP
        /// </summary>
        /// <param name="parentOrg_UID"></param>
        /// <param name="organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCurrentOPTypeAPI(int parentOrg_UID, int organization_UID)
        {

            var dto = fixtureService.GetCurrentOPType(parentOrg_UID, organization_UID);
            return Ok(dto);
        }
        /// <summary>
        /// 获取当前功能厂
        /// </summary>
        /// <param name="Optype"></param>
        /// <param name="Optypes"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetFunPlantByOPTypeAPI(int Optype, string Optypes = "")
        {
            var dto = fixtureService.GetFunPlantByOPType(Optype, Optypes);
            return Ok(dto);
        }
        /// <summary>
        /// 获取治具状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetFixtureStatuDTOAPI()
        {
            var dto = fixtureService.GetFixtureStatuDTO();
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetFixtureStatuDTOListWhenAddAPI()
        {
            //維修單新增時，維修狀態只有(3.維修中In-Repair;4.報廢Scrap;5:返供應商維修RTV)三種可以選取。
            var statusList = new List<string>();
            statusList.Add("维修中");
            statusList.Add("报废");
            statusList.Add("返供应商维修");
            var dtoList = fixtureService.GetFixtureStatuDTO();
            var resultList = new List<FixtureStatuDTO>();
            for (int i = 0; i < dtoList.Count; i++)
            {
                for (int j = 0; j < statusList.Count; j++)
                {
                    if (dtoList[i].StatuName.Contains(statusList[j]))
                    {
                        resultList.Add(dtoList[i]);
                    }
                }
            }
            return Ok(resultList);
        }
        
        /// <summary>
        /// 查询治具资料列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult QueryFixtureAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixtureDTO>(jsondata);
            var result = fixtureService.QueryFixture(searchModel, page);
            return Ok(result);

        }

        [HttpPost]
        public IHttpActionResult QueryFixtureStatusMoniterAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixtureDTO>(jsondata);
            var result = fixtureService.QueryFixtureStatusMoniter(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFixtureStatusMoniterListBySearchAPI(dynamic data)
        {
            var entity = data.ToString();
            var search = JsonConvert.DeserializeObject<FixtureDTO>(entity);
            var list = fixtureService.GetFixtureStatusMoniterListBySearch(search);
            return Ok(list);
        }

        [HttpPost]
        public IHttpActionResult GetFixtureStatusMoniterListBySelectedAPI(dynamic data)
        {
            var entity = data.ToString();
            var search = JsonConvert.DeserializeObject<FixtureDTO>(entity);
            var list = fixtureService.GetFixtureStatusMoniterListBySelected(search);
            return Ok(list);
        }

        [HttpGet]
        public IHttpActionResult FixtureListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var result = fixtureService.FixtureList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        /// <summary>
        /// 根据ID查询治具资料
        /// </summary>
        /// <param name="fixture_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult QueryFixtureByUidAPI(int fixture_UID)
        {
            var result = fixtureService.QueryFixtureByUid(fixture_UID);
            return Ok(result);
        }

       
        /// <summary>
        /// 根据厂区，OP 获取机台数据。
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetVendor_InfoListAPI(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var result = fixtureService.GetVendor_InfoList(Plant_Organization_UID, BG_Organization_UID);
            return Ok(result);
        }
        /// <summary>
        /// 根据 厂区，OP，功能厂获取产线数据
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetProductionLineDTOListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetProductionLineDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        public IHttpActionResult GetFixtureSystemUserAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetFixtureSystemUser(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
            /// <summary>
        /// 根据 厂区，OP，功能厂获取生产地点
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetWorkshopListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetWorkshopList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetWorkshopListByQueryAPI(WorkshopModelSearch search)
        {
            var result = fixtureService.GetWorkshopListByQuery(search);
            return Ok(result);
        }

        /// <summary>
        /// 根据 厂区，OP，功能厂获取工站
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetWorkstationListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetWorkstationList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetWorkstationListByQueryAPI(WorkStationModelSearch search)
        {
            var result = fixtureService.GetWorkstationListByQuery(search);
            return Ok(result);
        }
        /// <summary>
        /// 根据 厂区，OP，功能厂获取制程
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetProcess_InfoListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetProcess_InfoList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        /// <summary>
        /// 根据 厂区，OP，功能厂获取专案
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetProjectListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetProjectList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        /// <summary>
        /// 获取治具机台信息
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <param name="Production_Line_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetFixtureMachineDTOListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, int Production_Line_UID)
        {
            var result = fixtureService.GetFixtureMachineDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Production_Line_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetFixtureMachineByUidAPI(int Fixture_Machine_UID)
        {
            var result = fixtureService.GetFixtureMachineByUid(Fixture_Machine_UID);
            return Ok(result);
        }
        public string AddOrEditFixtureMAPI(FixtureDTO dto, bool isEdit)
        {
            var result = fixtureService.AddOrEditFixtureM(dto, isEdit);
            return result;
        }
        /// <summary>
        /// 根据产线ID获取制程，专案，工站，生产地点
        /// </summary>
        /// <param name="Production_Line_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetProductionLineDTOAPI(int Production_Line_UID)
        {
            var result = fixtureService.GetProductionLineDTO(Production_Line_UID);
            return Ok(result);
        }
        [HttpGet]
        public string DeleteFixtureMAPI(int Fixture_M_UID)
        {
            return fixtureService.DeleteFixtureM(Fixture_M_UID);
        }


        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFixtureStatusAPI()
        {
            var result = fixtureService.GetFixtureStatus();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtureReprotAPI(string Fixture_M_UIDs)
        {
            var dto = Ok(fixtureService.DoExportFixtureReprot(Fixture_M_UIDs));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportFixtureReprotAPI(FixtureDTO search)
        {
            var result = fixtureService.FixtureList(search);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult BatchEnableFixturematerial(FixtureDTO search,string IsEnable)
        {
            var result = fixtureService.BatchEnableFixturematerial(search, IsEnable);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult BatchEnablePartFixturematerialAPI(string Fixture_M_UIDs, string IsEnable,int AccountID)
        {
            var result = fixtureService.BatchEnablePartFixturematerial(Fixture_M_UIDs, IsEnable, AccountID);
            return Ok(result);
        }
        

        #endregion
        #region 治具保养 ----Add by keyong 2017-09-30

        /// <summary>
        /// 查询治具资料列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult QueryFixtureMaintenanceAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<Fixture_Maintenance_RecordDTO>(jsondata);
            var result = fixtureService.QueryFixtureMaintenance(searchModel, page);
            return Ok(result);

        }
        /// <summary>
        /// 根据ID查询保养资料
        /// </summary>
        /// <param name="fixture_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult QueryFixtureMaintenanceByUidAPI(int Fixture_Maintenance_Record_UID)
        {
            var result = fixtureService.QueryFixtureMaintenanceByUid(Fixture_Maintenance_Record_UID);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult DoAllExportFixtureMaintenanceReprotAPI(Fixture_Maintenance_RecordDTO search)
        {
            var result = fixtureService.DoAllExportFixtureMaintenanceReprot(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtureMaintenanceReprotAPI(string Fixture_Maintenance_Record_UIDs)
        {
            var dto = Ok(fixtureService.DoExportFixtureMaintenanceReprot(Fixture_Maintenance_Record_UIDs));
            return dto;
        }
        /// <summary>
        /// 根据ID列表获取保养信息
        /// </summary>
        /// <param name="Fixture_Maintenance_Record_UIDs"></param>
        /// <param name="straus"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetFixtureMaintenanceAPI(string Fixture_Maintenance_Record_UIDs, int straus)
        {
            var dto = Ok(fixtureService.GetFixtureMaintenance(Fixture_Maintenance_Record_UIDs, straus));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult GetFixtureMaintenanceListAPI(Fixture_Maintenance_RecordDTO fixture_Maintenance_RecordDTO, int straus)
        {
            var dto = Ok(fixtureService.GetFixtureMaintenanceList(fixture_Maintenance_RecordDTO, straus));
            return dto;
         
        }
        /// <summary>
        /// 更新保养，确认，取消。
        /// </summary>
        /// <param name="fixture_Maintenance_Record_UIDs"></param>
        /// <param name="NTID"></param>
        /// <param name="date"></param>
        /// <param name="straus"></param>
        /// <returns></returns>   
        [HttpPost]
        public string UpdateFixture_Maintenance_RecordAPI(Fixture_Maintenance_RecordDTOCS fixture_Maintenance_RecordDTOCS)
        {
                  
            var result = fixtureService.UpdateFixture_Maintenance_Record(fixture_Maintenance_RecordDTOCS.fixture_Maintenance_Record_UIDs, fixture_Maintenance_RecordDTOCS.NTID, fixture_Maintenance_RecordDTOCS.personNumber, fixture_Maintenance_RecordDTOCS.personName, fixture_Maintenance_RecordDTOCS.date, fixture_Maintenance_RecordDTOCS.straus, fixture_Maintenance_RecordDTOCS.CurrentUserID);
            return result;
        }
        /// <summary>
        /// 根据厂区，op，功能厂 获取治具计划
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult  GetFixtureMaintenance_PlanAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type=null)
        {
            var result = fixtureService.GetFixtureMaintenance_Plan(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Maintenance_Type);
            return Ok(result);
        }
        public string CreateFixture_Maintenance_RecordsAPI(Fixture_Maintenance_RecordDTO dto)
        {
            var result = fixtureService.CreateFixture_Maintenance_Records(dto);
            return result;
        }
        

        #endregion
        #region                  karl start
        #region    供应商维护--------------------START
        [HttpPost]
        public IHttpActionResult QueryVendorInfoAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<VendorInfoDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryVendorInfo(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetVendorInfoListAPI(VendorInfoDTO search)
        {
            var result = fixtureService.GetVendorInfoList(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOrgByParantAPI(int Parant_UID, int type)
        {
            var dto = fixtureService.GetOrgByParant(Parant_UID, type);
            return Ok(dto);
        }

        public string AddOrEditVendorInfoAPI(VendorInfoDTO dto)
        {
            return fixtureService.AddOrEditVendorInfo(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryVendorInfoByUidAPI(int Vendor_Info_UID)
        {
            var dto = fixtureService.QueryVendorInSingle(Vendor_Info_UID);
            var result = new VendorInfoDTO
            {
                Vendor_Info_UID = dto.Vendor_Info_UID,
                Plant_Organization_UID = dto.Plant_Organization_UID,
                BG_Organization_UID = dto.BG_Organization_UID,
                Vendor_ID = dto.Vendor_ID,
                Vendor_Name = dto.Vendor_Name,
                Is_Enable = dto.Is_Enable
            };
            return Ok(result);
        }

        [HttpGet]
        public string DeleteVendorInfoAPI(int Vendor_Info_UID)
        {
            return fixtureService.DeleteVendorInfo(Vendor_Info_UID);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllPlantAPI(int PLANT_UID, string leval)
        {
            var result = fixtureService.QueryAllPlants(PLANT_UID, leval);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllVendorInfoAPI()
        {
            var result = fixtureService.QueryAllVendorInfos();
            return Ok(result);
        }

        [HttpPost]
        public string InsertVendorInfoAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<VendorInfoDTO>>(jsondata);
            return fixtureService.InsertVendorInfoItem(list);
        }
        [HttpGet]
        public IHttpActionResult DoVIExportFunctionAPI(string uids)
        {
            var dto = Ok(fixtureService.DoVIExportFunction(uids));
            return dto;
        }

        [HttpGet]
        public IHttpActionResult GetAllOrgBomAPI()
        {
            var dto = fixtureService.GetAllOrgBom();
            return Ok(dto);
        }
        #endregion        供应商维护--------------------------END

        #region           保养计划设定维护------------------------START
        [HttpPost]
        public IHttpActionResult QueryMaintenancePlanAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<MaintenancePlanDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryMaintenancePlan(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryMaintenancePlanByUidAPI(int Maintenance_Plan_UID)
        {
            var dto = fixtureService.QueryMaintenanceSingle(Maintenance_Plan_UID);
            var result = new MaintenancePlanDTO
            {
                Maintenance_Plan_UID = dto.Maintenance_Plan_UID,
                Plant_Organization_UID = dto.Plant_Organization_UID,
                BG_Organization_UID = dto.BG_Organization_UID,
                FunPlant_Organization_UID = dto.FunPlant_Organization_UID,
                Maintenance_Type = dto.Maintenance_Type,
                Cycle_ID = dto.Cycle_ID,
                Cycle_Interval = dto.Cycle_Interval,
                Cycle_Unit = dto.Cycle_Unit,
                Lead_Time = dto.Lead_Time,
                Start_Date = dto.Start_Date,
                Tolerance_Time = dto.Tolerance_Time,
                Last_Execution_Date = dto.Last_Execution_Date,
                Next_Execution_Date = dto.Next_Execution_Date,
                Is_Enable = dto.Is_Enable
            };
            return Ok(result);
        }

        public string AddOrEditMaintenancePlanAPI(MaintenancePlanDTO dto)
        {
            return fixtureService.AddOrEditMaintenancePlan(dto);
        }

        [HttpGet]
        public string DeleteMaintenancePlanAPI(int Maintenance_Plan_UID)
        {
            return fixtureService.DeleteMaintenancePlan(Maintenance_Plan_UID);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllMaintenancePlanAPI()
        {
            var result = fixtureService.QueryAllMaintenancePlans();
            return Ok(result);
        }
        public string InsertMaintenancePlanAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<MaintenancePlanDTO>>(jsondata);
            return fixtureService.InsertMaintenancePlanItem(list);
        }
        [HttpGet]
        public IHttpActionResult DoMPExportFunctionAPI(string uids)
        {
            var dto = Ok(fixtureService.DoMPExportFunction(uids));
            return dto;
        }

        [HttpPost]
        public IHttpActionResult DoAllMPExportFunctionAPI(MaintenancePlanDTO search)
        {
            var result = fixtureService.DoAllMPExportFunction(search);
            return Ok(result);
        }



        #endregion           保养计划设定维护----------------------END

        #region           治具保养设定维护------------------------START
        [HttpPost]
        public IHttpActionResult QueryFixtureMaintenanceProfileAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FixtureMaintenanceProfileDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryFixtureMaintenanceProfile(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryFixtureMaintenanceProfileByUidAPI(int Fixture_Maintenance_Profile_UID)
        {
            var dto = fixtureService.QueryFixtureMaintenanceProfileSingle(Fixture_Maintenance_Profile_UID);
            var result = new FixtureMaintenanceProfileDTO
            {
                Plant_Organization_UID = dto.Plant_Organization_UID,
                BG_Organization_UID = dto.BG_Organization_UID,
                FunPlant_Organization_UID = dto.FunPlant_Organization_UID,
                Fixture_NO = dto.Fixture_NO,
                Maintenance_Type = dto.Maintenance_Type,
                Cycle_ID = dto.Cycle_ID,
                Maintenance_Plan_UID = dto.Maintenance_Plan_UID,
                Is_Enable = dto.Is_Enable
            };
            return Ok(result);
        }

        public string AddOrEditFixtureMaintenanceProfileAPI(FixtureMaintenanceProfileDTO dto)
        {
            return fixtureService.AddOrEditFixtureMaintenanceProfile(dto);
        }

        [HttpGet]
        public string DeleteFixtureMaintenanceProfileAPI(int Fixture_Maintenance_Profile_UID)
        {
            return fixtureService.DeleteFixtureMaintenanceProfile(Fixture_Maintenance_Profile_UID);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixtureMaintenanceProfileAPI()
        {
            var result = fixtureService.QueryAllFixtureMaintenanceProfiles();
            return Ok(result);
        }
        public string InsertFixtureMaintenanceProfileAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixtureMaintenanceProfileDTO>>(jsondata);
            return fixtureService.InsertFixtureMaintenanceProfileItem(list);
        }
        [HttpGet]
        public IHttpActionResult DoFMPExportFunctionAPI(string uids)
        {
            var dto = Ok(fixtureService.DoFMPExportFunction(uids));
            return dto;
        }

        [HttpPost]
        public IHttpActionResult DoAllFMPExportFunctionAPI(FixtureMaintenanceProfileDTO search)
        {
            var result = fixtureService.DoAllFMPExportFunction(search);
            return Ok(result);
        }


        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFixtureNoByFunPlantAPI(int BG_Organization_UID,int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetFixtureNoByFunPlant(BG_Organization_UID,FunPlant_Organization_UID);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetMaintenancePlanByFiltersAPI(int BG_Organization_UID, int FunPlant_Organization_UID,string Maintenance_Type)
        {
            var result = fixtureService.GetMaintenancePlanByFilters(BG_Organization_UID, FunPlant_Organization_UID, Maintenance_Type);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixtureAPI()
        {
            var result = fixtureService.QueryAllFixture();
            return Ok(result);
        }

        #endregion           治具保养设定维护----------------------END

        #region          用户车间设定------------------------START
        [HttpPost]
        public IHttpActionResult QueryFixtureUserWorkshopAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FixtureUserWorkshopDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryFixtureUserWorkshop(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryFixtureUserWorkshopByUidAPI(int Fixture_User_Workshop_UID)
        {
            var dto = fixtureService.QueryFixtureUserWorkshopSingle(Fixture_User_Workshop_UID);
            var result = new FixtureUserWorkshopDTO
            {
                Plant_Organization_UID=dto.Plant_Organization_UID,
                BG_Organization_UID=dto.BG_Organization_UID,
                FunPlant_Organization_UID=dto.FunPlant_Organization_UID,
                Fixture_User_Workshop_UID = dto.Fixture_User_Workshop_UID,
                Account_UID=dto.Account_UID,
                User_NTID=dto.User_NTID,
                User_Name=dto.User_Name,
                Workshop_ID=dto.Workshop_ID,
                Workshop_UID=dto.Workshop_UID,
                Is_Enable=dto.Is_Enable
            };
            return Ok(result);
        }

        public string AddOrEditFixtureUserWorkshopAPI(FixtureUserWorkshopDTO dto)
        {
            return fixtureService.AddOrEditFixtureUserWorkshop(dto);
        }

        [HttpGet]
        public string DeleteFixtureUserWorkshopAPI(int Fixture_User_Workshop_UID)
        {
            return fixtureService.DeleteFixtureUserWorkshop(Fixture_User_Workshop_UID);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixtureUserWorkshopAPI()
        {
            var result = fixtureService.QueryAllFixtureUserWorkshops();
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserByOpAPI(int BG_Organization_UID,int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetUserByOp(BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserByOpAPILY(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetUserByOpAPILY(BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllWorkshopsAPI()
        {
            var result = fixtureService.QueryAllWorkshops();
            return Ok(result);
        }

        public string InsertFixtureUserWorkshopAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixtureUserWorkshopDTO>>(jsondata);
            return fixtureService.InsertFixtureUserWorkshopItem(list);
        }

        [HttpGet]
        public IHttpActionResult GetWorkshopByNTIDAPI(int Account_UID)
        {
            var dto = fixtureService.GetWorkshopByNTID(Account_UID);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult DoFUWExportFunctionAPI(string uids)
        {
            var dto = Ok(fixtureService.DoFUWExportFunction(uids));
            return dto;
        }

        [HttpPost]
        public IHttpActionResult DoAllFUWExportFunctionAPI(FixtureUserWorkshopDTO search)
        {
            var result = fixtureService.DoAllFUWExportFunction(search);
            return Ok(result);
        }


        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllUsersAPI()
        {
            var result = fixtureService.QueryAllUsers();
            return Ok(result);
        }
        #endregion           用户车间设定----------------------END
        #region                维修地点设定-------------------------START 
        public IHttpActionResult QueryRepairLocationAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<RepairLocationDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryRepairLocation(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryRepairLocationByUidAPI(int Repair_Location_UID)
        {
            var dto = fixtureService.QueryRepairLocationSingle(Repair_Location_UID);
            var result = new RepairLocationDTO
            {
                Plant_Organization_UID = dto.Plant_Organization_UID,
                BG_Organization_UID = dto.BG_Organization_UID,
                FunPlant_Organization_UID = dto.FunPlant_Organization_UID,
                Repair_Location_ID = dto.Repair_Location_ID,
                Repair_Location_Name = dto.Repair_Location_Name,
                Repair_Location_Desc=dto.Repair_Location_Desc,
                Is_Enable=dto.Is_Enable
            };
            return Ok(result);
        }
        [HttpGet]
        public string DeleteRepairLocationAPI(int Repair_Location_UID)
        {
            return fixtureService.DeleteRepairLocation(Repair_Location_UID);
        }
        public string AddOrEditRepairLocationAPI(RepairLocationDTO dto)
        {
            return fixtureService.AddOrEditRepairLocation(dto);
        }

        public string InsertRepairLocationAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<RepairLocationDTO>>(jsondata);
            return fixtureService.InsertRepairLocation(list);
        }

        [HttpGet]
        public IHttpActionResult DoRLExportFunctionAPI(string uids)
        {
            var dto = Ok(fixtureService.DoRLExportFunction(uids));
            return dto;
        }

        [HttpPost]
        public IHttpActionResult DoAllRLExportFunctionAPI(RepairLocationDTO search)
        {
            var result = fixtureService.DoAllRLExportFunction(search);
            return Ok(result);
        }


        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllRepairLocationAPI()
        {
            var result = fixtureService.QueryAllRepairLocations();
            return Ok(result);
        }
        #endregion          维修地点设定---------------------------END
        #region                治具领用------------------START
        public IHttpActionResult QueryFixtureTotakeAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_Totake_MDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryFixtureTotake(searchModel, page);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult QueryFixtureTotakeByUidAPI(int Fixture_Totake_M_UID)
        {
            var dto = fixtureService.QueryFixtureTotakeSingle(Fixture_Totake_M_UID);
            var fixturelist = fixtureService.GetfixtureList(Fixture_Totake_M_UID);
            var result = new FixtureTotakeVM
            {
                Plant = dto.Plant,
                BG = dto.BG,
                FunPlant = dto.FunPlant,
                Totake_NO=dto.Totake_NO,
                Totaker = dto.Totaker,
                Shiper=dto.Shiper,
                Totake_Date=dto.Totake_Date,
                Ship_Date=dto.Ship_Date,
                fixtures = fixturelist
            };
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFixtureByWorkshopAPI(int Account_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name)
        {
            var result = fixtureService.GetFixtureByWorkshop(Account_UID, Line_ID, Line_Name, Machine_ID, Machine_Name, Process_ID, Process_Name);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserByWorkshopAPI(int Account_UID)
        {
            var result = fixtureService.GetUserByWorkshop(Account_UID);
            return Ok(result);
        }

        public string AddOrEditFixtureTotakeAPI(Fixture_Totake_MDTO dto)
        {
            var result = fixtureService.AddOrEditFixtureTotake(dto);
            return result;
        }

        public string UpdateFixtureAPI(dynamic labordata,int useruid,string Totake_NO,DateTime Totake_Date)
        {
            var fixtureentity = JsonConvert.DeserializeObject<List<FixtureDTO>>(labordata.ToString());
            return fixtureService.UpdateFixture(fixtureentity,useruid, Totake_NO, Totake_Date);
        }

        [HttpGet]
        public IHttpActionResult DoFTExportFunctionAPI(string uids)
        {
            var dto = Ok(fixtureService.DoFTExportFunction(uids));
            return dto;
        }

        [HttpPost]
        public IHttpActionResult DoAllFTExportFunctionAPI(Fixture_Totake_MDTO search)
        {
            var result = fixtureService.DoAllFTExportFunction(search);
            return Ok(result);
        }



        public IHttpActionResult QueryFixtureTotakeDetailAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_Totake_DDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryFixtureTotakeDetail(searchModel, page);
            return Ok(result);
        }
        #endregion           治具领用-------------------END  

        #endregion            karl  end

        #region 治具归还
        /// <summary>
        /// 根据条件拿所有的领用单
        /// </summary>
        /// <param name="plant_ID">厂区</param>
        /// <param name="op_type">OP</param>
        /// <param name="funPlant">功能</param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult FetchFixtureTotakeforFixtureReturnAPI(int plant_ID, int op_type, int funPlant)
        {
            //先拿所有需要归还治具的领用单信息
            var result = fixtureService.FetchFixtureTotakeforFixtureReturn(plant_ID, op_type, funPlant);
            //return Ok(result);
            return Ok(result);
        }
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult FetchAllFixturesBasedTakeNoAPI(string Take_NO)
        {
            //先拿所有需要归还治具的领用单信息的唯一标识
            var allFixtureTake_UID = fixtureService.FetchAllFixturesBasedTakeNo(Take_NO);
            //根据allFixtureTake_UID获取所有的治具信息+Fixture_take_UID


            //var ret = fixtureService.FetchFixtureTakenInfo(allFixtureTake_UID);//返回治具List
              
            //return Ok(result);
            return Ok(allFixtureTake_UID);
        }
        [HttpPost]
        public string AddFixtureRetrunAPI(Fixture_Return_MDTO dto)
        {
            var result = fixtureService.AddFixtureRetrun(dto);

            return result;
        }
        [HttpPost]
        public string UpdateFixtureRetrunAPI(Fixture_Return_MDTO dto)
        {
            var result = fixtureService.UpdateFixtureRetrun(dto);

            return result;
        }

        [HttpPost]
        public string AddFixtureRetrunDAPI(Fixture_Return_DDTO dto)
        {
            var result = fixtureService.AddFixtureRetrunD(dto);

            return result;
        }
        [HttpPost]
        public string UpdateFixtureRetrunDAPI(Fixture_Return_DDTO dto)
        {
            var result = fixtureService.UpdateFixtureRetrunD(dto);

            return result;
        }
        [HttpPost]
        public IHttpActionResult QueryFixtureToReturnAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_Return_MDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = fixtureService.QueryFixtureToReturn(searchModel, page);
            return Ok(result);
        }

        public IHttpActionResult QueryFixtureReturnUidAPI(int uid)
        {
            var result = fixtureService.QueryFixtureReturnUid(uid);
            return Ok(result);
        }
        public string FixtureReturnUpdatePostAPI(Fixture_Return_MDTO dto)
        {
            var result = fixtureService.FixtureReturnUpdatePost(dto);
            return result;
        }
        [HttpGet]
        public IHttpActionResult FixtureReturnDetailAPI(int uid)
        {
            var result = fixtureService.FixtureReturnDetail(uid);
            return Ok(result);
        }
        [HttpGet]
        public string DelFixtureReturnMAPI(int uid)
        {
            var result = fixtureService.DelFixtureReturnM(uid);
            return result;
        }
        public string GetCurrentReturnNubAPI()
        {
            var result = fixtureService.GetCurrentReturnNub();
            return result;
        }
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult FetchAllFixturesBasedReturnMUIDAPI(int uid)
        {
            var result = fixtureService.FetchAllFixturesBasedReturnMUID(uid);
            return Ok(result);
        }
        [HttpPost]
        public IHttpActionResult ExportFixtrueReturn2ExcelAPI(Fixture_Return_MDTO dto)
        {
            var result = fixtureService.ExportFixtrueReturn2Excel(dto);
            return Ok(result);
        }


        #endregion





        #region 治具异常原因及相应维修策略设定--------Add by justin 2017-09-29
        [HttpPost]
        public IHttpActionResult IsDefectRepairExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<DefectRepairSearch>(jsonData);
            bool isExist = fixtureService.IsDefectRepairExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public IHttpActionResult QueryDefectRepairsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<DefectRepairSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workshops = fixtureService.QueryDefectRepairs(searchModel, page);
            return Ok(workshops);
        }

        [HttpPost]
        public IHttpActionResult GetDefectRepairListAPI(DefectRepairSearch searchModel)
        {
            var defectRepairs = fixtureService.GetDefectRepairList(searchModel);
            return Ok(defectRepairs);
        }

        [HttpGet]
        public IHttpActionResult GetDefectListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetDefectList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetRepairSoulutionListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetRepairSoulutions(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
     

        [HttpGet]
        public IHttpActionResult QueryDefectRepairAPI(int uid)
        {
            var dto = new DefectRepairSolutionDTO();
            dto = AutoMapper.Mapper.Map<DefectRepairSolutionDTO>(fixtureService.QueryDefectRepairSingle(uid));
            return Ok(dto);
        }

        public string EditDefectRepairAPI(DefectRepairSolutionDTO dto)
        {
            var DefectRepair = fixtureService.QueryDefectRepairSingle(dto.Defect_RepairSolution_UID);
            DefectRepair.Plant_Organization_UID = dto.Plant_Organization_UID;
            DefectRepair.BG_Organization_UID = dto.BG_Organization_UID;
            DefectRepair.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            DefectRepair.Fixtrue_Defect_UID = dto.Fixtrue_Defect_UID;
            DefectRepair.Repair_Solution_UID = dto.Repair_Solution_UID;
            DefectRepair.Is_Enable = dto.Is_Enable;
            DefectRepair.Modified_UID = dto.Modified_UID;
            DefectRepair.Modified_Date = DateTime.Now;

            return fixtureService.EditDefectRepair(DefectRepair);

        }

        public string AddDefectRepairAPI(DefectRepairSolutionDTO dto)
        {
            var DefectRepair = AutoMapper.Mapper.Map<DefectCode_RepairSolution>(dto);
            DefectRepair.Created_UID = dto.Modified_UID;
            DefectRepair.Created_Date = DateTime.Now;
            DefectRepair.Modified_UID = dto.Modified_UID;
            DefectRepair.Modified_Date = DateTime.Now;
            var workshopstring = fixtureService.AddDefectRepair(DefectRepair);
            if (workshopstring != "SUCCESS")
                return workshopstring;
            else
                return "SUCCESS";
        }


        [AcceptVerbs("Post")]
        public string DeleteDefectRepairAPI(DefectRepairSolutionDTO dto)
        {

            var result = fixtureService.DeleteDefectRepair(dto.Defect_RepairSolution_UID);
            return result;
        }


        [HttpGet]
        public IHttpActionResult DoExportDefectRepairAPI(string uids)
        {
            var list = fixtureService.DoExportDefectRepair(uids);
            return Ok(list);
        }

        [HttpPost]
        public string InsertDefectRepairSolutionAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<DefectCode_RepairSolution>>(jsondata);
            return fixtureService.InsertDefectRepairSolutions(list);
        }




        #endregion 治具异常原因及相应维修策略设定--------Add by justin 2017-09-29

        #region  治具维修------------------------add  by  Justin  2017-10-05 ----------
        [HttpPost]
        public IHttpActionResult QueryFixtureRepairAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixtureRepairSearch>(jsondata);
            var result = fixtureService.QueryFixtureRepairs(searchModel, page);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFixtureRepairListAPI(FixtureRepairSearch search)
        {
            var workStations = fixtureService.GetFixtureRepairList(search);
            return Ok(workStations);
        }

        public string AddFixtureRepairAPI(Fixture_Repair_MDTO dto)
        {
            var result = fixtureService.AddFixtureRepair(dto);
            return result;
        }
        public string EditFixtureRepairAPI(Fixture_Repair_MDTO dto)
        {
            var result = fixtureService.EditFixtureRepair(dto);
            return result;
        }
        [HttpGet]
        public IHttpActionResult GetFixture_Repair_MDTOListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetFixture_Repair_MDTOList(Plant_Organization_UID,  BG_Organization_UID,  FunPlant_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetFixture_Repair_MDTOByIDAPI(int Fixture_Repair_M_UID)
        {
          var result = fixtureService.GetFixture_Repair_MDTOByID(Fixture_Repair_M_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetSentRepairNameByIdAPI(string SentOut_Number)
        {
            var result = fixtureService.GetSentRepairNameById(SentOut_Number);
            return Ok(result);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFixtureRepairByRepairNoAPI(string repairNo)
        {
            var result = fixtureService.GetFixtureRepairByRepairNo(repairNo);
            return Ok(result);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFixtureListFixtureRepairAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name)
        {
            var result = fixtureService.GetFixtureListFixtureRepair(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Line_ID, Line_Name, Machine_ID, Machine_Name, Process_ID, Process_Name);
            return Ok(result);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFixtureListFixtureRepairByUniqueIDAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string UniqueID)
        {
            var result = fixtureService.GetFixtureListFixtureRepairByUniqueID(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, UniqueID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetDefectCodeReapairSolutionListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetDefectCodeReapairSolutionDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFixtureByFixtureUniqueIDAPI(string fixtureUniqueID)
        {
            var fixtureDto = fixtureService.GetFixtureByFixtureUniqueID(fixtureUniqueID);

            if (fixtureDto == null)
            {
                return NotFound();
            }

            return Ok(fixtureDto);
        }

        [HttpGet]
        public IHttpActionResult GetRepairLocationListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetRepairLocationList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtureRepairAPI(string uids)
        {
            var list = fixtureService.DoExportFixtureRepair(uids);
            return Ok(list);
        }
        #endregion

        #region Jay:20170929
        #region 工站维护
        [HttpPost]
        public IHttpActionResult IsWorStationExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<WorkStationModelSearch>(jsonData);
            bool isExist = fixtureService.IsWorkStationExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public IHttpActionResult QueryWorkStationsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<WorkStationModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workStations = fixtureService.QueryWorkStations(searchModel, page);
            return Ok(workStations);
        }
        [HttpGet]
        public IHttpActionResult QueryWorkStationAPI(int uid)
        {
            var dto = new WorkStationDTO();
            dto = AutoMapper.Mapper.Map<WorkStationDTO>(fixtureService.QueryWorkStationSingle(uid));
            return Ok(dto);
        }

        public string EditWorkStationAPI(WorkStationDTO dto)
        {
            var workStation = fixtureService.QueryWorkStationSingle(dto.WorkStation_UID);
            workStation.Plant_Organization_UID = dto.Plant_Organization_UID;
            workStation.BG_Organization_UID = dto.BG_Organization_UID;
            workStation.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            workStation.Project_UID = dto.Project_UID;
            workStation.Process_Info_UID = dto.Process_Info_UID;
            workStation.WorkStation_ID = dto.WorkStation_ID;
            workStation.WorkStation_Name = dto.WorkStation_Name;
            workStation.WorkStation_Desc = dto.WorkStation_Desc;
            workStation.Is_Enable = dto.Is_Enable;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var WorkStationstring = fixtureService.EditWorkStation(workStation);
            return WorkStationstring;
        }

        public string AddWorkStationAPI(WorkStationDTO dto)
        {
            var workStation = AutoMapper.Mapper.Map<WorkStation>(dto);
            workStation.Created_UID = dto.Modified_UID;
            workStation.Created_Date = DateTime.Now;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var workStationstring = fixtureService.AddWorkStation(workStation);
            if (workStationstring != "SUCCESS")
                return workStationstring;
            else
                return "SUCCESS";
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllWorkStationsAPI()
        {
            var result = fixtureService.QueryAllWorkStations();
            return Ok(result);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteWorkStationAPI(WorkStationDTO dto)
        {

            var result = fixtureService.DeleteWorkStation(dto.WorkStation_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportWorkStationAPI(string uids)
        {
            var list = fixtureService.DoExportWorkStation(uids);
            return Ok(list);
        }

        [HttpPost]
        public string InsertWorkStationsAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<WorkStation>>(jsondata);
            return fixtureService.InsertWorkStaions(list);
        }
        #endregion

        #region 生产线维护
        [HttpPost]
        public IHttpActionResult IsProductionLineExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Production_LineModelSearch>(jsonData);
            bool isExist = fixtureService.IsProduction_LineExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public IHttpActionResult QueryProductionLinesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Production_LineModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workStations = fixtureService.QueryProduction_Lines(searchModel, page);
            return Ok(workStations);
        }
        [HttpPost]
        public IHttpActionResult GetProductionLineListAPI(Production_LineModelSearch search)
        {
            var productionLines = fixtureService.GetProductionLineList(search);
            return Ok(productionLines);
        }
        [HttpGet]
        public IHttpActionResult QueryProductionLineAPI(int uid)
        {
            var dto = new Production_LineDTO();
            var productionLine = fixtureService.QueryProduction_LineSingle(uid);
            if (productionLine != null)
            {
                dto = AutoMapper.Mapper.Map<Production_LineDTO>(productionLine);
                if (productionLine.System_Organization!=null)
                {
                    dto.Plant_Organization_Name = productionLine.System_Organization.Organization_Name;
                }
                if (productionLine.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = productionLine.System_Organization1.Organization_Name;
                }
                if (productionLine.System_Organization2!=null)
                {
                    dto.FunPlant_Organization_Name = productionLine.System_Organization2.Organization_Name;
                }
                if (productionLine.Workshop!=null)
                {
                    dto.Workshop_Name = productionLine.Workshop.Workshop_Name;
                    dto.Workshop_ID = productionLine.Workshop.Workshop_ID;
                }
                if (productionLine.WorkStation!=null)
                {
                    dto.Workstation_Name = productionLine.WorkStation.WorkStation_Name;
                    dto.Workstation_ID = productionLine.WorkStation.WorkStation_ID;
                }
                dto.Created_UserName = productionLine.System_Users.User_Name;
                dto.Modified_UserName = productionLine.System_Users1.User_Name;
                dto.Modified_Date = productionLine.Modified_Date;
                dto.Project_Code = productionLine.System_Project.Project_Name;
                dto.Project_Name = productionLine.System_Project.Project_Name;
                dto.Process_ID = productionLine.Process_Info.Process_ID;
                dto.Process_Name = productionLine.Process_Info.Process_Name;
            }
            
            return Ok(dto);
        }

        public string EditProductionLineAPI(Production_LineDTO dto)
        {
            var workStation = fixtureService.QueryProduction_LineSingle(dto.Production_Line_UID);
            workStation.Plant_Organization_UID = dto.Plant_Organization_UID;
            workStation.BG_Organization_UID = dto.BG_Organization_UID;
            workStation.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            workStation.Workshop_UID = dto.Workshop_UID;
            workStation.Workstation_UID = dto.Workstation_UID;
            workStation.Project_UID = dto.Project_UID;
            workStation.Process_Info_UID = dto.Process_Info_UID;
            workStation.Line_ID = dto.Line_ID;
            workStation.Line_Name = dto.Line_Name;
            workStation.Line_Desc = dto.Line_Desc;
            workStation.Is_Enable = dto.Is_Enable;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var Production_Linestring = fixtureService.EditProduction_Line(workStation);
            return Production_Linestring;
        }

        public string AddProductionLineAPI(Production_LineDTO dto)
        {
            var workStation = AutoMapper.Mapper.Map<Production_Line>(dto);
            workStation.Created_UID = dto.Modified_UID;
            workStation.Created_Date = DateTime.Now;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var workStationstring = fixtureService.AddProduction_Line(workStation);
            if (workStationstring != "SUCCESS")
                return workStationstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteProductionLineAPI(Production_LineDTO dto)
        {

            var result = fixtureService.DeleteProduction_Line(dto.Production_Line_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportProductionLineAPI(string uids)
        {
            var list = fixtureService.DoExportProduction_Line(uids);
            return Ok(list);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllProductionLinesAPI()
        {
            var result = fixtureService.QueryAllProduction_Lines();
            return Ok(result);
        }
        [HttpPost]
        public string InsertProductionLinesAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Production_Line>>(jsondata);
            return fixtureService.InsertProduction_Lines(list);
        }
        #endregion

        #region 设备机台维护
        [HttpPost]
        public IHttpActionResult IsFixtureMachineExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_MachineModelSearch>(jsonData);
            bool isExist = fixtureService.IsFixture_MachineExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public IHttpActionResult QueryFixtureMachinesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_MachineModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workStations = fixtureService.QueryFixture_Machines(searchModel, page);
            return Ok(workStations);
        }
        [HttpPost]
        public IHttpActionResult GetFixtureMachineListAPI(Fixture_MachineModelSearch search)
        {
            var fixtureMachines = fixtureService.GetFixtureMachineList(search);
            return Ok(fixtureMachines);
        }
        [HttpGet]
        public IHttpActionResult QueryFixtureMachineAPI(int uid)
        {
            var dto = new FixtureMachineDTO();
            var machine = fixtureService.QueryFixture_MachineSingle(uid);
            if (machine!=null)
            {
                dto = AutoMapper.Mapper.Map<FixtureMachineDTO>(machine);
                dto.Plant_Organization_Name = machine.System_Organization.Organization_Name;
                dto.BG_Organization_Name=machine.System_Organization1.Organization_Name;
                if (machine.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = machine.System_Organization2.Organization_Name;
                }
                if (machine.Production_Line !=null)
                {
                    dto.Production_Line_ID = machine.Production_Line.Line_ID;
                    dto.Production_Line_Name = machine.Production_Line.Line_Name;
                }
                dto.Created_UserName = machine.System_Users.User_Name;
                dto.Modified_UserName = machine.System_Users1.User_Name;
            }
            return Ok(dto);
        }

        public string EditFixtureMachineAPI(FixtureMachineDTO dto)
        {
            var workStation = fixtureService.QueryFixture_MachineSingle(dto.Fixture_Machine_UID);
            workStation.Plant_Organization_UID = dto.Plant_Organization_UID;
            workStation.BG_Organization_UID = dto.BG_Organization_UID;
            workStation.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            workStation.Production_Line_UID = dto.Production_Line_UID;
            workStation.Machine_ID = dto.Machine_ID;
            workStation.Machine_Name = dto.Machine_Name;
            workStation.Machine_Desc = dto.Machine_Desc;
            workStation.Is_Enable = dto.Is_Enable;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            workStation.EQP_Uid = dto.EQP_Uid;
            workStation.Equipment_No = dto.Equipment_No;
            var Fixture_Machinestring = fixtureService.EditFixture_Machine(workStation);
            return Fixture_Machinestring;
        }

        public string AddFixtureMachineAPI(FixtureMachineDTO dto)
        {
            var workStation = AutoMapper.Mapper.Map<Fixture_Machine>(dto);
            workStation.Created_UID = dto.Modified_UID;
            workStation.Created_Date = DateTime.Now;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var workStationstring = fixtureService.AddFixture_Machine(workStation);
            if (workStationstring != "SUCCESS")
                return workStationstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteFixtureMachineAPI(FixtureMachineDTO dto)
        {
            var result = fixtureService.DeleteFixture_Machine(dto.Fixture_Machine_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtureMachineAPI(string uids)
        {
            var list = fixtureService.DoExportFixture_Machine(uids);
            return Ok(list);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixtureMachinesAPI()
        {
            var result = fixtureService.QueryAllFixture_Machines();
            return Ok(result);
        }
        [HttpPost]
        public string InsertFixtureMachinesAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Fixture_Machine>>(jsondata);
            return fixtureService.InsertFixture_Machines(list);
        }

        #region 设备机台信息与治具机台关联功能 by Steven -2018/09/21---------
        //TODO 2018/09/18 steven add 取设备机台信息檔功能
        [HttpGet]
        public IHttpActionResult GetEquipmentInfoListApi(int Plant_Organization_UID, int? BG_Organization_UID, int? FunPlant_Organization_UID)
        {
            var result = fixtureService.GetEquipmentInfoList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        //TODO 2018/09/18 steven add 批量新增设备机台信息档
        [HttpGet]
        public IHttpActionResult BatchAppendApi(int UserID, int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            int result = fixtureService.BatchAppendExcute(UserID, Plant_Organization_UID ,BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        #endregion
        #endregion

        #region 治具异常原因维护
        [HttpPost]
        public IHttpActionResult IsFixtureDefectCodeExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_DefectCodeModelSearch>(jsonData);
            bool isExist = fixtureService.IsFixture_DefectCodeExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public IHttpActionResult QueryFixtureDefectCodesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_DefectCodeModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workStations = fixtureService.QueryFixture_DefectCodes(searchModel, page);
            return Ok(workStations);
        }

        [HttpPost]
        public IHttpActionResult GetFixtureDefectCodeListAPI(Fixture_DefectCodeModelSearch searchModel)
        {
            var workStations = fixtureService.GetFixtureDefectCodeList(searchModel);
            return Ok(workStations);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixtureDefectCodesAPI()
        {
            var result = fixtureService.QueryAllFixture_DefectCodes();
            return Ok(result);
        }


        [HttpGet]
        public IHttpActionResult QueryFixtureDefectCodeAPI(int uid)
        {
            var dto = new Fixture_DefectCodeDTO();
            dto = AutoMapper.Mapper.Map<Fixture_DefectCodeDTO>(fixtureService.QueryFixture_DefectCodeSingle(uid));
            return Ok(dto);
        }

        public string EditFixtureDefectCodeAPI(Fixture_DefectCodeDTO dto)
        {
            var workStation = fixtureService.QueryFixture_DefectCodeSingle(dto.Fixture_Defect_UID);
            workStation.Plant_Organization_UID = dto.Plant_Organization_UID;
            workStation.BG_Organization_UID = dto.BG_Organization_UID;
            workStation.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            workStation.DefectCode_ID = dto.DefectCode_ID;
            workStation.DefectCode_Name = dto.DefectCode_Name;
            workStation.Is_Enable = dto.Is_Enable;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var Fixture_DefectCodestring = fixtureService.EditFixture_DefectCode(workStation);
            return Fixture_DefectCodestring;
        }

        public string AddFixtureDefectCodeAPI(Fixture_DefectCodeDTO dto)
        {
            var workStation = AutoMapper.Mapper.Map<Fixture_DefectCode>(dto);
            workStation.Created_UID = dto.Modified_UID;
            workStation.Created_Date = DateTime.Now;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var workStationstring = fixtureService.AddFixture_DefectCode(workStation);
            if (workStationstring != "SUCCESS")
                return workStationstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteFixtureDefectCodeAPI(Fixture_DefectCodeDTO dto)
        {
            var result = fixtureService.DeleteFixture_DefectCode(dto.Fixture_Defect_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtureDefectCodeAPI(string uids)
        {
            var list = fixtureService.DoExportFixture_DefectCode(uids);
            return Ok(list);
        }
        [HttpPost]
        public string InsertFixtureDefectCodeAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Fixture_DefectCode>>(jsondata);
            return fixtureService.InsertFixture_DefectCodes(list);
        }
        [HttpGet]
        public IHttpActionResult GetFixture_DefectCodeAPI(string DefectCode_ID, int Plant_Organization_UID, int BG_Organization_UID)
        {
            var list = fixtureService.GetFixture_DefectCode(DefectCode_ID, Plant_Organization_UID, BG_Organization_UID);
            return Ok(list);
        }
        #endregion


        #region 维修对策维护
        [HttpPost]
        public IHttpActionResult IsFixtureRepairSolutionExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_RepairSolutionModelSearch>(jsonData);
            bool isExist = fixtureService.IsFixture_RepairSolutionExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public IHttpActionResult QueryFixtureRepairSolutionsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_RepairSolutionModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var workStations = fixtureService.QueryFixture_RepairSolutions(searchModel, page);
            return Ok(workStations);
        }

        [HttpPost]
        public IHttpActionResult GetFixtureRepairSolutionListAPI(Fixture_RepairSolutionModelSearch searchModel)
        {
            var workStations = fixtureService.GetRepairSolutionList(searchModel);
            return Ok(workStations);
        }

        [HttpGet]
        public IHttpActionResult QueryFixtureRepairSolutionAPI(int uid)
        {
            var dto = new Fixture_RepairSolutionDTO();
            dto = AutoMapper.Mapper.Map<Fixture_RepairSolutionDTO>(fixtureService.QueryFixture_RepairSolutionSingle(uid));
            return Ok(dto);
        }

        public string EditFixtureRepairSolutionAPI(Fixture_RepairSolutionDTO dto)
        {
            var workStation = fixtureService.QueryFixture_RepairSolutionSingle(dto.Fixture_RepairSolution_UID);
            workStation.Plant_Organization_UID = dto.Plant_Organization_UID;
            workStation.BG_Organization_UID = dto.BG_Organization_UID;
            workStation.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            workStation.RepairSolution_ID = dto.RepairSolution_ID;
            workStation.RepairSolution_Name = dto.RepairSolution_Name;
            workStation.Is_Enable = dto.Is_Enable;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var Fixture_RepairSolutionstring = fixtureService.EditFixture_RepairSolution(workStation);
            return Fixture_RepairSolutionstring;
        }

        public string AddFixtureRepairSolutionAPI(Fixture_RepairSolutionDTO dto)
        {
            var workStation = AutoMapper.Mapper.Map<Fixture_RepairSolution>(dto);
            workStation.Created_UID = dto.Modified_UID;
            workStation.Created_Date = DateTime.Now;
            workStation.Modified_UID = dto.Modified_UID;
            workStation.Modified_Date = DateTime.Now;
            var workStationstring = fixtureService.AddFixture_RepairSolution(workStation);
            if (workStationstring != "SUCCESS")
                return workStationstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteFixtureRepairSolutionAPI(Fixture_RepairSolutionDTO dto)
        {
            var result = fixtureService.DeleteFixture_RepairSolution(dto.Fixture_RepairSolution_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtureRepairSolutionAPI(string uids)
        {
            var list = fixtureService.DoExportFixture_RepairSolution(uids);
            return Ok(list);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixtureRepairSolutionsAPI()
        {
            var result = fixtureService.QueryAllFixture_RepairSolutions();
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllDefectCode_RepairSolutionAPI()
        {
            var result = fixtureService.QueryAllDefectCode_RepairSolution();
            return Ok(result);
        }

        [HttpPost]
        public string InsertFixtureRepairSolutionAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Fixture_RepairSolution>>(jsondata);
            return fixtureService.InsertFixture_RepairSolutions(list);
        }
        #endregion

        #region 车间维护
        [HttpPost]
        public IHttpActionResult IsWorkshopExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<WorkshopModelSearch>(jsonData);
            bool isExist = fixtureService.IsWorkshopExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public string InsertWorkshopAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Workshop>>(jsondata);
            return fixtureService.InsertWorkshops(list);
        }
        #endregion

        #region 制程维护
        [HttpPost]
        public IHttpActionResult IsProcessInfoExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Process_InfoModelSearch>(jsonData);
            bool isExist = fixtureService.IsProcess_InfoExist(searchModel);
            return Ok(isExist);
        }
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllProcessInfosAPI()
        {
            var result = fixtureService.QueryAllProcess_Infos();
            return Ok(result);
        }
        
        [HttpPost]
        public string InsertProcessInfosAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Process_Info>>(jsondata);
            return fixtureService.InsertProcess_Infos(list);
        }
        #endregion
        #region 治具配件维护
        [HttpPost]
        public IHttpActionResult IsFixturePartExistAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_PartModelSearch>(jsonData);
            bool isExist = fixtureService.IsFixture_PartExist(searchModel);
            return Ok(isExist);
        }
        [HttpPost]
        public IHttpActionResult QueryFixturePartsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_PartModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var fixtureParts = fixtureService.QueryFixture_Parts(searchModel, page);
            return Ok(fixtureParts);
        }
        [HttpPost]
        public IHttpActionResult QueryFixturePartListAPI(Fixture_PartModelSearch search)
        {
            var fixtureMachines = fixtureService.GetFixturePartList(search);
            return Ok(fixtureMachines);
        }
        [HttpGet]
        public IHttpActionResult QueryFixturePartAPI(int uid)
        {
            var dto = new Fixture_PartDTO();
            var fixturePart = fixtureService.QueryFixture_PartSingle(uid);
            if (fixturePart != null)
            {
                dto = AutoMapper.Mapper.Map<Fixture_PartDTO>(fixturePart);
                dto.Plant_Organization_Name = fixturePart.System_Organization.Organization_Name;
                dto.BG_Organization_Name = fixturePart.System_Organization1.Organization_Name;
                if (fixturePart.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixturePart.System_Organization2.Organization_Name;
                }
                dto.Created_UserName = fixturePart.System_Users.User_Name;
                dto.Modified_UserName = fixturePart.System_Users1.User_Name;
            }
            return Ok(dto);
        }

        public string EditFixturePartAPI(Fixture_PartDTO dto)
        {
            var fixturePart = fixtureService.QueryFixture_PartSingle(dto.Fixture_Part_UID);
            fixturePart.Plant_Organization_UID = dto.Plant_Organization_UID;
            fixturePart.BG_Organization_UID = dto.BG_Organization_UID;
            fixturePart.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            fixturePart.Part_ID = dto.Part_ID;
            fixturePart.Part_Name = dto.Part_Name;
            fixturePart.Part_Spec = dto.Part_Spec;
            fixturePart.Is_Automation = dto.Is_Automation;
            fixturePart.Is_Standardized = dto.Is_Standardized;
            fixturePart.Is_Storage_Managed = dto.Is_Storage_Managed;
            fixturePart.Purchase_Cycle = dto.Purchase_Cycle;
            fixturePart.Is_Enable = dto.Is_Enable;
            fixturePart.Modified_UID = dto.Modified_UID;
            fixturePart.Modified_Date = DateTime.Now;
            var Fixture_Machinestring = fixtureService.EditFixture_Part(fixturePart);
            return Fixture_Machinestring;
        }

        public string AddFixturePartAPI(Fixture_PartDTO dto)
        {
            var fixturePart = AutoMapper.Mapper.Map<Fixture_Part>(dto);
            fixturePart.Created_UID = dto.Modified_UID;
            fixturePart.Created_Date = DateTime.Now;
            fixturePart.Modified_UID = dto.Modified_UID;
            fixturePart.Modified_Date = DateTime.Now;
            var workStationstring = fixtureService.AddFixture_Part(fixturePart);
            if (workStationstring != "SUCCESS")
                return workStationstring;
            else
                return "SUCCESS";
        }

        /// <summary>
        /// 删除治具配件
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>SUCCSSS/FAIL</returns>
        [AcceptVerbs("Post")]
        public string DeleteFixturePartAPI(Fixture_PartDTO dto)
        {
            var result = fixtureService.DeleteFixture_Part(dto.Fixture_Part_UID);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DoExportFixturePartAPI(string uids)
        {
            var list = fixtureService.DoExportFixture_Part(uids);
            return Ok(list);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixturePartsAPI()
        {
            var result = fixtureService.QueryAllFixture_Parts();
            return Ok(result);
        }
        [HttpPost]
        public string InserFixturePartsAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Fixture_Part>>(jsondata);
            return fixtureService.InsertFixture_Parts(list);
        }
        #endregion

        #endregion

        #region 治具履历查询-----------------------  Add by ROck 2017-10-03 --------start
        public IHttpActionResult FixtureResumeSearchVMAPI(dynamic data)
        {
            var entity = data.ToString();
            FixtureResumeSearchVM search = JsonConvert.DeserializeObject<FixtureResumeSearchVM>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.FixtureResumeSearchVM(search, page);
            return Ok(list);
        }
        [HttpPost]
        public IHttpActionResult DoAllExportFixtureResumeReprotAPI(FixtureResumeSearchVM search)
        {
            var result = fixtureService.DoAllExportFixtureResumeReprot(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryFixtureResumeByUIDAPI(int Fixture_Resume_UID, int Fixture_M_UID)
        {
            var item = fixtureService.QueryFixtureResumeByUID(Fixture_Resume_UID, Fixture_M_UID);
            return Ok(item);
        }

        [HttpGet]
        public IHttpActionResult ExportFixtureResumeByUIDAPI(string uids)
        {
            var list = fixtureService.ExportFixtureResumeByUID(uids);
            return Ok(list);
        }

        #endregion 治具履历查询-----------------------  Add by ROck 2017-10-03 --------end

        #region 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------Start
        [HttpGet]
        public IHttpActionResult GetMaintenanceStatusAPI(string Maintenance_Type)
        {
            var item = fixtureService.GetMaintenanceStatus(Maintenance_Type);
            return Ok(item);

        }

        public IHttpActionResult QueryFixtureNotMaintainedAPI(dynamic data)
        {
            var entity = data.ToString();
            NotMaintenanceSearchVM search = JsonConvert.DeserializeObject<NotMaintenanceSearchVM>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryFixtureNotMaintained(search, page);
            return Ok(list);

        }
        [HttpPost]
        public IHttpActionResult DoAllExportFixtureNotMaintainedReprotAPI(NotMaintenanceSearchVM search)
        {
            var result = fixtureService.DoAllExportFixtureNotMaintainedReprot(search);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult ExportFixtureNotMaintainedByUIDAPI(string uids, string hidDate)
        {
            var list = fixtureService.ExportFixtureNotMaintainedByUID(uids, hidDate);
            return Ok(list);
        }
        #endregion 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------End

        #region 异常原因群组设定
        /// <summary>
        /// 查询治具资料列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult QueryDefectCode_GroupAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<DefectCode_GroupDTO>(jsondata);
            var result = fixtureService.QueryDefectCode_Group(searchModel, page);
            return Ok(result);

        }

        [HttpPost]
        public IHttpActionResult DoAllExportDefectCode_GroupReprotAPI(DefectCode_GroupDTO search)
        {
            var result = fixtureService.DoAllExportDefectCode_GroupReprot(search);
            return Ok(result);
        }
        


        [HttpGet]
        public IHttpActionResult DoExportDefectCode_GroupReprotAPI(string DefectCode_Group_UIDs)
        {
            var dto = Ok(fixtureService.DoExportDefectCode_GroupReprot(DefectCode_Group_UIDs));
            return dto;
        }
        public string AddDefectCode_GroupAPI(DefectCode_GroupDTO dto)
        {
            var result = fixtureService.AddDefectCode_Group(dto);
            return result;
        }
        [HttpGet]
        public IHttpActionResult DefectCode_GroupListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.DefectCode_GroupList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DefectCode_GroupListAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.DefectCode_GroupList(Plant_Organization_UID);
            return Ok(result);
        }
        
        [HttpPost]
        public string InsertDefectCode_GroupAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<DefectCode_GroupDTO>>(jsondata);
            return fixtureService.InsertDefectCode_Group(list);
        }

        [HttpGet]
        public string DeleteDefectCode_Group_UIDAPI(string DefectCode_Group_UIDs)
        {
            var result = fixtureService.DeleteDefectCode_Group_UID(DefectCode_Group_UIDs);
            return result;
          
        }
        #endregion 异常原因群组设定


        #region 治具异常原因维护

        [HttpGet]
        public string DeleteFixtureDefectCode_Setting_UIDAPI(string FixtureDefectCode_Setting_UIDs)
        {
            var result = fixtureService.DeleteFixtureDefectCode_Setting_UID(FixtureDefectCode_Setting_UIDs);
            return result;
        }

        [HttpPost]
        public IHttpActionResult QueryFixtureDefectCode_SettingAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixtureDefectCode_SettingDTO>(jsondata);
            var result = fixtureService.QueryFixtureDefectCode_Setting(searchModel, page);
            return Ok(result);

        }

        [HttpPost]
        public IHttpActionResult DoAllExportFixtureDefectCode_SettingReprotAPI(FixtureDefectCode_SettingDTO search)
        {
            var result = fixtureService.DoAllExportFixtureDefectCode_SettingReprot(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtureDefectCode_SettingReprotAPI(string FixtureDefectCode_Setting_UIDs)
        {
            var dto = Ok(fixtureService.DoExportFixtureDefectCode_SettingReprot(FixtureDefectCode_Setting_UIDs));
            return dto;
        }
        public string AddFixtureDefectCode_SettingAPI(FixtureDefectCode_SettingDTO dto)
        {
            var result = fixtureService.AddFixtureDefectCode_Setting(dto);
            return result;
        }
        [HttpGet]
        public IHttpActionResult GetDefectCodeByGroupAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID)
        {
            var result = fixtureService.GetDefectCodeByGroup(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, DefectCode_Group_ID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetDefectCodeGroupAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, string DefectCode_Group_ID)
        {
            var result = fixtureService.DefectCode_Group(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, DefectCode_Group_ID);
            return Ok(result);
        }


        [HttpGet]
        public IHttpActionResult GetDefectCodesByPlantAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.GetDefectCodesByPlant(Plant_Organization_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixtureDefectCode_SettingByPlantAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.GetFixtureDefectCode_SettingByPlant(Plant_Organization_UID);
            return Ok(result);
        }

       /// <summary>
       /// 根据厂区获取治具信息
       /// </summary>
       /// <param name="Plant_Organization_UID"></param>
       /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetFixture_MByPlantAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.GetFixture_MByPlant(Plant_Organization_UID);
            return Ok(result);
        }

        [HttpPost]
        public string InsertFixtureDefectCode_SettingAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixtureDefectCode_SettingDTO>>(jsondata);
            return fixtureService.InsertFixtureDefectCode_Setting(list);
        }
        /// <summary>
        /// 根据厂区获取生产线信息
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetProduction_LineByPlantAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.GetProduction_LineByPlant(Plant_Organization_UID);
            return Ok(result);
        }
        /// <summary>
        /// 根据厂区获取机台信息
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetFixtureMachineByPlantAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.GetFixtureMachineDTOList(Plant_Organization_UID,0,0,0);
            return Ok(result);
        }
        /// <summary>
        /// 根据厂区获取生产车间信息
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetWorkShopByPlantAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.GetWorkshopList(Plant_Organization_UID, 0, 0);
            return Ok(result);
        }
        /// <summary>
        /// 根据厂区获取供应商信息
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetVendor_InfoByPlantAPI(int Plant_Organization_UID)
        {
            var result = fixtureService.GetVendor_InfoList(Plant_Organization_UID, 0);
            return Ok(result);
        }


        [HttpPost]
        public string Insertfixture_MAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixtureDTO>>(jsondata);
            return fixtureService.Insertfixture_M(list);
        }

        [HttpPost]
        public string Updatefixture_MAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixtureDTO>>(jsondata);
            return fixtureService.Updatefixture_MAPI(list);
        }

        [HttpGet]
        public IHttpActionResult GetDefectCode_GroupByUIDAPI(int DefectCode_Group_UID)
        {
            var result = fixtureService.GetDefectCode_GroupByUID(DefectCode_Group_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixtureDefectCode_SettingDTOByUIDAPI(int FixtureDefectCode_Setting_UID)
        {
            var result = fixtureService.GetFixtureDefectCode_SettingDTOByUID(FixtureDefectCode_Setting_UID);
            return Ok(result);
        }

        public string EditDefectCode_GroupAPI(DefectCode_GroupDTO dto)
        {
            var result = fixtureService.EditDefectCode_Group(dto);
            return result;
        }
        public string EditFixtureDefectCode_SettingAPI(FixtureDefectCode_SettingDTO dto)
        {
            var result = fixtureService.EditFixtureDefectCode_Setting(dto);
            return result;
        }

        #endregion 治具异常原因维护

        #region 治具维修次查询 Add by Rock 2017-10-30------------------------ Start
        public IHttpActionResult QueryReportByRepairAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryReportByRepair(search, page);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByRepairAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var list = fixtureService.ExportReportByRepair(search);
            return Ok(list);
        }
        #endregion 治具维修次查询 Add by Rock 2017-10-30------------------------ End

        #region 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ Start
        public IHttpActionResult QueryReportByRepairPersonAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryReportByRepairPerson(search, page);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByRepairPersonValidAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var list = fixtureService.ExportReportByRepairPersonValid(search);
            return Ok(list);
        }


        #endregion 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ End

        #region 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ Start
        public IHttpActionResult QueryReportByPageAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryReportByPage(search, page);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByPageValidAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var list = fixtureService.ExportReportByPageValid(search);
            return Ok(list);
        }


        #endregion 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ End

        #region 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ Start
        public IHttpActionResult QueryFixtureReportByDetailAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryFixtureReportByDetail(search, page);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByDetailValidAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var list = fixtureService.ExportReportByDetailValid(search);
            return Ok(list);
        }

        #endregion 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ End



        #region 报表-治具间维修时间分析报表 Add by Rock 2017-11-28------------------------ Start
        public IHttpActionResult QueryFixtureReportByAnalisisAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryFixtureReportByAnalisis(search, page);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByAnalisisValidAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var list = fixtureService.ExportReportByAnalisisValid(search);
            return Ok(list);
        }

        #endregion 报表-治具间维修时间分析报表 Add by Rock 2017-11-28------------------------ End


        #region 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------Start

        [HttpGet]
        public IHttpActionResult GetFixtureNoListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixtureService.GetFixtureNoList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        public IHttpActionResult QueryFixtureReportByStatusAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryFixtureReportByStatus(search, page);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByStatusValidAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var list = fixtureService.ExportReportByStatusValid(search);
            return Ok(list);
        }

        #endregion 报表-治具数量查询(治具状态) Add by Rock 2017-11-28----------------End

        #region 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------Start
        public IHttpActionResult QueryFixtureReportByStatusAnalisisAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryFixtureReportByStatusAnalisis(search, page);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByStatusAnalisisValidAPI(dynamic data)
        {
            var entity = data.ToString();
            ReportByRepair search = JsonConvert.DeserializeObject<ReportByRepair>(entity);
            var list = fixtureService.ExportReportByStatusAnalisisValid(search);
            return Ok(list);
        }

        #endregion 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------End

        #region 报表-FMT Dashboard Add by Rock 2017-12-11----------------Start
        public IHttpActionResult QueryFixtureReportByFMTAPI(dynamic data)
        {
            var entity = data.ToString();
            Batch_ReportByStatus search = JsonConvert.DeserializeObject<Batch_ReportByStatus>(entity);
            var page = JsonConvert.DeserializeObject<Page>(entity);
            var list = fixtureService.QueryFixtureReportByFMT(search, page);
            return Ok(list);

        }

        [HttpGet]
        public IHttpActionResult QueryQueryFixtureReportByFMTDetailAPI(int Process_Info_UID, string startDate, string endDate)
        {
            var list = fixtureService.QueryQueryFixtureReportByFMTDetail(Process_Info_UID, startDate, endDate);
            return Ok(list);
        }

        public IHttpActionResult ExportReportByFMTValidAPI(dynamic data)
        {
            var entity = data.ToString();
            Batch_ReportByStatus search = JsonConvert.DeserializeObject<Batch_ReportByStatus>(entity);
            var list = fixtureService.ExportReportByFMTValid(search);
            return Ok(list);
        }



        #endregion 报表-FMT Dashboard Add by Rock 2017-12-11----------------End




    }



}

