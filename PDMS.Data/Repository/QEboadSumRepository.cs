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
    public interface IQEboadSumRepository : IRepository<QEboardSum>
    {



    }
    public class QEboadSumRepository : RepositoryBase<QEboardSum>, IQEboadSumRepository
    {
        public QEboadSumRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

    }
}
