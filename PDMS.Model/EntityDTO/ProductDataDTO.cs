using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ProductDataDTO : EntityDTOBase
    {
        public int Product_UID { get; set; }
        public bool Is_Comfirm { get; set; }
        public System.DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string FunPlant { get; set; }
        public string FunPlant_Manager { get; set; }
        public string Product_Phase { get; set; }
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string Process { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public string Color { get; set; }
        public int Prouct_Plan { get; set; }
        public int Product_Stage { get; set; }
        public double Target_Yield { get; set; }
        public int Good_QTY { get; set; }
        public string Good_MismatchFlag { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public int NG_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int? WIP_QTY { get; set; }
        public int? NullWip_QTY { get; set; }
        public string DRI { get; set; }
        public int Adjust_QTY { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public string Material_No { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int Normal_Good_QTY { get; set; }
        public int Abnormal_Good_QTY { get; set; }
        public int Normal_NG_QTY { get; set; }
        public int Abnormal_NG_QTY { get; set; }
        public string Rework_Flag { get; set; }
        public bool Location_Flag { get; set; }
        public string Unacommpolished_Reason { get; set; }

    }
    public class ProductData_Input : ProductDataDTO
    {
        //public string Rework_Flag { get; set; }
        public int Rework_QTY { get; set; }
        public List<ProductReworkInfoVM> ReworkInfoList { get; set; }
        public bool isEdit { get; set; }
      
    }

    public class ProductData_Edit : ProductDataDTO
    {
        public int? Rework_QTY { get; set; }
    }

    public class ProductDataViewDTO
    {
        public int Product_UID { get; set; }
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public int Good_QTY { get; set; }

        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }

        public int NG_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int Adjust_QTY { get; set; }
    }
  
    public class ProductDataVM
    {
        public int Product_UID { get; set; }
        public bool Is_Comfirm { get; set; }
        public System.DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string FunPlant { get; set; }
        public string FunPlant_Manager { get; set; }
        public string Product_Phase { get; set; }
        public int Process_Seq { get; set; }
        public string Place { get; set; }
        public string Process { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public string Color { get; set; }
        public int Prouct_Plan { get; set; }
        public int Product_Stage { get; set; }
        public double Target_Yield { get; set; }
        public int Good_QTY { get; set; }
        public string Good_MismatchFlag { get; set; }
        public string Good_Contact { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public string Picking_Contact { get; set; }
        public int NG_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int? WIP_QTY { get; set; }
        public int? NullWip_QTY { get; set; }
        public int? Current_WH_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public string Material_No { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
        public int Modified_UID { get; set; }
        public string Modified_UserNTID { get; set; }
        public bool IsLast { get; set; }
        public string DRI { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        //Rework
        public List<ProductReworkInfoVM> ReworkInfoList { get; set; }
        public string Rework_Flag { get; set; }
        //public int Rework_QTY { get; set; }
        public bool Is_Match { get; set; }
        public string SumInputOutput_ByRepair { get; set; }
        public string SumInputOutput_ByRework { get; set; }
        public int Rework_QTY { get; set; }
        //一个Repair对应多个Rework站点
        //一个Rework站点里面有多笔返工资料
        //所以这个list里面还要对应一个list
        public List<NewInfo_ReworkList> NewInfo_ReworkList { get; set; }
        public List<NewInfo_RepairList> NewInfo_RepairList { get; set; }

        public string RelatedRepairUID { get; set; }
        public bool IsRedDisplay { get; set; }
        //Q报表数据准备
        public int Normal_Good_QTY { get; set; }
        public int Abnormal_Good_QTY { get; set; }
        public int Normal_NG_QTY { get; set; }
        public int Abnormal_NG_QTY { get; set; }
        public bool Location_Flag { get; set; }
        public string Unacommpolished_Reason { get; set; }
    }

    public class ProductReworkInfoVM
    {
        public int Rework_UID { get; set; }
        public int Product_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int Opposite_Detail_UID { get; set; }
        public int Opposite_QTY { get; set; }
        public DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public bool Is_Match { get; set; }
        public string Rework_Type { get; set; }
    }

    public class ErrorInfoVM
    {

        public string Process { get; set; }
        public string FunPlant { get; set; }
        public string FunPlant_Manager { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
    }

    public class NewInfo_ReworkList
    {
        public int RepairDetailUID { get; set; }
        public string RepairProcess { get; set; }
        public string RepairPlace { get; set; }
        public string Color { get; set; }
        public string JoinName {
            get {
                return string.Format("{0}({1})({2})", RepairProcess, RepairPlace,Color);
            }
        }
        public List<ProductReworkInfoVM> ProductReworkInfoVM { get; set; }
    }

    public class NewInfo_RepairList
    {
        public int ReworkDetailUID { get; set; }
        public string ReworkProcess { get; set; }
        public string ReworkPlace { get; set; }
        public string Color { get; set; }
        public string JoinName
        {
            get
            {
                return string.Format("{0}({1})({2})", ReworkProcess, ReworkPlace, Color);
            }
        }
        public List<ProductReworkInfoVM> ProductRepairInfoVM { get; set; }

    }

    public class ProductLocationItem
    {
        public string Place { get; set; }
        public int Picking_QTY { get; set; }
        public int Good_QTY { get; set; }
        public int NG_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int? WIP_QTY { get; set; }
        public string Color { get; set; }
        public string Unacommpolished_Reason { get; set; }
        public string Time_Interval { get; set; }
    }
}
