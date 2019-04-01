using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Common.Constants;

namespace PDMS.Data.Repository
{
    public interface IFlowChartDetailRepository : IRepository<FlowChart_Detail>
    {
        IQueryable<string> QueryDistinctColor(string customer, string project, string productphase, string parttypes);

        IQueryable<string> GetAllColorByFM(string optype, string project, string productphase, string parttypes);
        IQueryable<string> QueryDistinctColorAPP(string project, string productphase, string parttypes);
        IQueryable<string> GetFunPlantForChart(string customer, string project, string productphase, string parttypes);
        List<int> GetDayVersion(string customer, string project, string productphase, string parttypes, string day);

        IQueryable<string> GetFunPlant(string customer, string project, string productphase, string parttypes);
        IQueryable<string> GetFunPlantAPP(string project, string productphase, string parttypes);

        IQueryable<string> QueryFunPlant(string customername, string projectname, string productphasename, string parttypesname, string color);

        IQueryable<string> QueryProcess(string customername, string projectname, string productphasename, string parttypesname, string color, string funPlant);

        IQueryable<int> QueryDistinctVersion(string customer, string project, string productphase,
            string parttypes, DateTime beginTime, DateTime endTime);

        int QueryVersion(string customer, string project, string productphase,
         string parttypes, DateTime referenceDay);

        VersionBeginEndDate GetVersionBeginEndDate(string customer, string project, string productphase,
            string parttypes, int version);

        List<string> QueryProcess(int FlowChart_Master_UID);
        List<string> QueryPlantByUser(int userid);

        List<CheckPointVM> GetCheckPointsList(int UserUId, int FlowChart_Master_UID, string Color, string MaterielType);
        List<CheckPointVM> GetCheckPointsForSearchHistory(string funplant, int FlowChart_Master_UID, string Color, DateTime Product_Date);

        CheckPointInputCondition QueryInputConditions(int FlowChart_Master_UID);

        List<CheckPointVM> QueryCheckPointByProject(string masterUID, string FunPlant);

        int CheckBomUser(GetFuncPlantProcessSearch search);

        List<FlowChartDetailAndMGDataDTO> QueryExportWUXI_MDetailList(int masterUID, int masterVersion);

        IQueryable<FlowChartBomGet> QueryBomByFlowChartUID(int id, int version, List<int> plants);

        List<string> QueryRecordColor(int Flowchart_Master_UID, string FunPlant, string ProductDate, string MaterialType);

        List<FlowChartDetailAndMGDataDTO> QueryExportDetailList(int masterUID, int masterVersion);

        List<FlowChartBomGet> QueryFLDetailByUIDAndVersion(int MasterUID, int Version, List<int> idList);

        List<FunPlantVM> QueryFunPlant(int Flowchart_Master_UID);

        /// <summary>
        /// 获取修改前的WIP值
        /// </summary>
        /// <param name="flId"></param>
        /// <returns></returns>
        int GetWIPValueByFLID(int flId);

