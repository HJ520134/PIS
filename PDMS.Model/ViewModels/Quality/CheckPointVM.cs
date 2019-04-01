using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class CheckPointVM
    {
        public string ProjectName { set; get; }
        public string ProcessName { set; get; }
        public int ProcessSeq { set; get; }
        public int Flowchart_Master_UID { set; get; }
        public string FunPlant { set; get; }
        public int FlowChart_Detail_UID { set; get; }
        public string OtherInfos { set; get; }

        public string IsQAProcess { set; get; }

    }

    public class CheckPointInputCondition : BaseModel
    {
        public List<string> Place = new List<string>();
        public List<FlowchartColor> ColorList = new List<FlowchartColor>();
        public List<string> MaterialType = new List<string>();
        public string Part_Types { get; set; }

    }

    public class OQCRecordCondition : BaseModel
    {
        public List<string> Place = new List<string>();
        public List<string> ColorList = new List<string>();
        public List<string> MaterialType = new List<string>();

    }

    public class FlowchartColor : BaseModel
    {
        public string Color { set; get; }
        public int Flowchart_Detail_UID { set; get; }
        public string Process { set; get; }
        public int Process_Seq { set; get; }
    }

    public class CheckPointInputConditionModel : BaseModel
    {
        public string Part_Types { get; set; }
        public string Place { set; get; }
        public string Color { set; get; }
        public string MaterialType { set; get; }
        public int Flowchart_Master_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public DateTime ProductDate { set; get; }
        public string Time_interval { set; get; }
        public string Project_Name { set; get; }
        public int Flowchart_Detail_UID { set; get; }
    }

}
