using PDMS.Data.Infrastructure;
using PDMS.Data.Repository;
using PDMS.Model.ViewModels.QualtyTrace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Data;
using System.Transactions;

namespace PDMS.Service
{
    public interface IQualityTraceService
    {
        void getTraceData();
    }
   public class QualityTraceService
    {
        #region Private interfaces properties

        private readonly IUnitOfWork unitOfWork;
   
        private readonly IEnumerationRepository enumerationRepository;
        private readonly IQEboadSumRepository qEboadSumRepository;
        private readonly ITopTenQeboardRepository topTenQeboardRepository;

        private readonly IQTrace_TopTen_SumRepository qTrace_TopTen_SumRepository;
        private readonly IQTrace_SumRepository qTrace_SumRepository;
        #endregion //Private interfaces properties

        #region Service constructor

        public QualityTraceService(
       
            IEnumerationRepository enumerationRepository,
              IQEboadSumRepository qEboadSumRepository,
                ITopTenQeboardRepository topTenQeboardRepository,
                IQTrace_SumRepository qTrace_SumRepository,
                IQTrace_TopTen_SumRepository qTrace_TopTen_SumRepository,
            IUnitOfWork unitOfWork)

        {
           
            this.unitOfWork = unitOfWork;
            this.enumerationRepository = enumerationRepository;
            this.qEboadSumRepository = qEboadSumRepository;
            this.topTenQeboardRepository = topTenQeboardRepository;
            this.qTrace_SumRepository = qTrace_SumRepository;
            this.qTrace_TopTen_SumRepository = qTrace_TopTen_SumRepository;

        }

        #endregion //Service constructor


