using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowChartMasterDTO : EntityDTOBase
    {
        public int FlowChart_Master_UID { get; set; }
        public int Project_UID { get; set; }
        public string Part_Types { get; set; }
        public int FlowChart_Version { get; set; }
        public string FlowChart_Version_Comment { get; set; }
        public bool Is_Latest { get; set; }
        public bool Is_Closed { get; set; }
        public int Organization_UID { get; set; }
        public string Product_Phase { get; set; }
        public string CurrentDepartent { get; set; }
        public int Statue_IE { get; set; }
        public int Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
    }

    public class FlowChartPlanManagerDTO : EntityDTOBase
    {
        public int Detail_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public DateTime date { get; set; }

        public string Color { get; set; }
        public int MonDayProduct_Plan { set; get; }
        public double MonDayTarget_Yield { set; get; }
        public int? MondayProper_WIP { set; get; }

        public int TuesDayProduct_Plan { set; get; }
        public double TuesDayTarget_Yield { set; get; }
        public int? TuesdayProper_WIP { set; get; }
        
        public int WednesdayProduct_Plan { set; get; }
        public double WednesdayTarget_Yield { set; get; }
        public int? WednesdayProper_WIP { set; get; }

        public int ThursdayProduct_Plan { set; get; }
        public double ThursdayTarget_Yield { set; get; }
        public int? ThursdayProper_WIP { set; get; }

        public int FriDayProduct_Plan { set; get; }
        public double FridayTarget_Yield { set; get; }
        public int? FridayProper_WIP { set; get; }

        public int SaterDayProduct_Plan { set; get; }
        public double SaterDayTarget_Yield { set; get; }
        public int? SaterdayProper_WIP { set; get; }

        public int SunDayProduct_Plan { set; get; }
        public double SunDayTarget_Yield { set; get; }
        public int? SundayProper_WIP { set; get; }
    }


    public class IEPlanManagerDTO : EntityDTOBase
    {
        public int Detail_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public DateTime date { get; set; }
        public string ShiftTimeID { get; set; }

        public string Color { get; set; }

        public int MondayIE_TargetEfficacy { set; get; }
        public int MondayIE_DeptHuman { set; get; }


        public int TuesdayIE_TargetEfficacy { set; get; }
        public int TuesdayIE_DeptHuman { set; get; }

        public int WednesdayIE_TargetEfficacy { set; get; }
        public int WednesdayIE_DeptHuman { set; get; }

        public int ThursdayIE_TargetEfficacy { set; get; }
        public int ThursdayIE_DeptHuman { set; get; }

        public int FridayIE_TargetEfficacy { set; get; }
        public int FridayIE_DeptHuman { set; get; }

        public int SaterdayIE_TargetEfficacy { set; get; }
        public int SaterdayIE_DeptHuman { set; get; }

        public int SundayIE_TargetEfficacy { set; get; }
        public int SundayIE_DeptHuman { set; get; }
    }
}
