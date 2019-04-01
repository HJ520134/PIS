using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels {
    public class CNCDataGroup2
    {
        //主軸溫度
        public string S_Temp { get; set; }
        //主軸扭力
        public string S_Torque { get; set; }
        //主軸熱伸長
        public string S_TE { get; set; }
        //宏變量 
        public string Mcode { get; set; }
        //切削液狀態 
        public string Cuttingfluid { get; set; }
        //主轴誤差
        public string S_Gap { get; set; }
        //伺服温度
        public string ServoTemp { get; set; }
        //主轴负载  
        public string S_Load { get; set; }
        //切削指定速度 
        public string CuttingSpeed { get; set; }
        //实际速度 
        public string ActualSpeed { get; set; }
        //倍率（快移） 
        public string RateJOG { get; set; }
        //倍率（切削） 
        public string RateFastmove { get; set; }
        //倍率（HAND） 
        public string RateHAND { get; set; }
        //倍率（主轴） 
        public string RateS { get; set; }
        //快速进给时间常数 
        public string FastmoveTmC { get; set; }
        //快移速度 
        public string Fastmovespeed { get; set; }

    }
}
