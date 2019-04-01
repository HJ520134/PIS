using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ExportReportDataResult : BaseModel
    {
        public int TotalItemCount { get; set; }
        public List<Daily_ProductReportItem> Items { get; set; }
    }

    public class ExportPPCheckDataResult : BaseModel
    {
        public int TotalItemCount { get; set; }
        public List<PPCheckDataItem> Items { get; set; }
    }

    public class ExportOnePaseeData : BaseModel
    {
        public string Project { get; set; }
        public decimal onePass { get; set; }
        public decimal CNCPass { get; set; }
        public decimal BiaoMianPass { get; set; }
        public decimal YangJiPass { get; set; }
        public decimal ZuZhuangPass { get; set; }
        public decimal OQCPass { get; set; }

    }

    public class ExportPPCheckData : BaseModel
    {


        //ie
        public int IE_TargetEfficacy { get; set; }
        public int IE_DeptHuman { get; set; }

        public string Customer { get; set; }
         public   string OP { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }
        public string Color { get; set; }
        public DateTime Reference_Date { get; set; }
        public string Time_InterVal { get; set; }
        public int input_day_verion { get; set; }
        public List<Tab_All_Text> TabList { get; set; }
        public string FunPlant { get; set; }
    }

    public class ExportPPCheckDataNew : BaseModel
    {
        public int Flowchart_Master_UID { get; set; }
        public string Customer { get; set; }
        public string OP { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }
        public string Color { get; set; }
        public DateTime Reference_Date { get; set; }
        public string Time_InterVal { get; set; }
        public int input_day_verion { get; set; }
        public List<Tab_All_Text> TabList { get; set; }
        public string FunPlant { get; set; }
    }

    public class ExportNullWipData : BaseModel
    {
        public string Customer { get; set; }
        public DateTime Product_Date { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }
        public string Color { get; set; }
        public DateTime Reference_Date { get; set; }
        public string Interval_Time { get; set; }
        public int input_day_verion { get; set; }
        public List<Tab_All_Text> TabList { get; set; }
        public string FunPlant { get; set; }
    }

    public class Tab_All_Text : BaseModel
    {
        public string Time_InterVal { get; set; }
    }
    public class PPEditWIP : EntityDTOBase
    {
        public List<ProductUIDAndWIP> PPEditValue { get; set; }
    }

    public class MesSyncParam : BaseModel
    {
        public string currentDate { get; set; }
        public string currentInterval { get; set; }

        public string Customer { get; set; }
        public string Project { get; set; }
        public string PhaseName { get; set; }
        public string PartTypes { get; set; }
        public int FlowChartMaster_UID { get; set; }
    }

    public class ProductUIDAndWIP : BaseModel
    {
        public string Product_UID { get; set; }
        public int Wip_Qty { get; set; }
    }
    public class PPCheckDataSearch : BaseModel
    {
        public string OP { get; set; }
        public string Customer { get; set; }
        public DateTime Product_Date { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }
        public string Color { get; set; }
        public DateTime Reference_Date { get; set; }
        public string Interval_Time { get; set; }
        public string Tab_Select_Text { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
    }
    public class ExportSearch : BaseModel
    {
        public string Time_Interval { get; set; }
        public DateTime Product_Date { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }
        public string Color { get; set; }
        public string ShowType { get; set; }
        public int Version { get; set; }
        public int Modified_UID { get; set; }
    }

    /// <summary>
    /// 获取枚举表的数据
    /// </summary>
    public class EnumTimeInterVal : BaseModel
    {
        public int Enum_UID { get; set; }
        public string Enum_Type { get; set; }
        public string Enum_Name { get; set; }
        public string Enum_Value { get; set; }
        public string Decription { get; set; }
    }

    public class ExportPPCheck_Data : BaseModel
    {
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string FunPlant_Manager { get; set; }
        public decimal Target_Yield { get; set; }
        public int Product_Plan { get; set; }
        public int Product_Plan_Sum { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public decimal Rolling_Yield_Rate { get; set; }
        public decimal Finally_Field { get; set; }
        public int WIP_QTY { get; set; }
        public int NullWip_QTY { get; set; }
        public int OKWip_QTY { get; set; }

        public int Product_Plan_Sum_Now { get; set; }
        public int Picking_QTY_Now { get; set; }
        public int WH_Picking_QTY_Now { get; set; }
        public int Good_QTY_Now { get; set; }
        public int Adjust_QTY_Now { get; set; }
        public int WH_QTY_Now { get; set; }
        public int NG_QTY_Now { get; set; }
        public decimal Rolling_Yield_Rate_Now { get; set; }
        public decimal Finally_Field_Now { get; set; }
        public int WIP_QTY_Now { get; set; }
        public bool Location_Flag { get; set; }
        public string Time_Interval { get; set; }

        public int? repairInputCount { get; set; }
        public int? FlowChart_Detail_UID { get; set; }
        public int? repairOutputCount { get; set; }
        public string Rework_Flag { get; set; }
        public int? Opposite_QTY { get; set; }
        public string rework_type { get; set; }
        public string Unacommpolished_Reason { get; set; }
    }
    public class ExportNullWIP_Data : BaseModel
    {
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string FunPlant_Manager { get; set; }
        public decimal Target_Yield { get; set; }
        public int Product_Plan { get; set; }
        public int Product_Plan_Sum { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public decimal Rolling_Yield_Rate { get; set; }
        public decimal Finally_Field { get; set; }
        public int WIP_QTY { get; set; }
        public int NullWip_QTY { get; set; }
        public int OKWip_QTY { get; set; }

        public int Product_Plan_Sum_Now { get; set; }
        public int Picking_QTY_Now { get; set; }
        public int WH_Picking_QTY_Now { get; set; }
        public int Good_QTY_Now { get; set; }
        public int Adjust_QTY_Now { get; set; }
        public int WH_QTY_Now { get; set; }
        public int NG_QTY_Now { get; set; }
        public decimal Rolling_Yield_Rate_Now { get; set; }
        public decimal Finally_Field_Now { get; set; }
        public int WIP_QTY_Now { get; set; }

    }

    public class PPCheckDataItem : BaseModel
    {

        public int Product_Input_UID { get; set; }
        public int Product_Stage { get; set; }
        public int Is_Comfirm { get; set; }
        public string Color { get; set; }
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string FunPlant_Manager { get; set; }
        public decimal Target_Yield { get; set; }
        public int Product_Plan { get; set; }
        public int Product_Plan_Sum { get; set; }
        public int Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public string Good_MismatchFlag { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public decimal Rolling_Yield_Rate { get; set; }
        public decimal Finally_Field { get; set; }
        public int? WIP_QTY { get; set; }
        public int? OKWIP_QTY { get; set; }
        public int? NullWIP_QTY { get; set; }
        public string Unacommpolished_Reason { get; set; }
    }
    public class PPCheckDataItemOrigOriginal : BaseModel
    {
        public int Product_Input_UID { get; set; }
        public int Product_Stage { get; set; }
        public int Is_Comfirm { get; set; }
        public string Color { get; set; }
        public string Time_Interval { get; set; }
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string FunPlant_Manager { get; set; }
        public double Target_Yield { get; set; }
        public int Product_Plan { get; set; }
        public int Product_Plan_Sum { get; set; }
        public int Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public string Good_MismatchFlag { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public double Rolling_Yield_Rate { get; set; }
        public double Finally_Field { get; set; }
        public int? WIP_QTY { get; set; }
        public string Unacommpolished_Reason { get; set; }
    }
    /// <summary>
    /// 日报表
    /// </summary>
    public class Daily_ProductReportItem
    {
        public object ALL_WH_Picking_QTY;
        public object ShiftTimeID;

        public int Product_Input_UID { get; set; }
        public int Product_Stage { get; set; }
        public bool Is_Comfirm { get; set; }
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string DRI { get; set; }
        public decimal Target_Yield { get; set; }
        public int All_Product_Plan { get; set; }
        public int All_Product_Plan_Sum { get; set; }
        public int All_Picking_QTY { get; set; }
        public string All_Picking_MismatchFlag { get; set; }
        public int All_WH_Picking_QTY { get; set; }
        public int All_Good_QTY { get; set; }
        public string All_Good_MismatchFlag { get; set; }
        public int All_Adjust_QTY { get; set; }
        public int All_WH_QTY { get; set; }
        public int All_NG_QTY { get; set; }
        public decimal All_Rolling_Yield_Rate { get; set; }
        public decimal All_Finally_Field { get; set; }
        public int Product_Plan { get; set; }
        public int Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public string Good_MismatchFlag { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public decimal Rolling_Yield_Rate { get; set; }
        public decimal Finally_Field { get; set; }
        public int? WIP_QTY { get; set; }

        public int? IE_TargetEfficacy { get; set; }
        public int? IE_DeptHuman { get; set; }

        public int? NullWIP_QTY { get; set; }
        public int? OKWIP_QTY { get; set; }
        public int? Proper_WIP { get; set; }
        public int? FlowChart_Detail_UID { get; set; }
        public  bool IsFloor { get; set; }

        public DateTime? Product_Date { get; set; }
        public  string Time_Interval { get; set; }
        public string FlowChart_Version { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Unacommpolished_Reason { get; set; }
    }

    public class Daily_ProductReport
    {
        public int FlowChart_Detail_UID { get; set; }
        public int Product_Stage { get; set; }
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string DRI { get; set; }
        public double Target_Yield { get; set; }
        public int All_Product_Plan { get; set; }
        public int All_Product_Plan_Sum { get; set; }
        public int All_Picking_QTY { get; set; }
        public string All_Picking_MismatchFlag { get; set; }
        public int All_WH_Picking_QTY { get; set; }
        public int All_Good_QTY { get; set; }
        public string All_Good_MismatchFlag { get; set; }
        public int All_Adjust_QTY { get; set; }
        public int All_WH_QTY { get; set; }
        public int All_NG_QTY { get; set; }
        public decimal All_Rolling_Yield_Rate { get; set; }
        public decimal All_Finally_Field { get; set; }
        public int Product_Plan { get; set; }
        public int Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public string Good_MismatchFlag { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public decimal Rolling_Yield_Rate { get; set; }
        public decimal Finally_Field { get; set; }
        public int? WIP_QTY { get; set; }


        public int IE_TargetEfficacy { get; set; }
        public int IE_DeptHuman { get; set; }


        public int ? NullWIP_QTY { get; set; }
        public int? OKWIP_QTY { get; set; }
        public int? Proper_WIP { get; set; }
        public string Unacommpolished_Reason { get; set; }
        public bool IsFloor { get; set; }

    }

    public class List_Daily_ProductReportItem
    {
        public List<Daily_ProductReportItem> Result { get; set; }
    }

    public class RePaireParam : BaseModel
    {
        public string process { get; set; }
        public string color { get; set; }
        public string place { get; set; }
        public string Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Rework_Type { get; set; }
        public string FunPlant { get; set; }
        public int isColor { get; set; }
    }

    public class ReportDataSearch : BaseModel
    {
        public int? Flowchart_Master_UID { get; set; }
        public string OP { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }
        public string Color { get; set; }
        public string Select_Type { get; set; }
        public string FunPlant { set; get; }
        public int Building { get; set; }

        //ie
        public int IE_TargetEfficacy { get; set; }
        public int IE_DeptHuman { get; set; }

        //日报表
        public DateTime? Reference_Date { get; set; }
        public string Interval_Time { get; set; }
        public string Tab_Select_Text { get; set; }
        public int input_day_verion { get; set; }
        //周报表
        public DateTime? Week_Date_Start { get; set; }
        public DateTime? Week_Date_End { get; set; }
        public int? Week_Version { get; set; }
        //月报表
        public DateTime? Month_Date_Start { get; set; }
        public DateTime? Month_Date_End { get; set; }
        public int? Month_Version { get; set; }
        //时段报表
        public DateTime? Interval_Date_Start { get; set; }
        public DateTime? Interval_Date_End { get; set; }
        public int? Verion_Interval { get; set; }
        public int IsColour { get; set; }

    }

    public class ReportDataSearchSUM : BaseModel
    {
        public string Flowchart_Master_UID { get; set; }
        public string OP { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }
        public string Color { get; set; }
        public string Select_Type { get; set; }
        public string FunPlant { set; get; }

        //日报表
        public DateTime? Reference_Date { get; set; }
        public string Interval_Time { get; set; }
        public string Tab_Select_Text { get; set; }
        public string input_day_verion { get; set; }
       
        public string IsColour { get; set; }

    }

    public class  NewProductReportSearch : BaseModel
    {
        public int FlowChart_Master_UID { get; set; }
        public int Flowchart_Version { get; set; }
        public DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Color { get; set; }
        public string OPType { get; set; }
        public string Funplant { get; set; }
        public bool IsColorSum { get; set; }
    }

    public class NewProductReportSumSearch:BaseModel
    {
        public int Flowchart_Master_UID { get; set; }
        public int input_day_verion { get; set; }
        public DateTime Reference_Date { get; set; }
        public string Tab_Select_Text { get; set; }
        public string Color { get; set; }
        public string OP { get; set; }
        public string FunPlant { get; set; }
        public int IsColour { get; set; }
    }
    public class ProductInputLocationSearch : BaseModel
    {
        public int FlowChart_Master_UID { get; set; }
        public DateTime? Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string opType { get; set; }
    }
    
    public class PDByProSeqSearch : BaseModel
    {
        public string customer { get; set; }
        public string project { get; set; }
        public string Product_Phase { get; set; }
        public string part_types { get; set; }
        public string Process_Seq { get; set; }
        public string input_date { get; set; }
        public string Time_Interval { get; set; }
        public string optype { get; set; }
    }

    public class VersionBeginEndDate : BaseModel
    {
        public DateTime VersionBeginDate { get; set; }
        public DateTime VersionEndDate { get; set; }
        public string Interval { get; set; }
    }

    public class GetProjectModel : BaseModel
    {
        public string Customer { get; set; }
        public List<string> OpTypes { get; set; }
        public List<int> Project_UID { get; set; }
        public List<int> orgs {get;set;}
    }

    public class GetErrorData : BaseModel
    {
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string Place { get; set; }
        public string FuncPlant { get; set; }
        public string Contact { get; set; }
    }

    public class CheckProductInputQty : BaseModel
    {
        public int Product_UID { get; set; }
        public DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int Opposite_Detail_UID { get; set; }
        public string Rework_Flag { get; set; }
        public string Rework_Type { get; set; }
        public int Opposite_QTY { get; set; }
        public bool Is_Match { get; set; }

    }

    public class Customer_Info
    {
        public string Product_Phase { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
    }

    public class Daily_ProductReportSum
    {
      
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string DRI { get; set; }
        public double Target_Yield { get; set; }

        public int Product_Plan { get; set; }
        public int Picking_QTY { get; set; }

        public int WH_Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int NG_QTY { get; set; }
        public decimal Rolling_Yield_Rate { get; set; }
        public decimal Finally_Field { get; set; }
        public int WIP_QTY { get; set; }
        public int NullWIP_QTY { get; set; }
        public int  OKWIP_QTY { get; set; }
        public int  Proper_WIP { get; set; }
        public int  FlowChart_Detail_UID { get; set; }
        public bool IsFloor { get; set; }

        public string Unacommpolished_Reason { get; set; }
    }


}
