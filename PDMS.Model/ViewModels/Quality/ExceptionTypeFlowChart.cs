using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model
{
    public class ExcepTypeFlowChartSearch : BaseModel
    {
        public string Project { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public string TypeName { get; set; }
        public string FunPlant { get; set; }
        public string TypeClassify { get; set; }
    }



    public class ExceptionTypeFlowChartVM : BaseModel
    {

        public string Project { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Detail_UID { set; get; }
        public int Process_Seq { set; get; }
        public string ExceptionType_Name { get; set; }
        public string FunPlant { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public System.DateTime Creator_Date { get; set; }
        public string TypeClassify { get; set; }

    }

  

    public class ExceptionTypeListVM : BaseModel
    {
        public int? ExceptionTypeWithFlowchart_UID { get; set; }
        public string Project { get; set; }
        public string Process { get; set; }
        public string ExceptionType_Name { get; set; }
        public string FunPlant { get; set; }
        public string Creator_User { get; set; }
        public string TypeClassify { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public System.DateTime Creator_Date { get; set; }
        public int Process_Seq { set; get; }
        public int FlowChart_Detail_UID { set; get; }
        public Nullable<int> FlowchartMaster_UID { set; get; }
    }

    public class ExceptionTypeProcessList : BaseModel
    {
        public List<ExceptionTypeFlowChartVM> ExceptionTypeProcessLists { set; get; }

    }

}
