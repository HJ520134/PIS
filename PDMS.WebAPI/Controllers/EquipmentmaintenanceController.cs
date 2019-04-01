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
    public class EquipmentmaintenanceController : ApiController
    {
        IEquipmentmaintenanceService equipmentmaintenanceService;
        IMaterialManageService materialManageService;

        public EquipmentmaintenanceController(IEquipmentmaintenanceService equipmentmaintenanceService, IMaterialManageService materialManageService)
        {
            this.equipmentmaintenanceService = equipmentmaintenanceService;
            this.materialManageService = materialManageService;
        }

        #region safestock
        public string EditSafeStockoAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<SafeStockDTO>>(jsondata);
            return equipmentmaintenanceService.UpdateSafeStock(list);
        }

        public IHttpActionResult GetSafeStockparameterAPI()
        {
            var dto = equipmentmaintenanceService.QuerySafeStock();
            return Ok(dto);
        }
        #endregion

        #region  Equipment
        [HttpPost]
        public IHttpActionResult QueryEquipmentInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EquipmentInfoSearchDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var bus = equipmentmaintenanceService.QueryEquipmentInfo(searchModel, page);
            return Ok(bus);
        }

        /// <summary>
        /// 导出全部设备信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportAllEquipmentInfoAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EquipmentInfoSearchDTO>(jsonData);
            var bus = equipmentmaintenanceService.ExportALLEquipmentInfo(searchModel);
            return Ok(bus);
        }

        /// <summary>
        /// 导出勾选部分的设备信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ExportPartEquipmentInfoAPI(string uids)
        {
            var result = equipmentmaintenanceService.ExportPartEquipmentInfo(uids);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryEquipmentAPI(string EQP_UID)
        {
            if (string.IsNullOrEmpty(EQP_UID))
                return Ok("");
            var dto = equipmentmaintenanceService.QueryEquipmentSingle(EQP_UID);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryProjectsAPI(int oporgid)
        {
            var dto = equipmentmaintenanceService.QueryProjects(oporgid);
            return Ok(dto);
        }
        
        [HttpGet]
        public IHttpActionResult QueryFunplantsAPI(int oporgid, string optypes="")
        {
            var dto = equipmentmaintenanceService.QueryFunplants(oporgid, optypes);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryAllProjectsAPI(int optype)
        {
            var dto = equipmentmaintenanceService.QueryAllProjects(optype);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryOpTypesByUserAPI(int oporguid, int accuntid = 0)
        {
            var dto = equipmentmaintenanceService.QueryOpType(oporguid,  accuntid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetCurrentOPTypeAPI(int parentOrg_UID, int organization_UID)
        {
            var dto = equipmentmaintenanceService. GetCurrentOPType( parentOrg_UID, organization_UID); 
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryAllPlantAPI(string optype)
        {
            var dto = equipmentmaintenanceService.QueryAllPlant(optype);
            return Ok(dto);
        }

        public string EditOrEditEquipmentInfoAPI( EquipmentInfoDTO dto, string EQP_UID,bool isEdit)
        {
            return equipmentmaintenanceService.AddOrEditEquipmentInfo(dto, EQP_UID, isEdit);
        }

        [HttpPost]
        public string InsertEquipmentAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<EquipmentInfoDTO>>(jsondata);
            return equipmentmaintenanceService.InsertEquipmentItem(list);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllProjectAPI()
        {
            var result = equipmentmaintenanceService.QueryAllProject();
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllEquipmentAPI(string location,int funplant,int optype)
        {
            var result = equipmentmaintenanceService.QueryAllEquipment( location, funplant, optype);
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllEquipmentAPI()
        {
            var result = equipmentmaintenanceService.QueryAllEquipment();
            return Ok(result);
        }

        [HttpGet]
        public string DeleteEquipmentAPI(int EQP_UID)
        {
            return equipmentmaintenanceService.DeleteEquipment(EQP_UID);
        }

        [HttpGet]
        public string CheckoptypesandplantAPI(string types,string plantname)
        {
            var result = equipmentmaintenanceService.Getplanguid(types, plantname);
            return result;
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllFunctionPlantsAPI()
        {
            var result = equipmentmaintenanceService.QueryAllFunctionPlants();
            return Ok(result);
        }

        #endregion
        #region   EQP_UserTable
        [HttpPost]
        public IHttpActionResult QueryEQPUserAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPUserTableDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = equipmentmaintenanceService.QueryEQPUser(searchModel,page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryEQPUserAPI(int EQPUser_Uid)
        {
            var dto = equipmentmaintenanceService.QueryEQPUserSingle(EQPUser_Uid);
            return Ok(dto);
        }

        public string AddOrEditEQPUserAPI(EQPUserTableDTO dto, bool isEdit)
        {
           return equipmentmaintenanceService.AddOrEditEQPUserInfo(dto, isEdit);
        }

        [HttpGet]
        public string DeleteEQPUserAPI(int EQPUser_Uid)
        {
            return equipmentmaintenanceService.DeleteEQPUser(EQPUser_Uid);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllEQPUserAPI()
        {
            var result = equipmentmaintenanceService.QueryAllEQPUser();
            return Ok(result);
        }

        [HttpPost]
        public string InsertEQPUserAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<EQPUserTableDTO>>(jsondata);
            return equipmentmaintenanceService.InsertEQPUserItem(list);
        }
        #endregion
        #region  material
        [HttpPost]
        public IHttpActionResult QueryMaterialAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<MaterialInfoDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = equipmentmaintenanceService.QueryMaterialr(searchModel,page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportMaterialReprotAPI(string uids)
        {
            var dto = Ok(equipmentmaintenanceService.DoExportMaterialReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportMaterialReprotAPI(MaterialInfoDTO search)
        {
            var result = equipmentmaintenanceService.DoAllExportMaterialReprot(search);
            return Ok(result);
        }



        [HttpGet]
        public IHttpActionResult QueryMaterialByUidAPI(int Material_Uid)
        {
            var dto = equipmentmaintenanceService.QueryMaterialSingle(Material_Uid);
            return Ok(dto);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllMaterialAPI()
        {
            var result = equipmentmaintenanceService.QueryAllMaterialInfo();
            return Ok(result);
        }
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllImportMaterialInfoAPI()
        {
            var result = equipmentmaintenanceService.QueryAllImportMaterialInfo();
            return Ok(result);
        }

   
        [HttpGet]
        public IHttpActionResult QueryAllOPMaterialAPI(int PlantID)
        {
            var result = equipmentmaintenanceService.QueryAllOPMaterialInfo(PlantID);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryOPMaterialByKeyAPI(int PlantID,string key)
        {
            var result = equipmentmaintenanceService.QueryOPMaterialInfoByKey(PlantID,key);
            return Ok(result);
        }

        public string AddOrEditMaterialInfoAPI(MaterialInfoDTO dto, bool isEdit)
        {
            return equipmentmaintenanceService.AddOrEditMaterialInfo(dto, isEdit);
        }

        [HttpGet]
        public string DeleteMaterialAPI(int Material_Uid)
        {
            return equipmentmaintenanceService.DeleteMaterial(Material_Uid);
        }

        [HttpPost]
        public string InsertMaterialAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<MaterialInfoDTO>>(jsondata);
            return equipmentmaintenanceService.InsertMaterialItem(list);
        }
        #endregion

        #region  Error Info

        [HttpPost]
        public IHttpActionResult QueryErrorInfoAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<ErrorInfoDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = equipmentmaintenanceService.QueryErrorInfo(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryErrorInfoAPI(int Enum_UID)
        {
            var dto = equipmentmaintenanceService.QueryErrorInfoSingle(Enum_UID);
            return Ok(dto);
        }

        [HttpGet]
        public string DeleteErrorInfoAPI(int Enum_UID)
        {
            return equipmentmaintenanceService.DeleteErrorInfo(Enum_UID);
        }

        public string AddOrEditErrorInfoAPI(ErrorInfoDTO dto,bool isEdit)
        {
            return equipmentmaintenanceService.AddErrorInfoItem( dto, isEdit);
        }

        #endregion

        #region EQPMaintenance

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult EQPMaintenanceAPI()
        {
            var errortype = equipmentmaintenanceService.GetAllErrorTypes();
            return Ok(errortype);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult EQPAllUserAPI(int bgUID ,int funplantUID, int plantUID)
        {
            var userlist = equipmentmaintenanceService.GetAllUser(bgUID, funplantUID,  plantUID);
            return Ok(userlist);
        }
        
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetEQPLocationAPI(int Plant_Organization_UID)
        {
            var userlist = equipmentmaintenanceService.GetAllEQPLocation(Plant_Organization_UID);
            return Ok(userlist);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserFunplantAPI(int userid,int optypeuid)
        {
            var userlist = equipmentmaintenanceService.GetUserFunPlantByUser(userid, optypeuid);
            return Ok(userlist);
        }
        
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetUserOPTypeAPI(int userid)
        {
            var userlist = equipmentmaintenanceService.GetUserOPTpye(userid);
            return Ok(userlist);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult EQPAllMaterialAPI(int planId = 0)
        {
            var materiallist = equipmentmaintenanceService.GetAllMaterial(planId);
            return Ok(materiallist);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult EQPUnitMatAPI()
        {
            var materiallist = equipmentmaintenanceService.GetUnitMaterial();
            return Ok(materiallist);
        }

        [HttpPost]
        public IHttpActionResult QueryEQPMaintenanceAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<EQPRepairInfoSearchDTO>(jsondata);
            var result = equipmentmaintenanceService.QueryRepairInfo(searchModel,page);
            return Ok(result);

        }

        [HttpGet]
        public IHttpActionResult QueryEQPMaintenanceByUidAPI(int Repair_Uid)
        {
            var dto = equipmentmaintenanceService.QueryEQPRepairSingle(Repair_Uid);
            var laborlist = equipmentmaintenanceService.GetLaborList(Repair_Uid);
            var materiallist = equipmentmaintenanceService.GetMatList(Repair_Uid);
            var result = new EQPRepair
            {
                OP_Types = dto.OP_TYPES,
                FunPlant = dto.FunPlant,
                Process = dto.Process,
                EQP_Plant_No = dto.EQP_Plant_No,
                Class_Desc = dto.Class_Desc,
                Mfg_Serial_Num = dto.Mfg_Serial_Num,
                EQP_Location = dto.EQP_Location,
                Reason_Analysis = dto.Reason_Analysis,
                Equipment = dto.Equipment,
                Repair_Reason = dto.Repair_Reason,
                Error_Types = dto.Error_Types,
                Error_Level = dto.Error_Level,
                Contact = dto.Contact,
                Contact_tel = dto.Contact_tel,
                Repair_id = dto.Repair_id,
                Repair_BeginTime = dto.Repair_BeginTime,
                Repair_Remark = dto.Repair_Remark,
                Repair_EndTime = dto.Repair_EndTime,
                Reason_Types = dto.Reason_Types,
                Repair_Method = dto.Repair_Method,
                Repair_Result = dto.Repair_Result,
                Repair_Uid = dto.Repair_Uid,
                EQP_Uid = dto.EQP_Uid,
                Asset = dto.Asset,
                Error_Time = dto.Error_Time,
                Apply_Time = dto.Apply_Time,
                matlist = materiallist,
                laborlist = laborlist,
                Organization_UID = dto.Organization_UID,
                System_FunPlant_UID = dto.System_FunPlant_UID,
                FunPlant_OrganizationUID = dto.FunPlant_OrganizationUID,
                Mentioner = dto.Mentioner,
                //成本中心
                CostCtr_UID = dto.CostCtr_UID,
                CostCtr_ID = dto.CostCtr_ID,
                CostCtr_Description = dto.CostCtr_Description
            };
            return Ok(result);
        }

        public string AddOrEditEQPMaintenanceAPI(EQPRepairInfoDTO dto,int eqp_id,bool isEdit)
        {
            var result = equipmentmaintenanceService.AddOrEditEQPRepair(dto, eqp_id, isEdit);
            return result;
        }

        public string AddManyMaintenanceAPI(Newtonsoft.Json.Linq.JObject json)
        {
            // the apihelper apipost async ? could not post string?
            string json_values = json.ToString();
            object dic = JsonConvert.DeserializeObject(json_values, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            var dtos = (Dictionary<int, EQPRepairInfoDTO>)dic;
            Dictionary<int, string> row_errors = equipmentmaintenanceService.AddManyEQPRepairs(dtos);
            if (row_errors.Count > 0)
            {
                var jsonString = JsonConvert.SerializeObject(row_errors, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
                return jsonString;
            }
            return string.Empty;
        }

        public string AddManyMaintenanceAPI2(Newtonsoft.Json.Linq.JObject json)
        {
            // the apihelper apipost async ? could not post string?
            string json_values1 = json["data"].ToString();
            string json_values2 = json["labors"].ToString();
            string json_values3 = json["materials"].ToString();
            object dic1 = JsonConvert.DeserializeObject(json_values1, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            object dic2 = JsonConvert.DeserializeObject(json_values2, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            object dic3 = JsonConvert.DeserializeObject(json_values3, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            var dtos1 = (Dictionary<int, EQPRepairInfoDTO>)dic1;
            var dtos2 = (Dictionary<double, Dictionary<int, LaborUsingInfoDTO>>)dic2;
            var dtos3 = (Dictionary<double, Dictionary<int, MeterialUpdateInfoDTO>>)dic3;
            Dictionary<int, string> row_errors = equipmentmaintenanceService.AddManyEQPRepairs2(dtos1, dtos2, dtos3);
            if (row_errors.Count > 0)
            {
                var jsonString = JsonConvert.SerializeObject(row_errors, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
                return jsonString;
            }
            return string.Empty;
        }

        public string AddManyMaintenanceAPI3(Newtonsoft.Json.Linq.JObject json)
        {
            // the apihelper apipost async ? could not post string?
            string json_values = json.ToString();
            object dic = JsonConvert.DeserializeObject(json_values, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            var dtos = (Dictionary<int, EQPRepairInfoDTO>)dic;

            Dictionary<int, string> row_errors = equipmentmaintenanceService.AddManyEQPRepairsClose(dtos);
            if (row_errors.Count > 0)
            {
                var jsonString = JsonConvert.SerializeObject(row_errors, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
                return jsonString;
            }
            return string.Empty;
        }

        [HttpGet]
        public string DeleteEQPRepariAPI(int Repair_Uid)
        {
            return equipmentmaintenanceService.DeleteEQPRepairInfo(Repair_Uid);
        }

        [HttpGet]
        public IHttpActionResult QueryMaterialUpdateByUidAPI(int Material_Uid)
        {
            var dto = equipmentmaintenanceService.QueryMaterialUpdateSingle(Material_Uid);
            return Ok(dto);
        }

        public string AddOrEditEQPMaintenance2API(EQPRepairInfoDTO dto,bool MH_Flag)
        {
            var result = equipmentmaintenanceService.AddOrEditEQPRepair2(dto, MH_Flag);
            return result;
        }
        

        public string UpdateLaborTableAPI(dynamic labordata,int userid,int Repair_Uid,bool MH_Flag)
        {
            var laborentity = JsonConvert.DeserializeObject<List<LaborUsingInfoDTO>>(labordata.ToString());
            return equipmentmaintenanceService.UpdateLabor(laborentity, userid, Repair_Uid, MH_Flag);
        }

        public string UpdateMatTableAPI(dynamic jsonMatData, int userid, DateTime updatetime, int EQP_Uid,
                                        int Repair_Uid,bool MH_Flag)
        {
            try
            {
                var laborentity = JsonConvert.DeserializeObject<List<MeterialUpdateInfoDTO>>(jsonMatData.ToString());
                return equipmentmaintenanceService.UpdateMat(laborentity, userid, updatetime, EQP_Uid, Repair_Uid, MH_Flag);
            }
            catch
            {
                return "请正确填写更新材料数量(正整数)";
            }
        }

        
        [HttpGet]
        public IHttpActionResult QueryUserRoleAPI(string userid)
        {
            var result = equipmentmaintenanceService.QueryUserRole(userid);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetOptypeAPI()
        {
            var dto = equipmentmaintenanceService.GetOptype();
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetOptypeAPIByUserAPI(int optype)
        {
            var dto = equipmentmaintenanceService.GetOptypeByUser(optype);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetProjectnameByOptypeAPI(string Optype)
        {
            var dto = equipmentmaintenanceService.GetProjectnameByOptype(Optype);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult GetFunPlantByOPTypeAPI( int Optype, string Optypes="")
        {
            var dto = equipmentmaintenanceService.GetFunPlantByOPType(Optype, Optypes);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult GetOrganization_PlantsAPI()
        {
            var dto = equipmentmaintenanceService.GetOrganization_Plants();
            return Ok(dto);
        }
        
        [HttpGet]
        public IHttpActionResult GetProcessByFunplantAPI(string funplantuid)
        {
            var dto = equipmentmaintenanceService.GetProcessByFunplant(funplantuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetEqpidByProcessAPI( string funplantuid)
        {
            var dto = equipmentmaintenanceService.GetEqpidByProcess(funplantuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult DoExportFunctionAPI(string uids)
        {
            var dto = Ok(equipmentmaintenanceService.DoExportFunction(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllEQPMaintenanceReprotAPI(EQPRepairInfoSearchDTO search)
        {
            var result = equipmentmaintenanceService.DoAllEQPMaintenanceReprot(search);
            return Ok(result);
        }



        [HttpGet]
        public IHttpActionResult DoExportFunctionAPI2( string optypes, string projectname, string funplant, string process,
                        string eqpid, string errortype, string repairid,string location,string classdesc,string contact,
                        DateTime fromdate,DateTime todate, string errorlever, string repairresult, string labor,
                        string updatepart, string remark,string status)
        {
            var dto = equipmentmaintenanceService.DoExportFunction2(optypes,projectname,funplant,process,eqpid, 
                errortype, repairid,location,classdesc, contact,fromdate,todate, errorlever, repairresult, labor,
                        updatepart, remark, status);
            return Ok(dto);
        }
        /// <summary>
        /// 导出部分
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DoPartExportFunctionAPI(string uids)
        {
            var result = equipmentmaintenanceService.DoPartExportFunctionInfo(uids);
            return Ok(result);
        }


        [HttpGet]
        public string EQPMaintenanceCloseAPI(int Repair_Uid)
        {
            return equipmentmaintenanceService.CloseEQPRepairInfo(Repair_Uid);
        }

        /// <summary>
        /// 取得成本中心信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCostCtrsAPI()
        {
            var dto = equipmentmaintenanceService.GetCostCtrs();
            return Ok(dto);
        }

        #endregion EQPMaintenance

        #region EQPMaintenanceReport

        [HttpPost]
        public IHttpActionResult QueryEQPMaintenanceReportAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<EQPRepairInfoSearchDTO>(jsondata);
            var result = equipmentmaintenanceService.QueryRepairReportInfo(searchModel, page);
            return Ok(result);

        }
        #endregion EQPMaintenanceReport

        #region CostCenterMaintenace Add By Darren 2018/12/18

        [AcceptVerbs("Post")]
        public IHttpActionResult QueryCostCentersAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<CostCenterModelSearch>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var costCenters = equipmentmaintenanceService.QueryCostCenters(searchModel, page);
            return Ok(costCenters);
        }

        /// <summary>
        /// Get specific function and its subfunctions by key
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult QueryCostCenterAPI(int uid)
        {
            var entity = equipmentmaintenanceService.QueryCostCenter(uid);
            
            var result = new CostCtr_infoDTO
            {
                Plant_Organization_UID = entity.Plant_Organization_UID,
                BG_Organization_UID = entity.BG_Organization_UID,
                FunPlant_Organization_UID = entity.FunPlant_Organization_UID,
                CostCtr_ID = entity.CostCtr_ID,
                CostCtr_Description = entity.CostCtr_Description,
                CostCtr_UID = entity.CostCtr_UID
            };
            return Ok(result);
        }

        /// <summary>
        /// Add function and its sub funtions
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //[AcceptVerbs("Post")]
        public string AddCostCenterAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<CostCtr_infoDTO>(data.ToString());
            return equipmentmaintenanceService.AddCostCenter(entity);
        }

        /// <summary>
        /// Modify function and its sub functions
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //[AcceptVerbs("Post")]
        public string ModifyCostCenterAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<CostCtr_infoDTO>(data.ToString());
            return equipmentmaintenanceService.ModifyCostCenter(entity);
        }

        /// <summary>
        /// Delete function
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns>operate result</returns>
        [HttpGet]
        public string DeleteCostCenterAPI(int uid)
        {
            return equipmentmaintenanceService.DeleteCostCenter(uid);
        }
        
        #endregion //end CostCenterMaintenace 

        #region 电子看板
        [AcceptVerbs("Post")]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetEQPBoardAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPRepairInfoDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var checkData = equipmentmaintenanceService.getShowContent(searchModel, page);
            var result = Ok(checkData);
            return result;
        }
        #endregion
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetFunPlantsAPI(string selectProjects,string opType)
        {
            var userlist = equipmentmaintenanceService.GetFunPlants(selectProjects, opType);
            return Ok(userlist);
        }
        //电子看板中的楼栋
        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult GetEBoardLocationAPI(string optype)
        {
            var userlist = equipmentmaintenanceService.GetAllEBoardLocation(optype);
            return Ok(userlist);
        }


        #region 机台类型、机台料号对应、每日开机-----------Robert2017/04/03

        #region 机台类型
        public string AddEQPTypeAPI(EQPTypeDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<EQP_Type>(dto);
            return materialManageService.AddEQPType(ent);
            
        }

        public string EditEQPTypeAPI(EQPTypeDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<EQP_Type>(dto);
            materialManageService.EditEQPType(ent);
            return "SUCCESS";
        }
        [HttpGet]
        public string DelEQPTypeAPI(int id)
        {
            var ent = materialManageService.QueryEQPTypeSingle(id);         
            string str= materialManageService.DelEQPType(ent);
            return str;
        }
        [HttpGet]
        public IHttpActionResult GetEQPTypesAPI()
        {
            return Ok( materialManageService.GetEQPTypes());
        }

        [HttpPost]
        public IHttpActionResult QueryEQPTypesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPTypeDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var Orgs = materialManageService.QueryEQPTypes(searchModel, page);
            return Ok(Orgs);
        }
        [HttpGet]
        public IHttpActionResult DoExportEQPTypeReprotAPI(string uids)
        {
            var dto = Ok(materialManageService.DoExportEQPTypeReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportEQPTypeReprotAPI(EQPTypeDTO search)
        {
            var result = materialManageService.DoAllExportEQPTypeReprot(search);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetEQPTypeAllsAPI()
        {
            var dto = materialManageService.GetEQPTypeAlls();
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryEQPTypeAPI(int uuid)
        {
            var dto = new EQPTypeDTO();
            dto = AutoMapper.Mapper.Map<EQPTypeDTO>(materialManageService.QueryEQPTypeSingle(uuid));
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryEQPTypeByBgAndFunplantAPI(int bg, int funplant)
        {
            var dto = new List<EQPTypeDTO>();
            dto = AutoMapper.Mapper.Map<List<EQPTypeDTO>>(materialManageService.QueryEQPTypeByBgAndFunplant(bg,funplant));
            return Ok(dto);
        }
        #endregion

        #region 机型料号对应关系
        [HttpPost]
        public IHttpActionResult QueryEQPMaterialsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPMaterialDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var Orgs = materialManageService.QueryEQPMaterials(searchModel, page);
            return Ok(Orgs);
        }

        [HttpGet]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryMaterialByMaterialIdAPI(string uuid)
        {
            var dto = new MaterialInfoDTO();
            dto = materialManageService.GetMaterialByMaterialId(uuid);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryMaterialByMaterialAPI()
        {

            var dto = materialManageService.GetMaterialByMaterialId();
            return Ok(dto);
        } 
        public string InsertEQPTypeAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<EQPTypeDTO>>(jsondata);
            return materialManageService.InsertEQPType(list);
        }
        public IHttpActionResult AddEQPMaterialAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var model = JsonConvert.DeserializeObject<List<EQPMaterialDTO>>(jsonData);
            bool flag = materialManageService.AddMaterial(model);
            return Ok(flag);
        }

        public IHttpActionResult EditEQPMaterialAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var model = JsonConvert.DeserializeObject<List<EQPMaterialDTO>>(jsonData);
            bool flag = materialManageService.EditMaterial(model);
            return Ok(flag);
        }
        public string InsertEQPMaterialAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<EQPMaterialDTO>>(jsondata);
            return materialManageService.InsertEQPMaterial(list);
        }
        [HttpGet]
        public IHttpActionResult QueryEQPMaterialsAllAPI()
        {

            var dto = new List<EQPMaterialDTO>();
            dto = AutoMapper.Mapper.Map<List<EQPMaterialDTO>>(materialManageService.QueryEQPMaterialsAll());
            return Ok(dto);
        }
        [HttpGet]
        public string DelEQPMaterialAPI(int uuid)
        {
            var dto = materialManageService.QueryMaterialsByEQPId(uuid).FirstOrDefault();
            materialManageService.DelMaterial(dto);
            return "SUCCESS";
        }

        [HttpGet]
        public IHttpActionResult QueryEQPMaterialAPI(int uuid)
        {
            var dto = new List<EQPTypeDTO>();
            dto = AutoMapper.Mapper.Map<List<EQPTypeDTO>>(materialManageService.QueryMaterialsByEQPId(uuid));
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryEQPMaterialByEqpAPI(int eqp)
        {
            var dto = new List<EQPMaterialDTO>();
            dto = AutoMapper.Mapper.Map<List<EQPMaterialDTO>>(materialManageService.GetMaterialByEqp(eqp));
            return Ok(dto);
        }
        
        #endregion

        #region 每日开机
        [HttpGet]
        public IHttpActionResult CheckFunplantAPI(string name,int op)
        {
            int i=materialManageService.QueryFunplant(name,op);
            return Ok(i);
        }

        [HttpGet]
        public IHttpActionResult CheckOPAPI(string name, int plant)
        {
            int i =materialManageService.QueryOP(name, plant);
            return Ok(i);
        }

        [HttpGet]
        public IHttpActionResult CheckEQPTypeAPI(int op,int funplant,string name)
        {
            int i = materialManageService.QueryEQPTypeSingle(op,funplant,name);
            return Ok(i);
        }

        [HttpGet]
        public IHttpActionResult CheckEQPPowerOnAPI(int eqpType, DateTime date)
        {
            int i = materialManageService.QueryPowerOn(eqpType,date);
            return Ok(i);
        }

        [HttpGet]
        public IHttpActionResult CheckEQPForecastPowerOnAPI(int eqpType, DateTime date)
        {
            int i = materialManageService.QueryForecastPowerOn(eqpType, date);
            return Ok(i);
        }

        public IHttpActionResult AddEQPPowerOnListAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var model = JsonConvert.DeserializeObject<List<EQPPowerOnDTO>>(jsonData);
            bool flag = materialManageService.AddPowerOnList(model);
            return Ok(flag);
        }

        public IHttpActionResult AddForecastPowerOnListAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var model = JsonConvert.DeserializeObject<List<EQPForecastPowerOnDTO>>(jsonData);
            bool flag = materialManageService.AddForecastPowerOnList(model);
            return Ok(flag);
        }

        public string AddEQPPowerOnAPI(EQPPowerOnDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<EQP_PowerOn>(dto);
            int i=materialManageService.AddPowerOn(ent);
            if (i == 0)
            {
                return "SUCCESS";
            }
            else if (i == -1)
            {
                return "Error,请联系管理员";
            }
            else
            {
                return "当前机台在" + dto.PowerOn_Date + "已经存在数据";
            }
        }

        public string EditEQPPowerOnAPI(EQPPowerOnDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<EQP_PowerOn>(dto);
            int i=materialManageService.EditPowerOn(ent);
            if (i == 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "Error,请联系管理员";
            }
        }
        [HttpGet]
        public string DelEQPPowerOnAPI(int uuid)
        {
            materialManageService.DelPowerOn(uuid);
            return "SUCCESS";
        }

        [HttpPost]
        public IHttpActionResult QueryEQPPowerOnsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPPowerOnDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var Orgs = materialManageService.QueryEQPPowerOns(searchModel, page);
            return Ok(Orgs);
        }

        [HttpGet]
        public IHttpActionResult QueryEQPPowerOnAPI(int uuid)
        {
            var dto =materialManageService.QueryEQPPowerOnSingle(uuid);
            dto.PowerOnDateString = dto.PowerOn_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
            return Ok(dto);
        }

        public string AddForecastPowerOnAPI(EQPForecastPowerOnDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<EQP_Forecast_PowerOn>(dto);
            int i=materialManageService.AddForecastPowerOn(ent);
            if (i == 0)
            {
                return "SUCCESS";
            }
            else if (i == -1)
            {
                return "Error,请联系管理员";
            }
            else
            {
                return "当前机台在" + dto.Demand_Date + "已经存在数据";
            }
        }

        public string EditForecastPowerOnAPI(EQPForecastPowerOnDTO dto)
        {
            var ent = AutoMapper.Mapper.Map<EQP_Forecast_PowerOn>(dto);
            int i=materialManageService.EditForecastPowerOn(ent);
            if (i == 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "Error,请联系管理员";
            }
        }
        [HttpGet]
        public string DelForecastPowerOnAPI(int uuid)
        {
            materialManageService.DelForecastPowerOn(uuid);
            return "SUCCESS";
        }

        [HttpPost]
        public IHttpActionResult QueryForecastPowerOnsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPForecastPowerOnDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var Orgs = materialManageService.QueryForecastPowerOns(searchModel, page);
            return Ok(Orgs);
        }

        [HttpGet]
        public IHttpActionResult QueryForecastPowerOnAPI(int uuid)
        {
            var dto = materialManageService.QueryForecastPowerOnSingle(uuid);
            dto.PowerOnDateString = dto.Demand_Date.ToString(FormatConstants.DateTimeFormatStringByDate);
            return Ok(dto);
        }
        [HttpGet]      
        public IHttpActionResult GetSystem_OrganizationAPI()
        {
            var users = materialManageService.GetSystem_Organization();
            return Ok(users);
        }
        [HttpGet]
        public IHttpActionResult GetSystem_OrganizationBOMAPI()
        {
            var users = materialManageService.GetSystem_OrganizationBOM();
            return Ok(users);
        }
        [HttpGet]
        public IHttpActionResult QueryOrgByNameAPI(string name)
        {
            var dto = materialManageService.QueryOrgByName(name);
            return Ok(dto);
        }
        #endregion
        #endregion

        #region 重要料号原因维护

        [HttpPost]
        public IHttpActionResult QueryMatReasonAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<MaterialReasonDTO>(jsondata);
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var result = equipmentmaintenanceService.QueryMatReason(searchModel, page);
            return Ok(result);
        }

        public string AddOrEditMaterialReasonAPI(MaterialReasonDTO dto)
        {
            return equipmentmaintenanceService.AddOrEditMaterialReason(dto);
        }

        [HttpGet]
        public IHttpActionResult QuerymatReasonByuidAPI(int Material_Reason_UID)
        {
            var dto = equipmentmaintenanceService.QuerymatReasonByuid(Material_Reason_UID);
            var result = new MaterialReasonDTO
            {
                Material_Reason_UID = dto[0].Material_Reason_UID,
                Material_UID = dto[0].Material_UID,
                Material_Id = dto[0].Material_Id,
                Material_Name = dto[0].Material_Name,
                Material_Types = dto[0].Material_Types,
                Reason = dto[0].Reason
            };
            return Ok(result);
        }

        [HttpGet]
        public string DeleteMatReasonAPI(int Material_Reason_UID, int userid)
        {
            return equipmentmaintenanceService.DeleteMatReason(Material_Reason_UID, userid);
        }

        public IHttpActionResult GetMatReasonByMatAPI(int Material_Uid)
        {
            var dto = equipmentmaintenanceService.GetMatReasonByMat(Material_Uid);
            return Ok(dto);
        }

        [HttpPost]
        public string InsertMatReasonAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<MaterialReasonDTO>>(jsondata);
            return equipmentmaintenanceService.InsertMatReasonItem(list);
        }

        [HttpGet]
        public IHttpActionResult DoExportMatReasonReprotAPI(string Material_Reason_UIDs)
        {
            var dto = Ok(equipmentmaintenanceService.DoExportMatReasonReprot(Material_Reason_UIDs));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportMatReasonReprotAPI(MaterialReasonDTO search)
        {
            var result = equipmentmaintenanceService.DoAllExportMatReasonReprot(search);
            return Ok(result);
        }
        

        #endregion 重要料号原因维护   

        [HttpPost]
        public IHttpActionResult QueryEquipmentInfoNOTReprotAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EquipmentReport>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var bus = equipmentmaintenanceService.QueryEquipmentInfoNOTReprot(searchModel, page);
            return Ok(bus);
        }

        [HttpPost]
        public IHttpActionResult QueryEquipmentInfoReprotAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EquipmentReport>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);
            var bus = equipmentmaintenanceService.QueryEquipmentInfoReprot(searchModel, page);
            return Ok(bus);
        }

        [HttpGet]
        public IHttpActionResult ExportEquipmentInfoReprotAPI(int plantId, int optypeId, int funplantId, int project_UID, string class_Desc,string mfg_Of_Asset,int organization_UID)
        {
            var searchModel = new EquipmentReport();
            searchModel.Plant_OrganizationUID = plantId;
            if(optypeId==0)
            {
                searchModel.OPType_OrganizationUID = organization_UID;
            }
            else
            {
                searchModel.OPType_OrganizationUID = optypeId;
            }
          
            searchModel.FunPlant_OrganizationUID = funplantId;
            searchModel.Project_UID = project_UID;
            searchModel.Class_Desc = class_Desc;
            searchModel.Mfg_Of_Asset = mfg_Of_Asset;
           
            var bus = equipmentmaintenanceService.ExportEquipmentInfoReprot(searchModel);
            return Ok(bus);
        }

        [HttpGet]
        public IHttpActionResult ExportEquipmentInfoNOTReprotAPI(int plantId, int optypeId, int funplantId,string class_Desc, string mfg_Of_Asset,int organization_UID)
        {
            var searchModel = new EquipmentReport();
            searchModel.Plant_OrganizationUID = plantId;
            if (optypeId == 0)
            {
                searchModel.OPType_OrganizationUID = organization_UID;
            }
            else
            {
                searchModel.OPType_OrganizationUID = optypeId;
            }
            searchModel.FunPlant_OrganizationUID = funplantId;
            searchModel.Mfg_Of_Asset = mfg_Of_Asset;
            searchModel.Class_Desc = class_Desc;
          
            var bus = equipmentmaintenanceService.ExportEquipmentInfoNOTReprot(searchModel);
            return Ok(bus);
        }
        [HttpPost]
        public IHttpActionResult QueryMaterial_Maintenance_RecordsAPI(dynamic data)
        {      
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<Material_Maintenance_RecordDTO>(jsondata);
            var result = equipmentmaintenanceService.QueryMaterial_Maintenance_Records(searchModel, page);
            return Ok(result);
        }
        public string AddOrEditMaterial_Maintenance_RecordAPI(Material_Maintenance_RecordDTO dto)
        {
            return equipmentmaintenanceService.AddOrEditMaterial_Maintenance_Record(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryMaterial_Maintenance_RecordByUidAPI(int Material_Maintenance_Record_UID)
        {
            var dto = equipmentmaintenanceService.QueryMaterial_Maintenance_RecordByUid(Material_Maintenance_Record_UID);
            return Ok(dto);
        }
        [HttpGet]
        public string Approve1Material_Maintenance_RecordByUidAPI(int Material_Maintenance_Record_UID, int Useruid)
        {
            return equipmentmaintenanceService.Approve1Material_Maintenance_RecordByUid(Material_Maintenance_Record_UID, Useruid);
        }
        public string Approve2Material_Maintenance_RecordAPI(Material_Maintenance_RecordDTO dto)
        {
            return equipmentmaintenanceService.Approve2Material_Maintenance_Record(dto);
        }
        [HttpGet]
        public string Approve3Material_Maintenance_RecordByUidAPI(int Material_Maintenance_Record_UID, int Useruid)
        {

            return equipmentmaintenanceService.Approve3Material_Maintenance_RecordByUid(Material_Maintenance_Record_UID, Useruid);

        }
        [HttpGet]
        public string DeleteMaterial_Maintenance_RecordAPI(int Material_Maintenance_Record_UID)
        {
            return equipmentmaintenanceService.DeleteMaterial_Maintenance_Record(Material_Maintenance_Record_UID);
        }

        [HttpPost]
        public IHttpActionResult DoAllExportMaterial_Maintenance_RecordAPI(Material_Maintenance_RecordDTO search)
        {
            var result = equipmentmaintenanceService.DoAllExportMaterial_Maintenance_Record(search);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult DoExportMaterial_Maintenance_RecordAPI(string Material_Maintenance_Record_UIDs)
        {
            var dto = Ok(equipmentmaintenanceService.DoExportMaterial_Maintenance_Record(Material_Maintenance_Record_UIDs));
            return dto;
        }
    }
}
