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
    public interface IFixture_ResumeRepository : IRepository<Fixture_Resume>
    {
      
    }
   public  class Fixture_ResumeRepository : RepositoryBase<Fixture_Resume>, IFixture_ResumeRepository
    {
        public Fixture_ResumeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
    }
}
