using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_MachineInfoDTO : BaseModel
    {
        public int OEE_MachineInfo_UID { get; set; }
        /// <summary>
        /// 厂区
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// 厂区名字
        /// </summary>
        public string Plant_Organization_Name { get; set; }
        /// <summary>
        /// OP—ID
        /// </summary>
        public int BG_Organization_UID { get; set; }

        /// <summary>
        /// OP-Name
        /// </summary>
        public string BG_Organization_Name { get; set; }
        /// <summary>
        /// 功能厂ID
        /// </summary>
        public int? FunPlant_Organization_UID { get; set; }

        /// <summary>
        /// 功能厂名称
        /// </summary>
        public string FunPlant_Organization_Name { get; set; }
        /// <summary>
        /// 专案ID
        /// </summary>
        public int Project_UID { get; set; }
        /// <summary>
        /// 专案名称
        /// </summary>
        public string Project_Name { get; set; }
        public int LineID { get; set; }
        public string Line_Name { get; set; }
        public int StationID { get; set; }
        public string Station_Name { get; set; }
        public int EQP_Uid { get; set; }
        public string EQP_EMTSerialNum { get; set; }
        public string MachineNo { get; set; }
        public bool Is_Enable { get; set; }
        public int Modify_UID { get; set; }
        public string Modify_Name { get; set; }
        public DateTime Modify_Date { get; set; }
       
        /// <summary>
        /// 前端是否启用
        /// </summary>
        public string query_Is_Enable { get; set; }

        public bool  station_Is_Enable { get; set; }
        public bool Line_Is_Enable { get; set; }
        public bool Poject_Is_Enable { get; set; }
        public string TimeInterval { get; set; }
        public string IsSubmit { get; set; }
    }

    public class UserModel
    {
        public int Modify_UID { get; set; }
        public string Modify_Name { get; set; }
        public DateTime Modify_Date { get; set; }
        public int num { get; set; }
    }

    public class TimeModel:BaseModel
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string currentDate { get; set; }
        /// <summary>
        /// 当前的时段
        /// </summary>
        public string currentTimeInterval { get; set; }
        /// <summary>
        /// 当前时间属于哪个班次
        /// </summary>
        public int currentShiftID { get; set; }
    }

    public class ShiftBaseInfo : BaseModel
    {
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
    }

    public class TimeIntervalParamModel : BaseModel
    {
        /// <summary>
        /// 班次
        /// </summary>
        public int CurrentShiftID { get; set; }

        /// <summary>
        /// 厂区
        /// </summary>
        public int Plant_Organization_UID { get; set; }

        /// <summary>
        /// OP
        /// </summary>
        public int BG_Organization_UID { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// 时段区间长度
        /// </summary>
        public int TimeIntervalLength { get; set; }
    }
}
