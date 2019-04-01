using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using PDMS.Model.EntityDTO;
using PDMS.Data;
using System.Text;
using PDMS.Common.Constants;

namespace PDMS.Service
{
    public interface IStorageManageService
    {
        #region 储位管理
        PagedListModel<WarehouseDTO> QueryWarehouses(WarehouseDTO searchModel, Page page);
        List<WarehouseDTO> DoExportWarehouseReprot(string uids);
        List<WarehouseDTO> DoAllExportWarehouseReprot(WarehouseDTO searchModel);

        List<WarehouseDTO> GetWarehouseStorageALL(int Plant_UID);
        List<SystemOrgDTO> QueryOpType(int plantorguid);
        List<EnumerationDTO> QueryWarehouseTypes();
        List<WarehouseBaseDTO> GetAllWarehouse();
      string InsertWarehouse(List<Warehouse_StorageDTO> dtolist);
        string InsertWarehouseBase(List<WarehouseBaseDTO> dtolist);
        List<WarehouseDTO> QueryWarehouses();
        List<SystemOrgDTO> QueryFunplantByop(int opuid);

        string AddOrEditWarehouseInfo(WarehouseStorages dto);
        string EditWarehouseStorageInfo(WarehouseDTO dto);
        List<WarehouseDTO> QuerySignleWarehouseSt(int Warehouse_Storage_UID);
        string DeleteWarehouseStorage(int WareStorage_UId);
        string DeleteWarehouse(int Warehouse_UID);
        List<WarehouseDTO> QueryWar(int Warehouse_UID);
        List<WarStorageDto> QueryWarhouseSts(int Warehouse_UID);
        #endregion 储位管理

        #region 开账
        PagedListModel<StorageInboundDTO> QueryCreateBounds(StorageInboundDTO searchModel, Page page);
        List<StorageInboundDTO> DoExportCreateBoundReprot(string uids);
        List<StorageInboundDTO> DoAllExportCreateBoundReprot(StorageInboundDTO search);

        List<WarehouseStorageDTO> QueryAllWarehouseSt();
        List<WarehouseStorageDTO> GetAllWarehouseSt(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<EnumerationDTO> QueryStorageEnums(string Enum_Type);
        List<StorageInboundDTO> QueryStorageInbound();
        string InsertCreateBoundItem(List<StorageInboundDTO> dtolist);
        List<WarehouseDTO> QueryWarehousesSt();
        List<WarehouseDTO> GetWarehouseSITE(int planID);        
        List<WarehouseDTO> QueryWarStroge(int OPTypeID);
        string AddOrEditCreateBound(StorageInboundDTO dto);
        string ApproveCreatebound(int Storage_Inbound_UID, int Useruid);
        string ApproveCreateboundAll(StorageInboundDTO dto);
        string DeleteCreateBound(int Storage_Inbound_UID, int userid);
        #endregion 开账

        #region 出入库
        List<InOutBoundInfoDTO> DoExportUpdateBoundReprot(List<InOutBoundVM> uids);
        List<InOutBoundInfoDTO> DoAllExportUpdateBoundReprot(InOutBoundInfoDTO search);
        List<MaterialInventoryDTO> QueryAllMatInventory();
        List<StorageInboundDTO> QueryCreateBound();
        PagedListModel<InOutBoundInfoDTO> QueryBoundDetails(InOutBoundInfoDTO searchModel, Page page);
        string AddInBoundApply(StorageInboundDTO dto);
        List<StorageInboundDTO> QueryInBoudSingle(int Storage_Inbound_UID);
        string ApproveInbound(int Storage_Inbound_UID, int Useruid);
        List<StorageInboundDTO> QueryWarSt(int inboundtype, int plantuid);
        List<StorageInboundDTO> QueryWarStByKey(int inboundtype,int warStUid, string key, int plantuid);
        List<string> QueryFunplantByUser(int userid);
        List<EQPUserTableDTO> QueryUsers(int PlantID);
        List<MaterialInventoryDTO> QuerySingleMatinventory(int Material_Uid, int Warehouse_Storage_UID);
        List<MaterialInventoryDTO> QueryMatinventoryByMat(int Material_Uid);
        List<EQPRepairInfoDTO> QuerySingleEqprepair(string Repair_id);
        string AddOrEditOutbound(OutBoundInfo dto);
        string DeleteBound(int Storage_UID, string Inout_Type, int UserUid);
        List<StorageOutboundDTO> QueryOutBoudSingle(int Storage_Outbound_UID);
        List<OutBoundDetail> QueryOutBondDs(int Storage_Bound_UID);
        List<OutBoundInfoDTO> GetOutBoundInfoDTOALL();
        string ApproveOutbound(int Storage_Outbound_M_UID, int Useruid);
        List<StorageInboundDTO> QueryPuqty(string PUNO, int Storage_Inbound_UID);
        List<SystemOrgDTO> QueryWarOps(int PlantID);
        List<SystemOrgDTO> QueryWarFunplantByop(int opuid, int PartType_UID);
        List<WarehouseDTO> QueryWarIdByFunction(int Functionuid, int PartType_UID);
        string SaveWarehouseSt(WarehouseStorageDTO dto);
        List<MaterialInventoryDTO> QueryMatByOutType(int Out_Type_Uid, int FunplantUid);
        List<MaterialInventoryDTO> QueryWarStByMaterial(int Material_UID);
        List<MaterialInventoryDTO> GetWarStByMatCheck(int Material_UID);
        Dictionary<int, string> ImportCoupaPO_To_InBoundApply(Dictionary<int, StorageInboundDTO> dics);
        #endregion 出入库 

        #region 盘点
        PagedListModel<StorageCheckDTO> QueryStorageChecks(StorageCheckDTO searchModel, Page page);
        List<StorageCheckDTO> DoAllExportStorageCheckReprot(StorageCheckDTO searchModel);
        List<StorageCheckDTO> DownloadStorageCheck(int PartType_UID, string Material_Id, string Material_Name, string Material_Types, string Warehouse_ID, string Rack_ID, string Storage_ID, int Plant_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        string ImportStorageCheck(List<StorageCheckDTO> dtolist);
        List<StorageCheckDTO> DoExportStorageCheckReprot(string uids);
        string AddOrEditStorageCheck(StorageCheckDTO dto);
        List<StorageCheckDTO> QueryStorageCheckSingle(int Storage_Check_UID);
        string ApproveStCheck(int Storage_Check_UID, int Useruid);
        string ApproveStorageCheck(StorageCheckDTO dto);
        string DeleteStorageCheck(int Storage_Check_UID, int userid);
        #endregion 盘点    

        #region 出入库明细查询 
        List<StorageDetailDTO> DoExportStorageDetailReprot(string uids);
        List<StorageDetailDTO> DoAllExportStorageDetailReprot(StorageSearchMod search);


        PagedListModel<StorageDetailDTO> QueryStorageDetails(StorageSearchMod searchModel, Page page);
        #endregion 出入库明细查询

        #region 储位移转
        PagedListModel<StorageTransferDTO> QueryStorageTransfers(StorageTransferDTO searchModel, Page page);
        string AddOrEditStorageTransfer(StorageTransferDTO dto);
        string ImportStorageTransfer(List<StorageTransferDTO> dtolist);
        List<Material_Inventory> GetMaterial_InventoryAll();
        List<StorageTransferDTO> QueryStorageTransferSingle(int Storage_Transfer_UID);
        List<StorageTransferDTO> DoExportStorageTransferReprot(string uids);
        List<StorageTransferDTO> DoAllExportStorageTransferReprot(StorageTransferDTO search);

        string ApproveStTransfer(int Storage_Transfer_UID, int Useruid);
        string DeleteStorageTransfer(int Storage_Check_UID, int userid);
        #endregion 储位移转   

        #region 一般需求计算
        PagedListModel<MaterialNormalDemandDTO> QueryMaterialNormalDemands(MaterialNormalDemandDTO searchModel, Page page);
        string AddMatNormalDemand(MaterialNormalDemandDTO dto);
        PagedListModel<MaterialNormalDemandDTO> QueryMaterialNormalDetails(MaterialNormalDemandDTO searchModel, Page page);
        string EditUserAdjustQty(matNDVM dto);
        string ApproveMatND(int Material_Normal_Demand_UID, int userid);
        string DisapproveMatND(int Material_Normal_Demand_UID, int userid);
        List<MaterialNormalDemandDTO> DoExportFunction(int Material_Normal_Demand_UID);
        string DeleteMatNormalDemandList(int Material_Normal_Demand_UID, int userid);
        string DeleteMatNormalDemand(int Material_Normal_Demand_UID, int userid);
        bool UpdateMaterialNormalDemandStatus(int uid, int statusUID);
        #endregion 一般需求计算  
        #region 库存明细查询

        List<MaterialInventoryDTO> DoExportMaterialInventoryReprot(string uids);
        List<MaterialInventoryDTO> DoAllExportMaterialInventoryReprot(MaterialInventoryDTO searchModel);

        PagedListModel<MaterialInventoryDTO> QueryMaterialInventorySum(MaterialInventoryDTO searchModel, Page page);
        PagedListModel<MaterialInventoryDTO> QueryMaterialInventoryDetails(MaterialInventoryDTO searchModel, Page page);
        #endregion 库存明细查询 
        #region 备品需求计算
        PagedListModel<MaterialSparepartsDemandDTO> QueryMatSparepartsDemands(MaterialSparepartsDemandDTO searchModel, Page page);
        string AddMatSparepartslDemand(MaterialSparepartsDemandDTO dto);
        PagedListModel<MaterialSparepartsDemandDTO> QueryMatSDDetails(MaterialSparepartsDemandDTO searchModel, Page page);
        string EditSDUserAdjustQty(matSDVM dto);
        string ApproveMatSD(int Material_Spareparts_Demand_UID, int userid);
        string DisapproveMatSD(int Material_Normal_Demand_UID, int userid);
        List<MaterialSparepartsDemandDTO> DoSDExportFunction(int Material_Spareparts_Demand_UID);
        string DeleteMatSparepartsDemandList(int Material_Spareparts_Demand_UID, int userid);
        string DeleteMatSD(int Material_Spareparts_Demand_UID, int userid);
        bool UpdateMaterialSparepartsDemandStatus(int uid, int statusUID);
        #endregion 备品需求计算    

        #region 返修品需求计算
        PagedListModel<MaterialRepairDemandDTO> QueryMatRepairDemands(MaterialRepairDemandDTO searchModel, Page page);
        string AddMatRepairDemand(MaterialRepairDemandDTO dto);
        PagedListModel<MaterialRepairDemandDTO> QueryMatRDDetails(MaterialRepairDemandDTO searchModel, Page page);
        string EditRDUserAdjustQty(matRDVM dto);
        string ApproveMatRD(int Material_Repair_Demand_UID, int userid);
        string DisapproveMatRD(int Material_Repair_Demand_UID, int userid);
        List<MaterialRepairDemandDTO> DoRDExportFunction(int Material_Repair_Demand_UID);
        string DeleteMatRepairDemandList(int Material_Repair_Demand_UID, int userid);
        string DeleteMatRD(int Material_Repair_Demand_UID, int userid);
        bool UpdateMaterialRepairDemandStatus(int uid, int statusUID);
        #endregion 返修品需求计算    
        #region 采购需求汇总
        PagedListModel<MaterialDemandSummaryDTO> QueryMatDemandSummary(MaterialDemandSummaryDTO searchModel, Page page);
        string AddMatDemandSummary(MaterialDemandSummaryDTO dto);
        PagedListModel<MaterialDemandSummaryDTO> QueryMatDSDetails(MaterialDemandSummaryDTO searchModel, Page page);
        string SubmitMatDemandSummary(int Material_Demand_Summary_UID, int userid);
        string PurchaseMatDS(int Material_Demand_Summary_UID, int userid);
        List<MaterialDemandSummaryDTO> DoDSExportFunction(int Material_Demand_Summary_UID);
        string DeleteMatDemandSummaryList(int Material_Demand_Summary_UID, int userid);
        string DeleteMatDS(int Material_Demand_Summary_UID, int userid);
        string DisdeleteMatDS(int Material_Demand_Summary_UID, int userid);
        string DisPurchaseMatDS(int Material_Demand_Summary_UID, int userid);
        bool UpdateMaterialDemandSummaryStatus(int uid, int statusUID);
        #endregion 采购需求汇总
        #region 库存报表
        PagedListModel<StorageReportDTO> QueryStorageReports(StorageReportDTO searchModel, Page page);
        List<SystemOrgDTO> QueryPlants(int plantorguid);
        List<SystemOrgDTO> QueryBGByPlant(int opuid);
        List<StorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string material, DateTime start_date, DateTime end_date);
        #endregion 库存报表 
    }
    public class StorageManageService : IStorageManageService
    {
        private readonly IWarehouseRepository warehouseRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWarehouseStorageRepository warehouseStorageRepository;
        private readonly IStorageInboundRepository storageInboundRepository;
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IMaterialInventoryRepository materialInventoryRepository;
        private readonly ISystemUserRepository systemUserRepository;
        private readonly IStorageInOutDetailRepository storageInOutDetailRepository;
        private readonly IEQPRepairInfoRepository eQPRepairInfoRepository;
        private readonly IStorageOutboundMRepository storageOutboundMRepository;
        private readonly IStorageOutboundDRepository storageOutboundDRepository;
        private readonly IEQPUserTableRepository eQPUserTableRepository;
        private readonly IStorageCheckRepository storageCheckRepository;
        private readonly IStorageTransferRepository storageTransferRepository;
        private readonly IMaterialNormalDemandRepository materialNormalDemandRepository;
        private readonly IEQP_MaterialRepository eQP_MaterialRepository;
        private readonly IEQP_TypeRepository eQP_TypeRepository;
        private readonly IMaterialInfoRepository materialInfoRepository;
        private readonly ISystemOrgRepository systemOrgRepository;
        private readonly IMaterialSparepartsDemandRepository materialSparepartsDemandRepository;
        private readonly IEQPForecastPowerOnRepository eQPForecastPowerOnRepository;
        private readonly IMaterialRepairDemandRepository materialRepairDemandRepository;
        private readonly IEQP_PowerOnRepository eQP_PowerOnRepository;
        private readonly IMaterialDemandSummaryRepository materialDemandSummaryRepository;
        private readonly IMeterialUpdateInfoRepository meterialUpdateInfoRepository;
        public StorageManageService(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork
            , IWarehouseStorageRepository warehouseStorageRepository
            , IStorageInboundRepository storageInboundRepository
            , IEnumerationRepository enumerationRepository
            , IMaterialInventoryRepository materialInventoryRepository
            , ISystemUserRepository systemUserRepository
            , IStorageInOutDetailRepository storageInOutDetailRepository
            , IEQPRepairInfoRepository eQPRepairInfoRepository
            , IStorageOutboundMRepository storageOutboundMRepository
            , IStorageOutboundDRepository storageOutboundDRepository
            , IEQPUserTableRepository eQPUserTableRepository
            , IStorageCheckRepository storageCheckRepository
            , IStorageTransferRepository storageTransferRepository
            , IMaterialNormalDemandRepository materialNormalDemandRepository
            , IEQP_MaterialRepository eQP_MaterialRepository
            , IEQP_TypeRepository eQP_TypeRepository
            , IMaterialInfoRepository materialInfoRepository
            , ISystemOrgRepository systemOrgRepository
            , IMaterialSparepartsDemandRepository materialSparepartsDemandRepository
            , IEQPForecastPowerOnRepository eQPForecastPowerOnRepository
            , IMaterialRepairDemandRepository materialRepairDemandRepository
            , IEQP_PowerOnRepository eQP_PowerOnRepository
            , IMaterialDemandSummaryRepository materialDemandSummaryRepository
            , IMeterialUpdateInfoRepository meterialUpdateInfoRepository)
        {
            this.warehouseRepository = warehouseRepository;
            this.unitOfWork = unitOfWork;
            this.warehouseStorageRepository = warehouseStorageRepository;
            this.storageInboundRepository = storageInboundRepository;
            this.enumerationRepository = enumerationRepository;
            this.materialInventoryRepository = materialInventoryRepository;
            this.systemUserRepository = systemUserRepository;
            this.storageInOutDetailRepository = storageInOutDetailRepository;
            this.eQPRepairInfoRepository = eQPRepairInfoRepository;
            this.storageOutboundMRepository = storageOutboundMRepository;
            this.storageOutboundDRepository = storageOutboundDRepository;
            this.eQPUserTableRepository = eQPUserTableRepository;
            this.storageCheckRepository = storageCheckRepository;
            this.storageTransferRepository = storageTransferRepository;
            this.materialNormalDemandRepository = materialNormalDemandRepository;
            this.eQP_MaterialRepository = eQP_MaterialRepository;
            this.eQP_TypeRepository = eQP_TypeRepository;
            this.materialInfoRepository = materialInfoRepository;
            this.systemOrgRepository = systemOrgRepository;
            this.materialSparepartsDemandRepository = materialSparepartsDemandRepository;
            this.eQPForecastPowerOnRepository = eQPForecastPowerOnRepository;
            this.materialRepairDemandRepository = materialRepairDemandRepository;
            this.eQP_PowerOnRepository = eQP_PowerOnRepository;
            this.materialDemandSummaryRepository = materialDemandSummaryRepository;
            this.meterialUpdateInfoRepository = meterialUpdateInfoRepository;
        }
        #region 储位管理
        public PagedListModel<WarehouseDTO> QueryWarehouses(WarehouseDTO searchModel, Page page)
        {
            int totalcount;
            var result = warehouseRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<WarehouseDTO>(totalcount, result);
            return bd;
        }

        public List<WarehouseDTO> DoExportWarehouseReprot(string uids)
        {
            return warehouseRepository.DoExportWarehouseReprot(uids);
        }
        public List<WarehouseDTO> DoAllExportWarehouseReprot(WarehouseDTO searchModel)
        {
            return warehouseRepository.DoAllExportWarehouseReprot(searchModel);
        }
        public List<WarehouseDTO> GetWarehouseStorageALL(int Plant_UID)
        {
            return warehouseRepository.GetWarehouseStorageALL( Plant_UID);
        }
        public List<SystemOrgDTO> QueryOpType(int plantorguid)
        {
            var bud = warehouseRepository.GetOpType(plantorguid).ToList();
            return bud;
        }
        public List<EnumerationDTO> QueryWarehouseTypes()
        {
            var bud = warehouseRepository.GetWarehouseType().ToList();
            return bud;
        }
        public List<WarehouseBaseDTO> GetAllWarehouse()
        {
            var bud = warehouseRepository.GetWarehouse().ToList();
            return bud;
        }
        public string InsertWarehouse(List<Warehouse_StorageDTO> dtolist)
        {
            return warehouseRepository.InsertWarehouseStorageItem(dtolist);
        }

        public string InsertWarehouseBase(List<WarehouseBaseDTO> dtolist)
        {
            return warehouseRepository.InsertWarehouseStorage(dtolist);
        }


