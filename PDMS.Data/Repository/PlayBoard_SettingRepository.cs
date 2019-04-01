using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IPlayBoard_SettingRepository : IRepository<PlayBoard_Setting>
    {
    }
    public class PlayBoard_SettingRepository : RepositoryBase<PlayBoard_Setting>, IPlayBoard_SettingRepository
    {
        public PlayBoard_SettingRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
