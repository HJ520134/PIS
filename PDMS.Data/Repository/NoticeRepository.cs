using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Model.EntityDTO;
using System.Text.RegularExpressions;

namespace PDMS.Data.Repository
{
    public interface INoticeRepository : IRepository<Notice>
    {
        List<Notice> getMovieUrl();
        List<Notice> getNoticeContent();
        List<Notice> getNotice(string optype);
        IQueryable<NoticeVtual> QueryNotice(NoticeSearch search, Page page, out int count);
        List<string> getPartTypes(string project);
        string GetPageSize();
        List<int> GetSelectMasterUid(string Projects);
        IQueryable<EboardVM> getShowContent(EboardS search, Page page, out int count);
        IQueryable<Electrical_Board_DT> getAllContent(EboardSearch search, Page page, out int count);
        List<string> GetFinnalYield(List<string> Projects);

        /// <summary>
        /// 获取不良项目的Top10的统计信息
        /// </summary>
        List<TopTenQeboardModel> GetNotReachRateHeadData(string Projects);

        /// <summary>
        /// 获取不良项目的Top10的详细数据
        /// </summary>
        /// <returns></returns>
        List<TopTenQeboardModel> GetNotReachRateInfoData(string Projects);

        /// <summary>
        /// 获取电子看板汇总信息
        /// </summary>
        /// <param name="Projects"></param>
        List<QEboardSumModel> GetQEboardSumDetailData(string Projects);

        /// <summary>
        /// 获取品质报表汇总数据
        /// </summary>
        List<QEboardSumModel> GetStaticQESumData(string projectName, string dataTime);

        /// <summary>
        /// 获取品质报表TopTen数据
        /// </summary>
        List<TopTenQeboardModel> GetStaticQETopTenData(string projectName, string dataTime);
    }
    public class NoticeRepository : RepositoryBase<Notice>, INoticeRepository
    {
        private Logger log = new Logger("NoticeRepository");
        public NoticeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public List<int> GetSelectMasterUid(string Projects)
        {
            var SplitProjects = Projects.Split(',');   //先获得所有用户选项的专案信息
            List<int> result = new List<int>();
            var ProjectName = string.Empty;
            var Product_Phase = string.Empty;
            var Part_Types = string.Empty;
            foreach (string item in SplitProjects)
            {
                ProjectName = item.Split('_')[0];
                Part_Types = item.Split('_')[1];
                Product_Phase = item.Split('_')[2];
                var query = from M in DataContext.FlowChart_Master
                            join P in DataContext.System_Project on M.Project_UID equals P.Project_UID
                            where P.Project_Name == ProjectName && P.Product_Phase == Product_Phase && M.Part_Types == Part_Types
                            select M.FlowChart_Master_UID;
                result.Add(query.FirstOrDefault());
            }
            return result;
        }

        public List<string> getPartTypes(string project)
        {
            var query = from M in DataContext.FlowChart_Master
                        join P in DataContext.System_Project on M.Project_UID equals P.Project_UID
                        where P.Project_Name == project && M.Is_Latest == true
                        select M.Part_Types;
            return query.ToList();

        }
        public string GetPageSize()
        {
            var query = from enumertion in DataContext.Enumeration

                        where enumertion.Enum_Type == "EboardPageSize"
                        select enumertion.Enum_Value;

            return query.FirstOrDefault();
        }
        public List<Notice> getNoticeContent()
        {

            var query = from A in DataContext.Notice
                        where ((A.State == "进行中" || A.State == "未开始") && A.Color == "通知")
                        orderby A.Creat_Time descending
                        select A;


            return query.ToList();
        }

