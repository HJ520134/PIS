using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_WIPShiftOutputRepository : IRepository<GL_WIPShiftOutput>
    {
    }
    public class GL_WIPShiftOutputRepository : RepositoryBase<GL_WIPShiftOutput>, IGL_WIPShiftOutputRepository
    {
        public GL_WIPShiftOutputRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
