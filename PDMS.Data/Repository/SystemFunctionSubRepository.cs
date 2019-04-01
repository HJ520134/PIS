using PDMS.Data.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;

namespace PDMS.Data.Repository
{
    public class SystemFunctionSubRepository : RepositoryBase<System_FunctionSub>, ISystemFunctionSubRepository
    {
        public SystemFunctionSubRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }

    public interface ISystemFunctionSubRepository : IRepository<System_FunctionSub>
    {
       
    }
}
