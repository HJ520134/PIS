using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class QAReportVM
    {
        public QAReportDaySummeryDTO summeryData { set; get; }
        public List<QAReportExceptionTypeRankDTO> FirstRejectionRateTopTen = new List<QAReportExceptionTypeRankDTO>();
        public List<QAReportExceptionTypeRankDTO> SecondRejectionRateTopTen = new List<QAReportExceptionTypeRankDTO>();
    }

    public class QAProjectVM
    { 
        public string ProjectName { set; get; }
        public int Project_UID { set; get; }
    }

    public class QAReportSearchProjectAndMetrialTypeVM : BaseModel
    {
        public List<string> MaterielType = new List<string>();
        public List<QAProjectVM> Project = new List<QAProjectVM>();
    }

    public class PartTypeVM : BaseModel
    {
        public int FlowChart_Master_UID { set; get; }
        public string Part_Type { set; get; }
    }

    public class QAReportSearchVM : BaseModel
    {

        /// <summary>
        /// 厂区
        /// </summary>
        public string Plant { set; get; }
        public int FlowChart_Master_UID { set; get; }
        public int FlowChart_verion { set; get; }
        public int FlowChart_Detail_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public string Color { set; get; }
        public string MaterialType { set; get; }
        public int Project_UID { set; get; }

        /// <summary>
        /// 部件
        /// </summary>
        public string Part_Type { set; get; }

        /// <summary>
        /// 楼栋
        /// </summary>
        public string Place { set; get; }
        public DateTime ProductDate { set; get; }
        /// <summary>
        /// 生产时间段
        /// </summary>
        public string Time_interval { set; get; }
        /// <summary>
        /// 功能厂
        /// </summary>
        public string FunPlant { set; get; }
        public int System_FunPlant_UID { set; get; }
        /// <summary>
        /// 专案名称
        /// </summary>
        public string ProjectName { set; get; }
        public string Tab_Select_Text { set; get; }
        /// <summary>
        /// OP类型
        /// </summary>
        public string OPType { set; get; }
        public string Product_Phase { set; get; }
        public int OPType_OrganizationUID { set; get;}

        public int Count { set; get; }
        /// <summary>
        /// 产销报表使用: @SearchType： 1.ALL查询当前功能厂不良明细汇总 2.Detail 查询当前功能厂制程不良明细
        /// </summary>
        public int SearchType { set; get; }
        /// <summary>
        /// 产销报表使用: @RateType : 1---一次良率；2---二次良率
        /// </summary>
        public int RateType { set; get; }
        public int? languageID { get; set; }

    }
    

    public class QAReportDaySummeryVM : BaseModel
    {
        public string FunPlant { get; set; }
        public string Process { set; get; }
        public int FirstCheck_Qty { set; get; }
        public int FirstOK_Qty { set; get; }
        public string FirstRejectionRate { set; get; }
        public string SecondRejectionRate { set; get; }
        public int Input { set; get; }
        public int SepcialAccept_Qty { set; get; }
        public int Shipment_Qty { set; get; }
        public string FirstTargetYield { set; get; }
        public string SecondTargetYield { set; get; }
        public int NG { get; set; }
        public int Process_Seq { set; get; }
        public int FlowChart_Detail_UID { set; get; }
        public string Flag { get; set; }
    }

    public class QAReportExceptionTypeRank:BaseModel
    {
        public Int64 RankNum { set; get; }
        public int TotalCount { set; get; }
        public string TypeName { set; get; }
        public string RejectionRate { set; get; }
        public string BadTypeEnglishCode { set; get; }
        /// <summary>
        /// 表示是外观不良 还是尺寸不良
        /// </summary>
        public string ExceptionType { set; get; }
    }

  
}
