using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ISystemLanguageRepository : IRepository<System_Language>
    {

    }

    public class SystemLanguageRepository : RepositoryBase<System_Language>, ISystemLanguageRepository
    {
        public SystemLanguageRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

    }
}
