
using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Constants;
using System.Data.Entity.SqlServer;
using System.Text;
using System.Data.Entity.Infrastructure;
using PDMS.Model.ViewModels.ProductionPlanning;
using System.Data;
using PDMS.Common.Helpers;
using System.Transactions;

namespace PDMS.Data.Repository
{
    public interface IFlowChartMasterRepository : IRepository<FlowChart_Master>
    {
        List<FlowChartModelGet> GetFlowchartMEList(FlowChartModelSearch search, Page page, out int count);
        List<FlowChartModelGet> GetFlowchartPPList(FlowChartModelSearch search, Page page, out int count);
        IQueryable<string> QueryDistinctPartTypes(string customer, string project, string productphase);
        IQueryable<string> QueryDistinctPartTypesAPP(string project, string productphase);
        IQueryable<FlowChartMasterDTO> QueryProjectTypes(string project);
        IQueryable<FlowChartModelGet> QueryFlowCharts(FlowChartModelSearch search, Page page, out int count);
        IQueryable<FlowChart_Detail> QueryFLDetailList(int id, int Version, out int count);
        IQueryable<FlowChart_Detail> QueryFLDetailWUXI_MList(int id, int Version, out int count);
        IQueryable<FlowChart_Master> QueryFLList(int id);

        List<FlowChartPlanManagerVM> QueryFlowMGData(int masterUID, DateTime date, out int count);
        List<IEPlanManagerVM> QueryFlowIEMGData(int masterUID, DateTime date, out int count);
        IQueryable<PrjectListVM> QueryProjectList(int user_account_uid, List<int> OPType_OrganizationUID, bool MHFlag_MulitProject);
        IQueryable<ProcessDataSearch> QueryFlowChartDataByMasterUid(int flowChartMaster_uid);

        void UpdateFolowCharts(FlowChartImport importItem, int accountID);
        void UpdateFolowChartsWUXI_M(FlowChartImport importItem, int accountID);
        FlowChartPlanManagerVM QueryFlowMGDataSingle(int masterUID, DateTime date);
        IEPlanManagerVM QueryFlowIEMGDataSingle(int masterUID, DateTime date, int shiftTimeId);
        IEPlanManagerVM QueryFlowIEMGDataSingle1(int masterUID, DateTime date);
        int getTheLastVersionDetailUID(int uid);
        IQueryable<FlowChart_MgData> UpdatePlan(int detailUID, DateTime date);
        IQueryable<FlowChart_IEData> UpdateIEPlan(int detailUID, DateTime date,int shiftTimeId);
        void BatchImportPlan(List<FlowChartMgDataDTO> mgDataList, int FlowChart_Master_UID);
       
        void BatchIEImportPlan(List<FlowChartIEMgDataDTO> mgDataList, int FlowChart_Master_UID);
        List<EQP_UserTable> GetByUserId(string User_id);
        void ExecSPTemp(FlowChartImport importItem);
        void ExecProjectSP(int projectId);
        List<FlowChartHistoryGet> QueryHistoryVersion(int FlowChart_Master_UID);
        void ImportFlowchartME(ProductionPlanningModelGetAPIModel importItem, bool isEdit, int accountID);
        #region ----排程排线

        string JudgeFlowchart(int FlowChart_Master_UID);

        #endregion
        int getFlowchartMaxUID(int master_UID, int FlowChart_Version, int Process_Seq, string color);
        int getFlowchartBuildingCount(int master_UID, int FlowChart_Version, int Process_Seq, string color);
        Product_Input getRealtedProduct(int flowchartMasterUid, int flowChart_Version, int Process_Seq, string color, DateTime ProductDate, string TimeInterVal);
        List<Product_Input_Location> getDetailBuildingList(int master_UID, int FlowChart_Version, int Process_Seq, string color, DateTime productDate, string TimeInterval);
        void ImportFlowChart(FlowChartImport importItem, int accountID);
        int GetWIPValueByFLID(int flId);

        List<WIPAlterDetialModel> GetWIPAlterRecordDetialData(WIPDetialSearchParam searchModel, Page page);

        int GetFlowChartMasterID(string BU_D_Name, string Project_Name, string Part_Types, string Product_Phase);
        
    }
    public class FlowChartMasterRepository : RepositoryBase<FlowChart_Master>, IFlowChartMasterRepository
    {
        private Logger log = new Logger("FlowChartMasterRepository");
        public FlowChartMasterRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        public int getTheLastVersionDetailUID(int uid)
        {
            var query = from d in DataContext.FlowChart_Detail
                        where d.FlowChart_Detail_UID == uid
                        select d;
            var item = query.FirstOrDefault();
            if (item.FlowChart_Version == 1)
                return 0;
            var query1 = from d in DataContext.FlowChart_Detail
                         join master in DataContext.FlowChart_Master
on d.FlowChart_Master_UID equals master.FlowChart_Master_UID
                         where d.FlowChart_Master_UID == item.FlowChart_Master_UID && d.FlowChart_Version == item.FlowChart_Version - 1
                         && d.Process == item.Process
                         select d.FlowChart_Detail_UID;
            if (query1.Count() > 0)
                return query1.FirstOrDefault();
            else return 0;
        }

        public int getFlowchartMaxUID(int master_UID, int FlowChart_Version, int Process_Seq, string color)
        {
            var query = from detail in DataContext.FlowChart_Detail
                        where detail.Process_Seq == Process_Seq && detail.FlowChart_Master_UID == master_UID && detail.Color == color && FlowChart_Version == detail.FlowChart_Version
                        orderby detail.FlowChart_Detail_UID descending
                        select detail.FlowChart_Detail_UID

                        ;
            return query.FirstOrDefault();
        }
        public int getFlowchartBuildingCount(int master_UID, int FlowChart_Version, int Process_Seq, string color)
        {
            var query = from detail in DataContext.FlowChart_Detail
                        where detail.Process_Seq == Process_Seq && detail.FlowChart_Master_UID == master_UID && detail.Color == color && FlowChart_Version == detail.FlowChart_Version
                        select detail.Place;
            return query.Count();
        }
        public List<Product_Input_Location> getDetailBuildingList(int master_UID, int FlowChart_Version, int Process_Seq, string color, DateTime productDate, string TimeInterval)
        {

            //    string sql = @" SELECT * FROM dbo.Product_Input_Location WHERE Product_Date={1} AND
            //Time_Interval={2} AND FlowChart_Master_UID={3} AND FlowChart_Version={4} 
            //          AND  Process_Seq={5} AND Color =N{6}";
            //    sql = string.Format(sql, productDate.ToShortDateString(), TimeInterval , master_UID , FlowChart_Version,Process_Seq ,color );
            //    var dblist = DataContext.Database.SqlQuery<Product_Input_Location>(sql).ToList();
            //    return dblist;

            var query = from detail in DataContext.Product_Input_Location
                        where detail.Process_Seq == Process_Seq && detail.FlowChart_Master_UID == master_UID && detail.Color == color && detail.FlowChart_Version == FlowChart_Version
                        && detail.Product_Date == productDate && detail.Time_Interval == TimeInterval
                        select detail;
            return query.ToList();
        }
        public IQueryable<FlowChartMasterDTO> QueryProjectTypes(string project)
        {
            var query_parttypes = from flowmaster in DataContext.FlowChart_Master
                                  join p in DataContext.System_Project on flowmaster.Project_UID equals p.Project_UID
                                  where flowmaster.Is_Closed == false && p.Project_Name == project
                                  select new FlowChartMasterDTO
                                  {
                                      FlowChart_Master_UID = flowmaster.FlowChart_Master_UID,
                                      Part_Types = flowmaster.Part_Types
                                  };
            return query_parttypes.Distinct();
        }

        public IQueryable<string> QueryDistinctPartTypes(string customername, string projectname, string productphasename)
        {
            var query = from bud in DataContext.System_BU_D
                        join project in DataContext.System_Project on bud.BU_D_UID equals project.BU_D_UID
                        where (bud.BU_D_Name == customername && project.Project_Name == projectname)
                        select (project.Project_UID);
            var project_uid = query.FirstOrDefault();
            var query_parttypes = from flowmaster in DataContext.FlowChart_Master
                                  where (flowmaster.Project_UID == project_uid && flowmaster.Is_Closed == false)
                                  select (flowmaster.Part_Types);
            return query_parttypes.Distinct();
        }

        public IQueryable<string> QueryDistinctPartTypesAPP(string projectname, string productphasename)
        {
            var query = from project in DataContext.System_Project
                        where (project.Project_Name == projectname)
                        select (project.Project_UID);
            var project_uid = query.FirstOrDefault();
            var query_parttypes = from flowmaster in DataContext.FlowChart_Master
                                  where (flowmaster.Project_UID == project_uid)
                                  select (flowmaster.Part_Types);
            return query_parttypes.Distinct();
        }

