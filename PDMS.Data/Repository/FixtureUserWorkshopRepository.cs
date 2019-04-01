using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IFixtureUserWorkshopRepository : IRepository<Fixture_User_Workshop>
    {
        IQueryable<FixtureUserWorkshopDTO> GetInfo(FixtureUserWorkshopDTO searchModel, Page page, out int totalcount);
        FixtureUserWorkshopDTO GetByUId(int Fixture_User_Workshop_UID);
        string InsertItem(List<FixtureUserWorkshopDTO> dtolist);
        List<WorkshopDTO> GetByNTID(int Account_UID);
        List<FixtureUserWorkshopDTO> DoExportFunction(string uids);
        List<FixtureUserWorkshopDTO> DoAllFUWExportFunction(FixtureUserWorkshopDTO searchModel);
        List<SystemUserDTO> GetUserByOp(int BG_Organization_UID, int FunPlant_Organization_UID);
        List<SystemUserDTO> GetUserByOpAPILY(int BG_Organization_UID, int FunPlant_Organization_UID);

    }
    public class FixtureUserWorkshopRepository : RepositoryBase<Fixture_User_Workshop>, IFixtureUserWorkshopRepository
    {
        public FixtureUserWorkshopRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<FixtureUserWorkshopDTO> GetInfo(FixtureUserWorkshopDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Fixture_User_Workshop
                        join workshop in DataContext.Workshop
                        on M.Workshop_UID equals workshop.Workshop_UID
                        join plantorg in DataContext.System_Organization
                        on workshop.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on workshop.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on workshop.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        join users in DataContext.System_Users
                        on M.Account_UID equals users.Account_UID
                        join users1 in DataContext.System_Users
                        on M.Created_UID equals users1.Account_UID
                        join users2 in DataContext.System_Users
                        on M.Modified_UID equals users2.Account_UID
                        select new FixtureUserWorkshopDTO
                        {
                            Fixture_User_Workshop_UID = M.Fixture_User_Workshop_UID,
                            Plant_Organization_UID = workshop.Plant_Organization_UID,
                            BG_Organization_UID = workshop.BG_Organization_UID,
                            FunPlant_Organization_UID = workshop.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = funplantorg.Organization_Name,
                            Workshop_UID = M.Workshop_UID,
                            Workshop_ID = workshop.Workshop_ID,
                            Account_UID = users.Account_UID,
                            User_NTID = users.User_NTID,
                            User_Name = users.User_Name,
                            Creator = users1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = users2.User_Name,
                            Modified_Date = M.Modified_Date,
                            Is_Enable = M.Is_Enable,
                            Workshop_Name = workshop.Workshop_Name
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Account_UID != 0)
                query = query.Where(m => m.Account_UID == searchModel.Account_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Workshop_ID))
                query = query.Where(m => m.Workshop_ID.Contains(searchModel.Workshop_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Workshop_Name))
                query = query.Where(m => m.Workshop_Name.Contains(searchModel.Workshop_Name));
            if (searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Workshop_ID).GetPage(page);
            return query;
        }
        public FixtureUserWorkshopDTO GetByUId(int Fixture_User_Workshop_UID)
        {
            string sql = @"SELECT t1.*,t2.User_NTID,t2.User_Name,t3.Workshop_ID,t3.Plant_Organization_UID,
                                    t3.BG_Organization_UID,t3.FunPlant_Organization_UID FROM dbo.Fixture_User_Workshop t1
                                    INNER JOIN dbo.System_Users t2 ON t1.Account_UID=t2.Account_UID
                                    INNER JOIN dbo.Workshop t3 ON t1.Workshop_UID=t3.Workshop_UID 
                                    WHERE t1.Fixture_User_Workshop_UID={0}";
            sql = string.Format(sql, Fixture_User_Workshop_UID);
            var dblist = DataContext.Database.SqlQuery<FixtureUserWorkshopDTO>(sql).ToList();
            return dblist[0];
        }
        public string InsertItem(List<FixtureUserWorkshopDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        var sql = string.Format(@"INSERT dbo.Fixture_User_Workshop
                                                                            ( Account_UID ,
                                                                                Workshop_UID ,
                                                                                Is_Enable ,
                                                                                Created_UID ,
                                                                                Created_Date ,
                                                                                Modified_UID ,
                                                                                Modified_Date
                                                                            )
                                                                    VALUES  ( {0} , -- Account_UID - int
                                                                                {1}, -- Workshop_UID - int
                                                                                {2} , -- Is_Enable - bit
                                                                                {3} , -- Created_UID - int
                                                                                N'{4}' , -- Created_Date - datetime
                                                                                {3} , -- Modified_UID - int
                                                                                N'{4}'  -- Modified_Date - datetime
                                                                            )",
                                            dtolist[i].Account_UID,
                                            dtolist[i].Workshop_UID,
                                            dtolist[i].Is_Enable ? 1 : 0,
                                            dtolist[i].Created_UID,
                                            DateTime.Now.ToString(FormatConstants.DateTimeFormatString));

                        DataContext.Database.ExecuteSqlCommand(sql);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    result = "Error" + ex;
                }
                return result;
            }
        }
        public List<WorkshopDTO> GetByNTID(int Account_UID)
        {
            var sqlStr = @"WITH ONE AS(
                    SELECT ISNULL(T2.Plant_OrganizationUID,0) Plant_OrganizationUID,ISNULL(T2.OPType_OrganizationUID,0) OPType_OrganizationUID,
                    ISNULL(T2.Funplant_OrganizationUID,0) Funplant_OrganizationUID FROM dbo.System_Users T1
                    LEFT JOIN dbo.System_UserOrg T2 ON T1.Account_UID=T2.Account_UID WHERE T1.Account_UID={0})
                    SELECT * FROM dbo.Workshop T1 INNER JOIN ONE
                    ON (T1.Plant_Organization_UID=ONE.Plant_OrganizationUID OR ONE.Plant_OrganizationUID=0)
                    AND (T1.BG_Organization_UID=ONE.OPType_OrganizationUID OR ONE.OPType_OrganizationUID=0)
                    AND (T1.FunPlant_Organization_UID=ONE.Funplant_OrganizationUID OR ONE.Funplant_OrganizationUID=0)";

            sqlStr = string.Format(sqlStr, Account_UID);
            var dbList = DataContext.Database.SqlQuery<WorkshopDTO>(sqlStr).ToList();
            return dbList;
        }
        public List<SystemUserDTO> GetUserByOp(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var sqlStr = @"SELECT t1.Account_UID,t1.User_Name,t1.User_NTID FROM dbo.System_Users t1 
                                    LEFT JOIN dbo.System_UserOrg t2  ON t1.Account_UID = t2.Account_UID
                                    WHERE (t2.OPType_OrganizationUID ={0}
                                    OR t2.OPType_OrganizationUID IS NULL) ";
            if (FunPlant_Organization_UID != 0)
                sqlStr += " AND t2.Funplant_OrganizationUID=" + FunPlant_Organization_UID;
            sqlStr = string.Format(sqlStr, BG_Organization_UID);
            var dbList = DataContext.Database.SqlQuery<SystemUserDTO>(sqlStr).ToList();
            return dbList;
        }
        public List<SystemUserDTO> GetUserByOpAPILY(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var sqlStr = @"SELECT t1.Account_UID,t1.User_Name,t1.User_NTID FROM dbo.System_Users t1 
                                    LEFT JOIN dbo.System_UserOrg t2  ON t1.Account_UID = t2.Account_UID
                                    WHERE (t2.OPType_OrganizationUID ={0}
                                    OR t2.OPType_OrganizationUID IS NULL) ";
            //过滤领用者
            sqlStr += "  AND t1.Account_UID IN (  SELECT DISTINCT Account_UID FROM  Fixture_User_Workshop WHERE Is_Enable=1) ";
            if (FunPlant_Organization_UID != 0)
                sqlStr += " AND t2.Funplant_OrganizationUID=" + FunPlant_Organization_UID;
            sqlStr = string.Format(sqlStr, BG_Organization_UID);
            var dbList = DataContext.Database.SqlQuery<SystemUserDTO>(sqlStr).ToList();
            return dbList;
        }
        public List<FixtureUserWorkshopDTO> DoExportFunction(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Fixture_User_Workshop
                        join workshop in DataContext.Workshop
                        on M.Workshop_UID equals workshop.Workshop_UID
                        join plantorg in DataContext.System_Organization
                        on workshop.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on workshop.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on workshop.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        join users in DataContext.System_Users
                        on M.Account_UID equals users.Account_UID
                        join users1 in DataContext.System_Users
                        on M.Created_UID equals users1.Account_UID
                        join users2 in DataContext.System_Users
                        on M.Modified_UID equals users2.Account_UID
                        where uids.Contains("," + M.Fixture_User_Workshop_UID + ",")
                        select new FixtureUserWorkshopDTO
                        {
                            Fixture_User_Workshop_UID = M.Fixture_User_Workshop_UID,
                            Plant_Organization_UID = workshop.Plant_Organization_UID,
                            BG_Organization_UID = workshop.BG_Organization_UID,
                            FunPlant_Organization_UID = workshop.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = funplantorg.Organization_Name,
                            Workshop_UID = M.Workshop_UID,
                            Workshop_ID = workshop.Workshop_ID,
                            Account_UID = users.Account_UID,
                            User_NTID = users.User_NTID,
                            User_Name = users.User_Name,
                            Creator = users1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = users2.User_Name,
                            Modified_Date = M.Modified_Date
                        };
            return query.ToList();
        }
        public List<FixtureUserWorkshopDTO> DoAllFUWExportFunction(FixtureUserWorkshopDTO searchModel)
        {

            var query = from M in DataContext.Fixture_User_Workshop
                        join workshop in DataContext.Workshop
                        on M.Workshop_UID equals workshop.Workshop_UID
                        join plantorg in DataContext.System_Organization
                        on workshop.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on workshop.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on workshop.FunPlant_Organization_UID equals funplantorg.Organization_UID
                        join users in DataContext.System_Users
                        on M.Account_UID equals users.Account_UID
                        join users1 in DataContext.System_Users
                        on M.Created_UID equals users1.Account_UID
                        join users2 in DataContext.System_Users
                        on M.Modified_UID equals users2.Account_UID
                        select new FixtureUserWorkshopDTO
                        {
                            Fixture_User_Workshop_UID = M.Fixture_User_Workshop_UID,
                            Plant_Organization_UID = workshop.Plant_Organization_UID,
                            BG_Organization_UID = workshop.BG_Organization_UID,
                            FunPlant_Organization_UID = workshop.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = funplantorg.Organization_Name,
                            Workshop_UID = M.Workshop_UID,
                            Workshop_ID = workshop.Workshop_ID,
                            Account_UID = users.Account_UID,
                            User_NTID = users.User_NTID,
                            User_Name = users.User_Name,
                            Creator = users1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = users2.User_Name,
                            Modified_Date = M.Modified_Date,
                            Is_Enable = M.Is_Enable,
                            Workshop_Name = workshop.Workshop_Name
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Account_UID != 0)
                query = query.Where(m => m.Account_UID == searchModel.Account_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Workshop_ID))
                query = query.Where(m => m.Workshop_ID.Contains(searchModel.Workshop_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Workshop_Name))
                query = query.Where(m => m.Workshop_Name.Contains(searchModel.Workshop_Name));
            if (searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Workshop_ID);
            return query.ToList();

        }
    }
}