        public List<WarehouseDTO> QueryWarehouses()
        {
            var bud = warehouseRepository.GetAll().ToList();
            List<WarehouseDTO> dtoList = new List<WarehouseDTO>();
            foreach (var item in bud)
            {
                dtoList.Add(AutoMapper.Mapper.Map<WarehouseDTO>(item));
            }
            return dtoList;
        }
        public List<SystemOrgDTO> QueryFunplantByop(int opuid)
        {
            var bud = warehouseRepository.GetFunplantsByop(opuid).ToList();
            return bud;
        }

        public string AddOrEditWarehouseInfo(WarehouseStorages dto)
        {
            try
            {
                var warehouse = warehouseRepository.GetById(dto.Warehouse_UID);
                if (warehouse != null)
                {
                    var ware = warehouseRepository.GetById(dto.Warehouse_UID);
                    ware.BG_Organization_UID = dto.BG_Organization_UID;
                    ware.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    ware.Warehouse_ID = dto.Warehouse_ID;
                    ware.Warehouse_Type_UID = dto.Warehouse_Type_UID;
                    ware.Name_ZH = dto.Name_ZH;
                    ware.Name_EN = dto.Name_EN;
                    ware.Desc = dto.Desc;
                    ware.Modified_UID = dto.Modified_UID;
                    ware.Modified_Date = DateTime.Now;
                    warehouseRepository.Update(ware);
                    unitOfWork.Commit();
                }
                else
                {
                    Warehouse ware = new Warehouse();
                    ware.BG_Organization_UID = dto.BG_Organization_UID;
                    ware.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    ware.Warehouse_ID = dto.Warehouse_ID;
                    ware.Warehouse_Type_UID = dto.Warehouse_Type_UID;
                    ware.Name_ZH = dto.Name_ZH;
                    ware.Name_EN = dto.Name_EN;
                    ware.Desc = dto.Desc;
                    ware.Modified_UID = dto.Modified_UID;
                    ware.Modified_Date = DateTime.Now;
                    warehouseRepository.Add(ware);
                    unitOfWork.Commit();
                }
                foreach (var item in dto.Storages)
                {
                    if (dto.Warehouse_UID == 0)
                    {
                        var war = warehouseRepository.GetFirstOrDefault(m => m.BG_Organization_UID == dto.BG_Organization_UID &
                                            m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID & m.Warehouse_ID == dto.Warehouse_ID);
                        dto.Warehouse_UID = war.Warehouse_UID;
                    }
                    if (item.Warehouse_Storage_UID == 0)
                    {
                        Warehouse_Storage ws = new Warehouse_Storage();
                        ws.Warehouse_UID = dto.Warehouse_UID;
                        ws.Rack_ID = item.Rack_ID;
                        ws.Storage_ID = item.Storage_ID;
                        ws.Desc = item.Desc;
                        ws.Modified_UID = dto.Modified_UID;
                        ws.Modified_Date = DateTime.Now;
                        var hasdata = warehouseStorageRepository.GetFirstOrDefault(m => m.Warehouse_UID == ws.Warehouse_UID
                                                    & m.Rack_ID == ws.Rack_ID & m.Storage_ID == ws.Storage_ID);
                        if (hasdata != null)
                            return "更新仓库信息失败:仓库:" + dto.Warehouse_ID + ",料架:" + ws.Rack_ID + ",储位:" + ws.Storage_ID + ",已经存在,不可重复添加";
                        warehouseStorageRepository.Add(ws);
                        unitOfWork.Commit();
                    }
                    else
                    {
                        var warehousestorate = warehouseStorageRepository.GetById(item.Warehouse_Storage_UID);

                        //var hasdata = warehouseStorageRepository.GetFirstOrDefault(m => m.Warehouse_UID == warehousestorate.Warehouse_UID &
                        //                            m.Rack_ID == warehousestorate.Rack_ID & m.Storage_ID == warehousestorate.Storage_ID & m.Warehouse_Storage_UID != warehousestorate.Warehouse_Storage_UID);
                        //if (hasdata != null)
                        //{
                        //    return "修改储位信息失败:此储位信息已经存在";
                        //}

                        var hasdata1 = warehouseStorageRepository.GetFirstOrDefault(m => m.Warehouse_UID == warehousestorate.Warehouse_UID & m.Rack_ID == warehousestorate.Rack_ID & m.Storage_ID == warehousestorate.Storage_ID);

                        if (hasdata1 == null)
                        {
                            warehousestorate.Modified_UID = dto.Modified_UID;
                            warehousestorate.Modified_Date = DateTime.Now;
                            warehousestorate.Warehouse_UID = dto.Warehouse_UID;
                            warehousestorate.Rack_ID = item.Rack_ID;
                            warehousestorate.Storage_ID = item.Storage_ID;
                            warehousestorate.Desc = item.Desc;
                            warehouseStorageRepository.Update(warehousestorate);
                            unitOfWork.Commit();

                        }

                    }
                }
                //unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "更新仓库信息失败:" + e.Message;
            }
        }

        public string EditWarehouseStorageInfo(WarehouseDTO dto)
        {
            try
            {
                var warehousestorate = warehouseStorageRepository.GetById(dto.Warehouse_Storage_UID);
                warehousestorate.Modified_UID = dto.Modified_UID;
                warehousestorate.Modified_Date = DateTime.Now;
                warehousestorate.Warehouse_UID = dto.Warehouse_UID;
                warehousestorate.Rack_ID = dto.Rack_ID;
                warehousestorate.Storage_ID = dto.Storage_ID;
                warehousestorate.Desc = dto.WarehouseStorageDesc;
                var hasdata = warehouseStorageRepository.GetFirstOrDefault(m => m.Warehouse_UID == warehousestorate.Warehouse_UID &
                                            m.Rack_ID == warehousestorate.Rack_ID & m.Storage_ID == warehousestorate.Storage_ID & m.Warehouse_Storage_UID != warehousestorate.Warehouse_Storage_UID);
                if (hasdata != null)
                {
                    return "修改储位信息失败:此储位信息已经存在";
                }
                warehouseStorageRepository.Update(warehousestorate);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "修改储位信息失败:" + e.Message;
            }
        }

        public List<WarehouseDTO> QuerySignleWarehouseSt(int Warehouse_Storage_UID)
        {
            var bud = warehouseStorageRepository.GetByUid(Warehouse_Storage_UID);
            return bud;
        }

        public string DeleteWarehouseStorage(int WareStorage_UId)
        {
            string result = "";
            var entity = warehouseStorageRepository.GetFirstOrDefault(p => p.Warehouse_Storage_UID == WareStorage_UId);
            if (entity == null)
            {
                result = "此储位已经删除";
            }
            else
            {
                try
                {
                    warehouseStorageRepository.Delete(entity);
                    unitOfWork.Commit();
                    result = "SUCCESS";
                }
                catch
                {
                    result = "此储位已经被使用,不能删除";
                }
            }
            return result;
        }

        public List<WarehouseDTO> QueryWar(int Warehouse_UID)
        {
            var bud = warehouseRepository.GetByUId(Warehouse_UID).ToList();
            return bud;
        }

        public List<WarStorageDto> QueryWarhouseSts(int Warehouse_UID)
        {
            var bud = warehouseStorageRepository.GetMany(m => m.Warehouse_UID == Warehouse_UID);
            List<WarStorageDto> dtoList = new List<WarStorageDto>();
            foreach (var item in bud)
            {
                dtoList.Add(AutoMapper.Mapper.Map<WarStorageDto>(item));
            }
            return dtoList;
        }

        public string DeleteWarehouse(int Warehouse_UID)
        {
            try
            {
                var war = warehouseRepository.GetById(Warehouse_UID);
                warehouseRepository.Delete(war);
                unitOfWork.Commit();
                return "";
            }
            catch
            {
                return "该仓库所属的储位已经被使用";
            }
        }
        #endregion 储位管理
        #region 开账
        public PagedListModel<StorageInboundDTO> QueryCreateBounds(StorageInboundDTO searchModel, Page page)
        {
            int totalcount;
            var result = storageInboundRepository.GetCreateInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<StorageInboundDTO>(totalcount, result);
            return bd;
        }

        public List<StorageInboundDTO> DoExportCreateBoundReprot(string uids)
        {
            return storageInboundRepository.DoExportCreateBoundReprot(uids);
        }
        public List<StorageInboundDTO> DoAllExportCreateBoundReprot(StorageInboundDTO search)
        {
            return storageInboundRepository.DoAllExportCreateBoundReprot(search);
        }


        public List<WarehouseStorageDTO> QueryAllWarehouseSt()
        {
            var warests = warehouseStorageRepository.GetAllInfo();

            List<WarehouseStorageDTO> dtoList = new List<WarehouseStorageDTO>();
            foreach (var item in warests)
            {
                dtoList.Add(AutoMapper.Mapper.Map<WarehouseStorageDTO>(item));
            }
            return dtoList;
        }
        public List<WarehouseStorageDTO> GetAllWarehouseSt(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            return warehouseStorageRepository.GetAllWarehouseSt(Plant_Organization_UID,  BG_Organization_UID,  FunPlant_Organization_UID);          
        }

        public List<EnumerationDTO> QueryStorageEnums(string Enum_Type)
        {
            var enums = enumerationRepository.GetMany(m => m.Enum_Type == Enum_Type);

            List<EnumerationDTO> dtoList = new List<EnumerationDTO>();
            foreach (var item in enums)
            {
                dtoList.Add(AutoMapper.Mapper.Map<EnumerationDTO>(item));
            }
            return dtoList;
        }

        public List<StorageInboundDTO> QueryStorageInbound()
        {
            var stoinbound = storageInboundRepository.GetAll();

            List<StorageInboundDTO> dtoList = new List<StorageInboundDTO>();
            foreach (var item in stoinbound)
            {
                dtoList.Add(AutoMapper.Mapper.Map<StorageInboundDTO>(item));
            }
            return dtoList;
        }

        public string InsertCreateBoundItem(List<StorageInboundDTO> dtolist)
        {
            return storageInboundRepository.InsertCreateItem(dtolist);
        }

        public List<WarehouseDTO> QueryWarehousesSt()
        {
            var bud = warehouseStorageRepository.GetStinfo().ToList();
            return bud;
        }
        public List<WarehouseDTO> GetWarehouseSITE(int planID)
        {
            var bud = storageTransferRepository.GetWarehouseSITE(planID);
            return bud;
        }
     
