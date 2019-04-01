using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDMS.Model.EntityDTO;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.ViewModels
{

    public class FixtureVM : BaseModel
    {
        public List<PlantVM> Plants { get; set; }
        public List<SystemProjectDTO> optypes { get; set; }
        public List<FixtureStatuDTO> FixtureStatus { get; set; }
        public List<string> WorkScheduleList { get; set; }
        public List<FixtureStatuDTO> SearchFixtureStatus { get; set; }
        public int OptypeID { get; set; }
        public int FunPlantID { get; set; }
    }

    public class FixtureResumeSearchVM : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }


        public int Fixture_Resume_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public string Data_Source { get; set; }
        public DateTime Resume_Date { get; set; }
        public int Source_UID { get; set; }
        public string Source_NO { get; set; }
        public string Resume_Notes { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public string Process_Name { get; set; }
        public int WorkStation_UID { get; set; }
        public int Process_Info_UID { get; set; }
        public int Production_Line_UID { get; set; }
        public string WorkStation_Name { get; set; }
        public string Equipment_No { get; set; }
        public string Fixture_NO { get; set; }
        public int Status { get; set; }
        public string FixStatus_Name { get; set; }
        public string Version { get; set; }
        public string Vendor_Name { get; set; }
        public string Line_Name { get; set; }
        public string ShortCode { get; set; }
        public string Fixture_Name { get; set; }
        public int Modified_UID { get; set; }
        public string User_Name { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Modified_Date { get; set; }
        /// <summary>
        /// 时间段开始
        /// </summary>
        public System.DateTime? End_Date_From { get; set; }
        /// <summary>
        /// 时间段结束
        /// </summary>
        public System.DateTime? End_Date_To { get; set; }
    }

    public class ViewResumeByUID : BaseModel
    {
        public int Fixture_Repair_D_UID { get; set; }
        //送修人UID
        public int SentOut_UID { get; set; }
        public string SentOut_Number { get; set; }
        //送修人
        public string SendName { get; set; }
        //送修时间
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? SendDate { get; set; }
        //领用人UID
        public int Totake_UID { get; set; }
        public string Totake_Number { get; set; }
        //领用人
        public string TotakeName { get; set; }
        //领用时间
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Totake_Date { get; set; }
        //保养人
        public string MaintenanceName { get; set; }
        //保养时间
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Maintenance_Date { get; set; }
        //确认人
        public string ConfirmorName { get; set; }
        //确认时间
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Confirm_Date { get; set; }
        public string Cycle_ID { get; set; }
        public string Cycle_Unit { get; set; }
        public string CycleValue { get; set; }
        public int Cycle_Interval { get; set; }
        public string Maintenance_Type { get; set; }
        public string Customer { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public List<DefectRepairSolution> DRList { get; set; }
        public string Maintenance_Person_Number { get; set; }
        public string Maintenance_Person_Name { get; set; }
        public string UpdateName { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Modified_Date { get; set; }
    }

    public class DefectRepairSolution
    {
        public string DefectCode_Name { get; set; }
        public string RepairSolution_Name { get; set; }
    }

    public class MainTenanceStatus
    {
        public string Cycle_UID { get; set; }
        public string CycleValue { get; set; }
    }

    public class NotMaintenanceSearchVM : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int Fixture_Maintenance_Profile_UID { get; set; }
        public string Process_Name { get; set; }
        public string WorkStation_Name { get; set; }
        public string ProjectName { get; set; }
        public string Line_Name { get; set; }
        public int QueryType { get; set; }
        public DateTime QueryDate { get; set; }
        public DateTime QueryDate_To { get; set; }
        public string DataFormat { get; set; }
        public string MaintenanceType { get; set; }
        public int Maintenance_Status { get; set; }
        public int? Confirm_Status { get; set; }
        public int? Confirmor_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int Maintenance_Plan_UID { get; set; }
        public int WorkStation_UID { get; set; }
        public int Process_Info_UID { get; set; }
        public int Production_Line_UID { get; set; }
        public string Equipment_No { get; set; }
        public string ShortCode { get; set; }
        public string CycleValue { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public string Fixture_NO { get; set; }
        public string MaintenanceName { get; set; }
        public string ConfirmName { get; set; }
        public string Version { get; set; }
        public string Fixture_Name { get; set; }
        public string TwoD_Barcode { get; set; }
        public string Fixture_Seq { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Vendor_Name { get; set; }
        public string BU_Name { get; set; }
        public string Machine_Type { get; set; }
        public string Cycle_ID { get; set; }
        public int Cycle_Interval { get; set; }
        public string Cycle_Unit { get; set; }
    }

    public class ReportByRepair : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int WorkStation_UID { get; set; }
        public string WorkStation_Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int QueryType { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public int Fixture_M_UID { get; set; }
        public string Fixture_NO { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public int Fixture_Defect_UID { get; set; }
        public string DefectCode_ID { get; set; }
        public string DefectCode_Name { get; set; }
        public int Repair_Staff_UID { get; set; }
        public string RepairName { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? SentOut_Date { get; set; }
        public DateTime? Completion_Date { get; set; }
        public int TotalCount { get; set; }
        //导出数据，一次性导出所有，用来判断sheet1和sheet2
        public int sheetCount { get; set; }
        //前端传递到后台的json导出数据
        public string JsonValue { get; set; }
        public string Version { get; set; }
        public int DisplayCount { get; set; }
        public bool WorkStation_Is_Enable { get; set; }
        public bool Production_Line_Is_Enable { get; set; }
        public int hidType { get; set; }
        public int Repair_Location_UID { get; set; }
        public string Repair_Location_Name { get; set; }
        public bool Repair_Is_Enable { get; set; }
        public string Fixture_Status { get; set; }
        public int Fixture_Status_UID { get; set; }
        public int? TimeInterval { get; set; }
        public int LessHalfMinutes { get; set; }
        public int LessTwoHour { get; set; }
        public int LessFourHour { get; set; }
        public int OtherHour { get; set; }
    }

    public class ReportByStatusAnalisis
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int WorkStation_UID { get; set; }
        public string WorkStation_Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        //治具总数
        public int TotalCount { get; set; }
        //使用中
        public decimal StatusOne { get; set; }
        //未使用
        public decimal StatusTwo { get; set; }
        //维修中
        public decimal StatusThree { get; set; }
        //报废
        public decimal StatusFour { get; set; }
        //返供应商维修RTV
        public decimal StatusFive { get; set; }
        //保养逾时
        public decimal StatusSix { get; set; }
        //已保养
        public decimal StatusSeven { get; set; }
        //未保养
        public decimal StatusEight { get; set; }
    }

    public class Batch_ReportByStatus : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public int WorkStation_UID { get; set; }
        public string WorkStation_Name { get; set; }
        public int Process_Info_UID { get; set; }
        public string Process_Name { get; set; }
        //治具总数
        public int TotalCount { get; set; }
        //当日新增总数
        public int NewCount { get; set; }
        //当日报废总数
        public int FreeCount { get; set; }
        //当日送修数
        public int SendRepairCount { get; set; }
        //当日领用数
        public int ShipCount { get; set; }
        //当日待修数
        public int WaitRepairCount { get; set; }
        //当日应保养数
        public int NeedMaintenCount { get; set; }
        //当日已保养数
        public int HasMaintenCount { get; set; }
        //当日未保养数
        public int NotMaintenCount { get; set; }
        //前台数据转换
        public string JsonValue { get; set; }
        public string hidDate { get; set; }
        public int SheetCount { get; set; }
    }

    public class Batch_FMTNew
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int Process_Info_UID { get; set; }
        public int Fixture_Resume_UID { get; set; }
        public string Data_Source { get; set; }
        public string Resume_Notes { get; set; }
        public DateTime Modified_Date { get; set; }
    }

    public class Batch_FMTRepair
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int Fixture_Repair_D_UID { get; set; }
        public int Fixture_M_UID { get; set; }
    }

    public class Batch_FMTTotake
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int Fixture_Totake_D_UID { get; set; }
        public int Fixture_M_UID { get; set; }
    }

    public class Batch_FMTHasMainten
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int Fixture_Maintenance_Record_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int Maintenance_Status { get; set; }
        public DateTime? Maintenance_Date { get; set; }
        public DateTime Created_Date { get; set; }
        public int Modified_UID { get; set; }
    }

    public class Batch_TotalFix
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int Fixture_M_UID { get; set; }
        public int Status { get; set; }
        public int Process_Info_UID { get; set; }
        public int Fixture_Resume_UID { get; set; }
    }

    public class Batch_SendEmail
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string User_Name_CN { get; set; }
        public string User_Name_EN { get; set; }
        public string Email { get; set; }
    }

    public class AppConfigModel
    {
        public string name { get; set; }
        public string runtime { get; set; }
        public string cycle { get; set; }
        public string enable { get; set; }
        public string ismail { get; set; }
    }
}
