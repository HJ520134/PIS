using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Linq;

namespace PDMS.Data.Repository
{
    public class SystemUserRoleRepository : RepositoryBase<System_User_Role>, ISystemUserRoleRepository
    {
        public SystemUserRoleRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// Query system users and order by account_id asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns></returns>
        public IQueryable<UserRoleItem> QueryUserRoles(UserRoleSearchModel search, Page page, out int count)
        {
            var query = from userrole in DataContext.System_User_Role.Include("System_Users").Include("System_Role")
                        join user in DataContext.System_Users on userrole.Modified_UID equals user.Account_UID
                        select new UserRoleItem
                        {
                            System_User_Role_UID = userrole.System_User_Role_UID,
                            User_NTID = userrole.System_Users.User_NTID,
                            User_Name = userrole.System_Users.User_Name,
                            Role_ID = userrole.System_Role.Role_ID,
                            Role_Name = userrole.System_Role.Role_Name,
                            Modified_Date = userrole.Modified_Date, 
                            Modified_UserName = user.User_Name,
                            Modified_UserNTID = user.User_NTID     
                        };
          if (string.IsNullOrEmpty(search.ExportUIds))
          {
                    if (search.System_User_Role_UID > 0)
                {
                    query = query.Where(p => p.System_User_Role_UID == search.System_User_Role_UID);
                }
                if (!string.IsNullOrWhiteSpace(search.User_NTID))
                {
                    query = query.Where(p => p.User_NTID == search.User_NTID);
                }
                if (!string.IsNullOrWhiteSpace(search.User_Name))
                {
                    query = query.Where(p => p.User_Name.Contains(search.User_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Role_ID))
                {
                    query = query.Where(p => p.Role_ID == search.Role_ID);
                }
                if (!string.IsNullOrWhiteSpace(search.Role_Name))
                {
                    query = query.Where(p => p.Role_Name == search.Role_Name);
                }
                if (!string.IsNullOrWhiteSpace(search.Modified_By))
                { 
                    query = query.Where(p => p.Modified_UserNTID == search.Modified_By);
                }
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(p => p.Modified_Date >= search.Modified_Date_From);
                }
                if (search.Modified_Date_End != null)
                {
                    var endDate = ((DateTime)search.Modified_Date_End).AddDays(1);
                    query = query.Where(p => p.Modified_Date < endDate);
                }
                count = query.Count();

                return query.OrderBy(o => o.System_User_Role_UID).GetPage(page);
           }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.System_User_Role_UID));

                count = 0;
                return query
                            .OrderBy(o => o.System_User_Role_UID)
                            .ThenBy(o => o.Role_ID);
                            
            }

        }
    }

    public interface ISystemUserRoleRepository : IRepository<System_User_Role>
    {
        /// <summary>
        /// Query system users and order by account_id asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns>paged records</returns>
        IQueryable<UserRoleItem> QueryUserRoles(UserRoleSearchModel search, Page page, out int count);
    }
}
