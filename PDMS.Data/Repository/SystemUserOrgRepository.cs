using PDMS.Data.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data;
using PDMS.Model;
using PDMS.Common.Helpers;
using PDMS.Common.Enums;
using System.Data.Entity.SqlServer;

namespace PDMS.Data.Repository
{
    public class SystemUserOrgRepository : RepositoryBase<System_UserOrg>, ISystemUserOrgRepository
    {
        private Logger log = new Logger("SystemUserOrgRepository");
        public SystemUserOrgRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<UserOrgItem> QueryUserOrgs(UserOrgModelSearch search, Page page, out int count)
        {
            var query = from userOrg in DataContext.System_UserOrg.Include("System_Users")
                        join users in DataContext.System_Users on userOrg.Account_UID equals users.Account_UID
                        join modifyusers in DataContext.System_Users on userOrg.Modified_UID equals modifyusers.Account_UID
                        join Org in DataContext.System_Organization on userOrg.Organization_UID equals Org.Organization_UID
                        select new UserOrgItem
                        {
                            System_UserOrgUID = userOrg.System_UserOrgUID,
                            User_NTID = users.User_NTID,
                            User_Name = users.User_Name,
                            Organization_ID = Org.Organization_ID,
                            Organization_Name = Org.Organization_Name,
                            Begin_Date = userOrg.Begin_Date,
                            End_Date = userOrg.End_Date,
                            Account_UID = userOrg.Account_UID,
                            Modified_UID = userOrg.Modified_UID,
                            Modified_Date = userOrg.Modified_Date,
                            Modified_UserName = modifyusers.User_Name,
                            Modified_UserNTID = modifyusers.User_NTID
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

                if (!string.IsNullOrWhiteSpace(search.User_NTID))
                {
                    query = query.Where(p => p.User_NTID == search.User_NTID);

                }
                if (!string.IsNullOrWhiteSpace(search.User_Name))
                {
                    query = query.Where(p => p.User_Name.Contains(search.User_Name));

                }
                if (!string.IsNullOrWhiteSpace(search.Organization_ID))
                {
                    query = query.Where(p => p.Organization_ID == search.Organization_ID);

                }
                if (!string.IsNullOrWhiteSpace(search.Organization_Name))
                {
                    query = query.Where(p => p.Organization_Name.Contains(search.Organization_Name));
                }

                if (!string.IsNullOrWhiteSpace(search.Modified_By_NTID))
                {
                    query = query.Where(q => q.Modified_UserNTID == search.Modified_By_NTID);
                }

                count = query.Count();
                return query.OrderBy(o => o.User_NTID).ThenBy(o => o.Organization_ID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.System_UserOrgUID));

                count = 0;
                return query.OrderByDescending(o => o.Modified_Date);
            }
        }
        public IQueryable<UserOrgWithOrg> QueryUserOrgsByAccountUID(int uuid)
        {
            var query = from userOrg in DataContext.System_UserOrg
                        join Org in DataContext.System_Organization on userOrg.Organization_UID equals Org.Organization_UID
                        join user in DataContext.System_Users on userOrg.Account_UID equals user.Account_UID
                        where user.Account_UID == uuid
                        select new UserOrgWithOrg
                        {
                            Organization_UID = Org.Organization_UID,
                            Organization_ID = Org.Organization_ID,
                            Organization_Name = Org.Organization_Name,
                            Org_Begin_Date = Org.Begin_Date,
                            Org_End_Date = Org.End_Date,
                            System_UserOrgUID = userOrg.System_UserOrgUID,
                            UserOrg_Begin_Date = userOrg.Begin_Date,
                            UserOrg_End_Date = (DateTime)userOrg.End_Date
                        };
            return query;
        }

