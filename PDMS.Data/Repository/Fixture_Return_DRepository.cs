using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Return_DRepository : IRepository<Fixture_Return_D>
    {

    }

    public class Fixture_Return_DRepository : RepositoryBase<Fixture_Return_D>, IFixture_Return_DRepository
    {
        public Fixture_Return_DRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }




    }

}
