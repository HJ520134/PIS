using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class CNCDataGroup3
    {

        //每次加工起始時間
        public string ProcessStartTm { get; set; }
        //工件加工C/T
        public string ProcessCycleTm { get; set; }
        //X軸的負載
        public string X_load { get; set; }
        //Y軸的負載 
        public string Y_load { get; set; }
        //Z軸的負載 
        public string Z_load { get; set; }
        //A軸的負載
        public string A_load { get; set; }
        //主軸轉速（S）
        public int S_RPM { get; set; }
        //進給(F)  
        public string Feed { get; set; }
        //四軸角度 
        public string Axisangle { get; set; }
        //對刀儀
        public string Toolset { get; set; }

    }
}
