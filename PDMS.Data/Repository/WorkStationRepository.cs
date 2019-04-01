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
    public interface IWorkStationRepository : IRepository<WorkStation>
    {
        IQueryable<WorkStation> QueryWorkStations(WorkStationModelSearch search, Page page, out int count);
    }
    public class WorkStationRepository : RepositoryBase<WorkStation>, IWorkStationRepository
    {
        public WorkStationRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        public IQueryable<WorkStation> QueryWorkStations(WorkStationModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.WorkStation select w;
            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.Plant_Organization_UID > 0)
                    {
                        query = query.Where(w => w.Plant_Organization_UID == search.Plant_Organization_UID);
                    }
                    if (search.BG_Organization_UID > 0)
                    {
                        query = query.Where(w => w.BG_Organization_UID == search.BG_Organization_UID);
                    }
                    if (search.FunPlant_Organization_UID.HasValue)
                    {
                        query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                    }
                    if (search.Project_UID > 0)
                    {
                        query = query.Where(w => w.Project_UID == search.Project_UID);
                    }
                    if (search.Process_Info_UID > 0)
                    {
                        query = query.Where(w => w.Process_Info_UID == search.Process_Info_UID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.WorkStation_ID))
                    {
                        query = query.Where(w => w.WorkStation_ID.Contains(search.WorkStation_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.WorkStation_Name))
                    {
                        query = query.Where(w => w.WorkStation_Name.Contains(search.WorkStation_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.WorkStation_Desc))
                    {
                        query = query.Where(w => w.WorkStation_Desc.Contains(search.WorkStation_Desc));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.WorkStation_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.WorkStation_UID)).OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.WorkStation_UID);
                count = 0;
                return query;
            }
        }
    }
}
