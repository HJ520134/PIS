using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    //public interface IDemissionRateAndWorkScheduleRepository : IRepository<DemissionRateAndWorkSchedule>
    //{
    //    List<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page, out int totalCount);
    //}

    //public class DemissionRateAndWorkScheduleRepository : RepositoryBase<DemissionRateAndWorkSchedule>, IDemissionRateAndWorkScheduleRepository
    //{
    //    public DemissionRateAndWorkScheduleRepository(IDatabaseFactory databaseFactory)
    //        : base(databaseFactory)
    //    {

    //    }

    //    public List<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page, out int totalCount)
    //    {
    //        string startDate = string.Empty;
    //        string endDate = string.Empty;

    //        var sql = @"SELECT DemissionRateAndWorkSchedule_UID,Plant_Organization_UID,B.Organization_Name, BG_Organization_UID,C.Organization_Name AS OPType,
    //            Product_Date,DemissionRate_NPI * 100 AS DemissionRate_NPI,DemissionRate_MP * 100 AS DemissionRate_MP,
    //            NPI_RecruitStaff_Qty,MP_RecruitStaff_Qty,WorkSchedule FROM dbo.DemissionRateAndWorkSchedule A
    //            JOIN dbo.System_Organization B ON A.Plant_Organization_UID = B.Organization_UID
    //            JOIN dbo.System_Organization C ON A.BG_Organization_UID = C.Organization_UID
    //            WHERE 1 = 1 ";

    //        if (searchModel.Plant_Organization_UID != 0)
    //        {
    //            sql = sql + string.Format("AND Plant_Organization_UID = {0} ", searchModel.Plant_Organization_UID);
    //        }

    //        if (searchModel.BG_Organization_UID != 0)
    //        {
    //            sql = sql + string.Format("AND BG_Organization_UID = {0}", searchModel.BG_Organization_UID);
    //        }

    //        if (searchModel.StartDate != null)
    //        {
    //            startDate = searchModel.StartDate.Value.ToString(FormatConstants.DateTimeFormatStringByDate);
    //            sql = sql + "AND CONVERT(VARCHAR(10),Product_Date,120) >= '" + startDate + "' ";
    //        }
    //        if (searchModel.EndDate != null)
    //        {
    //            endDate = searchModel.EndDate.Value.ToString(FormatConstants.DateTimeFormatStringByDate);
    //            sql = sql + "AND CONVERT(VARCHAR(10),Product_Date,120) <= '" + endDate + "' ";
    //        }




    //        totalCount = DataContext.Database.SqlQuery<DemissionRateAndWorkScheduleDTO>(sql).ToList().Count();
    //        var list = DataContext.Database.SqlQuery<DemissionRateAndWorkScheduleDTO>(sql).Skip(page.PageSize * page.PageNumber).Take(page.PageSize).ToList();
    //        return list;
    //    }

    //}
}
