using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels.Settings;
using System.Data.Entity;
using System.Collections.Generic;
using PDMS.Model.ViewModels.QualtyTrace;
using PDMS.Common.Helpers;

namespace PDMS.Data.Repository
{
    public class EnumerationRepository : RepositoryBase<Enumeration>, IEnumerationRepository
    {
        public EnumerationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// 取得当前时段(-2)或前一时段(-4) 2016-12-05 add by karl
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<string> GetSingleIntervalInfo(int type)
        {
            var strSql = @"
                DECLARE @nowdate NVARCHAR(20),@is_even BIT,@starthour INT,@endhour INT
                SET @nowdate=DATEADD(HOUR,{0},GETDATE());
                WITH one AS (SELECT CASE when  DATEPART(HOUR,@nowdate)%2=0 THEN DATEPART(HOUR,@nowdate) ELSE DATEPART(HOUR,@nowdate)-1
                END AS nowevenhour )
                SELECT @starthour=nowevenhour FROM one
                SELECT @endhour = @starthour+2;
                WITH one AS(
                SELECT CASE WHEN @starthour<10 THEN '0'+CONVERT(NVARCHAR(20),@starthour)
                +':00-' ELSE CONVERT(NVARCHAR(20),@starthour) +':00-' END AS starthour,
                CASE WHEN @endhour<10 THEN '0'+CONVERT(NVARCHAR(20),@endhour)
                +':00' ELSE CONVERT(NVARCHAR(20),@endhour) +':00' END AS endhour)
                SELECT REPLACE(starthour+endhour,'24:00','00:00') from one";
            strSql = string.Format(strSql, type);
            var dbList = DataContext.Database.SqlQuery<string>(strSql).ToList();
            return dbList;
        }
        /// <summary>
        /// 获取当前时段及日期
        /// </summary>
        /// <param name="opType"></param>
        /// <param name="Time_Interval"></param>
        /// <returns></returns>
        public List<IntervalEnum> GetIntervalInfo(string opType, string InputPut_Interval = "")
        {
            var opEnumType = "Time_Interval_" + opType;
            var nowTime = DateTime.Now.ToString(FormatConstants.DateTimeFormatStringByDate);
            //获取当前时间所在时段
            //            var strSql = @"DECLARE @AddLeftTime INT,@AddRightTime INT ,@NowDate NVARCHAR(20),@GETDATE DATETIME

            //SET @GETDATE=GETDATE()

            //SET @AddLeftTime=(select CONVERT(INT, Enum_Value)Enum_Value from
            //              dbo.Enumeration
            //              WHERE Enum_Type='Time_InterVal_ADD'
            //              AND Enum_Name='Time_InterVal_LEFTOP1')
            //SET @AddRightTime=(select CONVERT(INT, Enum_Value)Enum_Value from
            //              dbo.Enumeration
            //              WHERE Enum_Type='Time_InterVal_ADD'
            //              AND Enum_Name='Time_InterVal_RIGHTOP1')
            //SET @NowDate=( SELECT CASE WHEN DATEDIFF(MINUTE,@GETDATE,DivTime)>0 THEN CONVERT(varchar(100), DATEADD(DAY,-1,@GETDATE), 23) ELSE CONVERT(varchar(100), @GETDATE, 23) END AS nowDate
            //FROM (SELECT DATEADD(MINUTE,@AddLeftTime,CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 23)+' '+SUBSTRING(Enum_Value,0,6) +':00'))DivTime from
            //dbo.Enumeration AS e WHERE Enum_Type='Time_InterVal_OP1' AND Enum_Name='1' )divtemp)           

            // select Enum_Type OpEnumType,@NowDate NowDate,CAST(ORDERID AS NVARCHAR(20)) IntervalNo,Enum_Value Time_Interval,@GETDATE BeginTime,@GETDATE EndTime from
            // (
            // select beginminnute,CASE WHEN beginminnute>endminnute THEN endminnute+60*24 ELSE endminnute END endminnute,nowMinute,Enum_Value,orderID,Enum_Type from
            // (
            // select CASE WHEN DATEDIFF(MINUTE,nowTime,beginTime)-60<0 THEN 24*60+DATEDIFF(MINUTE,nowTime,beginTime) ELSE DATEDIFF(MINUTE,nowTime,beginTime) END beginminnute,
            // CASE WHEN DATEDIFF(MINUTE,nowTime,endTime)-60<0 THEN 24*60+DATEDIFF(MINUTE,nowTime,endTime) ELSE DATEDIFF(MINUTE,nowTime,endTime) END endminnute,
            // CASE WHEN DATEDIFF(MINUTE,nowTime,@GETDATE)-60<0 THEN 24*60+DATEDIFF(MINUTE,nowTime,@GETDATE)ELSE DATEDIFF(MINUTE,nowTime,@GETDATE) END nowMinute,Enum_Value,orderID,Enum_Type from
            // (          
            //select DATEADD(MINUTE,@AddLeftTime,CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 23)+' '+SUBSTRING(Enum_Value,0,6) +':00'))beginTime,
            //DATEADD(MINUTE,@AddRightTime,CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 23)+' '+SUBSTRING(Enum_Value,7,6) +':00')) endTime
            //,Enum_Value,CONVERT(INT, enum_name) orderID,Enum_Type,
            //CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 20))nowTime
            //from
            //dbo.Enumeration AS e
            //WHERE Enum_Type='Time_Interval_OP1') m)mm)temp
            //WHERE temp.nowMinute>=temp.beginminnute
            //AND temp.nowMinute<temp.endminnute
            //ORDER BY orderID";
            var strSql = @"DECLARE @AddLeftTime INT,@AddRightTime INT ,@NowDate NVARCHAR(20),@GETDATE DATETIME
SET @GETDATE=GETDATE()

SET @AddLeftTime=(select CONVERT(INT, Enum_Value)Enum_Value from
              dbo.Enumeration
              WHERE Enum_Type='Time_InterVal_ADD'
              AND Enum_Name='Time_InterVal_LEFTOP1')
SET @AddRightTime=(select CONVERT(INT, Enum_Value)Enum_Value from
              dbo.Enumeration
              WHERE Enum_Type='Time_InterVal_ADD'
              AND Enum_Name='Time_InterVal_RIGHTOP1')
 
            ;WITH
            one AS --这一句取出当前时间的年月日+(6:00小时）+（90分）= 当前时间年月日+90分 = '2016-09-18 07:30:00.000'
 (
            SELECT DATEADD(MINUTE,@AddLeftTime,CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 23)+' '+SUBSTRING(Enum_Value,0,6) +':00'))DivTime from
            dbo.Enumeration AS e WHERE Enum_Type='{0}' AND Enum_Name='1'
            ), 
            two AS -- 设定现在的时间，如果当前时间小于7：30则为前一天时间，否则为当天时间
 (
            SELECT  CASE WHEN DATEDIFF(MINUTE,@GETDATE,DivTime)>0 THEN CONVERT(varchar(100), DATEADD(DAY,-1,@GETDATE), 23) 
            ELSE CONVERT(varchar(100), @GETDATE, 23) END AS nowDate
            FROM one
            )
            SELECT @NowDate=two.nowDate FROM two

