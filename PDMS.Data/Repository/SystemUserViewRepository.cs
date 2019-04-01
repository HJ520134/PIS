using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ISystemUserViewRepository : IRepository<System_User_View>
    {

    }

    public class SystemUserViewRepository : RepositoryBase<System_User_View>, ISystemUserViewRepository
    {
        public SystemUserViewRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
