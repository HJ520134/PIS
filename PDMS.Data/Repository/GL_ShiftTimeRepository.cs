using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_ShiftTimeRepository : IRepository<GL_ShiftTime>
    {
    }
    public class GL_ShiftTimeRepository : RepositoryBase<GL_ShiftTime>, IGL_ShiftTimeRepository
    {

        public GL_ShiftTimeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }


    }
}
