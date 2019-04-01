using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data
{
    public interface IFixture_Repair_D_DefectRepository : IRepository<Fixture_Repair_D_Defect>
    {
    }

    public class Fixture_Repair_D_DefectRepository : RepositoryBase<Fixture_Repair_D_Defect>, IFixture_Repair_D_DefectRepository
    {
        public Fixture_Repair_D_DefectRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
