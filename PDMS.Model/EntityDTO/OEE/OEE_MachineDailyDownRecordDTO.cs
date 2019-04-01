using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_MachineDailyDownRecordDTO : EntityDTOBase
    {
        public int OEE_MachineDailyDownRecord_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int StationID { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public int OEE_DownTimeCode_UID { get; set; }
        public System.DateTime DownDate { get; set; }
        public int ShiftTimeID { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTIme { get; set; }
        public int DownTime { get; set; }
        public int OEE_DownTimeType_UID { get; set; }
        public bool Is_Enable { get; set; }
        public string Type_Name { get; set; }

        /// <summary>
        /// 异常代码
        /// </summary>
        public string Error_Code { get; set; }

        /// <summary>
        /// 上传方式
        /// </summary>
        public string Upload_Ways { get; set; }

        /// <summary>
        /// 异常名称 OR 细项分类
        /// </summary>
        public string Level_Details { get; set; }

    }

    public class OEEFourQParamModel : EntityDTOBase
    {
        public int OEE_MachineDailyDownRecord_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int StationID { get; set; }
        public int OEE_MachineInfo_UID { get; set; }
        public int OEE_DownTimeCode_UID { get; set; }
        public DateTime DownDate { get; set; }
        public int ShiftTimeID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DownTime { get; set; }
        public int OEE_DownTimeType_UID { get; set; }
        public bool Is_Enable { get; set; }
        public string Type_Name { get; set; }
        public string ReportType { get; set; }
        public string Param_Name { get; set; }

        public DateTime ActionCreateData { get; set; }
    }

    public class OEEFourQDTModel : EntityDTOBase
    {
        /// <summary>
        /// 宕机时长
        /// </summary>
        public int TotalDTTime { get; set; }

        /// <summary>
        /// DT 类型
        /// </summary>
        public string DTName { get; set; }

        /// <summary>
        ///  目标
        /// </summary>
        public double TotalGoalNum { get; set; }

        /// <summary>
        ///  月
        /// </summary>
        public string ReportMonth { get; set; }

        /// <summary>
        ///  周
        /// </summary>
        public string ReportWeek { get; set; }

        /// <summary>
        ///  天
        /// </summary>
        public string ReportDaily { get; set; }

        /// <summary>
        /// 不良类型名称
        /// </summary>
        public string DTTypeName { get; set; }
    }


    public class PaynterDetialModel : EntityDTOBase
    {
        public string name { get; set; }
        public string stack { get; set; }
        public string type { get; set; }
        public List<int> data { get; set; }
    }


    public class PaynTerChartModel : EntityDTOBase
    {
        /// <summary>
        /// 横坐标的时间周期名称
        /// </summary>
        public string DateCycleName { get; set; }

        public PaynterDetialModel PaynterDetial { get; set; }

        /// <summary>
        /// 宕机明细名称集合
        /// </summary>
        public List<string> DTTypeNameList { get; set; }

        /// <summary>
        /// 宕机的明细信息集合
        /// </summary>
        public List<OEEFourQDTModel> DTDetailsList { get; set; }
    }
}
