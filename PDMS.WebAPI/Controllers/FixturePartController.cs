using Newtonsoft.Json;
using PDMS.Core.Authentication;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Fixture;
using PDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PDMS.WebAPI.Controllers
{
    public class FixturePartController : ApiController
    {
        IFixturePartService fixturePartService;
        IFixturePartSettingMService fixturePartSettingMService;
        IFixturePartDemandMService fixturePartDemandMService;
        IFixturePartDemandDService fixturePartDemandDService;
        IFixturePartDemandSummaryMService fixturePartDemandSummaryMService;
        IFixturePartDemandSummaryDService fixturePartDemandSummaryDService;

        IFixture_Part_UseTimesService fixturePartUseTimesService;

        public FixturePartController(IFixturePartService fixturePartService, IFixturePartSettingMService fixturePartSettingMService, IFixturePartDemandMService fixturePartDemandMService, IFixturePartDemandDService fixturePartDemandDService, IFixturePartDemandSummaryMService fixturePartDemandSummaryMService, IFixturePartDemandSummaryDService fixturePartDemandSummaryDService
            , IFixture_Part_UseTimesService fixturePartUseTimesService)
        {
            this.fixturePartService = fixturePartService;
            this.fixturePartSettingMService = fixturePartSettingMService;
            this.fixturePartDemandMService = fixturePartDemandMService;
            this.fixturePartDemandDService = fixturePartDemandDService;
            this.fixturePartDemandSummaryMService = fixturePartDemandSummaryMService;
            this.fixturePartDemandSummaryDService = fixturePartDemandSummaryDService;
            this.fixturePartUseTimesService = fixturePartUseTimesService;
        }
        #region 治具配件维护
        [HttpPost]
        public IHttpActionResult QueryAllFixturePartsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_PartModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var fixtureParts = fixturePartService.QueryAllFixtureParts(searchModel);
            return Ok(fixtureParts);
        }
        
        #endregion
        #region 治具配件设定维护 Jay 2017-12-27
        [HttpPost]
        public IHttpActionResult QueryFixturePartSettingMsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_Part_Setting_MModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var fixtureParts = fixturePartService.QueryFixturePartSettingMs(searchModel, page);
            return Ok(fixtureParts);
        }

        public string AddFixturePartSettingAPI(Fixture_Part_Setting_MDTO dto)
        {
            var result = fixturePartService.AddFixturePartSetting(dto);
            return result;
        }
        public string EditFixturePartSettingAPI(Fixture_Part_Setting_MDTO dto)
        {
            var result = fixturePartService.EditFixturePartSetting(dto);
            return result;
        }

        [HttpGet]
        public IHttpActionResult GetFixturePartSettingMDTOByUIDAPI(int fixturePartSettingMUID)
        {
            var result = fixturePartService.GetFixturePartSettingMDTOByUID(fixturePartSettingMUID);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetFixturePartSettingMByQueryAPI(Fixture_Part_Setting_MModelSearch search)
        {
            var result = fixturePartService.GetFixturePartSettingMListByQuery(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportFixtuerPartSettingMAPI(string uids)
        {
            var list = fixturePartService.DoExportFixtuerPartSettingM(uids);
            return Ok(list);
        }

        //[HttpPost]
        //public IHttpActionResult IsFixturePartExistAPI(dynamic data)
        //{
        //    var jsonData = data.ToString();
        //    var searchModel = JsonConvert.DeserializeObject<Fixture_PartModelSearch>(jsonData);
        //    bool isExist = fixtureService.IsFixture_PartExist(searchModel);
        //    return Ok(isExist);
        //}
        //[HttpPost]
        //public IHttpActionResult QueryFixturePartListAPI(Fixture_PartModelSearch search)
        //{
        //    var fixtureMachines = fixtureService.GetFixturePartList(search);
        //    return Ok(fixtureMachines);
        //}
        //[HttpGet]
        //public IHttpActionResult QueryFixturePartAPI(int uid)
        //{
        //    var dto = new Fixture_PartDTO();
        //    var fixturePart = fixtureService.QueryFixture_PartSingle(uid);
        //    if (fixturePart != null)
        //    {
        //        dto = AutoMapper.Mapper.Map<Fixture_PartDTO>(fixturePart);
        //        dto.Plant_Organization_Name = fixturePart.System_Organization.Organization_Name;
        //        dto.BG_Organization_Name = fixturePart.System_Organization1.Organization_Name;
        //        if (fixturePart.System_Organization2 != null)
        //        {
        //            dto.FunPlant_Organization_Name = fixturePart.System_Organization2.Organization_Name;
        //        }
        //        dto.Created_UserName = fixturePart.System_Users.User_Name;
        //        dto.Modified_UserName = fixturePart.System_Users1.User_Name;
        //    }
        //    return Ok(dto);
        //}

        //public string EditFixturePartAPI(Fixture_PartDTO dto)
        //{
        //    var fixturePart = fixtureService.QueryFixture_PartSingle(dto.Fixture_Part_UID);
        //    fixturePart.Plant_Organization_UID = dto.Plant_Organization_UID;
        //    fixturePart.BG_Organization_UID = dto.BG_Organization_UID;
        //    fixturePart.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
        //    fixturePart.Part_ID = dto.Part_ID;
        //    fixturePart.Part_Name = dto.Part_Name;
        //    fixturePart.Part_Spec = dto.Part_Spec;
        //    fixturePart.Is_Automation = dto.Is_Automation;
        //    fixturePart.Is_Standardized = dto.Is_Standardized;
        //    fixturePart.Is_Storage_Managed = dto.Is_Storage_Managed;
        //    fixturePart.Safe_Storage_Ratio = dto.Safe_Storage_Ratio;
        //    fixturePart.Is_Enable = dto.Is_Enable;
        //    fixturePart.Modified_UID = dto.Modified_UID;
        //    fixturePart.Modified_Date = DateTime.Now;
        //    var Fixture_Machinestring = fixtureService.EditFixture_Part(fixturePart);
        //    return Fixture_Machinestring;
        //}

        //public string AddFixturePartAPI(Fixture_PartDTO dto)
        //{
        //    var fixturePart = AutoMapper.Mapper.Map<Fixture_Part>(dto);
        //    fixturePart.Created_UID = dto.Modified_UID;
        //    fixturePart.Created_Date = DateTime.Now;
        //    fixturePart.Modified_UID = dto.Modified_UID;
        //    fixturePart.Modified_Date = DateTime.Now;
        //    var workStationstring = fixtureService.AddFixture_Part(fixturePart);
        //    if (workStationstring != "SUCCESS")
        //        return workStationstring;
        //    else
        //        return "SUCCESS";
        //}

        ///// <summary>
        ///// 删除治具配件
        ///// </summary>
        ///// <param name="dto"></param>
        ///// <returns>SUCCSSS/FAIL</returns>
        //[AcceptVerbs("Post")]
        //public string DeleteFixturePartAPI(Fixture_PartDTO dto)
        //{
        //    var result = fixtureService.DeleteFixture_Part(dto.Fixture_Part_UID);
        //    return result;
        //}

        //[HttpGet]
        //public IHttpActionResult DoExportFixturePartAPI(string uids)
        //{
        //    var list = fixtureService.DoExportFixture_Part(uids);
        //    return Ok(list);
        //}

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFixturePartSettingMsAPI()
        {
            var result = fixturePartService.QueryAllFixturePartSettingMs();
            return Ok(result);
        }

        //治具配件动态查询
        [HttpPost]
        public IHttpActionResult QueryFixturePartAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<Fixture_PartModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<Fixture_PartModelSearch>>(jsonData);
            var result = fixturePartService.QueryDto(searchModel);
            return Ok(result);
        }

        //治具配件设定主表动态查询
        [HttpPost]
        public IHttpActionResult QueryFixturePartSettingMAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<Fixture_Part_Setting_MModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<Fixture_Part_Setting_MModelSearch>>(jsonData);
            var result = fixturePartSettingMService.QueryDto(searchModel);
            return Ok(result);
        }

        //判断是否重复
        [HttpPost]
        public IHttpActionResult IsFixturePartSettingMExistAPI(dynamic data)
        {
            var isExist = false;
            var jsonData = data.ToString();
            var search = JsonConvert.DeserializeObject<Fixture_Part_Setting_MModelSearch>(jsonData);
            var searchModel = new QueryModel<Fixture_Part_Setting_MModelSearch>() { Equal = search };
            var result = fixturePartSettingMService.QueryDto(searchModel);
            if (result.Count > 0)
            {
                isExist = true;
            }
            return Ok(isExist);
        }

        [HttpPost]
        public string InsertFixturePartSettingMsAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Fixture_Part_Setting_MDTO>>(jsondata);
            return fixturePartService.InsertFixturePartSettingMs(list);
        }

        [HttpPost]
        public string InsertOrUpdateFixturePartSettingDsAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Fixture_Part_Setting_DDTO>>(jsondata);
            return fixturePartService.InsertOrUpdateFixturePartSettingDs(list);
        }

        [AcceptVerbs("Post")]
        public string DeleteFixturePartSettingByUIDAPI(Fixture_Part_Setting_MDTO dto)
        {
            var result = fixturePartService.DeleteFixturePartSetting(dto.Fixture_Part_Setting_M_UID);
            return result;
        }

        #endregion
        #region 治具配件需求计算
        [HttpPost]
        public IHttpActionResult QueryQueryFixturePartDemandMsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_Part_Demand_MModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var fixtureParts = fixturePartDemandMService.QueryFixturePartDemandMs(searchModel, page);
            return Ok(fixtureParts);
        }

        [HttpGet]
        public IHttpActionResult QueryFixturePartDemandMByUIDAPI(int uid)
        {
            var dto = new Fixture_Part_Demand_MDTO();
            var demandM = fixturePartDemandMService.QueryFixturePartDemandMByUID(uid);
            if (demandM != null)
            {
                dto = AutoMapper.Mapper.Map<Fixture_Part_Demand_MDTO>(demandM);
                dto.Plant_Organization_Name = demandM.System_Organization.Organization_Name;
                dto.BG_Organization_Name = demandM.System_Organization1.Organization_Name;
                if (demandM.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = demandM.System_Organization2.Organization_Name;
                }
                dto.StatusName = demandM.Enumeration.Enum_Value;
                dto.Applicant_Name = demandM.System_Users.User_Name;
                dto.Approver_Name = demandM.System_Users1.User_Name;
            }
            var demandDDtoList = new List<Fixture_Part_Demand_DDTO>();
            foreach (var item in demandM.Fixture_Part_Demand_D)
            {
                var demandDto = AutoMapper.Mapper.Map<Fixture_Part_Demand_DDTO>(item);
                demandDto.Part_ID = item.Fixture_Part.Part_ID;
                demandDto.Part_Spec = item.Fixture_Part.Part_Spec;
                demandDto.Part_Name = item.Fixture_Part.Part_Name;
                demandDDtoList.Add(demandDto);
            }
            dto.Fixture_Part_Demand_DDTOList = demandDDtoList;
            return Ok(dto);
        }

        [HttpPost]
        public IHttpActionResult QueryFixturePartDemandMAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<Fixture_Part_Demand_MModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<Fixture_Part_Demand_MModelSearch>>(jsonData);
            var result = fixturePartDemandMService.QueryDto(searchModel);
            return Ok(result);
        }


        [HttpGet]
        public HttpResponseMessage CalculateFixturePartDemandAPI(int Plant_Organization_UID, int BG_Organization_UID, int? FunPlant_Organization_UID, DateTime Demand_Date,int Applicant_UID)
        {
            var response = new HttpResponseMessage();
            var result = fixturePartDemandMService.CalculateFixturePartDemand(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID, Demand_Date, Applicant_UID);
            response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        [HttpPost]
        public HttpResponseMessage EditFixturePartDemandDsAPI(List<Fixture_Part_Demand_DDTO> demandDList)
        {
            var response = new HttpResponseMessage();
            var result = fixturePartDemandDService.UpdateFixturePartDemandDList(demandDList);
            response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        /// <summary>
        /// 更新配件需求状态
        /// </summary>
        /// <param name="demandDList"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateFixturePartDemandMStatusAPI(int uid, int statusUID)
        {
            var response = new HttpResponseMessage();
            var result = fixturePartDemandMService.UpdateStatus(uid, statusUID);
            response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        [HttpPost]
        public IHttpActionResult QueryFixtureDemandDAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<Fixture_Part_Demand_DModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<Fixture_Part_Demand_DModelSearch>>(jsonData);
            var demandDList = fixturePartDemandDService.Query(searchModel);
            var demandDDtoList = new List<Fixture_Part_Demand_DDTO>();
            foreach (var item in demandDList)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Demand_DDTO>(item);
                dto.Part_ID = item.Fixture_Part.Part_ID;
                dto.Part_Name = item.Fixture_Part.Part_Name;
                dto.Part_Spec = item.Fixture_Part.Part_Spec;
                demandDDtoList.Add(dto);
            }
            return Ok(demandDDtoList);
        }

        [HttpGet]
        public HttpResponseMessage ApproveCancelFixturPartDemandAPI(int uid)
        {
            var response = new HttpResponseMessage();
            var result = fixturePartDemandMService.ApproveCancelFixturPartDemand(uid);
            response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }
        
        #endregion
        #region 治具配件需求计算汇总
        [HttpPost]
        public IHttpActionResult QueryQueryFixturePartDemandMSummarysAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_Part_Demand_Summary_MModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var fixtureParts = fixturePartDemandSummaryMService.QueryFixturePartDemandSummaryMs(searchModel, page);
            return Ok(fixtureParts);
        }

        [HttpGet]
        public IHttpActionResult QueryFixturePartDemandSummaryMByUIDAPI(int uid)
        {
            var dto = new Fixture_Part_Demand_Summary_MDTO();
            var demandSummaryM = fixturePartDemandSummaryMService.QueryFixturePartDemandSummaryMByUID(uid);
            if (demandSummaryM != null)
            {
                dto = AutoMapper.Mapper.Map<Fixture_Part_Demand_Summary_MDTO>(demandSummaryM);
                dto.Plant_Organization_Name = demandSummaryM.System_Organization.Organization_Name;
                dto.BG_Organization_Name = demandSummaryM.System_Organization1.Organization_Name;
                if (demandSummaryM.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = demandSummaryM.System_Organization2.Organization_Name;
                }
                dto.StatusName = demandSummaryM.Enumeration.Enum_Value;
                dto.Applicant_Name = demandSummaryM.System_Users.User_Name;
                dto.Approver_Name = demandSummaryM.System_Users1.User_Name;
            }
            var Demand_SummaryDDtoList = new List<Fixture_Part_Demand_Summary_DDTO>();
            foreach (var item in demandSummaryM.Fixture_Part_Demand_Summary_D)
            {
                var Demand_SummaryDto = AutoMapper.Mapper.Map<Fixture_Part_Demand_Summary_DDTO>(item);
                Demand_SummaryDto.Part_ID = item.Fixture_Part.Part_ID;
                Demand_SummaryDto.Part_Spec = item.Fixture_Part.Part_Spec;
                Demand_SummaryDto.Part_Name = item.Fixture_Part.Part_Name;
                Demand_SummaryDDtoList.Add(Demand_SummaryDto);
            }
            dto.Fixture_Part_Demand_Summary_DDTOList = Demand_SummaryDDtoList;
            return Ok(dto);
        }

        /// <summary>
        /// 更新配件需求汇总状态
        /// </summary>
        /// <param name="demandDList"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateFixturePartDemandSummaryMStatusAPI(int uid, int statusUID)
        {
            var response = new HttpResponseMessage();
            var result = fixturePartDemandSummaryMService.UpdateStatus(uid, statusUID);
            response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        /// <summary>
        /// 编辑需求汇总
        /// </summary>
        /// <param name="demandDList"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditFixturePartDemandSummaryDsAPI(List<Fixture_Part_Demand_Summary_DDTO> demandDList)
        {
            var response = new HttpResponseMessage();
            var result = fixturePartDemandSummaryDService.UpdateFixturePartDemandSummaryDList(demandDList);
            response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        [HttpGet]
        public HttpResponseMessage FixturePartDemandSubmitOrder(int demandSummaryMUID, int modifiedUID)
        {
            try
            {
                fixturePartDemandSummaryMService.FixturePartDemandSubmitOrder(demandSummaryMUID, modifiedUID);
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }
        }

        [HttpGet]
        public HttpResponseMessage CalculateFixturePartDemandSummaryAPI(int Fixture_Part_Demand_M_UID, int Applicant_UID)
        {
            var response = new HttpResponseMessage();
            var result = fixturePartDemandSummaryMService.CalculateFixturePartDemandSummary(Fixture_Part_Demand_M_UID, Applicant_UID);
            response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }
        [HttpPost]
        public IHttpActionResult QueryFixtureDemandSummaryDAPI(dynamic data)
        {
            var jsonData = data.ToString();
            QueryModel<Fixture_Part_Demand_Summary_DModelSearch> searchModel = JsonConvert.DeserializeObject<QueryModel<Fixture_Part_Demand_Summary_DModelSearch>>(jsonData);
            var demandDList = fixturePartDemandSummaryDService.Query(searchModel);
            var demandDDtoList = new List<Fixture_Part_Demand_Summary_DDTO>();
            foreach (var item in demandDList)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Demand_Summary_DDTO>(item);
                dto.Part_ID = item.Fixture_Part.Part_ID;
                dto.Part_Name = item.Fixture_Part.Part_Name;
                dto.Part_Spec = item.Fixture_Part.Part_Spec;
                demandDDtoList.Add(dto);
            }
            return Ok(demandDDtoList);
        }
        #endregion
        #region 仓库管理
        /// <summary>
        /// 根据厂区ID获取OP类型
        /// </summary>
        /// <param name="plantorguid"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult QueryOpTypesByUserAPI(int plantorguid)
        {
            var dto = fixturePartService.QueryOpType(plantorguid);
            return Ok(dto);
        }
        [HttpPost]
        public IHttpActionResult QueryWarehouseStoragesAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixturePartWarehouseDTO>(jsondata);
            var result = fixturePartService.QueryWarehouses(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryFunplantByopAPI(int opuid)
        {
            var dto = fixturePartService.QueryFunplantByop(opuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarehouseByuidAPI(int Warehouse_UID)
        {
            //获取仓库
            var war = fixturePartService.QueryWar(Warehouse_UID)[0];
            //获取储位
            var storages = fixturePartService.QueryWarhouseSts(Warehouse_UID);

            var result = new FixturePartWarehouseStorages
            {
                Fixture_Warehouse_UID = war.Fixture_Warehouse_UID,
                Plant_Organization_UID = war.Plant_Organization_UID,
                Plant = war.Plant,
                BG_Organization_UID = war.BG_Organization_UID,
                BG_Organization = war.BG_Organization,
                FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                FunPlant_Organization = war.FunPlant_Organization,
                Fixture_Warehouse_ID = war.Fixture_Warehouse_ID,
                Fixture_Warehouse_Name = war.Fixture_Warehouse_Name,
                Remarks = war.Remarks,
                Is_Enable = war.Is_Enable,
                Createder = war.Createder,
                Created_UID = war.Created_UID,
                Created_Date = war.Created_Date,
                Modifier = war.Modifier,
                Modified_UID = war.Modified_UID,
                Modified_Date = war.Modified_Date,
                Storages = storages
            };
            return Ok(result);
        }

        public string AddOrEditWarehouseInfoAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<FixturePartWarehouseStorages>(data.ToString());
            return fixturePartService.AddOrEditWarehouseInfo(entity);
        }

        [HttpGet]
        public string DeleteWarehouseStorageAPI(int WareStorage_UId)
        {
            return fixturePartService.DeleteWarehouseStorage(WareStorage_UId);
        }
        [HttpGet]
        public string DeleteWarehouseAPI(int Warehouse_UID)
        {
            return fixturePartService.DeleteWarehouse(Warehouse_UID);
        }

        [HttpGet]
        public IHttpActionResult QueryAllWarehouseStAPI(int Plant_Organization_UID)
        {
            var result = fixturePartService.QueryAllWarehouseSt(Plant_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetFixtureWarehouseStorageALLAPI(int Plant_Organization_UID)
        {
            var result = fixturePartService.GetFixtureWarehouseStorageALL(Plant_Organization_UID);
            return Ok(result);
        }
      
        [HttpPost]
        public string InsertWarehouseAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixturePartWarehouseDTO>>(jsondata);
            return fixturePartService.InsertWarehouseBase(list);
        }
        [HttpPost]
        public string InsertWarehouse_StorageAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixturePartWarehouseDTO>>(jsondata);
            return fixturePartService.InsertWarehouse(list);
        }

        [HttpGet]
        public IHttpActionResult DoExportWarehouseReprotAPI(string uids)
        {
            var dto = Ok(fixturePartService.DoExportWarehouseReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportWarehouseReprotAPI(FixturePartWarehouseDTO search)
        {
            var result = fixturePartService.DoAllExportWarehouseReprot(search);
            return Ok(result);
        }
        #endregion
        #region 开账作业
        [HttpPost]
        public IHttpActionResult QueryCreateBoundsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixturePartStorageInboundDTO>(jsondata);
            var result = fixturePartService.QueryCreateBounds(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixtureStatuDTOAPI()
        {
            var dto = fixturePartService.GetFixtureStatuDTO();
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetFixtureStatuDTOAPI(string Enum_Type)
        {
            var dto = fixturePartService.GetFixtureStatuDTO(Enum_Type);
            return Ok(dto);
        }
     
        [HttpGet]
        public IHttpActionResult GetFixtureWarehouseStoragesAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixturePartService.GetFixtureWarehouseStorages(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetFixturePartsAPI(int Fixture_Warehouse_Storage_UID)
        {
            var result = fixturePartService.GetFixtureParts(Fixture_Warehouse_Storage_UID);
            return Ok(result);
        }
        public string AddOrEditCreateBoundAPI(FixturePartStorageInboundDTO dto)
        {
            return fixturePartService.AddOrEditCreateBound(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryInboundByUidAPI(int Fixture_Storage_Inbound_UID)
        {

            var result = fixturePartService.QueryInboundByUid(Fixture_Storage_Inbound_UID);
            return Ok(result);

        }
        [HttpGet]
        public string DeleteCreateBoundAPI(int Fixture_Storage_Inbound_UID)
        {
            return fixturePartService.DeleteCreateBound(Fixture_Storage_Inbound_UID);
        }
        [HttpGet]
        public IHttpActionResult DoExportCreateBoundReprotAPI(string uids)
        {
            var dto = Ok(fixturePartService.DoExportCreateBoundReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportCreateBoundReprotAPI(FixturePartStorageInboundDTO search)
        {
            var result = fixturePartService.DoAllExportCreateBoundReprot(search);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult QueryAllStorageInboundAPI(int Plant_Organization_UID)
        {

            var result = fixturePartService.QueryAllStorageInbound(Plant_Organization_UID);
            return Ok(result);

        }
        [HttpPost]
        public string InsertCreateBoundAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixturePartStorageInboundDTO>>(jsondata);
            return fixturePartService.InsertCreateBoundItem(list);
        }

        [HttpGet]
        public string ApproveCreateboundByUidAPI(int Fixture_Storage_Inbound_UID, int Useruid)
        {
            return fixturePartService.ApproveCreatebound(Fixture_Storage_Inbound_UID, Useruid);
        }
        #endregion
        #region Add by Rock 2018-01-10 ----------------采购单维护作业----------start
        public IHttpActionResult QueryPurchaseAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FixturePart_OrderVM>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var fixtureParts = fixturePartService.QueryPurchase(searchModel, page);
            return Ok(fixtureParts);
        }

        [HttpPost]
        [IgnoreDBAuthorize]
        public IHttpActionResult doAllPurchaseorderMaintainAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FixturePart_OrderVM>(jsonData);
            var list = fixturePartService.doAllPurchaseorderMaintain(searchModel);
            return Ok(list);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult doPartPurchaseorderMaintainAPI(string uids)
        {
            var list = fixturePartService.doPartPurchaseorderMaintain(uids);
            return Ok(list);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryPurchaseByUIDAPI(int UID)
        {
            var result = fixturePartService.QueryPurchaseByUID(UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixturePartByPlantOptypeFuncAPI(int PlantUID, int Optype, int FuncUID)
        {
            var result = fixturePartService.GetFixturePartByPlantOptypeFunc(PlantUID,Optype, FuncUID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixturePartByMUIDAPI(int UID)
        {
            var result = fixturePartService.GetFixturePartByMUIDAPI(UID);
            return Ok(result);
        }

        [IgnoreDBAuthorize]
        [HttpGet]
        public string DeletePurchaseByUIDSAPI(string json)
        {
            List<int> idList = JsonConvert.DeserializeObject<List<int>>(json);
            var result = fixturePartService.DeletePurchaseByUIDS(idList);
            return result;
        }

        public string SaveFixturePartByMUID(dynamic data)
        {
            var jsonData = data.ToString();
            var item = JsonConvert.DeserializeObject<SubmitFixturePartOrderVM>(jsonData);
            var result = fixturePartService.SaveFixturePartByMUID(item);
            return result;
        }

        #endregion Add by Rock 2018-01-10 ----------------采购单维护作业----------end
        #region  出入库维护作业
        [HttpPost]
        public IHttpActionResult QueryBoundDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixturePartInOutBoundInfoDTO>(jsondata);
            var result = fixturePartService.QueryBoundDetails(searchModel, page);
            return Ok(result);
        }

        /// <summary>
        /// 导出ALL出入库维护
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportAllOutboundInfoAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FixturePartInOutBoundInfoDTO>(jsondata);
            var result = fixturePartService.ExportAllOutboundInfo(searchModel);
            return Ok(result);
        }

        /// <summary>
        /// 导出勾选出入库维护
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ExportPartOutboundInfoAPI(string uids)
        {
            var result = fixturePartService.ExportPartOutboundInfo(uids);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixture_Part_Order_MsAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixturePartService.GetFixture_Part_Order_Ms(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetFixturePartDTOListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixturePartService.GetFixturePartDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult  GetWarehouseStorageByFixtureUIDAPI(int Fixture_Part_UID)
        {
            var result = fixturePartService.GetWarehouseStorageByFixture_Part_UID(Fixture_Part_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixture_Part_Order_DsAPI(int Fixture_Part_Order_M_UID)
        {
            var result = fixturePartService.GetFixture_Part_Order_Ds(Fixture_Part_Order_M_UID);
            return Ok(result);
        }
        public string AddOrEditInboundApplyAPI(FixturePartInOutBoundInfoDTO dto)
        {
            return fixturePartService.AddOrEditInboundApply(dto);
        }
        [HttpGet]
        public IHttpActionResult GetFixture_Part_Order_DAPI(int Fixture_Part_Order_D_UID)
        {
            var result = fixturePartService.GetFixture_Part_Order_D(Fixture_Part_Order_D_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixture_Part_WarehousesAPI(int Fixture_Part_Order_D_UID)
        {
            var result = fixturePartService.GetFixture_Part_Warehouses(Fixture_Part_Order_D_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryInboundByUidAPI(int Storage_In_Out_Bound_UID, string Inout_Type)
        {
            if (Inout_Type == "入库单")
            {              
                var result = fixturePartService.QueryInBoudSingle(Storage_In_Out_Bound_UID);
                return Ok(result);
            }
            else
            {
                var dto = fixturePartService.QueryOutBoudSingle(Storage_In_Out_Bound_UID);
                //GetMatinventory()
                var details = fixturePartService. QueryOutBouddetails(Storage_In_Out_Bound_UID);

                var result = new FixturePartInOutBoundInfoDTO
                {
                    Storage_In_Out_Bound_UID = dto.Storage_In_Out_Bound_UID,
                    Storage_In_Out_Bound_D_UID = dto.Storage_In_Out_Bound_D_UID,
                    Storage_In_Out_Bound_Type_UID = dto.Storage_In_Out_Bound_Type_UID,
                    InOut_Type = dto.InOut_Type,
                    In_Out_Type = dto.In_Out_Type,
                    Fixture_Part_Order_M_UID = dto.Fixture_Part_Order_M_UID,
                    Fixture_Part_Order = dto.Fixture_Part_Order,
                    Issue_NO = dto.Issue_NO,
                    Fixture_Repair_M_UID = dto.Fixture_Repair_M_UID,
                    Fixture_Repair_ID = dto.Fixture_Repair_ID,
                    SentOut_Number = dto.SentOut_Number,
                    SentOut_Name = dto.SentOut_Name,
                    SentOut_Date = dto.SentOut_Date,
                    Storage_In_Out_Bound_ID = dto.Storage_In_Out_Bound_ID,
                    Status_UID = dto.Status_UID,
                    Fixture_Part_UID = dto.Fixture_Part_UID,
                    Fixture_Warehouse_Storage_UID = dto.Fixture_Warehouse_Storage_UID,
                    In_Out_Bound_Qty = dto.In_Out_Bound_Qty,
                    Applicant_UID = dto.Applicant_UID,
                    Applicant_Date = dto.Applicant_Date,
                    Applicant = dto.Applicant,
                    Approve_UID = dto.Approve_UID,
                    Approve_Date = dto.Approve_Date,
                    Approve = dto.Approve,
                    Outbound_Account_UID = dto.Outbound_Account_UID,
                    Outbound_Account = dto.Outbound_Account,
                    Plant_Organization_UID = dto.Plant_Organization_UID,
                    Plant = dto.Plant,
                    BG_Organization_UID = dto.BG_Organization_UID,
                    BG_Organization = dto.BG_Organization,
                    FunPlant_Organization_UID = dto.FunPlant_Organization_UID,
                    FunPlant_Organization = dto.FunPlant_Organization,
                    Storage_ID = dto.Storage_ID,
                    Rack_ID = dto.Rack_ID,
                    Fixture_Warehouse_ID = dto.Fixture_Warehouse_ID,
                    Fixture_Warehouse_Name = dto.Fixture_Warehouse_Name,
                    Part_ID = dto.Part_ID,
                    Part_Name = dto.Part_Name,
                    Part_Spec = dto.Part_Spec,
                    Status = dto.Status,
                    Price = dto.Price,
                    Qty = dto.Qty,
                    Remarks = dto.Remarks,
                    details = details
                };
                return Ok(result);

            
            }
        }
        [HttpGet]
        public IHttpActionResult GetFixtureWarehousesAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixturePartService.GetFixtureWarehouses(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }
        public string SaveWarehouseStorageAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<FixturePartWarehouseStorageDTO>(data.ToString());
            return fixturePartService.SaveWarehouseStorage(entity);
        }
        [HttpGet]
        public string DeleteBoundAPI(int Storage_In_Out_Bound_UID , string Inout_Type)
        {
            return fixturePartService.DeleteBound(Storage_In_Out_Bound_UID, Inout_Type);
        }
        [HttpGet]
        public string ApproveInboundByUidAPI(int Storage_In_Out_Bound_UID, int Useruid)
        {
            return fixturePartService.ApproveInbound(Storage_In_Out_Bound_UID, Useruid);
        }

        [HttpGet]
        public IHttpActionResult GetMatinventoryAPI(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID)
        {
            var result = fixturePartService.GetMatinventory(Fixture_Part_UID, Fixture_Warehouse_Storage_UID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixtureinventoryAPI(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID)
        {
            var result = fixturePartService.GetFixtureinventory(Fixture_Part_UID, Fixture_Warehouse_Storage_UID,new Page() {PageNumber=0,PageSize=10});
            return Ok(result);
        }
        public string AddOrEditOutboundAPI(dynamic data)
        {
            var matentity = JsonConvert.DeserializeObject<FixturePartInOutBoundInfoDTO>(data.ToString());
            return fixturePartService.AddOrEditOutbound(matentity);
        }

        [HttpGet]
        public string ApproveOutboundByUidAPI(int Storage_In_Out_Bound_UID, int Useruid)
        {
            return fixturePartService.ApproveOutbound(Storage_In_Out_Bound_UID, Useruid);
        }


        #endregion
        #region 储位移转维护作业
        [HttpPost]
        public IHttpActionResult QueryStorageTransfersAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<Fixture_Storage_TransferDTO>(jsondata);
            var result = fixturePartService.QueryStorageTransfers(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixturePartWarehouseStoragesAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixturePartService.GetFixturePartWarehouseStorages(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);
        }

        public string AddOrEditStorageTransferAPI(dynamic data)
        {
            var matentity = JsonConvert.DeserializeObject<Fixture_Storage_TransferDTO>(data.ToString());
            return fixturePartService.AddOrEditStorageTransfer(matentity);
        }
        [HttpGet]
        public IHttpActionResult QueryStorageTransferByUidAPI(int Fixture_Storage_Transfer_UID)
        {
            var result = fixturePartService.QueryStorageTransferByUid(Fixture_Storage_Transfer_UID);
            return Ok(result);
        }

        [HttpGet]
        public string DeleteStorageTransferAPI(int Fixture_Storage_Transfer_UID, int userid)
        {
            return fixturePartService.DeleteStorageTransfer(Fixture_Storage_Transfer_UID, userid);
        }
        [HttpGet]
        public IHttpActionResult DoExportStorageTransferReprotAPI(string uids)
        {
            var dto = Ok(fixturePartService.DoExportStorageTransferReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportStorageTransferReprotAPI(Fixture_Storage_TransferDTO search)
        {
            var result = fixturePartService.DoAllExportStorageTransferReprot(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult Fixture_Part_InventoryDTOListAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var result = fixturePartService.Fixture_Part_InventoryDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return Ok(result);

        }

        [HttpPost]
        public string ImportStorageTransferAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Fixture_Storage_TransferDTO>>(jsondata);
            return fixturePartService.ImportStorageTransfer(list);
        }

        [HttpGet]
        public string ApproveStTransferAPI(int Fixture_Storage_Transfer_UID, int Useruid)
        {
            return fixturePartService.ApproveStTransfer(Fixture_Storage_Transfer_UID, Useruid);
        }
        #endregion
        #region 盘点维护作业
        [HttpPost]
        public IHttpActionResult QueryStorageChecksAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixtureStorageCheckDTO>(jsondata);
            var result = fixturePartService.QueryStorageChecks(searchModel, page);
            return Ok(result);
        }
        public string AddOrEditStorageCheckAPI(FixtureStorageCheckDTO dto)
        {
            return fixturePartService.AddOrEditStorageCheck(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryStorageCheckByUidAPI(int Fixture_Storage_Check_UID)
        {
            var result = fixturePartService.QueryStorageCheckSingle(Fixture_Storage_Check_UID);         
            return Ok(result);
        }
        [HttpGet]
        public string DeleteStorageCheckAPI(int Fixture_Storage_Check_UID, int userid)
        {
            return fixturePartService.DeleteStorageCheck(Fixture_Storage_Check_UID, userid);
        }
        [HttpGet]
        public IHttpActionResult DoExportStorageCheckReprotAPI(string uids)
        {
            var dto = Ok(fixturePartService.DoExportStorageCheckReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportStorageCheckReprotAPI(FixtureStorageCheckDTO search)
        {
            var result = fixturePartService.DoAllExportStorageCheckReprot(search);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult DownloadStorageCheckAPI(string Part_ID, string Part_Name, string Part_Spec, string Fixture_Warehouse_ID, string Rack_ID, string Storage_ID)
        {
            var dto = Ok(fixturePartService.DownloadStorageCheck(Part_ID, Part_Name, Part_Spec, Fixture_Warehouse_ID, Rack_ID, Storage_ID));
            return dto;
        }

        [HttpPost]
        public string ImportStorageCheckAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<FixtureStorageCheckDTO>>(jsondata);
            return fixturePartService.ImportStorageCheck(list);
        }

        [HttpGet]
        public string ApproveStCheckAPI(int Fixture_Storage_Check_UID, int Useruid)
        {
            return fixturePartService.ApproveStCheck(Fixture_Storage_Check_UID, Useruid);
        }

        [HttpPost]
        public IHttpActionResult ApproveStorageCheckAPI(FixtureStorageCheckDTO search)
        {
            var result = fixturePartService.ApproveStorageCheck(search);
            return Ok(result);
        }


        /// <summary>
        /// 获取库位管理明细报表的API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetFPStoryDetialReportAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<Fixture_Part_InventoryDTO>(jsondata);
            var result = fixturePartService.GetFPStoryDetialReport(searchModel, page);
            return Ok(result);
        }

        /// <summary>
        /// 导出ALL库位管理明细报表的API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportAllFixtureInventoryAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<Fixture_Part_InventoryDTO>(jsondata);
            var result = fixturePartService.ExportAllFixtureInventoryDetialReport(searchModel);
            return Ok(result);
        }

        /// <summary>
        /// 导出勾选的库位管理明细报表的API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ExportSelectedFixtureInventoryAPI(string uids)
        {
            var result = fixturePartService.ExportSelectedFixtureInventoryDetialReport(uids);
            return Ok(result);
        }

        /// <summary>
        /// 获取出入库明细报表的API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetInOutDetialReportDataAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixtureInOutStorageModel>(jsondata);
            var result = fixturePartService.GetInOutDetialReport(searchModel, page);
            return Ok(result);
        }

        /// <summary>
        /// 导出出入库报表明细信息API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportALLInOutDetialReportDataAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<FixtureInOutStorageModel>(jsondata);
            var result = fixturePartService.ExportAllInOutDetialReport(searchModel);
            return Ok(result);
        }


        /// <summary>
        /// 导出勾选的出入库报表明细信息API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ExportSelectedInOutDetialReportDataAPI(string  uids)
        {
            var result = fixturePartService.ExportSelectedInOutDetialReport(uids);
            return Ok(result);
        }
        #endregion
        #region 库存表报查询
        [HttpPost]
        public IHttpActionResult QueryStorageReportsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<FixturePartStorageReportDTO>(jsondata);
            var result = fixturePartService.QueryStorageReports(searchModel, page);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult DoSRExportFunctionAPI(int plant, int bg, int funplant, string Part_ID, string Part_Name, string Part_Spec, DateTime start_date, DateTime end_date)
        {
            var dto = fixturePartService.DoSRExportFunction(plant, bg, funplant, Part_ID, Part_Name, Part_Spec,start_date, end_date);
            return Ok(dto);
        }
        #endregion
        [HttpGet]
        public IHttpActionResult GetFixturePartScanCodeDTOAPI(int plantID,int optypeID,string SN, int Modified_UID)
        {
            var result = fixturePartService.GetFixturePartScanCodeDTO(plantID,optypeID,SN,  Modified_UID);  
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixtureScanCodeDTOBySNAPI(int plantID,int optypeID,string SN)
        {
            var result = fixturePartService.GetFixtureScanCodeDTOBySN(plantID,optypeID,SN);  
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetFixturePartDTOByFixtureUIDAPI(int fixtureUID)
        {
            //查询Fixture_Part_UseTimes再过滤
            var fixturePartUseTimesQuery = fixturePartUseTimesService.Query(new QueryModel<Fixture_Part_UseTimesModelSearch>() { Equal = new Fixture_Part_UseTimesModelSearch() { Fixture_M_UID = fixtureUID } });
            var enabledFixturePartUseTimesList = fixturePartUseTimesQuery.Where(x => x.Fixture_Part_Setting_D.IsUseTimesManagement == true).OrderBy(y=>y.Fixture_Part_Setting_D.Fixture_Part.Part_Name).ToList();
            var fixturePartUseTimesDTOList = new List<Fixture_Part_UseTimesDTO>();
            foreach (var item in enabledFixturePartUseTimesList)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_UseTimesDTO>(item);
                dto.Part_ID = item.Fixture_Part_Setting_D.Fixture_Part.Part_ID;
                dto.Part_Name = item.Fixture_Part_Setting_D.Fixture_Part.Part_Name;
                dto.Part_Spec = item.Fixture_Part_Setting_D.Fixture_Part.Part_Spec;
                dto.Fixture_Part_Qty = item.Fixture_Part_Setting_D.Fixture_Part_Qty;
                dto.Fixture_Part_Life_UseTimes = item.Fixture_Part_Setting_D.Fixture_Part_Life_UseTimes;
                dto.IsNeedUpdate = item.Fixture_Part_UseTimesCount > dto.Fixture_Part_Life_UseTimes ? true : false;
                dto.Modified_UserName = item.System_Users.User_Name;
                dto.Fixture_Part_UID = item.Fixture_Part_Setting_D.Fixture_Part_UID;
                if (item.Fixture_Part_ClearHistory.Count>0)
                {
                    var lastHistory = item.Fixture_Part_ClearHistory.OrderByDescending(x => x.Modified_Date).FirstOrDefault();
                    if (lastHistory != null)
                    {
                        dto.LastClear_UserName = lastHistory.System_Users.User_Name;
                        dto.LastClear_DateTime = lastHistory.Modified_Date;
                    }
                }
                
                fixturePartUseTimesDTOList.Add(dto);
            }
            return Ok(fixturePartUseTimesDTOList);
        }

        [HttpPost]
        public IHttpActionResult ClearFixturePartUseTimesAPI(dynamic data)//(int[] uidList, int modifiedUid, DateTime modifiedDate)
        {
            //解析匿名类型，不用新建类
            var parameterType = new { uidList = new List<int>(), modifiedUid = 0, modifiedDate = DateTime.Now };
            var jsondata = data.ToString();
            var parameter = JsonConvert.DeserializeAnonymousType(jsondata, parameterType);
            var result = fixturePartUseTimesService.ClearFixturePartUseTimes(parameter.uidList, parameter.modifiedUid, parameter.modifiedDate);
            return Ok(result);

            //var jsondata = data.ToString();
            //var searchModel = JsonConvert.DeserializeObject<FixtureInOutStorageModel>(jsondata);
            //var result = fixturePartService.ExportAllInOutDetialReport(searchModel);
            //return Ok(result);
        }
    }
}
