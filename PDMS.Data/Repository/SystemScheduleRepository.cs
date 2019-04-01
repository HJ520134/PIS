using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model.ViewModels.Batch;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ISystemScheduleRepository : IRepository<System_Schedule>
    {
        //查询排程列表准备执行排程
        List<BatchExecVM> ExecBatch();

    }

    public class SystemScheduleRepository : RepositoryBase<System_Schedule>, ISystemScheduleRepository
    {
        public SystemScheduleRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        private static IDatabaseFactory _DatabaseFactory = new DatabaseFactory();
        SystemEmailMRepository emailRepository = new SystemEmailMRepository(_DatabaseFactory);
        

        public List<BatchExecVM> ExecBatch()
        {
            
            
            var Excetion_Email_UID = Convert.ToInt32(ConfigurationManager.AppSettings["Excetion_Email_UID"]);
            var functionName = StructConstants.BatchModuleName.EmailFunctionName;
            List<BatchExecVM> matchList = new List<BatchExecVM>();

            using (var context = new SPPContext())
            {
                try
                {

                    string strSql = @"SELECT B.*,C.Function_Name FROM dbo.System_Module A
                                JOIN dbo.System_Schedule B ON B.System_Module_UID = A.System_Module_UID
                                JOIN dbo.System_Function C ON B.Function_UID = C.Function_UID
                                WHERE A.Is_Enable = 1 AND B.Is_Enable = 1 AND C.Function_Name != '{0}'";

                    strSql = string.Format(strSql, functionName);
                    var list = context.Database.SqlQuery<BatchExecVM>(strSql).ToList();

                    //进行时间点对比
                    StringBuilder sb = new StringBuilder();
                    var nowDate = DateTime.Now;
                    DateTime? nextDate = null;
                    foreach (var item in list)
                    {
                        Excetion_Email_UID = item.System_Schedule_UID;

                        //年月日时分对比,因为WindowsService间隔是每分钟一次，所以不会有误差
                        if (
                            item.Next_Execution_Date.Year == nowDate.Year
                            && item.Next_Execution_Date.Month == nowDate.Month
                            && item.Next_Execution_Date.Day == nowDate.Day
                            && item.Next_Execution_Date.Hour == nowDate.Hour
                            && item.Next_Execution_Date.Minute == nowDate.Minute
                            )
                        {

                            matchList.Add(item);


                            if (item.Exec_Moment.ToUpper() == "Month_End".ToUpper())
                            {
                                var execTimeList = item.Exec_Time.Split(',').ToList();
                                var nowHour = nowDate.Hour;
                                var nowMinute = nowDate.Minute;
                                for (int i = 0; i < execTimeList.Count(); i++)
                                {
                                    var timeList = execTimeList[i].Split(':').ToList();
                                    var execTimeHour = Convert.ToInt32(timeList[0]);
                                    var execTimeMinute = Convert.ToInt32(timeList[1]);

                                    //如果当前的时间点不是最后执行的时间点
                                    if (nowHour == execTimeHour && nowMinute == execTimeMinute && (i != execTimeList.Count() - 1))
                                    {
                                        var nextTimeList = execTimeList[i + 1].Split(':').ToList();
                                        var nextExecTimeHour = Convert.ToInt32(nextTimeList[0]);
                                        var nextExecTimeMinute = Convert.ToInt32(nextTimeList[1]);
                                        nextDate = item.Next_Execution_Date.Date.AddHours(nextExecTimeHour).AddMinutes(nextExecTimeMinute);
                                        break;
                                    }
                                    //如果当前的时间点是最后执行的时间点
                                    if (nowHour == execTimeHour && nowMinute == execTimeMinute && (i == execTimeList.Count() - 1))
                                    {
                                        var firstTimeList = execTimeList[0].Split(':').ToList();
                                        var firstExecTimeHour = Convert.ToInt32(firstTimeList[0]);
                                        var firstExecTimeMinute = Convert.ToInt32(firstTimeList[1]);

                                        //下个月的第一天
                                        nextDate = item.Next_Execution_Date.AddDays(1);
                                        //下个月的最后一天
                                        nextDate = nextDate.Value.Date.AddDays(1 - nextDate.Value.Day).AddMonths(1).AddDays(-1).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                                        break;
                                    }

                                }
                                //下个月的第一天
                                //nextDate = item.Next_Execution_Date.AddDays(1);
                                //下个月的最后一天
                                //nextDate = nextDate.Value.AddDays(1 - nextDate.Value.Day).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                //更新System_Schedule表的数据
                                switch (item.Cycle_Unit)
                                {

                                    case "H": //按小时
                                        nextDate = emailRepository.GetHourTime(item, nowDate);

                                        break;
                                    case "W": //按周
                                        nextDate = emailRepository.GetWeekDay(item, nowDate);

                                        break;
                                    case "M": //按月
                                        nextDate = emailRepository.GetMonthDay(item, nowDate);
                                        break;
                                }
                            }
                            //更新下次执行日期Next_Execution_Date
                            var strUpdate = @"UPDATE dbo.System_Schedule SET Last_Execution_Date = GETDATE(),
                                            Next_Execution_Date = '{1}',
                                            Modified_Date = GETDATE(),
                                            Modified_UID = 99999
                                            WHERE System_Schedule_UID = {0}; ";
                            strUpdate = string.Format(strUpdate, item.System_Schedule_UID, nextDate);
                            sb.AppendLine(strUpdate);
                        }
                    }
                    if (sb.Length > 0)
                    {
                        context.Database.ExecuteSqlCommand(sb.ToString());
                    }

                    foreach (var item in matchList)
                    {
                        //执行完毕后写入日志
                        Batch_Log newLog = new Batch_Log
                        {
                            System_Schedule_UID = item.System_Schedule_UID,
                            Batch_Name = item.Function_Name + "执行成功",
                            Batch_Status = true,
                            Batch_Desc = item.Function_Name + "执行成功",
                            Batch_Date = DateTime.Now
                        };
                        emailRepository.InsertBatchLog(newLog);

                    }
                }
                catch (Exception ex)
                {
                    emailRepository.InsertExceptionBatchLog(Excetion_Email_UID, ex.Message);
                }

                return matchList;
            }
        }



    }
}
