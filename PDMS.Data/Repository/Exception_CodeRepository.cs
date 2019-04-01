using PDMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;
using AutoMapper;
using PDMS.Model;

namespace PDMS.Data.Repository
{
    public interface IException_CodeRepository : IRepository<Exception_Code>
    {
        int DeleExceptionCode(string IDs);
        ExceptionDTO AddExceptionCode(ExceptionDTO dto);
        IQueryable<ExceptionDTO> GetInfo(ExceptionDTO searchModel, Page page, out int totalcount);
        ExceptionDTO FetchExceptionCode(int uid);
        int UpdateExceptionCode(ExceptionDTO dto);
        List<ExceptionDTO> FetchExceptionCodeBasedDept(int deptUID);
    }

    class Exception_CodeRepository : RepositoryBase<Exception_Code>, IException_CodeRepository
    {
        public Exception_CodeRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
        /// <summary>
        /// 首页查询
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
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
                select distinct 
                        A.Exception_Code_UID, 
                        B.Exception_Dept_UID,
                        B.Exception_Dept_Name,A.Exception_Nub,A.Exception_Name,
                        isnull(F.Organization_Name,'Gold Line') as Plant,
                        isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
                        ISNULL(FunPlant.FunPlantName,'') as FunPlant,
                        U.User_Name as Created_User,
						A.Created_Date,
						V.User_Name as Modified_UserName,
						A.Modified_Date
                from Exception_Code A
				left join Exception_Dept B on B.Exception_Dept_UID=A.Exception_Dept_UID
                left join System_Organization F on F.Organization_UID=B.Plant_Organization_UID
                left join BG_Info on BG_Info.BG_Organization_UID=B.BG_Organization_UID
                left join FunPlant on FunPlant.BG_Organization_UID=B.BG_Organization_UID and                        FunPlant.FunPlant_Organization_UID=B.FunPlant_Organization_UID
                inner join System_Users U on U.Account_UID=A.Created_UID
                inner join System_Users V on V.Account_UID=A.Modified_UID
                where 1=1 ");
            if (!string.IsNullOrEmpty(searchModel.Exception_Dept_Name))
            {
                sql.Append($@" and B.Exception_Dept_Name='{searchModel.Exception_Dept_Name}'");
            }
            if (!string.IsNullOrEmpty(searchModel.Exception_Nub))
            {
                sql.Append($@" and A.Exception_Nub = '{searchModel.Exception_Nub}'");
            }
            if (!string.IsNullOrEmpty(searchModel.Exception_Name))
            {
                sql.Append($@" and A.Exception_Name = '{searchModel.Exception_Name}'");
            }
            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
            {
                sql.Append($@" and V.User_NTID = '{searchModel.Modified_UserNTID}'");
            }
            if (searchModel.Modified_Date_from != DateTime.MinValue)
            {
                sql.Append($@" and A.Modified_Date >= '{searchModel.Modified_Date_from}'");
            }
            if (searchModel.Modified_Date_to != DateTime.MinValue)
            {
                sql.Append($@" and A.Modified_Date <= '{searchModel.Modified_Date_to}'");
            }

