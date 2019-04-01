using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Fixture_Return_MDTO : EntityDTOBase
    {

        #region 原始字段
        /// <summary>
        /// 领用归还单流水号
        /// </summary>
        public int Fixture_Return_M_UID { get; set; }
        /// <summary>
        /// 领用归还单号R20181121_001
        /// </summary>
        public string Return_NO { get; set; }
        /// <summary>
        /// 领用单号流水号
        /// </summary>
        public int Fixture_Totake_M_UID { get; set; }
        /// <summary>
        /// 领用单编号
        /// </summary>
        public string Taken_NO { get; set; }
        /// <summary>
        /// 厂区
        /// </summary>
        public int Plant_Organization_UID { get; set; }
        /// <summary>
        /// 厂区名
        /// </summary>
        public string Plant { get; set; }
        /// <summary>
        /// OP类型
        /// </summary>
        public int BG_Organization_UID { get; set; }
        /// <summary>
        /// BG名
        /// </summary>
        public string BG { get; set; }
        /// <summary>
        /// 功能厂
        /// </summary>
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        /// <summary>
        /// 功能厂名
        /// </summary>
        public string FunPlant { get; set; }
        /// <summary>
        /// 领用归还人员
        /// </summary>
        public string Return_User { get; set; }
        /// <summary>
        /// 领用归还人员姓名
        /// </summary>
        public string Return_Name { get; set; }
        /// <summary>
        /// 归还日期
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Return_Date { get; set; }
        /// <summary>
        /// 创建人员
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Create_Date { get; set; }
        /// <summary>
        /// 制程唯一标识
        /// </summary>
        public int Process_Info_UID { get; set; }
        /// <summary>
        /// 工站唯一编号
        /// </summary>
        public int WorkStation_UID { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        public string WorkStation_Name { get; set; }
        /// <summary>
        /// 产线唯一编号
        /// </summary>
        public int Production_Line_UID { get; set; }
        /// <summary>
        /// 治具流水编号
        /// </summary>
        public int Fixture_M_UID { get; set; }
        /// <summary>
        /// 治具唯一编号
        /// </summary>
        public string Fixture_Unique_ID { get; set; }
        /// <summary>
        /// 治具编号
        /// </summary>
        public string Fixture_NO { get; set; }
        /// <summary>
        /// 治具设备编号
        /// </summary>
        public int Fixture_Machine_UID { get; set; }
        /// <summary>
        /// 生产地点
        /// </summary>
        public int Workshop_UID { get; set; }
        /// <summary>
        /// 车间名称
        /// </summary>
        public string Workshop_Name { get; set; }
        /// <summary>
        /// 治具短码
        /// </summary>
        public string ShortCode { get; set; }
        /// <summary>
        /// 修改开始时间
        /// </summary>
        public DateTime Modified_Date_from { get; set; }
        /// <summary>
        /// 修改结束时间
        /// </summary>
        public DateTime Modified_Date_to { get; set; }
        /// <summary>
        /// 是否归还
        /// </summary>
        public int? IS_Return { get; set; }
        /// <summary>
        /// 制程
        /// </summary>
        public string Process_Name { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string Machine_ID { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string User_Name { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Created_Date { get; set; }

        /// <summary>
        /// 修改人的名字
        /// </summary>
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 产线
        /// </summary>
        public string Line_NO { get; set; }
        /// <summary>
        /// 领用详细
        /// </summary>
        public int Fixture_Totake_D_UID { get; set; }
        #endregion

        #region 带出领用单集合
        public List<string> fixture_Taken_Info { get; set; }
        ///// <summary>
        ///// 所有治具
        ///// </summary>
        //public List<FixtureDTO> fixtures { get; set; }
        #endregion

    }

    public class Fixture_Taken_InfoDTO
    {
        /// <summary>
        /// 治具归D还唯一编号
        /// </summary>
        public int Fixture_Return_D_UID { get; set; }
        /// <summary>
        /// 治具领用单号
        /// </summary>
        public int Fixture_Totake_M_UID { get; set; }
        /// <summary>
        /// 治具流水号
        /// </summary>
        public int Fixture_M_UID { get; set; }
        /// <summary>
        /// 治具短码
        /// </summary>
        public string ShortCode { get; set; }
        /// <summary>
        /// 治具唯一编号
        /// </summary>
        public string Fixture_Unique_ID { get; set; }
        /// <summary>
        /// 制程
        /// </summary>
        public string Process { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        public string WorkStation { get; set; }
        /// <summary>
        /// 线别
        /// </summary>
        public string Line { get; set; }
        /// <summary>
        /// 是否已归还
        /// </summary>
        public int IS_Return { get; set; }
        /// <summary>
        /// 领用明细档UID
        /// </summary>
        public int Fixture_Totake_D_UID { get; set; }
    }

    /// <summary>
    /// Ajax提交的数据
    /// </summary>
    public class Fixture_Return_Post
    {
        /// <summary>
        /// 归还主档UID
        /// </summary>
        public int Fixture_Return_M_UID { get; set; }
        /// <summary>
        /// 归还明细当UID
        /// </summary>
        public int Fixture_Return_D_UID { get; set; }
        /// <summary>
        /// 领取唯一单号
        /// </summary>
        public int Fixture_Totake_M_UID { get; set; }
        /// <summary>
        /// 领用明细档UID
        /// </summary>
        public int Fixture_Totake_D_UID { get; set; }
        /// <summary>
        /// 治具流水号
        /// </summary>
        public int Fixture_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        /// <summary>
        /// 归还人员NT
        /// </summary>
        public string Return_User { get; set; }
        /// <summary>
        /// 领用归还人员姓名
        /// </summary>
        public string Return_Name { get; set; }
        /// <summary>
        /// 归还日期
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime Return_Date { get; set; }

    }

    public class Fixture_Return_Index: EntityDTOBase
    {

        public int Fixture_Return_M_UID { get; set; }
        public int Fixture_Totake_M_UID { get; set; }
        public string Return_NO { get; set; }

        /// <summary>
        /// 领用单编号
        /// </summary>
        public string Taken_NO { get; set; }
        /// <summary>
        /// 厂区
        /// </summary>
        public string Plant { get; set; }
        /// <summary>
        /// BG
        /// </summary>
        public string BG { get; set; }

        /// <summary>
        /// FunPlant
        /// </summary>
        public string FunPlant { get; set; }

        /// <summary>
        /// Return_User
        /// </summary>
        public string Return_User { get; set; }
        /// <summary>
        /// Return_User
        /// </summary>
        public string Return_Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Totake_Date { get; set; }


        public int Modified_UID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Modified_Date { get; set; }

        public string Machine_ID { get; set; }
    }

}
