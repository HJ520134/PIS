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
    public interface IVendorInfoRepository : IRepository<Vendor_Info>
    {
        IQueryable<VendorInfoDTO> GetInfo(VendorInfoDTO searchModel, Page page, out int totalcount);
        IQueryable<VendorInfoDTO> QueryVenderInfo(VendorInfoDTO search);
        List<SystemOrgDTO> GetOrgByParant(int Parant_UID, int type);
        string InsertItem(List<VendorInfoDTO> dtolist);
        List<VendorInfoDTO> DoExportFunction(string uids);
        List<OrgBomDTO> GetAllOrgBom();
    }
    public class VendorInfoRepository : RepositoryBase<Vendor_Info>, IVendorInfoRepository
    {
        public VendorInfoRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<VendorInfoDTO> GetInfo(VendorInfoDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Vendor_Info
                                join plantorg in DataContext.System_Organization
                                on M.Plant_Organization_UID equals plantorg.Organization_UID
                                join bgorg in DataContext.System_Organization
                                on M.BG_Organization_UID equals bgorg.Organization_UID
                                join user1 in DataContext.System_Users 
                                on M.Created_UID equals user1.Account_UID
                                join user2 in DataContext.System_Users
                                on M.Modified_UID equals user2.Account_UID

                        select new VendorInfoDTO
                        {
                            Vendor_Info_UID = M.Vendor_Info_UID,
                            Plant_Organization_UID=M.Plant_Organization_UID,
                            BG_Organization_UID=M.BG_Organization_UID,
                            Plant=plantorg.Organization_Name,
                            BG=bgorg.Organization_Name,
                            Vendor_ID = M.Vendor_ID,
                            Vendor_Name = M.Vendor_Name,
                            Is_Enable = M.Is_Enable,
                            Creator=user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier=user2.User_Name,
                            Modified_Date=M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID==searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID!=null && searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID );
            if (!string.IsNullOrWhiteSpace(searchModel.Vendor_ID))
                query = query.Where(m => m.Vendor_ID.Contains(searchModel.Vendor_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Vendor_Name))
                query = query.Where(m => m.Vendor_Name.Contains(searchModel.Vendor_Name));
            if(searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable==searchModel.Is_Enable);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Vendor_Name).GetPage(page);
            return query;
        }
        public IQueryable<VendorInfoDTO> QueryVenderInfo(VendorInfoDTO search)
        {
            var query = from M in DataContext.Vendor_Info
                                join plantorg in DataContext.System_Organization
                                on M.Plant_Organization_UID equals plantorg.Organization_UID
                                join bgorg in DataContext.System_Organization
                                on M.BG_Organization_UID equals bgorg.Organization_UID
                                join user1 in DataContext.System_Users 
                                on M.Created_UID equals user1.Account_UID
                                join user2 in DataContext.System_Users
                                on M.Modified_UID equals user2.Account_UID

                        select new VendorInfoDTO
                        {
                            Vendor_Info_UID = M.Vendor_Info_UID,
                            Plant_Organization_UID=M.Plant_Organization_UID,
                            BG_Organization_UID=M.BG_Organization_UID,
                            Plant=plantorg.Organization_Name,
                            BG=bgorg.Organization_Name,
                            Vendor_ID = M.Vendor_ID,
                            Vendor_Name = M.Vendor_Name,
                            Is_Enable = M.Is_Enable,
                            Creator=user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier=user2.User_Name,
                            Modified_Date=M.Modified_Date
                        };

            if (search.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID==search.Plant_Organization_UID);
            if (search.BG_Organization_UID!=null && search.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == search.BG_Organization_UID );
            if (!string.IsNullOrWhiteSpace(search.Vendor_ID))
                query = query.Where(m => m.Vendor_ID.Contains(search.Vendor_ID));
            if (!string.IsNullOrWhiteSpace(search.Vendor_Name))
                query = query.Where(m => m.Vendor_Name.Contains(search.Vendor_Name));
            if(search.needSearchEnable)
                query = query.Where(m => m.Is_Enable==search.Is_Enable);
            
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Vendor_Name);
            return query;
        }
        public List<SystemOrgDTO> GetOrgByParant(int Parant_UID,int type)
        {
            var sqlStr = "";
            if (type==1)                           //取得一级子组织
            {
                sqlStr = @"SELECT t1.* FROM dbo.System_Organization t1 INNER JOIN 
                            dbo.System_OrganizationBOM t2 ON t1.Organization_UID=t2.ChildOrg_UID
                            WHERE t2.ParentOrg_UID={0} and Organization_Name<>'Support team'";
           }
            else if (type==2)                         //取得二级子组织
            {
                sqlStr = @"WITH one AS 
                            (SELECT * FROM dbo.System_OrganizationBOM WHERE ParentOrg_UID={0})
                            SELECT DISTINCT t2.* FROM one INNER JOIN dbo.System_OrganizationBOM t1 
                            ON one.ChildOrg_UID=t1.ParentOrg_UID
                            INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                            INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
							WHERE t3.Organization_Name='OP'";
            }

            sqlStr = string.Format(sqlStr, Parant_UID);
            var dbList = DataContext.Database.SqlQuery<SystemOrgDTO>(sqlStr).ToList();
            return dbList;
        }

        public string InsertItem(List<VendorInfoDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        string strBG_Organization_UID = "Null";
                        if (dtolist[i].BG_Organization_UID != null)
                            strBG_Organization_UID = dtolist[i].BG_Organization_UID.ToString();
                        var sql = string.Format(@"INSERT dbo.Vendor_Info VALUES ({0},{1},N'{2}',N'{3}',{4},N'{5}',N'{6}',N'{5}',N'{6}')",
                                        dtolist[i].Plant_Organization_UID,
                                        strBG_Organization_UID,
                                        dtolist[i].Vendor_ID,
                                        dtolist[i].Vendor_Name,
                                        dtolist[i].Is_Enable? 1:0,
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
        public List<VendorInfoDTO> DoExportFunction(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Vendor_Info
                        join plantorg in DataContext.System_Organization
                        on M.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join user1 in DataContext.System_Users
                        on M.Created_UID equals user1.Account_UID
                        join user2 in DataContext.System_Users
                        on M.Modified_UID equals user2.Account_UID
                        where uids.Contains("," + M.Vendor_Info_UID + ",")
                        select new VendorInfoDTO
                        {
                            Vendor_Info_UID = M.Vendor_Info_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            Vendor_ID = M.Vendor_ID,
                            Vendor_Name = M.Vendor_Name,
                            Is_Enable = M.Is_Enable,
                            Creator = user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = user2.User_Name,
                            Modified_Date = M.Modified_Date
                        };
            return query.ToList();
        }
        public List<OrgBomDTO> GetAllOrgBom()
        {
            var sqlStr = @"WITH one AS(
                                SELECT Organization_UID Plant_Organization_UID,Organization_Name Plant FROM dbo.System_Organization WHERE Organization_ID LIKE '1%'),
                                two AS (SELECT one.*,t2.Organization_UID BG_Organization_UID,t2.Organization_Name BG FROM one INNER JOIN dbo.System_OrganizationBOM t1
                                ON one.Plant_Organization_UID=t1.ParentOrg_UID
                                INNER JOIN dbo.System_Organization t2 ON t2.Organization_UID=t1.ChildOrg_UID),
                                three AS (SELECT two.*,t2.Organization_UID uid3,t2.Organization_Name unit FROM two INNER JOIN dbo.System_OrganizationBOM t1
                                ON two.BG_Organization_UID=t1.ParentOrg_UID INNER JOIN dbo.System_Organization t2
                                ON t2.Organization_UID=t1.ChildOrg_UID),
                                four AS (SELECT three.*,t2.Organization_Name Funplant,t2.Organization_UID Funplant_UID FROM three INNER JOIN dbo.System_OrganizationBOM t1
                                ON three.uid3=t1.ParentOrg_UID INNER JOIN dbo.System_Organization t2
                                ON t2.Organization_UID=t1.ChildOrg_UID)SELECT * FROM four  WHERE  four.unit='OP'";
               //只是去取OP下的组织结构
            var dbList = DataContext.Database.SqlQuery<OrgBomDTO>(sqlStr).ToList();
            return dbList;
        }
    }
}
