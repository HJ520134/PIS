using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    //public interface IProductionSchedulRepository : IRepository<Production_Schedul>
    //{
    //    List<QueryProductionSchedulMPVM> QueryProductionSchedulMP(int id, int Version, DateTime? dateStart, DateTime? dateEnd, Page page, out int totalCount);

    //    List<ProductionSchedulMPVM> DownloadMPExcel(int id, int Version);

    //    List<FlowChart_Detail> CheckDownloadMPExcel(int id, int Version);
    //}

    //public class ProductionSchedulRepository : RepositoryBase<Production_Schedul>, IProductionSchedulRepository
    //{
    //    public ProductionSchedulRepository(IDatabaseFactory databaseFactory)
    //        : base(databaseFactory)
    //    {

    //    }

    //    public List<QueryProductionSchedulMPVM> QueryProductionSchedulMP(int id, int Version, DateTime? dateStart, DateTime? dateEnd, Page page, out int totalCount)
    //    {
    //        string sql = @"SELECT A.*,C.Project_Name,B.Product_Phase, CONVERT(VARCHAR(50),A.Target_Yield * 100 ) + '%' AS Per_Target_Yield,
    //                        CASE A.PlanType WHEN 1 THEN N'Lock计划' WHEN 2 THEN N'四周生产计划' END AS PlanTypeValue
    //                        FROM dbo.Production_Schedul A
    //                        JOIN dbo.FlowChart_Master B
    //                        ON A.FlowChart_Master_UID = B.FlowChart_Master_UID 
    //                        JOIN dbo.System_Project C
    //                        ON C.Project_UID = B.Project_UID
    //                        WHERE A.FlowChart_Master_UID={0} AND A.FlowChart_Version={1} ";

    //        if (dateStart != null)
    //        {
    //            sql = sql + " AND A.Product_Date >= '" + dateStart + "'";
    //        }

    //        if (dateEnd != null)
    //        {
    //            sql = sql + " AND A.Product_Date <= '" + dateEnd + "'";
    //        }
    //        sql = string.Format(sql, id, Version);
    //        totalCount = DataContext.Database.SqlQuery<QueryProductionSchedulMPVM>(sql).ToList().Count();
    //        var list = DataContext.Database.SqlQuery<QueryProductionSchedulMPVM>(sql).Skip(page.PageSize * page.PageNumber).Take(page.PageSize).ToList();
    //        return list;
    //    }

    //    public List<ProductionSchedulMPVM> DownloadMPExcel(int id, int Version)
    //    {
    //        string sql = @"SELECT A.Production_Schedul_UID,B.FlowChart_Master_UID,C.Project_Name,B.Product_Phase,A.Product_Date,A.Input_Qty 
    //                        FROM dbo.Production_Schedul A
    //                        JOIN dbo.FlowChart_Master B
    //                        ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
    //                        JOIN dbo.System_Project C
    //                        ON C.Project_UID = B.Project_UID
    //                        WHERE A.FlowChart_Master_UID = {0}";
    //        sql = string.Format(sql, id);
    //        var list = DataContext.Database.SqlQuery<ProductionSchedulMPVM>(sql).ToList();
    //        return list;
    //    }

    //    public List<FlowChart_Detail> CheckDownloadMPExcel(int id, int Version)
    //    {
    //        var list = DataContext.FlowChart_Detail.Where(m => m.FlowChart_Master_UID == id && m.FlowChart_Version == Version).ToList();
    //        return list;
    //    }
    //}
}
