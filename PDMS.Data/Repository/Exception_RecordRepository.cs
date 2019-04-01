using PDMS.Data;
using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;
using AutoMapper;
using System.Linq.Dynamic;
using PDMS.Model;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Net;

namespace PDMS.Data.Repository
{
    public interface IException_RecordRepository : IRepository<Exception_Record>
    {
        int ExceptionRecordAdd(ExceptionAddDTO dto);
        List<Stations> FetchStations(int lineID);
        ShiftTime FetchShifTime(int shiftTimeID);
        IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, Page page, out int totalcount);
        List<Line> FetchGL_Line(int plantuid, int bguid, int funuid);
        List<Line> FetchGL_LineWithGroup(ExceptionDTO dto);
        
        List<Stations> FetchStationsBasedLine(int uid);
        List<ShiftTime> FetchAllShifTime();
        int CloseExceptionOrder(ExceptionDTO dto);
        int DeleteExceptionOrder(int uid);
        int ExceptionReply(ReplyRecordDTO dto);
        List<ExceptionDTO> ExportFixtrueReturn2Excel(ExceptionDTO dto);
        void ExceptionShedule();
        List<Line> FetchLineBasedPlantBGCustomer(ExceptionDTO dto);
        List<ReplyRecordDTO> ViewRecordReplyAPI(int uid);
        List<SystemUserDTO> FethAllEmail();
        int SendEmailException(EmailSendDTO dto);
        List<ExceptionDTO> ExportSomeRecord2Excel(string uids);
        ShiftTime FetchShiftTimeDetail(int uid);
        List<ShiftTime> FetchShiftTimeBasedBG(int plantuid, int bguid);
        ChartsModel QueryExceptionRecordDashboard(DashboardSearchDTO dto);
        List<ExceptionDTO> QueryDowntimeRecords(DashboardSearchDTO dto);
        ExceptionDTO FetchExceptionDetail(int uid);
        ReplyRecordDTO FetchLatestReply(int uid);
        // string GetStartDate(DashboardSearchDTO search);
    }

    public class Exception_RecordRepository : RepositoryBase<Exception_Record>, IException_RecordRepository
    {
        public Exception_RecordRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public int ExceptionRecordAdd(ExceptionAddDTO dto)
        {
            int[] exceIDs = dto.Exception_Code_UIDs.Split(',').Select(s => int.Parse(s)).ToArray();
            List<int> exceRecordID = new List<int>();
            int ret = 0;
            foreach (int id in exceIDs)
            {
                Exception_Record entity = new Exception_Record();
                entity.WorkDate = dto.WorkDate.Date;
                entity.ShiftTimeID = dto.ShiftTimeID;
                entity.Origin_UID = dto.LineID;
                entity.SubOrigin_UID = dto.StationID;
                entity.Exception_Code_UID = id;
                entity.Note = dto.Note;
                entity.Contact_Person = dto.Contact_Person;
                entity.Contact_Phone = dto.Contact_Phone;
                entity.Created_UID = dto.Created_UID;
                entity.Created_Date = dto.Created_Date;
                entity.Modified_UID = dto.Modified_UID;
                entity.Modified_Date = dto.Modified_Date;
                entity.Note = dto.Note;
                entity.Project_UID = dto.Project_UID;
                DataContext.Exception_Record.Add(entity);
                ret += DataContext.SaveChanges();
                if (ret > 0)
                {
                    //执行一个存储过程
                    exceRecordID.Add(entity.Exception_Record_UID);
                    var exceNo = new SqlParameter("@exce_no", entity.Exception_Record_UID);
                    DataContext.Database.ExecuteSqlCommand("usp_ExceptionSheduleCreate @exce_no", exceNo);
                }
            }
            //接下来是发送邮件
            if (ret == exceRecordID.Count)
            {
                //var type = new SqlParameter("@type", 3);
                //var uid = new SqlParameter("@uid", dto.Exception_Dept_UID);
                DataSet ds = new DataSet();
                SqlConnection sqlConn = new SqlConnection();
                sqlConn = DataContext.Database.Connection as SqlConnection;
                SqlDataAdapter sqlDa = new SqlDataAdapter("usp_ExceptionSheduleQuery", sqlConn);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@type", SqlDbType.BigInt));
                sqlDa.SelectCommand.Parameters[0].Value = 3;
                sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@uid", SqlDbType.BigInt));
                sqlDa.SelectCommand.Parameters[1].Value = dto.Project_UID;
                sqlDa.Fill(ds);
                StringBuilder MailBody = new StringBuilder();
                MailBody.Append("</br><p> Issue details：");
                MailBody.Append("<table>");
                MailBody.Append("<tr>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Project" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Line" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Station" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Start Time" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Output Date" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Shift" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Department" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Issue Code" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Issue Reason" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Owner" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Contact" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + "Note" + "</td>");
                MailBody.Append("</tr>");
                foreach (int id in exceRecordID)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append($@"select 
	                            A.*,
	                            B.Exception_Name,
	                            B.Exception_Nub,
	                            C.Exception_Dept_Name,
	                            D.LineName,
	                            F.StationName,
	                            E.Shift,
	                            U.User_Name,
	                            V.User_Name as Modified_UserName,
                                P.Project_Name
                            from 
	                            Exception_Record A
	                            left join Exception_Code B on A.Exception_Code_UID=B.Exception_Code_UID
	                            left join Exception_Dept C on C.Exception_Dept_UID=B.Exception_Dept_UID
	                            left join GL_Line D on D.LineID=A.Origin_UID
	                            left join GL_Station F on F.StationID=A.SubOrigin_UID
	                            left join GL_ShiftTime E on E.ShiftTimeID=A.ShiftTimeID
	                            left join System_Users U on U.Account_UID=A.Created_UID
	                            left join System_Users V on V.Account_UID=A.Modified_UID
								left join System_Project P on P.Project_UID=A.Project_UID
                            where A.Exception_Record_UID={id}");
                    var entity = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).FirstOrDefault();
                    if (entity == null)
                        continue;
                    MailBody.Append("<tr>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Project_Name ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.LineName ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.StationName ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Created_Date.ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + Convert.ToDateTime(entity.WorkDate).ToString("yyyy-MM-dd") + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Shift ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Exception_Dept_Name ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Exception_Nub ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Exception_Name ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Contact_Person ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Contact_Phone ?? "" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + entity.Note ?? "" + "</td>");
                    MailBody.Append("</tr>");
                }
                MailBody.Append("</table>");
                MailBody.Append("<p>&nbsp&nbsp Please login PIS system to view detail information :<a href=");
                MailBody.Append("http://cnctug0pdmsap01.corp.jabil.org/PIS_M/");
                MailBody.Append(">Login</a></p>");
                MailBody.Append("</body>");
                MailBody.Append("</html>");

                //收件人
                int[] receiveIDs = Array.ConvertAll(dto.SendIDs.Split(','), s => Int32.Parse(s));
                var receive = DataContext.System_Users.Where(x => receiveIDs.Contains(x.Account_UID)).Select(x => x.Email).ToArray();
                //CC人员
                string[] cc = null;
                if (dto.CCIDs.Length > 0)
                {
                    int[] ccIDs = Array.ConvertAll(dto.CCIDs.Split(','), s => Int32.Parse(s));

                    cc = DataContext.System_Users.Where(x => ccIDs.Contains(x.Account_UID)).Select(x => x.Email).ToArray();
                }

                //List<string> MailTo = receive;
                ////-------------------send users---------------------
                //foreach (DataRow item in ds.Tables[0].Rows)
                //{
                //    //if (i == tbUsers.Rows.Count)
                //    //    flag = "";
                //    MailTo.Add(Convert.ToString(item["User_Email"]));
                //}
                //List<string> MailCC = new List<string>();

                //foreach (DataRow item in ds.Tables[1].Rows)
                //{
                //    //if (i == tbUsers.Rows.Count)
                //    //    flag = "";
                //    MailCC.Add(Convert.ToString(item["User_Email"]));
                //}
                StringBuilder sbThreme = new StringBuilder();
                sbThreme.Append("New issue need to be handled");

                SmtpClient client = new SmtpClient();
                client.Host = "corimc04.corp.jabil.org";
                client.Port = 25;
                MailMessage message = new MailMessage();
                try
                {
                    if (receive != null)
                        foreach (var item in receive)
                            message.To.Add(item);


                    if (cc != null)
                        foreach (var item in cc)
                            message.CC.Add(item);



                    message.From = new MailAddress("PIS_System@jabil.com"); //使用指定的邮件地址初始化MailAddress实例
                    //电子邮件的主题内容使用的编码
                    message.SubjectEncoding = Encoding.UTF8;
                    //电子邮件正文
                    message.Body = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(MailBody.ToString()));

                    //电子邮件正文的编码
                    //message.BodyEncoding = Encoding.Default;
                    //message.Priority = MailPriority.High;
                    message.BodyEncoding = Encoding.UTF8;
                    message.Priority = MailPriority.High;
                    message.IsBodyHtml = true;
                    message.Subject = sbThreme.ToString();
                    message.IsBodyHtml = true;
                    client.Send(message);
                }
                catch
                {

                }

            }
            return ret;
        }
        public List<Stations> FetchStations(int lineID)
        {
            var entities = DataContext.GL_Station.Where(x => x.LineID == lineID).Select(x => new Stations()
            {
                StationID = x.StationID,
                StationName = x.StationName
            }).ToList();
            return entities;
        }
        public ShiftTime FetchShifTime(int shiftTimeID)
        {
            var entity = DataContext.GL_ShiftTime.Where(x => x.ShiftTimeID == shiftTimeID).Select(x => new ShiftTime()
            {
                StartTime = x.StartTime,
                End_Time = x.End_Time,
                Shift = x.Shift
            }).FirstOrDefault();

            if ((entity.Shift == "晚班" || entity.Shift == "Nightshift") && DateTime.Now.Hour <= 23)//上夜班
            {
                entity.StartTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " " + entity.StartTime + ":00";
                entity.End_Time = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00";
            }
            else if ((entity.Shift == "晚班" || entity.Shift == "Nightshift"))
            {
                entity.StartTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00";
                entity.End_Time = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " " + entity.End_Time + " 00";
            }
            else
            {
                entity.StartTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + entity.StartTime + " 00";
                entity.End_Time = DateTime.Now.ToString("yyyy-MM-dd") + " " + entity.End_Time + " 00";
            }

            return entity;
        }

        public IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, Page page, out int totalcount)
        {
            #region 分页查询
            StringBuilder sql = new StringBuilder();
            sql.Append(@";With BG_Info AS (
                   select 
                       Organization_UID as BG_Organization_UID,
                       Organization_Name AS OP_TYPES 
                    from 
                       System_Organization 
                    where Organization_ID LIKE'20%'), 
                FunPlant AS( 
                    SELECT DISTINCT 
                        t2.Organization_Name as FunPlantName,
                        one.ParentOrg_UID as BG_Organization_UID,
                        t2.Organization_UID as FunPlant_Organization_UID 
                    FROM (
						    SELECT * FROM dbo.System_OrganizationBOM) one 
                            INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                            INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                            INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                        WHERE 
                           t3.Organization_Name='OP')
						   select 
	                            A.*,
	                            B.Exception_Name,
	                            B.Exception_Nub,
	                            C.Exception_Dept_Name,
	                            D.LineName,
                                F.Seq,
	                            F.StationName,
	                            E.Shift,
	                            U.User_Name,
	                            V.User_Name as Modified_UserName,
								isnull(H.Organization_Name,'Gold Line') as Plant,
								isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
								ISNULL(FunPlant.FunPlantName,'') as FunPlant,
                                P.Project_Name
                            from 
	                            Exception_Record A
	                            left join Exception_Code B on A.Exception_Code_UID=B.Exception_Code_UID
	                            left join Exception_Dept C on C.Exception_Dept_UID=B.Exception_Dept_UID
	                            left join GL_Line D on D.LineID=A.Origin_UID
	                            left join GL_Station F on F.StationID=A.SubOrigin_UID
	                            left join GL_ShiftTime E on E.ShiftTimeID=A.ShiftTimeID
	                            left join System_Users U on U.Account_UID=A.Created_UID
	                            left join System_Users V on V.Account_UID=A.Modified_UID
			                    left join System_Organization H on H.Organization_UID=C.Plant_Organization_UID
			                    left join BG_Info on BG_Info.BG_Organization_UID=C.BG_Organization_UID
			                    left join FunPlant on FunPlant.BG_Organization_UID=C.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=C.FunPlant_Organization_UID
								left join System_Project P on P.Project_UID=A.Project_UID
                            where 1=1 ");
            StringBuilder sbWhere = new StringBuilder();
            if (searchModel.LineID != 0)
            {
                sbWhere.Append($@" and  A.Origin_UID={searchModel.LineID}");
            }

            if (searchModel.StationID != 0)
            {
                sbWhere.Append($@" and  A.SubOrigin_UID={searchModel.StationID}");
            }
            if (searchModel.ShiftTimeID != 0)
            {
                sbWhere.Append($@" and  A.ShiftTimeID={searchModel.ShiftTimeID}");
            }
            if (searchModel.WorkDate != DateTime.MinValue)
            {
                sbWhere.Append($@" and  A.WorkDate='{searchModel.WorkDate}'");
            }
            if (searchModel.Modified_Date_from != DateTime.MinValue)
            {
                sbWhere.Append($@" and  A.Created_Date>='{searchModel.Modified_Date_from.ToString("yyyy-MM-dd") + " 00:00:00"}'");
            }

            if (searchModel.Exception_Code_UID != 0 && searchModel.Exception_Code_UID != null)
            {
                sbWhere.Append($@" and  A.Exception_Code_UID={searchModel.Exception_Code_UID}");
            }

            if (searchModel.DelayDayNub > 0)
            {
                //判断他最后日期有没有选择
                if (searchModel.Modified_Date_to != DateTime.MinValue)
                {
                    sbWhere.Append($@" and  A.Created_Date<='{searchModel.Modified_Date_to.AddDays(0 - searchModel.DelayDayNub).ToString("yyyy-MM-dd") + " 23:59:59"}'");
                }
                else
                {
                    sbWhere.Append($@" and  A.Created_Date<='{DateTime.Now.AddDays(0 - searchModel.DelayDayNub).ToString("yyyy-MM-dd") + " 23:59:59"}'");
                }
            }
            else
            {
                if (searchModel.Modified_Date_to != DateTime.MinValue)
                {
                    sbWhere.Append($@" and  A.Created_Date<='{searchModel.Modified_Date_to.ToString("yyyy-MM-dd") + " 23:59:59"}'");
                }
            }

            if (searchModel.Status >= 0)
            {
                sbWhere.Append($@" and A.Status ='{searchModel.Status}'");
            }
            if (searchModel.Exception_Dept_UID > 0)
            {
                sbWhere.Append($@" and B.Exception_Dept_UID ='{searchModel.Exception_Dept_UID}'");
            }

            if (searchModel.Plant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.Plant_Organization_UID={searchModel.Plant_Organization_UID}");
            }
            if (searchModel.BG_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.BG_Organization_UID={searchModel.BG_Organization_UID}");
            }
            if (searchModel.Funplant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.Funplant_Organization_UID={searchModel.Funplant_Organization_UID}");
            }
            if (searchModel.CustomerID != 0)
            {
                sbWhere.Append($@" and  A.Project_UID={searchModel.CustomerID}");
            }
            sql.Append(sbWhere);
            sql.Append($@" order by 
                        A.Status ASC,P.Project_Name,D.LineName,F.Seq 
                        OFFSET {page.PageNumber * page.PageSize} ROWS
                        FETCH NEXT {page.PageSize} ROWS ONLY");
            #endregion
            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList().AsQueryable();

            #region 计算总数
            StringBuilder sqlCount = new StringBuilder();
            sqlCount.Append(@"  select 
                                    count(1) 
	                            from 
	                            Exception_Record A
	                            left join Exception_Code B on A.Exception_Code_UID=B.Exception_Code_UID
	                            left join Exception_Dept C on C.Exception_Dept_UID=B.Exception_Dept_UID
	                            left join GL_Line D on D.LineID=A.Origin_UID
	                            left join GL_Station F on F.StationID=A.SubOrigin_UID
	                            left join GL_ShiftTime E on E.ShiftTimeID=A.ShiftTimeID
	                            left join System_Users U on U.Account_UID=A.Created_UID
	                            left join System_Users V on V.Account_UID=A.Modified_UID
                                left join System_Project P on P.Project_UID=A.Project_UID
                                where 1=1 ").Append(sbWhere);
            #endregion
            var count = DataContext.Database.SqlQuery<int>(sqlCount.ToString()).ToArray();
            totalcount = count[0];
            return dbList;
        }

        public List<Line> FetchGL_Line(int plantuid, int bguid, int funuid)
        {

            StringBuilder sql = new StringBuilder();
            sql.Append(@"select distinct 
                          B.LineID,
                          B.LineName 
                        from 
                            GL_Station A
	                    inner join GL_Line B on B.LineID=A.LineID
                        where 
		                A.IsGoldenLine=1 ");
            StringBuilder sbWhere = new StringBuilder();
            if (plantuid != 0)
            {
                sbWhere.Append($@" and  A.Plant_Organization_UID={plantuid}");
            }
            if (bguid != 0)
            {
                sbWhere.Append($@" and  A.BG_Organization_UID={bguid}");
            }
            if (funuid != 0)
            {
                sbWhere.Append($@" and  A.FunPlant_Organization_UID={funuid}");
            }
            sql.Append(sbWhere);
            sql.Append(" order by LineName");
            var entities = DataContext.Database.SqlQuery<Line>(sql.ToString()).ToList();
            return entities;
        }
        public List<Line> FetchGL_LineWithGroup(ExceptionDTO dto)
        {

            StringBuilder sql = new StringBuilder();
            sql.Append(@"select distinct 
                          B.LineID,
                          B.LineName 
                        from 
                            GL_Station A
	                    inner join GL_Line B on B.LineID=A.LineID
                        where 
		                A.IsGoldenLine=1 ");
            StringBuilder sbWhere = new StringBuilder();
            if (dto.Plant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  A.Plant_Organization_UID={dto.Plant_Organization_UID}");
            }
            if (dto.BG_Organization_UID != 0)
            {
                sbWhere.Append($@" and  A.BG_Organization_UID={dto.BG_Organization_UID}");
            }
            if (dto.CustomerID != 0)
            {
                sbWhere.Append($@" and  B.CustomerID={dto.CustomerID}");
            }
            sql.Append(sbWhere);
            var entities = DataContext.Database.SqlQuery<Line>(sql.ToString()).ToList();
            var entitiesGroup = new List<string>();
            foreach(var item in entities)
            {
                if (!item.LineName.Contains('-'))
                    continue;
                var name= item.LineName.Split('-')[0];
                if (!entitiesGroup.Contains(name.ToLower()))
                    entitiesGroup.Add(name.ToLower());
            }
            foreach(var item in entitiesGroup)
            {
                entities.Add(new Line() { LineID = -1, LineName = item });
            }
            entities = entities.OrderBy(x => x.LineName).ToList();

            return entities;

        }
        public List<Stations> FetchStationsBasedLine(int uid)
        {
            var entities = DataContext.GL_Station.Where(x => x.LineID == uid).Select(x => new Stations()
            {
                StationID = x.StationID,
                StationName = x.StationName
            }).ToList();
            return entities;
        }

        public List<ShiftTime> FetchAllShifTime()
        {
            var entities = DataContext.GL_ShiftTime.Select(x => new ShiftTime()
            {
                ShiftTimeID = x.ShiftTimeID,
                Shift = x.Shift
            }).OrderBy(x => x.Shift).ToList();
            return entities;
        }

        public int CloseExceptionOrder(ExceptionDTO dto)
        {
            var entity = DataContext.Exception_Record.Find(dto.Exception_Record_UID);
            entity.Status = 1;
            entity.End_Date = DateTime.Now;
            entity.Modified_UID = dto.Modified_UID;
            entity.Modified_Date = dto.Modified_Date;

            //todo：shedule表也要删除

            //执行一个存储过程
            var exceNo = new SqlParameter("@exce_no", entity.Exception_Record_UID);
            DataContext.Database.ExecuteSqlCommand("usp_ExceptionSheduleClose @exce_no", exceNo);

            return DataContext.SaveChanges();
        }

        public int DeleteExceptionOrder(int uid)
        {
            var entity = DataContext.Exception_Record.Find(uid);
            DataContext.Exception_Record.Remove(entity);
            return DataContext.SaveChanges();
        }

        public int ExceptionReply(ReplyRecordDTO dto)
        {
            DataContext.Exception_Reply.Add(new Exception_Reply()
            {
                Exception_Record_UID = dto.Exception_Record_UID,
                Exception_Content = dto.Exception_Content,
                Reply_UID = dto.Reply_UID,
                Reply_Date = dto.Reply_Date??default(DateTime)
            });
            //插入一个schedule 代表着即可就必须发送
            string sql = $@"INSERT INTO Exception_Shedule(Exception_Record_UID, SendDate, SendTime)  values({ dto.Exception_Record_UID}, GETDATE(), 0)";
            var ret = DataContext.Database.ExecuteSqlCommand(sql.ToString());
            return DataContext.SaveChanges();
        }

        public List<ExceptionDTO> ExportFixtrueReturn2Excel(ExceptionDTO searchModel)
        {
            #region 分页查询
            StringBuilder sql = new StringBuilder();
            sql.Append(@";With BG_Info AS (
                   select 
                       Organization_UID as BG_Organization_UID,
                       Organization_Name AS OP_TYPES 
                    from 
                       System_Organization 
                    where Organization_ID LIKE'20%'), 
                FunPlant AS( 
                    SELECT DISTINCT 
                        t2.Organization_Name as FunPlantName,
                        one.ParentOrg_UID as BG_Organization_UID,
                        t2.Organization_UID as FunPlant_Organization_UID 
                    FROM (
						    SELECT * FROM dbo.System_OrganizationBOM) one 
                            INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                            INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                            INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                        WHERE 
                           t3.Organization_Name='OP')
						   select 
	                            A.*,
	                            B.Exception_Name,
	                            B.Exception_Nub,
	                            C.Exception_Dept_Name,
	                            D.LineName,
                                F.Seq,
	                            F.StationName,
	                            E.Shift,
	                            U.User_Name,
	                            V.User_Name as Modified_UserName,
								isnull(H.Organization_Name,'Gold Line') as Plant,
								isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
								ISNULL(FunPlant.FunPlantName,'') as FunPlant,
                                P.Project_Name
                            from 
	                            Exception_Record A
	                            left join Exception_Code B on A.Exception_Code_UID=B.Exception_Code_UID
	                            left join Exception_Dept C on C.Exception_Dept_UID=B.Exception_Dept_UID
	                            left join GL_Line D on D.LineID=A.Origin_UID
	                            left join GL_Station F on F.StationID=A.SubOrigin_UID
	                            left join GL_ShiftTime E on E.ShiftTimeID=A.ShiftTimeID
	                            left join System_Users U on U.Account_UID=A.Created_UID
	                            left join System_Users V on V.Account_UID=A.Modified_UID
			                    left join System_Organization H on H.Organization_UID=C.Plant_Organization_UID
			                    left join BG_Info on BG_Info.BG_Organization_UID=C.BG_Organization_UID
			                    left join FunPlant on FunPlant.BG_Organization_UID=C.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=C.FunPlant_Organization_UID
                                left join System_Project P on P.Project_UID=A.Project_UID
                            where 1=1 ");
            StringBuilder sbWhere = new StringBuilder();
            if (searchModel.LineID != 0)
            {
                sbWhere.Append($@" and  A.Origin_UID={searchModel.LineID}");
            }

            if (searchModel.StationID != 0)
            {
                sbWhere.Append($@" and  A.SubOrigin_UID={searchModel.StationID}");
            }
            if (searchModel.ShiftTimeID != 0)
            {
                sbWhere.Append($@" and  A.ShiftTimeID={searchModel.ShiftTimeID}");
            }
            if (searchModel.WorkDate != DateTime.MinValue)
            {
                sbWhere.Append($@" and  A.WorkDate='{searchModel.WorkDate}'");
            }
            if (searchModel.Modified_Date_from != DateTime.MinValue)
            {
                sbWhere.Append($@" and  A.Created_Date>='{searchModel.Modified_Date_from.ToString("yyyy-MM-dd") + " 00:00:00"}'");
            }
            if (searchModel.Modified_Date_to != DateTime.MinValue)
            {
                sbWhere.Append($@" and  A.Created_Date<='{searchModel.Modified_Date_to.ToString("yyyy-MM-dd") + " 23:59:59"}'");
            }
            if (searchModel.Exception_Code_UID != 0 && searchModel.Exception_Code_UID != null)
            {
                sbWhere.Append($@" and  A.Exception_Code_UID={searchModel.Exception_Code_UID}");
            }

            if (searchModel.DelayDayNub != 0)
            {
                sbWhere.Append($@" and datediff(day,A.Created_Date,getdate()) >='{searchModel.DelayDayNub}'");
            }
            if (searchModel.Status >= 0)
            {
                sbWhere.Append($@" and A.Status ='{searchModel.Status}'");
            }
            if (searchModel.Exception_Dept_UID > 0)
            {
                sbWhere.Append($@" and B.Exception_Dept_UID ='{searchModel.Exception_Dept_UID}'");
            }

            if (searchModel.Plant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.Plant_Organization_UID={searchModel.Plant_Organization_UID}");
            }
            if (searchModel.BG_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.BG_Organization_UID={searchModel.BG_Organization_UID}");
            }
            if (searchModel.Funplant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.Funplant_Organization_UID={searchModel.Funplant_Organization_UID}");
            }
            if (searchModel.CustomerID != 0)
            {
                sbWhere.Append($@" and  D.CustomerID={searchModel.CustomerID}");
            }
            sql.Append(sbWhere);
            sql.Append($@" order by 
                        A.Status ASC,P.Project_Name,D.LineName,F.Seq");
            #endregion
            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList();

            return dbList;
        }

        public void ExceptionShedule()
        {
            #region 业务逻辑
            //ListDictionary Params = new ListDictionary();
            ////先查找需要发送的异常
            //Params.Add("type", 1);
            //Params.Add("uid", 0);
            var entities = DataContext.Database.SqlQuery<ExceptionRecordEntity>("exec usp_ExceptionSheduleQuery 1,0").ToList();
            try
            {
                if (entities.Count > 0)
                {
                    ILookup<string, ExceptionRecordEntity> exceKeyValue = entities.ToLookup(x => x.FlagName);
                    foreach (var item in exceKeyValue)
                    {
                        var dept_time = item.Key.Split(',').Select(s => int.Parse(s)).ToArray();
                        var sendObject = exceKeyValue[item.Key].ToList();
                        StringBuilder sbEmailRecord = new StringBuilder();
                        StringBuilder sbReply = new StringBuilder();//回复记录检查
                        sbEmailRecord = GenerateRecordBody();
                        List<string> send = new List<string>();//收件人
                        List<string> cc = new List<string>();//CC人员
                        foreach (var record in sendObject)//邮件内容
                        {
                            sbEmailRecord.Append("<tr>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Record_UID + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Project_Name + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.LineName + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Seq + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.StationName + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Created_Date.ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + Convert.ToDateTime(record.WorkDate).ToString("yyyy-MM-dd") + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Shift + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Dept_Name + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Nub + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Name + "</td>");

                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Contact_Person ?? "" + "</td>");
                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Contact_Phone ?? "" + "</td>");

                            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Note + "</td>");
                            string hourDiffer = (DateTime.Now - record.Created_Date).TotalHours.ToString("f1");
                            string dayDiffer = (DateTime.Now - record.Created_Date).TotalDays.ToString("f1");
                            if (Convert.ToDouble(dayDiffer) >= 1.0)
                            {
                                sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center;color:red\">" + Convert.ToDouble(dayDiffer) + " D" + "</td>");
                            }
                            else if (Convert.ToDouble(dayDiffer) < 1.0 && Convert.ToDouble(hourDiffer) >= 1.0)
                            {
                                sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center;color:red\">" + hourDiffer + " H" + "</td>");

                            }
                            else
                            {
                                sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "" + "</td>");

                            }
                            sbEmailRecord.Append("</tr>");
                            //检查该异常的回复记录

                            //var replyEntities=  db.Exception_Reply.Where(x => x.Exception_Record_UID == record.Exception_Record_UID).ToList();

                            var replyEntities = DataContext.Exception_Reply.Join(DataContext.System_Users, r => r.Reply_UID, u => u.Account_UID, (r, u) => new { r, u }).Where(m => m.r.Exception_Record_UID == record.Exception_Record_UID).OrderByDescending(x => x.r.Reply_Date).Select(m => new
                            {
                                Exception_Content = m.r.Exception_Content,
                                Reply_Date = m.r.Reply_Date,
                                User_Name = m.u.User_Name,
                                User_NTID = m.u.User_NTID
                            }).FirstOrDefault();
                            if (replyEntities != null)
                            {
                                if (sbReply.Length == 0)
                                {
                                    sbReply = GenerateReplyContent();
                                }
                                //foreach (var reply in replyEntities)
                                //{
                                sbReply.Append("<tr>");
                                sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Record_UID + "</td>");
                                sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + replyEntities.Exception_Content + "</td>");
                                sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + replyEntities.Reply_Date.ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                                sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + replyEntities.User_Name + "</td>");
                                sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + replyEntities.User_NTID + "</td>");
                                sbReply.Append("</tr>");
                                //}

                            }

                            //var sheduleEntities = DataContext.Exception_Shedule.Where(x => x.Exception_Record_UID == record.Exception_Record_UID && x.SendDate <= record.SendDate && x.IS_Send == 0).ToList();
                            //foreach (var shedule in sheduleEntities)
                            //{
                            //    shedule.IS_Send = 1;
                            //    shedule.UpdateDate = DateTime.Now;
                            //}
                            //DataContext.SaveChanges();

                        }

                        sbEmailRecord.Append("</table>");
                        if (sbReply.Length > 0)
                        {
                            sbReply.Append("</table>");
                        }
                        //收件人+CC人员

                        int time = dept_time[0];
                        int projectID = dept_time[1];
                        send = DataContext.Exception_Email.Where(x => x.Project_UID == projectID && x.ReceiveType == 1 && x.Exception_Times == time).Select(x => x.User_Email).Distinct().ToList();

                        cc = DataContext.Exception_Email.Where(x => (x.Project_UID == projectID && x.ReceiveType == 2 && x.Exception_Times == time) || (x.Project_UID == projectID && x.Exception_Times < time)).Select(x => x.User_Email).Distinct().ToList();
                        if (send.Count > 0)
                        {
                            SendEmailUsers(sbEmailRecord, sbReply, send, cc, sendObject[0].Project_Name);
                        }


                        Debug.WriteLine(item.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            #endregion
        }

        private StringBuilder GenerateReplyContent()
        {
            StringBuilder sbReply = new StringBuilder();
            sbReply.Append("</br><p> History reply records：");
            sbReply.Append("</p>");
            sbReply.Append("<table style=\"border:1px solid green;text-align: center\">");
            sbReply.Append("<tr>");
            sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + "System ID" + "</td>");
            sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + "Content" + "</td>");
            sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + "Reply Time" + "</td>");
            sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + "User Name" + "</td>");
            sbReply.Append("<td style=\"border:1px solid green;text-align: center\">" + "NTID" + "</td>");
            sbReply.Append("</tr>");
            return sbReply;
        }

        private StringBuilder GenerateRecordBody()
        {
            StringBuilder sbEmailRecord = new StringBuilder();
            sbEmailRecord.Append("</br><p> Issue details：");
            sbEmailRecord.Append("<table>");
            sbEmailRecord.Append("<tr>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "System ID" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Project" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Line" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Procedure" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Station" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Start Time" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Output Date" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Shift" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Department" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Issue Code" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Issue Reason" + "</td>");

            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Owner" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Contact" + "</td>");

            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Note" + "</td>");
            sbEmailRecord.Append("<td style=\"border:1px solid green;text-align: center\">" + "Delay" + "</td>");
            sbEmailRecord.Append("</tr>");
            return sbEmailRecord;
        }

        private void SendEmailUsers(StringBuilder sbEmailRecord, StringBuilder sbReply, List<string> sendEmail, List<string> ccEmail, string sbuject)
        {
            StringBuilder MailBody = new StringBuilder();

            MailBody.Append(sbEmailRecord).Append(sbReply);
            MailBody.Append("<p>&nbsp&nbsp Please login PIS system to view detail information :<a href=");
            MailBody.Append("http://cnctug0pdmsap01.corp.jabil.org/PIS_M/");
            MailBody.Append(">Login</a></p>");
            MailBody.Append("</body>");
            MailBody.Append("</html>");


            StringBuilder sbThreme = new StringBuilder();
            sbThreme.Append($"{sbuject} OLE Line Issue：Overdue reminder");
            //todo: 发布前取消注释
            //MailTo.Clear();
            //MailTo.Add("Oscar_Wu@jabil.com");
            //MailCC.Clear();
            //MailCC.Add("Oscar_Wu@jabil.com");
            var ret = EmailSend(sendEmail.ToArray(), ccEmail.ToArray(), sbThreme.ToString(), MailBody.ToString(), true, null);

        }
        public static bool EmailSend(string[] mailTo, string[] mailCcArray, string subject, string body, bool isBodyHtml,
                                  string[] attachmentsPath)
        {
            try
            {
                var @from = new MailAddress("PIS_System@jabil.com"); //使用指定的邮件地址初始化MailAddress实例
                var message = new MailMessage(); //初始化MailMessage实例 
                //向收件人地址集合添加邮件地址
                if (mailTo != null)
                {
                    //mailTo=mailTo.
                    foreach (string t in mailTo)
                    {
                        message.To.Add(t);
                    }
                }

                //向抄送收件人地址集合添加邮件地址
                if (mailCcArray != null)
                {
                    foreach (string t in mailCcArray)
                    {
                        message.CC.Add(t);
                    }
                }
                //发件人地址
                message.From = @from;

                //电子邮件的标题
                message.Subject = subject;

                //电子邮件的主题内容使用的编码
                message.SubjectEncoding = Encoding.UTF8;

                //电子邮件正文
                message.Body = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(body));

                //电子邮件正文的编码
                //message.BodyEncoding = Encoding.Default;
                //message.Priority = MailPriority.High;
                message.BodyEncoding = Encoding.UTF8;
                message.Priority = MailPriority.High;


                message.IsBodyHtml = true;

                //在有附件的情况下添加附件
                if (attachmentsPath != null && attachmentsPath.Length > 0)
                {
                    foreach (string path in attachmentsPath)
                    {
                        var attachFile = new Attachment(path);
                        message.Attachments.Add(attachFile);
                    }
                }
                try
                {
                    var smtp = new SmtpClient
                    {
                        Host = "corimc04.corp.jabil.org",
                        Port = 25
                    };

                    //将邮件发送到SMTP邮件服务器
                    smtp.Send(message);

                    //todo:记录日志
                    return true;
                }
                catch (SmtpException ex)
                {
                    string errorInfo = ex.Message;//错误信息
                    string functionName = new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name;//方法名称
                    string FormName = "Mail";//窗体名称


                    //todo:记录日志
                    return false;
                }
            }
            catch (SmtpException ex)
            {
                string errorInfo = ex.Message;//错误信息
                string functionName = new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name;//方法名称
                string FormName = "Mail";//窗体名称
                //todo:记录日志
                return false;
            }
        }

        public List<Line> FetchLineBasedPlantBGCustomer(ExceptionDTO dto)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select distinct 
                          B.LineID,
                          B.LineName 
                        from 
                            GL_Station A
	                    inner join GL_Line B on B.LineID=A.LineID
                        where 
		                A.IsGoldenLine=1 ");
            StringBuilder sbWhere = new StringBuilder();
            if (dto.Plant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  A.Plant_Organization_UID={dto.Plant_Organization_UID}");
            }
            if (dto.BG_Organization_UID != 0)
            {
                sbWhere.Append($@" and  A.BG_Organization_UID={dto.BG_Organization_UID}");
            }
            if (dto.Funplant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  A.FunPlant_Organization_UID={dto.Funplant_Organization_UID}");
            }
            if (dto.CustomerID != 0)
            {
                sbWhere.Append($@" and  B.CustomerID={dto.CustomerID}");
            }
            sql.Append(sbWhere);
            sql.Append(" order by LineName");
            var entities = DataContext.Database.SqlQuery<Line>(sql.ToString()).ToList();
            return entities;
        }

        public List<ReplyRecordDTO> ViewRecordReplyAPI(int uid)
        {
            var entity = DataContext.Exception_Reply.Join(DataContext.System_Users, u => u.Reply_UID, v => v.Account_UID, (u, v) => new { reply = u, users = v }).Where(x => x.reply.Exception_Record_UID == uid).Select(x => new ReplyRecordDTO
            {
                Reply_Date = x.reply.Reply_Date,
                Exception_Content = x.reply.Exception_Content,
                Modified_UserName = x.users.User_Name,
                Modified_UserNTID = x.users.User_NTID
            }).ToList();
            return entity;
        }

        public List<SystemUserDTO> FethAllEmail()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@";with CET_A as(
                            SELECT 
                                    Account_UID, 
                                    [User_Name],
                                    [Email],
                                    ROW_NUMBER() over (partition by Email order by Email) as Nub
                            FROM 
                                [dbo].[System_Users] 
                            where Email<>'')
                        select 
                                Account_UID,
                                [User_Name],
                                [Email] 
                        from 
                            CET_A 
                        where 
                            Nub=1");
            sql.Append($@" order by [Email]");
            var dbList = DataContext.Database.SqlQuery<SystemUserDTO>(sql.ToString()).ToList();
            return dbList;
        }

        public int SendEmailException(EmailSendDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Subject))
            {
                dto.Subject = "PIS_M Issue Handing";

            }
            //Array.ConvertAll(arr, s => int.Parse(s));
            //收件人
            int[] receiveIDs = Array.ConvertAll(dto.SendIDs.Split(','), s => Int32.Parse(s));
            var receive = DataContext.System_Users.Where(x => receiveIDs.Contains(x.Account_UID)).Select(x => x.Email).ToArray();
            //CC人员
            string[] cc = null;
            if (dto.CCIDs.Length > 0)
            {
                int[] ccIDs = Array.ConvertAll(dto.CCIDs.Split(','), s => Int32.Parse(s));

                cc = DataContext.System_Users.Where(x => ccIDs.Contains(x.Account_UID)).Select(x => x.Email).ToArray();
            }

            //主题

            //MaiBody:依次拿接收与发送人员
            //int[] exceIDs = Array.ConvertAll(dto.Exception_Record_UIDs.Split(','), s => Int32.Parse(s));

            StringBuilder MailBody = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            sql.Append($@"select 
				A.*,
				P.Project_Name,
				C.LineName,
				D.Seq,
				D.StationName,
				E.Exception_Nub,
				E.Exception_Name,
				F.Exception_Dept_Name,
				G.Shift,
				U.User_Name,
				V.User_Name as Modified_UserName
			from   
			    Exception_Record A 
			    left join GL_Line C on C.LineID=A.Origin_UID
			    left join GL_Station D on D.StationID=A.SubOrigin_UID
			    inner join Exception_Code E on E.Exception_Code_UID=A.Exception_Code_UID
			    inner join Exception_Dept F on F.Exception_Dept_UID=E.Exception_Dept_UID
			    left join GL_ShiftTime G on G.ShiftTimeID=A.ShiftTimeID
			    left join System_Users U on U.Account_UID=A.Created_UID
				left join System_Users V on V.Account_UID=A.Created_UID
			    left join System_Project P on P.Project_UID=C.CustomerID
			where A.Exception_Record_UID in ({dto.Exception_Record_UIDs})");
            var entities = DataContext.Database.SqlQuery<ExceptionRecordEntity>(sql.ToString()).ToList();


            MailBody.Append("</br><p> Issue details：");
            MailBody.Append("<table>");
            MailBody.Append("<tr>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "System ID" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Project" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Line" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Procedure" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Station" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Status" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Output Date" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Shift" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Department" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Issue Code" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Issue Reason" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Owner" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Contact" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Create User" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Create Time" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Modified by" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Modified Time" + "</td>");
            MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Note" + "</td>");
            MailBody.Append("</tr>");

            foreach (var record in entities)
            {
                MailBody.Append("<tr>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Record_UID + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Project_Name + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.LineName + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Seq + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.StationName + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + (record.Status == 0 ? "Open" : "Close") + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + Convert.ToDateTime(record.WorkDate).ToString("yyyy-MM-dd") + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Shift + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Dept_Name + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Nub + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Exception_Name + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Contact_Person ?? "" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Contact_Phone ?? "" + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.User_Name + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Created_Date.ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Modified_UserName + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Modified_Date.ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                MailBody.Append("<td style=\"border:1px solid green;text-align: center\">" + record.Note + "</td>");
                MailBody.Append("</tr>");
                var reply = DataContext.Exception_Reply.Join(DataContext.System_Users, r => r.Reply_UID, u => u.Account_UID, (r, u) => new { r, u }).Where(m => m.r.Exception_Record_UID == record.Exception_Record_UID).OrderByDescending(x => x.r.Reply_Date).Select(m => new
                {
                    Exception_Content = m.r.Exception_Content,
                    Reply_Date = m.r.Reply_Date,
                    User_Name = m.u.User_Name,
                    User_NTID = m.u.User_NTID
                }).FirstOrDefault();
                if (reply != null)
                {

                    MailBody.Append("<tr><td rowspan = \"2\" style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">" + "Reply Content" + "</td>");
                    MailBody.Append("<td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">NTID</td><td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\">人员</td ><td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\"> Content </td><td style=\"border:1px solid green;text-align: center; background-color:antiquewhite;\"> Reply Time </td>");
                    MailBody.Append("</tr>");
                    MailBody.Append($"<tr><td style=\"border:1px solid green;text-align: center\"> {reply.User_NTID}</td > <td style=\"border:1px solid green;text-align: center\"> {reply.User_Name}</td><td style=\"border:1px solid green;text-align: center\"> {reply.Exception_Content}</td ><td style=\"border:1px solid green;text-align: center\"> {reply.Reply_Date.ToString("yyyy-MM-dd HH:mm:ss")}</td ></tr >");
                }

            }

            MailBody.Append("<p>&nbsp&nbsp Please login PIS system to view detail information :<a href=");
            MailBody.Append("http://cnctug0pdmsap01.corp.jabil.org/PIS_M/");
            MailBody.Append(">Login</a></p>");
            MailBody.Append("</body>");
            MailBody.Append("</html>");
            EmailSend(receive, cc, dto.Subject, MailBody.ToString(), true, null);

            return 1;
        }

        public List<ExceptionDTO> ExportSomeRecord2Excel(string uids)
        {
            // int[] exceIDs = uids.Split(',').Select(s => int.Parse(s)).ToArray();
            StringBuilder sql = new StringBuilder();
            sql.Append($@";With BG_Info AS (
                   select 
                       Organization_UID as BG_Organization_UID,
                       Organization_Name AS OP_TYPES 
                    from 
                       System_Organization 
                    where Organization_ID LIKE'20%'), 
                FunPlant AS( 
                    SELECT DISTINCT 
                        t2.Organization_Name as FunPlantName,
                        one.ParentOrg_UID as BG_Organization_UID,
                        t2.Organization_UID as FunPlant_Organization_UID 
                    FROM (
						    SELECT * FROM dbo.System_OrganizationBOM) one 
                            INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                            INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                            INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                        WHERE 
                           t3.Organization_Name='OP')
						   select 
	                            A.*,
	                            B.Exception_Name,
	                            B.Exception_Nub,
	                            C.Exception_Dept_Name,
	                            D.LineName,
                                F.Seq,
	                            F.StationName,
	                            E.Shift,
	                            U.User_Name,
	                            V.User_Name as Modified_UserName,
								isnull(H.Organization_Name,'Gold Line') as Plant,
								isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
								ISNULL(FunPlant.FunPlantName,'') as FunPlant,
                                P.Project_Name
                            from 
	                            Exception_Record A
	                            left join Exception_Code B on A.Exception_Code_UID=B.Exception_Code_UID
	                            left join Exception_Dept C on C.Exception_Dept_UID=B.Exception_Dept_UID
	                            left join GL_Line D on D.LineID=A.Origin_UID
	                            left join GL_Station F on F.StationID=A.SubOrigin_UID
	                            left join GL_ShiftTime E on E.ShiftTimeID=A.ShiftTimeID
	                            left join System_Users U on U.Account_UID=A.Created_UID
	                            left join System_Users V on V.Account_UID=A.Modified_UID
			                    left join System_Organization H on H.Organization_UID=C.Plant_Organization_UID
			                    left join BG_Info on BG_Info.BG_Organization_UID=C.BG_Organization_UID
			                    left join FunPlant on FunPlant.BG_Organization_UID=C.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=C.FunPlant_Organization_UID
                                left join System_Project P on P.Project_UID=A.Project_UID
                            where A.Exception_Record_UID in ({uids}) 
                            order by A.Status ASC");
            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList();

            return dbList;

        }

        public ShiftTime FetchShiftTimeDetail(int uid)
        {
            string sqlStr = $@"select 
                                ShiftTimeID, 
                                Shift,
                                StartTime, 
                                End_Time 
                            from GL_ShiftTime 
                            where ShiftTimeID={uid}";
            var res = DataContext.Database.SqlQuery<ShiftTime>(sqlStr).FirstOrDefault();
            return res;
        }

        public List<ShiftTime> FetchShiftTimeBasedBG(int plantuid, int bguid)
        {
            string sqlStr = $@"select 
                                ShiftTimeID, 
                                Shift, 
                                StartTime,
                                End_Time
                            from GL_ShiftTime 
                            where Plant_Organization_UID={plantuid} and BG_Organization_UID={bguid}";
            var res = DataContext.Database.SqlQuery<ShiftTime>(sqlStr).ToList();
            return res;
        }

        public string GetStartDate(DashboardSearchDTO search)
        {



            string startDate = "";
            string startHour = "";
            string startMin = "";
            string startDatetime = "";
            if (search.ShiftTimeID != 0)
            {
                var shiftEntity = DataContext.GL_ShiftTime.Find(search.ShiftTimeID);
                startHour = shiftEntity.StartTime.Split(':')[0].ToString();
                startMin= shiftEntity.StartTime.Split(':')[1].ToString();
            }
            else
            {
                 startHour = "00";
                 startMin = "00";
            }

            
            if (!string.IsNullOrEmpty(search.WorkDate))
            {
                startDate = Convert.ToDateTime(search.WorkDate).ToString("yyy-MM-dd");
            }
            else
            {
                startDate = DateTime.Now.ToString("yyy-MM-dd");
            }
            if (!string.IsNullOrEmpty(search.StartH)&& search.StartH!="-1")
            {
                startHour = search.StartH;
                if (!string.IsNullOrEmpty(search.StartM))
                {
                    startMin = search.StartM;
                }
            }
            startDatetime = Convert.ToDateTime(startDate + " " + startHour + ":" + startMin + ":00").ToString("yyyy-MM-dd HH:mm:ss");
            return startDatetime;
        }

        public ChartsModel QueryExceptionRecordDashboard(DashboardSearchDTO dto)
        {
            
            
            DataSet ds = new DataSet();
            SqlConnection sqlConn = new SqlConnection();
            sqlConn = DataContext.Database.Connection as SqlConnection;
            SqlDataAdapter sqlDa = new SqlDataAdapter("usp_ExceptionDowntime", sqlConn);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@StartDatetime", SqlDbType.DateTime));
            sqlDa.SelectCommand.Parameters[0].Value = Convert.ToDateTime(GetStartDate(dto));
            sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@orginationID", SqlDbType.BigInt));
            sqlDa.SelectCommand.Parameters[1].Value = dto.Plant_Organization_UID;
            sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@optypeID", SqlDbType.BigInt));
            sqlDa.SelectCommand.Parameters[2].Value = dto.BG_Organization_UID;
            sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@projectID", SqlDbType.BigInt));
            sqlDa.SelectCommand.Parameters[3].Value = dto.Project_UID;
            if (dto.LineID == -1)
            {
                sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@lineID", SqlDbType.BigInt));
                sqlDa.SelectCommand.Parameters[4].Value =0;
                sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@lineName", SqlDbType.NVarChar));
                sqlDa.SelectCommand.Parameters[5].Value = dto.LineName;
            }
            else
            {
                sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@lineID", SqlDbType.BigInt));
                sqlDa.SelectCommand.Parameters[4].Value = dto.LineID;
                sqlDa.SelectCommand.Parameters.Add(new SqlParameter("@lineName", SqlDbType.NVarChar));
                sqlDa.SelectCommand.Parameters[5].Value = "";
            }
            sqlDa.Fill(ds);
            DataTable tb = ds.Tables[0];
            ChartsModel chartModel = new ChartsModel();
            chartModel.xLabel = new List<string>();
            chartModel.yLabel = new List<decimal>();
            foreach (DataRow item in tb.Rows)
            {
                chartModel.xLabel.Add(Convert.ToDateTime(item["xLabel"]).ToString("MM/dd HH:mm"));
                chartModel.yLabel.Add(Convert.ToDecimal(item["yLabel"]));
            }
            return chartModel;
        }

        public List<ExceptionDTO> QueryDowntimeRecords(DashboardSearchDTO search)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@";With BG_Info AS (
                   select 
                       Organization_UID as BG_Organization_UID,
                       Organization_Name AS OP_TYPES 
                    from 
                       System_Organization 
                    where Organization_ID LIKE'20%'), 
                FunPlant AS( 
                    SELECT DISTINCT 
                        t2.Organization_Name as FunPlantName,
                        one.ParentOrg_UID as BG_Organization_UID,
                        t2.Organization_UID as FunPlant_Organization_UID 
                    FROM (
						    SELECT * FROM dbo.System_OrganizationBOM) one 
                            INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                            INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                            INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                        WHERE 
                           t3.Organization_Name='OP')
						   select 
	                            A.*,
	                            B.Exception_Name,
	                            B.Exception_Nub,
	                            C.Exception_Dept_Name,
	                            D.LineName,
                                F.Seq,
	                            F.StationName,
	                            E.Shift,
	                            U.User_Name,
	                            V.User_Name as Modified_UserName,
								isnull(H.Organization_Name,'Gold Line') as Plant,
								isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
								ISNULL(FunPlant.FunPlantName,'') as FunPlant,
                                P.Project_Name,
								DATEDIFF(MINUTE,A.Created_Date,isnull(A.End_Date,GETDATE())) as Elapsed
                            from 
	                            Exception_Record A
	                            left join Exception_Code B on A.Exception_Code_UID=B.Exception_Code_UID
	                            left join Exception_Dept C on C.Exception_Dept_UID=B.Exception_Dept_UID
	                            left join GL_Line D on D.LineID=A.Origin_UID
	                            left join GL_Station F on F.StationID=A.SubOrigin_UID
	                            left join GL_ShiftTime E on E.ShiftTimeID=A.ShiftTimeID
	                            left join System_Users U on U.Account_UID=A.Created_UID
	                            left join System_Users V on V.Account_UID=A.Modified_UID
			                    left join System_Organization H on H.Organization_UID=C.Plant_Organization_UID
			                    left join BG_Info on BG_Info.BG_Organization_UID=C.BG_Organization_UID
			                    left join FunPlant on FunPlant.BG_Organization_UID=C.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=C.FunPlant_Organization_UID
								left join System_Project P on P.Project_UID=A.Project_UID
                            where 1=1 ");
            StringBuilder sbWhere = new StringBuilder();
            if (search.LineID > 0)
            {
                sbWhere.Append($@" and  A.Origin_UID={search.LineID}");
            }
            if (search.LineID == -1)
            {
                sbWhere.Append($@" and  LOWER(D.LineName) like '{search.LineName}%'");
            }

            string startDate = GetStartDate(search);
            string endDate = Convert.ToDateTime(startDate).AddHours(24).ToString("yyyy-MM-dd HH:mm:ss");
            //if (search.DelayDayNub > 0)
            //    //判断他最后日期有没有选择
            //    endDate= Convert.ToDateTime(endDate).AddHours(0 - search.DelayDayNub).ToString("yyyy-MM-dd HH:mm::ss");
            sbWhere.Append($@" and  A.Created_Date>='{startDate}'");
            sbWhere.Append($@" and  A.Created_Date<='{endDate}'");
            if (search.Plant_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.Plant_Organization_UID={search.Plant_Organization_UID}");
            }
            if (search.BG_Organization_UID != 0)
            {
                sbWhere.Append($@" and  D.BG_Organization_UID={search.BG_Organization_UID}");
            }
            if (search.Project_UID != 0)
            {
                sbWhere.Append($@" and  A.Project_UID={search.Project_UID}");
            }

            sql.Append(sbWhere);
            sql.Append($@" order by 
                        A.Status ASC,Elapsed desc,P.Project_Name,D.LineName,F.Seq");

            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList();
            return dbList;
        }

        public ExceptionDTO FetchExceptionDetail(int uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($@";With BG_Info AS (
                   select 
                       Organization_UID as BG_Organization_UID,
                       Organization_Name AS OP_TYPES 
                    from 
                       System_Organization 
                    where Organization_ID LIKE'20%'), 
                FunPlant AS( 
                    SELECT DISTINCT 
                        t2.Organization_Name as FunPlantName,
                        one.ParentOrg_UID as BG_Organization_UID,
                        t2.Organization_UID as FunPlant_Organization_UID 
                    FROM (
						    SELECT * FROM dbo.System_OrganizationBOM) one 
                            INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                            INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                            INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                        WHERE 
                           t3.Organization_Name='OP')
						   select 
	                            A.*,
	                            B.Exception_Name,
	                            B.Exception_Nub,
	                            C.Exception_Dept_Name,
	                            D.LineName,
                                F.Seq,
	                            F.StationName,
	                            E.Shift,
	                            U.User_Name,
	                            V.User_Name as Modified_UserName,
								isnull(H.Organization_Name,'Gold Line') as Plant,
								isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
								ISNULL(FunPlant.FunPlantName,'') as FunPlant,
                                P.Project_Name
                            from 
	                            Exception_Record A
	                            left join Exception_Code B on A.Exception_Code_UID=B.Exception_Code_UID
	                            left join Exception_Dept C on C.Exception_Dept_UID=B.Exception_Dept_UID
	                            left join GL_Line D on D.LineID=A.Origin_UID
	                            left join GL_Station F on F.StationID=A.SubOrigin_UID
	                            left join GL_ShiftTime E on E.ShiftTimeID=A.ShiftTimeID
	                            left join System_Users U on U.Account_UID=A.Created_UID
	                            left join System_Users V on V.Account_UID=A.Modified_UID
			                    left join System_Organization H on H.Organization_UID=C.Plant_Organization_UID
			                    left join BG_Info on BG_Info.BG_Organization_UID=C.BG_Organization_UID
			                    left join FunPlant on FunPlant.BG_Organization_UID=C.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=C.FunPlant_Organization_UID
								left join System_Project P on P.Project_UID=A.Project_UID
                            where A.Exception_Record_UID={uid}");

            var entity = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).FirstOrDefault();
            return entity;
        }

        public ReplyRecordDTO FetchLatestReply(int uid)
        {
            ReplyRecordDTO dto = new ReplyRecordDTO();
            var entity = DataContext.Exception_Reply.Where(x => x.Exception_Record_UID == uid).OrderByDescending(x => x.Reply_Date).FirstOrDefault();
            if (entity != null)
            {
                dto.Modified_UserName= DataContext.System_Users.Find(entity.Reply_UID).User_Name;
                dto.Reply_Date = entity.Reply_Date;
                dto.Exception_Content = entity.Exception_Content;
            }
            else
            {
                dto.Modified_UserName ="";
                dto.Reply_Date = null;
                dto.Exception_Content = "";
            }

            return dto;
        }
    }

    public static class ServiceRecurring
    {

        private static readonly object _syncRoot = new object();
        public static void ExceptionEmailMethod()
        {
            lock (_syncRoot)
            {

            }
        }
    }
}
