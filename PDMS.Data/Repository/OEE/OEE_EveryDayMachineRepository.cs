using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System.Data.SqlClient;

namespace PDMS.Data.Repository
{
    public interface IOEE_EveryDayMachineRepository : IRepository<OEE_EveryDayMachine>
    {
        Dictionary<string, int> SetOEE_MachineDayOutput(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime);

        Dictionary<string, int> SetOEE_CNCMachineDayOutput(DateTime startTime, DateTime endTime);


        List<OEE_MachineInfo> getAllMachineByProject(int ProjectUID);
        List<OEE_EveryDayMachineDTO> QueryOEE_EveryDayMachine(OEE_ReprortSearchModel serchModel, Page page);
        List<OEE_EveryDayMachineDTO> QueryBuTemplateData(OEE_ReprortSearchModel serchModel, Page page);
        List<OEE_EveryDayMachineDTO> GetAllStationData(OEE_ReprortSearchModel serchModel, Page page);
        List<OEE_EveryDayMachineDTO> GetLineStaticData(OEE_ReprortSearchModel serchModel, Page page);
        OEE_MachineInfo GetMachineInfoByMachineNo(string MachineNo);
        List<OEE_EveryDayMachine> judgmentData(int Project_UID, string Date, int shiftID);

        List<OEE_EveryDayMachine> ALLjudgmentData(int Project_UID, string Date);
        Dictionary<string, int> GetFixtureNum(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime);
        Dictionary<string, double> GetActualCT(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime);

        List<LocalCNCInfo> GetCNCActualCT(string Plant,DateTime startTime, DateTime endTime);

        Dictionary<string, int> GetCTUFixtureNum(string CurrentDate);
        bool AddOEEDailyInfo(List<OEE_EveryDayMachineDTO> WIPHourOutputList);
        bool UpDateOEEDailyInfo(List<OEE_EveryDayMachine> WIPHourOutputList);
        List<ProPerCT> GetPorCT(string Plant, string MESCustomerName);
        void setDefectSums(string ProductDate, int shiftID, int BG_Organization_UID);

        List<LoacalMachineDefect> GetDefectInterval(DateTime startTime, DateTime endTime);
        //void setDowntimesSums(string ProductDate, int shiftID, List<OEE_DownRecordsDTO> lists);
        double GetPlanHour(int shiftID);
        double GetALLPlanHour(int shiftID);
        List<OEE_DownTimeCode> getAllDownCodeByProject(int ProjectUID);
        List<OEE_StationDefectCode> getAllErrorCodeByProject(int ProjectUID);
        List<OEE_DownRecordsDTO> getOEELocalDatas(string Plant, DateTime startTime, DateTime endTime);
        List<OEE_DefectCodeDailyNum> GetOEEDefectRord(int Project_UID, string Date, int shiftID, string TimeInterval);
        List<OEE_DefectCodeDailySum> GetOEEDefectSum(int Project_UID, string Date, int shiftID);
        List<OEE_MachineDailyDownRecord> GetOEEDownRocord(int Project_UID, string Date, int shiftID);
       void DeleteDownRocords(int Project_UID, string Date, int shiftID);
        DateTime GetLatestUpdateTime(int ProjectUID);
        string getStartTime(int shiftID);

        string GetLastUpdateTime(OEE_ReprortSearchModel serchMode);
        string GetStationLastUpdateTime(OEE_ReprortSearchModel serchMode);
        string GetRealLastUpdateTime(OEE_ReprortSearchModel serchMode);
        string GetLineLastUpdateTime(OEE_ReprortSearchModel serchMode);

        List<String> GetNoOffLineMachineList(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime);
        List<OEE_MachineStatusD> getOEEStatusDatas(string Plant, DateTime startTime, DateTime endTime);
        List<string> getLocalMachineStatus(string Plant);
        OEE_RealStatusReport QueryRealStatusReport(OEE_ReprortSearchModel serchModel);
    }
    public class OEE_EveryDayMachineRepository : RepositoryBase<OEE_EveryDayMachine>, IOEE_EveryDayMachineRepository
    {
        public OEE_EveryDayMachineRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        /// <summary>
        /// 获取上次最新更新时间
        /// </summary>
        /// <param name="shiftID"></param>
        /// <returns></returns>
        public DateTime GetLatestUpdateTime(int ProjectUID)
        {
            var query = from q in DataContext.OEE_EveryDayMachine
                        where q.OEE_MachineInfo.Project_UID == ProjectUID
                        orderby q.OEE_EveryDayMachine_UID descending
                        select q.UpdateTime;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取当前没有断网的所有机台
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="MESCustomerName"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
       public   List<String> GetNoOffLineMachineList(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime)
        {
            List<string> ResultList = new List<string>();

            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"
                
                       SELECT   MachineName 
                   
                        
            FROM     dbo.API_MachineStatus
            WHERE    EventTime
            BETWEEN  '{0}' AND '{1}' and MachineName <>'unknown'
            GROUP BY MachineName 
                 ;
 ", startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"));
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                ResultList.Add(read["MachineName"].ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return ResultList;
        }

        /// <summary>
        /// /获取时段信息
        /// </summary>
        /// <param name="ProductDate"></param>
        /// <param name="TimeInterval"></param>
        public List<LoacalMachineDefect> GetDefectInterval(DateTime startTime, DateTime endTime)
        {
            List<LoacalMachineDefect> ResultList = new List<LoacalMachineDefect>();

            string connStr = string.Empty;

            connStr = ChengduconnStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = string.Format(@"
                
            SELECT   Machine ,
                     DefectCode ,
                     COUNT(*)   DownTime    
            FROM     dbo.WP_WipDefect
            WHERE    LastUpdateDate
            BETWEEN  '{0}' AND '{1}' and Machine <>'unknown'
            GROUP BY Machine ,
                     DefectCode;
 ", startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"));
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                LoacalMachineDefect item = new LoacalMachineDefect();
                                item.MachineNo = read["Machine"].ToString();
                                item.DefectCode = read["DefectCode"].ToString();
                                item.DownTime = int.Parse(read["DownTime"].ToString());
                                ResultList.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }


