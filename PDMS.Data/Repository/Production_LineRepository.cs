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
    public interface IProduction_LineRepository : IRepository<Production_Line>
    {
        IQueryable<Production_Line> QueryProduction_Lines(Production_LineModelSearch search, Page page, out int count);

    }
    public class Production_LineRepository : RepositoryBase<Production_Line>, IProduction_LineRepository
    {
        public Production_LineRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        public IQueryable<Production_Line> QueryProduction_Lines(Production_LineModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.Production_Line select w;
            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.Plant_Organization_UID > 0)
                    {
                        query = query.Where(l => l.Plant_Organization_UID == search.Plant_Organization_UID);
                    }
                    if (search.BG_Organization_UID > 0)
                    {
                        query = query.Where(l => l.BG_Organization_UID == search.BG_Organization_UID);
                    }
                    if (search.FunPlant_Organization_UID.HasValue)
                    {
                        query = query.Where(l => l.FunPlant_Organization_UID == search.FunPlant_Organization_UID.Value);
                    }

                    if (search.Workshop_UID > 0)
                    {
                        query = query.Where(l => l.Workshop_UID == search.Workshop_UID);
                    }
                    if (search.Workstation_UID > 0)
                    {
                        query = query.Where(l => l.Workstation_UID == search.Workstation_UID);
                    }
                    if (search.Project_UID > 0)
                    {
                        query = query.Where(l => l.Project_UID == search.Project_UID);
                    }
                    if (search.Process_Info_UID > 0)
                    {
                        query = query.Where(l => l.Process_Info_UID == search.Process_Info_UID);
                    }

                    if (!string.IsNullOrWhiteSpace(search.Line_ID ))
                    {
                        query = query.Where(l => l.Line_ID.Contains(search.Line_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Line_Name))
                    {
                        query = query.Where(l => l.Line_Name.Contains(search.Line_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Line_Desc))
                    {
                        query = query.Where(l => l.Line_Desc.Contains(search.Line_Desc));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(l => l.Is_Enable == search.Is_Enable.Value);
                    }
                }
                count = query.Count();
                return query.OrderBy(l => l.Plant_Organization_UID).ThenBy(l => l.BG_Organization_UID).ThenBy(l => l.Production_Line_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Production_Line_UID));
                count = 0;
                return query.OrderBy(l => l.Plant_Organization_UID).ThenBy(l => l.BG_Organization_UID).ThenBy(l => l.Production_Line_UID);
            }
        }
    }
}