        public List<Notice> getMovieUrl()
        {

            var query = from A in DataContext.Notice
                        where ((A.State == "进行中" || A.State == "未开始") && A.Color == "视频")
                        orderby A.Creat_Time descending
                        select A;


            return query.ToList();
        }
        public List<Notice> getNotice(string optype)
        {

            var query = from A in DataContext.Notice
                        where (A.State == "进行中" || A.State == "未开始") && A.Scope == optype
                        orderby A.Creat_Time descending
                        select A;


            return query.ToList();
        }
        public IQueryable<NoticeVtual> QueryNotice(NoticeSearch search, Page page, out int count)
        {


            var query = from A in DataContext.Notice
                        join B in DataContext.System_Users on A.Creator_UID equals B.Account_UID
                        where A.State == "进行中" || A.State == "未开始"
                        orderby A.Creat_Time descending
                        select new NoticeVtual
                        {
                            UID = A.UID,
                            Notice_Content = A.Notice_Content,
                            Start_Time = A.Start_Time,
                            End_Time = A.End_Time,
                            Color = A.Color,
                            Scope = A.Scope,
                            State = A.State,
                            Creator_User = B.User_Name,
                            Creat_Time = A.Creat_Time,
                            RepeatTime = A.RepeatTime
                        };


            count = query.Count();

            query = query.GetPage(page);
            return query;

        }



        public IQueryable<Electrical_Board_DT> getAllContent(EboardSearch search, Page page, out int count)
        {
            var query = from A in DataContext.Electrical_Board_DT
                        where search.Project.Contains(A.Project) && A.Prouct_Plan != 0    //去掉计划为0的数据
                        select A;


            count = query.Count();

            query = query.OrderBy(m => m.Project).ThenBy(m => m.Part_Types).ThenBy(m => m.FunPlant).GetPage(page);
            return query;

        }

