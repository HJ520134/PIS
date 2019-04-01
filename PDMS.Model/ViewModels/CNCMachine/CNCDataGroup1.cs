using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class CNCDataGroup1
    {
        //機台開機
        public string PowerOn { get; set; }
        //關機時間 (啟/停)
        public string ShutdownTm { get; set; }
        //CNC型号
        public string CNCModel { get; set; }
        //CNC类型
        public string CNCType { get; set; }
        //CNC序列号
        public string CNCSequence { get; set; }
        //CNC版本号 
        public string CNCVersion { get; set; }
        //轴名称 
        public string ServoName { get; set; }
        //开机时间(Min)
        public int PowerOnTm { get; set; }
        //切削时间
        public int CuttingTm { get; set; }
        //循环时间 
        public int CycleRunTm { get; set; }
        //当前刀具号 
        public string CurrenttoolNo { get; set; }
        //最大控制轴数
        public int MaxAxisNo { get; set; }
        //当前控制轴数 
        public int UseAxisNo { get; set; }
        //CNC模式
        public string CNCMode { get; set; }
        //主程序号
        public string MainProgram { get; set; }
        //当前程序号
        public string NowProgram { get; set; }
        //程序段号 
        public string ProgramStep { get; set; }
        //单段备注   
        public string Singlenote { get; set; }
        //程序备注 
        public string Programnote { get; set; }
        //系统IP地址 
        public string IPaddress { get; set; }
        //主轴指定速度
        public string S_SetSpeed { get; set; }
        //实际速度
        public string S_Speed { get; set; }
    }

}
