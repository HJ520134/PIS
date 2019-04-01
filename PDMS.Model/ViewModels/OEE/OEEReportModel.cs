using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public enum MachineIndexName
    {
        /// <summary>
        /// # of Fixtures
        /// </summary>
        [Description("# of Fixtures 治具数量")]
        Fixtures = 0,

        /// <summary>
        /// POR ( Provider of Record)CT 供应商的记录周期时间
        /// </summary>
        [Description("POR ( Provider of Record)CT   供应商的记录周期时间")]
        POR = 1,

        /// <summary>
        /// Actual CT 实际的周期时间
        /// </summary>
        [Description("Actual CT 实际的周期时间")]
        ActualCT = 2,

        /// <summary>
        /// CT Achivement Rate 周期成功率
        /// </summary>
        [Description("CT Achivement Rate 周期成功率")]
        CTAchivementRate = 3,

        /// <summary>
        /// Total Available Hour 总的可用时间
        /// </summary>
        [Description("Total Available Hour (Hour)  总的可用时间 ")]
        TotalAvailableHour = 4,

        /// <summary>
        /// Planned Hour 计划时间（小时）
        /// </summary>
        [Description("Planned Hour (Hour) 计划时间（小时）")]
        PlannedHour = 5,

        /// <summary>
        /// Planned Minutes 计划时间（分钟）
        /// </summary>
        [Description("Planned Minutes(Minute) 计划时间（分钟）")]//计划时间（分钟）
        PlannedMinute = 6,

        /// <summary>
        /// Planned Output 计划产量
        /// </summary>
        [Description("Planned Output 计划产量")]
        PlannedOutput = 7,

        [Description("Uptime（Minute）正常运行时间（分钟）")]
        UptimeMinute = 8,

        /// <summary>
        /// Running Capacity   运行生产能力
        /// </summary>
        [Description("Running Capacity 机械产能")]
        RunningCapacity = 9,



        /// <summary>
        /// Throughput 生产量
        /// </summary>
        [Description("Throughput 生产量")]
        Throughput = 10,

        /// <summary>
        /// Good Part Output 良品数量
        /// </summary>
        [Description("Good Part Output 良品数量")]
        GoodPartOutput = 11,

        /// <summary>
        /// NG QTY 不良品数量
        /// </summary>
        [Description("NG QTY 不良品数量")]
        NGQTY = 12,


        [Description("First pass yield % 通过率")]
        FirstPassYield = 13,

        /// <summary>
        /// AV% 设备时间开动率
        /// </summary>
        [Description("AV% 设备时间开动率")]
        AvailableRate = 14,

        /// <summary>
        /// PF% 性能开动
        /// </summary>
        [Description("PF% 性能开动")]
        PerformanceRate = 15,

        /// <summary>
        /// OEE% (w/o Microstop) 整体设备效率 （不包含微停）
        /// </summary>
        [Description("OEE% (w/o Microstop) 整体设备效率 （不包含微停）")]
        EquipmentEfficiency = 16,

        /// <summary>
        /// OEE% 整体设备效率
        /// </summary>
        [Description("OEE% 整体设备效率")]
        AllEquipmentEfficiency = 17,

        /// <summary>
        /// Production time loss (Hour) 生产损失（小时）
        /// </summary>
        [Description("Production time loss (Hour)  生产损失 （小时）")]
        ProductionTimeLoss = 18,

    };

    public enum DowntimeBreakdownEnum
    {

        [Description("Equip -  Pneumatic/Hydraulics  设备 - 气动 / 液压")]
        EquipPneumaticHydraulics = 0,

        [Description("Equip - mechanical 设备 - 机械")]
        EquipMechanical = 1,

        [Description("Equipment - Electronics 设备 - 电子器件")]
        EquipmentElectronics = 2,

        [Description("Equipment- PM 设备 - 保养")]
        EquipmentPM = 3,

        [Description("Process- adjustment 工艺流程 - 调试")]
        ProcessAdjustment = 4,

        [Description("Process- consumable 工艺流程 - 耗材")]
        ProcessConsumable = 5,

        [Description("Other 其他")]
        Other = 6,

        [Description("Uptime（Minute）正常运行时间（分钟）")]
        UptimeMinute = 7,

        [Description("First pass yield % 通过率")]
        FirstPassYield = 8,
    }

    public class MachineIndexModel
    {
        public string DownTimeType { get; set; }
        public string IndexName { get; set; }
        public string IndexCount { get; set; }
        public double Percentage { get; set; }
        public DateTime MachineDate { get; set; }
        public double TotalStaticCount { get; set; }
        public int OEE_DownTimeType_UID { get; set; }

        public int OEE_MachineInfo_UID { get; set; }
        public string MachineName { get; set; }
        public int Station_UID { get; set; }
        public string Station_Name { get; set; }
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
        /// <summary>
        /// DtCode 异常
        /// </summary>
        public bool Is_DtCodeAbnormal { get; set; }

        public bool Is_CncResetAbnomal { get; set; }
        public bool Is_DfCeodeAbnomal { get; set; }
        public bool Is_OeeAbnomal { get; set; }
        public bool Is_OfflineAbnomal { get; set; }

        /// <summary>
        /// 是否显示OffLine
        /// </summary>
        public bool Is_ShowOffLine { get; set; }
        public string ResetTime { get; set; }

        public string Color { get; set; }
        public string DashBoardTarget { get; set; }

        public double FirstDashBoardTarget
        { get; set; }
        /// <summary>
        /// OEE 仪表盘 二级目标设置
        /// </summary>
        public double SecondDashBoardTarget
        { get; set; }
    }

    public class MetricsPhotoModel
    {
        public string AllEquipmentEfficiency { get; set; }
        public string AvailableRate { get; set; }
        public string PerformanceRate { get; set; }
        public string FirstYield { get; set; }
        public string EquipmentDown { get; set; }
        public string ProcessDown { get; set; }

        public string Other { get; set; }
        public string Maitenance { get; set; }
        public string CTDiscrepacies { get; set; }
        public string OtherMicrostop { get; set; }
        public string Quality { get; set; }

        /// <summary>
        /// DtCode 异常
        /// </summary>
        public bool Is_DtCodeAbnormal { get; set; }

        public bool Is_CncResetAbnomal { get; set; }

        public bool Is_OeeAbnomal { get; set; }
        public bool Is_DFCodeAbnomal { get; set; }
        public string ResetTime { get; set; }
        public double FirstDashBoardTarget
        { get; set; }
        /// <summary>
        /// OEE 仪表盘 二级目标设置
        /// </summary>
        public double SecondDashBoardTarget
        { get; set; }
    }

    public class MachinePieIndexModel
    {
        public string OEEValue { get; set; }
        public string AvailableRate { get; set; }
        public string PerformanceRate { get; set; }
        public string FirstYield { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public string MachineName { get; set; }
        public int Station_UID { get; set; }
        public string StationName { get; set; }
        public string IsTotalStationStatic { get; set; }

        /// <summary>
        /// DtCode 异常
        /// </summary>
        public bool Is_DtCodeAbnormal { get; set; }

        public bool Is_CncResetAbnomal { get; set; }
        public bool Is_DfCodeAbnomal { get; set; }
        public bool Is_OeeAbnomal { get; set; }

        /// <summary>
        ///  是否断网
        /// </summary>
        public bool Is_OffLine{ get; set; }


        /// <summary>
        /// 是否显示OffLine
        /// </summary>
        public bool Is_ShowOffLine { get; set; }
        
        /// <summary>
        /// OEE 仪表盘一级的目标设置
        /// </summary>
        public double FirstDashBoardTarget
          { get; set; }
        /// <summary>
        /// OEE 仪表盘 二级目标设置
        /// </summary>
        public double SecondDashBoardTarget
        { get; set; }

        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
    }


    /// <summary>
    /// 不良大类对应颜色
    /// </summary>
    public enum BreakDownColorConfig
    {
        [Description("#C1232B")]
        Color1 = 1,

        [Description("#B5C334")]
        Color2 = 2,

        [Description("#FCCE10")]
        Color3 = 3,

        [Description("#E87C25")]
        Color4 = 4,

        [Description("#27727B")]
        Color5 = 5,

        [Description("#FE8463")]
        Color6 = 6,

        [Description("#9BCA63")]
        Color7 = 7,

        [Description("#FAD860")]
        Color8 = 8,

        [Description("#F3A43B")]
        Color9 = 9,

        [Description("#60C0DD")]
        Color10 = 10,

        [Description("#D7504B")]
        Color11 = 11,

        [Description("#C6E579")]
        Color12 = 12,

        [Description("#F4E001")]
        Color13 = 13,

        [Description("#F0805A")]
        Color14 = 14,

        [Description("#26C0C0")]
        Color15 = 15,
    }
}
