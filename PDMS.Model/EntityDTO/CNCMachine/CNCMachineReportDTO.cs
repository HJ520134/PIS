using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class CNCMachineReportDTO
    {
        /// <summary>
        /// 机台名称
        /// </summary>
        [Description("机台名称")]
        public string MachineName { get; set; }
        /// <summary>
        /// 机台编号
        /// </summary>
        [Description("机台编号")]
        public string AuthCode { get; set; }
        /// <summary>
        /// 机台SN号
        /// </summary>
        [Description("机台SN号")]
        public string dSn { get; set; }
        /// <summary>
        /// 機台狀態
        /// </summary>
        [Description("機台狀態")]
        public string Status { get; set; }
        /// <summary>
        /// 工件加工C/T
        /// </summary>
        [Description("工件加工C/T(sec)")]
        public string ProcessCycleTm { get; set; }
        /// <summary>
        /// 加工工件數
        /// </summary>
        [Description("加工工件數")]
        public string TotalNum { get; set; }
        /// <summary>
        /// X軸的負載
        /// </summary>
        [Description("X軸的負載(%)")]
        public string X_load { get; set; }
        /// <summary>
        ///  Y軸的負載
        /// </summary>
        [Description("Y軸的負載(%)")]
        public string Y_load { get; set; }
        /// <summary>
        /// Z軸的負載
        /// </summary>
        [Description("Z軸的負載(%)")]
        public string Z_load { get; set; }
        /// <summary>
        ///  A軸的負載
        /// </summary>
        [Description("A軸的負載(%)")]
        public string A_load { get; set; }
        /// <summary>
        /// 刀具壽命
        /// </summary>
        [Description("刀具壽命(次)")]
        public string ToolSum { get; set; }
        /// <summary>
        /// 主軸轉速（S）
        /// </summary>
        [Description("主軸轉速(rpm)")]
        public int S_RPM { get; set; }
        /// <summary>
        /// 進給(F)
        /// </summary>
        [Description("進給(F)(毫米/分)")]
        public string Feed { get; set; }
        /// <summary>
        /// 四軸角度
        /// </summary>
        [Description("四軸角度")]
        public string Axisangle { get; set; }
        /// <summary>
        /// 宏變量
        /// </summary>
        [Description("宏變量")]
        public string Mcode { get; set; }
        /// <summary>
        /// 主轴誤差
        /// </summary>
        [Description("主轴誤差")]
        public string S_Gap { get; set; }
        /// <summary>
        /// 轴名称
        /// </summary>
        [Description("轴名称")]
        public string ServoName { get; set; }
        /// <summary>
        /// 主轴负载
        /// </summary>
        [Description("主轴负载(%)")]
        public string S_Load { get; set; }
        /// <summary>
        /// 开机时间(Min)
        /// </summary>
        [Description("开机时间(Min)")]
        public int PowerOnTm { get; set; }
        /// <summary>
        /// 切削时间
        /// </summary>
        [Description("切削时间(Min)")]
        public int CuttingTm { get; set; }
        /// <summary>
        /// 循环时间
        /// </summary>
        [Description("循环时间(sec)")]
        public int CycleRunTm { get; set; }
        /// <summary>
        /// 当前刀具号
        /// </summary>
        [Description("当前刀具号")]
        public string CurrenttoolNo { get; set; }
        /// <summary>
        /// 报警号
        /// </summary>
        [Description("报警号")]
         public string  ErrorCode { get; set; }       
        /// <summary>
        /// 报警信息
        /// </summary>
        [Description("报警信息")]
        public string ErrorDels { get; set; }
        /// <summary>
        /// 倍率（快移）
        /// </summary>
        [Description("倍率（快移）")]
        public string RateJOG { get; set; }
        /// <summary>
        /// 倍率（切削） 
        /// </summary>
        [Description("倍率（切削）")]
        public string RateFastmove { get; set; }
        /// <summary>
        /// 倍率（JOG） 
        /// </summary>
        [Description("倍率（JOG）")]
        public string RateJOG1 { get; set; }
        /// <summary>
        /// 倍率（HAND）
        /// </summary>
        [Description("倍率（HAND）")]
        public string RateHAND { get; set; }
        /// <summary>
        ///  倍率（主轴）
        /// </summary>
        [Description("倍率（主轴）")]
        public string RateS { get; set; }
        /// <summary>
        /// 快速进给时间常数
        /// </summary>
        [Description("快速进给时间常数")]
        public string FastmoveTmC { get; set; }
        /// <summary>
        /// 快移速度
        /// </summary>
        [Description("快移速度")]
        public string Fastmovespeed { get; set; }
        /// <summary>
        /// 最大控制轴数
        /// </summary>
        [Description("最大控制轴数")]
        public int MaxAxisNo { get; set; }
        /// <summary>
        /// CNC模式
        /// </summary>
        [Description("CNC模式")]
        public string CNCMode { get; set; }
        /// <summary>
        /// 主程序号
        /// </summary>
        [Description("主程序号")]
        public string MainProgram { get; set; }
        /// <summary>
        /// 当前程序号
        /// </summary>
        [Description("当前程序号")]
        public string NowProgram { get; set; }
        /// <summary>
        /// 程序段号
        /// </summary>
        [Description("程序段号")]
        public string ProgramStep { get; set; }
        /// <summary>
        /// 单段备注
        /// </summary>
        [Description("单段备注")]
        public string Singlenote { get; set; }
        /// <summary>
        /// 程序备注
        /// </summary>
        [Description("程序备注")]
        public string Programnote { get; set; }
        /// <summary>
        /// 主轴指定速度
        /// </summary>
        [Description("主轴指定速度(rpm)")]
        public string S_SetSpeed { get; set; }
        /// <summary>
        /// 主轴实际速度
        /// </summary>
        [Description("主轴实际速度(rpm)")]
        public string S_Speed { get; set; }
        /// <summary>
        /// 系统IP地址
        /// </summary>
        [Description("系统IP地址")]
        public string IPaddress { get; set; }
        /// <summary>
        /// CNC型号
        /// </summary>
        [Description("CNC型号")]
        public string CNCModel { get; set; }
        /// <summary>
        /// CNC类型
        /// </summary>
        [Description("CNC类型")]
        public string CNCType { get; set; }
        /// <summary>
        /// CNC序列号
        /// </summary>
        [Description("CNC序列号")]
        public string CNCSequence { get; set; }
        /// <summary>
        /// CNC版本号
        /// </summary>
        [Description("CNC版本号")]
        public string CNCVersion { get; set; }
        /// <summary>
        /// Customer ID/ Name
        /// </summary>
        [Description("Customer ID/ Name")]
        public string Customer { get; set; }

        /// <summary>
        /// Site
        /// </summary>
        [Description("Site")]
        public string Site { get; set; }   
        /// <summary>
        /// Building
        /// </summary>
        [Description("Building")]
        public string Building { get; set; }
        /// <summary>
        /// Product ID
        /// </summary>
        [Description("Product ID")]
        public string ProductID { get; set; }
        /// <summary>
        /// Process name
        /// </summary>
        [Description("Process name")]
        public string ProcessName { get; set; }
        /// <summary>
        /// Operator ID
        /// </summary>
        [Description("Operator ID")]
        public string OperatorID { get; set; }
        /// <summary>
        /// 伺服温度
        /// </summary>
        [Description("伺服温度(℃)")]
        public string ServoTemp { get; set; }
        /// <summary>
        /// 主軸溫度
        /// </summary>
        [Description("主軸溫度(℃)")]
        public string S_Temp { get; set; }
        /// <summary>
        /// 主軸扭力
        /// </summary>
        [Description("主軸扭力")]
        public string S_Torque { get; set; }
        /// <summary>
        /// 停機時主軸原始座標X/Y/Z/A
        /// </summary>
        [Description("停機時主軸原始座標X/Y/Z/A")]
        public string XYZA_load { get; set; }
        /// <summary>
        /// 切削液狀態
        /// </summary>
        [Description("切削液狀態")]
        public string Cuttingfluid { get; set; }
        /// <summary>
        /// 切削指定速度 
        /// </summary>
        [Description("切削指定速度")]
        public string CuttingSpeed { get; set; }
        /// <summary>
        /// 伺服实际速度 
        /// </summary>
        [Description("伺服实际速度")]
        public string ActualSpeed { get; set; }
        /// <summary>
        /// 当前控制轴数 
        /// </summary>
        [Description("当前控制轴数")]
        public int UseAxisNo { get; set; }
        /// <summary>
        /// 报警类型 
        /// </summary>
        [Description("报警类型")]
        public string ErrorType { get; set; }

        /// <summary>
        /// 對刀儀
        /// </summary>
        [Description("對刀儀")]
        public string Toolset { get; set; }
        /// <summary>
        /// 主軸熱伸長
        /// </summary>
        [Description("主軸熱伸長(mm)")]
        public string S_TE { get; set; }
        /// <summary>
        ///  每次加工起始時間
        /// </summary>
        [Description("每次加工起始時間")]
        public string ProcessStartTm { get; set; }
        /// <summary>
        /// 伺服负载
        /// </summary>
        [Description("主轴负载(%)")]
        public string SF_Load { get; set; }
        /// <summary>
        ///  關機時間 (啟/停)
        /// </summary>
        //[Description("關機時間 (啟/停)")]
        //public string ShutdownTm { get; set; }
    }


    public class CNCMachineHisReportDTO
    {
        /// <summary>
        /// 机台名称
        /// </summary>
        [Description("机台名称")]
        public string MachineName { get; set; }
        /// <summary>
        /// 机台编号
        /// </summary>
        [Description("机台编号")]
        public string AuthCode { get; set; }
        /// <summary>
        /// 机台SN号
        /// </summary>
        [Description("机台SN号")]
        public string dSn { get; set; }
        /// <summary>
        /// 機台狀態
        /// </summary>
        [Description("機台狀態")]
        public string Status { get; set; }
        /// <summary>
        /// 工件加工C/T
        /// </summary>
        [Description("工件加工C/T")]
        public string ProcessCycleTm { get; set; }
        /// <summary>
        /// 加工工件數
        /// </summary>
        [Description("加工工件數")]
        public string TotalNum { get; set; }
        /// <summary>
        /// X軸的負載
        /// </summary>
        [Description("X軸的負載")]
        public string X_load { get; set; }
        /// <summary>
        ///  Y軸的負載
        /// </summary>
        [Description("Y軸的負載")]
        public string Y_load { get; set; }
        /// <summary>
        /// Z軸的負載
        /// </summary>
        [Description("Z軸的負載")]
        public string Z_load { get; set; }
        /// <summary>
        ///  A軸的負載
        /// </summary>
        [Description("A軸的負載")]
        public string A_load { get; set; }
        /// <summary>
        /// 刀具壽命
        /// </summary>
        [Description("刀具壽命")]
        public string ToolSum { get; set; }
        /// <summary>
        /// 主軸轉速（S）
        /// </summary>
        [Description("主軸轉速（S）")]
        public int S_RPM { get; set; }
        /// <summary>
        /// 進給(F)
        /// </summary>
        [Description("進給(F)")]
        public string Feed { get; set; }
        /// <summary>
        /// 四軸角度
        /// </summary>
        [Description("四軸角度")]
        public string Axisangle { get; set; }
        /// <summary>
        /// 宏變量
        /// </summary>
        [Description("宏變量")]
        public string Mcode { get; set; }
        /// <summary>
        /// 主轴誤差
        /// </summary>
        [Description("主轴誤差")]
        public string S_Gap { get; set; }
        /// <summary>
        /// 轴名称
        /// </summary>
        [Description("轴名称")]
        public string ServoName { get; set; }
        /// <summary>
        /// 主轴负载
        /// </summary>
        [Description("主轴负载")]
        public string S_Load { get; set; }
        /// <summary>
        /// 开机时间(Min)
        /// </summary>
        [Description("开机时间(Min)")]
        public int PowerOnTm { get; set; }
        /// <summary>
        /// 切削时间
        /// </summary>
        [Description("切削时间")]
        public int CuttingTm { get; set; }
        /// <summary>
        /// 循环时间
        /// </summary>
        [Description("循环时间")]
        public int CycleRunTm { get; set; }
        /// <summary>
        /// 当前刀具号
        /// </summary>
        [Description("当前刀具号")]
        public string CurrenttoolNo { get; set; }
        /// <summary>
        /// 报警号
        /// </summary>
        [Description("报警号")]
        public string ErrorCode { get; set; }
        /// <summary>
        /// 报警信息
        /// </summary>
        [Description("报警信息")]
        public string ErrorDels { get; set; }
        /// <summary>
        /// 倍率（快移）
        /// </summary>
        [Description("倍率（快移）")]
        public string RateJOG { get; set; }
        /// <summary>
        /// 倍率（切削） 
        /// </summary>
        [Description("倍率（切削）")]
        public string RateFastmove { get; set; }
        /// <summary>
        /// 倍率（JOG） 
        /// </summary>
        [Description("倍率（JOG）")]
        public string RateJOG1 { get; set; }
        /// <summary>
        /// 倍率（HAND）
        /// </summary>
        [Description("倍率（HAND）")]
        public string RateHAND { get; set; }
        /// <summary>
        ///  倍率（主轴）
        /// </summary>
        [Description("倍率（主轴）")]
        public string RateS { get; set; }
        /// <summary>
        /// 快速进给时间常数
        /// </summary>
        [Description("快速进给时间常数")]
        public string FastmoveTmC { get; set; }
        /// <summary>
        /// 快移速度
        /// </summary>
        [Description("快移速度")]
        public string Fastmovespeed { get; set; }
        /// <summary>
        /// 最大控制轴数
        /// </summary>
        [Description("最大控制轴数")]
        public int MaxAxisNo { get; set; }
        /// <summary>
        /// CNC模式
        /// </summary>
        [Description("CNC模式")]
        public string CNCMode { get; set; }
        /// <summary>
        /// 主程序号
        /// </summary>
        [Description("主程序号")]
        public string MainProgram { get; set; }
        /// <summary>
        /// 当前程序号
        /// </summary>
        [Description("当前程序号")]
        public string NowProgram { get; set; }
        /// <summary>
        /// 程序段号
        /// </summary>
        [Description("程序段号")]
        public string ProgramStep { get; set; }
        /// <summary>
        /// 单段备注
        /// </summary>
        [Description("单段备注")]
        public string Singlenote { get; set; }
        /// <summary>
        /// 程序备注
        /// </summary>
        [Description("程序备注")]
        public string Programnote { get; set; }
        /// <summary>
        /// 主轴指定速度
        /// </summary>
        [Description("主轴指定速度")]
        public string S_SetSpeed { get; set; }
        /// <summary>
        /// 主轴实际速度
        /// </summary>
        [Description("主轴实际速度")]
        public string S_Speed { get; set; }
        /// <summary>
        /// 系统IP地址
        /// </summary>
        [Description("系统IP地址")]
        public string IPaddress { get; set; }
        /// <summary>
        /// CNC型号
        /// </summary>
        [Description("CNC型号")]
        public string CNCModel { get; set; }
        /// <summary>
        /// CNC类型
        /// </summary>
        [Description("CNC类型")]
        public string CNCType { get; set; }
        /// <summary>
        /// CNC序列号
        /// </summary>
        [Description("CNC序列号")]
        public string CNCSequence { get; set; }
        /// <summary>
        /// CNC版本号
        /// </summary>
        [Description("CNC版本号")]
        public string CNCVersion { get; set; }
        /// <summary>
        /// Customer ID/ Name
        /// </summary>
        [Description("Customer ID/ Name")]
        public string Customer { get; set; }

        /// <summary>
        /// Site
        /// </summary>
        [Description("Site")]
        public string Site { get; set; }
        /// <summary>
        /// Building
        /// </summary>
        [Description("Building")]
        public string Building { get; set; }
        /// <summary>
        /// Product ID
        /// </summary>
        [Description("Product ID")]
        public string ProductID { get; set; }
        /// <summary>
        /// Process name
        /// </summary>
        [Description("Process name")]
        public string ProcessName { get; set; }
        /// <summary>
        /// Operator ID
        /// </summary>
        [Description("Operator ID")]
        public string OperatorID { get; set; }
        /// <summary>
        /// 伺服温度
        /// </summary>
        [Description("伺服温度")]
        public string ServoTemp { get; set; }
        /// <summary>
        /// 主軸溫度
        /// </summary>
        [Description("主軸溫度")]
        public string S_Temp { get; set; }
        /// <summary>
        /// 主軸扭力
        /// </summary>
        [Description("主軸扭力")]
        public string S_Torque { get; set; }
        /// <summary>
        /// 停機時主軸原始座標X/Y/Z/A
        /// </summary>
        [Description("停機時主軸原始座標X/Y/Z/A")]
        public string XYZA_load { get; set; }
        /// <summary>
        /// 切削液狀態
        /// </summary>
        [Description("切削液狀態")]
        public string Cuttingfluid { get; set; }
        /// <summary>
        /// 切削指定速度 
        /// </summary>
        [Description("切削指定速度")]
        public string CuttingSpeed { get; set; }
        /// <summary>
        /// 伺服实际速度 
        /// </summary>
        [Description("伺服实际速度")]
        public string ActualSpeed { get; set; }
        /// <summary>
        /// 当前控制轴数 
        /// </summary>
        [Description("当前控制轴数")]
        public int UseAxisNo { get; set; }
        /// <summary>
        /// 报警类型 
        /// </summary>
        [Description("报警类型")]
        public string ErrorType { get; set; }

        /// <summary>
        /// 對刀儀
        /// </summary>
        [Description("對刀儀")]
        public string Toolset { get; set; }
        /// <summary>
        /// 主軸熱伸長
        /// </summary>
        [Description("主軸熱伸長")]
        public string S_TE { get; set; }
        /// <summary>
        ///  每次加工起始時間
        /// </summary>
        [Description("每次加工起始時間")]
        public string ProcessStartTm { get; set; }
        /// <summary>
        /// 伺服负载
        /// </summary>
        [Description("伺服负载")]
        public string SF_Load { get; set; }
        /// <summary>
        ///  關機時間 (啟/停)
        /// </summary>
        //[Description("關機時間 (啟/停)")]
        //public string ShutdownTm { get; set; }

        public DateTime? CreateTime { get; set; }

        public string ScanType { get; set; }
  
    }
}