            ----------------------------------------------------------------------------------

            ;WITH
            three AS -- 将当前时间设置每个2小时一个时间段的12笔数据都前后加上对应的延长时间段，好让用户有足够的时间输入数据
 (          
            SELECT DATEADD(MINUTE,@AddLeftTime,CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 23)+' '+SUBSTRING(Enum_Value,0,6) +':00'))beginTime,
DATEADD(MINUTE,@AddRightTime,CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 23)+' '+SUBSTRING(Enum_Value,7,6) +':00')) endTime
,Enum_Value,CONVERT(INT, enum_name) orderID,Enum_Type,
CONVERT(DATETIME,CONVERT(varchar(100), @GETDATE, 20))nowTime
from
            dbo.Enumeration
            WHERE Enum_Type='{0}'
            ),
            four AS -- 做差值计算，那24*60作为一个中间点，减去90的就算量，这里的减去90和减去其他数效果都一样，有点怪
            (
            SELECT three.beginTime,three.endTime, three.nowTime,
            CASE 
	            WHEN DATEDIFF(MINUTE,nowTime,beginTime)-90<0 THEN 24*60+DATEDIFF(MINUTE,nowTime,beginTime) 
	            ELSE DATEDIFF(MINUTE,nowTime,beginTime) END beginminnute,
            CASE 
	            WHEN DATEDIFF(MINUTE,nowTime,endTime)-90<0 THEN 24*60+DATEDIFF(MINUTE,nowTime,endTime) 
	            ELSE DATEDIFF(MINUTE,nowTime,endTime) END endminnute,
            CASE 
	            WHEN DATEDIFF(MINUTE,nowTime,@GETDATE)-90<0 THEN 24*60+DATEDIFF(MINUTE,nowTime,@GETDATE)
	            ELSE DATEDIFF(MINUTE,nowTime,@GETDATE) END nowMinute,
            Enum_Value,orderID,Enum_Type FROM three
            ),
            five AS
            (
            SELECT beginminnute,
            CASE 
	            WHEN beginminnute>endminnute THEN endminnute+60*24 
	            ELSE endminnute END endminnute,
            nowMinute,Enum_Value,orderID,Enum_Type 
            FROM four 
            ),
            six AS
            (
            select Enum_Type OpEnumType,@NowDate NowDate,CAST(ORDERID AS NVARCHAR(20)) IntervalNo,Enum_Value Time_Interval,@GETDATE BeginTime,@GETDATE EndTime
            FROM five WHERE nowMinute>=beginminnute AND nowMinute<endminnute
            )
            SELECT * FROM six";
            strSql = string.Format(strSql, opEnumType);
            var dbList = DataContext.Database.SqlQuery<IntervalEnum>(strSql).ToList();
            return dbList;
        }

        public List<Enumeration> GetIntervalOrder(string OP)
        {
            var strSql = @"select * from
                            dbo.Enumeration AS e
                            WHERE Enum_Type='Time_InterVal_" + OP + @"'
                            ORDER BY CONVERT(INT, enum_name)";
            var dbList = DataContext.Database.SqlQuery<Enumeration>(strSql).ToList();
            return dbList;
        }

        public List<string> GetEnumNamebyType(string enumType)
        {
            var query = from enumertion in DataContext.Enumeration
                        where enumertion.Enum_Type == enumType
                        select enumertion.Enum_Name;

            return query.Distinct().ToList();

        }

        public List<Enumeration> GetMES_Project(string enumType)
        {
            var query = from enumertion in DataContext.Enumeration
                        where enumertion.Enum_Type == enumType
                        select enumertion;

            return query.ToList();
        }

        public List<Enumeration> GetMesNeedDeleteStation(string enumType, string enum_Name)
        {
            var query = from enumertion in DataContext.Enumeration
                        where enumertion.Enum_Type == enumType && enumertion.Enum_Name == enum_Name
                        select enumertion;

            return query.ToList();
        }

        public List<string> GetEnumValuebyType(string enumType)
        {
            var query = from enumertion in DataContext.Enumeration
                        where enumertion.Enum_Type == enumType
                        select enumertion.Enum_Value;

            return query.Distinct().ToList();

        }

        public IQueryable<EnumerationDTO> GetEnumValueForKeyProcess(string project, string partTypes)
        {
            var query = from enumertion in DataContext.Enumeration
                        where enumertion.Enum_Type == "Report_Key_Process" && enumertion.Enum_Name == partTypes && enumertion.Decription == project
                        select new EnumerationDTO()
                        {
                            Enum_Name = enumertion.Enum_Name,
                            Enum_Value = enumertion.Enum_Value,
                            Enum_UID = enumertion.Enum_UID,
                            Decription = enumertion.Decription
                        };
            return query.Distinct();
        }
        public List<string> GetTypeClassfy(int userUID)
        {
            var query = from enumertion in DataContext.Enumeration

                        where enumertion.Enum_Type == "QAExceptionType" && enumertion.Enum_Name == "BC41"
                        select enumertion.Enum_Value;

            return query.Distinct().ToList();

        }

        public string UpdateItem(string safestock, string lastopeneqp, string plantopeneqp)
        {
            try
            {
                int result = 0;
                string sql = "update Enumeration set Enum_Value='{0}' where Enum_Name='{1}'";
                sql = string.Format(sql, safestock, "Safe_Stock_Max");
                result += DataContext.Database.ExecuteSqlCommand(sql);

                sql = "update Enumeration set Enum_Value='{0}' where Enum_Name='{1}'";
                sql = string.Format(sql, lastopeneqp, "Safe_Stock_LastOpenEQP");
                result += DataContext.Database.ExecuteSqlCommand(sql);

                sql = "update Enumeration set Enum_Value='{0}' where Enum_Name='{1}'";
                sql = string.Format(sql, plantopeneqp, "Safe_Stock_PlanOpenEQP");
                result += DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 3)
                    return "";
                return "维护安全库存参数失败";
            }
            catch (Exception e)
            {
                return "维护安全库存参数失败" + e.Message;
            }

        }

        public string DeleteByUid(int Enum_UID)
        {
            try
            {
                string sql = "delete  Enumeration where Enum_UID={0}";
                sql = string.Format(sql, Enum_UID);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "";
                else
                    return "删除故障原因及各类失败";
            }
            catch (Exception e)
            {
                return "删除故障原因及各类失败" + e.Message;
            }
        }
        public List<ProcessTargetInfo> GetProcessTargetInfo(int flowchart_MasterUID, string ProductDate)
        {
            DateTime Currentdate = DateTime.Parse(ProductDate);

            var query = from MG in DataContext.QualityAssurance_MgData
                        join FD in DataContext.FlowChart_Detail on MG.FlowChart_Detail_UID equals FD.FlowChart_Detail_UID

                        where FD.FlowChart_Master_UID == flowchart_MasterUID && MG.ProductDate == Currentdate
                        select new ProcessTargetInfo
                        {
                            Process = FD.Process,
                            FirstTargetYield = MG.FirstRejectionRate,
                            SecondTargetYield = MG.SecondRejectionRate
                        };
            return query.ToList();
        }
        public string GetMappingName(string TraceName)
        {
            var query = from P in DataContext.Enumeration
                        where P.Enum_Type == "Trace_Eboard" && P.Enum_Name == TraceName
                        select P.Enum_Value;
            return query.FirstOrDefault();
        }
        public ProjectInfo GetProjectInfo(int flowchart_MasterUID)
        {
            var query = from P in DataContext.System_Project
                        join Master in DataContext.FlowChart_Master
                        on P.Project_UID equals Master.Project_UID
                        where Master.FlowChart_Master_UID == flowchart_MasterUID
                        select new ProjectInfo
                        {
                            Project = P.Project_Name,
                            Part_Types = P.Project_Type
                        };
            return query.FirstOrDefault();
        }
        public string ExeSql(string SQLText)
        {
            string result = string.Empty;
            string sql = SQLText.Trim();
            int intExeNum;
            using (var trans = DataContext.Database.BeginTransaction())
            {

                try
                {
                    //if (sql.Substring(0, 6).IndexOf("DELETE") != -1 || sql.Substring(0, 6).IndexOf("UPDATE") != -1 || sql.Substring(0, 6).IndexOf("INSERT") != -1)
                    //{
                        intExeNum = DataContext.Database.ExecuteSqlCommand(sql);
                        result = "影响的行数为" + intExeNum;
                    //}
                   
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                trans.Commit();
            }
            return result;
        }

        public int GetProcessSeq(string Process, int flowchartMasterUID)
        {
            var query = from Detail in DataContext.FlowChart_Detail
                        where Detail.Process == Process && Detail.FlowChart_Master_UID == flowchartMasterUID
                        select Detail.Process_Seq;
            return query.FirstOrDefault();
        }
        public void DeleteEboardData(string ProjectName)
        {
            try
            {
                string sql = "SET NOCOUNT OFF  DELETE dbo.QEboardSum  WHERE Project='{0}'";
                sql = string.Format(sql, ProjectName);

                string sql1 = "SET NOCOUNT OFF  DELETE dbo.TopTenQeboard  WHERE Project='{0}'";
                sql1 = string.Format(sql1, ProjectName);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                int result1 = DataContext.Database.ExecuteSqlCommand(sql1);

            }
            catch (Exception e)
            {
                //存放日志文件  "删除故障原因及各类失败" + e.Message;
                Logger logger = new Logger("删除Trace电子看板数据");
                logger.Error("删除Trace电子看板数据", e);
            }
        }

        public List<EnumerationDTO> GetEnumList(string enumType)
        {
            var query = from enumertion in DataContext.Enumeration
                        select new EnumerationDTO
                        {
                            Enum_UID = enumertion.Enum_UID,
                            Enum_Type = enumertion.Enum_Type,
                            Enum_Name = enumertion.Enum_Name,
                            Enum_Value = enumertion.Enum_Value,
                            Decription = enumertion.Decription
                        };

            query = query.Where(o => o.Enum_Type == enumType);
            return query.ToList();

        }
    }
    public interface IEnumerationRepository : IRepository<Enumeration>
    {
        List<ProcessTargetInfo> GetProcessTargetInfo(int flowchart_MasterUID, string ProductDate);
        ProjectInfo GetProjectInfo(int flowchart_MasterUID);
        List<IntervalEnum> GetIntervalInfo(string opType, string InputPut_Interval = "");
        List<string> GetSingleIntervalInfo(int type);
        List<Enumeration> GetIntervalOrder(string OP);
        IQueryable<EnumerationDTO> GetEnumValueForKeyProcess(string project, string partTypes);
        List<string> GetEnumNamebyType(string enumType);
        List<EnumerationDTO> GetEnumList(string enumType);
        List<string> GetEnumValuebyType(string enumType);
        List<string> GetTypeClassfy(int userUID);
        string UpdateItem(string safestock, string lastopeneqp, string plantopeneqp);
        string DeleteByUid(int Enum_UID);
        string ExeSql(string SQLText);

        string GetMappingName(string TraceName);
        int GetProcessSeq(string Process, int flowchartMasterUID);
        /// <summary>
        /// 删除OP3Trace数据
        /// </summary>
        /// <param name="ProjectName"></param>
        void DeleteEboardData(string ProjectName);
    }
}
