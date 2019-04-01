using PDMS.Common.Enums;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IGoldenLineRepository : IRepository<System_Project>
    {
        Dictionary<string, int> GetOutput(string MESCustomerName, string MESStationName, DateTime startTime, DateTime? endTime, int Plant_Organization_UID);
        Dictionary<string, GL_HoureOutputModel> GetNewOutput(string MESCustomerName, string MESStationName, DateTime startTime, DateTime? endTime, int Plant_Organization_UID);
        int GetLineOutput(int lineID, DateTime startTime, DateTime? endTime);
        EnumLineStatus GetLineStatus(int lineID);
        EnumLineStatus GetLineStatus(int lineID, DateTime dateTime);
        DateTime? GetStartTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime startTime, DateTime? endTime);

        double GetRunTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime);
        double GetUnPlanDownTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime);
        DateTime GetLastUpdateTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime flagTime);
        List<SystemProjectDTO> GetAllEnGL_Customer();

        List<GL_WIPEventDTO> GetGL_WIPEvent(string customerName, string stationName, int Plant_Organization_UID, DateTime startTime, DateTime? endTime);
        //List<GL_Customer> QueryCustomers(int oporgid, int? funorgid);

        List<WipEventModel> ExportWIPHourOutput(GL_WIPHourOutputDTO search);
        GL_StationDTO GetStationDTOIsOutput(int LineID);
        List<OEE_UserStationDTO> QueryOperatorList(int LineID);
        List<OEE_UserStationDTO> GetOperatorList(int CustomerID);
        List<SystemUserOEEDTO> GetAllUserByDTOs(int Plant_OrganizationUID);
        string ImportOperatorList(List<OEE_UserStationDTO> OperatorLists);

    }
    public class GoldenLineRepository : RepositoryBase<System_Project>, IGoldenLineRepository
    {
        private string connStr = "Server=CNWXIM0CCDDB01;DataBase=PMS;uid=PISUser;pwd=PISUser123";
        private string connStrCTU = "Server=CNCTUG0MLSQLV1A;DataBase=OEE;uid=pis;pwd=jabil@1234";
        public GoldenLineRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
        public Dictionary<string, int> GetOutput(string MESCustomerName, string MESStationName, DateTime startTime, DateTime? endTime, int Plant_Organization_UID)
        {
            var keyValue = new Dictionary<string, int>();
            string connectionStrings = "";
            if (Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }

            using (SqlConnection conn = new SqlConnection(connectionStrings))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    var sqlSB = new StringBuilder("select stationName,count = count(*) from wipevent where Status = 'Pass' and CustomerName = @customerName and StartTime >= @startTime");
                    cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                    cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                    if (!string.IsNullOrWhiteSpace(MESStationName))
                    {
                        sqlSB.Append(" and StationName = @stationName");
                        cmd.Parameters.Add(new SqlParameter("@stationName", MESStationName));
                    }
                    if (endTime.HasValue)
                    {
                        sqlSB.Append(" and StartTime <= @endTime");
                        cmd.Parameters.Add(new SqlParameter("@endTime", endTime.Value));
                    }
                    sqlSB.Append(" group by stationName");
                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                //获取字段信息
                                keyValue.Add(read["stationName"].ToString(), int.Parse(read["count"].ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
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

        public Dictionary<string, GL_HoureOutputModel> GetNewOutput(string MESCustomerName, string MESStationName, DateTime startTime, DateTime? endTime, int Plant_Organization_UID)
        {
            var keyValue = new Dictionary<string, GL_HoureOutputModel>();
            string connectionStrings = "";
            if (Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }

            using (SqlConnection conn = new SqlConnection(connectionStrings))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    var sqlSB = new StringBuilder("select stationName,LineName,count = count(*) from wipevent where Status = 'Pass' and CustomerName = @customerName  and StartTime >= @startTime");
                    cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                    cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                    if (!string.IsNullOrWhiteSpace(MESStationName))
                    {
                        sqlSB.Append(" and StationName = @stationName");
                        cmd.Parameters.Add(new SqlParameter("@stationName", MESStationName));
                    }
                    if (endTime.HasValue)
                    {
                        sqlSB.Append(" and StartTime <= @endTime");
                        cmd.Parameters.Add(new SqlParameter("@endTime", endTime.Value));
                    }

                    sqlSB.Append(" group by stationName,LineName");
                    cmd.CommandText = sqlSB.ToString();
                    try
                    {
                        conn.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())//读取每行数据
                            {
                                var model = new GL_HoureOutputModel();
                                model.StationName = read["stationName"].ToString();
                                model.LineName = read["LineName"].ToString();
                                model.Count = int.Parse(read["count"].ToString());
                                //获取字段信息
                                keyValue.Add(read["stationName"].ToString()+ read["LineName"].ToString(), model);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
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

        public int GetLineOutput(int lineID, DateTime startTime, DateTime? endTime)
        {
            var count = 0;
            var line = DataContext.GL_Line.FirstOrDefault(l => l.LineID == lineID && l.IsEnabled == true);
            if (line != null)
            {
                string connectionStrings = "";
                if (line.Plant_Organization_UID == 1)
                {
                    connectionStrings = connStrCTU;
                }
                else if (line.Plant_Organization_UID == 35)
                {
                    connectionStrings = connStr;
                }
                //获取产线的输出工站
                // var outputStation = line.GL_Station.FirstOrDefault(s => s.IsOutput == true && s.IsEnabled == true);
                if (!string.IsNullOrEmpty(connectionStrings))
                {

                    var outputStation = GetStationDTOIsOutput(line.LineID);

                    if (outputStation != null)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionStrings))
                        {
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                var sqlSB = new StringBuilder("select count = count(*) from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StationName = @stationName and StartTime >= @startTime");
                                cmd.Parameters.Add(new SqlParameter("@customerName", line.System_Project.MESProject_Name));
                                cmd.Parameters.Add(new SqlParameter("@lineName", line.MESLineName));
                                cmd.Parameters.Add(new SqlParameter("@stationName", outputStation.MESStationName));
                                cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                                if (endTime.HasValue)
                                {
                                    sqlSB.Append(" and StartTime <= @endTime");
                                    cmd.Parameters.Add(new SqlParameter("@endTime", endTime.Value));
                                }
                                cmd.CommandText = sqlSB.ToString();
                                try
                                {
                                    conn.Open();
                                    count = int.Parse(cmd.ExecuteScalar().ToString());
                                }
                                catch (Exception ex)
                                {
                                    throw;
                                }
                                finally
                                {
                                    cmd.Dispose();
                                    conn.Close();
                                }
                            }
                        }
                    }
                }
            }

            return count;
        }

        public GL_StationDTO GetStationDTOIsOutput(int LineID)
        {
            var query = from w in DataContext.GL_Station
                        where w.IsGoldenLine == true && w.GL_Line.IsEnabled == true
                        select new GL_StationDTO
                        {

                            StationID = w.StationID,
                            StationName = w.StationName,
                            LineID = w.LineID,
                            IsBirth = w.IsBirth,
                            IsOutput = w.IsOutput,
                            IsTest = w.IsTest,
                            Seq = w.Seq,
                            IsEnabled = w.IsEnabled,
                            CustomerID = w.GL_Line.CustomerID,
                            ProjectName = w.GL_Line.System_Project.Project_Name,
                            LineName = w.GL_Line.LineName,
                            LineIsEnabled = w.GL_Line.IsEnabled,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            LineCycleTime = w.GL_Line.CycleTime,
                            CycleTime = w.CycleTime,
                            MESStationName = w.MESStationName,
                            MESLineName = w.GL_Line.MESLineName,
                            MESProjectName = w.GL_Line.System_Project.MESProject_Name,
                        };

            query = query.Where(o => o.IsEnabled == true);
            query = query.Where(o => o.IsOutput == true);
            query = query.Where(o => o.LineID == LineID);
            return query.FirstOrDefault();

        }

        public EnumLineStatus GetLineStatus(int lineID)
        {
            var lastestTime = new DateTime();
            var lineStatus = EnumLineStatus.DownTime;
            var line = DataContext.GL_Line.FirstOrDefault(l => l.LineID == lineID && l.IsEnabled == true);
            //判空
            if (line != null)
            {
                //根据Plant_Organization_UID 获取链接字符串
                string connectionStrings = "";
                if (line.Plant_Organization_UID == 1)
                {
                    connectionStrings = connStrCTU;
                }
                else if (line.Plant_Organization_UID == 35)
                {
                    connectionStrings = connStr;
                }
                if (!string.IsNullOrEmpty(connectionStrings))
                {
                    //获取产线的输出工站
                    using (SqlConnection conn = new SqlConnection(connectionStrings))
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            var sqlSB = "select max(StartTime) from wipevent where StartTime > dateadd(day,-1,getdate()) and Status = 'Pass' and CustomerName = @customerName and LineName = @lineName";
                            cmd.Parameters.Add(new SqlParameter("@customerName", line.System_Project.MESProject_Name));
                            cmd.Parameters.Add(new SqlParameter("@lineName", line.MESLineName));
                            cmd.CommandText = sqlSB;
                            try
                            {
                                conn.Open();
                                var sqlResult = cmd.ExecuteScalar().ToString();
                                if (!string.IsNullOrEmpty(sqlResult))
                                {
                                    lastestTime = DateTime.Parse(sqlResult);
                                    var spanSeconds = (decimal)(DateTime.Now - lastestTime).TotalSeconds;
                                    //Project report : status highlight green, yellow and red based on >40x, 30x-40x, <=30x
                                    if (spanSeconds > (line.CycleTime * 40))
                                    {
                                        lineStatus = EnumLineStatus.DownTime;
                                    }
                                    else if (spanSeconds > (line.CycleTime * 30))
                                    {
                                        lineStatus = EnumLineStatus.Delay;
                                    }
                                    else
                                    {
                                        lineStatus = EnumLineStatus.Normal;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                            finally
                            {
                                cmd.Dispose();
                                conn.Close();
                            }
                        }
                    }
                }
            }
            return lineStatus;
        }

        /// <summary>
        /// 获取指定时间点的产线状态
        /// </summary>
        /// <param name="lineID"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public EnumLineStatus GetLineStatus(int lineID, DateTime dateTime)
        {
            var lastestTime = new DateTime();
            var lineStatus = EnumLineStatus.DownTime;
            var line = DataContext.GL_Line.FirstOrDefault(l => l.LineID == lineID && l.IsEnabled == true);
            //判空
            if (line != null)
            {
                //根据Plant_Organization_UID 获取链接字符串
                string connectionStrings = "";
                if (line.Plant_Organization_UID == 1)
                {
                    connectionStrings = connStrCTU;
                }
                else if (line.Plant_Organization_UID == 35)
                {
                    connectionStrings = connStr;
                }
                if (!string.IsNullOrEmpty(connectionStrings))
                {
                    //获取产线的输出工站
                    using (SqlConnection conn = new SqlConnection(connectionStrings))
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            var sqlSB = "select max(StartTime) from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime <= @dateTime";
                            cmd.Parameters.Add(new SqlParameter("@customerName", line.System_Project.Project_Name));
                            cmd.Parameters.Add(new SqlParameter("@lineName", line.MESLineName));
                            cmd.Parameters.Add(new SqlParameter("@dateTime", dateTime));
                            cmd.CommandText = sqlSB;
                            try
                            {
                                conn.Open();
                                var sqlResult = cmd.ExecuteScalar().ToString();
                                if (!string.IsNullOrEmpty(sqlResult))
                                {
                                    lastestTime = DateTime.Parse(sqlResult);
                                    var spanSeconds = (decimal)(dateTime - lastestTime).TotalSeconds;
                                    if (spanSeconds > (line.CycleTime * 3))
                                    {
                                        lineStatus = EnumLineStatus.DownTime;
                                    }
                                    else if (spanSeconds >= (line.CycleTime * 2))
                                    {
                                        lineStatus = EnumLineStatus.Delay;
                                    }
                                    else
                                    {
                                        lineStatus = EnumLineStatus.Normal;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                            finally
                            {
                                cmd.Dispose();
                                conn.Close();
                            }
                        }
                    }
                }
            }
            return lineStatus;
        }

        /// <summary>
        /// 获取开始时间
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="lineName"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DateTime? GetStartTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime startTime, DateTime? endTime)
        {
            DateTime? resultStartTime = null;

            //根据Plant_Organization_UID 获取连接字符串
            string connectionStrings = "";
            if (Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }
            if (!string.IsNullOrEmpty(connectionStrings))
            {

                using (SqlConnection conn = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (endTime.HasValue)
                        {
                            cmd.CommandText = "select min(StartTime) from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime >= @startTime and StartTime <= @endTime";
                            cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                            cmd.Parameters.Add(new SqlParameter("@lineName", MESLineName));
                            cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                            cmd.Parameters.Add(new SqlParameter("@endTime", endTime));
                        }
                        else
                        {
                            cmd.CommandText = "select min(StartTime) from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime >= @startTime";
                            cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                            cmd.Parameters.Add(new SqlParameter("@lineName", MESLineName));
                            cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                        }
                        try
                        {
                            conn.Open();
                            var strStartTime = cmd.ExecuteScalar().ToString();
                            if (strStartTime != null && strStartTime != "")
                            {
                                resultStartTime = DateTime.Parse(strStartTime);
                            }
                            //resultStartTime = DateTime.Parse(cmd.ExecuteScalar().ToString());
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return resultStartTime;
        }

        public double GetRunTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime)
        {
            var count = 0;
            decimal resultRunTimeSeconds = 0;

            //根据Plant_Organization_UID 获取连接字符串
            string connectionStrings = "";
            if (Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }
            if (!string.IsNullOrEmpty(connectionStrings))
            {

                using (SqlConnection conn = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (endTime.HasValue)
                        {
                            cmd.CommandText = "select * from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime >= @startTime and StartTime <= @endTime order by StartTime asc";
                            cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                            cmd.Parameters.Add(new SqlParameter("@lineName", MESLineName));
                            cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                            cmd.Parameters.Add(new SqlParameter("@endTime", endTime.Value));
                        }
                        else
                        {
                            cmd.CommandText = "select * from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime >= @startTime order by StartTime asc";
                            cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                            cmd.Parameters.Add(new SqlParameter("@lineName", MESLineName));
                            cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                        }
                        try
                        {
                            conn.Open();
                            DateTime? prevousStartTime = null;
                            DateTime currentStartTime = new DateTime();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    count++;
                                    currentStartTime = (DateTime)read["StartTime"];
                                    if (prevousStartTime.HasValue)
                                    {
                                        TimeSpan interval = currentStartTime - prevousStartTime.Value;
                                        decimal intervalSeconds = Convert.ToDecimal(interval.TotalSeconds);
                                        if (intervalSeconds <= (STDCT * 3))
                                        {
                                            resultRunTimeSeconds += intervalSeconds;
                                        }
                                    }
                                    prevousStartTime = currentStartTime;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return Convert.ToDouble(resultRunTimeSeconds);
        }

        public double GetUnPlanDownTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, decimal STDCT, DateTime startTime, DateTime? endTime)
        {
            var count = 0;
            double resultUpPlanDownTimeSeconds = 0;

            //根据Plant_Organization_UID 获取连接字符串
            string connectionStrings = "";
            if (Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }
            if (!string.IsNullOrEmpty(connectionStrings))
            {
                using (SqlConnection conn = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (endTime.HasValue)
                        {
                            cmd.CommandText = "select * from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime >= @startTime and StartTime <= @endTime order by StartTime asc";
                            cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                            cmd.Parameters.Add(new SqlParameter("@lineName", MESLineName));
                            cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                            cmd.Parameters.Add(new SqlParameter("@endTime", endTime.Value));
                        }
                        else
                        {
                            cmd.CommandText = "select * from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime >= @startTime order by StartTime asc";
                            cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                            cmd.Parameters.Add(new SqlParameter("@lineName", MESLineName));
                            cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                        }
                        try
                        {
                            conn.Open();
                            DateTime? prevousStartTime = null;
                            DateTime currentStartTime = new DateTime();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    count++;
                                    currentStartTime = (DateTime)read["StartTime"];
                                    if (prevousStartTime.HasValue)
                                    {
                                        TimeSpan interval = currentStartTime - prevousStartTime.Value;
                                        var intervalSeconds = interval.TotalSeconds;
                                        if (intervalSeconds > Convert.ToDouble(STDCT * 3))
                                        {
                                            resultUpPlanDownTimeSeconds += intervalSeconds;
                                        }
                                    }
                                    prevousStartTime = currentStartTime;
                                }
                            }
                            if (endTime.HasValue && prevousStartTime.HasValue)
                            {
                                //加上最后一片料的扫码时间到班次结束时间的时间差
                                var now = DateTime.Now;
                                if (now < endTime.Value)
                                {
                                    resultUpPlanDownTimeSeconds += (now - prevousStartTime.Value).TotalSeconds;
                                }
                                else
                                {
                                    resultUpPlanDownTimeSeconds += (endTime.Value - prevousStartTime.Value).TotalSeconds;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return resultUpPlanDownTimeSeconds;
        }

        public DateTime GetLastUpdateTime(string MESCustomerName, string MESLineName, int Plant_Organization_UID, DateTime flagTime)
        {
            var resultLastUpdateTime = new DateTime();
            //根据Plant_Organization_UID 获取连接字符串
            string connectionStrings = "";
            if (Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }
            if (!string.IsNullOrEmpty(connectionStrings))
            {
                using (SqlConnection conn = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select max(StartTime) from wipevent where Status = 'Pass' and CustomerName = @customerName and LineName = @lineName and StartTime <= @flagTime";
                        cmd.Parameters.Add(new SqlParameter("@customerName", MESCustomerName));
                        cmd.Parameters.Add(new SqlParameter("@lineName", MESLineName));
                        cmd.Parameters.Add(new SqlParameter("@startTime", flagTime));
                        try
                        {
                            conn.Open();
                            resultLastUpdateTime = DateTime.Parse(cmd.ExecuteScalar().ToString());
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return resultLastUpdateTime;
        }

        public List<SystemProjectDTO> GetAllEnGL_Customer()
        {
            var query = from w in DataContext.System_Project
                        select new SystemProjectDTO
                        {
                            Project_UID = w.Project_UID,
                            Project_Code = w.Project_Code,
                            BU_D_UID = w.BU_D_UID,
                            Project_Name = w.Project_Name,
                            MESProject_Name = w.MESProject_Name,
                            Product_Phase = w.Product_Phase,
                            Start_Date = w.Start_Date,
                            Closed_Date = w.Closed_Date,
                            OP_TYPES = w.OP_TYPES,
                            Organization_UID = w.Organization_UID,
                            Project_Type = w.Project_Type,
                            Modified_UID = w.Modified_UID,
                            Modified_Date = w.Modified_Date
                        };

            return query.ToList();
        }

        public List<GL_WIPEventDTO> GetGL_WIPEvent(string customerName, string stationName, int Plant_Organization_UID, DateTime startTime, DateTime? endTime)
        {
            var keyValue = new Dictionary<string, int>();
            List<GL_WIPEventDTO> WIPEventDTOList = new List<GL_WIPEventDTO>();

            //根据Plant_Organization_UID 获取连接字符串
            string connectionStrings = "";
            if (Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }
            if (!string.IsNullOrEmpty(connectionStrings))
            {

                #region
                using (SqlConnection conn = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        var sqlSB = new StringBuilder("'select CustomerName,count = count(*) from wipevent where Status = 'Pass' and CustomerName = @customerName and StartTime >= @startTime'");
                        cmd.Parameters.Add(new SqlParameter("@customerName", customerName));
                        cmd.Parameters.Add(new SqlParameter("@startTime", startTime));
                        if (!string.IsNullOrWhiteSpace(stationName))
                        {
                            sqlSB.Append(" and StationName = @stationName");
                            cmd.Parameters.Add(new SqlParameter("@stationName", stationName));
                        }
                        if (endTime.HasValue)
                        {
                            sqlSB.Append(" and StartTime <= @endTime");
                            cmd.Parameters.Add(new SqlParameter("@endTime", endTime.Value));
                        }
                        sqlSB.Append(" group by CustomerName");
                        cmd.CommandText = sqlSB.ToString();
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    //获取字段信息
                                    WIPEventDTOList.Add(new GL_WIPEventDTO()
                                    {
                                        CustomerID = int.Parse(read["CustomerName"].ToString()),
                                        LineID = int.Parse(read["LineID"].ToString()),
                                        StationID = int.Parse(read["StationID"].ToString()),
                                        outPutCount = int.Parse(read["count"].ToString()),

                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            #endregion
            return WIPEventDTOList;
        }

        /// <summary>
        /// WIP的Export
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<WipEventModel> ExportWIPHourOutput(GL_WIPHourOutputDTO search)
        {
            var keyValue = new Dictionary<string, int>();
            List<WipEventModel> WIPEventDTOList = new List<WipEventModel>();
            //根据Plant_Organization_UID 获取连接字符串
            string connectionStrings = "";
            if (search.Plant_Organization_UID == 1)
            {
                connectionStrings = connStrCTU;
            }
            else if (search.Plant_Organization_UID == 35)
            {
                connectionStrings = connStr;
            }
            if (!string.IsNullOrEmpty(connectionStrings))
            {
                #region
                using (SqlConnection conn = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        var sqlSB = new StringBuilder(@"SELECT
                                                           [CustomerId]
                                                          ,[CustomerName]
                                                          ,[LineId]
                                                          ,[LineName]
                                                          ,[StationId]
                                                          ,[StationName]
                                                          ,[MachineName]
                                                          ,[SerialNumber]
                                                          ,[StartTime]
                                                           FROM [dbo].[wipevent] where StartTime>=@StartTime and StartTime<=@EndTime ");
                        cmd.Parameters.Add(new SqlParameter("@CustomerName", search.MESCustomerName));
                        cmd.Parameters.Add(new SqlParameter("@LineName", search.MESLineName));
                        cmd.Parameters.Add(new SqlParameter("@StartTime", search.StartTime.ToString("yyyy-MM-dd HH:mm")));
                        cmd.Parameters.Add(new SqlParameter("@EndTime", search.EndTime.ToString("yyyy-MM-dd HH:mm")));

                        if (search.CustomerID != 0)
                        {
                            sqlSB.Append(" and CustomerName = @CustomerName");
                        }

                        if (search.LineID != 0)
                        {
                            sqlSB.Append(" and LineName = @LineName");
                        }

                        cmd.CommandText = sqlSB.ToString();
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    //获取字段信息
                                    WIPEventDTOList.Add(new WipEventModel()
                                    {
                                        CustomerId = int.Parse(read["CustomerId"].ToString()),
                                        CustomerName = read["CustomerName"].ToString(),
                                        LineId = int.Parse(read["LineId"].ToString()),
                                        LineName = read["LineName"].ToString(),
                                        StationId = int.Parse(read["StationId"].ToString()),
                                        StationName = read["StationName"].ToString(),
                                        SerialNumber = read["SerialNumber"].ToString(),
                                        StartTime = DateTime.Parse(read["StartTime"].ToString()),
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            #endregion
            return WIPEventDTOList;
        }

        public List<OEE_UserStationDTO> QueryOperatorList(int LineID)
        {


            var query = from w in DataContext.OEE_UserStation
                        where w.GL_Station.IsOEE == true && w.GL_Station.IsEnabled == true
                        select new OEE_UserStationDTO
                        {
                            OEE_UserStation_UID = w.OEE_UserStation_UID,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            KeyInNG_User_UID = w.KeyInNG_User_UID,
                            Project_UID = w.GL_Station.GL_Line.CustomerID,
                            Project_Name = w.GL_Station.GL_Line.System_Project.Project_Name,
                            Line_ID = w.GL_Station.LineID,
                            Line_Name = w.GL_Station.GL_Line.LineName,
                            StationID = w.StationID,
                            Station_Name = w.GL_Station.StationName,
                            User_NTID = w.System_Users1.User_NTID,
                            Modified_UID = w.Modified_UID,
                            Modified_name = w.System_Users.User_Name,
                            Modified_Date = w.Modified_Date,
                            Plant_Organization = w.System_Organization.Organization_Name,
                            BG_Organization = w.System_Organization1.Organization_Name,
                            FunPlant_Organization = w.System_Organization2.Organization_Name
                        };
            if (LineID != 0)
            {
                query = query.Where(o => o.Line_ID == LineID);
            }
            return query.ToList();

        }

        /// <summary>
        /// 根据专案获取用户信息
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public List<OEE_UserStationDTO> GetOperatorList(int CustomerID)
        {


            var query = from w in DataContext.OEE_UserStation
                        where w.GL_Station.IsOEE == true && w.GL_Station.IsEnabled == true
                        select new OEE_UserStationDTO
                        {
                            OEE_UserStation_UID = w.OEE_UserStation_UID,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            KeyInNG_User_UID = w.KeyInNG_User_UID,
                            Project_UID = w.GL_Station.GL_Line.CustomerID,
                            Project_Name = w.GL_Station.GL_Line.System_Project.Project_Name,
                            Line_ID = w.GL_Station.LineID,
                            Line_Name = w.GL_Station.GL_Line.LineName,
                            StationID = w.StationID,
                            Station_Name = w.GL_Station.StationName,
                            User_NTID = w.System_Users1.User_NTID,
                            Modified_UID = w.Modified_UID,
                            Modified_name = w.System_Users.User_Name,
                            Modified_Date = w.Modified_Date,
                            Plant_Organization = w.System_Organization.Organization_Name,
                            BG_Organization = w.System_Organization1.Organization_Name,
                            FunPlant_Organization = w.System_Organization2.Organization_Name
                        };
            if (CustomerID != 0)
            {
                query = query.Where(o => o.Project_UID == CustomerID);
            }
            return query.ToList();

        }

        public List<SystemUserOEEDTO> GetAllUserByDTOs(int Plant_OrganizationUID)
        {
            string sql = string.Format(@"SELECT a.*, ISNULL(b.Plant_OrganizationUID,0) as  Plant_OrganizationUID  FROM System_Users a  LEFT JOIN System_UserOrg b   ON a.Account_UID =b.Account_UID  WHERE ISNULL(b.Plant_OrganizationUID,0) IN (0,{0})  ", Plant_OrganizationUID);
            // sql = string.Format(sql, Plant_OrganizationUID);
            var dbList = DataContext.Database.SqlQuery<SystemUserOEEDTO>(sql).ToList();
            return dbList;
        }

        public string ImportOperatorList(List<OEE_UserStationDTO> OperatorLists)
        {
            try
            {
                using (var trans = DataContext.Database.BeginTransaction())
                {
                    //全插操作
                    StringBuilder sb = new StringBuilder();
                    if (OperatorLists != null && OperatorLists.Count > 0)
                    {
                        var SystemRole = GetSystemRoleDTOs("OEEDefectInput");
                        if (SystemRole != null && SystemRole.Count > 0)
                        {
                            //获取已经设置的用户
                            var UserStationList = GetUserStationList();
                            //获取已经设置了角色的用户
                            int Role_UID = SystemRole.FirstOrDefault().Role_UID;
                            var SystemUserRoleDTOs = GetSystemUserRoleDTOs(Role_UID);


                            string sqlUserRode = "";

                            List<int> UserIDs = new List<int>();
                            foreach (var item in OperatorLists)
                            {
                                //设置菜单
                                if (!UserIDs.Contains(item.KeyInNG_User_UID))
                                {
                                    //获取当前站点的用户
                                    var UserStation = UserStationList.FirstOrDefault(o => o.Plant_Organization_UID == item.Plant_Organization_UID && o.BG_Organization_UID == item.BG_Organization_UID && o.StationID == item.StationID);
                                    if (UserStation != null)
                                    {
                                        //删除当前存在的用户角色，然后添加新用户。
                                        sqlUserRode += string.Format(@"delete from System_User_Role where Account_UID={0} and  Role_UID={1} ;", UserStation.KeyInNG_User_UID, Role_UID);
                                    }

                                    var systemUserRoleDTOs = SystemUserRoleDTOs.Where(o => o.Account_UID == item.KeyInNG_User_UID);
                                    if (systemUserRoleDTOs != null && systemUserRoleDTOs.Count() > 0)
                                    {

                                        //删除当前存在的用户角色，然后添加新用户。
                                        sqlUserRode += string.Format(@"delete from System_User_Role where Account_UID={0} and  Role_UID={1} ;", item.KeyInNG_User_UID, Role_UID);
                                        //然后添加新用户角色。
                                        sqlUserRode += string.Format(@"  INSERT INTO System_User_Role
                                                                               (Account_UID
                                                                               ,Role_UID
                                                                               ,Modified_UID
                                                                               ,Modified_Date)
                                                                         VALUES
                                                                               ({0}
                                                                               ,{1}
                                                                               ,{2}
                                                                               ,'{3}') ;", item.KeyInNG_User_UID, Role_UID, item.Modified_UID, item.Modified_Date);



                                        UserIDs.Add(item.KeyInNG_User_UID);
                                    }
                                    else
                                    {
                                        //然后添加新用户角色。
                                        sqlUserRode += string.Format(@"  INSERT INTO System_User_Role
                                                                               (Account_UID
                                                                               ,Role_UID
                                                                               ,Modified_UID
                                                                               ,Modified_Date)
                                                                         VALUES
                                                                               ({0}
                                                                               ,{1}
                                                                               ,{2}
                                                                               ,'{3}') ;", item.KeyInNG_User_UID, Role_UID, item.Modified_UID, item.Modified_Date);

                                        UserIDs.Add(item.KeyInNG_User_UID);

                                    }

                                }
                                if (item.OEE_UserStation_UID == 0)
                                {
                                    //构造插入SQL数据
                                    var insertSql = string.Format(@"INSERT INTO OEE_UserStation
                                                                       (Plant_Organization_UID
                                                                       ,BG_Organization_UID
                                                                       ,FunPlant_Organization_UID
                                                                       ,StationID
                                                                       ,KeyInNG_User_UID
                                                                       ,Modified_UID
                                                                       ,Modified_Date)
                                                                 VALUES
                                                                       ({0}
                                                                       ,{1}
                                                                       ,{2}
                                                                       ,{3}
                                                                       ,{4}
                                                                       ,{5}
                                                                       ,'{6}') ;",
                                                                   item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99, item.StationID, item.KeyInNG_User_UID, item.Modified_UID, item.Modified_Date);

                                    insertSql = insertSql.Replace("-99", "NULL");
                                    sb.AppendLine(insertSql);
                                }
                                else
                                {

                                    var updateSql = string.Format(@" UPDATE OEE_UserStation
                                                               SET Plant_Organization_UID = {0}
                                                                  ,BG_Organization_UID = {1}
                                                                  ,FunPlant_Organization_UID = {2}
                                                                  ,StationID = {3}
                                                                  ,KeyInNG_User_UID = {4}
                                                                  ,Modified_UID = {5}
                                                                  ,Modified_Date = '{6}'
                                                             WHERE OEE_UserStation_UID={7} ;",
                                                                item.Plant_Organization_UID, item.BG_Organization_UID, item.FunPlant_Organization_UID != null ? item.FunPlant_Organization_UID : -99, item.StationID, item.KeyInNG_User_UID, item.Modified_UID, item.Modified_Date, item.OEE_UserStation_UID);

                                    updateSql = updateSql.Replace("-99", "NULL");
                                    sb.AppendLine(updateSql);

                                }
                            }
                            string sql = sqlUserRode + sb.ToString();
                            // string sql =  sb.ToString();
                            DataContext.Database.ExecuteSqlCommand(sql);
                            trans.Commit();
                        }
                        else
                        {
                            return "请先设置 OEE NG Accout 角色,然后才能导入人员。";
                        }
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }



        /// <summary>
        /// 根据角色ID获取角色实体 OEEDefectInput
        /// </summary>
        /// <param name="Role_ID"></param>
        /// <returns></returns>
        private List<SystemRoleDTO> GetSystemRoleDTOs(string Role_ID)
        {
            string sql = string.Format(@" select  Role_UID
                                           ,Role_ID
                                          ,Role_Name
                                          ,Modified_UID
                                          ,Modified_Date
                                          ,Father_Role_ID
	                                      from System_Role where Role_ID='{0}' ", Role_ID);

            var dbList = DataContext.Database.SqlQuery<SystemRoleDTO>(sql).ToList();
            return dbList;
        }
        /// <summary>
        /// 获取所有站点的用户信息
        /// </summary>
        /// <returns></returns>
        private List<OEE_UserStationDTO> GetUserStationList()
        {


            var query = from w in DataContext.OEE_UserStation
                        where w.GL_Station.IsOEE == true && w.GL_Station.IsEnabled == true
                        select new OEE_UserStationDTO
                        {
                            OEE_UserStation_UID = w.OEE_UserStation_UID,
                            Plant_Organization_UID = w.Plant_Organization_UID,
                            BG_Organization_UID = w.BG_Organization_UID,
                            FunPlant_Organization_UID = w.FunPlant_Organization_UID,
                            KeyInNG_User_UID = w.KeyInNG_User_UID,
                            Project_UID = w.GL_Station.GL_Line.CustomerID,
                            Project_Name = w.GL_Station.GL_Line.System_Project.Project_Name,
                            Line_ID = w.GL_Station.LineID,
                            Line_Name = w.GL_Station.GL_Line.LineName,
                            StationID = w.StationID,
                            Station_Name = w.GL_Station.StationName,
                            User_NTID = w.System_Users1.User_NTID,
                            Modified_UID = w.Modified_UID,
                            Modified_name = w.System_Users.User_Name,
                            Modified_Date = w.Modified_Date,
                            Plant_Organization = w.System_Organization.Organization_Name,
                            BG_Organization = w.System_Organization1.Organization_Name,
                            FunPlant_Organization = w.System_Organization2.Organization_Name
                        };
            return query.ToList();

        }
        /// <summary>
        /// 根据角色UID获取人员账号
        /// </summary>
        /// <param name="Role_UID"></param>
        /// <returns></returns>
        private List<SystemUserRoleDTO> GetSystemUserRoleDTOs(int Role_UID)
        {
            string sql = string.Format(@" SELECT System_User_Role_UID
                                                  ,Account_UID
                                                  ,Role_UID
                                                  ,Modified_UID
                                                  ,Modified_Date
                                              FROM System_User_Role where Role_UID={0} ", Role_UID);

            var dbList = DataContext.Database.SqlQuery<SystemUserRoleDTO>(sql).ToList();
            return dbList;
        }

    }
}
