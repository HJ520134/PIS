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
    public interface IFixture_Return_MRepository : IRepository<Fixture_Return_D>
    {
        /// <summary>
        /// 获取领用单号
        /// </summary>
        /// <param name="plant_ID"></param>
        /// <param name="op_type"></param>
        /// <param name="funPlant"></param>
        /// <returns></returns>
        List<string> FetchFixtureTotakeforFixtureReturn(int plant_ID, int op_type, int funPlant);
        /// <summary>
        /// 获取某单号下所有的治具编号
        /// </summary>
        /// <param name="fixtureTakeNo"></param>
        /// <returns></returns>
        List<Fixture_Taken_InfoDTO> FetchAllFixtureUIDBasedFixtureTakeNo(string fixtureTakeNo);
        /// <summary>
        /// 根据fixtureTake_UID返回治具Fixture_Taken_InfoDTO集合
        /// </summary>
        /// <param name="fixtureTake_UID"></param>
        /// <returns></returns>
        List<Fixture_Taken_InfoDTO> FetchFixtureTakenInfo(int fixtureTake_UID);
        string AddFixtureRetrun(Fixture_Return_MDTO dto);
        string AddFixtureRetrunD(Fixture_Return_DDTO dto);
        IQueryable<Fixture_Return_Index> GetInfo(Fixture_Return_MDTO searchModel, Page page, out int totalcount);
        Fixture_Return_MDTO QueryFixtureReturnUid(int uid);
        int FixtureReturnUpdatePost(Fixture_Return_MDTO dto);
        List<Fixture_Return_MDTO> FixtureReturnDetail(int uid);
        string DelFixtureReturnM(int uid);
        string GetCurrentReturnNub();
        List<Fixture_Taken_InfoDTO> FetchAllFixturesBasedReturnMUID(int uid);
        string UpdateFixtureRetrun(Fixture_Return_MDTO dto);
        string UpdateFixtureRetrunD(Fixture_Return_DDTO dto);
        List<Fixture_Return_Index> ExportFixtrueReturn2Excel(Fixture_Return_MDTO dto);
    }

    public class Fixture_Return_MRepository : RepositoryBase<Fixture_Return_D>, IFixture_Return_MRepository
    {
        public Fixture_Return_MRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {

        }
        /// <summary>
        /// 获取领用单号
        /// </summary>
        /// <param name="plant_ID"></param>
        /// <param name="op_type"></param>
        /// <param name="funPlant"></param>
        /// <returns></returns>
        public List<string> FetchFixtureTotakeforFixtureReturn(int plant_ID, int op_type, int funPlant)
        {
            string sql = $@" SELECT distinct 
                                    dbo.Fixture_Totake_M.Totake_NO 
                             FROM  
                                    dbo.Fixture_Totake_M WHERE EXISTS (
                                        SELECT 
                                            dbo.Fixture_Totake_D.Fixture_M_UID 
                                        FROM 
                                            dbo.Fixture_Totake_D 
                                        WHERE 
                                            dbo.Fixture_Totake_D.Fixture_Totake_M_UID = dbo.Fixture_Totake_M.Fixture_Totake_M_UID 
                                            and (dbo.Fixture_Totake_D.IS_Return=0 or dbo.Fixture_Totake_D.IS_Return is null)
                                        )
                                    and Plant_Organization_UID={plant_ID} 
                                    and BG_Organization_UID={op_type} 
                                    and FunPlant_Organization_UID={funPlant}";
            var dbList = DataContext.Database.SqlQuery<string>(sql).ToList();

            return dbList;
        }
        /// <summary>
        /// 获取所有治具的Fixture_M_UID唯一流水号
        /// </summary>
        /// <param name="fixtureTakeNo"></param>
        /// <returns></returns>
        public List<Fixture_Taken_InfoDTO> FetchAllFixtureUIDBasedFixtureTakeNo(string fixtureTakeNo)
        {
            //string sql = $@"select distinct B.Fixture_M_UID from Fixture_Totake_M A left join Fixture_Totake_D B on A.Fixture_Totake_M_UID=B.Fixture_Totake_M_UID left join Fixture_M C on B.Fixture_M_UID=C.Fixture_M_UID where A.Totake_NO='{fixtureTakeNo}'";
            //var dbList = DataContext.Database.SqlQuery<int>(sql).ToList();
            if (string.IsNullOrEmpty(fixtureTakeNo))
            {
                return null;
            }
            string sql = $@"SELECT 
                                    t1.Fixture_Totake_M_UID,
                                    t2.Fixture_Totake_D_UID,
                                    t3.Fixture_M_UID,
                                    t3.ShortCode,
                                    t3.Fixture_Unique_ID,
                                    isnull(t5.Process_ID,'')+'_'+isnull(t5.Process_Name,'') Process,
                                    isnull(t6.WorkStation_ID,'')+'_'+isnull(t6.WorkStation_Name,'') WorkStation,
                                    isnull(t4.Line_ID,'')+'_'+isnull(t4.Line_Name,'') Line,isnull(t2.IS_Return,0) as IS_Return,
                                    t2.Fixture_Totake_D_UID 
                            FROM 
                                    dbo.Fixture_Totake_M t1
                            INNER JOIN dbo.Fixture_Totake_D t2 ON t1.Fixture_Totake_M_UID=t2.Fixture_Totake_M_UID
                            INNER JOIN dbo.Fixture_M t3 ON t2.Fixture_M_UID=t3.Fixture_M_UID
                            LEFT JOIN dbo.Production_Line t4 ON t3.Production_Line_UID=t4.Production_Line_UID
                            LEFT JOIN dbo.Process_Info t5 ON t4.Process_Info_UID=t5.Process_Info_UID
                            LEFT JOIN dbo.WorkStation t6 ON t4.Workstation_UID=t6.WorkStation_UID
                            WHERE 
                                t1.Totake_NO='{fixtureTakeNo}'";
            var dbList = DataContext.Database.SqlQuery<Fixture_Taken_InfoDTO>(sql).ToList();
            return dbList;
        }

        public List<Fixture_Taken_InfoDTO> FetchFixtureTakenInfo(int fixtureTake_UID)
        {
            string sql = $@"SELECT 
                                t1.Fixture_Totake_M_UID,
                                t3.Fixture_M_UID,
                                t3.ShortCode,
                                t3.Fixture_Unique_ID,
                                isnull(t5.Process_ID,'')+'_'+isnull(t5.Process_Name,'') Process,
                                isnull(t6.WorkStation_ID,'')+'_'+isnull(t6.WorkStation_Name,'') WorkStation,
                                isnull(t4.Line_ID,'')+'_'+isnull(t4.Line_Name,'') Line,
                                isnull(t2.IS_Return,0) as IS_Return,
                                t2.Fixture_Totake_D_UID 
                            FROM 
                                dbo.Fixture_Totake_M t1
                            INNER JOIN dbo.Fixture_Totake_D t2 ON t1.Fixture_Totake_M_UID=t2.Fixture_Totake_M_UID
                            INNER JOIN dbo.Fixture_M t3 ON t2.Fixture_M_UID=t3.Fixture_M_UID
                            LEFT JOIN dbo.Production_Line t4 ON t3.Production_Line_UID=t4.Production_Line_UID
                            LEFT JOIN dbo.Process_Info t5 ON t4.Process_Info_UID=t5.Process_Info_UID
                            LEFT JOIN dbo.WorkStation t6 ON t4.Workstation_UID=t6.WorkStation_UID
                            WHERE 
                                t1.Fixture_Totake_M_UID={fixtureTake_UID}";
            var dbList = DataContext.Database.SqlQuery<Fixture_Taken_InfoDTO>(sql).ToList();
            return dbList;
        }

        public string AddFixtureRetrun(Fixture_Return_MDTO dto)
        {
            Fixture_Return_M fixM = new Fixture_Return_M();
            fixM.Fixture_Totake_M_UID = dto.Fixture_Totake_M_UID;
            fixM.Return_NO = dto.Return_NO;
            fixM.Return_User = dto.Return_User;
            fixM.Return_Name = dto.Return_Name;
            fixM.Return_Date = dto.Return_Date;
            fixM.Created_UID = dto.Created_UID;
            fixM.Plant_Organization_UID = dto.Plant_Organization_UID;
            fixM.BG_Organization_UID = dto.BG_Organization_UID;
            fixM.FunPlant_Organization_UID = dto.FunPlant_Organization_UID;
            fixM.Modified_UID = dto.Created_UID;
            fixM.Modified_Date = DateTime.Now;
            fixM.Created_Date = DateTime.Now;
            DataContext.Fixture_Return_M.Add(fixM);
            var entities = DataContext.Fixture_Totake_D.Where(x => x.Fixture_Totake_D_UID == dto.Fixture_Totake_D_UID).ToList();
            foreach (var entity in entities)
            {
                entity.IS_Return = 1;
            }

            DataContext.SaveChanges();
            var ret = fixM.Fixture_Return_M_UID.ToString();
            return ret;

        }

        public string AddFixtureRetrunD(Fixture_Return_DDTO dto)
        {
            Fixture_Return_D fixD = new Fixture_Return_D();
            fixD.Fixture_Return_M_UID = dto.Fixture_Return_M_UID;
            fixD.Fixture_M_UID = dto.Fixture_M_UID;
            fixD.Created_Date = System.DateTime.Now;
            fixD.Created_UID = dto.Created_UID;
            fixD.Modified_UID = dto.Created_UID;
            fixD.Modified_Date = System.DateTime.Now;
            fixD.IS_Return = dto.IS_Return;
            DataContext.Fixture_Return_D.Add(fixD);
            var entity = DataContext.Fixture_Totake_D.Find(dto.Fixture_Totake_D_UID);
            entity.IS_Return = 1;
            Fixture_Resume fr = new Fixture_Resume();
            fr.Fixture_M_UID= dto.Fixture_M_UID;
            fr.Data_Source = "2";
            fr.Resume_Date= System.DateTime.Now;
            fr.Resume_Notes = "Fixture_Return";
            fr.Source_UID= dto.Fixture_Return_M_UID;
            fr.Source_NO = dto.Return_NO;
            fr.Modified_UID= dto.Created_UID;
            fr.Modified_Date = System.DateTime.Now;
            DataContext.Fixture_Resume.Add(fr);

            var ret = DataContext.SaveChanges();
            return ret.ToString();
        }

        public IQueryable<Fixture_Return_Index> GetInfo(Fixture_Return_MDTO searchModel, Page page, out int totalcount)
        {
            #region 分页查询
            StringBuilder sql = new StringBuilder();
            sql.Append(@";With BG_Info AS (
                                select 
                                    Organization_UID as BG_Organization_UID,
                                    Organization_Name AS OP_TYPES 
                                from 
                                    System_Organization 
                                where 
                                    Organization_ID LIKE'20%'
                                ), 
                        FunPlant AS( 
                                SELECT DISTINCT 
                                    t2.Organization_Name as FunPlantName,
                                    one.ParentOrg_UID as BG_Organization_UID,
                                    t2.Organization_UID as FunPlant_Organization_UID 
                                FROM (
                                        SELECT * FROM dbo.System_OrganizationBOM
                                     ) one 
                                INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                                INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                                INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                                WHERE 
                                    t3.Organization_Name='OP'
                                    )
                        select distinct 
                            A.Fixture_Return_M_UID,
                            B.Fixture_Totake_M_UID,
                            A.Return_NO,B.Totake_NO as Taken_NO,
                            F.Organization_Name as Plant,
                            BG_Info.OP_TYPES as BG,
                            FunPlant.FunPlantName as FunPlant,
                            isnull(A.Return_User,'') as Return_User,
                            isnull(A.Return_Name,'') as Return_Name,
                            B.Totake_Date,
                            A.Modified_UID,
                            A.Modified_Date,
                            G.Machine_ID,
                            U.User_Name as Modified_UserName,
                            D.Fixture_Return_D_UID
                        from Fixture_Return_M A 
                        inner join Fixture_Totake_M B on A.Fixture_Totake_M_UID=B.Fixture_Totake_M_UID 
                        inner join Fixture_Totake_D C on B.Fixture_Totake_M_UID=C.Fixture_Totake_M_UID 
                        left join Fixture_Return_D D on D.Fixture_Return_M_UID=A.Fixture_Return_M_UID
                        inner join Fixture_M E on E.Fixture_M_UID=C.Fixture_M_UID 
                        inner join System_Organization F on F.Organization_UID=A.Plant_Organization_UID
                        left join Fixture_Machine G on G.Fixture_Machine_UID=E.Fixture_Machine_UID
                        left join Production_Line P on G.Production_Line_UID=P.Production_Line_UID
                        left join Process_Info Q on Q.Process_Info_UID=P.Process_Info_UID
                        left join WorkStation R on R.WorkStation_UID=P.Workstation_UID
                        left join Workshop S on S.Workshop_UID=P.Workshop_UID
                        inner join BG_Info on BG_Info.BG_Organization_UID=A.BG_Organization_UID
                        left join FunPlant on FunPlant.BG_Organization_UID=A.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=A.FunPlant_Organization_UID 
                        left join System_Users U on U.Account_UID=A.Modified_UID where 1=1");
            if (!(searchModel.Plant_Organization_UID == 0))
                sql.Append($" and A.Plant_Organization_UID = {searchModel.Plant_Organization_UID}");
            if (!(searchModel.BG_Organization_UID == 0))
                sql.Append($" and A.BG_Organization_UID = {searchModel.BG_Organization_UID}");
            if (!(searchModel.FunPlant_Organization_UID == 0)&&!(searchModel.FunPlant_Organization_UID == null))
                sql.Append($" and A.FunPlant_Organization_UID = {searchModel.FunPlant_Organization_UID}");
            if (!string.IsNullOrEmpty(searchModel.Return_NO))
                sql.Append($" and A.Return_NO = '{searchModel.Return_NO}'");
            if (!string.IsNullOrEmpty(searchModel.Return_User))
                sql.Append($" and A.Return_User = '{searchModel.Return_User}'");
            if (!(searchModel.Modified_Date_from == DateTime.MinValue))
                sql.Append($" and A.Modified_Date > '{searchModel.Modified_Date_from}'");
            if (!(searchModel.Modified_Date_to == DateTime.MinValue))
                sql.Append($" and A.Modified_Date < '{searchModel.Modified_Date_to}'");
            //and A.Plant_Organization_UID = 1
            //and A.BG_Organization_UID = 3
            //and A.FunPlant_Organization_UID = 29
            //and A.Return_NO = ''
            //and A.Return_User = ''
            //and A.Modified_Date > ''
            if (!string.IsNullOrEmpty(searchModel.Taken_NO))
                sql.Append($" and B.Totake_NO = '{searchModel.Taken_NO}'");
            //and B.Totake_NO = ''


            if (!string.IsNullOrEmpty(searchModel.Line_NO))
                sql.Append($" and P.Line_Name = N'{searchModel.Line_NO}'");
            //and P.Line_Name = ''


            if (!string.IsNullOrEmpty(searchModel.Fixture_NO))
                sql.Append($" and E.Fixture_NO = '{searchModel.Fixture_NO}'");
            if (!string.IsNullOrEmpty(searchModel.Fixture_Unique_ID))
                sql.Append($" and E.Fixture_Unique_ID = '{searchModel.Fixture_Unique_ID}'");
            if (!string.IsNullOrEmpty(searchModel.ShortCode))
                sql.Append($" and E.ShortCode = '{searchModel.ShortCode}'");

            //and E.Fixture_NO = ''
            //and E.Fixture_Unique_ID = ''
            //and E.ShortCode = ''

            if (!string.IsNullOrEmpty(searchModel.Machine_ID))
                sql.Append($" and G.Machine_ID = N'{searchModel.Machine_ID}'");
            //and G.Machine_ID = ''

            if (!string.IsNullOrEmpty(searchModel.Process_Name))
                sql.Append($" and Q.Process_Name = N'{searchModel.Process_Name}'");
            //and Q.Process_Name = ''


            if (!string.IsNullOrEmpty(searchModel.WorkStation_Name))
                sql.Append($" and R.WorkStation_Name = N'{searchModel.WorkStation_Name}'");
            //and R.WorkStation_Name = ''

            if (!string.IsNullOrEmpty(searchModel.Workshop_Name))
                sql.Append($" and S.Workshop_Name = N'{searchModel.Workshop_Name}'");
            //and S.Workshop_Name = ''

            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
                sql.Append($" and U.User_NTID = '{searchModel.Modified_UserNTID}'");
            //and U.User_NTID = ''

            sql.Append($@" order by 
                        A.Fixture_Return_M_UID desc 
                        OFFSET {page.PageNumber * page.PageSize} ROWS
                        FETCH NEXT {page.PageSize} ROWS ONLY");
            #endregion
            var dbList = DataContext.Database.SqlQuery<Fixture_Return_Index>(sql.ToString()).ToList().AsQueryable();

            #region 计算总数
            StringBuilder sqlCount = new StringBuilder();
            sqlCount.Append(@";With BG_Info AS (
                                select 
                                    Organization_UID as BG_Organization_UID,
                                    Organization_Name AS OP_TYPES 
                                from 
                                    System_Organization 
                                where 
                                    Organization_ID LIKE'20%'
                                ), 
                        FunPlant AS( 
                                SELECT DISTINCT 
                                    t2.Organization_Name as FunPlantName,
                                    one.ParentOrg_UID as BG_Organization_UID,
                                    t2.Organization_UID as FunPlant_Organization_UID 
                                FROM (
                                        SELECT * FROM dbo.System_OrganizationBOM
                                     ) one 
                                INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                                INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                                INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                                WHERE 
                                    t3.Organization_Name='OP'
                                    )
                        select 
                            count(distinct D.Fixture_Return_D_UID)
                        from 
                            Fixture_Return_M A 
                        inner join Fixture_Totake_M B on A.Fixture_Totake_M_UID=B.Fixture_Totake_M_UID 
                        inner join Fixture_Totake_D C on B.Fixture_Totake_M_UID=C.Fixture_Totake_M_UID 
                        left join Fixture_Return_D D on D.Fixture_Return_M_UID=A.Fixture_Return_M_UID
                        inner join Fixture_M E on E.Fixture_M_UID=C.Fixture_M_UID 
                        inner join System_Organization F on F.Organization_UID=A.Plant_Organization_UID
                        left join Fixture_Machine G on G.Fixture_Machine_UID=E.Fixture_Machine_UID
                        left join Production_Line P on G.Production_Line_UID=P.Production_Line_UID
                        left join Process_Info Q on Q.Process_Info_UID=P.Process_Info_UID
                        left join WorkStation R on R.WorkStation_UID=P.Workstation_UID
                        left join Workshop S on S.Workshop_UID=P.Workshop_UID
                        inner join BG_Info on BG_Info.BG_Organization_UID=A.BG_Organization_UID
                        left join FunPlant on FunPlant.BG_Organization_UID=A.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=A.FunPlant_Organization_UID 
                        left join System_Users U on U.Account_UID=A.Modified_UID where 1=1");
            if (!(searchModel.Plant_Organization_UID == 0))
                sqlCount.Append($" and A.Plant_Organization_UID = {searchModel.Plant_Organization_UID}");
            if (!(searchModel.BG_Organization_UID == 0))
                sqlCount.Append($" and A.BG_Organization_UID = {searchModel.BG_Organization_UID}");
            if (!(searchModel.FunPlant_Organization_UID == 0)&&!(searchModel.FunPlant_Organization_UID==null))
                sqlCount.Append($" and A.FunPlant_Organization_UID = {searchModel.FunPlant_Organization_UID}");
            if (!string.IsNullOrEmpty(searchModel.Return_NO))
                sqlCount.Append($" and A.Return_NO = '{searchModel.Return_NO}'");
            if (!string.IsNullOrEmpty(searchModel.Return_User))
                sqlCount.Append($" and A.Return_User = '{searchModel.Return_User}'");
            if (!(searchModel.Modified_Date_from == DateTime.MinValue))
                sqlCount.Append($" and A.Modified_Date > '{searchModel.Modified_Date_from}'");
            if (!(searchModel.Modified_Date_to== DateTime.MinValue))
                sqlCount.Append($" and A.Modified_Date < '{searchModel.Modified_Date_to}'");
            //and A.Plant_Organization_UID = 1
            //and A.BG_Organization_UID = 3
            //and A.FunPlant_Organization_UID = 29
            //and A.Return_NO = ''
            //and A.Return_User = ''
            //and A.Modified_Date > ''
            if (!string.IsNullOrEmpty(searchModel.Taken_NO))
                sqlCount.Append($" and B.Totake_NO = '{searchModel.Taken_NO}'");
            //and B.Totake_NO = ''


            if (!string.IsNullOrEmpty(searchModel.Line_NO))
                sqlCount.Append($" and P.Line_Name = N'{searchModel.Line_NO}'");
            //and P.Line_Name = ''


            if (!string.IsNullOrEmpty(searchModel.Fixture_NO))
                sqlCount.Append($" and E.Fixture_NO = '{searchModel.Fixture_NO}'");
            if (!string.IsNullOrEmpty(searchModel.Fixture_Unique_ID))
                sqlCount.Append($" and E.Fixture_Unique_ID = '{searchModel.Fixture_Unique_ID}'");
            if (!string.IsNullOrEmpty(searchModel.ShortCode))
                sqlCount.Append($" and E.ShortCode = '{searchModel.ShortCode}'");

            //and E.Fixture_NO = ''
            //and E.Fixture_Unique_ID = ''
            //and E.ShortCode = ''

            if (!string.IsNullOrEmpty(searchModel.Machine_ID))
                sqlCount.Append($" and G.Machine_ID = N'{searchModel.Machine_ID}'");
            //and G.Machine_ID = ''

            if (!string.IsNullOrEmpty(searchModel.Process_Name))
                sqlCount.Append($" and Q.Process_Name = N'{searchModel.Process_Name}'");
            //and Q.Process_Name = ''


            if (!string.IsNullOrEmpty(searchModel.WorkStation_Name))
                sqlCount.Append($" and R.WorkStation_Name = N'{searchModel.WorkStation_Name}'");
            //and R.WorkStation_Name = ''

            if (!string.IsNullOrEmpty(searchModel.Workshop_Name))
                sqlCount.Append($" and S.Workshop_Name = N'{searchModel.Workshop_Name}'");
            //and S.Workshop_Name = ''

            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
                sqlCount.Append($" and U.User_NTID = '{searchModel.Modified_UserNTID}'");

            #endregion
            var count = DataContext.Database.SqlQuery<int>(sqlCount.ToString()).ToArray();
            totalcount = count[0];
            return dbList;
        }

        public Fixture_Return_MDTO QueryFixtureReturnUid(int uid)
        {
            string sql = $@"";
            var dbList = DataContext.Database.SqlQuery<Fixture_Return_MDTO>(sql).FirstOrDefault();
            return dbList;
        }

        public int FixtureReturnUpdatePost(Fixture_Return_MDTO dto)
        {
            var list = DataContext.Fixture_Return_M.Where(x => x.Fixture_Return_M_UID == dto.Fixture_Return_M_UID).ToList();
            foreach (var item in list)
            {
                item.Return_User = dto.Return_User;
                item.Return_User = dto.Return_Name;
            }
            return DataContext.SaveChanges();
        }

        public List<Fixture_Return_MDTO> FixtureReturnDetail(int uid)
        {
            string sql = $@";With BG_Info AS (
                            select 
                                Organization_UID as BG_Organization_UID,
                                Organization_Name AS OP_TYPES 
                            from 
                                System_Organization 
                            where Organization_ID LIKE'20%'
                            ), 
                        FunPlant AS( 
                            SELECT DISTINCT 
                                t2.Organization_Name as FunPlantName,
                                one.ParentOrg_UID as BG_Organization_UID,
                                t2.Organization_UID as FunPlant_Organization_UID 
                            FROM (
                                SELECT * FROM dbo.System_OrganizationBOM
                            ) one 
                            INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                            INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                            INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                            WHERE 
                                t3.Organization_Name='OP')
                select distinct 
                        F.Organization_Name as Plant,
                        BG_Info.OP_TYPES as BG,
                        FunPlant.FunPlantName as FunPlant,
                        Q.Process_Name,
                        R.WorkStation_Name,
                        S.Workshop_Name,
                        P.Line_Name as Line_NO,
                        G.Machine_ID,
                        E.Fixture_Unique_ID,
                        E.ShortCode,
                        U.User_Name,
                        A.Created_Date,
                        C.IS_Return,
                        V.User_Name as ModifyUserName,
                        A.Modified_Date
                from 
                    Fixture_Return_M A 
                inner join Fixture_Totake_M B on A.Fixture_Totake_M_UID=B.Fixture_Totake_M_UID 
                inner join Fixture_Totake_D C on B.Fixture_Totake_M_UID=C.Fixture_Totake_M_UID 
                left join Fixture_Return_D D on D.Fixture_Return_M_UID=A.Fixture_Return_M_UID
                inner join Fixture_M E on E.Fixture_M_UID=C.Fixture_M_UID 
                inner join System_Organization F on F.Organization_UID=A.Plant_Organization_UID
                left join Fixture_Machine G on G.Fixture_Machine_UID=E.Fixture_Machine_UID
                left join Production_Line P on G.Production_Line_UID=P.Production_Line_UID
                left join Process_Info Q on Q.Process_Info_UID=P.Process_Info_UID
                left join WorkStation R on R.WorkStation_UID=P.Workstation_UID
                left join Workshop S on S.Workshop_UID=P.Workshop_UID
                inner join BG_Info on BG_Info.BG_Organization_UID=A.BG_Organization_UID
                left join FunPlant on FunPlant.BG_Organization_UID=A.BG_Organization_UID and                        FunPlant.FunPlant_Organization_UID=A.FunPlant_Organization_UID
                inner join System_Users U on U.Account_UID=A.Created_UID
                inner join System_Users V on V.Account_UID=A.Modified_UID
                where 
                    A.Fixture_Return_M_UID={uid}
                order by 
                    A.Created_Date desc";
            var dbList = DataContext.Database.SqlQuery<Fixture_Return_MDTO>(sql).ToList();
            return dbList;
        }

        public string DelFixtureReturnM(int uid)
        {
            //先判断是否可删除
            string sqlJudge = $@" select COUNT(1) from Fixture_Return_D where Fixture_Return_M_UID={uid} and IS_Return=1";
            var countArray = DataContext.Database.SqlQuery<int>(sqlJudge).ToArray();
            if (countArray[0] > 0)
            {
                return "-1";
            }
            else
            {
                string sql1 = $@" delete from Fixture_Return_D where Fixture_Return_M_UID={uid}";
                var retCount1 = DataContext.Database.SqlQuery<int>(sql1).ToArray();
                if (retCount1[0] > 0)
                {
                    string sql2 = $@"  delete from Fixture_Return_M where Fixture_Return_M_UID={uid}";
                    var retCount2 = DataContext.Database.SqlQuery<int>(sql2).ToArray();
                    return retCount2[0].ToString();
                }
                else
                {
                    return "0";
                }
            }
        }

        public string GetCurrentReturnNub()
        {
            string sql = $@"select COUNT(1)+1 from Fixture_Return_M where CONVERT(nvarchar(20), Created_Date,23)= CONVERT(nvarchar(20), GETDATE(),23)";
            //string curr = DateTime.Now.ToString("yyyy-MM-dd");
            //var dbList = DataContext.Fixture_Return_M.Where(x=>x.Created_Date.ToShortDateString()== curr).Count();
            var dbList = DataContext.Database.SqlQuery<int>(sql).ToArray();
            return dbList[0].ToString();
        }

        public List<Fixture_Taken_InfoDTO> FetchAllFixturesBasedReturnMUID(int uid)
        {
            string sql = $@"SELECT 
                                A.Fixture_Return_D_UID,
                                C.Fixture_M_UID,
                                C.ShortCode,
                                C.Fixture_Unique_ID,
                                isnull(E.Process_ID,'')+'_'+isnull(E.Process_Name,'') Process,
                                isnull(F.WorkStation_ID,'')+'_'+isnull(F.WorkStation_Name,'') WorkStation,
                                isnull(D.Line_ID,'')+'_'+isnull(D.Line_Name,'') Line,
                                isnull(A.IS_Return,0) as IS_Return 
                            FROM 
                                dbo.Fixture_Return_D A
                            left JOIN dbo.Fixture_Return_M B ON B.Fixture_Return_M_UID=A.Fixture_Return_M_UID
                            inner JOIN dbo.Fixture_M C ON A.Fixture_M_UID=C.Fixture_M_UID
                            left JOIN dbo.Production_Line D ON D.Production_Line_UID=C.Production_Line_UID
                            left JOIN dbo.Process_Info E ON D.Process_Info_UID=E.Process_Info_UID
                            left JOIN dbo.WorkStation F ON D.Workstation_UID=F.WorkStation_UID
                            WHERE A.Fixture_Return_M_UID={uid}";
            var dbList = DataContext.Database.SqlQuery<Fixture_Taken_InfoDTO>(sql).ToList();
            return dbList;
        }

        public string UpdateFixtureRetrun(Fixture_Return_MDTO dto)
        {
            var entities = DataContext.Fixture_Return_M.Where(x => x.Fixture_Return_M_UID == dto.Fixture_Return_M_UID).ToList();
            foreach (var entity in entities)
            {
                entity.Return_User = dto.Return_User;
                entity.Return_Name = dto.Return_Name;
                entity.Modified_UID = dto.Modified_UID;
                entity.Modified_Date = dto.Modified_Date;
            }

            var ret = DataContext.SaveChanges();
            return ret + "";
        }

        public string UpdateFixtureRetrunD(Fixture_Return_DDTO dto)
        {
            var entities = DataContext.Fixture_Return_D.Where(x => x.Fixture_Return_D_UID == dto.Fixture_Return_D_UID).ToList();
            foreach (var entity in entities)
            {
                entity.IS_Return = 1;
                entity.Modified_UID = dto.Modified_UID;
                entity.Modified_Date = dto.Modified_Date;
            }

            var ret = DataContext.SaveChanges();
            return ret + "";
        }

        public List<Fixture_Return_Index> ExportFixtrueReturn2Excel(Fixture_Return_MDTO searchModel)
        {
            #region 导出Excel
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
                                    SELECT * FROM dbo.System_OrganizationBOM
                                ) one 
                                INNER JOIN dbo.System_OrganizationBOM t1 ON one.ChildOrg_UID=t1.ParentOrg_UID 
                                INNER JOIN dbo.System_Organization t2 ON t1.ChildOrg_UID=t2.Organization_UID
                                INNER JOIN dbo.System_Organization t3 ON t1.ParentOrg_UID=t3.Organization_UID
                                WHERE 
                                    t3.Organization_Name='OP')
                        select distinct 
                                A.Fixture_Return_M_UID,
                                B.Fixture_Totake_M_UID,
                                A.Return_NO,
                                B.Totake_NO as Taken_NO,
                                F.Organization_Name as Plant,
                                BG_Info.OP_TYPES as BG,
                                FunPlant.FunPlantName as FunPlant,
                                isnull(A.Return_User,'') as Return_User,
                                isnull(A.Return_Name,'') as Return_Name,
                                B.Totake_Date,
                                A.Modified_UID,
                                A.Modified_Date,
                                G.Machine_ID,
                                U.User_Name as Modified_UserName,
                                D.Fixture_Return_D_UID
                        from Fixture_Return_M A 
                        inner join Fixture_Totake_M B on A.Fixture_Totake_M_UID=B.Fixture_Totake_M_UID 
                        inner join Fixture_Totake_D C on B.Fixture_Totake_M_UID=C.Fixture_Totake_M_UID 
                        left join Fixture_Return_D D on D.Fixture_Return_M_UID=A.Fixture_Return_M_UID
                        inner join Fixture_M E on E.Fixture_M_UID=C.Fixture_M_UID 
                        inner join System_Organization F on F.Organization_UID=A.Plant_Organization_UID
                        left join Fixture_Machine G on G.Fixture_Machine_UID=E.Fixture_Machine_UID
                        left join Production_Line P on G.Production_Line_UID=P.Production_Line_UID
                        left join Process_Info Q on Q.Process_Info_UID=P.Process_Info_UID
                        left join WorkStation R on R.WorkStation_UID=P.Workstation_UID
                        left join Workshop S on S.Workshop_UID=P.Workshop_UID
                        inner join BG_Info on BG_Info.BG_Organization_UID=A.BG_Organization_UID
                        left join FunPlant on FunPlant.BG_Organization_UID=A.BG_Organization_UID and FunPlant.FunPlant_Organization_UID=A.FunPlant_Organization_UID 
                        left join System_Users U on U.Account_UID=A.Modified_UID where 1=1");
            if (!(searchModel.Plant_Organization_UID == 0))
                sql.Append($" and A.Plant_Organization_UID = {searchModel.Plant_Organization_UID}");
            if (!(searchModel.BG_Organization_UID == 0))
                sql.Append($" and A.BG_Organization_UID = {searchModel.BG_Organization_UID}");
            if (!(searchModel.FunPlant_Organization_UID == 0)&&!(searchModel.FunPlant_Organization_UID==null))
                sql.Append($" and A.FunPlant_Organization_UID = {searchModel.FunPlant_Organization_UID}");
            if (!string.IsNullOrEmpty(searchModel.Return_NO))
                sql.Append($" and A.Return_NO = '{searchModel.Return_NO}'");
            if (!string.IsNullOrEmpty(searchModel.Return_User))
                sql.Append($" and A.Return_User = '{searchModel.Return_User}'");
            if (!(searchModel.Modified_Date_from == DateTime.MinValue))
                sql.Append($" and A.Modified_Date > '{searchModel.Modified_Date_from}'");
            if (!(searchModel.Modified_Date_to == DateTime.MinValue))
                sql.Append($" and A.Modified_Date < '{searchModel.Modified_Date_to}'");
            //and A.Plant_Organization_UID = 1
            //and A.BG_Organization_UID = 3
            //and A.FunPlant_Organization_UID = 29
            //and A.Return_NO = ''
            //and A.Return_User = ''
            //and A.Modified_Date > ''
            if (!string.IsNullOrEmpty(searchModel.Taken_NO))
                sql.Append($" and B.Totake_NO = '{searchModel.Taken_NO}'");
            //and B.Totake_NO = ''


            if (!string.IsNullOrEmpty(searchModel.Line_NO))
                sql.Append($" and P.Line_Name = N'{searchModel.Line_NO}'");
            //and P.Line_Name = ''


            if (!string.IsNullOrEmpty(searchModel.Fixture_NO))
                sql.Append($" and E.Fixture_NO = '{searchModel.Fixture_NO}'");
            if (!string.IsNullOrEmpty(searchModel.Fixture_Unique_ID))
                sql.Append($" and E.Fixture_Unique_ID = '{searchModel.Fixture_Unique_ID}'");
            if (!string.IsNullOrEmpty(searchModel.ShortCode))
                sql.Append($" and E.ShortCode = '{searchModel.ShortCode}'");

            //and E.Fixture_NO = ''
            //and E.Fixture_Unique_ID = ''
            //and E.ShortCode = ''

            if (!string.IsNullOrEmpty(searchModel.Machine_ID))
                sql.Append($" and G.Machine_ID = N'{searchModel.Machine_ID}'");
            //and G.Machine_ID = ''

            if (!string.IsNullOrEmpty(searchModel.Process_Name))
                sql.Append($" and Q.Process_Name = N'{searchModel.Process_Name}'");
            //and Q.Process_Name = ''


            if (!string.IsNullOrEmpty(searchModel.WorkStation_Name))
                sql.Append($" and R.WorkStation_Name = N'{searchModel.WorkStation_Name}'");
            //and R.WorkStation_Name = ''

            if (!string.IsNullOrEmpty(searchModel.Workshop_Name))
                sql.Append($" and S.Workshop_Name = N'{searchModel.Workshop_Name}'");
            //and S.Workshop_Name = ''

            if (!string.IsNullOrEmpty(searchModel.Modified_UserNTID))
                sql.Append($" and U.User_NTID = '{searchModel.Modified_UserNTID}'");
            //and U.User_NTID = ''




            sql.Append($@" order by A.Fixture_Return_M_UID desc");
            #endregion
            var dbList = DataContext.Database.SqlQuery<Fixture_Return_Index>(sql.ToString()).ToList();
            return dbList;
        }
    }

}
