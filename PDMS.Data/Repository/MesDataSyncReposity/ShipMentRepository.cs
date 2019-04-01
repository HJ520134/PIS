using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository.MesDataSyncReposity
{
    public interface IShipMentRepository : IRepository<Mes_OutShipMent>
    {
    }

    public class ShipMentRepository : RepositoryBase<Mes_OutShipMent>, IShipMentRepository
    {
        public ShipMentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }
    }
}
