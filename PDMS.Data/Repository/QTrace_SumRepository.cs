using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IQTrace_SumRepository : IRepository<QTrace_Sum>
    {



    }
    public class QTrace_SumRepository : RepositoryBase<QTrace_Sum>, IQTrace_SumRepository
    {
        public QTrace_SumRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

    }
}
