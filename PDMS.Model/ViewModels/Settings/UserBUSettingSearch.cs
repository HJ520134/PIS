using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.ViewModels.Settings
{
    public class UserBUSettingSearch : BaseModel
    {
        #region 往DB发送查询请求
        public string User_NTID { get; set; }

        public string User_Name { get; set; }

        public string BU_ID { get; set; }

        public string BU_Name { get; set; }

        public string BU_D_ID { get; set; }

        public string BU_D_Name { get; set; }

        public DateTime? Reference_Date { get; set; }

        public string queryTypes { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }

        public int OrgID { get; set; }

        public int CurrentUser { get; set; }
        #endregion
    }

    #region 从DB中接收数据
    public class CustomBUDDTO
    {
        public int? BU_D_UID { get; set; }

        public string BU_D_ID { get; set; }

        public string BU_D_Name { get; set; }

        public int Organization_UID { get; set; }
    }

    public class UserBUSettingGet
    {
        public SystemUserBusinessGroupDTO SystemUserBusinessGroupDTO { get; set; }

        public SystemBUMDTO SystemBUMDTO { get; set; }

        public CustomBUDDTO SystemBUDDTO { get; set; }

        public SystemUserDTO SystemUserDTO { get; set; }

        public SystemUserDTO SystemUserDTO1 { get; set; }
    }
    #endregion

    #region 编辑Detail页面根据UserNTID获取所有的BU主档和BU明细档
    public class BUAndBUDByUserNTID
    {
        public int System_User_BU_UID { get; set; }

        public string User_NTID { get; set; }

        public string User_Name { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Begin_Date { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? End_Date { get; set; }

        public int BU_M_UID { get; set; }

        public int? BU_D_UID { get; set; }

        public string BU_ID { get; set; }

        public string BU_Name { get; set; }

        public string BU_D_ID { get; set; }

        public string BU_D_Name { get; set; }

        public List<CustomBUD> BUDList { get; set; }
    }

    public class CustomBUD
    {
        public int BU_D_UID { get; set; }

        public string BU_D_ID { get; set; }

        public string BU_D_Name { get; set; }
    }

    #endregion


    #region 子表中根据输入的BU_ID获取BU Detail信息绑定到子表下拉框中
    public class CustomBUItemAndBUD 
    {
        public int BU_M_UID { get; set; }

        public string BU_ID { get; set; }

        public string BU_Name { get; set; }

        public List<SystemBUDDTO> SystemBUDDTOList { get; set; }
    }
    #endregion

    #region 新增或保存模型传入到DB中做Save操作
    public class UserBUAddOrSave : EntityDTOBase
    {
        public string User_NTID { get; set; }

        public string User_Name { get; set; }

        public List<CustomBUAndBUD> FunctionSubs { get; set; }
    }

    public class CustomBUAndBUD
    {
        public int System_User_BU_UID { get; set; }

        public int BU_M_UID { get; set; }

        public int? BU_D_UID { get; set; }

        public string BU_ID { get; set; }

        public string BU_D_ID { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Begin_Date { get; set; }

        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? End_Date { get; set; }

        public DateTime BU_BeginDate { get; set; }

        public DateTime? BU_EndDate { get; set; }

        public DateTime? BUD_BeginDate { get; set; }

        public DateTime? BUD_EndDate { get; set; }

        public int RowNum { get; set; }
    }
    #endregion
}