        /// <summary>
        /// 获取trace系统数据
        /// </summary>
        public void getTraceData()
        {
            //先获取当前OP，根据OP获取对应的当前时段，通过当前时段进行获取。
            //通过Enum表获取要获取的专案 V16BG
            int Flowchart_MasterUID = int.Parse(enumerationRepository.GetEnumValuebyType("TraceProjectName").FirstOrDefault());
            //通过UID获取专案信息。
            var ProjectInfo = enumerationRepository.GetProjectInfo(Flowchart_MasterUID);
            //通过当前时间判断是否大于8：30,大于8：30为今天，否则为昨天。
            //1获取当前日期
            DateTime CurrentDay = DateTime.Now.Date;
            //2 获取当然时间  
            int CurrentHours = DateTime.Now.Hour;
            int CurrentMintus = DateTime.Now.Minute;
            string ProductDate = CurrentDay.ToShortDateString();
            string StartTime;
            string EndTime = CurrentDay.AddDays(1).ToShortDateString() + " " + "06:00";
            if (CurrentHours < 6 || (CurrentHours == 6 && CurrentMintus >= 30))
            {
                ProductDate = CurrentDay.AddDays(-1).ToShortDateString();
                EndTime = CurrentDay.ToShortDateString() + " " + "06:00";
            }
            StartTime = ProductDate + " " + "06:00";
            //通过UID获取制程计划
            List<ProcessTargetInfo> ProcessTargetInfoLists = enumerationRepository.GetProcessTargetInfo(Flowchart_MasterUID ,ProductDate);

            string timeInterval = "06:00-" + DateTime.Now.ToString("HH:mm");
            //string CurrentOP = "OP3";
            //var IntervalInfo=    enumerationRepository.GetIntervalInfo(CurrentOP);
            //string Interval=IntervalInfo.FirstOrDefault().Time_Interval;
            //string StartTime = IntervalInfo.FirstOrDefault().NowDate +" "+ Interval.Substring(0, 5);
            //string EndTime = IntervalInfo.FirstOrDefault().NowDate + " "  + Interval.Substring(6);

            string APIurl= System.Configuration.ConfigurationManager.AppSettings["APIUrl"];

            var result = HttpPost(APIurl, "{\"productName\":\"" + ProjectInfo.Project + "\",\"startTime\":\"" + StartTime + "\",\"endTime\":\"" + EndTime + "\"} ");
            //var result = HttpPost("http://10.128.19.52:8808/api/qed", "{\"productName\":\"" + ProjectInfo.Project + "\",\"startTime\":\"" + "2018-01-23 08:00" + "\",\"endTime\":\"" + "2018-01-24 08:00" + "\"} ");


            TraceModelLists itemLists = JsonConvert.DeserializeObject<TraceModelLists>(result);

            //List<QTraceItem> itemLists1 = JsonConvert.DeserializeObject<List<QTraceItem>>(result.Substring( 8,result.Length-9));

            //根据设置获取当前班次生词设定的颜色。

            ///循环每个制程数据将相应的信息填入两个数据库表中
            ///
            if (itemLists.data.Count > 0)
            {
                //
                using (TransactionScope scope = new TransactionScope())
                {
                    //先删除该专案的所有信息

                    enumerationRepository.DeleteEboardData(ProjectInfo.Project);
                    List<TopTenQeboard> TopTenList = new List<TopTenQeboard>();

                    foreach (var item in itemLists.data)
                {
                        //获取对应在战情报表中对应制程名。

                        string Process = enumerationRepository.GetMappingName(item.proccess);
                        //获取制程对应的制程序号
                        //没有数据不显示
                        if (item.freshOK+item.ngFail+item.ngScrap+item.reworkOK == 0)
                        {
                            continue;
                        }
                        QEboardSum EboadSum = new QEboardSum();

                    EboadSum.Modified_Date = DateTime.Now;
                    EboadSum.Product_Date = ProductDate;
                    EboadSum.Process_Seq = enumerationRepository.GetProcessSeq(Process, Flowchart_MasterUID);
                    EboadSum.Process = Process;
                    EboadSum.Project = ProjectInfo.Project;
                    EboadSum.Part_Types = ProjectInfo.Part_Types;
                    EboadSum.FlowChartMaster_UID = Flowchart_MasterUID;
                    //一次检验数：	一次OK数  +  NG可重工数  +  NG报废数  +  返修OK数
                    //EboadSum. = item.freshOK + item.ngFail + item.ngScrap + item.reworkOK;
                    EboadSum.NGReject = item.ngScrap;
                    EboadSum.NGReuse = item.ngFail;
                    EboadSum.OneCheck_OK = item.freshOK;
                    EboadSum.OneCheck_QTY = item.freshOK + item.ngFail + item.ngScrap + item.reworkOK;
                    //通过检测系统中是否有该制程的目标良率，有就返回，没有为0
                  
                    EboadSum.OneTargetYield = (double)(ProcessTargetInfoLists.Find(C => C.Process == Process) == null ? 0 : ProcessTargetInfoLists.Find(C => C.Process == Process).FirstTargetYield);
                    //一次良率=一次OK/一次检验数
                  
                    EboadSum.OneYield = Math.Round((item.freshOK * 1.0) / (EboadSum.OneCheck_QTY * 1.0),8);
                    EboadSum.RepairOK = item.reworkOK;
                    EboadSum.SecondTargetYield = (double)(ProcessTargetInfoLists.Find(C => C.Process == Process) == null ? 0 : ProcessTargetInfoLists.Find(C => C.Process == Process).SecondTargetYield);

                    //二次良率   待定
                    EboadSum.SecondYield = Math.Round(((item.freshOK + item.reworkOK) * 1.0) / ((item.freshOK + item.reworkOK + item.ngFail + item.ngScrap) * 1.0),8);
                    EboadSum.Time_Interval = timeInterval;
                    //将数据插入到数据库
                    qEboadSumRepository.Add(EboadSum);
                      
                        //将TOP10数据写入到数据库
                        foreach (var Detail in item.issue)
                    {
                        TopTenQeboard TopItem = new TopTenQeboard();
                        TopItem.Process = Process;
                        TopItem.Process_EN = Detail.item_en;
                        TopItem.Project = ProjectInfo.Project;
                        TopItem.Part_Types = ProjectInfo.Part_Types;
                        TopItem.FlowChartMaster_UID = Flowchart_MasterUID;
                        TopItem.Process_Seq = enumerationRepository.GetProcessSeq(Process, Flowchart_MasterUID);
                        TopItem.Product_Date = ProductDate;
                        TopItem.Time_Interval = timeInterval;
                        TopItem.Modified_Date = DateTime.Now;
                        TopItem.NG = Detail.count;
                            // NG汇总 待定 需不需要包含 NG重工数 ----- 不需要
                        TopItem.TotolNG = item.ngScrap + item.ngFail;
                            //检验数需要待定 良品+NG

                        TopItem.CheckNum = item.freshOK + item.ngFail + item.ngScrap + item.reworkOK;
                        TopItem.TotalYidld = Math.Round((TopItem.TotolNG * 1.0) / (TopItem.CheckNum * 1.0),8);
                        TopItem.DefectName = Detail.item_ch;
                        // 不良类型需要待定，需要将所有不良类型导入系统，且和trace系统中不良类型相同。
                        TopItem.DefectType = "外观";
                        TopItem.Yield = Math.Round( (Detail.count * 1.0) / (1.0 * TopItem.CheckNum),8);
                      //  topTenQeboardRepository.Add(TopItem);
                        TopTenList.Add(TopItem);
                        }
                       
                    }
                    topTenQeboardRepository.AddList(TopTenList);
                    unitOfWork.Commit();
                scope.Complete();
            }
            }

        }

