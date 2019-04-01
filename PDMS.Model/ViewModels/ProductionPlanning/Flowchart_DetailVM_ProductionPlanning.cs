using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class Flowchart_Detail_ProductionPlanning:BaseModel
    {
        #region ---ME

        public Nullable<int>  Flowchart_Master_UID { set; get; }
        public Nullable<int>  Flowchart_Detail_ME_UID { set; get; }
        public string Process { set; get; }
        public Nullable<int>  Process_Seq { set; get; }
        public Nullable<int>  Binding_Seq { set; get; }
        public string Funplant { set; get; }
        public int System_FunPlant_UID { get; set; }
        public string Process_Station { set; get; }
        public string Process_Desc { set; get; }
        public string Processing_Equipment { set; get; }
        public string Automation_Equipment { set; get; }
        public string Auxiliary_Equipment { set; get; }
        public Nullable<decimal> Epquipment_CT { set; get; }
        public Nullable<decimal> Setup_Time { set; get; }
        public Nullable<decimal> Total_Cycletime { set; get; }
        public Nullable<decimal> Estimate_Yield { set; get; }
        public Nullable<decimal> Manpower_Ratio { set; get; }
        public Nullable<decimal> Capacity_ByHour { set; get; }
        public Nullable<decimal> Capacity_ByDay { set; get; }
        public Nullable<decimal>  Equipment_RequstQty { set; get; }
        public Nullable<int>  Manpower_TwoShift { set; get; }
        public Nullable<int>  FlowChart_Version { set; get; }

        #endregion

        #region ---IE

        public Nullable<int>  Flowchart_Detail_IE_UID { set; get; }
        public Nullable<int>  VariationOP_Qty { set; get; }
        public Nullable<int>  RegularOP_Qty { set; get; }
        public Nullable<int>  MaterialKeeper_Qty { set; get; }
        public Nullable<int>  Others_Qty { set; get; }
        public string Notes { set; get; }
        public Nullable<int>  Match_Type { set; get; }
        public Nullable<int>  Match_Rule { set; get; }
        public Nullable<decimal> Monitor_Match_Raito { set; get; }
        public Nullable<int>  VariableMonitor__Qty { set; get; }
        public decimal SquadLeader_Raito { get; set; }
        public int SquadLeader_Variable_Qty { get; set; }
        public decimal Technician_Raito { get; set; }
        public int Technician_Variable_Qty { get; set; }
        public Nullable<decimal> Technician_Match_Raito { set; get; }
        public Nullable<int>  VariableTechnician__Qty { set; get; }
        public Nullable<int>  VariationEquipment_RequstQty { set; get; }
        public Nullable<int> Technician_Qty { set; get; }
        public Nullable<int> SquadLeader_Qty { set; get; }

        #endregion

        public int Created_UID { set; get; }
        public DateTime Created_Date { set; get; }


    }

    public class FLowchart_Detail_IE_VM : BaseModel
    {
        public int? Flowchart_Detail_IE_UID { get; set; }
        public Nullable<int> Flowchart_Master_UID { set; get; }
        public Nullable<int> Flowchart_Detail_ME_UID { set; get; }
        public string Process { set; get; }
        public Nullable<int> Process_Seq { set; get; }
        public Nullable<int> Binding_Seq { set; get; }
        public string Funplant { set; get; }
        public int System_FunPlant_UID { get; set; }
        public string Process_Station { set; get; }
        public string Process_Desc { set; get; }
        public string Processing_Equipment { set; get; }
        public string Automation_Equipment { set; get; }
        public string Auxiliary_Equipment { set; get; }
        public Nullable<decimal> Epquipment_CT { set; get; }
        public Nullable<decimal> Setup_Time { set; get; }
        public Nullable<decimal> Total_Cycletime { set; get; }
        public Nullable<decimal> Estimate_Yield { set; get; }
        public Nullable<decimal> Manpower_Ratio { set; get; }
        public Nullable<decimal> Capacity_ByHour { set; get; }
        public Nullable<decimal> Capacity_ByDay { set; get; }
        public Nullable<decimal> Equipment_RequstQty { set; get; }
        public Nullable<int> Manpower_TwoShift { set; get; }
        public Nullable<int> FlowChart_Version { set; get; }
        public int Match_Rule { get; set; }
        public int VariationEquipment_RequstQty { get; set; }
        public int RegularOP_Qty { get; set; }
        public int VariationOP_Qty { get; set; }
        public decimal SquadLeader_Raito { get; set; }
        public int SquadLeader_Variable_Qty { get; set; }
        public decimal Technician_Raito { get; set; }
        public int Technician_Variable_Qty { get; set; }
        public int MaterialKeeper_Qty { get; set; }
        public int Others_Qty { get; set; }
        public int SquadLeader_Qty { get; set; }
        public int Technician_Qty { get; set; }
        public string Notes { get; set; }

        public string IEFlag { get; set; }
        public int Modified_UID { get; set; }
    }

    public class Flowchart_Detail_ProductionPlanningListVM : BaseModel
    {
        #region
        public Nullable<int> Flowchart_Detail_IE_UID { set; get; }
        public Nullable<int> Flowchart_Master_UID { set; get; }
        public Nullable<int> Flowchart_Detail_ME_UID { set; get; }
        public string Process { set; get; }
        public Nullable<int> Process_Seq { set; get; }
        public Nullable<int> Binding_Seq { set; get; }
        public string Funplant { set; get; }
        public string Process_Station { set; get; }
        public string Process_Desc { set; get; }
        public string Processing_Equipment { set; get; }
        public string Automation_Equipment { set; get; }
        public string Auxiliary_Equipment { set; get; }
        public string Epquipment_CTVM { set; get; }
        public string Setup_TimeVM { set; get; }
        public string Total_CycletimeVM { set; get; }
        public string Estimate_YieldVM { set; get; }
        public string Manpower_RatioVM { set; get; }
        public string Capacity_ByHourVM { set; get; }
        public string Capacity_ByDayVM { set; get; }
        public string Equipment_RequstQtyVM { set; get; }
        public string Manpower_TwoShiftVM { set; get; }
        public string FlowChart_VersionVM { set; get; }
        public string Notes { set; get; }
        public string VariationOP_QtyVM { set; get; }
        public string RegularOP_QtyVM { set; get; }
        public string MaterialKeeper_QtyVM { set; get; }
        public string Others_QtyVM { set; get; }
        public string Match_TypeVM { set; get; }
        public string Match_RuleVM { set; get; }

        public string SquadLeader_Raito { get; set; }
        public string SquadLeader_Variable_Qty { get; set; }
        public string Technician_Raito { get; set; }
        public string Technician_Variable_Qty { get; set; }

        public string Monitor_Match_RaitoVM { set; get; }
        public string VariableMonitor__QtyVM { set; get; }
        public string Technician_Match_RaitoVM { set; get; }
        public string VariableTechnician__QtyVM { set; get; }
        public string VariationEquipment_RequstQtyVM { set; get; }

        public int SquadLeader_Qty { get; set; }
        public int Technician_Qty { get; set; }

        public int Created_UID { set; get; }
        public DateTime Created_Date { set; get; }
        public int Modified_UID { set; get; }
        public DateTime Modified_Date { set; get; }
        public string IEFlag { get; set; }
        #endregion
    }

    public class Flowchart_DetailIEList : BaseModel
    {
        public List<Flowchart_Detail_ProductionPlanningListVM> mgDataList = new List<Flowchart_Detail_ProductionPlanningListVM>();
    }

    public class Flowchart_Detail_ProductionPlanningList : BaseModel
    {
        public List<Flowchart_Detail_ProductionPlanning> DataList = new List<Flowchart_Detail_ProductionPlanning>();
    }

    public class Flowchart_Detail_IE_VM : BaseModel
    {
        public int Flowchart_Detail_ME_UID { set;get;}
        public string Process { set; get; }
        public int Process_Seq { set; get; }
        public int Flowchart_Detail_ME_UID_Banding { set; get; }

        public Nullable<int> Monitor_Staff_UID { set; get; }
        public bool MonitorFlag { set; get; }

        public Nullable<int> Technician_Staff_UID { set; get; }
        public bool TechnicianFlag { set; get; }

        public int Created_UID { set; get; }
        public DateTime Created_Date { set; get; }
    }

    public class Flowchart_Detail_IE_VMList: BaseModel
    {
        public List<Flowchart_Detail_IE_VM> DataList = new List<Flowchart_Detail_IE_VM>();
    }

}