        public IQueryable<EboardVM> getShowContent(EboardS search, Page page, out int count)
        {
            // 查询FlowchartMasterID

            //从数据库获取pagesize

            page.PageSize = int.Parse(GetPageSize() == "" ? "11" : GetPageSize());


            //var queryOp = from B in DataContext.FlowChart_Master
            //              join C in DataContext.System_Project
            //              on B.Project_UID equals C.Project_UID
            //              where C.OP_TYPES == search.Optype && search.Project.Contains(C.Project_Name)
            //              select B.FlowChart_Master_UID;

            var listMasterUID = search.MasterUID;

            var query = from A in DataContext.Electrical_Board_DT
                       // join E in DataContext.FlowChart_IEData
                     //on A.Project_UID equals E.FlowChart_Detail_UID
                        where A.Prouct_Plan != 0 && listMasterUID.Contains(A.FlowChart_Master_UID)  //去掉计划为0的数据
                        select  A;

            //QE_location==AL分楼栋.
            query = query.Distinct();

            if (search.QE_location != "ALL")
            {
            query = query.Where(p => p.IsDiffLocation != false);
            }
            else//不分楼栋
            {
                query = query.Where(p => p.IsDiffLocation != true);
            }

            bool flag = false;
            bool AllProcess = false;
            foreach (var funPlant in search.FunPlant)
            {
                if (funPlant == "前十低达成")
                {
                    flag = true;
                }
                if (funPlant == "全部制程")
                {
                    AllProcess = true;
                }
            }
            if (!flag && !AllProcess)
            {
                query = from A in query
                        where search.FunPlant.Contains(A.FunPlant)
                        select A;
            }
            List<EboardVM> ResultList = new List<EboardVM>();
            List<EboardVM> FuncResultList = new List<EboardVM>();

            foreach (var item in query)
            {
                EboardVM dt = new EboardVM();
                dt.Adjust_QTY = item.Adjust_QTY;
                dt.Board_UID = item.Board_UID;
                dt.DRI = item.DRI;
                dt.FlowChart_Master_UID = item.FlowChart_Master_UID;
                dt.FunPlant = item.FunPlant;
                dt.Good_QTY = item.Good_QTY;
                dt.place = item.Place;
                if ((item.WH_QTY + item.Good_QTY + item.NG_QTY) == 0)
                {
                    dt.Good_yield = "100.00";
                    dt.GoodColor = "white";
                }
                else
                {
                    var yield = Math.Round(((float)(item.Good_QTY + item.WH_QTY) / (item.WH_QTY + item.Good_QTY + item.NG_QTY) * 100.00), 2);
                    if (yield < item.Target_Yield * 100)
                    {
                        dt.GoodColor = "rgba(255, 0, 0, 0.44)";

                    }
                    else dt.GoodColor = "white";
                    dt.Good_yield = String.Format("{0:N2}", yield) + "";
                }

                dt.NG_QTY = item.NG_QTY;
                dt.Part_Types = item.Part_Types;
                dt.Picking_QTY = item.Picking_QTY;

                if (string.IsNullOrWhiteSpace(item.Color))
                {
                    dt.Process = item.Process;
                }
                else

                    dt.Process = item.Process + "_" + item.Color;
                dt.Color = item.Color;


                dt.Process_Seq = item.Process_Seq;
                dt.Project = item.Project;

                if (item.Flag == "全天累计")
                {
                    dt.Prouct_Plan = item.Prouct_Plan;
                }
                else
                    dt.Prouct_Plan = item.Prouct_Plan / 12;

                dt.Target_Yield = String.Format("{0:N2}", item.Target_Yield * 100) + "";

                if (item.Prouct_Plan / 12 == 0)
                {
                    dt.Reach_yield = "100.00";
                    dt.ReachColor = "white";
                }
                else
                {

                    var yield = Math.Round(((float)(item.Good_QTY + item.WH_QTY) / (dt.Prouct_Plan)) * 100, 2);
                    if (yield < 95)
                    {
                        dt.ReachColor = "rgba(255, 0, 0, 0.44)";
                    }
                    else if (yield < 100)
                    {
                        dt.ReachColor = "yellow";
                    }
                    else
                        dt.ReachColor = "white";
                    dt.Reach_yield = String.Format("{0:N2}", yield) + "";
                    dt.yield = yield;
                }

                dt.Time_Interval = item.Time_Interval;
                dt.WH_Picking_QTY = item.WH_Picking_QTY;
                dt.WH_QTY = item.WH_QTY;
                dt.WIP_QTY = item.WIP_QTY;
                dt.flag = item.Flag;
                ResultList.Add(dt);
              //  if (AllProcess == true && string.IsNullOrEmpty(item.Flag))  //用于无锡战情室
                    if (AllProcess == true )  //成都要求修改，这里只显示全天汇总数据
                    {
                    FuncResultList.Add(dt);
                }
                else if (search.FunPlant.Contains(item.FunPlant))
                {
                    FuncResultList.Add(dt);
                }
            }

            if (search.QE_location != "ALL" &&search.QE_location!= "//" && search.QE_location  !=null && search.QE_location !=" ")
            {
                search.QE_location = search.QE_location.Replace('/', ' ').Trim();
                FuncResultList = FuncResultList.Where(p => p.place == search.QE_location).ToList();
                ResultList = ResultList.Where(p => p.place == search.QE_location).ToList();
            }

            List<EboardVM> FinnalVM = new List<EboardVM>();
            if (AllProcess == true)  //用于无锡战情室
            {
                FuncResultList = FuncResultList.AsQueryable().OrderBy(m => m.Project).ThenBy(m => m.Part_Types).ThenBy(m => m.Color).ThenBy(m => m.Process_Seq).ToList();
               
                var query1 = from A in DataContext.Electrical_Board_DT
                             where A.Prouct_Plan != 0 && listMasterUID.Contains(A.FlowChart_Master_UID)  //去掉计划为0的数据
                             select A.Color;
                var colors = query1.Distinct(); //获取所有颜色
                foreach (var color in colors)
                {
                    var currentF = FuncResultList.Where(m => m.Color == color).ToList();
                    FinnalVM.AddRange(currentF);
                    int CC = currentF.Count();
                    if (CC == 0) continue;
                    ;
                    int Remainder = 0;
                    if (CC > page.PageSize)
                    {
                        Remainder = page.PageSize - CC % page.PageSize;
                    }
                    else
                    {
                        Remainder = page.PageSize - CC;
                    }

                    if (Remainder > 0)
                    {
                        FinnalVM = AddItems(FinnalVM, Remainder);
                    }
                }
                var result1 = FinnalVM.Distinct().AsQueryable();


                count = result1.Count();

                result1 = result1.GetPage(page);
                return result1;
            }
            else
            {
                FuncResultList = FuncResultList.AsQueryable().OrderBy(m => m.Project).ThenBy(m => m.Part_Types).ThenBy(m => m.Color).ThenBy(m => m.FunPlant).ThenBy(m => m.Color).ThenBy(m => m.Process_Seq).ToList();

                foreach (var P in search.MasterUID)
                {
                    var query1 = from A in DataContext.Electrical_Board_DT
                                 where A.Prouct_Plan != 0 && A.FlowChart_Master_UID == P    //去掉计划为0的数据
                                 select A.Color;
                    var colors = query1.Distinct(); //获取所有颜色
                    if (colors.Count() == 1)
                    {
                        foreach (var F in search.FunPlant)
                        {
                            foreach (var color in colors)
                            {

                                for (int i = 0; i < 2; i++)
                                {

                                    List<EboardVM> currentF = new List<EboardVM>();
                                    if (i == 1)
                                    {
                                        currentF = FuncResultList.Where(R => R.FlowChart_Master_UID == P && (R.Color == color || R.Color == "") && R.FunPlant == F && R.flag == "全天累计").ToList();
                                    }
                                    else
                                    {
                                        currentF = FuncResultList.Where(R => R.FlowChart_Master_UID == P && (R.Color == color || R.Color == "") && R.FunPlant == F && R.flag != "全天累计").ToList();
                                    }

                                    FinnalVM.AddRange(currentF);
                                    int CC = currentF.Count();
                                    if (CC == 0) continue;
                                    ;
                                    int Remainder = 0;
                                    if (CC > page.PageSize)
                                    {
                                        Remainder = page.PageSize - CC % page.PageSize;
                                    }
                                    else
                                    {
                                        Remainder = page.PageSize - CC;
                                    }

                                    if (Remainder > 0)
                                    {
                                        FinnalVM = AddItems(FinnalVM, Remainder);
                                    }
                                }
                            }

                        }

                    }
                    else
                    {
                        foreach (var F in search.FunPlant)
                        {
                            foreach (var color in colors)
                            {
                                if (color == "") continue;
                                for (int i = 0; i < 2; i++)
                                {

                                    List<EboardVM> currentF = new List<EboardVM>();
                                    if (i == 1)
                                    {
                                        currentF = FuncResultList.Where(R => R.FlowChart_Master_UID == P && (R.Color == color || R.Color == "") && R.FunPlant == F && R.flag == "全天累计").ToList();
                                    }
                                    else
                                    {
                                        currentF = FuncResultList.Where(R => R.FlowChart_Master_UID == P && (R.Color == color || R.Color == "") && R.FunPlant == F && R.flag != "全天累计").ToList();
                                    }

                                    FinnalVM.AddRange(currentF);
                                    int CC = currentF.Count();
                                    if (CC == 0) continue;
                                    ;
                                    int Remainder = 0;
                                    if (CC > page.PageSize)
                                    {
                                        Remainder = page.PageSize - CC % page.PageSize;
                                    }
                                    else
                                    {
                                        Remainder = page.PageSize - CC;
                                    }

                                    if (Remainder > 0)
                                    {
                                        FinnalVM = AddItems(FinnalVM, Remainder);
                                    }
                                }
                            }

                        }
                    }
                    if (flag)
                    {

                        for (int i = 0; i < 2; i++)
                        {
                            List<EboardVM> Current = new List<EboardVM>();
                            List<EboardVM> list2 = new List<EboardVM>();
                            if (i == 1)
                            {
                                list2 = (from A in ResultList
                                         where A.FlowChart_Master_UID == P && A.flag == "全天累计"
                                         orderby A.yield
                                         select A).Take(10).ToList();
                            }
                            else
                            {
                                list2 = (from A in ResultList
                                         where A.FlowChart_Master_UID == P && A.flag != "全天累计"
                                         orderby A.yield
                                         select A).Take(10).ToList();

                            }


                            foreach (var item in list2)
                            {
                                var dt = new EboardVM();
                                dt.Color = item.Color;
                                dt.Color = item.Color;
                                dt.Adjust_QTY = item.Adjust_QTY;
                                dt.Board_UID = item.Board_UID;
                                dt.DRI = item.DRI;
                                dt.FlowChart_Master_UID = item.FlowChart_Master_UID;
                                dt.yield = item.yield;
                                dt.FunPlant = "The ten worst achieving rate process";
                                dt.Good_QTY = item.Good_QTY;
                                dt.GoodColor = item.GoodColor;
                                dt.Good_yield = item.Good_yield;
                                dt.NG_QTY = item.NG_QTY;
                                dt.Part_Types = item.Part_Types;
                                dt.Picking_QTY = item.Picking_QTY;
                                dt.Process = item.Process;
                                dt.Process_Seq = item.Process_Seq;
                                dt.Project = item.Project;
                                dt.Prouct_Plan = item.Prouct_Plan;
                                dt.Target_Yield = item.Target_Yield;
                                dt.ReachColor = item.ReachColor;
                                dt.Reach_yield = item.Reach_yield;
                                dt.Time_Interval = item.Time_Interval;
                                dt.WH_Picking_QTY = item.WH_Picking_QTY;
                                dt.WH_QTY = item.WH_QTY;
                                dt.WIP_QTY = item.WIP_QTY;
                                if (i == 1)
                                    dt.flag = "全天累计";
                                Current.Add(dt);
                            }
                            FinnalVM.AddRange(Current);
                            if (page.PageSize == 10) continue;

                            int Remainder1 = 0;
                            if (page.PageSize > 10)
                            {
                                Remainder1 = page.PageSize - 10;
                            }
                            else
                            {
                                Remainder1 = page.PageSize - 10 % page.PageSize;
                            }
                            FinnalVM = AddItems(FinnalVM, Remainder1);
                        }

                    }
                }
            }

            var result = FinnalVM.Distinct().AsQueryable();


            count = result.Count();

            result = result.GetPage(page);
            return result;

        }