        public List<WarehouseDTO> QueryWarStroge(int OPTypeID)
        {
            var bud = warehouseStorageRepository.QueryWarStroge(OPTypeID).ToList();
            return bud;
        }
        public string AddOrEditCreateBound(StorageInboundDTO dto)
        {
            try
            {
                if (dto.Storage_Inbound_UID == 0)
                {
                    var hasmatinventory = storageInboundRepository.GetMany(m => m.Material_UID == dto.Material_Uid && m.Status_UID != 420).ToList();
                    if (hasmatinventory.Count > 0)
                        return "此料号已开过账";
                    //fky2017/11/13
                    //dto.Status_UID = 374;
                    dto.Status_UID = 407;
                    //fky2017/11/13
                    //dto.Storage_Inbound_Type_UID = 372;
                    dto.Storage_Inbound_Type_UID = 406;
                    dto.Desc = "期初開帳";
                    var stoinbound = storageInboundRepository.GetAll().ToList();
                    string PreInboundID = "Opening" + DateTime.Today.ToString("yyyyMMdd");
                    var test = stoinbound.Where(m => m.Storage_Inbound_ID.StartsWith(PreInboundID)).ToList();
                    string PostInboundID = (test.Count() + 1).ToString("0000");
                    dto.Storage_Inbound_ID = PreInboundID + PostInboundID;

                    List<StorageInboundDTO> list = new List<StorageInboundDTO>();
                    list.Add(dto);
                    return InsertCreateBoundItem(list);
                }
                else
                {
                    var hasmatinventory = storageInboundRepository.GetMany(m => m.Material_UID == dto.Material_Uid & m.Storage_Inbound_UID != dto.Storage_Inbound_UID && m.Status_UID != 420).ToList();
                    if (hasmatinventory.Count > 0)
                        return "此料号已开过账";
                    var createbound = storageInboundRepository.GetFirstOrDefault(m => m.Storage_Inbound_UID == dto.Storage_Inbound_UID);
                    createbound.Material_UID = dto.Material_Uid;
                    createbound.Warehouse_Storage_UID = dto.Warehouse_Storage_UID;
                    createbound.Applicant_UID = dto.Applicant_UID;
                    createbound.Approver_Date = DateTime.Now;
                    createbound.Be_Check_Qty = dto.Inbound_Qty;
                    createbound.OK_Qty = dto.Inbound_Qty;
                    storageInboundRepository.Update(createbound);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "新增开账 信息失败:" + e.Message;
            }
        }
        public string ApproveCreateboundAll(StorageInboundDTO dto)
        {
            try
            {
                List<Storage_Inbound> Storage_Inbounds = storageInboundRepository.GetAll().ToList();
                List<Material_Inventory> Material_Inventorys = materialInventoryRepository.GetAll().ToList();
                List<StorageInboundDTO> StorageInboundDTOs = storageInboundRepository.DoAllExportCreateBoundReprot(dto).Where(o => o.Status == "未审核").ToList();

                StringBuilder strsql = new StringBuilder();
                foreach (var item in StorageInboundDTOs)
                {
                    //  ApproveCreatebound(item.Storage_Inbound_UID, dto.Approver_UID);
                    strsql.Append(ApproveCreatebound(item.Storage_Inbound_UID, dto.Approver_UID, Storage_Inbounds, Material_Inventorys));
                }
                if (strsql.ToString() != "")
                {
                    storageInboundRepository.InsertCreateAll(strsql);
                }

                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        public string ApproveCreatebound(int Storage_Inbound_UID, int Useruid, List<Storage_Inbound> Storage_Inbounds, List<Material_Inventory> Material_Inventorys)
        {

            var Createbound = Storage_Inbounds.FirstOrDefault(o => o.Storage_Inbound_UID == Storage_Inbound_UID);
            string sqlUpdate = string.Format("  UPDATE Storage_Inbound SET Status_UID= 408, Approver_UID = {0}, Approver_Date ='{1}' where Storage_Inbound_UID ={2}  ", Useruid, DateTime.Now.ToString(FormatConstants.DateTimeFormatString), Storage_Inbound_UID);
            #region 修改出入库明细表
            decimal balanceqty = 0;
            var matinventory = Material_Inventorys.Where(m => m.Material_UID == Createbound.Material_UID);
            if (matinventory.Count() > 0)
                balanceqty = matinventory.Sum(m => m.Inventory_Qty);
            string sqlInOutDetail = string.Format(@"  INSERT INTO Storage_InOut_Detail
                                                       (InOut_Type_UID
                                                       ,Storage_InOut_UID
                                                       ,Material_UID
                                                       ,Warehouse_Storage_UID
                                                       ,InOut_Date
                                                       ,InOut_Qty
                                                       ,Balance_Qty
                                                       ,UnitPrice
                                                       ,[Desc]
                                                       ,Modified_UID
                                                       ,Modified_Date)
                                                 VALUES ({0}, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, N'{8}', {9}, '{10}')   ",
                                                   415,
                                                   Storage_Inbound_UID,
                                                   Createbound.Material_UID,
                                                   Createbound.Warehouse_Storage_UID,
                                                   DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                                                   Createbound.OK_Qty,
                                                   balanceqty + Createbound.OK_Qty,
                                                   0,
                                                   "期初開帳",
                                                   Useruid,
                                                   DateTime.Now.ToString(FormatConstants.DateTimeFormatString));
            #endregion
            #region  修改料号库存明细表
            string sqlInventory = string.Format(@"  INSERT INTO Material_Inventory
                                                               (Material_UID
                                                               ,Warehouse_Storage_UID
                                                               ,Inventory_Qty
                                                               ,[Desc]
                                                               ,Modified_UID
                                                               ,Modified_Date)
                                                         VALUES  ({0}, {1}, {2}, N'{3}', {4}, '{5}')  ",
                                                           Createbound.Material_UID,
                                                           Createbound.Warehouse_Storage_UID,
                                                           Createbound.OK_Qty,
                                                           Createbound.Desc,
                                                           Useruid,
                                                           DateTime.Now.ToString(FormatConstants.DateTimeFormatString));
            #endregion 修改料号库存明细表

            return sqlUpdate + sqlInOutDetail + sqlInventory;
        }

        public string ApproveCreatebound(int Storage_Inbound_UID, int Useruid)
        {
            try
            {
                var Createbound = storageInboundRepository.GetById(Storage_Inbound_UID);
                //fky2017/11/13
                //Createbound.Status_UID = 376;
                Createbound.Status_UID = 408;
                Createbound.Approver_Date = DateTime.Now;
                Createbound.Approver_UID = Useruid;
                storageInboundRepository.Update(Createbound);

                #region 修改出入库明细表
                decimal balanceqty = 0;
                var matinventory = materialInventoryRepository.GetMany(m => m.Material_UID == Createbound.Material_UID);
                if (matinventory.Count() > 0)
                    balanceqty = matinventory.Sum(m => m.Inventory_Qty);
                Storage_InOut_Detail STD = new Storage_InOut_Detail();
                //fky2017/11/13
                // STD.InOut_Type_UID = 386;
                STD.InOut_Type_UID = 415;

                STD.Storage_InOut_UID = Storage_Inbound_UID;
                STD.Material_UID = Createbound.Material_UID;
                STD.Warehouse_Storage_UID = Createbound.Warehouse_Storage_UID;
                STD.InOut_Date = DateTime.Now;
                STD.InOut_Qty = Createbound.OK_Qty;
                STD.Balance_Qty = balanceqty + Createbound.OK_Qty;
                STD.UnitPrice = 0;
                STD.Desc = "期初開帳";
                STD.Modified_UID = Useruid;
                STD.Modified_Date = DateTime.Now;
                storageInOutDetailRepository.Add(STD);

                #endregion

                #region  修改料号库存明细表

                Material_Inventory matinven = new Material_Inventory();

                matinven.Material_UID = Createbound.Material_UID;
                matinven.Warehouse_Storage_UID = Createbound.Warehouse_Storage_UID;
                matinven.Inventory_Qty = Createbound.OK_Qty;
                matinven.Desc = Createbound.Desc;
                matinven.Modified_Date = DateTime.Now;
                matinven.Modified_UID = Useruid;

                materialInventoryRepository.Add(matinven);

                # endregion 修改料号库存明细表

                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        public string DeleteCreateBound(int Storage_Inbound_UID, int userid)
        {
            try
            {
                var bound = storageInboundRepository.GetById(Storage_Inbound_UID);
                bound.Applicant_UID = userid;
                bound.Applicant_Date = DateTime.Now;
                //fky2017/11/13
                //bound.Status_UID = 392;
                bound.Status_UID = 420;
                storageInboundRepository.Update(bound);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除开账信息失败:" + e.Message;
            }

        }
        #endregion 开账 
        #region 出入库
        public List<InOutBoundInfoDTO> DoExportUpdateBoundReprot(List<InOutBoundVM> uids)
        {
            List<OutBoundInfoDTO> OutBoundInfoDTOs = GetOutBoundInfoDTOALL();
            List<InOutBoundInfoDTO> InOutBoundInfoDTOs = storageInboundRepository.DoExportUpdateBoundReprot(uids);
            foreach (var item in InOutBoundInfoDTOs)
            {
                if (item.In_Out_Type == "领料单")
                {
                    item.OutBoundInfos = OutBoundInfoDTOs.Where(o => o.Storage_Outbound_M_UID == item.Storage_Bound_UID).ToList();
                }
            }

            return InOutBoundInfoDTOs;

            //return storageInboundRepository.DoExportUpdateBoundReprot(uids);
        }
        public List<InOutBoundInfoDTO> DoAllExportUpdateBoundReprot(InOutBoundInfoDTO search)
        {
            List<OutBoundInfoDTO> OutBoundInfoDTOs = GetOutBoundInfoDTOALL();
            List<InOutBoundInfoDTO> InOutBoundInfoDTOs = storageInboundRepository.DoAllExportUpdateBoundReprot(search);
            foreach (var item in InOutBoundInfoDTOs)
            {
                if (item.In_Out_Type == "领料单")
                {

                    item.OutBoundInfos = OutBoundInfoDTOs.Where(o => o.Storage_Outbound_M_UID == item.Storage_Bound_UID).ToList();
                    // Storage_Bound_ID
                }
            }

            return InOutBoundInfoDTOs;
        }
        public List<StorageInboundDTO> QueryCreateBound()
        {
            var storageinbound = storageInboundRepository.GetAllInfo();
            return storageinbound;
        }

        public List<MaterialInventoryDTO> QueryAllMatInventory()
        {
            var storageinbound = materialInventoryRepository.GetAllInfo();
            return storageinbound;
        }

        public PagedListModel<InOutBoundInfoDTO> QueryBoundDetails(InOutBoundInfoDTO searchModel, Page page)
        {
            int totalcount;
            var result = storageInboundRepository.GetDetailInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<InOutBoundInfoDTO>(totalcount, result);
            return bd;
        }

        public string AddInBoundApply(StorageInboundDTO dto)
        {
            try
            {
                if (dto.Storage_Inbound_UID == 0)
                {
                    if (dto.Sum_PuQty + dto.OK_Qty > dto.PU_Qty)
                        return "采购单" + dto.PU_NO + "已入库" + dto.Sum_PuQty + "剩余数量不足" + dto.OK_Qty;
                    //fky2017/11/13
                    //dto.Status_UID = 374;
                    dto.Status_UID = 407;
                    //fky2017/11/13
                    //dto.Storage_Inbound_Type_UID = 383;
                    dto.Storage_Inbound_Type_UID = 413;

                    dto.Desc = "入库单-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == dto.PartType_UID).Enum_Value;
                    string preInbound = "In" + DateTime.Today.ToString("yyyyMMdd");
                    var test = storageInboundRepository.GetMany(m => m.Storage_Inbound_ID.StartsWith(preInbound)).ToList();
                    string proInbound = (test.Count() + 1).ToString("0000");
                    dto.Storage_Inbound_ID = preInbound + proInbound;
                    dto.Inbound_Price = 0;
                    List<StorageInboundDTO> list = new List<StorageInboundDTO>();
                    list.Add(dto);
                    return InsertInBoundItem(list);
                }
                else
                {
                    if (dto.Sum_PuQty + dto.OK_Qty > dto.PU_Qty)
                        return "采购单【" + dto.PU_NO + "】的采购数量为" + dto.PU_Qty + ",已入库" + dto.Sum_PuQty + ",剩余数量不足" + dto.OK_Qty;
                    var storageinbolund = storageInboundRepository.GetById(dto.Storage_Inbound_UID);
                    //判斷採購單號不為空，則為採購入庫需重算移動成本
                    if (!string.IsNullOrEmpty(storageinbolund.PU_NO) || !string.IsNullOrEmpty(dto.PU_NO))
                    {
                        return storageInboundRepository.UpdateInBoundsWithMAP(dto);
                    }
                    else
                    {
                        storageinbolund.PU_NO = dto.PU_NO;
                        storageinbolund.PU_Qty = dto.PU_Qty;
                        storageinbolund.Issue_NO = dto.Issue_NO;
                        storageinbolund.NG_Qty = dto.NG_Qty;
                        storageinbolund.PartType_UID = dto.PartType_UID;
                        storageinbolund.Material_UID = dto.Material_Uid;
                        storageinbolund.Be_Check_Qty = dto.Be_Check_Qty;
                        storageinbolund.OK_Qty = dto.OK_Qty;
                        storageinbolund.Warehouse_Storage_UID = dto.Warehouse_Storage_UID;
                        storageInboundRepository.Update(storageinbolund);
                        unitOfWork.Commit();
                        return "0";
                    }
                }
            }
            catch (Exception e)
            {
                return "入库操作失败:" + e.Message;
            }
        }

        public string InsertInBoundItem(List<StorageInboundDTO> dtolist)
        {
            return storageInboundRepository.InsertInItem(dtolist);
        }

        public List<StorageInboundDTO> QueryInBoudSingle(int Storage_Inbound_UID)
        {
            var bud = storageInboundRepository.GetByUId(Storage_Inbound_UID).ToList();
            return bud;
        }

        public string ApproveInbound(int Storage_Inbound_UID, int Useruid)
        {
            try
            {
                var Inbound = storageInboundRepository.GetById(Storage_Inbound_UID);
                //fky2017/11/13
                //Inbound.Status_UID = 376;
                Inbound.Status_UID = 408;
                Inbound.Approver_Date = DateTime.Now;
                Inbound.Approver_UID = Useruid;
                storageInboundRepository.Update(Inbound);

                #region 修改出入库明细表
                //因为计算结存数量时不区分储位,所以此处matqty不限定储位
                decimal matqty = 0;
                var inventorys = materialInventoryRepository.GetMany(m => m.Material_UID == Inbound.Material_UID);
                if (inventorys.Count() > 0)
                {
                    matqty = inventorys.Sum(m => m.Inventory_Qty);
                }

                Storage_InOut_Detail STD = new Storage_InOut_Detail();
                //fky2017/11/13
                //STD.InOut_Type_UID = 379;
                STD.InOut_Type_UID = 411;
                STD.Storage_InOut_UID = Storage_Inbound_UID;
                STD.Material_UID = Inbound.Material_UID;
                STD.Warehouse_Storage_UID = Inbound.Warehouse_Storage_UID;
                STD.InOut_Date = DateTime.Now;
                STD.InOut_Qty = Inbound.OK_Qty;
                STD.Balance_Qty = matqty + Inbound.OK_Qty;
                STD.UnitPrice = 0;
                STD.Desc = Inbound.Desc;
                STD.Modified_UID = Useruid;
                STD.Modified_Date = DateTime.Now;
                storageInOutDetailRepository.Add(STD);
                #endregion 修改出入库明细表

                #region  修改料号库存明细表

                var matinven = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == Inbound.Material_UID &
                    m.Warehouse_Storage_UID == Inbound.Warehouse_Storage_UID);
                if (matinven != null)
                {
                    matinven.Modified_Date = DateTime.Now;
                    matinven.Modified_UID = Useruid;
                    matinven.Desc = Inbound.Desc;
                    matinven.Inventory_Qty += Inbound.OK_Qty;
                    materialInventoryRepository.Update(matinven);
                }
                else
                {
                    Material_Inventory newmatinv = new Material_Inventory();
                    newmatinv.Material_UID = Inbound.Material_UID;
                    newmatinv.Warehouse_Storage_UID = Inbound.Warehouse_Storage_UID;
                    newmatinv.Inventory_Qty = Inbound.OK_Qty;
                    newmatinv.Desc = Inbound.Desc;
                    newmatinv.Modified_Date = DateTime.Now;
                    newmatinv.Modified_UID = Useruid;
                    materialInventoryRepository.Add(newmatinv);
                }
                # endregion 修改料号库存明细表

                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        public List<StorageInboundDTO> QueryWarSt(int inboundtype, int plantuid)
        {
            var bud = storageInboundRepository.GetWarSt(inboundtype, plantuid);
            return bud;
        }

        public List<StorageInboundDTO> QueryWarStByKey(int inboundtype,int warStUid, string key, int plantuid)
        {
            var bud = storageInboundRepository.GetWarStByKey(inboundtype, warStUid, key, plantuid);
            return bud;
        }

        public List<string> QueryFunplantByUser(int userid)
        {
            var bud = storageInboundRepository.GetFunplantByUser(userid);
            return bud;
        }

        public List<EQPUserTableDTO> QueryUsers(int PlantID)
        {
            var systemuser = eQPUserTableRepository.GetAll();
            if (PlantID != 0)
            {
                systemuser = eQPUserTableRepository.GetAll().Where(o => o.Plant_OrganizationUID == PlantID);
            }
            List<EQPUserTableDTO> dtoList = new List<EQPUserTableDTO>();
            foreach (var item in systemuser)
            {
                dtoList.Add(AutoMapper.Mapper.Map<EQPUserTableDTO>(item));
            }
            return dtoList;
        }

        public List<MaterialInventoryDTO> QuerySingleMatinventory(int Material_Uid, int Warehouse_Storage_UID)
        {
            var bud = materialInventoryRepository.GetSingleMatinventory(Material_Uid, Warehouse_Storage_UID);
            return bud;
        }

        public List<MaterialInventoryDTO> QueryMatinventoryByMat(int Material_Uid)
        {
            var bud = materialInventoryRepository.GetMatinventoryByMat(Material_Uid);
            return bud;
        }

        public List<EQPRepairInfoDTO> QuerySingleEqprepair(string Repair_id)
        {
            var bud = eQPRepairInfoRepository.GetSingleEQPRepair(Repair_id);           
            if (bud != null && bud.Count > 0)
            { 
                //取得更新料号相關信息(含儲位)
                var Repair_Uid = bud[0].Repair_Uid;
                bud[0].listMeterialUpdateInfoWithWarehouse = meterialUpdateInfoRepository.GetMeterialUpdateInfoWithWarehouse(Repair_Uid);
            }
            return bud;
        }
        public string AddOrEditOutbound(OutBoundInfo dto)
        {
            try
            {
                if (dto.details.Count() != dto.details.GroupBy(m => m.Material_Uid & m.Warehouse_Storage_UID).Count())
                {
                    return "不可重复添加同储位的同料号";
                }

                //檢查維修單出庫數量
                if (dto.Storage_Outbound_Type_UID == 416 && dto.Repair_Uid != 0) {
                    List<RepairOutboundDTO> listR = storageOutboundDRepository.GetRepairOutboundQty(dto.Repair_Uid, dto.Storage_Outbound_M_UID);
                    var tmp = dto.details.GroupBy(m => m.Material_Uid).Select(g => new { Material_Uid = g.Key, Sum_Qty = g.Sum(m => m.Outbound_Qty) });
                    var errorMsg = ""
 ;                   foreach (var d in tmp) {
                        var r = listR.Where(m => m.Material_Uid == d.Material_Uid).FirstOrDefault();
                        if (r != null)
                        {
                            if ((r.Update_No - r.Sum_Outbound_Qty) < d.Sum_Qty)
                            {
                                errorMsg += "料号[" + r.Material_Id + "]申请数量[" + d.Sum_Qty.ToString("0") + "]大於维修单申请总量[" + r.Update_No + "]扣除出庫數量(含未審核)[" + r.Sum_Outbound_Qty.ToString("0") + "];";
                            }
                        }
                        else {
                            errorMsg += "查询维修单申请总量错误;";
                        }
                    }
                    if (errorMsg != "") {
                        return errorMsg;
                    }
                }
                
                if (dto.Storage_Outbound_M_UID == 0)
                {
                    Storage_Outbound_M SOM = new Storage_Outbound_M();

                    SOM.Storage_Outbound_Type_UID = dto.Storage_Outbound_Type_UID;
                    string preOutbound = "";
                    //fky2017/11/13
                    // if (dto.Storage_Outbound_Type_UID==387)
                    if (dto.Storage_Outbound_Type_UID == 416)
                    {
                        preOutbound = "Out" + DateTime.Today.ToString("yyyyMMdd");
                        SOM.Repair_Uid = dto.Repair_Uid;
                    }
                    else
                    {
                        preOutbound = "NGOut" + DateTime.Today.ToString("yyyyMMdd");
                        SOM.Desc = dto.Desc;
                    }
                    var test = storageOutboundMRepository.GetMany(m => m.Storage_Outbound_ID.StartsWith(preOutbound)).ToList();
                    string proOutbound = (test.Count() + 1).ToString("0000");
                    SOM.Storage_Outbound_ID = preOutbound + proOutbound;
                    SOM.Outbound_Account_UID = dto.Outbound_Account_UID;
                    SOM.Applicant_UID = dto.Modified_UID;
                    SOM.Applicant_Date = DateTime.Now;
                    //fky2017/11/13
                    //SOM.Status_UID = 374;
                    SOM.Status_UID = 407;
                    SOM.Approver_UID = dto.Modified_UID;
                    SOM.Approver_Date = DateTime.Now;

                    storageOutboundMRepository.Add(SOM);
                    unitOfWork.Commit();

                    //新增子表
                    foreach (OutBoundDetail detail in dto.details)
                    {
                        var outbound_uid = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_ID == preOutbound + proOutbound).Storage_Outbound_M_UID;
                        Storage_Outbound_D SOD = new Storage_Outbound_D();
                        SOD.Storage_Outbound_M_UID = outbound_uid;
                        SOD.Material_UID = detail.Material_Uid;
                        SOD.Warehouse_Storage_UID = detail.Warehouse_Storage_UID;
                        SOD.Outbound_Qty = detail.Outbound_Qty;
                        SOD.Outbound_Price = 0;
                        SOD.PartType_UID = detail.PartType_UID;
                        SOD.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == dto.Storage_Outbound_Type_UID).Enum_Value;
                        SOD.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == detail.PartType_UID).Enum_Value;
                        storageOutboundDRepository.Add(SOD);
                    }
                }
                else
                {
                    var outboundM = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_M_UID == dto.Storage_Outbound_M_UID);
                    var outboundDs = storageOutboundDRepository.GetMany(m => m.Storage_Outbound_M_UID == dto.Storage_Outbound_M_UID).ToList();
                    foreach (Storage_Outbound_D item in outboundDs)
                    {
                        storageOutboundDRepository.Delete(item);
                    }
                    //fky2017/11/13
                    // if (dto.Storage_Outbound_Type_UID == 387)
                    if (dto.Storage_Outbound_Type_UID == 416)
                    {
                        outboundM.Repair_Uid = dto.Repair_Uid;
                    }
                    else
                    {
                        outboundM.Desc = dto.Desc;
                    }
                    outboundM.Outbound_Account_UID = dto.Outbound_Account_UID;
                    outboundM.Applicant_UID = dto.Modified_UID;
                    outboundM.Applicant_Date = DateTime.Now;
                    //fky2017/11/13
                    //outboundM.Status_UID = 374;
                    outboundM.Status_UID = 407;
                    outboundM.Approver_UID = dto.Modified_UID;
                    outboundM.Approver_Date = DateTime.Now;
                    storageOutboundMRepository.Update(outboundM);
                    //新增子表
                    foreach (OutBoundDetail detail in dto.details)
                    {
                        Storage_Outbound_D outboundD = new Storage_Outbound_D();
                        outboundD.Storage_Outbound_M_UID = dto.Storage_Outbound_M_UID;
                        outboundD.Material_UID = detail.Material_Uid;
                        outboundD.Warehouse_Storage_UID = detail.Warehouse_Storage_UID;
                        outboundD.Outbound_Qty = detail.Outbound_Qty;
                        outboundD.Outbound_Price = 0;
                        outboundD.PartType_UID = detail.PartType_UID;
                        outboundD.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == dto.Storage_Outbound_Type_UID).Enum_Value;
                        outboundD.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == detail.PartType_UID).Enum_Value;
                        storageOutboundDRepository.Add(outboundD);
                    }
                }
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "出库操作失败:" + e.Message;
            }
        }

        public string DeleteBound(int Storage_UID, string Inout_Type, int UserUid)
        {
            try
            {
                //fky2017/11/13
                //int Status_Uid = 392;
                int Status_Uid = 420;
                if (Inout_Type == "入库单")
                {
                    var inbound = storageInboundRepository.GetById(Storage_UID);
                    //判斷採購單號不為空，則為採購入庫需重算移動成本
                    if (!string.IsNullOrEmpty(inbound.PU_NO))
                    {
                        var dto = new StorageInboundDTO();
                        dto.Storage_Inbound_UID = inbound.Storage_Inbound_UID;
                        dto.Applicant_Date = DateTime.Now;
                        dto.Applicant_UID = UserUid;
                        dto.Status_UID = Status_Uid;
                        return storageInboundRepository.DeleteInBoundsWithMAP(dto);
                    }
                    else
                    {
                        inbound.Applicant_Date = DateTime.Now;
                        inbound.Applicant_UID = UserUid;
                        inbound.Status_UID = Status_Uid;
                        storageInboundRepository.Update(inbound);
                        unitOfWork.Commit();
                        return "";
                    }
                }
                else
                {
                    //var outboundDs = storageOutboundDRepository.GetMany(m => m.Storage_Outbound_M_UID == Storage_UID).ToList();
                    //foreach (Storage_Outbound_D item in outboundDs)
                    //{
                    //    storageOutboundDRepository.Delete(item);
                    //}
                    var outboundM = storageOutboundMRepository.GetById(Storage_UID);
                    outboundM.Applicant_UID = UserUid;
                    outboundM.Applicant_Date = DateTime.Now;
                    outboundM.Status_UID = Status_Uid;
                    storageOutboundMRepository.Update(outboundM);
                    unitOfWork.Commit();
                    return "";
                }
            }
            catch (Exception e)
            {
                return "删除出入库信息失败:" + e.Message;
            }
        }

