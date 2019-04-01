using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IMes_StationColorRepository : IRepository<Mes_StationColor>
    {
        List<Mes_StationColorDTO> GetStationColorList(string Customer);
    }

    public class Mes_StationColorRepository : RepositoryBase<Mes_StationColor>, IMes_StationColorRepository
    {
        public Mes_StationColorRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }

        public List<Mes_StationColorDTO> GetStationColorList(string CustomerName)
        {
            var query = from stColor in DataContext.Mes_StationColor
                        select new Mes_StationColorDTO
                        {
                            Mes_StationColor_UID = stColor.Mes_StationColor_UID,
                            CustomerName = stColor.CustomerName,
                            StationName = stColor.StationName,
                            Color = stColor.Color
                        };
            query = query.Where(p => p.CustomerName == CustomerName);
            return query.ToList();
        }
    }
}
