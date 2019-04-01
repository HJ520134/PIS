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
    //public interface IProductionSchedulNPI
    //{
    //    PagedListModel<ProductionSchedulNPIVM> ExportProductionSchedulNPI(FlowchartMeNPI npiModel, Page page);
    //    string ImportNPIExcel(List<ProductionSchedulNPIDTO> NPIDTOList);
    //    string SaveNPIInfo(ProductionSchedulNPIDTO dto, int AccountID);
    //    string CheckDownloadNPIExcel(List<ProductionSchedulNPIDTO> list);
    //    string DeleteInfoByUIDList(List<int> idList);

    //}

    //public class ProductionSchedulNPIService : IProductionSchedulNPI
    //{
    //    #region Private interfaces properties

    //    private readonly IProductionSchedulNPIRepository ProductionSchedulNPIRepository;
    //    private readonly IUnitOfWork unitOfWork;

    //    #endregion //Private interfaces properties

    //    #region Service constructor
    //    public ProductionSchedulNPIService(
    //        IProductionSchedulNPIRepository ProductionSchedulNPIRepository,
    //        IUnitOfWork unitOfWork)
    //    {
    //        this.ProductionSchedulNPIRepository = ProductionSchedulNPIRepository;
    //        this.unitOfWork = unitOfWork;
    //    }
    //    #endregion

    //    public PagedListModel<ProductionSchedulNPIVM> ExportProductionSchedulNPI(FlowchartMeNPI npiModel, Page page)
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
    //        var list = ProductionSchedulNPIRepository.ExportProductionSchedulNPI(npiModel.id, npiModel.Version, dateStart, dateEnd, page, out totalCount);
            
    //        var flCharts = new PagedListModel<ProductionSchedulNPIVM>(totalCount, list);
    //        return flCharts;
    //    }

    //    public string ImportNPIExcel(List<ProductionSchedulNPIDTO> NPIDTOList)
    //    {
    //        //先删除数据
    //        //var Flowchart_Detail_ME_UID = NPIDTOList.First().Flowchart_Detail_ME_UID;
    //        //var oldList = ProductionSchedulNPIRepository.GetMany(m => m.Flowchart_Detail_ME_UID == Flowchart_Detail_ME_UID).ToList();
    //        //ProductionSchedulNPIRepository.DeleteList(oldList);
    //        //再插入新数据
    //        List<Production_Schedul_NPI> npiList = new List<Production_Schedul_NPI>();
    //        npiList = AutoMapper.Mapper.Map<List<Production_Schedul_NPI>>(NPIDTOList);
    //        ProductionSchedulNPIRepository.AddList(npiList);
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    public string CheckDownloadNPIExcel(List<ProductionSchedulNPIDTO> list)
    //    {
    //        var idList = list.Select(m => m.FlowChart_Master_UID).ToList();
    //        var versionList = list.Select(m => m.FlowChart_Version).ToList();
    //        var dateList = list.Select(m => m.Product_Date).ToList();
    //        var hasItem = ProductionSchedulNPIRepository.GetMany(m => idList.Contains(m.FlowChart_Master_UID) && versionList.Contains(m.FlowChart_Version) &&  
    //            dateList.Contains(m.Product_Date)).FirstOrDefault();
    //        if (hasItem != null)
    //        {
    //            return string.Format("日期：{0}已经存在", hasItem.Product_Date.Value.ToString(FormatConstants.DateTimeFormatStringByDate));
    //        }
    //        else
    //        {
    //            return string.Empty;
    //        }
    //    }

    //    public string DeleteInfoByUIDList(List<int> idList)
    //    {
    //        var list = ProductionSchedulNPIRepository.GetMany(m => idList.Contains(m.Production_Schedul_NPI_UID)).ToList();
    //        ProductionSchedulNPIRepository.DeleteList(list);
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }

    //    public string SaveNPIInfo(ProductionSchedulNPIDTO dto, int AccountID)
    //    {
    //        var item = ProductionSchedulNPIRepository.GetById(dto.Production_Schedul_NPI_UID);
    //        item.Input = dto.Input;
    //        unitOfWork.Commit();
    //        return string.Empty;
    //    }
    //}
}
