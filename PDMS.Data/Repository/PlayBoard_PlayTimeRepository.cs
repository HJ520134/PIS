using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IPlayBoard_PlayTimeRepository : IRepository<PlayBoard_PlayTime>
    {
    }
    public class PlayBoard_PlayTimeRepository : RepositoryBase<PlayBoard_PlayTime>, IPlayBoard_PlayTimeRepository
    {
        public PlayBoard_PlayTimeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
