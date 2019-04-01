using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public interface IFixture_Totake_MRepository: IRepository<Fixture_Totake_M>
    {
        IQueryable<Fixture_Totake_MDTO> GetInfo(Fixture_Totake_MDTO searchModel, Page page, out int totalcount);
        List<fixture> GetFixture(int Fixture_Totake_M_UID);
        Fixture_Totake_MDTO GetByUId(int Fixture_Totake_M_UID);
        List<Fixture_Totake_MDTO> DoExportFunction(string uids);
        List<Fixture_Totake_MDTO> DoAllFTExportFunction(Fixture_Totake_MDTO search);
        List<FixtureDTO> GetFixtureByFilters(int Account_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name, int status_uid);
        List<SystemUserDTO> GetUserByWorkshop(int Account_UID);
    }
    public class Fixture_Totake_MRepository : RepositoryBase<Fixture_Totake_M>, IFixture_Totake_MRepository
    {
        public Fixture_Totake_MRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<Fixture_Totake_MDTO> GetInfo(Fixture_Totake_MDTO searchModel, Page page, out int totalcount)
        {
            var query1 = from D in DataContext.Fixture_Totake_D
                         join M in DataContext.Fixture_M
                         on D.Fixture_M_UID equals M.Fixture_M_UID
                         join project in DataContext.System_Project
                         on M.Project_UID equals project.Project_UID into temp
                         from aa in temp.DefaultIfEmpty()
                         join vendor in DataContext.Vendor_Info
                         on M.Vendor_Info_UID equals vendor.Vendor_Info_UID into temp2
                         from bb in temp2.DefaultIfEmpty()
                         join productline in DataContext.Production_Line
            on M.Production_Line_UID equals productline.Production_Line_UID into temp3
            from cc in temp3.DefaultIfEmpty()
                                join process in DataContext.Process_Info
                                on cc.Process_Info_UID equals process.Process_Info_UID into temp4
                                from dd in temp4.DefaultIfEmpty()
                                join workstation in DataContext.WorkStation
                                on cc.Workstation_UID equals workstation.WorkStation_UID into temp5
                                from ee in temp5.DefaultIfEmpty()
                                join fixturemachine in DataContext.Fixture_Machine
                                on M.Fixture_Machine_UID equals fixturemachine.Fixture_Machine_UID into temp6
                                from ff in temp6.DefaultIfEmpty()
                         select new
                         {
                             Fixture_M_UID = M.Fixture_M_UID,
                             Process_ID = dd.Process_ID,
                             WorkStation_ID = ee.WorkStation_ID,
                             Machine_ID = ff.Machine_ID,
                             Fixture_Unique_ID = M.Fixture_Unique_ID,
                             Vendor_ID = bb.Vendor_ID,
                             Line_ID = cc.Line_ID,
                             ShortCode = M.ShortCode,
                             Fixture_Name = M.Fixture_Name
                         };
            if (!string.IsNullOrWhiteSpace(searchModel.Process_ID))
                query1 = query1.Where(m => m.Process_ID.Contains(searchModel.Process_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.WorkStation_ID))
                query1 = query1.Where(m => m.WorkStation_ID.Contains(searchModel.WorkStation_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query1 = query1.Where(m => m.Machine_ID.Contains(searchModel.Machine_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Unique_ID))
                query1 = query1.Where(m => m.Fixture_Unique_ID.Contains(searchModel.Fixture_Unique_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Vendor_ID))
                query1 = query1.Where(m => m.Vendor_ID.Contains(searchModel.Vendor_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Line_ID))
                query1 = query1.Where(m => m.Line_ID.Contains(searchModel.Line_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query1 = query1.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query1 = query1.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));

            var query2 = from Fixture in query1
                         join D in DataContext.Fixture_Totake_D
                         on Fixture.Fixture_M_UID equals D.Fixture_M_UID
                         group D by new
                         {
                             D.Fixture_Totake_M_UID
                         } into g
                         select new
                         {
                             Fixture_Totake_M_UID = g.Key.Fixture_Totake_M_UID
                         };
            var uids = query2.ToList();
            List<int> intuids = new List<int>();
            foreach (var item in uids)
            {
                intuids.Add(item.Fixture_Totake_M_UID);
            }

            var query = from M in DataContext.Fixture_Totake_M
                        join shipuser in DataContext.System_Users
                        on M.Shiper_UID equals shipuser.Account_UID
                        join plantorg in DataContext.System_Organization
                        on M.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID into temp
                        from aa in temp.DefaultIfEmpty()
                        join user1 in DataContext.System_Users
                        on M.Created_UID equals user1.Account_UID
                        join user2 in DataContext.System_Users
                        on M.Modified_UID equals user2.Account_UID
                        select new Fixture_Totake_MDTO
                        {
                            Fixture_Totake_M_UID = M.Fixture_Totake_M_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Totake_NO = M.Totake_NO,
                            Shiper = shipuser.User_Name,
                            Ship_Date = M.Ship_Date,
                            Totake_Number = M.Totake_Number,
                            Totake_Name = M.Totake_Name,
                            Totake_Date = M.Totake_Date,
                            Creator = user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = user2.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Totake_NO))
                query = query.Where(m => m.Totake_NO.Contains(searchModel.Totake_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Shiper))
                query = query.Where(m => m.Shiper.Contains(searchModel.Shiper));
            if (!string.IsNullOrWhiteSpace(searchModel.Totaker))
                query = query.Where(m => m.Totaker.Contains(searchModel.Totaker));
            if (!string.IsNullOrWhiteSpace(searchModel.Modifier))
                query = query.Where(m => m.Modifier.Contains(searchModel.Modifier));
            if (searchModel.Modified_Date_from.Year != 1)
                query = query.Where(m => m.Modified_Date >= searchModel.Modified_Date_from);
            if (searchModel.Modified_Date_to.Year != 1)
            {
                DateTime nextday = searchModel.Modified_Date_to.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextday);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Totake_NO).GetPage(page);
            return query;
        }

        public List<fixture> GetFixture(int Fixture_Totake_M_UID)
        {
            string sql = @"SELECT t2.Fixture_Totake_D_UID,t3.Fixture_M_UID,t3.ShortCode,t3.Fixture_Unique_ID,
                                isnull(t5.Process_ID,'')+'_'+isnull(t5.Process_Name,'') Process,isnull(t6.WorkStation_ID,'')+'_'+isnull(t6.WorkStation_Name,'') WorkStation,
                                isnull(t4.Line_ID,'')+'_'+isnull(t4.Line_Name,'') Line FROM dbo.Fixture_Totake_M t1
                                INNER JOIN dbo.Fixture_Totake_D t2 ON t1.Fixture_Totake_M_UID=t2.Fixture_Totake_M_UID
                                INNER JOIN dbo.Fixture_M t3 ON t2.Fixture_M_UID=t3.Fixture_M_UID
                                LEFT JOIN dbo.Production_Line t4 ON t3.Production_Line_UID=t4.Production_Line_UID
                                LEFT JOIN dbo.Process_Info t5 ON t4.Process_Info_UID=t5.Process_Info_UID
                                LEFT JOIN dbo.WorkStation t6 ON t4.Workstation_UID=t6.WorkStation_UID
                                WHERE t1.Fixture_Totake_M_UID={0}";
            sql = string.Format(sql, Fixture_Totake_M_UID);
            var dbList = DataContext.Database.SqlQuery<fixture>(sql).ToList();
            return dbList;
        }

        public Fixture_Totake_MDTO GetByUId(int Fixture_Totake_M_UID)
        {
            string sql = @"SELECT m.*,t1.Organization_Name Plant,t2.Organization_Name BG,t3.Organization_Name FunPlant,
                                    t4.User_Name Shiper,t5.User_Name Totaker
                                    FROM dbo.Fixture_Totake_M M
                                    INNER JOIN dbo.System_Organization t1
                                    ON m.Plant_Organization_UID=t1.Organization_UID
                                    INNER JOIN dbo.System_Organization t2 
                                    ON m.BG_Organization_UID=t2.Organization_UID
                                    LEFT JOIN dbo.System_Organization t3
                                    ON m.FunPlant_Organization_UID=t3.Organization_UID
                                    INNER JOIN dbo.System_Users t4 ON m.Shiper_UID=t4.Account_UID
                                    INNER JOIN dbo.System_Users t5 ON m.Totake_UID=t5.Account_UID
                                    WHERE M.Fixture_Totake_M_UID={0}";
            sql = string.Format(sql, Fixture_Totake_M_UID);
            var dbList = DataContext.Database.SqlQuery<Fixture_Totake_MDTO>(sql).ToList();
            return dbList[0];
        }
        public List<Fixture_Totake_MDTO> DoExportFunction(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Fixture_Totake_M
                        join shipuser in DataContext.System_Users
                        on M.Shiper_UID equals shipuser.Account_UID
                        join plantorg in DataContext.System_Organization
                        on M.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID into temp
                        from aa in temp.DefaultIfEmpty()
                        join user1 in DataContext.System_Users
                        on M.Created_UID equals user1.Account_UID
                        join user2 in DataContext.System_Users
                        on M.Modified_UID equals user2.Account_UID
                        where uids.Contains(","+M.Fixture_Totake_M_UID+",")
                        select new Fixture_Totake_MDTO
                        {
                            Fixture_Totake_M_UID = M.Fixture_Totake_M_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Totake_NO = M.Totake_NO,
                            Shiper = shipuser.User_Name,
                            Ship_Date = M.Ship_Date,
                            Totake_Date = M.Totake_Date,
                            Creator = user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = user2.User_Name,
                            Modified_Date = M.Modified_Date
                        };
            return query.ToList();
        }

        public List<Fixture_Totake_MDTO> DoAllFTExportFunction(Fixture_Totake_MDTO searchModel)
        {

            var query1 = from D in DataContext.Fixture_Totake_D
                         join M in DataContext.Fixture_M
                         on D.Fixture_M_UID equals M.Fixture_M_UID
                         join project in DataContext.System_Project
                         on M.Project_UID equals project.Project_UID into temp
                         from aa in temp.DefaultIfEmpty()
                         join vendor in DataContext.Vendor_Info
                         on M.Vendor_Info_UID equals vendor.Vendor_Info_UID into temp2
                         from bb in temp2.DefaultIfEmpty()
                         join productline in DataContext.Production_Line
            on M.Production_Line_UID equals productline.Production_Line_UID into temp3
                         from cc in temp3.DefaultIfEmpty()
                         join process in DataContext.Process_Info
                         on cc.Process_Info_UID equals process.Process_Info_UID into temp4
                         from dd in temp4.DefaultIfEmpty()
                         join workstation in DataContext.WorkStation
                         on cc.Workstation_UID equals workstation.WorkStation_UID into temp5
                         from ee in temp5.DefaultIfEmpty()
                         join fixturemachine in DataContext.Fixture_Machine
                         on M.Fixture_Machine_UID equals fixturemachine.Fixture_Machine_UID into temp6
                         from ff in temp6.DefaultIfEmpty()
                         select new
                         {
                             Fixture_M_UID = M.Fixture_M_UID,
                             Process_ID = dd.Process_ID,
                             WorkStation_ID = ee.WorkStation_ID,
                             Machine_ID = ff.Machine_ID,
                             Fixture_Unique_ID = M.Fixture_Unique_ID,
                             Vendor_ID = bb.Vendor_ID,
                             Line_ID = cc.Line_ID,
                             ShortCode = M.ShortCode,
                             Fixture_Name = M.Fixture_Name
                         };
            if (!string.IsNullOrWhiteSpace(searchModel.Process_ID))
                query1 = query1.Where(m => m.Process_ID.Contains(searchModel.Process_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.WorkStation_ID))
                query1 = query1.Where(m => m.WorkStation_ID.Contains(searchModel.WorkStation_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Machine_ID))
                query1 = query1.Where(m => m.Machine_ID.Contains(searchModel.Machine_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Unique_ID))
                query1 = query1.Where(m => m.Fixture_Unique_ID.Contains(searchModel.Fixture_Unique_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Vendor_ID))
                query1 = query1.Where(m => m.Vendor_ID.Contains(searchModel.Vendor_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.Line_ID))
                query1 = query1.Where(m => m.Line_ID.Contains(searchModel.Line_ID));
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query1 = query1.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query1 = query1.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));

            var query2 = from Fixture in query1
                         join D in DataContext.Fixture_Totake_D
                         on Fixture.Fixture_M_UID equals D.Fixture_M_UID
                         group D by new
                         {
                             D.Fixture_Totake_M_UID
                         } into g
                         select new
                         {
                             Fixture_Totake_M_UID = g.Key.Fixture_Totake_M_UID
                         };
            var uids = query2.ToList();
            List<int> intuids = new List<int>();
            foreach (var item in uids)
            {
                intuids.Add(item.Fixture_Totake_M_UID);
            }

            var query = from M in DataContext.Fixture_Totake_M
                        join shipuser in DataContext.System_Users
                        on M.Shiper_UID equals shipuser.Account_UID
                        join plantorg in DataContext.System_Organization
                        on M.Plant_Organization_UID equals plantorg.Organization_UID
                        join bgorg in DataContext.System_Organization
                        on M.BG_Organization_UID equals bgorg.Organization_UID
                        join funplantorg in DataContext.System_Organization
                        on M.FunPlant_Organization_UID equals funplantorg.Organization_UID into temp
                        from aa in temp.DefaultIfEmpty()
                        join user1 in DataContext.System_Users
                        on M.Created_UID equals user1.Account_UID
                        join user2 in DataContext.System_Users
                        on M.Modified_UID equals user2.Account_UID
                        select new Fixture_Totake_MDTO
                        {
                            Fixture_Totake_M_UID = M.Fixture_Totake_M_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Totake_NO = M.Totake_NO,
                            Shiper = shipuser.User_Name,
                            Ship_Date = M.Ship_Date,
                            Totake_Number = M.Totake_Number,
                            Totake_Name = M.Totake_Name,
                            Totake_Date = M.Totake_Date,
                            Creator = user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = user2.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Totake_NO))
                query = query.Where(m => m.Totake_NO.Contains(searchModel.Totake_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Shiper))
                query = query.Where(m => m.Shiper.Contains(searchModel.Shiper));
            if (!string.IsNullOrWhiteSpace(searchModel.Totaker))
                query = query.Where(m => m.Totaker.Contains(searchModel.Totaker));
            if (!string.IsNullOrWhiteSpace(searchModel.Modifier))
                query = query.Where(m => m.Modifier.Contains(searchModel.Modifier));
            if (searchModel.Modified_Date_from.Year != 1)
                query = query.Where(m => m.Modified_Date >= searchModel.Modified_Date_from);
            if (searchModel.Modified_Date_to.Year != 1)
            {
                DateTime nextday = searchModel.Modified_Date_to.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextday);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));

          
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Totake_NO);
            return query.ToList();

        }
        public List<FixtureDTO> GetFixtureByFilters(int Account_UID, string Line_ID, string Line_Name, string Machine_ID, string Machine_Name, string Process_ID, string Process_Name, int status_uid)
        {
            string sql = @"SELECT M.* FROM dbo.Fixture_M M
                            LEFT JOIN dbo.Production_Line t1 ON m.Production_Line_UID=t1.Production_Line_UID
                            LEFT JOIN dbo.Fixture_Machine t2 ON M.Fixture_Machine_UID=t2.Fixture_Machine_UID
                            JOIN dbo.Workshop t3 ON t1.Workshop_UID=t3.Workshop_UID
                            JOIN Process_Info t4 ON t1.Process_Info_UID=t4.Process_Info_UID
                            WHERE t1.Workshop_UID IN (SELECT Workshop_UID FROM 
                            dbo.Fixture_User_Workshop WHERE Account_UID=" + Account_UID + ") AND M.Status=" + status_uid;
            if (!string.IsNullOrWhiteSpace(Line_ID))
            {
                sql += " AND t1.Line_ID LIKE '%" + Line_ID + "%' ";
            }
            if (Line_Name != null && Line_Name != "")
                sql += " AND t1.Line_Name LIKE N'%" + Line_Name + "%' ";
            if (!string.IsNullOrWhiteSpace(Machine_ID))
            {
                sql += " AND t2.Machine_ID LIKE '%" + Machine_ID + "%'";
            }
            if (Machine_Name != null && Machine_Name != "")
                sql += " AND t2.Machine_Name LIKE N'%" + Machine_Name + "%'";
            if (!string.IsNullOrWhiteSpace(Process_ID))
            {
                sql += " AND t4.Process_ID LIKE '%" + Process_ID + "%'";
            }
            if (!string.IsNullOrWhiteSpace(Process_Name))
            {
                sql += " AND t4.Process_Name LIKE N'%" + Process_Name + "%'";
            }
            var dbList = DataContext.Database.SqlQuery<FixtureDTO>(sql).ToList();
            return dbList;
        }
        public List<SystemUserDTO> GetUserByWorkshop(int Account_UID)
        {
            string sql = @"SELECT DISTINCT t2.* FROM dbo.Fixture_User_Workshop t1
                                INNER JOIN dbo.System_Users t2 ON t1.Account_UID=t2.Account_UID WHERE Workshop_UID IN (
                                SELECT Workshop_UID FROM dbo.Fixture_User_Workshop WHERE Account_UID={0})";
            sql = string.Format(sql, Account_UID);
            var dbList = DataContext.Database.SqlQuery<SystemUserDTO>(sql).ToList();
            return dbList;
        }
    }
}
