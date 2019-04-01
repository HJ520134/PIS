using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QAInputDetailVM : BaseModel
    {
        public Nullable<int> QualityAssurance_InputMaster_UID { get; set; }
        public int QualityAssurance_InputDetail_UID { get; set; }
        public int ExceptionType_UID { get; set; }
        public Nullable<int> Repair_Qty { get; set; }
        public Nullable<int> SepcialAccept_Qty { get; set; }
        public Nullable<int> RepairNG_Qty { get; set; }
        public Nullable<int> NG_Qty { get; set; }

        public string ExceptionTypeName { set; get; }
        public bool IsDeleted { get; set; }
        public string ModifyReason { set; get; }
        public int Modified_UID { set; get; }
        public int Creator_UID { get; set; }
        public DateTime CreateDate { set; get; }
        public DateTime Modified_Date { get; set; }

        public string Project { set; get; }
        public string Process { set; get; }

        /// <summary>
        /// 置换
        /// </summary>
        public Nullable<int> Displace_Qty { set; get; }

        public string BadTypeCode { set; get; }

        public string BadTypeEnglishCode { set; get; }
       
        //反推报表
        public Nullable<int> System_FunPlant_UID { set; get; }
        public List<FunPlantVM> FunPlants = new List<FunPlantVM>();
    }




    public class QADetailSearch : BaseModel
    {
        public int QAMaster_UID { get; set; }
        public string TypeCode { set; get; }
        public string TypeName { get; set; }
        public string ShortName { get; set; }
        public bool IsContainsChild { set; get; }
        public int User_UID { set; get; }
        public int FlowChart_Detail_UID { set; get; }
        public string Project { get; set; }

        public int Process_seq { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public string ProductDate { set; get; }
        public string Time_interval { set; get; }
        public string MaterialType { set; get; }
        public string Color { set; get; }

        //不良明细获取MesAPI的配置的路径
        public string MesAPIPath { set; get; }
    }

    public class QAInputDetailListVM : BaseModel
    {
        public List<QAInputDetailVM> DataList = new List<QAInputDetailVM>();
    }


}
