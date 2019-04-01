using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixture_Part_Order_ScheduleRepository : IRepository<Fixture_Part_Order_Schedule>
    {

    }

    public class Fixture_Part_Order_ScheduleRepository : RepositoryBase<Fixture_Part_Order_Schedule>, IFixture_Part_Order_ScheduleRepository
    {
        public Fixture_Part_Order_ScheduleRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
    }
}
