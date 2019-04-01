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
    public interface ITopTenQeboardRepository : IRepository<TopTenQeboard>
    {

        

    }
    public class TopTenQeboardRepository : RepositoryBase<TopTenQeboard>, ITopTenQeboardRepository
    {
        public TopTenQeboardRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
    
    }
}
