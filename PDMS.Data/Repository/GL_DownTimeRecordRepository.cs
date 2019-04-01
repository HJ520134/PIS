using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.EntityDTO;

namespace PDMS.Data.Repository
{

    public interface IGL_DownTimeRecordRepository : IRepository<GL_FourQDownTimeRecordDTO>
    {
        List<GLFourQDTModel> GetDownTimeRecord(GLFourQParamModel searchModel);
        List<GLFourQDTModel> GetFourQDTTypeDetail(GLFourQParamModel searchModel);
        List<GLFourQDTModel> GetPaynterChartDetial(GLFourQParamModel searchModel);
    }

    public class GL_DownTimeRecordRepository : RepositoryBase<GL_FourQDownTimeRecordDTO>, IGL_DownTimeRecordRepository
    {
        public GL_DownTimeRecordRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<GLFourQDTModel> GetDownTimeRecord(GLFourQParamModel searchModel)
        {
            string sql="";
            #region Daily SQL
            if (searchModel.ReportType == "Daily")
                sql = string.Format(@"Declare @mindate date
                            Declare @maxdate date  
                            set @mindate  = DATEADD(day, -13, '{0}')  
                            set @maxdate  = '{0}'  
                            ;with temptab(date) as  
                            ( select @mindate  
                            union all  
                            select dateadd(d,1,temptab.date) as date  
                            from temptab  
                            where dateadd(d,1,temptab.date)<=@maxdate )

                            select convert(varchar, f.WorkDate, 111) as DTName,isnull(dt2.dt_one,0)DTTime from
                            (select WorkDate,sum(downtime)dt_one
                            from
                            (SELECT [Exception_Record_UID]
                                  ,[Origin_UID]
	                              ,c.LineName
                                  ,[SubOrigin_UID]
	                              ,b.StationName
                                  ,a.[Exception_Code_UID]
	                              ,d.Exception_Dept_UID
	                              ,e.Exception_Dept_Name
	                              ,e.Plant_Organization_UID
	                              ,e.BG_Organization_UID
	                              ,e.Funplant_Organization_UID
                                  ,[Project_UID]
                                  ,[WorkDate]
                                  ,[ShiftTimeID]
                                  ,[Status]
                                  ,[End_Date]
                                  ,[Note]
                                  ,a.[Created_UID]
                                  ,a.[Created_Date]
                                  ,a.[Modified_UID]
                                  ,a.[Modified_Date]
                                  ,[Contact_Person]
                                  ,[Contact_Phone]
	                              ,case when End_Date is null then DATEDIFF(MINUTE, a.Created_Date,GETDATE()  )
	                              else DATEDIFF(Hour, a.Created_Date,End_Date  ) end as downtime
                              FROM [dbo].[Exception_Record] a
                            left join GL_Station b on a.SubOrigin_UID=b.StationID
                            left join GL_Line c on a.Origin_UID = c.LineID
                            left join Exception_Code d on a.Exception_Code_UID = d.Exception_Code_UID
                            left join Exception_Dept e on d.Exception_Dept_UID = e.Exception_Dept_UID
                            where 1=1
                            and a.WorkDate >= DATEADD(day, -13, '{0}') and a.WorkDate <= '{0}' ", searchModel.EndTime.ToString("yyyy-MM-dd"));
            #endregion
            #region Month SQL
            if (searchModel.ReportType == "Month")
                sql += string.Format(@"Declare @mindate date
                                        Declare @maxdate date
                                        set @mindate = DATEADD(MONTH, -11, dateadd(dd, -datepart(dd, '{0}') + 1, '{0}'))
                                        set @maxdate = dateadd(dd, -datepart(dd, '{0}') + 1, '{0}')
                                        ; with temptab(date) as
                                         (select @mindate
                                        union all
                                        select dateadd(MONTH,1,temptab.date) as date
                                        from temptab
                                        where dateadd(MONTH, 1, temptab.date) <= @maxdate )

                                        select f.WorkDate,CONVERT(varchar(10), month(f.WorkDate))DTName,isnull(dt2.dt_one, 0)DTTime from
                                         (select month_fisrt_day, sum(downtime) dt_one from
                                         (SELECT[Exception_Record_UID]
                                               ,[Origin_UID]
                                               , c.LineName
                                               ,[SubOrigin_UID]
                                               , b.StationName
                                               , a.[Exception_Code_UID]
                                               , d.Exception_Dept_UID
                                               , e.Exception_Dept_Name
                                               , e.Plant_Organization_UID
                                               , e.BG_Organization_UID
                                               , e.Funplant_Organization_UID
                                               ,[Project_UID]
                                               ,[WorkDate]
                                               ,[ShiftTimeID]
                                               ,[Status]
                                               ,[End_Date]
                                               ,[Note]
                                               , a.[Created_UID]
                                               , a.[Created_Date]
                                               , a.[Modified_UID]
                                               , a.[Modified_Date]
                                               ,[Contact_Person]
                                               ,[Contact_Phone]
                                               ,case when End_Date is null then DATEDIFF(MINUTE, a.Created_Date, GETDATE()  )
	                                          else DATEDIFF(Hour, a.Created_Date, End_Date) end as downtime
	                                          ,(select dateadd(dd, -datepart(dd, WorkDate) + 1, WorkDate))month_fisrt_day
                                               FROM[dbo].[Exception_Record]
                                                a
                                            left join GL_Station b on a.SubOrigin_UID=b.StationID
                                            left join GL_Line c on a.Origin_UID = c.LineID
                                            left join Exception_Code d on a.Exception_Code_UID = d.Exception_Code_UID
                                            left join Exception_Dept e on d.Exception_Dept_UID = e.Exception_Dept_UID
                                            where a.WorkDate >= dateadd(dd,-datepart(dd, DATEADD(day, -365, '{0}'))+1,DATEADD(day, -365, '{0}')) and a.WorkDate <= dateadd(dd,-datepart(dd,'{0}') ,dateadd(mm,1,'{0}'))", searchModel.EndTime.ToString("yyyy-MM-dd"));
            #endregion
            #region Week SQL
            if (searchModel.ReportType == "Week")
                sql += string.Format(@"Declare @mindate date
                                        Declare @maxdate date  
                                        set @mindate  = DATEADD(day, -84, '{0}')  
                                        set @maxdate  = '{0}'  
                                        ;with temptab(date) as  
                                        ( select @mindate  
                                        union all  
                                        select dateadd(week,1,temptab.date) as date  
                                        from temptab  
                                        where dateadd(week,1,temptab.date)<=@maxdate )

                                        select f.date as WorkDate,CONVERT(varchar(10), f.week_num)DTName,isnull(dt2.dt_one,0)DTTime from (
                                        select week_num,sum(downtime)dt_one
                                        from
                                        (SELECT [Exception_Record_UID]
                                              ,[Origin_UID]
	                                          ,c.LineName
                                              ,[SubOrigin_UID]
	                                          ,b.StationName
                                              ,a.[Exception_Code_UID]
	                                          ,d.Exception_Dept_UID
	                                          ,e.Exception_Dept_Name
	                                          ,e.Plant_Organization_UID
	                                          ,e.BG_Organization_UID
	                                          ,e.Funplant_Organization_UID
                                              ,[Project_UID]
                                              ,[WorkDate]
                                              ,[ShiftTimeID]
                                              ,[Status]
                                              ,[End_Date]
                                              ,[Note]
                                              ,a.[Created_UID]
                                              ,a.[Created_Date]
                                              ,a.[Modified_UID]
                                              ,a.[Modified_Date]
                                              ,[Contact_Person]
                                              ,[Contact_Phone]
	                                          ,case when End_Date is null then DATEDIFF(MINUTE, a.Created_Date,GETDATE()  )
	                                          else DATEDIFF(Hour, a.Created_Date,End_Date  ) end as downtime
	                                          ,DATEPART(WEEK,WorkDate) AS week_num 
                                          FROM [dbo].[Exception_Record] a
                                        left join GL_Station b on a.SubOrigin_UID=b.StationID
                                        left join GL_Line c on a.Origin_UID = c.LineID
                                        left join Exception_Code d on a.Exception_Code_UID = d.Exception_Code_UID
                                        left join Exception_Dept e on d.Exception_Dept_UID = e.Exception_Dept_UID
                                        where a.WorkDate >=  dateadd(wk, datediff(wk,0,DATEADD(day, -84, '{0}') ), 0) and a.WorkDate<=dateadd(wk, datediff(wk,0,'{0}'), 6)", searchModel.EndTime.ToString("yyyy-MM-dd"));
            #endregion

            if (searchModel.Plant_Organization_UID != 0)
                sql += string.Format("and e.Plant_Organization_UID={0}", searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                sql += string.Format("and e.BG_Organization_UID={0}", searchModel.BG_Organization_UID);
            if (searchModel.LineID != 0)
                sql += string.Format("and Origin_UID={0}", searchModel.LineID);
            if (searchModel.StationID != 0)
                sql += string.Format("and SubOrigin_UID={0}", searchModel.StationID);
            if (searchModel.CustomerID != 0)
                sql += string.Format("and Project_UID={0}", searchModel.CustomerID);
            //全天
            if (searchModel.ShiftTimeID != -1)
            {
                sql += string.Format("and ShiftTimeID={0}", searchModel.ShiftTimeID);
            }
            if(searchModel.ReportType== "Daily")
                sql += @" )dt
                    GROUP by WorkDate)dt2
                    right join (select date as WorkDate,0 dt_one from temptab)f on f.WorkDate=dt2.WorkDate ";
            if (searchModel.ReportType == "Month")
                sql += @")dt
                    group by month_fisrt_day)dt2
                    right join (select date as WorkDate,0 dt_one from temptab)f on f.WorkDate=dt2.month_fisrt_day";
            if (searchModel.ReportType == "Week")
                sql += @")dt
                    GROUP by week_num) dt2
                    right join (select date,DATEPART(WEEK,date) AS week_num,0 as dt_one from temptab) f on dt2.week_num=f.week_num";

            var dblist = DataContext.Database.SqlQuery<GLFourQDTModel>(sql).ToList();
            return dblist;
        }

        public List<GLFourQDTModel> GetFourQDTTypeDetail(GLFourQParamModel searchModel)
        {
            string sql = "";
            sql = string.Format(@"select Exception_Dept_Name as DTName,sum(downtime) as DTTime from (
                                    SELECT [Exception_Record_UID]
                                          ,[Origin_UID]
	                                      ,c.LineName
                                          ,[SubOrigin_UID]
	                                      ,b.StationName
                                          ,a.[Exception_Code_UID]
	                                      ,d.Exception_Dept_UID
	                                      ,e.Exception_Dept_Name
	                                      ,e.Plant_Organization_UID
	                                      ,e.BG_Organization_UID
	                                      ,e.Funplant_Organization_UID
                                          ,[Project_UID]
                                          ,[WorkDate]
                                          ,[ShiftTimeID]
                                          ,[Status]
                                          ,[End_Date]
                                          ,[Note]
                                          ,a.[Created_UID]
                                          ,a.[Created_Date]
                                          ,a.[Modified_UID]
                                          ,a.[Modified_Date]
                                          ,[Contact_Person]
                                          ,[Contact_Phone]
	                                      ,case when End_Date is null then DATEDIFF(MINUTE, a.Created_Date,GETDATE()  )
	                                      else DATEDIFF(Hour, a.Created_Date,End_Date  ) end as downtime
                                      FROM [dbo].[Exception_Record] a
                                    left join GL_Station b on a.SubOrigin_UID=b.StationID
                                    left join GL_Line c on a.Origin_UID = c.LineID
                                    left join Exception_Code d on a.Exception_Code_UID = d.Exception_Code_UID
                                    left join Exception_Dept e on d.Exception_Dept_UID = e.Exception_Dept_UID
                                    where 1=1 ");
            if (searchModel.ReportType == "Daily")
                sql += string.Format(@"and a.WorkDate = '{0}'", searchModel.Param_Name);
            if (searchModel.ReportType == "Month")
                sql += string.Format(@"and a.WorkDate >= dateadd(dd,-datepart(dd,'{0}')+1,'{0}')  and a.WorkDate <= dateadd(dd,-datepart(dd,'{0}') ,dateadd(mm,1,'{0}'))", searchModel.Param_Name);
            if (searchModel.ReportType == "Week")
                sql += string.Format(@"and a.WorkDate >= dateadd(wk, datediff(wk,0,'{0}'), 0) and a.WorkDate <= dateadd(wk, datediff(wk,0,'{0}'), 6)", searchModel.Param_Name);
            if (searchModel.Plant_Organization_UID != 0)
                sql += string.Format("and e.Plant_Organization_UID={0}", searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                sql += string.Format("and e.BG_Organization_UID={0}", searchModel.BG_Organization_UID);
            if (searchModel.LineID != 0)
                sql += string.Format("and Origin_UID={0}", searchModel.LineID);
            if (searchModel.StationID != 0)
                sql += string.Format("and SubOrigin_UID={0}", searchModel.StationID);
            if (searchModel.CustomerID != 0)
                sql += string.Format("and Project_UID={0}", searchModel.CustomerID);
            //全天
            if (searchModel.ShiftTimeID != -1)
            {
                sql += string.Format("and ShiftTimeID={0}", searchModel.ShiftTimeID);
            }

            sql += " )dt group by Exception_Dept_Name order by DTTime desc";

            var dblist = DataContext.Database.SqlQuery<GLFourQDTModel>(sql).ToList();
            return dblist;
        }

        public List<GLFourQDTModel> GetPaynterChartDetial(GLFourQParamModel searchModel)
        {
            string sql = "";
            #region Daily SQL
            if (searchModel.ReportType == "Daily")
                sql += string.Format(@"Declare @mindate date
                            Declare @maxdate date  
                            set @mindate  = DATEADD(day, -13, '{0}')  
                            set @maxdate  = '{0}'  
                            ;with temptab(date) as  
                            ( select @mindate  
                            union all  
                            select dateadd(d,1,temptab.date) as date  
                            from temptab  
                            where dateadd(d,1,temptab.date)<=@maxdate )

                                    select f.WorkDate,CONVERT(varchar(10), f.WorkDate)DTMon,isnull(dt2.DTTime,0)DTTime,isnull(dt2.DTName,'')DTName from
                                    (select Exception_Dept_Name as DTName,WorkDate,sum(downtime) as DTTime from (
                                    SELECT [Exception_Record_UID]
                                            ,[Origin_UID]
	                                        ,c.LineName
                                            ,[SubOrigin_UID]
	                                        ,b.StationName
                                            ,a.[Exception_Code_UID]
	                                        ,d.Exception_Dept_UID
	                                        ,e.Exception_Dept_Name
	                                        ,e.Plant_Organization_UID
	                                        ,e.BG_Organization_UID
	                                        ,e.Funplant_Organization_UID
                                            ,[Project_UID]
                                            ,[WorkDate]
                                            ,[ShiftTimeID]
                                            ,[Status]
                                            ,[End_Date]
                                            ,[Note]
                                            ,a.[Created_UID]
                                            ,a.[Created_Date]
                                            ,a.[Modified_UID]
                                            ,a.[Modified_Date]
                                            ,[Contact_Person]
                                            ,[Contact_Phone]
	                                        ,case when End_Date is null then DATEDIFF(MINUTE, a.Created_Date,GETDATE()  )
	                                        else DATEDIFF(Hour, a.Created_Date,End_Date  ) end as downtime
	                                        ,MONTH(WorkDate)mon
                                        FROM [dbo].[Exception_Record] a
                                    left join GL_Station b on a.SubOrigin_UID=b.StationID
                                    left join GL_Line c on a.Origin_UID = c.LineID
                                    left join Exception_Code d on a.Exception_Code_UID = d.Exception_Code_UID
                                    left join Exception_Dept e on d.Exception_Dept_UID = e.Exception_Dept_UID", searchModel.EndTime.ToString("yyyy-MM-dd"));
            #endregion
            #region Month SQL
            if (searchModel.ReportType == "Month")
            sql += string.Format(@"Declare @mindate date
                                Declare @maxdate date  
                                set @mindate  = DATEADD(MONTH, -11, dateadd(dd,-datepart(dd,'{0}')+1,'{0}'))  
                                set @maxdate  = dateadd(dd,-datepart(dd,'{0}')+1,'{0}') 
                                ;with temptab(date) as  
                                ( select @mindate  
                                union all  
                                select dateadd(MONTH,1,temptab.date) as date  
                                from temptab  
                                where dateadd(MONTH,1,temptab.date)<=@maxdate )

                                select f.WorkDate,CONVERT(varchar(10), f.mon)DTMon,isnull(dt2.DTTime,0)DTTime,isnull(dt2.DTName,'')DTName from
                                (select Exception_Dept_Name as DTName,mon,sum(downtime) as DTTime from (
                                SELECT [Exception_Record_UID]
                                        ,[Origin_UID]
	                                    ,c.LineName
                                        ,[SubOrigin_UID]
	                                    ,b.StationName
                                        ,a.[Exception_Code_UID]
	                                    ,d.Exception_Dept_UID
	                                    ,e.Exception_Dept_Name
	                                    ,e.Plant_Organization_UID
	                                    ,e.BG_Organization_UID
	                                    ,e.Funplant_Organization_UID
                                        ,[Project_UID]
                                        ,[WorkDate]
                                        ,[ShiftTimeID]
                                        ,[Status]
                                        ,[End_Date]
                                        ,[Note]
                                        ,a.[Created_UID]
                                        ,a.[Created_Date]
                                        ,a.[Modified_UID]
                                        ,a.[Modified_Date]
                                        ,[Contact_Person]
                                        ,[Contact_Phone]
	                                    ,case when End_Date is null then DATEDIFF(MINUTE, a.Created_Date,GETDATE()  )
	                                    else DATEDIFF(Hour, a.Created_Date,End_Date  ) end as downtime
	                                    ,MONTH(WorkDate)mon
                                    FROM [dbo].[Exception_Record] a
                                left join GL_Station b on a.SubOrigin_UID=b.StationID
                                left join GL_Line c on a.Origin_UID = c.LineID
                                left join Exception_Code d on a.Exception_Code_UID = d.Exception_Code_UID
                                left join Exception_Dept e on d.Exception_Dept_UID = e.Exception_Dept_UID
                                where 1=1 ", searchModel.EndTime.ToString("yyyy-MM-dd"));
            #endregion
            #region Week SQL
            if (searchModel.ReportType == "Week")
                sql += string.Format(@"Declare @mindate date
                                        Declare @maxdate date  
                                        set @mindate  = DATEADD(day, -84, '{0}')  
                                        set @maxdate  = '{0}'  
                                        ;with temptab(date) as  
                                        ( select @mindate  
                                        union all  
                                        select dateadd(week,1,temptab.date) as date  
                                        from temptab  
                                        where dateadd(week,1,temptab.date)<=@maxdate )

                                    select f.WorkDate,CONVERT(varchar(10), f.week_num)DTMon,isnull(dt2.DTTime,0)DTTime,isnull(dt2.DTName,'')DTName from
                                    (select Exception_Dept_Name as DTName,week_num,sum(downtime) as DTTime from (
                                    SELECT [Exception_Record_UID]
                                            ,[Origin_UID]
	                                        ,c.LineName
                                            ,[SubOrigin_UID]
	                                        ,b.StationName
                                            ,a.[Exception_Code_UID]
	                                        ,d.Exception_Dept_UID
	                                        ,e.Exception_Dept_Name
	                                        ,e.Plant_Organization_UID
	                                        ,e.BG_Organization_UID
	                                        ,e.Funplant_Organization_UID
                                            ,[Project_UID]
                                            ,[WorkDate]
                                            ,[ShiftTimeID]
                                            ,[Status]
                                            ,[End_Date]
                                            ,[Note]
                                            ,a.[Created_UID]
                                            ,a.[Created_Date]
                                            ,a.[Modified_UID]
                                            ,a.[Modified_Date]
                                            ,[Contact_Person]
                                            ,[Contact_Phone]
	                                        ,case when End_Date is null then DATEDIFF(MINUTE, a.Created_Date,GETDATE()  )
	                                        else DATEDIFF(Hour, a.Created_Date,End_Date  ) end as downtime
	                                        ,DATEPART(WEEK,WorkDate) AS week_num
                                        FROM [dbo].[Exception_Record] a
                                    left join GL_Station b on a.SubOrigin_UID=b.StationID
                                    left join GL_Line c on a.Origin_UID = c.LineID
                                    left join Exception_Code d on a.Exception_Code_UID = d.Exception_Code_UID
                                    left join Exception_Dept e on d.Exception_Dept_UID = e.Exception_Dept_UID
                                    where 1=1   ", searchModel.EndTime.ToString("yyyy-MM-dd"));
            #endregion
            if (searchModel.ReportType == "Daily")
                sql += string.Format(@" and a.WorkDate >= DATEADD(day, -13, '{0}') and a.WorkDate <= '{0}' ", searchModel.EndTime.ToString("yyyy-MM-dd"));
            if (searchModel.ReportType == "Month")
                sql += string.Format(@" and a.WorkDate >= dateadd(dd,-datepart(dd, DATEADD(day, -365, '{0}'))+1,DATEADD(day, -365, '{0}')) and a.WorkDate <= dateadd(dd,-datepart(dd,'{0}') ,dateadd(mm,1,'{0}'))", searchModel.EndTime.ToString("yyyy-MM-dd"));
            if (searchModel.ReportType == "Week")
                sql += string.Format(" and a.WorkDate >=  dateadd(wk, datediff(wk,0,DATEADD(day, -84, '{0}') ), 0) and a.WorkDate<=dateadd(wk, datediff(wk,0,'{0}'), 6)", searchModel.EndTime.ToString("yyyy-MM-dd"));
            if (searchModel.Plant_Organization_UID != 0)
                sql += string.Format("and e.Plant_Organization_UID={0}", searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                sql += string.Format("and e.BG_Organization_UID={0}", searchModel.BG_Organization_UID);
            if (searchModel.LineID != 0)
                sql += string.Format("and Origin_UID={0}", searchModel.LineID);
            if (searchModel.StationID != 0)
                sql += string.Format("and SubOrigin_UID={0}", searchModel.StationID);
            if (searchModel.CustomerID != 0)
                sql += string.Format("and Project_UID={0}", searchModel.CustomerID);
            //全天
            if (searchModel.ShiftTimeID != -1)
            {
                sql += string.Format("and ShiftTimeID={0}", searchModel.ShiftTimeID);
            }
            if (searchModel.ReportType == "Daily")
                sql += @")dt group by Exception_Dept_Name,WorkDate)dt2
                        right join (select date as WorkDate,0 dt_one from temptab)f on f.WorkDate=dt2.WorkDate
                        order by f.WorkDate";
            if(searchModel.ReportType=="Month")
                sql += @")dt group by Exception_Dept_Name,mon)dt2
                            right join (select date as WorkDate,month(date) as mon,0 dt_one from temptab)f on f.mon=dt2.mon
                            order by f.WorkDate";
            if (searchModel.ReportType == "Week")
                sql += @")dt group by Exception_Dept_Name,week_num)dt2
                        right join (select date as WorkDate,DATEPART(WEEK,date) as week_num,0 dt_one from temptab)f on f.week_num=dt2.week_num
                        order by f.WorkDate";
            var dblist = DataContext.Database.SqlQuery<GLFourQDTModel>(sql).ToList();
            return dblist;
        }
    }
}
