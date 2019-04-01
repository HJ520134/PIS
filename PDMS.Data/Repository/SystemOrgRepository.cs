using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using PDMS.Common.Enums;
using PDMS.Common.Helpers;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace PDMS.Data.Repository
{
    public class SystemOrgRepository : RepositoryBase<System_Organization>, ISystemOrgRepository
    {
        private Logger log = new Logger("SystemOrgRepository");
        public SystemOrgRepository(IDatabaseFactory databaseFactory)
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
        public IQueryable<SystemOrgDTO> QueryOrgs(OrgModelSearch search, Page page, out int count)
        {
            var query = from Org in DataContext.System_Organization.Include("System_Users")
                        select new SystemOrgDTO
                        {
                            Organization_UID = Org.Organization_UID,
                            Organization_ID = Org.Organization_ID,
                            Organization_Name = Org.Organization_Name,
                            Organization_Desc = Org.Organization_Desc,
                            Begin_Date = Org.Begin_Date,
                            End_Date = Org.End_Date,
                            OrgManager_Name = Org.OrgManager_Name,
                            OrgManager_Tel = Org.OrgManager_Tel,
                            OrgManager_Email = Org.OrgManager_Email,
                            Cost_Center = Org.Cost_Center,
                            Modified_Date = Org.Modified_Date,
                            Modified_UserName = Org.System_Users.User_Name,
                            Modified_UserNTID = Org.System_Users.User_NTID
                        };

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                #region Query_Types

                if (search.query_types != null && search.Reference_Date != null)
                {
                    EnumValidity queryType = (EnumValidity)Enum.ToObject(typeof(EnumValidity), search.query_types);

                    switch (queryType)
                    {
                        case EnumValidity.Valid:
                            query = query.Where(p => p.Begin_Date <= search.Reference_Date && (p.End_Date >= search.Reference_Date || p.End_Date == null));
                            break;
                        case EnumValidity.Invalid:
                            query = query.Where(p => p.Begin_Date > search.Reference_Date || (p.End_Date < search.Reference_Date && p.End_Date != null));
                            break;
                        default:
                            break;
                    }

                }
                #endregion

                #region Modified_Date

                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_From) <= 0);
                }
                if (search.Modified_Date_End != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_End) >= 0);
                }
                #endregion

                if (!string.IsNullOrWhiteSpace(search.Organization_ID))
                {
                    query = query.Where(p => p.Organization_ID == search.Organization_ID);
                }
                if (!string.IsNullOrWhiteSpace(search.Organization_Name))
                {
                    query = query.Where(p => p.Organization_Name.Contains(search.Organization_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Organization_Desc))
                {
                    query = query.Where(p => p.Organization_Desc.Contains(search.Organization_Desc));
                }
                if (!string.IsNullOrWhiteSpace(search.OrgManager_Name))
                {
                    query = query.Where(p => p.OrgManager_Name.Contains(search.OrgManager_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Cost_Center))
                {
                    query = query.Where(p => p.Cost_Center == search.Cost_Center);
                }
                if (!string.IsNullOrWhiteSpace(search.Modified_By_NTID))
                {
                    query = query.Where(q => q.Modified_UserNTID == search.Modified_By_NTID);
                }
                count = query.Count();
                return query.OrderBy(o => o.Organization_ID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Organization_UID));

                count = 0;
                return query.OrderBy(o => o.Organization_ID);
            }
        }

        public IQueryable<System_Organization> GetValidOrgsByUserUId(int accountUId)
        {
            var now = DateTime.Now.Date;

            var query = from org in DataContext.System_Organization
                        join user_org in DataContext.System_UserOrg on org.Organization_UID equals user_org.Organization_UID
                        where user_org.Account_UID == accountUId
                            && ((user_org.Begin_Date <= now && !user_org.End_Date.HasValue)
                                || (user_org.Begin_Date <= now && user_org.End_Date >= now))
                        select org;

            return query;
        }

        public List<string> getFunPOrg(string Plant)
        {
            List<string> result = new List<string>();
            string sql = string.Format(@"
                                        IF EXISTS ( SELECT TOP 1
                                                            1
                                                    FROM    dbo.System_UserOrg SUR WITH ( NOLOCK )
                                                            INNER JOIN dbo.System_Organization SORG WITH ( NOLOCK ) 
                                                            ON SORG.Organization_UID = SUR.Organization_UID
                                                    WHERE   SUR.Account_UID = {0}
                                                            AND SORG.Organization_ID LIKE '2%' )
                                            BEGIN
                                                SELECT  SORG.Organization_Name
                                                FROM    dbo.System_UserOrg SUR WITH ( NOLOCK )
                                                        INNER JOIN dbo.System_Organization SORG WITH ( NOLOCK ) 
                                                        ON SORG.Organization_UID = SUR.Organization_UID
                                                WHERE   SUR.Account_UID = {0}
                                                        AND SORG.Organization_ID LIKE '2%'
                                            END
                                        ELSE
                                            BEGIN
                                                SELECT  SORG.Organization_Name
                                                FROM    dbo.System_Organization SORG WITH ( NOLOCK )
                                                WHERE   SORG.Organization_ID LIKE '2%'
                                            END
                                        ", Plant);

            var dbList = DataContext.Database.SqlQuery<string>(sql).ToList();

            result = dbList.ToList();
            return result;

        }


        public List<SystemOrg> GetOrgList()
        {
            //var sqlStr = @"SELECT DISTINCT Father_Org,Father_Org_ID,org.Organization_Name Child_Org,org.Organization_ID from
            //                (
            //                select Organization_UID,so.Organization_Name Father_Org,ChildOrg_UID ,so.Organization_ID Father_Org_ID from
            //                dbo.System_OrganizationBOM AS sob,dbo.System_Organization AS so
            //                WHERE sob.ParentOrg_UID=so.Organization_UID)m,dbo.System_Organization org
            //                WHERE m.ChildOrg_UID=org.Organization_UID";

            var sqlStr = @"SELECT DISTINCT Father_Org,m.Organization_UID as Father_Org_ID,org.Organization_Name Child_Org,org.Organization_UID,org.Organization_ID from
                            (
                            select Organization_UID,so.Organization_Name Father_Org,ChildOrg_UID ,so.Organization_ID Father_Org_ID from
                            dbo.System_OrganizationBOM AS sob,dbo.System_Organization AS so
                            WHERE sob.ParentOrg_UID=so.Organization_UID)m,dbo.System_Organization org
                            WHERE m.ChildOrg_UID=org.Organization_UID";
            sqlStr = string.Format(sqlStr);
            var dbList = DataContext.Database.SqlQuery<SystemOrg>(sqlStr).ToList();
            return dbList;
        }

        public List<SystemOrgDTO> GetOpTypeByPlant(int plantuid,int oporguid)
        {
            string sqlstr = "";
            if (oporguid != 0)
            {
                sqlstr = @"SELECT t.Organization_Name,t.Organization_UID from dbo.System_Organization t
                                    WHERE t.Organization_UID = {0}";
                sqlstr = string.Format(sqlstr, oporguid);
            }
            else
            {
                sqlstr = @"SELECT t1.Organization_Name,t1.Organization_UID FROM dbo.System_Organization t1 INNER JOIN 
                                            dbo.System_OrganizationBOM t2 ON t2.ChildOrg_UID = t1.Organization_UID
                                            WHERE t2.ParentOrg_UID={0}";
                sqlstr = string.Format(sqlstr, plantuid);
            }
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlstr).ToList();
            return dbList;
        }

        public List<SystemOrgDTO> GetPlants (int PlantOrgUid)
        {
            string strsql = "";
            if (PlantOrgUid == 0)
            {
                strsql = "SELECT * FROM dbo.System_Organization WHERE Organization_ID LIKE '1%'";
            }
            else
            {
                strsql = "SELECT * FROM dbo.System_Organization WHERE Organization_UID={0}";
                strsql = string.Format(strsql, PlantOrgUid);
            }
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(strsql).ToList();
            return dbList;
        }

        public  List<SystemOrgDTO> GetSystem_Organization()
        {
            string strsql = "";
            strsql = "SELECT * FROM dbo.System_Organization ";
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(strsql).ToList();
            return dbList;
        }
       public List<SystemOrgBomDTO> GetSystem_OrganizationBOM()
        {

            string strsql = "";
            strsql = "SELECT * FROM dbo.System_OrganizationBOM ";
            var dbList = DataContext.Database.SqlQuery<SystemOrgBomDTO>(strsql).ToList();
            return dbList;
        }


        public List<int> GetPlant(int orguid)
        {
            string sqlStr = @"SELECT  t1.Organization_UID FROM dbo.System_Organization t1 INNER JOIN dbo.System_OrganizationBOM
                                t2 ON t2.ParentOrg_UID = t1.Organization_UID WHERE t2.ChildOrg_UID={0}";
            sqlStr = string.Format(sqlStr, orguid);
            var dbList = DataContext.Database.SqlQuery<int>(sqlStr).ToList();
            return dbList;
        }

        public List<OrganiztionVM> QueryOrganzitionInfoByAccountID(int Account_UID)
        {
            List<OrganiztionVM> result = new List<OrganiztionVM>();
            try
            {
                string sql = string.Format(@"
SELECT  PlantORg.Organization_Name AS Plant ,
        org.Plant_OrganizationUID AS Plant_OrganizationUID ,
        OPTypeOrg.Organization_Name AS OPType ,
        ISNULL(org.OPType_OrganizationUID,0) AS OPType_OrganizationUID ,
        DepartmentORg.Organization_Name AS Department ,
        ISNULL(org.Department_OrganizationUID,0) AS Department_OrganizationUID ,
        FunPlantORg.Organization_Name AS Funplant ,
        ISNULL(org.Funplant_OrganizationUID,0) AS Funplant_OrganizationUID
FROM    dbo.System_UserOrg org WITH ( NOLOCK )
        LEFT JOIN dbo.System_Organization PlantORg WITH ( NOLOCK ) ON PlantORg.Organization_UID = org.Plant_OrganizationUID
        LEFT JOIN dbo.System_Organization OPTypeOrg WITH ( NOLOCK ) ON OPTypeOrg.Organization_UID = org.OPType_OrganizationUID
        LEFT JOIN dbo.System_Organization DepartmentORg WITH ( NOLOCK ) ON DepartmentORg.Organization_UID = org.Department_OrganizationUID
        LEFT JOIN dbo.System_Organization FunPlantORg WITH ( NOLOCK ) ON FunPlantORg.Organization_UID = org.Funplant_OrganizationUID
WHERE org.Account_UID={0}", Account_UID);

                var query = DataContext.Database.SqlQuery<OrganiztionVM>(sql).ToList();
                result = query;
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public List<OrgVM> QueryOrganzitionInfo(int Plant_OrganizationUID = 0, int OPType_OrganizationUID = 0, int Department_OrganizationUID = 0)
        {
            List<OrgVM> result = new List<OrgVM>();
            try
            {
                string sql = @"
SELECT  Org.Organization_Name as Name,
        Org.Organization_UID
FROM    dbo.System_Organization Org WITH(NOLOCK)
        INNER JOIN dbo.System_OrganizationBOM OrgBOM WITH(NOLOCK) ON OrgBOM.ChildOrg_UID = Org.Organization_UID
WHERE OrgBOM.ParentOrg_UID = {0}";

                if (Department_OrganizationUID!=0)
                {
                    sql = string.Format(sql, Department_OrganizationUID);
                }
                else if (OPType_OrganizationUID != 0)
                {
                    sql = string.Format(sql, OPType_OrganizationUID);
                }
                else if (Plant_OrganizationUID != 0)
                {
                    sql = string.Format(sql, Plant_OrganizationUID);
                }

                var query = DataContext.Database.SqlQuery<OrgVM>(sql).ToList();
                result = query;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }


        public List<SystemOrgDTO> GetChildByParentUID(int OrgUID)
        {
            string sql = @"SELECT * FROM dbo.System_Organization WHERE Organization_UID IN 
(SELECT ChildOrg_UID FROM dbo.System_OrganizationBOM WHERE ParentOrg_UID = {0})";
            sql = string.Format(sql, OrgUID);
            var list = DataContext.Database.SqlQuery<SystemOrgDTO>(sql).ToList();
            return list;
        }


        public System_Organization QueryOrgByName(string name)
        {
            var f = from c in DataContext.System_Organization
                    where c.Organization_Name == name
                    select c;
            return f.FirstOrDefault();
        }

    }

    public interface ISystemOrgRepository : IRepository<System_Organization>
    {
        /// <summary>
        /// Query system Org and order by Org asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns>paged records</returns>
        IQueryable<SystemOrgDTO> QueryOrgs(OrgModelSearch search, Page page, out int count);

        IQueryable<System_Organization> GetValidOrgsByUserUId(int accountUId);
        List<SystemOrg> GetOrgList();
        List<string> getFunPOrg(string Plant);
        List<OrganiztionVM> QueryOrganzitionInfoByAccountID(int Account_UId);
        List<OrgVM> QueryOrganzitionInfo(int Plant_OrganizationUID = 0, int OPType_OrganizationUID = 0, int Department_OrganizationUID = 0);
        List<SystemOrgDTO> GetPlants(int PlantOrgUid);
        List<SystemOrgDTO> GetOpTypeByPlant(int plantuid,int oporguid);
        List<int> GetPlant(int orguid);
        List<SystemOrgDTO> GetChildByParentUID(int OrgUID);
        System_Organization QueryOrgByName(string name);
        List<SystemOrgDTO> GetSystem_Organization();
        List<SystemOrgBomDTO> GetSystem_OrganizationBOM();
    }
};