        public List<StorageOutboundDTO> QueryOutBoudSingle(int Storage_Outbound_UID)
        {
            var bud = storageOutboundMRepository.GetByUid(Storage_Outbound_UID).ToList();
            return bud;
        }
        public List<OutBoundDetail> QueryOutBondDs(int Storage_Bound_UID)
        {
            var bud = storageOutboundDRepository.GetByMUid(Storage_Bound_UID).ToList();
            return bud;
        }
        public List<OutBoundInfoDTO> GetOutBoundInfoDTOALL()
        {
            var bud = storageOutboundDRepository.GetOutBoundInfoDTOALL().ToList();
            return bud;
        }
        public string ApproveOutbound(int Storage_Outbound_M_UID, int Useruid)
        {
            try
            {
                var Outbound = storageOutboundMRepository.GetById(Storage_Outbound_M_UID);
                //fky2017/11/13
                //Outbound.Status_UID = 376;
                Outbound.Status_UID = 408;
                Outbound.Approver_Date = DateTime.Now;
                Outbound.Approver_UID = Useruid;
                storageOutboundMRepository.Update(Outbound);


                var outboundds = storageOutboundDRepository.GetMany(m => m.Storage_Outbound_M_UID == Storage_Outbound_M_UID).ToList();


                foreach (Storage_Outbound_D item in outboundds)
                {
                    #region 修改出入库明细表
                    //因为计算结存数量时不区分储位,所以此处matqty不限定储位
                    var matqty = materialInventoryRepository.GetMany(m => m.Material_UID == item.Material_UID).Sum(m => m.Inventory_Qty);
                    Storage_InOut_Detail STD = new Storage_InOut_Detail();
                    //fky2017/11/13
                    //if (Outbound.Storage_Outbound_Type_UID == 388)
                    if (Outbound.Storage_Outbound_Type_UID == 417)
                        //fky2017/11/13
                        // STD.InOut_Type_UID = 382;
                        STD.InOut_Type_UID = 412;
                    //fky2017/11/13
                    //else if (Outbound.Storage_Outbound_Type_UID == 387)
                    else if (Outbound.Storage_Outbound_Type_UID == 416)
                        //fky2017/11/13
                        //STD.InOut_Type_UID = 400;
                        STD.InOut_Type_UID = 425;
                    STD.Storage_InOut_UID = item.Storage_Outbound_D_UID;
                    STD.Material_UID = item.Material_UID;
                    STD.Warehouse_Storage_UID = item.Warehouse_Storage_UID;
                    STD.InOut_Date = DateTime.Now;
                    STD.InOut_Qty = item.Outbound_Qty;
                    STD.Balance_Qty = matqty - item.Outbound_Qty;
                    STD.UnitPrice = 0;
                    STD.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == Outbound.Storage_Outbound_Type_UID).Enum_Value;
                    STD.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == item.PartType_UID).Enum_Value;
                    STD.Modified_UID = Useruid;
                    STD.Modified_Date = DateTime.Now;
                    storageInOutDetailRepository.Add(STD);
                    #endregion 修改出入库明细表

                    #region  修改料号库存明细表

                    var matinven = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == item.Material_UID &
                        m.Warehouse_Storage_UID == item.Warehouse_Storage_UID);
                    matinven.Modified_Date = DateTime.Now;
                    matinven.Modified_UID = Useruid;
                    matinven.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == Outbound.Storage_Outbound_Type_UID).Enum_Value;
                    matinven.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == item.PartType_UID).Enum_Value;
                    matinven.Inventory_Qty -= item.Outbound_Qty;
                    if (matinven.Inventory_Qty < 0)
                    {
                        return "库存量不足,无法审核";
                    }
                    materialInventoryRepository.Update(matinven);

