using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.QualtyTrace
{

    public class TraceModelLists
    {
        public List<QTraceItem> data { get; set; }
    }

    public class ToptenItem
    {
        public String proccess { get; set; }
        public String item_ch { get; set; }
        public String item_en { get; set; }
        public int count { get; set; }

    }

    public class QTraceItem
    {
        public List<ToptenItem> issue { get; set; }
        public String proccess { get; set; }
        public int freshOK { get; set; }
        public int ngFail { get; set; }
        public int ngScrap { get; set; }
        public int reworkOK { get; set; }


    }

    public class QEboadSum :BaseModel
    {
        public int FlowChartMaster_UID { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public int OneCheck_QTY { get; set; }
        public int OneCheck_OK { get; set; }
        public int NGReuse { get; set; }
        public int NGReject { get; set; }
        public decimal OneTargetYield { get; set; }
        public decimal OneYield { get; set; }
        public int RepairOK { get; set; }
        public decimal SecondTargetYield { get; set; }
        public decimal SecondYield { get; set; }
        public DateTime Modified_Date { get; set; }

}

    public class TopTenQeboardVM : BaseModel
    {
        public int FlowChartMaster_UID { get; set; }

        public string Project { get; set; }

        public string Part_Types { get; set; }

        public string Time_Interval { get; set; }
        //--生产日期
        public string Product_Date { get; set; }
        // --制程序号
        public int Process_Seq { get; set; }
        //  --制程名
        public string Process { get; set; }
        //--检验数
        public int CheckNum { get; set; }
        // --总不良数
        public int TotolNG { get; set; }
        // --总不良率
        public decimal TotalYidld { get; set; }
        //--不良名称
        public string DefectName { set; get; }
        //  --不良数
        public int NG { get; set; }
        // --不良率
        public decimal Yield { get; set; }
        // --不良类型
        public string DefectType { get; set; }
        public DateTime Modified_Date { get; set; }
    }

    public class ProjectInfo
    {
        public string Project { get; set; }

        public string Part_Types { get; set; }

    }

    public class ProcessTargetInfo
    {
        public string Process { get; set; }
        public decimal FirstTargetYield { get; set; }
        public decimal SecondTargetYield { get; set; }
    }
     

}
