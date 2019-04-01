using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System.Linq;
using System;
using System.Data.Entity.SqlServer;
using PDMS.Common.Enums;
using System.Collections.Generic;

namespace PDMS.Data.Repository
{
    public class SystemOrgBomRepository : RepositoryBase<System_OrganizationBOM>, ISystemOrgBomRepository
    {
        public SystemOrgBomRepository(IDatabaseFactory databaseFactory)
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
        public IQueryable<SystemOrgAndBomDTO> QueryOrgBoms(OrgBomModelSearch search, Page page, out int count)
        {

            var query = from OrgBom in DataContext.System_OrganizationBOM.Include("System_Users")
                        join childOrg in DataContext.System_Organization on OrgBom.ChildOrg_UID equals childOrg.Organization_UID
                        join parentOrg in DataContext.System_Organization on OrgBom.ParentOrg_UID equals parentOrg.Organization_UID into g1
                        from g2 in g1.DefaultIfEmpty()
                        select new SystemOrgAndBomDTO
                        {
                            OrganizationBOM_UID = OrgBom.OrganizationBOM_UID,
                            ParentOrg_ID = g2.Organization_ID ?? string.Empty,
                            Parent_Organization_Name = g2.Organization_Name ?? string.Empty,
                            ChildOrg_ID = childOrg.Organization_ID,
                            Child_Organization_Name = childOrg.Organization_Name,
                            Begin_Date = OrgBom.Begin_Date,
                            End_Date = OrgBom.End_Date,
                            Order_Index = OrgBom.Order_Index,
                            Modified_Date = OrgBom.Modified_Date,
                            Modified_UserName = OrgBom.System_Users.User_Name,
                            Modified_UserNTID = OrgBom.System_Users.User_NTID
                        };

            if (string.IsNullOrEmpty(search.ExportUIds))
            {
                if (!string.IsNullOrWhiteSpace(search.ParentOrg_ID))
                {
                    query = query.Where(p => p.ParentOrg_ID == search.ParentOrg_ID);
                }
                if (!string.IsNullOrWhiteSpace(search.ParentOrg_Name))
                {
                    query = query.Where(p => p.Parent_Organization_Name.Contains(search.ParentOrg_Name));
                }

                if (!string.IsNullOrWhiteSpace(search.ChildOrg_ID))
                {
                    query = query.Where(p => p.ChildOrg_ID == search.ChildOrg_ID);
                }
                if (!string.IsNullOrWhiteSpace(search.ChildOrg_Name))
                {
                    query = query.Where(p => p.Child_Organization_Name.Contains(search.ChildOrg_Name));
                }

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

                if (!string.IsNullOrWhiteSpace(search.Modified_By_NTID))
                {
                    query = query.Where(p => p.Modified_UserNTID == search.Modified_By_NTID);
                }

                count = query.Count();
                return query.OrderBy(o => o.ParentOrg_ID).ThenBy(o => o.Order_Index).ThenBy(o => o.ChildOrg_ID).GetPage(page);
            }
            else
            {
                //for export data
                var array = Array.ConvertAll(search.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.OrganizationBOM_UID));

                count = 0;
                return query.OrderBy(o => o.ParentOrg_ID).ThenBy(o => o.Order_Index).ThenBy(o => o.ChildOrg_ID);
            }
        }

