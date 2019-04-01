using AutoMapper;
using PDMS.Data.Infrastructure;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model;

namespace PDMS.Data.Repository
{

    public interface IException_DeptRepository : IRepository<Exception_Dept>
    {
        List<Depts> FetchExceptionDepts(int Plant_Organization_UID, int BG_Organization_UID);
        ExceptionDTO FetchExceptionDept(int uid);
        int DeleExceptionDept(int uid);
        int UpdateExceptionDept(ExceptionDTO dto);
        IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, Page page, out int totalcount);
        ExceptionDTO AddExceptionDept(ExceptionDTO dto);
    }

    public class Exception_DeptRepository : RepositoryBase<Exception_Dept>, IException_DeptRepository
    {
        public Exception_DeptRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ExceptionDTO AddExceptionDept(ExceptionDTO dto)
        {
            Exception_Dept exist_entity = DataContext.Exception_Dept.Where(x => x.Plant_Organization_UID == dto.Plant_Organization_UID && x.BG_Organization_UID == dto.BG_Organization_UID && x.Exception_Dept_Name == dto.Exception_Dept_Name).FirstOrDefault();
            if (exist_entity == null)
            {
                var obj = DataContext.Exception_Dept.Add(Mapper.Map<Exception_Dept>(dto));
                var ret = DataContext.SaveChanges();
                return Mapper.Map<ExceptionDTO>(obj);
            }
            else
            {
                 dto= Mapper.Map<ExceptionDTO>(exist_entity);
                dto.Repeat = 1;
                return dto;
            }
        }
        /// <summary>
        /// 删除异常部门
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int DeleExceptionDept(int uid)
        {
            var entitiesCode = DataContext.Exception_Code.Where(x => x.Exception_Dept_UID == uid).ToList();
            foreach (var entity in entitiesCode)
            {
                DataContext.Exception_Code.Remove(entity);
            }
            var entitiesEmail = DataContext.Exception_Email.Where(x => x.Project_UID == uid).ToList();
            foreach (var entity in entitiesEmail)
            {
                DataContext.Exception_Email.Remove(entity);
            }

            DataContext.SaveChanges();

            var item = DataContext.Exception_Dept.Find(uid);
            DataContext.Exception_Dept.Remove(item);
            return DataContext.SaveChanges();

        }

        /// <summary>
        /// 获取异常部门
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ExceptionDTO FetchExceptionDept(int uid)
        {
            var entity = DataContext.Exception_Dept.Find(uid);
            return Mapper.Map<ExceptionDTO>(entity);
        }
        /// <summary>
        /// 获取异常部门
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <returns></returns>
        public List<Depts> FetchExceptionDepts(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var entities = DataContext.Exception_Dept.Where(x => x.Plant_Organization_UID == Plant_Organization_UID && x.BG_Organization_UID == BG_Organization_UID).Select(x => new Depts()
            {
                Exception_Dept_UID = x.Exception_Dept_UID,
                Exception_Dept_Name = x.Exception_Dept_Name
            }).OrderBy(x => x.Exception_Dept_Name).ToList();
            return entities;
        }

        public IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, Page page, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            #region 分页查询
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
                select distinct 
                        A.Exception_Dept_UID,
                        A.Exception_Dept_Name,
                        isnull(F.Organization_Name,'Gold Line') as Plant,
                        isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
                        U.User_Name as Created_User,
						A.Created_Date,
						V.User_Name as Modified_UserName,
						A.Modified_Date
                from Exception_Dept A
                left join System_Organization F on F.Organization_UID=A.Plant_Organization_UID
                left join BG_Info on BG_Info.BG_Organization_UID=A.BG_Organization_UID
                inner join System_Users U on U.Account_UID=A.Created_UID
                inner join System_Users V on V.Account_UID=A.Modified_UID
                where 1=1 ");
            if (searchModel.Plant_Organization_UID!=0)
            {
                sbWhere.Append($@" and A.Plant_Organization_UID={searchModel.Plant_Organization_UID}");
            }
            if (searchModel.BG_Organization_UID!=0)
            {
                sbWhere.Append($@" and A.BG_Organization_UID={searchModel.BG_Organization_UID}");
            }

            if (!string.IsNullOrEmpty(searchModel.Exception_Dept_Name))
            {
                sbWhere.Append($@" and A.Exception_Dept_Name='{searchModel.Exception_Dept_Name.ToUpper()}'");
            }
            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
            {
                sbWhere.Append($@" and V.User_NTID = '{searchModel.Modified_UserNTID}'");
            }
            if (searchModel.Modified_Date_from != DateTime.MinValue)
            {
                sbWhere.Append($@" and A.Modified_Date >= '{searchModel.Modified_Date_from}'");
            }
            if (searchModel.Modified_Date_to != DateTime.MinValue)
            {
                sbWhere.Append($@" and A.Modified_Date <= '{searchModel.Modified_Date_to}'");
            }

            sql.Append(sbWhere).Append($@" order by 
                        A.Modified_Date desc 
                        OFFSET {page.PageNumber * page.PageSize} ROWS
                        FETCH NEXT {page.PageSize} ROWS ONLY");
            #endregion
            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList().AsQueryable();
            #region 计算总数

            sqlCount.Append(@";With BG_Info AS (
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
                select distinct 
                        count(1)
                from Exception_Dept A
                left join System_Organization F on F.Organization_UID=A.Plant_Organization_UID
                left join BG_Info on BG_Info.BG_Organization_UID=A.BG_Organization_UID
                inner join System_Users U on U.Account_UID=A.Created_UID
                inner join System_Users V on V.Account_UID=A.Modified_UID
                where 1=1 ").Append(sbWhere);


            #endregion
            var count = DataContext.Database.SqlQuery<int>(sqlCount.ToString()).ToArray();
            totalcount = count[0];
            return dbList;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int UpdateExceptionDept(ExceptionDTO dto)
        {
            var entity = DataContext.Exception_Dept.Find(dto.Exception_Dept_UID);
            if (entity == null)
                return 0;
            //查重部门名称
            var model = DataContext.Exception_Dept.Where(x => x.BG_Organization_UID == entity.BG_Organization_UID && x.Plant_Organization_UID == entity.Plant_Organization_UID && x.Exception_Dept_Name == dto.Exception_Dept_Name).FirstOrDefault();
            if (model != null)
            {
                return 2;
            }
            entity.Exception_Dept_Name = dto.Exception_Dept_Name.Trim();
            entity.Modified_UID = dto.Modified_UID;
            entity.Modified_Date = dto.Modified_Date;
            return DataContext.SaveChanges();
        }
    }
}
