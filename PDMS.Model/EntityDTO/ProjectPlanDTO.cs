using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class ProjectPlanDTO
    {
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public int Flowchart_Detail_ME_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string Project_Name { get; set; }
        public string Product_Phase { get; set; }
        public string Process { get; set; }
        public decimal Capacity_ByDay { get; set; }
        public decimal Estimate_Yield { get; set; }
        public DateTime? Product_Date_MP { get; set; }
        public int Input_Qty_MP { get; set; }
        public DayOfWeek DateInWeek { get; set; }
        public decimal Pink { get; set; }
        public int WeekOfMonth { get; set; }
        public DateTime Product_Date { get; set; }
        public int Input_Qty { get; set; }
    }

    public class PlanDataView
    {
        public string Process { get; set; }
        public decimal Pink { get; set; }
        public decimal Monday { get; set; }
        public decimal Tuesday { get; set; }
        public decimal Wednesday { get; set; }
        public decimal Thursday { get; set; }
        public decimal Friday { get; set; }
        public decimal Saturday { get; set; }
        public decimal Sunday { get; set; }
        public int WeekOfMonth { get; set; }
        public DateTime Product_Date_MP { get; set; }
    }

    public class HumanResourcesDTO
    {
        public string Project { get; set; }
        public string FunPlant { get; set; }
        public int Total { get; set; }
        public string Product_Phase { get; set; }
    }

    public class HumanResources
    {
        public string Project { get; set; }
        public string FunPlant { get; set; }
        public int Pink { get; set; }
        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
        public int Sunday { get; set; }
        public string Product_Phase { get; set; }
    }

    public class InputDataForSelectDTO
    {
        public decimal Capacity_ByDay { get; set; }
        public int FlowChart_Version { get; set; }
    }

    public class NowHumanDTO
    {
        public int TotalNum { get; set; }
        public DateTime ProductDate { get; set; }
        public DateTime Product_Date { get; set; }
        public DayOfWeek DateInWeek { get; set; }
        public int WeekOfMonth { get; set; }
        public decimal DRN { get; set; }
        public decimal DRM { get; set; }
        public int Recruit { get; set; }
    }

    public class HumanInfo
    {
        public List<NowHuman> MPHuman { get; set; }
        public List<NowHuman> NPIHuman { get; set; }
        public List<NowDemissionRate> MPDemissionRate { get; set; }
        public List<NowDemissionRate> NPIDemissionRate { get; set; }
        public List<NowHuman> RecruitStaff { get; set; }
    }

    public class NowHuman
    {
        public string Title { get; set; }
        public string Project { get; set; }
        public string FunPlant { get; set; }
        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
        public int Sunday { get; set; }
        public string Product_Phase { get; set; }
        public DayOfWeek DateInWeek { get; set; }
        public int WeekOfMonth { get; set; }
        public DateTime ProductDate { get; set; }
    }

    public class NowDemissionRate
    {
        public string Title { get; set; }
        public string Project { get; set; }
        public string FunPlant { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
        public string Product_Phase { get; set; }
        public DayOfWeek DateInWeek { get; set; }
        public int WeekOfMonth { get; set; }
        public DateTime ProductDate { get; set; }
    }
}
