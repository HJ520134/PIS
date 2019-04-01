using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_RepairSolutionRepository : IRepository<Fixture_RepairSolution>
    {
        IQueryable<Fixture_RepairSolution> QueryFixture_RepairSolutions(Fixture_RepairSolutionModelSearch search, Page page, out int count);
    }
    public class Fixture_RepairSolutionRepository : RepositoryBase<Fixture_RepairSolution>, IFixture_RepairSolutionRepository
    {
        public Fixture_RepairSolutionRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public IQueryable<Fixture_RepairSolution> QueryFixture_RepairSolutions(Fixture_RepairSolutionModelSearch search, Page page, out int count)
        {
            var query = from r in DataContext.Fixture_RepairSolution select r;
            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.Plant_Organization_UID > 0)
                    {
                        query = query.Where(r => r.Plant_Organization_UID == search.Plant_Organization_UID);
                    }
                    if (search.BG_Organization_UID > 0)
                    {
                        query = query.Where(r => r.BG_Organization_UID == search.BG_Organization_UID);
                    }
                    if (search.FunPlant_Organization_UID.HasValue)
                    {
                        query = query.Where(r => r.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(search.RepairSolution_ID))
                    {
                        query = query.Where(r => r.RepairSolution_ID.Contains(search.RepairSolution_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.RepairSolution_Name))
                    {
                        query = query.Where(r => r.RepairSolution_Name.Contains(search.RepairSolution_Name));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(r => r.Is_Enable == search.Is_Enable.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(r => r.Plant_Organization_UID).ThenBy(r => r.BG_Organization_UID).ThenBy(r => r.Fixture_RepairSolution_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(r => array.Contains(r.Fixture_RepairSolution_UID)).OrderBy(r => r.Plant_Organization_UID).ThenBy(r => r.BG_Organization_UID).ThenBy(r => r.Fixture_RepairSolution_UID);
                count = 0;
                return query;
            }
        }
    }
}
