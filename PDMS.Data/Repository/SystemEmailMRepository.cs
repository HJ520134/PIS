using PDMS.Common.Common;
using PDMS.Common.Constants;
using PDMS.Data.Infrastructure;
using PDMS.Model.ViewModels.Batch;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface ISystemEmailMRepository : IRepository<System_Email_M>
    {
        //查询邮件发送列表准备发送邮件
        void ExecSendEmail();

        void InsertBatchLog(Batch_Log newLog);

        void InsertExceptionBatchLog(int Excetion_Email_UID, string errorInfo);
    }

    public class SystemEmailMRepository : RepositoryBase<System_Email_M>, ISystemEmailMRepository
    {
        public SystemEmailMRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        //查询邮件发送列表准备发送邮件
        public void ExecSendEmail()
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
                                WHERE A.Is_Enable = 1 AND B.Is_Enable = 1 AND C.Function_Name = '{0}'";

                    strSql = string.Format(strSql, functionName);
                    var list = context.Database.SqlQuery<BatchExecVM>(strSql).ToList();

                    

                    #region 第一步：查询System_Schedule将时间相匹配的数据进行更新
                    StringBuilder sb = new StringBuilder();
                    var nowDate = DateTime.Now;
                    DateTime? nextDate = null;
                    foreach (var item in list)
                    {
                        Excetion_Email_UID = item.System_Schedule_UID;

                        //Next_Execution_Date若为当前时间，则执行
                        if (
                            item.Next_Execution_Date.Year == nowDate.Year
                            && item.Next_Execution_Date.Month == nowDate.Month
                            && item.Next_Execution_Date.Day == nowDate.Day
                            && item.Next_Execution_Date.Hour == nowDate.Hour
                            && item.Next_Execution_Date.Minute == nowDate.Minute
                            )
                        {
                            //var strTimeList = item.Exec_Moment.Split(',').ToList();
                            //将list<string>转换为list<int>
                            //var intIdList = strTimeList.Select<string, int>(x => Convert.ToInt32(x)).ToList();

                            matchList.Add(item);

                            switch (item.Cycle_Unit)
                            {
                                case "H": //按小时
                                    nextDate = GetHourTime(item, nowDate);

                                    break;
                                case "W": //按周
                                    nextDate = GetWeekDay(item, nowDate);

                                    break;
                                case "M": //按月
                                    nextDate = GetMonthDay(item, nowDate);
                                    break;

                            }
                            //更新下次执行时间Next_Execution_Date
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

                    #endregion

                    #region 第二步：发送邮件通知

                    //发送三天之内的邮件
                    //ISNULL(Reservation_Date,Modified_Date),GETDATE()) <= 3 防止预订的时间到了程序还没跑到
                    var strExecSql = @"SELECT * FROM dbo.System_Email_M WHERE (Is_Send = 0 OR Is_Send IS NULL) 
                                        AND (DATEDIFF(DAY,ISNULL(Reservation_Date,Modified_Date),GETDATE()) <= 3) 
                                        AND GETDATE() >= ISNULL(Reservation_Date,Modified_Date) ";

                    //上面的查询语句条件不能加System_Schedule_UID，因为邮件执行的排程外键不是发邮件的外键
                    var vmList = context.Database.SqlQuery<BatchMailVM>(strExecSql).ToList();
                    if (vmList.Count() > 0)
                    {
                        foreach (var item in vmList)
                        {
                            var IsSuccess = SendMail(item);
                            var entity = context.System_Email_M.Find(item.System_Email_M_UID);
                            if (IsSuccess)
                            {
                                entity.Is_Send = true;
                                entity.Send_Time = nowDate;

                                //执行完毕后写入日志
                                Batch_Log newLog = new Batch_Log
                                {
                                    System_Schedule_UID = item.System_Schedule_UID,
                                    Batch_Name = StructConstants.BatchLog.Email_Module_Success,
                                    Batch_Status = true,
                                    Batch_Desc = StructConstants.BatchLog.Email_Module_Success,
                                    Batch_Date = DateTime.Now
                                };
                                InsertBatchLog(newLog);
                            }
                        }
                        context.SaveChanges();
                    }
                    #endregion


                }
                catch (Exception ex)
                {
                    InsertExceptionBatchLog(Excetion_Email_UID, ex.Message);
                }
            }

        }

        /// <summary>
        /// 寄發郵件
        /// </summary>
        /// <param name="vm">郵件資料集合</param>
        /// <returns></returns>
        private bool SendMail(BatchMailVM vm)
        {
            try
            {
                var IsDevelop = System.Configuration.ConfigurationManager.AppSettings["IsDevelop"] == null ? false : bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IsDevelop"].ToString());
                SmtpClient smtpClient = new SmtpClient("CORIMC04.corp.jabil.org");
                MailMessage mm = new MailMessage();

                var _Email_To = vm.Email_To.Trim().Split(',');
                foreach (var item in _Email_To)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        mm.To.Add(item);
                    }
                }
                if (!string.IsNullOrEmpty(vm.Email_CC))
                {
                    var _Email_CC = vm.Email_CC.Trim().Split(',');
                    foreach (var item in _Email_CC)
                    {
                        mm.CC.Add(item);
                    }
                }
                if (!string.IsNullOrEmpty(vm.Email_BCC))
                {
                    var _Email_BCC = vm.Email_BCC.Trim().Split(',');
                    foreach (var item in _Email_BCC)
                    {
                        mm.Bcc.Add(item);
                    }
                }

                string mFromTitle = vm.Email_From.Split(',')[0];
                string mFromEmail = vm.Email_From.Split(',')[1];
                mm.From = new MailAddress(mFromEmail, mFromTitle);
                mm.Subject = IsDevelop ? "[系统测试]" + vm.Subject : vm.Subject;
                mm.SubjectEncoding = System.Text.Encoding.UTF8;
                mm.Body = vm.Body;
                mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.IsBodyHtml = true;

                List<string> layoutPathList = new List<string>();
                layoutPathList = new EmailFormat().Email_Type(vm.Email_Type);


                AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(vm.Body, null, "text/html");
                if (layoutPathList.Count > 0)
                {
                    var i = 0;
                    foreach (var file in layoutPathList)
                    {
                        LinkedResource lrImage = new LinkedResource(new ServerInfoUtility().MapPath(file), "image/png");
                        lrImage.ContentId = "LayoutImg" + i;
                        htmlBody.LinkedResources.Add(lrImage);
                        i++;
                    }
                }

                mm.AlternateViews.Add(htmlBody);

                smtpClient.Send(mm);
                EventLog.FilePath = new ServerInfoUtility().MapPath(StructConstants.Log_Path.BatchEmalLog);
                EventLog.Write("Send Success ! (System_Email_M_UID： " + vm.System_Email_M_UID + " , Email_To： " + vm.Email_To + ")");
                return true;
            }
            catch (Exception ex)
            {
                InsertExceptionBatchLog(1, ex.Message);
                //var message = ex.ToString();
                //EventLog.FilePath = new ServerInfoUtility().MapPath(StructConstants.Log_Path.BatchEmalLog);
                //EventLog.Write(message);
                return false;
            }
        }

        public void InsertBatchLog(Batch_Log newLog)
        {
            using (var context = new SPPContext())
            {
                context.Batch_Log.Add(newLog);
                context.SaveChanges();
            }
        }

        public void InsertExceptionBatchLog(int Excetion_Email_UID, string errorInfo)
        {
            using (var context = new SPPContext())
            {
                //发生异常后，需要发送邮件通知对应的系统人员或角色人员
                var excetionSql = @"SELECT A.System_PIC_UIDs,B.Users_PIC_UIDs,B.Role_UIDs,C.Function_Name FROM dbo.System_Module A
                                    JOIN dbo.System_Schedule B ON B.System_Module_UID = A.System_Module_UID
                                    JOIN dbo.System_Function C ON B.Function_UID = C.Function_UID
                                    WHERE B.System_Schedule_UID = {0}";

                excetionSql = string.Format(excetionSql, Excetion_Email_UID);
                var item = context.Database.SqlQuery<BatchExecVM>(excetionSql).First();
                List<int> AccountUIDList = new List<int>();

                var sysUIDList = item.System_PIC_UIDs.Split(',').ToList();
                var intSysUIdList = sysUIDList.Select<string, int>(x => Convert.ToInt32(x)).ToList();
                AccountUIDList.AddRange(intSysUIdList);

                if (!string.IsNullOrEmpty(item.Users_PIC_UIDs))
                {
                    var userUIDList = item.Users_PIC_UIDs.Split(',').ToList();
                    var intUserUIdList = userUIDList.Select<string, int>(x => Convert.ToInt32(x)).ToList();
                    AccountUIDList.AddRange(intUserUIdList);
                }

                if (!string.IsNullOrEmpty(item.Role_UIDs))
                {
                    var roleUIDList = item.Role_UIDs.Split(',').ToList();
                    var intRoleUIDList = roleUIDList.Select<string, int>(x => Convert.ToInt32(x)).ToList();
                    var roleUserUIDList = context.System_User_Role.Where(m => intRoleUIDList.Contains(m.Role_UID)).Select(m => m.Account_UID).ToList();
                    AccountUIDList.AddRange(roleUserUIDList);
                }

                AccountUIDList = AccountUIDList.Distinct().ToList();

                //var emailList = context.System_Users.Where(m => !string.IsNullOrEmpty(m.Email) && AccountUIDList.Contains(m.Account_UID)).Select(m => m.Email).ToList();
                //System_Email_M emailItem = new System_Email_M();
                //emailItem.System_Schedule_UID = Excetion_Email_UID;
                //emailItem.Subject = StructConstants.BatchLog.Email_Module_Failed;
                //emailItem.Body = item.Function_Name + "出现错误，请联系系统管理员";
                //emailItem.Email_From = StructConstants.Email_From.PIS_Email_From;
                //emailItem.Email_To = string.Join(",", emailList);
                //emailItem.Email_To_UIDs = string.Join(",", AccountUIDList);
                //emailItem.Is_Send = false;
                //emailItem.Email_Type = 1;
                //emailItem.Modified_UID = ConstConstants.AdminUID;
                //emailItem.Modified_Date = DateTime.Now;

                //context.System_Email_M.Add(emailItem);
                //context.SaveChanges();

                Batch_Log newLog = new Batch_Log
                {
                    System_Schedule_UID = Excetion_Email_UID,
                    Batch_Name = item.Function_Name + "执行失败",
                    Batch_Status = false,
                    Batch_Desc = errorInfo,
                    Batch_Date = DateTime.Now
                };
                InsertBatchLog(newLog);
            }

        }


        //获取按小时执行的时间
        public DateTime GetHourTime(BatchExecVM item, DateTime nowDate)
        {
            DateTime? nextDate = null;
            var hourList = item.Exec_Moment.Split(',').ToList();
            var minHour = Convert.ToInt32(hourList.First());
            var maxHour = Convert.ToInt32(hourList.Last());
            if (hourList.Count() == 1)
            {
                //var intervalHour = Convert.ToInt32(item.Exec_Moment);
                nextDate = item.Next_Execution_Date.AddHours(1);
            }
            else //分割的值有多个
            {
                if (nowDate.Minute == maxHour) //如果值等于最后的一个时间段
                {
                    nextDate = item.Next_Execution_Date.AddMinutes(60 - maxHour + minHour);
                }
                else
                {
                    //找到列表的索引序号
                    int currentSeq = 0;
                    for (int i = 0; i < hourList.Count(); i++)
                    {
                        if (Convert.ToInt16(hourList[i]) == nowDate.Minute)
                        {
                            currentSeq = i;
                            break;
                        }
                    }
                    nextDate = item.Next_Execution_Date.AddMinutes(Convert.ToInt16(hourList[currentSeq + 1]) - Convert.ToInt32(hourList[currentSeq]));
                }
            }
            return nextDate.Value;
        }

        //获取按周执行的时间
        public DateTime GetWeekDay(BatchExecVM item, DateTime nowDate)
        {
            DateTime? nextDate = null;
            var dayList = item.Exec_Moment.Split(',').ToList();
            var execTimeList = item.Exec_Time.Split(',').ToList();
            //如果只有一个执行时间点
            if (execTimeList.Count() == 1)
            {
                //如果分割的列表是7个则说明每天执行
                if (dayList.Count() == 7)
                {
                    nextDate = item.Next_Execution_Date.AddDays(1);
                }
                else if (dayList.Count() == 1) //如果分割的列表是1个则说明每周执行一次
                {
                    nextDate = item.Next_Execution_Date.AddDays(7);
                }
                else //不是每天执行则根据数字判断
                {
                    //系统里面把星期天当成0
                    var dayOfInt = (int)nowDate.DayOfWeek;
                    if (dayOfInt == 0)
                    {
                        dayOfInt = 7;
                    }

                    //找到列表的索引序号
                    for (int i = 0; i < dayList.Count(); i++)
                    {
                        //是最后一个元素
                        if (i == dayList.Count() - 1)
                        {
                            var interValDay = 7 - Convert.ToInt32(dayList[i]) + Convert.ToInt32(dayList[0]);
                            nextDate = item.Next_Execution_Date.AddDays(interValDay);
                            break;
                        }
                        if (Convert.ToInt16(dayList[i]) == dayOfInt)
                        {
                            nextDate = item.Next_Execution_Date.AddDays(Convert.ToInt16(dayList[i + 1]) - Convert.ToInt32(dayList[i]));
                            break;
                        }
                    }
                }
            }
            else //如果有多个执行时间点
            {
                var maxTime = execTimeList.Last();
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

                        //判断Exec_Moment执行的日期是不是最后一天
                        if (dayList.Count() == 1) //如果设置的是每周几执行
                        {
                            nextDate = item.Next_Execution_Date.Date.AddDays(7).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                        }
                        else if (dayList.Count() == 7)//每天执行1，2，3，4，5，6，7
                        {
                            nextDate = item.Next_Execution_Date.Date.AddDays(1).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                        }
                        else //间隔执行1，3，5
                        {
                            //系统里面把星期天当成0
                            var dayOfInt = (int)nowDate.DayOfWeek;
                            if (dayOfInt == 0)
                            {
                                dayOfInt = 7;
                            }

                            //找到列表的索引序号
                            for (int j = 0; j < dayList.Count(); j++)
                            {
                                //是最后一个元素
                                if (j == dayList.Count() - 1)
                                {
                                    var interValBBDay = 7 - Convert.ToInt32(dayList[j]) + Convert.ToInt32(dayList[0]);
                                    nextDate = item.Next_Execution_Date.Date.AddDays(interValBBDay).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                                    break;
                                }
                                if (Convert.ToInt16(dayList[j]) == dayOfInt)
                                {
                                    nextDate = item.Next_Execution_Date.Date.AddDays(Convert.ToInt16(dayList[j + 1]) - Convert.ToInt32(dayList[j])).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                                    break;
                                }
                            }
                            

                        }
                    }
                }
            }


            return nextDate.Value;
        }

        //获取按月执行的时间
        public DateTime GetMonthDay(BatchExecVM item, DateTime nowDate)
        {
            DateTime? nextDate = null;
            var dayList = item.Exec_Moment.Split(',').ToList();
            var minDay = Convert.ToInt32(dayList.First());
            var maxDay = Convert.ToInt32(dayList.Last());
            var execTimeList = item.Exec_Time.Split(',').ToList();
            //如果只有一个执行时间点
            if (execTimeList.Count() == 1)
            {
                if (dayList.Count() == 1)
                {
                    nextDate = item.Next_Execution_Date.AddMonths(1);
                }
                else
                {
                    //找到列表的索引序号
                    for (int i = 0; i < dayList.Count(); i++)
                    {
                        if (Convert.ToInt16(dayList[i]) == nowDate.Day)
                        {
                            //获取这个月有多少天
                            var days = DateTime.DaysInMonth(item.Next_Execution_Date.Year, item.Next_Execution_Date.Month);

                            if (i != dayList.Count() - 1) //不是最后一天即最后一个元素
                            {
                                var intervalDay = Convert.ToInt32(dayList[i + 1]) - Convert.ToInt32(dayList[i]);
                                nextDate = item.Next_Execution_Date.AddDays(intervalDay);
                                break;
                            }
                            else //是最后一天即最后一个元素
                            {
                                var intervalDay = days - Convert.ToInt32(dayList[i]) + Convert.ToInt32(dayList[0]);
                                nextDate = item.Next_Execution_Date.AddDays(intervalDay);
                                break;
                            }
                        }
                    }
                }
            }
            else //如果有多个执行时间点
            {
                var maxTime = execTimeList.Last();
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

                        if (dayList.Count() == 1)
                        {
                            nextDate = item.Next_Execution_Date.Date.AddMonths(1).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                        }
                        else
                        {
                            //找到列表的索引序号
                            for (int j = 0; j < dayList.Count(); j++)
                            {
                                if (Convert.ToInt16(dayList[j]) == nowDate.Day)
                                {
                                    //获取这个月有多少天
                                    var days = DateTime.DaysInMonth(item.Next_Execution_Date.Year, item.Next_Execution_Date.Month);

                                    if (j != dayList.Count() - 1) //不是最后一天即不是最后一个元素
                                    {
                                        var intervalDay = Convert.ToInt32(dayList[j + 1]) - Convert.ToInt32(dayList[j]);
                                        nextDate = item.Next_Execution_Date.Date.AddDays(intervalDay).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                                        break;
                                    }
                                    else //是最后一天即最后一个元素
                                    {
                                        var intervalDay = days - Convert.ToInt32(dayList[j]) + Convert.ToInt32(dayList[0]);
                                        nextDate = item.Next_Execution_Date.Date.AddDays(intervalDay).AddHours(firstExecTimeHour).AddMinutes(firstExecTimeMinute);
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }

            }

            return nextDate.Value;
        }


    }
}
