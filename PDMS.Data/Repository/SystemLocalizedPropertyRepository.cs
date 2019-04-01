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
    public interface ISystemLocalizedPropertyRepository : IRepository<System_LocalizedProperty>
    {
        IQueryable<System_LocalizedProperty> QuerySystemLocalizedProperties(System_LocalizedPropertyModelSearch search, Page page, out int count);
    }
    public class SystemLocalizedPropertyRepository : RepositoryBase<System_LocalizedProperty>, ISystemLocalizedPropertyRepository
    {
        public SystemLocalizedPropertyRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }

        public IQueryable<System_LocalizedProperty> QuerySystemLocalizedProperties(System_LocalizedPropertyModelSearch search, Page page, out int count)
        {
            var query = from w in DataContext.System_LocalizedProperty select w;
            if (string.IsNullOrWhiteSpace(search.ExportUIds))
            {
                if (search != null)
                {
                    if (search.System_Language_UID.HasValue)
                    {
                        query = query.Where(w => w.System_Language_UID == search.System_Language_UID.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(search.Table_Name))
                    {
                        query = query.Where(w => w.Table_Name.Contains(search.Table_Name));
                    }
                    if (search.TablePK_UID.HasValue)
                    {
                        query = query.Where(w => w.TablePK_UID == search.TablePK_UID);
                    }
                    if (!string.IsNullOrWhiteSpace(search.Table_ColumnName))
                    {
                        query = query.Where(w => w.Table_ColumnName.Contains(search.Table_ColumnName));
                    }
                    if (!string.IsNullOrWhiteSpace(search.ResourceValue))
                    {
                        query = query.Where(w => w.ResourceValue.Contains(search.ResourceValue));
                    }
                }
                count = query.Count();
                return query.OrderByDescending(w => w.Modified_Date).GetPage(page);
            }
            else
            {
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.System_LocalizedProperty_UID)).OrderByDescending(w => w.Modified_Date);
                count = 0;
                return query;
            }
        }
    }
}
