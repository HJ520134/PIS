using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.ProductionPlanning
{
    public class PPFlowchartMPVM : BaseModel
    {
        public int? Production_Schedul_UID { get; set; }

        public int FlowChart_Detail_UID { get; set; }

        public int Input_Qty { get; set; }

        public string PlanType { get; set; }

        public decimal Target_Yield { get; set; }

        public int Process_Seq { get; set; }

        public string Process { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime? Product_Date { get; set; }


    }

    public class PPFlowchartDetailVM : BaseModel
    {
        public int Flowchart_Detail_ME_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int? Flowchart_Mapping_UID { get; set; }
        public int Binding_Seq { get; set; }
        public int Process_Seq { get; set; }
        public string Process_Station { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string DRI { get; set; }
        public string Place { get; set; }
        public string PP_Process { get; set; }
        public string KeyProcess { get; set; }
        public string Rework_Flag { get; set; }
        public string IsQAProcess { get; set; }
        public string KeyProcessSub { get; set; }
    }

    public class ActiveManPowerSearchVM : BaseModel
    {
        public int id { get; set; }
        public int Version { get; set; }
        public int Sub_ProcessSeq { get; set; }
        public string Process_Seq { get; set; }
        public string Process { get; set; }
        public string SubProcess { get; set; }
        public string Place { get; set; }
        public string Equipment_Name { get; set; }
        public string Process_Station { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string Modified_Date_From { get; set; }
        public string Modified_Date_End { get; set; }

    }

    public class ActiveManPowerVM : BaseModel
    {
        public int? Product_RequestStaff_UID { get; set; }
        public int? FlowChart_Detail_UID { get; set; }
        public int? Flowchart_Detail_ME_UID { get; set; }
        public int? Flowchart_Detail_ME_Equipment_UID { get; set; }
        public int? System_FunPlant_UID { get; set; }
        public int? Father_UID { get; set; }
        public int? Child_UID { get; set; }
        public string Product_Phase { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? ProductDate { get; set; }
        public int Process_Seq { get; set; }
        public string FunPlant { get; set; }
        public string Place { get; set; }
        public string Process { get; set; }
        public int Sub_ProcessSeq { get; set; }
        public string SubProcess { get; set; }
        public int? OP_Qty { get; set; }
        public int? Monitor_Staff_Qty { get; set; }
        public int? Technical_Staff_Qty { get; set; }
        public int? Material_Keeper_Qty { get; set; }
        public int? Others_Qty { get; set; }
        public string Equipment_Name { get; set; }
        public int? Created_UID { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Modified_UID { get; set; }
        public DateTime? Modified_Date { get; set; }
        public int LanguageID { get; set; }

    }

    public class ActiveEquipVM : BaseModel
    {
        public int? FlowChart_Detail_UID { get; set; }
        public int? Flowchart_Detail_ME_UID { get; set; }
        public int? Flowchart_Detail_ME_Equipment_UID { get; set; }
        public int Process_Seq { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string Equipment_Name { get; set; }
        public int? Father_UID { get; set; }
        public int? Child_UID { get; set; }
    }
}
