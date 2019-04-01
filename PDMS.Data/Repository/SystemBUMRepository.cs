using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System.Linq;
using System.Data.Entity.SqlServer;
using System;
using PDMS.Common.Constants;
using System.Collections.Generic;

namespace PDMS.Data.Repository
{
    public interface ISystemBUMRepository : IRepository<System_BU_M>
    {
        /// <summary>
        /// Query system users and order by account_id asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns>paged records</returns>
        IQueryable<System_BU_M> QueryBUs(BUModelSearch search, Page page, out int count);

        /// <summary>
        /// 根据BU_ID获取信息
        /// </summary>
        /// <param name="BU_ID"></param>
        /// <returns></returns>
        System_BU_M GetInfoByBU_ID(string BU_ID, string BU_Name);

        IQueryable<System_BU_M> GetValidBUMsByUserUId(int accountUId);
    }

    public class SystemBUMRepository : RepositoryBase<System_BU_M>, ISystemBUMRepository
    {
        public SystemBUMRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IQueryable<System_BU_M> QueryBUs(BUModelSearch search, Page page, out int count)
        {
            var query = from bu in DataContext.System_BU_M
                        join modifiedUser in DataContext.System_Users
                        on bu.Modified_UID equals modifiedUser.Account_UID
                        select bu;
            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrWhiteSpace(search.BU_ID))
                {
                    query = query.Where(m => m.BU_ID.Contains(search.BU_ID));
                }
                if (!string.IsNullOrWhiteSpace(search.BU_Name))
                {
                    query = query.Where(m => m.BU_Name.Contains(search.BU_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.BUManager_Name))
                {
                    query = query.Where(m => m.BUManager_Name.Contains(search.BUManager_Name));
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
                if (search.Modified_By != null)
                {
                    query = query.Where(m => m.System_Users.User_NTID == search.Modified_By);
                }

                count = query.Count();

                return query.OrderBy(m => m.BU_ID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.BU_M_UID));

                count = 0;
                return query.OrderBy(o => o.BU_ID);
            }
        }

        public System_BU_M GetInfoByBU_ID(string BU_ID=null,string BU_Name = null)
        {
            System_BU_M bum = new System_BU_M();
            if (!string.IsNullOrWhiteSpace(BU_ID))
            {
                bum = DataContext.System_BU_M.Where(m => m.BU_ID == BU_ID).FirstOrDefault();
            }
            if (!string.IsNullOrWhiteSpace(BU_Name))
            {
                bum = DataContext.System_BU_M.Where(m => m.BU_Name == BU_Name).FirstOrDefault();
            }
            return bum;
        }

        public IQueryable<System_BU_M> GetValidBUMsByUserUId(int accountUId)
        {
            var now = DateTime.Now.Date;
            var query = from bum in DataContext.System_BU_M
                        join user_bu in DataContext.System_User_Business_Group on bum.BU_M_UID equals user_bu.BU_M_UID
                        where user_bu.Account_UID == accountUId
                            && ((user_bu.Begin_Date <= now && !user_bu.End_Date.HasValue)
                                || (user_bu.Begin_Date <= now && user_bu.End_Date >= now))
                        select bum;
            return query;
        }
    }


}