        private List<EboardVM> AddItems(List<EboardVM> inEboardVms, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var dt = new EboardVM();
                dt.Color = "";
                dt.Adjust_QTY = 0;
                dt.Board_UID = 0;
                dt.DRI = "";
                dt.FlowChart_Master_UID = 0;
                dt.FunPlant = "ADDValue";
                dt.yield = 0.00;
                dt.Good_QTY = 0;
                dt.GoodColor = "";
                dt.Good_yield = "";
                dt.NG_QTY = 0;
                dt.Part_Types = "";
                dt.Picking_QTY = 0;
                dt.Process = "";
                dt.Process_Seq = 0;
                dt.Project = "";
                dt.Prouct_Plan = 0;
                dt.Target_Yield = "";
                dt.ReachColor = "";
                dt.Reach_yield = "";
                dt.Time_Interval = "";
                dt.WH_Picking_QTY = 0;
                dt.WH_QTY = 0;
                dt.WIP_QTY = 0;

                inEboardVms.Add(dt);
            }

            return inEboardVms;
        }

        public int getMasterUID(string projectName, string Part_Types, string Phase)
        {
            var query = from FM in DataContext.FlowChart_Master
                        where FM.System_Project.Project_Name == projectName
                  && FM.Part_Types == Part_Types && FM.Product_Phase == Phase
                        select FM.FlowChart_Master_UID;
            return query.FirstOrDefault();
        }


