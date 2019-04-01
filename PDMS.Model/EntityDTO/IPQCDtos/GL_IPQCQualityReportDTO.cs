using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_IPQCQualityReportDTO : BaseModel
    {
        public int IPQCQualityReport_UID { get; set; }
        public int StationID { get; set; }
        public int ShiftID { get; set; }
        public string TimeInterval { get; set; }
        public int TimeIntervalIndex { get; set; }
        public DateTime ProductDate { get; set; }
        /// <summary>
        /// 一次良率 一次OK数/一次检验数
        /// </summary>
        public double FirstYield { get; set; }
        /// <summary> 
        /// 一次目标良率
        /// </summary>
        public double FirstTargetYield { get; set; }
        /// <summary>
        /// 二次良率 二次OK数/（二次OK数+NG数）
        /// </summary>
        public double SecondYield { get; set; }
        /// <summary>
        /// 二次目标良率
        /// </summary>
        public double SecondTargetYield { get; set; }
        /// <summary>
        /// PIS 进料数
        /// </summary>
        public int InputNumber { get; set; }
        /// <summary>
        /// 检验数 Scan IN IPQC扫码IN
        /// </summary>
        public int TestNumber { get; set; }
        /// <summary>
        /// 一次OK数   (MES-IPQC扫码IN)-(MES-IPQC返修)-(MES-IPQC-NG)
        /// </summary>
        public int FirstPassNumber { get; set; }
        /// <summary>
        /// 二次OK数 MES-IPQC OUT
        /// </summary>
        public int SecondPassNumber { get; set; }
        /// <summary>
        /// 返修数 SCAN MES-IPQC返修
        /// </summary>
        public int RepairNumber { get; set; }
        /// <summary>
        /// NG数 MES-IPQC-NG
        /// </summary>
        public int NGNumber { get; set; }
        /// <summary>
        /// WIP 上时段WIP+进料数-NG-良品出货数
        /// </summary>
        public int WIP { get; set; }

        public System.DateTime ModifyTime { get; set; }
        public System.DateTime NowDateTime { get; set; }
        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime SerchStartTime { get; set; }
        /// <summary>
        /// 班次结束时间
        /// </summary>
        public DateTime SerchEndTime { get; set; }
    }
}
