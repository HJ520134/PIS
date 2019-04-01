using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace PDMS.Data.Repository
{
    public interface IProcess_InfoRepository : IRepository<Process_Info>
    {
        IQueryable<Process_Info> QueryProcess_Infos(Process_InfoModelSearch search, Page page, out int count);
        IQueryable<Process_Info> QueryProcess_InfoList(Process_InfoModelSearch search);
    }

    public class Process_InfoRepository : RepositoryBase<Process_Info>, IProcess_InfoRepository
    {
        public Process_InfoRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        public IQueryable<Process_Info> QueryProcess_InfoList(Process_InfoModelSearch search)
        {
            var query = from w in DataContext.Process_Info select w;
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
                if (!string.IsNullOrWhiteSpace(search.Process_ID))
                {
                    query = query.Where(w => w.Process_ID.Contains(search.Process_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.Process_Name))
                {
                    query = query.Where(w => w.Process_Name.Contains(search.Process_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Process_Desc))
                {
                    query = query.Where(w => w.Process_Desc.Contains(search.Process_Desc));
                }
                if (search.Is_Enable.HasValue)
                {
                    query = query.Where(w => w.Is_Enable == search.Is_Enable);
                }
            }
            return query;
        }

        public IQueryable<Process_Info> QueryProcess_Infos(Process_InfoModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.Process_Info select w;
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
                    if (!string.IsNullOrWhiteSpace(search.Process_ID))
                    {
                        query = query.Where(w => w.Process_ID.Contains(search.Process_ID));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Process_Name))
                    {
                        query = query.Where(w => w.Process_Name.Contains(search.Process_Name));
                    }
                    if (!string.IsNullOrWhiteSpace(search.Process_Desc))
                    {
                        query = query.Where(w => w.Process_Desc.Contains(search.Process_Desc));
                    }
                    if (search.Is_Enable.HasValue)
                    {
                        query = query.Where(w => w.Is_Enable == search.Is_Enable);
                    }
                }
                count = query.Count();
                return query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Process_Info_UID).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Process_Info_UID)).OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Process_Info_UID);

                count = 0;
                return query.OrderBy(o => o.Process_Name);
            }

        }
    }
}