        public IQueryable<FlowChartModelGet> QueryFlowCharts(FlowChartModelSearch search, Page page, out int count)
        {
            var query = (from M in DataContext.FlowChart_Master
                         join P in DataContext.System_Project
                         on M.Project_UID equals P.Project_UID
                         join BUD in DataContext.System_BU_D
                         on P.BU_D_UID equals BUD.BU_D_UID
                         join U in DataContext.System_Users
                         on M.Modified_UID equals U.Account_UID
                         // where M.Is_Closed == false
                         select new FlowChartModelGet
                         {
                             FlowChart_Master_UID = M.FlowChart_Master_UID,
                             BU_D_Name = BUD.BU_D_Name,
                             Project_Name = P.Project_Name,
                             Part_Types = M.Part_Types,
                             Product_Phase = P.Product_Phase,
                             Is_Closed = M.Is_Closed,
                             Is_Latest = M.Is_Latest,
                             FlowChart_Version = M.FlowChart_Version,
                             FlowChart_Version_Comment = M.FlowChart_Version_Comment,
                             User_Name = U.User_Name,
                             Modified_Date = M.Modified_Date,
                             User_NTID = U.User_NTID,
                             IsTemp = false,
                             OP_type = P.OP_TYPES,
                             Organization_UID = P.Organization_UID,
                             Project_UID = P.Project_UID,
                             CurrentDepartent = M.CurrentDepartent
                         });

            //如果用户是超级管理员
            var isAdmin = search.RoleList.Exists(m => m.Role_UID == 1);
            //如果是超级管理员并且UserOrg表里面没有数据就是厂区什么的都没设，那就什么都不显示
            if (!isAdmin)
            {
                //根据用户所属的Organization_UID查询对应的专案
                query = query.Where(m => search.OPType_OrganizationUIDList.Contains(m.Organization_UID));
            }

            if (!string.IsNullOrEmpty(search.CurrentDepartent) && search.CurrentDepartent != "PP")
            {
                query = query.Where(m => m.CurrentDepartent == search.CurrentDepartent);
            }
            else
            {
                query = query.Where(m => m.CurrentDepartent == "PP" || m.CurrentDepartent == null);
            }

            //如果ProjectUIDList有值则查询，没有值则带出所有专案
            if (search.ProjectUIDList.Count() > 0)
            {
                query = query.Where(m => search.ProjectUIDList.Contains(m.Project_UID));
            }

            if (!string.IsNullOrWhiteSpace(search.BU_D_Name))
            {
                query = query.Where(m => m.BU_D_Name.Contains(search.BU_D_Name));
            }
            if (!string.IsNullOrWhiteSpace(search.Project_Name))
            {
                query = query.Where(m => m.Project_Name.Contains(search.Project_Name));
            }
            if (!string.IsNullOrWhiteSpace(search.Part_Types))
            {
                query = query.Where(m => m.Part_Types.Contains(search.Part_Types));
            }
            if (!string.IsNullOrWhiteSpace(search.Product_Phase))
            {
                query = query.Where(m => m.Product_Phase.Contains(search.Product_Phase));
            }
            switch (search.Is_Closed)
            {
                case StructConstants.IsClosedStatus.ClosedKey:
                    query = query.Where(m => m.Is_Closed == true && m.IsTemp == false);
                    break;
                case StructConstants.IsClosedStatus.ProcessKey:
                    query = query.Where(m => m.Is_Closed == false && m.IsTemp == false);
                    break;
                case StructConstants.IsClosedStatus.ApproveKey:
                    query = query.Where(m => m.IsTemp == true);
                    break;
            }
            if (search.Is_Latest == StructConstants.IsLastestStatus.LastestKey)
            {
                query = query.Where(m => m.Is_Latest == true);
            }
            if (search.Modified_Date_From != null)
            {
                query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_From) <= 0);
            }
            if (search.Modified_Date_End != null)
            {
                query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_End) >= 0);
            }
            if (search.Modified_By != null)
            {
                query = query.Where(m => m.User_NTID == search.Modified_By);
            }



            count = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Project_Name).GetPage(page);
            return query;
        }

        public IQueryable<FlowChart_Detail> QueryFLDetailList(int id, int Version, out int count)
        {
            var flMasterItem = DataContext.FlowChart_Master.Find(id);
            var flDetailQuery = from p in DataContext.FlowChart_Detail
                                where p.FlowChart_Master_UID == id && p.FlowChart_Version == Version
                                select p;

            count = flDetailQuery.Count();
            return flDetailQuery.OrderBy(m => m.Process_Seq);
        }

        public IQueryable<FlowChart_Detail> QueryFLDetailWUXI_MList(int id, int Version, out int count)
        {
            var flMasterItem = DataContext.FlowChart_Master.Find(id);
            var flDetailQuery = from p in DataContext.FlowChart_Detail.Include("System_Function_Plant").Include("System_Users")
                                where p.FlowChart_Master_UID == id && p.FlowChart_Version == Version
                                select p;

            count = flDetailQuery.Count();
            return flDetailQuery.OrderBy(m => m.Process_Seq);
        }



        public IQueryable<FlowChart_Master> QueryFLList(int id)
        {
            var flMasterItem = DataContext.FlowChart_Master.Find(id);
            var query = from P in DataContext.FlowChart_Master
                        where P.Project_UID == flMasterItem.Project_UID && P.Part_Types == flMasterItem.Part_Types
                        select P;
            return query;
        }

        public IQueryable<PrjectListVM> QueryProjectList(int user_account_uid, List<int> OPType_OrganizationUID, bool MHFlag_MulitProject)
        {
            //判断此帐号是否物料员，如果是物料员则不需要OPType_OrganizationUID
            if (MHFlag_MulitProject)
            {
                //获取物料员的专案信息
                var flIdAndVersionList = DataContext.FlowChart_PC_MH_Relationship.Where(m => m.MH_UID == user_account_uid && !m.FlowChart_Detail.FlowChart_Master.Is_Closed)
                    .Select(m => new { m.FlowChart_Detail.FlowChart_Master_UID, m.FlowChart_Detail.FlowChart_Version }).Distinct().ToList();


                var uidList = flIdAndVersionList.Select(m => m.FlowChart_Master_UID).ToList();
                var versionList = flIdAndVersionList.Select(m => m.FlowChart_Version).ToList();

                var query = (from flowchartMaster in DataContext.FlowChart_Master
                             join project in DataContext.System_Project on flowchartMaster.Project_UID equals project.Project_UID
                             join BUD in DataContext.System_BU_D on project.BU_D_UID equals BUD.BU_D_UID
                             where flowchartMaster.Is_Closed != true && uidList.Contains(flowchartMaster.FlowChart_Master_UID)
                             && versionList.Contains(flowchartMaster.FlowChart_Version)
                             select new PrjectListVM
                             {
                                 FlowChartMaster_Uid = flowchartMaster.FlowChart_Master_UID,
                                 Part_Types = flowchartMaster.Part_Types,
                                 Product_Phase = project.Product_Phase,
                                 Project = project.Project_Name,
                                 Customer = BUD.BU_D_Name
                             }).Distinct();

                return query.OrderBy(o => o.Project).OrderBy(o => o.Product_Phase);
            }
            else
            {
                var query = (from flowchartMaster in DataContext.FlowChart_Master
                             join project in DataContext.System_Project on flowchartMaster.Project_UID equals project.Project_UID
                             join BUD in DataContext.System_BU_D on project.BU_D_UID equals BUD.BU_D_UID
                             where flowchartMaster.Is_Closed != true && OPType_OrganizationUID.Contains(project.Organization_UID)
                             select new PrjectListVM
                             {
                                 FlowChartMaster_Uid = flowchartMaster.FlowChart_Master_UID,
                                 Part_Types = flowchartMaster.Part_Types,
                                 Product_Phase = project.Product_Phase,
                                 Project = project.Project_Name,
                                 Customer = BUD.BU_D_Name
                             }).Distinct();
                return query.OrderBy(o => o.Project).OrderBy(o => o.Product_Phase);
            }
        }

        public IQueryable<ProcessDataSearch> QueryFlowChartDataByMasterUid(int flowChartMaster_uid)
        {
            var query = from flowchartMaster in DataContext.FlowChart_Master
                        join project in DataContext.System_Project on flowchartMaster.Project_UID equals project.Project_UID
                        join BUD in DataContext.System_BU_D on project.BU_D_UID equals BUD.BU_D_UID
                        join detail in DataContext.FlowChart_Detail on flowchartMaster.FlowChart_Master_UID equals detail.FlowChart_Master_UID
                        where flowchartMaster.FlowChart_Master_UID == flowChartMaster_uid && flowchartMaster.Is_Closed != true
                        && flowchartMaster.FlowChart_Version == detail.FlowChart_Version
                        select new ProcessDataSearch
                        {
                            Part_Types = flowchartMaster.Part_Types,
                            Product_Phase = project.Product_Phase,
                            Project = project.Project_Name,
                            Customer = BUD.BU_D_Name,

                            Func_Plant = detail.System_Function_Plant.FunPlant,
                            FlowChart_Master_UID = flowchartMaster.FlowChart_Master_UID,
                            FlowChart_Version = flowchartMaster.FlowChart_Version
                        };

            return query;
        }


        public void ImportFlowChart(FlowChartImport importItem, int accountID)
        {
            using (var trans = DataContext.Database.BeginTransaction())
            {
                importItem.FlowChartMasterDTO.CurrentDepartent = "PP";

                string sql = @"INSERT INTO dbo.FlowChart_Master
                                    ( Project_UID ,
                                      Part_Types ,
                                      FlowChart_Version ,
                                      FlowChart_Version_Comment ,
                                      Is_Latest ,
                                      Is_Closed ,
                                      Modified_UID ,
                                      Modified_Date ,
                                      Organization_UID ,
                                      Product_Phase ,
                                      CurrentDepartent ,
                                      Statue_IE ,
                                      Created_UID ,
                                      Created_Date
                                    )
                            VALUES  ( {0} , -- Project_UID - int
                                      N'{1}' , -- Part_Types - nvarchar(50)
                                      {2} , -- FlowChart_Version - int
                                      N'{3}' , -- FlowChart_Version_Comment - nvarchar(200)
                                      {4} , -- Is_Latest - bit
                                      {5} , -- Is_Closed - bit
                                      {6} , -- Modified_UID - int
                                      GETDATE() , -- Modified_Date - datetime
                                      {7} , -- Organization_UID - int
                                      N'{8}' , -- Product_Phase - nvarchar(10)
                                      N'{9}' , -- CurrentDepartent - nvarchar(10)
                                      {10} , -- Statue_IE - int
                                      {6} , -- Created_UID - int
                                      GETDATE()  -- Created_Date - datetime
                                    )";
                sql = string.Format(sql,
                    importItem.FlowChartMasterDTO.Project_UID,
                    importItem.FlowChartMasterDTO.Part_Types,
                    importItem.FlowChartMasterDTO.FlowChart_Version,
                    importItem.FlowChartMasterDTO.FlowChart_Version_Comment,
                    importItem.FlowChartMasterDTO.Is_Latest ? 1 : 0,
                    importItem.FlowChartMasterDTO.Is_Closed ? 1 : 0,
                    accountID,
                    importItem.FlowChartMasterDTO.Organization_UID,
                    importItem.FlowChartMasterDTO.Product_Phase,
                    importItem.FlowChartMasterDTO.CurrentDepartent,
                    importItem.FlowChartMasterDTO.Statue_IE);

                DataContext.Database.ExecuteSqlCommand(sql);

                var suidSql = "SELECT  SCOPE_IDENTITY();";
                var flMasterUID = Convert.ToInt32(DataContext.Database.SqlQuery<decimal>(suidSql).First());


                List<FLUIDAndBindSeq> FlSeqList = new List<FLUIDAndBindSeq>();
                foreach (var detailDTOItem in importItem.FlowChartImportDetailDTOList)
                {
                    //var detailItem = AutoMapper.Mapper.Map<FlowChart_Detail>(detailDTOItem.FlowChartDetailDTO);
                    //detailItem.EndTime = null;
                    //flMasterItem.FlowChart_Detail.Add(detailItem);

                    detailDTOItem.FlowChartDetailDTO.FlowChart_Master_UID = flMasterUID;

                    //下面这段为RelatedRepairUID赋值的语句一定要放在insert的前面
                    if (detailDTOItem.FlowChartDetailDTO.Rework_Flag == StructConstants.ReworkFlag.Rework)
                    {
                        //转换为uid列表
                        ConvertBindSeq(detailDTOItem.FlowChartDetailDTO, FlSeqList);
                    }
                    var insertFlDetailSql = InsertFLDetailSql(detailDTOItem.FlowChartDetailDTO);
                    DataContext.Database.ExecuteSqlCommand(insertFlDetailSql);

                    var uidSql = "SELECT  SCOPE_IDENTITY();";
                    var detailUID = DataContext.Database.SqlQuery<decimal>(uidSql).First();
                    if (detailDTOItem.FlowChartDetailDTO.Rework_Flag == StructConstants.ReworkFlag.Repair)
                    {
                        var fuid = Convert.ToInt32(detailUID);
                        FlSeqList.Add(new FLUIDAndBindSeq
                        {
                            FlowChart_Detail_UID = fuid,
                            Binding_Seq = detailDTOItem.FlowChartDetailDTO.Binding_Seq
                        });

                    }
                }
                trans.Commit();
            }
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
            return Convert.ToInt32(WIP_QTY);
        }

        public List<WIPAlterDetialModel> GetWIPAlterRecordDetialData(WIPDetialSearchParam searchModel, Page page)
        {
            //var OpType = new SqlParameter("@OpType", searchModel.OpType);
            //var Project = new SqlParameter("@Project", searchModel.Project);
            //var Process = new SqlParameter("@Process", searchModel.Process);
            //var DayDate = new SqlParameter("@DayDate", searchModel.DayDate);
            //var AlterPerson = new SqlParameter("@AlterPerson", searchModel.AlterPerson);
            //var Color = new SqlParameter("@Color", searchModel.Color);
            StringBuilder sb = new StringBuilder();
            // 1 厂区
            if (!string.IsNullOrEmpty(searchModel.factoryAddress))
            {
                string factoryAddress = $" AND tempOrg.Organization_Name=N'{searchModel.factoryAddress}'";
                sb.AppendLine(factoryAddress);
            }
            // 2 OP类型
            if (!string.IsNullOrEmpty(searchModel.OpType))
            {
                string OpType = $" AND  temp.OP_Types=N'{searchModel.OpType}'";
                sb.AppendLine(OpType);
            }

            // 3 专案
            if (!string.IsNullOrEmpty(searchModel.Project))
            {
                string Project = $" AND  temp.Project_Name =N'{searchModel.Project}'";
                sb.AppendLine(Project);
            }

            // 4 部件类型
            if (!string.IsNullOrEmpty(searchModel.partType))
            {
                string partType = $" AND temp.Part_Types=N'{searchModel.partType}'";
                sb.AppendLine(partType);
            }

            // 5 功能厂
            if (!string.IsNullOrEmpty(searchModel.FunPlant))
            {
                string partType = $" AND temp.FunPlant=N'{searchModel.FunPlant}'";
                sb.AppendLine(partType);
            }

            //6 修改时间
            if (searchModel.StartDate.ToString("yyyy-MM-dd") == "0001-01-01")
            {
            }
            else
            {
                var stratTime = searchModel.StartDate.ToString("yyyy-MM-dd 00:00:00.000");
                sb.AppendLine($" AND temp.Modified_Date>='{stratTime}'");

            }

            if (searchModel.StartDate.ToString("yyyy-MM-dd") == "0001-01-01")
            {
            }
            else
            {
                var endTime = searchModel.EndDate.ToString("yyyy-MM-dd 00:00:00.000");
                sb.AppendLine($" AND temp.Modified_Date<='{endTime}'");
            }

            ////制程
            //if (!string.IsNullOrEmpty(searchModel.Process))
            //{
            //    string Process = $" AND temp.Process={searchModel.Process}";
            //    sb.AppendLine(Process);
            //}

            ////颜色
            //if (!string.IsNullOrEmpty(searchModel.Color))
            //{
            //    string Color = $" AND  temp.Color={searchModel.Color}";
            //    sb.AppendLine(Color);
            //}

            ////楼栋
            //if (!string.IsNullOrEmpty(searchModel.Color))
            //{
            //    string Color = $" AND  temp.Color={searchModel.Color}";
            //    sb.AppendLine(Color);
            //}

            ////修改人
            //if (!string.IsNullOrEmpty(searchModel.AlterPerson))
            //{
            //    string AlterPerson = $" AND temp.Modified_UID={searchModel.AlterPerson}";
            //    sb.AppendLine(AlterPerson);
            //}

            //FlowCchartDetial
            string strSQLFlowCchartDetial = GetWIPAlterDetialSql();

            var strSQLFCDParam = strSQLFlowCchartDetial + sb.ToString();

            var resultFCDList = DataContext.Database.SqlQuery<WIPAlterDetialModel>(strSQLFCDParam).ToList();

            //PPCkeck
            string strSQLPPCheck = GetGetWIPAlterDetialSqlForPPCkeck();

            var strSQLPPC = strSQLPPCheck + sb.ToString();

            var resultPPCList = DataContext.Database.SqlQuery<WIPAlterDetialModel>(strSQLPPC).ToList();

            List<WIPAlterDetialModel> result = new List<WIPAlterDetialModel>();
            result.AddRange(resultFCDList);
            result.AddRange(resultPPCList);

            //result.Skip(page.Skip).Take(page.PageSize);
            return result;
        }



        public void UpdateFolowCharts(FlowChartImport importItem, int accountID)
        {
            using (var trans = DataContext.Database.BeginTransaction())
            {
                var oldMasterFLItem = DataContext.FlowChart_Master.Find(importItem.FlowChartMasterDTO.FlowChart_Master_UID);
                //更新老版本的Detail表的EndTime为当前时间
                string updateTimeSql = "UPDATE dbo.FlowChart_Detail SET EndTime=GETDATE() WHERE FlowChart_Master_UID=" + oldMasterFLItem.FlowChart_Master_UID + " AND FlowChart_Version=" + oldMasterFLItem.FlowChart_Version;
                DataContext.Database.ExecuteSqlCommand(updateTimeSql);

                //更新FLowchartMaster数据
                string updateMasterSql = @"UPDATE dbo.FlowChart_Master SET FlowChart_Version={0},FlowChart_Version_Comment='{1}',Modified_Date='{2}',Modified_UID={3} WHERE FlowChart_Master_UID={4}";
                updateMasterSql = string.Format(updateMasterSql, oldMasterFLItem.FlowChart_Version + 1, importItem.FlowChartMasterDTO.FlowChart_Version_Comment, DateTime.Now.ToString(FormatConstants.DateTimeFormatString), importItem.FlowChartMasterDTO.Modified_UID, importItem.FlowChartMasterDTO.FlowChart_Master_UID);
                DataContext.Database.ExecuteSqlCommand(updateMasterSql);

                List<FLUIDAndBindSeq> FlSeqList = new List<FLUIDAndBindSeq>();
                foreach (var detailDTOItem in importItem.FlowChartImportDetailDTOList)
                {
                    //var detailItem = AutoMapper.Mapper.Map<FlowChart_Detail>(detailDTOItem.FlowChartDetailDTO);

                    //下面这段为RelatedRepairUID赋值的语句一定要放在insert的前面
                    if (detailDTOItem.FlowChartDetailDTO.Rework_Flag == StructConstants.ReworkFlag.Rework)
                    {
                        //转换为uid列表
                        ConvertBindSeq(detailDTOItem.FlowChartDetailDTO, FlSeqList);
                    }
                    var insertFlDetailSql = InsertFLDetailSql(detailDTOItem.FlowChartDetailDTO);
                    DataContext.Database.ExecuteSqlCommand(insertFlDetailSql);

                    var uidSql = "SELECT  SCOPE_IDENTITY();";
                    var detailUID = DataContext.Database.SqlQuery<decimal>(uidSql).First();
                    if (detailDTOItem.FlowChartDetailDTO.Rework_Flag == StructConstants.ReworkFlag.Repair)
                    {
                        var fuid = Convert.ToInt32(detailUID);
                        FlSeqList.Add(new FLUIDAndBindSeq
                        {
                            FlowChart_Detail_UID = fuid,
                            Binding_Seq = detailDTOItem.FlowChartDetailDTO.Binding_Seq
                        });
                    }



                    //插入生产计划表
                    if (detailDTOItem.MgDataList != null && detailDTOItem.MgDataList.Count() > 0)
                    {
                        var insertMgDataSql = InsertFLowcharMGDataSql(detailDTOItem.MgDataList, detailUID);
                        DataContext.Database.ExecuteSqlCommand(insertMgDataSql.ToString());
                    }

                    //将老版本的绑定物料员数据插入到新版本中去
                    if (detailDTOItem.PCMHList != null && detailDTOItem.PCMHList.Count() > 0)
                    {
                        //物料员跟FlDetail关系为1对1，但是这里因为detailItem.FlowChart_PC_MH_Relationship为集合类型不得已将pcList转为list
                        var insertPCSql = InsertPCMHSql(detailDTOItem.PCMHList, detailUID);
                        DataContext.Database.ExecuteSqlCommand(insertPCSql);
                    }

                    //----------Destiny需要更新的六张表    start----------------
                    if (detailDTOItem.FlowChartDetailDTO.Old_FlowChart_Detail_UID > 0 && importItem.Site == StructConstants.Site.CTU)
                    {
                        var updateQAAndOqCSql = UpdateQAAndOQCSql(detailDTOItem.FlowChartDetailDTO.Old_FlowChart_Detail_UID, detailUID, accountID);
                        DataContext.Database.ExecuteSqlCommand(updateQAAndOqCSql);
                    }
                    //----------Destiny需要更新的六张表    end----------------
                }

                //var reworkDetailList = importItem.FlowChartImportDetailDTOList.Where(m => m.FlowChartDetailDTO.Rework_Flag == StructConstants.ReworkFlag.Rework).ToList();
                //foreach (var reworkDetailItem in reworkDetailList)
                //{
                //    //转换为uid列表
                //    ConvertBindSeq(reworkDetailItem.FlowChartDetailDTO, FlSeqList);

                //}



                trans.Commit();
            }



        }

        public void ConvertBindSeq(FlowChartDetailDTO dto, List<FLUIDAndBindSeq> FlSeqList)
        {
            //这里分割出来的是BindingSeq的序号
            var strIdList = dto.RelatedRepairBindingSeq.Split(',').ToList();
            //将list<string>转换为list<int>
            var intIdList = strIdList.Select<string, int>(x => Convert.ToInt32(x));
            //设定FlowchartDetailUID的主键字符串
            List<int> uidList = new List<int>();

            foreach (var item in intIdList)
            {
                var uid = FlSeqList.Where(m => m.Binding_Seq == item).Select(m => m.FlowChart_Detail_UID).First();
                uidList.Add(uid);
            }

            dto.RelatedRepairUID = string.Join(",", uidList);
        }

        public void UpdateFolowChartsWUXI_M(FlowChartImport importItem, int accountID)
        {
            using (var trans = DataContext.Database.BeginTransaction())
            {
                var oldMasterFLItem = DataContext.FlowChart_Master.Find(importItem.FlowChartMasterDTO.FlowChart_Master_UID);
                //更新老版本的Detail表的EndTime为当前时间
                string updateTimeSql = "UPDATE dbo.FlowChart_Detail SET EndTime=GETDATE() WHERE FlowChart_Master_UID=" + oldMasterFLItem.FlowChart_Master_UID + " AND FlowChart_Version=" + oldMasterFLItem.FlowChart_Version;
                DataContext.Database.ExecuteSqlCommand(updateTimeSql);

                //更新FLowchartMaster数据
                string updateMasterSql = @"UPDATE dbo.FlowChart_Master SET FlowChart_Version={0},FlowChart_Version_Comment='{1}',Modified_Date='{2}',Modified_UID={3} WHERE FlowChart_Master_UID={4}";
                updateMasterSql = string.Format(updateMasterSql, oldMasterFLItem.FlowChart_Version + 1, importItem.FlowChartMasterDTO.FlowChart_Version_Comment, DateTime.Now.ToString(FormatConstants.DateTimeFormatString), importItem.FlowChartMasterDTO.Modified_UID, importItem.FlowChartMasterDTO.FlowChart_Master_UID);
                DataContext.Database.ExecuteSqlCommand(updateMasterSql);

                foreach (var detailDTOItem in importItem.FlowChartImportDetailDTOList)
                {
                    var detailItem = AutoMapper.Mapper.Map<FlowChart_Detail>(detailDTOItem.FlowChartDetailDTO);
                    //---跟成都厂区的区别
                    var insertFlDetailSql = InsertFLDetailWUXI_MSql(detailDTOItem.FlowChartDetailDTO);
                    DataContext.Database.ExecuteSqlCommand(insertFlDetailSql);
                    var uidSql = "SELECT  SCOPE_IDENTITY();";
                    var detailUID = DataContext.Database.SqlQuery<decimal>(uidSql).First();

                    //插入生产计划表
                    if (detailDTOItem.MgDataList != null && detailDTOItem.MgDataList.Count() > 0)
                    {
                        var insertMgDataSql = InsertFLowcharMGDataSql(detailDTOItem.MgDataList, detailUID);
                        DataContext.Database.ExecuteSqlCommand(insertMgDataSql.ToString());
                    }

                    //将老版本的绑定物料员数据插入到新版本中去
                    if (detailDTOItem.PCMHList != null && detailDTOItem.PCMHList.Count() > 0)
                    {
                        //物料员跟FlDetail关系为1对1，但是这里因为detailItem.FlowChart_PC_MH_Relationship为集合类型不得已将pcList转为list
                        var insertPCSql = InsertPCMHSql(detailDTOItem.PCMHList, detailUID);
                        DataContext.Database.ExecuteSqlCommand(insertPCSql);
                    }

                    //----------Destiny需要更新的六张表    start----------------
                    if (detailDTOItem.FlowChartDetailDTO.Old_FlowChart_Detail_UID > 0)
                    {
                        var updateQAAndOqCSql = UpdateQAAndOQCSql(detailDTOItem.FlowChartDetailDTO.Old_FlowChart_Detail_UID, detailUID, accountID);
                        DataContext.Database.ExecuteSqlCommand(updateQAAndOqCSql);
                    }
                    //----------Destiny需要更新的六张表    end----------------
                }

                ExecSPTemp(importItem);

                trans.Commit();
            }

        }

        public void ExecSPTemp(FlowChartImport importItem)
        {
            try
            {
                var newVersion = importItem.FlowChartMasterDTO.FlowChart_Version;
                var para1 = new SqlParameter("@uids", importItem.FlowChartMasterDTO.Project_UID);
                var para1Version = new SqlParameter("@version", newVersion);
                var partTypes = new SqlParameter("@partTypes", importItem.FlowChartMasterDTO.Part_Types);
                DataContext.Database.ExecuteSqlCommand("usp_InsertProjectDataToMiddleTable @uids,@version,@partTypes", para1, para1Version, partTypes);

                var para2 = new SqlParameter("@uids", importItem.FlowChartMasterDTO.Project_UID);
                var para2Version = new SqlParameter("@version", newVersion);
                var partTypes2 = new SqlParameter("@partTypes", importItem.FlowChartMasterDTO.Part_Types);
                DataContext.Database.ExecuteSqlCommand("usp_InsertFlowChartDataToMiddleTable @uids,@version,@partTypes", para2, para2Version, partTypes2);

                var para3 = new SqlParameter("@uids", importItem.FlowChartMasterDTO.Project_UID);
                var para3Version = new SqlParameter("@version", newVersion);
                var partTypes3 = new SqlParameter("@partTypes", importItem.FlowChartMasterDTO.Part_Types);
                DataContext.Database.ExecuteSqlCommand("usp_InsertProductRoutingDataToMiddleTable @uids,@version,@partTypes", para3, para3Version, partTypes3);

                DataContext.Database.ExecuteSqlCommand("[Etransfer_Prod].[dbo].DownloadPDMSProductMaster");
                DataContext.Database.ExecuteSqlCommand("[Etransfer_Prod].[dbo].DownloadPDMSStationMaster");
                DataContext.Database.ExecuteSqlCommand("[Etransfer_Prod].[dbo].DownloadPDMSProductRouting");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void ExecProjectSP(int projectId)
        {
            var para1 = new SqlParameter("@uids", projectId);
            DataContext.Database.ExecuteSqlCommand("usp_InsertProjectDataToMiddleTable @uids", para1);
            DataContext.Database.ExecuteSqlCommand("[Etransfer_Prod].[dbo].DownloadPDMSProductMaster");
        }


        public void BatchImportPlan(List<FlowChartMgDataDTO> mgDataList, int FlowChart_Master_UID)
        {
            if (mgDataList.Count() == 0)
            {
                return;
            }

            using (var trans = DataContext.Database.BeginTransaction())
            {
                var minDate = mgDataList.Select(m => m.Product_Date).Min().ToShortDateString();
                var maxDate = mgDataList.Select(m => m.Product_Date).Max().ToShortDateString();
                //全删操作
                var deleteSql = @"delete from FlowChart_MgData where Product_Date >= '{0}' and Product_Date <= '{1}' and FlowChart_Detail_UID in
                                    (select FlowChart_Detail_UID from FlowChart_Detail where FlowChart_Master_UID={2} and 
                                    FlowChart_Version=(select max(FlowChart_Version) from FlowChart_Detail where FlowChart_Master_UID={2}))";
                deleteSql = string.Format(deleteSql, minDate, maxDate, FlowChart_Master_UID);
                DataContext.Database.ExecuteSqlCommand(deleteSql);

                //全插操作
                StringBuilder sb = new StringBuilder();
                foreach (var mgDataItem in mgDataList)
                {
                    var insertSql = string.Format("insert into FlowChart_MgData values ({0},'{1}',{2},{3},{4},'{5}',{6});",
                        mgDataItem.FlowChart_Detail_UID,
                        mgDataItem.Product_Date.ToShortDateString(),
                        mgDataItem.Product_Plan,
                        mgDataItem.IE_DeptHuman,
                        mgDataItem.Modified_UID,
                        mgDataItem.Modified_Date.ToString("yyyy-MM-dd hh:mm:ss"),
                        mgDataItem.Proper_WIP);
                    sb.AppendLine(insertSql);

                }
                string sql = sb.ToString();
                DataContext.Database.ExecuteSqlCommand(sb.ToString());
                trans.Commit();
            }
        }


        public void BatchIEImportPlan(List<FlowChartIEMgDataDTO> IEMgDataList, int FlowChart_Master_UID)
        {
            if (IEMgDataList.Count() == 0)
            {
                return;
            }

            using (var trans = DataContext.Database.BeginTransaction())
            {
                var minDate = IEMgDataList.Select(m => m.IE_TargetDate).Min().ToShortDateString();
                var maxDate = IEMgDataList.Select(m => m.IE_TargetDate).Max().ToShortDateString();
                //全删操作
                var deleteSql = @"delete from FlowChart_IEData where IE_TargetDate >= '{0}' and IE_TargetDate <= '{1}' and FlowChart_Detail_UID in
                                    (select FlowChart_Detail_UID from FlowChart_Detail where FlowChart_Master_UID={2} and 
                                    FlowChart_Version=(select max(FlowChart_Version) from FlowChart_Detail where FlowChart_Master_UID={2}))";
                deleteSql = string.Format(deleteSql, minDate, maxDate, FlowChart_Master_UID);
                DataContext.Database.ExecuteSqlCommand(deleteSql);
                //当传入是每日计划时 判断表中是否存在当前日期的计划
                var timeNow =  DateTime.Now;
                string sql1 = @"delete FROM dbo.Flowchart_IEData where IE_TargetDate='{0}' ";
                sql1 = string.Format(sql1, timeNow);
               
                DataContext.Database.ExecuteSqlCommand(sql1);

                //全插操作
                StringBuilder sb = new StringBuilder();
               
                foreach (var mgDataItem in IEMgDataList)
                {
                    var insertIESql = string.Format("insert into Flowchart_IEData values ({0},'{1}',{2},{3},{4},{5},'{6}');",
                        mgDataItem.FlowChart_Detail_UID,
                     
                        mgDataItem.IE_TargetDate.ToShortDateString(),
                        mgDataItem.ShiftTimeID,
                          mgDataItem.IE_DeptHuman,
                         mgDataItem.IE_TargetEfficacy,
                       mgDataItem.Modified_UID,
                        mgDataItem.Modified_Date.ToString("yyyy-MM-dd hh:mm:ss"));
                        sb.AppendLine(insertIESql);

                }
                string sql = sb.ToString();
                var sss = sb.ToString();
                DataContext.Database.ExecuteSqlCommand(sb.ToString());
                trans.Commit();
            }
        }

        public List<FlowChartPlanManagerVM> QueryFlowMGData(int masterUID, DateTime date, out int count)
        {
            int uid = 0;
            var Flowchart_master_uid = new SqlParameter("Flowchart_master_uid", masterUID);
            var Monday_Date = new SqlParameter("Monday_Date", date);
            var Flowchar_detail_uid = new SqlParameter("Flowchar_detail_uid", uid);

            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<FlowChartPlanManagerVM>("usp_SearchProductPlan @Flowchart_master_uid , @Monday_Date, @Flowchar_detail_uid", Flowchart_master_uid, Monday_Date, Flowchar_detail_uid).ToList();
            count = 0;

            return result;

        }

        public List<IEPlanManagerVM> QueryFlowIEMGData(int masterUID, DateTime date, out int count)
        {
            int uid = 0;
            var Flowchart_master_uid = new SqlParameter("Flowchart_master_uid", masterUID);
            var Monday_Date = new SqlParameter("Monday_Date", date);
            var Flowchar_detail_uid = new SqlParameter("Flowchar_detail_uid", uid);

            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<IEPlanManagerVM>("usp_SearchIEProductPlan @Flowchart_master_uid , @Monday_Date, @Flowchar_detail_uid", Flowchart_master_uid, Monday_Date, Flowchar_detail_uid).ToList();
            count = 0;

            return result;

        }



        public FlowChartPlanManagerVM QueryFlowMGDataSingle(int detailUID, DateTime date)
        {
            int uid = 0;
            var Flowchart_master_uid = new SqlParameter("Flowchart_master_uid", uid);
            var Monday_Date = new SqlParameter("Monday_Date", date);
            var Flowchar_detail_uid = new SqlParameter("Flowchar_detail_uid", detailUID);

            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<FlowChartPlanManagerVM>("usp_SearchProductPlan @Flowchart_master_uid , @Monday_Date, @Flowchar_detail_uid", Flowchart_master_uid, Monday_Date, Flowchar_detail_uid).ToList();
            if (result.Count == 0) return null;
            else
                return result[0];
        }


        public IEPlanManagerVM QueryFlowIEMGDataSingle(int detailUID, DateTime date,int shiftTimeId)
        {
            int uid = 0;
            var Flowchart_master_uid = new SqlParameter("Flowchart_master_uid", uid);
            var Monday_Date = new SqlParameter("Monday_Date", date);
            var Flowchar_detail_uid = new SqlParameter("Flowchar_detail_uid", detailUID);
            var ShiftTimeID = new SqlParameter("ShiftTimeID", shiftTimeId);

            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<IEPlanManagerVM>("usp_SearchIEProductPlan1 @Flowchart_master_uid , @Monday_Date, @Flowchar_detail_uid, @ShiftTimeID", Flowchart_master_uid, Monday_Date, Flowchar_detail_uid, ShiftTimeID).ToList();
            if (result.Count == 0) return null;
            else
                return result[0];
        }

        public IEPlanManagerVM QueryFlowIEMGDataSingle1(int detailUID, DateTime date)
        {
            int uid = 0;
            var Flowchart_master_uid = new SqlParameter("Flowchart_master_uid", uid);
            var Monday_Date = new SqlParameter("Monday_Date", date);
            var Flowchar_detail_uid = new SqlParameter("Flowchar_detail_uid", detailUID);
            

            var result = ((IObjectContextAdapter)this.DataContext).ObjectContext.ExecuteStoreQuery<IEPlanManagerVM>("usp_SearchIEProductPlan @Flowchart_master_uid , @Monday_Date, @Flowchar_detail_uid", Flowchart_master_uid, Monday_Date, Flowchar_detail_uid).ToList();
            if (result.Count == 0) return null;
            else
                return result[0];
        }

        public IQueryable<FlowChart_MgData> UpdatePlan(int detailUID, DateTime date)
        {
            DateTime endDay = date.AddDays(6);
            var query = from detail in DataContext.FlowChart_Detail
                        join mgdata in DataContext.FlowChart_MgData on detail.FlowChart_Detail_UID equals mgdata.FlowChart_Detail_UID
                        where mgdata.Product_Date >= date && mgdata.Product_Date <= endDay && mgdata.FlowChart_Detail_UID == detailUID
                        select mgdata;

            return query.OrderBy(o => o.Product_Plan);
        }

        public IQueryable<FlowChart_IEData> UpdateIEPlan(int detailUID, DateTime date,int shiftTimeId)
        {
            DateTime endDay = date.AddDays(6);
            var query = from detail in DataContext.FlowChart_Detail
                        join mgdata in DataContext.FlowChart_IEData on detail.FlowChart_Detail_UID equals mgdata.FlowChart_Detail_UID
                        where mgdata.IE_TargetDate >= date && mgdata.IE_TargetDate <= endDay && mgdata.FlowChart_Detail_UID == detailUID && mgdata.ShiftTimeID == shiftTimeId
                        select mgdata;

            return query.OrderBy(o => o.FlowChart_Detail_UID);
        }

        public List<EQP_UserTable> GetByUserId(string User_id)
        {
            string sql = @"SELECT * FROM dbo.EQP_UserTable where User_id='{0}' ";
            sql = string.Format(sql, User_id);
            var dblist = DataContext.Database.SqlQuery<EQP_UserTable>(sql).ToList();
            return dblist;
        }

        public List<FlowChartHistoryGet> QueryHistoryVersion(int FlowChart_Master_UID)
        {
            string strSql = @"WITH 
                            one AS
                            (
                            SELECT FlowChart_Master_UID,FlowChart_Version, FlowChart_Version_Comment,  MAX(Modified_Date) AS Modified_Date
                            FROM dbo.FlowChart_Detail WHERE FlowChart_Master_UID={0} GROUP BY FlowChart_Master_UID, FlowChart_Version,FlowChart_Version_Comment
                            ),
                            two AS
                            (
                            SELECT *,
                            (SELECT TOP 1 Modified_UID FROM dbo.FlowChart_Detail fl 
                            WHERE one.FlowChart_Master_UID=fl.FlowChart_Master_UID 
                            AND one.FlowChart_Version = fl.FlowChart_Version
                            AND one.Modified_Date = fl.Modified_Date) AS Modified_UID FROM one
                            ),
                            three AS
                            (
                            SELECT two.*,User_Name,fm.Project_UID,fm.Part_Types,sp.Project_Name,sp.Product_Phase,bud.BU_D_Name FROM two
                            JOIN dbo.System_Users
                            ON two.Modified_UID = dbo.System_Users.Account_UID
                            JOIN dbo.FlowChart_Master fm
                            ON two.FlowChart_Master_UID = fm.FlowChart_Master_UID
                            JOIN dbo.System_Project sp
                            ON fm.Project_UID = sp.Project_UID
                            JOIN dbo.System_BU_D bud
                            ON sp.BU_D_UID = bud.BU_D_UID
                            )
                            SELECT * FROM three ORDER BY three.FlowChart_Version";
            strSql = string.Format(strSql, FlowChart_Master_UID);
            var list = DataContext.Database.SqlQuery<FlowChartHistoryGet>(strSql).ToList();
            return list;
        }


        private string InsertFLDetailSql(FlowChartDetailDTO detailItem)
        {
            var joinUid = "";
            if (!string.IsNullOrEmpty(detailItem.RelatedRepairUID))
            {
                joinUid = detailItem.RelatedRepairUID;
            }
            string insertDetailSql = @"INSERT INTO dbo.FlowChart_Detail
                                        ( FlowChart_Master_UID ,
                                          Process_Seq ,
                                          DRI ,
                                          Place ,
                                          Process ,
                                          Product_Stage ,
                                          Color ,
                                          Process_Desc ,
                                          Material_No ,
                                          FlowChart_Version ,
                                          FlowChart_Version_Comment ,
                                          Modified_UID ,
                                          Modified_Date ,
                                          FatherProcess_UID ,
                                          IsQAProcess ,
                                          Rework_Flag ,
                                          WIP_QTY ,
                                          Binding_Seq ,
                                          BeginTime ,
                                          EndTime,
                                          System_FunPlant_UID,
                                          Location_Flag,
                                          RelatedRepairUID,
                                          Current_WH_QTY ,
                                          NullWip,
                                          Data_Source,
                                          Is_Synchronous
                                        )
                                VALUES  ( {0} , -- FlowChart_Master_UID - int
                                          {1} , -- Process_Seq - int
                                          N'{2}' , -- DRI - nvarchar(50)
                                          N'{3}' , -- Place - nvarchar(50)
                                          N'{4}' , -- Process - nvarchar(50)
                                          {5} , -- Product_Stage - int
                                          N'{6}' , -- Color - nvarchar(50)
                                          N'{7}' , -- Process_Desc - nvarchar(100)
                                          N'{8}' , -- Material_No - nvarchar(50)
                                          {9} , -- FlowChart_Version - int
                                          N'{10}' , -- FlowChart_Version_Comment - nvarchar(200)
                                          {11} , -- Modified_UID - int
                                          '{12}' , -- Modified_Date - datetime
                                          NULL , -- FatherProcess_UID - int
                                          '{13}' , -- IsQAProcess - varchar(50)
                                          N'{14}' , -- Rework_Flag - nvarchar(20)
                                          {15} , -- WIP_QTY - int
                                          {16} , -- Binding_Seq - int
                                          '{17}' , -- BeginTime - datetime
                                          NULL,  -- EndTime - datetime,
                                          {18},
                                          {19},
                                          N'{20}',
                                           {21},
                                           {22},
                                          N'{23}',
                                           {24}
                                        );";
            insertDetailSql = string.Format(insertDetailSql,
            detailItem.FlowChart_Master_UID,
            detailItem.Process_Seq,
            detailItem.DRI,
            detailItem.Place,
            detailItem.Process,
            detailItem.Product_Stage,
            detailItem.Color,
            detailItem.Process_Desc,
            detailItem.Material_No,
            detailItem.FlowChart_Version,
            detailItem.FlowChart_Version_Comment,
            detailItem.Modified_UID,
            detailItem.Modified_Date.ToString(FormatConstants.DateTimeFormatString),
            detailItem.IsQAProcess,
            detailItem.Rework_Flag,
            detailItem.WIP_QTY,
            detailItem.Binding_Seq,
            detailItem.BeginTime.ToString(FormatConstants.DateTimeFormatString),
            detailItem.System_FunPlant_UID,
            detailItem.Location_Flag ? 1 : 0,
            joinUid,
            detailItem.Current_WH_QTY,
            detailItem.NullWip,
            detailItem.Data_Source,
            detailItem.Is_Synchronous ? 1 : 0
            );
            return insertDetailSql;
        }

        private string InsertFLDetailWUXI_MSql(FlowChartDetailDTO detailItem)
        {
            string insertDetailSql = @"INSERT INTO dbo.FlowChart_Detail
                                        ( FlowChart_Master_UID ,
                                          Process_Seq ,
                                          DRI ,
                                          Place ,
                                          Process ,
                                          Product_Stage ,
                                          Color ,
                                          Process_Desc ,
                                          Material_No ,
                                          FlowChart_Version ,
                                          FlowChart_Version_Comment ,
                                          Modified_UID ,
                                          Modified_Date ,
                                          FatherProcess_UID ,
                                          IsQAProcess ,
                                          Rework_Flag ,
                                          WIP_QTY ,
                                          Binding_Seq ,
                                          BeginTime ,
                                          EndTime,
                                          System_FunPlant_UID,
                                          FromWHS ,
                                          ToWHSOK ,
                                          ToWHSNG ,
                                          ItemNo ,
                                          Edition,
RelatedRepairUID,
Location_Flag
                                        )
                                VALUES  ( {0} , -- FlowChart_Master_UID - int
                                          {1} , -- Process_Seq - int
                                          N'{2}' , -- DRI - nvarchar(50)
                                          N'{3}' , -- Place - nvarchar(50)
                                          N'{4}' , -- Process - nvarchar(50)
                                          {5} , -- Product_Stage - int
                                          N'{6}' , -- Color - nvarchar(50)
                                          N'{7}' , -- Process_Desc - nvarchar(100)
                                          N'{8}' , -- Material_No - nvarchar(50)
                                          {9} , -- FlowChart_Version - int
                                          N'{10}' , -- FlowChart_Version_Comment - nvarchar(200)
                                          {11} , -- Modified_UID - int
                                          '{12}' , -- Modified_Date - datetime
                                          NULL , -- FatherProcess_UID - int
                                          '{13}' , -- IsQAProcess - varchar(50)
                                          N'{14}' , -- Rework_Flag - nvarchar(20)
                                          {15} , -- WIP_QTY - int
                                          {16} , -- Binding_Seq - int
                                          '{17}' , -- BeginTime - datetime
                                          NULL,  -- EndTime - datetime,
                                          {18}, -- System_FunPlant_UID
                                          '{19}', -- FromWHS
                                          '{20}', -- ToWHSOK
                                          '{21}', -- ToWHSNG
                                          '{22}', -- ItemNo
                                          N'{23}', -- Edition
                                           null,
                                           0
                                        );";
            insertDetailSql = string.Format(insertDetailSql,
                detailItem.FlowChart_Master_UID,
                detailItem.Process_Seq,
                detailItem.DRI,
                detailItem.Place,
                detailItem.Process,
                detailItem.Product_Stage,
                detailItem.Color,
                detailItem.Process_Desc,
                detailItem.Material_No,
                detailItem.FlowChart_Version,
                detailItem.FlowChart_Version_Comment,
                detailItem.Modified_UID,
                detailItem.Modified_Date,
                detailItem.IsQAProcess,
                detailItem.Rework_Flag,
                detailItem.WIP_QTY,
                detailItem.Binding_Seq,
                detailItem.BeginTime,
                detailItem.System_FunPlant_UID,
                detailItem.FromWHS,
                detailItem.ToWHSOK,
                detailItem.ToWHSNG,
                detailItem.ItemNo,
                detailItem.Edition
                );
            return insertDetailSql;
        }


        private StringBuilder InsertFLowcharMGDataSql(List<FlowChartMgDataDTO> mgDataList, decimal detailUID)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var mgDataItem in mgDataList)
            {
                string insertMGSql = @"INSERT INTO dbo.FlowChart_MgData
                            ( FlowChart_Detail_UID ,
                                Product_Date ,
                                Product_Plan ,
                                Target_Yield ,
                                Modified_UID ,
                                Modified_Date,
                                 Proper_WIP
                            )
                    VALUES  (   {0}, -- FlowChart_Detail_UID - int
                               '{1}', -- Product_Date - date
                                {2}, -- Product_Plan - int
                                {3}, -- Target_Yield - float
                                {4}, -- Modified_UID - int
                               '{5}', -- Modified_Date - datetime,
                                {6}
                            ) ";
                insertMGSql = string.Format(insertMGSql,
                    Convert.ToInt32(detailUID),
                    mgDataItem.Product_Date.ToString(FormatConstants.DateTimeFormatString),
                    mgDataItem.Product_Plan,
                    mgDataItem.IE_DeptHuman,
                    mgDataItem.Modified_UID,
                    mgDataItem.Modified_Date.ToString(FormatConstants.DateTimeFormatString),
                    mgDataItem.Proper_WIP == null ? 0 : mgDataItem.Proper_WIP
                    );
                sb.Append(insertMGSql);
            }
            return sb;
        }

        private string InsertPCMHSql(List<FlowChartPCMHRelationshipDTO> pcList, decimal detailUID)
        {
            var pcItem = pcList.First();
            string sql = @"INSERT INTO dbo.FlowChart_PC_MH_Relationship
                        ( FlowChart_Detail_UID ,
                            Place ,
                            MH_UID ,
                            Modified_UID ,
                            Modified_Date
                        )
                VALUES  ( 
                            {0} , -- FlowChart_Detail_UID - int
                            N'{1}' , -- Place - nvarchar(20)
                            {2} , -- MH_UID - int
                            {3} , -- Modified_UID - int
                            '{4}'  -- Modified_Date - datetime
                        )";
            sql = string.Format(sql, Convert.ToInt32(detailUID), pcItem.Place, pcItem.MH_UID, pcItem.Modified_UID, pcItem.Modified_Date.ToString(FormatConstants.DateTimeFormatString));
            return sql;
        }

        private string UpdateQAAndOQCSql(int oldFLDetailId, decimal detailUID, int accountID)
        {
            StringBuilder sb = new StringBuilder();

            string updateMESProcessMgSql = @" UPDATE dbo.ProcessIDTransformConfig SET PIS_ProcessID={0},Modified_UID={1},Modified_Date=GETDATE() WHERE PIS_ProcessID={2}; ";
            updateMESProcessMgSql = string.Format(updateMESProcessMgSql, Convert.ToInt32(detailUID), accountID, oldFLDetailId);
            sb.Append(updateMESProcessMgSql);

            string updateQAMgSql = @"UPDATE dbo.QualityAssurance_MgData SET FlowChart_Detail_UID={0},Modified_UID={1},Modified_Date=GETDATE() WHERE FlowChart_Detail_UID={2}; ";
            updateQAMgSql = string.Format(updateQAMgSql, Convert.ToInt32(detailUID), accountID, oldFLDetailId);
            sb.Append(updateQAMgSql);

            string updateQAMasterSql = @"UPDATE dbo.QualityAssurance_InputMaster SET FlowChart_Detail_UID={0},Modified_UID={1},Modified_Date=GETDATE() WHERE FlowChart_Detail_UID={2}; ";
            updateQAMasterSql = string.Format(updateQAMasterSql, Convert.ToInt32(detailUID), accountID, oldFLDetailId);
            sb.Append(updateQAMasterSql);

            string updateQAMasterHistorySql = @"UPDATE dbo.QualityAssurance_InputMaster_History SET FlowChart_Detail_UID={0},Modified_UID={1},Modified_Date=GETDATE() WHERE FlowChart_Detail_UID={2}; ";
            updateQAMasterHistorySql = string.Format(updateQAMasterHistorySql, Convert.ToInt32(detailUID), accountID, oldFLDetailId);
            sb.Append(updateQAMasterHistorySql);

            string updateOQCMasterSql = @"UPDATE dbo.OQC_InputMaster SET FlowChart_Detail_UID={0},Modifier_UID={1},Modified_date=GETDATE() WHERE FlowChart_Detail_UID={2}; ";
            updateOQCMasterSql = string.Format(updateOQCMasterSql, Convert.ToInt32(detailUID), accountID, oldFLDetailId);
            sb.Append(updateOQCMasterSql);

            string updateOQCMasterHistorySql = @"UPDATE dbo.OQC_InputMaster_History SET FlowChart_Detail_UID={0},Modifier_UID={1},Modified_date=GETDATE() WHERE FlowChart_Detail_UID={2}; ";
            updateOQCMasterHistorySql = string.Format(updateOQCMasterHistorySql, Convert.ToInt32(detailUID), accountID, oldFLDetailId);
            sb.Append(updateOQCMasterHistorySql);

            string updateExtionSql = @"UPDATE dbo.ExceptionTypeWithFlowchart SET FlowChart_Detail_UID={0} WHERE FlowChart_Detail_UID={1}; ";
            updateExtionSql = string.Format(updateExtionSql, Convert.ToInt32(detailUID), oldFLDetailId);
            sb.Append(updateExtionSql);

            string updateProcessIDTransformConfigSql = @"UPDATE dbo.ProcessIDTransformConfig SET PIS_ProcessID={0} WHERE PIS_ProcessID={1}; ";
            updateProcessIDTransformConfigSql = string.Format(updateProcessIDTransformConfigSql, Convert.ToInt32(detailUID), oldFLDetailId);
            sb.Append(updateExtionSql);

            string updateMES_StationDataRecordSql = @"UPDATE dbo.MES_StationDataRecord SET PIS_ProcessID={0} WHERE PIS_ProcessID={1}; ";
            updateMES_StationDataRecordSql = string.Format(updateMES_StationDataRecordSql, Convert.ToInt32(detailUID), oldFLDetailId);
            sb.Append(updateExtionSql);
            return sb.ToString();
        }


        #region ----排程排线

        public string JudgeFlowchart(int FlowChart_Master_UID)
        {
            string result = string.Empty;
            try
            {
                string sql = string.Format(@"UPDATE dbo.FlowChart_Master SET Statue_IE=2,[CurrentDepartent]='PP' WHERE FlowChart_Master_UID={0}", FlowChart_Master_UID);

                DataContext.Database.ExecuteSqlCommand(sql);
                result = "Success";
            }
            catch (Exception ex)
            {
                result = string.Format(@"审核失败，发生异常，请联系管理人员。异常详情：'{0}'", ex.Message.ToString());
            }
            return result;
        }

        #endregion

        public void ImportFlowchartME(ProductionPlanningModelGetAPIModel importItem, bool isEdit, int accountID)
        {
            List<ProductionPlanningModelGet> GetList = importItem.GetList;
            List<FlowchartDetailMEEquipmentDTO> EquipDTOList = importItem.EquipDTOList;
            List<FlowchartDetailMEEquipmentDTO> AutoEquipDTOList = importItem.AutoEquipDTOList;

            using (var trans = DataContext.Database.BeginTransaction())
            {
                decimal flowchartMasterUID = 0;
                foreach (var masterAndDetailItem in GetList)
                {
                    FlowChartMasterDTO masterDTO = masterAndDetailItem.flowchartMasterDTO;
                    List<FlowchartDetailMEDTO> detailMeList = masterAndDetailItem.flowchartDetailMeDTOList;
                    var masterUidSql = "SELECT  SCOPE_IDENTITY();";
                    if (isEdit)
                    {
                        string updateMasterSql = UpdateMasterSql(masterDTO);
                        DataContext.Database.ExecuteSqlCommand(updateMasterSql);
                        flowchartMasterUID = masterDTO.FlowChart_Master_UID;
                    }
                    else
                    {
                        string insertMasterSql = InsertMasterSql(masterDTO);
                        DataContext.Database.ExecuteSqlCommand(insertMasterSql);
                        flowchartMasterUID = DataContext.Database.SqlQuery<decimal>(masterUidSql).First();
                    }

                    //批量插入表Flowchart_Detail_ME，Flowchart_Detail_ME_Equipment
                    foreach (var detailMeItem in detailMeList)
                    {
                        string insertDetailMeSql = InsertDetailMeSql(detailMeItem, flowchartMasterUID);
                        DataContext.Database.ExecuteSqlCommand(insertDetailMeSql);
                        //获取主键值
                        var detailMeUID = DataContext.Database.SqlQuery<decimal>(masterUidSql).First();
                        //有可能一个制程对应多个设备
                        var equipList = EquipDTOList.Where(m => m.Process_Seq == detailMeItem.Process_Seq).ToList();
                        foreach (var equipItem in equipList)
                        {
                            string insertEquipSql = InsertEquipSql(equipItem, detailMeItem, detailMeUID);
                            DataContext.Database.ExecuteSqlCommand(insertEquipSql);
                        }
                        var autoEquipList = AutoEquipDTOList.Where(m => m.Process_Seq == detailMeItem.Process_Seq).ToList();
                        foreach (var autoEquipItem in autoEquipList)
                        {
                            string insertAutoEquipSql = InsertAutoEquipSql(autoEquipItem, detailMeItem, detailMeUID);
                            DataContext.Database.ExecuteSqlCommand(insertAutoEquipSql);
                        }
                    }
                }
                trans.Commit();
            }

        }


        public Product_Input getRealtedProduct(int flowchartMasterUid, int flowChart_Version, int Process_Seq, string color, DateTime ProductDate, string TimeInterVal)
        {
            var query = from PI in DataContext.Product_Input
                        where PI.FlowChart_Master_UID == flowchartMasterUid && PI.FlowChart_Version == flowChart_Version
                        && PI.Process_Seq == Process_Seq && PI.Color == color && PI.Product_Date == ProductDate
                        && PI.Time_Interval == TimeInterVal
                        select PI;
            return query.FirstOrDefault();
        }

        #region ME制程导入Sql
        public List<FlowChartModelGet> GetFlowchartMEList(FlowChartModelSearch search, Page page, out int count)
        {
            using (var context = new SPPContext())
            {
                var query = (from M in context.FlowChart_Master
                             join P in context.System_Project
                             on M.Project_UID equals P.Project_UID
                             join Bom in context.System_OrganizationBOM
                             on P.Organization_UID equals Bom.ChildOrg_UID
                             join BUD in context.System_BU_D
                             on P.BU_D_UID equals BUD.BU_D_UID
                             join U in context.System_Users
                             on M.Modified_UID equals U.Account_UID
                             select new FlowChartModelGet
                             {
                                 FlowChart_Master_UID = M.FlowChart_Master_UID,
                                 BU_D_Name = BUD.BU_D_Name,
                                 Project_Name = P.Project_Name,
                                 Part_Types = M.Part_Types,
                                 Product_Phase = M.Product_Phase,
                                 Is_Closed = M.Is_Closed,
                                 Is_Latest = M.Is_Latest,
                                 FlowChart_Version = M.FlowChart_Version,
                                 FlowChart_Version_Comment = M.FlowChart_Version_Comment,
                                 User_Name = U.User_Name,
                                 Modified_Date = M.Modified_Date,
                                 User_NTID = U.User_NTID,
                                 IsTemp = false,
                                 OP_type = P.OP_TYPES,
                                 Plant_OrganizationUID = Bom.ParentOrg_UID.Value,
                                 Organization_UID = P.Organization_UID,
                                 Project_UID = P.Project_UID,
                                 CurrentDepartent = M.CurrentDepartent,
                                 Statue_IE = M.Statue_IE
                             });

                //如果用户是超级管理员
                var isAdmin = search.RoleList.Exists(m => m.Role_UID == 1);
                //如果是超级管理员并且UserOrg表里面没有数据就是厂区什么的都没设，那就什么都不显示
                if (!isAdmin)
                {
                    //根据用户所属的Organization_UID查询对应的专案
                    query = query.Where(m => search.OPType_OrganizationUIDList.Contains(m.Organization_UID));
                }

                if (search.PlantUIDList.Count() > 0)
                {
                    query = query.Where(m => search.PlantUIDList.Contains(m.Plant_OrganizationUID));
                }

                if (search.OPType_OrganizationUIDList.Count() > 0)
                {
                    query = query.Where(m => search.OPType_OrganizationUIDList.Contains(m.Organization_UID));
                }

                var isMe = search.RoleList.Exists(m => m.Role_Name.Contains("ME"));
                //如果不是ME部门,则只获取MP阶段的数据
                //if (!isMe)
                //{
                //    query = query.Where(m => m.Product_Phase == "MP");
                //}

                //如果ProjectUIDList有值则查询，没有值则带出所有专案
                if (search.ProjectUIDList.Count() > 0)
                {
                    query = query.Where(m => search.ProjectUIDList.Contains(m.Project_UID));
                }

                if (!string.IsNullOrWhiteSpace(search.BU_D_Name))
                {
                    query = query.Where(m => m.BU_D_Name.Contains(search.BU_D_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Project_Name))
                {
                    query = query.Where(m => m.Project_Name.Contains(search.Project_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Part_Types))
                {
                    query = query.Where(m => m.Part_Types.Contains(search.Part_Types));
                }
                if (!string.IsNullOrWhiteSpace(search.Product_Phase))
                {
                    query = query.Where(m => m.Product_Phase.Contains(search.Product_Phase));
                }
                switch (search.Is_Closed)
                {
                    case StructConstants.IsClosedStatus.ClosedKey:
                        query = query.Where(m => m.Is_Closed == true && m.IsTemp == false);
                        break;
                    case StructConstants.IsClosedStatus.ProcessKey:
                        query = query.Where(m => m.Is_Closed == false && m.IsTemp == false);
                        break;
                    case StructConstants.IsClosedStatus.ApproveKey:
                        query = query.Where(m => m.IsTemp == true);
                        break;
                }
                if (search.Is_Latest == StructConstants.IsLastestStatus.LastestKey)
                {
                    query = query.Where(m => m.Is_Latest == true);
                }
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_From) <= 0);
                }
                if (search.Modified_Date_End != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_End) >= 0);
                }
                if (search.Modified_By != null)
                {
                    query = query.Where(m => m.User_NTID == search.Modified_By);
                }

                count = query.Count();
                query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Project_Name).Skip(page.Skip).Take(page.PageSize);
                var list = query.ToList();
                return list;
            }
        }

        private string InsertMasterSql(FlowChartMasterDTO masterDTO)
        {
            string insertMasterSql = @"INSERT INTO dbo.FlowChart_Master
                                    (Project_UID,
                                      Part_Types,
                                      FlowChart_Version,
                                      FlowChart_Version_Comment,
                                      Is_Latest,
                                      Is_Closed,
                                      Modified_UID,
                                      Modified_Date,
                                      Organization_UID,
                                      Product_Phase,
                                      CurrentDepartent,
                                      Statue_IE,
                                      Created_UID,
                                      Created_Date
                                    )
                            VALUES({0}, --Project_UID - int
                                      N'{1}', --Part_Types - nvarchar(50)
                                      {2}, --FlowChart_Version - int
                                      N'{3}', --FlowChart_Version_Comment - nvarchar(200)
                                      {4}, --Is_Latest - bit
                                      {5}, --Is_Closed - bit
                                      {6}, --Modified_UID - int
                                      '{7}', --Modified_Date - datetime
                                      {8}, --Organization_UID - int
                                      N'{9}', --Product_Phase - nvarchar(10)
                                      N'{10}', --CurrentDepartent - nvarchar(10)
                                      0,
                                      {11}, --Created_UID - int
                                      '{12}'-- Created_Date - datetime
                                    );";
            insertMasterSql = string.Format(insertMasterSql,
                masterDTO.Project_UID,
                masterDTO.Part_Types,
                masterDTO.FlowChart_Version,
                masterDTO.FlowChart_Version_Comment,
                masterDTO.Is_Latest ? 1 : 0,
                masterDTO.Is_Closed ? 1 : 0,
                masterDTO.Modified_UID,
                masterDTO.Modified_Date,
                masterDTO.Organization_UID,
                masterDTO.Product_Phase,
                masterDTO.CurrentDepartent,
                masterDTO.Created_UID,
                masterDTO.Created_Date);
            return insertMasterSql;
        }

        private string UpdateMasterSql(FlowChartMasterDTO masterDTO)
        {
            string updateMasterSql = @"UPDATE dbo.FlowChart_Master SET 
                                    FlowChart_Version = FlowChart_Version + 1,
                                    FlowChart_Version_Comment = N'{0}',
                                    CurrentDepartent = 'ME',
                                    Modified_UID = {1},
                                    Modified_Date = GETDATE()
                                    WHERE FlowChart_Master_UID = {2}";
            updateMasterSql = string.Format(updateMasterSql,
                masterDTO.FlowChart_Version_Comment,
                masterDTO.Modified_UID,
                masterDTO.FlowChart_Master_UID);
            return updateMasterSql;
        }

        private string InsertDetailMeSql(FlowchartDetailMEDTO detailMeItem, decimal flowchartMasterUID)
        {
            string insertDetailMeSql = @"INSERT INTO dbo.Flowchart_Detail_ME
                                        ( FlowChart_Master_UID ,
                                          Binding_Seq ,
                                          Process_Seq ,
                                          Process ,
                                          Process_Desc ,
                                          Color ,
                                          Processing_Equipment ,
                                          Automation_Equipment ,
                                          Processing_Fixtures ,
                                          Auxiliary_Equipment ,
                                          Equipment_CT ,
                                          Setup_Time ,
                                          Total_Cycletime ,
                                          Estimate_Yield ,
                                          Manpower_Ratio ,
                                          Capacity_ByHour ,
                                          Capacity_ByDay ,
                                          Equipment_RequstQty ,
                                          Manpower_2Shift ,
                                          Created_Date ,
                                          Created_UID ,
                                          Modified_Date ,
                                          Modified_UID ,
                                          Process_Station ,
                                          System_FunPlant_UID ,
                                          FlowChart_Version,
                                          FlowChart_Version_Comment 
                                        )
                                VALUES  ( {0} , -- FlowChart_Master_UID - int
                                          {1} , -- Binding_Seq - int
                                          {2} , -- Process_Seq - int
                                          N'{3}' , -- Process - nvarchar(50)
                                          N'{4}' , -- Process_Desc - nvarchar(100)
                                          N'{5}' , -- Color - nvarchar(50)
                                          N'{6}' , -- Processing_Equipment - nvarchar(200)
                                          N'{7}' , -- Automation_Equipment - nvarchar(200)
                                          N'{8}' , -- Processing_Fixtures - nvarchar(200)
                                          N'{9}' , -- Auxiliary_Equipment - nvarchar(200)
                                          {10} , -- Equipment_CT - decimal
                                          {11} , -- Setup_Time - decimal
                                          {12} , -- Total_Cycletime - decimal
                                          {13} , -- Estimate_Yield - decimal
                                          {14} , -- Manpower_Ratio - decimal
                                          {15} , -- Capacity_ByHour - decimal
                                          {16} , -- Capacity_ByDay - decimal
                                          {17} , -- Equipment_RequstQty - int
                                          {18} , -- Manpower_2Shift - int
                                          '{19}' , -- Created_Date - datetime
                                          {20} , -- Created_UID - int
                                          '{21}', -- Modified_Date - datetime
                                          {22} , -- Modified_UID - int
                                          N'{23}' , -- Process_Station - nvarchar(50)
                                          {24} , -- System_FunPlant_UID
                                          {25},  -- FlowChart_Version - int
                                          N'{26}'   -- FlowChart_Version_Comment
                                        ); ";

            insertDetailMeSql = string.Format(insertDetailMeSql,
                    Convert.ToInt32(flowchartMasterUID),
                    detailMeItem.Binding_Seq,
                    detailMeItem.Process_Seq,
                    detailMeItem.Process,
                    detailMeItem.Process_Desc,
                    detailMeItem.Color,
                    detailMeItem.Processing_Equipment.Replace("'", "''"), //防止excel里面的单引号导致不能插入问题
                    detailMeItem.Automation_Equipment.Replace("'", "''"),
                    detailMeItem.Processing_Fixtures.Replace("'", "''"),
                    detailMeItem.Auxiliary_Equipment.Replace("'", "''"),
                    detailMeItem.Equipment_CT ?? -1, //防止此数据为空而不能插入，下面会有替换
                    detailMeItem.Setup_Time ?? -1,
                    detailMeItem.Total_Cycletime ?? -1,
                    detailMeItem.Estimate_Yield,
                    detailMeItem.Manpower_Ratio ?? -1,
                    detailMeItem.Capacity_ByHour,
                    detailMeItem.Capacity_ByDay,
                    detailMeItem.Equipment_RequstQty ?? -1,
                    detailMeItem.Manpower_2Shift ?? -1,
                    detailMeItem.Created_Date,
                    detailMeItem.Created_UID,
                    detailMeItem.Modified_Date,
                    detailMeItem.Modified_UID,
                    detailMeItem.Process_Station,
                    detailMeItem.System_FunPlant_UID,
                    detailMeItem.FlowChart_Version,
                    detailMeItem.FlowChart_Version_Comment
            );

            insertDetailMeSql = insertDetailMeSql.Replace("-1", "NULL");

            return insertDetailMeSql;
        }

        private string InsertEquipSql(FlowchartDetailMEEquipmentDTO equipItem, FlowchartDetailMEDTO detailMeItem, decimal detailMeUID)
        {
            string insertSql = @"INSERT INTO dbo.Flowchart_Detail_ME_Equipment
                                ( Flowchart_Detail_ME_UID ,
                                  Equipment_Name ,
                                  Plan_CT ,
                                  EquipmentQty ,
                                  Ratio ,
                                  RequestQty ,
                                  EquipmentType ,
                                  Created_Date ,
                                  Created_UID ,
                                  Modified_Date ,
                                  Modified_UID
                                )
                        VALUES  ( {0} , -- Flowchart_Detail_ME_UID - int
                                  N'{1}' , -- Equipment_Name - nvarchar(50)
                                  {2} , -- Plan_CT - decimal
                                  {3} , -- EquipmentQty - int
                                  {4} , -- Ratio - decimal
                                  {5} , -- RequestQty - int
                                  N'{6}' , -- EquipmentType - nvarchar(50)
                                  '{7}' , -- Created_Date - datetime
                                  {8} , -- Created_UID - int
                                  '{9}' , -- Modified_Date - datetime
                                  {10}  -- Modified_UID - int
                                )";
            insertSql = string.Format(insertSql,
                detailMeUID,
                equipItem.Equipment_Name,
                equipItem.Plan_CT,
                equipItem.EquipmentQty,
                equipItem.Ratio,
                equipItem.RequestQty,
                equipItem.EquipmentType,
                equipItem.Created_Date,
                equipItem.Created_UID,
                equipItem.Modified_Date,
                equipItem.Modified_UID
                );
            return insertSql;
        }

        private string InsertAutoEquipSql(FlowchartDetailMEEquipmentDTO autoEquipItem, FlowchartDetailMEDTO detailMeItem, decimal detailMeUID)
        {
            string insertSql = @"INSERT INTO dbo.Flowchart_Detail_ME_Equipment
                                ( Flowchart_Detail_ME_UID ,
                                  Equipment_Name ,
                                  Plan_CT ,
                                  EquipmentQty ,
                                  Ratio ,
                                  RequestQty ,
                                  EquipmentType ,
                                  Created_Date ,
                                  Created_UID ,
                                  Modified_Date ,
                                  Modified_UID
                                )
                        VALUES  ( {0} , -- Flowchart_Detail_ME_UID - int
                                  N'{1}' , -- Equipment_Name - nvarchar(50)
                                  {2} , -- Plan_CT - decimal
                                  {3} , -- EquipmentQty - int
                                  {4} , -- Ratio - decimal
                                  {5} , -- RequestQty - int
                                  N'{6}' , -- EquipmentType - nvarchar(50)
                                  '{7}' , -- Created_Date - datetime
                                  {8} , -- Created_UID - int
                                  '{9}' , -- Modified_Date - datetime
                                  {10}  -- Modified_UID - int
                                )";
            insertSql = string.Format(insertSql,
                detailMeUID,
                autoEquipItem.Equipment_Name,
                detailMeItem.Total_Cycletime,
                autoEquipItem.EquipmentQty,
                autoEquipItem.Ratio,
                autoEquipItem.RequestQty,
                autoEquipItem.EquipmentType,
                autoEquipItem.Created_Date,
                autoEquipItem.Created_UID,
                autoEquipItem.Modified_Date,
                autoEquipItem.Modified_UID
                );
            return insertSql;
        }
        #endregion

        #region PP制程导入Sql
        public List<FlowChartModelGet> GetFlowchartPPList(FlowChartModelSearch search, Page page, out int count)
        {
            using (var context = new SPPContext())
            {
                var query = (from M in context.FlowChart_Master
                             join P in context.System_Project
                             on M.Project_UID equals P.Project_UID
                             join Bom in context.System_OrganizationBOM
                             on P.Organization_UID equals Bom.ChildOrg_UID
                             join BUD in context.System_BU_D
                             on P.BU_D_UID equals BUD.BU_D_UID
                             join U in context.System_Users
                             on M.Modified_UID equals U.Account_UID
                             //where M.CurrentDepartent == "PP"
                             select new FlowChartModelGet
                             {
                                 FlowChart_Master_UID = M.FlowChart_Master_UID,
                                 BU_D_Name = BUD.BU_D_Name,
                                 Project_Name = P.Project_Name,
                                 Part_Types = M.Part_Types,
                                 Product_Phase = M.Product_Phase,
                                 Is_Closed = M.Is_Closed,
                                 Is_Latest = M.Is_Latest,
                                 FlowChart_Version = M.FlowChart_Version,
                                 FlowChart_Version_Comment = M.FlowChart_Version_Comment,
                                 User_Name = U.User_Name,
                                 Modified_Date = M.Modified_Date,
                                 User_NTID = U.User_NTID,
                                 IsTemp = false,
                                 OP_type = P.OP_TYPES,
                                 Plant_OrganizationUID = Bom.ParentOrg_UID.Value,
                                 Organization_UID = P.Organization_UID,
                                 Project_UID = P.Project_UID,
                                 CurrentDepartent = M.CurrentDepartent,
                                 Statue_IE = M.Statue_IE
                             });

                //如果用户是超级管理员
                var isAdmin = search.RoleList.Exists(m => m.Role_UID == 1);
                //如果是超级管理员并且UserOrg表里面没有数据就是厂区什么的都没设，那就什么都不显示
                if (!isAdmin)
                {
                    //根据用户所属的Organization_UID查询对应的专案
                    query = query.Where(m => search.OPType_OrganizationUIDList.Contains(m.Organization_UID));
                }

                if (search.PlantUIDList.Count() > 0)
                {
                    query = query.Where(m => search.PlantUIDList.Contains(m.Plant_OrganizationUID));
                }

                if (search.OPType_OrganizationUIDList.Count() > 0)
                {
                    query = query.Where(m => search.OPType_OrganizationUIDList.Contains(m.Organization_UID));
                }

                //如果ProjectUIDList有值则查询，没有值则带出所有专案
                if (search.ProjectUIDList.Count() > 0)
                {
                    query = query.Where(m => search.ProjectUIDList.Contains(m.Project_UID));
                }

                if (!string.IsNullOrWhiteSpace(search.BU_D_Name))
                {
                    query = query.Where(m => m.BU_D_Name.Contains(search.BU_D_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Project_Name))
                {
                    query = query.Where(m => m.Project_Name.Contains(search.Project_Name));
                }
                if (!string.IsNullOrWhiteSpace(search.Part_Types))
                {
                    query = query.Where(m => m.Part_Types.Contains(search.Part_Types));
                }
                if (!string.IsNullOrWhiteSpace(search.Product_Phase))
                {
                    query = query.Where(m => m.Product_Phase.Contains(search.Product_Phase));
                }
                switch (search.Is_Closed)
                {
                    case StructConstants.IsClosedStatus.ClosedKey:
                        query = query.Where(m => m.Is_Closed == true && m.IsTemp == false);
                        break;
                    case StructConstants.IsClosedStatus.ProcessKey:
                        query = query.Where(m => m.Is_Closed == false && m.IsTemp == false);
                        break;
                    case StructConstants.IsClosedStatus.ApproveKey:
                        query = query.Where(m => m.IsTemp == true);
                        break;
                }
                if (search.Is_Latest == StructConstants.IsLastestStatus.LastestKey)
                {
                    query = query.Where(m => m.Is_Latest == true);
                }
                if (search.Modified_Date_From != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_From) <= 0);
                }
                if (search.Modified_Date_End != null)
                {
                    query = query.Where(m => SqlFunctions.DateDiff("dd", m.Modified_Date, search.Modified_Date_End) >= 0);
                }
                if (search.Modified_By != null)
                {
                    query = query.Where(m => m.User_NTID == search.Modified_By);
                }

                count = query.Count();
                query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Project_Name).Skip(page.Skip).Take(page.PageSize);
                var list = query.ToList();
                return list;
            }
        }

        #endregion

        /// <summary>
        /// 获取WIP查询类型是FlowChartDetial类型的SQL
        /// </summary>
        /// <returns></returns>
        private string GetWIPAlterDetialSql()
        {
            var FlowChartDetialSql = @"SELECT
	                                    temp.Part_Types,
	                                    temp.Organization_UID,
	                                    temp.User_Name,
	                                    temp.OP_Types,
	                                    temp.FunPlant,
	                                    temp.Project_Name,
	                                    temp.Process,
	                                    temp.Color,
	                                    temp.Place,
	                                    temp.Change_UID,
	                                    temp.Change_Type,
	                                    temp.Product_UID,
	                                    temp.WIP_Old,
	                                    temp.WIP_Add,
	                                    temp.Comment,
	                                    temp.Modified_UID,
	                                    temp.Modified_Date,
	                                    temp.FlowChart_Detail_UID,
	                                    tempOrg.Organization_Name
                                    FROM
	                                    (
		                                    SELECT
			                                    pp.Part_Types,
			                                    pp.Organization_UID,
			                                    pp.User_Name,
			                                    pp.OP_Types,
			                                    pp.FunPlant,
			                                    pp.Project_Name,
			                                    pp.Process,
			                                    pp.Color,
			                                    pp.Place,
			                                    pp.Change_UID,
			                                    pp.Change_Type,
			                                    pp.Product_UID,
			                                    pp.WIP_Old,
			                                    pp.WIP_Add,
			                                    pp.Comment,
			                                    pp.Modified_UID,
			                                    pp.Modified_Date,
			                                    pp.FlowChart_Detail_UID,
			                                    bom.ParentOrg_UID
		                                    FROM
			                                    (
				                                    SELECT
					                                    c.Part_Types AS Part_Types,
					                                    o.Organization_UID AS Organization_UID,
					                                    u.User_Name AS User_Name,
					                                    p.OP_Types AS OP_Types,
					                                    f.FunPlant AS FunPlant,
					                                    p.Project_Name AS Project_Name,
					                                    d.Process AS Process,
					                                    d.Color AS Color,
					                                    d.Place AS Place,
					                                    a.Change_UID AS Change_UID,
					                                    a.Change_Type AS Change_Type,
					                                    a.Product_UID AS Product_UID,
					                                    a.WIP_Old AS WIP_Old,
					                                    a.WIP_Add AS WIP_Add,
					                                    a.Comment AS Comment,
					                                    a.Modified_UID AS Modified_UID,
					                                    a.Modified_Date AS Modified_Date,
					                                    a.FlowChart_Detail_UID AS FlowChart_Detail_UID
				                                    FROM
					                                    WIP_Change_History AS a
				                                    LEFT JOIN System_Users AS u ON a.Modified_UID = u.Account_UID
				                                    LEFT JOIN FlowChart_Detail AS d ON a.FlowChart_Detail_UID = d.FlowChart_Detail_UID
				                                    LEFT JOIN System_Function_Plant AS f ON f.System_FunPlant_UID = d.System_FunPlant_UID
				                                    LEFT JOIN FlowChart_Master AS c ON d.FlowChart_Master_UID = c.FlowChart_Master_UID
				                                    LEFT JOIN System_Project AS p ON c.Project_UID = p.Project_UID
				                                    LEFT JOIN System_Organization AS o ON p.OP_Types = o.Organization_Name
				                                    WHERE
					                                    a.Product_UID = 0
			                                    ) pp
		                                    LEFT JOIN System_OrganizationBOM AS bom ON pp.Organization_UID = bom.ChildOrg_UID
	                                    ) temp
                                    LEFT JOIN System_Organization AS tempOrg ON temp.ParentOrg_UID = tempOrg.Organization_UID
                                   WHERE
	                               1 = 1
                                       ";

            return FlowChartDetialSql;
        }


        /// <summary>
        /// 获取Wip修改记录的PPCheckSQl
        /// </summary>
        /// <returns></returns>
        private string GetGetWIPAlterDetialSqlForPPCkeck()
        {
            var strSqlPPCkeck = @"SELECT
	                                temp.Part_Types,
	                                temp.Organization_UID,
	                                temp.User_Name,
	                                temp.OP_Types,
	                                temp.FunPlant,
	                                temp.Project_Name,
	                                temp.Process,
	                                temp.Color,
	                                temp.Place,
	                                temp.Change_UID,
	                                temp.Change_Type,
	                                temp.Product_UID,
	                                temp.WIP_Old,
	                                temp.WIP_Add,
	                                temp.Comment,
	                                temp.Modified_UID,
	                                temp.Modified_Date,
	                                temp.FlowChart_Detail_UID,
	                                tempOrg.Organization_Name
                                FROM
	                                (
		                                SELECT
			                                pp.Part_Types,
			                                pp.Organization_UID,
			                                pp.User_Name,
			                                pp.OP_Types,
			                                pp.FunPlant,
			                                pp.Project as Project_Name,
			                                pp.Process,
			                                pp.Color,
			                                pp.Place,
			                                pp.Change_UID,
			                                pp.Change_Type,
			                                pp.Product_UID,
			                                pp.WIP_Old,
			                                pp.WIP_Add,
			                                pp.Comment,
			                                pp.Modified_UID,
			                                pp.Modified_Date,
			                                pp.FlowChart_Detail_UID,
			                                bom.ParentOrg_UID
		                                FROM
			                                (
				                                SELECT
					                                kk.Project AS Project,
					                                kk.Part_Types AS Part_Types,
					                                kk.FunPlant AS FunPlant,
					                                kk.Place AS Place,
					                                kk.Process AS Process,
					                                kk.Color AS Color,
					                                kk.FlowChart_Detail_UID AS FlowChart_Detail_UID,
					                                kk.FlowChart_Master_UID AS FlowChart_Master_UID,
					                                kk.Change_UID AS Change_UID,
					                                kk.Change_Type AS Change_Type,
					                                kk.Product_UID AS Product_UID,
					                                kk.WIP_Old AS WIP_Old,
					                                kk.WIP_Add AS WIP_Add,
					                                kk.Comment AS Comment,
					                                kk.Modified_UID AS Modified_UID,
					                                kk.Modified_Date AS Modified_Date,
					                                u.User_Name AS User_Name,
					                                o.Organization_UID AS Organization_UID,
					                                p.OP_Types AS OP_Types
				                                FROM
					                                (
						                                SELECT
							                                m.Project AS Project,
							                                m.Part_Types AS Part_Types,
							                                m.FunPlant AS FunPlant,
							                                m.Place AS Place,
							                                m.Process AS Process,
							                                m.Color AS Color,
							                                m.FlowChart_Master_UID AS FlowChart_Master_UID,
							                                m.FlowChart_Detail_UID AS FlowChart_Detail_UID,
							                                a.Change_UID AS Change_UID,
							                                a.Change_Type AS Change_Type,
							                                a.Product_UID AS Product_UID,
							                                a.WIP_Old AS WIP_Old,
							                                a.WIP_Add AS WIP_Add,
							                                a.Comment AS Comment,
							                                a.Modified_UID AS Modified_UID,
							                                a.Modified_Date AS Modified_Date
						                                FROM
							                                WIP_Change_History a
						                                LEFT JOIN (
							                                SELECT
								                                n.Project,
								                                n.Part_Types,
								                                n.FunPlant,
								                                n.Place,
								                                n.Process,
								                                n.Color,
								                                n.Product_UID,
								                                n.FlowChart_Master_UID,
								                                n.FlowChart_Detail_UID
							                                FROM
								                                (
									                                SELECT
										                                Project,
										                                Part_Types,
										                                FunPlant,
										                                Place,
										                                Process,
										                                Color,
										                                Product_UID,
										                                FlowChart_Master_UID,
										                                FlowChart_Detail_UID
									                                FROM
										                                Product_Input
									                                UNION ALL
										                                SELECT
											                                Project,
											                                Part_Types,
											                                FunPlant,
											                                Place,
											                                Process,
											                                Color,
											                                Product_UID,
											                                FlowChart_Master_UID,
											                                FlowChart_Detail_UID
										                                FROM
											                                Product_Input_History
								                                ) n
						                                ) m ON a.FlowChart_Detail_UID = m.FlowChart_Detail_UID
						                                WHERE
							                                a.Product_UID <> 0
					                                ) kk
				                                LEFT JOIN System_Users AS u ON kk.Modified_UID = u.Account_UID
				                                LEFT JOIN FlowChart_Master AS c ON kk.FlowChart_Master_UID = c.FlowChart_Master_UID
				                                LEFT JOIN System_Project AS p ON c.Project_UID = p.Project_UID
				                                LEFT JOIN System_Organization AS o ON p.OP_Types = o.Organization_Name
			                                ) pp
		                                LEFT JOIN System_OrganizationBOM AS bom ON pp.Organization_UID = bom.ChildOrg_UID
	                                ) temp
                                LEFT JOIN System_Organization AS tempOrg ON temp.ParentOrg_UID = tempOrg.Organization_UID
                                WHERE
	                                1 = 1";

            return strSqlPPCkeck;
        }


        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public int GetFlowChartMasterID(string BU_D_Name, string Project_Name, string Part_Types, string Product_Phase)
        {
            var query = from flow in DataContext.FlowChart_Master
                        select new
                        {
                            Part_Types = flow.Part_Types,
                            Project_Name = flow.System_Project.Project_Name,
                            Product_Phase = flow.System_Project.Product_Phase,
                            BU_D_Name = flow.System_Project.System_BU_D.BU_D_Name,
                            FlowChart_Master_UID = flow.FlowChart_Master_UID
                        };

            query = query.Where(p => p.BU_D_Name == BU_D_Name && p.Project_Name == Project_Name && p.Part_Types == Part_Types && p.Product_Phase == Product_Phase);
            return query.ToList().FirstOrDefault().FlowChart_Master_UID;
        }
    }
}

