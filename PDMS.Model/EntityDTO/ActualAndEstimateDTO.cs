using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class ActualAndEstimateDTO
    {
        public string Project_Name { get; set; }
        public string FirstDayName { get; set; }
        public int FirstActual { get; set; }
        public int FirstEstimate { get; set; }
        public string SectDayName { get; set; }
        public int SecActual { get; set; }
        public int SecEstimate { get; set; }
        public string ThirdDayName { get; set; }
        public int ThirdActual { get; set; }
        public int ThirdEstimate { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public string Process { get; set; }
        public int Process_Seq { get; set; }
    }

    public class ActualAndEstimateInfo : DynamicObject
    {
        public string Project_Name { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public Dictionary<string, object> dictionary
        = new Dictionary<string, object>();
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }
        public ActualAndEstimateColumn ShowColumn { get; set; }
        public string Process { get; set; }
        public int Process_Seq { get; set; }
        public int FlowChart_Detail_UID { get; set; }
    }

    public class ActualInfo
    {
        public string Project_Name { get; set; }
        public int ActualNum { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public DateTime ProductDate { get; set; }
        public string Process { get; set; }
        public int Process_Seq { get; set; }
        public int FlowChart_Detail_UID { get; set; }
    }

    public class EstimateInfo
    {
        public string Project_Name { get; set; }
        public int EstimateNum { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public string ProductDate { get; set; }
        public string Process { get; set; }
        public int Process_Seq { get; set; }
        public int FlowChart_Detail_UID { get; set; }
    }

    public class ActualAndEstimateColumn
    {
        public List<string> ColumnList { get; set; }
    }

}
