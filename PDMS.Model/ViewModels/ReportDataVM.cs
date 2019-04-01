using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class TimeSpanReport
    {
        public string Process { set; get; }
        public int SumPlan { set; get; }
        public int SumGoodQty { set; get; }
        public decimal SumYieldRate { set; get; }
        public int WIP { get; set; }
        public int SumIE_TargetEfficacy { set; get; }
        public int SumIE_DeptHuman { set; get; }
        public int IE_TargetEfficacy { set; get; }
        public int IE_DeptHuman { set; get; }
    }

    public class TimeSpanReport_2
    {
        public string Place { set; get; }
        public string FunPlant { set; get; }
        public int Process_Seq { get; set; }
        public string Process { set; get; }
        public string Color { set; get; }
        public string DRI { set; get; }
        public decimal Target_Yield { set; get; }
        public int Product_Plan { set; get; }
        public decimal SumYieldRate { set; get; }
        public int Picking_QTY { set; get; }
        public int WH_Picking_QTY { set; get; }
        public int Good_QTY { set; get; }
        public int Adjust_QTY { set; get; }
        public int WH_QTY { set; get; }
        public int NG_QTY { set; get; }
        public decimal All_Finally_Achieving { set; get; }
        public decimal All_Finally_Yield { set; get; }
        public int NullWip_QTY { set; get; }
        public int WIP_QTY { set; get; }
        public int OK_QTY { set; get; }
        public int SumIE_TargetEfficacy { set; get; }
        public int SumIE_DeptHuman { set; get; }
        public int IE_TargetEfficacy { set; get; }
        public int IE_DeptHuman { set; get; }
    }

    public class ExportWeeklyReportDataResult : BaseModel
    {
        public int TotalItemCount { get; set; }
        public List<WeekReportVM> Items { get; set; }
    }

    public class ExportIntervalReportDataResult : BaseModel
    {
        public int TotalItemCount { get; set; }
        public List<TimeSpanReportVM> Items { get; set; }
    }

    public class WeekReport
    {
        public string Process { set; get; }
        public int SumPlan { set; get; }
        public int SumGoodQty { set; get; }
        public decimal SumYieldRate { set; get; }

        public int MondayPlan { set; get; }
        public int MondayGoodQty { set; get; }
        public decimal MondayYieldRate { set; get; }

        public int TuesdayPlan { set; get; }
        public int TuesdayGoodQty { set; get; }
        public decimal TuesdayYieldRate { set; get; }

        public int WednesdayPlan { set; get; }
        public int WednesdayGoodQty { set; get; }
        public decimal WednesdayYieldRate { set; get; }

        public int ThursdayPlan { set; get; }
        public int ThursdayGoodQty { set; get; }
        public decimal ThursdayYieldRate { set; get; }

        public int FridayPlan { set; get; }
        public int FridayGoodQty { set; get; }
        public decimal FridayYieldRate { set; get; }

        public int SaterdayPlan { set; get; }
        public int SaterdayGoodQty { set; get; }
        public decimal SaterdayYieldRate { set; get; }

        public int SundayPlan { set; get; }
        public int SundayGoodQty { set; get; }
        public decimal SundayYieldRate { set; get; }
    }

    public class TimeSpanReportVM
    {
        public int SumIE_DeptHuman { set; get; }
        public int SumIE_TargetEfficacy { set; get; }
        public string Process { set; get; }
        public int SumPlan { set; get; }
        public int SumGoodQty { set; get; }
        public string SumYieldRate { set; get; }
        public double reachedReate { get; set; }
      
    }
    public class ChartDailyReport
    {
        public string Process { set; get; }
        public int SumPlan { set; get; }
        public int SumGoodQty { set; get; }
        public string SumYieldRate { set; get; }
        public double reachedReate { get; set; }
        public int WIP { get; set; }
        public int ? Proper_WIP { get; set;}
    }

    public class YieldVM
    {
        public string Process { set; get; }
        public Double Target_Yield { set; get; }
        public decimal Good_yield { set; get; }
        public int Product_Plan { get; set; }
    }

    public class WeekReportVM
    {
        public string Process { set; get; }
        public int SumPlan { set; get; }
        public int SumGoodQty { set; get; }
        public string SumYieldRate { set; get; }
        public int SumIE_TargetEfficacy { set; get; }
        public int SumIE_DeptHuman { set; get; }
       

        public int MondayPlan { set; get; }
        public int MondayGoodQty { set; get; }
        public string MondayYieldRate { set; get; }
        public int MondayIE_TargetEfficacy { set; get; }
        public int MondayIE_DeptHuman { set; get; }

        public int TuesdayPlan { set; get; }
        public int TuesdayGoodQty { set; get; }
        public string TuesdayYieldRate { set; get; }
        public int TuesdayIE_TargetEfficacy { set; get; }
        public int TuesdayIE_DeptHuman { set; get; }

        public int WednesdayPlan { set; get; }
        public int WednesdayGoodQty { set; get; }
        public string WednesdayYieldRate { set; get; }
        public int WednesdayIE_TargetEfficacy { set; get; }
        public int WednesdayIE_DeptHuman { set; get; }

        public int ThursdayPlan { set; get; }
        public int ThursdayGoodQty { set; get; }
        public string ThursdayYieldRate { set; get; }
        public int ThursdayIE_TargetEfficacy { set; get; }
        public int ThursdayIE_DeptHuman { set; get; }

        public int FridayPlan { set; get; }
        public int FridayGoodQty { set; get; }
        public string FridayYieldRate { set; get; }
        public int FridayIE_TargetEfficacy { set; get; }
        public int FridayIE_DeptHuman { set; get; }

        public int SaterdayPlan { set; get; }
        public int SaterdayGoodQty { set; get; }
        public string SaterdayYieldRate { set; get; }
        public int SaterdayIE_TargetEfficacy { set; get; }
        public int SaterdayIE_DeptHuman { set; get; }

        public int SundayPlan { set; get; }
        public int SundayGoodQty { set; get; }
        public string SundayYieldRate { set; get; }
        public int SundayIE_TargetEfficacy { set; get; }
        public int SundayIE_DeptHuman { set; get; }
    }
    public class ExportTimeReport : BaseModel
    {
        public int TotalItemCount { get; set; }
        public List<TimeSpanReport_2> Items { get; set; }
    }

}
