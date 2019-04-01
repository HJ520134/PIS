using PDMS.Common.Constants;
using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service.ProductionPlanning
{
    //public interface IProductionSchedulService
    //{
    //    PagedListModel<QueryProductionSchedulMPVM> QueryProductionSchedulMP(FlowchartMeNPI npiModel, Page page);

    //    List<ProductionSchedulMPVM>  DownloadMPExcel(int id, int Version);

    //    string CheckDownloadMPExcel(int id, int Version);

    //    string DeleteInfoByUIDListAPI(List<int> idList);

    //    string ImportMPExcelAPI(List<ProductionSchedulDTO>  dtoList);

    //    string CheckImportMPExcel(List<ProductionSchedulDTO> dtoList);

    //    string SaveMPInfo(ProductionSchedulDTO dto);
    //}


    //public class ProductionSchedulService : IProductionSchedulService
    //{
    //    #region Private interfaces properties
    //    private readonly IProductionSchedulRepository ProductionSchedulRepository;
    //    private readonly IFlowChartMasterRepository FlowChartMasterRepository;
    //    private readonly IUnitOfWork unitOfWork;
    //    #endregion

    //    #region Service constructor
    //    public ProductionSchedulService(
    //        IProductionSchedulRepository ProductionSchedulRepository,
    //        IFlowChartMasterRepository FlowChartMasterRepository,
    //    IUnitOfWork unitOfWork)
    //    {
    //        this.ProductionSchedulRepository = ProductionSchedulRepository;
    //        this.FlowChartMasterRepository = FlowChartMasterRepository;
    //        this.unitOfWork = unitOfWork;
    //    }
    //    #endregion

    //    public PagedListModel<QueryProductionSchedulMPVM> QueryProductionSchedulMP(FlowchartMeNPI npiModel, Page page)
    //    {
    //        DateTime? dateStart = null;
    //        DateTime? dateEnd = null;
    //        if (!string.IsNullOrEmpty(npiModel.Modified_Date_From))
    //        {
    //            dateStart = Convert.ToDateTime(npiModel.Modified_Date_From);
    //        }
    //        if (!string.IsNullOrEmpty(npiModel.Modified_Date_End))
    //        {
    //            dateEnd = Convert.ToDateTime(npiModel.Modified_Date_End);
    //        }
    //        var totalCount = 0;
    //        var list = ProductionSchedulRepository.QueryProductionSchedulMP(npiModel.id, npiModel.Version, dateStart, dateEnd, page, out totalCount);
    //        var flCharts = new PagedListModel<QueryProductionSchedulMPVM>(totalCount, list);
    //        return flCharts;
    //    }


    //    public List<ProductionSchedulMPVM> DownloadMPExcel(int id, int Version)
    //    {
    //        var result = ProductionSchedulRepository.DownloadMPExcel(id, Version);
    //        return result;
    //    }

    //    public string CheckDownloadMPExcel(int id, int Version)
    //    {
    //        var list = ProductionSchedulRepository.CheckDownloadMPExcel(id, Version);
    //        if (list.Count() == 0)
    //        {
    //            return "还没有设定PP制程，请先设定";
    //        }
    //        else
    //        {
    //            return string.Empty;
    //        }
    //    }

    //    public string ImportMPExcelAPI(List<ProductionSchedulDTO>  dtoList)
    //    {
    //        List<Production_Schedul> list = new List<Production_Schedul>();
    //        //删掉DB中从今天起已经存在的数据
    //        var now = DateTime.Now;
    //        //var uidAndPlanTypeList = dtoList.GroupBy(m => new { m.FlowChart_Detail_UID, m.PlanType }).ToList();
    //        //foreach (var uidAndPlanTypeItem in uidAndPlanTypeList)
    //        //{
    //        //    var deleteList = ProductionSchedulRepository.GetMany(m => m.FlowChart_Detail_UID == uidAndPlanTypeItem.Key.FlowChart_Detail_UID 
    //        //    && m.PlanType == uidAndPlanTypeItem.Key.PlanType &&  m.Product_Date >= now.Date).ToList();

    //        //    list.AddRange(deleteList);
    //        //}
    //        //ProductionSchedulRepository.DeleteList(list);

    //        //插入数据
    //        var entityList = AutoMapper.Mapper.Map<List<Production_Schedul>>(dtoList);
    //        ProductionSchedulRepository.AddList(entityList);
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    public string CheckImportMPExcel(List<ProductionSchedulDTO> dtoList)
    //    {
    //        var idList = dtoList.Select(m => m.FlowChart_Master_UID).ToList();
    //        var versionList = dtoList.Select(m => m.FlowChart_Version).ToList();
    //        var plantTypeList = dtoList.Select(m => m.PlanType).ToList();
    //        var dateList = dtoList.Select(m => m.Product_Date).ToList();

    //        var hasItem = ProductionSchedulRepository.GetMany(m => idList.Contains(m.FlowChart_Master_UID) && versionList.Contains(m.FlowChart_Version)
    //        && plantTypeList.Contains(m.PlanType) && dateList.Contains(m.Product_Date)).FirstOrDefault();
    //        if (hasItem != null)
    //        {
    //            return string.Format("已经存在的日期:{0}", hasItem.Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate));
    //        }
    //        return string.Empty;
    //    }

    //    public string DeleteInfoByUIDListAPI(List<int> idList)
    //    {
    //        var list = ProductionSchedulRepository.GetMany(m => idList.Contains(m.Production_Schedul_UID)).ToList();
    //        ProductionSchedulRepository.DeleteList(list);
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    public string SaveMPInfo(ProductionSchedulDTO dto)
    //    {
    //        var item = ProductionSchedulRepository.GetById(dto.Production_Schedul_UID);
    //        item.Input_Qty = dto.Input_Qty;
    //        item.Target_Yield = dto.Target_Yield/100;
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //}


}
