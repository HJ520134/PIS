using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class OEE_DownTimeCodeDTO : BaseModel
    {

        public int OEE_DownTimeCode_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int OEE_DownTimeType_UID { get; set; }
        public int Project_UID { get; set; }
        public int? LineID { get; set; }
        public int? StationID { get; set; }
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
        /// <summary>
        /// 异常说明
        /// </summary>
        public string Error_Reasons { get; set; }
        public string Remarks { get; set; }
        public bool Is_Enable { get; set; }
        public bool? IsEnabled { get; set; }
        public int Modify_UID { get; set; }
        public System.DateTime Modify_Date { get; set; }

        //自定义
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string EnumDownTimeCodeType { get; set; }
        public string ProjectName { get; set; }
        public string LineName { get; set; }
        public string StationName { get; set; }
        public string Modifyer { get; set; }
        //Equip - Pneumatic/Hydraulics 设备-电气 Equip - Roftware 设备-软件 Process-adjustment 工艺-调试 Process-Consumable 工艺-更换耗材 Maintenace 保养 Other 其他
    }

    public class OEE_AbnormalDFCode : BaseModel
    {
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string ProjectName { get; set; }
        public string LineName { get; set; }
        public string StationName { get; set; }
        public string MachineName { get; set; }
        public string ShiftName { get; set; }
        public string DownTimeCode { get; set; }
        public string DFCode { get; set; }
        public int ShiftTimeID { get; set; }
        public int Machine_UID { get; set; }
        public DateTime ProductDate { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class AbnormalDFCode : BaseModel
    {
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string ProjectName { get; set; }
        public string LineName { get; set; }
        public string StationName { get; set; }
        public string MachineName { get; set; }
        public string ShiftName { get; set; }
        public string DFCode { get; set; }
        public Nullable<int> ShiftTimeID { get; set; }
        public Nullable<int> Machine_UID { get; set; }
        public DateTime ProductDate { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
