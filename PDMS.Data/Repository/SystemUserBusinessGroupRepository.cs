using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.Settings;


namespace PDMS.Data.Repository
{
    public interface ISystemUserBusinessGroupRepository : IRepository<System_User_Business_Group>
    {
        IQueryable<System_User_Business_Group> QueryUserBUSettings(UserBUSettingSearch search, Page page, out int count);

        List<BUAndBUDByUserNTID> QueryBUAndBUDByUserNTIDS(int System_User_BU_UID);

        System_BU_M QueryBUAndBUDSByBUIDS(string BU_ID);
    }

    public class SystemUserBusinessGroupRepository : RepositoryBase<System_User_Business_Group>, ISystemUserBusinessGroupRepository
    {
        public SystemUserBusinessGroupRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IQueryable<System_User_Business_Group> QueryUserBUSettings(UserBUSettingSearch search, Page page, out int count)
        {
            var query = from A in DataContext.System_User_Business_Group.Include("System_BU_M").Include("System_BU_D").Include("System_Users")
                        select A;

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrEmpty(search.User_NTID))
                {
                    query = query.Where(m => m.System_Users.User_NTID == search.User_NTID);
                }
                if (!string.IsNullOrEmpty(search.User_Name))
                {
                    query = query.Where(m => m.System_Users.User_Name.Contains(search.User_Name));
                }
                if (!string.IsNullOrEmpty(search.BU_ID))
                {
                    query = query.Where(m => m.System_BU_M.BU_ID == search.BU_ID);
                }
                if (!string.IsNullOrEmpty(search.BU_Name))
                {
                    query = query.Where(m => m.System_BU_M.BU_Name.Contains(search.BU_Name));
                }
                if (!string.IsNullOrEmpty(search.BU_D_ID))
                {
                    query = query.Where(m => m.System_BU_D.BU_D_ID == search.BU_D_ID);
                }
                if (!string.IsNullOrEmpty(search.BU_D_Name))
                {
                    query = query.Where(m => m.System_BU_D.BU_D_Name.Contains(search.BU_D_Name));
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
                if (!string.IsNullOrEmpty(search.Modified_By))
                {
                    query = query.Where(m => m.System_Users1.User_NTID == search.User_NTID);
                }


                count = query.Count();

                query = query.OrderBy(m => m.System_Users.User_NTID).ThenBy(m => m.System_BU_M.BU_ID).ThenBy(m => m.System_BU_D.BU_D_ID).GetPage(page);
                return query;
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.System_User_BU_UID));

                count = 0;
                return query.OrderBy(m => m.System_Users.User_NTID).ThenBy(m => m.System_BU_M.BU_ID).ThenBy(m => m.System_BU_D.BU_D_ID);
            }
        }

        public List<BUAndBUDByUserNTID> QueryBUAndBUDByUserNTIDS(int System_User_BU_UID)
        {
            var userBusItem = DataContext.System_User_Business_Group.Find(System_User_BU_UID);

            string strSql = GetSqlStr();
            strSql = string.Format(strSql, userBusItem.Account_UID);
            var dbList = DataContext.Database.SqlQuery<BUAndBUDByUserNTID>(strSql).ToList();

            foreach (var dbItem in dbList)
            {
                if (dbItem.BU_D_UID != null)
                {
                    List<CustomBUD> cusList = new List<CustomBUD>();
                    var budList = DataContext.System_BU_D.Where(m => m.BU_M_UID == dbItem.BU_M_UID).ToList();
                    foreach (var budItem in budList)
                    {
                        CustomBUD newBUDItem = new CustomBUD();
                        newBUDItem.BU_D_UID = budItem.BU_D_UID;
                        newBUDItem.BU_D_ID = budItem.BU_D_ID;
                        newBUDItem.BU_D_Name = budItem.BU_D_Name;
                        cusList.Add(newBUDItem);
                    }
                    dbItem.BUDList = cusList;
                }
            }

            return dbList;
        }

        private string GetSqlStr()
        {
            string sql = string.Empty;
            sql = @"WITH
                            one AS
                            (
	                            SELECT * FROM dbo.System_BU_M
                            ),
                            two AS
                            (
	                            SELECT * FROM dbo.System_BU_D
                            ),
                            three AS
                            (
	                            SELECT * FROM dbo.System_Users
                            ),
                            four AS
                            (
	                            SELECT System_User_BU_UID,one.BU_M_UID,UBG.BU_D_UID,BU_ID,BU_Name,BU_D_ID,BU_D_Name,
	                            UBG.Begin_Date,UBG.End_Date,three.User_NTID,three.User_Name
	                            FROM dbo.System_User_Business_Group UBG
	                            JOIN one
	                            ON UBG.BU_M_UID = one.BU_M_UID
	                            LEFT JOIN two
	                            ON UBG.BU_D_UID = two.BU_D_UID
	                            JOIN three
	                            ON UBG.Account_UID = three.Account_UID
                                WHERE three.Account_UID={0}
                            )
                            SELECT * FROM four";
            return sql;
        }

        public System_BU_M QueryBUAndBUDSByBUIDS(string BU_ID)
        {
            var query = DataContext.System_BU_M.Include("System_BU_D").Where(m => m.BU_ID == BU_ID).FirstOrDefault();
            return query;
        }
    }
}
