using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.Settings;

namespace PDMS.Data.Repository
{
    public interface ISystemBUDOrgRepository : IRepository<System_BU_D_Org>
    {
        IQueryable<System_BU_D_Org> QueryBU_Org(BUD_OrgSearch search, Page page, out int count);

        
    }

    public class SystemBUDOrgRepository : RepositoryBase<System_BU_D_Org>, ISystemBUDOrgRepository
    {
        public SystemBUDOrgRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }


        public IQueryable<System_BU_D_Org> QueryBU_Org(BUD_OrgSearch search, Page page, out int count)
        {
            var query = from bu in DataContext.System_BU_D_Org
                        select bu;
         

                count = query.Count();

                return query.OrderBy(m => m.System_BU_D_Org_UID).GetPage(page);
          
        }



    }
}
