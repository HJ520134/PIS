using AutoMapper;
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
    public interface IException_EmailRepository : IRepository<Exception_Email>
    {
        ExceptionDTO AddExceptionEmail(ExceptionDTO dto);
        IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, dynamic page, out int totalcount);
        ExceptionDTO FetchExceptionEmailInfo(int uid);
        int EditExceptionEmail(ExceptionDTO dto);
        int DeleExceptionEmail(int uid);
        List<string> FetchEmail(int uid);
        List<string> FetchEmailCC(int uid);
        ExceptionDTO FetchUserInfo(string nTID);
    }

    public class Exception_EmailRepository : RepositoryBase<Exception_Email>, IException_EmailRepository
    {
        public Exception_EmailRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
        public ExceptionDTO AddExceptionEmail(ExceptionDTO dto)
        {
            Exception_Email entity = new Exception_Email();
            entity.Project_UID = dto.Project_UID;
            entity.Exception_Times = dto.Exception_Times;
            entity.User_NT = dto.User_NT.Trim();
            entity.User_Name = dto.User_Name.Trim();
            entity.User_Email = dto.User_Email.Trim();
            entity.ReceiveType = dto.ReceiveType;
            entity.Created_UID = dto.Created_UID;
            entity.Created_Date = dto.Created_Date;
            entity.Modified_UID = dto.Modified_UID;
            entity.Modified_Date = dto.Modified_Date;
            var item = DataContext.Exception_Email.Where(x => x.Project_UID == dto.Project_UID && x.User_NT == entity.User_NT && x.Exception_Times == dto.Exception_Times && x.ReceiveType == dto.ReceiveType).FirstOrDefault();
            if (item != null)
            {
                dto = Mapper.Map<ExceptionDTO>(item);
                dto.Repeat = 1;
            }
            else
            {
                DataContext.Exception_Email.Add(entity);
                DataContext.SaveChanges();
            }
            return dto;

        }
        public IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, dynamic page, out int totalcount)
        {
            #region 分页查询
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            sql.Append(@";with CET_A as (
                            SELECT 
                                t1.Organization_UID as Plant_Organization_UID,
                                t1.Organization_Name as Plant,
                                t2.ChildOrg_UID as BG_Organization_UID 
                            FROM 
                                dbo.System_Organization t1 
                            INNER JOIN dbo.System_OrganizationBOM t2 ON t1.Organization_UID=t2.ParentOrg_UID 
	                        WHERE Organization_Name<>'Support team'
                            )		
	                    select 
						    A.Exception_Email_UID,
						    A.Exception_Times,
						    A.ReceiveType,
						    A.User_Email,
						    A.User_NT,
						    A.User_Name,
						    A.Created_Date,
						    A.Modified_Date,
						    P.Project_Name,
						    P.OP_TYPES as BG,
						    CET_A.Plant as Plant,	
						    C.User_Name as Created_User,
						    D.User_Name as Modified_UserName				
				        from 
			                Exception_Email A 
					            inner join Exception_Project B on B.Project_UID=A.Project_UID
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

            if (searchModel.Project_UID != 0)
            {
                sqlWhere.Append($@" and A.Project_UID = {searchModel.Project_UID}");
            }
            if (!string.IsNullOrEmpty(searchModel.User_Email))
            {
                sqlWhere.Append($@" and A.User_Email = N'{searchModel.User_Email}'");
            }
            if (!string.IsNullOrEmpty(searchModel.User_NT))
            {
                sqlWhere.Append($@" and A.User_NT = N'{searchModel.User_NT}'");
            }

            if (!string.IsNullOrEmpty(searchModel.User_Name))
            {
                sqlWhere.Append($@" and A.User_Name = N'{searchModel.User_Name}'");
            }
            if (searchModel.ReceiveType != 0)
            {
                sqlWhere.Append($@" and A.ReceiveType = {searchModel.ReceiveType}");
            }
            if (searchModel.Exception_Times != 0)
            {
                sqlWhere.Append($@" and A.Exception_Times = {searchModel.Exception_Times}");
            }
            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
            {
                sqlWhere.Append($@" and D.User_NTID = N'{searchModel.Modified_UserNTID}'");
            }
            if (searchModel.Modified_Date_from != DateTime.MinValue)
            {
                sql.Append($@" and A.Modified_Date >= '{searchModel.Modified_Date_from}'");
            }
            if (searchModel.Modified_Date_to != DateTime.MinValue)
            {
                sqlWhere.Append($@" and A.Modified_Date <= '{searchModel.Modified_Date_to}'");
            }
            sql.Append(sqlWhere).Append($@" order by 
                        A.Modified_Date desc 
                        OFFSET {page.PageNumber * page.PageSize} ROWS
                        FETCH NEXT {page.PageSize} ROWS ONLY");
            #endregion
            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList().AsQueryable();

            #region 计算总数
            StringBuilder sqlCount = new StringBuilder();
            sqlCount.Append(@";with CET_A as (
                            SELECT 
                                t1.Organization_UID as Plant_Organization_UID,
                                t1.Organization_Name as Plant,
                                t2.ChildOrg_UID as BG_Organization_UID 
                            FROM 
                                dbo.System_Organization t1 
                            INNER JOIN dbo.System_OrganizationBOM t2 ON t1.Organization_UID=t2.ParentOrg_UID 
	                        WHERE Organization_Name<>'Support team'
                            )		
	                    select 
                            count(1)			
				        from 
			                Exception_Email A 
					            inner join Exception_Project B on B.Project_UID=A.Project_UID
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
        public ExceptionDTO FetchExceptionEmailInfo(int uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($@"select 
	                        A.Exception_Email_UID,
	                        A.Exception_Times,
	                        A.ReceiveType,
	                        A.User_Email,
	                        A.User_NT,
	                        A.User_Name
	                    from 
                            Exception_Email A 
                        where 
                            A.Exception_Email_UID={uid}");
            var result = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).FirstOrDefault();
            return result;
        }
        public int EditExceptionEmail(ExceptionDTO dto)
        {
            //todo:判断修改后的NT账号是否已经存在于数据库，或者就不让修改NT账号
            var entity = DataContext.Exception_Email.Find(dto.Exception_Email_UID);
            var existEntity = DataContext.Exception_Email.Where(x => x.Project_UID == entity.Project_UID && x.Exception_Times == entity.Exception_Times && x.User_NT == dto.User_NT && x.ReceiveType == dto.ReceiveType).FirstOrDefault();
            if (existEntity != null)
                return 2;
            entity.Exception_Times = dto.Exception_Times;
            entity.ReceiveType = dto.ReceiveType;
            entity.User_NT = dto.User_NT.Trim();
            entity.User_Name = dto.User_Name;
            entity.User_Email = dto.User_Email;
            entity.Modified_UID = dto.Modified_UID;
            entity.Modified_Date = dto.Modified_Date;
            return DataContext.SaveChanges();
        }

        public int DeleExceptionEmail(int uid)
        {
            var entity = DataContext.Exception_Email.Find(uid);
            DataContext.Exception_Email.Remove(entity);
            return DataContext.SaveChanges();
        }

        public List<string> FetchEmail(int uid)
        {
            var entities = DataContext.Exception_Email.Where(x => x.Project_UID == uid && x.Exception_Times == 1 && x.ReceiveType == 1).Select(x => x.User_Email).ToList();
            return entities;
        }

        public List<string> FetchEmailCC(int uid)
        {
            var entities = DataContext.Exception_Email.Where(x => x.Project_UID == uid && x.Exception_Times == 1 && x.ReceiveType == 2).Select(x => x.User_Email).ToList();
            return entities;
        }

        public ExceptionDTO FetchUserInfo(string nTID)
        {
            var entity = DataContext.System_Users.Where(x => x.User_NTID == nTID).FirstOrDefault();
            if (entity == null)
            {
                return null;
            }
            else
            {
                ExceptionDTO dto = new ExceptionDTO();
                dto.User_Name = entity.User_Name;
                dto.User_Email = entity.Email;
                return dto;
            }
        }
    }
}