        public List<string> GetFinnalYield(List<string> Projects)
        {
            List<string> FinnalYield = new List<string>();
            foreach (var project in Projects)
            {    //Paris_Paris_MP

                var projectName = project.Split('_')[0];
                var projectPT = project.Split('_')[1];
                var projectPS = project.Split('_')[2];


                int flowchartMastrUID = getMasterUID(projectName, projectPT, projectPS);

                var query1 = from A in DataContext.Electrical_Board_DT
                             where A.Prouct_Plan != 0 && flowchartMastrUID == A.FlowChart_Master_UID  //去掉计划为0的数据
                             select A.Color;
                var colors = query1.Distinct(); //获取所有颜色
                if (colors.Count() == 1)
                {
                    foreach (var color in colors)
                    {
                        if (color == "汇总") continue;
                        float yield = 1;
                        var queryTemp = from A in DataContext.Electrical_Board_DT
                                        where A.FlowChart_Master_UID == flowchartMastrUID && A.Flag == "全天累计" && (A.Color == color || A.Color == "")
                                        select A;
                        foreach (var item in queryTemp)
                        {
                            if ((item.WH_QTY + item.Good_QTY + item.NG_QTY) != 0 && (item.Good_QTY + item.WH_QTY) != 0)
                            {

                                yield = yield * (float)(item.Good_QTY + item.WH_QTY) / (float)(item.WH_QTY + item.Good_QTY + item.NG_QTY);
                            }
                        }

                        FinnalYield.Add(project + ',' + color + "," + "   FPY： " + String.Format("{0:N2}", Math.Round((yield * 100), 2)) + "%");
                    }
                }
                else
                {
                    foreach (var color in colors)
                    {
                        if (color == "汇总" || color == "") continue;
                        float yield = 1;
                        var queryTemp = from A in DataContext.Electrical_Board_DT
                                        where A.FlowChart_Master_UID == flowchartMastrUID && A.Flag == "全天累计" && (A.Color == color || A.Color == "")
                                        select A;
                        foreach (var item in queryTemp)
                        {
                            if ((item.WH_QTY + item.Good_QTY + item.NG_QTY) != 0 && (item.Good_QTY + item.WH_QTY) != 0)
                            {

                                yield = yield * (float)(item.Good_QTY + item.WH_QTY) / (float)(item.WH_QTY + item.Good_QTY + item.NG_QTY);
                            }
                        }
                        FinnalYield.Add(project + ',' + color + "," + "   FPY： " + String.Format("{0:N2}", Math.Round((yield * 100), 2)) + "%");
                    }
                }

            }
            return FinnalYield;
        }

