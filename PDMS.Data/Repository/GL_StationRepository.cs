using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_StationRepository : IRepository<GL_Station>
    {

    }
    public class GL_StationRepository : RepositoryBase<GL_Station>, IGL_StationRepository
    {

        public GL_StationRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
