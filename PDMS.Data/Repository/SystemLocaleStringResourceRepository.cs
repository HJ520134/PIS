using PDMS.Data.Infrastructure;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ISystemLocaleStringResourceRepository : IRepository<System_LocaleStringResource>
    {
        List<SystemLocaleStringResourceDTO> LocaleStringResourceInfo(SystemLocaleStringResourceDTO searchModel, Page page, out int count);
    }

    public class SystemLocaleStringResourceRepository : RepositoryBase<System_LocaleStringResource>, ISystemLocaleStringResourceRepository
    {
        public SystemLocaleStringResourceRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<SystemLocaleStringResourceDTO> LocaleStringResourceInfo(SystemLocaleStringResourceDTO searchModel, Page page, out int count)
        {
            var linq = from A in DataContext.System_LocaleStringResource
                       where A.System_Language_UID == searchModel.System_Language_UID
                       select new SystemLocaleStringResourceDTO
                       {
                           System_LocaleStringResource_UID = A.System_LocaleStringResource_UID,
                           System_Language_UID = A.System_Language_UID,
                           ResourceName = A.ResourceName,
                           ResourceValue = A.ResourceValue
                       };
            if (!string.IsNullOrEmpty(searchModel.ResourceName))
            {
                linq = linq.Where(m => m.ResourceName.Contains(searchModel.ResourceName.Trim()));
            }

            if (!string.IsNullOrEmpty(searchModel.ResourceValue))
            {
                linq = linq.Where(m => m.ResourceValue.Contains(searchModel.ResourceValue.Trim()));
            }

            count = linq.Count();
            linq = linq.OrderBy(m => m.ResourceName).Skip(page.Skip).Take(page.PageSize);
            var list = linq.ToList();
            return list;
        }
    }
}
