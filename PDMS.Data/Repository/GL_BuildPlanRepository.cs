using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGL_BuildPlanRepository : IRepository<GL_BuildPlan>
    {
    }
    public class GL_BuildPlanRepository : RepositoryBase<GL_BuildPlan>, IGL_BuildPlanRepository
    {
        public GL_BuildPlanRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
