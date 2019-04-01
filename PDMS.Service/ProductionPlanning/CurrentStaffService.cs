using PDMS.Common.Constants;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service.ProductionPlanning
{
    //public interface ICurrentStaffService
    //{
    //    PagedListModel<CurrentStaffDTO> QueryCurrentStaffInfo(CurrentStaffDTO dto, Page page);
    //    string ImportCurrentStaffInfo(List<CurrentStaffDTO> list);
    //    string CheckImportCurrentStaffExcel(List<CurrentStaffDTO> list);
    //    string SaveStaffInfo(CurrentStaffDTO dto);

    //    PagedListModel<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page);
    //    string SaveDemissionInfo(DemissionRateAndWorkScheduleDTO dto);
    //    string CheckImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list);
    //    string ImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list);

    //    #region 实际人力数录入
    //    PagedListModel<ActiveManPowerVM> ActualPowerInfo(ActiveManPowerSearchVM vm, Page page);
    //    string CheckDownloadManPowerExcel(int id, int Version);
    //    List<ActiveManPowerVM> GetManPowerDownLoadInfo(int id, int Version);
    //    string CheckImportManPower(List<ActiveManPowerVM> list);
    //    string ImportManPower(List<ActiveManPowerVM> list);
    //    string SaveActualPowerInfo(ProductRequestStaffDTO dto);

    //    #endregion

    //    #region 实际机台数录入
    //    PagedListModel<ProductEquipmentQTYDTO> EquipInfo(ActiveManPowerSearchVM vm, Page page);
    //    List<ActiveEquipVM> GetEquipDownLoadInfo(int id, int Version);
    //    string CheckImportEquip(List<ProductEquipmentQTYDTO> list);
    //    string ImportEquip(List<ProductEquipmentQTYDTO> list);
    //    string SaveActualEquipInfo(ProductEquipmentQTYDTO dto);
    //    #endregion
    //}

    //public class CurrentStaffService : ICurrentStaffService
    //{
    //    #region Private interfaces properties

    //    private readonly IUnitOfWork unitOfWork;

    //    //private readonly ICurrentStaffRepository currentStaffRepository;
    //    //private readonly IDemissionRateAndWorkScheduleRepository demissionRateAndWorkScheduleRepository;
    //    //private readonly IProductRequestStaffRepository productRequestStaffRepository;
    //    private readonly IProductEquipmentQTYRepository productEquipmentQTYRepository;

    //    public CurrentStaffService(
    //        //ICurrentStaffRepository currentStaffRepository,
    //        //IDemissionRateAndWorkScheduleRepository demissionRateAndWorkScheduleRepository,
    //        //IProductRequestStaffRepository productRequestStaffRepository,
    //        IProductEquipmentQTYRepository productEquipmentQTYRepository,
    //    IUnitOfWork unitOfWork)
    //    {
    //        //this.currentStaffRepository = currentStaffRepository;
    //        //this.demissionRateAndWorkScheduleRepository = demissionRateAndWorkScheduleRepository;
    //        //this.productRequestStaffRepository = productRequestStaffRepository;
    //        this.productEquipmentQTYRepository = productEquipmentQTYRepository;
    //        this.unitOfWork = unitOfWork;
    //    }

    //    #endregion //Private interfaces properties

    //    #region 现有人力
    //    public PagedListModel<CurrentStaffDTO> QueryCurrentStaffInfo(CurrentStaffDTO dto, Page page)
    //    {
    //        var totalCount = 0;
    //        var list = currentStaffRepository.QueryCurrentStaffInfo(dto, page, out totalCount);
    //        return new PagedListModel<CurrentStaffDTO>(totalCount, list);
    //    }

    //    public string ImportCurrentStaffInfo(List<CurrentStaffDTO> list)
    //    {
    //        List<Current_Staff> dtoList = AutoMapper.Mapper.Map<List<Current_Staff>>(list);
    //        var Plant_Organization_UID = list.First().Plant_Organization_UID;
    //        var BG_Organization_UID = list.First().BG_Organization_UID;

    //        //var oldList = currentStaffRepository.GetMany(m => m.Plant_Organization_UID == Plant_Organization_UID
    //        //&& m.BG_Organization_UID == BG_Organization_UID).ToList();
    //        //currentStaffRepository.DeleteList(oldList);

    //        currentStaffRepository.AddList(dtoList);
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }


    //    public string CheckImportCurrentStaffExcel(List<CurrentStaffDTO> list)
    //    {
    //        var plantUIDList = list.Select(m => m.Plant_Organization_UID).ToList();
    //        var bgUIDList = list.Select(m => m.BG_Organization_UID).ToList();
    //        var dateList = list.Select(m => m.ProductDate).ToList();
    //        int LanguageID = list.Select(m => m.LanguageID).First();

    //        var hasItem = currentStaffRepository.GetMany(m => plantUIDList.Contains(m.Plant_Organization_UID) && bgUIDList.Contains(m.BG_Organization_UID)
    //        && dateList.Contains(m.ProductDate)).FirstOrDefault();

    //        if (hasItem != null)
    //        {

    //            //var error = LocalizedLanguageCommon.GetLocaleStringResource(LanguageID, "Common.DataExist");
    //            var errorInfo = string.Format("已经存在相同的日期：{0}", hasItem.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate));
    //            return errorInfo;
    //        }
    //        return string.Empty;
    //    }

    //    public string SaveStaffInfo(CurrentStaffDTO dto)
    //    {
    //        var item = currentStaffRepository.GetById(dto.Current_Staff_UID);
    //        item.OP_Qty = dto.OP_Qty;
    //        item.Monitor_Staff_Qty = dto.Monitor_Staff_Qty;
    //        item.Technical_Staff_Qty = dto.Technical_Staff_Qty;
    //        item.Material_Keeper_Qty = dto.Material_Keeper_Qty;
    //        item.Others_Qty = dto.Others_Qty;
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    #endregion


    //    #region 离职率排班维护
    //    public PagedListModel<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page)
    //    {
    //        var totalCount = 0;
    //        var list = demissionRateAndWorkScheduleRepository.QueryTurnoverSchedulingInfo(searchModel, page, out totalCount);
    //        return new PagedListModel<DemissionRateAndWorkScheduleDTO>(totalCount, list);
    //    }

    //    public string SaveDemissionInfo(DemissionRateAndWorkScheduleDTO dto)
    //    {
    //        var item = demissionRateAndWorkScheduleRepository.GetById(dto.DemissionRateAndWorkSchedule_UID);
    //        item.Product_Date = dto.Product_Date;
    //        item.DemissionRate_MP = dto.DemissionRate_MP / 100;
    //        item.DemissionRate_NPI = dto.DemissionRate_NPI / 100;
    //        item.MP_RecruitStaff_Qty = dto.MP_RecruitStaff_Qty;
    //        item.NPI_RecruitStaff_Qty = dto.NPI_RecruitStaff_Qty;
    //        item.WorkSchedule = dto.WorkSchedule;

    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    public string CheckImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list)
    //    {
    //        var dateList = list.Select(m => m.Product_Date).ToList();
    //        var hasItem = demissionRateAndWorkScheduleRepository.GetMany(m => dateList.Contains(m.Product_Date)).FirstOrDefault();
    //        if (hasItem != null)
    //        {
    //            return string.Format("已经存在日期：{0}的数据了", hasItem.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
    //        }
    //        return string.Empty;
    //    }

    //    public string ImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list)
    //    {
    //        List<DemissionRateAndWorkSchedule> dtoList = AutoMapper.Mapper.Map<List<DemissionRateAndWorkSchedule>>(list);
    //        demissionRateAndWorkScheduleRepository.AddList(dtoList);
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }
    //    #endregion

    //    #region 实际人力数据录入
    //    public PagedListModel<ActiveManPowerVM> ActualPowerInfo(ActiveManPowerSearchVM vm, Page page)
    //    {
    //        var totalCount = 0;
    //        var list = productRequestStaffRepository.ActualPowerInfo(vm, page, out totalCount);
    //        return new PagedListModel<ActiveManPowerVM>(totalCount, list);
    //    }

    //    public string CheckDownloadManPowerExcel(int id, int Version)
    //    {
    //        var list = productRequestStaffRepository.GetManPowerDownLoadInfo(id, Version);
    //        if (list.Count() == 0)
    //        {
    //            return "请先设定关键制程后再设置实际人力";
    //        }
    //        return string.Empty;
    //    }

    //    public List<ActiveManPowerVM> GetManPowerDownLoadInfo(int id, int Version)
    //    {
    //        var list = productRequestStaffRepository.GetManPowerDownLoadInfo(id, Version);
    //        return list;
    //    }

    //    public string CheckImportManPower(List<ActiveManPowerVM> list)
    //    {
    //        var flDetailIDList = list.Select(m => m.FlowChart_Detail_UID).ToList();
    //        var dateList = list.Select(m => m.ProductDate).ToList();

    //        var hasItem = productRequestStaffRepository.GetMany(m => flDetailIDList.Contains(m.FlowChart_Detail_UID) && dateList.Contains(m.ProductDate)).FirstOrDefault();

    //        if (hasItem != null)
    //        {
    //           return string.Format("已经存在相同的日期：{0}", hasItem.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate));
    //        }
    //        return string.Empty;

    //    }

    //    public string ImportManPower(List<ActiveManPowerVM> list)
    //    {
    //        List<Product_RequestStaff> dtoList = AutoMapper.Mapper.Map<List<Product_RequestStaff>>(list);
    //        productRequestStaffRepository.AddList(dtoList);
    //        unitOfWork.Commit();
    //        return string.Empty;

    //    }


    //    public string SaveActualPowerInfo(ProductRequestStaffDTO dto)
    //    {
    //        var item = productRequestStaffRepository.GetById(dto.Product_RequestStaff_UID.Value);
    //        item.OP_Qty = dto.OP_Qty.Value;
    //        item.Monitor_Staff_Qty = dto.Monitor_Staff_Qty.Value;
    //        item.Technical_Staff_Qty = dto.Technical_Staff_Qty.Value;
    //        item.Material_Keeper_Qty = dto.Material_Keeper_Qty.Value;
    //        item.Others_Qty = dto.Others_Qty.Value;
    //        item.Modified_UID = dto.Modified_UID;
    //        item.Modified_Date = DateTime.Now;
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    #endregion


    //    #region 实际机台数录入
    //    public PagedListModel<ProductEquipmentQTYDTO> EquipInfo(ActiveManPowerSearchVM vm, Page page)
    //    {
    //        var totalCount = 0;
    //        var list = productRequestStaffRepository.EquipInfo(vm, page, out totalCount);
    //        return new PagedListModel<ProductEquipmentQTYDTO>(totalCount, list);
    //    }


    //    public List<ActiveEquipVM> GetEquipDownLoadInfo(int id, int Version)
    //    {
    //        var list = productRequestStaffRepository.GetEquipDownLoadInfo(id, Version);
    //        return list;
    //    }

    //    public string CheckImportEquip(List<ProductEquipmentQTYDTO> list)
    //    {
    //        var flDUID = list.Select(m => m.FlowChart_Detail_UID).ToList();
    //        var flMeUID = list.Select(m => m.Flowchart_Detail_ME_UID).ToList();
    //        var dateList = list.Select(m => m.ProductDate).ToList();

    //        var hasItem = productEquipmentQTYRepository.GetMany(m => flDUID.Contains(m.FlowChart_Detail_UID) && flMeUID.Contains(m.Flowchart_Detail_ME_UID) 
    //        && dateList.Contains(m.ProductDate)).FirstOrDefault();

    //        if (hasItem != null)
    //        {
    //            return string.Format("已经存在相同的日期：{0}", hasItem.ProductDate.ToString(FormatConstants.DateTimeFormatStringByDate));
    //        }
    //        return string.Empty;
    //    }

    //    public string ImportEquip(List<ProductEquipmentQTYDTO> list)
    //    {
    //        List<Product_Equipment_QTY> dtoList = AutoMapper.Mapper.Map<List<Product_Equipment_QTY>>(list);
    //        productEquipmentQTYRepository.AddList(dtoList);
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    public string SaveActualEquipInfo(ProductEquipmentQTYDTO dto)
    //    {
    //        var item = productEquipmentQTYRepository.GetById(dto.Product_Equipment_QTY_UID.Value);
    //        item.Qty = dto.Qty.Value;
    //        item.Modified_UID = dto.Modified_UID;
    //        item.Modified_Date = DateTime.Now;
    //        unitOfWork.Commit();
    //        return string.Empty;

    //    }
    //    #endregion
    //}
}
