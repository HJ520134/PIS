using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PDMS.Model.ViewModels
{
    public class ZeroFunPlantInfo
    {

        public List<ZeroProcessDataSearch> ZeroList { get; set; }
    }

    public class ZeroProcessDataSearch : BaseModel
    {
        public int Create_User { get; set; }
        public DateTime Create_Time { get; set; }
        public string Func_Plant { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string QuertFlag { get; set; }
    }

    public class ProcessDataSearchModel : BaseModel
    {
        public string Func_Plant { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        
    }

    public class ProcessDataSearch : BaseModel
    {
        public string Func_Plant { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string QuertFlag { get; set; }
        public List<string> OpTypes { get; set; }
        public List<int> Project_UID { get; set; }
        public int Account_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
    }

    public class ProcessInfo : BaseModel
    {
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
    }
    public class ProductDataView : BaseModel
    {
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
    public class ProductDataList : BaseModel
    {
        public List<ProductDataItem> ProductLists { get; set; }
    }

    public class ReworkItem
    {
        public int Rework_UID { get; set; }
        public string reworkOper { get; set; }
        public int reworkQty { get; set; }
        //public int reworkUid { get; set; }
        public int reworkDetailUid { get; set; }
        public int detailuid { get; set; }
        public string Rework_Type { get; set; }
        public string Rework_Flag { get; set; }
        //public int projectuid { get; set; }
    }
    public class PDInputLocaton:BaseModel
    {
        public string Process { get; set; }
        public string Color { get; set; }
    }
    public class ProductDataItem : BaseModel
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
        public string DRI { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int Picking_QTY { get; set; }
        public int WH_Picking_QTY { get; set; }
        public string Picking_MismatchFlag { get; set; }
        public int NG_QTY { get; set; }
        public int WH_QTY { get; set; }
        public int WIP_QTY { get; set; }
        public int NullWip_QTY { get; set; }
        public int Adjust_QTY { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public string Material_No { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string Rework_QTY { get; set; }
        public int INum { get; set; }
        public int ONum { get; set; }
        //Q报表数据准备
        public int Normal_Good_QTY { get; set; }
        public int Abnormal_Good_QTY { get; set; }
        public int Normal_NG_QTY { get; set; }
        public int Abnormal_NG_QTY { get; set; }
        public string IsRepair { get; set; }
        public List<ReworkItem> ReworkList { get; set; }
        public bool Location_Flag { get; set; }
        public string Unacommpolished_Reason { get; set; }
        //一个Repair对应多个Rework站点
        //一个Rework站点里面有多笔返工资料
        //所以这个list里面还要对应一个list
        //public List<NewInfo_ReworkList> NewInfo_ReworkList { get; set; }
        public List<ProductReworkInfoVM> ProductReworkInfoVM { get; set; }
        public List<ProductReworkInfoVM> ProductRepairInfoVM { get; set; }
    }


    public class YieldChart : BaseModel
    {
        public int Prouct_Plan { get; set; }
        public string Process { get; set; }
        public int Good_QTY { get; set; }
        public int WH_QTY { get; set; }
    }

    public class YieldChartSearch : BaseModel
    {
        public string Customer { get; set; }
        public string Project { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
    }

    public class ReworkDatas : BaseModel
    {
        public string reworkOper { get; set; }
        public int? reworkQty { get; set; }
        public int? reworkUid { get; set; }
        public int? reworkDetailUid { get; set; }
    }

    public class ReworkList: EntityDTOBase
    {
        public int Product_Uid { get; set; }
        public int Detail_Uid { get; set; }
        public List<ReworkDatas> ReworkDatas { get; set; }
    }

    public class GetReworkOper : BaseModel
    {
        public string Process { get; set; }
        public string Color { get; set; }
        public string Detail_UID { get; set; }
        public string Opposite_UID { get; set; }
        public string Rework_UID { get; set; }
        public string Rework_QTY { get; set; }
        public string Is_Match { get; set; }
        public string Rework_Type { get; set; }
        public string Rework_Flag { get; set; }
    }

    public class IPQCOper : BaseModel
    {
        public int BeforeSeq { get; set; }
        public int AfterSeq { get; set; }
    }

    public class IPQC_NG
    {
        public int Normal_NG_QTY { get; set; }
        public int Abnormal_NG_QTY { get; set; }
    }

    public class IPQC_Input
    {
        public int INPUT_Normal { get; set; }
        public int Input_Abnormal { get; set; }
    }

    public class RepeatItem
    {
        public int FlowChart_Detail_UID { get; set; }
        public int Opposite_Detail_UID { get; set; }
        public string Rework_Type { get; set; }
    }

}
