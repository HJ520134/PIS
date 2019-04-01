using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IModelLineHRRepository : IRepository<ModelLineHR>
    {
    }
    public class ModelLineHRRepository : RepositoryBase<ModelLineHR>, IModelLineHRRepository
    {
        public ModelLineHRRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