        public SystemOrgAndBomDTO QueryOrgBom(int uid)
        {
            var query = from OrgBom in DataContext.System_OrganizationBOM.Include("System_Users")
                        join childOrg in DataContext.System_Organization on OrgBom.ChildOrg_UID equals childOrg.Organization_UID
                        join parentOrg in DataContext.System_Organization on OrgBom.ParentOrg_UID equals parentOrg.Organization_UID into g1
                        from g2 in g1.DefaultIfEmpty()
                        where OrgBom.OrganizationBOM_UID == uid
                        select new SystemOrgAndBomDTO
                        {
                            OrganizationBOM_UID = OrgBom.OrganizationBOM_UID,
                            ParentOrg_UID=g2.Organization_UID,
                            ParentOrg_ID = g2.Organization_ID ?? string.Empty,
                            Parent_Organization_Name = g2.Organization_Name ?? string.Empty,
                            ParentOrg_BeginDate = g2.Begin_Date,
                            ParentOrg_EndDate= g2.End_Date, 
                            ChildOrg_UID = childOrg.Organization_UID,                          
                            ChildOrg_ID = childOrg.Organization_ID,
                            ChildOrg_BeginDate=childOrg.Begin_Date,
                            ChildOrg_EndDate =childOrg.End_Date,
                            Child_Organization_Name = childOrg.Organization_Name,
                            Begin_Date = OrgBom.Begin_Date,
                            End_Date = OrgBom.End_Date,
                            Order_Index = OrgBom.Order_Index,
                            Modified_Date = OrgBom.Modified_Date,
                            Modified_UserName = OrgBom.System_Users.User_Name,
                            Modified_UserNTID = OrgBom.System_Users.User_NTID
                        };
            return query.FirstOrDefault();
        }

        public  string GetFatherOrgID(int? childUid = null)
        {
            var query = from org in DataContext.System_Organization
                        join orgBom in DataContext.System_OrganizationBOM on org.Organization_UID equals orgBom.ParentOrg_UID
                        where orgBom.ChildOrg_UID == childUid
                        select org.Organization_ID;
            return query.FirstOrDefault();
        }

        public List<SystemOrgAndBomDTO> GetChildOrgID(int? fatherUid = null)
        {
            var query = from org in DataContext.System_Organization
                        join orgBom in DataContext.System_OrganizationBOM on org.Organization_UID equals orgBom.ChildOrg_UID
                        where orgBom.ParentOrg_UID == fatherUid
                        select new SystemOrgAndBomDTO
                        {
                            ChildOrg_UID=org.Organization_UID,
                            Child_Organization_Name=org.Organization_Name
                        };
            return query.ToList();
        }

        public DateTime? GetMaxEnddate4Org(int orgUId)
        {
            var query = from OrgBom in DataContext.System_OrganizationBOM
                        where OrgBom.ParentOrg_UID == orgUId || OrgBom.ChildOrg_UID == orgUId
                        orderby OrgBom.End_Date descending
                        select OrgBom.End_Date;

            var queryUserOrg = from userOrg in DataContext.System_UserOrg
                               where userOrg.Organization_UID == orgUId
                               orderby userOrg.End_Date descending
                               select userOrg.End_Date;

            var maxOrgBomEnddate = query.FirstOrDefault();
            var maxUserOrgEnddate = queryUserOrg.FirstOrDefault();

            DateTime? result = maxOrgBomEnddate;
            if (result==null)
            {
                result = maxUserOrgEnddate;
            }
            else
            {
                if (maxUserOrgEnddate!=null)
                {
                    result = maxUserOrgEnddate > maxOrgBomEnddate ? maxUserOrgEnddate : maxOrgBomEnddate;
                }
            }

            return result;
        }
    }
    public interface ISystemOrgBomRepository : IRepository<System_OrganizationBOM>
    {
        /// <summary>
        /// Query system OrgBom and order by OrgBom asc
        /// </summary>
        /// <param name="search">search model</param>
        /// <param name="page">page info</param>
        /// <param name="count">number of total records</param>
        /// <returns>paged records</returns>
        IQueryable<SystemOrgAndBomDTO> QueryOrgBoms(OrgBomModelSearch search, Page page, out int count);
        string GetFatherOrgID(int? childUid = null);
        SystemOrgAndBomDTO QueryOrgBom(int uid);

        DateTime? GetMaxEnddate4Org(int orgUId);
        List<SystemOrgAndBomDTO> GetChildOrgID(int? fatherUid = null);
    }
}
