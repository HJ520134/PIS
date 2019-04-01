using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_WIPShiftBatchOutputRepository : IRepository<GL_WIPShiftBatchOutput>
    {
    }
    public class GL_WIPShiftBatchOutputRepository : RepositoryBase<GL_WIPShiftBatchOutput>, IGL_WIPShiftBatchOutputRepository
    {
        public GL_WIPShiftBatchOutputRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
