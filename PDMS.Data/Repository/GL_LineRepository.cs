using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_LineRepository : IRepository<GL_Line>
    {
    }
    public class GL_LineRepository : RepositoryBase<GL_Line>, IGL_LineRepository
    {
        public GL_LineRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
