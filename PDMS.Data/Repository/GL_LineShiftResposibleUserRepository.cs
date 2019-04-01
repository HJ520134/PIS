using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_LineShiftResposibleUserRepository : IRepository<GL_LineShiftResposibleUser>
    {
    }
    public class GL_LineShiftResposibleUserRepository : RepositoryBase<GL_LineShiftResposibleUser>, IGL_LineShiftResposibleUserRepository
    {
        public GL_LineShiftResposibleUserRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
