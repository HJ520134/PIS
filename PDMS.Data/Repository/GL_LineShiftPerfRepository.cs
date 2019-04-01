using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_LineShiftPerfRepository : IRepository<GL_LineShiftPerf>
    {
    }
    public class GL_LineShiftPerfRepository : RepositoryBase<GL_LineShiftPerf>, IGL_LineShiftPerfRepository
    {
        public GL_LineShiftPerfRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
