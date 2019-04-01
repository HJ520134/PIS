using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class FlowChartPlanManagerVM
    {
        public int Detail_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public string Place { get; set; }
        public DateTime date { get; set; }

        public string Color { get; set; }
        public int ? MondayProduct_Plan { set; get; }
        public double ?MondayTarget_Yield { set; get; }
        public int? MondayProper_WIP { set; get; }

        public int ?TuesdayProduct_Plan { set; get; }
        public double? TuesdayTarget_Yield { set; get; }
        public int? TuesdayProper_WIP { set; get; }

        public int? WednesdayProduct_Plan { set; get; }
        public double? WednesdayTarget_Yield { set; get; }
        public int? WednesdayProper_WIP { set; get; }

        public int ?ThursdayProduct_Plan { set; get; }
        public double ?ThursdayTarget_Yield { set; get; }
        public int? ThursdayProper_WIP { set; get; }

        public int ?FridayProduct_Plan { set; get; }
        public double? FridayTarget_Yield { set; get; }
        public int? FridayProper_WIP { set; get; }

        public int ?SaterdayProduct_Plan { set; get; }
        public double? SaterdayTarget_Yield { set; get; }
        public int? SaterdayProper_WIP { set; get; }

        public int? SundayProduct_Plan { set; get; }
        public double? SundayTarget_Yield { set; get; }
        public int? SundayProper_WIP { set; get; }
    }
    public class FlPlanManagerVM
    {
        public int Detail_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public string Place { get; set; }
        public DateTime date { get; set; }

        public string Color { get; set; }
        public int? MondayProduct_Plan { set; get; }
        public string MondayTarget_Yield { set; get; }
        public int? MondayProper_WIP { set; get; }

        public int? TuesdayProduct_Plan { set; get; }
        public string TuesdayTarget_Yield { set; get; }
        public int? TuesdayProper_WIP { set; get; }

        public int? WednesdayProduct_Plan { set; get; }
        public string WednesdayTarget_Yield { set; get; }
        public int? WednesdayProper_WIP { set; get; }

        public int? ThursdayProduct_Plan { set; get; }
        public string ThursdayTarget_Yield { set; get; }
        public int? ThursdayProper_WIP { set; get; }

        public int? FridayProduct_Plan { set; get; }
        public string FridayTarget_Yield { set; get; }
        public int? FridayProper_WIP { set; get; }

        public int? SaterdayProduct_Plan { set; get; }
        public string SaterdayTarget_Yield { set; get; }
        public int? SaterdayProper_WIP { set; get; }

        public int? SundayProduct_Plan { set; get; }
        public string SundayTarget_Yield { set; get; }
        public int? SundayProper_WIP { set; get; }
    }

    public class IEPlanManagerVM
    {
        public int Detail_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public string Place { get; set; }
        public DateTime date { get; set; }
        public int ShiftTimeID { get; set; }
        public string Color { get; set; }
        public int? MondayIE_TargetEfficacy { set; get; }
        public int? MondayIE_DeptHuman { set; get; }
       
       // public string ShiftTimeID { set; get; }
        public int? TuesdayIE_TargetEfficacy { set; get; }
        public int? TuesdayIE_DeptHuman { set; get; }
      
        public int? WednesdayIE_TargetEfficacy { set; get; }
        public int? WednesdayIE_DeptHuman { set; get; }
      

        public int? ThursdayIE_TargetEfficacy { set; get; }
        public int? ThursdayIE_DeptHuman { set; get; }
       

        public int? FridayIE_TargetEfficacy { set; get; }
        public int? FridayIE_DeptHuman { set; get; }
     

        public int? SaterdayIE_TargetEfficacy { set; get; }
        public int? SaterdayIE_DeptHuman { set; get; }
  

        public int? SundayIE_TargetEfficacy { set; get; }
        public int? SundayIE_DeptHuman { set; get; }
        
    }

    public class IEPlanManagerVM1
    {
        public int Detail_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public string Place { get; set; }
        public DateTime date { get; set; }
        public int ShiftTimeID { get; set; }
        public string Color { get; set; }
        public int? MondayIE_TargetEfficacy { set; get; }
        public int? MondayIE_DeptHuman { set; get; }

        // public string ShiftTimeID { set; get; }
        public int? TuesdayIE_TargetEfficacy { set; get; }
        public int? TuesdayIE_DeptHuman { set; get; }

        public int? WednesdayIE_TargetEfficacy { set; get; }
        public int? WednesdayIE_DeptHuman { set; get; }


        public int? ThursdayIE_TargetEfficacy { set; get; }
        public int? ThursdayIE_DeptHuman { set; get; }


        public int? FridayIE_TargetEfficacy { set; get; }
        public int? FridayIE_DeptHuman { set; get; }


        public int? SaterdayIE_TargetEfficacy { set; get; }
        public int? SaterdayIE_DeptHuman { set; get; }


        public int? SundayIE_TargetEfficacy { set; get; }
        public int? SundayIE_DeptHuman { set; get; }

    }
}
