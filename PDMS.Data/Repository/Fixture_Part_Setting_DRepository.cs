using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Part_Setting_DRepository : IRepository<Fixture_Part_Setting_D>
    {
    }
    public class Fixture_Part_Setting_DRepository : RepositoryBase<Fixture_Part_Setting_D>, IFixture_Part_Setting_DRepository
    {
        public Fixture_Part_Setting_DRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
    }
}
