using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowchartDetailMEDTO : BaseModel
    {
        public int Flowchart_Detail_ME_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int Binding_Seq { get; set; }
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public string Process_Desc { get; set; }
        public string Color { get; set; }
        public string Processing_Equipment { get; set; }
        public string Automation_Equipment { get; set; }
        public string Processing_Fixtures { get; set; }
        public string Auxiliary_Equipment { get; set; }
        public Nullable<int> Equipment_CT { get; set; }
        public Nullable<decimal> Setup_Time { get; set; }
        public Nullable<decimal> Total_Cycletime { get; set; }
        public decimal Estimate_Yield { get; set; }
        public Nullable<decimal> Manpower_Ratio { get; set; }
        public decimal Capacity_ByHour { get; set; }
        public decimal Capacity_ByDay { get; set; }
        public decimal? Equipment_RequstQty { get; set; }
        public Nullable<int> Manpower_2Shift { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public int Modified_UID { get; set; }
        public string Process_Station { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string FunPlant { get; set; }
        public int FlowChart_Version { get; set; }
        public string FlowChart_Version_Comment { get; set; }
        public int Manpower_Rule { get; set; }
        public string FuncPlant { get; set; }
        public string Project_Name { get; set; }
    }
}
