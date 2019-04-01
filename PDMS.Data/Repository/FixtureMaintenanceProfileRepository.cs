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
    public interface IFixtureMaintenanceProfileRepository : IRepository<Fixture_Maintenance_Profile>
    {
        IQueryable<FixtureMaintenanceProfileDTO> GetInfo(FixtureMaintenanceProfileDTO searchModel, Page page, out int totalcount);
        string InsertItem(List<FixtureMaintenanceProfileDTO> dtolist);
        FixtureMaintenanceProfileDTO GetByUId(int Fixture_Maintenance_Profile_UID);
        List<FixtureMaintenanceProfileDTO> DoExportFunction(string uids);
        List<FixtureMaintenanceProfileDTO> DoAllFMPExportFunction(FixtureMaintenanceProfileDTO searchModel);
    }
    public class FixtureMaintenanceProfileRepository : RepositoryBase<Fixture_Maintenance_Profile>, IFixtureMaintenanceProfileRepository
    {
        public FixtureMaintenanceProfileRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IQueryable<FixtureMaintenanceProfileDTO> GetInfo(FixtureMaintenanceProfileDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Fixture_Maintenance_Profile
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
                        join maintenanceplan in DataContext.Maintenance_Plan
                        on M.Maintenance_Plan_UID equals maintenanceplan.Maintenance_Plan_UID
                        select new FixtureMaintenanceProfileDTO
                        {
                            Fixture_Maintenance_Profile_UID=M.Fixture_Maintenance_Profile_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Fixture_NO=M.Fixture_NO,
                            Maintenance_Plan_UID=M.Maintenance_Plan_UID,
                            Maintenance_Type=maintenanceplan.Maintenance_Type,
                            Cycle_ID=maintenanceplan.Cycle_ID,
                            Is_Enable = M.Is_Enable,
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
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Type))
                query = query.Where(m => m.Maintenance_Type == searchModel.Maintenance_Type);
            if (searchModel.Maintenance_Plan_UID != 0)
                query = query.Where(m => m.Maintenance_Plan_UID == searchModel.Maintenance_Plan_UID);
            if(searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m=>m.Fixture_NO).GetPage(page);
            return query;
        }
        public string InsertItem(List<FixtureMaintenanceProfileDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        string strFunPlant_Organization_UID = "Null";
                        if (dtolist[i].FunPlant_Organization_UID!=null)
                            strFunPlant_Organization_UID = dtolist[i].FunPlant_Organization_UID.ToString();
                        var sql = string.Format(@"INSERT INTO  Fixture_Maintenance_Profile (
                                                Plant_Organization_UID
                                               ,BG_Organization_UID
                                               ,FunPlant_Organization_UID
                                               ,Fixture_NO
                                               ,Maintenance_Plan_UID
                                               ,Is_Enable
                                               ,Created_UID
                                               ,Created_Date
                                               ,Modified_UID
                                               ,Modified_Date)
                                               VALUES ({0},{1},{2},N'{3}',{4},{5},{6},N'{7}',{8},N'{9}')",
                         dtolist[i].Plant_Organization_UID,
                         dtolist[i].BG_Organization_UID,
                         strFunPlant_Organization_UID,
                         dtolist[i].Fixture_NO,
                         dtolist[i].Maintenance_Plan_UID,
                         dtolist[i].Is_Enable ? 1 : 0,
                         dtolist[i].Created_UID,
                         DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                         dtolist[i].Created_UID,
                         DateTime.Now.ToString(FormatConstants.DateTimeFormatString)
                         );

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

        public FixtureMaintenanceProfileDTO GetByUId(int Fixture_Maintenance_Profile_UID)
        {
            string sql = @"SELECT t1.*,t2.Maintenance_Type,t2.Cycle_ID FROM dbo.Fixture_Maintenance_Profile t1
                                    INNER JOIN dbo.Maintenance_Plan t2 ON t1.Maintenance_Plan_UID=t2.Maintenance_Plan_UID
                                    WHERE t1.Fixture_Maintenance_Profile_UID={0}";
            sql = string.Format(sql, Fixture_Maintenance_Profile_UID);
            var dblist = DataContext.Database.SqlQuery<FixtureMaintenanceProfileDTO>(sql).ToList();
            return dblist[0];
        }
        public List<FixtureMaintenanceProfileDTO> DoExportFunction(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Fixture_Maintenance_Profile
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
                        join maintenanceplan in DataContext.Maintenance_Plan
                        on M.Maintenance_Plan_UID equals maintenanceplan.Maintenance_Plan_UID
                        where uids.Contains("," + M.Fixture_Maintenance_Profile_UID + ",")
                        select new FixtureMaintenanceProfileDTO
                        {
                            Fixture_Maintenance_Profile_UID = M.Fixture_Maintenance_Profile_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Fixture_NO = M.Fixture_NO,
                            Maintenance_Plan_UID = M.Maintenance_Plan_UID,
                            Maintenance_Type = maintenanceplan.Maintenance_Type,
                            Cycle_ID = maintenanceplan.Cycle_ID,
                            Is_Enable = M.Is_Enable,
                            Creator = user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = user2.User_Name,
                            Modified_Date = M.Modified_Date
                        };
            return query.ToList();
        }

        public List<FixtureMaintenanceProfileDTO> DoAllFMPExportFunction(FixtureMaintenanceProfileDTO searchModel) {

            var query = from M in DataContext.Fixture_Maintenance_Profile
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
                        join maintenanceplan in DataContext.Maintenance_Plan
                        on M.Maintenance_Plan_UID equals maintenanceplan.Maintenance_Plan_UID
                        select new FixtureMaintenanceProfileDTO
                        {
                            Fixture_Maintenance_Profile_UID = M.Fixture_Maintenance_Profile_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Fixture_NO = M.Fixture_NO,
                            Maintenance_Plan_UID = M.Maintenance_Plan_UID,
                            Maintenance_Type = maintenanceplan.Maintenance_Type,
                            Cycle_ID = maintenanceplan.Cycle_ID,
                            Is_Enable = M.Is_Enable,
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
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Type))
                query = query.Where(m => m.Maintenance_Type == searchModel.Maintenance_Type);
            if (searchModel.Maintenance_Plan_UID != 0)
                query = query.Where(m => m.Maintenance_Plan_UID == searchModel.Maintenance_Plan_UID);
            if (searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

          
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Fixture_NO);
            return query.ToList();

        }
    }
}
