using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.ProductionPlanning
{
    public class ProductionPlanningModel : BaseModel
    {



    }

    public class ProductionPlanningModelGet : BaseModel
    {
        public FlowChartMasterDTO flowchartMasterDTO { get; set; }
        public List<FlowchartDetailMEDTO> flowchartDetailMeDTOList { get; set; }
    }

    public class ProductionPlanningModelGetAPIModel : BaseModel
    {
        public bool IsEdit { get; set; }
        public int AccountID { get; set; }
        public List<ProductionPlanningModelGet> GetList { get; set; }
        public List<FlowchartDetailMEEquipmentDTO> EquipDTOList { get; set; }
        public List<FlowchartDetailMEEquipmentDTO> AutoEquipDTOList { get; set; }
    }

    public class EquipmentGet : BaseModel
    {
        public int Flowchart_Detail_ME_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int Binding_Seq { get; set; }
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public decimal Capacity_ByDay { get; set; }
        public decimal Capacity_ByHour { get; set; }
        public string Process_Station { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string FunPlant { get; set; }
        public decimal Equipment_RequstQty { get; set; }
        public int FlowChart_Version { get; set; }
        public int Flowchart_Detail_ME_Equipment_UID { get; set; }
        public string Equipment_Name { get; set; }
        public string Equipment_Spec { get; set; }
        public Nullable<decimal> Plan_CT { get; set; }
        public Nullable<decimal> Current_CT { get; set; }
        public Nullable<int> EquipmentQty { get; set; }
        public Nullable<decimal> Ratio { get; set; }
        public Nullable<int> RequestQty { get; set; }
        public string EquipmentType { get; set; }
        public string Notes { get; set; }
        public int? NPI_CurrentQty { get; set; }
        public int? MP_CurrentQty { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public int Modified_UID { get; set; }

    }



}