        /// <summary>
        /// 获取不良项目的Top10的统计信息
        /// </summary>
        /// <param name="Projects"></param>
        /// <returns></returns>
        public List<TopTenQeboardModel> GetNotReachRateHeadData(string Projects)
        {
            string sql = @" SELECT 
	                        TopTenQeboard_UID,
	                        FlowChartMaster_UID,
	                        Project,
	                        Part_Types,
	                        Time_Interval,
	                        Product_Date,
	                        Process_Seq,
	                        Process,
	                        CheckNum,
	                        TotolNG,
	                        TotalYidld,
	                        DefectName,
	                        NG,
	                        Yield,
	                        DefectType
                            FROM [dbo].[TopTenQeboard] ";
            sql = sql + string.Format("where Project=N'{0}' ORDER BY Process_Seq,TopTenQeboard_UID ", Projects);
            var dblist = DataContext.Database.SqlQuery<TopTenQeboardModel>(sql).ToList();
            return dblist;
        }

        /// <summary>
        /// 获取不良项目的Top10的详细数据
        /// </summary>
        /// <returns></returns>
        public List<TopTenQeboardModel> GetNotReachRateInfoData(string Projects)
        {
            string sql = @"SELECT top 1
	                        TopTenQeboard_UID,
	                        FlowChartMaster_UID,
	                        Project,
	                        Part_Types,
	                        Time_Interval,
	                        Product_Date,
	                        Process_Seq,
	                        Process,
	                        CheckNum,
	                        TotolNG,
	                        TotalYidld,
	                        DefectName,
	                        NG,
	                        Yield,
	                        DefectType
                            FROM [dbo].[TopTenQeboard] ";
            sql = sql + string.Format("where Project=N'{0}'", Projects);
            var dblist = DataContext.Database.SqlQuery<TopTenQeboardModel>(sql).ToList();
            return dblist;
        }

