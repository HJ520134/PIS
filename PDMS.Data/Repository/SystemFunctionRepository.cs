using PDMS.Data.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public class SystemFunctionRepository : RepositoryBase<System_Function>, ISystemFunctionRepository
    {
        public SystemFunctionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        #region For System Use
        /// <summary>
        /// 根据用户UID获取菜单列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IEnumerable<System_Function> GetFunctionsByUserUId(int uid, int LanguageId)
        {
            var paramUId = new SqlParameter("id", uid);
            var paramLanguageId = new SqlParameter("languageId", LanguageId);

            return ((IObjectContextAdapter)this.DataContext).ObjectContext
                        .ExecuteStoreQuery<System_Function>("usp_get_functions_by_useruid @id, @languageId", paramUId, paramLanguageId)
                        .AsEnumerable();
        }

        /// <summary>
        /// 根据用户UID获取Mobile菜单列表 这里需要修改为获取两个表的数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IEnumerable<System_Function> GetMobileFunctionsByUserUId(int uid)
        {
            var paramUId = new SqlParameter("id", uid);

            return ((IObjectContextAdapter)this.DataContext).ObjectContext
                        .ExecuteStoreQuery<System_Function>("usp_get_mobilefunctions_by_useruid @id", paramUId)
                        .AsEnumerable();
        }

        /// <summary>
        /// 获取用户未被授权的页面元素
        /// </summary>
        /// <param name="ntid"></param>
        /// <returns></returns>
        public IEnumerable<PageUnauthorizedElement> GetUnauthorizedElementsByNTId(string ntid)
        {
            var paramUId = new SqlParameter("ntid", ntid);

            return ((IObjectContextAdapter)this.DataContext).ObjectContext
                        .ExecuteStoreQuery<PageUnauthorizedElement>("usp_get_unauthorized_elements_by_ntid @ntid", paramUId)
                        .AsEnumerable();
        }

        /// <summary>
        /// 根据用户ID和URL，查看用户是否被授权访问
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool HasActionQulification(int uid, string url)
        {
            var paramUId = new SqlParameter("uid", uid);
            var paramUrl = new SqlParameter("url", url);
            var paramExist = new SqlParameter
            {
                ParameterName = "Exist",
                Value = false,
                Direction = ParameterDirection.Output
            };

            ((IObjectContextAdapter)this.DataContext).ObjectContext
                       .ExecuteStoreCommand("usp_check_action_qulification @uid,@url,@Exist out", paramUId, paramUrl, paramExist);

            return (bool)paramExist.Value;
        }
        #endregion //For System Use

        #region System Function Maintenance Module -------------- Add by Tonny 2015-11-11
        /// <summary>
        /// get system functions
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<FunctionItem> QueryFunctions(FunctionModelSearch search, Page page, out int count)
        {
            var query = from func in DataContext.System_Function.Include("System_Users")
                        join func_sub in DataContext.System_FunctionSub on func.Function_UID equals func_sub.Function_UID into g1
                        from g2 in g1.DefaultIfEmpty()
                        group g2 by func into g
                        select new
                        {
                            Function_Parent_UID = g.Key.Parent_Function_UID,
                            Function_UID = g.Key.Function_UID,
                            Function_ID = g.Key.Function_ID,
                            Function_Name = g.Key.Function_Name,
                            Order_Index = g.Key.Order_Index,
                            Is_Show = g.Key.Is_Show,
                            URL = g.Key.URL,
                            SubFunctionCount = g.Count(s => s.Function_UID > 0),
                            Modified_Date = g.Key.Modified_Date,
                            Modified_User = g.Key.System_Users,
                        };

            var queryWithParent = from funcs in query
                                  join parent in DataContext.System_Function on funcs.Function_Parent_UID equals parent.Function_UID into gj
                                  from pfunc in gj.DefaultIfEmpty()
                                  select new FunctionItem
                                  {
                                      Function_UID = funcs.Function_UID,
                                      Function_ID = funcs.Function_ID,
                                      Function_Name = funcs.Function_Name,
                                      Parent_Function_ID = pfunc.Function_ID,
                                      Parent_Function_Name = pfunc.Function_Name,
                                      Order_Index = funcs.Order_Index,
                                      Is_Show = funcs.Is_Show,
                                      URL = funcs.URL,
                                      SubFunctionCount = funcs.SubFunctionCount,
                                      Modified_UserName = funcs.Modified_User.User_Name,
                                      Modified_Date = funcs.Modified_Date,
                                      Modified_UserNTID = funcs.Modified_User.User_NTID
                                  };

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrWhiteSpace(search.Parent_Function_ID))
                {
                    queryWithParent = queryWithParent.Where(p => p.Parent_Function_ID == search.Parent_Function_ID);
                }

                if (!string.IsNullOrWhiteSpace(search.Parent_Function_Name))
                {
                    queryWithParent = queryWithParent.Where(p => p.Parent_Function_Name.Contains(search.Parent_Function_Name));
                }

                if (!string.IsNullOrWhiteSpace(search.Function_ID))
                {
                    queryWithParent = queryWithParent.Where(p => p.Function_ID.Contains(search.Function_ID));
                }

                if (!string.IsNullOrWhiteSpace(search.Function_Name))
                {
                    queryWithParent = queryWithParent.Where(p => p.Function_Name.Contains(search.Function_Name));
                }

                if (!string.IsNullOrWhiteSpace(search.Modified_By))
                {
                    queryWithParent = queryWithParent.Where(p => p.Modified_UserNTID == search.Modified_By);
                }
                if (search.Modified_Date_From != null)
                {
                    queryWithParent = queryWithParent.Where(p => p.Modified_Date >= search.Modified_Date_From);
                }
                if (search.Modified_Date_End != null)
                {
                    var endDate = ((DateTime)search.Modified_Date_End).AddDays(1);
                    queryWithParent = queryWithParent.Where(p => p.Modified_Date < endDate);
                }

                count = queryWithParent.Count();
                return queryWithParent
                            .OrderBy(o =>o.Parent_Function_ID)
                            .ThenBy(o=>o.Function_ID)
                            .ThenBy(o=>o.Order_Index)
                            .GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                queryWithParent = queryWithParent.Where(p => array.Contains(p.Function_UID));

                count = 0;
                return queryWithParent
                            .OrderBy(o => o.Parent_Function_ID)
                            .ThenBy(o => o.Function_ID)
                            .ThenBy(o => o.Order_Index);
            }
        }

        /// <summary>
        /// Query Function with its subs by uid
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns></returns>
        public System_Function QueryFunctionWithSubs(int uid)
        {
            var query = from func_with_sub in DataContext.System_Function.Include("System_FunctionSub")
                        where func_with_sub.Function_UID == uid
                        select func_with_sub;

            return query.FirstOrDefault();
        }
        #endregion //System Function Maintenance Module

        public List<SystemFunctionDTO> GetFunctionLanguageList()
        {
            var sql = @"SELECT B.System_Language_UID, Function_UID,ISNULL(A.Parent_Function_UID,0) AS Parent_Function_UID,A.Function_UID,
                        A.Function_Name,A.Function_Desc, B.Table_ColumnName,B.ResourceValue,A.Order_Index,A.Icon_ClassName,A.URL,A.Mobile_URL,
                        A.Is_Show,A.Modified_UID,A.Modified_Date
                        FROM dbo.System_Function A
                        LEFT JOIN dbo.System_LocalizedProperty B
                        ON B.Table_Name='System_Function' AND B.TablePK_UID= A.Function_UID
                        WHERE A.URL IS NOT NULL AND B.System_Language_UID IS NOT NULL ";

            var list = DataContext.Database.SqlQuery<SystemFunctionDTO>(sql).ToList();
            return list;
        }
    }

    public interface ISystemFunctionRepository : IRepository<System_Function>
    {
        #region For System Use
        /// <summary>
        /// 根据用户UID获取菜单列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        IEnumerable<System_Function> GetFunctionsByUserUId(int uid, int LanguageId);

        /// <summary>
        /// 根据用户UID获取Mobile菜单列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        IEnumerable<System_Function> GetMobileFunctionsByUserUId(int uid);

        /// <summary>
        /// 获取用户未被授权的页面元素
        /// </summary>
        /// <param name="ntid"></param>
        /// <returns></returns>
        IEnumerable<PageUnauthorizedElement> GetUnauthorizedElementsByNTId(string ntid);

        /// <summary>
        /// 根据用户ID和URL，查看用户是否被授权访问
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        bool HasActionQulification(int uid, string url);
        #endregion //End for system use

        #region  define System Function Maintenance interface -------------- Add by Tonny 2015-11-11
        /// <summary>
        /// System Function Maintenance, get grid data
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IQueryable<FunctionItem> QueryFunctions(FunctionModelSearch search, Page page, out int count);

        /// <summary>
        /// Query Function with its subs by uid
        /// </summary>
        /// <param name="uid">key</param>
        /// <returns></returns>
        System_Function QueryFunctionWithSubs(int uid);
        #endregion //System Function Maintenance Module

        List<SystemFunctionDTO> GetFunctionLanguageList();
    }
}
