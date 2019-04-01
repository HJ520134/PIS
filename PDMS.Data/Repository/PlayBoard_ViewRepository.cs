using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IPlayBoard_ViewRepository : IRepository<PlayBoard_View>
    {
    }
    public class PlayBoard_ViewRepository: RepositoryBase<PlayBoard_View>,IPlayBoard_ViewRepository
    {
        public PlayBoard_ViewRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