            sql.Append($@" order by 
                        A.Modified_Date desc 
                        OFFSET {page.PageNumber * page.PageSize} ROWS
                        FETCH NEXT {page.PageSize} ROWS ONLY");
            #endregion
            var dbList = DataContext.Database.SqlQuery<ExceptionDTO>(sql.ToString()).ToList().AsQueryable();

            #region 计算总数
            StringBuilder sqlCount = new StringBuilder();
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
                select distinct count(A.Exception_Code_UID)
                from Exception_Code A
				left join Exception_Dept B on B.Exception_Dept_UID=A.Exception_Dept_UID
                left join System_Organization F on F.Organization_UID=B.Plant_Organization_UID
                left join BG_Info on BG_Info.BG_Organization_UID=B.BG_Organization_UID
                left join FunPlant on FunPlant.BG_Organization_UID=B.BG_Organization_UID and                        FunPlant.FunPlant_Organization_UID=B.FunPlant_Organization_UID
                inner join System_Users U on U.Account_UID=A.Created_UID
                inner join System_Users V on V.Account_UID=A.Modified_UID
                where 1=1 ");
            if (!string.IsNullOrEmpty(searchModel.Exception_Dept_Name))
            {
                sqlCount.Append($@" and B.Exception_Dept_Name='{searchModel.Exception_Dept_Name}'");
            }
            if (!string.IsNullOrEmpty(searchModel.Exception_Nub))
            {
                sqlCount.Append($@" and A.Exception_Nub = '{searchModel.Exception_Nub}'");
            }
            if (!string.IsNullOrEmpty(searchModel.Exception_Name))
            {
                sqlCount.Append($@" and A.Exception_Name = '{searchModel.Exception_Name}'");
            }
            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
            {
                sqlCount.Append($@" and V.User_NTID = '{searchModel.Modified_UserNTID}'");
            }
            if (searchModel.Modified_Date_from != DateTime.MinValue)
            {
                sqlCount.Append($@" and A.Modified_Date >= '{searchModel.Modified_Date_from}'");
            }
            if (searchModel.Modified_Date_to != DateTime.MinValue)
            {
                sqlCount.Append($@" and A.Modified_Date <= '{searchModel.Modified_Date_to}'");
            }

            #endregion
            var count = DataContext.Database.SqlQuery<int>(sqlCount.ToString()).ToArray();
            totalcount = count[0];
            return dbList;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ExceptionDTO AddExceptionCode(ExceptionDTO entity)
        {
            Exception_Code code = new Exception_Code();
            code.Exception_Nub = entity.Exception_Nub.Trim();
            code.Exception_Dept_UID = entity.Exception_Dept_UID;
            code.Exception_Name = entity.Exception_Name.Trim();
            code.Created_UID = entity.Created_UID;
            code.Created_Date = entity.Created_Date;
            code.Modified_UID = entity.Modified_UID;
            code.Modified_Date = entity.Modified_Date;
            var exist_entity = DataContext.Exception_Code.Where(x => x.Exception_Nub == code.Exception_Nub && x.Exception_Dept_UID == entity.Exception_Dept_UID).FirstOrDefault();
            if (exist_entity == null)
            {
                var obj = DataContext.Exception_Code.Add(code);
                var ret = DataContext.SaveChanges();
                string sql = $@";With BG_Info AS (
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
                select distinct A.*,B.Exception_Dept_Name,
                        isnull(F.Organization_Name,'Gold Line') as Plant,
                        isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
                        ISNULL(FunPlant.FunPlantName,'Gold Line') as FunPlant,
                        U.User_Name,
                        V.User_Name as ModifyUserName
                from Exception_Code A
				left join Exception_Dept B on B.Exception_Dept_UID=A.Exception_Dept_UID
                left join System_Organization F on F.Organization_UID=B.Plant_Organization_UID
                left join BG_Info on BG_Info.BG_Organization_UID=B.BG_Organization_UID
                left join FunPlant on FunPlant.BG_Organization_UID=B.BG_Organization_UID and                        FunPlant.FunPlant_Organization_UID=B.FunPlant_Organization_UID
                inner join System_Users U on U.Account_UID=A.Created_UID
                inner join System_Users V on V.Account_UID=A.Modified_UID
				where A.Exception_Code_UID={obj.Exception_Code_UID}";
                var retsult = DataContext.Database.SqlQuery<ExceptionDTO>(sql).FirstOrDefault();
                return retsult;
            }
            else
            {
                string sql = $@";With BG_Info AS (
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
                select distinct A.*,B.Exception_Dept_Name,
                        isnull(F.Organization_Name,'Gold Line') as Plant,
                        isnull(BG_Info.OP_TYPES,'Gold Line') as BG,
                        ISNULL(FunPlant.FunPlantName,'Gold Line') as FunPlant,
                        U.User_Name,
                        V.User_Name as ModifyUserName
                from Exception_Code A
				left join Exception_Dept B on B.Exception_Dept_UID=A.Exception_Dept_UID
                left join System_Organization F on F.Organization_UID=B.Plant_Organization_UID
                left join BG_Info on BG_Info.BG_Organization_UID=B.BG_Organization_UID
                left join FunPlant on FunPlant.BG_Organization_UID=B.BG_Organization_UID and                        FunPlant.FunPlant_Organization_UID=B.FunPlant_Organization_UID
                inner join System_Users U on U.Account_UID=A.Created_UID
                inner join System_Users V on V.Account_UID=A.Modified_UID
				where A.Exception_Code_UID={exist_entity.Exception_Code_UID}";
                var retsult = DataContext.Database.SqlQuery<ExceptionDTO>(sql).FirstOrDefault();
                retsult.Repeat = 1;
                return retsult;
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public int DeleExceptionCode(string IDs)
        {
            var idList = Array.ConvertAll(IDs.Split(','), s => Int32.Parse(s));
            foreach (var id in idList)
            {
                var item = DataContext.Exception_Code.Find(id);
                DataContext.Exception_Code.Remove(item);
            }
            //这里可能有问题，因为删除的时候记录表中有他的外键，是不允许删除的

            return DataContext.SaveChanges();
        }
        /// <summary>
        /// 根据uid获取
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ExceptionDTO FetchExceptionCode(int uid)
        {
            var entity = DataContext.Exception_Code.Find(uid);
            return Mapper.Map<ExceptionDTO>(entity);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int UpdateExceptionCode(ExceptionDTO dto)
        {
            var entity = DataContext.Exception_Code.Find(dto.Exception_Code_UID);
            if (entity == null)
                return 0;
            entity.Exception_Nub = dto.Exception_Nub;
            entity.Exception_Name = dto.Exception_Name;
            entity.Modified_UID = dto.Modified_UID;
            entity.Modified_Date = dto.Modified_Date;
            return DataContext.SaveChanges();
        }
        /// <summary>
        /// 根据部门ID获取所有编码
        /// </summary>
        /// <param name="deptUID"></param>
        /// <returns></returns>
        public List<ExceptionDTO> FetchExceptionCodeBasedDept(int deptUID)
        {
            var entities = (from A in DataContext.Exception_Code
                            from B in DataContext.Exception_Dept.Where(B => B.Exception_Dept_UID == A.Exception_Dept_UID && B.Plant_Organization_UID == 0 && B.BG_Organization_UID == 0 && B.Funplant_Organization_UID == 0).DefaultIfEmpty()
                            where (A.Exception_Dept_UID==deptUID)
                            select new ExceptionDTO
                            {
                                Exception_Code_UID = A.Exception_Code_UID,
                                Exception_Nub = A.Exception_Nub,
                                Exception_Name = A.Exception_Name
                            }).ToList();
            return entities;
            //return Mapper.Map<List<ExceptionDTO>>(entities);
        }
    }
}
