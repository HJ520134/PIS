using System;
using System.Linq;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Data;

namespace PDMS.Data.Repository
{
    public class SystemRoleFunctionRepository : RepositoryBase<System_Role_Function>, ISystemRoleFunctionRepository
    {
        public SystemRoleFunctionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        #region System Role Function Maintenance Module -------------- Add by Tonny 2015-11-18
        /// <summary>
        /// get system role functions for Role Function Setting page
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<RoleFunctionItem> QueryRoleFunctions(RoleFunctionSearchModel search, Page page, out int count)
        {
            var query = from func in DataContext.System_Role_Function.Include("System_Users").Include("System_Role").Include("System_Function")
                        join func_sub in DataContext.System_Role_FunctionSub 
                            on func.System_Role_Function_UID equals func_sub.System_Role_Function_UID into g1
                        from g2 in g1.DefaultIfEmpty()
                        group g2 by func into g
                        select new RoleFunctionItem
                        {
                            System_Role_Function_UID=g.Key.System_Role_Function_UID,
                            Role_UID=g.Key.Role_UID,
                            Function_UID = g.Key.Function_UID,
                            Role_ID = g.Key.System_Role.Role_ID,
                            Function_ID = g.Key.System_Function.Function_ID,
                            Function_Name = g.Key.System_Function.Function_Name,
                            Order_Index = g.Key.System_Function.Order_Index,
                            Is_Show_Function = g.Key.System_Function.Is_Show,
                            Is_Show_Role=g.Key.Is_Show,
                            URL = g.Key.System_Function.URL,
                            SubFunctionCount = g.Count(s => s.System_Role_FunctionSub_UID > 0),
                            Modified_Date = g.Key.Modified_Date,
                            Modified_UserName=g.Key.System_Users.User_Name,
                            Modified_UserNTID=g.Key.System_Users.User_NTID
                        };

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (search.Role_UID > 0)
                {
                    query = query.Where(p => p.Role_UID == search.Role_UID);
                }

                if (!string.IsNullOrWhiteSpace(search.Function_ID))
                {
                    query = query.Where(p => p.Function_ID.Contains(search.Function_ID));
                }

                if (!string.IsNullOrWhiteSpace(search.Function_Name))
                {
                    query = query.Where(p => p.Function_Name.Contains(search.Function_Name));
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
                return query
                        .OrderBy(o => o.Role_ID)
                        .ThenBy(o => o.Function_ID)
                        .GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.System_Role_Function_UID));

                count = 0;
                return query
                        .OrderBy(o => o.Role_ID)
                        .ThenBy(o => o.Function_ID);
            }
        }
        
        #endregion

    }

    public interface ISystemRoleFunctionRepository : IRepository<System_Role_Function>
    {
        /// <summary>
        /// get system role functions for Role Function Setting page
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IQueryable<RoleFunctionItem> QueryRoleFunctions(RoleFunctionSearchModel search, Page page, out int count);
    }
}
