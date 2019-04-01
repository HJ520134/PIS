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
    public interface IQTrace_TopTen_SumRepository : IRepository<QTrace_TopTen_Sum>
    {



    }
    public class QTrace_TopTen_SumRepository : RepositoryBase<QTrace_TopTen_Sum>, IQTrace_TopTen_SumRepository
    {
        public QTrace_TopTen_SumRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

    }
}
