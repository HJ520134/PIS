using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IWorkshopRepository : IRepository<Workshop>
    {
        /// <summary>
        /// Query Workshop and order by Building_Name asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns>paged records</returns>
        IQueryable<Workshop> QueryWorkshops(WorkshopModelSearch search, Page page, out int count);
    }
    public class WorkshopRepository : RepositoryBase<Workshop>, IWorkshopRepository
    {
        public WorkshopRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
        
        public IQueryable<Workshop> QueryWorkshops(WorkshopModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.Workshop select w;
            if (string.IsNullOrWhiteSpace( search.ExportUIds))
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
                    if (search.FunPlant_Organization_UID > 0)
                    {
                        query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.Workshop_ID))
                    {
                        query = query.Where(w => w.Workshop_ID.Contains(search.Workshop_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Workshop_Name))
                    {
                        query = query.Where(w => w.Workshop_Name.Contains(search.Workshop_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Building_Name))
                    {
                        query = query.Where(w => w.Building_Name.Contains(search.Building_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Floor_Name))
                    {
                        query = query.Where(w => w.Floor_Name.Contains(search.Floor_Name));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(w => w.Is_Enable == search.Is_Enable);
                    }
                }
                count = query.Count();
                return query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w=>w.FunPlant_Organization_UID).ThenBy(w => w.Workshop_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Workshop_UID)).OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.FunPlant_Organization_UID).ThenBy(w => w.Workshop_UID);

                count = 0;
                return query.OrderBy(o => o.Workshop_Name);
            }
        }
    }
}
