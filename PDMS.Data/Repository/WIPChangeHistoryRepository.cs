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
    public class WIPChangeHistoryRepository : RepositoryBase<WIP_Change_History>, IWIPChangeHistoryRepository
    {
        public WIPChangeHistoryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }

    public interface IWIPChangeHistoryRepository : IRepository<WIP_Change_History>
    {

    }
}
