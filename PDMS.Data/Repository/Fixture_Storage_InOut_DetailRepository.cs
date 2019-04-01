using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Storage_InOut_DetailRepository : IRepository<Fixture_Storage_InOut_Detail>
    {

    }
    public class Fixture_Storage_InOut_DetailRepository : RepositoryBase<Fixture_Storage_InOut_Detail>, IFixture_Storage_InOut_DetailRepository
    {
        public Fixture_Storage_InOut_DetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }


    }
}