                    #endregion 修改料号库存明细表
                }


                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        public List<StorageInboundDTO> QueryPuqty(string PUNO, int Storage_Inbound_UID)
        {
            var bud = storageInboundRepository.GetPuinfo(PUNO, Storage_Inbound_UID).ToList();
            return bud;
        }

        public List<SystemOrgDTO> QueryWarOps(int PlantID)
        {
            var bud = warehouseRepository.QueryWarOps(PlantID).ToList();
            return bud;
        }

        public List<SystemOrgDTO> QueryWarFunplantByop(int opuid, int PartType_UID)
        {
            var bud = warehouseRepository.GetWarFunplantsByop(opuid, PartType_UID).ToList();
            return bud;
        }
        public List<WarehouseDTO> QueryWarIdByFunction(int Functionuid, int PartType_UID)
        {
            var bud = warehouseRepository.GetWarIdByFunction(Functionuid, PartType_UID).ToList();
            return bud;
        }

        public string SaveWarehouseSt(WarehouseStorageDTO dto)
        {
            try
            {
                Warehouse_Storage ws = new Warehouse_Storage();
                ws.Warehouse_UID = dto.Warehouse_UID;
                ws.Rack_ID = dto.Rack_ID;
                ws.Storage_ID = dto.Storage_ID;
                ws.Is_Enable = false;
                ws.Desc = dto.Desc;
                ws.Modified_UID = dto.Modified_UID;
                ws.Modified_Date = DateTime.Now;
                warehouseStorageRepository.Add(ws);
                unitOfWork.Commit();
                var warst = warehouseStorageRepository.GetFirstOrDefault(m => m.Warehouse_UID == dto.Warehouse_UID &
                                        m.Rack_ID == dto.Rack_ID & m.Storage_ID == dto.Storage_ID);
                if (warst != null)
                    return warst.Warehouse_Storage_UID.ToString();
                else
                    return "FAIL!";
            }
            catch (Exception e)
            {
                return "FAIL!";
            }
        }

        public List<MaterialInventoryDTO> QueryMatByOutType(int Out_Type_Uid, int FunplantUid)
        {
            var bud = materialInventoryRepository.GetMatByOutType(Out_Type_Uid, FunplantUid);
            return bud;
        }

        public List<MaterialInventoryDTO> QueryWarStByMaterial(int Material_Uid)
        {
            var bud = materialInventoryRepository.GetWarStByMat(Material_Uid);
            return bud;
        }

        public List<MaterialInventoryDTO> GetWarStByMatCheck(int Material_Uid)
        {
            var bud = materialInventoryRepository.GetWarStByMatCheck(Material_Uid);
            return bud;
        }
        
        public Dictionary<int, string> ImportCoupaPO_To_InBoundApply(Dictionary<int, StorageInboundDTO> dics)
        {
            List<MaterialInfoDTO> mList = new List<MaterialInfoDTO>();
            List<StorageInboundDTO> addList = new List<StorageInboundDTO>();
            Dictionary<int, string> row_errors = new Dictionary<int, string>();
            foreach (var dic in dics)
            {
                StorageInboundDTO dto = dic.Value;
                try
                {
                    var sDto = storageInboundRepository.GetFirstOrDefault(s => s.PU_NO == dto.CoupaPO_ID && s.Status_UID != 420);
                    if (sDto != null) {
                        //若CoupaPO_ID已存在，且數量、單價不相符須提示，反之則不作任何動作
                        if (sDto.PU_Qty != dto.PU_Qty || sDto.Current_POPrice != dto.Current_POPrice)
                        {
                            row_errors.Add(dic.Key, "采购单号[" + dto.CoupaPO_ID + "]已存在，且数量、单价不相符;");
                        }
                        continue;
                    }

                    //檢查料號是否已加入清單
                    MaterialInfoDTO mDto = mList.FirstOrDefault(m => m.Material_Id == dto.Material_Id);
                    if (mDto == null)
                    {
                        //檢查料號信息是否正確
                        var mEntity = materialInfoRepository.GetFirstOrDefault(m => m.Material_Id == dto.Material_Id);
                        if (mEntity == null)
                        {
                            row_errors.Add(dic.Key, "料号[" + dto.Material_Id + "]查无材料信息;");
                            continue;
                        }
                        else
                        {
                            if (mEntity.Warehouse_Storage_UID == null)
                            {
                                row_errors.Add(dic.Key, "料号[" + dto.Material_Id + "]查无储位信息;");
                                continue;
                            }
                            //將料號加入清單
                            mDto = new MaterialInfoDTO();
                            mDto.Material_Id = mEntity.Material_Id;
                            mDto.Material_Uid = mEntity.Material_Uid;
                            mDto.Warehouse_Storage_UID = mEntity.Warehouse_Storage_UID;//材料信息檔
                            mDto.Unit_Price = mEntity.Unit_Price ?? 0;
                            mDto.Last_Qty = mEntity.Last_Qty ?? 0;
                            mList.Add(mDto);
                        }
                    }
                    //計算移動成本
                    decimal? newPrcie = 0;
                    decimal? newQty = mDto.Last_Qty + dto.PU_Qty;
                    if (newQty > 0)
                    {
                        newPrcie = ((mDto.Last_Qty * mDto.Unit_Price) + (dto.PU_Qty * dto.Current_POPrice)) / newQty;
                    }
                    mDto.Unit_Price = newPrcie;
                    mDto.Last_Qty = newQty;
                    addList.Add(dto);
                }
                catch (Exception e)
                {
                    row_errors.Add(dic.Key, "保存设备申请单失败，请检查你的权限！" + e.Message);
                }
            }
            try
            {
                if (row_errors.Count == 0)
                {
                    var result = storageInboundRepository.InsertInBoundsWithMAP(mList,addList);
                    if (result != "") {
                        row_errors.Add(1, "数据库储存失败" + result);
                    }
                }
            }
            catch (Exception ex)
            {
                row_errors.Add(row_errors.Max(x => x.Key) + 1, "数据库储存失败" + ex.Message);
            }
            return row_errors;
        }
        #endregion 出入库 

        #region 盘点

        public PagedListModel<StorageCheckDTO> QueryStorageChecks(StorageCheckDTO searchModel, Page page)
        {
            int totalcount;
            var result = storageCheckRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<StorageCheckDTO>(totalcount, result);
            return bd;
        }
        public string ImportStorageCheck(List<StorageCheckDTO> dtolist)
        {
            try
            {
                if (dtolist.Count > 0)
                {

                    foreach (var item in dtolist)
                    {
                        Storage_Check storage_Check = new Storage_Check();
                        string PreCheckID = "Check" + DateTime.Today.ToString("yyyyMMdd");
                        var storagecheck = storageCheckRepository.GetMany(m => m.Storage_Check_ID.StartsWith(PreCheckID)).ToList();
                        string PostCheckID = (storagecheck.Count() + 1).ToString("0000");
                        storage_Check.Storage_Check_ID = PreCheckID + PostCheckID;
                        storage_Check.Material_UID = item.Material_Uid;
                        storage_Check.Warehouse_Storage_UID = item.Warehouse_Storage_UID;
                        storage_Check.Check_Price = 0;
                        storage_Check.PartType_UID = item.PartType_UID;
                        var matinvent = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == item.Material_Uid
                                                         & m.Warehouse_Storage_UID == item.Warehouse_Storage_UID);
                        storage_Check.Old_Inventory_Qty = matinvent.Inventory_Qty;
                        storage_Check.Check_Qty = item.Check_Qty;
                        storage_Check.Applicant_UID = item.Approver_UID;
                        storage_Check.Applicant_Date = DateTime.Now;

                        storage_Check.Status_UID = 407;
                        if (storage_Check.Old_Inventory_Qty > storage_Check.Check_Qty)

                            storage_Check.Check_Status_UID = 422;
                        else if (storage_Check.Old_Inventory_Qty < storage_Check.Check_Qty)
                            storage_Check.Check_Status_UID = 421;
                        else
                            storage_Check.Check_Status_UID = 423;
                        storage_Check.Storage_InOut_UID = 0;
                        storage_Check.Approver_UID = item.Approver_UID;
                        storage_Check.Approver_Date = DateTime.Now;
                        storageCheckRepository.Add(storage_Check);

                        unitOfWork.Commit();
                    }
               
                    return "";

                }
                else
                {
                    return "没有异常原因数据！";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public List<StorageCheckDTO> DoAllExportStorageCheckReprot(StorageCheckDTO searchModel)
        {
            return storageCheckRepository.DoAllExportStorageCheckReprot(searchModel);
        }
        public List<StorageCheckDTO> DoExportStorageCheckReprot(string uids)
        {
            return storageCheckRepository.DoExportStorageCheckReprot(uids);
        }
        public List<StorageCheckDTO> DownloadStorageCheck(int PartType_UID, string Material_Id, string Material_Name, string Material_Types, string Warehouse_ID, string Rack_ID, string Storage_ID, int Plant_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            return storageCheckRepository.DownloadStorageCheck(PartType_UID, Material_Id, Material_Name, Material_Types, Warehouse_ID, Rack_ID, Storage_ID,  Plant_UID,  BG_Organization_UID,  FunPlant_Organization_UID);
        }

        public string AddOrEditStorageCheck(StorageCheckDTO dto)
        {
            try
            {
                if (dto.Storage_Check_UID == 0)
                {
                    Storage_Check SC = new Storage_Check();
                    string PreCheckID = "Check" + DateTime.Today.ToString("yyyyMMdd");
                    var storagecheck = storageCheckRepository.GetMany(m => m.Storage_Check_ID.StartsWith(PreCheckID)).ToList();
                    string PostCheckID = (storagecheck.Count() + 1).ToString("0000");
                    SC.Storage_Check_ID = PreCheckID + PostCheckID;
                    SC.Material_UID = dto.Material_Uid;
                    SC.Warehouse_Storage_UID = dto.Warehouse_Storage_UID;
                    SC.Check_Price = 0;
                    SC.PartType_UID = dto.PartType_UID;
                    var matinvent = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == dto.Material_Uid
                                                     & m.Warehouse_Storage_UID == dto.Warehouse_Storage_UID);
                    SC.Old_Inventory_Qty = matinvent.Inventory_Qty;
                    SC.Check_Qty = dto.Check_Qty;
                    SC.Applicant_UID = dto.Applicant_UID;
                    SC.Applicant_Date = DateTime.Now;
                    //fky2017/11/13
                    //SC.Status_UID = 374;
                    SC.Status_UID = 407;
                    if (SC.Old_Inventory_Qty > SC.Check_Qty)
                        //fky2017/11/13
                        // SC.Check_Status_UID = 396;
                        SC.Check_Status_UID = 422;
                    else if (SC.Old_Inventory_Qty < SC.Check_Qty)
                        //fky2017/11/13
                        // SC.Check_Status_UID = 395;
                        SC.Check_Status_UID = 421;
                    else
                        //fky2017/11/13
                        // SC.Check_Status_UID = 397;
                        SC.Check_Status_UID = 423;
                    SC.Storage_InOut_UID = 0;
                    SC.Approver_UID = dto.Applicant_UID;
                    SC.Approver_Date = DateTime.Now;
                    storageCheckRepository.Add(SC);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var StorageCheck = storageCheckRepository.GetFirstOrDefault(m => m.Storage_Check_UID == dto.Storage_Check_UID);
                    StorageCheck.Material_UID = dto.Material_Uid;
                    StorageCheck.Warehouse_Storage_UID = dto.Warehouse_Storage_UID;
                    StorageCheck.Applicant_UID = dto.Applicant_UID;
                    StorageCheck.Applicant_Date = DateTime.Now;
                    StorageCheck.PartType_UID = dto.PartType_UID;
                    var matinvent = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == dto.Material_Uid
                                 & m.Warehouse_Storage_UID == dto.Warehouse_Storage_UID);
                    StorageCheck.Old_Inventory_Qty = matinvent.Inventory_Qty;
                    StorageCheck.Check_Qty = dto.Check_Qty;
                    if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                        //fky2017/11/13
                        //StorageCheck.Check_Status_UID = 396;
                        StorageCheck.Check_Status_UID = 422;
                    else if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)
                        //fky2017/11/13
                        // StorageCheck.Check_Status_UID = 395;
                        StorageCheck.Check_Status_UID = 421;
                    else
                        //fky2017/11/13
                        //StorageCheck.Check_Status_UID = 397;
                        StorageCheck.Check_Status_UID = 423;
                    storageCheckRepository.Update(StorageCheck);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "盘点单申请失败:" + e.Message;
            }
        }

        public List<StorageCheckDTO> QueryStorageCheckSingle(int Storage_Check_UID)
        {
            var bud = storageCheckRepository.GetByUid(Storage_Check_UID).ToList();
            return bud;
        }

        public string ApproveStCheck(int Storage_Check_UID, int Useruid)
        {
            try
            {
                var StorageCheck = storageCheckRepository.GetById(Storage_Check_UID);
                //fky2017/11/13
                // StorageCheck.Status_UID = 376;
                StorageCheck.Status_UID = 408;
                StorageCheck.Approver_Date = DateTime.Now;
                StorageCheck.Approver_UID = Useruid;
                var matinvent = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == StorageCheck.Material_UID
                                                & m.Warehouse_Storage_UID == StorageCheck.Warehouse_Storage_UID);
                StorageCheck.Old_Inventory_Qty = matinvent.Inventory_Qty;
                if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                    //fky2017/11/13
                    // StorageCheck.Check_Status_UID = 396;
                    StorageCheck.Check_Status_UID = 422;
                else if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)
                    //fky2017/11/13
                    //StorageCheck.Check_Status_UID = 395;
                    StorageCheck.Check_Status_UID = 421;
                else
                    //fky2017/11/13
                    //StorageCheck.Check_Status_UID = 397;
                    StorageCheck.Check_Status_UID = 423;
                #region  修改出/入库表
                if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                {
                    Storage_Outbound_M SOM = new Storage_Outbound_M();
                    //fky2017/11/13
                    //SOM.Storage_Outbound_Type_UID= 389;
                    SOM.Storage_Outbound_Type_UID = 419;

                    SOM.Storage_Outbound_ID = StorageCheck.Storage_Check_ID;
                    SOM.Desc = "";
                    SOM.Applicant_UID = Useruid;
                    SOM.Applicant_Date = DateTime.Now;
                    //fky2017/11/13
                    // SOM.Status_UID = 376;
                    SOM.Status_UID = 408;

                    SOM.Approver_UID = Useruid;
                    SOM.Approver_Date = DateTime.Now;
                    storageOutboundMRepository.Add(SOM);
                    unitOfWork.Commit();

                    Storage_Outbound_D SOD = new Storage_Outbound_D();
                    SOD.Outbound_Price = 0;
                    SOD.Storage_Outbound_M_UID = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_ID == StorageCheck.Storage_Check_ID).Storage_Outbound_M_UID;
                    SOD.Material_UID = StorageCheck.Material_UID;
                    SOD.Warehouse_Storage_UID = StorageCheck.Warehouse_Storage_UID;
                    SOD.Outbound_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                    SOD.PartType_UID = StorageCheck.PartType_UID;
                    storageOutboundDRepository.Add(SOD);
                    SOD.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SOM.Storage_Outbound_Type_UID).Enum_Value;
                    SOD.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SOD.PartType_UID).Enum_Value;
                    unitOfWork.Commit();
                    var outboundD = storageOutboundDRepository.GetFirstOrDefault(m => m.Storage_Outbound_M_UID == SOD.Storage_Outbound_M_UID);
                    StorageCheck.Storage_InOut_UID = outboundD.Storage_Outbound_D_UID;
                }
                else
                {
                    Storage_Inbound SI = new Storage_Inbound();
                    //fky2017/11/13
                    //SI.Storage_Inbound_Type_UID = 384;
                    SI.Storage_Inbound_Type_UID = 414;
                    SI.Storage_Inbound_ID = StorageCheck.Storage_Check_ID;
                    SI.Material_UID = StorageCheck.Material_UID;
                    SI.Warehouse_Storage_UID = StorageCheck.Warehouse_Storage_UID;
                    SI.Inbound_Price = 0;
                    SI.PartType_UID = StorageCheck.PartType_UID;
                    SI.PU_NO = "None";
                    SI.PU_Qty = 0;
                    SI.Issue_NO = "None";
                    SI.Be_Check_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                    SI.OK_Qty = SI.Be_Check_Qty;
                    SI.NG_Qty = 0;
                    SI.Applicant_UID = Useruid;
                    SI.Applicant_Date = DateTime.Now;
                    //fky2017/11/13
                    // SI.Status_UID = 376;
                    SI.Status_UID = 408;
                    SI.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SI.Storage_Inbound_Type_UID).Enum_Value;
                    SI.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SI.PartType_UID).Enum_Value;
                    SI.Approver_UID = Useruid;
                    SI.Approver_Date = DateTime.Now;
                    storageInboundRepository.Add(SI);
                    unitOfWork.Commit();
                    var newinbound = storageInboundRepository.GetFirstOrDefault(m => m.Storage_Inbound_ID == SI.Storage_Inbound_ID);
                    StorageCheck.Storage_InOut_UID = newinbound.Storage_Inbound_UID;
                }

                #endregion  修改出/入库表
                storageCheckRepository.Update(StorageCheck);
                unitOfWork.Commit();
                Storage_Outbound_M storageoutboundM = new Storage_Outbound_M();
                Storage_Outbound_D storageoutboundD = new Storage_Outbound_D();
                Storage_Inbound inbound = new Storage_Inbound();
                if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                {
                    storageoutboundM = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_ID == StorageCheck.Storage_Check_ID);
                    storageoutboundD = storageOutboundDRepository.GetFirstOrDefault(m => m.Storage_Outbound_M_UID == storageoutboundM.Storage_Outbound_M_UID);
                }
                else
                {
                    inbound = storageInboundRepository.GetFirstOrDefault(m => m.Storage_Inbound_ID == StorageCheck.Storage_Check_ID);
                }
                #region 修改出入库明细表
                decimal balanceqty = 0;
                var matinventory = materialInventoryRepository.GetMany(m => m.Material_UID == StorageCheck.Material_UID);
                if (matinventory.Count() > 0)
                    balanceqty = matinventory.Sum(m => m.Inventory_Qty);
                Storage_InOut_Detail STD = new Storage_InOut_Detail();
                if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                {
                    //fky2017/11/13
                    //STD.InOut_Type_UID = 401;
                    STD.InOut_Type_UID = 426;
                    STD.Storage_InOut_UID = storageoutboundD.Storage_Outbound_D_UID;
                    STD.Desc = storageoutboundD.Desc;
                    STD.InOut_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                    STD.Balance_Qty = balanceqty - STD.InOut_Qty;
                }
                else
                {
                    //fky2017/11/13
                    //STD.InOut_Type_UID = 399;
                    STD.InOut_Type_UID = 424;
                    STD.Storage_InOut_UID = inbound.Storage_Inbound_UID;
                    STD.Desc = inbound.Desc;
                    STD.InOut_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                    STD.Balance_Qty = balanceqty + STD.InOut_Qty;
                }
                STD.Material_UID = StorageCheck.Material_UID;
                STD.Warehouse_Storage_UID = StorageCheck.Warehouse_Storage_UID;
                STD.InOut_Date = DateTime.Now;
                STD.UnitPrice = 0;
                STD.Modified_UID = Useruid;
                STD.Modified_Date = DateTime.Now;
                storageInOutDetailRepository.Add(STD);
                unitOfWork.Commit();
                #endregion

                #region  修改料号库存明细表

                var matinven = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == StorageCheck.Material_UID & m.Warehouse_Storage_UID == StorageCheck.Warehouse_Storage_UID);
                matinven.Inventory_Qty = StorageCheck.Check_Qty;
                if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                {
                    matinven.Desc = storageoutboundD.Desc;
                }
                else
                {
                    matinven.Desc = inbound.Desc;
                }
                matinven.Modified_Date = DateTime.Now;
                matinven.Modified_UID = Useruid;

                materialInventoryRepository.Update(matinven);

                # endregion 修改料号库存明细表

                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }
        public string ApproveStorageCheck(StorageCheckDTO dto)
        {
            try
            {
                List<StorageCheckDTO> StorageInboundDTOs = storageCheckRepository.DoAllExportStorageCheckReprot(dto).Where(o => o.Status == "未审核").ToList();
                List<Storage_Check> StorageCheckAll = storageCheckRepository.GetAll().ToList();
                List<Enumeration> enumerationAll = enumerationRepository.GetAll().ToList();
                if (StorageInboundDTOs != null && StorageInboundDTOs.Count() > 0)
                {
                    foreach (var item in StorageInboundDTOs)
                    {
                        var StorageCheck = StorageCheckAll.FirstOrDefault(o => o.Storage_Check_UID == item.Storage_Check_UID);
                        StorageCheck.Status_UID = 408;
                        StorageCheck.Approver_Date = DateTime.Now;
                        StorageCheck.Approver_UID = dto.Approver_UID;
                        var matinvent = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == StorageCheck.Material_UID
                                                        & m.Warehouse_Storage_UID == StorageCheck.Warehouse_Storage_UID);
                        StorageCheck.Old_Inventory_Qty = matinvent.Inventory_Qty;
                        if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)

                            StorageCheck.Check_Status_UID = 422;
                        else if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)

                            StorageCheck.Check_Status_UID = 421;
                        else
                            StorageCheck.Check_Status_UID = 423;
                        #region  修改出/入库表
                        if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                        {
                            Storage_Outbound_M SOM = new Storage_Outbound_M();
                            SOM.Storage_Outbound_Type_UID = 419;
                            SOM.Storage_Outbound_ID = StorageCheck.Storage_Check_ID;
                            SOM.Desc = "";
                            SOM.Applicant_UID = dto.Approver_UID;
                            SOM.Applicant_Date = DateTime.Now;
                            SOM.Status_UID = 408;
                            SOM.Approver_UID = dto.Approver_UID;
                            SOM.Approver_Date = DateTime.Now;
                            storageOutboundMRepository.Add(SOM);
                            unitOfWork.Commit();

                            Storage_Outbound_D SOD = new Storage_Outbound_D();
                            SOD.Outbound_Price = 0;
                            SOD.Storage_Outbound_M_UID = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_ID == StorageCheck.Storage_Check_ID).Storage_Outbound_M_UID;
                            SOD.Material_UID = StorageCheck.Material_UID;
                            SOD.Warehouse_Storage_UID = StorageCheck.Warehouse_Storage_UID;
                            SOD.Outbound_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                            SOD.PartType_UID = StorageCheck.PartType_UID;
                            storageOutboundDRepository.Add(SOD);
                            SOD.Desc = enumerationAll.FirstOrDefault(m => m.Enum_UID == SOM.Storage_Outbound_Type_UID).Enum_Value;
                            SOD.Desc += "-" + enumerationAll.FirstOrDefault(m => m.Enum_UID == SOD.PartType_UID).Enum_Value;
                            unitOfWork.Commit();

                            var outboundD = storageOutboundDRepository.GetFirstOrDefault(m => m.Storage_Outbound_M_UID == SOD.Storage_Outbound_M_UID);
                            StorageCheck.Storage_InOut_UID = outboundD.Storage_Outbound_D_UID;
                        }
                        else
                        {
                            Storage_Inbound SI = new Storage_Inbound();

                            SI.Storage_Inbound_Type_UID = 414;
                            SI.Storage_Inbound_ID = StorageCheck.Storage_Check_ID;
                            SI.Material_UID = StorageCheck.Material_UID;
                            SI.Warehouse_Storage_UID = StorageCheck.Warehouse_Storage_UID;
                            SI.Inbound_Price = 0;
                            SI.PartType_UID = StorageCheck.PartType_UID;
                            SI.PU_NO = "None";
                            SI.PU_Qty = 0;
                            SI.Issue_NO = "None";
                            SI.Be_Check_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                            SI.OK_Qty = SI.Be_Check_Qty;
                            SI.NG_Qty = 0;
                            SI.Applicant_UID = dto.Approver_UID;
                            SI.Applicant_Date = DateTime.Now;
                            SI.Status_UID = 408;
                            SI.Desc = enumerationAll.FirstOrDefault(m => m.Enum_UID == SI.Storage_Inbound_Type_UID).Enum_Value;
                            SI.Desc += "-" + enumerationAll.FirstOrDefault(m => m.Enum_UID == SI.PartType_UID).Enum_Value;
                            SI.Approver_UID = dto.Approver_UID;
                            SI.Approver_Date = DateTime.Now;
                            storageInboundRepository.Add(SI);
                            unitOfWork.Commit();
                            var newinbound = storageInboundRepository.GetFirstOrDefault(m => m.Storage_Inbound_ID == SI.Storage_Inbound_ID);
                            StorageCheck.Storage_InOut_UID = newinbound.Storage_Inbound_UID;
                        }

                        #endregion  修改出/入库表
                        storageCheckRepository.Update(StorageCheck);
                        unitOfWork.Commit();
                        Storage_Outbound_M storageoutboundM = new Storage_Outbound_M();
                        Storage_Outbound_D storageoutboundD = new Storage_Outbound_D();
                        Storage_Inbound inbound = new Storage_Inbound();
                        if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                        {
                            storageoutboundM = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_ID == StorageCheck.Storage_Check_ID);
                            storageoutboundD = storageOutboundDRepository.GetFirstOrDefault(m => m.Storage_Outbound_M_UID == storageoutboundM.Storage_Outbound_M_UID);
                        }
                        else
                        {
                            inbound = storageInboundRepository.GetFirstOrDefault(m => m.Storage_Inbound_ID == StorageCheck.Storage_Check_ID);
                        }
                        #region 修改出入库明细表
                        decimal balanceqty = 0;
                        var matinventory = materialInventoryRepository.GetMany(m => m.Material_UID == StorageCheck.Material_UID);
                        if (matinventory.Count() > 0)
                            balanceqty = matinventory.Sum(m => m.Inventory_Qty);
                        Storage_InOut_Detail STD = new Storage_InOut_Detail();
                        if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                        {

                            STD.InOut_Type_UID = 426;
                            STD.Storage_InOut_UID = storageoutboundD.Storage_Outbound_D_UID;
                            STD.Desc = storageoutboundD.Desc;
                            STD.InOut_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                            STD.Balance_Qty = balanceqty - STD.InOut_Qty;
                        }
                        else
                        {

                            STD.InOut_Type_UID = 424;
                            STD.Storage_InOut_UID = inbound.Storage_Inbound_UID;
                            STD.Desc = inbound.Desc;
                            STD.InOut_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                            STD.Balance_Qty = balanceqty + STD.InOut_Qty;
                        }
                        STD.Material_UID = StorageCheck.Material_UID;
                        STD.Warehouse_Storage_UID = StorageCheck.Warehouse_Storage_UID;
                        STD.InOut_Date = DateTime.Now;
                        STD.UnitPrice = 0;
                        STD.Modified_UID = dto.Approver_UID;
                        STD.Modified_Date = DateTime.Now;
                        storageInOutDetailRepository.Add(STD);
                        unitOfWork.Commit();
                        #endregion

                        #region  修改料号库存明细表

                        var matinven = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == StorageCheck.Material_UID & m.Warehouse_Storage_UID == StorageCheck.Warehouse_Storage_UID);
                        matinven.Inventory_Qty = StorageCheck.Check_Qty;
                        if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                        {
                            matinven.Desc = storageoutboundD.Desc;
                        }
                        else
                        {
                            matinven.Desc = inbound.Desc;
                        }
                        matinven.Modified_Date = DateTime.Now;
                        matinven.Modified_UID = dto.Approver_UID;
                        materialInventoryRepository.Update(matinven);
                        unitOfWork.Commit();
                        #endregion 修改料号库存明细表
                    }

                }
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }
        public string DeleteStorageCheck(int Storage_Check_UID, int userid)
        {
            try
            {
                var storagecheck = storageCheckRepository.GetById(Storage_Check_UID);
                storagecheck.Applicant_UID = userid;
                storagecheck.Applicant_Date = DateTime.Now;
                //fky2017/11/13
                //storagecheck.Status_UID = 392 ;
                storagecheck.Status_UID = 420;
                storageCheckRepository.Update(storagecheck);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除盘点单失败:" + e.Message;
            }

        }
        #endregion 盘点 
        #region 出入库明细查询

        public List<StorageDetailDTO> DoExportStorageDetailReprot(string uids)
        {

            return storageInOutDetailRepository.DoExportStorageDetailReprot(uids);


        }
        public List<StorageDetailDTO> DoAllExportStorageDetailReprot(StorageSearchMod search)
        {


            return storageInOutDetailRepository.DoAllExportStorageDetailReprot(search);

        }

        public PagedListModel<StorageDetailDTO> QueryStorageDetails(StorageSearchMod searchModel, Page page)
        {
            int totalcount;
            var result = storageInOutDetailRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<StorageDetailDTO>(totalcount, result);
            return bd;
        }
        #endregion 出入库明细查询
        #region 储位移转 

        public PagedListModel<StorageTransferDTO> QueryStorageTransfers(StorageTransferDTO searchModel, Page page)
        {
            int totalcount;
            var result = storageTransferRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<StorageTransferDTO>(totalcount, result);
            return bd;
        }


        public List<StorageTransferDTO> DoExportStorageTransferReprot(string uids)
        {
            return storageTransferRepository.DoExportStorageTransferReprot(uids);
        }
        public List<StorageTransferDTO> DoAllExportStorageTransferReprot(StorageTransferDTO search)
        {
            return storageTransferRepository.DoAllExportStorageTransferReprot(search);
        }
        public string AddOrEditStorageTransfer(StorageTransferDTO dto)
        {
            try
            {
                if (dto.In_Warehouse_Storage_UID == dto.Out_Warehouse_Storage_UID)
                {
                    return "转入储位与转出储位不可一致";
                }
                if (dto.Storage_Transfer_UID == 0)
                {
                    var matinventory = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == dto.Material_Uid & m.Warehouse_Storage_UID == dto.Out_Warehouse_Storage_UID);
                    if (dto.Transfer_Qty > matinventory.Inventory_Qty)
                    {
                        return "转出储位数量(" + matinventory.Inventory_Qty + ")不足";
                    }
                    Storage_Transfer ST = new Storage_Transfer();
                    string PreTransferID = "Transfer" + DateTime.Today.ToString("yyyyMMdd");
                    var storagetransfer = storageTransferRepository.GetMany(m => m.Storage_Transfer_ID.StartsWith(PreTransferID)).ToList();
                    string PostTransferID = (storagetransfer.Count() + 1).ToString("0000");
                    ST.Storage_Transfer_ID = PreTransferID + PostTransferID;
                    ST.Material_UID = dto.Material_Uid;
                    ST.Out_Warehouse_Storage_UID = dto.Out_Warehouse_Storage_UID;
                    ST.In_Warehouse_Storage_UID = dto.In_Warehouse_Storage_UID;
                    ST.Transfer_Price = 0;
                    ST.PartType_UID = dto.PartType_UID;
                    ST.Transfer_Qty = dto.Transfer_Qty;
                    ST.Applicant_UID = dto.Applicant_UID;
                    ST.Applicant_Date = DateTime.Now;

                    //fky2017/11/13
                    //ST.Status_UID = 374;
                    ST.Status_UID = 407;

                    ST.Approver_UID = dto.Applicant_UID;
                    ST.Approver_Date = DateTime.Now;
                    storageTransferRepository.Add(ST);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var StorageTransfer = storageTransferRepository.GetFirstOrDefault(m => m.Storage_Transfer_UID == dto.Storage_Transfer_UID);
                    StorageTransfer.Material_UID = dto.Material_Uid;
                    StorageTransfer.Out_Warehouse_Storage_UID = dto.Out_Warehouse_Storage_UID;
                    StorageTransfer.In_Warehouse_Storage_UID = dto.In_Warehouse_Storage_UID;
                    StorageTransfer.Applicant_UID = dto.Applicant_UID;
                    StorageTransfer.Applicant_Date = DateTime.Now;
                    StorageTransfer.PartType_UID = dto.PartType_UID;
                    StorageTransfer.Transfer_Qty = dto.Transfer_Qty;
                    StorageTransfer.Approver_UID = dto.Applicant_UID;
                    StorageTransfer.Approver_Date = DateTime.Now;
                    storageTransferRepository.Update(StorageTransfer);
                    unitOfWork.Commit();
                    return "0";
                }
            }
            catch (Exception e)
            {
                return "储位移转失败:" + e.Message;
            }
        }
        public List<Material_Inventory> GetMaterial_InventoryAll()
        {
            return materialInventoryRepository.GetAll().ToList();
        }

        public string ImportStorageTransfer(List<StorageTransferDTO> dtolist)
        {
            try
            {

                if (dtolist.Count > 0)
                {
                    var matinventorys = materialInventoryRepository.GetAll();

                    foreach (var item in dtolist)
                    {
                        if (item.In_Warehouse_Storage_UID == item.Out_Warehouse_Storage_UID)
                        {
                            return "转入储位与转出储位不可一致";
                        }
                        if (item.Storage_Transfer_UID == 0)
                        {
                            var matinventory = matinventorys.FirstOrDefault(m => m.Material_UID == item.Material_Uid & m.Warehouse_Storage_UID == item.Out_Warehouse_Storage_UID);
                            if (item.Transfer_Qty > matinventory.Inventory_Qty)
                            {
                                return "转出储位数量(" + matinventory.Inventory_Qty + ")不足";
                            }
                            Storage_Transfer ST = new Storage_Transfer();
                            string PreTransferID = "Transfer" + DateTime.Today.ToString("yyyyMMdd");
                            var storagetransfer = storageTransferRepository.GetMany(m => m.Storage_Transfer_ID.StartsWith(PreTransferID)).ToList();
                            string PostTransferID = (storagetransfer.Count() + 1).ToString("0000");
                            ST.Storage_Transfer_ID = PreTransferID + PostTransferID;
                            ST.Material_UID = item.Material_Uid;
                            ST.Out_Warehouse_Storage_UID = item.Out_Warehouse_Storage_UID;
                            ST.In_Warehouse_Storage_UID = item.In_Warehouse_Storage_UID;
                            ST.Transfer_Price = 0;
                            ST.PartType_UID = item.PartType_UID;
                            ST.Transfer_Qty = item.Transfer_Qty;
                            ST.Applicant_UID = item.Applicant_UID;
                            ST.Applicant_Date = DateTime.Now;
                            //fky2017/11/13
                            //ST.Status_UID = 374;
                            ST.Status_UID = 407;
                            ST.Approver_UID = item.Applicant_UID;
                            ST.Approver_Date = DateTime.Now;
                            storageTransferRepository.Add(ST);
                            unitOfWork.Commit();
                        }
                    }
              
                    return "";
                }
                return "";
            }
            catch (Exception e)
            {
                return "储位移转失败:" + e.Message;
            }
        }
        public List<StorageTransferDTO> QueryStorageTransferSingle(int Storage_Transfer_UID)
        {
            var bud = storageTransferRepository.GetByUid(Storage_Transfer_UID).ToList();
            return bud;
        }

        public string ApproveStTransfer(int Storage_Transfer_UID, int Useruid)
        {
            try
            {
                var StorageTransfer = storageTransferRepository.GetById(Storage_Transfer_UID);
                //fky2017/11/13
                //StorageTransfer.Status_UID = 376;
                StorageTransfer.Status_UID = 408;
                StorageTransfer.Approver_Date = DateTime.Now;
                StorageTransfer.Approver_UID = Useruid;

                storageTransferRepository.Update(StorageTransfer);

                #region  修改出&入库表
                Storage_Outbound_M SOM = new Storage_Outbound_M();
                //fky2017/11/13
                //SOM.Storage_Outbound_Type_UID = 405;
                SOM.Storage_Outbound_Type_UID = 430;
                SOM.Storage_Outbound_ID = StorageTransfer.Storage_Transfer_ID;
                SOM.Applicant_UID = Useruid;
                SOM.Applicant_Date = DateTime.Now;
                //fky2017/11/13
                //SOM.Status_UID = 376;
                SOM.Status_UID = 408;
                SOM.Approver_UID = Useruid;
                SOM.Approver_Date = DateTime.Now;
                SOM.Desc = "";
                storageOutboundMRepository.Add(SOM);
                unitOfWork.Commit();

                Storage_Outbound_D SOD = new Storage_Outbound_D();
                SOD.Storage_Outbound_M_UID = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_ID == StorageTransfer.Storage_Transfer_ID).Storage_Outbound_M_UID;
                SOD.Material_UID = StorageTransfer.Material_UID;
                SOD.Warehouse_Storage_UID = StorageTransfer.Out_Warehouse_Storage_UID;
                SOD.Outbound_Qty = StorageTransfer.Transfer_Qty;
                SOD.PartType_UID = StorageTransfer.PartType_UID;
                SOD.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SOM.Storage_Outbound_Type_UID).Enum_Value;
                SOD.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SOD.PartType_UID).Enum_Value;
                SOD.Outbound_Price = 0;
                storageOutboundDRepository.Add(SOD);

                Storage_Inbound SI = new Storage_Inbound();
                //fky2017/11/13
                //SI.Storage_Inbound_Type_UID = 406;
                SI.Storage_Inbound_Type_UID = 431;
                SI.Storage_Inbound_ID = StorageTransfer.Storage_Transfer_ID;
                SI.Material_UID = StorageTransfer.Material_UID;
                SI.Warehouse_Storage_UID = StorageTransfer.In_Warehouse_Storage_UID;
                SI.Inbound_Price = 0;
                SI.PartType_UID = StorageTransfer.PartType_UID;
                SI.PU_NO = "None";
                SI.PU_Qty = 0;
                SI.Issue_NO = "None";
                SI.Be_Check_Qty = StorageTransfer.Transfer_Qty;
                SI.OK_Qty = SI.Be_Check_Qty;
                SI.NG_Qty = 0;
                SI.Applicant_UID = Useruid;
                SI.Applicant_Date = DateTime.Now;
                //fky2017/11/13
                //SI.Status_UID = 376;
                SI.Status_UID = 408;
                SI.Desc = enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SI.Storage_Inbound_Type_UID).Enum_Value;
                SI.Desc += "-" + enumerationRepository.GetFirstOrDefault(m => m.Enum_UID == SI.PartType_UID).Enum_Value;
                SI.Approver_UID = Useruid;
                SI.Approver_Date = DateTime.Now;
                storageInboundRepository.Add(SI);
                unitOfWork.Commit();

                #endregion  修改出&入库表

                #region 修改出入库明细表
                decimal balanceqty = 0;
                var matinventory = materialInventoryRepository.GetMany(m => m.Material_UID == StorageTransfer.Material_UID);
                if (matinventory.Count() > 0)
                    balanceqty = matinventory.Sum(m => m.Inventory_Qty);

                Storage_InOut_Detail OUTSTD = new Storage_InOut_Detail();
                var storageoutboundM = storageOutboundMRepository.GetFirstOrDefault(m => m.Storage_Outbound_ID == StorageTransfer.Storage_Transfer_ID);
                var storageoutboundD = storageOutboundDRepository.GetFirstOrDefault(m => m.Storage_Outbound_M_UID == storageoutboundM.Storage_Outbound_M_UID);
                //fky2017/11/13
                //OUTSTD.InOut_Type_UID = 404;
                OUTSTD.InOut_Type_UID = 429;
                OUTSTD.Storage_InOut_UID = storageoutboundD.Storage_Outbound_D_UID;
                OUTSTD.Desc = storageoutboundD.Desc;
                OUTSTD.Material_UID = StorageTransfer.Material_UID;
                OUTSTD.Warehouse_Storage_UID = StorageTransfer.Out_Warehouse_Storage_UID;
                OUTSTD.InOut_Date = DateTime.Now;
                OUTSTD.InOut_Qty = StorageTransfer.Transfer_Qty;
                OUTSTD.Balance_Qty = balanceqty - OUTSTD.InOut_Qty;
                OUTSTD.UnitPrice = 0;
                OUTSTD.Modified_UID = Useruid;
                OUTSTD.Modified_Date = DateTime.Now;
                storageInOutDetailRepository.Add(OUTSTD);

                Storage_InOut_Detail INSTD = new Storage_InOut_Detail();
                var storageinbound = storageInboundRepository.GetFirstOrDefault(m => m.Storage_Inbound_ID == StorageTransfer.Storage_Transfer_ID);
                //fky2017/11/13
                // INSTD.InOut_Type_UID = 403;
                INSTD.InOut_Type_UID = 428;
                INSTD.Storage_InOut_UID = storageinbound.Storage_Inbound_UID;
                INSTD.Desc = storageinbound.Desc;
                INSTD.Material_UID = StorageTransfer.Material_UID;
                INSTD.Warehouse_Storage_UID = StorageTransfer.In_Warehouse_Storage_UID;
                INSTD.InOut_Date = DateTime.Now;
                INSTD.InOut_Qty = StorageTransfer.Transfer_Qty;
                INSTD.Balance_Qty = balanceqty;
                INSTD.UnitPrice = 0;
                INSTD.Modified_UID = Useruid;
                INSTD.Modified_Date = DateTime.Now;
                storageInOutDetailRepository.Add(INSTD);

                #endregion

                #region  修改料号库存明细表

                var outmatinven = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == StorageTransfer.Material_UID & m.Warehouse_Storage_UID == StorageTransfer.Out_Warehouse_Storage_UID);
                outmatinven.Inventory_Qty = outmatinven.Inventory_Qty - StorageTransfer.Transfer_Qty;
                outmatinven.Desc = storageoutboundD.Desc;
                outmatinven.Modified_Date = DateTime.Now;
                outmatinven.Modified_UID = Useruid;
                materialInventoryRepository.Update(outmatinven);

                var inmatinven = materialInventoryRepository.GetFirstOrDefault(m => m.Material_UID == StorageTransfer.Material_UID & m.Warehouse_Storage_UID == StorageTransfer.In_Warehouse_Storage_UID);
                if (inmatinven != null)
                {
                    inmatinven.Inventory_Qty = inmatinven.Inventory_Qty + StorageTransfer.Transfer_Qty;
                    inmatinven.Desc = storageinbound.Desc;
                    inmatinven.Modified_Date = DateTime.Now;
                    inmatinven.Modified_UID = Useruid;
                    materialInventoryRepository.Update(inmatinven);
                }
                else
                {
                    Material_Inventory MI = new Material_Inventory();
                    MI.Material_UID = StorageTransfer.Material_UID;
                    MI.Warehouse_Storage_UID = StorageTransfer.In_Warehouse_Storage_UID;
                    MI.Inventory_Qty = StorageTransfer.Transfer_Qty;
                    MI.Desc = storageinbound.Desc;
                    MI.Modified_UID = Useruid;
                    MI.Modified_Date = DateTime.Now;
                    materialInventoryRepository.Add(MI);
                }
                # endregion 修改料号库存明细表

                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        public string DeleteStorageTransfer(int Storage_Transfer_UID, int userid)
        {
            try
            {
                var storagetransfer = storageTransferRepository.GetById(Storage_Transfer_UID);
                storagetransfer.Applicant_UID = userid;
                storagetransfer.Applicant_Date = DateTime.Now;
                //fky2017/11/13
                //storagetransfer.Status_UID = 392;
                storagetransfer.Status_UID = 420;
                storageTransferRepository.Update(storagetransfer);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除储位移转单失败:" + e.Message;
            }
        }

        #endregion 储位移转
        #region 一般需求计算

        public PagedListModel<MaterialNormalDemandDTO> QueryMaterialNormalDemands(MaterialNormalDemandDTO searchModel, Page page)
        {
            int totalcount;
            var result = materialNormalDemandRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialNormalDemandDTO>(totalcount, result);
            return bd;
        }

        public string AddMatNormalDemand(MaterialNormalDemandDTO dto)
        {
            try
            {
                List<EQP_Type> eqptypelist = new List<EQP_Type>();
                if (dto.FunPlant_Organization_UID == 0)
                {
                    eqptypelist = eQP_TypeRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                  ).ToList();

                }
                else
                {
                    eqptypelist = eQP_TypeRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                   & m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID).ToList();
                }
                List<Material_Normal_Demand> olddate = new List<Material_Normal_Demand>();
                if (dto.FunPlant_Organization_UID == 0)
                {

                    olddate = materialNormalDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                       & m.Demand_Date == dto.Demand_Date).ToList();

                }
                else
                {

                    olddate = materialNormalDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                       & m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID & m.Demand_Date == dto.Demand_Date).ToList();

                }


                //fky2017/11/13
                //var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 412);
                var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 437);
                //fky2017/11/13
                // var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 415);
                var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 440);
                if (okolddata1 != null)
                    return "此功能厂且此需求日期的需求已经审核,请先退审";
                else if (okolddata2 != null)
                    return "此功能厂且此需求日期的需求已经采购,不可重新计算";
                else
                {
                    foreach (var item in olddate.ToList())
                    {
                        materialNormalDemandRepository.Delete(item);
                    }
                    unitOfWork.Commit();
                }

                DateTime currentDate = DateTime.Now;
                if (eqptypelist.Count == 0)
                {
                    return "该BG下未设定任何机台型号,无法计算";
                }
                foreach (EQP_Type eqptype in eqptypelist)
                {
                    var eqpmats = eQP_MaterialRepository.GetMany(m => m.EQP_Type_UID == eqptype.EQP_Type_UID);
                    //if (eqpmats.Count() == 0)
                    //{
                    //    return "未设定机台料号对照表,无法计算";
                    //}
                    if (eqpmats != null && eqpmats.Count() > 0)
                    {
                        foreach (EQP_Material eqpmat in eqpmats)
                        {
                            var mat = materialInfoRepository.GetFirstOrDefault(m => m.Material_Uid == eqpmat.Material_UID);
                            if (mat.Requisitions_Cycle == null || mat.Requisitions_Cycle == 0 || mat.Sign_days == null || mat.Sign_days == 0
                                || mat.Daily_Consumption == null || mat.Daily_Consumption == 0)
                            {
                                return "此BG下有料号的信息不完整,请至配置信息维护界面检查维护" + eqpmat.Material_Info.Material_Id;
                            }
                            //if (mat.Requisitions_Cycle == null)
                            //    return "请维护料号的请购周期";
                            //if (mat.Sign_days == null)
                            //    return "请维护料号的签核天数";
                            //if (mat.Daily_Consumption == null)
                            //    return "请维护料号的日用量";
                            decimal currentQty = 0;
                            var matinventory = materialInventoryRepository.GetMany(m => m.Material_UID == mat.Material_Uid);
                            if (matinventory != null && matinventory.Count() > 0)
                                currentQty = matinventory.Sum(m => m.Inventory_Qty);
                            Material_Normal_Demand MND = new Material_Normal_Demand();
                            MND.BG_Organization_UID = eqptype.BG_Organization_UID;
                            MND.FunPlant_Organization_UID = eqptype.FunPlant_Organization_UID;
                            MND.Material_UID = eqpmat.Material_UID;
                            MND.EQP_Type_UID = eqptype.EQP_Type_UID;
                            MND.Calculation_Date = currentDate;
                            MND.Demand_Date = dto.Demand_Date;
                            MND.Safe_Stock_Qty = CalSafeStockQty(mat.Material_Uid, (int)mat.Requisitions_Cycle, (int)mat.Sign_days, (decimal)mat.Daily_Consumption);
                            MND.Existing_Stock_Qty = currentQty;
                            MND.User_Adjustments_Qty = 0;
                            MND.Calculated_Demand_Qty = MND.Safe_Stock_Qty - MND.Existing_Stock_Qty;
                            MND.Applicant_UID = dto.Applicant_UID;
                            MND.Applicant_Date = currentDate;
                            //fky2017/11/13
                            // MND.Status_UID = 410;
                            MND.Status_UID = 435;
                            MND.Approver_UID = dto.Applicant_UID;
                            MND.Approver_Date = currentDate;
                            materialNormalDemandRepository.Add(MND);
                        }
                        unitOfWork.Commit();
                    }
                }

                return "";

            }
            catch (Exception e)
            {
                return "一般需求计算失败:" + e.Message;
            }
        }

        //计算安全库存
        int CalSafeStockQty(int Material_Uid, int Requisitions_Cycle, int Sign_days, decimal Daily_Consumption)
        {

            //三个月以后以下面的一句修改日用量
            //Daily_Consumption = GetDailyQty(Material_Uid);

            return Convert.ToInt32(Math.Round((Requisitions_Cycle + Sign_days) * Daily_Consumption, 0, MidpointRounding.AwayFromZero));
        }

        //计算日用量
        decimal GetDailyQty(int Material_Uid)
        {
            var outbound = storageOutboundDRepository.GetThreeMonthOut(Material_Uid);
            if (outbound == null || outbound.Count == 0)
                return 0;
            else
            {
                TimeSpan TS = DateTime.Today - DateTime.Today.AddMonths(-3);

                return outbound.Sum(m => m.Outbound_Qty) / TS.Days;
            }
        }

        public PagedListModel<MaterialNormalDemandDTO> QueryMaterialNormalDetails(MaterialNormalDemandDTO searchModel, Page page)
        {
            int totalcount;
            var matnd = materialNormalDemandRepository.GetFirstOrDefault(m => m.Material_Normal_Demand_UID == searchModel.Material_Normal_Demand_UID);
            searchModel.BG_Organization_UID = matnd.BG_Organization_UID;
            searchModel.Demand_Date = matnd.Demand_Date;
            searchModel.FunPlant_Organization_UID = matnd.FunPlant_Organization_UID;
            var result = materialNormalDemandRepository.GetDetailInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialNormalDemandDTO>(totalcount, result);
            return bd;
        }

        public string EditUserAdjustQty(matNDVM dto)
        {
            try
            {
                foreach (var item in dto.editList)
                {
                    var matND = materialNormalDemandRepository.GetById(item.Material_Normal_Demand_UID);
                    matND.Applicant_UID = dto.Modified_UID;
                    matND.Applicant_Date = DateTime.Now;
                    matND.Status_UID = dto.Status_UID;
                    matND.User_Adjustments_Qty = item.User_Adjustments_Qty;
                    matND.Approver_UID = dto.Modified_UID;
                    matND.Approver_Date = DateTime.Now;
                    materialNormalDemandRepository.Update(matND);
                    unitOfWork.Commit();
                }
                return "SUCCESS";
            }
            catch (Exception e)
            {
                //fky2017/11/13
                // if (dto.Status_UID==410)
                if (dto.Status_UID == 435)
                    return "保存一般需求失败:" + e.Message;
                else
                    return "提交一般需求失败:" + e.Message;
            }
        }

        public string ApproveMatND(int Material_Normal_Demand_UID, int userid)
        {
            try
            {
                var matND = materialNormalDemandRepository.GetById(Material_Normal_Demand_UID);
                var matNDList = materialNormalDemandRepository.GetMany(m => m.BG_Organization_UID == matND.BG_Organization_UID
                                                & m.Demand_Date == matND.Demand_Date & m.FunPlant_Organization_UID == matND.FunPlant_Organization_UID
                                                //fky2017/11/13
                                                // & m.Status_UID!=414).ToList();
                                                & m.Status_UID != 439).ToList();
                foreach (var item in matNDList)
                {
                    //fky2017/11/13
                    //item.Status_UID = 412;
                    item.Status_UID = 437;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialNormalDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核一般需求失败:" + e.Message;
            }
        }


        public string DisapproveMatND(int Material_Normal_Demand_UID, int userid)
        {
            try
            {
                var matND = materialNormalDemandRepository.GetById(Material_Normal_Demand_UID);
                var matNDList = materialNormalDemandRepository.GetMany(m => m.BG_Organization_UID == matND.BG_Organization_UID
                                                & m.Demand_Date == matND.Demand_Date & m.FunPlant_Organization_UID == matND.FunPlant_Organization_UID
                                                 //fky2017/11/13
                                                 //& m.Status_UID==414);
                                                 & m.Status_UID != 439);//已删除
                foreach (var item in matNDList)
                {
                    //fky2017/11/13
                    // item.Status_UID = 413;
                    item.Status_UID = 438;//审核取消
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialNormalDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核取消失败:" + e.Message;
            }
        }

        public List<MaterialNormalDemandDTO> DoExportFunction(int Material_Normal_Demand_UID)
        {
            var matND = materialNormalDemandRepository.GetFirstOrDefault(m => m.Material_Normal_Demand_UID == Material_Normal_Demand_UID);

            return materialNormalDemandRepository.DoExportFunction(matND);
        }

        public string DeleteMatNormalDemandList(int Material_Normal_Demand_UID, int userid)
        {
            try
            {
                var matND = materialNormalDemandRepository.GetById(Material_Normal_Demand_UID);
                var matNDList = materialNormalDemandRepository.GetMany(m => m.BG_Organization_UID == matND.BG_Organization_UID
                                                & m.Demand_Date == matND.Demand_Date & m.FunPlant_Organization_UID == matND.FunPlant_Organization_UID);
                foreach (var item in matNDList)
                {  //fky2017/11/13
                   // item.Status_UID = 414;
                    item.Status_UID = 439;
                    item.Applicant_UID = userid;
                    item.Applicant_Date = DateTime.Now;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialNormalDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        public string DeleteMatNormalDemand(int Material_Normal_Demand_UID, int userid)
        {
            try
            {
                var matND = materialNormalDemandRepository.GetById(Material_Normal_Demand_UID);
                //fky2017/11/13
                //matND.Status_UID = 414;
                matND.Status_UID = 439;
                matND.Applicant_UID = userid;
                matND.Applicant_Date = DateTime.Now;
                matND.Approver_UID = userid;
                matND.Approver_Date = DateTime.Now;
                materialNormalDemandRepository.Update(matND);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        /// <summary>
        /// 更新一般需求状态
        /// </summary>
        /// <param name="uid">Material_Normal_Demand.Material_Normal_Demand_UID</param>
        /// <param name="statusUID">Material_Normal_Demand.Status_UID(Enumeration.Enum_UID)</param>
        /// <returns></returns>
        public bool UpdateMaterialNormalDemandStatus(int uid, int statusUID)
        {
            try
            {
                var entity = materialNormalDemandRepository.GetFirstOrDefault(i => i.Material_Normal_Demand_UID == uid);
                var entityList = materialNormalDemandRepository.GetMany(i => i.BG_Organization_UID == entity.BG_Organization_UID && i.FunPlant_Organization_UID == entity.FunPlant_Organization_UID && i.Demand_Date == entity.Demand_Date);
                if (statusUID != 439)
                {
                    entityList = entityList.Where(i => i.Status_UID != 439);//439 已删除,审核的时候排除掉已删除的数据
                }
                foreach (var item in entityList)
                {
                    item.Status_UID = statusUID;
                    materialNormalDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion 一般需求计算   
        #region 库存明细查询
        public List<MaterialInventoryDTO> DoExportMaterialInventoryReprot(string uids)
        {

            return materialInventoryRepository.DoExportMaterialInventoryReprot(uids);


        }
        public List<MaterialInventoryDTO> DoAllExportMaterialInventoryReprot(MaterialInventoryDTO searchModel)
        {

            return materialInventoryRepository.DoAllExportMaterialInventoryReprot(searchModel);

        }

        public PagedListModel<MaterialInventoryDTO> QueryMaterialInventorySum(MaterialInventoryDTO searchModel, Page page)
        {
            int totalcount;
            var result = materialInventoryRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialInventoryDTO>(totalcount, result);
            return bd;
        }

        public PagedListModel<MaterialInventoryDTO> QueryMaterialInventoryDetails(MaterialInventoryDTO searchModel, Page page)
        {
            int totalcount;
            var matInventory = materialInventoryRepository.GetById(searchModel.Material_Inventory_UID);
            var warst = warehouseStorageRepository.GetById(matInventory.Warehouse_Storage_UID);
            var war = warehouseRepository.GetById(warst.Warehouse_UID);
            searchModel.BG_Organization_UID = war.BG_Organization_UID;
            searchModel.FunPlant_OrganizationUID = war.FunPlant_Organization_UID;
            searchModel.Warehouse_Type_UID = war.Warehouse_Type_UID;
            searchModel.Material_Uid = matInventory.Material_UID;
            var result = materialInventoryRepository.GetDetailInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialInventoryDTO>(totalcount, result);
            return bd;
        }

        #endregion 库存明细查询
        #region 备品需求计算

        public PagedListModel<MaterialSparepartsDemandDTO> QueryMatSparepartsDemands(MaterialSparepartsDemandDTO searchModel, Page page)
        {
            int totalcount;
            var result = materialSparepartsDemandRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialSparepartsDemandDTO>(totalcount, result);
            return bd;
        }

        public string AddMatSparepartslDemand(MaterialSparepartsDemandDTO dto)
        {
            try
            {
                List<EQP_Type> eqptypelist = new List<EQP_Type>();
                if (dto.FunPlant_Organization_UID == 0)
                {
                    eqptypelist = eQP_TypeRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                 ).ToList();

                }
                else
                {
                    eqptypelist = eQP_TypeRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                   & m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID).ToList();
                }
                List<Material_Spareparts_Demand> olddate = new List<Material_Spareparts_Demand>();
                if (dto.FunPlant_Organization_UID == 0)
                {

                    olddate = materialSparepartsDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                          & m.Demand_Date == dto.Demand_Date).ToList();
                }
                else
                {

                    olddate = materialSparepartsDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                          & m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID & m.Demand_Date == dto.Demand_Date).ToList();
                }

                //fky2017/11/13
                //var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 412);
                var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 437);
                //fky2017/11/13
                // var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 415);
                var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 440);
                if (okolddata1 != null)
                    return "此功能厂且此需求日期的需求已经审核,请先退审";
                else if (okolddata2 != null)
                    return "此功能厂且此需求日期的需求已经采购,不可重新计算";
                else
                {
                    foreach (var item in olddate.ToList())
                    {
                        materialSparepartsDemandRepository.Delete(item);
                    }
                    unitOfWork.Commit();
                }

                DateTime currentDate = DateTime.Now;
                foreach (EQP_Type eqptype in eqptypelist)
                {
                    var eqpmats = eQP_MaterialRepository.GetMany(m => m.EQP_Type_UID == eqptype.EQP_Type_UID);

                    foreach (EQP_Material eqpmat in eqpmats)
                    {
                        var eqpforecastpo = eQPForecastPowerOnRepository.GetFirstOrDefault(m => m.EQP_Type_UID == eqpmat.EQP_Type_UID & m.Demand_Date == dto.Demand_Date);
                        if (eqpforecastpo != null)
                        {
                            var mat = materialInfoRepository.GetFirstOrDefault(m => m.Material_Uid == eqpmat.Material_UID);
                            if (mat.Monthly_Consumption == null)
                                return "请维护料号(" + eqpmat.Material_Info.Material_Id + ")的每月单机用量";
                            if (mat.Material_Life == null || mat.Material_Life == 0)
                                return "请维护料号(" + eqpmat.Material_Info.Material_Id + ")的备品寿命";
                            decimal currentQty = 0;
                            var matinventory = materialInventoryRepository.GetMany(m => m.Material_UID == mat.Material_Uid);
                            if (matinventory != null && matinventory.Count() > 0)
                                currentQty = matinventory.Sum(m => m.Inventory_Qty);
                            Material_Spareparts_Demand MSD = new Material_Spareparts_Demand();
                            MSD.BG_Organization_UID = eqptype.BG_Organization_UID;
                            MSD.FunPlant_Organization_UID = eqptype.FunPlant_Organization_UID;
                            MSD.Material_UID = eqpmat.Material_UID;
                            MSD.EQP_Type_UID = eqptype.EQP_Type_UID;
                            MSD.Calculation_Date = currentDate;
                            MSD.Demand_Date = dto.Demand_Date;
                            MSD.Forecast_PowerOn_Qty = eqpforecastpo.PowerOn_Qty;
                            MSD.Material_Life = (int)mat.Material_Life;
                            MSD.Monthly_Consumption = (int)mat.Monthly_Consumption;
                            MSD.Calculated_Demand_Qty = Convert.ToInt32(Math.Round((1.0 * MSD.Monthly_Consumption / (int)MSD.Material_Life) * MSD.Forecast_PowerOn_Qty, 0));
                            MSD.User_Adjustments_Qty = 0;
                            MSD.Applicant_UID = dto.Applicant_UID;
                            MSD.Applicant_Date = currentDate;
                            //fky2017/11/13
                            // MSD.Status_UID = 410;
                            MSD.Status_UID = 435;
                            MSD.Approver_UID = dto.Applicant_UID;
                            MSD.Approver_Date = currentDate;
                            materialSparepartsDemandRepository.Add(MSD);
                        }
                        //else
                        //{
                        //    return "未设定预计开数数量资料,无法计算";
                        //}
                    }
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "备品需求计算失败:" + e.Message;
            }
        }

        public PagedListModel<MaterialSparepartsDemandDTO> QueryMatSDDetails(MaterialSparepartsDemandDTO searchModel, Page page)
        {
            int totalcount;
            var matsd = materialSparepartsDemandRepository.GetFirstOrDefault(m => m.Material_Spareparts_Demand_UID == searchModel.Material_Spareparts_Demand_UID);
            searchModel.BG_Organization_UID = matsd.BG_Organization_UID;
            searchModel.Demand_Date = matsd.Demand_Date;
            searchModel.FunPlant_Organization_UID = matsd.FunPlant_Organization_UID;
            var result = materialSparepartsDemandRepository.GetDetailInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialSparepartsDemandDTO>(totalcount, result);
            return bd;
        }

        public string EditSDUserAdjustQty(matSDVM dto)
        {
            try
            {
                foreach (var item in dto.editList)
                {
                    var matSD = materialSparepartsDemandRepository.GetById(item.Material_Spareparts_Demand_UID);
                    matSD.Applicant_UID = dto.Modified_UID;
                    matSD.Applicant_Date = DateTime.Now;
                    matSD.Status_UID = dto.Status_UID;
                    matSD.User_Adjustments_Qty = item.User_Adjustments_Qty;
                    matSD.Approver_UID = dto.Modified_UID;
                    matSD.Approver_Date = DateTime.Now;
                    materialSparepartsDemandRepository.Update(matSD);
                    unitOfWork.Commit();
                }
                return "SUCCESS";
            }
            catch (Exception e)
            {  //fky2017/11/13
               // if (dto.Status_UID == 410)
                if (dto.Status_UID == 435)
                    return "保存备品需求失败:" + e.Message;
                else
                    return "提交备品需求失败:" + e.Message;
            }
        }

        public string ApproveMatSD(int Material_Spareparts_Demand_UID, int userid)
        {
            try
            {
                var matND = materialSparepartsDemandRepository.GetById(Material_Spareparts_Demand_UID);
                var matNDList = materialSparepartsDemandRepository.GetMany(m => m.BG_Organization_UID == matND.BG_Organization_UID
                                                & m.Demand_Date == matND.Demand_Date & m.FunPlant_Organization_UID == matND.FunPlant_Organization_UID
                                                   //fky2017/11/13
                                                   //& m.Status_UID!=414).ToList();
                                                   & m.Status_UID != 439).ToList();

                foreach (var item in matNDList)
                {
                    //fky2017/11/13
                    //item.Status_UID = 412;
                    item.Status_UID = 437;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialSparepartsDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核备品需求失败:" + e.Message;
            }
        }

        public string DisapproveMatSD(int Material_Spareparts_Demand_UID, int userid)
        {
            try
            {
                var matND = materialSparepartsDemandRepository.GetById(Material_Spareparts_Demand_UID);
                var matNDList = materialSparepartsDemandRepository.GetMany(m => m.BG_Organization_UID == matND.BG_Organization_UID
                                                & m.Demand_Date == matND.Demand_Date & m.FunPlant_Organization_UID == matND.FunPlant_Organization_UID
                                                 //fky2017/11/13
                                                 //& m.Status_UID==414);
                                                 & m.Status_UID != 439);
                foreach (var item in matNDList)
                {    //fky2017/11/13
                    //item.Status_UID = 413;
                    item.Status_UID = 438;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialSparepartsDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核取消失败:" + e.Message;
            }
        }

        public List<MaterialSparepartsDemandDTO> DoSDExportFunction(int Material_Spareparts_Demand_UID)
        {
            var matSD = materialSparepartsDemandRepository.GetFirstOrDefault(m => m.Material_Spareparts_Demand_UID == Material_Spareparts_Demand_UID);

            return materialSparepartsDemandRepository.DoExportFunction(matSD);
        }

        public string DeleteMatSparepartsDemandList(int Material_Spareparts_Demand_UID, int userid)
        {
            try
            {
                var matSD = materialSparepartsDemandRepository.GetById(Material_Spareparts_Demand_UID);
                var matSDList = materialSparepartsDemandRepository.GetMany(m => m.BG_Organization_UID == matSD.BG_Organization_UID
                                                & m.Demand_Date == matSD.Demand_Date & m.FunPlant_Organization_UID == matSD.FunPlant_Organization_UID);
                foreach (var item in matSDList)
                {
                    //fky2017/11/13
                    //item.Status_UID = 414;
                    item.Status_UID = 439;
                    item.Applicant_UID = userid;
                    item.Applicant_Date = DateTime.Now;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialSparepartsDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        public string DeleteMatSD(int Material_Spareparts_Demand_UID, int userid)
        {
            try
            {
                var matSD = materialSparepartsDemandRepository.GetById(Material_Spareparts_Demand_UID);
                //fky2017/11/13
                // matSD.Status_UID = 414;
                matSD.Status_UID = 439;
                matSD.Applicant_UID = userid;
                matSD.Applicant_Date = DateTime.Now;
                matSD.Approver_UID = userid;
                matSD.Approver_Date = DateTime.Now;
                materialSparepartsDemandRepository.Update(matSD);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        /// <summary>
        /// 更新备品需求状态
        /// </summary>
        /// <param name="uid">Material_Spareparts_Demand.Material_Spareparts_Demand_UID</param>
        /// <param name="statusUID">Material_Spareparts_Demand.Status_UID(Enumeration.Enum_UID)</param>
        /// <returns></returns>
        public bool UpdateMaterialSparepartsDemandStatus(int uid, int statusUID)
        {
            try
            {
                var entity = materialSparepartsDemandRepository.GetFirstOrDefault(i => i.Material_Spareparts_Demand_UID == uid);
                var entityList = materialSparepartsDemandRepository.GetMany(i => i.BG_Organization_UID == entity.BG_Organization_UID && i.FunPlant_Organization_UID == entity.FunPlant_Organization_UID && i.Demand_Date == entity.Demand_Date);
                if (statusUID != 439)
                {
                    entityList = entityList.Where(i => i.Status_UID != 439);//439 已删除,审核的时候排除掉已删除的数据
                }
                foreach (var item in entityList)
                {
                    item.Status_UID = statusUID;
                    materialSparepartsDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion 备品需求计算
        #region 返修品需求计算

        public PagedListModel<MaterialRepairDemandDTO> QueryMatRepairDemands(MaterialRepairDemandDTO searchModel, Page page)
        {
            int totalcount;
            var result = materialRepairDemandRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialRepairDemandDTO>(totalcount, result);
            return bd;
        }

        public string AddMatRepairDemand(MaterialRepairDemandDTO dto)
        {
            try
            {
                List<EQP_Type> eqptypelist = new List<EQP_Type>();
                if (dto.FunPlant_Organization_UID == 0)
                {
                    eqptypelist = eQP_TypeRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                  ).ToList();

                }
                else
                {
                    eqptypelist = eQP_TypeRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                   & m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID).ToList();
                }

                DateTime currentDate = DateTime.Now;
                List<Material_Repair_Demand> olddate = new List<Material_Repair_Demand>();
                if (dto.FunPlant_Organization_UID == 0)
                {
                    olddate = materialRepairDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                  & m.Demand_Date == dto.Demand_Date).ToList();

                }
                else
                {
                    olddate = materialRepairDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                         & m.FunPlant_Organization_UID == dto.FunPlant_Organization_UID & m.Demand_Date == dto.Demand_Date).ToList();

                }
                //fky2017/11/13
                //var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 412);
                var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 437);
                //fky2017/11/13
                // var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 415);
                var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 440);
                if (okolddata1 != null)
                    return "此需求日期的需求已经审核,请先退审";
                else if (okolddata2 != null)
                    return "此需求日期的需求已经采购,不可重新计算";
                else
                {
                    foreach (var item in olddate.ToList())
                    {
                        materialRepairDemandRepository.Delete(item);
                    }
                    unitOfWork.Commit();
                }
                //  bool eQPForecastPowerOn = true;
                foreach (EQP_Type eqptype in eqptypelist)
                {
                    var eqpmats = eQP_MaterialRepository.GetMany(m => m.EQP_Type_UID == eqptype.EQP_Type_UID);
                    foreach (EQP_Material eqpmat in eqpmats)
                    {
                        var F3M_POQty = 0;
                        var eqpforecastpo = eQPForecastPowerOnRepository.GetFirstOrDefault(m => m.EQP_Type_UID == eqpmat.EQP_Type_UID & m.Demand_Date == dto.Demand_Date);
                        DateTime startDate = dto.Demand_Date.AddMonths(-3);
                        var eqppoweron = eQP_PowerOnRepository.GetMany(m => m.EQP_PowerOn_UID == eqpmat.EQP_Type_UID & m.PowerOn_Date <= startDate);
                        if (eqpforecastpo != null)
                        {
                            if (eqppoweron != null && eqppoweron.Count() > 0)
                                F3M_POQty = eqppoweron.Sum(m => m.Daily_PowerOn_Qty);
                            Material_Repair_Demand MRD = new Material_Repair_Demand();
                            MRD.BG_Organization_UID = eqptype.BG_Organization_UID;
                            MRD.FunPlant_Organization_UID = eqptype.FunPlant_Organization_UID;
                            MRD.Material_UID = eqpmat.Material_UID;
                            MRD.EQP_Type_UID = eqptype.EQP_Type_UID;
                            MRD.Calculation_Date = currentDate;
                            MRD.Demand_Date = dto.Demand_Date;
                            MRD.F3M_Damage_Qty = GetDamageQty(eqpmat.Material_UID, eqptype.EQP_Type1);
                            MRD.F3M_PowerOn_Qty = F3M_POQty;
                            MRD.Forecast_PowerOn_Qty = eqpforecastpo.PowerOn_Qty;
                            if (MRD.F3M_PowerOn_Qty == 0)
                                MRD.Calculated_Demand_Qty = 0;
                            else
                                MRD.Calculated_Demand_Qty = Math.Round(Convert.ToDecimal(eqpmat.BOM_Qty * MRD.F3M_Damage_Qty / F3M_POQty * MRD.Forecast_PowerOn_Qty), 0, MidpointRounding.AwayFromZero);
                            MRD.User_Adjustments_Qty = 0;
                            MRD.Applicant_UID = dto.Applicant_UID;
                            MRD.Applicant_Date = currentDate;
                            //fky2017/11/13
                            // MRD.Status_UID = 410;
                            MRD.Status_UID = 435;
                            MRD.Approver_UID = dto.Applicant_UID;
                            MRD.Approver_Date = currentDate;
                            materialRepairDemandRepository.Add(MRD);
                        }
                        //else
                        //{
                        //    eQPForecastPowerOn = false;
                        //}
                    }
                }
                unitOfWork.Commit();
                return "";
                //if (eQPForecastPowerOn)
                //{
                //    return "";
                //}
                //else
                //{
                //  return "未设定预计开数数量资料,无法计算";
                //}

            }
            catch (Exception e)
            {
                if (e.InnerException.InnerException.ToString().Contains("IMaterial_Repair_Demand_UCNdx1"))
                {
                    return "备品需求计算失败，" + "因为本次提交的需求范围与已提交的需求有重复的地方，请核实后再提交。";
                }
                return "备品需求计算失败:" + e.Message;
            }
        }

        //计算返修品数量
        int GetDamageQty(int Material_Uid, string EQP_Type)
        {
            var EqpRepair = eQPRepairInfoRepository.GetF3MDamageQty(Material_Uid, EQP_Type);
            if (EqpRepair.Count == 0)
                return 0;
            else
                return EqpRepair.Sum();
        }

        public PagedListModel<MaterialRepairDemandDTO> QueryMatRDDetails(MaterialRepairDemandDTO searchModel, Page page)
        {
            int totalcount;
            var matsd = materialRepairDemandRepository.GetFirstOrDefault(m => m.Material_Repair_Demand_UID == searchModel.Material_Repair_Demand_UID);
            searchModel.BG_Organization_UID = matsd.BG_Organization_UID;
            searchModel.Demand_Date = matsd.Demand_Date;
            searchModel.FunPlant_Organization_UID = matsd.FunPlant_Organization_UID;
            var result = materialRepairDemandRepository.GetDetailInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialRepairDemandDTO>(totalcount, result);
            return bd;
        }

        public string EditRDUserAdjustQty(matRDVM dto)
        {
            try
            {
                foreach (var item in dto.editList)
                {
                    var matRD = materialRepairDemandRepository.GetById(item.Material_Repair_Demand_UID);
                    matRD.Applicant_UID = dto.Modified_UID;
                    matRD.Applicant_Date = DateTime.Now;
                    matRD.Status_UID = dto.Status_UID;
                    matRD.User_Adjustments_Qty = item.User_Adjustments_Qty;
                    matRD.Approver_UID = dto.Modified_UID;
                    matRD.Approver_Date = DateTime.Now;
                    materialRepairDemandRepository.Update(matRD);
                    unitOfWork.Commit();
                }
                return "SUCCESS";
            }
            catch (Exception e)
            {
                //fky2017/11/13
                // if (dto.Status_UID == 410)
                if (dto.Status_UID == 435)
                    return "保存返修品需求失败:" + e.Message;
                else
                    return "保存返修品需求失败:" + e.Message;
            }
        }

        public string ApproveMatRD(int Material_Repair_Demand_UID, int userid)
        {
            try
            {
                var matRD = materialRepairDemandRepository.GetById(Material_Repair_Demand_UID);
                var matRDList = materialRepairDemandRepository.GetMany(m => m.BG_Organization_UID == matRD.BG_Organization_UID
                                                & m.Demand_Date == matRD.Demand_Date & m.FunPlant_Organization_UID == matRD.FunPlant_Organization_UID
                                                 //fky2017/11/13
                                                 //& m.Status_UID!=414).ToList();
                                                 & m.Status_UID != 439).ToList();

                foreach (var item in matRDList)
                {
                    //fky2017/11/13
                    // item.Status_UID = 412;
                    item.Status_UID = 437;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialRepairDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核返修品需求失败:" + e.Message;
            }
        }

        public string DisapproveMatRD(int Material_Repair_Demand_UID, int userid)
        {
            try
            {
                var matRD = materialRepairDemandRepository.GetById(Material_Repair_Demand_UID);
                var matRDList = materialRepairDemandRepository.GetMany(m => m.BG_Organization_UID == matRD.BG_Organization_UID
                                                & m.Demand_Date == matRD.Demand_Date & m.FunPlant_Organization_UID == matRD.FunPlant_Organization_UID
                                               //fky2017/11/13
                                               // & m.Status_UID==412);
                                               & m.Status_UID != 439);
                foreach (var item in matRDList)
                {
                    //fky2017/11/13
                    //item.Status_UID = 413;
                    item.Status_UID = 438;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialRepairDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核取消失败:" + e.Message;
            }
        }

        public List<MaterialRepairDemandDTO> DoRDExportFunction(int Material_Repair_Demand_UID)
        {
            var matRD = materialRepairDemandRepository.GetFirstOrDefault(m => m.Material_Repair_Demand_UID == Material_Repair_Demand_UID);

            return materialRepairDemandRepository.DoExportFunction(matRD);
        }

        public string DeleteMatRepairDemandList(int Material_Repair_Demand_UID, int userid)
        {
            try
            {
                var matRD = materialRepairDemandRepository.GetById(Material_Repair_Demand_UID);
                var matRDList = materialRepairDemandRepository.GetMany(m => m.BG_Organization_UID == matRD.BG_Organization_UID
                                                & m.Demand_Date == matRD.Demand_Date & m.FunPlant_Organization_UID == matRD.FunPlant_Organization_UID);
                foreach (var item in matRDList)
                {
                    //fky2017/11/13
                    //item.Status_UID = 414;
                    item.Status_UID = 439;
                    item.Applicant_UID = userid;
                    item.Applicant_Date = DateTime.Now;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialRepairDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        public string DeleteMatRD(int Material_Repair_Demand_UID, int userid)
        {
            try
            {
                var matRD = materialRepairDemandRepository.GetById(Material_Repair_Demand_UID);
                //fky2017/11/13
                //  matRD.Status_UID = 414;
                matRD.Status_UID = 439;
                matRD.Applicant_UID = userid;
                matRD.Applicant_Date = DateTime.Now;
                matRD.Approver_UID = userid;
                matRD.Approver_Date = DateTime.Now;
                materialRepairDemandRepository.Update(matRD);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        /// <summary>
        /// 更新返修品需求状态
        /// </summary>
        /// <param name="uid">Material_Repair_Demand.Material_Repair_Demand_UID</param>
        /// <param name="statusUID">Material_Repair_Demand.Status_UID(Enumeration.Enum_UID)</param>
        /// <returns></returns>
        public bool UpdateMaterialRepairDemandStatus(int uid, int statusUID)
        {
            try
            {
                var entity = materialRepairDemandRepository.GetFirstOrDefault(i => i.Material_Repair_Demand_UID == uid);
                var entityList = materialRepairDemandRepository.GetMany(i => i.BG_Organization_UID == entity.BG_Organization_UID && i.FunPlant_Organization_UID == entity.FunPlant_Organization_UID && i.Demand_Date == entity.Demand_Date);
                if (statusUID != 439)
                {
                    entityList = entityList.Where(i => i.Status_UID != 439);//439 已删除,审核的时候排除掉已删除的数据
                }
                foreach (var item in entityList)
                {
                    item.Status_UID = statusUID;
                    materialRepairDemandRepository.Update(item);
                }
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion 返修品需求计算
        #region 采购需求汇总
        public PagedListModel<MaterialDemandSummaryDTO> QueryMatDemandSummary(MaterialDemandSummaryDTO searchModel, Page page)
        {
            int totalcount;
            var result = materialDemandSummaryRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialDemandSummaryDTO>(totalcount, result);
            return bd;
        }

        public string AddMatDemandSummary(MaterialDemandSummaryDTO dto)
        {
            try
            {
                DateTime currentDate = DateTime.Now;

                var eqptypelist = eQP_TypeRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID).ToList();
                //如果已经计算过了，就要删除。
                var olddate = materialDemandSummaryRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                         & m.Demand_Date == dto.Demand_Date).ToList();
                //fky2017/11/13
                // var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 412);
                var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 437);
                //fky2017/11/13
                // var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 415);
                var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 440);
                if (okolddata1 != null)
                {
                    return "此BG且此需求日期的需求已经审核,请先退审";
                }
                else if (okolddata2 != null)
                {
                    return "此BG且此需求日期的需求已经采购,不可重新计算";
                }
                else
                {
                    if (olddate.Count > 0)
                    {
                        foreach (var item in olddate)
                        {
                            materialDemandSummaryRepository.Delete(item);
                        }
                        unitOfWork.Commit();

                    }
                }



                // bool bj = false;
                if (eqptypelist.Count > 0)
                {
                    foreach (EQP_Type eqptype in eqptypelist)
                    {
                        var eqpmats = eQP_MaterialRepository.GetMany(m => m.EQP_Type_UID == eqptype.EQP_Type_UID).ToList();
                        //var olddate = materialDemandSummaryRepository.GetMany(m => m.BG_Organization_UID == eqptype.BG_Organization_UID
                        //                            & m.Demand_Date == dto.Demand_Date).ToList();
                        ////fky2017/11/13
                        //// var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 412);
                        //var okolddata1 = olddate.FirstOrDefault(m => m.Status_UID == 437);
                        ////fky2017/11/13
                        //// var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 415);
                        //var okolddata2 = olddate.FirstOrDefault(m => m.Status_UID == 440);
                        //if (okolddata1 != null)
                        //{
                        //    return "此BG且此需求日期的需求已经审核,请先退审";
                        //}
                        //else if (okolddata2 != null)
                        //{
                        //    return "此BG且此需求日期的需求已经采购,不可重新计算";
                        //}
                        //else
                        //{
                        //    if (olddate.Count > 0 && bj == false)
                        //    {
                        //        foreach (var item in olddate)
                        //        {
                        //            materialDemandSummaryRepository.Delete(item);
                        //        }
                        //        unitOfWork.Commit();
                        //        bj = true;
                        //    }
                        //}
                        if (eqpmats.Count > 0)
                        {
                            foreach (EQP_Material eqpmat in eqpmats)
                            {

                                decimal matNormalQty = 0;
                                var matNormalDemand = materialNormalDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                                                    //fky2017/11/13
                                                                                    // & m.Demand_Date == dto.Demand_Date & m.Status_UID == 412
                                                                                    & m.Demand_Date == dto.Demand_Date & m.Status_UID == 437
                                                                                    & m.EQP_Type_UID == eqptype.EQP_Type_UID & m.Material_UID == eqpmat.Material_UID);
                                if (matNormalDemand.Count() > 0)
                                    matNormalQty = matNormalDemand.Sum(m => m.Calculated_Demand_Qty) + matNormalDemand.Sum(m => m.User_Adjustments_Qty);

                                decimal matSparepartsQty = 0;
                                var matSparepartsDemand = materialSparepartsDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                                                             //fky2017/11/13
                                                                                             //& m.Demand_Date == dto.Demand_Date & m.Status_UID == 412
                                                                                             & m.Demand_Date == dto.Demand_Date & m.Status_UID == 437
                                                                                            & m.EQP_Type_UID == eqptype.EQP_Type_UID & m.Material_UID == eqpmat.Material_UID);
                                if (matSparepartsDemand.Count() > 0)
                                    matSparepartsQty = matSparepartsDemand.Sum(m => m.Calculated_Demand_Qty) + matSparepartsDemand.Sum(m => m.User_Adjustments_Qty);

                                decimal matRepairQty = 0;
                                var matRepairDemand = materialRepairDemandRepository.GetMany(m => m.BG_Organization_UID == dto.BG_Organization_UID
                                                                                    //fky2017/11/13
                                                                                    //& m.Demand_Date == dto.Demand_Date & m.Status_UID == 412
                                                                                    & m.Demand_Date == dto.Demand_Date & m.Status_UID == 437
                                                                                    & m.EQP_Type_UID == eqptype.EQP_Type_UID & m.Material_UID == eqpmat.Material_UID);
                                if (matRepairDemand.Count() > 0)
                                    matRepairQty = matRepairDemand.Sum(m => m.Calculated_Demand_Qty) + matRepairDemand.Sum(m => m.User_Adjustments_Qty);
                                var OldMatDS = materialDemandSummaryRepository.GetFirstOrDefault(m => m.BG_Organization_UID == eqptype.BG_Organization_UID &
                                                                m.Demand_Date == dto.Demand_Date & m.Material_UID == eqpmat.Material_UID);
                                if (OldMatDS != null)
                                {
                                    OldMatDS.Be_Purchase_Qty += matNormalQty + matSparepartsQty + matRepairQty;
                                    materialDemandSummaryRepository.Update(OldMatDS);
                                }
                                else
                                {
                                    Material_Demand_Summary MDS = new Material_Demand_Summary();
                                    MDS.BG_Organization_UID = eqptype.BG_Organization_UID;
                                    MDS.FunPlant_Organization_UID = eqptype.FunPlant_Organization_UID;
                                    MDS.Material_UID = eqpmat.Material_UID;
                                    MDS.NormalDemand_Qty = matNormalQty;
                                    MDS.SparepartsDemand_Qty = matSparepartsQty;
                                    MDS.Repair_Demand_Qty = matRepairQty;
                                    MDS.Be_Purchase_Qty = matNormalQty + matSparepartsQty + matRepairQty;
                                    MDS.Calculation_Date = currentDate;
                                    MDS.Demand_Date = dto.Demand_Date;
                                    MDS.Applicant_UID = dto.Applicant_UID;
                                    MDS.Applicant_Date = currentDate;
                                    //fky2017/11/13
                                    //MDS.Status_UID = 410;
                                    MDS.Status_UID = 435;
                                    MDS.Approver_UID = dto.Applicant_UID;
                                    MDS.Approver_Date = currentDate;
                                    materialDemandSummaryRepository.Add(MDS);
                                }
                            }
                            //此步提交需放在这里,不能放在最外面,因为可能提交相同料号的资料
                            unitOfWork.Commit();
                        }
                    }

                }
                return "";
            }
            catch (Exception e)
            {
                return "采购需求汇总计算失败:" + e.Message;
            }
        }


        public PagedListModel<MaterialDemandSummaryDTO> QueryMatDSDetails(MaterialDemandSummaryDTO searchModel, Page page)
        {
            int totalcount;
            var matsd = materialDemandSummaryRepository.GetFirstOrDefault(m => m.Material_Demand_Summary_UID == searchModel.Material_Demand_Summary_UID);
            searchModel.BG_Organization_UID = matsd.BG_Organization_UID;
            searchModel.Demand_Date = matsd.Demand_Date;
            var result = materialDemandSummaryRepository.GetDetailInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<MaterialDemandSummaryDTO>(totalcount, result);
            return bd;
        }

        public string SubmitMatDemandSummary(int Material_Demand_Summary_UID, int userid)
        {
            try
            {
                var matDS = materialDemandSummaryRepository.GetById(Material_Demand_Summary_UID);
                var matDSList = materialDemandSummaryRepository.GetMany(m => m.BG_Organization_UID == matDS.BG_Organization_UID & m.Status_UID == 435);
                foreach (var item in matDSList)
                {  //fky2017/11/13
                   //item.Status_UID = 412;
                    item.Status_UID = 436;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialDemandSummaryRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "提交采购需求失败:" + e.Message;
            }
        }

        public string PurchaseMatDS(int Material_Demand_Summary_UID, int userid)
        {
            try
            {
                var matDS = materialDemandSummaryRepository.GetById(Material_Demand_Summary_UID);
                var matDSList = materialDemandSummaryRepository.GetMany(m => m.BG_Organization_UID == matDS.BG_Organization_UID
                                             //fky2017/11/13
                                             // & m.Status_UID == 412);
                                             & m.Status_UID == 437);
                foreach (var item in matDSList)
                {
                    //fky2017/11/13
                    // item.Status_UID = 415;
                    item.Status_UID = 440;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialDemandSummaryRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "采购操作失败:" + e.Message;
            }
        }

        public List<MaterialDemandSummaryDTO> DoDSExportFunction(int Material_Demand_Summary_UID)
        {
            var matDS = materialDemandSummaryRepository.GetFirstOrDefault(m => m.Material_Demand_Summary_UID == Material_Demand_Summary_UID);

            return materialDemandSummaryRepository.DoExportFunction(matDS);
        }

        public string DeleteMatDemandSummaryList(int Material_Demand_Summary_UID, int userid)
        {
            try
            {
                var matDS = materialDemandSummaryRepository.GetById(Material_Demand_Summary_UID);
                var matDSList = materialDemandSummaryRepository.GetMany(m => m.BG_Organization_UID == matDS.BG_Organization_UID
                                                & m.Demand_Date == matDS.Demand_Date);
                foreach (var item in matDSList)
                { //fky2017/11/13
                    //item.Status_UID = 414;
                    item.Status_UID = 439;
                    item.Applicant_UID = userid;
                    item.Applicant_Date = DateTime.Now;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialDemandSummaryRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        public string DeleteMatDS(int Material_Demand_Summary_UID, int userid)
        {
            try
            {
                var matDS = materialDemandSummaryRepository.GetById(Material_Demand_Summary_UID);
                //fky2017/11/13
                // matDS.Status_UID = 414;
                matDS.Status_UID = 439;
                matDS.Applicant_UID = userid;
                matDS.Applicant_Date = DateTime.Now;
                matDS.Approver_UID = userid;
                matDS.Approver_Date = DateTime.Now;
                materialDemandSummaryRepository.Update(matDS);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        public string DisdeleteMatDS(int Material_Demand_Summary_UID, int userid)
        {
            try
            {
                var matDS = materialDemandSummaryRepository.GetById(Material_Demand_Summary_UID);
                //fky2017/11/13
                // matDS.Status_UID = 410;
                matDS.Status_UID = 435;
                matDS.Applicant_UID = userid;
                matDS.Applicant_Date = DateTime.Now;
                matDS.Approver_UID = userid;
                matDS.Approver_Date = DateTime.Now;
                materialDemandSummaryRepository.Update(matDS);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除失败:" + e.Message;
            }
        }

        public string DisPurchaseMatDS(int Material_Demand_Summary_UID, int userid)
        {
            try
            {
                var matDS = materialDemandSummaryRepository.GetById(Material_Demand_Summary_UID);
                var matDSList = materialDemandSummaryRepository.GetMany(m => m.BG_Organization_UID == matDS.BG_Organization_UID
                                             & m.Status_UID == 440);
                foreach (var item in matDSList)
                {
                    item.Status_UID = 437;
                    item.Approver_UID = userid;
                    item.Approver_Date = DateTime.Now;
                    materialDemandSummaryRepository.Update(item);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "取消采购失败:" + e.Message;
            }
        }
        /// <summary>
        /// 更新采购需求汇总状态
        /// </summary>
        /// <param name="uid">Material_Demand_Summary.Material_Demand_Summary_UID</param>
        /// <param name="statusUID">Material_Demand_Summary.Status_UID(Enumeration.Enum_UID)</param>
        /// <returns></returns>
        public bool UpdateMaterialDemandSummaryStatus(int uid, int statusUID)
        {
            try
            {
                var entity = materialDemandSummaryRepository.GetFirstOrDefault(i => i.Material_Demand_Summary_UID == uid);
                var entityList = materialDemandSummaryRepository.GetMany(i => i.BG_Organization_UID == entity.BG_Organization_UID && i.FunPlant_Organization_UID == entity.FunPlant_Organization_UID && i.Demand_Date == entity.Demand_Date);
                if (statusUID != 439)
                {
                    entityList = entityList.Where(i => i.Status_UID != 439);//439 已删除,审核的时候排除掉已删除的数据
                }
                foreach (var item in entityList)
                {
                    item.Status_UID = statusUID;
                    materialDemandSummaryRepository.Update(item);
                }
                unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion 采购需求汇总 
        #region 库存报表

        public PagedListModel<StorageReportDTO> QueryStorageReports(StorageReportDTO searchModel, Page page)
        {

            int totalcount;
            if (searchModel.intMonth.Year != 1)
            {
                searchModel.Start_Date = searchModel.intMonth.AddDays(1 - searchModel.intMonth.Day);
                searchModel.End_Date = searchModel.Start_Date.AddMonths(1).AddDays(-1);
            }
            var result = storageInOutDetailRepository.GetStorageReportInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<StorageReportDTO>(totalcount, result);
            return bd;
        }

        public List<SystemOrgDTO> QueryPlants(int plantorguid)
        {
            var bud = warehouseRepository.GetPlants(plantorguid).ToList();
            return bud;
        }

        public List<SystemOrgDTO> QueryBGByPlant(int plantuid)
        {
            var bud = warehouseRepository.GetBGByPlant(plantuid).ToList();
            return bud;
        }

        public List<StorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string material, DateTime start_date, DateTime end_date)
        {
            return storageInOutDetailRepository.DoSRExportFunction(plant, bg, funplant, material, start_date, end_date);
        }

        #endregion 库存报表
    }
}