            return ResultList;
        }


        /// <summary>
        /// 将当天的两小时的分类数据同步到汇总表
        /// </summary>
        /// <param name="ProductDate"></param>
        public void setDefectSums(string ProductDate, int shiftID, int BG_Organization_UID)
        {

            //先删掉当天当班次的数据，然后再插入新的数据。
            var sql = @"DELETE dbo.OEE_DefectCodeDailySum WHERE  ProductDate='{0}' AND ShiftTimeID={1}
                    INSERT INTO OEE_DefectCodeDailySum ( Plant_Organization_UID ,
                    BG_Organization_UID ,
                    FunPlant_Organization_UID ,
                    OEE_MachineInfo_UID ,
                    OEE_StationDefectCode_UID ,
                    DefectNum ,
                    ProductDate ,
                    ShiftTimeID )
            SELECT   Plant_Organization_UID ,
                     BG_Organization_UID ,
                     FunPlant_Organization_UID ,
                     OEE_MachineInfo_UID ,
                     OEE_StationDefectCode_UID ,
                     SUM(DefectNum) DefectNum ,
                     ProductDate ,
                     ShiftTimeID
            FROM     dbo.OEE_DefectCodeDailyNum
            WHERE    ProductDate = '{0}'
                     AND ShiftTimeID = {1}
                   AND BG_Organization_UID={2}
            GROUP BY OEE_MachineInfo_UID ,
                     OEE_StationDefectCode_UID ,
                     Plant_Organization_UID ,
                     BG_Organization_UID ,
                     FunPlant_Organization_UID ,
                     ProductDate ,
                     ShiftTimeID;";
            var SQLSrl = string.Format(sql, ProductDate, shiftID, BG_Organization_UID);
            DataContext.Database.ExecuteSqlCommand(SQLSrl);

        }


        private string WuxiconnStr = @"Server=CNWXIg0lsqlv01b\inst1;DataBase=MESLDB;uid=PISUser;pwd=PISuser123";

        private string ChengduconnStr = @"Server=CNCTUG0MLSQLV1A;DataBase=OEE;uid=pis;pwd=jabil@1234";

        private string HuizhouconnStr = @"Server=CNHUZM0DB01;DataBase=MESLDB;uid=Huzsystem01;pwd=Jabil";

        public List<OEE_EveryDayMachineDTO> QueryOEE_EveryDayMachine(OEE_ReprortSearchModel serchModel, Page page)
        {
            var query = from dayMachine in DataContext.OEE_EveryDayMachine
                        select new OEE_EveryDayMachineDTO
                        {
                            OEE_EveryDayMachine_UID = dayMachine.OEE_EveryDayMachine_UID,
                            Plant_Organization_UID = dayMachine.Plant_Organization_UID,
                            BG_Organization_UID = dayMachine.BG_Organization_UID,
                            FunPlant_Organization_UID = dayMachine.FunPlant_Organization_UID,
                            OEE_MachineInfo_UID = dayMachine.OEE_MachineInfo_UID,
                            FixtureNum = dayMachine.FixtureNum,
                            PORCT = dayMachine.PORCT,
                            ActualCT = dayMachine.ActualCT,
                            TotalAvailableHour = dayMachine.TotalAvailableHour,
                            OutPut = dayMachine.OutPut,
                            PlannedHour = dayMachine.PlannedHour,
                            ShiftTimeID = dayMachine.ShiftTimeID,
                            Product_Date = dayMachine.Product_Date,
                            UpdateTime = dayMachine.UpdateTime,
                            ResetTime = dayMachine.ResetTime,
                            Is_DownType = dayMachine.Is_DownType,
                            AbnormalDFCode = dayMachine.AbnormalDFCode,
                            OEEDashBoardTarget = dayMachine.OEE_MachineInfo.GL_Station.DashboardTarget,
                            MachineName = dayMachine.OEE_MachineInfo.MachineNo,
                        };

            if (serchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID);
            }

            if (serchModel.BG_Organization_UID != 0)
            {
                query = query.Where(p => p.BG_Organization_UID == serchModel.BG_Organization_UID);
            }

            if (serchModel.EQP_Uid != 0)
            {
                query = query.Where(p => p.OEE_MachineInfo_UID == serchModel.EQP_Uid);
            }

            //ShiftTimeID=-1 全天
            query = query.Where(p => p.Product_Date >= serchModel.StartTime && p.Product_Date <= serchModel.EndTime);
            if (serchModel.ShiftTimeID != -1)
            {
                query = query.Where(p => p.ShiftTimeID == serchModel.ShiftTimeID);
            }
            else
            {
                query = query.Where(p => p.ShiftTimeID == null);
            }

            return query.ToList();
        }


        public List<OEE_EveryDayMachineDTO> QueryBuTemplateData(OEE_ReprortSearchModel serchModel, Page page)
        {
            var query = from dayMachine in DataContext.OEE_EveryDayMachine
                        where dayMachine.OEE_MachineInfo.Is_Enable == true &&
                        dayMachine.OEE_MachineInfo.GL_Station.IsEnabled == true &&
                         dayMachine.OEE_MachineInfo.GL_Station.StationID == serchModel.StationID
                        select new OEE_EveryDayMachineDTO
                        {
                            OEE_EveryDayMachine_UID = dayMachine.OEE_EveryDayMachine_UID,
                            Plant_Organization_UID = dayMachine.Plant_Organization_UID,
                            BG_Organization_UID = dayMachine.BG_Organization_UID,
                            FunPlant_Organization_UID = dayMachine.FunPlant_Organization_UID,
                            OEE_MachineInfo_UID = dayMachine.OEE_MachineInfo_UID,
                            FixtureNum = dayMachine.FixtureNum,
                            PORCT = dayMachine.PORCT,
                            ActualCT = dayMachine.ActualCT,
                            TotalAvailableHour = dayMachine.TotalAvailableHour,
                            OutPut = dayMachine.OutPut,
                            PlannedHour = dayMachine.PlannedHour,
                            ShiftTimeID = dayMachine.ShiftTimeID,
                            Product_Date = dayMachine.Product_Date,
                            UpdateTime = dayMachine.UpdateTime,
                            ResetTime = dayMachine.ResetTime,
                            Is_DownType = dayMachine.Is_DownType,
                            AbnormalDFCode = dayMachine.AbnormalDFCode,
                            OEEDashBoardTarget = dayMachine.OEE_MachineInfo.GL_Station.DashboardTarget,
                            MachineName = dayMachine.OEE_MachineInfo.MachineNo,
                            ShiftName = dayMachine.GL_ShiftTime.Shift
                        };

            if (serchModel.Plant_Organization_UID != 0)
            {
                query = query.Where(p => p.Plant_Organization_UID == serchModel.Plant_Organization_UID);
            }

            //if (serchModel.BG_Organization_UID != 0)
            //{
            //    query = query.Where(p => p.BG_Organization_UID == serchModel.BG_Organization_UID);
            //}

            if (serchModel.EQP_Uid != 0)
            {
                query = query.Where(p => p.OEE_MachineInfo_UID == serchModel.EQP_Uid);
            }

            //只取白天晚班的数据
            query = query.Where(p => p.Product_Date >= serchModel.StartTime && p.Product_Date <= serchModel.EndTime);
            query = query.Where(p => p.ShiftTimeID != null);

            return query.ToList();
        }
        /// <summary>
        /// 获取线下面所有机台的统计数据
        /// </summary>
        /// <param name="serchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<OEE_EveryDayMachineDTO> GetLineStaticData(OEE_ReprortSearchModel serchModel, Page page)
        {
            var query = from dayMachine in DataContext.OEE_EveryDayMachine
                        where dayMachine.Plant_Organization_UID == serchModel.Plant_Organization_UID &&
                         dayMachine.BG_Organization_UID == serchModel.BG_Organization_UID &&
                         dayMachine.OEE_MachineInfo.Project_UID == serchModel.CustomerID &&
                         dayMachine.OEE_MachineInfo.LineID == serchModel.LineID &&
                         dayMachine.OEE_MachineInfo.Is_Enable == true &&
                         dayMachine.OEE_MachineInfo.GL_Station.IsOEE == true &&
                         dayMachine.OEE_MachineInfo.GL_Line.IsEnabled == true
                        select new OEE_EveryDayMachineDTO
                        {
                            OEE_EveryDayMachine_UID = dayMachine.OEE_EveryDayMachine_UID,
                            Plant_Organization_UID = dayMachine.Plant_Organization_UID,
                            BG_Organization_UID = dayMachine.BG_Organization_UID,
                            FunPlant_Organization_UID = dayMachine.FunPlant_Organization_UID,
                            OEE_MachineInfo_UID = dayMachine.OEE_MachineInfo_UID,
                            MachineName = dayMachine.OEE_MachineInfo.MachineNo,
                            FixtureNum = dayMachine.FixtureNum,
                            PORCT = dayMachine.PORCT,
                            ActualCT = dayMachine.ActualCT,
                            TotalAvailableHour = dayMachine.TotalAvailableHour,
                            OutPut = dayMachine.OutPut,
                            PlannedHour = dayMachine.PlannedHour,
                            ShiftTimeID = dayMachine.ShiftTimeID,
                            Product_Date = dayMachine.Product_Date,
                            UpdateTime = dayMachine.UpdateTime,
                            StationID = dayMachine.OEE_MachineInfo.StationID,
                            StationName = dayMachine.OEE_MachineInfo.GL_Station.StationName,
                            OEEDashBoardTarget = dayMachine.OEE_MachineInfo.GL_Station.DashboardTarget
                        };
            serchModel.EndTime = serchModel.StartTime;
            query = query.Where(p => p.Product_Date >= serchModel.StartTime && p.Product_Date <= serchModel.EndTime);
            //ShiftTimeID=-1 全天
            if (serchModel.ShiftTimeID != -1)
            {
                query = query.Where(p => p.ShiftTimeID == serchModel.ShiftTimeID);
            }
            else
            {
                query = query.Where(p => p.ShiftTimeID == null);
            }

            return query.ToList();
        }
        public List<OEE_EveryDayMachineDTO> GetAllStationData(OEE_ReprortSearchModel serchModel, Page page)
        {
            var query = from dayMachine in DataContext.OEE_EveryDayMachine
                        where dayMachine.Plant_Organization_UID == serchModel.Plant_Organization_UID &&
                         dayMachine.BG_Organization_UID == serchModel.BG_Organization_UID &&
                         dayMachine.OEE_MachineInfo.Project_UID == serchModel.CustomerID &&
                         dayMachine.OEE_MachineInfo.LineID == serchModel.LineID &&
                         dayMachine.OEE_MachineInfo.StationID == serchModel.StationID &&
                         dayMachine.OEE_MachineInfo.Is_Enable == true
                        select new OEE_EveryDayMachineDTO
                        {
                            OEE_EveryDayMachine_UID = dayMachine.OEE_EveryDayMachine_UID,
                            Plant_Organization_UID = dayMachine.Plant_Organization_UID,
                            BG_Organization_UID = dayMachine.BG_Organization_UID,
                            FunPlant_Organization_UID = dayMachine.FunPlant_Organization_UID,
                            OEE_MachineInfo_UID = dayMachine.OEE_MachineInfo_UID,
                            MachineName = dayMachine.OEE_MachineInfo.MachineNo,
                            FixtureNum = dayMachine.FixtureNum,
                            PORCT = dayMachine.PORCT,
                            ActualCT = dayMachine.ActualCT,
                            TotalAvailableHour = dayMachine.TotalAvailableHour,
                            OutPut = dayMachine.OutPut,
                            PlannedHour = dayMachine.PlannedHour,
                            ShiftTimeID = dayMachine.ShiftTimeID,
                            Product_Date = dayMachine.Product_Date,
                            UpdateTime = dayMachine.UpdateTime,
                            ResetTime = dayMachine.ResetTime,
                            Is_DownType = dayMachine.Is_DownType,
                            AbnormalDFCode = dayMachine.AbnormalDFCode,
                            OEEDashBoardTarget = dayMachine.OEE_MachineInfo.GL_Station.DashboardTarget,
                            Is_Offline = dayMachine.Is_Offline,
                            OrganitionName= dayMachine.System_Organization.Organization_Name
                        };
            serchModel.EndTime = serchModel.StartTime;
            query = query.Where(p => p.Product_Date >= serchModel.StartTime && p.Product_Date <= serchModel.EndTime);
            //ShiftTimeID=-1 全天
            if (serchModel.ShiftTimeID != -1)
            {
                query = query.Where(p => p.ShiftTimeID == serchModel.ShiftTimeID);
            }
            else
            {
                query = query.Where(p => p.ShiftTimeID == null);
            }

            return query.ToList();
        }
        public Dictionary<string, int> SetOEE_MachineDayOutput(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime)
        {
            var keyValue = new Dictionary<string, int>();
            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;


            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    var sqlSB = new StringBuilder("SELECT MachineName ,  count(*) count FROM dbo.API_WIPData WHERE CustomerName=@customerName AND (StartTime BETWEEN  @startTime and @endTime) GROUP BY MachineName");
                    cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                    cmd.Parameters.Add(new SqlParameter("@startTime", startTime.ToString("yyyy-MM-dd HH:mm")));
                    cmd.Parameters.Add(new SqlParameter("@endTime", endTime.ToString("yyyy-MM-dd HH:mm")));
                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                keyValue.Add(read["MachineName"].ToString(), int.Parse(read["count"].ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }

            return keyValue;
        }

        public Dictionary<string, int> SetOEE_CNCMachineDayOutput(DateTime startTime, DateTime endTime)
        {
            var keyValue = new Dictionary<string, int>();
            string connStr = string.Empty;

            connStr = WuxiconnStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = string.Format(@"DECLARE @t1 TABLE
                                    (
                                        MachineName NVARCHAR(50),
                                        TotalNum INT
                                    );
                                DECLARE @t2 TABLE
                                    (
                                        MachineName NVARCHAR(50),
                                        TotalNum INT
                                    );

                                  INSERT INTO @t1(MachineName,
                                      TotalNum)
                                (SELECT AuctionRecords.MachineName,
                                        AuctionRecords.TotalNum
                                 FROM(SELECT ROW_NUMBER() OVER(PARTITION BY MachineName
                                                                       ORDER BY ID DESC) rowId,
                                                   *
                                            FROM   dbo.API_MachineStatus
                                            WHERE  EventTime
                                                   BETWEEN '{0}' AND '{1}'
                                                   AND MachineType = 'CNC') AS AuctionRecords
                                 WHERE  rowId = 1);

                              INSERT INTO @t2(MachineName,
                                      TotalNum)
                                (SELECT AuctionRecords.MachineName,
                                        AuctionRecords.TotalNum
                                 FROM(SELECT ROW_NUMBER() OVER(PARTITION BY MachineName
                                                                       ORDER BY ID DESC) rowId,
                                                   *
                                            FROM   dbo.API_MachineStatus
                                            WHERE  EventTime
                                                   BETWEEN '{2}' AND '{0}'
                                                   AND MachineType = 'CNC') AS AuctionRecords
                                 WHERE  rowId = 1);

                                    SELECT[@t1].MachineName ,
                       [@t1].TotalNum TotalNum1,
                       [@t2].TotalNum TotalNum2,
                       [@t1].TotalNum - [@t2].TotalNum AS TotalNum
                FROM   @t1
                LEFT JOIN @t2 ON[@t2].MachineName = [@t1].MachineName;", startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"), startTime.AddDays(-1));


                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                keyValue.Add(read["MachineName"].ToString(), int.Parse(read["TotalNum"].ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }

            return keyValue;
        }

        /// <summary>
        /// 判断该时段是否有数据
        /// </summary>
        /// <param name="Project_UID"></param>
        /// <param name="Date"></param>
        /// <param name="ShiftID"></param>
        /// <returns></returns>
        public List<OEE_EveryDayMachine> judgmentData(int Project_UID, string Date, int shiftID)
        {

            var sql = $"  SELECT OE.* FROM dbo.OEE_EveryDayMachine OE JOIN dbo.OEE_MachineInfo OM ON OM.OEE_MachineInfo_UID = OE.OEE_MachineInfo_UID  WHERE Product_Date = '{Date}' AND ShiftTimeID = {shiftID} AND OM.Project_UID = {Project_UID}";
            var dblist = DataContext.Database.SqlQuery<OEE_EveryDayMachine>(sql).ToList();
            return dblist;
        }

        public List<OEE_EveryDayMachine> ALLjudgmentData(int Project_UID, string Date)
        {

            var sql = $"  SELECT OE.* FROM dbo.OEE_EveryDayMachine OE JOIN dbo.OEE_MachineInfo OM ON OM.OEE_MachineInfo_UID = OE.OEE_MachineInfo_UID  WHERE Product_Date = '{Date}' AND ShiftTimeID  IS NULL AND OM.Project_UID = {Project_UID}";
            var dblist = DataContext.Database.SqlQuery<OEE_EveryDayMachine>(sql).ToList();
            return dblist;
        }

        public List<OEE_DefectCodeDailyNum> GetOEEDefectRord(int Project_UID, string Date, int shiftID, string TimeInterval)
        {
            var sql = $"   SELECT OMD.* FROM dbo.OEE_DefectCodeDailyNum OMD JOIN  dbo.OEE_MachineInfo OM  ON OM.OEE_MachineInfo_UID = OMD.OEE_MachineInfo_UID  WHERE OM.Project_UID= {Project_UID} AND OMD.ProductDate='{Date}' AND OMD.ShiftTimeID={shiftID} AND OMD.TimeInterval='{TimeInterval}'";
            var dblist = DataContext.Database.SqlQuery<OEE_DefectCodeDailyNum>(sql).ToList();
            return dblist;
        }

        public List<OEE_DefectCodeDailySum> GetOEEDefectSum(int Project_UID, string Date, int shiftID)
        {
            var sql = $"   SELECT OMD.* FROM dbo.OEE_DefectCodeDailySum OMD JOIN  dbo.OEE_MachineInfo OM  ON OM.OEE_MachineInfo_UID = OMD.OEE_MachineInfo_UID  WHERE OM.Project_UID= {Project_UID} AND OMD.ProductDate='{Date}' AND OMD.ShiftTimeID={shiftID}";
            var dblist = DataContext.Database.SqlQuery<OEE_DefectCodeDailySum>(sql).ToList();
            return dblist;
        }

        public void DeleteDownRocords(int Project_UID, string Date, int shiftID)
        {
            var sql = @"DELETE dbo.OEE_MachineDailyDownRecord WHERE OEE_MachineDailyDownRecord_UID IN (

  SELECT OMD.OEE_MachineDailyDownRecord_UID FROM dbo.OEE_MachineDailyDownRecord OMD JOIN dbo.OEE_MachineInfo OM ON OM.OEE_MachineInfo_UID = OMD.OEE_MachineInfo_UID 
  WHERE OM.Project_UID={0} AND OMD.DownDate='{1}' AND OMD.ShiftTimeID={2})";
            sql = string.Format(sql, Project_UID, Date, shiftID);
           var dblist = DataContext.Database.ExecuteSqlCommand(sql);
      
        }
        public List<OEE_MachineDailyDownRecord> GetOEEDownRocord(int Project_UID, string Date, int shiftID)
        {
            var sql = $" SELECT OMD.* FROM dbo.OEE_MachineDailyDownRecord OMD JOIN  dbo.OEE_MachineInfo OM  ON OM.OEE_MachineInfo_UID = OMD.OEE_MachineInfo_UID WHERE OM.Project_UID={Project_UID} AND OMD.DownDate='{Date}' AND OMD.ShiftTimeID={shiftID}";
            var dblist = DataContext.Database.SqlQuery<OEE_MachineDailyDownRecord>(sql).ToList();
            return dblist;
        }
        

        public List<OEE_MachineInfo> getAllMachineByProject(int ProjectUID)

        {
            var query = from Machine in DataContext.OEE_MachineInfo
                        where Machine.Project_UID == ProjectUID && Machine.Is_Enable == true
                        select Machine;
            return query.ToList();

        }

        public List<OEE_StationDefectCode> getAllErrorCodeByProject(int ProjectUID)

        {
            var query = from D in DataContext.OEE_StationDefectCode
                        where D.Project_UID == ProjectUID && D.Is_Enable == true
                        select D;
            return query.ToList();

        }
        public List<OEE_DownTimeCode> getAllDownCodeByProject(int ProjectUID)

        {
            var query = from D in DataContext.OEE_DownTimeCode
                        where D.Project_UID == ProjectUID && D.Is_Enable == true
                        select D;
            return query.ToList();

        }
        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="WIPHourOutputList"></param>
        /// <returns></returns>
        public bool AddOEEDailyInfo(List<OEE_EveryDayMachineDTO> WIPHourOutputList)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in WIPHourOutputList)
                    {
                        var sql = @" INSERT INTO dbo.OEE_EveryDayMachine ( Plant_Organization_UID ,BG_Organization_UID , FunPlant_Organization_UID ,OEE_MachineInfo_UID ,FixtureNum , PORCT , ActualCT ,TotalAvailableHour ,PlannedHour ,OutPut ,ShiftTimeID ,Product_Date )VALUES ( {0} , {1},{2},{3},{4},{5} , {6} ,  {7} , {8} ,{9} ,{10},      
GETDATE()
										)";
                        sql = string.Format(sql, item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID, item.OEE_MachineInfo_UID,
                            item.FixtureNum, item.PORCT, item.ActualCT, item.TotalAvailableHour, item.PlannedHour, item.OutPut, item.ShiftTimeID);
                        sb.AppendLine(sql);
                    }
                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="WIPHourOutputList"></param>
        /// <returns></returns>
        public bool UpDateOEEDailyInfo(List<OEE_EveryDayMachine> WIPHourOutputList)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    StringBuilder sb = new StringBuilder();
                    if (WIPHourOutputList.Count == 0)
                    {
                        return true;
                    }
                    foreach (var item in WIPHourOutputList)
                    {
                        string sql = string.Empty;
                        if(item.Is_Offline==null)
                        {
                            item.Is_Offline = false;
                        }
                        if (item.ResetTime == null && item.Is_DownType == null)
                        {
                            sql = $"UPDATE dbo.OEE_EveryDayMachine SET  FixtureNum={item.FixtureNum} ,PORCT={item.PORCT},ActualCT={item.ActualCT},PlannedHour={item.PlannedHour},OutPut={item.OutPut},TotalAvailableHour={item.TotalAvailableHour} ,Is_Offline='{item.Is_Offline}' , UpdateTime =GETDATE() WHERE OEE_EveryDayMachine_UID ={item.OEE_EveryDayMachine_UID}";
                        }
                        else if (item.ResetTime != null && item.Is_DownType == null)
                        {
                            sql = $"UPDATE dbo.OEE_EveryDayMachine SET  FixtureNum={item.FixtureNum} ,PORCT={item.PORCT},ActualCT={item.ActualCT},PlannedHour={item.PlannedHour},OutPut={item.OutPut},TotalAvailableHour={item.TotalAvailableHour} , ResetTime='{item.ResetTime}'  ,Is_Offline='{item.Is_Offline}', UpdateTime =GETDATE() WHERE OEE_EveryDayMachine_UID ={item.OEE_EveryDayMachine_UID}";
                        }
                        else if (item.ResetTime == null && item.Is_DownType != null)
                        {
                            sql = $"UPDATE dbo.OEE_EveryDayMachine SET  FixtureNum={item.FixtureNum} ,PORCT={item.PORCT},ActualCT={item.ActualCT},PlannedHour={item.PlannedHour},OutPut={item.OutPut},TotalAvailableHour={item.TotalAvailableHour} , Is_DownType='{item.Is_DownType}' ,Is_Offline='{item.Is_Offline}' , UpdateTime =GETDATE() WHERE OEE_EveryDayMachine_UID ={item.OEE_EveryDayMachine_UID}";
                        }
                        else if (item.ResetTime != null && item.Is_DownType != null)
                        {
                            sql = $"UPDATE dbo.OEE_EveryDayMachine SET  FixtureNum={item.FixtureNum} ,PORCT={item.PORCT},ActualCT={item.ActualCT},PlannedHour={item.PlannedHour},OutPut={item.OutPut},TotalAvailableHour={item.TotalAvailableHour}, ResetTime='{item.ResetTime}'  , Is_DownType='{item.Is_DownType}' ,Is_Offline='{item.Is_Offline}' , UpdateTime =GETDATE() WHERE OEE_EveryDayMachine_UID ={item.OEE_EveryDayMachine_UID}";
                        }

                        sb.AppendLine(sql.ToString());
                    }
                    DataContext.Database.ExecuteSqlCommand(sb.ToString());
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public OEE_MachineInfo GetMachineInfoByMachineNo(string MachineNo)
        {
            var sql = from P in DataContext.OEE_MachineInfo
                      where P.MachineNo == MachineNo
                      select P;

            return sql.FirstOrDefault();

        }

        public Dictionary<string, int> GetFixtureNum(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime)
        {
            var keyValue = new Dictionary<string, int>();
            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    var sqlSB = new StringBuilder(" SELECT MachineName, COUNT( DISTINCT(Fixture)) count FROM dbo.API_WIPData WHERE (StartTime BETWEEN  @startTime and @endTime) AND fixture IS NOT NULL AND fixture<>'' AND CustomerName=@customerName GROUP BY  MachineName");
                    cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                    cmd.Parameters.Add(new SqlParameter("@startTime", startTime.ToString("yyyy-MM-dd HH:mm")));
                    cmd.Parameters.Add(new SqlParameter("@endTime", endTime.ToString("yyyy-MM-dd HH:mm")));
                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                keyValue.Add(read["MachineName"].ToString(), int.Parse(read["count"].ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }

            return keyValue;
        }

        public Dictionary<string, int> GetCTUFixtureNum(string CurrentDate)
        {
            var keyValue = new Dictionary<string, int>();
            string connStr = string.Empty;

            connStr = ChengduconnStr;


            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    //var sqlSB = new StringBuilder("SELECT  MachineNo,FixtureNum FROM dbo.API_MachineInfo  WHERE FixtureNum<>'' AND MachineNo <>'' AND CONVERT(DATE,ProductDate) =@CurrentDate");
                    //cmd.Parameters.Add(new SqlParameter("@CurrentDate", CurrentDate));

                    cmd.CommandText = @"
                                                SELECT AuctionRecords.MachineNo ,AuctionRecords.FixtureNum
                                                FROM   (   SELECT ROW_NUMBER() OVER ( PARTITION BY MachineNo
                                                                                      ORDER BY ID DESC ) rowId ,
                                                                  *
                                                           FROM   API_MachineInfo
                                                         ) AS AuctionRecords
                                                WHERE  rowId = 1 AND AuctionRecords.MachineNo<>' '   and ISNUMERIC(AuctionRecords.FixtureNum)=1";
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                keyValue.Add(read["MachineNo"].ToString(), int.Parse(read["FixtureNum"].ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }

            return keyValue;
        }
        public Dictionary<string, double> GetActualCT(string Plant, string MESCustomerName, DateTime startTime, DateTime endTime)
        {
            var keyValue = new Dictionary<string, double>();
            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    var sqlSB = new StringBuilder(" SELECT MachineName, AVG(CONVERT(FLOAT, ActualCT)) AS ActualCT FROM dbo.API_WIPData WHERE CustomerName = @customerName  AND (StartTime BETWEEN  @startTime and @endTime)  AND   ISNUMERIC(ActualCT)<>0 GROUP BY MachineName");
                    cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                    cmd.Parameters.Add(new SqlParameter("@startTime", startTime.ToString("yyyy-MM-dd HH:mm")));
                    cmd.Parameters.Add(new SqlParameter("@endTime", endTime.ToString("yyyy-MM-dd HH:mm")));
                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                keyValue.Add(read["MachineName"].ToString(), double.Parse(read["ActualCT"].ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return keyValue;
        }
        public List<LocalCNCInfo> GetCNCActualCT(string Plant, DateTime startTime, DateTime endTime)
        {
            if (endTime > DateTime.Now)
                endTime = DateTime.Now;
            List<LocalCNCInfo> CNCInfoLists = new List<LocalCNCInfo>();
            string connStr = string.Empty;

              if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else 
            {
                connStr = ChengduconnStr;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = string.Format(@"
                
DECLARE @t1 TABLE --基础数据
    (
        MachineName NVARCHAR(50) ,
        TotalNum INT ,
        CT INT ,
        EventTime DATETIME NULL
    );

DECLARE @tCT TABLE --暂存数据CT 
    (
        MachineName NVARCHAR(50) ,
        CT INT
    );

DECLARE @tLatestOuput TABLE --暂存最新的产量 
    (
        MachineName NVARCHAR(50) ,
        TotalNum INT
    );

DECLARE @tStartOuput TABLE --暂存开始的产量 
    (
        MachineName NVARCHAR(50) ,
        TotalNum INT
    );
DECLARE @tOutput TABLE --暂存当班次的产量 
    (
        MachineName NVARCHAR(50) ,
        TotalNum INT
    );


--用于存放异常调零的数据-----
DECLARE @SpecialDB TABLE
    (
        MachineName NVARCHAR(50) ,
        EventTime DATETIME
    );
----获取所有的CT及产量相关基础信息
INSERT INTO @t1 ( MachineName ,
                  TotalNum ,
                  CT ,
                  EventTime )
            (SELECT MachineName ,
                    TotalNum ,
                    ProcessCycleTm / 2 CT ,
                    EventTime
             FROM   dbo.API_CNCProductInfo
             WHERE  EventTime > '{0}'
                    AND EventTime < '{1}'
                    AND MainProgram = '3200');
INSERT INTO @t1 ( MachineName ,
                  TotalNum ,
                  CT ,
                  EventTime )
            (SELECT MachineName ,
                    TotalNum ,
                    ProcessCycleTm CT ,
                    EventTime
             FROM   dbo.API_CNCProductInfo
             WHERE  EventTime > '{0}'
                    AND EventTime < '{1}'
                    AND MainProgram = '3100');

-- 获取产量调零时间-----
INSERT INTO @SpecialDB ( MachineName ,
                         EventTime )
            SELECT AuctionRecords.MachineName ,
                   AuctionRecords.EventTime
            FROM   (   SELECT ROW_NUMBER() OVER ( PARTITION BY MachineName
                                                  ORDER BY EventTime DESC ) rowId ,
                              *
                       FROM   @t1
                       WHERE  TotalNum = 0 ) AS AuctionRecords
            WHERE  rowId = 1;
----获取机台CT
INSERT INTO @tCT ( MachineName ,
                   CT )
            SELECT   MachineName ,
                     AVG(CT) CT
            FROM     @t1
            GROUP BY [@t1].MachineName
            ORDER BY MachineName;


-- 获取截止到目前为止机台的产量-----
INSERT INTO @tLatestOuput ( MachineName ,
                            TotalNum )
            (SELECT AuctionRecords.MachineName ,
                    AuctionRecords.TotalNum
             FROM   (   SELECT ROW_NUMBER() OVER ( PARTITION BY MachineName
                                                   ORDER BY EventTime DESC ) rowId ,
                               *
                        FROM   @t1 ) AS AuctionRecords
             WHERE  rowId = 1);

--获取班次开始时的产量-----
INSERT INTO @tStartOuput ( MachineName ,
                           TotalNum )
            (SELECT AuctionRecords.MachineName ,
                    AuctionRecords.TotalNum
             FROM   (   SELECT ROW_NUMBER() OVER ( PARTITION BY MachineName
                                                   ORDER BY EventTime ) rowId ,
                               *
                        FROM   @t1 ) AS AuctionRecords
             WHERE  rowId = 1);

--将已经调零的机台开始产量修改为0

UPDATE @tStartOuput
SET    TotalNum = 0
WHERE  MachineName IN (   SELECT MachineName
                          FROM   @SpecialDB );
---找到最终每个机台的产出
INSERT INTO @tOutput ( MachineName ,
                       TotalNum )
            SELECT [@tLatestOuput].MachineName ,
                   [@tLatestOuput].TotalNum - [@tStartOuput].TotalNum TotalNum
            FROM   @tLatestOuput
                   JOIN @tStartOuput ON [@tStartOuput].MachineName = [@tLatestOuput].MachineName;


--然后join在一起返回
SELECT  [@tOutput].MachineName ,TotalNum,CT,EventTime FROM @tOutput left JOIN @tCT ON [@tCT].MachineName = [@tOutput].MachineName 
LEFT JOIN  @SpecialDB ON [@SpecialDB].MachineName = [@tOutput].MachineName  ORDER BY [@SpecialDB].MachineName



 ", startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"));
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                LocalCNCInfo item = new LocalCNCInfo();
                                item.MachineNo = read["MachineName"].ToString();
                                item.OutPut = int.Parse(read["TotalNum"].ToString());
                                item.ActualCT = double.Parse(read["CT"].ToString());
                                item.ResetTime = read["EventTime"] == null ? null : read["EventTime"].ToString();
                                CNCInfoLists.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return CNCInfoLists;
        }

        public List<ProPerCT> GetPorCT(string Plant, string MESCustomerName)
        {
            var keyValue = new List<ProPerCT>();
            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT * FROM dbo.API_RegisteredMachine WHERE Customer='{0}'", MESCustomerName);
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                ProPerCT item = new ProPerCT();
                                item.MachineName = read["Name"].ToString();
                                if (read["PorCT"] == null || read["PorCT"].ToString() == "")
                                    item.PorCT = 0;
                                else
                                    item.PorCT = double.Parse(read["PorCT"].ToString());
                                    keyValue.Add(item);
                                // keyValue.Add(read["MachineName"].ToString(), double.Parse(read["PorCT"].ToString()));
                                //   keyValue.Add(read["MachineName"].ToString(),14.0);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return keyValue;
        }


        public List<string> GetALLCNCMachineName(string connStr)
        {
            var keyValue = new List<string>();


            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT   Name MachineName FROM dbo.API_RegisteredMachine WHERE   Category ='CNCDataCollection'";
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {

                                keyValue.Add(read["MachineName"].ToString());
                                // keyValue.Add(read["MachineName"].ToString(), double.Parse(read["PorCT"].ToString()));
                                //   keyValue.Add(read["MachineName"].ToString(),14.0);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return keyValue;
        }

       /// <summary>
       /// 获取指定厂区五分钟内有数据的机台名 -未断网的
       /// </summary>
       /// <param name="Plant"></param>
       /// <returns></returns>
        public List<string> getLocalMachineStatus(string Plant)
        {
            var keyValue = new List<string>();
            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //获取所有CNC机台，若机台为ＣＮＣ机台，则采用ＣＮＣ的方式（５秒钟更新一次的方式），其他机台不用处理，６０Ｓ更新一次，单位为分钟。

                    cmd.CommandText = "";
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                
                               
                                keyValue.Add("");

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
            return keyValue;
        }

        public List<OEE_DownRecordsDTO> getOEELocalDatas(string Plant, DateTime startTime, DateTime endTime)
        {
            var keyValue = new List<OEE_DownRecordsDTO>();
            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (Plant == "CTU_M" || Plant == "CTU")   //成都先直接用原数据
                    {
                        cmd.CommandText = string.Format(@"
                
              IF OBJECT_ID('tempdb..#MachineStatusData', 'U') IS NOT NULL
                            DROP TABLE #MachineStatusData;
                           CREATE TABLE #MachineStatusData
                            (
                                [MachineName] VARCHAR(100) ,
                                [Status] INT ,
                                [ErrorCode] NVARCHAR(50) ,
                                [EventTime] DATETIME ,
                                CONSTRAINT [PK_Dat4ta31s]
                                    PRIMARY KEY CLUSTERED
                                    (
                                        [Status] ,
                                        [ErrorCode] ,
                                        [EventTime] ASC )
                                    WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF ,
                                           IGNORE_DUP_KEY = ON, ALLOW_ROW_LOCKS = ON ,
                                           ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
                            ) ON [PRIMARY];
                        INSERT INTO #MachineStatusData ( [MachineName] ,
                                                         [Status] ,
                                                         [ErrorCode] ,
                                                         [EventTime] )
                                    SELECT
                                        --,[AuthCode]
                                          [MachineName] ,
                                          [Status] ,
                                          [ErrorCode] ,
                                          [EventTime]
                                    FROM  [dbo].[API_MachineStatus] a
                                    WHERE [EventTime]
                                        > '{0}'  AND [EventTime]<  '{1}';
                        SELECT [MachineName] ,
                               [Status] ,
                               [ErrorCode] ,
                               [EventTimeSt] ,
                               [EventTimeEd] ,
                               DATEDIFF(SECOND, [EventTimeSt], [EventTimeEd]) [Peroid]
                        FROM   (
                                   SELECT   [MachineName] ,
                                            [Status] ,
                                            [ErrorCode] ,
                                            MIN([EventTimeSt]) [EventTimeSt] ,
                                            [EventTimeEd]
                                   FROM     (
                                                SELECT [MachineName] ,
                                                       [Status] ,
                                                       [ErrorCode] ,
                                                       [EventTime] [EventTimeSt] ,
                                                       ISNULL(
                                                       (
                                                           SELECT MIN([EventTime])
                                                           FROM   #MachineStatusData
                                                           WHERE  [EventTime] > a.[EventTime]
                                                                  AND [MachineName] = a.[MachineName] ) ,
                                                      '{1}') [EventTimeEd]
                                                FROM   #MachineStatusData a
                                   ) x WHERE x.ErrorCode <>' '
                                   GROUP BY [MachineName] ,
                                            [Status] ,
                                            [ErrorCode] ,
                                            [EventTimeEd] ) y;
 ", startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"));
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    OEE_DownRecordsDTO OD = new OEE_DownRecordsDTO();
                                    OD.MachineName = read["MachineName"].ToString();
                                    OD.ErrorCode = read["ErrorCode"].ToString().Split(',')[0];
                                   OD.StartTime=read["EventTimeSt"].ToString();
                                    OD.EndTIme = read["EventTimeEd"].ToString();
                                    //if (CNCMachineList.Contains(OD.MachineName))  //判断是否为ＣＮＣ，ＣＮＣ需要除以５得到实际数据，已过时现在都不需要了
                                    //{
                                    //    OD.count = int.Parse(read["count"].ToString());
                                    //}
                                    //else
                                    //    OD.count = int.Parse(read["count"].ToString());
                                    OD.count = int.Parse(read["Peroid"].ToString());
                                    keyValue.Add(OD);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                    else
                    {
                        //获取所有CNC机台，若机台为ＣＮＣ机台，则采用ＣＮＣ的方式（５秒钟更新一次的方式），其他机台不用处理，６０Ｓ更新一次，单位为分钟。
                        // List<string> CNCMachineList = GetALLCNCMachineName(connStr);
                        var sqlStr = string.Format(@" SELECT MachineName, ErrorCode, COUNT(*) count FROM dbo.API_MachineStatus WHERE (EventTime BETWEEN {0} and {1})  AND ErrorCode <>'' GROUP BY MachineName, ErrorCode", startTime, endTime);
                        var sqlSB = new StringBuilder(" SELECT MachineName, ErrorCode, COUNT(*) count FROM dbo.API_MachineStatus WHERE (EventTime BETWEEN  @startTime and @endTime)  AND ErrorCode <>'' AND ErrorCode <>'RUN'    GROUP BY MachineName, ErrorCode");
                        cmd.Parameters.Add(new SqlParameter("@startTime", startTime.ToString("yyyy-MM-dd HH:mm")));
                        cmd.Parameters.Add(new SqlParameter("@endTime", endTime.ToString("yyyy-MM-dd HH:mm")));
                        cmd.CommandText = sqlSB.ToString();
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    OEE_DownRecordsDTO OD = new OEE_DownRecordsDTO();
                                    OD.MachineName = read["MachineName"].ToString();
                                    OD.ErrorCode = read["ErrorCode"].ToString().Split(',')[0];
                                    //if (CNCMachineList.Contains(OD.MachineName))  //判断是否为ＣＮＣ，ＣＮＣ需要除以５得到实际数据，已过时现在都不需要了
                                    //{
                                    //    OD.count = int.Parse(read["count"].ToString());
                                    //}
                                    //else
                                    //    OD.count = int.Parse(read["count"].ToString());
                                    OD.StartTime =DateTime.Now.ToString();
                                    OD.EndTIme = DateTime.Now.ToString();
                                    OD.count = int.Parse(read["count"].ToString()) * 60;
                                    keyValue.Add(OD);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return keyValue;
        }
        public double GetPlanHour(int shiftID)
        {
            var sql = from ST in DataContext.GL_ShiftTime
                      where ST.ShiftTimeID == shiftID
                      select ST;

            var item = sql.FirstOrDefault();
            DateTime Start = DateTime.Parse(item.StartTime);
            DateTime End = DateTime.Parse(item.End_Time);
            if (End < Start)  //如果结束时间小于开始时间，说明跨天了；
            {
                return (End - Start).TotalHours - item.Break_Time + 24;
            }
            return (End - Start).TotalHours - item.Break_Time;
        }

        public string GetLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            var query = from EveryDayMachine in DataContext.OEE_EveryDayMachine
                        where EveryDayMachine.Plant_Organization_UID == serchMode.Plant_Organization_UID
                           && EveryDayMachine.BG_Organization_UID == serchMode.BG_Organization_UID
                           && EveryDayMachine.OEE_MachineInfo_UID == serchMode.EQP_Uid
                        select EveryDayMachine;

            if (query.OrderByDescending(p => p.UpdateTime).FirstOrDefault() == null)
            {
                return "No";
            }
            else
            {
                var updateTime = query.OrderByDescending(p => p.UpdateTime).FirstOrDefault().UpdateTime;
                return updateTime.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string GetStationLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            var query = from EveryDayMachine in DataContext.OEE_EveryDayMachine
                        where EveryDayMachine.Plant_Organization_UID == serchMode.Plant_Organization_UID
                           && EveryDayMachine.BG_Organization_UID == serchMode.BG_Organization_UID
                           && EveryDayMachine.OEE_MachineInfo.StationID == serchMode.StationID
                        select EveryDayMachine;

            if (query.OrderByDescending(p => p.UpdateTime).FirstOrDefault() == null)
            {
                return "No";
            }
            else
            {
                var updateTime = query.OrderByDescending(p => p.UpdateTime).FirstOrDefault().UpdateTime;
                return updateTime.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string GetRealLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            var query = from EveryDayMachine in DataContext.OEE_MachineStatus
                        where  EveryDayMachine.OEE_MachineInfo.StationID == serchMode.StationID
                        select   EveryDayMachine ;

            if (query.OrderByDescending(p => p.UpdateTime).FirstOrDefault() == null)
            {
                return "No";
            }
            else
            {
                var updateTime = query.OrderByDescending(p => p.UpdateTime).FirstOrDefault().UpdateTime;
                return updateTime.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string GetLineLastUpdateTime(OEE_ReprortSearchModel serchMode)
        {
            var query = from EveryDayMachine in DataContext.OEE_EveryDayMachine
                        where EveryDayMachine.Plant_Organization_UID == serchMode.Plant_Organization_UID
                           && EveryDayMachine.BG_Organization_UID == serchMode.BG_Organization_UID
                           && EveryDayMachine.OEE_MachineInfo.GL_Line.LineID == serchMode.LineID
                        select EveryDayMachine;

            if (query.OrderByDescending(p => p.UpdateTime).FirstOrDefault() == null)
            {
                return "No";
            }
            else
            {
                var updateTime = query.OrderByDescending(p => p.UpdateTime).FirstOrDefault().UpdateTime;
                return updateTime.ToString("yyyy-MM-dd HH:mm");
            }
        }
        /// <summary>
        /// 获取全天班的计划时间
        /// </summary>
        /// <param name="BU_UID"></param>
        /// <returns></returns>
        public double GetALLPlanHour(int BU_UID)

        {
            var sql = from ST in DataContext.GL_ShiftTime
                      where ST.BG_Organization_UID == BU_UID
                      && ST.IsEnabled == true
                      select ST;

            var lists = sql.ToList();
            double BreakValue = 0.0;
            foreach (var item in lists)
            {
                BreakValue += item.Break_Time;
            }
            return 24 - BreakValue;

        }

        /// <summary>
        /// 根据bgid获取全天班的开始时间
        /// </summary>
        /// <param name="BG_UID"></param>
        /// <returns></returns>
        public string getStartTime(int BG_UID)
        {
            var query = from ST in DataContext.GL_ShiftTime
                        where ST.BG_Organization_UID == BG_UID
                        orderby ST.Sequence
                        select ST.StartTime;
            return query.ToList().FirstOrDefault();

        }

      public    List<OEE_MachineStatusD> getOEEStatusDatas(string Plant, DateTime startTime, DateTime endTime)
        {
            var keyValue = new List<OEE_MachineStatusD>();
            string connStr = string.Empty;
            if (Plant == "WUXI_M")
            {
                connStr = WuxiconnStr;
            }
            else if (Plant == "CTU_M" || Plant == "CTU")
            {
                connStr = ChengduconnStr;
            }
            else
                connStr = HuizhouconnStr;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (Plant == "CTU_M" || Plant == "CTU"||Plant == "WUXI_M")   //成都先直接用原数据
                    {
                        cmd.CommandText = string.Format(@"
                        IF OBJECT_ID('tempdb..#MachineStatusData', 'U') IS NOT NULL
                            DROP TABLE #MachineStatusData;
                           CREATE TABLE #MachineStatusData
                            (
                                [MachineName] VARCHAR(100) ,
                                [Status] INT ,
                                [ErrorCode] NVARCHAR(50) ,
                                [EventTime] DATETIME ,
                                CONSTRAINT [PK_Dat4ta31s]
                                    PRIMARY KEY CLUSTERED
                                    (
                                        [Status] ,
                                        [ErrorCode] ,
                                        [EventTime] ASC )
                                    WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF ,
                                           IGNORE_DUP_KEY = ON, ALLOW_ROW_LOCKS = ON ,
                                           ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
                            ) ON [PRIMARY];
                        INSERT INTO #MachineStatusData ( [MachineName] ,
                                                         [Status] ,
                                                         [ErrorCode] ,
                                                         [EventTime] )
                                    SELECT
                                        --,[AuthCode]
                                          [MachineName] ,
                                          [Status] ,
                                          [ErrorCode] ,
                                          [EventTime]
                                    FROM  [dbo].[API_MachineStatus] a
                                    WHERE [EventTime]
                                          BETWEEN '{0}' AND '{1}';
                        SELECT [MachineName] ,
                               [Status] ,
                               [ErrorCode] ,
                               [EventTimeSt] ,
                               [EventTimeEd] ,
                               DATEDIFF(SECOND, [EventTimeSt], [EventTimeEd]) [Peroid]
                        FROM   (
                                   SELECT   [MachineName] ,
                                            [Status] ,
                                            [ErrorCode] ,
                                            MIN([EventTimeSt]) [EventTimeSt] ,
                                            [EventTimeEd]
                                   FROM     (
                                                SELECT [MachineName] ,
                                                       [Status] ,
                                                       [ErrorCode] ,
                                                       [EventTime] [EventTimeSt] ,
                                                       ISNULL(
                                                       (
                                                           SELECT MIN([EventTime])
                                                           FROM   #MachineStatusData
                                                           WHERE  [EventTime] > a.[EventTime]
                                                                  AND [MachineName] = a.[MachineName] ) ,
                                                      '{1}') [EventTimeEd]
                                                FROM   #MachineStatusData a
                                   ) x
                                   GROUP BY [MachineName] ,
                                            [Status] ,
                                            [ErrorCode] ,
                                            [EventTimeEd] ) y;
                         ", startTime.ToString("yyyy-MM-dd HH:mm"), endTime.ToString("yyyy-MM-dd HH:mm"));
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    OEE_MachineStatusD OD = new OEE_MachineStatusD();
                                    OD.MachineName = read["MachineName"].ToString();
                                    OD.StartTime = DateTime.Parse( read["EventTimeSt"].ToString());
                                    OD.EndTIme = DateTime.Parse(read["EventTimeEd"].ToString());
                                    OD.StatusDuration = int.Parse(read["Peroid"].ToString());
                                    OD.UpdateTime = DateTime.Now;
                                    OD.StatusID= int.Parse(read["Status"].ToString());
                                    keyValue.Add(OD);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                    else
                    {

                    }
                }
            }
            return keyValue;
        }

     public   OEE_RealStatusReport QueryRealStatusReport(OEE_ReprortSearchModel serchModel)
        {
            var sqlT = from S in DataContext.GL_ShiftTime
                      where S.BG_Organization_UID == serchModel.BG_Organization_UID
                       && S.ShiftTimeID == serchModel.ShiftTimeID
                      select S;
            var sql = from ST in DataContext.OEE_MachineStatus
                      where ST.Product_Date == serchModel.StartTime && ST.ShiftTimeID == serchModel.ShiftTimeID
                      && ST.OEE_MachineInfo.StationID == serchModel.StationID
                      //orderby ST.OEE_MachineInfo.MachineNo
                      select new OEE_RealStatusDTO
                      {
                          MachineID = 0,
                          MachineNO = ST.OEE_MachineInfo.MachineNo,
                          StatusID = ST.StatusID,
                          StartTime = ST.StartTime,
                          EndTime = ST.EndTime
                      };
            OEE_RealStatusReport result = new OEE_RealStatusReport();
            var item = sqlT.FirstOrDefault();
            if (item == null) return null;
            result.StartTime = serchModel.StartTime.ToString("yyyy-MM-dd") + " " + item.StartTime;
            result.EndTime = serchModel.StartTime.ToString("yyyy-MM-dd") + " " + item.End_Time;
            var StartDateTime = DateTime.Parse(result.StartTime);
            var  EndDateTime = DateTime.Parse(result.EndTime);
          if(    DateTime.Compare(StartDateTime ,EndDateTime)>0)
            {
                result.EndTime=EndDateTime.AddDays(1).ToString("yyyy-MM-dd") + " " + item.End_Time;
            }
            result.OEE_RealStatusList = sql.ToList();
            return result;
        }
    }
}
