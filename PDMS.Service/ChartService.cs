using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PDMS.Service
{
    public interface IChartService
    {
        List<string> GetFunPlant(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color);

        List<string> GetProcess(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color, string FunPlant);
        PagedListModel<NoticeVM> QueryNotice(NoticeSearch search, Page page);
        string AddNotice(Notice ent);
        string DeleteNotice(int uiid);
        string getNoticeContent(string optype);
        string getMovieUrl(string optype, int CurrentLocation);
        string GetPageSize();

        string getNoticeColor(string optype);
        List<string> getPartTypes(string project);
        PagedListModel<EboardVM> getShowContent(EboardSearchModel search, Page page);
        List<string> GetFinnalYield(string Projects);

        /// <summary>
        /// 获取不良项目的Top10的统计信息
        /// </summary>
        /// <param name="Projects"></param>
        /// <returns></returns>
        PagedListModel<TopTenQeboardModel> GetNotReachRateHeadData(string Projects, int PageNumber, int PageSize);

        /// <summary>
        /// 获取不良项目的Top10的详细数据
        /// </summary>
        /// <returns></returns>
        PagedListModel<TopTenQeboardModel> GetNotReachRateInfoData(string Projects, int PageNumber, int PageSize);

        PagedListModel<QEboardSumModel> GteQEboardSumDetailData(string Projects, int PageNumber, int PageSize);

        /// <summary>
        /// 获取品质报表汇总数据
        /// </summary>
        /// <param name="Projects"></param>
        PagedListModel<QEboardSumModel> GetStaticQESumData(string projectName, string dataTime);

        /// <summary>
        /// 获取品质报表前十大不良数据
        /// </summary>
        /// <param name="Projects"></param>
        PagedListModel<TopTenQeboardModel> GetStaticQETopTenData(string projectName, string dataTime);
    }

    public class ChartService : IChartService
    {
        #region Private interfaces properties
        private readonly IUnitOfWork unitOfWork;
        private readonly IProductInputRepository productInputRepository;
        private readonly IFlowChartDetailRepository FlowChartDetailRepository;
        private readonly INoticeRepository NoticeRepository;
        #endregion //Private interfaces properties

        #region Service constructor
        public ChartService(
        IProductInputRepository productInputRepository, IFlowChartDetailRepository FlowChartDetailRepository,
          INoticeRepository NoticeRepository, IUnitOfWork unitOfWork)
        {
            this.productInputRepository = productInputRepository;
            this.FlowChartDetailRepository = FlowChartDetailRepository;
            this.NoticeRepository = NoticeRepository;
            this.unitOfWork = unitOfWork;
        }
        #endregion //Service constructor

        public List<string> GetFunPlant(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color)
        {
            List<string> result = new List<string>();
            result.Add("ALL");
            var EnumEntity = FlowChartDetailRepository.QueryFunPlant(CustomerName, ProjectName, ProductPhaseName, PartTypesName, Color);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }

        public List<string> GetProcess(string CustomerName, string ProjectName, string ProductPhaseName,
            string PartTypesName, string Color, string FunPlant)
        {
            List<string> result = new List<string>();
            result.Add("ALL");
            var EnumEntity = FlowChartDetailRepository.QueryProcess(CustomerName, ProjectName, ProductPhaseName, PartTypesName, Color, FunPlant);
            var customerList = AutoMapper.Mapper.Map<List<string>>(EnumEntity);
            result.AddRange(customerList);
            return result;
        }

        public List<string> getPartTypes(string project)
        {
            return NoticeRepository.getPartTypes(project);
        }

        public string GetPageSize()
        {
            return NoticeRepository.GetPageSize();
        }

        public PagedListModel<NoticeVM> QueryNotice(NoticeSearch search, Page page)
        {
            var totalCount = 0;
            var list = NoticeRepository.QueryNotice(search, page, out totalCount);
            IList<NoticeVM> listDTO = new List<NoticeVM>();

            foreach (var item in list)
            {

                var ent = new NoticeVM();
                ent.Color = item.Color;
                ent.Creator_User = item.Creator_User;
                ent.Creat_Time = item.Creat_Time;
                ent.Notice_Content = item.Notice_Content;
                ent.Period = item.Start_Time.ToShortDateString() + " " + item.Start_Time.ToShortTimeString() + "~" + item.End_Time.ToShortDateString() + " " + item.End_Time.ToShortTimeString();
                ent.Scope = item.Scope;
                ent.State = item.State;
                ent.UID = item.UID;
                ent.Color = item.Color;
                ent.RepeatTime = item.RepeatTime;
                listDTO.Add(ent);

            }
            return new PagedListModel<NoticeVM>(totalCount, listDTO);
        }
        public PagedListModel<EboardVM> getShowContent(EboardSearchModel search, Page page)
        {
            var totalCount = 0;
            List<int> Projects = NoticeRepository.GetSelectMasterUid(search.selectProjects);
            List<string> FunPlants = search.selectFunplants.Split(',').ToList();
            //循环功能厂获取对应中文名称 
            List<string> FunPlantLists = new List<string>();
            foreach (var item in FunPlants)
            {
                if (item == "Surface")
                {
                    FunPlantLists.Add("表面");
                }
                else if (item == "Anode")
                {
                    FunPlantLists.Add("阳极");
                }
                else if (item == "Assembly")
                {
                    FunPlantLists.Add("组装");
                }
                else if (item == "Top")
                {
                    FunPlantLists.Add("前十低达成");
                }
                else if (item == "ALL")
                {
                    FunPlantLists.Add("全部制程");
                }
                else
                    FunPlantLists.Add(item);
            }

            List<string> partS = search.Part_Types.Split(',').ToList();
            var searchModel = new EboardS
            {
                FunPlant = FunPlantLists,
                MasterUID = Projects,
                Part_Types = partS,
                Optype = search.Optype,
                QE_location = search.QE_location == null ? "ALL" : search.QE_location
            };

            var list = NoticeRepository.getShowContent(searchModel, page, out totalCount);
            var listDTO = list.ToList();
            return new PagedListModel<EboardVM>(totalCount, listDTO);
        }
        public string getNoticeContent(string optype)
        {
            List<Notice> notices = NoticeRepository.getNoticeContent();
            string result = string.Empty;
            if (notices.Count > 0)
            {
                foreach (var item in notices)
                {
                    if (DateTime.Compare(item.Start_Time, DateTime.Now) < 0 && item.State == "未开始")
                    {
                        var dt = NoticeRepository.GetById(item.UID);
                        dt.State = "进行中";
                        NoticeRepository.Update(dt);
                        unitOfWork.Commit();
                        if (item.Scope.Trim() == optype.Trim())
                        {
                            result += item.Notice_Content + "                              ";
                        }

                    }
                    else if (DateTime.Compare(item.End_Time, DateTime.Now) < 0)
                    {
                        var dt = NoticeRepository.GetById(item.UID);
                        dt.State = "已过时";
                        NoticeRepository.Update(dt);
                        unitOfWork.Commit();
                    }
                    else if (item.Scope.Trim() == optype.Trim())
                    {
                        result += item.Notice_Content + "                                 ";
                    }

                }

            }
            return result;

        }



        public string getMovieUrl(string optype, int CurrentLocation)
        {
            List<Notice> notices = NoticeRepository.getMovieUrl();
            List<Notice> Movies = new List<Notice>();
            string result = string.Empty;
            if (notices.Count > 0)
            {
                foreach (var item in notices)
                {

                    if (DateTime.Compare(item.Start_Time, DateTime.Now) < 0 && DateTime.Compare(item.End_Time, DateTime.Now) > 0 && item.State == "未开始")
                    {
                        var dt = NoticeRepository.GetById(item.UID);
                        dt.State = "进行中";
                        NoticeRepository.Update(dt);
                        unitOfWork.Commit();
                        if (item.Scope.Trim() == optype.Trim())
                            Movies.Add(item);
                    }
                    //else if (DateTime.Compare(item.Start_Time, DateTime.Now) >0 && item.State == "未开始")
                    //{
                    //   return   item.Notice_Content.Split('$')[0]+"$"+"未开始";

                    //}

                    else if (DateTime.Compare(item.End_Time, DateTime.Now) < 0)
                    {
                        //已过时的需要先判断是否有时间间隔播放的，若不是就删除，若是就将状态改为“未开始”，并将开始结束时间加 repeatTime小时。
                        var dt = NoticeRepository.GetById(item.UID);
                        if (dt.RepeatTime == null)
                        {
                            dt.State = "已过时";
                            NoticeRepository.Update(dt);
                            unitOfWork.Commit();
                            // 删除该视频文件
                            try
                            {
                                var delFile = @dt.Notice_Content.Split('$')[1];

                                File.Delete(delFile);

                            }

                            catch
                            {

                            }
                        }
                        else
                        {
                            dt.State = "未开始";
                            dt.Start_Time = dt.Start_Time.AddHours((Double)dt.RepeatTime);
                            dt.End_Time = dt.End_Time.AddHours((Double)dt.RepeatTime);
                            NoticeRepository.Update(dt);
                            unitOfWork.Commit();
                        }


                    }
                    else if (item.State == "进行中" && item.Scope.Trim() == optype.Trim())
                    {
                        Movies.Add(item);
                    }

                }
                var sortMovies = Movies.OrderBy(A => A.Creat_Time);
                Movies = sortMovies.ToList();

                if (Movies.Count > 0)
                {
                    if (Movies.Count <= CurrentLocation)
                    {
                        result = Movies[0].Notice_Content.Split('$')[0] + "$" + "从头再播放一次";
                    }
                    if (Movies.Count > CurrentLocation)
                    {
                        result = Movies[CurrentLocation].Notice_Content.Split('$')[0];
                    }

                }
            }
            return result;

        }
        public List<string> GetFinnalYield(string Projects)
        {
            List<string> ProjectList = Projects.Split(',').ToList();
            var result = NoticeRepository.GetFinnalYield(ProjectList);
            return result;

        }

        /// <summary>
        /// 获取不良项目的Top10的统计信息
        /// </summary>
        /// <param name="Projects"></param>
        /// <returns></returns>
        public PagedListModel<TopTenQeboardModel> GetNotReachRateHeadData(string Projects, int PageNumber, int PageSize)
        {
            var totalCount = 0;
            var result = NoticeRepository.GetNotReachRateHeadData(Projects);


            var rr = result.GroupBy(p => p.Process).ToDictionary(p => p.Key, q => q);

            var resultList = new List<TopTenQeboardModel>();

            foreach (var item in rr)
            {
                var maxYield = item.Value.Max(P => P.Yield);
                foreach (var value in item.Value)
                {
                    value.HistogramRate = ((value.Yield / maxYield) * 100).ToString("F") + "%";
                }

                Int32 makeVirtualData = 0;
                if (item.Value.Count() >= 10)
                {
                    resultList.AddRange(item.Value.OrderBy(p => p.FlowChartMaster_UID).Take(10));
                    makeVirtualData = PageSize - 10;
                    resultList.AddRange(ADDMakeVirtualData(makeVirtualData));
                }
                else if (item.Value.Count() < 10)
                {
                    resultList.AddRange(item.Value.OrderBy(p => p.FlowChartMaster_UID));
                    makeVirtualData = PageSize - item.Value.Count();
                    resultList.AddRange(ADDMakeVirtualData(makeVirtualData));
                }
            }

            result = resultList.Skip(PageSize * PageNumber).Take(PageSize).ToList();
            totalCount = resultList.Count;
            return new PagedListModel<TopTenQeboardModel>(totalCount, result);
        }


        public List<TopTenQeboardModel> ADDMakeVirtualData(Int32 makeVirtualDataCount)
        {
            List<TopTenQeboardModel> list = new List<TopTenQeboardModel>();
            for (int i = 0; i < makeVirtualDataCount; i++)
            {
                list.Add(new TopTenQeboardModel()
                {
                    TopTenQeboard_UID = 0,
                    FlowChartMaster_UID = 0,
                    Project = string.Empty,
                    Part_Types = string.Empty,
                    Time_Interval = string.Empty,
                    Product_Date = string.Empty,
                    Process_Seq = 0,
                    Process = string.Empty,
                    CheckNum = 0,
                    TotolNG = 0,
                    TotalYidld = 0,
                    DefectName = string.Empty,
                    NG = 0,
                    Yield = 0,
                    DefectType = string.Empty,
                    HistogramRate = string.Empty,
                });
            }
            return list;
        }
        /// <summary>
        /// 获取不良项目的头部数据
        /// </summary>
        /// <returns></returns>
        public PagedListModel<TopTenQeboardModel> GetNotReachRateInfoData(string Projects, int PageNumber, int PageSize)
        {
            var result = NoticeRepository.GetNotReachRateInfoData(Projects);
            var totalCount = result.Count;
            //var rr=  result.GroupBy(p => p.Process).ToDictionary(p=>p.Key,p=>p);
            return new PagedListModel<TopTenQeboardModel>(totalCount, result.OrderBy(p => p.Process).Take(10));
        }

        public PagedListModel<QEboardSumModel> GteQEboardSumDetailData(string Projects, int PageNumber, int PageSize)
        {
            var totalCount = 0;
            var result = NoticeRepository.GetQEboardSumDetailData(Projects);
            var OneDirectTarget = 100.0;
            var OneDirectTargetActual = 100.0;
            var TwoDirectTarget = 100.0;
            var TwoDirectTargetActual = 100.0;

            foreach (var item in result)
            {
                OneDirectTarget *= item.OneTargetYield.Equals(0) ? 1 : item.OneTargetYield;
                OneDirectTargetActual *= item.OneYield.Equals(0) ? 1 : item.OneYield;
                TwoDirectTarget *= item.SecondTargetYield.Equals(0) ? 1 : item.SecondTargetYield;
                TwoDirectTargetActual *= item.SecondYield.Equals(0) ? 1 : item.SecondYield;
            }

            foreach (var item in result)
            {
                item.OneDirectTarget = OneDirectTarget.ToString("F2") + '%';
                item.OneDirectTargetActual = OneDirectTargetActual.ToString("F2") + '%';
                item.TwoDirectTarget = TwoDirectTarget.ToString("F2") + '%';
                item.TwoDirectTargetActual = TwoDirectTargetActual.ToString("F2") + '%';
            }
            totalCount = result.Count;
            result = result.Skip(PageSize * PageNumber).Take(PageSize).ToList();
            return new PagedListModel<QEboardSumModel>(totalCount, result);
        }

        /// <summary>
        /// 获取品质报表汇总数据
        /// </summary>
        public PagedListModel<QEboardSumModel> GetStaticQESumData(string projectName, string dataTime)
        {
            var totalCount = 0;
            var result = NoticeRepository.GetStaticQESumData(projectName, dataTime);
            var OneDirectTarget = 100.0;
            var OneDirectTargetActual = 100.0;
            var TwoDirectTarget = 100.0;
            var TwoDirectTargetActual = 100.0;

            foreach (var item in result)
            {
                OneDirectTarget *= item.OneTargetYield.Equals(0) ? 1 : item.OneTargetYield;
                OneDirectTargetActual *= item.OneYield.Equals(0) ? 1 : item.OneYield;
                TwoDirectTarget *= item.SecondTargetYield.Equals(0) ? 1 : item.SecondTargetYield;
                TwoDirectTargetActual *= item.SecondYield.Equals(0) ? 1 : item.SecondYield;
            }

            foreach (var item in result)
            {
                item.OneDirectTarget = OneDirectTarget.ToString("F2") + '%';
                item.OneDirectTargetActual = OneDirectTargetActual.ToString("F2") + '%';
                item.TwoDirectTarget = TwoDirectTarget.ToString("F2") + '%';
                item.TwoDirectTargetActual = TwoDirectTargetActual.ToString("F2") + '%';
            }

            return new PagedListModel<QEboardSumModel>(totalCount, result);
        }

        /// <summary>
        /// 获取品质报表TopTen数据
        /// </summary>
        public PagedListModel<TopTenQeboardModel> GetStaticQETopTenData(string projectName, string dataTime)
        {
            var totalCount = 0;
            var result = NoticeRepository.GetStaticQETopTenData(projectName, dataTime);

            var rr = result.GroupBy(p => p.Process).ToDictionary(p => p.Key, q => q);
            var resultList = new List<TopTenQeboardModel>();

            foreach (var item in rr)
            {
                var maxYield = item.Value.Max(P => P.Yield);
                foreach (var value in item.Value)
                {
                    value.HistogramRate = ((value.Yield / maxYield) * 100).ToString("F") + "%";
                }
                resultList.AddRange(item.Value.OrderBy(p => p.FlowChartMaster_UID).Take(5));
            }

            return new PagedListModel<TopTenQeboardModel>(totalCount, resultList);
        }

        public string getNoticeColor(string optype)
        {
            List<Notice> notices = NoticeRepository.getNotice(optype);
            if (notices.Count > 0)
            {
                return notices[0].Color;
            }

            else
                return "green";

        }


        public string AddNotice(Notice ent)
        {

            try
            {
                NoticeRepository.Add(ent);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {

                return ex.ToString();// "新增失败，请核对信息后再重试，或联系系统管理员。";
            }
            return "OK";

        }



        public string DeleteNotice(int uiid)
        {
            var item = NoticeRepository.GetById(uiid);
            try
            {
                NoticeRepository.Delete(item);
                unitOfWork.Commit();
                // 删除该视频文件
                var delFile = @item.Notice_Content.Split('$')[1];
                File.Delete(delFile);



            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "OK";

        }
    }
}