        public List<string> GetUserOpTypes(int accountUId)
        {
            //获取组织关系是第二层的组织
            var strSql = @";WITH one AS
                        (
	                        --递归获取所有相关父子节点信息
	                        SELECT * FROM System_OrganizationBOM WHERE ChildOrg_UID=(SELECT TOP 1 Organization_UID FROM dbo.System_UserOrg WHERE Account_UID={0})
	                        UNION ALL
                            SELECT h.* FROM dbo.System_OrganizationBOM h JOIN one h1 ON h.ChildOrg_UID = h1.ParentOrg_UID
                        ),
                        two AS
                        (
	                        SELECT A.* FROM dbo.System_Organization A
	                        JOIN one
	                        ON one.ChildOrg_UID = A.Organization_UID
                        )
                        SELECT two.Organization_Name FROM two WHERE two.Organization_ID LIKE '2%'";
            strSql = string.Format(strSql, accountUId);
            var dbList = DataContext.Database.SqlQuery<string>(strSql).ToList();
            return dbList;
        }

        public string UpdateUserOrgInfo(SystemUserInfo1 info)
        {
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    
                    //先Delete掉当前用户所有组织信息，然后Insert所有组织信息
                    var org = from temp in DataContext.System_UserOrg
                              where (temp.Account_UID == info.Account_UID)
                              select temp;
                    DataContext.System_UserOrg.RemoveRange(org);
                    //新增当前所有组织信息

                    if (info.Org_FuncPlant != null)
                    {
                        //先Delete掉用户所在功能厂信息
                        var userFunc = from temp in DataContext.System_User_FunPlant
                                       where
                                           (temp.Account_UID == info.Account_UID)
                                       select temp;
                        DataContext.System_User_FunPlant.RemoveRange(userFunc);
                        foreach (var func in info.Org_FuncPlant)
                        {
                            int result = 0;
                            int intfunc = 0;
                            int intpp = 0;
                            try
                            {
                                result = int.Parse(func);
                                intpp = int.Parse(info.Org_PP);
                                intfunc = int.Parse(func);
                            }
                            catch
                            {
                                try
                                {
                                    result = int.Parse(info.Org_PP);
                                    intpp = int.Parse(info.Org_PP);
                                }
                                catch
                                {
                                    result = int.Parse(info.Org_OP);
                                }
                            }
                            var insertOrg = new System_UserOrg
                            {
                                Account_UID = info.Account_UID,
                                Organization_UID = result,
                                Modified_Date = DateTime.Now,
                                Begin_Date = DateTime.Now,
                                Modified_UID = info.Modified_UID,
                                Plant_OrganizationUID = GetOrgUidByName(info.Org_CTU),
                                OPType_OrganizationUID = int.Parse(info.Org_OP)
                            };
                            if (intpp != 0)
                            {
                                insertOrg. Department_OrganizationUID = intpp;
                            }
                            if (intfunc != 0)
                            {
                                insertOrg.Funplant_OrganizationUID = intfunc;
                            }
                            DataContext.System_UserOrg.Add(insertOrg);

                            //添加System_User_FunPlant数据

                            ///通过orgid获取func功能厂UID



                            //var funcUid = GetOrgUidByID(func);
                            //var insertUserFunc = new System_User_FunPlant()
                            //{
                            //    Account_UID = info.Account_UID,
                            //    System_FunPlant_UID = funcUid,
                            //    Modified_Date = DateTime.Now,
                            //    Begin_Date = DateTime.Now,
                            //    Modified_UID = info.Modified_UID
                            //};
                            //DataContext.System_User_FunPlant.Add(insertUserFunc);
                        }
                    }
                    else
                    {
                        var insertOrg = new System_UserOrg();
                        insertOrg.Account_UID = info.Account_UID;
                        insertOrg.Modified_Date = DateTime.Now;
                        insertOrg.Begin_Date = DateTime.Now;
                        insertOrg.Modified_UID = info.Modified_UID;
                        if (!(info.Org_PP == null || info.Org_PP == "Nothing"))
                        {
                            insertOrg.Organization_UID = int.Parse(info.Org_PP);
                            insertOrg.Department_OrganizationUID = int.Parse(info.Org_PP);
                        }
                        if (!(info.Org_OP == null || info.Org_OP == "Nothing"))
                        {
                            insertOrg.Organization_UID = int.Parse(info.Org_OP);
                            insertOrg.OPType_OrganizationUID = int.Parse(info.Org_OP);
                        }
                        if (info.Org_CTU != null && info.Org_CTU != "Nothing")
                        {
                            insertOrg.Organization_UID = GetOrgUidByName(info.Org_CTU);
                            insertOrg.Plant_OrganizationUID = GetOrgUidByName(info.Org_CTU);
                        }
                        else
                        {
                            return "SUCCESS";
                        }
                        DataContext.System_UserOrg.Add(insertOrg);
                    }
                    DataContext.SaveChanges();
                    trans.Commit();


                  

                    return "SUCCESS";
                }
                catch (Exception e)
                {

                    trans.Rollback();
                    return "False" + e.ToString();
                }
            }
        }

        public int GetOrgUidByID(string OrgID)
        {
            var result = from temp in DataContext.System_Organization
                         where (temp.Organization_ID == OrgID)
                         select temp.Organization_UID;
            return result.First();
        }

        public int GetOrgUidByName(string Org)
        {
            var result = from temp in DataContext.System_Organization
                         where (temp.Organization_Name == Org)
                         select temp.Organization_UID;
            return result.First();
        }

        public int getfunUidByID(string orgID)
        {
            int uid = 0;
            if(!int.TryParse(orgID,out uid))
            {
                return 0;
            }
            var result = from func in DataContext.System_Function_Plant
                         where func.FunPlant_OrganizationUID == uid
                         select func.System_FunPlant_UID;
            return result.FirstOrDefault();
        }

        public int GetOrgUid(string OrgName)
        {
            var result = from temp in DataContext.System_Organization
                         where (temp.Organization_Name == OrgName)
                         select temp.Organization_UID;
            return result.First();
        }

        public int GetPPOrPEOrgUid(string OPType, string OrgName)
        {
            var result = from temp in DataContext.System_OrganizationBOM
                         where
                             (temp.System_Organization.Organization_Name == OPType &&
                              temp.System_Organization1.Organization_Name == OrgName)
                         select temp.ChildOrg_UID;
            return result.FirstOrDefault();
        }

        public List<System_Organization> GetChildFunPlant(int Organization_UID)
        {
            string sql = @";WITH one AS
                        (
	                        --递归获取所有相关父子节点信息
	                        SELECT * FROM System_OrganizationBOM WHERE ParentOrg_UID={0}
	                        UNION ALL
                            SELECT h.* FROM dbo.System_OrganizationBOM h JOIN one h1 ON h.ParentOrg_UID = h1.ChildOrg_UID
                        ),
                        two AS
                        (
	                        SELECT A.* FROM dbo.System_Organization A
	                        JOIN one
	                        ON one.ChildOrg_UID = A.Organization_UID
                        )
                        SELECT two.* FROM two WHERE two.Organization_ID LIKE '4%'";
            sql = string.Format(sql, Organization_UID);
            var list = DataContext.Database.SqlQuery<System_Organization>(sql).ToList();
            return list;
        }

        public string InsertUserDataToMiddleTable(int uids)
        {
            //根据search对象获取masterUID
            try
            {

                var User_UID = new SqlParameter("uids", uids.ToString());
                //var Product_UID = new SqlParameter("Product_UID", search.Product_UID);
                DataContext.Database.ExecuteSqlCommand("usp_InsertUserDataToMiddleTable @uids", User_UID);
                //IEnumerable<SPReturnMessage> result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("usp_InsertUserDataToMiddleTable @uids",  User_UID).ToArray();
                DataContext.Database.ExecuteSqlCommand("[Etransfer_Prod].[dbo].DownloadPDMSUserMaster");
                //IEnumerable<SPReturnMessage> result1 = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<SPReturnMessage>("DownloadPDMSUserMaster").ToArray();
                //if (result1.Count() > 0)
                //{
                //    return result1.ToList()[0].Message;
                //}
                //else {
                return "Success";
                //}
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return "Fail";
            }
        }
    }
    public interface ISystemUserOrgRepository : IRepository<System_UserOrg>
    {
        IQueryable<UserOrgItem> QueryUserOrgs(UserOrgModelSearch search, Page page, out int count);
        IQueryable<UserOrgWithOrg> QueryUserOrgsByAccountUID(int uuid);
        List<string> GetUserOpTypes(int accountUId);
        string UpdateUserOrgInfo(SystemUserInfo1 info);
        List<System_Organization> GetChildFunPlant(int Organization_UID);
        int GetOrgUidByName(string Org);
        string InsertUserDataToMiddleTable(int uids);
    }
}
