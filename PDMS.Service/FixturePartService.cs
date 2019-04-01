using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Fixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IFixturePartService : IBaseSevice<Fixture_Part, Fixture_PartDTO, Fixture_PartModelSearch>
    {
        #region 治具配件
        IList<Fixture_PartDTO> QueryAllFixtureParts(Fixture_PartModelSearch search);
        #endregion

        #region 治具配件设定维护
        PagedListModel<Fixture_Part_Setting_MDTO> QueryFixturePartSettingMs(Fixture_Part_Setting_MModelSearch search, Page page);
        IList<Fixture_Part_Setting_MDTO> GetFixturePartSettingMListByQuery(Fixture_Part_Setting_MModelSearch search);
        string AddFixturePartSetting(Fixture_Part_Setting_MDTO dto);

        string EditFixturePartSetting(Fixture_Part_Setting_MDTO dto);

        string DeleteFixturePartSetting(int uid);

        Fixture_Part_Setting_MDTO GetFixturePartSettingMDTOByUID(int Fixture_Part_Setting_M_UID);

        //bool IsFixture_PartExist(Fixture_PartModelSearch search);
        //IList<Fixture_PartDTO> GetFixturePartList(Fixture_PartModelSearch search);
        //Fixture_Part QueryFixture_PartSingle(int uid);
        //string AddFixture_Part(Fixture_Part fixturePart);
        //string EditFixture_Part(Fixture_Part fixtrePart);
        //string DeleteFixture_Part(int uid);
        List<Fixture_Part_Setting_MDTO> QueryAllFixturePartSettingMs();
        IList<Fixture_Part_Setting_MDTO> DoExportFixtuerPartSettingM(string uids);

        //List<Fixture_Part_Setting_MDTO> GetManyFixturePartSettingM(Expression<Func<Fixture_Part_Setting_MDTO, bool>> where);

        string InsertFixturePartSettingMs(List<Fixture_Part_Setting_MDTO> list);
        string InsertOrUpdateFixturePartSettingDs(List<Fixture_Part_Setting_DDTO> list);
        #endregion



        #region 仓库管理
        List<SystemOrgDTO> QueryOpType(int plantorguid);
        PagedListModel<FixturePartWarehouseDTO> QueryWarehouses(FixturePartWarehouseDTO searchModel, Page page);
        List<SystemOrgDTO> QueryFunplantByop(int opuid);
        List<FixturePartWarehouseDTO> QueryWar(int Warehouse_UID);
        List<FixturePartWarehouseStorageDTO> QueryWarhouseSts(int Warehouse_UID);
        string AddOrEditWarehouseInfo(FixturePartWarehouseStorages dto);
        string DeleteWarehouseStorage(int WareStorage_UId);
        string DeleteWarehouse(int Warehouse_UID);
        List<FixturePartWarehouseDTO> QueryAllWarehouseSt(int Plant_Organization_UID);
        List<FixturePartWarehouseDTO> GetFixtureWarehouseStorageALL(int Plant_Organization_UID);
        string InsertWarehouseBase(List<FixturePartWarehouseDTO> dtolist);

        string InsertWarehouse(List<FixturePartWarehouseDTO> dtolist);

        List<FixturePartWarehouseDTO> DoExportWarehouseReprot(string uids);
        List<FixturePartWarehouseDTO> DoAllExportWarehouseReprot(FixturePartWarehouseDTO searchModel);
        #endregion
        #region 开账
        PagedListModel<FixturePartStorageInboundDTO> QueryCreateBounds(FixturePartStorageInboundDTO searchModel, Page page);
        List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO();
        List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO(string Enum_Type);
        List<FixturePartWarehouseDTO> GetFixtureWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Fixture_PartDTO> GetFixtureParts(int Fixture_Warehouse_Storage_UID);
        string AddOrEditCreateBound(FixturePartStorageInboundDTO dto);
        FixturePartStorageInboundDTO QueryInboundByUid(int Fixture_Storage_Inbound_UID);
        string DeleteCreateBound(int Fixture_Storage_Inbound_UID);

        List<FixturePartStorageInboundDTO> DoExportCreateBoundReprot(string uids);
        List<FixturePartStorageInboundDTO> DoAllExportCreateBoundReprot(FixturePartStorageInboundDTO search);

        List<FixturePartStorageInboundDTO> QueryAllStorageInbound(int Plant_Organization_UID);

        string InsertCreateBoundItem(List<FixturePartStorageInboundDTO> dtolist);

        string ApproveCreatebound(int Fixture_Storage_Inbound_UID, int Useruid);
        #endregion

        #region Add by Rock 2018-01-10 ----------------采购单维护作业----------start
        PagedListModel<FixturePart_OrderQueryVM> QueryPurchase(FixturePart_OrderVM model, Page page);

        List<FixturePart_OrderEdit> doAllPurchaseorderMaintain(FixturePart_OrderVM model);
        List<FixturePart_OrderEdit> doPartPurchaseorderMaintain(string uids);

        FixturePart_OrderEdit QueryPurchaseByUID(int Fixture_Part_Order_M_UID);
        List<FixturePartSettingMVM> GetFixturePartByPlantOptypeFunc(int PlantUID, int Optype, int FuncUID);
        List<FixturePartPurchaseVM> GetFixturePartByMUIDAPI(int UID);
        string SaveFixturePartByMUID(SubmitFixturePartOrderVM item);
        string DeletePurchaseByUIDS(List<int> idList);
        #endregion Add by Rock 2018-01-10 ----------------采购单维护作业----------end


        #region  出入库维护作业
        PagedListModel<FixturePartInOutBoundInfoDTO> QueryBoundDetails(FixturePartInOutBoundInfoDTO searchModel, Page page);

        List<FixturePartInOutBoundInfoDTO> ExportAllOutboundInfo(FixturePartInOutBoundInfoDTO searchModel);

        List<FixturePartInOutBoundInfoDTO> ExportPartOutboundInfo(string uids);

        List<Fixture_Part_Order_MDTO> GetFixture_Part_Order_Ms(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Fixture_PartDTO> GetFixturePartDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Fixture_Part_InventoryDTO> GetWarehouseStorageByFixture_Part_UID(int Fixture_Part_UID);
        List<Fixture_Part_Order_DDTO> GetFixture_Part_Order_Ds(int Fixture_Part_Order_M_UID);

        Fixture_Part_Order_DDTO GetFixture_Part_Order_D(int Fixture_Part_Order_D_UID);

        List<FixturePartWarehouseDTO> GetFixture_Part_Warehouses(int Fixture_Part_Order_D_UID);
        string AddOrEditInboundApply(FixturePartInOutBoundInfoDTO dto);

        FixturePartInOutBoundInfoDTO QueryInBoudSingle(int Storage_In_Out_Bound_UID);
        FixturePartInOutBoundInfoDTO QueryOutBoudSingle(int Storage_In_Out_Bound_UID);
        List<Fixture_Storage_Outbound_DDTO> QueryOutBouddetails(int Storage_In_Out_Bound_UID);
        List<FixturePartWarehouseDTO> GetFixtureWarehouses(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        string SaveWarehouseStorage(FixturePartWarehouseStorageDTO dto);
        string DeleteBound(int Storage_In_Out_Bound_UID, string Inout_Type);
        string ApproveInbound(int Storage_In_Out_Bound_UID, int Useruid);
        List<Fixture_Part_InventoryDTO> GetMatinventory(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID);
        PagedListModel<Fixture_Part_InventoryDTO> GetFixtureinventory(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID, Page page);

        string AddOrEditOutbound(FixturePartInOutBoundInfoDTO dto);
        string ApproveOutbound(int Storage_In_Out_Bound_UID, int Useruid);
        #endregion

        #region 储位移转维护作业
        PagedListModel<Fixture_Storage_TransferDTO> QueryStorageTransfers(Fixture_Storage_TransferDTO searchModel, Page page);
        List<Fixture_Warehouse_StorageDTO> GetFixturePartWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        string AddOrEditStorageTransfer(Fixture_Storage_TransferDTO dto);
        Fixture_Storage_TransferDTO QueryStorageTransferByUid(int Fixture_Storage_Transfer_UID);
        string DeleteStorageTransfer(int Fixture_Storage_Transfer_UID, int userid);

        List<Fixture_Storage_TransferDTO> DoExportStorageTransferReprot(string uids);
        List<Fixture_Storage_TransferDTO> DoAllExportStorageTransferReprot(Fixture_Storage_TransferDTO search);
        List<Fixture_Part_InventoryDTO> Fixture_Part_InventoryDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        string ImportStorageTransfer(List<Fixture_Storage_TransferDTO> dtolist);

        string ApproveStTransfer(int Fixture_Storage_Transfer_UID, int Useruid);
        #endregion


        #region 盘点
        PagedListModel<FixtureStorageCheckDTO> QueryStorageChecks(FixtureStorageCheckDTO searchModel, Page page);
        string AddOrEditStorageCheck(FixtureStorageCheckDTO dto);
        FixtureStorageCheckDTO QueryStorageCheckSingle(int Fixture_Storage_Check_UID);
        string DeleteStorageCheck(int Fixture_Storage_Check_UID, int userid);
        List<FixtureStorageCheckDTO> DoAllExportStorageCheckReprot(FixtureStorageCheckDTO searchModel);
        List<FixtureStorageCheckDTO> DoExportStorageCheckReprot(string uids);
        List<FixtureStorageCheckDTO> DownloadStorageCheck(string Part_ID, string Part_Name, string Part_Spec, string Fixture_Warehouse_ID, string Rack_ID, string Storage_ID);
        string ImportStorageCheck(List<FixtureStorageCheckDTO> dtolist);
        string ApproveStCheck(int Fixture_Storage_Check_UID, int Useruid);
        string ApproveStorageCheck(FixtureStorageCheckDTO dto);

        PagedListModel<Fixture_Part_InventoryDTO> GetFPStoryDetialReport(Fixture_Part_InventoryDTO searchModel, Page page);

        List<Fixture_Part_InventoryDTO> ExportAllFixtureInventoryDetialReport(Fixture_Part_InventoryDTO searchModel);

        List<Fixture_Part_InventoryDTO> ExportSelectedFixtureInventoryDetialReport(string uids);

        PagedListModel<FixtureInOutStorageModel> GetInOutDetialReport(FixtureInOutStorageModel searchModel, Page page);

        List<FixtureInOutStorageModel> ExportAllInOutDetialReport(FixtureInOutStorageModel searchModel);

        List<FixtureInOutStorageModel> ExportSelectedInOutDetialReport(string uids);

        #endregion
        #region 库存报表
        PagedListModel<FixturePartStorageReportDTO> QueryStorageReports(FixturePartStorageReportDTO searchModel, Page page);
        List<FixturePartStorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string Part_ID, string Part_Name, string Part_Spec, DateTime start_date, DateTime end_date);
        #endregion
        FixturePartScanCodeDTO GetFixturePartScanCodeDTO(int plantID, int optypeID, string SN, int Modified_UID);
        FixtureDTO GetFixtureScanCodeDTOBySN(int plantID, int optypeID, string SN);
    }
    public class FixturePartService : BaseSevice<Fixture_Part, Fixture_PartDTO, Fixture_PartModelSearch>, IFixturePartService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IFixture_PartRepository fixturePartRepository;
        private readonly IFixture_Part_Setting_MRepository fixturePartSettingMRepository;
        private readonly IFixture_Part_Setting_DRepository fixturePartSettingDRepository;
        private readonly IFixturePartWarehouseRepository fixturePartWarehouseRepository;
        private readonly IFixture_Warehouse_StorageRepository fixture_Warehouse_StorageRepository;
        private readonly IFixture_Storage_InboundRepository fixture_Storage_InboundRepository;
        private readonly IFixture_Part_InventoryRepository fixture_Part_InventoryRepository;
        private readonly IFixture_Storage_InOut_DetailRepository fixture_Storage_InOut_DetailRepository;
        private readonly IFixturePartOrderMRepository fixturePartOrderMRepository;
        private readonly IFixturePartOrderDRepository fixturePartOrderDRepository;
        private readonly IFixture_Storage_Outbound_MRepository fixture_Storage_Outbound_MRepository;
        private readonly IFixture_Storage_Outbound_DRepository fixture_Storage_Outbound_DRepository;
        private readonly IFixture_Storage_TransferRepository fixture_Storage_TransferRepository;
        private readonly IFixture_Storage_CheckRepository fixture_Storage_CheckRepository;
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IFixture_Part_Order_ScheduleRepository fixture_Part_Order_ScheduleRepository;
        private readonly IFixtureRepository fixtureRepository;

        public FixturePartService(IFixture_PartRepository fixturePartRepository, IUnitOfWork unitOfWork, IFixture_Part_Setting_MRepository fixturePartSettingMRepository, IFixturePartWarehouseRepository fixturePartWarehouseRepository, IFixture_Warehouse_StorageRepository fixture_Warehouse_StorageRepository
            , IFixture_Part_Setting_DRepository fixturePartSettingDRepository, IFixture_Storage_InboundRepository fixture_Storage_InboundRepository, IFixture_Part_InventoryRepository fixture_Part_InventoryRepository, IFixture_Storage_InOut_DetailRepository fixture_Storage_InOut_DetailRepository,
            IFixturePartOrderMRepository fixturePartOrderMRepository, IFixturePartOrderDRepository fixturePartOrderDRepository, IFixture_Storage_Outbound_MRepository fixture_Storage_Outbound_MRepository, IFixture_Storage_Outbound_DRepository fixture_Storage_Outbound_DRepository,
            IFixture_Storage_TransferRepository fixture_Storage_TransferRepository, IFixture_Storage_CheckRepository fixture_Storage_CheckRepository, IEnumerationRepository enumerationRepository, IFixture_Part_Order_ScheduleRepository fixture_Part_Order_ScheduleRepository, IFixtureRepository fixtureRepository)
            : base(fixturePartRepository)
        {
            this.unitOfWork = unitOfWork;
            this.fixturePartRepository = fixturePartRepository;
            this.fixturePartSettingMRepository = fixturePartSettingMRepository;
            this.fixturePartWarehouseRepository = fixturePartWarehouseRepository;
            this.fixture_Warehouse_StorageRepository = fixture_Warehouse_StorageRepository;
            this.fixturePartSettingDRepository = fixturePartSettingDRepository;
            this.fixture_Storage_InboundRepository = fixture_Storage_InboundRepository;
            this.fixture_Part_InventoryRepository = fixture_Part_InventoryRepository;
            this.fixture_Storage_InOut_DetailRepository = fixture_Storage_InOut_DetailRepository;
            this.fixturePartOrderMRepository = fixturePartOrderMRepository;
            this.fixturePartOrderDRepository = fixturePartOrderDRepository;
            this.fixture_Storage_Outbound_MRepository = fixture_Storage_Outbound_MRepository;
            this.fixture_Storage_Outbound_DRepository = fixture_Storage_Outbound_DRepository;
            this.fixture_Storage_TransferRepository = fixture_Storage_TransferRepository;
            this.fixture_Storage_CheckRepository = fixture_Storage_CheckRepository;
            this.enumerationRepository = enumerationRepository;
            this.fixture_Part_Order_ScheduleRepository = fixture_Part_Order_ScheduleRepository;
            this.fixtureRepository = fixtureRepository;
        }

        #region 治具配件
        public IList<Fixture_PartDTO> QueryAllFixtureParts(Fixture_PartModelSearch search)
        {
            var query = fixturePartRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID.HasValue && search.FunPlant_Organization_UID.Value > 0)
                {
                    query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                }
                if (!string.IsNullOrWhiteSpace(search.Part_ID))
                {
                    query = query.Where(w => w.Part_ID.Contains(search.Part_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.Part_Name))
                {
                    query = query.Where(w => w.Part_Name.Contains(search.Part_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Part_Spec))
                {
                    query = query.Where(w => w.Part_Spec.Contains(search.Part_Spec));
                }
                if (search.Purchase_Cycle.HasValue)
                {
                    query = query.Where(w => w.Purchase_Cycle == search.Purchase_Cycle.Value);
                }
                if (search.Is_Automation.HasValue)
                {
                    query = query.Where(w => w.Is_Automation == search.Is_Automation.Value);
                }
                if (search.Is_Standardized.HasValue)
                {
                    query = query.Where(w => w.Is_Standardized == search.Is_Standardized.Value);
                }
                if (search.Is_Storage_Managed.HasValue)
                {
                    query = query.Where(w => w.Is_Storage_Managed == search.Is_Storage_Managed.Value);
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
                }
            }
            query = query.OrderBy(w => w.Fixture_Part_UID);
            var dtoList = AutoMapper.Mapper.Map<IList<Fixture_PartDTO>>(query);
            return dtoList;
        }
        #endregion

        #region 治具配件设定维护
        //public bool IsFixture_PartExist(Fixture_PartModelSearch search)
        //{
        //    var isExist = false;
        //    var query = fixturePartRepository.GetAll();
        //    if (search.Fixture_Part_UID.HasValue)
        //    {
        //        query = query.Where(i => i.Fixture_Part_UID != search.Fixture_Part_UID.Value);
        //    }
        //    //料号Part_ID 不能重复
        //    isExist = query.Any(d => d.Plant_Organization_UID == search.Plant_Organization_UID && d.BG_Organization_UID == search.BG_Organization_UID && d.Part_ID == search.Part_ID);

        //    return isExist;
        //}
        public PagedListModel<Fixture_Part_Setting_MDTO> QueryFixturePartSettingMs(Fixture_Part_Setting_MModelSearch search, Page page)
        {
            var totalCount = 0;
            var fixture_Parts = fixturePartSettingMRepository.QueryFixturePartSettingMs(search, page, out totalCount);

            IList<Fixture_Part_Setting_MDTO> fixture_MachineDTOList = new List<Fixture_Part_Setting_MDTO>();

            foreach (var fixture_Part in fixture_Parts)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Setting_MDTO>(fixture_Part);
                if (fixture_Part.System_Organization != null)
                {
                    dto.Plant_Organization_Name = fixture_Part.System_Organization.Organization_Name;
                }
                if (fixture_Part.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = fixture_Part.System_Organization1.Organization_Name;
                }
                if (fixture_Part.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = fixture_Part.System_Organization2.Organization_Name;
                }
                if (fixture_Part.System_Users != null)
                {
                    dto.Created_UserName = fixture_Part.System_Users.User_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Part.System_Users1);
                fixture_MachineDTOList.Add(dto);
            }

            return new PagedListModel<Fixture_Part_Setting_MDTO>(totalCount, fixture_MachineDTOList);
        }

        public IList<Fixture_Part_Setting_MDTO> GetFixturePartSettingMListByQuery(Fixture_Part_Setting_MModelSearch search)
        {
            var query = fixturePartSettingMRepository.GetAll();
            if (search != null)
            {
                if (search.Plant_Organization_UID > 0)
                {
                    query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                }
                if (search.BG_Organization_UID > 0)
                {
                    query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                }
                if (search.FunPlant_Organization_UID.HasValue && search.FunPlant_Organization_UID.Value > 0)
                {
                    query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                }
                if (!string.IsNullOrWhiteSpace(search.Fixture_NO))
                {
                    query = query.Where(w => w.Fixture_NO.Contains(search.Fixture_NO));
                }
                if (search.Line_Qty.HasValue)
                {
                    query = query.Where(w => w.Line_Qty == search.Line_Qty);
                }
                if (search.Line_Fixture_Ratio_Qty.HasValue)
                {
                    query = query.Where(w => w.Line_Fixture_Ratio_Qty == search.Line_Fixture_Ratio_Qty);
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
                }
            }
            query = query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.FunPlant_Organization_UID).ThenBy(w => w.Fixture_NO);
            var settingMDTOList = new List<Fixture_Part_Setting_MDTO>();

            foreach (var item in query)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Setting_MDTO>(item);
                if (item.System_Organization != null)
                {
                    dto.Plant_Organization_Name = item.System_Organization.Organization_Name;
                }
                if (item.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = item.System_Organization1.Organization_Name;
                }
                if (item.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = item.System_Organization2.Organization_Name;
                }
                if (item.Fixture_Part_Setting_D.Count > 0)
                {
                    var settingDList = new List<Fixture_Part_Setting_DDTO>();
                    foreach (var settingD in item.Fixture_Part_Setting_D)
                    {
                        var settingDDTO = AutoMapper.Mapper.Map<Fixture_Part_Setting_DDTO>(settingD);
                        settingDDTO.Part_ID = settingD.Fixture_Part.Part_ID;
                        settingDDTO.Part_Name = settingD.Fixture_Part.Part_Name;
                        settingDDTO.Part_Spec = settingD.Fixture_Part.Part_Spec;
                        settingDDTO.Created_UserName = settingD.System_Users.User_Name;
                        settingDDTO.Modified_UserName = settingD.System_Users1.User_Name;
                        settingDList.Add(settingDDTO);
                    }
                    dto.Fixture_Part_Setting_Ds = settingDList;
                }
                dto.Created_UserName = item.System_Users.User_Name;
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users1);
                dto.Modified_UserName = dto.ModifiedUser.User_Name;
                settingMDTOList.Add(dto);
            }
            return settingMDTOList;
        }

        public string AddFixturePartSetting(Fixture_Part_Setting_MDTO dto)
        {
            try
            {
                if (dto.Fixture_Part_Setting_Ds.Count > 0)
                {
                    //验证相同厂区，OP，功能厂治具图号
                    var settingMQuery = fixturePartSettingMRepository.GetMany(i => i.Plant_Organization_UID == dto.Plant_Organization_UID && i.BG_Organization_UID == dto.BG_Organization_UID && i.Fixture_NO == dto.Fixture_NO);
                    if (dto.FunPlant_Organization_UID != null && dto.FunPlant_Organization_UID != 0)
                    {
                        settingMQuery = settingMQuery.Where(i => i.FunPlant_Organization_UID == dto.FunPlant_Organization_UID);
                    }
                    if (settingMQuery.Count() > 0)
                    {
                        return "治具图号重复";
                    }

                    var fixturePartSettingM = AutoMapper.Mapper.Map<Fixture_Part_Setting_M>(dto);
                    var fixturePartSettingM_db = fixturePartSettingMRepository.Add(fixturePartSettingM);

                    foreach (var item in dto.Fixture_Part_Setting_Ds)
                    {
                        item.Fixture_Part_Setting_M_UID = fixturePartSettingM_db.Fixture_Part_Setting_M_UID;
                        AutoMapper.Mapper.Map<Fixture_Part_Setting_D>(item);
                    }
                    var settingDs = AutoMapper.Mapper.Map<List<Fixture_Part_Setting_D>>(dto.Fixture_Part_Setting_Ds);

                    fixturePartSettingDRepository.AddList(settingDs);
                    //List<FixtureDefectCode_SettingDTO> fixtureDefectCode_Settinglists = fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingByPlant(dto.Fixture_Defect_UIDs[0].Plant_Organization_UID);

                    //foreach (var item in dto.Fixture_Defect_UIDs)
                    //{
                    //    int funPlant_Organization_UID = 0;
                    //    if (dto.FunPlant_Organization_UID != null)
                    //    {
                    //        funPlant_Organization_UID = dto.FunPlant_Organization_UID.Value;

                    //    }
                    //    var fixtureDefectCode_Settings = fixtureDefectCode_SettingRepository.GetFixtureDefectCode_SettingList(dto.Plant_Organization_UID, dto.BG_Organization_UID, funPlant_Organization_UID, item.Fixture_Defect_UID, dto.Fixture_NO.Trim());

                    //    if (fixtureDefectCode_Settings != null && fixtureDefectCode_Settings.Count >= 1)
                    //    {
                    //        return string.Format("数据重复，{1}治具下已经配置有该治具异常原因代码{0}", dto.DefectCode_ID, dto.Fixture_NO);
                    //    }

                    //    FixtureDefectCode_Setting defectCode_Setting = new FixtureDefectCode_Setting();
                    //    defectCode_Setting.Plant_Organization_UID = dto.Plant_Organization_UID;
                    //    defectCode_Setting.BG_Organization_UID = dto.BG_Organization_UID;
                    //    defectCode_Setting.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    //    defectCode_Setting.Fixture_Defect_UID = item.Fixture_Defect_UID;
                    //    defectCode_Setting.Fixture_NO = dto.Fixture_NO.Trim();
                    //    defectCode_Setting.Is_Enable = item.Is_Enable;
                    //    defectCode_Setting.Created_UID = dto.Created_UID.Value;
                    //    defectCode_Setting.Modified_UID = dto.Modified_UID;
                    //    defectCode_Setting.Created_Date = DateTime.Now;
                    //    defectCode_Setting.Modified_Date = DateTime.Now;
                    //    fixtureDefectCode_SettingRepository.Add(defectCode_Setting);
                    //}

                    unitOfWork.Commit();

                    return "SUCCESS";
                }
                else
                {
                    return "没有治具配件配比数据";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string EditFixturePartSetting(Fixture_Part_Setting_MDTO dto)
        {
            try
            {
                if (dto.Fixture_Part_Setting_Ds.Count > 0)
                {
                    //验证相同厂区，OP，功能厂治具图号
                    var settingMQuery = fixturePartSettingMRepository.GetMany(i => i.Fixture_Part_Setting_M_UID != dto.Fixture_Part_Setting_M_UID && i.Plant_Organization_UID == dto.Plant_Organization_UID && i.BG_Organization_UID == dto.BG_Organization_UID && i.Fixture_NO == dto.Fixture_NO);
                    if (dto.FunPlant_Organization_UID != null && dto.FunPlant_Organization_UID != 0)
                    {
                        settingMQuery = settingMQuery.Where(i => i.FunPlant_Organization_UID == dto.FunPlant_Organization_UID);
                    }
                    if (settingMQuery.Count() > 0)
                    {
                        return "治具图号重复";
                    }

                    //更新Fixture_Part_Setting_M
                    var fixturePartSettingM_db = fixturePartSettingMRepository.GetFirstOrDefault(i => i.Fixture_Part_Setting_M_UID == dto.Fixture_Part_Setting_M_UID);
                    fixturePartSettingM_db.Plant_Organization_UID = dto.Plant_Organization_UID;
                    fixturePartSettingM_db.BG_Organization_UID = dto.BG_Organization_UID;
                    if (dto.FunPlant_Organization_UID > 0)
                    {
                        fixturePartSettingM_db.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    }
                    fixturePartSettingM_db.Fixture_NO = dto.Fixture_NO;
                    fixturePartSettingM_db.Line_Qty = dto.Line_Qty;
                    fixturePartSettingM_db.UseTimesScanInterval = dto.UseTimesScanInterval;
                    fixturePartSettingM_db.Line_Fixture_Ratio_Qty = dto.Line_Fixture_Ratio_Qty;
                    fixturePartSettingM_db.Is_Enable = dto.Is_Enable;
                    fixturePartSettingM_db.Modified_UID = dto.Modified_UID;
                    fixturePartSettingM_db.Modified_Date = dto.Modified_Date;

                    fixturePartSettingMRepository.Update(fixturePartSettingM_db);

                    //更新
                    var updateSettingDs = dto.Fixture_Part_Setting_Ds.Where(i => i.Fixture_Part_Setting_D_UID > 0).ToList();
                    foreach (var item in updateSettingDs)
                    {
                        var settingD_Db = fixturePartSettingDRepository.GetFirstOrDefault(i => i.Fixture_Part_Setting_D_UID == item.Fixture_Part_Setting_D_UID);
                        //判断有更新的栏位才更新
                        if (settingD_Db != null && (settingD_Db.Fixture_Part_Qty != item.Fixture_Part_Qty || settingD_Db.Fixture_Part_Life != item.Fixture_Part_Life || settingD_Db.Is_Enable != item.Is_Enable
                            || settingD_Db.IsUseTimesManagement != item.IsUseTimesManagement || settingD_Db.Is_Enable != item.Is_Enable || settingD_Db.Fixture_Part_Life_UseTimes != item.Fixture_Part_Life_UseTimes))
                        {
                            settingD_Db.Fixture_Part_Qty = item.Fixture_Part_Qty;
                            settingD_Db.Fixture_Part_Life = item.Fixture_Part_Life;
                            settingD_Db.IsUseTimesManagement = item.IsUseTimesManagement;
                            settingD_Db.Fixture_Part_Life_UseTimes = item.Fixture_Part_Life_UseTimes;
                            settingD_Db.Is_Enable = item.Is_Enable;
                            settingD_Db.Modified_UID = dto.Modified_UID;
                            settingD_Db.Modified_Date = dto.Modified_Date;
                            fixturePartSettingDRepository.Update(settingD_Db);
                        }
                    }

                    //删除
                    var updateUIDList = (from i in updateSettingDs select i.Fixture_Part_Setting_D_UID).ToList();
                    var deleteSettingDs = fixturePartSettingM_db.Fixture_Part_Setting_D.Where(i => !updateUIDList.Contains(i.Fixture_Part_Setting_D_UID)).ToList();
                    if (deleteSettingDs.Count > 0)
                    {
                        fixturePartSettingDRepository.DeleteList(deleteSettingDs);
                    }

                    //新增
                    var addSettingDs = dto.Fixture_Part_Setting_Ds.Except(updateSettingDs).ToList();
                    if (addSettingDs.Count > 0)
                    {
                        foreach (var item in addSettingDs)
                        {
                            item.Fixture_Part_Setting_M_UID = fixturePartSettingM_db.Fixture_Part_Setting_M_UID;
                        }
                        var settingDs = AutoMapper.Mapper.Map<List<Fixture_Part_Setting_D>>(addSettingDs);
                        fixturePartSettingDRepository.AddList(settingDs);
                    }

                    unitOfWork.Commit();
                    return "SUCCESS";
                }
                else
                {
                    return "没有治具配件配比数据";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DeleteFixturePartSetting(int uid)
        {
            var fixturePartSettingM = fixturePartSettingMRepository.GetFirstOrDefault(w => w.Fixture_Part_Setting_M_UID == uid);
            if (fixturePartSettingM != null)
            {
                try
                {
                    var fixturePartSettingDs = fixturePartSettingM.Fixture_Part_Setting_D;
                    if (fixturePartSettingDs.Count > 0)
                    {
                        fixturePartSettingDRepository.DeleteList(fixturePartSettingDs.ToList());
                    }
                    fixturePartSettingMRepository.Delete(fixturePartSettingM);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "SUCCESS";
        }

        public Fixture_Part_Setting_MDTO GetFixturePartSettingMDTOByUID(int Fixture_Part_Setting_M_UID)
        {
            var settingM = fixturePartSettingMRepository.GetAll().FirstOrDefault(i => i.Fixture_Part_Setting_M_UID == Fixture_Part_Setting_M_UID);
            if (settingM != null)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Setting_MDTO>(settingM);
                if (settingM.System_Organization != null)
                {
                    dto.Plant_Organization_Name = settingM.System_Organization.Organization_Name;
                }
                if (settingM.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = settingM.System_Organization1.Organization_Name;
                }
                if (settingM.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = settingM.System_Organization2.Organization_Name;
                }
                if (settingM.System_Users != null)
                {
                    dto.Created_UserName = settingM.System_Users.User_Name;
                }
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(settingM.System_Users1);
                if (settingM.Fixture_Part_Setting_D.Count > 0)
                {
                    var settingDs = new List<Fixture_Part_Setting_DDTO>();
                    foreach (var item in settingM.Fixture_Part_Setting_D)
                    {
                        var settingDDTO = AutoMapper.Mapper.Map<Fixture_Part_Setting_DDTO>(item);
                        settingDDTO.Part_ID = item.Fixture_Part.Part_ID;
                        settingDDTO.Part_Name = item.Fixture_Part.Part_Name;
                        settingDDTO.Part_Spec = item.Fixture_Part.Part_Spec;
                        settingDs.Add(settingDDTO);
                    }
                    dto.Fixture_Part_Setting_Ds = settingDs;
                }

                return dto;
            }
            return null;
        }

        //public IList<Fixture_PartDTO> GetFixturePartList(Fixture_PartModelSearch search)
        //{
        //    var query = fixturePartRepository.GetAll();
        //    if (search != null)
        //    {
        //        if (search.Plant_Organization_UID > 0)
        //        {
        //            query = query.Where(m => m.Plant_Organization_UID == search.Plant_Organization_UID);
        //        }
        //        if (search.BG_Organization_UID > 0)
        //        {
        //            query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID);
        //        }
        //        if (search.FunPlant_Organization_UID.HasValue)
        //        {
        //            query = query.Where(m => m.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
        //        }
        //        if (!string.IsNullOrWhiteSpace(search.Part_ID))
        //        {
        //            query = query.Where(w => w.Part_ID.Contains(search.Part_ID));
        //        }
        //        if (!string.IsNullOrWhiteSpace(search.Part_Name))
        //        {
        //            query = query.Where(w => w.Part_Name.Contains(search.Part_Name));
        //        }
        //        if (!string.IsNullOrWhiteSpace(search.Part_Spec))
        //        {
        //            query = query.Where(w => w.Part_Spec.Contains(search.Part_Spec));
        //        }
        //        if (search.Safe_Storage_Ratio.HasValue)
        //        {
        //            query = query.Where(w => w.Safe_Storage_Ratio == search.Safe_Storage_Ratio.Value);
        //        }
        //        if (search.Is_Automation.HasValue)
        //        {
        //            query = query.Where(w => w.Is_Automation == search.Is_Automation.Value);
        //        }
        //        if (search.Is_Standardized.HasValue)
        //        {
        //            query = query.Where(w => w.Is_Standardized == search.Is_Standardized.Value);
        //        }
        //        if (search.Is_Storage_Managed.HasValue)
        //        {
        //            query = query.Where(w => w.Is_Storage_Managed == search.Is_Storage_Managed.Value);
        //        }
        //        if (search.Is_Enable.HasValue)
        //        {
        //            query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
        //        }
        //    }

        //    query = query.OrderBy(m => m.Plant_Organization_UID).ThenBy(m => m.BG_Organization_UID).ThenBy(m => m.Part_ID);

        //    IList<Fixture_PartDTO> fixture_MachineDTOList = new List<Fixture_PartDTO>();

        //    foreach (var fixture_Machine in query)
        //    {
        //        var dto = AutoMapper.Mapper.Map<Fixture_PartDTO>(fixture_Machine);
        //        if (fixture_Machine.System_Organization != null)
        //        {
        //            dto.Plant_Organization_Name = fixture_Machine.System_Organization.Organization_Name;
        //        }
        //        if (fixture_Machine.System_Organization1 != null)
        //        {
        //            dto.BG_Organization_Name = fixture_Machine.System_Organization1.Organization_Name;
        //        }
        //        if (fixture_Machine.System_Organization2 != null)
        //        {
        //            dto.FunPlant_Organization_Name = fixture_Machine.System_Organization2.Organization_Name;
        //        }
        //        dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(fixture_Machine.System_Users1);
        //        fixture_MachineDTOList.Add(dto);
        //    }
        //    return fixture_MachineDTOList;
        //}

        //public Fixture_Part QueryFixture_PartSingle(int uid)
        //{
        //    return fixturePartRepository.GetById(uid);
        //}

        //public string AddFixture_Part(Fixture_Part fixturePart)
        //{
        //    fixturePartRepository.Add(fixturePart);
        //    unitOfWork.Commit();
        //    return "SUCCESS";
        //}

        //public string EditFixture_Part(Fixture_Part fixturePart)
        //{
        //    fixturePartRepository.Update(fixturePart);
        //    unitOfWork.Commit();
        //    return "SUCCESS";
        //}

        //public string DeleteFixture_Part(int uid)
        //{
        //    var fixturePart = fixturePartRepository.GetFirstOrDefault(w => w.Fixture_Part_UID == uid);
        //    if (fixturePart != null)
        //    {
        //        //如果有关联的数据则不可删,未完成
        //        //var fixture_MCount = fixturePart.Fixture_M.Count();
        //        //if (fixture_MCount > 0)
        //        //{
        //        //    return "HAVEREFERENCE";
        //        //}
        //        try
        //        {
        //            fixturePartRepository.Delete(fixturePart);
        //            unitOfWork.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //    }
        //    return "SUCCESS";
        //}

        public IList<Fixture_Part_Setting_MDTO> DoExportFixtuerPartSettingM(string uids)
        {
            var totalCount = 0;
            var fixture_Parts = fixturePartSettingMRepository.QueryFixturePartSettingMs(new Fixture_Part_Setting_MModelSearch { ExportUIds = uids }, null, out totalCount);

            IList<Fixture_Part_Setting_MDTO> fixture_PartDTOList = new List<Fixture_Part_Setting_MDTO>();

            foreach (var item in fixture_Parts)
            {
                var dto = AutoMapper.Mapper.Map<Fixture_Part_Setting_MDTO>(item);
                if (item.System_Organization != null)
                {
                    dto.Plant_Organization_Name = item.System_Organization.Organization_Name;
                }
                if (item.System_Organization1 != null)
                {
                    dto.BG_Organization_Name = item.System_Organization1.Organization_Name;
                }

                if (item.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_Name = item.System_Organization2.Organization_Name;
                }
                if (item.Fixture_Part_Setting_D.Count > 0)
                {
                    var settingDList = new List<Fixture_Part_Setting_DDTO>();
                    foreach (var settingD in item.Fixture_Part_Setting_D)
                    {
                        var settingDDTO = AutoMapper.Mapper.Map<Fixture_Part_Setting_DDTO>(settingD);
                        settingDDTO.Part_ID = settingD.Fixture_Part.Part_ID;
                        settingDDTO.Part_Name = settingD.Fixture_Part.Part_Name;
                        settingDDTO.Part_Spec = settingD.Fixture_Part.Part_Spec;
                        settingDDTO.Created_UserName = settingD.System_Users.User_Name;
                        settingDDTO.Modified_UserName = settingD.System_Users1.User_Name;
                        settingDList.Add(settingDDTO);
                    }
                    dto.Fixture_Part_Setting_Ds = settingDList;
                }
                dto.Created_UserName = item.System_Users.User_Name;
                dto.ModifiedUser = AutoMapper.Mapper.Map<SystemUserDTO>(item.System_Users1);
                dto.Modified_UserName = dto.ModifiedUser.User_Name;
                fixture_PartDTOList.Add(dto);
            }
            return fixture_PartDTOList;
        }

        public List<Fixture_Part_Setting_MDTO> QueryAllFixturePartSettingMs()
        {
            var settingMList = fixturePartSettingMRepository.GetAll().ToList();
            var dtoList = AutoMapper.Mapper.Map<List<Fixture_Part_Setting_MDTO>>(settingMList);
            return dtoList;
        }

        //public List<Fixture_Part_Setting_MDTO> GetManyFixturePartSettingM(Expression<Func<Fixture_Part_Setting_MDTO, bool>> where)
        //{
        //    LogHelper
        //    var x = new Fixture_Part_Setting_M();

        //    var body = default(Expression);
        //    var parameter = Expression.Parameter(x.GetType(), "p");

        //    var propertys = x.GetType().GetProperties();
        //    foreach (var property in propertys)
        //    {
        //        //var name = property.Name;
        //        var value = property.GetValue(x, null);
        //        if (value != null)
        //        {
        //            Expression propertyCondition = null;
        //            if (property.PropertyType == typeof(string))
        //            {
        //                if (!string.IsNullOrEmpty(value as string))
        //                {
        //                    propertyCondition = Expression.Call(Expression.Property(parameter, property.Name), typeof(string).GetMethod("Contains"), Expression.Constant(value));
        //                }
        //            }
        //            else
        //            {
        //                ExpressionVisitor
        //                MailHelper
        //                parameter.Property(property.Name).
        //            }

        //            if (propertyCondition != null)
        //            {
        //                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
        //            }
        //        }
        //    }

        //    var whereConvert = AutoMapper.Mapper.Map<Expression<Func<Fixture_Part_Setting_M, bool>>>(where);
        //    var settingMList = fixturePartSettingMRepository.GetMany(whereConvert).ToList();
        //    var dtoList = AutoMapper.Mapper.Map<List<Fixture_Part_Setting_MDTO>>(settingMList);
        //    return dtoList;
        //}

        public string InsertFixturePartSettingMs(List<Fixture_Part_Setting_MDTO> list)
        {
            var data = AutoMapper.Mapper.Map<List<Fixture_Part_Setting_M>>(list);
            string result = "";
            try
            {
                fixturePartSettingMRepository.AddList(data);
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }

        public string InsertOrUpdateFixturePartSettingDs(List<Fixture_Part_Setting_DDTO> list)
        {
            var data = AutoMapper.Mapper.Map<List<Fixture_Part_Setting_D>>(list);
            var dataAdd = new List<Fixture_Part_Setting_D>();
            foreach (var item in data)
            {
                var dataDb = fixturePartSettingDRepository.GetFirstOrDefault(c => c.Fixture_Part_Setting_M_UID == item.Fixture_Part_Setting_M_UID && c.Fixture_Part_UID == item.Fixture_Part_UID);
                if (dataDb != null)
                {
                    dataDb.Fixture_Part_Qty = item.Fixture_Part_Qty;
                    dataDb.Fixture_Part_Life = item.Fixture_Part_Life;
                    dataDb.IsUseTimesManagement = item.IsUseTimesManagement;
                    dataDb.Fixture_Part_Life_UseTimes = item.Fixture_Part_Life_UseTimes;
                    dataDb.Is_Enable = item.Is_Enable;
                    dataDb.Modified_UID = item.Modified_UID;
                    dataDb.Modified_Date = item.Modified_Date;
                    fixturePartSettingDRepository.Update(dataDb);
                }
                else
                {
                    dataAdd.Add(item);
                }
            }
            string result = "";
            try
            {
                if (dataAdd.Count > 0)
                {
                    fixturePartSettingDRepository.AddList(dataAdd);
                }
                unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                result = "Error" + ex;
            }
            return result;
        }
        #endregion

        #region 仓库管理
        public List<SystemOrgDTO> QueryOpType(int plantorguid)
        {
            var bud = fixturePartWarehouseRepository.GetOpType(plantorguid).ToList();
            return bud;
        }

        public PagedListModel<FixturePartWarehouseDTO> QueryWarehouses(FixturePartWarehouseDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixturePartWarehouseRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<FixturePartWarehouseDTO>(totalcount, result);
            return bd;
        }
        public List<SystemOrgDTO> QueryFunplantByop(int opuid)
        {
            var bud = fixturePartWarehouseRepository.GetFunplantsByop(opuid).ToList();
            return bud;
        }
        public List<FixturePartWarehouseDTO> QueryWar(int Warehouse_UID)
        {
            var bud = fixturePartWarehouseRepository.GetByUId(Warehouse_UID).ToList();
            return bud;

        }
        public List<FixturePartWarehouseStorageDTO> QueryWarhouseSts(int Warehouse_UID)
        {
            var bud = fixture_Warehouse_StorageRepository.GetMany(m => m.Fixture_Warehouse_UID == Warehouse_UID);
            var userAll = fixturePartWarehouseRepository.GetUserAll();
            List<FixturePartWarehouseStorageDTO> dtoList = new List<FixturePartWarehouseStorageDTO>();
            foreach (var item in bud)
            {
                dtoList.Add(AutoMapper.Mapper.Map<FixturePartWarehouseStorageDTO>(item));
            }

            foreach (var item in dtoList)
            {
                var createder = userAll.Where(o => o.Account_UID == item.Created_UID).FirstOrDefault();
                if (createder != null)
                {
                    item.Createder = createder.User_Name;
                }
                var modifier = userAll.Where(o => o.Account_UID == item.Modified_UID).FirstOrDefault();
                if (modifier != null)
                {
                    item.Modifier = modifier.User_Name;
                }
                //item.Createder = userAll.Where(o => o.Account_UID == item.Created_UID).FirstOrDefault().User_Name;
                //item.Modifier = userAll.Where(o => o.Account_UID == item.Modified_UID).FirstOrDefault().User_Name;
            }

            return dtoList;

        }
        public string AddOrEditWarehouseInfo(FixturePartWarehouseStorages dto)
        {
            try
            {
                var warehouse = fixturePartWarehouseRepository.GetById(dto.Fixture_Warehouse_UID);
                if (warehouse != null)
                {
                    var ware = fixturePartWarehouseRepository.GetById(dto.Fixture_Warehouse_UID);
                    ware.Plant_Organization_UID = dto.Plant_Organization_UID;
                    ware.BG_Organization_UID = dto.BG_Organization_UID;
                    ware.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    ware.Fixture_Warehouse_ID = dto.Fixture_Warehouse_ID;
                    ware.Fixture_Warehouse_Name = dto.Fixture_Warehouse_Name;
                    ware.Remarks = dto.Remarks;
                    ware.Is_Enable = dto.Is_Enable.Value;
                    //ware.Created_Date = DateTime.Now;
                    //ware.Created_UID = dto.Created_UID;
                    ware.Modified_UID = dto.Modified_UID;
                    ware.Modified_Date = DateTime.Now;
                    fixturePartWarehouseRepository.Update(ware);
                    unitOfWork.Commit();
                }
                else
                {
                    Fixture_Warehouse ware = new Fixture_Warehouse();
                    ware.Plant_Organization_UID = dto.Plant_Organization_UID;
                    ware.BG_Organization_UID = dto.BG_Organization_UID;
                    ware.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                    ware.Fixture_Warehouse_ID = dto.Fixture_Warehouse_ID;
                    ware.Fixture_Warehouse_Name = dto.Fixture_Warehouse_Name;
                    ware.Remarks = dto.Remarks;
                    ware.Is_Enable = dto.Is_Enable.Value;
                    ware.Created_Date = DateTime.Now;
                    ware.Created_UID = dto.Created_UID;
                    ware.Modified_UID = dto.Modified_UID;
                    ware.Modified_Date = DateTime.Now;
                    fixturePartWarehouseRepository.Add(ware);
                    unitOfWork.Commit();
                }
                foreach (var item in dto.Storages)
                {
                    if (dto.Fixture_Warehouse_UID == 0)
                    {
                        var war = fixturePartWarehouseRepository.GetFirstOrDefault(m => m.BG_Organization_UID == dto.BG_Organization_UID &&
                                            m.Plant_Organization_UID == dto.Plant_Organization_UID && m.Fixture_Warehouse_ID == dto.Fixture_Warehouse_ID);
                        dto.Fixture_Warehouse_UID = war.Fixture_Warehouse_UID;
                    }
                    if (item.Fixture_Warehouse_Storage_UID == 0)
                    {
                        Fixture_Warehouse_Storage ws = new Fixture_Warehouse_Storage();
                        ws.Plant_Organization_UID = dto.Plant_Organization_UID;
                        ws.BG_Organization_UID = dto.BG_Organization_UID;
                        ws.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        ws.Fixture_Warehouse_UID = dto.Fixture_Warehouse_UID;
                        ws.Rack_ID = item.Rack_ID;
                        ws.Storage_ID = item.Storage_ID;
                        ws.Remarks = item.Remarks;
                        ws.Is_Enable = item.Is_Enable;
                        ws.Created_Date = DateTime.Now;
                        ws.Created_UID = dto.Created_UID;
                        ws.Modified_UID = dto.Modified_UID;
                        ws.Modified_Date = DateTime.Now;

                        var hasdata = fixture_Warehouse_StorageRepository.GetFirstOrDefault(m => m.Fixture_Warehouse_UID == ws.Fixture_Warehouse_UID
                                                    & m.Rack_ID == ws.Rack_ID & m.Storage_ID == ws.Storage_ID & ws.Remarks == m.Remarks & ws.Is_Enable == item.Is_Enable);
                        if (hasdata != null)
                            return "更新仓库信息失败:仓库:" + dto.Fixture_Warehouse_UID + ",料架:" + ws.Rack_ID + ",储位:" + ws.Storage_ID + ",已经存在,不可重复添加";
                        fixture_Warehouse_StorageRepository.Add(ws);
                        unitOfWork.Commit();
                    }
                    else
                    {
                        var warehousestorate = fixture_Warehouse_StorageRepository.GetById(item.Fixture_Warehouse_Storage_UID);
                        warehousestorate.Plant_Organization_UID = dto.Plant_Organization_UID;
                        warehousestorate.BG_Organization_UID = dto.BG_Organization_UID;
                        warehousestorate.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                        warehousestorate.Fixture_Warehouse_UID = dto.Fixture_Warehouse_UID;
                        warehousestorate.Rack_ID = item.Rack_ID;
                        warehousestorate.Storage_ID = item.Storage_ID;
                        warehousestorate.Remarks = item.Remarks;
                        warehousestorate.Is_Enable = item.Is_Enable;
                        //warehousestorate.Created_Date = DateTime.Now;
                        //warehousestorate.Created_UID = dto.Created_UID;
                        warehousestorate.Modified_UID = dto.Modified_UID;
                        warehousestorate.Modified_Date = DateTime.Now;
                        //var hasdata = warehouseStorageRepository.GetFirstOrDefault(m => m.Warehouse_UID == warehousestorate.Warehouse_UID &
                        //                            m.Rack_ID == warehousestorate.Rack_ID & m.Storage_ID == warehousestorate.Storage_ID & m.Warehouse_Storage_UID != warehousestorate.Warehouse_Storage_UID);
                        //if (hasdata != null)
                        //{
                        //    return "修改储位信息失败:此储位信息已经存在";
                        //}
                        fixture_Warehouse_StorageRepository.Update(warehousestorate);
                        unitOfWork.Commit();
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
        public string DeleteWarehouseStorage(int WareStorage_UId)
        {

            string result = "";
            var entity = fixture_Warehouse_StorageRepository.GetFirstOrDefault(p => p.Fixture_Warehouse_Storage_UID == WareStorage_UId);
            if (entity == null)
            {
                result = "此储位已经删除";
            }
            else
            {
                try
                {
                    fixture_Warehouse_StorageRepository.Delete(entity);
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

        public string DeleteWarehouse(int Warehouse_UID)
        {
            try
            {
                var war = fixturePartWarehouseRepository.GetById(Warehouse_UID);
                fixturePartWarehouseRepository.Delete(war);
                unitOfWork.Commit();
                return "";
            }
            catch
            {
                return "该仓库所属的储位已经被使用,不能删除";
            }
        }

        public List<FixturePartWarehouseDTO> QueryAllWarehouseSt(int Plant_Organization_UID)
        {
            return fixturePartWarehouseRepository.GetAllInfo(Plant_Organization_UID);

        }
        public List<FixturePartWarehouseDTO> GetFixtureWarehouseStorageALL(int Plant_Organization_UID)
        {
            return fixturePartWarehouseRepository.GetFixtureWarehouseStorageALL(Plant_Organization_UID);
        }
        public string InsertWarehouseBase(List<FixturePartWarehouseDTO> dtolist)
        {

            return fixturePartWarehouseRepository.InsertWarehouse(dtolist);

        }
        public string InsertWarehouse(List<FixturePartWarehouseDTO> dtolist)
        {
            return fixturePartWarehouseRepository.InsertWarehouseStorage(dtolist);
        }
        public List<FixturePartWarehouseDTO> DoExportWarehouseReprot(string uids)
        {
            return fixturePartWarehouseRepository.DoExportWarehouseReprot(uids);
        }
        public List<FixturePartWarehouseDTO> DoAllExportWarehouseReprot(FixturePartWarehouseDTO searchModel)
        {
            return fixturePartWarehouseRepository.DoAllExportWarehouseReprot(searchModel);
        }
        #endregion


        #region 开账
        public PagedListModel<FixturePartStorageInboundDTO> QueryCreateBounds(FixturePartStorageInboundDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixture_Storage_InboundRepository.GetCreateInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<FixturePartStorageInboundDTO>(totalcount, result);
            return bd;
        }

        public List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO()
        {
            var bud = fixture_Storage_InboundRepository.GetFixtureStatuDTO();
            return bud;
        }

        public List<FixturePartCreateboundStatuDTO> GetFixtureStatuDTO(string Enum_Type)
        {
            var bud = fixture_Storage_InboundRepository.GetFixtureStatuDTO(Enum_Type);
            return bud;
        }
        public List<FixturePartWarehouseDTO> GetFixtureWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var bud = fixturePartWarehouseRepository.GetFixtureWarehouseStorages(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;

        }

        public List<Fixture_PartDTO> GetFixtureParts(int Fixture_Warehouse_Storage_UID)
        {
            Fixture_PartModelSearch search = new Fixture_PartModelSearch();
            var fixture_Warehouse_Storage = fixture_Warehouse_StorageRepository.GetMany(m => m.Fixture_Warehouse_Storage_UID == Fixture_Warehouse_Storage_UID).FirstOrDefault();
            if (fixture_Warehouse_Storage != null)
            {
                search.Plant_Organization_UID = fixture_Warehouse_Storage.Plant_Organization_UID;
                search.BG_Organization_UID = fixture_Warehouse_Storage.BG_Organization_UID;
                search.FunPlant_Organization_UID = fixture_Warehouse_Storage.FunPlant_Organization_UID;
            }
            return QueryAllFixtureParts(search).ToList();

        }

        public string AddOrEditCreateBound(FixturePartStorageInboundDTO dto)
        {
            try
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                FixturePartCreateboundStatuDTO fixtureStatus = fixtureStatuDTOs.FirstOrDefault(o => o.Status == "已删除");
                if (dto.Fixture_Storage_Inbound_UID == 0)
                {
                    var hasmatinventory = fixture_Storage_InboundRepository.GetMany(m => m.Fixture_Part_UID == dto.Fixture_Part_UID && m.Status_UID != fixtureStatus.Status_UID).ToList();
                    if (hasmatinventory.Count > 0)
                        return "此料号已开过账";
                    // List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                    List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
                    var fixturePart = fixturePartRepository.GetById(dto.Fixture_Part_UID);


                    dto.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
                    dto.Fixture_Storage_Inbound_Type_UID = inbound_Types.Where(o => o.Status == "开账").FirstOrDefault().Status_UID;
                    dto.Remarks = "期初開帳-" + fixturePart.Part_ID + "-" + fixturePart.Part_Name;
                    dto.Issue_NO = "None";
                    dto.Inbound_Price = 0;
                    var stoinbound = fixture_Storage_InboundRepository.GetAll().ToList();
                    string PreInboundID = "Opening" + DateTime.Today.ToString("yyyyMMdd");
                    var test = stoinbound.Where(m => m.Fixture_Storage_Inbound_ID.StartsWith(PreInboundID)).ToList();
                    string PostInboundID = (test.Count() + 1).ToString().PadLeft(4, '0');
                    dto.Fixture_Storage_Inbound_ID = PreInboundID + PostInboundID;
                    List<FixturePartStorageInboundDTO> list = new List<FixturePartStorageInboundDTO>();
                    list.Add(dto);
                    return InsertCreateBoundItem(list);
                }
                else
                {
                    var hasmatinventory = fixture_Storage_InboundRepository.GetMany(m => m.Fixture_Part_UID == dto.Fixture_Part_UID & m.Fixture_Storage_Inbound_UID != dto.Fixture_Storage_Inbound_UID).ToList();
                    if (hasmatinventory.Count > 0)
                        return "此料号已开过账";
                    var createbound = fixture_Storage_InboundRepository.GetFirstOrDefault(m => m.Fixture_Storage_Inbound_UID == dto.Fixture_Storage_Inbound_UID);
                    createbound.Fixture_Part_UID = dto.Fixture_Part_UID;
                    createbound.Fixture_Warehouse_Storage_UID = dto.Fixture_Warehouse_Storage_UID;
                    createbound.Inbound_Qty = dto.Inbound_Qty;
                    createbound.Applicant_UID = dto.Applicant_UID;
                    createbound.Applicant_Date = DateTime.Now;
                    createbound.Approve_UID = dto.Approve_UID;
                    createbound.Approve_Date = DateTime.Now;
                    fixture_Storage_InboundRepository.Update(createbound);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "新增开账 信息失败:" + e.Message;
            }
        }

        public string InsertCreateBoundItem(List<FixturePartStorageInboundDTO> dtolist)
        {
            return fixture_Storage_InboundRepository.InsertCreateBoundItem(dtolist);
        }

        public FixturePartStorageInboundDTO QueryInboundByUid(int Fixture_Storage_Inbound_UID)
        {

            return fixture_Storage_InboundRepository.QueryInboundByUid(Fixture_Storage_Inbound_UID);

        }
        public string DeleteCreateBound(int Fixture_Storage_Inbound_UID)
        {
            return fixture_Storage_InboundRepository.DeleteCreateBound(Fixture_Storage_Inbound_UID);
        }

        public List<FixturePartStorageInboundDTO> DoExportCreateBoundReprot(string uids)
        {
            return fixture_Storage_InboundRepository.DoExportCreateBoundReprot(uids);
        }
        public List<FixturePartStorageInboundDTO> DoAllExportCreateBoundReprot(FixturePartStorageInboundDTO search)
        {
            return fixture_Storage_InboundRepository.DoAllExportCreateBoundReprot(search);
        }

        public List<FixturePartStorageInboundDTO> QueryAllStorageInbound(int Plant_Organization_UID)
        {
            return fixture_Storage_InboundRepository.QueryAllStorageInbound(Plant_Organization_UID);
        }

        public string ApproveCreatebound(int Fixture_Storage_Inbound_UID, int Useruid)
        {
            try
            {
                var Createbound = fixture_Storage_InboundRepository.GetById(Fixture_Storage_Inbound_UID);
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                Createbound.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                Createbound.Approve_Date = DateTime.Now;
                Createbound.Approve_UID = Useruid;
                fixture_Storage_InboundRepository.Update(Createbound);

                #region 添加出入库明细表

                Fixture_Part_Inventory fixture_Part_Inventory = new Fixture_Part_Inventory();
                fixture_Part_Inventory.Fixture_Warehouse_Storage_UID = Createbound.Fixture_Warehouse_Storage_UID;
                fixture_Part_Inventory.Fixture_Part_UID = Createbound.Fixture_Part_UID;
                fixture_Part_Inventory.Inventory_Qty = Createbound.Inbound_Qty;
                fixture_Part_Inventory.Remarks = Createbound.Remarks;
                fixture_Part_Inventory.Modified_UID = Useruid;
                fixture_Part_Inventory.Modified_Date = DateTime.Now;
                fixture_Part_InventoryRepository.Add(fixture_Part_Inventory);


                #endregion

                #region  添加料号库存明细表
                Fixture_Storage_InOut_Detail fixture_Storage_InOut_Detail = new Fixture_Storage_InOut_Detail();
                List<FixturePartCreateboundStatuDTO> fixtureInbound_TypeStatuDTOs = GetFixtureStatuDTO("FixturePartIn_out_Type");
                int Status_UID = fixtureInbound_TypeStatuDTOs.FirstOrDefault(o => o.Status == "开账").Status_UID;
                fixture_Storage_InOut_Detail.InOut_Type_UID = Status_UID;
                //fixture_Storage_InOut_Detail.InOut_Type_UID = Createbound.Fixture_Storage_Inbound_Type_UID;
                fixture_Storage_InOut_Detail.Fixture_Storage_InOut_UID = Createbound.Fixture_Storage_Inbound_UID;
                fixture_Storage_InOut_Detail.Fixture_Part_UID = Createbound.Fixture_Part_UID;
                fixture_Storage_InOut_Detail.Fixture_Warehouse_Storage_UID = Createbound.Fixture_Warehouse_Storage_UID;
                fixture_Storage_InOut_Detail.InOut_Date = DateTime.Now;
                fixture_Storage_InOut_Detail.InOut_Qty = Createbound.Inbound_Qty;
                fixture_Storage_InOut_Detail.Balance_Qty = Createbound.Inbound_Qty;
                fixture_Storage_InOut_Detail.Remarks = Createbound.Remarks;
                fixture_Storage_InOut_Detail.Modified_UID = Useruid;
                fixture_Storage_InOut_Detail.Modified_Date = DateTime.Now;
                fixture_Storage_InOut_DetailRepository.Add(fixture_Storage_InOut_Detail);

                #endregion 添加料号库存明细表

                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        #endregion

        #region Add by Rock 2018-01-10 ----------------采购单维护作业----------start
        public PagedListModel<FixturePart_OrderQueryVM> QueryPurchase(FixturePart_OrderVM model, Page page)
        {
            var totalCount = 0;
            var result = fixturePartOrderMRepository.QueryPurchase(model, page, out totalCount);
            return new PagedListModel<FixturePart_OrderQueryVM>(totalCount, result);
        }


        public List<FixturePart_OrderEdit> doAllPurchaseorderMaintain(FixturePart_OrderVM model)
        {
            return fixturePartOrderMRepository.GetExportPurchaseData(model);
        }

        public List<FixturePart_OrderEdit> doPartPurchaseorderMaintain(string uids)
        {
            uids = "," + uids + ",";
            FixturePart_OrderVM model = new FixturePart_OrderVM();
            var list = fixturePartOrderMRepository.GetExportPurchaseData(model);
            var query = list.Where(m => uids.Contains("," + m.Fixture_Part_Order_M_UID + ","));
            return query.ToList();
        }

        public FixturePart_OrderEdit QueryPurchaseByUID(int Fixture_Part_Order_M_UID)
        {
            var result = fixturePartOrderMRepository.QueryPurchaseByUID(Fixture_Part_Order_M_UID);
            return result;
        }

        public List<FixturePartSettingMVM> GetFixturePartByPlantOptypeFunc(int PlantUID, int Optype, int FuncUID)
        {
            var list = fixturePartOrderMRepository.GetFixturePartByPlantOptypeFunc(PlantUID, Optype, FuncUID);
            return list;
        }

        public string DeletePurchaseByUIDS(List<int> idList)
        {
            string errorInfo = string.Empty;
            var hasItem = fixturePartOrderMRepository.GetMany(m => idList.Contains(m.Fixture_Part_Order_M_UID) && m.Is_Complated == true).FirstOrDefault();
            //判断订单状态是否是已提交
            var IsSubmit = fixturePartOrderMRepository.GetMany(m => idList.Contains(m.Fixture_Part_Order_M_UID) && m.Is_SubmitFlag == true).FirstOrDefault();
            if (IsSubmit != null)
            {
                errorInfo = "订单已经提交不能删除";
                return errorInfo;
            }

            if (hasItem != null)
            {
                errorInfo = "订单已经是完成状态不能删除";
                return errorInfo;
            }
            else
            {
                var masterList = fixturePartOrderMRepository.GetMany(m => idList.Contains(m.Fixture_Part_Order_M_UID) && m.Del_Flag == false).ToList();
                var masterIdList = masterList.Select(m => m.Fixture_Part_Order_M_UID).ToList();
                var detailList = fixturePartOrderDRepository.GetMany(m => masterIdList.Contains(m.Fixture_Part_Order_M_UID)).ToList();
                var detailIdList = detailList.Select(m => m.Fixture_Part_Order_D_UID).ToList();
                var threeList = fixture_Part_Order_ScheduleRepository.GetMany(m => detailIdList.Contains(m.Fixture_Part_Order_D_UID)).ToList();

                foreach (var item in masterList)
                {
                    item.Del_Flag = true;
                }
                foreach (var item in detailList)
                {
                    item.Del_Flag = true;
                }
                foreach (var item in threeList)
                {
                    item.Del_Flag = true;
                }
                unitOfWork.Commit();
            }
            return errorInfo;
        }

        public List<FixturePartPurchaseVM> GetFixturePartByMUIDAPI(int UID)
        {
            var list = fixturePartOrderMRepository.GetFixturePartByMUIDAPI(UID);
            return list;

        }

        public string SaveFixturePartByMUID(SubmitFixturePartOrderVM item)
        {
            var result = string.Empty;
            //新增
            if (item.Fixture_Part_Order_M_UID == 0)
            {
                result = fixturePartOrderMRepository.NewSaveFixturePartByMUID(item);
            }
            else
            {
                if (item.Is_Submit == "交货") //交货
                {
                    result = fixturePartOrderMRepository.SaveSubmitFixturePartByMUID(item);

                }
                else if (item.Is_Submit == "交期")
                {
                    result = fixturePartOrderMRepository.SaveDeliveryPeriodByMUID(item);
                }
                else //编辑
                {
                    result = fixturePartOrderMRepository.SaveFixturePartByMUID(item);
                }
            }
            return result;
        }
        #endregion Add by Rock 2018-01-10 ----------------采购单维护作业----------end


        #region  出入库维护作业
        public PagedListModel<FixturePartInOutBoundInfoDTO> QueryBoundDetails(FixturePartInOutBoundInfoDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixture_Storage_InboundRepository.GetDetailInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<FixturePartInOutBoundInfoDTO>(totalcount, result);
            return bd;
        }

        public List<FixturePartInOutBoundInfoDTO> ExportAllOutboundInfo(FixturePartInOutBoundInfoDTO searchModel)
        {
            var result = fixture_Storage_InboundRepository.ExportAllOutboundInfo(searchModel);
            return result;
        }

        public List<FixturePartInOutBoundInfoDTO> ExportPartOutboundInfo(string uids)
        {
            var result = fixture_Storage_InboundRepository.ExportPartOutboundInfo(uids);
            return result;
        }

        public List<Fixture_Part_Order_MDTO> GetFixture_Part_Order_Ms(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixturePartOrderMRepository.GetFixture_Part_Order_Ms(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;

        }
        public List<Fixture_PartDTO> GetFixturePartDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = fixturePartRepository.GetAll();
            if (Plant_Organization_UID > 0)
            {
                query = query.Where(w => w.Plant_Organization_UID == Plant_Organization_UID);
            }
            if (BG_Organization_UID > 0)
            {
                query = query.Where(w => w.BG_Organization_UID == BG_Organization_UID);
            }
            if (FunPlant_Organization_UID > 0)
            {
                query = query.Where(w => w.FunPlant_Organization_UID == FunPlant_Organization_UID);
            }
            query = query.OrderBy(w => w.Fixture_Part_UID);
            var dtoList = AutoMapper.Mapper.Map<IList<Fixture_PartDTO>>(query);
            return dtoList.ToList();
        }

        public List<Fixture_Part_InventoryDTO> GetWarehouseStorageByFixture_Part_UID(int Fixture_Part_UID)
        {

            return fixture_Part_InventoryRepository.GetWarehouseStorageByFixture_Part_UID(Fixture_Part_UID);
        }

        public List<Fixture_Part_InventoryDTO> GetMatinventory(int Fixture_Part_UID, int Fixture_Warehouse_Storage_UID)
        {
            return fixture_Part_InventoryRepository.GetMatinventory(Fixture_Part_UID, Fixture_Warehouse_Storage_UID);
        }

        public PagedListModel<Fixture_Part_InventoryDTO> GetFixtureinventory(int Fixture_Part_UID,
            int Fixture_Warehouse_Storage_UID, Page page)
        {
            var result = fixture_Part_InventoryRepository.GetFixtureinventory(Fixture_Part_UID, Fixture_Warehouse_Storage_UID, page);
            int totalcount = 0;
            var bd = new PagedListModel<Fixture_Part_InventoryDTO>(totalcount, result);
            return bd;
        }


        public List<Fixture_Part_Order_DDTO> GetFixture_Part_Order_Ds(int Fixture_Part_Order_M_UID)
        {
            var bud = fixturePartOrderDRepository.GetFixture_Part_Order_Ds(Fixture_Part_Order_M_UID);
            return bud;
        }
        public Fixture_Part_Order_DDTO GetFixture_Part_Order_D(int Fixture_Part_Order_D_UID)
        {
            var bud = fixturePartOrderDRepository.GetFixture_Part_Order_D(Fixture_Part_Order_D_UID);
            return bud;
        }

        public List<FixturePartWarehouseDTO> GetFixture_Part_Warehouses(int Fixture_Part_Order_D_UID)
        {
            Fixture_Part_Order_DDTO fixture_Part_Order_DDTO = fixturePartOrderDRepository.GetFixture_Part_Order_D(Fixture_Part_Order_D_UID);

            int funPlant_Organization_UID = 0;
            var FunPlant_Organization_UID = fixture_Part_Order_DDTO.FunPlant_Organization_UID;
            if (FunPlant_Organization_UID != null)
            {
                funPlant_Organization_UID = FunPlant_Organization_UID.Value;
            }
            return fixturePartWarehouseRepository.GetFixtureWarehouseStorages(fixture_Part_Order_DDTO.Plant_Organization_UID, fixture_Part_Order_DDTO.BG_Organization_UID, funPlant_Organization_UID);
        }

        public string AddOrEditInboundApply(FixturePartInOutBoundInfoDTO dto)
        {
            try
            {
                dto.Fixture_Part_UID = GetFixture_Part_Order_D(dto.Fixture_Part_Order_D_UID).Fixture_Part_UID;
                decimal actual_Delivery_Qty = 0;
                decimal IsInOutBoundQTY = IsInOutBound(dto.Fixture_Part_UID, dto.Fixture_Part_Order_M_UID.Value, out actual_Delivery_Qty);
                if (IsInOutBoundQTY >= dto.In_Out_Bound_Qty)
                {
                    if (dto.Storage_In_Out_Bound_UID == 0)
                    {
                        FixturePartStorageInboundDTO fixturePartStorageInbound = new FixturePartStorageInboundDTO();
                        List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                        List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");

                        var fixturePart = fixturePartRepository.GetById(dto.Fixture_Part_UID);
                        fixturePartStorageInbound.Fixture_Storage_Inbound_Type_UID = inbound_Types.Where(o => o.Status == "入库单").FirstOrDefault().Status_UID;
                        var stoinbound = fixture_Storage_InboundRepository.GetAll().ToList();
                        string PreInboundID = "In" + DateTime.Today.ToString("yyyyMMdd");
                        var test = stoinbound.Where(m => m.Fixture_Storage_Inbound_ID.StartsWith(PreInboundID)).ToList();
                        string PostInboundID = (test.Count() + 1).ToString().PadLeft(4, '0');
                        fixturePartStorageInbound.Fixture_Storage_Inbound_ID = PreInboundID + PostInboundID;
                        fixturePartStorageInbound.Fixture_Part_UID = dto.Fixture_Part_UID;
                        fixturePartStorageInbound.Fixture_Warehouse_Storage_UID = dto.Fixture_Warehouse_Storage_UID;
                        fixturePartStorageInbound.Fixture_Part_Order_M_UID = dto.Fixture_Part_Order_M_UID;
                        fixturePartStorageInbound.Inbound_Qty = dto.In_Out_Bound_Qty;
                        fixturePartStorageInbound.Inbound_Price = 0;
                        fixturePartStorageInbound.Issue_NO = dto.Issue_NO;
                        fixturePartStorageInbound.Remarks = "入库单-" + fixturePart.Part_ID + "-" + fixturePart.Part_Name;
                        fixturePartStorageInbound.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
                        fixturePartStorageInbound.Applicant_UID = dto.Applicant_UID;
                        fixturePartStorageInbound.Applicant_Date = DateTime.Now;
                        fixturePartStorageInbound.Approve_UID = dto.Approve_UID;
                        fixturePartStorageInbound.Approve_Date = DateTime.Now;
                        List<FixturePartStorageInboundDTO> list = new List<FixturePartStorageInboundDTO>();
                        list.Add(fixturePartStorageInbound);
                        return InsertInBoundItem(list);
                    }
                    else
                    {
                        var createbound = fixture_Storage_InboundRepository.GetFirstOrDefault(m => m.Fixture_Storage_Inbound_UID == dto.Storage_In_Out_Bound_UID);

                        createbound.Fixture_Part_UID = dto.Fixture_Part_UID;
                        createbound.Fixture_Warehouse_Storage_UID = dto.Fixture_Warehouse_Storage_UID;
                        createbound.Fixture_Part_Order_M_UID = dto.Fixture_Part_Order_M_UID;
                        createbound.Inbound_Qty = dto.In_Out_Bound_Qty;
                        createbound.Issue_NO = dto.Issue_NO;
                        createbound.Applicant_UID = dto.Applicant_UID;
                        createbound.Applicant_Date = DateTime.Now;
                        createbound.Approve_UID = dto.Approve_UID;
                        createbound.Approve_Date = DateTime.Now;
                        fixture_Storage_InboundRepository.Update(createbound);
                        unitOfWork.Commit();
                        return "0";
                    }
                }
                else
                {

                    return string.Format(@"实际交货总数量为：{0},已入库数量为{1},最多还能入库数量为{2}", actual_Delivery_Qty, actual_Delivery_Qty - IsInOutBoundQTY, IsInOutBoundQTY);
                }

            }
            catch (Exception e)
            {
                return "入库操作失败:" + e.Message;
            }
        }

        public decimal IsInOutBound(int Fixture_Part_UID, int Fixture_Part_Order_M_UID, out decimal actual_Delivery_Qty)
        {

            //同一个采购单号的下的单个料的实际交货总数
            actual_Delivery_Qty = 0;
            //根据配件主键和采购主键获取单个配件的明细
            List<int> fixturePartOrderD_UIDs = fixturePartOrderDRepository.GetAll().Where(o => o.Fixture_Part_UID == Fixture_Part_UID && o.Fixture_Part_Order_M_UID == Fixture_Part_Order_M_UID).Select(o => o.Fixture_Part_Order_D_UID).ToList();
            //获取单个配件的实际交货数量
            List<Fixture_Part_Order_Schedule> fixture_Part_Order_Schedules = fixture_Part_Order_ScheduleRepository.GetAll().Where(o => fixturePartOrderD_UIDs.Contains(o.Fixture_Part_Order_D_UID)).ToList();

            if (fixture_Part_Order_Schedules != null && fixture_Part_Order_Schedules.Count > 0)
            {
                foreach (var item in fixture_Part_Order_Schedules)
                {
                    if (item.Actual_Delivery_Qty != null)
                    {
                        actual_Delivery_Qty += item.Actual_Delivery_Qty.Value;
                    }
                }
            }

            decimal Inbound_Qty = 0;
            List<Fixture_Storage_Inbound> fixture_Storage_Inbounds = fixture_Storage_InboundRepository.GetAll().Where(o => o.Fixture_Part_Order_M_UID == Fixture_Part_Order_M_UID && o.Fixture_Part_UID == Fixture_Part_UID).ToList();

            if (fixture_Storage_Inbounds != null && fixture_Storage_Inbounds.Count > 0)
            {
                foreach (var item in fixture_Storage_Inbounds)
                {

                    Inbound_Qty += item.Inbound_Qty;
                }
            }
            //剩余的最大可入库数量
            return actual_Delivery_Qty - Inbound_Qty;
        }

        public string InsertInBoundItem(List<FixturePartStorageInboundDTO> dtolist)
        {
            return fixture_Storage_InboundRepository.InsertInItem(dtolist);
        }

        public FixturePartInOutBoundInfoDTO QueryInBoudSingle(int Storage_In_Out_Bound_UID)
        {

            var bud = fixture_Storage_InboundRepository.GetByUId(Storage_In_Out_Bound_UID).FirstOrDefault();
            if (bud.Fixture_Part_Order_M_UID != null)
            {
                List<Fixture_Part_Order_DDTO> Fixture_Part_Order_DDTOs = fixturePartOrderDRepository.GetFixture_Part_Order_Ds(bud.Fixture_Part_Order_M_UID.Value);
                bud.Fixture_Part_Order_D_UID = Fixture_Part_Order_DDTOs.FirstOrDefault(o => o.Fixture_Part_UID == bud.Fixture_Part_UID).Fixture_Part_Order_D_UID;
            }

            return bud;
        }
        public FixturePartInOutBoundInfoDTO QueryOutBoudSingle(int Storage_In_Out_Bound_UID)
        {
            var bud = fixture_Storage_Outbound_MRepository.GetByUId(Storage_In_Out_Bound_UID).FirstOrDefault();
            return bud;
        }

        public List<Fixture_Storage_Outbound_DDTO> QueryOutBouddetails(int Storage_In_Out_Bound_UID)
        {

            var bud = fixture_Storage_Outbound_DRepository.QueryOutBouddetails(Storage_In_Out_Bound_UID);
            return bud;
        }
        public List<FixturePartWarehouseDTO> GetFixtureWarehouses(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            return fixturePartWarehouseRepository.GetFixtureWarehouses(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
        }
        public string SaveWarehouseStorage(FixturePartWarehouseStorageDTO dto)
        {
            try
            {
                Fixture_Warehouse_Storage ws = new Fixture_Warehouse_Storage();
                ws.Plant_Organization_UID = dto.Plant_Organization_UID;
                ws.BG_Organization_UID = dto.BG_Organization_UID;
                ws.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
                ws.Fixture_Warehouse_UID = dto.Fixture_Warehouse_UID;
                ws.Rack_ID = dto.Rack_ID;
                ws.Storage_ID = dto.Storage_ID;
                ws.Remarks = dto.Remarks;
                ws.Is_Enable = true;
                ws.Created_Date = DateTime.Now;
                ws.Created_UID = dto.Created_UID;
                ws.Modified_UID = dto.Modified_UID;
                ws.Modified_Date = DateTime.Now;

                var hasdata = fixture_Warehouse_StorageRepository.GetFirstOrDefault(m => m.Fixture_Warehouse_UID == ws.Fixture_Warehouse_UID
                                            & m.Rack_ID == ws.Rack_ID & m.Storage_ID == ws.Storage_ID & ws.Remarks == m.Remarks & ws.Is_Enable == true);
                if (hasdata != null)
                    return "更新仓库信息失败:仓库:" + dto.Fixture_Warehouse_UID + ",料架:" + ws.Rack_ID + ",储位:" + ws.Storage_ID + ",已经存在,不可重复添加";
                fixture_Warehouse_StorageRepository.Add(ws);
                unitOfWork.Commit();
                return "SUCCESS";
            }
            catch (Exception e)
            {
                return "更新仓库信息失败:" + e.Message;
            }
        }

        public string DeleteBound(int Storage_In_Out_Bound_UID, string Inout_Type)
        {
            if (Inout_Type == "入库单")
            {
                return fixture_Storage_InboundRepository.DeleteCreateBound(Storage_In_Out_Bound_UID);
            }
            else
            {
                return fixture_Storage_Outbound_MRepository.DeleteBound(Storage_In_Out_Bound_UID);
            }

        }

        public string ApproveInbound(int Storage_In_Out_Bound_UID, int Useruid)
        {
            try
            {
                var Createbound = fixture_Storage_InboundRepository.GetById(Storage_In_Out_Bound_UID);
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                Createbound.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                Createbound.Approve_Date = DateTime.Now;
                Createbound.Approve_UID = Useruid;
                fixture_Storage_InboundRepository.Update(Createbound);

                #region 添加出入库明细表

                Fixture_Storage_InOut_Detail fixture_Storage_InOut_Detail = new Fixture_Storage_InOut_Detail();
                List<FixturePartCreateboundStatuDTO> fixtureInbound_TypeStatuDTOs = GetFixtureStatuDTO("FixturePartIn_out_Type");
                int Status_UID = fixtureInbound_TypeStatuDTOs.FirstOrDefault(o => o.Status == "入库单").Status_UID;
                fixture_Storage_InOut_Detail.InOut_Type_UID = Status_UID;
                fixture_Storage_InOut_Detail.Fixture_Storage_InOut_UID = Createbound.Fixture_Storage_Inbound_UID;
                fixture_Storage_InOut_Detail.Fixture_Part_UID = Createbound.Fixture_Part_UID;
                fixture_Storage_InOut_Detail.Fixture_Warehouse_Storage_UID = Createbound.Fixture_Warehouse_Storage_UID;
                fixture_Storage_InOut_Detail.InOut_Date = DateTime.Now;
                fixture_Storage_InOut_Detail.InOut_Qty = Createbound.Inbound_Qty;
                //数量要处理
                //因为计算结存数量时不区分储位,所以此处matqty不限定储位 分厂区和BG统计
                decimal matqty = 0;
                //var inventorys = fixture_Part_InventoryRepository.GetMany(m => m.Fixture_Part_UID == Createbound.Fixture_Part_UID
                //&& m.Fixture_Warehouse_Storage.Plant_Organization_UID == Createbound.Fixture_Warehouse_Storage.Plant_Organization_UID
                //&& m.Fixture_Warehouse_Storage.BG_Organization_UID == Createbound.Fixture_Warehouse_Storage.BG_Organization_UID);

                var inventorys = fixture_Part_InventoryRepository.GetMany(m => m.Fixture_Part_UID == Createbound.Fixture_Part_UID);
                if (inventorys.Count() > 0)
                {
                    matqty = inventorys.Sum(m => m.Inventory_Qty);
                }
                fixture_Storage_InOut_Detail.Balance_Qty = matqty + Createbound.Inbound_Qty;
                fixture_Storage_InOut_Detail.Remarks = Createbound.Remarks;
                fixture_Storage_InOut_Detail.Modified_UID = Useruid;
                fixture_Storage_InOut_Detail.Modified_Date = DateTime.Now;
                fixture_Storage_InOut_DetailRepository.Add(fixture_Storage_InOut_Detail);


                #endregion

                #region  添加料号库存明细表

                var matinven = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == Createbound.Fixture_Part_UID &&
                       m.Fixture_Warehouse_Storage_UID == Createbound.Fixture_Warehouse_Storage_UID);
                if (matinven != null)
                {
                    matinven.Modified_Date = DateTime.Now;
                    matinven.Modified_UID = Useruid;
                    matinven.Remarks = Createbound.Remarks;
                    //数量要处理
                    matinven.Inventory_Qty += Createbound.Inbound_Qty;
                    fixture_Part_InventoryRepository.Update(matinven);
                }
                else
                {
                    Fixture_Part_Inventory fixture_Part_Inventory = new Fixture_Part_Inventory();
                    fixture_Part_Inventory.Fixture_Warehouse_Storage_UID = Createbound.Fixture_Warehouse_Storage_UID;
                    fixture_Part_Inventory.Fixture_Part_UID = Createbound.Fixture_Part_UID;
                    //数量要处理          
                    fixture_Part_Inventory.Inventory_Qty = Createbound.Inbound_Qty;
                    fixture_Part_Inventory.Remarks = Createbound.Remarks;
                    fixture_Part_Inventory.Modified_UID = Useruid;
                    fixture_Part_Inventory.Modified_Date = DateTime.Now;
                    fixture_Part_InventoryRepository.Add(fixture_Part_Inventory);
                }

                #endregion 添加料号库存明细表

                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }

        }

        public string AddOrEditOutbound(FixturePartInOutBoundInfoDTO dto)
        {
            try
            {
                if (dto.details.Count() != dto.details.GroupBy(m => m.Fixture_Part_UID & m.Fixture_Warehouse_Storage_UID).Count())
                {
                    return "不可重复添加同储位的同料号";
                }
                if (dto.Storage_In_Out_Bound_UID == 0)
                {
                    List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                    Fixture_Storage_Outbound_M SOM = new Fixture_Storage_Outbound_M();
                    SOM.Fixture_Storage_Outbound_Type_UID = dto.Storage_In_Out_Bound_Type_UID;
                    string preOutbound = "Out" + DateTime.Today.ToString("yyyyMMdd");
                    var test = fixture_Storage_Outbound_MRepository.GetMany(m => m.Fixture_Storage_Outbound_ID.StartsWith(preOutbound)).ToList();
                    string proOutboundID = (test.Count() + 1).ToString().PadLeft(4, '0');
                    SOM.Fixture_Storage_Outbound_ID = preOutbound + proOutboundID;
                    if (dto.Fixture_Repair_M_UID != 0)
                    {
                        SOM.Fixture_Repair_M_UID = dto.Fixture_Repair_M_UID;
                    }
                    if (dto.Outbound_Account_UID != 0)
                    {
                        SOM.Outbound_Account_UID = dto.Outbound_Account_UID;
                    }

                    SOM.Remarks = dto.Remarks;
                    SOM.Applicant_UID = dto.Modified_UID;
                    SOM.Applicant_Date = DateTime.Now;
                    SOM.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
                    SOM.Approve_UID = dto.Modified_UID;
                    SOM.Approve_Date = DateTime.Now;
                    fixture_Storage_Outbound_MRepository.Add(SOM);
                    unitOfWork.Commit();

                    //新增子表
                    foreach (Fixture_Storage_Outbound_DDTO detail in dto.details)
                    {

                        var outbound_uid = fixture_Storage_Outbound_MRepository.GetFirstOrDefault(m => m.Fixture_Storage_Outbound_ID == preOutbound + proOutboundID).Fixture_Storage_Outbound_M_UID;
                        Fixture_Storage_Outbound_D SOD = new Fixture_Storage_Outbound_D();
                        SOD.Fixture_Storage_Outbound_M_UID = outbound_uid;
                        SOD.Fixture_Part_UID = detail.Fixture_Part_UID;
                        SOD.Fixture_Warehouse_Storage_UID = detail.Fixture_Warehouse_Storage_UID;
                        SOD.Outbound_Qty = detail.Outbound_Qty;
                        SOD.Remarks = dto.In_Out_Type + '-' + detail.Part_ID + '-' + detail.Part_Name;
                        fixture_Storage_Outbound_DRepository.Add(SOD);
                    }
                }
                else
                {
                    var outboundM = fixture_Storage_Outbound_MRepository.GetFirstOrDefault(m => m.Fixture_Storage_Outbound_M_UID == dto.Storage_In_Out_Bound_UID);
                    var outboundDs = fixture_Storage_Outbound_DRepository.GetMany(m => m.Fixture_Storage_Outbound_M_UID == dto.Storage_In_Out_Bound_UID).ToList();
                    foreach (Fixture_Storage_Outbound_D item in outboundDs)
                    {
                        fixture_Storage_Outbound_DRepository.Delete(item);
                    }
                    if (dto.Fixture_Repair_M_UID != 0)
                    {
                        outboundM.Fixture_Repair_M_UID = dto.Fixture_Repair_M_UID;
                    }
                    if (dto.Outbound_Account_UID != 0)
                    {
                        outboundM.Outbound_Account_UID = dto.Outbound_Account_UID;
                    }
                    outboundM.Remarks = dto.Remarks;
                    outboundM.Applicant_UID = dto.Modified_UID;
                    outboundM.Applicant_Date = DateTime.Now;
                    outboundM.Approve_UID = dto.Modified_UID;
                    outboundM.Approve_Date = DateTime.Now;
                    fixture_Storage_Outbound_MRepository.Update(outboundM);
                    //新增子表
                    foreach (Fixture_Storage_Outbound_DDTO detail in dto.details)
                    {
                        Fixture_Storage_Outbound_D SOD = new Fixture_Storage_Outbound_D();
                        SOD.Fixture_Storage_Outbound_M_UID = dto.Storage_In_Out_Bound_UID;
                        SOD.Fixture_Part_UID = detail.Fixture_Part_UID;
                        SOD.Fixture_Warehouse_Storage_UID = detail.Fixture_Warehouse_Storage_UID;
                        SOD.Outbound_Qty = detail.Outbound_Qty;
                        SOD.Remarks = dto.In_Out_Type + '-' + detail.Part_ID + '-' + detail.Part_Name;
                        fixture_Storage_Outbound_DRepository.Add(SOD);
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

        public string ApproveOutbound(int Storage_In_Out_Bound_UID, int Useruid)
        {
            try
            {
                var Createbound = fixture_Storage_Outbound_MRepository.GetById(Storage_In_Out_Bound_UID);
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                Createbound.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                Createbound.Approve_Date = DateTime.Now;
                Createbound.Approve_UID = Useruid;
                fixture_Storage_Outbound_MRepository.Update(Createbound);

                List<FixturePartCreateboundStatuDTO> fixtureInbound_TypeStatuDTOs = GetFixtureStatuDTO("FixturePartIn_out_Type");
                string In_Out_Type = Createbound.Enumeration.Enum_Value;
                int Status_UID = fixtureInbound_TypeStatuDTOs.FirstOrDefault(o => o.Status == In_Out_Type).Status_UID;

                List<Fixture_Storage_Outbound_DDTO> Fixture_Storage_Outbound_DDTOs = QueryOutBouddetails(Storage_In_Out_Bound_UID);
                foreach (var item in Fixture_Storage_Outbound_DDTOs)
                {
                    #region 修改出库明细表
                    Fixture_Storage_InOut_Detail fixture_Storage_InOut_Detail = new Fixture_Storage_InOut_Detail();

                    fixture_Storage_InOut_Detail.InOut_Type_UID = Status_UID;
                    fixture_Storage_InOut_Detail.Fixture_Storage_InOut_UID = Createbound.Fixture_Storage_Outbound_M_UID;
                    fixture_Storage_InOut_Detail.Fixture_Part_UID = item.Fixture_Part_UID;
                    fixture_Storage_InOut_Detail.Fixture_Warehouse_Storage_UID = item.Fixture_Warehouse_Storage_UID;
                    fixture_Storage_InOut_Detail.InOut_Date = DateTime.Now;
                    fixture_Storage_InOut_Detail.InOut_Qty = item.In_Out_Bound_Qty;
                    //数量要处理
                    //因为计算结存数量时不区分储位,所以此处matqty不限定储位 分厂区和BG统计
                    decimal matqty = 0;
                    var inventorys = fixture_Part_InventoryRepository.GetMany(m => m.Fixture_Part_UID == item.Fixture_Part_UID);
                    if (inventorys.Count() > 0)
                    {
                        matqty = inventorys.Sum(m => m.Inventory_Qty);
                    }
                    fixture_Storage_InOut_Detail.Balance_Qty = matqty - item.In_Out_Bound_Qty;
                    fixture_Storage_InOut_Detail.Remarks = In_Out_Type + '-' + item.Fixture_Part_UID + '-' + item.Fixture_Warehouse_Storage_UID;
                    fixture_Storage_InOut_Detail.Modified_UID = Useruid;
                    fixture_Storage_InOut_Detail.Modified_Date = DateTime.Now;
                    fixture_Storage_InOut_DetailRepository.Add(fixture_Storage_InOut_Detail);

                    #endregion 修改出库明细表

                    #region  修改料号库存明细表

                    var matinven = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == item.Fixture_Part_UID &&
                        m.Fixture_Warehouse_Storage_UID == item.Fixture_Warehouse_Storage_UID);
                    if (matinven != null)
                    {
                        if (matinven.Inventory_Qty < 0)
                        {
                            return "库存量不足,无法审核";
                        }
                        matinven.Modified_Date = DateTime.Now;
                        matinven.Modified_UID = Useruid;
                        matinven.Remarks = In_Out_Type + '-' + item.Fixture_Part_UID + '-' + item.Fixture_Warehouse_Storage_UID;
                        //数量要处理
                        matinven.Inventory_Qty -= item.In_Out_Bound_Qty;
                        fixture_Part_InventoryRepository.Update(matinven);
                    }
                    else
                    {
                        return "库存量不足,无法审核";
                    }

                    #endregion 修改出入库明细表

                }
                unitOfWork.Commit();

                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }

        }
        #endregion


        #region 储位移转维护作业
        public PagedListModel<Fixture_Storage_TransferDTO> QueryStorageTransfers(Fixture_Storage_TransferDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixture_Storage_TransferRepository.QueryStorageTransfers(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<Fixture_Storage_TransferDTO>(totalcount, result);
            return bd;
        }

        public List<Fixture_Warehouse_StorageDTO> GetFixturePartWarehouseStorages(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var bud = fixture_Warehouse_StorageRepository.GetFixturePartWarehouseStorages(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
            return bud;

        }

        public string AddOrEditStorageTransfer(Fixture_Storage_TransferDTO dto)
        {
            try
            {

                if (dto.Fixture_Storage_Transfer_UID == 0)
                {
                    List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                    Fixture_Storage_Transfer fixture_Storage_Transfer = new Fixture_Storage_Transfer();
                    // 'Transfer' + YYYYMMDD + 流水號4碼(0001~9999)。
                    string preOutbound = "Transfer" + DateTime.Today.ToString("yyyyMMdd");
                    var test = fixture_Storage_TransferRepository.GetMany(m => m.Fixture_Storage_Transfer_ID.StartsWith(preOutbound)).ToList();
                    string proOutboundID = (test.Count() + 1).ToString().PadLeft(4, '0');
                    fixture_Storage_Transfer.Fixture_Storage_Transfer_ID = preOutbound + proOutboundID;
                    fixture_Storage_Transfer.Fixture_Part_UID = dto.Fixture_Part_UID;
                    fixture_Storage_Transfer.Out_Fixture_Warehouse_Storage_UID = dto.Out_Fixture_Warehouse_Storage_UID;
                    fixture_Storage_Transfer.In_Fixture_Warehouse_Storage_UID = dto.In_Fixture_Warehouse_Storage_UID;
                    fixture_Storage_Transfer.Transfer_Qty = dto.Transfer_Qty;
                    fixture_Storage_Transfer.Applicant_UID = dto.Applicant_UID;
                    fixture_Storage_Transfer.Applicant_Date = DateTime.Now;
                    fixture_Storage_Transfer.Approver_UID = dto.Approver_UID;
                    fixture_Storage_Transfer.Approver_Date = DateTime.Now;
                    fixture_Storage_Transfer.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;

                    fixture_Storage_TransferRepository.Add(fixture_Storage_Transfer);
                }
                else
                {
                    var fixture_Storage_Transfer = fixture_Storage_TransferRepository.GetById(dto.Fixture_Storage_Transfer_UID);
                    fixture_Storage_Transfer.Fixture_Part_UID = dto.Fixture_Part_UID;
                    fixture_Storage_Transfer.Out_Fixture_Warehouse_Storage_UID = dto.Out_Fixture_Warehouse_Storage_UID;
                    fixture_Storage_Transfer.In_Fixture_Warehouse_Storage_UID = dto.In_Fixture_Warehouse_Storage_UID;
                    fixture_Storage_Transfer.Transfer_Qty = dto.Transfer_Qty;
                    fixture_Storage_Transfer.Applicant_UID = dto.Applicant_UID;
                    fixture_Storage_Transfer.Applicant_Date = DateTime.Now;
                    fixture_Storage_Transfer.Approver_UID = dto.Approver_UID;
                    fixture_Storage_Transfer.Approver_Date = DateTime.Now;

                    fixture_Storage_TransferRepository.Update(fixture_Storage_Transfer);
                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "出库操作失败:" + e.Message;
            }
        }
        public Fixture_Storage_TransferDTO QueryStorageTransferByUid(int Fixture_Storage_Transfer_UID)
        {
            return fixture_Storage_TransferRepository.QueryStorageTransferByUid(Fixture_Storage_Transfer_UID);
        }
        public string DeleteStorageTransfer(int Fixture_Storage_Transfer_UID, int userid)
        {
            try
            {
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                var storagetransfer = fixture_Storage_TransferRepository.GetById(Fixture_Storage_Transfer_UID);
                storagetransfer.Applicant_UID = userid;
                storagetransfer.Applicant_Date = DateTime.Now;
                storagetransfer.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已删除").FirstOrDefault().Status_UID;
                fixture_Storage_TransferRepository.Update(storagetransfer);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除储位移转单失败:" + e.Message;
            }
        }
        public List<Fixture_Storage_TransferDTO> DoExportStorageTransferReprot(string uids)
        {

            return fixture_Storage_TransferRepository.DoExportStorageTransferReprot(uids);
        }
        public List<Fixture_Storage_TransferDTO> DoAllExportStorageTransferReprot(Fixture_Storage_TransferDTO search)
        {
            return fixture_Storage_TransferRepository.DoAllExportStorageTransferReprot(search);
        }
        public List<Fixture_Part_InventoryDTO> Fixture_Part_InventoryDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            return fixture_Part_InventoryRepository.Fixture_Part_InventoryDTOList(Plant_Organization_UID, BG_Organization_UID, FunPlant_Organization_UID);
        }
        //public string ImportStorageTransfer(List<Fixture_Storage_TransferDTO> dtolist)
        //{
        //    try
        //    {
        //        if (dtolist.Count > 0)
        //        {
        //            string preOutbound = "Transfer" + DateTime.Today.ToString("yyyyMMdd");
        //            var test = fixture_Storage_TransferRepository.GetMany(m => m.Fixture_Storage_Transfer_ID.StartsWith(preOutbound)).ToList();
        //            int i = 1;
        //            List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
        //            foreach (var item in dtolist)
        //            {
        //                if (item.Out_Fixture_Warehouse_Storage_UID == item.In_Fixture_Warehouse_Storage_UID)
        //                {
        //                    return "转入储位与转出储位不可一致";
        //                }
        //                if (item.Fixture_Storage_Transfer_UID == 0)
        //                {
        //                    Fixture_Storage_Transfer fixture_Storage_Transfer = new Fixture_Storage_Transfer();                                                 
        //                    string proOutboundID = (test.Count() + i).ToString().PadLeft(4, '0');
        //                    fixture_Storage_Transfer.Fixture_Storage_Transfer_ID = preOutbound + proOutboundID;
        //                    fixture_Storage_Transfer.Fixture_Part_UID = item.Fixture_Part_UID;
        //                    fixture_Storage_Transfer.Out_Fixture_Warehouse_Storage_UID = item.Out_Fixture_Warehouse_Storage_UID;
        //                    fixture_Storage_Transfer.In_Fixture_Warehouse_Storage_UID = item.In_Fixture_Warehouse_Storage_UID;
        //                    fixture_Storage_Transfer.Transfer_Qty = item.Transfer_Qty;
        //                    fixture_Storage_Transfer.Applicant_UID = item.Applicant_UID;
        //                    fixture_Storage_Transfer.Applicant_Date = DateTime.Now;
        //                    fixture_Storage_Transfer.Approver_UID = item.Approver_UID;
        //                    fixture_Storage_Transfer.Approver_Date = DateTime.Now;
        //                    fixture_Storage_Transfer.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
        //                    fixture_Storage_TransferRepository.Add(fixture_Storage_Transfer);
        //                    unitOfWork.Commit();
        //                    i++;
        //                }
        //            }

        //        }
        //        return "";
        //    }
        //    catch (Exception e)
        //    {
        //        return "储位移转失败:" + e.Message;
        //    }
        //}
        public string ImportStorageTransfer(List<Fixture_Storage_TransferDTO> dtolist)
        {

            string preOutbound = "Transfer" + DateTime.Today.ToString("yyyyMMdd");
            var test = fixture_Storage_TransferRepository.GetMany(m => m.Fixture_Storage_Transfer_ID.StartsWith(preOutbound)).ToList();
            int i = 1;
            List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            int Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
            foreach (var item in dtolist)
            {
                if (item.Fixture_Storage_Transfer_UID == 0)
                {
                    string proOutboundID = (test.Count() + i).ToString().PadLeft(4, '0');
                    item.Fixture_Storage_Transfer_ID = preOutbound + proOutboundID;
                    item.Status_UID = Status_UID;
                    i++;
                }
            }

            return fixture_Storage_TransferRepository.InsertStorageTransfer(dtolist);
        }
        public string ApproveStTransfer(int Fixture_Storage_Transfer_UID, int Useruid)
        {
            try
            {
                var StorageTransfer = fixture_Storage_TransferRepository.GetById(Fixture_Storage_Transfer_UID);
                //更新审核状态
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                StorageTransfer.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                StorageTransfer.Approver_Date = DateTime.Now;
                StorageTransfer.Approver_UID = Useruid;
                fixture_Storage_TransferRepository.Update(StorageTransfer);

                #region  修改出库表
                Fixture_Storage_Outbound_M SOM = new Fixture_Storage_Outbound_M();
                List<FixturePartCreateboundStatuDTO> OutfixtureStatuDTOs = GetFixtureStatuDTO("FixturePartOutbound_Type");
                SOM.Fixture_Storage_Outbound_Type_UID = OutfixtureStatuDTOs.Where(o => o.Status == "料品移转出库单").FirstOrDefault().Status_UID;
                SOM.Fixture_Storage_Outbound_ID = StorageTransfer.Fixture_Storage_Transfer_ID;
                SOM.Applicant_UID = StorageTransfer.Applicant_UID;
                SOM.Applicant_Date = StorageTransfer.Applicant_Date;
                SOM.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                SOM.Approve_UID = Useruid;
                SOM.Approve_Date = DateTime.Now;
                SOM.Remarks = "";
                fixture_Storage_Outbound_MRepository.Add(SOM);
                unitOfWork.Commit();
                Fixture_Storage_Outbound_D SOD = new Fixture_Storage_Outbound_D();
                SOD.Fixture_Storage_Outbound_M_UID = fixture_Storage_Outbound_MRepository.GetFirstOrDefault(m => m.Fixture_Storage_Outbound_ID == StorageTransfer.Fixture_Storage_Transfer_ID).Fixture_Storage_Outbound_M_UID;
                SOD.Fixture_Part_UID = StorageTransfer.Fixture_Part_UID;
                SOD.Fixture_Warehouse_Storage_UID = StorageTransfer.Out_Fixture_Warehouse_Storage_UID;
                SOD.Outbound_Qty = StorageTransfer.Transfer_Qty;
                SOD.Remarks = "料品移转出库单" + "-" + StorageTransfer.Fixture_Part_UID;
                fixture_Storage_Outbound_DRepository.Add(SOD);

                var fixture_Part_Inventory = fixture_Part_InventoryRepository.GetFirstOrDefault(o => o.Fixture_Part_UID == StorageTransfer.Fixture_Part_UID && o.Fixture_Warehouse_Storage_UID == StorageTransfer.Out_Fixture_Warehouse_Storage_UID);
                decimal inventory_Qty = fixture_Part_Inventory.Inventory_Qty - StorageTransfer.Transfer_Qty;
                fixture_Part_Inventory.Inventory_Qty = inventory_Qty;
                fixture_Part_Inventory.Remarks = "料品移转出库单" + "-" + StorageTransfer.Fixture_Part_UID;
                fixture_Part_Inventory.Modified_UID = Useruid;
                fixture_Part_Inventory.Modified_Date = DateTime.Now;
                fixture_Part_InventoryRepository.Update(fixture_Part_Inventory);

                Fixture_Storage_InOut_Detail SI = new Fixture_Storage_InOut_Detail();
                List<FixturePartCreateboundStatuDTO> OutInfixtureStatuDTOs = GetFixtureStatuDTO("FixturePartIn_out_Type");
                SI.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "料品移转出库单").FirstOrDefault().Status_UID;
                SI.Fixture_Storage_InOut_UID = StorageTransfer.Fixture_Storage_Transfer_UID;
                SI.Fixture_Part_UID = StorageTransfer.Fixture_Part_UID;
                SI.Fixture_Warehouse_Storage_UID = StorageTransfer.Out_Fixture_Warehouse_Storage_UID;
                SI.InOut_Date = DateTime.Now;
                SI.InOut_Qty = StorageTransfer.Transfer_Qty;
                SI.Balance_Qty = inventory_Qty;
                SI.Remarks = "料品移转出库单" + "-" + StorageTransfer.Fixture_Part_UID;
                SI.Modified_UID = Useruid;
                SI.Modified_Date = DateTime.Now;
                fixture_Storage_InOut_DetailRepository.Add(SI);
                //unitOfWork.Commit();

                #endregion  修改出库表

                #region 修改入库表

                Fixture_Storage_Inbound INSTD = new Fixture_Storage_Inbound();
                List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
                INSTD.Fixture_Storage_Inbound_Type_UID = inbound_Types.Where(o => o.Status == "料品移转入库单").FirstOrDefault().Status_UID;
                INSTD.Fixture_Storage_Inbound_ID = StorageTransfer.Fixture_Storage_Transfer_ID;
                INSTD.Fixture_Part_UID = StorageTransfer.Fixture_Part_UID;
                INSTD.Fixture_Warehouse_Storage_UID = StorageTransfer.In_Fixture_Warehouse_Storage_UID;
                INSTD.Inbound_Price = 0;
                INSTD.Inbound_Qty = StorageTransfer.Transfer_Qty;
                INSTD.Issue_NO = "None";
                INSTD.Remarks = "料品移转入库单" + "-" + StorageTransfer.Fixture_Part_UID;
                INSTD.Applicant_UID = StorageTransfer.Applicant_UID;
                INSTD.Applicant_Date = StorageTransfer.Applicant_Date;
                INSTD.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                INSTD.Approve_UID = Useruid;
                INSTD.Approve_Date = DateTime.Now;
                fixture_Storage_InboundRepository.Add(INSTD);

                decimal IN_Inventory_Qty = 0;
                var inmatinven = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == StorageTransfer.Fixture_Part_UID & m.Fixture_Warehouse_Storage_UID == StorageTransfer.In_Fixture_Warehouse_Storage_UID);
                if (inmatinven != null)
                {
                    IN_Inventory_Qty = inmatinven.Inventory_Qty + StorageTransfer.Transfer_Qty;
                    inmatinven.Inventory_Qty = IN_Inventory_Qty;
                    inmatinven.Remarks = "料品移转入库单" + "-" + StorageTransfer.Fixture_Part_UID;
                    inmatinven.Modified_Date = DateTime.Now;
                    inmatinven.Modified_UID = Useruid;
                    fixture_Part_InventoryRepository.Update(inmatinven);
                }
                else
                {
                    IN_Inventory_Qty = StorageTransfer.Transfer_Qty;
                    Fixture_Part_Inventory MI = new Fixture_Part_Inventory();
                    MI.Fixture_Part_UID = StorageTransfer.Fixture_Part_UID;
                    MI.Fixture_Warehouse_Storage_UID = StorageTransfer.In_Fixture_Warehouse_Storage_UID;
                    MI.Inventory_Qty = IN_Inventory_Qty;
                    MI.Remarks = "料品移转入库单" + "-" + StorageTransfer.Fixture_Part_UID;
                    MI.Modified_UID = Useruid;
                    MI.Modified_Date = DateTime.Now;
                    fixture_Part_InventoryRepository.Add(MI);
                }
                Fixture_Storage_InOut_Detail FINSTD = new Fixture_Storage_InOut_Detail();
                var storageinbound = fixture_Storage_InboundRepository.GetFirstOrDefault(m => m.Fixture_Storage_Inbound_ID == StorageTransfer.Fixture_Storage_Transfer_ID);
                FINSTD.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "料品移转入库单").FirstOrDefault().Status_UID;
                FINSTD.Fixture_Storage_InOut_UID = StorageTransfer.Fixture_Storage_Transfer_UID;
                FINSTD.Fixture_Part_UID = StorageTransfer.Fixture_Part_UID;
                FINSTD.Fixture_Warehouse_Storage_UID = StorageTransfer.In_Fixture_Warehouse_Storage_UID;
                FINSTD.InOut_Date = DateTime.Now;
                FINSTD.InOut_Qty = StorageTransfer.Transfer_Qty;
                FINSTD.Balance_Qty = IN_Inventory_Qty;
                FINSTD.Remarks = "料品移转入库单" + "-" + StorageTransfer.Fixture_Part_UID;
                FINSTD.Modified_UID = Useruid;
                FINSTD.Modified_Date = DateTime.Now;
                fixture_Storage_InOut_DetailRepository.Add(FINSTD);
                #endregion           
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        #endregion

        #region 盘点

        public PagedListModel<FixtureStorageCheckDTO> QueryStorageChecks(FixtureStorageCheckDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixture_Storage_CheckRepository.GetInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<FixtureStorageCheckDTO>(totalcount, result);
            return bd;
        }
        public string AddOrEditStorageCheck(FixtureStorageCheckDTO dto)
        {
            try
            {
                List<FixturePartCreateboundStatuDTO> FixtureStorageChecks = GetFixtureStatuDTO("FixtureStorageCheck");
                if (dto.Fixture_Storage_Check_UID == 0)
                {
                    List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();

                    Fixture_Storage_Check SC = new Fixture_Storage_Check();
                    string preOutbound = "Check" + DateTime.Today.ToString("yyyyMMdd");
                    var test = fixture_Storage_CheckRepository.GetMany(m => m.Fixture_Storage_Check_ID.StartsWith(preOutbound)).ToList();
                    string proOutboundID = (test.Count() + 1).ToString().PadLeft(4, '0');
                    SC.Fixture_Storage_Check_ID = preOutbound + proOutboundID;
                    SC.Fixture_Part_UID = dto.Fixture_Part_UID;
                    SC.Fixture_Warehouse_Storage_UID = dto.Fixture_Warehouse_Storage_UID;
                    var matinvent = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == dto.Fixture_Part_UID
                                                    & m.Fixture_Warehouse_Storage_UID == dto.Fixture_Warehouse_Storage_UID);
                    SC.Old_Inventory_Qty = matinvent.Inventory_Qty;
                    SC.Check_Qty = dto.Check_Qty;
                    //6.1.Check_Qty > Old_Inventory_Qty，1(盤盈入庫)
                    //6.2.Check_Qty = Old_Inventory_Qty，3(平盤)
                    //6.3.Check_Qty < Old_Inventory_Qty，2(盤點出庫)
                    if (SC.Old_Inventory_Qty > SC.Check_Qty)
                    {
                        SC.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                    }
                    else if (SC.Old_Inventory_Qty < SC.Check_Qty)
                    {
                        SC.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘盈入库").FirstOrDefault().Status_UID;
                    }
                    else
                    {
                        SC.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "平盘").FirstOrDefault().Status_UID;
                    }
                    SC.Applicant_UID = dto.Applicant_UID;
                    SC.Applicant_Date = DateTime.Now;
                    SC.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
                    SC.Approve_UID = dto.Applicant_UID;
                    SC.Approve_Date = DateTime.Now;
                    fixture_Storage_CheckRepository.Add(SC);
                    unitOfWork.Commit();
                    return "";
                }
                else
                {
                    var StorageCheck = fixture_Storage_CheckRepository.GetFirstOrDefault(m => m.Fixture_Storage_Check_UID == dto.Fixture_Storage_Check_UID);
                    StorageCheck.Fixture_Part_UID = dto.Fixture_Part_UID;
                    StorageCheck.Fixture_Warehouse_Storage_UID = dto.Fixture_Warehouse_Storage_UID;
                    var matinvent = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == dto.Fixture_Part_UID
                                 & m.Fixture_Warehouse_Storage_UID == dto.Fixture_Warehouse_Storage_UID);
                    StorageCheck.Old_Inventory_Qty = matinvent.Inventory_Qty;
                    StorageCheck.Check_Qty = dto.Check_Qty;

                    //6.1.Check_Qty > Old_Inventory_Qty，1(盤盈入庫)
                    //6.2.Check_Qty = Old_Inventory_Qty，3(平盤)
                    //6.3.Check_Qty < Old_Inventory_Qty，2(盤點出庫)
                    if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                    {
                        StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                    }
                    else if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)
                    {
                        StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘盈入库").FirstOrDefault().Status_UID;
                    }
                    else
                    {
                        StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "平盘").FirstOrDefault().Status_UID;
                    }
                    StorageCheck.Approve_UID = dto.Applicant_UID;
                    StorageCheck.Approve_Date = DateTime.Now;
                    StorageCheck.Applicant_UID = dto.Applicant_UID;
                    StorageCheck.Applicant_Date = DateTime.Now;
                    fixture_Storage_CheckRepository.Update(StorageCheck);
                    unitOfWork.Commit();
                    return "0";
                }

            }
            catch (Exception e)
            {
                return "盘点单申请失败:" + e.Message;
            }
        }

        public FixtureStorageCheckDTO QueryStorageCheckSingle(int Fixture_Storage_Check_UID)
        {
            var bud = fixture_Storage_CheckRepository.GetByUid(Fixture_Storage_Check_UID);
            return bud;
        }

        public string DeleteStorageCheck(int Fixture_Storage_Check_UID, int userid)
        {
            try
            {
                var storagecheck = fixture_Storage_CheckRepository.GetById(Fixture_Storage_Check_UID);
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                storagecheck.Applicant_UID = userid;
                storagecheck.Applicant_Date = DateTime.Now;
                storagecheck.Approve_UID = userid;
                storagecheck.Approve_Date = DateTime.Now;
                storagecheck.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已删除").FirstOrDefault().Status_UID;
                fixture_Storage_CheckRepository.Update(storagecheck);
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "删除盘点单失败:" + e.Message;
            }

        }

        public List<FixtureStorageCheckDTO> DoAllExportStorageCheckReprot(FixtureStorageCheckDTO searchModel)
        {
            return fixture_Storage_CheckRepository.DoAllExportStorageCheckReprot(searchModel);
        }
        public List<FixtureStorageCheckDTO> DoExportStorageCheckReprot(string uids)
        {
            return fixture_Storage_CheckRepository.DoExportStorageCheckReprot(uids);
        }

        public List<FixtureStorageCheckDTO> DownloadStorageCheck(string Part_ID, string Part_Name, string Part_Spec, string Fixture_Warehouse_ID, string Rack_ID, string Storage_ID)
        {
            return fixture_Storage_CheckRepository.DownloadStorageCheck(Part_ID, Part_Name, Part_Spec, Fixture_Warehouse_ID, Rack_ID, Storage_ID);

        }

        //public string ImportStorageCheck(List<FixtureStorageCheckDTO> dtolist)
        //{
        //    try
        //    {
        //        if (dtolist.Count > 0)
        //        {
        //            string PreCheckID = "Check" + DateTime.Today.ToString("yyyyMMdd");
        //            var storagecheck = fixture_Storage_CheckRepository.GetMany(m => m.Fixture_Storage_Check_ID.StartsWith(PreCheckID)).ToList();

        //            int i = 1;
        //            foreach (var item in dtolist)
        //            {
        //                Fixture_Storage_Check storage_Check = new Fixture_Storage_Check();
        //                string PostCheckID = (storagecheck.Count() + i).ToString("0000");
        //                storage_Check.Fixture_Storage_Check_ID = PreCheckID + PostCheckID;
        //                storage_Check.Fixture_Part_UID = item.Fixture_Part_UID;
        //                storage_Check.Fixture_Warehouse_Storage_UID = item.Fixture_Warehouse_Storage_UID;   
        //                storage_Check.Old_Inventory_Qty = item.Old_Inventory_Qty;
        //                storage_Check.Check_Qty = item.Check_Qty;
        //                storage_Check.Applicant_UID = item.Applicant_UID;
        //                storage_Check.Applicant_Date = DateTime.Now;
        //                storage_Check.Approve_UID = item.Approve_UID;
        //                storage_Check.Approve_Date = DateTime.Now;
        //                fixture_Storage_CheckRepository.Add(storage_Check);
        //                i++;

        //            }
        //            unitOfWork.Commit();
        //            return "";

        //        }
        //        else
        //        {
        //            return "没有异常原因数据！";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //}
        public string ImportStorageCheck(List<FixtureStorageCheckDTO> dtolist)
        {

            string preOutbound = "Check" + DateTime.Today.ToString("yyyyMMdd");
            var test = fixture_Storage_CheckRepository.GetMany(m => m.Fixture_Storage_Check_ID.StartsWith(preOutbound)).ToList();
            int i = 1;
            List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            int Status_UID = fixtureStatuDTOs.Where(o => o.Status == "未审核").FirstOrDefault().Status_UID;
            List<FixturePartCreateboundStatuDTO> FixtureStorageChecks = GetFixtureStatuDTO("FixtureStorageCheck");
            foreach (var item in dtolist)
            {
                if (item.Fixture_Storage_Check_UID == 0)
                {
                    string proOutboundID = (test.Count() + i).ToString().PadLeft(4, '0');
                    item.Fixture_Storage_Check_ID = preOutbound + proOutboundID;
                    item.Status_UID = Status_UID;
                    if (item.Old_Inventory_Qty > item.Check_Qty)
                    {
                        item.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                    }
                    else if (item.Old_Inventory_Qty < item.Check_Qty)
                    {
                        item.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘盈入库").FirstOrDefault().Status_UID;
                    }
                    else
                    {
                        item.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "平盘").FirstOrDefault().Status_UID;
                    }
                    i++;
                }
            }

            return fixture_Storage_CheckRepository.InsertStorageCheck(dtolist);
        }

        public string ApproveStCheck(int Fixture_Storage_Check_UID, int Useruid)
        {
            try
            {
                List<FixturePartCreateboundStatuDTO> OutInfixtureStatuDTOs = GetFixtureStatuDTO("FixturePartIn_out_Type");
                var StorageCheck = fixture_Storage_CheckRepository.GetById(Fixture_Storage_Check_UID);
                List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                int Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                StorageCheck.Status_UID = Status_UID;
                StorageCheck.Approve_Date = DateTime.Now;
                StorageCheck.Approve_UID = Useruid;
                var matinvent = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == StorageCheck.Fixture_Part_UID
                                                & m.Fixture_Warehouse_Storage_UID == StorageCheck.Fixture_Warehouse_Storage_UID);
                StorageCheck.Old_Inventory_Qty = matinvent.Inventory_Qty;

                List<FixturePartCreateboundStatuDTO> FixtureStorageChecks = GetFixtureStatuDTO("FixtureStorageCheck");
                if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                {
                    StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                }
                else if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)
                {
                    StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘盈入库").FirstOrDefault().Status_UID;
                }
                else
                {
                    StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "平盘").FirstOrDefault().Status_UID;
                }
                fixture_Storage_CheckRepository.Update(StorageCheck);
                unitOfWork.Commit();

                if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                {
                    Fixture_Storage_Outbound_M SOM = new Fixture_Storage_Outbound_M();
                    List<FixturePartCreateboundStatuDTO> OutfixtureStatuDTOs = GetFixtureStatuDTO("FixturePartOutbound_Type");
                    SOM.Fixture_Storage_Outbound_Type_UID = OutfixtureStatuDTOs.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                    SOM.Fixture_Storage_Outbound_ID = StorageCheck.Fixture_Storage_Check_ID;
                    SOM.Applicant_UID = StorageCheck.Applicant_UID;
                    SOM.Applicant_Date = StorageCheck.Applicant_Date;
                    SOM.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                    SOM.Approve_UID = Useruid;
                    SOM.Approve_Date = DateTime.Now;
                    SOM.Remarks = "";
                    fixture_Storage_Outbound_MRepository.Add(SOM);
                    unitOfWork.Commit();

                    Fixture_Storage_Outbound_D SOD = new Fixture_Storage_Outbound_D();
                    SOD.Fixture_Storage_Outbound_M_UID = fixture_Storage_Outbound_MRepository.GetFirstOrDefault(m => m.Fixture_Storage_Outbound_ID == StorageCheck.Fixture_Storage_Check_ID).Fixture_Storage_Outbound_M_UID;
                    SOD.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                    SOD.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                    SOD.Outbound_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                    SOD.Remarks = "盘点出库单" + "-" + StorageCheck.Fixture_Part_UID;
                    fixture_Storage_Outbound_DRepository.Add(SOD);

                    var fixture_Part_Inventory = fixture_Part_InventoryRepository.GetFirstOrDefault(o => o.Fixture_Part_UID == StorageCheck.Fixture_Part_UID && o.Fixture_Warehouse_Storage_UID == StorageCheck.Fixture_Warehouse_Storage_UID);
                    decimal inventory_Qty = fixture_Part_Inventory.Inventory_Qty - (StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty);
                    fixture_Part_Inventory.Inventory_Qty = inventory_Qty;
                    fixture_Part_Inventory.Remarks = "盘点出库单" + "-" + StorageCheck.Fixture_Part_UID;
                    fixture_Part_Inventory.Modified_UID = Useruid;
                    fixture_Part_Inventory.Modified_Date = DateTime.Now;
                    fixture_Part_InventoryRepository.Update(fixture_Part_Inventory);

                    Fixture_Storage_InOut_Detail SI = new Fixture_Storage_InOut_Detail();
                    SI.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                    SI.Fixture_Storage_InOut_UID = StorageCheck.Fixture_Storage_Check_UID;
                    SI.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                    SI.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                    SI.InOut_Date = DateTime.Now;
                    SI.InOut_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                    SI.Balance_Qty = inventory_Qty;
                    SI.Remarks = "盘点出库单" + "-" + StorageCheck.Fixture_Part_UID;
                    SI.Modified_UID = Useruid;
                    SI.Modified_Date = DateTime.Now;
                    fixture_Storage_InOut_DetailRepository.Add(SI);
                }
                else
                {
                    Fixture_Storage_Inbound INSTD = new Fixture_Storage_Inbound();
                    List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
                    INSTD.Fixture_Storage_Inbound_Type_UID = inbound_Types.Where(o => o.Status == "盘点入库").FirstOrDefault().Status_UID;
                    INSTD.Fixture_Storage_Inbound_ID = StorageCheck.Fixture_Storage_Check_ID;
                    INSTD.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                    INSTD.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                    INSTD.Inbound_Price = 0;
                    INSTD.Inbound_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                    INSTD.Issue_NO = "None";
                    INSTD.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                    INSTD.Applicant_UID = StorageCheck.Applicant_UID;
                    INSTD.Applicant_Date = StorageCheck.Applicant_Date;
                    INSTD.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                    INSTD.Approve_UID = Useruid;
                    INSTD.Approve_Date = DateTime.Now;
                    fixture_Storage_InboundRepository.Add(INSTD);

                    decimal IN_Inventory_Qty = 0;
                    var inmatinven = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == StorageCheck.Fixture_Part_UID & m.Fixture_Warehouse_Storage_UID == StorageCheck.Fixture_Warehouse_Storage_UID);
                    if (inmatinven != null)
                    {
                        IN_Inventory_Qty = inmatinven.Inventory_Qty + (StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty);
                        inmatinven.Inventory_Qty = IN_Inventory_Qty;
                        inmatinven.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                        inmatinven.Modified_Date = DateTime.Now;
                        inmatinven.Modified_UID = Useruid;
                        fixture_Part_InventoryRepository.Update(inmatinven);
                    }
                    else
                    {
                        IN_Inventory_Qty = StorageCheck.Check_Qty;
                        Fixture_Part_Inventory MI = new Fixture_Part_Inventory();
                        MI.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                        MI.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                        MI.Inventory_Qty = IN_Inventory_Qty;
                        MI.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                        MI.Modified_UID = Useruid;
                        MI.Modified_Date = DateTime.Now;
                        fixture_Part_InventoryRepository.Add(MI);
                    }
                    Fixture_Storage_InOut_Detail FINSTD = new Fixture_Storage_InOut_Detail();
                    var storageinbound = fixture_Storage_InboundRepository.GetFirstOrDefault(m => m.Fixture_Storage_Inbound_ID == StorageCheck.Fixture_Storage_Check_ID);
                    if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)
                    {
                        FINSTD.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "盘点入库").FirstOrDefault().Status_UID;
                    }
                    else
                    {
                        FINSTD.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                    }
                    FINSTD.Fixture_Storage_InOut_UID = StorageCheck.Fixture_Storage_Check_UID;
                    FINSTD.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                    FINSTD.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                    FINSTD.InOut_Date = DateTime.Now;
                    FINSTD.InOut_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                    FINSTD.Balance_Qty = IN_Inventory_Qty;
                    FINSTD.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                    FINSTD.Modified_UID = Useruid;
                    FINSTD.Modified_Date = DateTime.Now;
                    fixture_Storage_InOut_DetailRepository.Add(FINSTD);

                }
                unitOfWork.Commit();
                return "";
            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }
        public string ApproveStorageCheck(FixtureStorageCheckDTO dto)
        {
            try
            {
                List<FixtureStorageCheckDTO> StorageInboundDTOs = fixture_Storage_CheckRepository.DoAllExportStorageCheckReprot(dto).Where(o => o.Status == "未审核").ToList();
                List<Fixture_Storage_Check> StorageCheckAll = fixture_Storage_CheckRepository.GetAll().ToList();
                List<FixturePartCreateboundStatuDTO> OutInfixtureStatuDTOs = GetFixtureStatuDTO("FixturePartIn_out_Type");
                List<FixturePartCreateboundStatuDTO> FixtureStorageChecks = GetFixtureStatuDTO("FixtureStorageCheck");
                List<FixturePartCreateboundStatuDTO> OutfixtureStatuDTOs = GetFixtureStatuDTO("FixturePartOutbound_Type");
                List<FixturePartCreateboundStatuDTO> inbound_Types = GetFixtureStatuDTO("FixturePartInbound_Type");
                if (StorageInboundDTOs != null && StorageInboundDTOs.Count() > 0)
                {
                    foreach (var item in StorageInboundDTOs)
                    {

                        var StorageCheck = StorageCheckAll.FirstOrDefault(o => o.Fixture_Storage_Check_UID == item.Fixture_Storage_Check_UID);
                        List<FixturePartCreateboundStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                        int Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                        StorageCheck.Status_UID = Status_UID;
                        StorageCheck.Approve_Date = DateTime.Now;
                        StorageCheck.Approve_UID = item.Approve_UID;
                        var matinvent = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == StorageCheck.Fixture_Part_UID
                                                        & m.Fixture_Warehouse_Storage_UID == StorageCheck.Fixture_Warehouse_Storage_UID);
                        StorageCheck.Old_Inventory_Qty = matinvent.Inventory_Qty;


                        if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                        {
                            StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                        }
                        else if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)
                        {
                            StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "盘盈入库").FirstOrDefault().Status_UID;
                        }
                        else
                        {
                            StorageCheck.Check_Status_UID = FixtureStorageChecks.Where(o => o.Status == "平盘").FirstOrDefault().Status_UID;
                        }
                        fixture_Storage_CheckRepository.Update(StorageCheck);
                        unitOfWork.Commit();

                        if (StorageCheck.Old_Inventory_Qty > StorageCheck.Check_Qty)
                        {
                            Fixture_Storage_Outbound_M SOM = new Fixture_Storage_Outbound_M();

                            SOM.Fixture_Storage_Outbound_Type_UID = OutfixtureStatuDTOs.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                            SOM.Fixture_Storage_Outbound_ID = StorageCheck.Fixture_Storage_Check_ID;
                            SOM.Applicant_UID = StorageCheck.Applicant_UID;
                            SOM.Applicant_Date = StorageCheck.Applicant_Date;
                            SOM.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                            SOM.Approve_UID = item.Approve_UID;
                            SOM.Approve_Date = DateTime.Now;
                            SOM.Remarks = "";
                            fixture_Storage_Outbound_MRepository.Add(SOM);
                            unitOfWork.Commit();
                            Fixture_Storage_Outbound_D SOD = new Fixture_Storage_Outbound_D();
                            SOD.Fixture_Storage_Outbound_M_UID = fixture_Storage_Outbound_MRepository.GetFirstOrDefault(m => m.Fixture_Storage_Outbound_ID == StorageCheck.Fixture_Storage_Check_ID).Fixture_Storage_Outbound_M_UID;
                            SOD.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                            SOD.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                            SOD.Outbound_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                            SOD.Remarks = "盘点出库单" + "-" + StorageCheck.Fixture_Part_UID;
                            fixture_Storage_Outbound_DRepository.Add(SOD);

                            var fixture_Part_Inventory = fixture_Part_InventoryRepository.GetFirstOrDefault(o => o.Fixture_Part_UID == StorageCheck.Fixture_Part_UID && o.Fixture_Warehouse_Storage_UID == StorageCheck.Fixture_Warehouse_Storage_UID);
                            decimal inventory_Qty = fixture_Part_Inventory.Inventory_Qty - (StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty);
                            fixture_Part_Inventory.Inventory_Qty = inventory_Qty;
                            fixture_Part_Inventory.Remarks = "盘点出库单" + "-" + StorageCheck.Fixture_Part_UID;
                            fixture_Part_Inventory.Modified_UID = item.Approve_UID;
                            fixture_Part_Inventory.Modified_Date = DateTime.Now;
                            fixture_Part_InventoryRepository.Update(fixture_Part_Inventory);
                            unitOfWork.Commit();
                            Fixture_Storage_InOut_Detail SI = new Fixture_Storage_InOut_Detail();
                            SI.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                            SI.Fixture_Storage_InOut_UID = StorageCheck.Fixture_Storage_Check_UID;
                            SI.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                            SI.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                            SI.InOut_Date = DateTime.Now;
                            SI.InOut_Qty = StorageCheck.Old_Inventory_Qty - StorageCheck.Check_Qty;
                            SI.Balance_Qty = inventory_Qty;
                            SI.Remarks = "盘点出库单" + "-" + StorageCheck.Fixture_Part_UID;
                            SI.Modified_UID = item.Approve_UID;
                            SI.Modified_Date = DateTime.Now;
                            fixture_Storage_InOut_DetailRepository.Add(SI);
                            unitOfWork.Commit();
                        }
                        else
                        {
                            Fixture_Storage_Inbound INSTD = new Fixture_Storage_Inbound();

                            INSTD.Fixture_Storage_Inbound_Type_UID = inbound_Types.Where(o => o.Status == "盘点入库").FirstOrDefault().Status_UID;
                            INSTD.Fixture_Storage_Inbound_ID = StorageCheck.Fixture_Storage_Check_ID;
                            INSTD.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                            INSTD.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                            INSTD.Inbound_Price = 0;
                            INSTD.Inbound_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                            INSTD.Issue_NO = "None";
                            INSTD.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                            INSTD.Applicant_UID = StorageCheck.Applicant_UID;
                            INSTD.Applicant_Date = StorageCheck.Applicant_Date;
                            INSTD.Status_UID = fixtureStatuDTOs.Where(o => o.Status == "已审核").FirstOrDefault().Status_UID;
                            INSTD.Approve_UID = item.Approve_UID;
                            INSTD.Approve_Date = DateTime.Now;
                            fixture_Storage_InboundRepository.Add(INSTD);
                            unitOfWork.Commit();
                            decimal IN_Inventory_Qty = 0;
                            var inmatinven = fixture_Part_InventoryRepository.GetFirstOrDefault(m => m.Fixture_Part_UID == StorageCheck.Fixture_Part_UID & m.Fixture_Warehouse_Storage_UID == StorageCheck.Fixture_Warehouse_Storage_UID);
                            if (inmatinven != null)
                            {
                                IN_Inventory_Qty = inmatinven.Inventory_Qty + (StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty);
                                inmatinven.Inventory_Qty = IN_Inventory_Qty;
                                inmatinven.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                                inmatinven.Modified_Date = DateTime.Now;
                                inmatinven.Modified_UID = item.Approve_UID;
                                fixture_Part_InventoryRepository.Update(inmatinven);
                                unitOfWork.Commit();
                            }
                            else
                            {
                                IN_Inventory_Qty = StorageCheck.Check_Qty;
                                Fixture_Part_Inventory MI = new Fixture_Part_Inventory();
                                MI.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                                MI.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                                MI.Inventory_Qty = IN_Inventory_Qty;
                                MI.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                                MI.Modified_UID = item.Approve_UID;
                                MI.Modified_Date = DateTime.Now;
                                fixture_Part_InventoryRepository.Add(MI);
                                unitOfWork.Commit();
                            }
                            Fixture_Storage_InOut_Detail FINSTD = new Fixture_Storage_InOut_Detail();
                            var storageinbound = fixture_Storage_InboundRepository.GetFirstOrDefault(m => m.Fixture_Storage_Inbound_ID == StorageCheck.Fixture_Storage_Check_ID);
                            if (StorageCheck.Old_Inventory_Qty < StorageCheck.Check_Qty)
                            {
                                FINSTD.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "盘点入库").FirstOrDefault().Status_UID;
                            }
                            else
                            {
                                FINSTD.InOut_Type_UID = OutInfixtureStatuDTOs.Where(o => o.Status == "盘点出库").FirstOrDefault().Status_UID;
                            }
                            FINSTD.Fixture_Storage_InOut_UID = StorageCheck.Fixture_Storage_Check_UID;
                            FINSTD.Fixture_Part_UID = StorageCheck.Fixture_Part_UID;
                            FINSTD.Fixture_Warehouse_Storage_UID = StorageCheck.Fixture_Warehouse_Storage_UID;
                            FINSTD.InOut_Date = DateTime.Now;
                            FINSTD.InOut_Qty = StorageCheck.Check_Qty - StorageCheck.Old_Inventory_Qty;
                            FINSTD.Balance_Qty = IN_Inventory_Qty;
                            FINSTD.Remarks = "盘点入库单" + "-" + StorageCheck.Fixture_Part_UID;
                            FINSTD.Modified_UID = item.Approve_UID;
                            FINSTD.Modified_Date = DateTime.Now;
                            fixture_Storage_InOut_DetailRepository.Add(FINSTD);
                            unitOfWork.Commit();
                        }


                    }
                    return "";

                }
                else
                {
                    return "没有未审核的数据！";
                }

            }
            catch (Exception e)
            {
                return "审核失败:" + e.Message;
            }
        }

        /// <summary>
        /// 获取库位管理的明细报表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<Fixture_Part_InventoryDTO> GetFPStoryDetialReport(Fixture_Part_InventoryDTO searchModel, Page page)
        {
            int totalcount;
            var result = fixture_Part_InventoryRepository.GetStorageDetialReportInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<Fixture_Part_InventoryDTO>(totalcount, result);
            return bd;
        }

        /// <summary>
        /// 导出ALL库存报表明细数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Fixture_Part_InventoryDTO> ExportAllFixtureInventoryDetialReport(Fixture_Part_InventoryDTO searchModel)
        {
            int totalcount = 0;
            var result = fixture_Part_InventoryRepository.ExportAllFixtureInventoryDetialReport(searchModel);
            return result;
        }

        /// <summary>
        /// 导出勾选部分库存明细数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Fixture_Part_InventoryDTO> ExportSelectedFixtureInventoryDetialReport(string uids)
        {
            var result = fixture_Part_InventoryRepository.ExportSelectedFixtureInventoryDetialReport(uids);
            return result;
        }

        /// <summary>
        /// 获取出入库报表明细数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PagedListModel<FixtureInOutStorageModel> GetInOutDetialReport(FixtureInOutStorageModel searchModel, Page page)
        {
            int totalcount;
            var result = fixture_Storage_InboundRepository.GetInOutDetialReport(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<FixtureInOutStorageModel>(totalcount, result);
            return bd;
        }

        /// <summary>
        /// 导出ALL出入库报表明细数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<FixtureInOutStorageModel> ExportAllInOutDetialReport(FixtureInOutStorageModel searchModel)
        {
            int totalcount = 0;
            var result = fixture_Storage_InboundRepository.ExportAllInOutDetialReport(searchModel);
            return result;
        }


        /// <summary>
        /// 导出勾选部分出入库报表明细数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<FixtureInOutStorageModel> ExportSelectedInOutDetialReport(string uids)
        {
            var result = fixture_Storage_InboundRepository.ExportSelectedInOutDetialReport(uids);
            return result;
        }

        #endregion

        #region 库存报表

        public PagedListModel<FixturePartStorageReportDTO> QueryStorageReports(FixturePartStorageReportDTO searchModel, Page page)
        {

            int totalcount;
            if (searchModel.intMonth.Year != 1)
            {
                searchModel.Start_Date = searchModel.intMonth.AddDays(1 - searchModel.intMonth.Day);
                searchModel.End_Date = searchModel.Start_Date.AddMonths(1).AddDays(-1);
            }
            var result = fixture_Storage_InboundRepository.GetStorageReportInfo(searchModel, page, out totalcount).ToList();
            var bd = new PagedListModel<FixturePartStorageReportDTO>(totalcount, result);
            return bd;
        }
        public List<FixturePartStorageReportDTO> DoSRExportFunction(int plant, int bg, int funplant, string Part_ID, string Part_Name, string Part_Spec, DateTime start_date, DateTime end_date)
        {
            return fixture_Storage_InboundRepository.DoSRExportFunction(plant, bg, funplant, Part_ID, Part_Name, Part_Spec, start_date, end_date);
        }
        #endregion

        public FixturePartScanCodeDTO GetFixturePartScanCodeDTO(int plantID, int optypeID, string SN, int Modified_UID)
        {
            FixturePartScanCodeDTO fixturePartScanCodeDTO = new FixturePartScanCodeDTO();
            //获取治具
            var fixtureDTO = fixtureRepository.GetFixtureDTO(plantID, optypeID, SN);
            if (fixtureDTO != null)
            {
                if (!fixtureDTO.StatuName.Contains("使用中"))
                {
                    fixturePartScanCodeDTO.Messages = "此治具未使用，此治具的状态为：" + fixtureDTO.StatuName + "，请确认信息 。";
                    fixturePartScanCodeDTO.Code = 1;
                }
                else
                {


                    var fixtureSettingM = fixturePartSettingMRepository.GetFirstOrDefault(x => x.Fixture_NO == fixtureDTO.Fixture_NO);
                    //获取配件 
                    //根据治具图号plantID，optypeID， Fixture_NO 找到 Fixture_Part_Setting_M_UID， 
                    var fixture_Part_Setting_MDTO = fixtureRepository.GetFixture_Part_Setting_MDTO(plantID, optypeID, fixtureDTO.Fixture_NO);
                    if (fixture_Part_Setting_MDTO != null)
                    {
                        //当前扫码时间
                        DateTime ScanDateTime = DateTime.Now;
                        //得到计算次数的时间跨度。
                        int UseTimesScanInterval = 0;
                        if (fixture_Part_Setting_MDTO.UseTimesScanInterval != null)
                        {
                            UseTimesScanInterval = fixture_Part_Setting_MDTO.UseTimesScanInterval.Value;
                        }
                        // 获取 Fixture_M_UseScanHistory 表最后一条数据，
                        var fixture_M_UseScanHistory = fixtureRepository.GetFixture_M_UseScanHistoryDTO(fixtureDTO.Fixture_M_UID);
                        //可用扫码次数
                        int UseTimesTotal = 0;
                        bool IsValidSan = false;
                        // 每次增加 Fixture_M_UseScanHistory 表数据，
                        string Messages = "";
                        if (fixture_M_UseScanHistory != null)
                        {
                            if (fixtureDTO.UseTimesTotal != null)
                            {
                                UseTimesTotal = fixtureDTO.UseTimesTotal.Value;
                            }
                            // UseTimesTotal = fixture_M_UseScanHistory.UseTimesTotal;
                            //测试环境为方便测试，改为分钟，测试完成后需改为小时
                            //int allSumTime = Convert.ToInt32((ScanDateTime - fixture_M_UseScanHistory.ScanDateTime).TotalHours);
                            int allSumTimeSeconds = Convert.ToInt32((ScanDateTime - fixture_M_UseScanHistory.ScanDateTime).TotalSeconds);
                            //1分钟为60秒，一小时为3600秒
                            if (allSumTimeSeconds >= UseTimesScanInterval * 3600)
                            {
                                UseTimesTotal = UseTimesTotal + 1;
                                IsValidSan = true;
                            }
                            else
                            {

                                Messages = "扫码时间间隔不符合设置时间，扫码计数次数无效";

                            }

                        }
                        else
                        {
                            UseTimesTotal = 1;
                            IsValidSan = true;

                        }
                        //这里要更新 Fixture_M 主档 和 新增 Fixture_M_UseScanHistory 数据；得到脚本。
                        string sql = GetFixture_MAndFixture_M_UseScanHistory(fixtureDTO.Fixture_M_UID, UseTimesTotal, IsValidSan, ScanDateTime);
                        // 获取 fixture_Part_UseTimes 表数据，
                        var fixture_Part_UseTimesDTOs = fixtureRepository.GetFixture_Part_UseTimesDTO(fixtureDTO.Fixture_M_UID);
                        //然后根据 Fixture_Part_Setting_M_UID 找到 Fixture_Part_Setting_D 中的数据,找到对应的配件。
                        var fixture_Part_Setting_DDTOs = fixtureRepository.GetFixture_Part_Setting_DDTOs(fixture_Part_Setting_MDTO.Fixture_Part_Setting_M_UID);

                        if (fixture_Part_Setting_DDTOs != null && fixture_Part_Setting_DDTOs.Count > 0)
                        {

                            if (fixture_Part_UseTimesDTOs != null && fixture_Part_UseTimesDTOs.Count > 0)
                            {

                                foreach (var item in fixture_Part_Setting_DDTOs)
                                {

                                    var fixture_Part_UseTimesDTO = fixture_Part_UseTimesDTOs.FirstOrDefault(o => o.Fixture_Part_Setting_D_UID == item.Fixture_Part_Setting_D_UID);
                                    if (fixture_Part_UseTimesDTO != null)
                                    {

                                        if (IsValidSan)
                                        {
                                            sql += string.Format(@" UPDATE Fixture_Part_UseTimes
                                                               SET 
                                                                  Fixture_Part_UseTimesCount = {0}
                                                                  ,Modified_UID = {1}
                                                                  ,Modified_Date = '{2}'
                                                              WHERE Fixture_Part_UseTimes_UID ={3}; ", fixture_Part_UseTimesDTO.Fixture_Part_UseTimesCount + 1, Modified_UID, DateTime.Now, fixture_Part_UseTimesDTO.Fixture_Part_UseTimes_UID);
                                        }
                                    }
                                    else
                                    {
                                        sql += string.Format(@" INSERT INTO Fixture_Part_UseTimes
                                                                   ( Fixture_M_UID
                                                                   , Fixture_Part_Setting_D_UID
                                                                   , Fixture_Part_UseTimesCount
                                                                   , Modified_UID
                                                                   , Modified_Date)
                                                                   VALUES({0},{1},{2},{3},'{4}'); ", item.Fixture_Part_UID, item.Fixture_Part_Setting_D_UID, 1, Modified_UID, DateTime.Now);
                                    }

                                }
                            }
                            else
                            {

                                foreach (var item in fixture_Part_Setting_DDTOs)
                                {
                                    sql += string.Format(@" INSERT INTO Fixture_Part_UseTimes
                                                                   (Fixture_M_UID
                                                                   , Fixture_Part_Setting_D_UID
                                                                   , Fixture_Part_UseTimesCount
                                                                   , Modified_UID
                                                                   , Modified_Date)
                                                                   VALUES({0},{1},{2},{3},'{4}'); ", item.Fixture_Part_UID, item.Fixture_Part_Setting_D_UID, 1, Modified_UID, DateTime.Now);

                                }


                            }
                        }
                        // else
                        // {
                        //fixturePartScanCodeDTO.Messages = "没有找到对应的需要管控的治具配件";
                        //fixturePartScanCodeDTO.Code = 0;
                        //}
                        //保存
                        string result = fixtureRepository.SaveFixturePartScanCodeDTO(sql);
                        if (result == "0")
                        {
                            //最后返回对象
                            // 治具主檔流水號
                            fixturePartScanCodeDTO.Fixture_M_UID = fixtureDTO.Fixture_M_UID;
                            // 治具編號(圖號)
                            fixturePartScanCodeDTO.Fixture_NO = fixtureDTO.Fixture_NO;
                            //治具唯一編號
                            fixturePartScanCodeDTO.Fixture_Unique_ID = fixtureDTO.Fixture_Unique_ID;
                            // 治具名稱
                            fixturePartScanCodeDTO.Fixture_Name = fixtureDTO.Fixture_Name;
                            // 治具短碼
                            fixturePartScanCodeDTO.ShortCode = fixtureDTO.ShortCode;
                            // 治具二維條碼
                            fixturePartScanCodeDTO.TwoD_Barcode = fixtureDTO.TwoD_Barcode;
                            // 扫码总数
                            fixturePartScanCodeDTO.UseTimesTotal = UseTimesTotal;
                            // 当前扫码时间
                            fixturePartScanCodeDTO.ScanDateTime = ScanDateTime;
                            if (fixture_M_UseScanHistory != null)
                            {
                                // 上次扫码时间  
                                fixturePartScanCodeDTO.NextScanDateTime = fixture_M_UseScanHistory.ScanDateTime;
                            }
                            //专案
                            fixturePartScanCodeDTO.ProjectName = fixtureDTO.Project;
                            //扫码时间间隔
                            if (fixtureSettingM != null)
                            {
                                fixturePartScanCodeDTO.UseTimesScanInterval = fixtureSettingM.UseTimesScanInterval;
                            }

                            //获取配件的明细
                            var fixturePartScanDTOs = fixtureRepository.GetFixturePartScanDTOs(fixtureDTO.Fixture_M_UID);
                     
                            if (fixturePartScanDTOs != null && fixturePartScanDTOs.Count > 0)
                            {
                                foreach (var item in fixturePartScanDTOs)
                                {

                                    if (item.UseTimes >= item.Fixture_Part_Life_UseTimes)
                                    {
                                        item.IsReplace = true;
                                        Messages += "<div> " + item.Part_ID + "-" + item.Part_Name + "的使用次数为" + item.UseTimes + "，已经超过使用寿命次数" + item.Fixture_Part_Life_UseTimes + "，请更换配件。</div>";
                                    }
                                    else
                                    {
                                        item.IsReplace = false;
                                    }
                                }
                            }
                            //获取配件的明细
                            fixturePartScanCodeDTO.FixturePartScanDTOs = fixturePartScanDTOs;

                            fixturePartScanCodeDTO.Code = 1;
                            fixturePartScanCodeDTO.Messages = Messages;
                        }
                        else
                        {
                            fixturePartScanCodeDTO.Messages = result;
                            fixturePartScanCodeDTO.Code = 0;
                        }
                    }
                    else
                    {
                        fixturePartScanCodeDTO.Messages = "没有找到对应的治具主档设定";
                        fixturePartScanCodeDTO.Code = 0;
                    }
                }
            }
            else
            {
                fixturePartScanCodeDTO.Messages = "没有找到对应的治具";
                fixturePartScanCodeDTO.Code = 0;
            }
            return fixturePartScanCodeDTO;
        }


        public string GetFixture_MAndFixture_M_UseScanHistory(int Fixture_M_UID, int UseTimesTotal, bool IsValidSan, DateTime ScanDateTime)
        {
            string sql = "";
            sql += string.Format(@" UPDATE Fixture_M  SET  UseTimesTotal={0}  WHERE Fixture_M_UID ={1} ;", UseTimesTotal, Fixture_M_UID);
            sql += string.Format(@" INSERT INTO Fixture_M_UseScanHistory (Fixture_M_UID,UseTimesTotal,IsValidSan,ScanDateTime) VALUES ({0},{1},{2},'{3}'); ", Fixture_M_UID, UseTimesTotal, IsValidSan ? 1 : 0, ScanDateTime);
            return sql;

        }

        #region 治具扫码清零 Jay
        public FixtureDTO GetFixtureScanCodeDTOBySN(int plantID, int optypeID, string SN)
        {
            //获取治具
            var fixtureDTO = fixtureRepository.GetFixtureDTO(plantID, optypeID, SN);
            return fixtureDTO;
        }
        #endregion
    }
}
