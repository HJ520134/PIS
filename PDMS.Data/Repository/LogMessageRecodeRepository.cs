using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ILogMessageRecodeRepository : IRepository<LogMessageRecord>
    {

    }

    public class LogMessageRecodeRepository : RepositoryBase<LogMessageRecord>, ILogMessageRecodeRepository
    {
        public LogMessageRecodeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }
    }
}
