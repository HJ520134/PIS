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
    public class StorageManageController: ApiController
    {
        IStorageManageService storageManageService;
        IMaterialManageService materialManageService;
        public StorageManageController(IStorageManageService storageManageService, IMaterialManageService materialManageService)
        {
            this.storageManageService = storageManageService;
            this.materialManageService = materialManageService;
        }

        #region 仓库管理
        [HttpPost]
        public IHttpActionResult QueryWarehouseStoragesAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<WarehouseDTO>(jsondata);
            var result = storageManageService.QueryWarehouses(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportWarehouseReprotAPI(string uids)
        {
            var dto = Ok(storageManageService.DoExportWarehouseReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportWarehouseReprotAPI(WarehouseDTO search)
        {
            var result = storageManageService.DoAllExportWarehouseReprot(search);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetWarehouseStorageALLAPI(int Plant_UID)
        {
      
            var dto = Ok(storageManageService.GetWarehouseStorageALL(Plant_UID));
            return dto;
        }
        
        [HttpGet]
        public IHttpActionResult QueryOpTypesByUserAPI(int plantorguid)
        {
            var dto = storageManageService.QueryOpType(plantorguid);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult GetAllWarehouseAPI()
        {
            return Ok(storageManageService.GetAllWarehouse());
        }


        [HttpGet]
        public IHttpActionResult QueryWarehouseTypesAPI()
        {
            var dto = storageManageService.QueryWarehouseTypes();
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarehousesAPI()
        {
            var dto = storageManageService.QueryWarehouses();
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryFunplantByopAPI(int opuid)
        {
            var dto = storageManageService.QueryFunplantByop(opuid);
            return Ok(dto);
        }

        public string AddOrEditWarehouseInfoAPI(dynamic data)
        {
            var entity = JsonConvert.DeserializeObject<WarehouseStorages>(data.ToString());
            return storageManageService.AddOrEditWarehouseInfo(entity);
        }

        public string EditWarehouseStorageInfoAPI(WarehouseDTO dto)
        {
            return storageManageService.EditWarehouseStorageInfo(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarehouseStByuidAPI(int Warehouse_Storage_UID)
        {
            var dto = storageManageService.QuerySignleWarehouseSt(Warehouse_Storage_UID);

            return Ok(dto);
        }
        public IHttpActionResult GetAllWarehouseStAPI(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var dto = storageManageService.GetAllWarehouseSt(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);

            return Ok(dto);
        }
        [HttpPost]
        public string InsertWarehouse_StorageAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<Warehouse_StorageDTO>>(jsondata);
            return storageManageService.InsertWarehouse(list);
        }
        [HttpPost]
        public string InsertWarehouseAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<WarehouseBaseDTO>>(jsondata);
            return storageManageService.InsertWarehouseBase(list);
        }
        [HttpGet]
        public IHttpActionResult QueryWarehouseByuidAPI(int Warehouse_UID)
        {
            var war = storageManageService.QueryWar(Warehouse_UID)[0];
            var storages = storageManageService.QueryWarhouseSts(Warehouse_UID);
            var result = new WarehouseStorages
            {
                Plant_Organization_UID = war.Plant_Organization_UID,
                Plant = war.Plant,
                Warehouse_UID = war.Warehouse_UID,
                Warehouse_Type_UID = war.Warehouse_Type_UID,
                BG_Organization_UID = war.BG_Organization_UID,
                BG_Organization = war.BG_Organization,
                FunPlant_Organization_UID = war.FunPlant_Organization_UID,
                FunPlant_Organization = war.FunPlant_Organization,
                Warehouse_ID = war.Warehouse_ID,
                Warehouse_Type = war.Warehouse_Type,
                Name_ZH = war.Name_ZH,
                Name_EN = war.Name_EN,
                Desc = war.Desc,
                Storages = storages
            };
            return Ok(result);
        }

        [HttpGet]
        public string DeleteWarehouseStorageAPI(int WareStorage_UId)
        {
            return storageManageService.DeleteWarehouseStorage(WareStorage_UId);
        }

        [HttpGet]
        public string DeleteWarehouseAPI(int Warehouse_UID)
        {
            return storageManageService.DeleteWarehouse(Warehouse_UID);
        }
        #endregion 仓库管理

        #region  开账 
        [HttpPost]
        public IHttpActionResult QueryCreateBoundsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<StorageInboundDTO>(jsondata);
            var result = storageManageService.QueryCreateBounds(searchModel, page);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult DoExportCreateBoundReprotAPI(string uids)
        {
            var dto = Ok(storageManageService.DoExportCreateBoundReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportCreateBoundReprotAPI(StorageInboundDTO search)
        {
            var result = storageManageService.DoAllExportCreateBoundReprot(search);
            return Ok(result);
        }
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllWarehouseStAPI()
        {
            var result = storageManageService.QueryAllWarehouseSt();
            return Ok(result);
        }
        
        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryStorageEnumsAPI()
        {
            var result = storageManageService.QueryStorageEnums("StorageManage");
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryStorageInboundAPI()
        {
            var result = storageManageService.QueryStorageInbound();
            return Ok(result);
        }

        [HttpPost]
        public string InsertCreateBoundAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<StorageInboundDTO>>(jsondata);
            return storageManageService.InsertCreateBoundItem(list);
        }

        [HttpGet]
        public IHttpActionResult QueryWarehouseStAPI()
        {
            var dto = storageManageService.QueryWarehousesSt();
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult GetWarehouseSITEAPI(int planID)
        {
            var dto = storageManageService.GetWarehouseSITE(planID);
            return Ok(dto);
        }


        [HttpGet]
        public IHttpActionResult QueryWarStrogeAPI(int OPTypeID)
        {
            var dto = storageManageService.QueryWarStroge(OPTypeID);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryUsersAPI(int PlantID)
        {
            var dto = storageManageService.QueryUsers(PlantID);
            return Ok(dto);
        }

        public string AddOrEditCreateBoundAPI(StorageInboundDTO dto)
        {
            return storageManageService.AddOrEditCreateBound(dto);
        }

        [HttpGet]
        public string ApproveCreateboundByUidAPI(int Storage_Inbound_UID, int Useruid)
        {
            return storageManageService.ApproveCreatebound(Storage_Inbound_UID, Useruid);
        }

        [HttpGet]
        public string DeleteCreateBoundAPI(int Storage_Inbound_UID,int userid)
        {
            return storageManageService.DeleteCreateBound(Storage_Inbound_UID, userid);
        }
        [HttpPost]
        public IHttpActionResult ApproveCreateboundAPI(StorageInboundDTO search)
        {
            var result = storageManageService.ApproveCreateboundAll(search);
            return Ok(result);
        }
        #endregion  开账 

        #region 出入库 

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryAllMatInventoryAPI()
        {
            var result = storageManageService.QueryAllMatInventory();
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [IgnoreDBAuthorize]
        public IHttpActionResult QueryCreateBoundAPI()
        {
            var result = storageManageService.QueryCreateBound();
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult QueryBoundDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<InOutBoundInfoDTO>(jsondata);
            var result = storageManageService.QueryBoundDetails(searchModel, page);
            return Ok(result);
        }
        

        [HttpGet]
        public IHttpActionResult DoExportUpdateBoundReprotAPI(string uids)
        {
            var listBounds = JsonConvert.DeserializeObject<List<InOutBoundVM>>(uids).ToList();
            var dto = Ok(storageManageService.DoExportUpdateBoundReprot(listBounds));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportUpdateBoundReprotAPI(InOutBoundInfoDTO search)
        {
            var result = storageManageService.DoAllExportUpdateBoundReprot(search);
            return Ok(result);
        }
        public string AddOrEditInboundApplyAPI(StorageInboundDTO dto)
        {
            return storageManageService.AddInBoundApply(dto);
        }
        [HttpGet]
        public IHttpActionResult QueryInboundByUidAPI(int Storage_Bound_UID ,string Inout_Type)
        {
            if (Inout_Type == "入库单")
            {
                var dto = storageManageService.QueryInBoudSingle(Storage_Bound_UID);
                var result = new StorageInboundDTO
                {
                    Storage_Inbound_ID = dto[0].Storage_Inbound_ID,
                    Storage_Inbound_Type = dto[0].Storage_Inbound_Type,
                    PartType_UID = dto[0].PartType_UID,
                    PartType = dto[0].PartType,
                    Material_Id = dto[0].Material_Id,
                    Material_Name = dto[0].Material_Name,
                    Material_Types = dto[0].Material_Types,
                    Warehouse_ID = dto[0].Warehouse_ID,
                    Rack_ID = dto[0].Rack_ID,
                    Storage_ID = dto[0].Storage_ID,
                    Material_Uid = dto[0].Material_Uid,
                    PU_NO = dto[0].PU_NO,
                    PU_Qty = dto[0].PU_Qty,
                    Issue_NO = dto[0].Issue_NO,
                    Be_Check_Qty = dto[0].Be_Check_Qty,
                    OK_Qty = dto[0].OK_Qty,
                    NG_Qty = dto[0].NG_Qty,
                    Warehouse_Storage_UID = dto[0].Warehouse_Storage_UID,
                    Funplant = dto[0].Funplant,
                    Classification = dto[0].Classification,
                    Status = dto[0].Status,
                    Plant_UID= dto[0].Plant_UID,
                    BG_Organization_UID = dto[0].BG_Organization_UID,
                    FunPlant_Organization_UID = dto[0].FunPlant_Organization_UID,
                };
                return Ok(result);
            }
            else
            {
                var dto = storageManageService.QueryOutBoudSingle(Storage_Bound_UID);
                var details = storageManageService.QueryOutBondDs(Storage_Bound_UID);
                var result = new OutBoundInfo
                {
                    Storage_Outbound_ID = dto[0].Storage_Outbound_ID,
                    Storage_Outbound_Type_UID = dto[0].Storage_Outbound_Type_UID,
                    Storage_Outbound_Type = dto[0].Storage_Outbound_Type,
                    Repair_Uid = dto[0].Repair_Uid == null ? 0 : (int)dto[0].Repair_Uid,
                    Repair_id = dto[0].Repair_id,
                    Apply_Time = dto[0].Apply_Time,
                    EQP_Location = dto[0].EQP_Location,
                    Equipment = dto[0].Equipment,
                    OP_Types = dto[0].OP_Types,
                    FunPlant = dto[0].FunPlant,
                    Repair_Reason = dto[0].Repair_Reason,
                    Outbound_Account_UID = dto[0].Outbound_Account_UID,
                    Outbound_Account = dto[0].Outbound_Account,
                    Desc = dto[0].Desc,
                    Status = dto[0].Status,
                    details = details
                };
                return Ok(result);
            }
        }

        [HttpGet]
        public string ApproveInboundByUidAPI(int Storage_Inbound_UID,int Useruid)
        {
            return storageManageService.ApproveInbound(Storage_Inbound_UID, Useruid);
        }

        [HttpGet]
        public IHttpActionResult QueryWarStAPI( int inboundtype,int plantuid)
        {
            var dto = storageManageService.QueryWarSt( inboundtype, plantuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarStByKeyAPI(int inboundtype,int warStUid, string key, int plantuid)
        {
            var dto = storageManageService.QueryWarStByKey(inboundtype, warStUid, key, plantuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryFunplantByUserAPI(int userid)
        {
            var dto = storageManageService.QueryFunplantByUser(userid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryMatinventoryAPI(int Material_Uid, int Warehouse_Storage_UID)
        {
            var dto = storageManageService.QuerySingleMatinventory(Material_Uid, Warehouse_Storage_UID);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryMatinventoryByMatAPI(int Material_Uid)
        {
            var dto = storageManageService.QueryMatinventoryByMat(Material_Uid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryEqprepairAPI(string Repair_id)
        {
            var dto = storageManageService.QuerySingleEqprepair(Repair_id);
            return Ok(dto);
        }

        public string AddOrEditOutboundAPI(dynamic data)
        {
            var matentity = JsonConvert.DeserializeObject<OutBoundInfo>(data.ToString());
            return storageManageService.AddOrEditOutbound(matentity);
        }

        [HttpGet]
        public string DeleteBoundAPI(int Storage_Bound_UID, string Inout_Type,int UserUid)
        {
            return storageManageService.DeleteBound(Storage_Bound_UID, Inout_Type, UserUid);
        }

        [HttpGet]
        public string ApproveOutboundByUidAPI(int Storage_Outbound_M_UID, int Useruid)
        {
            return storageManageService.ApproveOutbound(Storage_Outbound_M_UID, Useruid);
        }

        [HttpGet]
        public IHttpActionResult QueryPuqtyAPI(string PUNO,int Storage_Inbound_UID)
        {
            var dto = storageManageService.QueryPuqty(PUNO, Storage_Inbound_UID);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarOpAPI(int PlantID)
        {
            var dto = storageManageService.QueryWarOps(PlantID);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarFunplantByopAPI(int opuid,int PartType_UID)
        {
            var dto = storageManageService.QueryWarFunplantByop(opuid, PartType_UID);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarIdByFunctionAPI(int Functionuid,int PartType_UID)
        {
            var dto = storageManageService.QueryWarIdByFunction(Functionuid, PartType_UID);
            return Ok(dto);
        }

        public string SaveWarehouseStAPI(WarehouseStorageDTO dto)
        {
            return storageManageService.SaveWarehouseSt(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryMatByOutTypeAPI(int Out_Type_Uid, int FunplantUid)
        {
            var dto = storageManageService.QueryMatByOutType(Out_Type_Uid, FunplantUid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryWarStByMaterialAPI(int Material_UID)
        {
            var dto = storageManageService.QueryWarStByMaterial(Material_UID);
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult GetWarStByMatCheckAPI(int Material_UID)
        {
            var dto = storageManageService.GetWarStByMatCheck(Material_UID);
            return Ok(dto);
        }
        
        public string ImportCoupaPO_To_InBoundApplyAPI(Newtonsoft.Json.Linq.JObject json)
        {
            string json_values = json.ToString();
            object dic = JsonConvert.DeserializeObject(json_values, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            var dtos = (Dictionary<int, StorageInboundDTO>)dic;
            Dictionary<int, string> row_errors = storageManageService.ImportCoupaPO_To_InBoundApply(dtos);
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
        #endregion 出入库    
        #region  盘点

        [HttpPost]
        public IHttpActionResult QueryStorageChecksAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<StorageCheckDTO>(jsondata);
            var result = storageManageService.QueryStorageChecks(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DoExportStorageCheckReprotAPI(string uids)
        {
            var dto = Ok(storageManageService.DoExportStorageCheckReprot(uids));
            return dto;
        }
        [HttpGet]
        public IHttpActionResult DownloadStorageCheckAPI(int PartType_UID, string Material_Id, string Material_Name, string Material_Types, string Warehouse_ID, string Rack_ID, string Storage_ID, int Plant_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var dto = Ok(storageManageService.DownloadStorageCheck(PartType_UID, Material_Id, Material_Name, Material_Types, Warehouse_ID, Rack_ID, Storage_ID, Plant_UID, BG_Organization_UID, FunPlant_Organization_UID));
            return dto;
        }
        

        [HttpPost]
        public IHttpActionResult DoAllExportStorageCheckReprotAPI(StorageCheckDTO search)
        {
            var result = storageManageService.DoAllExportStorageCheckReprot(search);
            return Ok(result);
        }
 
        [HttpPost]
        public string ImportStorageCheckAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<StorageCheckDTO>>(jsondata);
            return storageManageService.ImportStorageCheck(list);
        }

        public string AddOrEditStorageCheckAPI(StorageCheckDTO dto)
        {
            return storageManageService.AddOrEditStorageCheck(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryStorageCheckByUidAPI(int Storage_Check_UID)
        {
                var dto = storageManageService.QueryStorageCheckSingle(Storage_Check_UID);
                var result = new StorageCheckDTO
                {
                    Storage_Check_UID=dto[0].Storage_Check_UID,
                    PartType_UID = dto[0].PartType_UID,
                    PartType= dto[0].PartType,
                    Material_Uid = dto[0].Material_Uid,
                    Material_Id=dto[0].Material_Id,
                    Material_Name=dto[0].Material_Name,
                    Material_Types =dto[0].Material_Types,
                    Warehouse_Storage_UID= dto[0].Warehouse_Storage_UID,
                    Warehouse_ID=dto[0].Warehouse_ID,
                    Rack_ID=dto[0].Rack_ID,
                    Storage_ID=dto[0].Storage_ID,
                    Check_Qty = dto[0].Check_Qty,
                    Status=dto[0].Status 
                };
                return Ok(result);
            }
        [HttpGet]
        public string ApproveStCheckAPI(int Storage_Check_UID, int Useruid)
        {
            return storageManageService.ApproveStCheck(Storage_Check_UID, Useruid);
        }

        [HttpPost]
        public IHttpActionResult ApproveStorageCheckAPI(StorageCheckDTO search)
        {
            var result = storageManageService.ApproveStorageCheck(search);
            return Ok(result);
        }

        [HttpGet]
        public string DeleteStorageCheckAPI(int Storage_Check_UID, int userid)
        {
            return storageManageService.DeleteStorageCheck(Storage_Check_UID, userid);
        }

        #endregion  盘点  
        #region 出入库明细查询 
        [HttpGet]
        public IHttpActionResult DoExportStorageDetailReprotAPI(string uids)
        {
            var dto = Ok(storageManageService.DoExportStorageDetailReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportStorageDetailReprotAPI(StorageSearchMod search)
        {
            var result = storageManageService.DoAllExportStorageDetailReprot(search);
            return Ok(result);
        }


        [HttpPost]
        public IHttpActionResult QueryStorageDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<StorageSearchMod>(jsondata);
            var result = storageManageService.QueryStorageDetails(searchModel, page);
            return Ok(result);
        }

        #endregion 出入库明细查询
        #region 储位移转

        [HttpPost]
        public IHttpActionResult QueryStorageTransfersAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<StorageTransferDTO>(jsondata);
            var result = storageManageService.QueryStorageTransfers(searchModel, page);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult DoExportStorageTransferReprotAPI(string uids)
        {
            var dto = Ok(storageManageService.DoExportStorageTransferReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportStorageTransferReprotAPI(StorageTransferDTO search)
        {
            var result = storageManageService.DoAllExportStorageTransferReprot(search);
            return Ok(result);
        }
        public string AddOrEditStorageTransferAPI(StorageTransferDTO dto)
        {
            return storageManageService.AddOrEditStorageTransfer(dto);
        }
        public IHttpActionResult GetMaterial_InventoryAllAPI()
        {
            var dto = Ok(storageManageService.GetMaterial_InventoryAll());
            return dto;           
        }

        [HttpPost]
        public string ImportStorageTransferAPI(dynamic json)
        {
            var jsondata = json.ToString();
            var list = JsonConvert.DeserializeObject<IEnumerable<StorageTransferDTO>>(jsondata);
            return storageManageService.ImportStorageTransfer(list);
        }



        [HttpGet]
        public IHttpActionResult QueryStorageTransferByUidAPI(int Storage_Transfer_UID)
        {
            var dto = storageManageService.QueryStorageTransferSingle(Storage_Transfer_UID);
            var result = new StorageTransferDTO
            {
                Storage_Transfer_UID=dto[0].Storage_Transfer_UID,
                PartType_UID = dto[0].PartType_UID,
                PartType = dto[0].PartType,
                Material_Uid=dto[0].Material_Uid,
                Out_Warehouse_Storage_UID=dto[0].Out_Warehouse_Storage_UID,
                In_Warehouse_Storage_UID=dto[0].In_Warehouse_Storage_UID,
                Transfer_Qty = dto[0].Transfer_Qty,
                Material_Id= dto[0].Material_Id,
                Material_Name= dto[0].Material_Name,
                Material_Types= dto[0].Material_Types,
                Out_Warehouse_ID= dto[0].Out_Warehouse_ID,
                Out_Rack_ID= dto[0].Out_Rack_ID,
                Out_Storage_ID= dto[0].Out_Storage_ID,
                In_Warehouse_ID= dto[0].In_Warehouse_ID,
                In_Rack_ID= dto[0].In_Rack_ID,
                In_Storage_ID= dto[0].In_Storage_ID,
                Transfer_Type=dto[0].Transfer_Type,
                Status = dto[0].Status
            };
            return Ok(result);
        }

        [HttpGet]
        public string ApproveStTransferAPI(int Storage_Transfer_UID, int Useruid)
        {
            return storageManageService.ApproveStTransfer(Storage_Transfer_UID, Useruid);
        }

        [HttpGet]
        public string DeleteStorageTransferAPI(int Storage_Transfer_UID, int userid)
        {
            return storageManageService.DeleteStorageTransfer(Storage_Transfer_UID, userid);
        }

        #endregion 储位移转     
        #region 配置信息维护
        [HttpPost]
        public IHttpActionResult QueryEQPPowerOnsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPPowerOnDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var users = materialManageService.QueryEQPPowerOns(searchModel, page);
            return Ok(users);
        }

        [HttpPost]
        public IHttpActionResult QueryEQPMaterialsAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPMaterialDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var users = materialManageService.QueryEQPMaterials(searchModel, page);
            return Ok(users);
        }
        [HttpPost]
        public IHttpActionResult QueryEQPTypesAPI(dynamic data)
        {
            var jsonData = data.ToString();
            var searchModel = JsonConvert.DeserializeObject<EQPTypeDTO>(jsonData);
            var page = JsonConvert.DeserializeObject<Page>(jsonData);

            var users = materialManageService.QueryEQPTypes(searchModel, page);
            return Ok(users);
        }
        #endregion  配置信息维护
        #region 一般需求计算

        [HttpPost]
        public IHttpActionResult QueryMaterialNormalDemandsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialNormalDemandDTO>(jsondata);
            var result = storageManageService.QueryMaterialNormalDemands(searchModel, page);
            return Ok(result);
        }

        public string AddMatNormalDemandAPI(MaterialNormalDemandDTO dto)
        {
            return storageManageService.AddMatNormalDemand(dto);
        }

        [HttpPost]
        public IHttpActionResult QueryMatNormalDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialNormalDemandDTO>(jsondata);
            var result = storageManageService.QueryMaterialNormalDetails(searchModel, page);
            return Ok(result);
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult EditUserAdjustQtyAPI(matNDVM dto)
        {
            return Ok(storageManageService.EditUserAdjustQty(dto));
        }

        [HttpGet]
        public string ApproveMatNDAPI(int Material_Normal_Demand_UID, int userid)
        {
            return storageManageService.ApproveMatND(Material_Normal_Demand_UID, userid);
        }

        [HttpGet]
        public string DisapproveMatNDAPI(int Material_Normal_Demand_UID, int userid)
        {
            return storageManageService.DisapproveMatND(Material_Normal_Demand_UID, userid);
        }

        [HttpGet]
        public IHttpActionResult DoExportFunctionAPI(int Material_Normal_Demand_UID)
        {
            var dto = storageManageService.DoExportFunction(Material_Normal_Demand_UID);
            return Ok(dto);
        }

        [HttpGet]
        public string DeleteMatNormalDemandListAPI(int Material_Normal_Demand_UID, int userid)
        {
            return storageManageService.DeleteMatNormalDemandList(Material_Normal_Demand_UID, userid);
        }

        [HttpGet]
        public string DeleteMatNormalDemandAPI(int Material_Normal_Demand_UID, int userid)
        {
            return storageManageService.DeleteMatNormalDemand(Material_Normal_Demand_UID, userid);
        }

        /// <summary>
        /// 更新一般需求状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="statusUID"></param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdateMaterialNormalDemandStatusAPI(int uid, int statusUID)
        {
            var result = storageManageService.UpdateMaterialNormalDemandStatus(uid, statusUID);
            return result;
        }

        #endregion 一般需求计算      
        #region 库存明细查询

        [HttpGet]
        public IHttpActionResult DoExportMaterialInventoryReprotAPI(string uids)
        {
            var dto = Ok(storageManageService.DoExportMaterialInventoryReprot(uids));
            return dto;
        }
        [HttpPost]
        public IHttpActionResult DoAllExportMaterialInventoryReprotAPI(MaterialInventoryDTO search)
        {
            var result = storageManageService.DoAllExportMaterialInventoryReprot(search);
            return Ok(result);
        }


        public IHttpActionResult QueryMaterialInventorySumAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialInventoryDTO>(jsondata);
            var result = storageManageService.QueryMaterialInventorySum(searchModel, page);
            return Ok(result);
        }

        public IHttpActionResult QueryMaterialInventoryDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialInventoryDTO>(jsondata);
            var result = storageManageService.QueryMaterialInventoryDetails(searchModel, page);
            return Ok(result);
        }

        #endregion 库存明细查询  
        #region 备品需求计算

        [HttpPost]
        public IHttpActionResult QueryMatSparepartsDemandsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialSparepartsDemandDTO>(jsondata);
            var result = storageManageService.QueryMatSparepartsDemands(searchModel, page);
            return Ok(result);
        }

        public string AddMatSparepartsDemandAPI(MaterialSparepartsDemandDTO dto)
        {
            return storageManageService.AddMatSparepartslDemand(dto);
        }

        public IHttpActionResult QueryMatSDDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialSparepartsDemandDTO>(jsondata);
            var result = storageManageService.QueryMatSDDetails(searchModel, page);
            return Ok(result);
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult EditSDUserAdjustQtyAPI(matSDVM dto)
        {
            return Ok(storageManageService.EditSDUserAdjustQty(dto));
        }

        [HttpGet]
        public string ApproveMatSDAPI(int Material_Spareparts_Demand_UID, int userid)
        {
            return storageManageService.ApproveMatSD(Material_Spareparts_Demand_UID, userid);
        }

        [HttpGet]
        public string DisapproveMatSDAPI(int Material_Spareparts_Demand_UID, int userid)
        {
            return storageManageService.DisapproveMatSD(Material_Spareparts_Demand_UID, userid);
        }

        [HttpGet]
        public IHttpActionResult DoSDExportFunctionAPI(int Material_Spareparts_Demand_UID)
        {
            var dto = storageManageService.DoSDExportFunction(Material_Spareparts_Demand_UID);
            return Ok(dto);
        }

        [HttpGet]
        public string DeleteMatSparepartsDemandListAPI(int Material_Spareparts_Demand_UID, int userid)
        {
            return storageManageService.DeleteMatSparepartsDemandList(Material_Spareparts_Demand_UID, userid);
        }

        [HttpGet]
        public string DeleteMatSDAPI(int Material_Spareparts_Demand_UID, int userid)
        {
            return storageManageService.DeleteMatSD(Material_Spareparts_Demand_UID, userid);
        }

        /// <summary>
        /// 更新备品需求状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="statusUID"></param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdateMaterialSparepartsDemandStatusAPI(int uid, int statusUID)
        {
            var result = storageManageService.UpdateMaterialSparepartsDemandStatus(uid, statusUID);
            return result;
        }
        #endregion 备品需求计算    AddMatSparepartsDemandAPI
        #region 返修品需求计算

        [HttpPost]
        public IHttpActionResult QueryMatRepairDemandsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialRepairDemandDTO>(jsondata);
            var result = storageManageService.QueryMatRepairDemands(searchModel, page);
            return Ok(result);
        }

        public string AddMatRepairDemandAPI(MaterialRepairDemandDTO dto)
        {
            return storageManageService.AddMatRepairDemand(dto);
        }

        public IHttpActionResult QueryMatRDDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialRepairDemandDTO>(jsondata);
            var result = storageManageService.QueryMatRDDetails(searchModel, page);
            return Ok(result);
        }

        [AcceptVerbs("Post")]
        public IHttpActionResult EditRDUserAdjustQtyAPI(matRDVM dto)
        {
            return Ok(storageManageService.EditRDUserAdjustQty(dto));
        }

        [HttpGet]
        public string ApproveMatRDAPI(int Material_Repair_Demand_UID, int userid)
        {
            return storageManageService.ApproveMatRD(Material_Repair_Demand_UID, userid);
        }

        [HttpGet]
        public string DisapproveMatRDAPI(int Material_Repair_Demand_UID, int userid)
        {
            return storageManageService.DisapproveMatRD(Material_Repair_Demand_UID, userid);
        }

        [HttpGet]
        public IHttpActionResult DoRDExportFunctionAPI(int Material_Repair_Demand_UID)
        {
            var dto = storageManageService.DoRDExportFunction(Material_Repair_Demand_UID);
            return Ok(dto);
        }

        [HttpGet]
        public string DeleteMatRepairDemandListAPI(int Material_Repair_Demand_UID, int userid)
        {
            return storageManageService.DeleteMatRepairDemandList(Material_Repair_Demand_UID, userid);
        }

        [HttpGet]
        public string DeleteMatRDAPI(int Material_Repair_Demand_UID, int userid)
        {
            return storageManageService.DeleteMatRD(Material_Repair_Demand_UID, userid);
        }


        /// <summary>
        /// 更新返修品需求状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="statusUID"></param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdateMaterialRepairDemandStatusAPI(int uid, int statusUID)
        {
            var result = storageManageService.UpdateMaterialRepairDemandStatus(uid, statusUID);
            return result;
        }
        #endregion 返修品需求计算    
        #region 采购需求汇总

        [HttpPost]
        public IHttpActionResult QueryMatDemandSummaryAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialDemandSummaryDTO>(jsondata);
            var result = storageManageService.QueryMatDemandSummary(searchModel, page);
            return Ok(result);
        }

        public string AddMatDemandSummaryAPI(MaterialDemandSummaryDTO dto)
        {
            return storageManageService.AddMatDemandSummary(dto);
        }

        public IHttpActionResult QueryMatDSDetailsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<MaterialDemandSummaryDTO>(jsondata);
            var result = storageManageService.QueryMatDSDetails(searchModel, page);
            return Ok(result);
        }


        [HttpGet]
        public string SubmitMatDemandSummaryAPI(int Material_Demand_Summary_UID, int userid)
        {
            return storageManageService.SubmitMatDemandSummary(Material_Demand_Summary_UID, userid);
        }

        [HttpGet]
        public string PurchaseMatDSAPI(int Material_Demand_Summary_UID, int userid)
        {
            return storageManageService.PurchaseMatDS(Material_Demand_Summary_UID, userid);
        }

        [HttpGet]
        public IHttpActionResult DoDSExportFunctionAPI(int Material_Demand_Summary_UID)
        {
            var dto = storageManageService.DoDSExportFunction(Material_Demand_Summary_UID);
            return Ok(dto);
        }

        [HttpGet]
        public string DeleteMatDemandSummaryListAPI(int Material_Demand_Summary_UID, int userid)
        {
            return storageManageService.DeleteMatDemandSummaryList(Material_Demand_Summary_UID, userid);
        }

        [HttpGet]
        public string DeleteMatDSAPI(int Material_Demand_Summary_UID, int userid)
        {
            return storageManageService.DeleteMatDS(Material_Demand_Summary_UID, userid);
        }

        [HttpGet]
        public string DisdeleteMatDSAPI(int Material_Demand_Summary_UID, int userid)
        {
            return storageManageService.DisdeleteMatDS(Material_Demand_Summary_UID, userid);
        }

        [HttpGet]
        public string DisPurchaseMatDSAPI(int Material_Demand_Summary_UID, int userid)
        {
            return storageManageService.DisPurchaseMatDS(Material_Demand_Summary_UID, userid);
        }
        /// <summary>
        /// 更新采购需求汇总状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="statusUID"></param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdateMaterialDemandSummaryStatusAPI(int uid, int statusUID)
        {
            var result = storageManageService.UpdateMaterialDemandSummaryStatus(uid, statusUID);
            return result;
        }
        #endregion 采购需求汇总 
        #region 库存报表

        [HttpPost]
        public IHttpActionResult QueryStorageReportsAPI(dynamic data)
        {
            var jsondata = data.ToString();
            var page = JsonConvert.DeserializeObject<Page>(jsondata);
            var searchModel = JsonConvert.DeserializeObject<StorageReportDTO>(jsondata);
            var result = storageManageService.QueryStorageReports(searchModel, page);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult QueryPlantsAPI(int plantorguid)
        {
            var dto = storageManageService.QueryPlants(plantorguid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult QueryBGByPlantAPI(int plantuid)
        {
            var dto = storageManageService.QueryBGByPlant(plantuid);
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult DoSRExportFunctionAPI(int plant, int bg, int funplant, string material, DateTime start_date, DateTime end_date)
        {
            var dto = storageManageService.DoSRExportFunction(plant,bg,funplant,material,start_date,end_date);
            return Ok(dto);
        }

        #endregion 库存报表  
    }
}