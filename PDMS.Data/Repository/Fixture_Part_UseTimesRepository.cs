using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Part_UseTimesRepository : IRepository<Fixture_Part_UseTimes>
    {

    }
    public class Fixture_Part_UseTimesRepository : RepositoryBase<Fixture_Part_UseTimes>, IFixture_Part_UseTimesRepository
    {
        public Fixture_Part_UseTimesRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
