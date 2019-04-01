using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class ExceptionDTO : EntityDTOBase
    {

        /// <summary>
        /// 异常部门UID
        /// </summary>
        public int Exception_Dept_UID { get; set; }
        /// <summary>
        /// 厂区
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// BG
        /// </summary>
        public int BG_Organization_UID { get; set; }
        /// <summary>
        /// 功能厂
        /// </summary>
        public int Funplant_Organization_UID { get; set; }

        /// <summary>
        /// 异常部门名称
        /// </summary>
        public string Exception_Dept_Name { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string Created_User { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }

        /// <summary>
        /// 异常编码UID
        /// </summary>
        public int? Exception_Code_UID { get; set; }
        /// <summary>
        /// 异常编码
        /// </summary>
        public string Exception_Nub { get; set; }
        /// <summary>
        /// 异常名称
        /// </summary>
        public string Exception_Name { get; set; }

        /// <summary>
        /// 厂区
        /// </summary>
        public string Plant { get; set; }
        /// <summary>
        /// BG
        /// </summary>
        public string BG { get; set; }
        /// <summary>
        /// 功能厂
        /// </summary>
        public string FunPlant { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public int DelayDayNub { get; set; }
        /// <summary>
        /// 发送周期
        /// </summary>
        public int DayPeriod { get; set; }
        /// <summary>
        /// 最大发送次数
        /// </summary>
        public int SendMaxTime { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date_from { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date_to { get; set; }

        //---异常添加使用----
        /// <summary>
        /// 线体ID
        /// </summary>
        public int LineID { get; set; }
        /// <summary>
        /// 线体名
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 工站UID
        /// </summary>
        public int StationID { get; set; }
        /// <summary>
        /// 工站名称
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 异常描述
        /// </summary>
        public string Note { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime WorkDate { get; set; }
        /// <summary>
        /// 班次UID
        /// </summary>
        public int ShiftTimeID { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        public string Shift { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime? End_Date { get; set; }



        //----异常时间段
        public int Exception_Time_UID { get; set; }
        public int Origin_UID { get; set; }
        public string Exception_Time_At { get; set; }

        //------邮件接收方式1，2
        public int Exception_Email_UID { get; set; }
        public int ReceiveType { get; set; }
        //收件人NT
        public string User_NT { get; set; }
        //收件人名字
        public string User_Name { get; set; }
        //收件人邮箱
        public string User_Email { get; set; }

        public int Exception_Times { get; set; }

        //---异常单记录----
        public int Exception_Record_UID { get; set; }
        /// <summary>
        /// 异常单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 判断添加的模型是否重复
        /// </summary>
        public int Repeat { get; set; }
        public string Exception_Code_UIDs { get; set; }
        //专案的UID
        public int CustomerID { get; set; }
        //专案名称
        public string Project_Name { get; set; }

        public int Project_UID { get; set; }

        public int Seq { get; set; }

        public string Contact_Person { get; set; }
        public string Contact_Phone { get; set; }
        public int Elapsed { get;set;}
    }

    public class Depts
    {
        public int Exception_Dept_UID { get; set; }
        public string Exception_Dept_Name { get; set; }
    }
    public class Projects
    {
        public int Project_UID { get; set; }
        public string Project_Name { get; set; }
    }
    public class Stations
    {
        public int StationID { get; set; }
        public string StationName { get; set; }
    }

    public class ShiftTime
    {
        public int ShiftTimeID { get; set; }
        public string StartTime { get; set; }
        public string End_Time { get; set; }
        public string Shift { get; set; }
    }

    public class PeriodTime
    {
        public int Exception_Time_UID { get; set; }
        public string Exception_Time_At { get; set; }
        public int Repeat { get; set; }
    }

    public class Line
    {
        /// <summary>
        /// 线体的ID
        /// </summary>
        public int LineID { get; set; }
        /// <summary>
        /// 线体的名称
        /// </summary>
        public string LineName { get; set; }
    }

    public class ReplyRecordDTO : EntityDTOBase
    {
        public int Exception_Reply_UID { get; set; }
        public int Exception_Record_UID { get; set; }
        public string Exception_Content { get; set; }
        public int Reply_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime? Reply_Date { get; set; }
    }

    public class ExceptionRecordEntity : EntityDTOBase
    {
        public int Exception_Shedule_UID { get; set; }

        public DateTime SendDate { get; set; }
        public int SendTime { get; set; }

        public int Exception_Record_UID { get; set; }

        public string LineName { get; set; }

        public string StationName { get; set; }

        public DateTime Created_Date { get; set; }

        public string Exception_Dept_Name { get; set; }
        public string Exception_Nub { get; set; }
        public string Exception_Name { get; set; }

        public string Note { get; set; }
        public DateTime WorkDate { get; set; }
        public string Shift { get; set; }
        public string User_Name { get; set; }
        //专案名称
        public string Project_Name { get; set; }
        public int Seq { get; set; }
        public int Status { get; set; }
        public string FlagName { get; set; }//第一个是发送次数，第二个是部门UID
        public string Contact_Person { get; set; }
        public string Contact_Phone { get; set; }
    }

    public class EmailSendDTO : EntityDTOBase
    {
        public string Exception_Record_UIDs { get; set; }
        public string SendIDs { get; set; }
        public string CCIDs { get; set; }
        public string Subject { get; set; }
        public string ContentString { get; set; }

    }

    public class ExceptionProjectDTO : EntityDTOBase
    {
        public int PrException_Project_UID { get; set; }
        public int Project_UID { get; set; }
        public int DelayDayNub { get; set; }
        public int DayPeriod { get; set; }
        public int SendMaxTime { get; set; }
        public int Created_UID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }
    }

    public class ExceptionAddDTO : EntityDTOBase
    {
        /// <summary>
        /// 异常部门UID
        /// </summary>
        public int Exception_Dept_UID { get; set; }
        /// <summary>
        /// 厂区
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// BG
        /// </summary>
        public int BG_Organization_UID { get; set; }
        /// <summary>
        /// 功能厂
        /// </summary>
        public int Funplant_Organization_UID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string Created_User { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }

        /// <summary>
        /// 厂区
        /// </summary>
        public string Plant { get; set; }
        /// <summary>
        /// BG
        /// </summary>
        public string BG { get; set; }
        /// <summary>
        /// 功能厂
        /// </summary>
        public string FunPlant { get; set; }
   

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date_from { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date_to { get; set; }

        //---异常添加使用----
        /// <summary>
        /// 线体ID
        /// </summary>
        public int LineID { get; set; }
        /// <summary>
        /// 线体名
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 工站UID
        /// </summary>
        public int StationID { get; set; }
        /// <summary>
        /// 工站名称
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 异常描述
        /// </summary>
        public string Note { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime WorkDate { get; set; }
        /// <summary>
        /// 班次UID
        /// </summary>
        public int ShiftTimeID { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        public string Shift { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime? End_Date { get; set; }

        //---异常单记录----
        public int Exception_Record_UID { get; set; }
        public string Exception_Code_UIDs { get; set; }

        public int Project_UID { get; set; }

        public string Contact_Person { get; set; }
        public string Contact_Phone { get; set; }

        public string SendIDs { get; set; }
        public string CCIDs { get; set; }
    }

    public class DashboardRecordsDTO:BaseModel
    {
        public int Exception_Record_UID { get; set; }
        public string Plant { get; set; }
        public string BG { get; set; }
        public int LineID { get; set; }
        public string LineName { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public int Exception_Code_UID { get; set; }
        public string Exception_Nub { get; set; }
        public string Exception_Name { get; set; }
        public string Exception_Dept_Name { get; set; }
        public int Seq { get; set; }
        public string Shift { get; set; }
        public int Project_UID { get; set; }
        public string Project_Name { get; set; }
        public string WorkDate { get; set; }
        public int ShiftTimeID { get; set; }
        public int Status { get; set; }
        public string End_Date { get; set; }
        public string Note { get; set; }
        public string Contact_Persion { get; set; }
        public string Contact_Phone { get; set; }
        public string Created_User { get; set; }
        public string Created_Date { get; set; }
        public string Modified_UserName { get; set; }
        public string Elapsed { get; set; }
    }
    public class DashboardSearchDTO:BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int Project_UID { get; set; }
        public int LineID { get; set; }
        public string LineName { get; set; }
        public string WorkDate { get; set; }
        public int ShiftTimeID { get; set; }
        public string StartH { get; set; }
        public string StartM { get; set; }
        public int DelayDayNub { get; set; }
    }
    public class ChartsModel
    {
        public List<string> xLabel { get; set; }

        public List<decimal> yLabel { get; set; }
    }

}
