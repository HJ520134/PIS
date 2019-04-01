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
    public interface ISystemBUDRepository : IRepository<System_BU_D>
    {
        IQueryable<System_BU_D> QueryBUDetails(BUDetailModelSearch search, Page page, out int count);
        bool CheckExistBU_D_ID(string BU_D_ID, int BU_D_UID, bool isEdit);
        IQueryable<System_BU_D> GetValidBUDsByUserUId(int accountUId);
        IQueryable<string> QueryDistinctCustomer(List<int> userProjectUid,string oporg);
        List<System_BU_D> QueryBUDetailsByUser(BUDetailModelSearch search, Page page, out int count);
    }

    public class SystemBUDRepository : RepositoryBase<System_BU_D>, ISystemBUDRepository
    {
        public SystemBUDRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// 这里只查询该用户所鞥看到的BU
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<System_BU_D> QueryBUDetailsByUser(BUDetailModelSearch search, Page page, out int count)
        {
            //var query = from bud in DataContext.System_BU_D.Include("System_Users").Include("System_BU_M")
            //            .Include("System_BU_Org")
            //            select bud;
            var query = from bud in DataContext.System_BU_D join M in DataContext.System_BU_M on bud.BU_M_UID equals M.BU_M_UID
                         join U in DataContext.System_Users on bud.Modified_UID equals U.Modified_UID
                         join Bo in DataContext.System_BU_D_Org on bud.BU_D_UID equals Bo.BU_D_UID
                         where   search.Organization_UID==Bo.Organization_UID
                         select bud;

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrWhiteSpace(search.BU_ID))
                {
                    query= query.Where(m => m.System_BU_M.BU_ID.Equals(search.BU_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.BU_Name))
                {
                    query = query.Where(m => m.System_BU_M.BU_Name.Contains(search.BU_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.BU_D_ID))
                {
                    query = query.Where(m => m.BU_D_ID.Equals(search.BU_D_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.BU_D_Name))
                {
                    query = query.Where(m => m.BU_D_Name.Contains(search.BU_D_Name));
                }
              
                if (search.Reference_Date != null)
                {
                    switch (search.queryTypes)
                    {
                        case ConstConstants.Valid:
                            query = query.Where(m =>
                            (m.End_Date == null && SqlFunctions.DateDiff("dd", m.Begin_Date, search.Reference_Date.Value) >= 0)
                            ||
                            (
                                m.End_Date != null && (SqlFunctions.DateDiff("dd", m.Begin_Date, search.Reference_Date.Value)) >= 0
                                &&
                                SqlFunctions.DateDiff("dd", m.End_Date, search.Reference_Date) <= 0
                            )
                            );
                            break;
                        case ConstConstants.Invalid:
                            query = query.Where(m =>
                            (m.End_Date == null && SqlFunctions.DateDiff("dd", m.Begin_Date, search.Reference_Date.Value) < 0)
                            ||
                            (
                                m.End_Date != null && SqlFunctions.DateDiff("dd", m.End_Date, search.Reference_Date.Value) > 0
                            )
                            );
                            break;
                    }
                }
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_From) <= 0);
                }
                if (search.Modified_Date_End != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_End) >= 0);
                }
                if (!string.IsNullOrWhiteSpace(search.Modified_By))
                {
                    query = query.Where(m => m.System_Users.User_NTID == search.Modified_By);
                }

                count = query.Count();

                query = query.OrderBy(m => m.System_BU_M.BU_ID).ThenBy(m => m.BU_D_ID).GetPage(page);
                return query.ToList();
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.BU_D_UID));

                count = 0;
                return query.OrderBy(m => m.System_BU_M.BU_ID).ThenBy(m => m.BU_D_ID).ToList();
            }
        }
        public IQueryable<System_BU_D> QueryBUDetails(BUDetailModelSearch search, Page page, out int count)
        {
            var query = from bud in DataContext.System_BU_D.Include("System_Users").Include("System_BU_M")
                        select bud;
            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrWhiteSpace(search.BU_ID))
                {
                    query = query.Where(m => m.System_BU_M.BU_ID.Equals(search.BU_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.BU_Name))
                {
                    query = query.Where(m => m.System_BU_M.BU_Name.Contains(search.BU_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.BU_D_ID))
                {
                    query = query.Where(m => m.BU_D_ID.Equals(search.BU_D_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.BU_D_Name))
                {
                    query = query.Where(m => m.BU_D_Name.Contains(search.BU_D_Name));
                }
                if (search.Reference_Date != null)
                {
                    switch (search.queryTypes)
                    {
                        case ConstConstants.Valid:
                            query = query.Where(m =>
                            (m.End_Date == null && SqlFunctions.DateDiff("dd", m.Begin_Date, search.Reference_Date.Value) >= 0)
                            ||
                            (
                                m.End_Date != null && (SqlFunctions.DateDiff("dd", m.Begin_Date, search.Reference_Date.Value)) >= 0
                                &&
                                SqlFunctions.DateDiff("dd", m.End_Date, search.Reference_Date) <= 0
                            )
                            );
                            break;
                        case ConstConstants.Invalid:
                            query = query.Where(m =>
                            (m.End_Date == null && SqlFunctions.DateDiff("dd", m.Begin_Date, search.Reference_Date.Value) < 0)
                            ||
                            (
                                m.End_Date != null && SqlFunctions.DateDiff("dd", m.End_Date, search.Reference_Date.Value) > 0
                            )
                            );
                            break;
                    }
                }
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_From) <= 0);
                }
                if (search.Modified_Date_End != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_End) >= 0);
                }
                if (!string.IsNullOrWhiteSpace(search.Modified_By))
                {
                    query = query.Where(m => m.System_Users.User_NTID == search.Modified_By);
                }

                count = query.Count();

                query = query.OrderBy(m => m.System_BU_M.BU_ID).ThenBy(m => m.BU_D_ID).GetPage(page);
                return query;
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.BU_D_UID));

                count = 0;
                return query.OrderBy(m => m.System_BU_M.BU_ID).ThenBy(m => m.BU_D_ID);
            }
        }

        public bool CheckExistBU_D_ID(string BU_D_ID, int BU_D_UID, bool isEdit)
        {
            bool result = false;
            if (!isEdit)
            {
                var query = DataContext.System_BU_D.Where(m => m.BU_D_ID == BU_D_ID).FirstOrDefault();
                if (query == null) //验证通过
                {
                    result = true;
                }
            }
            else
            {
                var query = DataContext.System_BU_D.Where(m => m.BU_D_UID != BU_D_UID && m.BU_D_ID == BU_D_ID).FirstOrDefault();
                if (query == null)
                {
                    result = true;
                }
            }
            return result;
        }

        public IQueryable<System_BU_D> GetValidBUDsByUserUId(int accountUId)
        {
            var now = DateTime.Now.Date;
            var query = from bud in DataContext.System_BU_D
                        join user_bu in DataContext.System_User_Business_Group on bud.BU_D_UID equals user_bu.BU_D_UID
                        where user_bu.Account_UID == accountUId
                            && ((user_bu.Begin_Date <= now && !user_bu.End_Date.HasValue)
                                || (user_bu.Begin_Date <= now && user_bu.End_Date >= now))
                        select bud;
            return query;
        }

        public IQueryable<string> QueryDistinctCustomer(List<int> userProjectUid,string oporg)
        {
            if (userProjectUid!=null)
            {
                var result = from bu in DataContext.System_BU_D
                             join project in DataContext.System_Project on bu.BU_D_UID equals project.BU_D_UID
                             join bo in DataContext.System_BU_D_Org on  bu.BU_D_UID equals bo.BU_D_UID
                              where (userProjectUid.Contains(project.Project_UID)) & oporg.Contains("," + bo.Organization_UID + ",")    // 映射注释
                             select bu;
            return result.Select(p=>p.BU_D_Name).Distinct();
            }
            else if(oporg != null&& oporg!=",")
            {
                var result = from bu in DataContext.System_BU_D
                             join bo in DataContext.System_BU_D_Org on bu.BU_D_UID equals bo.BU_D_UID
                             where oporg.Contains("," + bo.Organization_UID + ",") //映射注释
                             select bu;
                return result.Select(m => m.BU_D_Name).Distinct();
            }
            else
            {
                var result = DataContext.System_BU_D.Select(m => m.BU_D_Name).Distinct();
                return result;
            }
        }
    }
}
