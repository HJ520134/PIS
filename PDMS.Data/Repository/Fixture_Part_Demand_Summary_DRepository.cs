using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Part_Demand_Summary_DRepository : IRepository<Fixture_Part_Demand_Summary_D>
    {
        //IQueryable<Fixture_Part_Demand_M> QueryFixturePartDemandMs(Fixture_Part_Summary_Demand_DModelSearch search, Page page, out int count);
    }
    public class Fixture_Part_Demand_Summary_DRepository : RepositoryBase<Fixture_Part_Demand_Summary_D>, IFixture_Part_Demand_Summary_DRepository
    {
        public Fixture_Part_Demand_Summary_DRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
    }
}
