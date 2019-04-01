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
    public interface IFixture_PartRepository : IRepository<Fixture_Part>
    {
        IQueryable<Fixture_Part> QueryFixtureParts(Fixture_PartModelSearch search, Page page, out int count);
    }
    public class Fixture_PartRepository : RepositoryBase<Fixture_Part>, IFixture_PartRepository
    {
        public Fixture_PartRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public IQueryable<Fixture_Part> QueryFixtureParts(Fixture_PartModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.Fixture_Part select w;
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
                    if (!string.IsNullOrWhiteSpace( search.Part_ID))
                    {
                        query = query.Where(w => w.Part_ID.Contains(search.Part_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Part_Name))
                    {
                        query = query.Where(w => w.Part_Name.Contains(search.Part_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Part_Spec))
                    {
                        query = query.Where(w => w.Part_Spec.Contains(search.Part_Spec));
                    }
                    if (search.Purchase_Cycle.HasValue)
                    {
                        query = query.Where(w => w.Purchase_Cycle == search.Purchase_Cycle.Value);
                    }
                    if (search.Is_Automation.HasValue)
                    {
                        query = query.Where(w => w.Is_Automation == search.Is_Automation.Value);
                    }
                    if (search.Is_Standardized.HasValue)
                    {
                        query = query.Where(w => w.Is_Standardized == search.Is_Standardized.Value);
                    }
                    if (search.Is_Storage_Managed.HasValue)
                    {
                        query = query.Where(w => w.Is_Storage_Managed == search.Is_Storage_Managed.Value);
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Fixture_Part_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Fixture_Part_UID)).OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Fixture_Part_UID);
                count = 0;
                return query;
            }
        }
    }
}
