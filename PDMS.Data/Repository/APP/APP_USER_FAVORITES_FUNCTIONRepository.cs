using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;
using PDMS.Data;
using System.Data.Entity.SqlServer;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository.APP
{
    public interface IAPP_USER_FAVORITES_FUNCTIONRepository: IRepository<APP_USER_FAVORITES_FUNCTION>
    {
        SystemOrgDTO getUserOp(string ntid);
    }
    public class APP_USER_FAVORITES_FUNCTIONRepository : RepositoryBase<APP_USER_FAVORITES_FUNCTION>, IAPP_USER_FAVORITES_FUNCTIONRepository
    {
        public APP_USER_FAVORITES_FUNCTIONRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public SystemOrgDTO getUserOp(string ntid)
        {
            var sqlStr = @"SELECT ISNULL(OPType_OrganizationUID,0) Organization_UID FROM dbo.System_Users t1 
                                    left JOIN dbo.System_UserOrg t2 ON t1.Account_UID=t2.Account_UID
                                    WHERE t1.User_NTID='{0}'";

            sqlStr = string.Format(sqlStr, ntid);
            var dbFirst = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList().FirstOrDefault();
            return dbFirst;
        }
    }
}
