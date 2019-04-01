using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class GL_ReprortSearchModel : EntityDTOBase
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ShiftTimeID { get; set; }
        public string Param_LineName { get; set; }
        public string Param_StationName { get; set; }
        public string DownTimeType { get; set; }
    }

    public class GL_FourQDownTimeRecordDTO : EntityDTOBase
    {
        public int Exception_Record_UID { get; set; }
        public int? Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int? StationID { get; set; }
        public string StationName { get; set; }
        public int Exception_Code_UID { get; set; }
        public System.DateTime? WorkDate { get; set; }
        public int? ShiftTimeID { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime? EndTIme { get; set; }
        
    }

    public class GLFourQParamModel : EntityDTOBase
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public int CustomerID { get; set; }
        public int LineID { get; set; }
        public int StationID { get; set; }
        public int ShiftTimeID { get; set; }
        public DateTime EndTime { get; set; }
        public int DownTime { get; set; }
        public bool Is_Enable { get; set; }
        public string ReportType { get; set; }
        public DateTime ActionCreateData { get; set; }
        public string Param_Name { get; set; }
    }

    public class GLFourQDTModel : EntityDTOBase
    {
        /// <summary>
        /// downtime總時間
        /// </summary>
        public int TotalDTTime { get; set; }
        /// <summary>
        /// downtime時間
        /// </summary>
        public int DTTime { get; set; }
        /// <summary>
        /// downtime占比值
        /// </summary>
        public double DTTime_p { get; set; }
        /// <summary>
        /// DT 类型
        /// </summary>
        public string DTName { get; set; }

        /// <summary>
        ///  月份
        /// </summary>
        public string DtMon { get; set; }

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


    public class GLPaynterDetialModel : EntityDTOBase
    {
        public string name { get; set; }
        public string stack { get; set; }
        public string type { get; set; }
        public List<int> data { get; set; }
    }


    public class GLPaynTerChartModel : EntityDTOBase
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
