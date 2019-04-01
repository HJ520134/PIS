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
    public interface IMaintenancePlanRepository  : IRepository<Maintenance_Plan>
    {
        IQueryable<MaintenancePlanDTO> GetInfo(MaintenancePlanDTO searchModel, Page page, out int totalcount);
        string InsertItem(List<MaintenancePlanDTO> dtolist);
        List<MaintenancePlanDTO> DoExportFunction(string uids);
        List<MaintenancePlanDTO> DoAllMPExportFunction(MaintenancePlanDTO searchModel);
        List<string> GetFixtureNoByFunPlant(int BG_Organization_UID,int FunPlant_Organization_UID);
        List<MaintenancePlanDTO> GetMaintenancePlanByFilters(int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type);
    }
    public class MaintenancePlanRepository : RepositoryBase<Maintenance_Plan>, IMaintenancePlanRepository
    {
        public MaintenancePlanRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IQueryable<MaintenancePlanDTO> GetInfo(MaintenancePlanDTO searchModel, Page page, out int totalcount)
        {
            var query = from M in DataContext.Maintenance_Plan
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
                        select new MaintenancePlanDTO
                        {
                            Maintenance_Plan_UID = M.Maintenance_Plan_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID=M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant=aa.Organization_Name,
                            Maintenance_Type = M.Maintenance_Type,
                            Cycle_ID = M.Cycle_ID,
                            Cycle_Interval = M.Cycle_Interval,
                            Cycle_Unit = M.Cycle_Unit,
                            Lead_Time=M.Lead_Time,
                            Start_Date=M.Start_Date,
                            Tolerance_Time=M.Tolerance_Time,
                            Last_Execution_Date=M.Last_Execution_Date,
                            Next_Execution_Date=M.Next_Execution_Date,
                            Is_Enable=M.Is_Enable,
                            Creator=user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = user2.User_Name,
                            Modified_Date = M.Modified_Date
                        };

            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if ( searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m=>m.FunPlant_Organization_UID==searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Type))
                query = query.Where(m => m.Maintenance_Type==searchModel.Maintenance_Type);
            if (!string.IsNullOrWhiteSpace(searchModel.Cycle_ID))
                query = query.Where(m => m.Cycle_ID.Contains(searchModel.Cycle_ID));
            if (searchModel.Cycle_Interval!=null)
                query = query.Where(m => m.Cycle_Interval == searchModel.Cycle_Interval);
            if (!string.IsNullOrWhiteSpace(searchModel.Cycle_Unit))
                query = query.Where(m => m.Cycle_Unit == searchModel.Cycle_Unit);
            if (searchModel.Lead_Time != null)
                query = query.Where(m => m.Lead_Time == searchModel.Lead_Time);
            if (searchModel.Start_Date_start.Year != 1)
                query = query.Where(m => m.Start_Date>= searchModel.Start_Date_start);
            if (searchModel.Start_Date_end.Year != 1)
            {
                DateTime nextday = searchModel.Start_Date_end.AddDays(1);
                query = query.Where(m => m.Start_Date < nextday);
            }

            if (searchModel.Last_Execution_Date_start.Year != 1)
                query = query.Where(m => m.Last_Execution_Date >= searchModel.Last_Execution_Date_start);
            if (searchModel.Last_Execution_Date_end.Year != 1)
            {
                DateTime nextday = searchModel.Last_Execution_Date_end.AddDays(1);
                query = query.Where(m => m.Last_Execution_Date < nextday);
            }

            if (searchModel.Next_Execution_Date_start.Year != 1)
                query = query.Where(m => m.Next_Execution_Date >= searchModel.Next_Execution_Date_start);
            if (searchModel.Next_Execution_Date_end.Year != 1)
            {
                DateTime nextday = searchModel.Next_Execution_Date_end.AddDays(1);
                query = query.Where(m => m.Next_Execution_Date < nextday);
            }

            if (searchModel.Tolerance_Time != null)
                query = query.Where(m => m.Tolerance_Time == searchModel.Tolerance_Time);
            if(searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);

            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).GetPage(page);
            return query;
        }
        public string InsertItem(List<MaintenancePlanDTO> dtolist)
        {
            string result = "";
            using (var trans = DataContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < dtolist.Count; i++)
                    {
                        string strFunPlant_Organization_UID = "Null";
                        if (dtolist[i].FunPlant_Organization_UID != null)
                            strFunPlant_Organization_UID = dtolist[i].FunPlant_Organization_UID.ToString();
                        var sql = string.Format(@"INSERT dbo.Maintenance_Plan VALUES ({0},{1},{2},N'{3}',N'{4}',{5},N'{6}',{7},N'{8}',{9},NULL,NULL,{10},{11},N'{12}',{11},N'{12}')",
                                        dtolist[i].Plant_Organization_UID,
                                        dtolist[i].BG_Organization_UID,
                                        strFunPlant_Organization_UID,
                                        dtolist[i].Maintenance_Type,
                                        dtolist[i].Cycle_ID,
                                        dtolist[i].Cycle_Interval,
                                        dtolist[i].Cycle_Unit,
                                        dtolist[i].Lead_Time,
                                        dtolist[i].Start_Date,
                                        dtolist[i].Tolerance_Time,
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
        public List<MaintenancePlanDTO> DoExportFunction(string uids)
        {
            uids = "," + uids + ",";
            var query = from M in DataContext.Maintenance_Plan
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
                        where uids.Contains("," + M.Maintenance_Plan_UID + ",")
                        select new MaintenancePlanDTO
                        {
                            Maintenance_Plan_UID = M.Maintenance_Plan_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Maintenance_Type = M.Maintenance_Type,
                            Cycle_ID = M.Cycle_ID,
                            Cycle_Interval = M.Cycle_Interval,
                            Cycle_Unit = M.Cycle_Unit,
                            Lead_Time = M.Lead_Time,
                            Start_Date = M.Start_Date,
                            Tolerance_Time = M.Tolerance_Time,
                            Last_Execution_Date = M.Last_Execution_Date,
                            Next_Execution_Date = M.Next_Execution_Date,
                            Is_Enable = M.Is_Enable,
                            Creator = user1.User_Name,
                            Created_Date = M.Created_Date,
                            Modifier = user2.User_Name,
                            Modified_Date = M.Modified_Date
                        };
            return query.ToList();
        }

        public List<MaintenancePlanDTO> DoAllMPExportFunction(MaintenancePlanDTO searchModel)
        {
            var query = from M in DataContext.Maintenance_Plan
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
                        select new MaintenancePlanDTO
                        {
                            Maintenance_Plan_UID = M.Maintenance_Plan_UID,
                            Plant_Organization_UID = M.Plant_Organization_UID,
                            BG_Organization_UID = M.BG_Organization_UID,
                            FunPlant_Organization_UID = M.FunPlant_Organization_UID,
                            Plant = plantorg.Organization_Name,
                            BG = bgorg.Organization_Name,
                            FunPlant = aa.Organization_Name,
                            Maintenance_Type = M.Maintenance_Type,
                            Cycle_ID = M.Cycle_ID,
                            Cycle_Interval = M.Cycle_Interval,
                            Cycle_Unit = M.Cycle_Unit,
                            Lead_Time = M.Lead_Time,
                            Start_Date = M.Start_Date,
                            Tolerance_Time = M.Tolerance_Time,
                            Last_Execution_Date = M.Last_Execution_Date,
                            Next_Execution_Date = M.Next_Execution_Date,
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
            if (!string.IsNullOrWhiteSpace(searchModel.Maintenance_Type))
                query = query.Where(m => m.Maintenance_Type == searchModel.Maintenance_Type);
            if (!string.IsNullOrWhiteSpace(searchModel.Cycle_ID))
                query = query.Where(m => m.Cycle_ID.Contains(searchModel.Cycle_ID));
            if (searchModel.Cycle_Interval != null)
                query = query.Where(m => m.Cycle_Interval == searchModel.Cycle_Interval);
            if (!string.IsNullOrWhiteSpace(searchModel.Cycle_Unit))
                query = query.Where(m => m.Cycle_Unit == searchModel.Cycle_Unit);
            if (searchModel.Lead_Time != null)
                query = query.Where(m => m.Lead_Time == searchModel.Lead_Time);
            if (searchModel.Start_Date_start.Year != 1)
                query = query.Where(m => m.Start_Date >= searchModel.Start_Date_start);
            if (searchModel.Start_Date_end.Year != 1)
            {
                DateTime nextday = searchModel.Start_Date_end.AddDays(1);
                query = query.Where(m => m.Start_Date < nextday);
            }

            if (searchModel.Last_Execution_Date_start.Year != 1)
                query = query.Where(m => m.Last_Execution_Date >= searchModel.Last_Execution_Date_start);
            if (searchModel.Last_Execution_Date_end.Year != 1)
            {
                DateTime nextday = searchModel.Last_Execution_Date_end.AddDays(1);
                query = query.Where(m => m.Last_Execution_Date < nextday);
            }

            if (searchModel.Next_Execution_Date_start.Year != 1)
                query = query.Where(m => m.Next_Execution_Date >= searchModel.Next_Execution_Date_start);
            if (searchModel.Next_Execution_Date_end.Year != 1)
            {
                DateTime nextday = searchModel.Next_Execution_Date_end.AddDays(1);
                query = query.Where(m => m.Next_Execution_Date < nextday);
            }

            if (searchModel.Tolerance_Time != null)
                query = query.Where(m => m.Tolerance_Time == searchModel.Tolerance_Time);
            if (searchModel.needSearchEnable)
                query = query.Where(m => m.Is_Enable == searchModel.Is_Enable);
       
            query = query.OrderByDescending(m => m.Modified_Date);
            return query.ToList();
        }
        public List<string> GetFixtureNoByFunPlant(int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            string sql = "";
            if (FunPlant_Organization_UID != 0)
            {
                sql = @"SELECT distinct Fixture_NO FROM  dbo.Fixture_M WHERE BG_Organization_UID={0}AND FunPlant_Organization_UID={1}";
                sql = string.Format(sql, BG_Organization_UID, FunPlant_Organization_UID);
            }else
            {
                sql = @"SELECT distinct Fixture_NO FROM  dbo.Fixture_M WHERE BG_Organization_UID={0}";
                sql = string.Format(sql, BG_Organization_UID);
            }

            var dblist = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dblist;
        }
        public List<MaintenancePlanDTO> GetMaintenancePlanByFilters(int BG_Organization_UID, int FunPlant_Organization_UID, string Maintenance_Type)
        {
            string sql = "SELECT * FROM dbo.Maintenance_Plan WHERE BG_Organization_UID="+BG_Organization_UID;
            if (FunPlant_Organization_UID != 0)
                sql += @" AND FunPlant_Organization_UID=" + FunPlant_Organization_UID;
            if (!string.IsNullOrWhiteSpace(Maintenance_Type))
                sql += @"AND Maintenance_Type='"+Maintenance_Type+"'";

            var dblist = DataContext.Database.SqlQuery<MaintenancePlanDTO>(sql).ToList();
            return dblist;
        }
    }
}
