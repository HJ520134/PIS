using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    /// <summary>
    /// 訊息模型
    /// </summary>
    public class MessageStatus
    {
        public string ModuleName { get; set; }
        public string M_UID { get; set; }
        public string No { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class RP_MESearch : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public string BG_Organization_Name { get; set; }
        public string Project_Name { get; set; }
        public string BU { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public bool? Is_Closed { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public int Account_UID { get; set; }
    }
    public class RP_ME_VM
    {
        public int RP_Flowchart_Master_UID { get; set; }
        public string BU_D_Name { get; set; }
        public int Project_UID { get; set; }
        public string Project_Name { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Plant_Organization_Name { get; set; }
        public int BG_Organization_UID { get; set; }
        public string BG_Organization_Name { get; set; }
        public int Daily_Targetoutput { get; set; }
        public decimal FPY { get; set; }
        public int FlowChart_Version { get; set; }
        public string FlowChart_Version_Comment { get; set; }
        public int Created_UID { get; set; }
        public string Created_UserName { get; set; }
        public DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public string Modified_UserName { get; set; }
        public DateTime Modified_Date { get; set; }
    }

    public class ProductionPlanMeVM : BaseModel
    {
        public List<PlantVM> Plants { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }

    public class RP_All_VM : BaseModel
    {
        public bool IsEdit { get; set; }
        public int AccountID { get; set; }
        public List<RP_VM> RP_VM { get; set; }
        public List<RP_ME_D_Equipment> ProcessingEquipList { get; set; }
        public List<RP_ME_D_Equipment> AutoEquipList { get; set; }
        public List<RP_ME_D_Equipment> AuxiliaryEquipList { get; set; }
    }


    public class RP_VM
    {
        public RP_M RP_M { get; set; }
        public List<RP_ME_D> RP_ME_D { get; set; }
    }

    public class RP_M
    {
        public string BU { get; set; }
        public string Project_Name { get; set; }
        public int RP_Flowchart_Master_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int Project_UID { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public string Color { get; set; }
        public int Daily_Targetoutput { get; set; }
        public decimal FPY { get; set; }
        public int FlowChart_Version { get; set; }
        public string FlowChart_Version_Comment { get; set; }
        public bool Is_Latest { get; set; }
        public bool Is_Closed { get; set; }
        public int Created_UID { get; set; }
        public string Created_User_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        public string Modified_User_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
    }

    public class RP_ME_D
    {
        public string BU { get; set; }
        public string Project_Name { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public int RP_Flowchart_Detail_ME_UID { get; set; }
        public int RP_Flowchart_Master_UID { get; set; }
        public int FunPlant_Organization_UID { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public int Process_Seq { get; set; }
        public string Process_Station { get; set; }
        public string Process { get; set; }
        public string Process_Desc { get; set; }
        public string Processing_Equipment { get; set; }
        public string Automation_Equipment { get; set; }
        public string Processing_Fixtures { get; set; }
        public string Auxiliary_Equipment { get; set; }
        public decimal? Equipment_CT { get; set; }
        public decimal? Setup_Time { get; set; }
        public decimal? Total_Cycletime { get; set; }
        public decimal ME_Estimate_Yield { get; set; }
        public decimal? Manpower_Ratio { get; set; }
        public decimal Capacity_ByHour { get; set; }
        public decimal Capacity_ByDay { get; set; }
        public decimal? Equipment_RequstQty { get; set; }
        public int? Manpower_2Shift { get; set; }
        public int Created_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
    }

    public class RP_ME_D_Equipment : BaseModel
    {
        public string Process_Station { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Process { get; set; }
        public decimal Capacity_ByHour { get; set; }
        public decimal Capacity_ByDay { get; set; }
        public decimal ME_Estimate_Yield { get; set; }

        public int RP_Flowchart_Detail_ME_Equipment_UID { get; set; }
        public int RP_Flowchart_Detail_ME_UID { get; set; }
        public string Equipment_Name { get; set; }
        public string Equipment_Spec { get; set; }
        public string Equipment_Type { get; set; }
        public decimal? Plan_CT { get; set; }
        public int Equipment_Qty { get; set; }
        public decimal? Ratio { get; set; }
        public int? Request_Qty { get; set; }
        public int? EQP_Variable_Qty { get; set; }
        public int NPI_Current_Qty { get; set; }
        public int MP_Current_Qty { get; set; }
        public string Notes { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }
        public int Created_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
        public int Modified_UID { get; set; }

        //匯入時用Key
        public int Process_Seq { get; set; }
    }

    /// <summary>
    /// 搜尋Equipment VM
    /// </summary>
    public class ME_EquipmentSearchVM : BaseModel
    {
        public int RP_Flowchart_Master_UID { get; set; }
        public string Equipment_Type { get; set; }
    }

    /// <summary>
    /// 儲存Equipment VM
    /// </summary>
    public class SaveME_EquipmentVM : BaseModel
    {
        public int RP_Flowchart_Detail_ME_Equipment_UID { get; set; }
        public decimal Equipment_Manpower_Ratio { get; set; }
        public int EQP_Variable_Qty { get; set; }
        public int NPI_Current_Qty { get; set; }
        public int Account_UID { get; set; }
    }

    #region 传参
    public class RP_ME_ExcelImportParas : BaseModel
    {
        public int FlowChart_Master_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public string Plant_Organization { get; set; }
        public int BG_Organization_UID { get; set; }
        public string BG { get; set; }
        public string Color { get; set; }
        public int Daily_Targetoutput { get; set; }
        public int FlowChart_Version { get; set; }
        public string FlowChart_Version_Comment { get; set; }
        public decimal FPY { get; set; }
        public string Project_Name { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public string BU_D_Name { get; set; }
        public bool isEdit { get; set; }
        public List<int> Organization_UIDList { get; set; }
    }
    #endregion
}
