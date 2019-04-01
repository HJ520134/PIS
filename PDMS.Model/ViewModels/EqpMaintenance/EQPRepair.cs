using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class EQPRepair : EntityDTOBase
    {
        public string Error_Types { get; set; }
        public int Repair_Uid { get; set; }
        public int EQP_Uid { get; set; }
        public string Equipment { get; set; }
        public string Repair_id { get; set; }
        public string Status { get; set; }
        public string Repair_Reason { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime Error_Time { get; set; }
        public string Error_Level { get; set; }
        public string Contact { get; set; }
        public string Contact_tel { get; set; }
        public string Reason_Types { get; set; }
        public string Repair_Method { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime? Repair_BeginTime { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime? Repair_EndTime { get; set; }
        public string Repair_Result { get; set; }
        public string Labor_List { get; set; }
        public string Update_Part { get; set; }
        public Nullable<decimal> Labor_Time { get; set; }
        public Nullable<decimal> All_RepairCost { get; set; }
        public int TotalTime { get; set; }
        public List<laborlist> laborlist { get; set; }
        public List<matlist> matlist { get; set; }
        public string Reason_Analysis { get; set; }
        public string Repair_Remark { get; set; }
        public string EQP_Plant_No { get; set; }
        public string Asset { get; set; }
        public string EQP_Location { get; set; }
        public string Mfg_Serial_Num { get; set; }
        public string OP_Types { get; set; }
        public string FunPlant { get; set; }
        public string Process { get; set; }
        public string Class_Desc { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Apply_Time { get; set; }
        public int? Organization_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public  int FunPlant_OrganizationUID { get; set; }
        /// <summary>
        /// 提报人员
        /// </summary>
        public string Mentioner { get; set; }
        //成本中心
        public int? CostCtr_UID { get; set; }
        public string CostCtr_ID { get; set; }
        public string CostCtr_Description { get; set; }
    }

    public class laborlist: EntityDTOBase
    {
        public int Labor_Using_Uid { get; set; }
        public int Repair_Uid { get; set; }
        public int EQPUser_Uid { get; set; }
        public int User_Id { get; set; }
        public string User_Name { get; set; }
    }

    public class matlist : EntityDTOBase
    {
        public int PartsUpdate_Uid { get; set; }
        public int Repair_Uid { get; set; }
        public int Material_Uid { get; set; }
        public string Material_Name { get; set; }
        public string Material_Id { get; set; }
        public int Update_No { get; set; }
        public Nullable<decimal> Update_Cost { get; set; }
        public string Assembly_Name { get; set; }
        public Nullable<decimal> Unit_Price { get; set; }
        public string Reason_Analysis { get; set; }
    }
}
