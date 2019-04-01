using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ISystemViewColumnRepository : IRepository<System_View_Column>
    {

    }

    public class SystemViewColumnRepository : RepositoryBase<System_View_Column>, ISystemViewColumnRepository
    {
        public SystemViewColumnRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
