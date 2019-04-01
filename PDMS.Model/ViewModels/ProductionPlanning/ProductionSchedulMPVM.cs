using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.ProductionPlanning
{
    public class ProductionSchedulMPVM : BaseModel
    {
        public int Production_Schedul_UID { get; set; } 
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public string Project_Name { get; set; }
        public string Product_Phase { get; set; }
        public DateTime Product_Date { get; set; }
        public int Input_Qty { get; set; }
        public int PlanType { get; set; }
        public int DayType { get; set; }
        public int Process_Seq { get; set; }
        public string Color { get; set; }
        public string ME_Process { get; set; }
        public string Detail_Process { get; set; }

    }

    public class QueryProductionSchedulMPVM : BaseModel
    {
        public int Production_Schedul_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public string Project_Name { get; set; }
        public string Product_Phase { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Product_Date { get; set; }
        public int Input_Qty { get; set; }
        public int PlanType { get; set; }
        public int DayType { get; set; }
        public decimal Target_Yield { get; set; }
        public DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        //自定义
        public int Process_Seq { get; set; }
        public string Per_Target_Yield { get; set; }
        public string PlanTypeValue { get; set; }
        public string DayTypeValue { get; set; }
        public string Process { get; set; }

    }
}