        public string HttpPost(string url, string body)
        {
           
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";

            byte[] buffer = encoding.GetBytes(body);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// 获取trace系统汇总数据 ， 每天汇总一分数据
        /// </summary>
        public void getTraceSumData()
        {
            //先获取当前OP，根据OP获取对应的当前时段，通过当前时段进行获取。
            //通过Enum表获取要获取的专案 V16BG
            int Flowchart_MasterUID = int.Parse(enumerationRepository.GetEnumValuebyType("TraceProjectName").FirstOrDefault());
            //通过UID获取专案信息。
            var ProjectInfo = enumerationRepository.GetProjectInfo(Flowchart_MasterUID);
            //通过当前时间判断是否大于8：30,大于8：30为今天，否则为昨天。
            //1获取当前日期
            DateTime CurrentDay = DateTime.Now.Date;
            //2 获取当然时间  
            int CurrentHours = DateTime.Now.Hour;
            int CurrentMintus = DateTime.Now.Minute;
            //生产日期为昨天
            string ProductDate = CurrentDay.AddDays(-1).ToShortDateString();
            string StartTime;
            string EndTime = CurrentDay.ToShortDateString() + " " + "06:00";
            //if (CurrentHours < 6 || (CurrentHours == 6 && CurrentMintus >= 30))
            //{
            //    ProductDate = CurrentDay.AddDays(-1).ToShortDateString();
            //    EndTime = CurrentDay.ToShortDateString() + " " + "06:00";
            //}
            StartTime = ProductDate + " " + "06:00";
            //通过UID获取制程计划
            List<ProcessTargetInfo> ProcessTargetInfoLists = enumerationRepository.GetProcessTargetInfo(Flowchart_MasterUID, ProductDate);

            string timeInterval = "06:00-" + DateTime.Now.ToString("HH:mm");
            //string CurrentOP = "OP3";
            //var IntervalInfo=    enumerationRepository.GetIntervalInfo(CurrentOP);
            //string Interval=IntervalInfo.FirstOrDefault().Time_Interval;
            //string StartTime = IntervalInfo.FirstOrDefault().NowDate +" "+ Interval.Substring(0, 5);
            //string EndTime = IntervalInfo.FirstOrDefault().NowDate + " "  + Interval.Substring(6);

            string APIurl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];

            var result = HttpPost(APIurl, "{\"productName\":\"" + ProjectInfo.Project + "\",\"startTime\":\"" + StartTime + "\",\"endTime\":\"" + EndTime + "\"} ");
            //var result = HttpPost("http://10.128.19.52:8808/api/qed", "{\"productName\":\"" + ProjectInfo.Project + "\",\"startTime\":\"" + "2018-01-23 08:00" + "\",\"endTime\":\"" + "2018-01-24 08:00" + "\"} ");


            TraceModelLists itemLists = JsonConvert.DeserializeObject<TraceModelLists>(result);

            //List<QTraceItem> itemLists1 = JsonConvert.DeserializeObject<List<QTraceItem>>(result.Substring( 8,result.Length-9));

            //根据设置获取当前班次生词设定的颜色。

            ///循环每个制程数据将相应的信息填入两个数据库表中
            ///
            if (itemLists.data.Count > 0)
            {
                //
                using (TransactionScope scope = new TransactionScope())
                {
                   
                    foreach (var item in itemLists.data)
                    {
                        //没有数据不显示
                        if (item.freshOK + item.ngFail + item.ngScrap + item.reworkOK == 0)
                        {
                            continue;
                        }

                        //获取对应在战情报表中对应制程名。

                        string Process = enumerationRepository.GetMappingName(item.proccess);
                        //获取制程对应的制程序号

                        QTrace_Sum EboadSum = new QTrace_Sum();

                        EboadSum.Modified_Date = DateTime.Now;
                        EboadSum.Product_Date = ProductDate;
                        EboadSum.Process_Seq = enumerationRepository.GetProcessSeq(Process, Flowchart_MasterUID);
                        EboadSum.Process = Process;
                        EboadSum.Project = ProjectInfo.Project;
                        EboadSum.Part_Types = ProjectInfo.Part_Types;
                        EboadSum.FlowChartMaster_UID = Flowchart_MasterUID;
                        //一次检验数：	一次OK数  +  NG可重工数  +  NG报废数  +  返修OK数
                        //EboadSum. = item.freshOK + item.ngFail + item.ngScrap + item.reworkOK;
                        EboadSum.NGReject = item.ngScrap;
                        EboadSum.NGReuse = item.ngFail;
                        EboadSum.OneCheck_OK = item.freshOK;
                        EboadSum.OneCheck_QTY = item.freshOK + item.ngFail + item.ngScrap + item.reworkOK;
                        //通过检测系统中是否有该制程的目标良率，有就返回，没有为0

                        EboadSum.OneTargetYield = (double)(ProcessTargetInfoLists.Find(C => C.Process == Process) == null ? 0 : ProcessTargetInfoLists.Find(C => C.Process == Process).FirstTargetYield);
                        //一次良率=一次OK/一次检验数
                      
                        EboadSum.OneYield = Math.Round((item.freshOK * 1.0) / (EboadSum.OneCheck_QTY * 1.0), 8);
                        EboadSum.RepairOK = item.reworkOK;
                        EboadSum.SecondTargetYield = (double)(ProcessTargetInfoLists.Find(C => C.Process == Process) == null ? 0 : ProcessTargetInfoLists.Find(C => C.Process == Process).SecondTargetYield);

                        //二次良率   待定
                        EboadSum.SecondYield = Math.Round(((item.freshOK + item.reworkOK) * 1.0) / ((item.freshOK + item.reworkOK + item.ngFail + item.ngScrap) * 1.0), 8);
                        EboadSum.Time_Interval = timeInterval;
                        //将数据插入到数据库
                        qTrace_SumRepository.Add(EboadSum);

                        //将TOP10数据写入到数据库
                        foreach (var Detail in item.issue)
                        {
                            QTrace_TopTen_Sum TopItem = new QTrace_TopTen_Sum();
                            TopItem.Process = Process;
                            TopItem.DefectName_EN = Detail.item_en;
                            TopItem.Project = ProjectInfo.Project;
                            TopItem.Part_Types = ProjectInfo.Part_Types;
                            TopItem.FlowChartMaster_UID = Flowchart_MasterUID;
                            TopItem.Process_Seq = enumerationRepository.GetProcessSeq(Process, Flowchart_MasterUID);
                            TopItem.Product_Date = ProductDate;
                            TopItem.Time_Interval = timeInterval;
                            TopItem.Modified_Date = DateTime.Now;
                            TopItem.NG = Detail.count;
                            // NG汇总 待定 需要包含 NG重工数
                            TopItem.TotolNG = item.ngScrap+item.ngFail;
                            //检验数需要待定 良品+NG
                            TopItem.CheckNum = item.freshOK + item.ngFail + item.ngScrap + item.reworkOK;
                            TopItem.TotalYidld = Math.Round((TopItem.TotolNG * 1.0) / (TopItem.CheckNum * 1.0), 8);
                            TopItem.DefectName = Detail.item_ch;
                            // 不良类型需要待定，需要将所有不良类型导入系统，且和trace系统中不良类型相同。
                            TopItem.DefectType = "外观";
                            TopItem.Yield = Math.Round((Detail.count * 1.0) / (1.0 * TopItem.CheckNum), 8);
                            qTrace_TopTen_SumRepository.Add(TopItem);
                        }
                    }
                    unitOfWork.Commit();
                    scope.Complete();
                }
            }

        }
    }

  
}
