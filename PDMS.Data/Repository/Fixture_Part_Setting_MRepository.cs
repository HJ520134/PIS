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
    public interface IFixture_Part_Setting_MRepository : IRepository<Fixture_Part_Setting_M>
    {
        IQueryable<Fixture_Part_Setting_M> QueryFixturePartSettingMs(Fixture_Part_Setting_MModelSearch search, Page page, out int count);
    }
    public class Fixture_Part_Setting_MRepository : RepositoryBase<Fixture_Part_Setting_M>, IFixture_Part_Setting_MRepository
    {
        public Fixture_Part_Setting_MRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
        public IQueryable<Fixture_Part_Setting_M> QueryFixturePartSettingMs(Fixture_Part_Setting_MModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.Fixture_Part_Setting_M select w;
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
                    if (search.FunPlant_Organization_UID.HasValue && search.FunPlant_Organization_UID.Value>0)
                    {
                        query = query.Where(w => w.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(search.Fixture_NO))
                    {
                        query = query.Where(w => w.Fixture_NO.Contains(search.Fixture_NO));
                    }
                    if (search.Line_Qty.HasValue)
                    {
                        query = query.Where(w => w.Line_Qty==search.Line_Qty);
                    }
                    if (search.Line_Fixture_Ratio_Qty.HasValue)
                    {
                        query = query.Where(w => w.Line_Fixture_Ratio_Qty==search.Line_Fixture_Ratio_Qty);
                    }
                    if (search.UseTimesScanInterval.HasValue)
                    {
                        query = query.Where(w => w.UseTimesScanInterval == search.UseTimesScanInterval.Value);
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(w => w.Is_Enable == search.Is_Enable.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Fixture_NO).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Fixture_Part_Setting_M_UID)).OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Fixture_NO);
                count = 0;
                return query;
            }
        }
    }
}
