using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.ViewModels
{
    #region 往DB发送搜索请求
    public class FlowChartModelSearch : BaseModel
    {
        public string BU_D_Name { get; set; }

        public string Project_Name { get; set; }

        public string Part_Types { get; set; }

        public string Product_Phase { get; set; }

        public int Is_Closed { get; set; }

        public int Is_Latest { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }

        public string Modified_By { get; set; }

        public List<string> OpTypes { get; set; }
        public List<int> Project_UID { get; set; }
        public List<int> ProjectUIDList { get; set; }
        public List<SystemRoleDTO> RoleList { get; set; }
        public List<int> OPType_OrganizationUIDList { get; set; }
        public List<int> PlantUIDList { get; set; }
        public int AccountId { get; set; }
        public string CurrentDepartent { set; get; }
    }
    #endregion

    #region 从DB中检索出数据加载
    public class FlowChartGet : BaseModel
    {
        public FlowChartMasterDTO FlowChartMasterDTO { get; set; }

        public SystemProjectDTO SystemProjectDTO { get; set; }

        public SystemUserDTO SystemUserDTO { get; set; }

        public string BU_D_Name { get; set; }
    }

    public class FlowChartModelGet : BaseModel
    {
        public int FlowChart_Master_UID { get; set; }

        public string Project_Name { get; set; }

        public string BU_D_Name { get; set; }

        public string Part_Types { get; set; }

        public string Product_Phase { get; set; }

        public bool Is_Closed { get; set; }

        public bool Is_Latest { get; set; }

        public int FlowChart_Version { get; set; }

        public string FlowChart_Version_Comment { get; set; }

        public string User_Name { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }

        public string User_NTID { get; set; }

        public bool IsTemp { get; set; }

        public string OP_type { set; get; }

        public int Project_UID { get; set; }

        public int Plant_OrganizationUID { get; set; }

        public int Organization_UID { get; set; }

        public string CurrentDepartent { get; set; }

        public int? Statue_IE { get; set; }
    }
    #endregion

    #region
    public class FlowChartImport : BaseModel
    {
        public FlowChartMasterDTO FlowChartMasterDTO { get; set; }

        public List<FlowChartImportDetailDTO> FlowChartImportDetailDTOList { get; set; }

        public List<int> FlowchartDetailUIDList { get; set; }

        public string Site { get; set; }

        public List<OrganiztionVM> OrganiztionVMList { get; set; }
    }

    public class FlowChartImportDetailDTO
    {
        public FlowChartDetailDTO FlowChartDetailDTO { get; set; }

        public List<FlowChartMgDataDTO> MgDataList { get; set; }

        public List<FlowChartPCMHRelationshipDTO> PCMHList { get; set; }

        //public FlowChartMgDataDTO FlowChartMgDataDTO { get; set; }

    }

    public class FLUIDAndBindSeq
    {
        public int FlowChart_Detail_UID { get; set; }
        public int Binding_Seq { get; set; }
    }
    #endregion

    #region 从DB中获取
    public class FlowChartDetailGet
    {
        public FlowChartDetailDTO FlowChartDetailDTO { get; set; }

        public FlowChartDetailTempDTO FlowChartDetailTempDTO { get; set; }

        public FlowChartMgDataDTO FlowChartMgDataDTO { get; set; }

        public FlowChartMgDataTempDTO FlowChartMgDataTempDTO { get; set; }

        public SystemUserDTO SystemUserDTO { get; set; }

        public Dictionary<int, string> FatherProcess { get; set; }

        public string FunPlant { get; set; }
        public int Organization_UID { get; set; }
    }


    public class FlowChartDetailGetByMasterInfo
    {
        public string BU_D_Name { get; set; }

        public string Project_Name { get; set; }

        public string Part_Types { get; set; }

        public string Product_Phase { get; set; }

        public List<SystemFunctionPlantDTO> SystemFunctionPlantDTOList { get; set; }
    }

    #endregion

    #region 查看历史版本从DB中获取数据
    public class FlowChartHistoryGet
    {
        public int FlowChart_Master_UID { get; set; }

        public string BU_D_Name { get; set; }

        public string Project_Name { get; set; }

        public string Product_Phase { get; set; }

        public string Part_Types { get; set; }

        public int FlowChart_Version { get; set; }

        public string FlowChart_Version_Comment { get; set; }

        public string User_Name { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
    }
    #endregion

    #region 传参
    public class FlowChartExcelImportParas : BaseModel
    {
        public string BU_D_Name { get; set; }
        public string Project_Name { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public bool isEdit { get; set; }
        public List<int> Organization_UIDList { get; set; }
    }
    #endregion

    #region 从DB中获取数据Export
    public class FlowChartExcelExport : BaseModel
    {
        public string BU_D_Name { get; set; }
        public string Project_Name { get; set; }
        public string Part_Types { get; set; }
        public string Product_Phase { get; set; }

        public SystemUserDTO SystemUserDTO { get; set; }

        public List<FlowChartDetailAndMGDataDTO> FlowChartDetailAndMGDataDTOList { get; set; }
    }

    public class FlowChartDetailAndMGDataDTO
    {
        public int FlowChart_Detail_UID { get; set; }

        public int FlowChart_Master_UID { get; set; }

        public int FlowChart_Version { get; set; }

        public int System_FunPlant_UID { get; set; }

        public int Binding_Seq { get; set; }

        public int Process_Seq { get; set; }

        public string DRI { get; set; }

        public string Place { get; set; }

        public string Process { get; set; }

        public string PlantName { get; set; }

        public string IsQAProcess { get; set; }

        public string Rework_Flag { get; set; }

        public string IsQAProcessName { get; set; }

        public int Product_Stage { get; set; }

        public int WIP_QTY { get; set; }

        public string Color { get; set; }

        public string Process_Desc { get; set; }

        public string ItemNo { get; set; }

        public string FromWHS { get; set; }

        public string ToWHSOK { get; set; }

        public string ToWHSNG { get; set; }

        public string Edition { get; set; }

        public bool Location_Flag { get; set; }
        public string RelatedRepairUID { get; set; }
        public string RepairJoin { get; set; }
        public int Current_WH_QTY { get; set; }

        public int NullWip { get; set; }

        public string Data_Source { get; set; }
        public bool Is_Synchronous { get; set; }

        //public double Target_Yield { get; set; }

        //public int Product_Plan { get; set; }
    }
    #endregion

    #region 从DBDetail中批量获取页面的数据
    public class FlowChartDetailAndMGDataInputDTO : BaseModel
    {
        public int FlowChart_Detail_UID { get; set; }

        public int Product_Plan { get; set; }

        public string Target_Yield { get; set; }
    }
    #endregion

    #region 获取下一周的日期
    public class Week
    {
        public DateTime Monday { get; set; }

        public DateTime Tuesday { get; set; }

        public DateTime Wednesday { get; set; }

        public DateTime Thursday { get; set; }

        public DateTime Friday { get; set; }

        public DateTime Saturday { get; set; }

        public DateTime Sunday { get; set; }
    }
    #endregion


    #region 绑定物料员从DB中获取
    public class FlowChartBomGet : BaseModel
    {
        public int FlowChart_Detail_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public int PC_MH_UID { get; set; }
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public string Place { get; set; }
        public string Color { get; set; }
        public string User_NTID { get; set; }
        public string User_Name { get; set; }
        public int Modified_UID { get; set; }
        public string Modified_NTID { get; set; }
        public string Modified_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }
        public int Binding_Seq { get; set; }
    }
    #endregion

    public class GetProcessSearch:BaseModel
    {
        public int user_account_uid { get; set; }
        public List<string> OpTypes { get; set; }
        public List<int> Project_UID { get; set; }
        public bool MHFlag_MulitProject { get; set; }
    }

    public class GetFuncPlantProcessSearch : BaseModel
    {
        public int Master_Uid { get; set; }
        public int Version { get; set; }
        public List<string> OwnerFuncPlant { get; set; }
    }

    public class FlowchartMeNPI : BaseModel
    {
        public int id { get; set; }
        public int Version { get; set; }
        public string Modified_Date_From { get; set; }
        public string Modified_Date_End { get; set; }
    }

    public class WIPDetialSearchParam: BaseModel
    {
        public string factoryAddress { get; set; }
        public string OpType { get; set; }
        public string Process { get; set; }
        public string Project { get; set; }
        public string FunPlant { get; set; }
        public string Color { get; set; }
        public string partType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AlterPerson { get; set; }
    }

    public class WIPDetialParam : BaseModel
    {
        public string ProjectName { get; set; }
        public string Plant { get; set; }
        public string OPType { get; set; }
        public string PartType { get; set; }
        public string FlowChart_Master_UID { get; set; }
        public string FunPlant { get; set; }
        public string Select_Type { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
