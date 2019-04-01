using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Repair_DRepository: IRepository<Fixture_Repair_D>
    {
    }

    public class Fixture_Repair_DRepository : RepositoryBase<Fixture_Repair_D>, IFixture_Repair_DRepository {
        public Fixture_Repair_DRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
}
