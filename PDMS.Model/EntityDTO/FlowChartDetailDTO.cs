using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class FlowChartDetailDTO : EntityDTOBase
    {
        public int FlowChart_Detail_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string Process_Seq { get; set; }
        public string DRI { get; set; }
        public string Place { get; set; }
        public string Process { get; set; }
        public int Product_Stage { get; set; }
        public string Color { get; set; }
        public string Edition { get; set; }
        public string Process_Desc { get; set; }
        public string Material_No { get; set; }
        public int FlowChart_Version { get; set; }
        public string FlowChart_Version_Comment { get; set; }
        public Nullable<int> FatherProcess_UID { get; set; }
        public string FatherProcess { get; set; }
        public string Rework_Flag { get; set; }
        public string IsQAProcess { get; set; }
        public string IsQAProcessName { get; set; }
        public int WIP_QTY { get; set; }
        public int Current_WH_QTY { get; set; }
        public int NullWip { get; set; }
        public int Binding_Seq { get; set; }
        public string FromWHS { get; set; }
        public string ToWHSOK { get; set; }
        public string ToWHSNG { get; set; }
        public string ItemNo { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Old_FlowChart_Detail_UID { get; set; }
        public int Organization_UID { get; set; }
        public bool Location_Flag { get; set; }
        public string RelatedRepairUID { get; set; }
        //自定义
        public int Flowchart_Detail_ME_UID { get; set; }
        public string chkPPProcess { get; set; }
        public string hidJson { get; set; }
        public string FunPlant { get; set; }
        public string RelatedRepairBindingSeq { get; set; }

        public string Data_Source { get; set; }
        public bool Is_Synchronous { get; set; }

    }
}
