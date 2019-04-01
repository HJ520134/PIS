using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class Eboard : BaseModel
    {
        public string Project { get; set; }
        public string Time_Interval { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public short Process_Seq { get; set; }
        public string DRI { get; set; }
        public int Prouct_Plan { get; set; }
        public double Target_Yield { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public int WIP_QTY { get; set; }
        public int Board_UID { get; set; }
        public string Part_Types { get; set; }
        public int FlowChart_Master_UID { get; set; }
    }

    public class EboardVM 
    {
        public string Color { get; set; }
        public string Project { get; set; }
        public string Time_Interval { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public short Process_Seq { get; set; }
        public string DRI { get; set; }
        public int Prouct_Plan { get; set; }
        public string  Target_Yield { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public int WIP_QTY { get; set; }
        public int Board_UID { get; set; }
        public string Part_Types { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public string Good_yield { get; set; }
        public string Reach_yield { get; set; }
        public  double yield { get; set; }
        public string GoodColor { get; set; }
        public string ReachColor { get; set; }
        public string flag { get; set; }
        public string place { get; set; }
    }

  
    public class EboardSearch :BaseModel
    {
        public List<string> Project { get; set; }
        public List<string> FunPlant { get; set; } // 五个功能厂和All
        public List<string> Part_Types { get; set; }
        public string Optype { get; set; }
    }
    public class EboardS : BaseModel
    {
        public List<int> MasterUID { get; set; }
        public List<string> FunPlant { get; set; } // 五个功能厂和All
        public List<string> Part_Types { get; set; }
        public string Optype { get; set; }
        public string QE_location { get; set; }
    }
    public class EboardSearchModel : BaseModel
    {
        public string selectProjects { get; set; }
        public string selectFunplants { get; set; } // 五个功能厂和All
        public string Part_Types { get; set; }
        public string Optype { get; set; }
        public string QE_location { get; set; }
    }

}
