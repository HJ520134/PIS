using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class OQCReprotDTO
    {
        public string Process { set; get; }
        public int DailyInput { set; get; }
        public int DailyOK { set; get; }
        public int DailyNG { set; get; }
        public int DailyRework { set; get; }
        public int NightInput { set; get; }
        public int NightOK { set; get; }
        public int NightNG { set; get; }
        public int NightRework { set; get; }
        public int Input { set; get; }
        public int OK { set; get; }
        public int NG { set; get; }
        public int Rework { set; get; }
        public int WIP { set; get; }
        public decimal FirstYieldRate { set; get; }
        public decimal SecondYieldRate { set; get; }

        public int FlowChart_Detail_UID { set; get; }
    }

    public class OQCReprotVM:BaseModel
    {
        public string Process { set; get; }
        public int DailyInput { set; get; }
        public int DailyOK { set; get; }
        public int DailyNG { set; get; }
        public int DailyRework { set; get; }
        public int NightInput { set; get; }
        public int NightOK { set; get; }
        public int NightNG { set; get; }
        public int NightRework { set; get; }
        public int Input { set; get; }
        public int OK { set; get; }
        public int NG { set; get; }
        public int Rework { set; get; }
        public int WIP { set; get; }
        public string FirstYieldRate { set; get; }
        public string SecondYieldRate { set; get; }

        public int FlowChart_Detail_UID { set; get; }
    }


    public class OQCReprotTopFiveTypeDTO
    {
        public int RankNum { set; get; }
        public string Process { set; get; }
        public string TypeName { set; get; }
        public string TOPType { set; get; }
        public int Qty { set; get; }
        public decimal YieldRate { set; get; }
    }

    public class OQCReprotTopFiveTypeVM:BaseModel
    {
        public int RankNum { set; get; }
        public string Process { set; get; }
        public string TypeName { set; get; }
        public string TOPType { set; get; }
        public int Qty { set; get; }
        public string YieldRate { set; get; }
        public string FunPlant { set; get; }
    }


    public class OQCReportExcel
    {
        public List<OQCReprotVM> SumData = new List<OQCReprotVM>();
        public List<OQCReprotTopFiveTypeVM> TopFive = new List<OQCReprotTopFiveTypeVM>();
    }

}
