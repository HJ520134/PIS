using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Repository
{
    public interface IException_TimeRepository : IRepository<Exception_Time>
    {
        int AddPeriodTime(ExceptionDTO dto);
        List<ExceptionDTO> FetchPeriodTimeBasedDeptID(int deptID);
        int DeletPeriodTime(string timeID);
        int UpdateDeptTime(int deptID, int dealyDayNub, int dayPeriod, int sendMaxTime);
        IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, dynamic page, out int totalcount);
        List<Projects> FetchExceptionProject(int uid);
        List<ExceptionDTO> FetchPeriodTimeBasedProjectID(int projectID);
        int UpdateProjectTime(ExceptionProjectDTO dto);
        ExceptionProjectDTO FetchExceptionProjectCycleTime(int uid);
    }
    public class Exception_TimeRepository : RepositoryBase<Exception_Time>, IException_TimeRepository
    {
        public Exception_TimeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public int AddPeriodTime(ExceptionDTO dto)
        {
            //先检查是否有相同的字段，如果有那么就直接返回
            var item = DataContext.Exception_Time.Where(x => x.Origin_UID == dto.Origin_UID && x.Exception_Time_At == dto.Exception_Time_At).FirstOrDefault();
            if (item != null)
            {
                return 0;
            }
            Exception_Time entity = new Exception_Time();
            entity.Origin_UID = dto.Origin_UID;
            entity.Exception_Time_At = dto.Exception_Time_At;
            entity.Created_UID = dto.Created_UID;
            entity.Created_Date = dto.Created_Date;
            entity.Modified_UID = dto.Modified_UID;
            entity.Modified_Date = dto.Modified_Date;
            DataContext.Exception_Time.Add(entity);
            var ret = DataContext.SaveChanges();
            return ret;

            //var item = DataContext.Exception_Time.Where(x => x.Origin_UID == dto.Origin_UID && x.Exception_Time_At == dto.Exception_Time_At).Select(x=>new PeriodTime() {
            //    Exception_Time_UID=x.Exception_Time_UID,
            //    Exception_Time_At=x.Exception_Time_At
            //}).FirstOrDefault();
            //if (item != null)
            //{
            //    PeriodTime result = new PeriodTime();
            //    result.Exception_Time_UID = item.Exception_Time_UID;
            //    result.Exception_Time_At = item.Exception_Time_At;
            //    result.Repeat = 1;
            //    return result;

            //}
            //Exception_Time entity = new Exception_Time();
            //entity.Origin_UID = dto.Exception_Dept_UID;
            //entity.Exception_Time_At = dto.Exception_Time_At;
            //entity.Created_UID = dto.Created_UID;
            //entity.Created_Date = dto.Created_Date;
            //entity.Modified_UID = dto.Modified_UID;
            //entity.Modified_Date = dto.Modified_Date;
            //DataContext.Exception_Time.Add(entity);
            //var ret= DataContext.SaveChanges();
            //if (ret > 0)
            //{
            //    PeriodTime result = new PeriodTime();
            //    result.Exception_Time_UID = entity.Exception_Time_UID;
            //    result.Exception_Time_At = entity.Exception_Time_At;

            //    return result;
            //}
            //return null;

        }

        public int DeletPeriodTime(string timeID)
        {
            var idList = Array.ConvertAll(timeID.Split(','), s => Int32.Parse(s));
            foreach (var id in idList)
            {
                var entity = DataContext.Exception_Time.Find(id);
                DataContext.Exception_Time.Remove(entity);
            }
            return DataContext.SaveChanges();

        }


        public IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, dynamic page, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            #region 分页查询
            sql.Append(@";with CET_A as (
                            SELECT 
                                t1.Organization_UID as Plant_Organization_UID,
                                t1.Organization_Name as Plant,
                                t2.ChildOrg_UID as BG_Organization_UID 
                            FROM 
                                dbo.System_Organization t1 
                        INNER JOIN dbo.System_OrganizationBOM t2 ON t1.Organization_UID=t2.ParentOrg_UID WHERE Organization_Name<>'Support team'
                        )		
				select 
				        B.Exception_Time_UID as Exception_Time_UID,	
						B.Exception_Time_At as Exception_Time_At,
						A.Project_UID,
						A.DelayDayNub,
						A.SendMaxTime,
						A.DayPeriod,
						P.Project_Name,
						P.OP_TYPES as BG,
						P.Organization_UID,
						B.Created_Date,
						B.Modified_Date,
						C.User_Name as Created_User,
						D.User_Name as Modified_UserName,
						CET_A.Plant
				from 
			        Exception_Project A 
					inner join Exception_Time B on B.Origin_UID=A.Project_UID
				    inner join System_Project P on P.Project_UID=A.Project_UID and P.MESProject_Name<>'' and P.MESProject_Name is not null
					left join System_Users C on C.Account_UID=B.Created_UID
			        left join System_Users D on D.Account_UID=B.Modified_UID
					inner join CET_A on CET_A.BG_Organization_UID=P.Organization_UID
                    where 1=1 ");
            if (searchModel.Plant_Organization_UID != 0)
            {
                sqlWhere.Append($@" and CET_A.Plant_Organization_UID = {searchModel.Plant_Organization_UID}");
            }
            if (searchModel.BG_Organization_UID != 0)
            {
                sqlWhere.Append($@" and P.Organization_UID = {searchModel.BG_Organization_UID}");
            }

            if (searchModel.Origin_UID != 0)
            {
                sqlWhere.Append($@" and B.Origin_UID = {searchModel.Origin_UID}");
            }

            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
            {
                sqlWhere.Append($@" and D.User_NTID = N'{searchModel.Modified_UserNTID}'");
            }
            if (searchModel.Modified_Date_from != DateTime.MinValue)
            {
                sqlWhere.Append($@" and B.Modified_Date >= '{searchModel.Modified_Date_from}'");
            }
            if (searchModel.Modified_Date_to != DateTime.MinValue)
            {
                sqlWhere.Append($@" and B.Modified_Date <= '{searchModel.Modified_Date_to}'");
            }
            sql.Append(sqlWhere).Append($@" order by 
                        CET_A.Plant,P.OP_TYPES,P.Project_Name desc,
						B.Exception_Time_At
                        OFFSET {page.PageNumber * page.PageSize} ROWS
                        FETCH NEXT {page.PageSize} ROWS ONLY");
            #endregion
            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList().AsQueryable();

            #region 计算总数
            sqlCount.Append(@";with CET_A as (
                            SELECT 
                                t1.Organization_UID as Plant_Organization_UID,
                                t1.Organization_Name as Plant,
                                t2.ChildOrg_UID as BG_Organization_UID 
                            FROM 
                                dbo.System_Organization t1 
                        INNER JOIN dbo.System_OrganizationBOM t2 ON t1.Organization_UID=t2.ParentOrg_UID WHERE Organization_Name<>'Support team'
                        )		
				select 
				        count(1)
				from 
			        Exception_Project A 
					inner join Exception_Time B on B.Origin_UID=A.Project_UID
				    inner join System_Project P on P.Project_UID=A.Project_UID and P.MESProject_Name<>'' and P.MESProject_Name is not null
					left join System_Users C on C.Account_UID=B.Created_UID
			        left join System_Users D on D.Account_UID=B.Modified_UID
					inner join CET_A on CET_A.BG_Organization_UID=P.Organization_UID
                    where 1=1 ").Append(sqlWhere);


            #endregion
            var count = DataContext.Database.SqlQuery<int>(sqlCount.ToString()).ToArray();
            totalcount = count[0];
            return dbList;
        }

        public int UpdateDeptTime(int deptID, int dealyDayNub, int dayPeriod, int sendMaxTime)
        {
            var entity = DataContext.Exception_Dept.Find(deptID);
            entity.DealyDayNub = dealyDayNub;
            entity.DayPeriod = dayPeriod;
            entity.SendMaxTime = sendMaxTime;

           return DataContext.SaveChanges();
        }


        public List<Projects> FetchExceptionProject(int uid)
        {
            var entities = DataContext.System_Project.Where(x => x.Organization_UID == uid&&x.MESProject_Name!=null&&x.MESProject_Name!="").Select(x => new Projects
            {
                Project_UID = x.Project_UID,
                Project_Name = x.Project_Name
            }).ToList();
            return entities;
        }
        public List<ExceptionDTO> FetchPeriodTimeBasedDeptID(int deptID)
        {
            var entities = DataContext.Exception_Time.Where(x => x.Origin_UID == deptID).OrderBy(x => x.Exception_Time_At).Select(x => new ExceptionDTO()
            {
                Exception_Time_UID = x.Exception_Time_UID,
                Exception_Time_At = x.Exception_Time_At
            }).ToList();
            return entities;
        }
        public List<ExceptionDTO> FetchPeriodTimeBasedProjectID(int projectID)
        {
            var entities = DataContext.Exception_Time.Where(x => x.Origin_UID == projectID).OrderBy(x => x.Exception_Time_At).Select(x => new ExceptionDTO()
            {
                Exception_Time_UID = x.Exception_Time_UID,
                Exception_Time_At = x.Exception_Time_At
            }).ToList();
            return entities;
        }

        public int UpdateProjectTime(ExceptionProjectDTO dto)
        {
            var entity = DataContext.Exception_Project.Where(x=>x.Project_UID== dto.Project_UID).FirstOrDefault();
            if (entity == null)
            {
                DataContext.Exception_Project.Add(new Exception_Project()
                {
                    Project_UID = dto.Project_UID,
                    DelayDayNub = dto.DelayDayNub,
                    DayPeriod = dto.DayPeriod,
                    SendMaxTime = dto.DayPeriod,
                    Created_UID = dto.Created_UID,
                    Created_Date = dto.Created_Date,
                    Modified_UID= dto.Created_UID,
                    Modified_Date= dto.Created_Date
                });
            }
            else
            {
                entity.DelayDayNub = dto.DelayDayNub;
                entity.DayPeriod = dto.DayPeriod;
                entity.SendMaxTime = dto.SendMaxTime;
                entity.Modified_UID = dto.Modified_UID;
                entity.Modified_Date = dto.Modified_Date;
            }


            return DataContext.SaveChanges();
        }

        public ExceptionProjectDTO FetchExceptionProjectCycleTime(int uid)
        {
            var entity = DataContext.Exception_Project.Where(x => x.Project_UID == uid).FirstOrDefault();
            ExceptionProjectDTO dto = new ExceptionProjectDTO();
            if (entity != null)
            {
                dto.DelayDayNub = entity.DelayDayNub;
                dto.DayPeriod = entity.DayPeriod;
                dto.SendMaxTime = entity.SendMaxTime;
            }
            return dto;
        }
    }

}