        /// <summary>
        /// 获取汇总信息
        /// </summary>
        /// <param name="Projects"></param>
        /// <returns></returns>
        public List<QEboardSumModel> GetQEboardSumDetailData(string Projects)
        {
            string sql = @" SELECT
	                        [QEboadSum_UID],
	                        [FlowChartMaster_UID],
	                        [Project],
	                        [Part_Types],
	                        [Product_Date],
	                        [Time_Interval],
	                        [Process_Seq],
	                        [Process],
	                        [OneCheck_QTY],
	                        [OneCheck_OK],
	                        [NGReuse],
	                        [NGReject],
	                        [OneTargetYield],
	                        [OneYield],
	                        [RepairOK],
	                        [SecondTargetYield],
	                        [SecondYield]
                            FROM
	                        [dbo].[QEboardSum]";
            sql += $" where Project=N'{Projects}' ORDER BY Process_Seq";
            var dblist = DataContext.Database.SqlQuery<QEboardSumModel>(sql).ToList();
            return dblist;
        }

        /// <summary>
        /// 获取品质报表汇总数据
        /// </summary>
        public List<QEboardSumModel> GetStaticQESumData(string projectName, string dataTime)
        {
            string sql = @" SELECT
	                        [QTrace_Sum_UID],
	                        [FlowChartMaster_UID],
	                        [Project],
	                        [Part_Types],
	                        [Product_Date],
	                        [Time_Interval],
	                        [Process_Seq],
	                        [Process],
	                        [OneCheck_QTY],
	                        [OneCheck_OK],
	                        [NGReuse],
	                        [NGReject],
	                        [OneTargetYield],
	                        [OneYield],
	                        [RepairOK],
	                        [SecondTargetYield],
	                        [SecondYield]
                            FROM
	                        [dbo].[QTrace_Sum]";
            sql += $" where Project=N'{projectName}' and Product_Date='{dataTime}' ORDER BY Process_Seq";
            var dblist = DataContext.Database.SqlQuery<QEboardSumModel>(sql).ToList();
            return dblist;
        }


        /// <summary>
        /// 获取品质报表TopTen数据
        /// </summary>
        public List<TopTenQeboardModel> GetStaticQETopTenData(string projectName, string dataTime)
        {
            string sql = @" SELECT 
	                        QTrace_TopTen_Sum_UID,
	                        FlowChartMaster_UID,
	                        Project,
	                        Part_Types,
	                        Time_Interval,
	                        Product_Date,
	                        Process_Seq,
	                        Process,
	                        CheckNum,
	                        TotolNG,
	                        TotalYidld,
	                        DefectName,
	                        NG,
	                        Yield,
	                        DefectType
                            FROM
                           [dbo].[QTrace_TopTen_Sum] ";
            sql += $" where Project=N'{projectName}' and Product_Date='{dataTime}' ORDER BY Process_Seq,QTrace_TopTen_Sum_UID";
            var dblist = DataContext.Database.SqlQuery<TopTenQeboardModel>(sql).ToList();
            return dblist;
        }
    }
}
