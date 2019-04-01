using PDMS.Common.Helpers;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    //public interface IProductionSchedulNPIRepository : IRepository<Production_Schedul_NPI>
    //{
    //    List<ProductionSchedulNPIVM> ExportProductionSchedulNPI(int id, int Version, DateTime? dateStart, DateTime? dateEnd, Page page, out int totalCount);

    //}

    //public class ProductionSchedulNPIRepository : RepositoryBase<Production_Schedul_NPI>, IProductionSchedulNPIRepository
    //{
    //    private Logger log = new Logger("ProductionSchedulNPIRepository");
    //    public ProductionSchedulNPIRepository(IDatabaseFactory databaseFactory)
    //        : base(databaseFactory)
    //    {

    //    }

    //    public List<ProductionSchedulNPIVM> ExportProductionSchedulNPI(int id, int Version, DateTime? dateStart, DateTime? dateEnd, Page page, out int totalCount)
    //    {
    //        var sql = @"SELECT A.Production_Schedul_NPI_UID, B.FlowChart_Master_UID, B.FlowChart_Version, C.Project_Name,B.Product_Phase,A.Product_Date,A.Input 
    //                    FROM dbo.Production_Schedul_NPI A
    //                    JOIN dbo.FlowChart_Master B
    //                    ON A.FlowChart_Master_UID = B.FlowChart_Master_UID
    //                    JOIN dbo.System_Project C
    //                    ON C.Project_UID = B.Project_UID
    //                    WHERE A.FlowChart_Master_UID = {0} AND A.FlowChart_Version={1} ";

    //        if (dateStart != null)
    //        {
    //            sql = sql + " AND A.Product_Date >= '" + dateStart + "'";
    //        }

    //        if (dateEnd != null)
    //        {
    //            sql = sql + " AND A.Product_Date <= '" + dateEnd + "'";
    //        }

    //        sql = string.Format(sql, id, Version);
    //        totalCount = DataContext.Database.SqlQuery<ProductionSchedulNPIVM>(sql).ToList().Count();
    //        var list = DataContext.Database.SqlQuery<ProductionSchedulNPIVM>(sql).Skip(page.PageSize * page.PageNumber).Take(page.PageSize).ToList();
    //        return list;
    //    }

    //}
}