        int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq, string Color);
        int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq);
        FlowChart_Detail GetNextProcess_Seq(int flowChartMasterID, int flowChart_Version, int process_Seq, string color);
        FlowChart_Detail GetProProcess_Seq(int flowChartMasterID, int flowChart_Version, int process_Seq, string color);
    }
    public class FlowChartDetailRepository : RepositoryBase<FlowChart_Detail>, IFlowChartDetailRepository
    {
        private Logger log = new Logger("FlowChartDetailRepository");
        public FlowChartDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        /// <summary>
        /// 通过绑定序号获取制程序号
        /// </summary>
        /// <param name="flowChartMasterID"></param>
        /// <param name="Binding_Seq"></param>
        /// <returns></returns>
        public int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq, string Color)
        {
            var query = from detial in DataContext.FlowChart_Detail
                        where detial.FlowChart_Master_UID == flowChartMasterID && detial.Binding_Seq == Binding_Seq && detial.Color == Color
                        select detial;
            var model = query.OrderByDescending(p => p.FlowChart_Version).FirstOrDefault();
            if (model != null)
            {
                return model.FlowChart_Detail_UID;
            }

            return 0;
        }

        /// <summary>
        /// 通过绑定序号获取制程序号
        /// </summary>
        /// <param name="flowChartMasterID"></param>
        /// <param name="Binding_Seq"></param>
        /// <returns></returns>
        public int GetFlowChart_DetailByID(int flowChartMasterID, int Binding_Seq)
        {
            var query = from detial in DataContext.FlowChart_Detail
                        where detial.FlowChart_Master_UID == flowChartMasterID && detial.Binding_Seq == Binding_Seq
                        select detial;
            var model = query.OrderByDescending(p => p.FlowChart_Version).FirstOrDefault();
            if (model != null)
            {
                return model.FlowChart_Detail_UID;
            }

            return 0;
        }

        /// <summary>
        /// 从FlowChartDetail获取资料公用方法
        /// </summary>
        /// <param name="customername"></param>
        /// <param name="projectname"></param>
        /// <param name="productphasename"></param>
        /// <param name="parttypesname"></param>
        /// <returns></returns>
        public IQueryable<FlowChart_Detail> DetailCommonSource(string customername, string projectname, string productphasename, string parttypesname)
        {
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (bud.BU_D_Name == customername && project.Project_Name == projectname)
                        select (project.Project_UID);
            var project_uid = query.FirstOrDefault();
            var query_parttypes = from flowmaster in DataContext.FlowChart_Master
                                  join flowdetail in DataContext.FlowChart_Detail on flowmaster.FlowChart_Master_UID equals flowdetail.FlowChart_Master_UID
                                  where (flowmaster.Project_UID == project_uid && flowmaster.Part_Types == parttypesname
                                  && flowmaster.FlowChart_Version == flowdetail.FlowChart_Version && flowdetail.Color != null && flowdetail.Color != "")
                                  select (flowdetail);
            return query_parttypes;
        }
        public IQueryable<FlowChart_Detail> DetailCommonSourceAPP(string projectname, string productphasename, string parttypesname)
        {
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (project.Project_Name == projectname)
                        select (project.Project_UID);
            var project_uid = query.FirstOrDefault();
            var query_parttypes = from flowmaster in DataContext.FlowChart_Master
                                  join flowdetail in DataContext.FlowChart_Detail on flowmaster.FlowChart_Master_UID equals flowdetail.FlowChart_Master_UID
                                  where (flowmaster.Project_UID == project_uid && flowmaster.Part_Types == parttypesname
                                  && flowmaster.FlowChart_Version == flowdetail.FlowChart_Version && flowdetail.Color != null && flowdetail.Color != "")
                                  select (flowdetail);
            return query_parttypes;
        }
        public IQueryable<string> GetFunPlantForChart(string customername, string projectname, string productphasename, string parttypesname)
        {
            var query = from eu in DataContext.Enumeration
                        where projectname == eu.Decription && parttypesname == eu.Enum_Name && eu.Enum_Type != "Report_Key_Process"
                        orderby eu.Enum_UID descending
                        select eu.Enum_Type
                       ;
            return query.Distinct();

        }
        /// <summary>
        /// 获取颜色------------------------------------Sidney
        /// </summary>
        /// <param name="customername"></param>
        /// <param name="projectname"></param>
        /// <param name="productphasename"></param>
        /// <param name="parttypesname"></param>
        /// <returns></returns>
        public IQueryable<string> QueryDistinctColor(string customername, string projectname, string productphasename, string parttypesname)
        {
            var query = DetailCommonSource(customername, projectname, productphasename, parttypesname);
            return query.Select(p => p.Color).Distinct();
        }


        public IQueryable<string> GetAllColorByFM(string optype, string projectname, string productphasename, string parttypesname)
        {
            var query = from fm in DataContext.FlowChart_Master
                        join project in DataContext.System_Project on fm.Project_UID equals project.Project_UID
                        join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                        where (project.Product_Phase == productphasename && project.OP_TYPES == optype && fm.Part_Types == parttypesname
                        && project.Project_Name == projectname)
                        select (fd.Color);

            return query.Distinct();
        }
        public List<int> GetDayVersion(string customer, string project, string productphase, string parttypes, string day)
        {
            var date = Convert.ToDateTime(day);

            var sqlstr = @"SELECT TOP 1 * FROM dbo.FlowChart_Master A
                            JOIN dbo.System_Project B
                            ON A.Project_UID = B.Project_UID
                            JOIN dbo.System_BU_D C
                            ON B.BU_D_UID = C.BU_D_UID
                            WHERE 
                            C.BU_D_Name =N'{0}' AND B.Project_Name =N'{1}' 
                            AND B.Product_Phase=N'{2}' AND A.Part_Types=N'{3}'";
            sqlstr = string.Format(sqlstr, customer, project, productphase, parttypes);
            var flMasterInfo = DataContext.FlowChart_Master.SqlQuery(sqlstr).FirstOrDefault();
            if (flMasterInfo != null)
            {
                //如果查询的日期是7天之前的，则查询历史表
                if (date.Date.AddDays(7) < DateTime.Now.Date)
                {
                    var list = DataContext.Product_Input_History.Where(m => m.Product_Date == date && m.FlowChart_Master_UID == flMasterInfo.FlowChart_Master_UID)
                        .Select(m => m.FlowChart_Version).Distinct();
                    return list.OrderByDescending(m => m).ToList();
                }
                else
                {
                    var list = DataContext.Product_Input.Where(m => m.Product_Date == date && m.FlowChart_Master_UID == flMasterInfo.FlowChart_Master_UID)
                        .Select(m => m.FlowChart_Version).Distinct();

                    return list.OrderByDescending(m => m).ToList();
                }

            }
            return null;
        }

        public IQueryable<string> QueryDistinctColorAPP(string projectname, string productphasename, string parttypesname)
        {
            var query = DetailCommonSourceAPP(projectname, productphasename, parttypesname);
            return query.Select(p => p.Color).Distinct();
        }

        public IQueryable<string> GetFunPlant(string customername, string projectname, string productphasename, string parttypesname)
        {
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (bud.BU_D_Name == customername && project.Project_Name == projectname)
                        select (project.Project_UID);
            var project_uid = query.FirstOrDefault();
            var query_parttypes = from flowmaster in DataContext.FlowChart_Master
                                  join flowdetail in DataContext.FlowChart_Detail on flowmaster.FlowChart_Master_UID equals flowdetail.FlowChart_Master_UID
                                  where (flowmaster.Project_UID == project_uid && flowmaster.Part_Types == parttypesname
                                  && flowmaster.FlowChart_Version == flowdetail.FlowChart_Version)
                                  select (flowdetail);


            return query_parttypes.Select(p => p.System_Function_Plant.FunPlant).Distinct();

        }

        public IQueryable<string> GetFunPlantAPP(string projectname, string productphasename, string parttypesname)
        {
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (project.Project_Name == projectname)
                        select (project.Project_UID);
            var project_uid = query.FirstOrDefault();
            var query_parttypes = from flowmaster in DataContext.FlowChart_Master
                                  join flowdetail in DataContext.FlowChart_Detail on flowmaster.FlowChart_Master_UID equals flowdetail.FlowChart_Master_UID
                                  where (flowmaster.Project_UID == project_uid && flowmaster.Part_Types == parttypesname
                                  && flowmaster.FlowChart_Version == flowdetail.FlowChart_Version)
                                  select (flowdetail);

            return query_parttypes.Select(p => p.System_Function_Plant.FunPlant).Distinct();

        }

        /// <summary>
        /// QueryFunPlant
        /// </summary>
        /// <param name="customername"></param>
        /// <param name="projectname"></param>
        /// <param name="productphasename"></param>
        /// <param name="parttypesname"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public IQueryable<string> QueryFunPlant(string customername, string projectname, string productphasename,
            string parttypesname, string color)
        {
            var query = DetailCommonSource(customername, projectname, productphasename, parttypesname);
            if (color != "ALL")
            {
                query = query.Where(p => p.Color == color);
                return query.Select(p => p.System_Function_Plant.FunPlant).Distinct();
            }
            else
            {
                return query.Select(p => p.System_Function_Plant.FunPlant).Distinct();
            }

        }
        /// <summary>
        /// QueryProcess
        /// </summary>
        /// <param name="customername"></param>
        /// <param name="projectname"></param>
        /// <param name="productphasename"></param>
        /// <param name="parttypesname"></param>
        /// <param name="color"></param>
        /// <param name="funPlant"></param>
        /// <returns></returns>
        public IQueryable<string> QueryProcess(string customername, string projectname, string productphasename, string parttypesname, string color, string funPlant)
        {
            var query = DetailCommonSource(customername, projectname, productphasename, parttypesname);
            if (color != "ALL")
                query = query.Where(p => p.Color == color);
            if (funPlant != "ALL")
                query = query.Where(p => p.System_Function_Plant.FunPlant == funPlant);
            var queryParttypes = query.Select(p => p.Process);
            return queryParttypes.Distinct();
        }

        #region Day Week Month Report Function------------------------Sidney 2016/01/28 

        public List<string> QueryPlantByUser(int userid)
        {
            string sql = string.Format(@"SELECT DISTINCT t6.Organization_Name FROM dbo.FlowChart_Detail t1 INNER JOIN dbo.FlowChart_PC_MH_Relationship t2
                                        ON t2.FlowChart_Detail_UID = t1.FlowChart_Detail_UID INNER JOIN dbo.System_Users t3
                                        ON t3.Account_UID=t2.MH_UID INNER JOIN dbo.System_Function_Plant t4
                                        ON t1.System_FunPlant_UID=t4.System_FunPlant_UID INNER JOIN dbo.System_OrganizationBOM t5
                                        ON t4.OPType_OrganizationUID=t5.ChildOrg_UID INNER JOIN dbo.System_Organization t6
                                        ON t5.ParentOrg_UID=t6.Organization_UID WHERE t3.Account_UID={0}", userid);

            var dbList = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dbList;
        }

        public IQueryable<int> QueryDistinctVersion(string customer, string project, string productphase,
            string parttypes, DateTime beginTime, DateTime endTime)
        {
            var query = from bud in DataContext.System_BU_D
                        join pro in DataContext.System_Project on bud.BU_D_UID equals pro.BU_D_UID
                        where (bud.BU_D_Name == customer && pro.Project_Name == project)
                        select (pro.Project_UID);
            var projectUid = query.FirstOrDefault();
            int betweenDay_B = (DateTime.Now - beginTime).Days; ;
            int betweenDay_T = (DateTime.Now - endTime).Days;
            if (betweenDay_B >= 7 && betweenDay_T >= 7) //  Justin 2-2 补充 需要查询出日期 如 版本 3 时间2-3到2~9   且计算版本时候有误， 开始日期大于七天，若结束日期在七天内，版本丢失
            {
                var queryVision = from fm in DataContext.FlowChart_Master
                                  join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                                  join pd in DataContext.Product_Input_History on fd.FlowChart_Detail_UID equals pd.FlowChart_Detail_UID
                                  where (pd.Create_Date >= beginTime && pd.Create_Date <= endTime &&
                                         fm.Project_UID == projectUid && fm.Part_Types == parttypes && pd.Is_Comfirm == true)
                                  select fd.FlowChart_Version;
                return queryVision.Distinct();
            }
            else if (betweenDay_B < 7 && betweenDay_T < 7)
            {
                var queryVision = from fm in DataContext.FlowChart_Master
                                  join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                                  join pd in DataContext.Product_Input on fd.FlowChart_Detail_UID equals pd.FlowChart_Detail_UID
                                  where (pd.Create_Date >= beginTime && pd.Create_Date <= endTime &&
                                         fm.Project_UID == projectUid && fm.Part_Types == parttypes && pd.Is_Comfirm == true)
                                  select fd.FlowChart_Version;
                return queryVision.Distinct();
            }
            else
            {
                var demoDay = DateTime.Now.AddDays(-7);
                var queryVision1 = from fm in DataContext.FlowChart_Master
                                   join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                                   join pd in DataContext.Product_Input_History on fd.FlowChart_Detail_UID equals pd.FlowChart_Detail_UID
                                   where (pd.Create_Date >= beginTime && pd.Create_Date <= demoDay &&
                                          fm.Project_UID == projectUid && fm.Part_Types == parttypes && pd.Is_Comfirm == true)
                                   select fd.FlowChart_Version;
                var queryVision2 = from fm in DataContext.FlowChart_Master
                                   join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                                   join pd in DataContext.Product_Input on fd.FlowChart_Detail_UID equals pd.FlowChart_Detail_UID
                                   where (pd.Create_Date >= demoDay && pd.Create_Date <= endTime &&
                                          fm.Project_UID == projectUid && fm.Part_Types == parttypes && pd.Is_Comfirm == true)
                                   select fd.FlowChart_Version;
                var result = queryVision1.Union(queryVision2).Distinct();
                return result;
            }
        }

        public int QueryVersion(string customer, string project, string productphase,
         string parttypes, DateTime referenceDay)
        {
            var query = from bud in DataContext.System_BU_D
                        join pro in DataContext.System_Project on bud.BU_D_UID equals pro.BU_D_UID
                        where (bud.BU_D_Name == customer && pro.Project_Name == project)
                        select (pro.Project_UID);
            var projectUid = query.FirstOrDefault();
            int betweenDay_B = (DateTime.Now - referenceDay).Days; ;

            if (betweenDay_B >= 7)
            {
                var queryVision = from fm in DataContext.FlowChart_Master
                                  join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                                  join pd in DataContext.Product_Input_History on fd.FlowChart_Detail_UID equals pd.FlowChart_Detail_UID
                                  where (pd.Product_Date == referenceDay &&
                                         fm.Project_UID == projectUid && fm.Part_Types == parttypes && pd.Is_Comfirm == true)
                                  select fd.FlowChart_Version;
                return queryVision.FirstOrDefault();
            }
            else
            {
                var queryVision = from fm in DataContext.FlowChart_Master
                                  join fd in DataContext.FlowChart_Detail on fm.FlowChart_Master_UID equals fd.FlowChart_Master_UID
                                  join pd in DataContext.Product_Input on fd.FlowChart_Detail_UID equals pd.FlowChart_Detail_UID
                                  where (pd.Product_Date == referenceDay &&
                                         fm.Project_UID == projectUid && fm.Part_Types == parttypes && pd.Is_Comfirm == true)
                                  select fd.FlowChart_Version;
                return queryVision.FirstOrDefault();
            }

        }
        public VersionBeginEndDate GetVersionBeginEndDate(string customer, string project, string productphase,
            string parttypes, int version)
        {
            string sql = string.Empty;
            sql = @"select MIN(pi.Create_Date)VersionBeginDate,MAX(pi.Create_Date)VersionEndDate from
                    (SELECT   [Is_Comfirm] ,[Product_Date] ,[Time_Interval] ,[Customer] ,[Project] ,[Part_Types] ,
                   [FunPlant] ,[FunPlant_Manager] ,[Product_Phase] ,[Process_Seq] ,[Place] ,[Process] ,[FlowChart_Master_UID] ,
                   [FlowChart_Version] ,[Color] ,[Prouct_Plan] ,[Product_Stage] ,[Target_Yield] ,[Good_QTY] ,[Good_MismatchFlag] ,
                   [Picking_QTY] ,[WH_Picking_QTY] ,[Picking_MismatchFlag] ,[NG_QTY] ,[WH_QTY] ,[WIP_QTY] ,[Adjust_QTY] ,
                   [Creator_UID] ,[Create_Date] ,[Material_No] ,[Modified_UID] ,[Modified_Date] ,DRI ,FlowChart_Detail_UID
                   FROM     dbo.Product_Input
                    where Is_Comfirm=1
                   UNION
                   SELECT   [Is_Comfirm] ,[Product_Date] ,[Time_Interval] ,[Customer] ,[Project] ,[Part_Types] ,
                   [FunPlant] ,[FunPlant_Manager] ,[Product_Phase] ,[Process_Seq] ,[Place] ,[Process] ,[FlowChart_Master_UID] ,
                   [FlowChart_Version] ,[Color] ,[Prouct_Plan] ,[Product_Stage] ,[Target_Yield] ,[Good_QTY] ,[Good_MismatchFlag] ,
                   [Picking_QTY] ,[WH_Picking_QTY] ,[Picking_MismatchFlag] ,[NG_QTY] ,[WH_QTY] ,[WIP_QTY] ,[Adjust_QTY] ,
                   [Creator_UID] ,[Create_Date] ,[Material_No] ,[Modified_UID] ,[Modified_Date] ,DRI ,FlowChart_Detail_UID
                   FROM     dbo.Product_Input_History
                    where Is_Comfirm=1) AS pi,
                    dbo.FlowChart_Detail AS fcd,dbo.FlowChart_Master AS fcm,
                    dbo.System_BU_D AS sbd,dbo.System_Project AS sp
                    WHERE 
                    pi.FlowChart_Detail_UID=fcd.FlowChart_Detail_UID
                    AND pi.FlowChart_Master_UID=fcd.FlowChart_Master_UID
                    AND fcm.FlowChart_Master_UID=fcd.FlowChart_Master_UID
                    AND sp.BU_D_UID=sbd.BU_D_UID
                    AND sp.Project_UID=fcm.Project_UID
                    AND sbd.BU_D_Name='{0}'
                    AND sp.Project_Name='{1}'
                    AND sp.Product_Phase='{2}'
                    AND fcm.Part_Types='{3}'
                    AND pi.FlowChart_Version={4}";
            sql = string.Format(sql, customer, project, productphase, parttypes, version);
            var dbList = DataContext.Database.SqlQuery<VersionBeginEndDate>(sql).ToList();
            return dbList[0];
        }
        #endregion

        public List<string> QueryProcess(int FlowChart_Master_UID)
        {
            string sql = string.Format(@"
                                        SELECT  DISTINCT process as Process
                                        FROM    dbo.FlowChart_Detail FD WITH ( NOLOCK )
                                        WHERE   Process NOT IN (
                                                SELECT DISTINCT
                                                        Process
                                                FROM    dbo.FlowChart_Detail FD WITH ( NOLOCK )
                                                        INNER JOIN dbo.FlowChart_Master FM WITH ( NOLOCK ) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                        INNER JOIN dbo.Enumeration en WITH ( NOLOCK ) ON en.Enum_Name = FM.Part_Types
                                                                                                      AND en.Enum_Value = FD.Process
                                                WHERE   en.Enum_Type = 'Report_Key_Process'
                                                        AND FM.FlowChart_Master_UID = {0} )
                                                AND FD.FlowChart_Master_UID = {0}
                                        ", FlowChart_Master_UID);

            var dbList = DataContext.Database.SqlQuery<string>(sql).ToList();
            return dbList;
        }

        #region ----- QA 

        /// <summary>
        /// get check ponints from flowchartdetail by user_Uid
        /// </summary>
        /// <param name="UserUId"></param>
        /// <returns></returns>
        public List<CheckPointVM> GetCheckPointsList(int UserUId, int FlowChart_Master_UID, string Color, string MaterielType)
        {
            List<CheckPointVM> result = new List<CheckPointVM>();
            try
            {
                string sql = string.Format(@"
DECLARE @Temp TABLE
    (
      ProcessSeq INT ,
      ProcessName NVARCHAR(50) ,
      ProjectName NVARCHAR(50) ,
      Flowchart_Master_UID INT ,
      Flowchart_Detail_UID int,
      FunPlant NVARCHAR(50) ,
      IsQAProcess NVARCHAR(50) ,
      QaUID INT
    )
IF EXISTS ( SELECT TOP 1
                    1
            FROM    dbo.System_User_Role SUR WITH ( NOLOCK )
                    INNER JOIN dbo.System_Role SR WITH ( NOLOCK ) ON SR.Role_UID = SUR.Role_UID
            WHERE   SUR.Account_UID = {1}
                    AND SR.Role_ID LIKE '%QA IPQC Input%' )
    BEGIN
        INSERT  INTO @Temp
                ( ProcessSeq ,
                  ProcessName ,
                  ProjectName ,
                  Flowchart_Master_UID ,
                  Flowchart_Detail_UID,
                  FunPlant ,
                  IsQAProcess ,
                  QaUID
                )
                SELECT  DISTINCT
                        Fd.Process_Seq ,
                        FD.Process ,
                        SP.Project_Name ,
                        FM.FlowChart_Master_UID ,
                        FD.Flowchart_Detail_UID,
                        SFP.FunPlant ,
                        FD.IsQAProcess ,
                        MAX(qa.QualityAssurance_InputMaster_UID)
                FROM    dbo.FlowChart_Detail FD WITH ( NOLOCK )
                        INNER JOIN dbo.FlowChart_Master FM WITH ( NOLOCK ) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                              AND FM.FlowChart_Version = FD.FlowChart_Version
                        INNER JOIN dbo.System_Project SP WITH ( NOLOCK ) ON SP.Project_UID = FM.Project_UID
                        INNER JOIN dbo.System_Function_Plant SFP WITH ( NOLOCK ) ON SFP.System_FunPlant_UID = FD.System_FunPlant_UID
                        Left JOIN (SELECT * FROM  dbo.QualityAssurance_InputMaster WHERE MaterielType=N'{3}') qa ON qa.FlowChart_Detail_UID = FD.FlowChart_Detail_UID
                WHERE   FD.FlowChart_Master_UID = {0} and (ISNULL(FD.Color,N'{2}')=N'{2}' OR FD.Color='') 
                        AND FD.IsQAProcess IN ( 'Inspect_IPQC', 'Polling_IPQC' )
                GROUP BY Fd.Process_Seq ,
                        FM.FlowChart_Master_UID ,
                        FD.IsQAProcess ,
                        FD.Process ,
                        SP.Project_Name ,FD.Flowchart_Detail_UID,
                        SFP.FunPlant

        SELECT  N'最新录入:日期[' + CAST(qa.Product_Date AS NVARCHAR(50)) + N']时段['
                + CAST(qa.Time_Interval AS NVARCHAR(50)) + N'颜色[' + qa.Color
                + ']' + N']WIP[' + CAST(qa.WIPForCheck_Qty AS NVARCHAR(50))
                + ']' AS OtherInfos ,
                T.ProcessSeq ,
                T.ProcessName ,
                T.ProjectName ,
                T.Flowchart_Master_UID ,
                T.Flowchart_Detail_UID,
                T.FunPlant ,
                T.IsQAProcess
        FROM    @Temp T LEFT JOIN dbo.QualityAssurance_InputMaster qa ON T.QaUID = qa.QualityAssurance_InputMaster_UID 
	ORDER BY T.ProcessSeq
    END
ELSE
    IF EXISTS ( SELECT TOP 1
                        1
                FROM    dbo.System_User_Role SUR WITH ( NOLOCK )
                        INNER JOIN dbo.System_Role SR WITH ( NOLOCK ) ON SR.Role_UID = SUR.Role_UID
                WHERE   SUR.Account_UID = {1}
                        AND SR.Role_ID LIKE '%QA OQC Input%' )
        BEGIN
            INSERT  INTO @Temp
                    ( ProcessSeq ,
                      ProcessName ,
                      ProjectName ,
                      Flowchart_Master_UID ,
                      Flowchart_Detail_UID,
                      FunPlant ,
                      IsQAProcess ,
                      QaUID
                    )
                   SELECT  DISTINCT
                        Fd.Process_Seq ,
                        FD.Process ,
                        SP.Project_Name ,
                        FM.FlowChart_Master_UID ,
                        FD.Flowchart_Detail_UID,
                        SFP.FunPlant ,
                        FD.IsQAProcess ,
                        MAX(qa.QualityAssurance_InputMaster_UID)
                FROM    dbo.FlowChart_Detail FD WITH ( NOLOCK )
                        INNER JOIN dbo.FlowChart_Master FM WITH ( NOLOCK ) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                              AND FM.FlowChart_Version = FD.FlowChart_Version
                        INNER JOIN dbo.System_Project SP WITH ( NOLOCK ) ON SP.Project_UID = FM.Project_UID
                        INNER JOIN dbo.System_Function_Plant SFP WITH ( NOLOCK ) ON SFP.System_FunPlant_UID = FD.System_FunPlant_UID
                        Left JOIN (SELECT * FROM  dbo.QualityAssurance_InputMaster WHERE MaterielType=N'{3}') qa ON qa.FlowChart_Detail_UID = FD.FlowChart_Detail_UID
                WHERE   FD.FlowChart_Master_UID = {0} and (ISNULL(FD.Color,N'{2}')=N'{2}' OR FD.Color='') 
                                                   AND FD.IsQAProcess LIKE '%Inspect_OQC%'
                GROUP BY Fd.Process_Seq ,
                        FM.FlowChart_Master_UID ,
                        FD.IsQAProcess ,
                        FD.Process ,
                        SP.Project_Name ,FD.Flowchart_Detail_UID,
                        SFP.FunPlant

        SELECT  N'最新录入:日期[' + CAST(qa.Product_Date AS NVARCHAR(50)) + N']时段['
                + CAST(qa.Time_Interval AS NVARCHAR(50)) + N'颜色[' + qa.Color
                + ']' + N']WIP[' + CAST(qa.WIPForCheck_Qty AS NVARCHAR(50))
                + ']' AS OtherInfos ,
                T.ProcessSeq ,
                T.ProcessName ,
                T.ProjectName ,
                T.Flowchart_Master_UID ,
                T.Flowchart_Detail_UID,
                T.FunPlant ,
                T.IsQAProcess
        FROM    @Temp T LEFT JOIN dbo.QualityAssurance_InputMaster qa ON T.QaUID = qa.QualityAssurance_InputMaster_UID 
	ORDER BY T.ProcessSeq
        END
    ELSE
        IF EXISTS ( SELECT TOP 1
                            1
                    FROM    dbo.System_User_Role SUR WITH ( NOLOCK )
                            INNER JOIN dbo.System_Role SR WITH ( NOLOCK ) ON SR.Role_UID = SUR.Role_UID
                    WHERE   SUR.Account_UID = {1}
                            AND SR.Role_ID LIKE '%QA Assemble Input%' )
            BEGIN
                INSERT  INTO @Temp
                        ( ProcessSeq ,
                          ProcessName ,
                          ProjectName ,
                          Flowchart_Master_UID ,
                          Flowchart_Detail_UID,
                          FunPlant ,
                          IsQAProcess ,
                          QaUID
                        )
                       SELECT  DISTINCT
                        Fd.Process_Seq ,
                        FD.Process ,
                        SP.Project_Name ,
                        FM.FlowChart_Master_UID ,
                        FD.Flowchart_Detail_UID,
                        SFP.FunPlant ,
                        FD.IsQAProcess ,
                        MAX(qa.QualityAssurance_InputMaster_UID)
                FROM    dbo.FlowChart_Detail FD WITH ( NOLOCK )
                        INNER JOIN dbo.FlowChart_Master FM WITH ( NOLOCK ) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                              AND FM.FlowChart_Version = FD.FlowChart_Version
                        INNER JOIN dbo.System_Project SP WITH ( NOLOCK ) ON SP.Project_UID = FM.Project_UID
                        INNER JOIN dbo.System_Function_Plant SFP WITH ( NOLOCK ) ON SFP.System_FunPlant_UID = FD.System_FunPlant_UID
                        Left JOIN (SELECT * FROM  dbo.QualityAssurance_InputMaster WHERE MaterielType=N'{3}') qa ON qa.FlowChart_Detail_UID = FD.FlowChart_Detail_UID
                WHERE   FD.FlowChart_Master_UID = {0} and (ISNULL(FD.Color,N'{2}')=N'{2}' OR FD.Color='') 
  AND FD.IsQAProcess IN ( 'Inspect_Assemble' )
                GROUP BY Fd.Process_Seq ,
                        FM.FlowChart_Master_UID ,
                        FD.IsQAProcess ,
                        FD.Process ,
                        SP.Project_Name ,FD.Flowchart_Detail_UID,
                        SFP.FunPlant

        SELECT  N'最新录入:日期[' + CAST(qa.Product_Date AS NVARCHAR(50)) + N']时段['
                + CAST(qa.Time_Interval AS NVARCHAR(50)) + N'颜色[' + qa.Color
                + ']' + N']WIP[' + CAST(qa.WIPForCheck_Qty AS NVARCHAR(50))
                + ']' AS OtherInfos ,
                T.ProcessSeq ,
                T.ProcessName ,
                T.ProjectName ,
                T.Flowchart_Master_UID ,
                T.Flowchart_Detail_UID,
                T.FunPlant ,
                T.IsQAProcess
        FROM    @Temp T LEFT JOIN dbo.QualityAssurance_InputMaster qa ON T.QaUID = qa.QualityAssurance_InputMaster_UID 
	ORDER BY T.ProcessSeq
            END
                                           
                        ", FlowChart_Master_UID, UserUId, Color, MaterielType);
                result = DataContext.Database.SqlQuery<CheckPointVM>(sql).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 历史查询界面获取工站
        /// </summary>
        /// <param name="funplant"></param>
        /// <param name="flowchartMasterUID"></param>
        /// <returns></returns>
        public List<CheckPointVM> GetCheckPointsForSearchHistory(string funplant, int flowchartMasterUID, string Color, DateTime Product_Date)
        {
            List<CheckPointVM> result = new List<CheckPointVM>();
            try
            {
                bool Ishistory = false;
                TimeSpan tSpan = DateTime.Now.Date - Product_Date;
                if (tSpan.Days >= 7)
                {
                    Ishistory = true;
                }

                string sql = "";

                sql = string.Format(@"          SELECT  DISTINCT Fd.Process_Seq AS ProcessSeq ,
                                                            FD.Process AS ProcessName ,
                                                            FD.FlowChart_Detail_UID,
                                                            SP.Project_Name AS ProjectName ,
                                                            FM.FlowChart_Master_UID AS FlowchartMasterUID ,
                                                            SFP.FunPlant AS FunPlant,
                                                            FD.IsQAProcess as IsQAProcess
                                                    FROM    dbo.FlowChart_Detail FD WITH (NOLOCK)
                                                            INNER JOIN dbo.FlowChart_Master FM WITH (NOLOCK) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                            INNER JOIN dbo.System_Project SP WITH (NOLOCK) ON SP.Project_UID = FM.Project_UID
                                                            INNER JOIN dbo.System_Function_Plant SFP WITH (NOLOCK) ON SFP.System_FunPlant_UID = FD.System_FunPlant_UID
                                                            INNER JOIN dbo.{2} qa WITH(NOLOCK) ON qa.FlowChart_Detail_UID = FD.FlowChart_Detail_UID
                                                    WHERE   FD.FlowChart_Master_UID = {0} and qa.Product_Date=N'{1}' and qa.Color=N'{3}' ", flowchartMasterUID, Product_Date.ToString(FormatConstants.DateTimeFormatStringByDate), Ishistory ? "QualityAssurance_InputMaster_History" : "QualityAssurance_InputMaster", Color);




                result = DataContext.Database.SqlQuery<CheckPointVM>(sql).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        public CheckPointInputCondition QueryInputConditions(int FlowChart_Master_UID)
        {
            CheckPointInputCondition result = new CheckPointInputCondition();
            try
            {
                var query = from FM in DataContext.FlowChart_Master
                            where FM.FlowChart_Master_UID == FlowChart_Master_UID
                            select FM.Part_Types;

                string sql = string.Format(@"
                                            SELECT DISTINCT
                                                    FD.Color
                                            FROM    dbo.FlowChart_Detail FD WITH(NOLOCK)
                                                    INNER JOIN dbo.FlowChart_Master FM WITH(NOLOCK) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                                                            AND FM.FlowChart_Version = FD.FlowChart_Version
                                            WHERE   FM.FlowChart_Master_UID = {0}", FlowChart_Master_UID);

                List<FlowchartColor> colorList = new List<FlowchartColor>();
                colorList = DataContext.Database.SqlQuery<FlowchartColor>(sql).ToList();
                result.ColorList = colorList.Distinct().ToList();
                result.Part_Types = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }
        public OQCRecordCondition QueryOQCReportConditions(string processName, string Project)
        {
            OQCRecordCondition result = new OQCRecordCondition();
            try
            {
                var queryColor = from Master in DataContext.FlowChart_Master
                                 join p in DataContext.System_Project on Master.Project_UID equals p.Project_UID
                                 join detail in DataContext.FlowChart_Detail on Master.FlowChart_Master_UID equals detail.FlowChart_Detail_UID
                                 where p.Project_Name == Project && detail.Process == processName
                                 select detail.Color;


                var queryPlace = from Master in DataContext.FlowChart_Master
                                 join p in DataContext.System_Project on Master.Project_UID equals p.Project_UID
                                 join detail in DataContext.FlowChart_Detail on Master.FlowChart_Master_UID equals detail.FlowChart_Detail_UID
                                 where p.Project_Name == Project && detail.Process == processName
                                 select detail.Place;

                result.ColorList = queryColor.Distinct().ToList();
                result.Place = queryPlace.Distinct().ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }

        public List<CheckPointVM> QueryCheckPointByProject(string masterUID, string FunPlant)
        {
            List<CheckPointVM> result = new List<CheckPointVM>();
            try
            {
                #region

                masterUID = masterUID.Replace("'", "");
                FunPlant = FunPlant.Replace("'", "");
                string sql = string.Format(@"
                                            DECLARE @FunPlant NVARCHAR(50),
                                                    @MasterUID INT

                                            SET @FunPlant=N'{0}'
                                            SET @MasterUID={1}

                                            IF @FunPlant='IPQC'
                                            BEGIN
                                                SELECT DISTINCT
                                                        FD.Process AS ProcessName ,
                                                        FD.Process_Seq AS ProcessSeq ,
                                                        FD.FlowChart_Master_UID AS FlowchartMasterUID
                                                FROM    dbo.FlowChart_Detail FD WITH ( NOLOCK )
                                                        INNER JOIN dbo.FlowChart_Master FM WITH ( NOLOCK ) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                                                                          AND FM.FlowChart_Version = FD.FlowChart_Version
                                                        INNER JOIN dbo.System_Function_Plant Splant WITH ( NOLOCK ) ON Splant.System_FunPlant_UID = FD.System_FunPlant_UID
                                                WHERE   FD.FlowChart_Master_UID = @MasterUID
                                          
                                                        AND IsQAProcess IN ('Inspect_IPQC','Polling_IPQC')
                                            END
                                            ELSE
                                            BEGIN
                                                SELECT DISTINCT
                                                        FD.Process AS ProcessName ,
                                                        FD.Process_Seq AS ProcessSeq ,
                                                        FD.FlowChart_Master_UID AS FlowchartMasterUID
                                                FROM    dbo.FlowChart_Detail FD WITH ( NOLOCK )
                                                        INNER JOIN dbo.FlowChart_Master FM WITH ( NOLOCK ) ON FM.FlowChart_Master_UID = FD.FlowChart_Master_UID
                                                                                                          AND FM.FlowChart_Version = FD.FlowChart_Version
                                                        INNER JOIN dbo.System_Function_Plant Splant WITH ( NOLOCK ) ON Splant.System_FunPlant_UID = FD.System_FunPlant_UID
                                                WHERE   FD.FlowChart_Master_UID = @MasterUID
                          
                                                        AND Splant.FunPlant= @FunPlant AND IsQAProcess IN ('Inspect_OQC','Polling_OQC')

                                            END", FunPlant, masterUID);

                result = DataContext.Database.SqlQuery<CheckPointVM>(sql).ToList();

                #endregion
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return result;
        }


        public List<string> QueryRecordColor(int Flowchart_Master_UID, string FunPlant, string ProductDate, string MaterialType)
        {
            List<string> result = new List<string>();
            try
            {

                FunPlant = FunPlant.Replace("'", "");
                bool Ishistory = false;

                if (!string.IsNullOrEmpty(ProductDate))
                {
                    TimeSpan tSpan = DateTime.Now.Date - DateTime.Parse(ProductDate);
                    if (tSpan.Days >= 7)
                    {
                        Ishistory = true;
                    }
                }

                string sqlWhere = "";

                string sql = string.Format(@"   
                                                    SELECT  DISTINCT
                                                            QAMaster.Color
                                                    FROM    dbo.{1} QAMaster WITH ( NOLOCK )
                                                            INNER JOIN dbo.FlowChart_Detail FD WITH ( NOLOCK ) ON FD.FlowChart_Detail_UID = QAMaster.FlowChart_Detail_UID
                                                            INNER JOIN dbo.System_Function_Plant WITH ( NOLOCK ) ON System_Function_Plant.System_FunPlant_UID = FD.System_FunPlant_UID
                                                    WHERE   QAMaster.ProductDate = N'{0}'
                                                            AND FD.Flowchart_Master_UID = {2} and QAMaster.MaterialType=N'{4}'
                                                    union 
                                                     SELECT  DISTINCT QAMaster.Color
                                                    FROM    dbo.{3} QAMaster WITH ( NOLOCK )
                                                            INNER JOIN dbo.FlowChart_Detail FD WITH ( NOLOCK ) ON FD.FlowChart_Detail_UID = QAMaster.FlowChart_Detail_UID
                                                    WHERE   QAMaster.Product_Date = N'{0}'
                                                            AND FD.Flowchart_Master_UID = N'{2}' and QAMaster.MaterielType=N'{4}'
                                                   
                                                    ", string.IsNullOrEmpty(ProductDate) ? DateTime.Now.Date.ToString(FormatConstants.DateTimeFormatStringByDate) : ProductDate, Ishistory ? "OQC_InputMaster_History" : "OQC_InputMaster", Flowchart_Master_UID, Ishistory ? "QualityAssurance_InputMaster_History" : "QualityAssurance_InputMaster", MaterialType);
                result = DataContext.Database.SqlQuery<string>(sql).ToList();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        #endregion

        public IQueryable<FlowChartBomGet> QueryBomByFlowChartUID(int id, int version, List<int> plants)
        {
            var linq = from a in DataContext.FlowChart_Detail
                       join b in DataContext.FlowChart_PC_MH_Relationship
                       on a.FlowChart_Detail_UID equals b.FlowChart_Detail_UID
                       join c in DataContext.System_Users
                       on b.MH_UID equals c.Account_UID
                       join d in DataContext.System_Users
                       on b.Modified_UID equals d.Account_UID
                       where a.FlowChart_Master_UID == id && a.FlowChart_Version == version
                       orderby a.Process_Seq, a.Color
                       select new FlowChartBomGet
                       {
                           FlowChart_Detail_UID = a.FlowChart_Detail_UID,
                           System_FunPlant_UID = a.System_FunPlant_UID,
                           PC_MH_UID = b.PC_MH_UID,
                           Process_Seq = a.Process_Seq,
                           Process = a.Process,
                           Place = b.Place,
                           Color = a.Color,
                           User_NTID = c.User_NTID,
                           User_Name = c.User_Name,
                           Modified_UID = b.Modified_UID,
                           Modified_NTID = d.User_NTID,
                           Modified_Name = d.User_Name,
                           Modified_Date = b.Modified_Date,
                           Binding_Seq = a.Binding_Seq
                       };

            if (plants.Count() > 0)
            {
                linq = linq.Where(m => plants.Contains(m.System_FunPlant_UID));
            }

            return linq;


        }


        public int CheckBomUser(GetFuncPlantProcessSearch search)
        {
            var linq = from a in DataContext.FlowChart_Detail
                       join b in DataContext.FlowChart_PC_MH_Relationship
                       on a.FlowChart_Detail_UID equals b.FlowChart_Detail_UID
                       where a.FlowChart_Master_UID == search.Master_Uid && a.FlowChart_Version == search.Version
                       && search.OwnerFuncPlant.Contains(a.System_Function_Plant.FunPlant)
                       select a;
            return linq.ToList().Count();
        }

        public List<FlowChartDetailAndMGDataDTO> QueryExportWUXI_MDetailList(int masterUID, int masterVersion)
        {
            var strSql = @";WITH 
                            one AS
                            (
                            SELECT * FROM dbo.FlowChart_Detail
                            WHERE FlowChart_Master_UID={0} AND FlowChart_Version={1}
                            ),
                            two AS
                            (
                            SELECT one.*,B.FunPlant AS PlantName,
                            CASE one.IsQAProcess 
                            WHEN 'Inspect_IPQC' THEN N'IPQC全检' 
                            WHEN 'Polling_IPQC' THEN N'IPQC巡检' 
                            WHEN 'Inspect_OQC' THEN N'OQC检测'
                            WHEN 'Inspect_Assemble' THEN N'组装检测'
                            WHEN 'Inspect_Assemble,Inspect_OQC' THEN N'组装&OQC检测'
                            END AS IsQAProcessName  
                            FROM one
                            JOIN dbo.System_Function_Plant B
                            ON one.System_FunPlant_UID = B.System_FunPlant_UID
                            )
                            SELECT * FROM two ORDER BY two.ItemNo";
            strSql = string.Format(strSql, masterUID, masterVersion);
            var list = DataContext.Database.SqlQuery<FlowChartDetailAndMGDataDTO>(strSql).ToList();
            return list;
        }

        public List<FlowChartDetailAndMGDataDTO> QueryExportDetailList(int masterUID, int masterVersion)
        {
            var strSql = @";WITH 
                        one AS
                        (
                        SELECT * FROM dbo.FlowChart_Detail
                        WHERE FlowChart_Master_UID={0} AND FlowChart_Version={1}
                        ),
                        two AS
                        (
                        SELECT one.*,B.FunPlant AS PlantName,
                        CASE one.IsQAProcess 
						WHEN 'Inspect_IPQC' THEN N'IPQC全检' 
						WHEN 'Polling_IPQC' THEN N'IPQC巡检' 
						WHEN 'Inspect_OQC' THEN N'OQC检测'
						WHEN 'Inspect_Assemble' THEN N'组装检测'
						WHEN 'Inspect_Assemble,Inspect_OQC' THEN N'组装&OQC检测'
						END AS IsQAProcessName  
                        FROM one
                        JOIN dbo.System_Function_Plant B
                        ON one.System_FunPlant_UID = B.System_FunPlant_UID
                        )
                        SELECT * FROM two ORDER BY two.Process_Seq";
            strSql = string.Format(strSql, masterUID, masterVersion);
            var list = DataContext.Database.SqlQuery<FlowChartDetailAndMGDataDTO>(strSql).ToList();
            var reworkList = list.Where(m => m.Rework_Flag == StructConstants.ReworkFlag.Rework).ToList();
            foreach (var reworkItem in reworkList)
            {

                var strIdList = reworkItem.RelatedRepairUID.Split(',').ToList();
                var intIdList = strIdList.Select<string, int>(x => Convert.ToInt32(x));
                var bindIdList = list.Where(m => intIdList.Contains(m.FlowChart_Detail_UID)).Select(m => m.Binding_Seq);
                reworkItem.RepairJoin = string.Join(",", bindIdList);
            }

            return list;
        }

        public List<FlowChartBomGet> QueryFLDetailByUIDAndVersion(int MasterUID, int Version, List<int> idList)
        {
            List<FlowChart_Detail> currentList = new List<FlowChart_Detail>();
            List<FlowChartBomGet> lastVersionList = new List<FlowChartBomGet>();

            var funPlantUIDList = DataContext.System_Function_Plant.Where(m => idList.Contains(m.FunPlant_OrganizationUID.Value)).Select(m => m.System_FunPlant_UID).ToList();

            //获取当前版本的FlowchartDetail信息
            currentList = DataContext.FlowChart_Detail.Where(m => m.FlowChart_Master_UID == MasterUID && m.FlowChart_Version == Version
            && funPlantUIDList.Contains(m.System_FunPlant_UID)).ToList();

            //根据用户所属的功能厂获取当前版本的FlowchartDetail信息
            //            if (idList.Count() > 0)
            //            {
            //                currentList = DataContext.FlowChart_Detail.Where(m => m.FlowChart_Master_UID == MasterUID && m.FlowChart_Version == Version
            //&& funPlantUIDList.Contains(m.System_FunPlant_UID)).ToList();
            //            }
            //            else
            //            {
            //                currentList = DataContext.FlowChart_Detail.Where(m => m.FlowChart_Master_UID == MasterUID && m.FlowChart_Version == Version).ToList();
            //            }


            var currentVersionList = AutoMapper.Mapper.Map<List<FlowChartBomGet>>(currentList);

            //获取上个版本的FLowchartDetail信息
            if (Version > 1)
            {
                //得到上个版本flowchartdetail绑定的物料员信息
                lastVersionList = QueryBomByFlowChartUID(MasterUID, Version - 1, funPlantUIDList).ToList();

                //循环当前版本将上个版本的物料员信息绑定到这个版本来
                foreach (var currentVersionItem in currentVersionList)
                {
                    var hasLastVersionItem = lastVersionList.Where(m => m.Binding_Seq == currentVersionItem.Binding_Seq).FirstOrDefault();
                    if (hasLastVersionItem != null)
                    {
                        currentVersionItem.User_NTID = hasLastVersionItem.User_NTID;
                        currentVersionItem.User_Name = hasLastVersionItem.User_Name;
                    }
                }
            }
            return currentVersionList;
        }

        public List<FunPlantVM> QueryFunPlant(int Flowchart_Master_UID)
        {
            List<FunPlantVM> result = new List<FunPlantVM>();
            try
            {
                string sql = string.Format(@"

                                SELECT DISTINCT
                                        SFP.FunPlant AS FunPlant ,
                                        SFP.System_FunPlant_UID AS System_FunPlant_UID
                                FROM    dbo.FlowChart_Detail Fd  WITH(NOLOCK)
                                        INNER JOIN dbo.FlowChart_Master FM WITH(NOLOCK)ON FM.FlowChart_Master_UID = Fd.FlowChart_Master_UID AND FM.FlowChart_Version = Fd.FlowChart_Version
                                        INNER JOIN dbo.System_Function_Plant SFP WITH(NOLOCK) ON SFP.System_FunPlant_UID = FD.System_FunPlant_UID
                                WHERE   Fd.FlowChart_Master_UID = {0}", Flowchart_Master_UID);

                var funplant = DataContext.Database.SqlQuery<FunPlantVM>(sql).ToList();
                if (funplant.Count != 0)
                {
                    result = funplant;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }


        /// <summary>
        /// 根据 FlowChar_detial_UID获取WIP值
        /// </summary>
        /// <param name="flId"></param>
        /// <returns></returns>
        public int GetWIPValueByFLID(int flId)
        {
            var strSQl = $"SELECT WIP_QTY FROM [dbo].[FlowChart_Detail] where FlowChart_Detail_UID={flId}";
            var WIP_QTY = DataContext.Database.SqlQuery<int>(strSQl);
            if (WIP_QTY.Any())
            {
                return Convert.ToInt32(WIP_QTY.First());
            }
            return 0;
        }

        /// <summary>
        /// 获取临近制程的后一个制程
        /// </summary>
        /// <returns></returns>
        public FlowChart_Detail GetNextProcess_Seq(int flowChartMasterID, int flowChart_Version, int process_Seq, string color)
        {
            var sql = @"SELECT TOP 1 [FlowChart_Detail_UID]
                              ,[FlowChart_Master_UID]
                              ,[System_FunPlant_UID]
                              ,[Process_Seq]
                              ,[DRI]
                              ,[Place]
                              ,[Process]
                              ,[Product_Stage]
                              ,[Color]
                              ,[Process_Desc]
                              ,[Material_No]
                              ,[FlowChart_Version]
                              ,[FlowChart_Version_Comment]
                              ,[Modified_UID]
                              ,[Modified_Date]
                              ,[FatherProcess_UID]
                              ,[IsQAProcess]
                              ,[Rework_Flag]
                              ,[WIP_QTY]
                              ,[Binding_Seq]
                              ,[BeginTime]
                              ,[EndTime]
                              ,[ToWHSOK]
                              ,[ToWHSNG]
                              ,[ItemNo]
                              ,[Edition]
                              ,[FromWHS]
                              ,[RelatedRepairUID]
                              ,[Location_Flag]
                              ,[Current_WH_QTY]
                              ,[NullWip]
                              ,[Data_Source]
                              ,[Is_Synchronous]
                          FROM [PDMS_Test].[dbo].[FlowChart_Detail]
                          WHERE ";
            var sqlWhere = $"FlowChart_Master_UID={flowChartMasterID} and FlowChart_Version={flowChart_Version} AND Color=N'{color}' and Process_Seq>{process_Seq} ORDER BY Process_Seq ASC";

            var detailModel = DataContext.Database.SqlQuery<FlowChart_Detail>(sql + sqlWhere).ToList().FirstOrDefault();
            return detailModel;
        }

        /// <summary>
        /// 获取临近制程的前一个制程
        /// </summary>
        /// <returns></returns>
        public FlowChart_Detail GetProProcess_Seq(int flowChartMasterID, int flowChart_Version, int process_Seq, string color)
        {
            var sql = @"SELECT TOP 1 [FlowChart_Detail_UID]
                              ,[FlowChart_Master_UID]
                              ,[System_FunPlant_UID]
                              ,[Process_Seq]
                              ,[DRI]
                              ,[Place]
                              ,[Process]
                              ,[Product_Stage]
                              ,[Color]
                              ,[Process_Desc]
                              ,[Material_No]
                              ,[FlowChart_Version]
                              ,[FlowChart_Version_Comment]
                              ,[Modified_UID]
                              ,[Modified_Date]
                              ,[FatherProcess_UID]
                              ,[IsQAProcess]
                              ,[Rework_Flag]
                              ,[WIP_QTY]
                              ,[Binding_Seq]
                              ,[BeginTime]
                              ,[EndTime]
                              ,[ToWHSOK]
                              ,[ToWHSNG]
                              ,[ItemNo]
                              ,[Edition]
                              ,[FromWHS]
                              ,[RelatedRepairUID]
                              ,[Location_Flag]
                              ,[Current_WH_QTY]
                              ,[NullWip]
                              ,[Data_Source]
                              ,[Is_Synchronous]
                          FROM [PDMS_Test].[dbo].[FlowChart_Detail]
                          WHERE ";
            var sqlWhere = $"FlowChart_Master_UID={flowChartMasterID} and FlowChart_Version={flowChart_Version} AND Color=N'{color}' and Process_Seq<{process_Seq} ORDER BY Process_Seq DESC";
            var detailModel = DataContext.Database.SqlQuery<FlowChart_Detail>(sql + sqlWhere).ToList().FirstOrDefault();
            return detailModel;
        }
    }

}
